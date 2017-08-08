/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_Payment
 * Chronological Development
 * Veena Pandey     23-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using System.Windows.Forms;
using VAdvantage.Process;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
namespace VAdvantage.Model
{
    public class MPayment : X_C_Payment, DocAction, ProcessCall
    {
        /**	Temporary	Payment Processors		*/
        private MPaymentProcessor[] _paymentProcessors = null;
        /**	Temporary	Payment Processor		*/
        private MPaymentProcessor _paymentProcessor = null;
        /** VVC not stored						*/
        private String _creditCardVV = null;
        // Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MPayment).FullName);
        /** Error Message						*/
        private String _errorMessage = null;

        /** Reversal Indicator			*/
        public const String REVERSE_INDICATOR = "^";

        /**	Process Message 			*/
        private String _processMsg = null;
        /**	Just Prepared Flag			*/
        private bool _justPrepared = false;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Payment_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPayment(Ctx ctx, int C_Payment_ID, Trx trxName)
            : base(ctx, C_Payment_ID, trxName)
        {
            //  New
            if (C_Payment_ID == 0)
            {
                SetDocAction(DOCACTION_Complete);
                SetDocStatus(DOCSTATUS_Drafted);
                SetTrxType(TRXTYPE_Sales);
                //
                SetR_AvsAddr(R_AVSZIP_Unavailable);
                SetR_AvsZip(R_AVSZIP_Unavailable);
                //
                SetIsReceipt(true);
                SetIsApproved(false);
                SetIsReconciled(false);
                SetIsAllocated(false);
                SetIsOnline(false);
                SetIsSelfService(false);
                SetIsDelayedCapture(false);
                SetIsPrepayment(false);
                SetProcessed(false);
                SetProcessing(false);
                SetPosted(false);
                //
                SetPayAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetTaxAmt(Env.ZERO);
                SetWriteOffAmt(Env.ZERO);
                SetIsOverUnderPayment(false);
                SetOverUnderAmt(Env.ZERO);
                //
                SetDateTrx(DateTime.Now);
                SetDateAcct(GetDateTrx());
                SetTenderType(TENDERTYPE_Check);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MPayment(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * 	Get Payments Of BPartner
	     *	@param ctx context
	     *	@param C_BPartner_ID id
	     *	@param trxName transaction
	     *	@return array
	     */
        public static MPayment[] GetOfBPartner(Ctx ctx, int C_BPartner_ID, Trx trxName)
        {
            List<MPayment> list = new List<MPayment>();
            String sql = "SELECT * FROM C_Payment WHERE C_BPartner_ID=" + C_BPartner_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MPayment(ctx, dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            //
            MPayment[] retValue = new MPayment[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
	     *  ReSet Payment to new status
	     */
        public void ReSetNew()
        {
            Set_ValueNoCheck("C_Payment_ID", 0);		//	forces new Record
            Set_ValueNoCheck("DocumentNo", null);
            SetDocAction(DOCACTION_Prepare);
            SetDocStatus(DOCSTATUS_Drafted);
            SetProcessed(false);
            SetPosted(false);
            SetIsReconciled(false);
            SetIsAllocated(false);
            SetIsOnline(false);
            SetIsDelayedCapture(false);
            //	SetC_BPartner_ID(0);
            SetC_Invoice_ID(0);
            SetC_Order_ID(0);
            SetC_Charge_ID(0);
            SetC_Project_ID(0);
            SetIsPrepayment(false);
        }

        /**
         * 	Is Cashbook Transfer Trx
         *	@return true if Cash Trx
         */
        public bool IsCashTrx()
        {
            return "X".Equals(GetTenderType());
        }

        /**
         *  Set Credit Card.
         *  Need to Set PatmentProcessor after Amount/Currency Set
         *
         *  @param TrxType Transaction Type see TRX_
         *  @param creditCardType CC type
         *  @param creditCardNumber CC number
         *  @param creditCardVV CC verification
         *  @param creditCardExpMM CC Exp MM
         *  @param creditCardExpYY CC Exp YY
         *  @return true if valid
         */
        public bool SetCreditCard(String TrxType, String creditCardType, String creditCardNumber,
            String creditCardVV, int creditCardExpMM, int creditCardExpYY)
        {
            SetTenderType(TENDERTYPE_CreditCard);
            SetTrxType(TrxType);
            //
            SetCreditCardType(creditCardType);
            SetCreditCardNumber(creditCardNumber);
            SetCreditCardVV(creditCardVV);
            SetCreditCardExpMM(creditCardExpMM);
            SetCreditCardExpYY(creditCardExpYY);
            //
            int check = MPaymentValidate.ValidateCreditCardNumber(creditCardNumber, creditCardType).Length
                + MPaymentValidate.ValidateCreditCardExp(creditCardExpMM, creditCardExpYY).Length;
            if (creditCardVV.Length > 0)
                check += MPaymentValidate.ValidateCreditCardVV(creditCardVV, creditCardType).Length;
            return check == 0;
        }

        /**
         *  Set Credit Card - Exp.
         *  Need to Set PatmentProcessor after Amount/Currency Set
         *
         *  @param TrxType Transaction Type see TRX_
         *  @param creditCardType CC type
         *  @param creditCardNumber CC number
         *  @param creditCardVV CC verification
         *  @param creditCardExp CC Exp
         *  @return true if valid
         */
        public bool SetCreditCard(String trxType, String creditCardType, String creditCardNumber,
            String creditCardVV, String creditCardExp)
        {
            return SetCreditCard(trxType, creditCardType, creditCardNumber,
                creditCardVV, MPaymentValidate.GetCreditCardExpMM(creditCardExp),
                MPaymentValidate.GetCreditCardExpYY(creditCardExp));
        }

        /**
         *  Set ACH BankAccount Info
         *
         *  @param C_BankAccount_ID bank account
         *  @param isReceipt true if receipt
         *  @return true if valid
         */
        public bool SetBankACH(MPaySelectionCheck preparedPayment)
        {
            //	Our Bank
            SetC_BankAccount_ID(preparedPayment.GetParent().GetC_BankAccount_ID());
            //	TarGet Bank
            int C_BP_BankAccount_ID = preparedPayment.GetC_BP_BankAccount_ID();
            MBPBankAccount ba = new MBPBankAccount(preparedPayment.GetCtx(), C_BP_BankAccount_ID, null);
            SetRoutingNo(ba.GetRoutingNo());
            SetAccountNo(ba.GetAccountNo());
            SetIsReceipt(X_C_Order.PAYMENTRULE_DirectDebit.Equals	//	AR only
                    (preparedPayment.GetPaymentRule()));
            //
            int check = MPaymentValidate.ValidateRoutingNo(GetRoutingNo()).Length
                + MPaymentValidate.ValidateAccountNo(GetAccountNo()).Length;
            return check == 0;
        }

        /**
         *  Set ACH BankAccount Info
         *
         *  @param C_BankAccount_ID bank account
         *  @param isReceipt true if receipt
         * 	@param tenderType - Direct Debit or Direct Deposit
         *  @param routingNo routing
         *  @param accountNo account
         *  @return true if valid
         */
        public bool SetBankACH(int C_BankAccount_ID, bool isReceipt, String tenderType,
            String routingNo, String accountNo)
        {
            SetTenderType(tenderType);
            SetIsReceipt(isReceipt);
            //
            if (C_BankAccount_ID > 0
                && (routingNo == null || routingNo.Length == 0 || accountNo == null || accountNo.Length == 0))
                SetBankAccountDetails(C_BankAccount_ID);
            else
            {
                SetC_BankAccount_ID(C_BankAccount_ID);
                SetRoutingNo(routingNo);
                SetAccountNo(accountNo);
            }
            SetCheckNo("");
            //
            int check = MPaymentValidate.ValidateRoutingNo(routingNo).Length
                + MPaymentValidate.ValidateAccountNo(accountNo).Length;
            return check == 0;
        }

        /**
         *  Set Check BankAccount Info
         *
         *  @param C_BankAccount_ID bank account
         *  @param isReceipt true if receipt
         *  @param checkNo chack no
         *  @return true if valid
         */
        public bool SetBankCheck(int C_BankAccount_ID, bool isReceipt, String checkNo)
        {
            return SetBankCheck(C_BankAccount_ID, isReceipt, null, null, checkNo);
        }

        /**
         *  Set Check BankAccount Info
         *
         *  @param C_BankAccount_ID bank account
         *  @param isReceipt true if receipt
         *  @param routingNo routing no
         *  @param accountNo account no
         *  @param checkNo chack no
         *  @return true if valid
         */
        public bool SetBankCheck(int C_BankAccount_ID, bool isReceipt,
            String routingNo, String accountNo, String checkNo)
        {
            SetTenderType(TENDERTYPE_Check);
            SetIsReceipt(isReceipt);
            //
            if (C_BankAccount_ID > 0
                && (routingNo == null || routingNo.Length == 0
                    || accountNo == null || accountNo.Length == 0))
                SetBankAccountDetails(C_BankAccount_ID);
            else
            {
                SetC_BankAccount_ID(C_BankAccount_ID);
                SetRoutingNo(routingNo);
                SetAccountNo(accountNo);
            }
            SetCheckNo(checkNo);
            //
            int check = MPaymentValidate.ValidateRoutingNo(routingNo).Length
                + MPaymentValidate.ValidateAccountNo(accountNo).Length
                + MPaymentValidate.ValidateCheckNo(checkNo).Length;
            return check == 0;       //  no error message
        }

        /**
         * 	Set Bank Account Details.
         * 	Look up Routing No & Bank Acct No
         * 	@param C_BankAccount_ID bank account
         */
        public void SetBankAccountDetails(int C_BankAccount_ID)
        {
            if (C_BankAccount_ID == 0)
                return;
            SetC_BankAccount_ID(C_BankAccount_ID);
            //
            String sql = "SELECT b.RoutingNo, ba.AccountNo "
                + "FROM C_BankAccount ba"
                + " INNER JOIN C_Bank b ON (ba.C_Bank_ID=b.C_Bank_ID) "
                + "WHERE C_BankAccount_ID=" + C_BankAccount_ID;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                if (idr.Read())
                {
                    SetRoutingNo(idr.GetString(0));
                    SetAccountNo(idr.GetString(1));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /**
         *  Set Account Address
         *
         *  @param name name
         *  @param street street
         *  @param city city
         *  @param state state
         *  @param zip zip
         * 	@param country country
         */
        public void SetAccountAddress(String name, String street, String city,
            String state, String zip, String country)
        {
            SetA_Name(name);
            SetA_Street(street);
            SetA_City(city);
            SetA_State(state);
            SetA_Zip(zip);
            SetA_Country(country);
        }


        /**
         *  Process Payment
         *  @return true if approved
         */
        public bool ProcessOnline()
        {
            log.Info("Amt=" + GetPayAmt());
            //
            SetIsOnline(true);
            SetErrorMessage(null);
            //	prevent charging twice
            if (IsApproved())
            {
                log.Info("Already processed - " + GetR_Result() + " - " + GetR_RespMsg());
                SetErrorMessage("Payment already Processed");
                return true;
            }

            if (_paymentProcessor == null)
                SetPaymentProcessor();
            if (_paymentProcessor == null)
            {
                log.Log(Level.WARNING, "No Payment Processor Model");
                SetErrorMessage("No Payment Processor Model");
                return false;
            }

            bool approved = false;
            /**	Process Payment on Server	*/
            //if (DataBase.isRemoteObjects())
            //{
            //    Server server = CConnection.Get().GetServer();
            //    try
            //    {
            //        if (server != null)
            //        {	//	See ServerBean
            //            String trxName = null;	//	unconditionally save
            //            Save(trxName);	//	server reads from disk
            //            approved = server.paymentOnline (GetCtx(), GetC_Payment_ID(), 
            //                _paymentProcessor.GetC_PaymentProcessor_ID(), trxName);
            //            if (CLogMgt.IsLevelFinest())
            //                s_log.Fine("server => " + approved);
            //            Load(trxName);	//	server saves to disk
            //            SetIsApproved(approved);
            //            return approved;
            //        }
            //        log.log(Level.WARNING, "AppsServer not found"); 
            //    }
            //    catch (RemoteException ex)
            //    {
            //        log.log(Level.SEVERE, "AppsServer error", ex);
            //    }
            //}
            /** **/

            //	Try locally
            try
            {
                PaymentProcessor pp = PaymentProcessor.Create(_paymentProcessor, this);
                if (pp == null)
                    SetErrorMessage("No Payment Processor");
                else
                {
                    approved = pp.ProcessCC();
                    if (approved)
                        SetErrorMessage(null);
                    else
                        SetErrorMessage("From " + GetCreditCardName() + ": " + GetR_RespMsg());
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, "processOnline", e);
                SetErrorMessage("Payment Processor Error");
            }
            SetIsApproved(approved);
            return approved;
        }

        /**
         *  Process Online Payment.
         *  implements ProcessEngine.ProcessCall after standard constructor
         *  Called when pressing the Process_Online button in C_Payment
         *
         *  @param ctx Ctx
         *  @param pi Process Info
         *  @param trx transaction
         *  @return true if the next process should be performed
         */
        public bool StartProcess(Ctx ctx, ProcessInfo pi, Trx trx)
        {
            log.Info("startProcess - " + pi.GetRecord_ID());
            bool retValue = false;
            //
            if (pi.GetRecord_ID() != Get_ID())
            {
                log.Log(Level.SEVERE, "startProcess - Not same Payment - " + pi.GetRecord_ID());
                return false;
            }
            //  Process it
            retValue = ProcessOnline();
            Save();
            return retValue;    //  Payment processed
        }


        /**
         * 	Before Save
         *	@param newRecord new
         *	@return save
         */
        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                //	We have a charge
                if (GetC_Charge_ID() != 0)
                {
                    if (newRecord || Is_ValueChanged("C_Charge_ID"))
                    {
                        SetC_Order_ID(0);
                        SetC_Invoice_ID(0);
                        SetWriteOffAmt(Env.ZERO);
                        SetDiscountAmt(Env.ZERO);
                        SetIsOverUnderPayment(false);
                        SetOverUnderAmt(Env.ZERO);
                        string sql = "SELECT IsAdvanceCharge FROM C_Charge WHERE C_Charge_ID = " + GetC_Charge_ID();
                        string isAdvCharge = "";
                        try
                        {
                            isAdvCharge = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
                        }
                        catch
                        {

                        }
                        if (isAdvCharge.Equals("Y"))
                        {
                            SetIsPrepayment(true);
                        }
                        else
                        {
                            SetIsPrepayment(false);
                        }
                    }
                }
                //	We need a BPartner
                //else if (GetC_BPartner_ID() == 0 && !IsCashTrx())
                else if (GetC_BPartner_ID() == 0 && GetTenderType() == null)
                {
                    if (GetC_Invoice_ID() != 0)
                    {
                    }
                    else if (GetC_Order_ID() != 0)
                    {
                    }
                    else
                    {
                        log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@: @C_BPartner_ID@"));
                        return false;
                    }
                }
                //	Prepayment: No charge and order or project (not as acct dimension)
                if (newRecord
                    || Is_ValueChanged("C_Charge_ID") || Is_ValueChanged("C_Invoice_ID")
                    || Is_ValueChanged("C_Order_ID") || Is_ValueChanged("C_Project_ID"))
                    SetIsPrepayment(GetC_Charge_ID() == 0
                        && GetC_BPartner_ID() != 0
                        && (GetC_Order_ID() != 0
                            || (GetC_Project_ID() != 0 && GetC_Invoice_ID() == 0)));
                if (GetC_Charge_ID() != 0)
                {
                    string sqlAdvCharge = "SELECT IsAdvanceCharge FROM C_Charge WHERE C_Charge_ID = " + GetC_Charge_ID();
                    string isAdvCharge = "";
                    try
                    {
                        isAdvCharge = Util.GetValueOfString(DB.ExecuteScalar(sqlAdvCharge, null, null));
                    }
                    catch
                    {

                    }
                    if (isAdvCharge.Equals("Y"))
                    {
                        SetIsPrepayment(true);
                    }
                    else
                    {
                        SetIsPrepayment(false);
                    }
                }

                if (IsPrepayment())
                {
                    if (newRecord
                        || Is_ValueChanged("C_Order_ID") || Is_ValueChanged("C_Project_ID"))
                    {
                        SetWriteOffAmt(Env.ZERO);
                        SetDiscountAmt(Env.ZERO);
                        SetIsOverUnderPayment(false);
                        SetOverUnderAmt(Env.ZERO);
                    }
                }

                //	Document Type/Receipt
                if (GetC_DocType_ID() == 0)
                    SetC_DocType_ID();
                else
                {
                    MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
                    SetIsReceipt(dt.IsSOTrx());
                }
                SetDocumentNo();
                //
                if (GetDateAcct() == null)
                    SetDateAcct(GetDateTrx());
                //
                //if (!IsOverUnderPayment())
                //    SetOverUnderAmt(Env.ZERO);

                //	Organization
                if ((newRecord || Is_ValueChanged("C_BankAccount_ID"))
                    && GetC_Charge_ID() == 0)	//	allow different org for charge
                {
                    MBankAccount ba = MBankAccount.Get(GetCtx(), GetC_BankAccount_ID());
                    if (ba.GetAD_Org_ID() != 0)
                        SetAD_Org_ID(ba.GetAD_Org_ID());
                }
            }
            catch (Exception ex)
            {
                log.Severe(ex.ToString());
                //MessageBox.Show("MPayment-Error Payment not saved");
                return false;
            }

            return true;
        }	//	beforeSave

        /**
         * 	Get Allocated Amt in Payment Currency
         *	@return amount or null
         */
        public Decimal? GetAllocatedAmt()
        {
            Decimal? retValue = null;
            if (GetC_Charge_ID() != 0)
                return GetPayAmt();
            //
            String sql = "SELECT SUM(currencyConvert(al.Amount,"
                    + "ah.C_Currency_ID, p.C_Currency_ID,ah.DateTrx,p.C_ConversionType_ID, al.AD_Client_ID,al.AD_Org_ID)) "
                + "FROM C_AllocationLine al"
                + " INNER JOIN C_AllocationHdr ah ON (al.C_AllocationHdr_ID=ah.C_AllocationHdr_ID) "
                + " INNER JOIN C_Payment p ON (al.C_Payment_ID=p.C_Payment_ID) "
                + "WHERE al.C_Payment_ID=" + GetC_Payment_ID() + ""
                + " AND ah.IsActive='Y' AND al.IsActive='Y'";
            //	+ " AND al.C_Invoice_ID IS NOT NULL";
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                if (idr.Read())
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "GetAllocatedAmt", e);
            }
            //	log.Fine("GetAllocatedAmt - " + retValue);
            //	? ROUND(NVL(v_AllocatedAmt,0), 2);
            return retValue;
        }

        /**
         * 	Test Allocation (and Set allocated flag)
         *	@return true if updated
         */
        public bool TestAllocation()
        {
            //	Cash Trx always allocated
            if (IsCashTrx())
            {
                if (!IsAllocated())
                {
                    SetIsAllocated(true);
                    return true;
                }
                return false;
            }
            //
            Decimal? alloc = GetAllocatedAmt();
            if (alloc == null)
                alloc = Env.ZERO;
            Decimal total = GetPayAmt();

            if (!IsReceipt())
                total = Decimal.Negate(total);
            bool test = total.CompareTo((Decimal)alloc) == 0;
            bool change = test != IsAllocated();
            if (change)
                SetIsAllocated(test);
            log.Fine("Allocated=" + test
                + " (" + alloc + "=" + total + ")");
            return change;
        }

        /**
         * 	Set Allocated Flag for payments
         * 	@param ctx context
         *	@param C_BPartner_ID if 0 all
         *	@param trxName trx
         */
        public static void SetIsAllocated(Ctx ctx, int C_BPartner_ID, Trx trxName)
        {
            int counter = 0;
            String sql = "SELECT * FROM C_Payment "
                + "WHERE IsAllocated='N' AND DocStatus IN ('CO','CL')";
            if (C_BPartner_ID > 1)
                sql += " AND C_BPartner_ID=" + C_BPartner_ID;
            else
                sql += " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MPayment pay = new MPayment(ctx, dr, trxName);
                        if (pay.TestAllocation())
                            if (pay.Save())
                                counter++;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            _log.Config("#" + counter);
        }

        /**
         * 	Set Error Message
         *	@param errorMessage error message
         */
        public void SetErrorMessage(String errorMessage)
        {
            _errorMessage = errorMessage;
        }

        /**
         * 	Get Error Message
         *	@return error message
         */
        public String GetErrorMessage()
        {
            return _errorMessage;
        }


        /**
         *  Set Bank Account for Payment.
         *  @param C_BankAccount_ID C_BankAccount_ID
         */
        public new void SetC_BankAccount_ID(int C_BankAccount_ID)
        {
            if (C_BankAccount_ID == 0)
            {
                SetPaymentProcessor();
                if (GetC_BankAccount_ID() == 0)
                    throw new ArgumentException("Can't find Bank Account");
            }
            else
                base.SetC_BankAccount_ID(C_BankAccount_ID);
        }

        /**
         *  Set BankAccount and PaymentProcessor
         *  @return true if found
         */
        public bool SetPaymentProcessor()
        {
            return SetPaymentProcessor(GetTenderType(), GetCreditCardType());
        }

        /**
         *  Set BankAccount and PaymentProcessor
         *  @param tender TenderType see TENDER_
         *  @param CCType CC Type see CC_
         *  @return true if found
         */
        public bool SetPaymentProcessor(String tender, String CCType)
        {
            _paymentProcessor = null;
            //	Get Processor List
            if (_paymentProcessors == null || _paymentProcessors.Length == 0)
                _paymentProcessors = MPaymentProcessor.Find(GetCtx(), tender, CCType, GetAD_Client_ID(),
                    GetC_Currency_ID(), GetPayAmt(), Get_Trx());
            //	Relax Amount
            if (_paymentProcessors == null || _paymentProcessors.Length == 0)
                _paymentProcessors = MPaymentProcessor.Find(GetCtx(), tender, CCType, GetAD_Client_ID(),
                    GetC_Currency_ID(), Env.ZERO, Get_Trx());
            if (_paymentProcessors == null || _paymentProcessors.Length == 0)
                return false;

            //	Find the first right one
            for (int i = 0; i < _paymentProcessors.Length; i++)
            {
                if (_paymentProcessors[i].Accepts(tender, CCType))
                {
                    _paymentProcessor = _paymentProcessors[i];
                }
            }
            if (_paymentProcessor != null)
                SetC_BankAccount_ID(_paymentProcessor.GetC_BankAccount_ID());
            //
            return _paymentProcessor != null;
        }


        /**
         * 	Get Accepted Credit Cards for PayAmt (default 0)
         *	@return credit cards
         */
        public ValueNamePair[] GetCreditCards()
        {
            return GetCreditCards(GetPayAmt());
        }


        /**
         * 	Get Accepted Credit Cards for amount
         *	@param amt trx amount
         *	@return credit cards
         */
        public ValueNamePair[] GetCreditCards(Decimal amt)
        {
            try
            {
                if (_paymentProcessors == null || _paymentProcessors.Length == 0)
                    _paymentProcessors = MPaymentProcessor.Find(GetCtx(), null, null,
                        GetAD_Client_ID(), GetC_Currency_ID(), amt, Get_Trx());
                //
                Dictionary<String, ValueNamePair> map = new Dictionary<String, ValueNamePair>(); //	to eliminate duplicates
                for (int i = 0; i < _paymentProcessors.Length; i++)
                {
                    if (_paymentProcessors[i].IsAcceptAMEX())
                        map.Add(CREDITCARDTYPE_Amex, GetCreditCardPair(CREDITCARDTYPE_Amex));
                    if (_paymentProcessors[i].IsAcceptDiners())
                        map.Add(CREDITCARDTYPE_Diners, GetCreditCardPair(CREDITCARDTYPE_Diners));
                    if (_paymentProcessors[i].IsAcceptDiscover())
                        map.Add(CREDITCARDTYPE_Discover, GetCreditCardPair(CREDITCARDTYPE_Discover));
                    if (_paymentProcessors[i].IsAcceptMC())
                        map.Add(CREDITCARDTYPE_MasterCard, GetCreditCardPair(CREDITCARDTYPE_MasterCard));
                    if (_paymentProcessors[i].IsAcceptCorporate())
                        map.Add(CREDITCARDTYPE_PurchaseCard, GetCreditCardPair(CREDITCARDTYPE_PurchaseCard));
                    if (_paymentProcessors[i].IsAcceptVisa())
                        map.Add(CREDITCARDTYPE_Visa, GetCreditCardPair(CREDITCARDTYPE_Visa));
                } //	for all payment processors
                //
                ValueNamePair[] retValue = new ValueNamePair[map.Count];
                map.Values.CopyTo(retValue, 0);
                log.Fine("GetCreditCards - #" + retValue.Length + " - Processors=" + _paymentProcessors.Length);
                return retValue;
            }
            catch (Exception ex)
            {
                //ex.StackTrace;
                log.Severe(ex.ToString());
                return null;
            }
        }

        /**
         * 	Get Type and name pair
         *	@param CreditCardType credit card Type
         *	@return pair
         */
        private ValueNamePair GetCreditCardPair(String creditCardType)
        {
            return new ValueNamePair(creditCardType, GetCreditCardName(creditCardType));
        }

        /**
         *  Credit Card Number
         *  @param CreditCardNumber CreditCard Number
         */
        public new void SetCreditCardNumber(String creditCardNumber)
        {
            base.SetCreditCardNumber(MPaymentValidate.CheckNumeric(creditCardNumber));
        }

        /**
         *  Verification Code
         *  @param newCreditCardVV CC verification
         */
        public void SetCreditCardVV(String newCreditCardVV)
        {
            _creditCardVV = MPaymentValidate.CheckNumeric(newCreditCardVV);
        }

        /**
         *  Verification Code
         *  @return CC verification
         */
        public String GetCreditCardVV()
        {
            return _creditCardVV;
        }

        /**
         *  Two Digit CreditCard MM
         *  @param CreditCardExpMM Exp month
         */
        public new void SetCreditCardExpMM(int creditCardExpMM)
        {
            if (creditCardExpMM < 1 || creditCardExpMM > 12)
            {
                ;
            }
            else
            {
                base.SetCreditCardExpMM(creditCardExpMM);
            }
        }

        /**
         *  Two digit CreditCard YY (til 2020)
         *  @param newCreditCardExpYY 2 or 4 digit year
         */
        public new  void SetCreditCardExpYY(int newCreditCardExpYY)
        {
            int creditCardExpYY = newCreditCardExpYY;
            if (newCreditCardExpYY > 1999)
                creditCardExpYY = newCreditCardExpYY - 2000;
            base.SetCreditCardExpYY(creditCardExpYY);
        }

        /**
         *  CreditCard Exp  MMYY
         *  @param mmyy Exp in form of mmyy
         *  @return true if valid
         */
        public bool SetCreditCardExp(String mmyy)
        {
            if (MPaymentValidate.ValidateCreditCardExp(mmyy).Length != 0)
                return false;
            //
            String exp = MPaymentValidate.CheckNumeric(mmyy);
            String mmStr = exp.Substring(0, 2);
            String yyStr = exp.Substring(2, 4);
            SetCreditCardExpMM(int.Parse(mmStr));
            SetCreditCardExpYY(int.Parse(yyStr));
            return true;
        }


        /**
         *  CreditCard Exp  MMYY
         *  @param delimiter / - or null
         *  @return Exp
         */
        public String GetCreditCardExp(String delimiter)
        {
            String mm = GetCreditCardExpMM().ToString();
            String yy = GetCreditCardExpYY().ToString();

            StringBuilder retValue = new StringBuilder();
            if (mm.Length == 1)
                retValue.Append("0");
            retValue.Append(mm);
            //
            if (delimiter != null)
                retValue.Append(delimiter);
            //
            if (yy.Length == 1)
                retValue.Append("0");
            retValue.Append(yy);
            //
            return (retValue.ToString());
        }

        /**
         *  MICR
         *  @param MICR MICR
         */
        public new void SetMicr(String sMICR)
        {
            base.SetMicr(MPaymentValidate.CheckNumeric(sMICR));
        }

        /**
         *  Routing No
         *  @param RoutingNo Routing No
         */
        public new void SetRoutingNo(String routingNo)
        {
            base.SetRoutingNo(MPaymentValidate.CheckNumeric(routingNo));
        }


        /**
         *  Bank Account No
         *  @param AccountNo AccountNo
         */
        public new void SetAccountNo(String accountNo)
        {
            base.SetAccountNo(MPaymentValidate.CheckNumeric(accountNo));
        }

        /**
         *  Check No
         *  @param CheckNo Check No
         */
        public new void SetCheckNo(String checkNo)
        {
            base.SetCheckNo(MPaymentValidate.CheckNumeric(checkNo));
        }

        /**
         *  Set DocumentNo to Payment Info.
         * 	If there is a R_PnRef that is Set automatically 
         */
        private void SetDocumentNo()
        {
            // Added By Bharat 17.05.2014
            if (TENDERTYPE_Check.Equals(GetTenderType()))
            {
                return;
            }

            //	Cash Transfer
            if ("X".Equals(GetTenderType()))
                return;
            //	Current Document No
            String documentNo = GetDocumentNo();
            //	Existing reversal
            if (documentNo != null
                && documentNo.IndexOf(REVERSE_INDICATOR) >= 0)
                return;

            //	If external number exists - enforce it 
            if (GetR_PnRef() != null && GetR_PnRef().Length > 0)
            {
                if (!GetR_PnRef().Equals(documentNo))
                    SetDocumentNo(GetR_PnRef());
                return;
            }

            documentNo = "";
            //	Credit Card
            if (TENDERTYPE_CreditCard.Equals(GetTenderType()))
            {
                documentNo = GetCreditCardType()
                    + " " + Obscure.ObscureValue(GetCreditCardNumber())
                    + " " + GetCreditCardExpMM()
                    + "/" + GetCreditCardExpYY();
            }
            //	Own Check No
            else if (TENDERTYPE_Check.Equals(GetTenderType())
                && !IsReceipt()
                && GetCheckNo() != null && GetCheckNo().Length > 0)
            {
                documentNo = GetCheckNo();
            }
            //	Customer Check: Routing: Account #Check 
            else if (TENDERTYPE_Check.Equals(GetTenderType())
                && IsReceipt())
            {
                if (GetRoutingNo() != null)
                    documentNo = GetRoutingNo() + ": ";
                if (GetAccountNo() != null)
                    documentNo += GetAccountNo();
                if (GetCheckNo() != null)
                {
                    if (documentNo.Length > 0)
                        documentNo += " ";
                    documentNo += "#" + GetCheckNo();
                }
            }

            //	Set Document No
            documentNo = documentNo.Trim();
            if (documentNo.Length > 0)
                SetDocumentNo(documentNo);
        }

        /**
         * 	Set Refernce No (and Document No)
         *	@param R_PnRef reference
         */
        public new void SetR_PnRef(String R_PnRef)
        {
            base.SetR_PnRef(R_PnRef);
            if (R_PnRef != null)
                SetDocumentNo(R_PnRef);
        }

        /**
         *  Set Payment Amount
         *  @param PayAmt Pay Amt
         */
        public new void SetPayAmt(Decimal? payAmt)
        {
            base.SetPayAmt(payAmt == null ? Env.ZERO : (Decimal)payAmt);
        }

        /**
         *  Set Payment Amount
         *
         * @param C_Currency_ID currency
         * @param payAmt amount
         */
        public void SetAmount(int C_Currency_ID, Decimal payAmt)
        {
            if (C_Currency_ID == 0)
                C_Currency_ID = MClient.Get(GetCtx()).GetC_Currency_ID();
            SetC_Currency_ID(C_Currency_ID);
            SetPayAmt(payAmt);
        }

        /**
         *  Discount Amt
         *  @param DiscountAmt Discount
         */
        public new void SetDiscountAmt(Decimal? discountAmt)
        {
            base.SetDiscountAmt(discountAmt == null ? Env.ZERO : (Decimal)discountAmt);
        }

        /**
         *  WriteOff Amt
         *  @param WriteOffAmt WriteOff
         */
        public new void SetWriteOffAmt(Decimal? writeOffAmt)
        {
            base.SetWriteOffAmt(writeOffAmt == null ? Env.ZERO : (Decimal)writeOffAmt);
        }

        /**
         *  OverUnder Amt
         *  @param OverUnderAmt OverUnder
         */
        public new void SetOverUnderAmt(Decimal? overUnderAmt)
        {
            base.SetOverUnderAmt(overUnderAmt == null ? Env.ZERO : (Decimal)overUnderAmt);
            SetIsOverUnderPayment(GetOverUnderAmt().CompareTo(Env.ZERO) != 0);
        }

        /**
         *  Tax Amt
         *  @param TaxAmt Tax
         */
        public new void SetTaxAmt(Decimal? taxAmt)
        {
            base.SetTaxAmt(taxAmt == null ? Env.ZERO : (Decimal)taxAmt);
        }

        /**
         * 	Set Info from BP Bank Account
         *	@param ba BP bank account
         */
        public void SetBP_BankAccount(MBPBankAccount ba)
        {
            log.Fine("" + ba);
            if (ba == null)
                return;
            SetC_BPartner_ID(ba.GetC_BPartner_ID());
            SetAccountAddress(ba.GetA_Name(), ba.GetA_Street(), ba.GetA_City(),
                ba.GetA_State(), ba.GetA_Zip(), ba.GetA_Country());
            SetA_EMail(ba.GetA_EMail());
            SetA_Ident_DL(ba.GetA_Ident_DL());
            SetA_Ident_SSN(ba.GetA_Ident_SSN());
            //	CC
            if (ba.GetCreditCardType() != null)
                SetCreditCardType(ba.GetCreditCardType());
            if (ba.GetCreditCardNumber() != null)
                SetCreditCardNumber(ba.GetCreditCardNumber());
            if (ba.GetCreditCardExpMM() != 0)
                SetCreditCardExpMM(ba.GetCreditCardExpMM());
            if (ba.GetCreditCardExpYY() != 0)
                SetCreditCardExpYY(ba.GetCreditCardExpYY());
            //	Bank
            if (ba.GetAccountNo() != null)
                SetAccountNo(ba.GetAccountNo());
            if (ba.GetRoutingNo() != null)
                SetRoutingNo(ba.GetRoutingNo());
        }

        /**
         * 	Save Info from BP Bank Account
         *	@param ba BP bank account
         * 	@return true if saved
         */
        public bool SaveToBP_BankAccount(MBPBankAccount ba)
        {
            if (ba == null)
                return false;
            ba.SetA_Name(GetA_Name());
            ba.SetA_Street(GetA_Street());
            ba.SetA_City(GetA_City());
            ba.SetA_State(GetA_State());
            ba.SetA_Zip(GetA_Zip());
            ba.SetA_Country(GetA_Country());
            ba.SetA_EMail(GetA_EMail());
            ba.SetA_Ident_DL(GetA_Ident_DL());
            ba.SetA_Ident_SSN(GetA_Ident_SSN());
            //	CC
            ba.SetCreditCardType(GetCreditCardType());
            ba.SetCreditCardNumber(GetCreditCardNumber());
            ba.SetCreditCardExpMM(GetCreditCardExpMM());
            ba.SetCreditCardExpYY(GetCreditCardExpYY());
            //	Bank
            if (GetAccountNo() != null)
                ba.SetAccountNo(GetAccountNo());
            if (GetRoutingNo() != null)
                ba.SetRoutingNo(GetRoutingNo());
            //	Trx
            ba.SetR_AvsAddr(GetR_AvsAddr());
            ba.SetR_AvsZip(GetR_AvsZip());
            //
            bool ok = ba.Save(Get_Trx());
            log.Fine(ba.ToString());
            return ok;
        }

        /**
         * 	Set Doc Type bases on IsReceipt
         */
        private void SetC_DocType_ID()
        {
            SetC_DocType_ID(IsReceipt());
        }

        /**
         * 	Set Doc Type
         * 	@param isReceipt is receipt
         */
        public void SetC_DocType_ID(bool isReceipt)
        {
            SetIsReceipt(isReceipt);
            String sql = "SELECT C_DocType_ID FROM C_DocType WHERE AD_Client_ID=@clid AND DocBaseType=@docbs ORDER BY IsDefault DESC";
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@clid", GetAD_Client_ID());
                if (isReceipt)
                {
                    param[1] = new SqlParameter("@docbs", MDocBaseType.DOCBASETYPE_ARRECEIPT);
                }
                else
                {
                    param[1] = new SqlParameter("@docbs", MDocBaseType.DOCBASETYPE_APPAYMENT);
                }

                idr = DataBase.DB.ExecuteReader(sql, param);
                if (idr.Read())
                {
                    SetC_DocType_ID(Utility.Util.GetValueOfInt(idr[0].ToString()));
                }
                else
                {
                    log.Warning("SetDocType - NOT found - isReceipt=" + isReceipt);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
        }

        /**
         * 	Set Document Type
         *	@param C_DocType_ID doc type
         */
        public new void SetC_DocType_ID(int C_DocType_ID)
        {
            //	if (GetDocumentNo() != null && GetC_DocType_ID() != C_DocType_ID)
            //		SetDocumentNo(null);
            base.SetC_DocType_ID(C_DocType_ID);
        }

        /**
         * 	Verify Document Type with Invoice
         *	@return true if ok
         */
        private bool VerifyDocType()
        {
            if (GetC_DocType_ID() == 0)
                return false;
            //
            Boolean? invoiceSO = null;
            IDataReader idr = null;
            String sql = "";
            //	Check Invoice First
            if (GetC_Invoice_ID() > 0)
            {
                sql = "SELECT idt.IsSOTrx "
                    + "FROM C_Invoice i"
                    + " INNER JOIN C_DocType idt ON (i.C_DocType_ID=idt.C_DocType_ID) "
                    + "WHERE i.C_Invoice_ID=" + GetC_Invoice_ID();
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                    if (idr.Read())
                        invoiceSO = "Y".Equals(idr[0].ToString());
                    idr.Close();
                    idr = null;
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
            }	//	Invoice

            //	DocumentType
            Boolean? paymentSO = null;
            sql = "SELECT IsSOTrx "
                + "FROM C_DocType "
                + "WHERE C_DocType_ID=" + GetC_DocType_ID();
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                if (idr.Read())
                    paymentSO = "Y".Equals(idr[0].ToString());
                idr.Close();
                idr = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            //	No Payment Info
            if (paymentSO == null)
                return false;
            SetIsReceipt((Boolean)paymentSO);

            //	We have an Invoice .. and it does not match
            if (invoiceSO != null
                    && (Boolean)invoiceSO != (Boolean)paymentSO)
                return false;
            //	OK
            return true;
        }


        /**
         * 	Set Invoice - Callout
         *	@param oldC_Invoice_ID old BP
         *	@param newC_Invoice_ID new BP
         *	@param windowNo window no
         */
        // @UICallout
        public void SetC_Invoice_ID(String oldC_Invoice_ID, String newC_Invoice_ID, int windowNo)
        {
            if (newC_Invoice_ID == null || newC_Invoice_ID.Length == 0)
                return;
            int C_Invoice_ID = int.Parse(newC_Invoice_ID);
            //  reSet as dependent fields Get reSet
            //p_changeVO.SetContext(GetCtx(), windowNo, "C_Invoice_ID", C_Invoice_ID);
            SetContext(windowNo, "C_Invoice_ID", C_Invoice_ID.ToString());
            SetC_Invoice_ID(C_Invoice_ID);
            if (C_Invoice_ID == 0)
                return;

            SetC_Order_ID(0);
            SetC_Charge_ID(0);
            SetC_Project_ID(0);
            SetIsPrepayment(false);
            //
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsOverUnderPayment(false);
            SetOverUnderAmt(Env.ZERO);

            int C_InvoicePaySchedule_ID = 0;
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_Invoice_ID") == C_Invoice_ID
            && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_InvoicePaySchedule_ID") != 0)
                C_InvoicePaySchedule_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_InvoicePaySchedule_ID");

            //  Payment Date
            DateTime? ts = GetDateTrx();
            if (ts == null)
                ts = DateTime.Now;
            //
            String sql = "SELECT C_BPartner_ID,C_Currency_ID,"		        //	1..2
                + " invoiceOpen(C_Invoice_ID, @paysch),"					//	3		#1
                + " invoiceDiscount(C_Invoice_ID,@tsdt,@paysch1), IsSOTrx "	//	4..5	#2/3
                + "FROM C_Invoice WHERE C_Invoice_ID=@invid";			    //			#4
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@paysch", C_InvoicePaySchedule_ID);
                param[1] = new SqlParameter("@tsdt", (DateTime?)ts);
                param[2] = new SqlParameter("@paysch1", C_InvoicePaySchedule_ID);
                param[3] = new SqlParameter("@invid", C_Invoice_ID);

                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    SetC_BPartner_ID(Utility.Util.GetValueOfInt(idr[0].ToString()));
                    int C_Currency_ID = Utility.Util.GetValueOfInt(idr[1].ToString());	//	Set Invoice Currency
                    SetC_Currency_ID(C_Currency_ID);
                    //
                    Decimal? invoiceOpen = Utility.Util.GetValueOfDecimal(idr[2]);		//	Set Invoice OPen Amount
                    if (invoiceOpen == null)
                        invoiceOpen = Env.ZERO;
                    Decimal? discountAmt = Utility.Util.GetValueOfDecimal(idr[3]);		//	Set Discount Amt
                    if (discountAmt == null)
                        discountAmt = Env.ZERO;
                    SetPayAmt(Decimal.Subtract(Utility.Util.GetValueOfDecimal(invoiceOpen), Utility.Util.GetValueOfDecimal(discountAmt)));
                    SetDiscountAmt((Decimal)discountAmt);
                    //IsSOTrx, Project
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            CheckDocType(windowNo);
        }

        /**
         * 	Set Order - Callout
         *	@param oldC_Order_ID old BP
         *	@param newC_Order_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetC_Order_ID(String oldC_Order_ID, String newC_Order_ID, int windowNo)
        {
            if (newC_Order_ID == null || newC_Order_ID.Length == 0)
                return;
            int C_Order_ID = int.Parse(newC_Order_ID);
            SetC_Order_ID(C_Order_ID);
            if (C_Order_ID == 0)
                return;
            //
            SetC_Invoice_ID(0);
            SetC_Charge_ID(0);
            SetC_Project_ID(0);
            SetIsPrepayment(true);
            //
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsOverUnderPayment(false);
            SetOverUnderAmt(Env.ZERO);
            //  Payment Date
            DateTime? ts = GetDateTrx();
            if (ts == null)
                ts = DateTime.Now;
            //
            String sql = "SELECT C_BPartner_ID,C_Currency_ID, GrandTotal, C_Project_ID "
                + "FROM C_Order WHERE C_Order_ID=" + C_Order_ID; 	// #1
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    SetC_BPartner_ID(Utility.Util.GetValueOfInt(idr[0].ToString()));
                    int C_Currency_ID = Utility.Util.GetValueOfInt(idr[1].ToString());	//	Set Order Currency
                    SetC_Currency_ID(C_Currency_ID);
                    //
                    Decimal? grandTotal = Utility.Util.GetValueOfDecimal(idr[2]);		//	Set Pay Amount
                    if (grandTotal == null)
                        grandTotal = Env.ZERO;
                    SetPayAmt(Utility.Util.GetValueOfDecimal(grandTotal));
                    SetC_Project_ID(Utility.Util.GetValueOfInt(idr[3].ToString()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            CheckDocType(windowNo);
        }

        /**
         * 	Set Charge - Callout
         *	@param oldC_Charge_ID old BP
         *	@param newC_Charge_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetC_Charge_ID(String oldC_Charge_ID, String newC_Charge_ID, int windowNo)
        {
            if (newC_Charge_ID == null || newC_Charge_ID.Length == 0)
                return;
            int C_Charge_ID = int.Parse(newC_Charge_ID);
            SetC_Charge_ID(C_Charge_ID);
            if (C_Charge_ID == 0)
                return;
            //
            SetC_Order_ID(0);
            SetC_Invoice_ID(0);
            SetC_Project_ID(0);
            SetIsPrepayment(true);
            SetIsReceipt(false);
            //
            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetIsOverUnderPayment(false);
            SetOverUnderAmt(Env.ZERO);
        }

        /**
         * 	Set Order - Callout
         *	@param oldC_DocType_ID old BP
         *	@param newC_DocType_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetC_DocType_ID(String oldC_DocType_ID, String newC_DocType_ID, int windowNo)
        {
            if (newC_DocType_ID == null || newC_DocType_ID.Length == 0)
                return;
            int C_DocType_ID = int.Parse(newC_DocType_ID);
            SetC_DocType_ID(C_DocType_ID);
            CheckDocType(windowNo);
        }

        /**
         * 	Check Document Type (Callout)
         *	@param windowNo windowNo no
         */
        private void CheckDocType(int windowNo)
        {
            int C_Invoice_ID = GetC_Invoice_ID();
            int C_Order_ID = GetC_Order_ID();
            int C_DocType_ID = GetC_DocType_ID();
            log.Fine("C_Invoice_ID=" + C_Invoice_ID + ", C_DocType_ID=" + C_DocType_ID);
            MDocType dt = null;
            if (C_DocType_ID != 0)
            {
                dt = MDocType.Get(GetCtx(), C_DocType_ID);
                SetIsReceipt(dt.IsSOTrx());
                //p_changeVO.SetContext(GetCtx(), windowNo, "IsSOTrx", dt.IsSOTrx());
                SetContext(windowNo, "IsSOTrx", dt.IsSOTrx());
            }
            //	Invoice
            if (C_Invoice_ID != 0)
            {
                MInvoice inv = new MInvoice(GetCtx(), C_Invoice_ID, null);
                if (dt != null)
                {
                    if (inv.IsSOTrx() != dt.IsSOTrx())
                    {
                        //p_changeVO.addError(Msg.GetMsg(GetCtx(),"PaymentDocTypeInvoiceInconsistent"));
                    }
                }
            }
            //	Order Waiting Payment (can only be SO)
            if (C_Order_ID != 0 && !dt.IsSOTrx())
            {
                //p_changeVO.addError(Msg.GetMsg(GetCtx(),"PaymentDocTypeInvoiceInconsistent"));
            }
        }

        /**
         * 	Set Rate - Callout.
         *	@param oldC_ConversionType_ID old
         *	@param newC_ConversionType_ID new
         *	@param windowNo window no
         */
        //@UICallout
        public void SetC_ConversionType_ID(String oldC_ConversionType_ID,
            String newC_ConversionType_ID, int windowNo)
        {
            if (newC_ConversionType_ID == null || newC_ConversionType_ID.Length == 0)
                return;
            int C_ConversionType_ID = int.Parse(newC_ConversionType_ID);
            SetC_ConversionType_ID(C_ConversionType_ID);
            if (C_ConversionType_ID == 0)
                return;
            CheckAmt(windowNo, "C_ConversionType_ID");
        }

        /**
         * 	Set Currency - Callout.
         *	@param oldC_Currency_ID old
         *	@param newC_Currency_ID new
         *	@param windowNo window no
         */
        //@UICallout
        public void SetC_Currency_ID(String oldC_Currency_ID, String newC_Currency_ID, int windowNo)
        {
            if (newC_Currency_ID == null || newC_Currency_ID.Length == 0)
                return;
            int C_Currency_ID = int.Parse(newC_Currency_ID);
            if (C_Currency_ID == 0)
                return;
            SetC_Currency_ID(C_Currency_ID);
            CheckAmt(windowNo, "C_Currency_ID");
        }


        /**
         * 	Set Discount - Callout
         *	@param oldDiscountAmt old value
         *	@param newDiscountAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetDiscountAmt(String oldDiscountAmt, String newDiscountAmt, int windowNo)
        {
            if (newDiscountAmt == null || newDiscountAmt.Length == 0)
                return;
            Decimal? discountAmt = PO.ConvertToBigDecimal(newDiscountAmt);
            SetDiscountAmt(discountAmt);
            CheckAmt(windowNo, "DiscountAmt");
        }

        /**
         * 	Set Is Over Under Payment - Callout
         *	@param oldIsOverUnderPayment old value
         *	@param newIsOverUnderPayment new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetIsOverUnderPayment(String oldIsOverUnderPayment,
                String newIsOverUnderPayment, int windowNo)
        {
            if (newIsOverUnderPayment == null || newIsOverUnderPayment.Length == 0)
                return;
            CheckAmt(windowNo, "IsOverUnderPayment");
            SetIsOverUnderPayment("Y".Equals(newIsOverUnderPayment));

        }

        /**
         * 	Set Over Under Amt - Callout
         *	@param oldOverUnderAmt old value
         *	@param newOverUnderAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        // @UICallout
        public void SetOverUnderAmt(String oldOverUnderAmt, String newOverUnderAmt, int windowNo)
        {
            if (newOverUnderAmt == null || newOverUnderAmt.Length == 0)
                return;
            Decimal? OverUnderAmt = PO.ConvertToBigDecimal(newOverUnderAmt);
            SetOverUnderAmt(OverUnderAmt);
            CheckAmt(windowNo, "OverUnderAmt");
        }

        /**
         * 	Set Pay Amt - Callout
         *	@param oldPayAmt old value
         *	@param newPayAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        // @UICallout
        public void SetPayAmt(String oldPayAmt, String newPayAmt, int windowNo)
        {
            if (newPayAmt == null || newPayAmt.Length == 0)
                return;
            Decimal? PayAmt = PO.ConvertToBigDecimal(newPayAmt);
            SetPayAmt(PayAmt);
            CheckAmt(windowNo, "PayAmt");
        }

        /**
         * 	Set WriteOff Amt - Callout
         *	@param oldWriteOffAmt old value
         *	@param newWriteOffAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetWriteOffAmt(String oldWriteOffAmt, String newWriteOffAmt, int windowNo)
        {
            if (newWriteOffAmt == null || newWriteOffAmt.Length == 0)
                return;
            Decimal? WriteOffAmt = PO.ConvertToBigDecimal(newWriteOffAmt);
            SetWriteOffAmt(WriteOffAmt);
            CheckAmt(windowNo, "WriteOffAmt");
        }

        /**
         * 	Set DateTrx - Callout
         *	@param oldDateTrx old
         *	@param newDateTrx new
         *	@param windowNo window no
         */
        //@UICallout
        public void SetDateTrx(String oldDateTrx, String newDateTrx, int windowNo)
        {
            if (newDateTrx == null || newDateTrx.Length == 0)
                return;
            DateTime? dateTrx = PO.ConvertToTimestamp(newDateTrx);
            if (dateTrx == null)
                return;
            SetDateTrx((DateTime?)dateTrx);
            SetDateAcct((DateTime?)dateTrx);
            CheckAmt(windowNo, "DateTrx");
        }

        /**
         * 	Check amount (Callout)
         *	@param windowNo window
         *	@param columnName columnName
         */
        private void CheckAmt(int windowNo, String columnName)
        {
            int C_Invoice_ID = GetC_Invoice_ID();
            //	New Payment
            if (GetC_Payment_ID() == 0
                && GetC_BPartner_ID() == 0
                && C_Invoice_ID == 0)
                return;
            int C_Currency_ID = GetC_Currency_ID();
            if (C_Currency_ID == 0)
                return;

            //	Changed Column
            if (columnName.Equals("IsOverUnderPayment")	//	Set Over/Under Amt to Zero
                || !IsOverUnderPayment())
                SetOverUnderAmt(Env.ZERO);

            int C_InvoicePaySchedule_ID = 0;
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_Invoice_ID") == C_Invoice_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_InvoicePaySchedule_ID") != 0)
                C_InvoicePaySchedule_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "C_InvoicePaySchedule_ID");

            //	Get Open Amount & Invoice Currency
            Decimal? invoiceOpenAmt = Env.ZERO;
            Decimal discountAmt = Env.ZERO;
            int C_Currency_Invoice_ID = 0;
            if (C_Invoice_ID != 0)
            {
                DateTime? ts = GetDateTrx();
                if (ts == null)
                    ts = DateTime.Now;
                String sql = "SELECT C_BPartner_ID,C_Currency_ID,"		        //	1..2
                    + " invoiceOpen(C_Invoice_ID, @paysch),"					//	3		#1
                    + " invoiceDiscount(C_Invoice_ID,@tsdt,@paysch1), IsSOTrx "	//	4..5	#2/3
                    + "FROM C_Invoice WHERE C_Invoice_ID=@invid";			    //			#4
                IDataReader idr = null;
                try
                {
                    SqlParameter[] param = new SqlParameter[4];
                    param[0] = new SqlParameter("@paysch", C_InvoicePaySchedule_ID);
                    param[1] = new SqlParameter("@tsdt", Convert.ToDateTime(ts));
                    param[2] = new SqlParameter("@paysch1", C_InvoicePaySchedule_ID);
                    param[3] = new SqlParameter("@invid", C_Invoice_ID);
                    idr = DataBase.DB.ExecuteReader(sql, null, null);
                    if (idr.Read())
                    {
                        C_Currency_Invoice_ID = Utility.Util.GetValueOfInt(idr[1].ToString());
                        invoiceOpenAmt = idr.GetDecimal(2);		//	Set Invoice Open Amount
                        if (invoiceOpenAmt == null)
                            invoiceOpenAmt = Env.ZERO;
                        discountAmt = idr.GetDecimal(3);
                    }
                    idr.Close();
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
            }	//	Get Invoice Info
            log.Fine("Open=" + invoiceOpenAmt + " Discount= " + discountAmt
                + ", C_Invoice_ID=" + C_Invoice_ID
               + ", C_Currency_ID=" + C_Currency_Invoice_ID);

            //	Get Info from Tab
            Decimal payAmt = GetPayAmt();
            Decimal writeOffAmt = GetWriteOffAmt();
            Decimal overUnderAmt = GetOverUnderAmt();
            Decimal enteredDiscountAmt = GetDiscountAmt();
            log.Fine("Pay=" + payAmt + ", Discount=" + enteredDiscountAmt
               + ", WriteOff=" + writeOffAmt + ", OverUnderAmt=" + overUnderAmt);
            //	Get Currency Info
            MCurrency currency = MCurrency.Get(GetCtx(), C_Currency_ID);
            DateTime? convDate = GetDateTrx();
            int C_ConversionType_ID = GetC_ConversionType_ID();
            int AD_Client_ID = GetAD_Client_ID();
            int AD_Org_ID = GetAD_Org_ID();
            //	Get Currency Rate
            Decimal currencyRate = Env.ONE;
            if ((C_Currency_ID > 0 && C_Currency_Invoice_ID > 0 &&
                C_Currency_ID != C_Currency_Invoice_ID)
                || columnName.Equals("C_Currency_ID") || columnName.Equals("C_ConversionType_ID"))
            {
                log.Fine("InvCurrency=" + C_Currency_Invoice_ID
                    + ", PayCurrency=" + C_Currency_ID
                    + ", Date=" + convDate + ", Type=" + C_ConversionType_ID);
                currencyRate = MConversionRate.GetRate(C_Currency_Invoice_ID, C_Currency_ID,
                    convDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
                if (currencyRate.CompareTo(Env.ZERO) == 0)
                {
                    //	mTab.SetValue("C_Currency_ID", new Integer(C_Currency_Invoice_ID));	//	does not work
                    if (C_Currency_Invoice_ID != 0)
                    {
                        //p_changeVO.addError(Msg.GetMsg(GetCtx(),"NoCurrencyConversion"));
                    }
                    return;
                }
                //
                invoiceOpenAmt = Decimal.Round(Decimal.Multiply((Decimal)invoiceOpenAmt, currencyRate),
                    currency.GetStdPrecision(), MidpointRounding.AwayFromZero);
                discountAmt = Decimal.Round(Decimal.Multiply(discountAmt, currencyRate),
                    currency.GetStdPrecision(), MidpointRounding.AwayFromZero);
                log.Fine("Rate=" + currencyRate + ", InvoiceOpenAmt=" + invoiceOpenAmt + ", DiscountAmt=" + discountAmt);
            }

            //	Currency Changed - convert all
            if (columnName.Equals("C_Currency_ID") || columnName.Equals("C_ConversionType_ID"))
            {

                writeOffAmt = Decimal.Round(Decimal.Multiply(writeOffAmt, currencyRate),
                    currency.GetStdPrecision(), MidpointRounding.AwayFromZero);
                SetWriteOffAmt(writeOffAmt);
                overUnderAmt = Decimal.Round(Decimal.Multiply(overUnderAmt, currencyRate),
                    currency.GetStdPrecision(), MidpointRounding.AwayFromZero);
                SetOverUnderAmt(overUnderAmt);

                // Entered Discount amount should be converted to entered currency 
                enteredDiscountAmt = Decimal.Round(Decimal.Multiply(enteredDiscountAmt, currencyRate),
                    currency.GetStdPrecision(), MidpointRounding.AwayFromZero);
                SetDiscountAmt(enteredDiscountAmt);

                //PayAmt = InvoiceOpenAmt.subtract(DiscountAmt).subtract(WriteOffAmt).subtract(OverUnderAmt);
                payAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract((Decimal)invoiceOpenAmt, discountAmt), writeOffAmt), overUnderAmt);
                SetPayAmt(payAmt);
            }

            //	No Invoice - Set Discount, Witeoff, Under/Over to 0
            else if (C_Invoice_ID == 0)
            {
                if (Env.Signum(discountAmt) != 0)
                    SetDiscountAmt(Env.ZERO);
                if (Env.Signum(writeOffAmt) != 0)
                    SetWriteOffAmt(Env.ZERO);
                if (Env.Signum(overUnderAmt) != 0)
                    SetOverUnderAmt(Env.ZERO);
            }
            //  PayAmt - calculate write off
            else if (columnName.Equals("PayAmt"))
            {
                //WriteOffAmt = InvoiceOpenAmt.subtract(PayAmt).subtract(DiscountAmt).subtract(OverUnderAmt);
                writeOffAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract((Decimal)invoiceOpenAmt, payAmt), discountAmt), overUnderAmt);
                if (Env.ZERO.CompareTo(writeOffAmt) > 0)
                {
                    if (Math.Abs(writeOffAmt).CompareTo(discountAmt) <= 0)
                        discountAmt = Decimal.Add(discountAmt, writeOffAmt);
                    else
                        discountAmt = Env.ZERO;
                    //WriteOffAmt = InvoiceOpenAmt.subtract(PayAmt).subtract(DiscountAmt).subtract(OverUnderAmt);
                    writeOffAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract((Decimal)invoiceOpenAmt, payAmt), discountAmt), overUnderAmt);
                }
                SetDiscountAmt(discountAmt);
                SetWriteOffAmt(writeOffAmt);
            }
            else    //  calculate PayAmt
            {
                /* Allow reduction in discount, but not an increase. To give a discount that is higher
                   than the calculated discount, users have to enter a write off */
                if (enteredDiscountAmt.CompareTo(discountAmt) < 0)
                    discountAmt = enteredDiscountAmt;
                //PayAmt = invoiceOpenAmt.subtract(discountAmt).subtract(WriteOffAmt).subtract(OverUnderAmt);
                payAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract((Decimal)invoiceOpenAmt, discountAmt), writeOffAmt), overUnderAmt);
                SetPayAmt(payAmt);
                SetDiscountAmt(discountAmt);
            }
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
         * 	Get Document Status
         *	@return Document Status Clear Text
         */
        public String GetDocStatusName()
        {
            return MRefList.GetListName(GetCtx(), 131, GetDocStatus());
        }

        /**
         *	Get Name of Credit Card
         *	@return Name
         */
        public String GetCreditCardName()
        {
            return GetCreditCardName(GetCreditCardType());
        }

        /**
         *	Get Name of Credit Card
         * 	@param CreditCardType credit card type
         *	@return Name
         */
        public String GetCreditCardName(String creditCardType)
        {
            if (creditCardType == null)
                return "--";
            else if (CREDITCARDTYPE_MasterCard.Equals(creditCardType))
                return "MasterCard";
            else if (CREDITCARDTYPE_Visa.Equals(creditCardType))
                return "Visa";
            else if (CREDITCARDTYPE_Amex.Equals(creditCardType))
                return "Amex";
            else if (CREDITCARDTYPE_ATM.Equals(creditCardType))
                return "ATM";
            else if (CREDITCARDTYPE_Diners.Equals(creditCardType))
                return "Diners";
            else if (CREDITCARDTYPE_Discover.Equals(creditCardType))
                return "Discover";
            else if (CREDITCARDTYPE_PurchaseCard.Equals(creditCardType))
                return "PurchaseCard";
            return "?" + creditCardType + "?";
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
         * 	Get Pay Amt
         * 	@param absolute if true the absolute amount (i.e. negative if payment)
         *	@return amount
         */
        public Decimal GetPayAmt(bool absolute)
        {
            if (IsReceipt())
                return base.GetPayAmt();
            return Decimal.Negate(base.GetPayAmt());
        }

        /**
         * 	Get Pay Amt in cents
         *	@return amount in cents
         */
        public int GetPayAmtInCents()
        {
            Decimal bd = Decimal.Multiply(base.GetPayAmt(), Env.ONEHUNDRED);
            return Decimal.ToInt32(bd);
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
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
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
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetDateAcct(),
                IsReceipt() ? MDocBaseType.DOCBASETYPE_ARRECEIPT : MDocBaseType.DOCBASETYPE_APPAYMENT))
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


            //	Unsuccessful Online Payment
            if (IsOnline() && !IsApproved())
            {
                if (GetR_Result() != null)
                    _processMsg = "@OnlinePaymentFailed@";
                else
                    _processMsg = "@PaymentNotProcessed@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	Waiting Payment - Need to create Invoice & Shipment
            if (GetC_Order_ID() != 0 && GetC_Invoice_ID() == 0)
            {	//	see WebOrder.process
                MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_Trx());
                if (DOCSTATUS_WaitingPayment.Equals(order.GetDocStatus()))
                {
                    order.SetC_Payment_ID(GetC_Payment_ID());
                    order.SetDocAction(X_C_Order.DOCACTION_WaitComplete);
                    order.Set_TrxName(Get_Trx());
                    //	Boolean ok = 
                    order.ProcessIt(X_C_Order.DOCACTION_WaitComplete);
                    _processMsg = order.GetProcessMsg();
                    order.Save(Get_Trx());


                    /******************Commented By Lakhwinder
                     * //// Payment was Not Completed Against Prepay Order//////////*
                    //	Set Invoice
                    MInvoice[] invoices = order.GetInvoices(true);
                    int length = invoices.Length;
                    if (length > 0)		//	Get last invoice
                        SetC_Invoice_ID(invoices[length - 1].GetC_Invoice_ID());
                    //
                    if (GetC_Invoice_ID() == 0)
                    {
                        _processMsg = "@NotFound@ @C_Invoice_ID@";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    */
                    ////////////////////////
                }	//	WaitingPayment
            }

            //	Consistency of Invoice / Document Type and IsReceipt
            if (!VerifyDocType())
            {
                _processMsg = "@PaymentDocTypeInvoiceInconsistent@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	Do not pay when Credit Stop/Hold
            if (!IsReceipt())
            {
                MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
                if (X_C_BPartner.SOCREDITSTATUS_CreditStop.Equals(bp.GetSOCreditStatus()))
                {
                    _processMsg = "@BPartnerCreditStop@ - @TotalOpenBalance@="
                        + bp.GetTotalOpenBalance()
                        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    return DocActionVariables.STATUS_INVALID;
                }
                if (X_C_BPartner.SOCREDITSTATUS_CreditHold.Equals(bp.GetSOCreditStatus()))
                {
                    _processMsg = "@BPartnerCreditHold@ - @TotalOpenBalance@="
                        + bp.GetTotalOpenBalance()
                        + ", @SO_CreditLimit@=" + bp.GetSO_CreditLimit();
                    return DocActionVariables.STATUS_INVALID;
                }
            }

            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
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

            //	Charge Handling
            if (GetC_Charge_ID() != 0)
            {
                if (!IsPrepayment())
                    SetIsAllocated(true);
            }
            else
            {
                AllocateIt();	//	Create Allocation Records
                TestAllocation();
            }

            //	Project update
            if (GetC_Project_ID() != 0)
            {
                //	MProject project = new MProject(GetCtx(), GetC_Project_ID());
            }
            //	Update BP for Prepayments
            if (GetC_BPartner_ID() != 0 && GetC_Invoice_ID() == 0)
            {
                MBPartner bp1 = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
                bp1.SetTotalOpenBalance();
                bp1.Save();
            }

            //	Counter Doc
            MPayment counter = CreateCounterDoc();
            if (counter != null)
                _processMsg += " @CounterDoc@: @C_Payment_ID@=" + counter.GetDocumentNo();

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            // When Tender Type Is Check. Account Date Must Be Same As Check Date
            if (GetTenderType() == "K")
            {
                if (GetDateAcct() != GetCheckDate())
                {
                    _processMsg = "Account Date and Check Date must be same";
                    return DocActionVariables.STATUS_INVALID;
                }
            }
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);

            // nnayak - update BP open balance and credit used
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
            Decimal? payAmt = MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(GetPayAmt(false), GetDiscountAmt()), GetWriteOffAmt()),
                GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
            if (payAmt == null)
            {
                _processMsg = "Could not convert C_Currency_ID=" + GetC_Currency_ID()
                    + " to base C_Currency_ID=" + MClient.Get(GetCtx()).GetC_Currency_ID();
                return DocActionVariables.STATUS_INVALID;
            }
            //	Total Balance
            Decimal? newBalance = bp.GetTotalOpenBalance(false);
            if (newBalance == null)
                newBalance = Env.ZERO;
            if (IsReceipt())
            {
                newBalance = Decimal.Subtract((Decimal)newBalance, (Decimal)payAmt);

                Decimal? newCreditAmt = bp.GetSO_CreditUsed();
                if (newCreditAmt == null)
                    newCreditAmt = Decimal.Negate((Decimal)payAmt);
                else
                    newCreditAmt = Decimal.Subtract((Decimal)newCreditAmt, (Decimal)payAmt);
                //
                log.Fine("TotalOpenBalance=" + bp.GetTotalOpenBalance(false) + "(" + payAmt
                    + ", Credit=" + bp.GetSO_CreditUsed() + "->" + newCreditAmt
                    + ", Balance=" + bp.GetTotalOpenBalance(false) + " -> " + newBalance);
                bp.SetSO_CreditUsed((Decimal)newCreditAmt);
            }	//	SO
            else
            {
                newBalance = Decimal.Subtract((Decimal)newBalance, (Decimal)payAmt);
                log.Fine("Payment Amount =" + GetPayAmt(false) + "(" + payAmt
                    + ") Balance=" + bp.GetTotalOpenBalance(false) + " -> " + newBalance);
            }
            bp.SetTotalOpenBalance(Convert.ToDecimal(newBalance));
            bp.SetSOCreditStatus();
            if (!bp.Save(Get_Trx()))
            {
                _processMsg = "Could not update Business Partner";
                return DocActionVariables.STATUS_INVALID;
            }
            if (GetC_InvoicePaySchedule_ID() != 0)
            {
                MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), GetC_InvoicePaySchedule_ID(), Get_Trx());
                paySch.SetC_Payment_ID(GetC_Payment_ID());
                paySch.Save();
            }
            else
            {
                int[] InvoicePaySchedule_ID = MInvoicePaySchedule.GetAllIDs("C_InvoicePaySchedule", "C_Invoice_ID = " + GetC_Invoice_ID() + @" AND C_InvoicePaySchedule_ID NOT IN 
                    (SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Payment_ID IN (SELECT NVL(C_Payment_ID,0) FROM C_InvoicePaySchedule) UNION 
                    SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule  WHERE C_Cashline_ID IN (SELECT NVL(C_Cashline_ID,0) FROM C_InvoicePaySchedule))", Get_Trx());
                foreach (int invocePay in InvoicePaySchedule_ID)
                {
                    MInvoicePaySchedule paySch = new MInvoicePaySchedule(GetCtx(), invocePay, Get_Trx());
                    paySch.SetC_Payment_ID(GetC_Payment_ID());
                    paySch.Save();
                }
            }
            return DocActionVariables.STATUS_COMPLETED;
        }

        /**
         * 	Create Counter Document
         * 	@return payment
         */
        private MPayment CreateCounterDoc()
        {
            //	Is this a counter doc ?
            if (GetRef_Payment_ID() != 0)
                return null;

            //	Org Must be linked to BPartner
            MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
            //jz int counterC_BPartner_ID = org.GetLinkedC_BPartner_ID(); 
            int counterC_BPartner_ID = org.GetLinkedC_BPartner_ID(Get_Trx());
            if (counterC_BPartner_ID == 0)
                return null;
            //	Business Partner needs to be linked to Org
            //jz MBPartner bp = new MBPartner (GetCtx(), GetC_BPartner_ID(), null);
            MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
            int counterAD_Org_ID = bp.GetAD_OrgBP_ID_Int();
            if (counterAD_Org_ID == 0)
                return null;

            //jz MBPartner counterBP = new MBPartner (GetCtx(), counterC_BPartner_ID, null);
            MBPartner counterBP = new MBPartner(GetCtx(), counterC_BPartner_ID, Get_Trx());
            //	MOrgInfo counterOrgInfo = MOrgInfo.Get(GetCtx(), counterAD_Org_ID);
            log.Info("Counter BP=" + counterBP.GetName());

            //	Document Type
            int C_DocTypeTarGet_ID = 0;
            MDocTypeCounter counterDT = MDocTypeCounter.GetCounterDocType(GetCtx(), GetC_DocType_ID());
            if (counterDT != null)
            {
                log.Fine(counterDT.ToString());
                if (!counterDT.IsCreateCounter() || !counterDT.IsValid())
                    return null;
                C_DocTypeTarGet_ID = counterDT.GetCounter_C_DocType_ID();
            }
            else	//	indirect
            {
                C_DocTypeTarGet_ID = MDocTypeCounter.GetCounterDocType_ID(GetCtx(), GetC_DocType_ID());
                log.Fine("Indirect C_DocTypeTarGet_ID=" + C_DocTypeTarGet_ID);
                if (C_DocTypeTarGet_ID <= 0)
                    return null;
            }

            //	Deep Copy
            MPayment counter = new MPayment(GetCtx(), 0, Get_Trx());
            counter.SetAD_Org_ID(counterAD_Org_ID);
            counter.SetC_BPartner_ID(counterBP.GetC_BPartner_ID());
            counter.SetIsReceipt(!IsReceipt());
            counter.SetC_DocType_ID(C_DocTypeTarGet_ID);
            counter.SetTrxType(GetTrxType());
            counter.SetTenderType(GetTenderType());
            //
            counter.SetPayAmt(GetPayAmt());
            counter.SetDiscountAmt(GetDiscountAmt());
            counter.SetTaxAmt(GetTaxAmt());
            counter.SetWriteOffAmt(GetWriteOffAmt());
            counter.SetIsOverUnderPayment(IsOverUnderPayment());
            counter.SetOverUnderAmt(GetOverUnderAmt());
            counter.SetC_Currency_ID(GetC_Currency_ID());
            counter.SetC_ConversionType_ID(GetC_ConversionType_ID());
            //
            counter.SetDateTrx(GetDateTrx());
            counter.SetDateAcct(GetDateAcct());
            counter.SetRef_Payment_ID(GetC_Payment_ID());
            //
            String sql = "SELECT C_BankAccount_ID FROM C_BankAccount "
                + "WHERE C_Currency_ID=" + GetC_Currency_ID() + " AND AD_Org_ID IN (0," + counterAD_Org_ID + ") AND IsActive='Y' "
                + "ORDER BY IsDefault DESC";
            int C_BankAccount_ID = DataBase.DB.GetSQLValue(Get_Trx(), sql);
            counter.SetC_BankAccount_ID(C_BankAccount_ID);

            //	Refernces
            counter.SetC_Activity_ID(GetC_Activity_ID());
            counter.SetC_Campaign_ID(GetC_Campaign_ID());
            counter.SetC_Project_ID(GetC_Project_ID());
            counter.SetUser1_ID(GetUser1_ID());
            counter.SetUser2_ID(GetUser2_ID());
            counter.Save(Get_Trx());
            log.Fine(counter.ToString());
            SetRef_Payment_ID(counter.GetC_Payment_ID());

            //	Document Action
            if (counterDT != null)
            {
                if (counterDT.GetDocAction() != null)
                {
                    counter.SetDocAction(counterDT.GetDocAction());
                    counter.ProcessIt(counterDT.GetDocAction());
                    counter.Save(Get_Trx());
                }
            }
            return counter;
        }

        /**
         * 	Allocate It.
         * 	Only call when there is NO allocation as it will create duplicates.
         * 	If an invoice exists, it allocates that 
         * 	otherwise it allocates Payment Selection.
         *	@return true if allocated
         */
        public bool AllocateIt()
        {
            //	Create invoice Allocation -	See also MCash.completeIt
            if (GetC_Invoice_ID() != 0)
                return AllocateInvoice();
            //	Invoices of a AP Payment Selection
            if (AllocatePaySelection())
                return true;

            if (GetC_Order_ID() != 0)
                return false;

            //	Allocate to multiple Payments based on entry
            MPaymentAllocate[] pAllocs = MPaymentAllocate.Get(this);
            if (pAllocs.Length == 0)
                return false;

            MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false,
                GetDateTrx(), GetC_Currency_ID(),
                    Msg.Translate(GetCtx(), "C_Payment_ID") + ": " + GetDocumentNo(),
                    Get_TrxName());
            alloc.SetAD_Org_ID(GetAD_Org_ID());
            if (!alloc.Save())
            {
                log.Severe("P.Allocations not created");
                return false;
            }
            //	Lines
            for (int i = 0; i < pAllocs.Length; i++)
            {
                MPaymentAllocate pa = pAllocs[i];
                MAllocationLine aLine = null;
                if (IsReceipt())
                    aLine = new MAllocationLine(alloc, pa.GetAmount(),
                        pa.GetDiscountAmt(), pa.GetWriteOffAmt(), pa.GetOverUnderAmt());
                else
                    aLine = new MAllocationLine(alloc, Decimal.Negate(pa.GetAmount()),
                        Decimal.Negate(pa.GetDiscountAmt()), Decimal.Negate(pa.GetWriteOffAmt()), Decimal.Negate(pa.GetOverUnderAmt()));
                aLine.SetDocInfo(pa.GetC_BPartner_ID(), 0, pa.GetC_Invoice_ID());
                aLine.SetPaymentInfo(GetC_Payment_ID(), 0);
                if (!aLine.Save(Get_TrxName()))
                {
                    log.Warning("P.Allocations - line not saved");
                }
                else
                {
                    pa.SetC_AllocationLine_ID(aLine.GetC_AllocationLine_ID());
                    pa.Save();
                }
            }
            //	Should start WF
            alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
            _processMsg = "@C_AllocationHdr_ID@: " + alloc.GetDocumentNo();
            return alloc.Save(Get_TrxName());
        }

        /**
         * 	Allocate single AP/AR Invoice
         * 	@return true if allocated
         */
        private bool AllocateInvoice()
        {
            try
            {
                //	calculate actual allocation
                Decimal allocationAmt = GetPayAmt();			//	underpayment
                if (Env.Signum(GetOverUnderAmt()) < 0 && Env.Signum(GetPayAmt()) > 0)
                    allocationAmt = Decimal.Add(allocationAmt, GetOverUnderAmt());	//	overpayment (negative)

                MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false,
                    GetDateTrx(), GetC_Currency_ID(),
                    Msg.Translate(GetCtx(), "C_Payment_ID") + ": " + GetDocumentNo() + " [1]", Get_TrxName());

                alloc.SetAD_Org_ID(GetAD_Org_ID());
                if (!alloc.Save())
                {
                    log.Log(Level.SEVERE, "Could not create Allocation Hdr");
                    return false;
                }
                MAllocationLine aLine = null;
                if (IsReceipt())
                    aLine = new MAllocationLine(alloc, allocationAmt,
                        GetDiscountAmt(), GetWriteOffAmt(), GetOverUnderAmt());
                else
                    aLine = new MAllocationLine(alloc, Decimal.Negate(allocationAmt),
                        Decimal.Negate(GetDiscountAmt()), Decimal.Negate(GetWriteOffAmt()), Decimal.Negate(GetOverUnderAmt()));
                aLine.SetDocInfo(GetC_BPartner_ID(), 0, GetC_Invoice_ID());
                aLine.SetC_Payment_ID(GetC_Payment_ID());
                if (!aLine.Save(Get_TrxName()))
                {
                    log.Log(Level.SEVERE, "Could not create Allocation Line");
                    return false;
                }
                //	Should start WF
                alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                alloc.Save(Get_Trx());
                //_processMsg = "@C_AllocationHdr_ID@: " + alloc.GetDocumentNo();
                _processMsg = " View @C_AllocationHdr_ID@ created successfully with doc no: " + alloc.GetDocumentNo();
                //	Get Project from Invoice
                int C_Project_ID = DataBase.DB.GetSQLValue(Get_Trx(),
                    "SELECT MAX(C_Project_ID) FROM C_Invoice WHERE C_Invoice_ID=@param1", GetC_Invoice_ID());
                if (C_Project_ID > 0 && GetC_Project_ID() == 0)
                {
                    SetC_Project_ID(C_Project_ID);
                }
                else if (C_Project_ID > 0 && GetC_Project_ID() > 0 && C_Project_ID != GetC_Project_ID())
                {
                    log.Warning("Invoice C_Project_ID=" + C_Project_ID
                        + " <> Payment C_Project_ID=" + GetC_Project_ID());
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("MPayment-Error in AllocateInvoice");
                log.Severe(ex.ToString());
            }
            return true;
        }

        /**
         * 	Allocate Payment Selection
         * 	@return true if allocated
         */
        private Boolean AllocatePaySelection()
        {
            MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false,
                GetDateTrx(), GetC_Currency_ID(),
                Msg.Translate(GetCtx(), "C_Payment_ID") + ": " + GetDocumentNo() + " [n]", Get_Trx());
            alloc.SetAD_Org_ID(GetAD_Org_ID());

            String sql = "SELECT psc.C_BPartner_ID, psl.C_Invoice_ID, psl.IsSOTrx, "	//	1..3
                + " psl.PayAmt, psl.DiscountAmt, psl.DifferenceAmt, psl.OpenAmt "
                + "FROM C_PaySelectionLine psl"
                + " INNER JOIN C_PaySelectionCheck psc ON (psl.C_PaySelectionCheck_ID=psc.C_PaySelectionCheck_ID) "
                + "WHERE psc.C_Payment_ID=" + GetC_Payment_ID();
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
                while (idr.Read())
                {
                    int C_BPartner_ID = Utility.Util.GetValueOfInt(idr[0].ToString());
                    int C_Invoice_ID = Utility.Util.GetValueOfInt(idr[1].ToString());
                    if (C_BPartner_ID == 0 && C_Invoice_ID == 0)
                        continue;
                    Boolean isSOTrx = "Y".Equals(idr[2].ToString());
                    Decimal payAmt = Utility.Util.GetValueOfDecimal(idr[3]);
                    Decimal discountAmt = Utility.Util.GetValueOfDecimal(idr[4]);
                    Decimal writeOffAmt = Utility.Util.GetValueOfDecimal(idr[5]);
                    Decimal openAmt = Utility.Util.GetValueOfDecimal(idr[6]);
                    Decimal overUnderAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Add(openAmt, payAmt),
                        discountAmt), writeOffAmt);
                    //
                    if (alloc.Get_ID() == 0 && !alloc.Save(Get_Trx()))
                    {
                        log.Log(Level.SEVERE, "Could not create Allocation Hdr");
                        idr.Close();
                        return false;
                    }
                    MAllocationLine aLine = null;
                    if (isSOTrx)
                        aLine = new MAllocationLine(alloc, payAmt,
                            discountAmt, writeOffAmt, overUnderAmt);
                    else
                        aLine = new MAllocationLine(alloc, Decimal.Negate(payAmt),
                            Decimal.Negate(discountAmt), Decimal.Negate(writeOffAmt), Decimal.Negate(overUnderAmt));
                    aLine.SetDocInfo(C_BPartner_ID, 0, C_Invoice_ID);
                    aLine.SetC_Payment_ID(GetC_Payment_ID());
                    if (!aLine.Save(Get_Trx()))
                    {
                        log.Log(Level.SEVERE, "Could not create Allocation Line");
                    }
                }
                idr.Close();
                idr = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "allocatePaySelection", e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                idr = null;
            }


            //	Should start WF
            Boolean ok = true;
            if (alloc.Get_ID() == 0)
            {
                log.Fine("No Allocation created - C_Payment_ID="
                   + GetC_Payment_ID());
                ok = false;
            }
            else
            {
                alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
                ok = alloc.Save(Get_Trx());
                _processMsg = "@C_AllocationHdr_ID@: " + alloc.GetDocumentNo();
            }
            return ok;
        }

        /**
         * 	De-allocate Payment.
         * 	Unkink Invoices and Orders and delete Allocations
         */
        private void DeAllocate()
        {
            if (GetC_Order_ID() != 0)
                SetC_Order_ID(0);
            //	if (GetC_Invoice_ID() == 0)
            //		return;
            //	De-Allocate all 
            MAllocationHdr[] allocations = MAllocationHdr.GetOfPayment(GetCtx(),
                GetC_Payment_ID(), Get_Trx());
            log.Fine("#" + allocations.Length);
            for (int i = 0; i < allocations.Length; i++)
            {
                allocations[i].Set_TrxName(Get_Trx());
                allocations[i].SetDocAction(DocActionVariables.ACTION_REVERSE_CORRECT);
                allocations[i].ProcessIt(DocActionVariables.ACTION_REVERSE_CORRECT);
                allocations[i].Save();
            }

            // 	Unlink (in case allocation did not Get it)
            if (GetC_Invoice_ID() != 0)
            {
                //	Invoice					
                String sql = "UPDATE C_Invoice "
                    + "SET C_Payment_ID = NULL, IsPaid='N' "
                    + "WHERE C_Invoice_ID=" + GetC_Invoice_ID()
                    + " AND C_Payment_ID=" + GetC_Payment_ID();
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                if (no != 0)
                {
                    log.Fine("Unlink Invoice #" + no);
                }
                //	Order
                sql = "UPDATE C_Order o "
                    + "SET C_Payment_ID = NULL "
                    + "WHERE EXISTS (SELECT * FROM C_Invoice i "
                        + "WHERE o.C_Order_ID=i.C_Order_ID AND i.C_Invoice_ID=" + GetC_Invoice_ID() + ")"
                    + " AND C_Payment_ID=" + GetC_Payment_ID();
                no = DataBase.DB.ExecuteQuery(sql, null, Get_Trx());
                if (no != 0)
                {
                    log.Fine("Unlink Order #" + no);
                }
            }
            //
            SetC_Invoice_ID(0);
            SetIsAllocated(false);
        }

        /**
         * 	Void Document.
         * 	@return true if success 
         */
        public Boolean VoidIt()
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
            //	If on Bank Statement, don't void it - reverse it
            if (GetC_BankStatementLine_ID() > 0)
                return ReverseCorrectIt();

            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                AddDescription(Msg.GetMsg(GetCtx(), "Voided") + " (" + GetPayAmt() + ")");
                SetPayAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetWriteOffAmt(Env.ZERO);
                SetOverUnderAmt(Env.ZERO);
                SetIsAllocated(false);
                //	Unlink & De-Allocate
                DeAllocate();
            }
            else
                return ReverseCorrectIt();

            //
            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Close Document.
         * 	@return true if success 
         */
        public Boolean CloseIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return true if success 
         */
        public Boolean ReverseCorrectIt()
        {
            log.Info(ToString());

            //	Std Period open?
            DateTime? dateAcct = GetDateAcct();
            if (!MPeriod.IsOpen(GetCtx(), dateAcct,
                IsReceipt() ? MDocBaseType.DOCBASETYPE_ARRECEIPT : MDocBaseType.DOCBASETYPE_APPAYMENT))
                dateAcct = DateTime.Now;

            //	Auto Reconcile if not on Bank Statement
            Boolean reconciled = false; //	GetC_BankStatementLine_ID() == 0;

            //	Create Reversal
            MPayment reversal = new MPayment(GetCtx(), 0, Get_Trx());
            CopyValues(this, reversal);
            reversal.SetClientOrg(this);
            reversal.SetC_Order_ID(0);
            reversal.SetC_Invoice_ID(0);
            reversal.SetDateAcct(dateAcct);
            //
            reversal.SetDocumentNo(GetDocumentNo() + REVERSE_INDICATOR);	//	indicate reversals
            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            //
            reversal.SetPayAmt(Decimal.Negate(GetPayAmt()));
            reversal.SetDiscountAmt(Decimal.Negate(GetDiscountAmt()));
            reversal.SetWriteOffAmt(Decimal.Negate(GetWriteOffAmt()));
            reversal.SetOverUnderAmt(Decimal.Negate(GetOverUnderAmt()));
            //
            reversal.SetIsAllocated(true);
            reversal.SetIsReconciled(reconciled);	//	to put on bank statement
            reversal.SetIsOnline(false);
            reversal.SetIsApproved(true);
            reversal.SetR_PnRef(null);
            reversal.SetR_Result(null);
            reversal.SetR_RespMsg(null);
            reversal.SetR_AuthCode(null);
            reversal.SetR_Info(null);
            reversal.SetProcessing(false);
            reversal.SetOProcessing("N");
            reversal.SetProcessed(false);
            reversal.SetPosted(false);
            reversal.SetDescription(GetDescription());
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            reversal.Save(Get_Trx());
            //	Post Reversal
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            reversal.CloseIt();
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save(Get_Trx());

            //	Unlink & De-Allocate
            DeAllocate();
            SetIsReconciled(reconciled);
            SetIsAllocated(true);	//	the allocation below is overwritten
            //	Set Status 
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");
            SetDocStatus(DOCSTATUS_Reversed);
            SetDocAction(DOCACTION_None);
            SetProcessed(true);

            //	Create automatic Allocation
            MAllocationHdr alloc = new MAllocationHdr(GetCtx(), false,
                GetDateTrx(), GetC_Currency_ID(),
                Msg.Translate(GetCtx(), "C_Payment_ID") + ": " + reversal.GetDocumentNo(), Get_Trx());
            alloc.SetAD_Org_ID(GetAD_Org_ID());
            if (!alloc.Save())
            {
                log.Warning("Automatic allocation - hdr not saved");
            }
            else
            {
                //	Original Allocation
                MAllocationLine aLine = new MAllocationLine(alloc, GetPayAmt(true),
                    Env.ZERO, Env.ZERO, Env.ZERO);
                aLine.SetDocInfo(GetC_BPartner_ID(), 0, 0);
                aLine.SetPaymentInfo(GetC_Payment_ID(), 0);
                if (!aLine.Save(Get_Trx()))
                {
                    log.Warning("Automatic allocation - line not saved");
                }
                //	Reversal Allocation
                aLine = new MAllocationLine(alloc, reversal.GetPayAmt(true),
                    Env.ZERO, Env.ZERO, Env.ZERO);
                aLine.SetDocInfo(reversal.GetC_BPartner_ID(), 0, 0);
                aLine.SetPaymentInfo(reversal.GetC_Payment_ID(), 0);
                if (!aLine.Save(Get_Trx()))
                {
                    log.Warning("Automatic allocation - reversal line not saved");
                }
            }
            alloc.ProcessIt(DocActionVariables.ACTION_COMPLETE);
            alloc.Save(Get_Trx());
            //
            StringBuilder Info = new StringBuilder(reversal.GetDocumentNo());
            Info.Append(" - @C_AllocationHdr_ID@: ").Append(alloc.GetDocumentNo());

            //	Update BPartner
            if (GetC_BPartner_ID() != 0)
            {

                // nnayak - update BP open balance and credit used
                MBPartner bp = new MBPartner(GetCtx(), GetC_BPartner_ID(), Get_Trx());
                Decimal? payAmt = MConversionRate.ConvertBase(GetCtx(), Decimal.Add(Decimal.Add(GetPayAmt(false), GetDiscountAmt()), GetWriteOffAmt()),	//	CM adjusted 
                    GetC_Currency_ID(), GetDateAcct(), 0, GetAD_Client_ID(), GetAD_Org_ID());
                if (payAmt == null)
                {
                    _processMsg = "Could not convert C_Currency_ID=" + GetC_Currency_ID()
                        + " to base C_Currency_ID=" + MClient.Get(GetCtx()).GetC_Currency_ID();
                    return false;
                }
                //	Total Balance
                Decimal newBalance = bp.GetTotalOpenBalance(false);
                //if (newBalance == null)
                //    newBalance = Env.ZERO;
                if (IsReceipt())
                {
                    newBalance = Decimal.Add(newBalance, (Decimal)payAmt);

                    Decimal newCreditAmt = bp.GetSO_CreditUsed();
                    //if (newCreditAmt == null)
                    //    newCreditAmt = (Decimal)payAmt;
                    //else
                        newCreditAmt = Decimal.Add(newCreditAmt, (Decimal)payAmt);
                    //
                    log.Fine("TotalOpenBalance=" + bp.GetTotalOpenBalance(false) + "(" + payAmt
                        + ", Credit=" + bp.GetSO_CreditUsed() + "->" + newCreditAmt
                        + ", Balance=" + bp.GetTotalOpenBalance(false) + " -> " + newBalance);
                    bp.SetSO_CreditUsed(newCreditAmt);
                }	//	SO
                else
                {
                    newBalance = Decimal.Add(newBalance, (Decimal)payAmt);
                    log.Fine("Payment Amount =" + GetPayAmt(false) + "(" + payAmt
                        + ") Balance=" + bp.GetTotalOpenBalance(false) + " -> " + newBalance);
                }
                bp.SetTotalOpenBalance(newBalance);
                bp.SetSOCreditStatus();
                if (!bp.Save(Get_Trx()))
                {
                    _processMsg = "Could not update Business Partner";
                    return false;
                }


                //bp.SetTotalOpenBalance();
                bp.Save(Get_Trx());
            }

            _processMsg = Info.ToString();
            return true;
        }

        /**
         * 	Get Bank Statement Line of payment or 0
         *	@return id or 0
         */
        private int GetC_BankStatementLine_ID()
        {
            String sql = "SELECT C_BankStatementLine_ID FROM C_BankStatementLine WHERE C_Payment_ID=" + GetC_Payment_ID();
            int id = DataBase.DB.GetSQLValue(Get_Trx(), sql);
            if (id < 0)
                return 0;
            return id;
        }

        /**
         * 	Reverse Accrual - none
         * 	@return true if success 
         */
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return true if success 
         */
        public bool ReActivateIt()
        {
            log.Info(ToString());
            if (ReverseCorrectIt())
                return true;
            return false;
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPayment[");
            sb.Append(Get_ID()).Append("-").Append(GetDocumentNo())
                .Append(",Receipt=").Append(IsReceipt())
                .Append(",PayAmt=").Append(GetPayAmt())
                .Append(",Discount=").Append(GetDiscountAmt())
                .Append(",WriteOff=").Append(GetWriteOffAmt())
                .Append(",OverUnder=").Append(GetOverUnderAmt());
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
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

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
            //	ReportEngine re = ReportEngine.Get (GetCtx(), ReportEngine.PAYMENT, GetC_Payment_ID());
            //	if (re == null)
            return null;
            //	return re.GetPDF(file);
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
                .Append(Msg.Translate(GetCtx(), "PayAmt")).Append("=").Append(GetPayAmt())
                .Append(",").Append(Msg.Translate(GetCtx(), "WriteOffAmt")).Append("=").Append(GetWriteOffAmt());
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
            return GetCreatedBy();
        }

        /**
         * 	Get Document Approval Amount
         *	@return amount payment(AP) or write-off(AR)
         */
        public Decimal GetApprovalAmt()
        {
            if (IsReceipt())
                return GetWriteOffAmt();
            return GetPayAmt();
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