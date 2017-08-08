/********************************************************
 * Class Name     : MInvoice
 * Purpose        : Calculate the invoice using C_Invoice table
 * Class Used     : X_C_Invoice, DocAction
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
//using java.io;
//using System.IO;
using VAdvantage.Logging;
using VAdvantage.Print;


namespace VAdvantage.Model
{
    public class MInvoice : X_C_Invoice, DocAction
    {
        #region Variables
        //	Open Amount		
        private Decimal? _openAmt = null;

        //	Invoice Lines	
        private MInvoiceLine[] _lines;
        //	Invoice Taxes	
        private MInvoiceTax[] _taxes;
        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;

        /** Reversal Flag		*/
        private bool _reversal = false;
        //	Cache					
        private static CCache<int, MInvoice> _cache = new CCache<int, MInvoice>("C_Invoice", 20, 2);	//	2 minutes
        //	Logger			
        private static VLogger _log = VLogger.GetVLogger(typeof(MInvoice).FullName);
        #endregion

        /// <summary>
        /// Get Payments Of BPartner
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_BPartner_ID">id</param>
        /// <param name="trxName">transaction</param>
        /// <returns>Array</returns>
        public static MInvoice[] GetOfBPartner(Ctx ctx, int C_BPartner_ID, Trx trxName)
        {
            List<MInvoice> list = new List<MInvoice>();
            String sql = "SELECT * FROM C_Invoice WHERE C_BPartner_ID=" + C_BPartner_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInvoice(ctx, dr, trxName));
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
            finally
            {
                dt = null;
            }

            MInvoice[] retValue = new MInvoice[list.Count];

            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///	Create new Invoice by copying
        /// </summary>
        /// <param name="from">invoice</param>
        /// <param name="dateDoc">date of the document date</param>
        /// <param name="C_DocTypeTarget_ID">target doc type</param>
        /// <param name="counter">sales order</param>
        /// <param name="trxName">trx</param>
        /// <param name="setOrder">set Order links</param>
        /// <returns></returns>
        public static MInvoice CopyFrom(MInvoice from, DateTime? dateDoc, int C_DocTypeTarget_ID,
            Boolean counter, Trx trxName, Boolean setOrder)
        {
            MInvoice to = new MInvoice(from.GetCtx(), 0, null);
            to.Set_TrxName(trxName);
            PO.CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("C_Invoice_ID", I_ZERO);
            to.Set_ValueNoCheck("DocumentNo", null);
            //
            to.SetDocStatus(DOCSTATUS_Drafted);		//	Draft
            to.SetDocAction(DOCACTION_Complete);
            //
            to.SetC_DocType_ID(0);
            to.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID, true);
            //
            to.SetDateInvoiced(dateDoc);
            to.SetDateAcct(dateDoc);
            to.SetDatePrinted(null);
            to.SetIsPrinted(false);
            //
            to.SetIsApproved(false);
            to.SetC_Payment_ID(0);
            to.SetC_CashLine_ID(0);
            to.SetIsPaid(false);
            to.SetIsInDispute(false);
            //
            //	Amounts are updated by trigger when adding lines
            to.SetGrandTotal(Env.ZERO);
            to.SetTotalLines(Env.ZERO);
            //
            to.SetIsTransferred(false);
            to.SetPosted(false);
            to.SetProcessed(false);
            //	delete references
            to.SetIsSelfService(false);
            if (!setOrder)
                to.SetC_Order_ID(0);
            if (counter)
            {
                to.SetRef_Invoice_ID(from.GetC_Invoice_ID());
                //	Try to find Order link
                if (from.GetC_Order_ID() != 0)
                {
                    MOrder peer = new MOrder(from.GetCtx(), from.GetC_Order_ID(), from.Get_TrxName());
                    if (peer.GetRef_Order_ID() != 0)
                        to.SetC_Order_ID(peer.GetRef_Order_ID());
                }
            }
            else
                to.SetRef_Invoice_ID(0);

            if (!to.Save(trxName))
                //throw new IllegalException("Could not create Invoice");
                throw new Exception("Could not create Invoice Lines");
            if (counter)
                from.SetRef_Invoice_ID(to.GetC_Invoice_ID());
            //	Lines
            if (to.CopyLinesFrom(from, counter, setOrder) == 0)
                //throw new IllegalStateException("Could not create Invoice Lines");
                throw new Exception("Could not create Invoice Lines");
            return to;
        }

        /// <summary>
        /// Get PDF File Name
        /// </summary>
        /// <param name="documentDir">directory</param>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <returns>file name</returns>
        public static String GetPDFFileName(String documentDir, int C_Invoice_ID)
        {
            StringBuilder sb = new StringBuilder(documentDir);
            if (sb.Length == 0)
                sb.Append(".");
            //if (!sb.ToString().EndsWith(File.separator))
            //    sb.Append(File.separator);
            if (!sb.ToString().EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                sb.Append(Path.AltDirectorySeparatorChar.ToString());
            }
            sb.Append("C_Invoice_ID_")
                .Append(C_Invoice_ID)
                .Append(".pdf");
            return sb.ToString();
        }

        /// <summary>
        /// Get MInvoice from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Invoice_ID">id</param>
        /// <returns>MInvoice</returns>
        public static MInvoice Get(Ctx ctx, int C_Invoice_ID)
        {
            int key = C_Invoice_ID;
            MInvoice retValue = (MInvoice)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MInvoice(ctx, C_Invoice_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Invoice Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Invoice_ID">invoice or 0 for new</param>
        /// <param name="trxName">trx name</param>
        public MInvoice(Ctx ctx, int C_Invoice_ID, Trx trxName) :
            base(ctx, C_Invoice_ID, trxName)
        {
            if (C_Invoice_ID == 0)
            {
                SetDocStatus(DOCSTATUS_Drafted);		//	Draft
                SetDocAction(DOCACTION_Complete);
                //
                //  SetPaymentRule(PAYMENTRULE_OnCredit);	//	Payment Terms

                SetDateInvoiced(DateTime.Now);
                SetDateAcct(DateTime.Now);
                //
                SetChargeAmt(Env.ZERO);
                SetTotalLines(Env.ZERO);
                SetGrandTotal(Env.ZERO);
                //
                SetIsSOTrx(true);
                SetIsTaxIncluded(false);
                SetIsApproved(false);
                SetIsDiscountPrinted(false);
                base.SetIsPaid(false);
                SetSendEMail(false);
                SetIsPrinted(false);
                SetIsTransferred(false);
                SetIsSelfService(false);
                SetIsPayScheduleValid(false);
                SetIsInDispute(false);
                SetPosted(false);
                SetIsReturnTrx(false);
                base.SetProcessed(false);
                SetProcessing(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MInvoice(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Create Invoice from Order
        /// </summary>
        /// <param name="order">order</param>
        /// <param name="C_DocTypeTarget_ID">target document type</param>
        /// <param name="invoiceDate">date or null</param>
        public MInvoice(MOrder order, int C_DocTypeTarget_ID, DateTime? invoiceDate)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {
            try
            {

                SetClientOrg(order);
                SetOrder(order);	//	set base settings
                //
                if (C_DocTypeTarget_ID == 0)
                {
                    C_DocTypeTarget_ID = DataBase.DB.GetSQLValue(null,
                "SELECT C_DocTypeInvoice_ID FROM C_DocType WHERE C_DocType_ID=@param1",
                order.GetC_DocType_ID());
                    //C_DocTypeTarget_ID = DataBase.DB.ExecuteQuery("SELECT C_DocTypeInvoice_ID FROM C_DocType WHERE C_DocType_ID=" + order.GetC_DocType_ID(), null, null);
                }
                SetC_DocTypeTarget_ID(C_DocTypeTarget_ID, true);
                if (invoiceDate != null)
                {
                    SetDateInvoiced(invoiceDate);
                }
                SetDateAcct(GetDateInvoiced());
                //
                SetSalesRep_ID(order.GetSalesRep_ID());
                //
                SetC_BPartner_ID(order.GetBill_BPartner_ID());
                SetC_BPartner_Location_ID(order.GetBill_Location_ID());
                SetAD_User_ID(order.GetBill_User_ID());
            }
            catch { }
        }

        /// <summary>
        /// Create Invoice from Shipment
        /// </summary>
        /// <param name="ship">shipment</param>
        /// <param name="invoiceDate">date or null</param>
        public MInvoice(MInOut ship, DateTime? invoiceDate)
            : this(ship.GetCtx(), 0, ship.Get_TrxName())
        {

            SetClientOrg(ship);
            SetShipment(ship);	//	set base settings
            //
            SetC_DocTypeTarget_ID();
            if (invoiceDate != null)
                SetDateInvoiced(invoiceDate);
            SetDateAcct(GetDateInvoiced());
            //
            SetSalesRep_ID(ship.GetSalesRep_ID());
            SetAD_User_ID(ship.GetAD_User_ID());
        }

        /// <summary>
        /// Create Invoice from Batch Line
        /// </summary>
        /// <param name="batch">batch</param>
        /// <param name="line">batch line</param>
        public MInvoice(MInvoiceBatch batch, MInvoiceBatchLine line)
            : this(line.GetCtx(), 0, line.Get_TrxName())
        {

            SetClientOrg(line);
            SetDocumentNo(line.GetDocumentNo());
            //
            SetIsSOTrx(batch.IsSOTrx());
            MBPartner bp = new MBPartner(line.GetCtx(), line.GetC_BPartner_ID(), line.Get_TrxName());
            SetBPartner(bp);	//	defaults
            //
            SetIsTaxIncluded(line.IsTaxIncluded());
            //	May conflict with default price list
            SetC_Currency_ID(batch.GetC_Currency_ID());
            SetC_ConversionType_ID(batch.GetC_ConversionType_ID());
            //
            //	setPaymentRule(order.getPaymentRule());
            //	setC_PaymentTerm_ID(order.getC_PaymentTerm_ID());
            //	setPOReference("");
            SetDescription(batch.GetDescription());
            //	setDateOrdered(order.getDateOrdered());
            //
            SetAD_OrgTrx_ID(line.GetAD_OrgTrx_ID());
            SetC_Project_ID(line.GetC_Project_ID());
            //	setC_Campaign_ID(line.getC_Campaign_ID());
            SetC_Activity_ID(line.GetC_Activity_ID());
            SetUser1_ID(line.GetUser1_ID());
            SetUser2_ID(line.GetUser2_ID());
            //
            SetC_DocTypeTarget_ID(line.GetC_DocType_ID(), true);
            SetDateInvoiced(line.GetDateInvoiced());
            SetDateAcct(line.GetDateAcct());
            //
            SetSalesRep_ID(batch.GetSalesRep_ID());
            //
            SetC_BPartner_ID(line.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(line.GetC_BPartner_Location_ID());
            SetAD_User_ID(line.GetAD_User_ID());
        }

        /// <summary>
        /// Overwrite Client/Org if required
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">org</param>
        //public void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        //{
        //    base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        //}

        /// <summary>
        /// Set Business Partner Defaults & Details
        /// </summary>
        /// <param name="bp">business partner</param>
        public void SetBPartner(MBPartner bp)
        {
            if (bp == null)
                return;

            SetC_BPartner_ID(bp.GetC_BPartner_ID());
            //	Set Defaults
            int ii = 0;
            if (IsSOTrx())
                ii = bp.GetC_PaymentTerm_ID();
            else
                ii = bp.GetPO_PaymentTerm_ID();
            if (ii != 0)
                SetC_PaymentTerm_ID(ii);
            //
            if (IsSOTrx())
                ii = bp.GetM_PriceList_ID();
            else
                ii = bp.GetPO_PriceList_ID();
            if (ii != 0)
                SetM_PriceList_ID(ii);
            //
            String ss = null;
            if (IsSOTrx())
                ss = bp.GetPaymentRule();
            else
                ss = bp.GetPaymentRulePO();
            if (ss != null)
                SetPaymentRule(ss);


            //	Set Locations
            MBPartnerLocation[] locs = bp.GetLocations(false);
            if (locs != null)
            {
                for (int i = 0; i < locs.Length; i++)
                {
                    if ((locs[i].IsBillTo() && IsSOTrx())
                    || (locs[i].IsPayFrom() && !IsSOTrx()))
                        SetC_BPartner_Location_ID(locs[i].GetC_BPartner_Location_ID());
                }
                //	set to first
                if (GetC_BPartner_Location_ID() == 0 && locs.Length > 0)
                    SetC_BPartner_Location_ID(locs[0].GetC_BPartner_Location_ID());
            }
            if (GetC_BPartner_Location_ID() == 0)
            {
                log.Log(Level.SEVERE, "Has no To Address: " + bp);
            }

            //	Set Contact
            MUser[] contacts = bp.GetContacts(false);
            if (contacts != null && contacts.Length > 0)	//	get first User
                SetAD_User_ID(contacts[0].GetAD_User_ID());
        }

        /// <summary>
        /// 	Set Order References
        /// </summary>
        /// <param name="order"> order</param>
        public void SetOrder(MOrder order)
        {
            if (order == null)
                return;

            SetC_Order_ID(order.GetC_Order_ID());
            SetIsSOTrx(order.IsSOTrx());
            SetIsDiscountPrinted(order.IsDiscountPrinted());
            SetIsSelfService(order.IsSelfService());
            SetSendEMail(order.IsSendEMail());
            //
            SetM_PriceList_ID(order.GetM_PriceList_ID());
            SetIsTaxIncluded(order.IsTaxIncluded());
            SetC_Currency_ID(order.GetC_Currency_ID());
            SetC_ConversionType_ID(order.GetC_ConversionType_ID());
            //

            SetPaymentRule(order.GetPaymentRule());
            SetC_PaymentTerm_ID(order.GetC_PaymentTerm_ID());
            SetPOReference(order.GetPOReference());
            SetDescription(order.GetDescription());
            SetDateOrdered(order.GetDateOrdered());
            //
            SetAD_OrgTrx_ID(order.GetAD_OrgTrx_ID());
            SetC_Project_ID(order.GetC_Project_ID());
            SetC_Campaign_ID(order.GetC_Campaign_ID());
            SetC_Activity_ID(order.GetC_Activity_ID());
            SetUser1_ID(order.GetUser1_ID());
            SetUser2_ID(order.GetUser2_ID());
        }

        /// <summary>
        /// Set Shipment References
        /// </summary>
        /// <param name="ship">shipment</param>
        public void SetShipment(MInOut ship)
        {
            if (ship == null)
                return;

            SetIsSOTrx(ship.IsSOTrx());
            //vikas 9/16/14 Set cb partner 
            MOrder ord = new MOrder(GetCtx(), ship.GetC_Order_ID(), null);
            MBPartner bp = new MBPartner(GetCtx(), ord.GetBill_BPartner_ID(), null);
            //vikas
            //MBPartner bp = new MBPartner(GetCtx(), ship.GetC_BPartner_ID(), null);
            SetBPartner(bp);
            SetAD_User_ID(ord.GetBill_User_ID());
            //
            SetSendEMail(ship.IsSendEMail());
            //
            SetPOReference(ship.GetPOReference());
            SetDescription(ship.GetDescription());
            SetDateOrdered(ship.GetDateOrdered());
            //
            SetAD_OrgTrx_ID(ship.GetAD_OrgTrx_ID());

            SetC_Project_ID(ship.GetC_Project_ID());
            SetC_Campaign_ID(ship.GetC_Campaign_ID());
            SetC_Activity_ID(ship.GetC_Activity_ID());
            SetUser1_ID(ship.GetUser1_ID());
            SetUser2_ID(ship.GetUser2_ID());
            //
            if (ship.GetC_Order_ID() != 0)
            {
                SetC_Order_ID(ship.GetC_Order_ID());
                MOrder order = new MOrder(GetCtx(), ship.GetC_Order_ID(), Get_TrxName());
                SetIsDiscountPrinted(order.IsDiscountPrinted());
                SetDateOrdered(order.GetDateOrdered());
                SetM_PriceList_ID(order.GetM_PriceList_ID());
                SetIsTaxIncluded(order.IsTaxIncluded());
                SetC_Currency_ID(order.GetC_Currency_ID());
                SetC_ConversionType_ID(order.GetC_ConversionType_ID());
                SetPaymentRule(order.GetPaymentRule());
                SetC_PaymentTerm_ID(order.GetC_PaymentTerm_ID());
                //
                MDocType dt = MDocType.Get(GetCtx(), order.GetC_DocType_ID());
                if (dt.GetC_DocTypeInvoice_ID() != 0)
                    SetC_DocTypeTarget_ID(dt.GetC_DocTypeInvoice_ID(), true);
                //	Overwrite Invoice Address
                SetC_BPartner_Location_ID(order.GetBill_Location_ID());
            }
        }

        /// <summary>
        /// Set Target Document Type
        /// </summary>
        /// <param name="DocBaseType">doc type MDocBaseType.DOCBASETYPE_</param>
        public void SetC_DocTypeTarget_ID(String DocBaseType)
        {
            //String sql = "SELECT C_DocType_ID FROM C_DocType "
            //    + "WHERE AD_Client_ID=@param1 AND DocBaseType=@param2"
            //    + " AND IsActive='Y' "
            //    + "ORDER BY IsDefault DESC";
            String sql = "SELECT C_DocType_ID FROM C_DocType "
               + "WHERE AD_Client_ID=@param1 AND DocBaseType=@param2"
               + " AND IsActive='Y' AND IsExpenseInvoice = 'N' "
               + "ORDER BY C_DocType_ID DESC ,   IsDefault DESC";
            int C_DocType_ID = DataBase.DB.GetSQLValue(null, sql, GetAD_Client_ID(), DocBaseType);
            if (C_DocType_ID <= 0)
            {
                log.Log(Level.SEVERE, "Not found for AC_Client_ID="
                    + GetAD_Client_ID() + " - " + DocBaseType);
            }
            else
            {
                log.Fine(DocBaseType);
                SetC_DocTypeTarget_ID(C_DocType_ID);
                bool isSOTrx = MDocBaseType.DOCBASETYPE_ARINVOICE.Equals(DocBaseType)
                    || MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(DocBaseType);
                SetIsSOTrx(isSOTrx);
                bool isReturnTrx = MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(DocBaseType)
                    || MDocBaseType.DOCBASETYPE_APCREDITMEMO.Equals(DocBaseType);
                SetIsReturnTrx(isReturnTrx);
            }
        }

        /// <summary>
        /// Set Target Document Type.
        //Based on SO flag AP/AP Invoice
        /// </summary>
        public void SetC_DocTypeTarget_ID()
        {
            if (GetC_DocTypeTarget_ID() > 0)
                return;
            if (IsSOTrx())
                SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_ARINVOICE);
            else
                SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_APINVOICE);
        }

        /// <summary>
        /// Set Target Document Type
        /// </summary>
        /// <param name="C_DocTypeTarget_ID"></param>
        /// <param name="setReturnTrx">if true set ReturnTrx and SOTrx</param>
        public void SetC_DocTypeTarget_ID(int C_DocTypeTarget_ID, bool setReturnTrx)
        {
            base.SetC_DocTypeTarget_ID(C_DocTypeTarget_ID);
            if (setReturnTrx)
            {
                MDocType dt = MDocType.Get(GetCtx(), C_DocTypeTarget_ID);
                SetIsSOTrx(dt.IsSOTrx());
                SetIsReturnTrx(dt.IsReturnTrx());
            }
        }

        /// <summary>
        /// Get Grand Total
        /// </summary>
        /// <param name="creditMemoAdjusted">adjusted for CM (negative)</param>
        /// <returns>grand total</returns>
        public Decimal GetGrandTotal(bool creditMemoAdjusted)
        {
            if (!creditMemoAdjusted)
                return base.GetGrandTotal();
            //
            Decimal amt = GetGrandTotal();
            if (IsCreditMemo())
            {
                //return amt * -1;// amt.negate();
                return Decimal.Negate(amt);
            }
            return amt;
        }

        /// <summary>
        /// Get Invoice Lines of Invoice
        /// </summary>
        /// <param name="whereClause">starting with AND</param>
        /// <returns>lines</returns>
        private MInvoiceLine[] GetLines(String whereClause)
        {
            List<MInvoiceLine> list = new List<MInvoiceLine>();
            String sql = "SELECT * FROM C_InvoiceLine WHERE C_Invoice_ID= " + GetC_Invoice_ID();
            if (whereClause != null)
                sql += whereClause;
            sql += " ORDER BY Line";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MInvoiceLine il = new MInvoiceLine(GetCtx(), dr, Get_TrxName());
                    il.SetInvoice(this);
                    list.Add(il);
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getLines", e);
            }

            MInvoiceLine[] lines = new MInvoiceLine[list.Count];
            lines = list.ToArray();
            return lines;
        }

        /// <summary>
        /// Get Invoice Lines
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns>lines</returns>
        public MInvoiceLine[] GetLines(bool requery)
        {
            if (_lines == null || _lines.Length == 0 || requery)
                _lines = GetLines(null);
            return _lines;
        }

        /// <summary>
        /// Get Lines of Invoice
        /// </summary>
        /// <returns>lines</returns>
        public MInvoiceLine[] GetLines()
        {
            return GetLines(false);
        }

        /// <summary>
        /// Renumber Lines
        /// </summary>
        /// <param name="step">start and step</param>
        public void RenumberLines(int step)
        {
            int number = step;
            MInvoiceLine[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInvoiceLine line = lines[i];
                line.SetLine(number);
                line.Save();
                number += step;
            }
            _lines = null;
        }

        /// <summary>
        /// Copy Lines From other Invoice.
        /// </summary>
        /// <param name="otherInvoice">invoice</param>
        /// <param name="counter">create counter links</param>
        /// <param name="setOrder">set order links</param>
        /// <returns>number of lines copied</returns>
        public int CopyLinesFrom(MInvoice otherInvoice, bool counter, bool setOrder)
        {
            if (IsProcessed() || IsPosted() || otherInvoice == null)
            {
                return 0;
            }
            MInvoiceLine[] fromLines = otherInvoice.GetLines(false);
            int count = 0;
            for (int i = 0; i < fromLines.Length; i++)
            {
                MInvoiceLine line = new MInvoiceLine(GetCtx(), 0, Get_TrxName());
                MInvoiceLine fromLine = fromLines[i];
                if (counter)	//	header
                    PO.CopyValues(fromLine, line, GetAD_Client_ID(), GetAD_Org_ID());
                else
                    PO.CopyValues(fromLine, line, fromLine.GetAD_Client_ID(), fromLine.GetAD_Org_ID());
                line.SetC_Invoice_ID(GetC_Invoice_ID());
                line.SetInvoice(this);
                line.Set_ValueNoCheck("C_InvoiceLine_ID", I_ZERO);	// new
                //	Reset
                if (!setOrder)
                    line.SetC_OrderLine_ID(0);
                line.SetRef_InvoiceLine_ID(0);
                line.SetM_InOutLine_ID(0);
                line.SetA_Asset_ID(0);
                line.SetM_AttributeSetInstance_ID(0);
                line.SetS_ResourceAssignment_ID(0);
                //	New Tax
                if (GetC_BPartner_ID() != otherInvoice.GetC_BPartner_ID())
                    line.SetTax();	//	recalculate
                //
                if (counter)
                {
                    line.SetRef_InvoiceLine_ID(fromLine.GetC_InvoiceLine_ID());
                    if (fromLine.GetC_OrderLine_ID() != 0)
                    {
                        MOrderLine peer = new MOrderLine(GetCtx(), fromLine.GetC_OrderLine_ID(), Get_TrxName());
                        if (peer.GetRef_OrderLine_ID() != 0)
                            line.SetC_OrderLine_ID(peer.GetRef_OrderLine_ID());
                    }
                    line.SetM_InOutLine_ID(0);
                    if (fromLine.GetM_InOutLine_ID() != 0)
                    {
                        MInOutLine peer = new MInOutLine(GetCtx(), fromLine.GetM_InOutLine_ID(), Get_TrxName());
                        if (peer.GetRef_InOutLine_ID() != 0)
                            line.SetM_InOutLine_ID(peer.GetRef_InOutLine_ID());
                    }
                }
                //
                line.SetProcessed(false);
                if (line.Save(Get_TrxName()))
                    count++;
                //	Cross Link
                if (counter)
                {
                    fromLine.SetRef_InvoiceLine_ID(line.GetC_InvoiceLine_ID());
                    fromLine.Save(Get_TrxName());
                }
            }
            if (fromLines.Length != count)
            {
                log.Log(Level.SEVERE, "Line difference - From=" + fromLines.Length + " <> Saved=" + count);
            }
            return count;
        }

        /**
         * 	Set Reversal
         *	@param reversal reversal
         */
        private void SetReversal(bool reversal)
        {
            _reversal = reversal;
        }

        /**
         * 	Is Reversal
         *	@return reversal
         */
        private bool IsReversal()
        {
            return _reversal;
        }

        /**
         * 	Get Taxes
         *	@param requery requery
         *	@return array of taxes
         */
        public MInvoiceTax[] GetTaxes(bool requery)
        {
            if (_taxes != null && !requery)
                return _taxes;
            String sql = "SELECT * FROM C_InvoiceTax WHERE C_Invoice_ID=" + GetC_Invoice_ID();
            List<MInvoiceTax> list = new List<MInvoiceTax>();
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MInvoiceTax(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "getTaxes", e);
            }
            finally
            {
                ds = null;
            }

            _taxes = new MInvoiceTax[list.Count];
            _taxes = list.ToArray();
            return _taxes;
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /**
         * 	Is it a Credit Memo?
         *	@return true if CM
         */
        public bool IsCreditMemo()
        {
            MDocType dt = MDocType.Get(GetCtx(),
                GetC_DocType_ID() == 0 ? GetC_DocTypeTarget_ID() : GetC_DocType_ID());
            return MDocBaseType.DOCBASETYPE_APCREDITMEMO.Equals(dt.GetDocBaseType())
                || MDocBaseType.DOCBASETYPE_ARCREDITMEMO.Equals(dt.GetDocBaseType());
        }

        /**
         * 	Set Processed.
         * 	Propergate to Lines/Taxes
         *	@param processed processed
         */
        public new void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String set = "SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE C_Invoice_ID=" + GetC_Invoice_ID();
            int noLine = DataBase.DB.ExecuteQuery("UPDATE C_InvoiceLine " + set, null, Get_Trx());
            int noTax = DataBase.DB.ExecuteQuery("UPDATE C_InvoiceTax " + set, null, Get_Trx());
            _lines = null;
            _taxes = null;
            log.Fine(processed + " - Lines=" + noLine + ", Tax=" + noTax);
        }

        /**
         * 	Validate Invoice Pay Schedule
         *	@return pay schedule is valid
         */
        public bool ValidatePaySchedule()
        {
            MInvoicePaySchedule[] schedule = MInvoicePaySchedule.GetInvoicePaySchedule
                (GetCtx(), GetC_Invoice_ID(), 0, Get_Trx());
            log.Fine("#" + schedule.Length);
            if (schedule.Length == 0)
            {
                SetIsPayScheduleValid(false);
                return false;
            }
            //	Add up due amounts
            Decimal total = Env.ZERO;
            for (int i = 0; i < schedule.Length; i++)
            {
                schedule[i].SetParent(this);
                Decimal due = schedule[i].GetDueAmt();
                //if (due != null)
                    total = Decimal.Add(total, due);
            }
            bool valid = GetGrandTotal().CompareTo(total) == 0;
            SetIsPayScheduleValid(valid);

            //	Update Schedule Lines
            for (int i = 0; i < schedule.Length; i++)
            {
                if (schedule[i].IsValid() != valid)
                {
                    schedule[i].SetIsValid(valid);
                    schedule[i].Save(Get_Trx());
                }
            }
            return valid;
        }

        /***
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	No Partner Info - set Template
            if (GetC_BPartner_ID() == 0)
                SetBPartner(MBPartner.GetTemplate(GetCtx(), GetAD_Client_ID()));
            if (GetC_BPartner_Location_ID() == 0)
                SetBPartner(new MBPartner(GetCtx(), GetC_BPartner_ID(), null));

            //	Price List
            if (GetM_PriceList_ID() == 0)
            {
                int ii = GetCtx().GetContextAsInt("#M_PriceList_ID");
                if (ii != 0)
                    SetM_PriceList_ID(ii);
                else
                {
                    String sql = "SELECT M_PriceList_ID FROM M_PriceList WHERE AD_Client_ID=@param1 AND IsDefault='Y'";
                    ii = DataBase.DB.GetSQLValue(null, sql, GetAD_Client_ID());
                    if (ii != 0)
                        SetM_PriceList_ID(ii);
                }
            }

            //	Currency
            if (GetC_Currency_ID() == 0)
            {
                String sql = "SELECT C_Currency_ID FROM M_PriceList WHERE M_PriceList_ID=@param1";
                int ii = DataBase.DB.GetSQLValue(null, sql, GetM_PriceList_ID());
                if (ii != 0)
                    SetC_Currency_ID(ii);
                else
                    SetC_Currency_ID(GetCtx().GetContextAsInt("#C_Currency_ID"));
            }

            //	Sales Rep
            if (GetSalesRep_ID() == 0)
            {
                int ii = GetCtx().GetContextAsInt("#SalesRep_ID");
                if (ii != 0)
                    SetSalesRep_ID(ii);
            }

            //	Document Type
            if (GetC_DocType_ID() == 0)
                SetC_DocType_ID(0);	//	make sure it's set to 0
            if (GetC_DocTypeTarget_ID() == 0)
                SetC_DocTypeTarget_ID(IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARINVOICE : MDocBaseType.DOCBASETYPE_APINVOICE);

            //	Payment Term
            if (GetC_PaymentTerm_ID() == 0)
            {
                int ii = GetCtx().GetContextAsInt("#C_PaymentTerm_ID");
                if (ii != 0)
                    SetC_PaymentTerm_ID(ii);
                else
                {
                    String sql = "SELECT C_PaymentTerm_ID FROM C_PaymentTerm WHERE AD_Client_ID=@param1 AND IsDefault='Y'";
                    ii = DataBase.DB.GetSQLValue(null, sql, GetAD_Client_ID());
                    if (ii != 0)
                        SetC_PaymentTerm_ID(ii);
                }
            }
            //	BPartner Active
            if (newRecord || Is_ValueChanged("C_BPartner_ID"))
            {
                MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                if (!bp.IsActive())
                {
                    log.SaveWarning("NotActive", Msg.GetMsg(GetCtx(), "C_BPartner_ID"));
                    return false;
                }
            }
            return true;
        }

        /**
         * 	Before Delete
         *	@return true if it can be deleted
         */
        protected override bool BeforeDelete()
        {
            if (GetC_Order_ID() != 0)
            {
                log.SaveError("Error", Msg.GetMsg(GetCtx(), "CannotDelete"));
                return false;
            }
            return true;
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInvoice[")
                .Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",GrandTotal=").Append(GetGrandTotal());
            if (_lines != null)
                sb.Append(" (#").Append(_lines.Length).Append(")");
            sb.Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info (untranslated)
         */
        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success || newRecord)
                return success;

            if (Is_ValueChanged("AD_Org_ID"))
            {
                String sql = "UPDATE C_InvoiceLine ol"
                    + " SET AD_Org_ID ="
                        + "(SELECT AD_Org_ID"
                        + " FROM C_Invoice o WHERE ol.C_Invoice_ID=o.C_Invoice_ID) "
                    + "WHERE C_Invoice_ID=" + GetC_Invoice_ID();
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                log.Fine("Lines -> #" + no);
            }
            return true;
        }

        /**
         * 	Set Price List (and Currency) when valid
         * 	@param M_PriceList_ID price list
         */
        public new void SetM_PriceList_ID(int M_PriceList_ID)
        {
            String sql = "SELECT M_PriceList_ID, C_Currency_ID "
                + "FROM M_PriceList WHERE M_PriceList_ID=" + M_PriceList_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    base.SetM_PriceList_ID(Convert.ToInt32(dr[0]));
                    SetC_Currency_ID(Convert.ToInt32(dr[1]));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "setM_PriceList_ID", e);
            }
            finally
            {

                dt = null;
            }
        }

        /**
         * 	Get Allocated Amt in Invoice Currency
         *	@return pos/neg amount or null
         */
        public Decimal? GetAllocatedAmt()
        {
            Decimal? retValue = null;
            String sql = "SELECT SUM(currencyConvert(al.Amount+al.DiscountAmt+al.WriteOffAmt,"
                    + "ah.C_Currency_ID, i.C_Currency_ID,ah.DateTrx,COALESCE(i.C_ConversionType_ID,0), al.AD_Client_ID,al.AD_Org_ID)) " //jz 
                + "FROM C_AllocationLine al"
                + " INNER JOIN C_AllocationHdr ah ON (al.C_AllocationHdr_ID=ah.C_AllocationHdr_ID)"
                + " INNER JOIN C_Invoice i ON (al.C_Invoice_ID=i.C_Invoice_ID) "
                + "WHERE al.C_Invoice_ID=" + GetC_Invoice_ID()
                + " AND ah.IsActive='Y' AND al.IsActive='Y'";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = Convert.ToDecimal(dr[0]);
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
            //	log.Fine("getAllocatedAmt - " + retValue);
            //	? ROUND(NVL(v_AllocatedAmt,0), 2);
            return retValue;
        }

        /**
         * 	Test Allocation (and set paid flag)
         *	@return true if updated
         */
        public bool TestAllocation()
        {
            Decimal? alloc = GetAllocatedAmt();	//	absolute
            if (alloc == null)
                alloc = Env.ZERO;
            Decimal total = GetGrandTotal();
            if (!IsSOTrx())
                total = Decimal.Negate(total);
            if (IsCreditMemo())
                total = Decimal.Negate(total);
            bool test = total.CompareTo(alloc) == 0;
            bool change = test != IsPaid();
            if (change)
                SetIsPaid(test);
            log.Fine("Paid=" + test + " (" + alloc + "=" + total + ")");
            return change;
        }

        /**
         * 	Set Paid Flag for invoices
         * 	@param ctx context
         *	@param C_BPartner_ID if 0 all
         *	@param trxName transaction
         */
        public static void SetIsPaid(Ctx ctx, int C_BPartner_ID, Trx trxName)
        {
            int counter = 0;
            String sql = "SELECT * FROM C_Invoice "
                + "WHERE IsPaid='N' AND DocStatus IN ('CO','CL')";
            if (C_BPartner_ID > 1)
                sql += " AND C_BPartner_ID=" + C_BPartner_ID;
            else
                sql += " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    MInvoice invoice = new MInvoice(ctx, dr, trxName);
                    if (invoice.TestAllocation())
                        if (invoice.Save())
                            counter++;
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
            _log.Config("#" + counter);
        }

        /**
         * 	Get Open Amount.
         * 	Used by web interface
         * 	@return Open Amt
         */
        public Decimal? GetOpenAmt()
        {
            return GetOpenAmt(true, null);
        }

        /**
         * 	Get Open Amount
         * 	@param creditMemoAdjusted adjusted for CM (negative)
         * 	@param paymentDate ignored Payment Date
         * 	@return Open Amt
         */
        public Decimal? GetOpenAmt(bool creditMemoAdjusted, DateTime? paymentDate)
        {
            if (IsPaid())
                return Env.ZERO;
            //
            if (_openAmt == null)
            {
                _openAmt = GetGrandTotal();
                if (paymentDate != null)
                {
                    //	Payment Discount
                    //	Payment Schedule
                }
                Decimal? allocated = GetAllocatedAmt();
                if (allocated != null)
                {
                    allocated = Math.Abs((Decimal)allocated);//.abs();	//	is absolute
                    _openAmt = Decimal.Subtract((Decimal)_openAmt, (Decimal)allocated);
                }
            }

            if (!creditMemoAdjusted)
                return _openAmt;
            if (IsCreditMemo())
                return Decimal.Negate((Decimal)_openAmt);
            return _openAmt;
        }

        /**
         * 	Get Document Status
         *	@return Document Status Clear Text
         */
        public String GetDocStatusName()
        {
            return MRefList.GetListName(GetCtx(), 131, GetDocStatus());
        }

        /**
         * 	Create PDF
         *	@return File or null
         */
        //public File CreatePDF ()
        //public FileInfo CreatePDF()
        //{
        //    try
        //    {
        //        File temp = File.createTempFile(get_TableName() + get_ID() + "_", ".pdf");
        //        return createPDF(temp);
        //    }
        //    catch (Exception e)
        //    {
        //        log.severe("Could not create PDF - " + e.getMessage());
        //    }
        //    return null;
        //}	

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        //public FileInfo CreatePDF (File file)
        //{
        //    ReportEngine re = ReportEngine.get (GetCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
        //    if (re == null)
        //        return null;
        //    return re.getPDF(file);
        //}	

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                //string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                String fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo() + ".pdf";
                string filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload", fileName);


                ReportEngine_N re = ReportEngine_N.Get(GetCtx(), ReportEngine_N.INVOICE, GetC_Invoice_ID());
                if (re == null)
                    return null;

                re.GetView();
                bool b = re.CreatePDF(filePath);

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    re.CreatePDF(filePath);
                    return new FileInfo(filePath);
                }
                else
                    return temp;
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
            //ReportEngine re = ReportEngine.get(GetCtx(), ReportEngine.ORDER, GetC_Order_ID());
            //if (re == null)
            //    return null;
            //return re.getPDF(file);

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
         * 	Get PDF File Name
         *	@param documentDir directory
         *	@return file name
         */
        public String GetPDFFileName(String documentDir)
        {
            return GetPDFFileName(documentDir, GetC_Invoice_ID());
        }

        /**
         *	Get ISO Code of Currency
         *	@return Currency ISO
         */
        public String GetCurrencyISO()
        {
            return MCurrency.GetISO_Code(GetCtx(), GetC_Currency_ID());
        }

        /**
         * 	Get Currency Precision
         *	@return precision
         */
        public int GetPrecision()
        {
            return MCurrency.GetStdPrecision(GetCtx(), GetC_Currency_ID());
        }

        /***
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
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocTypeTarget_ID());
            SetIsReturnTrx(dt.IsReturnTrx());
            SetIsSOTrx(dt.IsSOTrx());

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType()))
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

            //	Lines
            MInvoiceLine[] lines = GetLines(true);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            //	No Cash Book
            if (PAYMENTRULE_Cash.Equals(GetPaymentRule())
                && MCashBook.Get(GetCtx(), GetAD_Org_ID(), GetC_Currency_ID()) == null)
            {
                _processMsg = "@NoCashBook@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	Convert/Check DocType
            if (GetC_DocType_ID() != GetC_DocTypeTarget_ID())
                SetC_DocType_ID(GetC_DocTypeTarget_ID());
            if (GetC_DocType_ID() == 0)
            {
                _processMsg = "No Document Type";
                return DocActionVariables.STATUS_INVALID;
            }

            ExplodeBOM();
            if (!CalculateTaxTotal())	//	setTotals
            {
                _processMsg = "Error calculating Tax";
                return DocActionVariables.STATUS_INVALID;
            }

            CreatePaySchedule();

            //	Credit Status
            if (IsSOTrx() && !IsReversal())
            {
                MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
                if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
                {
                    _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
                        + bp.GetTotalOpenBalance()
                        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            //	Landed Costs
            if (!IsSOTrx())
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    MInvoiceLine line = lines[i];
                    String error = line.AllocateLandedCosts();
                    if (error != null && error.Length > 0)
                    {
                        _processMsg = error;
                        return DocActionVariables.STATUS_INVALID;
                    }
                }
            }

            //	Add up Amounts
            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Explode non stocked BOM.
         */
        private void ExplodeBOM()
        {
            String where = "AND IsActive='Y' AND EXISTS "
                + "(SELECT * FROM M_Product p WHERE C_InvoiceLine.M_Product_ID=p.M_Product_ID"
                + " AND	p.IsBOM='Y' AND p.IsVerified='Y' AND p.IsStocked='N')";
            //
            String sql = "SELECT COUNT(*) FROM C_InvoiceLine "
                + "WHERE C_Invoice_ID=" + GetC_Invoice_ID() + " " + where;
            int count = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
            while (count != 0)
            {
                RenumberLines(100);

                //	Order Lines with non-stocked BOMs
                MInvoiceLine[] lines = GetLines(where);
                for (int i = 0; i < lines.Length; i++)
                {
                    MInvoiceLine line = lines[i];
                    MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());
                    log.Fine(product.GetName());
                    //	New Lines
                    int lineNo = line.GetLine();
                    MProductBOM[] boms = MProductBOM.GetBOMLines(product);
                    for (int j = 0; j < boms.Length; j++)
                    {
                        MProductBOM bom = boms[j];
                        MInvoiceLine newLine = new MInvoiceLine(this);
                        newLine.SetLine(++lineNo);
                        newLine.SetM_Product_ID(bom.GetProduct().GetM_Product_ID(),
                            bom.GetProduct().GetC_UOM_ID());
                        newLine.SetQty(Decimal.Multiply(line.GetQtyInvoiced(), bom.GetBOMQty()));		//	Invoiced/Entered
                        if (bom.GetDescription() != null)
                            newLine.SetDescription(bom.GetDescription());
                        //
                        newLine.SetPrice();
                        newLine.Save(Get_TrxName());
                    }
                    //	Convert into Comment Line
                    line.SetM_Product_ID(0);
                    line.SetM_AttributeSetInstance_ID(0);
                    line.SetPriceEntered(Env.ZERO);
                    line.SetPriceActual(Env.ZERO);
                    line.SetPriceLimit(Env.ZERO);
                    line.SetPriceList(Env.ZERO);
                    line.SetLineNetAmt(Env.ZERO);
                    //
                    String description = product.GetName();
                    if (product.GetDescription() != null)
                        description += " " + product.GetDescription();
                    if (line.GetDescription() != null)
                        description += " " + line.GetDescription();
                    line.SetDescription(description);
                    line.Save(Get_TrxName());
                } //	for all lines with BOM

                _lines = null;
                count = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetC_Invoice_ID());
                RenumberLines(10);
            }	//	while count != 0
        }

        /**
         * 	Calculate Tax and Total
         * 	@return true if calculated
         */
        private bool CalculateTaxTotal()
        {
            log.Fine("");
            //	Delete Taxes
            DataBase.DB.ExecuteQuery("DELETE FROM C_InvoiceTax WHERE C_Invoice_ID=" + GetC_Invoice_ID(), null, Get_TrxName());
            _taxes = null;

            //	Lines
            Decimal totalLines = Env.ZERO;
            List<int> taxList = new List<int>();
            MInvoiceLine[] lines = GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                MInvoiceLine line = lines[i];
                /**	Sync ownership for SO
                if (isSOTrx() && line.getAD_Org_ID() != getAD_Org_ID())
                {
                    line.setAD_Org_ID(getAD_Org_ID());
                    line.Save();
                }	**/
                int taxID = (int)line.GetC_Tax_ID();
                if (!taxList.Contains(taxID))
                {
                    MInvoiceTax iTax = MInvoiceTax.Get(line, GetPrecision(),
                        false, Get_TrxName());	//	current Tax
                    if (iTax != null)
                    {
                        iTax.SetIsTaxIncluded(IsTaxIncluded());
                        if (!iTax.CalculateTaxFromLines())
                            return false;
                        if (!iTax.Save())
                            return false;
                        taxList.Add(taxID);
                    }
                }
                totalLines = Decimal.Add(totalLines, line.GetLineNetAmt());
            }

            //	Taxes
            Decimal grandTotal = totalLines;
            MInvoiceTax[] taxes = GetTaxes(true);
            for (int i = 0; i < taxes.Length; i++)
            {
                MInvoiceTax iTax = taxes[i];
                MTax tax = iTax.GetTax();
                if (tax.IsSummary())
                {
                    MTax[] cTaxes = tax.GetChildTaxes(false);	//	Multiple taxes
                    for (int j = 0; j < cTaxes.Length; j++)
                    {
                        MTax cTax = cTaxes[j];
                        Decimal taxAmt = cTax.CalculateTax(iTax.GetTaxBaseAmt(), IsTaxIncluded(), GetPrecision());
                        //
                        MInvoiceTax newITax = new MInvoiceTax(GetCtx(), 0, Get_TrxName());
                        newITax.SetClientOrg(this);
                        newITax.SetC_Invoice_ID(GetC_Invoice_ID());
                        newITax.SetC_Tax_ID(cTax.GetC_Tax_ID());
                        newITax.SetPrecision(GetPrecision());
                        newITax.SetIsTaxIncluded(IsTaxIncluded());
                        newITax.SetTaxBaseAmt(iTax.GetTaxBaseAmt());
                        newITax.SetTaxAmt(taxAmt);
                        if (!newITax.Save(Get_TrxName()))
                            return false;
                        //
                        if (!IsTaxIncluded())
                            grandTotal = Decimal.Add(grandTotal, taxAmt);
                    }
                    if (!iTax.Delete(true, Get_TrxName()))
                        return false;
                }
                else
                {
                    if (!IsTaxIncluded())
                        grandTotal = Decimal.Add(grandTotal, iTax.GetTaxAmt());
                }
            }
            //
            SetTotalLines(totalLines);
            SetGrandTotal(grandTotal);
            return true;
        }

        /**
         * 	(Re) Create Pay Schedule
         *	@return true if valid schedule
         */
        private bool CreatePaySchedule()
        {
            if (GetC_PaymentTerm_ID() == 0)
                return false;
            MPaymentTerm pt = new MPaymentTerm(GetCtx(), GetC_PaymentTerm_ID(), null);
            log.Fine(pt.ToString());
            return pt.Apply(this);		//	calls validate pay schedule
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
                    ApproveIt();
                log.Info(ToString());
                StringBuilder Info = new StringBuilder();

                //	Create Cash
                if (PAYMENTRULE_Cash.Equals(GetPaymentRule()))
                {
                    MCash cash = MCash.Get(GetCtx(), GetAD_Org_ID(),
                        GetDateInvoiced(), GetC_Currency_ID(), Get_TrxName());
                    if (cash == null || cash.Get_ID() == 0)
                    {
                        _processMsg = "@NoCashBook@";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    //Added By Manjot -Changes done for Target Doctype Cash Journal
                    Int32 DocType_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT C_Doctype_ID FROM C_Doctype WHERE docbasetype='CMC' AND Ad_client_id='" + GetCtx().GetAD_Client_ID() + "' AND ad_org_id in('0','" + GetCtx().GetAD_Org_ID() + "') ORDER BY  ad_org_id desc"));
                    cash.SetC_DocType_ID(DocType_ID);
                    // Manjot
                    MCashLine cl = new MCashLine(cash);
                    cl.SetInvoice(this);
                    if (!cl.Save(Get_TrxName()))
                    {
                        _processMsg = "Could not Save Cash Journal Line";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    Info.Append("@C_Cash_ID@: " + cash.GetName() + " #" + cl.GetLine());
                    SetC_CashLine_ID(cl.GetC_CashLine_ID());
                }	//	CashBook

                //	Update Order & Match
                int matchInv = 0;
                int matchPO = 0;
                MInvoiceLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MInvoiceLine line = lines[i];

                    //	Update Order Line
                    MOrderLine ol = null;
                    if (line.GetC_OrderLine_ID() != 0)
                    {
                        if (IsSOTrx()
                            || line.GetM_Product_ID() == 0)
                        {
                            ol = new MOrderLine(GetCtx(), line.GetC_OrderLine_ID(), Get_TrxName());
                           // if (line.GetQtyInvoiced() != null)
                            {
                                // special check to Update orderline's Qty Invoiced
                                // if Qtyentered == QtyInvoiced Then do not update 

                                // Commented this check as it was not updating QtyInvoiced at Orderline //Lokesh Chauhan
                                // if (ol.GetQtyInvoiced() < ol.GetQtyEntered())
                                //{
                                //    ol.SetQtyInvoiced(Decimal.Add(ol.GetQtyInvoiced(), line.GetQtyInvoiced()));
                                //}

                                ol.SetQtyInvoiced(Decimal.Add(ol.GetQtyInvoiced(), line.GetQtyInvoiced()));

                            }
                            if (!ol.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not update Order Line";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        //	Order Invoiced Qty updated via Matching Inv-PO
                        else if (!IsSOTrx()
                            && line.GetM_Product_ID() != 0
                            && !IsReversal())
                        {
                            //	MatchPO is created also from MInOut when Invoice exists before Shipment
                            Decimal matchQty = line.GetQtyInvoiced();
                            MMatchPO po = MMatchPO.Create(line, null,
                                GetDateInvoiced(), matchQty);
                            if (!po.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not create PO Matching";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            else
                                matchPO++;
                        }
                    }

                    //	Matching - Inv-Shipment
                    if (!IsSOTrx()
                        && line.GetM_InOutLine_ID() != 0
                        && line.GetM_Product_ID() != 0
                        && !IsReversal())
                    {
                        MInOutLine receiptLine = new MInOutLine(GetCtx(), line.GetM_InOutLine_ID(), Get_TrxName());
                        Decimal matchQty = line.GetQtyInvoiced();

                        if (receiptLine.GetMovementQty().CompareTo(matchQty) < 0)
                            matchQty = receiptLine.GetMovementQty();

                        MMatchInv inv = new MMatchInv(line, GetDateInvoiced(), matchQty);
                        if (!inv.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not create Invoice Matching";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        else
                            matchInv++;
                    }

                    //	Lead/Request
                    line.CreateLeadRequest(this);
                }	//	for all lines
                if (matchInv > 0)
                    Info.Append(" @M_MatchInv_ID@#").Append(matchInv).Append(" ");
                if (matchPO > 0)
                    Info.Append(" @M_MatchPO_ID@#").Append(matchPO).Append(" ");



                //	Update BP Statistics
                MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
                //	Update total revenue and balance / credit limit (reversed on AllocationLine.processIt)
                Decimal invAmt = MConversionRate.ConvertBase(GetCtx(), GetGrandTotal(true),	//	CM adjusted 
                    GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                //if (invAmt == null)
                //{
                //    _processMsg = "Could not convert C_Currency_ID=" + GetC_Currency_ID()
                //        + " to base C_Currency_ID=" + MClient.Get(GetCtx()).GetC_Currency_ID();
                //    return DocActionVariables.STATUS_INVALID;
                //}
                //	Total Balance
                Decimal newBalance = bp.GetTotalOpenBalance(false);
                //if (newBalance == null)
                //    newBalance = Env.ZERO;
                if (IsSOTrx())
                {
                    newBalance = Decimal.Add(newBalance, invAmt);
                    //
                    if (bp.GetFirstSale() == null)
                    {
                        bp.SetFirstSale(GetDateInvoiced());
                    }
                    Decimal newLifeAmt = bp.GetActualLifeTimeValue();
                    //if (newLifeAmt == null)
                    //{
                    //    newLifeAmt = invAmt;
                    //}
                    //else
                    //{
                        newLifeAmt = Decimal.Add(newLifeAmt, invAmt);
                   // }
                    Decimal newCreditAmt = bp.GetSO_CreditUsed();
                    //if (newCreditAmt == null)
                    //{
                    //    newCreditAmt = invAmt;
                    //}
                    //else
                    //{
                        newCreditAmt = Decimal.Add(newCreditAmt, invAmt);
                    //}
                    log.Fine("GrandTotal=" + GetGrandTotal(true) + "(" + invAmt
                        + ") BP Life=" + bp.GetActualLifeTimeValue() + "->" + newLifeAmt
                        + ", Credit=" + bp.GetSO_CreditUsed() + "->" + newCreditAmt
                        + ", Balance=" + bp.GetTotalOpenBalance(false) + " -> " + newBalance);
                    bp.SetActualLifeTimeValue(newLifeAmt);
                    bp.SetSO_CreditUsed(newCreditAmt);
                }	//	SO
                else
                {
                    newBalance = Decimal.Subtract(newBalance, invAmt);
                    log.Fine("GrandTotal=" + GetGrandTotal(true) + "(" + invAmt
                        + ") Balance=" + bp.GetTotalOpenBalance(false) + " -> " + newBalance);
                }
                bp.SetTotalOpenBalance(newBalance);
                bp.SetSOCreditStatus();
                if (!bp.Save(Get_TrxName()))
                {
                    _processMsg = "Could not update Business Partner";
                    return DocActionVariables.STATUS_INVALID;
                }

                //	User - Last Result/Contact
                if (GetAD_User_ID() != 0)
                {
                    MUser user = new MUser(GetCtx(), GetAD_User_ID(), Get_TrxName());
                    //user.SetLastContact(new DateTime(System.currentTimeMillis()));
                    user.SetLastContact(DateTime.Now);
                    user.SetLastResult(Msg.Translate(GetCtx(), "C_Invoice_ID") + ": " + GetDocumentNo());
                    if (!user.Save(Get_TrxName()))
                    {
                        _processMsg = "Could not update Business Partner User";
                        return DocActionVariables.STATUS_INVALID;
                    }
                }	//	user

                //	Update Project
                if (IsSOTrx() && GetC_Project_ID() != 0)
                {
                    MProject project = new MProject(GetCtx(), GetC_Project_ID(), Get_TrxName());
                    Decimal amt = GetGrandTotal(true);
                    int C_CurrencyTo_ID = project.GetC_Currency_ID();
                    if (C_CurrencyTo_ID != GetC_Currency_ID())
                        amt = MConversionRate.Convert(GetCtx(), amt, GetC_Currency_ID(), C_CurrencyTo_ID,
                            GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                    //if (amt == null)
                    //{
                    //    _processMsg = "Could not convert C_Currency_ID=" + GetC_Currency_ID()
                    //        + " to Project C_Currency_ID=" + C_CurrencyTo_ID;
                    //    return DocActionVariables.STATUS_INVALID;
                    //}
                    Decimal newAmt = project.GetInvoicedAmt();
                    //if (newAmt == null)
                    //    newAmt = amt;
                    //else
                        newAmt = Decimal.Add(newAmt, amt);
                    log.Fine("GrandTotal=" + GetGrandTotal(true) + "(" + amt + ") Project " + project.GetName()
                        + " - Invoiced=" + project.GetInvoicedAmt() + "->" + newAmt);
                    project.SetInvoicedAmt(newAmt);
                    if (!project.Save(Get_TrxName()))
                    {
                        _processMsg = "Could not update Project";
                        return DocActionVariables.STATUS_INVALID;
                    }
                }	//	project

                //	User Validation
                String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
                if (valid != null)
                {
                    _processMsg = valid;
                    return DocActionVariables.STATUS_INVALID;
                }

                //	Counter Documents
                MInvoice counter = CreateCounterDoc();
                if (counter != null)
                    Info.Append(" - @CounterDoc@: @C_Invoice_ID@=").Append(counter.GetDocumentNo());

                _processMsg = Info.ToString().Trim();
                SetProcessed(true);
                SetDocAction(DOCACTION_Close);
            }
            catch 
            {
                // MessageBox.Show("MInvoice--CompleteIt");

            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        /**
         * 	Create Counter Document
         * 	@return counter invoice
         */
        private MInvoice CreateCounterDoc()
        {
            //	Is this a counter doc ?
            if (GetRef_Invoice_ID() != 0)
                return null;

            //	Org Must be linked to BPartner
            MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
            int counterC_BPartner_ID = org.GetLinkedC_BPartner_ID(Get_TrxName()); //jz
            if (counterC_BPartner_ID == 0)
                return null;
            //	Business Partner needs to be linked to Org
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
            int counterAD_Org_ID = bp.GetAD_OrgBP_ID_Int();
            if (counterAD_Org_ID == 0)
                return null;

            MBPartner counterBP = new MBPartner(GetCtx(), counterC_BPartner_ID, null);
            MOrgInfo counterOrgInfo = MOrgInfo.Get(GetCtx(), counterAD_Org_ID, null);
            log.Info("Counter BP=" + counterBP.GetName());

            //	Document Type
            int C_DocTypeTarget_ID = 0;
            MDocTypeCounter counterDT = MDocTypeCounter.GetCounterDocType(GetCtx(), GetC_DocType_ID());
            if (counterDT != null)
            {
                log.Fine(counterDT.ToString());
                if (!counterDT.IsCreateCounter() || !counterDT.IsValid())
                    return null;
                C_DocTypeTarget_ID = counterDT.GetCounter_C_DocType_ID();
            }
            else	//	indirect
            {
                C_DocTypeTarget_ID = MDocTypeCounter.GetCounterDocType_ID(GetCtx(), GetC_DocType_ID());
                log.Fine("Indirect C_DocTypeTarget_ID=" + C_DocTypeTarget_ID);
                if (C_DocTypeTarget_ID <= 0)
                    return null;
            }

            //	Deep Copy
            MInvoice counter = CopyFrom(this, GetDateInvoiced(),
                C_DocTypeTarget_ID, true, Get_TrxName(), true);
            //
            counter.SetAD_Org_ID(counterAD_Org_ID);
            //	counter.setM_Warehouse_ID(counterOrgInfo.getM_Warehouse_ID());
            //
            counter.SetBPartner(counterBP);
            //	Refernces (Should not be required
            counter.SetSalesRep_ID(GetSalesRep_ID());
            counter.Save(Get_TrxName());

            //	Update copied lines
            MInvoiceLine[] counterLines = counter.GetLines(true);
            for (int i = 0; i < counterLines.Length; i++)
            {
                MInvoiceLine counterLine = counterLines[i];
                counterLine.SetClientOrg(counter);
                counterLine.SetInvoice(counter);	//	copies header values (BP, etc.)
                counterLine.SetPrice();
                counterLine.SetTax();
                //
                counterLine.Save(Get_TrxName());
            }

            log.Fine(counter.ToString());

            //	Document Action
            if (counterDT != null)
            {
                if (counterDT.GetDocAction() != null)
                {
                    counter.SetDocAction(counterDT.GetDocAction());
                    counter.ProcessIt(counterDT.GetDocAction());
                    counter.Save(Get_TrxName());
                }
            }
            return counter;
        }

        /**
         * 	Void Document.
         * 	@return true if success 
         */
        public bool VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
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
                //	Set lines to 0
                MInvoiceLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MInvoiceLine line = lines[i];
                    Decimal old = line.GetQtyInvoiced();
                    if (old.CompareTo(Env.ZERO) != 0)
                    {
                        line.SetQty(Env.ZERO);
                        line.SetTaxAmt(Env.ZERO);
                        line.SetLineNetAmt(Env.ZERO);
                        line.SetLineTotalAmt(Env.ZERO);
                        line.AddDescription(Msg.GetMsg(GetCtx(), "Voided") + " (" + old + ")");
                        //	Unlink Shipment
                        if (line.GetM_InOutLine_ID() != 0)
                        {
                            MInOutLine ioLine = new MInOutLine(GetCtx(), line.GetM_InOutLine_ID(), Get_TrxName());
                            ioLine.SetIsInvoiced(false);
                            ioLine.Save(Get_TrxName());
                            line.SetM_InOutLine_ID(0);
                        }
                        line.Save(Get_TrxName());
                    }
                }
                AddDescription(Msg.GetMsg(GetCtx(), "Voided"));
                SetIsPaid(true);
                SetC_Payment_ID(0);
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Close Document.
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            log.Info(ToString());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Reverse Correction - same date
         * 	@return true if success 
         */
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetDateAcct()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }


            //	Don't touch allocation for cash as that is handled in CashJournal
            bool isCash = PAYMENTRULE_Cash.Equals(GetPaymentRule());

            if (!isCash)
            {
                MAllocationHdr[] allocations = MAllocationHdr.GetOfInvoice(GetCtx(),
                    GetC_Invoice_ID(), Get_TrxName());
                for (int i = 0; i < allocations.Length; i++)
                {
                    allocations[i].SetDocAction(DocActionVariables.ACTION_REVERSE_CORRECT);
                    allocations[i].ReverseCorrectIt();
                    allocations[i].Save(Get_TrxName());
                }
            }
            //	Reverse/Delete Matching
            if (!IsSOTrx())
            {
                MMatchInv[] mInv = MMatchInv.GetInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
                for (int i = 0; i < mInv.Length; i++)
                    mInv[i].Delete(true);
                MMatchPO[] mPO = MMatchPO.GetInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
                for (int i = 0; i < mPO.Length; i++)
                {
                    if (mPO[i].GetM_InOutLine_ID() == 0)
                        mPO[i].Delete(true);
                    else
                    {
                        mPO[i].SetC_InvoiceLine_ID(null);
                        mPO[i].Save(Get_TrxName());
                    }
                }
            }
            //
            Load(Get_TrxName());	//	reload allocation reversal Info

            //	Deep Copy
            MInvoice reversal = CopyFrom(this, GetDateInvoiced(),
                GetC_DocType_ID(), false, Get_TrxName(), true);
            if (reversal == null)
            {
                _processMsg = "Could not create Invoice Reversal";
                return false;
            }
            reversal.SetReversal(true);

            //	Reverse Line Qty
            MInvoiceLine[] rLines = reversal.GetLines(false);
            for (int i = 0; i < rLines.Length; i++)
            {
                MInvoiceLine rLine = rLines[i];
                rLine.SetQtyEntered(Decimal.Negate(rLine.GetQtyEntered()));
                rLine.SetQtyInvoiced(Decimal.Negate(rLine.GetQtyInvoiced()));
                rLine.SetLineNetAmt(Decimal.Negate(rLine.GetLineNetAmt()));
                if (((Decimal)rLine.GetTaxAmt()).CompareTo(Env.ZERO) != 0)
                    rLine.SetTaxAmt(Decimal.Negate((Decimal)rLine.GetTaxAmt()));
                if (((Decimal)rLine.GetLineTotalAmt()).CompareTo(Env.ZERO) != 0)
                    rLine.SetLineTotalAmt(Decimal.Negate((Decimal)rLine.GetLineTotalAmt()));
                if (!rLine.Save(Get_TrxName()))
                {
                    _processMsg = "Could not correct Invoice Reversal Line";
                    return false;
                }
            }
            reversal.SetC_Order_ID(GetC_Order_ID());
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            //
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            reversal.SetC_Payment_ID(0);
            reversal.SetIsPaid(true);
            reversal.CloseIt();
            reversal.SetProcessing(false);
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save(Get_TrxName());
            _processMsg = reversal.GetDocumentNo();
            //
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");

            //	Clean up Reversed (this)
            MInvoiceLine[] iLines = GetLines(false);
            for (int i = 0; i < iLines.Length; i++)
            {
                MInvoiceLine iLine = iLines[i];
                if (iLine.GetM_InOutLine_ID() != 0)
                {
                    MInOutLine ioLine = new MInOutLine(GetCtx(), iLine.GetM_InOutLine_ID(), Get_TrxName());
                    ioLine.SetIsInvoiced(false);
                    ioLine.Save(Get_TrxName());
                    //	Reconsiliation
                    iLine.SetM_InOutLine_ID(0);
                    iLine.Save(Get_TrxName());
                }
            }
            SetProcessed(true);
            SetDocStatus(DOCSTATUS_Reversed);	//	may come from void
            SetDocAction(DOCACTION_None);
            SetC_Payment_ID(0);
            SetIsPaid(true);

            //	Create Allocation
            if (!isCash)
            {
                MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false, GetDateAcct(),
                    GetC_Currency_ID(),
                    Msg.Translate(GetCtx(), "C_Invoice_ID") + ": " + GetDocumentNo() + "/" + reversal.GetDocumentNo(),
                    Get_TrxName());
                alloc.SetAD_Org_ID(GetAD_Org_ID());
                if (alloc.Save())
                {
                    //	Amount
                    Decimal gt = GetGrandTotal(true);
                    if (!IsSOTrx())
                        gt = Decimal.Negate(gt);
                    //	Orig Line
                    MAllocationLine aLine = new MAllocationLine(alloc, gt, Env.ZERO, Env.ZERO, Env.ZERO);
                    aLine.SetC_Invoice_ID(GetC_Invoice_ID());
                    aLine.Save();
                    //	Reversal Line
                    MAllocationLine rLine = new MAllocationLine(alloc, Decimal.Negate(gt), Env.ZERO, Env.ZERO, Env.ZERO);
                    rLine.SetC_Invoice_ID(reversal.GetC_Invoice_ID());
                    rLine.Save();
                    //	Process It
                    if (alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE))
                        alloc.Save();
                }
            }	//	notCash

            //	Explicitly Save for balance calc.
            Save();
            //	Update BP Balance
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            bp.SetTotalOpenBalance();
            bp.Save();

            return true;
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

        /***
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Grand Total = 123.00 (#1)
            sb.Append(": ").
                Append(Msg.Translate(GetCtx(), "GrandTotal")).Append("=").Append(GetGrandTotal())
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
            return GetSalesRep_ID();
        }

        /**
         * 	Get Document Approval Amount
         *	@return amount
         */
        public Decimal GetApprovalAmt()
        {
            return GetGrandTotal();
        }

        /**
         * 	Set Price List - Callout
         *	@param oldM_PriceList_ID old value
         *	@param newM_PriceList_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetM_PriceList_ID(String oldM_PriceList_ID, String newM_PriceList_ID, int windowNo)
        {
            if (newM_PriceList_ID == null || newM_PriceList_ID.Length == 0)
                return;
            int M_PriceList_ID = int.Parse(newM_PriceList_ID);
            if (M_PriceList_ID == 0)
                return;

            String sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";
            //	Use newest price list - may not be future
            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, null);
                if (dr.Read())
                {
                    base.SetM_PriceList_ID(M_PriceList_ID);
                    //	Tax Included
                    SetIsTaxIncluded("Y".Equals(dr[0].ToString()));
                    //	Price Limit Enforce
                    //if (p_changeVO != null)
                    //{
                    //	p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", dr.getString(2));
                    //}
                    //	Currency
                    int ii = Utility.Util.GetValueOfInt(dr[2]);
                    SetC_Currency_ID(ii);
                    //	PriceList Version
                    //if (p_changeVO != null)
                    //{
                    //	p_changeVO.setContext(GetCtx(), windowNo, "M_PriceList_Version_ID", dr.getInt(5));
                    //}
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }

                log.Log(Level.SEVERE, sql, e);
            }

        }

        /**
         * 
         * @param oldC_PaymentTerm_ID
         * @param newC_PaymentTerm_ID
         * @param windowNo
         * @throws Exception
         */
        ///@UICallout
        public void SetC_PaymentTerm_ID(String oldC_PaymentTerm_ID, String newC_PaymentTerm_ID, int windowNo)
        {
            if (newC_PaymentTerm_ID == null || newC_PaymentTerm_ID.Length == 0)
                return;
            int C_PaymentTerm_ID = int.Parse(newC_PaymentTerm_ID);
            int C_Invoice_ID = GetC_Invoice_ID();
            if (C_PaymentTerm_ID == 0 || C_Invoice_ID == 0)	//	not saved yet
                return;

            MPaymentTerm pt = new MPaymentTerm(GetCtx(), C_PaymentTerm_ID, null);
            if (pt.Get_ID() == 0)
            {
                //addError(Msg.getMsg(GetCtx(), "PaymentTerm not found"));
            }

            bool valid = pt.Apply(C_Invoice_ID);
            SetIsPayScheduleValid(valid);
            return;
        }

        /**
         *	Invoice Header - DocType.
         *		- PaymentRule
         *		- temporary Document
         *  Ctx:
         *  	- DocSubTypeSO
         *		- HasCharges
         *	- (re-sets Business Partner Info of required)
         *	@param ctx context
         *	@param WindowNo window no
         *	@param mTab tab
         *	@param mField field
         *	@param value value
         *	@return null or error message
         */
        ///@UICallout
        public void SetC_DocTypeTarget_ID(String oldC_DocTypeTarget_ID, String newC_DocTypeTarget_ID, int WindowNo)
        {
            if (newC_DocTypeTarget_ID == null || newC_DocTypeTarget_ID.Length == 0)
            {
                return;
            }
            int C_DocType_ID = Utility.Util.GetValueOfInt(newC_DocTypeTarget_ID);
            if (C_DocType_ID.ToString() == null || C_DocType_ID == 0)
            {
                return;
            }

            String sql = "SELECT d.HasCharges,'N',d.IsDocNoControlled,"
                + "s.CurrentNext, d.DocBaseType "
                /*//jz outer join
                + "FROM C_DocType d, AD_Sequence s "
                + "WHERE C_DocType_ID=?"		//	1
                + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
                */
                + "FROM C_DocType d "
                + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
                + "WHERE C_DocType_ID=" + C_DocType_ID;		//	1

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql, null, null);
                if (dr.Read())
                {
                    //	Charges - Set Ctx
                    SetContext(WindowNo, "HasCharges", dr[0].ToString());
                    //	DocumentNo
                    if (dr[2].ToString().Equals("Y"))
                        SetDocumentNo("<" + dr[3].ToString() + ">");
                    //  DocBaseType - Set Ctx
                    String s = dr[4].ToString();
                    SetContext(WindowNo, "DocBaseType", s);
                    //  AP Check & AR Credit Memo
                    if (s.StartsWith("AP"))
                        SetPaymentRule("S");    //  Check
                    else if (s.EndsWith("C"))
                        SetPaymentRule("P");    //  OnCredit
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            return;
        }

        /**
         *	Invoice Header- BPartner.
         *		- M_PriceList_ID (+ Ctx)
         *		- C_BPartner_Location_ID
         *		- AD_User_ID
         *		- POReference
         *		- SO_Description
         *		- IsDiscountPrinted
         *		- PaymentRule
         *		- C_PaymentTerm_ID
         *	@param ctx context
         *	@param WindowNo window no
         *	@param mTab tab
         *	@param mField field
         *	@param value value
         *	@return null or error message
         */
        //@UICallout
        public void SetC_BPartner_ID(String oldC_BPartner_ID, String newC_BPartner_ID, int WindowNo)
        {
            if (newC_BPartner_ID == null || newC_BPartner_ID.Length == 0)
                return;
            int C_BPartner_ID = int.Parse(newC_BPartner_ID);
            if ( C_BPartner_ID == 0)
                return;

            String sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " l.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            bool isSOTrx = IsSOTrx();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                //
                for (int ik = 0; ik < ds.Tables[0].Rows.Count; ik++)
                {
                    DataRow dr = ds.Tables[0].Rows[ik];
                    //	PriceList & IsTaxIncluded & Currency
                    int ii = Utility.Util.GetValueOfInt(dr[isSOTrx ? "M_PriceList_ID" : "PO_PriceList_ID"].ToString());
                    if (dr != null)
                        SetM_PriceList_ID(ii);
                    else
                    {	//	get default PriceList
                        int i = GetCtx().GetContextAsInt("#M_PriceList_ID");
                        if (i != 0)
                            SetM_PriceList_ID(i);
                    }

                    //	PaymentRule
                    String s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].ToString();
                    if (s != null && s.Length != 0)
                    {
                        if (GetCtx().GetContext(WindowNo, "DocBaseType").EndsWith("C"))	//	Credits are Payment Term
                            s = "P";
                        else if (isSOTrx && (s.Equals("S") || s.Equals("U")))	//	No Check/Transfer for SO_Trx
                            s = "P";											//  Payment Term
                        SetPaymentRule(s);
                    }
                    //  Payment Term
                    ii = (int)dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"];
                    if (dr != null)
                        SetC_PaymentTerm_ID(ii);

                    //	Location
                    int locID = (int)dr["C_BPartner_Location_ID"];
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may belong to differnt BP)
                    if (C_BPartner_ID.ToString().Equals(GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID")))
                    {
                        String loc = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
                        if (loc.Length > 0)
                            locID = int.Parse(loc);
                    }
                    if (locID == 0)
                    {
                        //p_changeVO.addChangedValue("C_BPartner_Location_ID", (String)null);
                    }
                    else
                    {
                        SetC_BPartner_Location_ID(locID);
                    }
                    //	Contact - overwritten by InfoBP selection
                    int contID = (int)dr["AD_User_ID"];
                    if (C_BPartner_ID.ToString().Equals(GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID")))
                    {
                        String cont = GetCtx().GetContext(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
                        if (cont.Length > 0)
                            contID = int.Parse(cont);
                    }
                    SetAD_User_ID(contID);

                    //	CreditAvailable
                    if (isSOTrx)
                    {
                        double CreditLimit = (double)dr["SO_CreditLimit"];
                        if (CreditLimit != 0)
                        {
                            double CreditAvailable = (double)dr["CreditAvailable"];
                            //if (!dr.IsDBNull() && CreditAvailable < 0)
                            if (dr != null && CreditAvailable < 0)
                            {
                                String msg = Msg.GetMsg(GetCtx(), "CreditLimitOver");//, DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
                                //addError(msg);
                            }
                        }
                    }

                    //	PO Reference
                    s = dr["POReference"].ToString();
                    if (s != null && s.Length != 0)
                        SetPOReference(s);
                    else
                        SetPOReference(null);
                    //	SO Description
                    s = dr["SO_Description"].ToString();
                    if (s != null && s.Trim().Length != 0)
                        SetDescription(s);
                    //	IsDiscountPrinted
                    s = dr["IsDiscountPrinted"].ToString();
                    SetIsDiscountPrinted("Y".Equals(s));
                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "bPartner", e);
            }

            return;
        }

        /**
         * 	Set DateInvoiced - Callout
         *	@param oldDateInvoiced old
         *	@param newDateInvoiced new
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetDateInvoiced(String oldDateInvoiced, String newDateInvoiced, int windowNo)
        {
            if (newDateInvoiced == null || newDateInvoiced.Length == 0)
                return;
            DateTime dateInvoiced = Convert.ToDateTime(PO.ConvertToTimestamp(newDateInvoiced));
            if (dateInvoiced == null)
                return;
            SetDateInvoiced(dateInvoiced);
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
