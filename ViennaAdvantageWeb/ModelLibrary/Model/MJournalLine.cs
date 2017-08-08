/********************************************************
 * Class Name     : MJournalLine
 * Purpose        : Journal Line Model
 * Class Used     : X_GL_JournalLine
 * Chronological    Development
 * Deepak           15-JAN-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MJournalLine : X_GL_JournalLine
    {
    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="GL_JournalLine_ID">id</param>
    /// <param name="trxName"> transaction</param>
    public MJournalLine (Ctx ctx, int GL_JournalLine_ID, Trx trxName):base(ctx, GL_JournalLine_ID, trxName)
    {
        //super (ctx, GL_JournalLine_ID, trxName);
        if (GL_JournalLine_ID == 0)
        {
        //	setGL_JournalLine_ID (0);		//	PK
        //	setGL_Journal_ID (0);			//	Parent
        //	setC_Currency_ID (0);
        //	setC_ValidCombination_ID (0);
            SetLine (0);
            SetAmtAcctCr (Env.ZERO);
            SetAmtAcctDr (Env.ZERO);
            SetAmtSourceCr (Env.ZERO);
            SetAmtSourceDr (Env.ZERO);
            SetCurrencyRate (Env.ONE);
        //	setC_ConversionType_ID (0);
            //SetDateAcct (new Timestamp(System.currentTimeMillis()));
            SetDateAcct(DateTime.Now);
            SetIsGenerated (true);
        }
    }	//	MJournalLine

    /// <summary>
    /// Load Constructor
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="rs">datarow</param>
    /// <param name="trxName">transaction</param>
    public MJournalLine (Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
    {
        //super(ctx, rs, trxName);
    }	//	MJournalLine

    /// <summary>
    /// Parent Constructor
    /// </summary>
    /// <param name="parent">journal</param>
    public MJournalLine (MJournal parent):this (parent.GetCtx(), 0, parent.Get_TrxName())
    {
        //this (parent.getCtx(), 0, parent.get_TrxName());
        SetClientOrg(parent);
        SetGL_Journal_ID(parent.GetGL_Journal_ID());
        SetC_Currency_ID(parent.GetC_Currency_ID());
        SetC_ConversionType_ID(parent.GetC_ConversionType_ID());
        SetDateAcct(parent.GetDateAcct());
		
    }	//	MJournalLine

    /**	Currency Precision		*/
    private int					m_precision = 2;
    /**	Account Combination		*/
    private MAccount		 	m_account = null;
    /** Account Element			*/
    private MElementValue		m_accountElement = null;
	
    /// <summary>
    /// Set Currency Info
    /// </summary>
    /// <param name="C_Currency_ID">currency</param>
    /// <param name="C_ConversionType_ID">type</param>
    /// <param name="CurrencyRate">rate</param>
    public void SetCurrency (int C_Currency_ID, int C_ConversionType_ID, Decimal? CurrencyRate)
    {
        SetC_Currency_ID(C_Currency_ID);
        if (C_ConversionType_ID != 0)
        {
            SetC_ConversionType_ID(C_ConversionType_ID);
        }
        //if (CurrencyRate != null && CurrencyRate.signum() == 0)
        if (CurrencyRate != null && Env.Signum(CurrencyRate.Value) == 0)
        {
            SetCurrencyRate(CurrencyRate);
        }
    }	//	setCurrency

    /// <summary>
    /// Set C_Currency_ID and precision
    /// </summary>
    /// <param name="C_Currency_ID">currency</param>
    public new void SetC_Currency_ID (int C_Currency_ID)
    {
        if (C_Currency_ID == 0)
        {
            return;
        }
        base.SetC_Currency_ID (C_Currency_ID);
        m_precision = MCurrency.GetStdPrecision(GetCtx(), C_Currency_ID);
    }	//	setC_Currency_ID
	
    /// <summary>
    ///	Get Currency Precision
    /// </summary>
    /// <returns></returns>
    public int GetPrecision()
    {
        return m_precision;
    }	//	getPrecision
	
    /// <summary>
    /// Set Currency Rate
     /// </summary>
    /// <param name="CurrencyRate">CurrencyRate check for null (->one)</param>
    public new void SetCurrencyRate (Decimal? CurrencyRate)
    {
        if (CurrencyRate == null)
        {
            log.Warning("was NULL - set to 1");
            base.SetCurrencyRate (Env.ONE);
        }
        //else if (CurrencyRate.signum() < 0)
        else if (Env.Signum(CurrencyRate.Value)< 0)
        {
            log.Warning("negative - " + CurrencyRate + " - set to 1");
            base.SetCurrencyRate (Env.ONE);
        }
        else
        {
            base.SetCurrencyRate (CurrencyRate);
        }
    }	//	setCurrencyRate
	

    /// <summary>
    /// Set Rate - Callout.
    /// </summary>
    /// <param name="oldC_ConversionType_ID">old</param>
    /// <param name="newC_ConversionType_ID">new</param>
    /// <param name="windowNo">window no</param>
     public void SetC_ConversionType_ID (String oldC_ConversionType_ID, 
            String newC_ConversionType_ID, int windowNo)
    {
        if (newC_ConversionType_ID == null || newC_ConversionType_ID.Length == 0)
        {
            return;
        }
        int? C_ConversionType_ID = Utility.Util.GetValueOfInt(newC_ConversionType_ID);
        if (C_ConversionType_ID == 0)
        {
            return;
        }
        SetC_ConversionType_ID(C_ConversionType_ID.Value);
        SetRate(windowNo);
    }	//	setC_ConversionType_ID
	
    /// <summary>
    /// Set Currency - Callout.
    /// </summary>
    /// <param name="oldC_Currency_ID">old</param>
    /// <param name="newC_Currency_ID">new</param>
    /// <param name="windowNo">window no</param>
     public void SetC_Currency_ID (String oldC_Currency_ID, 
            String newC_Currency_ID, int windowNo) 
    {
        if (newC_Currency_ID == null || newC_Currency_ID.Length == 0)
        {
            return;
        }
        int? C_Currency_ID = Utility.Util.GetValueOfInt(newC_Currency_ID);
        if (C_Currency_ID == 0)
        {
            return;
        }
        SetC_Currency_ID(C_Currency_ID.Value);
        SetRate(windowNo);
    }	//	setC_Currency_ID

    /// <summary>
    /// set rate
    /// </summary>
    /// <param name="windowNo"></param>
    private void SetRate(int windowNo)
    {
        //  Source info
        int? C_Currency_ID = GetC_Currency_ID();
        int? C_ConversionType_ID = GetC_ConversionType_ID();
        if (C_Currency_ID == 0 || C_ConversionType_ID == 0)
        {
            return;
        }
        DateTime? DateAcct = GetDateAcct();
        if (DateAcct == null)
        {
            DateAcct =DateTime.Now;// new Timestamp(System.currentTimeMillis());
        }
        //
        int? C_AcctSchema_ID = GetCtx().GetContextAsInt(windowNo, "C_AcctSchema_ID");
        MAcctSchema ass = MAcctSchema.Get (GetCtx(), C_AcctSchema_ID.Value);
        int? AD_Client_ID = GetAD_Client_ID();
        int? AD_Org_ID = GetAD_Org_ID();

        Decimal? CurrencyRate = (Decimal?)MConversionRate.GetRate(C_Currency_ID.Value,  ass.GetC_Currency_ID(), 
            DateAcct, C_ConversionType_ID.Value, AD_Client_ID.Value, AD_Org_ID.Value);
        log.Fine("rate = " + CurrencyRate);
        if (CurrencyRate == null)
        {
            CurrencyRate = Env.ZERO;
        }
        SetCurrencyRate(CurrencyRate);
        SetAmt(windowNo);
    }	//	setRate


    /// <summary>
    /// Set Accounted Amounts only if not 0.Amounts overwritten in beforeSave - set conversion rate
    /// </summary>
    /// <param name="AmtAcctDr">dr</param>
    /// <param name="AmtAcctCr">cr</param>
    public void SetAmtAcct (Decimal AmtAcctDr, Decimal AmtAcctCr)
    {
        //	setConversion
        Double? rateDR = 0;
        if ( Env.Signum(AmtAcctDr) != 0)
        {
            rateDR = Utility.Util.GetValueOfDouble(AmtAcctDr)/ Utility.Util.GetValueOfDouble(GetAmtSourceDr());
            base.SetAmtAcctDr(AmtAcctDr);
            
        }
        Double? rateCR = 0;
        if ( Env.Signum(AmtAcctCr) != 0)
        {
            rateCR = Utility.Util.GetValueOfDouble(AmtAcctCr) / Utility.Util.GetValueOfDouble(GetAmtSourceCr());
            base.SetAmtAcctCr(AmtAcctCr);
        }
        if (rateDR != 0 && rateCR != 0 && rateDR != rateCR)
        {
            log.Warning("Rates Different DR=" + rateDR + "(used) <> CR=" + rateCR + "(ignored)");
            rateCR = 0;
        }
        if (rateDR < 0 || Double.IsInfinity(rateDR.Value) || Double.IsNaN(rateDR.Value))
        { 
            log.Warning("DR Rate ignored - " + rateDR);
            return;
        }
        if (rateCR < 0 || Double.IsInfinity(rateCR.Value) || Double.IsNaN(rateCR.Value))
        {
            log.Warning("CR Rate ignored - " + rateCR);
            return;
        }
		
        if (rateDR != 0)
        {
            SetCurrencyRate(Utility.Util.GetValueOfDecimal(rateDR));
        }
        if (rateCR != 0)
        {
            SetCurrencyRate(Utility.Util.GetValueOfDecimal(rateCR));
        }
    }	//	setAmtAcct

    /// <summary>
    /// Set AmtSourceCr - Callout
    /// </summary>
    /// <param name="oldAmtSourceCr">old value</param>
    /// <param name="newAmtSourceCr">new value</param>
    /// <param name="windowNo">window no</param>
     public void SetAmtSourceCr (String oldAmtSourceCr, 
            String newAmtSourceCr, int windowNo) 
    {
        if (newAmtSourceCr == null || newAmtSourceCr.Length == 0)
        {
            return;
        }
        Decimal AmtSourceCr = Utility.Util.GetValueOfDecimal(newAmtSourceCr);
        base.SetAmtSourceCr(AmtSourceCr);
        SetAmt(windowNo);
    }	//	SetAmtSourceCr
	
    /// <summary>
    ///	Set AmtSourceDr - Callout
    /// </summary>
    /// <param name="oldAmtSourceDr">old</param>
    /// <param name="newAmtSourceDr">new</param>
    /// <param name="windowNo">window no</param>
     public void SetAmtSourceDr (String oldAmtSourceDr, 
            String newAmtSourceDr, int windowNo)
    {
        if (newAmtSourceDr == null || newAmtSourceDr.Length == 0)
        {
            return;
        }
        Decimal? AmtSourceDr =Utility.Util.GetValueOfDecimal(newAmtSourceDr);
        base.SetAmtSourceDr(AmtSourceDr);
        SetAmt(windowNo);
    }	//	setAmtSourceDr
	
    /// <summary>
    /// Set CurrencyRate - Callout
    /// </summary>
    /// <param name="oldCurrencyRate">old</param>
    /// <param name="newCurrencyRate">new</param>
    /// <param name="windowNo">window no</param>
     public void SetCurrencyRate (String oldCurrencyRate, 
            String newCurrencyRate, int windowNo) 
    {
        if (newCurrencyRate == null || newCurrencyRate.Length == 0)
        {
            return;
        }
        Decimal? CurrencyRate = Utility.Util.GetValueOfDecimal(newCurrencyRate);
        base.SetCurrencyRate(CurrencyRate);
        SetAmt(windowNo);
    }	//	setCurrencyRate

	
    /// <summary>
    /// 	Set Accounted Amounts
    /// </summary>
    /// <param name="windowNo">window no</param>
    private void SetAmt(int windowNo)
    {
        //  Get Target Currency & Precision from C_AcctSchema.C_Currency_ID
        int? C_AcctSchema_ID = GetCtx().GetContextAsInt(windowNo, "C_AcctSchema_ID");
        MAcctSchema ass = MAcctSchema.Get(GetCtx(), C_AcctSchema_ID.Value);
        int? Precision = ass.GetStdPrecision();

        Decimal? CurrencyRate = GetCurrencyRate();
        if (CurrencyRate == null)
        {
            CurrencyRate = Env.ONE;
            SetCurrencyRate(CurrencyRate);
        }

        //  AmtAcct = AmtSource * CurrencyRate  ==> Precision
        Decimal? AmtSourceDr = GetAmtSourceDr();
        if (AmtSourceDr == null)
        {
            AmtSourceDr = Env.ZERO;
        }
        Decimal? AmtSourceCr = GetAmtSourceCr();
        if (AmtSourceCr == null)
        {
            AmtSourceCr = Env.ZERO;
        }

        Decimal? AmtAcctDr =(Decimal.Multiply(AmtSourceDr.Value,CurrencyRate.Value));
        //AmtAcctDr = AmtAcctDr.setScale(Precision, BigDecimal.ROUND_HALF_UP);
        AmtAcctDr =Decimal.Round(AmtAcctDr.Value,Precision.Value,MidpointRounding.AwayFromZero);
        SetAmtAcctDr(AmtAcctDr);
        Decimal? AmtAcctCr =Decimal.Multiply(AmtSourceCr.Value,CurrencyRate.Value);
        AmtAcctCr = Decimal.Round(AmtAcctCr.Value, Precision.Value, MidpointRounding.AwayFromZero);
        SetAmtAcctCr(AmtAcctCr);
    }	//	setAmt
	
	
    /// <summary>
    /// Set C_ValidCombination_ID
    /// </summary>
    /// <param name="C_ValidCombination_ID">id</param>
    public new void SetC_ValidCombination_ID (int C_ValidCombination_ID)
    {
        base.SetC_ValidCombination_ID (C_ValidCombination_ID);
        m_account = null;
        m_accountElement = null;
    }	//	setC_ValidCombination_ID
	
    /// <summary>
    ///	Set C_ValidCombination_ID
    /// </summary>
    /// <param name="acct">account</param>
    public void SetC_ValidCombination_ID (MAccount acct)
    {
        if (acct == null)
        {
            throw new ArgumentException("Account is null");
        }
        base.SetC_ValidCombination_ID (acct.GetC_ValidCombination_ID());
        m_account = acct;
        m_accountElement = null;
    }	//	setC_ValidCombination_ID

    /// <summary>
    /// Get Account (Valid Combination)
    /// </summary>
    /// <returns> combination or null</returns>
    public MAccount GetAccount()
    {
        if (m_account == null && GetC_ValidCombination_ID() != 0)
                 m_account = new MAccount (GetCtx(), GetC_ValidCombination_ID(), Get_TrxName());
        return m_account;
    }	//	getValidCombination
	
    /// <summary>
    ///	Get Natural Account Element Value
    /// </summary>
    /// <returns> account</returns>
    public MElementValue GetAccountElementValue()
    {
        if (m_accountElement == null)
        {
            MAccount vc = GetAccount();
            if (vc != null && vc.GetAccount_ID() != 0)
            {
                m_accountElement = new MElementValue(GetCtx(), vc.GetAccount_ID(), Get_TrxName());
            }
        }
        return m_accountElement;
    }	//	getAccountElement
	
    /// <summary>
    /// Is it posting to a Control Acct
    /// </summary>
    /// <returns> true if control acct</returns>
    public Boolean IsDocControlled()
    {
        MElementValue acct = GetAccountElementValue();
        if (acct == null)
        {
            log.Warning ("Account not found for C_ValidCombination_ID=" + GetC_ValidCombination_ID());
            return false;
        }
        return acct.IsDocControlled();
    }	//	isDocControlled
	
	
    /// <summary>
    /// Before Save
    /// </summary>
    /// <param name="newRecord">new</param>
    /// <returns>true</returns>
    protected override Boolean BeforeSave (Boolean newRecord)
    {
        //	Acct Amts
        Decimal? rate = GetCurrencyRate();
        Decimal? amt =Decimal.Multiply(rate.Value,GetAmtSourceDr());
        if (Env.Scale(amt.Value) > GetPrecision())
        {
            amt = Decimal.Round(amt.Value, GetPrecision(), MidpointRounding.AwayFromZero);
        }
        SetAmtAcctDr(amt.Value);
        amt =Decimal.Multiply(rate.Value,GetAmtSourceCr());
        if (Env.Scale(amt.Value) > GetPrecision())
        {
            amt = Decimal.Round(amt.Value, GetPrecision(), MidpointRounding.AwayFromZero);
        }
        SetAmtAcctCr(amt.Value);
        //	Set Line Org to Acct Org
        if (newRecord
            || Is_ValueChanged("C_ValidCombination_ID")
            || Is_ValueChanged("AD_Org_ID"))
        {
            SetAD_Org_ID(GetAccount().GetAD_Org_ID());
        }
        return true;
    }	//	beforeSave
	
    /// <summary>
    /// After Save.	Update Journal/Batch Total
    /// </summary>
    /// <param name="newRecord">true if new record</param>
    /// <param name="success"> true if success</param>
    /// <returns>success</returns>
    protected override Boolean AfterSave (Boolean newRecord, Boolean success)
    {
        if (!success)
        {
            return success;
        }
        return UpdateJournalTotal();
    }	//	afterSave
	
	
    /// <summary>
    /// After Delete
     /// </summary>
    /// <param name="success">true if deleted</param>
    /// <returns>true if success</returns>
    protected override Boolean AfterDelete (Boolean success)
    {
        if (!success)
        {
            return success;
        }
        return UpdateJournalTotal();
    }	//	afterDelete

	
    /// <summary>
    ///	Update Journal and Batch Total
    /// </summary>
    /// <returns>true if success</returns>
    private Boolean UpdateJournalTotal()
    {
        //	Update Journal Total
        String sql = "UPDATE GL_Journal j"
            + " SET (TotalDr, TotalCr) = (SELECT COALESCE(SUM(AmtSourceDr),0), COALESCE(SUM(AmtSourceCr),0)" //jz ", "
                + " FROM GL_JournalLine jl WHERE jl.IsActive='Y' AND j.GL_Journal_ID=jl.GL_Journal_ID) "
            + "WHERE GL_Journal_ID=" + GetGL_Journal_ID();
        int no = DataBase.DB.ExecuteQuery(sql,null, Get_TrxName());
        if (no != 1)
        {
            log.Warning("afterSave - Update Journal #" + no);
        }
		
        //	Update Batch Total
        sql = "UPDATE GL_JournalBatch jb"
            + " SET (TotalDr, TotalCr) = (SELECT COALESCE(SUM(TotalDr),0), COALESCE(SUM(TotalCr),0)" //jz hard coded ", "
                + " FROM GL_Journal j WHERE jb.GL_JournalBatch_ID=j.GL_JournalBatch_ID) "
            + "WHERE GL_JournalBatch_ID="
                + "(SELECT DISTINCT GL_JournalBatch_ID FROM GL_Journal WHERE GL_Journal_ID=" 
                + GetGL_Journal_ID() + ")";
        no = DataBase.DB.ExecuteQuery(sql,null, Get_TrxName());
        if (no != 1)
        {
            log.Warning("Update Batch #" + no);
        }
        return no == 1;
    }	//	updateJournalTotal
	
}	//	MJournalLine

}
