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
/** Generated Model for C_TaxCategory
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_TaxCategory : PO
{
public X_C_TaxCategory (Context ctx, int C_TaxCategory_ID, Trx trxName) : base (ctx, C_TaxCategory_ID, trxName)
{
/** if (C_TaxCategory_ID == 0)
{
SetC_TaxCategory_ID (0);
SetIsDefault (false);
SetName (null);
}
 */
}
public X_C_TaxCategory (Ctx ctx, int C_TaxCategory_ID, Trx trxName) : base (ctx, C_TaxCategory_ID, trxName)
{
/** if (C_TaxCategory_ID == 0)
{
SetC_TaxCategory_ID (0);
SetIsDefault (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxCategory (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxCategory (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_TaxCategory (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_TaxCategory()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514375384L;
/** Last Updated Timestamp 7/29/2010 1:07:38 PM */
public static long updatedMS = 1280389058595L;
/** AD_Table_ID=252 */
public static int Table_ID;
 // =252;

/** TableName=C_TaxCategory */
public static String Table_Name="C_TaxCategory";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_C_TaxCategory[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Tax Category.
@param C_TaxCategory_ID Tax Category */
public void SetC_TaxCategory_ID (int C_TaxCategory_ID)
{
if (C_TaxCategory_ID < 1) throw new ArgumentException ("C_TaxCategory_ID is mandatory.");
Set_ValueNoCheck ("C_TaxCategory_ID", C_TaxCategory_ID);
}
/** Get Tax Category.
@return Tax Category */
public int GetC_TaxCategory_ID() 
{
Object ii = Get_Value("C_TaxCategory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Commodity Code.
@param CommodityCode Commodity code used for tax calculation */
public void SetCommodityCode (String CommodityCode)
{
if (CommodityCode != null && CommodityCode.Length > 20)
{
log.Warning("Length > 20 - truncated");
CommodityCode = CommodityCode.Substring(0,20);
}
Set_Value ("CommodityCode", CommodityCode);
}
/** Get Commodity Code.
@return Commodity code used for tax calculation */
public String GetCommodityCode() 
{
return (String)Get_Value("CommodityCode");
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
