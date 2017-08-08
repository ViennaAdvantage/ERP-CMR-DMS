/********************************************************
 * Module  Name   : 
 * Purpose        : Create Inventory Count List with current Book value
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

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    /// <summary>
    /// Create Inventory Count List with current Book value
    /// </summary>
    public class InventoryCountCreate : ProcessEngine.SvrProcess
    {
        /** Physical Inventory Parameter		*/
        private int _m_Inventory_ID = 0;
        /** Physical Inventory					*/
        private MInventory _inventory = null;
        /** Locator Parameter			*/
        private int _m_Locator_ID = 0;
        /** Locator Parameter			*/
        private String _locatorValue = null;
        /** Product Parameter			*/
        private String _productValue = null;
        /** Product Category Parameter	*/
        private int _m_Product_Category_ID = 0;
        /** Qty Range Parameter			*/
        private String _qtyRange = null;
        /** Update to What			*/
        private Boolean _inventoryCountSetZero = false;
        /** Delete Parameter			*/
        private Boolean _deleteOld = false;

        /** Inventory Line				*/
        private MInventoryLine _line = null;

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
                else if (name.Equals("M_Locator_ID"))
                    _m_Locator_ID = para[i].GetParameterAsInt();
                else if (name.Equals("LocatorValue"))
                    _locatorValue = (String)para[i].GetParameter();
                else if (name.Equals("ProductValue"))
                    _productValue = (String)para[i].GetParameter();
                else if (name.Equals("M_Product_Category_ID"))
                    _m_Product_Category_ID = para[i].GetParameterAsInt();
                else if (name.Equals("QtyRange"))
                    _qtyRange = (String)para[i].GetParameter();
                else if (name.Equals("InventoryCountSet"))
                    _inventoryCountSetZero = "Z".Equals(para[i].GetParameter());
                else if (name.Equals("DeleteOld"))
                    _deleteOld = "Y".Equals(para[i].GetParameter());
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
            log.Info("M_Inventory_ID=" + _m_Inventory_ID
                + ", M_Locator_ID=" + _m_Locator_ID + ", LocatorValue=" + _locatorValue
                + ", ProductValue=" + _productValue
                + ", M_Product_Category_ID=" + _m_Product_Category_ID
                + ", QtyRange=" + _qtyRange + ", DeleteOld=" + _deleteOld);
            _inventory = new MInventory(GetCtx(), _m_Inventory_ID, Get_TrxName());
            if (_inventory.Get_ID() == 0)
                throw new SystemException("Not found: M_Inventory_ID=" + _m_Inventory_ID);
            if (_inventory.IsProcessed())
                throw new SystemException("@M_Inventory_ID@ @Processed@");
            //
            String sqlQry = "";
            if (_deleteOld)
            {
                sqlQry = "DELETE FROM M_InventoryLine WHERE Processed='N' "
                    + "AND M_Inventory_ID=" + _m_Inventory_ID;
                int no = DataBase.DB.ExecuteQuery(sqlQry, null, Get_TrxName());
                log.Fine("doIt - Deleted #" + no);
            }

            //	Create Null Storage records
            //if (_qtyRange != null && _qtyRange.Equals("="))
            //{
            //    sqlQry = "INSERT INTO M_Storage "
            //        + "(AD_Client_ID, AD_Org_ID, IsActive, Created, CreatedBy, Updated, UpdatedBy,"
            //        + " M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,"
            //        + " qtyOnHand, QtyReserved, QtyOrdered, DateLastInventory) "
            //        + "SELECT l.AD_CLIENT_ID, l.AD_ORG_ID, 'Y', SysDate, 0,SysDate, 0,"
            //        + " l.M_Locator_ID, p.M_Product_ID, 0,"
            //        + " 0,0,0,null "
            //        + "FROM M_Locator l"
            //        + " INNER JOIN M_Product p ON (l.AD_Client_ID=p.AD_Client_ID) "
            //        + "WHERE l.M_Warehouse_ID=" + _inventory.GetM_Warehouse_ID();
            //    if (_m_Locator_ID != 0)
            //        sqlQry += " AND l.M_Locator_ID=" + _m_Locator_ID;
            //    sqlQry += " AND l.IsDefault='Y'"
            //        + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'"
            //        + " AND NOT EXISTS (SELECT * FROM M_Storage s"
            //            + " INNER JOIN M_Locator sl ON (s.M_Locator_ID=sl.M_Locator_ID) "
            //            + "WHERE sl.M_Warehouse_ID=l.M_Warehouse_ID"
            //            + " AND s.M_Product_ID=p.M_Product_ID)";
            //    int no = DataBase.DB.ExecuteQuery(sqlQry, null, Get_TrxName());
            //    log.Fine("'0' Inserted #" + no);
            //}

            StringBuilder sql = new StringBuilder(
                "SELECT s.M_Product_ID, s.M_Locator_ID, s.M_AttributeSetInstance_ID,"
                + " s.qtyOnHand, p.M_AttributeSet_ID "
                + "FROM M_Product p"
                + " INNER JOIN M_Storage s ON (s.M_Product_ID=p.M_Product_ID)"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
                + "WHERE l.M_Warehouse_ID=" + _inventory.GetM_Warehouse_ID()
                + " AND p.IsActive='Y' AND p.IsStocked='Y' and p.ProductType='I'");
            //
            if (_m_Locator_ID != 0)
                sql.Append(" AND s.M_Locator_ID=" + _m_Locator_ID);
            //
            if (_locatorValue != null &&
                (_locatorValue.Trim().Length == 0 || _locatorValue.Equals("%")))
                _locatorValue = null;
            if (_locatorValue != null)
                sql.Append(" AND UPPER(l.Value) LIKE '" + _locatorValue.ToUpper() + "'");
            //
            if (_productValue != null &&
                (_productValue.Trim().Length == 0 || _productValue.Equals("%")))
                _productValue = null;
            if (_productValue != null)
                sql.Append(" AND UPPER(p.Value) LIKE '" + _productValue.ToUpper() + "'");
            //
            if (_m_Product_Category_ID != 0)
                sql.Append(" AND p.M_Product_Category_ID=" + _m_Product_Category_ID);

            //	Do not overwrite existing records
            if (!_deleteOld)
                sql.Append(" AND NOT EXISTS (SELECT * FROM M_InventoryLine il "
                + "WHERE il.M_Inventory_ID=" + _m_Inventory_ID
                + " AND il.M_Product_ID=s.M_Product_ID"
                + " AND il.M_Locator_ID=s.M_Locator_ID"
                + " AND COALESCE(il.M_AttributeSetInstance_ID,0)=COALESCE(s.M_AttributeSetInstance_ID,0))");
            //	+ " AND il.M_AttributeSetInstance_ID=s.M_AttributeSetInstance_ID)");
            //
            sql.Append(" ORDER BY l.Value, p.Value, s.qtyOnHand DESC");	//	Locator/Product
            //
            int count = 0;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                while (idr.Read())
                {
                    decimal currentQty = 0;
                    string query = "", qry = "";
                    int result = 0;
                    int M_Product_ID = Utility.Util.GetValueOfInt(idr[0]);
                    int M_Locator_ID = Utility.Util.GetValueOfInt(idr[1]);
                    int M_AttributeSetInstance_ID = Utility.Util.GetValueOfInt(idr[2]);
                    Decimal qtyOnHand = Utility.Util.GetValueOfDecimal(idr[3]);
                    int M_AttributeSet_ID = Utility.Util.GetValueOfInt(idr[4]);

                    query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) + @" 
                           AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                    result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                    if (result > 0)
                    {
                        qry = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                        currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                    }
                    else
                    {
                        query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                        result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                        if (result > 0)
                        {
                            qry = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(_inventory.GetMovementDate(), true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                            currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(qry));
                        }

                    }
                    int compare = currentQty.CompareTo(Env.ZERO);
                    if (_qtyRange == null
                        || (_qtyRange.Equals(">") && compare > 0)
                        || (_qtyRange.Equals("<") && compare < 0)
                        || (_qtyRange.Equals("=") && compare == 0)
                        || (_qtyRange.Equals("N") && compare != 0))
                    {
                        count += CreateInventoryLine(M_Locator_ID, M_Product_ID,
                            M_AttributeSetInstance_ID, currentQty, currentQty, M_AttributeSet_ID);
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
                log.Log(Level.SEVERE, sql.ToString(), e);
            }

            //	Set Count to Zero
            if (_inventoryCountSetZero)
            {
                String sql1 = "UPDATE M_InventoryLine l "
                    + "SET QtyCount=0 "
                    + "WHERE M_Inventory_ID=" + _m_Inventory_ID;
                int no = DataBase.DB.ExecuteQuery(sql1, null, Get_TrxName());
                log.Info("Set Cont to Zero=" + no);
            }

            //
            return "@M_InventoryLine_ID@ - #" + count;
        }

        /// <summary>
        /// Create/Add to Inventory Line
        /// </summary>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="qtyOnHand">quantity</param>
        /// <param name="M_AttributeSet_ID">attribute set</param>
        /// <returns>lines added</returns>
        private int CreateInventoryLine(int M_Locator_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, Decimal qtyOnHand, Decimal currentQty, int M_AttributeSet_ID)
        {
            Boolean oneLinePerASI = false;
            if (M_AttributeSet_ID != 0)
            {
                MAttributeSet mas = MAttributeSet.Get(GetCtx(), M_AttributeSet_ID);
                oneLinePerASI = mas.IsInstanceAttribute();
            }
            if (oneLinePerASI)
            {
                MInventoryLine line = new MInventoryLine(_inventory, M_Locator_ID,
                    M_Product_ID, M_AttributeSetInstance_ID,
                    qtyOnHand, qtyOnHand);		//	book/count
                line.SetOpeningStock(currentQty);
                if (line.Save())
                    return 1;
                return 0;
            }

            if (Env.Signum(qtyOnHand) == 0)
                M_AttributeSetInstance_ID = 0;

            if (_line != null
                && _line.GetM_Locator_ID() == M_Locator_ID
                && _line.GetM_Product_ID() == M_Product_ID)
            {
                if (Env.Signum(qtyOnHand) == 0)
                    return 0;
                //	Same ASI (usually 0)
                if (_line.GetM_AttributeSetInstance_ID() == M_AttributeSetInstance_ID)
                {
                    _line.SetQtyBook(Decimal.Add(_line.GetQtyBook(), qtyOnHand));
                    _line.SetQtyCount(Decimal.Add(_line.GetQtyCount(), qtyOnHand));
                    _line.SetOpeningStock((Decimal.Add(_line.GetOpeningStock(), currentQty)));
                    _line.Save();
                    return 0;
                }
                //	Save Old Line info
                else if (_line.GetM_AttributeSetInstance_ID() != 0)
                {
                    MInventoryLineMA ma = new MInventoryLineMA(_line,
                        _line.GetM_AttributeSetInstance_ID(), _line.GetQtyBook());
                    if (!ma.Save())
                        ;
                }
                _line.SetM_AttributeSetInstance_ID(0);
                _line.SetQtyBook(Decimal.Add(_line.GetQtyBook(), qtyOnHand));
                _line.SetQtyCount(Decimal.Add(_line.GetQtyCount(), qtyOnHand));
                _line.SetOpeningStock((Decimal.Add(_line.GetOpeningStock(), currentQty)));
                _line.Save();
                //
                MInventoryLineMA ma1 = new MInventoryLineMA(_line, M_AttributeSetInstance_ID, qtyOnHand);
                if (!ma1.Save())
                    ;
                return 0;
            }
            //	new line
            _line = new MInventoryLine(_inventory, M_Locator_ID, M_Product_ID,
                M_AttributeSetInstance_ID, qtyOnHand, qtyOnHand);		//	book/count
            _line.SetOpeningStock(currentQty);
            if (_line.Save())
                return 1;
            return 0;
        }
    }
}
