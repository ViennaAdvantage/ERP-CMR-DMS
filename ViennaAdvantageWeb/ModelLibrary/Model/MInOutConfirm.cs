/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInOutConfirm
 * Purpose        : Shipment Confirmation Model
 * Class Used     : X_M_InOutConfirm, DocAction
 * Chronological    Development
 * Raghunandan     05-Jun-2009
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
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInOutConfirm : X_M_InOutConfirm, DocAction
    {
        /**	Confirm Lines					*/
        private MInOutLineConfirm[] _lines = null;
        /** Credit Memo to create			*/
        private MInvoice _creditMemo = null;
        /**	Physical Inventory to create	*/
        private MInventory _inventory = null;

        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private Boolean _justPrepared = false;
        /**	Static Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MInOutConfirm).FullName);


        /**
	 * 	Create Confirmation or return existing one
	 *	@param ship shipment
	 *	@param confirmType confirmation type
	 *	@param checkExisting if false, new confirmation is created
	 *	@return Confirmation
	 */
        public static MInOutConfirm Create(MInOut ship, String confirmType, Boolean checkExisting)
        {
            if (checkExisting)
            {
                MInOutConfirm[] confirmations = ship.GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MInOutConfirm confirm = confirmations[i];
                    if (confirm.GetConfirmType().Equals(confirmType))
                    {
                        _log.Info("create - existing: " + confirm);
                        return confirm;
                    }
                }
            }

            MInOutConfirm confirm1 = new MInOutConfirm(ship, confirmType);
            confirm1.Save(ship.Get_TrxName());
            MInOutLine[] shipLines = ship.GetLines(false);
            for (int i = 0; i < shipLines.Length; i++)
            {
                MInOutLine sLine = shipLines[i];
                MInOutLineConfirm cLine = new MInOutLineConfirm(confirm1);
                cLine.SetInOutLine(sLine);
                cLine.Save(ship.Get_TrxName());
            }
            _log.Info("New: " + confirm1);
            return confirm1;
        }

        /***
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_InOutConfirm_ID id
         *	@param trxName transaction
         */
        public MInOutConfirm(Ctx ctx, int M_InOutConfirm_ID, Trx trxName) :
            base(ctx, M_InOutConfirm_ID, trxName)
        {

            if (M_InOutConfirm_ID == 0)
            {
                //	setConfirmType (null);
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetIsApproved(false);
                SetIsCancelled(false);
                SetIsInDispute(false);
                base.SetProcessed(false);
            }
        }	

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result set
         *	@param trxName transaction
         */
        public MInOutConfirm(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }	

        /**
         * 	Parent Constructor
         *	@param ship shipment
         *	@param confirmType confirmation type
         */
        public MInOutConfirm(MInOut ship, String confirmType)
            : this(ship.GetCtx(), 0, ship.Get_TrxName())
        {

            SetClientOrg(ship);
            SetM_InOut_ID(ship.GetM_InOut_ID());
            SetConfirmType(confirmType);
        }	


        /**
         * 	Get Lines
         *	@param requery requery
         *	@return array of lines
         */
        public MInOutLineConfirm[] GetLines(Boolean requery)
        {
            if (_lines != null && !requery)
                return _lines;
            String sql = "SELECT * FROM M_InOutLineConfirm "
                + "WHERE M_InOutConfirm_ID=" + GetM_InOutConfirm_ID();
            List<MInOutLineConfirm> list = new List<MInOutLineConfirm>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInOutLineConfirm(GetCtx(), dr, Get_TrxName()));
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
            }


            _lines = new MInOutLineConfirm[list.Count];
            _lines = list.ToArray();
            return _lines;
        }	//	getLines

        /**
         * 	Add to Description
         *	@param description text
         */
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

        /**
         * 	Get Name of ConfirmType
         *	@return confirm type
         */
        public String GetConfirmTypeName()
        {
            return MRefList.GetListName(GetCtx(), CONFIRMTYPE_AD_Reference_ID, GetConfirmType());
        }	

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInOutConfirm[");
            sb.Append(Get_ID()).Append("-").Append(GetSummary())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info (untranslated)
         */
        public String GetDocumentInfo()
        {
            return Msg.GetElement(GetCtx(), "M_InOutConfirm_ID") + " " + GetDocumentNo();
        }	

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

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

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (GetCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
            //	if (re == null)
            //return null;
            //	return re.getPDF(file);

            using (StreamWriter sw = file.CreateText())
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }

            return file;
        }

        /**
         * 	Set Approved
         *	@param IsApproved approval
         */
        public new void SetIsApproved(Boolean isApproved)
        {
            if (isApproved && !IsApproved())
            {
                int AD_User_ID = GetCtx().GetAD_User_ID();
                MUser user = MUser.Get(GetCtx(), AD_User_ID);
                String Info = user.GetName()
                    + ": "
                    + Msg.Translate(GetCtx(), "IsApproved")
                    + " - " + DateTime.Now.ToString();
                AddDescription(Info);
            }
            base.SetIsApproved(isApproved);
        }


        /**
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public Boolean ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }	

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public Boolean UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }	

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public Boolean InvalidateIt()
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
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this,
                ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            /***********Compier comment
            MDocType dt = MDocType.Get(GetCtx(), getC_DocTypeTarget_ID());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), getDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }
            ****/

            MInOutLineConfirm[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	Set dispute if not fully confirmed
            Boolean difference = false;
            for (int i = 0; i < lines.Length; i++)
            {
                if (!lines[i].IsFullyConfirmed())
                {
                    difference = true;
                    break;
                }
            }
            SetIsInDispute(difference);

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
        public Boolean ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }	

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public Boolean RejectIt()
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
            log.Info(ToString());
            //
            MInOut inout = new MInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
            MInOutLineConfirm[] lines = GetLines(false);

            //	Check if we need to split Shipment
            if (IsInDispute())
            {
                MDocType dt = MDocType.Get(GetCtx(), inout.GetC_DocType_ID());
                if (dt.IsSplitWhenDifference())
                {
                    if (dt.GetC_DocTypeDifference_ID() == 0)
                    {
                        _processMsg = "No Split Document Type defined for: " + dt.GetName();
                        return DocActionVariables.STATUS_INVALID;
                    }
                    SplitInOut(inout, dt.GetC_DocTypeDifference_ID(), lines);
                    _lines = null;
                }
            }

            //	All lines
            for (int i = 0; i < lines.Length; i++)
            {
                MInOutLineConfirm confirmLine = lines[i];
                confirmLine.Set_TrxName(Get_TrxName());
                if (!confirmLine.ProcessLine(inout.IsSOTrx(), GetConfirmType()))
                {
                    _processMsg = "ShipLine not saved - " + confirmLine;
                    return DocActionVariables.STATUS_INVALID;
                }
                if (confirmLine.IsFullyConfirmed())
                {
                    confirmLine.SetProcessed(true);
                    confirmLine.Save(Get_TrxName());
                }
                else
                {
                    if (CreateDifferenceDoc(inout, confirmLine))
                    {
                        confirmLine.SetProcessed(true);
                        confirmLine.Save(Get_TrxName());
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Scrapped=" + confirmLine.GetScrappedQty()
                        	+ " - Difference=" + confirmLine.GetDifferenceQty());

                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }	//	for all lines

            if (_creditMemo != null)
                _processMsg += " @C_Invoice_ID@=" + _creditMemo.GetDocumentNo();
            if (_inventory != null)
                _processMsg += " @M_Inventory_ID@=" + _inventory.GetDocumentNo();


            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this,
                ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }	

        /**
         * 	Split Shipment into confirmed and dispute
         *	@param original original shipment
         *	@param C_DocType_ID target DocType
         *	@param confirmLines confirm lines
         */
        private void SplitInOut(MInOut original, int C_DocType_ID, MInOutLineConfirm[] confirmLines)
        {
            MInOut split = new MInOut(original, C_DocType_ID, original.GetMovementDate());
            split.AddDescription("Splitted from " + original.GetDocumentNo());
            split.SetIsInDispute(true);

            //nnayak : Change for bug 1431337
            split.SetRef_InOut_ID(original.Get_ID());

            if (!split.Save(Get_TrxName()))
                throw new Exception("Cannot save Split");
            original.AddDescription("Split: " + split.GetDocumentNo());
            if (!original.Save(Get_TrxName()))
                throw new Exception("Cannot update original Shipment");

            //	Go through confirmations 
            for (int i = 0; i < confirmLines.Length; i++)
            {
                MInOutLineConfirm confirmLine = confirmLines[i];
                Decimal differenceQty = confirmLine.GetDifferenceQty();
                if (differenceQty.CompareTo(Env.ZERO) == 0)
                    continue;
                //
                MInOutLine oldLine = confirmLine.GetLine();
                 log.Fine("Qty=" + differenceQty + ", Old=" + oldLine);
                //
                MInOutLine splitLine = new MInOutLine(split);
                splitLine.SetC_OrderLine_ID(oldLine.GetC_OrderLine_ID());
                splitLine.SetC_UOM_ID(oldLine.GetC_UOM_ID());
                splitLine.SetDescription(oldLine.GetDescription());
                splitLine.SetIsDescription(oldLine.IsDescription());
                splitLine.SetLine(oldLine.GetLine());
                splitLine.SetM_AttributeSetInstance_ID(oldLine.GetM_AttributeSetInstance_ID());
                splitLine.SetM_Locator_ID(oldLine.GetM_Locator_ID());
                splitLine.SetM_Product_ID(oldLine.GetM_Product_ID());
                splitLine.SetM_Warehouse_ID(oldLine.GetM_Warehouse_ID());
                splitLine.SetRef_InOutLine_ID(oldLine.GetRef_InOutLine_ID());
                splitLine.AddDescription("Split: from " + oldLine.GetMovementQty());
                //	Qtys
                splitLine.SetQty(differenceQty);		//	Entered/Movement
                if (!splitLine.Save(Get_TrxName()))
                    throw new Exception("Cannot save Split Line");
                //	Old
                oldLine.AddDescription("Splitted: from " + oldLine.GetMovementQty());
                oldLine.SetQty(Decimal.Subtract(oldLine.GetMovementQty(), differenceQty));
                if (!oldLine.Save(Get_TrxName()))
                    throw new Exception("Cannot save Splited Line");
                //	Update Confirmation Line
                confirmLine.SetTargetQty(Decimal.Subtract(confirmLine.GetTargetQty(), differenceQty));
                confirmLine.SetDifferenceQty(Env.ZERO);
                if (!confirmLine.Save(Get_TrxName()))
                    throw new Exception("Cannot save Split Confirmation");
            }	//	for all confirmations

            _processMsg = "Split @M_InOut_ID@=" + split.GetDocumentNo()
                + " - @M_InOutConfirm_ID@=";

            //	Create Dispute Confirmation
            split.ProcessIt(DocActionVariables.ACTION_PREPARE);
            //	split.createConfirmation();
            split.Save(Get_TrxName());
            MInOutConfirm[] splitConfirms = split.GetConfirmations(true);
            if (splitConfirms.Length > 0)
            {
                int index = 0;
                if (splitConfirms[index].IsProcessed())
                {
                    if (splitConfirms.Length > 1)
                        index++;	//	try just next
                    if (splitConfirms[index].IsProcessed())
                    {
                        _processMsg += splitConfirms[index].GetDocumentNo() + " processed??";
                        return;
                    }
                }
                splitConfirms[index].SetIsInDispute(true);
                splitConfirms[index].Save(Get_TrxName());
                _processMsg += splitConfirms[index].GetDocumentNo();
                //	Set Lines to unconfirmed
                MInOutLineConfirm[] splitConfirmLines = splitConfirms[index].GetLines(false);
                for (int i = 0; i < splitConfirmLines.Length; i++)
                {
                    MInOutLineConfirm splitConfirmLine = splitConfirmLines[i];
                    splitConfirmLine.SetScrappedQty(Env.ZERO);
                    splitConfirmLine.SetConfirmedQty(Env.ZERO);
                    splitConfirmLine.Save(Get_TrxName());
                }
            }
            else
            {
                _processMsg += "??";
            }

        }	


        /**
         * 	Create Difference Document
         * 	@param inout shipment/receipt
         *	@param confirm confirm line
         *	@return true if created
         */
        private bool CreateDifferenceDoc(MInOut inout, MInOutLineConfirm confirm)
        {
            if (_processMsg == null)
                _processMsg = "";
            else if (_processMsg.Length > 0)
                _processMsg += "; ";
            //	Credit Memo if linked Document
            if (Env.Signum(confirm.GetDifferenceQty()) != 0
                && !inout.IsSOTrx() && !inout.IsReturnTrx() && inout.GetRef_InOut_ID() != 0)
            {
                log.Info("Difference=" + confirm.GetDifferenceQty());
                if (_creditMemo == null)
                {
                    _creditMemo = new MInvoice(inout, null);
                    _creditMemo.SetDescription(Msg.Translate(GetCtx(),
                        "M_InOutConfirm_ID") + " " + GetDocumentNo());
                    _creditMemo.SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_APCREDITMEMO);
                    if (!_creditMemo.Save(Get_TrxName()))
                    {
                        _processMsg += "Credit Memo not created";
                        return false;
                    }
                    SetC_Invoice_ID(_creditMemo.GetC_Invoice_ID());
                }
                MInvoiceLine line = new MInvoiceLine(_creditMemo);
                line.SetShipLine(confirm.GetLine());
                line.SetQty(confirm.GetDifferenceQty());	//	Entered/Invoiced
                if (!line.Save(Get_TrxName()))
                {
                    _processMsg += "Credit Memo Line not created";
                    return false;
                }
                confirm.SetC_InvoiceLine_ID(line.GetC_InvoiceLine_ID());
            }

            //	Create Inventory Difference
            if (Env.Signum(confirm.GetScrappedQty()) != 0)
            {
                log.Info("Scrapped=" + confirm.GetScrappedQty());
                if (_inventory == null)
                {
                    MWarehouse wh = MWarehouse.Get(GetCtx(), inout.GetM_Warehouse_ID());
                    _inventory = new MInventory(wh);
                    _inventory.SetDescription(Msg.Translate(GetCtx(),
                        "M_InOutConfirm_ID") + " " + GetDocumentNo());
                    if (!_inventory.Save(Get_TrxName()))
                    {
                        _processMsg += "Inventory not created";
                        return false;
                    }
                    SetM_Inventory_ID(_inventory.GetM_Inventory_ID());
                }
                MInOutLine ioLine = confirm.GetLine();
                MInventoryLine line = new MInventoryLine(_inventory,
                    ioLine.GetM_Locator_ID(), ioLine.GetM_Product_ID(),
                    ioLine.GetM_AttributeSetInstance_ID(),
                    confirm.GetScrappedQty(), Env.ZERO);
                if (!line.Save(Get_TrxName()))
                {
                    _processMsg += "Inventory Line not created";
                    return false;
                }
                confirm.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
            }

            //
            if (!confirm.Save(Get_TrxName()))
            {
                _processMsg += "Confirmation Line not saved";
                return false;
            }
            return true;
        }	

        /**
         * 	Void Document.
         * 	@return false 
         */
        public bool VoidIt()
        {
            log.Info(ToString());
            return false;
        }	

        /**
         * 	Close Document.
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
         * 	@return false 
         */
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            return false;
        }	

        /**
         * 	Reverse Accrual - none
         * 	@return false 
         */
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }	

        /** 
         * 	Re-activate
         * 	@return false 
         */
        public bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }	

        /**
         * 	Get Summary
         *	@return Summary of Document
         */
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
            return GetUpdatedBy();
        }

        /**
         * 	Get Document Currency
         *	@return C_Currency_ID
         */
        public int GetC_Currency_ID()
        {
            //	MPriceList pl = MPriceList.get(getCtx(), getM_PriceList_ID());
            //	return pl.getC_Currency_ID();
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
