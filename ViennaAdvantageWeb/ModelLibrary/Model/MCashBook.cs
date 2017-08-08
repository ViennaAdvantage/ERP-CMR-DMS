/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCashBook
 * Purpose        : Cash Book Model
 * Class Used     : X_C_CashBook
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process; using VAdvantage.ProcessEngine;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCashBook : X_C_CashBook
    {

        //	Cache						
        private static CCache<int, MCashBook> _cache = new CCache<int, MCashBook>("", 20);
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCashBook).FullName);
        /**
         * 	Get MCashBook from Cache
         *	@param ctx context
         *	@param C_CashBook_ID id
         *	@return MCashBook
         */
        public static MCashBook Get(Ctx ctx, int C_CashBook_ID)
        {
            int key = C_CashBook_ID;
            MCashBook retValue = (MCashBook)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCashBook(ctx, C_CashBook_ID, null);
            if (retValue.Get_ID() != 0)
            {
                _cache.Add(key, retValue);
            }
            return retValue;
        }

        /**
         * 	Get CashBook for Org and Currency
         *	@param ctx context
         *	@param AD_Org_ID org
         *	@param C_Currency_ID currency
         *	@return cash book or null
         */
        public static MCashBook Get(Ctx ctx, int AD_Org_ID, int C_Currency_ID)
        {
            //	Try from cache
            //Iterator it = _cache.values().iterator();
            IEnumerator it = _cache.Values.GetEnumerator();
            while (it.MoveNext())
            {
                MCashBook cb = (MCashBook)it.Current;
                if (cb.GetAD_Org_ID() == AD_Org_ID && cb.GetC_Currency_ID() == C_Currency_ID)
                    return cb;
            }

            //	Get from DB
            MCashBook retValue = null;
            String sql = "SELECT * FROM C_CashBook "
                + "WHERE AD_Org_ID=" + AD_Org_ID + " AND C_Currency_ID=" + C_Currency_ID
                + "ORDER BY IsDefault DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCashBook(ctx, dr, null);
                    int key = retValue.GetC_CashBook_ID();
                    _cache.Add(key, retValue);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, "get", e);
            }
            finally { dt = null; }
            return retValue;
        }

        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_CashBook_ID id
         *	@param trxName transaction
         */
        public MCashBook(Ctx ctx, int C_CashBook_ID, Trx trxName)
            : base(ctx, C_CashBook_ID, trxName)
        {
            if (C_CashBook_ID == 0)
                SetIsDefault(false);
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MCashBook(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
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
                Insert_Accounting("C_CashBook_Acct", "C_AcctSchema_Default", null);

            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("C_Cashbook_Acct");
        }

    }
}
