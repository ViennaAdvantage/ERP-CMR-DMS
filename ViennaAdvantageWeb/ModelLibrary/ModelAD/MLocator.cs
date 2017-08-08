using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Web;

namespace VAdvantage.Model
{
    public class MLocator : X_M_Locator
    {
        //	Logger						
        private static VLogger _log = VLogger.GetVLogger(typeof(MLocator).FullName);
        //	Cache
        private static CCache<int, MLocator> cache;

        /// <summary>
        /// Standard Locator Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Locator_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MLocator(Ctx ctx, int M_Locator_ID, Trx trxName)
            : base(ctx, M_Locator_ID, trxName)
        {
            if (M_Locator_ID == 0)
            {
                //SetM_Locator_ID(0);		//	PK
                //SetM_Warehouse_ID(0);		//	Parent
                SetIsDefault(false);
                SetPriorityNo(50);
                //SetValue(null);
                //SetX(null);
                //SetY(null);
                //SetZ(null);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MLocator(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }

        public MLocator(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /// <summary>
        /// New Locator Constructor with XYZ=000
        /// </summary>
        /// <param name="warehouse">parent</param>
        /// <param name="value">value</param>
        public MLocator(MWarehouse warehouse, String value)
            : this(warehouse.GetCtx(), 0, warehouse.Get_TrxName())
        {
            SetClientOrg(warehouse);
            SetM_Warehouse_ID(warehouse.GetM_Warehouse_ID());		//	Parent
            SetValue(value);
            SetXYZ("0", "0", "0");
        }

        /// <summary>
        /// Set Location
        /// </summary>
        /// <param name="X">x</param>
        /// <param name="Y">y</param>
        /// <param name="Z">z</param>
        public void SetXYZ(String X, String Y, String Z)
        {
            SetX(X);
            SetY(Y);
            SetZ(Z);
        }

        /// <summary>
        /// Get Warehouse Name
        /// </summary>
        /// <returns>name</returns>
        public String GetWarehouseName()
        {
            MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
            if (wh.Get_ID() == 0)
                return "<" + GetM_Warehouse_ID() + ">";
            return wh.GetName();
        }

        /// <summary>
        /// Get Locator from Cache
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Locator_ID">id</param>
        /// <returns>MLocator</returns>
        public static MLocator Get(Ctx ctx, int M_Locator_ID)
        {
            if (cache == null)
                cache = new CCache<int, MLocator>("M_Locator", 20);
            int key = M_Locator_ID;
            MLocator retValue = null;
            if (cache.ContainsKey(key))
            {
                retValue = (MLocator)cache[key];
            }
            if (retValue != null)
                return retValue;
            retValue = new MLocator(ctx, M_Locator_ID, null);
            if (retValue.Get_ID() != 0)
                cache.Add(key, retValue);
            return retValue;
        }

        /// <summary>
        /// Get the Locator with the combination or create new one
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Warehouse_ID">id</param>
        /// <param name="value">value</param>
        /// <param name="X">x</param>
        /// <param name="Y">y</param>
        /// <param name="Z">z</param>
        /// <returns>locator</returns>
        public static MLocator Get(Ctx ctx, int M_Warehouse_ID, String value,
            String X, String Y, String Z)
        {
            MLocator retValue = null;
            String sql = "SELECT * FROM M_Locator WHERE M_Warehouse_ID=" + M_Warehouse_ID + " AND " +
                "X='" + X + "' AND Y='" + Y + "' AND Z='" + Z + "'";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataRow rs = ds.Tables[0].Rows[0];
                    retValue = new MLocator(ctx, rs, null);
                }
            }
            catch (Exception ex)
            {
                _log.Log(Level.SEVERE, "get", ex);
            }

            //
            if (retValue == null)
            {

                MWarehouse wh = MWarehouse.Get(ctx, M_Warehouse_ID);
                retValue = new MLocator(wh, HttpUtility.HtmlEncode(value));
                retValue.SetXYZ(HttpUtility.HtmlEncode(X), HttpUtility.HtmlEncode(Y), HttpUtility.HtmlEncode(Z));
                if (!retValue.Save())
                    retValue = null;
            }
            return retValue;
        }

        /// <summary>
        /// Get oldest Default Locator of warehouse with locator
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Locator_ID">locator</param>
        /// <returns>locator or null</returns>
        public static MLocator GetDefault(Ctx ctx, int M_Locator_ID)
        {
            Trx trxName = null;
            MLocator retValue = null;
            String sql = "SELECT * FROM M_Locator l "
                + "WHERE IsDefault='Y'"
                + " AND EXISTS (SELECT * FROM M_Locator lx "
                    + "WHERE l.M_Warehouse_ID=lx.M_Warehouse_ID AND lx.M_Locator_ID=" + M_Locator_ID + ") "
                + "ORDER BY Created";
            DataSet ds = null;
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                if (ds.Tables.Count > 0)
                {
                    DataRow rs = null;
                    int totCount = ds.Tables[0].Rows.Count;
                    for (int i = 0; i < totCount; i++)
                    {
                        rs = ds.Tables[0].Rows[i];
                        retValue = new MLocator(ctx, rs, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        public override String ToString()
        {
            return GetValue();
        }
        public static MLocator GetDefaultLocatorOfOrg(Ctx ctx, int AD_Org_ID)
        {
            MLocator retValue = null;
            List<int> defaultlocators = new List<int>();
            List<int> locators = new List<int>();
            String sql = "SELECT M_Locator_ID, IsDefault FROM M_Locator WHERE (AD_Org_ID=" + AD_Org_ID + " OR 0=" + AD_Org_ID + ")";
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql);
                while (idr != null && idr.Read())
                {
                    if (Util.GetValueOfString(idr[1]) == "Y")
                    {
                        defaultlocators.Add(Util.GetValueOfInt(idr[0]));
                        break;
                    }
                    else
                    {
                        locators.Add(Util.GetValueOfInt(idr[0]));
                    }
                }
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                if (defaultlocators.Count > 0)
                {
                    retValue = MLocator.Get(ctx, Util.GetValueOfInt(defaultlocators[0]));
                    return retValue;
                }
                if (locators.Count > 0)
                {
                    retValue = MLocator.Get(ctx, Util.GetValueOfInt(locators[0]));
                    return retValue;
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
            return retValue;
        }
    }
}
