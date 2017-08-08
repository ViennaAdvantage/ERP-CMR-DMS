/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MRequisition
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan     07-July-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MRequisition : X_M_Requisition, DocAction
    {
        //	Process Message 		
        private String _processMsg = null;
        //	Just Prepared Flag		
        private bool _justPrepared = false;
        //Lines						
        private MRequisitionLine[] _lines = null;
        MStorage storage = null;
        /**
        * 	Standard Constructor
        *	@param Ctx context
        *	@param M_Requisition_ID id
        */
        public MRequisition(Ctx ctx, int M_Requisition_ID, Trx trxName)
            : base(ctx, M_Requisition_ID, trxName)
        {
            try
            {

                if (M_Requisition_ID == 0)
                {
                    //	setDocumentNo (null);
                    //	setAD_User_ID (0);
                    //	setM_PriceList_ID (0);
                    //	setM_Warehouse_ID(0);
                    //SetDateDoc(new Timestamp(System.currentTimeMillis()));
                    SetDateDoc(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                    //SetDateDoc(Convert.ToDateTime(DateTime.Now));
                    //setDateRequired(new Timestamp(System.currentTimeMillis()));
                    SetDateRequired(CommonFunctions.CovertMilliToDate(CommonFunctions.CurrentTimeMillis()));
                    //SetDateRequired(Convert.ToDateTime(DateTime.Now));
                    SetDocAction(DocActionVariables.ACTION_COMPLETE);	// CO
                    SetDocStatus(DocActionVariables.STATUS_DRAFTED);		// DR
                    SetPriorityRule(PRIORITYRULE_Medium);	// 5
                    SetTotalLines(Env.ZERO);
                    SetIsApproved(false);
                    SetPosted(false);
                    SetProcessed(false);
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("MRequisition--Standard Constructor");
                log.Severe(ex.ToString());
            }
        }

        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         */
        public MRequisition(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }


        /**
         * 	Get Lines
         *	@return array of lines
         */
        public MRequisitionLine[] GetLines()
        {
            try
            {
                if (_lines != null)
                {
                    return _lines;
                }

                List<MRequisitionLine> list = new List<MRequisitionLine>();
                String sql = "SELECT * FROM M_RequisitionLine WHERE M_Requisition_ID=" + GetM_Requisition_ID() + " ORDER BY Line";
                DataTable dt = null;
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql.ToString(), null, Get_TrxName());
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        list.Add(new MRequisitionLine(GetCtx(), dr, Get_TrxName()));
                    }
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                   log.Log(Level.SEVERE, "getLines", e);
                }
                finally {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    dt = null;
                }
                _lines = new MRequisitionLine[list.Count];
                _lines = list.ToArray();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MRequisition--GetLines");
                log.Severe(ex.ToString());
            }
            return _lines;
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MRequisition[");
            sb.Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",Status=").Append(GetDocStatus()).Append(",Action=").Append(GetDocAction())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info
         */
        public String GetDocumentInfo()
        {
            //return Msg.getElement(getContext(), "M_Requisition_ID") + " " + getDocumentNo();
            return Msg.GetElement(GetCtx(), "M_Requisition_ID") + " " + GetDocumentNo();
        }

        /**
         * 	Create PDF
         *	@return File or null
         */
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
            //Create a file to write to.
            return null;
        }


        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            /*	ReportEngine re = ReportEngine.get (getContext(), ReportEngine.INVOICE, getC_Invoice_ID());
                if (re == null)*/
            return null;
            //return file;
            /*	return re.getPDF(file);*/
        }

        /**
         * 	Set default PriceList
         */
        public void SetM_PriceList_ID()
        {
            try
            {
                MPriceList defaultPL = MPriceList.GetDefault(GetCtx(), false);
                if (defaultPL == null)
                {
                    defaultPL = MPriceList.GetDefault(GetCtx(), true);
                }
                if (defaultPL != null)
                {
                    SetM_PriceList_ID(defaultPL.GetM_PriceList_ID());
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("MRequisition--SetM_PriceList_ID");
                log.Severe(ex.ToString());
            }
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetM_PriceList_ID() == 0)
            {
                SetM_PriceList_ID();
            }
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
            log.Info("unlockIt - " + ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
        {
            log.Info("invalidateIt - " + ToString());
            return true;
        }

        /**
         *	Prepare Document
         * 	@return new status (In Progress or Invalid) 
         */
        public String PrepareIt()
        {
            try
            {
                log.Info(ToString());
                _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
                if (_processMsg != null)
                    return DocActionVariables.STATUS_INVALID;
                MRequisitionLine[] lines = GetLines();

                //	Invalid
                if (GetAD_User_ID() == 0
                    || GetM_PriceList_ID() == 0
                    || GetM_Warehouse_ID() == 0
                    || lines.Length == 0)
                    return DocActionVariables.STATUS_INVALID;

                //	Std Period open?
                if (!MPeriod.IsOpen(GetCtx(), GetDateDoc(), MDocBaseType.DOCBASETYPE_PURCHASEREQUISITION))
                {
                    _processMsg = "@PeriodClosed@";
                    return DocActionVariables.STATUS_INVALID;
                }

                // is Non Business Day?
                if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateDoc()))
                {
                    _processMsg = Common.Common.NONBUSINESSDAY;
                    return DocActionVariables.STATUS_INVALID;
                }



                //	Add up Amounts
                int precision = MPriceList.GetStandardPrecision(GetCtx(), GetM_PriceList_ID());
                Decimal totalLines = Env.ZERO;
                for (int i = 0; i < lines.Length; i++)
                {
                    MRequisitionLine line = lines[i];
                    Decimal lineNet = Decimal.Multiply(line.GetQty(), line.GetPriceActual());
                    lineNet = Decimal.Round(lineNet, precision);//, MidpointRounding.AwayFromZero);
                    if (lineNet.CompareTo(line.GetLineNetAmt()) != 0)
                    {
                        line.SetLineNetAmt(lineNet);
                        line.Save();
                    }
                    totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
                }
                if (totalLines.CompareTo(GetTotalLines()) != 0)
                {
                    SetTotalLines(totalLines);
                    Save();
                }
                _justPrepared = true;
            }
            catch (Exception ex)
            {
               // MessageBox.Show("MRequisition--PrepareIt");
                log.Severe(ex.ToString());
            }
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public bool ApproveIt()
        {
            log.Info("approveIt - " + ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public bool RejectIt()
        {
            log.Info("rejectIt - " + ToString());
            SetIsApproved(false);
            return true;
        }

        /**
         * 	Complete Document
         * 	@return new status (Complete, In Progress, Invalid, Waiting ..)
         */
        public String CompleteIt()
        {
            try
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
                {
                    ApproveIt();
                }
                log.Info(ToString());

                //	User Validation
                String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
                if (valid != null)
                {
                    _processMsg = valid;
                    return DocActionVariables.STATUS_INVALID;
                }
                //
                SetProcessed(true);
                SetDocAction(DocActionVariables.ACTION_CLOSE);
                /**************************************************************************************************************/
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("DTD001_", out mInfo))
                {
                    MRequisitionLine[] lines = GetLines();
                    for (int i = 0; i < lines.Length; i++)
                    {
                        MRequisitionLine line = lines[i];
                        int loc_id = GetLocation(GetM_Warehouse_ID());
                        if (loc_id == 0)
                        {
                            //return Msg.GetMsg(GetCtx(),"MMPM_DefineLocator");
                            _processMsg = "Define Locator For That Warehouse";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
                        {
                            storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), null);
                            if (storage == null)
                            {
                                storage = MStorage.GetCreate(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), null);
                            }
                            storage.SetDTD001_QtyReserved((Decimal.Add(storage.GetDTD001_QtyReserved(), (Decimal)line.GetQty())));
                            if (!storage.Save())
                            {
                                log.Info("Requisition Reserverd Quantity not saved at storage at locator " + loc_id + " and product is " + line.GetM_Product_ID());
                            }
                        }
                        else
                        {
                            storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), 0, null);
                            if (storage == null)
                            {
                                //MStorage.Add(GetCtx(), GetM_Warehouse_ID(), loc_id, line.GetM_Product_ID(), 0, 0, 0, 0, line.GetQty(), null);
                                MStorage.Add(GetCtx(), GetM_Warehouse_ID(), loc_id, line.GetM_Product_ID(), 0, 0, (Decimal)0, (Decimal)0, (Decimal)0, line.GetQty(), null);
                            }
                            else
                            {
                                storage.SetDTD001_QtyReserved((Decimal.Add(storage.GetDTD001_QtyReserved(), (Decimal)line.GetQty())));
                                storage.Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MRequisition--CompleteIt");
                log.Severe(ex.ToString());
            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        public int GetLocation(int ware_ID)
        {
            //string qry = @"SELECT sg.M_Locator_ID FROM M_Storage sg INNER JOIN M_Locator loc ON sg.M_Locator_ID=loc.M_Locator_ID INNER JOIN M_Warehouse wh ON loc.M_Warehouse_ID=wh.M_Warehouse_ID where wh.M_Warehouse_ID = " + ware_ID + " and sg.M_Product_ID = " + prod_ID + " and loc.IsActive = 'Y'";
            //IDataReader idrloc =DB.ExecuteReader(qry);
            int location = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_Locator where M_Warehouse_ID = " + ware_ID + " and IsActive = 'Y' and IsDefault='Y'"));
            if (location > 0)
            {
                return location;
            }
            else
            {
                //qry = @"SELECT sg.M_Locator_ID FROM M_Storage sg INNER JOIN M_Locator loc ON sg.M_Locator_ID=loc.M_Locator_ID INNER JOIN M_Warehouse wh ON loc.M_Warehouse_ID=wh.M_Warehouse_ID where wh.M_Warehouse_ID = " + ware_ID + " and loc.IsDefault='Y' and loc.IsActive = 'Y'";
                location = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_Locator_ID FROM M_Locator where M_Warehouse_ID = " + ware_ID + " and IsActive = 'Y'"));
                if (location > 0)
                {
                    return location;
                }
                else
                {
                    return 0;
                }
            }
        }
        /**
         * 	Void Document.
         * 	Same as Close.
         * 	@return true if success 
         */
        public bool VoidIt()
        {
            log.Info("voidIt - " + ToString());
            return CloseIt();
        }

        /**
         * 	Close Document.
         * 	Cancel not delivered Qunatities
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            try
            {
                log.Info("closeIt - " + ToString());
                //	Close Not delivered Qty
                MRequisitionLine[] lines = GetLines();
                Decimal totalLines = Env.ZERO;
                for (int i = 0; i < lines.Length; i++)
                {
                    MRequisitionLine line = lines[i];
                    Decimal finalQty = line.GetQty();
                    if (line.GetC_OrderLine_ID() == 0)
                    {
                        finalQty = Env.ZERO;
                    }
                    else
                    {
                        MOrderLine ol = new MOrderLine(GetCtx(), line.GetC_OrderLine_ID(), Get_TrxName());
                        finalQty = ol.GetQtyOrdered();
                    }
                    Tuple<String, String, String> mInfo = null;
                    if (Env.HasModulePrefix("DTD001_", out mInfo))
                    {
                        int quant = Util.GetValueOfInt(line.GetQty() - line.GetDTD001_DeliveredQty());
                        //Update storage requisition reserved qty
                        if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
                        {
                            if (GetDocAction() != "VO" && GetDocStatus() != "DR")
                            {
                                if (quant > 0)
                                {
                                    int loc_id = GetLocation(GetM_Warehouse_ID());
                                    storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), null);
                                    if (storage == null)
                                    {
                                        storage = MStorage.GetCreate(GetCtx(), loc_id, line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), null);
                                    }
                                    storage.SetDTD001_QtyReserved((Decimal.Subtract(storage.GetDTD001_QtyReserved(), (Decimal)quant)));
                                    storage.Save();
                                }
                            }
                        }
                        else if (GetDocAction() != "VO" && GetDocStatus() != "DR")
                        {
                            if (quant > 0)
                            {
                                int loc_id = GetLocation(GetM_Warehouse_ID());
                                storage = MStorage.Get(GetCtx(), loc_id, line.GetM_Product_ID(), 0, null);
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), loc_id, line.GetM_Product_ID(), 0, null);
                                }
                                storage.SetDTD001_QtyReserved((Decimal.Subtract(storage.GetDTD001_QtyReserved(), (Decimal)quant)));
                                storage.Save();
                            }
                        }
                    }
                    //	final qty is not line qty
                    if (finalQty.CompareTo(line.GetQty()) != 0)
                    {
                        String description = line.GetDescription();
                        if (description == null)
                            description = "";
                        description += " [" + line.GetQty() + "]";
                        line.SetDescription(description);
                        // Amit 9-feb-2015 
                        // line.SetQty(finalQty);
                        //Amit
                        line.SetLineNetAmt();
                        line.Save();
                    }
                    //get Grand Total or SubTotal
                    totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
                }
                if (totalLines.CompareTo(GetTotalLines()) != 0)
                {
                    SetTotalLines(totalLines);
                    Save();
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show("MRequisition--CloseIt");
                log.Severe(ex.ToString());
            }
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return true if success 
         */
        public bool ReverseCorrectIt()
        {
            log.Info("reverseCorrectIt - " + ToString());
            return false;
        }

        /**
         * 	Reverse Accrual - none
         * 	@return true if success 
         */
        public bool ReverseAccrualIt()
        {
            log.Info("reverseAccrualIt - " + ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return true if success 
         */
        public bool ReActivateIt()
        {
            log.Info("reActivateIt - " + ToString());
            //	setProcessed(false);
            if (ReverseCorrectIt())
            {
                return true;
            }
            return false;
        }

        /*
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	 - User
            sb.Append(" - ").Append(GetUserName());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ").
                Append(Msg.Translate(GetCtx(), "TotalLines")).Append("=").Append(GetTotalLines())
                .Append(" (#").Append(GetLines().Length).Append(")");
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
         * 	Get Document Owner
         *	@return AD_User_ID
         */
        public int GetDoc_User_ID()
        {
            return GetAD_User_ID();
        }

        /**
         * 	Get Document Currency
         *	@return C_Currency_ID
         */
        public int GetC_Currency_ID()
        {
            MPriceList pl = MPriceList.Get(GetCtx(), GetM_PriceList_ID(), Get_TrxName());
            return pl.GetC_Currency_ID();
        }

        /**
         * 	Get Document Approval Amount
         *	@return amount
         */
        public Decimal GetApprovalAmt()
        {
            return GetTotalLines();
        }

        /**
         * 	Get User Name
         *	@return user name
         */
        public String GetUserName()
        {
            return MUser.Get(GetCtx(), GetAD_User_ID()).GetName();
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
