using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using VAdvantage.Classes;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;

using VIS.Helpers;
using VIS.DataContracts;
using VIS.Classes;
using VIS.Models;
using System.Web.Mvc;
using VIS.Filters;
using System.Web.SessionState;

namespace VIS.Controllers
{
    /// <summary>
    /// common class to handle json data request 
    /// </summary>
    /// 
    [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    [SessionState(SessionStateBehavior.ReadOnly)]
    // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]

    public class JsonDataController : Controller
    {

        public JsonResult UpdateCtx(Dictionary<string, object> dCtx)
        {

            string ip = Request.UserHostAddress;
            string hostName = Request.UserHostName;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                if (dCtx != null && dCtx.Count > 0)
                {
                    foreach (var i in dCtx)
                    {
                        ctx.SetContext(i.Key, (string)i.Value);
                    }
                }
                ctx.SetContext("IPAddress", ip);
                ctx.SetContext("HostName", hostName);
            }
            else
            {

            }
            return Json(new { result = "" }, JsonRequestBehavior.AllowGet);
        }

        #region Window

        /// <summary>
        /// retrun Grid window Model json object against window Id
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <returns>grid window json result</returns>
        public JsonResult GetGridWindow(int windowNo, int AD_Window_ID)
        {
            GridWindow wVo = null;
            string retJSON = "";
            string retError = null;
            string windowCtx = "";

            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                GridWindowVO vo = AEnv.GetMWindowVO(ctx, windowNo, AD_Window_ID, 0);
                if (vo != null)
                {
                    wVo = new GridWindow(vo);
                    retJSON = JsonConvert.SerializeObject(wVo, Formatting.None);
                }
                else
                {
                    retError = "AccessTableNoView";
                }
                windowCtx = JsonConvert.SerializeObject(ctx.GetMap(windowNo));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError, wCtx = windowCtx }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// retrun Grid window Model json object against window Id
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <returns>grid window json result</returns>
        public JsonResult GetWindowRecords(List<string> fields, SqlParamsIn sqlIn, int rowCount, string sqlCount, int AD_Table_ID)
        {
            object data = null;
            if (Session["ctx"] == null)
            {

            }
            else
            {
                using (var w = new WindowHelper())
                {
                    data = w.GetWindowRecords(sqlIn, fields, Session["ctx"] as Ctx, rowCount, sqlCount, AD_Table_ID);
                }
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }


        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public JsonResult GetWindowRecord(string sql, List<string> fields)
        {
            object data = null;
            if (Session["ctx"] == null)
            {
            }
            else
            {
                using (var w = new WindowHelper())
                {
                    data = w.GetWindowRecord(sql, fields, Session["ctx"] as Ctx);
                }
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save or Update Window Record
        /// </summary>
        /// <param name="gridTable">Data Contract </param>
        /// <returns>json result </returns>
        public JsonResult InsertOrUpdateWRecords(SaveRecordIn gridTable)
        {
            try
            {
                SaveRecordOut gOut = null;
                if (Session["ctx"] == null)
                {
                    gOut = new SaveRecordOut();
                    gOut.IsError = true;
                    gOut.ErrorMsg = "Session Expired";
                }
                else
                {
                    using (var w = new WindowHelper())
                    {
                        gOut = w.InsertOrUpdateRecord(gridTable, Session["ctx"] as Ctx);
                    }
                }
                return Json(JsonConvert.SerializeObject(gOut), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                SaveRecordOut o = new SaveRecordOut();
                o.IsError = true;
                o.ErrorMsg = e.Message;
                return Json(JsonConvert.SerializeObject(o), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Window Record(s) 
        ///  - handle error or restriction 
        ///  - delete multiple or single record
        /// </summary>
        /// <param name="dInn">data contract</param>
        /// <returns>json result</returns>
        public JsonResult DeleteWRecords(DeleteRecordIn dInn)
        {
            DeleteRecordOut gOut = null;
            if (Session["ctx"] == null)
            {
                gOut = new DeleteRecordOut();
                gOut.IsError = true;
                gOut.ErrorMsg = "Session Expired";
            }
            else
            {
                using (var w = new WindowHelper())
                {
                    gOut = w.DeleteRecord(dInn, Session["ctx"] as Ctx);
                }
            }
            return Json(JsonConvert.SerializeObject(gOut), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecordInfo(RecordInfoIn dse)
        {
            RecordInfoOut gOut = null;
            using (var w = new WindowHelper())
            {
                gOut = w.GetRecordInfo(dse, Session["ctx"] as Ctx);
            }
            return Json(JsonConvert.SerializeObject(gOut), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrInsertPersonalLock(int AD_User_ID, int AD_Table_ID, int Record_ID, bool locked)
        {
            MPrivateAccess access = MPrivateAccess.Get(Session["ctx"] as Ctx, AD_User_ID, AD_Table_ID, Record_ID);
            if (access == null)
            {
                access = new MPrivateAccess(Session["ctx"] as Ctx, AD_User_ID, AD_Table_ID, Record_ID);
            }
            access.SetIsActive(locked);
            bool ret = access.Save();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Form

        /// <summary>
        /// get form Info agianst form Id 
        /// - call from menu form item
        /// </summary>
        /// <param name="AD_Form_ID">id of form</param>
        /// <returns>json result</returns>
        public JsonResult GetFormInfo(int AD_Form_ID)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                retJSON = JsonConvert.SerializeObject(FormHelper.GetFormInfo(AD_Form_ID, Session["ctx"] as Ctx));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Process

        /// <summary>
        /// Get Process Info
        /// - call from process menu Item 
        /// </summary>
        /// <param name="AD_Process_ID">id of process</param>
        /// <returns>json result</returns>
        public JsonResult GetProcessInfo(int AD_Process_ID)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                retJSON = JsonConvert.SerializeObject(ProcessHelper.GetProcessInfo(AD_Process_ID, Session["ctx"] as Ctx));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// execute process 
        /// - return process parameter fields if has any, halt processing
        /// </summary>
        /// <param name="AD_Process_ID">id of process</param>
        /// <param name="Name">name of process</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record  id of table</param>
        /// <param name="WindowNo">window number</param>
        /// <returns>json result
        /// - either complete or uncomplete message 
        /// - or list process fields
        /// </returns>
        public JsonResult Process(int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, bool csv, bool pdf)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                retJSON = JsonConvert.SerializeObject(ProcessHelper.Process(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_Table_ID, Record_ID, WindowNo, csv, pdf));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// execute process   
        /// - call after parameter dialog window
        /// - first save parameter for Process then execute
        /// </summary>
        /// <param name="AD_Process_ID">id of process</param>
        /// <param name="Name">name of process</param>
        /// <param name="AD_PInstance_ID">process instance id</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="ParameterList">process parameter list</param>
        /// <returns>json result</returns>
        public JsonResult ExecuteProcess(int AD_Process_ID, string Name, int AD_PInstance_ID, int AD_Table_ID, int Record_ID, ProcessPara[] ParameterList, bool csv, bool pdf)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                ProcessReportInfo rep = (ProcessHelper.ExecuteProcess(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_PInstance_ID, AD_Table_ID, Record_ID, ParameterList, csv, pdf));
                //rep.HTML=GetHtml(rep.ReportFilePath);
                if (rep.Report != null && (rep.Report.Length > 1048576 || Record_ID > 0) && !csv && !pdf)
                {
                    rep.AskForNewTab = true;
                    string filePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload" + "\\temp_" + DateTime.Now.Ticks + ".pdf";
                    System.IO.File.WriteAllBytes(filePath, rep.Report);
                    rep.ReportFilePath = filePath.Substring(filePath.IndexOf("TempDownload"));
                    rep.HTML = null;
                    rep.Report = null;
                }
                retJSON = JsonConvert.SerializeObject(rep);
                //retJSON = JsonConvert.SerializeObject(ProcessHelper.ExecuteProcess(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_PInstance_ID, AD_Table_ID, Record_ID, ParameterList));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region "Dataset"

        //private string GetHtml(string pdfFilePath)
        //{


        //    string pathToPdf = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + pdfFilePath;
        //    string pathToHtml = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\temp" + DateTime.Now.Ticks + ".html";

        //    // Convert PDF file to HTML file
        //    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
        //    // Let's force the component to store images inside HTML document
        //    // using base-64 encoding
        //    f.HtmlOptions.IncludeImageInHtml = true;
        //    f.HtmlOptions.Title = "Simple text";

        //    // This property is necessary only for registered version
        //    //f.Serial = "XXXXXXXXXXX";

        //    f.OpenPdf(pathToPdf);

        //    if (f.PageCount > 0)
        //    {
        //      //  f.ToHtml(pathToHtml);
        //        return f.ToHtml();

        //    }
        //    return null;
        //}
        /// <summary>
        /// json object representation of dataset
        /// --has table or tables -
        /// -- 
        /// </summary>
        /// <param name="sqlIn">sql param datacontract </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult JDataSet(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            object data = h.ExecuteJDataSet(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// Execute Multiple queries and return result of each query in a list
        /// 
        /// </summary>
        /// <param name="sql">sql splitted by '/'</param>
        /// <param name="lstParams">Parameters for each query </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult ExecuteNonQueries(string sql, List<List<SqlParams>> param)
        {
            SqlHelper h = new SqlHelper();
            object data = h.ExecuteNonQueries(sql, param);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// Execute Multiple queries and return result of each query in a list
        /// 
        /// </summary>
        /// <param name="sql">sql splitted by '/'</param>
        /// <param name="lstParams">Parameters for each query </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult ExecuteNonQuery(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            object data = h.ExecuteNonQuery(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Others"
        public JsonResult GetLookup(Dictionary<string, object> ctx, int windowNo, int column_ID, int AD_Reference_ID, string columnName,
            int AD_Reference_Value_ID, bool isParent, string validationCode)
        {
            Ctx _ctx = new Ctx(ctx);
            //Ctx _ctx = null;//(ctx) as Ctx;
            Lookup res = LookupHelper.GetLookup(_ctx, windowNo, column_ID, AD_Reference_ID, columnName,
                AD_Reference_Value_ID, isParent, validationCode);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateReport(int id, string queryInfo, Object code, bool isCreateNew, string ProcessInfo, bool pdf, bool csv)
        {
            if (Session["ctx"] != null)
            {
                List<string> qryInfo = JsonConvert.DeserializeObject<List<string>>(queryInfo);
                Dictionary<string, object> nProcessInfo = null;
                if (ProcessInfo != null)
                {
                    nProcessInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(queryInfo);
                }
                ProcessReportInfo rep = (ProcessHelper.GenerateReport(Session["ctx"] as Ctx, id, qryInfo, code, isCreateNew, nProcessInfo, pdf, csv));
                return Json(JsonConvert.SerializeObject(rep), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GeneratePrint(int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, bool csv)
        {
            if (Session["ctx"] != null)
            {
                ProcessReportInfo rep = (ProcessHelper.GeneratePrint(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_Table_ID, Record_ID, WindowNo, csv));
                return Json(JsonConvert.SerializeObject(rep), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult ArchiveDoc(int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int C_BPartner_ID, bool isReport, byte[] binaryData)
        {
            if (Session["ctx"] != null)
            {
                return Json(JsonConvert.SerializeObject(ProcessHelper.ArchiveDoc(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_Table_ID, Record_ID, C_BPartner_ID, isReport, binaryData)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AttachFrom(int docID, int winID, int tableID, int recID)
        {
            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                string sql = "Select count(VADMS_WindowDocLink_ID) from VADMS_WindowDocLink where AD_Table_ID=" + tableID + " AND record_ID=" + recID + " AND AD_Window_ID=" + winID + " AND VADMS_Document_ID=" + docID;
                if (Convert.ToInt32(DB.ExecuteScalar(sql)) > 0)
                {
                    return Json(JsonConvert.SerializeObject("NotSaved"), JsonRequestBehavior.AllowGet);
                }
                VAdvantage.Model.X_VADMS_WindowDocLink wlink = new VAdvantage.Model.X_VADMS_WindowDocLink(ctx, 0, null);
                wlink.SetAD_Client_ID(ctx.GetAD_Client_ID());
                wlink.SetAD_Org_ID(ctx.GetAD_Org_ID());
                wlink.SetAD_Table_ID(tableID);
                wlink.SetAD_Window_ID(winID);
                wlink.SetRecord_ID(recID);
                wlink.SetVADMS_Document_ID(docID);
                if (wlink.Save())
                {
                    return Json(JsonConvert.SerializeObject("OK"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(JsonConvert.SerializeObject("NotSaved"), JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }



    /// <summary>
    /// handle Tree Creation Request for AD_Window
    /// </summary>
    public class TreeController : Controller
    {
        public ActionResult GetTreeAsString(int AD_Tree_ID, bool editable, int windowNo)
        {
            var html = "";
            if (Session["ctx"] != null)
            {
                var m = new MenuHelper(Session["ctx"] as Ctx);
                var tree = m.GetMenuTree(AD_Tree_ID, editable);
                html = m.GetMenuTreeUI(tree.GetRootNode(), @Url.Content("~/"), windowNo.ToString(), tree.GetNodeTableName());
                m.dispose();
            }
            return Content(html);
        }
    }
}
