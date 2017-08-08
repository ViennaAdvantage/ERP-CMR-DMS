/********************************************************
 * Module Name    : VIS
 * Purpose        : Print payment Selection
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     18 May 2015
 ******************************************************/
; VIS = window.VIS || {};
; (function (VIS, $) {
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    function VPayPrint() {
        this.windowNo = null;
        var $root = $("<div style='width: 100%; height: 100%; background-color: white;'>");
        var $self = this;
        var $divContainer = null;
        var $cmbPaymentSelect = null;
        var $txtBankAccount = null;
        var $txtCurrentBalance = null;
        var $cmbPaymentMethod = null;
        var $txtCurreny = null;
        var $txtCheckNo = null;
        var $txtNoOfPayments = null;
        var $chkShowPrintedPayments = null;
        var $btnPrint = null;
        var $btnExport = null;
        var $btnCancel = null;
        var C_BankAccount_ID = null;
        var $divBusy = null;
        this.initialize = function () {
            customDesign();

        };
        //*****************
        //Load BusyDiv
        //*****************
        function busyIndicator() {
            $divBusy = $("<div>");
            $divBusy.css({
                "position": "absolute", "bottom": "0", "background": "url('" + VIS.Application.contextUrl + "Areas/VIS/Images/busy.gif') no-repeat", "background-position": "center center",
                "width": "98%", "height": "98%", 'text-align': 'center', 'opacity': '.1', 'z-index': '9999999'
            });
            $divBusy[0].style.visibility = "hidden";
            $root.append($divBusy);
        };
        //*****************
        //Show/Hide Busy Indicator
        //*****************
        function isBusy(show) {
            if (show) {
                $divBusy[0].style.visibility = "visible";
            }
            else {
                $divBusy[0].style.visibility = "hidden";
            }
        };
        //******************
        //Custom Design of Paymnet Selection Form
        //******************
        function customDesign() {
            $divContainer = $("<div class='vis-mainContainer'>");
            var designPSelectInfo = " <div class='vis-pPrintInfo'>"  // div pSelectInfo starts here                            
                             + " <div class='vis-paymentselect-field'>"  // div PaySelection starts here
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_PaySelection_ID") + " </label>"
                             + " <div class='vis-paymentPrintChk-field'>"  // div ShowPrintedPayments starts here
                             + " <input type='checkbox' id='VIS_chkShowPrintPayment_" + $self.windowNo + "' style='height: 15px;width: auto;'></input>"
                             + " <label for='VIS_chkOnlyDue_" + $self.windowNo + "'>" + VIS.Msg.getMsg("ShowPrintedPayments") + " </label>"
                             + " </div>" // div ShowPrintedPayments ends here 
                             + " <select id='VIS_PaySelection_" + $self.windowNo + "' style='background-position: 99%;'></select>"
                             + " </div>" // div PaySelection ends here 
                             + " <div class='vis-paymentPrint-field' style='margin-right:24px;'>"  // div bankAccount starts here
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_BankAccount_ID") + " </label>"
                             + " <input type='text' name='vis-name vis-fieldreadonly' style='background: #f8f8f8 !important;' disabled id='VIS_txtBankAccount_" + $self.windowNo + "' MaxLength='50'></input>"
                             + " </div>" // div bankAccount ends here 
                             + " <div class='vis-paymentPrint-field'>"  // div currentBalance starts here
                             + " <label>" + VIS.Msg.getMsg("CurrentBalance") + " </label>"
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtCurrentBal_" + $self.windowNo + "' MaxLength='50'></input>"
                             + " </div>" // div currentBalance ends here                             
                             + " <div class='vis-paymentPrint-field' style='margin-right:24px;'>"  // div paymentMethod starts here
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "PaymentRule") + " </label>"
                             + " <select name='vis-name' id='VIS_cmbPaymentMethod_" + $self.windowNo + "'></select>"
                             + " </div>" // div paymentMethod ends here 
                             + " <div class='vis-paymentPrint-field'>"  // div Currency starts here
                             + " <label>" + VIS.Msg.translate(VIS.Env.getCtx(), "C_Currency_ID") + " </label>"
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtCurrency_" + $self.windowNo + "' MaxLength='50'></input>"
                             + " </div>" // div Currency ends here                            
                             + " <div class='vis-paymentPrint-field' style='margin-right:24px;'>"  // div CheckNo starts here
                             + " <label>" + VIS.Msg.getMsg("CheckNo") + " </label>"
                             + " <input type='number' name='vis-name'  id='VIS_txtCheckNo_" + $self.windowNo + "' MaxLength='50'></input>"
                             + " </div>" // div CheckNo ends here    
                             + " <div class='vis-paymentPrint-field'>"  // div NoOfPayments starts here
                             + " <label>" + VIS.Msg.getMsg("NoOfPayments") + " </label>"
                             + " <input type='text' class='vis-fieldreadonly' disabled id='VIS_txtNoOfPayments_" + $self.windowNo + "' MaxLength='50'></input>"
                             + " </div>" // div NoOfPayments ends here  
                             + " </div>" // div pSelectInfo ends here      

            var designPSelectProcess = " <div class='vis-pPrintProcess'>"  // div pSelectProcess starts here
                                     + " <div class='vis-paymentPrint-field' style='float:right;'>"  // div starts here  
                                      + " <input id='VIS_btnCancel_" + $self.windowNo + "' style='background-color:#616364;color: white;font-weight: 200;font-family: helvetica;font-size: 14px;padding: 10px 15px;float:right;width:100px;margin-top:10px;margin-left:10px;height:40px;' type='submit' value='" + VIS.Msg.getMsg("Cancel") + "' ></input>"
                                      + " <input id='VIS_btnExport_" + $self.windowNo + "' style='background-color:#616364;color: white;font-weight: 200;font-family: helvetica;font-size: 14px;padding: 10px 15px;float:right;width:100px;margin-top:10px;margin-left:10px;height:40px;' type='submit' value='" + VIS.Msg.getMsg("Export") + "' ></input>"
                                      + " <input id='VIS_btnPrint_" + $self.windowNo + "' style='background-color:#616364;color: white;font-weight: 200;font-family: helvetica;font-size: 14px;padding: 10px 15px;float:right;width:100px;margin-top:10px;margin-left:10px;height:40px;' type='submit' value='" + VIS.Msg.getMsg("Print") + "' ></input>"
                                     + " </div>" // div ends here 
                                     + " </div>" // div pSelectProcess ends here 
            $divContainer.append($(designPSelectInfo)).append($(designPSelectProcess));
            $root.append($divContainer);
            busyIndicator();
            findControls();

        };
        //******************
        //Find Controls through ID
        //******************
        function findControls() {
            $cmbPaymentSelect = $('#VIS_PaySelection_' + $self.windowNo);
            $txtBankAccount = $('#VIS_txtBankAccount_' + $self.windowNo);
            $txtCurrentBalance = $('#VIS_txtCurrentBal_' + $self.windowNo);
            $cmbPaymentMethod = $('#VIS_cmbPaymentMethod_' + $self.windowNo);
            $txtCurreny = $('#VIS_txtCurrency_' + $self.windowNo);
            $txtCheckNo = $('#VIS_txtCheckNo_' + $self.windowNo);
            $txtNoOfPayments = $('#VIS_txtNoOfPayments_' + $self.windowNo);
            $chkShowPrintedPayments = $('#VIS_chkShowPrintPayment_' + $self.windowNo);
            $btnPrint = $('#VIS_btnPrint_' + $self.windowNo);
            $btnExport = $('#VIS_btnExport_' + $self.windowNo);
            $btnCancel = $('#VIS_btnCancel_' + $self.windowNo);
            loadFormData(true);
            eventHandling();

        };
        //******************
        //Load Data on Form Load
        //******************
        function loadFormData(isFirstTime) {
            var pSelectID = null;
            if (isFirstTime) {
                pSelectID = 0;
            }
            else {
                pSelectID = $cmbPaymentSelect.val();
            }
            isBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/GetDetail",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { showPrintedPayment: $chkShowPrintedPayments.prop("checked"), C_PaymentSelect_ID: pSelectID, isFirstTime: isFirstTime },
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    controlsData = data;
                    if (data != null || data != undefined) {
                        fillData(data);
                        LoadPaymentRuleInfo();
                        isBusy(false);
                    }
                },
                error: function (e) {
                    isBusy(false);
                    console.log(e);
                }
            });
        };
        //******************
        //Load Data on Form Load
        //******************
        function LoadPaymentRuleInfo() {
            debugger;
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/LoadPaymentRuleInfo",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { paymentMethod_ID: $cmbPaymentMethod.val(), C_PaySelection_ID: $cmbPaymentSelect.val(), m_C_BankAccount_ID: C_BankAccount_ID, PaymentRule: $cmbPaymentMethod.text() },
                success: function (jsonResult) {
                    var data = JSON.parse(jsonResult);
                    isBusy(false);
                    if (data != null || data != undefined) {
                        $txtCheckNo.val(data.CheckNo);
                        $txtNoOfPayments.val(data.NoOfPayments);
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //Fill Data in Controls
        //******************
        function fillData(data) {
            try {
                //* Load PaymentSelection
                if (data.PaymentSelection != null || data.PaymentSelection.length > 0) {
                    $cmbPaymentSelect.empty();
                    for (var i in data.PaymentSelection) {
                        $cmbPaymentSelect.append($('<Option value="' + data.PaymentSelection[i].ID + '">' + data.PaymentSelection[i].Value + '</option>'));
                    }
                }
                //* Load PaymentMethod
                if (data.PaymentMethod != null || data.PaymentMethod.length > 0) {
                    $cmbPaymentMethod.empty();
                    for (var i in data.PaymentMethod) {
                        $cmbPaymentMethod.append($('<Option value="' + data.PaymentMethod[i].ID + '">' + data.PaymentMethod[i].Value + '</option>'));
                    }
                }
                //* Load Another Values
                if (data.PSelectInfo != null || data.PSelectInfo.length > 0) {
                    $txtBankAccount.val(data.PSelectInfo[0].BankAccount);
                    $txtCheckNo.val(data.PSelectInfo[0].CheckNo);
                    $txtCurrentBalance.val(data.PSelectInfo[0].CurrentBalance);
                    $txtCurreny.val(data.PSelectInfo[0].Currency);
                    $txtNoOfPayments.val(data.PSelectInfo[0].NoOfPayments);
                    C_BankAccount_ID = data.PSelectInfo[0].BankAccount_ID;
                }
            }
            catch (err) {
                VIS.ADialog.info("VPayPrintNoRecords");
                isBusy(false);
                return false;

            }
            if ($cmbPaymentSelect.val() == null) {
                VIS.ADialog.info("VPayPrintNoRecords");
                return false;
            }
        };
        //******************
        //EventHandling
        //******************
        function eventHandling() {
            //**On click of Print Button**//
            $btnPrint.on("click", function () {
                if (validateInputOnPrint()) {
                    cmd_Print();
                }
            });
            $btnExport.on("click", function () {
                if (validateInputOnPrint()) {
                    cmd_Export();
                }
            });
            //**On click of Cancel Button**//
            $btnCancel.on("click", function () {
                $self.dispose();
            });

            //**On change of PaymentSelection($cmbPaymentSelect) **//
            $cmbPaymentSelect.on("change", function () {
                isBusy(true);
                window.setTimeout(function () {
                    loadFormData(false);
                    isBusy(false);
                }, 20);
            });

            //**On change of PaymentMethod($cmbPaymentMethod) **//
            $cmbPaymentMethod.on("change", function () {
                isBusy(true);
                window.setTimeout(function () {
                    LoadPaymentRuleInfo();
                    isBusy(false);
                }, 20);
            });

            //**On checked change of Show only Printed Payments checkbox **//
            $chkShowPrintedPayments.on("click", function () {
                isBusy(true);
                window.setTimeout(function () {
                    loadFormData(false);
                    isBusy(false);
                }, 20);
            });


        };
        //******************
        //validateInputOnPrint
        //******************
        function validateInputOnPrint() {
            try {
                if ($cmbPaymentSelect == null || $cmbPaymentSelect.val() <= -1 || C_BankAccount_ID <= -1 || $cmbPaymentMethod.val() <= -1 || $txtCheckNo.val().length <= 0) {
                    VIS.ADialog.info("VPayPrintNoRecords");
                    return false;
                }
            }
            catch (err) {
                VIS.ADialog.info("VPayPrintNoRecords");
                return false;
            }
            return true;
        };
        //******************
        //Cmd Print
        //******************
        function cmd_Print() {
            isBusy(true);

            // var printParent = new VIS.AProcess(300); //initlize AForm

            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/Cmd_Print",
                async: false,
                datatype: "Json",
                type: "GET",
                cache: false,
                data: { paymentMethod_ID: $cmbPaymentMethod.val(), C_PaySelection_ID: $cmbPaymentSelect.val(), m_C_BankAccount_ID: C_BankAccount_ID, PaymentRule: $cmbPaymentMethod.text(), checkNo: $txtCheckNo.val() },
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        if (VIS.ADialog.ask("ContinueCheckPrint?")) {
                            isBusy(true);
                            window.setTimeout(function () {

                                var ad_process_id = VIS.DB.executeScalar("select ad_process_id from ad_process where value = 'CheckPrintFormat'");
                                var ad_table_id = VIS.DB.executeScalar("select ad_table_id from ad_table where tablename = 'C_PaySelectionCheck'");

                                //for (var j = 0; j < data.check_ID; j++) {
                                var pi = new VIS.ProcessInfo(null, ad_process_id, ad_table_id, parseInt(data.check_ID[0]));
                                // var pi = new VIS.ProcessInfo(null, 1000196, 525, 1000078);
                                pi.setAD_User_ID(VIS.context.getAD_User_ID());
                                pi.setAD_Client_ID(VIS.context.getAD_Client_ID());
                                var ctl = new VIS.ProcessCtl($self, pi, null);
                                ctl.setIsPdf(true);
                                ctl.process($self.windowNo); //call dispose intenally
                                //ctl = null;

                                // }

                                //var data = {
                                //    AD_Process_ID: pi.getAD_Process_ID(),
                                //    AD_PInstance_ID: pi.getAD_PInstance_ID(),
                                //    Name: pi.getTitle(),
                                //    AD_Table_ID: pi.getTable_ID(),
                                //    Record_ID: pi.getRecord_ID(),
                                //    ParameterList: null,
                                //    csv: false,
                                //    pdf: true
                                //};
                                //VIS.dataContext.executeProcess(data, function (jsonStr) {
                                //    if (jsonStr.error != null) {
                                //        pCtl.pi.setSummary(jsonStr.error, true);
                                //        pCtl.unlock();
                                //        pCtl = null;
                                //        return;
                                //    }
                                //    var json = JSON.parse(jsonStr.result);
                                //    if (json.IsError) {
                                //        pCtl.pi.setSummary(json.Message, true);
                                //        pCtl.unlock();
                                //        pCtl = null;
                                //        return;
                                //    }
                                //    window.open(VIS.Application.contextUrl + json.ReportFilePath);
                                //    continueCheckPrint(data);

                                //})
                                continueCheckPrint(data);

                            }, 20);
                        }
                    }
                    else {
                        VIS.ADialog.info("VPayPrintNoRecords");
                        return false;
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //ContinueCheckPrint
        //******************
        function continueCheckPrint(data) {
            var param = [];
            param.push($cmbPaymentMethod.val());
            param.push($cmbPaymentSelect.val());
            param.push(C_BankAccount_ID);
            param.push($cmbPaymentMethod.text());
            param.push($txtCheckNo.val());
            param.push(JSON.stringify(data.check_ID));
            param.push(JSON.stringify(data.m_batch));
            param.push(JSON.stringify(data.m_checks));
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/ContinueCheckPrint",
                async: false,
                datatype: "Json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(param),
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        if (VIS.ADialog.ask("VPayPrintPrintRemittance?")) {
                            isBusy(true);
                            window.setTimeout(function () {
                                vPayPrintPrintRemittance(jsonResult);
                            }, 20);
                        }
                    }
                },
                error: function (e) {
                    isBusy(false);
                    console.log(e);
                }
            });
        };
        //******************
        //VPayPrintPrintRemittance
        //******************
        function vPayPrintPrintRemittance(data) {
            var paymentID = data;
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/VPayPrintRemittance",
                async: false,
                datatype: "Json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: data,
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (paymentID != null || paymentID != undefined) {
                        var ad_process_id = VIS.DB.executescalar("select ad_process_id from ad_process where value = 'remittanceprintformat'");
                        var ad_table_id = VIS.DB.executescalar("select ad_table_id from ad_table where tablename = 'c_payment'");
                       // for (var j = 0; j < data.check_id; j++) {
                        var pi = new VIS.processinfo(null, ad_process_id, ad_table_id, paymentID[0]);
                            pi.setad_user_id(VIS.context.getad_user_id());
                            pi.setad_client_id(VIS.context.getad_client_id());
                            var ctl = new VIS.processctl($self, pi, null);
                            ctl.setispdf(true);
                            ctl.process($self.windowno); //call dispose intenally
                            //ctl = null;

                       // }
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //Cmd Export
        //******************
        function cmd_Export() {
            isBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/Cmd_Export",
                async: false,
                datatype: "Json",
                type: "POST",
                cache: false,
                data: { paymentMethod_ID: $cmbPaymentMethod.val(), C_PaySelection_ID: $cmbPaymentSelect.val(), m_C_BankAccount_ID: C_BankAccount_ID, PaymentRule: $cmbPaymentMethod.text(), checkNo: $txtCheckNo.val() },
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {
                        if (VIS.ADialog.ask("VPayPrintSuccess?")) {
                            isBusy(true);
                            window.setTimeout(function () {
                                vPayPrintSuccess(data);
                            }, 20);
                            var url = VIS.Application.contextUrl + "TempDownload/" + data.filepath;
                            window.open(url, '_blank');

                        }
                    }
                    else {
                        VIS.ADialog.info("VPayPrintNoRecords");
                        return false;
                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //******************
        //VPayPrintSuccess
        //******************
        function vPayPrintSuccess(data) {
            var param = [];
            param.push(JSON.stringify(data.check_ID));
            param.push(JSON.stringify(data.m_batch));
            param.push(JSON.stringify(data.m_checks));
            $.ajax({
                url: VIS.Application.contextUrl + "VPayPrint/VPayPrintSuccess",
                async: false,
                datatype: "Json",
                type: "POST",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(param),
                success: function (jsonResult) {
                    isBusy(false);
                    var data = JSON.parse(jsonResult);
                    if (data != null || data != undefined) {

                    }
                },
                error: function (e) {
                    console.log(e);
                }
            });
        };
        //*******************
        //Get Root
        //*******************
        this.getRoot = function () {
            return $root;
        };


        this.lockUI = function (abc) {

        };

        this.unlockUI = function (abc) {

        };


        //********************
        //Dispose Component
        //********************
        this.disposeComponent = function () {
            windowNo = null;
            $root = null;
            $self = this;

        };
    };

    //Must Implement with same parameter
    VPayPrint.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
        this.initialize();
    };

    //Must implement dispose
    VPayPrint.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };
    VIS.Apps.AForms.VPayPrint = VPayPrint;
})(VIS, jQuery);