/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDocType
 * Purpose        : Document Type Model
 * Class Used     : X_C_DocType
 * Chronological    Development
 * Raghunandan      7-May-2009 
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.DataBase;
using System.Collections;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDocType : X_C_DocType
    {
        //	Cache					
        static private CCache<int, MDocType> s_cache = new CCache<int, MDocType>("C_DocType", 20);
        /**	Static Logger	*/
        private static VLogger	s_log	= VLogger.GetVLogger(typeof(MDocType).FullName);

        /// <summary>
        ///Get Client Document Type with DocBaseType
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="DocBaseType">DocBaseType base document type</param>
        /// <returns>array of doc types</returns>
        static public MDocType[] GetOfDocBaseType(Ctx ctx, string docBaseType)
        {
            List<MDocType> list = new List<MDocType>();
            String sql = "SELECT * FROM C_DocType "
                + "WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND DocBaseType='" + docBaseType + "' AND IsActive='Y'"
                + "ORDER BY C_DocType_ID";
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    list.Add(new MDocType(ctx, rs, null));
                }
                pstmt = null;
            }
            catch (Exception e)
            {
               
                s_log.Log(Level.SEVERE, sql, e);
            }
            MDocType[] retValue = new MDocType[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Get Client Document Types
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <returns>array of doc types</returns>
        public static MDocType[] GetOfClient(Ctx ctx)
        {
            List<MDocType> list = new List<MDocType>();
            String sql = "SELECT * FROM C_DocType WHERE AD_Client_ID=" + ctx.GetAD_Client_ID();
            DataSet pstmt = null;
            try
            {
                pstmt = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < pstmt.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = pstmt.Tables[0].Rows[i];
                    list.Add(new MDocType(ctx, rs, null));
                }
                pstmt = null;
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, sql, e);
            }
            MDocType[] retValue = new MDocType[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        ///Get Document Type (cached)
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_DocType_ID">id</param>
        /// <returns>document type</returns>
        static public MDocType Get(Ctx ctx, int C_DocType_ID)
        {
            int key = (int)C_DocType_ID;
            MDocType retValue = (MDocType)s_cache[key];
            if (retValue == null)
            {
                retValue = new MDocType(ctx, C_DocType_ID, null);
                s_cache.Add(key, retValue);
            }
            return retValue;
        }

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="C_DocType_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDocType(Ctx ctx, int C_DocType_ID, Trx trxName)
            : base(ctx, C_DocType_ID, trxName)
        {
            if (C_DocType_ID == 0)
            {
                //	setName (null);
                //	setPrintName (null);
                //	setDocBaseType (null);
                //	setGL_Category_ID (0);
                SetDocumentCopies(0);
                SetHasCharges(false);
                SetIsDefault(false);
                SetIsDocNoControlled(false);
                SetIsSOTrx(false);
                SetIsPickQAConfirm(false);
                SetIsShipConfirm(false);
                SetIsSplitWhenDifference(false);
                SetIsReturnTrx(false);
                SetIsCreateCounter(true);
                SetIsDefaultCounterDoc(false);
                SetIsIndexed(true);
                SetIsInTransit(false);
            }
        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MDocType(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        /// <summary>
        ///New Constructor
        /// </summary>
        /// <param name="ctx">Ctx</param>
        /// <param name="DocBaseType">DocBaseType document base type</param>
        /// <param name="Name">name</param>
        /// <param name="trxName">transaction</param>
        public MDocType(Ctx ctx, string docBaseType, string name, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetAD_Org_ID(0);
            SetDocBaseType(docBaseType);
            SetName(name);
            SetPrintName(name);
            SetGL_Category_ID();
        }

        /// <summary>
        /// Set Default GL Category
        /// </summary>
        public void SetGL_Category_ID()
        {
            String sql = "SELECT * FROM GL_Category WHERE AD_Client_ID=" + GetAD_Client_ID() + "AND IsDefault='Y'";

            int GL_Category_ID = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
            if (GL_Category_ID == 0)
            {
                sql = "SELECT * FROM GL_Category WHERE AD_Client_ID=@param1";
                GL_Category_ID = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetAD_Client_ID());
            }
            SetGL_Category_ID(GL_Category_ID);
        }

        /// <summary>
        /// Set SOTrx based on document base type
        /// </summary>
        public void SetIsSOTrx()
        {
            bool isSOTrx = MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType())
                || MDocBaseType.DOCBASETYPE_MATERIALDELIVERY.Equals(GetDocBaseType())
                || GetDocBaseType().StartsWith("AR");
            base.SetIsSOTrx(isSOTrx);
        }

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MDocType[");
            sb.Append(Get_ID()).Append("-").Append(GetName())
                .Append(",DocNoSequence_ID=").Append(GetDocNoSequence_ID())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        ///Is this a Quotation (Binding)
        /// </summary>
        /// <returns>true if Quotation</returns>
        public bool IsQuotation()
        {
            return DOCSUBTYPESO_Quotation.Equals(GetDocSubTypeSO())
                && MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType());
        }

        /// <summary>
        ///Is this a Proposal (Not binding)
        /// </summary>
        /// <returns>true if proposal</returns>
        public bool IsProposal()
        {
            return DOCSUBTYPESO_Proposal.Equals(GetDocSubTypeSO())
                && MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType());
        }

        /// <summary>
        ///Is this a Proposal or Quotation
        /// </summary>
        /// <returns>true if proposal or quotation</returns>
        public bool IsOffer()
        {
            return (DOCSUBTYPESO_Proposal.Equals(GetDocSubTypeSO())
                    || DOCSUBTYPESO_Quotation.Equals(GetDocSubTypeSO()))
                && MDocBaseType.DOCBASETYPE_SALESORDER.Equals(GetDocBaseType());
        }	

        /// <summary>
        ///Get Print Name
        /// </summary>
        /// <param name="AD_Language">language</param>
        /// <returns>print Name if available translated</returns>
        public string GetPrintName(string AD_Language)
        {
            if (AD_Language == null || AD_Language.Length == 0)
                return base.GetPrintName();
            string retValue = Get_Translation("PrintName", AD_Language);
            if (retValue != null)
                return retValue;
            return base.GetPrintName();
        }

        /// <summary>
        ///Before Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);

            //	Sync DocBaseType && Return Trx
            //	if (newRecord || is_ValueChanged("DocBaseType"))
            SetIsSOTrx();
            return true;
        }
    }
}
