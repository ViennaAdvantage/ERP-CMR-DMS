/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInvoicePaySchedule
 * Purpose        : Invoice payment shedule calculations 
 * Class Used     : X_C_InvoicePaySchedule
 * Chronological    Development
 * Raghunandan     22-Jun-2009
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
    public class MInvoicePaySchedule : X_C_InvoicePaySchedule
    {
        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MInvoicePaySchedule).FullName);
        // 100								
        private static Decimal HUNDRED = 100.0M;
        /**	Parent						*/
        private MInvoice _parent = null;

        /**
         * 	Get Payment Schedule of the invoice
         * 	@param Ctx context
         * 	@param C_Invoice_ID invoice id (direct)
         * 	@param C_InvoicePaySchedule_ID id (indirect)
         *	@param trxName transaction
         *	@return array of schedule
         */
        public static MInvoicePaySchedule[] GetInvoicePaySchedule(Ctx Ctx,
            int C_Invoice_ID, int C_InvoicePaySchedule_ID, Trx trxName)
        {
            String sql = "SELECT * FROM C_InvoicePaySchedule ips ";
            if (C_Invoice_ID != 0)
            {
                sql += "WHERE C_Invoice_ID=" + C_Invoice_ID;
            }
            else
            {
                sql += "WHERE EXISTS (SELECT * FROM C_InvoicePaySchedule xps"
                + " WHERE xps.c_invoicepayschedule_id=" + C_InvoicePaySchedule_ID + " AND ips.C_Invoice_ID=xps.C_Invoice_ID) ";
            }
            sql += "ORDER BY duedate";

            //
            List<MInvoicePaySchedule> list = new List<MInvoicePaySchedule>();
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
                    list.Add(new MInvoicePaySchedule(Ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
               _log.Log(Level.SEVERE, "getInvoicePaySchedule", e); 
            }
            finally
            {
                dt = null;
            }

            MInvoicePaySchedule[] retValue = new MInvoicePaySchedule[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /***
         * 	Standard Constructor
         *	@param Ctx context
         *	@param C_InvoicePaySchedule_ID id
         *	@param trxName transaction
         */
        public MInvoicePaySchedule(Ctx Ctx, int C_InvoicePaySchedule_ID, Trx trxName)
            : base(Ctx, C_InvoicePaySchedule_ID, trxName)
        {
            if (C_InvoicePaySchedule_ID == 0)
            {
                //	setC_Invoice_ID (0);
                //	setDiscountAmt (Env.ZERO);
                //	setDiscountDate (new Datetime(System.currentTimeMillis()));
                //	setDueAmt (Env.ZERO);
                //	setDueDate (new Datetime(System.currentTimeMillis()));
                SetIsValid(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MInvoicePaySchedule(Ctx Ctx, DataRow dr, Trx trxName)
            : base(Ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param invoice invoice
         *	@param paySchedule payment schedule
         */
        public MInvoicePaySchedule(MInvoice invoice, MPaySchedule paySchedule)
            : base(invoice.GetCtx(), 0, invoice.Get_TrxName())
        {

            _parent = invoice;
            SetClientOrg(invoice);
            SetC_Invoice_ID(invoice.GetC_Invoice_ID());
            SetC_PaySchedule_ID(paySchedule.GetC_PaySchedule_ID());

            //	Amounts
            int scale = MCurrency.GetStdPrecision(GetCtx(), invoice.GetC_Currency_ID());
            Decimal due = invoice.GetGrandTotal();
            if (due.CompareTo(Env.ZERO) == 0)
            {
                SetDueAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetIsValid(false);
            }
            else
            {
                //due = due.multiply(paySchedule.getPercentage()).divide(HUNDRED, scale, Decimal.ROUND_HALF_UP);
                due = Decimal.Multiply(due, Decimal.Divide(paySchedule.GetPercentage(),
                    Decimal.Round(HUNDRED, scale, MidpointRounding.AwayFromZero)));
                SetDueAmt(due);
                Decimal discount = Decimal.Multiply(due, Decimal.Divide(paySchedule.GetDiscount(),
                    Decimal.Round(HUNDRED, scale, MidpointRounding.AwayFromZero)));
                SetDiscountAmt(discount);
                SetIsValid(true);
            }

            //	Dates		
            DateTime dueDate = TimeUtil.AddDays(invoice.GetDateInvoiced(), paySchedule.GetNetDays());
            SetDueDate(dueDate);
            DateTime discountDate = TimeUtil.AddDays(invoice.GetDateInvoiced(), paySchedule.GetDiscountDays());
            SetDiscountDate(discountDate);
        }



        /**
         * @return Returns the parent.
         */
        public MInvoice GetParent()
        {
            if (_parent == null)
                _parent = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
            return _parent;
        }

        /**
         * @param parent The parent to set.
         */
        public void SetParent(MInvoice parent)
        {
            _parent = parent;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInvoicePaySchedule[");
            sb.Append(Get_ID()).Append("-Due=" + GetDueDate() + "/" + GetDueAmt())
                .Append(";Discount=").Append(GetDiscountDate() + "/" + GetDiscountAmt())
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
            if (Is_ValueChanged("DueAmt"))
            {
               log.Fine("beforeSave");
                SetIsValid(false);
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
            if (Is_ValueChanged("DueAmt"))
            {
                log.Fine("afterSave");
                GetParent();
                _parent.ValidatePaySchedule();
                _parent.Save();
            }
            return success;
        }

    }
}
