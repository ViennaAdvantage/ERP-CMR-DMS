
/********************************************************
 * Module Name    :     Application
 * Purpose        :     Generate Product info Scan Form.
 * Author         :     Bharat
 * Date           :     27-May-2015
  ******************************************************/
; (function (VIS, $) {

    function InfoScanForm(title, M_Warehouse_ID, M_Locator_ID, M_Product_ID, C_BPartner_ID) {

        var $self = this;
        this.onClose = null;
        var $root = $("<div>");
        var $busyDiv = $("<div class='vis-apanel-busy'>")

        var mWarehouseID = M_Warehouse_ID;
        var mLocatorID = M_Locator_ID;
        var mProductID = M_Product_ID;
        var mCBPartnerID = C_BPartner_ID;
        var mtitle = title;
        this.arrListColumns = [];
        this.dGrid = null;

        var mAttributeSetInstanceID = -1;
        var mAttributeSetInstanceName = null;
        var msql = "";

        //	From Clause						
        var msqlFrom = "M_Storage s"
            + " INNER JOIN M_Locator l ON (s.M_Locator_ID=l.M_Locator_ID)"
            + " INNER JOIN M_Warehouse w ON (l.M_Warehouse_ID=w.M_Warehouse_ID)"
            + " INNER JOIN M_Product p ON (s.M_Product_ID=p.M_Product_ID)"
            + " LEFT OUTER JOIN M_AttributeSetInstance asi ON (s.M_AttributeSetInstance_ID=asi.M_AttributeSetInstance_ID)";

        var msqlWhere = " s.M_Product_ID=@M_Product_ID AND s.M_AttributeSetInstance_ID != 0";
        var msqlNonZero = " AND (s.QtyOnHand<>0 OR s.QtyReserved<>0 OR s.QtyOrdered<>0)";
        var msqlMinLife = "";

        this.log = VIS.Logging.VLogger.getVLogger("PAttributeInstance");
        var windowNo = VIS.Env.getWindowNo();

        var chkShowAll = $("<input id='" + "chkShowAll_" + windowNo + "' type='checkbox' >" +
               "<span><label id='" + "lblShowAll_" + windowNo + "' class='VIS_Pref_Label_Font'>" + VIS.Msg.getMsg("ShowAll") + "</label></span>");

        var btnOk = $("<input id='" + "btnOk_" + windowNo + "' class='VIS_Pref_btn-2' style='margin-bottom: 1px; margin-top: 5px; float: right; margin-right: 15px ;' type='button' value='" + VIS.Msg.getMsg("OK") + "'>");

        var btnCancel = $("<input id='" + "btnCancel_" + windowNo + "' class='VIS_Pref_btn-2' style='margin-bottom: 1px; margin-top: 5px; float: right;margin-right: 0px;' type='button' value='" + VIS.Msg.getMsg("Cancel") + "'>");

        var topdiv = $("<div id='" + "topdiv_" + windowNo + "' style='float: left; width: 100%; height: 10%; text-align: right;'>");
        var middeldiv = $("<div id='" + "middeldiv_" + windowNo + "' style='float: left; width: 100%; height: 244px;'>");
        var bottomdiv = $("<div id='" + "bottomdiv_" + windowNo + "' style='float: left; width: 100%; height: 14%;'>");


        if (VIS.Application.isRTL) {
            topdiv.css("text-align", "left");
            btnOk.css("margin-right", "-130px");
            btnCancel.css("margin-right", "55px");
        }


        function dynInit(C_BPartner_ID) {
            $self.log.config("C_BPartner_ID=" + C_BPartner_ID);
            if (C_BPartner_ID != 0) {
                var shelfLifeMinPct = 0;
                var shelfLifeMinDays = 0;
                var sql = "SELECT bp.ShelfLifeMinPct, bpp.ShelfLifeMinPct, bpp.ShelfLifeMinDays "
                    + "FROM C_BPartner bp "
                    + " LEFT OUTER JOIN C_BPartner_Product bpp"
                    + " ON (bp.C_BPartner_ID=bpp.C_BPartner_ID AND bpp.M_Product_ID=" + mProductID + ") "
                    + "WHERE bp.C_BPartner_ID=" + C_BPartner_ID;

                var dr = null;
                try {

                    dr = VIS.DB.executeReader(sql, null);
                    if (dr.read()) {
                        shelfLifeMinPct = dr.getInt(0);		//	BP
                        var pct = dr.getInt(1);				//	BP_P
                        if (pct > 0)	//	overwrite
                        {
                            shelfLifeMinDays = pct;
                        }
                        shelfLifeMinDays = dr.getInt(2);
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

                if (shelfLifeMinPct > 0) {
                    msqlMinLife = " AND COALESCE(TRUNC((daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD'))/p.GuaranteeDays)*100),0)>=" + shelfLifeMinPct;
                    $self.log.config("PAttributeInstance.dynInit - ShelfLifeMinPct=" + shelfLifeMinPct);
                }
                if (shelfLifeMinDays > 0) {
                    msqlMinLife += " AND COALESCE(daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD')),0)>=" + shelfLifeMinDays;
                    $self.log.config("PAttributeInstance.dynInit - ShelfLifeMinDays=" + shelfLifeMinDays);
                }
            }	//	BPartner != 0

            msql = prepareTable(msqlFrom, msqlWhere, false, "s") + " ORDER BY asi.GuaranteeDate, s.QtyOnHand";	//	oldest, smallest first
            refresh();

            topdiv.append(chkShowAll);
            bottomdiv.append(btnCancel).append(btnOk);
            $root.append($busyDiv).append(topdiv).append(middeldiv).append(bottomdiv);
            events();
        }

        function prepareTable(from, where, multiSelection, tableName) {
            var sql = "SELECT s.M_AttributeSetInstance_ID, asi.Description, asi.Lot, asi.SerNo, asi.GuaranteeDate, l.Value,s.M_Locator_ID," +
                " s.QtyOnHand, s.QtyReserved, s.QtyOrdered, (daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD'))-p.GuaranteeDaysMin) as ShelfLifeDays," +
                " daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD')) as GoodForDays, CASE WHEN p.GuaranteeDays > 0 THEN " +
                " ROUND(daysBetween(TRUNC(asi.GuaranteeDate,'DD'),TRUNC(SysDate,'DD'))/p.GuaranteeDays*100,12) ELSE 0 END as ShelfLifeRemainingPct";

            sql = sql.concat(" FROM ").concat(from);
            sql = sql.concat(" WHERE ").concat(where);

            //if (mLocatorID != 0) {
            //    sql = sql.concat(" AND s.M_Locator_ID = " + mLocatorID);
            //}
            if (mWarehouseID != 0) {
                sql = sql.concat(" AND w.M_Warehouse_ID = " + mWarehouseID);
            }

            if (from.length == 0) {
                return sql.toString();
            }
            //
            $self.log.finest(finalSQL);
            var finalSQL = VIS.MRole.getDefault().addAccessSQL(sql, tableName, VIS.MRole.SQL_FULLYQUALIFIED, VIS.MRole.SQL_RO);
            return finalSQL
        }

        function refresh() {
            if (msql == null) {
                msql = "";
            }
            var data = [];

            var sql = msql;
            var pos = msql.lastIndexOf(" ORDER BY ");
            if (!chkShowAll.prop("checked")) {
                sql = msql.substring(0, pos) + msqlNonZero;
                if (msqlMinLife.length > 0) {
                    sql += msqlMinLife;
                }
                sql += msql.substring(pos);
            }
            //
            $self.log.finest(sql);

            try {
                var param = [];
                param[0] = new VIS.DB.SqlParam("@M_Product_ID", mProductID);
                var dr = VIS.DB.executeReader(sql, param);
                var count = 1;
                while (dr.read()) {
                    var line = {};
                    line['M_AttributeSetInstance_ID'] = dr.getInt("M_AttributeSetInstance_ID");
                    line['Description'] = dr.getString("Description");
                    line['Lot'] = dr.getString("Lot");
                    line['SerNo'] = dr.getString("SerNo");
                    line['GuaranteeDate'] = dr.getString("GuaranteeDate");
                    line['Value'] = dr.getString("Value");
                    line['QtyReserved'] = dr.getString("QtyReserved");
                    line['QtyOrdered'] = dr.getString("QtyOrdered");
                    line['QtyOnHand'] = dr.getString("QtyOnHand");
                    line['GoodForDays'] = dr.getString("GoodForDays");
                    line['ShelfLifeDays'] = dr.getString("ShelfLifeDays");
                    line['ShelfLifeRemainingPct'] = dr.getString("ShelfLifeRemainingPct");
                    line['M_Locator_ID'] = dr.getString("M_Locator_ID");
                    line['recid'] = count;
                    count++;
                    data.push(line);
                }
                enableButtons();
            }
            catch (e) {
                //$self.log.Log(Level.SEVERE, sql, e);
            }

            loadGrid(data);
        }

        function loadGrid(data) {

            if ($self.dGrid != null) {
                $self.dGrid.destroy();
                $self.dGrid = null;
            }
            if ($self.arrListColumns.length == 0) {

                $self.arrListColumns.push({ field: "Description", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Description"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "Lot", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Lot"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "SerNo", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "SerNo"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "GuaranteeDate", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "GuaranteeDate"), sortable: true, size: '16%', min: 150, hidden: false, render: 'date' });
                $self.arrListColumns.push({ field: "Value", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_Locator_ID"), sortable: true, size: '16%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "QtyReserved", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "QtyReserved"), sortable: true, size: '20%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "QtyOrdered", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "QtyOrdered"), sortable: true, size: '20%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "QtyOnHand", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "QtyOnHand"), sortable: true, size: '20%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "GoodForDays", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "GoodForDays"), sortable: true, size: '20%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "ShelfLifeDays", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "ShelfLifeDays"), sortable: true, size: '20%', min: 150, hidden: false });
                $self.arrListColumns.push({ field: "ShelfLifeRemainingPct", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "ShelfLifeRemainingPct"), sortable: true, size: '20%', min: 150, hidden: false });

                $self.arrListColumns.push({ field: "M_Locator_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_Locator_ID"), sortable: true, size: '16%', min: 150, hidden: true });
                $self.arrListColumns.push({ field: "M_AttributeSetInstance_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ID"), sortable: true, size: '16%', min: 150, hidden: true });

            }
            setTimeout(10);
            w2utils.encodeTags(data);
            $self.dGrid = $(middeldiv).w2grid({
                name: "gridPinstance" + windowNo,
                recordHeight: 30,
                columns: $self.arrListColumns,
                records: data,
                onClick: function (event) {
                    if ($self.dGrid.records.length > 0) {
                        btnOk.removeAttr("disabled");
                    }
                }
            });
            //$self.dGrid.hideColumn('M_Locator_ID');
        }

        function events() {

            if (btnOk != null)
                btnOk.on(VIS.Events.onTouchStartOrClick, function () {
                    enableButtons();
                    if ($self.onClose)
                        $self.onClose(mAttributeSetInstanceID, mAttributeSetInstanceName, mLocatorID);
                    $root.dialog('close');
                });

            if (btnCancel != null)
                btnCancel.on(VIS.Events.onTouchStartOrClick, function () {
                    $root.dialog('close');
                });            
        }

        this.showDialog = function () {
            $root.dialog({
                modal: true,
                title: VIS.Msg.translate(VIS.Env.getCtx(), mtitle),
                width: 800,
                height: 400,
                resizable: false,
                close: function () {
                    $self.dispose();
                    $self = null;
                    $root.dialog("destroy");
                    $root = null;
                }
            });

            refresh();
        };

        this.disposeComponent = function () {

            if (btnOk)
                btnOk.off("click");
            if (btnCancel)
                btnCancel.off("click");

            btnOk = null;
            btnCancel = null;

            windowNo = null;
            this.onClose = null;
            $busyDiv = null;

            mWarehouseID = null;
            mLocatorID = null;
            mProductID = null;
            mCBPartnerID = null;
            mtitle = null;
            this.arrListColumns = null;
            this.dGrid = null;
            msqlFrom = null;
            msqlWhere = null;
            msqlNonZero = null;
            msqlMinLife = null;
            this.log = null;
            chkShowAll = null;
            topdiv = null;
            middeldiv = null;
            bottomdiv = null;

            mAttributeSetInstanceID = null;
            mAttributeSetInstanceName = null;
            msql = null;
            this.disposeComponent = null;
        };

        dynInit(mCBPartnerID);

    };

    //dispose call
    InfoScanForm.prototype.dispose = function () {

        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();
    };

    VIS.InfoScanForm = InfoScanForm;

})(VIS, jQuery);
