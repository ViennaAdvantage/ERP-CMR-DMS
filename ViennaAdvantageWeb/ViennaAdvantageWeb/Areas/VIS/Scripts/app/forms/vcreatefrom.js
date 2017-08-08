
; (function (VIS, $) {

    //form declaretion
    function VCreateFrom(mTab) {

        //call parent function on close
        this.onClose = null;
        this.dGrid = null;
        var SELECT_DESELECT_ALL = "SelectDeselectAll";
        //select all button status
        var _checkStatus = true;
        //To button pic on grid selected records
        this.arrListColumns = [];

        var $self = this;
        this.$root = $("<div style='position:relative'>");

        this.$busyDiv = $("<div class='vis-apanel-busy' style='width:98%;height:98%;position:absolute'>");

        this.topDiv = $("<div style='float: left; width: 100%;'>");
        this.middelDiv = $("<div style='height: 60%;float: left; width: 99.5%;border: 1px solid;'>");
        this.bottomDiv = $("<div style='height: 40px; width: 100%; float: left ;margin-top: 10px;'>");

        this.windowNo = mTab.getWindowNo();
        this.mTab = mTab;
        this.initOK = false;


        var name = "btnOk_" + this.windowNo;
        this.Okbtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' style='margin-bottom:0px;margin-top:0px ;float:right; margin-right:10px' type='button' value='" + VIS.Msg.getMsg("OK") + "'>");

        name = "btnCancel_" + this.windowNo;
        this.cancelbtn = $("<input id='" + name + "' class='VIS_Pref_btn-2' style='margin-bottom:0px;margin-top:0px; margin-right: 3px; float:right' type='button' value='" + VIS.Msg.getMsg("Cancel") + "'>");

        name = "btnDelete_" + this.windowNo;
        var src = VIS.Application.contextUrl + "Areas/VIS/Images/base/uncheck-icon.png";
        this.selectAllButton = $("<button type='button' id='" + name + "' style='margin-bottom:0px;margin-top:0px;display: none;float: left;padding: 0px;border: 0px;' value='Ok' role='button' aria-disabled='false'>" +
       "<img src='" + src + "'></button>");

        name = "btnRefresh_" + this.windowNo;
        src = VIS.Application.contextUrl + "Areas/VIS/Images/base/Refresh24.png";
        this.btnRefresh = $("<button id='" + name + "' style='margin-bottom: 1px; margin-top: 0px;display: none; float: left; margin-left: 0px;height: 38px; ' class='VIS_Pref_btn-2'>" +
                   "<img src='" + src + "'></button>");

        //check Arebic Calture
        if (VIS.Application.isRTL) {
            this.Okbtn.css("margin-right", "-132px");
            this.cancelbtn.css("margin-right", "55px");
        }


        this.lblBankAccount = new VIS.Controls.VLabel();
        this.lblBPartner = new VIS.Controls.VLabel();
        this.lblOrder = new VIS.Controls.VLabel();
        this.lblInvoice = new VIS.Controls.VLabel();
        this.lblShipment = new VIS.Controls.VLabel();
        this.lblLocator = new VIS.Controls.VLabel();
        this.cmbOrder = new VIS.Controls.VComboBox('', false, false, true);
        this.cmbInvoice = new VIS.Controls.VComboBox('', false, false, true);
        this.cmbShipment = new VIS.Controls.VComboBox('', false, false, true);
        this.cmbBankAccount = null;
        this.vBPartner = null;
        this.locatorField = null;
        this.child = null;
        this.relatedToOrg = new VIS.Controls.VCheckBox('Related To My Organization', false, false, true, 'Related To My Organization', null);

        this.title = "";
        this.vetoablechange = function (evt) {
            this.locatorField.setBackground("");
        };

        this.setBusy = function (isBusy) {
            $self.$busyDiv.css("display", isBusy ? 'block' : 'none');
        };

        this.showDialog = function () {
            if (this.locatorField) {
                this.locatorField.setBackground(false);
                this.locatorField.addVetoableChangeListener(this);
            }

            var obj = this;
            this.$root.append(this.$busyDiv);
            this.setBusy(false);
            this.$root.dialog({
                modal: true,
                title: this.title,
                width: 900,
                height: 500,
                resizable: false,
                position: { at: "center top", of: window },
                close: function () {
                    obj.dispose();
                    if (obj.dGrid != null) {
                        obj.dGrid.destroy();
                    }
                    $self = null;
                    obj.$root.dialog("destroy");
                    obj.$root = null;
                }
            });
        };

        this.disposeComponent = function () {
            $self = null;
            if (this.Okbtn)
                this.Okbtn.off("click");
            if (this.cancelbtn)
                this.cancelbtn.off("click");
            if (this.selectAllButton)
                this.selectAllButton.off("click");
            if (this.btnRefresh)
                this.btnRefresh.off("click");

            this.disposeComponent = null;
        };
    };

    //dispose call
    VCreateFrom.prototype.dispose = function () {
        this.disposeComponent();
    };

    VCreateFrom.prototype.create = function (mTab) {
        //	dynamic init preparation
        var AD_Table_ID = VIS.Env.getCtx().getContextAsInt(mTab.getWindowNo(), "BaseTable_ID");

        var retValue = null;// VCreateFrom form object
        if (AD_Table_ID == 392)             //  C_BankStatement
        {
            retValue = new VIS.VCreateFromStatement(mTab);
        }
        else if (AD_Table_ID == 318)        //  C_Invoice
        {
            retValue = new VIS.VCreateFromInvoice(mTab);
        }
        else if (AD_Table_ID == 319)        //  M_InOut
        {
            retValue = new VIS.VCreateFromShipment(mTab);
        }
        else if (AD_Table_ID == 426)		//	C_PaySelection
        {
            return null;	//	ignore - will call process C_PaySelection_CreateFrom
        }
        else    //  Not supported CreateFrom
        {
            return null;
        }
        return retValue;
    }

    VCreateFrom.prototype.initBPDetails = function (C_BPartner_ID) {
        return C_BPartner_ID;
    };

    VCreateFrom.prototype.initBPartner = function (forInvoice) {
        //  load BPartner
        var AD_Column_ID = 3499;        //  C_Invoice.C_BPartner_ID
        var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), this.windowNo, AD_Column_ID, VIS.DisplayType.Search);

        this.vBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, lookup);
        var C_BPartner_ID = VIS.Env.getCtx().getContextAsInt(this.windowNo, "C_BPartner_ID");
        this.vBPartner.setValue(C_BPartner_ID);
        //  initial loading
        return this.initBPartnerOIS(C_BPartner_ID, forInvoice);
    }

    VCreateFrom.prototype.initBPartnerOIS = function (C_BPartner_ID, forInvoice) {
        var orcmb = this.cmbOrder;
        //$(orcmb).empty()
        $(orcmb).html("");

        var isReturnTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(this.windowNo, "IsReturnTrx"));
        var orders = this.getOrders(VIS.Env.getCtx(), C_BPartner_ID, isReturnTrx, forInvoice);

        for (var i = 0; i < orders.length; i++) {
            if (i == 0) {
                this.cmbOrder.getControl().append(" <option value=0> </option>");
            }
            this.cmbOrder.getControl().append(" <option value=" + orders[i].ID + ">" + orders[i].value + "</option>");
        };

        this.cmbOrder.getControl().prop('selectedIndex', 0);
        return this.initBPDetails(C_BPartner_ID);
    }

    VCreateFrom.prototype.getOrders = function (ctx, C_BPartner_ID, isReturnTrx, forInvoice) {
        var pairs = [];
        // Display
        var display = "o.DocumentNo||' - ' ||".concat(VIS.DB.to_char("o.DateOrdered", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("||' - '||").concat(
                VIS.DB.to_char("o.GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));

        var column = "m.M_InOutLine_ID";
        if (forInvoice) {
            column = "m.C_InvoiceLine_ID";
        }

        var sql = ("SELECT o.C_Order_ID,").concat(display).concat(
                " FROM C_Order o " + "WHERE o.C_BPartner_ID=" + C_BPartner_ID + " AND o.IsSOTrx='N' AND o.DocStatus IN ('CL','CO') "
                        + "AND o.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' AND o.C_Order_ID IN "
                        + "(SELECT ol.C_Order_ID FROM C_OrderLine ol"
                        + " LEFT OUTER JOIN M_MatchPO m ON (ol.C_OrderLine_ID=m.C_OrderLine_ID) "
                        + "GROUP BY ol.C_Order_ID,ol.C_OrderLine_ID, ol.QtyOrdered,").concat(column).concat(
                " HAVING (ol.QtyOrdered <> SUM(m.Qty) AND ").concat(column).concat(" IS NOT NULL) OR ").concat(column)
                .concat(" IS NULL) " + "ORDER BY o.DateOrdered");
        try {
            var dr = VIS.DB.executeReader(sql.toString(), null);
            var key, value;
            while (dr.read()) {
                key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                value = dr.getString(1);
                pairs.push({ ID: key, value: value });
            }
            dr.close();
        }
        catch (e) {
        }
        return pairs;
    }

    VCreateFrom.prototype.jbInit = function () {
        this.init();
        
        this.lblBankAccount.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "C_BankAccount_ID"));
        this.lblBPartner.getControl().text(VIS.Msg.getElement(VIS.Env.getCtx(), "C_BPartner_ID"));
        this.lblOrder.getControl().text(VIS.Msg.getElement(VIS.Env.getCtx(), "C_Order_ID", false));
        this.lblInvoice.getControl().text(VIS.Msg.getElement(VIS.Env.getCtx(), "C_Invoice_ID", false));
        this.lblShipment.getControl().text(VIS.Msg.getElement(VIS.Env.getCtx(), "M_InOut_ID", false));
        this.lblLocator.getControl().text(VIS.Msg.translate(VIS.Env.getCtx(), "M_Locator_ID"));

        //Line1
        var line = $("<div class='VIS_Pref_show'>");
        var col = $("<div class='VIS_Pref_dd' style='float: left; height: 34px;'>");
        var label = $("<div style='float: left; margin-right: 5px; width: 30%; text-align: right'>");

        if (VIS.Application.isRTL) {
            label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        }

        var lableCtrl = $("<h5 style='width: 100%'>");
        var ctrl = $("<div style='float: left; width: 68%;' class='VIS_Pref_slide-show pp'>");

        this.topDiv.append(line);
        line.append(col);
        col.append(label);
        label.append(lableCtrl);

        if (this.cmbBankAccount != null) {
            col.css("float", "none");
            lableCtrl.append(this.lblBankAccount.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbBankAccount.getControl().css('width', '100%'));
            return;
        }
        if (VIS.Application.isRTL) {
            //reverse controls order
            lableCtrl.append(this.lblOrder.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbOrder.getControl().css('width', '100%'));
        }
        else {

            if (this.vBPartner != null) {
                lableCtrl.append(this.lblBPartner.getControl().addClass('VIS_Pref_Label_Font'));
                ctrl.removeClass("VIS_Pref_slide-show pp");
                col.append(ctrl);
                ctrl.append(this.vBPartner.getControl().css('width', '88%')).append(this.vBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            }
        }

        col = $("<div class='VIS_Pref_dd' style='float: left ; height: 34px;'>");
        label = $("<div style='float: left; margin-right: 5px; width: 30%; text-align: right'>");
        if (VIS.Application.isRTL) {
            label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        }
        lableCtrl = $("<h5 style='width: 100%'>");
        ctrl = $("<div style='float: left; width: 68%;' class='VIS_Pref_slide-show'>");


        line.append(col);
        col.append(label);
        label.append(lableCtrl);

        if (VIS.Application.isRTL) {
            //reverse controls order
            if (this.vBPartner != null) {
                lableCtrl.append(this.lblBPartner.getControl().addClass('VIS_Pref_Label_Font'));
                ctrl.removeClass("VIS_Pref_slide-show pp");
                col.append(ctrl);
                ctrl.append(this.vBPartner.getControl().css('width', '88%')).append(this.vBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            }
        }
        else {
            lableCtrl.append(this.lblOrder.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbOrder.getControl().css('width', '100%'));

        }

        //Line2
        line = $("<div class='VIS_Pref_show'>");
        col = $("<div class='VIS_Pref_dd' style='float: left ; height: 34px;'>");
        label = $("<div style='float: left; margin-right: 5px;width: 30%; text-align: right'>");
        if (VIS.Application.isRTL) {
            label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        }
        lableCtrl = $("<h5 style='width: 100%'>");
        ctrl = $("<div style='float: left; width: 68%;' class='VIS_Pref_slide-show pp'>");


        this.topDiv.append(line);
        line.append(col);
        col.append(label);
        label.append(lableCtrl);



        if (VIS.Application.isRTL) {
            //reverse controls order

            lableCtrl.append(this.lblInvoice.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbInvoice.getControl().css('width', '100%'));

            lableCtrl.append(this.lblShipment.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbShipment.getControl().css('width', '100%'));

        } else {

            if (this.locatorField != null) {
                lableCtrl.append(this.lblLocator.getControl().addClass('VIS_Pref_Label_Font'));
                ctrl.removeClass("VIS_Pref_slide-show pp");
                col.append(ctrl);
                ctrl.append(this.locatorField.getControl().css('width', '88%')).append(this.locatorField.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            }
        }

        col = $("<div class='VIS_Pref_dd' style='float: left ; height: 34px;'>");
        label = $("<div style='float: left; margin-right: 5px;width: 30%; text-align: right'>");
        if (VIS.Application.isRTL) {
            label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        }
        lableCtrl = $("<h5 style='width: 100%'>");
        ctrl = $("<div style='float: left; width: 68%;' class='VIS_Pref_slide-show pp'>");

        line.append(col);
        col.append(label);
        label.append(lableCtrl);

        if (VIS.Application.isRTL) {
            //reverse controls order
            if (this.locatorField != null) {
                lableCtrl.append(this.lblLocator.getControl().addClass('VIS_Pref_Label_Font'));
                ctrl.removeClass("VIS_Pref_slide-show pp");
                col.append(ctrl);
                ctrl.append(this.locatorField.getControl().css('width', '88%')).append(this.locatorField.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            }

        } else {

            lableCtrl.append(this.lblInvoice.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbInvoice.getControl().css('width', '100%'));

            lableCtrl.append(this.lblShipment.getControl().addClass('VIS_Pref_Label_Font'));
            col.append(ctrl);
            ctrl.append(this.cmbShipment.getControl().css('width', '100%'));
        }


        // specific to Batir
        //if (window.BTR001) {
        //Line3
        line = $("<div class='VIS_Pref_show'>");
        col = $("<div class='VIS_Pref_dd' style='float: left ; height: 34px;'>");
        label = $("<div style='float: left; margin-right: 5px;width: 100%; text-align: left'>");
        if (VIS.Application.isRTL) {
            label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        }
        lableCtrl = $("<h5 style='width: 100%'>");
        ctrl = $("<div style='float: left; width: 68%;' class='VIS_Pref_slide-show pp'>");


        this.topDiv.append(line);
        line.append(col);
        col.append(label);
        label.append(lableCtrl);

        //if (window.DTD001) {
            lableCtrl.append(this.relatedToOrg.getControl().css('width', '100%'));
        //}

        if (VIS.Application.isRTL) {
            //reverse controls order

            //lableCtrl.append(this.lblInvoice.getControl().addClass('VIS_Pref_Label_Font'));
            //col.append(ctrl);
            //ctrl.append(this.cmbInvoice.getControl().css('width', '100%'));

            //lableCtrl.append(this.lblShipment.getControl().addClass('VIS_Pref_Label_Font'));
            //col.append(ctrl);
            //ctrl.append(this.cmbShipment.getControl().css('width', '100%'));

        } else {

            //if (this.locatorField != null) {
            //    lableCtrl.append(this.lblLocator.getControl().addClass('VIS_Pref_Label_Font'));
            //    ctrl.removeClass("VIS_Pref_slide-show pp");
            //    col.append(ctrl);
            //    ctrl.append(this.locatorField.getControl().css('width', '88%')).append(this.locatorField.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            //}
        }

        col = $("<div class='VIS_Pref_dd' style='float: left ; height: 34px;'>");
        label = $("<div style='float: left; margin-right: 5px;width: 30%; text-align: right'>");
        if (VIS.Application.isRTL) {
            label = $("<div style='float: right; margin-right: 5px; width: 30%; text-align: left'>");
        }
        lableCtrl = $("<h5 style='width: 100%'>");
        ctrl = $("<div style='float: left; width: 68%;' class='VIS_Pref_slide-show pp'>");


        line.append(col);
        col.append(label);
        label.append(lableCtrl);

        if (VIS.Application.isRTL) {
            //reverse controls order
            //if (this.locatorField != null) {
            //    lableCtrl.append(this.lblLocator.getControl().addClass('VIS_Pref_Label_Font'));
            //    ctrl.removeClass("VIS_Pref_slide-show pp");
            //    col.append(ctrl);
            //    ctrl.append(this.locatorField.getControl().css('width', '88%')).append(this.locatorField.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            //}

        } else {

            //lableCtrl.append(this.lblInvoice.getControl().addClass('VIS_Pref_Label_Font'));
            //col.append(ctrl);
            //ctrl.append(this.cmbInvoice.getControl().css('width', '100%'));

            //lableCtrl.append(this.lblShipment.getControl().addClass('VIS_Pref_Label_Font'));
            //col.append(ctrl);
            //ctrl.append(this.cmbShipment.getControl().css('width', '100%'));
        }
        //Line3
        // }

        var value = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), this.windowNo, 3499, VIS.DisplayType.Search);
        this.vBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, value);
    }

    VCreateFrom.prototype.init = function () {
        this.$root.append(this.topDiv).append(this.middelDiv).append(this.bottomDiv);

        this.bottomDiv.append(this.selectAllButton);
        this.bottomDiv.append(this.btnRefresh);
        this.bottomDiv.append(this.cancelbtn);
        this.bottomDiv.append(this.Okbtn);
        //this.dGrid= VIS.Utility.Util.gridLoad(,,);
        this.middelDiv.append(this.dGrid);
        $self = this;
        this.Okbtn.on("click", function () {
            
            var output = false;
            $self.setBusy(true);
            if ($self.locatorField != null) {
                //for shipment haveing locator filed
                output = VIS.VCreateFromShipment.prototype.saveMInOut();
            }
            else if ($self.cmbBankAccount != null) {
                output = VIS.VCreateFromStatement.prototype.saveStatment();
                $self.middelDiv.css("height", "79%");
            }
            else {
                output = VIS.VCreateFromInvoice.prototype.saveInvoice();
            }
            $self.setBusy(false);
            if (output) {
                $self.$root.dialog('close');
            }
        });

        this.cancelbtn.on("click", function () {
            $self.$root.dialog('close');
        });

        var toggal = true;
        this.selectAllButton.on("click", function () {
            //if ($self.dGrid != null) {
            //    if (toggal) {
            //        $(this.children[0]).attr('src', './Areas/VIS/Images/base/check-icon.png');
            //        w2ui.gridCreateForm.selectAll()
            //        toggal = false;
            //    }
            //    else {
            //        $(this.children[0]).attr('src', './Areas/VIS/Images/base/uncheck-icon.png');
            //        w2ui.gridCreateForm.selectNone()
            //        toggal = true;
            //    }
            //}

        });

        if (this.relatedToOrg) {
            this.relatedToOrg.getControl().on("click", function () {
                $self.setBusy(true);
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                $self.cmbShipment.getControl().prop('selectedIndex', -1);
                $self.loadOrder(C_Order_ID, false);
                $self.setBusy(false);
            });
        }

        if (this.btnRefresh) {
            this.btnRefresh.on("click", function () {
                if ($self.cmbBankAccount != null) {
                    $self.setBusy(true);
                    var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                    if (C_BankAccount_ID) {
                        VIS.VCreateFromStatement.prototype.loadBankAccount(C_BankAccount_ID);
                    }
                    $self.setBusy(false);
                }
            });
        }

        if (this.cmbOrder) {
            this.cmbOrder.getControl().change(function () {
                
                $self.setBusy(true);
                var C_Order_ID = $self.cmbOrder.getControl().find('option:selected').val();
                //  set Invoice and Shipment to Null
                $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                $self.cmbShipment.getControl().prop('selectedIndex', -1);
                if ($self.locatorField != null) {
                    //for shipment haveing locator filed
                    $self.loadOrder(C_Order_ID, false);
                }
                else {
                    //for invoice
                    $self.loadOrder(C_Order_ID, true);
                }
                $self.setBusy(false);
            });
        }

        if (this.cmbBankAccount) {
            this.cmbBankAccount.getControl().change(function () {
                $self.setBusy(true);
                var C_BankAccount_ID = $self.cmbBankAccount.getControl().find('option:selected').val();
                if (C_BankAccount_ID) {
                    VIS.VCreateFromStatement.prototype.loadBankAccount(C_BankAccount_ID);
                }
                $self.setBusy(false);
            });
        }

        if (this.cmbInvoice) {
            this.cmbInvoice.getControl().change(function () {
                $self.setBusy(true);
                var C_Invoice_ID = $self.cmbInvoice.getControl().find('option:selected').val();
                //  set Order and Shipment to Null
                $self.cmbOrder.getControl().prop('selectedIndex', -1);
                $self.cmbShipment.getControl().prop('selectedIndex', -1);
                VIS.VCreateFromShipment.prototype.loadInvoice(C_Invoice_ID);
                $self.setBusy(false);
            });
        }

        if (this.cmbShipment) {
            this.cmbShipment.getControl().change(function () {
                $self.setBusy(true);
                var M_InOut_ID = $self.cmbShipment.getControl().find('option:selected').val();
                //  set Order and Shipment to Null
                $self.cmbOrder.getControl().prop('selectedIndex', -1);
                $self.cmbInvoice.getControl().prop('selectedIndex', -1);
                VIS.VCreateFromInvoice.prototype.loadShipment(M_InOut_ID);
                $self.setBusy(false);
            });
        }
    };

    VCreateFrom.prototype.isInitOK = function () {
        return this.initOK;
    }

    VCreateFrom.prototype.loadOrder = function (C_Order_ID, forInvoice) {
        //_order = new MOrder(Env.getCtx(), C_Order_ID, null);      //  save
        
        var data = null;
        data = this.getOrderData(VIS.Env.getCtx(), C_Order_ID, forInvoice);
        this.loadGrid(data);
    }

    VCreateFrom.prototype.getOrderData = function (ctx, C_Order_ID, forInvoice) {
        /**
         *  Selected        - 0
         *  Qty             - 1
         *  C_UOM_ID        - 2
         *  M_Product_ID    - 3
         *  OrderLine       - 4
         *  ShipmentLine    - 5
         *  InvoiceLine     - 6
         */

        var data = [];
        var sql = "";
        
        // Enable this check
        // if (window.DTD001) {
        if ($self.relatedToOrg.getValue()) {
            sql = ("SELECT "
              + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
              + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
              + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
              + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
              + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
              + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
              + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
              + " ins.description , "
              + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
              + " FROM C_OrderLine l"
               + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

            sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
            sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

            if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
                sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
            }
            else {
                sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
            }

            sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");

            sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID + " AND l.DTD001_Org_ID = " + $self.mTab.getValue($self.windowNo, "AD_Org_ID")
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                    + "l.M_Product_ID,COALESCE(p.Name,c.Name),l.M_AttributeSetInstance_ID , l.Line,l.C_OrderLine_ID, ins.description  "
                + "ORDER BY l.Line");
        }
        else {
            sql = ("SELECT "
               + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "					//	1               
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QUANTITY,"	//	2
               + "round((l.QtyOrdered-SUM(COALESCE(m.Qty,0))) * "
               + "(CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END ),2) as QTYENTER,"	//	added by bharat
               + " l.C_UOM_ID  as C_UOM_ID  ,COALESCE(uom.UOMSymbol,uom.Name) as UOM,"			//	3..4
               + " COALESCE(l.M_Product_ID,0) as M_PRODUCT_ID ,COALESCE(p.Name,c.Name) as PRODUCT,"	//	5..6
               + " l.M_AttributeSetInstance_ID AS M_ATTRIBUTESETINSTANCE_ID ,"
               + " ins.description , "
               + " l.C_OrderLine_ID as C_ORDERLINE_ID,l.Line  as LINE,'false' as SELECTROW  "								//	7..8
               + " FROM C_OrderLine l"
                + " LEFT OUTER JOIN M_MatchPO m ON (l.C_OrderLine_ID=m.C_OrderLine_ID AND ");

            sql = sql.concat(forInvoice ? "m.C_InvoiceLine_ID" : "m.M_InOutLine_ID");
            sql = sql.concat(" IS NOT NULL)").concat(" LEFT OUTER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID)" + " LEFT OUTER JOIN C_Charge c ON (l.C_Charge_ID=c.C_Charge_ID)");

            if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
                sql = sql.concat(" LEFT OUTER JOIN C_UOM uom ON (l.C_UOM_ID=uom.C_UOM_ID)");
            }
            else {
                sql = sql.concat(" LEFT OUTER JOIN C_UOM_Trl uom ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("')");
            }

            sql = sql.concat(" LEFT OUTER JOIN M_AttributeSetInstance ins ON (ins.M_AttributeSetInstance_ID =l.M_AttributeSetInstance_ID) ");

            sql = sql.concat(" WHERE l.C_Order_ID=" + C_Order_ID			//	#1
                + " GROUP BY l.QtyOrdered,CASE WHEN l.QtyOrdered=0 THEN 0 ELSE l.QtyEntered/l.QtyOrdered END, "
                + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name), "
                    + "l.M_Product_ID,COALESCE(p.Name,c.Name), l.M_AttributeSetInstance_ID, l.Line,l.C_OrderLine_ID, ins.description  "
                + "ORDER BY l.Line");
        }

        var sqlNew = "SELECT * FROM (" + sql.toString() + ") WHERE QUANTITY > 0";

        var dr = null;
        try {
            dr = VIS.DB.executeReader(sqlNew.toString(), null, null);
            console.log(sqlNew.toString());
            var count = 1;
            while (dr.read()) {
                var line = {};
                //line.push(false);       //  0-Selection

                ////var qtyOrdered = dr.getDecimal(0);
                ////var multiplier = dr.getDecimal(1);
                ////var qtyEntered = qtyOrdered * multiplier;
                //line['Quantity'] = qtyEntered;  //  1-Qty
                //line['C_UOM_ID'] = dr.getString(3);    //  2-UOM
                //line['M_Product_ID'] = dr.getString(5);      //  3-Product
                //line['C_Order_ID'] = dr.getString(7);;      //  4-OrderLine
                //line['M_InOut_ID'] = null;        //  5-Ship
                //line['C_Invoice_ID'] = null;        //  6-Invoice
                //line['C_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
                //line['M_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
                //line['C_Order_ID_K'] = dr.getInt(6);;      //  4-OrderLine -Key
                //line['M_InOut_ID_K'] = null;        //  5-Ship -Key
                //line['C_Invoice_ID_K'] = null;        //  6-Invoice -Key

                line['Quantity'] = dr.getString("quantity");  //  1-Qty
                line['QuantityEntered'] = dr.getString("qtyenter");  //  2-Qty
                line['C_UOM_ID'] = dr.getString("uom");    //  3-UOM
                line['M_Product_ID'] = dr.getString("product");    //  4-Product
                line['M_AttributeSetInstance_ID'] = dr.getString("description");        //  5-Ship -Key
                line['C_Order_ID'] = dr.getString("line");      //  4-OrderLine
                line['M_InOut_ID'] = null;        //  5-Ship
                line['C_Invoice_ID'] = null;        //  6-Invoice
                // line['Att'] = dr.getString("description");
                line['C_UOM_ID_K'] = dr.getString("c_uom_id");    //  2-UOM -Key
                line['M_Product_ID_K'] = dr.getString("m_product_id");      //  3-Product -Key
                line['M_AttributeSetInstance_ID_K'] = dr.getString("m_attributesetinstance_id");        //  5-Ship -Key
                line['C_Order_ID_K'] = dr.getString("c_orderline_id");      //  4-OrderLine -Key
                line['M_InOut_ID_K'] = null;        //  5-Ship -Key
                line['C_Invoice_ID_K'] = null;        //  6-Invoice -Key

                line['recid'] = count;
                count++;
                data.push(line);
            }
            dr.close();
        }
        catch (e) {
            // s_log.log(Level.SEVERE, sql.toString(), e);
        }
        return data;
    }

    VCreateFrom.prototype.loadGrid = function (data) {
        if (this.dGrid != null) {
            this.dGrid.destroy();
            this.dGrid = null;
        }
        
        

        if (this.arrListColumns.length == 0) {
            // this.arrListColumns.push({ field: "Select", caption: VIS.Msg.getMsg("Select"), sortable: true, size: '50px', hidden: false });
            this.arrListColumns.push({ field: "Quantity", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Quantity"), sortable: false, size: '150px', render: 'number:1', hidden: false });
            this.arrListColumns.push({ field: "QuantityEntered", caption: VIS.Msg.getMsg("QtyEntered"), editable: { type: 'float' }, render: 'number:1', sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "C_UOM_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_UOM_ID"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "M_Product_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_Product_ID"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({
                field: "M_AttributeSetInstance_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_AttributeSetInstance_ID"), sortable: false, size: '150px', hidden: false,
                render: function () {
                    return '<div><input type=text readonly="readonly" style= "width:80%; border:none" ></input><img src="' + VIS.Application.contextUrl + 'Areas/VIS/Images/base/MultiX16.png" alt="Attribute Set Instance" title="Attribute Set Instance" style="opacity:1;float:right;"></div>';
                }
            });
            this.arrListColumns.push({ field: "C_Order_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "C_OrderLine_ID"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "M_InOut_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_InOut_ID"), sortable: false, size: '150px', hidden: false });
            this.arrListColumns.push({ field: "C_Invoice_ID", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "C_Invoice_ID"), sortable: false, size: '150px', hidden: false });
            //this.arrListColumns.push({ field: "Att", caption: "Att", "type": "button", "value": "Att", sortable: true, size: '150px', hidden: false }),
            // Hidden -- > true
            this.arrListColumns.push({ field: "C_UOM_ID_K", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_UOM_ID"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "M_Product_ID_K", caption: VIS.Msg.translate(VIS.Env.getCtx(), "M_Product_ID"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "C_Order_ID_K", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "C_Order_ID"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "M_InOut_ID_K", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "M_InOut_ID"), sortable: false, size: '150px', hidden: true });
            this.arrListColumns.push({ field: "C_Invoice_ID_K", caption: VIS.Msg.getElement(VIS.Env.getCtx(), "C_Invoice_ID"), sortable: false, size: '150px', hidden: true });
        }

        w2utils.encodeTags(data);

        this.dGrid = $(this.middelDiv).w2grid({
            name: 'gridCreateForm',
            recordHeight: 30,
            show: { selectColumn: true },
            multiSelect: true,
            columns: this.arrListColumns,
            records: data
        });

        if (this.dGrid.records.length > 0) {
            for (var i = 0; i < this.dGrid.records.length; i++) {
                $("#grid_" + $self.dGrid.name + "_rec_" + (i + 1)).find("input[type=text]").val(data[i].M_AttributeSetInstance_ID);
            }
        }

        this.dGrid.on("change", function (e) {
            if ($self.dGrid.columns[e.column].field == "QuantityEntered") {
                $self.dGrid.records[e.index]["QuantityEntered"] = e.value_new;
            }
        });

        this.dGrid.on("click", function (e) {
            e.preventDefault;
            if ($self.dGrid.columns[e.column].field == "M_AttributeSetInstance_ID") {
                var AD_Column_ID = 0;
                var productWindow = AD_Column_ID == 8418;		//	HARDCODED
                var M_Locator_ID = VIS.context.getContextAsInt($self.windowNo, "M_Locator_ID");
                var C_BPartner_ID = VIS.context.getContextAsInt($self.windowNo, "C_BPartner_ID");
                var obj = new VIS.PAttributesForm(VIS.Utility.Util.getValueOfInt($self.dGrid.records[e.recid - 1].M_AttributeSetInstance_ID_K), VIS.Utility.Util.getValueOfInt($self.dGrid.records[e.recid - 1].M_Product_ID_K), M_Locator_ID, C_BPartner_ID, productWindow, AD_Column_ID, $self.windowNo);
                if (obj.hasAttribute) {
                    obj.showDialog();
                }
                obj.onClose = function (mAttributeSetInstanceId, name, mLocatorId) {
                    $("#grid_" + $self.dGrid.name + "_rec_" + (e.recid)).find("input[type=text]").val(name);
                    $self.dGrid.records[e.recid-1].M_AttributeSetInstance_ID_K = mAttributeSetInstanceId;
                    $self.dGrid.records[e.recid-1].M_AttributeSetInstance_ID = name;
                };
            }
        });

    }

    //Load form into VIS
    VIS.VCreateFrom = VCreateFrom;

})(VIS, jQuery);