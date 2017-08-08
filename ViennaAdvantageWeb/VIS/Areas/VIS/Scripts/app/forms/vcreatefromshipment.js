
; (function (VIS, $) {

    //form declaretion
    function VCreateFromShipment(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var selfChild = this;
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;

        function dynInit() {
            //DynInit
            baseObj.title = VIS.Msg.getElement(VIS.Env.getCtx(), "M_InOut_ID", false) + " .. " + VIS.Msg.translate(VIS.Env.getCtx(), "CreateFrom");

            // baseObj.lblShipment.visible = false;
            // baseObj.cmbShipment.visible = false;

            if (baseObj.lblShipment != null)
                baseObj.lblShipment.getControl().css('display', 'none');
            if (baseObj.cmbShipment != null)
                baseObj.cmbShipment.getControl().css('display', 'none');

            var AD_Column_ID = 3537;            //  M_InOut.M_Locator_ID
            var lookup = new VIS.MLocatorLookup(VIS.Env.getCtx(), baseObj.windowNo);
            baseObj.locatorField = new VIS.Controls.VLocator("M_Locator_ID", true, false, true, VIS.DisplayType.Locator, lookup);
            var C_BPartner_ID = baseObj.initBPartner(false);
            initBPDetails(C_BPartner_ID);
        }

        function getInvoices(ctx, C_BPartner_ID, isReturnTrx) {
            var pairs = [];

            var display = ("i.DocumentNo||' - '||").concat(
                    VIS.DB.to_char("DateInvoiced", VIS.DisplayType.Date, VIS.Env.getAD_Language(ctx))).concat("|| ' - ' ||")
                    .concat(VIS.DB.to_char("GrandTotal", VIS.DisplayType.Amount, VIS.Env.getAD_Language(ctx)));

            var sql = ("SELECT i.C_Invoice_ID,").concat(display).concat(
                    " FROM C_Invoice i INNER JOIN C_DocType d ON (i.C_DocType_ID = d.C_DocType_ID) "
                            + "WHERE i.C_BPartner_ID=" + C_BPartner_ID + " AND i.IsSOTrx='N' "
                            + "AND d.IsReturnTrx='" + (isReturnTrx ? "Y" : "N") + "' "
                            + "AND i.DocStatus IN ('CL','CO')"
                            + " AND i.C_Invoice_ID IN " + "(SELECT il.C_Invoice_ID FROM C_InvoiceLine il"
                            + " LEFT OUTER JOIN M_MatchInv mi ON (il.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) "
                            + "GROUP BY il.C_Invoice_ID,mi.C_InvoiceLine_ID,il.QtyInvoiced "
                            + "HAVING (il.QtyInvoiced<>SUM(mi.Qty) AND mi.C_InvoiceLine_ID IS NOT NULL)"
                            + " OR mi.C_InvoiceLine_ID IS NULL) " + "ORDER BY i.DateInvoiced");

            var dr = null;
            try {
                dr = VIS.DB.executeReader(sql.toString(), null, null);
                while (dr.read()) {
                    key = VIS.Utility.Util.getValueOfInt(dr.getString(0));
                    value = dr.getString(1);
                    pairs.push({ ID: key, value: value });
                }
                dr.close();
            }
            catch (e) {
                dr.close();
            }
            return pairs;
        }

        function initBPDetails(C_BPartner_ID) {
            baseObj.cmbInvoice.getControl().html("")
            var isReturnTrx = "Y".equals(VIS.Env.getCtx().getWindowContext(baseObj.windowNo, "IsReturnTrx"));
            var invoices = getInvoices(VIS.Env.getCtx(), C_BPartner_ID, isReturnTrx);
            for (var i = 0; i < invoices.length; i++) {
                if (i == 0) {
                    baseObj.cmbInvoice.getControl().append(" <option value=0> </option>");
                }
                baseObj.cmbInvoice.getControl().append(" <option value=" + invoices[i].ID + ">" + invoices[i].value + "</option>");
            };
            baseObj.cmbInvoice.getControl().prop('selectedIndex', 0);
        }

        this.disposeComponent = function () {
            baseObj = null;
            selfChild = null;
            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFromShipment, VIS.VCreateFrom);//Inherit from VCreateFrom

    VCreateFromShipment.prototype.saveMInOut = function () {
        debugger;
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

        var loc = this.$super.locatorField.getValue();

        if (loc == null || loc == 0) {
            alert("Locator Field is Mendatory");
            return false;
        }
        if (model.length == 0) {
            return false;
        }

        var M_Locator_ID = loc;
        //	Get Shipment
        var M_InOut_ID = this.$super.mTab.getValue("M_InOut_ID");
        var C_Invoice_ID = this.$super.cmbInvoice.getControl().find('option:selected').val();
        var C_Order_ID = this.$super.cmbOrder.getControl().find('option:selected').val();

        return this.saveData(model, selectedItems, C_Order_ID, C_Invoice_ID, M_Locator_ID, M_InOut_ID);
    }

    VCreateFromShipment.prototype.loadInvoice = function (C_Invoice_ID) {
        var data = this.getInvoiceData(VIS.Env.getCtx(), C_Invoice_ID);
        this.$super.loadGrid(data);
    }

    VCreateFromShipment.prototype.getInvoiceData = function (ctx, C_Invoice_ID) {
        var data = [];
        var sql = ("SELECT "	//	Entered UOM
            + "l.QtyInvoiced-SUM(NVL(mi.Qty,0)),round(l.QtyEntered/l.QtyInvoiced,6),"
            + " l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"			//  3..4
            + " l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,"      //  5..8
            + " l.C_OrderLine_ID ");                   					//  9

        if (VIS.Env.isBaseLanguage(ctx, "C_UOM")) {

            sql = sql.concat("FROM C_UOM uom ").concat("INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID) ");
        }
        else {
            sql = sql.concat("FROM C_UOM_Trl uom ")
               .concat("INNER JOIN C_InvoiceLine l ON (l.C_UOM_ID=uom.C_UOM_ID AND uom.AD_Language='")
               .concat(VIS.Env.getAD_Language(ctx)).concat("') ");
        }
        sql = sql.concat("INNER JOIN M_Product p ON (l.M_Product_ID=p.M_Product_ID) ")
           .concat("LEFT OUTER JOIN M_MatchInv mi ON (l.C_InvoiceLine_ID=mi.C_InvoiceLine_ID) ")
           .concat("WHERE l.C_Invoice_ID=" + C_Invoice_ID 									//  #1
            + "GROUP BY l.QtyInvoiced, round(l.QtyEntered/l.QtyInvoiced,6),"
            + "l.C_UOM_ID,COALESCE(uom.UOMSymbol,uom.Name),"
                + "l.M_Product_ID,p.Name, l.C_InvoiceLine_ID,l.Line,l.C_OrderLine_ID "
            + "ORDER BY l.Line");

        var dr = null;
        try {
            dr = VIS.DB.executeReader(sql.toString(), null, null);
            var count = 1;
            while (dr.read()) {
                var line = {};

                //line.Add(false);           //  0-Selection
                var qtyInvoiced = dr.getDecimal(0);
                var multiplier = dr.getDecimal(1);
                var qtyEntered = qtyInvoiced * multiplier;

                line['Quantity'] = qtyEntered;          //  1-Qty
                line['C_UOM_ID'] = dr.getString(3);     //  2-UOM
                line['M_Product_ID'] = dr.getString(5); //  3-Product
                line['C_Order_ID'] = ".";      //  4-Order
                line['M_InOut_ID'] = null;        //  5-Ship
                line['C_Invoice_ID'] = dr.getString(7);        //  6-Invoice

                line['C_UOM_ID_K'] = dr.getString(2);    //  2-UOM -Key
                line['M_Product_ID_K'] = dr.getInt(4);      //  3-Product -Key
                line['C_Order_ID_K'] = dr.getInt(8);;      //  4-OrderLine -Key
                line['M_InOut_ID_K'] = null;        //  5-Ship -Key
                line['C_Invoice_ID_K'] = dr.getInt(6);        //  6-Invoice -Key

                line['recid'] = count;
                count++;
                data.push(line);
            }
            dr.close();
        }
        catch (e) {
            //s_log.log(Level.SEVERE, sql.toString(), e);
        }
        return data;
    }

    VCreateFromShipment.prototype.saveData = function (model, selectedItems, C_Order_ID, C_Invoice_ID, m_locator_id, M_inout_id) {
        var obj = this;
        console.log(m_locator_id);
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveShipment",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_Order_ID: C_Order_ID,
                C_Invoice_ID: C_Invoice_ID,
                M_Locator_ID: m_locator_id,
                M_InOut_ID: M_inout_id
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
    VCreateFromShipment.prototype.dispose = function () {
        if (this.disposeComponent != null)
            this.disposeComponent();
    };



    //Load form into VIS
    VIS.VCreateFromShipment = VCreateFromShipment;

})(VIS, jQuery);

