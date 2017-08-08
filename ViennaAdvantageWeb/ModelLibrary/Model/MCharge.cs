/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCharge
 * Purpose        : Charge Modle
 * Class Used     : X_C_Charge
 * Chronological    Development
 * Raghunandan     23-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using java.math;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCharge : X_C_Charge
    {
        /**
         * 
         */
        private const long serialVedrionUID = 1L;


        /**
         *  Get Charge Account
         *  @param C_Charge_ID charge
         *  @param as account schema
         *  @param amount amount for expense(+)/revenue(-)
         *  @return Charge Account or null
         */
        public static MAccount GetAccount(int C_Charge_ID, MAcctSchema aSchema, Decimal amount)
        {
            if (C_Charge_ID == 0 || aSchema == null)
                return null;

            int acct_index = 1;     //  Expense (positive amt)
            if ( amount < 0)
            {
                acct_index = 2;     //  Revenue (negative amt) 
            }

            String sql = "SELECT CH_Expense_Acct, CH_Revenue_Acct FROM C_Charge_Acct WHERE C_Charge_ID=" + C_Charge_ID + " AND C_AcctSchema_ID=" + aSchema.GetC_AcctSchema_ID();
            int Account_ID = 0;

            IDataReader dr = null;
            try
            {
                //	PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //	pstmt.setInt (1, C_Charge_ID);
                //	pstmt.setInt (2, aSchema.getC_AcctSchema_ID());
                //	ResultSet dr = pstmt.executeQuery();
                dr = DataBase.DB.ExecuteReader(sql, null, null);

                if (dr.Read())
                    Account_ID = Utility.Util.GetValueOfInt(dr[acct_index - 1].ToString());
                dr.Close();
                //pstmt.close();
            }
            catch (SqlException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
                return null;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            
            //	No account
            if (Account_ID == 0)
            {
                _log.Severe ("NO account for C_Charge_ID=" + C_Charge_ID);
                return null;
            }

            //	Return Account
            MAccount acct = MAccount.Get(aSchema.GetCtx(), Account_ID);
            return acct;


        }   //  getAccount

        /**
         * 	Get MCharge from Cache
         *	@param ctx context
         *	@param C_Charge_ID id
         *	@return MCharge
         */
        public static MCharge Get(Ctx ctx, int C_Charge_ID)
        {
            int key = C_Charge_ID;
            MCharge retValue = _cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCharge(ctx, C_Charge_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }	//	get

        /**	Cache						*/
        private static CCache<int, MCharge> _cache = new CCache<int, MCharge>("C_Charge", 10);

        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MCharge).FullName);


        /**************************************************************************
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_Charge_ID id
         *	@param trxName transaction
         */
        public MCharge(Ctx ctx, int C_Charge_ID, Trx trxName) :
            base(ctx, C_Charge_ID, null)
        {
            //super (ctx, C_Charge_ID, null);
            if (C_Charge_ID == 0)
            {
                SetChargeAmt(Env.ZERO);
                SetIsSameCurrency(false);
                SetIsSameTax(false);
                SetIsTaxIncluded(false);	// N
                //	setName (null);
                //	setC_TaxCategory_ID (0);
            }
        }	//	MCharge

        /**
         * 	Load Constructor
         *	@param ctx ctx
         *	@param dr result set
         *	@param trxName transaction
         */
        public MCharge(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {
            //super(ctx, dr, trxName);
        }	//	MCharge

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        //@Override
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (newRecord & success)
                success = Insert_Accounting("C_Charge_Acct", "C_AcctSchema_Default", null);

            return success;
        }	//	aftedrave

        /**
         * 	Before Delete
         *	@return true
         */
        //	@Override
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_Charge_Acct");
        }	//	beforeDelete

    }
}
