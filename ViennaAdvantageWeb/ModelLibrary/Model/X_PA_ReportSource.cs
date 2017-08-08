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
/** Generated Model for PA_ReportSource
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_ReportSource : PO
{
public X_PA_ReportSource (Context ctx, int PA_ReportSource_ID, Trx trxName) : base (ctx, PA_ReportSource_ID, trxName)
{
/** if (PA_ReportSource_ID == 0)
{
SetElementType (null);
SetPA_ReportLine_ID (0);
SetPA_ReportSource_ID (0);
}
 */
}
public X_PA_ReportSource (Ctx ctx, int PA_ReportSource_ID, Trx trxName) : base (ctx, PA_ReportSource_ID, trxName)
{
/** if (PA_ReportSource_ID == 0)
{
SetElementType (null);
SetPA_ReportLine_ID (0);
SetPA_ReportSource_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ReportSource (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ReportSource (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_ReportSource (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_ReportSource()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514382280L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065491L;
/** AD_Table_ID=450 */
public static int Table_ID;
 // =450;

/** TableName=PA_ReportSource */
public static String Table_Name="PA_ReportSource";

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
StringBuilder sb = new StringBuilder ("X_PA_ReportSource[").Append(Get_ID()).Append("]");
return sb.ToString();
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

/** C_ElementValue_ID AD_Reference_ID=182 */
public static int C_ELEMENTVALUE_ID_AD_Reference_ID=182;
/** Set Account Element.
@param C_ElementValue_ID Account Element */
public void SetC_ElementValue_ID (int C_ElementValue_ID)
{
if (C_ElementValue_ID <= 0) Set_Value ("C_ElementValue_ID", null);
else
Set_Value ("C_ElementValue_ID", C_ElementValue_ID);
}
/** Get Account Element.
@return Account Element */
public int GetC_ElementValue_ID() 
{
Object ii = Get_Value("C_ElementValue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID)
{
if (C_Location_ID <= 0) Set_Value ("C_Location_ID", null);
else
Set_Value ("C_Location_ID", C_Location_ID);
}
/** Get Address.
@return Location or Address */
public int GetC_Location_ID() 
{
Object ii = Get_Value("C_Location_ID");
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

/** ElementType AD_Reference_ID=181 */
public static int ELEMENTTYPE_AD_Reference_ID=181;
/** Account = AC */
public static String ELEMENTTYPE_Account = "AC";
/** Activity = AY */
public static String ELEMENTTYPE_Activity = "AY";
/** BPartner = BP */
public static String ELEMENTTYPE_BPartner = "BP";
/** Location From = LF */
public static String ELEMENTTYPE_LocationFrom = "LF";
/** Location To = LT */
public static String ELEMENTTYPE_LocationTo = "LT";
/** Campaign = MC */
public static String ELEMENTTYPE_Campaign = "MC";
/** Organization = OO */
public static String ELEMENTTYPE_Organization = "OO";
/** Org Trx = OT */
public static String ELEMENTTYPE_OrgTrx = "OT";
/** Project = PJ */
public static String ELEMENTTYPE_Project = "PJ";
/** Product = PR */
public static String ELEMENTTYPE_Product = "PR";
/** Sub Account = SA */
public static String ELEMENTTYPE_SubAccount = "SA";
/** Sales Region = SR */
public static String ELEMENTTYPE_SalesRegion = "SR";
/** User List 1 = U1 */
public static String ELEMENTTYPE_UserList1 = "U1";
/** User List 2 = U2 */
public static String ELEMENTTYPE_UserList2 = "U2";
/** User Element 1 = X1 */
public static String ELEMENTTYPE_UserElement1 = "X1";
/** User Element 2 = X2 */
public static String ELEMENTTYPE_UserElement2 = "X2";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsElementTypeValid (String test)
{
return test.Equals("AC") || test.Equals("AY") || test.Equals("BP") || test.Equals("LF") || test.Equals("LT") || test.Equals("MC") || test.Equals("OO") || test.Equals("OT") || test.Equals("PJ") || test.Equals("PR") || test.Equals("SA") || test.Equals("SR") || test.Equals("U1") || test.Equals("U2") || test.Equals("X1") || test.Equals("X2");
}
/** Set Type.
@param ElementType Element Type (account or user defined) */
public void SetElementType (String ElementType)
{
if (ElementType == null) throw new ArgumentException ("ElementType is mandatory");
if (!IsElementTypeValid(ElementType))
throw new ArgumentException ("ElementType Invalid value - " + ElementType + " - Reference_ID=181 - AC - AY - BP - LF - LT - MC - OO - OT - PJ - PR - SA - SR - U1 - U2 - X1 - X2");
if (ElementType.Length > 2)
{
log.Warning("Length > 2 - truncated");
ElementType = ElementType.Substring(0,2);
}
Set_Value ("ElementType", ElementType);
}
/** Get Type.
@return Element Type (account or user defined) */
public String GetElementType() 
{
return (String)Get_Value("ElementType");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetElementType().ToString());
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

/** Org_ID AD_Reference_ID=322 */
public static int ORG_ID_AD_Reference_ID=322;
/** Set Organization.
@param Org_ID Organizational entity within client */
public void SetOrg_ID (int Org_ID)
{
if (Org_ID <= 0) Set_Value ("Org_ID", null);
else
Set_Value ("Org_ID", Org_ID);
}
/** Get Organization.
@return Organizational entity within client */
public int GetOrg_ID() 
{
Object ii = Get_Value("Org_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Line.
@param PA_ReportLine_ID Report Line */
public void SetPA_ReportLine_ID (int PA_ReportLine_ID)
{
if (PA_ReportLine_ID < 1) throw new ArgumentException ("PA_ReportLine_ID is mandatory.");
Set_ValueNoCheck ("PA_ReportLine_ID", PA_ReportLine_ID);
}
/** Get Report Line.
@return Report Line */
public int GetPA_ReportLine_ID() 
{
Object ii = Get_Value("PA_ReportLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Report Source.
@param PA_ReportSource_ID Restriction of what will be shown in Report Line */
public void SetPA_ReportSource_ID (int PA_ReportSource_ID)
{
if (PA_ReportSource_ID < 1) throw new ArgumentException ("PA_ReportSource_ID is mandatory.");
Set_ValueNoCheck ("PA_ReportSource_ID", PA_ReportSource_ID);
}
/** Get Report Source.
@return Restriction of what will be shown in Report Line */
public int GetPA_ReportSource_ID() 
{
Object ii = Get_Value("PA_ReportSource_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
