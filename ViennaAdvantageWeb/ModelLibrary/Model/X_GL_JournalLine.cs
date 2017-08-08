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
/** Generated Model for GL_JournalLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_GL_JournalLine : PO
{
public X_GL_JournalLine (Context ctx, int GL_JournalLine_ID, Trx trxName) : base (ctx, GL_JournalLine_ID, trxName)
{
/** if (GL_JournalLine_ID == 0)
{
SetAmtAcctCr (0.0);
SetAmtAcctDr (0.0);
SetAmtSourceCr (0.0);
SetAmtSourceDr (0.0);
SetC_ConversionType_ID (0);
SetC_Currency_ID (0);	// @C_Currency_ID@
SetC_ValidCombination_ID (0);
SetCurrencyRate (0.0);	// @CurrencyRate@;
1
SetDateAcct (DateTime.Now);	// @DateAcct@
SetGL_JournalLine_ID (0);
SetGL_Journal_ID (0);
SetIsGenerated (false);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM GL_JournalLine WHERE GL_Journal_ID=@GL_Journal_ID@
SetProcessed (false);	// N
}
 */
}
public X_GL_JournalLine (Ctx ctx, int GL_JournalLine_ID, Trx trxName) : base (ctx, GL_JournalLine_ID, trxName)
{
/** if (GL_JournalLine_ID == 0)
{
SetAmtAcctCr (0.0);
SetAmtAcctDr (0.0);
SetAmtSourceCr (0.0);
SetAmtSourceDr (0.0);
SetC_ConversionType_ID (0);
SetC_Currency_ID (0);	// @C_Currency_ID@
SetC_ValidCombination_ID (0);
SetCurrencyRate (0.0);	// @CurrencyRate@;
1
SetDateAcct (DateTime.Now);	// @DateAcct@
SetGL_JournalLine_ID (0);
SetGL_Journal_ID (0);
SetIsGenerated (false);
SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM GL_JournalLine WHERE GL_Journal_ID=@GL_Journal_ID@
SetProcessed (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_JournalLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_JournalLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_GL_JournalLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_GL_JournalLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376654L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059865L;
/** AD_Table_ID=226 */
public static int Table_ID;
 // =226;

/** TableName=GL_JournalLine */
public static String Table_Name="GL_JournalLine";

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
StringBuilder sb = new StringBuilder ("X_GL_JournalLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr)
{
if (AmtAcctCr == null) throw new ArgumentException ("AmtAcctCr is mandatory.");
Set_ValueNoCheck ("AmtAcctCr", (Decimal?)AmtAcctCr);
}
/** Get Accounted Credit.
@return Accounted Credit Amount */
public Decimal GetAmtAcctCr() 
{
Object bd =Get_Value("AmtAcctCr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounted Debit.
@param AmtAcctDr Accounted Debit Amount */
public void SetAmtAcctDr (Decimal? AmtAcctDr)
{
if (AmtAcctDr == null) throw new ArgumentException ("AmtAcctDr is mandatory.");
Set_ValueNoCheck ("AmtAcctDr", (Decimal?)AmtAcctDr);
}
/** Get Accounted Debit.
@return Accounted Debit Amount */
public Decimal GetAmtAcctDr() 
{
Object bd =Get_Value("AmtAcctDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Credit.
@param AmtSourceCr Source Credit Amount */
public void SetAmtSourceCr (Decimal? AmtSourceCr)
{
if (AmtSourceCr == null) throw new ArgumentException ("AmtSourceCr is mandatory.");
Set_Value ("AmtSourceCr", (Decimal?)AmtSourceCr);
}
/** Get Source Credit.
@return Source Credit Amount */
public Decimal GetAmtSourceCr() 
{
Object bd =Get_Value("AmtSourceCr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Source Debit.
@param AmtSourceDr Source Debit Amount */
public void SetAmtSourceDr (Decimal? AmtSourceDr)
{
if (AmtSourceDr == null) throw new ArgumentException ("AmtSourceDr is mandatory.");
Set_Value ("AmtSourceDr", (Decimal?)AmtSourceDr);
}
/** Get Source Debit.
@return Source Debit Amount */
public Decimal GetAmtSourceDr() 
{
Object bd =Get_Value("AmtSourceDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Currency Type.
@param C_ConversionType_ID Currency Conversion Rate Type */
public void SetC_ConversionType_ID (int C_ConversionType_ID)
{
if (C_ConversionType_ID < 1) throw new ArgumentException ("C_ConversionType_ID is mandatory.");
Set_Value ("C_ConversionType_ID", C_ConversionType_ID);
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
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
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
/** Set UOM.
@param C_UOM_ID Unit of Measure */
public void SetC_UOM_ID (int C_UOM_ID)
{
if (C_UOM_ID <= 0) Set_Value ("C_UOM_ID", null);
else
Set_Value ("C_UOM_ID", C_UOM_ID);
}
/** Get UOM.
@return Unit of Measure */
public int GetC_UOM_ID() 
{
Object ii = Get_Value("C_UOM_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Combination.
@param C_ValidCombination_ID Valid Account Combination */
public void SetC_ValidCombination_ID (int C_ValidCombination_ID)
{
if (C_ValidCombination_ID < 1) throw new ArgumentException ("C_ValidCombination_ID is mandatory.");
Set_Value ("C_ValidCombination_ID", C_ValidCombination_ID);
}
/** Get Combination.
@return Valid Account Combination */
public int GetC_ValidCombination_ID() 
{
Object ii = Get_Value("C_ValidCombination_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Rate.
@param CurrencyRate Currency Conversion Rate */
public void SetCurrencyRate (Decimal? CurrencyRate)
{
if (CurrencyRate == null) throw new ArgumentException ("CurrencyRate is mandatory.");
Set_ValueNoCheck ("CurrencyRate", (Decimal?)CurrencyRate);
}
/** Get Rate.
@return Currency Conversion Rate */
public Decimal GetCurrencyRate() 
{
Object bd =Get_Value("CurrencyRate");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Account Date.
@param DateAcct General Ledger Date */
public void SetDateAcct (DateTime? DateAcct)
{
if (DateAcct == null) throw new ArgumentException ("DateAcct is mandatory.");
Set_Value ("DateAcct", (DateTime?)DateAcct);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetDateAcct() 
{
return (DateTime?)Get_Value("DateAcct");
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
/** Set Journal Line.
@param GL_JournalLine_ID General Ledger Journal Line */
public void SetGL_JournalLine_ID (int GL_JournalLine_ID)
{
if (GL_JournalLine_ID < 1) throw new ArgumentException ("GL_JournalLine_ID is mandatory.");
Set_ValueNoCheck ("GL_JournalLine_ID", GL_JournalLine_ID);
}
/** Get Journal Line.
@return General Ledger Journal Line */
public int GetGL_JournalLine_ID() 
{
Object ii = Get_Value("GL_JournalLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Journal.
@param GL_Journal_ID General Ledger Journal */
public void SetGL_Journal_ID (int GL_Journal_ID)
{
if (GL_Journal_ID < 1) throw new ArgumentException ("GL_Journal_ID is mandatory.");
Set_ValueNoCheck ("GL_Journal_ID", GL_Journal_ID);
}
/** Get Journal.
@return General Ledger Journal */
public int GetGL_Journal_ID() 
{
Object ii = Get_Value("GL_Journal_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Generated.
@param IsGenerated This Line is generated */
public void SetIsGenerated (Boolean IsGenerated)
{
Set_ValueNoCheck ("IsGenerated", IsGenerated);
}
/** Get Generated.
@return This Line is generated */
public Boolean IsGenerated() 
{
Object oo = Get_Value("IsGenerated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Line No.
@param Line Unique line for this document */
public void SetLine (int Line)
{
Set_Value ("Line", Line);
}
/** Get Line No.
@return Unique line for this document */
public int GetLine() 
{
Object ii = Get_Value("Line");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetLine().ToString());
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
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
