/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCash
 * Purpose        : Cash Journal Model
 * Class Used     : X_C_Cash, DocAction
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
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;
using VAdvantage.Print;

namespace VAdvantage.Model
{
    public class MCash : X_C_Cash, DocAction
    {
        #region variables
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MCash).FullName);
        /**	Lines					*/
        private MCashLine[] _lines = null;
        /** CashBook				*/
        private MCashBook _book = null;
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;
        #endregion

        /**
         * 	Get Cash Journal for currency, org and date
         *	@param ctx context
         *	@param C_Currency_ID currency
         *	@param AD_Org_ID org
         *	@param dateAcct date
         *	@param trxName transaction
         *	@return cash
         */
        public static MCash Get(Ctx ctx, int AD_Org_ID, DateTime? dateAcct, int C_Currency_ID, Trx trxName)
        {
            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.AD_Org_ID=" + AD_Org_ID						//	#1
                + " AND TRUNC(c.StatementDate, 'DD')=@sdate"			//	#2
                + " AND c.Processed='N'"
                + " AND EXISTS (SELECT * FROM C_CashBook cb "
                    + "WHERE c.C_CashBook_ID=cb.C_CashBook_ID AND cb.AD_Org_ID=c.AD_Org_ID"
                    + " AND cb.C_Currency_ID=" + C_Currency_ID + ")";			//	#3
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));

                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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

            if (retValue != null)
                return retValue;

            //	Get CashBook
            MCashBook cb = MCashBook.Get(ctx, AD_Org_ID, C_Currency_ID);
            if (cb == null)
            {
                _log.Warning("No CashBook for AD_Org_ID=" + AD_Org_ID + ", C_Currency_ID=" + C_Currency_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            retValue.Save(trxName);
            return retValue;
        }

        /**
         * 	Get Cash Journal for CashBook and date
         *	@param ctx context
         *	@param C_CashBook_ID cashbook
         *	@param dateAcct date
         *	@param trxName transaction
         *	@return cash
         */
        public static MCash Get(Ctx ctx, int C_CashBook_ID, DateTime? dateAcct, Trx trxName)
        {
            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.C_CashBook_ID=" + C_CashBook_ID					//	#1
                + " AND TRUNC(c.StatementDate,'DD')=@sdate"			//	#2
                + " AND c.Processed='N'";
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));
                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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

            if (retValue != null)
                return retValue;

            //	Get CashBook
            MCashBook cb = new MCashBook(ctx, C_CashBook_ID, trxName);
            if (cb.Get_ID() == 0)
            {
                _log.Warning("Not found C_CashBook_ID=" + C_CashBook_ID);
                return null;
            }

            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            retValue.Save(trxName);
            return retValue;
        }
        //Amit 10-9-2014 - Correspity Work
        public static MCash Get(Ctx ctx, int AD_Org_ID, DateTime? dateAcct, int C_Currency_ID, int C_CashBook_ID, int C_DocType_ID, Trx trxName)
        {
            MCash retValue = null;
            //	Existing Journal
            String sql = "SELECT * FROM C_Cash c "
                + "WHERE c.AD_Org_ID=" + AD_Org_ID						//	#1
                + " AND TRUNC(c.StatementDate, 'DD')=@sdate"			//	#2
                + " AND c.Processed='N'"
                + " AND EXISTS (SELECT * FROM C_CashBook cb "
                    + "WHERE c.C_CashBook_ID=" + C_CashBook_ID + " AND cb.AD_Org_ID=c.AD_Org_ID)";			//	#3
            DataTable dt = null;
            SqlParameter[] param = null;
            IDataReader idr = null;
            try
            {
                param = new SqlParameter[1];
                param[0] = new SqlParameter("@sdate", TimeUtil.GetDay(dateAcct));

                idr = DB.ExecuteReader(sql, param, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCash(ctx, dr, trxName);
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

            if (retValue != null)
                return retValue;

            //	Get CashBook
            //MCashBook cb = MCashBook.Get(ctx, AD_Org_ID, C_Currency_ID);
            //if (cb == null)
            //{
            //    _log.Warning("No CashBook for AD_Org_ID=" + AD_Org_ID + ", C_Currency_ID=" + C_Currency_ID);
            //    return null;
            //}
            MCashBook cb = new MCashBook(ctx, C_CashBook_ID, trxName);
            if (cb == null)
            {
                _log.Warning("No CashBook");
                return null;
            }

            //Get Closing Balance of Last Record of that cashbook
            //sql = @"SELECT EndingBalance FROM C_Cash WHERE IsActive    = 'Y' AND C_CashBook_ID = "+ C_CashBook_ID
            //         + " AND c_cash_id = (SELECT MAX(C_Cash_ID) FROM C_Cash WHERE IsActive = 'Y' AND C_CashBook_ID = " + C_CashBook_ID+ ")";
            sql = @"SELECT SUM(completedbalance + runningbalance)AS BegningBalance FROM c_cashbook WHERE IsActive = 'Y' AND  c_cashbook_id =" + C_CashBook_ID;
            decimal beginingBalance = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, trxName));


            //	Create New Journal
            retValue = new MCash(cb, dateAcct);
            retValue.SetC_DocType_ID(C_DocType_ID);
            retValue.SetBeginningBalance(beginingBalance);
            retValue.Save(trxName);
            return retValue;
        }
        //Amit
        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param C_Cash_ID id
         *	@param trxName transaction
         */
        public MCash(Ctx ctx, int C_Cash_ID, Trx trxName)
            : base(ctx, C_Cash_ID, trxName)
        {
            if (C_Cash_ID == 0)
            {
                //	setC_CashBook_ID (0);		//	FK
                SetBeginningBalance(Env.ZERO);
                SetEndingBalance(Env.ZERO);
                SetStatementDifference(Env.ZERO);
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                //
                DateTime today = TimeUtil.GetDay(DateTime.Now);
                SetStatementDate(today);	// @#Date@
                SetDateAcct(today);	// @#Date@
                //String name = DisplayType.getDateFormat(DisplayType.Date).format(today) + " " + MOrg.Get(ctx, GetAD_Org_ID()).GetValue();
                String name = String.Format("{0:d}", today) + " " + MOrg.Get(ctx, GetAD_Org_ID()).GetValue();
                SetName(name);
                SetIsApproved(false);
                SetPosted(false);	// N
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MCash(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param cb cash book
         *	@param today date - if null today
         */
        public MCash(MCashBook cb, DateTime? today)
            : this(cb.GetCtx(), 0, cb.Get_TrxName())
        {
            SetClientOrg(cb);
            SetC_CashBook_ID(cb.GetC_CashBook_ID());
            if (today != null)
            {
                SetStatementDate(today);
                SetDateAcct(today);
                //String name = DisplayType.getDateFormat(DisplayType.Date).format(today) + " " + cb.GetName();
                String name = String.Format("{0:d}", today) + " " + cb.GetName();
                SetName(name);
            }
            _book = cb;
        }



        /**
         * 	Get Lines
         *	@param requery requery
         *	@return lines
         */
        public MCashLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MCashLine> list = new List<MCashLine>();
            String sql = "SELECT * FROM C_CashLine WHERE C_Cash_ID=" + GetC_Cash_ID() + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MCashLine(GetCtx(), dr, Get_TrxName()));
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
            finally { dt = null; }

            _lines = new MCashLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Get Cash Book
         *	@return cash book
         */
        public MCashBook GetCashBook()
        {
            if (_book == null)
                _book = MCashBook.Get(GetCtx(), GetC_CashBook_ID());
            return _book;
        }

        /**
         * 	Get Document No 
         *	@return name
         */
        public String GetDocumentNo()
        {
            return GetName();
        }

        /**
         * 	Get Document Info
         *	@return document info (untranslated)
         */
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "C_Cash_ID") + " " + GetDocumentNo();
        }

        ///**
        // * 	Create PDF
        // *	@return File or null
        // */
        //public File createPDF ()
        //{
        //    try
        //    {
        //        File temp = File.createTempFile(get_TableName()+get_ID()+"_", ".pdf");
        //        return createPDF (temp);
        //    }
        //    catch (Exception e)
        //    {
        //        //log.severe("Could not create PDF - " + e.getMessage());
        //    }
        //    return null;
        //}	//	getPDF

        ///**
        // * 	Create PDF file
        // *	@param file output file
        // *	@return file if success
        // */
        //public File createPDF (File file)
        //{
        ////	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
        ////	if (re == null)
        //        return null;
        ////	return re.getPDF(file);
        //}	

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            //try
            //{
            //    string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
            //                        + ".txt"; //.pdf
            //    string filePath = Path.GetTempPath() + fileName;

            //    //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
            //    //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

            //    FileInfo temp = new FileInfo(filePath);
            //    if (!temp.Exists)
            //    {
            //        return CreatePDF(temp);
            //    }
            //}
            //catch (Exception e)
            //{
            //    log.Severe("Could not create PDF - " + e.Message);
            //}
            //return null;






            try
            {
                //string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                String fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo() + ".pdf";
                string filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload", fileName);

                int processID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Process_Id FROM AD_Process WHERE VALUE='CashJournalReport'", null, null));
                MPInstance instance = new MPInstance(GetCtx(), processID, GetC_Cash_ID());
                instance.Save();
                ProcessInfo pi = new ProcessInfo("", processID, Get_Table_ID(), GetC_Cash_ID());
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

                ProcessCtl ctl = new ProcessCtl();
                ctl.IsArabicReportFromOutside = false;
                byte[] report = null;
                ReportEngine_N re = null;
                Dictionary<string, object> d = ctl.Process(pi, GetCtx(), out report, out re);

                File.WriteAllBytes(filePath, report);
                return new FileInfo(filePath);
                //re.GetView();
                //bool b = re.CreatePDF(filePath);

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                //FileInfo temp = new FileInfo(filePath);

                //if (!temp.Exists)
                //{
                //    b = re.CreatePDF(filePath);
                //    if (b)
                //    {
                //        return new FileInfo(filePath);
                //    }
                //    return null;
                //}
                //else
                //    return temp;
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public FileInfo CreatePDF(FileInfo file)
        {
            ////ReportEngine re = ReportEngine.Get(GetCtx(), ReportEngine.ORDER, GetC_Order_ID());
            ////if (re == null)
            ////    return null;
            ////return re.getPDF(file);

            //Create a file to write to.
            using (StreamWriter sw = file.CreateText())
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }

            return file;            

        }

        /**
         * 	Set StatementDate - Callout
         *	@param oldStatementDate old
         *	@param newStatementDate new
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetStatementDate(String oldStatementDate, String newStatementDate, int windowNo)
        {
            if (newStatementDate == null || newStatementDate.Length == 0)
                return;
            DateTime statementDate = Convert.ToDateTime(PO.ConvertToTimestamp(newStatementDate));
            if (statementDate == null)
                return;
            SetStatementDate(statementDate);
        }

        /**
         *	Set Statement Date and Acct Date
         */
        public void SetStatementDate(DateTime? statementDate)
        {
            base.SetStatementDate(statementDate);
            base.SetDateAcct(statementDate);
        }

        /**
         * 	Before Save
         *	@param newRecord
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            SetAD_Org_ID(GetCashBook().GetAD_Org_ID());
            if (GetAD_Org_ID() == 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@AD_Org_ID@"));
                return false;
            }
            //	Calculate End Balance
            SetEndingBalance(Decimal.Add(GetBeginningBalance(), GetStatementDifference()));
            return true;
        }

        /**
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /**
         *	Prepare Document
         * 	@return new status (In Progress or Invalid) 
         */
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), MDocBaseType.DOCBASETYPE_CASHJOURNAL))
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


            MCashLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Add up Amounts
            Decimal difference = Env.ZERO;
            int C_Currency_ID = GetC_Currency_ID();
            for (int i = 0; i < lines.Length; i++)
            {
                MCashLine line = lines[i];
                if (!line.IsActive())
                    continue;
                if (C_Currency_ID == line.GetC_Currency_ID())
                    difference = Decimal.Add(difference, line.GetAmount());
                else
                {
                    Decimal amt = MConversionRate.Convert(GetCtx(), line.GetAmount(),
                        line.GetC_Currency_ID(), C_Currency_ID, GetDateAcct(), 0,
                        GetAD_Client_ID(), GetAD_Org_ID());
                    if (amt == null)
                    {
                        _processMsg = "No Conversion Rate found - @C_CashLine_ID@= " + line.GetLine();
                        return DocActionVariables.STATUS_INVALID;
                    }
                    difference = Decimal.Add(difference, amt);
                }
            }
            SetStatementDifference(difference);
            //	setEndingBalance(getBeginningBalance().add(getStatementDifference()));
            //
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /**
         * 	Complete Document
         * 	@return new status (Complete, In Progress, Invalid, Waiting ..)
         */
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
            //
            log.Info(ToString());

            //	Allocation Header
            MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false,
                GetDateAcct(), GetC_Currency_ID(),
                Msg.Translate(GetCtx(), "C_Cash_ID") + ": " + GetName(), Get_TrxName());
            alloc.SetAD_Org_ID(GetAD_Org_ID());
            if (!alloc.Save())
            {
                _processMsg = "Could not create Allocation Hdr";
                return DocActionVariables.STATUS_INVALID;
            }
            //
            MCashLine[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MCashLine line = lines[i];

                if (Util.GetValueOfInt(line.GetC_InvoicePaySchedule_ID()) != 0)
                {
                    MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), Util.GetValueOfInt(line.GetC_InvoicePaySchedule_ID()), Get_TrxName());
                    paySch.SetC_CashLine_ID(line.GetC_CashLine_ID());
                    paySch.Save();
                }
                else
                {
                    int[] InvoicePaySchedule_ID = MInvoicePaySchedule.GetAllIDs("C_InvoicePaySchedule", "C_Invoice_ID = " + line.GetC_Invoice_ID() + @" AND C_InvoicePaySchedule_ID NOT IN 
                    (SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Payment_ID IN (SELECT NVL(C_Payment_ID,0) FROM C_InvoicePaySchedule) UNION 
                    SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule  WHERE C_Cashline_ID IN (SELECT NVL(C_Cashline_ID,0) FROM C_InvoicePaySchedule))", Get_TrxName());

                    foreach (int invocePay in InvoicePaySchedule_ID)
                    {
                        MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), invocePay, Get_TrxName());
                        paySch.SetC_CashLine_ID(line.GetC_CashLine_ID());
                        paySch.Save();
                    }
                }

                if (MCashLine.CASHTYPE_Invoice.Equals(line.GetCashType()))
                {
                    bool differentCurrency = GetC_Currency_ID() != line.GetC_Currency_ID();
                    MAllocationHdr hdr = alloc;
                    if (differentCurrency)
                    {
                        hdr = new MAllocationHdr(GetCtx(), false,
                            GetDateAcct(), line.GetC_Currency_ID(),
                            Msg.Translate(GetCtx(), "C_Cash_ID") + ": " + GetName(), Get_TrxName());
                        hdr.SetAD_Org_ID(GetAD_Org_ID());
                        if (!hdr.Save())
                        {
                            _processMsg = "Could not create Allocation Hdr";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                    //	Allocation Line
                    MAllocationLine aLine = new MAllocationLine(hdr, line.GetAmount(),
                        line.GetDiscountAmt(), line.GetWriteOffAmt(), line.GetOverUnderAmt());
                    aLine.SetC_Invoice_ID(line.GetC_Invoice_ID());
                    aLine.SetC_CashLine_ID(line.GetC_CashLine_ID());
                    if (!aLine.Save())
                    {
                        _processMsg = "Could not create Allocation Line";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    if (differentCurrency)
                    {
                        //	Should start WF
                        hdr.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                        hdr.Save();
                    }
                }
                else if (MCashLine.CASHTYPE_BankAccountTransfer.Equals(line.GetCashType()))
                {
                    //	Payment just as intermediate info
                    MPayment pay = new MPayment(GetCtx(), 0, Get_TrxName());
                    pay.SetAD_Org_ID(GetAD_Org_ID());
                    String documentNo = GetName();
                    pay.SetDocumentNo(documentNo);
                    pay.SetR_PnRef(documentNo);
                    pay.Set_Value("TrxType", "X");		//	Transfer
                    pay.Set_Value("TenderType", "X");
                    //
                    pay.SetC_BankAccount_ID(line.GetC_BankAccount_ID());
                    pay.SetC_DocType_ID(true);	//	Receipt
                    pay.SetDateTrx(GetStatementDate());
                    pay.SetDateAcct(GetDateAcct());
                    pay.SetAmount(line.GetC_Currency_ID(), Decimal.Negate(line.GetAmount()));	//	Transfer
                    pay.SetDescription(line.GetDescription());
                    pay.SetDocStatus(MPayment.DOCSTATUS_Closed);
                    pay.SetDocAction(MPayment.DOCACTION_None);
                    pay.SetPosted(true);
                    pay.SetIsAllocated(true);	//	Has No Allocation!
                    pay.SetProcessed(true);
                    if (!pay.Save())
                    {
                        _processMsg = "Could not create Payment";
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
                // Added to Update Open Balance of Business Partner
                else if (MCashLine.CASHTYPE_BusinessPartner.Equals(line.GetCashType()))
                {
                    if (line.GetC_BPartner_ID() != 0)
                    {
                        Decimal? UpdatedBal = 0;
                        MBPartner bp = new MBPartner(GetCtx(), line.GetC_BPartner_ID(), Get_TrxName());

                        Decimal? cashAmt = VAdvantage.Model.MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(line.GetAmount(), line.GetDiscountAmt()), line.GetWriteOffAmt()),
                       GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                        if (cashAmt > 0)
                        {
                            UpdatedBal = Decimal.Subtract((Decimal)bp.GetTotalOpenBalance(), (Decimal)cashAmt);

                            Decimal? newCreditAmt = bp.GetSO_CreditUsed();
                            if (newCreditAmt == null)
                                newCreditAmt = Decimal.Negate((Decimal)cashAmt);
                            else
                                newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)cashAmt);
                            //
                            log.Fine("TotalOpenBalance=" + bp.GetTotalOpenBalance(false) + "(" + cashAmt
                                + ", Credit=" + bp.GetSO_CreditUsed() + "->" + newCreditAmt
                                + ", Balance=" + bp.GetTotalOpenBalance(false) + " -> " + UpdatedBal);
                            bp.SetSO_CreditUsed((Decimal)newCreditAmt);
                        }
                        else
                        {
                            UpdatedBal = Decimal.Subtract((Decimal)bp.GetTotalOpenBalance(), (Decimal)cashAmt);
                            log.Fine("Payment Amount =" + line.GetAmount() + "(" + cashAmt
                                + ") Balance=" + bp.GetTotalOpenBalance(false) + " -> " + UpdatedBal);
                        }
                        bp.SetTotalOpenBalance(Convert.ToDecimal(UpdatedBal));
                        bp.SetSOCreditStatus();
                        if (!bp.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not update Business Partner";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }
            //	Should start WF
            alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
            alloc.Save();

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            if (!UpdateCompletedBalance())
            {
                _processMsg = "Could not update Header";
                return VAdvantage.Process.DocActionVariables.STATUS_INVALID;
            }

            return VAdvantage.Process.DocActionVariables.STATUS_COMPLETED;
        }

        private bool UpdateCompletedBalance()
        {
            MCashBook cashbook = new MCashBook(GetCtx(), GetC_CashBook_ID(), Get_TrxName());
            cashbook.SetCompletedBalance(Decimal.Add(cashbook.GetCompletedBalance(), GetStatementDifference()));
            cashbook.SetRunningBalance(Decimal.Subtract(cashbook.GetRunningBalance(), GetStatementDifference()));

            if (!cashbook.Save())
            {
                return false;
            }
            return true;
        }

        /**
         * 	Void Document.
         * 	Same as Close.
         * 	@return true if success 
         */
        public bool VoidIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return false;
        }

        /**
         * 	Close Document.
         * 	Cancel not delivered Qunatities
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return true if success 
         */
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return false;
        }

        /**
         * 	Reverse Accrual - none
         * 	@return true if success 
         */
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return true if success 
         */
        public bool ReActivateIt()
        {
            log.Info(ToString());
            SetProcessed(false);
            if (ReverseCorrectIt())
                return true;
            return false;
        }

        /**
         * 	Set Processed
         *	@param processed processed
         */
        public void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            String sql = "UPDATE C_CashLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_Cash_ID=" + GetC_Cash_ID();
            int noLine = DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }

        /**
         * 	String Representation
         *	@return info
         */
        public String ToString()
        {
            StringBuilder sb = new StringBuilder("MCash[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(", Balance=").Append(GetBeginningBalance())
                .Append("->").Append(GetEndingBalance())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetName());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "BeginningBalance")).Append("=").Append(GetBeginningBalance())
                .Append(",")
                .Append(Msg.Translate(GetCtx(), "EndingBalance")).Append("=").Append(GetEndingBalance())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /**
         * 	Get Process Message
         *	@return clear text error message
         */
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /**
         * 	Get Document Owner (Responsible)
         *	@return AD_User_ID
         */
        public int GetDoc_User_ID()
        {
            return GetCreatedBy();
        }

        /**
         * 	Get Document Approval Amount
         *	@return amount difference
         */
        public Decimal GetApprovalAmt()
        {
            return GetStatementDifference();
        }

        /**
         * 	Get Currency
         *	@return Currency
         */
        public int GetC_Currency_ID()
        {
            return GetCashBook().GetC_Currency_ID();
        }



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
    }
}
