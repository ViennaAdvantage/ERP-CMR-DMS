; (function (VIS, $) {

    var MUserQuery = {

        getData: function (ctx, AD_Tab_ID, AD_Table_ID, valueColumnName) {
            var AD_Client_ID = ctx.getAD_Client_ID();
            var dr = null;
            var sql = "SELECT NAME," + valueColumnName + ", AD_UserQuery_ID FROM AD_UserQuery WHERE"
                + " AD_Client_ID=" + AD_Client_ID + " AND IsActive='Y'"
                + " AND (AD_Tab_ID=" + AD_Tab_ID + " OR AD_Table_ID=" + AD_Table_ID + ")"
                + " ORDER BY AD_UserQuery_ID";
            try {
                dr = VIS.DB.executeDataReader(sql, null, null);
            }
            catch (ex) {
                if (dr != null) {
                    dr = null;
                }
                //_log.Log(Level.SEVERE, sql, ex);
            }
            return dr;
        },

        getQueryLines: function (AD_UserQuery_ID) {
            var dr = null;
            var lines = [];
            var sql = "SELECT KEYNAME,KEYVALUE,OPERATOR AS OPERATORNAME,VALUE1NAME," +
                "VALUE1VALUE,VALUE2NAME,VALUE2VALUE,AD_USERQUERYLINE_ID FROM AD_UserQueryLine WHERE AD_UserQuery_ID=" +
                AD_UserQuery_ID + " ORDER BY SeqNo";
            try {
                dr = VIS.DB.executeDataReader(sql, null, null);

                //dc = new DataColumn("OPERATOR");
                //ds.Tables[0].Columns.Add(dc);
                // DataRow dr;
                var optrName = "";
                var optr = "";
                while (dr.read()) {
                    var obj = {};
                    obj["KEYNAME"] = dr.get("KEYNAME");
                    obj["KEYVALUE"] = dr.get("KEYVALUE");
                    obj["OPERATORNAME"] = dr.get("OPERATORNAME");
                    obj["VALUE1NAME"] = dr.get("VALUE1NAME");
                    obj["VALUE1VALUE"] = dr.get("VALUE1VALUE");
                    obj["VALUE2NAME"] = dr.get("VALUE2NAME");
                    obj["VALUE2VALUE"] = dr.get("VALUE2VALUE");
                    obj["AD_USERQUERYLINE_ID"] = dr.get("AD_USERQUERYLINE_ID");

                    optrName = dr.get("OPERATORNAME").toString();
                    optr = VIS.Query.prototype.OPERATORS[optrName];
                    obj["OPERATOR"] = optr;
                    lines.push(obj);
                }
            }
            catch (ex) {
            }
            return lines;
        },

        deleteLines: function (AD_UserQuery_ID) {
            var sqlQry = "DELETE FROM AD_UserQueryLine WHERE AD_UserQuery_ID=" + AD_UserQuery_ID;
            var no = VIS.DB.executeQuery(sqlQry, null, null);
            //log.Info("#" + no);
            //_lines = null;
            return no >= 0;
        },

        deleteUserQuery: function (AD_UserQuery_ID) {
            var no = -1;
            $.ajax({
                url: VIS.Application.contextUrl + 'ASearch/DeleteQuery',
                type: "POST",
                datatype: "json",
                async: false,
                data: { id: AD_UserQuery_ID }
            }).done(function (json) {
                no = parseInt(json);
            })
            return no >= 0;
        },

        insertOrUpdate: function (value, name, where, AD_Tab_ID, AD_Table_ID, dsAdvanceData) {
            var no = -1;
            $.ajax({
                url: VIS.Application.contextUrl + 'ASearch/InsertOrUpdateQuery',
                type: "POST",
                datatype: "json",
                contentType: "application/json; charset=utf-8",
                async: false,
                data: JSON.stringify({ id: value, name: name, where: where, tabid: AD_Tab_ID, tid: AD_Table_ID, qLines: dsAdvanceData })
            }).done(function (json) {
                no = parseInt(json);
            })
            return no >= 0;
        },

    };

    function Find(windowNo, curTab, minRecord) {
        var title = curTab.getName();
        var AD_Tab_ID = curTab.getAD_Tab_ID();
        var AD_Table_ID = curTab.getAD_Table_ID();
        var tableName = curTab.getTableName();
        var whereExtended = curTab.getWhereClause();
        var findFields = curTab.getFields();

        var $root = $("<div style='height:100%'>").html("<div class='vis-apanel-busy' style='height:96%; width:98%;'></div>");
        var $busy = null;

        var self = this;
        var ch = null;
        var btnOk, btnCancel, btnDelete, btnSave, btnRefresh;
        var txtQryName, drpSavedQry, drpColumns, drpOp, drpDynamicOp, chkDynamic, txtYear, txtMonth, txtDay, txtStatus;
        var ulQryList, divDynamic, divYear, divMonth, divDay, divValue1, divValue2, tblGrid, tblBody;

        var FIELDLENGTH = 20, TABNO = 99;

        var total = 0, isLoadError = false, isSaveError = false, isBusy = false;
        var dsAdvanceData = null;
        var log = VIS.Logging.VLogger.getVLogger("Find");

        this.onClose = null, this.created = false, this.days = 0, this.okPressed = false, this.okBtnPressed = false;

        var control1, control2, ulListStaticHtml = "";;

        var query = new VIS.Query(tableName); //query
        query.addRestriction(whereExtended); // restriction

        $root.load(VIS.Application.contextUrl + 'ASearch/Index/?windowNo=' + windowNo, function (evt) {
            initUI();
            initFind();
            bindEvents();

        });

        function initUI() {
            //right side list
            ulQryList = $root.find("#ulQry_" + windowNo);
            drpSavedQry = $root.find("#drpSavedQry_" + windowNo);
            drpColumns = $root.find("#drpColumn_" + windowNo);
            drpOp = $root.find("#drpOperator_" + windowNo);
            divValue1 = $root.find("#divValue1_" + windowNo);
            divValue2 = $root.find("#divValue2_" + windowNo);
            txtQryName = $root.find("#txtQryName_" + windowNo);

            //actions
            btnOk = $root.find("#btnOk_" + windowNo);
            btnCancel = $root.find("#btnCancel_" + windowNo);
            btnSave = $root.find("#btnSave_" + windowNo);
            btnDelete = $root.find("#btnDelete_" + windowNo);
            btnRefresh = $root.find("#btnRefresh_" + windowNo);
            //dynamic
            divDynamic = $root.find("#divDynamic_" + windowNo);
            chkDynamic = $root.find("#chkDynamic_" + windowNo);
            drpDynamicOp = $root.find("#drpDynamicOp_" + windowNo);
            txtYear = $root.find("#txtYear_" + windowNo);
            txtMonth = $root.find("#txtMonth_" + windowNo);
            txtDay = $root.find("#txtDay_" + windowNo);
            divYear = $root.find("#divYear_" + windowNo);
            divMonth = $root.find("#divMonth_" + windowNo);
            divDay = $root.find("#divDay_" + windowNo);
            divYear.hide();
            divMonth.hide();
            divDay.hide();
            divDynamic.hide();

            //if (VIS.Application.isRTL) {
            //    btnOk.css("margin-left", "-142px");
            //    btnCancel.css("margin-left", "70px");
            //}


            //grid 
            tblGrid = $root.find("#tblQry_" + windowNo);
            tblBody = tblGrid.find("tbody");
            txtStatus = $root.find("#pstatus_" + windowNo);
            $busy = $root.find("#divBusy_" + windowNo);
        };

        function initFind() {
            total = getNoOfRecords(null, false);
            var drListQueries = MUserQuery.getData(VIS.context, AD_Tab_ID, AD_Table_ID, "Code");

            setStatusDB(total);
            ulListStaticHtml = ulQryList.html();
            fillList(drListQueries);
            //SetBusy(false);

            var html = '<option value="-1"> </option>';
            for (var c = 0; c < findFields.length; c++) {
                // get field
                var field = findFields[c];
                if (field.getIsEncrypted())
                    continue;
                // get field's column name
                var columnName = field.getColumnName();
                if (field.getDisplayType() == VIS.DisplayType.Button) {
                    if (field.getAD_Reference_Value_ID() == 0)
                        continue;
                    if (columnName.endsWith("_ID"))
                        field.setDisplayType(VIS.DisplayType.Table);
                    else
                        field.setDisplayType(VIS.DisplayType.List);
                    //field.loadLookUp();
                }
                // get text to be displayed
                var header = field.getHeader();
                if (header == null || header.length == 0) {
                    // get text according to the language selected
                    header = VIS.Msg.getElement(VIS.context, columnName);
                    if (header == null || header.Length == 0)
                        continue;
                }
                // if given field is any key, then add "(ID)" to it
                if (field.getIsKey())
                    header += (" (ID)");

                // add a new row in datatable and set values
                //dr = dt.NewRow();
                //dr[0] = header; // Name
                //dr[1] = columnName; // DB_ColName
                //dt.Rows.Add(dr);
                html += '<option value="' + columnName + '">' + header + '</option>';
            }
            drpColumns.html(html);
            setBusy(false);
        };

        function bindEvents() {

            btnCancel.on("click", function () {
                if (isBusy) return;
                self.okBtnPressed = false;
                ch.close();
            });

            chkDynamic.on("change", function () {
                var enable = chkDynamic.prop("checked");
                drpDynamicOp.prop("disabled", !enable);
                drpOp.prop("disabled", enable);
                setValueEnabled(!enable);
                setValue2Enabled(!enable);
                if (enable) {
                    setDynamicQryControls(self.getIsUserColumn(drpColumns.val()));
                }
                else {
                    divYear.hide();
                    divMonth.hide();
                    divDay.hide();
                }
            });

            drpDynamicOp.on("change", function () {

                setDynamicQryControls();
            });

            drpColumns.on("change", function () {
                if (isBusy) return;
                chkDynamic.prop("disabled", true);
                chkDynamic.prop("checked", false);
                divDynamic.hide();

                // set control at value1 position according to the column selected
                var columnName = drpColumns.val();
                setControlNullValue(true);
                if (columnName && columnName != "-1") {
                    var dsOp = null;
                    var dsOpDynamic = null;
                    // if column name is of ant ID
                    if (columnName.endsWith("_ID") || columnName.endsWith("_Acct") || columnName.endsWith("_ID_1") || columnName.endsWith("_ID_2") || columnName.endsWith("_ID_3")) {
                        // fill dataset with operators of type ID
                        dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_ID);
                    }
                    else if (columnName.startsWith("Is")) {
                        // fill dataset with operators of type Yes No
                        dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_YN);
                    }
                    else {
                        // fill dataset with all operators available
                        dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS);
                    }

                    var f = curTab.getField(columnName);

                    if (f != null && VIS.DisplayType.IsDate(f.getDisplayType())) {
                        drpDynamicOp.html(self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DATE_DYNAMIC, true));
                        divDynamic.show();
                        chkDynamic.prop("disabled", false);
                        setDynamicQryControls();
                    }
                    else if (self.getIsUserColumn(columnName)) {
                        drpDynamicOp.html(self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_DYNAMIC_ID, true));
                        divDynamic.show();
                        chkDynamic.prop("disabled", false);
                        setDynamicQryControls(true);
                    }

                    drpOp.html(dsOp);
                    drpOp[0].SelectedIndex = 0;
                    // get field
                    var field = getTargetMField(columnName);
                    // set control at value1 position
                    setControl(true, field);
                    // enable the save row button
                    setEnableButton(btnSave, true);//silverlight comment
                    drpOp.prop("disabled", false);
                }
                // enable control at value1 position
                setValueEnabled(true);
                // disable control at value2 position
                setValue2Enabled(false);

            });

            drpOp.on("change", function () {
                if (isBusy) return;
                var selOp = drpOp.val();

                // set control at value2 position according to the operator selected
                if (!selOp) {
                    selOp = "";
                }

                var columnName = "";
                var field = "";

                if (selOp && selOp != "0") {
                    //if user selects between operator
                    if (VIS.Query.prototype.BETWEEN.equals(selOp)) {
                        columnName = drpColumns.val();
                        // get field
                        field = getTargetMField(columnName);
                        // set control at value2 position
                        setControl(false, field);
                        // enable the control at value2 position
                        setValue2Enabled(true);
                    }
                    else {
                        setValue2Enabled(false);
                    }
                }
                else {
                    setEnableButton(btnSave, false);//
                    setValue2Enabled(false);
                    setControlNullValue(true);
                }
            });

            drpSavedQry.on("change", function () {
                if (isBusy) return;
                // binds grid according to the query selected in combobox
                var val = drpSavedQry.val();
                if (val && val != "-1") {
                    //setBusy(true);
                    var obj = null;


                    dsAdvanceData = MUserQuery.getQueryLines(val);

                    bindGrid(dsAdvanceData);
                    txtQryName.val(drpSavedQry.find("option:selected").text());
                    txtQryName.prop("readonly", false);
                    //setBusy(false);
                    setEnableButton(btnDelete, true);
                }
                else {
                    // if nothing is selected
                    //ibtnDelQuery.Enabled = false;
                    setEnableButton(btnDelete, false);//silverlight comment
                    txtQryName.val("");
                    txtQryName.prop("readonly", true);
                    dsAdvanceData = null;
                    bindGrid(null);
                }
            });

            tblGrid.on("click", function (e) {
                if (isBusy) return;
                if (e.target.nodeName === "IMG") {
                    var index = $(e.target).data("index");
                    dsAdvanceData.splice(index, 1);//  .Tables[0].Rows.RemoveAt(index);
                    bindGrid(dsAdvanceData);
                }
            });

            ulQryList.on("click", "LI", function (e) {
                if (isBusy) return;


                var val = $(this).data("value");
                //try for double click list
                if (val != null) {
                    setBusy(true);

                    var ii = $(this).index();
                    setTimeout(function () {

                        if (ii < 9) {
                            var valSplit = val.toString().split('|');
                            var cnt = valSplit.length;
                            if (cnt == 1) {
                                self.created = false;
                            }
                            else {
                                self.created = true;
                            }

                            self.days = parseInt(valSplit[0]);
                            val = "";
                        }
                        else {
                            self.days = 0;
                        }

                        query = new VIS.Query(tableName);
                        query.addRestriction(val);

                        var no = 0;

                        no = getNoOfRecords(query, true);
                        query.setRecordCount(no);

                        setBusy(false);
                        if (no != 0) {
                            self.okBtnPressed = true;
                            ch.close();
                            //Close(_isOkButtonPressed);
                        }
                    }, 10);
                }
                else {
                    // SetBusy(false);
                    self.okBtnPressed = false;
                    ch.close();
                    // Close(_isOkButtonPressed);
                }
            });

            btnSave.on("click", saveRowTemp);

            btnDelete.on("click", function () {
                if (isBusy) return;
                var obj = drpSavedQry.val();
                if (obj == null || obj.toString() == "" || parseInt(obj) < 1)
                    return;

                setBusy(true);

                var uq;

                window.setTimeout(function () {
                    // get name of the query
                    var name = drpSavedQry.find("option:selected").text();
                    // delete query

                    if (MUserQuery.deleteUserQuery(obj)) {
                        var drListQueries = MUserQuery.getData(VIS.context, AD_Tab_ID, AD_Table_ID, "Code");

                        ulQryList.empty();
                        ulQryList.html(ulListStaticHtml);
                        fillList(drListQueries);
                        drpSavedQry[0].selectedIndex = 0;
                        //// show message to user
                        VIS.ADialog.info("Deleted", true, name, "");
                        txtQryName.val("");
                        txtQryName.prop("readony", true);
                        setBusy(false);
                        drpSavedQry.trigger("change");
                    }
                    else {
                        VIS.ADialog.info("DeleteError", true, name, "");
                    }

                    setBusy(false);
                }, 10);

            });

            btnOk.on("click", function () {
                if (isBusy) return;
                //setBusy(true);
                self.okPressed = true;
                self.okBtnPressed = true;
                //	Save pending
                saveAdvanced();
            });

            btnRefresh.on("click", function () {
                if (isBusy) return;
                //setBusy(true);
                // save row
                saveRowTemp();	//	unsaved 
                // get query
                var temp = getQueryAdvanced();
                var records = 0;

                records = getNoOfRecords(temp, true);
                setStatusDB(records);
                //setBusy(false);
            });
        };

        function unBindEvents() {

            if (!btnCancel)
                return;

            btnCancel.off("click");
            chkDynamic.off("change");
            drpDynamicOp.off("change");
            drpColumns.off("change");
            drpOp.off("change");
            drpSavedQry.off("change");
            tblGrid.off("click");
            ulQryList.off("click");
            btnSave.off("click");
            btnDelete.off("click");
            btnOk.off("click");
            btnRefresh.off("click");
        };

        function fillList(dr) {
            var html = "";
            var html1 = "";
            while (dr.read()) {
                html += '<li data-value="' + dr.get(1) + '" >' + VIS.Utility.encodeText(dr.get(0)) + '</li>';
                html1 += '<option value="' + dr.get("AD_UserQuery_ID") + '">' + VIS.Utility.encodeText(dr.get(0)) + '</option>';
            }
            if (html.length > 0) {
                ulQryList.append(html);

                //drpSavedQry.html("<option value='-1' > </option>" + html1);   

                //Commented by karan, Because if last item deleted from dropdown,
                //then html.length is 0 and our dropdown doesn't get refreshed.. hence plased his code outside of check...
            }

            drpSavedQry.html("<option value='-1' > </option>" + html1);


        };

        /* show hide Dynamic div area */
        function setDynamicQryControls(isUser) {
            var index = drpDynamicOp[0].selectedIndex;
            if (isUser) {
                divYear.hide();
                divMonth.hide();
                divDay.hide();
                return;
            }
            divYear.show();
            divMonth.show();
            divDay.show();
            txtDay.prop("readonly", false);
            txtMonth.prop("min", 1);
            if (index == 3) {
                txtMonth.prop("min", 0);
                txtDay.val(0);
                txtMonth.val(0);
                txtYear.val(1);
            }

            else if (index == 2) {
                divYear.hide();
                txtYear.val("");
                txtMonth.val(1);
                txtDay.val(0);
            }
            else if (index == 1) {
                divYear.hide();
                divMonth.hide();
                txtDay.val(0);
            }
            else if (index == 0) {
                txtDay.prop("readonly", true);
                divYear.hide();
                divMonth.hide();
                txtDay.val(0);
                //divDay.hide();
            }
        };

        function getIsDyanamicVisible() {
            return divDay.is(':visible') || divYear.is(':visible') || divDay.is(':visible');
        };

        function getDynamicText(index) {
            var text = "";
            var timeUnit;
            if (index == 3) {
                timeUnit = Math.round((getTotalDays(index) / 365), 1);
                text = "Last " + timeUnit.toString() + " Years";
            }
            else if (index == 2) {
                timeUnit = Math.round((getTotalDays(index) / 31), 1);
                text = "Last " + timeUnit.toString() + " Month";
            }
            else {
                if (getTotalDays() != 0) {
                    text = "Last " + getTotalDays() + " Days";
                }
                else {
                    text = "This Day";
                }
            }
            return text;
        };

        function getDynamicValue(index) {
            var text = "";
            text = " adddays(sysdate, - " + getTotalDays(index) + ") ";
            return text;
        };

        function getTotalDays(index) {
            var totasldays = 0;
            if (index == 3) {
                var y = txtYear.val(), m = txtMonth.val(), d = txtDay.val();

                y = (y && y != "") ? parseInt(y) : 0;
                m = (m && m != "") ? parseInt(m) : 0;
                d = (d && d != "") ? parseInt(d) : 0;

                totasldays = (y * 365) + (m * 31) + (d);
            }
            else if (index == 2) {
                var m = txtMonth.val(), d = txtDay.val();

                m = (m && m != "") ? parseInt(m) : 0;
                d = (d && d != "") ? parseInt(d) : 0;

                totasldays = (m * 31) + (d);
            }
            else {
                var d = d = txtDay.val();
                d = (d && d != "") ? parseInt(d) : 0;
                totasldays = d;
            }
            return totasldays;
        };

        function setValueEnabled(isEnabled) {
            // get control
            var ctrl = divValue1.children()[1];
            var btn = null;
            if (divValue1.children().length > 2)
                btn = divValue1.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        function setValue2Enabled(isEnabled) {
            var ctrl = divValue2.children()[1];
            var btn = null;
            if (divValue2.children().length > 2)
                btn = divValue2.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        function setEnableButton(btn, isEnable) {
            btn.prop("disabled", !isEnable);
        };

        function setControl(isValue1, field) {
            // set column and row position
            /*****Get control form specified column and row from Grid***********/
            if (isValue1)
                control1 = null;
            control2 = null;
            var ctrl = null;
            var ctrl2 = null;
            if (isValue1) {
                ctrl = divValue1.children()[1];
                if (divValue1.children().length > 2)
                    ctrl2 = divValue1.children()[2];
            }
            else {
                ctrl = divValue2.children()[1];
                if (divValue2.children().length > 2)
                    ctrl2 = divValue2.children()[2];
            }

            //var eList = from child in tblpnlA.Children
            //where Grid.GetRow((FrameworkElement)child) == row && Grid.GetColumn((FrameworkElement)child) == col
            //select child;

            //Remove any elements in the list
            if (ctrl != null) {
                $(ctrl).remove();
                if (ctrl2 != null)
                    $(ctrl2).remove();
                ctrl = null;
            }
            /**********************************/
            var crt = null;
            // if any filed is given
            if (field != null) {
                // if field id any key, then show number textbox 
                if (field.getIsKey()) {
                    crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                                     field.getColumnName());
                }
                else {
                    crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                }
            }
            else {
                // if no field is given show an empty disabled textbox
                crt = new VIS.Controls.VTextBox("columnName", false, true, false, 20, 20, "format",
                          "GetObscureType", false);// VAdvantage.Controls.VTextBox.TextType.Text, DisplayType.String);
            }
            if (crt != null) {
                //crt.SetIsMandatory(false);
                crt.setReadOnly(false);

                if (VIS.DisplayType.Text == field.getDisplayType() || VIS.DisplayType.TextLong == field.getDisplayType()) {
                    crt.getControl().attr("rows", "1");
                    crt.getControl().css("width", "100%");
                }
                else if (VIS.DisplayType.YesNo == field.getDisplayType()) {
                    crt.getControl().css("clear", "both");
                }
                else if (VIS.DisplayType.IsDate(field.getDisplayType())) {
                    crt.getControl().css("line-height", "1");
                }

                var btn = null;
                if (crt.getBtnCount() > 0 && !(crt instanceof VIS.Controls.VComboBox))
                    btn = crt.getBtn(0);

                if (isValue1) {

                    divValue1.append(crt.getControl());
                    control1 = crt;
                    if (btn) {
                        divValue1.append(btn);
                        crt.getControl().css("width", "65%");
                        btn.css("max-width", "35px");
                    }
                }
                else {
                    divValue2.append(crt.getControl());
                    control2 = crt;
                    if (btn) {
                        divValue2.append(btn);
                        crt.getControl().css("width", "65%");
                        btn.css("max-width", "35px");
                    }
                }
            }
        };

        function bindGrid(list) {
            tblBody.empty();
            var html = "";
            var htm = "", obj = null;

            if (list) {
                for (var i = 0, j = list.length; i < j; i++) {
                    htm = '<tr class="vis-advancedSearchTableRow">';
                    obj = list[i];
                    htm += '<td>' + obj["KEYNAME"] + '</td><td style="display:none">' + obj["KEYVALUE"] + '</td><td>' + obj["OPERATORNAME"] +
                           '</td><td>' + obj["VALUE1NAME"] + '</td><td style="display:none">' + obj["VALUE1VALUE"] + '</td><td>' + obj["VALUE2NAME"] +
                           '</td><td style="display:none">' + obj["VALUE2VALUE"] + '</td><td style="display:none">' + obj["AD_USERQUERYLINE_ID"] + '</td><td style="display:none">' + obj["OPERATOR"] +
                           '</td><td><img style="cursor:pointer" data-index = "' + i + '" src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/delete10.png" /></td>';
                    htm += '</tr>';
                    html += htm;
                }
            }
            tblBody.html(html);
        };

        /* get total number of record */
        function getNoOfRecords(query, alertZeroRecords) {
            // make query
            var sql = "SELECT COUNT(*) FROM ";
            sql += tableName;
            var hasWhere = false;
            // add where clause if already exists
            if (whereExtended != null && whereExtended.length > 0) {
                if (whereExtended.indexOf("@") == -1) {
                    sql += " WHERE " + whereExtended;
                }
                else {
                    sql += " WHERE " + VIS.Env.parseContext(VIS.context, windowNo, whereExtended, false);
                }
                hasWhere = true;
            }
            // if user has given any query
            if (query != null && query.getIsActive()) {
                // if where clause is started, then add "AND"
                if (hasWhere) {
                    sql += " AND ";
                }
                    // add "WHERE"
                else {
                    sql += " WHERE ";
                }
                sql += query.getWhereClause(true);
            }
            //	Add Access
            var finalSQL = VIS.MRole.getDefault().addAccessSQL(sql.toString(), tableName,
                VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);
            //finalSQL = Env.ParseContext(_ctx, _targetWindowNo, finalSQL, false);

            VIS.context.setContext(windowNo, TABNO, "FindSQL", finalSQL);

            //  Execute Query
            total = 999999;

            //System.Threading.ThreadPool.QueueUserWorkItem(delegate
            //{
            //    try
            //    {
            //        //_total = int.Parse(ExecuteQuery.ExecuteScalar(finalSQL));
            //        _total = DataBase.DB.GetSQLValue(null, finalSQL, null);
            //    }
            //    catch (Exception ex)
            //    {
            //        log.Log(Level.SEVERE, finalSQL, ex);
            //    }
            //    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() => GetTotal(_total, query, alertZeroRecords));

            //});
            try {

                //_total = int.Parse(ExecuteQuery.ExecuteScalar(finalSQL));
                total = VIS.DB.executeScalar(finalSQL, null);
            }
            catch (ex) {
                log.Log(VIS.Level.SEVERE, finalSQL, ex);
            }
            var role = VIS.MRole.getDefault();
            //	No Records
            self.okPressed = false;
            if (total == 0 || total == null) {
                total = 0;
                VIS.ADialog.info("FindZeroRecords", true, "FindZeroRecords");
            }
                //	More then allowed
            else if (query != null && role.getIsQueryMax(total)) {
                VIS.ADialog.error("FindOverMax", true, total + " > " + role.getMaxQueryRecords());//silverlight
                //MessageBox.Show("FindZeroRecords " + _total + " > " + role.GetMaxQueryRecords());
            }
            else {
                self.okPressed = true;
                //log.Config("#" + _total);
            }
            // show query's where clause on status bar
            if (query != null) {
                //CommonFunctions.ShowMessage(query.GetWhereClause(), lblStatusBar);
            }
            return total;
        };

        function setStatusDB(currentCount) {
            var text = " " + currentCount + " / " + total + " ";
            // show records on status bar
            txtStatus.text(text);
            if (total < minRecord) {
                isLoadError = true;
                return;
            }
        };

        function getTargetMField(columnName) {
            // if no column name, then return null
            if (columnName == null || columnName.length == 0)
                return null;
            // else find field for the given column
            for (var c = 0; c < findFields.length; c++) {
                var field = findFields[c];
                if (columnName.equals(field.getColumnName()))
                    return field;
            }
            return null;
        };

        function saveRowTemp() {
            // set column name

            var cVal = drpColumns.val();

            if (!cVal || cVal == "-1")
                return false;

            var colName = drpColumns.find("option:selected").text();
            var colValue = "";
            if (colName == null || colName.trim().length == 0) {
                return false;
            }
            else {
                // set column value
                colValue = cVal.toString();
            }

            var dCheck = chkDynamic.prop("checked");

            if (dCheck) {

                if (getIsDyanamicVisible()) {
                    var opValueD = ">=";
                    var opNameD = " >= ";
                    var controlText = getDynamicText(drpDynamicOp[0].selectedIndex);
                    var controlValue = getDynamicValue(drpDynamicOp[0].selectedIndex);
                    addRow(colName, colValue, opNameD, opValueD, controlText, controlValue, null, null);
                }
                else {
                    var opValueD = "=";
                    var opNameD = " = ";
                    var controlText = drpDynamicOp.find("option:selected").text();
                    var controlValue = drpDynamicOp.val();
                    addRow(colName, colValue, opNameD, opValueD, controlText, controlValue, null, null);
                }
            }
            else {

                // set operator name
                var opName = drpOp.val();

                if (drpOp[0].selectedIndex > -1)
                    opName = drpDynamicOp.find("option:selected").text();;;// vcmbOperator.Text;//silverlight comment
                // set operator (sign)
                var opValue = drpOp.val();
                // add row in dataset
                addRow(colName, colValue, opName, opValue, getControlText(true), getControlValue(true), getControlText(false), getControlValue(false));
            }
            //reset column & operator comboBox
            drpColumns[0].selectedIndex = 0;
            drpOp[0].selectedIndex = 0;
            setControlNullValue();
            setControlNullValue(true);
            txtQryName.prop("readonly", false)
            return true;
        };

        /* Gets control's value*/

        function getControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            else {
                //  crtlObj = (IControl)tblpnlA2.GetControlFromPosition(3, 1);
                crtlObj = control2;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                // return control's value
                return crtlObj.getValue();
            }
            return "";
        };

        /* <param name="isValue1">true if get control's text at value1 position else false</param>
         */
        function getControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            else {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(3, 1);
                crtlObj = control2;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text
                return crtlObj.getDisplay();
            }
            return "";
        };

        function addRow(colName, colValue, optr, optrName,
           value1Name, value1Value, value2Name, value2Value) {

            if (dsAdvanceData == null)
                dsAdvanceData = [];

            var obj = {};
            obj["KEYNAME"] = colName;
            //dsAdvanceData.Tables[0].Columns.Add(dc);
            obj["KEYVALUE"] = colValue;

            obj["OPERATORNAME"] = optrName;
            obj["VALUE1NAME"] = VIS.Utility.encodeText(value1Name);

            if (value1Name == "")
                obj["VALUE1VALUE"] = "";
            else {
                if (value1Value == null)
                    obj["VALUE1VALUE"] = "NULL";
                else
                    obj["VALUE1VALUE"] = VIS.Utility.encodeText(VIS.Utility.Util.getValueOfString(value1Value));
            }

            obj["VALUE2NAME"] = VIS.Utility.encodeText(value2Name);
            if (value2Value == null)
                obj["VALUE2VALUE"] = "NULL";
            else
                obj["VALUE2VALUE"] = VIS.Utility.encodeText(VIS.Utility.Util.getValueOfString(value2Value));
            obj["AD_USERQUERYLINE_ID"] = 0;
            obj["OPERATOR"] = optr;
            dsAdvanceData.push(obj);
            bindGrid(dsAdvanceData);//for the time beeing commented today 3Dec.2010
        };


        function setControlNullValue(isValue2) {
            var crtlObj = null;
            if (isValue2) {
                crtlObj = control2;
            }
            else {
                crtlObj = control1;
            }

            // if control exists
            if (crtlObj != null) {
                crtlObj.setValue(null);
            }

        };

        function getQueryAdvanced() {
            var _query = new VIS.Query(tableName);
            // check if dataset have any table
            // for every row in dataset
            if (dsAdvanceData) {

                for (var i = 0; i < dsAdvanceData.length; i++) {
                    var dtRowObj = dsAdvanceData[i];
                    //	Column
                    var infoName = dtRowObj["KEYNAME"].toString();
                    var columnName = dtRowObj["KEYVALUE"].toString();
                    var field = getTargetMField(columnName);
                    var columnSQL = field.getColumnSQL(); //
                    //	Operator
                    var optr = dtRowObj["OPERATORNAME"].toString(); //dtRowObj["OPERATOR"].ToString()
                    //	Value

                    var value = dtRowObj["VALUE1VALUE"];
                    var parsedValue = null;
                    if (value != null && value.toString().trim().startsWith("adddays") || value.toString().trim().startsWith("@")) {
                        ;
                    }
                    else {
                        parsedValue = parseValue(field, value);
                    }
                    //string infoDisplay = dtRowObj["VALUE1NAME"].ToString();
                    var infoDisplay = null;

                    if (value == null || value.toString().length < 1) {
                        if (VIS.Query.prototype.BETWEEN.equals(optr))
                            continue;	//	no null in between
                        parsedValue = VIS.Env.NULLString;
                        infoDisplay = "NULL";
                    }
                    else {
                        infoDisplay = dtRowObj["VALUE1NAME"].toString();
                    }

                    //	Value2
                    // if "BETWEEN" selected
                    if (VIS.Query.prototype.BETWEEN.equals(optr)) {

                        var value2 = dtRowObj["VALUE2VALUE"].toString();
                        if (value2 == null || value2.toString().trim().length < 1)
                            continue;

                        var parsedValue2 = parseValue(field, value2);
                        var infoDisplay_to = dtRowObj["VALUE2NAME"].toString();
                        if (parsedValue2 == null)
                            continue;
                        // else add restriction where clause to query with between operator
                        _query.addRangeRestriction(columnSQL, parsedValue, parsedValue2, infoName,
                            infoDisplay, infoDisplay_to);
                    }
                    else {
                        // else add simple restriction where clause to query

                        if (parsedValue == null && value != null && (value.toString().trim().startsWith("adddays") || value.toString().trim().startsWith("@"))) {
                            var Where = columnName + optr + value;
                            where = VIS.Env.parseContext(VIS.context, windowNo, Where, false);
                            _query.addRestriction(Where);
                        }
                        else {
                            _query.addRestriction(columnSQL, optr, parsedValue, infoName, infoDisplay);
                        }
                    }
                }
            }
            return _query;
        };

        function parseValue(field, pp) {
            if (pp == null)
                return null;
            var dt = field.getDisplayType();
            var inStr = pp.toString();
            if (inStr == null || inStr.equals(VIS.Env.NULLString) || inStr == "")
                return null;
            try {
                //	Return Integer
                if (dt == VIS.DisplayType.Integer
                    || (VIS.DisplayType.IsID(dt) && field.getColumnName().endsWith("_ID"))) {
                    //i = int.Parse(inStr);
                    return parseInt(inStr);
                    // return i;
                }
                    //	Return BigDecimal
                else if (VIS.DisplayType.IsNumeric(dt)) {
                    return parseFloat(inStr);       //DisplayType.GetNumberFormat(dt).GetFormatedValue(inStr);
                }
                    //	Return Timestamp
                else if (VIS.DisplayType.IsDate(dt)) {
                    var time = "";
                    try {
                        return new Date(inStr);
                    }
                    catch (e) {
                        //log.Log(Level.WARNING, inStr + "(" + inStr.GetType().FullName + ")" + e);
                        time = "";//DisplayType.GetDateFormat(dt).Format(inStr);
                    }
                    try {
                        return Date.Parse(time);
                    }
                    catch (ee) {
                        return null;
                    }
                }
            }
            catch (ex) {
                //     log.Log(Level.WARNING, "Object=" + inStr, ex);
                var error = ex.message;
                if (error == null || error.length == 0)
                    error = ex.toString();
                var errMsg = "";
                errMsg += field.getColumnName() + " = " + inStr + " - " + error;
                //
                //if(pp != null && pp.ToString().Trim().StartsWith("adddays") || pp.ToString().Trim().StartsWith("adddays")
                VIS.ADialog.error("ValidationError", true, errMsg.toString());
                //MessageBox.Show("ValidationError " + errMsg.ToString());
                return null;
            }

            return inStr;
        };	//	parseValue

        function saveAdvanced() {
            // save all query lines temporarily

            setBusy(true);

            saveRowTemp();	//	unsaved 
            // get query
            query = getQueryAdvanced();
            if (query.getRestrictionCount() == 0) {
                setBusy(false);
                return;
            }


            // get where clause
            var where = query.getWhereClause(true);
            // get query name entered by the user
            var name = VIS.Utility.encodeText(txtQryName.val());// vtxtQueryName.Text.Trim();
            if (name != null && name.length == 0)
                name = null;
            // get the selected value
            var value = drpSavedQry.val();// vcmbQueryA.SelectedValue;
            var s = "";// vcmbQueryA.Text;//silverlight comment
            //	Update Existing Query
            var qMessage = "";

            value = (value != null && value.toString() != "-1" && parseInt(value) > 0) ? value : 0;

            window.setTimeout(function () {

                if (value != 0 || name != null) {

                    if (MUserQuery.insertOrUpdate(value, name, where, AD_Tab_ID, AD_Table_ID, dsAdvanceData)) {
                        isSaveError = false;
                        //ShowMessage.Info("Updated", true, uq.GetName(), "");
                        qMessage = (value > 0 ? "Updated" : "Saved") + " " + name;
                    }
                    else {
                        isSaveError = true;
                        //ShowMessage.Info("Updated", true, uq.GetName(), "");
                        qMessage = (value > 0 ? "UpdatedError" : "SaveError") + " " + name;
                    }
                }

                var result = false;
                if (getNoOfRecords(query, true) != 0) {
                    result = true;
                }
                setBusy(false);
                if (qMessage != "") {
                    //MessageBox.Show(qMessage);
                    VIS.ADialog.info(qMessage, true, "", null);
                }
                if (result) {
                    ch.close();
                }
            }, 10);
        };

        function setBusy(busy) {
            isBusy = busy;
            $busy.css("visibility", isBusy ? "visible" : "hidden");
            btnOk.prop("disabled", busy);
            btnCancel.prop("disabled", busy);
            btnRefresh.prop("disabled", busy);
        };

        this.show = function () {
            ch = new VIS.ChildDialog();

            ch.setHeight(550);
            ch.setWidth(860);
            ch.setTitle(VIS.Msg.getMsg("Find"));
            ch.setModal(true);
            //Disposing Everything on Close
            ch.onClose = function () {
                //self.okBtnPressed = false;
                if (self.onClose) self.onClose();
                self.dispose();
            };

            ch.show();
            ch.setContent($root);
            ch.hideButtons();

            //  bindEvents();
        };

        this.getQuery = function () {
            var role = VIS.MRole.getDefault();
            if (role.getIsQueryMax(total)) {
                query = VIS.Query.prototype.getNoRecordQuery(tableName, false);
                total = 0;
                log.warning("Query - over max");
            }
            else
                log.info("Query=" + query);
            return query;
        };

        this.disposeComponent = function () {
            unBindEvents();
            btnOk = btnCancel = btnDelete = btnSave = btnRefresh = null;
            txtQryName = drpSavedQry = drpColumns = drpOp = drpDynamicOp = chkDynamic = txtYear = txtMonth = txtDay = null;
            ulQryList = divDynamic = divYear = divMonth = divDay = null;
            if ($root)
                $root.remove();
            $root = null;
            total = isLoadError = isSaveError = null;
            dsAdvanceData = null;
            log = null;
            this.created = this.days = 0, this.okPressed = this.okBtnPressed = null;
            control1 = control2 = ulListStaticHtml = null;
            query = null;
        };
    };

    Find.prototype.getOperatorsQuery = function (vnpObj, translate) {
        var html = "";
        var val = "";
        for (var p in vnpObj) {
            val = vnpObj[p];
            if (translate)
                val = VIS.Msg.getMsg(val);
            html += '<option value="' + p + '">' + val + '</option>';
        }
        return html;
    };

    Find.prototype.getIsUserColumn = function (columnName) {
        if (columnName.endsWith("atedBy") || columnName.equals("AD_User_ID"))
            return true;
        if (columnName.equals("SalesRep_ID"))
            return true;
        return false;
    };

    Find.prototype.getCurrentDays = function () {
        return this.days;
    };

    Find.prototype.getIsCreated = function () {
        return this.created;
    };

    Find.prototype.getIsOKPressed = function () {
        return this.okPressed && this.okBtnPressed;
    };



    Find.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.Find = Find;

}(VIS, jQuery));