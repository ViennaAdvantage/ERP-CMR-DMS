
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate info dialog for tables which do not have any respective entry in Info WIndow .
 * Author         :     Lakhwinder
 * Date           :     10-Aug-2014
  ******************************************************/
; (function (VIS, $) {
    function infoGeneral(modal, WindowNo, value, tableName, keyColumn, multiSelection, validationCode) {

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
        var canceltxt = null;
        var Oktxt = null;
        var divbtnRight = null;
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
        var multiValues = [];
        var singlevalue = null;
        var grdname = null;

        function initializeComponent() {
            inforoot.css("width", "100%");
            inforoot.css("height", "100%");
            inforoot.css("position", "relative");
            subroot.css("width", "98%");
            subroot.css("height", "97%");
            subroot.css("position", "absolute");
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

            refreshtxt = VIS.Msg.getMsg("Refresh");
            if (refreshtxt.indexOf('&') > -1) {
                refreshtxt = refreshtxt.replace('&', '');
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
            if (VIS.Application.isRTL) {
                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(Oktxt);
            }
            else {
                btnCancel = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append(canceltxt);
                btnOK = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;margin-right:5px'>").append(Oktxt);
            }

            divbtnRight.append(btnCancel);
            divbtnRight.append(btnOK);
            btnsec.append(divbtnRight);


            divbtnLeft = $("<div style='float:left;'>");
            btnRequery = $("<button class='VIS_Pref_btn-2' style='margin-top: 5px;margin-bottom: -10px;'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png'>"))//.append(refreshtxt);


            if (VIS.Application.isRTL) {
                divbtnLeft.append(btnCancel);
                divbtnLeft.append(btnOK);
                divbtnRight.append(btnRequery);
            }
            else {
                divbtnRight.append(btnCancel);
                divbtnRight.append(btnOK);
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

        function bindEvent() {
            //btnExpander.on('click', function () {
            //    //sSContainer.css("width", '4%');
            //    //datasec.css("width", '95%');
            //    //searchSec.hide();
            //    //$(document.getElementsByName('gridInfodata')).css("width", '95%');

            //    if (isExpanded) {
            //        btnExpander.animate({ borderSpacing: 180 }, {
            //            step: function (now, fx) {
            //                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
            //                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
            //                $(this).css('transform', 'rotate(' + now + 'deg)');
            //            },
            //            duration: 'slow'
            //        }, 'linear');

            //        sSContainer.animate({ width: '4%' }, "slow");
            //        searchSec.hide();
            //        //datasec.animate({ width: "94%" }, "slow");
            //        //datasec.css("width", '94.8%');
            //        //dGrid.animate({ width: "94%" }, "fast");               

            //        // $(window).trigger('resize');
            //        // $(document.getElementsByName('gridInfodata')).css('width','100%')
            //        datasec.animate({ width: "94%" }, "slow", null, function () {
            //            dGrid.resize();
            //            dGrid.refresh();
            //        });

            //    }
            //    else {
            //        btnExpander.animate({ borderSpacing: 0 }, {
            //            step: function (now, fx) {
            //                $(this).css('-webkit-transform', 'rotate(' + now + 'deg)');
            //                $(this).css('-moz-transform', 'rotate(' + now + 'deg)');
            //                $(this).css('transform', 'rotate(' + now + 'deg)');
            //            },
            //            duration: 'slow'
            //        }, 'linear');

            //        // datasec.css("width", '76%');
            //        searchSec.show();
            //        datasec.animate({ width: "75%" }, "slow");
            //        sSContainer.animate({ width: '23%' }, "slow");
            //    }
            //    isExpanded = !isExpanded;
            //});

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
            btnCancel.on("click", function () {

                disposeComponent();

            });
            btnOK.on("click", function () {
                btnOKClick();
            });
            btnRequery.on("click", function () {
                //debugger;
                bsyDiv[0].style.visibility = 'visible';
                if (validationCode != null && validationCode != undefined && validationCode.length > 0) {
                    validationCode = VIS.Env.parseContext(VIS.Env.getCtx(), WindowNo, validationCode, false, false);
                }
                displayData(true);
            });
        };

        bindEvent();

        var btnOKClick = function () {
            //debugger;
            //if (multiSelection) {
            var selection = w2ui[grdname].getSelection();
            for (item in selection) {
                multiValues.push(w2ui[grdname].get(selection[item])[keyCol]);
            }

            if (self.onClose != null && multiValues.length > 0) {
                self.onClose();
            }
            disposeComponent();

        };

        function onClosing() {

            disposeComponent();
            //inforoot.dialog("close");
            //inforoot = null;
        };

        this.show = function () {
            // debugger;
            $.ajax({
                url: VIS.Application.contextUrl + "InfoGeneral/GetSearchColumns/?tableName=" + tableName,
                dataType: "json",
                error: function () {
                    alert(VIS.Msg.getMsg('ERRORGettingSearchCols'));
                    bsyDiv[0].style.visibility = "hidden";
                },
                success: function (data) {

                    schema = data.result;
                    if (schema == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingSearchCols'));
                        bsyDiv[0].style.visibility = "hidden";
                        return;
                    }
                    displaySearchCol();
                }
            });

        };

        var displaySearchCol = function () {


            displayCols = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "InfoGeneral/GetDispalyColumns", { "AD_Table_ID": schema[0].AD_Table_ID })).result;

            if (displayCols == null) {
                alert(VIS.Msg.getMsg('ERRORGettingDisplayCols'));
                bsyDiv[0].style.visibility = "hidden";
                return;
            }
            var label = null;
            var ctrl = null;
            var tableSArea = $("<table>");
            tableSArea.css("width", "100%");
            var tr = null;
            //var rowctrl = 0;
            for (var item in schema) {

                tr = $("<tr>");
                tableSArea.append(tr);

                var td = $("<td>");
                label = new VIS.Controls.VLabel(schema[item].Name, schema[item].ColumnName);
                td.append(label.getControl().css("margin-bottom", "0px").css("margin-top", "5px").css("font-weight", "inherit"));
                tr.append(td);

                tr = $("<tr>");
                tableSArea.append(tr);
                var srchCtrl = {
                };

                ctrl = new VIS.Controls.VTextBox(schema[item].ColumnName, false, false, true, 50, 100, null, null, false);// getControl(schema[item].AD_Reference_ID, schema[item].ColumnName, schema[item].Name, schema[item].AD_Reference_Value_ID, schema[item].lookup);
                srchCtrl.Ctrl = ctrl;
                srchCtrl.AD_Reference_ID = 10;
                srchCtrl.ColumnName = schema[item].ColumnName;
                if (schema[item].IsIdentifier) {
                    ctrl.setValue(value);
                }
                var tdctrl = $("<td>");
                tr.append(tdctrl);
                tdctrl.append(ctrl.getControl().css("width", "97%").css("font-weight", "inherit"));

                srchCtrls.push(srchCtrl);
            }

            searchSec.append(tableSArea);
            var refreshtxt = VIS.Msg.getMsg("Refresh");
            if (refreshtxt.indexOf('&') > -1) {
                refreshtxt = refreshtxt.replace('&', '');
            }
            var canceltxt = VIS.Msg.getMsg("Cancel");
            if (canceltxt.indexOf('&') > -1) {
                canceltxt = canceltxt.replace('&', '');
            }
            var Oktxt = VIS.Msg.getMsg("Ok");
            if (Oktxt.indexOf('&') > -1) {
                Oktxt = Oktxt.replace('&', '');
            }
            inforoot.dialog({
                width: 1020,
                height: 500,
                resizable: false,

                modal: true
                //,
                //buttons: [
                //     {
                //         text: refreshtxt,
                //         click: function () {

                //             bsyDiv[0].style.visibility = 'visible';                           
                //             displayData(true);

                //         },
                //         style: "float:left;margin:0px;margin-right:5px;margin-bottom:-5px"
                //         //class: "VIS_Pref_pass-btn"
                //     },
                //     {
                //         text: Oktxt,
                //         click: function () {
                //             btnOKClick();
                //         },
                //         style: "margin:0px;margin-right:5px;margin-bottom:-5px"
                //         //class: "VIS_Pref_pass-btn"
                //     },
                //     {
                //         text: canceltxt,
                //         click: function () {
                //             disposeComponent();
                //             inforoot.dialog("close");
                //             inforoot = null;
                //         },
                //         style: "margin:0px;margin-right:5px;margin-bottom:-5px"
                //         //class: "VIS_Pref_pass-btn"
                //     }
                //]
                ,
                close: onClosing
            });
            displayData(false);


        };

        var displayData = function (requery) {

            disposeDataSec();
            var sql = "SELECT ";
            //var colName = null;
            //var cname = null;
            var displayType = 0;
            var count = $.makeArray(displayCols).length;
            //get Qry from InfoColumns
            for (var item in displayCols) {


                displayType = displayCols[item].AD_Reference_ID;
                if (displayType == VIS.DisplayType.YesNo) {
                    sql += " ( CASE " + displayCols[item].ColumnName + " WHEN 'Y' THEN  'True' ELSE 'False'  END ) AS " + (displayCols[item].ColumnName);
                }
                else if (displayType == VIS.DisplayType.List) {

                    var refList = displayCols[item].RefList;
                    sql += (" CASE ");
                    for (var refListItem in refList) {
                        sql += " WHEN " + displayCols[item].ColumnName + "='" + refList[refListItem].Key + "' THEN '" + refList[refListItem].Value.replace("'", "''") + "'";
                    }
                    sql += " END AS " + displayCols[item].ColumnName;

                }
                else {
                    sql += displayCols[item].ColumnName + " ";
                }

                if (displayCols[item].IsKey) {
                    keyCol = displayCols[item].ColumnName.toUpperCase();
                }


                if (!((count - 1) == item)) {
                    sql += ', ';
                }

            }

            sql += " FROM " + tableName;


            if (requery == true) {
                var whereClause = " ";
                var srchValue = null;
                var appendAND = false;
                for (var i = 0; i < srchCtrls.length; i++) {
                    srchValue = srchCtrls[i].Ctrl.getValue();
                    if (srchValue == null || srchValue.length == 0 || srchValue == 0) {
                        continue;
                    }

                    if (appendAND == true) {
                        whereClause += " AND ";
                    }

                    if (!(String(srchValue).indexOf("%") == 0)) {
                        srchValue = "●" + srchValue;
                    }
                    else {
                        srchValue = String(srchValue).replace("%", "●");
                    }
                    if (!((String(srchValue).lastIndexOf("●")) == (String(srchValue).length))) {
                        srchValue = srchValue + "●";
                    }
                    whereClause += "  UPPER(" + srchCtrls[i].ColumnName + ") LIKE '" + srchValue.toUpperCase() + "' ";
                    appendAND = true;
                }

                if (whereClause.length > 1) {
                    sql += " WHERE " + whereClause;
                    if (validationCode != null && validationCode.length > 0) {
                        sql += " AND " + validationCode;
                    }
                }
                else if (validationCode != null && validationCode.length > 0) {
                    sql += " WHERE " + validationCode;
                }
            }
            else {
                if (validationCode.length > 0 && validationCode.trim().toUpperCase().startsWith('WHERE')) {
                    sql += " " + validationCode + " AND " + tableName + "_ID=-1";
                }
                else if (validationCode.length > 0) {
                    sql += " WHERE " + tableName + "_ID=-1 AND " + validationCode;
                }
                else {
                    sql += " WHERE " + tableName + "_ID=-1";
                }
            }


            $.ajax({
                url: VIS.Application.contextUrl + "InfoGeneral/GetData",
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
                oColumn.hidden = !displayCols[item].IsDisplayed;
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
                    row[dynData[item].ColumnName.toUpperCase()] = dynData[item].Values[j];
                }
                row['recid'] = j + 1;
                grdRows[j] = row;
            }
            grdname = 'gridGenInfodata' + Math.random();
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
                multiSelect: multiSelection
            });

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


        };


    };
    VIS.infoGeneral = infoGeneral;
})(VIS, jQuery);