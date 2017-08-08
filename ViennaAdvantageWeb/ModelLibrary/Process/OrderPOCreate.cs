/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : OrderPOCreate
 * Purpose        : Generate PO from Sales Order
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     03-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class OrderPOCreate : ProcessEngine.SvrProcess
    {
        #region Private Variables  
        //Order Date From	
        private DateTime? _DateOrdered_From = null;
        //Order Date To		
        private DateTime? _DateOrdered_To = null;
        //Customer			
        private int _C_BPartner_ID;
        //Vendor			
        private int _Vendor_ID;
        //Sales Order		
        private int _C_Order_ID;
        //Drop Ship			
        private String _IsDropShip;
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("DateOrdered"))
                {
                    _DateOrdered_From = (DateTime?)para[i].GetParameter();
                    _DateOrdered_To = (DateTime?)para[i].GetParameter_To();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("Vendor_ID"))
                {
                    _Vendor_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = Utility.Util.GetValueOfInt(para[i].GetParameter());//.intValue();
                }
                else if (name.Equals("IsDropShip"))
                {
                    _IsDropShip = (String)para[i].GetParameter();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message </returns>
        protected override String DoIt()
        {
            log.Info("DateOrdered=" + _DateOrdered_From + " - " + _DateOrdered_To
                + " - C_BPartner_ID=" + _C_BPartner_ID + " - Vendor_ID=" + _Vendor_ID
                + " - IsDropShip=" + _IsDropShip + " - C_Order_ID=" + _C_Order_ID);
            if (_C_Order_ID == 0 && _IsDropShip == null
                && _DateOrdered_From == null && _DateOrdered_To == null
                && _C_BPartner_ID == 0 && _Vendor_ID == 0)
            {
                throw new Exception("You need to restrict selection");
            }
            //
            String sql = "SELECT * FROM C_Order o "
                + "WHERE o.IsSOTrx='Y'"
                //	No Duplicates
                //	" AND o.Ref_Order_ID IS NULL"
                + " AND NOT EXISTS (SELECT * FROM C_OrderLine ol WHERE o.C_Order_ID=ol.C_Order_ID AND ol.Ref_OrderLine_ID IS NOT NULL)"
                ;
            if (_C_Order_ID != 0)
            {
                sql += " AND o.C_Order_ID=" + _C_Order_ID;
            }
            else
            {
                if (_C_BPartner_ID != 0)
                {
                    sql += " AND o.C_BPartner_ID=" + _C_BPartner_ID;
                }
                if (_IsDropShip != null)
                {
                    sql += " AND o.IsDropShip=" + _IsDropShip;
                }
                if (_Vendor_ID != 0)
                {
                    sql += " AND EXISTS (SELECT * FROM C_OrderLine ol"
                        + " INNER JOIN M_Product_PO po ON (ol.M_Product_ID=po.M_Product_ID) "
                            + "WHERE o.C_Order_ID=ol.C_Order_ID AND po.C_BPartner_ID=" + _Vendor_ID + ")";
                }
                if (_DateOrdered_From != null && _DateOrdered_To != null)
                {
                    sql += "AND TRUNC(o.DateOrdered,'DD') BETWEEN '" + _DateOrdered_From + "' AND '" + _DateOrdered_To + "'";
                }
                else if (_DateOrdered_From != null && _DateOrdered_To == null)
                {
                    sql += "AND TRUNC(o.DateOrdered,'DD') >= '" + _DateOrdered_From + "'";
                }
                else if (_DateOrdered_From == null && _DateOrdered_To != null)
                {
                    sql += "AND TRUNC(o.DateOrdered,'DD') <= '" + _DateOrdered_To + "'";
                }
            }
            DataTable dt = null;
            IDataReader idr = null;
            int counter = 0;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    counter += CreatePOFromSO(new MOrder(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            if (counter == 0)
            {
                log.Fine(sql);
            }
            return "@Created@ " + counter;
        }

        /// <summary>
        /// Create PO From SO
        /// </summary>
        /// <param name="so">sales order</param>
        /// <returns>number of POs created</returns>
        private int CreatePOFromSO(MOrder so)
        {
            log.Info(so.ToString());
            MOrderLine[] soLines = so.GetLines(true, null);
            if (soLines == null || soLines.Length == 0)
            {
                log.Warning("No Lines - " + so);
                return 0;
            }
            //
            int counter = 0;
            //	Order Lines with a Product which has a current vendor 
            String sql = "SELECT DISTINCT po.C_BPartner_ID, po.M_Product_ID "
                + "FROM M_Product_PO po"
                + " INNER JOIN C_OrderLine ol ON (po.M_Product_ID=ol.M_Product_ID) "
                + "WHERE ol.C_Order_ID=" + so.GetC_Order_ID() + " AND po.IsCurrentVendor='Y' "
                + "ORDER BY 1";
            IDataReader idr = null;
            MOrder po = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (idr.Read())
                {
                    //	New Order
                    int C_BPartner_ID = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                    if (po == null || po.GetBill_BPartner_ID() != C_BPartner_ID)
                    {
                        po = CreatePOForVendor(Utility.Util.GetValueOfInt(idr[0]), so);
                        AddLog(0, null, null, po.GetDocumentNo());
                        counter++;
                    }

                    //	Line
                    int M_Product_ID = Utility.Util.GetValueOfInt(idr[1]);//.getInt(2);
                    for (int i = 0; i < soLines.Length; i++)
                    {
                        if (soLines[i].GetM_Product_ID() == M_Product_ID)
                        {
                            MOrderLine poLine = new MOrderLine(po);
                            poLine.SetRef_OrderLine_ID(soLines[i].GetC_OrderLine_ID());
                            poLine.SetM_Product_ID(soLines[i].GetM_Product_ID());
                            poLine.SetM_AttributeSetInstance_ID(soLines[i].GetM_AttributeSetInstance_ID());
                            poLine.SetC_UOM_ID(soLines[i].GetC_UOM_ID());
                            poLine.SetQtyEntered(soLines[i].GetQtyEntered());
                            poLine.SetQtyOrdered(soLines[i].GetQtyOrdered());
                            poLine.SetDescription(soLines[i].GetDescription());
                            poLine.SetDatePromised(soLines[i].GetDatePromised());
                            poLine.SetPrice();
                            poLine.Save();
                        }
                    }

                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            

            //	Set Reference to PO
            if (counter == 1 && po != null)
            {
                so.SetRef_Order_ID(po.GetC_Order_ID());
                so.Save();
            }
            return counter;
        }

        /// <summary>
        /// Create PO for Vendor
        /// </summary>
        /// <param name="C_BPartner_ID">vendor</param>
        /// <param name="so">sales order</param>
        /// <returns>MOrder</returns>
        public MOrder CreatePOForVendor(int C_BPartner_ID, MOrder so)
        {
            MOrder po = new MOrder(GetCtx(), 0, Get_TrxName());
            po.SetClientOrg(so.GetAD_Client_ID(), so.GetAD_Org_ID());
            po.SetRef_Order_ID(so.GetC_Order_ID());
            po.SetIsSOTrx(false);
            po.SetC_DocTypeTarget_ID();
            //
            po.SetDescription(so.GetDescription());
            po.SetPOReference(so.GetDocumentNo());
            po.SetPriorityRule(so.GetPriorityRule());
            po.SetSalesRep_ID(so.GetSalesRep_ID());
            po.SetM_Warehouse_ID(so.GetM_Warehouse_ID());
            //	Set Vendor
            MBPartner vendor = new MBPartner(GetCtx(), C_BPartner_ID, Get_TrxName());
            po.SetBPartner(vendor);
            //	Drop Ship
            po.SetIsDropShip(so.IsDropShip());
            if (so.IsDropShip())
            {
                po.SetShip_BPartner_ID(so.GetC_BPartner_ID());
                po.SetShip_Location_ID(so.GetC_BPartner_Location_ID());
                po.SetShip_User_ID(so.GetAD_User_ID());
            }
            //	References
            po.SetC_Activity_ID(so.GetC_Activity_ID());
            po.SetC_Campaign_ID(so.GetC_Campaign_ID());
            po.SetC_Project_ID(so.GetC_Project_ID());
            po.SetUser1_ID(so.GetUser1_ID());
            po.SetUser2_ID(so.GetUser2_ID());
            //
            po.Save();
            return po;
        }
    }
}
