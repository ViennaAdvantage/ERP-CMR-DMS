/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProduct
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     04-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProduct : X_M_Product
    {
        //	UOM Precision			
        private int? _precision = null;
        // Additional Downloads				
        private MProductDownload[] _downloads = null;
        //	Cache						
        private static CCache<int, MProduct> s_cache = new CCache<int, MProduct>("M_Product", 40, 5);	//	5 minutes
        //	Static Logger	*
        private static VLogger _log = VLogger.GetVLogger(typeof(MProduct).FullName);

        /// <summary>
        /// Get MProduct from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Product_ID">id</param>
        /// <returns>MProduct</returns>
        public static MProduct Get(Ctx ctx, int M_Product_ID)
        {
            int key = M_Product_ID;
            MProduct retValue = (MProduct)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MProduct(ctx, M_Product_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }

        /*	Get MProduct from Cache
        *	@param ctx context
        *	@param whereClause sql where clause
        *	@param trxName trx
        *	@return MProduct
        */
        public static MProduct[] Get(Ctx ctx, String whereClause, Trx trxName)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            String sql = "SELECT * FROM M_Product";
            if (whereClause != null && whereClause.Length > 0)
                sql += " WHERE AD_Client_ID=" + AD_Client_ID + " AND " + whereClause;
            List<MProduct> list = new List<MProduct>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {

                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProduct(ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MProduct[] retValue = new MProduct[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Trial Products for Entity Type
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="entityType">entity type</param>
        /// <returns>trial product or null</returns>
        public static MProduct GetTrial(Ctx ctx, String entityType)
        {
            if (Utility.Util.IsEmpty(entityType))
            {
                _log.Warning("No Entity Type");
                return null;
            }
            MProduct retValue = null;
            String sql = "SELECT * FROM M_Product "
                + "WHERE LicenseInfo LIKE '%" + entityType + "%' AND TrialPhaseDays > 0 AND IsActive='Y'";
            //String entityTypeLike = "%" + entityType + "%";
            //pstmt.setString(1, entityTypeLike);
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MProduct(ctx, dr, null);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            if (retValue != null && retValue.GetAD_Client_ID() != ctx.GetAD_Client_ID())
            {
                _log.Warning("ProductClient_ID=" + retValue.GetAD_Client_ID() + " <> EnvClient_ID=" + ctx.GetAD_Client_ID());
            }
            if (retValue != null && retValue.GetA_Asset_Group_ID() == 0)
            {
                _log.Warning("Product has no Asset Group - " + retValue);
                return null;
            }
            if (retValue == null)
            {
                _log.Warning("No Product for EntityType - " + entityType);
            }
            return retValue;
        }

        /*	Is Product Stocked
        * 	@param ctx context
        *	@param M_Product_ID id
        *	@return true if found and stocked - false otherwise
        */
        public static bool IsProductStocked(Ctx ctx, int M_Product_ID)
        {
            MProduct product = Get(ctx, M_Product_ID);
            return product.IsStocked();
        }

        /* 	Standard Constructor
         *	@param ctx context
         *	@param M_Product_ID id
         *	@param trxName transaction
         */
        public MProduct(Ctx ctx, int M_Product_ID, Trx trxName)
            : base(ctx, M_Product_ID, trxName)
        {
            if (M_Product_ID == 0)
            {
                //	setValue (null);
                //	setName (null);
                //	setM_Product_Category_ID (0);
                //	setC_TaxCategory_ID (0);
                //	setC_UOM_ID (0);
                //
                SetProductType(PRODUCTTYPE_Item);	// I
                SetIsBOM(false);	// N
                SetIsInvoicePrintDetails(false);
                SetIsPickListPrintDetails(false);
                SetIsPurchased(true);	// Y
                SetIsSold(true);	// Y
                SetIsStocked(true);	// Y
                SetIsSummary(false);
                SetIsVerified(false);	// N
                SetIsWebStoreFeatured(false);
                SetIsSelfService(true);
                SetIsExcludeAutoDelivery(false);
                SetProcessing(false);	// N
                SetIsDropShip(false); // N
                SetSupportUnits(1);
            }
        }

        /**
         * 	Load constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MProduct(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param et parent
         */
        public MProduct(MExpenseType et)
            : this(et.GetCtx(), 0, et.Get_TrxName())
        {

            SetProductType(X_M_Product.PRODUCTTYPE_ExpenseType);
            SetExpenseType(et);
        }

        /**
         * 	Parent Constructor
         *	@param resource parent
         *	@param resourceType resource type
         */
        public MProduct(MResource resource, MResourceType resourceType)
            : this(resource.GetCtx(), 0, resource.Get_TrxName())
        {

            SetProductType(X_M_Product.PRODUCTTYPE_Resource);
            SetResource(resource);
            SetResource(resourceType);
        }

        /**
         * 	Import Constructor
         *	@param impP import
         */
        public MProduct(X_I_Product impP)
            : this(impP.GetCtx(), 0, impP.Get_TrxName())
        {
            PO.CopyValues(impP, this, impP.GetAD_Client_ID(), impP.GetAD_Org_ID());
        }


        /*	Set Expense Type
        *	@param parent expense type
        *	@return true if changed
        */
        public bool SetExpenseType(MExpenseType parent)
        {
            bool changed = false;
            if (!PRODUCTTYPE_ExpenseType.Equals(GetProductType()))
            {
                SetProductType(PRODUCTTYPE_ExpenseType);
                changed = true;
            }
            if (parent.GetS_ExpenseType_ID() != GetS_ExpenseType_ID())
            {
                SetS_ExpenseType_ID(parent.GetS_ExpenseType_ID());
                changed = true;
            }
            if (parent.IsActive() != IsActive())
            {
                SetIsActive(parent.IsActive());
                changed = true;
            }
            //
            if (!parent.GetValue().Equals(GetValue()))
            {
                SetValue(parent.GetValue());
                changed = true;
            }
            if (!parent.GetName().Equals(GetName()))
            {
                SetName(parent.GetName());
                changed = true;
            }
            if ((parent.GetDescription() == null && GetDescription() != null)
                || (parent.GetDescription() != null && !parent.GetDescription().Equals(GetDescription())))
            {
                SetDescription(parent.GetDescription());
                changed = true;
            }
            if (parent.GetC_UOM_ID() != GetC_UOM_ID())
            {
                SetC_UOM_ID(parent.GetC_UOM_ID());
                changed = true;
            }
            if (parent.GetM_Product_Category_ID() != GetM_Product_Category_ID())
            {
                SetM_Product_Category_ID(parent.GetM_Product_Category_ID());
                changed = true;
            }
            if (parent.GetC_TaxCategory_ID() != GetC_TaxCategory_ID())
            {
                SetC_TaxCategory_ID(parent.GetC_TaxCategory_ID());
                changed = true;
            }
            //
            return changed;
        }

        /**
         * 	Set Resource
         *	@param parent resource
         *	@return true if changed
         */
        public bool SetResource(MResource parent)
        {
            bool changed = false;
            if (!PRODUCTTYPE_Resource.Equals(GetProductType()))
            {
                SetProductType(PRODUCTTYPE_Resource);
                changed = true;
            }
            if (parent.GetS_Resource_ID() != GetS_Resource_ID())
            {
                SetS_Resource_ID(parent.GetS_Resource_ID());
                changed = true;
            }
            if (parent.IsActive() != IsActive())
            {
                SetIsActive(parent.IsActive());
                changed = true;
            }
            //
            if (!parent.GetValue().Equals(GetValue()))
            {
                SetValue(parent.GetValue());
                changed = true;
            }
            if (!parent.GetName().Equals(GetName()))
            {
                SetName(parent.GetName());
                changed = true;
            }
            if ((parent.GetDescription() == null && GetDescription() != null)
                || (parent.GetDescription() != null && !parent.GetDescription().Equals(GetDescription())))
            {
                SetDescription(parent.GetDescription());
                changed = true;
            }
            //
            return changed;
        }

        /**
         * 	Set Resource Type
         *	@param parent resource type
         *	@return true if changed
         */
        public bool SetResource(MResourceType parent)
        {
            bool changed = false;
            if (PRODUCTTYPE_Resource.Equals(GetProductType()))
            {
                SetProductType(PRODUCTTYPE_Resource);
                changed = true;
            }
            //
            if (parent.GetC_UOM_ID() != GetC_UOM_ID())
            {
                SetC_UOM_ID(parent.GetC_UOM_ID());
                changed = true;
            }
            if (parent.GetM_Product_Category_ID() != GetM_Product_Category_ID())
            {
                SetM_Product_Category_ID(parent.GetM_Product_Category_ID());
                changed = true;
            }
            if (parent.GetC_TaxCategory_ID() != GetC_TaxCategory_ID())
            {
                SetC_TaxCategory_ID(parent.GetC_TaxCategory_ID());
                changed = true;
            }
            //
            return changed;
        }

        /**
       * 	Get UOM Standard Precision
       *	@return UOM Standard Precision
       */
        public int GetUOMPrecision()
        {
            if (_precision == null)
            {
                int C_UOM_ID = GetC_UOM_ID();
                if (C_UOM_ID == 0)
                    return 0;	//	EA
                _precision = (int)MUOM.GetPrecision(GetCtx(), C_UOM_ID);
            }
            return (int)_precision;
        }

        /**
        * 	Create Asset Group for this product
        *	@return asset group id
        */
        public int GetA_Asset_Group_ID()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
            return pc.GetA_Asset_Group_ID();
        }


        /**
        * 	Create Asset for this product
        *	@return true if asset is created
        */
        public bool IsCreateAsset()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
            return pc.GetA_Asset_Group_ID() != 0;
        }

        /* 	Get Attribute Set
        *	@return set or null
        */
        public MAttributeSet GetAttributeSet()
        {
            if (GetM_AttributeSet_ID() != 0)
                return MAttributeSet.Get(GetCtx(), GetM_AttributeSet_ID());
            return null;
        }


        /*	Has the Product Instance Attribute
         *	@return true if instance attributes
         */
        public bool IsInstanceAttribute()
        {
            if (GetM_AttributeSet_ID() == 0)
                return false;
            MAttributeSet mas = MAttributeSet.Get(GetCtx(), GetM_AttributeSet_ID());
            return mas.IsInstanceAttribute();
        }

        /**
        * 	Create One Asset Per UOM
        *	@return individual asset
        */
        public bool IsOneAssetPerUOM()
        {
            MProductCategory pc = MProductCategory.Get(GetCtx(), GetM_Product_Category_ID());
            if (pc.GetA_Asset_Group_ID() == 0)
                return false;
            MAssetGroup ag = MAssetGroup.Get(GetCtx(), pc.GetA_Asset_Group_ID());
            return ag.IsOneAssetPerUOM();
           
        }

        /* Product is Item
        *	@return true if item
        */
        public bool IsItem()
        {
            return PRODUCTTYPE_Item.Equals(GetProductType());
        }

        /**
         * 	Product is an Item and Stocked
         *	@return true if stocked and item
         */
        public new bool IsStocked()
        {
            return base.IsStocked() && IsItem();
        }

        /**
         * 	Is Service
         *	@return true if service (resource, online)
         */
        public bool IsService()
        {
            //	PRODUCTTYPE_Service, PRODUCTTYPE_Resource, PRODUCTTYPE_Online
            return !IsItem();	//	
        }

        /*	Get UOM Symbol
         *	@return UOM Symbol
         */
        public String GetUOMSymbol()
        {
            int C_UOM_ID = GetC_UOM_ID();
            if (C_UOM_ID == 0)
                return "";
            return MUOM.Get(GetCtx(), C_UOM_ID).GetUOMSymbol();
        }

        /**
        * 	Get Active(!) Product Downloads
        * 	@param requery requery
        *	@return array of downloads
        */
        public MProductDownload[] GetProductDownloads(bool requery)
        {
            if (_downloads != null && !requery)
                return _downloads;
            //
            List<MProductDownload> list = new List<MProductDownload>();
            String sql = "SELECT * FROM M_ProductDownload "
                + "WHERE M_Product_ID=" + GetM_Product_ID() + " AND IsActive='Y' ORDER BY Name";
            //
            DataTable dt = null;
            IDataReader idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
            try
            {
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MProductDownload(GetCtx(), dr, Get_TrxName()));
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

            _downloads = new MProductDownload[list.Count];
            _downloads = list.ToArray();
            return _downloads;
        }

        /**
        * 	Does the product have downloads
        *	@return true if downloads exists
        */
        public bool HasDownloads()
        {
            GetProductDownloads(false);
            return _downloads != null && _downloads.Length > 0;
        }

        /*	Get SupportUnits
        *	@return units per UOM
        */
        public new int GetSupportUnits()
        {
            int ii = base.GetSupportUnits();
            if (ii < 1)
                ii = 1;
            return ii;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MProduct[");
            sb.Append(Get_ID()).Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }


        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	Check Storage
            if (!newRecord && 	//	
                ((Is_ValueChanged("IsActive") && !IsActive())		//	now not active 
                || (Is_ValueChanged("IsStocked") && !IsStocked())	//	now not stocked
                || (Is_ValueChanged("ProductType") 					//	from Item
                    && PRODUCTTYPE_Item.Equals(Get_ValueOld("ProductType")))))
            {
                MStorage[] storages = MStorage.GetOfProduct(GetCtx(), Get_ID(), Get_TrxName());
                Decimal OnHand = Env.ZERO;
                Decimal Ordered = Env.ZERO;
                Decimal Reserved = Env.ZERO;
                for (int i = 0; i < storages.Length; i++)
                {
                    OnHand = Decimal.Add(OnHand, (storages[i].GetQtyOnHand()));
                    Ordered = Decimal.Add(OnHand, (storages[i].GetQtyOrdered()));
                    Reserved = Decimal.Add(OnHand, (storages[i].GetQtyReserved()));
                }
                String errMsg = "";
                if (Env.Signum(OnHand) != 0)
                    errMsg = "@QtyOnHand@ = " + OnHand;
                if (Env.Signum(Ordered) != 0)
                    errMsg += " - @QtyOrdered@ = " + Ordered;
                if (Env.Signum(Reserved) != 0)
                    errMsg += " - @QtyReserved@" + Reserved;
                if (errMsg.Length > 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), errMsg));
                    return false;
                }
            }	//	storage

            //	Reset Stocked if not Item
            if (IsStocked() && !PRODUCTTYPE_Item.Equals(GetProductType()))
                SetIsStocked(false);

            //	UOM reset
            if (_precision != null && Is_ValueChanged("C_UOM_ID"))
                _precision = null;
            if (Util.GetValueOfInt(Env.GetCtx().GetContext("#AD_User_ID")) != 100)
            {
                if (Is_ValueChanged("C_UOM_ID") || Is_ValueChanged("M_AttributeSet_ID"))
                {
                    string uqry = "SELECT SUM(cc) as count FROM  (SELECT COUNT(*) AS cc FROM M_MovementLine WHERE M_Product_ID = " + GetM_Product_ID() + @"  UNION
                SELECT COUNT(*) AS cc FROM M_InventoryLine WHERE M_Product_ID = " + GetM_Product_ID() + " UNION SELECT COUNT(*) AS cc FROM C_OrderLine WHERE M_Product_ID = " + GetM_Product_ID() +
                    " UNION  SELECT COUNT(*) AS cc FROM M_InOutLine WHERE M_Product_ID = " + GetM_Product_ID() + ")";
                    int no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
                    if (no == 0 || IsBOM())
                    {
                        uqry = "SELECT count(*) FROM M_ProductionPlan WHERE M_Product_ID = " + GetM_Product_ID();
                        no = Util.GetValueOfInt(DB.ExecuteScalar(uqry));
                    }
                    if (no > 0)
                    {
                        log.SaveError("Could not Save Record. Transactions available in System.", "");
                        return false;
                    }
                }
            }
            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;

            //	Value/Name change in Account
            if (!newRecord && (Is_ValueChanged("Value") || Is_ValueChanged("Name")))
                MAccount.UpdateValueDescription(GetCtx(), "M_Product_ID=" + GetM_Product_ID(), Get_TrxName());

            //	Name/Description Change in Asset	MAsset.setValueNameDescription
            if (!newRecord && (Is_ValueChanged("Name") || Is_ValueChanged("Description")))
            {
                //String sql = "UPDATE A_Asset a "
                //    + "SET (Name, Description)="
                //        + "(SELECT SUBSTR(bp.Name || ' - ' || p.Name,1,60), p.Description "
                //        + "FROM M_Product p, C_BPartner bp "
                //        + "WHERE p.M_Product_ID=a.M_Product_ID AND bp.C_BPartner_ID=a.C_BPartner_ID) "
                //    + "WHERE IsActive='Y'"
                //    //	+ " AND GuaranteeDate > SysDate"
                //    + "  AND M_Product_ID=" + GetM_Product_ID();

  

                String sql = " UPDATE A_Asset a SET Name=(SELECT SUBSTR(bp.Name || ' - ' || p.Name,1,60) FROM M_Product p, C_BPartner bp  WHERE p.M_Product_ID=a.M_Product_ID AND bp.C_BPartner_ID=a.C_BPartner_ID)," +
  "Description=(SELECT  p.Description FROM M_Product p, C_BPartner bp WHERE p.M_Product_ID=a.M_Product_ID AND bp.C_BPartner_ID=a.C_BPartner_ID)" +
  "WHERE IsActive='Y'  AND M_Product_ID=" + GetM_Product_ID();

                int no = 0;
                try
                {
                    no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                }
                catch { }
                log.Fine("Asset Description updated #" + no);
            }

            //	New - Acct, Tree, Old Costing
            if (newRecord)
            {
                Insert_Accounting("M_Product_Acct", "M_Product_Category_Acct",
                    "p.M_Product_Category_ID=" + GetM_Product_Category_ID());
                //
                MAcctSchema[] mass = MAcctSchema.GetClientAcctSchema(GetCtx(), GetAD_Client_ID(), Get_TrxName());
                for (int i = 0; i < mass.Length; i++)
                {
                    //	Old
                    MProductCosting pcOld = new MProductCosting(this, mass[i].GetC_AcctSchema_ID());
                    pcOld.Save();
                }
            }

            //	New Costing
            if (newRecord || Is_ValueChanged("M_Product_Category_ID"))
            {
                MCost.Create(this);
            }

            return success;
        }

        /**
         * 	Before Delete
         *	@return true if it can be deleted
         */
        protected override bool BeforeDelete()
        {
            //	Check Storage
            if (IsStocked() || PRODUCTTYPE_Item.Equals(GetProductType()))
            {
                MStorage[] storages = MStorage.GetOfProduct(GetCtx(), Get_ID(), Get_TrxName());
                Decimal OnHand = Env.ZERO;
                Decimal Ordered = Env.ZERO;
                Decimal Reserved = Env.ZERO;
                for (int i = 0; i < storages.Length; i++)
                {
                    OnHand = Decimal.Add(OnHand, (storages[i].GetQtyOnHand()));
                    Ordered = Decimal.Add(OnHand, (storages[i].GetQtyOrdered()));
                    Reserved = Decimal.Add(OnHand, (storages[i].GetQtyReserved()));
                }
                String errMsg = "";
                if (Env.Signum(OnHand) != 0)
                    errMsg = "@QtyOnHand@ = " + OnHand;
                if (Env.Signum(Ordered) != 0)
                    errMsg += " - @QtyOrdered@ = " + Ordered;
                if (Env.Signum(Reserved) != 0)
                    errMsg += " - @QtyReserved@" + Reserved;
                if (errMsg.Length > 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), errMsg));
                    return false;
                }

            }
            //	delete costing
            MProductCosting[] costings = MProductCosting.GetOfProduct(GetCtx(), Get_ID(), Get_TrxName());
            for (int i = 0; i < costings.Length; i++)
                costings[i].Delete(true, Get_TrxName());
            return Delete_Accounting("M_Product_Acct");
        }

    }
}
