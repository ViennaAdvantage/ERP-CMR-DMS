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
/** Generated Model for C_BankStatementLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BankStatementLine : PO
{
public X_C_BankStatementLine (Context ctx, int C_BankStatementLine_ID, Trx trxName) : base (ctx, C_BankStatementLine_ID, trxName)
{
/** if (C_BankStatementLine_ID == 0)
{
SetC_BankStatementLine_ID (0);
SetC_BankStatement_ID (0);
SetC_Currency_ID (0);	// @SQL=SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=@C_BankAccount_ID@
SetChargeAmt (0.0);
SetDateAcct (DateTime.Now);	// @StatementDate@
SetInterestAmt (0.0);
SetIsManual (true);	// Y
SetIsReversal (false);
SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 FROM C_BankStatementLine WHERE C_BankStatement_ID=@C_BankStatement_ID@
SetProcessed (false);	// N
SetStatementLineDate (DateTime.Now);	// @StatementLineDate@
SetStmtAmt (0.0);
SetTrxAmt (0.0);
SetValutaDate (DateTime.Now);	// @StatementDate@
}
 */
}
public X_C_BankStatementLine (Ctx ctx, int C_BankStatementLine_ID, Trx trxName) : base (ctx, C_BankStatementLine_ID, trxName)
{
/** if (C_BankStatementLine_ID == 0)
{
SetC_BankStatementLine_ID (0);
SetC_BankStatement_ID (0);
SetC_Currency_ID (0);	// @SQL=SELECT C_Currency_ID FROM C_BankAccount WHERE C_BankAccount_ID=@C_BankAccount_ID@
SetChargeAmt (0.0);
SetDateAcct (DateTime.Now);	// @StatementDate@
SetInterestAmt (0.0);
SetIsManual (true);	// Y
SetIsReversal (false);
SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 FROM C_BankStatementLine WHERE C_BankStatement_ID=@C_BankStatement_ID@
SetProcessed (false);	// N
SetStatementLineDate (DateTime.Now);	// @StatementLineDate@
SetStmtAmt (0.0);
SetTrxAmt (0.0);
SetValutaDate (DateTime.Now);	// @StatementDate@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementLine (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementLine (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementLine (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BankStatementLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370871L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054082L;
/** AD_Table_ID=393 */
public static int Table_ID;
 // =393;

/** TableName=C_BankStatementLine */
public static String Table_Name="C_BankStatementLine";

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
StringBuilder sb = new StringBuilder ("X_C_BankStatementLine[").Append(Get_ID()).Append("]");
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
/** Set Bank statement line.
@param C_BankStatementLine_ID Line on a statement from this Bank */
public void SetC_BankStatementLine_ID (int C_BankStatementLine_ID)
{
if (C_BankStatementLine_ID < 1) throw new ArgumentException ("C_BankStatementLine_ID is mandatory.");
Set_ValueNoCheck ("C_BankStatementLine_ID", C_BankStatementLine_ID);
}
/** Get Bank statement line.
@return Line on a statement from this Bank */
public int GetC_BankStatementLine_ID() 
{
Object ii = Get_Value("C_BankStatementLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Statement.
@param C_BankStatement_ID Bank Statement of account */
public void SetC_BankStatement_ID (int C_BankStatement_ID)
{
if (C_BankStatement_ID < 1) throw new ArgumentException ("C_BankStatement_ID is mandatory.");
Set_ValueNoCheck ("C_BankStatement_ID", C_BankStatement_ID);
}
/** Get Bank Statement.
@return Bank Statement of account */
public int GetC_BankStatement_ID() 
{
Object ii = Get_Value("C_BankStatement_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Charge.
@param C_Charge_ID Additional document charges */
public void SetC_Charge_ID (int C_Charge_ID)
{
if (C_Charge_ID <= 0) Set_Value ("C_Charge_ID", null);
else
Set_Value ("C_Charge_ID", C_Charge_ID);
}
/** Get Charge.
@return Additional document charges */
public int GetC_Charge_ID() 
{
Object ii = Get_Value("C_Charge_ID");
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
/** Set Invoice.
@param C_Invoice_ID Invoice Identifier */
public void SetC_Invoice_ID (int C_Invoice_ID)
{
if (C_Invoice_ID <= 0) Set_Value ("C_Invoice_ID", null);
else
Set_Value ("C_Invoice_ID", C_Invoice_ID);
}
/** Get Invoice.
@return Invoice Identifier */
public int GetC_Invoice_ID() 
{
Object ii = Get_Value("C_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment.
@param C_Payment_ID Payment identifier */
public void SetC_Payment_ID (int C_Payment_ID)
{
if (C_Payment_ID <= 0) Set_Value ("C_Payment_ID", null);
else
Set_Value ("C_Payment_ID", C_Payment_ID);
}
/** Get Payment.
@return Payment identifier */
public int GetC_Payment_ID() 
{
Object ii = Get_Value("C_Payment_ID");
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
/** Set Create Payment.
@param CreatePayment Create Payment */
public void SetCreatePayment (String CreatePayment)
{
if (CreatePayment != null && CreatePayment.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreatePayment = CreatePayment.Substring(0,1);
}
Set_Value ("CreatePayment", CreatePayment);
}
/** Get Create Payment.
@return Create Payment */
public String GetCreatePayment() 
{
return (String)Get_Value("CreatePayment");
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
/** Set EFT Amount.
@param EftAmt Electronic Funds Transfer Amount */
public void SetEftAmt (Decimal? EftAmt)
{
Set_Value ("EftAmt", (Decimal?)EftAmt);
}
/** Get EFT Amount.
@return Electronic Funds Transfer Amount */
public Decimal GetEftAmt() 
{
Object bd =Get_Value("EftAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set EFT Check No.
@param EftCheckNo Electronic Funds Transfer Check No */
public void SetEftCheckNo (String EftCheckNo)
{
if (EftCheckNo != null && EftCheckNo.Length > 20)
{
log.Warning("Length > 20 - truncated");
EftCheckNo = EftCheckNo.Substring(0,20);
}
Set_Value ("EftCheckNo", EftCheckNo);
}
/** Get EFT Check No.
@return Electronic Funds Transfer Check No */
public String GetEftCheckNo() 
{
return (String)Get_Value("EftCheckNo");
}
/** Set EFT Currency.
@param EftCurrency Electronic Funds Transfer Currency */
public void SetEftCurrency (String EftCurrency)
{
if (EftCurrency != null && EftCurrency.Length > 20)
{
log.Warning("Length > 20 - truncated");
EftCurrency = EftCurrency.Substring(0,20);
}
Set_Value ("EftCurrency", EftCurrency);
}
/** Get EFT Currency.
@return Electronic Funds Transfer Currency */
public String GetEftCurrency() 
{
return (String)Get_Value("EftCurrency");
}
/** Set EFT Memo.
@param EftMemo Electronic Funds Transfer Memo */
public void SetEftMemo (String EftMemo)
{
if (EftMemo != null && EftMemo.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
EftMemo = EftMemo.Substring(0,2000);
}
Set_Value ("EftMemo", EftMemo);
}
/** Get EFT Memo.
@return Electronic Funds Transfer Memo */
public String GetEftMemo() 
{
return (String)Get_Value("EftMemo");
}
/** Set EFT Payee.
@param EftPayee Electronic Funds Transfer Payee information */
public void SetEftPayee (String EftPayee)
{
if (EftPayee != null && EftPayee.Length > 255)
{
log.Warning("Length > 255 - truncated");
EftPayee = EftPayee.Substring(0,255);
}
Set_Value ("EftPayee", EftPayee);
}
/** Get EFT Payee.
@return Electronic Funds Transfer Payee information */
public String GetEftPayee() 
{
return (String)Get_Value("EftPayee");
}
/** Set EFT Payee Account.
@param EftPayeeAccount Electronic Funds Transfer Payyee Account Information */
public void SetEftPayeeAccount (String EftPayeeAccount)
{
if (EftPayeeAccount != null && EftPayeeAccount.Length > 40)
{
log.Warning("Length > 40 - truncated");
EftPayeeAccount = EftPayeeAccount.Substring(0,40);
}
Set_Value ("EftPayeeAccount", EftPayeeAccount);
}
/** Get EFT Payee Account.
@return Electronic Funds Transfer Payyee Account Information */
public String GetEftPayeeAccount() 
{
return (String)Get_Value("EftPayeeAccount");
}
/** Set EFT Reference.
@param EftReference Electronic Funds Transfer Reference */
public void SetEftReference (String EftReference)
{
if (EftReference != null && EftReference.Length > 60)
{
log.Warning("Length > 60 - truncated");
EftReference = EftReference.Substring(0,60);
}
Set_Value ("EftReference", EftReference);
}
/** Get EFT Reference.
@return Electronic Funds Transfer Reference */
public String GetEftReference() 
{
return (String)Get_Value("EftReference");
}
/** Set EFT Statement Line Date.
@param EftStatementLineDate Electronic Funds Transfer Statement Line Date */
public void SetEftStatementLineDate (DateTime? EftStatementLineDate)
{
Set_Value ("EftStatementLineDate", (DateTime?)EftStatementLineDate);
}
/** Get EFT Statement Line Date.
@return Electronic Funds Transfer Statement Line Date */
public DateTime? GetEftStatementLineDate() 
{
return (DateTime?)Get_Value("EftStatementLineDate");
}
/** Set EFT Trx ID.
@param EftTrxID Electronic Funds Transfer Transaction ID */
public void SetEftTrxID (String EftTrxID)
{
if (EftTrxID != null && EftTrxID.Length > 40)
{
log.Warning("Length > 40 - truncated");
EftTrxID = EftTrxID.Substring(0,40);
}
Set_Value ("EftTrxID", EftTrxID);
}
/** Get EFT Trx ID.
@return Electronic Funds Transfer Transaction ID */
public String GetEftTrxID() 
{
return (String)Get_Value("EftTrxID");
}
/** Set EFT Trx Type.
@param EftTrxType Electronic Funds Transfer Transaction Type */
public void SetEftTrxType (String EftTrxType)
{
if (EftTrxType != null && EftTrxType.Length > 20)
{
log.Warning("Length > 20 - truncated");
EftTrxType = EftTrxType.Substring(0,20);
}
Set_Value ("EftTrxType", EftTrxType);
}
/** Get EFT Trx Type.
@return Electronic Funds Transfer Transaction Type */
public String GetEftTrxType() 
{
return (String)Get_Value("EftTrxType");
}
/** Set EFT Effective Date.
@param EftValutaDate Electronic Funds Transfer Valuta (effective) Date */
public void SetEftValutaDate (DateTime? EftValutaDate)
{
Set_Value ("EftValutaDate", (DateTime?)EftValutaDate);
}
/** Get EFT Effective Date.
@return Electronic Funds Transfer Valuta (effective) Date */
public DateTime? GetEftValutaDate() 
{
return (DateTime?)Get_Value("EftValutaDate");
}
/** Set Interest Amount.
@param InterestAmt Interest Amount */
public void SetInterestAmt (Decimal? InterestAmt)
{
if (InterestAmt == null) throw new ArgumentException ("InterestAmt is mandatory.");
Set_Value ("InterestAmt", (Decimal?)InterestAmt);
}
/** Get Interest Amount.
@return Interest Amount */
public Decimal GetInterestAmt() 
{
Object bd =Get_Value("InterestAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_Value ("IsManual", IsManual);
}
/** Get Manual.
@return This is a manual process */
public Boolean IsManual() 
{
Object oo = Get_Value("IsManual");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Reversal.
@param IsReversal This is a reversing transaction */
public void SetIsReversal (Boolean IsReversal)
{
Set_Value ("IsReversal", IsReversal);
}
/** Get Reversal.
@return This is a reversing transaction */
public Boolean IsReversal() 
{
Object oo = Get_Value("IsReversal");
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
/** Set Match Statement.
@param MatchStatement Match Statement */
public void SetMatchStatement (String MatchStatement)
{
if (MatchStatement != null && MatchStatement.Length > 1)
{
log.Warning("Length > 1 - truncated");
MatchStatement = MatchStatement.Substring(0,1);
}
Set_Value ("MatchStatement", MatchStatement);
}
/** Get Match Statement.
@return Match Statement */
public String GetMatchStatement() 
{
return (String)Get_Value("MatchStatement");
}
/** Set Memo.
@param Memo Memo Text */
public void SetMemo (String Memo)
{
if (Memo != null && Memo.Length > 255)
{
log.Warning("Length > 255 - truncated");
Memo = Memo.Substring(0,255);
}
Set_Value ("Memo", Memo);
}
/** Get Memo.
@return Memo Text */
public String GetMemo() 
{
return (String)Get_Value("Memo");
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
/** Set Statement Line Date.
@param StatementLineDate Date of the Statement Line */
public void SetStatementLineDate (DateTime? StatementLineDate)
{
if (StatementLineDate == null) throw new ArgumentException ("StatementLineDate is mandatory.");
Set_Value ("StatementLineDate", (DateTime?)StatementLineDate);
}
/** Get Statement Line Date.
@return Date of the Statement Line */
public DateTime? GetStatementLineDate() 
{
return (DateTime?)Get_Value("StatementLineDate");
}
/** Set Statement amount.
@param StmtAmt Statement Amount */
public void SetStmtAmt (Decimal? StmtAmt)
{
if (StmtAmt == null) throw new ArgumentException ("StmtAmt is mandatory.");
Set_Value ("StmtAmt", (Decimal?)StmtAmt);
}
/** Get Statement amount.
@return Statement Amount */
public Decimal GetStmtAmt() 
{
Object bd =Get_Value("StmtAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Transaction Amount.
@param TrxAmt Amount of a transaction */
public void SetTrxAmt (Decimal? TrxAmt)
{
if (TrxAmt == null) throw new ArgumentException ("TrxAmt is mandatory.");
Set_Value ("TrxAmt", (Decimal?)TrxAmt);
}
/** Get Transaction Amount.
@return Amount of a transaction */
public Decimal GetTrxAmt() 
{
Object bd =Get_Value("TrxAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Effective date.
@param ValutaDate Date when money is available */
public void SetValutaDate (DateTime? ValutaDate)
{
if (ValutaDate == null) throw new ArgumentException ("ValutaDate is mandatory.");
Set_Value ("ValutaDate", (DateTime?)ValutaDate);
}
/** Get Effective date.
@return Date when money is available */
public DateTime? GetValutaDate() 
{
return (DateTime?)Get_Value("ValutaDate");
}
}

}
