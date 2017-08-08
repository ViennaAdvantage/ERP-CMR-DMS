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
/** Generated Model for C_BPartner
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_BPartner : PO
{
public X_C_BPartner (Context ctx, int C_BPartner_ID, Trx trxName) : base (ctx, C_BPartner_ID, trxName)
{
/** if (C_BPartner_ID == 0)
{
SetC_BP_Group_ID (0);
SetC_BPartner_ID (0);
SetIsCustomer (false);
SetIsEmployee (false);
SetIsOneTime (false);
SetIsProspect (false);
SetIsSalesRep (false);
SetIsSummary (false);
SetIsVendor (false);
SetName (null);
SetSO_CreditLimit (0.0);
SetSO_CreditUsed (0.0);
SetSendEMail (false);
SetValue (null);
}
 */
}
public X_C_BPartner (Ctx ctx, int C_BPartner_ID, Trx trxName) : base (ctx, C_BPartner_ID, trxName)
{
/** if (C_BPartner_ID == 0)
{
SetC_BP_Group_ID (0);
SetC_BPartner_ID (0);
SetIsCustomer (false);
SetIsEmployee (false);
SetIsOneTime (false);
SetIsProspect (false);
SetIsSalesRep (false);
SetIsSummary (false);
SetIsVendor (false);
SetName (null);
SetSO_CreditLimit (0.0);
SetSO_CreditUsed (0.0);
SetSendEMail (false);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BPartner (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BPartner (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BPartner (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BPartner()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370416L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053627L;
/** AD_Table_ID=291 */
public static int Table_ID;
 // =291;

/** TableName=C_BPartner */
public static String Table_Name="C_BPartner";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_BPartner[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=327 */
public static int AD_LANGUAGE_AD_Reference_ID=327;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language != null && AD_Language.Length > 5)
{
log.Warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_Value ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}

/** AD_OrgBP_ID AD_Reference_ID=417 */
public static int AD_ORGBP_ID_AD_Reference_ID=417;
/** Set Linked Organization.
@param AD_OrgBP_ID The Business Partner is another Organization for explicit Inter-Org transactions */
public void SetAD_OrgBP_ID (String AD_OrgBP_ID)
{
if (AD_OrgBP_ID != null && AD_OrgBP_ID.Length > 22)
{
log.Warning("Length > 22 - truncated");
AD_OrgBP_ID = AD_OrgBP_ID.Substring(0,22);
}
Set_Value ("AD_OrgBP_ID", AD_OrgBP_ID);
}
/** Get Linked Organization.
@return The Business Partner is another Organization for explicit Inter-Org transactions */
public String GetAD_OrgBP_ID() 
{
return (String)Get_Value("AD_OrgBP_ID");
}
/** Set Acquisition Cost.
@param AcqusitionCost The cost of gaining the prospect as a customer */
public void SetAcqusitionCost (Decimal? AcqusitionCost)
{
Set_Value ("AcqusitionCost", (Decimal?)AcqusitionCost);
}
/** Get Acquisition Cost.
@return The cost of gaining the prospect as a customer */
public Decimal GetAcqusitionCost() 
{
Object bd =Get_Value("AcqusitionCost");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Life Time Value.
@param ActualLifeTimeValue Actual Life Time Revenue */
public void SetActualLifeTimeValue (Decimal? ActualLifeTimeValue)
{
Set_Value ("ActualLifeTimeValue", (Decimal?)ActualLifeTimeValue);
}
/** Get Life Time Value.
@return Actual Life Time Revenue */
public Decimal GetActualLifeTimeValue() 
{
Object bd =Get_Value("ActualLifeTimeValue");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Partner Parent.
@param BPartner_Parent_ID Business Partner Parent */
public void SetBPartner_Parent_ID (int BPartner_Parent_ID)
{
if (BPartner_Parent_ID <= 0) Set_Value ("BPartner_Parent_ID", null);
else
Set_Value ("BPartner_Parent_ID", BPartner_Parent_ID);
}
/** Get Partner Parent.
@return Business Partner Parent */
public int GetBPartner_Parent_ID() 
{
Object ii = Get_Value("BPartner_Parent_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner Group.
@param C_BP_Group_ID Business Partner Group */
public void SetC_BP_Group_ID (int C_BP_Group_ID)
{
if (C_BP_Group_ID < 1) throw new ArgumentException ("C_BP_Group_ID is mandatory.");
Set_Value ("C_BP_Group_ID", C_BP_Group_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetC_BP_Group_ID() 
{
Object ii = Get_Value("C_BP_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BP Size.
@param C_BP_Size_ID Business Partner Size */
public void SetC_BP_Size_ID (int C_BP_Size_ID)
{
if (C_BP_Size_ID <= 0) Set_Value ("C_BP_Size_ID", null);
else
Set_Value ("C_BP_Size_ID", C_BP_Size_ID);
}
/** Get BP Size.
@return Business Partner Size */
public int GetC_BP_Size_ID() 
{
Object ii = Get_Value("C_BP_Size_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set BP Status.
@param C_BP_Status_ID Business Partner Status */
public void SetC_BP_Status_ID (int C_BP_Status_ID)
{
if (C_BP_Status_ID <= 0) Set_Value ("C_BP_Status_ID", null);
else
Set_Value ("C_BP_Status_ID", C_BP_Status_ID);
}
/** Get BP Status.
@return Business Partner Status */
public int GetC_BP_Status_ID() 
{
Object ii = Get_Value("C_BP_Status_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Dunning.
@param C_Dunning_ID Dunning Rules for overdue invoices */
public void SetC_Dunning_ID (int C_Dunning_ID)
{
if (C_Dunning_ID <= 0) Set_Value ("C_Dunning_ID", null);
else
Set_Value ("C_Dunning_ID", C_Dunning_ID);
}
/** Get Dunning.
@return Dunning Rules for overdue invoices */
public int GetC_Dunning_ID() 
{
Object ii = Get_Value("C_Dunning_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Greeting.
@param C_Greeting_ID Greeting to print on correspondence */
public void SetC_Greeting_ID (int C_Greeting_ID)
{
if (C_Greeting_ID <= 0) Set_Value ("C_Greeting_ID", null);
else
Set_Value ("C_Greeting_ID", C_Greeting_ID);
}
/** Get Greeting.
@return Greeting to print on correspondence */
public int GetC_Greeting_ID() 
{
Object ii = Get_Value("C_Greeting_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Industry Code.
@param C_IndustryCode_ID Business Partner Industry Classification */
public void SetC_IndustryCode_ID (int C_IndustryCode_ID)
{
if (C_IndustryCode_ID <= 0) Set_Value ("C_IndustryCode_ID", null);
else
Set_Value ("C_IndustryCode_ID", C_IndustryCode_ID);
}
/** Get Industry Code.
@return Business Partner Industry Classification */
public int GetC_IndustryCode_ID() 
{
Object ii = Get_Value("C_IndustryCode_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Schedule.
@param C_InvoiceSchedule_ID Schedule for generating Invoices */
public void SetC_InvoiceSchedule_ID (int C_InvoiceSchedule_ID)
{
if (C_InvoiceSchedule_ID <= 0) Set_Value ("C_InvoiceSchedule_ID", null);
else
Set_Value ("C_InvoiceSchedule_ID", C_InvoiceSchedule_ID);
}
/** Get Invoice Schedule.
@return Schedule for generating Invoices */
public int GetC_InvoiceSchedule_ID() 
{
Object ii = Get_Value("C_InvoiceSchedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Term.
@param C_PaymentTerm_ID The terms of Payment (timing, discount) */
public void SetC_PaymentTerm_ID (int C_PaymentTerm_ID)
{
if (C_PaymentTerm_ID <= 0) Set_Value ("C_PaymentTerm_ID", null);
else
Set_Value ("C_PaymentTerm_ID", C_PaymentTerm_ID);
}
/** Get Payment Term.
@return The terms of Payment (timing, discount) */
public int GetC_PaymentTerm_ID() 
{
Object ii = Get_Value("C_PaymentTerm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set City.
@param City Identifies a City */
public void SetCity (String City)
{
if (City != null && City.Length > 50)
{
log.Warning("Length > 50 - truncated");
City = City.Substring(0,50);
}
Set_Value ("City", City);
}
/** Get City.
@return Identifies a City */
public String GetCity() 
{
return (String)Get_Value("City");
}
/** Set Contact Name.
@param ContactName Business Partner Contact Name */
public void SetContactName (String ContactName)
{
if (ContactName != null && ContactName.Length > 50)
{
log.Warning("Length > 50 - truncated");
ContactName = ContactName.Substring(0,50);
}
Set_Value ("ContactName", ContactName);
}
/** Get Contact Name.
@return Business Partner Contact Name */
public String GetContactName() 
{
return (String)Get_Value("ContactName");
}
/** Set D-U-N-S.
@param DUNS Creditor Check (Dun & Bradstreet) Number */
public void SetDUNS (String DUNS)
{
if (DUNS != null && DUNS.Length > 12)
{
log.Warning("Length > 12 - truncated");
DUNS = DUNS.Substring(0,12);
}
Set_Value ("DUNS", DUNS);
}
/** Get D-U-N-S.
@return Creditor Check (Dun & Bradstreet) Number */
public String GetDUNS() 
{
return (String)Get_Value("DUNS");
}

/** DeliveryRule AD_Reference_ID=151 */
public static int DELIVERYRULE_AD_Reference_ID=151;
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
public bool IsDeliveryRuleValid (String test)
{
return test == null || test.Equals("A") || test.Equals("F") || test.Equals("L") || test.Equals("M") || test.Equals("O") || test.Equals("R");
}
/** Set Shipping Rule.
@param DeliveryRule Defines the timing of Shipping */
public void SetDeliveryRule (String DeliveryRule)
{
if (!IsDeliveryRuleValid(DeliveryRule))
throw new ArgumentException ("DeliveryRule Invalid value - " + DeliveryRule + " - Reference_ID=151 - A - F - L - M - O - R");
if (DeliveryRule != null && DeliveryRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
DeliveryRule = DeliveryRule.Substring(0,1);
}
Set_Value ("DeliveryRule", DeliveryRule);
}
/** Get Shipping Rule.
@return Defines the timing of Shipping */
public String GetDeliveryRule() 
{
return (String)Get_Value("DeliveryRule");
}

/** DeliveryViaRule AD_Reference_ID=152 */
public static int DELIVERYVIARULE_AD_Reference_ID=152;
/** Delivery = D */
public static String DELIVERYVIARULE_Delivery = "D";
/** Pickup = P */
public static String DELIVERYVIARULE_Pickup = "P";
/** Shipper = S */
public static String DELIVERYVIARULE_Shipper = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsDeliveryViaRuleValid (String test)
{
return test == null || test.Equals("D") || test.Equals("P") || test.Equals("S");
}
/** Set Shipping Method.
@param DeliveryViaRule How the order will be delivered */
public void SetDeliveryViaRule (String DeliveryViaRule)
{
if (!IsDeliveryViaRuleValid(DeliveryViaRule))
throw new ArgumentException ("DeliveryViaRule Invalid value - " + DeliveryViaRule + " - Reference_ID=152 - D - P - S");
if (DeliveryViaRule != null && DeliveryViaRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
DeliveryViaRule = DeliveryViaRule.Substring(0,1);
}
Set_Value ("DeliveryViaRule", DeliveryViaRule);
}
/** Get Shipping Method.
@return How the order will be delivered */
public String GetDeliveryViaRule() 
{
return (String)Get_Value("DeliveryViaRule");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Document Copies.
@param DocumentCopies Number of copies to be printed */
public void SetDocumentCopies (int DocumentCopies)
{
Set_Value ("DocumentCopies", DocumentCopies);
}
/** Get Document Copies.
@return Number of copies to be printed */
public int GetDocumentCopies() 
{
Object ii = Get_Value("DocumentCopies");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set EMail Address.
@param EMail Electronic Mail Address */
public void SetEMail (String EMail)
{
if (EMail != null && EMail.Length > 50)
{
log.Warning("Length > 50 - truncated");
EMail = EMail.Substring(0,50);
}
Set_Value ("EMail", EMail);
}
/** Get EMail Address.
@return Electronic Mail Address */
public String GetEMail() 
{
return (String)Get_Value("EMail");
}
/** Set First Sale.
@param FirstSale Date of First Sale */
public void SetFirstSale (DateTime? FirstSale)
{
Set_Value ("FirstSale", (DateTime?)FirstSale);
}
/** Get First Sale.
@return Date of First Sale */
public DateTime? GetFirstSale() 
{
return (DateTime?)Get_Value("FirstSale");
}
/** Set Flat Discount %.
@param FlatDiscount Flat discount percentage */
public void SetFlatDiscount (Decimal? FlatDiscount)
{
Set_Value ("FlatDiscount", (Decimal?)FlatDiscount);
}
/** Get Flat Discount %.
@return Flat discount percentage */
public Decimal GetFlatDiscount() 
{
Object bd =Get_Value("FlatDiscount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** FreightCostRule AD_Reference_ID=153 */
public static int FREIGHTCOSTRULE_AD_Reference_ID=153;
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
public bool IsFreightCostRuleValid (String test)
{
return test == null || test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L");
}
/** Set Freight Cost Rule.
@param FreightCostRule Method for charging Freight */
public void SetFreightCostRule (String FreightCostRule)
{
if (!IsFreightCostRuleValid(FreightCostRule))
throw new ArgumentException ("FreightCostRule Invalid value - " + FreightCostRule + " - Reference_ID=153 - C - F - I - L");
if (FreightCostRule != null && FreightCostRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
FreightCostRule = FreightCostRule.Substring(0,1);
}
Set_Value ("FreightCostRule", FreightCostRule);
}
/** Get Freight Cost Rule.
@return Method for charging Freight */
public String GetFreightCostRule() 
{
return (String)Get_Value("FreightCostRule");
}

/** InvoiceRule AD_Reference_ID=150 */
public static int INVOICERULE_AD_Reference_ID=150;
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
public bool IsInvoiceRuleValid (String test)
{
return test == null || test.Equals("D") || test.Equals("I") || test.Equals("O") || test.Equals("S");
}
/** Set Invoicing Rule.
@param InvoiceRule Frequency and method of invoicing */
public void SetInvoiceRule (String InvoiceRule)
{
if (!IsInvoiceRuleValid(InvoiceRule))
throw new ArgumentException ("InvoiceRule Invalid value - " + InvoiceRule + " - Reference_ID=150 - D - I - O - S");
if (InvoiceRule != null && InvoiceRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
InvoiceRule = InvoiceRule.Substring(0,1);
}
Set_Value ("InvoiceRule", InvoiceRule);
}
/** Get Invoicing Rule.
@return Frequency and method of invoicing */
public String GetInvoiceRule() 
{
return (String)Get_Value("InvoiceRule");
}

/** Invoice_PrintFormat_ID AD_Reference_ID=261 */
public static int INVOICE_PRINTFORMAT_ID_AD_Reference_ID=261;
/** Set Invoice Print Format.
@param Invoice_PrintFormat_ID Print Format for printing Invoices */
public void SetInvoice_PrintFormat_ID (int Invoice_PrintFormat_ID)
{
if (Invoice_PrintFormat_ID <= 0) Set_Value ("Invoice_PrintFormat_ID", null);
else
Set_Value ("Invoice_PrintFormat_ID", Invoice_PrintFormat_ID);
}
/** Get Invoice Print Format.
@return Print Format for printing Invoices */
public int GetInvoice_PrintFormat_ID() 
{
Object ii = Get_Value("Invoice_PrintFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Customer.
@param IsCustomer Indicates if this Business Partner is a Customer */
public void SetIsCustomer (Boolean IsCustomer)
{
Set_Value ("IsCustomer", IsCustomer);
}
/** Get Customer.
@return Indicates if this Business Partner is a Customer */
public Boolean IsCustomer() 
{
Object oo = Get_Value("IsCustomer");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Discount Printed.
@param IsDiscountPrinted Print Discount on Invoice and Order */
public void SetIsDiscountPrinted (Boolean IsDiscountPrinted)
{
Set_Value ("IsDiscountPrinted", IsDiscountPrinted);
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
/** Set Employee.
@param IsEmployee Indicates if  this Business Partner is an employee */
public void SetIsEmployee (Boolean IsEmployee)
{
Set_Value ("IsEmployee", IsEmployee);
}
/** Get Employee.
@return Indicates if  this Business Partner is an employee */
public Boolean IsEmployee() 
{
Object oo = Get_Value("IsEmployee");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set One time transaction.
@param IsOneTime One time transaction */
public void SetIsOneTime (Boolean IsOneTime)
{
Set_Value ("IsOneTime", IsOneTime);
}
/** Get One time transaction.
@return One time transaction */
public Boolean IsOneTime() 
{
Object oo = Get_Value("IsOneTime");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Prospect.
@param IsProspect Indicates this is a Prospect */
public void SetIsProspect (Boolean IsProspect)
{
Set_Value ("IsProspect", IsProspect);
}
/** Get Prospect.
@return Indicates this is a Prospect */
public Boolean IsProspect() 
{
Object oo = Get_Value("IsProspect");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Representative.
@param IsSalesRep Indicates if  the business partner is a representative or company agent */
public void SetIsSalesRep (Boolean IsSalesRep)
{
Set_Value ("IsSalesRep", IsSalesRep);
}
/** Get Representative.
@return Indicates if  the business partner is a representative or company agent */
public Boolean IsSalesRep() 
{
Object oo = Get_Value("IsSalesRep");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Tax exempt.
@param IsTaxExempt Business partner is exempt from tax */
public void SetIsTaxExempt (Boolean IsTaxExempt)
{
Set_Value ("IsTaxExempt", IsTaxExempt);
}
/** Get Tax exempt.
@return Business partner is exempt from tax */
public Boolean IsTaxExempt() 
{
Object oo = Get_Value("IsTaxExempt");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Vendor.
@param IsVendor Indicates if this Business Partner is a Vendor */
public void SetIsVendor (Boolean IsVendor)
{
Set_Value ("IsVendor", IsVendor);
}
/** Get Vendor.
@return Indicates if this Business Partner is a Vendor */
public Boolean IsVendor() 
{
Object oo = Get_Value("IsVendor");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}

/** M_DiscountSchema_ID AD_Reference_ID=325 */
public static int M_DISCOUNTSCHEMA_ID_AD_Reference_ID=325;
/** Set Discount Schema.
@param M_DiscountSchema_ID Schema to calculate price lists or the trade discount percentage */
public void SetM_DiscountSchema_ID (int M_DiscountSchema_ID)
{
if (M_DiscountSchema_ID <= 0) Set_Value ("M_DiscountSchema_ID", null);
else
Set_Value ("M_DiscountSchema_ID", M_DiscountSchema_ID);
}
/** Get Discount Schema.
@return Schema to calculate price lists or the trade discount percentage */
public int GetM_DiscountSchema_ID() 
{
Object ii = Get_Value("M_DiscountSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Price List.
@param M_PriceList_ID Unique identifier of a Price List */
public void SetM_PriceList_ID (int M_PriceList_ID)
{
if (M_PriceList_ID <= 0) Set_Value ("M_PriceList_ID", null);
else
Set_Value ("M_PriceList_ID", M_PriceList_ID);
}
/** Get Price List.
@return Unique identifier of a Price List */
public int GetM_PriceList_ID() 
{
Object ii = Get_Value("M_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Return Policy.
@param M_ReturnPolicy_ID The Return Policy dictates the timeframe within which goods can be returned. */
public void SetM_ReturnPolicy_ID (int M_ReturnPolicy_ID)
{
if (M_ReturnPolicy_ID <= 0) Set_Value ("M_ReturnPolicy_ID", null);
else
Set_Value ("M_ReturnPolicy_ID", M_ReturnPolicy_ID);
}
/** Get Return Policy.
@return The Return Policy dictates the timeframe within which goods can be returned. */
public int GetM_ReturnPolicy_ID() 
{
Object ii = Get_Value("M_ReturnPolicy_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set NAICS/SIC.
@param NAICS Standard Industry Code or its successor NAIC - http://www.osha.gov/oshstats/sicser.html */
public void SetNAICS (String NAICS)
{
if (NAICS != null && NAICS.Length > 6)
{
log.Warning("Length > 6 - truncated");
NAICS = NAICS.Substring(0,6);
}
Set_Value ("NAICS", NAICS);
}
/** Get NAICS/SIC.
@return Standard Industry Code or its successor NAIC - http://www.osha.gov/oshstats/sicser.html */
public String GetNAICS() 
{
return (String)Get_Value("NAICS");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Name 2.
@param Name2 Additional Name */
public void SetName2 (String Name2)
{
if (Name2 != null && Name2.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name2 = Name2.Substring(0,60);
}
Set_Value ("Name2", Name2);
}
/** Get Name 2.
@return Additional Name */
public String GetName2() 
{
return (String)Get_Value("Name2");
}
/** Set Employees.
@param NumberEmployees Number of employees */
public void SetNumberEmployees (int NumberEmployees)
{
Set_Value ("NumberEmployees", NumberEmployees);
}
/** Get Employees.
@return Number of employees */
public int GetNumberEmployees() 
{
Object ii = Get_Value("NumberEmployees");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Order Reference.
@param POReference Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
public void SetPOReference (String POReference)
{
if (POReference != null && POReference.Length > 20)
{
log.Warning("Length > 20 - truncated");
POReference = POReference.Substring(0,20);
}
Set_Value ("POReference", POReference);
}
/** Get Order Reference.
@return Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
public String GetPOReference() 
{
return (String)Get_Value("POReference");
}

/** PO_DiscountSchema_ID AD_Reference_ID=325 */
public static int PO_DISCOUNTSCHEMA_ID_AD_Reference_ID=325;
/** Set PO Discount Schema.
@param PO_DiscountSchema_ID Schema to calculate the purchase trade discount percentage */
public void SetPO_DiscountSchema_ID (int PO_DiscountSchema_ID)
{
if (PO_DiscountSchema_ID <= 0) Set_Value ("PO_DiscountSchema_ID", null);
else
Set_Value ("PO_DiscountSchema_ID", PO_DiscountSchema_ID);
}
/** Get PO Discount Schema.
@return Schema to calculate the purchase trade discount percentage */
public int GetPO_DiscountSchema_ID() 
{
Object ii = Get_Value("PO_DiscountSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PO_PaymentTerm_ID AD_Reference_ID=227 */
public static int PO_PAYMENTTERM_ID_AD_Reference_ID=227;
/** Set PO Payment Term.
@param PO_PaymentTerm_ID Payment rules for a purchase order */
public void SetPO_PaymentTerm_ID (int PO_PaymentTerm_ID)
{
if (PO_PaymentTerm_ID <= 0) Set_Value ("PO_PaymentTerm_ID", null);
else
Set_Value ("PO_PaymentTerm_ID", PO_PaymentTerm_ID);
}
/** Get PO Payment Term.
@return Payment rules for a purchase order */
public int GetPO_PaymentTerm_ID() 
{
Object ii = Get_Value("PO_PaymentTerm_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PO_PriceList_ID AD_Reference_ID=166 */
public static int PO_PRICELIST_ID_AD_Reference_ID=166;
/** Set Purchase Pricelist.
@param PO_PriceList_ID Price List used by this Business Partner */
public void SetPO_PriceList_ID (int PO_PriceList_ID)
{
if (PO_PriceList_ID <= 0) Set_Value ("PO_PriceList_ID", null);
else
Set_Value ("PO_PriceList_ID", PO_PriceList_ID);
}
/** Get Purchase Pricelist.
@return Price List used by this Business Partner */
public int GetPO_PriceList_ID() 
{
Object ii = Get_Value("PO_PriceList_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PO_ReturnPolicy_ID AD_Reference_ID=431 */
public static int PO_RETURNPOLICY_ID_AD_Reference_ID=431;
/** Set Vendor Return Policy.
@param PO_ReturnPolicy_ID Vendor Return Policy */
public void SetPO_ReturnPolicy_ID (int PO_ReturnPolicy_ID)
{
if (PO_ReturnPolicy_ID <= 0) Set_Value ("PO_ReturnPolicy_ID", null);
else
Set_Value ("PO_ReturnPolicy_ID", PO_ReturnPolicy_ID);
}
/** Get Vendor Return Policy.
@return Vendor Return Policy */
public int GetPO_ReturnPolicy_ID() 
{
Object ii = Get_Value("PO_ReturnPolicy_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PaymentRule AD_Reference_ID=195 */
public static int PAYMENTRULE_AD_Reference_ID=195;
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
public bool IsPaymentRuleValid (String test)
{
return test == null || test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
}
/** Set Payment Method.
@param PaymentRule How you pay the invoice */
public void SetPaymentRule (String PaymentRule)
{
if (!IsPaymentRuleValid(PaymentRule))
throw new ArgumentException ("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - P - S - T");
if (PaymentRule != null && PaymentRule.Length > 1)
{
log.Warning("Length > 1 - truncated");
PaymentRule = PaymentRule.Substring(0,1);
}
Set_Value ("PaymentRule", PaymentRule);
}
/** Get Payment Method.
@return How you pay the invoice */
public String GetPaymentRule() 
{
return (String)Get_Value("PaymentRule");
}

/** PaymentRulePO AD_Reference_ID=195 */
public static int PAYMENTRULEPO_AD_Reference_ID=195;
/** Cash = B */
public static String PAYMENTRULEPO_Cash = "B";
/** Direct Debit = D */
public static String PAYMENTRULEPO_DirectDebit = "D";
/** Credit Card = K */
public static String PAYMENTRULEPO_CreditCard = "K";
/** On Credit = P */
public static String PAYMENTRULEPO_OnCredit = "P";
/** Check = S */
public static String PAYMENTRULEPO_Check = "S";
/** Direct Deposit = T */
public static String PAYMENTRULEPO_DirectDeposit = "T";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPaymentRulePOValid (String test)
{
return test == null || test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
}
/** Set Payment Rule.
@param PaymentRulePO Purchase payment option */
public void SetPaymentRulePO (String PaymentRulePO)
{
if (!IsPaymentRulePOValid(PaymentRulePO))
throw new ArgumentException ("PaymentRulePO Invalid value - " + PaymentRulePO + " - Reference_ID=195 - B - D - K - P - S - T");
if (PaymentRulePO != null && PaymentRulePO.Length > 1)
{
log.Warning("Length > 1 - truncated");
PaymentRulePO = PaymentRulePO.Substring(0,1);
}
Set_Value ("PaymentRulePO", PaymentRulePO);
}
/** Get Payment Rule.
@return Purchase payment option */
public String GetPaymentRulePO() 
{
return (String)Get_Value("PaymentRulePO");
}
/** Set Potential Life Time Value.
@param PotentialLifeTimeValue Total Revenue expected */
public void SetPotentialLifeTimeValue (Decimal? PotentialLifeTimeValue)
{
Set_Value ("PotentialLifeTimeValue", (Decimal?)PotentialLifeTimeValue);
}
/** Get Potential Life Time Value.
@return Total Revenue expected */
public Decimal GetPotentialLifeTimeValue() 
{
Object bd =Get_Value("PotentialLifeTimeValue");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** Rating AD_Reference_ID=419 */
public static int RATING_AD_Reference_ID=419;
/** Not Rated = - */
public static String RATING_NotRated = "-";
/** A = A */
public static String RATING_A = "A";
/** B = B */
public static String RATING_B = "B";
/** C = C */
public static String RATING_C = "C";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsRatingValid (String test)
{
return test == null || test.Equals("-") || test.Equals("A") || test.Equals("B") || test.Equals("C");
}
/** Set Rating.
@param Rating Classification or Importance */
public void SetRating (String Rating)
{
if (!IsRatingValid(Rating))
throw new ArgumentException ("Rating Invalid value - " + Rating + " - Reference_ID=419 - - - A - B - C");
if (Rating != null && Rating.Length > 1)
{
log.Warning("Length > 1 - truncated");
Rating = Rating.Substring(0,1);
}
Set_Value ("Rating", Rating);
}
/** Get Rating.
@return Classification or Importance */
public String GetRating() 
{
return (String)Get_Value("Rating");
}
/** Set Reference No.
@param ReferenceNo Your customer or vendor number at the Business Partner's site */
public void SetReferenceNo (String ReferenceNo)
{
if (ReferenceNo != null && ReferenceNo.Length > 40)
{
log.Warning("Length > 40 - truncated");
ReferenceNo = ReferenceNo.Substring(0,40);
}
Set_Value ("ReferenceNo", ReferenceNo);
}
/** Get Reference No.
@return Your customer or vendor number at the Business Partner's site */
public String GetReferenceNo() 
{
return (String)Get_Value("ReferenceNo");
}

/** SOCreditStatus AD_Reference_ID=289 */
public static int SOCREDITSTATUS_AD_Reference_ID=289;
/** Credit Hold = H */
public static String SOCREDITSTATUS_CreditHold = "H";
/** Credit OK = O */
public static String SOCREDITSTATUS_CreditOK = "O";
/** Credit Stop = S */
public static String SOCREDITSTATUS_CreditStop = "S";
/** Credit Watch = W */
public static String SOCREDITSTATUS_CreditWatch = "W";
/** No Credit Check = X */
public static String SOCREDITSTATUS_NoCreditCheck = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsSOCreditStatusValid (String test)
{
return test == null || test.Equals("H") || test.Equals("O") || test.Equals("S") || test.Equals("W") || test.Equals("X");
}
/** Set Credit Status.
@param SOCreditStatus Business Partner Credit Status */
public void SetSOCreditStatus (String SOCreditStatus)
{
if (!IsSOCreditStatusValid(SOCreditStatus))
throw new ArgumentException ("SOCreditStatus Invalid value - " + SOCreditStatus + " - Reference_ID=289 - H - O - S - W - X");
if (SOCreditStatus != null && SOCreditStatus.Length > 1)
{
log.Warning("Length > 1 - truncated");
SOCreditStatus = SOCreditStatus.Substring(0,1);
}
Set_Value ("SOCreditStatus", SOCreditStatus);
}
/** Get Credit Status.
@return Business Partner Credit Status */
public String GetSOCreditStatus() 
{
return (String)Get_Value("SOCreditStatus");
}
/** Set Credit Limit.
@param SO_CreditLimit Total outstanding invoice amounts allowed */
public void SetSO_CreditLimit (Decimal? SO_CreditLimit)
{
if (SO_CreditLimit == null) throw new ArgumentException ("SO_CreditLimit is mandatory.");
Set_Value ("SO_CreditLimit", (Decimal?)SO_CreditLimit);
}
/** Get Credit Limit.
@return Total outstanding invoice amounts allowed */
public Decimal GetSO_CreditLimit() 
{
Object bd =Get_Value("SO_CreditLimit");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Credit Used.
@param SO_CreditUsed Current open balance */
public void SetSO_CreditUsed (Decimal? SO_CreditUsed)
{
if (SO_CreditUsed == null) throw new ArgumentException ("SO_CreditUsed is mandatory.");
Set_ValueNoCheck ("SO_CreditUsed", (Decimal?)SO_CreditUsed);
}
/** Get Credit Used.
@return Current open balance */
public Decimal GetSO_CreditUsed() 
{
Object bd =Get_Value("SO_CreditUsed");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Order Description.
@param SO_Description Description to be used on orders */
public void SetSO_Description (String SO_Description)
{
if (SO_Description != null && SO_Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
SO_Description = SO_Description.Substring(0,255);
}
Set_Value ("SO_Description", SO_Description);
}
/** Get Order Description.
@return Description to be used on orders */
public String GetSO_Description() 
{
return (String)Get_Value("SO_Description");
}

/** SalesRep_ID AD_Reference_ID=190 */
public static int SALESREP_ID_AD_Reference_ID=190;
/** Set Representative.
@param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public void SetSalesRep_ID (int SalesRep_ID)
{
if (SalesRep_ID <= 0) Set_Value ("SalesRep_ID", null);
else
Set_Value ("SalesRep_ID", SalesRep_ID);
}
/** Get Representative.
@return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
public int GetSalesRep_ID() 
{
Object ii = Get_Value("SalesRep_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Volume.
@param SalesVolume Total Volume of Sales in Thousands of Base Currency */
public void SetSalesVolume (int SalesVolume)
{
Set_Value ("SalesVolume", SalesVolume);
}
/** Get Sales Volume.
@return Total Volume of Sales in Thousands of Base Currency */
public int GetSalesVolume() 
{
Object ii = Get_Value("SalesVolume");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Send EMail.
@param SendEMail Enable sending Document EMail */
public void SetSendEMail (Boolean SendEMail)
{
Set_Value ("SendEMail", SendEMail);
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
/** Set Share.
@param ShareOfCustomer Share of Customer's business as a percentage */
public void SetShareOfCustomer (int ShareOfCustomer)
{
Set_Value ("ShareOfCustomer", ShareOfCustomer);
}
/** Get Share.
@return Share of Customer's business as a percentage */
public int GetShareOfCustomer() 
{
Object ii = Get_Value("ShareOfCustomer");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Min Shelf Life %.
@param ShelfLifeMinPct Minimum Shelf Life in percent based on Product Instance Guarantee Date */
public void SetShelfLifeMinPct (int ShelfLifeMinPct)
{
Set_Value ("ShelfLifeMinPct", ShelfLifeMinPct);
}
/** Get Min Shelf Life %.
@return Minimum Shelf Life in percent based on Product Instance Guarantee Date */
public int GetShelfLifeMinPct() 
{
Object ii = Get_Value("ShelfLifeMinPct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Street.
@param Street Street */
public void SetStreet (String Street)
{
if (Street != null && Street.Length > 50)
{
log.Warning("Length > 50 - truncated");
Street = Street.Substring(0,50);
}
Set_Value ("Street", Street);
}
/** Get Street.
@return Street */
public String GetStreet() 
{
return (String)Get_Value("Street");
}
/** Set Tax ID.
@param TaxID Tax Identification */
public void SetTaxID (String TaxID)
{
if (TaxID != null && TaxID.Length > 20)
{
log.Warning("Length > 20 - truncated");
TaxID = TaxID.Substring(0,20);
}
Set_Value ("TaxID", TaxID);
}
/** Get Tax ID.
@return Tax Identification */
public String GetTaxID() 
{
return (String)Get_Value("TaxID");
}
/** Set Open Balance.
@param TotalOpenBalance Total Open Balance Amount in primary Accounting Currency */
public void SetTotalOpenBalance (Decimal? TotalOpenBalance)
{
Set_Value ("TotalOpenBalance", (Decimal?)TotalOpenBalance);
}
/** Get Open Balance.
@return Total Open Balance Amount in primary Accounting Currency */
public Decimal GetTotalOpenBalance() 
{
Object bd =Get_Value("TotalOpenBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set URL.
@param URL Full URL address - e.g. http://www.viennaadvantage.com */
public void SetURL (String URL)
{
if (URL != null && URL.Length > 120)
{
log.Warning("Length > 120 - truncated");
URL = URL.Substring(0,120);
}
Set_Value ("URL", URL);
}
/** Get URL.
@return Full URL address - e.g. http://www.viennaadvantage.com */
public String GetURL() 
{
return (String)Get_Value("URL");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
/** Set ZIP.
@param ZIP ZIP */
public void SetZIP (String ZIP)
{
if (ZIP != null && ZIP.Length > 50)
{
log.Warning("Length > 50 - truncated");
ZIP = ZIP.Substring(0,50);
}
Set_Value ("ZIP", ZIP);
}
/** Get ZIP.
@return ZIP */
public String GetZIP() 
{
return (String)Get_Value("ZIP");
}

/** Set Pic.
@param Pic Identifies a Pic */
public void SetPic(String Pic)
{
    if (Pic != null && Pic.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        Pic = Pic.Substring(0, 50);
    }
    Set_Value("Pic", Pic);
}
/** Get Pic.
@return Identifies a Pic */
public String GetPic()
{
    return (String)Get_Value("Pic");
}

/** Set C_Campaign_ID.
@param C_Campaign_ID */
public void SetC_Campaign_ID(int C_Campaign_ID)
{
    if (C_Campaign_ID < 1) throw new ArgumentException("C_Campaign_ID is mandatory.");
    Set_Value("C_Campaign_ID", C_Campaign_ID);
}
/** Get C_Campaign_ID.
@return C_Campaign_ID */
public int GetC_Campaign_ID()
{
    Object ii = Get_Value("C_Campaign_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set BPCODE.
@param BPCODE BPCODE */
public void SetBPCODE(String BPCODE)
{
    if (BPCODE != null && BPCODE.Length > 20)
    {
        log.Warning("Length > 20 - truncated");
        BPCODE = BPCODE.Substring(0, 20);
    }
    Set_Value("BPCODE", BPCODE);
}
/** Get BPCODE.
@return BPCODE */
public String GetBPCODE()
{
    return (String)Get_Value("BPCODE");
}

/** Set C_CONSOLIDATIONREFERENCE_ID.
@param C_CONSOLIDATIONREFERENCE_ID C_CONSOLIDATIONREFERENCE_ID */
public void SetC_CONSOLIDATIONREFERENCE_ID(int C_CONSOLIDATIONREFERENCE_ID)
{
    if (C_CONSOLIDATIONREFERENCE_ID <= 0) Set_Value("C_CONSOLIDATIONREFERENCE_ID", null);
    else
        Set_Value("C_CONSOLIDATIONREFERENCE_ID", C_CONSOLIDATIONREFERENCE_ID);
}
/** Get C_CONSOLIDATIONREFERENCE_ID.
@return C_CONSOLIDATIONREFERENCE_ID */
public int GetC_CONSOLIDATIONREFERENCE_ID()
{
    Object ii = Get_Value("C_CONSOLIDATIONREFERENCE_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Country.
@param C_Country_ID Country  */
public void SetC_Country_ID(int C_Country_ID)
{
    if (C_Country_ID <= 0) Set_Value("C_Country_ID", null);
    else
        Set_Value("C_Country_ID", C_Country_ID);
}
/** Get Country.
@return Country  */
public int GetC_Country_ID()
{
    Object ii = Get_Value("C_Country_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set Position.
@param C_Job_ID Job Position */
public void SetC_Job_ID(int C_Job_ID)
{
    if (C_Job_ID <= 0) Set_Value("C_Job_ID", null);
    else
        Set_Value("C_Job_ID", C_Job_ID);
}
/** Get Position.
@return Job Position */
public int GetC_Job_ID()
{
    Object ii = Get_Value("C_Job_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID(int C_Location_ID)
{
    if (C_Location_ID <= 0) Set_Value("C_Location_ID", null);
    else
        Set_Value("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID()
{
    Object ii = Get_Value("C_Location_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set C_PERSONALINFO_ID.
@param C_PERSONALINFO_ID C_PERSONALINFO_ID */
public void SetC_PERSONALINFO_ID(int C_PERSONALINFO_ID)
{
    if (C_PERSONALINFO_ID <= 0) Set_Value("C_PERSONALINFO_ID", null);
    else
        Set_Value("C_PERSONALINFO_ID", C_PERSONALINFO_ID);
}
/** Get C_PERSONALINFO_ID.
@return C_PERSONALINFO_ID */
public int GetC_PERSONALINFO_ID()
{
    Object ii = Get_Value("C_PERSONALINFO_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}

/** Set EMPLOYEE_FILTER.
@param EMPLOYEE_FILTER EMPLOYEE_FILTER */
public void SetEMPLOYEE_FILTER(Boolean EMPLOYEE_FILTER)
{
    Set_Value("EMPLOYEE_FILTER", EMPLOYEE_FILTER);
}
/** Get EMPLOYEE_FILTER.
@return EMPLOYEE_FILTER */
public Boolean IsEMPLOYEE_FILTER()
{
    Object oo = Get_Value("EMPLOYEE_FILTER");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}
/** Set EMPLOYEE_GRADE.
@param EMPLOYEE_GRADE EMPLOYEE_GRADE */
public void SetEMPLOYEE_GRADE(String EMPLOYEE_GRADE)
{
    if (EMPLOYEE_GRADE != null && EMPLOYEE_GRADE.Length > 5)
    {
        log.Warning("Length > 5 - truncated");
        EMPLOYEE_GRADE = EMPLOYEE_GRADE.Substring(0, 5);
    }
    Set_Value("EMPLOYEE_GRADE", EMPLOYEE_GRADE);
}
/** Get EMPLOYEE_GRADE.
@return EMPLOYEE_GRADE */
public String GetEMPLOYEE_GRADE()
{
    return (String)Get_Value("EMPLOYEE_GRADE");
}

/** EMPLOYEE_STATUS AD_Reference_ID=1000009 */
public static int EMPLOYEE_STATUS_AD_Reference_ID = 1000009;
/** Bachelor = B */
public static String EMPLOYEE_STATUS_Bachelor = "B";
/** Family = F */
public static String EMPLOYEE_STATUS_Family = "F";
/** Labour = L */
public static String EMPLOYEE_STATUS_Labour = "L";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsEMPLOYEE_STATUSValid(String test)
{
    return test == null || test.Equals("B") || test.Equals("F") || test.Equals("L");
}
/** Set EMPLOYEE_STATUS.
@param EMPLOYEE_STATUS EMPLOYEE_STATUS */
public void SetEMPLOYEE_STATUS(String EMPLOYEE_STATUS)
{
    if (!IsEMPLOYEE_STATUSValid(EMPLOYEE_STATUS))
        throw new ArgumentException("EMPLOYEE_STATUS Invalid value - " + EMPLOYEE_STATUS + " - Reference_ID=1000009 - B - F - L");
    if (EMPLOYEE_STATUS != null && EMPLOYEE_STATUS.Length > 1)
    {
        log.Warning("Length > 1 - truncated");
        EMPLOYEE_STATUS = EMPLOYEE_STATUS.Substring(0, 1);
    }
    Set_Value("EMPLOYEE_STATUS", EMPLOYEE_STATUS);
}
/** Get EMPLOYEE_STATUS.
@return EMPLOYEE_STATUS */
public String GetEMPLOYEE_STATUS()
{
    return (String)Get_Value("EMPLOYEE_STATUS");
}

/** Set Mobile.
@param Mobile Mobile */
public void SetMobile(String Mobile)
{
    if (Mobile != null && Mobile.Length > 50)
    {
        log.Warning("Length > 50 - truncated");
        Mobile = Mobile.Substring(0, 50);
    }
    Set_Value("Mobile", Mobile);
}
/** Get Mobile.
@return Mobile */
public String GetMobile()
{
    return (String)Get_Value("Mobile");
}
/** SALARYPROCESS AD_Reference_ID=1000010 */
public static int SALARYPROCESS_AD_Reference_ID = 1000010;
/** Direct Deposit = A */
public static String SALARYPROCESS_DirectDeposit = "A";
/** Cash = C */
public static String SALARYPROCESS_Cash = "C";
/** Cheque = K */
public static String SALARYPROCESS_Cheque = "K";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsSALARYPROCESSValid(String test)
{
    return test == null || test.Equals("A") || test.Equals("C") || test.Equals("K");
}
/** Set Salary Process.
@param SALARYPROCESS Salary Process */
public void SetSALARYPROCESS(String SALARYPROCESS)
{
    if (!IsSALARYPROCESSValid(SALARYPROCESS))
        throw new ArgumentException("SALARYPROCESS Invalid value - " + SALARYPROCESS + " - Reference_ID=1000010 - A - C - K");
    if (SALARYPROCESS != null && SALARYPROCESS.Length > 1)
    {
        log.Warning("Length > 1 - truncated");
        SALARYPROCESS = SALARYPROCESS.Substring(0, 1);
    }
    Set_Value("SALARYPROCESS", SALARYPROCESS);
}
/** Get Salary Process.
@return Salary Process */
public String GetSALARYPROCESS()
{
    return (String)Get_Value("SALARYPROCESS");
}
/** Set UNIQUEEMPCODE.
@param UNIQUEEMPCODE UNIQUEEMPCODE */
public void SetUNIQUEEMPCODE(int UNIQUEEMPCODE)
{
    Set_Value("UNIQUEEMPCODE", UNIQUEEMPCODE);
}
/** Get UNIQUEEMPCODE.
@return UNIQUEEMPCODE */
public int GetUNIQUEEMPCODE()
{
    Object ii = Get_Value("UNIQUEEMPCODE");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
/** Set UnivRegNo.
@param UNIVREGNO UnivRegNo */
public void SetUNIVREGNO(String UNIVREGNO)
{
    if (UNIVREGNO != null && UNIVREGNO.Length > 40)
    {
        log.Warning("Length > 40 - truncated");
        UNIVREGNO = UNIVREGNO.Substring(0, 40);
    }
    Set_Value("UNIVREGNO", UNIVREGNO);
}
/** Get UnivRegNo.
@return UnivRegNo */
public String GetUNIVREGNO()
{
    return (String)Get_Value("UNIVREGNO");
}

/** VENDOR_NAME AD_Reference_ID=192 */
public static int VENDOR_NAME_AD_Reference_ID = 192;
/** Set VENDOR_NAME.
@param VENDOR_NAME VENDOR_NAME */
public void SetVENDOR_NAME(int VENDOR_NAME)
{
    Set_Value("VENDOR_NAME", VENDOR_NAME);
}
/** Get VENDOR_NAME.
@return VENDOR_NAME */
public int GetVENDOR_NAME()
{
    Object ii = Get_Value("VENDOR_NAME");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
