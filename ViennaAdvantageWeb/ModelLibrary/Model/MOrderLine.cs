/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_C_OrderLine
 * Chronological Development
 * Veena Pandey     19-May-2009
 * raghunandan      10-June-2009 (adding new functions in class)
 ******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Common;
using VAdvantage.Utility;
using System.Data;
using System.Windows.Forms;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.WF;
//using VAdvantage.Grid;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Order Line model.
    /// </summary>
    public class MOrderLine : X_C_OrderLine
    {
        #region Private variables
        private int _M_PriceList_ID = 0;
        private bool _IsSOTrx = true;
        private bool _IsReturnTrx = true;
        private static VLogger _log = VLogger.GetVLogger(typeof(MOrderLine).FullName);
        //	Product Pricing
        private MProductPricing _productPrice = null;
        //Cached Currency Precision	
        private int? _precision = null;
        //	Product					
        private MProduct _product = null;
        //Parent					
        private MOrder _parent = null;

        private int I_Order_ID = 0;
        #endregion

        /// <summary>
        /// Get Order Unreserved Qty
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_Warehouse_ID">wh</param>
        /// <param name="M_Product_ID">product</param>
        /// <param name="M_AttributeSetInstance_ID">asi</param>
        /// <param name="excludeC_OrderLine_ID">exclude C_OrderLine_ID</param>
        /// <returns>Unreserved Qty</returns>
        public static Decimal GetNotReserved(Ctx ctx, int M_Warehouse_ID,
            int M_Product_ID, int M_AttributeSetInstance_ID, int excludeC_OrderLine_ID)
        {
            Decimal retValue = Env.ZERO;
            String sql = "SELECT SUM(qtyOrdered-QtyDelivered-QtyReserved) "
                + "FROM C_OrderLine ol"
                + " INNER JOIN C_Order o ON (ol.C_Order_ID=o.C_Order_ID) "
                + "WHERE ol.M_Warehouse_ID=" + M_Warehouse_ID	//	#1
                + " AND M_Product_ID=" + M_Product_ID			//	#2
                + " AND o.IsSOTrx='Y' AND o.DocStatus='DR'"
                + " AND qtyOrdered-QtyDelivered-QtyReserved<>0"
                + " AND ol.C_OrderLine_ID<>" + excludeC_OrderLine_ID;
            if (M_AttributeSetInstance_ID != 0)
                sql += " AND M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID;

            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    retValue = Utility.Util.GetValueOfDecimal(idr[0]);
                }
                if (idr != null)
                    idr.Close();
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                    idr.Close();
            }
            if (retValue == null)
            {
                _log.Fine("-");
            }
            else
            {
                _log.Fine(retValue.ToString());
            }
            return retValue;
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_OrderLine_ID">order line to load</param>
        /// <param name="trxName">transction</param>
        public MOrderLine(Ctx ctx, int C_OrderLine_ID, Trx trxName)
            : base(ctx, C_OrderLine_ID, trxName)
        {
            if (C_OrderLine_ID == 0)
            {
                //	setC_Order_ID (0);
                //	setLine (0);
                //	setM_Warehouse_ID (0);	// @M_Warehouse_ID@
                //	setC_BPartner_ID(0);
                //	setC_BPartner_Location_ID (0);	// @C_BPartner_Location_ID@
                //	setC_Currency_ID (0);	// @C_Currency_ID@
                //	setDateOrdered (new Timestamp(System.currentTimeMillis()));	// @DateOrdered@
                //
                //	setC_Tax_ID (0);
                //	setC_UOM_ID (0);
                //
                SetFreightAmt(Env.ZERO);
                SetLineNetAmt(Env.ZERO);
                //
                SetPriceEntered(Env.ZERO);
                SetPriceActual(Env.ZERO);
                SetPriceLimit(Env.ZERO);
                SetPriceList(Env.ZERO);
                //
                SetM_AttributeSetInstance_ID(0);
                //
                SetQtyEntered(Env.ZERO);
                SetQtyOrdered(Env.ZERO);	// 1
                SetQtyDelivered(Env.ZERO);
                SetQtyInvoiced(Env.ZERO);
                SetQtyReserved(Env.ZERO);
                //
                SetIsDescription(false);	// N
                SetProcessed(false);
                SetLine(0);
            }
        }

        /// <summary>
        /// Parent Constructor.
        /// ol.setM_Product_ID(wbl.getM_Product_ID());
        /// ol.setQtyOrdered(wbl.getQuantity());
        /// ol.setPrice();
        /// ol.setPriceActual(wbl.getPrice());
        /// ol.setTax();
        /// ol.save();
        /// </summary>
        /// <param name="order">parent order</param>
        public MOrderLine(MOrder order)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {
            if (order.Get_ID() == 0)
                throw new ArgumentException("Header not saved");
            SetC_Order_ID(order.GetC_Order_ID());	//	parent
            SetOrder(order);
        }

        public MOrderLine(MOrder order, int p_I_Order_ID)
            : this(order.GetCtx(), 0, order.Get_TrxName())
        {
            if (order.GetC_Order_ID() != 0)
            {
                SetC_Order_ID(order.GetC_Order_ID());	//	parent
            }
            SetOrder(order);
            I_Order_ID = p_I_Order_ID;
        }

        public int GetI_Order_ID()
        {
            return I_Order_ID;
        }

        public MOrderLine GetRef_OrderLine()
        {
            String sql = "SELECT C_OrderLine_ID FROM C_OrderLine WHERE Ref_OrderLine_ID=@param1";
            MOrderLine line = null;

            int ii = DB.GetSQLValue(Get_TrxName(), sql, GetC_OrderLine_ID());
            if (ii > 0)
            {
                line = new MOrderLine(GetCtx(), ii, Get_TrxName());
            }
            return line;
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">set record</param>
        /// <param name="trxName">transaction</param>
        public MOrderLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        public MOrderLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Set Defaults from Order.
        /// Does not set Parent !!
        /// </summary>
        /// <param name="order">order</param>
        public void SetOrder(MOrder order)
        {
            SetClientOrg(order);
            SetC_BPartner_ID(order.GetC_BPartner_ID());
            SetC_BPartner_Location_ID(order.GetC_BPartner_Location_ID());
            SetM_Warehouse_ID(order.GetM_Warehouse_ID());
            SetDateOrdered(order.GetDateOrdered());
            SetDatePromised(order.GetDatePromised());
            SetC_Currency_ID(order.GetC_Currency_ID());
            SetHeaderInfo(order);	//	sets m_order
            //	Don't set Activity, etc as they are overwrites
        }

        /// <summary>
        /// Set Header Info
        /// </summary>
        /// <param name="order">order</param>
        public void SetHeaderInfo(MOrder order)
        {
            _parent = order;
            _precision = order.GetPrecision();
            _M_PriceList_ID = order.GetM_PriceList_ID();
            _IsSOTrx = order.IsSOTrx();
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>parent</returns>
        public MOrder GetParent()
        {
            if (_parent == null)
                _parent = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
            return _parent;
        }

        /// <summary>
        /// Set Price Entered/Actual.
        /// Use this Method if the Line UOM is the Product UOM 
        /// </summary>
        /// <param name="priceActual">price</param>
        public void SetPrice(Decimal priceActual)
        {
            SetPriceEntered(priceActual);
            SetPriceActual(priceActual);
        }

        /// <summary>
        /// Set Price for Product and PriceList.
        /// Use only if newly created.
        /// Uses standard price list of not set by order constructor
        /// </summary>
        public void SetPrice()
        {
            if (GetM_Product_ID() == 0)
                return;
            if (_M_PriceList_ID == 0)
                throw new Exception("PriceList unknown!");
            SetPrice(_M_PriceList_ID);
        }

        /// <summary>
        /// Set Price for Product and PriceList
        /// </summary>
        /// <param name="M_PriceList_ID">price list</param>
        public void SetPrice(int M_PriceList_ID)
        {
            if (GetM_Product_ID() == 0)
                return;
            log.Fine(ToString() + " - M_PriceList_ID=" + M_PriceList_ID);
            GetProductPricing(M_PriceList_ID);
            SetPriceActual(_productPrice.GetPriceStd());
            SetPriceList(_productPrice.GetPriceList());
            SetPriceLimit(_productPrice.GetPriceLimit());
            //
            if (GetQtyEntered().CompareTo(GetQtyOrdered()) == 0)
            {
                SetPriceEntered(GetPriceActual());
            }
            else
            {
                //SetPriceEntered(GetPriceActual().multiply(getQtyOrdered().divide(getQtyEntered(), 12, BigDecimal.ROUND_HALF_UP)));	//	recision
                SetPriceEntered(Decimal.Multiply(GetPriceActual(), Decimal.Divide(GetQtyOrdered(), Decimal.Round(GetQtyEntered(), 12, MidpointRounding.AwayFromZero))));
            }
            //	Calculate Discount
            SetDiscount(_productPrice.GetDiscount());

            //	Set UOM
            // gwu: only set UOM if not already set
            if (GetC_UOM_ID() == 0)
                SetC_UOM_ID(_productPrice.GetC_UOM_ID());
        }

        /// <summary>
        /// Get and calculate Product Pricing
        /// </summary>
        /// <param name="M_PriceList_ID">id</param>
        /// <returns>product pricing</returns>
        private MProductPricing GetProductPricing(int M_PriceList_ID)
        {
            _productPrice = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                GetM_Product_ID(), GetC_BPartner_ID(), GetQtyOrdered(), _IsSOTrx);
            _productPrice.SetM_PriceList_ID(M_PriceList_ID);
            //Amit 24-nov-2014
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'")) > 0)
            {
                _productPrice.SetC_UOM_ID(GetC_UOM_ID());
            }
            //Amit
            _productPrice.SetPriceDate(GetDateOrdered());
            _productPrice.SetC_UOM_ID(GetC_UOM_ID());
            _productPrice.CalculatePrice();
            return _productPrice;
        }

        /// <summary>
        /// Set Tax
        /// </summary>
        /// <returns>true if tax is set</returns>
        public bool SetTax()
        {
            // Change to Set Tax ID based on the VAT Engine Module
            // if (_IsSOTrx)
            // {
            MOrder inv = new MOrder(Env.GetCtx(), Util.GetValueOfInt(Get_Value("C_Order_ID")), null);
            
            string taxRule = string.Empty;
            int _CountVATAX = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX = 'VATAX_'"));

            string sql = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID=" + inv.GetAD_Org_ID() + " AND IsActive ='Y' AND AD_Client_ID =" + Env.GetCtx().GetAD_Client_ID();
            if (_CountVATAX > 0)
            {
                taxRule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
            }
            // if (taxRule == "T" && _IsSOTrx)
            if (taxRule == "T" && ((_IsSOTrx && !_IsReturnTrx) || (!_IsSOTrx && !_IsReturnTrx)))
            {
                sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner_Location WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() +
                               " AND IsActive = 'Y'  AND C_BPartner_Location_ID = " + inv.GetC_BPartner_Location_ID();
                int taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (taxType == 0)
                {
                    sql = @"SELECT VATAX_TaxType_ID FROM C_BPartner WHERE C_BPartner_ID =" + inv.GetC_BPartner_ID() + " AND IsActive = 'Y'";
                    taxType = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                }
                MProduct prod = new MProduct(Env.GetCtx(), System.Convert.ToInt32(GetM_Product_ID()), null);
                sql = "SELECT C_Tax_ID FROM VATAX_TaxCatRate WHERE C_TaxCategory_ID = " + prod.GetC_TaxCategory_ID() + " AND IsActive ='Y' AND VATAX_TaxType_ID =" + taxType;
                int taxId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                if (taxId > 0)
                {
                    SetC_Tax_ID(taxId);
                    return true;
                }
                return false;
            }
            // }
            // }
            else
            {
                int ii = Tax.Get(GetCtx(), GetM_Product_ID(), GetC_Charge_ID(),
                    GetDateOrdered(), GetDateOrdered(),
                    GetAD_Org_ID(), GetM_Warehouse_ID(),
                    GetC_BPartner_Location_ID(),		//	should be bill to
                    GetC_BPartner_Location_ID(), _IsSOTrx);
                if (ii == 0)
                {
                    log.Log(Level.SEVERE, "No Tax found");
                    return false;
                }
                SetC_Tax_ID(ii);
            }
            return true;
        }

        /// <summary>
        /// Set Tax - (Callout follow-up)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        /// <returns>true</returns>
        private bool SetTax(int windowNo, String columnName)
        {
            //	Check Product
            int M_Product_ID = GetM_Product_ID();
            int C_Charge_ID = GetC_Charge_ID();
            log.Fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
            if (M_Product_ID == 0 && C_Charge_ID == 0)
            {
                return SetAmt(windowNo, columnName);		//	true
            }

            //	Check Partner Location
            int shipC_BPartner_Location_ID = GetC_BPartner_Location_ID();
            if (shipC_BPartner_Location_ID == 0)
                return SetAmt(windowNo, columnName);		//
            log.Fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
            DateTime? billDate = GetDateOrdered();
            log.Fine("Bill Date=" + billDate);

            DateTime? shipDate = GetDatePromised();
            log.Fine("Ship Date=" + shipDate);

            int AD_Org_ID = GetAD_Org_ID();
            log.Fine("Org=" + AD_Org_ID);

            int M_Warehouse_ID = GetM_Warehouse_ID();
            log.Fine("Warehouse=" + M_Warehouse_ID);

            int billC_BPartner_Location_ID = GetCtx().GetContextAsInt(windowNo, "Bill_Location_ID");
            if (billC_BPartner_Location_ID == 0)
                billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
            log.Fine("Bill BP_Location=" + billC_BPartner_Location_ID);
            //
            int C_Tax_ID = Tax.Get(GetCtx(), M_Product_ID, C_Charge_ID, billDate, shipDate,
                AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
                GetCtx().IsSOTrx(windowNo));
            log.Info("Tax ID=" + C_Tax_ID);
            //
            if (C_Tax_ID == 0)
            {
                // MessageBox.Show("Tax Error--error return from CLogger class and add in p_changeVO");
                //ValueNamePair pp = CLogger.retrieveError();
                //if (pp != null)
                //{
                //    p_changeVO.addError(pp.getValue());
                //}
                //else
                //{
                //    p_changeVO.addError("Tax Error");
                //}
            }
            else
                base.SetC_Tax_ID(C_Tax_ID);
            return SetAmt(windowNo, columnName);
        }

        /// <summary>
        /// Calculate Extended Amt.
        /// May or may not include tax
        /// </summary>
        public void SetLineNetAmt()
        {
            Decimal LineNetAmt = Decimal.Multiply(GetPriceActual(), GetQtyEntered());

            if (Env.Scale(LineNetAmt) > GetPrecision())
            {
                LineNetAmt = Decimal.Round(LineNetAmt, GetPrecision(), MidpointRounding.AwayFromZero);
                base.SetLineNetAmt(LineNetAmt);
            }
            else
            {
                base.SetLineNetAmt(LineNetAmt);
            }
        }

        private decimal BreakCalculation(int ProductId, int ClientId, decimal amount, int DiscountSchemaId, decimal FlatDiscount, decimal? QtyEntered)
        {
            StringBuilder query = new StringBuilder();
            decimal amountAfterBreak = amount;
            query.Append(@"SELECT UNIQUE M_Product_Category_ID FROM M_Product WHERE IsActive='Y' AND M_Product_ID = " + ProductId);
            int productCategoryId = Util.GetValueOfInt(DB.ExecuteScalar(query.ToString(), null, null));
            bool isCalulate = false;

            #region Product Based
            query.Clear();
            query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_ID = " + ProductId
                                                                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
            DataSet dsDiscountBreak = new DataSet();
            dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsDiscountBreak != null)
            {
                if (dsDiscountBreak.Tables.Count > 0)
                {
                    if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        int m = 0;
                        decimal discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }
            #endregion

            #region Product Category Based
            query.Clear();
            query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID = " + productCategoryId
                                                                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
            dsDiscountBreak.Clear();
            dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsDiscountBreak != null)
            {
                if (dsDiscountBreak.Tables.Count > 0)
                {
                    if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        int m = 0;
                        decimal discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }
            #endregion

            #region Otherwise
            query.Clear();
            query.Append(@"SELECT M_Product_Category_ID , M_Product_ID , BreakValue , IsBPartnerFlatDiscount , BreakDiscount FROM M_DiscountSchemaBreak WHERE 
                                                                   M_DiscountSchema_ID = " + DiscountSchemaId + " AND M_Product_Category_ID IS NULL AND m_product_id IS NULL "
                                                                       + " AND IsActive='Y'  AND AD_Client_ID=" + ClientId + "Order BY BreakValue DESC");
            dsDiscountBreak.Clear();
            dsDiscountBreak = DB.ExecuteDataset(query.ToString(), null, null);
            if (dsDiscountBreak != null)
            {
                if (dsDiscountBreak.Tables.Count > 0)
                {
                    if (dsDiscountBreak.Tables[0].Rows.Count > 0)
                    {
                        int m = 0;
                        decimal discountBreakValue = 0;

                        for (m = 0; m < dsDiscountBreak.Tables[0].Rows.Count; m++)
                        {
                            if (QtyEntered.Value.CompareTo(Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakValue"])) < 0)
                            {
                                continue;
                            }
                            if (Util.GetValueOfString(dsDiscountBreak.Tables[0].Rows[0]["IsBPartnerFlatDiscount"]) == "N")
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, Util.GetValueOfDecimal(dsDiscountBreak.Tables[0].Rows[m]["BreakDiscount"])), 100));
                                break;
                            }
                            else
                            {
                                isCalulate = true;
                                discountBreakValue = Decimal.Subtract(amount, Decimal.Divide(Decimal.Multiply(amount, FlatDiscount), 100));
                                break;
                            }
                        }
                        if (isCalulate)
                        {
                            amountAfterBreak = discountBreakValue;
                            return amountAfterBreak;
                        }
                    }
                }
            }
            #endregion

            return amountAfterBreak;
        }

        /// <summary>
        /// Get Currency Precision from Currency
        /// </summary>
        /// <returns>precision</returns>
        public int GetPrecision()
        {
            if (_precision != null)
                return (int)_precision;
            //
            if (GetC_Currency_ID() == 0)
            {
                SetOrder(GetParent());
                if (_precision != null)
                    return (int)_precision;
            }
            if (GetC_Currency_ID() != 0)
            {
                MCurrency cur = MCurrency.Get(GetCtx(), GetC_Currency_ID());
                if (cur.Get_ID() != 0)
                {
                    _precision = (int)(cur.GetStdPrecision());
                    return (int)_precision;
                }
            }
            //	Fallback
            String sql = "SELECT c.StdPrecision "
                + "FROM C_Currency c INNER JOIN C_Order x ON (x.C_Currency_ID=c.C_Currency_ID) "
                + "WHERE x.C_Order_ID=" + GetC_Order_ID();

            int i = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
            _precision = i;
            return (int)_precision;
        }

        /// <summary>
        /// Set Product
        /// </summary>
        /// <param name="product">product</param>
        public void SetProduct(MProduct product)
        {
            _product = product;
            if (_product != null)
            {
                SetM_Product_ID(_product.GetM_Product_ID());
                SetC_UOM_ID(_product.GetC_UOM_ID());
            }
            else
            {
                SetM_Product_ID(0);
                Set_ValueNoCheck("C_UOM_ID", null);
            }
            SetM_AttributeSetInstance_ID(0);
        }

        /// <summary>
        /// Set M_Product_ID
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <param name="setUOM">set also UOM</param>
        public void SetM_Product_ID(int M_Product_ID, bool setUOM)
        {
            if (setUOM)
                SetProduct(MProduct.Get(GetCtx(), M_Product_ID));
            else
                base.SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(0);
        }

        /// <summary>
        /// Set Product and UOM
        /// </summary>
        /// <param name="M_Product_ID">product</param>
        /// <param name="C_UOM_ID">uom</param>
        public void SetM_Product_ID(int M_Product_ID, int C_UOM_ID)
        {
            base.SetM_Product_ID(M_Product_ID);
            if (C_UOM_ID != 0)
                base.SetC_UOM_ID(C_UOM_ID);
            SetM_AttributeSetInstance_ID(0);
        }

        /// <summary>
        ///Set Product - Callout 
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            int M_Product_ID = int.Parse(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            if (M_Product_ID == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            // Skip these steps for RMA. These fields are copied over from the orignal order instead.
            if (GetParent().IsReturnTrx())
                return;
            //
            SetC_Charge_ID(0);
            //	Set Attribute
            int M_AttributeSetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID && M_AttributeSetInstance_ID != 0)
                SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            else
                SetM_AttributeSetInstance_ID(0);

            /*****	Price Calculation see also qty	****/
            int C_BPartner_ID = GetCtx().GetContextAsInt(windowNo, "C_BPartner_ID");
            Decimal Qty = GetQtyOrdered();
            bool IsSOTrx = GetCtx().IsSOTrx(windowNo);
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    M_Product_ID, C_BPartner_ID, Qty, IsSOTrx);
            //
            int M_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_ID");
            pp.SetM_PriceList_ID(M_PriceList_ID);
            /** PLV is only accurate if PL selected in header */
            int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            DateTime? orderDate = GetDateOrdered();
            pp.SetPriceDate(orderDate);
            //
            SetPriceList(pp.GetPriceList());
            SetPriceLimit(pp.GetPriceLimit());
            SetPriceActual(pp.GetPriceStd());
            SetPriceEntered(pp.GetPriceStd());
            SetC_Currency_ID(pp.GetC_Currency_ID());
            SetDiscount(pp.GetDiscount());
            SetC_UOM_ID(pp.GetC_UOM_ID());
            SetQtyOrdered(GetQtyEntered());
            //if (p_changeVO != null)
            //{
            //    p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", pp.isEnforcePriceLimit());
            //    p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", pp.isDiscountSchema());
            //}

            /**********************************Eclips commmented code*******************************/
            //	Check/Update Warehouse Setting
            //	int M_Warehouse_ID = ctx.getContextAsInt( Env.WINDOW_INFO, "M_Warehouse_ID");
            //	Integer wh = (Integer)mTab.getValue("M_Warehouse_ID");
            //	if (wh.intValue() != M_Warehouse_ID)
            //	{
            //		mTab.setValue("M_Warehouse_ID", new Integer(M_Warehouse_ID));
            //		ADialog.warn(,WindowNo, "WarehouseChanged");
            //	}
            /**********************************Eclips commmented code*******************************/


            if (IsSOTrx)
            {
                MProduct product = GetProduct();
                if (product.IsStocked())
                {
                    Decimal qtyOrdered = GetQtyOrdered();
                    int M_Warehouse_ID = GetM_Warehouse_ID();
                    M_AttributeSetInstance_ID = GetM_AttributeSetInstance_ID();
                    Decimal available = (Decimal)MStorage.GetQtyAvailable
                        (M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    if (available == null)
                        available = Env.ZERO;
                    if (available == 0)
                    {
                        //p_changeVO.addError(Msg.GetMsg(GetCtx(), "NoQtyAvailable", "0"));
                    }
                    else if (available.CompareTo(qtyOrdered) < 0)
                    {
                        // p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", available.toString()));
                    }
                    else
                    {
                        int C_OrderLine_ID = GetC_OrderLine_ID();
                        Decimal notReserved = MOrderLine.GetNotReserved(GetCtx(),
                            M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = Env.ZERO;
                        //BigDecimal total = available.subtract(notReserved);
                        Decimal total = available - notReserved;
                        if (total.CompareTo(qtyOrdered) < 0)
                        {
                            String info = Msg.ParseTranslation(GetCtx(), "@QtyAvailable@=" + available
                                + " - @QtyNotReserved@=" + notReserved + " = " + total);
                            //p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", info));
                        }
                    }
                }
            }
            //
            SetTax(windowNo, "M_Product_ID");
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product or null</returns>
        public MProduct GetProduct()
        {
            if (_product == null && GetM_Product_ID() != 0)
                _product = MProduct.Get(GetCtx(), GetM_Product_ID());
            return _product;
        }

        /// <summary>
        /// Set M_AttributeSetInstance_ID
        /// </summary>
        /// <param name="M_AttributeSetInstance_ID">id</param>
        public new void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID == 0)		//	 0 is valid ID
                Set_Value("M_AttributeSetInstance_ID", 0);
            else
                base.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
        }

        /// <summary>
        /// Set Warehouse
        /// </summary>
        /// <param name="M_Warehouse_ID">warehouse</param>
        public new void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (GetM_Warehouse_ID() > 0 && GetM_Warehouse_ID() != M_Warehouse_ID && !CanChangeWarehouse())
            {
                log.Severe("Ignored - Already Delivered/Invoiced/Reserved");
            }
            else
            {
                base.SetM_Warehouse_ID(M_Warehouse_ID);
            }
        }

        /// <summary>
        ///	Set Partner Location - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetC_BPartner_Location_ID(String oldC_BPartner_Location_ID,
               String newC_BPartner_Location_ID, int windowNo)
        {
            if (newC_BPartner_Location_ID == null || newC_BPartner_Location_ID.Length == 0)
                return;
            int C_BPartner_Location_ID = int.Parse(newC_BPartner_Location_ID);
            if (C_BPartner_Location_ID == 0)
                return;
            //
            base.SetC_BPartner_Location_ID(C_BPartner_Location_ID);
            SetTax(windowNo, "C_BPartner_Location_ID");
        }

        /// <summary>
        ///	SSet UOM - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetC_UOM_ID(String oldC_UOM_ID, String newC_UOM_ID, int windowNo)
        {
            if (newC_UOM_ID == null || newC_UOM_ID.Length == 0)
                return;
            int C_UOM_ID = int.Parse(newC_UOM_ID);
            if (C_UOM_ID == 0)
                return;
            //
            base.SetC_UOM_ID(C_UOM_ID);
            SetQty(windowNo, "C_UOM_ID");
            SetAmt(windowNo, "C_UOM_ID");
        }

        /// <summary>
        ///	Set AttributeSet Instance - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetM_AttributeSetInstance_ID(String oldM_AttributeSetInstance_ID,
                String newM_AttributeSetInstance_ID, int windowNo)
        {
            if (newM_AttributeSetInstance_ID == null || newM_AttributeSetInstance_ID.Length == 0)
                return;
            int M_AttributeSetInstance_ID = int.Parse(newM_AttributeSetInstance_ID);
            if (M_AttributeSetInstance_ID == 0)
                return;
            base.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetQty(windowNo, "M_AttributeSetInstance_ID");
        }

        /// <summary>
        ///	Set Discount - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetDiscount(String oldDiscount, String newDiscount, int windowNo)
        {
            if (newDiscount == null || newDiscount.Length == 0)
                return;
            Decimal Discount = Convert.ToDecimal(newDiscount);
            base.SetDiscount(Discount);
            SetAmt(windowNo, "Discount");
        }

        /// <summary>
        ///	Set priceActual - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceActual(String oldPriceActual, String newPriceActual, int windowNo)
        {
            if (newPriceActual == null || newPriceActual.Length == 0)
                return;
            Decimal priceActual = Convert.ToDecimal(newPriceActual);
            base.SetPriceActual(priceActual);
            SetAmt(windowNo, "priceActual");
        }

        /// <summary>
        ///	Set priceEntered - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceEntered(String oldPriceEntered, String newPriceEntered, int windowNo)
        {
            if (newPriceEntered == null || newPriceEntered.Length == 0)
                return;
            Decimal priceEntered = Convert.ToDecimal(newPriceEntered);
            base.SetPriceEntered(priceEntered);
            SetAmt(windowNo, "priceEntered");
        }

        /// <summary>
        ///	Set PriceList - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetPriceList(String oldPriceList, String newPriceList, int windowNo)
        {
            if (newPriceList == null || newPriceList.Length == 0)
                return;
            Decimal PriceList = Convert.ToDecimal(newPriceList);
            base.SetPriceList(PriceList);
            SetAmt(windowNo, "PriceList");
        }

        /// <summary>
        ///	Set qtyEntered - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetQtyEntered(String oldQtyEntered, String newQtyEntered, int windowNo)
        {
            if (newQtyEntered == null || newQtyEntered.Length == 0)
                return;
            Decimal qtyEntered = Convert.ToDecimal(newQtyEntered);
            base.SetQtyEntered(qtyEntered);
            SetQty(windowNo, "QtyEntered");
            SetAmt(windowNo, "QtyEntered");
        }

        /// <summary>
        ///	Set qtyOrdered - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetQtyOrdered(String oldQtyOrdered, String newQtyOrdered, int windowNo)
        {
            if (newQtyOrdered == null || newQtyOrdered.Length == 0)
                return;
            Decimal qtyOrdered = Convert.ToDecimal(newQtyOrdered);
            base.SetQtyOrdered(qtyOrdered);
            SetQty(windowNo, "QtyOrdered");
            SetAmt(windowNo, "QtyOrdered");
        }

        /// <summary>
        ///	Set Resource Assignment - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetS_ResourceAssignment_ID(String oldS_ResourceAssignment_ID, String newS_ResourceAssignment_ID, int windowNo)
        {
            if (newS_ResourceAssignment_ID == null || newS_ResourceAssignment_ID.Length == 0)
                return;
            int S_ResourceAssignment_ID = int.Parse(newS_ResourceAssignment_ID);
            if (S_ResourceAssignment_ID == 0)
                return;
            //
            base.SetS_ResourceAssignment_ID(S_ResourceAssignment_ID);

            int M_Product_ID = 0;
            String Name = null;
            String Description = null;
            Decimal? Qty = null;
            String sql = "SELECT p.M_Product_ID, ra.Name, ra.Description, ra.Qty "
                + "FROM S_ResourceAssignment ra"
                + " INNER JOIN M_Product p ON (p.S_Resource_ID=ra.S_Resource_ID) "
                + "WHERE ra.S_ResourceAssignment_ID=" + S_ResourceAssignment_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    M_Product_ID = Utility.Util.GetValueOfInt(dr[0].ToString());//.getInt (1);
                    Name = dr[1].ToString();//.getString(2);
                    Description = dr[2].ToString();//.getString(3);
                    Qty = Utility.Util.GetValueOfDecimal(dr[3].ToString());//.getBigDecimal(4);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }

            log.Fine("S_ResourceAssignment_ID=" + S_ResourceAssignment_ID
                    + " - M_Product_ID=" + M_Product_ID);
            if (M_Product_ID != 0)
            {
                SetM_Product_ID(M_Product_ID);
                if (Description != null)
                    Name += " (" + Description + ")";
                if (!".".Equals(Name))
                    SetDescription(Name);
                if (Qty != null)
                {
                    SetQtyOrdered((Decimal)Qty);
                }
            }
        }

        /// <summary>
        /// Set Amount (Callout)
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        /// <returns></returns>
        private bool SetAmt(int windowNo, String columnName)
        {
            int C_UOM_To_ID = GetC_UOM_ID();
            int M_Product_ID = GetM_Product_ID();
            int M_PriceList_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_ID");
            int StdPrecision = MPriceList.GetPricePrecision(GetCtx(), M_PriceList_ID);
            Decimal qtyEntered, qtyOrdered, priceEntered, priceActual, PriceLimit, Discount, PriceList;
            //	get values
            qtyEntered = GetQtyEntered();
            qtyOrdered = GetQtyOrdered();
            log.Fine("qtyEntered=" + qtyEntered + ", Ordered=" + qtyOrdered + ", UOM=" + C_UOM_To_ID);
            //
            priceEntered = GetPriceEntered();
            priceActual = GetPriceActual();
            Discount = GetDiscount();
            PriceLimit = GetPriceLimit();
            PriceList = GetPriceList();
            log.Fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            log.Fine("priceEntered=" + priceEntered + ", Actual=" + priceActual + ", Discount=" + Discount);

            //	Qty changed - recalc price
            if ((columnName.Equals("QtyOrdered")
                || columnName.Equals("QtyEntered")
                || columnName.Equals("M_Product_ID"))
                && !"N".Equals(GetCtx().GetContext(windowNo, "DiscountSchema")))
            {
                int C_BPartner_ID = GetC_BPartner_ID();
                if (columnName.Equals("QtyEntered"))
                    qtyOrdered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyEntered);
                if (qtyOrdered == null)
                    qtyOrdered = qtyEntered;
                bool IsSOTrx = GetCtx().IsSOTrx(windowNo);
                MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                        M_Product_ID, C_BPartner_ID, qtyOrdered, IsSOTrx);
                pp.SetM_PriceList_ID(M_PriceList_ID);
                int M_PriceList_Version_ID = GetCtx().GetContextAsInt(windowNo, "M_PriceList_Version_ID");
                pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                DateTime? date = GetDateOrdered();
                pp.SetPriceDate(date);
                //
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, pp.GetPriceStd());
                if (priceEntered == null)
                    priceEntered = pp.GetPriceStd();
                //
                log.Fine("QtyChanged -> priceActual=" + pp.GetPriceStd()
                    + ", priceEntered=" + priceEntered + ", Discount=" + pp.GetDiscount());
                priceActual = pp.GetPriceStd();
                SetPriceActual(priceActual);
                SetDiscount(pp.GetDiscount());
                SetPriceEntered(priceEntered);
                //p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", pp.isDiscountSchema());
            }
            else if (columnName.Equals("priceActual"))
            {
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                    C_UOM_To_ID, priceActual);
                if (priceEntered == null)
                    priceEntered = priceActual;
                //
                log.Fine("priceActual=" + priceActual
                    + " -> priceEntered=" + priceEntered);
                SetPriceEntered(priceEntered);
            }
            else if (columnName.Equals("priceEntered"))
            {
                priceActual = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID,
                    C_UOM_To_ID, priceEntered);
                if (priceActual == null)
                    priceActual = priceEntered;
                //
                log.Fine("priceEntered=" + priceEntered
                  + " -> priceActual=" + priceActual);
                SetPriceActual(priceActual);
            }

            //  Discount entered - Calculate Actual/Entered
            if (columnName.Equals("Discount"))
            {
                //priceActual = new BigDecimal((100.0 - Discount.doubleValue())/ 100.0 * PriceList.doubleValue());
                priceActual = (Decimal)(100.0 - Decimal.ToDouble(Discount) / 100.0 * Decimal.ToDouble(PriceList));
                if (Env.Scale(priceActual) > StdPrecision)
                {
                    priceActual = Decimal.Round(priceActual, StdPrecision, MidpointRounding.AwayFromZero);
                }
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, priceActual);
                if (priceEntered == null)
                    priceEntered = priceActual;
                SetPriceActual(priceActual);
                SetPriceEntered(priceEntered);
            }
            //	calculate Discount
            else
            {
                if (PriceList == 0)
                {
                    Discount = Env.ZERO;
                }
                else
                {
                    //Discount = new BigDecimal((PriceList.doubleValue() - priceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    Discount = (Decimal)((Decimal.ToDouble(PriceList) - Decimal.ToDouble(priceActual)) / Decimal.ToDouble(PriceList) * 100.0);
                }
                if (Env.Scale(Discount) > 2)
                {
                    Discount = Decimal.Round(Discount, 2, MidpointRounding.AwayFromZero);
                }
                SetDiscount(Discount);
            }
            log.Fine("priceEntered=" + priceEntered + ", Actual=" + priceActual + ", Discount=" + Discount);

            //	Check PriceLimit
            bool epl = "Y".Equals(GetCtx().GetContext(windowNo, "EnforcePriceLimit"));
            bool enforce = epl && GetCtx().IsSOTrx(windowNo);
            if (enforce && MRole.GetDefault(GetCtx()).IsOverwritePriceLimit())
                enforce = false;
            //	Check Price Limit?
            if (enforce && (Double)PriceLimit != 0.0 && priceActual.CompareTo(PriceLimit) < 0)
            {
                priceActual = PriceLimit;
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, PriceLimit);
                if (priceEntered == null)
                {
                    priceEntered = PriceLimit;
                }
                log.Fine("(under) priceEntered=" + priceEntered + ", Actual" + PriceLimit);
                SetPriceActual(PriceLimit);
                SetPriceEntered(priceEntered);
                //p_changeVO.addError(Msg.GetMsg(GetCtx(), "UnderLimitPrice"));
                //	Repeat Discount calc
                if (PriceList != 0)
                {
                    //Discount = new BigDecimal((PriceList.doubleValue() - priceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    Discount = (Decimal)((Double)PriceList - (Double)priceActual / (Double)PriceList * 100.0);
                    if (Env.Scale(Discount) > 2)
                    {
                        Discount = Decimal.Round(Discount, 2, MidpointRounding.AwayFromZero);
                    }
                    SetDiscount(Discount);
                }
            }
            //	Line Net Amt
            Decimal LineNetAmt = Decimal.Multiply(qtyOrdered, priceActual);
            if (Env.Scale(LineNetAmt) > StdPrecision)
            {
                LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision, MidpointRounding.AwayFromZero);
            }
            log.Info("LineNetAmt=" + LineNetAmt);
            SetLineNetAmt(LineNetAmt);
            return true;
        }

        /// <summary>
        /// Set Qty (Callout follow-up).
        /// enforces qty UOM relationship
        /// </summary>
        /// <param name="windowNo">window</param>
        /// <param name="columnName">changed column</param>
        /// <returns></returns>
        private bool SetQty(int windowNo, String columnName)
        {
            int M_Product_ID = GetM_Product_ID();
            Decimal qtyOrdered = Env.ZERO;
            //Decimal qtyEntered = null;
            Decimal qtyEntered = Env.ZERO;
            ;
            Decimal priceActual, priceEntered;
            int C_UOM_To_ID = GetC_UOM_ID();
            bool isReturnTrx = GetParent().IsReturnTrx();

            //	No Product
            if (M_Product_ID == 0)
            {
                qtyEntered = GetQtyEntered();
                qtyOrdered = (decimal)qtyEntered;
                SetQtyOrdered(qtyOrdered);
            }
            //	UOM Changed - convert from Entered -> Product
            else if (columnName.Equals("C_UOM_ID") || columnName.Equals("Orig_InOutLine_ID"))
            {
                qtyEntered = GetQtyEntered();
                //Decimal QtyEntered1 = qtyEntered.setScale(MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), BigDecimal.ROUND_HALF_UP);
                Decimal QtyEntered1 = Decimal.Round(qtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (qtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                       + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    SetQtyEntered(qtyEntered);
                }
                qtyOrdered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyEntered);
                if (qtyOrdered == null)
                {
                    qtyOrdered = qtyEntered;
                }
                bool conversion = qtyEntered.CompareTo(qtyOrdered) != 0;
                priceActual = GetPriceActual();
                priceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, priceActual);
                if (priceEntered == null)
                {
                    priceEntered = priceActual;
                }
                log.Fine("UOM=" + C_UOM_To_ID
                    + ", qtyEntered/priceActual=" + qtyEntered + "/" + priceActual
                    + " -> " + conversion
                   + " qtyOrdered/priceEntered=" + qtyOrdered + "/" + priceEntered);
                //p_changeVO.setContext(GetCtx(), windowNo, "UOMConversion", conversion);
                SetQtyOrdered(qtyOrdered);
                SetPriceEntered(priceEntered);
            }
            //	qtyEntered changed - calculate qtyOrdered
            else if (columnName.Equals("QtyEntered"))
            {
                qtyEntered = GetQtyEntered();
                Decimal QtyEntered1 = Decimal.Round(qtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                if (qtyEntered.CompareTo(QtyEntered1) != 0)
                {
                    log.Fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                       + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    SetQtyEntered(qtyEntered);
                }
                qtyOrdered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyEntered);
                if (qtyOrdered == null)
                {
                    qtyOrdered = qtyEntered;
                }
                bool conversion = qtyEntered.CompareTo(qtyOrdered) != 0;
                log.Fine("UOM=" + C_UOM_To_ID
                   + ", qtyEntered=" + qtyEntered
                    + " -> " + conversion
                    + " qtyOrdered=" + qtyOrdered);
                //p_changeVO.setContext(GetCtx(), windowNo, "UOMConversion", conversion);
                SetQtyOrdered(qtyOrdered);
            }
            //	qtyOrdered changed - calculate qtyEntered (should not happen)
            else if (columnName.Equals("QtyOrdered"))
            {
                qtyOrdered = GetQtyOrdered();
                int precision = GetProduct().GetUOMPrecision();
                //BigDecimal QtyOrdered1 = qtyOrdered.setScale(precision, BigDecimal.ROUND_HALF_UP);
                Decimal QtyOrdered1 = Decimal.Round(qtyOrdered, precision, MidpointRounding.AwayFromZero);
                if (qtyOrdered.CompareTo(QtyOrdered1) != 0)
                {
                    log.Fine("Corrected qtyOrdered Scale "
                        + qtyOrdered + "->" + QtyOrdered1);
                    qtyOrdered = QtyOrdered1;
                    SetQtyOrdered(qtyOrdered);
                }
                qtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyOrdered);
                if (qtyEntered == null)
                {
                    qtyEntered = qtyOrdered;
                }
                bool conversion = qtyOrdered.CompareTo(qtyEntered) != 0;
                log.Fine("UOM=" + C_UOM_To_ID
                    + ", qtyOrdered=" + qtyOrdered
                    + " -> " + conversion
                    + " qtyEntered=" + qtyEntered);
                //p_changeVO.setContext(GetCtx(), windowNo, "UOMConversion", conversion);
                SetQtyEntered(qtyEntered);
            }
            else
            {
                //	qtyEntered = getQtyEntered();
                qtyOrdered = GetQtyOrdered();
            }

            // RMA : Check qty returned is less than qty shipped
            if (M_Product_ID != 0 && isReturnTrx)
            {
                int inOutLine_ID = GetOrig_InOutLine_ID();
                if (inOutLine_ID != 0)
                {
                    MInOutLine inOutLine = new MInOutLine(GetCtx(), inOutLine_ID, null);
                    Decimal shippedQty = inOutLine.GetMovementQty();
                    qtyOrdered = GetQtyOrdered();
                    if (shippedQty.CompareTo(qtyOrdered) < 0)
                    {
                        if (GetCtx().IsSOTrx(windowNo))
                        {
                            // p_changeVO.addError(Msg.GetMsg(GetCtx(), "QtyShippedLessThanQtyReturned", shippedQty));
                        }
                        else
                        {
                            //p_changeVO.addError(Msg.GetMsg(GetCtx(), "QtyReceivedLessThanQtyReturned", shippedQty));
                        }

                        SetQtyOrdered(shippedQty);
                        qtyOrdered = shippedQty;

                        qtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID, C_UOM_To_ID, qtyOrdered);
                        if (qtyEntered == null)
                        {
                            qtyEntered = qtyOrdered;
                        }
                        SetQtyEntered(qtyEntered);
                        log.Fine("qtyEntered : " + qtyEntered.ToString() +
                               "qtyOrdered : " + qtyOrdered.ToString());
                    }
                }
            }

            //	Storage
            if (M_Product_ID != 0 && GetCtx().IsSOTrx(windowNo) && qtyOrdered > 0 && !isReturnTrx)		//	no negative (returns)
            {
                MProduct product = GetProduct();
                if (product.IsStocked())
                {
                    int M_Warehouse_ID = GetM_Warehouse_ID();
                    int M_AttributeSetInstance_ID = GetM_AttributeSetInstance_ID();
                    Decimal available = (Decimal)MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    if (available == null)
                    {
                        available = Env.ZERO;
                    }
                    if (available == 0)
                    {
                        //   p_changeVO.addError(Msg.GetMsg(GetCtx(), "NoQtyAvailable"));
                    }
                    else if (available.CompareTo(qtyOrdered) < 0)
                    {
                        // p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", available));
                    }
                    else
                    {
                        int C_OrderLine_ID = GetC_OrderLine_ID();
                        Decimal notReserved = MOrderLine.GetNotReserved(GetCtx(),
                            M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                            C_OrderLine_ID);
                        if (notReserved == null)
                        {
                            notReserved = Env.ZERO;
                        }
                        Decimal total = Decimal.Subtract(available, notReserved);
                        if (total.CompareTo(qtyOrdered) < 0)
                        {
                            //String info = Msg.parseTranslation(GetCtx(), "@QtyAvailable@=" + available
                            //    + "  -  @QtyNotReserved@=" + notReserved + "  =  " + total);
                            //p_changeVO.addError(Msg.GetMsg(GetCtx(), "InsufficientQtyAvailable", info));
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Can Change Warehouse
        /// </summary>
        /// <returns>true if warehouse can be changed</returns>
        public bool CanChangeWarehouse()
        {
            if (GetQtyDelivered() != 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + GetQtyDelivered());
                return false;
            }
            if (GetQtyInvoiced() != 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + GetQtyDelivered());
                return false;
            }
            if (GetQtyReserved() != 0)
            {
                log.SaveError("Error", Msg.Translate(GetCtx(), "QtyReserved") + "=" + GetQtyReserved());
                return false;
            }
            //	We can change
            return true;
        }

        /// <summary>
        /// Get C_Project_ID
        /// </summary>
        /// <returns>project</returns>
        public new int GetC_Project_ID()
        {
            int ii = base.GetC_Project_ID();
            if (ii == 0)
                ii = GetParent().GetC_Project_ID();
            return ii;
        }

        /// <summary>
        /// Get C_Activity_ID
        /// </summary>
        /// <returns>Activity</returns>
        public new int GetC_Activity_ID()
        {
            int ii = base.GetC_Activity_ID();
            if (ii == 0)
                ii = GetParent().GetC_Activity_ID();
            return ii;
        }

        /// <summary>
        /// Get C_Campaign_ID
        /// </summary>
        /// <returns>Campaign</returns>
        public new int GetC_Campaign_ID()
        {
            int ii = base.GetC_Campaign_ID();
            if (ii == 0)
                ii = GetParent().GetC_Campaign_ID();
            return ii;
        }

        /// <summary>
        /// Get User2_ID
        /// </summary>
        /// <returns>User2</returns>
        public new int GetUser1_ID()
        {
            int ii = base.GetUser1_ID();
            if (ii == 0)
                ii = GetParent().GetUser1_ID();
            return ii;
        }

        /// <summary>
        /// Get User2_ID
        /// </summary>
        /// <returns>User2</returns>
        public new int GetUser2_ID()
        {
            int ii = base.GetUser2_ID();
            if (ii == 0)
                ii = GetParent().GetUser2_ID();
            return ii;
        }

        /// <summary>
        /// Get AD_OrgTrx_ID
        /// </summary>
        /// <returns>trx org</returns>
        public new int GetAD_OrgTrx_ID()
        {
            int ii = base.GetAD_OrgTrx_ID();
            if (ii == 0)
                ii = GetParent().GetAD_OrgTrx_ID();
            return ii;
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MOrderLine[")
                .Append(Get_ID()).Append(",Line=").Append(GetLine())
                .Append(",Ordered=").Append(GetQtyOrdered())
                .Append(",Delivered=").Append(GetQtyDelivered())
                .Append(",Invoiced=").Append(GetQtyInvoiced())
                .Append(",Reserved=").Append(GetQtyReserved())
                .Append(", LineNet=").Append(GetLineNetAmt())
                .Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Add to description
        /// </summary>
        /// <param name="description">text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Description Text.
        /// For jsp access (vs. isDescription)
        /// </summary>
        /// <returns>description</returns>
        public String GetDescriptionText()
        {
            return base.GetDescription();
        }

        /// <summary>
        /// 	Get Name
        /// </summary>
        /// <returns>get the name of the line (from Product)</returns>
        public String GetName()
        {
            GetProduct();
            if (_product != null)
                return _product.GetName();
            if (GetC_Charge_ID() != 0)
            {
                MCharge charge = MCharge.Get(GetCtx(), GetC_Charge_ID());
                return charge.GetName();
            }
            return "";
        }

        /// <summary>
        /// Set C_Charge_ID
        /// </summary>
        /// <param name="C_Charge_ID">charge</param>
        public new void SetC_Charge_ID(int C_Charge_ID)
        {
            base.SetC_Charge_ID(C_Charge_ID);
            if (C_Charge_ID > 0)
                Set_ValueNoCheck("C_UOM_ID", null);
        }

        /// <summary>
        /// Set Charge - Callout
        /// </summary>
        /// <param name="oldC_Charge_ID">old value</param>
        /// <param name="newC_Charge_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetC_Charge_ID(String oldC_Charge_ID, String newC_Charge_ID, int windowNo)
        {
            if (newC_Charge_ID == null || newC_Charge_ID.Length == 0)
                return;
            int C_Charge_ID = int.Parse(newC_Charge_ID);
            if (C_Charge_ID == 0)
                return;
            // Skip these steps for RMA. These fields are copied over from the orignal order instead.
            if (GetParent().IsReturnTrx())
                return;
            //
            //	No Product defined
            if (GetM_Product_ID() != 0)
            {
                base.SetC_Charge_ID(0);
                //p_changeVO.addError(Msg.GetMsg(GetCtx(), "ChargeExclusively"));
                return;
            }

            base.SetC_Charge_ID(C_Charge_ID);
            SetM_AttributeSetInstance_ID(0);
            SetS_ResourceAssignment_ID(0);
            SetC_UOM_ID(100);	//	EA
            //p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", "N");
            String sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    SetPriceEntered(Convert.ToDecimal(dr[0]));//.getBigDecimal (1));
                    SetPriceActual(Convert.ToDecimal(dr[0]));//.getBigDecimal (1));
                    SetPriceLimit(Env.ZERO);
                    SetPriceList(Env.ZERO);
                    SetDiscount(Env.ZERO);
                }
            }
            catch (Exception e)
            {
                log.Log(Level.SEVERE, sql, e);
            }
            //
            SetTax(windowNo, "C_Charge_ID");
        }

        /// <summary>
        /// Set Discount
        /// </summary>
        public void SetDiscount()
        {
            Decimal list = GetPriceList();
            //	No List Price
            if (Env.ZERO.CompareTo(list) == 0)
                return;
            //BigDecimal discount = list.subtract(getPriceActual()).multiply(new BigDecimal(100)).divide(list, getPrecision(), BigDecimal.ROUND_HALF_UP);
            Decimal discount = Decimal.Round(Decimal.Divide(Decimal.Multiply(Decimal.Subtract(list, GetPriceEntered()), new Decimal(100)), list), GetPrecision(), MidpointRounding.AwayFromZero);
            SetDiscount(discount);
        }

        /// <summary>
        /// Is Tax Included in Amount
        /// </summary>
        /// <returns>true if tax calculated</returns>
        public bool IsTaxIncluded()
        {
            if (_M_PriceList_ID == 0)
            {
                _M_PriceList_ID = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar("SELECT M_PriceList_ID FROM C_Order WHERE C_Order_ID=" + GetC_Order_ID(), null, Get_TrxName()));
            }
            MPriceList pl = MPriceList.Get(GetCtx(), _M_PriceList_ID, Get_TrxName());
            return pl.IsTaxIncluded();
        }

        /// <summary>
        /// Set Qty Entered/Ordered.
        /// Use this Method if the Line UOM is the Product UOM
        /// </summary>
        /// <param name="qty">qtyOrdered/Entered</param>
        public void SetQty(Decimal qty)
        {
            base.SetQtyEntered(qty);
            base.SetQtyOrdered(GetQtyEntered());
        }

        /// <summary>
        /// Set Qty Entered - enforce entered UOM 
        /// </summary>
        /// <param name="qtyEntered">qtyEntered</param>
        public new void SetQtyEntered(Decimal qtyEntered)
        {
            if (qtyEntered != null && GetC_UOM_ID() != 0)
            {
                int precision = MUOM.GetPrecision(GetCtx(), GetC_UOM_ID());
                qtyEntered = Decimal.Round(qtyEntered, precision, MidpointRounding.AwayFromZero);
            }
            base.SetQtyEntered(qtyEntered);
        }

        /// <summary>
        /// Set Qty Ordered - enforce Product UOM 
        /// </summary>
        /// <param name="qtyOrdered"></param>
        public new void SetQtyOrdered(Decimal qtyOrdered)
        {
            MProduct product = GetProduct();
            if (qtyOrdered != null && product != null)
            {
                int precision = product.GetUOMPrecision();
                qtyOrdered = Decimal.Round(qtyOrdered, precision, MidpointRounding.AwayFromZero);
            }
            base.SetQtyOrdered(qtyOrdered);
        }

        /// <summary>
        /// 	Set Original OrderLine for RMA
        /// 	SOTrx should be set.
        /// </summary>
        /// <param name="origOrderLine"> MInOutLine</param>
        public void SetOrigOrderLine(MOrderLine origOrderLine)
        {
            if (origOrderLine == null || origOrderLine.Get_ID() == 0)
                return;
            SetOrig_InOutLine_ID(-1);
            SetC_Tax_ID(origOrderLine.GetC_Tax_ID());
            SetPriceList(origOrderLine.GetPriceList());
            SetPriceLimit(origOrderLine.GetPriceLimit());
            SetPriceActual(origOrderLine.GetPriceActual());
            SetPriceEntered(origOrderLine.GetPriceEntered());
            SetC_Currency_ID(origOrderLine.GetC_Currency_ID());
            SetDiscount(origOrderLine.GetDiscount());

            return;

        }

        /// <summary>
        /// Set Original Order Line - Callout
        /// </summary>
        /// <param name="oldOrig_OrderLine_ID">old Orig Order</param>
        /// <param name="newOrig_OrderLine_ID">new Orig Order</param>
        /// <param name="windowNo">window no</param>
        /// //@UICallout
        public void SetOrig_OrderLine_ID(String oldOrig_OrderLine_ID, String newOrig_OrderLine_ID, int windowNo)
        {
            if (newOrig_OrderLine_ID == null || newOrig_OrderLine_ID.Length == 0)
                return;
            int Orig_OrderLine_ID = int.Parse(newOrig_OrderLine_ID);
            if (Orig_OrderLine_ID == 0)
                return;

            // For returns, Price Limit is not enforced
            //p_changeVO.setContext(GetCtx(), windowNo, "EnforcePriceLimit", false);
            // For returns, discount is copied over from the sales order
            //p_changeVO.setContext(GetCtx(), windowNo, "DiscountSchema", false);

            //		Get Details
            MOrderLine oLine = new MOrderLine(GetCtx(), Orig_OrderLine_ID, null);
            if (oLine.Get_ID() != 0)
                SetOrigOrderLine(oLine);
        }

        /// <summary>
        /// 	Set Original Shipment Line for RMA
        /// 	SOTrx should be set.
        /// </summary>
        /// <param name="Orig_InOutLine">MInOutLine</param>
        public void SetOrigInOutLine(MInOutLine Orig_InOutLine)
        {
            if (Orig_InOutLine == null || Orig_InOutLine.Get_ID() == 0)
                return;

            SetC_Project_ID(Orig_InOutLine.GetC_Project_ID());
            SetC_Campaign_ID(Orig_InOutLine.GetC_Campaign_ID());
            SetM_Product_ID(Orig_InOutLine.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(Orig_InOutLine.GetM_AttributeSetInstance_ID());
            SetC_UOM_ID(Orig_InOutLine.GetC_UOM_ID());

            return;
        }

        /// <summary>
        ///Set Original Shipment Line - Callout
        /// </summary>
        /// <param name="oldC_Charge_ID">old value</param>
        /// <param name="newC_Charge_ID">new value</param>
        /// <param name="windowNo">window</param>
        //@UICallout
        public void SetOrig_InOutLine_ID(String oldOrig_InOutLine_ID, String newOrig_InOutLine_ID, int windowNo)
        {
            if (newOrig_InOutLine_ID == null || newOrig_InOutLine_ID.Length == 0)
                return;
            int Orig_InOutLine_ID = int.Parse(newOrig_InOutLine_ID);
            if (Orig_InOutLine_ID == 0)
                return;

            //		Get Details
            MInOutLine ioLine = new MInOutLine(GetCtx(), Orig_InOutLine_ID, null);
            if (ioLine.Get_ID() != 0)
                SetOrigInOutLine(ioLine);
            SetQty(windowNo, "Orig_InOutLine_ID");
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">newRecord</param>
        /// <returns>true if it can be saved</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            Decimal? PriceActual, PriceEntered;
            //	Get Defaults from Parent
            if (GetC_BPartner_ID() == 0 || GetC_BPartner_Location_ID() == 0
                || GetM_Warehouse_ID() == 0
                || GetC_Currency_ID() == 0)
                SetOrder(GetParent());
            if (_M_PriceList_ID == 0)
                SetHeaderInfo(GetParent());
            //	R/O Check - Product/Warehouse Change
            if (!newRecord && (Is_ValueChanged("M_Product_ID") || Is_ValueChanged("M_Warehouse_ID")))
            {
                if (!CanChangeWarehouse())
                    return false;
            }	//	Product Changed

            //	Charge
            MOrder Ord = new MOrder(Env.GetCtx(), GetC_Order_ID(), null);
            //	Qty on instance ASI for SO
            if (_IsSOTrx
                && GetM_AttributeSetInstance_ID() != 0
                && (newRecord || Is_ValueChanged("M_Product_ID")
                    || Is_ValueChanged("M_AttributeSetInstance_ID")
                    || Is_ValueChanged("M_Warehouse_ID")))
            {
                MProduct product = GetProduct();
                if (product.IsStocked())
                {
                    int M_AttributeSet_ID = product.GetM_AttributeSet_ID();
                    bool isInstance = M_AttributeSet_ID != 0;
                    if (isInstance)
                    {
                        MAttributeSet mas = MAttributeSet.Get(GetCtx(), M_AttributeSet_ID);
                        isInstance = mas.IsInstanceAttribute();
                    }
                    //	Max
                    if (isInstance)
                    {
                        MStorage[] storages = MStorage.GetWarehouse(GetCtx(),
                            GetM_Warehouse_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(),
                            M_AttributeSet_ID, false, null, true, Get_TrxName());
                        Decimal qty = Env.ZERO;
                        for (int i = 0; i < storages.Length; i++)
                        {
                            if (storages[i].GetM_AttributeSetInstance_ID() == GetM_AttributeSetInstance_ID())
                            {
                                qty = Decimal.Add(qty, storages[i].GetQtyOnHand());
                            }
                        }
                        if (GetQtyOrdered().CompareTo(qty) > 0)
                        {
                            log.Warning("Qty - Stock=" + qty + ", Ordered=" + GetQtyOrdered());
                            log.SaveError("QtyInsufficient", "=" + qty);
                            return false;
                        }
                    }
                }	//	stocked
            }	//	SO instance
            //else
            //{
            //    MProduct pro = new MProduct(GetCtx(), GetM_Product_ID(), null);
            //    String qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + GetM_Product_ID() + " AND vdr.C_BPartner_ID = " + Ord.GetC_BPartner_ID();
            //    int uom = Util.GetValueOfInt(DB.ExecuteScalar(qryUom));
            //    if (pro.GetC_UOM_ID() != 0)
            //    {
            //        if (pro.GetC_UOM_ID() != uom && uom != 0)
            //        {
            //            decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + GetM_Product_ID() + " AND IsActive='Y'"));
            //            if (Res > 0)
            //            {
            //                SetQtyEntered(GetQtyEntered() * Res);
            //                //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
            //            }
            //            else
            //            {
            //                decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
            //                if (res > 0)
            //                {
            //                    SetQtyEntered(GetQtyEntered() * res);
            //                    //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
            //                }
            //            }
            //            SetC_UOM_ID(uom);
            //        }
            //        else
            //        {
            //            SetC_UOM_ID(pro.GetC_UOM_ID());
            //        }
            //    }
            //}


            if (GetC_Charge_ID() != 0 && GetM_Product_ID() != 0)
                SetM_Product_ID(0);
            //	No Product
            if (GetM_Product_ID() == 0)
                SetM_AttributeSetInstance_ID(0);
            //	Product
            else	//	Set/check Product Price
            {
                //	Set Price if Actual = 0
                StringBuilder sql = new StringBuilder();
                Tuple<String, String, String> iInfo = null;
                if (Env.HasModulePrefix("ED011_", out iInfo))
                {
                    string qry = "SELECT M_PriceList_Version_ID FROM m_pricelist_version WHERE IsActive = 'Y' AND m_pricelist_id = " + _M_PriceList_ID + @" AND VALIDFROM <= sysdate order by validfrom desc";
                    int _Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                    sql.Append(@"SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + GetM_Product_ID()
                                         + " AND M_PriceList_Version_ID = " + _Version_ID
                                         + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() + " AND C_UOM_ID=" + GetC_UOM_ID());

                    DataSet ds = new DataSet();
                    try
                    {
                        ds = DB.ExecuteDataset(sql.ToString(), null, null);
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                SetPriceList(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]));
                                SetPriceLimit(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]));
                                SetPriceActual(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]));
                                SetPriceEntered(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]));
                            }

                            else
                            {
                                ds.Dispose();
                                sql.Clear();
                                sql.Append(@"SELECT PriceList , PriceStd , PriceLimit FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + GetM_Product_ID()
                                             + " AND M_PriceList_Version_ID = " + _Version_ID
                                             + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID=" + GetC_UOM_ID());
                                ds = DB.ExecuteDataset(sql.ToString(), null, null);
                                if (ds.Tables.Count > 0)
                                {
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        SetPriceList(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceList"]));
                                        SetPriceLimit(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceLimit"]));
                                        SetPriceActual(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]));
                                        SetPriceEntered(Util.GetValueOfDecimal(ds.Tables[0].Rows[0]["PriceStd"]));
                                    }

                                    else
                                    {
                                        PriceActual = Util.GetValueOfDecimal(GetPriceEntered());
                                        PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(GetCtx(), GetM_Product_ID(),
                                            GetC_UOM_ID(), PriceActual.Value);
                                        if (PriceEntered == null)
                                            PriceEntered = PriceActual;

                                        MProduct prod = new MProduct(Env.GetCtx(), Util.GetValueOfInt(GetM_Product_ID()), null);
                                        sql.Clear();
                                        sql.Append(@"SELECT PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.GetValueOfInt(GetM_Product_ID())
                                               + " AND M_PriceList_Version_ID = " + _Version_ID
                                               + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() + " AND C_UOM_ID=" + prod.GetC_UOM_ID());
                                        decimal pricelist = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                                        if (pricelist == 0)
                                        {
                                            sql.Clear();
                                            sql.Append(@"SELECT PriceList FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.GetValueOfInt(GetM_Product_ID())
                                               + " AND M_PriceList_Version_ID = " + _Version_ID
                                               + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID=" + prod.GetC_UOM_ID());
                                            pricelist = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                                        }
                                        sql.Clear();
                                        sql.Append(@"SELECT PriceStd FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.GetValueOfInt(GetM_Product_ID())
                                            + " AND M_PriceList_Version_ID = " + _Version_ID
                                            + " AND M_AttributeSetInstance_ID = " + GetM_AttributeSetInstance_ID() + " AND C_UOM_ID=" + prod.GetC_UOM_ID());
                                        decimal pricestd = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                                        if (pricestd == 0)
                                        {
                                            sql.Clear();
                                            sql.Append(@"SELECT PriceStd FROM M_ProductPrice WHERE Isactive='Y' AND M_Product_ID = " + Util.GetValueOfInt(GetM_Product_ID())
                                            + " AND M_PriceList_Version_ID = " + _Version_ID
                                            + " AND M_AttributeSetInstance_ID = 0 AND C_UOM_ID=" + prod.GetC_UOM_ID());
                                            pricestd = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                                        }
                                        sql.Clear();
                                        sql.Append(@"SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y' AND con.M_Product_ID = " + Util.GetValueOfInt(GetM_Product_ID()) +
                                               " AND con.C_UOM_ID = " + prod.GetC_UOM_ID() + " AND con.C_UOM_To_ID = " + GetC_UOM_ID());
                                        decimal rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                                        if (rate == 0)
                                        {
                                            sql.Clear();
                                            sql.Append(@"SELECT con.DivideRate FROM C_UOM_Conversion con INNER JOIN C_UOM uom ON con.C_UOM_ID = uom.C_UOM_ID WHERE con.IsActive = 'Y'" +
                                              " AND con.C_UOM_ID = " + prod.GetC_UOM_ID() + " AND con.C_UOM_To_ID = " + GetC_UOM_ID());

                                            rate = Util.GetValueOfDecimal(DB.ExecuteScalar(sql.ToString(), null, null));
                                        }
                                        if (rate > 0)
                                        {
                                            SetPriceList(Decimal.Multiply(pricelist, rate));
                                            SetPriceActual(Decimal.Multiply(pricestd, rate));
                                            SetPriceEntered(Decimal.Multiply(pricestd, rate));
                                        }
                                        else
                                        {
                                            SetPriceList(pricelist);
                                            SetPriceActual(pricestd);
                                            SetPriceEntered(pricestd);
                                        }
                                    }
                                }
                            }
                        }
                        ds.Dispose();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        ds.Dispose();
                    }
                }
                else
                {
                    if (_productPrice == null && Env.ZERO.CompareTo(GetPriceActual()) == 0
                        && Env.ZERO.CompareTo(GetPriceList()) == 0)
                        SetPrice();
                    //	Check if on Price list
                    if (_productPrice == null)
                        GetProductPricing(_M_PriceList_ID);
                }
                /******** Commented for ViennaCRM. Now it will not be checked whether the Product is on PriceList or Not ********/
                //if (!_productPrice.IsCalculated())
                //{
                //    log.SaveError("Error", Msg.GetMsg(GetCtx(), "ProductNotOnPriceList"));
                //    return false;
                //}
            }
            //	UOM
            if (GetC_UOM_ID() == 0
                && (GetM_Product_ID() != 0 || GetPriceEntered().CompareTo(Env.ZERO) != 0
                    || GetC_Charge_ID() != 0))
            {
                int C_UOM_ID = MUOM.GetDefault_UOM_ID(GetCtx());
                if (C_UOM_ID > 0)
                    SetC_UOM_ID(C_UOM_ID);
            }
            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetQtyEntered(GetQtyEntered());
            if (newRecord || Is_ValueChanged("QtyOrdered"))
                SetQtyOrdered(GetQtyOrdered());



            //	FreightAmt Not used
            if (Env.ZERO.CompareTo(GetFreightAmt()) != 0)
                SetFreightAmt(Env.ZERO);

            //	Set Tax
            if (GetC_Tax_ID() == 0)
                SetTax();

            //	Get Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM C_OrderLine WHERE C_Order_ID=" + GetC_Order_ID();
                int ii = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                SetLine(ii);
            }

            //	Calculations & Rounding
            SetLineNetAmt();	//	extended Amount with or without tax
            SetDiscount();

            // Validate Return Policy for RMA
            MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
            bool isReturnTrx = order.IsReturnTrx();
            if (isReturnTrx)
            {
                Boolean withinPolicy = true;

                if (order.GetM_ReturnPolicy_ID() == 0)
                    order.SetM_ReturnPolicy_ID();

                if (order.GetM_ReturnPolicy_ID() == 0)
                    withinPolicy = false;
                else
                {
                    ////////////////////////////////////////////////////////////
                    MInOut origInOut = new MInOut(GetCtx(), order.GetOrig_InOut_ID(), Get_TrxName());
                    MReturnPolicy rpolicy = new MReturnPolicy(GetCtx(), order.GetM_ReturnPolicy_ID(), Get_TrxName());

                    log.Fine("RMA Date : " + order.GetDateOrdered() + " Shipment Date : " + origInOut.GetMovementDate());
                    withinPolicy = rpolicy.CheckReturnPolicy(origInOut.GetMovementDate(), order.GetDateOrdered(), GetM_Product_ID());
                }

                if (!withinPolicy)
                {
                    if (!MRole.GetDefault(GetCtx()).IsOverrideReturnPolicy())
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "ReturnPolicyExceeded"));
                        return false;
                    }
                    else
                    {
                        log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "ReturnPolicyExceeded"));
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if it can be deleted</returns>
        public bool BeforeDelete()
        {
            //	R/O Check - Something delivered. etc.
            if (Env.ZERO.CompareTo(GetQtyDelivered()) != 0)
            {
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyDelivered") + "=" + GetQtyDelivered());
                return false;
            }
            if (Env.ZERO.CompareTo(GetQtyInvoiced()) != 0)
            {
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyInvoiced") + "=" + GetQtyDelivered());
                return false;
            }
            if (Env.ZERO.CompareTo(GetQtyReserved()) != 0)
            {
                //	For PO should be On Order
                log.SaveError("DeleteError", Msg.Translate(GetCtx(), "QtyReserved") + "=" + GetQtyReserved());
                return false;
            }
            return true;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new records</param>
        /// <param name="success">success</param>
        /// <returns>saved</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (!success)
                return success;
            if (!IsProcessed())
            {
                if (!newRecord && Is_ValueChanged("C_Tax_ID"))
                {
                    //	Recalculate Tax for old Tax
                    MOrderTax tax = MOrderTax.Get(this, GetPrecision(), true, Get_TrxName());	//	old Tax
                    if (tax != null)
                    {
                        if (!tax.CalculateTaxFromLines())
                            return false;
                        if (!tax.Save(Get_TrxName()))
                            return false;
                    }
                }
                if (!UpdateHeaderTax())
                    return false;
            }

            // nnayak : Changes for bug 1535824 - Order: Fully Invoiced
            if (!newRecord && Is_ValueChanged("QtyInvoiced"))
            {
                MOrder order = new MOrder(GetCtx(), GetC_Order_ID(), Get_TrxName());
                MOrderLine[] oLines = order.GetLines(true, null);
                bool isInvoiced = true;
                for (int i = 0; i < oLines.Length; i++)
                {
                    MOrderLine line = oLines[i];
                    if (line.GetQtyInvoiced().CompareTo(line.GetQtyOrdered()) < 0)
                    {
                        isInvoiced = false;
                        break;
                    }
                }
                order.SetIsInvoiced(isInvoiced);

                if (!order.Save())
                    return false;
            }

            return true;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>deleted</returns>
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            if (GetS_ResourceAssignment_ID() != 0)
            {
                /////////////////////////////////////////////////////////////
                MResourceAssignment ra = new MResourceAssignment(GetCtx(), GetS_ResourceAssignment_ID(), Get_TrxName());
                ra.Delete(true);
            }

            return UpdateHeaderTax();
        }


        /// <summary>
        /// Update Tax & Header
        /// </summary>
        /// <returns>true if header updated</returns>
        private bool UpdateHeaderTax()
        {
            //	Recalculate Tax for this Tax
            MOrderTax tax = MOrderTax.Get(this, GetPrecision(), false, Get_TrxName());	//	current Tax
            if (!tax.CalculateTaxFromLines())
                return false;
            if (!tax.Save(Get_TrxName()))
                return false;

            //	Update Order Header
            String sql = "UPDATE C_Order i"
                + " SET TotalLines="
                    + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM C_OrderLine il WHERE i.C_Order_ID=il.C_Order_ID) "
                + "WHERE C_Order_ID=" + GetC_Order_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(1) #" + no);
            }

            if (IsTaxIncluded())
                sql = "UPDATE C_Order i "
                    + "SET GrandTotal=TotalLines "
                    + "WHERE C_Order_ID=" + GetC_Order_ID();
            else
                sql = "UPDATE C_Order i "
                    + "SET GrandTotal=TotalLines+"
                        + "(SELECT COALESCE(SUM(TaxAmt),0) FROM C_OrderTax it WHERE i.C_Order_ID=it.C_Order_ID) "
                        + "WHERE C_Order_ID=" + GetC_Order_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(2) #" + no);
            }
            _parent = null;
            return no == 1;
        }
    }
}
