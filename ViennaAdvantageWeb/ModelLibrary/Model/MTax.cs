/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MTax
 * Purpose        : for workflow
 * Chronological    Development
 * Raghunandan     05-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using java.math;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MTax : X_C_Tax
    {
        #region Private Variables
        //	Cache			
        private static CCache<int, MTax> _cache = new CCache<int, MTax>("C_Tax", 5);
        //	Cache of Client	
        private static CCache<int, MTax[]> _cacheAll = new CCache<int, MTax[]>("C_Tax", 5);
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MTax).FullName);
        //	100				
        private const Decimal ONEHUNDRED = 100M;
        //	Child Taxes		
        private MTax[] _childTaxes = null;
        // Postal Codes		
        private MTaxPostal[] _postals = null;

        #endregion

        /// <summary>
        /// Get All
        /// </summary>
        /// <param name="ctx">context</param>
        /// <returns>array list</returns>
        public static MTax[] GetAll(Ctx ctx)
        {
            int AD_Client_ID = ctx.GetAD_Client_ID();
            int key = AD_Client_ID;
            MTax[] retValue = (MTax[])_cacheAll[key];
            if (retValue != null)
                return retValue;

            //	Create it
            String sql = "SELECT * FROM C_Tax WHERE AD_Client_ID=@AD_Client_ID"
                + " ORDER BY C_Country_ID, C_Region_ID, To_Country_ID, To_Region_ID";
            List<MTax> list = new List<MTax>();
            //PreparedStatement pstmt = null;
            DataSet ds;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@AD_Client_ID", AD_Client_ID);
                ds = new DataSet();
                ds = DataBase.DB.ExecuteDataset(sql, param);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MTax tax = new MTax(ctx, dr, null);
                    _cache.Add(tax.GetC_Tax_ID(), tax);
                    list.Add(tax);
                }
                ds = null;
                //pstmt.close ();
                //pstmt = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                ds = null;
            }

            //	Create Array
            retValue = new MTax[list.Count];
            retValue = list.ToArray();
            //
            _cacheAll.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Tax from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Tax_ID">id</param>
        /// <returns>MTax</returns>
        public static MTax Get(Ctx ctx, int C_Tax_ID)
        {
            int key = C_Tax_ID;
            MTax retValue = (MTax)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MTax(ctx, C_Tax_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_Tax_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MTax(Ctx ctx, int C_Tax_ID, Trx trxName) :
            base(ctx, C_Tax_ID, trxName)
        {

            if (C_Tax_ID == 0)
            {
                //	setC_Tax_ID (0);		PK
                SetIsDefault(false);
                SetIsDocumentLevel(true);
                SetIsSummary(false);
                SetIsTaxExempt(false);
                //	setName (null);
                SetRate(Env.ZERO);
                SetRequiresTaxCertificate(false);
                //	setC_TaxCategory_ID (0);	//	FK
                SetSOPOType(SOPOTYPE_Both);
                SetValidFrom(TimeUtil.GetDay(1990, 1, 1));
                SetIsSalesTax(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">result set</param>
        /// <param name="trxName">transaction</param>
        public MTax(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// New Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="Name">Name</param>
        /// <param name="Rate"></param>
        /// <param name="C_TaxCategory_ID"></param>
        /// <param name="trxName">transaction</param>
        public MTax(Ctx ctx, String Name, Decimal Rate, int C_TaxCategory_ID, Trx trxName)
            : this(ctx, 0, trxName)
        {

            SetName(Name);
            SetRate(Rate);
            SetC_TaxCategory_ID(C_TaxCategory_ID);	//	FK
        }

        /// <summary>
        /// Get Child Taxes
        /// </summary>
        /// <param name="requery">reload</param>
        /// <returns>array of taxes or null</returns>
        public MTax[] GetChildTaxes(Boolean requery)
        {
            if (!IsSummary())
                return null;
            if (_childTaxes != null && !requery)
                return _childTaxes;
            //
            String sql = "SELECT * FROM C_Tax WHERE Parent_Tax_ID=" + GetC_Tax_ID();
            List<MTax> list = new List<MTax>();
            DataSet ds;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, Get_TrxName());
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MTax(GetCtx(), dr, Get_TrxName()));

                }
                ds = null;
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                ds = null;
            }

            _childTaxes = new MTax[list.Count];
            _childTaxes = list.ToArray();
            return _childTaxes;
        }

        /// <summary>
        /// Get Postal Qualifiers
        /// </summary>
        /// <param name="requery">requery</param>
        /// <returns> array of postal codes</returns>
        public MTaxPostal[] GetPostals(Boolean requery)
        {
            if (_postals != null && !requery)
                return _postals;

            String sql = "SELECT * FROM C_TaxPostal WHERE C_Tax_ID=" + GetC_Tax_ID() + " ORDER BY Postal, Postal_To";
            List<MTaxPostal> list = new List<MTaxPostal>();
            IDataReader dr = null;
            try
            {
                 dr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                while (dr.Read())
                {
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }

            _postals = new MTaxPostal[list.Count];
            _postals = list.ToArray();
            return _postals;
        }

        /// <summary>
        /// Do we have Postal Codes
        /// </summary>
        /// <returns>true if postal codes exist</returns>
        public Boolean IsPostal()
        {
            return GetPostals(false).Length > 0;
        }

        /// <summary>
        /// Is Zero Tax
        /// </summary>
        /// <returns>true if tax rate is 0</returns>
        public Boolean IsZeroTax()
        {
            return Env.ZERO.CompareTo(GetRate()) == 0;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MTax[");
            sb.Append(Get_ID()).Append(",").Append(GetName())
                .Append(", SO/PO=").Append(GetSOPOType())
                .Append(",Rate=").Append(GetRate())
                .Append(",C_TaxCategory_ID=").Append(GetC_TaxCategory_ID())
                .Append(",Summary=").Append(IsSummary())
                .Append(",Parent=").Append(GetParent_Tax_ID())
                .Append(",Country=").Append(GetC_Country_ID()).Append("|").Append(GetTo_Country_ID())
                .Append(",Region=").Append(GetC_Region_ID()).Append("|").Append(GetTo_Region_ID())
                .Append(",From=").Append(GetValidFrom())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Calculate Tax - no rounding
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="taxIncluded">if true tax is calculated from gross otherwise from net </param>
        /// <param name="scale"></param>
        /// <returns>tax amount</returns>
        public Decimal CalculateTax(Decimal amount, Boolean taxIncluded, int scale)
        {
            //	Null Tax
            if (IsZeroTax())
                return Env.ZERO;
            Decimal multiplier = Decimal.Round(Decimal.Divide(GetRate(), ONEHUNDRED), 12, MidpointRounding.AwayFromZero);
            //BigDecimal multiplier = getRate().divide(ONEHUNDRED, 12, BigDecimal.ROUND_HALF_UP);		
            Decimal? tax = null;
            if (!taxIncluded)	//	$100 * 6 / 100 == $6 == $100 * 0.06
            {
                tax = Decimal.Multiply(amount, multiplier);
            }
            else			//	$106 - ($106 / (100+6)/100) == $6 == $106 - ($106/1.06)
            {
                multiplier = Decimal.Add(multiplier, Env.ONE);
                //BigDecimal bbase = amount.divide(multiplier, 12, BigDecimal.ROUND_HALF_UP); 
                Decimal bbase = Decimal.Divide(amount, multiplier);
                bbase = Decimal.Round(bbase, 12, MidpointRounding.AwayFromZero);


                tax = Decimal.Subtract(amount, bbase);
            }
            Decimal finalTax = Decimal.Round((Decimal)tax, scale, MidpointRounding.AwayFromZero);
            //BigDecimal finalTax = tax.setScale(scale, BigDecimal.ROUND_HALF_UP);
            log.Fine("calculateTax " + amount 
                + " (incl=" + taxIncluded + ",mult=" + multiplier + ",scale=" + scale 
                + ") = " + finalTax + " [" + tax + "]");
            return finalTax;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <param name="success"></param>
        /// <returns>success</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (newRecord & success)
                Insert_Accounting("C_Tax_Acct", "C_AcctSchema_Default", null);

            return success;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true</returns>
        protected override Boolean BeforeDelete()
        {
            return Delete_Accounting("C_Tax_Acct");
        }
    }
}
