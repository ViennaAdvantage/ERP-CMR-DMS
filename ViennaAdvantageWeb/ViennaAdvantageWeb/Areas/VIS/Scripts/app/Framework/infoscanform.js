
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Product info Scan Form.
 * Author         :     Bharat
 * Date           :     27-May-2015
  ******************************************************/
; (function (VIS, $) {

    function InfoScanForm(windowNo) {
        debugger;
        this.onClose = null;
        this.arrListColumns = [];
        var inforoot = $("<div>");
        var isExpanded = true;
        var subroot = $("<div>");
        var searchTab = null;
        var searchSec = null;
        var datasec = null;
        var btnsec = null;
        var searchtxt = null;
        var canceltxt = null;
        var Oktxt = null;
        var divbtnRight = null;
        var divbtnsec = null;
        var btnCancel = null;
        var btnOK = null;
        var btnSearch = null;
        var divbtnLeft = null;
        var bsyDiv = null;
        var dGrid = null;
        var self = this;
        var window_ID = 0;
        var grdname = null;
        var lblNametxt = null;
        var lblReftxt = null;
        var lblDatetxt = null;
        var label = null;
        var ctrl = null;
        var tableSArea = $("<table>");
        var tr = null;
        var refreshUI = false;
        var srchCtrls = [];
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
                searchTab = $("<div style='background-color: rgb(241, 241, 241);height:88.9%;display: inline-block;width:25%;height:87.8%;overflow:auto;padding-right: 10px;margin-left: 10px;'>");
                searchSec = $("<div style='background-color: rgb(241, 241, 241);display: inline-block;height:87.8%;'>");
                searchTab.append(searchSec);
                datasec = $("<div style='display: inline-block;width:73%;height:87.8%;overflow:auto;'>");
                btnsec = $("<div style='display: inline-block;width:99%;height:auto;margin-top: 2px;'>");
            }
            else {
                searchTab = $("<div style='background-color: rgb(241, 241, 241);padding-left: 7px;height:88.9%;display: inline-block; float: left;width:25%;height:87.8%;overflow:auto;'>");
                searchSec = $("<div style='background-color: rgb(241, 241, 241);'>");
                searchTab.append(searchSec);
                datasec = $("<div style='display: inline-block; float: left;width:73%;height:87.8%;margin-left:10px;'>");
                btnsec = $("<div style='display: inline-block; float: left;width:99%;height:auto;margin-top: 2px;'>");
            }

            subroot.append(searchTab);
            subroot.append(datasec);
            subroot.append(btnsec);

            lblNametxt = VIS.Msg.translate(VIS.Env.getCtx(), "Name")
            if (lblNametxt.indexOf('&') > -1) {
                lblNametxt = lblNametxt.replace('&', '');
            }
            lblReftxt = VIS.Msg.translate(VIS.Env.getCtx(), "ReferenceNo");
            if (lblReftxt.indexOf('&') > -1) {
                lblReftxt = lblUPCtxt.replace('&', '');
            }
            lblDatetxt = VIS.Msg.translate(VIS.Env.getCtx(), "DateTrx");
            if (lblDatetxt.indexOf('&') > -1) {
                lblDatetxt = lblDatetxt.replace('&', '');
            }

            canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }

            searchtxt = VIS.Msg.getMsg("Search");
            if (searchtxt.indexOf('&') > -1) {
                searchtxt = searchtxt.replace('&', '');
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
                btnSearch = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:10px'>").append(searchtxt);
            }
            else {

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
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblReftxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox("Reference", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "Reference";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));
                srchCtrls.push(srchCtrl);

                tr = $("<tr>");
                tableSArea.append(tr);
                var td = $("<td>");
                label = $("<label class='VIS_Pref_Label_Font' style='margin-bottom: 0px;margin-top:5px;font-weight:inherit'>").append(lblDatetxt);
                td.append(label);
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                //ctrl = new VIS.Controls.VTextBox("TrxDate", false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                ctrl = new VIS.Controls.VDate("TrxDate", false, false, true, VIS.DisplayType.Date, VIS.Msg.translate("DateTrx"));
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = "TrxDate";
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit").css("height", "30px"));
                //tdctrl.append(ctrl.getControl().addClass("vis-allocation-date"));
                srchCtrls.push(srchCtrl);

                searchSec.append(tableSArea);

                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(Oktxt);
                btnSearch = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:10px'>").append(searchtxt);
            }

            divbtnRight.append(btnCancel);
            divbtnRight.append(btnOK);
            divbtnsec.append(btnSearch);
            searchTab.append(divbtnsec);
            btnsec.append(divbtnRight);

            if (VIS.Application.isRTL) {
                divbtnLeft.append(btnCancel);
                divbtnLeft.append(btnOK);
                divbtnsec.append(btnSearch);
                searchTab.append(divbtnsec);
            }
            else {
                divbtnRight.append(btnCancel);
                divbtnRight.append(btnOK);
                divbtnsec.append(btnSearch);
                searchTab.append(divbtnsec);
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

        function bindEvent() {
            if (!VIS.Application.isMobile) {
                inforoot.on('keyup', function (e) {
                    if (!(e.keyCode === 13)) {
                        return;
                    }
                    bsyDiv[0].style.visibility = 'visible';
                    displayData(true);
                });
            }

            btnCancel.on("click", function () {
                disposeComponent();
            });

            btnOK.on("click", function () {
                btnOKClick();
            });

            btnSearch.on("click", function () {
                bsyDiv[0].style.visibility = 'visible';
                displayData(true);
            });
        };

        bindEvent();

        var displayData = function (requery) {

            if (requery == true) {
                var query = "";
                var whereClause = " rownum <= 30 AND";
                var name = "";
                var ref = "";
                var date = "";
                var srchValue = null;
                var drInv = null;
                var data = [];
                var sql = "SELECT AD_Window_ID FROM AD_Tab WHERE AD_Tab_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNo, 0, "AD_Tab_ID"));
                window_ID = VIS.Utility.Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));

                for (var i = 0; i < srchCtrls.length; i++) {
                    srchValue = srchCtrls[i].Ctrl.getValue();
                    if (srchValue == null || srchValue.length == 0 || srchValue == 0) {
                        continue;
                    }

                    if (srchCtrls[i].Ctrl.colName == "Name") {
                        query += " AND upper(VAICNT_ScanName) LIKE '" + srchValue.toUpperCase() + "' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "Reference") {
                        query += " AND upper(VAICNT_ReferenceNo) LIKE '" + srchValue.toUpperCase() + "' ";
                    }

                    else if (srchCtrls[i].Ctrl.colName == "TrxDate") {
                        query += " AND to_char(TO_DATE(DateTrx,'DD-MM-YYYY')) = to_char(" + Globalize.format(new Date(srchValue), true) + ")";
                    }
                }

                if (window_ID == 184) {
                    query += " AND VAICNT_TransactionType = 'MR' and VAICNT_ReferenceNo in (SELECT DocumentNo from C_Order WHERE  C_BPartner_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNo, 0, "C_BPartner_ID")) + ")";
                }
                else if (window_ID == 319 || window_ID == 170) {
                    query += " AND VAICNT_TransactionType = 'IM' ";
                }
                else if (window_ID == 168) {
                    query += " AND VAICNT_TransactionType = 'PI' ";
                }
                else if (window_ID == 169) {
                    query += " AND VAICNT_TransactionType = 'SH' and VAICNT_ReferenceNo in (SELECT DocumentNo from C_Order WHERE  C_BPartner_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getWindowTabContext(windowNo, 0, "C_BPartner_ID")) + ")";
                }
                else if (window_ID == 341) {
                    query += " AND VAICNT_TransactionType = 'IU' ";
                }

            }
            else {
                query += " AND VAICNT_InventoryCount_ID = -1";
            }

            sql = "SELECT VAICNT_ScanName,VAICNT_ReferenceNo,DateTrx,VAICNT_InventoryCount_ID FROM VAICNT_InventoryCount WHERE IsActive='Y' AND AD_Client_ID = " + VIS.Utility.Util.getValueOfInt(VIS.context.getAD_Client_ID()) + query;
            try {
                drInv = VIS.DB.executeReader(sql, null, null);
                var count = 1;
                while (drInv.read()) {
                    var line = {};
                    line['Name'] = drInv.getString("VAICNT_ScanName");
                    line['Reference'] = drInv.getString("VAICNT_ReferenceNo");
                    line['TrxDate'] = drInv.getString("DateTrx");
                    line['count_ID'] = drInv.getInt("VAICNT_InventoryCount_ID");
                    line['recid'] = count;
                    data.push(line);
                    count++;
                }
            }
            catch (e) {

            }

            loadGrid(data);
        };

        var loadGrid = function (data) {

            if (data == null) {
                bsyDiv[0].style.visibility = "hidden";
                return;
            }
            if (dGrid != null) {
                dGrid.destroy();
                dGrid = null;
            }

            dGrid = null;
            self.arrListColumns = [];

            self.arrListColumns.push({ field: "Name", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Name"), sortable: true, size: '16%', min: 150, hidden: false });
            self.arrListColumns.push({ field: "Reference", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ReferenceNo"), sortable: true, size: '16%', min: 150, hidden: false });
            self.arrListColumns.push({ field: "TrxDate", caption: VIS.Msg.translate(VIS.Env.getCtx(), "DateTrx"), sortable: true, size: '16%', min: 150, hidden: false, render: 'date' });
            self.arrListColumns.push({ field: "count_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ID"), sortable: true, size: '16%', min: 150, hidden: true });
            var grdname = 'gridPrdInfodata' + Math.random();
            grdname = grdname.replace('.', '');
            w2utils.encodeTags(data);
            dGrid = $(datasec).w2grid({
                name: grdname,
                recordHeight: 30,
                columns: self.arrListColumns,
                records: data,
                onClick: function (event) {
                    if (dGrid.records.length > 0) {
                        btnOK.removeAttr("disabled");
                    }
                }
            });
            bsyDiv[0].style.visibility = "hidden";
        };

        var btnOKClick = function () {
            debugger;
            var recid = -1;            
            if (dGrid != null) {
                recid = Number(dGrid.getSelection().toString());
                if (recid > 0) {
                    var countID = dGrid.get(recid).count_ID;
                    var _query = "select cnt.M_Product_ID,cnt.vaicnt_quantity,cnt.vaicnt_attributeno,ats.Islot,ats.IsSerNo,ats.IsGuaranteeDate,case when (ats.IsGuaranteeDate = 'Y') then sysdate+p.GuaranteeDays end as ExpiryDate,cnt.VAICNT_ReferenceNo from (SELECT CASE WHEN (cl.upc = mr.upc) THEN mr.M_product_ID"
                    + " ELSE CASE WHEN (cl.upc = prd.upc) THEN prd.M_Product_ID ELSE CASE WHEN (cl.upc = patr.upc) THEN patr.M_Product_ID END END END AS M_Product_ID,cl.vaicnt_quantity,cl.vaicnt_attributeno,ic.VAICNT_ReferenceNo FROM VAICNT_InventoryCount ic INNER JOIN VAICNT_InventoryCountLine cl ON (ic.VAICNT_InventoryCount_ID = cl.VAICNT_InventoryCount_ID)"
                    + " LEFT JOIN M_manufacturer mr ON cl.upc = mr.upc LEFT JOIN M_product prd ON cl.upc = prd.upc LEFT JOIN M_ProductAttributes patr ON cl.upc = patr.upc WHERE ic.IsActive = 'Y' AND cl.IsActive = 'Y' AND (cl.upc = mr.upc OR cl.upc = prd.upc OR cl.upc = patr.upc) AND ic.ad_client_id = " + VIS.context.getAD_Client_ID() +
                    " AND ic.VAICNT_InventoryCount_ID = " + countID + " ) cnt inner join M_Product p on cnt.M_Product_ID=p.M_Product_ID left join M_attributeset ats on p.M_attributeset_id=ats.M_attributeset_id";
                    var drProd = VIS.DB.executeReader(_query, null, null);
                    if (self.onClose)
                        self.onClose(drProd);
                    inforoot.dialog('close');
                }
                else {
                    alert("Please Select a Record");
                }
            }
        };

        this.showDialog = function () {
            inforoot.dialog({
                modal: true,
                title: VIS.Msg.translate(VIS.Env.getCtx(), "Info Scan"),
                width: 800,
                height: 450,
                resizable: false,
                close: function () {
                    self.dispose();
                }
            });
            displayData(false);
        };

        var disposeComponent = function () {

            inforoot.off('keyup');
            btnCancel.off("click");
            btnOK.off("click");
            btnSearch.off("click");
            datasec = null;
            searchSec = null;

            if (dGrid != null) {
                dGrid.destroy();
            }
            dGrid = null;

            isExpanded = null;
            subroot = null;
            searchTab = null;
            btnsec = null;
            searchtxt = null;
            canceltxt = null;
            Oktxt = null;
            divbtnRight = null;
            btnCancel = null;
            btnOK = null;
            divbtnLeft = null;
            bsyDiv = null;
            srchCtrls = null;
            self = null;
            refreshUI = false;
            if (inforoot != null) {
                // inforoot.dialog('close');
                inforoot.dialog('destroy');
                inforoot.remove();
            }
            inforoot = null;

            //this.btnOKClick = null;
            this.displayData = null;
            this.disposeComponent = null;
        };
    };

    //dispose call
    InfoScanForm.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    VIS.InfoScanForm = InfoScanForm;

})(VIS, jQuery);
