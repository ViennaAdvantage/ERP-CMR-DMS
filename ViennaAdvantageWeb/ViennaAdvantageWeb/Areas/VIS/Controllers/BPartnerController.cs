using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;
using VIS.Models;
//namespace ViennaAdvantageWeb.Areas.VIS.Controllers
namespace VIS.Controllers
{
    public class BPartnerController : Controller
    {
        //
        // GET: /VIS/BPartner/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetBPartner(string param)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = param.Split(',');
                int C_BPartner_ID;

                //Assign parameter value
                C_BPartner_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MBPartner bpartner = new MBPartner(ctx, C_BPartner_ID, null);



                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment




                retDic["M_ReturnPolicy_ID"] = bpartner.GetM_ReturnPolicy_ID().ToString();

                retDic["M_ReturnPolicy_ID"] = bpartner.GetPO_ReturnPolicy_ID().ToString();

                //retDic["DateOrdered", order.GetDateOrdered());


                retJSON = JsonConvert.SerializeObject(retDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InitBP(int WinNo, int bPartnerID, string bpType)
        {
            ViewBag.WindowNumber = WinNo;
            Ctx ctx = Session["ctx"] as Ctx;
            BPartnerModel objBPModel = new BPartnerModel(WinNo, bPartnerID, bpType, ctx);

            return Json(JsonConvert.SerializeObject(objBPModel), JsonRequestBehavior.AllowGet);
        }
        [HttpPost, ValidateInput(false)]
        public JsonResult AddBPartnerInfo(int C_BPartner_ID, string searchKey, string name, string name2, string greeting, string bpGroup, string bpRelation, string bpLocation, string contact, string greeting1, string title, string email, string address, string phoneNo, string phoneNo2, string fax, int windowNo, string BPtype, bool isCustomer, bool isVendor)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            BPartnerModel objContactModel = new BPartnerModel();
            string resultMsg = string.Empty;
            //searchKey = Server.HtmlEncode(searchKey);
            //name = Server.HtmlEncode(name);
            //name2 = Server.HtmlEncode(name2);
            //contact = Server.HtmlEncode(contact);
            //email = Server.HtmlEncode(email);
            //title = Server.HtmlEncode(title);
            //phoneNo = Server.HtmlEncode(phoneNo);
            //phoneNo2 = Server.HtmlEncode(phoneNo2);
            //fax = Server.HtmlEncode(fax);
            if (C_BPartner_ID > 0)
            {
                resultMsg = objContactModel.AddBPartner(searchKey, name, name2, greeting, bpGroup, bpRelation, bpLocation, contact, greeting1, title, email, address, phoneNo, phoneNo2, fax, ctx, windowNo, BPtype, C_BPartner_ID, isCustomer, isVendor); // Update Business Partner
            }
            else
            {
                resultMsg = objContactModel.AddBPartner(searchKey, name, name2, greeting, bpGroup, bpRelation, bpLocation, contact, greeting1, title, email, address, phoneNo, phoneNo2, fax, ctx, windowNo, BPtype, C_BPartner_ID, isCustomer, isVendor);// Add New Business Partner
            }
            if (resultMsg != string.Empty)
            {
                return Json(JsonConvert.SerializeObject(resultMsg), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject(VAdvantage.Utility.Msg.GetMsg((string)ViewBag.culture, "RecordSave")), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Business Partner Location 
        /// </summary>
        /// <param name="WinNo"></param>
        /// <param name="bpartnerID"></param>
        /// <returns></returns>
        public JsonResult GetBPLocation(int WinNo, int bpartnerID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            BPartnerModel objContactModel = new BPartnerModel();
            objContactModel.FillBPLocation(bpartnerID, ctx);
            return Json(JsonConvert.SerializeObject(objContactModel), JsonRequestBehavior.AllowGet);
        }
    }
}
