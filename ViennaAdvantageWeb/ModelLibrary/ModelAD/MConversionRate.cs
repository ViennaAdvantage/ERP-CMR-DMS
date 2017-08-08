/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MConversionRate
 * Purpose        : Currency Conversion Rate Model
 * Class Used     : X_C_Conversion_Rate
 * Chronological    Development
 * Raghunandan      28-04-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Windows.Forms;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MConversionRate : X_C_Conversion_Rate
    {
        TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        //Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MConversionRate).FullName);

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Conversion_Rate_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MConversionRate(Ctx ctx, int C_Conversion_Rate_ID, Trx trxName)
            : base(ctx, C_Conversion_Rate_ID, trxName)
        {
            if (C_Conversion_Rate_ID == 0)
            {
                //	setC_Conversion_Rate_ID (0);
                //	setC_Currency_ID (0);
                //	setC_Currency_To_ID (0);
                base.SetDivideRate(Env.ZERO);
                base.SetMultiplyRate(Env.ZERO);
                //SetValidFrom(new DateTime(CommonFunctions.CurrentTimeMillis()));
                SetValidFrom(DateTime.Now.Date);
            }
        }

        /// <summary>
        ///	Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MConversionRate(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        ///New Constructor
        /// </summary>
        /// <param name="po">parent</param>
        /// <param name="C_ConversionType_ID">conversion type</param>
        /// <param name="C_Currency_ID">currency to</param>
        /// <param name="C_Currency_To_ID"></param>
        /// <param name="MultiplyRate">multiply rate</param>
        /// <param name="ValidFrom">valid from</param>
        public MConversionRate(PO po, int C_ConversionType_ID, int C_Currency_ID, int C_Currency_To_ID,
            Decimal multiplyRate, DateTime? validFrom)
            : this(po.GetCtx(), 0, po.Get_TrxName())
        {
            //this(po.getCtx(), 0, po.get_TrxName());
            SetClientOrg(po);
            SetC_ConversionType_ID(C_ConversionType_ID);
            SetC_Currency_ID(C_Currency_ID);
            SetC_Currency_To_ID(C_Currency_To_ID);
            SetMultiplyRate(multiplyRate);
            SetValidFrom(validFrom);
        }

        /// <summary>
        /// Convert an amount to base Currency
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Amt">amount to be converted</param>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="ConvDate">conversion date - if null - use current date</param>
        /// <param name="C_ConversionType_ID">conversion rate type - if 0 - use Default</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>converted amount</returns>
        public static Decimal ConvertBase(Ctx ctx, Decimal amt, int CurFrom_ID,
            DateTime? convDate, int C_ConversionType_ID, int AD_Client_ID, int AD_Org_ID)
        {
            return Convert(ctx, amt, CurFrom_ID, VAdvantage.Model.MClient.Get(ctx).GetC_Currency_ID(),
                convDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
        }

        /// <summary>
        ///Convert an amount with today's default rate
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Amt">amount to be converted</param>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="CurTo_ID">The C_Currency_ID TO</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>converted amount</returns>
        public static Decimal Convert(Ctx ctx, decimal amt, int CurFrom_ID, int CurTo_ID,
            int AD_Client_ID, int AD_Org_ID)
        {
            return Convert(ctx, amt, CurFrom_ID, CurTo_ID, null, 0, AD_Client_ID, AD_Org_ID);
        }

        /// <summary>
        ///Convert an amount
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="Amt">amount to be converted</param>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="CurTo_ID">The C_Currency_ID TO</param>
        /// <param name="ConvDate">conversion date - if null - use current date</param>
        /// <param name="C_ConversionType_ID">C_ConversionType_ID conversion rate type - if 0 - use Default</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>converted amount or null if no rate</returns>
        public static Decimal Convert(Ctx ctx, Decimal amt, int CurFrom_ID, int CurTo_ID,
            DateTime? convDate, int C_ConversionType_ID,
            int AD_Client_ID, int AD_Org_ID)
        {
            //if (amt == null)
            //{
            //    throw new ArgumentException("Required parameter missing - Amt");
            //}
            if (CurFrom_ID == CurTo_ID || amt.Equals(Env.ZERO))
            {
                return amt;
            }
            //	Get Rate
            Decimal retValue = GetRate(CurFrom_ID, CurTo_ID, convDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
            //if (retValue == null)
            //{
            //    //return null;
            //    return retValue;
            //}
            //	Get Amount in Currency Precision
            retValue = Decimal.Multiply(retValue, amt);
            int stdPrecision = MCurrency.GetStdPrecision(ctx, CurTo_ID);
            if (Env.Scale(retValue) > stdPrecision)
            {
                retValue = Decimal.Round(retValue, stdPrecision, MidpointRounding.AwayFromZero);
            }
            return retValue;
        }

        /// <summary>
        ///Get Currency Conversion Rate
        /// </summary>
        /// <param name="CurFrom_ID">The C_Currency_ID FROM</param>
        /// <param name="CurTo_ID">The C_Currency_ID TO</param>
        /// <param name="ConvDate">The Conversion date - if null - use current date</param>
        /// <param name="ConversionType_ID">Conversion rate type - if 0 - use Default</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="AD_Org_ID">organization</param>
        /// <returns>currency Rate or null</returns>
        public static Decimal GetRate(int CurFrom_ID, int CurTo_ID,
            DateTime? convDate, int ConversionType_ID, int AD_Client_ID, int AD_Org_ID)
        {
            if (CurFrom_ID == CurTo_ID)
            {
                return Env.ONE;
            }
            //	Conversion Type
            int C_ConversionType_ID = ConversionType_ID;
            if (C_ConversionType_ID == 0)
                C_ConversionType_ID = MConversionType.GetDefault(AD_Client_ID);
            //	Conversion Date
            if (convDate == null)
            {
               // convDate = new DateTime(CommonFunctions.CurrentTimeMillis());
                convDate = System.DateTime.Now.Date;
            }

            //	Get Rate
            String sql = "SELECT MultiplyRate "
                + "FROM C_Conversion_Rate "
                + "WHERE C_Currency_ID=" + CurFrom_ID					//	#1
                + " AND C_Currency_To_ID=" + CurTo_ID					//	#2
                + " AND	C_ConversionType_ID=" + C_ConversionType_ID				//	#3
                + " AND " + DataBase.DB.TO_DATE(convDate, true) + " BETWEEN ValidFrom AND ValidTo"	//	#4	TRUNC (?) ORA-00932: inconsistent datatypes: expected NUMBER got TIMESTAMP
                + " AND AD_Client_ID IN (0," + AD_Client_ID + ")"				//	#5
                + " AND AD_Org_ID IN (0," + AD_Org_ID + ") "				//	#6
                + "ORDER BY AD_Client_ID DESC, AD_Org_ID DESC, ValidFrom DESC";
            //decimal retValue = null;
            decimal? retValue =  null;
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = Utility.Util.GetValueOfDecimal(dr[0].ToString());
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            if (retValue == null)
            {
                _log.Info("Not found - CurFrom=" + CurFrom_ID
          + ", CurTo=" + CurTo_ID
          + ", " + convDate
          + ", Type=" + ConversionType_ID + (ConversionType_ID == C_ConversionType_ID ? "" : "->" + C_ConversionType_ID)
          + ", Client=" + AD_Client_ID
          + ", Org=" + AD_Org_ID);
                retValue = 0;
            }
            return retValue.Value;
        }

        /**
         * 	Callout
         *	@param MultiplyRateOld old value
         *	@param MultiplyRateNew new value
         *	@param windowNo windowNo
         */
        //@UICallout
        public void SetMultiplyRate(String multiplyRateOld, String multiplyRateNew, int windowNo)
        {
            SetMultiplyRate(ConvertToBigDecimal(multiplyRateNew));
        }

        /// <summary>
        /// Set Multiply Rate
        /// Sets also Divide Rate
        /// </summary>
        /// <param name="MultiplyRate">multiply rate</param>
        public new void SetMultiplyRate(Decimal? multiplyRate)
        {
            if (multiplyRate == null
                ||Env.Signum(System.Convert.ToDecimal(multiplyRate)) == 0
                || (System.Convert.ToDecimal(multiplyRate)).CompareTo(Env.ONE) == 0)
            {
                base.SetDivideRate(Env.ONE);
                base.SetMultiplyRate(Env.ONE);
            }
            else
            {
                base.SetMultiplyRate(System.Convert.ToDecimal(multiplyRate));
                double dd = System.Convert.ToDouble((1 / System.Convert.ToDouble(multiplyRate)));
                base.SetDivideRate(System.Convert.ToDecimal(dd));
            }
        }

        /**
         * 	Callout
         *	@param DivideRateOld old value
         *	@param DivideRateNew new value
         *	@param windowNo window no
         */
        //@UICallout
        public void SetDivideRate (String DivideRateOld, String DivideRateNew, int WindowNo)
        {
            SetDivideRate(ConvertToBigDecimal(DivideRateNew));
        }	

        /// <summary>
        /// Set Divide Rate.
        /// Sets also Multiply Rate
        /// </summary>
        /// <param name="DivideRate">divide rate</param>
        public new void SetDivideRate(Decimal? divideRate)
        {
            if (divideRate == null
                ||Env.Signum(System.Convert.ToDecimal(divideRate)) == 0
                || ((Decimal)divideRate).CompareTo(Env.ONE) == 0)
            {
                base.SetDivideRate(Env.ONE);
                base.SetMultiplyRate(Env.ONE);
            }
            else
            {
                base.SetDivideRate(System.Convert.ToDecimal(divideRate));
                //double dd = 1 / DivideRate.doubleValue();
                double dd = System.Convert.ToDouble((1 / System.Convert.ToDecimal(divideRate)));
                //base.setMultiplyRate(new BigDecimal(dd));
                base.SetMultiplyRate(System.Convert.ToDecimal(dd));
            }
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MConversionRate[");
            sb.Append(Get_ID())
                .Append(",Currency=").Append(GetC_Currency_ID())
                .Append(",To=").Append(GetC_Currency_To_ID())
                .Append(", Multiply=").Append(GetMultiplyRate())
                .Append(",Divide=").Append(GetDivideRate())
                .Append(", ValidFrom=").Append(GetValidFrom());
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Before Save.
        /// - Same Currency
        /// - Date Range Check
        /// - Set To date to 2056
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true if OK to save</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            //	From - To is the same
            if (GetC_Currency_ID() == GetC_Currency_To_ID())
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@C_Currency_ID@ = @C_Currency_ID@"));
                return false;
            }
            //	Nothing to convert
            if (GetMultiplyRate().CompareTo(Utility.Env.ZERO) <= 0)
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@MultiplyRate@ <= 0"));
                return false;
            }

            //	Date Range Check
            DateTime? from = GetValidFrom();
            if (GetValidTo() == null)
            {
                SetValidTo(TimeUtil.GetDay(2056, 1, 29));	//	 no exchange rates after my 100th birthday
            }
            DateTime? to = GetValidTo();

            //if (to.before(from))
            if (to < from)
            {
                //SimpleDateFormat df = DisplayType.getDateFormat(DisplayType.Date);
                 log.SaveError("Error", to + " < " + from);
                return false;
            }
            return true;
        }

    }
}
