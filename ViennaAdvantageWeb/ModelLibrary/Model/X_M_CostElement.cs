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
/** Generated Model for M_CostElement
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_CostElement : PO
{
public X_M_CostElement (Context ctx, int M_CostElement_ID, Trx trxName) : base (ctx, M_CostElement_ID, trxName)
{
/** if (M_CostElement_ID == 0)
{
SetCostElementType (null);
SetIsCalculated (false);
SetM_CostElement_ID (0);
SetName (null);
}
 */
}
public X_M_CostElement (Ctx ctx, int M_CostElement_ID, Trx trxName) : base (ctx, M_CostElement_ID, trxName)
{
/** if (M_CostElement_ID == 0)
{
SetCostElementType (null);
SetIsCalculated (false);
SetM_CostElement_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostElement (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostElement (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostElement (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_CostElement()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514378958L;
/** Last Updated Timestamp 7/29/2010 1:07:42 PM */
public static long updatedMS = 1280389062169L;
/** AD_Table_ID=770 */
public static int Table_ID;
 // =770;

/** TableName=M_CostElement */
public static String Table_Name="M_CostElement";

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
StringBuilder sb = new StringBuilder ("X_M_CostElement[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** CostElementType AD_Reference_ID=338 */
public static int COSTELEMENTTYPE_AD_Reference_ID=338;
/** Burden (M.Overhead) = B */
public static String COSTELEMENTTYPE_BurdenMOverhead = "B";
/** Material = M */
public static String COSTELEMENTTYPE_Material = "M";
/** Overhead = O */
public static String COSTELEMENTTYPE_Overhead = "O";
/** Resource = R */
public static String COSTELEMENTTYPE_Resource = "R";
/** Outside Processing = X */
public static String COSTELEMENTTYPE_OutsideProcessing = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCostElementTypeValid (String test)
{
return test.Equals("B") || test.Equals("M") || test.Equals("O") || test.Equals("R") || test.Equals("X");
}
/** Set Cost Element Type.
@param CostElementType Type of Cost Element */
public void SetCostElementType (String CostElementType)
{
if (CostElementType == null) throw new ArgumentException ("CostElementType is mandatory");
if (!IsCostElementTypeValid(CostElementType))
throw new ArgumentException ("CostElementType Invalid value - " + CostElementType + " - Reference_ID=338 - B - M - O - R - X");
if (CostElementType.Length > 1)
{
log.Warning("Length > 1 - truncated");
CostElementType = CostElementType.Substring(0,1);
}
Set_Value ("CostElementType", CostElementType);
}
/** Get Cost Element Type.
@return Type of Cost Element */
public String GetCostElementType() 
{
return (String)Get_Value("CostElementType");
}

/** CostingMethod AD_Reference_ID=122 */
public static int COSTINGMETHOD_AD_Reference_ID=122;
/** Average PO = A */
public static String COSTINGMETHOD_AveragePO = "A";
/** Fifo = F */
public static String COSTINGMETHOD_Fifo = "F";
/** Average Invoice = I */
public static String COSTINGMETHOD_AverageInvoice = "I";
/** Lifo = L */
public static String COSTINGMETHOD_Lifo = "L";
/** Standard Costing = S */
public static String COSTINGMETHOD_StandardCosting = "S";
/** User Defined = U */
public static String COSTINGMETHOD_UserDefined = "U";
/** Last Invoice = i */
public static String COSTINGMETHOD_LastInvoice = "i";
/** Last PO Price = p */
public static String COSTINGMETHOD_LastPOPrice = "p";
/** _ = x */
public static String COSTINGMETHOD__ = "x";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCostingMethodValid (String test)
{
return test == null || test.Equals("A") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i") || test.Equals("p") || test.Equals("x");
}
/** Set Costing Method.
@param CostingMethod Indicates how Costs will be calculated */
public void SetCostingMethod (String CostingMethod)
{
if (!IsCostingMethodValid(CostingMethod))
throw new ArgumentException ("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - F - I - L - S - U - i - p - x");
if (CostingMethod != null && CostingMethod.Length > 1)
{
log.Warning("Length > 1 - truncated");
CostingMethod = CostingMethod.Substring(0,1);
}
Set_Value ("CostingMethod", CostingMethod);
}
/** Get Costing Method.
@return Indicates how Costs will be calculated */
public String GetCostingMethod() 
{
return (String)Get_Value("CostingMethod");
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
/** Set Calculated.
@param IsCalculated The value is calculated by the system */
public void SetIsCalculated (Boolean IsCalculated)
{
Set_Value ("IsCalculated", IsCalculated);
}
/** Get Calculated.
@return The value is calculated by the system */
public Boolean IsCalculated() 
{
Object oo = Get_Value("IsCalculated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
public void SetM_CostElement_ID (int M_CostElement_ID)
{
if (M_CostElement_ID < 1) throw new ArgumentException ("M_CostElement_ID is mandatory.");
Set_ValueNoCheck ("M_CostElement_ID", M_CostElement_ID);
}
/** Get Cost Element.
@return Product Cost Element */
public int GetM_CostElement_ID() 
{
Object ii = Get_Value("M_CostElement_ID");
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
}

}
