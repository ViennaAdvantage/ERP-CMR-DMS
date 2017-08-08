using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using VAdvantage.Acct;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Filters;

namespace VIS.Controllers
{
    public class PostingController : Controller
    {
        //
        // GET: /VIS/Posting/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult PostImmediate(int AD_Client_ID, int AD_Table_ID, int Record_ID, bool force)
        {

            Ctx ctx = Session["ctx"] as Ctx;
            string res = "";
            try
            {
                string clientName = ctx.GetAD_Org_Name() + "_" + ctx.GetAD_User_Name();
                string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");
                storedPath += clientName;
                VLogMgt.Initialize(true, storedPath);


                MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
                res = Doc.PostImmediate(ass, AD_Table_ID, Record_ID, force, null);
                if (res == null || res.Trim().Length > 0)
                {
                    res = "OK";
                }
            }
            catch (Exception ex)
            {
                res += ex.Message;
            }
            return Json(new { result = res }, JsonRequestBehavior.AllowGet);
        }
    }
}