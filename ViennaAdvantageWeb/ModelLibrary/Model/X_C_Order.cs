namespace VAdvantage.Model
{

    /** Generated Model - DO NOT CHANGE */
    using System;
    using System.Text;
    using VAdvantage.DataBase;
    using VAdvantage.Common;
    using VAdvantage.Classes;
    using VAdvantage.Process;
    using VAdvantage.Model;
    using VAdvantage.Utility;
    using System.Data;
    /** Generated Model for C_Order
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Order : PO
    {
        public X_C_Order(Context ctx, int C_Order_ID, Trx trxName)
            : base(ctx, C_Order_ID, trxName)
        {
            /** if (C_Order_ID == 0)
            {
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_DocTypeTarget_ID (0);
            SetC_DocType_ID (0);	// 0
            SetC_Order_ID (0);
            SetC_PaymentTerm_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDateOrdered (DateTime.Now);	// @#Date@
            SetDeliveryRule (null);	// F
            SetDeliveryViaRule (null);	// P
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetFreightCostRule (null);	// I
            SetGrandTotal (0.0);
            SetInvoiceRule (null);	// I
            SetIsApproved (false);	// @IsApproved@
            SetIsCreditApproved (false);
            SetIsDelivered (false);
            SetIsDiscountPrinted (false);
            SetIsDropShip (false);	// N
            SetIsInvoiced (false);
            SetIsPrinted (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);	// @IsSOTrx@
            SetIsSelected (false);
            SetIsSelfService (false);
            SetIsTaxIncluded (false);
            SetIsTransferred (false);
            SetM_PriceList_ID (0);
            SetM_Warehouse_ID (0);
            SetPaymentRule (null);	// B
            SetPosted (false);	// N
            SetPriorityRule (null);	// 5
            SetProcessed (false);	// N
            SetSalesRep_ID (0);
            SetSendEMail (false);
            SetTotalLines (0.0);
            }
             */
        }
        public X_C_Order(Ctx ctx, int C_Order_ID, Trx trxName)
            : base(ctx, C_Order_ID, trxName)
        {
            /** if (C_Order_ID == 0)
            {
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_DocTypeTarget_ID (0);
            SetC_DocType_ID (0);	// 0
            SetC_Order_ID (0);
            SetC_PaymentTerm_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDateOrdered (DateTime.Now);	// @#Date@
            SetDeliveryRule (null);	// F
            SetDeliveryViaRule (null);	// P
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetFreightCostRule (null);	// I
            SetGrandTotal (0.0);
            SetInvoiceRule (null);	// I
            SetIsApproved (false);	// @IsApproved@
            SetIsCreditApproved (false);
            SetIsDelivered (false);
            SetIsDiscountPrinted (false);
            SetIsDropShip (false);	// N
            SetIsInvoiced (false);
            SetIsPrinted (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);	// @IsSOTrx@
            SetIsSelected (false);
            SetIsSelfService (false);
            SetIsTaxIncluded (false);
            SetIsTransferred (false);
            SetM_PriceList_ID (0);
            SetM_Warehouse_ID (0);
            SetPaymentRule (null);	// B
            SetPosted (false);	// N
            SetPriorityRule (null);	// 5
            SetProcessed (false);	// N
            SetSalesRep_ID (0);
            SetSendEMail (false);
            SetTotalLines (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Order(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Order(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Order(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_Order()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514373112L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056323L;
        /** AD_Table_ID=259 */
        public static int Table_ID;
        // =259;

        /** TableName=C_Order */
        public static String Table_Name = "C_Order";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(1);
        /** AccessLevel
        @return 1 - Org 
        */
        protected override int Get_AccessLevel()
        {
            return Convert.ToInt32(accessLevel.ToString());
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_C_Order[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetAD_OrgTrx_ID()
        {
            Object ii = Get_Value("AD_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Business Partner Contact */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Bill_BPartner_ID AD_Reference_ID=138 */
        public static int BILL_BPARTNER_ID_AD_Reference_ID = 138;
        /** Set Bill To.
        @param Bill_BPartner_ID Business Partner to be invoiced */
        public void SetBill_BPartner_ID(int Bill_BPartner_ID)
        {
            if (Bill_BPartner_ID <= 0) Set_Value("Bill_BPartner_ID", null);
            else
                Set_Value("Bill_BPartner_ID", Bill_BPartner_ID);
        }
        /** Get Bill To.
        @return Business Partner to be invoiced */
        public int GetBill_BPartner_ID()
        {
            Object ii = Get_Value("Bill_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Bill_Location_ID AD_Reference_ID=159 */
        public static int BILL_LOCATION_ID_AD_Reference_ID = 159;
        /** Set Bill To Location.
        @param Bill_Location_ID Business Partner Location for invoicing */
        public void SetBill_Location_ID(int Bill_Location_ID)
        {
            if (Bill_Location_ID <= 0) Set_Value("Bill_Location_ID", null);
            else
                Set_Value("Bill_Location_ID", Bill_Location_ID);
        }
        /** Get Bill To Location.
        @return Business Partner Location for invoicing */
        public int GetBill_Location_ID()
        {
            Object ii = Get_Value("Bill_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Bill_User_ID AD_Reference_ID=110 */
        public static int BILL_USER_ID_AD_Reference_ID = 110;
        /** Set Invoice Contact.
        @param Bill_User_ID Business Partner Contact for invoicing */
        public void SetBill_User_ID(int Bill_User_ID)
        {
            if (Bill_User_ID <= 0) Set_Value("Bill_User_ID", null);
            else
                Set_Value("Bill_User_ID", Bill_User_ID);
        }
        /** Get Invoice Contact.
        @return Business Partner Contact for invoicing */
        public int GetBill_User_ID()
        {
            Object ii = Get_Value("Bill_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Activity.
        @param C_Activity_ID Business Activity */
        public void SetC_Activity_ID(int C_Activity_ID)
        {
            if (C_Activity_ID <= 0) Set_Value("C_Activity_ID", null);
            else
                Set_Value("C_Activity_ID", C_Activity_ID);
        }
        /** Get Activity.
        @return Business Activity */
        public int GetC_Activity_ID()
        {
            Object ii = Get_Value("C_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory.");
            Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Location.
        @param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID < 1) throw new ArgumentException("C_BPartner_Location_ID is mandatory.");
            Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetC_Campaign_ID()
        {
            Object ii = Get_Value("C_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Journal Line.
        @param C_CashLine_ID Cash Journal Line */
        public void SetC_CashLine_ID(int C_CashLine_ID)
        {
            if (C_CashLine_ID <= 0) Set_Value("C_CashLine_ID", null);
            else
                Set_Value("C_CashLine_ID", C_CashLine_ID);
        }
        /** Get Cash Journal Line.
        @return Cash Journal Line */
        public int GetC_CashLine_ID()
        {
            Object ii = Get_Value("C_CashLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge.
        @param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetC_Charge_ID()
        {
            Object ii = Get_Value("C_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency Type.
        @param C_ConversionType_ID Currency Conversion Rate Type */
        public void SetC_ConversionType_ID(int C_ConversionType_ID)
        {
            if (C_ConversionType_ID <= 0) Set_Value("C_ConversionType_ID", null);
            else
                Set_Value("C_ConversionType_ID", C_ConversionType_ID);
        }
        /** Get Currency Type.
        @return Currency Conversion Rate Type */
        public int GetC_ConversionType_ID()
        {
            Object ii = Get_Value("C_ConversionType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_ValueNoCheck("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_DocTypeTarget_ID AD_Reference_ID=170 */
        public static int C_DOCTYPETARGET_ID_AD_Reference_ID = 170;
        /** Set Target Doc Type.
        @param C_DocTypeTarget_ID Target document type for documents */
        public void SetC_DocTypeTarget_ID(int C_DocTypeTarget_ID)
        {
            if (C_DocTypeTarget_ID < 1) throw new ArgumentException("C_DocTypeTarget_ID is mandatory.");
            Set_Value("C_DocTypeTarget_ID", C_DocTypeTarget_ID);
        }
        /** Get Target Doc Type.
        @return Target document type for documents */
        public int GetC_DocTypeTarget_ID()
        {
            Object ii = Get_Value("C_DocTypeTarget_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_DocType_ID AD_Reference_ID=170 */
        public static int C_DOCTYPE_ID_AD_Reference_ID = 170;
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
            Set_ValueNoCheck("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param C_Order_ID Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID < 1) throw new ArgumentException("C_Order_ID is mandatory.");
            Set_ValueNoCheck("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Term.
        @param C_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetC_PaymentTerm_ID(int C_PaymentTerm_ID)
        {
            if (C_PaymentTerm_ID < 1) throw new ArgumentException("C_PaymentTerm_ID is mandatory.");
            Set_Value("C_PaymentTerm_ID", C_PaymentTerm_ID);
        }
        /** Get Payment Term.
        @return The terms of Payment (timing, discount) */
        public int GetC_PaymentTerm_ID()
        {
            Object ii = Get_Value("C_PaymentTerm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment.
        @param C_Payment_ID Payment identifier */
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            if (C_Payment_ID <= 0) Set_Value("C_Payment_ID", null);
            else
                Set_Value("C_Payment_ID", C_Payment_ID);
        }
        /** Get Payment.
        @return Payment identifier */
        public int GetC_Payment_ID()
        {
            Object ii = Get_Value("C_Payment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project.
        @param C_Project_ID Financial Project */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge amount.
        @param ChargeAmt Charge Amount */
        public void SetChargeAmt(Decimal? ChargeAmt)
        {
            Set_Value("ChargeAmt", (Decimal?)ChargeAmt);
        }
        /** Get Charge amount.
        @return Charge Amount */
        public Decimal GetChargeAmt()
        {
            Object bd = Get_Value("ChargeAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Copy From.
        @param CopyFrom Copy From Record */
        public void SetCopyFrom(String CopyFrom)
        {
            if (CopyFrom != null && CopyFrom.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CopyFrom = CopyFrom.Substring(0, 1);
            }
            Set_Value("CopyFrom", CopyFrom);
        }
        /** Get Copy From.
        @return Copy From Record */
        public String GetCopyFrom()
        {
            return (String)Get_Value("CopyFrom");
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory.");
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Date Ordered.
        @param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered)
        {
            if (DateOrdered == null) throw new ArgumentException("DateOrdered is mandatory.");
            Set_Value("DateOrdered", (DateTime?)DateOrdered);
        }
        /** Get Date Ordered.
        @return Date of Order */
        public DateTime? GetDateOrdered()
        {
            return (DateTime?)Get_Value("DateOrdered");
        }
        /** Set Date printed.
        @param DatePrinted Date the document was printed. */
        public void SetDatePrinted(DateTime? DatePrinted)
        {
            Set_Value("DatePrinted", (DateTime?)DatePrinted);
        }
        /** Get Date printed.
        @return Date the document was printed. */
        public DateTime? GetDatePrinted()
        {
            return (DateTime?)Get_Value("DatePrinted");
        }
        /** Set Date Promised.
        @param DatePromised Date Order was promised */
        public void SetDatePromised(DateTime? DatePromised)
        {
            Set_Value("DatePromised", (DateTime?)DatePromised);
        }
        /** Get Date Promised.
        @return Date Order was promised */
        public DateTime? GetDatePromised()
        {
            return (DateTime?)Get_Value("DatePromised");
        }

        /** DeliveryRule AD_Reference_ID=151 */
        public static int DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        public static String DELIVERYRULE_Availability = "A";
        /** Force = F */
        public static String DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        public static String DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        public static String DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        public static String DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        public static String DELIVERYRULE_AfterReceipt = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDeliveryRuleValid(String test)
        {
            return test.Equals("A") || test.Equals("F") || test.Equals("L") || test.Equals("M") || test.Equals("O") || test.Equals("R");
        }
        /** Set Shipping Rule.
        @param DeliveryRule Defines the timing of Shipping */
        public void SetDeliveryRule(String DeliveryRule)
        {
            if (DeliveryRule == null) throw new ArgumentException("DeliveryRule is mandatory");
            if (!IsDeliveryRuleValid(DeliveryRule))
                throw new ArgumentException("DeliveryRule Invalid value - " + DeliveryRule + " - Reference_ID=151 - A - F - L - M - O - R");
            if (DeliveryRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DeliveryRule = DeliveryRule.Substring(0, 1);
            }
            Set_Value("DeliveryRule", DeliveryRule);
        }
        /** Get Shipping Rule.
        @return Defines the timing of Shipping */
        public String GetDeliveryRule()
        {
            return (String)Get_Value("DeliveryRule");
        }

        /** DeliveryViaRule AD_Reference_ID=152 */
        public static int DELIVERYVIARULE_AD_Reference_ID = 152;
        /** Delivery = D */
        public static String DELIVERYVIARULE_Delivery = "D";
        /** Pickup = P */
        public static String DELIVERYVIARULE_Pickup = "P";
        /** Shipper = S */
        public static String DELIVERYVIARULE_Shipper = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDeliveryViaRuleValid(String test)
        {
            return test.Equals("D") || test.Equals("P") || test.Equals("S");
        }
        /** Set Shipping Method.
        @param DeliveryViaRule How the order will be delivered */
        public void SetDeliveryViaRule(String DeliveryViaRule)
        {
            if (DeliveryViaRule == null) throw new ArgumentException("DeliveryViaRule is mandatory");
            if (!IsDeliveryViaRuleValid(DeliveryViaRule))
                throw new ArgumentException("DeliveryViaRule Invalid value - " + DeliveryViaRule + " - Reference_ID=152 - D - P - S");
            if (DeliveryViaRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DeliveryViaRule = DeliveryViaRule.Substring(0, 1);
            }
            Set_Value("DeliveryViaRule", DeliveryViaRule);
        }
        /** Get Shipping Method.
        @return How the order will be delivered */
        public String GetDeliveryViaRule()
        {
            return (String)Get_Value("DeliveryViaRule");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }

        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;
        /** <None> = -- */
        public static String DOCACTION_None = "--";
        /** Approve = AP */
        public static String DOCACTION_Approve = "AP";
        /** Close = CL */
        public static String DOCACTION_Close = "CL";
        /** Complete = CO */
        public static String DOCACTION_Complete = "CO";
        /** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";
        /** Post = PO */
        public static String DOCACTION_Post = "PO";
        /** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";
        /** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";
        /** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";
        /** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";
        /** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";
        /** Void = VO */
        public static String DOCACTION_Void = "VO";
        /** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";
        /** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocActionValid(String test)
        {
            return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
        }
        /** Set Document Action.
        @param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (DocAction == null) throw new ArgumentException("DocAction is mandatory");
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
            if (DocAction.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocAction = DocAction.Substring(0, 2);
            }
            Set_Value("DocAction", DocAction);
        }
        /** Get Document Action.
        @return The targeted status of the document */
        public String GetDocAction()
        {
            return (String)Get_Value("DocAction");
        }

        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;
        /** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocStatusValid(String test)
        {
            return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory");
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocStatus = DocStatus.Substring(0, 2);
            }
            Set_Value("DocStatus", DocStatus);
        }
        /** Get Document Status.
        @return The current status of the document */
        public String GetDocStatus()
        {
            return (String)Get_Value("DocStatus");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo == null) throw new ArgumentException("DocumentNo is mandatory.");
            if (DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_ValueNoCheck("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetDocumentNo());
        }
        /** Set Freight Amount.
        @param FreightAmt Freight Amount */
        public void SetFreightAmt(Decimal? FreightAmt)
        {
            Set_Value("FreightAmt", (Decimal?)FreightAmt);
        }
        /** Get Freight Amount.
        @return Freight Amount */
        public Decimal GetFreightAmt()
        {
            Object bd = Get_Value("FreightAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** FreightCostRule AD_Reference_ID=153 */
        public static int FREIGHTCOSTRULE_AD_Reference_ID = 153;
        /** Calculated = C */
        public static String FREIGHTCOSTRULE_Calculated = "C";
        /** Fix price = F */
        public static String FREIGHTCOSTRULE_FixPrice = "F";
        /** Freight included = I */
        public static String FREIGHTCOSTRULE_FreightIncluded = "I";
        /** Line = L */
        public static String FREIGHTCOSTRULE_Line = "L";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsFreightCostRuleValid(String test)
        {
            return test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L");
        }
        /** Set Freight Cost Rule.
        @param FreightCostRule Method for charging Freight */
        public void SetFreightCostRule(String FreightCostRule)
        {
            if (FreightCostRule == null) throw new ArgumentException("FreightCostRule is mandatory");
            if (!IsFreightCostRuleValid(FreightCostRule))
                throw new ArgumentException("FreightCostRule Invalid value - " + FreightCostRule + " - Reference_ID=153 - C - F - I - L");
            if (FreightCostRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                FreightCostRule = FreightCostRule.Substring(0, 1);
            }
            Set_Value("FreightCostRule", FreightCostRule);
        }
        /** Get Freight Cost Rule.
        @return Method for charging Freight */
        public String GetFreightCostRule()
        {
            return (String)Get_Value("FreightCostRule");
        }
        /** Set Grand Total.
        @param GrandTotal Total amount of document */
        public void SetGrandTotal(Decimal? GrandTotal)
        {
            if (GrandTotal == null) throw new ArgumentException("GrandTotal is mandatory.");
            Set_ValueNoCheck("GrandTotal", (Decimal?)GrandTotal);
        }
        /** Get Grand Total.
        @return Total amount of document */
        public Decimal GetGrandTotal()
        {
            Object bd = Get_Value("GrandTotal");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** InvoiceRule AD_Reference_ID=150 */
        public static int INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        public static String INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        public static String INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        public static String INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        public static String INVOICERULE_CustomerScheduleAfterDelivery = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsInvoiceRuleValid(String test)
        {
            return test.Equals("D") || test.Equals("I") || test.Equals("O") || test.Equals("S");
        }
        /** Set Invoicing Rule.
        @param InvoiceRule Frequency and method of invoicing */
        public void SetInvoiceRule(String InvoiceRule)
        {
            if (InvoiceRule == null) throw new ArgumentException("InvoiceRule is mandatory");
            if (!IsInvoiceRuleValid(InvoiceRule))
                throw new ArgumentException("InvoiceRule Invalid value - " + InvoiceRule + " - Reference_ID=150 - D - I - O - S");
            if (InvoiceRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                InvoiceRule = InvoiceRule.Substring(0, 1);
            }
            Set_Value("InvoiceRule", InvoiceRule);
        }
        /** Get Invoicing Rule.
        @return Frequency and method of invoicing */
        public String GetInvoiceRule()
        {
            return (String)Get_Value("InvoiceRule");
        }
        /** Set Approved.
        @param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved)
        {
            Set_ValueNoCheck("IsApproved", IsApproved);
        }
        /** Get Approved.
        @return Indicates if this document requires approval */
        public Boolean IsApproved()
        {
            Object oo = Get_Value("IsApproved");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Credit Approved.
        @param IsCreditApproved Credit  has been approved */
        public void SetIsCreditApproved(Boolean IsCreditApproved)
        {
            Set_ValueNoCheck("IsCreditApproved", IsCreditApproved);
        }
        /** Get Credit Approved.
        @return Credit  has been approved */
        public Boolean IsCreditApproved()
        {
            Object oo = Get_Value("IsCreditApproved");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Delivered.
        @param IsDelivered Delivered */
        public void SetIsDelivered(Boolean IsDelivered)
        {
            Set_ValueNoCheck("IsDelivered", IsDelivered);
        }
        /** Get Delivered.
        @return Delivered */
        public Boolean IsDelivered()
        {
            Object oo = Get_Value("IsDelivered");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Discount Printed.
        @param IsDiscountPrinted Print Discount on Invoice and Order */
        public void SetIsDiscountPrinted(Boolean IsDiscountPrinted)
        {
            Set_Value("IsDiscountPrinted", IsDiscountPrinted);
        }
        /** Get Discount Printed.
        @return Print Discount on Invoice and Order */
        public Boolean IsDiscountPrinted()
        {
            Object oo = Get_Value("IsDiscountPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip)
        {
            Set_ValueNoCheck("IsDropShip", IsDropShip);
        }
        /** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip()
        {
            Object oo = Get_Value("IsDropShip");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Invoiced.
        @param IsInvoiced Is this invoiced? */
        public void SetIsInvoiced(Boolean IsInvoiced)
        {
            Set_ValueNoCheck("IsInvoiced", IsInvoiced);
        }
        /** Get Invoiced.
        @return Is this invoiced? */
        public Boolean IsInvoiced()
        {
            Object oo = Get_Value("IsInvoiced");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Printed.
        @param IsPrinted Indicates if this document / line is printed */
        public void SetIsPrinted(Boolean IsPrinted)
        {
            Set_ValueNoCheck("IsPrinted", IsPrinted);
        }
        /** Get Printed.
        @return Indicates if this document / line is printed */
        public Boolean IsPrinted()
        {
            Object oo = Get_Value("IsPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Return Transaction.
        @param IsReturnTrx This is a return transaction */
        public void SetIsReturnTrx(Boolean IsReturnTrx)
        {
            Set_Value("IsReturnTrx", IsReturnTrx);
        }
        /** Get Return Transaction.
        @return This is a return transaction */
        public Boolean IsReturnTrx()
        {
            Object oo = Get_Value("IsReturnTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Sales Transaction.
        @param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx)
        {
            Set_Value("IsSOTrx", IsSOTrx);
        }
        /** Get Sales Transaction.
        @return This is a Sales Transaction */
        public Boolean IsSOTrx()
        {
            Object oo = Get_Value("IsSOTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Selected.
        @param IsSelected Selected */
        public void SetIsSelected(Boolean IsSelected)
        {
            Set_Value("IsSelected", IsSelected);
        }
        /** Get Selected.
        @return Selected */
        public Boolean IsSelected()
        {
            Object oo = Get_Value("IsSelected");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Self-Service.
        @param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
        public void SetIsSelfService(Boolean IsSelfService)
        {
            Set_Value("IsSelfService", IsSelfService);
        }
        /** Get Self-Service.
        @return This is a Self-Service entry or this entry can be changed via Self-Service */
        public Boolean IsSelfService()
        {
            Object oo = Get_Value("IsSelfService");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Price includes Tax.
        @param IsTaxIncluded Tax is included in the price */
        public void SetIsTaxIncluded(Boolean IsTaxIncluded)
        {
            Set_Value("IsTaxIncluded", IsTaxIncluded);
        }
        /** Get Price includes Tax.
        @return Tax is included in the price */
        public Boolean IsTaxIncluded()
        {
            Object oo = Get_Value("IsTaxIncluded");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Transferred.
        @param IsTransferred Transferred to General Ledger (i.e. accounted) */
        public void SetIsTransferred(Boolean IsTransferred)
        {
            Set_ValueNoCheck("IsTransferred", IsTransferred);
        }
        /** Get Transferred.
        @return Transferred to General Ledger (i.e. accounted) */
        public Boolean IsTransferred()
        {
            Object oo = Get_Value("IsTransferred");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Price List.
        @param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID < 1) throw new ArgumentException("M_PriceList_ID is mandatory.");
            Set_Value("M_PriceList_ID", M_PriceList_ID);
        }
        /** Get Price List.
        @return Unique identifier of a Price List */
        public int GetM_PriceList_ID()
        {
            Object ii = Get_Value("M_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set RMA Category.
        @param M_RMACategory_ID Return Material Authorization Category */
        public void SetM_RMACategory_ID(int M_RMACategory_ID)
        {
            if (M_RMACategory_ID <= 0) Set_Value("M_RMACategory_ID", null);
            else
                Set_Value("M_RMACategory_ID", M_RMACategory_ID);
        }
        /** Get RMA Category.
        @return Return Material Authorization Category */
        public int GetM_RMACategory_ID()
        {
            Object ii = Get_Value("M_RMACategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Return Policy.
        @param M_ReturnPolicy_ID The Return Policy dictates the timeframe within which goods can be returned. */
        public void SetM_ReturnPolicy_ID(int M_ReturnPolicy_ID)
        {
            if (M_ReturnPolicy_ID <= 0) Set_Value("M_ReturnPolicy_ID", null);
            else
                Set_Value("M_ReturnPolicy_ID", M_ReturnPolicy_ID);
        }
        /** Get Return Policy.
        @return The Return Policy dictates the timeframe within which goods can be returned. */
        public int GetM_ReturnPolicy_ID()
        {
            Object ii = Get_Value("M_ReturnPolicy_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Freight Carrier.
        @param M_Shipper_ID Method or manner of product delivery */
        public void SetM_Shipper_ID(int M_Shipper_ID)
        {
            if (M_Shipper_ID <= 0) Set_Value("M_Shipper_ID", null);
            else
                Set_Value("M_Shipper_ID", M_Shipper_ID);
        }
        /** Get Freight Carrier.
        @return Method or manner of product delivery */
        public int GetM_Shipper_ID()
        {
            Object ii = Get_Value("M_Shipper_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_InOut_ID AD_Reference_ID=337 */
        public static int ORIG_INOUT_ID_AD_Reference_ID = 337;
        /** Set Orig Shipment.
        @param Orig_InOut_ID Original shipment of the RMA */
        public void SetOrig_InOut_ID(int Orig_InOut_ID)
        {
            if (Orig_InOut_ID <= 0) Set_Value("Orig_InOut_ID", null);
            else
                Set_Value("Orig_InOut_ID", Orig_InOut_ID);
        }
        /** Get Orig Shipment.
        @return Original shipment of the RMA */
        public int GetOrig_InOut_ID()
        {
            Object ii = Get_Value("Orig_InOut_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_Order_ID AD_Reference_ID=290 */
        public static int ORIG_ORDER_ID_AD_Reference_ID = 290;
        /** Set Orig Sales Order.
        @param Orig_Order_ID Original Sales Order for Return Material Authorization */
        public void SetOrig_Order_ID(int Orig_Order_ID)
        {
            if (Orig_Order_ID <= 0) Set_Value("Orig_Order_ID", null);
            else
                Set_Value("Orig_Order_ID", Orig_Order_ID);
        }
        /** Get Orig Sales Order.
        @return Original Sales Order for Return Material Authorization */
        public int GetOrig_Order_ID()
        {
            Object ii = Get_Value("Orig_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Reference.
        @param POReference Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public void SetPOReference(String POReference)
        {
            if (POReference != null && POReference.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                POReference = POReference.Substring(0, 20);
            }
            Set_Value("POReference", POReference);
        }
        /** Get Order Reference.
        @return Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public String GetPOReference()
        {
            return (String)Get_Value("POReference");
        }
        /** Set Payment BPartner.
        @param Pay_BPartner_ID Business Partner responsible for the payment */
        public void SetPay_BPartner_ID(int Pay_BPartner_ID)
        {
            if (Pay_BPartner_ID <= 0) Set_Value("Pay_BPartner_ID", null);
            else
                Set_Value("Pay_BPartner_ID", Pay_BPartner_ID);
        }
        /** Get Payment BPartner.
        @return Business Partner responsible for the payment */
        public int GetPay_BPartner_ID()
        {
            Object ii = Get_Value("Pay_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Location.
        @param Pay_Location_ID Location of the Business Partner responsible for the payment */
        public void SetPay_Location_ID(int Pay_Location_ID)
        {
            if (Pay_Location_ID <= 0) Set_Value("Pay_Location_ID", null);
            else
                Set_Value("Pay_Location_ID", Pay_Location_ID);
        }
        /** Get Payment Location.
        @return Location of the Business Partner responsible for the payment */
        public int GetPay_Location_ID()
        {
            Object ii = Get_Value("Pay_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PaymentRule AD_Reference_ID=195 */
        public static int PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        public static String PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        public static String PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        public static String PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        public static String PAYMENTRULE_DirectDeposit = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPaymentRuleValid(String test)
        {
            return test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
        }
        /** Set Payment Method.
        @param PaymentRule How you pay the invoice */
        public void SetPaymentRule(String PaymentRule)
        {
            if (PaymentRule == null) throw new ArgumentException("PaymentRule is mandatory");
            if (!IsPaymentRuleValid(PaymentRule))
                throw new ArgumentException("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - P - S - T");
            if (PaymentRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PaymentRule = PaymentRule.Substring(0, 1);
            }
            Set_Value("PaymentRule", PaymentRule);
        }
        /** Get Payment Method.
        @return How you pay the invoice */
        public String GetPaymentRule()
        {
            return (String)Get_Value("PaymentRule");
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_Value("Posted", Posted);
        }
        /** Get Posted.
        @return Posting status */
        public Boolean IsPosted()
        {
            Object oo = Get_Value("Posted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** PriorityRule AD_Reference_ID=154 */
        public static int PRIORITYRULE_AD_Reference_ID = 154;
        /** Urgent = 1 */
        public static String PRIORITYRULE_Urgent = "1";
        /** High = 3 */
        public static String PRIORITYRULE_High = "3";
        /** Medium = 5 */
        public static String PRIORITYRULE_Medium = "5";
        /** Low = 7 */
        public static String PRIORITYRULE_Low = "7";
        /** Minor = 9 */
        public static String PRIORITYRULE_Minor = "9";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPriorityRuleValid(String test)
        {
            return test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
        }
        /** Set Priority.
        @param PriorityRule Priority of a document */
        public void SetPriorityRule(String PriorityRule)
        {
            if (PriorityRule == null) throw new ArgumentException("PriorityRule is mandatory");
            if (!IsPriorityRuleValid(PriorityRule))
                throw new ArgumentException("PriorityRule Invalid value - " + PriorityRule + " - Reference_ID=154 - 1 - 3 - 5 - 7 - 9");
            if (PriorityRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PriorityRule = PriorityRule.Substring(0, 1);
            }
            Set_Value("PriorityRule", PriorityRule);
        }
        /** Get Priority.
        @return Priority of a document */
        public String GetPriorityRule()
        {
            return (String)Get_Value("PriorityRule");
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_ValueNoCheck("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
        @return Process Now */
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Ref_Order_ID AD_Reference_ID=290 */
        public static int REF_ORDER_ID_AD_Reference_ID = 290;
        /** Set Referenced Order.
        @param Ref_Order_ID Reference to corresponding Sales/Purchase Order */
        public void SetRef_Order_ID(int Ref_Order_ID)
        {
            if (Ref_Order_ID <= 0) Set_Value("Ref_Order_ID", null);
            else
                Set_Value("Ref_Order_ID", Ref_Order_ID);
        }
        /** Get Referenced Order.
        @return Reference to corresponding Sales/Purchase Order */
        public int GetRef_Order_ID()
        {
            Object ii = Get_Value("Ref_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
        /** Set Representative.
        @param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID < 1) throw new ArgumentException("SalesRep_ID is mandatory.");
            Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Representative.
        @return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Send EMail.
        @param SendEMail Enable sending Document EMail */
        public void SetSendEMail(Boolean SendEMail)
        {
            Set_Value("SendEMail", SendEMail);
        }
        /** Get Send EMail.
        @return Enable sending Document EMail */
        public Boolean IsSendEMail()
        {
            Object oo = Get_Value("SendEMail");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Ship Date.
        @param ShipDate Shipment Date/Time */
        public void SetShipDate(DateTime? ShipDate)
        {
            throw new ArgumentException("ShipDate Is virtual column");
        }
        /** Get Ship Date.
        @return Shipment Date/Time */
        public DateTime? GetShipDate()
        {
            return (DateTime?)Get_Value("ShipDate");
        }
        /** Set SubTotal.
        @param TotalLines Total of all document lines (excluding Tax) */
        public void SetTotalLines(Decimal? TotalLines)
        {
            if (TotalLines == null) throw new ArgumentException("TotalLines is mandatory.");
            Set_ValueNoCheck("TotalLines", (Decimal?)TotalLines);
        }
        /** Get SubTotal.
        @return Total of all document lines (excluding Tax) */
        public Decimal GetTotalLines()
        {
            Object bd = Get_Value("TotalLines");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** User1_ID AD_Reference_ID=134 */
        public static int USER1_ID_AD_Reference_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
        }
        /** Get User List 1.
        @return User defined list element #1 */
        public int GetUser1_ID()
        {
            Object ii = Get_Value("User1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User2_ID AD_Reference_ID=137 */
        public static int USER2_ID_AD_Reference_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Volume.
        @param Volume Volume of a product */
        public void SetVolume(Decimal? Volume)
        {
            Set_Value("Volume", (Decimal?)Volume);
        }
        /** Get Volume.
        @return Volume of a product */
        public Decimal GetVolume()
        {
            Object bd = Get_Value("Volume");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Weight.
        @param Weight Weight of a product */
        public void SetWeight(Decimal? Weight)
        {
            Set_Value("Weight", (Decimal?)Weight);
        }
        /** Get Weight.
        @return Weight of a product */
        public Decimal GetWeight()
        {
            Object bd = Get_Value("Weight");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }


        public void SetPayStatus(String PayStatus)
        {
            Set_Value("PayStatus", PayStatus);
        }

        /** Set PaymentMethod.
        @param PaymentMethod of a product */
        public void SetPaymentMethod(String PaymentMethod)
        {
            Set_Value("PaymentMethod", (String)PaymentMethod);
        }
        /** Get PaymentMethod.
        @return PaymentMethod of a product */
        public String GetPaymentMethod()
        {
            Object bd = Get_Value("PaymentMethod");
            if (bd == null) return "";
            return Convert.ToString(bd);
        }

        /** Set Create Service Contract.
@param Create Service Contract*/
        public void SetCreateServiceContract(String CreateServiceContract)
        {
            if (CreateServiceContract != null && CreateServiceContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CreateServiceContract = CreateServiceContract.Substring(0, 50);
            }
            Set_Value("CreateServiceContract", CreateServiceContract);
        }
        /** Get Create Service Contract.
        @return Create Service Contract */
        public String GetCreateServiceContract()
        {
            return (String)Get_Value("CreateServiceContract");
        }

        /** Set Accpt. Contact.
        @param AcceptContact Accpt. Contact */
        public void SetAcceptContact(String AcceptContact)
        {
            if (AcceptContact != null && AcceptContact.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                AcceptContact = AcceptContact.Substring(0, 50);
            }
            Set_Value("AcceptContact", AcceptContact);
        }
        /** Get Accpt. Contact.
        @return Accpt. Contact */
        public String GetAcceptContact()
        {
            return (String)Get_Value("AcceptContact");
        }
        /** Set Accpt. Remarks.
        @param AcceptanceRemarks Accpt. Remarks */
        public void SetAcceptanceRemarks(String AcceptanceRemarks)
        {
            if (AcceptanceRemarks != null && AcceptanceRemarks.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                AcceptanceRemarks = AcceptanceRemarks.Substring(0, 50);
            }
            Set_Value("AcceptanceRemarks", AcceptanceRemarks);
        }
        /** Get Accpt. Remarks.
        @return Accpt. Remarks */
        public String GetAcceptanceRemarks()
        {
            return (String)Get_Value("AcceptanceRemarks");
        }
        /** Set Accepted By.
        @param AcceptedBy Accepted By */
        public void SetAcceptedBy(String AcceptedBy)
        {
            if (AcceptedBy != null && AcceptedBy.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                AcceptedBy = AcceptedBy.Substring(0, 50);
            }
            Set_Value("AcceptedBy", AcceptedBy);
        }
        /** Get Accepted By.
        @return Accepted By */
        public String GetAcceptedBy()
        {
            return (String)Get_Value("AcceptedBy");
        }

        /** Set To Business Partner.
        @param C_BPartner_To_ID To Business Partner */
        public void SetC_BPartner_To_ID(int C_BPartner_To_ID)
        {
            if (C_BPartner_To_ID <= 0) Set_Value("C_BPartner_To_ID", null);
            else
                Set_Value("C_BPartner_To_ID", C_BPartner_To_ID);
        }
        /** Get To Business Partner.
        @return To Business Partner */
        public int GetC_BPartner_To_ID()
        {
            Object ii = Get_Value("C_BPartner_To_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Work Complete.
       @param DateWorkComplete Date when work is (planned to be) complete */
        public void SetDateWorkComplete(DateTime? DateWorkComplete)
        {
            Set_Value("DateWorkComplete", (DateTime?)DateWorkComplete);
        }
        /** Get Work Complete.
        @return Date when work is (planned to be) complete */
        public DateTime? GetDateWorkComplete()
        {
            return (DateTime?)Get_Value("DateWorkComplete");
        }

        /** DeliveredBy AD_Reference_ID=252 */
        public static int DELIVEREDBY_AD_Reference_ID = 252;
        /** Set Delivered By.
        @param DeliveredBy Delivered By */
        public void SetDeliveredBy(int DeliveredBy)
        {
            Set_Value("DeliveredBy", DeliveredBy);
        }
        /** Get Delivered By.
        @return Delivered By */
        public int GetDeliveredBy()
        {
            Object ii = Get_Value("DeliveredBy");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Delivery Contact.
       @param DeliveryContact Delivery Contact */
        public void SetDeliveryContact(String DeliveryContact)
        {
            if (DeliveryContact != null && DeliveryContact.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                DeliveryContact = DeliveryContact.Substring(0, 50);
            }
            Set_Value("DeliveryContact", DeliveryContact);
        }
        /** Get Delivery Contact.
        @return Delivery Contact */
        public String GetDeliveryContact()
        {
            return (String)Get_Value("DeliveryContact");
        }

        /** Set EMail Address.
        @param EMail Electronic Mail Address */
        public void SetEMail(String EMail)
        {
            if (EMail != null && EMail.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                EMail = EMail.Substring(0, 50);
            }
            Set_Value("EMail", EMail);
        }
        /** Get EMail Address.
        @return Electronic Mail Address */
        public String GetEMail()
        {
            return (String)Get_Value("EMail");
        }

        /** Set Comment.
       @param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Help = Help.Substring(0, 50);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
        @return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }

        /** Set RTTS_TTNo.
        @param RTTS_TTNo RTTS_TTNo */
        public void SetRTTS_TTNo(String RTTS_TTNo)
        {
            if (RTTS_TTNo != null && RTTS_TTNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                RTTS_TTNo = RTTS_TTNo.Substring(0, 50);
            }
            Set_Value("RTTS_TTNo", RTTS_TTNo);
        }
        /** Get RTTS_TTNo.
        @return RTTS_TTNo */
        public String GetRTTS_TTNo()
        {
            return (String)Get_Value("RTTS_TTNo");
        }

        /** StatusCode AD_Reference_ID=1000028 */
        public static int STATUSCODE_AD_Reference_ID = 1000028;
        /** Completed = C */
        public static String STATUSCODE_Completed = "C";
        /** Pending = P */
        public static String STATUSCODE_Pending = "P";
        /** Rejected = R */
        public static String STATUSCODE_Rejected = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsStatusCodeValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("P") || test.Equals("R");
        }
        /** Set Status Code.
        @param StatusCode Status Code */
        public void SetStatusCode(String StatusCode)
        {
            if (!IsStatusCodeValid(StatusCode))
                throw new ArgumentException("StatusCode Invalid value - " + StatusCode + " - Reference_ID=1000028 - C - P - R");
            if (StatusCode != null && StatusCode.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                StatusCode = StatusCode.Substring(0, 1);
            }
            Set_Value("StatusCode", StatusCode);
        }
        /** Get Status Code.
        @return Status Code */
        public String GetStatusCode()
        {
            return (String)Get_Value("StatusCode");
        }

        /** Set VSS_Circuit_ID.
        @param VSS_Circuit_ID VSS_Circuit_ID */
        public void SetVSS_Circuit_ID(int VSS_Circuit_ID)
        {
            if (VSS_Circuit_ID <= 0) Set_Value("VSS_Circuit_ID", null);
            else
                Set_Value("VSS_Circuit_ID", VSS_Circuit_ID);
        }
        /** Get VSS_Circuit_ID.
        @return VSS_Circuit_ID */
        public int GetVSS_Circuit_ID()
        {
            Object ii = Get_Value("VSS_Circuit_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VSS_OrderTaskType_ID.
        @param VSS_OrderTaskType_ID VSS_OrderTaskType_ID */
        public void SetVSS_OrderTaskType_ID(int VSS_OrderTaskType_ID)
        {
            if (VSS_OrderTaskType_ID <= 0) Set_Value("VSS_OrderTaskType_ID", null);
            else
                Set_Value("VSS_OrderTaskType_ID", VSS_OrderTaskType_ID);
        }
        /** Get VSS_OrderTaskType_ID.
        @return VSS_OrderTaskType_ID */
        public int GetVSS_OrderTaskType_ID()
        {
            Object ii = Get_Value("VSS_OrderTaskType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VSS_OrderType_ID.
        @param VSS_OrderType_ID VSS_OrderType_ID */
        public void SetVSS_OrderType_ID(int VSS_OrderType_ID)
        {
            if (VSS_OrderType_ID <= 0) Set_Value("VSS_OrderType_ID", null);
            else
                Set_Value("VSS_OrderType_ID", VSS_OrderType_ID);
        }
        /** Get VSS_OrderType_ID.
        @return VSS_OrderType_ID */
        public int GetVSS_OrderType_ID()
        {
            Object ii = Get_Value("VSS_OrderType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set SONumber.
        @param SONumber */
        public void SetSONumber(String SONumber)
        {
            if (SONumber != null && SONumber.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                SONumber = SONumber.Substring(0, 20);
            }
            Set_Value("SONumber", SONumber);
        }
        /** Get SONumber.
        @return  */
        public String GetSONumber()
        {
            return (String)Get_Value("SONumber");
        }


        /** Set Budget Violated.
@param IsBudgetViolated Budget Violated */
        public void SetIsBudgetViolated(Boolean IsBudgetViolated)
        {
            Set_Value("IsBudgetViolated", IsBudgetViolated);
        }
        /** Get Budget Violated.
        @return Budget Violated */
        public Boolean IsBudgetViolated()
        {
            Object oo = Get_Value("IsBudgetViolated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Max Budget Violation Amount.
@param MaxBudgetViolationAmount Max Budget Violation Amount */
        public void SetMaxBudgetViolationAmount(Decimal? MaxBudgetViolationAmount)
        {
            Set_Value("MaxBudgetViolationAmount", (Decimal?)MaxBudgetViolationAmount);
        }
        /** Get Max Budget Violation Amount.
        @return Max Budget Violation Amount */
        public Decimal GetMaxBudgetViolationAmount()    
        {
            Object bd = Get_Value("MaxBudgetViolationAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Is Contract.**/
        public void SetIsContract(Boolean IsContract)
        {
            Set_ValueNoCheck("IsContract", IsContract);
        }
        /** Get Is Contract.**/
        public Boolean GetIsContract()
        {
            Object oo = Get_Value("IsContract");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
    }

}
