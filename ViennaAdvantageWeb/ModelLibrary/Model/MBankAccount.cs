/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_BankAccount
 * Chronological Development
 * Veena Pandey     24-June-2009
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

namespace VAdvantage.Model
{
    public class MBankAccount : X_C_BankAccount
    {
        /**	Cache						*/
        private static CCache<int, MBankAccount> _cache
            = new CCache<int, MBankAccount>("C_BankAccount", 5);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BankAccount_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MBankAccount(Ctx ctx, int C_BankAccount_ID, Trx trxName)
            : base(ctx, C_BankAccount_ID, trxName)
        {
            if (C_BankAccount_ID == 0)
            {
                SetIsDefault(false);
                SetBankAccountType(BANKACCOUNTTYPE_Checking);
                SetCurrentBalance(Env.ZERO);
                SetUnMatchedBalance(Env.ZERO);
                //	SetC_Currency_ID (0);
                SetCreditLimit(Env.ZERO);
                //	SetC_BankAccount_ID (0);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MBankAccount(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * 	Get BankAccount from Cache
	     *	@param ctx context
	     *	@param C_BankAccount_ID id
	     *	@return MBankAccount
	     */
        public static MBankAccount Get(Ctx ctx, int C_BankAccount_ID)
        {
            int key = C_BankAccount_ID;
            MBankAccount retValue = (MBankAccount)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MBankAccount(ctx, C_BankAccount_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
	     * 	Get Bank
	     *	@return bank parent
	     */
        public MBank GetBank()
        {
            return MBank.Get(GetCtx(), GetC_Bank_ID());
        }

        /**
         * 	Get Bank Name and Account No
         *	@return Bank/Account
         */
        public String GetName()
        {
            return GetBank().GetName() + " " + GetAccountNo();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true if valid
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            MBank bank = GetBank();
            BankVerificationInterface verify = bank.GetVerificationClass();
            if (verify != null)
            {
                String errorMsg = verify.VerifyAccountNo(bank, GetAccountNo());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @AccountNo@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifyBBAN(bank, GetBBAN());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @BBAN@ " + errorMsg);
                    return false;
                }
                errorMsg = verify.VerifyIBAN(bank, GetIBAN());
                if (errorMsg != null)
                {
                    //log.saveError("Error", "@Invalid@ @IBAN@ " + errorMsg);
                    return false;
                }
            }
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
            if (newRecord & success)
                return Insert_Accounting("C_BankAccount_Acct", "C_AcctSchema_Default", null);
            return success;
        }

        /**
         * 	Before Delete
         *	@return true
         */
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_BankAccount_Acct");
        }

        /**
	     * 	String representation
	     *	@return info
	     */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MBankAccount[")
                .Append(Get_ID())
                .Append("-").Append(GetAccountNo())
                .Append("]");
            return sb.ToString();
        }

    }
}
