
; (function (VIS, $) {

    //form declaretion
    function VCreateFromStatement(tab) {

        var baseObj = this.$super;
        baseObj.constructor(tab);
        var localObj = this;
        var windowNo = tab.getWindowNo();
        dynInit();
        baseObj.jbInit();
        baseObj.initOK = true;

        baseObj.middelDiv.css("height", "79%");


        function dynInit() {
            if (baseObj.mTab.getValue("C_BankStatement_ID") == null) {
                VIS.ADialog.error(VIS.Msg.getMsg(VIS.Env.getCtx(), "SaveErrorRowNotFound", true));
                return false;
            }

            baseObj.title = VIS.Msg.getElement(VIS.Env.getCtx(), "C_BankStatement_ID", false) + " .. " + VIS.Msg.translate(VIS.Env.getCtx(), "CreateFrom");
            var AD_Column_ID = 4917;
            var lookup = VIS.MLookupFactory.getMLookUp(VIS.Env.getCtx(), windowNo, AD_Column_ID, VIS.DisplayType.TableDir);
            baseObj.cmbBankAccount = new VIS.Controls.VComboBox("C_BankAccount_ID", true, false, true, lookup, 150, VIS.DisplayType.TableDir, 0);
            baseObj.cmbBankAccount.getControl().prop('selectedIndex', 0);

            //  Set Default
            var C_BankAccount_ID = VIS.Env.getCtx().getContextAsInt(windowNo, "C_BankAccount_ID");
            baseObj.cmbBankAccount.setValue(C_BankAccount_ID);
            localObj.loadBankAccount(C_BankAccount_ID);
            if (baseObj.btnRefresh)
                baseObj.btnRefresh.css("display", "black");
            return true;
        }

        this.disposeComponent = function () {

            this.disposeComponent = null;
        };
    };

    VIS.Utility.inheritPrototype(VCreateFromStatement, VIS.VCreateFrom);//Inherit from VCreateFrom

    VCreateFromStatement.prototype.getTableFieldVOs = function () {
        var baseObj = this.$super;
        baseObj.arrListColumns = [];
        baseObj.arrListColumns.push({ field: "Date", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Date"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "C_Payment_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_Payment_ID"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "C_Currency_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_Currency_ID"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "Amount", caption: VIS.Msg.translate(VIS.Env.getCtx(), "Amount"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "ConvertedAmount", caption: VIS.Msg.translate(VIS.Env.getCtx(), "ConvertedAmount"), sortable: true, size: '150px', hidden: false });
        baseObj.arrListColumns.push({ field: "C_BPartner_ID", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_BPartner_ID"), sortable: true, size: '150px', hidden: false });

        baseObj.arrListColumns.push({ field: "C_Payment_ID_K", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_Payment_ID"), sortable: true, size: '150px', hidden: true });
        baseObj.arrListColumns.push({ field: "C_Currency_ID_K", caption: VIS.Msg.translate(VIS.Env.getCtx(), "C_Currency_ID"), sortable: true, size: '150px', hidden: true });
    }

    VCreateFromStatement.prototype.loadBankAccount = function (C_BankAccount_ID) {
        /**
         *  Selected        - 0
         *  Date            - 1
         *  C_Payment_ID    - 2
         *  C_Currenncy     - 3
         *  Amt             - 4
         */
        var baseObj = this.$super;
        //  Get StatementDate
        var ts = baseObj.mTab.getValue("StatementDate");
        var data = this.getBankAccountData(VIS.Env.getCtx(), C_BankAccount_ID, ts);
        //  Header Info
        this.getTableFieldVOs();
        setTimeout(function () {
            baseObj.loadGrid(data);
        }, 10);

    }

    VCreateFromStatement.prototype.getBankAccountData = function (ctx, C_BankAccount_ID, ts) {
        var data = [];

        var sql = "SELECT p.DateTrx,p.C_Payment_ID,p.DocumentNo, p.C_Currency_ID,c.ISO_Code, p.PayAmt,"
            + "currencyConvert(p.PayAmt,p.C_Currency_ID,ba.C_Currency_ID,@t,null,p.AD_Client_ID,p.AD_Org_ID),"   //  #1
            + " bp.Name "
            + "FROM C_BankAccount ba"
            + " INNER JOIN C_Payment_v p ON (p.C_BankAccount_ID=ba.C_BankAccount_ID)"
            + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID)"
            + " LEFT OUTER JOIN C_BPartner bp ON (p.C_BPartner_ID=bp.C_BPartner_ID) "
            + "WHERE p.Processed='Y' AND p.IsReconciled='N'"
            + " AND p.DocStatus IN ('CO','CL','RE','VO') AND p.PayAmt<>0"
            + " AND p.C_BankAccount_ID=@C_BankAccount_ID"                              	//  #2
            + " AND NOT EXISTS (SELECT * FROM C_BankStatementLine l "
            //	Voided Bank Statements have 0 StmtAmt
                + "WHERE p.C_Payment_ID=l.C_Payment_ID AND l.StmtAmt <> 0)";

        try {
            if (ts == null) {
                ts = DateTime.Today.Date;
            }

            var dr = null;
            var param = [];

            param[0] = new VIS.DB.SqlParam("@t", ts);
            param[0].setIsDate(true);
            param[1] = new VIS.DB.SqlParam("@C_BankAccount_ID", C_BankAccount_ID);

            dr = VIS.DB.executeReader(sql, param, null);
            var count = 1;
            while (dr.read()) {
                var line = {};
                line['Date'] = dr.getString(0);             //  1-DateTrx
                line['C_Payment_ID'] = dr.getString(2);     //  2-C_Payment_ID
                line['C_Currency_ID'] = dr.getString(4);    //  3-Currency
                line['Amount'] = dr.getDecimal(5);          //  4-PayAmt
                line['ConvertedAmount'] = dr.getDecimal(6); //  5-Conv Amt
                line['C_BPartner_ID'] = dr.getString(7);    //  6-BParner

                line['C_Payment_ID_K'] = dr.getString(1);   //  2-C_Payment_ID -Key
                line['C_Currency_ID_K'] = dr.getInt(3);     //  3-Currency -Key

                line['recid'] = count;
                count++;
                data.push(line);
            }
        }
        catch (e) {

        }
        return data;
    }

    VCreateFromStatement.prototype.saveStatment = function () {
        if (this.$super.dGrid == null) {
            return false;
        }

        //	Get Shipment
        var C_BankStatement_ID = this.$super.mTab.getValue("C_BankStatement_ID");

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



        return this.saveData(model, selectedItems, C_BankStatement_ID);
    }

    VCreateFromStatement.prototype.saveData = function (model, selectedItems, C_BankStatement_ID) {
        var obj = this;
        $.ajax({
            type: "POST",
            url: VIS.Application.contextUrl + "Common/SaveStatment",
            dataType: "json",
            data: {
                model: model,
                selectedItems: selectedItems,
                C_BankStatement_ID: C_BankStatement_ID
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
    VCreateFromStatement.prototype.dispose = function () {
        this.disposeComponent();
    };

    //Load form into VIS
    VIS.VCreateFromStatement = VCreateFromStatement;

})(VIS, jQuery);

