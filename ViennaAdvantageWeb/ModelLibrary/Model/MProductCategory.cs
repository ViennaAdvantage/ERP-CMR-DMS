/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductCategory
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
    public class MProductCategory : X_M_Product_Category
    {
        /**	Categopry Cache				*/
        private static CCache<int, MProductCategory> s_cache = new CCache<int, MProductCategory>("M_Product_Category", 20);
        /**	Product Cache				*/
        private static CCache<int, int?> s_products = new CCache<int, int?>("M_Product", 100);
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MProductCategory).FullName);

        /* 	Get from Cache
        *	@param ctx context
        *	@param M_Product_Category_ID id
        *	@return category
        */
        public static MProductCategory Get(Ctx ctx, int M_Product_Category_ID)
        {
            int ii = M_Product_Category_ID;
            MProductCategory pc = (MProductCategory)s_cache[ii];
            if (pc == null)
                pc = new MProductCategory(ctx, M_Product_Category_ID, null);
            return pc;
        }

        /**
         * 	Is Product in Category
         *	@param M_Product_Category_ID category
         *	@param M_Product_ID product
         *	@return true if product has category
         */
        public static bool IsCategory(int M_Product_Category_ID, int M_Product_ID)
        {
            if (M_Product_ID == 0 || M_Product_Category_ID == 0)
                return false;
            //	Look up
            int product = (int)M_Product_ID;
            int? category = s_products[product];
            if (category != null)
                return category == M_Product_Category_ID;

            String sql = "SELECT M_Product_Category_ID FROM M_Product WHERE M_Product_ID=" + M_Product_ID;
            DataSet ds = null;
            try
            {
                ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    category = (int?)rs[0];
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e); 
            }

            if (category != null)
            {
                //	TODO: LRU logic  
                s_products.Add(product, category);
                //
                _log.Fine("M_Product_ID=" + M_Product_ID + "(" + category
                    + ") in M_Product_Category_ID=" + M_Product_Category_ID
                    + " - " + (category == M_Product_Category_ID));
                return category.Value == M_Product_Category_ID;
            }
            _log.Log(Level.SEVERE, "Not found M_Product_ID=" + M_Product_ID);
            return false;
        }

        /**************************************************************************
         * 	Default Constructor
         *	@param ctx context
         *	@param M_Product_Category_ID id
         *	@param trxName transaction
         */
        public MProductCategory(Ctx ctx, int M_Product_Category_ID, Trx trxName)
            : base(ctx, M_Product_Category_ID, trxName)
        {

            if (M_Product_Category_ID == 0)
            {
                //	setName (null);
                //	setValue (null);
                SetMMPolicy(MMPOLICY_FiFo);	// F
                SetPlannedMargin(Env.ZERO);
                SetIsDefault(false);
                SetIsSelfService(true);	// Y
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MProductCategory(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord & success)
                Insert_Accounting("M_Product_Category_Acct", "C_AcctSchema_Default", null);
            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("M_Product_Category_Acct");
        }

        /**
         * 	FiFo Material Movement Policy
         *	@return true if FiFo
         */
        public bool IsFiFo()
        {
            return MMPOLICY_FiFo.Equals(GetMMPolicy());
        }
    }
}