/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAsset
 * Purpose        : used for A_Asset table
 * Class Used     : X_A_Asset
 * Chronological    Development
 * Raghunandan     08-Jun-2009
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
using System.Globalization;
using System.Web.UI;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MAsset : X_A_Asset
    {
        #region private variables
        //	Logger							
        private static VLogger _log = VLogger.GetVLogger(typeof(MAsset).FullName);
        //	Product Info					
        private MProduct _product = null;
        #endregion

        /// <summary>
        /// Get Asset From Shipment.
        /// (Order.reverseCorrect)
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_InOutLine_ID">shipment line</param>
        /// <param name="trxName">transaction</param>
        /// <returns>asset or null</returns>
        public static MAsset GetFromShipment(Ctx ctx, int M_InOutLine_ID, Trx trxName)
        {
            MAsset retValue = null;
            String sql = "SELECT * FROM A_Asset WHERE M_InOutLine_ID=" + M_InOutLine_ID;
            DataSet ds = new DataSet();
            try
            {
                ds = DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    retValue = new MAsset(ctx, dr, trxName);
                }
                ds = null;
            }
            catch (Exception e)
            {
                 _log.Log(Level.SEVERE, sql, e);
            }

            return retValue;
        }

        /**
         * 	Create Trial Asset
         *	@param ctx context
         *	@param user user
         *	@param entityType entity type
         *	@return asset or null if no product found
         */
        public static MAsset GetTrial(Ctx ctx, MUser user, String entityType)
        {
            if (user == null)
            {
                _log.Warning("Cannot create Trial - No User");
                return null;
            }
            if (Utility.Util.IsEmpty(entityType))
            {
                _log.Warning("Cannot create Trial - No Entity Type");
                return null;
            }
            MProduct product = MProduct.GetTrial(ctx, entityType);
            if (product == null)
            {
                _log.Warning("No Trial for Entity Type=" + entityType);
                return null;
            }
            //
            DateTime now = Convert.ToDateTime(CommonFunctions.CurrentTimeMillis());
            //
            MAsset asset = new MAsset(ctx, 0, null);
            asset.SetClientOrg(user);
            asset.SetAssetServiceDate(now);
            asset.SetIsOwned(false);
            asset.SetIsTrialPhase(true);
            //
            MBPartner partner = new MBPartner(ctx, user.GetC_BPartner_ID(), null);
            String documentNo = "Trial";
            //	Value
            String value = partner.GetValue() + "_" + product.GetValue();
            if (value.Length > 40 - documentNo.Length)
                value = value.Substring(0, 40 - documentNo.Length) + documentNo;
            asset.SetValue(value);
            //	Name		MProduct.afterSave
            String name = "Trial " + partner.GetName() + " - " + product.GetName();
            if (name.Length > 60)
            {
                name = name.Substring(0, 60);
            }
            asset.SetName(name);
            //	Description
            String description = product.GetDescription();
            asset.SetDescription(description);

            //	User
            asset.SetAD_User_ID(user.GetAD_User_ID());
            asset.SetC_BPartner_ID(user.GetC_BPartner_ID());
            //	Product
            asset.SetM_Product_ID(product.GetM_Product_ID());
            asset.SetA_Asset_Group_ID(product.GetA_Asset_Group_ID());
            asset.SetQty(new Decimal(product.GetSupportUnits()));
            //	Guarantee & Version
            asset.SetGuaranteeDate(TimeUtil.AddDays(now, product.GetTrialPhaseDays()));
            asset.SetVersionNo(product.GetVersionNo());
            //
            return asset;
        }

        /* 	Asset Constructor
     *	@param ctx context
     *	@param A_Asset_ID asset
     *	@param trxName transaction name 
     */
        public MAsset(Ctx ctx, int A_Asset_ID, Trx trxName)
            : base(ctx, A_Asset_ID, trxName)
        {

            if (A_Asset_ID == 0)
            {
                SetIsDepreciated(false);
                SetIsFullyDepreciated(false);
                SetIsInPosession(false);
                SetIsOwned(false);
                SetIsDisposed(false);
                SetM_AttributeSetInstance_ID(0);
                SetQty(Env.ONE);
                SetIsTrialPhase(false);
            }
        }

        /**
         * 	Discontinued Asset Constructor - DO NOT USE (but don't delete either)
         *	@param ctx context
         *	@param A_Asset_ID asset
         *	@deprecated
         */
        public MAsset(Ctx ctx, int A_Asset_ID)
            : this(ctx, A_Asset_ID, null)
        {

        }

        /**
         *  Load Constructor
         *  @param ctx context
         *  @param dr result set record
         *	@param trxName transaction
         */
        public MAsset(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MAsset(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /**
        * 	Shipment Constructor
        * 	@param shipment shipment
        *	@param shipLine shipment line
        *	@param deliveryCount 0 or number of delivery
        */
        public MAsset(MInOut shipment, MInOutLine shipLine, int deliveryCount)
            : this(shipment.GetCtx(), 0, shipment.Get_TrxName())
        {

            SetClientOrg(shipment);

            SetValueNameDescription(shipment, shipLine, deliveryCount);
            //	Header

            SetIsOwned(true);
            SetC_BPartner_ID(shipment.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(shipment.GetC_BPartner_Location_ID());
            SetAD_User_ID(shipment.GetAD_User_ID());
            SetM_Locator_ID(shipLine.GetM_Locator_ID());
            SetIsInPosession(true);

            //	Line
            MProduct product = shipLine.GetProduct();
            SetM_Product_ID(product.GetM_Product_ID());
            SetA_Asset_Group_ID(product.GetA_Asset_Group_ID());

            //////////////////////////////*
            //Changes for vafam
            // SetAssetServiceDate(shipment.GetMovementDate());
            //SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            MAssetGroup _assetGroup = new MAssetGroup(GetCtx(), GetA_Asset_Group_ID(), shipment.Get_TrxName());
            if (_assetGroup.IsOwned())
            {
                SetIsOwned(true);
                //SetC_BPartner_ID(0);
            }
            if (_assetGroup.IsDepreciated())
            {
                SetIsDepreciated(true);
                SetIsFullyDepreciated(false);
            }            
            ////////////////////////////////////


            //	Guarantee & Version
            SetGuaranteeDate(TimeUtil.AddDays(shipment.GetMovementDate(), product.GetGuaranteeDays()));
            SetVersionNo(product.GetVersionNo());
            if (shipLine.GetM_AttributeSetInstance_ID() != 0)		//	Instance
            {
                MAttributeSetInstance asi = new MAttributeSetInstance(GetCtx(), shipLine.GetM_AttributeSetInstance_ID(), Get_TrxName());
                SetM_AttributeSetInstance_ID(asi.GetM_AttributeSetInstance_ID());
                SetLot(asi.GetLot());
                SetSerNo(asi.GetSerNo());
            }
            SetHelp(shipLine.GetDescription());
            //	Qty
            int units = product.GetSupportUnits();
            if (units == 0)
                units = 1;
            if (deliveryCount != 0)		//	one asset per UOM
                SetQty(shipLine.GetMovementQty(), units);
            else
                SetQty((Decimal)units);
            SetM_InOutLine_ID(shipLine.GetM_InOutLine_ID());

            //	Activate
            MAssetGroup ag = MAssetGroup.Get(GetCtx(), GetA_Asset_Group_ID());
            if (!ag.IsCreateAsActive())
                SetIsActive(false);
        }

        /**
        * 	Set Value Name Description
        *	@param shipment shipment
        *	@param line line
        *	@param deliveryCount
        */
        public void SetValueNameDescription(MInOut shipment, MInOutLine line, int deliveryCount)
        {
            MProduct product = line.GetProduct();
            MBPartner partner = shipment.GetBPartner();
            SetValueNameDescription(shipment, deliveryCount, product, partner);
        }

        /**
        * 	Set Value, Name, Description
        *	@param shipment shipment
        *	@param deliveryCount count
        *	@param product product
        *	@param partner partner
        */
        public void SetValueNameDescription(MInOut shipment,
            int deliveryCount, MProduct product, MBPartner partner)
        {
            String documentNo = "_" + shipment.GetDocumentNo();
            if (deliveryCount > 1)
                documentNo += "_" + deliveryCount;
            //	Value
            String value = partner.GetValue() + "_" + product.GetValue();
            if (value.Length > 40 - documentNo.Length)
                value = value.Substring(0, 40 - documentNo.Length) + documentNo;
            // Change to set Value from Document Sequence
            // SetValue(value);

            // Change to set only name of product as value in Asset 
            //	Name		MProduct.afterSave
            // String name = partner.GetName() + " - " + product.GetName();

            String name = product.GetName();
            if (name.Length > 60)
                name = name.Substring(0, 60);
            SetName(name);
            //	Description
            String description = product.GetDescription();
            SetDescription(description);
        }

        /**
       * 	Add to Description
       *	@param description text
       */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /* 	Get Qty
	 *	@return 1 or Qty
	 */
        public new Decimal GetQty()
        {
            Decimal qty = base.GetQty();
            if (qty == null || qty.Equals(Env.ZERO))
                SetQty(Env.ONE);
            return base.GetQty();
        }

        /**
         * 	Set Qty
         *	@param Qty quantity
         *	@param multiplier support units
         */
        public void SetQty(Decimal Qty, int multiplier)
        {
            if (multiplier == 0)
                multiplier = 1;
            Decimal mm = new Decimal(multiplier);
            base.SetQty(Decimal.Multiply(Qty, mm));
        }

        /**
         * 	Set Qty based on product * shipment line if exists
         */
        public void SetQty()
        {
            //	UPDATE M_Product SET SupportUnits=1 WHERE SupportUnits IS NULL OR SupportUnits<1;
            //	UPDATE A_Asset a SET Qty = (SELECT l.MovementQty * p.SupportUnits FROM M_InOutLine l, M_Product p WHERE a.M_InOutLine_ID=l.M_InOutLine_ID AND a.M_Product_ID=p.M_Product_ID) WHERE a.M_Product_ID IS NOT NULL AND a.M_InOutLine_ID IS NOT NULL;
            Decimal Qty = Env.ONE;
            if (GetM_InOutLine_ID() != 0)
            {
                MInOutLine line = new MInOutLine(GetCtx(), GetM_InOutLine_ID(), Get_TrxName());
                Qty = line.GetMovementQty();
            }
            int multiplier = GetProduct().GetSupportUnits();
            Decimal mm = new Decimal(multiplier);
            base.SetQty(Decimal.Multiply(Qty, mm));
        }

        /**
         * 	String representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAsset[")
                .Append(Get_ID())
                .Append("-").Append(GetValue())
                .Append("]");
            return sb.ToString();
        }

        /* 	Get Deliveries
    * 	@return deliveries
    */
        public MAssetDelivery[] GetDeliveries()
        {
            List<MAssetDelivery> list = new List<MAssetDelivery>();

            String sql = "SELECT * FROM A_Asset_Delivery WHERE A_Asset_ID=" + GetA_Asset_ID() + " ORDER BY Created DESC";
            DataTable dt = null;
            IDataReader idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
            try
            {
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MAssetDelivery(GetCtx(), dr, Get_TrxName()));
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            
            //
            MAssetDelivery[] retValue = new MAssetDelivery[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
         * 	Get Delivery count
         * 	@return delivery count
         */
        public int GetDeliveryCount()
        {
            String sql = "SELECT COUNT(*) FROM A_Asset_Delivery WHERE A_Asset_ID=" + GetA_Asset_ID();
            return Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /*
         * 	Can we download.
         * 	Based on guarantee date and availability of download
         * 	@return true if downloadable
         */
        public bool IsDownloadable()
        {
            if (IsActive(true))
            {
                GetProduct();
                return _product != null
                    && _product.HasDownloads();
            }
            //
            return false;
        }	//	isDownloadable

        /**
         * 	Is Active 
         *	@param checkDate check guarantee date
         *	@return true if active and within guarantee
         */
        public bool IsActive(bool checkDate)
        {
            if (!checkDate)
                return IsActive();
            if (!IsActive())
                return false;

            //	Guarantee Date
            DateTime? guarantee = GetGuaranteeDate();
            if (guarantee == null)
            {
                return false;
            }
            guarantee = TimeUtil.GetDay(guarantee);
            DateTime now = TimeUtil.GetDay(DateTime.Now);
            //	valid
            //return !now.after(guarantee);	//	not after guarantee date
            return !(now > guarantee);	//	not after guarantee date
        }

        /*
         * 	Get Product Version No
         *	@return VersionNo
         */
        public String GetProductVersionNo()
        {
            return GetProduct().GetVersionNo();
        }

        /**
         * 	Get Product R_MailText_ID
         *	@return R_MailText_ID
         */
        public int GetProductR_MailText_ID()
        {
            return GetProduct().GetR_MailText_ID();
        }

        /**
         * 	Get Product Info
         * 	@return product
         */
        private MProduct GetProduct()
        {
            if (_product == null)
                _product = MProduct.Get(GetCtx(), GetM_Product_ID());
            return _product;
        }

        /* 	Get Active Addl. Product Downloads
        *	@return array of downloads
        */
        public MProductDownload[] GetProductDownloads()
        {
            if (_product == null)
                GetProduct();
            if (_product != null)
                return _product.GetProductDownloads(false);
            return null;
        }	

        /**
         * 	Get Additional Download Names
         *	@return names
         */
        public String[] GetDownloadNames()
        {
            MProductDownload[] dls = GetProductDownloads();
            if (dls != null && dls.Length > 0)
            {
                String[] retValue = new String[dls.Length];
                for (int i = 0; i < retValue.Length; i++)
                    retValue[i] = dls[i].GetName();
                log.Fine("#" + dls.Length);
                return retValue;
            }
            return new String[] { };
        }

        /**
         * 	Get Additional Download URLs
         *	@return URLs
         */
        public String[] GetDownloadURLs()
        {
            MProductDownload[] dls = GetProductDownloads();
            if (dls != null && dls.Length > 0)
            {
                String[] retValue = new String[dls.Length];
                for (int i = 0; i < retValue.Length; i++)
                {
                    String url = dls[i].GetDownloadURL();
                    int pos = Math.Max(url.LastIndexOf('/'), url.LastIndexOf('\\'));
                    if (pos != -1)
                        url = url.Substring(pos + 1);
                    retValue[i] = url;
                }
                return retValue;
            }
            return new String[] { };
        }

        /**
         * 	Get Asset Group
         *	@return asset Group
         */
        public MAssetGroup GetAssetGroup()
        {
            return MAssetGroup.Get(GetCtx(), GetA_Asset_Group_ID());
        }	

        /**
         * 	Get SupportLevel
         *	@return support level or Unsupported
         */
        public String GetSupportLevel()
        {
            return GetAssetGroup().GetSupportLevel();
        }	

        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            GetQty();		//	set to 1
            return true;
        }	

        /**
         * 	Confirm Asset EMail Delivery
         *	@param email email sent
         * 	@param AD_User_ID recipient
         * 	@return asset delivery
         */
        public MAssetDelivery ConfirmDelivery(EMail email, int AD_User_ID)
        {
            SetVersionNo(GetProductVersionNo());
            MAssetDelivery ad = new MAssetDelivery(this, email, null, AD_User_ID);
            return ad;
        }

        /* 	Confirm Asset Download Delivery
        *	@param request request
        * 	@param AD_User_ID recipient
        * 	@return asset delivery
        */
        //public MAssetDelivery ConfirmDelivery(HttpServletRequest request, int AD_User_ID)
        //{
        //    SetVersionNo(GetProductVersionNo());
        //    SetLifeUseUnits(GetLifeUseUnits().add(Env.ONE));
        //    MAssetDelivery ad = new MAssetDelivery(this, request, AD_User_ID);
        //    return ad;
        //}

    }
}
