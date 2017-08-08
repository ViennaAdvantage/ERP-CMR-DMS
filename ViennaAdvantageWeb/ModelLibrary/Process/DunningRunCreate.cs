/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DunningRunCreate
 * Purpose        : Create Dunning Run Entries/Lines
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak          10-Nov-2009
  ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class DunningRunCreate : ProcessEngine.SvrProcess
    {
        #region Private Variable
        private Boolean _IncludeInDispute = false;
        private Boolean _OnlySOTrx = false;
        private Boolean _IsAllCurrencies = false;
        private int _SalesRep_ID = 0;
        private int _C_Currency_ID = 0;
        private int _C_BPartner_ID = 0;
        private int _C_BP_Group_ID = 0;
        private int _C_DunningRun_ID = 0;
        private MDunningRun _run = null;
        private MDunningLevel _level = null;
        #endregion

        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("IncludeInDispute"))
                {
                    _IncludeInDispute = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("OnlySOTrx"))
                {
                    _OnlySOTrx = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("IsAllCurrencies"))
                {
                    _IsAllCurrencies = "Y".Equals(para[i].GetParameter());
                }
                else if (name.Equals("SalesRep_ID"))
                {
                    _SalesRep_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_Currency_ID"))
                {
                    _C_Currency_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("C_BP_Group_ID"))
                {
                    _C_BP_Group_ID = para[i].GetParameterAsInt();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _C_DunningRun_ID = GetRecord_ID();
        }

        /// <summary>
        /// 	Process
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("C_DunningRun_ID=" + _C_DunningRun_ID
                + ", Dispute=" + _IncludeInDispute
                + ", C_BP_Group_ID=" + _C_BP_Group_ID
                + ", C_BPartner_ID=" + _C_BPartner_ID);
            _run = new MDunningRun(GetCtx(), _C_DunningRun_ID, Get_TrxName());
            if (_run.Get_ID() == 0)
            {
                throw new ArgumentException("Not found MDunningRun");
            }
            if (!_run.DeleteEntries(true))
            {
                throw new ArgumentException("Cannot delete existing entries");
            }
            if (_SalesRep_ID == 0)
            {
                throw new ArgumentException("No SalesRep");
            }
            if (_C_Currency_ID == 0)
            {
                throw new ArgumentException("No Currency");
            }

            // Pickup the Runlevel
            _level = _run.GetLevel();

            // add up all invoices
            int inv = AddInvoices();
            // add up all payments
            int pay = AddPayments();

            // If the level should charge a fee do it now...
            if (_level.IsChargeFee())
            {
                AddFees();
            }
            if (_level.IsChargeInterest())
            {
                AddFees();
            }

            // we need to check whether this is a statement or not and some other rules
            CheckDunningEntry();

            int entries = 0;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader("SELECT count(*) FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID(), null, Get_TrxName());
                if (idr.Read())
                {
                    entries = Utility.Util.GetValueOfInt(idr[0]);//.getInt(1);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "countResults", e);
            }
            
            return "@C_DunningRunEntry_ID@ #" + entries;
        }


        /// <summary>
        /// Add Invoices to Run
        /// </summary>
        /// <returns>no of invoices</returns>
        private int AddInvoices()
        {
            int size = 5;
            int count = 0;
            String sql = "SELECT i.C_Invoice_ID, i.C_Currency_ID,"
                + " i.GrandTotal*i.MultiplierAP,"
                + " invoiceOpen(i.C_Invoice_ID,i.C_InvoicePaySchedule_ID)*MultiplierAP,"
                + " COALESCE(daysBetween(@param1,ips.DueDate),paymentTermDueDays(i.C_PaymentTerm_ID,i.DateInvoiced,@param2))," // ##1/2
                + " i.IsInDispute, i.C_BPartner_ID "
                + "FROM C_Invoice_v i "
                + " LEFT OUTER JOIN C_InvoicePaySchedule ips ON (i.C_InvoicePaySchedule_ID=ips.C_InvoicePaySchedule_ID) "
                + "WHERE i.IsPaid='N' AND i.AD_Client_ID=@param3"				//	##3
                + " AND i.DocStatus IN ('CO','CL')"
                //  Invoice Collection Status Collection Agency, Uncollectable, Legal will not been dunned any longer as per Def. YS + KP 12/02/06
                + " AND (NOT i.InvoiceCollectionType IN ('" + X_C_Invoice.INVOICECOLLECTIONTYPE_CollectionAgency + "', "
                    + "'" + X_C_Invoice.INVOICECOLLECTIONTYPE_LegalProcedure + "', '" + X_C_Invoice.INVOICECOLLECTIONTYPE_Uncollectable + "')"
                    + " OR InvoiceCollectionType IS NULL)"
                //  Do not show future docs...
                + " AND DateInvoiced<=@param4" // ##4
                //	Only BP(Group) with Dunning defined
                + " AND EXISTS (SELECT * FROM C_DunningLevel dl "
                    + "WHERE dl.C_DunningLevel_ID=@param5"	//	//	##5
                    + " AND dl.C_Dunning_ID IN "
                        + "(SELECT COALESCE(bp.C_Dunning_ID, bpg.C_Dunning_ID) "
                        + "FROM C_BPartner bp"
                        + " INNER JOIN C_BP_Group bpg ON (bp.C_BP_Group_ID=bpg.C_BP_Group_ID) "
                        + "WHERE i.C_BPartner_ID=bp.C_BPartner_ID))";
            // for specific Business Partner
            if (_C_BPartner_ID != 0)
            {
                size = size + 1;
                sql += " AND i.C_BPartner_ID=@param6";	//	##6
            }
            // or a specific group
            else if (_C_BP_Group_ID != 0)
            {
                size = size + 1;
                sql += " AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE i.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=@param6)";	//	##6
            }
            // Only Sales Trx
            if (_OnlySOTrx)
            {
                sql += " AND i.IsSOTrx='Y'";
            }
            // Only single currency
            if (!_IsAllCurrencies)
            {
                sql += " AND i.C_Currency_ID=" + _C_Currency_ID;
            }
            //	log.info(sql);

            String sql2 = null;

            // if sequentially we must check for other levels with smaller days for
            // which this invoice is not yet included!
            if (_level.GetParent().IsCreateLevelsSequentially())
            {
                // Build a list of all topmost Dunning Levels
                MDunningLevel[] previousLevels = _level.GetPreviousLevels();
                if (previousLevels != null && previousLevels.Length > 0)
                {
                    String sqlAppend = "";
                    for (int i = 0; i < previousLevels.Length; i++)
                    {
                        sqlAppend += " AND i.C_Invoice_ID IN (SELECT C_Invoice_ID FROM C_DunningRunLine WHERE " +
                        "C_DunningRunEntry_ID IN (SELECT C_DunningRunEntry_ID FROM C_DunningRunEntry WHERE " +
                        "C_DunningRun_ID IN (SELECT C_DunningRun_ID FROM C_DunningRun WHERE " +
                        "C_DunningLevel_ID=" + previousLevels[i].Get_ID() + ")) AND Processed<>'N')";
                    }
                    sql += sqlAppend;
                }
            }
            // ensure that we do only dunn what's not yet dunned, so we lookup the max of last Dunn Date which was processed
            sql2 = "SELECT COUNT(*), COALESCE(DAYSBETWEEN(MAX(dr2.DunningDate), MAX(dr.DunningDate)),0)"
                + "FROM C_DunningRun dr2, C_DunningRun dr"
                + " INNER JOIN C_DunningRunEntry dre ON (dr.C_DunningRun_ID=dre.C_DunningRun_ID)"
                + " INNER JOIN C_DunningRunLine drl ON (dre.C_DunningRunEntry_ID=drl.C_DunningRunEntry_ID) "
                + "WHERE drl.Processed='Y' AND dr2.C_DunningRun_ID=@C_DunningRun_ID AND drl.C_Invoice_ID=@C_Invoice_ID"; // ##1 ##2

            Decimal DaysAfterDue = _run.GetLevel().GetDaysAfterDue();
            int DaysBetweenDunning = _run.GetLevel().GetDaysBetweenDunning();

            IDataReader idr = null;
            IDataReader idr1 = null;
            try
            {
                SqlParameter[] param = new SqlParameter[size];
                param[0] = new SqlParameter("@param1", _run.GetDunningDate());
                param[1] = new SqlParameter("@param2", _run.GetDunningDate());
                param[2] = new SqlParameter("@param3", _run.GetAD_Client_ID());
                param[3] = new SqlParameter("@param4", _run.GetDunningDate());
                param[4] = new SqlParameter("@param5", _run.GetC_DunningLevel_ID());
                if (_C_BPartner_ID != 0)
                {
                    
                    //pstmt.setInt(6, _C_BPartner_ID);
                    param[5] = new SqlParameter("@param6", _C_BPartner_ID);
                }
                else if (_C_BP_Group_ID != 0)
                {
                    //pstmt.setInt(6, _C_BP_Group_ID);
                    param[5] = new SqlParameter("@param6", _C_BP_Group_ID);
                }

                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                //
                while (idr.Read())
                {
                    int C_Invoice_ID = Utility.Util.GetValueOfInt(idr[0]);
                    int C_Currency_ID = Utility.Util.GetValueOfInt(idr[1]);
                    Decimal GrandTotal = Utility.Util.GetValueOfDecimal(idr[2]);
                    Decimal Open = Utility.Util.GetValueOfDecimal(idr[3]);
                    int DaysDue = Utility.Util.GetValueOfInt(idr[4]);
                    bool IsInDispute = "Y".Equals(Utility.Util.GetValueOfString(idr[5]));
                    int C_BPartner_ID = Utility.Util.GetValueOfInt(idr[6]);
                    //
                    // Check for Dispute
                    if (!_IncludeInDispute && IsInDispute)
                    {
                        continue;
                    }
                    // Check the day again based on rulesets
                    if (DaysDue < Utility.Util.GetValueOfInt(DaysAfterDue) && !_level.IsShowAllDue())
                    {
                        continue;
                    }
                    // Check for an open amount
                    if (Env.ZERO.CompareTo(Open) == 0)
                    {
                        continue;
                    }
                    //
                    int timesDunned = 0;
                    int daysAfterLast = 0;
                    
                    //2nd record set
                    SqlParameter[] param1 = new SqlParameter[2];
                    param1[0] = new SqlParameter("@C_DunningRun_ID", _run.Get_ID());
                    param1[1] = new SqlParameter("@C_Invoice_ID", C_Invoice_ID);
                    idr1 = DataBase.DB.ExecuteReader(sql2, param1, Get_TrxName());
                    //	SubQuery
                    if (idr1.Read())
                    {
                        timesDunned = Utility.Util.GetValueOfInt(idr1[0]);
                        daysAfterLast = Utility.Util.GetValueOfInt(idr1[1]);
                    }
                    idr1.Close();
                    //	SubQuery

                    // Ensure that Daysbetween Dunning is enforced
                    // Ensure Not ShowAllDue and Not ShowNotDue is selected
                    // PROBLEM: If you have ShowAll activated then DaysBetweenDunning is not working, because we don't know whether
                    //          there is something which we really must Dunn.
                    if (DaysBetweenDunning != 0 && daysAfterLast < DaysBetweenDunning && !_level.IsShowAllDue() && !_level.IsShowNotDue())
                    {
                        continue;
                    }

                    // We don't want to show non due documents
                    if (DaysDue < 0 && !_level.IsShowNotDue())
                    {
                        continue;
                    }

                    // We will minus the timesDunned if this is the DaysBetweenDunning is not fullfilled.
                    // Remember in checkup later we must reset them!
                    // See also checkDunningEntry()
                    if (daysAfterLast < DaysBetweenDunning)
                    {
                        timesDunned = timesDunned * -1;
                    }
                    //
                    CreateInvoiceLine(C_Invoice_ID, C_Currency_ID, GrandTotal, Open,
                        DaysDue, IsInDispute, C_BPartner_ID,
                        timesDunned, daysAfterLast);
                    count++;
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (idr1 != null)
                {
                    idr1.Close();
                }
                log.Log(Level.SEVERE, "addInvoices", e);
            }
            
            return count;
        }

        /// <summary>
        /// Create Invoice Line
        /// </summary>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="GrandTotal">total</param>
        /// <param name="Open">amount</param>
        /// <param name="DaysDue">days due</param>
        /// <param name="IsInDispute">in dispute</param>
        /// <param name="C_BPartner_ID">bp</param>
        /// <param name="timesDunned">number of dunnings</param>
        /// <param name="daysAfterLast">days after last dunning</param>
        private void CreateInvoiceLine(int C_Invoice_ID, int C_Currency_ID,
            Decimal GrandTotal, Decimal Open,
            int DaysDue, bool IsInDispute,
            int C_BPartner_ID, int timesDunned, int daysAfterLast)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {
                    throw new Exception("Cannot save MDunningRunEntry");
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetInvoice(C_Invoice_ID, C_Currency_ID, GrandTotal, Open,
                new Decimal(0), DaysDue, IsInDispute, timesDunned,
                daysAfterLast);
            if (!line.Save())
            {
                throw new Exception("Cannot save MDunningRunLine");
            }
        }


        /// <summary>
        /// Add Payments to Run
        /// </summary>
        /// <returns>no of payments</returns>
        private int AddPayments()
        {
            String sql = "SELECT C_Payment_ID, C_Currency_ID, PayAmt,"
                + " paymentAvailable(C_Payment_ID), C_BPartner_ID "
                + "FROM C_Payment_v p "
                + "WHERE AD_Client_ID=" + GetAD_Client_ID()			//	##1
                + " AND IsAllocated='N' AND C_BPartner_ID IS NOT NULL"
                + " AND C_Charge_ID IS NULL"
                + " AND DocStatus IN ('CO','CL')"
                //	Only BP with Dunning defined
                + " AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE p.C_BPartner_ID=bp.C_BPartner_ID"
                    + " AND bp.C_Dunning_ID=(SELECT C_Dunning_ID FROM C_DunningLevel WHERE C_DunningLevel_ID=" + _run.GetC_DunningLevel_ID() + "))";	// ##2
            if (_C_BPartner_ID != 0)
            {
                sql += " AND C_BPartner_ID=" + _C_BPartner_ID;		//	##3
            }
            else if (_C_BP_Group_ID != 0)
            {
                sql += " AND EXISTS (SELECT * FROM C_BPartner bp "
                    + "WHERE p.C_BPartner_ID=bp.C_BPartner_ID AND bp.C_BP_Group_ID=" + _C_BP_Group_ID + ")";	//	##3
            }
            // If it is not a statement we will add lines only if InvoiceLines exists,
            // because we do not want to dunn for money we owe the customer!
            if (!_level.GetDaysAfterDue().Equals(new Decimal(-9999)))
            {
                sql += " AND C_BPartner_ID IN (SELECT C_BPartner_ID FROM C_DunningRunEntry WHERE C_DunningRun_ID=" + _run.Get_ID() + ")";
            }
            // show only receipts / if only Sales
            if (_OnlySOTrx)
            {
                sql += " AND IsReceipt='Y'";
            }

            int count = 0;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());

                while (idr.Read())
                {
                    int C_Payment_ID = Utility.Util.GetValueOfInt(idr[0]);
                    int C_Currency_ID = Utility.Util.GetValueOfInt(idr[1]);
                    Decimal PayAmt = Decimal.Negate(Utility.Util.GetValueOfDecimal(idr[2]));//.getBigDecimal(3).negate();
                    Decimal openAmt = Decimal.Negate(Utility.Util.GetValueOfDecimal(idr[3]));// rs.getBigDecimal(4).negate();
                    int C_BPartner_ID = Utility.Util.GetValueOfInt(idr[4]);//.getInt(5);

                    // checkup the amount
                    if (Env.ZERO.CompareTo(openAmt) == 0)
                    {
                        continue;
                    }
                    //
                    CreatePaymentLine(C_Payment_ID, C_Currency_ID, PayAmt, openAmt,
                        C_BPartner_ID);
                    count++;
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
           
            return count;
        }

        /// <summary>
        /// Create Payment Line
        /// </summary>
        /// <param name="C_Payment_ID">payment</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="PayAmt">amount</param>
        /// <param name="openAmt">open</param>
        /// <param name="C_BPartner_ID">bp</param>
        private void CreatePaymentLine(int C_Payment_ID, int C_Currency_ID,
            Decimal PayAmt, Decimal openAmt, int C_BPartner_ID)
        {
            MDunningRunEntry entry = _run.GetEntry(C_BPartner_ID, _C_Currency_ID, _SalesRep_ID);
            if (entry.Get_ID() == 0)
            {
                if (!entry.Save())
                {
                    throw new Exception("Cannot save MDunningRunEntry");
                }
            }
            //
            MDunningRunLine line = new MDunningRunLine(entry);
            line.SetPayment(C_Payment_ID, C_Currency_ID, PayAmt, openAmt);
            if (!line.Save())
            {
                throw new Exception("Cannot save MDunningRunLine");
            }
        }

        /// <summary>
        /// Add Fees for every line
        /// </summary>
        private void AddFees()
        {
            // Only add a fee if it contains InvoiceLines and is not a statement
            // TODO: Assumes Statement = -9999 and 
            bool onlyInvoices = _level.GetDaysAfterDue().Equals(new Decimal(-9999));
            MDunningRunEntry[] entries = _run.GetEntries(true, onlyInvoices);
            if (entries != null && entries.Length > 0)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    MDunningRunLine line = new MDunningRunLine(entries[i]);
                    line.SetFee(_C_Currency_ID, _level.GetFeeAmt());
                    if (!line.Save())
                    {
                        throw new Exception("Cannot save MDunningRunLine");
                    }
                    entries[i].SetQty(Decimal.Subtract(entries[i].GetQty(), new Decimal(1)));
                }
            }
        }

        /// <summary>
        /// Check the dunning run
        /// 1) Check for following Rule: ShowAll should produce only a record if at least one new line is found
        /// </summary>
        private void CheckDunningEntry()
        {
            // Check rule 1)
            if (_level.IsShowAllDue())
            {
                MDunningRunEntry[] entries = _run.GetEntries(true);
                if (entries != null && entries.Length > 0)
                {
                    for (int i = 0; i < entries.Length; i++)
                    {
                        // We start with saying we delete this entry as long as we don't find something new
                        bool entryDelete = true;
                        MDunningRunLine[] lines = entries[i].GetLines(true);
                        for (int j = 0; j < lines.Length; j++)
                        {
                            if (lines[j].GetTimesDunned() < 0)
                            {
                                // We clean up the *-1 from line 255
                                lines[j].SetTimesDunned(lines[j].GetTimesDunned() * -1);
                                if (!lines[j].Save())
                                {
                                    throw new Exception("Cannot save MDunningRunLine");
                                }
                            }

                            else
                            {
                                // We found something new, so we would not save anything...
                                entryDelete = false;
                            }
                        }
                        if (entryDelete)
                        {
                            entries[i].Delete(false);
                        }
                    }
                }
            }
        }

    }

}
