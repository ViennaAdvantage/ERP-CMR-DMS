using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class InfoProductModel
    {
        private static InfoColumn[] s_productLayout = null;
        // Header for Price List Version 	
        private static String s_headerPriceList = "";
        // Header for Warehouse 		
        private static String s_headerWarehouse = "";
        private static int INDEX_NAME = 0;
        private static int INDEX_PATTRIBUTE = 0;
        bool isConferm = false;

        public InfoColumn[] GetInfoColumns(VAdvantage.Utility.Ctx ctx)
        {
            if (s_productLayout != null)
                return s_productLayout;
            //
            s_headerPriceList = Msg.Translate(ctx, "M_PriceList_Version_ID");
            s_headerWarehouse = Msg.Translate(ctx, "M_Warehouse_ID");
            //  Euro 13
            MClient client = MClient.Get(ctx);
            if ("FRIE".Equals(client.GetValue()))
            {
                InfoColumn[] frieLayout =
            {
				new InfoColumn(Msg.Translate(ctx,"M_Product_ID"),"M_Product_ID", true, "p.M_Product_ID",DisplayType.ID).Seq(10),
				new InfoColumn(Msg.Translate(ctx, "Name"), "Name", true, "p.Name",DisplayType.String).Seq(20),
                new InfoColumn(Msg.Translate(ctx,"QtyEntered"), "QtyEntered", false, "0 as QtyEntered" , DisplayType.Quantity).Seq(30), 
				new InfoColumn(Msg.Translate(ctx, "QtyAvailable"), "QtyAvailable",true,
					"bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable", DisplayType.Quantity).Seq(40),
				new InfoColumn(s_headerPriceList, "M_PriceList_Version_ID",true, "plv.Name", DisplayType.Amount).Seq(50),
                new InfoColumn(s_headerWarehouse, "M_Warehouse_ID",true, "w.Name", DisplayType.String).Seq(60),
				new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList",true,
					"bomPriceList(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceList",  DisplayType.Amount).Seq(70),
				new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd",true,
					"bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceStd", DisplayType.Amount).Seq(80),
				new InfoColumn("Einzel MWSt", "",true,
					"pr.PriceStd * 1.19", DisplayType.Amount).Seq(90),
				new InfoColumn("Einzel kompl", "",true,
					"(pr.PriceStd+13) * 1.19", DisplayType.Amount).Seq(100),
				new InfoColumn("Satz kompl", "", true,
					"((pr.PriceStd+13) * 1.19) * 4", DisplayType.Amount).Seq(110),
				new InfoColumn(Msg.Translate(ctx, "QtyOnHand"), "QtyOnHand",true,
					"bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOnHand", DisplayType.Quantity).Seq(120),
				new InfoColumn(Msg.Translate(ctx, "QtyReserved"), "QtyReserved",true,
					"bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyReserved", DisplayType.Quantity).Seq(130),
				new InfoColumn(Msg.Translate(ctx, "QtyOrdered"), "QtyOrdered",true,
					"bomQtyOrdered(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOrdered", DisplayType.Quantity).Seq(140),
				new InfoColumn(Msg.Translate(ctx, "Discontinued").Substring(0, 1), "Discontinued",true,
					"p.Discontinued", DisplayType.YesNo).Seq(150),
				new InfoColumn(Msg.Translate(ctx, "SalesMargin"), "SalesMargin",true,
					"bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID)-bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS Margin", DisplayType.Amount).Seq(160),
				new InfoColumn(Msg.Translate(ctx, "PriceLimit"), "PriceLimit",true,
					"bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceLimit", DisplayType.Amount).Seq(170),
				new InfoColumn(Msg.Translate(ctx, "IsInstanceAttribute"), "IsInstanceAttribute",true,
					"pa.IsInstanceAttribute", DisplayType.YesNo).Seq(180),
                new InfoColumn(Msg.Translate(ctx,"GuranteeDays"),"GuranteeDays",true, "Sysdate+p.GuaranteeDays as GuranteeDays",DisplayType.Date).Seq(190)
                    //new InfoColumn(Msg.Translate(ctx, "Quantity"), "0 as Quantity" , typeof(Boolean)).Seq(180) 
                   
			};
                INDEX_NAME = 1;
                INDEX_PATTRIBUTE = frieLayout.Length - 1;	//	last item
                s_productLayout = frieLayout;
                return s_productLayout;
            }
            if (s_productLayout == null)
            {
                List<InfoColumn> list = new List<InfoColumn>();
                list.Add(new InfoColumn(Msg.Translate(ctx, "M_Product_ID"),"M_Product_ID",true, "p.M_Product_ID", DisplayType.ID).Seq(10));
                //list.Add(new InfoColumn(Msg.Translate(ctx, "SelectProduct"),"SelectProduct",true, "'N'", DisplayType.YesNo).Seq(20));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Discontinued"),"Discontinued",true, "p.Discontinued", DisplayType.YesNo).Seq(30));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Value"),"Value",true, "p.Value", DisplayType.String).Seq(40));
                list.Add(new InfoColumn(Msg.Translate(ctx, "Name"),"Name",true, "p.Name", DisplayType.String).Seq(50));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyEntered"), "QtyEntered", false, "0 as QtyEntered", DisplayType.Quantity).Seq(80));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyAvailable"),"QtyAvailable",true,
                    "bomQtyAvailable(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyAvailable", DisplayType.Quantity).Seq(90));
                //}
                list.Add(new InfoColumn(s_headerPriceList, "PriceListVersion", true, "plv.Name as PriceListVersion", DisplayType.String).Seq(100));
                list.Add(new InfoColumn(s_headerWarehouse, "Warehouse", true, "w.Name as Warehouse", DisplayType.String).Seq(110));
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("VAPRC_", out mInfo))
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"),"PriceList",true,
                        "bomPriceListUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceList", DisplayType.Amount).Seq(120));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                        "bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                    }
                    else
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                            "bomPriceListAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS PriceList", DisplayType.Amount).Seq(120));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"), "PriceStd", true,
                            "bomPriceStdAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                    }
                }
                else
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceList"), "PriceList", true,
                        "bomPriceList(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceList", DisplayType.Amount).Seq(120));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceStd"),"PriceStd",true,
                        "bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceStd", DisplayType.Amount).Seq(130));
                }

                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyOnHand"),"QtyOnHand",true,
                    "bomQtyOnHand(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOnHand", DisplayType.Quantity).Seq(140));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyReserved"),"QtyReserved",true,
                    "bomQtyReserved(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyReserved", DisplayType.Quantity).Seq(150));
                list.Add(new InfoColumn(Msg.Translate(ctx, "QtyOrdered"),"QtyOrdered",true,
                    "bomQtyOrdered(p.M_Product_ID,w.M_Warehouse_ID,0) AS QtyOrdered", DisplayType.Quantity).Seq(160));
                //}
                if (isConferm) //IsUnconfirmed())
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "QtyUnconfirmed"),"QtyUnconfirmed",true,
                        "(SELECT SUM(c.TargetQty) FROM M_InOutLineConfirm c INNER JOIN M_InOutLine il ON (c.M_InOutLine_ID=il.M_InOutLine_ID) INNER JOIN M_InOut i ON (il.M_InOut_ID=i.M_InOut_ID) WHERE c.Processed='N' AND i.M_Warehouse_ID=w.M_Warehouse_ID AND il.M_Product_ID=p.M_Product_ID) AS QtyUnconfirmed",
                        DisplayType.Quantity).Seq(170));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "QtyUnconfirmedMove"),"QtyUnconfirmedMove",true,
                        "(SELECT SUM(c.TargetQty) FROM M_MovementLineConfirm c INNER JOIN M_MovementLine ml ON (c.M_MovementLine_ID=ml.M_MovementLine_ID) INNER JOIN M_Locator l ON (ml.M_LocatorTo_ID=l.M_Locator_ID) WHERE c.Processed='N' AND l.M_Warehouse_ID=w.M_Warehouse_ID AND ml.M_Product_ID=p.M_Product_ID) AS QtyUnconfirmedMove",
                        DisplayType.Quantity).Seq(180));
                }
                if (Env.HasModulePrefix("VAPRC_", out mInfo))
                {
                    Tuple<String, String, String> aInfo = null;
                    if (Env.HasModulePrefix("ED011_", out aInfo))
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"),"SalesMargin",true,
                            "bomPriceStdUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID)-bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS Margin",
                            DisplayType.Amount).Seq(190));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"),"PriceLimit",true,
                            "bomPriceLimitUom(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID,pr.C_UOM_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                    }
                    else
                    {
                        list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"),"SalesMargin",true,
                            "bomPriceStdAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID)-bomPriceLimitAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS Margin",
                            DisplayType.Amount).Seq(190));
                        list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"),"PriceLimit",true,
                            "bomPriceLimitAttr(p.M_Product_ID, pr.M_PriceList_Version_ID,pr.M_AttriButeSetInstance_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                    }
                }
                else
                {
                    list.Add(new InfoColumn(Msg.Translate(ctx, "SalesMargin"),"SalesMargin",true,
                            "bomPriceStd(p.M_Product_ID, pr.M_PriceList_Version_ID)-bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS Margin", DisplayType.Amount).Seq(190));
                    list.Add(new InfoColumn(Msg.Translate(ctx, "PriceLimit"),"PriceLimit",true,
                        "bomPriceLimit(p.M_Product_ID, pr.M_PriceList_Version_ID) AS PriceLimit", DisplayType.Amount).Seq(200));
                }
                list.Add(new InfoColumn(Msg.Translate(ctx, "IsInstanceAttribute"), "IsInstanceAttribute",true, "pa.IsInstanceAttribute", DisplayType.YesNo).Seq(210));
                list.Add(new InfoColumn(Msg.Translate(ctx, "GuranteeDays"), "GuranteeDays", true, "Sysdate+p.GuaranteeDays as GuranteeDays", DisplayType.Date).Seq(220));

                s_productLayout = new InfoColumn[list.Count];
                s_productLayout = list.ToArray();
                INDEX_NAME = 3;
                INDEX_PATTRIBUTE = s_productLayout.Length - 1;	//	last item
            }
            return s_productLayout;
        }

        public List<DataObject> GetData(string sql, string tableName, VAdvantage.Utility.Ctx ctx)
        {
            try
            {
                sql = sql.Replace('●', '%');
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                                MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                DataSet data = DBase.DB.ExecuteDataset(sql,null,null);
                if (data == null)
                {
                    return null;
                }

                List<DataObject> dyndata = new List<DataObject>();
                DataObject item = null;
                List<object> values = null;
                for (int i = 0; i < data.Tables[0].Columns.Count; i++)  //columns
                {
                    item = new DataObject();

                    item.ColumnName = data.Tables[0].Columns[i].ColumnName;
                    values = new List<object>();
                    for (int j = 0; j < data.Tables[0].Rows.Count; j++)  //rows
                    {

                        values.Add(data.Tables[0].Rows[j][data.Tables[0].Columns[i].ColumnName]);
                    }
                    item.Values = values;
                    item.RowCount = data.Tables[0].Rows.Count;
                    dyndata.Add(item);
                }
                return dyndata;
            }
            catch
            {
                return null;
            }
        }

        public bool SetProductQty(int recordID, string keyColName, List<string> product, List<string> attribute, List<string> qty, List<string> qtybook, List<string> oline_ID, int ordID, List<string> locID, int lineID, VAdvantage.Utility.Ctx ctx)
        {
            if (keyColName.ToUpper().Trim() == "C_ORDER_ID")
            {
                MOrder ord = new MOrder(ctx, recordID, null);
                for (int i = 0; i < product.Count; i++)
                {
                    MOrderLine oline = new MOrderLine(ctx, lineID, null);
                    oline.SetAD_Client_ID(ord.GetAD_Client_ID());
                    oline.SetAD_Org_ID(ord.GetAD_Org_ID());
                    oline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                    oline.SetQty(Util.GetValueOfDecimal(qty[i]));
                    oline.SetC_Order_ID(recordID);
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        oline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                    }
                    if (!ord.IsSOTrx())
                    {
                        MProduct pro = new MProduct(ctx, oline.GetM_Product_ID(), null);
                        String qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + oline.GetM_Product_ID() + " AND vdr.C_BPartner_ID = " + ord.GetC_BPartner_ID();
                        int uom = Util.GetValueOfInt(DB.ExecuteScalar(qryUom));
                        if (pro.GetC_UOM_ID() != 0)
                        {
                            if (pro.GetC_UOM_ID() != uom && uom != 0)
                            {
                                decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + oline.GetM_Product_ID() + " AND IsActive='Y'"));
                                if (Res > 0)
                                {
                                    oline.SetQtyEntered(oline.GetQtyEntered() * Res);
                                    //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                                }
                                else
                                {
                                    decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                                    if (res > 0)
                                    {
                                        oline.SetQtyEntered(oline.GetQtyEntered() * res);
                                        //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
                                    }
                                }
                                oline.SetC_UOM_ID(uom);
                            }
                            else
                            {
                                oline.SetC_UOM_ID(pro.GetC_UOM_ID());
                            }
                        }
                    }
                    if (!oline.Save())
                    {

                    }
                }
            }
            else if (keyColName.ToUpper().Trim() == "C_INVOICE_ID")
            {
                MInvoice inv = new MInvoice(ctx, recordID, null);
                for (int i = 0; i < product.Count; i++)
                {
                    MInvoiceLine invline = new MInvoiceLine(ctx, lineID, null);
                    invline.SetAD_Client_ID(inv.GetAD_Client_ID());
                    invline.SetAD_Org_ID(inv.GetAD_Org_ID());
                    invline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                    invline.SetQty(Util.GetValueOfDecimal(qty[i]));
                    invline.SetC_Invoice_ID(recordID);
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        invline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                    }
                    if (!inv.IsSOTrx())
                    {
                        MProduct pro = new MProduct(ctx, invline.GetM_Product_ID(), null);
                        String qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + invline.GetM_Product_ID() + " AND vdr.C_BPartner_ID = " + inv.GetC_BPartner_ID();
                        int uom = Util.GetValueOfInt(DB.ExecuteScalar(qryUom));
                        if (pro.GetC_UOM_ID() != 0)
                        {
                            if (pro.GetC_UOM_ID() != uom && uom != 0)
                            {
                                decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + invline.GetM_Product_ID() + " AND IsActive='Y'"));
                                if (Res > 0)
                                {
                                    invline.SetQtyEntered(invline.GetQtyEntered() * Res);
                                    //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                                }
                                else
                                {
                                    decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                                    if (res > 0)
                                    {
                                        invline.SetQtyEntered(invline.GetQtyEntered() * res);
                                        //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
                                    }
                                }
                                invline.SetC_UOM_ID(uom);
                            }
                            else
                            {
                                invline.SetC_UOM_ID(pro.GetC_UOM_ID());
                            }
                        }
                    }
                    if (!invline.Save())
                    {

                    }
                }
            }
            else if (keyColName.ToUpper().Trim() == "M_INOUT_ID")
            {
                MInOut inv = new MInOut(ctx, recordID, null);
                if (ordID > 0)
                {
                    inv.SetC_Order_ID(ordID);
                }
                if (inv.Save())
                {
                    for (int i = 0; i < product.Count; i++)
                    {
                        MInOutLine ioline = new MInOutLine(ctx, lineID, null);
                        ioline.SetAD_Client_ID(inv.GetAD_Client_ID());
                        ioline.SetAD_Org_ID(inv.GetAD_Org_ID());
                        ioline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                        ioline.SetQty(Util.GetValueOfDecimal(qty[i]));
                        ioline.SetM_InOut_ID(recordID);
                        ioline.SetC_OrderLine_ID(Util.GetValueOfInt(oline_ID[i]));
                        ioline.SetM_Locator_ID(Util.GetValueOfInt(locID[i]));
                        if (Util.GetValueOfInt(attribute[i]) != 0)
                        {
                            ioline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                        }
                        if (!inv.IsSOTrx())
                        {
                            MProduct pro = new MProduct(ctx, ioline.GetM_Product_ID(), null);
                            String qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + ioline.GetM_Product_ID() + " AND vdr.C_BPartner_ID = " + inv.GetC_BPartner_ID();
                            int uom = Util.GetValueOfInt(DB.ExecuteScalar(qryUom));
                            if (pro.GetC_UOM_ID() != 0)
                            {
                                if (pro.GetC_UOM_ID() != uom && uom != 0)
                                {
                                    decimal? Res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + ioline.GetM_Product_ID() + " AND IsActive='Y'"));
                                    if (Res > 0)
                                    {
                                        ioline.SetQtyEntered(ioline.GetQtyEntered() * Res);
                                        //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                                    }
                                    else
                                    {
                                        decimal? res = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + pro.GetC_UOM_ID() + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                                        if (res > 0)
                                        {
                                            ioline.SetQtyEntered(ioline.GetQtyEntered() * res);
                                            //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
                                        }
                                    }
                                    ioline.SetC_UOM_ID(uom);
                                }
                                else
                                {
                                    ioline.SetC_UOM_ID(pro.GetC_UOM_ID());
                                }
                            }
                        }
                        if (!ioline.Save())
                        {

                        }
                    }
                }
            }
            else if (keyColName.ToUpper().Trim() == "M_PACKAGE_ID")
            {
                MPackage pkg = new MPackage(ctx, recordID, null);
                for (int i = 0; i < product.Count; i++)
                {
                    MPackageLine mline = new MPackageLine(ctx, lineID, null);
                    mline.SetAD_Client_ID(pkg.GetAD_Client_ID());
                    mline.SetAD_Org_ID(pkg.GetAD_Org_ID());
                    mline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                    mline.SetQty(Util.GetValueOfDecimal(qty[i]));
                    if (Util.GetValueOfInt(oline_ID[i]) > 0)
                    {
                        mline.SetM_MovementLine_ID(Util.GetValueOfInt(oline_ID[i]));
                        MMovementLine mov = new MMovementLine(ctx, Util.GetValueOfInt(oline_ID[i]), null);
                        mline.SetDTD001_TotalQty(mov.GetMovementQty());
                    }
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        mline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                    }
                    mline.SetM_Package_ID(recordID);
                    if (!mline.Save())
                    {

                    }
                }
            }
            else if (keyColName.ToUpper().Trim() == "M_INVENTORY_ID")
            {
                MInventory inv = new MInventory(ctx, recordID, null);
                for (int i = 0; i < product.Count; i++)
                {
                    MInventoryLine invline = new MInventoryLine(ctx, lineID, null);
                    invline.SetAD_Client_ID(inv.GetAD_Client_ID());
                    invline.SetAD_Org_ID(inv.GetAD_Org_ID());
                    invline.SetM_Locator_ID(Util.GetValueOfInt(locID[i]));
                    invline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                    invline.SetQtyCount(Util.GetValueOfDecimal(qty[i]));
                    invline.SetQtyBook(Util.GetValueOfDecimal(qtybook[i]));
                    invline.SetAsOnDateCount(Util.GetValueOfDecimal(qty[i]));
                    invline.SetOpeningStock(Util.GetValueOfDecimal(qtybook[i]));
                    invline.SetM_Inventory_ID(recordID);
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        invline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                    }
                    else
                    {
                        invline.SetM_AttributeSetInstance_ID(0);
                    }
                    if (!invline.Save())
                    {

                    }
                }
            }
            return true;
        }

        public bool SetProductQty1(int recordID, string keyColName, List<string> product, List<string> attribute, List<string> qty, List<string> oline_ID, List<string> locID, int LocToID, List<string> AssetID, int lineID, VAdvantage.Utility.Ctx ctx)
        {            
            if (keyColName.ToUpper().Trim() == "M_MOVEMENT_ID")
            {
                MMovement inv = new MMovement(ctx, recordID, null);
                for (int i = 0; i < product.Count; i++)
                {
                    MMovementLine invline = new MMovementLine(ctx, lineID, null);
                    invline.SetAD_Client_ID(inv.GetAD_Client_ID());
                    invline.SetAD_Org_ID(inv.GetAD_Org_ID());
                    invline.SetM_Locator_ID(Util.GetValueOfInt(locID[i]));
                    invline.SetM_LocatorTo_ID(LocToID);
                    invline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                    invline.SetMovementQty(Util.GetValueOfDecimal(qty[i]));
                    invline.SetM_Movement_ID(recordID);
                    if (Util.GetValueOfInt(AssetID[i]) > 0)
                    {
                        invline.SetA_Asset_ID(Util.GetValueOfInt(AssetID[i]));
                    }
                    if (Util.GetValueOfInt(oline_ID[i]) > 0)
                    {
                        invline.SetM_RequisitionLine_ID(Util.GetValueOfInt(oline_ID[i]));
                    }
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        invline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                    }
                    if (!invline.Save())
                    {

                    }
                }
            }
            else if (keyColName.ToUpper().Trim() == "M_INVENTORY_ID")
            {
                MInventory inv = new MInventory(ctx, recordID, null);
                for (int i = 0; i < product.Count; i++)
                {
                    MInventoryLine invline = new MInventoryLine(ctx, lineID, null);
                    invline.SetAD_Client_ID(inv.GetAD_Client_ID());
                    invline.SetAD_Org_ID(inv.GetAD_Org_ID());                    
                    invline.SetM_Product_ID(Util.GetValueOfInt(product[i]));
                    invline.SetQtyInternalUse(Util.GetValueOfDecimal(qty[i]));
                    invline.SetC_Charge_ID(Util.GetValueOfInt(AssetID[i]));
                    invline.SetM_Inventory_ID(recordID);
                    invline.SetIsInternalUse(true);
                    invline.SetM_RequisitionLine_ID(Util.GetValueOfInt(oline_ID[i]));
                    if (Util.GetValueOfInt(attribute[i]) != 0)
                    {
                        invline.SetM_AttributeSetInstance_ID(Util.GetValueOfInt(attribute[i]));
                    }
                    else
                    {
                        invline.SetM_AttributeSetInstance_ID(0);
                    }
                    if (!invline.Save())
                    {

                    }
                }
            }
            return true;
        }

        public KeyNamePair GetAttribute(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int M_Product_ID = 0;
            string attributeNo = "", isLot = "", IsSerNo ="", IsGuaranteeDate = "", RefNo = "" ;
            DateTime? expiryDate = null;
            if(paramValue.Length > 0 )
            {
                M_Product_ID = Util.GetValueOfInt(paramValue[0]);
                attributeNo = Util.GetValueOfString(paramValue[1]);
                isLot = Util.GetValueOfString(paramValue[2]);
                IsSerNo = Util.GetValueOfString(paramValue[3]);
                IsGuaranteeDate = Util.GetValueOfString(paramValue[4]);
                expiryDate = Util.GetValueOfDateTime(paramValue[5]);
                RefNo = Util.GetValueOfString(paramValue[6]);
            }
            string qry = "";
            int attrID = 0;
            string name = "";
            KeyNamePair attribute = null;
            StringBuilder sql = new StringBuilder();
            MAttributeSetInstance _mast = MAttributeSetInstance.Get(Env.GetCtx(), 0, M_Product_ID);            
            if (!string.IsNullOrEmpty(attributeNo))
            {
                qry = "SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE M_Product_ID = " + M_Product_ID + "AND UPC = '" + attributeNo + "'";
                attrID = Util.GetValueOfInt(DB.ExecuteScalar(qry));
                if (attrID == 0)
                {
                    if (isLot == "Y")
                    {
                        _mast.SetLot(attributeNo);
                        sql.Append("UPPER(Lot) = " + attributeNo.ToUpper());
                    }
                    else if (IsSerNo == "Y")
                    {
                        _mast.SetSerNo(attributeNo);
                        sql.Append(" UPPER(SerNo) = " + attributeNo.ToUpper());
                    }
                    _mast.SetDescription(attributeNo);
                    if (IsGuaranteeDate == "Y")
                    {
                        if (sql.Length > 0)
                        {
                            sql.Append(" AND GuaranteeDate = " + GlobalVariable.TO_DATE(expiryDate, true));
                        }
                        else
                        {
                            sql.Append(" GuaranteeDate = " + GlobalVariable.TO_DATE(expiryDate, true));
                        }
                        _mast.SetGuaranteeDate(expiryDate);
                        if (!String.IsNullOrEmpty(attributeNo))
                        {
                            _mast.SetDescription(attributeNo + "_" + expiryDate);
                        }
                        else
                        {
                            _mast.SetDescription(expiryDate.ToString());
                        }
                    }
                    qry = @"SELECT M_AttributeSetInstance_ID FROM M_AttributeSetINstance";

                    if (sql.Length > 0)
                    {
                        sql.Insert(0, " where ");
                        qry += sql + " order by m_attributesetinstance_id";
                    }
                    else
                    {
                        qry = "";
                    }
                    if (qry != "")
                    {
                        attrID = Util.GetValueOfInt(DB.ExecuteScalar(qry, null, null));
                        if (attrID == 0)
                        {
                            if (_mast.Save())
                            {
                                attrID = _mast.GetM_AttributeSetInstance_ID();
                                name = _mast.GetDescription();
                            }
                        }
                    }
                }
                if (attrID > 0)
                {
                    MAttributeSetInstance mas = new MAttributeSetInstance(ctx, attrID, null);
                    name = mas.GetDescription();
                }
                attribute = new KeyNamePair(attrID, name);
            }
            return attribute;
        }
    }

    public class InfoProduct
    {
        public int AD_Table_ID
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsIdentifier
        {
            get;
            set;
        }


    }
}