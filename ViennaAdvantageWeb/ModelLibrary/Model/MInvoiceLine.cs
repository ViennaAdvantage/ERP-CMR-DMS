/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MMatchPO
 * Purpose        : Used for invoice window's 2nd tab with C_InvoiceLine table
 * Class Used     : X_C_InvoiceLine
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
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInvoiceLine : X_C_InvoiceLine
    {
        //	Static Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(MInvoiceLine).FullName);
        private int _M_PriceList_ID = 0;
        private DateTime? _DateInvoiced = null;
        private int _C_BPartner_ID = 0;
        private int _C_BPartner_Location_ID = 0;
        private Boolean _IsSOTrx = true;
        private Boolean _priceSet = false;
        private MProduct _product = null;

        /**	Cached Name of the line		*/
        private String _name = null;
        /** Cached Precision			*/
        private int? _precision = null;
        /** Product Pricing				*/
        private MProductPricing _productPricing = null;
        /** Parent						*/
        private MInvoice _parent = null;
        private Decimal _PriceList = Env.ZERO;
        private Decimal _PriceStd = Env.ZERO;
        private Decimal _PriceLimit = Env.ZERO;
        private int M_AttributeSetInstance_ID = 0;
        private int C_UOM_ID = 0;

        /**
        * Get Invoice Line referencing InOut Line
        *	@param sLine shipment line
        *	@return (first) invoice line
        */
        public static MInvoiceLine GetOfInOutLine(MInOutLine sLine)
        {
            if (sLine == null)
            {
                return null;
            }
            MInvoiceLine retValue = null;
            try
            {
                String sql = "SELECT * FROM C_InvoiceLine WHERE M_InOutLine_ID=" + sLine.GetM_InOutLine_ID();
                DataSet ds = new DataSet();
                try
                {
                    ds = DataBase.DB.ExecuteDataset(sql, null, sLine.Get_TrxName());
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = ds.Tables[0].Rows[i];
                        retValue = new MInvoiceLine(sLine.GetCtx(), dr, sLine.Get_TrxName());
                        if (dr.HasErrors)
                        {
                            _log.Warning("More than one C_InvoiceLine of " + sLine);
                        }
                    }
                    ds = null;
                }
                catch (Exception e)
                {
                    _log.Log(Level.SEVERE, sql, e);
                }
                finally
                {
                    ds = null;
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--GetOfInOutLine");
            }
            return retValue;
        }

        /***
        * 	Invoice Line Constructor
        * 	@param ctx context
        * 	@param C_InvoiceLine_ID invoice line or 0
        * 	@param trxName transaction name
        */
        public MInvoiceLine(Ctx ctx, int C_InvoiceLine_ID, Trx trxName) :
            base(ctx, C_InvoiceLine_ID, trxName)
        {
            try
            {
                if (C_InvoiceLine_ID == 0)
                {
                    SetIsDescription(false);
                    SetIsPrinted(true);
                    SetLineNetAmt(Env.ZERO);
                    SetPriceEntered(Env.ZERO);
                    SetPriceActual(Env.ZERO);
                    SetPriceLimit(Env.ZERO);
                    SetPriceList(Env.ZERO);
                    SetM_AttributeSetInstance_ID(0);
                    SetTaxAmt(Env.ZERO);
                    //
                    SetQtyEntered(Env.ZERO);
                    SetQtyInvoiced(Env.ZERO);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--Invoice Line Constructor");
            }
        }

        /**
         * 	Parent Constructor
         * 	@param invoice parent
         */
        public MInvoiceLine(MInvoice invoice)
            : this(invoice.GetCtx(), 0, invoice.Get_TrxName())
        {
            try
            {

                if (invoice.Get_ID() == 0)
                    throw new ArgumentException("Header not saved");
                SetClientOrg(invoice.GetAD_Client_ID(), invoice.GetAD_Org_ID());
                SetC_Invoice_ID(invoice.GetC_Invoice_ID());
                SetInvoice(invoice);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--Parent Constructor");
            }
        }


        /**
         *  Load Constructor
         *  @param ctx context
         *  @param rs result Set record
         *  @param trxName transaction
         */
        public MInvoiceLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Set Defaults from Order.
         * 	Called also from copy lines from invoice
         * 	Does not Set Parent !!
         * 	@param invoice invoice
         */
        public void SetInvoice(MInvoice invoice)
        {
            try
            {
                _parent = invoice;
                _M_PriceList_ID = invoice.GetM_PriceList_ID();
                _DateInvoiced = invoice.GetDateInvoiced();
                _C_BPartner_ID = invoice.GetC_BPartner_ID();
                _C_BPartner_Location_ID = invoice.GetC_BPartner_Location_ID();
                _IsSOTrx = invoice.IsSOTrx();
                _precision = invoice.GetPrecision();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetInvoice");
            }
        }

        /**
         * 	Get Parent
         *	@return parent
         */
        public MInvoice GetParent()
        {
            try
            {
                if (_parent == null)
                {
                    _parent = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--GetParent");
            }
            return _parent;
        }

        /**
         * 	Set Client Org
         *	@param AD_Client_ID client
         *	@param AD_Org_ID org
         */
        public void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }

        /**
         * 	Set values from Order Line.
         * 	Does not Set quantity!
         *	@param oLine line
         */
        public void SetOrderLine(MOrderLine oLine)
        {
            try
            {
                SetC_OrderLine_ID(oLine.GetC_OrderLine_ID());
                //
                SetLine(oLine.GetLine());
                SetIsDescription(oLine.IsDescription());
                SetDescription(oLine.GetDescription());
                //
                SetC_Charge_ID(oLine.GetC_Charge_ID());
                //
                SetM_Product_ID(oLine.GetM_Product_ID());
                SetM_AttributeSetInstance_ID(oLine.GetM_AttributeSetInstance_ID());
                SetS_ResourceAssignment_ID(oLine.GetS_ResourceAssignment_ID());
                SetC_UOM_ID(oLine.GetC_UOM_ID());
                //
                SetPriceEntered(oLine.GetPriceEntered());
                SetPriceActual(oLine.GetPriceActual());
                SetPriceLimit(oLine.GetPriceLimit());
                SetPriceList(oLine.GetPriceList());
                //
                SetC_Tax_ID(oLine.GetC_Tax_ID());
                SetLineNetAmt(oLine.GetLineNetAmt());
                //
                SetC_Project_ID(oLine.GetC_Project_ID());
                SetC_ProjectPhase_ID(oLine.GetC_ProjectPhase_ID());
                SetC_ProjectTask_ID(oLine.GetC_ProjectTask_ID());
                SetC_Activity_ID(oLine.GetC_Activity_ID());
                SetC_Campaign_ID(oLine.GetC_Campaign_ID());
                SetAD_OrgTrx_ID(oLine.GetAD_OrgTrx_ID());
                SetUser1_ID(oLine.GetUser1_ID());
                SetUser2_ID(oLine.GetUser2_ID());
                //
                SetRRAmt(oLine.GetRRAmt());
                SetRRStartDate(oLine.GetRRStartDate());
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetOrderLine");
            }
        }

        /**
         * 	Set values from Shipment Line.
         * 	Does not Set quantity!
         *	@param sLine ship line
         */
        public void SetShipLine(MInOutLine sLine)
        {
            try
            {
                SetM_InOutLine_ID(sLine.GetM_InOutLine_ID());
                SetC_OrderLine_ID(sLine.GetC_OrderLine_ID());

                //
                SetLine(sLine.GetLine());
                SetIsDescription(sLine.IsDescription());
                SetDescription(sLine.GetDescription());
                //
                SetM_Product_ID(sLine.GetM_Product_ID());
                SetC_UOM_ID(sLine.GetC_UOM_ID());
                SetM_AttributeSetInstance_ID(sLine.GetM_AttributeSetInstance_ID());
                //	SetS_ResourceAssignment_ID(sLine.GetS_ResourceAssignment_ID());
                SetC_Charge_ID(sLine.GetC_Charge_ID());
                int C_OrderLine_ID = sLine.GetC_OrderLine_ID();
                if (C_OrderLine_ID != 0)
                {
                    MOrderLine oLine = new MOrderLine(GetCtx(), C_OrderLine_ID, Get_TrxName());
                    MOrder ord = new MOrder(GetCtx(), oLine.GetC_Order_ID(), Get_TrxName());          //Added By Bharat
                    SetS_ResourceAssignment_ID(oLine.GetS_ResourceAssignment_ID());
                    M_AttributeSetInstance_ID = sLine.GetM_AttributeSetInstance_ID();               //Added By Bharat
                    C_UOM_ID = oLine.GetC_UOM_ID();  
                    string docsubTypeSO  = Util.GetValueOfString(DB.ExecuteScalar("SELECT DocSubTypeSO FROM C_Doctype WHERE C_DocType_ID = " + ord.GetC_DocTypeTarget_ID()));
                    if (docsubTypeSO == "WR")
                    {
                        SetPriceEntered(oLine.GetPriceEntered());
                        SetPriceActual(oLine.GetPriceActual());
                        SetPriceLimit(oLine.GetPriceLimit());
                        SetPriceList(oLine.GetPriceList());
                    }
                    else
                    {
                        // Added By Bharat
                        // Changes Done For VAPRC Module To Set Price By Attribute Set Instance
                        Tuple<String, String, String> mInfo = null;
                        if (Env.HasModulePrefix("VAPRC_", out mInfo) && ord.IsSOTrx() && !ord.IsReturnTrx())
                        {
                            string qry = "SELECT max(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE M_PriceList_ID=" + _M_PriceList_ID;
                            int M_PriceList_Version_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                            Tuple<String, String, String> mInfo1 = null;
                            if (Env.HasModulePrefix("ED011_", out mInfo1))
                            {
                                SetPriceForUOM(sLine.GetM_Product_ID(), M_PriceList_Version_ID, sLine.GetM_AttributeSetInstance_ID(), C_UOM_ID);
                            }
                            else
                            {
                                SetPriceForAttribute(sLine.GetM_Product_ID(), M_PriceList_Version_ID, sLine.GetM_AttributeSetInstance_ID());
                            }
                            SetPriceEntered(_PriceStd);
                            SetPriceActual(_PriceStd);
                            SetPriceLimit(_PriceLimit);
                            SetPriceList(_PriceList);
                        }
                        else
                        {
                            SetPriceEntered(oLine.GetPriceEntered());
                            SetPriceActual(oLine.GetPriceActual());
                            SetPriceLimit(oLine.GetPriceLimit());
                            SetPriceList(oLine.GetPriceList());
                        }
                    }
                    //
                    SetC_Tax_ID(oLine.GetC_Tax_ID());
                    SetLineNetAmt(oLine.GetLineNetAmt());
                    SetC_Project_ID(oLine.GetC_Project_ID());
                }
                else
                {
                    SetPrice();
                    SetTax();
                }
                //
                SetC_Project_ID(sLine.GetC_Project_ID());
                SetC_ProjectPhase_ID(sLine.GetC_ProjectPhase_ID());
                SetC_ProjectTask_ID(sLine.GetC_ProjectTask_ID());
                SetC_Activity_ID(sLine.GetC_Activity_ID());
                SetC_Campaign_ID(sLine.GetC_Campaign_ID());
                SetAD_OrgTrx_ID(sLine.GetAD_OrgTrx_ID());
                SetUser1_ID(sLine.GetUser1_ID());
                SetUser2_ID(sLine.GetUser2_ID());
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetShipLine");
            }
        }

        private void SetPriceForAttribute(int _M_Product_ID, int _M_PriceList_Version_ID, int _M_AttributeSetInstance_ID)
        {
            string sql = "SELECT bomPriceStdAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceStd,"	//	1
                    + " bomPriceListAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceList,"		//	2
                    + " bomPriceLimitAttr(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID) AS PriceLimit"	//	3
                    + " FROM M_Product p"
                    + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                    + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                    + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                    + "WHERE pv.IsActive='Y'"
                    + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                    + " AND pv.M_PriceList_Version_ID=" + _M_PriceList_Version_ID	//	#2
                    + " AND pp.M_AttributeSetInstance_ID =" + _M_AttributeSetInstance_ID;	                //	#3
            DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                //	Prices
                _PriceStd = Util.GetValueOfDecimal(dr[0]);//.getBigDecimal(1);
                if (dr[0] == null)
                    _PriceStd = Env.ZERO;
                _PriceList = Util.GetValueOfDecimal(dr[1]);//.getBigDecimal(2);
                if (dr[1] == null)
                    _PriceList = Env.ZERO;
                _PriceLimit = Util.GetValueOfDecimal(dr[2]);//.getBigDecimal(3);
                if (dr[2] == null)
                    _PriceLimit = Env.ZERO;
            }
        }

        private void SetPriceForUOM(int _M_Product_ID, int _M_PriceList_Version_ID, int _M_AttributeSetInstance_ID, int UOM)
        {
            string sql = "SELECT bomPriceStdUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceStd,"	//	1
                          + " bomPriceListUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceList,"		//	2
                          + " bomPriceLimitUOM(p.M_Product_ID,pv.M_PriceList_Version_ID,pp.M_AttributeSetInstance_ID , pp.C_UOM_ID) AS PriceLimit,"	//	3
                          + " p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID,p.M_Product_Category_ID,"	//	4..7
                          + " pl.EnforcePriceLimit, pl.IsTaxIncluded "	// 8..9
                          + "FROM M_Product p"
                          + " INNER JOIN M_ProductPrice pp ON (p.M_Product_ID=pp.M_Product_ID)"
                          + " INNER JOIN  M_PriceList_Version pv ON (pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID)"
                          + " INNER JOIN M_PriceList pl ON (pv.M_PriceList_ID=pl.M_PriceList_ID) "
                          + "WHERE pv.IsActive='Y'"
                          + " AND p.M_Product_ID=" + _M_Product_ID				//	#1
                          + " AND pv.M_PriceList_Version_ID=" + _M_PriceList_Version_ID	//	#2
                          + " AND pp.M_AttributeSetInstance_ID = " + _M_AttributeSetInstance_ID  //	#3
                          + " AND pp.C_UOM_ID = " + C_UOM_ID  //    #4
                          + " AND pp.IsActive='Y'";
            DataSet ds = ExecuteQuery.ExecuteDataset(sql, null);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow dr = ds.Tables[0].Rows[i];
                //	Prices
                _PriceStd = Util.GetValueOfDecimal(dr[0]);//.getBigDecimal(1);
                if (dr[0] == null)
                    _PriceStd = Env.ZERO;
                _PriceList = Util.GetValueOfDecimal(dr[1]);//.getBigDecimal(2);
                if (dr[1] == null)
                    _PriceList = Env.ZERO;
                _PriceLimit = Util.GetValueOfDecimal(dr[2]);//.getBigDecimal(3);
                if (dr[2] == null)
                    _PriceLimit = Env.ZERO;
            }
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            try
            {
                String desc = GetDescription();
                if (desc == null)
                {
                    SetDescription(description);
                }
                else
                {
                    SetDescription(desc + " | " + description);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--addDescription");
            }
        }

        /**
         * 	Set M_AttributeSetInstance_ID
         *	@param M_AttributeSetInstance_ID id
         */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            try
            {
                if (M_AttributeSetInstance_ID == 0)		//	 0 is valid ID
                    Set_Value("M_AttributeSetInstance_ID", 0);
                else
                    base.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetM_AttributeSetInstance_ID");
            }
        }

        /***
         * 	Set Price for Product and PriceList.
         * 	Uses standard SO price list of not Set by invoice constructor
         */
        public void SetPrice()
        {
            try
            {
                if (GetM_Product_ID() == 0 || IsDescription())
                    return;
                if (_M_PriceList_ID == 0 || _C_BPartner_ID == 0)
                    SetInvoice(GetParent());
                if (_M_PriceList_ID == 0 || _C_BPartner_ID == 0)
                    throw new Exception("setPrice - PriceList unknown!");
                //throw new IllegalStateException("setPrice - PriceList unknown!");
                SetPrice(_M_PriceList_ID, _C_BPartner_ID);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetPrice");
            }
        }

        /**
         * 	Set Price for Product and PriceList
         * 	@param M_PriceList_ID price list
         * 	@param C_BPartner_ID business partner
         */
        public void SetPrice(int M_PriceList_ID, int C_BPartner_ID)
        {
            try
            {
                if (GetM_Product_ID() == 0 || IsDescription())
                    return;
                //
                log.Fine("M_PriceList_ID=" + M_PriceList_ID);
                _productPricing = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    GetM_Product_ID(), C_BPartner_ID, GetQtyInvoiced(), _IsSOTrx);
                _productPricing.SetM_PriceList_ID(M_PriceList_ID);
                _productPricing.SetPriceDate(_DateInvoiced);
                _productPricing.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
                //Amit 25-nov-2014
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='ED011_'")) > 0)
                {
                    _productPricing.SetC_UOM_ID(GetC_UOM_ID());
                }
                ////Amit
                SetPriceActual(_productPricing.GetPriceStd());
                SetPriceList(_productPricing.GetPriceList());
                SetPriceLimit(_productPricing.GetPriceLimit());
                //
                if (Decimal.Compare(GetQtyEntered(), GetQtyInvoiced()) == 0)
                    SetPriceEntered(GetPriceActual());
                else
                    SetPriceEntered(Decimal.Multiply(GetPriceActual(), Decimal.Round(Decimal.Divide(GetQtyInvoiced(), GetQtyEntered()), 6)));

                //
                if (GetC_UOM_ID() == 0)
                    SetC_UOM_ID(_productPricing.GetC_UOM_ID());
                //
                _priceSet = true;
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetPrice");
            }
        }

        /**
         * 	Set Price Entered/Actual.
         * 	Use this Method if the Line UOM is the Product UOM 
         *	@param PriceActual price
         */
        public void SetPrice(Decimal priceActual)
        {
            try
            {
                SetPriceEntered(priceActual);
                SetPriceActual(priceActual);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetPrice");
            }
        }

        /**
         * 	Set Price Actual.
         * 	(actual price is not updateable)
         *	@param PriceActual actual price
         */
        public void SetPriceActual(Decimal? priceActual)
        {
            try
            {
                if (priceActual == null)
                    throw new ArgumentException("PriceActual is mandatory");
                Set_ValueNoCheck("PriceActual", priceActual);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetPriceActual");
            }
        }


        /**
         *	Set Tax - requires Warehouse
         *	@return true if found
         */
        public Boolean SetTax()
        {
            try
            {

                if (IsDescription())
                    return true;


                // Change to Set Tax ID based on the VAT Engine Module
                if (_IsSOTrx)
                {
                    MInvoice inv = new MInvoice(Env.GetCtx(), Util.GetValueOfInt(Get_Value("C_Invoice_ID")), null);
                    // Table ID Fixed for OrgInfo Table
                    string taxrule = string.Empty;
                    int _CountVATAX = (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX = 'VATAX_'")));
                                
                    string sql = "SELECT VATAX_TaxRule FROM AD_OrgInfo WHERE AD_Org_ID=" + inv.GetAD_Org_ID() + " AND IsActive ='Y' AND AD_Client_ID =" + Env.GetCtx().GetAD_Client_ID();
                    if (_CountVATAX > 0)
                    {
                        taxrule = Util.GetValueOfString(DB.ExecuteScalar(sql, null, null));
                    }
                    // if (taxrule == "T" && _IsSOTrx)
                    if (taxrule == "T")
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
                    else
                    {
                        MOrg org = MOrg.Get(GetCtx(), GetAD_Org_ID());
                        int M_Warehouse_ID = org.GetM_Warehouse_ID();
                        //
                        int C_Tax_ID = Tax.Get(GetCtx(), GetM_Product_ID(), GetC_Charge_ID(),
                            _DateInvoiced, _DateInvoiced,
                            GetAD_Org_ID(), M_Warehouse_ID,
                            _C_BPartner_Location_ID,		//	should be bill to
                            _C_BPartner_Location_ID, _IsSOTrx);
                        if (C_Tax_ID == 0)
                        {
                            log.Log(Level.SEVERE, "No Tax found");
                            return false;
                        }
                        SetC_Tax_ID(C_Tax_ID);
                        if (_IsSOTrx)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetTax");
            }
            return true;
        }


        /**
         * 	Calculare Tax Amt.
         * 	Assumes Line Net is calculated
         */
        public void SetTaxAmt()
        {
            try
            {
                Decimal TaxAmt = Env.ZERO;
                if (GetC_Tax_ID() == 0)
                    return;
                //	SetLineNetAmt();
                MTax tax = MTax.Get(GetCtx(), GetC_Tax_ID());
                if (tax.IsDocumentLevel() && _IsSOTrx)		//	AR Inv Tax
                    return;
                //
                TaxAmt = tax.CalculateTax(GetLineNetAmt(), IsTaxIncluded(), GetPrecision());
                if (IsTaxIncluded())
                    SetLineTotalAmt(GetLineNetAmt());
                else
                    SetLineTotalAmt(Decimal.Add(GetLineNetAmt(), TaxAmt));
                base.SetTaxAmt(TaxAmt);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--SetTaxAmt");
            }
        }

        /**
         * 	Calculate Extended Amt.
         * 	May or may not include tax
         */
        public void SetLineNetAmt()
        {
            try
            {
                //	Calculations & Rounding
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
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetLineNetAmt");
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

        /**
         * 	Set Qty Invoiced/Entered.
         *	@param Qty Invoiced/Ordered
         */
        public void SetQty(int Qty)
        {
            try
            {
                SetQty(new Decimal(Qty));
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetQty");
            }
        }

        /**
         * 	Set Qty Invoiced
         *	@param Qty Invoiced/Entered
         */
        public void SetQty(Decimal Qty)
        {
            SetQtyEntered(Qty);
            SetQtyInvoiced(GetQtyEntered());
        }

        /**
         * 	Set Qty Entered - enforce entered UOM 
         *	@param QtyEntered
         */
        public void SetQtyEntered(Decimal? QtyEntered)
        {
            try
            {
                if (QtyEntered != null && GetC_UOM_ID() != 0)
                {
                    int precision = MUOM.GetPrecision(GetCtx(), GetC_UOM_ID());
                    QtyEntered = Decimal.Round((Decimal)QtyEntered, precision, MidpointRounding.AwayFromZero);
                }
                base.SetQtyEntered(Convert.ToDecimal(QtyEntered));
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetQtyEntered");
            }
        }

        /**
         * 	Set Qty Invoiced - enforce Product UOM 
         *	@param QtyInvoiced
         */
        public void SetQtyInvoiced(Decimal? QtyInvoiced)
        {
            try
            {
                MProduct product = GetProduct();
                if (QtyInvoiced != null && product != null)
                {
                    int precision = product.GetUOMPrecision();
                    QtyInvoiced = Decimal.Round((Decimal)QtyInvoiced, precision, MidpointRounding.AwayFromZero);
                }
                base.SetQtyInvoiced(Convert.ToDecimal(QtyInvoiced));
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetQtyInvoiced");
            }
        }

        /**
         * 	Set Product
         *	@param product product
         */
        public void SetProduct(MProduct product)
        {
            try
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
                    SetC_UOM_ID(0);
                }
                SetM_AttributeSetInstance_ID(0);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetProduct");
            }
        }


        /**
         * 	Set M_Product_ID
         *	@param M_Product_ID product
         *	@param SetUOM Set UOM from product
         */
        public void SetM_Product_ID(int M_Product_ID, Boolean SetUOM)
        {
            try
            {
                if (SetUOM)
                    SetProduct(MProduct.Get(GetCtx(), M_Product_ID));
                else
                    base.SetM_Product_ID(M_Product_ID);
                SetM_AttributeSetInstance_ID(0);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetM_Product_ID");
            }
        }

        /**
         * 	Set Product and UOM
         *	@param M_Product_ID product
         *	@param C_UOM_ID uom
         */
        public void SetM_Product_ID(int M_Product_ID, int C_UOM_ID)
        {
            try
            {
                base.SetM_Product_ID(M_Product_ID);
                base.SetC_UOM_ID(C_UOM_ID);
                SetM_AttributeSetInstance_ID(0);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetM_Product_ID");
            }
        }

        /**
         * 	Get Product
         *	@return product or null
         */
        public MProduct GetProduct()
        {
            try
            {
                if (_product == null && GetM_Product_ID() != 0)
                {
                    _product = MProduct.Get(GetCtx(), GetM_Product_ID());
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--GetProduct");
            }
            return _product;
        }

        /**
         * 	Get C_Project_ID
         *	@return project
         */
        public int GetC_Project_ID()
        {
            int ii = base.GetC_Project_ID();
            try
            {
                if (ii == 0)
                {
                    ii = GetParent().GetC_Project_ID();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--GetC_Project_ID");
            }
            return ii;
        }

        /**
         * 	Get C_Activity_ID
         *	@return Activity
         */
        public int GetC_Activity_ID()
        {
            int ii = base.GetC_Activity_ID();
            try
            {
                if (ii == 0)
                {
                    ii = GetParent().GetC_Activity_ID();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--GetC_Activity_ID");
            }
            return ii;
        }

        /**
         * 	Get C_Campaign_ID
         *	@return Campaign
         */
        public int GetC_Campaign_ID()
        {
            int ii = base.GetC_Campaign_ID();
            if (ii == 0)
                ii = GetParent().GetC_Campaign_ID();
            return ii;
        }	//	GetC_Campaign_ID

        /**
         * 	Get User2_ID
         *	@return User2
         */
        public int GetUser1_ID()
        {
            int ii = base.GetUser1_ID();
            if (ii == 0)
                ii = GetParent().GetUser1_ID();
            return ii;
        }	//	GetUser1_ID

        /**
         * 	Get User2_ID
         *	@return User2
         */
        public int GetUser2_ID()
        {
            int ii = base.GetUser2_ID();
            if (ii == 0)
                ii = GetParent().GetUser2_ID();
            return ii;
        }	//	GetUser2_ID

        /**
         * 	Get AD_OrgTrx_ID
         *	@return trx org
         */
        public int GetAD_OrgTrx_ID()
        {
            int ii = base.GetAD_OrgTrx_ID();
            if (ii == 0)
                ii = GetParent().GetAD_OrgTrx_ID();
            return ii;
        }	//	GetAD_OrgTrx_ID

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInvoiceLine[")
                .Append(Get_ID()).Append(",").Append(GetLine())
                .Append(",QtyInvoiced=").Append(GetQtyInvoiced())
                .Append(",LineNetAmt=").Append(GetLineNetAmt())
                .Append("]");
            return sb.ToString();
        }	//	toString

        /**
         * 	Get (Product/Charge) Name
         * 	@return name
         */
        public String GetName()
        {
            if (_name == null)
            {
                String sql = "SELECT COALESCE (p.Name, c.Name) "
                    + "FROM C_InvoiceLine il"
                    + " LEFT OUTER JOIN M_Product p ON (il.M_Product_ID=p.M_Product_ID)"
                    + " LEFT OUTER JOIN C_Charge C ON (il.C_Charge_ID=c.C_Charge_ID) "
                    + "WHERE C_InvoiceLine_ID=" + GetC_InvoiceLine_ID();
                IDataReader idr = null;
                try
                {
                    idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                    if (idr.Read())
                    {
                        _name = idr[0].ToString();
                    }
                    idr.Close();
                    //pstmt.close();
                    //pstmt = null;
                    if (_name == null)
                        _name = "??";
                }
                catch (Exception e)
                {
                    if (idr != null)
                    {
                        idr.Close();
                    }
                    log.Log(Level.SEVERE, "GetName", e);
                }

            }
            return _name;
        }

        /**
         * 	Set Temporary (cached) Name
         * 	@param tempName Cached Name
         */
        public void SetName(String tempName)
        {
            _name = tempName;
        }

        /**
         * 	Get Description Text.
         * 	For jsp access (vs. isDescription)
         *	@return description
         */
        public String GetDescriptionText()
        {
            return base.GetDescription();
        }	//	GetDescriptionText

        /**
         * 	Get Currency Precision
         *	@return precision
         */
        public int GetPrecision()
        {
            try
            {
                if (_precision != null)
                {
                    return Convert.ToInt32(_precision);
                }

                String sql = "SELECT c.StdPrecision "
                    + "FROM C_Currency c INNER JOIN C_Invoice x ON (x.C_Currency_ID=c.C_Currency_ID) "
                    + "WHERE x.C_Invoice_ID=" + GetC_Invoice_ID();
                int i = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteScalar(sql, null, Get_TrxName()));
                if (i < 0)
                {
                    log.Warning("Precision=" + i + " - Set to 2");
                    i = 2;
                }
                _precision = i;
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--GetPrecision()");
            }
            return (int)_precision;
        }

        /**
         *	Is Tax Included in Amount
         *	@return true if tax is included
         */
        public Boolean IsTaxIncluded()
        {
            // try
            // {
            if (_M_PriceList_ID == 0)
            {
                _M_PriceList_ID = DataBase.DB.GetSQLValue(Get_TrxName(),
                    "SELECT M_PriceList_ID FROM C_Invoice WHERE C_Invoice_ID=@param1",
                    GetC_Invoice_ID());
            }
            MPriceList pl = MPriceList.Get(GetCtx(), _M_PriceList_ID, Get_TrxName());
            // }
            // catch (Exception ex)
            //{
            //    // MessageBox.Show("MinvoiceLine--isTaxIncluded");
            //}
            return pl.IsTaxIncluded();
        }

        /// <summary>
        /// Create Lead/Request
        /// </summary>
        /// <param name="invoice"></param>
        public void CreateLeadRequest(MInvoice invoice)
        {
            try
            {
                if (GetProduct() == null || _product.GetR_Source_ID() == 0)
                    return;
                String summary = "Purchased: " + _product.GetName()
                    + " - " + GetQtyEntered() + " * " + GetPriceEntered();
                //
                MSource source = MSource.Get(GetCtx(), _product.GetR_Source_ID());
                //	Create Request
                if (MSource.SOURCECREATETYPE_Both.Equals(source.GetSourceCreateType())
                    || MSource.SOURCECREATETYPE_Request.Equals(source.GetSourceCreateType()))
                {
                    MRequest request = new MRequest(GetCtx(), 0, Get_TrxName());
                    request.SetClientOrg(this);
                    request.SetSummary(summary);
                    request.SetAD_User_ID(invoice.GetAD_User_ID());
                    request.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
                    request.SetC_Invoice_ID(invoice.GetC_Invoice_ID());
                    request.SetC_Order_ID(invoice.GetC_Order_ID());
                    request.SetC_Activity_ID(invoice.GetC_Activity_ID());
                    request.SetC_Campaign_ID(invoice.GetC_Campaign_ID());
                    request.SetC_Project_ID(invoice.GetC_Project_ID());
                    //
                    request.SetM_Product_ID(GetM_Product_ID());
                    request.SetR_Source_ID(source.GetR_Source_ID());
                    request.Save();
                }
                //	Create Lead
                if (MSource.SOURCECREATETYPE_Both.Equals(source.GetSourceCreateType())
                    || MSource.SOURCECREATETYPE_Lead.Equals(source.GetSourceCreateType()))
                {
                    MLead lead = new MLead(GetCtx(), 0, Get_TrxName());
                    lead.SetClientOrg(this);
                    lead.SetDescription(summary);
                    lead.SetAD_User_ID(invoice.GetAD_User_ID());
                    lead.SetC_BPartner_Location_ID(invoice.GetC_BPartner_Location_ID());
                    lead.SetC_BPartner_ID(invoice.GetC_BPartner_ID());
                    lead.SetC_Campaign_ID(invoice.GetC_Campaign_ID());
                    lead.SetC_Project_ID(invoice.GetC_Project_ID());
                    //
                    MBPartnerLocation bpLoc = new MBPartnerLocation(GetCtx(), invoice.GetC_BPartner_Location_ID(), null);
                    MLocation loc = bpLoc.GetLocation(false);
                    lead.SetAddress1(loc.GetAddress1());
                    lead.SetAddress2(loc.GetAddress2());
                    lead.SetCity(loc.GetCity());
                    lead.SetPostal(loc.GetPostal());
                    lead.SetPostal_Add(loc.GetPostal_Add());
                    lead.SetRegionName(loc.GetRegionName(false));
                    lead.SetC_Region_ID(loc.GetC_Region_ID());
                    lead.SetC_City_ID(loc.GetC_City_ID());
                    lead.SetC_Country_ID(loc.GetC_Country_ID());
                    //
                    lead.SetR_Source_ID(source.GetR_Source_ID());
                    lead.Save();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--createLeadRequest");
            }
        }


        /**
         * 	Set Resource Assignment - Callout
         *	@param oldS_ResourceAssignment_ID old value
         *	@param newS_ResourceAssignment_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetS_ResourceAssignment_ID(String oldS_ResourceAssignment_ID,
            String newS_ResourceAssignment_ID, int windowNo)
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
                + "WHERE ra.S_ResourceAssignment_ID= " + S_ResourceAssignment_ID;
            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.SetInt(1, S_ResourceAssignment_ID);
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    M_Product_ID = Utility.Util.GetValueOfInt(idr[0]);
                    Name = idr.GetString(1);
                    Description = idr.GetString(2);
                    Qty = Utility.Util.GetValueOfDecimal(idr[3]);
                }
                idr.Close();


            }
            catch (SqlException e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
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
                    SetQtyInvoiced(Qty);
            }
        }


        /**************************************************************************
         * 	Before Save
         *	@param newRecord
         *	@return true if save
         */
        protected override bool BeforeSave(bool newRecord)
        {
            try
            {
                log.Fine("New=" + newRecord);
                //	Charge
                if (GetC_Charge_ID() != 0)
                {
                    if (GetM_Product_ID() != 0)
                        SetM_Product_ID(0);
                }

                MInvoice inv = new MInvoice(GetCtx(), GetC_Invoice_ID(), null);
                if (!inv.IsSOTrx())
                {
                    _IsSOTrx = inv.IsSOTrx();
                    //MProduct pro = new MProduct(GetCtx(), GetM_Product_ID(), null);
                    //String qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + GetM_Product_ID() + " AND vdr.C_BPartner_ID = " + inv.GetC_BPartner_ID();
                    //int uom = Util.GetValueOfInt(DB.ExecuteScalar(qryUom));
                    //if (pro.GetC_UOM_ID() != 0)
                    //{
                    //    if (pro.GetC_UOM_ID() != uom && uom != 0)
                    //    {
                    //        decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + GetM_Product_ID() + " AND IsActive='Y'"));
                    //        if (Res > 0)
                    //        {
                    //            SetQtyEntered(GetQtyEntered() * Res);
                    //            //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                    //        }
                    //        else
                    //        {
                    //            decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                    //            if (res > 0)
                    //            {
                    //                SetQtyEntered(GetQtyEntered() * res);
                    //                //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
                    //            }
                    //        }
                    //        SetC_UOM_ID(uom);
                    //    }
                    //    else
                    //    {
                    //        SetC_UOM_ID(pro.GetC_UOM_ID());
                    //    }
                    //}

                    // Set Converted Price 
                    StringBuilder sql = new StringBuilder();
                    Tuple<String, String, String> iInfo = null;
                    if (Env.HasModulePrefix("ED011_", out iInfo))
                    {
                        string qry = "SELECT M_PriceList_Version_ID FROM m_pricelist_version WHERE IsActive = 'Y' AND m_pricelist_id = " + inv.GetM_PriceList_ID() + @" AND VALIDFROM <= sysdate order by validfrom desc";
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
                                            decimal? PriceActual = Util.GetValueOfDecimal(GetPriceEntered());
                                            decimal? PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(GetCtx(), GetM_Product_ID(),
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
                    else	//	Set Product Price
                    {
                        if (!_priceSet
                            && Env.ZERO.CompareTo(GetPriceActual()) == 0
                            && Env.ZERO.CompareTo(GetPriceList()) == 0)
                            SetPrice();
                    }
                }
                else	//	Set Product Price
                {
                    if (!_priceSet
                        && Env.ZERO.CompareTo(GetPriceActual()) == 0
                        && Env.ZERO.CompareTo(GetPriceList()) == 0)
                        SetPrice();
                }

                //	Set Tax
                SetC_Tax_ID(GetCtx().GetContextAsInt("C_Tax_ID"));
                if (GetC_Tax_ID() == 0)
                    SetTax();

                //	Get Line No
                if (GetLine() == 0)
                {
                    String sql = "SELECT COALESCE(MAX(Line),0)+10 FROM C_InvoiceLine WHERE C_Invoice_ID=@param1";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql, GetC_Invoice_ID());
                    SetLine(ii);
                }
                //	UOM
                if (GetC_UOM_ID() == 0)
                {
                    int C_UOM_ID = MUOM.GetDefault_UOM_ID(GetCtx());
                    if (C_UOM_ID > 0)
                        SetC_UOM_ID(C_UOM_ID);
                }
                //	Qty Precision
                if (newRecord || Is_ValueChanged("QtyEntered"))
                    SetQtyEntered(GetQtyEntered());
                if (newRecord || Is_ValueChanged("QtyInvoiced"))
                    SetQtyInvoiced(GetQtyInvoiced());

                //	Calculations & Rounding
                SetLineNetAmt();
                if (((Decimal)GetTaxAmt()).CompareTo(Env.ZERO) == 0)
                    SetTaxAmt();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--BeforeSave");
            }
            return true;
        }


        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return saved
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            try
            {
                if (!success || IsProcessed())
                    return success;
                if (!newRecord && Is_ValueChanged("C_Tax_ID"))
                {
                    //	Recalculate Tax for old Tax
                    MInvoiceTax tax = MInvoiceTax.Get(this, GetPrecision(),
                        true, Get_TrxName());	//	old Tax
                    if (tax != null)
                    {
                        if (!tax.CalculateTaxFromLines())
                            return false;
                        if (!tax.Save(Get_TrxName()))
                            return true;
                    }
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MInvoiceLine--AfterSave");
            }
            return UpdateHeaderTax();
        }

        /**
         * 	After Delete
         *	@param success success
         *	@return deleted
         */
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            return UpdateHeaderTax();
        }

        /**
         *	Update Tax & Header
         *	@return true if header updated with tax
         */
        private bool UpdateHeaderTax()
        {
            try
            {
                //	Recalculate Tax for this Tax
                MInvoiceTax tax = MInvoiceTax.Get(this, GetPrecision(),
                    false, Get_TrxName());	//	current Tax
                if (tax != null)
                {
                    if (!tax.CalculateTaxFromLines())
                        return false;
                    if (!tax.Save(Get_TrxName()))
                        return false;
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--UpdateHeaderTax()");
            }

            //	Update Invoice Header
            String sql = "UPDATE C_Invoice i"
                + " SET TotalLines="
                    + "(SELECT COALESCE(SUM(LineNetAmt),0) FROM C_InvoiceLine il WHERE i.C_Invoice_ID=il.C_Invoice_ID) "
                + "WHERE C_Invoice_ID=" + GetC_Invoice_ID();
            int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(1) #" + no);
            }

            if (IsTaxIncluded())
                sql = "UPDATE C_Invoice i "
                    + "SET GrandTotal=TotalLines "
                    + "WHERE C_Invoice_ID=" + GetC_Invoice_ID();
            else
                sql = "UPDATE C_Invoice i "
                    + "SET GrandTotal=TotalLines+"
                        + "(SELECT COALESCE(SUM(TaxAmt),0) FROM C_InvoiceTax it WHERE i.C_Invoice_ID=it.C_Invoice_ID) "
                        + "WHERE C_Invoice_ID=" + GetC_Invoice_ID();
            no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            if (no != 1)
            {
                log.Warning("(2) #" + no);
            }
            _parent = null;

            return no == 1;
        }


        /***
         * 	Allocate Landed Costs
         *	@return error message or ""
         */
        public String AllocateLandedCosts()
        {
            try
            {
                if (IsProcessed())
                    return "Processed";
                MLandedCost[] lcs = MLandedCost.GetLandedCosts(this);
                if (lcs.Length == 0)
                    return "";
                String sql = "DELETE FROM C_LandedCostAllocation WHERE C_InvoiceLine_ID="
                    + GetC_InvoiceLine_ID();
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                if (no != 0)
                {
                    log.Info("Deleted #" + no);
                }
                int inserted = 0;
                //	*** Single Criteria ***
                if (lcs.Length == 1)
                {
                    MLandedCost lc = lcs[0];
                    if (lc.GetM_InOut_ID() != 0)
                    {
                        //	Create List
                        List<MInOutLine> list = new List<MInOutLine>();
                        MInOut ship = new MInOut(GetCtx(), lc.GetM_InOut_ID(), Get_TrxName());
                        MInOutLine[] lines = ship.GetLines();
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].IsDescription() || lines[i].GetM_Product_ID() == 0)
                                continue;
                            if (lc.GetM_Product_ID() == 0
                                || lc.GetM_Product_ID() == lines[i].GetM_Product_ID())
                                list.Add(lines[i]);
                        }
                        if (list.Count == 0)
                            return "No Matching Lines (with Product) in Shipment";
                        //	Calculate total & base
                        Decimal total = Env.ZERO;
                        for (int i = 0; i < list.Count; i++)
                        {
                            MInOutLine iol = (MInOutLine)list[i];
                            total = Decimal.Add(total, iol.GetBase(lc.GetLandedCostDistribution()));
                        }
                        if (Env.Signum(total) == 0)
                            return "Total of Base values is 0 - " + lc.GetLandedCostDistribution();
                        //	Create Allocations
                        for (int i = 0; i < list.Count; i++)
                        {
                            MInOutLine iol = (MInOutLine)list[i];
                            MLandedCostAllocation lca = new MLandedCostAllocation(this,
                                lc.GetM_CostElement_ID());
                            lca.SetM_Product_ID(iol.GetM_Product_ID());
                            lca.SetM_AttributeSetInstance_ID(iol.GetM_AttributeSetInstance_ID());
                            Decimal base1 = iol.GetBase(lc.GetLandedCostDistribution());
                            lca.SetBase(base1);
                            if (Env.Signum(base1) != 0)
                            {
                                double result = Decimal.ToDouble(Decimal.Multiply(GetLineNetAmt(), base1));
                                result = Decimal.ToDouble(total);
                                lca.SetAmt(result, GetPrecision());
                            }
                            lca.SetQty(iol.GetQtyEntered());
                            if (!lca.Save())
                                return "Cannot save line Allocation = " + lca;
                            inserted++;
                        }
                        log.Info("Inserted " + inserted);
                        AllocateLandedCostRounding();
                        return "";
                    }
                    //	Single Line
                    else if (lc.GetM_InOutLine_ID() != 0)
                    {
                        MInOutLine iol = new MInOutLine(GetCtx(), lc.GetM_InOutLine_ID(), Get_TrxName());
                        if (iol.IsDescription() || iol.GetM_Product_ID() == 0)
                            return "Invalid Receipt Line - " + iol;
                        MLandedCostAllocation lca = new MLandedCostAllocation(this, lc.GetM_CostElement_ID());
                        lca.SetM_Product_ID(iol.GetM_Product_ID());
                        lca.SetM_AttributeSetInstance_ID(iol.GetM_AttributeSetInstance_ID());
                        lca.SetAmt(GetLineNetAmt());
                        if (lca.Save())
                            return "";
                        return "Cannot save single line Allocation = " + lc;
                    }
                    //	Single Product
                    else if (lc.GetM_Product_ID() != 0)
                    {
                        MLandedCostAllocation lca = new MLandedCostAllocation(this, lc.GetM_CostElement_ID());
                        lca.SetM_Product_ID(lc.GetM_Product_ID());	//	No ASI
                        lca.SetAmt(GetLineNetAmt());
                        if (lca.Save())
                            return "";
                        return "Cannot save Product Allocation = " + lc;
                    }
                    else
                        return "No Reference for " + lc;
                }

                //	*** Multiple Criteria ***
                String LandedCostDistribution = lcs[0].GetLandedCostDistribution();
                int M_CostElement_ID = lcs[0].GetM_CostElement_ID();
                for (int i = 0; i < lcs.Length; i++)
                {
                    MLandedCost lc = lcs[i];
                    if (!LandedCostDistribution.Equals(lc.GetLandedCostDistribution()))
                        return "Multiple Landed Cost Rules must have consistent Landed Cost Distribution";
                    if (lc.GetM_Product_ID() != 0 && lc.GetM_InOut_ID() == 0 && lc.GetM_InOutLine_ID() == 0)
                        return "Multiple Landed Cost Rules cannot directly allocate to a Product";
                    if (M_CostElement_ID != lc.GetM_CostElement_ID())
                        return "Multiple Landed Cost Rules cannot different Cost Elements";
                }
                //	Create List
                List<MInOutLine> list1 = new List<MInOutLine>();
                for (int ii = 0; ii < lcs.Length; ii++)
                {
                    MLandedCost lc = lcs[ii];
                    if (lc.GetM_InOut_ID() != 0 && lc.GetM_InOutLine_ID() == 0)		//	entire receipt
                    {
                        MInOut ship = new MInOut(GetCtx(), lc.GetM_InOut_ID(), Get_TrxName());
                        MInOutLine[] lines = ship.GetLines();
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].IsDescription()		//	decription or no product 
                                || lines[i].GetM_Product_ID() == 0)
                                continue;
                            if (lc.GetM_Product_ID() == 0		//	no restriction or product match
                                || lc.GetM_Product_ID() == lines[i].GetM_Product_ID())
                                list1.Add(lines[i]);
                        }
                    }
                    else if (lc.GetM_InOutLine_ID() != 0)	//	receipt line
                    {
                        MInOutLine iol = new MInOutLine(GetCtx(), lc.GetM_InOutLine_ID(), Get_TrxName());
                        if (!iol.IsDescription() && iol.GetM_Product_ID() != 0)
                            list1.Add(iol);
                    }
                }
                if (list1.Count == 0)
                    return "No Matching Lines (with Product)";
                //	Calculate total & base
                Decimal total1 = Env.ZERO;
                for (int i = 0; i < list1.Count; i++)
                {
                    MInOutLine iol = (MInOutLine)list1[i];
                    total1 = Decimal.Add(total1, iol.GetBase(LandedCostDistribution));
                }
                if (Env.Signum(total1) == 0)
                    return "Total of Base values is 0 - " + LandedCostDistribution;
                //	Create Allocations
                for (int i = 0; i < list1.Count; i++)
                {
                    MInOutLine iol = (MInOutLine)list1[i];
                    MLandedCostAllocation lca = new MLandedCostAllocation(this, lcs[0].GetM_CostElement_ID());
                    lca.SetM_Product_ID(iol.GetM_Product_ID());
                    lca.SetM_AttributeSetInstance_ID(iol.GetM_AttributeSetInstance_ID());
                    Decimal base1 = iol.GetBase(LandedCostDistribution);
                    lca.SetBase(base1);
                    if (Env.Signum(base1) != 0)
                    {
                        double result = Decimal.ToDouble(Decimal.Multiply(GetLineNetAmt(), base1));
                        result /= Decimal.ToDouble(total1);
                        lca.SetAmt(result, GetPrecision());
                    }
                    if (!lca.Save())
                        return "Cannot save line Allocation = " + lca;
                    inserted++;
                }

                log.Info("Inserted " + inserted);
                AllocateLandedCostRounding();
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--AllocateLandedCosts");
            }
            return "";
        }

        /**
         * 	Allocate Landed Cost - Enforce Rounding
         */
        private void AllocateLandedCostRounding()
        {
            try
            {
                MLandedCostAllocation[] allocations = MLandedCostAllocation.GetOfInvoiceLine(
                    GetCtx(), GetC_InvoiceLine_ID(), Get_TrxName());
                MLandedCostAllocation largestAmtAllocation = null;
                Decimal allocationAmt = Env.ZERO;
                for (int i = 0; i < allocations.Length; i++)
                {
                    MLandedCostAllocation allocation = allocations[i];
                    if (largestAmtAllocation == null
                        || allocation.GetAmt().CompareTo(largestAmtAllocation.GetAmt()) > 0)
                        largestAmtAllocation = allocation;
                    allocationAmt = Decimal.Add(allocationAmt, allocation.GetAmt());
                }
                Decimal difference = Decimal.Subtract(GetLineNetAmt(), allocationAmt);
                if (Env.Signum(difference) != 0)
                {
                    largestAmtAllocation.SetAmt(Decimal.Add(largestAmtAllocation.GetAmt(), difference));
                    largestAmtAllocation.Save();
                    log.Config("Difference=" + difference
                        + ", C_LandedCostAllocation_ID=" + largestAmtAllocation.GetC_LandedCostAllocation_ID()
                        + ", Amt" + largestAmtAllocation.GetAmt());
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--AllocateLandedCostRounding");
            }
        }

        /**
         *	Invoice Line - Quantity.
         *		- called from C_UOM_ID, QtyEntered, QtyInvoiced
         *		- enforces qty UOM relationship
         *	@param ctx context
         *	@param WindowNo window no
         *	@param mTab tab
         *	@param mField field
         *	@param value value
         *	@return null or error message
         */
        private bool SetQty(int WindowNo, String columnName)
        {
            try
            {
                int M_Product_ID = GetM_Product_ID();
                //	log.log(Level.WARNING,"qty - init - M_Product_ID=" + M_Product_ID);
                Decimal QtyInvoiced;
                Decimal QtyEntered, PriceActual, PriceEntered;

                //	No Product
                if (M_Product_ID == 0)
                {
                    QtyEntered = GetQtyEntered();
                    SetQtyInvoiced(QtyEntered);
                }
                //	UOM Changed - convert from Entered -> Product
                else if (columnName.Equals("C_UOM_ID"))
                {
                    int C_UOM_To_ID = GetC_UOM_ID();
                    QtyEntered = GetQtyEntered();
                    Decimal QtyEntered1 = Decimal.Round((Decimal)QtyEntered,
                        MUOM.GetPrecision(GetCtx(), C_UOM_To_ID)
                        , MidpointRounding.AwayFromZero);
                    if (QtyEntered.CompareTo(QtyEntered1) != 0)
                    {
                        log.Fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                            + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                        QtyEntered = QtyEntered1;
                        SetQtyEntered(QtyEntered);
                    }
                    QtyInvoiced = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, QtyEntered);
                    if (QtyInvoiced == null)
                    {
                        QtyInvoiced = QtyEntered;
                    }
                    bool conversion = QtyEntered.CompareTo(QtyInvoiced) != 0;
                    PriceActual = GetPriceActual();
                    PriceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, PriceActual);
                    if (PriceEntered == null)
                    {
                        PriceEntered = PriceActual;
                    }
                    log.Fine("qty - UOM=" + C_UOM_To_ID
                        + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                        + " -> " + conversion
                        + " QtyInvoiced/PriceEntered=" + QtyInvoiced + "/" + PriceEntered);
                    SetContext(WindowNo, "UOMConversion", conversion ? "Y" : "N");
                    SetQtyInvoiced(QtyInvoiced);
                    SetPriceEntered(PriceEntered);
                }
                //	QtyEntered changed - calculate QtyInvoiced
                else if (columnName.Equals("QtyEntered"))
                {
                    int C_UOM_To_ID = GetC_UOM_ID();
                    QtyEntered = GetQtyEntered();
                    QtyEntered = Decimal.Round(QtyEntered, MUOM.GetPrecision(GetCtx(), C_UOM_To_ID), MidpointRounding.AwayFromZero);
                    Decimal QtyEntered1 = QtyEntered;
                    if (QtyEntered.CompareTo(QtyEntered1) != 0)
                    {
                        log.Fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                            + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                        QtyEntered = QtyEntered1;
                        SetQtyEntered(QtyEntered);
                    }
                    QtyInvoiced = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, QtyEntered);
                    if (QtyInvoiced == null)
                        QtyInvoiced = QtyEntered;
                    bool conversion = QtyEntered.CompareTo(QtyInvoiced) != 0;
                    log.Fine("qty - UOM=" + C_UOM_To_ID
                        + ", QtyEntered=" + QtyEntered
                        + " -> " + conversion
                        + " QtyInvoiced=" + QtyInvoiced);
                    SetContext(WindowNo, "UOMConversion", conversion ? "Y" : "N");
                    SetQtyInvoiced(QtyInvoiced);
                }
                //	QtyInvoiced changed - calculate QtyEntered (should not happen)
                else if (columnName.Equals("QtyInvoiced"))
                {
                    int C_UOM_To_ID = GetC_UOM_ID();
                    QtyInvoiced = GetQtyInvoiced();
                    int precision = MProduct.Get(GetCtx(), M_Product_ID).GetUOMPrecision();
                    Decimal QtyInvoiced1 = Decimal.Round(QtyInvoiced, precision, MidpointRounding.AwayFromZero);
                    if (QtyInvoiced.CompareTo(QtyInvoiced1) != 0)
                    {
                        log.Fine("Corrected QtyInvoiced Scale "
                            + QtyInvoiced + "->" + QtyInvoiced1);
                        QtyInvoiced = QtyInvoiced1;
                        SetQtyInvoiced(QtyInvoiced);
                    }
                    QtyEntered = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, QtyInvoiced);
                    if (QtyEntered == null)
                        QtyEntered = QtyInvoiced;
                    bool conversion = QtyInvoiced.CompareTo(QtyEntered) != 0;
                    log.Fine("qty - UOM=" + C_UOM_To_ID
                        + ", QtyInvoiced=" + QtyInvoiced
                        + " -> " + conversion
                        + " QtyEntered=" + QtyEntered);
                    SetContext(WindowNo, "UOMConversion", conversion ? "Y" : "N");
                    SetQtyEntered(QtyEntered);
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetQty");
            }
            return true;
        }


        /**
         *	Invoice - Amount.
         *		- called from QtyInvoiced, PriceActual
         *		- calculates LineNetAmt
         *	@param ctx context
         *	@param WindowNo window no
         *	@param mTab tab
         *	@param mField field
         *	@param value value
         *	@return null or error message
         */
        private bool SetAmt(int WindowNo, String columnName)
        {
            try
            {

                //	log.log(Level.WARNING,"amt - init");
                int C_UOM_To_ID = GetC_UOM_ID();
                int M_Product_ID = GetM_Product_ID();
                int M_PriceList_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_ID");
                int StdPrecision = MPriceList.GetPricePrecision(GetCtx(), M_PriceList_ID);
                Decimal PriceActual, PriceEntered, PriceLimit, PriceList, Discount;
                Decimal? QtyEntered, QtyInvoiced;
                //	Get values
                QtyEntered = GetQtyEntered();
                QtyInvoiced = GetQtyInvoiced();
                log.Fine("QtyEntered=" + QtyEntered + ", Invoiced=" + QtyInvoiced + ", UOM=" + C_UOM_To_ID);
                //
                PriceEntered = GetPriceEntered();
                PriceActual = GetPriceActual();
                //	Discount = (Decimal)mTab.GetValue("Discount");
                PriceLimit = GetPriceLimit();
                PriceList = GetPriceList();
                log.Fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
                log.Fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual);// + ", Discount=" + Discount);

                //	Qty changed - recalc price
                if ((columnName.Equals("QtyInvoiced")
                    || columnName.Equals("QtyEntered")
                    || columnName.Equals("M_Product_ID"))
                    && !"N".Equals(GetCtx().GetContext(WindowNo, "DiscountSchema")))
                {
                    int C_BPartner_ID = GetCtx().GetContextAsInt(WindowNo, "C_BPartner_ID");
                    if (columnName.Equals("QtyEntered"))
                        QtyInvoiced = MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID,
                            C_UOM_To_ID, QtyEntered);
                    if (QtyInvoiced == null)
                        QtyInvoiced = QtyEntered;
                    bool IsSOTrx = GetCtx().IsSOTrx(WindowNo);
                    MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                            M_Product_ID, C_BPartner_ID, QtyInvoiced, IsSOTrx);
                    pp.SetM_PriceList_ID(M_PriceList_ID);
                    int M_PriceList_Version_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
                    pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                    DateTime date = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime(WindowNo, "DateInvoiced"));

                    pp.SetPriceDate(date);
                    //
                    PriceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, pp.GetPriceStd());
                    if (PriceEntered == null)
                        PriceEntered = pp.GetPriceStd();
                    //
                    log.Fine("amt - QtyChanged -> PriceActual=" + pp.GetPriceStd()
                        + ", PriceEntered=" + PriceEntered + ", Discount=" + pp.GetDiscount());
                    PriceActual = pp.GetPriceStd();
                    SetPriceActual(PriceActual);
                    //	mTab.SetValue("Discount", pp.GetDiscount());
                    SetPriceEntered(PriceEntered);
                    SetContext(WindowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");
                }
                else if (columnName.Equals("PriceActual"))
                {
                    PriceActual = GetPriceActual();
                    PriceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, (Decimal)PriceActual);
                    if (PriceEntered == null)
                        PriceEntered = PriceActual;
                    //
                    log.Fine("amt - PriceActual=" + PriceActual
                        + " -> PriceEntered=" + PriceEntered);
                    SetPriceEntered(PriceEntered);
                }
                else if (columnName.Equals("PriceEntered"))
                {
                    PriceEntered = GetPriceEntered();
                    PriceActual = (Decimal)MUOMConversion.ConvertProductTo(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, PriceEntered);
                    if (PriceActual == null)
                        PriceActual = PriceEntered;
                    //
                    log.Fine("amt - PriceEntered=" + PriceEntered
                        + " -> PriceActual=" + PriceActual);
                    SetPriceActual(PriceActual);
                }

                /**  Discount entered - Calculate Actual/Entered
                if (columnName.equals("Discount"))
                {
                    PriceActual = new Decimal ((100.0 - Discount.doubleValue()) / 100.0 * PriceList.doubleValue());
                    if (PriceActual.scale() > StdPrecision)
                        PriceActual = PriceActual.SetScale(StdPrecision, Decimal.ROUND_HALF_UP);
                    PriceEntered = MUOMConversion.convertProductFrom (ctx, M_Product_ID, 
                        C_UOM_To_ID, PriceActual);
                    if (PriceEntered == null)
                        PriceEntered = PriceActual;
                    mTab.SetValue("PriceActual", PriceActual);
                    mTab.SetValue("PriceEntered", PriceEntered);
                }
                //	calculate Discount
                else
                {
                    if (PriceList.intValue() == 0)
                        Discount = Env.ZERO;
                    else
                        Discount = new Decimal ((PriceList.doubleValue() - PriceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    if (Discount.scale() > 2)
                        Discount = Discount.SetScale(2, Decimal.ROUND_HALF_UP);
                    mTab.SetValue("Discount", Discount);
                }
                log.Fine("amt = PriceEntered=" + PriceEntered + ", Actual" + PriceActual + ", Discount=" + Discount);
                /* */

                //	Check PriceLimit
                String epl = GetCtx().GetContext(WindowNo, "EnforcePriceLimit");
                bool enforce = GetCtx().IsSOTrx(WindowNo) && epl != null && epl.Equals("Y");
                if (enforce && MRole.GetDefault(GetCtx()).IsOverwritePriceLimit())
                    enforce = false;
                //	Check Price Limit?
                if (enforce && Decimal.ToDouble((Decimal)PriceLimit) != 0.0
                  && PriceActual.CompareTo(PriceLimit) < 0)
                {
                    PriceActual = PriceLimit;
                    PriceEntered = (Decimal)MUOMConversion.ConvertProductFrom(GetCtx(), M_Product_ID,
                        C_UOM_To_ID, (Decimal)PriceLimit);
                    if (PriceEntered == 0)
                        PriceEntered = PriceLimit;
                    log.Fine("amt =(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                    SetPriceActual(PriceLimit);
                    SetPriceEntered(PriceEntered);
                    //addError(Msg.GetMsg(GetCtx(), "UnderLimitPrice"));
                    //	Repeat Discount calc
                    if (Decimal.ToInt32(PriceList) != 0)
                    {
                        Discount = new Decimal((Decimal.ToDouble(PriceList) - Decimal.ToDouble(PriceActual)) / Decimal.ToDouble(PriceList) * 100.0);
                        if (Env.Scale(Discount) > 2)
                            Discount = Decimal.Round(Discount, 2, MidpointRounding.AwayFromZero);
                        //	mTab.SetValue ("Discount", Discount);
                    }
                }

                //	Line Net Amt
                Decimal LineNetAmt = Decimal.Multiply((Decimal)QtyInvoiced, PriceActual);
                if (Env.Scale(LineNetAmt) > StdPrecision)
                    LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision, MidpointRounding.AwayFromZero);
                log.Info("amt = LineNetAmt=" + LineNetAmt);
                SetLineNetAmt(LineNetAmt);

                //	Calculate Tax Amount for PO
                bool isSOTrx = GetCtx().IsSOTrx(WindowNo);
                if (!isSOTrx)
                {
                    Decimal TaxAmt = Env.ZERO;
                    if (columnName.Equals("TaxAmt"))
                    {
                        TaxAmt = (Decimal)GetTaxAmt();
                    }
                    else
                    {
                        int taxID = GetC_Tax_ID();
                        if (taxID != null)
                        {
                            int C_Tax_ID = taxID;
                            MTax tax = new MTax(GetCtx(), C_Tax_ID, null);
                            TaxAmt = tax.CalculateTax(LineNetAmt, IsTaxIncluded(), StdPrecision);
                            SetTaxAmt(TaxAmt);
                        }
                    }
                    //	Add it up
                    SetLineTotalAmt(Decimal.Add(LineNetAmt, TaxAmt));
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetAmt");
            }
            return true;
        }


        /**
         * 	Set UOM - Callout
         *	@param oldC_UOM_ID old value
         *	@param newC_UOM_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetC_UOM_ID(String oldC_UOM_ID,
            String newC_UOM_ID, int windowNo)
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


        /**
         * 	Set QtyEntered - Callout
         *	@param oldQtyEntered old value
         *	@param newQtyEntered new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetQtyEntered(String oldQtyEntered,
            String newQtyEntered, int windowNo)
        {
            if (newQtyEntered == null || newQtyEntered.Length == 0)
                return;
            Decimal QtyEntered = Convert.ToDecimal(newQtyEntered);
            base.SetQtyEntered(QtyEntered);
            SetQty(windowNo, "QtyEntered");
            SetAmt(windowNo, "QtyEntered");
        }

        /**
         * 	Set QtyOrdered - Callout
         *	@param oldQtyInvoiced old value
         *	@param newQtyInvoiced new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetQtyInvoiced(String oldQtyInvoiced,
            String newQtyInvoiced, int windowNo)
        {
            if (newQtyInvoiced == null || newQtyInvoiced.Length == 0)
                return;
            Decimal qtyInvoiced = Convert.ToDecimal(newQtyInvoiced);
            base.SetQtyInvoiced(qtyInvoiced);
            SetQty(windowNo, "QtyInvoiced");
            SetAmt(windowNo, "QtyInvoiced");
        }



        /**
         * 	Set C_Tax_ID - Callout
         *	@param oldC_Tax_ID old value
         *	@param newC_Tax_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetC_Tax_ID(String oldC_Tax_ID,
            String newC_Tax_ID, int windowNo)
        {
            if (newC_Tax_ID == null || newC_Tax_ID.Length == 0)
                return;
            Decimal C_Tax_ID = Convert.ToDecimal(newC_Tax_ID);
            base.SetTaxAmt(C_Tax_ID);
            SetAmt(windowNo, "C_Tax_ID");
        }


        /**
         * 	Set PriceActual - Callout
         *	@param oldPriceActual old value
         *	@param newPriceActual new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetPriceActual(String oldPriceActual,
            String newPriceActual, int windowNo)
        {
            if (newPriceActual == null || newPriceActual.Length == 0)
                return;
            Decimal PriceActual = Convert.ToDecimal(newPriceActual);
            base.SetPriceActual(PriceActual);
            SetAmt(windowNo, "PriceActual");
        }

        /**
         * 	Set PriceEntered - Callout
         *	@param oldPriceEntered old value
         *	@param newPriceEntered new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetPriceEntered(String oldPriceEntered,
            String newPriceEntered, int windowNo)
        {
            if (newPriceEntered == null || newPriceEntered.Length == 0)
                return;
            Decimal PriceEntered = Convert.ToDecimal(newPriceEntered);
            base.SetPriceEntered(PriceEntered);
            SetAmt(windowNo, "PriceEntered");
        }	//	SetPriceEntered


        /**
         * 	Set TaxAmt - Callout
         *	@param oldTaxAmt old value
         *	@param newTaxAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetTaxAmt(String oldTaxAmt,
            String newTaxAmt, int windowNo)
        {
            if (newTaxAmt == null || newTaxAmt.Length == 0)
                return;
            Decimal taxAmt = Convert.ToDecimal(newTaxAmt);
            base.SetTaxAmt(taxAmt);
            SetAmt(windowNo, "TaxAmt");
        }


        /***
         *	Invoice Line - Product.
         *		- reSet C_Charge_ID / M_AttributeSetInstance_ID
         *		- PriceList, PriceStd, PriceLimit, C_Currency_ID, EnforcePriceLimit
         *		- UOM
         *	Calls Tax
         *	@param oldM_Product_ID old value
         *	@param newM_Product_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetM_Product_ID(String oldM_Product_ID,
            String newM_Product_ID, int WindowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                return;
            int M_Product_ID = int.Parse(newM_Product_ID);
            if (M_Product_ID == 0)
                return;

            SetC_Charge_ID(0);

            //	Set Attribute
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO,
                Env.TAB_INFO, "M_AttributeSetInstance_ID") != 0)
                SetM_AttributeSetInstance_ID((GetCtx().GetContextAsInt(Env.WINDOW_INFO,
                    Env.TAB_INFO, "M_AttributeSetInstance_ID")));
            else
                SetM_AttributeSetInstance_ID(-1);

            /*****	Price Calculation see also qty	****/
            bool IsSOTrx = GetCtx().IsSOTrx(WindowNo);
            int C_BPartner_ID = GetCtx().GetContextAsInt(WindowNo, "C_BPartner_ID");
            Decimal Qty = GetQtyInvoiced();
            MProductPricing pp = new MProductPricing(GetAD_Client_ID(), GetAD_Org_ID(),
                    M_Product_ID, C_BPartner_ID, Qty, IsSOTrx);
            //
            int M_PriceList_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_ID");
            pp.SetM_PriceList_ID(M_PriceList_ID);
            int M_PriceList_Version_ID = GetCtx().GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            long time = GetCtx().GetContextAsTime(WindowNo, "DateInvoiced");
            pp.SetPriceDate(time);
            //		
            SetPriceList(pp.GetPriceList());
            SetPriceLimit(pp.GetPriceLimit());
            SetPriceActual(pp.GetPriceStd());
            SetPriceEntered(pp.GetPriceStd());
            SetContext(WindowNo, "C_Currency_ID", pp.GetC_Currency_ID().ToString());
            //	mTab.SetValue("Discount", pp.GetDiscount());
            SetC_UOM_ID(pp.GetC_UOM_ID());
            SetContext(WindowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
            SetContext(WindowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");
            //
            SetTax(WindowNo, "M_Product_ID");

            return;
        }

        /**
         * 	Set Charge - Callout
         *	@param oldC_Charge_ID old value
         *	@param newC_Charge_ID new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetC_Charge_ID(String oldC_Charge_ID,
            String newC_Charge_ID, int WindowNo)
        {
            if (newC_Charge_ID == null || newC_Charge_ID.Length == 0)
                return;
            int C_Charge_ID = int.Parse(newC_Charge_ID);
            if (C_Charge_ID == 0)
                return;

            //	No Product defined
            if (GetM_Product_ID() != 0)
            {
                SetC_Charge_ID(0);

                //addError( Msg.GetMsg( GetCtx(), "ChargeExclusively" ) );
            }
            SetM_AttributeSetInstance_ID(-1);
            SetS_ResourceAssignment_ID(0);
            SetC_UOM_ID(100);	//	EA

            SetContext(WindowNo, "DiscountSchema", "N");
            String sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;

            IDataReader idr = null;
            try
            {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.SetInt(1, C_Charge_ID);
                //ResultSet rs = pstmt.executeQuery();
                idr = DataBase.DB.ExecuteReader(sql, null, null);

                if (idr.Read())
                {
                    SetPriceEntered(idr.GetDecimal(0));
                    SetPriceActual(idr.GetDecimal(0));
                    SetPriceLimit(Env.ZERO);
                    SetPriceList(Env.ZERO);
                    SetContext(WindowNo, "Discount", Env.ZERO.ToString());
                }

                idr.Close();
            }
            catch (SqlException e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql + e);
                //addError( e.GetLocalizedMessage() );
            }

            //
            SetTax(WindowNo, "C_Charge_ID");
        }



        /**
         *	Invoice Line - Tax.
         *		- basis: Product, Charge, BPartner Location
         *		- Sets C_Tax_ID
         *  Calles Amount
         *	@param ctx context
         *	@param WindowNo window no
         *	@param mTab tab
         *	@param mField field
         *	@param value value
         *	@return null or error message
         */
        private bool SetTax(int WindowNo, String columnName)
        {
            try
            {
                //	Check Product
                int M_Product_ID = GetM_Product_ID();
                int C_Charge_ID = GetC_Charge_ID();
                log.Fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
                if (M_Product_ID == 0 && C_Charge_ID == 0)
                    return SetAmt(WindowNo, columnName);

                //	Check Partner Location
                int shipC_BPartner_Location_ID = GetCtx().GetContextAsInt(WindowNo, "C_BPartner_Location_ID");
                if (shipC_BPartner_Location_ID == 0)
                    return SetAmt(WindowNo, columnName);
                log.Fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
                int billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
                log.Fine("Bill BP_Location=" + billC_BPartner_Location_ID);

                //	Dates
                DateTime billDate = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime(WindowNo, "DateInvoiced"));
                log.Fine("Bill Date=" + billDate);
                DateTime shipDate = billDate;
                log.Fine("Ship Date=" + shipDate);

                int AD_Org_ID = GetAD_Org_ID();
                log.Fine("Org=" + AD_Org_ID);

                int M_Warehouse_ID = GetCtx().GetContextAsInt("#M_Warehouse_ID");
                log.Fine("Warehouse=" + M_Warehouse_ID);

                //
                int C_Tax_ID = Tax.Get(GetCtx(), M_Product_ID, C_Charge_ID, billDate, shipDate,
                    AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
                    GetCtx().IsSOTrx(WindowNo));
                log.Info("Tax ID=" + C_Tax_ID);
                //
                if (C_Tax_ID == 0)
                {
                    //ValueNamePair pp = CLogger.retrieveError();
                    //if (pp != null)
                    //    addError(pp.GetValue());
                    //else
                    //    addError( Msg.GetMsg( GetCtx(), "Tax Error" ) );
                }
                else
                    SetC_Tax_ID(C_Tax_ID);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("MinvoiceLine--SetTax");
            }
            return SetAmt(WindowNo, columnName);
        }

    }
}
