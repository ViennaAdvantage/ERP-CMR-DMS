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
/** Generated Model for M_Locator
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Locator : PO
{
public X_M_Locator (Context ctx, int M_Locator_ID, Trx trxName) : base (ctx, M_Locator_ID, trxName)
{
/** if (M_Locator_ID == 0)
{
SetIsDefault (false);
SetM_Locator_ID (0);
SetM_Warehouse_ID (0);
SetPriorityNo (0);	// 50
SetValue (null);
}
 */
}
public X_M_Locator (Ctx ctx, int M_Locator_ID, Trx trxName) : base (ctx, M_Locator_ID, trxName)
{
/** if (M_Locator_ID == 0)
{
SetIsDefault (false);
SetM_Locator_ID (0);
SetM_Warehouse_ID (0);
SetPriorityNo (0);	// 50
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Locator (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Locator (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Locator (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Locator()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514379851L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063062L;
/** AD_Table_ID=207 */
public static int Table_ID;
 // =207;

/** TableName=M_Locator */
public static String Table_Name="M_Locator";

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
StringBuilder sb = new StringBuilder ("X_M_Locator[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID < 1) throw new ArgumentException ("M_Locator_ID is mandatory.");
Set_ValueNoCheck ("M_Locator_ID", M_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() 
{
Object ii = Get_Value("M_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");
Set_ValueNoCheck ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_Warehouse_ID().ToString());
}
/** Set Relative Priority.
@param PriorityNo Where inventory should be picked from first */
public void SetPriorityNo (int PriorityNo)
{
Set_Value ("PriorityNo", PriorityNo);
}
/** Get Relative Priority.
@return Where inventory should be picked from first */
public int GetPriorityNo() 
{
Object ii = Get_Value("PriorityNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Aisle (X).
@param X X dimension, e.g., Aisle */
public void SetX (String X)
{
if (X != null && X.Length > 60)
{
log.Warning("Length > 60 - truncated");
X = X.Substring(0,60);
}
Set_Value ("X", X);
}
/** Get Aisle (X).
@return X dimension, e.g., Aisle */
public String GetX() 
{
return (String)Get_Value("X");
}
/** Set Bin (Y).
@param Y Y dimension, e.g., Bin */
public void SetY (String Y)
{
if (Y != null && Y.Length > 60)
{
log.Warning("Length > 60 - truncated");
Y = Y.Substring(0,60);
}
Set_Value ("Y", Y);
}
/** Get Bin (Y).
@return Y dimension, e.g., Bin */
public String GetY() 
{
return (String)Get_Value("Y");
}
/** Set Level (Z).
@param Z Z dimension, e.g., Level */
public void SetZ (String Z)
{
if (Z != null && Z.Length > 60)
{
log.Warning("Length > 60 - truncated");
Z = Z.Substring(0,60);
}
Set_Value ("Z", Z);
}
/** Get Level (Z).
@return Z dimension, e.g., Level */
public String GetZ() 
{
return (String)Get_Value("Z");
}
}

}
