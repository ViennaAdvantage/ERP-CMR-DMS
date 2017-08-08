using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Filters;
namespace VIS.Controllers
{
    public class InfoProductController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetInfoColumns(string tableName)
        {
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetInfoColumns(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetData(string sql, string tableName)
        {
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetData(sql, tableName, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]

        public JsonResult Save(int id, string keyColumn, string prod, string listAst, string qty, string qtyBook, string ordlineID, int ordID, string listLoc, int lineID)
        {
            List<string> prodID = new List<string>();
            if (prod != null && prod.Trim().Length > 0)
            {
                prodID = JsonConvert.DeserializeObject<List<string>>(prod);
            }
            List<string> Attributes = new List<string>();
            if (listAst != null && listAst.Trim().Length > 0)
            {
                Attributes = JsonConvert.DeserializeObject<List<string>>(listAst);
            }
            List<string> quantity = new List<string>();
            if (qty != null && qty.Trim().Length > 0)
            {
                quantity = JsonConvert.DeserializeObject<List<string>>(qty);
            }
            List<string> qtybook = new List<string>();
            if (qtyBook != null && qtyBook.Trim().Length > 0)
            {
                qtybook = JsonConvert.DeserializeObject<List<string>>(qtyBook);
            }
            List<string> olineID = new List<string>();
            if (ordlineID != null && ordlineID.Trim().Length > 0)
            {
                olineID = JsonConvert.DeserializeObject<List<string>>(ordlineID);
            }
            List<string> Locators = new List<string>();
            if (listLoc != null && listLoc.Trim().Length > 0)
            {
                Locators = JsonConvert.DeserializeObject<List<string>>(listLoc);
            }
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            var value = model.SetProductQty(id, keyColumn, prodID, Attributes, quantity, qtybook, olineID, ordID, Locators, lineID, Session["ctx"] as Ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]

        public JsonResult Save1(int id, string keyColumn, string prod, string listAst, string qty, string ordlineID, string listLoc, int locatorTo, string astID, int lineID)
        {
            List<string> prodID = new List<string>();
            if (prod != null && prod.Trim().Length > 0)
            {
                prodID = JsonConvert.DeserializeObject<List<string>>(prod);
            }
            List<string> Attributes = new List<string>();
            if (listAst != null && listAst.Trim().Length > 0)
            {
                Attributes = JsonConvert.DeserializeObject<List<string>>(listAst);
            }
            List<string> quantity = new List<string>();
            if (qty != null && qty.Trim().Length > 0)
            {
                quantity = JsonConvert.DeserializeObject<List<string>>(qty);
            }
            List<string> olineID = new List<string>();
            if (ordlineID != null && ordlineID.Trim().Length > 0)
            {
                olineID = JsonConvert.DeserializeObject<List<string>>(ordlineID);
            }
            List<string> Locators = new List<string>();
            if (listLoc != null && listLoc.Trim().Length > 0)
            {
                Locators = JsonConvert.DeserializeObject<List<string>>(listLoc);
            }
            List<string> assetid = new List<string>();
            if (astID != null && astID.Trim().Length > 0)
            {
                assetid = JsonConvert.DeserializeObject<List<string>>(listLoc);
            }
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            var value = model.SetProductQty1(id, keyColumn, prodID, Attributes, quantity, olineID, Locators, locatorTo, assetid, lineID, Session["ctx"] as Ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetAttribute(string fields)
        {
            KeyNamePair retJSON = null;
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                VIS.Models.InfoProductModel obj = new VIS.Models.InfoProductModel();
                retJSON = obj.GetAttribute(ctx, fields);
            }
            return Json( JsonConvert.SerializeObject(retJSON), JsonRequestBehavior.AllowGet);            
        }
    }
}