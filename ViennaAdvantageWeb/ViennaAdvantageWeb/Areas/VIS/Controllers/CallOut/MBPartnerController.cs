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
    public class MBPartnerController : Controller
    {
        //
        // GET: /VIS/BPartner/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetBPartner(string fields)
        {            
            string retJSON = "";      
            VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPartner(ctx,fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}