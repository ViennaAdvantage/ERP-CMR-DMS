using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VIS.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /VIS/Common/
        public ActionResult Index()
        {
            return View();
        }

        #region Create Line From

        /// <summary>
        /// save/create lines from shipment form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveShipment(List<Dictionary<string, string>> model, string selectedItems, string C_Order_ID, string C_Invoice_ID, string m_locator_id, string M_inout_id)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveShipmentData(ctx, model, selectedItems, Convert.ToInt32(C_Order_ID), Convert.ToInt32(C_Invoice_ID), Convert.ToInt32(m_locator_id), Convert.ToInt32(M_inout_id));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from shipment form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveInvoice(List<Dictionary<string, string>> model, string selectedItems, string C_Order_ID, string C_Invoice_ID, string M_inout_id)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveInvoiceData(ctx, model, selectedItems, Convert.ToInt32(C_Order_ID), Convert.ToInt32(C_Invoice_ID), Convert.ToInt32(M_inout_id));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// save/create lines from BankStatement form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult SaveStatment(List<Dictionary<string, string>> model, string selectedItems, string C_BankStatement_ID)
        {
            var value = false;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                value = obj.SaveStatmentData(ctx, model, selectedItems, Convert.ToInt32(C_BankStatement_ID));
            }
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Menual Forms

        public JsonResult GenerateInvoices(string whereClause)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GenerateInvoices(ctx, whereClause);
                return Json(new { obj.ErrorMsg, obj.lblStatusInfo, obj.statusBar, obj.DocumentText }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateShipments(string whereClause, string M_Warehouse_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GenerateShipments(ctx, whereClause, M_Warehouse_ID);
                return Json(new { obj.ErrorMsg, obj.lblStatusInfo, obj.statusBar, obj.DocumentText }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Match PO


        public JsonResult Consolidate()
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                MMatchPO.Consolidate(ctx);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult createMatchRecord(bool invoice, string M_InOutLine_ID, string Line_ID, string qty)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.CreateMatchRecord(ctx, invoice, Convert.ToInt32(M_InOutLine_ID), Convert.ToInt32(Line_ID), Convert.ToDecimal(qty));
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Account Viewer

        public JsonResult GetDataQuery(int AD_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4, String sort1, String sort2, String sort3, String sort4)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.GetDataQuery(ctx, AD_Client_ID, whereClause, orderClause, gr1, gr2, gr3, gr4, sort1, sort2, sort3, sort4);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Archive Viewer

        public JsonResult UpdateArchive(string name, string des, string help, int archiveId)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var value = obj.UpdateArchive(ctx, name, des, help, archiveId);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadPdf(int archiveId)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var base64EncodedPDF = obj.DownloadPdf(ctx, archiveId);
                return Json(new { result = base64EncodedPDF }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Generate XClasses

        public JsonResult GenerateXClasses(string directory, bool chkStatus, string tableId, string classType)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                StringBuilder sbTextCopy = new StringBuilder();
                string fileName = string.Empty;

                var msg = VAdvantage.Tool.GenerateModel.StartProcess("ViennaAdvantage.Model", directory, chkStatus, tableId, classType, out  sbTextCopy, out  fileName);
                string contant = sbTextCopy.ToString();
                return Json(new { contant, fileName, msg }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = "Error" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region VAttributeGrid
        public JsonResult GetDataQueryAttribute()
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                List<AttributeGrid> lst = new List<AttributeGrid>();
                var value = MAttribute.GetOfClient(ctx, true, true);

                for (int i = 0; i < value.Length; i++)
                {
                    AttributeGrid lstObj = new AttributeGrid();
                    lstObj.GetKeyNamePair = value[i].GetKeyNamePair();
                    List<MAttributeValueList> lstAttValueObj = new List<MAttributeValueList>();
                    for (int k = 0; k < value[i].GetMAttributeValues().Length; k++)
                    {
                        var attrValue = value[i].GetMAttributeValues()[k];
                        if (attrValue != null)
                        {
                            MAttributeValueList localObj = new MAttributeValueList();
                            localObj.GetM_Attribute_ID = attrValue.GetM_Attribute_ID();
                            localObj.GetM_AttributeValue_ID = attrValue.GetM_AttributeValue_ID();
                            localObj.Name = attrValue.GetName();
                            lstAttValueObj.Add(localObj);
                        }
                    }

                    lstObj.GetMAttributeValues = lstAttValueObj.ToArray();
                    lstObj.GetName = value[i].GetName();
                    lst.Add(lstObj);
                }

                CommonModel obj = new CommonModel();
                List<KeyNamePair> priceList = new List<KeyNamePair>();
                List<KeyNamePair> whList = new List<KeyNamePair>();
                obj.FillPicks(ctx, out priceList, out whList);
                return Json(new { lst, priceList, whList }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGridElement(int xM_Attribute_ID, int xM_AttributeValue_ID, int yM_Attribute_ID, int yM_AttributeValue_ID, int M_PriceList_Version_ID, int M_Warehouse_ID, string windowNo)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                CommonModel obj = new CommonModel();
                var stValue = obj.GetGridElement(ctx, xM_Attribute_ID, xM_AttributeValue_ID, yM_Attribute_ID, yM_AttributeValue_ID, M_PriceList_Version_ID, M_Warehouse_ID, windowNo);
                return Json(new { stValue }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }

    public class AttributeGrid
    {
        public KeyNamePair GetKeyNamePair { get; set; }
        public MAttributeValueList[] GetMAttributeValues { get; set; }
        public String GetName { get; set; }
    }

    public class MAttributeValueList
    {
        public int GetM_Attribute_ID { get; set; }
        public int GetM_AttributeValue_ID { get; set; }
        public string Name { get; set; }
    }


    public class CommonModel
    {
        #region VAttributeGrid

        public string GetGridElement(Ctx ctx, int xM_Attribute_ID, int xM_AttributeValue_ID, int yM_Attribute_ID, int yM_AttributeValue_ID, int M_PriceList_Version_ID, int M_Warehouse_ID, string windowNo)
        {
            StringBuilder panel = new StringBuilder();
            String sql = "SELECT * FROM M_Product WHERE IsActive='Y'";
            //	Product Attributes
            if (xM_Attribute_ID > 0)
            {
                sql += " AND M_AttributeSetInstance_ID IN "
                    + "(SELECT M_AttributeSetInstance_ID "
                    + "FROM M_AttributeInstance "
                    + "WHERE M_Attribute_ID=" + xM_Attribute_ID
                    + " AND M_AttributeValue_ID=" + xM_AttributeValue_ID + ")";
            }
            if (yM_Attribute_ID > 0)
            {
                sql += " AND M_AttributeSetInstance_ID IN "
                    + "(SELECT M_AttributeSetInstance_ID "
                    + "FROM M_AttributeInstance "
                    + "WHERE M_Attribute_ID=" + yM_Attribute_ID
                    + " AND M_AttributeValue_ID=" + yM_AttributeValue_ID + ")";
            }
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_Product", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);
            DataTable dt = null;
            IDataReader idr = null;
            int noProducts = 0;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                idr = null;
                foreach (DataRow dr in dt.Rows)
                {
                    MProduct product = new MProduct(Env.GetContext(), dr, null);
                    panel.Append(AddProduct(product, M_PriceList_Version_ID, M_Warehouse_ID, windowNo));
                    noProducts++;
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                idr = null;
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }

            return panel.ToString();
        }

        private string AddProduct(MProduct product, int M_PriceList_Version_ID, int M_Warehouse_ID, string windowNo)
        {
            int M_Product_ID = product.GetM_Product_ID();
            StringBuilder obj = new StringBuilder();
            obj.Append("<table style='width: 100%;'><tr>");

            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblproductVal_" + windowNo + "' class='VIS_Pref_Label_Font'>" + product.GetValue() + "</label>");
            obj.Append("</td>");

            String formatted = "";
            if (M_PriceList_Version_ID != 0)
            {
                MProductPrice pp = MProductPrice.Get(Env.GetContext(), M_PriceList_Version_ID, M_Product_ID, null);
                if (pp != null)
                {
                    Decimal price = pp.GetPriceStd();
                    formatted = price.ToString();// _price.format(price);
                }
                else
                {
                    formatted = "-";
                }
            }

            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblformate_" + windowNo + "' class='VIS_Pref_Label_Font'>" + formatted.ToString() + "</label>");
            obj.Append("</td>");


            obj.Append("</tr>");
            obj.Append("<tr>");

            //	Product Name - Qty
            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblProductName_" + windowNo + "' class='VIS_Pref_Label_Font'>" + product.GetName() + "</label>");
            obj.Append("</td>");

            formatted = "";
            if (M_Warehouse_ID != 0)
            {
                Decimal qty = Util.GetValueOfDecimal(MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, 0, null));
                if (qty == null)
                {
                    formatted = "-";
                }
                else
                {
                    formatted = qty.ToString();
                }
            }

            obj.Append("</tr>");
            obj.Append("<tr>");

            //	Product Name - Qty
            obj.Append("<td>");
            obj.Append("<label style='padding-bottom: 10px; padding-right: 5px;' id=lblfomatepanel_" + windowNo + "' class='VIS_Pref_Label_Font'>" + formatted.ToString() + "</label>");
            obj.Append("</td>");

            obj.Append("</tr>");
            obj.Append("</table>");

            return obj.ToString();
        }

        public void FillPicks(Ctx ctx, out List<KeyNamePair> priceList, out List<KeyNamePair> whList)
        {
            priceList = new List<KeyNamePair>();
            whList = new List<KeyNamePair>();
            //	Price List
            String sql = "SELECT M_PriceList_Version.M_PriceList_Version_ID,"
                + " M_PriceList_Version.Name || ' (' || c.Iso_Code || ')' AS ValueName "
                + "FROM M_PriceList_Version, M_PriceList pl, C_Currency c "
                + "WHERE M_PriceList_Version.M_PriceList_ID=pl.M_PriceList_ID"
                + " AND pl.C_Currency_ID=c.C_Currency_ID"
                + " AND M_PriceList_Version.IsActive='Y' AND pl.IsActive='Y'";
            //	Add Access & Order
            sql = MRole.GetDefault(ctx).AddAccessSQL(sql, "M_PriceList_Version", true, false)	// fully qualidfied - RO 
                + " ORDER BY M_PriceList_Version.Name";
            System.Data.IDataReader idr = null;
            try
            {
                priceList.Add(new KeyNamePair(0, ""));
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    KeyNamePair kn = new KeyNamePair(Util.GetValueOfInt(idr[0]), idr[1].ToString());
                    priceList.Add(kn);
                }
                idr.Close();
                //	Warehouse
                sql = "SELECT M_Warehouse_ID, Value || ' - ' || Name AS ValueName "
                    + "FROM M_Warehouse "
                    + "WHERE IsActive='Y'";
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql,
                        "M_Warehouse", MRole.SQL_NOTQUALIFIED, MRole.SQL_RO)
                    + " ORDER BY Value";
                whList.Add(new KeyNamePair(0, ""));
                idr = DB.ExecuteReader(sql, null, null);
                while (idr.Read())
                {
                    KeyNamePair kn = new KeyNamePair(Util.GetValueOfInt(idr["M_Warehouse_ID"]), idr["ValueName"].ToString());
                    whList.Add(kn);
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
            }
        }

        #endregion


        #region Create Line From

        public bool SaveShipmentData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int C_Order_ID, int C_Invoice_ID, int M_Locator_ID, int M_InOut_ID)
        {
            MOrder _order = null;
            if (C_Order_ID > 0)
            {
                _order = new MOrder(ctx, C_Order_ID, null);
            }

            MInvoice _invoice = null;
            if (C_Invoice_ID > 0)
            {
                _invoice = new MInvoice(ctx, C_Invoice_ID, null);
            }


            MInOut inout = null;
            if (M_InOut_ID > 0)
            {
                inout = new MInOut(ctx, M_InOut_ID, null);
            }

            /**
             *  Selected        - 0
             *  QtyEntered      - 1
             *  C_UOM_ID        - 2
             *  M_Product_ID    - 3
             *  OrderLine       - 4
             *  ShipmentLine    - 5
             *  InvoiceLine     - 6
             */

            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                //  variable values
                int C_UOM_ID = 0;
                int M_Product_ID = 0;
                int C_OrderLine_ID = 0;
                int C_InvoiceLine_ID = 0;
                int M_AttributeSetInstance_ID = 0;
                Double d = Convert.ToDouble(model[i]["QuantityEntered"]);                      //  1-Qty
                Decimal QtyEntered = Convert.ToDecimal(d);
                if (model[i]["C_UOM_ID_K"] != "")
                    C_UOM_ID = Convert.ToInt32((model[i]["C_UOM_ID_K"]));               //  2-UOM
                if (model[i]["M_Product_ID_K"] != "")
                    M_Product_ID = Convert.ToInt32((model[i]["M_Product_ID_K"]));       //  3-Product
                if (model[i]["C_Order_ID_K"] != "")
                    C_OrderLine_ID = Convert.ToInt32((model[i]["C_Order_ID_K"]));       //  4-OrderLine
                if (model[i]["C_Invoice_ID_K"] != "")
                    C_InvoiceLine_ID = Convert.ToInt32((model[i]["C_Invoice_ID_K"]));   //  6-InvoiceLine
                if (model[i]["M_AttributeSetInstance_ID_K"] != "")
                    M_AttributeSetInstance_ID = Convert.ToInt32((model[i]["M_AttributeSetInstance_ID_K"]));   //  6-InvoiceLine

                MInvoiceLine il = null;
                if (C_InvoiceLine_ID != 0)
                {
                    il = new MInvoiceLine(ctx, C_InvoiceLine_ID, null);
                }
                bool isInvoiced = (C_InvoiceLine_ID != 0);
                //	Precision of Qty UOM
                int precision = 2;
                if (M_Product_ID != 0)
                {
                    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    precision = product.GetUOMPrecision();
                }
                QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero);

                //	Create new InOut Line
                MInOutLine iol = new MInOutLine(inout);
                iol.SetM_Product_ID(M_Product_ID, C_UOM_ID, M_AttributeSetInstance_ID);	//	Line UOM
                iol.SetQty(QtyEntered);							//	Movement/Entered
                //
                MOrderLine ol = null;
                if (C_OrderLine_ID != 0)
                {
                    iol.SetC_OrderLine_ID(C_OrderLine_ID);
                    ol = new MOrderLine(ctx, C_OrderLine_ID, null);
                    if (ol.GetQtyEntered().CompareTo(ol.GetQtyOrdered()) != 0)
                    {
                        iol.SetMovementQty(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, ol.GetQtyOrdered()), ol.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        iol.SetC_UOM_ID(ol.GetC_UOM_ID());
                    }
                    // iol.SetM_AttributeSetInstance_ID(ol.GetM_AttributeSetInstance_ID());
                    //iol.SetM_AttributeSetInstance_ID(0);//zero Becouse create diffrent SetM_AttributeSetInstance_ID for MR agaist one PO
                    iol.SetDescription(ol.GetDescription());
                    iol.SetC_Project_ID(ol.GetC_Project_ID());
                    iol.SetC_ProjectPhase_ID(ol.GetC_ProjectPhase_ID());
                    iol.SetC_ProjectTask_ID(ol.GetC_ProjectTask_ID());
                    iol.SetC_Activity_ID(ol.GetC_Activity_ID());
                    iol.SetC_Campaign_ID(ol.GetC_Campaign_ID());
                    iol.SetAD_OrgTrx_ID(ol.GetAD_OrgTrx_ID());
                    iol.SetUser1_ID(ol.GetUser1_ID());
                    iol.SetUser2_ID(ol.GetUser2_ID());
                }
                else if (il != null)
                {
                    if (il.GetQtyEntered().CompareTo(il.GetQtyInvoiced()) != 0)
                    {
                        //iol.SetQtyEntered(QtyEntered.multiply(il.getQtyInvoiced()).divide(il.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        iol.SetQtyEntered(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered, il.GetQtyInvoiced()), il.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                        iol.SetC_UOM_ID(il.GetC_UOM_ID());
                    }
                    iol.SetDescription(il.GetDescription());
                    iol.SetC_Project_ID(il.GetC_Project_ID());
                    iol.SetC_ProjectPhase_ID(il.GetC_ProjectPhase_ID());
                    iol.SetC_ProjectTask_ID(il.GetC_ProjectTask_ID());
                    iol.SetC_Activity_ID(il.GetC_Activity_ID());
                    iol.SetC_Campaign_ID(il.GetC_Campaign_ID());
                    iol.SetAD_OrgTrx_ID(il.GetAD_OrgTrx_ID());
                    iol.SetUser1_ID(il.GetUser1_ID());
                    iol.SetUser2_ID(il.GetUser2_ID());
                }
                //	Charge
                if (M_Product_ID == 0)
                {
                    if (ol != null && ol.GetC_Charge_ID() != 0)			//	from order
                    {
                        iol.SetC_Charge_ID(ol.GetC_Charge_ID());
                    }
                    else if (il != null && il.GetC_Charge_ID() != 0)	//	from invoice
                    {
                        iol.SetC_Charge_ID(il.GetC_Charge_ID());
                    }
                }

                iol.SetM_Locator_ID(M_Locator_ID);

                if (!iol.Save())
                {
                    //s_log.log(Level.SEVERE, "Line NOT created #" + i);
                }
                //	Create Invoice Line Link
                else if (il != null)
                {
                    il.SetM_InOutLine_ID(iol.GetM_InOutLine_ID());
                    il.Save();
                }

            }   //  for all rows

            /**
             *  Update Header
             *  - if linked to another order/invoice - remove link
             *  - if no link set it
             */
            if (_order != null && _order.GetC_Order_ID() != 0)
            {
                inout.SetC_Order_ID(_order.GetC_Order_ID());
                inout.SetDateOrdered(_order.GetDateOrdered());
                inout.SetAD_OrgTrx_ID(_order.GetAD_OrgTrx_ID());
                inout.SetC_Project_ID(_order.GetC_Project_ID());
                inout.SetC_Campaign_ID(_order.GetC_Campaign_ID());
                inout.SetC_Activity_ID(_order.GetC_Activity_ID());
                inout.SetUser1_ID(_order.GetUser1_ID());
                inout.SetUser2_ID(_order.GetUser2_ID());
            }
            if (_invoice != null && _invoice.GetC_Invoice_ID() != 0)
            {
                if (inout.GetC_Order_ID() == 0)
                {
                    inout.SetC_Order_ID(_invoice.GetC_Order_ID());
                }
                inout.SetC_Invoice_ID(_invoice.GetC_Invoice_ID());
                inout.SetDateOrdered(_invoice.GetDateOrdered());
                inout.SetAD_OrgTrx_ID(_invoice.GetAD_OrgTrx_ID());
                inout.SetC_Project_ID(_invoice.GetC_Project_ID());
                inout.SetC_Campaign_ID(_invoice.GetC_Campaign_ID());
                inout.SetC_Activity_ID(_invoice.GetC_Activity_ID());
                inout.SetUser1_ID(_invoice.GetUser1_ID());
                inout.SetUser2_ID(_invoice.GetUser2_ID());
            }
            inout.Save();
            return true;
        }

        public bool SaveInvoiceData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int C_Order_ID, int C_Invoice_ID, int M_InOut_ID)
        {
            MOrder _order = null;
            if (C_Order_ID > 0)
            {
                _order = new MOrder(ctx, C_Order_ID, null);
            }

            MInvoice _invoice = null;
            if (C_Invoice_ID > 0)
            {
                _invoice = new MInvoice(ctx, C_Invoice_ID, null);
            }


            MInOut _inout = null;
            if (M_InOut_ID > 0)
            {
                _inout = new MInOut(ctx, M_InOut_ID, null);
            }

            if (_order != null)
            {
                _invoice.SetOrder(_order);	//	overwrite header values
                _invoice.Save();
            }
            if (_inout != null && _inout.GetM_InOut_ID() != 0
                && _inout.GetC_Invoice_ID() == 0)	//	only first time
            {
                _inout.SetC_Invoice_ID(C_Invoice_ID);
                _inout.Save();
            }

            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                //  variable values
                int C_UOM_ID = 0;
                int M_Product_ID = 0;
                int C_OrderLine_ID = 0;
                int M_InOutLine_ID = 0;
                Double d = Convert.ToDouble(model[i]["Quantity"]);                      //  1-Qty
                Decimal QtyEntered = Convert.ToDecimal(d);
                if (model[i]["C_UOM_ID_K"] != "")
                    C_UOM_ID = Convert.ToInt32((model[i]["C_UOM_ID_K"]));               //  2-UOM
                if (model[i]["M_Product_ID_K"] != "")
                    M_Product_ID = Convert.ToInt32((model[i]["M_Product_ID_K"]));       //  3-Product
                if (model[i]["C_Order_ID_K"] != "")
                    C_OrderLine_ID = Convert.ToInt32((model[i]["C_Order_ID_K"]));       //  4-OrderLine
                if (model[i]["M_InOut_ID_K"] != "")
                    M_InOutLine_ID = Convert.ToInt32((model[i]["M_InOut_ID_K"]));   //  5-Shipment

                //	Precision of Qty UOM
                int precision = 2;
                if (M_Product_ID != 0)
                {
                    MProduct product = MProduct.Get(ctx, M_Product_ID);
                    precision = product.GetUOMPrecision();
                }
                QtyEntered = Decimal.Round(QtyEntered, precision, MidpointRounding.AwayFromZero);
                //s_log.fine("Line QtyEntered=" + QtyEntered
                //    + ", Product_ID=" + M_Product_ID 
                //    + ", OrderLine_ID=" + C_OrderLine_ID + ", InOutLine_ID=" + M_InOutLine_ID);

                //	Create new Invoice Line
                MInvoiceLine invoiceLine = new MInvoiceLine(_invoice);
                invoiceLine.SetM_Product_ID(M_Product_ID, C_UOM_ID);	//	Line UOM
                invoiceLine.SetQty(QtyEntered);							//	Invoiced/Entered
                //  Info
                MOrderLine orderLine = null;
                if (C_OrderLine_ID != 0)
                    orderLine = new MOrderLine(ctx, C_OrderLine_ID, null);
                MInOutLine inoutLine = null;
                if (M_InOutLine_ID != 0)
                {
                    inoutLine = new MInOutLine(ctx, M_InOutLine_ID, null);
                    if (orderLine == null && inoutLine.GetC_OrderLine_ID() != 0)
                    {
                        C_OrderLine_ID = inoutLine.GetC_OrderLine_ID();
                        orderLine = new MOrderLine(ctx, C_OrderLine_ID, null);
                    }
                }
                else
                {
                    MInOutLine[] lines = MInOutLine.GetOfOrderLine(ctx, C_OrderLine_ID, null, null);
                    //s_log.fine ("Receipt Lines with OrderLine = #" + lines.length);
                    if (lines.Length > 0)
                    {
                        for (int j = 0; j < lines.Length; j++)
                        {
                            MInOutLine line = lines[j];
                            if (line.GetQtyEntered().CompareTo(QtyEntered) == 0)
                            {
                                inoutLine = line;
                                M_InOutLine_ID = inoutLine.GetM_InOutLine_ID();
                                break;
                            }
                        }
                        if (inoutLine == null)
                        {
                            inoutLine = lines[0];	//	first as default
                            M_InOutLine_ID = inoutLine.GetM_InOutLine_ID();
                        }
                    }
                }	//	get Ship info

                //	Shipment Info
                if (inoutLine != null)
                {
                    invoiceLine.SetShipLine(inoutLine);		//	overwrites
                    if (inoutLine.GetQtyEntered().CompareTo(inoutLine.GetMovementQty()) != 0)
                    {
                        //invoiceLine.setQtyInvoiced(QtyEntered
                        //.multiply(inoutLine.getMovementQty())
                        //.divide(inoutLine.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        invoiceLine.SetQtyInvoiced(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered,
                        inoutLine.GetMovementQty()),
                        inoutLine.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                    }
                }
                else
                {
                    //s_log.fine("No Receipt Line");
                }

                //	Order Info
                if (orderLine != null)
                {
                    invoiceLine.SetOrderLine(orderLine);	//	overwrites

                    /* nnayak - Bug 1567690. The organization from the Orderline can be different from the organization 
                    on the header */
                    invoiceLine.SetClientOrg(orderLine.GetAD_Client_ID(), orderLine.GetAD_Org_ID());
                    if (orderLine.GetQtyEntered().CompareTo(orderLine.GetQtyOrdered()) != 0)
                    {
                        //invoiceLine.setQtyInvoiced(QtyEntered
                        //    .multiply(orderLine.getQtyOrdered())
                        //    .divide(orderLine.getQtyEntered(), 12, Decimal.ROUND_HALF_UP));
                        invoiceLine.SetQtyInvoiced(Decimal.Round(Decimal.Divide(Decimal.Multiply(QtyEntered,
                        orderLine.GetQtyOrdered()),
                        orderLine.GetQtyEntered()), 12, MidpointRounding.AwayFromZero));
                    }

                }
                else
                {
                    //s_log.fine("No Order Line");

                    /* nnayak - Bug 1567690. The organization from the Receipt can be different from the organization 
                    on the header */
                    if (inoutLine != null)
                    {
                        invoiceLine.SetClientOrg(inoutLine.GetAD_Client_ID(), inoutLine.GetAD_Org_ID());
                    }

                    invoiceLine.SetPrice();
                    invoiceLine.SetTax();
                }
                if (!invoiceLine.Save())
                {
                    //s_log.log(Level.SEVERE, "Line NOT created #" + i);
                }

            }   //  for all rows

            return true;
        }

        public bool SaveStatmentData(Ctx ctx, List<Dictionary<string, string>> model, string selectedItems, int C_BankStatement_ID)
        {
            MBankStatement bs = new MBankStatement(ctx, C_BankStatement_ID, null);
            //  Lines
            for (int i = 0; i < model.Count; i++)
            {
                DateTime trxDate = Convert.ToDateTime(model[i]["Date"]);          //  1-DateTrx
                int C_Payment_ID = Convert.ToInt32(model[i]["C_Payment_ID_K"]);   //  2-C_Payment_ID
                int C_Currency_ID = Convert.ToInt32(model[i]["C_Currency_ID_K"]); //  3-Currency
                Decimal TrxAmt = Convert.ToDecimal(model[i]["Amount"]);           //  4-PayAmt

                MBankStatementLine bsl = new MBankStatementLine(bs);
                bsl.SetStatementLineDate(trxDate);
                bsl.SetPayment(new MPayment(ctx, C_Payment_ID, null));
                if (!bsl.Save())
                {
                    //s_log.log(Level.SEVERE, "Line not created #" + i);
                }
            }   //  for all rows
            return true;
        }

        #endregion

        #region Menual Forms

        public string statusBar { get; set; }
        public string lblStatusInfo { get; set; }
        public string ErrorMsg { get; set; }
        public string DocumentText { get; set; }

        public string GenerateInvoices(Ctx ctx, string whereClause)
        {
            //String trxName = null;
            Trx trx = null;

            //	Reset Selection
            String sql = "UPDATE C_Order SET IsSelected = 'N' WHERE IsSelected='Y'"
                + " AND AD_Client_ID=" + ctx.GetAD_Client_ID()
                + " AND AD_Org_ID=" + ctx.GetAD_Org_ID();

            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));

            //log.Config("Reset=" + no);

            //	Set Selection
            sql = "UPDATE C_Order SET IsSelected = 'Y' WHERE " + whereClause;
            no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));
            if (no == 0)
            {
                String msg = "No Invoices";     //  not translated!
                //log.Config(msg);
                //info.setText(msg);
                lblStatusInfo = msg.ToString();
                return msg.ToString();
            }
            // log.Config("Set=" + no);

            lblStatusInfo = Msg.GetMsg(ctx, "InvGenerateGen");
            statusBar += no.ToString();

            //	Prepare Process
            int AD_Process_ID = 134;  // HARDCODED    C_InvoiceCreate
            MPInstance instance = new MPInstance(ctx, AD_Process_ID, 0);
            if (!instance.Save())
            {
                lblStatusInfo = Msg.GetMsg(ctx, "ProcessNoInstance");
                return Msg.GetMsg(ctx, "ProcessNoInstance");
            }

            ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
            pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

            pi.SetAD_Client_ID(ctx.GetAD_Client_ID());

            //	Add Parameters
            MPInstancePara para = new MPInstancePara(instance, 10);
            para.setParameter("Selection", "Y");
            if (!para.Save())
            {
                String msg = "No Selection Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            para = new MPInstancePara(instance, 20);
            para.setParameter("DocAction", "CO");
            if (!para.Save())
            {
                String msg = "No DocAction Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }
            ProcessCtl worker = new ProcessCtl(ctx, null, pi, trx);
            worker.Run();
            GenerateInvoice_complete(ctx, pi, "");
            return "";
        }

        public string GenerateShipments(Ctx ctx, string whereClause, string M_Warehouse_ID)
        {

            Trx trx = null;

            //	Reset Selection
            String sql = "UPDATE C_Order SET IsSelected = 'N' "
            + "WHERE IsSelected='Y'"
            + " AND AD_Client_ID=" + ctx.GetAD_Client_ID();
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));

            //	Set Selection
            sql = "UPDATE C_Order SET IsSelected = 'Y' WHERE " + whereClause;
            no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, trx));
            if (no == 0)
            {
                String msg = "No Shipments";     //  not translated!
                lblStatusInfo = msg.ToString();
            }
            lblStatusInfo = Msg.GetMsg(ctx, "InOutGenerateGen");
            statusBar += no.ToString();

            //	Prepare Process
            int AD_Process_ID = 199;	  // M_InOutCreate - Vframwork.Process.InOutGenerate
            MPInstance instance = new MPInstance(ctx, AD_Process_ID, 0);
            if (!instance.Save())
            {
                lblStatusInfo = Msg.GetMsg(ctx, "ProcessNoInstance");
            }
            ProcessInfo pi = new ProcessInfo("VInOutGen", AD_Process_ID);
            pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());

            pi.SetAD_Client_ID(ctx.GetAD_Client_ID());

            //	Add Parameter - Selection=Y
            MPInstancePara para = new MPInstancePara(instance, 10);
            para.setParameter("Selection", "Y");
            if (!para.Save())
            {
                String msg = "No Selection Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
            }
            //	Add Parameter - M_Warehouse_ID=x
            para = new MPInstancePara(instance, 20);
            para.setParameter("M_Warehouse_ID", Util.GetValueOfInt(M_Warehouse_ID));
            if (!para.Save())
            {
                String msg = "No DocAction Parameter added";  //  not translated
                lblStatusInfo = msg.ToString();
            }

            //	Execute Process
            ProcessCtl worker = new ProcessCtl(ctx, null, pi, trx);
            worker.Run();
            GenerateShipments_complete(ctx, pi, "");
            return "";
        }

        //bool waiting = false;

        private void GenerateInvoice_complete(Ctx ctx, ProcessInfo pi, string whereClause)
        {
            ProcessInfoUtil.SetLogFromDB(pi);
            StringBuilder iText = new StringBuilder();
            iText.Append("<b>").Append(pi.GetSummary())
                .Append("</b><br>(")
                .Append(Msg.GetMsg(ctx, "InvGenerateInfo"))
                //Invoices are generated depending on the Invoicing Rule selection in the Order
                .Append(")<br>")
                .Append(pi.GetLogInfo(true));
            //info.setText(iText.toString());
            DocumentText = iText.ToString();

            //	Reset Selection
            String sql = "UPDATE C_Order SET IsSelected = 'N' WHERE " + whereClause;
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
            //log.Config("Reset=" + no);
            //	Get results
            int[] ids = pi.GetIDs();
            if (ids == null || ids.Length == 0)
            {
                // waiting = true;
                return;
            }
            // waiting = true;
        }

        private void GenerateShipments_complete(Ctx ctx, ProcessInfo pi, string whereClause)
        {
            //  Switch Tabs
            ProcessInfoUtil.SetLogFromDB(pi);
            StringBuilder iText = new StringBuilder();
            iText.Append("<b>").Append(pi.GetSummary())
                .Append("</b><br>(")
                .Append(Msg.GetMsg(ctx, "InOutGenerateInfo"))
                //  Shipments are generated depending on the Delivery Rule selection in the Order
                .Append(")<br>")
                .Append(pi.GetLogInfo(true));

            DocumentText = iText.ToString();

            //	Reset Selection
            String sql = "UPDATE C_Order SET IsSelected = 'N' WHERE " + whereClause;
            int no = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, null));
            //	Get results
            int[] ids = pi.GetIDs();
            if (ids == null || ids.Length == 0)
            {
                // waiting = true;
                return;
            }
            // waiting = true;
        }

        //public void LockUI(ProcessInfo pi)
        //{

        //}

        //public void UnlockUI(ProcessInfo pi)
        //{

        //    if (pi.GetClassName().Contains("InOutGenerate"))
        //    {
        //        GenerateShipments_complete(pi, "");
        //    }
        //    else
        //    {
        //        GenerateInvoice_complete(pi, "");
        //    }
        //}


        //public bool IsUILocked()
        //{
        //    return true;
        //}

        //public void ExecuteASync(ProcessInfo pi)
        //{
        //    throw new NotImplementedException();
        //}
        #endregion

        #region Match PO

        public bool CreateMatchRecord(Ctx ctx, bool invoice, int M_InOutLine_ID, int Line_ID, Decimal qty)
        {
            Trx trx = Trx.GetTrx("MatchPO");

            if (qty.CompareTo(Env.ZERO) == 0)
                return true;
            //log.Fine("IsInvoice=" + invoice
            //    + ", M_InOutLine_ID=" + M_InOutLine_ID + ", Line_ID=" + Line_ID
            //    + ", Qty=" + qty);

            bool success = false;

            try
            {
                MInOutLine sLine = new MInOutLine(ctx, M_InOutLine_ID, trx);
                if (invoice)	//	Shipment - Invoice
                {
                    //	Update Invoice Line
                    MInvoiceLine iLine = new MInvoiceLine(ctx, Line_ID, trx);
                    iLine.SetM_InOutLine_ID(M_InOutLine_ID);
                    if (sLine.GetC_OrderLine_ID() != 0)
                        iLine.SetC_OrderLine_ID(sLine.GetC_OrderLine_ID());
                    iLine.Save();
                    //	Create Shipment - Invoice Link
                    if (iLine.GetM_Product_ID() != 0)
                    {
                        try
                        {
                            MMatchInv match = new MMatchInv(iLine, null, qty);
                            match.SetM_InOutLine_ID(M_InOutLine_ID);
                            if (match.Save())
                                success = true;
                            else
                            {
                                //VLogger.Get log.Log(Level.SEVERE, "Inv Match not created: " + match);
                            }
                        }
                        catch (Exception)
                        {
                            //log.Log(Level.SEVERE, "Inv Match not created: " + e.Message);
                        }
                    }
                    else
                        success = true;
                    //	Create PO - Invoice Link = corrects PO
                    if (iLine.GetC_OrderLine_ID() != 0 && iLine.GetM_Product_ID() != 0)
                    {
                        try
                        {
                            MMatchPO matchPO = MMatchPO.Create(iLine, sLine, null, qty);
                            matchPO.SetC_InvoiceLine_ID(iLine);
                            matchPO.SetM_InOutLine_ID(M_InOutLine_ID);
                            if (!matchPO.Save())
                            {
                                //   log.Log(Level.SEVERE, "PO(Inv) Match not created: " + matchPO);
                            }
                        }
                        catch (Exception)
                        {
                            // log.Log(Level.SEVERE, "PO(Inv) Match not created: " + e.Message);
                        }
                    }
                }
                else	//	Shipment - Order
                {
                    //	Update Shipment Line
                    sLine.SetC_OrderLine_ID(Line_ID);
                    sLine.Save();
                    //	Update Order Line
                    MOrderLine oLine = new MOrderLine(ctx, Line_ID, trx);
                    if (oLine.Get_ID() != 0)	//	other in MInOut.completeIt
                    {
                        //oLine.SetQtyReserved(oLine.GetQtyReserved().subtract(qty));
                        oLine.SetQtyReserved(Decimal.Subtract(oLine.GetQtyReserved(), qty));
                        if (!oLine.Save())
                        {
                            //   log.Severe("QtyReserved not updated - C_OrderLine_ID=" + Line_ID);
                        }
                    }


                    //	Create PO - Shipment Link
                    if (sLine.GetM_Product_ID() != 0)
                    {
                        MMatchPO match = new MMatchPO(sLine, null, qty);
                        if (!match.Save())
                        {
                            //   log.Log(Level.SEVERE, "PO Match not created: " + match);
                        }
                        else
                        {
                            success = true;
                            //	Correct Ordered Qty for Stocked Products (see MOrder.reserveStock / MInOut.processIt)
                            if (sLine.GetProduct() != null && sLine.GetProduct().IsStocked())
                                success = MStorage.Add(ctx, sLine.GetM_Warehouse_ID(),
                                    sLine.GetM_Locator_ID(),
                                    sLine.GetM_Product_ID(),
                                    sLine.GetM_AttributeSetInstance_ID(), oLine.GetM_AttributeSetInstance_ID(),
                                    null, null, Decimal.Negate(qty), null);
                        }
                    }
                    else
                    {
                        success = true;
                    }
                }
                if (success)
                {
                    trx.Commit();
                }
                else
                {
                    trx.Rollback();
                }
            }
            catch (Exception)
            {
                success = false;
                trx.Rollback();
            }
            finally
            {
                trx.Close();
            }
            return success;
        }
        #endregion

        #region Account Viewer
        //  Display Info
        //Display Qty			
        public bool displayQty = false;
        //Display Source Surrency
        public bool displaySourceAmt = false;
        //Display Document info	
        public bool displayDocumentInfo = false;
        //
        public String sortBy1 = "";
        public String sortBy2 = "";
        public String sortBy3 = "";
        public String sortBy4 = "";
        //
        public bool group1 = false;
        public bool group2 = false;
        public bool group3 = false;
        public bool group4 = false;

        // Leasing Columns		
        private int _leadingColumns = 0;
        //UserElement1 Reference	
        private String _ref1 = null;
        //UserElement2 Reference	
        private String _ref2 = null;
        public String PostingType = "";
        public MAcctSchema[] ASchemas = null;
        public MAcctSchema ASchema = null;


        public AccountViewClass GetDataQuery(Ctx ctx, int AD_Client_ID, string whereClause, string orderClause, bool gr1, bool gr2, bool gr3, bool gr4, String sort1, String sort2, String sort3, String sort4)
        {
            group1 = gr1; group2 = gr2; group3 = gr3; group4 = gr4;
            sortBy1 = sort1; sortBy2 = sort2; sortBy3 = sort3; sortBy4 = sort4;
            ASchemas = MAcctSchema.GetClientAcctSchema(ctx, AD_Client_ID);
            ASchema = ASchemas[0];

            RModel rm = GetRModel(ctx);

            //  Groups
            if (group1 && sortBy1.Length > 0)
            {
                rm.SetGroup(sortBy1);
            }
            if (group2 && sortBy2.Length > 0)
            {
                rm.SetGroup(sortBy2);
            }
            if (group3 && sortBy3.Length > 0)
            {
                rm.SetGroup(sortBy3);
            }
            if (group4 && sortBy4.Length > 0)
            {
                rm.SetGroup(sortBy4);
            }

            //  Totals
            rm.SetFunction("AmtAcctDr", RModel.FUNCTION_SUM);
            rm.SetFunction("AmtAcctCr", RModel.FUNCTION_SUM);

            rm.Query(ctx, whereClause.ToString(), orderClause.ToString());

            //return rm;

            AccountViewClass obj = new AccountViewClass();
            int col = rm.GetColumnCount();
            var arrList = new List<string>();
            for (int i = 0; i < col; i++)
            {
                RColumn rc = rm.GetRColumn(i);
                arrList.Add(rc.GetColHeader());
            }
            obj.Columns = arrList;
            obj.Data = rm._data.rows;
            return obj;
        }

        /// <summary>
        /// fillter data and column from RModel;
        /// </summary>
        /// <param name="reportModel"></param>
        public List<string> SetModel(RModel reportModel)
        {
            int col = reportModel.GetColumnCount();
            var arrList = new List<string>();

            for (int i = 0; i < col; i++)
            {
                RColumn rc = reportModel.GetRColumn(i);
                arrList.Add(rc.GetColHeader());
            }


            return arrList;
        }


        /// <summary>
        /// Create Report Model (Columns)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>Report Model</returns>
        public RModel GetRModel(Ctx ctx)
        {
            RModel rm = new RModel("Fact_Acct");
            //  Add Key (Lookups)
            List<String> keys = CreateKeyColumns();
            int max = _leadingColumns;
            if (max == 0)
            {
                max = keys.Count;
            }
            for (int i = 0; i < max; i++)
            {
                String column = (String)keys[i];
                if (column != null && column.StartsWith("Date"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.Date));
                }
                else if (column != null && column.EndsWith("_ID"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir));
                }
            }
            //  Main Info
            rm.AddColumn(new RColumn(ctx, "AmtAcctDr", DisplayType.Amount));
            rm.AddColumn(new RColumn(ctx, "AmtAcctCr", DisplayType.Amount));
            if (displaySourceAmt)
            {
                if (!keys.Contains("DateTrx"))
                {
                    rm.AddColumn(new RColumn(ctx, "DateTrx", DisplayType.Date));
                }
                rm.AddColumn(new RColumn(ctx, "C_Currency_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "AmtSourceDr", DisplayType.Amount));
                rm.AddColumn(new RColumn(ctx, "AmtSourceCr", DisplayType.Amount));
                rm.AddColumn(new RColumn(ctx, "Rate", DisplayType.Amount,
                    "CASE WHEN (AmtSourceDr + AmtSourceCr) = 0 THEN 0"
                    + " ELSE  Round((AmtAcctDr + AmtAcctCr) / (AmtSourceDr + AmtSourceCr),6) END"));
            }
            //	Remaining Keys
            for (int i = max; i < keys.Count; i++)
            {
                String column = (String)keys[i];
                if (column != null && column.StartsWith("Date"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.Date));
                }
                else if (column.StartsWith("UserElement"))
                {
                    if (column.IndexOf("1") != -1)
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref1));
                    }
                    else
                    {
                        rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir, null, 0, _ref2));
                    }
                }
                else if (column != null && column.EndsWith("_ID"))
                {
                    rm.AddColumn(new RColumn(ctx, column, DisplayType.TableDir));
                }
            }
            //	Info
            if (!keys.Contains("DateAcct"))
            {
                rm.AddColumn(new RColumn(ctx, "DateAcct", DisplayType.Date));
            }
            if (!keys.Contains("C_Period_ID"))
            {
                rm.AddColumn(new RColumn(ctx, "C_Period_ID", DisplayType.TableDir));
            }
            if (displayQty)
            {
                rm.AddColumn(new RColumn(ctx, "C_UOM_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Qty", DisplayType.Quantity));
            }
            if (displayDocumentInfo)
            {
                rm.AddColumn(new RColumn(ctx, "AD_Table_ID", DisplayType.TableDir));
                rm.AddColumn(new RColumn(ctx, "Record_ID", DisplayType.ID));
                rm.AddColumn(new RColumn(ctx, "Description", DisplayType.String));
            }
            if (PostingType == null || PostingType.Length == 0)
            {
                rm.AddColumn(new RColumn(ctx, "PostingType", DisplayType.List,
                    MFactAcct.POSTINGTYPE_AD_Reference_ID));
            }
            return rm;
        }

        /// <summary>
        /// Create the key columns in sequence
        /// </summary>
        /// <returns>List of Key Columns</returns>
        private List<String> CreateKeyColumns()
        {
            List<String> columns = new List<String>();
            _leadingColumns = 0;
            //  Sorting Fields
            columns.Add(sortBy1);               //  may add ""
            if (!columns.Contains(sortBy2))
            {
                columns.Add(sortBy2);
            }
            if (!columns.Contains(sortBy3))
            {
                columns.Add(sortBy3);
            }
            if (!columns.Contains(sortBy4))
            {
                columns.Add(sortBy4);
            }


            //  Add Account Segments
            MAcctSchemaElement[] elements = ASchema.GetAcctSchemaElements();
            for (int i = 0; i < elements.Length; i++)
            {
                if (_leadingColumns == 0 && columns.Contains("AD_Org_ID") && columns.Contains("Account_ID"))
                {
                    _leadingColumns = columns.Count;
                }
                //
                MAcctSchemaElement ase = elements[i];
                String columnName = ase.GetColumnName();
                if (columnName.StartsWith("UserElement"))
                {
                    if (columnName.IndexOf("1") != -1)
                    {
                        _ref1 = ase.GetDisplayColumnName();
                    }
                    else
                    {
                        _ref2 = ase.GetDisplayColumnName();
                    }
                }
                if (!columns.Contains(columnName))
                {
                    columns.Add(columnName);

                }
            }
            if (_leadingColumns == 0 && columns.Contains("AD_Org_ID") && columns.Contains("Account_ID"))
            {
                _leadingColumns = columns.Count;
            }
            return columns;
        }

        #endregion

        #region Archive Viewer

        public bool UpdateArchive(Ctx ctx, string name, string des, string help, int archiveId)
        {
            MArchive ar = new MArchive(ctx, archiveId, null);//  m_archives[m_index];
            ar.SetName(name);
            ar.SetDescription(des);
            ar.SetHelp(help);
            if (ar.Save())
            {
                return true;
            }
            return false;
        }

        public string DownloadPdf(Ctx ctx, int archiveId)
        {
            MArchive ar = new MArchive(ctx, archiveId, null);//  m_archives[m_index];
            if (ar != null)
            {
                return Convert.ToBase64String(ar.GetBinaryData());
            }
            return null;
        }


        #endregion
    }

    public class AccountViewClass
    {
        public List<String> Columns { get; set; }
        public List<List<object>> Data { get; set; }
    }
}