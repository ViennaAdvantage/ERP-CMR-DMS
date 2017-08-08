/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_Warehouse
 * Chronological Development
 * Veena Pandey     
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MWarehouse : X_M_Warehouse
    {
        //	Cache
        private static CCache<int, MWarehouse> cache = new CCache<int, MWarehouse>("MWarehouse", 5);
        //	Warehouse Locators
        private MLocator[] locators = null;
        //	Default Locator
        private int M_Locator_ID = -1;
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MWarehouse).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Warehouse_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MWarehouse(Ctx ctx, int M_Warehouse_ID, Trx trxName)
            : base(ctx, M_Warehouse_ID, trxName)
        {
            if (M_Warehouse_ID == 0)
            {
                //SetValue(null);
                //SetName(null);
                //SetC_Location_ID(0);
                SetSeparator("*");	// *
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MWarehouse(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Organization Constructor
        /// </summary>
        /// <param name="org">org</param>
        public MWarehouse(MOrg org)
            : this(org.GetCtx(), 0, org.Get_TrxName())
        {
            SetClientOrg(org);
            SetValue(org.GetValue());
            SetName(org.GetName());
            if (org.GetInfo() != null)
                SetC_Location_ID(org.GetInfo().GetC_Location_ID());
        }

        /// <summary>
        /// Get from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Warehouse_ID">id</param>
        /// <returns>warehouse</returns>
        public static MWarehouse Get(Ctx ctx, int M_Warehouse_ID)
        {
            int key = M_Warehouse_ID;
            MWarehouse retValue = null;
            if (cache.ContainsKey(key))
            {
                retValue = (MWarehouse)cache[key];
            }
            if (retValue != null)
                return retValue;
            //
            retValue = new MWarehouse(ctx, M_Warehouse_ID, null);
            cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get Warehouses for Org
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Org_ID">id</param>
        /// <returns>warehouse</returns>
        public static MWarehouse[] GetForOrg(Ctx ctx, int AD_Org_ID)
        {
            List<MWarehouse> list = new List<MWarehouse>();
            String sql = "SELECT * FROM M_Warehouse WHERE AD_Org_ID=" + AD_Org_ID + " ORDER BY Created";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MWarehouse(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MWarehouse[] retValue = new MWarehouse[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// Get Default Locator
        /// </summary>
        /// <returns>(first) default locator</returns>
        public MLocator GetDefaultLocator()
        {
            MLocator[] locators = GetLocators(false);	//	ordered by x,y,z
            MLocator loc1 = null;
            for (int i = 0; i < locators.Length; i++)
            {
                MLocator locIn = locators[i];
                if (locIn.IsDefault() && locIn.IsActive())
                    return locIn;
                if (loc1 == null || loc1.GetPriorityNo() > locIn.GetPriorityNo())
                    loc1 = locIn;
            }
            //	No Default - return highest priority
            if (locators.Length > 0)
            {
                log.Warning("No default Locator for " + GetName());
                return loc1;
            }
            //	No Locator - create one
            MLocator loc = new MLocator(this, "Standard");
            loc.SetIsDefault(true);
            loc.Save();
            log.Info("Created default locator for " + GetName());
            return loc;
        }

        /// <summary>
        /// Get Default M_Locator_ID
        /// </summary>
        /// <returns>id</returns>
        public int GetDefaultM_Locator_ID()
        {
            if (M_Locator_ID <= 0)
                M_Locator_ID = GetDefaultLocator().GetM_Locator_ID();
            return M_Locator_ID;
        }

        /// <summary>
        /// Get Locators
        /// </summary>
        /// <param name="reload">if true reload</param>
        /// <returns>array of locators</returns>
        public MLocator[] GetLocators(bool reload)
        {
            if (!reload && locators != null)
                return locators;
            //
            String sql = "SELECT * FROM M_Locator WHERE M_Warehouse_ID=" + GetM_Warehouse_ID() + " ORDER BY X,Y,Z";
            List<MLocator> list = new List<MLocator>();
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    DataRow dr = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        dr = ds.Tables[0].Rows[i];
                        list.Add(new MLocator(GetCtx(), dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            //
            locators = new MLocator[list.Count];
            locators = list.ToArray();
            return locators;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeforeDelete()
        {
            return Delete_Accounting("M_Warehouse_Acct");
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord & success)
                Insert_Accounting("M_Warehouse_Acct", "C_AcctSchema_Default", null);

            return success;
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord"></param>
        /// <returns></returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            /* Disallow Negative Inventory cannot be checked if there are storage records 
            with negative onhand. */
            if (Is_ValueChanged("IsDisallowNegativeInv") && IsDisallowNegativeInv())
            {
                //String sql = "SELECT M_Product_ID FROM M_StorageDetail s " +
                //             "WHERE s.QtyType = 'H' AND s.M_Locator_ID IN (SELECT M_Locator_ID FROM M_Locator l " +
                //                            "WHERE M_Warehouse_ID=" + GetM_Warehouse_ID() + " )" +
                //             " GROUP BY M_Product_ID, M_Locator_ID " +
                //             " HAVING SUM(s.Qty) < 0 ";
                String sql = "SELECT M_Product_ID FROM M_Storage s " +
                            "WHERE s.M_Locator_ID IN (SELECT M_Locator_ID FROM M_Locator l " +
                                           "WHERE M_Warehouse_ID=" + GetM_Warehouse_ID() + " )" +
                            " GROUP BY M_Product_ID, M_Locator_ID " +
                            " HAVING SUM(s.Qty) < 0 ";

                IDataReader idr = null;
                Boolean negInvExists = false;
                try
                {
                    idr = DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        negInvExists = true;
                    }
                }
                catch (Exception e)
                {
                    log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    if (idr != null)
                    {
                        idr.Close();
                        idr = null;
                    }
                }

                if (negInvExists)
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "NegativeOnhandExists"));
                    return false;
                }
            }

            if (GetAD_Org_ID() == 0)
            {
                int context_AD_Org_ID = GetCtx().GetAD_Org_ID();
                if (context_AD_Org_ID != 0)
                {
                    SetAD_Org_ID(context_AD_Org_ID);
                    log.Warning("Changed Org to Context=" + context_AD_Org_ID);
                }
                else
                {
                    log.SaveError("Error", Msg.Translate(GetCtx(), "Org0NotAllowed"));
                    return false;
                }
            }

            if ((newRecord || Is_ValueChanged("IsWMSEnabled") || Is_ValueChanged("IsDisallowNegativeInv"))
                    && IsWMSEnabled() && !IsDisallowNegativeInv())
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "NegativeInventoryDisallowedWMS"));
                return false;
            }

            if (newRecord || Is_ValueChanged("Separator"))
            {
                if (GetSeparator() == null || GetSeparator().Length <= 0)
                {
                    SetSeparator("*");
                }
            }

            return true;
        }

        /// <summary>
        /// Check if locator is in warehouse 
        /// </summary>
        /// <param name="p_M_Warehouse_ID"></param>
        /// <param name="p_M_Locator_ID"></param>
        /// <returns>true if locator is in the warehouse</returns>
        public static Boolean IsLocatorInWarehouse(int p_M_Warehouse_ID, int p_M_Locator_ID)
        {
            int M_Warehouse_ID = DB.GetSQLValue(null,
                    "SELECT M_Warehouse_ID FROM M_Locator WHERE M_Locator_ID=@param1", p_M_Locator_ID);
            if (p_M_Warehouse_ID == M_Warehouse_ID)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MWarehouse[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}
