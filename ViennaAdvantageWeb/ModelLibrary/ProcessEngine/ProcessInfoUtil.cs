/********************************************************
 * Module Name    : Process
 * Purpose        : Execute the process
 * Author         : Jagmohan Bhatt
 * Date           : 3-may-2009
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using System.Threading;
using System.Data;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Logging;
using VAdvantage.Process;

namespace VAdvantage.ProcessEngine
{
    /// <summary>
    /// Utility function for process
    /// </summary>
    public class ProcessInfoUtil
    {
        //Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(ProcessInfoUtil).FullName);


        /// <summary>
        /// Sets the summary from database
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static void SetSummaryFromDB(ProcessInfo pi)
        {
            int sleepTime = 2000;	//	2 secomds
            int noRetry = 5;        //  10 seconds total
            //
            String sql = "SELECT Result, ErrorMsg FROM AD_PInstance "
                + "WHERE AD_PInstance_ID=@instanceid"
                + " AND Result IS NOT NULL";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                for (int noTry = 0; noTry < noRetry; noTry++)
                {
                    param[0] = new SqlParameter("@instanceid", pi.GetAD_PInstance_ID());
                    dr = DataBase.DB.ExecuteReader(sql,param,null);
                    while (dr.Read())
                    {
                        //	we have a result
                        int i = Utility.Util.GetValueOfInt(dr[0].ToString());
                        if (i == 1)
                        {
                            pi.SetSummary(Msg.GetMsg(Env.GetContext(), "Success", true));
                        }
                        else
                        {
                            pi.SetSummary(Msg.GetMsg(Env.GetContext(), "Failure", true));
                        }

                        String Message = dr[1].ToString();
                        dr.Close();
                        //
                        if (Message != null)
                        {
                            if (Message != "")
                                pi.AddSummary("  (" + Utility.Msg.ParseTranslation(Utility.Env.GetContext(), Message) + ")");
                        }
                        return;
                    }

                    dr.Close();
                    //	sleep
                    try
                    {
                        Thread.Sleep(sleepTime);
                    }
                    catch (Exception ie)
                    {
                        if (dr != null)
                        {
                            dr.Close();
                        }
                        _log.Log(Level.SEVERE, "Sleep Thread", ie);
                    }
                }

            }
            catch (SqlException e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
                pi.SetSummary(e.Message, true);
                return;
            }
            pi.SetSummary(Msg.GetMsg(Env.GetContext(), "Timeout", true));
        }	//	setSummaryFromDB


        /// <summary>
        /// Set param from db
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static void SetParameterFromDB(ProcessInfo pi)
        {
            List<ProcessInfoParameter> list = new List<ProcessInfoParameter>();
            String sql = "SELECT p.ParameterName,"         			    	//  1
                + " p.P_String,p.P_String_To, p.P_Number,p.P_Number_To,"    //  2/3 4/5
                + " p.P_Date,p.P_Date_To, p.Info,p.Info_To, "               //  6/7 8/9
                + " i.AD_Client_ID, i.AD_Org_ID, i.AD_User_ID "				//	10..12
                + "FROM AD_PInstance_Para p"
                + " INNER JOIN AD_PInstance i ON (p.AD_PInstance_ID=i.AD_PInstance_ID) "
                + "WHERE p.AD_PInstance_ID=@pinstanceid "
                + "ORDER BY p.SeqNo";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", pi.GetAD_PInstance_ID());
                //param[0] = new SqlParameter("@pinstanceid", 1000296);

                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    String ParameterName = dr[0].ToString();
                    //	String
                    Object Parameter = dr[1].ToString(); 
                    Object Parameter_To = dr[2].ToString();

                    Parameter = Parameter.ToString() == "" ? null  : Parameter;
                    Parameter_To = Parameter_To.ToString() == "" ? null : Parameter_To;

                    //	Big Decimal
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {

                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter = Utility.Util.GetValueOfDecimal(dr[3]);
                        }
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter_To = Utility.Util.GetValueOfDecimal(dr[4]);
                        }
                    }
                    //	Timestamp
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        if (!(dr[5] == DBNull.Value))
                        {
                            Parameter = DateTime.Parse(dr[5].ToString());
                        }
                        if (!(dr[6] == DBNull.Value))
                        {
                            Parameter_To = DateTime.Parse(dr[6].ToString());
                        }
                    }
                    //	Info
                    String Info = dr[7].ToString();
                    String Info_To = dr[8].ToString();
                    //
                    list.Add(new ProcessInfoParameter(ParameterName, Parameter, Parameter_To, Info, Info_To));
                    //
                    if (pi.GetAD_Client_ID() == null)
                        pi.SetAD_Client_ID(int.Parse(dr[9].ToString()));
                    if (pi.GetAD_User_ID() == null)
                        pi.SetAD_User_ID(int.Parse(dr[11].ToString()));
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Severe(e.ToString());
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //
            ProcessInfoParameter[] pars = new ProcessInfoParameter[list.Count()];
            pars = list.ToArray();
            pi.SetParameter(pars);
        }   //  setParameterFromDB


        /// <summary>
        /// Set param from db
        /// </summary>
        /// <param name="pi">ProcessInfo object</param>
        public static ProcessInfoParameter[] SetCrystalParameterFromDB(int id)
        {
            List<ProcessInfoParameter> list = new List<ProcessInfoParameter>();
            String sql = "SELECT p.ParameterName,"         			    	//  1
                + " p.P_String,p.P_String_To, p.P_Number,p.P_Number_To,"    //  2/3 4/5
                + " p.P_Date,p.P_Date_To, p.Info,p.Info_To, "               //  6/7 8/9
                + " i.AD_Client_ID, i.AD_Org_ID "				//	10..12
                + "FROM AD_CrystalInstance_Para p"
                + " INNER JOIN AD_CrystalInstance i ON (p.AD_CrystalInstance_ID=i.AD_CrystalInstance_ID) "
                + "WHERE p.AD_CrystalInstance_ID=@pinstanceid "
                + "ORDER BY p.SeqNo";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", id);
                //param[0] = new SqlParameter("@pinstanceid", 1000296);

                dr = DataBase.DB.ExecuteReader(sql, param, null);
                while (dr.Read())
                {
                    String ParameterName = dr[0].ToString();
                    //	String
                    Object Parameter = dr[1].ToString();
                    Object Parameter_To = dr[2].ToString();
                    //	Big Decimal
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter = Utility.Util.GetValueOfDecimal(dr[3]);
                        }
                        if (!(string.IsNullOrEmpty(dr[3].ToString())))
                        {
                            Parameter_To = Utility.Util.GetValueOfDecimal(dr[4]);
                        }
                    }
                    //	Timestamp
                    if ((Parameter == null && Parameter_To == null) || (Parameter.Equals("") && Parameter_To.Equals("")))
                    {
                        if (!(dr[5] == DBNull.Value))
                        {
                            Parameter = DateTime.Parse(dr[5].ToString());
                        }
                        if (!(dr[6] == DBNull.Value))
                        {
                            Parameter_To = DateTime.Parse(dr[6].ToString());
                        }
                    }
                    //	Info
                    String Info = dr[7].ToString();
                    String Info_To = dr[8].ToString();
                    //
                    list.Add(new ProcessInfoParameter(ParameterName, Parameter, Parameter_To, Info, Info_To));
                    //
                    //if (pi.GetAD_Client_ID() == null)
                    //    pi.SetAD_Client_ID(int.Parse(dr[9].ToString()));
                    //if (pi.GetAD_User_ID() == null)
                    //    pi.SetAD_User_ID(int.Parse(dr[11].ToString()));
                }
                dr.Close();

            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Severe(e.ToString());
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }
            //
            ProcessInfoParameter[] pars = new ProcessInfoParameter[list.Count()];
            pars = list.ToArray();
            return pars;
            //pi.SetParameter(pars);
        }   //  setParameterFromDB

        public static void SetLogFromDB(ProcessInfo pi)
        {
            String sql = "SELECT Log_ID, P_ID, P_Date, P_Number, P_Msg "
                + "FROM AD_PInstance_Log "
                + "WHERE AD_PInstance_ID=@instanceid "
                + "ORDER BY Log_ID";
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@instanceid", pi.GetAD_PInstance_ID());
                dr = DataBase.DB.ExecuteReader(sql, param);

                int? ival;
                while (dr.Read())
                {
                    if (dr[1].ToString() == "")
                    {
                        ival = null;
                    }
                    else
                    {
                        ival = (int?)Utility.Util.GetValueOfInt(dr[1]);
                    }
                    pi.AddLog(Utility.Util.GetValueOfInt(dr[0]), ival, Utility.Util.GetValueOfDateTime(dr[2]), Utility.Util.GetValueOfDecimal(dr[3]), dr[4].ToString());
                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                _log.Log(Level.SEVERE, "setLogFromDB", e);
            }



        }

        public static void SaveLogToDB(ProcessInfo pi)
        {
            Context p_ctx = Env.GetContext();
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Env.GetLanguage(p_ctx).GetCulture(Env.GetBaseAD_Language());
            ProcessInfoLog[] logs = pi.GetLogs();
            if (logs == null || logs.Length == 0)
            {
                _log.Fine("No Log");
                return;
            }
            if (pi.GetAD_PInstance_ID() == 0)
            {
                _log.Log(Level.WARNING, "AD_PInstance_ID==0");
                return;
            }
            for (int i = 0; i < logs.Length; i++)
            {
                StringBuilder sql = new StringBuilder("INSERT INTO AD_PInstance_Log "
                    + "(AD_PInstance_ID, Log_ID, P_Date, P_ID, P_Number, P_Msg)"
                    + " VALUES (");
                sql.Append(pi.GetAD_PInstance_ID()).Append(",")
                    .Append(logs[i].GetLog_ID()).Append(",");
                if (logs[i].GetP_Date() == null)
                    sql.Append("NULL");
                else
                    sql.Append(GlobalVariable.TO_DATE(logs[i].GetP_Date(), false));
                sql.Append(",");
                if (logs[i].GetP_ID() == 0)
                    sql.Append("NULL");
                else
                    sql.Append(logs[i].GetP_ID());
                sql.Append(",");
                if (logs[i].GetP_Number() == null)
                    sql.Append("NULL");
                else
                    sql.Append(logs[i].GetP_Number());
                sql.Append(",");
                if (logs[i].GetP_Msg() == null)
                    sql.Append("NULL)");
                else
                {
                    sql.Append(GlobalVariable.TO_STRING(logs[i].GetP_Msg(), 2000)).Append(")");
                }

                SqlExec.ExecuteQuery.ExecuteNonQuery(sql.ToString());
            }
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
            System.Threading.Thread.CurrentThread.CurrentCulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            System.Threading.Thread.CurrentThread.CurrentUICulture = Utility.Env.GetLanguage(Utility.Env.GetContext()).GetCulture(Env.GetLoginLanguage(p_ctx).GetAD_Language());
            pi.SetLogList(null);	//	otherwise log entries are twice
        }

    }
}
