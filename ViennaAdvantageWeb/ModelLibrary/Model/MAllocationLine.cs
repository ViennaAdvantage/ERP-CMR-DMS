/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_AllocationLine
 * Chronological Development
 * Veena Pandey     23-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAllocationLine : X_C_AllocationLine
    {
        /**	Invoice info			*/
        private MInvoice _invoice = null;
        /** Allocation Header		*/
        private MAllocationHdr _parent = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_AllocationLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAllocationLine(Ctx ctx, int C_AllocationLine_ID, Trx trxName)
            : base(ctx, C_AllocationLine_ID, trxName)
        {
            if (C_AllocationLine_ID == 0)
            {
                //	setC_AllocationHdr_ID (0);
                SetAmount(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetWriteOffAmt(Env.ZERO);
                SetOverUnderAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MAllocationLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MAllocationLine(MAllocationHdr parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetC_AllocationHdr_ID(parent.GetC_AllocationHdr_ID());
            _parent = parent;
            Set_TrxName(parent.Get_TrxName());
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">parent</param>
        /// <param name="amount">amount</param>
        /// <param name="discountAmt">optional discount</param>
        /// <param name="writeOffAmt">optional write off</param>
        /// <param name="overUnderAmt">over/underpayment</param>
        public MAllocationLine(MAllocationHdr parent, Decimal amount,
            Decimal? discountAmt, Decimal? writeOffAmt, Decimal? overUnderAmt)
            : this(parent)
        {
            SetAmount(amount);
            SetDiscountAmt(discountAmt == null ? Env.ZERO : (Decimal)discountAmt);
            SetWriteOffAmt(writeOffAmt == null ? Env.ZERO : (Decimal)writeOffAmt);
            SetOverUnderAmt(overUnderAmt == null ? Env.ZERO : (Decimal)overUnderAmt);
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>parent</returns>
        public MAllocationHdr GetParent()
        {
            if (_parent == null)
                _parent = new MAllocationHdr(GetCtx(), GetC_AllocationHdr_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Parent
        /// </summary>
        /// <param name="parent">parent</param>
        public void SetParent(MAllocationHdr parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Get Parent Trx Date
        /// </summary>
        /// <returns>date trx</returns>
        public new DateTime? GetDateTrx()
        {
            return GetParent().GetDateTrx();
        }

        /// <summary>
        /// Set Document Info
        /// </summary>
        /// <param name="C_BPartner_ID">partner</param>
        /// <param name="C_Order_ID">order</param>
        /// <param name="C_Invoice_ID">invoice</param>
        public void SetDocInfo(int C_BPartner_ID, int C_Order_ID, int C_Invoice_ID)
        {
            SetC_BPartner_ID(C_BPartner_ID);
            SetC_Order_ID(C_Order_ID);
            SetC_Invoice_ID(C_Invoice_ID);
        }

        /// <summary>
        /// Set Payment Info
        /// </summary>
        /// <param name="C_Payment_ID">payment</param>
        /// <param name="C_CashLine_ID">cash line</param>
        public void SetPaymentInfo(int C_Payment_ID, int C_CashLine_ID)
        {
            if (C_Payment_ID != 0)
                SetC_Payment_ID(C_Payment_ID);
            if (C_CashLine_ID != 0)
                SetC_CashLine_ID(C_CashLine_ID);
        }

        /// <summary>
        /// Get Invoice
        /// </summary>
        /// <returns>invoice or null</returns>
        public MInvoice GetInvoice()
        {
            if (_invoice == null && GetC_Invoice_ID() != 0)
                _invoice = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
            return _invoice;
        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true if success</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (!newRecord
                && (Is_ValueChanged("C_BPartner_ID") || Is_ValueChanged("C_Invoice_ID")))
            {
                log.Severe("Cannot Change Business Partner or Invoice");
                return false;
            }

            //	Set BPartner/Order from Invoice
            if (GetC_BPartner_ID() == 0 && GetInvoice() != null)
                SetC_BPartner_ID(GetInvoice().GetC_BPartner_ID());
            if (GetC_Order_ID() == 0 && GetInvoice() != null)
                SetC_Order_ID(GetInvoice().GetC_Order_ID());
            //
            return true;
        }


        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if reversed</returns>
        protected override bool BeforeDelete()
        {
            SetIsActive(false);
            ProcessIt(true);
            return true;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAllocationLine[");
            sb.Append(Get_ID());
            if (GetC_Payment_ID() != 0)
                sb.Append(",C_Payment_ID=").Append(GetC_Payment_ID());
            if (GetC_CashLine_ID() != 0)
                sb.Append(",C_CashLine_ID=").Append(GetC_CashLine_ID());
            if (GetC_Invoice_ID() != 0)
                sb.Append(",C_Invoice_ID=").Append(GetC_Invoice_ID());
            if (GetC_BPartner_ID() != 0)
                sb.Append(",C_BPartner_ID=").Append(GetC_BPartner_ID());
            sb.Append(", Amount=").Append(GetAmount())
                .Append(",Discount=").Append(GetDiscountAmt())
                .Append(",WriteOff=").Append(GetWriteOffAmt())
                .Append(",OverUnder=").Append(GetOverUnderAmt());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Process Allocation (does not update line).
        /// - Update and Link Invoice/Payment/Cash
        /// </summary>
        /// <param name="reverse">reverse if true allocation is reversed</param>
        /// <returns>C_BPartner_ID</returns>
        public int ProcessIt(bool reverse)
        {
            log.Fine ("Reverse=" + reverse + " - " + ToString());
            int C_Invoice_ID = GetC_Invoice_ID();
            MInvoice invoice = GetInvoice();
            if (invoice != null
                && GetC_BPartner_ID() != invoice.GetC_BPartner_ID())
                SetC_BPartner_ID(invoice.GetC_BPartner_ID());
            //
            int C_Payment_ID = GetC_Payment_ID();
            int C_CashLine_ID = GetC_CashLine_ID();

            //	Update Payment
            if (C_Payment_ID != 0)
            {
                MPayment payment = new MPayment(GetCtx(), C_Payment_ID, Get_TrxName());
                if (GetC_BPartner_ID() != payment.GetC_BPartner_ID())
                {
                    log.Warning("C_BPartner_ID different - Invoice=" + GetC_BPartner_ID() + " - Payment=" + payment.GetC_BPartner_ID());
                }
                if (reverse)
                {
                    if (!payment.IsCashTrx())
                    {
                        payment.SetIsAllocated(false);
                        payment.Save();
                    }
                }
                else
                {
                    if (payment.TestAllocation())
                        payment.Save();
                }
            }

            //	Payment - Invoice
            if (C_Payment_ID != 0 && invoice != null)
            {
                //	Link to Invoice
                if (reverse)
                {
                    invoice.SetC_Payment_ID(0);
                    log.Fine("C_Payment_ID=" + C_Payment_ID
                       + " Unlinked from C_Invoice_ID=" + C_Invoice_ID);
                }
                else if (invoice.IsPaid())
                {
                    invoice.SetC_Payment_ID(C_Payment_ID);
                    log.Fine("C_Payment_ID=" + C_Payment_ID
                        + " Linked to C_Invoice_ID=" + C_Invoice_ID);
                }

                //	Link to Order
                String update = "UPDATE C_Order o "
                    + "SET C_Payment_ID="
                        + (reverse ? "NULL " : "(SELECT C_Payment_ID FROM C_Invoice WHERE C_Invoice_ID=" + C_Invoice_ID + ") ")
                    + "WHERE EXISTS (SELECT * FROM C_Invoice i "
                        + "WHERE o.C_Order_ID=i.C_Order_ID AND i.C_Invoice_ID=" + C_Invoice_ID + ")";
                if (DataBase.DB.ExecuteQuery(update, null, Get_TrxName()) > 0)
                {
                    log.Fine("C_Payment_ID=" + C_Payment_ID
                        + (reverse ? " UnLinked from" : " Linked to")
                        + " order of C_Invoice_ID=" + C_Invoice_ID);
                }
            }

            //	Cash - Invoice
            if (C_CashLine_ID != 0 && invoice != null)
            {
                //	Link to Invoice
                if (reverse)
                {
                    invoice.SetC_CashLine_ID(0);
                    log.Fine("C_CashLine_ID=" + C_CashLine_ID
                        + " Unlinked from C_Invoice_ID=" + C_Invoice_ID);
                }
                else
                {
                    invoice.SetC_CashLine_ID(C_CashLine_ID);
                    log.Fine("C_CashLine_ID=" + C_CashLine_ID
                        + " Linked to C_Invoice_ID=" + C_Invoice_ID);
                }

                //	Link to Order
                String update = "UPDATE C_Order o "
                    + "SET C_CashLine_ID="
                        + (reverse ? "NULL " : "(SELECT C_CashLine_ID FROM C_Invoice WHERE C_Invoice_ID=" + C_Invoice_ID + ") ")
                    + "WHERE EXISTS (SELECT * FROM C_Invoice i "
                        + "WHERE o.C_Order_ID=i.C_Order_ID AND i.C_Invoice_ID=" + C_Invoice_ID + ")";
                if (DataBase.DB.ExecuteQuery(update, null, Get_TrxName()) > 0)
                {
                    log.Fine("C_CashLine_ID=" + C_CashLine_ID
                        + (reverse ? " UnLinked from" : " Linked to")
                        + " order of C_Invoice_ID=" + C_Invoice_ID);
                }
            }

            //	Update Balance / Credit used - Counterpart of MInvoice.completeIt
            if (invoice != null)
            {
                if (invoice.TestAllocation() && !invoice.Save())
                {
                    log.Log(Level.SEVERE, "Invoice not updated - " + invoice);
                }
            }

            return GetC_BPartner_ID();
        }

    }
}
