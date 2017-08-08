; (function (VIS, $) {

    // PAttributesForm form declraion for constructor class
    function PAttributesForm(M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, C_BPartner_ID, proWindow, AD_Column_ID, pwindowNo) {
        this.onClose = null;
        var $self = this;
        var $root = $("<div style='position:relative'>");
        var $busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");

        var windowNo = VIS.Env.getWindowNo();
        var mAttributeSetInstanceId = null;
        var mLocatorId = null;
        var mAttributeSetInstanceName = null;
        var mProductId = null;
        var cBPartnerId = null;
        var adColumnId = null;
        var windowNoParent = null;
        var M_Lot_ID = null;
        /**	Enter Product Attributes		*/
        var productWindow = false;
        /**	Change							*/
        var changed = false;
        var INSTANCE_VALUE_LENGTH = 40;
        var attributesList = {};

        var controlList = null;
        var mAttributeSetID = null;
        var winQry = "";
        var window_ID = 0;
        var IsSOTrx = false;
        var IsInternalUse = false;
        this.log = VIS.Logging.VLogger.getVLogger("PAttributesForm");
        this.log.config("M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID + ", M_Product_ID=" + M_Product_ID + ", C_BPartner_ID=" + C_BPartner_ID + ", ProductW=" + productWindow + ", Column=" + AD_Column_ID);

        //constructor load
        mAttributeSetInstanceId = M_AttributeSetInstance_ID;
        mProductId = M_Product_ID;
        cBPartnerId = C_BPartner_ID;
        productWindow = proWindow;
        adColumnId = AD_Column_ID;
        windowNoParent = pwindowNo;
        mLocatorId = M_Locator_ID;
        if (windowNoParent != -1) {
            winQry = "SELECT AD_Window_ID FROM AD_Tab WHERE AD_Tab_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNoParent, 0, "AD_Tab_ID"));
            window_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(winQry, null, null));
            IsSOTrx = VIS.context.isSOTrx(windowNoParent);
            IsInternalUse = VIS.context.getWindowTabContext(windowNoParent, 0, "IsInternalUse");
        }
        //productid is must for this form dependency
        //call InitAttributes in the load function
        if (mProductId == 0) {
            return;
        }
        this.hasAttribute = true;
        init();
        //initialize control varibales after load from root
        var Okbtn = $root.find("#btnOk_" + windowNo);
        var cancelbtn = $root.find("#btnCancel_" + windowNo);
        var btnSelect = $root.find("#btnSelect_" + windowNo);
        var btnLot = $root.find("#btnLot_" + windowNo);
        var btnSerNo = $root.find("#btnSerNo_" + windowNo);
        var cmbLot = $root.find("#cmbLot_" + windowNo);;
        var chkNewEdit = $root.find("#chkNewEdit_" + windowNo);
        var txtDescription = $root.find("#txtDescription_" + windowNo);
        var txtLotString = $root.find("#txtLotString_" + windowNo);
        var txtSerNo = $root.find("#txtSerNo_" + windowNo);
        var dtGuaranteeDate = $root.find("#dtpicGuaranteeDate_" + windowNo);
        var txtAttrCode = $root.find("#txtAttrCode_" + windowNo);        
        btnSelect.children(0).attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/base/Locator10.png")
        //check Arebic Calture
        if (VIS.Application.isRTL) {
            Okbtn.css("margin-right", "-138px");
            cancelbtn.css("margin-right", "55px");
        }

        //var dt = new Date(this.dtGuaranteeDate.attr("value"));
        //if (dt != null) {
        //    this.dtGuaranteeDate.val(Globalize.format(dt, "yyyy-MM-dd"));
        //}

        //	New/Edit Window
        if (!productWindow) {
            //chkNewEdit.prop("checked", mAttributeSetInstanceId == 0);
        }
        //Control that genrate run time get for first attribute
        function init() {

            $.ajax({
                url: VIS.Application.contextUrl + "PAttributes/Load",
                dataType: "json",
                async: false,
                data: {
                    mAttributeSetInstanceId: mAttributeSetInstanceId,
                    mProductId: mProductId,
                    productWindow: productWindow,
                    windowNo: windowNo,                    
                    AD_Column_ID: AD_Column_ID,
                    window_ID: window_ID,
                    IsSOTrx: IsSOTrx,
                    IsInternalUse: IsInternalUse
                },
                success: function (data) {
                    returnValue = data.result;
                    if (returnValue.Error != null) {
                        VIS.ADialog.error(returnValue.Error, null, null, null);
                        $self.hasAttribute = false;
                        return;
                    }
                    //load div
                    $root.html(returnValue.tableStucture);
                    $root.append($busyDiv);
                    if (returnValue.ControlList) {
                        controlList = returnValue.ControlList.split(',');                        
                    }
                    mAttributeSetID = returnValue.MAttributeSetID;                    
                }
            });            
        };

        function fillAttribute(attrValue) {
            attrValue = jQuery.parseJSON(attrValue);
            if (attrValue.result != null) {
                for (var item in attrValue.result) {
                    if (controlList) {
                        var cntrl = $root.find("#" + controlList[item]);
                        if (controlList[item].toString().contains("cmb")) {
                            cntrl.val(VIS.Utility.Util.getValueOfInt(attrValue.result[item]));
                        }
                        else {
                            cntrl.val(attrValue.result[item]);
                        }
                        continue;
                    }
                }
            }
            if (txtLotString != null && attrValue.lot != "") {
                txtLotString.val(attrValue.lot);
            }
            else if (txtSerNo != null && attrValue.serial != "") {
                txtSerNo.val(attrValue.serial);
            }
            if (dtGuaranteeDate != null && attrValue.gdate != "") {
                dtGuaranteeDate.val(Globalize.format(new Date(attrValue.gdate), "yyyy-MM-dd"));
            }
        };

        function saveCheckedEdit() {
            var flag = true;
            var status = true;
            if (chkNewEdit.prop("checked")) {
                var text = txtLotString.val();
                flag = false;
                var sql = "select count(*) from ad_column where columnname = 'UniqueLot' and ad_table_id = (select ad_table_id from ad_table where tablename = 'M_AttributeSet')";
                var count = VIS.DB.executeScalar(sql);
                if (count > 0) {
                    var check = checkAttrib(text);
                    if (check) {
                        flag = true;
                        VIS.ADialog.Info("LotNoExists", null, null, null);
                        return false;
                    }
                }
                if (!flag) {
                    status = saveSelection();
                }
            }

            if (flag) {
                status = saveSelection();
            }
            return status;
        }

        function saveSelection() {
            //get all controls values into it
            var result = true;
            var lst = [];
            if (controlList) {
                for (var i = 0; i < controlList.length; i++) {
                    var cntrl = $root.find("#" + controlList[i]);
                    if (controlList[i].toString().contains("cmb")) {
                        if (cntrl.find('option:selected').length > 0) {
                            lst.push({ 'Key': Number(cntrl.find('option:selected').val()), 'Name': cntrl.find('option:selected').text() });
                        }
                        else {
                            lst.push({ 'Key': 0, 'Name': cntrl.val() });
                        }
                    }
                    else {
                        if (cntrl.attr("class").contains("vis-gc-vpanel-table-mandatory") && cntrl.val().trim().length == 0) {
                            VIS.ADialog.warn("FillMandatoryFields", true, null);
                            result = false;
                            return result;
                        }
                        if (cntrl.attr("type") == "number") {
                            if(!(cntrl.val().contains("."))){
                                cntrl.val(cntrl.val().concat(".0"));
                            }
                        }
                        lst.push({ 'Key': 0, 'Name': cntrl.val() });
                    }
                }
            }

            $.ajax({
                url: VIS.Application.contextUrl + "PAttributes/Save",
                type: "POST",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify({
                    windowNoParent: windowNoParent,
                    strLotString: txtLotString.val(),
                    strSerNo: txtSerNo.val(),
                    dtGuaranteeDate: dtGuaranteeDate.val(),
                    strAttrCode: txtAttrCode.val(),
                    productWindow: productWindow,
                    mAttributeSetInstanceId: mAttributeSetInstanceId,
                    mProductId: M_Product_ID,
                    windowNo: windowNo,
                    lst: lst
                }),
                success: function (data) {
                    returnValue = data.result;
                    if (returnValue)
                        if (returnValue.Error != null) {
                            alert(returnValue.Error);
                            result = false;
                        }
                        else {
                            if ($self.onClose)
                                $self.onClose(returnValue.M_AttributeSetInstance_ID, returnValue.M_AttributeSetInstanceName, mLocatorId);
                        }
                }
            });
            return result;
        }

        function cmdSelect() {
            $self.log.config("");            
            var M_Warehouse_ID = 0;
            if (windowNoParent != -1) {
                if (VIS.DTD001 && window_ID == 170) {
                    M_Warehouse_ID = VIS.Env.getCtx().getContextAsInt(windowNoParent, "DTD001_MWarehouseSource_ID");
                }
                else {
                    M_Warehouse_ID = VIS.Env.getCtx().getContextAsInt(windowNoParent, "M_Warehouse_ID");
                }
            }
            var title = "";
            //	Get Text
            var sql = "SELECT p.Name, w.Name FROM M_Product p, M_Warehouse w "
                + "WHERE p.M_Product_ID=" + mProductId + " AND w.M_Warehouse_ID=" + M_Warehouse_ID;

            var dr = null;
            try {
                dr = VIS.DB.executeReader(sql, null);
                if (dr.read()) {
                    title = dr.getString(0) + " - " + dr.getString(1);
                }
                dr.close();
                dr = null;
            }
            catch (e) {
                if (dr != null) {
                    dr.close();
                }
                this.log.Log(Level.SEVERE, sql, e);
            }

            var AttributeSetInstance_ID = -1;
            //open new form
            var obj = new VIS.PAttributeInstance(title, M_Warehouse_ID, mLocatorId, mProductId, cBPartnerId);
            obj.showDialog();
            obj.onClose = function (attributeSetInstanceID, name, M_Locator_ID) {
                if (attributeSetInstanceID != -1) {
                    mAttributeSetInstanceId = attributeSetInstanceID;
                    mAttributeSetInstanceName = name;
                    mLocatorId = M_Locator_ID;
                    changed = true;
                    if ($self.onClose) {
                        $self.onClose(mAttributeSetInstanceId, mAttributeSetInstanceName, mLocatorId);
                        $root.dialog('close');
                    }
                }
            };
            obj = null;
        }

        function cmdNewEdit() {
            var rw = chkNewEdit.prop("checked");
            if (txtLotString) {
                txtLotString.attr("readOnly", !rw);
                if (rw) {
                    txtLotString.removeClass("vis-gc-vpanel-table-readOnly");
                }
                else {
                    txtLotString.addClass("vis-gc-vpanel-table-readOnly");
                }
            }

            if (cmbLot) {
                if (rw) {
                    cmbLot.removeClass("vis-gc-vpanel-table-readOnly");
                    cmbLot.removeAttr("disabled");
                } else {
                    cmbLot.attr("disabled", true);
                    cmbLot.addClass("vis-gc-vpanel-table-readOnly");
                }
            }

            if (btnLot) {
                if (rw) {
                    btnLot.removeAttr("disabled");
                } else {
                    btnLot.attr("disabled", true);
                }
            }

            if (txtSerNo) {
                txtSerNo.attr("readOnly", !rw);
                if (rw) {
                    txtSerNo.removeClass("vis-gc-vpanel-table-readOnly");

                } else {
                    txtSerNo.addClass("vis-gc-vpanel-table-readOnly");
                }
            }

            if (btnSerNo) {
                if (rw) {
                    btnSerNo.removeAttr("disabled");
                } else {
                    btnSerNo.attr("disabled", true);
                }
            }

            if (dtGuaranteeDate) {
                dtGuaranteeDate.prop("disable", rw);
                if (rw) {
                    dtGuaranteeDate.removeClass("vis-gc-vpanel-table-readOnly");
                    dtGuaranteeDate.removeAttr("disabled");

                } else {
                    dtGuaranteeDate.addClass("vis-gc-vpanel-table-readOnly");
                    dtGuaranteeDate.attr("disabled", true);
                }
            }

            if (controlList) {
                for (var i = 0; i < controlList.length; i++) {
                    var cntrl = $root.find("#" + controlList[i]);
                    cntrl.attr("readOnly", !rw);
                    if (rw) {
                        cntrl.removeClass("vis-gc-vpanel-table-readOnly");
                    }
                    else {
                        cntrl.addClass("vis-gc-vpanel-table-readOnly");
                    }
                }
            }
        }

        function checkAttrib(lotString) {
            var sql = "";
            var check = false;
            var dr = null;
            try {
                sql = "SELECT COUNT(*) FROM M_Storage s INNER JOIN M_Locator l ON (l.M_Locator_ID = s.M_Locator_ID) "
                       + " inner join M_warehouse w ON (w.M_warehouse_ID = l.M_Warehouse_ID) WHERE AD_Client_ID = " + VIS.context.getAD_Client_ID();

                var sqlWhere = "";
                var AD_Org_ID = VIS.Env.getCtx().getContextAsInt(windowNoParent, "AD_Org_ID");

                var sqlChk = "SELECT IsOrganization, IsProduct, IsWarehouse FROM M_AttributeSet aSet INNER JOIN M_Product mp on mp.M_AttributeSet_ID = aset.M_AttributeSet_ID"
                    + " WHERE mp.M_Product_ID = " + mProductId;

                dr = VIS.DB.executeReader(sqlChk, null);
                if (dr.read()) {
                    if (dr.getString(0).toUpper() == "Y") {
                        sqlWhere = sqlWhere.concat(" OR s.AD_Org_ID = " + AD_Org_ID);
                    }
                    if (dr.getString(1).toUpper() == "Y") {
                        sqlWhere = sqlWhere.concat(" OR s.M_Product_ID = " + mProductId);
                    }
                    if (dr.getString(2).toUpper() == "Y") {
                        var M_Warehouse_ID = 0;
                        var sqlMovement = "SELECT TableName FROM AD_Table WHERE AD_Table_ID = " + VIS.Env.getCtx().getContext(windowNoParent, "BaseTable_ID");
                        var innerdr = VIS.DB.executeReader(sqlMovement, null);
                        if (innerdr.read()) {
                            if (innerdr.getString(0).toUpper() == "M_MOVEMENT") {
                                var sqlWarehouse = "SELECT wh.M_Warehouse_ID FROM M_Warehouse wh INNER JOIN M_Locator l ON (wh.M_Warehouse_ID = l.M_Warehouse_ID) "
                        + " WHERE l.M_Locator_ID = " + VIS.Env.getCtx().getContext(windowNoParent, "M_LocatorTo_ID");
                                M_Warehouse_ID = VIS.DB.executeScalar(sqlWarehouse, null);
                            }
                            innerdr.close();
                            innerdr = null;
                        }
                        else {
                            M_Warehouse_ID = VIS.Env.getCtx().getContextAsInt(windowNoParent, "M_Warehouse_ID");
                        }

                        sqlWhere = sqlWhere.concat(" OR w.M_Warehouse_ID = " + M_Warehouse_ID);
                    }
                    if (sqlWhere.length > 0) {
                        sqlWhere = sqlWhere.Remove(0, 3);
                        sql = sql + " AND (" + sqlWhere.toString();
                        sql = sql + ") AND s.M_AttributeSetInstance_ID IN (SELECT M_AttributeSetInstance_ID FROM M_AttributeSetInstance WHERE Lot = '" + lotString + "')";
                    }
                }
                dr.close();
                dr = null;

                var checkProd = VIS.DB.executeScalar(sql);
                if (checkProd > 0) {
                    check = true;
                }

            }
            catch (e) {
                if (dr != null) {
                    dr.close();
                }
                //$self.log.log(Level.SEVERE, sql, e);
            }
            return check;
        }

        function lotChange(returnValue) {
            cmbLot.append(new Option(returnValue.Name, returnValue.Key, true, true));
            cmbLot.change();
        }

        function events() {
            if (cmbLot != null) {
                cmbLot.change(function () {
                    var pp = cmbLot.find('option:selected').val();
                    var name = cmbLot.find('option:selected').text();
                    if (pp != null && pp != -1) {
                        txtLotString.val(name);
                        txtLotString.attr("readOnly", true);
                        txtLotString.addClass("vis-gc-vpanel-table-readOnly");
                        M_Lot_ID = pp;
                    }
                    else {
                        txtLotString.attr("readOnly", false);
                        txtLotString.removeClass("vis-gc-vpanel-table-readOnly");
                    }
                });
            }

            if (chkNewEdit != null) {
                chkNewEdit.change(function () {
                    cmdNewEdit();
                });
            }

            if (txtAttrCode != null) {
                txtAttrCode.on("keydown", function (event) {
                    if (event.keyCode == 13 || event.keyCode == 9) {//will work on press of Tab key OR Enter Key
                        $.ajax({
                            url: VIS.Application.contextUrl + "PAttributes/GetAttribute",
                            dataType: "json",
                            async: false,
                            data: {
                                mAttributeSetInstanceId: mAttributeSetInstanceId,
                                mProductId: mProductId,
                                productWindow: productWindow,
                                windowNo: windowNo,
                                AD_Column_ID: AD_Column_ID,
                                attrcode: txtAttrCode.val()
                            },
                            success: function (data) {
                                returnValue = data;
                                if (returnValue != null) {
                                    fillAttribute(returnValue);
                                }
                                else {
                                }
                            }
                        });
                    }
                });
            }

            if (Okbtn != null) {
                Okbtn.on("click", function () {
                    setBusy(true);
                    if (!saveCheckedEdit()) {
                    } else
                        $root.dialog('close');

                    setBusy(false);
                });
            }

            if (cancelbtn != null) {
                cancelbtn.on("click", function () {
                    $root.dialog('close');
                });
            }

            if (btnSelect != null) {
                setBusy(true);
                btnSelect.on("click", function () {
                    cmdSelect();
                });
                setBusy(false);
            }

            if (btnLot != null) {
                btnLot.on("click", function () {
                    $.ajax({
                        url: VIS.Application.contextUrl + "PAttributes/CreateLot",
                        dataType: "json",
                        async: false,
                        data: {
                            mAttributeSetInstanceId: mAttributeSetInstanceId,
                            mProductId: mProductId
                        },
                        success: function (data) {
                            returnValue = data.result;
                            lotChange(returnValue);
                        }
                    });
                });
            }

            if (btnSerNo != null) {
                btnSerNo.on("click", function () {
                    $.ajax({
                        url: VIS.Application.contextUrl + "PAttributes/GetSerNo",
                        dataType: "json",
                        async: false,
                        data: {
                            mAttributeSetInstanceId: mAttributeSetInstanceId,
                            mProductId: mProductId
                        },
                        success: function (data) {
                            returnValue = data.result;
                            txtSerNo.val(returnValue);
                        }
                    });
                });
            }

            if (controlList) {
                // add evnts at run time
            }
        }

        events();

        function setBusy(isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.showDialog = function () {
            $root.dialog({
                modal: true,
                title: VIS.Msg.translate(VIS.Env.getCtx(), "M_AttributeSetInstance_ID"),
                width: 490,
                close: function () {
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });
            if (controlList && M_AttributeSetInstance_ID == 0) {
                for (var i = 0; i < controlList.length; i++) {
                    if (controlList[i].toString().contains("cmb")) {
                        $root.find("#" + controlList[i]).prop('selectedIndex', -1);
                    }
                }
            }
            setBusy(false);
        };

        this.disposeComponent = function () {
            if (Okbtn)
                Okbtn.off("click");
            if (cancelbtn)
                cancelbtn.off("click");
            VIS.Env.clearWinContext(VIS.Env.getCtx(), windowNo);
            VIS.Env.getCtx().setContext(VIS.Env.WINDOW_INFO, VIS.Env.TAB_INFO, "M_AttributeSetInstance_ID", mAttributeSetInstanceId);
            VIS.Env.getCtx().setContext(VIS.Env.WINDOW_INFO, VIS.Env.TAB_INFO, "M_Locator_ID", mLocatorId);

            mLocatorId = null;
            mAttributeSetInstanceName = null;
            mProductId = null;
            cBPartnerId = null;
            adColumnId = null;
            windowNoParent = null;
            productWindow = null;
            /**	Change							*/
            changed = null;
            INSTANCE_VALUE_LENGTH = 0;
            attributesList = null;

            Okbtn = null;
            cancelbtn = null;
            btnSelect = null;
            btnLot = null;
            btnSerNo = null;
            cmbLot = null;
            chkNewEdit = null;
            txtDescription = null;
            txtLotString = null;
            txtSerNo = null;
            dtGuaranteeDate = null;

            $self = null;
            windowNo = null;
            mAttributeSetInstanceId = null;
            this.disposeComponent = null;
        };

    };

    //dispose call
    PAttributesForm.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    VIS.PAttributesForm = PAttributesForm;

})(VIS, jQuery);

