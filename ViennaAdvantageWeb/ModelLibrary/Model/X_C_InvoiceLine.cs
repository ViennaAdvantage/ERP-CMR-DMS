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
    /** Generated Model for C_InvoiceLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_InvoiceLine : PO
    {
        public X_C_InvoiceLine(Context ctx, int C_InvoiceLine_ID, Trx trxName)
            : base(ctx, C_InvoiceLine_ID, trxName)
        {
            /** if (C_InvoiceLine_ID == 0)
            {
            SetC_InvoiceLine_ID (0);
            SetC_Invoice_ID (0);
            SetIsDescription (false);	// N
            SetIsPrinted (true);	// Y
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_InvoiceLine WHERE C_Invoice_ID=@C_Invoice_ID@
            SetLineNetAmt (0.0);
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetProcessed (false);	// N
            SetQtyEntered (0.0);	// 1
            SetQtyInvoiced (0.0);	// 1
            }
             */
        }
        public X_C_InvoiceLine(Ctx ctx, int C_InvoiceLine_ID, Trx trxName)
            : base(ctx, C_InvoiceLine_ID, trxName)
        {
            /** if (C_InvoiceLine_ID == 0)
            {
            SetC_InvoiceLine_ID (0);
            SetC_Invoice_ID (0);
            SetIsDescription (false);	// N
            SetIsPrinted (true);	// Y
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM C_InvoiceLine WHERE C_Invoice_ID=@C_Invoice_ID@
            SetLineNetAmt (0.0);
            SetPriceActual (0.0);
            SetPriceEntered (0.0);
            SetPriceLimit (0.0);
            SetPriceList (0.0);
            SetProcessed (false);	// N
            SetQtyEntered (0.0);	// 1
            SetQtyInvoiced (0.0);	// 1
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_InvoiceLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_InvoiceLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_InvoiceLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_InvoiceLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514372485L;
        /** Last Updated Timestamp 7/29/2010 1:07:35 PM */
        public static long updatedMS = 1280389055696L;
        /** AD_Table_ID=333 */
        public static int Table_ID;
        // =333;

        /** TableName=C_InvoiceLine */
        public static String Table_Name = "C_InvoiceLine";

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
            StringBuilder sb = new StringBuilder("X_C_InvoiceLine[").Append(Get_ID()).Append("]");
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
        /** Set Asset.
        @param A_Asset_ID Asset used internally or by customers */
        public void SetA_Asset_ID(int A_Asset_ID)
        {
            if (A_Asset_ID <= 0) Set_Value("A_Asset_ID", null);
            else
                Set_Value("A_Asset_ID", A_Asset_ID);
        }
        /** Get Asset.
        @return Asset used internally or by customers */
        public int GetA_Asset_ID()
        {
            Object ii = Get_Value("A_Asset_ID");
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
        /** Set Invoice Line.
        @param C_InvoiceLine_ID Invoice Detail Line */
        public void SetC_InvoiceLine_ID(int C_InvoiceLine_ID)
        {
            if (C_InvoiceLine_ID < 1) throw new ArgumentException("C_InvoiceLine_ID is mandatory.");
            Set_ValueNoCheck("C_InvoiceLine_ID", C_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetC_InvoiceLine_ID()
        {
            Object ii = Get_Value("C_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID < 1) throw new ArgumentException("C_Invoice_ID is mandatory.");
            Set_ValueNoCheck("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_Invoice_ID().ToString());
        }
        /** Set Order Line.
        @param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_ValueNoCheck("C_OrderLine_ID", null);
            else
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
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
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
            if (C_UOM_ID <= 0) Set_ValueNoCheck("C_UOM_ID", null);
            else
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
        /** Set Printed.
        @param IsPrinted Indicates if this document / line is printed */
        public void SetIsPrinted(Boolean IsPrinted)
        {
            Set_Value("IsPrinted", IsPrinted);
        }
        /** Get Printed.
        @return Indicates if this document / line is printed */
        public Boolean IsPrinted()
        {
            Object oo = Get_Value("IsPrinted");
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

        /** LineDocStatus AD_Reference_ID=131 */
        public static int LINEDOCSTATUS_AD_Reference_ID = 131;
        /** Unknown = ?? */
        public static String LINEDOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String LINEDOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String LINEDOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String LINEDOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String LINEDOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String LINEDOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String LINEDOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String LINEDOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String LINEDOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String LINEDOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String LINEDOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String LINEDOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsLineDocStatusValid(String test)
        {
            return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Line Document Status.
        @param LineDocStatus The current status of the document line */
        public void SetLineDocStatus(String LineDocStatus)
        {
            if (!IsLineDocStatusValid(LineDocStatus))
                throw new ArgumentException("LineDocStatus Invalid value - " + LineDocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (LineDocStatus != null && LineDocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                LineDocStatus = LineDocStatus.Substring(0, 2);
            }
            Set_Value("LineDocStatus", LineDocStatus);
        }
        /** Get Line Document Status.
        @return The current status of the document line */
        public String GetLineDocStatus()
        {
            return (String)Get_Value("LineDocStatus");
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
        /** Set Line Total.
        @param LineTotalAmt Total line amount incl. Tax */
        public void SetLineTotalAmt(Decimal? LineTotalAmt)
        {
            Set_Value("LineTotalAmt", (Decimal?)LineTotalAmt);
        }
        /** Get Line Total.
        @return Total line amount incl. Tax */
        public Decimal GetLineTotalAmt()
        {
            Object bd = Get_Value("LineTotalAmt");
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
        /** Set Shipment/Receipt Line.
        @param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID <= 0) Set_ValueNoCheck("M_InOutLine_ID", null);
            else
                Set_ValueNoCheck("M_InOutLine_ID", M_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID()
        {
            Object ii = Get_Value("M_InOutLine_ID");
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
            Set_Value("QtyInvoiced", (Decimal?)QtyInvoiced);
        }
        /** Get Quantity Invoiced.
        @return Invoiced Quantity */
        public Decimal GetQtyInvoiced()
        {
            Object bd = Get_Value("QtyInvoiced");
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
        /** Set Referenced Invoice Line.
        @param Ref_InvoiceLine_ID Referenced Invoice Line */
        public void SetRef_InvoiceLine_ID(int Ref_InvoiceLine_ID)
        {
            if (Ref_InvoiceLine_ID <= 0) Set_Value("Ref_InvoiceLine_ID", null);
            else
                Set_Value("Ref_InvoiceLine_ID", Ref_InvoiceLine_ID);
        }
        /** Get Referenced Invoice Line.
        @return Referenced Invoice Line */
        public int GetRef_InvoiceLine_ID()
        {
            Object ii = Get_Value("Ref_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Assigned Resource.
        @param S_ResourceAssignment_ID Assigned Resource */
        public void SetS_ResourceAssignment_ID(int S_ResourceAssignment_ID)
        {
            if (S_ResourceAssignment_ID <= 0) Set_ValueNoCheck("S_ResourceAssignment_ID", null);
            else
                Set_ValueNoCheck("S_ResourceAssignment_ID", S_ResourceAssignment_ID);
        }
        /** Get Assigned Resource.
        @return Assigned Resource */
        public int GetS_ResourceAssignment_ID()
        {
            Object ii = Get_Value("S_ResourceAssignment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Amount.
        @param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt)
        {
            Set_Value("TaxAmt", (Decimal?)TaxAmt);
        }
        /** Get Tax Amount.
        @return Tax Amount for a document */
        public Decimal GetTaxAmt()
        {
            Object bd = Get_Value("TaxAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
    }

}
