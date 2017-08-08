using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class VImageFormController : Controller
    {
        //
        // GET: /VIS/VImageForm/
        public ActionResult Index(string windowno, int ad_image_id)
        {
            VImageModel obj = new VImageModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                obj = obj.GetImage(ctx, Convert.ToInt32(ad_image_id), 0);
            }

            ViewBag.WindowNumber = windowno;
            return PartialView(obj);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult SaveImage(HttpPostedFileBase file, bool isDatabaseSave, string ad_image_id)
        {
            if (file == null)
            {
                Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

            VImageModel obj = new VImageModel();
            var value = 0;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                if (!Directory.Exists(Path.Combine(Server.MapPath("~/Images"), "RecordImages")))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("~/Images"), "RecordImages"));
                }
                value = obj.SaveImage(ctx, Server.MapPath("~/Images/RecordImages"), file, Convert.ToInt32(ad_image_id), isDatabaseSave);
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetImageAsByte(int ad_image_id)
        {
            VImageModel obj = new VImageModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                obj = obj.GetImage(ctx, Convert.ToInt32(ad_image_id), 16);
            }
            return Json(new { result = obj.UsrImage }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFileByteArray(HttpPostedFileBase file)
        {
            VImageModel obj = new VImageModel();
            var value = obj.GetArrayFromFile(Server.MapPath("~/Images/RecordImages"), file);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }
    }
}