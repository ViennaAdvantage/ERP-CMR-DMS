/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : InvoiceGenerate  
 * Purpose        : Generate Invoices
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan     07-Sep-2009
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
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;

namespace ViennaAdvantage.Process
{
    public class InvoiceGenerate : VAdvantage.ProcessEngine.SvrProcess
    {
        #region Private Variable
        /**	Manual Selection		*/
        private bool _Selection = false;
        /**	Date Invoiced			*/
        private DateTime? _DateInvoiced = null;
        /**	Org						*/
        private int _AD_Org_ID = 0;
        /** BPartner				*/
        private int _C_BPartner_ID = 0;
        /** Order					*/
        private int _C_Order_ID = 0;
        /** Consolidate				*/
        private bool _ConsolidateDocument = true;
        /** Invoice Document Action	*/
        private String _docAction = DocActionVariables.ACTION_COMPLETE;

        /**	The current Invoice	*/
        private MInvoice _invoice = null;
        /**	The current Shipment	*/
        private MInOut _ship = null;
        /** Numner of Invoices		*/
        private int _created = 0;
        /**	Line Number				*/
        private int _line = 0;
        /**	Business Partner		*/
        private MBPartner _bp = null;
        #endregion

        /**
	 *  Prepare - e.g., get Parameters.
	 */
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("Selection"))
                {
                    _Selection = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("DateInvoiced"))
                {
                    _DateInvoiced = (DateTime?)para[i].GetParameter();
                }
                else if (name.Equals("AD_Org_ID"))
                {
                    _AD_Org_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_Order_ID"))
                {
                    _C_Order_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("ConsolidateDocument"))
                {
                    _ConsolidateDocument = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("DocAction"))
                {
                    _docAction = (String)para[i].GetParameter();
                }
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }

