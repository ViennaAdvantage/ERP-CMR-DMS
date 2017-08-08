using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class WFActivityController : Controller
    {
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetActivities(int pageNo, int pageSize,bool refresh)
        {
            WFActivityModel model = new WFActivityModel();
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(new { result = model.GetActivities(ctx, ctx.GetAD_User_ID(), ctx.GetAD_Client_ID(), pageNo, pageSize, refresh) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetActivityInfo(int activityID, int nodeID, int wfProcessID)
        {
            WFActivityModel model = new WFActivityModel();
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(new { result = model.GetActivityInfo(activityID,nodeID,wfProcessID ,ctx) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult ApproveIt(int activityID, int nodeID, string txtMsg, object fwd, object answer)
        {
            WFActivityModel model = new WFActivityModel();
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(new { result = model.ApproveIt(nodeID,activityID, txtMsg,fwd,answer, ctx) }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetRelativeData(int activityID)
        {
            WFActivityModel model = new WFActivityModel();
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(new { result = model.GetRelativeData(ctx, activityID) }, JsonRequestBehavior.AllowGet);
        }

    }
}