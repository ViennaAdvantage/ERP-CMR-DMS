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
/** Generated Model for C_BPartner_Location
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BPartner_Location : PO
{
public X_C_BPartner_Location (Context ctx, int C_BPartner_Location_ID, Trx trxName) : base (ctx, C_BPartner_Location_ID, trxName)
{
/** if (C_BPartner_Location_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_Location_ID (0);
SetIsBillTo (true);	// Y
SetIsPayFrom (true);	// Y
SetIsRemitTo (true);	// Y
SetIsShipTo (true);	// Y
SetName (null);	// .
}
 */
}
public X_C_BPartner_Location (Ctx ctx, int C_BPartner_Location_ID, Trx trxName) : base (ctx, C_BPartner_Location_ID, trxName)
{
/** if (C_BPartner_Location_ID == 0)
{
SetC_BPartner_ID (0);
SetC_BPartner_Location_ID (0);
SetC_Location_ID (0);
SetIsBillTo (true);	// Y
SetIsPayFrom (true);	// Y
SetIsRemitTo (true);	// Y
SetIsShipTo (true);	// Y
SetName (null);	// .
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BPartner_Location (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BPartner_Location (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BPartner_Location (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BPartner_Location()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370542L;
/** Last Updated Timestamp 7/29/2010 1:07:33 PM */
public static long updatedMS = 1280389053753L;
/** AD_Table_ID=293 */
public static int Table_ID;
 // =293;

/** TableName=C_BPartner_Location */
public static String Table_Name="C_BPartner_Location";

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
StringBuilder sb = new StringBuilder ("X_C_BPartner_Location[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Business Partner.
@param C_BPartner_ID Identifies a Business Partner */
public void SetC_BPartner_ID (int C_BPartner_ID)
{
if (C_BPartner_ID < 1) throw new ArgumentException ("C_BPartner_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_ID", C_BPartner_ID);
}
/** Get Business Partner.
@return Identifies a Business Partner */
public int GetC_BPartner_ID() 
{
Object ii = Get_Value("C_BPartner_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Partner Location.
@param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
public void SetC_BPartner_Location_ID (int C_BPartner_Location_ID)
{
if (C_BPartner_Location_ID < 1) throw new ArgumentException ("C_BPartner_Location_ID is mandatory.");
Set_ValueNoCheck ("C_BPartner_Location_ID", C_BPartner_Location_ID);
}
/** Get Partner Location.
@return Identifies the (ship to) address for this Business Partner */
public int GetC_BPartner_Location_ID() 
{
Object ii = Get_Value("C_BPartner_Location_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Address.
@param C_Location_ID Location or Address */
public void SetC_Location_ID (int C_Location_ID)
{
if (C_Location_ID < 1) throw new ArgumentException ("C_Location_ID is mandatory.");
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
/** Set Fax.
@param Fax Facsimile number */
public void SetFax (String Fax)
{
if (Fax != null && Fax.Length > 40)
{
log.Warning("Length > 40 - truncated");
Fax = Fax.Substring(0,40);
}
Set_Value ("Fax", Fax);
}
/** Get Fax.
@return Facsimile number */
public String GetFax() 
{
return (String)Get_Value("Fax");
}
/** Set ISDN.
@param ISDN ISDN or modem line */
public void SetISDN (String ISDN)
{
if (ISDN != null && ISDN.Length > 40)
{
log.Warning("Length > 40 - truncated");
ISDN = ISDN.Substring(0,40);
}
Set_Value ("ISDN", ISDN);
}
/** Get ISDN.
@return ISDN or modem line */
public String GetISDN() 
{
return (String)Get_Value("ISDN");
}
/** Set Invoice Address.
@param IsBillTo Business Partner Invoice/Bill Address */
public void SetIsBillTo (Boolean IsBillTo)
{
Set_Value ("IsBillTo", IsBillTo);
}
/** Get Invoice Address.
@return Business Partner Invoice/Bill Address */
public Boolean IsBillTo() 
{
Object oo = Get_Value("IsBillTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Pay-From Address.
@param IsPayFrom Business Partner pays from that address and we'll send dunning letters there */
public void SetIsPayFrom (Boolean IsPayFrom)
{
Set_Value ("IsPayFrom", IsPayFrom);
}
/** Get Pay-From Address.
@return Business Partner pays from that address and we'll send dunning letters there */
public Boolean IsPayFrom() 
{
Object oo = Get_Value("IsPayFrom");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Remit-To Address.
@param IsRemitTo Business Partner payment address */
public void SetIsRemitTo (Boolean IsRemitTo)
{
Set_Value ("IsRemitTo", IsRemitTo);
}
/** Get Remit-To Address.
@return Business Partner payment address */
public Boolean IsRemitTo() 
{
Object oo = Get_Value("IsRemitTo");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Ship Address.
@param IsShipTo Business Partner Shipment Address */
public void SetIsShipTo (Boolean IsShipTo)
{
Set_Value ("IsShipTo", IsShipTo);
}
/** Get Ship Address.
@return Business Partner Shipment Address */
public Boolean IsShipTo() 
{
Object oo = Get_Value("IsShipTo");
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
/** Set Phone.
@param Phone Identifies a telephone number */
public void SetPhone (String Phone)
{
if (Phone != null && Phone.Length > 40)
{
log.Warning("Length > 40 - truncated");
Phone = Phone.Substring(0,40);
}
Set_Value ("Phone", Phone);
}
/** Get Phone.
@return Identifies a telephone number */
public String GetPhone() 
{
return (String)Get_Value("Phone");
}
/** Set 2nd Phone.
@param Phone2 Identifies an alternate telephone number. */
public void SetPhone2 (String Phone2)
{
if (Phone2 != null && Phone2.Length > 40)
{
log.Warning("Length > 40 - truncated");
Phone2 = Phone2.Substring(0,40);
}
Set_Value ("Phone2", Phone2);
}
/** Get 2nd Phone.
@return Identifies an alternate telephone number. */
public String GetPhone2() 
{
return (String)Get_Value("Phone2");
}
}

}
