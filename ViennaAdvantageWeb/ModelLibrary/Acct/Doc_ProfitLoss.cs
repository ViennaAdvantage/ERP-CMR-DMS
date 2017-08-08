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

namespace ModelLibrary.Acct
{
    class Doc_ProfitLoss: Doc
    {

       // private int C_AcctSchema = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ass"></param>
        /// <param name="idr"></param>
        /// <param name="trxName"></param>
        public Doc_ProfitLoss(MAcctSchema[] ass, IDataReader idr, Trx trxName)
            : base(ass, typeof(MProfitLoss), idr, null, trxName)
        {

        }
        public Doc_ProfitLoss(MAcctSchema[] ass, DataRow dr, Trx trxName)
            : base(ass, typeof(MProfitLoss), dr, null, trxName)
        {

        }

        /// <summary>
        /// Load Specific Document Details
        /// </summary>
        /// <returns>error message or null</returns>
        public override String LoadDocumentDetails()
        {
            MProfitLoss pay = (MProfitLoss)GetPO();
            SetDateDoc(pay.GetDateTrx());            
            _lines = LoadLines(pay);
            log.Fine("Lines=" + _lines.Length);
            return null;
        }
        private DocLine[] LoadLines(MProfitLoss pay)
        {
            List<DocLine> list = new List<DocLine>();
            MProfitLossLines[] lines = pay.GetLines(false);
            //C_AcctSchema = Util.GetValueOfInt(DB.ExecuteScalar("SELECT c_acctschema1_id FROM AD_ClientInfo WHERE AD_Client_ID=" + GetAD_Client_ID()));
            for (int i = 0; i < lines.Length; i++)
            {
                MProfitLossLines line = lines[i];
                DocLine docLine = new DocLine(line, this);
                docLine.SetConvertedAmt(line.GetC_AcctSchema_ID(), line.GetAccountDebit(), line.GetAccountCredit());
                //
                list.Add(docLine);
            }

            //	Return Array
            DocLine[] dls = new DocLine[list.Count];
            dls = list.ToArray();
            return dls;
        }

        private int GetCurrency(int c_acct_Schema_id)
        {
            if (c_acct_Schema_id > 0)
            {
                int Currency_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Currency_ID FROM C_AcctSchema WHERE C_AcctSchema_ID=" +c_acct_Schema_id));
                return Currency_ID;
            }
            return 0;
        }


        /// <summary>
        /// Get Source Currency Balance - subtracts line amounts from total - no rounding
        /// </summary>
        /// <returns>positive amount, if total invoice is bigger than lines</returns>
        public override Decimal GetBalance()
        {
            Decimal retValue = Env.ZERO;
            StringBuilder sb = new StringBuilder(" [");
            //  Total
            retValue = Decimal.Add(retValue, GetAmount(Doc.AMTTYPE_Gross).Value);
            sb.Append(GetAmount(Doc.AMTTYPE_Gross));
            //  - Lines
            for (int i = 0; i < _lines.Length; i++)
            {
                retValue = Decimal.Subtract(retValue, _lines[i].GetAmtSource());
                sb.Append("-").Append(_lines[i].GetAmtSource());
            }
            sb.Append("]");
            //
            log.Fine(ToString() + " Balance=" + retValue + sb.ToString());
            //	return retValue;
            return Env.ZERO;    //  Lines are balanced
        }

        /// <summary>
        /// Create Facts (the accounting logic) for
        /// CMC.
        /// <pre>
        /// Expense
        /// CashExpense     DR
        ///        CashAsset               CR
        ///Receipt
        ///        CashAsset       DR
        ///        CashReceipt             CR
        ///  Charge
        ///        Charge          DR
        ///          CashAsset               CR
        ///  Difference
        ///          CashDifference  DR
        ///          CashAsset               CR
        ///  Invoice
        ///          CashAsset       DR
        ///          CashTransfer            CR
        ///  Transfer
        ///          BankInTransit   DR
        ///          CashAsset               CR
        ///  </pre>
        /// </summary>
        /// <param name="?"></param>
        /// <returns>Fact</returns>
        public override List<Fact> CreateFacts(MAcctSchema as1)
        {
            //  create Fact Header
            List<Fact> facts = new List<Fact>();

            if (GetDocumentType().Equals(MDocBaseType.DOCBASETYPE_PROFITLOSS))
            {
                //	Decimal grossAmt = getAmount(Doc.AMTTYPE_Gross);
                SetC_Currency_ID(GetCurrency(as1.GetC_AcctSchema_ID()));
                //  Commitment
                Fact fact = new Fact(this, as1, Fact.POST_Actual);
                Decimal total = Env.ZERO, totalCredit = Env.ZERO, totalDebit = Env.ZERO;
                Decimal credit = Env.ZERO, debit = Env.ZERO;

                for (int i = 0; i < _lines.Length; i++)
                {
                    DocLine dline = _lines[i];
                    MProfitLossLines line = new MProfitLossLines(GetCtx(), dline.Get_ID(), null);
                    credit = Util.GetValueOfDecimal(dline.GetAmtAcctCr());
                    debit = Util.GetValueOfDecimal(dline.GetAmtAcctDr());
                    if (credit > 0)
                    {
                        totalCredit = Decimal.Add(totalCredit, credit);
                    }
                    if (debit > 0)
                    {
                        totalDebit = Decimal.Add(totalDebit, debit);
                    }

                    //	Account
                    MAccount expense = MAccount.Get(GetCtx(), GetAD_Client_ID(), GetAD_Org_ID(), line.GetC_AcctSchema_ID(), line.GetAccount_ID(), line.GetC_SubAcct_ID(), line.GetM_Product_ID(), line.GetC_BPartner_ID(), line.GetAD_OrgTrx_ID(),
                        line.GetC_LocFrom_ID(), line.GetC_LocTo_ID(), line.GetC_SalesRegion_ID(), line.GetC_Project_ID(), line.GetC_Campaign_ID(), line.GetC_Activity_ID(), line.GetUser1_ID(), line.GetUser2_ID(), line.GetUserElement1_ID(), line.GetUserElement2_ID());

                    fact.CreateLine(dline, expense, GetCurrency(line.GetC_AcctSchema_ID()), debit, credit);
                }
                total = totalCredit - totalDebit;
                if (total != Env.ZERO)
                {
                    int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT IncomeSummary_Acct FROM C_AcctSchema_GL WHERE C_AcctSchema_ID="+as1.GetC_AcctSchema_ID() +" AND AD_Client_ID = " + GetAD_Client_ID()));
                    MAccount acct = MAccount.Get(GetCtx(), validComID);
                    fact.CreateLine(null, acct,GetC_Currency_ID(),total);
                }
                //if (TotalCurrLoss != Env.ZERO)
                //{
                //    int validComID = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT C_ValidCombination_ID FROM C_ValidCombination WHERE Account_ID= ( SELECT C_ElementValue_ID FROM C_ElementValue WHERE Value='82540' AND AD_Client_ID = " + GetAD_Client_ID() + " )"));
                //    MAccount acct = MAccount.Get(GetCtx(), validComID);
                //    TotalCurrLoss = MConversionRate.Convert(GetCtx(), TotalCurrLoss, childCashCurrency, headerCashCurrency, GetAD_Client_ID(), GetAD_Org_ID());
                //    fact.CreateLine(null, acct,
                //     GetC_Currency_ID(), (TotalCurrLoss));
                //}

                facts.Add(fact);
            }
            return facts;            
        }
    }
}
