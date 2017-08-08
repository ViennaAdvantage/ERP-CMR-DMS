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
/** Generated Model for C_BP_Group
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BP_Group : PO
{
public X_C_BP_Group (Context ctx, int C_BP_Group_ID, Trx trxName) : base (ctx, C_BP_Group_ID, trxName)
{
/** if (C_BP_Group_ID == 0)
{
SetC_BP_Group_ID (0);
SetIsConfidentialInfo (false);	// N
SetIsDefault (false);
SetName (null);
SetValue (null);
}
 */
}
public X_C_BP_Group (Ctx ctx, int C_BP_Group_ID, Trx trxName) : base (ctx, C_BP_Group_ID, trxName)
{
/** if (C_BP_Group_ID == 0)
{
SetC_BP_Group_ID (0);
SetIsConfidentialInfo (false);	// N
SetIsDefault (false);
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Group (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Group (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BP_Group (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BP_Group()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370103L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053314L;
/** AD_Table_ID=394 */
public static int Table_ID;
 // =394;

/** TableName=C_BP_Group */
public static String Table_Name="C_BP_Group";

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
StringBuilder sb = new StringBuilder ("X_C_BP_Group[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Print Color.
@param AD_PrintColor_ID Color used for printing and display */
public void SetAD_PrintColor_ID (int AD_PrintColor_ID)
{
if (AD_PrintColor_ID <= 0) Set_Value ("AD_PrintColor_ID", null);
else
Set_Value ("AD_PrintColor_ID", AD_PrintColor_ID);
}
/** Get Print Color.
@return Color used for printing and display */
public int GetAD_PrintColor_ID() 
{
Object ii = Get_Value("AD_PrintColor_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Business Partner Group.
@param C_BP_Group_ID Business Partner Group */
public void SetC_BP_Group_ID (int C_BP_Group_ID)
{
if (C_BP_Group_ID < 1) throw new ArgumentException ("C_BP_Group_ID is mandatory.");
Set_ValueNoCheck ("C_BP_Group_ID", C_BP_Group_ID);
}
/** Get Business Partner Group.
@return Business Partner Group */
public int GetC_BP_Group_ID() 
{
Object ii = Get_Value("C_BP_Group_ID");
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
/** Set Credit Watch %.
@param CreditWatchPercent Credit Watch - Percent of Credit Limit when OK switches to Watch */
public void SetCreditWatchPercent (Decimal? CreditWatchPercent)
{
Set_Value ("CreditWatchPercent", (Decimal?)CreditWatchPercent);
}
/** Get Credit Watch %.
@return Credit Watch - Percent of Credit Limit when OK switches to Watch */
public Decimal GetCreditWatchPercent() 
{
Object bd =Get_Value("CreditWatchPercent");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
/** Set Confidential Info.
@param IsConfidentialInfo Can enter confidential information */
public void SetIsConfidentialInfo (Boolean IsConfidentialInfo)
{
Set_Value ("IsConfidentialInfo", IsConfidentialInfo);
}
/** Get Confidential Info.
@return Can enter confidential information */
public Boolean IsConfidentialInfo() 
{
Object oo = Get_Value("IsConfidentialInfo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault)
{
Set_Value ("IsDefault", IsDefault);
}
/** Get Default.
@return Default value */
public Boolean IsDefault() 
{
Object oo = Get_Value("IsDefault");
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
/** Set Price Match Tolerance.
@param PriceMatchTolerance PO-Invoice Match Price Tolerance in percent of the purchase price */
public void SetPriceMatchTolerance (Decimal? PriceMatchTolerance)
{
Set_Value ("PriceMatchTolerance", (Decimal?)PriceMatchTolerance);
}
/** Get Price Match Tolerance.
@return PO-Invoice Match Price Tolerance in percent of the purchase price */
public Decimal GetPriceMatchTolerance() 
{
Object bd =Get_Value("PriceMatchTolerance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** PriorityBase AD_Reference_ID=350 */
public static int PRIORITYBASE_AD_Reference_ID=350;
/** Higher = H */
public static String PRIORITYBASE_Higher = "H";
/** Lower = L */
public static String PRIORITYBASE_Lower = "L";
/** Same = S */
public static String PRIORITYBASE_Same = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPriorityBaseValid (String test)
{
return test == null || test.Equals("H") || test.Equals("L") || test.Equals("S");
}
/** Set Priority Base.
@param PriorityBase Base of Priority */
public void SetPriorityBase (String PriorityBase)
{
if (!IsPriorityBaseValid(PriorityBase))
throw new ArgumentException ("PriorityBase Invalid value - " + PriorityBase + " - Reference_ID=350 - H - L - S");
if (PriorityBase != null && PriorityBase.Length > 1)
{
log.Warning("Length > 1 - truncated");
PriorityBase = PriorityBase.Substring(0,1);
}
Set_Value ("PriorityBase", PriorityBase);
}
/** Get Priority Base.
@return Base of Priority */
public String GetPriorityBase() 
{
return (String)Get_Value("PriorityBase");
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
}

}
