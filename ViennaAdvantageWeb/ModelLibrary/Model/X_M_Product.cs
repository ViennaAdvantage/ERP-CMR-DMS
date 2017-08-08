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
/** Generated Model for M_Product
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Product : PO
    {
        public X_M_Product(Context ctx, int M_Product_ID, Trx trxName)
            : base(ctx, M_Product_ID, trxName)
        {
            /** if (M_Product_ID == 0)
            {
            SetC_TaxCategory_ID (0);
            SetC_UOM_ID (0);
            SetIsBOM (false);	// N
            SetIsDropShip (false);
            SetIsExcludeAutoDelivery (false);	// N
            SetIsInvoicePrintDetails (false);
            SetIsPickListPrintDetails (false);
            SetIsPurchased (true);	// Y
            SetIsSelfService (true);	// Y
            SetIsSold (true);	// Y
            SetIsStocked (true);	// Y
            SetIsSummary (false);
            SetIsVerified (false);	// N
            SetIsWebStoreFeatured (false);
            SetM_Product_Category_ID (0);
            SetM_Product_ID (0);
            SetName (null);
            SetProductType (null);	// I
            SetValue (null);
            }
             */
        }
        public X_M_Product(Ctx ctx, int M_Product_ID, Trx trxName)
            : base(ctx, M_Product_ID, trxName)
        {
            /** if (M_Product_ID == 0)
            {
            SetC_TaxCategory_ID (0);
            SetC_UOM_ID (0);
            SetIsBOM (false);	// N
            SetIsDropShip (false);
            SetIsExcludeAutoDelivery (false);	// N
            SetIsInvoicePrintDetails (false);
            SetIsPickListPrintDetails (false);
            SetIsPurchased (true);	// Y
            SetIsSelfService (true);	// Y
            SetIsSold (true);	// Y
            SetIsStocked (true);	// Y
            SetIsSummary (false);
            SetIsVerified (false);	// N
            SetIsWebStoreFeatured (false);
            SetM_Product_Category_ID (0);
            SetM_Product_ID (0);
            SetName (null);
            SetProductType (null);	// I
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Product(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Product()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514380447L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063658L;
        /** AD_Table_ID=208 */
        public static int Table_ID;
        // =208;

        /** TableName=M_Product */
        public static String Table_Name = "M_Product";

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
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_M_Product[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Revenue Recognition.
        @param C_RevenueRecognition_ID Method for recording revenue */
        public void SetC_RevenueRecognition_ID(int C_RevenueRecognition_ID)
        {
            if (C_RevenueRecognition_ID <= 0) Set_Value("C_RevenueRecognition_ID", null);
            else
                Set_Value("C_RevenueRecognition_ID", C_RevenueRecognition_ID);
        }
        /** Get Revenue Recognition.
        @return Method for recording revenue */
        public int GetC_RevenueRecognition_ID()
        {
            Object ii = Get_Value("C_RevenueRecognition_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Subscription Type.
        @param C_SubscriptionType_ID Type of subscription */
        public void SetC_SubscriptionType_ID(int C_SubscriptionType_ID)
        {
            if (C_SubscriptionType_ID <= 0) Set_Value("C_SubscriptionType_ID", null);
            else
                Set_Value("C_SubscriptionType_ID", C_SubscriptionType_ID);
        }
        /** Get Subscription Type.
        @return Type of subscription */
        public int GetC_SubscriptionType_ID()
        {
            Object ii = Get_Value("C_SubscriptionType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Category.
        @param C_TaxCategory_ID Tax Category */
        public void SetC_TaxCategory_ID(int C_TaxCategory_ID)
        {
            if (C_TaxCategory_ID < 1) throw new ArgumentException("C_TaxCategory_ID is mandatory.");
            Set_Value("C_TaxCategory_ID", C_TaxCategory_ID);
        }
        /** Get Tax Category.
        @return Tax Category */
        public int GetC_TaxCategory_ID()
        {
            Object ii = Get_Value("C_TaxCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
            Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Classification.
        @param Classification Classification for grouping */
        public void SetClassification(String Classification)
        {
            if (Classification != null && Classification.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Classification = Classification.Substring(0, 1);
            }
            Set_Value("Classification", Classification);
        }
        /** Get Classification.
        @return Classification for grouping */
        public String GetClassification()
        {
            return (String)Get_Value("Classification");
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Description URL.
        @param DescriptionURL URL for the description */
        public void SetDescriptionURL(String DescriptionURL)
        {
            if (DescriptionURL != null && DescriptionURL.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                DescriptionURL = DescriptionURL.Substring(0, 120);
            }
            Set_Value("DescriptionURL", DescriptionURL);
        }
        /** Get Description URL.
        @return URL for the description */
        public String GetDescriptionURL()
        {
            return (String)Get_Value("DescriptionURL");
        }
        /** Set Discontinued.
        @param Discontinued This product is no longer available */
        public void SetDiscontinued(Boolean Discontinued)
        {
            Set_Value("Discontinued", Discontinued);
        }
        /** Get Discontinued.
        @return This product is no longer available */
        public Boolean IsDiscontinued()
        {
            Object oo = Get_Value("Discontinued");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Discontinued by.
        @param DiscontinuedBy Discontinued By */
        public void SetDiscontinuedBy(DateTime? DiscontinuedBy)
        {
            Set_Value("DiscontinuedBy", (DateTime?)DiscontinuedBy);
        }
        /** Get Discontinued by.
        @return Discontinued By */
        public DateTime? GetDiscontinuedBy()
        {
            return (DateTime?)Get_Value("DiscontinuedBy");
        }
        /** Set Document Note.
        @param DocumentNote Additional information for a Document */
        public void SetDocumentNote(String DocumentNote)
        {
            if (DocumentNote != null && DocumentNote.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                DocumentNote = DocumentNote.Substring(0, 2000);
            }
            Set_Value("DocumentNote", DocumentNote);
        }
        /** Get Document Note.
        @return Additional information for a Document */
        public String GetDocumentNote()
        {
            return (String)Get_Value("DocumentNote");
        }
        /** Set Guarantee Days.
        @param GuaranteeDays Number of days the product is guaranteed or available */
        public void SetGuaranteeDays(int GuaranteeDays)
        {
            Set_Value("GuaranteeDays", GuaranteeDays);
        }
        /** Get Guarantee Days.
        @return Number of days the product is guaranteed or available */
        public int GetGuaranteeDays()
        {
            Object ii = Get_Value("GuaranteeDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Min Guarantee Days.
        @param GuaranteeDaysMin Minumum number of guarantee days */
        public void SetGuaranteeDaysMin(int GuaranteeDaysMin)
        {
            Set_Value("GuaranteeDaysMin", GuaranteeDaysMin);
        }
        /** Get Min Guarantee Days.
        @return Minumum number of guarantee days */
        public int GetGuaranteeDaysMin()
        {
            Object ii = Get_Value("GuaranteeDaysMin");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Comment.
        @param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Help = Help.Substring(0, 2000);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
        @return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }
        /** Set Image URL.
        @param ImageURL URL of  image */
        public void SetImageURL(String ImageURL)
        {
            if (ImageURL != null && ImageURL.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                ImageURL = ImageURL.Substring(0, 120);
            }
            Set_Value("ImageURL", ImageURL);
        }
        /** Get Image URL.
        @return URL of  image */
        public String GetImageURL()
        {
            return (String)Get_Value("ImageURL");
        }
        /** Set Bill of Materials.
        @param IsBOM Bill of Materials */
        public void SetIsBOM(Boolean IsBOM)
        {
            Set_Value("IsBOM", IsBOM);
        }
        /** Get Bill of Materials.
        @return Bill of Materials */
        public Boolean IsBOM()
        {
            Object oo = Get_Value("IsBOM");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip)
        {
            Set_Value("IsDropShip", IsDropShip);
        }
        /** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip()
        {
            Object oo = Get_Value("IsDropShip");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Exclude Auto Delivery.
        @param IsExcludeAutoDelivery Exclude from automatic Delivery */
        public void SetIsExcludeAutoDelivery(Boolean IsExcludeAutoDelivery)
        {
            Set_Value("IsExcludeAutoDelivery", IsExcludeAutoDelivery);
        }
        /** Get Exclude Auto Delivery.
        @return Exclude from automatic Delivery */
        public Boolean IsExcludeAutoDelivery()
        {
            Object oo = Get_Value("IsExcludeAutoDelivery");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Print detail records on invoice.
        @param IsInvoicePrintDetails Print detail BOM elements on the invoice */
        public void SetIsInvoicePrintDetails(Boolean IsInvoicePrintDetails)
        {
            Set_Value("IsInvoicePrintDetails", IsInvoicePrintDetails);
        }
        /** Get Print detail records on invoice.
        @return Print detail BOM elements on the invoice */
        public Boolean IsInvoicePrintDetails()
        {
            Object oo = Get_Value("IsInvoicePrintDetails");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Print detail records on pick list.
        @param IsPickListPrintDetails Print detail BOM elements on the pick list */
        public void SetIsPickListPrintDetails(Boolean IsPickListPrintDetails)
        {
            Set_Value("IsPickListPrintDetails", IsPickListPrintDetails);
        }
        /** Get Print detail records on pick list.
        @return Print detail BOM elements on the pick list */
        public Boolean IsPickListPrintDetails()
        {
            Object oo = Get_Value("IsPickListPrintDetails");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Purchased.
        @param IsPurchased Organization purchases this product */
        public void SetIsPurchased(Boolean IsPurchased)
        {
            Set_Value("IsPurchased", IsPurchased);
        }
        /** Get Purchased.
        @return Organization purchases this product */
        public Boolean IsPurchased()
        {
            Object oo = Get_Value("IsPurchased");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Self-Service.
        @param IsSelfService This is a Self-Service entry or this entry can be changed via Self-Service */
        public void SetIsSelfService(Boolean IsSelfService)
        {
            Set_Value("IsSelfService", IsSelfService);
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
        /** Set Sold.
        @param IsSold Organization sells this product */
        public void SetIsSold(Boolean IsSold)
        {
            Set_Value("IsSold", IsSold);
        }
        /** Get Sold.
        @return Organization sells this product */
        public Boolean IsSold()
        {
            Object oo = Get_Value("IsSold");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Stocked.
        @param IsStocked Organization stocks this product */
        public void SetIsStocked(Boolean IsStocked)
        {
            Set_Value("IsStocked", IsStocked);
        }
        /** Get Stocked.
        @return Organization stocks this product */
        public Boolean IsStocked()
        {
            Object oo = Get_Value("IsStocked");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary Level.
        @param IsSummary This is a summary entity */
        public void SetIsSummary(Boolean IsSummary)
        {
            Set_Value("IsSummary", IsSummary);
        }
        /** Get Summary Level.
        @return This is a summary entity */
        public Boolean IsSummary()
        {
            Object oo = Get_Value("IsSummary");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Verified.
        @param IsVerified The BOM configuration has been verified */
        public void SetIsVerified(Boolean IsVerified)
        {
            Set_ValueNoCheck("IsVerified", IsVerified);
        }
        /** Get Verified.
        @return The BOM configuration has been verified */
        public Boolean IsVerified()
        {
            Object oo = Get_Value("IsVerified");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Featured in Web Store.
        @param IsWebStoreFeatured If selected, the product is displayed in the inital or any empy search */
        public void SetIsWebStoreFeatured(Boolean IsWebStoreFeatured)
        {
            Set_Value("IsWebStoreFeatured", IsWebStoreFeatured);
        }
        /** Get Featured in Web Store.
        @return If selected, the product is displayed in the inital or any empy search */
        public Boolean IsWebStoreFeatured()
        {
            Object oo = Get_Value("IsWebStoreFeatured");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set License Info.
        @param LicenseInfo License Information */
        public void SetLicenseInfo(String LicenseInfo)
        {
            if (LicenseInfo != null && LicenseInfo.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                LicenseInfo = LicenseInfo.Substring(0, 255);
            }
            Set_Value("LicenseInfo", LicenseInfo);
        }
        /** Get License Info.
        @return License Information */
        public String GetLicenseInfo()
        {
            return (String)Get_Value("LicenseInfo");
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute Set.
        @param M_AttributeSet_ID Product Attribute Set */
        public void SetM_AttributeSet_ID(int M_AttributeSet_ID)
        {
            if (M_AttributeSet_ID <= 0) Set_Value("M_AttributeSet_ID", null);
            else
                Set_Value("M_AttributeSet_ID", M_AttributeSet_ID);
        }
        /** Get Attribute Set.
        @return Product Attribute Set */
        public int GetM_AttributeSet_ID()
        {
            Object ii = Get_Value("M_AttributeSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Freight Category.
        @param M_FreightCategory_ID Category of the Freight */
        public void SetM_FreightCategory_ID(int M_FreightCategory_ID)
        {
            if (M_FreightCategory_ID <= 0) Set_Value("M_FreightCategory_ID", null);
            else
                Set_Value("M_FreightCategory_ID", M_FreightCategory_ID);
        }
        /** Get Freight Category.
        @return Category of the Freight */
        public int GetM_FreightCategory_ID()
        {
            Object ii = Get_Value("M_FreightCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID <= 0) Set_Value("M_Locator_ID", null);
            else
                Set_Value("M_Locator_ID", M_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetM_Locator_ID()
        {
            Object ii = Get_Value("M_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_Product_Category_ID AD_Reference_ID=163 */
        public static int M_PRODUCT_CATEGORY_ID_AD_Reference_ID = 163;
        /** Set Product Category.
        @param M_Product_Category_ID Category of a Product */
        public void SetM_Product_Category_ID(int M_Product_Category_ID)
        {
            if (M_Product_Category_ID < 1) throw new ArgumentException("M_Product_Category_ID is mandatory.");
            Set_Value("M_Product_Category_ID", M_Product_Category_ID);
        }
        /** Get Product Category.
        @return Category of a Product */
        public int GetM_Product_Category_ID()
        {
            Object ii = Get_Value("M_Product_Category_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
            Set_ValueNoCheck("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
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
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
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

        /** ProductType AD_Reference_ID=270 */
        public static int PRODUCTTYPE_AD_Reference_ID = 270;
        /** Expense type = E */
        public static String PRODUCTTYPE_ExpenseType = "E";
        /** Item = I */
        public static String PRODUCTTYPE_Item = "I";
        /** Online = O */
        public static String PRODUCTTYPE_Online = "O";
        /** Resource = R */
        public static String PRODUCTTYPE_Resource = "R";
        /** Service = S */
        public static String PRODUCTTYPE_Service = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsProductTypeValid(String test)
        {
            return test.Equals("E") || test.Equals("I") || test.Equals("O") || test.Equals("R") || test.Equals("S");
        }
        /** Set Product Type.
        @param ProductType Type of product */
        public void SetProductType(String ProductType)
        {
            if (ProductType == null) throw new ArgumentException("ProductType is mandatory");
            if (!IsProductTypeValid(ProductType))
                throw new ArgumentException("ProductType Invalid value - " + ProductType + " - Reference_ID=270 - E - I - O - R - S");
            if (ProductType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ProductType = ProductType.Substring(0, 1);
            }
            Set_Value("ProductType", ProductType);
        }
        /** Get Product Type.
        @return Type of product */
        public String GetProductType()
        {
            return (String)Get_Value("ProductType");
        }
        /** Set Mail Template.
        @param R_MailText_ID Text templates for mailings */
        public void SetR_MailText_ID(int R_MailText_ID)
        {
            if (R_MailText_ID <= 0) Set_Value("R_MailText_ID", null);
            else
                Set_Value("R_MailText_ID", R_MailText_ID);
        }
        /** Get Mail Template.
        @return Text templates for mailings */
        public int GetR_MailText_ID()
        {
            Object ii = Get_Value("R_MailText_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Source.
        @param R_Source_ID Source for the Lead or Request */
        public void SetR_Source_ID(int R_Source_ID)
        {
            if (R_Source_ID <= 0) Set_Value("R_Source_ID", null);
            else
                Set_Value("R_Source_ID", R_Source_ID);
        }
        /** Get Source.
        @return Source for the Lead or Request */
        public int GetR_Source_ID()
        {
            Object ii = Get_Value("R_Source_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set SKU.
        @param SKU Stock Keeping Unit */
        public void SetSKU(String SKU)
        {
            if (SKU != null && SKU.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                SKU = SKU.Substring(0, 30);
            }
            Set_Value("SKU", SKU);
        }
        /** Get SKU.
        @return Stock Keeping Unit */
        public String GetSKU()
        {
            return (String)Get_Value("SKU");
        }
        /** Set Expense Type.
        @param S_ExpenseType_ID Expense report type */
        public void SetS_ExpenseType_ID(int S_ExpenseType_ID)
        {
            if (S_ExpenseType_ID <= 0) Set_ValueNoCheck("S_ExpenseType_ID", null);
            else
                Set_ValueNoCheck("S_ExpenseType_ID", S_ExpenseType_ID);
        }
        /** Get Expense Type.
        @return Expense report type */
        public int GetS_ExpenseType_ID()
        {
            Object ii = Get_Value("S_ExpenseType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Resource.
        @param S_Resource_ID Resource */
        public void SetS_Resource_ID(int S_Resource_ID)
        {
            if (S_Resource_ID <= 0) Set_ValueNoCheck("S_Resource_ID", null);
            else
                Set_ValueNoCheck("S_Resource_ID", S_Resource_ID);
        }
        /** Get Resource.
        @return Resource */
        public int GetS_Resource_ID()
        {
            Object ii = Get_Value("S_Resource_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
        /** Set Representative.
        @param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Representative.
        @return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shelf Depth.
        @param ShelfDepth Shelf depth required */
        public void SetShelfDepth(int ShelfDepth)
        {
            Set_Value("ShelfDepth", ShelfDepth);
        }
        /** Get Shelf Depth.
        @return Shelf depth required */
        public int GetShelfDepth()
        {
            Object ii = Get_Value("ShelfDepth");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shelf Height.
        @param ShelfHeight Shelf height required */
        public void SetShelfHeight(int ShelfHeight)
        {
            Set_Value("ShelfHeight", ShelfHeight);
        }
        /** Get Shelf Height.
        @return Shelf height required */
        public int GetShelfHeight()
        {
            Object ii = Get_Value("ShelfHeight");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shelf Width.
        @param ShelfWidth Shelf width required */
        public void SetShelfWidth(int ShelfWidth)
        {
            Set_Value("ShelfWidth", ShelfWidth);
        }
        /** Get Shelf Width.
        @return Shelf width required */
        public int GetShelfWidth()
        {
            Object ii = Get_Value("ShelfWidth");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Support Units.
        @param SupportUnits Number of Support Units, e.g. Supported Internal Users */
        public void SetSupportUnits(int SupportUnits)
        {
            Set_Value("SupportUnits", SupportUnits);
        }
        /** Get Support Units.
        @return Number of Support Units, e.g. Supported Internal Users */
        public int GetSupportUnits()
        {
            Object ii = Get_Value("SupportUnits");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Trial Phase Days.
        @param TrialPhaseDays Days for a Trail */
        public void SetTrialPhaseDays(int TrialPhaseDays)
        {
            Set_Value("TrialPhaseDays", TrialPhaseDays);
        }
        /** Get Trial Phase Days.
        @return Days for a Trail */
        public int GetTrialPhaseDays()
        {
            Object ii = Get_Value("TrialPhaseDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UPC/EAN.
        @param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC)
        {
            if (UPC != null && UPC.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                UPC = UPC.Substring(0, 30);
            }
            Set_Value("UPC", UPC);
        }
        /** Get UPC/EAN.
        @return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC()
        {
            return (String)Get_Value("UPC");
        }
        /** Set Units Per Pallet.
        @param UnitsPerPallet Units Per Pallet */
        public void SetUnitsPerPallet(int UnitsPerPallet)
        {
            Set_Value("UnitsPerPallet", UnitsPerPallet);
        }
        /** Get Units Per Pallet.
        @return Units Per Pallet */
        public int GetUnitsPerPallet()
        {
            Object ii = Get_Value("UnitsPerPallet");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
        /** Set Version No.
        @param VersionNo Version Number */
        public void SetVersionNo(String VersionNo)
        {
            if (VersionNo != null && VersionNo.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                VersionNo = VersionNo.Substring(0, 20);
            }
            Set_Value("VersionNo", VersionNo);
        }
        /** Get Version No.
        @return Version Number */
        public String GetVersionNo()
        {
            return (String)Get_Value("VersionNo");
        }
        /** Set Volume.
        @param Volume Volume of a product */
        public void SetVolume(Decimal? Volume)
        {
            Set_Value("Volume", (Decimal?)Volume);
        }
        /** Get Volume.
        @return Volume of a product */
        public Decimal GetVolume()
        {
            Object bd = Get_Value("Volume");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Weight.
        @param Weight Weight of a product */
        public void SetWeight(Decimal? Weight)
        {
            Set_Value("Weight", (Decimal?)Weight);
        }
        /** Get Weight.
        @return Weight of a product */
        public Decimal GetWeight()
        {
            Object bd = Get_Value("Weight");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set BINARYDESCRIPTION.
        @param BINARYDESCRIPTION BINARYDESCRIPTION */
        public void SetBINARYDESCRIPTION(Byte[] BINARYDESCRIPTION)
        {
            Set_Value("BINARYDESCRIPTION", BINARYDESCRIPTION);
        }
        /** Get BINARYDESCRIPTION.
        @return BINARYDESCRIPTION */
        public Byte[] GetBINARYDESCRIPTION()
        {
            return (Byte[])Get_Value("BINARYDESCRIPTION");
        }

        /** Set UploadWithoutInventory.
@param UploadWithoutInventory UploadWithoutInventory */
        public void SetUploadWithoutInventory(Boolean UploadWithoutInventory)
        {
            Set_Value("UploadWithoutInventory", UploadWithoutInventory);
        }
        /** Get UploadWithoutInventory.
        @return UploadWithoutInventory */
        public Boolean IsUploadWithoutInventory()
        {
            Object oo = Get_Value("UploadWithoutInventory");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Available.
        @param IsAvailable Resource is available */
        public void SetIsAvailable(Boolean IsAvailable)
        {
            Set_Value("IsAvailable", IsAvailable);
        }
        /** Get Available.
        @return Resource is available */
        public Boolean IsAvailable()
        {
            Object oo = Get_Value("IsAvailable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
    }

}
