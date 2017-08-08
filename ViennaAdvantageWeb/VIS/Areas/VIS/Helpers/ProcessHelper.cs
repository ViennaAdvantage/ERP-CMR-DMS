using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Hosting;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Print;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using VIS.DataContracts;

namespace VIS.Helpers
{
    public class ProcessHelper
    {
        public static ProcessDataOut GetProcessInfo(int AD_Process_ID, Ctx ctx)
        {
            ProcessDataOut outt = new ProcessDataOut();

            bool trl = !Env.IsBaseLanguage(ctx, "AD_Process");
            String sql = "SELECT p.Name, p.Description, p.Help, p.IsReport, p.AD_CtxArea_ID, ca.IsSOTrx "
                    + "FROM AD_Process p "
                    + "LEFT OUTER JOIN AD_CtxArea ca ON (p.AD_CtxArea_ID=ca.AD_CtxArea_ID) "
                    + "WHERE AD_Process_ID=" + AD_Process_ID;
            if (trl)
                sql = "SELECT t.Name, t.Description, t.Help, p.IsReport, p.AD_CtxArea_ID, ca.IsSOTrx "
                    + "FROM AD_Process p "
                    + "LEFT OUTER JOIN AD_CtxArea ca ON (p.AD_CtxArea_ID=ca.AD_CtxArea_ID) "
                    + " INNER JOIN AD_Process_Trl t ON (p.AD_Process_ID=t.AD_Process_ID) "
                    + "WHERE p.AD_Process_ID=" + AD_Process_ID + " AND t.AD_Language='" + Env.GetAD_Language(ctx) + "'";

            IDataReader dr = null;

            try
            {

                dr = DB.ExecuteReader(sql, null);

                if (dr.Read())
                {
                    outt.Name = Env.TrimModulePrefix(dr.GetString(0));
                    outt.IsReport = dr.GetString(3).Equals("Y");

                    //
                    string msgText = "<b>";

                    if (dr.IsDBNull(1))
                        msgText += Msg.GetMsg(ctx, "StartProcess?");
                    else
                        msgText += dr.GetString(1);

                    msgText += "</b>";

                    if (!dr.IsDBNull(2))
                        msgText += "<p>" + dr.GetString(2) + "</p>";
                    //
                    outt.MessageText = msgText;

                    String isSOTrx = dr[5].ToString();
                    if (isSOTrx == "")
                        isSOTrx = "Y";
                    outt.IsSOTrx = isSOTrx;

                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                    dr.Close();
                outt.IsError = true;
                outt.Message = e.Message;
            }
            return outt;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal static ProcessReportInfo ExecuteProcess(Ctx ctx, int AD_Process_ID, string Name, int AD_PInstance_ID, int AD_Table_ID, int Record_ID, ProcessPara[] pList, bool csv = false, bool pdf = false)
        {
            int vala = 0;
            if (pList != null && pList.Length > 0) //we have process parameter
            {
                for (int i = 0; i < pList.Length; i++)
                {
                    var pp = pList[i];
                    //	Create Parameter
                    MPInstancePara para = new MPInstancePara(ctx, AD_PInstance_ID, i);
                    para.SetParameterName(pp.Name);

                    if (DisplayType.IsDate(pp.DisplayType))
                    {
                        if (pp.Result != null)
                        {
                            para.SetP_Date(Convert.ToDateTime(pp.Result));
                        }

                        if (pp.Result2 != null)
                        {
                            para.SetP_Date_To(Convert.ToDateTime(pp.Result2));
                        }
                    }
                    //else if (pp.Result is int || pp.Result2 is int)
                    //{
                    //    if (pp.Result != null)
                    //    {
                    //        para.SetP_Number(Convert.ToInt32(pp.Result));
                    //    }

                    //    if (pp.Result2 != null)
                    //    {
                    //        para.SetP_Number_To(Convert.ToInt32(pp.Result2));
                    //    }
                    //}
                    //else if (pp.Result is decimal || pp.Result2 is decimal)
                    //{
                    //    if (pp.Result != null)
                    //    {
                    //        para.SetP_Number(Convert.ToDecimal(pp.Result));
                    //    }
                    //    if (pp.Result2 != null)
                    //    {
                    //        para.SetP_Number_To(Convert.ToDecimal(pp.Result2));
                    //    }
                    //}
                    ////	Boolean
                    //else if (pp.Result is Boolean)
                    //{
                    //    Boolean bb = (Boolean)pp.Result;
                    //    String value = bb ? "Y" : "N";
                    //    para.SetP_String(value);
                    //    //	to does not make sense
                    //}
                    //*********
                    else if ((DisplayType.IsID(pp.DisplayType) || DisplayType.Integer == pp.DisplayType))
                    {
                        if (pp.Result != null)
                        {
                            if (int.TryParse(pp.Result.ToString(), out vala))
                            {
                                para.SetP_Number(Convert.ToInt32(pp.Result));
                            }
                            else
                            {
                                para.SetP_String(pp.Result.ToString());
                            }
                        }
                        if (pp.Result2 != null)
                        {
                            if (int.TryParse(pp.Result2.ToString(), out vala))
                            {
                                para.SetP_Number_To(Convert.ToInt32(pp.Result2));
                            }
                            else
                            {
                                para.SetP_String_To(pp.Result2.ToString());
                            }
                        }

                    }
                    else if (DisplayType.IsNumeric(pp.DisplayType))
                    {
                        if (pp.Result != null)
                        {
                            para.SetP_Number(Convert.ToDecimal(pp.Result));
                        }
                        if (pp.Result2 != null)
                        {
                            para.SetP_Number_To(Convert.ToDecimal(pp.Result2));
                        }

                    }
                    else if (DisplayType.YesNo == pp.DisplayType)
                    {
                        Boolean bb = (Boolean)pp.Result;
                        String value = bb ? "Y" : "N";
                        para.SetP_String(value);
                    }
                    //*********
                    else
                    {
                        if (pp.Result != null)
                        {
                            para.SetP_String(pp.Result.ToString());
                        }
                        if (pp.Result2 != null)
                        {
                            para.SetP_String_To(pp.Result.ToString());
                        }
                    }

                    para.SetInfo(pp.Info);
                    if (pp.Info_To != null)
                        para.SetInfo_To(pp.Info_To);
                    para.Save();
                }
            }

            // ReportEngine_N re = null;

            string lang = ctx.GetAD_Language().Replace("_", "-");
            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);


            ////////log/////
            string clientName = ctx.GetAD_Org_Name() + "_" + ctx.GetAD_User_Name();
            string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");
            storedPath += clientName;
            VLogMgt.Initialize(true, storedPath);
            // VLogMgt.AddHandler(VLogFile.Get(true, storedPath, true));
            ////////////////

            byte[] report = null;
            string rptFilePath = null;
            ProcessReportInfo rep = new ProcessReportInfo();
            try
            {
                ProcessInfo pi = new ProcessInfo(Name, AD_Process_ID, AD_Table_ID, Record_ID);
                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(AD_PInstance_ID);

                //report = null;
                ProcessCtl ctl = new ProcessCtl();
                ctl.IsArabicReportFromOutside = false;
                ctl.SetIsPrintCsv(csv);
                if (pdf)
                {
                    ctl.SetIsPrintFormat(true);
                }
                Dictionary<string, object> d = ctl.Process(pi, ctx, out report, out rptFilePath);
                rep = new ProcessReportInfo();
                rep.ReportProcessInfo = d;
                rep.Report = report;
                rep.ReportString = ctl.ReportString;
                rep.ReportFilePath = rptFilePath;
                rep.IsRCReport = ctl.IsRCReport();
                //  rep.AD_PrintFormat_ID = ctl.GetAD_PrintFormat_ID();
                if (d.ContainsKey("AD_PrintFormat_ID"))
                {
                    rep.AD_PrintFormat_ID = Convert.ToInt32(d["AD_PrintFormat_ID"]);
                }
                ctl.ReportString = null;
                rep.HTML = ctl.GetRptHtml();
                rep.AD_Table_ID = ctl.GetReprortTableID();


                //Env.GetCtx().Clear();
            }
            catch (Exception e)
            {
                rep.IsError = true;
                rep.Message = e.Message;
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = original;
            System.Threading.Thread.CurrentThread.CurrentUICulture = original;

            return rep;
        }

        internal static ProcessReportInfo Process(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, bool csv, bool pdf)
        {
            ProcessReportInfo ret = new ProcessReportInfo();
            MPInstance instance = null;
            try
            {
                instance = new MPInstance(ctx, AD_Process_ID, Record_ID);
            }
            catch (Exception e)
            {
                ret.IsError = true;
                ret.Message = e.Message;
                return ret;
            }

            if (!instance.Save())
            {
                ret.IsError = true;
                ret.Message = Msg.GetMsg(ctx, "ProcessNoInstance");
                return ret;
            }
            ret.AD_PInstance_ID = instance.GetAD_PInstance_ID();

            //	Get Parameters (Dialog)
            //Check If Contaon Parameter

            List<GridField> fields = ProcessParameter.GetParametersList(ctx, AD_Process_ID, WindowNo);

            if (fields.Count < 1) //no Parameter
            {
                ret = ExecuteProcess(ctx, AD_Process_ID, Name, ret.AD_PInstance_ID, AD_Table_ID, Record_ID, null, csv, pdf);
            }
            else
            {
                ret.ShowParameter = true;
                ret.ProcessFields = fields;
            }
            return ret;
        }

        public static ProcessReportInfo GenerateReport(Ctx _ctx, int id, List<string> queryInfo, Object code, bool isCreateNew, Dictionary<string, object> nProcessInfo, bool pdf, bool csv)
        {
            ProcessReportInfo rep = null;
            ReportEngine_N re = null;
            Query _query = null;
            int Record_ID = 0;
            object AD_tab_ID = 0;
            // _ctx.SetContext("#TimeZoneName", "India Standard Time");
            if (queryInfo.Count > 0)
            {
                string tableName = queryInfo[0];
                string wherClause = queryInfo[1];
                _query = new Query(tableName);

                if (!string.IsNullOrEmpty(wherClause))
                    _query.AddRestriction(wherClause);


                if (_query.GetRestrictionCount() == 1 && (code).GetType() == typeof(int))
                    Record_ID = ((int)code);
            }
            if (queryInfo.Count > 2)
            {
                if (queryInfo[2] != null && queryInfo[2] != "" && Convert.ToInt32(queryInfo[2]) > 0)
                {
                    AD_tab_ID = Convert.ToInt32(queryInfo[2]);
                }
            }
            //Context _ctx = new Context(ctxDic);
            //Env.SetContext(_ctx);
            string lang = _ctx.GetAD_Language().Replace("_", "-");


            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);



            byte[] b = null;
            try
            {
                MPrintFormat pf = null;

                if (!isCreateNew)
                    pf = MPrintFormat.Get(_ctx, id, true);
                else
                {
                    //   pf = MPrintFormat.CreateFromTable(_ctx, id);
                    if (Convert.ToInt32(AD_tab_ID) > 0)
                    {
                        pf = MPrintFormat.CreateFromTable(_ctx, id, AD_tab_ID);
                    }
                    else
                    {
                        pf = MPrintFormat.CreateFromTable(_ctx, id);
                    }

                }

                rep = new ProcessReportInfo();
                pf.SetName(Env.TrimModulePrefix(pf.GetName()));
                if (nProcessInfo == null)
                {
                    PrintInfo info = new PrintInfo(pf.GetName(), pf.GetAD_Table_ID(), Record_ID);
                    info.SetDescription(_query == null ? "" : _query.GetInfo());
                    re = new ReportEngine_N(_ctx, pf, _query, info);
                }
                else
                {
                    ProcessInfo pi = new ProcessInfo().FromList(nProcessInfo);
                    pi.Set_AD_PrintFormat_ID(id);
                    ProcessCtl ctl = new ProcessCtl();
                    ctl.Process(pi, _ctx, out b, out re);
                    re.SetPrintFormat(pf);
                }

                re.GetView();
                if (pdf)
                {
                    rep.ReportFilePath = re.GetReportFilePath(true, out b);


                }
                else if (csv)
                {
                    rep.ReportFilePath = re.GetCSVPath(_ctx);
                }
                else
                {
                    rep.HTML = re.GetRptHtml().ToString();
                    rep.AD_PrintFormat_ID = re.GetPrintFormat().GetAD_PrintFormat_ID();
                    rep.ReportProcessInfo = null;
                    rep.Report = re.CreatePDF();
                }
                // b = re.CreatePDF();
                //rep.Report = b;
                //rep.HTML = re.GetRptHtml().ToString();
                //rep.AD_PrintFormat_ID = re.GetPrintFormat().GetAD_PrintFormat_ID();
                //rep.ReportProcessInfo = null;
            }
            catch (Exception ex)
            {
                rep.IsError = true;
                rep.ErrorText = ex.Message;
            }
            return rep;
        }

        public static ProcessReportInfo GeneratePrint(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, bool csv)
        {
            ProcessReportInfo ret = new ProcessReportInfo();
            MPInstance instance = null;
            try
            {
                instance = new MPInstance(ctx, AD_Process_ID, Record_ID);
            }
            catch (Exception e)
            {
                ret.IsError = true;
                ret.Message = e.Message;
                return ret;
            }

            if (!instance.Save())
            {
                ret.IsError = true;
                ret.Message = Msg.GetMsg(ctx, "ProcessNoInstance");
                return ret;
            }
            ret.AD_PInstance_ID = instance.GetAD_PInstance_ID();

            List<GridField> fields = ProcessParameter.GetParametersList(ctx, AD_Process_ID, WindowNo);

            if (fields.Count < 1) //no Parameter
            {


                //ReportEngine_N re = null;

                string lang = ctx.GetAD_Language().Replace("_", "-");
                System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);


                ////////log/////
                string clientName = ctx.GetAD_Org_Name() + "_" + ctx.GetAD_User_Name();
                string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");
                storedPath += clientName;
                VLogMgt.Initialize(true, storedPath);
                ////////////////

                byte[] report = null;
                string rptFilePath = null;
                // ProcessReportInfo rep = new ProcessReportInfo();
                try
                {
                    ProcessInfo pi = new ProcessInfo(Name, AD_Process_ID, AD_Table_ID, Record_ID);
                    pi.SetAD_User_ID(ctx.GetAD_User_ID());
                    pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                    pi.SetAD_PInstance_ID(ret.AD_PInstance_ID);

                    //report = null;
                    ProcessCtl ctl = new ProcessCtl();
                    ctl.SetIsPrintFormat(true);
                    ctl.IsArabicReportFromOutside = false;
                    ctl.SetIsPrintCsv(csv);
                    Dictionary<string, object> d = ctl.Process(pi, ctx, out report, out rptFilePath);
                    //rep = new ProcessReportInfo();
                    ret.ReportProcessInfo = d;
                    ret.Report = report;
                    ret.ReportString = ctl.ReportString;
                    ret.ReportFilePath = rptFilePath;
                    ret.HTML = ctl.GetRptHtml();
                    ret.IsRCReport = ctl.IsRCReport();
                    ctl.ReportString = null;


                    //Env.GetCtx().Clear();
                }
                catch (Exception e)
                {
                    ret.IsError = true;
                    ret.Message = e.Message;
                }

                System.Threading.Thread.CurrentThread.CurrentCulture = original;
                System.Threading.Thread.CurrentThread.CurrentUICulture = original;

                //return ret;
                //ret = ExecuteProcess(ctx, AD_Process_ID, Name, ret.AD_PInstance_ID, AD_Table_ID, Record_ID, null,true);
            }
            else
            {
                ret.ShowParameter = true;
                ret.ProcessFields = fields;
            }
            return ret;
        }

        public static bool ArchiveDoc(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int C_BPartner_ID, bool isReport, byte[] binaryData)
        {
            MArchive archive = new MArchive(ctx, 0, null);
            archive.SetName(Name);
            archive.SetIsReport(isReport);
            archive.SetAD_Process_ID(AD_Process_ID);
            archive.SetAD_Table_ID(AD_Table_ID);
            archive.SetRecord_ID(Record_ID);
            archive.SetC_BPartner_ID(C_BPartner_ID);
            archive.SetBinaryData(binaryData);
            return archive.Save();
        }

    }
}