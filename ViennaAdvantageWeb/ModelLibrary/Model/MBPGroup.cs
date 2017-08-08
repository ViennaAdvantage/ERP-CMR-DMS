/********************************************************
 * Module Name    : Vframwork
 * Purpose        : Business Partner Group Model 
 * Class Used     : X_C_BP_Group
 * Chronological Development
 * Raghunandan    24-June-2009
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
    public class MBPGroup : X_C_BP_Group
    {
        /**
	     * 	Get MBPGroup from Cache
	     *	@param ctx context
	     *	@param C_BP_Group_ID id
	     *	@return MBPGroup
	     */
        public static MBPGroup Get(Ctx ctx, int C_BP_Group_ID)
        {
            int key = C_BP_Group_ID;
            MBPGroup retValue = (MBPGroup)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MBPGroup(ctx, C_BP_Group_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
         * 	Get Default MBPGroup
         *	@param ctx context
         *	@return MBPGroup
         */
        public static MBPGroup GetDefault(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            int key = AD_Client_ID;
            MBPGroup retValue = (MBPGroup)_cacheDefault[key];
            if (retValue != null)
                return retValue;

            DataTable dt = null;
            String sql = "SELECT * FROM C_BP_Group g "
                + "WHERE IsDefault='Y' AND AD_Client_ID= " + AD_Client_ID
                + " ORDER BY IsActive DESC";
            IDataReader idr = null;
            try
            {
                 idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MBPGroup(ctx, dr, null);
                    if (retValue.Get_ID() != 0)
                        _cacheDefault.Add(key, retValue);
                    break;
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
            finally { dt = null; }

            if (retValue == null)
            {
                _log.Warning("No Default BP Group for AD_Client_ID=" + AD_Client_ID);
            }
            return retValue;
        }

        /**
         * 	Get MBPGroup from Business Partner
         *	@param ctx context
         *	@param C_BPartner_ID business partner id
         *	@return MBPGroup
         */
        public static MBPGroup GetOfBPartner(Ctx ctx, int C_BPartner_ID)
        {
            MBPGroup retValue = null;
            DataTable dt = null;
            String sql = "SELECT * FROM C_BP_Group g "
                + "WHERE EXISTS (SELECT * FROM C_BPartner p "
                    + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.C_BP_Group_ID=g.C_BP_Group_ID)";
            IDataReader idr = null;
            try
            {
                 idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MBPGroup(ctx, dr, null);
                    int key = retValue.GetC_BP_Group_ID();
                    if (retValue.Get_ID() != 0)
                        _cache.Add(key, retValue);
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
            finally { dt = null; }

            return retValue;
        }

        //	Cache						
        private static CCache<int, MBPGroup> _cache
            = new CCache<int, MBPGroup>("BP_Group", 10);
        //	Default Cache					
        private static CCache<int, MBPGroup> _cacheDefault
            = new CCache<int, MBPGroup>("BP_Group", 5);
        //Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MBPGroup).FullName);

        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_BP_Group_ID id
         *	@param trxName transaction
         */
        public MBPGroup(Ctx ctx, int C_BP_Group_ID, Trx trxName) :
            base(ctx, C_BP_Group_ID, trxName)
        {
            if (C_BP_Group_ID == 0)
            {
                //	SetValue (null);
                //	SetName (null);
                SetIsConfidentialInfo(false);	// N
                SetIsDefault(false);
                SetPriorityBase(PRIORITYBASE_Same);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result Set
         *	@param trxName transaction
         */
        public MBPGroup(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Get Credit Watch Percent
         *	@return 90 or defined percent
         */
        public new Decimal GetCreditWatchPercent()
        {
            Object bd = Get_Value("CreditWatchPercent");
            if (bd != null)
            {
                return Convert.ToDecimal(bd);
            }
            return new Decimal(90);
        }
        /**
         * 	Get Credit Watch Ratio
         *	@return 0.90 or defined percent
         */
        public Decimal GetCreditWatchRatio()
        {
            //Decimal bd = base.GetCreditWatchPercent();
            Object bd = Get_Value("CreditWatchPercent");
            if (bd != null)
                return Decimal.Round(Decimal.Divide(Convert.ToDecimal(bd), Env.ONEHUNDRED), 2, MidpointRounding.AwayFromZero);
           return new Decimal(0.90);
        }

        protected override Boolean BeforeSave(Boolean newRecord)
        {
            // TODO Auto-generated method stub
            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new record
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            object table = DB.ExecuteScalar("SELECT count(*) from AD_Table WHERE TableName='C_BP_Group_Acct'");
            if (table == null || table == DBNull.Value || table == "" || Convert.ToInt16(table) == 0)
            {
                return success;
            }


            if (newRecord & success)
                return Insert_Accounting("C_BP_Group_Acct", "C_AcctSchema_Default", null);
            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_BP_Group_Acct");
        }
    }
}
