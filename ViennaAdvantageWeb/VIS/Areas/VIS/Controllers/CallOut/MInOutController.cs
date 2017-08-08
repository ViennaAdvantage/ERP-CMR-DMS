using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MInOutController:Controller
    {
        //
        // GET: /VIS/InOut/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetInOut(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                retJSON = JsonConvert.SerializeObject(objInOut.GetInOut(ctx,fields));
            }          
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
       
    }
}