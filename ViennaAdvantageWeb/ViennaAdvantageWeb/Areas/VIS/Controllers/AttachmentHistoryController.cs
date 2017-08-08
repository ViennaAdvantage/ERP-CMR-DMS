using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class AttachmentHistoryController : Controller
    {
        //
        // GET: /VIS/AttachmentHistory/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult RelatedHistory(int keyColumnID, int pageSize, int pageNo, string searchText, string keyColName)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            RealtedHistoryInfoDetails hisIfno = model.history(keyColumnID, pageSize, pageNo, ct, searchText, keyColName);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserHistory(int C_BPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            RealtedHistoryInfoDetails hisIfno = model.Userhistory(C_BPartner_ID, pageSize, pageNo, ct, searchText);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadSentMails(int ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            MailInfo hisIfno = model.Sentmails(ID, ct);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadInboxMails(int ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            MailInfo hisIfno = model.InboxMails(ID, ct);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadLetters(int ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel model = new AttachmentHistoryModel();
            MailInfo hisIfno = model.Letters(ID, ct);
            return Json(JsonConvert.SerializeObject(hisIfno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadAttachment(int ID, string Name)
        {
            Ctx ct = Session["ctx"] as Ctx;
            Name = Server.HtmlDecode(Name);
            MMailAttachment1 _mAttachment = new MMailAttachment1(ct, ID, null);
            string path = "";
            var fName = "";
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                if (Name.ToUpper() == oMAttachEntry.GetName().ToUpper())
                {
                    fName = DateTime.Now.Ticks.ToString() + Name;
                    path = Path.Combine(Server.MapPath("~/TempDownload"), fName);
                    byte[] bytes = oMAttachEntry.GetData();

                    using (FileStream fs = new FileStream(path, FileMode.Append, System.IO.FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                    break;
                }
            }

            return Json(JsonConvert.SerializeObject(fName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ViewChatonHistory(int record_ID, bool isAppointment)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            List<ChatInfos> cInfo = his.ViewChatonHistory(ct, record_ID, isAppointment);

            return Json(JsonConvert.SerializeObject(cInfo), JsonRequestBehavior.AllowGet);

        }

        public JsonResult SaveComment(int ID, string Text, bool isAppointment)
        {
            Ctx ct = Session["ctx"] as Ctx;
            AttachmentHistoryModel his = new AttachmentHistoryModel();
            return Json(JsonConvert.SerializeObject(his.SaveComment(ID, Text, isAppointment, ct)), JsonRequestBehavior.AllowGet);
        }


    }
}