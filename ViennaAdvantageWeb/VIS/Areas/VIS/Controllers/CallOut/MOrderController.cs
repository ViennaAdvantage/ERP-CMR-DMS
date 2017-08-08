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
    public class MOrderController:Controller
    {
        //
        // GET: /VIS/MOrder/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetOrder(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetOrder(ctx,fields));
            }      
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}