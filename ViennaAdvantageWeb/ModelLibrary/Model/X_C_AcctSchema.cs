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
/** Generated Model for C_AcctSchema
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_AcctSchema : PO
    {
        public X_C_AcctSchema(Context ctx, int C_AcctSchema_ID, Trx trxName)
            : base(ctx, C_AcctSchema_ID, trxName)
        {
            /** if (C_AcctSchema_ID == 0)
            {
            SetAutoPeriodControl (false);
            SetC_AcctSchema_ID (0);
            SetC_Currency_ID (0);
            SetCommitmentType (null);	// N
            SetCostingLevel (null);	// C
            SetCostingMethod (null);	// S
            SetGAAP (null);
            SetHasAlias (false);
            SetHasCombination (false);
            SetIsAccrual (true);	// Y
            SetIsAdjustCOGS (false);
            SetIsDiscountCorrectsTax (false);
            SetIsExplicitCostAdjustment (false);	// N
            SetIsPostServices (false);	// N
            SetIsTradeDiscountPosted (false);
            SetM_CostType_ID (0);
            SetName (null);
            SetSeparator (null);	// -
            SetTaxCorrectionType (null);	// N
            }
             */
        }
        public X_C_AcctSchema(Ctx ctx, int C_AcctSchema_ID, Trx trxName)
            : base(ctx, C_AcctSchema_ID, trxName)
        {
            /** if (C_AcctSchema_ID == 0)
            {
            SetAutoPeriodControl (false);
            SetC_AcctSchema_ID (0);
            SetC_Currency_ID (0);
            SetCommitmentType (null);	// N
            SetCostingLevel (null);	// C
            SetCostingMethod (null);	// S
            SetGAAP (null);
            SetHasAlias (false);
            SetHasCombination (false);
            SetIsAccrual (true);	// Y
            SetIsAdjustCOGS (false);
            SetIsDiscountCorrectsTax (false);
            SetIsExplicitCostAdjustment (false);	// N
            SetIsPostServices (false);	// N
            SetIsTradeDiscountPosted (false);
            SetM_CostType_ID (0);
            SetName (null);
            SetSeparator (null);	// -
            SetTaxCorrectionType (null);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_AcctSchema()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27597354887785L;
        /** Last Updated Timestamp 9/5/2011 7:02:51 PM */
        public static long updatedMS = 1315229570996L;
        /** AD_Table_ID=265 */
        public static int Table_ID;
        // =265;

        /** TableName=C_AcctSchema */
        public static String Table_Name = "C_AcctSchema";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(2);
        /** AccessLevel
        @return 2 - Client 
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
            StringBuilder sb = new StringBuilder("X_C_AcctSchema[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgOnly_ID AD_Reference_ID=322 */
        public static int AD_ORGONLY_ID_AD_Reference_ID = 322;
        /** Set Only Organization.
        @param AD_OrgOnly_ID Create posting entries only for this organization */
        public void SetAD_OrgOnly_ID(int AD_OrgOnly_ID)
        {
            if (AD_OrgOnly_ID <= 0) Set_Value("AD_OrgOnly_ID", null);
            else
                Set_Value("AD_OrgOnly_ID", AD_OrgOnly_ID);
        }
        /** Get Only Organization.
        @return Create posting entries only for this organization */
        public int GetAD_OrgOnly_ID()
        {
            Object ii = Get_Value("AD_OrgOnly_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Automatic Period Control.
        @param AutoPeriodControl If selected, the periods are automatically opened and closed */
        public void SetAutoPeriodControl(Boolean AutoPeriodControl)
        {
            Set_Value("AutoPeriodControl", AutoPeriodControl);
        }
        /** Get Automatic Period Control.
        @return If selected, the periods are automatically opened and closed */
        public Boolean IsAutoPeriodControl()
        {
            Object oo = Get_Value("AutoPeriodControl");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory.");
            Set_ValueNoCheck("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID < 1) throw new ArgumentException("C_Currency_ID is mandatory.");
            Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Period.
        @param C_Period_ID Period of the Calendar */
        public void SetC_Period_ID(int C_Period_ID)
        {
            if (C_Period_ID <= 0) Set_ValueNoCheck("C_Period_ID", null);
            else
                Set_ValueNoCheck("C_Period_ID", C_Period_ID);
        }
        /** Get Period.
        @return Period of the Calendar */
        public int GetC_Period_ID()
        {
            Object ii = Get_Value("C_Period_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** CommitmentType AD_Reference_ID=359 */
        public static int COMMITMENTTYPE_AD_Reference_ID = 359;
        /** Commitment & Reservation = B */
        public static String COMMITMENTTYPE_CommitmentReservation = "B";
        /** Commitment only = C */
        public static String COMMITMENTTYPE_CommitmentOnly = "C";
        /** None = N */
        public static String COMMITMENTTYPE_None = "N";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCommitmentTypeValid(String test)
        {
            return test.Equals("B") || test.Equals("C") || test.Equals("N");
        }
        /** Set Commitment Type.
        @param CommitmentType Create Commitment and/or Reservations for Budget Control */
        public void SetCommitmentType(String CommitmentType)
        {
            if (CommitmentType == null) throw new ArgumentException("CommitmentType is mandatory");
            if (!IsCommitmentTypeValid(CommitmentType))
                throw new ArgumentException("CommitmentType Invalid value - " + CommitmentType + " - Reference_ID=359 - B - C - N");
            if (CommitmentType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CommitmentType = CommitmentType.Substring(0, 1);
            }
            Set_Value("CommitmentType", CommitmentType);
        }
        /** Get Commitment Type.
        @return Create Commitment and/or Reservations for Budget Control */
        public String GetCommitmentType()
        {
            return (String)Get_Value("CommitmentType");
        }

        /** CostingLevel AD_Reference_ID=355 */
        public static int COSTINGLEVEL_AD_Reference_ID = 355;
        /** Batch/Lot = B */
        public static String COSTINGLEVEL_BatchLot = "B";
        /** Client = C */
        public static String COSTINGLEVEL_Client = "C";
        /** Organization = O */
        public static String COSTINGLEVEL_Organization = "O";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCostingLevelValid(String test)
        {
            return test.Equals("B") || test.Equals("C") || test.Equals("O");
        }
        /** Set Costing Level.
        @param CostingLevel The lowest level to accumulate Costing Information */
        public void SetCostingLevel(String CostingLevel)
        {
            if (CostingLevel == null) throw new ArgumentException("CostingLevel is mandatory");
            if (!IsCostingLevelValid(CostingLevel))
                throw new ArgumentException("CostingLevel Invalid value - " + CostingLevel + " - Reference_ID=355 - B - C - O");
            if (CostingLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CostingLevel = CostingLevel.Substring(0, 1);
            }
            Set_Value("CostingLevel", CostingLevel);
        }
        /** Get Costing Level.
        @return The lowest level to accumulate Costing Information */
        public String GetCostingLevel()
        {
            return (String)Get_Value("CostingLevel");
        }

        /** CostingMethod AD_Reference_ID=122 */
        public static int COSTINGMETHOD_AD_Reference_ID = 122;
        /** Average PO = A */
        public static String COSTINGMETHOD_AveragePO = "A";
        /** Fifo = F */
        public static String COSTINGMETHOD_Fifo = "F";
        /** Average Invoice = I */
        public static String COSTINGMETHOD_AverageInvoice = "I";
        /** Lifo = L */
        public static String COSTINGMETHOD_Lifo = "L";
        /** Standard Costing = S */
        public static String COSTINGMETHOD_StandardCosting = "S";
        /** User Defined = U */
        public static String COSTINGMETHOD_UserDefined = "U";
        /** Last Invoice = i */
        public static String COSTINGMETHOD_LastInvoice = "i";
        /** Last PO Price = p */
        public static String COSTINGMETHOD_LastPOPrice = "p";
        /** _ = x */
        public static String COSTINGMETHOD__ = "x";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCostingMethodValid(String test)
        {
            return test.Equals("A") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i") || test.Equals("p") || test.Equals("x");
        }
        /** Set Costing Method.
        @param CostingMethod Indicates how Costs will be calculated */
        public void SetCostingMethod(String CostingMethod)
        {
            if (CostingMethod == null) throw new ArgumentException("CostingMethod is mandatory");
            if (!IsCostingMethodValid(CostingMethod))
                throw new ArgumentException("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - F - I - L - S - U - i - p - x");
            if (CostingMethod.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CostingMethod = CostingMethod.Substring(0, 1);
            }
            Set_Value("CostingMethod", CostingMethod);
        }
        /** Get Costing Method.
        @return Indicates how Costs will be calculated */
        public String GetCostingMethod()
        {
            return (String)Get_Value("CostingMethod");
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

        /** GAAP AD_Reference_ID=123 */
        public static int GAAP_AD_Reference_ID = 123;
        /** German HGB = DE */
        public static String GAAP_GermanHGB = "DE";
        /** French Accounting Standard = FR */
        public static String GAAP_FrenchAccountingStandard = "FR";
        /** International GAAP = UN */
        public static String GAAP_InternationalGAAP = "UN";
        /** US GAAP = US */
        public static String GAAP_USGAAP = "US";
        /** Custom Accounting Rules = XX */
        public static String GAAP_CustomAccountingRules = "XX";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsGAAPValid(String test)
        {
            return test.Equals("DE") || test.Equals("FR") || test.Equals("UN") || test.Equals("US") || test.Equals("XX");
        }
        /** Set GAAP.
        @param GAAP Generally Accepted Accounting Principles */
        public void SetGAAP(String GAAP)
        {
            if (GAAP == null) throw new ArgumentException("GAAP is mandatory");
            if (!IsGAAPValid(GAAP))
                throw new ArgumentException("GAAP Invalid value - " + GAAP + " - Reference_ID=123 - DE - FR - UN - US - XX");
            if (GAAP.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                GAAP = GAAP.Substring(0, 2);
            }
            Set_Value("GAAP", GAAP);
        }
        /** Get GAAP.
        @return Generally Accepted Accounting Principles */
        public String GetGAAP()
        {
            return (String)Get_Value("GAAP");
        }
        /** Set Use Account Alias.
        @param HasAlias Ability to select (partial) account combinations by an Alias */
        public void SetHasAlias(Boolean HasAlias)
        {
            Set_Value("HasAlias", HasAlias);
        }
        /** Get Use Account Alias.
        @return Ability to select (partial) account combinations by an Alias */
        public Boolean IsHasAlias()
        {
            Object oo = Get_Value("HasAlias");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Use Account Combination Control.
        @param HasCombination Combination of account elements are checked */
        public void SetHasCombination(Boolean HasCombination)
        {
            Set_Value("HasCombination", HasCombination);
        }
        /** Get Use Account Combination Control.
        @return Combination of account elements are checked */
        public Boolean IsHasCombination()
        {
            Object oo = Get_Value("HasCombination");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Accrual.
        @param IsAccrual Indicates if Accrual or Cash Based accounting will be used */
        public void SetIsAccrual(Boolean IsAccrual)
        {
            Set_Value("IsAccrual", IsAccrual);
        }
        /** Get Accrual.
        @return Indicates if Accrual or Cash Based accounting will be used */
        public Boolean IsAccrual()
        {
            Object oo = Get_Value("IsAccrual");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Adjust COGS.
        @param IsAdjustCOGS Adjust Cost of Good Sold */
        public void SetIsAdjustCOGS(Boolean IsAdjustCOGS)
        {
            Set_Value("IsAdjustCOGS", IsAdjustCOGS);
        }
        /** Get Adjust COGS.
        @return Adjust Cost of Good Sold */
        public Boolean IsAdjustCOGS()
        {
            Object oo = Get_Value("IsAdjustCOGS");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Correct tax for Discounts/Charges.
        @param IsDiscountCorrectsTax Correct the tax for payment discount and charges */
        public void SetIsDiscountCorrectsTax(Boolean IsDiscountCorrectsTax)
        {
            Set_Value("IsDiscountCorrectsTax", IsDiscountCorrectsTax);
        }
        /** Get Correct tax for Discounts/Charges.
        @return Correct the tax for payment discount and charges */
        public Boolean IsDiscountCorrectsTax()
        {
            Object oo = Get_Value("IsDiscountCorrectsTax");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Explicit Cost Adjustment.
        @param IsExplicitCostAdjustment Post the cost adjustment explicitly */
        public void SetIsExplicitCostAdjustment(Boolean IsExplicitCostAdjustment)
        {
            Set_Value("IsExplicitCostAdjustment", IsExplicitCostAdjustment);
        }
        /** Get Explicit Cost Adjustment.
        @return Post the cost adjustment explicitly */
        public Boolean IsExplicitCostAdjustment()
        {
            Object oo = Get_Value("IsExplicitCostAdjustment");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Services Separately.
        @param IsPostServices Differentiate between Services and Product Receivable/Payables */
        public void SetIsPostServices(Boolean IsPostServices)
        {
            Set_Value("IsPostServices", IsPostServices);
        }
        /** Get Post Services Separately.
        @return Differentiate between Services and Product Receivable/Payables */
        public Boolean IsPostServices()
        {
            Object oo = Get_Value("IsPostServices");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Post Trade Discount.
        @param IsTradeDiscountPosted Generate postings for trade discounts */
        public void SetIsTradeDiscountPosted(Boolean IsTradeDiscountPosted)
        {
            Set_Value("IsTradeDiscountPosted", IsTradeDiscountPosted);
        }
        /** Get Post Trade Discount.
        @return Generate postings for trade discounts */
        public Boolean IsTradeDiscountPosted()
        {
            Object oo = Get_Value("IsTradeDiscountPosted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Cost Type.
        @param M_CostType_ID Type of Cost (e.g. Current, Plan, Future) */
        public void SetM_CostType_ID(int M_CostType_ID)
        {
            if (M_CostType_ID < 1) throw new ArgumentException("M_CostType_ID is mandatory.");
            Set_Value("M_CostType_ID", M_CostType_ID);
        }
        /** Get Cost Type.
        @return Type of Cost (e.g. Current, Plan, Future) */
        public int GetM_CostType_ID()
        {
            Object ii = Get_Value("M_CostType_ID");
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
        /** Set NotPostPOVariance.
        @param NotPostPOVariance NotPostPOVariance */
        public void SetNotPostPOVariance(Boolean NotPostPOVariance)
        {
            Set_Value("NotPostPOVariance", NotPostPOVariance);
        }
        /** Get NotPostPOVariance.
        @return NotPostPOVariance */
        public Boolean IsNotPostPOVariance()
        {
            Object oo = Get_Value("NotPostPOVariance");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Future Days.
        @param Period_OpenFuture Number of days to be able to post to a future date (based on system date) */
        public void SetPeriod_OpenFuture(int Period_OpenFuture)
        {
            Set_Value("Period_OpenFuture", Period_OpenFuture);
        }
        /** Get Future Days.
        @return Number of days to be able to post to a future date (based on system date) */
        public int GetPeriod_OpenFuture()
        {
            Object ii = Get_Value("Period_OpenFuture");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set History Days.
        @param Period_OpenHistory Number of days to be able to post in the past (based on system date) */
        public void SetPeriod_OpenHistory(int Period_OpenHistory)
        {
            Set_Value("Period_OpenHistory", Period_OpenHistory);
        }
        /** Get History Days.
        @return Number of days to be able to post in the past (based on system date) */
        public int GetPeriod_OpenHistory()
        {
            Object ii = Get_Value("Period_OpenHistory");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Element Separator.
        @param Separator Element Separator */
        public void SetSeparator(String Separator)
        {
            if (Separator == null) throw new ArgumentException("Separator is mandatory.");
            if (Separator.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Separator = Separator.Substring(0, 1);
            }
            Set_Value("Separator", Separator);
        }
        /** Get Element Separator.
        @return Element Separator */
        public String GetSeparator()
        {
            return (String)Get_Value("Separator");
        }

        /** TaxCorrectionType AD_Reference_ID=392 */
        public static int TAXCORRECTIONTYPE_AD_Reference_ID = 392;
        /** Write-off and Discount = B */
        public static String TAXCORRECTIONTYPE_Write_OffAndDiscount = "B";
        /** Discount only = D */
        public static String TAXCORRECTIONTYPE_DiscountOnly = "D";
        /** None = N */
        public static String TAXCORRECTIONTYPE_None = "N";
        /** Write-off only = W */
        public static String TAXCORRECTIONTYPE_Write_OffOnly = "W";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsTaxCorrectionTypeValid(String test)
        {
            return test.Equals("B") || test.Equals("D") || test.Equals("N") || test.Equals("W");
        }
        /** Set Tax Correction.
        @param TaxCorrectionType Type of Tax Correction */
        public void SetTaxCorrectionType(String TaxCorrectionType)
        {
            if (TaxCorrectionType == null) throw new ArgumentException("TaxCorrectionType is mandatory");
            if (!IsTaxCorrectionTypeValid(TaxCorrectionType))
                throw new ArgumentException("TaxCorrectionType Invalid value - " + TaxCorrectionType + " - Reference_ID=392 - B - D - N - W");
            if (TaxCorrectionType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                TaxCorrectionType = TaxCorrectionType.Substring(0, 1);
            }
            Set_Value("TaxCorrectionType", TaxCorrectionType);
        }
        /** Get Tax Correction.
        @return Type of Tax Correction */
        public String GetTaxCorrectionType()
        {
            return (String)Get_Value("TaxCorrectionType");
        }
    }

}
