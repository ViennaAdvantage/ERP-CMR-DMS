; (function (VIS, $) {

    /**
     *	Dialog to Start process.
     *	Displays information about the process
     *		and lets the user decide to start it
     *  	and displays results (optionally print them).
     *  Calls ProcessCtl to execute.
     *
     */
    function AProcess(height) {

        this.parent;
        this.mPanel;
        this.ctx = VIS.Env.getCtx();
        this.isLocked = false;
        this.log = VIS.Logging.VLogger.getVLogger("VIS.AProcess");
        this.isComplete = false;
        this.jpObj = null; //Jsong Process Info Object

        //InitComponenet

        var $root, $busyDiv, $contentGrid, $table;
        var $btnOK, $btnClose, $text
        function initilizedComponent() {

            $root = $("<div style='position:relative;'>");
            $busyDiv = $("<div class='vis-apanel-busy'>");
            $table = $("<table  class='vis-process-table'>");

            $table.height(height);
            $busyDiv.height(height);

            $contentGrid = $("<td>");
            $table.append($("<tr>").append($contentGrid));
            $root.append($table).append($busyDiv); // append to root



            $btnOK = $("<button class='vis-button-ok'>").text(VIS.Msg.getMsg("OK"));
            $btnClose = $("<button class='vis-button-close'>").text(VIS.Msg.getMsg("Close"));
            $text = $("<span>").val("asassasasasasaasa");
            //ProcessDialog
            $contentGrid.append($text);
            $contentGrid.append($btnOK).append($btnClose);


        }
        initilizedComponent();

        this.setSize = function (height, width) {
            if ($root) {
                $root.height(height); //height 
                $busyDiv.height(height);
                $contentGrid.height(height);
            }
        }
        this.setSize(height);

        //privilized function
        this.getRoot = function () { return $root; };
        this.getContentGrid = function () { return $contentGrid; };
        this.setBusy = function (busy, focus) {
            isLocked = busy;
            if (busy) {
                //		setStatusLine(processing);
                $busyDiv[0].style.visibility = 'visible';// .show();
            }
            else {
                //$busyDiv.hide();
                $busyDiv[0].style.visibility = 'hidden';
                if (focus) {
                    //curGC.requestFocusInWindow();
                }
            }
        };
        this.setMsg = function (msg) {
            $text.append(msg);
        };


        var self = this; //self pointer


        $btnOK.on(VIS.Events.onTouchStartOrClick, function () { //click event

            if (self.isComplete) {
                self.dispose();
                return;
            }


            var pi = new VIS.ProcessInfo(self.jpObj.Name, self.jpObj.AD_Process_ID, 0, 0);
            pi.setAD_User_ID(self.ctx.getAD_User_ID());
            pi.setAD_Client_ID(self.ctx.getAD_Client_ID());
            var ctl = new VIS.ProcessCtl(self, pi, null);
            ctl.process(self.windowNo); //call dispose intenally
            ctl = null;
        });

        $btnClose.on(VIS.Events.onTouchStartOrClick, function () {
            self.parent.dispose();
        });

        this.disposeComponent = function () {
            if ($btnOK)
                $btnOK.off(VIS.Events.onTouchStartOrClick);
            if ($btnClose)
                $btnClose.off(VIS.Events.onTouchStartOrClick);
            if ($root)
                $root.remove();
            $root = null;
            if ($contentGrid)
                $contentGrid.remove();
            $contentGrid = null;
            if ($busyDiv)
                $busyDiv.remove();
            $busyDiv = null;
            $btnOK = $btnClose = $text = null;
            this.$parentWindow = null;
            //this.ctx = null;
            this.isComplete = false;
            //this.setSize = null;
            this.jpObj = null;
            self = null;
        }

    };

    /**
	 *	Dynamic Init
	 *  @return true, if there is something to process (start from menu)
	 */
    AProcess.prototype.init = function (json, $parent, windowNo) {
        
        //if (json.IsReport) {
        //    VIS.ADialog.info("Process Reoprt is not supported yet");
        //    return fasle;
        //}
        if (json.Name == null || json.Name == "") {
            VIS.ADialog.info("Process Name is empty");
            return fasle;
        }

        this.ctx.setWindowContext(windowNo, "IsSOTrx", json.IsSOTrx);

        $parent.getContentGrid().append(this.getRoot());
        this.setMsg(json.MessageText);
        this.setBusy(false);

        this.parent = $parent; // this parameter
        this.jpObj = json;
        this.windowNo = windowNo;
        return true;
    };

    AProcess.prototype.setTitle = function (title) {
        if (this.parent)
            this.parent.setTitle(VIS.Utility.Util.cleanMnemonic(title));
    };

    /**
	 *  Lock User Interface
	 *  Called from the process before processing
	 *  @param pi process info
	 */
    AProcess.prototype.lockUI = function (pi) {
        this.isLocked = true;
        this.setBusy(true);
    };

    /**
	 *  Unlock User Interface.
	 *  Called from the complete when processing is done
	 *  @param pi process info
	 */
    AProcess.prototype.unlockUI = function (pi) {
        VIS.ProcessInfoUtil.setLogFromDB(pi);

        var msg = "";
        msg = "<p><font color=\"" + (pi.getIsError() ? "#FF0000" : "#0000FF") + "\">** " +
            pi.getSummary() + "</font></p>";

        msg += pi.getLogInfo(true);

        this.setMsg(msg);
        console.log(msg);
        //btnOK.IsEnabled = true;
        this.isComplete = true;
        this.setBusy(false);
        this.isLocked = false;
    };

    /**
	 *  Is the UI locked (Internal method)
	 *  @return true, if UI is locked
	 */
    AProcess.prototype.getIsUILocked = function () {
        return this.isLocked;
    };

    /**
	 *  clean up
	 *  @return true, if UI is locked
	 */
    AProcess.prototype.dispose = function () {
        if (this.disposed)
            return;
        this.disposed = true;
        if (this.mPanel) {
            this.mPanel.dispose();
            this.mPanel = null;
        }

        if (this.parent) {
            this.parent.dispose();
            this.parent = null;
        }
        VIS.Env.clearWinContext(this.ctx, this.windowNo);

        this.disposeComponent();
    };

   
    AProcess.prototype.showReport = function (panel, AD_PrintFormat_ID, _pCtl, _winNo, _paraList, _AD_Table_ID) {



        var toolbar = null;
        var btnClose = null;
        var actionContainer = null;
        var ulAction = null;
        var btnArchive = null;
        var btnRequery = null;

        var btnCustomize = null;
        var btnPrint = null;
        var btnSaveCsv = null;
        var btnSavePdf = null;
        var btnRF = null;
        var overlay = $('<div>');
        var $menu = $("<ul class='vis-apanel-rb-ul'>");
        overlay.append($menu);
        var cPanel = null;
        var cFrame = null;

        var pCtl = null;
        var winNo = null;
        var paraList = null;
        var AD_Table_ID = null;
        var tableName = null;
        var canExport = false;
        var otherPf = [];
        var reportBytes = null;





        AD_Table_ID = _AD_Table_ID;
        paraList = _paraList;
        pCtl = _pCtl;
        winNo = _winNo;
        
        canExport = VIS.MRole.getDefault().getIsCanExport(AD_Table_ID);
        /* dispose Content */
        if (this.mPanel) {
            this.mPanel.dispose();
            this.mPanel = null;
        }
        this.disposeComponent();

        this.mPanel = panel;
        cFrame = this.parent;
        cPanel = panel;


        tableName = VIS.DB.executeScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + AD_Table_ID);
        $menu.empty();
        var sql = VIS.MRole.getDefault().addAccessSQL(
                "SELECT AD_PrintFormat_ID, Name, Description,IsDefault "
                    + "FROM AD_PrintFormat "
                    + "WHERE AD_Table_ID= " + AD_Table_ID
                    + " ORDER BY Name",
                "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);
        var dr = null;
        var checkName = [];
        var count = -1;
        otherPf = [];
        try {
            dr = VIS.DB.executeReader(sql, null, null);

            while (dr.read()) {
                count = parseInt(count) + 1;
                var name = dr.getString(1);
                if (checkName.indexOf(name) > -1) {
                    name = name + "_" + (parseInt(count) + 1);
                }
                var item = {};
                item.ID = VIS.Utility.Util.getValueOfInt(dr.get(0));
                item.Name = name;
                item.IsDefault = dr.getString(3);
                //var ulItem = $('<li><a data-isdefbtn="no" data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '">' + name + '</a><a data-isdefbtn="yes" data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '" style="min-height: 16px;display: inline-block;margin-left: 5px;min-width: 16px;" class="vis-mainnonfavitem" > </a></li>');

                ////$menu.append($('<li data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '">').append(name));
                //$menu.append(ulItem);
                checkName.push(dr.getString(1));
                otherPf.push(item);
                //pp = {};
                //pp.Key = VIS.Utility.Util.getValueOfInt(dr.get(0));
                //pp.Name = dr.getString(1);
                //list.push(pp);                    
            }
            dr.close();
        }
        catch (e) { dr.close(); }
        dr = null;
        $menu.append($('<li data-id="-1">').append(VIS.Msg.getMsg('NewReport')));


        panel.getRoot().height(this.parent.getContentGrid().height());
        //Create Custom Report ToolBar
        toolbar = $("<div class='vis-report-header'>").append($('<h3 class="vis-report-tittle" style="float:left;padding-top: 10px;">').append(VIS.Msg.getMsg("Report")));
        btnClose = $('<a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose" style="float:right">');
        actionContainer = $('<div class="vis-report-top-icons" style="float:right;">');
        ulAction = $('<ul style="margin-top: 10px;">');
        actionContainer.append(ulAction);
        btnRF = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-repformat-ico'></a></li>");
        ulAction.append(btnRF);
        if (canExport) {
            btnSaveCsv = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-savecsv-ico'></a></li>");
            ulAction.append(btnSaveCsv);
            btnSavePdf = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-savepdf-ico'></a></li>");
            ulAction.append(btnSavePdf);
        }
        btnArchive = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-archive-ico'></a></li>");
        ulAction.append(btnArchive);
        btnRequery = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-requery-ico'></a></li>");
        ulAction.append(btnRequery);
      
        btnCustomize = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-customize-ico'></a></li>");
        ulAction.append(btnCustomize);
        btnPrint = $('<li><a style="cursor:pointer;" class="vis-report-icon vis-print-ico"></a></li>');
        ulAction.append(btnPrint);

        toolbar.append(btnClose);
        toolbar.append(actionContainer);
        panel.getRoot().css('width', $(window).width());
        this.parent.getContentGrid().append(toolbar).append(panel.getRoot());
        try {
            var tables = document.getElementsByClassName('vis-reptabledetect');
            if (tables.length > 0) {
                var tableWidth = 0;
                var tmp = 0;
                for (var i = 0, j = tables.length; i < j; i++) {
                    tmp = $(tables[i]).width();
                    if (tmp > tableWidth) tableWidth = tmp;
                }
                panel.getRightDiv().width($(window).width() - 10);
                panel.getRightInnerDiv().width(tableWidth + 50);
            }
        }
        catch (e) { }

        var repHeaderEvents = function () {

            btnClose.off("click");
            btnRequery.off("click");
            btnCustomize.off("click");
            btnPrint.off('click');
            btnRF.off('click');
            $menu.off('click');
            btnArchive.off('click');
            btnClose.on('click', function () {
                if (cPanel) {
                    cPanel.dispose();
                    cPanel = null;
                }
                if (cFrame) {
                    cFrame.dispose();
                    cFrame = null;
                }
                ulAction.empty();
                toolbar.empty();
                toolbar.remove();

                btnClose.off("click");
                btnRequery.off("click");
                btnCustomize.off("click");
                btnPrint.off('click');
                btnRF.off('click');
                $menu.off('click');
                toolbar = null;
                btnClose = null;
                actionContainer = null;
                ulAction = null;
                btnArchive = null;
                btnRequery = null;
             
                btnCustomize = null;
                btnPrint = null;
                btnSaveCsv = null;
                btnSavePdf = null;
                repHeaderEvents = null;
            });
            btnRequery.on('click', function () {
                panel.setBusy(true);
                panel.getRightInnerDiv().html("");
                panel.getRightInnerDiv().width(0);
                var data = {
                    AD_Process_ID: pCtl.pi.getAD_Process_ID(),
                    AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
                    Name: pCtl.pi.getTitle(),
                    AD_Table_ID: pCtl.pi.getTable_ID(),
                    Record_ID: pCtl.pi.getRecord_ID(),
                    ParameterList: paraList,
                    csv: false,
                    pdf: false
                }

                VIS.dataContext.executeProcess(data, function (jsonStr) {
                    if (jsonStr.error != null) {
                        pCtl.pi.setSummary(jsonStr.error, true);
                        pCtl.unlock();
                        pCtl = null;
                        return;
                    }
                    var json = JSON.parse(jsonStr.result);
                    if (json.IsError) {
                        pCtl.pi.setSummary(json.Message, true);
                        pCtl.unlock();
                        pCtl = null;
                        return;
                    }
                    try {
                        panel.getRightInnerDiv().html(json.HTML);

                        var tables = document.getElementsByClassName('vis-reptabledetect');
                        if (tables.length > 0) {
                            var tableWidth = 0;
                            var tmp = 0;
                            for (var i = 0, j = tables.length; i < j; i++) {
                                tmp = $(tables[i]).width();
                                if (tmp > tableWidth) tableWidth = tmp;
                            }
                            panel.getRightDiv().width($(window).width() - 10);
                            panel.getRightDiv().css('min-width', $(window).width() + 'px');
                            panel.getRightInnerDiv().width(tableWidth + 50);
                        }
                        panel.getRightInnerDiv().width(tableWidth);
                    }
                    catch (e) { }
                    panel.setBusy(false);

                });

            });           
            btnCustomize.on('click', function () {
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_PrintFormat_ID", VIS.Query.prototype.EQUAL, AD_PrintFormat_ID);
                VIS.viewManager.startWindow(240, zoomQuery);
            });
            btnPrint.on('click', function () {
                if (cPanel.getIsHtmlReport()) {
                    var mywindow = window.open();
                    mywindow.document.write('<html><head>');
                    mywindow.document.write('</head><body >');
                    mywindow.document.write(cPanel.getHtml());
                    mywindow.document.write('</body></html>');
                    mywindow.print();
                    mywindow.close();
                }
            });
            if (canExport) {
                btnSaveCsv.on('click', function () {
                    panel.setBusy(true);
                    var data = {
                        AD_Process_ID: pCtl.pi.getAD_Process_ID(),
                        AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
                        Name: pCtl.pi.getTitle(),
                        AD_Table_ID: pCtl.pi.getTable_ID(),
                        Record_ID: pCtl.pi.getRecord_ID(),
                        ParameterList: paraList,
                        csv: true,
                        pdf: false
                    }
                    VIS.dataContext.executeProcess(data, function (jsonStr) {
                        if (jsonStr.error != null) {
                            pCtl.pi.setSummary(jsonStr.error, true);
                            pCtl.unlock();
                            pCtl = null;
                            return;
                        }
                        var json = JSON.parse(jsonStr.result);
                        if (json.IsError) {
                            pCtl.pi.setSummary(json.Message, true);
                            pCtl.unlock();
                            pCtl = null;
                            return;
                        }
                        window.open(VIS.Application.contextUrl + json.ReportFilePath);

                        panel.setBusy(false);
                    });
                });
                btnSavePdf.on('click', function () {
                    panel.setBusy(true);
                    var data = {
                        AD_Process_ID: pCtl.pi.getAD_Process_ID(),
                        AD_PInstance_ID: pCtl.pi.getAD_PInstance_ID(),
                        Name: pCtl.pi.getTitle(),
                        AD_Table_ID: pCtl.pi.getTable_ID(),
                        Record_ID: pCtl.pi.getRecord_ID(),
                        ParameterList: paraList,
                        csv: false,
                        pdf: true
                    }
                    VIS.dataContext.executeProcess(data, function (jsonStr) {
                        if (jsonStr.error != null) {
                            pCtl.pi.setSummary(jsonStr.error, true);
                            pCtl.unlock();
                            pCtl = null;
                            return;
                        }
                        var json = JSON.parse(jsonStr.result);
                        if (json.IsError) {
                            pCtl.pi.setSummary(json.Message, true);
                            pCtl.unlock();
                            pCtl = null;
                            return;
                        }
                        window.open(VIS.Application.contextUrl + json.ReportFilePath);

                        panel.setBusy(false);
                    });
                });
            }
            btnRF.on('click', function () {
                $menu.empty();
                for (var i = 0; i < otherPf.length; i++) {
                    var className = otherPf[i].IsDefault == "Y" ? "vis-mainfavitem" : "vis-mainnonfavitem";
                    var ulItem = $('<li><a data-isdefbtn="no" data-id="' + otherPf[i].ID + '">' + otherPf[i].Name + '</a></li>');
                    $menu.append(ulItem);
                }
                $menu.append($('<li><a data-id="-1">' + VIS.Msg.getMsg('NewReport') + '</a>'));
                $(this).w2overlay(overlay.clone(true), { css: { height: '300px' } });
            });
            $menu.on('click', "LI", function (e) {
                debugger;
                panel.setBusy(true);

                var btn = $(e.target);
                var id = btn.data("id");
                if (btn.data("isdefbtn") == "yes") {
                    var sql = "UPDATE AD_PrintFormat SET IsDefault='N' WHERE IsDefault='Y' AND AD_Table_ID=" + Ad_Table_ID + " AND AD_Tab_ID=" + curTab.getAD_Tab_ID();
                    VIS.DB.executeQuery(sql);
                    var sql = "UPDATE AD_PrintFormat SET IsDefault='Y' WHERE AD_PrintFormat_ID=" + id;
                    VIS.DB.executeQuery(sql);
                    for (var i = 0; i < otherPf.length; i++) {
                        if (otherPf[i].ID == id) {
                            otherPf[i].IsDefault = "Y";
                        }
                        else {
                            otherPf[i].IsDefault = "N";
                        }
                    }
                    return;
                }
                AD_PrintFormat_ID=id;
                var isCreateNew = false;
                if (id == -1) {
                    id = AD_Table_ID;
                    isCreateNew = true;
                }
                var queryInfo = [];
                var query = new VIS.Query(tableName);
                queryInfo.push(query.getTableName());
                queryInfo.push(query.getWhereClause(true));
                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                    dataType: "json",
                    data: {
                        id: id,
                        queryInfo: JSON.stringify(queryInfo),
                        code: query.getCode(0),
                        isCreateNew: isCreateNew,
                        nProcessInfo: JSON.stringify(""),
                        pdf: false,
                        csv: false
                    },
                    success: function (jsonStr) {

                        var json = jQuery.parseJSON(jsonStr);
                        if (json.IsError) {
                            pCtl.pi.setSummary(json.Message, true);
                            pCtl.unlock();
                            pCtl = null;
                            return;
                        }
                        try {
                            panel.getRightInnerDiv().html(json.HTML);

                            var tables = document.getElementsByClassName('vis-reptabledetect');
                            if (tables.length > 0) {
                                var tableWidth = 0;
                                var tmp = 0;
                                for (var i = 0, j = tables.length; i < j; i++) {
                                    tmp = $(tables[i]).width();
                                    if (tmp > tableWidth) tableWidth = tmp;
                                }
                                panel.getRightDiv().width($(window).width() - 10);
                                panel.getRightInnerDiv().width(tableWidth + 50);
                            }
                            panel.getRightInnerDiv().width(tableWidth);
                        }
                        catch (e) { }
                        panel.setBusy(false);

                    }
                });

            });
            btnArchive.on('click', function () {
                panel.setBusy(true);
                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/ArchiveDoc/",
                    dataType: "json",
                    type: "post",
                    data: {
                        AD_Process_ID: pCtl.pi.getAD_Process_ID(),
                        Name: pCtl.pi.getTitle(),
                        AD_Table_ID: pCtl.pi.getTable_ID(),
                        Record_ID: pCtl.pi.getRecord_ID(),
                        C_BPartner_ID: 0,
                        isReport: true,
                        binaryData: reportBytes
                    },
                    success: function (data) {
                        VIS.ADialog.info('Archived', true, "", "");
                        panel.setBusy(false);
                    }
                });
            });
           
        };
        repHeaderEvents();
        panel.setBusy(false);
        this.parent.hideHeader(true);
        
    };
    AProcess.prototype.setReportBytes = function (bytes) {
        reportBytes = bytes;
    };
    AProcess.prototype.sizeChanged = function (height, width) {
        this.setSize(height, width);
        if (this.mPanel && this.mPanel.sizeChanged) {
            this.mPanel.sizeChanged(height, width);
        }
    };

    AProcess.prototype.refresh = function () {
        if (this.mPanel && this.mPanel.refresh) {
            this.mPanel.refresh();
        }
    };

    //Assignment
    VIS.AProcess = AProcess;

})(VIS, jQuery);