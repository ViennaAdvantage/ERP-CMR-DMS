/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : Doc_Payment
 * Purpose        : Post Invoice Documents.
 *                  <pre>
 *                   Table:              C_Payment (335)
 *                   Document Types      ARP, APP
 *                   </pre>
 * Class Used     : Doc
 * Chronological    Development
 * Raghunandan      19-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Acct;

namespace VAdvantage.Acct
{
    public class Doc_Payment : Doc
    {
        //	Tender Type			
        private String _TenderType = null;
        // Prepayment			
        private bool _Prepayment = false;
        // Bank Account			
        private int _C_BankAccount_ID = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_Payment(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MPayment), idr, null, trxName)
        {

        }
        public Doc_Payment(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MPayment), dr, null, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MPayment pay = (MPayment)GetPO();
            SetDateDoc(pay.GetDateTrx());
            _TenderType = pay.GetTenderType();
            _Prepayment = pay.IsPrepayment();
            _C_BankAccount_ID = pay.GetC_BankAccount_ID();
            //	Amount
            SetAmount(Doc.AMTTYPE_Gross, pay.GetPayAmt());
            return null;
        }

        /// <summary>
        /// Get Source Currency Balance - always zero
        /// </summary>
        /// <returns>Zero (always balanced)</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            //log.Config( ToString() + " Balance=" + retValue);
            return retValue;
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        ///  ARP, APP.
        ///<pre>
        ///  ARP
        ///      BankInTransit   DR
        ///      UnallocatedCash         CR
        ///      or Charge/C_Prepayment
        ///  APP
        ///      PaymentSelect   DR
        ///      or Charge/V_Prepayment
        ///      BankInTransit           CR
        ///  CashBankTransfer
        ///      -
        ///  </pre>
        /// </summary>
        /// <param name="as1"></param>
        /// <returns>fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            Fact fact = new Fact(this, as1, Fact.POST_Actual);
            //	Cash Transfer
            if ("X".Equals(_TenderType))
            {
                List<Fact> facts = new List<Fact>();
                facts.Add(fact);
                return facts;
            }

            int AD_Org_ID = GetBank_Org_ID();		//	Bank Account Org	
            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_ARRECEIPT))
            {
                //	Asset
                FactLine fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1),
                    GetC_Currency_ID(), GetAmount(), null);
                if (fl != null && AD_Org_ID != 0)
                {
                    fl.SetAD_Org_ID(AD_Org_ID);
                }
                //	
                MAccount acct = null;
                if (GetC_Charge_ID() != 0)
                {
                    acct = MCharge.GetAccount(GetC_Charge_ID(), as1, GetAmount());
                }
                else if (_Prepayment)
                {
                    acct = GetAccount(Doc.ACCTTYPE_C_Prepayment, as1);
                }
                else
                {
                    acct = GetAccount(Doc.ACCTTYPE_UnallocatedCash, as1);
                }
                fl = fact.CreateLine(null, acct,
                    GetC_Currency_ID(), null, GetAmount());
                if (fl != null && AD_Org_ID != 0
                    && GetC_Charge_ID() == 0)		//	don't overwrite charge
                {
                    fl.SetAD_Org_ID(AD_Org_ID);
                }
            }
            //  APP
            else if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_APPAYMENT))
            {
                MAccount acct = null;
                if (GetC_Charge_ID() != 0)
                {
                    acct = MCharge.GetAccount(GetC_Charge_ID(), as1, GetAmount());
                }
                else if (_Prepayment)
                {
                    acct = GetAccount(Doc.ACCTTYPE_V_Prepayment, as1);
                }
                else
                {
                    acct = GetAccount(Doc.ACCTTYPE_PaymentSelect, as1);
                }
                FactLine fl = fact.CreateLine(null, acct,
                    GetC_Currency_ID(), GetAmount(), null);
                if (fl != null && AD_Org_ID != 0
                    && GetC_Charge_ID() == 0)		//	don't overwrite charge
                    fl.SetAD_Org_ID(AD_Org_ID);

                //	Asset
                fl = fact.CreateLine(null, GetAccount(Doc.ACCTTYPE_BankInTransit, as1),
                    GetC_Currency_ID(), null, GetAmount());
                if (fl != null && AD_Org_ID != 0)
                {
                    fl.SetAD_Org_ID(AD_Org_ID);
                }
            }
            else
            {
                _error = "DocumentType unknown: " + GetDocumentType();
                log.Log(Level.SEVERE, _error);
                fact = null;
            }
            //
            List<Fact> facts1 = new List<Fact>();
            facts1.Add(fact);
            return facts1;
        }

        /// <summary>
        /// Get AD_Org_ID from Bank Account
        /// </summary>
        /// <returns>AD_Org_ID or 0</returns>
        private int GetBank_Org_ID()
        {
            if (_C_BankAccount_ID == 0)
            {
                return 0;
            }
            //
            MBankAccount ba = MBankAccount.Get(GetCtx(), _C_BankAccount_ID);
            return ba.GetAD_Org_ID();
        }

    }
}
