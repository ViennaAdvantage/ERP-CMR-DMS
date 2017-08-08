; (function (VIS, $) {

    function VAllocation() {
        this.frame = null;
        this.windowNo = 0;
        var ctx = VIS.Env.getCtx();
        var $root = $('<div class="vis-allocate-root">');
        var $row1 = $('<div  style="width:25%;background-color: #f8f8f8;float: left;height: 100%;position: relative;padding: 15px;">');
        var rowContiner = $('<div class="vis-allocation-rightContainer">');
        var $row2 = $('<div class="vis-allocate-paymentdiv" >');
        var $row3 = $('<div class="vis-allocate-cashdiv" >');
        var $row4 = $('<div class="vis-allocate-invoicediv" >');
        var $row5 = $('<div >');

        var $vSearchBPartner = null;
        var $cmbCurrency = $('<select class="vis-allocation-currencycmb" ></select>');
        var $divPayment = null;
        var $divCashline = null;
        var $divInvoice = null;

        var $gridPayment = null;
        var $gridCashline = null;
        var $gridInvoice = null;

        var $vchkMultiCurrency = null;
        var $vchkAllocation = null;
        var $date = null;
        var _C_BPartner_ID = 0;
        var _C_Currency_ID = 0;
        var _payment = 7;
        var date = null;
        var _open = 6;
        var _discount = 7;
        var _writeOff = 10;
        var _applied = 11;

        var $lblPaymentSum = null;
        var $lblCashSum = null;
        var $lblInvoiceSum = null;

        var _noInvoices = 0;
        var _noPayments = 0;
        var _noCashLines = 0;


        var colInvCheck = false;
        var colPayCheck = false;
        var colCashCheck = false;

        var $vtxtDifference = null;
        var $vbtnAllocate = null;
        var $vlblAllocCurrency = null;
        var $vchkAutoWriteOff = null;

        var selection = null;
        var $bsyDiv = null;

        var self = this;

        var readOnlyCash = true;
        var readOnlyPayment = false;

        var paymentTotal = 0;
        var invoiceTotal = 0;



        initialize();

        function initialize() {
            _C_Currency_ID = ctx.getContextAsInt("$C_Currency_ID");   //  default
            fillLookups();
            createRow1();
            createRow2();
            createRow3();
            createRow4();
            rowContiner.append($row2).append($row3).append($row4).append($row5);
            $root.append($row1).append(rowContiner);
            createBusyIndicator();
            eventHandling();
            $bsyDiv[0].style.visibility = "hidden";
        };

        function eventHandling() {
            $vSearchBPartner.fireValueChanged = bpValueChanged;
            $cmbCurrency.on("change", function (e) {
                vetoableChange("C_Currency_ID", $cmbCurrency.val());
                loadBPartner();
            });
            $vchkMultiCurrency.on("change", function (e) {
                vetoableChange("Date", $vchkMultiCurrency.is(':checked'));
                loadBPartner();
            });
            $vchkAllocation.on("change", function (e) {
                vchkapplocationChange();
            });
            $date.on("change", function (e) {
                vetoableChange("Date", $date.val());
                loadBPartner();
            });
            $vbtnAllocate.on("click", function (e) {
                allocate();
            });
            $vchkAutoWriteOff.on("click", function (e) {
                autoWriteOff();
                calculate();
            });
        };

        function fillLookups() {

            var data = null;
            var value = VIS.MLookupFactory.getMLookUp(ctx, self.windowNo, 3505, VIS.DisplayType.TableDir);
            data = value.getData(true, true, false, false);
            if (data != null && data != undefined && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    $cmbCurrency.append('<option value=' + data[i].Key + '>' + data[i].Name + '</option>');
                    if (data[i].Key == _C_Currency_ID) {
                        $cmbCurrency.val(_C_Currency_ID);
                    }
                }
            }

            value = VIS.MLookupFactory.getMLookUp(ctx, self.windowNo, 3499, VIS.DisplayType.Search);
            $vSearchBPartner = new VIS.Controls.VTextBoxButton("C_BPartner_ID", true, false, true, VIS.DisplayType.Search, value);
        };

        function createRow1() {

            var $divBp = $('<div class="vis-allocation-leftControls">');
            $divBp.append('<span class="vis-allocation-inputLabels">' + VIS.Msg.translate(ctx, "C_BPartner_ID") + '</span>').append($vSearchBPartner.getControl().addClass("vis-allocation-bpartner")).append($vSearchBPartner.getBtn(0).css('width', '30px').css('height', '30px').css('padding', '0px').css('border-color', '#BBBBBB'));
            var $divCu = $('<div class="vis-allocation-leftControls">');
            $divCu.append('<span class="vis-allocation-inputLabels">' + VIS.Msg.translate(ctx, "C_Currency_ID") + '</span>').append($cmbCurrency);
            $row1.append($divBp)
                .append($divCu)
                .append('<div class="vis-allocation-leftControls"><span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("Date") + '</span><input  class="vis-allocation-date"  type="date"></input>')
                .append('<div class="vis-allocation-leftControls"><input name="vchkMultiCurrency" class="vis-allocation-multicheckbox" type="checkbox"><label>' + VIS.Msg.getMsg("MultiCurrency") + '</label></div>')
                .append('<div class="vis-allocation-leftControls"><input  class="vis-allocation-cashbox"  type="checkbox"><label>' + VIS.Msg.getMsg("Cash") + '</label></div>')
                .append('<div class="vis-allocation-leftControls"><input  class="vis-allocation-autowriteoff"  type="checkbox"><label>' + VIS.Msg.getMsg("AutoWriteOff") + '</label></div>');
            $vchkMultiCurrency = $row1.find('.vis-allocation-multicheckbox');
            $vchkAllocation = $row1.find('.vis-allocation-cashbox');
            $date = $row1.find('.vis-allocation-date');
            $vchkAutoWriteOff = $row1.find('.vis-allocation-autowriteoff');

            $date.val(Globalize.format(new Date(), "yyyy-MM-dd"));


            var $resultDiv = $('<div class="vis-allocation-resultdiv">');
            $resultDiv.append('<div class="vis-allocation-leftControls"><span class="vis-allocation-inputLabels">' + VIS.Msg.getMsg("Difference") + '</span>');
            $resultDiv.append('<div style="width:20%" class="vis-allocation-leftControls"><span class="vis-allocation-lblCurrnecy"></span>');
            $resultDiv.append('<div style="width:80%"  class="vis-allocation-leftControls"><span class="vis-allocation-lbldifferenceAmt"></span>');
            //$resultDiv.append('<div class="vis-allocation-leftControls"><input  class="vis-allocation-autowriteoff"  type="checkbox"><label>' + VIS.Msg.getMsg("AutoWriteOff") + '</label></div>');
            $resultDiv.append(' <a class="vis-group-btn vis-group-create vis-group-grayBtn" style="float: right;">' + VIS.Msg.getMsg('Process') + '</a>');

            $row1.append($resultDiv);
            $vtxtDifference = $resultDiv.find('.vis-allocation-lbldifferenceAmt');
            $vbtnAllocate = $resultDiv.find('.vis-group-btn');
            $vbtnAllocate.css({ "pointer-events": "none", "opacity": "0.5" });
            $vlblAllocCurrency = $resultDiv.find('.vis-allocation-lblCurrnecy');
        };

        function createRow2() {
            $row2.append('<p>' + VIS.Msg.translate(ctx, "C_Payment_ID") + '</p>').append('<div class="vis-allocation-payment-grid"></div>').append('<p class="vis-allocate-paymentSum">0-Sum 0.00</p>');
            $divPayment = $row2.find('.vis-allocation-payment-grid');
            $lblPaymentSum = $row2.find('.vis-allocate-paymentSum');
        };

        function createRow3() {
            $row3.append('<p>' + VIS.Msg.translate(ctx, "C_CashLine_ID") + '</p>').append('<div  class="vis-allocation-cashLine-grid"></div>').append('<p class="vis-allocate-cashSum">0-Sum 0.00</p>');
            $divCashline = $row3.find('.vis-allocation-cashLine-grid');
            $lblCashSum = $row3.find('.vis-allocate-cashSum');
        };

        function createRow4() {
            $row4.append('<p>' + VIS.Msg.translate(ctx, "C_Invoice_ID") + '</p>').append('<div  class="vis-allocation-invoice-grid"></div>').append('<p class="vis-allocate-invoiceSum">0-Sum 0.00</p>');
            $divInvoice = $row4.find('.vis-allocation-invoice-grid');
            $lblInvoiceSum = $row4.find('.vis-allocate-invoiceSum');
        };


        /*
          Create busyIndicator
      */
        function createBusyIndicator() {
            $bsyDiv = $("<div class='vis-apanel-busy' style='height:96%; width:98%;'></div>");
            $bsyDiv[0].style.visibility = "hidden";
            $root.append($bsyDiv);
        }

        /// <summary>
        ///  Load Business Partner Info
        ///  - Payments
        ///  - Invoices
        /// </summary>
        function loadBPartner() {
            if (_C_BPartner_ID == 0 || _C_Currency_ID == 0) {
                //  SetBusy(false);
                return;
            }
            $bsyDiv[0].style.visibility = "visible";
            var getDate = null;
            date = $date.val();
            if (date != null && date != undefined && date != "") {
                getDate = VIS.DB.to_date(date);
            }
            var chk = $vchkMultiCurrency.is(':checked');
            //vdgvPayment.ItemsSource = null;
            //vdgvCashLine.ItemsSource = null;
            //vdgvInvoice.ItemsSource = null;
            //SetBusy(true);
            //txtBusy.Text = Msg.GetMsg("Processing");

            var AD_Role_ID = ctx.getAD_Role_ID();
            var AD_User_ID = ctx.getAD_User_ID();
            var role = VIS.MRole.getDefault();

            //  log.Config("BPartner=" + _C_BPartner_ID + ", Cur=" + _C_Currency_ID);
            //  Need to have both values


            //	Async BPartner Test
            var key = _C_BPartner_ID;
            /********************************
         *  Load unallocated Payments
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            var sql = "SELECT 'false' as SELECTROW, p.DateTrx as DATE1,p.DocumentNo As DOCUMENTNO,p.C_Payment_ID As CPAYMENTID,"  //  1..3
               + "c.ISO_Code as ISOCODE,p.PayAmt AS PAYMENT,"                            //  4..5
               + "currencyConvert(p.PayAmt ,p.C_Currency_ID ," + _C_Currency_ID + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID ) AS CONVERTEDAMOUNT,"//  6   #1
               + "currencyConvert(ALLOCPAYMENTAVAILABLE(C_Payment_ID) ,p.C_Currency_ID ," + _C_Currency_ID + ",p.DateTrx ,p.C_ConversionType_ID ,p.AD_Client_ID ,p.AD_Org_ID) as OPENAMT,"  //  7   #2
               + "p.MultiplierAP as MULTIPLIERAP, "
               + "0 as APPLIEDAMT "
               + "FROM C_Payment_v p"		//	Corrected for AP/AR
               + " INNER JOIN C_Currency c ON (p.C_Currency_ID=c.C_Currency_ID) "
               + "WHERE "
          + "  ((p.IsAllocated ='N' and p.c_charge_id is null) "
+ " OR (p.isallocated = 'N' AND p.c_charge_id is not null and p.isprepayment = 'Y'))"
            + " AND p.Processed='Y'"
               + " AND p.C_BPartner_ID=" + _C_BPartner_ID;
            if (!chk) {
                sql += " AND p.C_Currency_ID=" + _C_Currency_ID;				//      #4
            }
            sql += " ORDER BY p.DateTrx,p.DocumentNo";

            sql = role.addAccessSQL(sql, "p", true, false);
            var dsPayment = VIS.DB.executeDataSet(sql);
            _payment = 9;
            bindPaymentGrid(dsPayment, chk);


            /********************************
         *  Load unallocated Cash lines
         *      1-TrxDate, 2-DocumentNo, (3-Currency, 4-PayAmt,)
         *      5-ConvAmt, 6-ConvOpen, 7-Allocated
         */
            var sqlCash = "SELECT 'false' as SELECTROW, cn.created as CREATED, cn.receiptno AS RECEIPTNO, cn.c_cashline_id AS CCASHLINEID,"
            + "c.ISO_Code AS ISO_CODE,cn.amount AS AMOUNT, "
             + "currencyConvert(cn.Amount ,cn.C_Currency_ID ," + _C_Currency_ID + ",cn.Created ,114 ,cn.AD_Client_ID ,cn.AD_Org_ID ) AS CONVERTEDAMOUNT,"//  6   #1cn.amount as OPENAMT,"
            + " cn.amount as OPENAMT,"
  + "cn.MultiplierAP AS MULTIPLIERAP,"
  + "0 as APPLIEDAMT "
   + " from c_cashline_new cn"
                //+ " from C_CASHLINE_VW cn"
            + " INNER join c_currency c ON (cn.C_Currency_ID=c.C_Currency_ID)"
            + " WHERE cn.IsAllocated   ='N' AND cn.Processed ='Y'"
            + " and cn.cashtype = 'B' and cn.docstatus in ('CO','CL') "
                // Commented because Against Business Partner there is no charge
                // + " AND cn.C_Charge_ID  IS Not NULL"
            + " AND cn.C_BPartner_ID=" + _C_BPartner_ID;
            if (!chk) {
                sqlCash += " AND cn.C_Currency_ID=" + _C_Currency_ID;
            }
            sqlCash += " ORDER BY cn.created,cn.receiptno";

            sqlCash = role.addAccessSQL(sqlCash, "cn", true, false);

            var dsCash = VIS.DB.executeDataSet(sqlCash);
            bindCashline(dsCash, chk);
            _payment = 9;


            var sqlInvoice = "SELECT 'false' as SELECTROW , i.DateInvoiced  as DATE1 ,"
+ "  i.DocumentNo    AS DOCUMENTNO  ,"
+ "  i.C_Invoice_ID AS CINVOICEID,"
+ "  c.ISO_Code AS ISO_CODE    ,"
                //+ "  (i.GrandTotal  *i.MultiplierAP) AS CURRENCY    ,"
+ "  (invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP) AS CURRENCY    ,"
                //+ "currencyConvert(i.GrandTotal  *i.MultiplierAP,i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ) AS CONVERTED  ,"
+ "currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID)  *i.MultiplierAP,i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID ) AS CONVERTED  ,"
+ " currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID," + _C_Currency_ID + ",i.DateInvoiced,i.C_ConversionType_ID,i.AD_Client_ID,i.AD_Org_ID)                                         *i.MultiplierAP AS AMOUNT,"
                //+ "(currencyConvert(invoiceOpen(C_Invoice_ID,C_InvoicePaySchedule_ID),i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID)) AS AMOUNT ,"
+ "  (currencyConvert(invoiceDiscount(i.C_Invoice_ID ," + getDate + ",C_InvoicePaySchedule_ID),i.C_Currency_ID ," + _C_Currency_ID + ",i.DateInvoiced ,i.C_ConversionType_ID ,i.AD_Client_ID ,i.AD_Org_ID )*i.Multiplier*i.MultiplierAP) AS DISCOUNT ,"
+ "  i.MultiplierAP ,i.docbasetype  ,"
+ "0 as WRITEOFF ,"
+ "0 as APPLIEDAMT"
                 + " FROM C_Invoice_v i"		//  corrected for CM/Split
                 + " INNER JOIN C_Currency c ON (i.C_Currency_ID=c.C_Currency_ID) "
                 + "WHERE i.IsPaid='N' AND i.Processed='Y'"
                 + " AND i.C_BPartner_ID=" + _C_BPartner_ID;                                            //  #5
            if (!chk) {
                sqlInvoice += " AND i.C_Currency_ID=" + _C_Currency_ID;                                   //  #6
            }
            sqlInvoice += " ORDER BY i.DateInvoiced, i.DocumentNo";

            sqlInvoice = role.addAccessSQL(sqlInvoice, "i", true, false);

            var dsInvoice = VIS.DB.executeDataSet(sqlInvoice);

            bindInvoiceGrid(dsInvoice, chk);
            //vdgvInvoice.AutoSize = true;
            _open = 7;
            // _discount = chk ? 7 : 5;
            _discount = 8;
            _writeOff = 11;
            _applied = 12;



            calculate();
            $bsyDiv[0].style.visibility = "hidden";

        };

        function bindPaymentGrid(ds, chk) {
            var columns = [];
            columns.push({ field: 'selectrow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            //columns.push({
            //    field: "selectrow", caption: "Select", sortable: true, size: '50px',
            //    render: function () { return '<div><input type="checkbox" style="width:15px;height:15px"></input></div>' }
            //});
            for (var c = 0; c < ds.tables[0].columns.length; c++) {
                if (ds.tables[0].columns[c].name == "date1") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "Date"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "documentno") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "DOCUMENTNO"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "isocode" && chk == true) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "TRXCURRENCY"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "isocode" && chk == false) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "TRXCURRENCY"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "payment" && chk == true) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "payment" && chk == false) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AMOUNT"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "convertedamount") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "CONVERTEDAMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "openamt") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "OPENAMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "appliedamt") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AppliedAmount"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "cpaymentid") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "c_payment_id"), hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "multiplierap") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "multiplierap"), hidden: true });
                }

            }

            var rows = [];

            var colkeys = [];


            if (ds.tables[0].rows.length > 0) {
                colkeys = Object.keys(ds.tables[0].rows[0].cells);
            }

            for (var r = 0; r < ds.tables[0].rows.length; r++) {
                var singleRow = {};
                singleRow['recid'] = r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(ds.tables[0].rows[r].cells[colna]);
                    }
                }
                rows.push(singleRow);
            }

            if ($gridPayment != undefined && $gridPayment != null) {
                $gridPayment.destroy();
                $gridPayment = null;
            }

            $gridPayment = $divPayment.w2grid({
                name: 'openformatgrid'
              ,
                // show: { selectColumn: true },
                multiSelect: true,
                columns: columns,
                records: rows,
                onClick: function (event) {
                    paymentCellClicked(event);
                },
                onChange: function (event) {
                    paymentCellChanged(event);
                },
                onDblClick: function (event) {
                    paymentDoubleClicked(event);
                }
            });


        };

        function bindCashline(ds, chk) {
            var columns = [];
            columns.push({ field: 'selectrow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            for (var c = 0; c < ds.tables[0].columns.length; c++) {
                if (ds.tables[0].columns[c].name == "created") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "Date"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "receiptno") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "RECEIPTNO"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "iso_code" && chk == true) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("TRX"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "iso_code" && chk == false) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("TRX"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "amount" && chk == true) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "amount" && chk == false) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AMOUNT"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "convertedamount") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Converted"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "openamt") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "OPENAMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "appliedamt") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AppliedAmount"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "ccashlineid") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "ccashlineid"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "multiplierap") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "multiplierap"), hidden: true });
                }
            }

            var rows = [];

            var colkeys = [];


            if (ds.tables[0].rows.length > 0) {
                colkeys = Object.keys(ds.tables[0].rows[0].cells);
            }

            for (var r = 0; r < ds.tables[0].rows.length; r++) {
                var singleRow = {};
                singleRow['recid'] = r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(ds.tables[0].rows[r].cells[colna]);
                    }
                }
                rows.push(singleRow);
            }

            if ($gridCashline != undefined && $gridCashline != null) {
                $gridCashline.destroy();
                $gridCashline = null;
            }

            $gridCashline = $divCashline.w2grid({
                name: 'openformatgridcash'
               ,
                multiSelect: true,
                columns: columns,
                records: rows,
                onChange: function (event) {
                    cashCellChanged(event);
                },
                onClick: function (event) {
                    cashCellClicked(event);
                },
                onDblClick: function (event) {
                    cashDoubleClicked(event);
                }
            });


        };

        function bindInvoiceGrid(ds, chk) {
            var columns = [];
            columns.push({ field: 'selectrow', caption: 'check', size: '40px', editable: { type: 'checkbox' } });
            for (var c = 0; c < ds.tables[0].columns.length; c++) {
                if (ds.tables[0].columns[c].name == "date1") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "Date"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "documentno") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "DocumentNo"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "iso_code" && chk == true) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Trx"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "iso_code" && chk == false) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Trx"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "currency" && chk == true) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "currency" && chk == false) {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AMOUNT"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "converted") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.getMsg("Converted"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "amount") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "OPENAMOUNT"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "docbasetype") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "DocBaseType"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "discount") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "Discount"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "writeoff") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "WriteOff"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "appliedamt") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "AppliedAmount"), size: '150px', hidden: false });
                }
                else if (ds.tables[0].columns[c].name == "cinvoiceid") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "cinvoiceid"), size: '150px', hidden: true });
                }
                else if (ds.tables[0].columns[c].name == "multiplierap") {
                    columns.push({ field: ds.tables[0].columns[c].name, caption: VIS.Msg.translate(ctx, "multiplierap"), hidden: true });
                }
            }

            var rows = [];

            var colkeys = [];


            if (ds.tables[0].rows.length > 0) {
                colkeys = Object.keys(ds.tables[0].rows[0].cells);
            }

            for (var r = 0; r < ds.tables[0].rows.length; r++) {
                var singleRow = {};
                singleRow['recid'] = r;
                for (var c = 0; c < colkeys.length; c++) {
                    var colna = colkeys[c];
                    if (colna == "selectrow") {
                        singleRow[colna] = false;
                    }
                    else {
                        singleRow[colna] = VIS.Utility.encodeText(ds.tables[0].rows[r].cells[colna]);
                    }
                }
                rows.push(singleRow);
            }

            if ($gridInvoice != undefined && $gridInvoice != null) {
                $gridInvoice.destroy();
                $gridInvoice = null;
            }

            $gridInvoice = $divInvoice.w2grid({
                name: 'openformatgridinvoice'
                 ,
                //  show: { selectColumn: true },
                multiSelect: true,
                columns: columns,
                records: rows,
                onChange: function (event) {
                    invoiceCellChanged(event);
                },
                onClick: function (event) {
                    invoiceCellClicked(event);
                },
                onDblClick: function (event) {
                    invoiceDoubleClicked(event);
                }
            });


        };


        function paymentDoubleClicked(event) {
            if (w2ui.openformatgrid.columns[event.column].field == "appliedamt") {
                var getChanges = w2ui.openformatgrid.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].selectrow == undefined) {
                    w2ui.openformatgrid.columns[event.column].editable = false;
                    return;
                }
                w2ui.openformatgrid.columns[event.column].editable = { type: 'text' }
            }
        };

        function cashDoubleClicked(event) {
            if (w2ui.openformatgridcash.columns[event.column].field == "appliedamt") {
                var getChanges = w2ui.openformatgridcash.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].selectrow == undefined) {
                    w2ui.openformatgridcash.columns[event.column].editable = false;
                    return;
                }
                w2ui.openformatgridcash.columns[event.column].editable = { type: 'text' }
            }
        };

        function invoiceDoubleClicked(event) {
            if (w2ui.openformatgridinvoice.columns[event.column].field == "appliedamt" ||
                w2ui.openformatgridinvoice.columns[event.column].field == "writeoff" ||
                w2ui.openformatgridinvoice.columns[event.column].field == "discount") {
                var getChanges = w2ui.openformatgridinvoice.getChanges();
                if (getChanges == undefined || getChanges.length == 0) {
                    return;
                }

                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element[0].selectrow == undefined) {
                    w2ui.openformatgridinvoice.columns[event.column].editable = false;
                    return;
                }
                w2ui.openformatgridinvoice.columns[event.column].editable = { type: 'text' }
            }
        };


        function invoiceCellClicked(event) {
            var colIndex = w2ui.openformatgridinvoice.getColumn('appliedamt', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = w2ui.openformatgridinvoice.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].selectrow == undefined) {
                    w2ui.openformatgridinvoice.refreshCell(0, "appliedamt");
                    w2ui.openformatgridinvoice.refreshCell(0, "writeoff");
                    w2ui.openformatgridinvoice.refreshCell(0, "discount");
                }
                else {
                    if (element[0].selectrow == true) {

                    }
                    else {
                        w2ui.openformatgridinvoice.unselect(event.recid);
                        w2ui.openformatgridinvoice.columns[colIndex].editable = false;
                        w2ui.openformatgridinvoice.get(0).changes.appliedamt = "0";
                        w2ui.openformatgridinvoice.refreshCell(0, "appliedamt");
                        w2ui.openformatgridinvoice.get(0).changes.discount = "0";
                        w2ui.openformatgridinvoice.refreshCell(0, "discount");
                        w2ui.openformatgridinvoice.get(0).changes.writeoff = "0";
                        w2ui.openformatgridinvoice.refreshCell(0, "writeoff");
                    }

                }
                tableChanged(event.recid, event.column, true, false);
            }
        };

        function paymentCellClicked(event) {
            if (readOnlyPayment) {
                event.preventDefault();
                return;
            }
            var colIndex = w2ui.openformatgrid.getColumn('appliedamt', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = w2ui.openformatgrid.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].selectrow == undefined) {
                    w2ui.openformatgrid.refreshCell(event.recid, "appliedamt");
                }
                else {
                    if (element[0].selectrow == true) {

                    }
                    else {
                        w2ui.openformatgrid.unselect(event.recid);
                        w2ui.openformatgrid.columns[colIndex].editable = false;
                        w2ui.openformatgrid.get(event.recid).changes.appliedamt = "0";
                        w2ui.openformatgrid.refreshCell(event.recid, "appliedamt");
                    }

                }
                tableChanged(event.recid, event.column, false, false);
            }
        };

        function cashCellClicked(event) {
            if (readOnlyCash) {
                event.preventDefault();
                return;
            }
            var colIndex = w2ui.openformatgridcash.getColumn('appliedamt', true);
            if (event.column == 0 || event.column == null) {
                var getChanges = w2ui.openformatgridcash.getChanges();
                var element = $.grep(getChanges, function (ele, index) {
                    return parseInt(ele.recid) == parseInt(event.recid);
                });
                if (element == null || element == undefined || element.length == 0 || element[0].selectrow == undefined) {
                    w2ui.openformatgridcash.refreshCell(0, "appliedamt");
                }
                else {
                    if (element[0].selectrow == true) {

                    }
                    else {
                        w2ui.openformatgridcash.unselect(event.recid);
                        w2ui.openformatgridcash.columns[colIndex].editable = false;
                        w2ui.openformatgridcash.get(0).changes.appliedamt = "0";
                        w2ui.openformatgridcash.refreshCell(0, "appliedamt");
                    }
                }
                tableChanged(event.recid, event.column, true, true);
            }
        };

        function paymentCellChanged(event) {

            if (readOnlyPayment) {
                event.preventDefault();
                return;
            }
            var colIndex = w2ui.openformatgrid.getColumn('appliedamt', true);

            if (w2ui.openformatgrid.columns[colIndex].editable == undefined) {
                return;
            }
            if (w2ui.openformatgrid.getChanges(event.recid) != undefined && w2ui.openformatgrid.getChanges(event.recid).length > 0) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                w2ui.openformatgrid.get(event.recid).changes.appliedamt = event.value_new;
                w2ui.openformatgrid.refreshCell(event.recid, "appliedamt");
            }
            else {
                if (event.column > 0) {
                    w2ui.openformatgrid.set(event.recid, { appliedamt: event.value_new });
                }
            }
            if (event.column == colIndex) {
                //logic to not set greater appliedAmount then open amount
                if (parseFloat(w2ui.openformatgrid.get(event.index).openamt) > parseFloat(w2ui.openformatgrid.get(event.index).appliedamt)) {

                }
                else {
                    w2ui.openformatgrid.set(0, { "appliedamt": w2ui.openformatgrid.get(event.index).openamt });
                }
                tableChanged(event.index, event.column, false, false);
            }
        };

        function cashCellChanged(event) {
            if (readOnlyCash) {
                event.preventDefault();
                return;
            }
            var colIndex = w2ui.openformatgridcash.getColumn('appliedamt', true);

            if (w2ui.openformatgridcash.columns[colIndex].editable == undefined) {
                return;
            }
            if (w2ui.openformatgridcash.getChanges(event.recid) != undefined && w2ui.openformatgridcash.getChanges(event.recid).length > 0) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                w2ui.openformatgridcash.get(event.recid).changes.appliedamt = event.value_new;
                w2ui.openformatgridcash.refreshCell(event.recid, "appliedamt");
            }
            else {
                if (event.column > 0) {
                    w2ui.openformatgridcash.set(event.recid, { appliedamt: event.value_new });
                }
            }
            if (event.column == colIndex) {
                //logic to not set greater appliedAmount then open amount
                if (parseFloat(w2ui.openformatgridcash.get(event.index).openamt) > parseFloat(w2ui.openformatgridcash.get(event.index).appliedamt)) {

                }
                else {
                    w2ui.openformatgridcash.set(0, { "appliedamt": w2ui.openformatgridcash.get(event.index).paidamount });
                }
                tableChanged(event.index, event.column, true, true);
            }




            //var colIndex = w2ui.openformatgridcash.getColumn('appliedamt', true);

            //if (w2ui.openformatgridcash.columns[colIndex].editable == undefined) {
            //    return;
            //}

            //if (event.column == colIndex) {
            //    //logic to not set greater appliedAmount then open amount
            //    if (parseFloat(w2ui.openformatgridcash.get(event.index).openamt) > parseFloat(w2ui.openformatgridcash.get(event.index).appliedamt)) {

            //    }
            //    else {
            //        w2ui.openformatgridcash.set(0, { "appliedamt": w2ui.openformatgridcash.get(event.index).paidamount });
            //    }
            //    tableChanged(event.index, event.column, true, true);
            //}
        };

        function invoiceCellChanged(event) {

            var colIndex = w2ui.openformatgridinvoice.getColumn('appliedamt', true);
            var wcolIndex = w2ui.openformatgridinvoice.getColumn('writeoff', true);
            var dcolIndex = w2ui.openformatgridinvoice.getColumn('discount', true);

            if (w2ui.openformatgridinvoice.columns[colIndex].editable == undefined && w2ui.openformatgridinvoice.columns[wcolIndex].editable == undefined && w2ui.openformatgridinvoice.columns[dcolIndex].editable == undefined) {
                return;
            }
            if (w2ui.openformatgridinvoice.getChanges(event.recid) != undefined && w2ui.openformatgridinvoice.getChanges(event.recid).length > 0) {
                // if changes are there like  checkbox is cheked, then we have to set value in changes becoz textbox in grid show data from changes...
                if (colIndex == event.column) {
                    w2ui.openformatgridinvoice.get(event.recid).changes.appliedamt = event.value_new;
                    w2ui.openformatgridinvoice.refreshCell(event.recid, "appliedamt");
                }

                else if (wcolIndex == event.column) {
                    w2ui.openformatgridinvoice.get(event.recid).changes.writeoff = event.value_new;
                    w2ui.openformatgridinvoice.refreshCell(event.recid, "writeoff");
                }
                else if (dcolIndex == event.column) {
                    w2ui.openformatgridinvoice.get(event.recid).changes.discount = event.value_new;
                    w2ui.openformatgridinvoice.refreshCell(event.recid, "discount");
                }
            }
            else {
                if (event.column > 0) {
                    if (colIndex == event.column) {
                        w2ui.openformatgridinvoice.set(event.recid, { appliedamt: event.value_new });
                    }
                    else if (wcolIndex == event.column) {
                        w2ui.openformatgridinvoice.set(event.recid, { writeoff: event.value_new });
                    }
                    else if (dcolIndex == event.column) {
                        w2ui.openformatgridinvoice.set(event.recid, { discount: event.value_new });
                    }
                }
            }

            if (event.column == w2ui.openformatgridinvoice.getColumn('discount', true)
               || event.column == w2ui.openformatgridinvoice.getColumn('writeoff', true)
               || event.column == w2ui.openformatgridinvoice.getColumn('appliedamt', true)) {

                tableChanged(event.index, event.column, true, false);

            }
        };

        function tableChanged(rowIndex, colIndex, isInvoice, cash) {

            //var rowInvoice = w2ui.openformatgridinvoice.getSelection();
            //var rowPayment = w2ui.openformatgrid.getSelection();
            //var rowCash = w2ui.openformatgridcash.getSelection();
            //  Not a table update
            //if (!isUpdate)
            //{
            //    Calculate();
            //    return;
            //}

            //Setting defaults
            //if (_calculating)  //  Avoid recursive calls
            //    return;
            //_calculating = true;
            var row = rowIndex;
            var col = colIndex;
            if (col == null || col == undefined) {
                col = 0;
            }
            //   log.Config("Row=" + row + ", Col=" + col + ", InvoiceTable=" + isInvoice);


            //  Payments
            if (!isInvoice) {
                //TableModel payment = vdgvInvoice.getModel();
                if (col == 0) {
                    var columns = w2ui.openformatgrid.columns;
                    colPayCheck = getBoolValue(w2ui.openformatgrid.getChanges(), row);
                    var payemntCol = columns[_payment].field;
                    //  selected - set payment amount
                    var changes = w2ui.openformatgrid.get(row).changes;

                    if (colPayCheck) {
                        var amount = parseFloat(w2ui.openformatgrid.get(row)[columns[_open].field]);
                        if (payemntCol == "appliedamt") {
                            if (changes != null && changes != undefined) {
                                changes.appliedamt = amount;
                                w2ui.openformatgrid.refreshCell(row, "appliedamt");
                            }
                            else {
                                w2ui.openformatgrid.set(row, { "appliedamt": amount });
                            }
                        }
                    }
                    else    //  de-selected
                    {
                        if (payemntCol == "appliedamt") {
                            if (changes != null && changes != undefined) {
                                changes.appliedamt = 0;
                                w2ui.openformatgrid.refreshCell(row, "appliedamt");
                            }
                            else {
                                w2ui.openformatgrid.set(row, { "appliedamt": 0 });
                            }
                        }
                    }
                }
            }
                //Cash Lines
            else if (cash) {

                if (col == 0) {
                    var columns = w2ui.openformatgridcash.columns;
                    colCashCheck = getBoolValue(w2ui.openformatgridcash.getChanges(), row);
                    var payemntCol = columns[_payment].field;
                    //  selected - set payment amount
                    var changes = w2ui.openformatgridcash.get(row).changes;
                    //  selected - set payment amount
                    if (colCashCheck) {
                        var amount = parseFloat(w2ui.openformatgridcash.get(row)[columns[_open].field]);
                        if (payemntCol == "appliedamt") {
                            if (changes != null && changes != undefined) {
                                changes.appliedamt = amount;
                                w2ui.openformatgridcash.refreshCell(row, "appliedamt");
                            }
                            else {
                                w2ui.openformatgridcash.set(row, { "appliedamt": amount });
                            }
                        }
                        //************************************** Changed
                    }
                    else    //  de-selected
                    {
                        //************************************** Changed
                        if (payemntCol == "appliedamt") {
                            if (changes != null && changes != undefined) {
                                changes.appliedamt = 0;
                                w2ui.openformatgridcash.refreshCell(row, "appliedamt");
                            }
                            else {
                                w2ui.openformatgridcash.set(row, { "appliedamt": 0 });
                            }
                        }
                    }
                }
            }
                //  Invoice Selection
            else if (col == 0) {
                var columns = w2ui.openformatgridinvoice.columns;
                colInvCheck = getBoolValue(w2ui.openformatgridinvoice.getChanges(), row);

                //var payemntCol = columns[_payment].field;
                //  selected - set payment amount
                var changes = w2ui.openformatgridinvoice.get(row).changes;
                //  selected - set payment amount

                var writeOff = columns[_writeOff].field;
                var applied = columns[_applied].field;
                //  selected - set applied amount
                if (colInvCheck) {
                    var amount = parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_open].field]);
                    amount = amount - parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_discount].field]);
                    if (applied == "appliedamt") {
                        if (changes != null && changes != undefined) {
                            changes.writeoff = 0;
                            changes.appliedamt = amount;
                            w2ui.openformatgridinvoice.refreshCell(row, "writeoff");
                            w2ui.openformatgridinvoice.refreshCell(row, "appliedamt");
                        }
                        else {
                            w2ui.openformatgridinvoice.set(row, { "writeoff": 0 });
                            w2ui.openformatgridinvoice.set(row, { "appliedamt": amount });

                        }
                    }
                    //************************************** Changed
                }
                else    //  de-selected
                {
                    if (applied == "appliedamt") {
                        if (changes != null && changes != undefined) {
                            changes.writeoff = 0;
                            changes.appliedamt = amount;
                            w2ui.openformatgridinvoice.refreshCell(row, "writeoff");
                            w2ui.openformatgridinvoice.refreshCell(row, "appliedamt");
                        }
                        else {
                            //w2ui.openformatgridinvoice.set(row, { "writeoff": 0 });
                            //w2ui.openformatgridinvoice.set(row, { "appliedamt": amount });
                        }
                    }
                }
                //if (colInvCheck) {
                //    var amount = parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_open].field]);
                //    amount = amount - parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_discount].field]);


                //    w2ui.openformatgridinvoice.set(row, { writeOff: 0 });
                //    w2ui.openformatgridinvoice.set(row, { applied: amount });
                //}

            }

            //  Invoice - Try to balance entry
            if ($vchkAutoWriteOff.is(':checked')) {



                autoWriteOff();




                //var columns = w2ui.openformatgridinvoice.columns;
                ////  if applied entered, adjust writeOff
                //var writeOff = columns[_writeOff].field
                //var applied = columns[_applied].field;
                //if (col == _applied) {
                //    var openAmount = parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_open].field]);
                //    var amount = openAmount - parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_discount].field]);

                //    amount = amount - parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_applied].field]);

                //    w2ui.openformatgridinvoice.set(row, { writeOff: amount });
                //    if ((amount / openAmount) > .30) {
                //        VIS.ADialog.error("AllocationWriteOffWarn");
                //    }
                //}
                //else    //  adjust applied
                //{
                //    var amount = parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_open].field]); //  OpenAmount
                //    amount = amount - parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_discount].field]);
                //    amount = amount - parseFloat(w2ui.openformatgridinvoice.get(row)[columns[_writeOff].field]);
                //    w2ui.openformatgridinvoice.set(row, { appliedamt: amount });

                //}
            }
            calculate();
        };

        function getBoolValue(changes, row) {
            if (changes == null || changes.length == 0) {
                return false;
            }
            var element = $.grep(changes, function (ele, index) {
                return parseInt(ele.recid) == row;
            });
            if (element == null || element == undefined || element.length == 0 || element[0].selectrow == undefined) {
                return false;
            }
            else {
                return element[0].selectrow;
            }

        };

        function autoWriteOff() {

            invoiceTotal = 0;
            paymentTotal = 0;

            if ($vchkAllocation.is(':checked')) {
                var cashChanges = w2ui.openformatgridcash.getChanges();
                for (var i = 0; i < cashChanges.length; i++) {
                    if (cashChanges[i].selectrow == true) {
                        paymentTotal += parseFloat(cashChanges[i].appliedamt);
                    }
                }
            }
            else {
                var paymentChanges = w2ui.openformatgrid.getChanges();
                for (var i = 0; i < paymentChanges.length; i++) {
                    if (paymentChanges[i].selectrow == true) {
                        paymentTotal += parseFloat(paymentChanges[i].appliedamt);
                    }
                }
            }






            var invoiceChanges = w2ui.openformatgridinvoice.getChanges();

            var lastRow = null;
            var columns = w2ui.openformatgridinvoice.columns;
            for (var i = invoiceChanges.length - 1; i >= 0; i--) {
                if (invoiceChanges[i].selectrow == true) {
                    invoiceTotal += parseFloat(invoiceChanges[i].appliedamt);

                    if (lastRow == null) {
                        lastRow = w2ui.openformatgridinvoice.get(invoiceChanges[i].recid);
                        if (lastRow[columns[_open].field] <= 0) {
                            lastRow = null;
                        }
                    }

                }
            }


            //for (var i = 0; i < invoiceChanges.length; i++) {
            //    if (invoiceChanges[i].selectrow == true) {
            //        paymentTotal += parseFloat(invoiceChanges[i].appliedamt);

            //        if (i == invoiceChanges.length - 1) {
            //            lastRow = w2ui.openformatgridinvoice.get(invoiceChanges[i].recid);
            //        }

            //    }
            //}

            if (invoiceTotal > paymentTotal) {
                var difference = invoiceTotal - paymentTotal;
                if ((invoiceTotal * .05) >= difference) {
                    //    VIS.ADialog.error("AllocationWriteOffWarn");
                    //}
                    if (lastRow != null) {
                        lastRow.changes.writeoff = difference;
                        lastRow.changes.appliedamt = lastRow.changes.appliedamt - difference;
                        w2ui.openformatgridinvoice.refreshCell(lastRow.recid, "writeoff");
                        w2ui.openformatgridinvoice.refreshCell(lastRow.recid, "appliedamt");
                    }
                }
            }
            //else if (paymentTotal > invoiceTotal) {
            //    var difference = paymentTotal - invoiceTotal;
            //    //if ((paymentTotal * .05) >= difference) {
            //    //    VIS.ADialog.error("AllocationWriteOffWarn");
            //    //}
            //    if (lastRow != null) {
            //        if (lastRow.changes.appliedamt % 1 === 0) {

            //        }
            //        else {
            //            lastRow.changes.writeoff = difference;
            //            lastRow.changes.appliedamt = lastRow.changes.appliedamt - difference;
            //            w2ui.openformatgridinvoice.refreshCell(lastRow.recid, "writeoff");
            //            w2ui.openformatgridinvoice.refreshCell(lastRow.recid, "appliedamt");
            //        }
            //    }
            //}


        };


        function bpValueChanged() {
            vetoableChange($vSearchBPartner.getName(), $vSearchBPartner.value);
        };

        /**
        *  Vetoable Change Listener.
        *  - Business Partner
        *  - Currency
        * 	- Date
        *  @param e event
        */

        function vetoableChange(name, value) {
            //log.Config(name + "=" + value);
            if (value == null) {
                //SetBusy(false);
                return;
            }

            //  BPartner
            if (name == "C_BPartner_ID") {
                _C_BPartner_ID = value;
                loadBPartner();
            }
                //	Currency
            else if (name == "C_Currency_ID") {
                _C_Currency_ID = parseInt(value);
                loadBPartner();
            }
                //	Date for Multi-Currency
            else if (name == "Date" && $vchkMultiCurrency) {
                loadBPartner();
            }
        }

        function calculate() {
            //log.Config("");
            //
            //DecimalFormat format = DisplayType.GetNumberFormat(DisplayType.Amount);
            var format = VIS.DisplayType.GetNumberFormat(VIS.DisplayType.Amount);
            var allocDate = new Date();
            allocDate = null;

            //  Payment******************
            var totalPay = parseFloat(0);
            //int rows = vdgvPayment.ItemsSource.OfType<object>().Count();
            _noPayments = 0;
            var rowsPayment = null;
            var rowsCash = null;
            var rowsInvoice = null;
            //Dispatcher.BeginInvoke(delegate
            //{
            //  rowsPayment = w2ui.openformatgrid.getSelection();
            rowsPayment = w2ui.openformatgrid.getChanges();
            rowsCash = w2ui.openformatgridcash.getChanges();
            rowsInvoice = w2ui.openformatgridinvoice.getChanges();

            //});
            if (rowsPayment != null) {
                for (var i = 0; i < rowsPayment.length; i++) {
                    if (rowsPayment[i].selectrow == true) {
                        var currnetRow = w2ui.openformatgrid.get(rowsPayment[i].recid);
                        var ts = new Date(currnetRow.date1);
                        //allocDate = VIS.TimeUtil.max(allocDate, ts);

                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);

                        var keys = Object.keys(currnetRow);
                        var bd = parseFloat(rowsPayment[i][keys[_payment + 1]]);
                        totalPay = totalPay + bd;  //  Applied Pay
                        _noPayments++;
                    }
                    //  log.Fine("Payment_" + i + " = " + bd + " - Total=" + totalPay);
                }
            }
            $lblPaymentSum.text(_noPayments + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalPay) + " ");

            //  Cash******************
            var totalCash = parseFloat(0);
            //rows = vdgvCashLine.ItemsSource.OfType<object>().Count();
            _noCashLines = 0;
            if (rowsCash != null) {
                for (var i = 0; i < rowsCash.length; i++) {
                    if (rowsCash[i].selectrow == true) {
                        var currnetRow = w2ui.openformatgridcash.get(rowsCash[i].recid);
                        var ts = new Date(currnetRow.date1);
                        //allocDate = VIS.TimeUtil.max(allocDate, ts);

                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);
                        //allocDate = ts;
                        //************************************** Changed

                        var keys = Object.keys(currnetRow);

                        var bd = parseFloat(rowsCash[i][keys[_payment + 1]]);// Util.GetValueOfDecimal(((BindableObject)rowsCash[i]).GetValue(_payment));
                        totalCash = totalCash + bd;  //  Applied Pay
                        _noCashLines++;
                        //log.Fine("Payment_" + i + " = " + bd + " - Total=" + totalCash);
                    }
                }
            }
            $lblCashSum.text(_noCashLines + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalCash) + " ");


            //  Invoices******************
            var totalInv = parseFloat(0);
            //rows = vdgvInvoice.ItemsSource.OfType<object>().Count();
            _noInvoices = 0;
            if (rowsInvoice != null) {
                for (var i = 0; i < rowsInvoice.length; i++) {
                    if (rowsInvoice[i].selectrow == true) {
                        var currnetRow = w2ui.openformatgridinvoice.get(rowsInvoice[i].recid);
                        var ts = new Date(currnetRow.date1);
                        //allocDate = VIS.TimeUtil.max(allocDate, ts);

                        var timeUtil = new VIS.TimeUtil();
                        allocDate = timeUtil.max(allocDate, ts);

                        //allocDate = ts;

                        var keys = Object.keys(currnetRow);
                        var bd;
                        if (rowsInvoice[i][keys[_applied + 1]] != "") {
                            bd = parseFloat(rowsInvoice[i][keys[_applied + 1]]);
                        }
                        else {
                            bd = 0;
                        }
                        totalInv = totalInv + bd;  //  Applied Inv
                        _noInvoices++;
                        //log.Fine("Invoice_" + i + " = " + bd + " - Total=" + totalPay);
                    }
                }
            }
            $lblInvoiceSum.text(_noInvoices + " - " + VIS.Msg.getMsg("Sum") + "  " + format.GetFormatedValue(totalInv) + " ");



            //	Set AllocationDate
            if (allocDate != null)
                $date.val(Globalize.format(allocDate, "yyyy-MM-dd"));
            //  Set Allocation Currency
            if ($cmbCurrency.children("option").filter(":selected").text() != null) {
                //vlblAllocCurrency.Content = cmbCurrencyPick.GetText(); //.getDisplay());
                $vlblAllocCurrency.text($cmbCurrency.children("option").filter(":selected").text());
            }
            // }
            //  Difference 
            //  Difference --New Logic for Invoice-(cash+payment)-by raghu 18-jan-2011 //  Cash******************
            var difference = (totalPay + totalCash) - totalInv;

            $vtxtDifference.text(format.GetFormatedValue(difference));
            if (difference == parseFloat(0)) {
                $vbtnAllocate.prop("readonly", false);
                $vbtnAllocate.css({ "pointer-events": "auto", "opacity": "1" });

            }
            else {
                $vbtnAllocate.prop("readonly", true);
                $vbtnAllocate.css({ "pointer-events": "none", "opacity": "0.5" });
            }



        }

        function vchkapplocationChange() {
            var rowsPayment = w2ui.openformatgrid.getChanges();
            var rowsCash = w2ui.openformatgridcash.getChanges();

            if ($vchkAllocation.is(':checked')) {
                readOnlyCash = false;
                readOnlyPayment = true;

                if (rowsPayment != null) {
                    for (var i = 0; i < rowsPayment.length; i++) {
                        var boolValue = getBoolValue(w2ui.openformatgrid.getChanges(), rowsPayment[i].recid);
                        if (boolValue) {
                            var changes = w2ui.openformatgrid.get(rowsPayment[i].recid).changes;
                            if (changes != null && changes != undefined) {
                                changes.selectrow = false;
                                changes.appliedamt = 0;
                                w2ui.openformatgrid.refreshCell(rowsPayment[i].recid, "selectrow");
                                w2ui.openformatgrid.refreshCell(rowsPayment[i].recid, "appliedamt");
                            }
                        }
                    }
                    w2ui.openformatgrid.refresh();
                }
            }
            else {
                readOnlyCash = true;
                readOnlyPayment = false;

                if (rowsCash != null) {
                    for (var i = 0; i < rowsCash.length; i++) {
                        var boolValue = getBoolValue(w2ui.openformatgridcash.getChanges(), rowsCash[i].recid);
                        if (boolValue) {
                            var changes = w2ui.openformatgridcash.get(rowsCash[i].recid).changes;
                            if (changes != null && changes != undefined) {
                                changes.selectrow = false;
                                changes.appliedamt = 0;
                                w2ui.openformatgridcash.refreshCell(rowsCash[i].recid, "selectrow");
                                w2ui.openformatgridcash.refreshCell(rowsCash[i].recid, "appliedamt");
                            }
                        }
                    }
                    w2ui.openformatgrid.refresh();
                }
            }
            calculate();

        };

        function allocate() {
            var canContinue = true;
            if (invoiceTotal > paymentTotal) {
                var difference = invoiceTotal - paymentTotal;
                if ((invoiceTotal * .05) <= difference) {
                    canContinue = VIS.ADialog.ask("AllocationWriteOffWarn");
                }
            }
            else if (paymentTotal > invoiceTotal) {
                var difference = paymentTotal - invoiceTotal;
                if ((paymentTotal * .05) >= difference) {
                    canContinue = VIS.ADialog.error("AllocationWriteOffWarn");
                }
            }
            if (canContinue) {
                $bsyDiv[0].style.visibility = "visible";
                var rowsPayment = w2ui.openformatgrid.getChanges();
                var rowsCash = w2ui.openformatgridcash.getChanges();
                var rowsInvoice = w2ui.openformatgridinvoice.getChanges();
                var DateTrx = $date.val();

                if ($vchkAllocation.is(':checked')) {
                    saveCashData(rowsPayment, rowsCash, rowsInvoice, DateTrx);
                }
                else {
                    savePaymentData(rowsPayment, rowsCash, rowsInvoice, DateTrx);
                }
            }

        };

        function saveCashData(rowsPayment, rowsCash, rowsInvoice, DateTrx) {

            if (_noInvoices + _noCashLines == 0)
                return "";
            var payment = "";
            var applied = "";
            var discount = "";
            var writeOff = "";
            var open = "";

            var paymentData = [];
            var cashData = [];
            var invoiceData = [];
            for (var i = 0; i < rowsPayment.length; i++) {
                var row = w2ui.openformatgrid.get(rowsPayment[i].recid);

                paymentData.push({
                    appliedamt: rowsPayment[i].appliedamt, date: row.date1, converted: row.convertedamount, cpaymentid: row.cpaymentid, documentno: row.documentno, isocode: row.isocode,
                    multiplierap: row.multiplierap, openamt: row.openamt, payment: row.payment
                });
            }
            if (rowsCash.length > 0) {
                var keys = Object.keys(w2ui.openformatgridcash.get(0));
                payment = keys[_payment + 1];
                for (var i = 0; i < rowsCash.length; i++) {
                    var row = w2ui.openformatgridcash.get(rowsCash[i].recid);

                    cashData.push({
                        appliedamt: rowsCash[i].appliedamt, date: row.created, amount: row.amount, ccashlineid: row.ccashlineid, converted: row.convertedamount, isocode: row.iso_code,
                        multiplierap: row.multiplierap, openamt: row.openamt, receiptno: row.receiptno
                    });
                }
            }

            if (rowsInvoice.length > 0) {
                var keys = Object.keys(w2ui.openformatgridinvoice.get(0));
                applied = keys[_applied + 1];
                discount = keys[_discount + 1];
                writeOff = keys[_writeOff + 1];
                open = keys[_open + 1];

                for (var i = 0; i < rowsInvoice.length; i++) {
                    var row = w2ui.openformatgridinvoice.get(rowsInvoice[i].recid);

                    var discounts = rowsInvoice[i].discount;
                    if (discounts == undefined) {
                        discounts = row.discount;
                    }

                    var appliedamts = rowsInvoice[i].appliedamt;
                    if (appliedamts == undefined) {
                        appliedamts = row.appliedamt;
                    }
                    var writeoffs = rowsInvoice[i].writeoff;
                    if (writeoffs == undefined) {
                        writeoffs = row.writeoff;
                    }

                    invoiceData.push({
                        appliedamt: appliedamts, discount: discounts, writeoff: writeoffs,
                        cinvoiceid: row.cinvoiceid, converted: row.converted, currency: row.currency,
                        date: row.date1, docbasetype: row.docbasetype,
                        documentno: row.documentno, isocode: row.iso_code, multiplierap: row.multiplierap, amount: row.amount
                    });
                }
            }


            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/SaveCashData",
                data: ({
                    paymentData: JSON.stringify(paymentData), cashData: JSON.stringify(cashData), invoiceData: JSON.stringify(invoiceData), currency: $cmbCurrency.val(),
                    isCash: $vchkAllocation.is(':checked'), _C_BPartner_ID: _C_BPartner_ID, _windowNo: self.windowNo, payment: payment, DateTrx: $date.val(), appliedamt: applied
                        , discount: discount, writeOff: writeOff, open: open
                }),
                success: function (result) {
                    loadBPartner();
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function (result) {
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });

        };


        function savePaymentData(rowsPayment, rowsCash, rowsInvoice, DateTrx) {

            if (_noInvoices + _payment == 0)
                return "";
            var payment = "";
            var applied = "";
            var discount = "";
            var writeOff = "";
            var open = "";

            var paymentData = [];
            var cashData = [];
            var invoiceData = [];
            for (var i = 0; i < rowsPayment.length; i++) {
                var row = w2ui.openformatgrid.get(rowsPayment[i].recid);
                var keys = Object.keys(w2ui.openformatgrid.get(0));
                payment = keys[10];
                paymentData.push({
                    appliedamt: rowsPayment[i].appliedamt, date: row.date1, converted: row.convertedamount, cpaymentid: row.cpaymentid, documentno: row.documentno, isocode: row.isocode,
                    multiplierap: row.multiplierap, openamt: row.openamt, payment: row.payment
                });
            }
            if (rowsCash.length > 0) {
                var keys = Object.keys(w2ui.openformatgridcash.get(0));
                for (var i = 0; i < rowsCash.length; i++) {
                    var row = w2ui.openformatgridcash.get(rowsCash[i].recid);

                    cashData.push({
                        appliedamt: rowsCash[i].appliedamt, date: row.created, amount: row.amount, ccashlineid: row.ccashlineid, converted: row.convertedamount, isocode: row.iso_code,
                        multiplierap: row.multiplierap, openamt: row.openamt, receiptno: row.receiptno
                    });
                }
            }

            if (rowsInvoice.length > 0) {
                var keys = Object.keys(w2ui.openformatgridinvoice.get(0));
                applied = keys[_applied + 1];
                payment = keys[_applied + 1];

                discount = keys[_discount + 1];
                writeOff = keys[_writeOff + 1];
                open = keys[_open + 1];

                for (var i = 0; i < rowsInvoice.length; i++) {
                    var row = w2ui.openformatgridinvoice.get(rowsInvoice[i].recid);

                    var discounts = rowsInvoice[i].discount;
                    if (discounts == undefined) {
                        discounts = row.discount;
                    }

                    var appliedamts = rowsInvoice[i].appliedamt;
                    if (appliedamts == undefined) {
                        appliedamts = row.appliedamt;
                    }
                    var writeoffs = rowsInvoice[i].writeoff;
                    if (writeoffs == undefined) {
                        writeoffs = row.writeoff;
                    }

                    invoiceData.push({
                        appliedamt: appliedamts, discount: discounts, writeoff: writeoffs,
                        cinvoiceid: row.cinvoiceid, converted: row.converted, currency: row.currency,
                        date: row.date1, docbasetype: row.docbasetype,
                        documentno: row.documentno, isocode: row.iso_code, multiplierap: row.multiplierap, amount: row.amount
                    });
                }
            }


            $.ajax({
                url: VIS.Application.contextUrl + "PaymentAllocation/SavePaymentData",
                data: ({
                    paymentData: JSON.stringify(paymentData), cashData: JSON.stringify(cashData), invoiceData: JSON.stringify(invoiceData), currency: $cmbCurrency.val(),
                    isCash: $vchkAllocation.is(':checked'), _C_BPartner_ID: _C_BPartner_ID, _windowNo: self.windowNo, payment: payment, DateTrx: $date.val(), appliedamt: applied
                        , discount: discount, writeOff: writeOff, open: open
                }),
                success: function (result) {
                    loadBPartner();
                    $bsyDiv[0].style.visibility = "hidden";
                },
                error: function () {
                    $bsyDiv[0].style.visibility = "hidden";
                }
            });

        };

        this.disposeComponent = function () {
            if ($gridPayment != undefined && $gridPayment != null) {
                $gridPayment.destroy();
                $gridPayment = null;
            }

            if ($gridCashline != undefined && $gridCashline != null) {
                $gridCashline.destroy();
                $gridCashline = null;
            }

            if ($gridInvoice != undefined && $gridInvoice != null) {
                $gridInvoice.destroy();
                $gridInvoice = null;
            }
        };


        this.getRoot = function () {
            return $root;
        };
    };

    VAllocation.prototype.init = function (windowNo, frame) {
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
    };

    VAllocation.prototype.dispose = function () {
        /*CleanUp Code */
        //dispose this component
        this.disposeComponent();

        //call frame dispose function
        if (this.frame)
            this.frame.dispose();
        this.frame = null;
    };

    VIS.Apps = VIS.Apps || {};
    VIS.Apps.AForms = VIS.Apps.AForms || {};
    VIS.Apps.AForms.VAllocation = VAllocation;

})(VIS, jQuery)