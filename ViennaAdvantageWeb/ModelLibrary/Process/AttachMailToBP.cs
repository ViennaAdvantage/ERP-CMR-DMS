using Limilabs.Client.IMAP;
using Limilabs.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Process
{
    public class AttachMailToBP : ProcessEngine.SvrProcess
    {
        int AD_User_ID = 0;
        int AD_Client_ID = 0;
        int AD_Org_ID = 0;
        private Imap imapMail;
        string sender = "contacts";
        string folderName = "Inbox";


        private StringBuilder retVal = new StringBuilder();

        protected override void Prepare()
        {
            //ProcessInfoParameter[] para = GetParameter();
            //for (int i = 0; i < para.Length; i++)
            //{
            //    String name = para[i].GetParameterName();
            //    if (para[i].GetParameter() == null)
            //    {
            //        ;
            //    }
            //    else if (name.Equals("AD_User_ID"))
            //    {
            //        AD_User_ID = para[i].GetParameterAsInt();
            //    }
            //    else
            //    {
            //        log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            //    }
            //}
        }

        protected override string DoIt()
        {
            string sql = @"SELECT umail.imaphost,
                                  umail.imapisssl,
                                  umail.imappassword,
                                  umail.imapport,
                                  umail.imapusername,
                                  umail.AD_User_ID,
                                  umail.AD_CLient_ID,
                                  umail.AD_Org_ID
                                FROM ad_usermailconfigration umail
                                WHERE umail.IsActive ='Y' ";

            if (AD_User_ID > 0)
            {
                sql += " AND umail.AD_User_ID=" + AD_User_ID;
            }
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                log.Log(Level.SEVERE, "No Config found");
                return "No Config found";
            }

            UserInformation user = null;


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                user = new UserInformation();
                if (ds.Tables[0].Rows[i]["imapusername"] != DBNull.Value && ds.Tables[0].Rows[i]["imapusername"] != null)
                {
                    user.Username = Convert.ToString(ds.Tables[0].Rows[i]["imapusername"]);
                }
                else
                {
                    log.Log(Level.SEVERE, "UserName not found for AD_User_ID=" + ds.Tables[0].Rows[i]["AD_User_ID"].ToString());
                    continue;
                }

                if (ds.Tables[0].Rows[i]["imappassword"] != DBNull.Value && ds.Tables[0].Rows[i]["imappassword"] != null)
                {
                    user.Password = Convert.ToString(ds.Tables[0].Rows[i]["imappassword"]);
                }
                else
                {
                    log.Log(Level.SEVERE, "password not found for AD_User_ID=" + ds.Tables[0].Rows[i]["AD_User_ID"].ToString());
                    continue;
                }

                if (ds.Tables[0].Rows[i]["imapisssl"] != DBNull.Value && ds.Tables[0].Rows[i]["imapisssl"] != null)
                {
                    user.UseSSL = Convert.ToString(ds.Tables[0].Rows[i]["imapisssl"]) == "Y" ? true : false;
                }
                else
                {
                    log.Log(Level.SEVERE, "SSL not found for AD_User_ID=" + ds.Tables[0].Rows[i]["AD_User_ID"].ToString());
                    continue;
                }

                if (ds.Tables[0].Rows[i]["imapport"] != DBNull.Value && ds.Tables[0].Rows[i]["imapport"] != null)
                {
                    user.HostPort = Convert.ToInt32(ds.Tables[0].Rows[i]["imapport"]);
                }
                else
                {
                    log.Log(Level.SEVERE, "imapport not found for AD_User_ID=" + ds.Tables[0].Rows[i]["AD_User_ID"].ToString());
                    continue;
                }

                if (ds.Tables[0].Rows[i]["imaphost"] != DBNull.Value && ds.Tables[0].Rows[i]["imaphost"] != null)
                {
                    user.Host = Convert.ToString(ds.Tables[0].Rows[i]["imaphost"]);
                }
                else
                {
                    log.Log(Level.SEVERE, "imaphost not found for AD_User_ID=" + ds.Tables[0].Rows[i]["AD_User_ID"].ToString());
                    continue;
                }


                if (ds.Tables[0].Rows[i]["AD_User_ID"] != DBNull.Value && ds.Tables[0].Rows[i]["AD_User_ID"] != null)
                {
                    AD_User_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_User_ID"]);
                }

                //AD_Client_ID
                if (ds.Tables[0].Rows[i]["AD_Client_ID"] != DBNull.Value && ds.Tables[0].Rows[i]["AD_Client_ID"] != null)
                {
                    AD_Client_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Client_ID"]);
                }

                if (ds.Tables[0].Rows[i]["AD_Org_ID"] != DBNull.Value && ds.Tables[0].Rows[i]["AD_Org_ID"] != null)
                {
                    AD_Org_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Org_ID"]);
                }

                if (AD_User_ID > 0)
                {

                     GetMails(user, AD_User_ID, AD_Client_ID, AD_Org_ID);

                }
            }



            return retVal.ToString();
        }

        private void GetMails(UserInformation user, int AD_User_ID, int AD_Client_ID, int AD_Org_ID)
        {
            string login = Login(user);
            if (login.Equals(""))
            {
                imapMail.SelectInbox();



                List<long> uidList = imapMail.SearchFlag(Flag.All);
                uidList.Reverse();


                //DocumentService ser = new DocumentService();
                byte[] bytes = null;
                string tableName = "AD_User";
                int _tableID = -1;
                int existRec = -1;
                StringBuilder attachmentID = new StringBuilder();
                foreach (long uid in uidList)
                {
                    try
                    {
                        Envelope structure = imapMail.GetEnvelopeByUID(uid);
                        string from = structure.From[0].Address;
                        try
                        {
                            string sql = "SELECT " + tableName + "_ID " + " , C_BPartner_ID " + "FROM " + tableName + " WHERE lower(Email) =" + "'" + from.Trim().ToLower() + "'";
                            sql += " AND AD_Client_ID=" + GetCtx().GetAD_Client_ID();
                            string finalSql = MRole.GetDefault(GetCtx(), false).AddAccessSQL(sql, tableName.ToString(), MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
                            IDataReader idr = DB.ExecuteReader(sql);//+ " order by ad_texttemplate_id");                    
                            DataTable dt = new DataTable();
                            dt.Load(idr);
                            idr.Close();


                            if (dt.Rows.Count <= 0)
                            {
                                retVal.Append("Either proper access is not there or Email not found in database");
                                continue;
                            }

                            if (sender == "contacts")
                            {
                                _tableID = PO.Get_Table_ID("AD_User");
                                existRec = GetAttachedRecord(_tableID, Convert.ToInt32(dt.Rows[0][0]), Convert.ToInt32(uid), folderName);

                            }
                            if (sender == "businessPartner")
                            {
                                _tableID = PO.Get_Table_ID("C_BPartner");
                                existRec = GetAttachedRecord(_tableID, Convert.ToInt32(dt.Rows[0][1]), Convert.ToInt32(uid), folderName);
                            }

                            if (existRec > 0)// Is mail already attached
                            {
                                retVal.Append("MailAlreadyAttachedWithParticularRecord");
                                continue;
                            }

                            if (dt.Rows.Count == 1)
                            {
                                MMailAttachment1 mAttachment = new MMailAttachment1(GetCtx(), 0, null);
                                IMail message;
                                String eml = imapMail.GetMessageByUID(uid);
                                message = new MailBuilder().CreateFromEml(eml);

                                string textmsg = message.Html;
                                bool isAttachment = false;

                                for (int i = 0; i < message.Attachments.Count; i++)
                                {
                                    isAttachment = true;
                                    mAttachment.SetBinaryData(message.Attachments[i].Data);
                                    mAttachment.AddEntry(message.Attachments[i].FileName, message.Attachments[i].Data);
                                }

                                string cc = "";// mailBody.Cc;
                                for (int i = 0; i < message.Cc.Count; i++)
                                {
                                    cc += ((Limilabs.Mail.Headers.MailBox)message.Cc[i]).Address + ";";
                                }
                                string bcc = "";// mailBody.Bcc;
                                for (int i = 0; i < message.Bcc.Count; i++)
                                {
                                    bcc += ((Limilabs.Mail.Headers.MailBox)message.Bcc[i]).Address + ";";
                                }
                                string title = message.Subject;


                                

                                string mailAddress = "";
                                for (int i = 0; i < message.To.Count; i++)
                                {
                                    mailAddress += ((Limilabs.Mail.Headers.MailBox)message.To[i]).Address + ";";
                                }
                                string mailFrom = "";
                                for (int i = 0; i < message.From.Count; i++)
                                {
                                    mailFrom += ((Limilabs.Mail.Headers.MailBox)message.From[i]).Address + ";";
                                }
                                string date = ((DateTime)message.Date).ToShortDateString();


                                mAttachment.SetAD_Client_ID(GetCtx().GetAD_Client_ID());
                                mAttachment.SetAD_Org_ID(GetCtx().GetAD_Org_ID());
                                mAttachment.SetAD_Table_ID(_tableID);
                                mAttachment.SetAttachmentType("I");
                                mAttachment.SetDateMailReceived(message.Date);
                                mAttachment.SetFolderName(folderName);
                                mAttachment.SetIsActive(true);
                                mAttachment.SetIsAttachment(isAttachment);
                                mAttachment.SetMailAddress(mailAddress);
                                mAttachment.SetMailAddressBcc(bcc);
                                mAttachment.SetMailAddressCc(cc);
                                mAttachment.SetMailAddressFrom(mailFrom);

                                if (sender == "contacts")
                                {
                                    mAttachment.SetRecord_ID(Convert.ToInt32(dt.Rows[0][0]));
                                }
                                if (sender == "businessPartner")
                                {
                                    mAttachment.SetRecord_ID(Convert.ToInt32(dt.Rows[0][1]));
                                }

                                mAttachment.SetMailUID(Convert.ToInt32(uid));
                                mAttachment.SetMailUserName(mailAddress);
                                mAttachment.SetTextMsg(textmsg);
                                mAttachment.SetTitle(message.Subject);
                                if (!mAttachment.Save())//save into database
                                {
                                    retVal.Append("SaveError");
                                }
                            }

                            else if (dt.Rows.Count == 0)
                            {
                                retVal.Append("NoRecordFound");
                            }
                            else
                            {
                                retVal.Append("MultipleRecordFound");
                            }
                        }
                        catch
                        { }

                    }
                    catch (Exception ex)
                    {
                        Logout();
                    }

                }

                Logout();

            }
            else
            {
                log.Log(Level.SEVERE, login);
              
            }
        }

        private int GetAttachedRecord(int tableID, int RecordID, int MailUID, string folderName)//, string MailUserFrom)
        {
            String sql = "SELECT MAILATTACHMENT1_ID FROM MAILATTACHMENT1 where AD_TABLE_ID=" + tableID
                        + " AND RECORD_ID=" + RecordID
                        + " AND MAILUID=" + MailUID
                        + " AND FolderName=" + "'" + folderName + "'";

            System.Data.DataSet ds = DB.ExecuteDataset(sql);
            return ds.Tables[0].Rows.Count;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public string Login(UserInformation userInfo)
        {
            if (userInfo != null)
            {
                try
                {
                    this.imapMail = CreateImapConnection(userInfo);
                    return "";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
                return "";
        }

        /// <summary>
        /// Create Imap Connection
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private static Imap CreateImapConnection(UserInformation userInfo)
        {
            // connect
            Imap imapC = new Imap();
            imapC.SendTimeout = new TimeSpan(10, 0, 0);
            imapC.ReceiveTimeout = new TimeSpan(10, 0, 0);


            if (userInfo != null)
            {
                if (userInfo.UseSSL)
                {
                    imapC.SSLConfiguration.EnabledSslProtocols = SslProtocols.Ssl3;
                    // Ignore certificate errors
                    imapC.ServerCertificateValidate += (sender, e) => { e.IsValid = true; };

                    imapC.ConnectSSL(userInfo.Host, userInfo.HostPort);
                }
                else
                {
                    imapC.Connect(userInfo.Host, userInfo.HostPort);
                }

                // Login
                imapC.Login(userInfo.Username, userInfo.Password);
            }
            return imapC;
        }

        /// <summary>
        /// Logout
        /// </summary>
        public void Logout()
        {
            this.CloseImapConnection();
        }

        /// <summary>
        /// closing Impa connection
        /// </summary>
        private void CloseImapConnection()
        {
            if (this.imapMail != null)
            {
                try
                {
                    if (this.imapMail.Connected)
                    {
                        this.imapMail.Close();
                    }

                    this.imapMail.Dispose();
                }
                catch
                {
                    if (this.imapMail.Connected)
                    {
                        this.imapMail.Close();
                    }

                    this.imapMail.Dispose();
                }
            }
        }

        //private string[] ToText(Mail_t_AddressList mail_t_AddressList)
        //{
        //    if (mail_t_AddressList == null)
        //        return new string[0];
        //    else
        //        return mail_t_AddressList.Mailboxes.Select(mb => mb.Address.ToString()).ToArray();
        //}

        //private string[] ToText(Mail_t_MailboxList mail_t_MailboxList)
        //{
        //    if (mail_t_MailboxList == null)
        //        return new string[0];
        //    else
        //        return mail_t_MailboxList.ToArray().Select(mb => mb.Address.ToString()).ToArray();
        //}




    }

    public class UserInformation
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Email { get; set; }
        public int HostPort { get; set; }
        public bool UseSSL { get; set; }

        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpHost { get; set; }
        public bool IsSmtpAuth { get; set; }
        public bool IsSmtpUseSsl { get; set; }
        public int SmtpHostPort { get; set; }
    }


}
