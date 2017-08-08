/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_PaymentTerm
 * Chronological Development
 * Veena Pandey     22-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPaymentTerm : X_C_PaymentTerm
    {
        /** 100									*/
        private const Decimal HUNDRED = 100.0M;

        /**	Payment Schedule children			*/
        private MPaySchedule[] _schedule;

        /**
	     * 	Standard Constructor
	     *	@param ctx context
	     *	@param C_PaymentTerm_ID id
	     *	@param trxName transaction
	     */
        public MPaymentTerm(Ctx ctx, int C_PaymentTerm_ID, Trx trxName)
            : base(ctx, C_PaymentTerm_ID, trxName)
        {
            if (C_PaymentTerm_ID == 0)
            {
                SetAfterDelivery(false);
                SetNetDays(0);
                SetDiscount(Env.ZERO);
                SetDiscount2(Env.ZERO);
                SetDiscountDays(0);
                SetDiscountDays2(0);
                SetGraceDays(0);
                SetIsDueFixed(false);
                SetIsValid(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MPaymentTerm(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * 	Get Payment Schedule
	     * 	@param requery if true re-query
	     *	@return array of schedule
	     */
        public MPaySchedule[] GetSchedule(bool requery)
        {
            if (_schedule != null && !requery)
                return _schedule;
            String sql = "SELECT * FROM C_PaySchedule WHERE C_PaymentTerm_ID=" + GetC_PaymentTerm_ID() + " ORDER BY NetDays";
            List<MPaySchedule> list = new List<MPaySchedule>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MPaySchedule ps = new MPaySchedule(GetCtx(), dr, Get_TrxName());
                        ps.SetParent(this);
                        list.Add(ps);
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "GetSchedule", e);
            }

            _schedule = new MPaySchedule[list.Count];
            _schedule = list.ToArray();
            return _schedule;
        }

        /**
         * 	Validate Payment Term & Schedule
         *	@return Validation Message @OK@ or error
         */
        public String Validate()
        {
            GetSchedule(true);
            if (_schedule.Length == 0)
            {
                SetIsValid(true);
                return "@OK@";
            }
            if (_schedule.Length == 1)
            {
                SetIsValid(false);
                return "@Invalid@ @Count@ # = 1 (@C_PaySchedule_ID@)";
            }

            //	Add up
            Decimal total = Env.ZERO;
            for (int i = 0; i < _schedule.Length; i++)
            {
                Decimal percent = _schedule[i].GetPercentage();
               // if (percent != null)
                    total = Decimal.Add(total, percent);
            }
            bool valid = total.CompareTo(HUNDRED) == 0;
            SetIsValid(valid);
            for (int i = 0; i < _schedule.Length; i++)
            {
                if (_schedule[i].IsValid() != valid)
                {
                    _schedule[i].SetIsValid(valid);
                    _schedule[i].Save();
                }
            }
            String msg = "@OK@";
            if (!valid)
                msg = "@Total@ = " + total + " - @Difference@ = " + Decimal.Subtract(HUNDRED, total);
            return Msg.ParseTranslation(GetCtx(), msg);
        }


        /*************************************************************************
         * 	Apply Payment Term to Invoice -
         *	@param C_Invoice_ID invoice
         *	@return true if payment schedule is valid
         */
        public bool Apply(int C_Invoice_ID)
        {
            MInvoice invoice = new MInvoice(GetCtx(), C_Invoice_ID, Get_TrxName());
            if (invoice == null || invoice.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "apply - Not valid C_Invoice_ID=" + C_Invoice_ID);
                return false;
            }
            return Apply(invoice);
        }

        /**
         * 	Apply Payment Term to Invoice
         *	@param invoice invoice
         *	@return true if payment schedule is valid
         */
        public bool Apply(MInvoice invoice)
        {
            if (invoice == null || invoice.Get_ID() == 0)
            {
                log.Log(Level.SEVERE, "No valid invoice - " + invoice);
                return false;
            }

            if (!IsValid())
                return ApplyNoSchedule(invoice);
            //
            GetSchedule(true);
            if (_schedule.Length <= 1)
                return ApplyNoSchedule(invoice);
            else	//	only if valid
                return ApplySchedule(invoice);
        }

        /**
         * 	Apply Payment Term without schedule to Invoice
         *	@param invoice invoice
         *	@return false as no payment schedule
         */
        private bool ApplyNoSchedule(MInvoice invoice)
        {
            DeleteInvoicePaySchedule(invoice.GetC_Invoice_ID(), invoice.Get_TrxName());
            //	updateInvoice
            if (invoice.GetC_PaymentTerm_ID() != GetC_PaymentTerm_ID())
                invoice.SetC_PaymentTerm_ID(GetC_PaymentTerm_ID());
            if (invoice.IsPayScheduleValid())
                invoice.SetIsPayScheduleValid(false);
            return false;
        }

        /**
         * 	Apply Payment Term with schedule to Invoice
         *	@param invoice invoice
         *	@return true if payment schedule is valid
         */
        private bool ApplySchedule(MInvoice invoice)
        {
            DeleteInvoicePaySchedule(invoice.GetC_Invoice_ID(), invoice.Get_TrxName());
            //	Create Schedule
            MInvoicePaySchedule ips = null;
            Decimal remainder = invoice.GetGrandTotal();
            for (int i = 0; i < _schedule.Length; i++)
            {
                ips = new MInvoicePaySchedule(invoice, _schedule[i]);
                ips.Save(invoice.Get_TrxName());
                log.Fine(ips.ToString());
                remainder = Decimal.Subtract(remainder, ips.GetDueAmt());
            }	//	for all schedules
            //	Remainder - update last
            if (remainder.CompareTo(Env.ZERO) != 0 && ips != null)
            {
                ips.SetDueAmt(Decimal.Add(ips.GetDueAmt(), remainder));
                ips.Save(invoice.Get_TrxName());
                log.Fine("Remainder=" + remainder + " - " + ips);
            }

            //	updateInvoice
            if (invoice.GetC_PaymentTerm_ID() != GetC_PaymentTerm_ID())
                invoice.SetC_PaymentTerm_ID(GetC_PaymentTerm_ID());
            return invoice.ValidatePaySchedule();
        }

        /**
         * 	Delete existing Invoice Payment Schedule
         *	@param C_Invoice_ID id
         *	@param trxName transaction
         */
        private void DeleteInvoicePaySchedule(int C_Invoice_ID, Trx trxName)
        {
            String sql = "DELETE FROM C_InvoicePaySchedule WHERE C_Invoice_ID=" + C_Invoice_ID;
            int no = DataBase.DB.ExecuteQuery(sql, null, trxName);
            log.Fine("C_Invoice_ID=" + C_Invoice_ID + " - #" + no);
        }


        /**************************************************************************
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaymentTerm[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append(",Valid=").Append(IsValid())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (IsDueFixed())
            {
                int dd = GetFixMonthDay();
                if (dd < 1 || dd > 31)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@Invalid@ @FixMonthDay@"));
                    return false;
                }
                dd = GetFixMonthCutoff();
                if (dd < 1 || dd > 31)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@Invalid@ @FixMonthCutoff@"));
                    return false;
                }
            }

            if (!newRecord || !IsValid())
                Validate();
            return true;
        }
    }
}
