/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_Storage
 * Chronological    Development
 * Raghunandan     04-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MStorage : X_M_Storage
    {
        #region Private variables
        // Log		
        private static VLogger s_log = VLogger.GetVLogger(typeof(MStorage).FullName);

        private static VLogger _log = VLogger.GetVLogger(typeof(MStorage).FullName);
        //private static CLogger		s_log = CLogger.getCLogger (MStorage.class);
        // Warehouse					
        private int _M_Warehouse_ID = 0;
        #endregion

        /*	Get Storage Info
        *	@param ctx context
        *	@param M_Locator_ID locator
        *	@param M_Product_ID product
        *	@param M_AttributeSetInstance_ID instance
        *	@param trxName transaction
        *	@return existing or null
        */
        public static MStorage Get(Ctx ctx, int M_Locator_ID, int M_Product_ID,
             int M_AttributeSetInstance_ID, Trx trxName)
        {
            MStorage retValue = null;
            String sql = "SELECT * FROM M_Storage "
                + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND ";
            if (M_AttributeSetInstance_ID == 0)
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            else
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MStorage(ctx, dr, trxName);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            if (retValue == null)
            {
                _log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                    + ", M_Product_ID=" + M_Product_ID + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID);
            }
            else
            {
                _log.Fine("M_Locator_ID=" + M_Locator_ID
                    + ", M_Product_ID=" + M_Product_ID + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID);
            }
            return retValue;
        }

        /**
       * 	Get all Storages for Product with ASI
       *	@param ctx context
       *	@param M_Product_ID product
       *	@param M_Locator_ID locator
       *	@param FiFo first in-first-out
       *	@param trxName transaction
       *	@return existing or null
       */
        public static MStorage[] GetAllWithASI(Ctx ctx, int M_Product_ID, int M_Locator_ID,
            bool FiFo, Trx trxName)
        {
            List<MStorage> list = new List<MStorage>();
            String sql = "SELECT * FROM M_Storage "
                + "WHERE M_Product_ID=" + M_Product_ID + " AND M_Locator_ID=" + M_Locator_ID
                + " AND M_AttributeSetInstance_ID > 0"
                + " AND QtyOnHand > 0 "
                + "ORDER BY M_AttributeSetInstance_ID";
            if (!FiFo)
                sql += " DESC";
            DataSet ds = new DataSet();
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    list.Add(new MStorage(ctx, dr, trxName));
                }
                ds = null;
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            ds = null;
            MStorage[] retValue = new MStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get all Storages for Product
        *	@param ctx context
        *	@param M_Product_ID product
        *	@param M_Locator_ID locator
        *	@param trxName transaction
        *	@return existing or null
        */
        public static MStorage[] GetAll(Ctx ctx, int M_Product_ID, int M_Locator_ID, Trx trxName)
        {
            List<MStorage> list = new List<MStorage>();
            String sql = "SELECT * FROM M_Storage "
                + "WHERE M_Product_ID=" + M_Product_ID + " AND M_Locator_ID=" + M_Locator_ID
                + " AND QtyOnHand <> 0 "
                + "ORDER BY M_AttributeSetInstance_ID";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MStorage(ctx, dr, trxName));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MStorage[] retValue = new MStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Get Storage Info for Product across warehouses
        *	@param ctx context
        *	@param M_Product_ID product
        *	@param trxName transaction
        *	@return existing or null
        */
        public static MStorage[] GetOfProduct(Ctx ctx, int M_Product_ID, Trx trxName)
        {
            List<MStorage> list = new List<MStorage>();
            String sql = "SELECT * FROM M_Storage "
                + "WHERE M_Product_ID=" + M_Product_ID;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MStorage(ctx, dr, trxName));
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            MStorage[] retValue = new MStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /* 	Get Storage Info for Warehouse
       *	@param Ctx context
       *	@param M_Warehouse_ID 
       *	@param M_Product_ID product
       *	@param M_AttributeSetInstance_ID instance
       *	@param M_AttributeSet_ID attribute set
       *	@param allAttributeInstances if true, all attribute set instances
       *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances
       *	@param FiFo first in-first-out
       *	@param trxName transaction
       *	@return existing - ordered by location priority (desc) and/or guarantee date
       */
        public static MStorage[] GetWarehouse(Ctx Ctx, int M_Warehouse_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
            bool allAttributeInstances, DateTime? minGuaranteeDate,
            bool FiFo, Trx trxName)
        {
            if (M_Warehouse_ID == 0 || M_Product_ID == 0)
                return new MStorage[0];

            if (M_AttributeSet_ID == 0)
                allAttributeInstances = true;
            else
            {
                MAttributeSet mas = MAttributeSet.Get(Ctx, M_AttributeSet_ID);
                if (!mas.IsInstanceAttribute())
                    allAttributeInstances = true;
            }

            List<MStorage> list = new List<MStorage>();
            //	Specific Attribute Set Instance
            String sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
                + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) "
                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                + " AND s.M_Product_ID=" + M_Product_ID
                + " AND COALESCE(s.M_AttributeSetInstance_ID,0)= " + M_AttributeSetInstance_ID
                + "ORDER BY l.PriorityNo DESC, M_AttributeSetInstance_ID";
            if (!FiFo)
            {
                sql += " DESC";
            }
            //	All Attribute Set Instances
            if (allAttributeInstances)
            {
                sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
                    + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
                    + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
                    + "FROM M_Storage s"
                    + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
                    + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) "
                    + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                    + " AND s.M_Product_ID=" + M_Product_ID;
                if (minGuaranteeDate != null)
                {
                    //sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>'" + String.Format("{0:dd-MMM-yy}", minGuaranteeDate) + "') "
                    //    + "ORDER BY asi.GuaranteeDate, M_AttributeSetInstance_ID";	//	Has Prio over Locator
                    sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>" + GlobalVariable.TO_DATE(minGuaranteeDate, true) + ")"         // Changes done by Bharat for Italian Language
                        + "ORDER BY asi.GuaranteeDate, M_AttributeSetInstance_ID";	//	Has Prio over Locator
                    if (!FiFo)
                        sql += " DESC";
                    sql += ", l.PriorityNo DESC, s.QtyOnHand DESC";
                }
                else
                {
                    sql += "ORDER BY l.PriorityNo DESC, l.M_Locator_ID, s.M_AttributeSetInstance_ID";
                    if (!FiFo)
                        sql += " DESC";
                    sql += ", s.QtyOnHand DESC";
                }
            }
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MStorage(Ctx, dr, trxName));
                }
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
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            MStorage[] retValue = new MStorage[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /*	Create or Get Storage Info
        *	@param ctx context
        *	@param M_Locator_ID locator
        *	@param M_Product_ID product
        *	@param M_AttributeSetInstance_ID instance
        *	@param trxName transaction
        *	@return existing/new or null
        */
        public static MStorage GetCreate(Ctx ctx, int M_Locator_ID, int M_Product_ID,
             int M_AttributeSetInstance_ID, Trx trxName)
        {
            if (M_Locator_ID == 0)
            {
                throw new ArgumentException("M_Locator_ID=0");
            }
            if (M_Product_ID == 0)
            {
                throw new ArgumentException("M_Product_ID=0");
            }
            MStorage retValue = Get(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, trxName);
            if (retValue != null)
                return retValue;

            //	Insert row based on locator
            MLocator locator = new MLocator(ctx, M_Locator_ID, trxName);
            if (locator.Get_ID() != M_Locator_ID)
            {
                throw new ArgumentException("Not found M_Locator_ID=" + M_Locator_ID);
            }
            retValue = new MStorage(locator, M_Product_ID, M_AttributeSetInstance_ID);
            retValue.Save(trxName);
            _log.Fine("New " + retValue);
            return retValue;
        }

        /* 	Update Storage Info add.
       * 	Called from MProjectIssue
       *	@param Ctx context
       *	@param M_Warehouse_ID warehouse
       *	@param M_Locator_ID locator
       *	@param M_Product_ID product
       *	@param M_AttributeSetInstance_ID AS Instance
       *	@param reservationAttributeSetInstance_ID reservation AS Instance
       *	@param diffQtyOnHand add on hand
       *	@param diffQtyReserved add reserved
       *	@param diffQtyOrdered add order
       *	@param trxName transaction
       *	@return true if updated
       */

        public static bool Add(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int reservationAttributeSetInstance_ID,
            Decimal? diffQtyOnHand,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Trx trxName)
        {
            MStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetM_Locator_ID() != M_Locator_ID
                && storage.GetM_Product_ID() != M_Product_ID
                && storage.GetM_AttributeSetInstance_ID() != M_AttributeSetInstance_ID)
            {
                _log.Severe("No Storage found - M_Locator_ID=" + M_Locator_ID
                + ",M_Product_ID=" + M_Product_ID + ",ASI=" + M_AttributeSetInstance_ID);
                return false;
            }
            MStorage storageASI = null;
            if (M_AttributeSetInstance_ID != reservationAttributeSetInstance_ID)
            {
                int reservationM_Locator_ID = M_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
                    reservationM_Locator_ID = wh.GetDefaultM_Locator_ID();
                }
                storageASI = Get(Ctx, reservationM_Locator_ID, M_Product_ID, reservationAttributeSetInstance_ID, trxName);
                if (storageASI == null)	//	create if not existing - should not happen
                {
                    MProduct product = MProduct.Get(Ctx, M_Product_ID);
                    int xM_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
                    if (xM_Locator_ID == 0)
                    {
                        MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
                        xM_Locator_ID = wh.GetDefaultM_Locator_ID();
                    }
                    storageASI = GetCreate(Ctx, xM_Locator_ID,
                        M_Product_ID, reservationAttributeSetInstance_ID, trxName);
                }
            }
            bool changed = false;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                else
                {
                    storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (storageASI == null)
                    storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                else
                    storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                _log.Fine(diffText.ToString());
                if (storageASI != null)
                    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                return storage.Save(trxName);
            }

            return true;
        }
        // Change for Day to Day (DTD001) add paramete req reserved qty
        public static bool Add(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int reservationAttributeSetInstance_ID,
            Decimal? diffQtyOnHand,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Decimal? ReqReservedQty, Trx trxName)
        {
            MStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetM_Locator_ID() != M_Locator_ID
                && storage.GetM_Product_ID() != M_Product_ID
                && storage.GetM_AttributeSetInstance_ID() != M_AttributeSetInstance_ID)
            {
                s_log.Severe("No Storage found - M_Locator_ID=" + M_Locator_ID
                + ",M_Product_ID=" + M_Product_ID + ",ASI=" + M_AttributeSetInstance_ID);
                return false;
            }
            MStorage storageASI = null;
            if (M_AttributeSetInstance_ID != reservationAttributeSetInstance_ID)
            {
                int reservationM_Locator_ID = M_Locator_ID;
                if (reservationAttributeSetInstance_ID == 0)
                {
                    MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
                    reservationM_Locator_ID = wh.GetDefaultM_Locator_ID();
                }
                storageASI = Get(Ctx, reservationM_Locator_ID, M_Product_ID, reservationAttributeSetInstance_ID, trxName);
                if (storageASI == null)	//	create if not existing - should not happen
                {
                    MProduct product = MProduct.Get(Ctx, M_Product_ID);
                    int xM_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
                    if (xM_Locator_ID == 0)
                    {
                        MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
                        xM_Locator_ID = wh.GetDefaultM_Locator_ID();
                    }
                    storageASI = GetCreate(Ctx, xM_Locator_ID,
                        M_Product_ID, reservationAttributeSetInstance_ID, trxName);
                }
            }
            bool changed = false;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (storageASI == null)
                {
                    storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                else
                {
                    storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }

            //Requisition Reserved Quantity
            if (ReqReservedQty != null && ReqReservedQty != 0)
            {
                if (storageASI == null)
                {
                    storage.SetDTD001_QtyReserved(Decimal.Add(storage.GetDTD001_QtyReserved(), (Decimal)ReqReservedQty));
                }
                else
                {
                    storageASI.SetDTD001_QtyReserved(Decimal.Add(storageASI.GetDTD001_QtyReserved(), (Decimal)ReqReservedQty));
                }
                diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                changed = true;
            }
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (storageASI == null)
                    storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                else
                    storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                s_log.Fine(diffText.ToString());
                if (storageASI != null)
                    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                return storage.Save(trxName);
            }

            return true;
        }

        //Added By Bharat
        public static bool Add(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID, int Ord_Warehouse_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int reservationAttributeSetInstance_ID,
            Decimal? diffQtyOnHand,
            Decimal? diffQtyReserved, Decimal? diffQtyOrdered, Trx trxName)
        {
            MStorage storage = null;
            StringBuilder diffText = new StringBuilder("(");

            //	Get Storage
            if (storage == null)
            {
                storage = GetCreate(Ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, trxName);
            }
            //	Verify
            if (storage.GetM_Locator_ID() != M_Locator_ID
                && storage.GetM_Product_ID() != M_Product_ID
                && storage.GetM_AttributeSetInstance_ID() != M_AttributeSetInstance_ID)
            {
                _log.Severe("No Storage found - M_Locator_ID=" + M_Locator_ID
                + ",M_Product_ID=" + M_Product_ID + ",ASI=" + M_AttributeSetInstance_ID);
                return false;
            }
            //MStorage storageASI = null;
            //if (M_AttributeSetInstance_ID != reservationAttributeSetInstance_ID)
            //{
            //    int reservationM_Locator_ID = M_Locator_ID;
            //    if (reservationAttributeSetInstance_ID == 0)
            //    {
            //        MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
            //        reservationM_Locator_ID = wh.GetDefaultM_Locator_ID();
            //    }
            //    storageASI = Get(Ctx, reservationM_Locator_ID, M_Product_ID, reservationAttributeSetInstance_ID, trxName);
            //    if (storageASI == null)	//	create if not existing - should not happen
            //    {
            //        MProduct product = MProduct.Get(Ctx, M_Product_ID);
            //        int xM_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
            //        if (xM_Locator_ID == 0)
            //        {
            //            MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
            //            xM_Locator_ID = wh.GetDefaultM_Locator_ID();
            //        }
            //        storageASI = GetCreate(Ctx, xM_Locator_ID,
            //            M_Product_ID, reservationAttributeSetInstance_ID, trxName);
            //    }
            //}
            bool changed = false;
            if (diffQtyOnHand != null && diffQtyOnHand != 0)
            {
                storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand.Value));
                diffText.Append("OnHand=").Append(diffQtyOnHand);
                changed = true;
            }
            //	Reserved Qty
            if (diffQtyReserved != null && diffQtyReserved != 0)
            {
                if (Ord_Warehouse_ID != 0)
                {
                    MWarehouse wh = MWarehouse.Get(Ctx, Ord_Warehouse_ID);
                    int Ord_Locator_ID = GetResLocator_ID(Ord_Warehouse_ID, M_Product_ID, reservationAttributeSetInstance_ID, diffQtyReserved.Value, trxName);
                    MStorage ordStorage = GetCreate(Ctx, Ord_Locator_ID, M_Product_ID, reservationAttributeSetInstance_ID, trxName);
                    ordStorage.SetQtyReserved(Decimal.Add(ordStorage.GetQtyReserved(), (Decimal)diffQtyReserved));
                    if (ordStorage.GetQtyReserved() < 0)
                    {
                        ordStorage.SetQtyReserved(0);
                    }
                    ordStorage.Save(trxName);
                }
                //else
                //{
                //    if (storageASI == null)
                //    {
                //        storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
                //    }
                //    else
                //    {
                //        storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
                //    }
                //    diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
                //}
                changed = true;
            }
            if (diffQtyOrdered != null && diffQtyOrdered != 0)
            {
                if (Ord_Warehouse_ID != 0)
                {
                    MWarehouse wh = MWarehouse.Get(Ctx, Ord_Warehouse_ID);
                    int Ord_Locator_ID = GetLocator_ID(Ord_Warehouse_ID, M_Product_ID, reservationAttributeSetInstance_ID, diffQtyOrdered.Value, trxName);
                    MStorage ordStorage = GetCreate(Ctx, Ord_Locator_ID, M_Product_ID, reservationAttributeSetInstance_ID, trxName);
                    ordStorage.SetQtyOrdered(Decimal.Add(ordStorage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                    if (ordStorage.GetQtyOrdered() < 0)
                    {
                        ordStorage.SetQtyOrdered(0);
                    }
                    ordStorage.Save(trxName);
                }
                //else
                //{
                //    if (storageASI == null)
                //        storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                //    else
                //        storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
                //    diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
                //}
                changed = true;
            }
            if (changed)
            {
                diffText.Append(") -> ").Append(storage.ToString());
                _log.Fine(diffText.ToString());
                //if (storageASI != null)
                //    storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
                return storage.Save(trxName);
            }

            return true;
        }

        /*	Get Location with highest Locator Priority and a sufficient OnHand Qty
        * 	@param M_Warehouse_ID warehouse
        * 	@param M_Product_ID product
        * 	@param M_AttributeSetInstance_ID asi
        * 	@param Qty qty
        *	@param trxName transaction
        * 	@return id
        */
        public static int GetM_Locator_ID(int M_Warehouse_ID, int M_Product_ID,
            int M_AttributeSetInstance_ID, Decimal Qty, Trx trxName)
        {
            int M_Locator_ID = 0;
            int firstM_Locator_ID = 0;
            String sql = "SELECT s.M_Locator_ID, s.QtyOnHand "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
                + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
                + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                + " AND s.M_Product_ID=" + M_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
                + " AND l.IsActive='Y' "
                + "ORDER BY l.PriorityNo DESC, s.QtyOnHand DESC";
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    Decimal QtyOnHand = Convert.ToDecimal(dr[1]);//.getBigDecimal(2);
                    if (QtyOnHand != null && Qty.CompareTo(QtyOnHand) <= 0)
                    {
                        M_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
                        break;
                    }
                    if (firstM_Locator_ID == 0)
                        firstM_Locator_ID = Convert.ToInt32(dr[0]); //dr.getInt(1);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                dt = null;
                if (idr != null)
                {
                    idr.Close();
                }
            }
            if (M_Locator_ID != 0)
                return M_Locator_ID;
            return firstM_Locator_ID;
        }


        public static int GetLocator_ID(int M_Warehouse_ID, int M_Product_ID,
           int M_AttributeSetInstance_ID, Decimal Qty, Trx trxName)
        {
            if (Qty < 0)
            {
                Qty = Decimal.Negate(Qty);
            }
            else
            {
                Qty = 0;
            }
            int M_Locator_ID = 0;
            IDataReader idr = null;
            DataTable dt = null;
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param", Qty);

            String sql = "SELECT s.M_Locator_ID "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
                + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
                + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                + " AND s.M_Product_ID=" + M_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='Y' AND s.QtyOrdered >= @param ";
            M_Locator_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, param, trxName));
            if (M_Locator_ID == 0)
            {
                string qry = "SELECT s.M_Locator_ID "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
                + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
                + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                + " AND s.M_Product_ID=" + M_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='N' AND s.QtyOrdered >= @param ";
                try
                {
                    idr = DB.ExecuteReader(qry, param, trxName);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        M_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    _log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    dt = null;
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }
            }
            return M_Locator_ID;
        }

        public static int GetResLocator_ID(int M_Warehouse_ID, int M_Product_ID,
           int M_AttributeSetInstance_ID, Decimal Qty, Trx trxName)
        {
            if (Qty < 0)
            {
                Qty = Decimal.Negate(Qty);
            }
            else
            {
                Qty = 0;
            }
            int M_Locator_ID = 0;
            IDataReader idr = null;
            DataTable dt = null;
            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@param", Qty);

            String sql = "SELECT s.M_Locator_ID "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
                + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
                + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                + " AND s.M_Product_ID=" + M_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='Y' AND s.QtyReserved >= @param ";
            M_Locator_ID = Util.GetValueOfInt(DB.ExecuteScalar(sql, param, trxName));
            if (M_Locator_ID == 0)
            {
                string qry = "SELECT s.M_Locator_ID "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
                + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
                + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
                + " AND s.M_Product_ID=" + M_Product_ID
                + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
                + " AND l.IsActive='Y' AND l.IsDefault='N' AND s.QtyReserved >= @param ";
                try
                {
                    idr = DB.ExecuteReader(qry, param, trxName);
                    dt = new DataTable();
                    dt.Load(idr);
                    idr.Close();
                    foreach (DataRow dr in dt.Rows)
                    {
                        M_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    _log.Log(Level.SEVERE, sql, ex);
                }
                finally
                {
                    dt = null;
                    if (idr != null)
                    {
                        idr.Close();
                    }
                }
            }
            return M_Locator_ID;
        }

        /**
	 * 	Get Available Qty.
	 * 	The call is accurate only if there is a storage record 
	 * 	and assumes that the product is stocked 
	 *	@param M_Warehouse_ID wh
	 *	@param M_Product_ID product
	 *	@param M_AttributeSetInstance_ID masi
	 *	@param trxName transaction
	 *	@return qty available (QtyOnHand-QtyReserved) or null
	 */
        public static Decimal? GetQtyAvailable(int M_Warehouse_ID, int M_Product_ID,
             int M_AttributeSetInstance_ID, Trx trxName)
        {
            Decimal? retValue = null;
            DataSet ds = null;
            String sql = "SELECT SUM(QtyOnHand-QtyReserved) "
                + "FROM M_Storage s"
                + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
                + "WHERE s.M_Product_ID=" + M_Product_ID		//	#1
                + " AND l.M_Warehouse_ID=" + M_Warehouse_ID;
            if (M_AttributeSetInstance_ID != 0)
            {
                sql += " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }
            try
            {
                ds = DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    if (dr[0] == DBNull.Value)
                    {
                        //retValue = DBNull.Value;
                        retValue = null;
                    }
                    else
                    {
                        retValue = Util.GetValueOfDecimal(dr[0]);
                    }
                    if (dr[i] == null)
                    {
                        retValue = null;
                    }
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            _log.Fine("M_Warehouse_ID=" + M_Warehouse_ID
               + ",M_Product_ID=" + M_Product_ID + " = " + retValue);
            return retValue;
        }

        /*	Persistency Constructor
        *	@param ctx context
        *	@param ignored ignored
        *	@param trxName transaction
        */
        public MStorage(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
                throw new ArgumentException("Multi-Key");
            //
            SetQtyOnHand(Env.ZERO);
            SetQtyOrdered(Env.ZERO);
            SetQtyReserved(Env.ZERO);
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MStorage(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
         * 	Full NEW Constructor
         *	@param locator (parent) locator
         *	@param M_Product_ID product
         *	@param M_AttributeSetInstance_ID attribute
         */
        private MStorage(MLocator locator, int M_Product_ID, int M_AttributeSetInstance_ID)
            : this(locator.GetCtx(), 0, locator.Get_TrxName())
        {

            SetClientOrg(locator);
            SetM_Locator_ID(locator.GetM_Locator_ID());
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
        }

        /**
         * 	Change Qty OnHand
         *	@param qty quantity
         *	@param add add if true 
         */
        public void ChangeQtyOnHand(Decimal qty, bool add)
        {
            if (qty == null || Env.Signum(qty) == 0)
                return;
            if (add)
                SetQtyOnHand(Decimal.Add(GetQtyOnHand(), qty));
            else
                SetQtyOnHand(Decimal.Subtract(GetQtyOnHand(), qty));
        }

        /**
         * 	Get M_Warehouse_ID of Locator
         *	@return warehouse
         */
        public int GetM_Warehouse_ID()
        {
            if (_M_Warehouse_ID == 0)
            {
                MLocator loc = MLocator.Get(GetCtx(), GetM_Locator_ID());
                _M_Warehouse_ID = loc.GetM_Warehouse_ID();
            }
            return _M_Warehouse_ID;
        }

        /**
         *	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MStorage[")
                .Append("M_Locator_ID=").Append(GetM_Locator_ID())
                    .Append(",M_Product_ID=").Append(GetM_Product_ID())
                    .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
                .Append(": OnHand=").Append(GetQtyOnHand())
                .Append(",Reserved=").Append(GetQtyReserved())
                .Append(",Ordered=").Append(GetQtyOrdered())
                .Append("]");
            return sb.ToString();
        }

        public static List<MStorage> GetMultipleForUpdate(Ctx ctx, int M_Locator_ID,
               int M_Product_ID, int M_AttributeSetInstance_ID,
               List<String> types, Trx trx)
        {
            return GetMultiple(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, types, true, trx);
        }

        private static List<MStorage> GetMultiple(Ctx ctx, int M_Locator_ID,
              int M_Product_ID, int M_AttributeSetInstance_ID,
              List<String> types, Boolean forUpdate, Trx trx)
        {
            List<MStorage> retValue = new List<MStorage>();
            StringBuilder qtyTypesSql = new StringBuilder(" ");
            //if (!types.isEmpty())
            if (types.Count > 0)
            {
                qtyTypesSql.Append("AND QtyType in (");
                foreach (var type in types)
                {
                    qtyTypesSql.Append("'");
                    qtyTypesSql.Append(type);//type.GetValue()
                    qtyTypesSql.Append("',");
                }
                //qtyTypesSql.deleteCharAt(qtyTypesSql.Length - 1);
                qtyTypesSql.Remove(qtyTypesSql.Length - 1, qtyTypesSql.Length);
                qtyTypesSql.Append("') ");
            }

            String sql = "SELECT * FROM M_Storage "
                    + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND ";
            if (M_AttributeSetInstance_ID == 0)
            {
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            }
            else
            {
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }
            if (qtyTypesSql.Length > 1)
            {
                sql += qtyTypesSql.ToString();
            }
            if (forUpdate)
            {
                sql += " FOR UPDATE ";
            }
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue.Add(new MStorage(ctx, dr, trx));
                }
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            if (retValue.Count == 0)//   isEmpty())
            {
                _log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                        + ", M_Product_ID=" + M_Product_ID
                        + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            else
            {
                _log.Fine("M_Locator_ID=" + M_Locator_ID + ", M_Product_ID="
                        + M_Product_ID + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            return retValue;
        }

        /// <summary>
        /// Static Get/update
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_Locator_ID"></param>
        /// <param name="M_Product_ID"></param>
        /// <param name="M_AttributeSetInstance_ID"></param>
        /// <param name="type"></param>
        /// <param name="trx"></param>
        /// <returns></returns>
        public static MStorage GetForRead(Ctx ctx, int M_Locator_ID,
                int M_Product_ID, int M_AttributeSetInstance_ID,
                String type, Trx trx)
        {
            return Get(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    type, false, trx);
        }

        private static MStorage Get(Ctx ctx, int M_Locator_ID,
               int M_Product_ID, int M_AttributeSetInstance_ID,
               String type, Boolean forUpdate, Trx trx)
        {
            MStorage retValue = null;
            String sql = "SELECT * FROM M_Storage "
                    + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND QtyType='" + type + "' AND ";//type.GetValue()
            if (M_AttributeSetInstance_ID == 0)
            {
                sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
            }
            else
            {
                sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
            }
            if (forUpdate)
            {
                sql += " FOR UPDATE ";
            }
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trx);
                DataTable dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                if (dt.Rows.Count > 0)
                {
                    retValue = new MStorage(ctx, dt.Rows[0], trx);
                }
            }
            catch (Exception ex)
            {
                s_log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
            }
            if (retValue == null)
                _log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                        + ", M_Product_ID=" + M_Product_ID
                        + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            else
            {
                _log.Fine("M_Locator_ID=" + M_Locator_ID + ", M_Product_ID="
                        + M_Product_ID + ", M_AttributeSetInstance_ID="
                        + M_AttributeSetInstance_ID);
            }
            return retValue;
        }

        public static Decimal GetForUpdate(Ctx ctx, int M_Warehouse_ID,
               int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
               int reservationAttributeSetInstance_ID, X_Ref_Quantity_Type type,
               Trx trx)
        {
            return Decimal.Zero;
        }
        public static MStorage GetForUpdate(Ctx ctx, int M_Locator_ID,
               int M_Product_ID, int M_AttributeSetInstance_ID,
               String type, Trx trx)
        {
            return Get(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID,
                    type, true, trx);
        }
        //Get Product Qty From Storage Where Attribute Is Not Selected.
        public static MStorage Get(Ctx ctx, int M_Locator_ID, int M_Product_ID, bool isfifo, Trx trxName)
        {
            MStorage retValue = null;
            String sql = "SELECT * FROM M_Storage "
                + "WHERE M_Product_ID=" + M_Product_ID + " AND M_Locator_ID=" + M_Locator_ID
                + " AND M_AttributeSetInstance_ID > 0"
                + " AND QtyOnHand >= 0 "
                + "ORDER BY M_AttributeSetInstance_ID";
            if (!isfifo)
                sql += " DESC";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    retValue = new MStorage(ctx, dr, trxName);
                }
            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, ex);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            if (retValue == null)
            {
                s_log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
                    + ", M_Product_ID=" + M_Product_ID);
            }
            else
            {
                s_log.Fine("M_Locator_ID=" + M_Locator_ID
                    + ", M_Product_ID=" + M_Product_ID);
            }
            return retValue;
        }
    }


    //public class MStorage : X_M_Storage
    //{ 
    //    #region Private variables
    //    // Log		
    //    private static VLogger _log = VLogger.GetVLogger(typeof(MStorage).FullName);		
    //    //private static CLogger		s_log = CLogger.getCLogger (MStorage.class);
    //    // Warehouse					
    //    private int _M_Warehouse_ID = 0;
    //    #endregion

    //    /*	Get Storage Info
    //    *	@param ctx context
    //    *	@param M_Locator_ID locator
    //    *	@param M_Product_ID product
    //    *	@param M_AttributeSetInstance_ID instance
    //    *	@param trxName transaction
    //    *	@return existing or null
    //    */
    //    public static MStorage Get(Ctx ctx, int M_Locator_ID, int M_Product_ID,
    //         int M_AttributeSetInstance_ID, String trxName)
    //    {
    //        MStorage retValue = null;
    //        String sql = "SELECT * FROM M_Storage "
    //            + "WHERE M_Locator_ID=" + M_Locator_ID + " AND M_Product_ID=" + M_Product_ID + " AND ";
    //        if (M_AttributeSetInstance_ID == 0)
    //            sql += "(M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + " OR M_AttributeSetInstance_ID IS NULL)";
    //        else
    //            sql += "M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                retValue = new MStorage(ctx, dr, trxName);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }

    //        if (retValue == null)
    //        {
    //            _log.Fine("Not Found - M_Locator_ID=" + M_Locator_ID
    //                + ", M_Product_ID=" + M_Product_ID + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID);
    //        }
    //        else
    //        {
    //            _log.Fine("M_Locator_ID=" + M_Locator_ID
    //                + ", M_Product_ID=" + M_Product_ID + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID);
    //        }
    //        return retValue;
    //    }

    //    /**
    //   * 	Get all Storages for Product with ASI
    //   *	@param ctx context
    //   *	@param M_Product_ID product
    //   *	@param M_Locator_ID locator
    //   *	@param FiFo first in-first-out
    //   *	@param trxName transaction
    //   *	@return existing or null
    //   */
    //    public static MStorage[] GetAllWithASI(Ctx ctx, int M_Product_ID, int M_Locator_ID,
    //        bool FiFo, String trxName)
    //    {
    //        List<MStorage> list = new List<MStorage>();
    //        String sql = "SELECT * FROM M_Storage "
    //            + "WHERE M_Product_ID=" + M_Product_ID + " AND M_Locator_ID=" + M_Locator_ID
    //            + " AND M_AttributeSetInstance_ID > 0"
    //            + " AND QtyOnHand > 0 "
    //            + "ORDER BY M_AttributeSetInstance_ID";
    //        if (!FiFo)
    //            sql += " DESC";
    //        DataSet ds = new DataSet();
    //        try
    //        {
    //            ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
    //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
    //            {
    //                DataRow dr = ds.Tables[0].Rows[i];
    //                list.Add(new MStorage(ctx, dr, trxName));
    //            }
    //            ds = null;
    //        }
    //        catch (Exception ex)
    //        {
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        ds = null;
    //        MStorage[] retValue = new MStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /*	Get all Storages for Product
    //    *	@param ctx context
    //    *	@param M_Product_ID product
    //    *	@param M_Locator_ID locator
    //    *	@param trxName transaction
    //    *	@return existing or null
    //    */
    //    public static MStorage[] GetAll(Ctx ctx, int M_Product_ID, int M_Locator_ID, String trxName)
    //    {
    //        List<MStorage> list = new List<MStorage>();
    //        String sql = "SELECT * FROM M_Storage "
    //            + "WHERE M_Product_ID=" + M_Product_ID + " AND M_Locator_ID=" + M_Locator_ID
    //            + " AND QtyOnHand <> 0 "
    //            + "ORDER BY M_AttributeSetInstance_ID";
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //             idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                list.Add(new MStorage(ctx, dr, trxName));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }
    //        MStorage[] retValue = new MStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /*	Get Storage Info for Product across warehouses
    //    *	@param ctx context
    //    *	@param M_Product_ID product
    //    *	@param trxName transaction
    //    *	@return existing or null
    //    */
    //    public static MStorage[] GetOfProduct(Ctx ctx, int M_Product_ID, String trxName)
    //    {
    //        List<MStorage> list = new List<MStorage>();
    //        String sql = "SELECT * FROM M_Storage "
    //            + "WHERE M_Product_ID=" + M_Product_ID;
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //             idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                list.Add(new MStorage(ctx, dr, trxName));
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }

    //        MStorage[] retValue = new MStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /* 	Get Storage Info for Warehouse
    //   *	@param Ctx context
    //   *	@param M_Warehouse_ID 
    //   *	@param M_Product_ID product
    //   *	@param M_AttributeSetInstance_ID instance
    //   *	@param M_AttributeSet_ID attribute set
    //   *	@param allAttributeInstances if true, all attribute set instances
    //   *	@param minGuaranteeDate optional minimum guarantee date if all attribute instances
    //   *	@param FiFo first in-first-out
    //   *	@param trxName transaction
    //   *	@return existing - ordered by location priority (desc) and/or guarantee date
    //   */
    //    public static MStorage[] GetWarehouse(Ctx Ctx, int M_Warehouse_ID,
    //        int M_Product_ID, int M_AttributeSetInstance_ID, int M_AttributeSet_ID,
    //        bool allAttributeInstances, DateTime? minGuaranteeDate,
    //        bool FiFo, String trxName)
    //    {
    //        if (M_Warehouse_ID == 0 || M_Product_ID == 0)
    //            return new MStorage[0];

    //        if (M_AttributeSet_ID == 0)
    //            allAttributeInstances = true;
    //        else
    //        {
    //            MAttributeSet mas = MAttributeSet.Get(Ctx, M_AttributeSet_ID);
    //            if (!mas.IsInstanceAttribute())
    //                allAttributeInstances = true;
    //        }

    //        List<MStorage> list = new List<MStorage>();
    //        //	Specific Attribute Set Instance
    //        String sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
    //            + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
    //            + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
    //            + "FROM M_Storage s"
    //            + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID) "
    //            + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
    //            + " AND s.M_Product_ID=" + M_Product_ID
    //            + " AND COALESCE(s.M_AttributeSetInstance_ID,0)= " + M_AttributeSetInstance_ID
    //            + "ORDER BY l.PriorityNo DESC, M_AttributeSetInstance_ID";
    //        if (!FiFo)
    //        {
    //            sql += " DESC";
    //        }
    //        //	All Attribute Set Instances
    //        if (allAttributeInstances)
    //        {
    //            sql = "SELECT s.M_Product_ID,s.M_Locator_ID,s.M_AttributeSetInstance_ID,"
    //                + "s.AD_Client_ID,s.AD_Org_ID,s.IsActive,s.Created,s.CreatedBy,s.Updated,s.UpdatedBy,"
    //                + "s.QtyOnHand,s.QtyReserved,s.QtyOrdered,s.DateLastInventory "
    //                + "FROM M_Storage s"
    //                + " INNER JOIN M_Locator l ON (l.M_Locator_ID=s.M_Locator_ID)"
    //                + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID) "
    //                + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
    //                + " AND s.M_Product_ID=" + M_Product_ID;
    //            if (minGuaranteeDate != null)
    //            {
    //                sql += "AND (asi.GuaranteeDate IS NULL OR asi.GuaranteeDate>'" + String.Format("{0:dd-MMM-yy}", minGuaranteeDate) + "') "
    //                    + "ORDER BY asi.GuaranteeDate, M_AttributeSetInstance_ID";	//	Has Prio over Locator
    //                if (!FiFo)
    //                    sql += " DESC";
    //                sql += ", l.PriorityNo DESC, s.QtyOnHand DESC";
    //            }
    //            else
    //            {
    //                sql += "ORDER BY l.PriorityNo DESC, l.M_Locator_ID, s.M_AttributeSetInstance_ID";
    //                if (!FiFo)
    //                    sql += " DESC";
    //                sql += ", s.QtyOnHand DESC";
    //            }
    //        }
    //        DataTable dt = null;
    //        IDataReader idr = null;
    //        try
    //        {
    //            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                list.Add(new MStorage(Ctx, dr, trxName));
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, e);
    //        }
    //        finally {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            dt = null;
    //        }
    //        MStorage[] retValue = new MStorage[list.Count];
    //        retValue = list.ToArray();
    //        return retValue;
    //    }

    //    /*	Create or Get Storage Info
    //    *	@param ctx context
    //    *	@param M_Locator_ID locator
    //    *	@param M_Product_ID product
    //    *	@param M_AttributeSetInstance_ID instance
    //    *	@param trxName transaction
    //    *	@return existing/new or null
    //    */
    //    public static MStorage GetCreate(Ctx ctx, int M_Locator_ID, int M_Product_ID,
    //         int M_AttributeSetInstance_ID, String trxName)
    //    {
    //        if (M_Locator_ID == 0)
    //        {
    //            throw new ArgumentException("M_Locator_ID=0");
    //        }
    //        if (M_Product_ID == 0)
    //        {
    //            throw new ArgumentException("M_Product_ID=0");
    //        }
    //        MStorage retValue = Get(ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, trxName);
    //        if (retValue != null)
    //            return retValue;

    //        //	Insert row based on locator
    //        MLocator locator = new MLocator(ctx, M_Locator_ID, trxName);
    //        if (locator.Get_ID() != M_Locator_ID)
    //        {
    //            throw new ArgumentException("Not found M_Locator_ID=" + M_Locator_ID);
    //        }
    //        retValue = new MStorage(locator, M_Product_ID, M_AttributeSetInstance_ID);
    //        retValue.Save(trxName);
    //        _log.Fine("New " + retValue);
    //        return retValue;
    //    }

    //    /* 	Update Storage Info add.
    //   * 	Called from MProjectIssue
    //   *	@param Ctx context
    //   *	@param M_Warehouse_ID warehouse
    //   *	@param M_Locator_ID locator
    //   *	@param M_Product_ID product
    //   *	@param M_AttributeSetInstance_ID AS Instance
    //   *	@param reservationAttributeSetInstance_ID reservation AS Instance
    //   *	@param diffQtyOnHand add on hand
    //   *	@param diffQtyReserved add reserved
    //   *	@param diffQtyOrdered add order
    //   *	@param trxName transaction
    //   *	@return true if updated
    //   */
    //    public static bool Add(Ctx Ctx, int M_Warehouse_ID, int M_Locator_ID,
    //        int M_Product_ID, int M_AttributeSetInstance_ID, int reservationAttributeSetInstance_ID,
    //        Decimal diffQtyOnHand,
    //        Decimal? diffQtyReserved, Decimal? diffQtyOrdered, String trxName)
    //    {
    //        MStorage storage = null;
    //        StringBuilder diffText = new StringBuilder("(");

    //        //	Get Storage
    //        if (storage == null)
    //        {
    //            storage = GetCreate(Ctx, M_Locator_ID, M_Product_ID, M_AttributeSetInstance_ID, trxName);
    //        }
    //        //	Verify
    //        if (storage.GetM_Locator_ID() != M_Locator_ID
    //            && storage.GetM_Product_ID() != M_Product_ID
    //            && storage.GetM_AttributeSetInstance_ID() != M_AttributeSetInstance_ID)
    //        {
    //            _log.Severe("No Storage found - M_Locator_ID=" + M_Locator_ID
    //            + ",M_Product_ID=" + M_Product_ID + ",ASI=" + M_AttributeSetInstance_ID);
    //            return false;
    //        }
    //        MStorage storageASI = null;
    //        if (M_AttributeSetInstance_ID != reservationAttributeSetInstance_ID)
    //        {
    //            int reservationM_Locator_ID = M_Locator_ID;
    //            if (reservationAttributeSetInstance_ID == 0)
    //            {
    //                MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
    //                reservationM_Locator_ID = wh.GetDefaultM_Locator_ID();
    //            }
    //            storageASI = Get(Ctx, reservationM_Locator_ID, M_Product_ID, reservationAttributeSetInstance_ID, trxName);
    //            if (storageASI == null)	//	create if not existing - should not happen
    //            {
    //                MProduct product = MProduct.Get(Ctx, M_Product_ID);
    //                int xM_Locator_ID = MProductLocator.GetFirstM_Locator_ID(product, M_Warehouse_ID);
    //                if (xM_Locator_ID == 0)
    //                {
    //                    MWarehouse wh = MWarehouse.Get(Ctx, M_Warehouse_ID);
    //                    xM_Locator_ID = wh.GetDefaultM_Locator_ID();
    //                }
    //                storageASI = GetCreate(Ctx, xM_Locator_ID,
    //                    M_Product_ID, reservationAttributeSetInstance_ID, trxName);
    //            }
    //        }
    //        bool changed = false;
    //        if (diffQtyOnHand != null && diffQtyOnHand != 0)
    //        {
    //            storage.SetQtyOnHand(Decimal.Add(storage.GetQtyOnHand(), diffQtyOnHand));
    //            diffText.Append("OnHand=").Append(diffQtyOnHand);
    //            changed = true;
    //        }
    //        //	Reserved Qty
    //        if (diffQtyReserved != null && diffQtyReserved != 0)
    //        {
    //            if (storageASI == null)
    //            {
    //                storage.SetQtyReserved(Decimal.Add(storage.GetQtyReserved(), (Decimal)diffQtyReserved));
    //            }
    //            else
    //            {
    //                storageASI.SetQtyReserved(Decimal.Add(storageASI.GetQtyReserved(), (Decimal)diffQtyReserved));
    //            }
    //            diffText.Append(" Reserved=").Append((Decimal)diffQtyReserved);
    //            changed = true;
    //        }
    //        if (diffQtyOrdered != null && diffQtyOrdered != 0)
    //        {
    //            if (storageASI == null)
    //                storage.SetQtyOrdered(Decimal.Add(storage.GetQtyOrdered(), (Decimal)diffQtyOrdered));
    //            else
    //                storageASI.SetQtyOrdered(Decimal.Add(storageASI.GetQtyOrdered(), (Decimal)diffQtyOrdered));
    //            diffText.Append(" Ordered=").Append((Decimal)diffQtyOrdered);
    //            changed = true;
    //        }
    //        if (changed)
    //        {
    //            diffText.Append(") -> ").Append(storage.ToString());
    //            _log.Fine(diffText.ToString());
    //            if (storageASI != null)
    //                storageASI.Save(trxName);		//	No AttributeSetInstance (reserved/ordered)
    //            return storage.Save(trxName);
    //        }

    //        return true;
    //    }

    //    /*	Get Location with highest Locator Priority and a sufficient OnHand Qty
    //    * 	@param M_Warehouse_ID warehouse
    //    * 	@param M_Product_ID product
    //    * 	@param M_AttributeSetInstance_ID asi
    //    * 	@param Qty qty
    //    *	@param trxName transaction
    //    * 	@return id
    //    */
    //    public static int GetM_Locator_ID(int M_Warehouse_ID, int M_Product_ID, 
    //        int M_AttributeSetInstance_ID, Decimal Qty, String trxName)
    //    {
    //        int M_Locator_ID = 0;
    //        int firstM_Locator_ID = 0;
    //        String sql = "SELECT s.M_Locator_ID, s.QtyOnHand "
    //            + "FROM M_Storage s"
    //            + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
    //            + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
    //            + " LEFT OUTER JOIN M_AttributeSet mas ON (p.M_AttributeSet_ID=mas.M_AttributeSet_ID) "
    //            + "WHERE l.M_Warehouse_ID=" + M_Warehouse_ID
    //            + " AND s.M_Product_ID=" + M_Product_ID
    //            + " AND (mas.IsInstanceAttribute IS NULL OR mas.IsInstanceAttribute='N' OR s.M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ")"
    //            + " AND l.IsActive='Y' "
    //            + "ORDER BY l.PriorityNo DESC, s.QtyOnHand DESC";
    //        IDataReader idr = null;
    //        DataTable dt = null;
    //        try
    //        {
    //            idr = DataBase.DB.ExecuteReader(sql, null, trxName);
    //            dt = new DataTable();
    //            dt.Load(idr);
    //            idr.Close();
    //            foreach (DataRow dr in dt.Rows)
    //            {
    //                Decimal QtyOnHand = Convert.ToDecimal(dr[1]);//.getBigDecimal(2);
    //                if (QtyOnHand != null && Qty.CompareTo(QtyOnHand) <= 0)
    //                {
    //                    M_Locator_ID = Convert.ToInt32(dr[0]);//.getInt(1);
    //                    break;
    //                }
    //                if (firstM_Locator_ID == 0)
    //                    firstM_Locator_ID = Convert.ToInt32(dr[0]); //dr.getInt(1);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //            _log.Log(Level.SEVERE, sql, ex);
    //        }
    //        finally {
    //            dt = null;
    //            if (idr != null)
    //            {
    //                idr.Close();
    //            }
    //        }
    //        if (M_Locator_ID != 0)
    //            return M_Locator_ID;
    //        return firstM_Locator_ID;
    //    }

    //    /**
    // * 	Get Available Qty.
    // * 	The call is accurate only if there is a storage record 
    // * 	and assumes that the product is stocked 
    // *	@param M_Warehouse_ID wh
    // *	@param M_Product_ID product
    // *	@param M_AttributeSetInstance_ID masi
    // *	@param trxName transaction
    // *	@return qty available (QtyOnHand-QtyReserved) or null
    // */
    //    public static Decimal? GetQtyAvailable(int M_Warehouse_ID, int M_Product_ID,
    //         int M_AttributeSetInstance_ID, String trxName)
    //    {
    //        Decimal? retValue = null;
    //        DataSet ds = null;
    //        String sql = "SELECT SUM(QtyOnHand-QtyReserved) "
    //            + "FROM M_Storage s"
    //            + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID) "
    //            + "WHERE s.M_Product_ID=" + M_Product_ID		//	#1
    //            + " AND l.M_Warehouse_ID=" + M_Warehouse_ID;
    //        if (M_AttributeSetInstance_ID != 0)
    //        {
    //            sql += " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;
    //        }
    //        try
    //        {
    //            ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
    //            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
    //            {
    //                DataRow dr = ds.Tables[0].Rows[i];
    //                if (dr[0] == DBNull.Value)
    //                {
    //                    //retValue = DBNull.Value;
    //                    retValue = null;
    //                }
    //                else
    //                {
    //                    retValue = Utility.Util.GetValueOfDecimal(dr[0]);
    //                }
    //                if (dr[i] == null)
    //                {
    //                    retValue = null;
    //                }
    //            }
    //            ds = null;
    //        }
    //        catch (Exception e)
    //        {
    //            _log.Log(Level.SEVERE, sql, e);
    //        }
    //        _log.Fine("M_Warehouse_ID=" + M_Warehouse_ID
    //           + ",M_Product_ID=" + M_Product_ID + " = " + retValue);
    //        return retValue;
    //    }

    //    /*	Persistency Constructor
    //    *	@param ctx context
    //    *	@param ignored ignored
    //    *	@param trxName transaction
    //    */
    //    public MStorage(Ctx ctx, int ignored, String trxName)
    //        : base(ctx, 0, trxName)
    //    {
    //        if (ignored != 0)
    //            throw new ArgumentException("Multi-Key");
    //        //
    //        SetQtyOnHand(Env.ZERO);
    //        SetQtyOrdered(Env.ZERO);
    //        SetQtyReserved(Env.ZERO);
    //    }

    //    /**
    //     * 	Load Constructor
    //     *	@param ctx context
    //     *	@param dr result set
    //     *	@param trxName transaction
    //     */
    //    public MStorage(Ctx ctx, DataRow dr, String trxName)
    //        : base(ctx, dr, trxName)
    //    {
    //    }

    //    /**
    //     * 	Full NEW Constructor
    //     *	@param locator (parent) locator
    //     *	@param M_Product_ID product
    //     *	@param M_AttributeSetInstance_ID attribute
    //     */
    //    private MStorage(MLocator locator, int M_Product_ID, int M_AttributeSetInstance_ID)
    //        : this(locator.GetCtx(), 0, locator.Get_TrxName())
    //    {

    //        SetClientOrg(locator);
    //        SetM_Locator_ID(locator.GetM_Locator_ID());
    //        SetM_Product_ID(M_Product_ID);
    //        SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
    //    }

    //    /**
    //     * 	Change Qty OnHand
    //     *	@param qty quantity
    //     *	@param add add if true 
    //     */
    //    public void ChangeQtyOnHand(Decimal qty, bool add)
    //    {
    //        if (qty == null || Env.Signum(qty) == 0)
    //            return;
    //        if (add)
    //            SetQtyOnHand(Decimal.Add(GetQtyOnHand(), qty));
    //        else
    //            SetQtyOnHand(Decimal.Subtract(GetQtyOnHand(), qty));
    //    }

    //    /**
    //     * 	Get M_Warehouse_ID of Locator
    //     *	@return warehouse
    //     */
    //    public int GetM_Warehouse_ID()
    //    {
    //        if (_M_Warehouse_ID == 0)
    //        {
    //            MLocator loc = MLocator.Get(GetCtx(), GetM_Locator_ID());
    //            _M_Warehouse_ID = loc.GetM_Warehouse_ID();
    //        }
    //        return _M_Warehouse_ID;
    //    }

    //    /**
    //     *	String Representation
    //     * 	@return info
    //     */
    //    public override String ToString()
    //    {
    //        StringBuilder sb = new StringBuilder("MStorage[")
    //            .Append("M_Locator_ID=").Append(GetM_Locator_ID())
    //                .Append(",M_Product_ID=").Append(GetM_Product_ID())
    //                .Append(",M_AttributeSetInstance_ID=").Append(GetM_AttributeSetInstance_ID())
    //            .Append(": OnHand=").Append(GetQtyOnHand())
    //            .Append(",Reserved=").Append(GetQtyReserved())
    //            .Append(",Ordered=").Append(GetQtyOrdered())
    //            .Append("]");
    //        return sb.ToString();
    //    }
    //}
}
