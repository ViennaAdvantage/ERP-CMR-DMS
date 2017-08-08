
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Product info.
 * Author         :     Bharat
 * Date           :     01-May-2015
  ******************************************************/
; (function (VIS, $) {
    function infoProduct(modal, WindowNo, M_Warehouse_ID, M_PriceList_ID, value, tableName, keyColumn, multiSelection, validationCode) {

        this.onClose = null;

        var inforoot = $("<div>");
        var isExpanded = true;
        var subroot = $("<div>");
        var sSContainer = null;
        var ssHeader = null;
        var btnExpander = null;
        var searchTab = null;
        var searchSec = null;
        var datasec = null;
        var btnsec = null;
        var refreshtxt = null;
        var addCatTxt = null;
        var showSavedTxt = null;
        var canceltxt = null;
        var Oktxt = null;
        var divbtnRight = null;
        var divbtnsec = null;
        var btnScanFile = null;
        var btnShowSaved = null;
        var btnAddCart = null;
        var btnCancel = null;
        var btnOK = null;
        var divbtnLeft = null;
        var btnRequery = null;
        var bsyDiv = null;
        var schema = null;
        var srchCtrls = [];
        var displayCols = null;
        var dGrid = null;
        var self = this;
        var keyCol = '';
        var window_ID = 0;
        var multiValues = [];
        var singlevalue = null;
        var grdname = null;
        var lblValuetxt = null;
        var lblNametxt = null;
        var lblUPCtxt = null;
        var lblSKUtxt = null;
        var lblWarehousetxt = null;
        var lblPriceListtxt = null;
        var label = null;
        var ctrl = null;
        var cmbWarehoue = null;
        var cmbPriceList = null;
        var tableSArea = $("<table>");
        var tr = null;
        var lstWarehouse = null;
        var lstPriceList = null;
        var PriceList = null;
        var M_PriceList_Version_ID = null;
        var infoLines = [];
        var savedProduct = [];
        var refreshUI = false;
        var updating = false;
        var AD_Column_ID = 0;
        function initializeComponent() {
            inforoot.css("width", "100%");
            inforoot.css("height", "100%");
            inforoot.css("position", "relative");
            subroot.css("width", "98%");
            subroot.css("height", "97%");
            subroot.css("position", "absolute");
            tableSArea.css("width", "100%");
            //subroot.css("margin-left", "-10px");

            //sSContainer = $("<div style='display: inline-block; float: left;width:23%;height:87.8%;overflow:auto;'>");
            //ssHeader = $('<div style="padding: 7px; background-color: #F1F1F1;margin-bottom: 2px;height:10.5%;">');
            //btnExpander = $('<button style="border: 0px;background-color: transparent; padding: 0px;">').append($('<img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/arrow-left.png">'));

            //ssHeader.append(btnExpander);
            //sSContainer.append(ssHeader);

            if (VIS.Application.isRTL) {
                searchTab = $("<div style='background-color: rgb(241, 241, 241);height:88.9%;display: inline-block;width:23%;height:87.8%;overflow:auto;padding-right: 10px;margin-left: 10px;'>");
                searchSec = $("<div style='background-color: rgb(241, 241, 241);display: inline-block;height:87.8%;'>");
                searchTab.append(searchSec);
                datasec = $("<div style='display: inline-block;width:75%;height:87.8%;overflow:auto;'>");
                btnsec = $("<div style='display: inline-block;width:99%;height:auto;margin-top: 2px;'>");
            }
            else {
                searchTab = $("<div style='background-color: rgb(241, 241, 241);padding-left: 7px;height:88.9%;display: inline-block; float: left;width:23%;height:87.8%;overflow:auto;'>");
                searchSec = $("<div style='background-color: rgb(241, 241, 241);'>");
                searchTab.append(searchSec);
                datasec = $("<div style='display: inline-block; float: left;width:75%;height:87.8%;margin-left:10px;'>");
                btnsec = $("<div style='display: inline-block; float: left;width:99%;height:auto;margin-top: 2px;'>");
            }
            //searchTab = $("<div style='background-color: rgb(241, 241, 241);padding-left: 7px;height:88.9%;overflow:auto;'>");
            //searchSec = $("<div style='background-color: rgb(241, 241, 241)'>");
            //if (true) {//IPAD
            //    ssHeader.css('display', 'none');
            //    searchTab.css('height', '100%');
            //    searchSec.css('height', '100%');
            //}
            //searchTab.append(searchSec);
            //sSContainer.append(searchTab);

            // var searchSec = $("<div style='display: inline-block; float: left;width:23%;height:86.8%;overflow:auto'>");
            //  datasec = $("<div style='display: inline-block; float: left;width:75%;height:87.8%;margin-left:10px;'>");
            //  btnsec = $("<div style='display: inline-block; float: left;width:99%;height:auto;margin-top: 2px;'>");
            subroot.append(searchTab);
            subroot.append(datasec);
            subroot.append(btnsec);

            lblValuetxt = VIS.Msg.getMsg("Value");
            if (lblValuetxt.indexOf('&') > -1) {
                lblValuetxt = lblValuetxt.replace('&', '');
            }
            lblNametxt = VIS.Msg.getMsg("Name");
            if (lblNametxt.indexOf('&') > -1) {
                lblNametxt = lblNametxt.replace('&', '');
            }
            lblUPCtxt = VIS.Msg.getMsg("UPC");
            if (lblUPCtxt.indexOf('&') > -1) {
                lblUPCtxt = lblUPCtxt.replace('&', '');
            }
            lblSKUtxt = VIS.Msg.getMsg("SKU");
            if (lblSKUtxt.indexOf('&') > -1) {
                lblSKUtxt = lblSKUtxt.replace('&', '');
            }
            lblWarehousetxt = VIS.Msg.getMsg("Warehouse");
            if (lblWarehousetxt.indexOf('&') > -1) {
                lblWarehousetxt = lblWarehousetxt.replace('&', '');
            }
            lblPriceListtxt = VIS.Msg.getMsg("PriceListVersion");
            if (lblPriceListtxt.indexOf('&') > -1) {
                lblPriceListtxt = lblPriceListtxt.replace('&', '');
            }
            refreshtxt = VIS.Msg.getMsg("Refresh");
            if (refreshtxt.indexOf('&') > -1) {
                refreshtxt = refreshtxt.replace('&', '');
            }
            addCatTxt = VIS.Msg.getMsg("AddCatalog");
            if (addCatTxt.indexOf('&') > -1) {
                addCatTxt = addCatTxt.replace('&', '');
            }
            showSavedTxt = VIS.Msg.getMsg("ShowSaved");
            if (showSavedTxt.indexOf('&') > -1) {
                showSavedTxt = showSavedTxt.replace('&', '');
            }
            canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }
            Oktxt = VIS.Msg.getMsg("Ok");
            if (Oktxt.indexOf('&') > -1) {
                Oktxt = Oktxt.replace('&', '');
            }
            divbtnRight = $("<div style='float:right;'>");
            divbtnsec = $("<div style='float:right;'>");
            if (VIS.Application.isRTL) {
                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(Oktxt);
                btnAddCart = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(addCatTxt);
                btnShowSaved = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(showSavedTxt);
            }
            else {
                //var rowctrl = 0;               

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblValuetxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("Value", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "Value";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblNametxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("Name", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "Name";
                ctrl.setValue(value);
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblUPCtxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("UPC", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "UPC";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblSKUtxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("SKU", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "SKU";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblWarehousetxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                cmbWarehoue = new VIS.Controls.VComboBox("M_Warehouse_ID", false, false, true, null, 50);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = cmbWarehoue;
                srchCtrl.AD_Reference_ID = VIS.DisplayType.tableDir;
                srchCtrl.ColumnName = "M_Warehouse_ID";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(cmbWarehoue.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblPriceListtxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                cmbPriceList = new VIS.Controls.VComboBox("M_PriceList_Version_ID", false, false, true, null, 50);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = cmbPriceList;
                srchCtrl.AD_Reference_ID = VIS.DisplayType.tableDir;
                srchCtrl.ColumnName = "M_PriceList_Version_ID";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(cmbPriceList.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                searchSec.append(tableSArea);


                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(Oktxt);
                btnAddCart = $("<button class='VIS_Pref_btn-2' style='margin-right:5px;margin-top: 5px;margin-bottom: -10px'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/addToCart.png'>"))
                btnShowSaved = $("<button class='VIS_Pref_btn-2' disabled style='margin-right:5px;margin-top: 5px;margin-bottom: -10px'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/list.png'>"))
                btnScanFile = $("<button class='VIS_Pref_btn-2' style='margin-right:5px;margin-top: 5px;margin-bottom: -10px'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/scan.png'>"))
                //btnShowSaved.prop('disabled', true);
            }

            divbtnRight.append(btnCancel);
            divbtnRight.append(btnOK);
            divbtnRight.append(btnAddCart);
            divbtnRight.append(btnShowSaved);
            divbtnsec.append(btnScanFile);
            searchTab.append(divbtnsec);
            btnsec.append(divbtnRight);


            divbtnLeft = $("<div style='float:left;'>");
            btnRequery = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png'>"))//.append(refreshtxt);


            if (VIS.Application.isRTL) {
                divbtnLeft.append(btnCancel);
                divbtnLeft.append(btnOK);
                divbtnLeft.append(btnShowSaved);
                divbtnLeft.append(btnAddCart);
                divbtnsec.append(btnScanFile);
                searchTab.append(divbtnsec);
                divbtnRight.append(btnRequery);
            }
            else {
                divbtnRight.append(btnCancel);
                divbtnRight.append(btnOK);
                divbtnRight.append(btnShowSaved);
                divbtnRight.append(btnAddCart);
                divbtnsec.append(btnScanFile);
                searchTab.append(divbtnsec);
                divbtnLeft.append(btnRequery);
            }

            // divbtnLeft.append(btnRequery);
            btnsec.append(divbtnRight);
            btnsec.append(divbtnLeft);
            //Busy Indicator
            bsyDiv = $("<div class='vis-apanel-busy'>");
            bsyDiv.css("width", "98%");
            bsyDiv.css("height", "97%");
            bsyDiv.css('text-align', 'center');
            bsyDiv.css("position", "absolute");
            bsyDiv[0].style.visibility = "visible";


            inforoot.append(subroot);
            inforoot.append(bsyDiv);
        };

        initializeComponent();
        if (VIS.context.getContextAsInt(WindowNo, VIS.context.getWindowTabContext(WindowNo, 1, "KeyColumnName")) != 0) {
            updating = true;
        }
        if (!multiSelection || updating) {
            btnAddCart.hide();
            btnShowSaved.hide();
            btnScanFile.hide();
        }
        var s_productFrom =
            "M_Product p"
            + " LEFT OUTER JOIN M_ProductPrice pr ON (p.M_Product_ID=pr.M_Product_ID AND pr.IsActive='Y')"
            + " LEFT OUTER JOIN M_PriceList_Version plv ON (pr.M_PriceList_Version_ID=plv.M_PriceList_Version_ID)"
            + " LEFT OUTER JOIN M_AttributeSet pa ON (p.M_AttributeSet_ID=pa.M_AttributeSet_ID)"
            + " LEFT OUTER JOIN M_manufacturer mr ON (p.M_Product_ID=mr.M_Product_ID)"
            + " LEFT OUTER JOIN M_ProductAttributes patr ON (p.M_Product_ID=patr.M_Product_ID),"
            + " M_Warehouse w";

        function bindEvent() {
            if (!VIS.Application.isMobile) {
                inforoot.on('keyup', function (e) {
                    if (!(e.keyCode === 13)) {
                        return;
                    }
                    bsyDiv[0].style.visibility = 'visible';
                    //if (ctrl != null) {
                    if (validationCode != null && validationCode.length > 0) {
                        validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), WindowNo, validationCode, false, false);
                    }
                    // }
                    displayData(true);
                });
            }
            btnScanFile.on("click", function () {
                btnScanClick();
            });

            btnAddCart.on("click", function () {
                btnAddCartClick();
            });

            btnShowSaved.on("click", function () {
                btnShowSavedClick();
            });

            btnCancel.on("click", function () {

                disposeComponent();

            });
            btnOK.on("click", function () {
                btnOKClick();
            });
            btnRequery.on("click", function () {
                //debugger;
                if (multiSelection && !updating) {
                    btnAddCart.show();
                }
                if (infoLines.length == 0) {
                    btnShowSaved.prop('disabled', true);
                }
                multiValues = [];
                bsyDiv[0].style.visibility = 'visible';
                if (validationCode != null && validationCode != undefined && validationCode.length > 0) {
                    validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), WindowNo, validationCode, false, false);
                }
                displayData(true);
            });
        };

        bindEvent();
        InitInfo(M_Warehouse_ID, M_PriceList_ID);

        function InitInfo(M_Warehouse_ID, M_PriceList_ID) {
            var winQry = "SELECT AD_Window_ID FROM AD_Tab WHERE AD_Tab_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(WindowNo, 0, "AD_Tab_ID"));
            window_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(winQry));
            cmbWarehoue.ctrl.append($('<Option value="' + 0 + '">' + "" + '</option>'));
            cmbPriceList.ctrl.append($('<Option value="' + 0 + '">' + "" + '</option>'));
            lstWarehouse = jQuery.parseJSON(lstWarehouse);
            lstPriceList = jQuery.parseJSON(lstPriceList);
            GetWarehouse();
            M_PriceList_Version_ID = FindPLV(VIS.Env.getCtx(), WindowNo, M_PriceList_ID);
            GetPriceList(M_PriceList_ID);
        }

        function GetWarehouse() {
            var sql = VIS.MRole.getDefault().addAccessSQL(
                    "SELECT M_Warehouse_ID, Value || ' - ' || Name AS ValueName " + "FROM M_Warehouse " + "WHERE IsActive='Y'",
                    "M_Warehouse", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO)
                    + " ORDER BY Value";
            var dr = VIS.DB.executeReader(sql.toString(), null);
            var key, value;
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                value = dr.getString(1);
                cmbWarehoue.getControl().append(" <option value=" + key + ">" + value + "</option>");
                if (key == M_Warehouse_ID) {
                    cmbWarehoue.ctrl.val(M_Warehouse_ID);
                }
            }
            dr.close();
        }

        function GetPriceList(PriceList) {
            var sql = "SELECT plv.M_PriceList_Version_ID,"
                     + " plv.Name || ' (' || c.ISO_Code || ')' AS ValueName "
                     + "FROM M_PriceList_Version plv, M_PriceList pl, C_Currency c "
                     + "WHERE plv.M_PriceList_ID=pl.M_PriceList_ID" + " AND pl.C_Currency_ID=c.C_Currency_ID"
                     + " AND plv.IsActive='Y' AND pl.IsActive='Y'";
            // Same PL currency as original one
            if (PriceList != 0) {
                sql += " AND EXISTS (SELECT * FROM M_PriceList xp WHERE xp.M_PriceList_ID=" + PriceList + ")";
                //   +" AND pl.C_Currency_ID=xp.C_Currency_ID)";
            }
            // Add Access & Order
            var qry = VIS.MRole.getDefault().addAccessSQL(sql, "M_PriceList_Version", VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO) // fully qualidfied - RO
                    + " ORDER BY plv.Name";

            var dr = VIS.DB.executeReader(qry.toString(), null);
            var key, value;
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                value = dr.getString(1);
                cmbPriceList.getControl().append(" <option value=" + key + ">" + value + "</option>");
                if (key == M_PriceList_Version_ID) {
                    cmbPriceList.ctrl.val(M_PriceList_Version_ID);
                }
            }
            dr.close();
        }

        function FindPLV(ctx, p_WindowNo, M_PriceList_ID) {
            var priceDate = null;

            var dateStr = ctx.getContextAsInt(p_WindowNo, "DateOrdered");

            if (dateStr != null && dateStr.Length > 0) {
                priceDate = System.Convert.ToDateTime(VIS.Env.parseContext(VIS.Env.getCtx(), WindowNo, "DateOrdered", false, false));
            }
            else {
                dateStr = ctx.getContextAsInt(p_WindowNo, "DateInvoiced");
                if (dateStr != null && dateStr.Length > 0) {
                    priceDate = System.Convert.ToDateTime(VIS.Env.parseContext(VIS.Env.getCtx(), WindowNo, "DateInvoiced", false, false));
                }
            }
            //	Today 
            if (priceDate == null) {
                var d = new Date();
                priceDate = VIS.Utility.Util.getValueOfDate(d);
            }
            //
            //_log.config("M_PriceList_ID=" + M_PriceList_ID + " - " + priceDate);
            var retValue = 0;
            var sql = "SELECT plv.M_PriceList_Version_ID, plv.ValidFrom "
               + "FROM M_PriceList pl, M_PriceList_Version plv "
               + "WHERE pl.M_PriceList_ID=plv.M_PriceList_ID"
               + " AND plv.IsActive='Y'"
               + " AND pl.M_PriceList_ID=" + M_PriceList_ID					//	1
               + " ORDER BY plv.ValidFrom DESC";
            //	find newest one
            var dr = null;
            try {

                dr = VIS.DB.executeReader(sql, null);
                if (dr.read()) {
                    var pldate = dr.getDateTime(1);
                    if (priceDate > pldate) {
                        retValue = dr.getInt(0);
                    }
                    else {
                        retValue = dr.getInt(0);
                    }
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
            ctx.setContext(p_WindowNo, "M_PriceList_Version_ID", retValue);
            return retValue;
        }

        var btnScanClick = function () {
            debugger;
            var obj = new VIS.InfoScanForm(WindowNo);
            obj.showDialog();
            obj.onClose = function (drProd) {
                while (drProd.read()) {
                    var prodName = "";
                    var uom = "";
                    var attr_ID = 0;
                    var name = "";
                    var qty = drProd.getDecimal("vaicnt_quantity");
                    if (qty > 0) {
                        infoLines.push(
                            {
                                _prodQty: qty,
                                _prdID: drProd.getInt("M_Product_ID"),
                                _AD_Session_ID: VIS.Env.getCtx().getContext("#AD_Session_ID"),
                                _windowNo: WindowNo,
                                _RefNo: drProd.getString("VAICNT_ReferenceNo"),
                                _Attribute: drProd.getString("vaicnt_attributeno"),
                                _AttributeName: "",
                                _Locator_ID: "",
                                _IsLotSerial: "N"
                            }
                        );
                    }
                    var M_Product_ID = drProd.getInt("M_Product_ID");
                    var attributeNo = drProd.getString("vaicnt_attributeno");
                    var isLot = drProd.getString("Islot");
                    var IsSerNo = drProd.getString("IsSerNo");
                    var IsGuaranteeDate = drProd.getString("IsGuaranteeDate");
                    var expiryDate = drProd.getDateTime("ExpiryDate");
                    if (expiryDate == null) {
                        expiryDate = "null";
                    }
                    var RefNo = drProd.getString("VAICNT_ReferenceNo");
                    if (RefNo == "") {
                        RefNo = "null";
                    }
                    if (attributeNo != "") {
                        attr_ID = VIS.Utility.Util.getValueOfInt(attributeNo);
                        var attrQry = "SELECT Description FROM M_AttributeSetInstance WHERE M_AttributeSetInstance_ID = " + attr_ID;
                        name = VIS.Utility.Util.getValueOfString(VIS.DB.executeScalar(attrQry));
                    }
                    //if (attributeNo != "") {
                    //    var paramString = M_Product_ID.toString().concat(",", attributeNo, ",", //2
                    //                                       isLot, ",", //3
                    //                                       IsSerNo, ",", //4 
                    //                                       IsGuaranteeDate, ",", //5
                    //                                       expiryDate.toString(), ",", //6
                    //                                       RefNo); //7
                    //    var attribute = VIS.dataContext.getJSONRecord("InfoProduct/GetAttribute", paramString);
                    //    if (attribute != null) {
                    //        attr_ID = attribute.Key;
                    //        name = attribute.Name;
                    //    }
                    //}
                    var sqlaa = "select p.name as Product, u.name as UOM from m_product p left outer  join c_uom u on (u.c_uom_id = p.c_uom_id) where p.m_product_id = " + M_Product_ID;
                    try {
                        var dr = VIS.DB.executeReader(sqlaa.toString(), null, null);
                        while (dr.read()) {
                            prodName = dr.getString(0);
                            uom = dr.getString(1);
                        }
                    }
                    catch (e) {
                    }
                    savedProduct.push(
                       {
                           SrNo: savedProduct.length,
                           Product: prodName,
                           QtyEntered: qty,
                           UOM: uom,
                           M_Product_ID1: M_Product_ID,
                           Attribute: attr_ID,
                           AttributeName: name,
                           RefNo: RefNo
                       }
                    );
                }
                BindGridSavedProducts();
            };
        };

        var btnAddCartClick = function () {
            //debugger;
            //if (multiSelection) {
            //var selection = w2ui[grdname].getSelection();
            //for (item in selection) {
            //    multiValues.push(w2ui[grdname].get(selection[item]));
            //}
            saveSelection();
            displayData(false);
            if (infoLines.length > 0) {
                btnShowSaved.prop('disabled', false);
            }
            else {
                btnShowSaved.prop('disabled', true);
            }
            multiValues = [];
        };

        var btnShowSavedClick = function () {
            var prodID = [];
            var qty = [];
            var prodName = [];
            var uom = [];
            var Attribute = [];
            var AttributeName = [];
            var ReferenceNo = [];
            savedProduct = [];
            btnAddCart.hide();
            for (var item in infoLines) {
                prodID.push(infoLines[item]._prdID);
                qty.push(infoLines[item]._prodQty);
                ReferenceNo.push(infoLines[item]._RefNo);
                Attribute.push(infoLines[item]._Attribute);
                AttributeName.push(infoLines[item]._AttributeName);
                var sqlaa = "select p.name as Product, u.name as UOM from m_product p left outer  join c_uom u on (u.c_uom_id = p.c_uom_id) where p.m_product_id = " + VIS.Utility.Util.getValueOfInt(prodID[item]);
                try {
                    var dr = VIS.DB.executeReader(sqlaa.toString(), null, null);
                    while (dr.read()) {
                        prodName.push(dr.getString(0));
                        uom.push(dr.getString(1));
                    }
                }
                catch (e) {
                }
            }
            if (prodID.length > 0) {
                for (var i = 0; i < prodID.length; i++) {
                    savedProduct.push(
                       {
                           SrNo: i,
                           Product: prodName[i],
                           QtyEntered: qty[i],
                           UOM: uom[i],
                           M_Product_ID1: prodID[i],
                           Attribute: Attribute[i],
                           AttributeName: AttributeName[i],
                           RefNo: ReferenceNo[i]
                       }
                    );
                }
                BindGridSavedProducts();
            }
        };

        function BindGridSavedProducts() {
            dGrid = null;
            var grdRows = [];
            for (var j = 0; j < savedProduct.length; j++) {
                var row = {};
                //grdCol[item] = { field: dynData[item].ColumnName, sortable: true, attr: 'align=center' };
                row["Product"] = savedProduct[j].Product;
                row["QtyEntered"] = savedProduct[j].QtyEntered;
                row["UOM"] = savedProduct[j].UOM;
                row["Attribute"] = savedProduct[j].AttributeName;
                row['recid'] = j + 1;
                grdRows[j] = row;
            }
            grdname = 'gridsavedprd' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(grdRows);
            dGrid = $(datasec).w2grid({
                name: grdname,
                recordHeight: 40,
                show: {

                    toolbar: true,  // indicates if toolbar is v isible
                    columnHeaders: true,   // indicates if columns is visible
                    lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: true,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: false,   // indicates if toolbar delete button is visible
                    toolbarSave: false,   // indicates if toolbar save button is visible
                    selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    recordTitles: false	 // indicates if to define titles for records

                },
                columns: [
                    { field: "Product", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Product") + '</span></div>', sortable: false, size: '20%', min: 200, hidden: false },
                    { field: "QtyEntered", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "QtyEntered") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false, editable: { type: 'float' }, render: 'number:1' },
                    { field: "UOM", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "UOM") + '</span></div>', sortable: false, size: '80px', min: 80, hidden: false },
                    {
                        field: "Attribute", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Attribute") + '</span></div>', sortable: false, size: '20%', min: 150, hidden: false,
                        render: function () {
                            return '<div><input type=text readonly="readonly" style= "width:85%; border:none" ></input><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/MultiX16.png" alt="Attribute Set Instance" title="Attribute Set Instance" style="opacity:1;float:right;"></div>';
                        }
                    },
                    {
                        field: "Delete", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Delete") + '</span></div>', sortable: false, size: '40px', min: 80, hidden: false,
                        render: function () { return '<div style="text-align: center;"><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/delete-ico-hover.png" alt="Delete record" title="Delete record" style="opacity: 1;"></div>'; }
                    },
                    {
                        field: "Insert", caption: '<div style="text-align: center;" ><span>' + VIS.Msg.translate(VIS.Env.getCtx(), "Insert") + '</span></div>', sortable: false, size: '40px', min: 80, hidden: false,
                        render: function () { return '<div style="text-align: center;"><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/add-edit.png" alt="Insert record" title="Insert record" style="opacity: 1;"></div>'; }
                    }
                ],
                records: grdRows,
                //multiSelect: multiSelection,

                onChange: function (event) {
                    w2ui[grdname].records[event.index]["QtyEntered"] = event.value_new;
                    infoLines[event.index]._prodQty = event.value_new;
                    savedProduct[event.index].QtyEntered = event.value_new;
                },
                onClick: function (event) {
                    if (event.column == 4 && dGrid.records.length > 0) {
                        infoLines.splice(event.recid - 1, 1);
                        savedProduct.splice(event.recid - 1, 1);
                        BindGridSavedProducts();
                    }
                    else if (event.column == 5 && dGrid.records.length > 0) {
                        savedProduct.push(
                        {
                            SrNo: savedProduct.length,
                            Product: savedProduct[event.recid - 1].Product,
                            QtyEntered: savedProduct[event.recid - 1].QtyEntered,
                            UOM: savedProduct[event.recid - 1].UOM,
                            M_Product_ID1: savedProduct[event.recid - 1].M_Product_ID1,
                            Attribute: "",
                            AttributeName: "",
                            RefNo: savedProduct[event.recid - 1].RefNo
                        }
                       );
                        infoLines[infoLines.length] = {
                            _prodQty: savedProduct[savedProduct.length - 1].QtyEntered,
                            _prdID: savedProduct[savedProduct.length - 1].M_Product_ID1,
                            _AD_Session_ID: VIS.Env.getCtx().getContext("#AD_Session_ID"),
                            _windowNo: WindowNo,
                            _RefNo: "",
                            _Attribute: "",
                            _AttributeName: "",
                            _Locator_ID: "",
                            _IsLotSerial: "N"
                        };
                        BindGridSavedProducts();
                    }
                    else if (event.column == 3 && dGrid.records.length > 0) {
                        debugger;
                        var productWindow = AD_Column_ID == 8418;		//	HARDCODED
                        var M_Locator_ID = VIS.context.getContextAsInt(WindowNo, "M_Locator_ID");
                        var C_BPartner_ID = VIS.context.getContextAsInt(WindowNo, "C_BPartner_ID");
                        var obj = new VIS.PAttributesForm(VIS.Utility.Util.getValueOfInt(savedProduct[event.recid - 1].Attribute), VIS.Utility.Util.getValueOfInt(savedProduct[event.recid - 1].M_Product_ID1), M_Locator_ID, C_BPartner_ID, productWindow, AD_Column_ID, WindowNo);
                        if (obj.hasAttribute) {
                            obj.showDialog();
                        }
                        obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                            this.M_Locator_ID = mLocatorId;
                            savedProduct[event.recid - 1].AttributeName = name;
                            savedProduct[event.recid - 1].Attribute = mAttributeSetInstanceId;
                            infoLines[event.recid - 1]._Attribute = mAttributeSetInstanceId;
                            infoLines[event.recid - 1]._AttributeName = name;
                            BindGridSavedProducts();
                        };
                    }
                }
            });
            for (var k = 0; k < dGrid.records.length; k++) {
                $("#grid_" + grdname + "_rec_" + dGrid.records[k].recid).find("input[type=text]").val(savedProduct[k].AttributeName);
            }
            bsyDiv[0].style.visibility = "hidden";
        };

        var btnOKClick = function () {
            //debugger;
            if (multiSelection && infoLines.length > 0) {
                multiValues = [];
                refreshUI = true;
                SaveProducts(infoLines);
            }
            else {
                multiValues = [];
                var selection = w2ui[grdname].getSelection();
                for (item in selection) {
                    multiValues.push(w2ui[grdname].get(selection[item])[keyCol]);
                }
            }
            if (self.onClose != null) {
                self.onClose();
            }
            disposeComponent();
        };

        function SaveProducts(pqtyAll) {
            var recordID = "";
            var keycolName = "";
            var lineID = "";
            var lineName = "";
            var M_Locator_ID = 0;
            var M_LocatorTo_ID = 0;
            var ordID = 0;
            var id = 0, lineid = 0;

            M_Locator_ID = VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(WindowNo, 1, "M_Locator_ID"));
            recordID = VIS.context.getContextAsInt(WindowNo, VIS.context.getWindowTabContext(WindowNo, 0, "KeyColumnName"));
            keycolName = VIS.context.getWindowTabContext(WindowNo, 0, "KeyColumnName");

            lineID = VIS.context.getContextAsInt(WindowNo, VIS.context.getWindowTabContext(WindowNo, 1, "KeyColumnName"));
            lineName = VIS.context.getWindowTabContext(WindowNo, 1, "KeyColumnName");

            if (window_ID == 170) {
                M_LocatorTo_ID = VIS.context.getContextAsInt(VIS.context.getWindowTabContext(WindowNo, 1, "M_LocatorTo_ID"));
            }

            id = VIS.Utility.Util.getValueOfInt(recordID);
            lineid = VIS.Utility.Util.getValueOfInt(lineID);


            var prodID = [];
            var qty = [];
            var qtybook = [];
            var Attr = [];
            var RefNo = [];
            var OrdLineID = [];
            var AstID = [];
            var listAst = [];
            var ListLoc = [];
            var IsLotSerial = [];

            for (var item in pqtyAll) {
                prodID.push(infoLines[item]._prdID.toString());
                qty.push(infoLines[item]._prodQty.toString());
                RefNo.push(infoLines[item]._RefNo.toString());
                Attr.push(infoLines[item]._Attribute.toString());
                if (infoLines[item]._Locator_ID > 0) {
                    ListLoc.push(infoLines[item]._Locator_ID.toString());
                }
                else {
                    ListLoc.push(M_Locator_ID.toString());
                }
                if (lineid != 0) {
                    break;
                }
            }
            for (var j = 0; j < Attr.length; j++) {
                var attrID = 0, ordline = 0;
                var qry = "";
                var Asset_ID = 0;
                attrID = VIS.Utility.Util.getValueOfInt(Attr[j]);
                listAst.push(attrID.toString());

                if (window.VAICNT && RefNo[j]) {
                    if (window_ID == 184)          //Material Receipt
                    {
                        qry = "SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND DocumentNo = '" + RefNo[j] + "'";
                        ordID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                        if (window.DTD001) {
                            qry = "SELECT ol.C_OrderLine_ID FROM C_OrderLine ol INNER JOIN C_Order o ON ol.C_Order_ID=o.C_Order_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j] +
                                " AND ol.M_AttributeSetInstance_ID = " + listAst[j] + " AND ol.DTD001_Org_ID = " + VIS.context().GetAD_Org_ID();
                            ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                        }

                        if (ordline == 0) {
                            qry = "SELECT ol.C_OrderLine_ID FROM C_OrderLine ol INNER JOIN C_Order o ON ol.C_Order_ID=o.C_Order_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j] +
                                " AND ol.M_AttributeSetInstance_ID = " + listAst[j];
                            ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                            if (ordline == 0) {
                                qry = "SELECT ol.C_OrderLine_ID FROM C_OrderLine ol INNER JOIN C_Order o ON ol.C_Order_ID=o.C_Order_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j];
                                ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                            }
                        }
                        OrdLineID.push(ordline.toString());
                    }
                    else if (window_ID == 169)       //Shipment (Customer)
                    {
                        qry = "SELECT C_Order_ID FROM C_Order WHERE IsActive = 'Y' AND DocumentNo = '" + RefNo[j] + "'";
                        ordID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));

                        qry = "SELECT ol.C_OrderLine_ID FROM C_OrderLine ol INNER JOIN C_Order o ON ol.C_Order_ID=o.C_Order_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j] +
                            " AND ol.M_AttributeSetInstance_ID = " + listAst[j];
                        ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                        if (ordline == 0) {
                            qry = "SELECT ol.C_OrderLine_ID FROM C_OrderLine ol INNER JOIN C_Order o ON ol.C_Order_ID=o.C_Order_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j];
                            ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                        }
                        OrdLineID.push(ordline.toString());
                    }
                    else if (window_ID == 319)       //Package
                    {

                        if (window.DTD001) {
                            var move = "";
                            if (OrdLineID.length > 0) {
                                for (var z = 0; z < OrdLineID.Count; z++) {
                                    if (OrdLineID[z] > 0) {
                                        if (move) {
                                            move += "," + OrdLineID[z];
                                        }
                                        else {
                                            move += OrdLineID[z];
                                        }
                                    }
                                }
                            }

                            qry = "SELECT ol.M_MovementLine_ID FROM M_MovementLine ol INNER JOIN M_Movement o ON ol.M_Movement_ID=o.M_Movement_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j];
                            if (move) {
                                qry += " AND ol.M_MovementLine_ID Not In ( " + move + ")";
                            }
                            ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                            OrdLineID.push(ordline.toString());
                        }
                    }
                    else if (window_ID == 170)         //Inventory Maove
                    {
                        if (window.DTD001) {
                            var move = "";
                            if (OrdLineID.length > 0) {
                                for (var z = 0; z < OrdLineID.Count; z++) {
                                    if (OrdLineID[z] > 0) {
                                        if (move) {
                                            move += "," + OrdLineID[z];
                                        }
                                        else {
                                            move += OrdLineID[z];
                                        }
                                    }
                                }
                            }
                            qry = "SELECT ol.M_RequisitionLine_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o ON ol.M_Requisition_ID=o.M_Requisition_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j];
                            if (move) {
                                qry += " AND ol.M_RequisitionLine_ID Not In ( " + move + ")";
                            }
                            ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                            OrdLineID.push(ordline.toString());
                            if (Attr[j]) {
                                if (IsLotSerial[j] != "N" && attrID > 0) {
                                    qry = "SELECT A_Asset_ID FROM A_Asset WHERE IsActive = 'Y' AND M_Product_ID = " + prod[j] + " AND M_AttributeSetInstance_ID = " + attrID;
                                    Asset_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                                }
                                else {
                                    qry = "SELECT A_Asset_ID FROM A_Asset WHERE IsActive = 'Y' AND M_Product_ID = " + prod[j] + " AND SerNo = " + Attr[j];
                                    Asset_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                                }
                            }
                            AstID.Add(Asset_ID);
                        }
                    }
                    else if (window_ID == 341)          //Internal Use Inventory
                    {
                        if (window.DTD001) {
                            var move = "";
                            if (OrdLineID.length > 0) {
                                for (var z = 0; z < OrdLineID.Count; z++) {
                                    if (OrdLineID[z] > 0) {
                                        if (move) {
                                            move += "," + OrdLineID[z];
                                        }
                                        else {
                                            move += OrdLineID[z];
                                        }
                                    }
                                }
                            }

                            qry = "SELECT ol.M_RequisitionLine_ID FROM M_RequisitionLine ol INNER JOIN M_Requisition o ON ol.M_Requisition_ID=o.M_Requisition_ID WHERE o.DocumentNo = '" + RefNo[j] + "' AND ol.M_Product_ID = " + prod[j];
                            if (move.Length > 0) {
                                qry += " AND ol.M_RequisitionLine_ID Not In ( " + move + ")";
                            }
                            ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                            OrdLineID.push(ordline.toString());
                        }
                    }
                    else if (window_ID == 168) {
                        qry = "SELECT M_Locator_ID FROM M_Locator WHERE IsActive = 'Y' AND Value = '" + RefNo[j] + "'";
                        M_LocatorTo_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                        if (M_LocatorTo_ID > 0) {
                            M_Locator_ID = M_LocatorTo_ID;
                        }
                    }
                }
                else if (window_ID == 169)       //Shipment (Customer)
                {
                    qry = "SELECT C_Order_ID FROM M_InOut WHERE IsActive = 'Y' AND M_InOut_ID = " + recordID;
                    ordID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));

                    qry = "SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID = " + ordID + " AND M_Product_ID = " + prod[j] +
                        " AND M_AttributeSetInstance_ID = " + listAst[j];
                    ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                    if (ordline == 0) {
                        qry = "SELECT C_OrderLine_ID FROM C_OrderLine WHERE C_Order_ID = " + ordID + " AND M_Product_ID = " + prod[j];
                        ordline = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                    }
                    OrdLineID.push(ordline.toString());
                }
                else {
                    OrdLineID.push(ordline.toString());
                }
                if (window_ID == 168) {
                    qtybook.push(SetQtyBook(attrID, prod[j], M_Locator_ID).toString());
                    AstID.push(Asset_ID.toString());
                }
                else if (window_ID == 341) {
                    qtybook.push("0");
                    qry = "SELECT C_Charge_ID FROM C_Charge WHERE IsActive = 'Y' AND DTD001_ChargeType = 'INV'";
                    Asset_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(qry));
                    AstID.push(Asset_ID.toString());
                }
                else {
                    AstID.push(Asset_ID.toString());
                    qtybook.push("0");
                }
            }
            //debugger;
            if (window_ID == 170 || window_ID == 341) {

                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "InfoProduct/Save1",
                    dataType: "json",
                    data: {
                        id: id,
                        keyColumn: keycolName,
                        prod: JSON.stringify(prodID),
                        listAst: JSON.stringify(listAst),
                        qty: JSON.stringify(qty),
                        ordlineID: JSON.stringify(OrdLineID),
                        listLoc: JSON.stringify(ListLoc),
                        locatorTo: M_LocatorTo_ID,
                        astID: JSON.stringify(AstID),
                        lineID: lineid
                    },
                    success: function (data) {
                        var returnValue = data.result;
                        if (returnValue) {
                            return;
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        //debugger;
                        console.log(textStatus);
                        if (returnValue) {
                            return;
                        }
                    }
                });
            }
            else {
                //debugger;
                $.ajax({
                    type: "POST",
                    url: VIS.Application.contextUrl + "InfoProduct/Save",
                    dataType: "json",
                    data: {
                        id: id,
                        keyColumn: keycolName,
                        prod: JSON.stringify(prodID),
                        listAst: JSON.stringify(listAst),
                        qty: JSON.stringify(qty),
                        qtyBook: JSON.stringify(qtybook),
                        ordlineID: JSON.stringify(OrdLineID),
                        ordID: ordID,
                        listLoc: JSON.stringify(ListLoc),
                        lineID: lineid
                    },
                    success: function (data) {
                        //debugger;
                        var returnValue = data.result;
                        if (returnValue) {
                            return;
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        //debugger;
                        console.log(textStatus);
                        if (returnValue) {
                            return;
                        }
                    }
                });
            }
        }


        function SetQtyBook(M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID) {
            // Set QtyBook from first storage location
            var bd = null;
            var sql = "SELECT QtyOnHand FROM M_Storage "
                + "WHERE M_Product_ID = " + M_Product_ID	//	1
                + " AND M_Locator_ID = " + M_Locator_ID		//	2
                + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            if (M_AttributeSetInstance_ID == 0)
                sql = "SELECT SUM(QtyOnHand) FROM M_Storage "
                + "WHERE M_Product_ID = " + M_Product_ID	//	1
                + " AND M_Locator_ID = " + M_Locator_ID		//	2
            var idr = null;
            idr = VIS.DB.executeReader(sql);
            if (idr.read()) {
                bd = VIS.Utility.Util.getValueOfDecimal(idr.getDecimal(0));
                if (bd != null)
                    return bd.Value;
            }
            else {
                // clear Booked Quantity to zero first in case the query returns no rows, 
                // for example when the locator has never stored a particular product.
                idr.close();
                return 0;
            }
            idr.Close();
            return 0;
        }

        function onClosing() {

            disposeComponent();
            //inforoot.dialog("close");
            //inforoot = null;
        };

        this.show = function () {
            //debugger;
            $.ajax({
                url: VIS.Application.contextUrl + "InfoProduct/GetInfoColumns",
                dataType: "json",
                error: function () {
                    alert(VIS.Msg.getMsg('ERRORGettingSearchCols'));
                    bsyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {

                    displayCols = jQuery.parseJSON(data);
                    if (displayCols == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingSearchCols'));
                        bsyDiv[0].style.visibility = "hidden";
                        return;
                    }

                    //var refreshtxt = VIS.Msg.getMsg("Refresh");
                    //if (refreshtxt.indexOf('&') > -1) {
                    //    refreshtxt = refreshtxt.replace('&', '');
                    //}
                    //var canceltxt = VIS.Msg.getMsg("Cancel");
                    //if (canceltxt.indexOf('&') > -1) {
                    //    canceltxt = canceltxt.replace('&', '');
                    //}
                    //var Oktxt = VIS.Msg.getMsg("Ok");
                    //if (Oktxt.indexOf('&') > -1) {
                    //    Oktxt = Oktxt.replace('&', '');
                    //}
                    inforoot.dialog({
                        width: 1020,
                        height: 500,
                        resizable: false,

                        modal: true
                        ,
                        close: onClosing
                    });
                    displayData(false);
                }
            });
        };

        var displayData = function (requery) {
            //debugger;
            disposeDataSec();
            var sql = "SELECT DISTINCT ";
            //var colName = null;
            //var cname = null;
            var displayType = 0;
            var count = $.makeArray(displayCols).length;
            //get Qry from InfoColumns
            for (var item in displayCols) {
                displayType = displayCols[item].AD_Reference_ID;
                if (displayType == VIS.DisplayType.YesNo) {
                    sql += " ( CASE " + displayCols[item].ColumnSQL + " WHEN 'Y' THEN  'True' ELSE 'False'  END ) " + displayCols[item].ColumnName;
                }
                else {
                    sql += displayCols[item].ColumnSQL + " ";
                }

                if (displayType == VIS.DisplayType.ID) {
                    keyCol = displayCols[item].ColumnName.toUpperCase();
                }

                if (!((count - 1) == item)) {
                    sql += ', ';
                }
            }

            sql += " FROM " + s_productFrom;


            if (requery == true) {
                var whereClause = " rownum <= 30";
                var name = "";
                var value = "";
                var upc = "";
                var sku = "";
                var srchValue = null;

                for (var i = 0; i < srchCtrls.length; i++) {
                    srchValue = srchCtrls[i].Ctrl.getValue();
                    if (srchValue == null || srchValue.length == 0 || srchValue == 0) {
                        continue;
                    }

                    whereClause += " AND ";

                    if (srchCtrls[i].Ctrl.displayType == 10) {
                        if (!(String(srchValue).indexOf("%") == 0)) {
                            srchValue = "●" + srchValue;
                        }
                        else {
                            srchValue = String(srchValue).replace("%", "●");
                        }
                        if (!((String(srchValue).lastIndexOf("●")) == (String(srchValue).length))) {
                            srchValue = srchValue + "●";
                        }
                    }

                    if (srchCtrls[i].Ctrl.colName == "Value") {
                        whereClause += "  UPPER(p." + srchCtrls[i].ColumnName + ") LIKE '" + srchValue.toUpperCase() + "' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "Name") {
                        whereClause += "  UPPER(p." + srchCtrls[i].ColumnName + ") LIKE '" + srchValue.toUpperCase() + "' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "UPC") {
                        whereClause += " (UPPER(patr.UPC) LIKE '" + srchValue.toUpperCase() + "' OR UPPER(p.UPC) LIKE '" + srchValue.toUpperCase() + "' OR UPPER(mr.UPC) LIKE '" + srchValue.toUpperCase() + "')"
                    }

                    else if (srchCtrls[i].Ctrl.colName == "SKU") {
                        whereClause += "  UPPER(p." + srchCtrls[i].ColumnName + ") LIKE '" + srchValue.toUpperCase() + "' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "M_Warehouse_ID") {
                        if (VIS.Utility.Util.getValueOfInt(srchValue) != 0) {
                            whereClause += " w.M_Warehouse_ID=" + VIS.Utility.Util.getValueOfInt(srchValue);
                        }
                    }
                    else if (srchCtrls[i].Ctrl.colName == "M_PriceList_Version_ID") {
                        if (VIS.Utility.Util.getValueOfInt(srchValue) != 0) {
                            whereClause += " pr.M_PriceList_Version_ID=" + VIS.Utility.Util.getValueOfInt(srchValue);
                        }
                    }
                }

                if (whereClause.length > 1) {
                    sql += " WHERE " + whereClause;
                    if (validationCode != null && validationCode.length > 0) {
                        sql += " AND " + validationCode.replace(/M_Product\./g, "p.");
                        //sql += " AND " + validationCode;
                    }
                }
                else if (validationCode != null && validationCode.length > 0) {
                    sql += " WHERE " + validationCode.replace(/M_Product\./g, "p.");
                }

                if (window_ID == 181) {
                    validationCode += " AND (p.Discontinued = 'N' OR p.DiscontinuedBy > sysdate )";
                }
            }
            else {
                if (validationCode.length > 0 && validationCode.trim().toUpperCase().startsWith('WHERE')) {
                    sql += " " + validationCode.replace(/M_Product\./g, "p.") + " AND " + tableName + "_ID=-1";
                }
                else if (validationCode.length > 0) {
                    sql += " WHERE p." + tableName + "_ID=-1 AND " + validationCode.replace(/M_Product\./g, "p.");
                }
                else {
                    sql += " WHERE p." + tableName + "_ID=-1";
                }
            }


            $.ajax({
                url: VIS.Application.contextUrl + "InfoProduct/GetData",
                dataType: "json",
                type: "POST",
                //async: false,
                data: {
                    sql: sql,
                    tableName: tableName
                },
                error: function () {
                    alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    bsyDiv[0].style.visibility = 'hidden';
                    return;
                },
                success: function (dyndata) {
                    loadData(JSON.parse(dyndata));
                }
            });
        };

        var loadData = function (dynData) {

            if (dynData == null) {
                alert(VIS.Msg.getMsg('NoDataFound'));
                bsyDiv[0].style.visibility = "hidden";
                return;
            }
            var grdCols = [];
            var grdRows = [];
            var rander = null;
            var displayType = null;

            for (var item in dynData) {

                var oColumn = {

                    resizable: true
                }
                displayType = displayCols[item].AD_Reference_ID;

                oColumn.caption = displayCols[item].Name;
                oColumn.field = displayCols[item].ColumnName.toUpperCase();
                oColumn.sortable = true;
                //oColumn.hidden = !displayCols[item].IsDisplayed;
                oColumn.size = '100px';

                if (VIS.DisplayType.IsNumeric(displayType)) {

                    if (displayType == VIS.DisplayType.Integer) {
                        oColumn.render = 'int';
                    }
                    else if (displayType == VIS.DisplayType.Amount) {
                        oColumn.render = 'number:2';
                    }
                    else {
                        oColumn.render = 'number:1';
                    }
                    if (displayCols[item].ColumnName.toUpperCase() == 'QTYENTERED') {
                        if (multiSelection && !updating) {
                            oColumn.editable = {
                                type: 'float'
                            }
                        }
                    }
                }
                    //	YesNo
                    //else if (displayType == VIS.DisplayType.YesNo) {

                    //    oColumn.render = function (record, index, colIndex) {

                    //        var chk = (record[grdCols[colIndex].field]) == 'True' ? "checked" : "";

                    //        return '<input type="checkbox" ' + chk + ' disabled="disabled" >';
                    //    }
                    //}

                    //Date /////////
                else if (VIS.DisplayType.IsDate(displayType)) {
                    oColumn.render = function (record, index, colIndex) {

                        var d = record[grdCols[colIndex].field];
                        if (d) {
                            d = Globalize.format(new Date(d), 'd');
                        }
                        else d = "";
                        return d;

                    }
                }

                else if (displayType == VIS.DisplayType.Location || displayType == VIS.DisplayType.Locator) {
                    oColumn.render = 'int';
                }

                else if (displayType == VIS.DisplayType.Account) {
                    oColumn.render = 'int';
                }

                else if (displayType == VIS.DisplayType.PAttribute) {
                    oColumn.render = 'int';
                }

                else if (displayType == VIS.DisplayType.Button) {
                    oColumn.render = function (record) {
                        return '<div>button</div>';
                    }
                }

                else if (displayType == VIS.DisplayType.Image) {

                    oColumn.render = function (record) {
                        return '<div>Image</div>';
                    }
                }

                else if (VIS.DisplayType.IsLOB(displayType)) {

                    oColumn.render = function (record) {
                        return '<div>[Lob-blob]</div>';
                    }
                }



                grdCols[item] = oColumn;

                //grdCols[item] = { field: dynData[item].ColumnName, caption: schema[item].Name, hidden: !schema[item].IsDisplayed, sortable: true, size: '100px' };

            }


            for (var j = 0; j < dynData[0].RowCount && j <= 30; j++) {
                var row = {};
                for (var item in dynData) {
                    //grdCol[item] = { field: dynData[item].ColumnName, sortable: true, attr: 'align=center' };
                    row[dynData[item].ColumnName] = dynData[item].Values[j];
                }
                row['recid'] = j + 1;
                grdRows[j] = row;
            }
            grdname = 'gridPrdInfodata' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(grdRows);
            dGrid = $(datasec).w2grid({
                name: grdname,
                recordHeight: 40,
                show: {

                    toolbar: true,  // indicates if toolbar is v isible
                    columnHeaders: true,   // indicates if columns is visible
                    lineNumbers: true,  // indicates if line numbers column is visible
                    selectColumn: true,  // indicates if select column is visible
                    toolbarReload: false,   // indicates if toolbar reload button is visible
                    toolbarColumns: true,   // indicates if toolbar columns button is visible
                    toolbarSearch: false,   // indicates if toolbar search controls are visible
                    toolbarAdd: false,   // indicates if toolbar add new button is visible
                    toolbarDelete: false,   // indicates if toolbar delete button is visible
                    toolbarSave: false,   // indicates if toolbar save button is visible
                    selectionBorder: false,	 // display border arround selection (for selectType = 'cell')
                    recordTitles: false	 // indicates if to define titles for records

                },
                columns: grdCols,
                records: grdRows,
                //multiSelect: multiSelection,

                onChange: function (event) {
                    var chk = -1;
                    w2ui[grdname].records[event.index]["QtyEntered"] = event.value_new;
                    if (multiValues.length > 0) {
                        for (var item in multiValues) {
                            if (multiValues[item].recid == w2ui[grdname].records[event.index].recid) {
                                chk = item;
                                break;
                            }
                        }
                        if (chk > -1) {
                            multiValues[event.index].QtyEntered = event.value_new;
                        }
                        else {
                            multiValues.push(w2ui[grdname].records[event.index]);
                        }
                    }
                    else {
                        multiValues.push(w2ui[grdname].records[event.index]);
                    }
                }
            });
            w2ui[grdname].hideColumn('M_PRODUCT_ID');
            if (!multiSelection || updating) {
                w2ui[grdname].hideColumn('QtyEntered');
            }
            // $("#w2ui-even").css("height", "40px");
            bsyDiv[0].style.visibility = "hidden";
        };

        var disposeDataSec = function () {
            if (dGrid != null) {
                dGrid.destroy();
            }
            dGrid = null;

        };

        this.getSelectedValues = function () {
            return multiValues;
        };

        this.getRefreshStatus = function () {
            return refreshUI;
        };
        var saveSelection = function () {
            if (multiSelection && multiValues.length > 0) {
                GetSelectedRowKeys();
            }
            else {
                return;
            }

        };

        var GetSelectedRowKeys = function () {
            for (item in multiValues) {
                var qty = multiValues[item].QtyEntered;
                if (qty > 0) {
                    infoLines.push(
                        {
                            _prodQty: qty,
                            _prdID: multiValues[item].M_PRODUCT_ID,
                            _AD_Session_ID: VIS.Env.getCtx().getContext("#AD_Session_ID"),
                            _windowNo: WindowNo,
                            _RefNo: "",
                            _Attribute: "",
                            _AttributeName: "",
                            _Locator_ID: "",
                            _IsLotSerial: "N"
                        }
                    );
                }
            }
        };

        var disposeComponent = function () {

            //   btnExpander.off('click');
            inforoot.off('keyup');
            btnCancel.off("click");
            btnOK.off("click");
            btnRequery.off("click");

            datasec = null;
            searchSec = null;

            if (dGrid != null) {
                dGrid.destroy();
            }
            dGrid = null;

            isExpanded = null;
            subroot = null;
            sSContainer = null;
            ssHeader = null;
            btnExpander = null;
            searchTab = null;
            searchSec = null;
            datasec = null;
            btnsec = null;
            refreshtxt = null;
            canceltxt = null;
            Oktxt = null;
            divbtnRight = null;
            btnCancel = null;
            btnOK = null;
            divbtnLeft = null;
            btnRequery = null;
            bsyDiv = null;
            schema = null;
            srchCtrls = null;
            displayCols = null;
            dGrid = null;
            self = null;
            keyCol = null;
            multiValues = null;
            singlevalue = null;
            infoLines = null;
            savedProduct = null;
            refreshUI = false;
            if (inforoot != null) {
                // inforoot.dialog('close');
                inforoot.dialog('destroy');
                inforoot.remove();
            }
            inforoot = null;

            this.btnOKClick = null;
            this.displaySearchCol = null;
            this.displayData = null;
            this.loadData = null;
            this.disposeComponent = null;
            this.disposeDataSec = null;
            this.getSelectedValues = null;
            this.refreshUI = null;

        };

    };
    VIS.infoProduct = infoProduct;
})(VIS, jQuery);