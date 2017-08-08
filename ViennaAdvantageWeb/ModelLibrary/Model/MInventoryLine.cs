/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_InventoryLine
 * Chronological Development
 * Veena Pandey     22-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInventoryLine : X_M_InventoryLine
    {
        //	Logger				
        private static VLogger _log = VLogger.GetVLogger(typeof(MInventoryLine).FullName);
        /** Manually created				*/
        private bool _isManualEntry = true;
        /** Parent							*/
        private MInventory _parent = null;
        /** Product							*/
        private MProduct _product = null;
        public Decimal? OnHandQty = 0;
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_InventoryLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MInventoryLine(Ctx ctx, int M_InventoryLine_ID, Trx trxName)
            : base(ctx, M_InventoryLine_ID, trxName)
        {
            if (M_InventoryLine_ID == 0)
            {
                //	setM_Inventory_ID (0);			//	Parent
                //	setM_InventoryLine_ID (0);		//	PK
                //	setM_Locator_ID (0);			//	FK
                SetLine(0);
                //	setM_Product_ID (0);			//	FK
                SetM_AttributeSetInstance_ID(0);	//	FK
                SetInventoryType(INVENTORYTYPE_InventoryDifference);
                SetQtyBook(Env.ZERO);
                SetQtyCount(Env.ZERO);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MInventoryLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Detail Constructor.
        /// Locator/Product/AttributeSetInstance must be unique
        /// </summary>
        /// <param name="inventory">parent</param>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">instance</param>
        /// <param name="qtyBook">book value</param>
        /// <param name="qtyCount">count value</param>
        public MInventoryLine(MInventory inventory, int M_Locator_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, Decimal qtyBook, Decimal qtyCount)
            : this(inventory.GetCtx(), 0, inventory.Get_TrxName())
        {
            if (inventory.Get_ID() == 0)
                throw new ArgumentException("Header not saved");
            _parent = inventory;
            SetM_Inventory_ID(inventory.GetM_Inventory_ID());		//	Parent
            SetClientOrg(inventory.GetAD_Client_ID(), inventory.GetAD_Org_ID());
            SetM_Locator_ID(M_Locator_ID);		//	FK
            SetM_Product_ID(M_Product_ID);		//	FK
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            //
            // if (qtyBook != null)
            SetQtyBook(qtyBook);
            if (qtyCount != null && Env.Signum(qtyCount) != 0)
                SetQtyCount(qtyCount);
            _isManualEntry = false;
        }

        /// <summary>
        /// Get Inventory Line with parameters
        /// </summary>
        /// <param name="inventory">inventory</param>
        /// <param name="M_Locator_ID">locator</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <returns>line or null</returns>
        public static MInventoryLine Get(MInventory inventory, int M_Locator_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID)
        {
            MInventoryLine retValue = null;
            String sql = "SELECT * FROM M_InventoryLine "
                + "WHERE M_Inventory_ID=@invenid AND M_Locator_ID=@locid"
                + " AND M_Product_ID=@prodid AND M_AttributeSetInstance_ID=@asiid";
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@invenid", inventory.GetM_Inventory_ID());
                param[1] = new SqlParameter("@locid", M_Locator_ID);
                param[2] = new SqlParameter("@prodid", M_Product_ID);
                param[3] = new SqlParameter("@asiid", M_AttributeSetInstance_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        retValue = new MInventoryLine(inventory.GetCtx(), dr, inventory.Get_TrxName());
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            return retValue;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>true</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            //	Create MA
            if (newRecord && success
                && _isManualEntry && GetM_AttributeSetInstance_ID() == 0)
                CreateMA(true);
            if (!IsInternalUse())
            {
                MInventory inv = new MInventory(GetCtx(), GetM_Inventory_ID(), Get_Trx());
                inv.SetIsAdjusted(false);
                if (!inv.Save())
                {

                }
            }
            return true;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord && _isManualEntry)
            {
                //	Product requires ASI
                if (GetM_AttributeSetInstance_ID() == 0)
                {
                    MProduct product = MProduct.Get(GetCtx(), GetM_Product_ID());
                    if (product.GetM_AttributeSet_ID() != 0)
                    {
                        MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                        if (mas.IsInstanceAttribute()
                            && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                        {
                            log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstance_ID"));
                            return false;
                        }
                    }
                }	//	No ASI
            }	//	new or manual

            //	Set Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM M_InventoryLine WHERE M_Inventory_ID=" + GetM_Inventory_ID();
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                SetLine(ii);
            }
            int M_Warehouse_ID = 0; MWarehouse wh = null;
            string qry = "select m_warehouse_id from m_locator where m_locator_id=" + GetM_Locator_ID();
            M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry));

            wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
            qry = "SELECT QtyOnHand FROM M_Storage where m_locator_id=" + GetM_Locator_ID() + " and m_product_id=" + GetM_Product_ID();
            if (GetM_AttributeSetInstance_ID() != 0)
            {
                qry += " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID();
            }
            OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry));
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (GetQtyCount() < 0)
                {
                    if (wh.IsDisallowNegativeInv() == true)
                    {
                        //ShowMessage.Info("Qty Available  " + OnHandQty, true, null, null);
                        return false;
                    }
                }
                else if ((GetQtyInternalUse() > 0))
                {
                    if ((OnHandQty - GetQtyInternalUse()) < 0)
                    {
                        if (wh.IsDisallowNegativeInv() == true)
                        {
                            //ShowMessage.Info("Qty Available  " + OnHandQty, true, null, null);
                            return false;
                        }
                    }
                }
                else if ((GetQtyInternalUse() < 0))
                {
                    if (wh.IsDisallowNegativeInv() == true)
                    {
                        //ShowMessage.Info("Qty Available  " + OnHandQty, true, null, null);
                        return false;
                    }
                }
            }
            //	Enforce Qty UOM
            if (newRecord || Is_ValueChanged("QtyCount"))
                SetQtyCount(GetQtyCount());
            if (newRecord || Is_ValueChanged("QtyInternalUse"))
                SetQtyInternalUse(GetQtyInternalUse());

            //	InternalUse Inventory 
            if (IsInternalUse() && Env.Signum(GetQtyInternalUse()) == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "QtyInternalUse"));
                return false;
            }
            if (Env.Signum(GetQtyInternalUse()) != 0)
            {
                if (!INVENTORYTYPE_ChargeAccount.Equals(GetInventoryType()))
                    SetInventoryType(INVENTORYTYPE_ChargeAccount);
                //
                if (GetC_Charge_ID() == 0)
                {
                    log.SaveError("InternalUseNeedsCharge", "");
                    return false;
                }
            }
            else if (INVENTORYTYPE_ChargeAccount.Equals(GetInventoryType()))
            {
                if (GetC_Charge_ID() == 0)
                {
                    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "C_Charge_ID"));
                    return false;
                }
            }
            else if (GetC_Charge_ID() != 0)
                SetC_Charge_ID(0);

            //	Set AD_Org to parent if not charge
            if (GetC_Charge_ID() == 0)
                SetAD_Org_ID(GetParent().GetAD_Org_ID());

            return true;
        }

        /// <summary>
        /// Create Material Allocations for new Instances
        /// </summary>
        /// <param name="updateQtyBooked"></param>
        public void CreateMA(bool updateQtyBooked)
        {
            int delMA = MInventoryLineMA.DeleteInventoryLineMA(GetM_InventoryLine_ID(), Get_TrxName());
            log.Info("DeletedMA=" + delMA);

            MStorage[] storages = MStorage.GetAll(GetCtx(), GetM_Product_ID(),
                GetM_Locator_ID(), Get_TrxName());
            bool allZeroASI = true;
            for (int i = 0; i < storages.Length; i++)
            {
                if (storages[i].GetM_AttributeSetInstance_ID() != 0)
                {
                    allZeroASI = false;
                    break;
                }
            }
            if (allZeroASI)
                return;

            MInventoryLineMA ma = null;
            Decimal sum = Env.ZERO;
            for (int i = 0; i < storages.Length; i++)
            {
                MStorage storage = storages[i];
                // nnayak - ignore negative layers
                if (Env.Signum(storage.GetQtyOnHand()) <= 0)
                {
                    continue;
                }
                if (ma != null
                    && ma.GetM_AttributeSetInstance_ID() == storage.GetM_AttributeSetInstance_ID())
                {
                    ma.SetMovementQty(Decimal.Add(ma.GetMovementQty(), storage.GetQtyOnHand()));
                }
                else
                {
                    ma = new MInventoryLineMA(this,
                        storage.GetM_AttributeSetInstance_ID(), storage.GetQtyOnHand());
                }
                if (!ma.Save())
                {
                    ;
                }
                sum = Decimal.Add(sum, storage.GetQtyOnHand());
            }
            if (updateQtyBooked && sum.CompareTo(GetQtyBook()) != 0)
            {
                log.Warning("QtyBook=" + GetQtyBook() + " corrected to Sum of MA=" + sum);
                SetQtyBook(sum);
            }
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>parent</returns>
        public MInventory GetParent()
        {
            if (_parent == null)
                _parent = new MInventory(GetCtx(), GetM_Inventory_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product or null if not defined</returns>
        public MProduct GetProduct()
        {
            int M_Product_ID = GetM_Product_ID();
            if (M_Product_ID == 0)
                return null;
            if (_product != null && _product.GetM_Product_ID() != M_Product_ID)
                _product = null;	//	reset
            if (_product == null)
                _product = MProduct.Get(GetCtx(), M_Product_ID);
            return _product;
        }

        /// <summary>
        /// Get Qty Book
        /// </summary>
        /// <returns>Qty Book</returns>
        public new Decimal GetQtyBook()
        {
            Decimal bd = base.GetQtyBook();
            //if (bd == null)
            //    bd = Env.ZERO;
            return bd;
        }

        /// <summary>
        /// Get Qty Count
        /// </summary>
        /// <returns>Qty Count</returns>
        public new Decimal GetQtyCount()
        {
            Decimal bd = base.GetQtyCount();
            //if (bd == null)
            //    bd = Env.ZERO;
            return bd;
        }

        /// <summary>
        /// Set Attribute Set Instance - Callout
        /// </summary>
        /// <param name="oldM_AttributeSetInstance_ID">old value</param>
        /// <param name="newM_AttributeSetInstance_ID">new value</param>
        /// <param name="windowNo">window</param>
        //  @UICallout
        public void SetM_AttributeSetInstance_ID(String oldM_AttributeSetInstance_ID,
            String newM_AttributeSetInstance_ID, int windowNo)
        {
            if (newM_AttributeSetInstance_ID == null || newM_AttributeSetInstance_ID.Length == 0)
                return;
            //
            int M_AttributeSetInstance_ID = int.Parse(newM_AttributeSetInstance_ID);
            base.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            //
            SetQtyBook(windowNo, "M_AttributeSetInstance_ID");
        }

        /// <summary>
        /// Set Locator - Callout
        /// </summary>
        /// <param name="oldM_Locator_ID">old value</param>
        /// <param name="newM_Locator_ID">new value</param>
        /// <param name="windowNo">window</param>
        //  @UICallout
        public void SetM_Locator_ID(String oldM_Locator_ID, String newM_Locator_ID, int windowNo)
        {
            if (newM_Locator_ID == null || newM_Locator_ID.Length == 0)
                return;
            int M_Locator_ID = int.Parse(newM_Locator_ID);
            base.SetM_Locator_ID(M_Locator_ID);
            SetQtyBook(windowNo, "M_Locator_ID");
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //  @UICallout
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                return;
            int M_Product_ID = int.Parse(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            //
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID)
            {
                int M_AttributeSetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
                SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            }
            else
                SetM_AttributeSetInstance_ID(-1);

            SetQtyBook(windowNo, "M_Product_ID");
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MInventory parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Set Qty Book from Product, AST, Locator
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        private void SetQtyBook(int windowNo, String columnName)
        {
            int M_Product_ID = GetM_Product_ID();
            int M_Locator_ID = GetM_Locator_ID();
            if (M_Product_ID == 0 || M_Locator_ID == 0)
            {
                SetQtyBook(Env.ZERO);
                return;
            }
            int M_AttributeSetInstance_ID = GetM_AttributeSetInstance_ID();

            // Set QtyBook from first storage location
            Decimal qtyBook = Env.ZERO;
            String sql = "SELECT QtyOnHand FROM M_Storage "
                + "WHERE M_Product_ID=@prodid"	//	1
                + " AND M_Locator_ID=@locid"		//	2
                + " AND M_AttributeSetInstance_ID=@asiid";
            if (M_AttributeSetInstance_ID == 0)
                sql = "SELECT SUM(QtyOnHand) FROM M_Storage "
                + "WHERE M_Product_ID=@prodid"	//	1
                + " AND M_Locator_ID=@locid";	//	2
            IDataReader idr = null;
            SqlParameter[] param = null;
            try
            {
                if (M_AttributeSetInstance_ID != 0)
                {
                    param = new SqlParameter[3];
                }
                else
                {
                    param = new SqlParameter[2];
                }
                param[0] = new SqlParameter("@prodid", M_Product_ID);
                param[1] = new SqlParameter("@locid", M_Locator_ID);
                if (M_AttributeSetInstance_ID != 0)
                {
                    param[2] = new SqlParameter("@asiid", M_AttributeSetInstance_ID);
                }

                idr = DataBase.DB.ExecuteReader(sql, param);
                if (idr.Read())
                {
                    qtyBook = Utility.Util.GetValueOfDecimal(idr[0]);
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
                //ErrorLog.FillErrorLog("MInventoryLine.SetQtyBook", DataBase.GlobalVariable.LAST_EXECUTED_QUERY, e.Message, VAdvantage.Framework.Message.MessageType.ERROR);
            }
            SetQtyBook(qtyBook);
            //
            log.Info("M_Product_ID=" + M_Product_ID
                + ", M_Locator_ID=" + M_Locator_ID
                + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
                + " - QtyBook=" + qtyBook);
        }

        /// <summary>
        /// Set Count Qty - enforce UOM 
        /// </summary>
        /// <param name="qtyCount">qty</param>
        public new void SetQtyCount(Decimal? qtyCount)
        {
            if (qtyCount != null)
            {
                MProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    qtyCount = Decimal.Round(qtyCount.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetQtyCount(qtyCount);
        }

        /// <summary>
        /// Set Internal Use Qty - enforce UOM
        /// </summary>
        /// <param name="qtyInternalUse">qty</param>
        public new void SetQtyInternalUse(Decimal? qtyInternalUse)
        {
            if (qtyInternalUse != null)
            {
                MProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    qtyInternalUse = Decimal.Round(qtyInternalUse.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetQtyInternalUse(qtyInternalUse);
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInventoryLine[");
            sb.Append(Get_ID())
                .Append("-M_Product_ID=").Append(GetM_Product_ID())
                .Append(",QtyCount=").Append(GetQtyCount())
                .Append(",QtyInternalUse=").Append(GetQtyInternalUse())
                .Append(",QtyBook=").Append(GetQtyBook())
                .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
                .Append("]");
            return sb.ToString();
        }

    }
}
