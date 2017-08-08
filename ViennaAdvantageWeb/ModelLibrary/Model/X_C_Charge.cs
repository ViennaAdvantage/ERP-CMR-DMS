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
/** Generated Model for C_Charge
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_Charge : PO
{
public X_C_Charge (Context ctx, int C_Charge_ID, Trx trxName) : base (ctx, C_Charge_ID, trxName)
{
/** if (C_Charge_ID == 0)
{
SetC_Charge_ID (0);
SetC_TaxCategory_ID (0);
SetChargeAmt (0.0);
SetIsSameCurrency (false);
SetIsSameTax (false);
SetIsTaxIncluded (false);	// N
SetName (null);
}
 */
}
public X_C_Charge (Ctx ctx, int C_Charge_ID, Trx trxName) : base (ctx, C_Charge_ID, trxName)
{
/** if (C_Charge_ID == 0)
{
SetC_Charge_ID (0);
SetC_TaxCategory_ID (0);
SetChargeAmt (0.0);
SetIsSameCurrency (false);
SetIsSameTax (false);
SetIsTaxIncluded (false);	// N
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Charge (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Charge (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_Charge (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_Charge()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514371184L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054395L;
/** AD_Table_ID=313 */
public static int Table_ID;
 // =313;

/** TableName=C_Charge */
public static String Table_Name="C_Charge";

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
StringBuilder sb = new StringBuilder ("X_C_Charge[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Charge.
@param C_Charge_ID Additional document charges */
public void SetC_Charge_ID (int C_Charge_ID)
{
if (C_Charge_ID < 1) throw new ArgumentException ("C_Charge_ID is mandatory.");
Set_ValueNoCheck ("C_Charge_ID", C_Charge_ID);
}
/** Get Charge.
@return Additional document charges */
public int GetC_Charge_ID() 
{
Object ii = Get_Value("C_Charge_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tax Category.
@param C_TaxCategory_ID Tax Category */
public void SetC_TaxCategory_ID (int C_TaxCategory_ID)
{
if (C_TaxCategory_ID < 1) throw new ArgumentException ("C_TaxCategory_ID is mandatory.");
Set_Value ("C_TaxCategory_ID", C_TaxCategory_ID);
}
/** Get Tax Category.
@return Tax Category */
public int GetC_TaxCategory_ID() 
{
Object ii = Get_Value("C_TaxCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge amount.
@param ChargeAmt Charge Amount */
public void SetChargeAmt (Decimal? ChargeAmt)
{
if (ChargeAmt == null) throw new ArgumentException ("ChargeAmt is mandatory.");
Set_Value ("ChargeAmt", (Decimal?)ChargeAmt);
}
/** Get Charge amount.
@return Charge Amount */
public Decimal GetChargeAmt() 
{
Object bd =Get_Value("ChargeAmt");
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
/** Set Same Currency.
@param IsSameCurrency Same Currency */
public void SetIsSameCurrency (Boolean IsSameCurrency)
{
Set_Value ("IsSameCurrency", IsSameCurrency);
}
/** Get Same Currency.
@return Same Currency */
public Boolean IsSameCurrency() 
{
Object oo = Get_Value("IsSameCurrency");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Same Tax.
@param IsSameTax Use the same tax as the main transaction */
public void SetIsSameTax (Boolean IsSameTax)
{
Set_Value ("IsSameTax", IsSameTax);
}
/** Get Same Tax.
@return Use the same tax as the main transaction */
public Boolean IsSameTax() 
{
Object oo = Get_Value("IsSameTax");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Price includes Tax.
@param IsTaxIncluded Tax is included in the price */
public void SetIsTaxIncluded (Boolean IsTaxIncluded)
{
Set_Value ("IsTaxIncluded", IsTaxIncluded);
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
}

}
