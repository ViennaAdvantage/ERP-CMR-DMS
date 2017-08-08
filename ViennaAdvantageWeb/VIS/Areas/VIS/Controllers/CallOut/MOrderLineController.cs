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
    public class MOrderLineController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetOrderLine(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderLineModel objOrderLine = new MOrderLineModel();
                retJSON = JsonConvert.SerializeObject(objOrderLine.GetOrderLine(ctx, fields));
            }         
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNotReserved(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderLineModel objOrderLine = new MOrderLineModel();
                retJSON = JsonConvert.SerializeObject(objOrderLine.GetNotReserved(ctx, fields));
            }           
            return Json(retJSON, JsonRequestBehavior.AllowGet);

        }
    }
}