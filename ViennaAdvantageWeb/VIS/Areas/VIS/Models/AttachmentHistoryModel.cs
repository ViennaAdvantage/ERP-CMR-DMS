/********************************************************
 * Project Name   : VIS
 * Class Name     : AttachmentHistoryModel
 * Purpose        : Used to show history attached of record like mail sent on recod, letter , Appointments saved, inbox mails
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
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DBase;

namespace VIS.Models
{
    public class AttachmentHistoryModel
    {

        /// <summary>
        /// Fetch history bind to the current business partner from all tables in DB
        /// </summary>
        /// <param name="bPartner_ID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="ctx"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public RealtedHistoryInfoDetails history(int bPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText, string keyColName)
        {
            RealtedHistoryInfoDetails hDetails = new RealtedHistoryInfoDetails();
            List<RelatedHistoryInfo> histo = new List<RelatedHistoryInfo>();

            string tableName = keyColName.Substring(0, keyColName.Length - 3);

            //select all tables where AD_User_ID is used and have an entry in attachment(appointmentinfo OR mailattachment1)

            var sql = @"SELECT ab.AD_Table_ID, ab.Record_ID, att.TableName,ab.Attachtype From (select distinct ad_table_id,record_ID,'A' as Attachtype from appointmentsinfo where ad_table_id IN(     SELECT DISTINCT t.AD_Table_ID
                        FROM AD_Table t WHERE t.AD_Table_ID IN (SELECT AD_Table_ID FROM AD_Column WHERE ColumnName='" + keyColName + "' ) AND TableName NOT LIKE 'I%' AND TableName NOT LIKE '" + tableName + @"' )
                        UNION
                        select distinct ad_table_id,record_ID,'M' as Attachtype from mailattachment1 where ad_table_id IN(     SELECT DISTINCT t.AD_Table_ID
                        FROM AD_Table t WHERE t.AD_Table_ID IN (SELECT AD_Table_ID FROM AD_Column WHERE ColumnName='" + keyColName + "' ) AND TableName NOT LIKE 'I%' AND TableName NOT LIKE '" + tableName + @"' )
                        ) ab JOIn AD_Table att on att.AD_Table_ID=ab.AD_Table_ID order by TableName";


            DataSet ds = DB.ExecuteDataset(sql);
            StringBuilder finalsql = new StringBuilder();
            StringBuilder finalSqlCount = new StringBuilder();




            List<string> tableNames = new List<string>();   //contains tables 
            List<int> AD_Table_ID = new List<int>();        //Contains TableIDs
            List<string> recIDsA = new List<string>();      //contains record ids  of tables from appointment info like recIDsA[0]="123123,123,123123,12312"
            List<string> recIDsM = new List<string>();      //contains record ids of tables from MailAttachment1 info like recIDsM[0]="123123,123,123123,12312"
            StringBuilder curRecordA = new StringBuilder();
            StringBuilder curRecordM = new StringBuilder();


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                {
                    if (curRecordA.Length > 0)
                    {
                        recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                        curRecordA.Clear();
                    }
                    else if (i > 0)
                    {
                        recIDsA.Add("");
                    }


                    if (curRecordM.Length > 0)
                    {
                        recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                        curRecordM.Clear();
                    }
                    else if (i > 0)
                    {
                        recIDsM.Add("");
                    }

                    tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                    AD_Table_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Table_ID"].ToString()));
                }

                if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A")
                {
                    curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                }
                else
                {
                    curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                }


            } //these lists are being filled with data in this loop


            if (curRecordA.Length > 0)
            {
                recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                curRecordA.Clear();
            }
            else
            {
                recIDsA.Add("");
            }


            if (curRecordM.Length > 0)
            {
                recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                curRecordM.Clear();
            }
            else
            {
                recIDsM.Add("");
            }





            for (int i = 0; i < tableNames.Count; i++)// This will create where clause like (ai.AD_Table_ID     =259 AND ai.Record_ID       =1013547) OR (ai.AD_Table_ID     =259 AND ai.Record_ID       =1013548)
            {
                createquer(AD_Table_ID[i], tableNames[i], "", recIDsA[i], recIDsM[i], ctx, keyColName, bPartner_ID);
            }
            if (whereApp.Length > 0)
            {
                whereApp.Remove(whereApp.Length - 4, 3);
            }
            if (whereMAtt.Length > 0)
            {
                whereMAtt.Remove(whereMAtt.Length - 4, 3);
            }



            
            finalSqlCount.Append(@"Select COUNT(*) from (SELECT created FROM ");


            // For Appointments
            finalsql.Append("  SELECT n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
            finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
            if (whereApp.Length > 0)
            {
                finalsql.Append(" WHERE " + whereApp.ToString());
                finalSqlCount.Append(" WHERE " + whereApp.ToString());
            }
            else
            {
                finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
            }

            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                if (finalsql.ToString().Contains("WHERE"))
                {
                    finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
                else
                {
                    finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
            }

            if (finalsql.ToString().Contains("WHERE"))
            {
                finalsql.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                finalSqlCount.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
            }
            else
            {
                finalsql.Append(" WHERE  ai.IsTask='N' ");
                finalSqlCount.Append(" WHERE  ai.IsTask='N' ");
            }


            finalsql.Append("  UNION ");
            finalSqlCount.Append("  UNION ");


            //For Tasks
            finalsql.Append("  SELECT n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
            finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
            if (whereApp.Length > 0)
            {
                finalsql.Append(" WHERE " + whereApp.ToString());
                finalSqlCount.Append(" WHERE " + whereApp.ToString());
            }
            else
            {
                finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
            }

            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                if (finalsql.ToString().Contains("WHERE"))
                {
                    finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
                else
                {
                    finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
            }


            if (finalsql.ToString().Contains("WHERE"))
            {
                finalsql.Append(" AND  ai.IsTask='Y' ");
                finalSqlCount.Append(" AND  ai.IsTask='Y' ");
            }
            else
            {
                finalsql.Append(" WHERE  ai.IsTask='Y' ");
                finalSqlCount.Append(" WHERE  ai.IsTask='Y' ");
            }


            finalsql.Append("  UNION ");
            finalSqlCount.Append("  UNION ");


            // For Letter, sent Mail, Inbox mail

            finalsql.Append(" SELECT attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,adt.Name  as TableName,ai.AD_Table_ID   FROM mailattachment1 ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID  ");
            finalSqlCount.Append(" SELECT ai.created FROM mailattachment1  ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
            if (whereMAtt.Length > 0)
            {
                finalsql.Append(" WHERE " + whereMAtt.ToString());

                finalSqlCount.Append("WHERE " + whereMAtt.ToString());

            }
            else
            {
                finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
            }
            if (searchText != "undefined" && searchText != null && searchText != "")
            {
                if (finalsql.ToString().Contains("WHERE"))
                {
                    finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
                else
                {
                    finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                    finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                }
            }


            finalsql.Append(" order by created desc  ");
            finalSqlCount.Append(" ))) ");

            if (finalsql.Length > 0)
            {

                hDetails.TotalRecords = Util.GetValueOfInt(DB.ExecuteScalar(finalSqlCount.ToString(), null, null));

                DataSet dsfinal = VIS.DBase.DB.ExecuteDatasetPaging(finalsql.ToString(), pageNo, pageSize);

                dsfinal = VAdvantage.DataBase.DB.SetUtcDateTime(dsfinal);

                finalsql.Clear();
                finalsql.Length = 0;
                finalsql = null;

                finalSqlCount.Clear();
                finalSqlCount.Length = 0;
                finalSqlCount = null;


                if (dsfinal != null && dsfinal.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsfinal.Tables[0].Rows.Count; i++)
                    {
                        RelatedHistoryInfo related = new RelatedHistoryInfo();
                        related.ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["ID"]);
                        related.Record_ID = dsfinal.Tables[0].Rows[i]["record_ID"].ToString();
                        related.Created = DateTime.SpecifyKind(Convert.ToDateTime(dsfinal.Tables[0].Rows[i]["created"]), DateTimeKind.Utc);
                        related.Type = dsfinal.Tables[0].Rows[i]["attachmenttype"].ToString();
                        related.Subject = dsfinal.Tables[0].Rows[i]["Subject"].ToString();
                        related.TableName = dsfinal.Tables[0].Rows[i]["TableName"].ToString();
                        related.AD_Table_ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["AD_Table_ID"]);
                        histo.Add(related);
                    }
                }

                dsfinal.Clear();
                dsfinal = null;

            }

            //return list of selcted records 
            hDetails.RHistory = histo;
            return hDetails;

        }



        StringBuilder whereApp = new StringBuilder();
        StringBuilder whereMAtt = new StringBuilder();

        /// <summary>
        /// this list contains where clause for each user 
        /// </summary>
        List<UserInfoForWhereClause> userQuery = new List<UserInfoForWhereClause>();


        private string createquer(int AD_Table_ID, string tableName, string users, string recordsA, string recordsM, Ctx ctx, string colName, int colID)
        {
            if (tableName.ToUpper().Equals("AD_USER"))//skip AD_User Table
            {
                return "";
            }

            if (recordsA.Length > 0 && recordsA.EndsWith(","))
            {
                recordsA = recordsA.Substring(0, recordsA.Length - 1);
            }

            if (recordsM.Length > 0 && recordsM.EndsWith(","))
            {
                recordsM = recordsM.Substring(0, recordsM.Length - 1);
            }

            if (recordsA.Length == 0)
            {
                recordsA = "''";
            }

            if (recordsM.Length == 0)
            {
                recordsM = "''";
            }




            string sql = "";

            if (users.Length > 0)  //will work in case of user history
            {
                sql = "SELECT aa." + tableName + "_ID,bb.Name,bb.AD_User_ID FROM " + tableName + " aa JOIN AD_User bb ";
                if (colName.Length > 0)  // if refrenece is Table and refrence column is AD_User_ID like SalesRep_ID
                {
                    sql += "  ON aa." + colName + "=bb.AD_User_ID WHERE aa.IsActive='Y' ";
                    sql += "    AND  aa." + colName + " IN (" + users + ") ";
                }
                else// if column Name is AD_User_ID
                {
                    sql += "  ON aa.AD_User_ID=bb.AD_User_ID WHERE aa.IsActive='Y' ";
                    sql += "    AND  aa.AD_User_ID IN( " + users + ") ";
                }
            }
            if (colID > 0)      // will work in case of related history records
            {
                sql = "SELECT " + tableName + "_ID FROM " + tableName + " WHERE IsActive='Y' ";
                sql += "    AND  " + colName + " = " + colID + " ";
                sql += " AND (" + tableName + "_ID IN (" + recordsA + ") OR " + tableName + "_ID IN (" + recordsM + "))";
            }







            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                            MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);


            DataSet dsRecords = DB.ExecuteDataset(sql);

            if (colID > 0) //in case of related history...
            {
                if (dsRecords != null && dsRecords.Tables[0].Rows.Count > 0)
                {
                    for (int j = 0; j < dsRecords.Tables[0].Rows.Count; j++)//if found then fetch that record's appointment, mail, letter, inboxmail and make a single query....
                    {
                        if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            whereApp.Append(" (ai.AD_Table_ID=" + AD_Table_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                        if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                        {
                            whereMAtt.Append(" (ai.AD_Table_ID=" + AD_Table_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                        }
                    }
                }
            }
            else
            {
                if (dsRecords != null && dsRecords.Tables[0].Rows.Count > 0) 
                {
                    for (int j = 0; j < dsRecords.Tables[0].Rows.Count; j++)//if found then fetch that record's appointment, mail, letter, inboxmail and make a single query....
                    {
                        var key = Util.GetValueOfInt(dsRecords.Tables[0].Rows[j]["AD_User_ID"].ToString());


                        UserInfoForWhereClause uif = userQuery.Where(aa => aa.UserID == key).FirstOrDefault();

                        if (uif == null)
                        {
                            uif = new UserInfoForWhereClause() { UserID = key };
                            uif.UserName = dsRecords.Tables[0].Rows[j]["Name"].ToString();
                            uif.whereApp = new StringBuilder();
                            uif.whereMAtt = new StringBuilder();
                            if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereApp.Append(" (ai.AD_Table_ID=" + AD_Table_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                            if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereMAtt.Append(" (ai.AD_Table_ID=" + AD_Table_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                            userQuery.Add(uif);
                        }
                        else
                        {
                            if (recordsA.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereApp.Append(" (ai.AD_Table_ID=" + AD_Table_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                            if (recordsM.Contains(dsRecords.Tables[0].Rows[j][0].ToString()))
                            {
                                uif.whereMAtt.Append(" (ai.AD_Table_ID=" + AD_Table_ID + " AND ai.Record_ID=" + dsRecords.Tables[0].Rows[j][0].ToString() + ") OR ");
                            }
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Fetch history bind to the current business partner from all tables in DB
        /// </summary>
        /// <param name="bPartner_ID"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="ctx"></param>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public RealtedHistoryInfoDetails Userhistory(int C_BPartner_ID, int pageSize, int pageNo, Ctx ctx, string searchText)
        {
            RealtedHistoryInfoDetails hDetails = new RealtedHistoryInfoDetails();
            List<RelatedHistoryInfo> histo = new List<RelatedHistoryInfo>();


            //select all tables where AD_User_ID is used and have an entry in attachment(appointmentinfo OR mailattachment1)
            var sql = @"SELECT ab.AD_Table_ID, ab.Record_ID, att.TableName,ab.Attachtype From (select distinct ad_table_id,record_ID,'A' as Attachtype from appointmentsinfo where ad_table_id IN(     SELECT DISTINCT t.AD_Table_ID
                        FROM AD_Table t WHERE t.AD_Table_ID IN (SELECT AD_Table_ID FROM AD_Column WHERE ColumnName='AD_User_ID' ) AND TableName NOT LIKE 'I%' )
                        UNION
                        select distinct ad_table_id,record_ID,'M' as Attachtype from mailattachment1 where ad_table_id IN(     SELECT DISTINCT t.AD_Table_ID
                        FROM AD_Table t WHERE t.AD_Table_ID IN (SELECT AD_Table_ID FROM AD_Column WHERE ColumnName='AD_User_ID' ) AND TableName NOT LIKE 'I%' )
                        ) ab JOIn AD_Table att on att.AD_Table_ID=ab.AD_Table_ID order by TableName";
            DataSet ds = DB.ExecuteDataset(sql);


            //get all users of bpartner
            sql = "SELECT AD_User_ID  FROM AD_User WHERE C_Bpartner_ID=" + C_BPartner_ID + " AND IsActive='Y' ORDER BY AD_User_ID";
            DataSet dsUsers = DB.ExecuteDataset(sql);

            if (dsUsers != null && dsUsers.Tables[0].Rows.Count == 0)
            {
                return hDetails;
            }

            string users = "";

            for (int a = 0; a < dsUsers.Tables[0].Rows.Count; a++)
            {
                users += dsUsers.Tables[0].Rows[a]["AD_User_ID"] + ",";
            }
            users = users.Substring(0, users.Length - 1);  //users in string like 101,102



            StringBuilder finalsql = new StringBuilder();
            StringBuilder finalSqlCount = new StringBuilder();

            try
            {
                List<string> tableNames = new List<string>();//contains tables 
                List<int> AD_Table_ID = new List<int>();     //Contains TableIDs
                List<string> recIDsA = new List<string>();   //contains record ids  of tables from appointment info like recIDsA[0]="123123,123,123123,12312"
                List<string> recIDsM = new List<string>();   //contains record ids of tables from MailAttachment1 info like recIDsM[0]="123123,123,123123,12312"
                StringBuilder curRecordA = new StringBuilder();
                StringBuilder curRecordM = new StringBuilder();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                    {
                        if (curRecordA.Length > 0)
                        {
                            recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                            curRecordA.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsA.Add("");
                        }


                        if (curRecordM.Length > 0)
                        {
                            recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                            curRecordM.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsM.Add("");
                        }

                        tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                        AD_Table_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Table_ID"].ToString()));
                    }

                    if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A")
                    {
                        curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }
                    else
                    {
                        curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }


                }           //these lists are being filled with data in this loop


                if (curRecordA.Length > 0)
                {
                    recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                    curRecordA.Clear();
                }
                else
                {
                    recIDsA.Add("");
                }


                if (curRecordM.Length > 0)
                {
                    recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                    curRecordM.Clear();
                }
                else
                {
                    recIDsM.Add("");
                }




                for (int i = 0; i < tableNames.Count; i++)// This will create where clause like (ai.AD_Table_ID     =259 AND ai.Record_ID       =1013547) OR (ai.AD_Table_ID     =259 AND ai.Record_ID       =1013548)
                {
                    createquer(AD_Table_ID[i], tableNames[i], users, recIDsA[i], recIDsM[i], ctx, "", 0);
                }

                finalSqlCount.Append(@"Select COUNT(*) from (SELECT created FROM ");

                //for (int i = 0; i < userQuery.Count; i++)
                //{
                //    if (userQuery[i].whereApp.Length > 0)
                //    {
                //        userQuery[i].whereApp = userQuery[i].whereApp.Remove(userQuery[i].whereApp.Length - 4, 3);
                //    }
                //    if (userQuery[i].whereMAtt.Length > 0)
                //    {
                //        userQuery[i].whereMAtt = userQuery[i].whereMAtt.Remove(userQuery[i].whereMAtt.Length - 4, 3);
                //    }


                //    finalsql.Append("  SELECT '"+ userQuery[i].UserName+"' as UserName,  n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //    finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //    if (userQuery[i].whereApp.Length > 0)
                //    {
                //        finalsql.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                //        finalSqlCount.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                //    }
                //    else
                //    {
                //        finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //        finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //    }

                //    if (searchText != "undefined" && searchText != null && searchText != "")
                //    {
                //        if (finalsql.ToString().Contains("WHERE"))
                //        {
                //            finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //            finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        }
                //        else
                //        {
                //            finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //            finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        }
                //    }
                //    if (finalsql.ToString().Contains("WHERE"))
                //    {
                //        finalsql.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                //        finalSqlCount.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                //    }
                //    else
                //    {
                //        finalsql.Append(" WHERE  ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                //        finalSqlCount.Append(" WHERE  ai.IsTask='N'  AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                //    }


                //    finalsql.Append("  UNION ");
                //    finalSqlCount.Append("  UNION ");

                //    finalsql.Append("  SELECT  '" + userQuery[i].UserName + "' as UserName, n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //    finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //    if (userQuery[i].whereApp.Length > 0)
                //    {
                //        finalsql.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                //        finalSqlCount.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                //    }
                //    else
                //    {
                //        finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //        finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //    }

                //    if (searchText != "undefined" && searchText != null && searchText != "")
                //    {
                //        if (finalsql.ToString().Contains("WHERE"))
                //        {
                //            finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //            finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        }
                //        else
                //        {
                //            finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //            finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        }
                //    }
                //    if (finalsql.ToString().Contains("WHERE"))
                //    {
                //        finalsql.Append(" AND  ai.IsTask='Y' ");
                //        finalSqlCount.Append(" AND  ai.IsTask='Y' ");
                //    }
                //    else
                //    {
                //        finalsql.Append(" WHERE  ai.IsTask='Y' ");
                //        finalSqlCount.Append(" WHERE  ai.IsTask='Y' ");
                //    }


                //    finalsql.Append("  UNION ");
                //    finalSqlCount.Append("  UNION ");

                //    finalsql.Append(" SELECT  '" + userQuery[i].UserName + "' as UserName, attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,adt.Name  as TableName,ai.AD_Table_ID   FROM mailattachment1 ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID  ");
                //    finalSqlCount.Append(" SELECT ai.created FROM mailattachment1  ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //    if (userQuery[i].whereMAtt.Length > 0)
                //    {
                //        finalsql.Append(" WHERE (" + userQuery[i].whereMAtt.ToString() + ")");

                //        finalSqlCount.Append("WHERE (" + userQuery[i].whereMAtt.ToString() + ")");

                //    }
                //    else
                //    {
                //        finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //        finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //    }

                //    if (searchText != "undefined" && searchText != null && searchText != "")
                //    {
                //        if (finalsql.ToString().Contains("WHERE"))
                //        {
                //            finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //            finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        }
                //        else
                //        {
                //            finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //            finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        }
                //    }


                //    finalsql.Append(" order by created desc  ");
                //    finalSqlCount.Append(" )) )");



                //}















                // here in this query we will find all columns which has Table reference and Key Column is AD_User_ID.
                //Createdby and UpdatedBy


                sql = @"select * from (select distinct ab.AD_Table_ID,ab.ColumnName,ai.record_ID,'A'  as Attachtype,att.TableName from appointmentsinfo ai JOIN   (SELECT  AD_Table_ID,
                                ColumnName FROM AD_Column WHERE lower(ColumnName)   !='updatedby' AND lower(columnname)     !='createdby' AND ad_reference_Value_id IN
                                (SELECT ad_reference_id FROM ad_ref_table WHERE column_key_id= (SELECT AD_Column_ID FROM AD_Column  WHERE columnname='AD_User_ID'
                                  AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName='AD_User' ) ) ) ) ab ON ab.AD_Table_ID=ai.AD_Table_ID
                              JOIN AD_Table att on ai.AD_Table_ID=att.AD_Table_ID
                              UNION
                              select distinct ab.AD_Table_ID,ab.ColumnName,ai.record_ID,'M' as  Attachtype,att.TableName from mailattachment1 ai JOIN   (SELECT  AD_Table_ID,
                                ColumnName FROM AD_Column WHERE lower(ColumnName)   !='updatedby' AND lower(columnname)     !='createdby' AND ad_reference_Value_id IN
                                (SELECT ad_reference_id FROM ad_ref_table WHERE column_key_id= (SELECT AD_Column_ID FROM AD_Column WHERE columnname='AD_User_ID'
                                  AND AD_Table_ID = (SELECT AD_Table_ID FROM AD_Table WHERE TableName='AD_User' ) ) ) ) ab ON ab.AD_Table_ID=ai.AD_Table_ID
                              JOIN AD_Table att on ai.AD_Table_ID=att.AD_Table_ID) order by tablename";

                ds = DB.ExecuteDataset(sql);


                tableNames = new List<string>();
                AD_Table_ID = new List<int>();
                recIDsA = new List<string>();
                recIDsM = new List<string>();
                curRecordA = new StringBuilder();
                curRecordM = new StringBuilder();

                //here in this dictionary, we will have TableName as key  and columnNames in List<string>.
                //here list<string> is used because there may be more that one column with Table reference in a table like Salesrep_ID and BILL_USER_ID in C_ORder

                Dictionary<string, List<string>> colNamess = new Dictionary<string, List<string>>();

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!tableNames.Contains(ds.Tables[0].Rows[i]["TableName"].ToString()))
                    {
                        if (curRecordA.Length > 0)
                        {
                            recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                            curRecordA.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsA.Add("");
                        }


                        if (curRecordM.Length > 0)
                        {
                            recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                            curRecordM.Clear();
                        }
                        else if (i > 0)
                        {
                            recIDsM.Add("");
                        }

                        tableNames.Add(ds.Tables[0].Rows[i]["TableName"].ToString());
                        AD_Table_ID.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_Table_ID"].ToString()));

                        List<string> lst = new List<string>();
                        lst.Add(ds.Tables[0].Rows[i]["ColumnName"].ToString());
                        colNamess[ds.Tables[0].Rows[i]["TableName"].ToString()] = lst;
                    }

                    if (!colNamess[ds.Tables[0].Rows[i]["TableName"].ToString()].Contains(ds.Tables[0].Rows[i]["ColumnName"].ToString()))
                    {
                        colNamess[ds.Tables[0].Rows[i]["TableName"].ToString()].Add(ds.Tables[0].Rows[i]["ColumnName"].ToString());
                    }

                    if (ds.Tables[0].Rows[i]["Attachtype"].ToString() == "A")
                    {
                        curRecordA.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }
                    else
                    {
                        curRecordM.Append(ds.Tables[0].Rows[i]["Record_ID"].ToString() + ",");
                    }


                }


                if (curRecordA.Length > 0)
                {
                    recIDsA.Add(curRecordA.Remove(curRecordA.Length - 1, 1).ToString());
                    curRecordA.Clear();
                }
                else
                {
                    recIDsA.Add("");
                }


                if (curRecordM.Length > 0)
                {
                    recIDsM.Add(curRecordM.Remove(curRecordM.Length - 1, 1).ToString());
                    curRecordM.Clear();
                }
                else
                {
                    recIDsM.Add("");
                }




                for (int i = 0; i < tableNames.Count; i++)
                {
                    for (int j = 0; j < colNamess[tableNames[i]].Count; j++)        // for each column in each table we are prepareing where clause...
                    {
                        createquer(AD_Table_ID[i], tableNames[i], users, recIDsA[i], recIDsM[i], ctx, colNamess[tableNames[i]][j], 0);
                    }
                }



                for (int i = 0; i < userQuery.Count; i++)           // Create   query 
                {
                    if (userQuery[i].whereApp.Length > 0)
                    {
                        userQuery[i].whereApp = userQuery[i].whereApp.Remove(userQuery[i].whereApp.Length - 4, 3);
                    }
                    if (userQuery[i].whereMAtt.Length > 0)
                    {
                        userQuery[i].whereMAtt = userQuery[i].whereMAtt.Remove(userQuery[i].whereMAtt.Length - 4, 3);
                    }
                    if (i > 0)
                    {
                        finalSqlCount.Append(" UNION ");
                        finalsql.Append(" UNION ");
                    }

                    // For Appointment
                    finalsql.Append("  SELECT '" + userQuery[i].UserName + "' as UserName,  n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                    finalSqlCount.Append(" (SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                    if (userQuery[i].whereApp.Length > 0)
                    {
                        finalsql.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                        finalSqlCount.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                    }
                    else
                    {
                        finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                        finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                    }

                    if (searchText != "undefined" && searchText != null && searchText != "")
                    {
                        if (finalsql.ToString().Contains("WHERE"))
                        {
                            finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                            finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                        }
                        else
                        {
                            finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                            finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                        }
                    }
                    if (finalsql.ToString().Contains("WHERE"))
                    {
                        finalsql.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                        finalSqlCount.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                    }
                    else
                    {
                        finalsql.Append(" WHERE  ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                        finalSqlCount.Append(" WHERE  ai.IsTask='N'  AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                    }
                    finalSqlCount.Append(")");

                    finalsql.Append("  UNION ");
                    finalSqlCount.Append("  UNION ");


                    //For Tasks
                    finalsql.Append("  SELECT  '" + userQuery[i].UserName + "' as UserName, n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                    finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                    if (userQuery[i].whereApp.Length > 0)
                    {
                        finalsql.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                        finalSqlCount.Append(" WHERE (" + userQuery[i].whereApp.ToString() + ")");
                    }
                    else
                    {
                        finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                        finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                    }

                    if (searchText != "undefined" && searchText != null && searchText != "")
                    {
                        if (finalsql.ToString().Contains("WHERE"))
                        {
                            finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                            finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                        }
                        else
                        {
                            finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                            finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                        }
                    }
                    if (finalsql.ToString().Contains("WHERE"))
                    {
                        finalsql.Append(" AND  ai.IsTask='Y' ");
                        finalSqlCount.Append(" AND  ai.IsTask='Y' ");
                    }
                    else
                    {
                        finalsql.Append(" WHERE  ai.IsTask='Y' ");
                        finalSqlCount.Append(" WHERE  ai.IsTask='Y' ");
                    }

                    finalSqlCount.Append(")");
                    finalsql.Append("  UNION ");
                    finalSqlCount.Append("  UNION ");


                    //For Letter, Sent mail, Inbox Mail
                    finalsql.Append(" SELECT  '" + userQuery[i].UserName + "' as UserName, attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,adt.Name  as TableName,ai.AD_Table_ID   FROM mailattachment1 ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID  ");
                    finalSqlCount.Append(" (SELECT ai.created FROM mailattachment1  ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                    if (userQuery[i].whereMAtt.Length > 0)
                    {
                        finalsql.Append(" WHERE (" + userQuery[i].whereMAtt.ToString() + ")");

                        finalSqlCount.Append("WHERE (" + userQuery[i].whereMAtt.ToString() + ")");

                    }
                    else
                    {
                        finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                        finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                    }

                    if (searchText != "undefined" && searchText != null && searchText != "")
                    {
                        if (finalsql.ToString().Contains("WHERE"))
                        {
                            finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                            finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                        }
                        else
                        {
                            finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                            finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                        }
                    }


                    finalSqlCount.Append(")");


                }


                finalsql.Append(" order by created desc  ");
                finalSqlCount.Append(" )");


                //if (whereApp.Length > 0)
                //{
                //    whereApp.Remove(whereApp.Length - 4, 3);
                //}
                //if (whereMAtt.Length > 0)
                //{
                //    whereMAtt.Remove(whereMAtt.Length - 4, 3);
                //}





                //finalsql.Append("  SELECT n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //if (whereApp.Length > 0)
                //{
                //    finalsql.Append(" WHERE " + whereApp.ToString());
                //    finalSqlCount.Append(" WHERE " + whereApp.ToString());
                //}
                //else
                //{
                //    finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //    finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //}

                //if (searchText != "undefined" && searchText != null && searchText != "")
                //{
                //    if (finalsql.ToString().Contains("WHERE"))
                //    {
                //        finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //    }
                //    else
                //    {
                //        finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //    }
                //}
                //if (finalsql.ToString().Contains("WHERE"))
                //{
                //    finalsql.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                //    finalSqlCount.Append(" AND ai.IsTask='N' AND ai.AD_User_ID= " + ctx.GetAD_User_ID());
                //}
                //else
                //{
                //    finalsql.Append(" WHERE  ai.IsTask='N' ");
                //    finalSqlCount.Append(" WHERE  ai.IsTask='N' ");
                //}


                //finalsql.Append("  UNION ");
                //finalSqlCount.Append("  UNION ");

                //finalsql.Append("  SELECT n'A' as  attachmenttype,ai.AppointmentsInfo_ID AS ID, ai.record_ID, ai.created,'" + Msg.GetMsg(ctx, "Appointment") + "' AS TYPE,ai.Subject,adt.Name as TableName,ai.AD_Table_ID FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //finalSqlCount.Append(" ( SELECT ai.created FROM AppointmentsInfo ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //if (whereApp.Length > 0)
                //{
                //    finalsql.Append(" WHERE " + whereApp.ToString());
                //    finalSqlCount.Append(" WHERE " + whereApp.ToString());
                //}
                //else
                //{
                //    finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //    finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //}

                //if (searchText != "undefined" && searchText != null && searchText != "")
                //{
                //    if (finalsql.ToString().Contains("WHERE"))
                //    {
                //        finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //    }
                //    else
                //    {
                //        finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //    }
                //}
                //if (finalsql.ToString().Contains("WHERE"))
                //{
                //    finalsql.Append(" AND  ai.IsTask='Y' ");
                //    finalSqlCount.Append(" AND  ai.IsTask='Y' ");
                //}
                //else
                //{
                //    finalsql.Append(" WHERE  ai.IsTask='Y' ");
                //    finalSqlCount.Append(" WHERE  ai.IsTask='Y' ");
                //}


                //finalsql.Append("  UNION ");
                //finalSqlCount.Append("  UNION ");

                //finalsql.Append(" SELECT attachmenttype, ai.MAILATTACHMENT1_ID AS ID, ai.record_ID,ai.created,'" + Msg.GetMsg(ctx, "SentMail") + "' AS TYPE, ai.TITLE AS Subject,adt.Name  as TableName,ai.AD_Table_ID   FROM mailattachment1 ai JOIN AD_User au on au.AD_User_ID=ai.createdby  JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID  ");
                //finalSqlCount.Append(" SELECT ai.created FROM mailattachment1  ai JOIN AD_User au on au.AD_User_ID=ai.createdby JOIN AD_Table adt on adt.AD_Table_ID =ai.AD_Table_ID ");
                //if (whereMAtt.Length > 0)
                //{
                //    finalsql.Append(" WHERE " + whereMAtt.ToString());

                //    finalSqlCount.Append("WHERE " + whereMAtt.ToString());

                //}
                //else
                //{
                //    finalsql.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //    finalSqlCount.Append("WHERE (ai.AD_Table_ID=" + 0 + " AND ai.Record_ID=" + 0 + ")");
                //}

                //if (searchText != "undefined" && searchText != null && searchText != "")
                //{
                //    if (finalsql.ToString().Contains("WHERE"))
                //    {
                //        finalsql.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        finalSqlCount.Append(" AND upper(ai.Subject)  like upper('%" + searchText + "%')");
                //    }
                //    else
                //    {
                //        finalsql.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //        finalSqlCount.Append(" WHERE upper(ai.Subject)  like upper('%" + searchText + "%')");
                //    }
                //}


                //finalsql.Append(" order by created desc  ");
                //finalSqlCount.Append(" )) )");

            }
            catch
            {

            }

            hDetails.TotalRecords = Util.GetValueOfInt(DB.ExecuteScalar(finalSqlCount.ToString(), null, null));

            DataSet dsfinal = VIS.DBase.DB.ExecuteDatasetPaging(finalsql.ToString(), pageNo, pageSize);

            dsfinal = VAdvantage.DataBase.DB.SetUtcDateTime(dsfinal);

            finalsql.Clear();
            finalsql = null;

            finalSqlCount.Clear();
            finalSqlCount = null;


            whereApp.Clear();
            whereApp = null;

            whereMAtt.Clear();
            whereMAtt = null;


            if (dsfinal != null && dsfinal.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsfinal.Tables[0].Rows.Count; i++)
                {
                    RelatedHistoryInfo related = new RelatedHistoryInfo();
                    related.ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["ID"]);
                    related.Record_ID = dsfinal.Tables[0].Rows[i]["record_ID"].ToString();
                    related.Created = DateTime.SpecifyKind(Convert.ToDateTime(dsfinal.Tables[0].Rows[i]["created"]), DateTimeKind.Utc);
                    related.Type = dsfinal.Tables[0].Rows[i]["attachmenttype"].ToString();
                    related.Subject = dsfinal.Tables[0].Rows[i]["Subject"].ToString();
                    related.TableName = dsfinal.Tables[0].Rows[i]["TableName"].ToString();
                    related.AD_Table_ID = Util.GetValueOfInt(dsfinal.Tables[0].Rows[i]["AD_Table_ID"]);
                    related.UserName = Util.GetValueOfString(dsfinal.Tables[0].Rows[i]["UserName"]);
                    histo.Add(related);
                }
            }

            dsfinal.Clear();
            dsfinal = null;


            //return list of selcted records 
            hDetails.RHistory = histo;
            return hDetails;

        }

        /// <summary>
        /// when user click on sent mail record from histort form's Grid, then fetch its information like attachment, to , bccc etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public MailInfo Sentmails(int ID, Ctx ctx)
        {
            MailInfo info = new MailInfo();

            var strSql = "select MAILADDRESSFROM, MAILADDRESS,TITLE,MailAddressBcc,MailAddressCc,CREATED" +
                          ",  ISATTACHMENT, MAILATTACHMENT1_ID, AD_CLIENT_ID, AD_ORG_ID, AD_TABLE_ID, RECORD_ID, CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg from mailattachment1 where mailattachment1_ID=" + ID;
            var ds = DB.ExecuteDataset(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                info.To = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddress"]);
                info.From = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressfrom"]);
                info.Date = Util.GetValueOfString(ds.Tables[0].Rows[0]["created"]);
                info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                info.Bcc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressbcc"]);
                info.Cc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddresscc"]);
                //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                info.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ad_table_id"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                info.IsMail = true;
            };

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos();
                i.Name = oMAttachEntry.GetName();
                i.ID = ID;
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        /// <summary>
        /// when user click on inbox mail record from histort form's Grid, then fetch its information like attachment, to , bccc etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public MailInfo InboxMails(int ID, Ctx ctx)
        {
            MailInfo info = new MailInfo();

            var strSql = "select MAILADDRESSFROM, MAILADDRESS,MailAddressBcc,MailAddressCc,TITLE " +
                ",DateMailReceived ,created " +
                ",  ISATTACHMENT, MAILATTACHMENT1_ID, AD_CLIENT_ID, AD_ORG_ID, AD_TABLE_ID, RECORD_ID, CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg from mailattachment1 where mailattachment1_ID=" + ID;
            var ds = DB.ExecuteDataset(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                info.To = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddress"]);
                info.From = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressfrom"]);
                info.Date = Util.GetValueOfString(ds.Tables[0].Rows[0]["created"]);
                info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                info.Bcc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddressbcc"]);
                info.Cc = Util.GetValueOfString(ds.Tables[0].Rows[0]["mailaddresscc"]);
                //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                info.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ad_table_id"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                info.IsMail = true;
            };

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos();
                i.Name = oMAttachEntry.GetName();
                i.ID = ID;
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        /// <summary>
        /// when user click on letter record from histort form's Grid, then fetch its information like attachment, subject etc...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public MailInfo Letters(int ID, Ctx ctx)
        {
            MailInfo info = new MailInfo();

            var strSql = "select TITLE ,CREATED" +
                 ", ISATTACHMENT, MAILATTACHMENT1_ID, AD_CLIENT_ID, AD_ORG_ID, AD_TABLE_ID, RECORD_ID, CREATEDBY, ISACTIVE, ISMAILSENT,TextMsg from mailattachment1 where mailattachment1_ID=" + ID;
            var ds = DB.ExecuteDataset(strSql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                info.Title = Util.GetValueOfString(ds.Tables[0].Rows[0]["title"]);
                info.Date = Util.GetValueOfString(ds.Tables[0].Rows[0]["created"]);
                info.Detail = Util.GetValueOfString(ds.Tables[0].Rows[0]["textmsg"]);
                //info.Comments = Util.GetValueOfString(ds.Tables[0].Rows[0]["comments"]);
                info.AD_Table_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["ad_table_id"]);
                info.Record_ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["record_id"]);
                info.ID = Util.GetValueOfInt(ds.Tables[0].Rows[0]["mailattachment1_id"]);
                info.IsMail = true;
            };

            MMailAttachment1 _mAttachment = new MMailAttachment1(ctx, ID, null);
            List<AttachmentInfos> attac = new List<AttachmentInfos>();
            foreach (MAttachmentEntry oMAttachEntry in _mAttachment.GetEntries())
            {
                AttachmentInfos i = new AttachmentInfos();
                i.Name = oMAttachEntry.GetName();
                i.ID = ID;
                attac.Add(i);
            }
            info.Attach = attac;
            return info;
        }

        public List<ChatInfos> ViewChatonHistory(Ctx ctx, int record_ID, bool isAppointment)
        {
            string sql = @"SELECT inn.ChatID,inn.EntryID,CH.characterdata,ch.cm_chatentry_id,au.Name AS NAME,ch.created, ai.ad_image_id ,ai.binarydata  AS UsrImg,CH.createdby
                    FROM (SELECT * FROM (SELECT CH.cm_chat_id    AS ChatID,MAX(CE.cm_chatentry_id)AS EntryID FROM cm_chatentry CE
                        JOIN cm_chat CH ON CE.cm_chat_id= CH.cm_chat_id GROUP BY CH.cm_chat_id ORDER BY entryID )inn1  ) inn
                    JOIN cm_chatentry CH ON inn.ChatID= ch.cm_chat_id JOIN cm_chat CMH ON (cmh.cm_chat_id= inn.chatid)
                    JOIN ad_user Au ON au.ad_user_id= CH.createdBy LEFT OUTER JOIN ad_image AI ON(ai.ad_image_id=au.ad_image_id)";
            if (isAppointment)
            {
                sql += " WHERE cMh.AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE lower(TableName)='appointmentsinfo')";
            }
            else
            {
                sql += " WHERE cMh.AD_Table_ID =(SELECT AD_Table_ID FROM AD_Table WHERE lower(TableName)='mailattachment1')";
            }


            sql += @" AND Record_ID=" + record_ID + @" 
                    ORDER BY inn.EntryID DESC,ch.cm_chatentry_id ASC";

            List<ChatInfos> cInfo = new List<ChatInfos>();

            DataSet ds = DB.ExecuteDataset(sql);

            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    ChatInfos inf = new ChatInfos();
                    inf.ChatID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ChatID"]);
                    inf.EntryID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ChatID"]);
                    inf.CharacterData = Util.GetValueOfString(ds.Tables[0].Rows[i]["characterdata"]);
                    inf.UserName = Util.GetValueOfString(ds.Tables[0].Rows[i]["Name"]);
                    //inf.created = Util.GetValueOfString(ds.Tables[0].Rows[i]["created"]); ;

                    if (ds.Tables[0].Rows[i]["created"].ToString() != null && ds.Tables[0].Rows[i]["created"].ToString() != "")
                    {
                        DateTime _createdDate = Convert.ToDateTime(ds.Tables[0].Rows[i]["created"].ToString());
                        DateTime _format = DateTime.SpecifyKind(new DateTime(_createdDate.Year, _createdDate.Month, _createdDate.Day, _createdDate.Hour, _createdDate.Minute, _createdDate.Second), DateTimeKind.Utc);
                        inf.Created = _format;
                    }
                    int uimgId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["ad_image_id"].ToString());

                    if (uimgId > 0)
                    {
                        MImage mimg = new MImage(ctx, uimgId, null);
                        var imgfll = mimg.GetThumbnailURL(46, 46);
                        inf.UserImage = imgfll;
                    }
                    inf.CreatedBy = Util.GetValueOfInt(ds.Tables[0].Rows[i]["createdby"]);
                    cInfo.Add(inf);

                }
            }
            return cInfo;
        }


        /// <summary>
        /// Used to save comment on on particular history record.
        /// always update  privious one...
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="text"></param>
        /// <param name="isAppointment"></param>
        public List<ChatInfos> SaveComment(int ID, string text, bool isAppointment, Ctx ctx)
        {
            object chatID = 0;
            if (isAppointment)
            {
                string sql = "SELECT CM_Chat_ID FROM CM_Chat WHERE AD_Table_ID=(SELECT AD_Table_ID FROM AD_Table WHERE lower(TableName)='appointmentsinfo') and Record_ID=" + ID;
                chatID = DB.ExecuteScalar(sql, null, null);
                if (chatID == null || chatID == DBNull.Value)
                {
                    chatID = DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE lower(TableName)='appointmentsinfo'", null, null);

                    MChat chats = new MChat(ctx, 0, null);
                    chats.SetAD_Table_ID(Convert.ToInt32(chatID));
                    chats.SetRecord_ID(ID);
                    chats.SetDescription("");
                    chats.Save();
                    chatID = chats.GetCM_Chat_ID();
                }

            }
            else
            {
                string sql = "SELECT CM_Chat_ID FROM CM_Chat WHERE AD_Table_ID=(SELECT AD_Table_ID FROM AD_Table WHERE lower(TableName)='mailattachment1') and Record_ID=" + ID;
                chatID = DB.ExecuteScalar(sql, null, null);
                if (chatID == null || chatID == DBNull.Value)
                {
                    chatID = DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE lower(TableName)='mailattachment1'", null, null);
                    MChat chats = new MChat(ctx, 0, null);
                    chats.SetAD_Table_ID(Convert.ToInt32(chatID));
                    chats.SetRecord_ID(ID);
                    chats.SetDescription("");
                    chats.Save();
                    chatID = chats.GetCM_Chat_ID();
                }
            }

            MChatEntry entry = new MChatEntry(ctx, 0, null);
            entry.SetCM_Chat_ID(Convert.ToInt32(chatID));
            entry.SetCharacterData(text);
            entry.SetAD_User_ID(ctx.GetAD_User_ID());
            entry.Save();

            List<ChatInfos> cInfo = ViewChatonHistory(ctx, ID, isAppointment);
            return cInfo;

        }


    }

    public class RealtedHistoryInfoDetails
    {
        public int TotalRecords { get; set; }
        public List<RelatedHistoryInfo> RHistory { get; set; }
    }

    public class RelatedHistoryInfo
    {
        public int ID { get; set; }
        public string Record_ID { get; set; }
        public object Created { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string TableName { get; set; }
        public int AD_Table_ID { get; set; }
        public string UserName { get; set; }
    }

    public class MailInfo
    {
        public string Title { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Date { get; set; }
        public string Detail { get; set; }
        public string Bcc { get; set; }
        public string Cc { get; set; }
        public string Comments { get; set; }
        public bool IsMail { get; set; }
        public bool IsLetter { get; set; }
        public List<AttachmentInfos> Attach { get; set; }
        public int AD_Table_ID { get; set; }
        public int Record_ID { get; set; }
        public int ID { get; set; }
    }


    public class AttachmentInfos
    {
        public string Name { get; set; }
        public int ID { get; set; }
    }

    public class ChatInfos
    {
        public int ChatID { get; set; }
        public int EntryID { get; set; }
        public int AD_Image_ID { get; set; }
        public int CreatedBy { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string CharacterData { get; set; }
        public DateTime Created { get; set; }
    }

    public class UserInfoForWhereClause
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public StringBuilder whereApp { get; set; }
        public StringBuilder whereMAtt { get; set; }
    }


}
