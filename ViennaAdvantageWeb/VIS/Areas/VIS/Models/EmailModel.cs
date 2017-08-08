/********************************************************
 * Project Name   : VIS
 * Class Name     : EmailModel
 * Purpose        : Used to perform server side tasks related to  email and letters...
 * Chronological    Development
 * Karan            
  ******************************************************/


using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.Mvc;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.pdf;
using VAdvantage.DataBase;
using VAdvantage.MailBox;
using VAdvantage.Model;
using VAdvantage.Utility;
using ViennaAdvantageWeb.Areas.VIS.Models;
using System.ComponentModel;
using iTextSharp.text.html.simpleparser;

namespace VIS.Models
{
    public class EmailModel
    {
        Ctx ctx = null;

        string AttachmentsUploadFolderName = "TempDownload";
        // UserInformation userinfo = null;

        public EmailModel(Ctx ctx)
        {
            this.ctx = ctx;
        }


        /// <summary>
        /// Used to send mails .... Fetechs credentails used to send mails...
        /// </summary>
        /// <param name="mails"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="attachment_ID"></param>
        /// <param name="fileNames"></param>
        /// <param name="fileNameForOpenFormat"></param>
        /// <param name="mailFormat"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public string SendMails(List<NewMailMessage> mails, int AD_User_ID, int AD_Client_ID, int AD_Org_ID, int attachment_ID, List<string> fileNames, List<string> fileNameForOpenFormat, string mailFormat, bool notify)
        {

            


            VAdvantage.Utility.EMail sendmail = new VAdvantage.Utility.EMail(ctx, "", "", "", "", "", "",
                  true, false);
            string isConfigExist = sendmail.IsConfigurationExist(ctx);
            if (isConfigExist != "OK")
            {
                return isConfigExist;
            }


            if (notify)//if want to send mail on server and want notice on home screen.     Else u have to wait, and it will show alert message of return value....
            {
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    SendMailstart(mails, AD_User_ID, AD_Client_ID, AD_Org_ID, attachment_ID, fileNames, fileNameForOpenFormat, mailFormat, notify, sendmail);
                });
                return "";
            }
            else
            {
                return SendMailstart(mails, AD_User_ID, AD_Client_ID, AD_Org_ID, attachment_ID, fileNames, fileNameForOpenFormat, mailFormat, notify, sendmail);
            }
        }

        /// <summary>
        /// this method actually send mail, both static and dynamic.... and save info in MailAttachment....
        /// </summary>
        /// <param name="mails"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="attachment_ID"></param>
        /// <param name="fileNames"></param>
        /// <param name="fileNameForOpenFormat"></param>
        /// <param name="mailFormat"></param>
        /// <param name="notify"></param>
        /// <returns></returns>
        public string SendMailstart(List<NewMailMessage> mails, int AD_User_ID, int AD_Client_ID, int AD_Org_ID, int attachment_ID, List<string> fileNames, List<string> fileNameForOpenFormat, string mailFormat, bool notify, VAdvantage.Utility.EMail sendmail)
        {

            if (ctx == null)
            {
                return null;
            }


            UserInformation userinfo = new UserInformation();
            SMTPConfig config = null;
            config = MailConfigMethod.GetUSmtpConfig(AD_User_ID, ctx);
            // var config = "";
            if (config == null)
            {
                MClient client = new MClient(ctx, AD_Client_ID, null);
                userinfo.Email = client.GetRequestEMail();
            }
            else
            {
                //Add user info to list..
                userinfo.Email = config.Email;
            }

            string[] to = null;
            string[] bc = null;

            string[] cc = null;
            string sub = null;
            string message = null;
            // int _record_id = 0;
            int _table_id = 0;

            string[] records = null;

            StringBuilder res = new StringBuilder();

            List<NewMailMessage> mail = mails.GetRange(0, mails.Count);

            for (int j = 0; j < mails.Count; j++)
            {
                to = mails[j].To.Split(';');
                bc = mails[j].Bcc;
                cc = mails[j].Cc.Split(';');
                StringBuilder bcctext = new StringBuilder();
                sub = mails[j].Subject;
                message = mailFormat;
                if (mails[j].Body != null && mails[j].Body.Count > 0)
                {
                    List<string> keysss = mails[j].Body.Keys.ToList();
                    for (int q = 0; q < keysss.Count; q++)
                    {
                        message = message.Replace(keysss[q], mails[j].Body[keysss[q]]);
                    }
                }

                if (mails[j].Recordids != null)          //in case of static mail
                {
                    records = mails[j].Recordids.Split(',');
                }

                _table_id = Convert.ToInt32(mail[j].TableID);

                VAdvantage.Model.MMailAttachment1 _mAttachment = new VAdvantage.Model.MMailAttachment1(ctx, 0, null);


                sendmail.SetSubject(sub);
                sendmail.SetMessageHTML(message);


                //used to get attachments uploaded by user.....
                if (mail[j].AttachmentFolder != null && mail[j].AttachmentFolder.Trim().Length > 0)
                {
                    string storedAttachmentPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, AttachmentsUploadFolderName + "\\" + mail[j].AttachmentFolder);
                    if (Directory.Exists(storedAttachmentPath))
                    {
                        DirectoryInfo info = new DirectoryInfo(storedAttachmentPath);
                        if (info.GetFiles().Length > 0)
                        {
                            FileInfo[] files = info.GetFiles();

                            for (int a = 0; a < files.Length; a++)
                            {
                                if (fileNames.Contains(files[a].Name))
                                {
                                    FileStream attachmentStream = File.OpenRead(files[a].FullName);
                                    BinaryReader binary = new BinaryReader(attachmentStream);
                                    byte[] buffer = binary.ReadBytes((int)attachmentStream.Length);
                                    sendmail.AddAttachment(buffer, files[a].Name);
                                    _mAttachment.AddEntry(files[a].Name, buffer);
                                }
                            }
                        }
                    }
                }

                //used to get attachments of saved formats.. Currently not Supporting......
                if (attachment_ID > 0)
                {
                    VAdvantage.Model.MMailAttachment1 _mAttachment1 = new VAdvantage.Model.MMailAttachment1(ctx, attachment_ID, null);
                    if (_mAttachment1.GetEntryCount() > 0)
                    {
                        MAttachmentEntry[] entries = _mAttachment1.GetEntries();
                        for (int m = 0; m < entries.Count(); m++)
                        {
                            //if (fileNameForOpenFormat.Contains(entries[m].GetName()))
                            //{
                            byte[] buffer = entries[m].GetData();
                            sendmail.AddAttachment(buffer, entries[m].GetName());
                            _mAttachment.AddEntry(entries[m].GetName(), buffer);
                            //}
                        }
                    }
                }

                if (to != null)
                {

                    for (int i = 0; i < to.Length; i++)
                    {
                        if (to[i].ToString() != "")
                        {
                            sendmail.AddTo(to[i].ToString(), "");
                            // totext.Append(to[i].ToString() + ",");
                        }
                    }
                }

                if (bc != null)
                {
                    for (int i = 0; i < bc.Length; i++)
                    {
                        if (bc[i].ToString() != "")
                        {
                            sendmail.AddBcc(bc[i].ToString(), "");
                            bcctext.Append(bc[i].ToString() + ",");
                        }
                    }
                }

                if (cc != null)
                {
                    for (int i = 0; i < cc.Length; i++)
                    {
                        if (cc[i].ToString() != "")
                        {
                            sendmail.AddCc(cc[i].ToString(), "");
                            ///  cctext.Append(cc[i].ToString() + ",");
                        }
                    }
                }

                string res1 = sendmail.Send();

                if (records != null && records.Length > 0)//save entery in MailAttachment......
                {
                    for (int k = 0; k < records.Length; k++)
                    {
                        if (records[k] == null || records[k] == "" || records[k] == "0")
                        {
                            continue;
                        }
                        if (res1 != "OK")
                        {
                            _mAttachment.SetIsMailSent(false);
                        }
                        else
                        {
                            _mAttachment.SetIsMailSent(true);
                        }
                        int AD_Client_Id = ctx.GetAD_Client_ID();
                        int iOrgid = ctx.GetAD_Org_ID();

                        _mAttachment.SetAD_Client_ID(AD_Client_Id);
                        _mAttachment.SetAD_Org_ID(iOrgid);
                        _mAttachment.SetAD_Table_ID(_table_id);
                        _mAttachment.IsActive();
                        _mAttachment.SetMailAddress(bcctext.ToString());
                        _mAttachment.SetAttachmentType("M");

                        _mAttachment.SetRecord_ID(Convert.ToInt32(records[k]));

                        _mAttachment.SetTextMsg(message);
                        _mAttachment.SetTitle(sub);

                        _mAttachment.SetMailAddressBcc(bcctext.ToString());
                        _mAttachment.SetMailAddress(mails[j].To);
                        _mAttachment.SetMailAddressCc(mails[j].Cc);
                        _mAttachment.SetMailAddressFrom(userinfo.Email);
                        if (_mAttachment.GetEntries().Length > 0)
                        {
                            _mAttachment.SetIsAttachment(true);
                        }
                        else
                        {
                            _mAttachment.SetIsAttachment(false);
                        }
                        _mAttachment.NewRecord();
                        if (_mAttachment.Save())
                        { }
                        else
                        {
                            // log.SaveError(Msg.GetMsg(Env.GetCtx(), "RecordNotSaved"), "");
                        }
                    }
                }

                if (res1 != "OK")           // if mail not sent....
                {
                    if (res1 == "AuthenticationFailed.")
                    {
                        res.Append("AuthenticationFailed");
                        return res.ToString();
                    }
                    else if (res1 == "ConfigurationIncompleteOrNotFound")
                    {
                        res.Append("ConfigurationIncompleteOrNotFound");
                        return res.ToString();
                    }
                    else
                    {
                        res.Append(" " + Msg.GetMsg(ctx, "MailNotSentTo") + ": " + mails[j].To + " " + bcctext + " " + mails[j].Cc);
                    }


                }
                else
                {
                    {
                        if (!res.ToString().Contains("MailSent"))
                        {
                            res.Append("MailSent");
                        }
                    }

                }
                bcctext = null;
            }

            if (notify)             //  make an entry in Notice window.....
            {
                MNote note = new MNote(ctx, "SentMailNotice", AD_User_ID,
                    AD_Client_ID, AD_Org_ID, null);
                //  Reference
                note.SetReference(ToString());	//	Document
                //	Text
                note.SetTextMsg(res.ToString());
                note.Save();
            }

            userinfo = null;
            cc = null;
            bc = null;
            to = null; records = null;
            sub = null; message = null;
            records = null;
            return res.ToString();
        }



        /// <summary>
        /// Save new mail formats and update existing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="AD_Client_ID"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="name"></param>
        /// <param name="isDynamic"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="saveforAll"></param>
        /// <param name="AD_Window_ID"></param>
        /// <param name="folder"></param>
        /// <param name="attachmentID"></param>
        /// <returns></returns>
        public int SaveFormats(int id, int AD_Client_ID, int AD_Org_ID, string name, bool isDynamic, string subject, string text, bool saveforAll, int AD_Window_ID, string folder, int attachmentID)
        {
            X_AD_TextTemplate _textTemplate = new X_AD_TextTemplate(ctx, id, null);
            _textTemplate.Set_Value("AD_Client_ID", AD_Client_ID);
            _textTemplate.Set_Value("AD_Org_ID", AD_Org_ID);
            _textTemplate.Set_Value("Name", name);
            _textTemplate.Set_Value("IsActive", true);
            _textTemplate.Set_Value("IsHtml", true);
            _textTemplate.Set_Value("IsDynamicContent", isDynamic);
            if (subject.Trim().Length > 0)
            {
                _textTemplate.Set_Value("Subject", subject);
            }
            else
            {
                _textTemplate.Set_Value("Subject", name);
            }

            _textTemplate.Set_Value("MailText", text);

            if (!saveforAll)
            {
                _textTemplate.Set_Value("AD_Window_ID", AD_Window_ID);
            }
            if (_textTemplate.Save())
            {
                //MMailAttachment1 mAttachment = new MMailAttachment1(ctx, attachmentID, null);
                //mAttachment.SetAD_Client_ID(AD_Client_ID);
                //mAttachment.SetAD_Org_ID(AD_Org_ID);
                //mAttachment.SetAD_Table_ID(_textTemplate.Get_Table_ID());
                //mAttachment.IsActive();
                //mAttachment.SetAttachmentType("Z");
                ////get first key coloumn
                //mAttachment.SetRecord_ID(_textTemplate.GetAD_TextTemplate_ID());
                //mAttachment.SetTextMsg(text);
                //mAttachment.SetTitle(name);
                //FileInfo[] files = null;
                //if (folder != null && folder.Trim().Length > 0)
                //{
                //    string storedAttachmentPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, AttachmentsUploadFolderName + "\\" + folder);
                //    if (Directory.Exists(storedAttachmentPath))
                //    {
                //        DirectoryInfo info = new DirectoryInfo(storedAttachmentPath);
                //        if (info.GetFiles().Length > 0)
                //        {
                //            files = info.GetFiles();

                //            for (int a = 0; a < files.Length; a++)
                //            {

                //                FileStream attachmentStream = File.OpenRead(files[a].FullName);
                //                BinaryReader binary = new BinaryReader(attachmentStream);
                //                byte[] buffer = binary.ReadBytes((int)attachmentStream.Length);
                //                mAttachment.AddEntry(files[a].Name, buffer);
                //                if (mAttachment.Save())
                //                {

                //                }
                //            }
                //        }
                //    }
                //}


                //if (files == null || files.Length == 0)
                //{
                //    mAttachment.AddEntry("", null);
                //}
                //files = null;
                return _textTemplate.GetAD_TextTemplate_ID();
            }
            return 0;
        }

        //public SavedAttachmentInfo SavedAttachmentForFormat(int textTemplate_ID)
        //{
        //    List<string> entry = new List<string>();
        //    string sql = "select MailAttachment1_ID from MailAttachment1 where ad_table_id=(Select AD_Table_ID from AD_Table where tablename='AD_TextTemplate') and record_id=" + textTemplate_ID;
        //    int attach = Util.GetValueOfInt(DB.ExecuteScalar(sql));

        //    MMailAttachment1 mattach = new MMailAttachment1(ctx, attach, null);
        //    MAttachmentEntry[] entries = mattach.GetEntries();

        //    List<AttachmentInfo> aifo = new List<AttachmentInfo>();

        //    if (entries.Count() > 0)
        //    {
        //        for (int i = 0; i < entries.Count(); i++)
        //        {
        //            AttachmentInfo inf = new AttachmentInfo();
        //            inf.Name = entries[i].GetName();
        //            inf.Size = entries[i].GetData().Length.ToString();
        //            aifo.Add(inf);
        //        }
        //    }

        //    SavedAttachmentInfo info = new SavedAttachmentInfo();
        //    info.FileNames = aifo;
        //    info.AttachmentID = mattach.GetMailAttachment1_ID();

        //    return info;
        //}



        /// <summary>
        /// used to convert html to pdf ... mainly does makes a string of html with values of fields selected in mail during dynamci mails, otherewise created simple html...
        /// </summary>
        /// <param name="html"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public string HtmlToPdf(string html, List<Dictionary<string, string>> values)
        {
            //string html = System.IO.File.ReadAllText(path);

            StringBuilder sHtml = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                string copy = html;
                List<string> keysss = values[i].Keys.ToList();
                for (int q = 0; q < keysss.Count; q++)
                {
                    copy = copy.Replace(keysss[q], values[i][keysss[q]]);
                }
                sHtml.Append(copy).Append("~");
            }

            byte[] arrays = HtmlToPdfbytes(sHtml.ToString());
            return Convert.ToBase64String(arrays);
        }


        /// <summary>
        /// Actually convert HTMl to PDF and return byte[]...
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public byte[] HtmlToPdfbytes(string html)
        {
            StringWriter sw = new StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(sw);

            string htmlContent = "";
            if (ctx.GetIsRightToLeft())
            {
                htmlContent = @"<html dir='rtl'>
                <body>";
            }
            else
            {
                htmlContent = @"<html>
                <body>";
            }

            htmlContent += html;
            html += "</html></body>";
            string[] htmls = html.Split('~');

            byte[] docArray = null;

            //if (ctx.GetIsRightToLeft())
            //{
            //    Document document = new Document();
            //    using (var memoryStream = new MemoryStream())
            //    {
            //        try
            //        {
            //            document = new Document(PageSize.A4, 50, 50, 50, 50);
            //            // step 2
            //            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            //            // step 3
            //            document.Open();

            //            BaseFont bf = BaseFont.CreateFont("c:\\windows\\fonts\\arabtype.ttf", BaseFont.IDENTITY_H, true);
            //            iTextSharp.text.

            //            Font f2 = new iTextSharp.text.Font(bf, 24, iTextSharp.text.Font.NORMAL, iTextSharp.text.Color.BLUE);


            //            String atext = "صصصث  خ8 صث اهلا";
            //            PdfPTable table = new PdfPTable(1);
            //            table.RunDirection =

            //            PdfWriter.RUN_DIRECTION_RTL;
            //            PdfPCell cell = new PdfPCell(new Phrase(10, atext, f2));
            //            table.AddCell(cell);

            //            document.Add(table);

            //            document.Close();

            //        }
            //        catch (DocumentException de)
            //        {
            //            //              this.Message = de.Message;
            //        }
            //        catch (IOException ioe)
            //        {
            //            //                this.Message = ioe.Message;
            //        }

            //        // step 5: we close the document
            //        document.Close();

            //        docArray = memoryStream.ToArray();

            //    }



            //}
            //else
            //{

            FontFactory.Register("c:/windows/fonts/arabtype.TTF");
            StyleSheet style = new StyleSheet();
            style.LoadTagStyle("body", "face", "Arabic Typesetting");
            style.LoadTagStyle("body", "encoding", BaseFont.IDENTITY_H);
            style.LoadTagStyle("body", "direction", "RTL");

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ar-IQ");
            using (var memoryStream = new MemoryStream())
            {
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4.Rotate());
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    writer.CloseStream = false;

                    if (ctx.GetIsRightToLeft())
                    {
                        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                    }

                    // Write PDF here.

                    document.Open();


                    //iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(document);

                    //for (int i = 0; i < htmls.Count(); i++)
                    //{
                    //   // hw.Style = style;
                    //    hw.Parse(new StringReader(htmls[i]));
                    //    document.NewPage();
                    //}


                    for (int i = 0; i < htmls.Count(); i++)
                    {
                        var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(htmls[i]), style);
                        foreach (var htmlElement in parsedHtmlElements)
                        {
                            document.Add(htmlElement as IElement);
                        }

                        document.NewPage();
                    }




                    document.Close();

                }

                docArray = memoryStream.ToArray();
            }

            //}


            return docArray;

        }

        private static void SetDirection(PdfPTable tbl)
        {
            tbl.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            tbl.HorizontalAlignment = Element.ALIGN_LEFT;
            foreach (PdfPRow pr in tbl.Rows)
            {
                foreach (PdfPCell pc in pr.GetCells())
                {
                    if (pc != null)
                    {
                        pc.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                        pc.HorizontalAlignment = Element.ALIGN_LEFT;
                        if (pc.CompositeElements != null)
                        {
                            foreach (var element in pc.CompositeElements)
                            {
                                if (element is PdfPTable)
                                {
                                    SetDirection((PdfPTable)element);
                                }
                            }
                        }
                    }
                }
            }
        }



        //Used to save letters as Attachment.... User can see saved letteres from History Form.......
        public string SaveAttachment(string subject, int AD_Table_ID, string html, Dictionary<string, Dictionary<string, string>> values)
        {
            StringBuilder strb = new StringBuilder();
            StringBuilder sHtml = new StringBuilder();
            //for (int i = 0; i < values.Count; i++)
            //{
            List<string> keysss = values.Keys.ToList();
            for (int q = 0; q < keysss.Count; q++)
            {
                string copy = html;

                Dictionary<string, string> val = values[keysss[q]];
                List<string> valKeys = val.Keys.ToList();

                for (int c = 0; c < valKeys.Count; c++)
                {
                    copy = copy.Replace(valKeys[c], val[valKeys[c]]);
                }

                byte[] arrays = HtmlToPdfbytes(copy);

                int AD_Client_Id = ctx.GetAD_Client_ID();
                int iOrgid = ctx.GetAD_Org_ID();
                MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, 0, null);
                _mAttachment.AddEntry(subject + ".pdf", arrays);
                _mAttachment.SetAD_Client_ID(AD_Client_Id);
                _mAttachment.SetAD_Org_ID(iOrgid);
                _mAttachment.SetAD_Table_ID(AD_Table_ID);
                _mAttachment.IsActive();
                _mAttachment.SetAttachmentType("L");
                //get first key coloumn

                _mAttachment.SetRecord_ID(Util.GetValueOfInt(keysss[q]));
                _mAttachment.SetTextMsg(copy);
                _mAttachment.SetTitle(subject);
                _mAttachment.NewRecord();
                if (_mAttachment.Save())
                {
                }




            }
            //}


            return strb.ToString();
        }



    }



    public class NewMailMessage
    {
        public string Subject { get; set; }

        public Dictionary<string, string> Body { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Cc { get; set; }

        public string Sender { get; set; }

        public string[] Bcc { get; set; }

        public DateTime Date { get; set; }

        public bool IsHtml { get; set; }

        public int TableID { get; set; }

        //  public AttachmentInfo[] Attachments { get; set; }

        public string Recordids { get; set; }

        public string AttachmentFolder { get; set; }
    }



    public class KeyValues
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }



    public class SavedAttachmentInfo
    {
        public List<AttachmentInfo> FileNames { get; set; }
        public int AttachmentID { get; set; }
    }

    public class AttachmentInfo
    {
        public string Name { get; set; }
        public string Size { get; set; }
    }


}