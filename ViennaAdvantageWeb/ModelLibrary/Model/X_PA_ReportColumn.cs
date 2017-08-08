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
/** Generated Model for PA_ReportColumn
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_ReportColumn : PO
{
public X_PA_ReportColumn (Context ctx, int PA_ReportColumn_ID, Trx trxName) : base (ctx, PA_ReportColumn_ID, trxName)
{
/** if (PA_ReportColumn_ID == 0)
{
SetColumnType (null);	// R
SetIsPrinted (true);	// Y
SetName (null);
SetPA_ReportColumnSet_ID (0);
SetPA_ReportColumn_ID (0);
SetPostingType (null);	// A
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM PA_ReportColumn WHERE PA_ReportColumnSet_ID=@PA_ReportColumnSet_ID@
}
 */
}
public X_PA_ReportColumn (Ctx ctx, int PA_ReportColumn_ID, Trx trxName) : base (ctx, PA_ReportColumn_ID, trxName)
{
/** if (PA_ReportColumn_ID == 0)
{
SetColumnType (null);	// R
SetIsPrinted (true);	// Y
SetName (null);
SetPA_ReportColumnSet_ID (0);
SetPA_ReportColumn_ID (0);
SetPostingType (null);	// A
SetSeqNo (0);	// @SQL=SELECT NVL(MAX(SeqNo),0)+10 AS DefaultValue FROM PA_ReportColumn WHERE PA_ReportColumnSet_ID=@PA_ReportColumnSet_ID@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ReportColumn (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ReportColumn (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ReportColumn (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_ReportColumn()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382092L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065303L;
/** AD_Table_ID=446 */
public static int Table_ID;
 // =446;

/** TableName=PA_ReportColumn */
public static String Table_Name="PA_ReportColumn";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_PA_ReportColumn[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AmountType AD_Reference_ID=235 */
public static int AMOUNTTYPE_AD_Reference_ID=235;
/** Period Balance = BP */
public static String AMOUNTTYPE_PeriodBalance = "BP";
/** Total Balance = BT */
public static String AMOUNTTYPE_TotalBalance = "BT";
/** Year Balance = BY */
public static String AMOUNTTYPE_YearBalance = "BY";
/** Period Credit Only = CP */
public static String AMOUNTTYPE_PeriodCreditOnly = "CP";
/** Total Credit Only = CT */
public static String AMOUNTTYPE_TotalCreditOnly = "CT";
/** Year Credit Only = CY */
public static String AMOUNTTYPE_YearCreditOnly = "CY";
/** Period Debit Only = DP */
public static String AMOUNTTYPE_PeriodDebitOnly = "DP";
/** Total Debit Only = DT */
public static String AMOUNTTYPE_TotalDebitOnly = "DT";
/** Year Debit Only = DY */
public static String AMOUNTTYPE_YearDebitOnly = "DY";
/** Period Quantity = QP */
public static String AMOUNTTYPE_PeriodQuantity = "QP";
/** Total Quantity = QT */
public static String AMOUNTTYPE_TotalQuantity = "QT";
/** Year Quantity = QY */
public static String AMOUNTTYPE_YearQuantity = "QY";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAmountTypeValid (String test)
{
return test == null || test.Equals("BP") || test.Equals("BT") || test.Equals("BY") || test.Equals("CP") || test.Equals("CT") || test.Equals("CY") || test.Equals("DP") || test.Equals("DT") || test.Equals("DY") || test.Equals("QP") || test.Equals("QT") || test.Equals("QY");
}
/** Set Amount Type.
@param AmountType Type of amount to report */
public void SetAmountType (String AmountType)
{
if (!IsAmountTypeValid(AmountType))
throw new ArgumentException ("AmountType Invalid value - " + AmountType + " - Reference_ID=235 - BP - BT - BY - CP - CT - CY - DP - DT - DY - QP - QT - QY");
if (AmountType != null && AmountType.Length > 2)
{
log.Warning("Length > 2 - truncated");
AmountType = AmountType.Substring(0,2);
}
Set_Value ("AmountType", AmountType);
}
/** Get Amount Type.
@return Type of amount to report */
public String GetAmountType() 
{
return (String)Get_Value("AmountType");
}
/** Set Activity.
@param C_Activity_ID Business Activity */
public void SetC_Activity_ID (int C_Activity_ID)
{
if (C_Activity_ID <= 0) Set_Value ("C_Activity_ID", null);
else
Set_Value ("C_Activity_ID", C_Activity_ID);
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
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID <= 0) Set_Value ("C_BPartner_ID", null);
else
Set_Value ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Campaign.
@param C_Campaign_ID Marketing Campaign */
public void SetC_Campaign_ID (int C_Campaign_ID)
{
if (C_Campaign_ID <= 0) Set_Value ("C_Campaign_ID", null);
else
Set_Value ("C_Campaign_ID", C_Campaign_ID);
}
/** Get Campaign.
@return Marketing Campaign */
public int GetC_Campaign_ID() 
{
Object ii = Get_Value("C_Campaign_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID <= 0) Set_Value ("C_Currency_ID", null);
else
Set_Value ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID)
{
if (C_ElementValue_ID <= 0) Set_Value ("C_ElementValue_ID", null);
else
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);
}
/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() 
{
Object ii = Get_Value("C_ElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID)
{
if (C_Location_ID <= 0) Set_Value ("C_Location_ID", null);
else
Set_Value ("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID() 
{
Object ii = Get_Value("C_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sales Region.
@param C_SalesRegion_ID Sales coverage region */
public void SetC_SalesRegion_ID (int C_SalesRegion_ID)
{
if (C_SalesRegion_ID <= 0) Set_Value ("C_SalesRegion_ID", null);
else
Set_Value ("C_SalesRegion_ID", C_SalesRegion_ID);
}
/** Get Sales Region.
@return Sales coverage region */
public int GetC_SalesRegion_ID() 
{
Object ii = Get_Value("C_SalesRegion_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** CalculationType AD_Reference_ID=236 */
public static int CALCULATIONTYPE_AD_Reference_ID=236;
/** Add (Op1+Op2) = A */
public static String CALCULATIONTYPE_AddOp1PlusOp2 = "A";
/** Percentage (Op1 of Op2) = P */
public static String CALCULATIONTYPE_PercentageOp1OfOp2 = "P";
/** Add Range (Op1 to Op2) = R */
public static String CALCULATIONTYPE_AddRangeOp1ToOp2 = "R";
/** Subtract (Op1-Op2) = S */
public static String CALCULATIONTYPE_SubtractOp1_Op2 = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCalculationTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("P") || test.Equals("R") || test.Equals("S");
}
/** Set Calculation.
@param CalculationType Calculation */
public void SetCalculationType (String CalculationType)
{
if (!IsCalculationTypeValid(CalculationType))
throw new ArgumentException ("CalculationType Invalid value - " + CalculationType + " - Reference_ID=236 - A - P - R - S");
if (CalculationType != null && CalculationType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CalculationType = CalculationType.Substring(0,1);
}
Set_Value ("CalculationType", CalculationType);
}
/** Get Calculation.
@return Calculation */
public String GetCalculationType() 
{
return (String)Get_Value("CalculationType");
}

/** ColumnType AD_Reference_ID=237 */
public static int COLUMNTYPE_AD_Reference_ID=237;
/** Calculation = C */
public static String COLUMNTYPE_Calculation = "C";
/** Relative Period = R */
public static String COLUMNTYPE_RelativePeriod = "R";
/** Segment Value = S */
public static String COLUMNTYPE_SegmentValue = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsColumnTypeValid (String test)
{
return test.Equals("C") || test.Equals("R") || test.Equals("S");
}
/** Set Column Type.
@param ColumnType Column Type */
public void SetColumnType (String ColumnType)
{
if (ColumnType == null) throw new ArgumentException ("ColumnType is mandatory");
if (!IsColumnTypeValid(ColumnType))
throw new ArgumentException ("ColumnType Invalid value - " + ColumnType + " - Reference_ID=237 - C - R - S");
if (ColumnType.Length > 1)
{
log.Warning("Length > 1 - truncated");
ColumnType = ColumnType.Substring(0,1);
}
Set_Value ("ColumnType", ColumnType);
}
/** Get Column Type.
@return Column Type */
public String GetColumnType() 
{
return (String)Get_Value("ColumnType");
}

/** CurrencyType AD_Reference_ID=238 */
public static int CURRENCYTYPE_AD_Reference_ID=238;
/** Accounting Currency = A */
public static String CURRENCYTYPE_AccountingCurrency = "A";
/** Source Currency = S */
public static String CURRENCYTYPE_SourceCurrency = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCurrencyTypeValid (String test)
{
return test == null || test.Equals("A") || test.Equals("S");
}
/** Set Currency Conversion Type.
@param CurrencyType Currency Conversion Type */
public void SetCurrencyType (String CurrencyType)
{
if (!IsCurrencyTypeValid(CurrencyType))
throw new ArgumentException ("CurrencyType Invalid value - " + CurrencyType + " - Reference_ID=238 - A - S");
if (CurrencyType != null && CurrencyType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CurrencyType = CurrencyType.Substring(0,1);
}
Set_Value ("CurrencyType", CurrencyType);
}
/** Get Currency Conversion Type.
@return Currency Conversion Type */
public String GetCurrencyType() 
{
return (String)Get_Value("CurrencyType");
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

/** ElementType AD_Reference_ID=181 */
public static int ELEMENTTYPE_AD_Reference_ID=181;
/** Account = AC */
public static String ELEMENTTYPE_Account = "AC";
/** Activity = AY */
public static String ELEMENTTYPE_Activity = "AY";
/** BPartner = BP */
public static String ELEMENTTYPE_BPartner = "BP";
/** Location From = LF */
public static String ELEMENTTYPE_LocationFrom = "LF";
/** Location To = LT */
public static String ELEMENTTYPE_LocationTo = "LT";
/** Campaign = MC */
public static String ELEMENTTYPE_Campaign = "MC";
/** Organization = OO */
public static String ELEMENTTYPE_Organization = "OO";
/** Org Trx = OT */
public static String ELEMENTTYPE_OrgTrx = "OT";
/** Project = PJ */
public static String ELEMENTTYPE_Project = "PJ";
/** Product = PR */
public static String ELEMENTTYPE_Product = "PR";
/** Sub Account = SA */
public static String ELEMENTTYPE_SubAccount = "SA";
/** Sales Region = SR */
public static String ELEMENTTYPE_SalesRegion = "SR";
/** User List 1 = U1 */
public static String ELEMENTTYPE_UserList1 = "U1";
/** User List 2 = U2 */
public static String ELEMENTTYPE_UserList2 = "U2";
/** User Element 1 = X1 */
public static String ELEMENTTYPE_UserElement1 = "X1";
/** User Element 2 = X2 */
public static String ELEMENTTYPE_UserElement2 = "X2";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsElementTypeValid (String test)
{
return test == null || test.Equals("AC") || test.Equals("AY") || test.Equals("BP") || test.Equals("LF") || test.Equals("LT") || test.Equals("MC") || test.Equals("OO") || test.Equals("OT") || test.Equals("PJ") || test.Equals("PR") || test.Equals("SA") || test.Equals("SR") || test.Equals("U1") || test.Equals("U2") || test.Equals("X1") || test.Equals("X2");
}
/** Set Type.
@param ElementType Element Type (account or user defined) */
public void SetElementType (String ElementType)
{
if (!IsElementTypeValid(ElementType))
throw new ArgumentException ("ElementType Invalid value - " + ElementType + " - Reference_ID=181 - AC - AY - BP - LF - LT - MC - OO - OT - PJ - PR - SA - SR - U1 - U2 - X1 - X2");
if (ElementType != null && ElementType.Length > 2)
{
log.Warning("Length > 2 - truncated");
ElementType = ElementType.Substring(0,2);
}
Set_Value ("ElementType", ElementType);
}
/** Get Type.
@return Element Type (account or user defined) */
public String GetElementType() 
{
return (String)Get_Value("ElementType");
}
/** Set Budget.
@param GL_Budget_ID General Ledger Budget */
public void SetGL_Budget_ID (int GL_Budget_ID)
{
if (GL_Budget_ID <= 0) Set_Value ("GL_Budget_ID", null);
else
Set_Value ("GL_Budget_ID", GL_Budget_ID);
}
/** Get Budget.
@return General Ledger Budget */
public int GetGL_Budget_ID() 
{
Object ii = Get_Value("GL_Budget_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Adhoc Conversion.
@param IsAdhocConversion Perform conversion for all amounts to currency */
public void SetIsAdhocConversion (Boolean IsAdhocConversion)
{
Set_Value ("IsAdhocConversion", IsAdhocConversion);
}
/** Get Adhoc Conversion.
@return Perform conversion for all amounts to currency */
public Boolean IsAdhocConversion() 
{
Object oo = Get_Value("IsAdhocConversion");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Printed.
@param IsPrinted Indicates if this document / line is printed */
public void SetIsPrinted (Boolean IsPrinted)
{
Set_Value ("IsPrinted", IsPrinted);
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
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);
else
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** Oper_1_ID AD_Reference_ID=239 */
public static int OPER_1_ID_AD_Reference_ID=239;
/** Set Operand 1.
@param Oper_1_ID First operand for calculation */
public void SetOper_1_ID (int Oper_1_ID)
{
if (Oper_1_ID <= 0) Set_Value ("Oper_1_ID", null);
else
Set_Value ("Oper_1_ID", Oper_1_ID);
}
/** Get Operand 1.
@return First operand for calculation */
public int GetOper_1_ID() 
{
Object ii = Get_Value("Oper_1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Oper_2_ID AD_Reference_ID=239 */
public static int OPER_2_ID_AD_Reference_ID=239;
/** Set Operand 2.
@param Oper_2_ID Second operand for calculation */
public void SetOper_2_ID (int Oper_2_ID)
{
if (Oper_2_ID <= 0) Set_Value ("Oper_2_ID", null);
else
Set_Value ("Oper_2_ID", Oper_2_ID);
}
/** Get Operand 2.
@return Second operand for calculation */
public int GetOper_2_ID() 
{
Object ii = Get_Value("Oper_2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Org_ID AD_Reference_ID=130 */
public static int ORG_ID_AD_Reference_ID=130;
/** Set Organization.
@param Org_ID Organizational entity within client */
public void SetOrg_ID (int Org_ID)
{
if (Org_ID <= 0) Set_Value ("Org_ID", null);
else
Set_Value ("Org_ID", Org_ID);
}
/** Get Organization.
@return Organizational entity within client */
public int GetOrg_ID() 
{
Object ii = Get_Value("Org_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Column Set.
@param PA_ReportColumnSet_ID Collection of Columns for Report */
public void SetPA_ReportColumnSet_ID (int PA_ReportColumnSet_ID)
{
if (PA_ReportColumnSet_ID < 1) throw new ArgumentException ("PA_ReportColumnSet_ID is mandatory.");
Set_ValueNoCheck ("PA_ReportColumnSet_ID", PA_ReportColumnSet_ID);
}
/** Get Report Column Set.
@return Collection of Columns for Report */
public int GetPA_ReportColumnSet_ID() 
{
Object ii = Get_Value("PA_ReportColumnSet_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Column.
@param PA_ReportColumn_ID Column in Report */
public void SetPA_ReportColumn_ID (int PA_ReportColumn_ID)
{
if (PA_ReportColumn_ID < 1) throw new ArgumentException ("PA_ReportColumn_ID is mandatory.");
Set_ValueNoCheck ("PA_ReportColumn_ID", PA_ReportColumn_ID);
}
/** Get Report Column.
@return Column in Report */
public int GetPA_ReportColumn_ID() 
{
Object ii = Get_Value("PA_ReportColumn_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** PostingType AD_Reference_ID=125 */
public static int POSTINGTYPE_AD_Reference_ID=125;
/** Actual = A */
public static String POSTINGTYPE_Actual = "A";
/** Budget = B */
public static String POSTINGTYPE_Budget = "B";
/** Commitment = E */
public static String POSTINGTYPE_Commitment = "E";
/** Reservation = R */
public static String POSTINGTYPE_Reservation = "R";
/** Statistical = S */
public static String POSTINGTYPE_Statistical = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test)
{
return test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (PostingType == null) throw new ArgumentException ("PostingType is mandatory");
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S");
if (PostingType.Length > 1)
{
log.Warning("Length > 1 - truncated");
PostingType = PostingType.Substring(0,1);
}
Set_Value ("PostingType", PostingType);
}
/** Get PostingType.
@return The type of posted amount for the transaction */
public String GetPostingType() 
{
return (String)Get_Value("PostingType");
}
/** Set Relative Period.
@param RelativePeriod Period offset (0 is current) */
public void SetRelativePeriod (Decimal? RelativePeriod)
{
Set_Value ("RelativePeriod", (Decimal?)RelativePeriod);
}
/** Get Relative Period.
@return Period offset (0 is current) */
public Decimal GetRelativePeriod() 
{
Object bd =Get_Value("RelativePeriod");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
