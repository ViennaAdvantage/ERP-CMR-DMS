using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class AttributesObjects
    {
        public bool IsReturnNull { get; set; }
        public string Error { get; set; }
        public string tableStucture = "";
        public string ControlList { get; set; }
        public int MAttributeSetID { get; set; }
    }

    public class AttributeInstance
    {
        public AttributeInstance()
        {
        }
        public int M_AttributeSetInstance_ID { get; set; }
        public string M_AttributeSetInstanceName { get; set; }
        //property for genral attribute
        public string Description { get; set; }
        public string GenSetInstance { get; set; }
        public string Error { get; set; }
    }


    public class PAttributesModel
    {
        private VLogger log = VLogger.GetVLogger(typeof(PAttributesModel).FullName);

        //Dictionary<MAttribute, KeyValuePair<MAttributeInstance, MAttributeValue[]>> attributesList = new Dictionary<MAttribute, KeyValuePair<MAttributeInstance, MAttributeValue[]>>(4);

        public AttributesObjects LoadInit(int _M_AttributeSetInstance_ID, int _M_Product_ID, bool _productWindow, int windowNo, Ctx ctx, int AD_Column_ID, int window_ID, bool IsSOTrx, string IsInternalUse)
        {

            AttributesObjects obj = new AttributesObjects();

            MAttributeSet aset = null;
            MAttribute[] attributes = null;            
            //	Get Model
            MAttributeSetInstance _masi = MAttributeSetInstance.Get(ctx, _M_AttributeSetInstance_ID, _M_Product_ID);
            MProduct _prd = new MProduct(ctx, _M_Product_ID, null);
            if (_masi == null)
            {
                obj.IsReturnNull = true;
                obj.Error = "No Model for M_AttributeSetInstance_ID=" + _M_AttributeSetInstance_ID + ", M_Product_ID=" + _M_Product_ID;
                return obj;
            }

            //	Get Attribute Set
            aset = _masi.GetMAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                obj.IsReturnNull = true;
                obj.Error = "PAttributeNoAttributeSet";
                return obj;
            }

            obj.MAttributeSetID = aset.Get_ID();

            //	Product has no Instance Attributes
            if (!_productWindow && !aset.IsInstanceAttribute())
            {
                obj.Error = "NPAttributeNoInstanceAttribute=";
                return obj;
            }

            if (_productWindow)
            {
                attributes = aset.GetMAttributes(false);
            }
            else
            {
                attributes = aset.GetMAttributes(true);
            }

            //Row 0
            obj.tableStucture = "<table style='width: 100%;'><tr>";
            if (_productWindow)
            {
                for (int i = 0; i < attributes.Length; i++)
                {
                    obj.tableStucture = AddAttributeLine(attributes[i], _M_AttributeSetInstance_ID, true, false, windowNo, obj, i);
                }
            }
            else
            {
                var newEditContent = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "NewRecord"));
                if (_M_AttributeSetInstance_ID > 0)
                {
                    newEditContent = VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "EditRecord"));
                }
                //column 1
                obj.tableStucture += "<td style = 'visibility: hidden;'>";
                obj.tableStucture += "<input type='checkbox' id='chkNewEdit_" + windowNo + "' ><label  class='VIS_Pref_Label_Font'>" + newEditContent + "</label>";
                obj.tableStucture += "</td>";

                //column 2
                obj.tableStucture += "<td>";
                obj.tableStucture += "<button type='button' style='margin-bottom: 10px;' id='btnSelect_" + windowNo + "' role='button' aria-disabled='false'><img style='float: left;' src='~/Areas/VIS/Images/base/Delete24.PNG' /><span style='float: left;margin-left: 5px;margin-right: 5px;' >" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "SelectExisting")) + "</span></button>";
                obj.tableStucture += "</td>";
                obj.tableStucture += "</tr>";

                //Change 20-May-2015 Bharat
                var label = Msg.Translate(ctx, "AttrCode");
                obj.tableStucture += "<tr>";
                obj.tableStucture += "<td>";
                obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id=lot_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label>";
                obj.tableStucture += "</td>";
                //column 2
                obj.tableStucture += "<td>";
                obj.tableStucture += "<input  style='width: 100%;' id='txtAttrCode_" + windowNo + "' value='' class='VIS_Pref_pass' type='text'>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";

                //Row 1
                obj.tableStucture += "<tr>";
                //	All Attributes
                for (int i = 0; i < attributes.Length; i++)
                {
                    obj.tableStucture = AddAttributeLine(attributes[i], _M_AttributeSetInstance_ID, true, false, windowNo, obj, i);
                }
            }

            //	Lot
            if (!_productWindow && aset.IsLot())
            {
                //column 1
                var label = Msg.Translate(ctx, "Lot");
                obj.tableStucture += "<td>";
                obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id=lot_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label>";
                obj.tableStucture += "</td>";
                //column 2
                obj.tableStucture += "<td>";
                obj.tableStucture += "<input  style='width: 100%;' id='txtLotString_" + windowNo + "' value='" + _masi.GetLot() + "' class='VIS_Pref_pass' type='text'>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";

                //Row 1
                if (!IsSOTrx || IsInternalUse == "N" || window_ID == 191 || window_ID == 140)
                {
                    obj.tableStucture += "<tr>";
                    //column 1
                    label = Msg.Translate(ctx, "M_Lot_ID");
                    obj.tableStucture += "<td>";
                    obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id=M_Lot_ID_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label>";
                    obj.tableStucture += "</td>";


                    String sql = "SELECT M_Lot_ID, Name "
                        + "FROM M_Lot l "
                        + "WHERE EXISTS (SELECT M_Product_ID FROM M_Product p "
                            + "WHERE p.M_AttributeSet_ID=" + _masi.GetM_AttributeSet_ID()
                            + " AND p.M_Product_ID=l.M_Product_ID)";

                    KeyNamePair[] data = DB.GetKeyNamePairs(sql, true);
                    //column 2
                    obj.tableStucture += "<td>";
                    obj.tableStucture += "<select style='width: 100%;margin-bottom: 10px;' id='cmbLot_" + windowNo + "'>";
                    obj.tableStucture += " <option selected value='" + 0 + "' > </option>";
                    for (int i = 1; i < data.Length; i++)
                    {
                        if (Convert.ToInt32(data[i].Key) == _masi.GetM_Lot_ID())
                        {
                            obj.tableStucture += " <option selected value='" + data[i].Key + "' >" + data[i].Name + "</option>";
                        }
                        else
                        {
                            obj.tableStucture += " <option value='" + data[i].Key + "' >" + data[i].Name + "</option>";
                        }
                    }

                    obj.tableStucture += "</select>";
                    obj.tableStucture += "</td>";
                    obj.tableStucture += "</tr>";


                    //Row 2
                    obj.tableStucture += "<tr>";

                    //	New Lot Button
                    if (_masi.GetMAttributeSet().GetM_LotCtl_ID() != 0)
                    {
                        if (MRole.GetDefault(ctx).IsTableAccess(MLot.Table_ID, false) && MRole.GetDefault(ctx).IsTableAccess(MLotCtl.Table_ID, false))
                        {
                            if (!_masi.IsExcludeLot(AD_Column_ID, IsSOTrx))//_windowNoParent
                            {
                                //column 1
                                obj.tableStucture += "<td></td>";
                                //column 2
                                obj.tableStucture += "<td>";
                                obj.tableStucture += "<button type='button' style='margin-bottom: 10px;' id='btnLot_" + windowNo + "' role='button' aria-disabled='false'><span >" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "New")) + "</span></button>";
                                obj.tableStucture += "</td>";

                                obj.tableStucture += "</tr>";
                                //Row 3
                                obj.tableStucture += "<tr>";
                            }
                        }
                    }
                }
                //mZoom = new System.Windows.Forms.ToolStripMenuItem(Msg.GetMsg(Env.GetContext(), "Zoom"), Env.GetImageIcon("Zoom16.gif"));
                //mZoom.Click += new EventHandler(mZoom_Click);
                //ctxStrip.Items.Add(mZoom);
            }

            //	SerNo
            if (!_productWindow && aset.IsSerNo())
            {
                //column 1
                var label = Msg.Translate(ctx, "SerNo");
                obj.tableStucture += "<td>";
                obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id=SerNo_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label>";
                obj.tableStucture += "</td>";

                //column 2
                // txtSerNo.Text = _masi.GetSerNo();
                obj.tableStucture += "<td>";
                obj.tableStucture += "<input style='width: 100%;'  id='txtSerNo_" + windowNo + "' value='" + _masi.GetSerNo() + "' class='VIS_Pref_pass' type='text'>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";

                //Row 1
                obj.tableStucture += "<tr>";

                //	New SerNo Button
                if (_masi.GetMAttributeSet().GetM_SerNoCtl_ID() != 0)
                {
                    if (MRole.GetDefault(ctx).IsTableAccess(MSerNoCtl.Table_ID, false))
                    {
                        if (!_masi.IsExcludeSerNo(AD_Column_ID, IsSOTrx))//_windowNoParent
                        {
                            //column 1
                            obj.tableStucture += "<td></td>";
                            obj.tableStucture += "<td>";
                            obj.tableStucture += "<button type='button' style='margin-bottom: 10px;' id='btnSerNo_" + windowNo + "' role='button' aria-disabled='false'><span >" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "New")) + "</span></button>";
                            obj.tableStucture += "</td>";
                        }

                        obj.tableStucture += "</tr>";
                        //Row 2
                        obj.tableStucture += "<tr>";
                    }
                }
            }	//	SerNo

            ////	GuaranteeDate
            if (!_productWindow && aset.IsGuaranteeDate())
            {
                var dtpicGuaranteeDate = TimeUtil.AddDays(DateTime.Now, _prd.GetGuaranteeDays());
                if (_M_AttributeSetInstance_ID > 0)
                {
                    dtpicGuaranteeDate = (DateTime)(_masi.GetGuaranteeDate());
                }
                var label = Msg.Translate(ctx, "GuaranteeDate");
                //Column 1
                obj.tableStucture += "<td>";
                obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id='guaranteeDate_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label + "</label>";
                obj.tableStucture += "</td>";
                //Column 2
                obj.tableStucture += "<td>";
                //obj.tableStucture += "<input style='width: 100%;' value='" + String.Format("{0:yyyy-MM-dd}", dtpicGuaranteeDate) + "' type='date'  id='dtpicGuaranteeDate_" + windowNo + "' class='VIS_Pref_pass'/>";
                obj.tableStucture += "<input style='width: 100%;' value='" + String.Format("{0:yyyy-MM-dd}", dtpicGuaranteeDate) + "' type='date'  id='dtpicGuaranteeDate_" + windowNo + "' class='VIS_Pref_pass'/>";
                obj.tableStucture += "</td>";

                obj.tableStucture += "</tr>";
                //Row 2
                obj.tableStucture += "<tr>";
            }

            //string[] sep = new string[1];
            //sep[0] = "<tr>";
            //sep = obj.tableStucture.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            //if (sep.Length <= 3)
            //{
            //    obj.Error = "PAttributeNoInfo";
            //    obj.IsReturnNull = true;
            //    return null;
            //}

            //	New/Edit Window
            if (!_productWindow)
            {
                //chkNewEdit.IsChecked = _M_AttributeSetInstance_ID == 0;
            }

            //	Attrribute Set Instance Description
            //Column 1
            var label1 = Msg.Translate(ctx, "Description");
            obj.tableStucture += "<td>";
            obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' id='description_" + windowNo + "' class='VIS_Pref_Label_Font'>" + label1 + "</label>";
            obj.tableStucture += "</td>";
            //Column 2
            obj.tableStucture += "<td>";
            obj.tableStucture += "<input style='width: 100%;' readonly  id='txtDescription_" + windowNo + "' value='" + _masi.GetDescription() + "' class='VIS_Pref_pass vis-gc-vpanel-table-readOnly' type='text'>";
            obj.tableStucture += "</td>";

            obj.tableStucture += "</tr>";


            //Add Ok and Cancel button 
            //Last row
            obj.tableStucture += "<tr>";

            obj.tableStucture += "<td style='text-align:right'  colspan='2'>";
            obj.tableStucture += "<button style='margin-bottom:0px;margin-top:0px; float:right' type='button' class='VIS_Pref_btn-2' style='float: right;'  id='btnCancel_" + windowNo + "' role='button' aria-disabled='false'>" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "Cancel")) + "</button>";
            obj.tableStucture += "<button style='margin-bottom:0px;margin-top:0px; float:right; margin-right: 10px;' type='button' class='VIS_Pref_btn-2' style='float: right; margin-right: 10px;' id='btnOk_" + windowNo + "' role='button' aria-disabled='false'>" + VAdvantage.Utility.Util.CleanMnemonic(Msg.GetMsg(ctx, "OK")) + "</button>";
            obj.tableStucture += "</td>";
            obj.tableStucture += "</tr>";

            obj.tableStucture += "</table>";
            if (obj.ControlList != null)
            {
                if (obj.ControlList.Length > 1)
                    obj.ControlList = obj.ControlList.Substring(0, obj.ControlList.Length - 1); ;
            }
            return obj;
        }

        public List<String> GetAttribute(int _M_AttributeSetInstance_ID, int _M_Product_ID, bool _productWindow, int windowNo, Ctx ctx, int AD_Column_ID, int attrcode)
        {
            List<String> attrValues = new List<String>();

            int attr_ID = 0;
            StringBuilder sql = new StringBuilder();
            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            MAttributeSetInstance _masi = MAttributeSetInstance.Get(ctx, _M_AttributeSetInstance_ID, _M_Product_ID);
            //	Get Attribute Set
            aset = _masi.GetMAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                Msg.GetMsg("PAttributeNoAttributeSet", null);
                return null; ;
            }
            string attrsetQry = @"SELECT ats.M_AttributeSet_ID FROM M_ProductAttributes patr LEFT JOIN  M_AttributeSetInstance ats 
                        ON (patr.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) where patr.UPC='" + attrcode + "'";
            int attributeSet = Util.GetValueOfInt(DB.ExecuteScalar(attrsetQry));

            if (attributeSet != aset.Get_ID())
            {
                return null;
            }

            ////	Product has no Instance Attributes
            //if (!_productWindow && !aset.IsInstanceAttribute())
            //{
            //    Dispatcher.BeginInvoke(() => Classes.ShowMessage.Error("PAttributeNoInstanceAttribute", null));
            //    //ADialog.error(m_WindowNo, this, "PAttributeNoInstanceAttribute");
            //    return;
            //}
            if (_productWindow)
            {
                attributes = aset.GetMAttributes(false);
                log.Fine("Product Attributes=" + attributes.Length);

            }
            else
            {
                attributes = aset.GetMAttributes(true);
            }

            if (attributes.Length > 0)
            {
                string attrQry = @"SELECT ats.M_Attribute_ID,ats.M_AttributeValue_ID,ats.Value,att.attributevaluetype FROM M_ProductAttributes patr LEFT JOIN  M_AttributeInstance ats 
                    ON (patr.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) inner join M_attributesetinstance ast ON (patr.M_AttributeSetInstance_ID=ast.M_AttributeSetInstance_ID)
                    LEFT JOIN M_Attribute att ON ats.M_Attribute_ID=att.M_Attribute_ID
                    where patr.UPC='" + attrcode + "' AND ast.M_AttributeSet_ID = " + _masi.GetM_AttributeSet_ID() + " Order By ats.M_Attribute_ID";
                DataSet ds = null;
                try
                {
                    ds = DB.ExecuteDataset(attrQry, null, null);
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                if (Util.GetValueOfString(ds.Tables[0].Rows[i]["AttributeValueType"]) == "L")
                                {
                                    attrValues.Add(Util.GetValueOfString(ds.Tables[0].Rows[i]["M_AttributeValue_ID"]));
                                }
                                else
                                {
                                    attrValues.Add(Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]));
                                }
                            }
                            ds.Dispose();
                        }
                        else
                        {
                            ds.Dispose();
                        }
                    }
                    else
                    {
                        ds.Dispose();
                    }
                }
                catch
                {

                }
                finally
                {
                    ds.Dispose();
                }
            }
            if (attrValues.Count == 0)
            {
                attrValues.Add("");                
            }
            return attrValues;
        }
        public List<String> GetAttributeInstance(int _M_AttributeSetInstance_ID, int _M_Product_ID, bool _productWindow, int windowNo, Ctx ctx, int AD_Column_ID, int attrcode)
        {
            List<String> attrValues = new List<String>();

            int attr_ID = 0;
            StringBuilder sql = new StringBuilder();
            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            MAttributeSetInstance _masi = MAttributeSetInstance.Get(ctx, _M_AttributeSetInstance_ID, _M_Product_ID);
            //	Get Attribute Set
            aset = _masi.GetMAttributeSet();
            //	Product has no Attribute Set
            if (aset == null)
            {
                Msg.GetMsg("PAttributeNoAttributeSet", null);
                return null; ;
            }
            string attrsetQry = @"SELECT ats.M_AttributeSet_ID FROM M_ProductAttributes patr LEFT JOIN  M_AttributeSetInstance ats 
                        ON (patr.M_AttributeSetInstance_ID=ats.M_AttributeSetInstance_ID) where patr.UPC='" + attrcode + "'";
            int attributeSet = Util.GetValueOfInt(DB.ExecuteScalar(attrsetQry));

            if (attributeSet != aset.Get_ID())
            {
                return null;
            }
            if (!_productWindow && aset.IsLot())
            {
                sql.Append("SELECT ats.Lot,ats.SerNo,ats.GuaranteeDate ");
            }	//	Lot
            //if (!_productWindow && aset.IsSerNo())
            //{
            //    sql.Append(" SELECT ats.SerNo");
            //}
            //if (!_productWindow && aset.IsGuaranteeDate())
            //{
            //    if (sql.Length > 0)
            //    {
            //        sql.Append(",ats.GuaranteeDate");
            //    }
            //    else
            //    {
            //        sql.Append("SELECT ats.GuaranteeDate");
            //    }
            //}	//	GuaranteeDate
            if (sql.Length > 0)
            {
                sql.Append(@" FROM M_ProductAttributes patr INNER JOIN M_AttributeSetInstance ats ON (patr.m_attributesetinstance_id=ats.m_attributesetinstance_id) WHERE patr.UPC='" + attrcode + "'");
                DataSet ds1 = null;
                try
                {
                    ds1 = DB.ExecuteDataset(sql.ToString(), null, null);

                    if (ds1 != null)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            if (!_productWindow && aset.IsLot())
                            {
                                attrValues.Add(Util.GetValueOfString(ds1.Tables[0].Rows[0]["Lot"]));
                            }	//	Lot
                            else
                            {
                                attrValues.Add("");
                            }
                            if (!_productWindow && aset.IsSerNo())
                            {
                                attrValues.Add(Util.GetValueOfString(ds1.Tables[0].Rows[0]["SerNo"]));
                            }
                            else
                            {
                                attrValues.Add("");
                            }
                            if (!_productWindow && aset.IsGuaranteeDate())
                            {
                                attrValues.Add(Util.GetValueOfString(ds1.Tables[0].Rows[0]["GuaranteeDate"]));
                            }	//	GuaranteeDate
                            else
                            {
                                attrValues.Add("");
                            }
                            ds1.Dispose();
                        }
                        else
                        {
                            ds1.Dispose();
                        }
                    }
                    else
                    {
                        ds1.Dispose();
                    }
                }
                catch
                {
                    attrValues.Clear();
                }
                finally
                {
                    ds1.Dispose();
                }
            }
            if (attrValues.Count == 0)
            {
                attrValues.Add("");
                attrValues.Add("");
                attrValues.Add("");
            }
            return attrValues;
        }

        /// <summary>
        /// Table line structure
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="product"></param>
        /// <param name="readOnly"></param>
        private string AddAttributeLine(MAttribute attribute, int M_AttributeSetInstance_ID, bool product, bool readOnly, int windowNo, AttributesObjects obj, int count)
        {
            log.Fine(attribute + ", Product=" + product + ", R/O=" + readOnly);
            //Column 1
            obj.tableStucture += "<td>";
            if (product)
            {
                obj.tableStucture += "<label style=' font-weight:bold; padding-bottom: 10px; padding-right: 5px;' class='VIS_Pref_Label_Font' id=" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ">" + attribute.GetName() + "</label>";
            }
            else
            {
                obj.tableStucture += "<label style='padding-bottom: 10px; padding-right: 5px;' class='VIS_Pref_Label_Font' id=" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "  >" + attribute.GetName() + "</label>";
            }
            obj.tableStucture += "</td>";

            MAttributeInstance instance = attribute.GetMAttributeInstance(M_AttributeSetInstance_ID);

            if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attribute.GetAttributeValueType()))
            {
                MAttributeValue[] values = attribute.GetMAttributeValues();
                //Column 2
                obj.tableStucture += "<td>";
                if (readOnly)
                {
                    obj.tableStucture += "<select style='width: 100%;margin-bottom: 10px;'  readonly id='cmb_" + count + "_" + windowNo + "'>";
                }
                else
                {
                    obj.tableStucture += "<select style='width: 100%;margin-bottom: 10px;;'  id='cmb_" + count + "_" + windowNo + "'>";
                }
                obj.ControlList += "cmb_" + count + "_" + windowNo + ",";
                bool found = false;

                for (int i = 0; i < values.Length; i++)
                {
                    //Set first value default empty
                    if (values[i] == null && i == 0)
                    {
                        obj.tableStucture += " <option value='0' > </option>";
                    }
                    else if (values[i] != null)
                    {
                        if (instance != null)
                        {
                            if (values[i].GetM_AttributeValue_ID() == instance.GetM_AttributeValue_ID())
                            {
                                obj.tableStucture += " <option selected value='" + values[i].GetM_AttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                            }
                            else
                            {
                                obj.tableStucture += " <option value='" + values[i].GetM_AttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                            }
                        }
                        else
                        {
                            obj.tableStucture += " <option value='" + values[i].GetM_AttributeValue_ID() + "' >" + values[i].GetName() + "</option>";
                        }
                    }
                }

                obj.tableStucture += "</select>";
                obj.tableStucture += "</td>";

                if (found)
                {
                    log.Fine("Attribute=" + attribute.GetName() + " #" + values.Length + " - found: " + instance);
                }
                else
                {
                    log.Warning("Attribute=" + attribute.GetName() + " #" + values.Length + " - NOT found: " + instance);
                }

                if (instance != null)
                {
                }
                else
                {
                    log.Fine("Attribute=" + attribute.GetName() + " #" + values.Length + " no instance");
                }
            }
            else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attribute.GetAttributeValueType()))
            {
                string value = null;
                if (instance != null)
                {
                    value = instance.GetValue();
                }
                //Column 2
                obj.tableStucture += "<td>";
                if (readOnly)
                {
                    obj.tableStucture += "<input style='width: 100%;' class='VIS_Pref_pass' readonly id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + value + "' class='' type='number'>";
                }
                else
                {
                    string addclass = "VIS_Pref_pass";
                    if (attribute.IsMandatory())
                    {
                        addclass += " vis-gc-vpanel-table-mandatory ";
                    }

                    obj.tableStucture += "<input style='width: 100% ;' maxlength='40' class='" + addclass + "' id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + value + "' class='' type='number'>";
                }
                obj.ControlList += "txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ",";
                obj.tableStucture += "</td>";
            }
            else	//	Text Field
            {
                string value = null;
                if (instance != null)
                {
                    value = instance.GetValue();
                }

                //Column 2
                obj.tableStucture += "<td>";
                if (readOnly)
                {
                    obj.tableStucture += "<input style='width: 100%;' class='VIS_Pref_pass' readonly id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + value + "' class='' type='text'>";
                }
                else
                {
                    string addclass = "VIS_Pref_pass";
                    if (attribute.IsMandatory())
                    {
                        addclass += " vis-gc-vpanel-table-mandatory ";
                    }

                    obj.tableStucture += "<input style='width: 100%;' maxlength='40' class='" + addclass + "' id='txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + "' value='" + value + "' class='' type='text'>";
                }
                obj.ControlList += "txt" + attribute.GetName().Replace(" ", "") + "_" + windowNo + ",";
                obj.tableStucture += "</td>";
            }

            obj.tableStucture += "</tr>";
            //Row Add
            obj.tableStucture += "<tr>";
            return obj.tableStucture;
        }

        public bool GetExcludeEntry(int productId, int adColumn, int windowNo, Ctx ctx)
        {
            bool exclude = true;
            VAdvantage.Model.MProduct product = VAdvantage.Model.MProduct.Get(ctx, productId);
            int M_AttributeSet_ID = product.GetM_AttributeSet_ID();
            if (M_AttributeSet_ID != 0)
            {
                VAdvantage.Model.MAttributeSet mas = VAdvantage.Model.MAttributeSet.Get(ctx, M_AttributeSet_ID);
                exclude = mas.ExcludeEntry(adColumn, ctx.IsSOTrx(windowNo));
            }
            return exclude;
        }

        public AttributeInstance SaveAttribute(int windowNoParent, string strLotStringC, string strSerNoC, string dtGuaranteeDateC, string strAttrCodeC,
           bool productWindow, int mAttributeSetInstanceId, int mProductId, int windowNo, List<KeyNamePair> values, Ctx ctx)
        {
            var editors = values;
            AttributeInstance obj = new AttributeInstance();
            String strLotString = "", strSerNo = "", strAttrCode = "";
            int attributeID = 0, prdAttributes = 0, pAttribute_ID = 0, product_id = 0;
            StringBuilder sql = new StringBuilder();
            string qry = "";
            StringBuilder qryAttr = null;
            DataSet ds = null;
            DateTime? dtGuaranteeDate = null;
            bool _changed = false;

            if (!productWindow && strLotStringC != null)
            {
                strLotString = strLotStringC;
            }	//	L

            if (!productWindow && strSerNoC != null)
            {
                log.Fine("SerNo=" + strSerNoC);
                strSerNo = strSerNoC;
            }

            if (!productWindow && dtGuaranteeDateC != null)
            {
                dtGuaranteeDate = Convert.ToDateTime(dtGuaranteeDateC);
            }	//	Gua

            if (!productWindow && strAttrCodeC != null)
            {
                strAttrCode = strAttrCodeC;
            }
            if (String.IsNullOrEmpty(strAttrCode))
            {
                ctx.SetContext(windowNoParent, "AttrCode", "");
            }
            else
            {
                ctx.SetContext(windowNoParent, "AttrCode", strAttrCode);
            }

            MAttributeSet aset = null;
            MAttribute[] attributes = null;
            String mandatory = "";
            var _masi = MAttributeSetInstance.Get(ctx, 0, mProductId);
            aset = _masi.GetMAttributeSet();
            if (aset == null)
            {
                return null;
            }

            if (!productWindow && strAttrCode != "")
            {
                qryAttr = new StringBuilder();
                qryAttr.Append(@"SELECT count(*) FROM M_Product prd LEFT JOIN M_ProductAttributes patr on (prd.M_Product_ID=patr.M_Product_ID) " +
                " LEFT JOIN M_Manufacturer muf on (prd.M_Product_ID=muf.M_Product_ID) WHERE (patr.UPC = '" + strAttrCode + "' OR prd.UPC = '" + strAttrCode + "' OR muf.UPC = '" + strAttrCode + "')");
                //"AND (patr.M_Product_ID = " + _M_Product_ID + " OR prd.M_Product_ID = " + _M_Product_ID + " OR muf.M_Product_ID = " + _M_Product_ID + ")";
                prdAttributes = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                if (prdAttributes != 0)
                {
                    //qryAttr.Clear();
                    //qryAttr.Append("SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE UPC = '" + strAttrCode + "' AND M_Product_ID = " + _M_Product_ID);
                    //attributeID = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                    //if (attributeID == 0)
                    //{
                    qryAttr.Clear();
                    qryAttr.Append("SELECT M_ProductAttributes_ID FROM M_ProductAttributes WHERE UPC = '" + strAttrCode + "'");
                    pAttribute_ID = Util.GetValueOfInt(DB.ExecuteScalar(qryAttr.ToString()));
                    if (pAttribute_ID != 0)
                    {
                        MProductAttributes patr = new MProductAttributes(ctx, pAttribute_ID, null);
                        attributeID = patr.GetM_AttributeSetInstance_ID();
                        product_id = patr.GetM_Product_ID();
                    }
                    //}
                }
                _changed = true;
            }	//	Attribute Code

            if (!productWindow && aset.IsLot())
            {
                log.Fine("Lot=" + strLotString);
                String text = strLotString;
                _masi.SetLot(text);
                sql.Append("UPPER(ats.Lot) = '" + text.ToUpper() + "'");
                if (aset.IsLotMandatory() && (text == null || text.Length == 0))
                    mandatory += " - " + Msg.Translate(ctx, "Lot");
                _changed = true;
            }	//	Lot
            if (!productWindow && aset.IsSerNo())
            {
                log.Fine("SerNo=" + strSerNo);
                String text = strSerNo;
                _masi.SetSerNo(text);
                _masi.SetSerNo(text);
                if (sql.Length > 0)
                {
                    sql.Append(" and UPPER(ats.SerNo) = '" + text.ToUpper() + "'");
                }
                else
                {
                    sql.Append(" UPPER(ats.SerNo) = '" + text.ToUpper() + "'");
                }
                if (aset.IsSerNoMandatory() && (text == null || text.Length == 0))
                    mandatory += " - " + Msg.Translate(ctx, "SerNo");
                _changed = true;
            }
            if (!productWindow && aset.IsGuaranteeDate())
            {
                log.Fine("GuaranteeDate=" + dtGuaranteeDate);
                DateTime? ts = dtGuaranteeDate;
                _masi.SetGuaranteeDate(ts);
                if (sql.Length > 0)
                {
                    sql.Append(" AND ats.GuaranteeDate = " + GlobalVariable.TO_DATE(dtGuaranteeDate, true));
                }
                else
                {
                    sql.Append(" ats.GuaranteeDate = " + GlobalVariable.TO_DATE(dtGuaranteeDate, true));
                }
                if (aset.IsGuaranteeDateMandatory() && ts == null)
                    mandatory += " - " + Msg.Translate(ctx, "GuaranteeDate");
                _changed = true;
            }	//	GuaranteeDate

            if (sql.Length > 0)
            {
                sql.Insert(0, " where ");
            }
            sql.Append(" order by ats.m_attributesetinstance_id");

            //	***	Save Attributes ***
            //	New Instance
            if (_changed || _masi.GetM_AttributeSetInstance_ID() == 0)
            {
                //_masi.Save();
                //obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                //mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                //obj.M_AttributeSetInstanceName = _masi.GetDescription();
            }
            //	Save Instance Attributes
            attributes = aset.GetMAttributes(!productWindow);

            if (attributes.Length > 0)
            {
                qry = @"SELECT ats.M_AttributeSetInstance_ID, av.M_AttributeValue_ID,ats.M_AttributeSet_ID,au.Value,att.AttributeValueType FROM M_AttributeSetInstance ats 
                        INNER JOIN M_AttributeInstance au ON ats.M_AttributeSetInstance_ID=au.M_AttributeSetInstance_ID LEFT JOIN M_Attribute att 
                        ON au.M_Attribute_ID=att.M_Attribute_ID LEFT JOIN M_AttributeValue av ON au.M_AttributeValue_ID=av.M_AttributeValue_ID";
            }
            else
            {
                qry = @"SELECT ats.M_AttributeSetInstance_ID FROM M_AttributeSetInstance ats ";
            }

            if (sql.Length > 0)
            {
                qry += sql;
            }
            if (attributes.Length > 0)
            {
                qry += ",au.M_Attribute_ID";
            }

            ds = DB.ExecuteDataset(qry, null, null);
            Dictionary<MAttribute, object> lst = new Dictionary<MAttribute, object>();
            for (int i = 0; i < attributes.Length; i++)
            {
                if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                {
                    object editor = editors[i];
                    MAttributeValue value = null;
                    if (Convert.ToInt32(editors[i].Key) > 0)
                    {
                        value = new MAttributeValue(ctx, Convert.ToInt32(editors[i].Key), null);
                        value.SetName(editors[i].Name);
                    }
                    log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && value == null)
                    {
                        mandatory += " - " + attributes[i].GetName();
                    }
                    lst[attributes[i]] = value;
                    //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                }
                else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                {
                    object editor = editors[i].Name;
                    decimal value = Convert.ToDecimal(editor);
                    log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory())
                    {
                        mandatory += " - " + attributes[i].GetName();
                    }
                    lst[attributes[i]] = value;
                    //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                }
                else
                {
                    object editor = editors[i].Name;
                    String value = Convert.ToString(editor);
                    log.Fine(attributes[i].GetName() + "=" + value);
                    if (attributes[i].IsMandatory() && (value == null || value.Length == 0))
                    {
                        mandatory += " - " + attributes[i].GetName();
                    }
                    lst[attributes[i]] = value;
                    //attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                }
                _changed = true;
            }

            if (_changed)
            {
                if (mandatory.Length > 0)
                {
                    obj.Error = Msg.GetMsg(ctx, "FillMandatory") + mandatory;
                    return obj;
                }
                if (attributes.Length > 0)
                {                    
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            int attCount = 0;
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                int attSetID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSet_ID"]);
                                string valueType = Util.GetValueOfString(ds.Tables[0].Rows[i]["AttributeValueType"]);
                                int attributesetinstance_iD = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                if (i > 0 && attributesetinstance_iD != Util.GetValueOfInt(ds.Tables[0].Rows[i - 1]["M_AttributeSetInstance_ID"]))
                                {
                                    attCount = 0;
                                }
                                for (int j = 0; j < attributes.Length; j++)
                                {
                                    if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[j].GetAttributeValueType()) && MAttribute.ATTRIBUTEVALUETYPE_List.Equals(valueType))
                                    {
                                        int attID = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeValue_ID"]);
                                        MAttributeValue atr = new MAttributeValue(ctx, attID, null);

                                        if (Util.GetValueOfString(atr.GetName()) == Util.GetValueOfString(lst[attributes[j]]) && attSetID == aset.GetM_AttributeSet_ID())
                                        {
                                            attCount += 1;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[j].GetAttributeValueType()) && MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(valueType))
                                    {
                                        decimal? attVal = Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["Value"]);
                                        if (attVal == Util.GetValueOfDecimal(lst[attributes[j]]) && attSetID == aset.GetM_AttributeSet_ID())
                                        {
                                            attCount += 1;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else if (MAttribute.ATTRIBUTEVALUETYPE_StringMax40.Equals(attributes[j].GetAttributeValueType()) && MAttribute.ATTRIBUTEVALUETYPE_StringMax40.Equals(valueType))
                                    {
                                        string attVal = Util.GetValueOfString(ds.Tables[0].Rows[i]["Value"]);
                                        if (attVal == Util.GetValueOfString(lst[attributes[j]]) && attSetID == aset.GetM_AttributeSet_ID())
                                        {
                                            attCount += 1;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }

                                if (attCount == attributes.Length)
                                {
                                    mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                    break;
                                }
                            }
                            if (attCount != attributes.Length)
                            {
                                mAttributeSetInstanceId = 0;
                            }
                        }
                        else
                        {
                            mAttributeSetInstanceId = 0;
                        }
                    }
                    else
                    {
                        mAttributeSetInstanceId = 0;
                    }
                    ds.Dispose();
                }
                else
                {
                    if (ds != null)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                mAttributeSetInstanceId = Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]);
                                break;
                            }
                        }
                        else
                        {
                            mAttributeSetInstanceId = 0;
                        }
                    }
                    else
                    {
                        mAttributeSetInstanceId = 0;
                    }
                    ds.Dispose();
                }

                if (mAttributeSetInstanceId == 0)
                {
                    _masi.Save();

                    mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                    obj.M_AttributeSetInstanceName = _masi.GetDescription();
                }

                else
                {
                    _masi = new MAttributeSetInstance(ctx, mAttributeSetInstanceId, null);
                }

                for (int i = 0; i < attributes.Length; i++)
                {
                    if (MAttribute.ATTRIBUTEVALUETYPE_List.Equals(attributes[i].GetAttributeValueType()))
                    {
                        MAttributeValue value = lst[attributes[i]] != null ? lst[attributes[i]] as MAttributeValue : null;
                        attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, value);
                    }
                    else if (MAttribute.ATTRIBUTEVALUETYPE_Number.Equals(attributes[i].GetAttributeValueType()))
                    {
                        attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, (decimal?)lst[attributes[i]]);
                    }
                    else
                    {
                        attributes[i].SetMAttributeInstance(mAttributeSetInstanceId, (String)lst[attributes[i]]);
                    }
                }

                if (attributeID == 0 && strAttrCode != "")
                {
                    MProductAttributes pAttr = new MProductAttributes(ctx, 0, null);
                    pAttr.SetUPC(strAttrCode);
                    pAttr.SetM_Product_ID(mProductId);
                    pAttr.SetM_AttributeSetInstance_ID(mAttributeSetInstanceId);
                    pAttr.Save();
                }
                _masi.SetDescription();
                _masi.Save();

                mAttributeSetInstanceId = _masi.GetM_AttributeSetInstance_ID();
                obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();
                //
                if (attributeID != 0 && (attributeID != mAttributeSetInstanceId || product_id != mProductId))
                {
                    obj.Error = Msg.GetMsg(ctx, "AttributeCodeExists");                    
                }                
            }
            else
            {
                obj.M_AttributeSetInstance_ID = _masi.GetM_AttributeSetInstance_ID();
                obj.M_AttributeSetInstanceName = _masi.GetDescription();              
            }
            return obj;
        }


        public string GetSerNo(Ctx ctx, int M_AttributeSetInstance_ID, int M_Product_ID)
        {
            var masi = MAttributeSetInstance.Get(ctx, M_AttributeSetInstance_ID, M_Product_ID);
            return masi.GetSerNo(true);
        }

        public KeyNamePair CreateLot(Ctx ctx, int M_AttributeSetInstance_ID, int M_Product_ID)
        {
            var masi = MAttributeSetInstance.Get(ctx, M_AttributeSetInstance_ID, M_Product_ID);
            return masi.CreateLot(M_Product_ID);
        }
    }
}