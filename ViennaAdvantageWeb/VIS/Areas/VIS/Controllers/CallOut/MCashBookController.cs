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
    public class MCashBookController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //Get CashBook Detail
        public JsonResult GetCashBook(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MCashBookModel objCashBookModel = new MCashBookModel();
                retJSON = JsonConvert.SerializeObject(objCashBookModel.GetCashBook(ctx,fields));
            }
          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}