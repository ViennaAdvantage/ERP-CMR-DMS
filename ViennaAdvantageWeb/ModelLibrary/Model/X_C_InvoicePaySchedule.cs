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
/** Generated Model for C_InvoicePaySchedule
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_InvoicePaySchedule : PO
{
public X_C_InvoicePaySchedule (Context ctx, int C_InvoicePaySchedule_ID, Trx trxName) : base (ctx, C_InvoicePaySchedule_ID, trxName)
{
/** if (C_InvoicePaySchedule_ID == 0)
{
SetC_InvoicePaySchedule_ID (0);
SetC_Invoice_ID (0);
SetDiscountAmt (0.0);
SetDiscountDate (DateTime.Now);
SetDueAmt (0.0);
SetDueDate (DateTime.Now);
SetIsValid (false);
SetProcessed (false);	// N
}
 */
}
public X_C_InvoicePaySchedule (Ctx ctx, int C_InvoicePaySchedule_ID, Trx trxName) : base (ctx, C_InvoicePaySchedule_ID, trxName)
{
/** if (C_InvoicePaySchedule_ID == 0)
{
SetC_InvoicePaySchedule_ID (0);
SetC_Invoice_ID (0);
SetDiscountAmt (0.0);
SetDiscountDate (DateTime.Now);
SetDueAmt (0.0);
SetDueDate (DateTime.Now);
SetIsValid (false);
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoicePaySchedule (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoicePaySchedule (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_InvoicePaySchedule (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_InvoicePaySchedule()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514372548L;
/** Last Updated Timestamp 7/29/2010 1:07:35 PM */
public static long updatedMS = 1280389055759L;
/** AD_Table_ID=551 */
public static int Table_ID;
 // =551;

/** TableName=C_InvoicePaySchedule */
public static String Table_Name="C_InvoicePaySchedule";

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
StringBuilder sb = new StringBuilder ("X_C_InvoicePaySchedule[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Invoice Payment Schedule.
@param C_InvoicePaySchedule_ID Invoice Payment Schedule */
public void SetC_InvoicePaySchedule_ID (int C_InvoicePaySchedule_ID)
{
if (C_InvoicePaySchedule_ID < 1) throw new ArgumentException ("C_InvoicePaySchedule_ID is mandatory.");
Set_ValueNoCheck ("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
}
/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
public int GetC_InvoicePaySchedule_ID() 
{
Object ii = Get_Value("C_InvoicePaySchedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID < 1) throw new ArgumentException ("C_Invoice_ID is mandatory.");
Set_ValueNoCheck ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Schedule.
@param C_PaySchedule_ID Payment Schedule Template */
public void SetC_PaySchedule_ID (int C_PaySchedule_ID)
{
if (C_PaySchedule_ID <= 0) Set_ValueNoCheck ("C_PaySchedule_ID", null);
else
Set_ValueNoCheck ("C_PaySchedule_ID", C_PaySchedule_ID);
}
/** Get Payment Schedule.
@return Payment Schedule Template */
public int GetC_PaySchedule_ID() 
{
Object ii = Get_Value("C_PaySchedule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Discount Amount.
@param DiscountAmt Calculated amount of discount */
public void SetDiscountAmt (Decimal? DiscountAmt)
{
if (DiscountAmt == null) throw new ArgumentException ("DiscountAmt is mandatory.");
Set_Value ("DiscountAmt", (Decimal?)DiscountAmt);
}
/** Get Discount Amount.
@return Calculated amount of discount */
public Decimal GetDiscountAmt() 
{
Object bd =Get_Value("DiscountAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Discount Date.
@param DiscountDate Last Date for payments with discount */
public void SetDiscountDate (DateTime? DiscountDate)
{
if (DiscountDate == null) throw new ArgumentException ("DiscountDate is mandatory.");
Set_Value ("DiscountDate", (DateTime?)DiscountDate);
}
/** Get Discount Date.
@return Last Date for payments with discount */
public DateTime? GetDiscountDate() 
{
return (DateTime?)Get_Value("DiscountDate");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetDiscountDate().ToString());
}
/** Set Amount due.
@param DueAmt Amount of the payment due */
public void SetDueAmt (Decimal? DueAmt)
{
if (DueAmt == null) throw new ArgumentException ("DueAmt is mandatory.");
Set_Value ("DueAmt", (Decimal?)DueAmt);
}
/** Get Amount due.
@return Amount of the payment due */
public Decimal GetDueAmt() 
{
Object bd =Get_Value("DueAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Due Date.
@param DueDate Date when the payment is due */
public void SetDueDate (DateTime? DueDate)
{
if (DueDate == null) throw new ArgumentException ("DueDate is mandatory.");
Set_Value ("DueDate", (DateTime?)DueDate);
}
/** Get Due Date.
@return Date when the payment is due */
public DateTime? GetDueDate() 
{
return (DateTime?)Get_Value("DueDate");
}
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
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
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
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
}

}
