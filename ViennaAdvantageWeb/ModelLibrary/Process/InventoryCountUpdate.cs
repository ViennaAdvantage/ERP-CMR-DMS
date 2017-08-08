/********************************************************
 * Module  Name   : 
 * Purpose        : Update existing Inventory Count List with current Book value
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Veena        21-Oct-2009
  ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.Model;
using VAdvantage.Logging;
using VAdvantage.DataBase;
using System.Data;
using System.Data.SqlClient;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    /// <summary>
    /// Update existing Inventory Count List with current Book value
    /// </summary>
    public class InventoryCountUpdate : ProcessEngine.SvrProcess
    {
        /** Physical Inventory		*/
        private int _m_Inventory_ID = 0;
        /** Update to What			*/
        private Boolean _inventoryCountSetZero = false;
        private Boolean _AdjustinventoryCount = false;

        /// <summary>
        /// Prepare - e.g., Get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                    ;
                else if (name.Equals("InventoryCountSet"))
                {
                    _inventoryCountSetZero = "Z".Equals(para[i].GetParameter());
                    _AdjustinventoryCount = "A".Equals(para[i].GetParameter());
                }
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
            _m_Inventory_ID = GetRecord_ID();
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <returns>info</returns>
        protected override String DoIt()
        {
            log.Info("M_Inventory_ID=" + _m_Inventory_ID);
            MInventory inventory = new MInventory(GetCtx(), _m_Inventory_ID, Get_TrxName());
            if (inventory.Get_ID() == 0)
                throw new SystemException("Not found: M_Inventory_ID=" + _m_Inventory_ID);

            //	Multiple Lines for one item
            //jz simple the SQL so that Derby also like it. To avoid testing Oracle by now, leave no change for Oracle
            String sql = null;
            if (DataBase.DB.IsOracle())
            {
                sql = "UPDATE M_InventoryLine SET IsActive='N' "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                    + " AND (M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID) IN "
                        + "(SELECT M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID "
                        + "FROM M_InventoryLine "
                        + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                        + " GROUP BY M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID "
                        + "HAVING COUNT(*) > 1)";
            }
            else
            {
                sql = "UPDATE M_InventoryLine SET IsActive='N' "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                    + " AND EXISTS "
                        + "(SELECT COUNT(*) "
                        + "FROM M_InventoryLine "
                        + "WHERE M_Inventory_ID=" + _m_Inventory_ID
                        + " GROUP BY M_Product_ID, M_Locator_ID, M_AttributeSetInstance_ID "
                        + "HAVING COUNT(*) > 1)";
            }
            int multiple = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Info("Multiple=" + multiple);

            int delMA = MInventoryLineMA.DeleteInventoryMA(_m_Inventory_ID, Get_TrxName());
            log.Info("DeletedMA=" + delMA);

            //	ASI
            sql = "UPDATE M_InventoryLine l "
                + "SET (QtyBook,QtyCount) = "
                    + "(SELECT QtyOnHand,QtyOnHand FROM M_Storage s "
                    + "WHERE s.M_Product_ID=l.M_Product_ID AND s.M_Locator_ID=l.M_Locator_ID"
                    + " AND s.M_AttributeSetInstance_ID=l.M_AttributeSetInstance_ID),"
                + " Updated=SysDate,"
                + " UpdatedBy=" + GetAD_User_ID()
                //
                + " WHERE M_Inventory_ID=" + _m_Inventory_ID
                + " AND EXISTS (SELECT * FROM M_Storage s "
                    + "WHERE s.M_Product_ID=l.M_Product_ID AND s.M_Locator_ID=l.M_Locator_ID"
                    + " AND s.M_AttributeSetInstance_ID=l.M_AttributeSetInstance_ID)";
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Info("Update with ASI =" + no);

            //	No ASI
            int noMA = UpdateWithMA();

            //	Set Count to Zero
            if (_inventoryCountSetZero)
            {
                sql = "UPDATE M_InventoryLine l "
                    + "SET QtyCount=0 "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID;
                no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                log.Info("Set Count to Zero =" + no);
            }
            if (_AdjustinventoryCount)
            {
                MInventoryLine[] lines = inventory.GetLines(true);
                for (int i = 0; i < lines.Length; i++)
                {
                    decimal currentQty = 0;
                    string query = "", qry = "";
                    int result = 0;
                    MInventoryLine iLine = lines[i];
                    int M_Product_ID = Utility.Util.GetValueOfInt(iLine.GetM_Product_ID());
                    int M_Locator_ID = Utility.Util.GetValueOfInt(iLine.GetM_Locator_ID());
                    int M_AttributeSetInstance_ID = Util.GetValueOfInt(iLine.GetM_AttributeSetInstance_ID());

                    query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                           AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                    result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                    if (result > 0)
                    {
                        qry = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                        currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                    }
                    else
                    {
                        query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                        result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                        if (result > 0)
                        {
                            qry = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(inventory.GetMovementDate(), true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                            currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                        }
                    }
                    iLine.SetQtyBook(currentQty);
                    iLine.SetOpeningStock(currentQty);
                    if (iLine.GetAdjustmentType() == "A")
                    {
                        iLine.SetDifferenceQty(Util.GetValueOfDecimal(iLine.GetOpeningStock()) - Util.GetValueOfDecimal(iLine.GetAsOnDateCount()));
                    }
                    else if (iLine.GetAdjustmentType() == "D")
                    {
                        iLine.SetAsOnDateCount(Util.GetValueOfDecimal(iLine.GetOpeningStock()) - Util.GetValueOfDecimal(iLine.GetDifferenceQty()));
                    }
                    iLine.SetQtyCount(Util.GetValueOfDecimal(iLine.GetQtyBook()) - Util.GetValueOfDecimal(iLine.GetDifferenceQty()));
                    if (!iLine.Save())
                    {

                    }
                }

                //sql = "UPDATE M_InventoryLine "
                //    + "SET QtyCount = QtyBook - NVL(DifferenceQty,0),OpeningStock = " + currentQty
                //    + "WHERE M_Inventory_ID=" + _m_Inventory_ID;
                //no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                //log.Info("Inventory Adjustment =" + no);
            }

            inventory.SetIsAdjusted(true);
            if (!inventory.Save())
            {

            }
            if (multiple > 0)
                return "@M_InventoryLine_ID@ - #" + (no + noMA) + " --> @InventoryProductMultiple@";

            return "@M_InventoryLine_ID@ - #" + (no + noMA);
        }

        /// <summary>
        /// Update Inventory Lines With Material Allocation
        /// </summary>
        /// <returns>no update</returns>
        private int UpdateWithMA()
        {
            int no = 0;
            //
            String sql = "SELECT * FROM M_InventoryLine WHERE M_Inventory_ID=@iid AND COALESCE(M_AttributeSetInstance_ID,0)=0 ";

            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@iid", _m_Inventory_ID);
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, null);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MInventoryLine il = new MInventoryLine(GetCtx(), dr, Get_TrxName());
                    Decimal onHand = Env.ZERO;
                    MStorage[] storages = MStorage.GetAll(GetCtx(), il.GetM_Product_ID(), il.GetM_Locator_ID(), Get_TrxName());
                    MInventoryLineMA ma = null;
                    for (int i = 0; i < storages.Length; i++)
                    {
                        MStorage storage = storages[i];
                        if (Env.Signum(storage.GetQtyOnHand()) == 0)
                            continue;
                        onHand = Decimal.Add(onHand, storage.GetQtyOnHand());
                        //	No ASI
                        if (storage.GetM_AttributeSetInstance_ID() == 0
                            && storages.Length == 1)
                            continue;
                        //	Save ASI
                        ma = new MInventoryLineMA(il,
                            storage.GetM_AttributeSetInstance_ID(), storage.GetQtyOnHand());
                        if (!ma.Save())
                            ;
                    }
                    il.SetQtyBook(onHand);
                    il.SetQtyCount(onHand);
                    if (il.Save())
                        no++;
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            log.Info("#" + no);
            return no;
        }
    }
}
