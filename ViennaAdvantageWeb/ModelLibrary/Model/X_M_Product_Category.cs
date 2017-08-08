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
/** Generated Model for M_Product_Category
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Product_Category : PO
{
public X_M_Product_Category (Context ctx, int M_Product_Category_ID, Trx trxName) : base (ctx, M_Product_Category_ID, trxName)
{
/** if (M_Product_Category_ID == 0)
{
SetIsDefault (false);
SetIsSelfService (true);	// Y
SetMMPolicy (null);	// F
SetM_Product_Category_ID (0);
SetName (null);
SetPlannedMargin (0.0);
SetValue (null);
}
 */
}
public X_M_Product_Category (Ctx ctx, int M_Product_Category_ID, Trx trxName) : base (ctx, M_Product_Category_ID, trxName)
{
/** if (M_Product_Category_ID == 0)
{
SetIsDefault (false);
SetIsSelfService (true);	// Y
SetMMPolicy (null);	// F
SetM_Product_Category_ID (0);
SetName (null);
SetPlannedMargin (0.0);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Category (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Category (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Category (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Product_Category()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380791L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064002L;
/** AD_Table_ID=209 */
public static int Table_ID;
 // =209;

/** TableName=M_Product_Category */
public static String Table_Name="M_Product_Category";

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
StringBuilder sb = new StringBuilder ("X_M_Product_Category[").Append(Get_ID()).Append("]");
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
/** Set Asset Group.
@param A_Asset_Group_ID Group of Assets */
public void SetA_Asset_Group_ID (int A_Asset_Group_ID)
{
if (A_Asset_Group_ID <= 0) Set_Value ("A_Asset_Group_ID", null);
else
Set_Value ("A_Asset_Group_ID", A_Asset_Group_ID);
}
/** Get Asset Group.
@return Group of Assets */
public int GetA_Asset_Group_ID() 
{
Object ii = Get_Value("A_Asset_Group_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Self-Service.
@param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
public void SetIsSelfService (Boolean IsSelfService)
{
Set_Value ("IsSelfService", IsSelfService);
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

/** MMPolicy AD_Reference_ID=335 */
public static int MMPOLICY_AD_Reference_ID=335;
/** FiFo = F */
public static String MMPOLICY_FiFo = "F";
/** LiFo = L */
public static String MMPOLICY_LiFo = "L";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMMPolicyValid (String test)
{
return test.Equals("F") || test.Equals("L");
}
/** Set Material Policy.
@param MMPolicy Material Movement Policy */
public void SetMMPolicy (String MMPolicy)
{
if (MMPolicy == null) throw new ArgumentException ("MMPolicy is mandatory");
if (!IsMMPolicyValid(MMPolicy))
throw new ArgumentException ("MMPolicy Invalid value - " + MMPolicy + " - Reference_ID=335 - F - L");
if (MMPolicy.Length > 1)
{
log.Warning("Length > 1 - truncated");
MMPolicy = MMPolicy.Substring(0,1);
}
Set_Value ("MMPolicy", MMPolicy);
}
/** Get Material Policy.
@return Material Movement Policy */
public String GetMMPolicy() 
{
return (String)Get_Value("MMPolicy");
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID < 1) throw new ArgumentException ("M_Product_Category_ID is mandatory.");
Set_ValueNoCheck ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
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
/** Set Planned Margin %.
@param PlannedMargin Project's planned margin as a percentage */
public void SetPlannedMargin (Decimal? PlannedMargin)
{
if (PlannedMargin == null) throw new ArgumentException ("PlannedMargin is mandatory.");
Set_Value ("PlannedMargin", (Decimal?)PlannedMargin);
}
/** Get Planned Margin %.
@return Project's planned margin as a percentage */
public Decimal GetPlannedMargin() 
{
Object bd =Get_Value("PlannedMargin");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
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
