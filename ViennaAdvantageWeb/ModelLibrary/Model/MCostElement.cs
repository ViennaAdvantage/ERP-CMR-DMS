﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MCostElement
 * Purpose        : Product/element cost purpose 
 * Class Used     : X_M_CostElement
 * Chronological    Development
 * Raghunandan     18-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Print;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.IO;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCostElement : X_M_CostElement
    {
        //Cache					
        private static CCache<int, MCostElement> s_cache = new CCache<int, MCostElement>("M_CostElement", 20);

        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostElement).FullName);


        /**
         * 	Get Material Cost Element or create it
         *	@param po parent
         *	@param CostingMethod method
         *	@return cost element
         */
        public static MCostElement GetMaterialCostElement(PO po, String CostingMethod)
        {
            if (CostingMethod == null || CostingMethod.Length == 0)
            {
                _log.Severe("No CostingMethod");
                return null;
            }
            //
            MCostElement retValue = null;
            String sql = "SELECT * FROM M_CostElement WHERE AD_Client_ID=" + po.GetAD_Client_ID() + " AND CostingMethod=@costingMethod ORDER BY AD_Org_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@costingMethod", CostingMethod);

                idr = DataBase.DB.ExecuteReader(sql, param, po.Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                //bool n = dr.next(); //jz to fix DB2 resultSet closed problem
                //if (n)
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCostElement(po.GetCtx(), dr, po.Get_TrxName());
                }
                //if (n && dr.next())
                //    s_log.warning("More then one Material Cost Element for CostingMethod=" + CostingMethod);
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            if (retValue != null)
                return retValue;

            //	Create New
            retValue = new MCostElement(po.GetCtx(), 0, po.Get_TrxName());
            retValue.SetClientOrg(po.GetAD_Client_ID(), 0);
            String name = MRefList.GetListName(po.GetCtx(), COSTINGMETHOD_AD_Reference_ID, CostingMethod);
            if (name == null || name.Length == 0)
                name = CostingMethod;
            retValue.SetName(name);
            retValue.SetCostElementType(COSTELEMENTTYPE_Material);
            retValue.SetCostingMethod(CostingMethod);
            retValue.Save();
            return retValue;
        }

        /**
         * 	Get first Material Cost Element
         *	@param ctx context
         *	@param CostingMethod costing method
         *	@return Cost Element or null
         */
        public static MCostElement GetMaterialCostElement(Ctx ctx, String CostingMethod)
        {
            MCostElement retValue = null;
            String sql = "SELECT * FROM M_CostElement WHERE AD_Client_ID=" + ctx.GetAD_Client_ID() + " AND CostingMethod=@CostingMethod ORDER BY AD_Org_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@CostingMethod", CostingMethod);
                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MCostElement(ctx, dr, null);
                }
                //if (dr.next())
                //    s_log.info("More then one Material Cost Element for CostingMethod=" + CostingMethod);
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            return retValue;
        }


        /**
         * 	Get active Material Cost Element for client 
         *	@param po parent
         *	@return cost element array
         */
        public static MCostElement[] GetCostingMethods(PO po)
        {
            List<MCostElement> list = new List<MCostElement>();
            String sql = "SELECT * FROM M_CostElement "
                + "WHERE AD_Client_ID=@Client_ID"
                + " AND IsActive='Y' AND CostElementType='M' AND CostingMethod IS NOT NULL";
            DataTable dt = null;
            //IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@Client_ID", po.GetAD_Client_ID());
                //idr = DataBase.DB.ExecuteReader(sql, param, po.Get_TrxName());
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param, po.Get_TrxName());
                dt = new DataTable();
                dt = ds.Tables[0];
                //idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MCostElement(po.GetCtx(), dr, po.Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                //if (idr != null)
                // {
                //     idr.Close();
                //}
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MCostElement[] retValue = new MCostElement[list.Count];
            retValue = list.ToArray();
            return retValue;
        }


        /**
         * 	Get Cost Element from Cache
         *	@param ctx context
         *	@param M_CostElement_ID id
         *	@return Cost Element
         */
        public static MCostElement Get(Ctx ctx, int M_CostElement_ID)
        {
            int key = M_CostElement_ID;
            MCostElement retValue = (MCostElement)s_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MCostElement(ctx, M_CostElement_ID, null);
            if (retValue.Get_ID() != 0)
                s_cache.Add(key, retValue);
            return retValue;
        }


        /**
         * 	Standard Constructor
         *	@param ctx context
         *	@param M_CostElement_ID id
         *	@param trxName trx
         */
        public MCostElement(Ctx ctx, int M_CostElement_ID, Trx trxName)
            : base(ctx, M_CostElement_ID, trxName)
        {
            if (M_CostElement_ID == 0)
            {
                //	setName (null);
                SetCostElementType(COSTELEMENTTYPE_Material);
                SetIsCalculated(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName trx
         */
        public MCostElement(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            //	Check Unique Costing Method
            if (COSTELEMENTTYPE_Material.Equals(GetCostElementType())
                && (newRecord || Is_ValueChanged("CostingMethod")))
            {
                String sql = "SELECT COALESCE(MAX(M_CostElement_ID),0) FROM M_CostElement "
                    + "WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND CostingMethod='" + GetCostingMethod() + "'";
                int id = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, null));
                if (id > 0 && id != Get_ID())
                {
                    log.SaveError("AlreadyExists", Msg.GetElement(GetCtx(), "CostingMethod"));
                    return false;
                }
            }

            //	Maintain Calclated
            if (COSTELEMENTTYPE_Material.Equals(GetCostElementType()))
            {
                String cm = GetCostingMethod();
                if (cm == null || cm.Length == 0
                    || COSTINGMETHOD_StandardCosting.Equals(cm))
                    SetIsCalculated(false);
                else
                    SetIsCalculated(true);
            }
            else
            {
                if (IsCalculated())
                    SetIsCalculated(false);
                if (GetCostingMethod() != null)
                    SetCostingMethod(null);
            }

            if (GetAD_Org_ID() != 0)
                SetAD_Org_ID(0);
            return true;
        }

        /**
         * 	Before Delete
         *	@return true if can be deleted
         */
        protected override bool BeforeDelete()
        {
            String cm = GetCostingMethod();
            if (cm == null
                || !COSTELEMENTTYPE_Material.Equals(GetCostElementType()))
                return true;

            //	Costing Methods on AS level
            MAcctSchema[] ass = MAcctSchema.GetClientAcctSchema(GetCtx(), GetAD_Client_ID());
            for (int i = 0; i < ass.Length; i++)
            {
                if (ass[i].GetCostingMethod().Equals(GetCostingMethod()))
                {
                    log.SaveError("CannotDeleteUsed", Msg.GetElement(GetCtx(), "C_AcctSchema_ID")
                       + " - " + ass[i].GetName());
                    return false;
                }
            }

            //	Costing Methods on PC level
            String sql = "SELECT M_Product_Category_ID FROM M_Product_Category_Acct WHERE AD_Client_ID=" + GetAD_Client_ID() + " AND CostingMethod=@costingMethod";
            int M_Product_Category_ID = 0;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@costingMethod", GetCostingMethod());

                idr = DataBase.DB.ExecuteReader(sql, param, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    M_Product_Category_ID = Convert.ToInt32(dr[0]);
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            if (M_Product_Category_ID != 0)
            {
                log.SaveError("CannotDeleteUsed", Msg.GetElement(GetCtx(), "M_Product_Category_ID")
                   + " (ID=" + M_Product_Category_ID + ")");
                return false;
            }
            return true;
        }

        /**
         * 	Is this a Costing Method
         *	@return true if not Material cost or no costing method.
         */
        public bool IsCostingMethod()
        {
            return COSTELEMENTTYPE_Material.Equals(GetCostElementType())
                && GetCostingMethod() != null;
        }

        /**
         * 	Is Avg Invoice Costing Method
         *	@return true if AverageInvoice
         */
        public bool IsAverageInvoice()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_AverageInvoice)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Avg PO Costing Method
         *	@return true if AveragePO
         */
        public bool IsAveragePO()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_AveragePO)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is FiFo Costing Method
         *	@return true if Fifo
         */
        public bool IsFifo()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_Fifo)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Last Invoice Costing Method
         *	@return true if LastInvoice
         */
        public bool IsLastInvoice()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_LastInvoice)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Last PO Costing Method
         *	@return true if LastPOPrice
         */
        public bool IsLastPOPrice()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_LastPOPrice)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is LiFo Costing Method
         *	@return true if Lifo
         */
        public bool IsLifo()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_Lifo)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is Std Costing Method
         *	@return true if StandardCosting
         */
        public bool IsStandardCosting()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_StandardCosting)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	Is User Costing Method
         *	@return true if User Defined
         */
        public bool IsUserDefined()
        {
            String cm = GetCostingMethod();
            return cm != null
                && cm.Equals(COSTINGMETHOD_UserDefined)
                && COSTELEMENTTYPE_Material.Equals(GetCostElementType());
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MCostElement[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Type=").Append(GetCostElementType())
                .Append(",Method=").Append(GetCostingMethod())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Get Cost element
        /// </summary>
        /// <param name="AD_Client_ID"></param>
        /// <returns></returns>
        public static int GetMfgCostElement(int AD_Client_ID)
        {
            int ce = 0;
            String sql = " SELECT M_CostElement_ID FROM M_CostElement WHERE AD_Client_ID =" + AD_Client_ID + " AND IsMfgMaterialCost = 'Y'";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null);
                if (idr.Read())
                {
                    ce = Util.GetValueOfInt(idr[0]);
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            return ce;
        }

    }
}
