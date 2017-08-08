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
/** Generated Model for Fact_Acct_Balance
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_Fact_Acct_Balance : PO
{
public X_Fact_Acct_Balance (Context ctx, int Fact_Acct_Balance_ID, Trx trxName) : base (ctx, Fact_Acct_Balance_ID, trxName)
{
/** if (Fact_Acct_Balance_ID == 0)
{
SetAccount_ID (0);
SetAmtAcctCr (0.0);
SetAmtAcctDr (0.0);
SetC_AcctSchema_ID (0);
SetDateAcct (DateTime.Now);
SetPostingType (null);
SetQty (0.0);
}
 */
}
public X_Fact_Acct_Balance (Ctx ctx, int Fact_Acct_Balance_ID, Trx trxName) : base (ctx, Fact_Acct_Balance_ID, trxName)
{
/** if (Fact_Acct_Balance_ID == 0)
{
SetAccount_ID (0);
SetAmtAcctCr (0.0);
SetAmtAcctDr (0.0);
SetC_AcctSchema_ID (0);
SetDateAcct (DateTime.Now);
SetPostingType (null);
SetQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Fact_Acct_Balance (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Fact_Acct_Balance (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_Fact_Acct_Balance (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_Fact_Acct_Balance()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514376199L;
/** Last Updated Timestamp 7/29/2010 1:07:39 PM */
public static long updatedMS = 1280389059410L;
/** AD_Table_ID=547 */
public static int Table_ID;
 // =547;

/** TableName=Fact_Acct_Balance */
public static String Table_Name="Fact_Acct_Balance";

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
StringBuilder sb = new StringBuilder ("X_Fact_Acct_Balance[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_OrgTrx_ID AD_Reference_ID=276 */
public static int AD_ORGTRX_ID_AD_Reference_ID=276;
/** Set Trx Organization.
@param AD_OrgTrx_ID Performing or initiating organization */
public void SetAD_OrgTrx_ID (int AD_OrgTrx_ID)
{
if (AD_OrgTrx_ID <= 0) Set_Value ("AD_OrgTrx_ID", null);
else
Set_Value ("AD_OrgTrx_ID", AD_OrgTrx_ID);
}
/** Get Trx Organization.
@return Performing or initiating organization */
public int GetAD_OrgTrx_ID() 
{
Object ii = Get_Value("AD_OrgTrx_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Account_ID AD_Reference_ID=182 */
public static int ACCOUNT_ID_AD_Reference_ID=182;
/** Set Account.
@param Account_ID Account used */
public void SetAccount_ID (int Account_ID)
{
if (Account_ID < 1) throw new ArgumentException ("Account_ID is mandatory.");
Set_ValueNoCheck ("Account_ID", Account_ID);
}
/** Get Account.
@return Account used */
public int GetAccount_ID() 
{
Object ii = Get_Value("Account_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Accounted Credit.
@param AmtAcctCr Accounted Credit Amount */
public void SetAmtAcctCr (Decimal? AmtAcctCr)
{
if (AmtAcctCr == null) throw new ArgumentException ("AmtAcctCr is mandatory.");
Set_Value ("AmtAcctCr", (Decimal?)AmtAcctCr);
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
Set_Value ("AmtAcctDr", (Decimal?)AmtAcctDr);
}
/** Get Accounted Debit.
@return Accounted Debit Amount */
public Decimal GetAmtAcctDr() 
{
Object bd =Get_Value("AmtAcctDr");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_Value ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_AcctSchema_ID().ToString());
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

/** C_LocFrom_ID AD_Reference_ID=133 */
public static int C_LOCFROM_ID_AD_Reference_ID=133;
/** Set Location From.
@param C_LocFrom_ID Location that inventory was moved from */
public void SetC_LocFrom_ID (int C_LocFrom_ID)
{
if (C_LocFrom_ID <= 0) Set_Value ("C_LocFrom_ID", null);
else
Set_Value ("C_LocFrom_ID", C_LocFrom_ID);
}
/** Get Location From.
@return Location that inventory was moved from */
public int GetC_LocFrom_ID() 
{
Object ii = Get_Value("C_LocFrom_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** C_LocTo_ID AD_Reference_ID=133 */
public static int C_LOCTO_ID_AD_Reference_ID=133;
/** Set Location To.
@param C_LocTo_ID Location that inventory was moved to */
public void SetC_LocTo_ID (int C_LocTo_ID)
{
if (C_LocTo_ID <= 0) Set_Value ("C_LocTo_ID", null);
else
Set_Value ("C_LocTo_ID", C_LocTo_ID);
}
/** Get Location To.
@return Location that inventory was moved to */
public int GetC_LocTo_ID() 
{
Object ii = Get_Value("C_LocTo_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Phase.
@param C_ProjectPhase_ID Phase of a Project */
public void SetC_ProjectPhase_ID (int C_ProjectPhase_ID)
{
if (C_ProjectPhase_ID <= 0) Set_ValueNoCheck ("C_ProjectPhase_ID", null);
else
Set_ValueNoCheck ("C_ProjectPhase_ID", C_ProjectPhase_ID);
}
/** Get Project Phase.
@return Phase of a Project */
public int GetC_ProjectPhase_ID() 
{
Object ii = Get_Value("C_ProjectPhase_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Task.
@param C_ProjectTask_ID Actual Project Task in a Phase */
public void SetC_ProjectTask_ID (int C_ProjectTask_ID)
{
if (C_ProjectTask_ID <= 0) Set_ValueNoCheck ("C_ProjectTask_ID", null);
else
Set_ValueNoCheck ("C_ProjectTask_ID", C_ProjectTask_ID);
}
/** Get Project Task.
@return Actual Project Task in a Phase */
public int GetC_ProjectTask_ID() 
{
Object ii = Get_Value("C_ProjectTask_ID");
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
/** Set Sub Account.
@param C_SubAcct_ID Sub account for Element Value */
public void SetC_SubAcct_ID (int C_SubAcct_ID)
{
if (C_SubAcct_ID <= 0) Set_ValueNoCheck ("C_SubAcct_ID", null);
else
Set_ValueNoCheck ("C_SubAcct_ID", C_SubAcct_ID);
}
/** Get Sub Account.
@return Sub account for Element Value */
public int GetC_SubAcct_ID() 
{
Object ii = Get_Value("C_SubAcct_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Virtual = V */
public static String POSTINGTYPE_Virtual = "V";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsPostingTypeValid (String test)
{
return test.Equals("A") || test.Equals("B") || test.Equals("E") || test.Equals("R") || test.Equals("S")|| test.Equals("V");
}
/** Set PostingType.
@param PostingType The type of posted amount for the transaction */
public void SetPostingType (String PostingType)
{
if (PostingType == null) throw new ArgumentException ("PostingType is mandatory");
if (!IsPostingTypeValid(PostingType))
throw new ArgumentException ("PostingType Invalid value - " + PostingType + " - Reference_ID=125 - A - B - E - R - S - V");
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
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
if (Qty == null) throw new ArgumentException ("Qty is mandatory.");
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

/** User1_ID AD_Reference_ID=134 */
public static int USER1_ID_AD_Reference_ID=134;
/** Set User List 1.
@param User1_ID User defined list element #1 */
public void SetUser1_ID (int User1_ID)
{
if (User1_ID <= 0) Set_Value ("User1_ID", null);
else
Set_Value ("User1_ID", User1_ID);
}
/** Get User List 1.
@return User defined list element #1 */
public int GetUser1_ID() 
{
Object ii = Get_Value("User1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** User2_ID AD_Reference_ID=137 */
public static int USER2_ID_AD_Reference_ID=137;
/** Set User List 2.
@param User2_ID User defined list element #2 */
public void SetUser2_ID (int User2_ID)
{
if (User2_ID <= 0) Set_Value ("User2_ID", null);
else
Set_Value ("User2_ID", User2_ID);
}
/** Get User List 2.
@return User defined list element #2 */
public int GetUser2_ID() 
{
Object ii = Get_Value("User2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Element 1.
@param UserElement1_ID User defined accounting Element */
public void SetUserElement1_ID (int UserElement1_ID)
{
if (UserElement1_ID <= 0) Set_Value ("UserElement1_ID", null);
else
Set_Value ("UserElement1_ID", UserElement1_ID);
}
/** Get User Element 1.
@return User defined accounting Element */
public int GetUserElement1_ID() 
{
Object ii = Get_Value("UserElement1_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set User Element 2.
@param UserElement2_ID User defined accounting Element */
public void SetUserElement2_ID (int UserElement2_ID)
{
if (UserElement2_ID <= 0) Set_Value ("UserElement2_ID", null);
else
Set_Value ("UserElement2_ID", UserElement2_ID);
}
/** Get User Element 2.
@return User defined accounting Element */
public int GetUserElement2_ID() 
{
Object ii = Get_Value("UserElement2_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
