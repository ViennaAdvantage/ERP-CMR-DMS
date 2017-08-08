
; (function (VIS, $) {

    //form declaretion
    function VCreateFromInvoice(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var selfChild = this;
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;
        var windowNo = tab.getWindowNo();

        function dynInit() {
            //DynInit
            baseObj.title = VIS.Msg.getElement(VIS.Env.getCtx(), "C_Invoice_ID", false) + " .. " + VIS.Msg.translate(VIS.Env.getCtx(), "CreateFrom");

            //baseObj.lblInvoice.visible = false;
            //baseObj.cmbInvoice.visible = false;
            //baseObj.lblLocator.visible = false;
            //baseObj.locatorField.visible = false;

            if (baseObj.lblInvoice != null)
                baseObj.lblInvoice.getControl().css('display', 'none');
            if (baseObj.cmbInvoice != null)
                baseObj.cmbInvoice.getControl().css('display', 'none');
            if (baseObj.lblLocator != null)
                baseObj.lblLocator.getControl().css('display', 'none');
            if (baseObj.locatorField != null)
                baseObj.locatorField.getControl().css('display', 'none');

            var C_BPartner_ID = baseObj.initBPartner(true);
            initBPDetails(C_BPartner_ID);
            return true;
        }

        function getShipments(ctx, C_BPartner_ID) {
            var pairs = [];

            //	Display
            var display = ("s.DocumentNo||' - '||")
                .concat(VIS.DB.to_char("s.MovementDate", VIS.DisplayType.Date, VIS.Env.getAD_Language(VIS.Env.getCtx())));

            var sql = ("SELECT s.M_InOut_ID,").concat(display)
                .concat(" FROM M_InOut s "
                + "WHERE s.C_BPartner_ID=" + C_BPartner_ID + " AND s.IsSOTrx='N' AND s.DocStatus IN ('CL','CO')"
                + " AND s.M_InOut_ID IN "
                    + "(SELECT sl.M_InOut_ID FROM M_InOutLine sl"
                    + " LEFT OUTER JOIN M_MatchInv mi ON (sl.M_InOutLine_ID=mi.M_InOutLine_ID) "
                    + "GROUP BY sl.M_InOut_ID,mi.M_InOutLine_ID,sl.MovementQty "
                    + "HAVING (sl.MovementQty<>SUM(mi.Qty) AND mi.M_InOutLine_ID IS NOT NULL)"
                    + " OR mi.M_InOutLine_ID IS NULL) "
                + "ORDER BY s.MovementDate");

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
                //s_log.log(Level.SEVERE, sql.toString(), e);
            }
            return pairs;
        }

        function initBPDetails(C_BPartner_ID) {
            baseObj.cmbShipment.getControl().html("");
            var shipments = getShipments(VIS.Env.getCtx(), C_BPartner_ID);

            for (var i = 0; i < shipments.length; i++) {
                if (i == 0) {
                    baseObj.cmbShipment.getControl().append(" <option value=0> </option>");
                }
                baseObj.cmbShipment.getControl().append(" <option value=" + shipments[i].ID + ">" + shipments[i].value + "</option>");
            };
            baseObj.cmbShipment.getControl().prop('selectedIndex', 0);
        }

        this.disposeComponent = function () {
            baseObj = null;
            selfChild = null;
            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFromInvoice, VIS.VCreateFrom);//Inherit from VCreateFrom

    VCreateFromInvoice.prototype.saveInvoice = function () {
        if (this.$super.dGrid == null) {
            return false;
        }

        var model = {};//this.$super.dGrid.records;
        var selectedItems = this.$super.dGrid.getSelection();

        if (selectedItems == null) {
            return;
        }
        if (selectedItems.length <= 0) {
            return;
        }
        var splitValue = selectedItems.toString().split(',');
        for (var i = 0; i < splitValue.length; i++) {
            model[i] = (this.$super.dGrid.get(splitValue[i]));
        }



        //	Get Shipment
        var C_Invoice_ID = this.$super.mTab.getValue("C_Invoice_ID");
        var C_Order_ID = this.$super.cmbOrder.getControl().find('option:selected').val();
        var M_InOut_ID = this.$super.cmbShipment.getControl().find('option:selected').val();
       
        return this.saveData(model, selectedItems, C_Order_ID, M_InOut_ID, C_Invoice_ID);
    }

    VCreateFromInvoice.prototype.loadShipment = function (M_InOut_ID) {
        var data = this.getShipmentData(VIS.Env.getCtx(), M_InOut_ID);
        this.$super.loadGrid(data);
    }

    VCreateFromInvoice.prototype.getShipmentData = function (ctx, M_InOut_ID) {

        var data = [];
        var sql = ("SELECT " // QtyEntered
                + "l.MovementQty-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.MovementQty,6),"
                + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name)," // 3..4
                + " l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line," // 5..8
                + " l.C_OrderLine_ID "); // 9

        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {
            sql = sql.concat("FROM C_UOM uom ").concat("INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ");
        }
        else {
            sql = sql.concat("FROM C_UOM_Trl uom ").concat("INNER JOIN M_InOutLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='").concat(VIS.Env.getAD_Language(ctx)).concat("') ");
        }
        sql = sql.concat("INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) ").concat(
                "LEFT OUTER JOIN M_MatchInv mi ON (l.M_InOutLine_ID=mi.M_InOutLine_ID) ").concat("WHERE l.M_InOut_ID=" + M_InOut_ID) // #1
                .concat(
                        "GROUP BY l.MovementQty, round(l.QtyEntered/l.MovementQty,6)," + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                                + "l.M_Product_ID,p.Name, l.M_InOutLine_ID,l.Line,l.C_OrderLine_ID ").concat("ORDER BY l.Line");

        try {

            dr = VIS.DB.executeReader(sql.toString(), null, null);
            var count = 1;
            while (dr.read()) {
                var line = {};

                //line.Add(false);           //  0-Selection
                var qtyMovement = dr.getDecimal(0);
                var multiplier = dr.getDecimal(1);
                var qtyEntered = qtyMovement * multiplier;

                line['Quantity'] = qtyEntered;          //  1-Qty
                line['C_UOM_ID'] = dr.getString(3);     //  2-UOM
                line['M_Product_ID'] = dr.getString(5); //  3-Product
                line['C_Order_ID'] = ".";               //  4-Order
                line['M_InOut_ID'] = dr.getString(7);   //  5-Ship
                line['C_Invoice_ID'] = null;            //  6-Invoice

                line['C_UOM_ID_K'] = dr.getString(2);   //  2-UOM -Key
                line['M_Product_ID_K'] = dr.getInt(4);  //  3-Product -Key
                line['C_Order_ID_K'] = dr.getInt(8);;   //  4-OrderLine -Key
                line['M_InOut_ID_K'] = dr.getInt(6);    //  5-Ship -Key
                line['C_Invoice_ID_K'] = null;          //  6-Invoice -Key

                line['recid'] = count;
                count++;
                data.push(line);
            }
            dr.close();
        }
        catch (e) {
            //s_log.log( Level.SEVERE, sql.toString(), e );
        }
        return data;
    }

    VCreateFromInvoice.prototype.saveData = function (model, selectedItems, C_Order_ID, M_inout_id, C_Invoice_ID) {
        var obj = this;
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveInvoice",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_Order_ID: C_Order_ID,
                C_Invoice_ID: C_Invoice_ID,
                M_inout_id: M_inout_id
            },
            success: function (data) {
                returnValue = data.result;
                if (returnValue) {
                    obj.dispose();
                    obj.$super.$root.dialog('close');
                    return;
                }
                alert(returnValue);
            }
        });

    }

    //dispose call
    VCreateFromInvoice.prototype.dispose = function () {
        if (this.disposeComponent != null)
            this.disposeComponent();
    };

    //Load form into VIS
    VIS.VCreateFromInvoice = VCreateFromInvoice;

})(VIS, jQuery);

