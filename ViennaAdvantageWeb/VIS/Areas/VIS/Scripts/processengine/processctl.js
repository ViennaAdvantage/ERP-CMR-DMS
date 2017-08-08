; (function (VIS, $) {

    /**
     *	Process Interface Controller.
     *
     */
    function ProcessCtl(parent, pi, trx) {
        this.parent = parent;
        this.pi = pi;
        this.windowNo;
        this.paraList = null;
        this.isPdf = false;
        this.isCsv = false;
    };

    ProcessCtl.prototype.setIsPdf = function (ispdf) {
        this.isPdf = ispdf;
    };

    ProcessCtl.prototype.setIsCsv = function (iscsv) {
        this.isCsv = iscsv;
    };

    /**
	 *	Async Process - Do it all.
	 *  <code>
	 *	- Get Instance ID
	 *	- Get Parameters
	 *	- execute (lock - start process - unlock)
	 *  </code>
	 *  Creates a ProcessCtl instance, which calls
	 *  lockUI and unlockUI if parent is a ASyncProcess
	 *  <br>
	 *	Called from ProcessCtl.startProcess, ProcessDialog.actionPerformed,
	 *  APanel.cmd_print, APanel.actionButton, VPaySelect.cmd_generate
	 *
	 *  @param WindowNo window no
	 */
    ProcessCtl.prototype.process = function (windowNo) {

        this.lock();
        this.windowNo = windowNo;

        var self = this; //self pointer

        var data = {
            AD_Process_ID: this.pi.getAD_Process_ID(),
            Name: this.pi.getTitle(),
            AD_Table_ID: this.pi.getTable_ID(),
            Record_ID: this.pi.getRecord_ID(),
            WindowNo: windowNo,
            csv: this.isCsv,
            pdf: this.isPdf
        }

        VIS.dataContext.process(data, function (jsonStr) {

            if (jsonStr.error != null) {
                self.pi.setSummary(jsonStr.error, true);
                self.unlock();
                self = null;
                return;
            }
            var json = JSON.parse(jsonStr.result);
            if (json.IsError) {
                self.pi.setSummary(json.Message, true);
                self.unlock();
                self = null;
                return;
            }

            self.complete(json);
            self = null;

        });
    }; //process

    /**
	 *	Execute Process complete.
	 *  Calls unlockUI if parent is a ASyncProcess
     *  @param jObject process data contract from server
	 */
    ProcessCtl.prototype.complete = function (jObject) {

        if (jObject.ShowParameter) { //Open Paramter Dialog
            this.pi.setAD_PInstance_ID(jObject.AD_PInstance_ID);
            try {
                var pp = new VIS.ProcessParameter(this.pi, this, this.windowNo);
                pp.initDialog(jObject.ProcessFields);
                pp.showDialog();
                pp = null;
            }
            catch (e) {

                this.pi.setSummary(e.message, true);
            }
            this.unlock();
        }
        else {
            this.pi.dispose(); // dispose current pi
            this.pi = this.pi.fromJson(jObject.ReportProcessInfo);

            //if (jObject.ReportFilePath && jObject.ReportFilePath.length > 10) { //report
            //    if (this.parent) {
            //        this.parent.showReport(new  VIS.PdfViewer());
            //    }
            //}

            this.unlock();
            if (jObject.HTML && jObject.HTML.length > 0) {
                if (this.parent) {
                    if (jObject.AskForNewTab) {
                        if (VIS.ADialog.ask("VIS_OpenNewTab")) {
                            window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                        }
                    }
                    else {
                        this.parent.setReportBytes(jObject.Report);
                        this.parent.showReport(new VIS.PdfViewer(jObject.HTML, null, true), jObject.AD_PrintFormat_ID, this, this.windowNo, this.paraList, jObject.AD_Table_ID);
                    }
                }
            }
            else {
                if (jObject.ReportFilePath && jObject.ReportFilePath.length > 0) {
                    window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
                }
            }

            //if (jObject.IsRCReport && jObject.ReportFilePath && jObject.ReportFilePath.length > 10) { //report
            //    if (this.parent) {

            //        //var ismobile = /ipad|iphone|ipod|mobile|android|blackberry|webos|windows phone/i.test(navigator.userAgent.toLowerCase());
            //        //if (ismobile) {
            //        //    // window.setTimeout(function () {
            //            window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
            //        //    // }, 200);                      
            //        //}
            //        //else {
            //        //    this.parent.setReportBytes(jObject.Report);
            //        //    this.parent.showReport(new VIS.PdfViewer(jObject.ReportFilePath), jObject.AD_PrintFormat_ID, jObject.AD_PrintFormat_ID, this, this.windowNo, this.paraList, jObject.AD_Table_ID);

            //        //}
            //    }
            //}
            //else if ((jObject.ReportFilePath && jObject.ReportFilePath.length>10) || (jObject.HTML && jObject.HTML.length > 0)) { //report
            //    if (this.parent) {
            //        //this.parent.showReport(new VIS.PdfViewer(jObject.ReportFilePath));
            //        //window.open(VIS.Application.contextUrl + jObject.ReportFilePath, '_blank');
            //        if (jObject.AskForNewTab) {
            //            if (VIS.ADialog.ask("VIS_OpenNewTab")) {
            //                window.open(VIS.Application.contextUrl + jObject.ReportFilePath);
            //            }
            //        }
            //        else {
            //            this.parent.setReportBytes(jObject.Report);
            //            this.parent.showReport(new VIS.PdfViewer(jObject.HTML, null, true), jObject.AD_PrintFormat_ID, this, this.windowNo, this.paraList, jObject.AD_Table_ID);
            //        }
            //    }
            //}

            this.dispose();

        }
    };

    /**
    *	handle process parameter closed event
    *   - Calls lock and unlock
    *   - call async process excute function
    *   
    *  @param isOk userclick Ok button 
    *  @param gridfield array  for each process parameter  if any
    */
    ProcessCtl.prototype.onProcessDialogClosed = function (isOK, paraList) {
        if (isOK) { //Ok clicked
            this.paraList = paraList;
            this.lock();
            var self = this;
            var data = {
                AD_Process_ID: this.pi.getAD_Process_ID(),
                AD_PInstance_ID: this.pi.getAD_PInstance_ID(),
                Name: this.pi.getTitle(),
                AD_Table_ID: this.pi.getTable_ID(),
                Record_ID: this.pi.getRecord_ID(),
                ParameterList: paraList,
                csv: this.isCsv,
                pdf: this.isPdf
            }

            VIS.dataContext.executeProcess(data, function (jsonStr) {
                if (jsonStr.error != null) {
                    self.pi.setSummary(jsonStr.error, true);
                    self.unlock();
                    self = null;
                    return;
                }
                var json = JSON.parse(jsonStr.result);
                if (json.IsError) {
                    self.pi.setSummary(json.Message, true);
                    self.unlock();
                    self = null;
                    return;
                }

                self.complete(json); // call process complete
                self = null;

            });
        }
        else {

        }
    };

    /**
	 *  Unlock UI & dispose 
	 */
    ProcessCtl.prototype.unlock = function () {
        var summary = this.pi.getSummary();
        if (summary != null && summary.indexOf("@") != -1) {
            this.pi.setSummary(VIS.Msg.parseTranslation(VIS.Env.getCtx(), summary));
        }
        if (this.parent) {
            this.parent.unlockUI(this.pi);
        }
    };//unlock

    /**
	 *  Lock UI & show Waiting
	 */
    ProcessCtl.prototype.lock = function () {
        if (this.parent)
            this.parent.lockUI(this.pi);
    };//lock

    /**
    *  dispose
    */
    ProcessCtl.prototype.dispose = function () {
        this.parent = null;
        if (this.pi)
            this.pi.dispose();
        this.pi = null;
    };



    function PdfViewer(rptName, pi, isHtml) {
        this.bytes = null;
        this.RptFileName = rptName;
        this.Pi = pi;

        var $root = null;
        var $leftDiv = null, $rightDiv = null; $innerRightDiv = null;
        var $object = null;
        var $btn = "";
        var bsyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        function initializedComponenet() {

            $root = $("<div class='vis-height-full'>");
            $rightDiv = $("<div class='vis-height-full' style='overflow:auto;background: #63BFE9;'>");
            $innerRightDiv = $("<div class='vis-height-full' style='padding:5px;'>");
            $rightDiv.append($innerRightDiv);
            $rightDiv.append(bsyDiv);
            $root.append($rightDiv);
            if (isHtml) {
                $innerRightDiv.html(rptName);
            }
            else {
                $object = $("<iframe style = 'width:100%;height:100%;' pluginspage='http://www.adobe.com/products/acrobat/readstep2.html'>");
                $object.attr("src", VIS.Application.contextUrl + rptName);
                $rightDiv.append($object);
            }
        };

        initializedComponenet();

        //$leftDiv.on("click", function () {
        //    setTimeout(function () {
        //        $object.get(0).contentWindow.focus();
        //        $object.get(0).contentWindow.print();
        //    }, 100);
        //});

        this.getRoot = function () {
            return $root;
        };
        this.getRightDiv = function () {
            return $rightDiv;
        };
        this.getRightInnerDiv = function () {
            return $innerRightDiv;
        };
        this.getIsHtmlReport = function () {
            return isHtml;
        };
        this.getHtml = function () {
            return rptName;
        };

        this.disposeComponent = function () {
            if ($root != null) {
                $root.remove();
                $root = null;
            }
        };
        this.setBusy = function (isBusy) {
            bsyDiv.css("display", isBusy ? 'block' : 'none');
        };
    };

    PdfViewer.prototype.dispose = function () {
        this.disposeComponent();
    };

    VIS.PdfViewer = PdfViewer;
    //global assignment
    VIS.ProcessCtl = ProcessCtl;

})(VIS, jQuery);