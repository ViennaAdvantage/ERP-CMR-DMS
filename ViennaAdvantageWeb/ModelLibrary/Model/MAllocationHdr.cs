/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_AllocationHdr
 * Chronological Development
 * Veena Pandey     23-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAllocationHdr : X_C_AllocationHdr, DocAction
    {
        /**	Logger						*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MAllocationHdr).FullName);
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;
        /**	Lines						*/
        private MAllocationLine[] _lines = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AllocationHdr_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAllocationHdr(Ctx ctx, int C_AllocationHdr_ID, Trx trxName)
            : base(ctx, C_AllocationHdr_ID, trxName)
        {
            if (C_AllocationHdr_ID == 0)
            {
                //	setDocumentNo (null);
                SetDateTrx(DateTime.Now);
                SetDateAcct(GetDateTrx());
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                //	setC_Currency_ID (0);
                SetApprovalAmt(Env.ZERO);
                SetIsApproved(false);
                SetIsManual(false);
                //
                SetPosted(false);
                SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MAllocationHdr(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Mandatory New Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="IsManual">manual trx</param>
        /// <param name="dateTrx">date (if null today)</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="description">description</param>
        /// <param name="trxName">transaction</param>
        public MAllocationHdr(Ctx ctx, bool IsManual, DateTime? dateTrx, int C_Currency_ID,
            String description, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetIsManual(IsManual);
            if (dateTrx != null)
            {
                SetDateTrx(dateTrx);
                SetDateAcct(dateTrx);
            }
            SetC_Currency_ID(C_Currency_ID);
            if (description != null)
                SetDescription(description);
        }

        /// <summary>
        /// Get Allocations of Payment
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Payment_ID">payment</param>
        /// <param name="trxName">transaction</param>
        /// <returns>allocations of payment</returns>
        public static MAllocationHdr[] GetOfPayment(Ctx ctx, int C_Payment_ID, Trx trxName)
        {
            String sql = "SELECT * FROM C_AllocationHdr h "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM C_AllocationLine l "
                    + "WHERE h.C_AllocationHdr_ID=l.C_AllocationHdr_ID AND l.C_Payment_ID=" + C_Payment_ID + ")";
            List<MAllocationHdr> list = new List<MAllocationHdr>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MAllocationHdr(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MAllocationHdr[] retValue = new MAllocationHdr[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Allocations of Invoice
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="trxName">transaction</param>
        /// <returns>allocations of invoice</returns>
        public static MAllocationHdr[] GetOfInvoice(Ctx ctx, int C_Invoice_ID, Trx trxName)
        {
            String sql = "SELECT * FROM C_AllocationHdr h "
                + "WHERE IsActive='Y'"
                + " AND EXISTS (SELECT * FROM C_AllocationLine l "
                    + "WHERE h.C_AllocationHdr_ID=l.C_AllocationHdr_ID AND l.C_Invoice_ID=" + C_Invoice_ID + ")";
            List<MAllocationHdr> list = new List<MAllocationHdr>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MAllocationHdr(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            MAllocationHdr[] retValue = new MAllocationHdr[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="requery">if true requery</param>
        /// <returns>lines</returns>
        public MAllocationLine[] GetLines(bool requery)
        {
            if (_lines != null && _lines.Length != 0 && !requery)
                return _lines;
            //
            String sql = "SELECT * FROM C_AllocationLine WHERE C_AllocationHdr_ID=" + GetC_AllocationHdr_ID();
            List<MAllocationLine> list = new List<MAllocationLine>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MAllocationLine line = new MAllocationLine(GetCtx(), dr, Get_TrxName());
                        line.SetParent(this);
                        list.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            _lines = new MAllocationLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Set Processed
        /// </summary>
        /// <param name="processed">processed</param>
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE C_AllocationHdr SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_AllocationHdr_ID=" + GetC_AllocationHdr_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - #" + no);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Changed from Not to Active
            if (!newRecord && Is_ValueChanged("IsActive") && IsActive())
            {
                log.Severe("Cannot Re-Activate deactivated Allocations");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if acct was deleted</returns>
        protected override bool BeforeDelete()
        {
            Trx trxName = Get_Trx();
            if (trxName == null)
            {
                log.Warning("No transaction");
            }
            if (IsPosted())
            {
                if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION))
                {
                    log.Warning("Period Closed");
                    return false;
                }
                //// is Non Business Day?
                //if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateTrx()))
                //{
                //    log.Warning("DateIsInNonBusinessDay");
                //    return false;
                //}

                SetPosted(false);
                if (MFactAcct.Delete(Table_ID, Get_ID(), trxName) < 0)
                    return false;
            }
            //	Mark as Inactive
            SetIsActive(false);		//	updated DB for line delete/process
            String sql = "UPDATE C_AllocationHdr SET IsActive='N' WHERE C_AllocationHdr_ID=" + GetC_AllocationHdr_ID();
            DataBase.DB.ExecuteQuery(sql, null, trxName);

            //	Unlink
            GetLines(true);
            HashSet<int> bps = new HashSet<int>();
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                bps.Add(line.GetC_BPartner_ID());
                if (!line.Delete(true, trxName))
                    return false;
            }
            UpdateBP(bps);
            return true;
        }

        /// <summary>
        /// After save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            return success;
        }

        /// <summary>
        /// Process document
        /// </summary>
        /// <param name="processAction">document action</param>
        /// <returns>true if performed</returns>
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /// <summary>
        /// Unlock Document.
        /// </summary>
        /// <returns>true if success</returns>
        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /// <summary>
        /// Invalidate Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /// <summary>
        /// Prepare Document
        /// </summary>
        /// <returns>new status (In Progress or Invalid)</returns>
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate
                (this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }
            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }

            GetLines(false);
            if (_lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Add up Amounts & validate
            Decimal approval = Env.ZERO;
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                approval = Decimal.Add(Decimal.Add(approval, line.GetWriteOffAmt()), line.GetDiscountAmt());
                //	Make sure there is BP
                if (line.GetC_BPartner_ID() == 0)
                {
                    _processMsg = "No Business Partner";
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            SetApprovalAmt(approval);
            //
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success</returns>
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success</returns>
        public bool RejectIt()
        {
            log.Info(ToString());
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
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }
            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());

            //	Link
            GetLines(false);
            HashSet<int> bps = new HashSet<int>();
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                bps.Add(line.ProcessIt(false));	//	not reverse
            }
            UpdateBP(bps);

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate
                (this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Void Document.
        ///	Same as Close.
        /// </summary>
        /// <returns>true if success</returns>
        public bool VoidIt()
        {
            log.Info(ToString());
            bool retValue = ReverseIt();
            SetDocAction(DOCACTION_None);
            return retValue;
        }

        /// <summary>
        /// Close Document.
        ///	Cancel not delivered Qunatities
        /// </summary>
        /// <returns>true if success</returns>
        public bool CloseIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction
        /// </summary>
        /// <returns>true if success</returns>
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            bool retValue = ReverseIt();
            SetDocAction(DOCACTION_None);
            return retValue;
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            bool retValue = ReverseIt();
            SetDocAction(DOCACTION_None);
            return retValue;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAllocationHdr[");
            sb.Append(Get_ID()).Append("-").Append(GetSummary()).Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Document Info
        /// </summary>
        /// <returns>document info (untranslated)</returns>
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "C_AllocationHdr_ID") + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>file or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
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
        /// Get Summary
        /// </summary>
        /// <returns>Summary of Document</returns>
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "ApprovalAmt")).Append("=").Append(GetApprovalAmt())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /// <summary>
        /// Get Process Message
        /// </summary>
        /// <returns>clear text error message</returns>
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /// <summary>
        /// Get Document Owner (Responsible)
        /// </summary>
        /// <returns>AD_User_ID</returns>
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }

        /// <summary>
        /// Reverse Allocation.
        /// Period needs to be open
        /// </summary>
        /// <returns>true if reversed</returns>
        private bool ReverseIt()
        {
            if (!IsActive())
                throw new Exception("Allocation already reversed (not active)");

            //	Can we delete posting
            if (!MPeriod.IsOpen(GetCtx(), GetDateTrx(), MDocBaseType.DOCBASETYPE_PAYMENTALLOCATION))
                throw new Exception("@PeriodClosed@");
            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateTrx()))
            {
                throw new Exception(Common.Common.NONBUSINESSDAY);
            }

            //	Set Inactive
            SetIsActive(false);
            SetDocumentNo(GetDocumentNo() + "^");
            SetDocStatus(DOCSTATUS_Reversed);	//	for direct calls
            if (!Save() || IsActive())
                throw new Exception("Cannot de-activate allocation");

            //	Delete Posting
            String sql = "DELETE FROM Fact_Acct WHERE AD_Table_ID=" + MAllocationHdr.Table_ID
                + " AND Record_ID=" + GetC_AllocationHdr_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            log.Fine("Fact_Acct deleted #" + no);

            //	Unlink Invoices
            GetLines(true);
            HashSet<int> bps = new HashSet<int>();
            for (int i = 0; i < _lines.Length; i++)
            {
                MAllocationLine line = _lines[i];
                line.SetIsActive(false);
                line.Save();
                bps.Add(line.ProcessIt(true));	//	reverse
            }
            UpdateBP(bps);
            return true;
        }

        /// <summary>
        /// Update Open Balance of BP's
        /// </summary>
        /// <param name="bps">list of business partners</param>
        private void UpdateBP(HashSet<int> bps)
        {
            log.Info("#" + bps.Count);
            IEnumerator<int> it = bps.GetEnumerator();
            it.Reset();
            while (it.MoveNext())
            {
                int C_BPartner_ID = it.Current;
                MBPartner bp = new MBPartner(GetCtx(), C_BPartner_ID, Get_TrxName());
                bp.SetTotalOpenBalance();		//	recalculates from scratch
                //	bp.setSOCreditStatus();			//	called automatically
                if (bp.Save())
                {
                    log.Fine(bp.ToString());
                }
                else
                {
                    log.Log(Level.SEVERE, "BP not updated - " + bp);
                }
            }
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