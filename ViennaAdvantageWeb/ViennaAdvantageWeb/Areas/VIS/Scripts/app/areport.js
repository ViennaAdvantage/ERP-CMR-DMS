/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Report
 * Author         :     Lakhwinder
 * Date           :     2-Nov-2014
  ******************************************************/
; (function (VIS, $) {

    var $root = $('<div style="width:100%;height:100%">');//, {
    var $menu = $("<ul class='vis-apanel-rb-ul'  style='width:100%;height:100%'>");
    $root.append($menu);
    var liCSV = $("<li>").append(VIS.Msg.getMsg('OpenCSV'));
    $menu.append(liCSV);
    var liPDF = $("<li>").append(VIS.Msg.getMsg('OpenPDF'));
    $menu.append(liPDF);

    function AReport(AD_Table_ID, query, AD_Tab_ID, windowNo, curTab) {


        var AD_Client_ID = null;
        var sql = null;
        var dr = null;
        var list = null;


        var rv = new VIS.ReportViewer(windowNo, curTab);

        var getPrintFormats = function (AD_Table_ID) {
            AD_Client_ID = VIS.context.getAD_Client_ID();
            sql = "SELECT AD_PrintFormat_ID, Name, AD_Client_ID "
                   + "FROM AD_PrintFormat "
                   + "WHERE AD_Table_ID='" + AD_Table_ID + "' AND IsTableBased='Y' ";
            if (AD_Tab_ID > 0) {
                sql = sql + " AND AD_Tab_ID='" + AD_Tab_ID + "' ";
            }
            sql = sql + "ORDER BY AD_Client_ID DESC, IsDefault DESC, Name";	//	Own First
            sql = VIS.MRole.getDefault().addAccessSQL(sql,		//	Own First
                   "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);



            //sql = VIS.MRole.getDefault().addAccessSQL(
            //    "SELECT AD_PrintFormat_ID, Name, AD_Client_ID "
            //        + "FROM AD_PrintFormat "
            //        + "WHERE AD_Table_ID='" + AD_Table_ID + "' AND IsTableBased='Y' "
            //        + "ORDER BY AD_Client_ID DESC, IsDefault DESC, Name",		//	Own First
            //    "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);
            dr = null;
            var pp = null;
            var list = [];
            try {
                dr = VIS.DB.executeReader(sql, null, null);
                while (dr.read()) {
                    if (VIS.Utility.Util.getValueOfInt(dr.get(2).toString()) == AD_Client_ID) {
                        pp = {};
                        pp.Key = VIS.Utility.Util.getValueOfInt(dr.get(0));
                        pp.Name = dr.getString(1);
                        list.push(pp);
                    }
                }
                dr.close();

                if (list.length == 0) {
                    if (pp == null) launchReport(null, AD_Table_ID, AD_Table_ID);		//	calls launch
                    else launchReport(pp, 0, AD_Table_ID);
                }
                else launchReport(list[0], 0, AD_Table_ID);
            }
            catch (e) { dr.close(); }
        };



        var launchReport = function (pp, Ad_Table_ID, TableID) {
            var queryInfo = [];
            queryInfo.push(query.getTableName());
            queryInfo.push(query.getWhereClause(true));
            if (AD_Tab_ID) {
                queryInfo.push(AD_Tab_ID.toString());
            }
            var id = null;
            if (pp != null) id = pp.Key;
            else id = Ad_Table_ID;

            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: {
                    id: id,
                    queryInfo: JSON.stringify(queryInfo),
                    code: query.getCode(0),
                    isCreateNew: (Ad_Table_ID > 0 && AD_Tab_ID > 0),
                    nProcessInfo: JSON.stringify(""),
                    pdf: false,
                    csv: false
                },
                success: function (data) {
                    if (data == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                    }
                    var d = jQuery.parseJSON(data);
                    if (d.IsError) {
                        VIS.ADialog.error(d.ErrorText);
                        rv.close();
                        return;
                    }
                    if (d.HTML && d.HTML.length > 0) { //report

                        //rv.show(d.HTML, id, pp, TableID, queryInfo, query.getCode(0));
                        rv.show(d.HTML, d.AD_PrintFormat_ID, pp, TableID, queryInfo, query.getCode(0));
                        rv.setReportBytes(d.Report);
                    }
                }
            });
        };
        getPrintFormats(AD_Table_ID);
    };


    function ReportViewer(windowNo, curTab) {
        var $root = $("<div style='height:100%;width:100%;'>");
        var toolbar = null;
        var btnClose = null;
        var actionContainer = null;
        var ulAction = null;
        var btnArchive = null;
        var btnRequery = null;
        var btnSearch = null;
        var btnCustomize = null;
        var btnPrint = null;
        var btnSavePdf = null;
        var btnSaveCsv = null
        var cFrame = null;
        var html = null;
        var AD_PrintFormat_ID = null;
        var processInfo = null;
        var Ad_Table_ID = null;
        var queryInfo = null;
        var code = null;
        var height = VIS.Env.getScreenHeight() - 50;
        var width = $(window).width();
        var self = this;
        cFrame = new VIS.CFrame();
        cFrame.setName(VIS.Msg.getMsg("Report"));
        cFrame.setTitle(VIS.Msg.getMsg("Report"));
        var btnRF = null;
        var overlay = $('<div>');
        var $menu = $("<ul class='vis-apanel-rb-ul'>");
        overlay.append($menu);
        var canExport = false;
        var otherPf = [];
        var reportBytes = null;

        this.setReportBytes = function (bytes) {
            reportBytes = bytes;
        };

        this.getRoot = function () {
            return $root;
        };
        this.dispose = function () {
        };
        this.show = function (content, _AD_PrintFormat_ID, _processInfo, _Ad_Table_ID, _queryInfo, _code) {

            processInfo = _processInfo;
            Ad_Table_ID = _Ad_Table_ID;
            queryInfo = _queryInfo;
            AD_PrintFormat_ID = _AD_PrintFormat_ID;
            code = _code;
            canExport = VIS.MRole.getDefault().getIsCanExport(Ad_Table_ID);
            if (canExport) {
                btnSaveCsv.css('display', 'block');
                btnSavePdf.css('display', 'block');
            }
            else {
                btnSaveCsv.css('display', 'none');
                btnSavePdf.css('display', 'none');

            }
            showReport(content);
        }

        var disposeComponant = function () {
            $root.empty();
            $root.remove();
            ulAction.empty();
            toolbar.empty();
            toolbar.remove();
            if (cFrame) {
                cFrame.dispose();
                cFrame = null;
            }
            $root = null;
            toolbar = null;
            btnClose = null;
            actionContainer = null;
            ulAction = null;
            btnArchive = null;
            btnRequery = null;
            btnSearch = null;
            btnCustomize = null;
            btnPrint = null;
        };
        this.close = function () {
            disposeComponant();
        };
        var createHeader = function () {


            if (VIS.Application.isRTL) {
                toolbar = $("<div class='vis-report-header' style='padding-right: 10px;padding-left:0px;'>").append($('<h3 class="vis-report-tittle" style="float:right;padding-top: 10px;">').append(VIS.Msg.getMsg("Report")));
                btnClose = $('<a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose" style="float:left">');
                actionContainer = $('<div class="vis-report-top-icons" style="float:left;">');
                ulAction = $('<ul style="margin-top: 10px;">');
                actionContainer.append(ulAction);

                btnPrint = $('<li><a style="cursor:pointer;" class="vis-report-icon vis-print-ico"></a></li>');
                ulAction.append(btnPrint);
                btnCustomize = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-customize-ico'></a></li>");
                ulAction.append(btnCustomize);
                btnSearch = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-search-ico'></a></li>");
                ulAction.append(btnSearch);
                btnRequery = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-requery-ico'></a></li>");
                ulAction.append(btnRequery);
                btnArchive = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-archive-ico'></a></li>");
                ulAction.append(btnArchive);
                btnSavePdf = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-savepdf-ico'></a></li>");
                ulAction.append(btnSavePdf);
                btnSaveCsv = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-savecsv-ico'></a></li>");
                ulAction.append(btnSaveCsv);

                btnRF = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-repformat-ico'></a></li>");
                ulAction.append(btnRF);
              
               
               
               
               
               
                toolbar.append(btnClose);
                toolbar.append(actionContainer);
                $root.append(toolbar);
            }
            else {
                toolbar = $("<div class='vis-report-header' style='padding-left: 10px;padding-right: 0px;'>").append($('<h3 class="vis-report-tittle" style="float:left;padding-top: 10px;">').append(VIS.Msg.getMsg("Report")));
                btnClose = $('<a href="javascript:void(0)" class="vis-mainMenuIcons vis-icon-menuclose" style="float:right">');
                actionContainer = $('<div class="vis-report-top-icons" style="float:right;">');
                ulAction = $('<ul style="margin-top: 10px;">');
                actionContainer.append(ulAction);
                btnRF = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-repformat-ico'></a></li>");
                ulAction.append(btnRF);
                btnSaveCsv = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-savecsv-ico'></a></li>");
                ulAction.append(btnSaveCsv);
                btnSavePdf = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-savepdf-ico'></a></li>");
                ulAction.append(btnSavePdf);
                btnArchive = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-archive-ico'></a></li>");
                ulAction.append(btnArchive);
                btnRequery = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-requery-ico'></a></li>");
                ulAction.append(btnRequery);
                btnSearch = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-search-ico'></a></li>");
                ulAction.append(btnSearch);
                btnCustomize = $("<li><a style='cursor:pointer;' class='vis-report-icon vis-customize-ico'></a></li>");
                ulAction.append(btnCustomize);
                btnPrint = $('<li><a style="cursor:pointer;" class="vis-report-icon vis-print-ico"></a></li>');
                ulAction.append(btnPrint);
                toolbar.append(btnClose);
                toolbar.append(actionContainer);
                $root.append(toolbar);
            }
            bindEvents();
        };

        var bindEvents = function () {
            btnClose.on('click', function () {
                disposeComponant();
            });
            btnPrint.on('click', function () {
                printReport();
            });
            btnCustomize.on('click', function () {
                //var AD_Window_ID = 240;		// hardcoded               
                var zoomQuery = new VIS.Query();
                zoomQuery.addRestriction("AD_PrintFormat_ID", VIS.Query.prototype.EQUAL, AD_PrintFormat_ID);
                VIS.viewManager.startWindow(240, zoomQuery);
            });
            btnRequery.on('click', function () {
                subContentPane.empty();
                subContentPane.css('width', '0px');
                launchReport();
            });

            btnSavePdf.on('click', function () {
                getPdf();
            });
            btnSaveCsv.on('click', function () {
                getCsv();
            });

            btnRF.on('click', function () {
                $menu.empty();
                for (var i = 0; i < otherPf.length; i++) {
                    var className = otherPf[i].IsDefault == "Y" ? "vis-favitmchecked" : "vis-favitmunchecked";
                    var ulItem = $('<li><a data-isdefbtn="no" data-id="' + otherPf[i].ID + '">' + otherPf[i].Name + '</a><a data-isdefbtn="yes" data-id="' + otherPf[i].ID + '" style="min-height: 16px;display: inline-block;margin-left: 5px;min-width: 16px;margin-top: 0px" class="vis-menufavitm ' + className + '" > </a></li>');
                    $menu.append(ulItem);
                }
                $menu.append($('<li><a data-id="-1">' + VIS.Msg.getMsg('NewReport') + '</a>'));
                $(this).w2overlay(overlay.clone(true), { css: { height: '300px' } });
            });
            $menu.on('click', "LI", function (e) {

                var btn = $(e.target);
                var id = btn.data("id");
                if (btn.data("isdefbtn") == "yes") {
                    var sql = "UPDATE AD_PrintFormat SET IsDefault='N' WHERE IsDefault='Y' AND AD_Table_ID=" + Ad_Table_ID + " AND AD_Tab_ID=" + curTab.getAD_Tab_ID();
                    VIS.DB.executeQuery(sql);
                    var sql = "UPDATE AD_PrintFormat SET IsDefault='Y' WHERE AD_PrintFormat_ID="+id;
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
                AD_PrintFormat_ID = id;
                subContentPane.empty();
                subContentPane.css('width', '0px');
                setBusy(true);
                var isCreateNew = false;
                if (id == -1) {
                    id = Ad_Table_ID;
                    isCreateNew = true;
                }

                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                    dataType: "json",
                    data: {
                        id: id,
                        queryInfo: JSON.stringify(queryInfo),
                        code: code,
                        isCreateNew: isCreateNew,
                        nProcessInfo: JSON.stringify(""),
                        pdf: false,
                        csv: false
                    },
                    success: function (data) {
                        if (data == null) {
                            alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                        }
                        var d = jQuery.parseJSON(data);

                        if (d.HTML && d.HTML.length > 0) { //report
                            showReport(d.HTML);

                        }
                        AD_PrintFormat_ID = d.AD_PrintFormat_ID;
                        setBusy(false);
                    }
                });
            });
            btnSearch.on('click', function () {
                var find = new VIS.Find(windowNo, curTab, 0);
                find.onClose = function () {
                    if (find.getIsOKPressed()) {
                        var query = find.getQuery();
                        //	History
                        var onlyCurrentDays = find.getCurrentDays();
                        var created = find.getIsCreated();
                        find.dispose();
                        find = null;
                        queryInfo[1] = (query.getWhereClause(true));
                        launchReport();
                    }
                };
                find.show();
            });
            btnArchive.on('click', function () {
                setBusy(true);
                $.ajax({
                    url: VIS.Application.contextUrl + "JsonData/ArchiveDoc/",
                    dataType: "json",
                    type: "post",
                    data: {
                        AD_Process_ID:0,
                        Name:queryInfo[0],
                        AD_Table_ID:Ad_Table_ID,
                        Record_ID:0,
                        C_BPartner_ID:0,
                        isReport:true,
                        binaryData:reportBytes
                    },
                    success: function (data) {
                        VIS.ADialog.info('Archived', true, "", "");
                        setBusy(false);
                    }
                });
            });
        };

        function printReport() {
            var mywindow = window.open();
            mywindow.document.write('<html dir="RTL"><head>');
            mywindow.document.write('</head><body >');
            mywindow.document.write(html);
            mywindow.document.write('</body></html>');
            mywindow.print();
            mywindow.close();
        };

        var launchReport = function () {

            var id = null;
            if (AD_PrintFormat_ID > 0) {
                id = AD_PrintFormat_ID;
            }
            else if (processInfo != null) {
                id = processInfo.Key;
            }
            else {
                id = Ad_Table_ID;
            }
            setBusy(true);
            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: {
                    id: id,
                    queryInfo: JSON.stringify(queryInfo),
                    code: code,
                    isCreateNew: false,
                    nProcessInfo: JSON.stringify(""),
                    pdf: false,
                    csv: false
                },
                success: function (data) {
                    if (data == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                    }
                    var d = jQuery.parseJSON(data);

                    if (d.HTML && d.HTML.length > 0) { //report
                        showReport(d.HTML);

                    }
                    setBusy(false);
                }
            });
        };

        var getPdf = function () {
            setBusy(true);
            var id = null;
            if (processInfo != null) id = processInfo.Key;
            else id = Ad_Table_ID;

            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: {
                    id: id,
                    queryInfo: JSON.stringify(queryInfo),
                    code: code,
                    isCreateNew: false,
                    nProcessInfo: JSON.stringify(""),
                    pdf: true,
                    csv: false
                },
                success: function (data) {
                    if (data == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                    }
                    var d = jQuery.parseJSON(data);

                    if (d.HTML && d.HTML.length > 0) { //report
                        showReport(d.HTML);
                    }
                    else {
                        window.open(VIS.Application.contextUrl + d.ReportFilePath);
                    }
                    setBusy(false);
                }
            });
        };


        var getCsv = function () {
            setBusy(true);
            var id = null;
            if (processInfo != null) id = processInfo.Key;
            else id = Ad_Table_ID;

            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GenerateReport/",
                dataType: "json",
                data: {
                    id: id,
                    queryInfo: JSON.stringify(queryInfo),
                    code: code,
                    isCreateNew: false,
                    nProcessInfo: JSON.stringify(""),
                    pdf: false,
                    csv: true
                },
                success: function (data) {
                    if (data == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                    }
                    var d = jQuery.parseJSON(data);

                    if (d.HTML && d.HTML.length > 0) { //report
                        showReport(d.HTML);
                    }
                    else {
                        window.open(VIS.Application.contextUrl + d.ReportFilePath);
                    }
                    setBusy(false);
                }
            });
        };

        var showReport = function (content) {
            $menu.empty();
            var sql ="SELECT AD_PrintFormat_ID, Name, Description,IsDefault "
                        + "FROM AD_PrintFormat "
                        + "WHERE AD_Table_ID="+ Ad_Table_ID;
            if (curTab.getAD_Tab_ID() > 0) {
                sql = sql + " AND AD_Tab_ID=" + curTab.getAD_Tab_ID();
            }
            sql = sql + " ORDER BY Name";
            sql = VIS.MRole.getDefault().addAccessSQL(sql, "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);

                //VIS.MRole.getDefault().addAccessSQL(
                //    "SELECT AD_PrintFormat_ID, Name, Description "
                //        + "FROM AD_PrintFormat "
                //        + "WHERE AD_Table_ID= " + Ad_Table_ID
                //        + " ORDER BY Name",
                //    "AD_PrintFormat", VIS.MRole.SQL_NOTQUALIFIED, VIS.MRole.SQL_RO);

            var dr = null;
            var checkName = [];
            var count = -1;
            try {
                dr = VIS.DB.executeReader(sql, null, null);
                otherPf = [];
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
                    //// $menu.append($('<li data-id="' + VIS.Utility.Util.getValueOfInt(dr.get(0)) + '">').append(name));
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
           
            subContentPane.html(content);
            subContentPane.css('min-width', width + 'px');
            html = content;
            var tables = document.getElementsByClassName('vis-reptabledetect');
            var tableWidth = 0;
            var tmp = 0;
            for (var i = 0, j = tables.length; i < j; i++) {
                tmp = $(tables[i]).width();
                if (tmp > tableWidth) tableWidth = tmp;
            }
            subContentPane.css('width', tableWidth+50);
            setBusy(false);
        };
        var setBusy = function (isBusy) {
            bsyDiv.css("display", isBusy ? 'block' : 'none');
        };
        createHeader();
        var contentPane = $("<div Style='height:" + height + "px;width:" + width + "px;overflow:auto;background:#63BFE9;float:left;'>");
        var subContentPane = $("<div Style='height:" + height + "px;width:" + width + "px;'>");
        contentPane.append(subContentPane);
        var bsyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");
        setBusy(true);
        contentPane.append(bsyDiv);
        $root.append(contentPane);
        cFrame.hideHeader(true);
        cFrame.setContent(self);
        cFrame.show();
    };

    function APrint(AD_Process_ID, table_ID, record_ID, WindowNo) {

        // var rv = new VIS.ReportViewer();

        this.start = function ($btnInfo) {
            liCSV.off('click');
            liPDF.off('click');
            liCSV.on('click', function () {
                process(true);
            });
            liPDF.on('click', function () {
                process(false);
            });
            $btnInfo.w2overlay($root.clone(true), { css: { height: '300px' } });
        };

        function process(csv) {

            $.ajax({
                url: VIS.Application.contextUrl + "JsonData/GeneratePrint/",
                dataType: "json",
                data: {
                    AD_Process_ID: AD_Process_ID,
                    Name: 'Print',
                    AD_Table_ID: table_ID,
                    Record_ID: record_ID,
                    WindowNo: WindowNo,
                    csv: csv

                },
                success: function (data) {
                    if (data == null) {
                        alert(VIS.Msg.getMsg('ERRORGettingRepData'));
                    }
                    var d = jQuery.parseJSON(data);
                    if (d.IsError) {
                        VIS.ADialog.error(d.ErrorText);
                        // rv.close();
                        return;
                    }

                    if (d.ReportFilePath) {
                        window.open(VIS.Application.contextUrl + d.ReportFilePath);
                    }
                }
            });
        };
    };

    VIS.ReportViewer = ReportViewer;
    VIS.AReport = AReport;
    VIS.APrint = APrint;
})(VIS, jQuery);