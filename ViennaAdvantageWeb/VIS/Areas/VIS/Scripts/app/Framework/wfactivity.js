/********************************************************
 * Module Name    :     Application
 * Purpose        :     Show Suspended Workflow And Process further
 * Author         :     Lakhwinder
 * Date           :     20-Oct-2014
  ******************************************************/
; (function (VIS, $) {
    function wfActivity(container, divDetail, workflowActivityData, welcomeScreenFeedsListScroll) {
        var data = null;
        var fulldata = [];
        var dataItemDivs = [];


        var divOuter = null;


        this.Load = function (refresh) {
            //  $("#divfeedbsy")[0].style.visibility = "visible";
            $("#divTabMenuDataLoder").show();

            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetActivities",
                data: { pageNo: 1, pageSize: 10, refresh: refresh },
                dataType: "json",
                type: "POST",

                error: function () {
                    //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    //bsyDiv[0].style.visibility = "hidden";
                    //   $("#divfeedbsy")[0].style.visibility = "hidden";
                    $("#divTabMenuDataLoder").hide();
                    VIS.HomeMgr.BindMenuClick();
                    return;
                },
                success: function (dyndata) {
                    VIS.HomeMgr.BindMenuClick();
                    var active = VIS.HomeMgr.getActiveTab();
                    if (active.activeTabType == 3) {
                        if (dyndata.result == null || dyndata.result.LstInfo == null) {
                            $("#divTabMenuDataLoder").hide();
                            $("#divfActivity").empty();
                            $("#divfActivity").append(0);


                            //$("#sAlrtTxtType").empty();
                            //$("#sAlrtTxtType").append(VIS.Msg.getMsg('WorkflowActivities'));
                            $("#hAlrtTxtTypeCount").empty();
                            $("#hAlrtTxtTypeCount").append(0);
                            str = "<p style=' margin-top:150px;text-align: center'>" + VIS.Msg.getMsg('NoRecordFound') + "!!!</p>";
                            container.append(str);
                            return;
                        }
                        data = dyndata.result.LstInfo;
                        container.empty();

                        if (refresh == true) {
                            //Reset Count Header
                            dataItemDivs = [];
                            fulldata = [];
                            $("#divfActivity").empty();
                            $("#divfActivity").append(dyndata.result.count);


                            //$("#sAlrtTxtType").empty();
                            //$("#sAlrtTxtType").append(VIS.Msg.getMsg('WorkflowActivities'));
                            $("#hAlrtTxtTypeCount").empty();
                            $("#hAlrtTxtTypeCount").append(dyndata.result.count);

                        }

                        LoadRecords(0);
                        VIS.HomeMgr.adjustDivSize();
                    }
                    else {
                        return;
                    }
                    // workflowActivityData.refresh();
                }
            });
            // LoadRecords();


        };


        this.AppendRecord = function (pageNo, paeSize) {

            $("#divTabMenuDataLoder").show();
            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetActivities",
                data: { pageNo: pageNo, pageSize: paeSize, refresh: false },
                dataType: "json",
                type: "POST",
                error: function () {
                    //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    //bsyDiv[0].style.visibility = "hidden";
                    $("#divTabMenuDataLoder").hide();
                    VIS.HomeMgr.BindMenuClick();
                    return;
                },
                success: function (dyndata) {
                    VIS.HomeMgr.BindMenuClick();
                    var active = VIS.HomeMgr.getActiveTab();
                    if (active.activeTabType == 3) {
                        data = dyndata.result.LstInfo;

                        LoadRecords(pageNo - 1);
                        VIS.HomeMgr.adjustDivSize();
                    }
                    else {
                        return;
                    }
                    //workflowActivityData.refresh();
                }
            });
        }

        var LoadRecords = function (pageNumber) {


            //data = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/GetActivities", {})).result;
            if (data == null || data.length == 0) {
                //bsyDiv[0].style.visibility = "hidden";

                $("#divTabMenuDataLoder").hide();


                return;
            }



            for (var item in data) {
                fulldata.push(data[item]);
                var dataIem = {};
                divOuter = $("<div class='vis-activityContainer' data-id='" + (Number(10 * pageNumber) + Number(item)) + "'>");
                divOuter.on('click', function (e) {

                    //$('div.activityContainer div.feedDetails').click(function () {
                    //    $('#workflowActivity').show();
                    //    //workflowActivityData.refresh();
                    //     adjust_size();
                    //});
                    $('.vis-activityContainer').removeClass('vis-activeFeed');
                    $(this).addClass('vis-activeFeed');
                    var datatab = $(e.target).data("viswfazoom");
                    if (datatab == 'wfZoom') {
                        $(this).addClass('vis-activeFeed');
                        var id = $(e.target).data("index");
                        //alert(dataItemDivs[id].recordID);
                        //getDetail(dataItemDivs[id].recordID, id);
                        zoom(id);
                        return;
                    }

                    //workflowActivityData.refresh();
                    var id = $(this).data("id");
                    //alert(dataItemDivs[id].recordID);
                    // var selectedEffect = "transfer";
                    //  var options = {};
                    //  options = { to: divDetail, className: "wsp-ui-effects-transfer" };
                    //  $(this).effect(selectedEffect, options, 500, function () {
                    getDetail(dataItemDivs[id].wfActivityID, id, this);
                    // });

                });
                var divActions = $("<div class='vis-feedTitleBar'>");

                var header = $("<h3>");
                header.css('font-weight', 'normal');
                header.css('color', '#1b95d7');
                header.append(VIS.Utility.encodeText(data[item].NodeName));
                divActions.append(header);

                var divBtns = $("<div class='vis-feedTitleBar-buttons'>");
                var ul = $("<ul>");
                var liZoom = $("<li>");
                var aZoom = $("<a href='javascript:void(0)' class='vis-feedIcons vis-icon-viewFeed' data-index='" + (Number(10 * pageNumber) + Number(item)) + "' data-viswfazoom='wfZoom' >");
                aZoom.append("View Feed");

                //aZoom.on(VIS.Events.onTouchStartOrClick, function () {

                //});
                liZoom.append(aZoom);
                ul.append(liZoom);

                //var liOk = $("<li>");
                //var aOk = $("<a href='#' class='vis-feedIcons vis-icon-check'>");
                //aOk.append("feed check");
                //liOk.append(aOk);
                //liOk.on("click", function () {
                //    alert("clicked");
                //});
                //ul.append(liOk);

                //var liDelete = $("<li>");
                //var aDelete = $("<a href='#' class='vis-feedIcons vis-icon-delete'>");
                //aDelete.append("feed check");
                //liDelete.append(aDelete);
                //ul.append(liDelete);

                divBtns.append(ul);

                divActions.append(divBtns);

                divOuter.append(divActions);

                var divContent = $("<div class='vis-feedDetails'>");

                var para = $("<p>");

                //var str = $("<strong>");
                //str.append(data[item].NodeName)
                //para.append(str);

                //var br = $("<br>");
                //para.append(br);

                para.append(VIS.Utility.encodeText(data[item].Summary));
                var br = $("<br>");
                para.append(br);
                para.append(VIS.Utility.encodeText(VIS.Msg.getMsg('Priority') + " " + data[item].Priority));

                divContent.append(para);

                var pdate = $("<div class='vis-feedDateTime'>");
                //var divPriority = $("<div>");
                //divPriority.css('float', 'left');
                // divPriority.append(VIS.Msg.getMsg('Priority') + " "+data[item].Priority);
                pdate.append($("<br>")).append(Globalize.format(new Date(data[item].Created), 'd'));
                divContent.append(pdate);

                divOuter.append(divContent);


                container.append(divOuter);
                //divScroll.append(divOuter);
                dataIem.ctrl = divOuter;
                dataIem.index = item;

                dataIem.recordID = data[item].Record_ID;
                dataIem.wfActivityID = data[item].AD_WF_Activity_ID;

                dataItemDivs.push(dataIem);


            }

            // welcomeScreenFeedsListScroll.refresh();
            // $("#divfeedbsy")[0].style.visibility = "hidden";
            $("#divTabMenuDataLoder").hide();


        };


        var adjust_size = function () {
            //alert( "adjust_size called." );
            var windowWidth = $(window).width();
            var divCount = $(".row.scrollerHorizontal > div").length
            divCount = divCount - 1;
            $('.scrollerHorizontal').width(windowWidth);
            var sectionsWidth = windowWidth / divCount - 20;
            var sectionsWidthFinal = parseInt(sectionsWidth);

            /* latop and large display screen size */
            if (windowWidth >= 1366) {
                if ($('#workflowActivity').css('display') == 'none') {
                    //alert(divCount);
                    $("#fllupsScreen").css("left", (sectionsWidthFinal + 20));
                    $("#favLinks").css("left", (sectionsWidthFinal * 2 + 40));
                    $('#welcomeScreen,#fllupsScreen,#favLinks').width(sectionsWidthFinal);
                }
                else if ($('#workflowActivity').css('display') == 'block') {
                    var newWidth = windowWidth + sectionsWidthFinal + 20;
                    $('.scrollerHorizontal').width(newWidth);
                    // horizontalScroll.refresh();
                    //alert(newWidth);
                    $("#workflowActivity").css("left", (sectionsWidthFinal + 20));
                    $("#fllupsScreen").css("left", (sectionsWidthFinal * 2 + 40));
                    $("#favLinks").css("left", (sectionsWidthFinal * 3 + 60));
                    $('#welcomeScreen,#fllupsScreen,#favLinks,#workflowActivity').width(sectionsWidthFinal);
                }
            }
        };


        var lstDetailCtrls = [];


        var getDetail = function (wfActivityID, index, ctrl) {

            //$("#divfeedbsy")[0].style.visibility = "visible";
            $("#divTabMenuDataLoder").show();
            $.ajax({
                url: VIS.Application.contextUrl + "WFActivity/GetActivityInfo",
                dataType: "json",
                type: "POST",
                data: {
                    activityID: wfActivityID,
                    nodeID: fulldata[index].AD_Node_ID,
                    wfProcessID: fulldata[index].AD_WF_Process_ID
                },
                error: function () {
                    //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                    //bsyDiv[0].style.visibility = "hidden";
                    // $("#divfeedbsy")[0].style.visibility = "hidden";
                    $("#divTabMenuDataLoder").hide();
                    return;
                },
                success: function (res) {
                    $('#workflowActivity').show();
                    VIS.HomeMgr.adjustDivSize();
                    var selectedEffect = "transfer";
                    var options = {};
                    options = { to: divDetail, className: "wsp-ui-effects-transfer" };

                    window.setTimeout(function () {
                        loadDetail(wfActivityID, index, res.result);
                        $(ctrl).effect(selectedEffect, options, 500, function () {



                        });
                    }, 400);
                    //  workflowActivityData.refresh();
                }
            });

        };

        var loadDetail = function (wfActivityID, index, info) {


            var detailCtrl = {};
            lstDetailCtrls = [];
            detailCtrl.Index = index;

            // var info = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/GetActivityInfo", { "activityID": wfActivityID, "nodeID": data[index].AD_Node_ID, "wfProcessID": data[index].AD_WF_Process_ID })).result;


            divDetail.empty();

            var divHeader = $("<div class='vis-workflowActivityDetails-Heading'>");
            divDetail.append(divHeader);

            var hHeader = $("<h3>");
            hHeader.append(VIS.Msg.getMsg('Detail'));
            divHeader.append(hHeader);

            var aZoom = $("<a href='javascript:void(0)' class='vis-btn-zoom vis-icon-zoomFeedButton  vis-workflowActivityIcons' data-id='" + index + "'>");
            //aZoom.css("data-id", index);
            aZoom.append($("<span class='vis-btn-ico vis-btn-zoom-bg vis-btn-zoom-border'>"));
            aZoom.append(VIS.Msg.getMsg('Zoom'));
            divHeader.append(aZoom);
            aZoom.on(VIS.Events.onTouchStartOrClick, function (e) {
                var id = $(this).data("id");
                zoom(id);
            });

            divHeader.append($("<div class='clearfix'>"));

            var ul = $("<ul class='vis-IIColumnContent'>");
            divDetail.append(ul);

            var li1 = $("<li>");
            li1.css('width', '100%');
            var p1 = $("<p>");
            p1.append(VIS.Msg.getMsg('Node'));
            p1.append($("<br>"));
            p1.append(VIS.Utility.encodeText(fulldata[index].NodeName));
            li1.append(p1);
            ul.append(li1);


            // var li2 = $("<li>");
            var p2 = $("<p>");
            p2.css('margin-top', '10px');
            p2.append(VIS.Msg.getMsg('Summary'));
            p2.append($("<br>"));
            p2.append(VIS.Utility.encodeText(fulldata[index].Summary));
            li1.append(p2);
            //ul.append(li2);

            divDetail.append($("<div class='clearfix'>"));

            var hDesc = $("<h4>");
            hDesc.append(VIS.Msg.getMsg('Description'));
            divDetail.append(hDesc);
            var pDesc = $("<p>");
            pDesc.append(VIS.Utility.encodeText(fulldata[index].Description));
            divDetail.append(pDesc);

            divDetail.append($("<div class='clearfix'>"));

            var hHelp = $("<h4>");
            //hHelp.append($("<span class='vis-workflowActivityIcons vis-icon-help'>"))
            hHelp.append(VIS.Msg.getMsg('Help'));
            divDetail.append(hHelp);
            var pHelp = $("<p>");
            pHelp.append(VIS.Utility.encodeText(fulldata[index].Help));
            divDetail.append(pHelp);

            divDetail.append($("<h3>").append(VIS.Msg.getMsg('Action')));
            divDetail.append($("<div class='clearfix'>"));

            var ulA = $("<ul class='vis-IIColumnContent'>");

            var liAInput = $("<li>");
            ulA.append(liAInput);
            liAInput.append($("<p>").append(VIS.Msg.getMsg('Answer')));
            //Get Answer Control

            if (info.NodeAction == 'C') {
                var ctrl = getControl(info, wfActivityID);
                detailCtrl.AnswerCtrl = ctrl;
                if (ctrl != null) {


                    if (ctrl.getBtnCount() > 0) {
                        var divFwd = $("<div>");
                        divFwd.append(ctrl.getControl().css("width", "86%").css("margin-bottom", "10px"));
                        divFwd.append(ctrl.getBtn(0).css("width", "14%").css("height", '29px').css('padding', '0px').css('border-color', '#BBBBBB'));
                        liAInput.append(divFwd);

                    }
                    else {
                        liAInput.append(ctrl.getControl());
                    }
                    detailCtrl.AnswerCtrl = ctrl;
                }
                detailCtrl.Action = 'C';
            }
            else if (info.NodeAction == 'W') {
                var ansBtn = $('<button style="margin-bottom:10px;margin-top: 0px;width: 100%;" class="VIS_Pref_pass-btn" data-id="' + index + '" data-window="' + info.AD_Window_ID + '" data-col="' + info.KeyCol + '">').append(info.NodeName);
                detailCtrl.AnswerCtrl = ansBtn;
                liAInput.append(ansBtn);
                ansBtn.on('click', function () {

                    ansBtnClick($(this).data("id"), $(this).data("window"), $(this).data("col"));
                });
                detailCtrl.Action = 'W';
            }


            liAInput.append($("<p>").append(VIS.Msg.getMsg('Forward')));

            //Get User Lookup
            var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, "AD_User_ID", 0, false, null);
            var txtb = new VIS.Controls.VTextBoxButton("AD_User_ID", false, false, true, VIS.DisplayType.Search, lookup);
            detailCtrl.FwdCtrl = txtb;
            txtb.getBtn();

            if (txtb.getBtnCount() == 2) {
                var divFwd = $("<div>");
                divFwd.append(txtb.getControl().css("width", "86%").css("margin-bottom", "10px"));
                divFwd.append(txtb.getBtn(0).css("width", "14%").css("height", '29px').css('padding', '0px').css('border-color', '#BBBBBB'));
                liAInput.append(divFwd);

            }

            //Add Vtextbox button for User Selection

            var liDoit = $("<li>");
            var aOk = $("<a href='javascript:void(0)' class='vis-btn vis-btn-done vis-icon-doneButton vis-workflowActivityIcons' data-id='" + index + "'>");
            //aOk.css("data-id",index);
            aOk.append($("<span class='vis-btn-ico vis-btn-done-bg vis-btn-done-border'>"));
            aOk.append(VIS.Msg.getMsg('Done'));
            liDoit.append(aOk);
            aOk.on(VIS.Events.onTouchStartOrClick, function (e) {

                var id = $(this).data("id");
                approveIt(id);
            });
            ulA.append(liDoit);

            divDetail.append(ulA);
            divDetail.append($("<div class='clearfix'>"));

            divDetail.append($("<p>").append(VIS.Msg.getMsg('Message')));
            divDetail.append($("<div class='clearfix'>"));

            var divMsg = $("<div class='vis-sendMessage'>");
            var msg = $("<input type='text' placeholder='" + VIS.Msg.getMsg('TypeMessage') + "....'>");
            detailCtrl.MsgCtrl = msg;
            divMsg.append(msg);
            divMsg.append($("<input  type='button' class='vis-feedIcons vis-icon-message'>"));
            divMsg.append($("<div class='clearfix'>"));

            divDetail.append(divMsg);

            divDetail.append($("<h3>").append(VIS.Msg.getMsg('History')));
            divDetail.append($("<div class='clearfix'>"));

            var divHistory = $("<div class='vis-history-wrap'>");
            divDetail.append(divHistory);

            lstDetailCtrls.push(detailCtrl);

            if (info.Node != null) {
                var divHistoryNode = $("<div style='margin-top:15px;margin-bottom:15px'>");

                for (node in info.Node) {

                    if (info.Node[node].History != null) {
                        for (hNode in info.Node[node].History) {

                            if (info.Node[node].History[hNode].State == 'CC'
                                && node < (info.Node.length - 1)) {
                                divHistoryNode.append($("<div class='vis-vertical-img'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/home/4.jpg'>")));
                                var divAppBy = $("<div class='vis-approved_wrap'>");
                                var nodename = '';
                                nodename = info.Node[node].Name;



                                var divLeft = $("<div class='vis-left-part'>");
                                if (info.Node[node].History[hNode].TextMsg.length > 0) {
                                    var btnDetail = $("<a href='javascript:void(0)' class='VIS_Pref_tooltip' style='margin-right:5px'>").append($("<img class='VIS_Pref_img-i'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/i.png"));
                                    var span = $("<span>");
                                    span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
                                    span.append($("<label class='VIS_Pref_Label_Font'>").append(info.Node[node].History[hNode].TextMsg))
                                    btnDetail.append(span);

                                    divLeft.append(btnDetail);
                                }
                                divLeft.append(nodename);
                                divAppBy.append(divLeft);
                                var divRight = $("<div class='vis-right-part'>");
                                divRight.append(VIS.Msg.getMsg('CompletedBy')).append($("<span class='vis-app_by'>").append(info.Node[node].History[hNode].ApprovedBy));
                                //divRight.append(btnDetail);
                                divAppBy.append(divRight);
                                divHistoryNode.append(divAppBy);

                            }
                            else if ((node < (info.Node.length - 1)) || info.Node.length == 1) {
                                var divAppBy = $("<div class='vis-pending_wrap' style='margin-top:-2px'>");
                                divAppBy.append($("<div class='vis-left-part'>").append(info.Node[node].Name));
                                divAppBy.append($("<div class='vis-right-part'>").append(VIS.Msg.getMsg('Pending')));
                                divHistoryNode.append(divAppBy);
                                //divHistoryNode.append($("<div class='vis-vertical-img'>").append($("<img src='/ViennaAdvantageWeb/Areas/VIS/Images/home/4.jpg'>")));
                            }
                            else {
                                divHistoryNode.append($("<div class='vis-vertical-img'>").append($("<img src='" + VIS.Application.contextUrl + "Areas/VIS/Images/home/4.jpg'>")));
                                var divStart = $("<div class='vis-start_wrap' style='margin-bottom:-8px'>");


                                var divLeft = $("<div class='vis-left-part'>");
                                if (info.Node[node].History[hNode].TextMsg.length > 0) {
                                    var btnDetail = $("<a href='javascript:void(0)' class='VIS_Pref_tooltip' style='margin-right:5px'>").append($("<img class='VIS_Pref_img-i'>").attr("src", VIS.Application.contextUrl + "Areas/VIS/Images/i.png"));
                                    var span = $("<span >");
                                    span.append($("<img class='VIS_Pref_callout'>").attr('src', VIS.Application.contextUrl + "Areas/VIS/Images/ccc.png").append("ToolTip Text"));
                                    span.append($("<label class='VIS_Pref_Label_Font'>").append(info.Node[node].History[hNode].TextMsg))
                                    btnDetail.append(span);

                                    divLeft.append(btnDetail);
                                }
                                divLeft.append(info.Node[node].Name);

                                divStart.append(divLeft);
                                var divRight = $("<div class='vis-right-part'>");
                                divRight.append(VIS.Msg.getMsg('CompletedBy')).append($("<span class='vis-app_by'>").append(info.Node[node].History[hNode].ApprovedBy));
                                //divRight.append(btnDetail);
                                divStart.append(divRight);
                                // divStart.append($("<div class='vis-right-part'>").append(VIS.Msg.getMsg('CompletedBy')).append($("<span class='vis-app_by'>").append(info.Node[node].History[hNode].ApprovedBy)));
                                divHistoryNode.append(divStart);
                            }
                        }
                        divHistory.append(divHistoryNode);
                    }


                }
            }
            //  $("#divfeedbsy")[0].style.visibility = "hidden";
            $("#divTabMenuDataLoder").hide();
        };

        var zoom = function (index) {

            //alert(index);
            //data[index].Record_ID;

            var sql = "SELECT TableName, AD_Window_ID, PO_Window_ID FROM AD_Table WHERE AD_Table_ID=" + fulldata[index].AD_Table_ID;


            //var sql = "select ad_window_id from ad_window where name='Mail Configuration'";// Upper( name)=Upper('user' )
            var ad_window_Id = -1;
            var tableName = '';
            try {
                var dr = VIS.DB.executeDataReader(sql);
                if (dr.read()) {
                    ad_window_Id = dr.getInt(1);
                    tableName = dr.get(0);
                }
                dr.dispose();
            }
            catch (e) {
                this.log.log(VIS.Logging.Level.SEVERE, sql, e);
            }

            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction(tableName + "_ID", VIS.Query.prototype.EQUAL, fulldata[index].Record_ID);
            VIS.viewManager.startWindow(ad_window_Id, zoomQuery);
        };

        var approveIt = function (index) {

            $("#divfeedbsy")[0].style.visibility = "visible";
            window.setTimeout(function () {
                for (var item in lstDetailCtrls) {
                    try {
                        if (index === lstDetailCtrls[item].Index) {
                            var fwdTo = lstDetailCtrls[item].FwdCtrl.getValue();
                            var msg = lstDetailCtrls[item].MsgCtrl.val();
                            var answer = null;
                            if (lstDetailCtrls[item].Action == 'C') {
                                var answer = lstDetailCtrls[item].AnswerCtrl.getValue();

                            }

                            var info = (VIS.dataContext.getJSONData(VIS.Application.contextUrl + "WFActivity/ApproveIt",
                                { "activityID": fulldata[index].AD_WF_Activity_ID, "nodeID": fulldata[index].AD_Node_ID, "txtMsg": msg, "fwd": fwdTo, "answer": answer })).result;

                            if (info == '') {
                                //refresh
                                //alert("Done");
                                $("#divfeedbsy")[0].style.visibility = "hidden";
                                $('#workflowActivity').hide();
                                container.empty();
                                adjust_size();

                                // $("#divfeedbsy")[0].style.visibility = "visible";
                                $("#divTabMenuDataLoder").show();
                                $.ajax({
                                    url: VIS.Application.contextUrl + "WFActivity/GetActivities",
                                    dataType: "json",
                                    type: "POST",
                                    data: { pageNo: 1, pageSize: 10, refresh: true },
                                    error: function () {
                                        //alert(VIS.Msg.getMsg('ErrorWhileGettingData'));
                                        //bsyDiv[0].style.visibility = "hidden";
                                        //  $("#divfeedbsy")[0].style.visibility = "hidden";
                                        $("#divTabMenuDataLoder").hide();
                                        return;
                                    },
                                    success: function (dyndata) {
                                        //lstDetailCtrls = null;

                                        lstDetailCtrls = [];
                                        if (VIS.HomeMgr.getActiveTab().activeTabType == 3) {
                                            if (dyndata.result == null || dyndata.result.LstInfo == null) {
                                                $("#divTabMenuDataLoder").hide();
                                                $("#divfActivity").empty();
                                                $("#divfActivity").append(0);


                                                //$("#sAlrtTxtType").empty();
                                                //$("#sAlrtTxtType").append(VIS.Msg.getMsg('WorkflowActivities'));
                                                $("#hAlrtTxtTypeCount").empty();
                                                $("#hAlrtTxtTypeCount").append(0);

                                                return;
                                            }
                                            data = dyndata.result.LstInfo;
                                            container.empty();

                                            // if (refresh == true) {
                                            //Reset Count Header
                                            dataItemDivs = [];
                                            fulldata = [];
                                            $("#divfActivity").empty();
                                            $("#divfActivity").append(dyndata.result.count);
                                            //}

                                            //$("#sAlrtTxtType").empty();
                                            //$("#sAlrtTxtType").append(VIS.Msg.getMsg('WorkflowActivities'));
                                            $("#hAlrtTxtTypeCount").empty();
                                            $("#hAlrtTxtTypeCount").append(dyndata.result.count);

                                            LoadRecords(0);
                                            VIS.HomeMgr.adjustDivSize();
                                            //$("#divfActivity").empty();
                                            //$("#divfActivity").append(dyndata.result.count);
                                            //LoadRecords(0);
                                        }
                                    }
                                });
                            }
                            else {

                                alert(info);
                            }
                            break;
                        }
                    }
                    catch (e) {

                        alert('FillManadatory');
                    }

                }
                $("#divfeedbsy")[0].style.visibility = "hidden";
            }, 2);

        };

        var getControl = function (info, wfActivityID) {


            var ctrl = null;

            if (info.ColID == 0) {
                return ctrl;
            }


            if (info.ColReference == VIS.DisplayType.YesNo) {

                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.List, info.ColName, 319, false, null);
                ctrl = new VIS.Controls.VComboBox(info.ColName, false, false, true, lookup, 50);
                return ctrl;
            }
            else if (info.ColReference == VIS.DisplayType.List) {
                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.List, info.ColName, info.ColReferenceValue, false, null);
                ctrl = new VIS.Controls.VComboBox(info.ColName, false, false, true, lookup, 50);
                return ctrl;
            }
            else if (info.ColName.toUpperCase() == "C_GENATTRIBUTESETINSTANCE_ID") {
               // alert('Gen Attribute Not Implement Yet');
                var vAttSetInstance = null;
                var lookupCur = new VIS.MGAttributeLookup(VIS.context, 0);
                $.ajax({
                    url: VIS.Application.contextUrl + "WFActivity/GetRelativeData",
                    async: false,
                    data: { activityID: wfActivityID },
                    dataType: "json",
                    success: function (dyndata) {
                        if (dyndata.result) {
                            vAttSetInstance = new VIS.Controls.VPAttribute('C_GenAttributeSetInstance', true, false, true, VIS.DisplayType.PAttribute, lookupCur, 0, true, false, false, false);
                            vAttSetInstance.SetC_GenAttributeSet_ID(dyndata.result.GenAttributeSetID);
                        }
                    }
                });


                return vAttSetInstance;
            }
            else if (info.ColReference == VIS.DisplayType.TableDir) {
                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.TableDir, info.ColName, info.ColReferenceValue, false, null);
                ctrl = new VIS.Controls.VComboBox(info.ColName, false, false, true, lookup, 50);
                return ctrl;
            }
            else if (info.ColReference == VIS.DisplayType.Search) {
                var lookup = VIS.MLookupFactory.get(VIS.context, 0, 0, VIS.DisplayType.Search, info.ColName, info.ColReferenceValue, false, null);
                ctrl = new VIS.Controls.VTextBoxButton(info.ColName, false, false, true, VIS.DisplayType.Search, lookup);
                return ctrl;
            }
            else {
                ctrl = new VIS.Controls.VTextBox(info.ColName, false, false, true, 50, 100, null, null, false);
                return ctrl;
            }
        };

        var ansBtnClick = function (index, AD_Window_ID, columnName) {
            var zoomQuery = new VIS.Query();
            zoomQuery.addRestriction(columnName, VIS.Query.prototype.EQUAL, fulldata[index].Record_ID);
            VIS.viewManager.startWindow(AD_Window_ID, zoomQuery);
        };

        this.clear = function () {

            for (var itm in dataItemDivs) {
                dataItemDivs[itm] = null;
            }

        };

    }; VIS.wfActivity = wfActivity;
})(VIS, jQuery);