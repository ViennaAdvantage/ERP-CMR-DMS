using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VAdvantage.WF;

namespace VIS.Models
{
    public class WFActivityModel
    {
        public WFInfo GetActivities(Ctx ctx, int AD_User_ID, int AD_Client_ID, int pageNo, int pageSize,bool refresh)
        {

            string sql = @"SELECT * FROM (SELECT a.*,rownum as abcx
                            FROM AD_WF_Activity a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.AD_Client_ID =@clientid
                            AND ( (a.AD_User_ID=@userid
                            OR a.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=@userid
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND COALESCE(r.AD_User_ID,0)=0
                              AND (a.AD_User_ID           =@userid
                              OR a.AD_User_ID            IS NULL
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=@userid
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (r.AD_User_ID           =@userid
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=@userid
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              INNER JOIN AD_User_Roles ur
                              ON (r.AD_Role_ID            =ur.AD_Role_ID)
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (ur.AD_User_ID          =@userid
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=@userid
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              AND r.responsibletype !='H'
                              ) )
                           ORDER BY a.Priority DESC,Created ) WHERE abcx BETWEEN " + (((pageNo - 1) * pageSize) + 1) + " AND " + (((pageNo - 1) * pageSize) + pageSize);
           
            //temp ORDER BY Created desc,a.Priority DESC
               //final  ORDER BY a.Priority DESC,Created
            //int AD_User_ID = Envs.GetContext().GetAD_User_ID();
            try
            {
                SqlParameter[] param = new SqlParameter[2];
                param[0] = new SqlParameter("@clientid", AD_Client_ID);
                param[1] = new SqlParameter("@userid", AD_User_ID);

                DataSet ds = DB.ExecuteDataset(sql, param);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                List<WFActivityInfo> lstInfo = new List<WFActivityInfo>();
                WFActivityInfo itm = null;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    itm = new WFActivityInfo();

                    itm.AD_Table_ID = Util.GetValueOfInt(dr["AD_Table_ID"]);
                    itm.AD_User_ID = Util.GetValueOfInt(dr["AD_User_ID"]);
                    itm.AD_WF_Activity_ID = Util.GetValueOfInt(dr["AD_WF_Activity_ID"]);

                    itm.AD_Node_ID = Util.GetValueOfInt(dr["AD_WF_Node_ID"]);
                    itm.AD_WF_Process_ID = Util.GetValueOfInt(dr["AD_WF_Process_ID"]);
                    itm.AD_WF_Responsible_ID = Util.GetValueOfInt(dr["AD_WF_Responsible_ID"]);
                    itm.AD_Workflow_ID = Util.GetValueOfInt(dr["AD_Workflow_ID"]);
                    itm.CreatedBy = Util.GetValueOfInt(dr["CreatedBy"]);
                    itm.DynPriorityStart = Util.GetValueOfInt(dr["DynPriorityStart"]);
                    itm.Record_ID = Util.GetValueOfInt(dr["Record_ID"]);
                    itm.TxtMsg = Util.GetValueOfString(dr["TextMsg"]);
                    itm.WfState = Util.GetValueOfString(dr["WfState"]);
                    itm.EndWaitTime = Util.GetValueOfDateTime(dr["EndWaitTime"]);
                    itm.Created = Util.GetValueOfString(dr["Created"]);
                    MWFActivity act = new MWFActivity(ctx, itm.AD_WF_Activity_ID, null);
                    itm.NodeName = act.GetNodeName();
                    itm.Summary = act.GetSummary();
                    itm.Description = act.GetNodeDescription();
                    itm.Help = act.GetNodeHelp();
                    itm.History = act.GetHistoryHTML();
                    itm.Priority = Util.GetValueOfInt(dr["Priority"]);
                    lstInfo.Add(itm);

                }

                WFInfo info = new WFInfo();
                info.LstInfo = lstInfo;
                //return lstInfo;

                if (refresh)
                {
                    sql = @"SELECT COUNT(*)
                            FROM AD_WF_Activity a
                            WHERE a.Processed  ='N'
                            AND a.WFState      ='OS'
                            AND a.AD_Client_ID =" + ctx.GetAD_Client_ID() + @"
                            AND ( (a.AD_User_ID=" + ctx.GetAD_User_ID() + @"
                            OR a.AD_User_ID   IN
                              (SELECT AD_User_ID
                              FROM AD_User_Substitute
                              WHERE IsActive   ='Y'
                              AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                              AND (validfrom  <=sysdate)
                              AND (sysdate    <=validto )
                              ))
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND COALESCE(r.AD_User_ID,0)=0
                              AND (a.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IS NULL
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (r.AD_User_ID           =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              )
                            OR EXISTS
                              (SELECT *
                              FROM AD_WF_Responsible r
                              INNER JOIN AD_User_Roles ur
                              ON (r.AD_Role_ID            =ur.AD_Role_ID)
                              WHERE a.AD_WF_Responsible_ID=r.AD_WF_Responsible_ID
                              AND (ur.AD_User_ID          =" + ctx.GetAD_User_ID() + @"
                              OR a.AD_User_ID            IN
                                (SELECT AD_User_ID
                                FROM AD_User_Substitute
                                WHERE IsActive   ='Y'
                                AND Substitute_ID=" + ctx.GetAD_User_ID() + @"
                                AND (validfrom  <=sysdate)
                                AND (sysdate    <=validto )
                                ))
                              AND r.responsibletype !='H'
                              ) )
                           ";

                    info.count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                }
                return info;
            }
            catch
            {
                return null;
            }
        }

        public ActivityInfo GetActivityInfo(int activityID, int nodeID,int wfProcessID ,Ctx ctx)
        {
            ActivityInfo info = new ActivityInfo();
            try
            {
                MWFNode node = new MWFNode(ctx, nodeID, null);
                info.NodeAction = node.GetAction();
                info.NodeName = node.GetName();
                if (MWFNode.ACTION_UserChoice.Equals(node.GetAction()))
                {
                    MColumn col = node.GetColumn();
                    info.ColID = col.GetAD_Column_ID();
                    info.ColReference = col.GetAD_Reference_ID();
                    info.ColReferenceValue = col.GetAD_Reference_Value_ID();
                    info.ColName = col.GetColumnName();
                }
                else if (MWFNode.ACTION_UserWindow.Equals(node.GetAction()))
                {
                   info.AD_Window_ID= node.GetAD_Window_ID();
                   MWFActivity activity = new MWFActivity(ctx, activityID, null);
                   info.KeyCol = activity.GetPO().Get_TableName() + "_ID";
                }
                else if (MWFNode.ACTION_UserForm.Equals(node.GetAction()))
                {
                    info.AD_Form_ID = node.GetAD_Form_ID();
                }

              

                string sql = @"SELECT node.ad_wf_node_ID,
                                  node.Name AS NodeName,
                                  usr.Name AS UserName,
                                  wfea.wfstate,
                                  wfea.TextMsg
                              FROM ad_wf_eventaudit wfea
                                INNER JOIN Ad_WF_Node node
                                ON (node.Ad_Wf_node_ID=wfea.AD_Wf_Node_id)
                                INNER JOIN AD_User usr
                                ON (usr.Ad_User_ID         =wfea.ad_User_ID)
                              WHERE wfea.AD_WF_Process_ID=" + wfProcessID+@"
                              Order By wfea.ad_wf_eventaudit_id desc";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    List<NodeInfo> nodeInfo = new List<NodeInfo>();
                    List<int> nodes = new List<int>();
                    NodeInfo ni = null;
                    NodeHistory nh = null;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        if (!nodes.Contains(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_WF_Node_ID"])))
                        {
                            ni = new NodeInfo();
                            ni.Name = Util.GetValueOfString(ds.Tables[0].Rows[i]["NodeName"]);
                            nh = new NodeHistory();
                            nh.State = Util.GetValueOfString(ds.Tables[0].Rows[i]["WFState"]);
                            nh.ApprovedBy = Util.GetValueOfString(ds.Tables[0].Rows[i]["UserName"]);
                            ni.History   =new List<NodeHistory>();

                            if (ds.Tables[0].Rows[i]["TextMsg"] == null || ds.Tables[0].Rows[i]["TextMsg"] == DBNull.Value)
                            {
                                nh.TextMsg = string.Empty;
                            }
                            else
                            {
                                nh.TextMsg = ds.Tables[0].Rows[i]["TextMsg"].ToString();
                            }
                            ni.History.Add(nh);
                            nodes.Add(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_WF_Node_ID"]));
                            nodeInfo.Add(ni);
                        }
                        else
                        {
                            int index = nodes.IndexOf(Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_WF_Node_ID"]));
                            nh = new NodeHistory();
                            nh.State = Util.GetValueOfString(ds.Tables[0].Rows[i]["WFState"]);
                            nh.ApprovedBy = Util.GetValueOfString(ds.Tables[0].Rows[i]["UserName"]);
                            if (ds.Tables[0].Rows[i]["TextMsg"] == null || ds.Tables[0].Rows[i]["TextMsg"] == DBNull.Value)
                            {
                                nh.TextMsg = string.Empty;
                            }
                            else
                            {
                                nh.TextMsg = ds.Tables[0].Rows[i]["TextMsg"].ToString();
                            }
                            nodeInfo[index].History.Add(nh);
                        }
                    }
                    info.Node = nodeInfo;

                }

                return info;

            }
            catch
            {
                return info;
            }
        }

        public string ApproveIt(int nodeID, int activityID, string textMsg, object forward, object answer, Ctx ctx)
        {
            MWFActivity activity = new MWFActivity(ctx, activityID, null);
            MWFNode node = activity.GetNode();
            int approvalLevel = node.GetApprovalLeval();
            int AD_User_ID = ctx.GetAD_User_ID();
            MColumn column = node.GetColumn();

            if (forward != null) // Prefer Forward 
            {
                int fw = int.Parse(forward.ToString());
                if (fw == AD_User_ID || fw == 0)
                {
                    return "";
                }
                if (!activity.ForwardTo(fw, textMsg, true))
                {
                    return "CannotForward";
                }
            }
            //	User Choice - Answer
            else if (MWFNode.ACTION_UserChoice.Equals(node.GetAction()))
            {
                if (column == null)
                    column = node.GetColumn();
                //	Do we have an answer?
                int dt = column.GetAD_Reference_ID();
                String value = null;
                value = answer != null ? answer.ToString() : null;
                //if (dt == DisplayType.YesNo || dt == DisplayType.List || dt == DisplayType.TableDir)
                if (!node.IsMultiApproval() &&
                    (dt == DisplayType.YesNo || dt == DisplayType.List || dt == DisplayType.TableDir))
                {
                    if (value == null || value.Length == 0)
                    { 
                        return "FillMandatory";
                    }
                    //
                    string res= SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node);
                    if (res != "OK")
                    {
                        return res;
                    }
                }
                //Genral Attribute Instance
                //else if (column.GetColumnName().ToUpper().Equals("C_GENATTRIBUTESETINSTANCE_ID"))
                //{
                //    if (attrib == null)
                //    {
                //        Dispatcher.BeginInvoke(delegate
                //        {
                //            SetBusy(false);
                //            ShowMessage.Error("FillMandatory", true, Msg.GetMsg(Envs.GetContext(), "Answer", true));
                //            //log.Config("Answer=" + value + " - " + textMsg);
                //            return;
                //        });
                //        return;
                //    }

                //    SetUserChoice(AD_User_ID, attrib.GetAttributeSetInstance().ToString(), 0, textMsg, activity, node);
                //}
                
                else if (forward == null && node.IsMultiApproval() && approvalLevel > 0 && answer.ToString().Equals("Y"))
                {

                    int eventCount = Util.GetValueOfInt(DB.ExecuteScalar(@"SELECT COUNT(WFE.AD_WF_EventAudit_ID) FROM AD_WF_EventAudit WFE
                                                                                INNER JOIN AD_WF_Process WFP ON (WFP.AD_WF_Process_ID=WFE.AD_WF_Process_ID)
                                                                                INNER JOIN AD_WF_Activity WFA ON (WFA.AD_WF_Process_ID=WFP.AD_WF_Process_ID)
                                                                                WHERE WFE.AD_WF_Node_ID=" + node.GetAD_WF_Node_ID() + " AND WFA.AD_WF_Activity_ID=" + activity.GetAD_WF_Activity_ID()));
                    if (eventCount < approvalLevel) //Forward Activity
                    {
                        int superVisiorID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Supervisor_ID FROM AD_User WHERE IsActive='Y' AND AD_User_ID=" + activity.GetAD_User_ID()));
                        if (superVisiorID == 0)//Approve
                        {
                            //SetUserConfirmation(AD_User_ID, textMsg, activity, node);

                            string res = SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node);
                            if (res != "OK")
                            {
                                return res;
                            }
                        }
                        else //forward
                        {

                            if (!activity.ForwardTo(superVisiorID, textMsg, true))
                            {
                                //Dispatcher.BeginInvoke(delegate
                                //{
                                //    SetBusy(false);
                                //    ShowMessage.Error("CannotForward", true);
                                //    return;
                                //});
                                return "CannotForward";
                            }
                        }
                    }
                    else //Approve
                    {

                        //SetUserConfirmation(AD_User_ID, textMsg, activity, node);

                        string res = SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node);
                        if (res != "OK")
                        {
                            return res;
                        }
                    }
                }
                else
                {
                    
                    string res= SetUserChoice(AD_User_ID, value, dt, textMsg, activity, node);
                    if (res != "OK")
                    {
                        return res;
                    }
                }
              

            }
            //	User Action
            else
            {
               //   log.Config("Action=" + node.GetAction() + " - " + textMsg);
                //try
                //{
                //    activity.SetUserConfirmation(AD_User_ID, textMsg);
                //}
                //catch (Exception exx)
                //{
                //    Dispatcher.BeginInvoke(delegate
                //            {
                //                SetBusy(false);
                //                log.Log(Level.SEVERE, node.GetName(), exx);
                //                ShowMessage.Error("Error", true, exx.ToString());
                //                return;
                //            });
                //    return;
                //}
                activity.SetUserConfirmation(AD_User_ID, textMsg);

            }

            return "";
        }



        private string SetUserChoice(int AD_User_ID, string value, int dt, string textMsg, MWFActivity _activity, MWFNode _node)
        {
            try
            {
                _activity.SetUserChoice(AD_User_ID, value, dt, textMsg);
                return "OK";
            }
            catch (Exception ex)
            {
                //Dispatcher.BeginInvoke(delegate
                //{
                //    SetBusy(false);
                //    log.Log(Level.SEVERE, _node.GetName(), ex);
                //    ShowMessage.Error("Error", true, ex.ToString());
                //    return;
                //});
                return "Error"+ex.Message;
            }
        }

        public AttributeInfo GetRelativeData(Ctx ctx, int activityID)
        {
            try
            {
                AttributeInfo aInfo = new AttributeInfo();
                DataSet ds = DB.ExecuteDataset(@"SELECT 
                                                            WFP.AD_Table_ID,
                                                            WFP.Record_ID
                                                            FROM AD_WF_Process WFP
                                                            INNER JOIN AD_WF_Activity WFA
                                                            ON (WFA.AD_WF_Process_ID=WFP.AD_WF_Process_ID)
                                                            WHERE WFA.AD_WF_Activity_ID=" + activityID, null);
                PO doc = GetPO(Util.GetValueOfInt(ds.Tables[0].Rows[0][0]), Util.GetValueOfInt(ds.Tables[0].Rows[0][1]), ctx);
                ds = null;
                aInfo.GenAttributeSetID = Util.GetValueOfInt(doc.Get_Value("C_GenAttributeSet_ID"));
                aInfo.GenAttributeSetInstanceID = Util.GetValueOfInt(doc.Get_Value("C_GenAttributeSetInstance_ID"));
                doc = null;
                doc = GetPO("C_GenAttributeSetInstance", aInfo.GenAttributeSetInstanceID, ctx);
                aInfo.Description = Util.GetValueOfString(doc.Get_Value("Description"));
                return aInfo;
            }
            catch
            {
                return null;
            }

        }
        private PO GetPO(int tableID, int recordID,Ctx ctx)
        {
            MTable table = MTable.Get(ctx, tableID);
            return table.GetPO(ctx, recordID, null);

        }
        private PO GetPO(string tableName, int recordID, Ctx ctx)
        {
            //throw new NotImplementedException();
            MTable table = MTable.Get(ctx, Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName='" + tableName + "'")));
            return table.GetPO(ctx, recordID, null);
        }
    }


    public class WFInfo
    {
        public List<WFActivityInfo> LstInfo
        {
            get;
            set;
        }
        public int count
        {
            get;
            set;
        }

    }

    public class WFActivityInfo
    {
       
        public int AD_Table_ID
        {
            get;
            set;
        }
        public int AD_User_ID
        {
            get;
            set;
        }
        public int AD_WF_Activity_ID
        {
            get;
            set;
        }
        public int AD_Node_ID
        {
            get;
            set;
        }
        public int AD_WF_Process_ID
        {
            get;
            set;
        }
        public int AD_WF_Responsible_ID
        {
            get;
            set;
        }
        public int AD_Workflow_ID
        {
            get;
            set;
        }
       
        public int CreatedBy
        {
            get;
            set;
        }
        public int DynPriorityStart
        {
            get;
            set;
        }
        public DateTime? EndWaitTime
        {
            get;
            set;
        }
        public int Record_ID
        {
            get;
            set;
        }
        public string TxtMsg
        {
            get;
            set;
        }
        public string WfState
        {
            get;
            set;
        }
        public string NodeName
        {
            get;
            set;
        }

        public string Summary
        {
            get;
            set;
        }
        public string Created
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Help
        {
            get;
            set;
        }
        public string History
        {
            get;
            set;
        }
        public int Priority
        {
            get;
            set;
        }
    }
    public class ActivityInfo
    {
        public string NodeAction
        {
            get;
            set;
        }
        public string NodeName
        {
            get;
            set;
        }
        public string KeyCol
        {
            get;
            set;
        }
        public int ColID
        {
            get;
            set;
        }
        public int ColReference
        {
            get;
            set;
        }
        public string ColName
        {
            get;
            set;
        }
        public int ColReferenceValue
        {
            get;
            set;
        }
        public int AD_Window_ID
        {
            get;
            set;
        }
        public int AD_Form_ID
        {
            get;
            set;
        }
        public List<NodeInfo> Node
        {
            get;
            set;
        }
       
    }
    public class NodeInfo
    {
        public string Name
        {
            get;
            set;
        }
        public List<NodeHistory> History
        {
            get;
            set;
        }
      
    }
    public class NodeHistory
    {
        public string State
        {
            get;
            set;
        }
        public string ApprovedBy
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string TextMsg
        {
            get;
            set;
        }
    }

    public class AttributeInfo
    {
        public int GenAttributeSetID
        {
            get;
            set;
        }
        public int GenAttributeSetInstanceID
        {
            get;
            set;
        }
        public string  Description
        {
            get;
            set;
        }

    }
}