/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInOut
 * Purpose        : Class linked with the shipment,invoice window
 * Class Used     : X_M_InOut, DocAction
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
    public class MInOut : X_M_InOut, DocAction
    {
        #region variable
        //	Process Message 			
        private String _processMsg = null;
        //	Just Prepared Flag			
        private bool _justPrepared = false;
        //	Lines					
        private MInOutLine[] _lines = null;
        // Confirmations			
        private MInOutConfirm[] _confirms = null;
        // BPartner				
        private MBPartner _partner = null;
        // Reversal Flag		
        private bool _reversal = false;

        private string sql = "";
        private Decimal? trxQty = 0;
        private bool isGetFromStorage = false;

        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_InOut_ID"></param>
        /// <param name="trxName">rx name</param>
        public MInOut(Ctx ctx, int M_InOut_ID, Trx trxName)
            : base(ctx, M_InOut_ID, trxName)
        {

            if (M_InOut_ID == 0)
            {
                //	setDocumentNo (null);
                //	setC_BPartner_ID (0);
                //	setC_BPartner_Location_ID (0);
                //	setM_Warehouse_ID (0);
                //	setC_DocType_ID (0);
                SetIsSOTrx(false);
                SetMovementDate(DateTime.Now);
                SetDateAcct(GetMovementDate());
                //	setMovementType (MOVEMENTTYPE_CustomerShipment);
                SetDeliveryRule(DELIVERYRULE_Availability);
                SetDeliveryViaRule(DELIVERYVIARULE_Pickup);
                SetFreightCostRule(FREIGHTCOSTRULE_FreightIncluded);
                SetDocStatus(DOCSTATUS_Drafted);
                SetDocAction(DOCACTION_Complete);
                SetPriorityRule(PRIORITYRULE_Medium);
                SetNoPackages(0);
                SetIsInTransit(false);
                SetIsPrinted(false);
                SetSendEMail(false);
                SetIsInDispute(false);
                SetIsReturnTrx(false);
                //
                SetIsApproved(false);
                base.SetProcessed(false);
                SetProcessing(false);
                SetPosted(false);
            }
        }

        /* Create Shipment From Order
        *	@param order order
        *	@param movementDate optional movement date
        *	@param forceDelivery ignore order delivery rule
        *	@param allAttributeInstances if true, all attribute set instances
        *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances
        *	@param complete complete document (Process if false, Complete if true)
        *	@param trxName transaction  
        *	@return Shipment or null
        */
        public static MInOut CreateFrom(MOrder order, DateTime? movementDate,
            bool forceDelivery, bool allAttributeInstances, DateTime? minGuaranteeDate,
            bool complete, Trx trxName)
        {
            if (order == null)
            {
                throw new ArgumentException("No Order");
            }
            //
            if (!forceDelivery && DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
            {
                return null;
            }

            //	Create Meader
            MInOut retValue = new MInOut(order, 0, movementDate);
            retValue.SetDocAction(complete ? DOCACTION_Complete : DOCACTION_Prepare);

            //	Check if we can create the lines
            MOrderLine[] oLines = order.GetLines(true, "M_Product_ID");
            for (int i = 0; i < oLines.Length; i++)
            {
                //Decimal qty = oLines[i].GetQtyOrdered().subtract(oLines[i].getQtyDelivered());
                Decimal qty = Decimal.Subtract(oLines[i].GetQtyOrdered(), oLines[i].GetQtyDelivered());
                //	Nothing to deliver
                if (qty == 0)
                {
                    continue;
                }
                //	Stock Info
                MStorage[] storages = null;
                MProduct product = oLines[i].GetProduct();
                if (product != null && product.Get_ID() != 0 && product.IsStocked())
                {
                    MProductCategory pc = MProductCategory.Get(order.GetCtx(), product.GetM_Product_Category_ID());
                    String MMPolicy = pc.GetMMPolicy();
                    if (MMPolicy == null || MMPolicy.Length == 0)
                    {
                        MClient client = MClient.Get(order.GetCtx());
                        MMPolicy = client.GetMMPolicy();
                    }
                    storages = MStorage.GetWarehouse(order.GetCtx(), order.GetM_Warehouse_ID(),
                        oLines[i].GetM_Product_ID(), oLines[i].GetM_AttributeSetInstance_ID(),
                        product.GetM_AttributeSet_ID(),
                        allAttributeInstances, minGuaranteeDate,
                        MClient.MMPOLICY_FiFo.Equals(MMPolicy), trxName);
                }
                if (!forceDelivery)
                {
                    Decimal maxQty = Env.ZERO;
                    for (int ll = 0; ll < storages.Length; ll++)
                        maxQty = Decimal.Add(maxQty, storages[ll].GetQtyOnHand());
                    if (DELIVERYRULE_Availability.Equals(order.GetDeliveryRule()))
                    {
                        if (maxQty.CompareTo(qty) < 0)
                            qty = maxQty;
                    }
                    else if (DELIVERYRULE_CompleteLine.Equals(order.GetDeliveryRule()))
                    {
                        if (maxQty.CompareTo(qty) < 0)
                            continue;
                    }
                }
                //	Create Line
                if (retValue.Get_ID() == 0)	//	not saved yet
                    retValue.Save(trxName);
                //	Create a line until qty is reached
                for (int ll = 0; ll < storages.Length; ll++)
                {
                    Decimal lineQty = storages[ll].GetQtyOnHand();
                    if (lineQty.CompareTo(qty) > 0)
                        lineQty = qty;
                    MInOutLine line = new MInOutLine(retValue);
                    line.SetOrderLine(oLines[i], storages[ll].GetM_Locator_ID(),
                        order.IsSOTrx() ? lineQty : Env.ZERO);
                    line.SetQty(lineQty);	//	Correct UOM for QtyEntered
                    if (oLines[i].GetQtyEntered().CompareTo(oLines[i].GetQtyOrdered()) != 0)
                    {
                        //line.SetQtyEntered(lineQty.multiply(oLines[i].getQtyEntered()).divide(oLines[i].getQtyOrdered(), 12, Decimal.ROUND_HALF_UP));
                        line.SetQtyEntered(Decimal.Multiply(lineQty, Decimal.Divide(oLines[i].GetQtyEntered(), Decimal.Round(oLines[i].GetQtyOrdered(), 12, MidpointRounding.AwayFromZero))));
                    }
                    line.SetC_Project_ID(oLines[i].GetC_Project_ID());
                    line.Save(trxName);
                    //	Delivered everything ?
                    qty = Decimal.Subtract(qty, lineQty);
                    if (qty == 0)
                    {
                        break;
                    }
                }
            }	//	for all order lines

            //	No Lines saved		
            if (retValue.Get_ID() == 0)
            {
                return null;
            }

            return retValue;
        }

        /**
         * 	Create new Shipment by copying
         * 	@param from shipment
         * 	@param dateDoc date of the document date
         * 	@param C_DocType_ID doc type
         * 	@param isSOTrx sales order
         * 	@param counter create counter links
         * 	@param trxName trx
         * 	@param setOrder set the order link
         *	@return Shipment
         */
        public static MInOut CopyFrom(MInOut from, DateTime? dateDoc,
            int C_DocType_ID, bool isSOTrx, bool isReturnTrx,
            bool counter, Trx trxName, bool setOrder)
        {
            MInOut to = new MInOut(from.GetCtx(), 0, null);
            to.Set_TrxName(trxName);
            CopyValues(from, to, from.GetAD_Client_ID(), from.GetAD_Org_ID());
            to.Set_ValueNoCheck("M_InOut_ID", I_ZERO);
            to.Set_ValueNoCheck("DocumentNo", null);
            //
            to.SetDocStatus(DOCSTATUS_Drafted);		//	Draft
            to.SetDocAction(DOCACTION_Complete);
            //
            to.SetC_DocType_ID(C_DocType_ID);
            to.SetIsReturnTrx(isReturnTrx);
            to.SetIsSOTrx(isSOTrx);
            if (counter)
            {
                if (!isReturnTrx)
                {
                    to.SetMovementType(isSOTrx ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
                }
                else
                {
                    to.SetMovementType(isSOTrx ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);
                }
            }
            //
            to.SetDateOrdered(dateDoc);
            to.SetDateAcct(dateDoc);
            to.SetMovementDate(dateDoc);
            to.SetDatePrinted(null);
            to.SetIsPrinted(false);
            to.SetDateReceived(null);
            to.SetNoPackages(0);
            to.SetShipDate(null);
            to.SetPickDate(null);
            to.SetIsInTransit(false);
            //
            to.SetIsApproved(false);
            to.SetC_Invoice_ID(0);
            to.SetTrackingNo(null);
            to.SetIsInDispute(false);
            //
            to.SetPosted(false);
            to.SetProcessed(false);
            to.SetC_Order_ID(0);	//	Overwritten by setOrder
            if (counter)
            {
                to.SetC_Order_ID(0);
                to.SetRef_InOut_ID(from.GetM_InOut_ID());
                //	Try to find Order/Invoice link
                if (from.GetC_Order_ID() != 0)
                {
                    MOrder peer = new MOrder(from.GetCtx(), from.GetC_Order_ID(), from.Get_TrxName());
                    if (peer.GetRef_Order_ID() != 0)
                        to.SetC_Order_ID(peer.GetRef_Order_ID());
                }
                if (from.GetC_Invoice_ID() != 0)
                {
                    MInvoice peer = new MInvoice(from.GetCtx(), from.GetC_Invoice_ID(), from.Get_TrxName());
                    if (peer.GetRef_Invoice_ID() != 0)
                        to.SetC_Invoice_ID(peer.GetRef_Invoice_ID());
                }
            }
            else
            {
                to.SetRef_InOut_ID(0);
                if (setOrder)
                    to.SetC_Order_ID(from.GetC_Order_ID());
            }
            //Amit for Reverse Case handle 8-jan-2015
            to.SetDescription("RC");
            //Amit

            //
            if (!to.Save(trxName))
            {
                throw new Exception("Could not create Shipment");
            }
            if (counter)
            {
                from.SetRef_InOut_ID(to.GetM_InOut_ID());
            }

            if (to.CopyLinesFrom(from, counter, setOrder) == 0)
            {
                throw new Exception("Could not create Shipment Lines");
            }

            return to;
        }

        /**
         *  Load Constructor
         *  @param ctx context
         *  @param dr result set record
         *	@param trxName transaction  
         */
        public MInOut(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Order Constructor - create header only
         *	@param order order
         *	@param movementDate optional movement date (default today)
         *	@param C_DocTypeShipment_ID document type or 0
         */
        public MInOut(MOrder order, int C_DocTypeShipment_ID, DateTime? movementDate)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {

            SetOrder(order);

            if (C_DocTypeShipment_ID == 0)
            {
                C_DocTypeShipment_ID = int.Parse(ExecuteQuery.ExecuteScalar("SELECT C_DocTypeShipment_ID FROM C_DocType WHERE C_DocType_ID=" + order.GetC_DocType_ID()));
            }
            SetC_DocType_ID(C_DocTypeShipment_ID, true);

            //	Default - Today
            if (movementDate != null)
            {
                SetMovementDate(movementDate);
            }
            SetDateAcct(GetMovementDate());

        }



        /**
         * 	Invoice Constructor - create header only
         *	@param invoice invoice
         *	@param C_DocTypeShipment_ID document type or 0
         *	@param movementDate optional movement date (default today)
         *	@param M_Warehouse_ID warehouse
         */
        public MInOut(MInvoice invoice, int C_DocTypeShipment_ID,
            DateTime? movementDate, int M_Warehouse_ID)
            : this(invoice.GetCtx(), 0, invoice.Get_TrxName())
        {
            SetClientOrg(invoice);
            MOrder ord = new MOrder(GetCtx(), invoice.GetC_Order_ID(), null);
            SetC_BPartner_ID(ord.GetC_BPartner_ID());
            //SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(ord.GetC_BPartner_Location_ID());	//	shipment address
            SetAD_User_ID(ord.GetAD_User_ID());
            //
            SetM_Warehouse_ID(M_Warehouse_ID);
            SetIsSOTrx(invoice.IsSOTrx());
            SetIsReturnTrx(invoice.IsReturnTrx());

            if (!IsReturnTrx())
                SetMovementType(invoice.IsSOTrx() ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
            else
                SetMovementType(invoice.IsSOTrx() ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);

            MOrder order = null;
            if (invoice.GetC_Order_ID() != 0)
                order = new MOrder(invoice.GetCtx(), invoice.GetC_Order_ID(), invoice.Get_TrxName());
            if (C_DocTypeShipment_ID == 0 && order != null)
                C_DocTypeShipment_ID = int.Parse(ExecuteQuery.ExecuteScalar("SELECT C_DocTypeShipment_ID FROM C_DocType WHERE C_DocType_ID=" + order.GetC_DocType_ID()));
            if (C_DocTypeShipment_ID != 0)
                SetC_DocType_ID(C_DocTypeShipment_ID, true);
            else
                SetC_DocType_ID();

            //	Default - Today
            if (movementDate != null)
                SetMovementDate(movementDate);
            SetDateAcct(GetMovementDate());

            //	Copy from Invoice
            SetC_Order_ID(invoice.GetC_Order_ID());
            SetSalesRep_ID(invoice.GetSalesRep_ID());
            //
            SetC_Activity_ID(invoice.GetC_Activity_ID());
            SetC_Campaign_ID(invoice.GetC_Campaign_ID());
            SetC_Charge_ID(invoice.GetC_Charge_ID());
            SetChargeAmt(invoice.GetChargeAmt());
            //
            SetC_Project_ID(invoice.GetC_Project_ID());
            SetDateOrdered(invoice.GetDateOrdered());
            SetDescription(invoice.GetDescription());
            SetPOReference(invoice.GetPOReference());
            SetAD_OrgTrx_ID(invoice.GetAD_OrgTrx_ID());
            SetUser1_ID(invoice.GetUser1_ID());
            SetUser2_ID(invoice.GetUser2_ID());

            if (order != null)
            {
                SetDeliveryRule(order.GetDeliveryRule());
                SetDeliveryViaRule(order.GetDeliveryViaRule());
                SetM_Shipper_ID(order.GetM_Shipper_ID());
                SetFreightCostRule(order.GetFreightCostRule());
                SetFreightAmt(order.GetFreightAmt());
            }
        }

        /**
         * 	Copy Constructor - create header only
         *	@param original original 
         *	@param movementDate optional movement date (default today)
         *	@param C_DocTypeShipment_ID document type or 0
         */
        public MInOut(MInOut original, int C_DocTypeShipment_ID, DateTime? movementDate)
            : this(original.GetCtx(), 0, original.Get_TrxName())
        {

            SetClientOrg(original);
            SetC_BPartner_ID(original.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(original.GetC_BPartner_Location_ID());	//	shipment address
            SetAD_User_ID(original.GetAD_User_ID());
            //
            SetM_Warehouse_ID(original.GetM_Warehouse_ID());
            SetIsSOTrx(original.IsSOTrx());
            SetMovementType(original.GetMovementType());
            if (C_DocTypeShipment_ID == 0)
            {
                SetC_DocType_ID(original.GetC_DocType_ID());
                SetIsReturnTrx(original.IsReturnTrx());
            }
            else
                SetC_DocType_ID(C_DocTypeShipment_ID, true);

            //	Default - Today
            if (movementDate != null)
                SetMovementDate(movementDate);
            SetDateAcct(GetMovementDate());

            //	Copy from Order
            SetC_Order_ID(original.GetC_Order_ID());
            SetDeliveryRule(original.GetDeliveryRule());
            SetDeliveryViaRule(original.GetDeliveryViaRule());
            SetM_Shipper_ID(original.GetM_Shipper_ID());
            SetFreightCostRule(original.GetFreightCostRule());
            SetFreightAmt(original.GetFreightAmt());
            SetSalesRep_ID(original.GetSalesRep_ID());
            //
            SetC_Activity_ID(original.GetC_Activity_ID());
            SetC_Campaign_ID(original.GetC_Campaign_ID());
            SetC_Charge_ID(original.GetC_Charge_ID());
            SetChargeAmt(original.GetChargeAmt());
            //
            SetC_Project_ID(original.GetC_Project_ID());
            SetDateOrdered(original.GetDateOrdered());
            SetDescription(original.GetDescription());
            SetPOReference(original.GetPOReference());
            SetSalesRep_ID(original.GetSalesRep_ID());
            SetAD_OrgTrx_ID(original.GetAD_OrgTrx_ID());
            SetUser1_ID(original.GetUser1_ID());
            SetUser2_ID(original.GetUser2_ID());
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
         *	String representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInOut[")
                .Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",DocStatus=").Append(GetDocStatus())
                .Append("]");
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
         * 	Create PDF
         *	@return File or null
         */
        public FileInfo CreatePDF()
        {
            try
            {
                //string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                String fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo() + ".pdf";
                string filePath = Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "TempDownload", fileName);


                ReportEngine_N re = ReportEngine_N.Get(GetCtx(), ReportEngine_N.SHIPMENT, GetC_Invoice_ID());
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

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {

            //ReportEngine re = ReportEngine.get(getCtx(), ReportEngine.SHIPMENT, getC_Invoice_ID());
            //if (re == null)
            //    return null;
            //return re.getPDF(file);
            return null;
        }

        /**
         * 	Get Lines of Shipment
         * 	@param requery refresh from db
         * 	@return lines
         */
        public MInOutLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            List<MInOutLine> list = new List<MInOutLine>();
            String sql = "SELECT * FROM M_InOutLine WHERE M_InOut_ID=" + GetM_InOut_ID() + " ORDER BY Line";
            DataSet ds = null;
            DataRow dr = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    dr = ds.Tables[0].Rows[i];
                    list.Add(new MInOutLine(GetCtx(), dr, Get_TrxName()));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                log.Log(Level.SEVERE, sql, ex);
                list = null;
            }
            ds = null;
            //
            if (list == null)
                return null;
            _lines = new MInOutLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Get Lines of Shipment
         * 	@return lines
         */
        public MInOutLine[] GetLines()
        {
            return GetLines(false);
        }

        /**
         * 	Get Confirmations
         * 	@param requery requery
         *	@return array of Confirmations
         */
        public MInOutConfirm[] GetConfirmations(bool requery)
        {
            if (_confirms != null && !requery)
                return _confirms;

            List<MInOutConfirm> list = new List<MInOutConfirm>();
            String sql = "SELECT * FROM M_InOutConfirm WHERE M_InOut_ID=" + GetM_InOut_ID();
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
                    list.Add(new MInOutConfirm(GetCtx(), dr, Get_TrxName()));
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
            _confirms = new MInOutConfirm[list.Count];
            _confirms = list.ToArray();
            return _confirms;
        }

        /**
         * 	Copy Lines From other Shipment
         *	@param otherShipment shipment
         *	@param counter set counter Info
         *	@param setOrder set order link
         *	@return number of lines copied
         */
        public int CopyLinesFrom(MInOut otherShipment, bool counter, bool setOrder)
        {
            if (IsProcessed() || IsPosted() || otherShipment == null)
                return 0;
            MInOutLine[] fromLines = otherShipment.GetLines(false);
            int count = 0;
            for (int i = 0; i < fromLines.Length; i++)
            {
                MInOutLine line = new MInOutLine(this);
                MInOutLine fromLine = fromLines[i];
                line.Set_TrxName(Get_TrxName());
                if (counter)	//	header
                    PO.CopyValues(fromLine, line, GetAD_Client_ID(), GetAD_Org_ID());
                else
                    PO.CopyValues(fromLine, line, fromLine.GetAD_Client_ID(), fromLine.GetAD_Org_ID());
                line.SetM_InOut_ID(GetM_InOut_ID());
                line.Set_ValueNoCheck("M_InOutLine_ID", I_ZERO);	//	new
                //	Reset
                if (!setOrder)
                    line.SetC_OrderLine_ID(0);
                if (!counter)
                    line.SetM_AttributeSetInstance_ID(0);
                //	line.setS_ResourceAssignment_ID(0);
                line.SetRef_InOutLine_ID(0);
                line.SetIsInvoiced(false);
                //
                line.SetConfirmedQty(Env.ZERO);
                line.SetPickedQty(Env.ZERO);
                line.SetScrappedQty(Env.ZERO);
                line.SetTargetQty(Env.ZERO);
                //	Set Locator based on header Warehouse
                if (GetM_Warehouse_ID() != otherShipment.GetM_Warehouse_ID())
                {
                    line.SetM_Locator_ID(0);
                    line.SetM_Locator_ID((int)Env.ZERO);
                }
                //
                if (counter)
                {
                    line.SetRef_InOutLine_ID(fromLine.GetM_InOutLine_ID());
                    if (fromLine.GetC_OrderLine_ID() != 0)
                    {
                        MOrderLine peer = new MOrderLine(GetCtx(), fromLine.GetC_OrderLine_ID(), Get_TrxName());
                        if (peer.GetRef_OrderLine_ID() != 0)
                            line.SetC_OrderLine_ID(peer.GetRef_OrderLine_ID());
                    }
                }
                //
                line.SetProcessed(false);
                if (line.Save(Get_TrxName()))
                    count++;
                //	Cross Link
                if (counter)
                {
                    fromLine.SetRef_InOutLine_ID(line.GetM_InOutLine_ID());
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
         * 	Copy from Order
         *	@param order order
         */
        private void SetOrder(MOrder order)
        {
            SetClientOrg(order);
            SetC_Order_ID(order.GetC_Order_ID());
            //
            SetC_BPartner_ID(order.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());	//	shipment address
            SetAD_User_ID(order.GetAD_User_ID());
            //
            SetM_Warehouse_ID(order.GetM_Warehouse_ID());
            SetIsSOTrx(order.IsSOTrx());
            SetIsReturnTrx(order.IsReturnTrx());

            if (!IsReturnTrx())
                SetMovementType(order.IsSOTrx() ? MOVEMENTTYPE_CustomerShipment : MOVEMENTTYPE_VendorReceipts);
            else
                SetMovementType(order.IsSOTrx() ? MOVEMENTTYPE_CustomerReturns : MOVEMENTTYPE_VendorReturns);
            //
            SetDeliveryRule(order.GetDeliveryRule());
            SetDeliveryViaRule(order.GetDeliveryViaRule());
            SetM_Shipper_ID(order.GetM_Shipper_ID());
            SetFreightCostRule(order.GetFreightCostRule());
            SetFreightAmt(order.GetFreightAmt());
            SetSalesRep_ID(order.GetSalesRep_ID());
            //
            SetC_Activity_ID(order.GetC_Activity_ID());
            SetC_Campaign_ID(order.GetC_Campaign_ID());
            SetC_Charge_ID(order.GetC_Charge_ID());
            SetChargeAmt(order.GetChargeAmt());
            //
            SetC_Project_ID(order.GetC_Project_ID());
            SetDateOrdered(order.GetDateOrdered());
            SetDescription(order.GetDescription());
            SetPOReference(order.GetPOReference());
            SetSalesRep_ID(order.GetSalesRep_ID());
            SetAD_OrgTrx_ID(order.GetAD_OrgTrx_ID());
            SetUser1_ID(order.GetUser1_ID());
            SetUser2_ID(order.GetUser2_ID());

        }

        /**
         * 	Set Order - Callout
         *	@param oldC_Order_ID old BP
         *	@param newC_Order_ID new BP
         *	@param windowNo window no
         */
        //@UICallout Web user interface method
        public void SetC_Order_ID(String oldC_Order_ID, String newC_Order_ID, int windowNo)
        {
            if (newC_Order_ID == null || newC_Order_ID.Length == 0)
                return;
            int C_Order_ID = int.Parse(newC_Order_ID);
            if (C_Order_ID == 0)
                return;
            //	Get Details
            MOrder order = new MOrder(GetCtx(), C_Order_ID, null);
            if (order.Get_ID() != 0)
                SetOrder(order);
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
            String sql = "UPDATE M_InOutLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE M_InOut_ID=" + GetM_InOut_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine(processed + " - Lines=" + noLine);
        }

        /**
         * 	Get BPartner
         *	@return partner
         */
        public MBPartner GetBPartner()
        {
            if (_partner == null)
                _partner = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            return _partner;
        }

        /**
         * 	Set Document Type
         * 	@param DocBaseType doc type MDocBaseType.DOCBASETYPE_
         */
        public void SetC_DocType_ID(String DocBaseType)
        {
            String sql = "SELECT C_DocType_ID FROM C_DocType "
                + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND DocBaseType=" + DocBaseType
                + " AND IsActive='Y' AND IsReturnTrx='N'"
                + " AND IsSOTrx='" + (IsSOTrx() ? "Y" : "N") + "' "
                + "ORDER BY IsDefault DESC";
            int C_DocType_ID = int.Parse(ExecuteQuery.ExecuteScalar(sql));
            if (C_DocType_ID <= 0)
            {
                log.Log(Level.SEVERE, "Not found for AC_Client_ID="
                     + GetAD_Client_ID() + " - " + DocBaseType);
            }
            else
            {
                log.Fine("DocBaseType=" + DocBaseType + " - C_DocType_ID=" + C_DocType_ID);
                SetC_DocType_ID(C_DocType_ID);
                bool isSOTrx = MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(DocBaseType);
                SetIsSOTrx(isSOTrx);
                SetIsReturnTrx(false);
            }
        }

        /**
         * 	Set Default C_DocType_ID.
         * 	Based on SO flag
         */
        public void SetC_DocType_ID()
        {
            if (IsSOTrx())
                SetC_DocType_ID(MDocBaseType.DOCBASETYPE_MATERIALDELIVERY);
            else
                SetC_DocType_ID(MDocBaseType.DOCBASETYPE_MATERIALRECEIPT);
        }

        /**
         * 	Set Document Type
         *	@param C_DocType_ID dt
         *	@param setReturnTrx if true set IsRteurnTrx and SOTrx
         */
        public void SetC_DocType_ID(int C_DocType_ID, bool setReturnTrx)
        {
            base.SetC_DocType_ID(C_DocType_ID);
            if (setReturnTrx)
            {
                MDocType dt = MDocType.Get(GetCtx(), C_DocType_ID);
                SetIsReturnTrx(dt.IsReturnTrx());
                SetIsSOTrx(dt.IsSOTrx());
            }
        }

        /**
         * 	Set Document Type - Callout.
         * 	Sets MovementType, DocumentNo
         * 	@param oldC_DocType_ID old ID
         * 	@param newC_DocType_ID new ID
         * 	@param windowNo window
         */
        //	@UICallout
        public void SetC_DocType_ID(String oldC_DocType_ID,
               String newC_DocType_ID, int windowNo)
        {
            if (newC_DocType_ID == null || newC_DocType_ID.Length == 0)
                return;
            int C_DocType_ID = int.Parse(newC_DocType_ID);
            if (C_DocType_ID == 0)
                return;
            String sql = "SELECT d.DocBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
                + "FROM C_DocType d, AD_Sequence s "
                + "WHERE C_DocType_ID=" + C_DocType_ID		//	1
                + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
            try
            {
                DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetC_DocType_ID(C_DocType_ID);
                    /********************************************************************************************/
                    /********************************************************************************************/
                    //Consequences of - Field Value Changes	- New Row- Save (Update/Insert) Row
                    //Set Ctx - add to changed Ctx
                    //p_changeVO.setContext(getCtx(), windowNo, "C_DocTypeTarget_ID", C_DocType_ID);
                    //	Set Movement Type
                    String DocBaseType = dr["DocBaseType"].ToString();
                    Boolean IsReturnTrx = "Y".Equals(dr[3].ToString());

                    if (DocBaseType.Equals(MDocBaseType.DOCBASETYPE_MATERIALDELIVERY))		//	Shipments
                    {
                        if (IsReturnTrx)
                            SetMovementType(MOVEMENTTYPE_CustomerReturns);
                        else
                            SetMovementType(MOVEMENTTYPE_CustomerShipment);
                    }
                    else if (DocBaseType.Equals(MDocBaseType.DOCBASETYPE_MATERIALRECEIPT))	//	Receipts
                    {
                        if (IsReturnTrx)
                            SetMovementType(MOVEMENTTYPE_VendorReturns);
                        else
                            SetMovementType(MOVEMENTTYPE_VendorReceipts);
                    }

                    //	DocumentNo
                    if (dr["IsDocNoControlled"].ToString().Equals("Y"))
                        SetDocumentNo("<" + dr["CurrentNext"].ToString() + ">");
                    SetIsReturnTrx(IsReturnTrx);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /**
         * 	Set Business Partner Defaults & Details
         * 	@param bp business partner
         */
        public void SetBPartner(MBPartner bp)
        {
            if (bp == null)
                return;

            SetC_BPartner_ID(bp.GetC_BPartner_ID());

            //	Set Locations*******************************************************************************
            MBPartnerLocation[] locs = bp.GetLocations(false);
            if (locs != null)
            {
                for (int i = 0; i < locs.Length; i++)
                {
                    if (locs[i].IsShipTo())
                        SetC_BPartner_Location_ID(locs[i].GetC_BPartner_Location_ID());
                }
                //	set to first if not set
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
        /// Set Business Partner - Callout
        /// </summary>
        /// <param name="oldC_BPartner_ID">old BP</param>
        /// <param name="newC_BPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetC_BPartner_ID(String oldC_BPartner_ID,
               String newC_BPartner_ID, int windowNo)
        {
            if (newC_BPartner_ID == null || newC_BPartner_ID.Length == 0)
                return;
            int C_BPartner_ID = int.Parse(newC_BPartner_ID);
            if (C_BPartner_ID == 0)
                return;
            String sql = "SELECT p.AD_Language, p.POReference,"
                + "SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "l.C_BPartner_Location_ID, c.AD_User_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID)"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID;		//	1
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetC_BPartner_ID(C_BPartner_ID);
                    //	Location
                    int ii = (int)dr["C_BPartner_Location_ID"];
                    if (ii != 0)
                        SetC_BPartner_Location_ID(ii);
                    //	Contact
                    ii = (int)dr["AD_User_ID"];
                    SetAD_User_ID(ii);

                    //	CreditAvailable
                    if (IsSOTrx() && !IsReturnTrx())
                    {
                        Decimal CreditLimit = Convert.ToDecimal(dr["SO_CreditLimit"]);
                        if (CreditLimit != null && CreditLimit != 0)
                        {
                            Decimal CreditAvailable = Convert.ToDecimal(dr["CreditAvailable"]);
                            //if (p_changeVO != null && CreditAvailable != null && CreditAvailable < 0)
                            {
                                String msg = Msg.Translate(GetCtx(), "CreditLimitOver");//,DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable));
                                //p_changeVO.addError(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /// <summary>
        /// Set Movement Date - Callout
        /// </summary>
        /// <param name="oldC_BPartner_ID">old BP</param>
        /// <param name="newC_BPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetMovementDate(String oldMovementDate,
            String newMovementDate, int windowNo)
        {
            if (newMovementDate == null || newMovementDate.Length == 0)
                return;
            //		DateTime movementDate = PO.convertToTimestamp(newMovementDate);
            DateTime movementDate = Convert.ToDateTime(newMovementDate);
            if (movementDate == null)
                return;
            SetMovementDate(movementDate);
            SetDateAcct(movementDate);
        }

        /// <summary>
        /// Set Warehouse and check/set Organization
        /// </summary>
        /// <param name="M_Warehouse_ID">id</param>
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID == 0)
            {
                log.Severe("Ignored - Cannot set AD_Warehouse_ID to 0");
                return;
            }
            base.SetM_Warehouse_ID(M_Warehouse_ID);
            //
            MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
            if (wh.GetAD_Org_ID() != GetAD_Org_ID())
            {
                log.Warning("M_Warehouse_ID=" + M_Warehouse_ID
                + ", Overwritten AD_Org_ID=" + GetAD_Org_ID() + "->" + wh.GetAD_Org_ID());
                SetAD_Org_ID(wh.GetAD_Org_ID());
            }
        }

        /// <summary>
        /// Set Business Partner - Callout
        /// </summary>
        /// <param name="oldC_BPartner_ID">old BP</param>
        /// <param name="newC_BPartner_ID">new BP</param>
        /// <param name="windowNo">window no</param>
        /// @UICallout
        public void SetM_Warehouse_ID(String oldM_Warehouse_ID,
            String newM_Warehouse_ID, int windowNo)
        {
            if (newM_Warehouse_ID == null || newM_Warehouse_ID.Length == 0)
                return;
            int M_Warehouse_ID = int.Parse(newM_Warehouse_ID);
            if (M_Warehouse_ID == 0)
                return;
            //
            String sql = "SELECT w.AD_Org_ID, l.M_Locator_ID "
                + "FROM M_Warehouse w"
                + " LEFT OUTER JOIN M_Locator l ON (l.M_Warehouse_ID=w.M_Warehouse_ID AND l.IsDefault='Y') "
                + "WHERE w.M_Warehouse_ID=" + M_Warehouse_ID;		//	1

            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetM_Warehouse_ID(M_Warehouse_ID);
                    //	Org
                    int AD_Org_ID = (int)dr[0];
                    SetAD_Org_ID(AD_Org_ID);
                    //	Locator
                    int M_Locator_ID = (int)dr[1];
                    if (M_Locator_ID != 0)
                    {
                        //p_changeVO.setContext(getCtx(), windowNo, "M_Locator_ID", M_Locator_ID);
                    }
                    else
                    {
                        //p_changeVO.setContext(getCtx(), windowNo, "M_Locator_ID", (String)null);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

        }


        /// <summary>
        /// Create the missing next Confirmation
        /// </summary>
        public void CreateConfirmation()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            bool pick = dt.IsPickQAConfirm();
            bool ship = dt.IsShipConfirm();
            //	Nothing to do
            if (!pick && !ship)
            {
                log.Fine("No need");
                return;
            }

            //	Create Both .. after each other
            if (pick && ship)
            {
                bool havePick = false;
                bool haveShip = false;
                MInOutConfirm[] confirmations = GetConfirmations(false);
                for (int i = 0; i < confirmations.Length; i++)
                {
                    MInOutConfirm confirm = confirmations[i];
                    if (MInOutConfirm.CONFIRMTYPE_PickQAConfirm.Equals(confirm.GetConfirmType()))
                    {
                        if (!confirm.IsProcessed())		//	wait until done
                        {
                            log.Fine("Unprocessed: " + confirm);
                            return;
                        }
                        havePick = true;
                    }
                    else if (MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm.Equals(confirm.GetConfirmType()))
                        haveShip = true;
                }
                //	Create Pick
                if (!havePick)
                {
                    MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_PickQAConfirm, false);
                    return;
                }
                //	Create Ship
                if (!haveShip)
                {
                    MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, false);
                    return;
                }
                return;
            }
            //	Create just one
            if (pick)
                MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_PickQAConfirm, true);
            else if (ship)
                MInOutConfirm.Create(this, MInOutConfirm.CONFIRMTYPE_ShipReceiptConfirm, true);
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true or false</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	Warehouse Org
            if (newRecord)
            {
                MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                if (wh.GetAD_Org_ID() != GetAD_Org_ID())
                {
                    log.SaveError("WarehouseOrgConflict", "");
                    return false;
                }
            }
            //	Shipment - Needs Order
            if (IsSOTrx() && GetC_Order_ID() == 0)
            {
                log.SaveError("FillMandatory", Msg.Translate(GetCtx(), "C_Order_ID"));
                return false;
            }
            if (newRecord || Is_ValueChanged("C_BPartner_ID"))
            {
                MBPartner bp = MBPartner.Get(GetCtx(), GetC_BPartner_ID());
                if (!bp.IsActive())
                {
                    log.SaveError("NotActive", Msg.GetMsg(GetCtx(), "C_BPartner_ID"));
                    return false;
                }
            }


            return true;
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
                String sql = "UPDATE M_InOutLine ol"
                    + " SET AD_Org_ID ="
                        + "(SELECT AD_Org_ID"
                        + " FROM M_InOut o WHERE ol.M_InOut_ID=o.M_InOut_ID) "
                    + "WHERE M_InOut_ID=" + GetC_Order_ID();
                int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql, null, Get_TrxName()));
                log.Fine("Lines -> #" + no);
            }
            return true;
        }

        /****
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public virtual bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }

        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public virtual bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public virtual bool InvalidateIt()
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
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
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


            //	Credit Check
            if (IsSOTrx() && !IsReversal() && !IsReturnTrx())
            {
                MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), null);
                if (MBPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
                {
                    _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
                        + bp.GetTotalOpenBalance()
                        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    return DocActionVariables.STATUS_INVALID;
                }
                if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
                {
                    _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
                        + bp.GetTotalOpenBalance()
                        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    return DocActionVariables.STATUS_INVALID;
                }
                Decimal notInvoicedAmt = MBPartner.GetNotInvoicedAmt(GetC_BPartner_ID());
                if (MBPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus(notInvoicedAmt)))
                {
                    _processMsg = "@BPartnerOverSCreditHold@ - @TotalOpenBalance@="
                        + bp.GetTotalOpenBalance() + ", @NotInvoicedAmt@=" + notInvoicedAmt
                        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            //	Lines
            MInOutLine[] lines = GetLines(true);
            if (lines == null || lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }
            Decimal Volume = Env.ZERO;
            Decimal Weight = Env.ZERO;

            //	Mandatory Attributes
            for (int i = 0; i < lines.Length; i++)
            {
                MInOutLine line = lines[i];
                MProduct product = line.GetProduct();
                if (product != null)
                {
                    Volume = Decimal.Add(Volume, Decimal.Multiply((Decimal)product.GetVolume(),
                        line.GetMovementQty()));
                    Weight = Decimal.Add(Weight, Decimal.Multiply(product.GetWeight(),
                        line.GetMovementQty()));
                }
                if (line.GetM_AttributeSetInstance_ID() != 0)
                    continue;
                if (product != null)
                {
                    int M_AttributeSet_ID = product.GetM_AttributeSet_ID();
                    if (M_AttributeSet_ID != 0)
                    {
                        MAttributeSet mas = MAttributeSet.Get(GetCtx(), M_AttributeSet_ID);
                        if (mas != null
                            && ((IsSOTrx() && mas.IsMandatory())
                                || (!IsSOTrx() && mas.IsMandatoryAlways())))
                        {
                            _processMsg = "@M_AttributeSet_ID@ @IsMandatory@";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
            }
            SetVolume(Volume);
            SetWeight(Weight);
            if (!IsReversal())	//	don't change reversal
            {
                /* nnayak - Bug 1750251 : check material policy and update storage
                   at the line level in completeIt()*/
                // checkMaterialPolicy();	//	set MASI
                CreateConfirmation();
            }

            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /// <summary>
        /// Approve Document
        /// </summary>
        /// <returns>true if success </returns>
        public virtual bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /// <summary>
        /// Reject Approval
        /// </summary>
        /// <returns>true if success </returns>
        public virtual bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /// <summary>
        /// Complete Document
        /// </summary>
        /// <returns>new status (Complete, In Progress, Invalid, Waiting ..)</returns>
        public virtual String CompleteIt()
        {
            //************* Change By Lokesh Chauhan ***************
            // If qty on locator is insufficient then return
            // Will not complete.
            string sql = "";
            if (IsSOTrx())
            {
                sql = "SELECT ISDISALLOWNEGATIVEINV FROM M_Warehouse WHERE M_Warehouse_ID = " + Util.GetValueOfInt(GetM_Warehouse_ID());
                string disallow = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (disallow.ToUpper() == "Y")
                {
                    int[] ioLine = MInOutLine.GetAllIDs("M_InoutLine", "M_inout_id = " + GetM_InOut_ID(), Get_TrxName());
                    int m_locator_id = 0;
                    int m_product_id = 0;
                    bool check = false;
                    for (int i = 0; i < ioLine.Length; i++)
                    {
                        MInOutLine iol = new MInOutLine(Env.GetCtx(), ioLine[i], Get_TrxName());
                        m_locator_id = Util.GetValueOfInt(iol.GetM_Locator_ID());
                        m_product_id = Util.GetValueOfInt(iol.GetM_Product_ID());
                        sql = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + m_product_id;
                        int m_attribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        if (m_attribute_ID == 0)
                        {
                            sql = "SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id;
                            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            int qtyToMove = Util.GetValueOfInt(iol.GetQtyEntered());
                            if (qty < qtyToMove)
                            {
                                check = true;
                                break;
                            }
                        }
                        else
                        {
                            sql = "SELECT SUM(QtyOnHand) FROM M_Storage WHERE M_Locator_ID = " + m_locator_id + " AND M_Product_ID = " + m_product_id + " AND M_AttributeSetInstance_ID = " + iol.GetM_AttributeSetInstance_ID();
                            int qty = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_TrxName()));
                            int qtyToMove = Util.GetValueOfInt(iol.GetQtyEntered());
                            if (qty < qtyToMove)
                            {
                                check = true;
                                break;
                            }
                        }
                    }
                    if (check)
                    {
                        sql = "SELECT Value FROM M_Locator WHERE M_Locator_ID = " + m_locator_id;
                        string loc = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        sql = "SELECT Name FROM M_Product WHERE M_Product_ID = " + m_product_id;
                        string prod = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_TrxName()));
                        _processMsg = Msg.GetMsg(Env.GetCtx(), "InsufficientQuantityFor:" + prod + " On " + loc + " Locator!");
                        // ShowMessage.Info("InsufficientQuantity", true, null, null);
                        return DocActionVariables.STATUS_DRAFTED;
                    }
                }
            }
            //*****************************************************

            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }

            //	Outstanding (not processed) Incoming Confirmations ?
            MInOutConfirm[] confirmations = GetConfirmations(true);
            for (int i = 0; i < confirmations.Length; i++)
            {
                MInOutConfirm confirm = confirmations[i];
                if (!confirm.IsProcessed())
                {
                    if (MInOutConfirm.CONFIRMTYPE_CustomerConfirmation.Equals(confirm.GetConfirmType()))
                        continue;
                    //
                    _processMsg = "Open @M_InOutConfirm_ID@: " +
                        confirm.GetConfirmTypeName() + " - " + confirm.GetDocumentNo();
                    return DocActionVariables.STATUS_INPROGRESS;
                }
            }
            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());
            StringBuilder Info = new StringBuilder();

            //	For all lines
            MInOutLine[] lines = GetLines(false);
            for (int Index = 0; Index < lines.Length; Index++)
            {
                MInOutLine Line = lines[Index];
                if (Line.GetM_Locator_ID() == 0)
                {
                    _processMsg = Msg.GetMsg(Env.GetCtx(), "LocatorNotFound");
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                MInOutLine sLine = lines[lineIndex];
                MProduct product = sLine.GetProduct();
                if (product != null)
                {
                    MProductCategory pc = new MProductCategory(Env.GetCtx(), product.GetM_Product_Category_ID(), Get_TrxName());
                    if (IsSOTrx() && !IsReturnTrx() && pc.GetA_Asset_Group_ID() > 0 && sLine.GetA_Asset_ID() == 0)
                    {
                        _processMsg = "AssetNotSetONShipmentLine: LineNo" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }
                if (IsSOTrx() && sLine.GetA_Asset_ID() != 0)
                {
                    MAsset ast = new MAsset(Env.GetCtx(), sLine.GetA_Asset_ID(), Get_TrxName());
                    ast.SetIsDisposed(true);
                    ast.SetAssetDisposalDate(GetDateAcct());
                    if (!ast.Save(Get_TrxName()))
                    {
                        _processMsg = "AssetNotUpdated" + sLine.GetLine() + " :-->" + sLine.GetDescription();
                        return DocActionVariables.STATUS_INPROGRESS;
                    }
                }



                //	Qty & Type
                String MovementType = GetMovementType();
                Decimal Qty = sLine.GetMovementQty();
                //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                {
                    Qty = Decimal.Negate(Qty);
                }
                Decimal QtySO = Env.ZERO;
                Decimal QtyPO = Env.ZERO;
                decimal qtytoset = Env.ZERO;
                //	Update Order Line
                MOrderLine oLine = null;
                if (sLine.GetC_OrderLine_ID() != 0)
                {
                    oLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
                    log.Fine("OrderLine - Reserved=" + oLine.GetQtyReserved()
                    + ", Delivered=" + oLine.GetQtyDelivered());
                    // nnayak - Qty reserved and Qty updated not affected by returns
                    if (!IsReturnTrx())
                    {
                        if (IsSOTrx())
                            QtySO = Decimal.Negate(sLine.GetMovementQty());
                        else
                            QtyPO = Decimal.Negate(sLine.GetMovementQty());
                    }
                }

                log.Info("Line=" + sLine.GetLine() + " - Qty=" + sLine.GetMovementQty() + "Return " + IsReturnTrx());

                /* nnayak - Bug 1750251 : If you have multiple lines for the same product
                in the same Sales Order, or if the generate shipment process was generating
                multiple shipments for the same product in the same run, the first layer 
                was getting consumed by all the shipments. As a result, the first layer had
                negative Inventory even though there were other positive layers. */
                CheckMaterialPolicy(sLine);
                //	Stock Movement - Counterpart MOrder.reserveStock
                if (product != null
                    && product.IsStocked())
                {
                    log.Fine("Material Transaction");
                    MTransaction mtrx = null;
                    ////	Reservation ASI - assume none
                    int reservationAttributeSetInstance_ID = 0; // sLine.getM_AttributeSetInstance_ID();
                    if (oLine != null)
                        reservationAttributeSetInstance_ID = oLine.GetM_AttributeSetInstance_ID();
                    //
                    if (sLine.GetM_AttributeSetInstance_ID() == 0)
                    {
                        MInOutLineMA[] mas = MInOutLineMA.Get(GetCtx(),
                            sLine.GetM_InOutLine_ID(), Get_TrxName());
                        for (int j = 0; j < mas.Length; j++)
                        {
                            MInOutLineMA ma = mas[j];
                            Decimal QtyMA = ma.GetMovementQty();
                            //if (MovementType.charAt(1) == '-')	//	C- Customer Shipment - V- Vendor Return
                            if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                            {
                                QtyMA = Decimal.Negate(QtyMA);
                            }
                            Decimal QtySOMA = Env.ZERO;
                            Decimal QtyPOMA = Env.ZERO;

                            // nnayak - Don't update qty reserved or qty ordered for Returns
                            if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                            {
                                if (IsSOTrx())
                                    QtySOMA = Decimal.Negate(ma.GetMovementQty());
                                else
                                    QtyPOMA = Decimal.Negate(ma.GetMovementQty());
                            }

                            log.Fine("QtyMA : " + QtyMA + " QtySOMA " + QtySOMA + " QtyPOMA " + QtyPOMA);
                            //	Update Storage - see also VMatch.createMatchRecord
                            if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                            {
                                MOrderLine ordLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
                                MOrder ord = new MOrder(GetCtx(), ordLine.GetC_Order_ID(), Get_TrxName());
                                if (!IsReversal())
                                {
                                    qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                    if (qtytoset >= sLine.GetMovementQty())
                                    {
                                        qtytoset = sLine.GetMovementQty();
                                    }
                                    qtytoset = Decimal.Negate(qtytoset);
                                }
                                else
                                {
                                    if (IsSOTrx())
                                    {
                                        qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                    }
                                    else
                                    {
                                        qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), Decimal.Add(ordLine.GetQtyDelivered(), Decimal.Negate(sLine.GetMovementQty())));
                                    }

                                    if (qtytoset < 0)
                                    {
                                        qtytoset = Decimal.Add(qtytoset, Decimal.Negate(sLine.GetMovementQty()));
                                    }
                                    else
                                    {
                                        qtytoset = Decimal.Negate(sLine.GetMovementQty());
                                    }
                                }
                                if (IsSOTrx())
                                {
                                    QtySO = qtytoset;
                                }
                                else
                                {
                                    QtyPO = qtytoset;
                                }
                                if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                sLine.GetM_Locator_ID(), ord.GetM_Warehouse_ID(),
                                sLine.GetM_Product_ID(),
                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
                                Qty, QtySO, QtyPO, Get_TrxName()))
                                {
                                    _processMsg = "Cannot correct Inventory";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }
                            else
                            {
                                if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                    sLine.GetM_Locator_ID(),
                                    sLine.GetM_Product_ID(),
                                    ma.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
                                    QtyMA, QtySOMA, QtyPOMA, Get_TrxName()))
                                {
                                    _processMsg = "Cannot correct Inventory (MA)";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                            }

                            // Done to Update Current Qty at Transaction

                            MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
                            int attribSet_ID = pro.GetM_AttributeSet_ID();
                            isGetFromStorage = false;
                            if (attribSet_ID > 0)
                            {
                                sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID()
                                        + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                                {
                                    trxQty = GetProductQtyFromTransaction(sLine, GetMovementDate(), true);
                                    isGetFromStorage = true;
                                }
                            }
                            else
                            {
                                sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID()
                                      + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                                {
                                    trxQty = GetProductQtyFromTransaction(sLine, GetMovementDate(), false);
                                    isGetFromStorage = true;
                                }
                            }
                            if (!isGetFromStorage)
                            {
                                trxQty = GetProductQtyFromStorage(sLine);
                            }
                            // Done to Update Current Qty at Transaction

                            //	Create Transaction
                            mtrx = new MTransaction(GetCtx(), sLine.GetAD_Org_ID(),
                                MovementType, sLine.GetM_Locator_ID(),
                                sLine.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                                QtyMA, GetMovementDate(), Get_TrxName());
                            mtrx.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                            mtrx.SetCurrentQty(trxQty + QtyMA);
                            if (!mtrx.Save())
                            {
                                _processMsg = "Could not create Material Transaction (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            UpdateTransaction(sLine, mtrx, Qty + trxQty.Value);
                            //UpdateCurrentRecord(sLine, mtrx, Qty);
                        }
                    }
                    if (mtrx == null)
                    {
                        //	Fallback: Update Storage - see also VMatch.createMatchRecord
                        if (sLine.GetC_OrderLine_ID() != 0 && !IsReturnTrx())
                        {
                            MOrderLine ordLine = new MOrderLine(GetCtx(), sLine.GetC_OrderLine_ID(), Get_TrxName());
                            MOrder ord = new MOrder(GetCtx(), ordLine.GetC_Order_ID(), Get_TrxName());
                            if (!IsReversal())
                            {
                                qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                if (qtytoset >= sLine.GetMovementQty())
                                {
                                    qtytoset = sLine.GetMovementQty();
                                }
                                qtytoset = Decimal.Negate(qtytoset);
                            }
                            else
                            {
                                if (IsSOTrx())
                                {
                                    qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), ordLine.GetQtyDelivered());
                                }
                                else
                                {
                                    qtytoset = Decimal.Subtract(ordLine.GetQtyOrdered(), Decimal.Add(ordLine.GetQtyDelivered(), Decimal.Negate(sLine.GetMovementQty())));
                                }

                                if (qtytoset < 0)
                                {
                                    qtytoset = Decimal.Add(qtytoset, Decimal.Negate(sLine.GetMovementQty()));
                                }
                                else
                                {
                                    qtytoset = Decimal.Negate(sLine.GetMovementQty());
                                }
                            }
                            if (IsSOTrx())
                            {
                                QtySO = qtytoset;
                            }
                            else
                            {
                                QtyPO = qtytoset;
                            }
                            if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                sLine.GetM_Locator_ID(), ord.GetM_Warehouse_ID(),
                                sLine.GetM_Product_ID(),
                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
                                Qty, QtySO, QtyPO, Get_TrxName()))
                            {
                                _processMsg = "Cannot correct Inventory";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                        else
                        {
                            if (!MStorage.Add(GetCtx(), GetM_Warehouse_ID(),
                                sLine.GetM_Locator_ID(),
                                sLine.GetM_Product_ID(),
                                sLine.GetM_AttributeSetInstance_ID(), reservationAttributeSetInstance_ID,
                                Qty, QtySO, QtyPO, Get_TrxName()))
                            {
                                _processMsg = "Cannot correct Inventory";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }

                        // Done to Update Current Qty at Transaction
                        MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
                        int attribSet_ID = pro.GetM_AttributeSet_ID();
                        isGetFromStorage = false;
                        if (attribSet_ID > 0)
                        {
                            sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID()
                                    + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                            {
                                trxQty = GetProductQtyFromTransaction(sLine, GetMovementDate(), true);
                                isGetFromStorage = true;
                            }
                        }
                        else
                        {
                            sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID()
                                       + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                            {
                                trxQty = GetProductQtyFromTransaction(sLine, GetMovementDate(), false);
                                isGetFromStorage = true;
                            }
                        }
                        if (!isGetFromStorage)
                        {
                            trxQty = GetProductQtyFromStorage(sLine);
                        }
                        // Done to Update Current Qty at Transaction


                        //	FallBack: Create Transaction
                        mtrx = new MTransaction(GetCtx(), sLine.GetAD_Org_ID(), MovementType, sLine.GetM_Locator_ID(),
                            sLine.GetM_Product_ID(), sLine.GetM_AttributeSetInstance_ID(),
                            Qty, GetMovementDate(), Get_TrxName());
                        mtrx.SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                        mtrx.SetCurrentQty(trxQty + Qty);
                        if (!mtrx.Save())
                        {
                            _processMsg = "Could not create Material Transaction";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        UpdateTransaction(sLine, mtrx, Qty + trxQty.Value);
                        //UpdateCurrentRecord(sLine, mtrx, Qty);
                    }
                }	//	stock movement

                //	Correct Order Line
                if (product != null && oLine != null && !IsReturnTrx())		//	other in VMatch.createMatchRecord
                {
                    if (MovementType.IndexOf('-') == 1)	//	C- Customer Shipment - V- Vendor Return
                    {

                    }
                    else
                    {
                        qtytoset = Decimal.Negate(qtytoset);
                    }
                    if (IsSOTrx())
                    {
                        oLine.SetQtyReserved(Decimal.Add(oLine.GetQtyReserved(), qtytoset));
                        if (oLine.GetQtyReserved() < 0)
                        {
                            oLine.SetQtyReserved(0);
                        }
                    }
                    else
                    {
                        oLine.SetQtyReserved(Decimal.Subtract(oLine.GetQtyReserved(), qtytoset));
                        if (oLine.GetQtyReserved() < 0)
                        {
                            oLine.SetQtyReserved(0);
                        }
                    }
                }

                //	Update Sales Order Line
                if (oLine != null)
                {
                    if (!IsReturnTrx())
                    {
                        if (IsSOTrx()							//	PO is done by Matching
                                || sLine.GetM_Product_ID() == 0)	//	PO Charges, empty lines
                        {
                            if (IsSOTrx())
                                oLine.SetQtyDelivered(Decimal.Subtract(oLine.GetQtyDelivered(), Qty));
                            else
                                oLine.SetQtyDelivered(Decimal.Add(oLine.GetQtyDelivered(), Qty));
                            oLine.SetDateDelivered(GetMovementDate());	//	overwrite=last
                        }
                    }
                    else // Returns
                    {
                        MOrderLine origOrderLine = new MOrderLine(GetCtx(), oLine.GetOrig_OrderLine_ID(), Get_TrxName());
                        if (IsSOTrx()							//	PO is done by Matching
                                || sLine.GetM_Product_ID() == 0)	//	PO Charges, empty lines
                        {
                            if (IsSOTrx())
                            {
                                oLine.SetQtyDelivered(Decimal.Add(oLine.GetQtyDelivered(), Qty));
                                oLine.SetQtyReturned(Decimal.Add(oLine.GetQtyReturned(), Qty));
                                origOrderLine.SetQtyReturned(Decimal.Add(origOrderLine.GetQtyReturned(), Qty));
                            }
                            else
                            {
                                oLine.SetQtyDelivered(Decimal.Subtract(oLine.GetQtyDelivered(), Qty));
                                oLine.SetQtyReturned(Decimal.Subtract(oLine.GetQtyReturned(), Qty));
                                origOrderLine.SetQtyReturned(Decimal.Subtract(origOrderLine.GetQtyReturned(), Qty));
                            }
                        }

                        oLine.SetDateDelivered(GetMovementDate());	//	overwrite=last

                        if (!origOrderLine.Save())
                        {
                            _processMsg = "Could not update Original Order Line";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        log.Fine("QtyRet " + origOrderLine.GetQtyReturned().ToString() + " Qty : " + Qty.ToString());

                    }
                    if (!oLine.Save())
                    {
                        _processMsg = "Could not update Order Line";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    else
                    {
                        log.Fine("OrderLine -> Reserved=" + oLine.GetQtyReserved().ToString()
                        + ", Delivered=" + oLine.GetQtyDelivered().ToString()
                        + ", Returned=" + oLine.GetQtyReturned().ToString());
                    }
                }

                //	Create Asset for SO
                //if (product != null && IsSOTrx() && product.IsCreateAsset() && sLine.GetMovementQty() > 0
                //    && !IsReversal() && !IsReturnTrx())
                if (product != null && product.IsCreateAsset() && sLine.GetMovementQty() > 0
                   && !IsReversal() && !IsReturnTrx() && sLine.GetA_Asset_ID() == 0)
                {
                    if (!IsSOTrx())
                    {
                        log.Fine("Asset");
                        Info.Append("@A_Asset_ID@: ");
                        int noAssets = (int)sLine.GetMovementQty();

                        //if (!product.IsOneAssetPerUOM())
                        //    noAssets = 1;
                        // Check Added only run when Product is one Asset Per UOM
                        if (product.IsOneAssetPerUOM())
                        {
                            for (int i = 0; i < noAssets; i++)
                            {
                                if (i > 0)
                                    Info.Append(" - ");
                                int deliveryCount = i + 1;
                                if (product.IsOneAssetPerUOM())
                                    deliveryCount = 0;
                                MAsset asset = new MAsset(this, sLine, deliveryCount);

                                if (!asset.Save(Get_TrxName()))
                                {
                                    _processMsg = "Could not create Asset";
                                    return DocActionVariables.STATUS_INVALID;
                                }
                                Info.Append(asset.GetValue());
                            }
                        }
                    }
                    //Create Asset Delivery

                }


                //	Matching
                if (!IsSOTrx()
                    && sLine.GetM_Product_ID() != 0
                    && !IsReversal())
                {
                    Decimal matchQty = sLine.GetMovementQty();
                    //	Invoice - Receipt Match (requires Product)
                    MInvoiceLine iLine = MInvoiceLine.GetOfInOutLine(sLine);

                    if (iLine != null && iLine.GetM_Product_ID() != 0)
                    {
                        if (matchQty.CompareTo(iLine.GetQtyInvoiced()) > 0)
                            matchQty = iLine.GetQtyInvoiced();

                        MMatchInv[] matches = MMatchInv.Get(GetCtx(),
                            sLine.GetM_InOutLine_ID(), iLine.GetC_InvoiceLine_ID(), Get_TrxName());
                        if (matches == null || matches.Length == 0)
                        {
                            MMatchInv inv = new MMatchInv(iLine, GetMovementDate(), matchQty);
                            if (sLine.GetM_AttributeSetInstance_ID() != iLine.GetM_AttributeSetInstance_ID())
                            {
                                iLine.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                                iLine.Save();	//	update matched invoice with ASI
                                inv.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                            }
                            if (!inv.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not create Inv Matching";
                                return DocActionVariables.STATUS_INVALID;
                            }
                        }
                    }

                    //	Link to Order
                    if (sLine.GetC_OrderLine_ID() != 0)
                    {
                        log.Fine("PO Matching");
                        //	Ship - PO
                        MMatchPO po = MMatchPO.Create(null, sLine, GetMovementDate(), matchQty);
                        if (!po.Save(Get_TrxName()))
                        {
                            _processMsg = "Could not create PO Matching";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        //	Update PO with ASI                      Commented by Bharat
                        //if (oLine != null && oLine.GetM_AttributeSetInstance_ID() == 0)
                        //{
                        //    oLine.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                        //    oLine.Save(Get_TrxName());
                        //}
                    }
                    else	//	No Order - Try finding links via Invoice
                    {
                        //	Invoice has an Order Link
                        if (iLine != null && iLine.GetC_OrderLine_ID() != 0)
                        {
                            //	Invoice is created before  Shipment
                            log.Fine("PO(Inv) Matching");
                            //	Ship - Invoice
                            MMatchPO po = MMatchPO.Create(iLine, sLine, GetMovementDate(), matchQty);
                            if (!po.Save(Get_TrxName()))
                            {
                                _processMsg = "Could not create PO(Inv) Matching";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            //	Update PO with ASI                   Commented by Bharat
                            //oLine = new MOrderLine(GetCtx(), po.GetC_OrderLine_ID(), Get_TrxName());
                            //if (oLine != null && oLine.GetM_AttributeSetInstance_ID() == 0)
                            //{
                            //    oLine.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                            //    oLine.Save(Get_TrxName());
                            //}
                        }
                    }	//	No Order
                }	//	PO Matching

            }	//	for all lines

            //	Counter Documents
            MInOut counter = CreateCounterDoc();
            if (counter != null)
                Info.Append(" - @CounterDoc@: @M_InOut_ID@=").Append(counter.GetDocumentNo());
            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }

            _processMsg = Info.ToString();
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        /// <summary>
        /// Update Current Quantity at Transaction Tab
        /// </summary>
        /// <param name="sLine"></param>
        /// <param name="mtrx"></param>
        /// <param name="Qty"></param>
        private void UpdateTransaction(MInOutLine sLine, MTransaction mtrx, decimal Qty)
        {
            MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID();
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID()
                              + " ORDER BY movementdate ASC , m_transaction_id ASC, created ASC";
                }
                else
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + Qty + " WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ";
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id ,  MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(mtrx.GetMovementDate().Value.AddDays(1), true)
                              + " AND M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 "
                              + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }
                //int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                 Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), null);
                                if (!inventory.IsInternalUse())
                                {
                                    //break;
                                    inventoryLine.SetQtyBook(Qty);
                                    inventoryLine.SetOpeningStock(Qty);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                    }

                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(Qty, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                    }
                                    else
                                    {
                                        Qty = trx.GetCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != Qty)
                                        {
                                            storage.SetQtyOnHand(Qty);
                                            storage.Save();
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(Qty + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                            }
                            else
                            {
                                Qty = trx.GetCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != Qty)
                                {
                                    storage.SetQtyOnHand(Qty);
                                    storage.Save();
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }

        private void UpdateCurrentRecord(MInOutLine line, MTransaction trxM, decimal qtyDiffer)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }

                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                }
                else
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }
                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                }

                // int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
            }
            catch
            {
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }
        /// <summary>
        /// Returns the OnHandQuantity of a Product from loacator 
        /// Based on Attribute set bind on Product
        /// </summary>
        /// <param name="sLine"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromStorage(MInOutLine sLine)
        {
            return 0;
            //    MProduct pro = new MProduct(Env.GetCtx(), sLine.GetM_Product_ID(), Get_TrxName());
            //    int attribSet_ID = pro.GetM_AttributeSet_ID();
            //    string sql = "";

            //    if (attribSet_ID > 0)
            //    {
            //        sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID()
            //             + " AND M_AttributeSetInstance_ID = " + sLine.GetM_AttributeSetInstance_ID();
            //    }
            //    else
            //    {
            //        sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + sLine.GetM_Product_ID() + " AND M_Locator_ID = " + sLine.GetM_Locator_ID();
            //    }
            //    return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /// <summary>
        /// Get Latest Current Quantity based on movementdate
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromTransaction(MInOutLine line, DateTime? movementDate, bool isAttribute)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                   AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ")) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id )   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                      AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ")) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            return result;
        }

        /// <summary>
        /// Check Material Policy
        /// </summary>
        /// <param name="line">Sets line ASI</param>
        private void CheckMaterialPolicy(MInOutLine line)
        {
            int no = MInOutLineMA.DeleteInOutLineMA(line.GetM_InOutLine_ID(), Get_TrxName());
            if (no > 0)
            {
                log.Config("Delete old #" + no);
            }

            //	Incoming Trx
            String MovementType = GetMovementType();
            //bool inTrx = MovementType.charAt(1) == '+';	//	V+ Vendor Receipt, C+ Customer Return
            bool inTrx = MovementType.IndexOf('+') == 1;	//	V+ Vendor Receipt, C+ Customer Return
            MClient client = MClient.Get(GetCtx());

            bool needSave = false;
            MProduct product = line.GetProduct();

            //	Need to have Location
            if (product != null
                && line.GetM_Locator_ID() == 0)
            {
                line.SetM_Warehouse_ID(GetM_Warehouse_ID());
                line.SetM_Locator_ID(inTrx ? Env.ZERO : line.GetMovementQty());	//	default Locator
                needSave = true;
            }
            //	Attribute Set Instance
            if (product != null
                && line.GetM_AttributeSetInstance_ID() == 0)
            {
                if (product.GetM_AttributeSet_ID() > 0)
                {
                    if (inTrx)
                    {
                        MAttributeSetInstance asi = new MAttributeSetInstance(GetCtx(), 0, Get_TrxName());
                        asi.SetClientOrg(GetAD_Client_ID(), 0);
                        asi.SetM_AttributeSet_ID(product.GetM_AttributeSet_ID());
                        if (asi.Save())
                        {
                            line.SetM_AttributeSetInstance_ID(asi.GetM_AttributeSetInstance_ID());
                            log.Config("New ASI=" + line);
                            needSave = true;
                        }
                    }
                    else	//	Outgoing Trx
                    {
                        MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
                        String MMPolicy = pc.GetMMPolicy();
                        if (MMPolicy == null || MMPolicy.Length == 0)
                            MMPolicy = client.GetMMPolicy();
                        //
                        MStorage[] storages = MStorage.GetAllWithASI(GetCtx(),
                            line.GetM_Product_ID(), line.GetM_Locator_ID(),
                            MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                        Decimal qtyToDeliver = line.GetMovementQty();
                        for (int ii = 0; ii < storages.Length; ii++)
                        {
                            MStorage storage = storages[ii];
                            if (ii == 0)
                            {
                                if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                                {
                                    line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                                    needSave = true;
                                    log.Config("Direct - " + line);
                                    qtyToDeliver = Env.ZERO;
                                }
                                else
                                {
                                    log.Config("Split - " + line);
                                    MInOutLineMA ma = new MInOutLineMA(line,
                                        storage.GetM_AttributeSetInstance_ID(),
                                        storage.GetQtyOnHand());
                                    if (!ma.Save())
                                    {
                                        ;
                                    }
                                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                                    log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                                }
                            }
                            else	//	 create addl material allocation
                            {
                                MInOutLineMA ma = new MInOutLineMA(line,
                                    storage.GetM_AttributeSetInstance_ID(),
                                    qtyToDeliver);
                                if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                                    qtyToDeliver = Env.ZERO;
                                else
                                {
                                    ma.SetMovementQty(storage.GetQtyOnHand());
                                    //qtyToDeliver = qtyToDeliver.subtract(storage.getQtyOnHand());
                                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                                }
                                if (!ma.Save())
                                {
                                    ;
                                }
                                log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                            }
                            if (qtyToDeliver == 0)
                                break;
                        }	//	 for all storages

                        //	No AttributeSetInstance found for remainder
                        if (qtyToDeliver != 0)
                        {
                            MInOutLineMA ma = new MInOutLineMA(line, 0, qtyToDeliver);
                            if (!ma.Save())
                            {
                                ;
                            }
                            log.Fine("##: " + ma);
                        }
                    }
                }	//	outgoing Trx
            }	//	attributeSetInstance

            if (needSave && !line.Save())
            {
                log.Severe("NOT saved " + line);
            }
        }

        /// <summary>
        /// Create Counter Document
        /// </summary>
        /// <returns>InOut</returns>
        private MInOut CreateCounterDoc()
        {
            //	Is this a counter doc ?
            if (GetRef_InOut_ID() != 0)
                return null;

            //	Org Must be linked to BPartner
            MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
            //jz int counterC_BPartner_ID = org.getLinkedC_BPartner_ID(get_TrxName()); 
            int counterC_BPartner_ID = org.GetLinkedC_BPartner_ID(Get_TrxName());
            if (counterC_BPartner_ID == 0)
                return null;
            //	Business Partner needs to be linked to Org
            //jz MBPartner bp = new MBPartner (getCtx(), getC_BPartner_ID(), null);
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_TrxName());
            int counterAD_Org_ID = bp.GetAD_OrgBP_ID_Int();
            if (counterAD_Org_ID == 0)
                return null;

            //jz MBPartner counterBP = new MBPartner (getCtx(), counterC_BPartner_ID, null);
            MBPartner counterBP = new MBPartner(GetCtx(), counterC_BPartner_ID, Get_TrxName());
            MOrgInfo counterOrgInfo = MOrgInfo.Get(GetCtx(), counterAD_Org_ID, null);
            log.Info("Counter BP=" + counterBP.GetName());

            //	Document Type
            int C_DocTypeTarget_ID = 0;
            bool isReturnTrx = false;
            MDocTypeCounter counterDT = MDocTypeCounter.GetCounterDocType(GetCtx(), GetC_DocType_ID());
            if (counterDT != null)
            {
                log.Fine(counterDT.ToString());
                if (!counterDT.IsCreateCounter() || !counterDT.IsValid())
                    return null;
                C_DocTypeTarget_ID = counterDT.GetCounter_C_DocType_ID();
                isReturnTrx = counterDT.GetCounterDocType().IsReturnTrx();
            }
            else	//	indirect
            {
                C_DocTypeTarget_ID = MDocTypeCounter.GetCounterDocType_ID(GetCtx(), GetC_DocType_ID());
                log.Fine("Indirect C_DocTypeTarget_ID=" + C_DocTypeTarget_ID);
                if (C_DocTypeTarget_ID <= 0)
                    return null;
            }

            //	Deep Copy
            MInOut counter = CopyFrom(this, GetMovementDate(),
                C_DocTypeTarget_ID, !IsSOTrx(), isReturnTrx, true, Get_TrxName(), true);

            //
            counter.SetAD_Org_ID(counterAD_Org_ID);
            counter.SetM_Warehouse_ID(counterOrgInfo.GetM_Warehouse_ID());
            //
            counter.SetBPartner(counterBP);
            //	Refernces (Should not be required
            counter.SetSalesRep_ID(GetSalesRep_ID());
            counter.Save(Get_TrxName());

            string MovementType = counter.GetMovementType();
            //bool inTrx = MovementType.charAt(1) == '+';	//	V+ Vendor Receipt
            bool inTrx = MovementType.IndexOf('+') == 1;	//	V+ Vendor Receipt

            //	Update copied lines
            MInOutLine[] counterLines = counter.GetLines(true);
            for (int i = 0; i < counterLines.Length; i++)
            {
                MInOutLine counterLine = counterLines[i];
                counterLine.SetClientOrg(counter);
                counterLine.SetM_Warehouse_ID(counter.GetM_Warehouse_ID());
                counterLine.SetM_Locator_ID(0);
                counterLine.SetM_Locator_ID(Convert.ToInt32(inTrx ? Env.ZERO : counterLine.GetMovementQty()));
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

        /// <summary>
        /// 	Void Document.
        /// </summary>
        /// <returns>true if success</returns>
        public virtual bool VoidIt()
        {
            log.Info(ToString());

            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
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
                MInOutLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MInOutLine line = lines[i];
                    Decimal old = line.GetMovementQty();
                    if (old != 0)
                    {
                        line.SetQty(Env.ZERO);
                        line.AddDescription("Void (" + old + ")");
                        line.Save(Get_TrxName());
                    }
                }
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Close Document.
        /// </summary>
        /// <returns>true if success</returns>
        public virtual bool CloseIt()
        {
            log.Info(ToString());
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Is Only For Order
        /// </summary>
        /// <param name="order">order</param>
        /// <returns>true if all shipment lines are from this order</returns>
        public bool IsOnlyForOrder(MOrder order)
        {
            //	TODO Compare Lines
            return GetC_Order_ID() == order.GetC_Order_ID();
        }

        /// <summary>
        /// Reverse Correction - same date
        /// </summary>
        /// <param name="order">if not null only for this order</param>
        /// <returns>true if success </returns>
        public bool ReverseCorrectIt(MOrder order)
        {
            log.Info(ToString());
            string ss = ToString();
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

            //	Reverse/Delete Matching
            if (!IsSOTrx())
            {
                MMatchInv[] mInv = MMatchInv.GetInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
                for (int i = 0; i < mInv.Length; i++)
                    mInv[i].Delete(true);
                MMatchPO[] mPO = MMatchPO.GetInOut(GetCtx(), GetM_InOut_ID(), Get_TrxName());
                for (int i = 0; i < mPO.Length; i++)
                {
                    if (mPO[i].GetC_InvoiceLine_ID() == 0)
                        mPO[i].Delete(true);
                    else
                    {
                        mPO[i].SetM_InOutLine_ID(0);
                        mPO[i].Save();
                    }
                }
            }

            //	Deep Copy
            MInOut reversal = CopyFrom(this, GetMovementDate(),
                GetC_DocType_ID(), IsSOTrx(), dt.IsReturnTrx(), false, Get_TrxName(), true);
            if (reversal == null)
            {
                _processMsg = "Could not create Ship Reversal";
                return false;
            }
            reversal.SetReversal(true);

            //	Reverse Line Qty
            MInOutLine[] sLines = GetLines(false);
            MInOutLine[] rLines = reversal.GetLines(false);
            for (int i = 0; i < rLines.Length; i++)
            {
                MInOutLine rLine = rLines[i];
                rLine.SetQtyEntered(Decimal.Negate(rLine.GetQtyEntered()));
                rLine.SetMovementQty(Decimal.Negate(rLine.GetMovementQty()));
                rLine.SetM_AttributeSetInstance_ID(sLines[i].GetM_AttributeSetInstance_ID());

                if (!rLine.Save(Get_TrxName()))
                {
                    _processMsg = "Could not correct Ship Reversal Line";
                    return false;
                }
                //	We need to copy MA
                if (rLine.GetM_AttributeSetInstance_ID() == 0)
                {
                    MInOutLineMA[] mas = MInOutLineMA.Get(GetCtx(), sLines[i].GetM_InOutLine_ID(), Get_TrxName());
                    for (int j = 0; j < mas.Length; j++)
                    {
                        MInOutLineMA ma = new MInOutLineMA(rLine, mas[j].GetM_AttributeSetInstance_ID(), mas[j].GetMovementQty());
                        if (!ma.Save())
                        {
                            ;
                        }
                    }
                }
                //	De-Activate Asset
                MAsset asset = MAsset.GetFromShipment(GetCtx(), sLines[i].GetM_InOutLine_ID(), Get_TrxName());
                if (asset != null)
                {
                    asset.SetIsActive(false);
                    asset.AddDescription("(" + reversal.GetDocumentNo() + " #" + rLine.GetLine() + "<-)");
                    asset.Save();
                }
            }
            reversal.SetC_Order_ID(GetC_Order_ID());
            reversal.AddDescription("{->" + GetDocumentNo() + ")");

            //
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE)
                || !reversal.GetDocStatus().Equals(DocActionVariables.STATUS_COMPLETED))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            reversal.CloseIt();
            reversal.SetProcessing(false);
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save(Get_TrxName());
            //
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");

            _processMsg = reversal.GetDocumentNo();
            SetProcessed(true);
            SetDocStatus(DOCSTATUS_Reversed);		//	 may come from void
            SetDocAction(DOCACTION_None);
            return true;
        }

        /// <summary>
        /// Reverse Correction - same date
        /// </summary>
        /// <returns>true if success </returns>
        public bool ReverseCorrectIt()
        {
            return ReverseCorrectIt(null);
        }

        /// <summary>
        /// Reverse Accrual - none
        /// </summary>
        /// <returns>false</returns>
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /// <summary>
        /// Re-activate
        /// </summary>
        /// <returns>false</returns>
        public virtual bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
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
            sb.Append(":")
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
            return GetSalesRep_ID();
        }

        /// <summary>
        /// Get Document Approval Amount
        /// </summary>
        /// <returns>amount</returns>
        public Decimal GetApprovalAmt()
        {
            return Env.ZERO;
        }

        /// <summary>
        /// Get C_Currency_ID
        /// </summary>
        /// <returns>Accounting Currency</returns>
        public int GetC_Currency_ID()
        {
            return GetCtx().GetContextAsInt("$C_Currency_ID ");
        }

        /// <summary>
        /// Document Status is Complete or Closed
        /// </summary>
        /// <returns>true if CO, CL or RE</returns>
        public bool IsComplete()
        {
            String ds = GetDocStatus();
            return DOCSTATUS_Completed.Equals(ds)
                || DOCSTATUS_Closed.Equals(ds)
                || DOCSTATUS_Reversed.Equals(ds);
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
