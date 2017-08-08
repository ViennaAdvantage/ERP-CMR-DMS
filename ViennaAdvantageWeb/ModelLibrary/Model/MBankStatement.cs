/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : MBankStatement
    * Purpose        : Bank Statement Model
    * Class Used     : X_C_BankStatement and DocAction
    * Chronological    Development
    * Raghunandan     01-Aug-2009
******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MBankStatement : X_C_BankStatement, DocAction
    {
        //Lines							
        private MBankStatementLine[] m_lines = null;
        //	Process Message 			
        private String m_processMsg = null;
        //	Just Prepared Flag			
        private bool m_justPrepared = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_BankStatement_ID"></param>
        /// <param name="trxName">Transaction</param>
        public MBankStatement(Ctx ctx, int C_BankStatement_ID, Trx trxName)
            : base(ctx, C_BankStatement_ID, trxName)
        {
            if (C_BankStatement_ID == 0)
            {
                //	setC_BankAccount_ID (0);	//	parent
                SetStatementDate(DateTime.Today.Date);// new DateTime(System.currentTimeMillis()));	// @Date@
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetBeginningBalance(Env.ZERO);
                SetStatementDifference(Env.ZERO);
                SetEndingBalance(Env.ZERO);
                SetIsApproved(false);	// N
                SetIsManual(true);	// Y
                SetPosted(false);	// N
                base.SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">Current context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MBankStatement(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="account">Bank Account</param>
        /// <param name="isManual">Manual statement</param>
        public MBankStatement(MBankAccount account, bool isManual)
            : this(account.GetCtx(), 0, account.Get_TrxName())
        {
            SetClientOrg(account);
            SetC_BankAccount_ID(account.GetC_BankAccount_ID());
            SetStatementDate(DateTime.Today.Date);//new DateTime(System.currentTimeMillis()));
            SetBeginningBalance(account.GetCurrentBalance());
            SetName(GetStatementDate().ToString());
            SetIsManual(isManual);
        }

        /// <summary>
        /// Create a new Bank Statement
        /// </summary>
        /// <param name="account">Bank account</param>
        public MBankStatement(MBankAccount account)
            : this(account, false)
        {

        }

        /// <summary>
        /// Get Bank Statement Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>line array</returns>
        public MBankStatementLine[] GetLines(bool requery)
        {
            if (m_lines != null && !requery)
            {
                return m_lines;
            }
            List<MBankStatementLine> list = new List<MBankStatementLine>();
            String sql = "SELECT * FROM C_BankStatementLine"
                + " WHERE C_BankStatement_ID=@C_BankStatement_ID"
                + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@C_BankStatement_ID", GetC_BankStatement_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)// while (dr.next())
                {
                    list.Add(new MBankStatementLine(GetCtx(), dr, Get_TrxName()));
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
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }

            MBankStatementLine[] retValue = new MBankStatementLine[list.Count];
            retValue = list.ToArray();
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
            {
                SetDescription(description);
            }
            else
            {
                SetDescription(desc + " | " + description);
            }
        }

        /// <summary>
        /// Set Processed.
        /// Propagate to Lines/Taxes
        /// </summary>
        /// <param name="processed">processed</param>
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
            {
                return;
            }
            String sql = "UPDATE C_BankStatementLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_BankStatement_ID=" + GetC_BankStatement_ID();

            int noLine = Convert.ToInt32(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
            m_lines = null;
            log.Fine("setProcessed - " + processed + " - Lines=" + noLine);
        }

        /// <summary>
        /// Get Bank Account
        /// </summary>
        /// <returns>Bank Account</returns>
        public MBankAccount GetBankAccount()
        {
            return MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
        }

        /// <summary>
        /// Set Bank Account
        /// </summary>
        /// <param name="C_BankAccount_ID">acc/id</param>
        public new void SetC_BankAccount_ID(int C_BankAccount_ID)
        {
            base.SetC_BankAccount_ID(C_BankAccount_ID);
        }

        /// <summary>
        /// Set Bank Account - Callout
        /// </summary>
        /// <param name="oldC_BankAccount_ID">Oldbank</param>
        /// <param name="newC_BankAccount_ID">new bank</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetC_BankAccount_ID(String oldC_BankAccount_ID, String newC_BankAccount_ID, int windowNo)
        {
            if (newC_BankAccount_ID == null || newC_BankAccount_ID.Length == 0)
            {
                return;
            }
            int C_BankAccount_ID = int.Parse(newC_BankAccount_ID);
            if (C_BankAccount_ID == 0)
            {
                return;
            }
            SetC_BankAccount_ID(C_BankAccount_ID);
            MBankAccount ba = GetBankAccount();
            SetBeginningBalance(ba.GetCurrentBalance());
        }

        /// <summary>
        /// Get Document No 
        /// </summary>
        /// <returns>name</returns>
        public String GetDocumentNo()
        {
            return GetName();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            return GetBankAccount().GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            //try
            //{
            //    File temp = File.createTempFile(get_TableName() + get_ID() + "_", ".pdf");
            //    return createPDF(temp);
            //}
            //catch (Exception e)
            //{
            //    log.severe("Could not create PDF - " + e.getMessage());
            //}
            return null;
        }

        /// <summary>
        /// Create PDF file
        /// </summary>
        /// <param name="file">output file</param>
        /// <returns>file if success</returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (getCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
            //	if (re == null)
            return null;
            //	return re.getPDF(file);
        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            SetEndingBalance(Decimal.Add(GetBeginningBalance(), GetStatementDifference()));
            return true;
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String processAction)
        {
            m_processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }


        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success </returns>
        public bool UnlockIt()
        {
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success </returns>
        public bool InvalidateIt()
        {
            log.Info("invalidateIt - " + ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid) </returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            m_processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (m_processMsg != null)
            {
                return DocActionVariables.STATUS_INVALID;
            }

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetStatementDate(), MDocBaseType.DOCBASETYPE_BANKSTATEMENT))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            MBankStatementLine[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                m_processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Lines
            Decimal total = Env.ZERO;
            DateTime? minDate = GetStatementDate();
            DateTime? maxDate = minDate;
            for (int i = 0; i < lines.Length; i++)
            {
                MBankStatementLine line = lines[i];
                total = Decimal.Add(total, line.GetStmtAmt());
                if (line.GetDateAcct() < (minDate))//before
                {
                    minDate = line.GetDateAcct();
                }
                if (line.GetDateAcct() > maxDate)//after
                {
                    maxDate = line.GetDateAcct();
                }
            }
            SetStatementDifference(total);
            SetEndingBalance(Decimal.Add(GetBeginningBalance(), total));
            if (!MPeriod.IsOpen(GetCtx(), minDate, MDocBaseType.DOCBASETYPE_BANKSTATEMENT)
                || !MPeriod.IsOpen(GetCtx(), maxDate, MDocBaseType.DOCBASETYPE_BANKSTATEMENT))
            {
                m_processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate()))
            {
                m_processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }


            m_justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
            {
                SetDocAction(DOCACTION_Complete);
            }
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success </returns>
        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public String CompleteIt()
        {
            //	Re-Check
            if (!m_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                {
                    return status;
                }
            }
            //	Implicit Approval
            if (!IsApproved())
            {
                ApproveIt();
            }
            log.Info("completeIt - " + ToString());

            //	Set Payment reconciled
            MBankStatementLine[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MBankStatementLine line = lines[i];
                if (line.GetC_Payment_ID() != 0)
                {
                    MPayment payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_TrxName());
                    payment.SetIsReconciled(true);
                    payment.Save(Get_TrxName());
                }
            }
            //	Update Bank Account
            MBankAccount ba = MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
            ba.SetCurrentBalance(GetEndingBalance());
            ba.Save(Get_TrxName());

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                m_processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Void Document.
        /// </summary>
        /// <returns>false</returns>
        public bool VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                m_processMsg = "Document Closed: " + GetDocStatus();
                SetDocAction(DOCACTION_None);
                return false;
            }

            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                ;
            }
            //	Std Period open?
            else
            {
                if (!MPeriod.IsOpen(GetCtx(), GetStatementDate(), MDocBaseType.DOCBASETYPE_BANKSTATEMENT))
                {
                    m_processMsg = "@PeriodClosed@";
                    return false;
                }

                // is Non Business Day?
                if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetStatementDate()))
                {
                    m_processMsg = Common.Common.NONBUSINESSDAY;
                    return false;
                }


                if (MFactAcct.Delete(Table_ID, GetC_BankStatement_ID(), Get_TrxName()) < 0)
                {
                    return false;	//	could not delete
                }
            }

            //	Set lines to 0
            MBankStatementLine[] lines = GetLines(true);
            for (int i = 0; i < lines.Length; i++)
            {
                MBankStatementLine line = lines[i];
                if (line.GetStmtAmt().CompareTo(Env.ZERO) != 0)
                {
                    String description = Msg.Translate(GetCtx(), "Voided") + " ("
                        + Msg.Translate(GetCtx(), "StmtAmt") + "=" + line.GetStmtAmt();
                    if (line.GetTrxAmt().CompareTo(Env.ZERO) != 0)
                    {
                        description += ", " + Msg.Translate(GetCtx(), "TrxAmt") + "=" + line.GetTrxAmt();
                    }
                    if (line.GetChargeAmt().CompareTo(Env.ZERO) != 0)
                    {
                        description += ", " + Msg.Translate(GetCtx(), "ChargeAmt") + "=" + line.GetChargeAmt();
                    }
                    if (line.GetInterestAmt().CompareTo(Env.ZERO) != 0)
                    {
                        description += ", " + Msg.Translate(GetCtx(), "InterestAmt") + "=" + line.GetInterestAmt();
                    }
                    description += ")";
                    line.AddDescription(description);
                    line.SetStmtAmt(Env.ZERO);
                    line.SetTrxAmt(Env.ZERO);
                    line.SetChargeAmt(Env.ZERO);
                    line.SetInterestAmt(Env.ZERO);
                    line.Save(Get_TrxName());
                    if (line.GetC_Payment_ID() != 0)
                    {
                        MPayment payment = new MPayment(GetCtx(), line.GetC_Payment_ID(), Get_TrxName());
                        payment.SetIsReconciled(false);
                        payment.Save(Get_TrxName());
                    }
                }
            }
            AddDescription(Msg.Translate(GetCtx(), "Voided"));
            Decimal voidedDifference = GetStatementDifference();
            SetStatementDifference(Env.ZERO);

            //	Update Bank Account
            MBankAccount ba = MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
            ba.SetCurrentBalance(Decimal.Subtract(ba.GetCurrentBalance(), voidedDifference));
            ba.Save(Get_TrxName());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns> true if success </returns>
        public bool CloseIt()
        {
            log.Info("closeIt - " + ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseCorrectIt()
        {
            log.Info("reverseCorrectIt - " + ToString());
            return false;
        }

        /// <summary>
        /// Reverse Accrual
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info("reverseAccrualIt - " + ToString());
            return false;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            log.Info("reActivateIt - " + ToString());
            return false;
        }


        /// <summary>
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetName());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "StatementDifference")).Append("=").Append(GetStatementDifference())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
            {
                sb.Append(" - ").Append(GetDescription());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return m_processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /// <summary>
        /// Get Document Approval Amount.
        /// Statement Difference
        /// </summary>
        /// <returns>amount</returns>
        public Decimal GetApprovalAmt()
        {
            return GetStatementDifference();
        }

        /// <summary>
        /// Get Document Currency
        /// </summary>
        /// <returns>c_currency_id</returns>
        public int GetC_Currency_ID()
        {
            /*/	MPriceList pl = MPriceList.get(getCtx(), getM_PriceList_ID());
            //	return pl.getC_Currency_ID();*/
            return 0;
        }

        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }



        public void SetProcessMsg(string processMsg)
        {

        }



        #endregion

    }
}