            //	Login Date
            if (_DateInvoiced == null)
            {
                //_DateInvoiced = new DateTime(GetCtx().GetContextAsTime("#Date"));
                _DateInvoiced = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime("#Date"));//change at 22-Sep-2009
            }

            //	DocAction check
            if (!DocActionVariables.ACTION_COMPLETE.Equals(_docAction))
            {
                _docAction = DocActionVariables.ACTION_PREPARE;
            }
        }

        /**
         * 	Generate Invoices
         *	@return info
         *	@throws Exception
         */
        protected override String DoIt()
        {
            log.Info("Selection=" + _Selection + ", DateInvoiced=" + _DateInvoiced
                + ", AD_Org_ID=" + _AD_Org_ID + ", C_BPartner_ID=" + _C_BPartner_ID
                + ", C_Order_ID=" + _C_Order_ID + ",  VAdvantage.Process.DocAction=" + _docAction
                + ", Consolidate=" + _ConsolidateDocument);
            //
            String sql = null;
            if (_Selection)	//	VInvoiceGen
            {
                sql = "SELECT * FROM C_Order "
                    + "WHERE IsSelected='Y' AND DocStatus='CO' AND IsSOTrx='Y' "
                    + "ORDER BY M_Warehouse_ID, PriorityRule, C_BPartner_ID, C_PaymentTerm_ID, C_Order_ID";
            }
            else
            {
                sql = "SELECT * FROM C_Order o "
                    + "WHERE DocStatus IN('CO','CL') AND IsSOTrx='Y'";
                if (_AD_Org_ID != 0)
                {
                    sql += " AND AD_Org_ID=" + _AD_Org_ID;
                }
                if (_C_BPartner_ID != 0)
                {
                    sql += " AND C_BPartner_ID=" + _C_BPartner_ID;
                }
                if (_C_Order_ID != 0)
                {
                    sql += " AND C_Order_ID=" + _C_Order_ID;
                }
                //
                sql += " AND EXISTS (SELECT * FROM C_OrderLine ol "
                        + "WHERE o.C_Order_ID=ol.C_Order_ID AND ol.QtyOrdered<>ol.QtyInvoiced AND ol.IsContract ='N') "
                    + "ORDER BY M_Warehouse_ID, PriorityRule, C_BPartner_ID, C_PaymentTerm_ID, C_Order_ID";

                //sql += " AND EXISTS (SELECT * FROM C_OrderLine ol INNER JOIN c_order ord "
                //      + "  ON (ord.c_order_id = ol.c_order_id) WHERE ord.C_Order_ID  =ol.C_Order_ID "
                //     + "  AND ol.QtyOrdered <> ol.QtyInvoiced AND ol.Iscontract ='N') "
                //  + "ORDER BY M_Warehouse_ID, PriorityRule, C_BPartner_ID, C_PaymentTerm_ID, C_Order_ID";
            }
            //	sql += " FOR UPDATE";

            IDataReader idr = null;

            try
            {
                idr = DB.ExecuteReader(sql, null, Get_TrxName());
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            return Generate(idr);
        }


        /**
         * 	Generate Shipments
         * 	@param pstmt order query 
         *	@return info
         */
        private String Generate(IDataReader idr)
        {
            DataTable dt = new DataTable();
            try
            {

                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)//  while (dr.next ())
                {
                    MOrder order = new MOrder(GetCtx(), dr, Get_TrxName());

                    //	New Invoice Location
                    if (!_ConsolidateDocument
                        || (_invoice != null
                        && (_invoice.GetC_BPartner_Location_ID() != order.GetBill_Location_ID()
                            || _invoice.GetC_PaymentTerm_ID() != order.GetC_PaymentTerm_ID())))
                    {
                        CompleteInvoice();
                    }
                    bool completeOrder = MOrder.INVOICERULE_AfterOrderDelivered.Equals(order.GetInvoiceRule());

                    //	Schedule After Delivery
                    bool doInvoice = false;
                    if (MOrder.INVOICERULE_CustomerScheduleAfterDelivery.Equals(order.GetInvoiceRule()))
                    {
                        _bp = new MBPartner(GetCtx(), order.GetBill_BPartner_ID(), null);
                        if (_bp.GetC_InvoiceSchedule_ID() == 0)
                        {
                            log.Warning("BPartner has no Schedule - set to After Delivery");
                            order.SetInvoiceRule(MOrder.INVOICERULE_AfterDelivery);
                            order.Save();
                        }
                        else
                        {
                            MInvoiceSchedule ins = MInvoiceSchedule.Get(GetCtx(), _bp.GetC_InvoiceSchedule_ID(), Get_TrxName());
                            if (ins.CanInvoice(order.GetDateOrdered(), order.GetGrandTotal()))
                            {
                                doInvoice = true;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }	//	Schedule

                    //	After Delivery
                    if (doInvoice || MOrder.INVOICERULE_AfterDelivery.Equals(order.GetInvoiceRule()))
                    {
                        MInOut shipment = null;
                        MInOutLine[] shipmentLines = order.GetShipmentLines();
                        for (int i = 0; i < shipmentLines.Length; i++)
                        {
                            MInOutLine shipLine = shipmentLines[i];
                            if (shipLine.IsInvoiced())
                            {
                                continue;
                            }
                            if (shipment == null
                                || shipment.GetM_InOut_ID() != shipLine.GetM_InOut_ID())
                            {
                                shipment = new MInOut(GetCtx(), shipLine.GetM_InOut_ID(), Get_TrxName());
                            }
                            if (!shipment.IsComplete()		//	ignore incomplete or reversals 
                                || shipment.GetDocStatus().Equals(MInOut.DOCSTATUS_Reversed))
                            {
                                continue;
                            }
                            //
                            CreateLine(order, shipment, shipLine);
                        }	//	shipment lines
                        _line += 1000;
                    }
                    //	After Order Delivered, Immediate
                    else
                    {
                        MOrderLine[] oLines = order.GetLines(true, null);
                        for (int i = 0; i < oLines.Length; i++)
                        {
                            MOrderLine oLine = oLines[i];
                            Decimal toInvoice = Decimal.Subtract(oLine.GetQtyOrdered(), oLine.GetQtyInvoiced());
                            if (toInvoice.CompareTo(Env.ZERO) == 0 && oLine.GetM_Product_ID() != 0)
                            {
                                continue;
                            }
                            //
                            bool fullyDelivered = oLine.GetQtyOrdered().CompareTo(oLine.GetQtyDelivered()) == 0;

                            //	Complete Order
                            if (completeOrder && !fullyDelivered)
                            {
                                log.Fine("Failed CompleteOrder - " + oLine);
                                completeOrder = false;
                                break;
                            }
                            //	Immediate
                            else if (MOrder.INVOICERULE_Immediate.Equals(order.GetInvoiceRule()))
                            {
                                log.Fine("Immediate - ToInvoice=" + toInvoice + " - " + oLine);
                                Decimal qtyEntered = toInvoice;
                                //	Correct UOM for QtyEntered
                                if (oLine.GetQtyEntered().CompareTo(oLine.GetQtyOrdered()) != 0)
                                {
                                    qtyEntered = Decimal.Round(Decimal.Divide(Decimal.Multiply(
                                        toInvoice, oLine.GetQtyEntered()),
                                        oLine.GetQtyOrdered()), 12, MidpointRounding.AwayFromZero);
                                }
                                //
                                if (oLine.IsContract() == false)
                                {
                                    CreateLine(order, oLine, toInvoice, qtyEntered);
                                    log.Info("ID " + oLine.Get_ID() + "Qty Ordered " + oLine.GetQtyOrdered() + " Qty Invoiced " + oLine.GetQtyInvoiced());
                                }

                            }
                            else
                            {
                                log.Fine("Failed: " + order.GetInvoiceRule()
                                    + " - ToInvoice=" + toInvoice + " - " + oLine);
                            }
                        }	//	for all order lines
                        if (MOrder.INVOICERULE_Immediate.Equals(order.GetInvoiceRule()))
                            _line += 1000;
                    }

                    //	Complete Order successful
                    if (completeOrder && MOrder.INVOICERULE_AfterOrderDelivered.Equals(order.GetInvoiceRule()))
                    {
                        MInOut[] shipments = order.GetShipments(true);
                        for (int i = 0; i < shipments.Length; i++)
                        {
                            MInOut ship = shipments[i];
                            if (!ship.IsComplete()		//	ignore incomplete or reversals 
                                || ship.GetDocStatus().Equals(MInOut.DOCSTATUS_Reversed))
                            {
                                continue;
                            }
                            MInOutLine[] shipLines = ship.GetLines(false);
                            for (int j = 0; j < shipLines.Length; j++)
                            {
                                MInOutLine shipLine = shipLines[j];
                                if (!order.IsOrderLine(shipLine.GetC_OrderLine_ID()))
                                {
                                    continue;
                                }
                                if (!shipLine.IsInvoiced())
                                {
                                    CreateLine(order, ship, shipLine);
                                }
                            }	//	lines
                            _line += 1000;
                        }	//	all shipments
                    }	//	complete Order

                }	//	for all orders
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }

                dt = null;
            }

            CompleteInvoice();
            return "@Created@ = " + _created;
        }



        /**************************************************************************
         * 	Create Invoice Line from Order Line
         *	@param order order
         *	@param orderLine line
         *	@param qtyInvoiced qty
         *	@param qtyEntered qty
         */
        private void CreateLine(MOrder order, MOrderLine orderLine,
            Decimal qtyInvoiced, Decimal qtyEntered)
        {
            if (_invoice == null)
            {
                _invoice = new MInvoice(order, 0, _DateInvoiced);
                if (!_invoice.Save())
                {
                    throw new Exception("Could not create Invoice (o)");
                }
            }
            //	
            MInvoiceLine line = new MInvoiceLine(_invoice);
            line.SetOrderLine(orderLine);
            line.SetQtyInvoiced(qtyInvoiced);

            log.Info("Qty Invoiced" + line.GetQtyInvoiced());
            line.SetQtyEntered(qtyEntered);
            line.SetLine(_line + orderLine.GetLine());
            if (!line.Save())
            {
                throw new Exception("Could not create Invoice Line (o)");
            }
            log.Fine(line.ToString());
        }

        /**
         * 	Create Invoice Line from Shipment
         *	@param order order
         *	@param ship shipment header
         *	@param sLine shipment line
         */
        private void CreateLine(MOrder order, MInOut ship, MInOutLine sLine)
        {
            if (_invoice == null)
            {
                _invoice = new MInvoice(order, 0, _DateInvoiced);
                if (!_invoice.Save())
                {
                    throw new Exception("Could not create Invoice (s)");
                }
            }
            #region Comment Create Shipment Comment Line
            //	Create Shipment Comment Line
            //if (_ship == null
            //    || _ship.GetM_InOut_ID() != ship.GetM_InOut_ID())
            //{
            //    MDocType dt = MDocType.Get(GetCtx(), ship.GetC_DocType_ID());
            //    if (_bp == null || _bp.GetC_BPartner_ID() != ship.GetC_BPartner_ID())
            //    {
            //        _bp = new MBPartner(GetCtx(), ship.GetC_BPartner_ID(), Get_TrxName());
            //    }

            //    //	Reference: Delivery: 12345 - 12.12.12
            //    MClient client = MClient.Get(GetCtx(), order.GetAD_Client_ID());
            //    String AD_Language = client.GetAD_Language();
            //    if (client.IsMultiLingualDocument() && _bp.GetAD_Language() != null)
            //    {
            //        AD_Language = _bp.GetAD_Language();
            //    }
            //    if (AD_Language == null)
            //    {
            //        // MessageBox.Show("Set base Language");
            //        //AD_Language = Language.getBaseAD_Language();
            //    }
            //    //java.text.SimpleDateFormat format = DisplayType.getDateFormat
            //    //    (DisplayType.Date, Language.getLanguage(AD_Language));

            //    //String reference = dt.GetPrintName(_bp.GetAD_Language())
            //    //    + ": " + ship.GetDocumentNo()
            //    //    + " - " + format.format(ship.GetMovementDate());
            //    String reference = dt.GetPrintName(_bp.GetAD_Language())
            //        + ": " + ship.GetDocumentNo()
            //        + " - " + ship.GetMovementDate();
            //    _ship = ship;
            //    //
            //    MInvoiceLine line = new MInvoiceLine(_invoice);
            //    line.SetIsDescription(true);
            //    line.SetDescription(reference);
            //    line.SetLine(_line + sLine.GetLine() - 2);
            //    if (!line.Save())
            //    {
            //        throw new Exception("Could not create Invoice Comment Line (sh)");
            //    }
            //    //	Optional Ship Address if not Bill Address
            //    if (order.GetBill_Location_ID() != ship.GetC_BPartner_Location_ID())
            //    {
            //        MLocation addr = MLocation.GetBPLocation(GetCtx(), ship.GetC_BPartner_Location_ID(), null);
            //        line = new MInvoiceLine(_invoice);
            //        line.SetIsDescription(true);
            //        line.SetDescription(addr.ToString());
            //        line.SetLine(_line + sLine.GetLine() - 1);
            //        if (!line.Save())
            //        {
            //            throw new Exception("Could not create Invoice Comment Line 2 (sh)");
            //        }
            //    }
            //}
            #endregion
            //	
            MInvoiceLine line1 = new MInvoiceLine(_invoice);
            line1.SetShipLine(sLine);
            line1.SetQtyEntered(sLine.GetQtyEntered());
            line1.SetQtyInvoiced(sLine.GetMovementQty());
            line1.SetLine(_line + sLine.GetLine());
            line1.SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());

            if (!line1.Save())
            {
                throw new Exception("Could not create Invoice Line (s)");
            }
            //	Link
            sLine.SetIsInvoiced(true);
            if (!sLine.Save())
            {
                throw new Exception("Could not update Shipment Line");
            }

            log.Fine(line1.ToString());
        }

        /**
         * 	Complete Invoice
         */
        private void CompleteInvoice()
        {
            if (_invoice != null)
            {
                if (!_invoice.ProcessIt(_docAction))
                {
                    log.Warning("completeInvoice - failed: " + _invoice);
                }
                _invoice.Save();
                //
                AddLog(_invoice.GetC_Invoice_ID(), Convert.ToDateTime(_invoice.GetDateInvoiced()), null, _invoice.GetDocumentNo());
                _created++;
            }
            _invoice = null;
            _ship = null;
            _line = 0;
        }

    }
}
