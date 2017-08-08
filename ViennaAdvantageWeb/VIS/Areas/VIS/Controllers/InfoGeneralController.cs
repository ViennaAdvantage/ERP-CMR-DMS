using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
namespace VIS.Controllers
{
    public class InfoGeneralController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetSearchColumns(string tableName)
        {
            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();
            
            return Json(new { result = model.GetSchema(tableName)}, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetDispalyColumns(int AD_Table_ID)
        {
            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();

            return Json(new { result = model.GetDisplayCol(AD_Table_ID) }, JsonRequestBehavior.AllowGet);
            //return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetData(string sql,string tableName)
        {
            VIS.Models.InfoGeneralModel model = new Models.InfoGeneralModel();
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetData(sql, tableName, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

    }
}