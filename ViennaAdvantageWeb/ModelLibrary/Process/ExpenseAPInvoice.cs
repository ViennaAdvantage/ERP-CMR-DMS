/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ExpenseAPInvoice
 * Purpose        : Create AP Invoices from Expense Reports
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           2-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;
namespace VAdvantage.Process
{
    public class ExpenseAPInvoice : ProcessEngine.SvrProcess
    {
        private int _C_BPartner_ID = 0;
        private DateTime? _DateFrom = null;
        private DateTime? _DateTo = null;
        private int _noInvoices = 0;

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
                else if (name.Equals("C_BPartner_ID"))
                {
                    _C_BPartner_ID = para[i].GetParameterAsInt();
                }
                else if (name.Equals("DateReport"))
                {
                    _DateFrom = (DateTime?)para[i].GetParameter();
                    _DateTo = (DateTime?)para[i].GetParameter_To();
                }
                else
                {
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
        }	//	prepare


        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            int index = 1;
            StringBuilder sql = new StringBuilder("SELECT * "
                + "FROM S_TimeExpense e "
                + "WHERE e.Processed='Y'"
                + " AND e.AD_Client_ID=@param1");				//	#1
            if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
            {
                index++;
                sql.Append(" AND e.C_BPartner_ID=@param2");	//	#2
            }
            if (_DateFrom != null)
            {
                index++;
                sql.Append(" AND e.DateReport >=@param3");	//	#3
            }
            if (_DateTo != null)
            {
                index++;
                sql.Append(" AND e.DateReport <=@param4");	//	#4
                sql.Append(" AND EXISTS (SELECT * FROM S_TimeExpenseLine el "
                    + "WHERE e.S_TimeExpense_ID=el.S_TimeExpense_ID"
                    + " AND el.C_InvoiceLine_ID IS NULL"
                    + " AND el.ConvertedAmt<>0) "
                    + "ORDER BY e.C_BPartner_ID, e.S_TimeExpense_ID");
            }
            //
            int old_BPartner_ID = -1;
            MInvoice invoice = null;
            //
            //PreparedStatement pstmt = null;
            SqlParameter[] param = new SqlParameter[index];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql.toString (), get_TrxName());
                int par = 0;
                //pstmt.setInt(par++, getAD_Client_ID());
                param[0] = new SqlParameter("@param1", GetAD_Client_ID());
                if (_C_BPartner_ID != 0 && _C_BPartner_ID != -1)
                {
                    par++;
                    //pstmt.setInt (par++, _C_BPartner_ID);
                    param[par] = new SqlParameter("@param2", _C_BPartner_ID);
                }
                if (_DateFrom != null)
                {
                    par++;
                    //pstmt.setTimestamp (par++, _DateFrom);
                    param[par] = new SqlParameter("@param3", _DateFrom);
                }
                if (_DateTo != null)
                {
                    par++;
                    //pstmt.setTimestamp (par++, _DateTo);
                    param[par] = new SqlParameter("@param4", _DateTo);
                }
                //ResultSet rs = pstmt.executeQuery ();
                idr = DataBase.DB.ExecuteReader(sql.ToString(), param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                //	********* Expense Line Loop
                {
                    MTimeExpense te = new MTimeExpense(GetCtx(), dr, Get_TrxName());

                    //	New BPartner - New Order
                    if (te.GetC_BPartner_ID() != old_BPartner_ID)
                    {
                        CompleteInvoice(invoice);
                        MBPartner bp = new MBPartner(GetCtx(), te.GetC_BPartner_ID(), Get_TrxName());
                        //
                        log.Info("New Invoice for " + bp);
                        invoice = new MInvoice(GetCtx(), 0, null);
                        invoice.SetClientOrg(te.GetAD_Client_ID(), te.GetAD_Org_ID());
                        invoice.SetC_DocTypeTarget_ID(MDocBaseType.DOCBASETYPE_APINVOICE);	//	API
                        invoice.SetDocumentNo(te.GetDocumentNo());
                        //
                        invoice.SetBPartner(bp);
                        if (invoice.GetC_BPartner_Location_ID() == 0)
                        {
                            log.Log(Level.SEVERE, "No BP Location: " + bp);
                            AddLog(0, te.GetDateReport(),
                                null, "No Location: " + te.GetDocumentNo() + " " + bp.GetName());
                            invoice = null;
                            break;
                        }
                        invoice.SetM_PriceList_ID(te.GetM_PriceList_ID());
                        invoice.SetSalesRep_ID(te.GetDoc_User_ID());
                        String descr = Msg.Translate(GetCtx(), "S_TimeExpense_ID")
                            + ": " + te.GetDocumentNo() + " "
                            + DisplayType.GetDateFormat(DisplayType.Date).Format(te.GetDateReport());
                        invoice.SetDescription(descr);
                        if (!invoice.Save())
                        {
                            new Exception("Cannot save Invoice");
                        }
                        old_BPartner_ID = bp.GetC_BPartner_ID();
                    }
                    MTimeExpenseLine[] tel = te.GetLines(false);
                    for (int i = 0; i < tel.Length; i++)
                    {
                        MTimeExpenseLine line = tel[i];

                        //	Already Invoiced or nothing to be reimbursed
                        if (line.GetC_InvoiceLine_ID() != 0
                            || Env.ZERO.CompareTo(line.GetQtyReimbursed()) == 0
                            || Env.ZERO.CompareTo(line.GetPriceReimbursed()) == 0)
                        {
                            continue;
                        }
                        //	Update Header info
                        if (line.GetC_Activity_ID() != 0 && line.GetC_Activity_ID() != invoice.GetC_Activity_ID())
                        {
                            invoice.SetC_Activity_ID(line.GetC_Activity_ID());
                        }
                        if (line.GetC_Campaign_ID() != 0 && line.GetC_Campaign_ID() != invoice.GetC_Campaign_ID())
                        {
                            invoice.SetC_Campaign_ID(line.GetC_Campaign_ID());
                        }
                        if (line.GetC_Project_ID() != 0 && line.GetC_Project_ID() != invoice.GetC_Project_ID())
                        {
                            invoice.SetC_Project_ID(line.GetC_Project_ID());
                        }
                        if (!invoice.Save())
                        {
                            new Exception("Cannot save Invoice");
                        }

                        //	Create OrderLine
                        MInvoiceLine il = new MInvoiceLine(invoice);
                        //
                        if (line.GetM_Product_ID() != 0)
                        {
                            il.SetM_Product_ID(line.GetM_Product_ID(), true);
                        }
                        il.SetQty(line.GetQtyReimbursed());		//	Entered/Invoiced
                        il.SetDescription(line.GetDescription());
                        //
                        il.SetC_Project_ID(line.GetC_Project_ID());
                        il.SetC_ProjectPhase_ID(line.GetC_ProjectPhase_ID());
                        il.SetC_ProjectTask_ID(line.GetC_ProjectTask_ID());
                        il.SetC_Activity_ID(line.GetC_Activity_ID());
                        il.SetC_Campaign_ID(line.GetC_Campaign_ID());
                        //
                        //	il.setPrice();	//	not really a list/limit price for reimbursements
                        il.SetPrice(line.GetPriceReimbursed());	//
                        il.SetTax();
                        if (!il.Save())
                        {
                            new Exception("Cannot save Invoice Line");
                        }
                        //	Update TEL
                        line.SetC_InvoiceLine_ID(il.GetC_InvoiceLine_ID());
                        line.SetIsInvoiced(true);
                        line.Save();
                    }	//	for all expense lines
                }								//	********* Expense Line Loop
                dt = null;
                //dt.Clear();
            }
            catch (Exception e)
            {
                if (dt != null)
                {
                    dt = null;
                }
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql.ToString(), e);
            }
            finally
            {
                if (dt != null)
                {
                    dt = null;
                }
                if (idr != null)
                {
                    idr.Close();
                }
            }
            CompleteInvoice(invoice);
            if (_noInvoices == 0)
            {
                return " @No Record Found for Invoice  Creation@";
            }
            return "" + _noInvoices + " @Invoices Generated Successfully@";
        }	//	doIt

        /// <summary>
        /// Complete Invoice
        /// </summary>
        /// <param name="invoice">invoice</param>
        private void CompleteInvoice(MInvoice invoice)
        {
            if (invoice == null)
            {
                return;
            }
            invoice.SetDocAction(DocActionVariables.ACTION_PREPARE);
            invoice.ProcessIt(DocActionVariables.ACTION_PREPARE);
            if (!invoice.Save())
            {
                new Exception("Cannot save Invoice");
            }
            //
            _noInvoices++;
            AddLog(invoice.Get_ID(), invoice.GetDateInvoiced(),
                invoice.GetGrandTotal(), invoice.GetDocumentNo());
        }	//	completeInvoice

    }	//	ExpenseAPInvoice

}
