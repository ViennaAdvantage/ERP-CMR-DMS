/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_CostDetail
 * Chronological Development
 * Veena Pandey     18-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Process;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCostDetail : X_M_CostDetail
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostDetail).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_CostDetail_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(Ctx ctx, int M_CostDetail_ID, Trx trxName)
            : base(ctx, M_CostDetail_ID, trxName)
        {
            if (M_CostDetail_ID == 0)
            {
                //	setC_AcctSchema_ID (0);
                //	setM_Product_ID (0);
                SetM_AttributeSetInstance_ID(0);
                //	setC_OrderLine_ID (0);
                //	setM_InOutLine_ID(0);
                //	setC_InvoiceLine_ID (0);
                SetProcessed(false);
                SetAmt(Env.ZERO);
                SetQty(Env.ZERO);
                SetIsSOTrx(false);
                SetDeltaAmt(Env.ZERO);
                SetDeltaQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_CostElement_ID">optional cost element for Freight</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        public MCostDetail(MAcctSchema mas, int AD_Org_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
            int M_CostElement_ID, Decimal Amt, Decimal Qty, String Description, Trx trxName)
            : this(mas.GetCtx(), 0, trxName)
        {
            SetClientOrg(mas.GetAD_Client_ID(), AD_Org_ID);
            SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            //
            SetM_CostElement_ID(M_CostElement_ID);
            //
            SetAmt(Amt);
            SetQty(Qty);
            SetDescription(Description);
        }

        /// <summary>
        /// Create New Order Cost Detail for Physical Inventory.
        /// Called from Doc_Inventory
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_InventoryLine_ID">order</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateInventory(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_InventoryLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_InventoryLine_ID=" + M_InventoryLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_InventoryLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                M_InventoryLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_InventoryLine_ID(M_InventoryLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetM_InventoryLine_ID(M_InventoryLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }




            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Invoice Cost Detail for AP Invoices.
        /// Called from Doc_Invoice - for Invoice Adjustments
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="C_InvoiceLine_ID">invoice</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static bool CreateInvoice(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int C_InvoiceLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND C_InvoiceLine_ID=" + C_InvoiceLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "C_InvoiceLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                C_InvoiceLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetC_InvoiceLine_ID(C_InvoiceLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,                              M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetC_InvoiceLine_ID(C_InvoiceLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);

                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Order Cost Detail for Movements.
        /// Called from Doc_Movement
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_MovementLine_ID">movement</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="from">if true the from (reduction)</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateMovement(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_MovementLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, bool from, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_MovementLine_ID=" + M_MovementLine_ID
                + " AND IsSOTrx=" + (from ? "'Y'" : "'N'")
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_MovementLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2 AND IsSOTrx="
                + (from ? "'Y'" : "'N'"),
                M_MovementLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_MovementLine_ID(M_MovementLine_ID);
                cd.SetIsSOTrx(from);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,                        M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetM_MovementLine_ID(M_MovementLine_ID);
                    cd.SetIsSOTrx(from);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Order Cost Detail for Purchase Orders.
        /// Called from Doc_MatchPO
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="C_OrderLine_ID">order</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static bool CreateOrder(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int C_OrderLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {

            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND C_OrderLine_ID=" + C_OrderLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "C_OrderLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                C_OrderLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew cost deatil for selected product
            {
                cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID, Amt, Qty, Description, trxName);
                cd.SetC_OrderLine_ID(C_OrderLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetC_OrderLine_ID(C_OrderLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                {
                    try
                    {
                        cd.Process();
                    }
                    catch { }
                }
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Cost by deleting old cost
        /// </summary>
        /// <param name="cd"></param>
        /// <param name="mas"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_CostElement_ID"></param>
        /// <param name="Amt"></param>
        /// <param name="Qty"></param>
        /// <param name="Description"></param>
        /// <param name="trxName"></param>
        /// <param name="RectifyPostedRecords"></param>
        /// <param name="Id"></param>
        //private static void CostSetByProcess(MCostDetail cd, MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
        //    int M_AttributeSetInstance_ID, int M_CostElement_ID, Decimal Amt, Decimal Qty,
        //    String Description, Trx trx, bool RectifyPostedRecords)
        //{
        //    if (RectifyPostedRecords)
        //    {
        //        cd.SetProcessed(false);
        //        if (cd.Delete(true, trxName))
        //        {
        //            cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
        //                M_CostElement_ID, Amt, Qty, Description, trxName);
        //        }
        //    }
        //}

        /// <summary>
        /// Create New Order Cost Detail for Production.
        ///	Called from Doc_Production
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_ProductionLine_ID">production line</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateProduction(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_ProductionLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_ProductionLine_ID=" + M_ProductionLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_ProductionLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                M_ProductionLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_ProductionLine_ID(M_ProductionLine_ID);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);

                    cd.SetM_ProductionLine_ID(M_ProductionLine_ID);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }


            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Shipment Cost Detail for SO Shipments.
        ///	Called from Doc_MInOut - for SO Shipments
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="M_InOutLine_ID">shipment</param>
        /// <param name="M_CostElement_ID">optional cost element</param>
        /// <param name="Amt">total amount</param>
        /// <param name="Qty">quantity</param>
        /// <param name="Description">optional description</param>
        /// <param name="IsSOTrx">is sales order</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool CreateShipment(MAcctSchema mas, int AD_Org_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, int M_InOutLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, bool IsSOTrx, Trx trxName, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_InOutLine_ID=" + M_InOutLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            if (no != 0)
            {
                _log.Config("Deleted #" + no);
            }
            MCostDetail cd = Get(mas.GetCtx(), "M_InOutLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                M_InOutLine_ID, M_AttributeSetInstance_ID, trxName);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(mas, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID,
                    Amt, Qty, Description, trxName);
                cd.SetM_InOutLine_ID(M_InOutLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trxName))
                    {
                        cd = new MCostDetail(mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trxName);
                    }
                    //CostSetByProcess(cd, mas, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trxName, RectifyPostedRecords);
                    cd.SetM_InOutLine_ID(M_InOutLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
                }
                else
                {
                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }



            bool ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(mas.GetCtx(), mas.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Get Cost Detail
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="whereClause">where clause</param>
        /// <param name="ID">1st parameter</param>
        /// <param name="M_AttributeSetInstance_ID">attribute set instance</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost detail</returns>
        private static MCostDetail Get(Ctx ctx, String whereClause, int ID,
            int M_AttributeSetInstance_ID, Trx trxName)
        {
            String sql = "SELECT * FROM M_CostDetail WHERE " + whereClause;
            MCostDetail retValue = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@param1", ID);
                param[1] = new SqlParameter("@param2", M_AttributeSetInstance_ID);
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MCostDetail(ctx, dr, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql + " - " + ID, e);
            }
            return retValue;
        }

        /// <summary>
        /// Process Cost Details for product
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if no error</returns>
        public static bool ProcessProduct(MProduct product, Trx trxName)
        {
            String sql = "SELECT * FROM M_CostDetail "
                + "WHERE M_Product_ID=" + product.GetM_Product_ID()
                + " AND Processed='N' "
                + "ORDER BY C_AcctSchema_ID, M_CostElement_ID, AD_Org_ID, M_AttributeSetInstance_ID, Created";
            int counterOK = 0;
            int counterError = 0;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MCostDetail cd = new MCostDetail(product.GetCtx(), dr, trxName);
                        if (cd.Process())	//	saves
                            counterOK++;
                        else
                            counterError++;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
                counterError++;
            }

            _log.Config(product.GetValue() + ": OK=" + counterOK + ", Errors=" + counterError);
            return counterError == 0;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new record</param>
        /// <param name="success">success</param>
        /// <returns>true</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>false if processed</returns>
        protected override bool BeforeDelete()
        {
            return !IsProcessed();
        }

        /// <summary>
        /// Is this a Delta Record (previously processed)?
        /// </summary>
        /// <returns>true if delta is not null</returns>
        public bool IsDelta()
        {
            return !(Env.Signum(GetDeltaAmt()) == 0 && Env.Signum(GetDeltaQty()) == 0);
        }

        /// <summary>
        /// Is Invoice
        /// </summary>
        /// <returns>true if invoice line</returns>
        public bool IsInvoice()
        {
            return GetC_InvoiceLine_ID() != 0;
        }

        /// <summary>
        /// Is Order
        /// </summary>
        /// <returns>true if order line</returns>
        public bool IsOrder()
        {
            return GetC_OrderLine_ID() != 0;
        }

        /// <summary>
        /// Is Shipment
        /// </summary>
        /// <returns>true if sales order shipment</returns>
        public bool IsShipment()
        {
            return IsSOTrx() && GetM_InOutLine_ID() != 0;
        }

        /// <summary>
        /// Process Cost Detail Record.
        /// The record is saved if processed.
        /// </summary>
        /// <returns>true if processed</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Process()
        {
            if (IsProcessed())
            {
                log.Info("Already processed");
                return true;
            }
            bool ok = false;

            //	get costing level for product
            MAcctSchema mas = new MAcctSchema(GetCtx(), GetC_AcctSchema_ID(), null);
            String CostingLevel = mas.GetCostingLevel();
            MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());
            MProductCategoryAcct pca = MProductCategoryAcct.Get(GetCtx(),
                product.GetM_Product_Category_ID(), GetC_AcctSchema_ID(), null);
            if (pca.GetCostingLevel() != null)
                CostingLevel = pca.GetCostingLevel();
            //	Org Element
            int Org_ID = GetAD_Org_ID();
            int M_ASI_ID = GetM_AttributeSetInstance_ID();
            if (MAcctSchema.COSTINGLEVEL_Client.Equals(CostingLevel))
            {
                Org_ID = 0;
                M_ASI_ID = 0;
            }
            else if (MAcctSchema.COSTINGLEVEL_Organization.Equals(CostingLevel))
                M_ASI_ID = 0;
            else if (MAcctSchema.COSTINGLEVEL_BatchLot.Equals(CostingLevel))
                Org_ID = 0;

            //	Create Material Cost elements
            if (GetM_CostElement_ID() == 0)
            {
                MCostElement[] ces = MCostElement.GetCostingMethods(this);
                try
                {
                    for (int i = 0; i < ces.Length; i++)
                    {
                        MCostElement ce = ces[i];
                        ok = Process(mas, product, ce, Org_ID, M_ASI_ID);
                        if (!ok)
                            break;
                    }
                }
                catch { }
            }	//	Material Cost elements
            else
            {
                MCostElement ce = MCostElement.Get(GetCtx(), GetM_CostElement_ID());
                ok = Process(mas, product, ce, Org_ID, M_ASI_ID);
            }

            //	Save it
            if (ok)
            {
                SetDeltaAmt(Convert.ToDecimal(null));
                SetDeltaQty(Convert.ToDecimal(null));
                SetProcessed(true);
                ok = Save();
            }
            log.Info(ok + " - " + ToString());
            return ok;
        }

        /// <summary>
        /// Process cost detail for cost record
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="ce">cost element</param>
        /// <param name="Org_ID">org - corrected for costing level</param>
        /// <param name="M_ASI_ID">asi corrected for costing level</param>
        /// <returns>true if cost ok</returns>
        private bool Process(MAcctSchema mas, MProduct product, MCostElement ce,
            int Org_ID, int M_ASI_ID)
        {
            string qry = "";
            if (SaveCost(mas, product, ce, Org_ID, M_ASI_ID, 0))
            {
                if (product.IsOneAssetPerUOM())
                {
                    if (GetC_OrderLine_ID() > 0)
                    {
                        qry = @"SELECT ast.A_Asset_ID from A_Asset ast INNER JOIN M_InOutLine inl ON (ast.M_InOutLine_ID = inl.M_InOutLine_ID)
                            INNER JOIN C_OrderLine odl ON (inl.C_OrderLine_ID = odl.C_OrderLine_ID) Where ast.IsActive='Y' 
                            AND ast.M_Product_ID=" + product.GetM_Product_ID() + " AND inl.C_OrderLine_ID=" + GetC_OrderLine_ID();
                    }
                    else
                    {
                        qry = @"SELECT ast.A_Asset_ID from A_Asset ast INNER JOIN M_InOutLine inl ON (ast.M_InOutLine_ID = inl.M_InOutLine_ID)
                            INNER JOIN C_InvoiceLine inv ON (inv.M_InOutLine_ID = inl.M_InOutLine_ID) WHERE ast.IsActive ='Y' 
                            AND ast.M_Product_ID=" + product.GetM_Product_ID() + " AND inv.C_InvoiceLine_ID =" + GetC_InvoiceLine_ID();
                    }
                    DataSet ds = DB.ExecuteDataset(qry, null, null);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                int A_Asset_ID = Util.GetValueOfInt(ds.Tables[0].Rows[i][0]);
                                if (!SaveCost(mas, product, ce, Org_ID, M_ASI_ID, A_Asset_ID))
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SaveCost(MAcctSchema mas, MProduct product, MCostElement ce, int Org_ID, int M_ASI_ID, int A_Asset_ID)
        {
            MCost cost = null;
            if (A_Asset_ID == 0)
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID());
            }
            else
            {
                cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID(), A_Asset_ID);
            }
            //	if (cost == null)
            //		cost = new MCost(product, M_ASI_ID, 
            //			as, Org_ID, ce.getM_CostElement_ID());

            Decimal qty = GetQty();
            Decimal amt = GetAmt();
            int precision = mas.GetCostingPrecision();

            if (A_Asset_ID > 0)
            {
                qty = 1;
                amt = Decimal.Round(Decimal.Divide(GetAmt(), GetQty()), precision, MidpointRounding.AwayFromZero);

            }

            Decimal price = amt;
            if (Env.Signum(qty) != 0)
                price = Decimal.Round(Decimal.Divide(amt, qty), precision, MidpointRounding.AwayFromZero);


            /** All Costing Methods
            if (ce.isAverageInvoice())
            else if (ce.isAveragePO())
            else if (ce.isFifo())
            else if (ce.isLifo())
            else if (ce.isLastInvoice())
            else if (ce.isLastPOPrice())
            else if (ce.isStandardCosting())
            else if (ce.isUserDefined())
            else if (!ce.isCostingMethod())
            **/

            //	*** Purchase Order Detail Record ***
            if (GetC_OrderLine_ID() != 0)
            {
                MOrderLine oLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), null);
                bool isReturnTrx = Env.Signum(qty) < 0;
                log.Fine(" ");

                if (ce.IsAveragePO())
                {
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    /**********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /**********************************/

                    log.Finer("PO - AveragePO - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {
                    if (!isReturnTrx)
                    {
                        if (Env.Signum(qty) != 0)
                            cost.SetCurrentCostPrice(price);
                        else
                        {
                            Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                            cost.SetCurrentCostPrice(cCosts);
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("PO - LastPO - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    log.Finer("PO - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    log.Finer("PO - " + ce + " - " + cost);
                }
                //	else
                //		log.warning("PO - " + ce + " - " + cost);
            }

            //	*** AP Invoice Detail Record ***
            else if (GetC_InvoiceLine_ID() != 0)
            {
                bool isReturnTrx = Env.Signum(qty) < 0;
                if (ce.IsAverageInvoice())
                {
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    /**********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /**********************************/
                    log.Finer("Inv - AverageInv - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    //	Real ASI - costing level Org
                    MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                        mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
                    cq.SetCosts(amt, qty, precision);
                    cq.Save(Get_TrxName());
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    cost.Add(amt, qty);
                    log.Finer("Inv - FiFo/LiFo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {
                    if (!isReturnTrx)
                    {
                        if (Env.Signum(qty) != 0)
                            cost.SetCurrentCostPrice(price);
                        else
                        {
                            Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                            cost.SetCurrentCostPrice(cCosts);
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - LastInv - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                    {
                        cost.SetCurrentCostPrice(price);
                        //	seed initial price
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                        {
                            cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                    mas, Org_ID, ce.GetCostingMethod(), GetC_OrderLine_ID()));
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    cost.Add(amt, qty);
                    log.Finer("Inv - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())		//	Cost Adjustments
                {
                    //Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    //cost.SetCurrentCostPrice(cCosts);
                    //cost.Add(amt, qty);
                    //log.Finer("Inv - none - " + cost);
                    Decimal cCosts = Decimal.Add(Decimal.Multiply(cost.GetCurrentCostPrice(), cost.GetCurrentQty()), amt);
                    Decimal qty1 = Decimal.Add(cost.GetCurrentQty(), qty);
                    if (qty1.CompareTo(Decimal.Zero) == 0)
                    {
                        qty1 = Decimal.One;
                    }
                    cCosts = Decimal.Round(Decimal.Divide(cCosts, qty1), precision, MidpointRounding.AwayFromZero);
                    cost.SetCurrentCostPrice(cCosts);
                    cost.Add(amt, qty);
                    log.Finer("Inv - none - " + cost);
                }
                //	else
                //		log.warning("Inv - " + ce + " - " + cost);
            }
            //	*** Qty Adjustment Detail Record ***
            else if (GetM_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetM_MovementLine_ID() != 0
                || GetM_InventoryLine_ID() != 0
                || GetM_ProductionLine_ID() != 0
                || GetC_ProjectIssue_ID() != 0
                || GetM_WorkOrderTransactionLine_ID() != 0
                || GetM_WorkOrderResourceTxnLine_ID() != 0)
            {
                bool addition = Env.Signum(qty) > 0;
                //
                if (ce.IsAverageInvoice())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    /**********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /**********************************/
                    log.Finer("QtyAdjust - AverageInv - " + cost);
                }
                else if (ce.IsAveragePO())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));

                    /**********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /**********************************/

                    log.Finer("QtyAdjust - AveragePO - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    if (addition)
                    {
                        //	Real ASI - costing level Org
                        MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                            mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
                        cq.SetCosts(amt, qty, precision);
                        cq.Save();
                    }
                    else
                    {
                        //	Adjust Queue - costing level Org/ASI
                        MCostQueue.AdjustQty(product, M_ASI_ID,
                            mas, Org_ID, ce, Decimal.Negate(qty), Get_TrxName());
                    }
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - FiFo/Lifo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastInv - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastPO - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        //	Initial
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0
                            && cost.Get_ID() == 0)
                            cost.SetCurrentCostPrice(price);
                    }
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    if (addition)
                        cost.Add(amt, qty);
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    //	Should not happen
                    log.Finer("QtyAdjust - ?none? - " + cost);
                }
                else
                {
                    log.Warning("QtyAdjust - " + ce + " - " + cost);
                }
            }
            else	//	unknown or no id
            {
                log.Warning("Unknown Type: " + ToString());
                return false;
            }
            if (A_Asset_ID != 0)
            {
                cost.SetIsAssetCost(true);
            }
            return cost.Save();
        }

        /// <summary>
        /// Set Amt
        /// </summary>
        /// <param name="amt">amount</param>
        public new void SetAmt(Decimal? amt)
        {
            if (IsProcessed())
                throw new Exception("Cannot change Amt - processed");
            if (amt == null)
                base.SetAmt(Env.ZERO);
            else
                base.SetAmt(amt);
        }

        /// <summary>
        /// Set Qty
        /// </summary>
        /// <param name="qty">quantity</param>
        public new void SetQty(Decimal? qty)
        {
            if (IsProcessed())
                throw new Exception("Cannot change Qty - processed");
            if (qty == null)
                base.SetQty(Env.ZERO);
            else
                base.SetQty(qty);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MCostDetail[");
            sb.Append(Get_ID());
            if (GetC_OrderLine_ID() != 0)
                sb.Append(",C_OrderLine_ID=").Append(GetC_OrderLine_ID());
            if (GetM_InOutLine_ID() != 0)
                sb.Append(",M_InOutLine_ID=").Append(GetM_InOutLine_ID());
            if (GetC_InvoiceLine_ID() != 0)
                sb.Append(",C_InvoiceLine_ID=").Append(GetC_InvoiceLine_ID());
            if (GetC_ProjectIssue_ID() != 0)
                sb.Append(",C_ProjectIssue_ID=").Append(GetC_ProjectIssue_ID());
            if (GetM_MovementLine_ID() != 0)
                sb.Append(",M_MovementLine_ID=").Append(GetM_MovementLine_ID());
            if (GetM_InventoryLine_ID() != 0)
                sb.Append(",M_InventoryLine_ID=").Append(GetM_InventoryLine_ID());
            if (GetM_ProductionLine_ID() != 0)
                sb.Append(",M_ProductionLine_ID=").Append(GetM_ProductionLine_ID());
            sb.Append(",Amt=").Append(GetAmt())
                .Append(",Qty=").Append(GetQty());
            if (IsDelta())
                sb.Append(",DeltaAmt=").Append(GetDeltaAmt())
                    .Append(",DeltaQty=").Append(GetDeltaQty());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Process Reversed cost detail for cost record
        /// </summary>
        /// <param name="mas">accounting schema</param>
        /// <param name="product">product</param>
        /// <param name="ce">cost element</param>
        /// <param name="Org_ID">org - corrected for costing level</param>
        /// <param name="M_ASI_ID">asi corrected for costing level</param>
        /// <returns>true if cost ok</returns>
        public bool ProcessReversedCost(VAdvantage.Model.MAcctSchema mas, MProduct product, VAdvantage.Model.MCostElement ce,
            int Org_ID, int M_ASI_ID)
        {
            MCost cost = MCost.Get(product, M_ASI_ID, mas, Org_ID, ce.GetM_CostElement_ID());
            //	if (cost == null)
            //		cost = new MCost(product, M_ASI_ID, 
            //			as1, Org_ID, ce.getM_CostElement_ID());

            Decimal qty = GetQty();
            Decimal amt = GetAmt();
            int precision = mas.GetCostingPrecision();
            Decimal price = amt;
            //if (Env.Signum(qty) != 0)
            //    price = Decimal.Round(Decimal.Divide(amt, qty), precision, MidpointRounding.AwayFromZero);


            /** All Costing Methods
            if (ce.isAverageInvoice())
            else if (ce.isAveragePO())
            else if (ce.isFifo())
            else if (ce.isLifo())
            else if (ce.isLastInvoice())
            else if (ce.isLastPOPrice())
            else if (ce.isStandardCosting())
            else if (ce.isUserDefined())
            else if (!ce.isCostingMethod())
            **/

            //	*** Purchase Order Detail Record ***
            if (GetC_OrderLine_ID() != 0)
            {
                MOrderLine oLine = new MOrderLine(GetCtx(), GetC_OrderLine_ID(), null);
                bool isReturnTrx = Env.Signum(qty) < 0;
                log.Fine(" ");

                if (ce.IsAveragePO())
                {
                    /*********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /*********************************/
                    //if (!isReturnTrx)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.Add(amt, qty);

                    log.Finer("VAdvantage.Model.PO - AveragePO - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {


                    string lastPrice = "select round(amt/ qty,6) as Price from m_costdetail where m_product_id="
                        + cost.GetM_Product_ID() + " and c_orderline_id is NOT NULL and c_orderline_id<> @param1"
                        + " ORDER BY m_costdetail_id DESC";
                    cost.SetCurrentCostPrice(DB.GetSQLValueBD(Get_TrxName(), lastPrice, GetC_OrderLine_ID()));

                    cost.Add(amt, qty);


                    //if (!isReturnTrx)
                    //{
                    //    if (Env.Signum(qty) != 0)
                    //        cost.SetCurrentCostPrice(price);
                    //    else
                    //    {
                    //        Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    //        cost.SetCurrentCostPrice(cCosts);
                    //    }
                    //}

                    log.Finer("VAdvantage.Model.PO - LastPO - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    log.Finer("VAdvantage.Model.PO - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    log.Finer("VAdvantage.Model.PO - " + ce + " - " + cost);
                }
                //	else
                //		log.warning("VAdvantage.Model.PO - " + ce + " - " + cost);
            }

            //	*** AP Invoice Detail Record ***
            else if (GetC_InvoiceLine_ID() != 0)
            {
                bool isReturnTrx = Env.Signum(qty) < 0;
                if (ce.IsAverageInvoice())
                {
                    /*********************************/
                    cost.Add(amt, qty);
                    if (Env.Signum(cost.GetCumulatedQty()) != 0)
                    {
                        price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                    }
                    cost.SetCurrentCostPrice(price);
                    /*********************************/

                    //if (!isReturnTrx)
                    //{
                    //cost.SetWeightedAverage(amt, qty);
                    //}
                    //else
                    //{
                    //    cost.Add(amt, qty);
                    //}
                    log.Finer("Inv - AverageInv - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    //	Real ASI - costing level Org
                    MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                        mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
                    cq.SetCosts(amt, qty, precision);
                    cq.Save();
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                    {
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - FiFo/LiFo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {

                    string lastPrice = "select round(amt/ qty,6) as Price from m_costdetail where m_product_id="
                        + cost.GetM_Product_ID() + " and c_invoiceline_id is NOT NULL and c_invoiceline_id<> @param1"
                        + " ORDER BY m_costdetail_id DESC";
                    cost.SetCurrentCostPrice(DB.GetSQLValueBD(Get_TrxName(), lastPrice, GetC_InvoiceLine_ID()));

                    cost.Add(amt, qty);
                    //if (!isReturnTrx)
                    //{
                    //    if (Env.Signum(qty) != 0)
                    //        cost.SetCurrentCostPrice(price);
                    //    else
                    //    {
                    //        Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    //        cost.SetCurrentCostPrice(cCosts);
                    //    }
                    //}
                    log.Finer("Inv - LastInv - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (Env.Signum(cost.GetCurrentCostPrice()) == 0)
                    {
                        cost.SetCurrentCostPrice(price);
                        //	seed initial price
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0 && cost.Get_ID() == 0)
                        {
                            cost.SetCurrentCostPrice(MCost.GetSeedCosts(product, M_ASI_ID,
                                    mas, Org_ID, ce.GetCostingMethod(), GetC_OrderLine_ID()));
                        }
                    }
                    cost.Add(amt, qty);
                    log.Finer("Inv - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    cost.Add(amt, qty);
                    log.Finer("Inv - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())		//	Cost Adjustments
                {
                    Decimal cCosts = Decimal.Add(cost.GetCurrentCostPrice(), amt);
                    cost.SetCurrentCostPrice(cCosts);
                    cost.Add(amt, qty);
                    log.Finer("Inv - none - " + cost);
                }
                //	else
                //		log.warning("Inv - " + ce + " - " + cost);

            }
            //	*** Qty Adjustment Detail Record ***
            else if (GetM_InOutLine_ID() != 0 		//	AR Shipment Detail Record  
                || GetM_MovementLine_ID() != 0
                || GetM_InventoryLine_ID() != 0
                || GetM_ProductionLine_ID() != 0
                || GetC_ProjectIssue_ID() != 0
                || GetM_WorkOrderTransactionLine_ID() != 0
                || GetM_WorkOrderResourceTxnLine_ID() != 0)
            {
                bool addition = Env.Signum(qty) > 0;
                //
                if (ce.IsAverageInvoice())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    /*********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /*********************************/
                    log.Finer("QtyAdjust - AverageInv - " + cost);
                }
                else if (ce.IsAveragePO())
                {
                    //if (addition)
                    //    cost.SetWeightedAverage(amt, qty);
                    //else
                    //    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    /*********************************/
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        if (Env.Signum(cost.GetCumulatedQty()) != 0)
                        {
                            price = Decimal.Round(Decimal.Divide(cost.GetCumulatedAmt(), cost.GetCumulatedQty()), precision, MidpointRounding.AwayFromZero);
                        }
                        cost.SetCurrentCostPrice(price);
                    }
                    else
                    {
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    }
                    /*********************************/
                    log.Finer("QtyAdjust - AveragePO - " + cost);
                }
                else if (ce.IsFifo() || ce.IsLifo())
                {
                    if (addition)
                    {
                        //	Real ASI - costing level Org
                        MCostQueue cq = MCostQueue.Get(product, GetM_AttributeSetInstance_ID(),
                            mas, Org_ID, ce.GetM_CostElement_ID(), Get_TrxName());
                        cq.SetCosts(amt, qty, precision);
                        cq.Save();
                    }
                    else
                    {
                        //	Adjust Queue - costing level Org/ASI
                        MCostQueue.AdjustQty(product, M_ASI_ID,
                            mas, Org_ID, ce, Decimal.Negate(qty), Get_TrxName());
                    }
                    //	Get Costs - costing level Org/ASI
                    MCostQueue[] cQueue = MCostQueue.GetQueue(product, M_ASI_ID,
                        mas, Org_ID, ce, Get_TrxName());
                    if (cQueue != null && cQueue.Length > 0)
                        cost.SetCurrentCostPrice(cQueue[0].GetCurrentCostPrice());
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - FiFo/Lifo - " + cost);
                }
                else if (ce.IsLastInvoice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastInv - " + cost);
                }
                else if (ce.IsLastPOPrice())
                {
                    cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - LastPO - " + cost);
                }
                else if (ce.IsStandardCosting())
                {
                    if (addition)
                    {
                        cost.Add(amt, qty);
                        //	Initial
                        if (Env.Signum(cost.GetCurrentCostPrice()) == 0
                            && cost.Get_ID() == 0)
                            cost.SetCurrentCostPrice(price);
                    }
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - Standard - " + cost);
                }
                else if (ce.IsUserDefined())
                {
                    //	Interface
                    if (addition)
                        cost.Add(amt, qty);
                    else
                        cost.SetCurrentQty(Decimal.Add(cost.GetCurrentQty(), qty));
                    log.Finer("QtyAdjust - UserDef - " + cost);
                }
                else if (!ce.IsCostingMethod())
                {
                    //	Should not happen
                    log.Finer("QtyAdjust - ?none? - " + cost);
                }
                else
                {
                    log.Warning("QtyAdjust - " + ce + " - " + cost);
                }
            }
            else	//	unknown or no id
            {
                log.Warning("Unknown Type: " + ToString());
                return false;
            }

            if (cost.GetCurrentQty() == 0)
            {
                cost.SetCurrentCostPrice(0);
            }
            return cost.Save();
        }


        /**
       * 	Create New Work Order Resource Transaction Cost detail
       * 	Called from Doc_WorkOrderTransaction - for Work Order Transactions  
       *	@param as1 accounting schema
       *	@param AD_Org_ID org
       *	@param M_Product_ID product
       *	@param M_AttributeSetInstance_ID asi
       *	@param M_InOutLine_ID shipment
       *	@param M_CostElement_ID optional cost element for Freight
       *	@param Amt amt
       *	@param Qty qty
       *	@param Description optional description
       *	@param IsSOTrx sales order
       *	@param trx transaction
       *	@return true if no error
       */
        public static Boolean CreateWorkOrderResourceTransaction(MAcctSchema as1, int AD_Org_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID,
            int M_WorkOrderResourceTransactionLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_WorkOrderResourceTxnLine_ID= " + M_WorkOrderResourceTransactionLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
                + " AND C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID();


            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);
            MCostDetail cd = Get(as1.GetCtx(), "M_WorkOrderResourceTxnLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                    M_WorkOrderResourceTransactionLine_ID, M_AttributeSetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, AD_Org_ID,
                    M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx);
                cd.SetM_WorkOrderResourceTxnLine_ID(M_WorkOrderResourceTransactionLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trx))
                    {
                        cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trx);
                    }
                    // CostSetByProcess(cd, as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx, RectifyPostedRecords);

                    cd.SetM_WorkOrderResourceTxnLine_ID(M_WorkOrderResourceTransactionLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);
                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }



            Boolean ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(as1.GetCtx(), as1.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

        /// <summary>
        /// Create New Work Order Transaction Cost detail
        /// Called from Doc_WorkOrderTransaction - for Work Order Transactions  
        /// </summary>
        /// <param name="as1"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="M_WorkOrderTransactionLine_ID"></param>
        /// <param name="M_CostElement_ID">optional cost element for Freight</param>
        /// <param name="Amt">amt</param>
        /// <param name="Qty">qty</param>
        /// <param name="Description">optional description</param>
        /// <param name="IsSOTrx">sales order</param>
        /// <param name="trx">transaction</param>
        /// <returns>true if no error</returns>
        public static Boolean CreateWorkOrderTransaction(MAcctSchema as1, int AD_Org_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID,
            int M_WorkOrderTransactionLine_ID, int M_CostElement_ID,
            Decimal Amt, Decimal Qty, String Description, Boolean IsSOTrx, Trx trx, bool RectifyPostedRecords)
        {
            //	Delete Unprocessed zero Differences
            String sql = "DELETE FROM M_CostDetail "
                + "WHERE Processed='N' AND COALESCE(DeltaAmt,0)=0 AND COALESCE(DeltaQty,0)=0"
                + " AND M_WorkOrderTransactionLine_ID= " + M_WorkOrderTransactionLine_ID
                + " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
                + " AND C_AcctSchema_ID = " + as1.GetC_AcctSchema_ID()
                + " AND M_CostElement_ID = " + M_CostElement_ID;

            int no = DB.ExecuteQuery(sql, null, trx);
            if (no != 0)
                _log.Config("Deleted #" + no);

            MCostDetail cd = Get(as1.GetCtx(), "M_WorkOrderTransactionLine_ID=@param1 AND M_AttributeSetInstance_ID=@param2",
                    M_WorkOrderTransactionLine_ID, M_AttributeSetInstance_ID, trx);
            //
            if (cd == null)		//	createNew
            {
                cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    M_CostElement_ID, Amt, Qty, Description, trx);

                cd.SetM_WorkOrderTransactionLine_ID(M_WorkOrderTransactionLine_ID);
                cd.SetIsSOTrx(IsSOTrx);
            }
            else
            {
                if (RectifyPostedRecords)
                {
                    cd.SetProcessed(false);
                    if (cd.Delete(true, trx))
                    {
                        cd = new MCostDetail(as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            M_CostElement_ID, Amt, Qty, Description, trx);
                    }
                    //CostSetByProcess(cd, as1, AD_Org_ID, M_Product_ID, M_AttributeSetInstance_ID, M_CostElement_ID, Amt, Qty, Description, trx, RectifyPostedRecords);

                    cd.SetM_WorkOrderTransactionLine_ID(M_WorkOrderTransactionLine_ID);
                    cd.SetIsSOTrx(IsSOTrx);
                    /*****************************************/
                    cd.SetC_AcctSchema_ID(as1.GetC_AcctSchema_ID());
                    cd.SetM_Product_ID(M_Product_ID);

                }
                else
                {

                    cd.SetDeltaAmt(Decimal.Subtract(cd.GetAmt(), Amt));
                    cd.SetDeltaQty(Decimal.Subtract(cd.GetQty(), Qty));
                    if (cd.IsDelta())
                        cd.SetProcessed(false);
                    else
                        return true;	//	nothing to do
                }
            }

            Boolean ok = cd.Save();
            if (ok && !cd.IsProcessed())
            {
                MClient client = MClient.Get(as1.GetCtx(), as1.GetAD_Client_ID());
                if (client.IsCostImmediate())
                    cd.Process();
            }
            _log.Config("(" + ok + ") " + cd);
            return ok;
        }

    }
}