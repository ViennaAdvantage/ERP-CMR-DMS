using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using System.Data;
using VAdvantage.Model;

namespace VAdvantage.Process
{
    class ProfitLossClosing : SvrProcess
    {
        string qry = "";
        DateTime? stDate,eDate;
        Decimal expense = 0, revenue = 0;
        DataSet ds = null;
        MProfitLoss prof = null;
        MProfitLossLines pLine = null;
        protected override string DoIt()
        {
            DB.ExecuteQuery("DELETE FROM C_ProfitLossLines WHERE C_ProfitLoss_ID=" + GetRecord_ID());
            prof = new MProfitLoss(GetCtx(), GetRecord_ID(), Get_TrxName());            

            stDate = Util.GetValueOfDateTime(DB.ExecuteScalar("select p.startdate from c_period p  inner join c_year y on p.c_year_id=y.c_year_id where p.periodno='1' and p.c_year_id= "+ prof.GetC_Year_ID() +" and y.ad_client_id= "+GetCtx().GetAD_Client_ID(), null, null));                        
            eDate = Util.GetValueOfDateTime(DB.ExecuteScalar("select p.enddate from c_period p  inner join c_year y on p.c_year_id=y.c_year_id where p.periodno='12' and p.c_year_id= " + prof.GetC_Year_ID() + " and y.ad_client_id= " + GetCtx().GetAD_Client_ID(), null, null));

            qry = @"select ft.C_AcctSchema_ID,ft.PostingType,ft.AmtAcctDr,ft.AmtAcctCr,ft.Account_ID,ft.C_SubAcct_ID,ft.C_BPartner_ID,ft.M_Product_ID,ft.C_Project_ID,ft.C_SalesRegion_ID,ft.C_Campaign_ID,ft.AD_OrgTrx_ID,ft.C_LocFrom_ID,ft.C_LocTo_ID,ft.C_Activity_ID,ft.User1_ID,ft.User2_ID,ft.UserElement1_ID,ft.UserElement2_ID,ft.GL_Budget_ID,ft.C_ProjectPhase_ID,ft.C_ProjectTask_ID," +
            "ev.Value as LedgerCode,ev.Name as LedgerName from fact_acct_balance ft inner join c_elementvalue ev on ft.account_id=ev.c_elementvalue_id where ft.ad_client_id=" + GetAD_Client_ID() + " and ft.DateAcct >=" + GlobalVariable.TO_DATE(stDate, true) + " AND ft.DateAcct <= " + GlobalVariable.TO_DATE(eDate, true) + " AND (ev.accounttype='E' OR ev.accounttype='R') and ev.isintermediatecode='N'";            
            ds = DB.ExecuteDataset(qry, null, Get_TrxName());
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        pLine = new MProfitLossLines(GetCtx(), 0, Get_TrxName());
                        pLine.SetAD_Client_ID(GetAD_Client_ID());
                        pLine.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                        pLine.SetC_ProfitLoss_ID(GetRecord_ID());
                        pLine.SetC_ProfitAndLoss_ID(prof.GetC_ProfitAndLoss_ID());
                        pLine.SetC_AcctSchema_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_AcctSchema_ID"]));
                        pLine.SetPostingType(Util.GetValueOfString(ds.Tables[0].Rows[i]["PostingType"]));
                        pLine.SetLedgerCode(Util.GetValueOfString(ds.Tables[0].Rows[i]["LedgerCode"]));
                        pLine.SetLedgerName(Util.GetValueOfString(ds.Tables[0].Rows[i]["LedgerName"]));
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AmtAcctDr"]) > 0)
                        {
                            pLine.SetAccountCredit(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AmtAcctDr"]));
                        }
                        if (Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AmtAcctCr"]) > 0)
                        {
                            pLine.SetAccountDebit(Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["AmtAcctCr"]));
                        }
                        pLine.SetC_SubAcct_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_SubAcct_ID"]));
                        pLine.SetAccount_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["Account_ID"]));
                        pLine.SetC_BPartner_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_BPartner_ID"]));
                        pLine.SetM_Product_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]));
                        pLine.SetC_Project_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Project_ID"]));
                        pLine.SetC_SalesRegion_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_SalesRegion_ID"]));
                        pLine.SetC_Campaign_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Campaign_ID"]));
                        pLine.SetAD_OrgTrx_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_OrgTrx_ID"]));
                        pLine.SetC_LocFrom_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_LocFrom_ID"]));
                        pLine.SetC_LocTo_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_LocTo_ID"]));
                        pLine.SetC_Activity_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_Activity_ID"]));
                        pLine.SetUser1_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["User1_ID"]));
                        pLine.SetUser2_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["User2_ID"]));
                        pLine.SetUserElement1_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["UserElement1_ID"]));
                        pLine.SetUserElement2_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["UserElement2_ID"]));
                        pLine.SetGL_Budget_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["GL_Budget_ID"]));
                        pLine.SetC_ProjectPhase_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectPhase_ID"]));
                        pLine.SetC_ProjectTask_ID(Util.GetValueOfInt(ds.Tables[0].Rows[i]["C_ProjectTask_ID"]));
                        int lineno = Util.GetValueOfInt(DB.ExecuteScalar("SELECT NVL(MAX(Line),0)+10 FROM C_ProfitLossLines WHERE C_ProfitLoss_ID=" + GetRecord_ID(),null,Get_TrxName()));
                        pLine.SetLine(lineno);
                        if (!pLine.Save())
                        {
                            ds.Dispose();
                            return Msg.GetMsg(GetCtx(), "ProfitLinesNotSaved");
                        }
                    }

                    expense = Util.GetValueOfDecimal(DB.ExecuteScalar("select SUM(ft.AmtAcctCr+ft.AmtAcctDr) from fact_acct_balance ft inner join c_elementvalue ev on ft.account_id=ev.c_elementvalue_id where ft.ad_client_id=" + GetAD_Client_ID() + " and ft.DateAcct >=" + GlobalVariable.TO_DATE(stDate, true) + " AND ft.DateAcct <= " + GlobalVariable.TO_DATE(eDate, true) + " AND ev.accounttype='E' and ev.isintermediatecode='N'"));
                    revenue = Util.GetValueOfDecimal(DB.ExecuteScalar("select SUM(ft.AmtAcctCr+ft.AmtAcctDr) from fact_acct_balance ft inner join c_elementvalue ev on ft.account_id=ev.c_elementvalue_id where ft.ad_client_id=" + GetAD_Client_ID() + " and ft.DateAcct >=" + GlobalVariable.TO_DATE(stDate, true) + " AND ft.DateAcct <= " + GlobalVariable.TO_DATE(eDate, true) + " AND ev.accounttype='R' and ev.isintermediatecode='N'"));
                    prof.SetProfitBeforeTax(revenue - expense);
                    if (!prof.Save())
                    {
                        ds.Dispose();
                        Rollback();
                        return Msg.GetMsg(GetCtx(), "ProfitNotSaved");
                    }
                    ds.Dispose();
                    return Msg.GetMsg(GetCtx(), "LinesGenerated");
                }
            }
            return Msg.GetMsg(GetCtx(), "RecordNoFound"); 
            
        }

        protected override void Prepare()
        {
            
        }
    }
}
