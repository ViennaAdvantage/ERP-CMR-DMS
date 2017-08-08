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
    /** Generated Model for C_OrderLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_OrderLine : PO
    {
        public X_C_OrderLine(Context ctx, int C_OrderLine_ID, Trx trxName)
            : base(ctx, C_OrderLine_ID, trxName)
        {
            /** if (C_OrderLine_ID == 0)
            {
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_OrderLine_ID (0);
            SetC_Order_ID (0);
            SetC_Tax_ID (0);
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetDateOrdered (DateTime.Now);	// @DateOrdered@
            SetFreightAmt (0.0);
            SetIsDescription (false);	// N
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_OrderLine WHERE C_Order_ID=@C_Order_ID@
            SetLineNetAmt (0.0);
            SetM_Warehouse_ID (0);	// @M_Warehouse_ID@
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetProcessed (false);	// N
            SetQtyDelivered (0.0);
            SetQtyEntered (0.0);	// 1
            SetQtyInvoiced (0.0);
            SetQtyLostSales (0.0);
            SetQtyOrdered (0.0);	// 1
            SetQtyReserved (0.0);
            }
             */
        }
        public X_C_OrderLine(Ctx ctx, int C_OrderLine_ID, Trx trxName)
            : base(ctx, C_OrderLine_ID, trxName)
        {
            /** if (C_OrderLine_ID == 0)
            {
            SetC_Currency_ID (0);	// @C_Currency_ID@
            SetC_OrderLine_ID (0);
            SetC_Order_ID (0);
            SetC_Tax_ID (0);
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetDateOrdered (DateTime.Now);	// @DateOrdered@
            SetFreightAmt (0.0);
            SetIsDescription (false);	// N
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM C_OrderLine WHERE C_Order_ID=@C_Order_ID@
            SetLineNetAmt (0.0);
            SetM_Warehouse_ID (0);	// @M_Warehouse_ID@
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetProcessed (false);	// N
            SetQtyDelivered (0.0);
            SetQtyEntered (0.0);	// 1
            SetQtyInvoiced (0.0);
            SetQtyLostSales (0.0);
            SetQtyOrdered (0.0);	// 1
            SetQtyReserved (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_OrderLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_OrderLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_OrderLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_OrderLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514373331L;
        /** Last Updated Timestamp 7/29/2010 1:07:36 PM */
        public static long updatedMS = 1280389056542L;
        /** AD_Table_ID=260 */
        public static int Table_ID;
        // =260;

        /** TableName=C_OrderLine */
        public static String Table_Name = "C_OrderLine";

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
            StringBuilder sb = new StringBuilder("X_C_OrderLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetAD_OrgTrx_ID()
        {
            Object ii = Get_Value("AD_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Activity.
        @param C_Activity_ID Business Activity */
        public void SetC_Activity_ID(int C_Activity_ID)
        {
            if (C_Activity_ID <= 0) Set_Value("C_Activity_ID", null);
            else
                Set_Value("C_Activity_ID", C_Activity_ID);
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
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_ValueNoCheck("C_BPartner_ID", null);
            else
                Set_ValueNoCheck("C_BPartner_ID", C_BPartner_ID);
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
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetC_Campaign_ID()
        {
            Object ii = Get_Value("C_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge.
        @param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
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
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_ValueNoCheck("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Line.
        @param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID < 1) throw new ArgumentException("C_OrderLine_ID is mandatory.");
            Set_ValueNoCheck("C_OrderLine_ID", C_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetC_OrderLine_ID()
        {
            Object ii = Get_Value("C_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param C_Order_ID Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID < 1) throw new ArgumentException("C_Order_ID is mandatory.");
            Set_ValueNoCheck("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_Order_ID().ToString());
        }
        /** Set Project Phase.
        @param C_ProjectPhase_ID Phase of a Project */
        public void SetC_ProjectPhase_ID(int C_ProjectPhase_ID)
        {
            if (C_ProjectPhase_ID <= 0) Set_ValueNoCheck("C_ProjectPhase_ID", null);
            else
                Set_ValueNoCheck("C_ProjectPhase_ID", C_ProjectPhase_ID);
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
        public void SetC_ProjectTask_ID(int C_ProjectTask_ID)
        {
            if (C_ProjectTask_ID <= 0) Set_ValueNoCheck("C_ProjectTask_ID", null);
            else
                Set_ValueNoCheck("C_ProjectTask_ID", C_ProjectTask_ID);
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
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax.
        @param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID < 1) throw new ArgumentException("C_Tax_ID is mandatory.");
            Set_Value("C_Tax_ID", C_Tax_ID);
        }
        /** Get Tax.
        @return Tax identifier */
        public int GetC_Tax_ID()
        {
            Object ii = Get_Value("C_Tax_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
            Set_ValueNoCheck("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Date Delivered.
        @param DateDelivered Date when the product was delivered */
        public void SetDateDelivered(DateTime? DateDelivered)
        {
            Set_ValueNoCheck("DateDelivered", (DateTime?)DateDelivered);
        }
        /** Get Date Delivered.
        @return Date when the product was delivered */
        public DateTime? GetDateDelivered()
        {
            return (DateTime?)Get_Value("DateDelivered");
        }
        /** Set Date Invoiced.
        @param DateInvoiced Date printed on Invoice */
        public void SetDateInvoiced(DateTime? DateInvoiced)
        {
            Set_ValueNoCheck("DateInvoiced", (DateTime?)DateInvoiced);
        }
        /** Get Date Invoiced.
        @return Date printed on Invoice */
        public DateTime? GetDateInvoiced()
        {
            return (DateTime?)Get_Value("DateInvoiced");
        }
        /** Set Date Ordered.
        @param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered)
        {
            if (DateOrdered == null) throw new ArgumentException("DateOrdered is mandatory.");
            Set_Value("DateOrdered", (DateTime?)DateOrdered);
        }
        /** Get Date Ordered.
        @return Date of Order */
        public DateTime? GetDateOrdered()
        {
            return (DateTime?)Get_Value("DateOrdered");
        }
        /** Set Date Promised.
        @param DatePromised Date Order was promised */
        public void SetDatePromised(DateTime? DatePromised)
        {
            Set_Value("DatePromised", (DateTime?)DatePromised);
        }
        /** Get Date Promised.
        @return Date Order was promised */
        public DateTime? GetDatePromised()
        {
            return (DateTime?)Get_Value("DatePromised");
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
        /** Set Discount %.
        @param Discount Discount in percent */
        public void SetDiscount(Decimal? Discount)
        {
            Set_Value("Discount", (Decimal?)Discount);
        }
        /** Get Discount %.
        @return Discount in percent */
        public Decimal GetDiscount()
        {
            Object bd = Get_Value("Discount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Freight Amount.
        @param FreightAmt Freight Amount */
        public void SetFreightAmt(Decimal? FreightAmt)
        {
            if (FreightAmt == null) throw new ArgumentException("FreightAmt is mandatory.");
            Set_Value("FreightAmt", (Decimal?)FreightAmt);
        }
        /** Get Freight Amount.
        @return Freight Amount */
        public Decimal GetFreightAmt()
        {
            Object bd = Get_Value("FreightAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Description Only.
        @param IsDescription if true, the line is just description and no transaction */
        public void SetIsDescription(Boolean IsDescription)
        {
            Set_Value("IsDescription", IsDescription);
        }
        /** Get Description Only.
        @return if true, the line is just description and no transaction */
        public Boolean IsDescription()
        {
            Object oo = Get_Value("IsDescription");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line No.
        @param Line Unique line for this document */
        public void SetLine(int Line)
        {
            Set_Value("Line", Line);
        }
        /** Get Line No.
        @return Unique line for this document */
        public int GetLine()
        {
            Object ii = Get_Value("Line");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Line Amount.
        @param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public void SetLineNetAmt(Decimal? LineNetAmt)
        {
            if (LineNetAmt == null) throw new ArgumentException("LineNetAmt is mandatory.");
            Set_ValueNoCheck("LineNetAmt", (Decimal?)LineNetAmt);
        }
        /** Get Line Amount.
        @return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public Decimal GetLineNetAmt()
        {
            Object bd = Get_Value("LineNetAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Freight Carrier.
        @param M_Shipper_ID Method or manner of product delivery */
        public void SetM_Shipper_ID(int M_Shipper_ID)
        {
            if (M_Shipper_ID <= 0) Set_Value("M_Shipper_ID", null);
            else
                Set_Value("M_Shipper_ID", M_Shipper_ID);
        }
        /** Get Freight Carrier.
        @return Method or manner of product delivery */
        public int GetM_Shipper_ID()
        {
            Object ii = Get_Value("M_Shipper_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_Warehouse_ID AD_Reference_ID=197 */
        public static int M_WAREHOUSE_ID_AD_Reference_ID = 197;
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_InOutLine_ID AD_Reference_ID=295 */
        public static int ORIG_INOUTLINE_ID_AD_Reference_ID = 295;
        /** Set Orig Shipment Line.
        @param Orig_InOutLine_ID Original shipment line of the RMA */
        public void SetOrig_InOutLine_ID(int Orig_InOutLine_ID)
        {
            if (Orig_InOutLine_ID <= 0) Set_Value("Orig_InOutLine_ID", null);
            else
                Set_Value("Orig_InOutLine_ID", Orig_InOutLine_ID);
        }
        /** Get Orig Shipment Line.
        @return Original shipment line of the RMA */
        public int GetOrig_InOutLine_ID()
        {
            Object ii = Get_Value("Orig_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_OrderLine_ID AD_Reference_ID=271 */
        public static int ORIG_ORDERLINE_ID_AD_Reference_ID = 271;
        /** Set Orig Sales Order Line.
        @param Orig_OrderLine_ID Original Sales Order Line for Return Material Authorization */
        public void SetOrig_OrderLine_ID(int Orig_OrderLine_ID)
        {
            if (Orig_OrderLine_ID <= 0) Set_Value("Orig_OrderLine_ID", null);
            else
                Set_Value("Orig_OrderLine_ID", Orig_OrderLine_ID);
        }
        /** Get Orig Sales Order Line.
        @return Original Sales Order Line for Return Material Authorization */
        public int GetOrig_OrderLine_ID()
        {
            Object ii = Get_Value("Orig_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unit Price.
        @param PriceActual Actual Price */
        public void SetPriceActual(Decimal? PriceActual)
        {
            if (PriceActual == null) throw new ArgumentException("PriceActual is mandatory.");
            Set_ValueNoCheck("PriceActual", (Decimal?)PriceActual);
        }
        /** Get Unit Price.
        @return Actual Price */
        public Decimal GetPriceActual()
        {
            Object bd = Get_Value("PriceActual");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Cost Price.
        @param PriceCost Price per Unit of Measure including all indirect costs (Freight, etc.) */
        public void SetPriceCost(Decimal? PriceCost)
        {
            Set_Value("PriceCost", (Decimal?)PriceCost);
        }
        /** Get Cost Price.
        @return Price per Unit of Measure including all indirect costs (Freight, etc.) */
        public Decimal GetPriceCost()
        {
            Object bd = Get_Value("PriceCost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price.
        @param PriceEntered Price Entered - the price based on the selected/base UoM */
        public void SetPriceEntered(Decimal? PriceEntered)
        {
            if (PriceEntered == null) throw new ArgumentException("PriceEntered is mandatory.");
            Set_Value("PriceEntered", (Decimal?)PriceEntered);
        }
        /** Get Price.
        @return Price Entered - the price based on the selected/base UoM */
        public Decimal GetPriceEntered()
        {
            Object bd = Get_Value("PriceEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Limit Price.
        @param PriceLimit Lowest price for a product */
        public void SetPriceLimit(Decimal? PriceLimit)
        {
            if (PriceLimit == null) throw new ArgumentException("PriceLimit is mandatory.");
            Set_Value("PriceLimit", (Decimal?)PriceLimit);
        }
        /** Get Limit Price.
        @return Lowest price for a product */
        public Decimal GetPriceLimit()
        {
            Object bd = Get_Value("PriceLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set List Price.
        @param PriceList List Price */
        public void SetPriceList(Decimal? PriceList)
        {
            if (PriceList == null) throw new ArgumentException("PriceList is mandatory.");
            Set_Value("PriceList", (Decimal?)PriceList);
        }
        /** Get List Price.
        @return List Price */
        public Decimal GetPriceList()
        {
            Object bd = Get_Value("PriceList");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
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
        /** Set Quantity Delivered.
        @param QtyDelivered Quantity Delivered */
        public void SetQtyDelivered(Decimal? QtyDelivered)
        {
            if (QtyDelivered == null) throw new ArgumentException("QtyDelivered is mandatory.");
            Set_ValueNoCheck("QtyDelivered", (Decimal?)QtyDelivered);
        }
        /** Get Quantity Delivered.
        @return Quantity Delivered */
        public Decimal GetQtyDelivered()
        {
            Object bd = Get_Value("QtyDelivered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity.
        @param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered)
        {
            if (QtyEntered == null) throw new ArgumentException("QtyEntered is mandatory.");
            Set_Value("QtyEntered", (Decimal?)QtyEntered);
        }
        /** Get Quantity.
        @return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered()
        {
            Object bd = Get_Value("QtyEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Invoiced.
        @param QtyInvoiced Invoiced Quantity */
        public void SetQtyInvoiced(Decimal? QtyInvoiced)
        {
            if (QtyInvoiced == null) throw new ArgumentException("QtyInvoiced is mandatory.");
            Set_ValueNoCheck("QtyInvoiced", (Decimal?)QtyInvoiced);
        }
        /** Get Quantity Invoiced.
        @return Invoiced Quantity */
        public Decimal GetQtyInvoiced()
        {
            Object bd = Get_Value("QtyInvoiced");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Lost Sales Quantity.
        @param QtyLostSales Quantity of potential sales */
        public void SetQtyLostSales(Decimal? QtyLostSales)
        {
            if (QtyLostSales == null) throw new ArgumentException("QtyLostSales is mandatory.");
            Set_Value("QtyLostSales", (Decimal?)QtyLostSales);
        }
        /** Get Lost Sales Quantity.
        @return Quantity of potential sales */
        public Decimal GetQtyLostSales()
        {
            Object bd = Get_Value("QtyLostSales");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Ordered Quantity.
        @param QtyOrdered Ordered Quantity */
        public void SetQtyOrdered(Decimal? QtyOrdered)
        {
            if (QtyOrdered == null) throw new ArgumentException("QtyOrdered is mandatory.");
            Set_Value("QtyOrdered", (Decimal?)QtyOrdered);
        }
        /** Get Ordered Quantity.
        @return Ordered Quantity */
        public Decimal GetQtyOrdered()
        {
            Object bd = Get_Value("QtyOrdered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Reserved.
        @param QtyReserved Quantity Reserved */
        public void SetQtyReserved(Decimal? QtyReserved)
        {
            if (QtyReserved == null) throw new ArgumentException("QtyReserved is mandatory.");
            Set_ValueNoCheck("QtyReserved", (Decimal?)QtyReserved);
        }
        /** Get Quantity Reserved.
        @return Quantity Reserved */
        public Decimal GetQtyReserved()
        {
            Object bd = Get_Value("QtyReserved");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Returned.
        @param QtyReturned Quantity Returned */
        public void SetQtyReturned(Decimal? QtyReturned)
        {
            Set_Value("QtyReturned", (Decimal?)QtyReturned);
        }
        /** Get Quantity Returned.
        @return Quantity Returned */
        public Decimal GetQtyReturned()
        {
            Object bd = Get_Value("QtyReturned");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Revenue Recognition Amt.
        @param RRAmt Revenue Recognition Amount */
        public void SetRRAmt(Decimal? RRAmt)
        {
            Set_Value("RRAmt", (Decimal?)RRAmt);
        }
        /** Get Revenue Recognition Amt.
        @return Revenue Recognition Amount */
        public Decimal GetRRAmt()
        {
            Object bd = Get_Value("RRAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Revenue Recognition Start.
        @param RRStartDate Revenue Recognition Start Date */
        public void SetRRStartDate(DateTime? RRStartDate)
        {
            Set_Value("RRStartDate", (DateTime?)RRStartDate);
        }
        /** Get Revenue Recognition Start.
        @return Revenue Recognition Start Date */
        public DateTime? GetRRStartDate()
        {
            return (DateTime?)Get_Value("RRStartDate");
        }

        /** Ref_OrderLine_ID AD_Reference_ID=271 */
        public static int REF_ORDERLINE_ID_AD_Reference_ID = 271;
        /** Set Referenced Order Line.
        @param Ref_OrderLine_ID Reference to corresponding Sales/Purchase Order */
        public void SetRef_OrderLine_ID(int Ref_OrderLine_ID)
        {
            if (Ref_OrderLine_ID <= 0) Set_Value("Ref_OrderLine_ID", null);
            else
                Set_Value("Ref_OrderLine_ID", Ref_OrderLine_ID);
        }
        /** Get Referenced Order Line.
        @return Reference to corresponding Sales/Purchase Order */
        public int GetRef_OrderLine_ID()
        {
            Object ii = Get_Value("Ref_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Assigned Resource.
        @param S_ResourceAssignment_ID Assigned Resource */
        public void SetS_ResourceAssignment_ID(int S_ResourceAssignment_ID)
        {
            if (S_ResourceAssignment_ID <= 0) Set_Value("S_ResourceAssignment_ID", null);
            else
                Set_Value("S_ResourceAssignment_ID", S_ResourceAssignment_ID);
        }
        /** Get Assigned Resource.
        @return Assigned Resource */
        public int GetS_ResourceAssignment_ID()
        {
            Object ii = Get_Value("S_ResourceAssignment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User1_ID AD_Reference_ID=134 */
        public static int USER1_ID_AD_Reference_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
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
        public static int USER2_ID_AD_Reference_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Contract.
        @param IsContract Contract */
        public void SetIsContract(Boolean IsContract)
        {
            Set_Value("IsContract", IsContract);
        }
        /** Get Contract.
        @return Contract */
        public Boolean IsContract()
        {
            Object oo = Get_Value("IsContract");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set C_Contract_ID.
        @param C_Contract_ID C_Contract_ID */
        public void SetC_Contract_ID(int C_Contract_ID)
        {
            if (C_Contract_ID <= 0) Set_Value("C_Contract_ID", null);
            else
                Set_Value("C_Contract_ID", C_Contract_ID);
        }
        /** Get C_Contract_ID.
        @return C_Contract_ID */
        public int GetC_Contract_ID()
        {
            Object ii = Get_Value("C_Contract_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Start Date.
        @param StartDate First effective day (inclusive) */
        public void SetStartDate(DateTime? StartDate)
        {
            Set_Value("StartDate", (DateTime?)StartDate);
        }
        /** Get Start Date.
        @return First effective day (inclusive) */
        public DateTime? GetStartDate()
        {
            return (DateTime?)Get_Value("StartDate");
        }

        /** Set End Date.
        @param EndDate Last effective date (inclusive) */
        public void SetEndDate(DateTime? EndDate)
        {
            Set_Value("EndDate", (DateTime?)EndDate);
        }
        /** Get End Date.
        @return Last effective date (inclusive) */
        public DateTime? GetEndDate()
        {
            return (DateTime?)Get_Value("EndDate");
        }

        /** Set Billing Frequency.
        @param C_Frequency_ID Billing Frequency */
        public void SetC_Frequency_ID(int C_Frequency_ID)
        {
            if (C_Frequency_ID <= 0) Set_Value("C_Frequency_ID", null);
            else
                Set_Value("C_Frequency_ID", C_Frequency_ID);
        }
        /** Get Billing Frequency.
        @return Billing Frequency */
        public int GetC_Frequency_ID()
        {
            Object ii = Get_Value("C_Frequency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set QuantityPerCycle.
        @param QuantityPerCycle The QuantityPerCycle Entered is based on the selected UoM */
        public void SetQtyPerCycle(Decimal? QtyPerCycle)
        {
            if (QtyPerCycle == null) throw new ArgumentException("QtyPerCycle is mandatory.");
            Set_Value("QtyPerCycle", (Decimal?)QtyPerCycle);
        }
        /** Get QuantityPerCycle.
        @return The QuantityPerCycle is based on the selected UoM */
        public Decimal GetQtyPerCycle()
        {
            Object bd = Get_Value("QtyPerCycle");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set No of Cycle.
        @param NoofCycle No of Cycle */
        public void SetNoofCycle(int NoofCycle)
        {
            Set_Value("NoofCycle", NoofCycle);
        }
        /** Get No of Cycle.
        @return No of Cycle */
        public int GetNoofCycle()
        {
            Object ii = Get_Value("NoofCycle");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Checked.
        @param IsChecked Checked */
        public void SetIsChecked(Boolean IsChecked)
        {
            Set_Value("IsChecked", IsChecked);
        }
        /** Get Checked.
        @return Checked */
        public Boolean IsChecked()
        {
            Object oo = Get_Value("IsChecked");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Create Service Contract.
        @param CreateServiceContract Create Service Contract */
        public void SetCreateServiceContract(String CreateServiceContract)
        {
            if (CreateServiceContract != null && CreateServiceContract.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CreateServiceContract = CreateServiceContract.Substring(0, 50);
            }
            Set_Value("CreateServiceContract", CreateServiceContract);
        }
        /** Get Create Service Contract.
        @return Create Service Contract */
        public String GetCreateServiceContract()
        {
            return (String)Get_Value("CreateServiceContract");
        }

        /** Set Budget Violation Amount.
        @param BudgetViolationAmount Budget Violation Amount */
        public void SetBudgetViolationAmount(Decimal? BudgetViolationAmount)
        {
            Set_Value("BudgetViolationAmount", (Decimal?)BudgetViolationAmount);
        }
        /** Get Budget Violation Amount.
        @return Budget Violation Amount */
        public Decimal GetBudgetViolationAmount()
        {
            Object bd = Get_Value("BudgetViolationAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

    }

}
