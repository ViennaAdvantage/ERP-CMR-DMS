; (function (VIS, $) {
    //form declaretion
    function AccountForm(header, account, cAcctSchemaId) {
        this.onClose = null;
        var root = $("<div style='position:relative;'>");
        var $busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");

        var $self = this;

        var title = header;
        var mAccount = account;
        var C_AcctSchema_ID = cAcctSchemaId;
        var windowNo = VIS.Env.getWindowNo();
        var _comb = null;
        var f_Description = new VIS.Controls.VLabel();
        //  Editors for Query
        var f_Alias = null, f_Combination = null, f_AD_Org_ID = null, f_Account_ID = null, f_SubAcct_ID = null,
            f_M_Product_ID = null, f_C_BPartner_ID = null, f_C_Campaign_ID = null, f_C_LocFrom_ID = null, f_C_LocTo_ID = null,
            f_C_Project_ID = null, f_C_SalesRegion_ID = null, f_AD_OrgTrx_ID = null, f_C_Activity_ID = null,
            f_User1_ID = null, f_User2_ID = null;

        this.log = VIS.Logging.VLogger.getVLogger("AccountForm");
        this.log.config("C_AcctSchema_ID=" + C_AcctSchema_ID + ", C_ValidCombination_ID=" + mAccount.C_ValidCombination_ID);
        //control on form


        /** ElementType AD_Reference_ID=181 */
        var eLEMENTTYPE_AD_Reference_ID = 181;
        /** Account = AC */
        var eLEMENTTYPE_Account = "AC";
        /** Activity = AY */
        var eLEMENTTYPE_Activity = "AY";
        /** BPartner = BP */
        var eLEMENTTYPE_BPartner = "BP";
        /** Location From = LF */
        var eLEMENTTYPE_LocationFrom = "LF";
        /** Location To = LT */
        var eLEMENTTYPE_LocationTo = "LT";
        /** Campaign = MC */
        var eLEMENTTYPE_Campaign = "MC";
        /** Organization = OO */
        var eLEMENTTYPE_Organization = "OO";
        /** Org Trx = OT */
        var eLEMENTTYPE_OrgTrx = "OT";
        /** Project = PJ */
        var eLEMENTTYPE_Project = "PJ";
        /** Product = PR */
        var eLEMENTTYPE_Product = "PR";
        /** Sub Account = SA */
        var eLEMENTTYPE_SubAccount = "SA";
        /** Sales Region = SR */
        var eLEMENTTYPE_SalesRegion = "SR";
        /** User List 1 = U1 */
        var eLEMENTTYPE_UserList1 = "U1";
        /** User List 2 = U2 */
        var eLEMENTTYPE_UserList2 = "U2";
        /** User Element 1 = X1 */
        var eLEMENTTYPE_UserElement1 = "X1";
        /** User Element 2 = X2 */
        var eLEMENTTYPE_UserElement2 = "X2";

        var tableSArea = $("<table>");
        tableSArea.css("width", "100%");
        var tr = $("<tr>");

        var Okbtn = null;
        var cancelbtn = null;
        var btnRefresh = null;
        var btnUndo = null;
        var btnSave = null;
        var lblParameter = null;
        var parameterDiv = null;
        var bottumDiv = null;
        var discriptionDiv = null;
        var accDiv = null;
        var acctbl = null;
        var gridController = null;
        var changed = false;
        var C_ValidCombination_ID = null;
        var query = null;
        var _mTab = null;
        var lblbottumMsg = null;
        var lblCount = null;
        var lblbottumMsg2 = null;
        var accountSchemaElements = null;

        this.load = function () {
            root.load(VIS.Application.contextUrl + 'AccountForm/Index/?windowno=' + windowNo, function (event) {
                $self.setBusy(true);
                $self.init(root);
                $self.setBusy(false);
            });
        };

       this.setBusy = function (isBusy) {
            $busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.init = function (root) {
            //Buttons
            Okbtn = root.find("#btnOk_" + windowNo);
            cancelbtn = root.find("#btnCancel_" + windowNo);
            btnRefresh = root.find("#btnRefresh_" + windowNo);
            btnUndo = root.find("#btnUndo_" + windowNo);
            btnSave = root.find("#btnSave_" + windowNo);

            //labels
            lblParameter = root.find("#lblParameter_" + windowNo);
            parameterDiv = root.find("#parameterDiv_" + windowNo);

            bottumDiv = root.find("#bottumDiv_" + windowNo);
            discriptionDiv = root.find("#discriptionDiv_" + windowNo);
            accDiv = root.find("#accDiv_" + windowNo);
            acctbl = root.find("#acctbl_" + windowNo);
            lblbottumMsg = root.find("#lblbottumMsg_" + windowNo);
            lblCount = root.find("#lblCount_" + windowNo);
            lblbottumMsg2 = root.find("#lblbottumMsg2_" + windowNo);

            if (VIS.Application.isRTL) {
                Okbtn.css("margin-right", "-128px");
                cancelbtn.css("margin-right", "55px");
            }

            // bottumDiv.style.display = 'hidden';
            // accDiv.height = "50%;";

            //Calture on label
            Okbtn.val(VIS.Msg.getMsg("OK"));
            cancelbtn.val(VIS.Utility.Util.cleanMnemonic(VIS.Msg.getMsg("Cancel")));
            lblParameter.val(VIS.Msg.getMsg("Parameter"));

            loadParameters();

            function loadAcctSchemaRecords(arrOfarr) {
                var length = arrOfarr.length;
                var textToInsert = "";
                for (var a = 0; a < length; a += 1) {
                    textToInsert += "<tr>";
                    for (var i = 0; i < arrOfarr[a].length; i += 1) {
                        var obj = arrOfarr[a][i];
                        if (i == 0) {
                            if (a % 2) {
                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {

                                textToInsert += "<td class='VIS_Pref_table-row1' style='border-left: 1px solid #ECECEC;'>" + obj + "</td>";
                            }
                        }
                        else if (i == arrOfarr[a].length - 1) {
                            if (a % 2) {

                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {
                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                        else {
                            if (a % 2) {
                                textToInsert += "<td class='VIS_Pref_table-row2'>" + obj + "</td>";
                            }
                            else {
                                textToInsert += "<td class='VIS_Pref_table-row1'>" + obj + "</td>";
                            }
                        }
                    }
                    textToInsert += " </tr>";
                }
                $error.find('tbody > tr').eq(0).after($.parseHTML(textToInsert));
                arr = null;
                textToInsert = null;
            };

            function loadParameters() {
                VIS.Env.getCtx().setContext(windowNo, "C_AcctSchema_ID", C_AcctSchema_ID);

                $.ajax({
                    url: VIS.Application.contextUrl + "AccountForm/LoadControls",
                    dataType: "json",
                    data: {
                        windowNo: windowNo,
                        C_AcctSchema_ID: C_AcctSchema_ID
                    },
                    success: function (data) {
                        returnValue = data.result;
                        designSchema(returnValue);
                        accountSchemaElements = returnValue;

                    }
                });
            };

            function designSchema(obj) {
                //  Model
                var AD_Window_ID = 153;		//	Mavarain Account Combinations 
                VIS.AEnv.getGridWindow(windowNo, AD_Window_ID, function (json) {
                    if (json.error != null) {
                        VIS.ADialog.error(json.error);    //log error
                        self.dispose();
                        self = null;
                        return;
                    }
                    var jsonData = $.parseJSON(json.result); // widow json
                    VIS.context.setContextOfWindow($.parseJSON(json.wCtx), windowNo);// set window context
                    var GridWindow = new VIS.GridWindow(jsonData);
                    if (GridWindow == null) {
                        return;
                    }
                    _mTab = GridWindow.getTabs()[0];
                    //  ParameterPanel restrictions
                    _mTab.getField("Alias").setDisplayLength(15);
                    _mTab.getField("Combination").setDisplayLength(15);
                    //  Grid restrictions
                    _mTab.getField("AD_Client_ID").setDisplayed(false);
                    _mTab.getField("C_AcctSchema_ID").setDisplayed(false);
                    _mTab.getField("IsActive").setDisplayed(false);
                    _mTab.getField("IsFullyQualified").setDisplayed(false);
                    //  don't show fields not being displayed in this environment

                    for (var i = 0; i < _mTab.getFieldCount() ; i++) {
                        var field = _mTab.getField(i);
                        if (!field.getIsDisplayed(true))      //  check context
                        {
                            field.setDisplayed(false);
                        }

                        //var tdChild = $("<td style='display: none;' class='VIS_Pref_table-row'>");
                        ////add column varo grid
                        //if (field.getIsDisplayed()) {
                        //    tdChild = $("<td style='width: auto' class='VIS_Pref_table-row'>");
                        //}

                        //tdChild.concat(field.getHeader());
                        //tr.concat(tdChild);
                    }

                    var id = windowNo + "_" + C_AcctSchema_ID; //uniqueID
                    gridController = new VIS.GridController(false, false, id);
                    gridController.initGrid(true, windowNo, $self, _mTab);
                    gridController.setVisible(true);
                    // gridController.sizeChanged(530, 400);
                    if (window.innerHeight > 700) {
                        gridController.sizeChanged(window.innerHeight - 230, window.innerWidth);
                    }
                    else {
                        gridController.sizeChanged(window.innerHeight - 215, window.innerWidth);
                    }

                    accDiv.append(gridController.getRoot());
                    gridController.activate();

                    //  GridController
                    if (obj.IsHasAlies) {
                        var alias = _mTab.getField("Alias");
                        f_Alias = VIS.VControlFactory.getControl(_mTab, alias, false);
                        addLine(alias, f_Alias, false);
                    }	//	Alias

                    //	Combination
                    var combination = _mTab.getField("Combination");
                    f_Combination = VIS.VControlFactory.getControl(_mTab, combination, false);
                    addLine(combination, f_Combination, false);

                    //Create Fields in Element Order
                    for (var i = 0; i < obj.Elements.length; i++) {
                        var type = returnValue.Elements[i].Type;
                        var isMandatory = returnValue.Elements[i].IsMandatory;

                        if (type.equals(eLEMENTTYPE_Organization)) {
                            var field = _mTab.getField("AD_Org_ID");
                            f_AD_Org_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_AD_Org_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_Account)) {
                            var field = _mTab.getField("Account_ID");
                            f_Account_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            f_Account_ID.setValue(C_AcctSchema_ID);
                            addLine(field, f_Account_ID, isMandatory);
                            // f_Account_ID.VetoableChangeListener += new EventHandler(f_Account_ID_VetoableChangeListener);
                        }
                        else if (type.equals(eLEMENTTYPE_SubAccount)) {
                            var field = _mTab.getField("C_SubAcct_ID");
                            f_SubAcct_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_SubAcct_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_Product)) {
                            var field = _mTab.getField("M_Product_ID");
                            f_M_Product_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            f_M_Product_ID.getBtn();
                            addLine(field, f_M_Product_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_BPartner)) {
                            var field = _mTab.getField("C_BPartner_ID");
                            f_C_BPartner_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_BPartner_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_Campaign)) {
                            var field = _mTab.getField("C_Campaign_ID");
                            f_C_Campaign_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_Campaign_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_LocationFrom)) {
                            var field = _mTab.getField("C_LocFrom_ID");
                            f_C_LocFrom_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_LocFrom_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_LocationTo)) {
                            var field = _mTab.getField("C_LocTo_ID");
                            f_C_LocTo_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_LocTo_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_Project)) {
                            var field = _mTab.getField("C_Project_ID");
                            f_C_Project_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_Project_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_SalesRegion)) {
                            var field = _mTab.getField("C_SalesRegion_ID");
                            f_C_SalesRegion_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_SalesRegion_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_OrgTrx)) {
                            var field = _mTab.getField("AD_OrgTrx_ID");
                            f_AD_OrgTrx_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_AD_OrgTrx_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_Activity)) {
                            var field = _mTab.getField("C_Activity_ID");
                            f_C_Activity_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_C_Activity_ID, isMandatory);
                        }
                            //	User1
                        else if (type.equals(eLEMENTTYPE_UserList1)) {
                            var field = _mTab.getField("User1_ID");
                            f_User1_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_User1_ID, isMandatory);
                        }
                        else if (type.equals(eLEMENTTYPE_UserList2)) {
                            var field = _mTab.getField("User2_ID");
                            f_User2_ID = VIS.VControlFactory.getControl(_mTab, field, false);
                            addLine(field, f_User2_ID, isMandatory);
                        }
                    }

                    //Discription in label
                    // tr = $("<tr>");
                    // tableSArea.append(tr);
                    // var child = $("<td>");
                    // tr.append(child);
                    //f_Description.height = 21;
                    // child.append(f_Description.getControl().addClass('VIS_Pref_Label_Font'));


                    //discriptionDiv.append(f_Description.getControl().addClass('VIS_Pref_Label_Font'));
                    lblbottumMsg2.css("display", "none");
                    //f_Description.Visible = false;

                    parameterDiv.html(tableSArea);


                    query = new VIS.Query();
                    query.addRestriction("C_AcctSchema_ID", VIS.Query.prototype.EQUAL, C_AcctSchema_ID);
                    query.addRestriction("IsFullyQualified", VIS.Query.prototype.EQUAL, "Y");
                    if (mAccount.C_ValidCombination_ID == 0)
                        _mTab.setQuery(VIS.Query.prototype.getEqualQuery("1", "2"));
                    else {
                        var _query = new VIS.Query();
                        _query.addRestriction("C_AcctSchema_ID", VIS.Query.prototype.EQUAL, C_AcctSchema_ID);
                        _query.addRestriction("C_ValidCombination_ID", VIS.Query.prototype.EQUAL, mAccount.C_ValidCombination_ID);
                        _mTab.setQuery(_query);
                    }

                    gridController.query(0, 0, false);

                    lblbottumMsg.val(obj.Description);
                    lblCount.val("?");

                });
            };

            function addLine(field, editor, mandatory) {
                $self.log.fine("Field=" + field);
                //new row
                tr = $("<tr>");
                tableSArea.append(tr);

                var label = VIS.VControlFactory.getLabel(field);
                editor.setReadOnly(false);
                editor.setMandatory(mandatory);
                field.setPropertyChangeListener(editor);
                //new column
                var tdChild1 = $("<td>");
                //	label
                tr.append(tdChild1.css("padding", "4px 0px 2px 0px"));
                tdChild1.append(label.getControl().addClass('VIS_Pref_Label_Font'));
                //new row
                tr = $("<tr>");
                tableSArea.append(tr);
                var tdChild2 = $("<td>");
                //	Field

                if (editor.getBtnCount() >= 2) {

                    tr.append(tdChild2);
                    var div = $("<Div style='width: 97%'>");
                    tdChild2.append(div);
                    if (window.innerHeight > 700) {
                        div.append(editor.getControl().css('width', '90%'));
                        div.append(editor.getBtn(0).css('width', '10%'));
                    }
                    else {
                        div.append(editor.getControl().css('width', '87%'));
                        div.append(editor.getBtn(0).css('width', '30px').css('-webkit-appearance', 'none').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
                    }

                }
                else {
                    tr.append(tdChild2);
                    tdChild2.append(editor.getControl().css("width", "97%"));
                }
            }

            function isNull(value) {
                if (value == null || value == 0) {
                    return true;
                }
                return false;
            }

            function saveSelection() {
                C_ValidCombination_ID = _mTab.getValue("C_ValidCombination_ID");

                if ($self.onClose)
                    $self.onClose(C_ValidCombination_ID);
                root.dialog('close');
            }

            function actionFind(includeAliasCombination) {
                var localquery = null;
                if (query != null) {
                    //localquery = query;//query.DeepCopy();
                    localquery = jQuery.extend(true, {}, query);
                }
                else {
                    localquery = new VIS.Query();
                }

                //	Alias
                if (includeAliasCombination && f_Alias != null && f_Alias.getValue().toString().length > 0) {
                    var value = f_Alias.getValue().toString().toUpperCase();
                    if (!value.endsWith("%")) {
                        value += "%";
                    }
                    localquery.addRestriction("UPPER(Alias)", localquery.LIKE, value);
                }
                //	Combination (mandatory)
                if (includeAliasCombination && f_Combination.getValue().toString().length > 0) {
                    var value = f_Combination.getValue().toString().toUpperCase();
                    if (!value.endsWith("%"))
                        value += "%";
                    localquery.addRestriction("UPPER(Combination)", localquery.LIKE, value);
                }
                //	Org (mandatory)
                if (f_AD_Org_ID != null && !isNull(f_AD_Org_ID.getValue()))
                    localquery.addRestriction("AD_Org_ID", localquery.EQUAL, f_AD_Org_ID.getValue());
                //	Account (mandatory)
                if (f_Account_ID != null && !isNull(f_Account_ID.getValue()))
                    localquery.addRestriction("Account_ID", localquery.EQUAL, f_Account_ID.getValue());

                if (f_SubAcct_ID != null && !isNull(f_SubAcct_ID.getValue()))
                    localquery.addRestriction("C_SubAcct_ID", localquery.EQUAL, f_SubAcct_ID.getValue());

                //	Product
                if (f_M_Product_ID != null && !isNull(f_M_Product_ID.getValue()))
                    localquery.addRestriction("M_Product_ID", localquery.EQUAL, f_M_Product_ID.getValue());
                //	BPartner
                if (f_C_BPartner_ID != null && !isNull(f_C_BPartner_ID.getValue()))
                    localquery.addRestriction("C_BPartner_ID", localquery.EQUAL, f_C_BPartner_ID.getValue());
                //	Campaign
                if (f_C_Campaign_ID != null && !isNull(f_C_Campaign_ID.getValue()))
                    localquery.addRestriction("C_Campaign_ID", localquery.EQUAL, f_C_Campaign_ID.getValue());
                //	Loc From
                if (f_C_LocFrom_ID != null && !isNull(f_C_LocFrom_ID.getValue()))
                    localquery.addRestriction("C_LocFrom_ID", localquery.EQUAL, f_C_LocFrom_ID.getValue());
                //	Loc To
                if (f_C_LocTo_ID != null && !isNull(f_C_LocTo_ID.getValue()))
                    localquery.addRestriction("C_LocTo_ID", localquery.EQUAL, f_C_LocTo_ID.getValue());
                //	Project
                if (f_C_Project_ID != null && !isNull(f_C_Project_ID.getValue()))
                    localquery.addRestriction("C_Project_ID", localquery.EQUAL, f_C_Project_ID.getValue());
                //	SRegion
                if (f_C_SalesRegion_ID != null && !isNull(f_C_SalesRegion_ID.getValue()))
                    localquery.addRestriction("C_SalesRegion_ID", localquery.EQUAL, f_C_SalesRegion_ID.getValue());
                //	Org Trx
                if (f_AD_OrgTrx_ID != null && !isNull(f_AD_OrgTrx_ID.getValue()))
                    localquery.addRestriction("AD_OrgTrx_ID", localquery.EQUAL, f_AD_OrgTrx_ID.getValue());
                //	Activity
                if (f_C_Activity_ID != null && !isNull(f_C_Activity_ID.getValue()))
                    localquery.addRestriction("C_Activity_ID", localquery.EQUAL, f_C_Activity_ID.getValue());
                //	User 1
                if (f_User1_ID != null && !isNull(f_User1_ID.getValue()))
                    localquery.addRestriction("User1_ID", localquery.EQUAL, f_User1_ID.getValue());
                //	User 2
                if (f_User2_ID != null && !isNull(f_User2_ID.getValue()))
                    localquery.addRestriction("User2_ID", localquery.EQUAL, f_User2_ID.getValue());

                //	Query
                _mTab.setQuery(localquery);
                gridController.query(0, 0, false);
            }

            function actionIgnore() {
                if (f_Alias != null) {
                    f_Alias.setValue("");
                }
                f_Combination.setValue("");
                f_Description.Content = "";
                f_Description.Visible = false;
                lblbottumMsg2.val("");


                //
                //	Org (mandatory)
                f_AD_Org_ID.setValue(null);
                //	Account (mandatory)
                f_Account_ID.setValue(null);
                if (f_SubAcct_ID != null)
                    f_SubAcct_ID.setValue(null);

                //	Product
                if (f_M_Product_ID != null)
                    f_M_Product_ID.setValue(null);
                //	BPartner
                if (f_C_BPartner_ID != null)
                    f_C_BPartner_ID.setValue(null);
                //	Campaign
                if (f_C_Campaign_ID != null)
                    f_C_Campaign_ID.setValue(null);
                //	Loc From
                if (f_C_LocFrom_ID != null)
                    f_C_LocFrom_ID.setValue(null);
                //	Loc To
                if (f_C_LocTo_ID != null)
                    f_C_LocTo_ID.setValue(null);
                //	Project
                if (f_C_Project_ID != null)
                    f_C_Project_ID.setValue(null);
                //	SRegion
                if (f_C_SalesRegion_ID != null)
                    f_C_SalesRegion_ID.setValue(null);
                //	Org Trx
                if (f_AD_OrgTrx_ID != null)
                    f_AD_OrgTrx_ID.setValue(null);
                //	Activity
                if (f_C_Activity_ID != null)
                    f_C_Activity_ID.setValue(null);
                //	User 1
                if (f_User1_ID != null)
                    f_User1_ID.setValue(null);
                //	User 2
                if (f_User2_ID != null)
                    f_User2_ID.setValue(null);
            }

            function actionSave() {
                var sb = null;
                var sql = "SELECT C_ValidCombination_ID, Alias FROM C_ValidCombination WHERE ";
                var value = null;

                if (accountSchemaElements.IsHasAlies) {
                    value = f_Alias.getValue();
                    if (value == null && sb != null)
                        sb = sb.concat(VIS.Msg.translate(VIS.Env.getCtx(), "Alias")).concat(", ");
                }

                for (var i = 0; i < accountSchemaElements.Elements.length; i++) {
                    var ase = accountSchemaElements.Elements[i];
                    var isMandatory = accountSchemaElements.Elements[i].IsMandatory;
                    var type = accountSchemaElements.Elements[i].Type;
                    //
                    if (type.equals(eLEMENTTYPE_Organization)) {
                        value = f_AD_Org_ID.getValue();
                        sql = sql.concat("AD_Org_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_Account)) {
                        value = f_Account_ID.getValue();
                        sql = sql.concat("Account_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_SubAccount)) {
                        value = f_SubAcct_ID.getValue();
                        sql = sql.concat("C_SubAcct_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_Product)) {
                        value = f_M_Product_ID.getValue();
                        sql = sql.concat("M_Product_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_BPartner)) {
                        value = f_C_BPartner_ID.getValue();
                        sql = sql.concat("C_BPartner_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_Campaign)) {
                        value = f_C_Campaign_ID.getValue();
                        sql = sql.concat("C_Campaign_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_LocationFrom)) {
                        value = f_C_LocFrom_ID.getValue();
                        sql = sql.concat("C_LocFrom_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_LocationTo)) {
                        value = f_C_LocTo_ID.getValue();
                        sql = sql.concat("C_LocTo_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_Project)) {
                        value = f_C_Project_ID.getValue();
                        sql = sql.concat("C_Project_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_SalesRegion)) {
                        value = f_C_SalesRegion_ID.getValue();
                        sql = sql.concat("C_SalesRegion_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_OrgTrx)) {
                        value = f_AD_OrgTrx_ID.getValue();
                        sql = sql.concat("AD_OrgTrx_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_Activity)) {
                        value = f_C_Activity_ID.getValue();
                        sql = sql.concat("C_Activity_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_UserList1)) {
                        value = f_User1_ID.getValue();
                        sql = sql.concat("User1_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    else if (type.equals(eLEMENTTYPE_UserList2)) {
                        value = f_User2_ID.getValue();
                        sql = sql.concat("User2_ID");
                        if (isNull(value))
                            sql = sql.concat(" IS NULL AND ");
                        else
                            sql = sql.concat("=").concat(value).concat(" AND ");
                    }
                    //  
                    if (isMandatory && (value == null) && sb != null) {
                        sb = sb.concat(ase.getName()).concat(", ");
                    }
                }

                if (sb != null) {
                    VIS.ADialog.info("FillMandatory", true, sb.ToString().Substring(0, sb.Length - 2), null);
                    return;
                }
                if (f_AD_Org_ID == null || isNull(f_AD_Org_ID.getValue())) {
                    VIS.ADialog.info("FillMandatory", true, VIS.Msg.getElement(VIS.Env.getCtx(), "AD_Org_ID"), null);
                    return;
                }
                if (f_Account_ID == null || isNull(f_Account_ID.getValue())) {
                    VIS.ADialog.info("FillMandatory", true, VIS.Msg.getElement(VIS.Env.getCtx(), "Account_ID"), null);
                    $self.setBusy(false);
                    return;
                }

                //Check if already exists
                sql = sql.concat("AD_Client_ID=" + VIS.Env.getCtx().getAD_Client_ID() + " AND C_AcctSchema_ID=" + C_AcctSchema_ID);
                $self.log.fine("Check = " + sql.toString());

                //Check Alies Value
                var alies = f_Alias.getValue().toString();
                var IDvalue = 0;
                var Alias = null;
                var f_alies = f_Alias.getValue().toString();
                var dr = null;
                try {
                    dr = VIS.DB.executeReader(sql, null);
                    if (dr.read()) {
                        IDvalue = dr.tables[0].rows[0].cells["c_validcombination_id"];
                        Alias = dr.tables[0].rows[0].cells["alias"];
                    }
                    dr.close();
                    dr = null;
                }
                catch (e) {
                    //$self.log.Log(Level.SEVERE, sql.ToString(), e);
                    IDvalue = 0;
                    if (dr != null) {
                        dr.close();
                        dr = null;
                    }
                }

                $self.log.fine("ID=" + IDvalue + ", Alias=" + Alias);
                var fAlie
                if (Alias == null)
                    Alias = "";
                //	We have an account like this already - check alias
                if (IDvalue != 0 && accountSchemaElements.IsHasAlies && f_alies.equals(Alias)) {
                    sql = "UPDATE C_ValidCombination SET Alias=";
                    if (f_alies.toString().length == 0) {
                        sql = sql.concat("NULL");
                    }
                    else
                        sql = sql.concat("'").concat(f_alies).concat("'");
                    sql = sql.concat(" WHERE C_ValidCombination_ID=").concat(IDvalue);
                    var i = 0;
                    try {
                        i = VIS.DB.executeQuery(sql);
                    }
                    catch (e) {
                        //$self.log.Log(Level.SEVERE, sql.ToString(), e);
                    }
                }

                if (IDvalue != 0) {
                    loadInfo(IDvalue, C_AcctSchema_ID);
                    return;
                }

                //	load and display

                $self.log.config("New");
                Alias = null;
                if (f_Alias != null)
                    Alias = f_Alias.getValue().toString();
                var C_SubAcct_ID = 0;
                if (f_SubAcct_ID != null && (f_SubAcct_ID.getValue() != null))
                    C_SubAcct_ID = f_SubAcct_ID.getValue();
                var M_Product_ID = 0;
                if (f_M_Product_ID != null && !isNull(f_M_Product_ID.getValue()))
                    M_Product_ID = f_M_Product_ID.getValue();
                var C_BPartner_ID = 0;
                if (f_C_BPartner_ID != null && !isNull(f_C_BPartner_ID.getValue()))
                    C_BPartner_ID = f_C_BPartner_ID.getValue();
                var AD_OrgTrx_ID = 0;
                if (f_AD_OrgTrx_ID != null && !isNull(f_AD_OrgTrx_ID.getValue()))
                    AD_OrgTrx_ID = f_AD_OrgTrx_ID.getValue();
                var C_LocFrom_ID = 0;
                if (f_C_LocFrom_ID != null && !isNull(f_C_LocFrom_ID.getValue()))
                    C_LocFrom_ID = f_C_LocFrom_ID.getValue();
                var C_LocTo_ID = 0;
                if (f_C_LocTo_ID != null && !isNull(f_C_LocTo_ID.getValue()))
                    C_LocTo_ID = f_C_LocTo_ID.getValue();
                var C_SRegion_ID = 0;
                if (f_C_SalesRegion_ID != null && !isNull(f_C_SalesRegion_ID.getValue()))
                    C_SRegion_ID = f_C_SalesRegion_ID.getValue();
                var C_Project_ID = 0;
                if (f_C_Project_ID != null && !isNull(f_C_Project_ID.getValue()))
                    C_Project_ID = f_C_Project_ID.getValue();
                var C_Campaign_ID = 0;
                if (f_C_Campaign_ID != null && !isNull(f_C_Campaign_ID.getValue()))
                    C_Campaign_ID = f_C_Campaign_ID.getValue();
                var C_Activity_ID = 0;
                if (f_C_Activity_ID != null && !isNull(f_C_Activity_ID.getValue()))
                    C_Activity_ID = f_C_Activity_ID.getValue();
                var User1_ID = 0;
                if (f_User1_ID != null && !isNull(f_User1_ID.getValue()))
                    User1_ID = f_User1_ID.getValue();
                var User2_ID = 0;
                if (f_User2_ID != null && !isNull(f_User2_ID.getValue()))
                    User2_ID = f_User2_ID.getValue();

                var AD_Org_ID = f_AD_Org_ID.getValue();
                var AD_Account_ID = f_Account_ID.getValue();


                //Ajex to save Account onto database 

                $.ajax({
                    url: VIS.Application.contextUrl + "AccountForm/Save",
                    dataType: "json",
                    data: {
                        AD_Client_ID: VIS.Env.getCtx().getAD_Client_ID(),
                        AD_Org_ID: AD_Org_ID,
                        C_AcctSchema_ID: C_AcctSchema_ID,
                        AD_Account_ID: AD_Account_ID,
                        C_SubAcct_ID: C_SubAcct_ID,
                        M_Product_ID: M_Product_ID,
                        C_BPartner_ID: C_BPartner_ID,
                        AD_OrgTrx_ID: AD_OrgTrx_ID,
                        C_LocFrom_ID: C_LocFrom_ID,
                        C_LocTo_ID: C_LocTo_ID,
                        C_SRegion_ID: C_SRegion_ID,
                        C_Project_ID: C_Project_ID,
                        C_Campaign_ID: C_Campaign_ID,
                        C_Activity_ID: C_Activity_ID,
                        User1_ID: User1_ID,
                        User2_ID: User2_ID,
                        Alias: Alias
                    },
                    success: function (data) {
                        returnValue = data.result;
                        //load control
                        loadInfo(returnValue.C_ValidCombination_ID, returnValue.C_AcctSchema_ID);
                    }
                });
            };

            function loadInfo(C_ValidCombination_ID, C_AcctSchema_ID) {
                // this.log.fine("C_ValidCombination_ID=" + C_ValidCombination_ID);
                var sql = "SELECT * FROM C_ValidCombination WHERE C_ValidCombination_ID=" + C_ValidCombination_ID + " AND C_AcctSchema_ID=" + C_AcctSchema_ID;
                var dr = null;
                try {
                    dr = VIS.DB.executeReader(sql, null);
                    if (dr.read()) {
                        if (f_Alias != null)
                            f_Alias.setValue(dr.getString("Alias"));
                        if (f_Combination != null)
                            f_Combination.setValue(dr.getString("Combination"));
                        if (f_AD_Org_ID != null)
                            f_AD_Org_ID.setValue(dr.getInt("AD_Org_ID"));
                        if (f_Account_ID != null)
                            f_Account_ID.setValue(dr.getInt("Account_ID"));
                        if (f_SubAcct_ID != null)
                            f_SubAcct_ID.setValue(dr.getInt("C_SubAcct_ID"));
                        if (f_M_Product_ID != null)
                            f_M_Product_ID.setValue(dr.getInt("M_Product_ID"));
                        if (f_C_BPartner_ID != null)
                            f_C_BPartner_ID.setValue(dr.getInt("C_BPartner_ID"));
                        if (f_C_Campaign_ID != null)
                            f_C_Campaign_ID.setValue(dr.getInt("C_Campaign_ID"));
                        if (f_C_LocFrom_ID != null)
                            f_C_LocFrom_ID.setValue(dr.getInt("C_LocFrom_ID"));
                        if (f_C_LocTo_ID != null)
                            f_C_LocTo_ID.setValue(dr.getInt("C_LocTo_ID"));
                        if (f_C_Project_ID != null)
                            f_C_Project_ID.setValue(dr.getInt("C_Project_ID"));
                        if (f_C_SalesRegion_ID != null)
                            f_C_SalesRegion_ID.setValue(dr.getInt("C_SalesRegion_ID"));
                        if (f_AD_OrgTrx_ID != null)
                            f_AD_OrgTrx_ID.setValue(dr.getInt("AD_OrgTrx_ID"));
                        if (f_C_Activity_ID != null)
                            f_C_Activity_ID.setValue(dr.getInt("C_Activity_ID"));
                        if (f_User1_ID != null)
                            f_User1_ID.setValue(dr.getInt("User1_ID"));
                        if (f_User2_ID != null)
                            f_User2_ID.setValue(dr.getInt("User2_ID"));
                        if (lblbottumMsg2 != null)
                            lblbottumMsg2.val(dr.getString("Description"));

                    }
                    dr.close();
                    dr = null;
                }
                catch (e) {
                    if (dr != null) {
                        dr.close();
                        dr = null;
                    }
                }

                actionFind(false);
            }

            //Events
            Okbtn.on("click", function () {
                $self.setBusy(true);
                saveSelection();
                $self.setBusy(false);
            });

            cancelbtn.on("click", function () {
                root.dialog('close');
            });

            btnRefresh.on("click", function () {
                $self.setBusy(true);
                actionFind(true);
                $self.setBusy(false);
            });

            btnUndo.on("click", function () {
                actionIgnore();
            });

            btnSave.on("click", function () {
                $self.setBusy(true);
                actionSave();
                $self.setBusy(false);
            });

        };

        this.showDialog = function () {
            var w = $(window).width() - 50;
            var h = $(window).height() - 60;
            $busyDiv.height(h);
            $busyDiv.width(w);
            root.append($busyDiv);
            root.dialog({
                modal: false,
                resizable: false,
                title: title,
                width: w,
                height: h,
                position: { at: "center top", of: window },
                close: function () {
                    $self.dispose();
                    root.dialog("destroy");
                    root = null;
                    $self = null;
                }
            });
        };

        this.dataStatusChanged = function (e) {
            var info = _mTab.getValue("Description");
            //f_Description.getControl().val(info);

            _comb = _mTab.getValue("COMBINATION");

            if (info != null && info.length > 0) {
                lblbottumMsg2.css("display", "inline-block");
            }
            else {
                lblbottumMsg2.css("display", "none");
            }
            lblbottumMsg2.val(info);
            lblCount.val(e.totalRecords);
        };

        this.disposeComponent = function () {
            if (Okbtn) {
                Okbtn.off("click");
            }
            if (cancelbtn)
                cancelbtn.off("click");
            if (btnRefresh)
                btnRefresh.off("click");
            if (btnUndo)
                btnUndo.off("click");
            if (btnSave) {
                btnSave.off("click");
            }

            title = null;
            mAccount = null;
            C_AcctSchema_ID = null;
            windowNo = null;
            _comb = null;
            f_Description = null;
            f_Alias = null;
            f_Combination = null;
            f_AD_Org_ID = null; f_Account_ID = null; f_SubAcct_ID = null;
            f_M_Product_ID = null; f_C_BPartner_ID = null; f_C_Campaign_ID = null; f_C_LocFrom_ID = null; f_C_LocTo_ID = null;
            f_C_Project_ID = null; f_C_SalesRegion_ID = null; f_AD_OrgTrx_ID = null; f_C_Activity_ID = null;
            f_User1_ID = null; f_User2_ID = null;
            this.log = null;
            eLEMENTTYPE_AD_Reference_ID = null;
            eLEMENTTYPE_Account = null;
            eLEMENTTYPE_Activity = null;
            eLEMENTTYPE_BPartner = null;
            eLEMENTTYPE_LocationFrom = null;
            eLEMENTTYPE_LocationTo = null;
            eLEMENTTYPE_Campaign = null;
            eLEMENTTYPE_Organization = null;
            eLEMENTTYPE_OrgTrx = null;
            eLEMENTTYPE_Project = null;
            eLEMENTTYPE_Product = null;
            eLEMENTTYPE_SubAccount = null;
            eLEMENTTYPE_SalesRegion = null;
            eLEMENTTYPE_UserList1 = null;
            eLEMENTTYPE_UserList2 = null;
            eLEMENTTYPE_UserElement1 = null;
            eLEMENTTYPE_UserElement2 = null;
            tableSArea = null;
            tr = null;
            Okbtn = null;
            cancelbtn = null;
            btnRefresh = null;
            btnUndo = null;
            btnSave = null;
            lblParameter = null;
            parameterDiv = null;
            bottumDiv = null;
            discriptionDiv = null;
            accDiv = null;
            acctbl = null;
            gridController = null;
            changed = false;
            C_ValidCombination_ID = null;
            query = null;
            _mTab = null;
            lblbottumMsg = null;
            lblCount = null;
            lblbottumMsg2 = null;
            accountSchemaElements = null;
        };
    };

    AccountForm.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.AccountForm = AccountForm;

})(VIS, jQuery);