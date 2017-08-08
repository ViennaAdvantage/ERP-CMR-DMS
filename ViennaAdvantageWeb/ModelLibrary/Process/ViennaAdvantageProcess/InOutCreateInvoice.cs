/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : InOutCreateInvoice
    * Purpose        : Create (Generate) Invoice from Shipment
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Raghunandan     20-Aug-2009
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
using VAdvantage.ProcessEngine;


namespace ViennaAdvantage.Process
{
    public class InOutCreateInvoice : VAdvantage.ProcessEngine.SvrProcess
    {
        //	Shipment					
        private int _M_InOut_ID = 0;
        //Price List Version		
        private int _M_PriceList_ID = 0;
        //Document No					
        private String _InvoiceDocumentNo = null;

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
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
                else if (name.Equals("M_PriceList_ID"))
                {
                    _M_PriceList_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("InvoiceDocumentNo"))
                {
                    _InvoiceDocumentNo = (String)para[i].GetParameter();
                }
                else
                {
                    //log.log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _M_InOut_ID = GetRecord_ID();
        }

        /// <summary>
        /// Create Invoice.
        /// </summary>
        /// <returns>document no</returns>
        protected override String DoIt()
        {
            //log.info("M_InOut_ID=" + _M_InOut_ID 
            //    + ", M_PriceList_ID=" + _M_PriceList_ID
            //    + ", InvoiceDocumentNo=" + _InvoiceDocumentNo);
            if (_M_InOut_ID == 0)
            {
                throw new ArgumentException("No Shipment");
            }
            //
            MInOut ship = new MInOut(GetCtx(), _M_InOut_ID, null);
            if (ship.Get_ID() == 0)
            {
                throw new ArgumentException("Shipment not found");
            }
            if (!MInOut.DOCSTATUS_Completed.Equals(ship.GetDocStatus()))
            {
                throw new ArgumentException("Shipment not completed");
            }

            MInvoice invoice = new MInvoice(ship, null);

            if (ship.IsReturnTrx())
            {
                invoice.SetC_DocTypeTarget_ID(ship.IsSOTrx() ? MDocBaseType.DOCBASETYPE_ARCREDITMEMO : MDocBaseType.DOCBASETYPE_APCREDITMEMO);
            }
            if (_M_PriceList_ID != 0)
            {
                invoice.SetM_PriceList_ID(_M_PriceList_ID);
            }
            if (_InvoiceDocumentNo != null && _InvoiceDocumentNo.Length > 0)
            {
                invoice.SetDocumentNo(_InvoiceDocumentNo);
            }
            if (!invoice.Save())
            {
                throw new ArgumentException("Cannot save Invoice");
            }
            MInOutLine[] shipLines = ship.GetLines(false);
            for (int i = 0; i < shipLines.Length; i++)
            {
                MInOutLine sLine = shipLines[i];
                MInvoiceLine line = new MInvoiceLine(invoice);
                line.SetShipLine(sLine);
                line.SetQtyEntered(sLine.GetQtyEntered());
                line.SetQtyInvoiced(sLine.GetMovementQty());
                if (!line.Save())
                {
                    throw new ArgumentException("Cannot save Invoice Line");
                }
            }
            return invoice.GetDocumentNo();
        }
    }
}
