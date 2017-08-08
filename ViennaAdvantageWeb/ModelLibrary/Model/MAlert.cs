using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MAlert : X_AD_Alert
    {
        public MAlert(Ctx ctx, int AD_Alert_ID, Trx trx)
            : base(ctx, AD_Alert_ID, trx)
        {
            if (AD_Alert_ID == 0)
            {
                //	setAD_AlertProcessor_ID (0);
                //	setName (null);
                //	setAlertMessage (null);
                //	setAlertSubject (null);
                SetEnforceClientSecurity(true);	// Y
                SetEnforceRoleSecurity(true);	// Y
                SetIsValid(true);	// Y
            }
        }	//	MAlert

        public MAlert(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
        }	//	MAlert

        /**	The Rules						*/
        private MAlertRule[] m_rules = null;
        /**	The Recipients					*/
        private MAlertRecipient[] m_recipients = null;


        public MAlertRule[] GetRules(bool reload)
        {
            if (m_rules != null && !reload)
                return m_rules;
            String sql = "SELECT * FROM AD_AlertRule "
                + "WHERE AD_Alert_ID=" + GetAD_Alert_ID();
            List<MAlertRule> list = new List<MAlertRule>();

            DataSet ds = DB.ExecuteDataset(sql);
            try
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    list.Add(new MAlertRule(GetCtx(), dr, null));
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            m_rules = new MAlertRule[list.Count()];
            m_rules = list.ToArray();
            return m_rules;
        }	//	getRules


        public MAlertRecipient[] GetRecipients(bool reload)
        {
            if (m_recipients != null && !reload)
                return m_recipients;
            String sql = "SELECT * FROM AD_AlertRecipient "
                + "WHERE AD_Alert_ID=" + GetAD_Alert_ID();
            List<MAlertRecipient> list = new List<MAlertRecipient>();
            try
            {
                DataSet ds = DB.ExecuteDataset(sql);
                foreach (DataRow dr in ds.Tables[0].Rows)
                    list.Add(new MAlertRecipient(GetCtx(), dr, null));
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            m_recipients = new MAlertRecipient[list.Count()];
            m_recipients = list.ToArray();
            return m_recipients;
        }	//	getRecipients


        public int GetFirstAD_Role_ID()
        {
            GetRecipients(false);
            foreach (MAlertRecipient element in m_recipients)
            {
                if (element.GetAD_Role_ID() != -1)
                    return element.GetAD_Role_ID();
            }
            return -1;
        }	//	getForstAD_Role_ID


        public int GetFirstUserAD_Role_ID()
        {
            GetRecipients(false);
            int AD_User_ID = GetFirstAD_User_ID();
            if (AD_User_ID != -1)
            {
                MUserRoles[] urs = MUserRoles.GetOfUser(GetCtx(), AD_User_ID);
                foreach (MUserRoles element in urs)
                {
                    if (element.IsActive())
                        return element.GetAD_Role_ID();
                }
            }
            return -1;
        }	//	getFirstUserAD_Role_ID


        public int GetFirstAD_User_ID()
        {
            GetRecipients(false);
            foreach (MAlertRecipient element in m_recipients)
            {
                if (element.GetAD_User_ID() != -1)
                    return element.GetAD_User_ID();
            }
            return -1;
        }	//	getFirstAD_User_ID


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAlert[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Valid=").Append(IsValid());
            if (m_rules != null)
                sb.Append(",Rules=").Append(m_rules.Length);
            if (m_recipients != null)
                sb.Append(",Recipients=").Append(m_recipients.Length);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
