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
    /** Generated Model for M_MovementLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_MovementLine : PO
    {
        public X_M_MovementLine(Context ctx, int M_MovementLine_ID, Trx trxName)
            : base(ctx, M_MovementLine_ID, trxName)
        {
            /** if (M_MovementLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID=@M_Movement_ID@
            SetM_LocatorTo_ID (0);	// @M_LocatorTo_ID@
            SetM_Locator_ID (0);	// @M_Locator_ID@
            SetM_MovementLine_ID (0);
            SetM_Movement_ID (0);
            SetM_Product_ID (0);
            SetMovementQty (0.0);	// 1
            SetProcessed (false);	// N
            }
             */
        }
        public X_M_MovementLine(Ctx ctx, int M_MovementLine_ID, Trx trxName)
            : base(ctx, M_MovementLine_ID, trxName)
        {
            /** if (M_MovementLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID=@M_Movement_ID@
            SetM_LocatorTo_ID (0);	// @M_LocatorTo_ID@
            SetM_Locator_ID (0);	// @M_Locator_ID@
            SetM_MovementLine_ID (0);
            SetM_Movement_ID (0);
            SetM_Product_ID (0);
            SetMovementQty (0.0);	// 1
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MovementLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MovementLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MovementLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_MovementLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514380102L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063313L;
        /** AD_Table_ID=324 */
        public static int Table_ID;
        // =324;

        /** TableName=M_MovementLine */
        public static String Table_Name = "M_MovementLine";

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
            StringBuilder sb = new StringBuilder("X_M_MovementLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Confirmed Quantity.
        @param ConfirmedQty Confirmation of a received quantity */
        public void SetConfirmedQty(Decimal? ConfirmedQty)
        {
            Set_Value("ConfirmedQty", (Decimal?)ConfirmedQty);
        }
        /** Get Confirmed Quantity.
        @return Confirmation of a received quantity */
        public Decimal GetConfirmedQty()
        {
            Object bd = Get_Value("ConfirmedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Attribute Number.
        @param DTD001_AttributeNumber Attribute Number */
        public void SetDTD001_AttributeNumber(String DTD001_AttributeNumber)
        {
            Set_Value("DTD001_AttributeNumber", DTD001_AttributeNumber);
        }
        /** Get Attribute Number.
        @return Attribute Number */
        public String GetDTD001_AttributeNumber()
        {
            return (String)Get_Value("DTD001_AttributeNumber");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetLine().ToString());
        }

        /** M_AttributeSetInstanceTo_ID AD_Reference_ID=418 */
        public static int M_ATTRIBUTESETINSTANCETO_ID_AD_Reference_ID = 418;
        /** Set Attribute Set Instance To.
        @param M_AttributeSetInstanceTo_ID Target Product Attribute Set Instance */
        public void SetM_AttributeSetInstanceTo_ID(int M_AttributeSetInstanceTo_ID)
        {
            if (M_AttributeSetInstanceTo_ID <= 0) Set_ValueNoCheck("M_AttributeSetInstanceTo_ID", null);
            else
                Set_ValueNoCheck("M_AttributeSetInstanceTo_ID", M_AttributeSetInstanceTo_ID);
        }
        /** Get Attribute Set Instance To.
        @return Target Product Attribute Set Instance */
        public int GetM_AttributeSetInstanceTo_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstanceTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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

        /** M_LocatorTo_ID AD_Reference_ID=191 */
        public static int M_LOCATORTO_ID_AD_Reference_ID = 191;
        /** Set Locator To.
        @param M_LocatorTo_ID Location inventory is moved to */
        public void SetM_LocatorTo_ID(int M_LocatorTo_ID)
        {
            if (M_LocatorTo_ID < 1) throw new ArgumentException("M_LocatorTo_ID is mandatory.");
            Set_Value("M_LocatorTo_ID", M_LocatorTo_ID);
        }
        /** Get Locator To.
        @return Location inventory is moved to */
        public int GetM_LocatorTo_ID()
        {
            Object ii = Get_Value("M_LocatorTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID < 1) throw new ArgumentException("M_Locator_ID is mandatory.");
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
        /** Set Move Line.
        @param M_MovementLine_ID Inventory Move document Line */
        public void SetM_MovementLine_ID(int M_MovementLine_ID)
        {
            if (M_MovementLine_ID < 1) throw new ArgumentException("M_MovementLine_ID is mandatory.");
            Set_ValueNoCheck("M_MovementLine_ID", M_MovementLine_ID);
        }
        /** Get Move Line.
        @return Inventory Move document Line */
        public int GetM_MovementLine_ID()
        {
            Object ii = Get_Value("M_MovementLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Inventory Move.
        @param M_Movement_ID Movement of Inventory */
        public void SetM_Movement_ID(int M_Movement_ID)
        {
            if (M_Movement_ID < 1) throw new ArgumentException("M_Movement_ID is mandatory.");
            Set_ValueNoCheck("M_Movement_ID", M_Movement_ID);
        }
        /** Get Inventory Move.
        @return Movement of Inventory */
        public int GetM_Movement_ID()
        {
            Object ii = Get_Value("M_Movement_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_Product_ID AD_Reference_ID=171 */
        public static int M_PRODUCT_ID_AD_Reference_ID = 171;
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
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
        /** Set Requisition Line.
        @param M_RequisitionLine_ID Material Requisition Line */
        public void SetM_RequisitionLine_ID(int M_RequisitionLine_ID)
        {
            if (M_RequisitionLine_ID <= 0) Set_Value("M_RequisitionLine_ID", null);
            else
                Set_Value("M_RequisitionLine_ID", M_RequisitionLine_ID);
        }
        /** Get Requisition Line.
        @return Material Requisition Line */
        public int GetM_RequisitionLine_ID()
        {
            Object ii = Get_Value("M_RequisitionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Movement Quantity.
        @param MovementQty Quantity of a product moved. */
        public void SetMovementQty(Decimal? MovementQty)
        {
            if (MovementQty == null) throw new ArgumentException("MovementQty is mandatory.");
            Set_Value("MovementQty", (Decimal?)MovementQty);
        }
        /** Get Movement Quantity.
        @return Quantity of a product moved. */
        public Decimal GetMovementQty()
        {
            Object bd = Get_Value("MovementQty");
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
        /** Set Scrapped Quantity.
        @param ScrappedQty The Quantity scrapped due to QA issues */
        public void SetScrappedQty(Decimal? ScrappedQty)
        {
            Set_Value("ScrappedQty", (Decimal?)ScrappedQty);
        }
        /** Get Scrapped Quantity.
        @return The Quantity scrapped due to QA issues */
        public Decimal GetScrappedQty()
        {
            Object bd = Get_Value("ScrappedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Target Quantity.
        @param TargetQty Target Movement Quantity */
        public void SetTargetQty(Decimal? TargetQty)
        {
            Set_Value("TargetQty", (Decimal?)TargetQty);
        }
        /** Get Target Quantity.
        @return Target Movement Quantity */
        public Decimal GetTargetQty()
        {
            Object bd = Get_Value("TargetQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}
