using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class PaymentAllocation
    {
        Ctx ctx = new Ctx();
        public PaymentAllocation(Ctx ctx)
        {
            this.ctx = ctx;
        }

        public string SaveCashData(List<Dictionary<string, string>> paymentData, List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsInvoice, string currency,
            bool isCash, int _C_BPartner_ID, int _windowNo, string payment, DateTime DateTrx, string applied, string discount, string writeOff, string open)
        {
            //if (_noInvoices + _noCashLines == 0)
            //    return "";
            int C_Currency_ID = Convert.ToInt32(currency);
            //  fixed fields
            int AD_Client_ID = ctx.GetContextAsInt(_windowNo, "AD_Client_ID");
            int AD_Org_ID = ctx.GetContextAsInt(_windowNo, "AD_Org_ID");
            int C_BPartner_ID = _C_BPartner_ID;
            int C_Order_ID = 0;
            int C_CashLine_ID = 0;

            //
            if (AD_Org_ID == 0)
            {
                //Classes.ShowMessage.Error("Org0NotAllowed", null);
                return "";
            }
            //
            //  log.Config("Client=" + AD_Client_ID + ", Org=" + AD_Org_ID
            //    + ", BPartner=" + C_BPartner_ID + ", Date=" + DateTrx);

            Trx trx = Trx.Get(Trx.CreateTrxName("AL"), true);

            /**
             * Generation of allocations:               amount/discount/writeOff
             *  - if there is one payment -- one line per invoice is generated
             *    with both the Invoice and Payment reference
             *      Pay=80  Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *    or
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#2
             *
             *  - if there are multiple payment lines -- the amounts are allocated
             *    starting with the first payment and payment
             *      Pay=60  Inv=100 Disc=10 WOff=10 =>  60/10/10    Pay#1   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  20/0/0      Pay#2   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#2   Inv#2
             *
             *  - if you apply a credit memo to an invoice
             *              Inv=10  Disc=0  WOff=0  =>  10/0/0              Inv#1
             *              Inv=-10 Disc=0  WOff=0  =>  -10/0/0             Inv#2
             *
             *  - if you want to write off a (partial) invoice without applying,
             *    enter zero in applied
             *              Inv=10  Disc=1  WOff=9  =>  0/1/9               Inv#1
             *  Issues
             *  - you cannot write-off a payment
             */

            //  CashLines - Loop and Add them to cashList/CashAmountList
            #region CashLines-Loop
            // int cRows = vdgvCashLines.RowCount;
            // IList rowsCash = vdgvCashLine.ItemsSource as IList;

            List<int> cashList = new List<int>(rowsCash.Count);
            List<Decimal> CashAmtList = new List<Decimal>(rowsCash.Count);
            Decimal cashAppliedAmt = Env.ZERO;
            for (int i = 0; i < rowsCash.Count; i++)
            {
                //  Payment line is selected
                bool boolValue = false;
                bool flag = false;
                // if (boolValue)
                {
                    //  Payment variables
                    C_CashLine_ID = Util.GetValueOfInt(rowsCash[i]["ccashlineid"]);
                    cashList.Add(C_CashLine_ID);
                    //
                    //Decimal PaymentAmt = Util.GetValueOfDecimal(((BindableObject)rowsCash[i]).GetValue(_payment));  //  Applied Payment


                    Decimal PaymentAmt = Util.GetValueOfDecimal(rowsCash[i][payment]);  //  Applied Payment

                    CashAmtList.Add(PaymentAmt);
                    //
                    cashAppliedAmt = Decimal.Add(cashAppliedAmt, PaymentAmt);
                    //
                    // log.Fine("C_CashLine_ID=" + C_CashLine_ID
                    //  + " - PaymentAmt=" + PaymentAmt); // + " * " + Multiplier + " = " + PaymentAmtAbs);
                }
            }
            //log.Config("Number of Cashlines=" + cashList.Count + " - Total=" + cashAppliedAmt);
            #endregion

            //  Invoices - Loop and generate alloctions
            #region Invoice-Loop with allocation
            // int iRows = vdgvInvoice.RowCount;
            //  IList rowsInvoice = vdgvInvoice.ItemsSource as IList;
            Decimal totalAppliedAmt = Env.ZERO;

            //	Create Allocation - but don't save yet
            MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
                DateTrx, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
            alloc.SetAD_Org_ID(AD_Org_ID);

            //	For all invoices
            int invoiceLines = 0;
            //for (int i = 0; i < rowsCash.Count; i++)
            for (int i = 0; i < rowsInvoice.Count; i++)
            {
                //  Invoice line is selected
                bool boolValue = false;
                bool flag = false;
                // if (boolValue)
                {
                    invoiceLines++;
                    //  Invoice variables
                    /// int C_Invoice_ID = Util.GetValueOfInt(((BindableObject)rowsInvoice[i]).GetValue("C_INVOICE_ID"));

                    int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);

                    Decimal AppliedAmt = Util.GetValueOfDecimal(rowsInvoice[i][applied]);
                    //  semi-fixed fields (reset after first invoice)
                    Decimal DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i][discount]);
                    Decimal WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i][writeOff]);
                    //	OverUnderAmt needs to be in Allocation Currency
                    Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(rowsInvoice[i][open]),
                        Decimal.Subtract(AppliedAmt, Decimal.Subtract(DiscountAmt, WriteOffAmt)));

                    //log.Config("Invoice #" + i + " - AppliedAmt=" + AppliedAmt);// + " -> " + AppliedAbs);

                    //CashLines settelment************
                    //  loop through all payments until invoice applied
                    int noCashlines = 0;
                    for (int j = 0; j < cashList.Count && Env.Signum(AppliedAmt) != 0; j++)
                    {
                        C_CashLine_ID = Util.GetValueOfInt(cashList[j]);
                        Decimal PaymentAmt = Util.GetValueOfDecimal(CashAmtList[j]);
                        if (Env.Signum(PaymentAmt) != 0)
                        {
                            //log.Config(".. with payment #" + j + ", Amt=" + PaymentAmt);
                            noCashlines++;
                            //  use Invoice Applied Amt
                            Decimal amount = AppliedAmt;
                            //log.Fine("C_CashLine_ID=" + C_CashLine_ID + ", C_Invoice_ID=" + C_Invoice_ID
                            //    + ", Amount=" + amount + ", Discount=" + DiscountAmt + ", WriteOff=" + WriteOffAmt);

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                //  log.Log(Level.SEVERE, "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return "";
                            }
                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, amount,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(0, C_CashLine_ID);//payment for payment allocation is zero
                            if (!aLine.Save())
                                //  log.Log(Level.SEVERE, "Allocation Line not written - Invoice=" + C_Invoice_ID);

                                //  Apply Discounts and WriteOff only first time
                                DiscountAmt = Env.ZERO;
                            WriteOffAmt = Env.ZERO;
                            //  subtract amount from Payment/Invoice
                            AppliedAmt = Decimal.Subtract(AppliedAmt, amount);
                            //AppliedAmt = Decimal.Subtract(PaymentAmt, AppliedAmt);
                            PaymentAmt = Decimal.Subtract(PaymentAmt, amount);
                            //log.Fine("Allocation Amount=" + amount + " - Remaining  Applied=" + AppliedAmt + ", Payment=" + PaymentAmt);

                            //amountList.set(j, PaymentAmt);  //  update
                            if (CashAmtList.Count > 0)
                            {
                                MCashLine cline = new MCashLine(ctx, C_CashLine_ID, null);
                                cline.SetAmount(Decimal.Subtract(cline.GetAmount(), CashAmtList[j]));
                                if (!cline.Save())
                                {
                                    // log.SaveError("AmountIsNotUpdated" + C_CashLine_ID.ToString(), "");
                                }
                                CashAmtList[j] = PaymentAmt;  //  update//set
                            }

                        }	//	for all applied amounts
                    }	//	loop through Cash for invoice(Charge)

                    //  No Cashlines allocated and none existing 
                    if (rowsCash.Count > 0)
                        if (noCashlines == 0 && cashList.Count == 0)
                        {
                            C_CashLine_ID = 0;
                            //log.Config(" ... no CashLines - TotalApplied=" + totalAppliedAmt);
                            //  Create Allocation
                            // log.Fine("C_CashLine_ID=" + C_CashLine_ID + ", C_Invoice_ID=" + C_Invoice_ID
                            //     + ", Amount=" + AppliedAmt + ", Discount=" + DiscountAmt + ", WriteOff=" + WriteOffAmt);

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                //log.Log(Level.SEVERE, "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return "";
                            }
                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, AppliedAmt,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            aLine.SetPaymentInfo(0, C_CashLine_ID);
                            if (!aLine.Save(trx))
                            { }
                            // log.Log(Level.SEVERE, "Allocation Line not written - Invoice=" + C_Invoice_ID);

                            //log.Fine("Allocation Amount=" + AppliedAmt);
                        }
                    totalAppliedAmt = Decimal.Add(totalAppliedAmt, AppliedAmt);
                    //log.Config("TotalRemaining=" + totalAppliedAmt);
                }   //  invoice selected
            }   //  invoice loop

            #endregion

            if (Env.Signum(totalAppliedAmt) != 0)
                //log.Log(Level.SEVERE, "Remaining TotalAppliedAmt=" + totalAppliedAmt);

                //	Should start WF
                if (alloc.Get_ID() != 0)
                {
                    alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                    alloc.Save();
                }

            //  Test/Set IsPaid for Invoice - requires that allocation is posted
            #region Set Invoice IsPaid
            for (int i = 0; i < rowsInvoice.Count; i++)
            {
                bool boolValue = false;
                //  Invoice line is selected
                bool flag = false;
                //Dispatcher.BeginInvoke(delegate
                //{
                //    boolValue = GetBoolValue(vdgvInvoice, i, 0);
                //    flag = true;
                //    SetBusy(false);
                //});
                //while (!flag)
                //{
                //    System.Threading.Thread.Sleep(1);
                //}
                // if (boolValue)
                {
                    //KeyNamePair pp = (KeyNamePair)vdgvInvoice.Rows[i].Cells[2].Value;    //  Value
                    //KeyNamePair pp = (KeyNamePair)((BindableObject)rowsInvoice[i]).GetValue(2);    //  Value
                    //  Invoice variables
                    int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                    String sql = "SELECT invoiceOpen(C_Invoice_ID, 0) "
                        + "FROM C_Invoice WHERE C_Invoice_ID=@param1";
                    Decimal opens = Util.GetValueOfDecimal(DB.GetSQLValueBD(trx, sql, C_Invoice_ID));
                    if (open != null && Env.Signum(opens) == 0)
                    {
                        sql = "UPDATE C_Invoice SET IsPaid='Y' "
                            + "WHERE C_Invoice_ID=" + C_Invoice_ID;
                        int no = DB.ExecuteQuery(sql, null, trx);
                        // log.Config("Invoice #" + i + " is paid");
                    }
                    else
                    {
                        // log.Config("Invoice #" + i + " is not paid - " + open);
                    }
                }
            }
            #endregion

            //  Test/Set CashLine is fully allocated
            #region Set CashLine Allocated
            if (rowsCash.Count > 0)
                for (int i = 0; i < cashList.Count; i++)
                {
                    C_CashLine_ID = Util.GetValueOfInt(cashList[i]);
                    MCashLine cash = new MCashLine(ctx, C_CashLine_ID, trx);
                    if (cash.GetAmount() == 0)
                    {
                        cash.SetIsAllocated(true);
                        cash.Save();
                    }
                    // log.Config("Cash #" + i + (cash.IsAllocated() ? " not" : " is")
                    //   + " fully allocated");
                }
            #endregion

            cashList.Clear();
            CashAmtList.Clear();
            trx.Commit();
            trx.Close();
            return "";
        }


        /// <summary>
        /// Save Data
        /// </summary>

        public void SavePaymentData(List<Dictionary<string, string>> rowsPayment, List<Dictionary<string, string>> rowsCash, List<Dictionary<string, string>> rowsInvoice, string currency,
            bool isCash, int _C_BPartner_ID, int _windowNo, string payment, DateTime DateTrx, string applied, string discount, string writeOff, string open)
        {

            //  fixed fields
            int AD_Client_ID = ctx.GetContextAsInt(_windowNo, "AD_Client_ID");
            int AD_Org_ID = ctx.GetContextAsInt(_windowNo, "AD_Org_ID");
            int C_BPartner_ID = _C_BPartner_ID;
            int C_Order_ID = 0;
            int C_CashLine_ID = 0;
            //DateTime? DateTrx = Util.GetValueOfDateTime(vdtpDateField.GetValue());
            int C_Currency_ID = Convert.ToInt32(currency);
            //
            if (AD_Org_ID == 0)
            {
                //Classes.ShowMessage.Error("Org0NotAllowed", null);
                return;
            }
            //
            // log.Config("Client=" + AD_Client_ID + ", Org=" + AD_Org_ID
            //     + ", BPartner=" + C_BPartner_ID + ", Date=" + DateTrx);

            Trx trx = Trx.Get(Trx.CreateTrxName("AL"), true);

            /**
             * Generation of allocations:               amount/discount/writeOff
             *  - if there is one payment -- one line per invoice is generated
             *    with both the Invoice and Payment reference
             *      Pay=80  Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *    or
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#1
             *      Pay=160 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#1   Inv#2
             *
             *  - if there are multiple payment lines -- the amounts are allocated
             *    starting with the first payment and payment
             *      Pay=60  Inv=100 Disc=10 WOff=10 =>  60/10/10    Pay#1   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  20/0/0      Pay#2   Inv#1
             *      Pay=100 Inv=100 Disc=10 WOff=10 =>  80/10/10    Pay#2   Inv#2
             *
             *  - if you apply a credit memo to an invoice
             *              Inv=10  Disc=0  WOff=0  =>  10/0/0              Inv#1
             *              Inv=-10 Disc=0  WOff=0  =>  -10/0/0             Inv#2
             *
             *  - if you want to write off a (partial) invoice without applying,
             *    enter zero in applied
             *              Inv=10  Disc=1  WOff=9  =>  0/1/9               Inv#1
             *  Issues
             *  - you cannot write-off a payment
             */


            //  Payment - Loop and Add them to paymentList/amountList

            try
            {
                #region Payment-Loop
                //int pRows = vdgvPayment.ItemsSource.OfType<object>().Count();
                //  IList rowsPayment = vdgvPayment.ItemsSource as IList;

                List<int> paymentList = new List<int>(rowsPayment.Count);
                List<Decimal> amountList = new List<Decimal>(rowsPayment.Count);
                Decimal paymentAppliedAmt = Env.ZERO;
                for (int i = 0; i < rowsPayment.Count; i++)
                {
                    //  Payment line is selected
                    // bool boolValue = false;
                    //if (boolValue)
                    {
                        //KeyNamePair pp = (KeyNamePair)vdgvPayment.Rows[i].Cells[2].Value;
                        // KeyNamePair pp = (KeyNamePair)(((BindableObject)rowsPayment[i]).GetValue(2));   //  Value
                        //  Payment variables
                        int C_Payment_ID = Util.GetValueOfInt(rowsPayment[i]["cpaymentid"]);
                        paymentList.Add(C_Payment_ID);
                        //
                        // Decimal PaymentAmt = Util.GetValueOfDecimal(vdgvPayment.Rows[i].Cells[_payment].Value);  //  Applied Payment
                        Decimal PaymentAmt = Util.GetValueOfDecimal(rowsPayment[i][payment]);  //  Applied Payment
                        amountList.Add(PaymentAmt);
                        //
                        paymentAppliedAmt = Decimal.Add(paymentAppliedAmt, PaymentAmt);
                        //
                        //   log.Fine("C_Payment_ID=" + C_Payment_ID
                        //       + " - PaymentAmt=" + PaymentAmt); // + " * " + Multiplier + " = " + PaymentAmtAbs);
                        MPayment pay1 = new MPayment(ctx, C_Payment_ID, trx);
                    }
                }
                //log.Config("Number of Payments=" + paymentList.Count + " - Total=" + paymentAppliedAmt);
                #endregion

                //  Invoices - Loop and generate alloctions
                #region Invoice-Loop with allocation
                // int iRows = vdgvInvoice.ItemsSource.OfType<object>().Count();
                //  IList rowsInvoice=vdgvInvoice.ItemsSource as IList;

                Decimal totalAppliedAmt = Env.ZERO;

                //	Create Allocation - but don't save yet
                MAllocationHdr alloc = new MAllocationHdr(ctx, true,	//	manual
                    DateTrx, C_Currency_ID, ctx.GetContext("#AD_User_Name"), trx);
                alloc.SetAD_Org_ID(AD_Org_ID);


                //	For all invoices
                int invoiceLines = 0;
                for (int i = 0; i < rowsInvoice.Count; i++)
                {
                    //  Invoice line is selected

                    //if (boolValue)
                    {
                        invoiceLines++;
                        //  KeyNamePair pp = (KeyNamePair)vdgvInvoice.Rows[i].Cells[2].Value;    //  Value
                        // KeyNamePair pp = (KeyNamePair)(((BindableObject)rowsPayment[i]).GetValue(2));      //  Value
                        //  Invoice variables
                        int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                        //  Decimal AppliedAmt = Util.GetValueOfDecimal(vdgvInvoice.Rows[i].Cells[_applied].Value);

                        Decimal AppliedAmt = Util.GetValueOfDecimal(rowsInvoice[i][applied]);
                        //  semi-fixed fields (reset after first invoice)
                        //     Decimal DiscountAmt = Util.GetValueOfDecimal(vdgvInvoice.Rows[i].Cells[_discount].Value);
                        //    Decimal WriteOffAmt = Util.GetValueOfDecimal(vdgvInvoice.Rows[i].Cells[_writeOff].Value);

                        Decimal DiscountAmt = Util.GetValueOfDecimal(rowsInvoice[i][discount]);
                        Decimal WriteOffAmt = Util.GetValueOfDecimal(rowsInvoice[i][writeOff]);

                        //	OverUnderAmt needs to be in Allocation Currency
                        //     Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(vdgvInvoice.Rows[i].Cells[_open].Value),
                        //        Decimal.Subtract(AppliedAmt, Decimal.Subtract(DiscountAmt, WriteOffAmt)));

                        Decimal OverUnderAmt = Decimal.Subtract(Util.GetValueOfDecimal(rowsInvoice[i][open]),
                        Decimal.Subtract(AppliedAmt, Decimal.Subtract(DiscountAmt, WriteOffAmt)));

                        // log.Config("Invoice #" + i + " - AppliedAmt=" + AppliedAmt);// + " -> " + AppliedAbs);

                        //Payment Settelment**********
                        //  loop through all payments until invoice applied
                        int noPayments = 0;
                        for (int j = 0; j < paymentList.Count && Env.Signum(AppliedAmt) != 0; j++)
                        {
                            int C_Payment_ID = Util.GetValueOfInt(paymentList[j]);
                            Decimal PaymentAmt = Util.GetValueOfDecimal(amountList[j]);
                            if (Env.Signum(PaymentAmt) != 0)
                            {
                                // log.Config(".. with payment #" + j + ", Amt=" + PaymentAmt);
                                noPayments++;
                                //  use Invoice Applied Amt
                                Decimal amount = Env.ZERO;
                                if ((Math.Abs(AppliedAmt)).CompareTo(Math.Abs(PaymentAmt)) > 0)
                                {
                                    amount = PaymentAmt;
                                }
                                else
                                {
                                    amount = AppliedAmt;
                                }
                                //log.Fine("C_Payment_ID=" + C_Payment_ID + ", C_Invoice_ID=" + C_Invoice_ID
                                // + ", Amount=" + amount + ", Discount=" + DiscountAmt + ", WriteOff=" + WriteOffAmt);

                                //	Allocation Header
                                if (alloc.Get_ID() == 0 && !alloc.Save())
                                {
                                    // log.Log(Level.SEVERE, "Allocation not created");
                                    trx.Rollback();
                                    trx.Close();
                                    return;
                                }
                                //	Allocation Line
                                MAllocationLine aLine = new MAllocationLine(alloc, amount,
                                    DiscountAmt, WriteOffAmt, OverUnderAmt);
                                aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                                //aLine.SetPaymentInfo(C_Payment_ID, C_CashLine_ID);
                                aLine.SetPaymentInfo(C_Payment_ID, 0);//cashline for payment allocation is zero
                                if (!aLine.Save())
                                {
                                    // log.Log(Level.SEVERE, "Allocation Line not written - Invoice=" + C_Invoice_ID);
                                }
                                //  Apply Discounts and WriteOff only first time
                                DiscountAmt = Env.ZERO;
                                WriteOffAmt = Env.ZERO;
                                //  subtract amount from Payment/Invoice
                                AppliedAmt = Decimal.Subtract(AppliedAmt, amount);
                                //AppliedAmt = Decimal.Subtract(PaymentAmt, AppliedAmt);
                                PaymentAmt = Decimal.Subtract(PaymentAmt, amount);
                                //log.Fine("Allocation Amount=" + amount + " - Remaining  Applied=" + AppliedAmt + ", Payment=" + PaymentAmt);

                                //amountList.set(j, PaymentAmt);  //  update
                                amountList[j] = PaymentAmt;  //  update//set

                            }	//	for all applied amounts

                            MPayment pay1 = new MPayment(ctx, C_Payment_ID, trx);

                        }	//	loop through payments for invoice

                        //  No Payments allocated and none existing (e.g. Inv/CM)

                        if (noPayments == 0 && paymentList.Count == 0)
                        {
                            int C_Payment_ID = 0;
                            //  log.Config(" ... no payment - TotalApplied=" + totalAppliedAmt);
                            //  Create Allocation
                            //  log.Fine("C_Payment_ID=" + C_Payment_ID + ", C_Invoice_ID=" + C_Invoice_ID
                            //  + ", Amount=" + AppliedAmt + ", Discount=" + DiscountAmt + ", WriteOff=" + WriteOffAmt);

                            //	Allocation Header
                            if (alloc.Get_ID() == 0 && !alloc.Save())
                            {
                                //log.Log(Level.SEVERE, "Allocation not created");
                                trx.Rollback();
                                trx.Close();
                                return;
                            }
                            //	Allocation Line
                            MAllocationLine aLine = new MAllocationLine(alloc, AppliedAmt,
                                DiscountAmt, WriteOffAmt, OverUnderAmt);
                            aLine.SetDocInfo(C_BPartner_ID, C_Order_ID, C_Invoice_ID);
                            //aLine.SetPaymentInfo(C_Payment_ID, C_CashLine_ID);
                            aLine.SetPaymentInfo(C_Payment_ID, 0);
                            if (!aLine.Save(trx))
                            {
                                //Log(Level.SEVERE, "Allocation Line not written - Invoice=" + C_Invoice_ID);
                            }

                            // log.Fine("Allocation Amount=" + AppliedAmt);
                            MPayment pay1 = new MPayment(ctx, C_Payment_ID, trx);
                        }

                        totalAppliedAmt = Decimal.Add(totalAppliedAmt, AppliedAmt);
                        //   log.Config("TotalRemaining=" + totalAppliedAmt);
                    }   //  invoice selected
                }   //  invoice loop

                #endregion

                //	Only Payments and total of 0 (e.g. Payment/Reversal)
                #region Reversal Payments
                if (invoiceLines == 0 && paymentList.Count > 0
                    && Env.Signum(paymentAppliedAmt) == 0)
                {
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        int C_Payment_ID = Util.GetValueOfInt(paymentList[i]);
                        Decimal PaymentAmt = Util.GetValueOfDecimal(amountList[i]);
                        // log.Fine("Payment=" + C_Payment_ID
                        //         + ", Amount=" + PaymentAmt);// + ", Abs=" + PaymentAbs);

                        //	Allocation Header
                        if (alloc.Get_ID() == 0 && !alloc.Save())
                        {
                            // log.Log(Level.SEVERE, "Allocation not created");
                            trx.Rollback();
                            trx.Close();
                            return;
                        }
                        //	Allocation Line
                        MAllocationLine aLine = new MAllocationLine(alloc, PaymentAmt,
                            Env.ZERO, Env.ZERO, Env.ZERO);
                        aLine.SetDocInfo(C_BPartner_ID, 0, 0);
                        aLine.SetPaymentInfo(C_Payment_ID, 0);
                        if (!aLine.Save(trx))
                        {
                            //  log.Log(Level.SEVERE, "Allocation Line not saved - Payment=" + C_Payment_ID);
                        }
                        MPayment pay1 = new MPayment(ctx, C_Payment_ID, trx);
                    }
                }	//	onlyPayments
                #endregion

                if (Env.Signum(totalAppliedAmt) != 0)
                {
                    //log.Log(Level.SEVERE, "Remaining TotalAppliedAmt=" + totalAppliedAmt);
                }

                //	Should start WF
                if (alloc.Get_ID() != 0)
                {
                    alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                    alloc.Save();

                }

                //  Test/Set IsPaid for Invoice - requires that allocation is posted
                #region Set Invoice IsPaid
                for (int i = 0; i < rowsInvoice.Count; i++)
                {
                    //  Invoice line is selected

                    // if (boolValue)
                    {
                        //KeyNamePair pp = (KeyNamePair)vdgvInvoice.Rows[i].Cells[2].Value;    //  Value
                        // KeyNamePair pp = (KeyNamePair)((BindableObject)rowsInvoice[i]).GetValue(2);    //  Value

                        //  Invoice variables
                        int C_Invoice_ID = Util.GetValueOfInt(rowsInvoice[i]["cinvoiceid"]);
                        String sql = "SELECT invoiceOpen(C_Invoice_ID, 0) "
                            + "FROM C_Invoice WHERE C_Invoice_ID=@param1";
                        Decimal opens = Util.GetValueOfDecimal(DB.GetSQLValueBD(trx, sql, C_Invoice_ID));
                        if (open != null && Env.Signum(opens) == 0)
                        {
                            sql = "UPDATE C_Invoice SET IsPaid='Y' "
                                + "WHERE C_Invoice_ID=" + C_Invoice_ID;
                            int no = DB.ExecuteQuery(sql, null, trx);
                            // log.Config("Invoice #" + i + " is paid");
                        }
                        else
                        {
                            //  log.Config("Invoice #" + i + " is not paid - " + open);
                        }
                    }
                }
                #endregion

                //  Test/Set Payment is fully allocated
                #region Set Payment Allocated
                if (rowsPayment.Count > 0)
                    for (int i = 0; i < paymentList.Count; i++)
                    {
                        int C_Payment_ID = Util.GetValueOfInt(paymentList[i]);
                        MPayment pay = new MPayment(ctx, C_Payment_ID, trx);
                        if (pay.TestAllocation())
                        {
                            pay.Save();
                        }

                        string sqlGetOpenPayments = "SELECT  currencyConvert(ALLOCPAYMENTAVAILABLE(C_Payment_ID) ,p.C_Currency_ID ,260,p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID) FROM C_Payment p Where C_Payment_ID = " + C_Payment_ID;
                        object result = DB.ExecuteScalar(sqlGetOpenPayments, null, trx);
                        Decimal? amtPayment = 0;
                        if (result == null || result == DBNull.Value)
                        {
                            amtPayment = -1;
                        }
                        else
                        {
                            amtPayment = Util.GetValueOfDecimal(result);
                        }

                        if (amtPayment == 0)
                        {
                            pay.SetIsAllocated(true);
                        }
                        else
                        {
                            pay.SetIsAllocated(false);
                        }
                        pay.Save();

                        //log.Config("Payment #" + i + (pay.IsAllocated() ? " not" : " is")
                        //    + " fully allocated");
                    }
                #endregion

                paymentList.Clear();
                amountList.Clear();
                trx.Commit();
                trx.Close();
            }
            catch
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Close();
                    trx = null;
                }
            }
            finally
            {
                if (trx != null)
                {
                    trx.Rollback();
                    trx.Close();
                    trx = null;
                }

            }

        }




    }

    public class PaymentDetails
    {
        public decimal appliedAmt { get; set; }
        public decimal discount { get; set; }
        public decimal writeoff { get; set; }
        public decimal cinvoiceid { get; set; }
        public decimal converted { get; set; }
        public decimal currency { get; set; }
        public string date { get; set; }
        public string docbasetype { get; set; }
        public decimal documentno { get; set; }
        public string isocode { get; set; }
        public decimal multiplierap { get; set; }
        public decimal openamt { get; set; }
        public decimal payment { get; set; }
        public int ccashlineid { get; set; }
        public int cpaymentid { get; set; }
    }




}