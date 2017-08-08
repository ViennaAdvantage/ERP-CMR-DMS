/** 
  *    Sample Class for Callout
       -  must call base class (CalloutEngine)
       -- must inheirt Base class
  */


; (function (VIS, $) {

    var Level = VIS.Logging.Level;
    var Util = VIS.Utility.Util;
    var steps = false;
    //1
    /* Sample Start */

     
    /**
    *  Callout Class
      -   must call this function
             VIS.CalloutEngine.call(this, [className]);
    */
    function TestClass() {
        VIS.CalloutEngine.call(this, "VIS.TestClass"); //must call
    };
    /**
     * Inherit CallourEngile Class 
     */
    VIS.Utility.inheritPrototype(TestClass, VIS.CalloutEngine);//inherit CalloutEngine


    /**
     *  Callout function
     */
    TestClass.prototype.set = function (ctx, windowNo, mTab, mField, value, oldValue) {
        mTab.setValue("Description", value);
        return "";
    };

    VIS.Model.TestClass = TestClass; //assign object in Model NameSpace

    /* Sample END */

        //2 

    //************Callout Order*************//
    //****CalloutOrder Start
    function CalloutOrder() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrder");
    };
    //#endregion
    VIS.Utility.inheritPrototype(CalloutOrder, VIS.CalloutEngine); //inherit calloutengine

    CalloutOrder.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {

        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        /** DeliveryRule AD_Reference_ID=151 */
        var XC_DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var XC_DELIVERYRULE_Availability = "A";
        /** Force = F */
        var XC_DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var XC_DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var XC_DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var XC_DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var XC_DELIVERYRULE_AfterReceipt = "R";

        var XC_INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var XC_INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var XC_INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var XC_INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var XC_INVOICERULE_CustomerScheduleAfterDelivery = "S";



        /** PaymentRule AD_Reference_ID=195 */
        var XC_PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        var XC_PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        var XC_PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        var XC_PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        var XC_PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        var XC_PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        var XC_PAYMENTRULE_DirectDeposit = "T";
        //var Util=VIS.Util;

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        //var C_DocType_ID = Util.getValueOfInt(value);		//	Actually C_DocTypeTarget_ID
        var C_DocType_ID = Util.getValueOfInt(value);

        if (C_DocType_ID == null || C_DocType_ID == 0) {
            return "";
        }

        this.setCalloutActive(true);
        //	Re-Create new DocNo, if there is a doc number already
        //	and the existing source used a different Sequence number

        var oldDocNo = Util.getValueOfString(mTab.getValue("DocumentNo"));

        var newDocNo = (oldDocNo == null);

        if (!newDocNo && oldDocNo.startsWith("<") && oldDocNo.endsWith(">"))
            newDocNo = true;

        var oldC_DocType_ID = Util.getValueOfInt(mTab.getValue("C_DocType_ID"));

        var sql = "SELECT d.DocSubTypeSO,d.HasCharges,'N',"			//	1..3
            + "d.IsDocNoControlled,s.CurrentNext,s.CurrentNextSys,"     //  4..6
            + "s.AD_Sequence_ID,d.IsSOTrx, d.IsReturnTrx "              //	7..9
            + "FROM C_DocType d "
            + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
            + "WHERE C_DocType_ID=@param1";	//	#1
        var idr = null;
        //var param = new SqlParameter[1];
        var p = [];

        try {
            var AD_Sequence_ID = 0;
            //	Get old AD_SeqNo for comparison
            if (!newDocNo && Util.getValueOfInt(oldC_DocType_ID) != 0) {
                p[0] = new VIS.DB.SqlParam("@param1", oldC_DocType_ID);

                //param[0] = new SqlParameter("@param1", oldC_DocType_ID);
                idr = VIS.DB.executeReader(sql, p);
                if (idr.read()) {
                    //AD_Sequence_ID = Util.getValueOfInt(idr.tables[0].rows[0].cells["currentnextsys"]);
                    AD_Sequence_ID = Util.getValueOfInt(idr.get("currentnextsys"));
                }
                idr.close();
            }
            p[0] = new VIS.DB.SqlParam("@param1", C_DocType_ID);
            idr = VIS.DB.executeReader(sql, p);

            p.length = 0;
            p = null;

            var DocSubTypeSO = "";
            var IsSOTrx = true;
            var isReturnTrx = false;
            if (idr.read())		//	we found document type
            {

                //	Set Context:	Document Sub Type for Sales Orders
                DocSubTypeSO = idr.get("docsubtypeso");
                if (DocSubTypeSO == null)
                    DocSubTypeSO = "--";
                ctx.setContext(windowNo, "OrderType", DocSubTypeSO);
                //	No Drop Ship other than Standard
                if (!DocSubTypeSO == DocSubTypeSO_Standard)
                    mTab.setValue("IsDropShip", "N");

                //	IsSOTrx
                if ("N" == idr.get("issotrx"))
                    IsSOTrx = false;

                //IsReturnTrx
                isReturnTrx = idr.get("isreturntrx") == "Y" ? true : false;

                //	Skip these steps for RMA. These are copied from the Original Order
                if (!isReturnTrx) {
                    if (DocSubTypeSO == DocSubTypeSO_POS)
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Force);
                    else if (DocSubTypeSO == DocSubTypeSO_Prepay)
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_AfterReceipt);
                    else
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Availability);

                    //	Invoice Rule
                    if ((DocSubTypeSO == DocSubTypeSO_POS)
                        || (DocSubTypeSO == DocSubTypeSO_Prepay)
                        || (DocSubTypeSO == DocSubTypeSO_OnCredit))
                        mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
                    else
                        mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);

                    //	Payment Rule - POS Order
                    if (DocSubTypeSO == DocSubTypeSO_POS)
                        mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
                    else
                        mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);


                    //	Set Context:
                    ctx.setContext(windowNo, "HasCharges", Util.getValueOfString(idr.get("hascharges")));
                }
                else // Returns
                {
                    if (DocSubTypeSO == DocSubTypeSO_POS)
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Force);
                    else
                        mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Manual);
                }

                //	DocumentNo
                if (idr.get("isdocnocontrolled") == "Y")			//	IsDocNoControlled
                {
                    if (!newDocNo && AD_Sequence_ID != Util.getValueOfInt(idr.get("ad_sequence_id")))
                        newDocNo = true;
                    if (newDocNo)  //Temporaly Commented By Sarab
                        //if (Ini.isPropertyBool(Ini.P_VIENNASYS) && Env.getCtx().getAD_Client_ID() < 1000000) 
                        if (VIS.Ini.getLocalStorage(VIS.IniConstants.P_VIENNASYS) && ctx.getAD_Client_ID() < 1000000)
                            mTab.setValue("DocumentNo", "<" + idr.tables[0].rows[0].cells["currentnextsys"] + ">");
                        else
                            mTab.setValue("DocumentNo", "<" + idr.tables[0].rows[0].cells["currentnext"] + ">");
                }
            }
            idr.close();

            // Skip remaining steps for RMA
            if (isReturnTrx) {
                this.setCalloutActive(false);
                return "";
            }
            //  When BPartner is changed, the Rules are not set if
            //  it is a POS or Credit Order (i.e. defaults from Standard BPartner)
            //  This re-reads the Rules and applies them.
            if ((DocSubTypeSO == DocSubTypeSO_POS)
                || (DocSubTypeSO == DocSubTypeSO_Prepay))    //  not for POS/PrePay
                ;
            else {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                sql = "SELECT PaymentRule,C_PaymentTerm_ID,"            //  1..2
                    + "InvoiceRule,DeliveryRule,"                       //  3..4
                    + "FreightCostRule,DeliveryViaRule, "               //  5..6
                    + "PaymentRulePO,PO_PaymentTerm_ID "
                    + "FROM C_BPartner "
                    + "WHERE C_BPartner_ID=" + C_BPartner_ID;		//	#1
                idr = VIS.DB.executeReader(sql, null);

                if (idr.read()) {
                    //	PaymentRule
                    //var s = Util.getValueOfString(idr[IsSOTrx ? "PaymentRule" : "PaymentRulePO"]);
                    var s = idr.get(IsSOTrx ? "paymentrule" : "paymentrulepo");
                    if (s != null && s.length != 0) {
                        if (IsSOTrx && (s == "B") || (s == "S") || (s == "U"))	//	No Cash/Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        if (!IsSOTrx && (s == "B"))					//	No Cash for PO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    //var ii = Util.getValueOfInt(idr[IsSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"]);
                    var ii = idr.get(IsSOTrx ? "c_paymentterm_id" : "po_paymentterm_id");

                    //if (!idr.wasNull())
                    if (idr != null)
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    //	InvoiceRule
                    s = idr.get("invoicerule");
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = idr.get("deliveryrule");
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = idr.get("freightcostrule");
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = idr.get("deliveryviarule");
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
                idr.close();
            }   //  re-read customer rules
        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
                idr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Header - BPartner.
    /// - M_PriceList_ID (+ Context)
    /// - C_BPartner_Location_ID
    /// - Bill_BPartner_ID/Bill_Location_ID
    /// 	- AD_User_ID
    /// 	- POReference
    /// 	- SO_Description
    /// 	- IsDiscountPrinted
    /// 	- InvoiceRule/DeliveryRule/PaymentRule/FreightCost/DeliveryViaRule
    /// 	- C_PaymentTerm_ID
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrder.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {

        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        /** DeliveryRule AD_Reference_ID=151 */
        var XC_DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var XC_DELIVERYRULE_Availability = "A";
        /** Force = F */
        var XC_DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var XC_DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var XC_DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var XC_DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var XC_DELIVERYRULE_AfterReceipt = "R";

        /** InvoiceRule AD_Reference_ID=150 */
        var XC_INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var XC_INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var XC_INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var XC_INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var XC_INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** PaymentRule AD_Reference_ID=195 */
        var XC_PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        var XC_PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        var XC_PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        var XC_PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        var XC_PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        var XC_PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        var XC_PAYMENTRULE_DirectDeposit = "T";

        //var Util=VIS.Util;
        var sql = "";
        var dr = null;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {

            var C_BPartner_ID = 0;
            if (value != null)
                C_BPartner_ID = Util.getValueOfInt(value.toString());
            if (C_BPartner_ID == 0)
                return "";

            // Skip rest of steps for RMA. These fields are copied over from the orignal order instead.
            var isReturnTrx = Util.getValueOfBoolean(mTab.getValue("IsReturnTrx"));
            if (isReturnTrx)
                return "";



            sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " lship.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
                + " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            var isSOTrx = ctx.isSOTrx();

            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	PriceList (indirect: IsTaxIncluded & Currency)
                //var ii = Util.getValueOfInt(dr[isSOTrx ? "M_PriceList_ID" : "PO_PriceList_ID"].toString()); //Sarab
                var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                if (dr != null && ii != 0) {
                    mTab.setValue("M_PriceList_ID", ii);
                }
                else {	//	get default PriceList
                    //var i1 = ctx.getContextAsInt("#M_PriceList_ID");//Sarab
                    var i1 = ctx.getContextAsInt(windowNo, "#M_PriceList_ID", false);
                    if (i1 != 0)
                        mTab.setValue("M_PriceList_ID", i1);
                }
                //	Bill-To BPartner
                this.setCalloutActive(true);
                mTab.setValue("Bill_BPartner_ID", C_BPartner_ID);
                //var bill_Location_ID = Util.getValueOfInt(dr["Bill_Location_ID"].toString());//Sarab
                var bill_Location_ID = Util.getValueOfInt(dr.get("bill_location_id"));
                if (bill_Location_ID == 0)
                    mTab.setValue("Bill_Location_ID", null);
                else
                    mTab.setValue("Bill_Location_ID", bill_Location_ID);

                // Ship-To Location
                //var shipTo_ID = Util.getValueOfInt(dr["C_BPartner_Location_ID"].toString());//sarab
                var shipTo_ID = Util.getValueOfInt(dr.get("c_bpartner_location_id"));
                //	overwritten by InfoBP selection - works only if InfoWindow
                //	was used otherwise creates error (uses last value, may belong to differnt BP)
                //if (C_BPartner_ID.toString()==ctx.getContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID"))//Sarab
                //{
                //    var loc = ctx.getContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_Location_ID");
                //    if (loc.length > 0)
                //        shipTo_ID = Util.getValueOfInt(loc);
                //}

                if (C_BPartner_ID.toString() == ctx.getContext("C_BPartner_ID")) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.length > 0)
                        shipTo_ID = Util.getValueOfInt(loc);
                }
                if (shipTo_ID == 0)
                    mTab.setValue("C_BPartner_Location_ID", null);
                else {
                    mTab.setValue("C_BPartner_Location_ID", shipTo_ID);
                    if ("Y" == dr.get("isshipto").toString())	//	set the same
                        mTab.setValue("Bill_Location_ID", shipTo_ID);
                }
                //	Contact - overwritten by InfoBP selection
                //var contID = Util.getValueOfInt(dr.tables[0].rows[0].cells["AD_User_ID"].toString());//Sarab
                var contID = Util.getValueOfInt(dr.get("ad_user_id"));

                //if (C_BPartner_ID.toString()==ctx.getContext(Env.WINDOW_INFO, Env.TAB_INFO, "C_BPartner_ID"))//Sarab
                //{
                //    var cont = ctx.getContext(Env.WINDOW_INFO, Env.TAB_INFO, "AD_User_ID");
                //    if (cont.length > 0)
                //        contID = Util.getValueOfInt(cont);
                //}
                if (C_BPartner_ID.toString() == ctx.getContext("C_BPartner_ID")) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.length > 0)
                        contID = Util.getValueOfInt(cont);
                }
                if (contID == 0)
                    mTab.setValue("AD_User_ID", null);
                else {
                    mTab.setValue("AD_User_ID", contID);
                    mTab.setValue("Bill_User_ID", contID);
                }

                //	CreditAvailable 
                if (isSOTrx) {
                    var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                    //	var SOCreditStatus = dr.getString("SOCreditStatus");
                    if (CreditLimit != 0) {
                        var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                        if (dr != null && CreditAvailable < 0) {
                            //mTab.fireDataStatusEEvent("CreditLimitOver",
                            //    DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable),
                            //    false);
                            VIS.ADialog.info("CreditLimitOver", null, "", "");
                        }
                    }
                }

                //	PO Reference
                //var s = dr["POReference"].toString();//Sarab
                var s = dr.get("poreference");
                if (s != null && s.length != 0)
                    mTab.setValue("POReference", s);
                // should not be reset to null if we entered already value! VHARCQ, accepted YS makes sense that way
                // TODO: should get checked and removed if no longer needed!
                /*else
                    mTab.setValue("POReference", null);*/

                //	SO Description
                //s = dr["SO_Description"].toString();//Sarab
                s = dr.get("so_description");
                if (s != null && s.trim().length != 0)
                    mTab.setValue("Description", s);
                //	IsDiscountPrinted
                //s = dr["IsDiscountPrinted"].toString();//Sarab
                s = dr.get("isdiscountprinted")
                if (s != null && s.length != 0)
                    mTab.setValue("IsDiscountPrinted", s);
                else
                    mTab.setValue("IsDiscountPrinted", "N");

                //	Defaults, if not Walkin Receipt or Walkin Invoice
                var OrderType = ctx.getContext("OrderType");
                mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);
                mTab.setValue("DeliveryRule", XC_DELIVERYRULE_Availability);
                mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);
                if (OrderType == DocSubTypeSO_Prepay) {
                    mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
                    mTab.setValue("DeliveryRule", XC_DELIVERYRULE_AfterReceipt);
                }
                else if (OrderType == DocSubTypeSO_POS)	//  for POS
                    mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
                else {
                    //	PaymentRule
                    //s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].toString();//Sarab
                    s = dr.get(isSOTrx ? "paymentrule" : "paymentrulepo");
                    if (s != null && s.length != 0) {
                        if (s == "B")				//	No Cache in Non PO
                            s = "P";
                        if (isSOTrx && (s == "S") || (s == "U"))	//	No Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    //ii = Util.getValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].toString());//Sarab
                    ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                    if (dr != null && ii != 0)//ii=0 when dr return ""
                    {
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    }
                    //	InvoiceRule
                    //s = dr["InvoiceRule"].toString();//Sarab
                    s = dr.get("invoicerule");
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    //s = dr["DeliveryRule"].toString();//Sarab
                    s = dr.get("deliveryrule");
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    //s = dr["FreightCostRule"].toString();//Sarab
                    s = dr.get("freightcostrule");
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    //s = dr["DeliveryViaRule"].toString();//Sarab
                    s = dr.get("deliveryviarule");
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
            }
            dr.close();
        }
        catch (err) {
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// Order Header - Invoice BPartner.
    /// - M_PriceList_ID (+ Context)
    /// - Bill_Location_ID
    /// - Bill_User_ID
    /// - POReference
    /// - SO_Description
    /// - IsDiscountPrinted
    /// - InvoiceRule/PaymentRule
    /// - C_PaymentTerm_ID
    ///   *  @param ctx      
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrder.prototype.BPartnerBill = function (ctx, windowNo, mTab, mField, value, oldValue) {


        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        /** DeliveryRule AD_Reference_ID=151 */
        var XC_DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var XC_DELIVERYRULE_Availability = "A";
        /** Force = F */
        var XC_DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var XC_DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var XC_DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var XC_DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var XC_DELIVERYRULE_AfterReceipt = "R";


        var XC_INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var XC_INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var XC_INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var XC_INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var XC_INVOICERULE_CustomerScheduleAfterDelivery = "S";



        /** PaymentRule AD_Reference_ID=195 */
        var XC_PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        var XC_PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        var XC_PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        var XC_PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        var XC_PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        var XC_PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        var XC_PAYMENTRULE_DirectDeposit = "T";
        if (this.isCalloutActive())
            return "";
        if (value == null || value.toString() == "") {
            return "";
        }
        var dr = null;
        try {
            var bill_BPartner_ID = Util.getValueOfInt(value.toString());
            if (bill_BPartner_ID == null || bill_BPartner_ID == 0)
                return "";
            this.setCalloutActive(true);
            // Skip rest of steps for RMA
            var isReturnTrx = Util.getValueOfBoolean(mTab.getValue("IsReturnTrx"));
            if (isReturnTrx) {
                this.setCalloutActive(false);
                return "";
            }
            var sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + "p.M_PriceList_ID,p.PaymentRule,p.POReference,"
                + "p.SO_Description,p.IsDiscountPrinted,"
                + "p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + "p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "c.AD_User_ID,"
                + "p.PO_PriceList_ID, p.PaymentRulePO, p.PO_PaymentTerm_ID,"
                + "lbill.C_BPartner_Location_ID AS Bill_Location_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + bill_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            var isSOTrx = "Y" == (ctx.getWindowContext(windowNo, "IsSOTrx", true));
            //var isSOTrx = false;
            //if("Y"==(ctx.getContext("IsSOTrx")))
            //{
            //    isSOTrx=true;
            //}


            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	PriceList (indirect: IsTaxIncluded & Currency)
                var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                if (ii != 0)
                    mTab.setValue("M_PriceList_ID", ii);
                else {	//	get default PriceList
                    var iCont = ctx.getContextAsInt(windowNo, "#M_PriceList_ID", false);
                    //var iCont = ctx.getContextAsInt("#M_PriceList_ID");//Sarab
                    if (iCont != 0)
                        mTab.setValue("M_PriceList_ID", Util.getValueOfInt(iCont));
                }
                var bill_Location_ID = Util.getValueOfInt(dr.get("bill_location_id"));
                //	overwritten by InfoBP selection - works only if InfoWindow
                //	was used otherwise creates error (uses last value, may belong to differnt BP)
                if (bill_BPartner_ID.toString() == (ctx.getContext("C_BPartner_ID"))) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.length > 0)
                        bill_Location_ID = Util.getValueOfInt(loc);
                }
                if (bill_Location_ID == 0)
                    mTab.setValue("Bill_Location_ID", null);
                else
                    mTab.setValue("Bill_Location_ID", Util.getValueOfInt(bill_Location_ID));

                //	Contact - overwritten by InfoBP selection
                var contID = Util.getValueOfInt(dr.get("ad_user_id"));
                if (bill_BPartner_ID.toString() == (ctx.getContext("C_BPartner_ID"))) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.length > 0)
                        contID = Util.getValueOfInt(cont);
                }
                if (contID == 0)
                    mTab.setValue("Bill_User_ID", null);
                else
                    mTab.setValue("Bill_User_ID", Util.getValueOfInt(contID.toString()));
                //	CreditAvailable 
                if (isSOTrx) {
                    var CreditLimit = 0;
                    if (CreditLimit != 0) {
                        var CreditAvailable = 0;
                        if (dr != null && CreditAvailable < 0) {
                            //mTab.fireDataStatusEEvent("CreditLimitOver",
                            //    DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable),
                            //    false);
                            VIS.ADialog.info("CreditLimitOver", null, "", "");
                        }
                    }
                }

                //	PO Reference
                var s = dr.get("poreference");

                // Order Reference should not be set by Bill To BPartner; only by BPartner.
                /* if (s != null && s.length() != 0)
                    mTab.setValue("POReference", s);
                else
                    mTab.setValue("POReference", null);*/
                //	SO Description
                s = dr.get("so_description");
                if (s != null && s.toString().trim().length != 0)
                    mTab.setValue("Description", s);
                //	IsDiscountPrinted
                s = dr.get("isdiscountprinted");
                if (s != null && s.toString().length != 0)
                    mTab.setValue("IsDiscountPrinted", s);
                else
                    mTab.setValue("IsDiscountPrinted", "N");

                //	Defaults, if not Walkin Receipt or Walkin Invoice
                var OrderType = ctx.getContext("OrderType");
                mTab.setValue("InvoiceRule", XC_INVOICERULE_AfterDelivery);
                mTab.setValue("PaymentRule", XC_PAYMENTRULE_OnCredit);
                if (OrderType == DocSubTypeSO_Prepay) {

                    mTab.setValue("InvoiceRule", XC_INVOICERULE_Immediate);
                }
                else if (OrderType == DocSubTypeSO_POS)	//  for POS 
                {
                    mTab.setValue("PaymentRule", XC_PAYMENTRULE_Cash);
                }

                else {
                    //	PaymentRule
                    s = dr.get(isSOTrx ? "paymentrule" : "paymentrulepo");
                    if (s != null && s.toString().length != 0) {
                        if (s == "B")				//	No Cache in Non POS
                            s = "P";
                        if (isSOTrx && ((s == "S") || s == "U"))	//	No Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    ii = dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id");
                    if (dr != null)
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    //	InvoiceRule
                    s = dr.get("invoicerule");
                    if (s != null && s.toString().length != 0)
                        mTab.setValue("InvoiceRule", s);
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log(Level.SEVERE, "bPartnerBill", err);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };



    /// <summary>
    /// Order Header - PriceList.
    /// (used also in Invoice)
    /// - C_Currency_ID
    /// 	- IsTaxIncluded
    /// 	Window Context:
    /// 	- EnforcePriceLimit
    /// 	- StdPrecision
    /// 	- M_PriceList_Version_ID
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.PriceList = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        var sql = "";
        var dr = null;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {

            var M_PriceList_ID = Util.getValueOfInt(value.toString());
            if (M_PriceList_ID == null || M_PriceList_ID == 0)
                return "";
            this.setCalloutActive(true);
            if (steps) {
                this.log.warning("init");
            }

            sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";
            //	Use net price list - may not be future

            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	Tax Included
                mTab.setValue("IsTaxIncluded", "Y" == dr.get("istaxincluded").toString());
                //	Price Limit Enforce
                ctx.setContext(windowNo, "EnforcePriceLimit", dr.get("enforcepricelimit").toString());
                //	Currency
                var ii = Util.getValueOfInt(dr.get("c_currency_id"));
                mTab.setValue("C_Currency_ID", ii);
                var prislst = Util.getValueOfInt(dr.get("m_pricelist_version_id"));
                //	PriceList Version
                ctx.setContext(windowNo, "M_PriceList_Version_ID", prislst);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log(Level.SEVERE, sql, err);
            return err;
        }
        if (steps) {
            this.log.warning("finish");
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Line - Product.
    /// - reset C_Charge_ID / M_AttributeSetInstance_ID
    /// - PriceList, PriceStd, PriceLimit, C_Currency_ID, EnforcePriceLimit
    /// - UOM
    /// Calls Tax
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = Util.getValueOfInt(value);
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";

        var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));
        if (isReturnTrx)
            return "";

        this.setCalloutActive(true);
        try {
            if (steps) {
                this.log.warning("init");
            }
            //
            mTab.setValue("C_Charge_ID", null);
            //	Set Attribute
            if (ctx.getContextAsInt(windowNo, "M_Product_ID", false) == M_Product_ID
                && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false) != 0) {
                mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false)));
            }
            else {
                mTab.setValue("M_AttributeSetInstance_ID", null);
            }

            // Amit 26-05-2015
            //
            var sql = "SELECT producttype FROM m_product where isactive = 'Y' AND M_Product_ID = " + M_Product_ID;
            var productType = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
            if (productType == "S") {
                mTab.setValue("IsContract", true);
            }
            else {
                mTab.setValue("IsContract", false);
                mTab.setValue("NoofCycle", null);
                mTab.setValue("QtyPerCycle", null);
                mTab.setValue("StartDate", null);
                mTab.setValue("C_Contract_ID", 0); // Contract
                mTab.setValue("EndDate", null);
                mTab.setValue("C_Frequency_ID", 0);  // Billing frequncy
            }
            //




            /*****	Price Calculation see also qty	****/
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
            var Qty = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID", false);
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID", false);
            var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false);

            var orderDate = mTab.getValue("DateOrdered");


            //1                                                              
            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                            Qty.toString(), ",", //3
                                                            isSOTrx, ",", //4 
                                                            M_PriceList_ID.toString(), ",", //5
                                                            M_PriceList_Version_ID.toString(), ",", //6
                                                            orderDate.toString(), ",",         //7
                                                            null, ",", M_AttributeSetInstance_ID.toString()); //8
            try {


                //Get product price information
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


                var rowDataDB = null;
                //	only one row
                //if (dr.read())
                {
                    //rowDataDB = this.readData(dr);

                    mTab.setValue("PriceList", dr["PriceList"]);
                    mTab.setValue("PriceLimit", dr.PriceLimit);
                    mTab.setValue("PriceActual", dr.PriceActual);
                    mTab.setValue("PriceEntered", dr.PriceEntered);
                    mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
                    mTab.setValue("Discount", dr.Discount);
                    mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
                    mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));
                    ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
                    ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");
                }
                //	Check/Update Warehouse Setting
                //	var M_Warehouse_ID = ctx.getContextAsInt( Env.WINDOW_INFO, "M_Warehouse_ID");
                //	var wh = (int)mTab.getValue("M_Warehouse_ID");
                //	if (wh.intValue() != M_Warehouse_ID)
                //	{
                //		mTab.setValue("M_Warehouse_ID", new int(M_Warehouse_ID));
                //		ADialog.warn(,windowNo, "WarehouseChanged");
                //	}

                if (ctx.isSOTrx()) {

                    //MProduct product = MProduct.get(ctx, M_Product_ID);
                    //Check product Stock
                    //var dr = null;
                    // dr = VIS.dataContext.getJSONRecord("CalloutOrder/GetProductStock", M_Product_ID.toString());


                    //if (dr.IsStocked) {
                    var QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                    var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                    var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
                    var C_OrderLine_ID = 0;

                    if (mTab.getValue("C_OrderLine_ID") != null) {
                        C_OrderLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                    }

                    if (C_OrderLine_ID == null)
                        C_OrderLine_ID = 0;

                    //Get Qty information from server side
                    var paramString = M_Product_ID.toString().concat(",", M_Warehouse_ID.toString(), ",", //2
                                                           M_AttributeSetInstance_ID.toString(), ",", //3
                                                           C_OrderLine_ID.toString()); //4

                    //Get product price information
                    //var dr = null;
                    var available = VIS.dataContext.getJSONRecord("MStorage/GetQtyAvailable", paramString);



                    //var available = dr.available;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);

                    if (available == null)
                        available = VIS.Env.ZERO;
                    if (available == 0) {
                        //mTab.fireDataStatusEEvent("NoQtyAvailable", "0", false);
                        // VIS.ADialog.info("NoQtyAvailable", true, "0", "");
                    }
                    else if (available.toString().compareTo(QtyOrdered) < 0) {
                        //mTab.fireDataStatusEEvent("InsufficientQtyAvailable", available.toString(), false);
                        // VIS.ADialog.info("InsufficientQtyAvailable", true, available.toString(), "");
                    }
                    else {

                        var paramString = M_Warehouse_ID.toString() + "," + M_Product_ID.toString() + "," + M_AttributeSetInstance_ID.toString() + "," + C_OrderLine_ID.toString();
                        var notReserved = VIS.dataContext.getJSONRecord("MOrderLine/GetNotReserved", paramString);
                        //var notReserved = dr.notReserved;//.getNotReserved(ctx,
                        //M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                        //C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = VIS.Env.ZERO;
                        //var total = available.subtract(notReserved);
                        var total = available - notReserved;
                        if (total.compareTo(QtyOrdered) < 0) {
                            var info = VIS.Msg.parseTranslation(ctx, "@QtyAvailable@=" + available
                                + " - @QtyNotReserved@=" + notReserved + " = " + total);
                            VIS.ADialog.info("InsufficientQtyAvailable", true, info, "");
                            //mTab.fireDataStatusEEvent("InsufficientQtyAvailable",info, false);
                            // VIS.ADiathis.log.info("InsufficientQtyAvailable", true, info, "");
                        }
                        // }
                    }
                }


                if (steps) {
                    this.log.warning("fini");
                }
            }
            catch (err) {
                this.log.saveError("calloutorder", err.toString());
                this.setCalloutActive(false);

                //for (var k = 0; k < VAdvantage.Classes.infoLines.PQ.Count; k++)
                //{

                //    VAdvantage.Classes.infoLines.PQ.RemoveAt(k);
                //}
                return err.message;
            }
            this.setCalloutActive(false);
            //return Tax(ctx, windowNo, mTab, mField, value);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.saveError("calloutorder", err.toString());
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);

    };




    /// <summary>
    /// Order Line - Charge.
    /// - updates PriceActual from Charge
    /// - sets PriceLimit, PriceList to zero
    /// 	Calles tax
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value"> New Value</param>
    /// <returns>null or error message</returns>


    CalloutOrder.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {

        try {
            if (this.isCalloutActive() || value == null || value.toString() == "") {
                return "";
            }
            this.setCalloutActive(true);
            var C_Charge_ID = Util.getValueOfInt(value);
            if (C_Charge_ID == null || C_Charge_ID == 0) {
                this.setCalloutActive(false);
                return "";
            }

            var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));
            if (isReturnTrx) {
                this.setCalloutActive(false);
                return "";
            }

            //	No Product defined
            if (mTab.getValue("M_Product_ID") != null) {

                mTab.setValue("M_Product_ID", null);
                //mTab.setValue("C_Charge_ID", null);
                //this.setCalloutActive(false);
                //return "ChargeExclusively";
            }
            mTab.setValue("M_AttributeSetInstance_ID", null);
            mTab.setValue("S_ResourceAssignment_ID", null);
            mTab.setValue("C_UOM_ID", 100);	//	EA

            ctx.setContext(windowNo, "DiscountSchema", "N");
            var sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;
            var dr = null;

            dr = VIS.DB.executeReader(sql);
            var PriceEntered;
            if (dr != null) {
                if (dr.read()) {
                    // DataRow dr = ds.Tables[0].Rows[i];
                    PriceEntered = Util.getValueOfDecimal(dr.get("chargeamt"));
                    //mTab.setValue("PriceEntered", Util.getValueOfDecimal(dr[0]));
                    //mTab.setValue("PriceActual", Util.getValueOfDecimal(dr[0]));
                    //mTab.setValue("PriceLimit", VIS.Env.ZERO);
                    //mTab.setValue("PriceList", VIS.Env.ZERO);
                    //mTab.setValue("Discount", VIS.Env.ZERO);
                }
            }
            mTab.setValue("PriceEntered", PriceEntered);
            //mTab.SetValue("PriceEntered", Utility.Util.GetValueOfDecimal(dr[0]));
            //mTab.SetValue("PriceActual", Utility.Util.GetValueOfDecimal(dr[0]));
            mTab.setValue("PriceActual", PriceEntered);
            mTab.setValue("PriceLimit", VIS.Env.ZERO);
            mTab.setValue("PriceList", VIS.Env.ZERO);
            mTab.setValue("Discount", VIS.Env.ZERO);
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            return err
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);

    };

    /// <summary>
    /// Order Line - Tax.
    /// - basis: Product, Charge, BPartner Location
    /// 	- sets C_Tax_ID
    /// 	Calles Amount
    /// 	@param ctx 
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>ull or error message</returns>
    CalloutOrder.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var column = mField.getColumnName();
            if (value == null)
                return "";
            if (steps) {
                this.log.warning("init");
            }
            //	Check Product
            var M_Product_ID = 0;
            if (column == "M_Product_ID") {
                M_Product_ID = Util.getValueOfInt(value);
            }
            else {
                M_Product_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_Product_ID"));
            }
            var C_Charge_ID = 0;
            if (column == "C_Charge_ID") {
                C_Charge_ID = Util.getValueOfInt(value);
            }
            else {
                C_Charge_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_Charge_ID"));
            }
            this.log.fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
            if (M_Product_ID == 0 && C_Charge_ID == 0) {
                this.setCalloutActive(false);
                return this.Amt(ctx, windowNo, mTab, mField, value);		//
            }
            //	Check Partner Location
            var shipC_BPartner_Location_ID = 0;
            if (column == "C_BPartner_Location_ID") {
                shipC_BPartner_Location_ID = Util.getValueOfInt(value);
            }
            else {
                shipC_BPartner_Location_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_BPartner_Location_ID"));
            }
            if (shipC_BPartner_Location_ID == 0) {
                this.setCalloutActive(false);
                return this.Amt(ctx, windowNo, mTab, mField, value);		// 
            }
            this.log.fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
            //DateTime billDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DateOrdered"));
            var billDate = ctx.getContext("DateOrdered");
            this.log.fine("Bill Date=" + billDate);
            //DateTime shipDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DatePromised"));
            var shipDate = ctx.getContext("DatePromised");
            this.log.fine("Ship Date=" + shipDate);
            var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
            this.log.fine("Org=" + AD_Org_ID);
            var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
            this.log.fine("Warehouse=" + M_Warehouse_ID);
            var billC_BPartner_Location_ID = ctx.getContextAsInt(windowNo, "Bill_Location_ID");
            if (billC_BPartner_Location_ID == 0)
                billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
            this.log.fine("Bill BP_Location=" + billC_BPartner_Location_ID);
            //var C_Tax_ID = VAdvantage.Model.Tax.get(ctx, M_Product_ID, C_Charge_ID, billDate, shipDate,
            //    AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
            //    "Y" == (ctx.getContext("IsSOTrx")));
            var isSotTrx = "Y" == ctx.getWindowContext(windowNo, "IsSOTrx", true);
            var paramString = M_Product_ID.toString() + "," + C_Charge_ID.toString() + "," + billDate.toString() + "," +
                               shipDate.toString() + "," + AD_Org_ID.toString() + "," + M_Warehouse_ID.toString() + "," + billC_BPartner_Location_ID.toString()
                               + "," + shipC_BPartner_Location_ID.toString() + ","
                               + isSotTrx.toString();
            var C_Tax_ID = VIS.dataContext.getJSONRecord("MTax/Get_Tax_ID", paramString);
            this.log.info("Tax ID=" + C_Tax_ID);
            //
            if (C_Tax_ID == 0) {
                //mTab.fireDataStatusEEvent(CLogger.retrieveError());
                // VIS.ADialog.info(VLogger.RetrieveError().toString(), true, "", "");
            }
            else {
                mTab.setValue("C_Tax_ID", Util.getValueOfInt(C_Tax_ID));
            }
            //
            if (steps) {
                this.log.warning("fini");
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    /// <summary>
    /// Order Line - Amount.
    /// - called from QtyOrdered, Discount and PriceActual
    /// - calculates Discount or Actual Amount
    /// - calculates LineNetAmt
    /// - enforces PriceLimit
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //////////;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {

            if (steps) {
                this.log.Warning("init");
            }

            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");

            //Get Qty information from server side
            var pStr = M_PriceList_ID.toString(); //1

            //Get product price information
            var dr;
            dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", M_PriceList_ID.toString());


            var StdPrecision = Util.getValueOfInt(dr["StdPrecision"]);
            var QtyEntered, QtyOrdered, PriceEntered, PriceActual, PriceLimit, Discount, PriceList;
            //	get values
            QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
            this.log.fine("QtyEntered=" + QtyEntered + ", Ordered=" + QtyOrdered + ", UOM=" + C_UOM_To_ID);
            //
            PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));

            Discount = Util.getValueOfDecimal(mTab.getValue("Discount"));
            PriceLimit = Util.getValueOfDecimal(mTab.getValue("PriceLimit"));
            PriceList = Util.getValueOfDecimal(mTab.getValue("PriceList"));
            this.log.fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Qty changed - recalc price
            if ((mField.getColumnName() == "QtyOrdered"
                || mField.getColumnName() == "QtyEntered"
                || mField.getColumnName() == "M_Product_ID")
                && !("N" == (ctx.getContext("DiscountSchema")))) {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                if (mField.getColumnName() == "QtyEntered") {
                    var paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString() + "," + QtyEntered.toString());

                    var dr = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);

                    QtyOrdered = dr;
                }

                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;
                var isSOTrx = (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y");


                var orderDate = mTab.getValue("DateOrdered");

                var paramStr = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                                   QtyOrdered.toString(), ",", //3
                                                                   isSOTrx, ",", //4 
                                                                   M_PriceList_ID.toString(), ",", //5
                                                                   "0,", //6
                                                                   orderDate.toString(), ",", null, ",", null); //7


                //Get product price information
                var pp = null;
                pp = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramStr);
                var stdPrice = pp.PriceStd;

                //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
                //      M_Product_ID, C_BPartner_ID, QtyOrdered, isSOTrx);

                // pp.SetM_PriceList_ID(M_PriceList_ID);
                //var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);

                //pp.SetPriceDate(date);
                //
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                    stdPrice.toString()); //3


                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                PriceEntered = drPC;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, pp.getPriceStd());

                if (PriceEntered == null)
                    PriceEntered = stdPrice;
                //
                this.log.fine("QtyChanged -> PriceActual=" + stdPrice
                    + ", PriceEntered=" + PriceEntered + ", Discount=" + pp.Discount);
                PriceActual = pp.PriceStd;
                mTab.setValue("PriceActual", PriceActual);
                mTab.setValue("Discount", pp.Discount);
                mTab.setValue("PriceEntered", PriceEntered);
                ctx.setContext(windowNo, "DiscountSchema", pp.DiscountSchema ? "Y" : "N");
            }
            else if (mField.getColumnName() == "PriceActual") {
                PriceActual = Util.getValueOfDecimal(value);

                //make parameter string
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                      PriceActual.toString()); //3

                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                PriceEntered = drPC;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);



                if (PriceEntered == null)
                    PriceEntered = PriceActual;
                //
                this.log.fine("PriceActual=" + PriceActual
                    + " -> PriceEntered=" + PriceEntered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
            else if (mField.getColumnName() == "PriceEntered") {
                PriceEntered = Util.getValueOfDecimal(value);

                //make parameter string
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                  PriceEntered.toString()); //3

                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                PriceActual = drPC;//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceEntered);
                if (PriceActual == null)
                    PriceActual = PriceEntered;
                //
                this.log.fine("PriceEntered=" + PriceEntered
                    + " -> PriceActual=" + PriceActual);
                mTab.setValue("PriceActual", PriceActual);
            }

            //  Discount entered - Calculate Actual/Entered
            if (mField.getColumnName() == "Discount") {
                //PriceActual = Util.getValueOfDecimal(((100.0 - Decimal.ToDouble(Discount.Value))
                //    / 100.0 * Decimal.ToDouble(PriceList.Value)));
                PriceActual = Util.getValueOfDecimal((100.0 - Discount)
                    / 100.0 * PriceList);

                //if (Env.Scale(PriceActual.Value) > StdPrecision)
                if (Util.scale(PriceActual) > StdPrecision)
                    //PriceActual = Decimal.Round(PriceActual.Value, StdPrecision);//, MidpointRounding.AwayFromZero);
                    PriceActual = PriceActual.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                     PriceActual.toString()); //3

                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                PriceEntered = drPC;// (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null)
                    PriceEntered = PriceActual;
                mTab.setValue("PriceActual", PriceActual);
                mTab.setValue("PriceEntered", PriceEntered);
            }
                //	calculate Discount
            else {
                if (PriceList == 0) {
                    Discount = VIS.Env.ZERO;
                }
                else {
                    //Discount = new Bigvar ((PriceList.doubleValue() - PriceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    //Discount = Util.getValueOfDecimal(((Decimal.ToDouble(PriceList.Value) - Decimal.ToDouble(PriceActual.Value)) / Decimal.ToDouble(PriceList.Value) * 100.0));
                    Discount = Util.getValueOfDecimal(((PriceList - PriceActual) / PriceList * 100.0));
                    if (isNaN(Discount)) {
                        this.setCalloutActive(false);
                        return "PriceListNotSelected";
                    }
                }
                //if (Discount.Scale() > 2)
                //   Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                Discount = Discount.toFixed(2);

                mTab.setValue("Discount", Discount);
            }
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Check PriceLimit
            var epl = ctx.getContext("EnforcePriceLimit");
            var enforce = (ctx.isSOTrx() && epl != null && epl == "Y");
            var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));
            if (enforce && (VIS.MRole.getDefault().IsOverwritePriceLimit() || isReturnTrx))
                enforce = false;

            //	Check Price Limit?
            if (enforce && PriceLimit != 0.0
              && PriceActual.compareTo(PriceLimit) < 0) {
                PriceActual = PriceLimit;

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                    PriceLimit.toString()); //3

                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                PriceEntered = drPC; //(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceLimit.Value);
                if (PriceEntered == null)
                    PriceEntered = PriceLimit;
                this.log.fine("(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                mTab.setValue("PriceActual", PriceLimit);
                mTab.setValue("PriceEntered", PriceEntered);
                //mTab.fireDataStatusEEvent("UnderLimitPrice", "", false);
                VIS.ADialog.info("UnderLimitPrice", true, "", "");
                //	Repeat Discount calc
                if (PriceList != 0) {
                    //Discount = Util.getValueOfDecimal(((Decimal.ToDouble(PriceList.Value) - Decimal.ToDouble(PriceActual.Value)) / Decimal.ToDouble(PriceList.Value) * 100.0));
                    Discount = Util.getValueOfDecimal(((PriceList - PriceActual) / PriceList * 100.0));
                    //if (Env.scale(Discount.Value) > 2)
                    //{

                    //    Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                    //}
                    Discount = Discount.toFixed(2);
                    mTab.setValue("Discount", Discount);
                }
            }

            //	Line Net Amt
            var LineNetAmt = QtyOrdered * PriceActual;

            if (Util.scale(LineNetAmt) > StdPrecision) {//LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision);//, MidpointRounding.AwayFromZero);
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);
            }
            this.log.info("LineNetAmt=" + LineNetAmt);
            mTab.setValue("LineNetAmt", LineNetAmt);

        }
        catch (err) {
            //MessageBox.Show("error in Amt" + ex.Message.toString());
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Line - Quantity.
    /// - called from C_UOM_ID, QtyEntered, QtyOrdered
    /// - enforces qty UOM relationship
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var paramStr = ""; //user for send and parameter value to controller Action
        //var U=Util;
        if (this.isCalloutActive() || value == null || value.toString() == "")
            return "";
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            if (steps) {
                this.log.Warning("init - M_Product_ID=" + M_Product_ID + " - ");
            }
            var QtyOrdered = VIS.Env.ZERO;
            var QtyEntered = VIS.Env.ZERO;
            var PriceActual, PriceEntered;

            // Check for RMA
            var isReturnTrx = "Y" == (ctx.getContext("IsReturnTrx"));

            //	No Product
            if (M_Product_ID == 0) {
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                QtyOrdered = QtyEntered;
                mTab.setValue("QtyOrdered", QtyOrdered);
            }
                //	UOM Changed - convert from Entered -> Product
            else if (mField.getColumnName() == "C_UOM_ID") {
                var C_UOM_To_ID = Util.getValueOfInt(value);
                QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));

                //Get precision from server side
                paramStr = C_UOM_To_ID.toString().concat(","); //1

                var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);



                //var QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.getPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
                var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));//, MidpointRounding.AwayFromZero);

                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }


                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                    QtyEntered.toString()); //3
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                QtyOrdered = pc;// (Decimal?) MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, QtyEntered.Value);


                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;
                var conversion = QtyEntered != QtyOrdered;
                PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                PriceActual.toString()); //3
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                PriceEntered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null)
                    PriceEntered = PriceActual;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                    + " -> " + conversion
                    + " QtyOrdered/PriceEntered=" + QtyOrdered + "/" + PriceEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyOrdered", QtyOrdered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
                //	QtyEntered changed - calculate QtyOrdered
            else if (mField.getColumnName() == "QtyEntered") {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyEntered = Util.getValueOfDecimal(value);

                //Get precision from server
                paramStr = C_UOM_To_ID.toString().concat(","); //1
                var gp = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramStr);


                //var QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.getPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
                var QtyEntered1 = QtyEntered.toFixed(Util.getValueOfInt(gp));//, MidpointRounding.AwayFromZero);

                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2

                   QtyEntered.toString()); //3
                var pc = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                QtyOrdered = pc;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, QtyEntered.Value);
                if (QtyOrdered == null)
                    QtyOrdered = QtyEntered;

                //var conversion = QtyEntered.Value.compareTo(QtyOrdered.Value) != 0;
                var conversion = QtyEntered != QtyOrdered;



                this.log.fine("UOM=" + C_UOM_To_ID
                        + ", QtyEntered=" + QtyEntered
                        + " -> " + conversion
                        + " QtyOrdered=" + QtyOrdered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");

                mTab.setValue("QtyOrdered", QtyOrdered);
            }
                //	QtyOrdered changed - calculate QtyEntered (should not happen)
            else if (mField.getColumnName() == "QtyOrdered") {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyOrdered = Util.getValueOfDecimal(value);

                paramStr = M_Product_ID.toString().concat(","); //1
                var gp = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramStr);

                var precision = gp;//MProduct.get(ctx, M_Product_ID).getUOMPrecision();

                var QtyOrdered1 = QtyOrdered.toFixed(precision);

                if (QtyOrdered != QtyOrdered1) {
                    this.log.fine("Corrected QtyOrdered Scale "
                        + QtyOrdered + "->" + QtyOrdered1);
                    QtyOrdered = QtyOrdered1;
                    mTab.setValue("QtyOrdered", QtyOrdered);
                }

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                      QtyOrdered.toString()); //3

                var pt = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                QtyEntered = pt//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //C_UOM_To_ID, QtyOrdered);
                if (QtyEntered == null)
                    QtyEntered = QtyOrdered;
                var conversion = QtyOrdered != QtyEntered;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", QtyOrdered=" + QtyOrdered
                    + " -> " + conversion
                    + " QtyEntered=" + QtyEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyEntered", QtyEntered);
            }
            else {
                //	QtyEntered = (Decimal)mTab.getValue("QtyEntered");
                QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
            }

            if (M_Product_ID != 0
               && isReturnTrx) {
                var inOutLine_ID = Util.getValueOfInt(mTab.getValue("Orig_InOutLine_ID"));
                if (inOutLine_ID != 0) {
                    paramStr = inOutLine_ID.toString();
                    var dr = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", paramStr);
                    var mq = dr["MovementQty"];
                    //MInOutLine inOutLine = new MInOutLine(ctx, inOutLine_ID, null);
                    //var retValue = inOutLine.GetMovementQty();
                    //MInOutLine inOutLine = new MInOutLine(ctx, inOutLine_ID, null);
                    var shippedQty = Util.getValueOfDecimal(mq);

                    QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                    if (shippedQty < QtyOrdered) {
                        if (ctx.isSOTrx()) {
                            //mTab.fireDataStatusEEvent("QtyShippedLessThanQtyReturned", shippedQty.toString(), false);
                            VIS.ADialog.info("QtyShippedAndReturned", null, shippedQty.toString(), "");
                        }
                        else {
                            // mTab.fireDataStatusEEvent("QtyReceivedLessThanQtyReturned", shippedQty.toString(), false);
                            VIS.ADialog.info("QtyRecievedAndReturnd", null, shippedQty.toString(), "");
                        }
                        mTab.setValue("QtyOrdered", shippedQty);
                        QtyOrdered = shippedQty;

                        var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");

                        paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                        QtyOrdered.toString()); //3

                        QtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);
                        // QtyEntered = pt.retValue;//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                        //C_UOM_To_ID, QtyOrdered);
                        if (QtyEntered == null)
                            QtyEntered = QtyOrdered;
                        mTab.setValue("QtyEntered", QtyEntered);
                        this.log.fine("QtyEntered : " + QtyEntered.toString() +
                                    "QtyOrdered : " + QtyOrdered.toString());
                    }
                }
            }

            //	Storage
            if (M_Product_ID != 0
                && ctx.isSOTrx()
                && QtyOrdered > 0
                && !isReturnTrx)		//	no negative (returns)
            {

                var pi = VIS.dataContext.getJSONRecord("MProduct/GetProduct", M_Product_ID.toString());
                //MProduct product = MProduct.get(ctx, M_Product_ID);
                var C_OrderLine_ID = 0;

                if (Util.getValueOfBoolean(pi.IsStocked)) {
                    var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                    var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");


                    //Decimal? available = MStorage.getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    //Get Qty information from server side
                    var paramString = M_Product_ID.toString().concat(",", M_Warehouse_ID.toString(), ",", //2
                                                           M_AttributeSetInstance_ID.toString(), ",", //3
                                                           C_OrderLine_ID.toString()); //4

                    //Get product price information
                    var dr = null;
                    var available = VIS.dataContext.getJSONRecord("MStorage/GetQtyAvailable", paramString);

                    // var available = dr.available;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI

                    if (available == null)
                        available = VIS.Env.ZERO;
                    if (available == 0) {
                        //mTab.fireDataStatusEEvent("NoQtyAvailable", "0", false);
                        //VIS.ADiathis.log.info("NoQtyAvailable", null, "0", "");
                    }
                    else if (available.toString().compareTo(QtyOrdered) < 0) {
                        // VIS.ADialog.info("InsufficientQtyAvailable", null, available.toString(), "");
                    }
                    else {

                        if (mTab.getValue("C_OrderLine_ID") == "") {
                            C_OrderLine_ID = 0;
                        }
                        else {
                            C_OrderLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                        }
                        if (C_OrderLine_ID == null)
                            C_OrderLine_ID = 0;

                        var notReserved = dr.notReserved;//MOrderLine.getNotReserved(ctx,
                        //M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                        //C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = VIS.Env.ZERO;
                        //var total = Decimal.Subtract(available.Value, notReserved);
                        var total = available - notReserved;
                        if (total < QtyOrdered) {
                            var info = VIS.Msg.parseTranslation(ctx, "@QtyAvailable@=" + available
                                + "  -  @QtyNotReserved@=" + notReserved + "  =  " + total);
                            VIS.ADialog.info("InsufficientQtyAvailable", null, info, "");
                        }
                    }
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Orig_Order - Orig Order Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_Order = function (ctx, windowNo, mTab, mField, value, oldValue) {

        
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var C_Order_ID = Util.getValueOfInt(value);
        if (C_Order_ID == null || C_Order_ID == 0)
            return "";
        this.setCalloutActive(true);
        try {
            // var U=Util;
            //	Get Details from Original Order
            //dr = VIS.dataContext.getJSONRecord("CalloutOrder/GetQtyInfo", paramString);
            var order = VIS.dataContext.getJSONRecord("MOrder/GetOrder", C_Order_ID.toString());
            //MOrder order = new MOrder(ctx, C_Order_ID, null);
            var bpartner = VIS.dataContext.getJSONRecord("MBPartner/GetBPartner", order["C_BPartner_ID"].toString());
            //MBPartner bpartner = new MBPartner(ctx, order.C_BPartner_ID, null);

            // Reset Orig Shipment
            mTab.setValue("Orig_InOut_ID", null);

            mTab.setValue("C_BPartner_ID", Util.getValueOfInt(order["C_BPartner_ID"]));
            mTab.setValue("C_BPartner_Location_ID", Util.getValueOfInt(order["C_BPartner_Location_ID"]));
            mTab.setValue("Bill_BPartner_ID", Util.getValueOfInt(order["Bill_BPartner_ID"]));
            mTab.setValue("Bill_Location_ID", Util.getValueOfInt(order["Bill_Location_ID"]));

            if (order["AD_User_ID"] != 0)
                mTab.setValue("AD_User_ID", Util.getValueOfInt(order["AD_User_ID"]));

            if (order["Bill_User_ID"] != 0)
                mTab.setValue("Bill_User_ID", Util.getValueOfInt(order["Bill_User_ID"]));

            if (ctx.isSOTrx())
                mTab.setValue("M_ReturnPolicy_ID", Util.getValueOfInt(bpartner["M_ReturnPolicy_ID"]));
            else
                mTab.setValue("M_ReturnPolicy_ID", Util.getValueOfInt(bpartne["PO_ReturnPolicy_ID"]));

            //mTab.setValue("DateOrdered", order.getDateOrdered());
            mTab.setValue("M_PriceList_ID", Util.getValueOfInt(order["M_PriceList_ID"]));
            mTab.setValue("PaymentRule", order["PaymentRule"]);
            mTab.setValue("C_PaymentTerm_ID", Util.getValueOfInt(order["C_PaymentTerm_ID"]));
            //mTab.setValue ("DeliveryRule", X_C_Order.DELIVERYRULE_Manual);

            mTab.setValue("Bill_Location_ID", Util.getValueOfInt(order["Bill_Location_ID"]));
            mTab.setValue("InvoiceRule", order["InvoiceRule"]);
            mTab.setValue("PaymentRule", order["PaymentRule"]);
            mTab.setValue("DeliveryViaRule", order["DeliveryViaRule"]);
            mTab.setValue("FreightCostRule", orde["FreightCostRule"]);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Orig_InOut - Shipment Line Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_InOut = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            // var U=Util;
            var Orig_InOut_ID = Util.getValueOfInt(value);
            if (Orig_InOut_ID == null || Orig_InOut_ID == 0)
                return "";
            this.setCalloutActive(true);
            //	Get Details from Original Shipment
            //MInOut io = new MInOut(ctx, Orig_InOut_ID, null);
            var io = VIS.dataContext.getJSONRecord("MInOut/GetInOut", Orig_InOut_ID.toString());
            mTab.setValue("C_Project_ID", Util.getValueOfInt(io.C_Project_ID));
            mTab.setValue("C_Campaign_ID", Util.getValueOfInt(io.C_Campaign_ID));
            mTab.setValue("C_Activity_ID", Util.getValueOfInt(io.C_Activity_ID));
            mTab.setValue("AD_OrgTrx_ID", Util.getValueOfInt(io.AD_OrgTrx_ID));
            mTab.setValue("User1_ID", Util.getValueOfInt(io.User1_ID));
            mTab.setValue("User2_ID", Util.getValueOfInt(io.User2_ID));
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Orig_Order - Orig Order Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_OrderLine = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try { //var U=Util;
            var Orig_OrderLine_ID = Util.getValueOfInt(value);
            if (Orig_OrderLine_ID == null || Orig_OrderLine_ID == 0)
                return "";
            this.setCalloutActive(true);

            //MOrderLine orderline = new MOrderLine(ctx, Orig_OrderLine_ID, null);
            var orderline = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", Orig_OrderLine_ID.toString());
            mTab.setValue("Orig_InOutLine_ID", null);
            mTab.setValue("C_Tax_ID", Util.getValueOfInt(orderline["C_Tax_ID"]));
            mTab.setValue("PriceList", Util.getValueOfDecimal(orderline["PriceList"]));
            mTab.setValue("PriceLimit", Util.getValueOfDecimal(orderline["PriceLimit"]));
            mTab.setValue("PriceActual", Util.getValueOfDecimal(orderline["PriceActual"]));
            mTab.setValue("PriceEntered", Util.getValueOfDecimal(orderline["PriceEntered"]));
            mTab.setValue("C_Currency_ID", Util.getValueOfInt(orderline["C_Currency_ID"]));
            mTab.setValue("Discount", Util.getValueOfDecimal(orderline["Discount"]));
            // mTab.setValue("Discount", Util.getValueOfDecimal(orderline.Discount));
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            var dr;
            dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", M_PriceList_ID.toString());


            var StdPrecision = Util.getValueOfInt(dr["StdPrecision"]);
            var QtyOrdered;
            QtyOrdered = Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));



            //	Line Net Amt
            var LineNetAmt = QtyOrdered * Util.getValueOfDecimal(orderline["PriceActual"]);

            if (Util.scale(LineNetAmt) > StdPrecision) {//LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision);//, MidpointRounding.AwayFromZero);
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);
            }
            this.log.info("LineNetAmt=" + LineNetAmt);
            mTab.setValue("LineNetAmt", LineNetAmt);
        }
        catch (err) {
            //MessageBox.Show("error in Orig_OrderLine");
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };

    /// <summary>
    /// Orig_InOutLine - Shipment Line Defaults.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrder.prototype.Orig_InOutLine = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        try {
            //var U=Util;
            var Orig_InOutLine_ID = Util.getValueOfInt(value);
            if (Orig_InOutLine_ID == null || Orig_InOutLine_ID == 0)
                return "";

            this.setCalloutActive(true);
            //	Get Details
            var Orig_InOutLine = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", Orig_InOutLine_ID.toString())
            //MInOutLine Orig_InOutLine = new MInOutLine(ctx, Orig_InOutLine_ID, null);

            if (Orig_InOutLine != null) {
                mTab.setValue("C_Project_ID", Util.getValueOfInt(Orig_InOutLine["C_Project_ID"]));
                mTab.setValue("C_Campaign_ID", Util.getValueOfInt(Orig_InOutLine["C_Campaign_ID"]));
                mTab.setValue("M_Product_ID", Util.getValueOfInt(Orig_InOutLine["M_Product_ID"]));
                mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(Orig_InOutLine["M_AttributeSetInstance_ID"]));
                mTab.setValue("C_UOM_ID", Util.getValueOfInt(Orig_InOutLine["C_UOM_ID"]));
            }
        }
        catch (err) {
            //MessageBox.Show("error in Orig_InOutLine");
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutOrder = CalloutOrder;
    //***CalloutOrder End


    //CalloutAssigment Start
    function CalloutAssignment() {
        VIS.CalloutEngine.call(this, "VIS.CalloutAssignment"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutAssignment, VIS.CalloutEngine);//inherit CalloutEngine

    /// <summary>
    /// Assignment_Product.
    /// - called from S_ResourceAssignment_ID
    /// - sets M_Product_ID, Description
    /// - Qty.. 
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>

    CalloutAssignment.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {


        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        //	get value
        var S_ResourceAssignment_ID = Util.getValueOfInt(value);
        if (S_ResourceAssignment_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);

        var M_Product_ID = 0;
        var name = null;
        var description = null;
        var Qty = null;
        var sql = "SELECT p.M_Product_ID, ra.Name, ra.Description, ra.Qty "
            + "FROM S_ResourceAssignment ra"
            + " INNER JOIN M_Product p ON (p.S_Resource_ID=ra.S_Resource_ID) "
            + "WHERE ra.S_ResourceAssignment_ID=" + S_ResourceAssignment_ID;
        var idr = null;
        try {
            idr = VIS.DB.executeReader(sql, null, null);
            if (idr.read()) {
                M_Product_ID = Util.getValueOfInt(idr.get(0));//.getInt (1);
                name = Util.getValueOfString(idr.get(1));//.getString(2);
                description = Util.getValueOfString(idr.get(2));//.getString(3);
                Qty = Util.getValueOfDecimal(idr.get(3));//.getBigDecimal(4);
            }
            idr.close();
        }
        catch (err) {
            if (idr != null) {
                idr.close();
                idr = null;
            }
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, "product", err);
        }

        this.log.fine("S_ResourceAssignment_ID=" + S_ResourceAssignment_ID + " - M_Product_ID=" + M_Product_ID);
        if (M_Product_ID != 0) {
            mTab.setValue("M_Product_ID", Util.getValueOfInt(M_Product_ID));
            if (description != null) {
                name += " (" + description + ")";
            }
            if ("." != name)//(!".".equals(name))
            {
                mTab.setValue("Description", name);
            }
            //
            var variable = "Qty";	//	TimeExpenseLine
            if (mTab.getTableName().startsWith("C_Order")) {
                variable = "QtyOrdered";
            }
            else if (mTab.getTableName().startsWith("C_Invoice")) {
                variable = "QtyInvoiced";
            }
            if (Qty != null) {
                mTab.setValue(variable, Qty);
            }
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutAssignment = CalloutAssignment;
    //CalloutAssigment End

    //**********CalloutCashJournal Start*************
    function CalloutCashJournal() {
        VIS.CalloutEngine.call(this, "VIS.CalloutCashJournal"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutCashJournal, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    ///  Cash Journal Line Invoice. when Invoice selected - set C_Currency,
    ///  DiscountAnt, Amount, WriteOffAmt
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutCashJournal.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive()) // assuming it is resetting value
        {
            return "";
        }
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        var C_Invoice_ID = Util.getValueOfInt(value);

        //------14-11-2014-------
        var dueAmount = 0;
        var C_InvoicePaySchedule_ID = 0;
        var _chk = 0;
        var _sqlAmt = "SELECT * FROM   (SELECT ips.C_InvoicePaySchedule_ID, "
        + " ips.DueAmt  FROM C_Invoice i  INNER JOIN C_InvoicePaySchedule ips "
        + " ON (i.C_Invoice_ID        =ips.C_Invoice_ID)  WHERE i.IsPayScheduleValid='Y' "
        + " AND ips.IsValid           ='Y'  AND ips.isactive          ='Y' "
        + " AND i.C_Invoice_ID    = " + C_Invoice_ID
        + "  AND ips.C_InvoicePaySchedule_ID NOT IN"
        + "(SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE c_payment_id IN"
        + "(SELECT NVL(c_payment_id,0) FROM C_InvoicePaySchedule)  union "
        + " SELECT NVL(C_InvoicePaySchedule_id,0) FROM C_InvoicePaySchedule  WHERE c_cashline_id IN"
        + "(SELECT NVL(c_cashline_id,0) FROM C_InvoicePaySchedule )) "
        + " ORDER BY ips.duedate ASC  ) WHERE rownum=1";
        var drAmt = null;
        try {
            drAmt = VIS.DB.executeReader(_sqlAmt, null, null);
            if (drAmt.read()) {
                C_InvoicePaySchedule_ID = Util.getValueOfInt(drAmt.get("c_invoicepayschedule_id"));
                mTab.setValue("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
                dueAmount = Util.getValueOfDecimal(drAmt.get("dueamt"));
                _chk = 1;
            }
            drAmt.close();
        }
        catch (err) {
            if (drAmt != null) {
                drAmt.close();
            }
            this.log.log(Level.SEVERE, _sqlAmt, err.message);
            return err.toString();
        }
        //-------------
        if (C_Invoice_ID == null || C_Invoice_ID == 0) {
            mTab.setValue("C_Currency_ID", null);
            this.setCalloutActive(false);
            return "";
        }

        // Date
        // var ts = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DateAcct"));
        //DateTime billDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DateOrdered"));
        var ts = new Date(ctx.getContext(windowNo, "DateAcct"));
        //DateTime ts = new DateTime(ctx.getContextAsTime(windowNo, "DateAcct")); // from
        // C_Cash
        var tsDate = "TO_DATE( '" + (Number(ts.getMonth()) + 1) + "-" + ts.getDate() + "-" + ts.getFullYear() + "', 'MM-DD-YYYY')";// GlobalVariable.TO_DATE(Util.GetValueOfDateTime(srchCtrls[i].Ctrl.getValue()), true);
        var sql = "SELECT C_BPartner_ID, C_Currency_ID, invoiceOpen(C_Invoice_ID, 0) as invoiceOpen, IsSOTrx, paymentTermDiscount(invoiceOpen(C_Invoice_ID, 0),C_Currency_ID,C_PaymentTerm_ID,DateInvoiced, " + tsDate
            + " ) as paymentTermDiscount,C_DocTypeTarget_ID FROM C_Invoice WHERE C_Invoice_ID=" + C_Invoice_ID;
        var idr = null;
        try {
            idr = VIS.DB.executeReader(sql, null, null);
            //pstmt.setTimestamp(1, ts);
            //pstmt.setInt(2, C_Invoice_ID.intValue());
            //ResultSet rs = pstmt.executeQuery();
            if (idr != null) {
                if (idr.read()) {
                    var payAmt = 0;
                    mTab.setValue("C_BPartner_ID", Util.getValueOfInt(idr.get("c_bpartner_id")));
                    mTab.setValue("C_Currency_ID", Util.getValueOfInt(idr.get("c_currency_id")));//.getInt(2)));
                    if (_chk == 0) {
                        payAmt = Util.getValueOfDecimal(idr.get("invoiceopen"));//.getBigDecimal(3);
                    }
                    else {
                        payAmt = (dueAmount) * (-1);
                    }
                    var discountAmt = Util.getValueOfDecimal(idr.get("paymenttermdiscount"));//.getBigDecimal(5);
                    var isSOTrx = "Y" == idr.get("issotrx");//.getString(4));
                    if (!isSOTrx) {
                        if (_chk == 0)//Pratap
                        {
                            payAmt = (payAmt) * (-1);
                        }
                        else//Pratap
                        {
                            payAmt = (dueAmount) * (-1);
                        }
                        discountAmt = (discountAmt) * (-1);//.negate();
                    }
                    // // Bharat
                    var doctype_ID = Util.getValueOfInt(idr.get("c_doctypetarget_id"));
                    var _qry = "SELECT DocBaseType FROM C_DocType WHERE C_DocType_ID = " + doctype_ID;
                    var docbaseType = Util.getValueOfString(VIS.DB.executeScalar(_qry));
                    if ("ARC" == docbaseType || "API" == docbaseType) {
                        mTab.setValue("VSS_PAYMENTTYPE", "P");
                    }
                    else {
                        mTab.setValue("VSS_PAYMENTTYPE", "R");
                    }
                    mTab.setValue("Amount", (payAmt - discountAmt));
                    mTab.setValue("DiscountAmt", discountAmt);
                    mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
                    ctx.setContext("InvTotalAmt", payAmt.toString());
                }
                idr.close();
            }
        }
        catch (err) {
            if (idr != null) {
                idr.close();
                idr = null;
            }
            this.log.log(Level.SEVERE, "invoice", err);
            this.setCalloutActive(false);
            return err.toString();
            //return e.getLocalizedMessage();
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Invoice Pay Schedule
    /// When Invoice Pay Schedule Selected
    /// The Amount Corresponding to that pay Schedule
    /// filled in Amount
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutCashJournal.prototype.SetAmount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        if (value == null || value.toString() == "") {
            return "";
        }
        if (value == null || value.toString() == "") {

            if (Util.getValueOfInt(mTab.getValue("C_Invoice_ID")) > 0) {
                this.setCalloutActive(true);
                var sql = "SELECT sum(ips.DueAmt)  FROM C_Invoice i INNER JOIN C_InvoicePaySchedule ips ON (i.C_Invoice_ID=ips.C_Invoice_ID) WHERE i.IsPayScheduleValid='Y' AND ips.IsValid ='Y' AND ips.isactive ='Y'" +
                    "AND i.C_Invoice_ID = " + Util.getValueOfInt(mTab.getValue("C_Invoice_ID")) + " AND C_InvoicePaySchedule_ID NOT IN (SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Payment_ID IN " +
                    "(SELECT NVL(C_Payment_ID,0) FROM C_InvoicePaySchedule) UNION SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE C_Cashline_ID IN (SELECT NVL(C_Cashline_ID,0) FROM C_InvoicePaySchedule))";
                var Amount = Util.getValueOfDecimal(VIS.DB.executeScalar(sql, null, null));
                ctx.setContext(windowNo, "InvTotalAmt", Amount.toString());
                mTab.setValue("Amount", Amount);
                this.setCalloutActive(false);
                return "";
            }
            else {
                return "";
            }
        }
        this.setCalloutActive(true);
        var C_InvoicePaySchedule_ID = Util.getValueOfInt(value);
        if (C_InvoicePaySchedule_ID == null || C_InvoicePaySchedule_ID == 0) {
            ctx.setContext(windowNo, "InvTotalAmt", null);
            mTab.setValue("Amount", null);
            this.setCalloutActive(false);
            return "";
        }
        var qry = "SELECT DueAmt FROM C_InvoicePaySchedule WHERE C_InvoicePaySchedule_ID=" + C_InvoicePaySchedule_ID;
        var Amt = Util.getValueOfDecimal(VIS.DB.executeScalar(qry, null, null));
        ctx.setContext(windowNo, "InvTotalAmt", Amt.toString());
        mTab.setValue("Amount", Amt);
        this.setCalloutActive(false);
        return "";
    }

    /// <summary>
    /// Cash Journal Line Invoice Amounts. when DiscountAnt, Amount, WriteOffAmt
    /// change making sure that add up to InvTotalAmt (created by
    /// CashJournal_Invoice)
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutCashJournal.prototype.Amounts = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //if (value == DBNull.Value || value == null || value.toString() == "")
        //{
        //    return "";
        //}
        // Needs to be Invoice
        if (this.isCalloutActive() || "I" != mTab.getValue("CashType")) {
            return "";
        }
        // Check, if InvTotalAmt exists
        var total = ctx.getContext("InvTotalAmt");
        if (total == null || total.toString().length == 0) {
            return "";
        }
        //Decimal invTotalAmt = new Decimal(total);
        var invTotalAmt = Util.getValueOfDecimal(total);
        this.setCalloutActive(true);

        var payAmt = Util.getValueOfDecimal(mTab.getValue("Amount"));
        var discountAmt = Util.getValueOfDecimal(mTab.getValue("DiscountAmt"));
        var writeOffAmt = Util.getValueOfDecimal(mTab.getValue("WriteOffAmt"));
        var overUnderAmt = Util.getValueOfDecimal(mTab.getValue("OverUnderAmt"));
        var colName = mField.getColumnName();
        this.log.fine(colName + " - Invoice=" + invTotalAmt + " - Amount=" + payAmt
                 + ", Discount=" + discountAmt + ", WriteOff=" + writeOffAmt);

        // Amount - calculate write off
        if (colName == "Amount") {
            var sub = invTotalAmt - payAmt;
            overUnderAmt = sub - discountAmt - writeOffAmt;
            //  writeOffAmt = Decimal.Subtract(Decimal.Subtract(invTotalAmt, payAmt), discountAmt);
            mTab.setValue("OverUnderAmt", overUnderAmt);
        }
        else // calculate PayAmt
        {
            sub = invTotalAmt - discountAmt;
            payAmt = sub - writeOffAmt - overUnderAmt;
            //payAmt = Decimal.Subtract(Decimal.Subtract(invTotalAmt, discountAmt), writeOffAmt);
            mTab.setValue("Amount", payAmt);
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Biginning balace calculation
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutCashJournal.prototype.BeginningBalCalc = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive())		// assuming it is resetting value
        {
            return "";
        }
        this.setCalloutActive(true);

        var C_Cash_ID = Util.getValueOfInt(value);
        if (C_Cash_ID == null || C_Cash_ID == 0) {
            mTab.setValue("BeginningBalance", 0);
            this.setCalloutActive(false);
            return "";
        }


        var C_CashBook_ID = Util.getValueOfInt(mTab.getValue("C_CashBook_ID"));
        var AD_Client_ID = Util.getValueOfInt(mTab.getValue("AD_Client_ID"));
        var AD_Org_ID = Util.getValueOfInt(mTab.getValue("AD_Org_ID"));

        // Date
        //var ts = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DateAcct"));
        var ts = ctx.getContext("DateAcct");
        //DateTime ts = new DateTime(ctx.getContextAsTime(windowNo, "DateAcct"));     // from
        // C_Cash
        var sql = "SELECT EndingBalance FROM C_Cash WHERE C_CashBook_ID=" + C_CashBook_ID + " AND" +
                " AD_Client_ID=" + AD_Client_ID + " AND AD_Org_ID=" + AD_Org_ID + " AND " +
                "c_cash_id IN (SELECT Max(c_cash_id) FROM C_Cash WHERE C_CashBook_ID=" + C_CashBook_ID
                + "AND AD_Client_ID=" + AD_Client_ID + " AND AD_Org_ID=" + AD_Org_ID + ") AND Processed='Y'";
        var idr = null;
        try {
            idr = VIS.DB.executeReader(sql, null, null);
            //pstmt.setInt(1, C_CashBook_ID.intValue());
            //pstmt.setInt(2, AD_Client_ID.intValue());
            //pstmt.setInt(3, AD_Org_ID.intValue());
            //pstmt.setInt(4, C_CashBook_ID.intValue());
            //pstmt.setInt(5, AD_Client_ID.intValue());
            //pstmt.setInt(6, AD_Org_ID.intValue());
            //ResultSet rs = pstmt.executeQuery();
            if (idr.read()) {
                var beginningBalance = Util.getValueOfDecimal(idr.get(0));//.getBigDecimal(1);
                mTab.setValue("BeginningBalance", beginningBalance);
            }
            else {
                //var zero = 0;
                mTab.setValue("BeginningBalance", 0);
            }

            idr.close();
        }
        catch (err) {
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, "Beginning balance", err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.toString();
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    CalloutCashJournal.prototype.SetCurrency = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            mTab.setValue("C_Currency_ID", 0);
            return "";
        }
        if (this.isCalloutActive())		// assuming it is resetting value
        {
            return "";
        }
        this.setCalloutActive(true);
        var C_CashBook_ID = Util.getValueOfInt(value);
        if (C_CashBook_ID == null || C_CashBook_ID == 0) {
            mTab.setValue("C_Currency_ID", 0);
            this.setCalloutActive(false);
            return "";
        }
        var paramString = C_CashBook_ID.toString();
        var cBook = VIS.dataContext.getJSONRecord("MCashBook/GetCashBook", paramString);
        // MCashBook cBook = new MCashBook(ctx, C_CashBook_ID, null);
        mTab.setValue("C_Currency_ID", cBook["C_Currency_ID"]);
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /// <summary>
    /// ConvertedAmt calculation
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutCashJournal.prototype.ConvertedAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive())		// assuming it is resetting value
        {
            return "";
        }
        this.setCalloutActive(true);
        var sql = "SELECT CASHLINE.Amount FROM C_CashLine CASHLINE INNER JOIN C_Cashbook CB ON (CB.C_Cashbook_ID=CASHLINE.C_Cashbook_ID) WHERE CASHLINE.C_CashLine_ID =" + value;
        try {
            var currTo = Util.getValueOfInt(VIS.DB.executeScalar("SELECT C_Currency_ID FROM C_CashBook WHERE C_CashBook_ID=(SELECT C_CashBook_ID FROM C_Cash WHERE C_Cash_ID=" + mTab.getValue("C_Cash_ID") + ")"));
            if (currTo == 0) {
                // ShowMessage.Info("PleaseSelectCashBook", true, null, null);
                this.setCalloutActive(false);
                return "";
            }
            var amt = Util.getValueOfDecimal(VIS.DB.executeScalar(sql));
            var CurrFrom = Util.getValueOfInt(VIS.DB.executeScalar("SELECT C_Currency_ID FROM C_Cashbook WHERE C_Cashbook_ID=" + mTab.getValue("C_Cashbook_ID")));
            var paramString = amt.toString() + "," + CurrFrom.toString() + "," + currTo.toString() + "," +
                                     ctx.getAD_Client_ID().toString() + "," + ctx.getAD_Org_ID().toString();

            //ConvertedAmt = VAdvantage.Model.MConversionRate.Convert(ctx,
            //    ConvertedAmt, Util.getValueOfInt(C_Currency_From_ID), C_Currency_To_ID,
            //    DateExpense, 0, AD_Client_ID, AD_Org_ID);
            var transferdAmt = VIS.dataContext.getJSONRecord("MConversionRate/Convert", paramString);
            //var transferdAmt = MConversionRate.Convert(Env.GetCtx(),amt,CurrFrom,currTo,
            //     Env.GetCtx().GetAD_Client_ID(), Env.GetCtx().GetAD_Org_ID());
            mTab.setValue("ConvertedAmt", transferdAmt);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };



    CalloutCashJournal.prototype.GetBeginBal = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value == "") {
            mTab.getField("C_Invoice_ID").setReadOnly(false);
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        //decimal BeginAmt = Util.GetValueOfDecimal(DB.ExecuteScalar(" select CompletedBalance from c_cashbook where c_cashbook_id=" + value));
        //tab.SetValue("BeginningBalance", BeginAmt);
        //Added By Manjot Changes Done to  Set Beginning Balance
        var TotalAmt = Util.getValueOfDecimal(VIS.DB.executeScalar(" select sum(nvl(CompletedBalance,0)) + sum(nvl(runningbalance,0)) as TotalBal from c_cashbook where c_cashbook_id=" + value));
        mTab.setValue("BeginningBalance", TotalAmt);

        this.setCalloutActive(false);
        return "";
    }


    VIS.Model.CalloutCashJournal = CalloutCashJournal;
    //********CalloutCashJournal End********

    //********** CalloutSetAttributeCode Start *****

    function CalloutSetAttributeCode() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetAttributeCode"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutSetAttributeCode, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutSetAttributeCode.prototype.FillAttribute = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var AttrCode = ctx.getContext(windowNo, "AttrCode");
        this.setCalloutActive(true);
        if (AttrCode != null && AttrCode != "") {
            var qry = "SELECT M_AttributeSet_ID FROM M_Product WHERE M_Product_ID = " + Util.getValueOfInt(value);
            var attributeSet_ID = Util.getValueOfInt(VIS.DB.executeScalar(qry));
            if (attributeSet_ID > 0) {
                qry = "SELECT M_AttributeSetInstance_ID FROM M_ProductAttributes WHERE UPC = " + AttrCode + " AND M_Product_ID = " + Util.getValueOfInt(value);
                var Attribute_ID = Util.getValueOfInt(VIS.DB.executeScalar(qry));
                if (Attribute_ID > 0) {
                    mTab.setValue("M_AttributeSetInstance_ID", Attribute_ID);
                }
            }
        }
        this.setCalloutActive(false);
        return "";
    }

    CalloutSetAttributeCode.prototype.FillUPC = function (ctx, windowNo, mTab, mField, value, oldValue) {
        var sql = "";
        var manu_ID = 0;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var AttrCode = ctx.getContext(windowNo, "AttrCode");
        this.setCalloutActive(true);
        if (!string.IsNullOrEmpty(AttrCode)) {
            sql = "SELECT Count(*) FROM M_Manufacturer WHERE IsActive = 'Y' AND UPC = '" + AttrCode + "'";
            manu_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            if (manu_ID > 0) {
                this.setCalloutActive(false);
                return VIS.Env.Msg.getMsg("Attribute Code Already Exist");
            }
            else {
                mTab.setValue("UPC", AttrCode);
            }
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutSetAttributeCode = CalloutSetAttributeCode;
    //********CalloutSetAttribute End********


    //********** CalloutProduct Start *****
    /// <summary>
    /// Product Callouts
    /// </summary>
    function CalloutProduct() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProduct"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProduct, VIS.CalloutEngine);//inherit CalloutEngine
    /**
     *	Product Category
     *  @param ctx context
     *  @param windowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutProduct.prototype.ProductCategory = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_Category_ID = Util.getValueOfInt(value);
        if (M_Product_Category_ID == null || Util.getValueOfInt(M_Product_Category_ID) == 0
            || M_Product_Category_ID == 0)
            return "";
        var paramString = Util.getValueOfInt(value);
        /**
         * Modified for update Book Qty on existing records.
         * Also checks the old asi and removes it if product has been change.
         */

        //Get MInventoryLine Information
        //Get product price information
        var dr = null;
        dr = VIS.dataContext.getJSONRecord("MProductCategory/GetProductCategory", paramString);

        //var M_Product_ID = dr.M_Product_ID;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI
        //var M_Locator_ID=dr.M_Locator_ID;     
        var IsPurchasedToOrder = dr.IsPurchasedToOrder;
        //  MProductCategory pc = new MProductCategory(ctx, M_Product_Category_ID, null);
        mTab.setValue("IsPurchasedToOrder", IsPurchasedToOrder);
        pc = null;
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    CalloutProduct.prototype.UOM = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        if (mTab.getValue("M_Product_id") == null) {
            return "";
        }
        this.setCalloutActive(true);

        // 26-09-2011 Lokesh Chauhan

        // If values are in The Transaction Tab then restrict user so that UOM can't be changed or set to what it was previously.
        var sql = "select count(*) from m_transaction where m_product_id = " + Util.getValueOfInt(mTab.getValue("M_Product_id"));
        var result = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));

        if (result > 0) {
            //ShowMessage.Info("Can't change UOM due to Transactions happens based on existing UOM", true, null, null);
            VIS.ADialog.info("Can't change UOM due to Transactions happens based on existing UOM");
            sql = "select c_uom_id from m_product where m_product_id =  " + Util.getValueOfInt(mTab.getValue("M_Product_id"));
            var uom_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            mTab.setValue("C_UOM_ID", uom_ID);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *	Resource Group
     *  @param ctx context
     *  @param windowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutProduct.prototype.ResourceGroup = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var resgrp = Util.getValueOfString(value);
        if (resgrp == null || resgrp.length == 0)
            return "";

        if ("O" == resgrp)
            mTab.setValue("BasisType", null);
        else
            mTab.setValue("BasisType", "I");
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *  Organization
     *  @param ctx context
     *  @param windowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutProduct.prototype.Organization = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var AD_Org_ID = Util.getValueOfInt(value);

        if (AD_Org_ID == null) {
            return "";
        }
        var dr = null;
        dr = VIS.dataContext.getJSONRecord("MLocator/GetLocator", paramString);

        var Default_Locator_ID = dr["Default_Locator_ID"];//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI


        // MLocator defaultLocator = MLocator.GetDefaultLocatorOfOrg(ctx, AD_Org_ID);
        // if (defaultLocator != null)
        // {
        mTab.setValue("M_Locator_ID", Default_Locator_ID);
        //}
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutProduct = CalloutProduct;
    //**********CalloutProduct End ******

    //************CalloutProject Start *******
    function CalloutProject() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProject"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProject, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Project Line Planned - Price + Qty.
    //- called from PlannedPrice, PlannedQty, PriceList, Discount
    //- calculates PlannedAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Gridfield</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutProject.prototype.Planned = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null) {
            return "";
        }
        this.setCalloutActive(true);

        var PlannedQty, PlannedPrice, PriceList, Discount;
        var RemainingMargin = 0;
        var StdPrecision = ctx.getStdPrecision();

        //	get values
        var id = Util.getValueOfInt(mTab.getValue("C_ProjectTask_ID"));
        var Sql = "SELECT C_Project_ID FROM C_ProjectPhase WHERE C_ProjectPhase_id IN (select C_ProjectPhase_id from" +
                    " C_ProjectTask WHERE C_ProjectTask_ID=" + id + ")";
        var projID = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));

        if (id == 0) {
            projID = Util.getValueOfInt(mTab.getValue("C_Project_ID"));
        }

        var query = "SELECT PriceLimit FROM M_ProductPrice WHERE M_PriceList_Version_ID = (SELECT c.m_pricelist_version_id FROM  c_project c WHERE c.c_project_Id=" + projID + ")  AND M_Product_id=" + Util.getValueOfInt(mTab.getValue("M_Product_ID"));
        var PriceLimit = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));

        PlannedQty = Util.getValueOfDecimal(mTab.getValue("PlannedQty"));
        if (PlannedQty == null) {
            PlannedQty = Envs.ONE;
        }
        else {

            RemainingMargin = (Util.getValueOfDecimal(mTab.getValue("PlannedPrice")) - PriceLimit) * Util.getValueOfDecimal(mTab.getValue("PlannedQty"));
        }


        PlannedPrice = Util.getValueOfDecimal(mTab.getValue("PlannedPrice"));
        if (PlannedPrice == null) {
            PlannedPrice = VIS.Env.ZERO;
        }
        PriceList = mTab.getValue("PriceList");
        if (PriceList == null) {
            PriceList = PlannedPrice;
        }
        Discount = mTab.getValue("Discount");
        if (Discount == null) {
            Discount = VIS.Env.ZERO;
        }

        var columnName = mField.getColumnName();
        if (columnName == "PlannedPrice") {
            if (PriceList == 0) {
                Discount = VIS.Env.ZERO;
            }
            else {
                //Decimal multiplier = PlannedPrice.multiply(Env.ONEHUNDRED)
                //.divide(PriceList, StdPrecision, Decimal.ROUND_HALF_UP);
                //var multiplier = Decimal.Round(Decimal.Divide(Decimal.Multiply(PlannedPrice.Value, Utility.Env.ONEHUNDRED),
                //   PriceList.Value), StdPrecision);//, MidpointRounding.AwayFromZero);

                var multiplier = ((PlannedPrice * VIS.Env.ONEHUNDRED) /
                   PriceList).toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
                //Discount = Env.ONEHUNDRED.subtract(multiplier);
                //Discount = Decimal.Subtract(VIS.Env.ONEHUNDRED, multiplier);
                Discount = (VIS.Env.ONEHUNDRED - multiplier);
            }
            mTab.setValue("Discount", Discount);
            this.log.fine("PriceList=" + PriceList + " - Discount=" + Discount
                 + " -> [PlannedPrice=" + PlannedPrice + "] (Precision=" + StdPrecision + ")");
        }
        else if (columnName == "PriceList") {
            if (VIS.Env.signum(PriceList) == 0) {
                Discount = VIS.Env.ZERO;
            }
            else {
                var multiplier = ((PlannedPrice * VIS.Env.ONEHUNDRED) /
                    PriceList).toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
                //Discount = Env.ONEHUNDRED.subtract(multiplier);
                Discount = VIS.Env.ONEHUNDRED - multiplier;
            }
            mTab.setValue("Discount", Discount);
            this.log.fine("[PriceList=" + PriceList + "] - Discount=" + Discount
                 + " -> PlannedPrice=" + PlannedPrice + " (Precision=" + StdPrecision + ")");
        }
        else if (columnName == "Discount") {
            var multiplier = (Discount / VIS.Env.ONEHUNDRED).toFixed(10);//, MidpointRounding.AwayFromZero);

            multiplier = VIS.Env.ONE - multiplier;
            //
            PlannedPrice = PriceList * multiplier;
            if (Util.scale(PlannedPrice) > StdPrecision) {
                PlannedPrice = PlannedPrice.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
            }
            mTab.setValue("PlannedPrice", PlannedPrice);
            this.log.fine("PriceList=" + PriceList + " - [Discount=" + Discount
                 + "] -> PlannedPrice=" + PlannedPrice + " (Precision=" + StdPrecision + ")");
        }

        //	Calculate Amount
        var PlannedAmt = PlannedQty * PlannedPrice;
        if (Util.scale(PlannedAmt) > StdPrecision) {
            //PlannedAmt = PlannedAmt.setScale(StdPrecision,Decimal.ROUND_HALF_UP);
            PlannedAmt = PlannedAmt.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);

        }
        //
        this.log.fine("PlannedQty=" + PlannedQty + " * PlannedPrice=" + PlannedPrice + " -> PlannedAmt=" + PlannedAmt + " (Precision=" + StdPrecision + ")");
        mTab.setValue("PlannedAmt", PlannedAmt);
        mTab.setValue("PlannedMarginAmt", (RemainingMargin));
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    //	planned

    /// <summary>
    /// Project Line Product
    //- called from Product
    //- calculates PlannedPrice, PriceList, Discount
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="mTab">grid tab</param>
    /// <param name="mField">grid field</param>
    /// <param name="value">new valuw</param>
    /// <returns>null or error message</returns>
    CalloutProject.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = Util.getValueOfInt(value);
        var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
        if (M_Product_ID == null || Util.getValueOfInt(M_Product_ID) == 0
            || M_PriceList_Version_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);

        var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
        var Qty = Util.getValueOfDecimal(mTab.getValue("PlannedQty"));
        var IsSOTrx = true;
        //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
        //    Util.getValueOfInt(M_Product_ID), C_BPartner_ID, Qty, IsSOTrx);
        //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
        var date = Util.getValueOfDateTime(mTab.getValue("PlannedDate"));
        //DateTime date = (DateTime)mTab.getValue("PlannedDate");
        if (date == null) {
            date = Util.getValueOfDateTime(mTab.getValue("DateContract"));
            if (date == null) {
                date = Util.getValueOfDateTime(mTab.getValue("DateFinish"));
                if (date == null) {
                    //date = new DateTime(System.currentTimeMillis());
                    date = new Date();
                    //date = new DateTime(CommonFunctions.CurrentTimeMillis());// (DateTime)(Util.getValueOfDateTime(CommonFunctions.CurrentTimeMillis()));
                }
            }
            //pp.SetPriceDate(date);
            ////
            //var PriceList = pp.GetPriceList();
            //mTab.setValue("PriceList", PriceList);
            //var PlannedPrice = pp.GetPriceStd();
            //mTab.setValue("PlannedPrice", PlannedPrice);
            //var Discount = pp.GetDiscount();
            //mTab.setValue("Discount", Discount);
            ////
            //var curPrecision = 2;
            //var PlannedAmt = pp.GetLineAmt(curPrecision);
            //mTab.setValue("PlannedAmt", PlannedAmt);


            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                               Qty.toString(), ",", //3
                                                               isSOTrx, ",", //4 
                                                               M_PriceList_ID.toString(), ",", //5
                                                               M_PriceList_Version_ID.toString(), ",", //6
                                                               date.toString(), ",",//7
                                                               null); //8





            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


            var rowDataDB = null;


            // MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //     M_Product_ID, C_BPartner_ID, Qty, isSOTrx);


            //		
            mTab.setValue("PriceList", dr["PriceList"]);
            // mTab.setValue("PriceLimit", dr.PriceLimit);
            mTab.setValue("PlannedPrice", dr.PriceActual);
            //mTab.setValue("PriceEntered", dr.PriceEntered);
            //  mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
            mTab.setValue("Discount", dr.Discount);
            mTab.setValue("PlannedAmt", dr.LineAmt);


            //	
            //this.log.fine("PlannedQty=" + Qty + " * PlannedPrice=" + PlannedPrice + " -> PlannedAmt=" + PlannedAmt);
            return "";
        }	//	product
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	CalloutProject

    VIS.Model.CalloutProject = CalloutProject;
    //************CalloutProject End ******

    //************Callout CalloutBankStatement*************//
    function CalloutBankStatement() {
        VIS.CalloutEngine.call(this, "VIS.CalloutBankStatement"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutBankStatement, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Bank Account Changed.
    /// Update Beginning Balance
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutBankStatement.prototype.BankAccount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }

        try {
            var C_BankAccount_ID = value;//).intValue();

            var paramString = value.toString();


            //Get BankAccount information
            var balance;
            balance = VIS.dataContext.getJSONRecord("MBankStatement/GetCurrentBalance", paramString);


            //dr = jQuery.parseJSON(VIS.dataContext.getJSONRecord("MBankStatement/GetBankAccount", paramString));
            //MBankAccount ba = MBankAccount.Get(ctx, C_BankAccount_ID);
            mTab.setValue("BeginningBalance", balance);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, "", err);

        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// BankStmt - Amount.
    /// Calculate ChargeAmt = StmtAmt - TrxAmt - InterestAmt
    /// or id Charge is entered - InterestAmt = StmtAmt - TrxAmt - ChargeAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutBankStatement.prototype.Amount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);

        //  Get Stmt & Trx
        var stmt = mTab.getValue("StmtAmt");
        if (stmt == null) {
            stmt = 0;
        }
        var trx = mTab.getValue("TrxAmt");
        if (trx == null) {
            trx = 0;
        }
        var bd = stmt - trx;
        //  Charge - calculate Interest
        if (mField.getColumnName() == "ChargeAmt") {
            var charge = value;
            if (charge == null) {
                charge = 0;
            }
            //bd = Decimal.Subtract(bd, charge);
            bd = bd - charge;
            //log.trace(log.l5_DData, "Interest (" + bd + ") = Stmt(" + stmt + ") - Trx(" + trx + ") - Charge(" + charge + ")");
            mTab.setValue("InterestAmt", bd);
        }
            //  Calculate Charge
        else {
            var interest = mTab.getValue("InterestAmt");
            if (interest == null) {
                interest = 0;
            }
            //  bd = Decimal.Subtract(bd, interest);
            bd = bd - interest;
            //log.trace(log.l5_DData, "Charge (" + bd + ") = Stmt(" + stmt + ") - Trx(" + trx + ") - Interest(" + interest + ")");
            mTab.setValue("ChargeAmt", bd);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// BankStmt - Payment.
    /// Update Transaction Amount when payment is selected
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutBankStatement.prototype.Payment = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Payment_ID = value;
        if (C_Payment_ID == null || C_Payment_ID == 0) {
            return "";
        }
        //
        var stmt = mTab.getValue("StmtAmt");
        if (stmt == null) {
            stmt = 0;
        }

        var sql = "SELECT PayAmt FROM C_Payment_v WHERE C_Payment_ID=@C_Payment_ID";		//	1
        var dr = null;
        var param = [];
        try {
            param[0] = new VIS.DB.SqlParam("@C_Payment_ID", C_Payment_ID);
            dr = VIS.DB.executeReader(sql, param, null);
            if (dr.read())/// if (rs.next())
            {
                var bd = dr.get("payamt");// rs.getBigDecimal(1);
                mTab.setValue("TrxAmt", bd);
                if (stmt == 0) {
                    mTab.setValue("StmtAmt", bd);
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.log(Level.SEVERE, "BankStmt_Payment", err);
            //ErrorLog.FillErrorLog("BankStmt_Payment", sql, e.Message.toString(), VAdvantage.Framework.Message.MessageType.ERROR);
            //return e.getLocalizedMessage();
            return err.toString();
        }
        //  Recalculate Amounts
        this.Amount(ctx, windowNo, mTab, mField, value);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutBankStatement = CalloutBankStatement;
    //*****CalloutBankStatement End**********

    //*******CalloutOfferSerIncluded Start ****

    function CalloutOfferSerIncluded() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOfferSerIncluded"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOfferSerIncluded, VIS.CalloutEngine);//inherit CalloutEngine
    CalloutOfferSerIncluded.prototype.CalculatePrice = function (number, price, rebate) {
        return Decimal.Subtract(Decimal.Multiply(number, price), rebate);
    };
    CalloutOfferSerIncluded.prototype.CalculateRebate = function (number, price, rebate) {
        return Decimal.Multiply(Decimal.Multiply(number, price), Decimal.Multiply(new Decimal(0.01), rebate));

        //return (new Decimal(number),price,arebate);
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *   if service is selected then UOM will be displayed
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.OfferServices = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var FO_SERVICE_ID = Util.getValueOfInt(value);
        if (FO_SERVICE_ID == null || FO_SERVICE_ID == 0)
            return "";
        var sql = "select uom.FO_HOTEL_UOM_ID,s.DESCRIPTION " +
                   "from FO_HOTEL_UOM uom " +
                   "inner join FO_SERVICE s ON(uom.FO_HOTEL_UOM_ID= s.FO_HOTEL_UOM_ID) " +
                   "where s.FO_SERVICE_ID=@FO_SERVICE_ID ";

        var FO_HOTEL_UOM_ID = 0;
        var APRICE = new Decimal(0);
        var CG1PRICE = new Decimal(0);
        var CG2PRICE = new Decimal(0);
        var cal, cal1, cal2, grand_total;
        var calrebate, cg1calrebate, cg2calrebate;
        var UOM1, UOM2;
        var dr = null;
        try {
            //PreparedStatement pst = DataBase.prepareStatement(sql,null);
            //pst.setInt(1,FO_SERVICE_ID);
            //ResultSet rs = pst.executeQuery();
            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            param[0] = new VIS.DB.SqlParam("@FO_SERVICE_ID", FO_SERVICE_ID);
            dr = VIS.DB.executeReader(sql, param, null);

            while (dr.read()) {
                FO_HOTEL_UOM_ID = Util.getValueOfInt(dr[0]);
                mTab.setValue("AUOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("CG1UOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("CG2UOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("DESCRIPTION", Util.getValueOfString(dr[1]));//rs.getString(2));
            }
            dr.close();
            //rs.close();
            //pst.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        var sqlprice = "select PRICE,CHILDGROUP1,CHILDGROUP2,UOM1,UOM2 from FO_SERVICE_PRICE_PRICELINE " +
                        "where CREATED=(select max(CREATED) from FO_SERVICE_PRICE_PRICELINE " +
                        "where FO_SERVICE_ID=@FO_SERVICE_ID)";
        try {
            //PreparedStatement pst1 = DataBase.prepareStatement(sqlprice,null);
            //pst1.setInt(1,FO_SERVICE_ID);
            //ResultSet rs1 = pst1.executeQuery();
            var param = [];
            // SqlParameter[] param = new VIS.DB.SqlParam[1];
            param[0] = new VIS.DB.SqlParam("@FO_SERVICE_ID", FO_SERVICE_ID);
            dr = VIS.DB.executeReader(sqlprice, param, null);

            while (dr.read()) {
                APRICE = Util.getValueOfDecimal(dr[0]);
                CG1PRICE = Util.getValueOfDecimal(dr[1]);
                CG2PRICE = Util.getValueOfDecimal(dr[2]);
                //code changed by sandeep for is%
                UOM1 = dr[3].toString();
                UOM2 = dr[4].toString();
                if (UOM1.equals("Y")) {
                    //CG1PRICE=APRICE.multiply(CG1PRICE).multiply(new BigDecimal(0.01));
                    CG1PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG1PRICE), new Decimal(0.01));
                }
                if (UOM2.equals("Y")) {
                    CG2PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG2PRICE), new Decimal(0.01));
                    //CG2PRICE=APRICE.multiply(CG2PRICE).multiply(new BigDecimal(0.01));
                }

                //code changes end
                mTab.setValue("APRICE", APRICE);
                mTab.setValue("CG1PRICE", CG1PRICE);
                mTab.setValue("CG2PRICE", CG2PRICE);
            }
            dr.close();
            //rs1.close();
            //pst1.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var aprice = Util.getValueOfDecimal(mTab.getValue("AP0RICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1P0RICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var miltodate=atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate=afromDate.GetTime();
        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        var t = afromDate.Subtract(atoDate);
        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }

        var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
        var secdiff = t.TotalSeconds;// mildiff / 1000;
        var mindiff = t.TotalMinutes;// secdiff / 60;

        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    // grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
                    break;
            }


        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *    Check todate with fromdate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.tochkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var a = Util.getValueOfDateTime(value);
        if (a == null || a.equals(0))
            return "";
        var atoDate = Util.getValueOfDateTime(value);
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        // var milfromdate = afromDate.GetTime();
        try {
            atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
            //   var miltodate = atoDate.GetTime();
            var numdays;
            numdays = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
            if (numdays <= 0) {

                ////////////////////////////////////////////////
                var incdays;
                incdays = Utility.TimeUtil.AddDays(afromDate, 1);
                mTab.setValue("ADATETO", incdays);
                //////////////////////////////////////////////
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                var t = atoDate.Subtract(afromDate);
                var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
                var secdiff = t.TotalMinutes;// mildiff / 1000;
                var mindiff = t.TotalMinutes;// secdiff / 60;
                var monthact;
                var weekact;
                var monthquo = daydiff / 30;
                var monthrem = daydiff % 30;
                var weekquo = daydiff / 7;
                var weekrem = daydiff % 7;
                if (monthrem == 0) {
                    monthact = monthquo;
                }
                else {
                    monthact = monthquo + 1;
                }

                if (weekrem == 0) {
                    weekact = weekquo;
                }
                else {
                    weekact = weekquo + 1;
                }
                var setafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", setafromDate);
                mTab.setValue("CG2DATEFROM", setafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();
                if (uomsel == "" || uomsel.toString().trim().length == 0) {
                    uomsel = "0";
                }
                var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
                var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
                var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));

                if (Util.getValueOfInt(arebate) == 0) {
                    calrebate = VIS.Env.ZERO;
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            cal1 = cg1price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007:// per month
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);

                            break;
                        case 1000008://per week
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week 	
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
                            break;
                    }
                }
                else {
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007://per month 
                            calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000008://per week
                            calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week  
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
                            break;
                    }

                }
                if (anumber == 0)//update code on 11-09-2009 by sandeep
                {
                    mTab.setValue("ATOTAL_AMOUNT", 0);
                    mTab.setValue("GRAND_TOTAL", 0);
                }
                if (cg1number == 0) {
                    mTab.setValue("CG1TOTAL_AMOUNT", 0);

                }
                if (cg2number == 0) {
                    mTab.setValue("CG2TOTAL_AMOUNT", 0);
                }
            }
        }

        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //e.getLocalizedMessage();
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *    Check fromdate with todate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.fromchkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var b = Util.getValueOfDateTime(value);
        if (b == null || b.equals(0))
            return "";
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        //var miltodate=atoDate.GetTime();
        try {
            afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
            //  var milfromdate=afromDate.GetTime();
            if (afromDate.compareTo(atoDate) > 0) {
                //				Object ob[]={"ok"};
                //				JOptionPane.showOptionDialog(new JFrame(),"FromDate cannot appear after ToDate","FO",0,JOptionPane.ERROR_MESSAGE,null,ob,ob[0]);
                //				mTab.setValue("ADATEFROM",null);
                //				mTab.setValue("CG1DATEFROM",null);
                //				mTab.setValue("CG2DATEFROM",null);
                mTab.setValue("CG1DATEFROM", afromDate);
                mTab.setValue("CG2DATEFROM", afromDate);
                ////////////////////////////////////////////////			
                var incdays;
                incdays = Utility.timeUtil.addDays(afromDate, 1);
                mTab.setValue("ADATETO", incdays);
                //////////////////////////////////////////////	
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                var t = atoDate.Subtract(afromDate);
                var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
                var secdiff = t.TotalSeconds;// mildiff / 1000;
                var mindiff = t.TotalMinutes;// secdiff / 60;
                var monthact;
                var weekact;
                var monthquo = daydiff / 30;
                var monthrem = daydiff % 30;
                var weekquo = daydiff / 7;
                var weekrem = daydiff % 7;
                if (monthrem == 0) {
                    monthact = monthquo;
                }
                else {
                    monthact = monthquo + 1;
                }

                if (weekrem == 0) {
                    weekact = weekquo;
                }
                else {
                    weekact = weekquo + 1;
                }
                var setafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", setafromDate);
                mTab.setValue("CG2DATEFROM", setafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();
                if (uomsel == "" || uomsel.toString().trim().length == 0) {
                    uomsel = "0";
                }
                var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
                var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
                var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));

                if (Util.getValueOfInt(arebate) == 0) {
                    calrebate = VIS.Env.ZERO;
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            cal1 = cg1price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007:// per month
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);

                            break;
                        case 1000008://per week
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week 	
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
                            break;
                    }
                }
                else {
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007://per month 
                            calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000008://per week
                            calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week  
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            // System.out.println("false");
                            break;
                    }


                }
                if (anumber == 0)//update code on 11-09-2009 by sandeep
                {
                    mTab.setValue("ATOTAL_AMOUNT", 0);
                    mTab.setValue("GRAND_TOTAL", 0);
                }
                if (cg1number == 0) {
                    mTab.setValue("CG1TOTAL_AMOUNT", 0);

                }
                if (cg2number == 0) {
                    mTab.setValue("CG2TOTAL_AMOUNT", 0);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //e.getLocalizedMessage();
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/

    /*****************************************************************************
     *   if Adult UOM is selected the same will be displayed in child records
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.UOMSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;

        //log.severe("value"+value);
        var UOMsel = Util.getValueOfInt(value);
        UOMsel = Util.getValueOfInt(mTab.getValue("AUOM_ID"));
        if (UOMsel == null)
            return "";
        mTab.setValue("CG1UOM_ID", UOMsel);
        mTab.setValue("CG2UOM_ID", UOMsel);
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var miltodate=atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate=afromDate.GetTime();
        var t = atoDate.Subtract(afromDate);
        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
        var secdiff = t.TotalSeconds;// mildiff / 1000;
        var mindiff = t.TotalMinutes;// secdiff / 60;
        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }

        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
                    break;
            }


        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *   if Adult Number is selected then the calculation is done
     ****************************************************************************/
    CalloutOfferSerIncluded.prototype.AVALSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var stdprice = 24;//Standard Price
        //			if (isCalloutActive())
        //				return"";
        //			setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var uomsel = null;
        try {
            uomsel = mTab.getValue("AUOM_ID").toString();
        }
        catch (err) {
            this.setCalloutActive(false);
            return "";
        }
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        //var miltodate=atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate=afromDate.GetTime();
        var t = atoDate.Subtract(afromDate);
        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);

        var mildiff = t.TotalMilliseconds;// miltodate - milfromdate;
        var secdiff = t.TotalSeconds;// mildiff / 1000;
        var mindiff = t.TotalMinutes;// secdiff / 60;
        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }

        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal((daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal((daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(stdprice * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(stdprice * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(stdprice * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(stdprice * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(stdprice * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    // System.out.println("false");
                    break;
            }


        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //						setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutOfferSerIncluded = CalloutOfferSerIncluded;
    //******* CalloutOfferSerIncluded End***** 


    //********* CalloutOfferServices Start *******
    function CalloutOfferServices() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOfferServices"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOfferServices, VIS.CalloutEngine);//inherit CalloutEngine
    CalloutOfferServices.prototype.CalculatePrice = function (number, price, rebate) {
        return Decimal.Subtract(Decimal.Multiply(number, price), rebate);
    };
    CalloutOfferServices.prototype.CalculateRebate = function (number, price, rebate) {
        return Decimal.Multiply(Decimal.Multiply(number, price), Decimal.Multiply(new Decimal(0.01), rebate));
        //return (new Decimal(number),price,arebate);
    };
    /************************************************************************
 *  Service Selected
 *
 *  @param ctx      Context
 *  @param windowNo current Window No
 *  @param mTab     Model Tab
 *  @param mField   Model Field
 *  @param value    The new value
 *  @return Error message or ""
 ************************************************************************/
    /*****************************************************************************
     *   if service is selected then UOM will be displayed
     ****************************************************************************/
    CalloutOfferServices.prototype.OfferServices = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var FO_SERVICE_ID = Util.getValueOfInt(value);
        if (FO_SERVICE_ID == null || FO_SERVICE_ID == 0)
            return "";
        var sql = "select uom.FO_HOTEL_UOM_ID,s.DESCRIPTION " +
                   "from FO_HOTEL_UOM uom " +
                   "inner join FO_SERVICE s ON(uom.FO_HOTEL_UOM_ID= s.FO_HOTEL_UOM_ID) " +
                   "where s.FO_SERVICE_ID=@FO_SERVICE_ID ";
        var FO_HOTEL_UOM_ID = 0;
        var APRICE = new Decimal(0);
        var CG1PRICE = new Decimal(0);
        var CG2PRICE = new Decimal(0);
        var cal, cal1, cal2, grand_total;
        var calrebate, cg1calrebate, cg2calrebate;
        var UOM1, UOM2;
        var dr = null;
        try {
            //PreparedStatement pst = DataBase.prepareStatement(sql,null);
            //pst.setInt(1,FO_SERVICE_ID);
            //ResultSet rs = pst.executeQuery();
            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            param[0] = new VIS.DB.SqlParam("@FO_SERVICE_ID", FO_SERVICE_ID);

            dr = VIS.DB.executeReader(sql, param, null);
            while (dr.read()) {
                FO_HOTEL_UOM_ID = Util.getValueOfInt(dr[0]);
                mTab.setValue("AUOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("CG1UOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("CG2UOM_ID", FO_HOTEL_UOM_ID);
                mTab.setValue("DESCRIPTION", Util.getValueOfString(dr[1]));//rs.getString(2));
            }
            dr.close();
            //pst.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        var sqlprice = "select PRICE,CHILDGROUP1,CHILDGROUP2,UOM1,UOM2 from FO_SERVICE_PRICE_PRICELINE " +
                        "where CREATED=(select max(CREATED) from FO_SERVICE_PRICE_PRICELINE " +
                        "where FO_SERVICE_ID=@FO_SERVICE_ID)";
        try {
            //PreparedStatement pst1 = DataBase.prepareStatement(sqlprice,null);
            //pst1.setInt(1,FO_SERVICE_ID);
            //ResultSet rs1 = pst1.executeQuery();
            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@FO_SERVICE_ID", FO_SERVICE_ID);
            dr = VIS.DB.executeReader(sqlprice, param, null);
            while (dr.read()) {
                APRICE = Util.getValueOfDecimal(dr[0]);
                CG1PRICE = Util.getValueOfDecimal(dr[1]);
                CG2PRICE = Util.getValueOfDecimal(dr[2]);

                //code changed by sandeep for is%
                UOM1 = dr[3].toString();
                UOM2 = dr[4].toString();
                if (UOM1 == "Y") {
                    //CG1PRICE=APRICE.multiply(CG1PRICE).multiply(new BigDecimal(0.01));
                    CG1PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG1PRICE), new Decimal(0.01));
                }
                if (UOM2 == "Y") {
                    CG2PRICE = Decimal.Multiply(Decimal.Multiply(APRICE, CG2PRICE), new Decimal(0.01));
                    //CG2PRICE=APRICE.multiply(CG2PRICE).multiply(new BigDecimal(0.01));
                }

                //changes end
                mTab.setValue("APRICE", APRICE);
                mTab.setValue("CG1PRICE", CG1PRICE);
                mTab.setValue("CG2PRICE", CG2PRICE);
            }
            //rs1.close();
            //pst1.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));



        //Get Days wise difference
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var atoDate=Util.getValueOfDateTime(mTab.getValue("ADATETO");
        //var miltodate=atoDate.GetTime();

        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var afromDate=Util.getValueOfDateTime(mTab.getValue("ADATEFROM");
        //var milfromdate=afromDate.GetTime();

        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        var t = afromDate.Subtract(atoDate);
        //var daydiff = t.Days;


        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }

        //var mildiff=miltodate-milfromdate;
        var secdiff = t.Seconds;
        var mindiff = t.Minutes;
        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *    Check todate with fromdate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferServices.prototype.tochkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var a = Util.getValueOfDateTime(value);// Timestamp a = (Timestamp)value;
        if (a == null || a.equals(0))
            return "";
        var atoDate = Util.getValueOfDateTime(value);
        atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));

        //var afromDate=Util.getValueOfDateTime(mTab.getValue("ADATEFROM");
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));

        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        //var milfromdate=afromDate.GetTime();
        try {
            atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
            //var miltodate=atoDate.GetTime();
            //TimeSpan t = afromDate.Subtract(atoDate);//commented be sandeep
            // var numdays = t.Days;//commented be sandeep
            var numdays;
            numdays = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);

            //numdays=TimeUtil.getDaysBetween(afromDate, atoDate);
            if (numdays <= 0) {
                //Object[] ob={"ok"};
                //JOptionPane.showOptionDialog(new JFrame(),"ToDate cannot appear Before After Date","FO",0,JOptionPane.ERROR_MESSAGE,null,ob,ob[0]);
                // ShowMessage.Error("'ToDate' cannot appear Before 'After Date'", true);
                ////////////////////////////////////////////////			
                var incdays;// = afromDate.AddDays(1);
                incdays = Utility.TimeUtil.AddDays(afromDate, 1);

                mTab.setValue("ADATETO", incdays);

                //Date date2 = new Date();
                //////////////////////////////////////////////	
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                var t1 = atoDate.Subtract(afromDate);
                // var daydiff = t1.Days;//TimeUtil.getDaysBetween(afromDate,atoDate) ;

                var daydiff = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t1.Milliseconds;//miltodate-milfromdate;
                var secdiff = t1.Seconds;//mildiff/1000;
                var mindiff = t1.Minutes;//secdiff/60;
                var monthact;
                var weekact;
                var monthquo = daydiff / 30;
                var monthrem = daydiff % 30;
                var weekquo = daydiff / 7;
                var weekrem = daydiff % 7;
                if (monthrem == 0) {
                    monthact = monthquo;
                }
                else {
                    monthact = monthquo + 1;
                }

                if (weekrem == 0) {
                    weekact = weekquo;
                }
                else {
                    weekact = weekquo + 1;
                }
                var SetafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", SetafromDate);
                mTab.setValue("CG2DATEFROM", SetafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();
                if (uomsel == "" || uomsel.Trim().Length == 0) {
                    uomsel = "0";
                }
                var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
                var anumber = System.Convert.ToInt16(mTab.getValue("ANUM"));
                var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
                if (Util.getValueOfInt(arebate) == 0) {
                    calrebate = VIS.Env.ZERO;
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            cal1 = cg1price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007:// per month
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);

                            break;
                        case 1000008://per week
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week 	
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                else {
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007://per month 
                            calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000008://per week
                            calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week  
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                if (anumber == 0)//update code on 11-09-2009 by sandeep
                {
                    mTab.setValue("ATOTAL_AMOUNT", 0);
                    mTab.setValue("GRAND_TOTAL", 0);
                }
                if (cg1number == 0) {
                    mTab.setValue("CG1TOTAL_AMOUNT", 0);

                }
                if (cg2number == 0) {
                    mTab.setValue("CG2TOTAL_AMOUNT", 0);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //e.getLocalizedMessage();
        }
        //setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/

    /*****************************************************************************
     *    Check fromdate with todate for Adult
     *    and if the todate is less then from date the info message will pop up 
     *    also it will calculate the number of days between todate and from date 
     *    if todate>fromdate
     ****************************************************************************/
    CalloutOfferServices.prototype.fromchkdate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var b = Util.getValueOfDateTime(value);
        if (b == null || b.equals(0))
            return "";
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));

        //var miltodate=atoDate.GetTime();
        try {
            afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
            //var milfromdate=afromDate.getTime();
            //if(afromDate.after(atoDate))
            if (afromDate.compareTo(atoDate) > 0) {
                mTab.setValue("CG1DATEFROM", afromDate);
                mTab.setValue("CG2DATEFROM", afromDate);
                ////////////////////////////////////////////////			
                var incdays;

                //incdays=TimeUtil.addDays(afromDate, 1);
                incdays = afromDate.addDays(1);// TimeUtil.addDays(afromDate, 1);
                mTab.setValue("ADATETO", incdays);
                //////////////////////////////////////////////	
            }
            else if (atoDate.compareTo(afromDate) > 0) {
                // TimeSpan t2 = atoDate.Subtract(afromDate);//commented by sandeep
                var t2 = afromDate.subtract(atoDate);//Added by Sandeep
                //var daydiff = t2.Days;
                var daydiff = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);
                var mildiff = t2.Milliseconds;
                var secdiff = t2.Seconds;
                var mindiff = t2.Minutes;
                var monthact;
                var weekact;
                var monthquo = daydiff / 30;
                var monthrem = daydiff % 30;
                var weekquo = daydiff / 7;
                var weekrem = daydiff % 7;
                if (monthrem == 0) {
                    monthact = monthquo;
                }
                else {
                    monthact = monthquo + 1;
                }

                if (weekrem == 0) {
                    weekact = weekquo;
                }
                else {
                    weekact = weekquo + 1;
                }
                var SetafromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
                mTab.setValue("CG1DATETO", atoDate);
                mTab.setValue("CG2DATETO", atoDate);
                mTab.setValue("CG1DATEFROM", SetafromDate);
                mTab.setValue("CG2DATEFROM", SetafromDate);
                var uomsel = mTab.getValue("AUOM_ID").toString();

                if (uomsel == "" || uomsel.Trim().Length == 0) {
                    uomsel = "0";
                }
                var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
                var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
                var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
                if (Util.getValueOfInt(arebate) == 0) {
                    calrebate = VIS.Env.ZERO;
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            cal1 = cg1price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007:// per month
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);

                            break;
                        case 1000008://per week
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week 	
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                else {
                    switch (int.Parse(uomsel)) {
                        case 1000003://flat rate
                            cal = aprice;
                            cal1 = cg1price;
                            cal2 = cg2price;
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000004://per person
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000005://per stock
                            calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000006://per night
                            calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000007://per month 
                            calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000008://per week
                            calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000009://per day
                            calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000010://per standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000011://per minute
                            calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000012://per second
                            calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                            cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000013://person/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000014://stock/night
                            calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000015://person/week  
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000016://stock/week
                            calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000017://person/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000018://stock/day
                            calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                            cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000019://person/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        case 1000020://stock/standard
                            calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                            cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                            cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                            cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                            cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                            cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                            mTab.setValue("ATOTAL_AMOUNT", cal);
                            mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                            mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                            grand_total = cal;
                            mTab.setValue("GRAND_TOTAL", grand_total);
                            break;
                        default:
                            ShowMessage.Info("false", true, "", "");
                            break;
                    }
                }
                if (anumber == 0)//update code on 11-09-2009 by sandeep
                {
                    mTab.setValue("ATOTAL_AMOUNT", 0);
                    mTab.setValue("GRAND_TOTAL", 0);
                }
                if (cg1number == 0) {
                    mTab.setValue("CG1TOTAL_AMOUNT", 0);

                }
                if (cg2number == 0) {
                    mTab.setValue("CG2TOTAL_AMOUNT", 0);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //e.getLocalizedMessage();
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *   if Adult UOM is selected the same will be displayed in child records
     ****************************************************************************/
    CalloutOfferServices.prototype.UOMSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        //		if (isCalloutActive())
        //			return"";
        //		setCalloutActive(true);
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var UOMsel = Util.getValueOfInt(value);
        UOMsel = Util.getValueOfInt(mTab.getValue("AUOM_ID"));
        if (UOMsel == null)
            return "";
        mTab.setValue("CG1UOM_ID", UOMsel);
        mTab.setValue("CG2UOM_ID", UOMsel);
        var uomsel = mTab.getValue("AUOM_ID").toString();
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        //var miltodate = atoDate.GetTime();

        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate = afromDate.GetTime();
        var t3;
        if (afromDate.compareTo(atoDate) > 0) {
            t3 = afromDate.Subtract(atoDate);
        }
        else {
            t3 = atoDate.Subtract(afromDate);
        }


        var daydiff = Utility.timeUtil.getDaysBetween(afromDate, atoDate);
        //var daydiff = t3.Days;

        //var mildiff = miltodate - milfromdate;
        //var secdiff = mildiff / 1000;
        //var mindiff = secdiff / 60;

        var mildiff = t3.Milliseconds;
        var secdiff = t3.Seconds;
        var mindiff = t3.Minutes;
        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }
        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }

        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //		setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /************************************************************************
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     ************************************************************************/
    /*****************************************************************************
     *   if Adult Number is selected then the calculation is done
     ****************************************************************************/
    CalloutOfferServices.prototype.AVALSelected = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var cal, cal1, cal2;
        var calrebate, cg1calrebate, cg2calrebate, grand_total;
        var uomsel = null;
        try {
            uomsel = mTab.getValue("AUOM_ID").toString();
        }
        catch (err) {
            this.setCalloutActive(false);
            return "";
        }
        var aprice = Util.getValueOfDecimal(mTab.getValue("APRICE"));
        var arebate = Util.getValueOfDecimal(mTab.getValue("AREBATE"));
        var anumber = Util.getValueOfInt(mTab.getValue("ANUM"));
        var atoDate = Util.getValueOfDateTime(mTab.getValue("ADATETO"));
        var cg1number = Util.getValueOfInt(mTab.getValue("CG1NUM"));
        var cg2number = Util.getValueOfInt(mTab.getValue("CG2NUM"));
        var cg1price = Util.getValueOfDecimal(mTab.getValue("CG1PRICE"));
        var cg2price = Util.getValueOfDecimal(mTab.getValue("CG2PRICE"));

        //var miltodate = atoDate.GetTime();
        var afromDate = Util.getValueOfDateTime(mTab.getValue("ADATEFROM"));
        //var milfromdate = afromDate.GetTime();
        var t4 = afromDate.Subtract(atoDate);

        var daydiff = Utility.TimeUtil.getDaysBetween(afromDate, atoDate);
        //var daydiff = t4.Days;

        //var mildiff = miltodate - milfromdate;
        //var secdiff = mildiff / 1000;
        //var mindiff = secdiff / 60;

        var mildiff = t4.Milliseconds;
        var secdiff = t4.Seconds;
        var mindiff = t4.Minutes;

        var monthact;
        var weekact;
        var monthquo = daydiff / 30;
        var monthrem = daydiff % 30;
        var weekquo = daydiff / 7;
        var weekrem = daydiff % 7;
        if (monthrem == 0) {
            monthact = monthquo;
        }
        else {
            monthact = monthquo + 1;
        }

        if (weekrem == 0) {
            weekact = weekquo;
        }
        else {
            weekact = weekquo + 1;
        }
        if (Util.getValueOfInt(arebate) == 0) {
            calrebate = VIS.Env.ZERO;
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    cal1 = cg1price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);

                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);

                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007:// per month
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);

                    break;
                case 1000008://per week
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    cal = CalculatePrice(new Decimal((daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week 	
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal((daydiff + 1) * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal((daydiff + 1) * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, calrebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        else {
            switch (int.Parse(uomsel)) {
                case 1000003://flat rate
                    cal = aprice;
                    cal1 = cg1price;
                    cal2 = cg2price;
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000004://per person
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000005://per stock
                    calrebate = CalculateRebate(new Decimal(anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000006://per night
                    calrebate = CalculateRebate(new Decimal(daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000007://per month 
                    calrebate = CalculateRebate(new Decimal(monthact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(monthact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(monthact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(monthact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(monthact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(monthact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000008://per week
                    calrebate = CalculateRebate(new Decimal(weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000009://per day
                    calrebate = CalculateRebate(new Decimal(daydiff + 1), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff + 1), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff + 1), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff + 1), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff + 1), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff + 1), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000010://per standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000011://per minute
                    calrebate = CalculateRebate(new Decimal(mindiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(mindiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(mindiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(mindiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(mindiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(mindiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000012://per second
                    calrebate = CalculateRebate(new Decimal(secdiff), aprice, arebate);
                    cal = CalculatePrice(new Decimal(secdiff), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(secdiff), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(secdiff), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(secdiff), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(secdiff), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000013://person/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000014://stock/night
                    calrebate = CalculateRebate(new Decimal(daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000015://person/week  
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000016://stock/week
                    calrebate = CalculateRebate(new Decimal(anumber * weekact), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * weekact), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * weekact), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * weekact), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * weekact), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * weekact), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000017://person/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal(anumber * (daydiff + 1)), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000018://stock/day
                    calrebate = CalculateRebate(new Decimal(anumber * (daydiff + 1)), aprice, arebate);
                    cal = CalculatePrice(new Decimal((daydiff + 1) * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(cg1number * (daydiff + 1)), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(cg1number * (daydiff + 1)), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(cg2number * (daydiff + 1)), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(cg2number * (daydiff + 1)), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000019://person/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = Decimal.Add(Decimal.Add(cal, cal1), cal2);
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                case 1000020://stock/standard
                    calrebate = CalculateRebate(new Decimal(24 * daydiff * anumber), aprice, arebate);
                    cal = CalculatePrice(new Decimal(24 * daydiff * anumber), aprice, calrebate);
                    cg1calrebate = CalculateRebate(new Decimal(24 * daydiff * cg1number), cg1price, arebate);
                    cal1 = CalculatePrice(new Decimal(24 * daydiff * cg1number), cg1price, cg1calrebate);
                    cg2calrebate = CalculateRebate(new Decimal(24 * daydiff * cg2number), cg2price, arebate);
                    cal2 = CalculatePrice(new Decimal(24 * daydiff * cg2number), cg2price, cg2calrebate);
                    mTab.setValue("ATOTAL_AMOUNT", cal);
                    mTab.setValue("CG1TOTAL_AMOUNT", cal1);
                    mTab.setValue("CG2TOTAL_AMOUNT", cal2);
                    grand_total = cal;
                    mTab.setValue("GRAND_TOTAL", grand_total);
                    break;
                default:
                    ShowMessage.Info("false", true, "", "");
                    break;
            }
        }
        if (anumber == 0)//update code on 11-09-2009 by sandeep
        {
            mTab.setValue("ATOTAL_AMOUNT", 0);
            mTab.setValue("GRAND_TOTAL", 0);
        }
        if (cg1number == 0) {
            mTab.setValue("CG1TOTAL_AMOUNT", 0);

        }
        if (cg2number == 0) {
            mTab.setValue("CG2TOTAL_AMOUNT", 0);
        }
        //						setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOfferServices = CalloutOfferServices;
    //*********CalloutOfferServices End *******



    //**************CalloutCheckInOut Start*******
    function CalloutCheckInOut() {
        VIS.CalloutEngine.call(this, "VIS.CalloutCheckInOut"); //must call
    };
    /**
    *  @param ctx      Context
    *  @param windowNo current Window No
    *  @param mTab     Model Tab
    *  @param mField   Model Field
    *  @param value    The new value
    *  @return Error message or ""
    */
    CalloutCheckInOut.prototype.TypeSelection = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        //Boolean flag1=Boolean.parseBoolean(value.toString());
        var flag1 = Util.getValueOfBoolean(value);
        if (flag1 == null) {
            return "";
        }
        if (flag1 == true) {
            mTab.setValue("flag1", true);
        }
        else {
            mTab.setValue("flag1", false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *  
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */

    CalloutCheckInOut.prototype.RecordSort = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }

        //Boolean flag2=Boolean.parseBoolean(value.toString());
        var flag2 = Util.getValueOfBoolean(value);
        if (flag2 == null)
            return "";
        if (flag2 == true) {
            mTab.setValue("flag2", true);
        }
        else {
            mTab.setValue("flag2", false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /**
     *  
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */
    CalloutCheckInOut.prototype.EndDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        //Timestamp a=(Timestamp)value;
        var a = Util.getValueOfDateTime(value);
        if (a == null || a.equals(0))
            return "";
        var enddate = Util.getValueOfDateTime(value);
        var startdate = Util.getValueOfDateTime(mTab.getValue("DATE_FROM"));
        try {
            enddate = Util.getValueOfDateTime(mTab.getValue("TILL_DATE"));
            if (enddate.compareTo(startdate) < 0)
                //if(enddate.before(startdate))
            {
                //Object ob[]={"ok"};
                //JOptionPane.showOptionDialog(new JFrame(),"'End Date' cannot appear Before 'Start Date'","FO",0,JOptionPane.ERROR_MESSAGE,null,ob,ob[0]);
                // ShowMessage.Error("EndAndStartDate", true);
                VIS.ADialog.info("EndAndStartDate");
                mTab.setValue("TILL_DATE", startdate);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            //e.getLocalizedMessage();
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *  
     *
     *  @param ctx      Context
     *  @param windowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */
    CalloutCheckInOut.prototype.StartDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        // Timestamp b=(Timestamp)value;
        var b = Util.getValueOfDateTime(value);
        if (b == null || b.equals(0))
            return "";
        var startdate = Util.getValueOfDateTime(mTab.getValue("DATE_FROM"));
        var enddate = Util.getValueOfDateTime(mTab.getValue("TILL_DATE"));
        try {
            startdate = Util.getValueOfDateTime(mTab.getValue("DATE_FROM"));
            if (startdate.compareTo(enddate) > 0)
                //if(startdate.after(enddate))
            {
                mTab.setValue("TILL_DATE", startdate);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            //	e.getLocalizedMessage();
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutCheckInOut = CalloutCheckInOut;
    //**************CalloutCheckInOut End************ 


    //************CalloutInventory Start********
    /// <summary>
    /// Physical Inventory Callouts
    /// </summary>

    function CalloutInventory() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInventory"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutInventory, VIS.CalloutEngine); //inherit calloutengine
    /// <summary>
    /// Product/Locator/asi modified.
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>error message or ""</returns>
    CalloutInventory.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive())
            return "";

        //	overkill - see new implementation
        var inventoryLine = Util.getValueOfInt(mTab.getValue("M_InventoryLine_ID"));
        var M_Inventory_ID = Util.getValueOfInt(mTab.getValue("M_Inventory_ID"));
        var dr1 = null;
        var paramInventory = M_Inventory_ID.toString();
        dr1 = VIS.dataContext.getJSONRecord("MInventory/GetMInventory", paramInventory);
        var MovementDate = dr1["MovementDate"];
        var AD_Org_ID = dr1["AD_Org_ID"];

        var bd = null;
        var paramString = inventoryLine.toString();
        /**
         * Modified for update Book Qty on existing records.
         * Also checks the old asi and removes it if product has been change.
         */
        if (inventoryLine != null && inventoryLine != 0) {
            //Get MInventoryLine Information
            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MInventoryLine/GetMInventoryLine", paramString);

            var M_Product_ID = dr["M_Product_ID"];//dr.M_Product_ID;//getQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetI
            var M_Locator_ID = dr["M_Locator_ID"];//dr.M_Locator_ID;




            // MInventoryLine iLine = new MInventoryLine(ctx, inventoryLine.Value, null);  
            var M_Product_ID1 = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
            var M_Locator_ID1 = Util.getValueOfInt(mTab.getValue("M_Locator_ID"));
            var M_AttributeSetInstance_ID1 = 0;
            // if product or locator has changed recalculate Book Qty
            //if (M_Product_ID1 != iLine.GetM_Product_ID() || M_Locator_ID1 != iLine.GetM_Locator_ID())
            //{
            if (M_Product_ID1 != M_Product_ID || M_Locator_ID1 != M_Locator_ID) {
                this.setCalloutActive(true);
                // Check asi - if product has been changed remove old asi
                if (M_Product_ID1 == M_Product_ID) {
                    M_AttributeSetInstance_ID1 = Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"));
                }
                else {
                    mTab.setValue("M_AttributeSetInstance_ID", null);
                }
                try {
                    bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID1, M_Product_ID1, M_Locator_ID1, Util.getValueOfDate(MovementDate));
                    mTab.setValue("QtyBook", bd);
                }
                catch (err) {
                    this.setCalloutActive(false);
                    this.log.severe(err.toString());
                    return mTab.setValue("QtyBook", bd);
                }
            }
            this.setCalloutActive(false);
            ctx = windowNo = mTab = mField = value = oldValue = null;
            return "";
        }

        //	New Line - Get Book Value
        var M_Product_ID = 0;
        var product = Util.getValueOfInt(mTab.getValue("M_Product_ID"));
        if (product != null)
            M_Product_ID = product;
        if (M_Product_ID == 0)
            return "";
        var M_Locator_ID = 0;
        var locator = Util.getValueOfInt(mTab.getValue("M_Locator_ID"));
        if (locator != null)
            M_Locator_ID = locator;
        if (M_Locator_ID == 0)
            return "";

        this.setCalloutActive(true);
        //	Set Attribute
        var M_AttributeSetInstance_ID = 0;
        var asi = Util.getValueOfInt(mTab.getValue("M_AttributeSetInstance_ID"));
        if (asi != null)
            M_AttributeSetInstance_ID = asi;
        //	Product Selection
        if (ctx.getContextAsInt(windowNo, "M_Product_ID", false) == M_Product_ID) {
            M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false);
            if (M_AttributeSetInstance_ID != 0)
                mTab.setValue("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
            else
                mTab.setValue("M_AttributeSetInstance_ID", null);
        }

        // Call's now the extracted function
        try {
            bd = this.SetQtyBook(AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, Util.getValueOfDate(MovementDate));
            mTab.setValue("QtyBook", bd);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            return mTab.setValue("QtyBook", bd);
        }

        //
        this.log.info("M_Product_ID=" + M_Product_ID
             + ", M_Locator_ID=" + M_Locator_ID
             + ", M_AttributeSetInstance_ID=" + M_AttributeSetInstance_ID
             + " - QtyBook=" + bd);
        this.setCalloutActive(false);

        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// FifferenceQty/AsOnDateCount modified.
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>error message or ""</returns>
    CalloutInventory.prototype.SetDiff = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var asOnDateQty = 0, openingStock = 0, diffQty = 0;
        this.setCalloutActive(true);
        try {
            if (mField.getColumnName().equals("DifferenceQty")) {
                openingStock = Util.getValueOfDecimal(mTab.getValue("OpeningStock"));
                diffQty = Util.getValueOfDecimal(mTab.getValue("DifferenceQty"));
                asOnDateQty = openingStock - diffQty;
                mTab.setValue("AsOnDateCount", asOnDateQty);
            }
            else if (mField.getColumnName().equals("AsOnDateCount")) {
                openingStock = Util.getValueOfDecimal(mTab.getValue("OpeningStock"));
                asOnDateQty = Util.getValueOfDecimal(mTab.getValue("AsOnDateCount"));
                diffQty = openingStock - asOnDateQty;
                mTab.setValue("DifferenceQty", diffQty);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            return err.message;
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// Returns the current Book Qty for given parameters or 0
    /// </summary>
    /// <param name="M_AttributeSetInstance_ID">attribute set instance id</param>
    /// <param name="M_Product_ID">product id</param>
    /// <param name="M_Locator_ID">locator id</param>
    /// <returns></returns>
    CalloutInventory.prototype.SetQtyBook = function (AD_Org_ID, M_AttributeSetInstance_ID, M_Product_ID, M_Locator_ID, MovementDate) {
        //  
        // Set QtyBook from first storage location
        var bd = null;
        var query = "", qry = "";
        var result = 0;
        var tsDate = "TO_DATE( '" + (Number(MovementDate.getMonth()) + 1) + "-" + MovementDate.getDate() + "-" + MovementDate.getFullYear() + "', 'MM-DD-YYYY')";     // GlobalVariable.TO_DATE(MovementDate, true);
        query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + tsDate +
            " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
        result = Util.getValueOfInt(VIS.DB.executeScalar(query));
        if (result > 0) {
            qry = "SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
                "(SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + tsDate + "  AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND AD_Org_ID = " + AD_Org_ID + " AND  M_Product_ID = " + M_Product_ID +
                " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            bd = Util.getValueOfDecimal(VIS.DB.executeScalar(qry));
        }
        else {
            query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID +
                " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            result = Util.getValueOfInt(VIS.DB.executeScalar(query));
            if (result > 0) {
                qry = "SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID = (SELECT MAX(M_Transaction_ID)   FROM M_Transaction  WHERE movementdate = " +
                    " (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + tsDate + " AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                    " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID +
                    " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + ") AND AD_Org_ID = " + AD_Org_ID + " AND  M_Product_ID = " + M_Product_ID +
                    " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                bd = Util.getValueOfDecimal(VIS.DB.executeScalar(qry));
            }
        }
        if (bd != null) {
            return bd;
        }
        return 0;
    };
    VIS.Model.CalloutInventory = CalloutInventory;
    //************CalloutInventory End*********


    //**************CalloutInvoice Start*********
    function CalloutInvoice() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInvoice"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutInvoice, VIS.CalloutEngine);//inherit CalloutEngine

    /**
     *	Invoice Header - DocType.
     *		- PaymentRule
     *		- temporary Document
     *  Context:
     *  	- DocSubTypeSO
     *		- HasCharges
     *	- (re-sets Business Partner info of required)
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }

        var C_DocType_ID = value;//(int)value;
        if (C_DocType_ID == null || C_DocType_ID == 0)
            return "";

        var sql = "SELECT d.HasCharges,'N',d.IsDocNoControlled,"
            + "s.CurrentNext, d.DocBaseType "
            /*//jz outer join
            + "FROM C_DocType d, AD_Sequence s "
            + "WHERE C_DocType_ID=?"		//	1
            + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
            */
            + "FROM C_DocType d "
            + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
            + "WHERE C_DocType_ID=" + C_DocType_ID;		//	1
        var dr = null;
        try {

            dr = VIS.DB.executeReader(sql, null, null);

            if (dr.read()) {
                //	Charges - Set Context
                // ctx.setContext(windowNo, "HasCharges", dr[0].toString());
                ctx.setContext(windowNo, "HasCharges", dr.get("hascharges"));
                //	DocumentNo
                if (dr.get("isdocnocontrolled") == "Y") {
                    //if (dr[2].toString().equals("Y")) {
                    // mTab.setValue("DocumentNo", "<" + dr[3].toString() + ">");
                    mTab.setValue("DocumentNo", "<" + dr.get("currentnext") + ">");
                }
                //  DocBaseType - Set Context
                //var s = dr[4].toString();//.getString(5);
                var s = Util.getValueOfString(dr.get("docbasetype"));//.getString(5);
                ctx.setContext(windowNo, "DocBaseType", s);
                //  AP Check & AR Credit Memo
                if (s.startsWith("AP")) {
                    mTab.setValue("PaymentRule", "S");    //  Check
                }
                else if (s.endsWith("C")) {
                    mTab.setValue("PaymentRule", "P");    //  OnCredit
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
            return err.message;
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /**
     *	Invoice Header- BPartner.
     *		- M_PriceList_ID (+ Context)
     *		- C_BPartner_Location_ID
     *		- AD_User_ID
     *		- POReference
     *		- SO_Description
     *		- IsDiscountPrinted
     *		- PaymentRule
     *		- C_PaymentTerm_ID
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (value == null || value.toString() == "") {
            return "";
        }
        try {

            var C_BPartner_ID = Util.getValueOfInt(value);//(int)value;
            if (C_BPartner_ID == null || C_BPartner_ID == 0) {
                return "";
            }

            var sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " l.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1

            var isSOTrx = ctx.isSOTrx();
            var dr = null;
            try {

                dr = VIS.DB.executeReader(sql, null, null);

                if (dr.read()) {
                    //	PriceList & IsTaxIncluded & Currency
                    var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                    if (ii > 0) {
                        mTab.setValue("M_PriceList_ID", ii);
                    }
                    else {	//	get default PriceList
                        var i = ctx.getContextAsInt("#M_PriceList_ID");
                        if (i != 0) {
                            mTab.setValue("M_PriceList_ID", i);
                        }
                    }

                    //	PaymentRule
                    var s = Util.getValueOfString(dr.get(isSOTrx ? "paymentrule" : "paymentrulepo"));
                    if (s != null && s.length != 0) {
                        if (ctx.getContext("DocBaseType").toString().endsWith("C"))	//	Credits are Payment Term
                        {
                            s = "P";
                        }
                        else if (isSOTrx && (s.toString().equals("S") || s.toString().equals("U")))	//	No Check/Transfer for SO_Trx
                        {
                            s = "P";											//  Payment Term
                        }
                        mTab.setValue("PaymentRule", s);
                    }
                    //  Payment Term
                    ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                    //if (!dr.wasNull())
                    if (ii > 0) {
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    }
                    //	Location
                    var locID = Util.getValueOfInt(dr.get("c_bpartner_location_id"));
                    //	overwritten by InfoBP selection - works only if InfoWindow
                    //	was used otherwise creates error (uses last value, may bevar  to differnt BP)
                    if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                        var loc = ctx.getContext("C_BPartner_Location_ID");
                        if (loc && loc.toString().length > 0) {
                            locID = parseInt(loc);
                        }
                    }
                    if (locID == 0) {
                        mTab.setValue("C_BPartner_Location_ID", null);
                    }
                    else {
                        mTab.setValue("C_BPartner_Location_ID", locID);
                    }

                    //	Contact - overwritten by InfoBP selection
                    var contID = Util.getValueOfInt(dr.get("ad_user_id"));
                    if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                        var cont = ctx.getContext("AD_User_ID");
                        if (cont && cont.toString().length > 0) {
                            contID = parseInt(cont);
                        }
                    }
                    if (contID == 0) {
                        mTab.setValue("AD_User_ID", null);
                    }
                    else {
                        mTab.setValue("AD_User_ID", contID);
                    }
                    //	CreditAvailable
                    if (isSOTrx) {
                        var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                        if (CreditLimit != 0) {
                            var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                            //if (!dr.wasNull() && CreditAvailable < 0)
                            if (CreditAvailable < 0) {
                                //   ShowMessage.Info("CreditLimitOver", null, CreditAvailable.toString(), "");
                                VIS.ADialog.info("CreditLimitOver");
                            }
                        }
                    }

                    //	PO Reference
                    s = Util.getValueOfString(dr.get("poreference"));
                    if (s != null && s.length != 0) {
                        mTab.setValue("POReference", s);
                    }
                    else {
                        mTab.setValue("POReference", null);
                    }
                    //	SO Description
                    s = Util.getValueOfString(dr.get("so_description"));
                    if (s != null && s.toString().trim().length != 0) {
                        mTab.setValue("Description", s);
                    }
                    //	IsDiscountPrinted
                    s = Util.getValueOfString(dr.get("isdiscountprinted"));
                    if (s != null && s.toString().trim().length != 0) {
                        mTab.setValue("IsDiscountPrinted", s);
                    }
                    else {
                        mTab.setValue("IsDiscountPrinted", "N");
                    }
                }
                dr.close();
            }
            catch (err) {
                this.setCalloutActive(false);
                if (dr != null) {
                    dr.close();
                }
                this.log.log(Level.SEVERE, "bPartner", err);
                return err.message;
            }
            finally {
                if (dr != null) {
                    dr.close();
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *	Set Payment Term.
     *	Payment Term has changed 
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.PaymentTerm = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_PaymentTerm_ID = value;
            var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID", false);
            var get_ID;
            var apply;
            if (C_PaymentTerm_ID == null || C_PaymentTerm_ID == 0
                || C_Invoice_ID == 0)	//	not saved yet
            {
                return "";
            }
            //
            var paramString = C_PaymentTerm_ID.toString().concat(",", C_Invoice_ID.toString());

            var dr = VIS.dataContext.getJSONRecord("MPaymentTerm/GetPaymentTerm", paramString);

            get_ID = dr.get("Get_ID");
            var valid = dr.get("Apply");
            if (get_ID == 0) {
                return "PaymentTerm not found";
            }

            //MPaymentTerm pt = new MPaymentTerm(ctx, C_PaymentTerm_ID, null);
            //if (pt.Get_ID() == 0)
            //{
            //    return "PaymentTerm not found";
            //}

            //var valid = pt.Apply(C_Invoice_ID);
            mTab.setValue("IsPayScheduleValid", valid ? "Y" : "N");
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /***
     *	Invoice Line - Product.
     *		- reset C_Charge_ID / M_AttributeSetInstance_ID
     *		- PriceList, PriceStd, PriceLimit, C_Currency_ID, EnforcePriceLimit
     *		- UOM
     *	Calls Tax
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";
        this.setCalloutActive(true);
        try {

            mTab.setValue("C_Charge_ID", null);

            //	Set Attribute
            if (ctx.getContextAsInt(windowNo, "M_Product_ID", false) == M_Product_ID
                && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false) != 0) {
                mTab.setValue("M_AttributeSetInstance_ID", ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID", false));
            }
            else {
                mTab.setValue("M_AttributeSetInstance_ID", null);
            }

            //try
            //{
            //    object[] pqtyAll = VAdvantage.Classes.InfoLines.PQ.ToArray();
            //    for (int x = 0; x < pqtyAll.Length; x++)
            //    {
            //        object f = pqtyAll.GetValue(x);

            //        int AD_Session_ID = Util.getValueOfInt(((VAdvantage.Classes.InfoLines)f)._AD_Session_ID);
            //        int winNo = Util.getValueOfInt(((VAdvantage.Classes.InfoLines)f)._windowNo);
            //        Dictionary<int, Decimal> ProductQty = ((VAdvantage.Classes.InfoLines)f)._prodQty;

            //        List<int> key = ProductQty.Keys.ToList();
            //        if (AD_Session_ID == Env.GetCtx().GetAD_Session_ID() && winNo == windowNo && Util.getValueOfInt(value) == Util.getValueOfInt(key[0]))
            //        {
            //            Decimal qty = Util.getValueOfDecimal(ProductQty[Util.getValueOfInt(value)]);
            //            mTab.setValue("QtyEntered", qty);
            //            mTab.setValue("QtyInvoiced", qty);
            //            VAdvantage.Classes.InfoLines.PQ.RemoveAt(x);
            //            break;
            //        }

            //    }
            //}
            //catch
            //{
            //    for (int k = 0; k < VAdvantage.Classes.InfoLines.PQ.Count; k++)
            //    {
            //        VAdvantage.Classes.InfoLines.PQ.RemoveAt(k);
            //    }
            //}

            /*****	Price Calculation see also qty	****/
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID", false);
            var Qty = mTab.getValue("QtyInvoiced");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID", false);
            // pp.SetM_PriceList_ID(M_PriceList_ID);
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
            var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
            // pp.setM_PriceList_Version_ID(M_PriceList_Version_ID);
            //var  time = ctx.getContextAsTime(windowNo, "DateInvoiced");
            //pp.SetPriceDate(time);
            var time = ctx.getContext("DateInvoiced");
            //pp.setPriceDate1(time);
            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                           Qty.toString(), ",", //3
                                                           isSOTrx, ",", //4 
                                                           M_PriceList_ID.toString(), ",", //5
                                                           M_PriceList_Version_ID.toString(), ",", //6
                                                           null, ",",//7
                                                           time.toString(), ",",
                                                           M_AttributeSetInstance_ID.toString()); //8





            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


            var rowDataDB = null;


            // MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //     M_Product_ID, C_BPartner_ID, Qty, isSOTrx);


            //		
            mTab.setValue("PriceList", dr["PriceList"]);
            mTab.setValue("PriceLimit", dr.PriceLimit);
            mTab.setValue("PriceActual", dr.PriceActual);
            mTab.setValue("PriceEntered", dr.PriceEntered);
            mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
            // mTab.setValue("Discount", dr.Discount);
            mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
            // mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));
            ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
            ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");



            //mTab.setValue("PriceList", pp.GetPriceList());
            //mTab.setValue("PriceLimit", pp.GetPriceLimit());
            //mTab.setValue("PriceActual", pp.GetPriceStd());
            //mTab.setValue("PriceEntered", pp.GetPriceStd());
            //mTab.setValue("C_Currency_ID", pp.GetC_Currency_ID());
            //mTab.setValue("C_UOM_ID", pp.GetC_UOM_ID());
            //ctx.setContext(windowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
            //ctx.setContext(windowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };

    /**
     *	Invoice Line - Charge.
     * 		- updates PriceActual from Charge
     * 		- sets PriceLimit, PriceList to zero
     * 	Calles tax
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var dr = null;
        try {
            var C_Charge_ID = value;
            if (C_Charge_ID == null || C_Charge_ID == 0)
                return "";

            //	No Product defined
            if (mTab.getValue("M_Product_ID") != null) {
                mTab.setValue("C_Charge_ID", null);
                return "ChargeExclusively";
            }
            mTab.setValue("M_AttributeSetInstance_ID", null);
            mTab.setValue("S_ResourceAssignment_ID", null);
            mTab.setValue("C_UOM_ID", 100);	//	EA

            ctx.setContext(windowNo, "DiscountSchema", "N");
            var sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=" + C_Charge_ID;

            dr = VIS.DB.executeReader(sql, null, null);
            if (dr.read()) {
                mTab.setValue("PriceEntered", Util.getValueOfDecimal(dr.get("chargeamt")));//.getBigDecimal(1));
                mTab.setValue("PriceActual", Util.getValueOfDecimal(dr.get("chargeamt")));//dr.getBigDecimal(1));
                mTab.setValue("PriceLimit", 0);
                mTab.setValue("PriceList", 0);
                mTab.setValue("Discount", 0);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            return err.message;
        }
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };

    /**
     *	Invoice Line - Tax.
     *		- basis: Product, Charge, BPartner Location
     *		- sets C_Tax_ID
     *  Calles Amount
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var column = mField.getColumnName();
        try {
            //	Check Product
            var M_Product_ID = 0;
            if (column.toString() == "M_Product_ID") {
                M_Product_ID = value;
            }
            else {
                M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            }
            var C_Charge_ID = 0;
            if (column.toString() == "C_Charge_ID") {
                C_Charge_ID = value;
            }
            else {
                C_Charge_ID = ctx.getContextAsInt(windowNo, "C_Charge_ID");
            }
            this.log.fine("Product=" + M_Product_ID + ", C_Charge_ID=" + C_Charge_ID);
            if (M_Product_ID == 0 && C_Charge_ID == 0) {
                return this.Amt(ctx, windowNo, mTab, mField, value);
            }

            //	Check Partner Location
            var shipC_BPartner_Location_ID = ctx.getContextAsInt(windowNo, "C_BPartner_Location_ID");
            if (shipC_BPartner_Location_ID == 0) {
                return this.Amt(ctx, windowNo, mTab, mField, value);
            }
            this.log.fine("Ship BP_Location=" + shipC_BPartner_Location_ID);
            var billC_BPartner_Location_ID = shipC_BPartner_Location_ID;
            this.log.fine("Bill BP_Location=" + billC_BPartner_Location_ID);

            //	Dates 
            //DateTime billDate = new DateTime(ctx.getContextAsTime(windowNo, "DateInvoiced"));
            var billDate = (ctx.getContext("DateInvoiced"));
            this.log.fine("Bill Date=" + billDate);
            var shipDate = billDate;
            this.log.fine("Ship Date=" + shipDate);

            var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
            this.log.fine("Org=" + AD_Org_ID);

            var M_Warehouse_ID = ctx.getContextAsInt("#M_Warehouse_ID");
            this.log.fine("Warehouse=" + M_Warehouse_ID);



            var paramString = C_Charge_ID.toString().concat(",", billDate.toString(),
                                                           shipDate.toString(), ",",
                                                            AD_Org_ID.toString(), ",",
                                                            M_Warehouse_ID.toString(), ",",
                                                            billC_BPartner_Location_ID.toString(), ",",
                                                            shipC_BPartner_Location_ID.toString(), ",",
                                                            ctx.getWindowContext(windowNo, "IsSOTrx", true).equals("Y"));


            var C_Tax_ID = Util.getValueOfInt(VIS.dataContext.getJSONRecord("MTax/Get", paramString));


            //var C_Tax_ID = VAdvantage.Model.Tax.Get(ctx, M_Product_ID, C_Charge_ID, billDate, shipDate,
            //    AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
            //    ctx.getContext("IsSOTrx").equals("Y"));
            this.log.info("Tax ID=" + C_Tax_ID);
            //
            if (C_Tax_ID == 0) {
                //mTab.fireDataStatusEEvent(CLogger.retrieveError());
                // VIS.ADialog.info("");
                //  ShowMessage.Info(VLogger.RetrieveError().Key.toString(), null, VLogger.RetrieveError().Name, "");
            }
            else {
                mTab.setValue("C_Tax_ID", C_Tax_ID);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    /**
     *	Invoice - Amount.
     *		- called from QtyInvoiced, PriceActual
     *		- calculates LineNetAmt
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        try {
            this.log.log(Level.WARNING, "amt - init");
            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");


            var pStr = M_PriceList_ID.toString(); //1

            //Get product price information
            var dr;
            dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", pStr);


            var StdPrecision = dr["StdPrecision"];;
            var QtyEntered, QtyInvoiced, PriceEntered, PriceActual, PriceLimit, Discount, PriceList;
            //	get values
            QtyEntered = mTab.getValue("QtyEntered");
            QtyInvoiced = mTab.getValue("QtyInvoiced");
            this.log.fine("QtyEntered=" + QtyEntered + ", Invoiced=" + QtyInvoiced + ", UOM=" + C_UOM_To_ID);
            //
            PriceEntered = mTab.getValue("PriceEntered");
            PriceActual = mTab.getValue("PriceActual");
            PriceLimit = mTab.getValue("PriceLimit");
            PriceList = mTab.getValue("PriceList");

            this.log.fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual);// + ", Discount=" + Discount);

            //	Qty changed - recalc price
            if ((mField.getColumnName() == "QtyInvoiced"
                || mField.getColumnName() == "QtyEntered"
                || mField.getColumnName() == "M_Product_ID")
                && !"N" == ctx.getContext("DiscountSchema")) {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                if (mField.getColumnName() == "QtyEntered") {
                    var paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                     QtyEntered.toString()); //3

                    QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                    //QtyInvoiced = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                    //    C_UOM_To_ID, QtyEntered);
                }
                if (QtyInvoiced == null)
                    QtyInvoiced = QtyEntered;
                var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";







                //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
                //        M_Product_ID, C_BPartner_ID, QtyInvoiced, isSOTrx);
                //pp.SetM_PriceList_ID(M_PriceList_ID);
                //var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                //var date = Util.getValueOfDateTime(mTab.getValue("DateInvoiced"));
                //pp.SetPriceDate(date);
                //

                //1                                                              
                var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                                Qty.toString(), ",", //3
                                                                isSOTrx, ",", //4 
                                                                M_PriceList_ID.toString(), ",", //5
                                                                M_PriceList_Version_ID.toString(), ",", //6
                                                                date.toString(), null); //7



                //Get product price information
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);

                //make parameter string
                var paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                       PriceActual.toString()); //3

                //var drPC = VIS.dataContext.getJSONRecord("CalloutOrder/ConvertProductFrom", paramStr);

                PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, pp.getPriceStd());

                //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, pp.GetPriceStd());
                if (PriceEntered == null) {
                    PriceEntered = dr.PriceStd;
                }
                this.log.fine("amt - QtyChanged -> PriceActual=" + dr.PriceStd
                     + ", PriceEntered=" + PriceEntered + ", Discount=" + dr.Discount);
                PriceActual = dr.PriceStd;
                mTab.setValue("PriceActual", PriceActual);
                //	mTab.setValue("Discount", pp.getDiscount());
                mTab.setValue("PriceEntered", PriceEntered);
                ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");
            }
            else if (mField.getColumnName() == "PriceActual") {
                PriceActual = value;

                //make parameter string
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                      PriceActual.toString()); //3

                var drPC = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);

                PriceEntered = drPC;//(Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);


                //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null) {
                    PriceEntered = PriceActual;
                }
                //
                this.log.fine("amt - PriceActual=" + PriceActual
                     + " -> PriceEntered=" + PriceEntered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
            else if (mField.getColumnName() == "PriceEntered") {
                PriceEntered = value;


                //make parameter string
                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                  PriceEntered.toString()); //3

                PriceActual = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramStr);

                //PriceActual = dr.retValue;//(Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //PriceActual = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceEntered);
                if (PriceActual == null) {
                    PriceActual = PriceEntered;
                }
                //
                this.log.fine("amt - PriceEntered=" + PriceEntered
                   + " -> PriceActual=" + PriceActual);
                mTab.setValue("PriceActual", PriceActual);
            }

            //	Check PriceLimit
            var epl = ctx.getContext("EnforcePriceLimit");
            var enforce = ctx.isSOTrx() && epl != null && epl == "Y";
            if (enforce && VIS.MRole.getDefault().IsOverwritePriceLimit()) {
                enforce = false;
            }
            //	Check Price Limit?
            if (enforce && Util.getValueOfDouble(PriceLimit) != 0.0
              && PriceActual.compareTo(PriceLimit) < 0) {
                PriceActual = PriceLimit;

                paramStr = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(), ",", //2
                   PriceActual.toString()); //3

                PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramStr);
                // PriceEntered = drPC.retValue;// (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //C_UOM_To_ID, PriceActual.Value);

                //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceLimit.Value);
                if (PriceEntered == null) {
                    PriceEntered = PriceLimit;
                }
                this.log.fine("amt =(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                mTab.setValue("PriceActual", PriceLimit);
                mTab.setValue("PriceEntered", PriceEntered);

                //mTab.fireDataStatusEEvent("UnderLimitPrice", "", false);
                // ShowMessage.Info("UnderLimitPrice", null, "", ""); //Temporary
                VIS.ADialog.info("UnderLimitPrice");
                //	Repeat Discount calc
                if (Util.getValueOfInt(PriceList) != 0) {
                    Discount = (PriceList - PriceActual) / PriceList * 100.0;
                    if (Util.scale(Discount) > 2) {
                        Discount = Discount.toFixed(2);// MidpointRounding.AwayFromZero);
                    }
                }
            }

            //	Line Net Amt
            var lineNetAmt = QtyInvoiced * PriceActual;
            if (Util.scale(lineNetAmt) > StdPrecision) {
                lineNetAmt = lineNetAmt.toFixed(StdPrecision);// MidpointRounding.AwayFromZero);
            }
            this.log.info("amt = LineNetAmt=" + lineNetAmt);
            mTab.setValue("LineNetAmt", lineNetAmt);

            //	Calculate Tax Amount for PO
            var isSOTrx1 = "Y" == ctx.getWindowContext(windowNo, "IsSOTrx", true);
            if (!isSOTrx1) {
                var taxAmt = VIS.Env.ZERO;
                if (mField.getColumnName() == "TaxAmt") {
                    taxAmt = mTab.getValue("TaxAmt");
                }
                else {
                    var taxID = mTab.getValue("C_Tax_ID");
                    if (taxID != null) {
                        var C_Tax_ID = taxID;//.intValue();
                        var IsTaxIncluded = this.IsTaxIncluded(windowNo, ctx);
                        var paramString = C_Tax_ID.toString().concat(",", lineNetAmt.toString(), ",", //2
                                                         IsTaxIncluded, ",", //3
                                                         StdPrecision.toString() //4 
                                                        ); //7          
                        var dr = null;
                        taxAmt = VIS.dataContext.getJSONRecord("MTax/CalculateTax", paramString);
                        //


                        //MTax tax = new MTax(ctx, C_Tax_ID, null);
                        // taxAmt = dr[0];



                        //MTax tax = new MTax(ctx, C_Tax_ID, null);
                        //taxAmt = tax.CalculateTax(lineNetAmt, IsTaxIncluded(windowNo), StdPrecision);
                        mTab.setValue("TaxAmt", taxAmt);
                    }
                }
                //	Add it up
                mTab.setValue("LineTotalAmt", (lineNetAmt + taxAmt));
            }

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     * 	Is Tax Included
     *	@param windowNo window no
     *	@return tax included (default: false)
     */
    CalloutInvoice.prototype.IsTaxIncluded = function (windowNo, ctx) {
        //  

        //var ctx = Env.getContext();
        var ss = ctx.getContext("IsTaxIncluded");
        try {
            //	Not Set Yet
            if (ss.toString().length == 0) {
                var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
                if (M_PriceList_ID == 0) {
                    return false;
                }
                ss = VIS.DB.executeScalar("SELECT IsTaxIncluded FROM M_PriceList WHERE M_PriceList_ID=" + M_PriceList_ID, null, null).toString();
                if (ss == null) {
                    ss = "N";
                }
                ctx.setContext(windowNo, "IsTaxIncluded", ss);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "Y" == ss;
    };

    /**
     *	Invoice Line - Quantity.
     *		- called from C_UOM_ID, QtyEntered, QtyInvoiced
     *		- enforces qty UOM relationship
     *	@param ctx context
     *	@param windowNo window no
     *	@param mTab tab
     *	@param mField field
     *	@param value value
     *	@return null or error message
     */
    CalloutInvoice.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        this.setCalloutActive(true);
        try {

            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            this.log.log(Level.WARNING, "qty - init - M_Product_ID=" + M_Product_ID);
            var QtyInvoiced, QtyEntered, PriceActual, PriceEntered;

            //	No Product
            if (M_Product_ID == 0) {
                QtyEntered = mTab.getValue("QtyEntered");
                mTab.setValue("QtyInvoiced", QtyEntered);
            }
                //	UOM Changed - convert from Entered -> Product
            else if (mField.getColumnName().toString().equals("C_UOM_ID")) {
                var C_UOM_To_ID = Util.getValueOfInt(value);//.intValue();
                QtyEntered = mTab.getValue("QtyEntered");
                var QtyEntered1 = null;

                if (QtyEntered != null) {
                    var paramString = C_UOM_To_ID.toString();
                    var precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramString);

                    //QtyEntered1 = Decimal.Round(QtyEntered.Value, MUOM.GetPrecision(ctx, C_UOM_To_ID));//, MidpointRounding.AwayFromZero);
                    QtyEntered1 = QtyEntered.toFixed(precision);
                }

                //if (QtyEntered.Value.compareTo(QtyEntered1.Value) != 0)
                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                                  ",", QtyEntered.toString());
                QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);

                // QtyInvoiced = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //  C_UOM_To_ID, QtyEntered);
                if (QtyInvoiced == null) {
                    QtyInvoiced = QtyEntered;
                }
                // bool conversion = QtyEntered.compareTo(QtyInvoiced) != 0;
                var conversion = QtyEntered != QtyInvoiced;

                PriceActual = mTab.getValue("PriceActual");

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                             ",", PriceActual.toString());
                PriceEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);

                //PriceEntered = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //   C_UOM_To_ID, PriceActual);
                if (PriceEntered == null) {
                    PriceEntered = PriceActual;
                }
                this.log.fine("qty - UOM=" + C_UOM_To_ID
                     + ", QtyEntered/PriceActual=" + QtyEntered + "/" + PriceActual
                     + " -> " + conversion
                     + " QtyInvoiced/PriceEntered=" + QtyInvoiced + "/" + PriceEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyInvoiced", QtyInvoiced);
                mTab.setValue("PriceEntered", PriceEntered);
            }
                //	QtyEntered changed - calculate QtyInvoiced
            else if (mField.getColumnName().equals("QtyEntered")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                QtyEntered = value;

                var QtyEntered1 = null;

                if (QtyEntered != null) {
                    var precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", C_UOM_To_ID.toString());
                    QtyEntered1 = QtyEntered.toFixed(precision);//, MidpointRounding.AwayFromZero);
                }

                //if (QtyEntered.Value.compareTo(QtyEntered1.Value) != 0)
                if (QtyEntered != QtyEntered1) {
                    this.log.fine("Corrected QtyEntered Scale UOM=" + C_UOM_To_ID
                         + "; QtyEntered=" + QtyEntered + "->" + QtyEntered1);
                    QtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", QtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", QtyEntered.toString());
                QtyInvoiced = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);

                //QtyInvoiced = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, QtyEntered);
                if (QtyInvoiced == null) {
                    QtyInvoiced = QtyEntered;
                }

                //bool conversion = QtyEntered.compareTo(QtyInvoiced) != 0;
                var conversion = QtyEntered != QtyInvoiced;

                this.log.fine("qty - UOM=" + C_UOM_To_ID
                     + ", QtyEntered=" + QtyEntered
                     + " -> " + conversion
                     + " QtyInvoiced=" + QtyInvoiced);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyInvoiced", QtyInvoiced);
            }
                //	QtyInvoiced changed - calculate QtyEntered (should not happen)
            else if (mField.getColumnName().equals("QtyInvoiced")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");

                QtyInvoiced = value;

                //  var precision = MProduct.Get(ctx, M_Product_ID).GetUOMPrecision();
                var paramString = M_Product_ID.toString();
                var precision = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramString);

                var QtyInvoiced1 = null;

                if (QtyInvoiced != null) {
                    QtyInvoiced1 = QtyInvoiced.toFixed(precision);//, MidpointRounding.AwayFromZero);
                }

                //if (QtyEntered.Value.compareTo(QtyEntered1.Value) != 0)
                if (QtyInvoiced != QtyInvoiced1) {
                    this.log.fine("Corrected QtyInvoiced Scale "
                         + QtyInvoiced + "->" + QtyInvoiced1);
                    QtyInvoiced = QtyInvoiced1;
                    mTab.setValue("QtyInvoiced", QtyInvoiced);
                }
                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                              ",", QtyInvoiced.toString());
                QtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);
                //  QtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //  C_UOM_To_ID, QtyInvoiced);
                if (QtyEntered == null) {
                    QtyEntered = QtyInvoiced;
                }

                //bool conversion = QtyInvoiced.compareTo(QtyEntered) != 0;
                var conversion = QtyInvoiced != QtyEntered;


                this.log.fine("qty - UOM=" + C_UOM_To_ID
                     + ", QtyInvoiced=" + QtyInvoiced
                     + " -> " + conversion
                     + " QtyEntered=" + QtyEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyEntered", QtyEntered);
            }
            //

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //MessageBox.Show("CalloutInvoice--Qty");
        }
        finally {

            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Header - PriceList.
    /// (used also in Invoice)
    /// - C_Currency_ID
    /// 	- IsTaxIncluded
    /// 	Window Context:
    /// 	- EnforcePriceLimit
    /// 	- StdPrecision
    /// 	- M_PriceList_Version_ID
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutInvoice.prototype.PriceList = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var sql = "";
        var dr = null;
        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var M_PriceList_ID = Util.getValueOfInt(value.toString());
            if (M_PriceList_ID == null || M_PriceList_ID == 0)
                return "";

            sql = "SELECT pl.IsTaxIncluded,pl.EnforcePriceLimit,pl.C_Currency_ID,c.StdPrecision,"
                + "plv.M_PriceList_Version_ID,plv.ValidFrom "
                + "FROM M_PriceList pl,C_Currency c,M_PriceList_Version plv "
                + "WHERE pl.C_Currency_ID=c.C_Currency_ID"
                + " AND pl.M_PriceList_ID=plv.M_PriceList_ID"
                + " AND pl.M_PriceList_ID=" + M_PriceList_ID						//	1
                + "ORDER BY plv.ValidFrom DESC";
            //	Use newest price list - may not be future

            //DataSet ds = VIS.DB..executeDataset(sql, null);
            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)

            dr = VIS.DB.executeReader(sql);
            if (dr.read()) {
                //DataRow dr = ds.Tables[0].Rows[i];
                //	Tax Included
                mTab.setValue("IsTaxIncluded", (Boolean)("Y".equals(dr.get("istaxincluded"))));
                //	Price Limit Enforce
                ctx.setContext(windowNo, "EnforcePriceLimit", dr.get("enforcepricelimit"));
                //	Currency
                var ii = Util.getValueOfInt(dr.get("c_currency_id"));
                mTab.setValue("C_Currency_ID", ii);
                var prislst = Util.getValueOfInt(dr.get("m_pricelist_version_id"));
                //	PriceList Version
                ctx.setContext(windowNo, "M_PriceList_Version_ID", prislst);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null)
                dr.close();
            this.log.log(Level.severe, sql, err);
            return err.message;
            //MessageBox.Show("Callout--PriceList");
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutInvoice = CalloutInvoice;
    //**************CalloutInvoice End*********


    //********CalloutInvoiceBatch Start ******

    function CalloutInvoiceBatch() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInvoiceBatch"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutInvoiceBatch, VIS.CalloutEngine);//inherit CalloutEngine

    //private static VLogger _log = VLogger.GetVLogger(typeof(CalloutInvoiceBatch).FullName);  //Sarab
    /// <summary>
    ///	Invoice Batch Line - DateInvoiced.	- updates DateAcct
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab"> tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutInvoiceBatch.prototype.Date = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        mTab.setValue("DateAcct", value);
        //
        this.SetDocumentNo(ctx, windowNo, mTab);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    //	date

    /// <summary>
    ///  Invoice Batch Line - BPartner.
    //		- C_BPartner_Location_ID
    //		- AD_User_ID
    //		- PaymentRule
    //		- C_PaymentTerm_ID
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutInvoiceBatch.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_BPartner_ID = Util.getValueOfInt(value);
        if (C_BPartner_ID == null || C_BPartner_ID == 0) {
            return "";
        }
        var sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
            + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
            + " p.SO_Description,p.IsDiscountPrinted,"
            + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
            + " l.C_BPartner_Location_ID,c.AD_User_ID,"
            + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID "
            + "FROM C_BPartner p"
            + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
            + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID AND l.IsBillTo='Y' AND l.IsActive='Y')"
            + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
            + "WHERE p.C_BPartner_ID=@Param1 AND p.IsActive='Y'";		//	#1

        var IsSOTrx = ctx.isSOTrx();
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[1];
        var idr = null;
        try {
            Param[0] = new VIS.DB.SqlParam("@Param1", C_BPartner_ID);
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                //	PaymentRule
                // var s = Util.getValueOfString(idr[]);
                var s = Util.getValueOfString(idr.get((IsSOTrx ? "paymentrule" : "paymentrulepo")));
                if (s != null && s.length != 0) {
                    if (ctx.getContext("DocBaseType").endsWith("C"))// .endsWith("C"))	//	Credits are Payment Term
                    {
                        s = "P";
                    }
                    else if (this.isSOTrx && (s == "S") || (s == "U"))	//	No Check/Transfer for SO_Trx
                    {
                        s = "P";											//  Payment Term
                    }
                }
                //  Payment Term
                //  var ii = Util.getValueOfInt(idr[IsSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"]);
                var ii = Util.getValueOfInt(idr.get((IsSOTrx ? "c_paymentterm_id" : "po_paymentterm_id")));
                if (ii > 0) {
                    mTab.setValue("C_PaymentTerm_ID", ii);
                }
                //	Location
                //var locID = Util.getValueOfInt(idr["C_BPartner_Location_ID"]);
                var locID = Util.getValueOfInt(idr.get("c_bpartner_location_id"));
                //	overwritten by InfoBP selection - works only if InfoWindow
                //	was used otherwise creates error (uses last value, may belong to differnt BP)
                if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.toString().length > 0) {
                        locID = Util.getValueOfInt(loc);
                    }
                }
                if (locID == 0) {
                    mTab.setValue("C_BPartner_Location_ID", null);
                }
                else {
                    mTab.setValue("C_BPartner_Location_ID", Util.getValueOfInt(locID));
                }

                //	Contact - overwritten by InfoBP selection
                //var contID = Util.getValueOfInt(idr["AD_User_ID"]);
                var contID = Util.getValueOfInt(idr.get("ad_user_id"));
                if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.toString().length > 0) {
                        contID = Util.getValueOfInt(cont);
                    }
                }
                if (contID == 0) {
                    mTab.setValue("AD_User_ID", null);
                }
                else {
                    mTab.setValue("AD_User_ID", Util.getValueOfInt(contID));
                }
                //	CreditAvailable
                if (IsSOTrx) {
                    var CreditLimit = Util.getValueOfDouble(idr.get("so_creditlimit"));
                    if (CreditLimit != 0) {
                        var CreditAvailable = Util.getValueOfDouble(idr.get("creditavailable"));
                        if (idr == null && CreditAvailable < 0) {
                            // ShowMessage.Info("CreditLimitOver", null, "", "");
                            VIS.ADialog.info("CreditLimitOver");
                        }
                    }
                }
            }
            idr.close();

        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.severe, sql, err);
            return err.message;
        }
        //
        this.SetDocumentNo(ctx, windowNo, mTab);
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };//	bPartner

    /// <summary>
    /// Document Type.
    //- called from DocType
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab"> tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutInvoiceBatch.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        this.SetDocumentNo(ctx, windowNo, mTab);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	docType

    /// <summary>
    /// Set Document No (increase existing)
    /// </summary>
    /// <param name="ctx"> Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab"> Model Tab</param>
    CalloutInvoiceBatch.prototype.SetDocumentNo = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //	Get last line
        //  
        var C_InvoiceBatch_ID = ctx.getContextAsInt(windowNo, "C_InvoiceBatch_ID");
        var sql = "SELECT COALESCE(MAX(C_InvoiceBatchLine_ID),0) FROM C_InvoiceBatchLine WHERE C_InvoiceBatch_ID=" + C_InvoiceBatch_ID;

        // var C_InvoiceBatchLine_ID = VIS.DB.getSQLValue(null, sql, C_InvoiceBatch_ID);
        var C_InvoiceBatchLine_ID = VIS.DB.executeScalar(sql);
        if (C_InvoiceBatchLine_ID == 0) {
            return;
        }


        var paramString = C_InvoiceBatchLine_ID.toString();

        //Get product price information
        var dr = null;
        dr = VIS.dataContext.getJSONRecord("MInvoiceBatchLine/GetInvoiceBatchLine", paramString);





        //MInvoiceBatchLine last = new MInvoiceBatchLine(Env.GetCtx(), C_InvoiceBatchLine_ID, null);
        //MInvoiceBatchLine last = new MInvoiceBatchLine(ctx, C_InvoiceBatchLine_ID, null);
        //	Need to Increase when different DocType or BP
        var C_DocType_ID = ctx.getContextAsInt(windowNo, "C_DocType_ID");
        var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
        //if (C_DocType_ID == last.GetC_DocType_ID()
        //    && C_BPartner_ID == last.GetC_BPartner_ID())
        //{
        //    return;
        //}
        if (C_DocType_ID == dr["C_DocType_ID"]
          && C_BPartner_ID == dr["C_BPartner_ID"]) {
            return;
        }
        //	New Number
        //var oldDocNo = last.getDocumentNo();
        var oldDocNo = dr["DocumentNo"];
        if (oldDocNo == null) {
            return;
        }
        var docNo = 0;
        docNo = Util.getValueOfInt(oldDocNo);
        if (docNo == 0) {
            return;
        }
        var newDocNo = Util.getValueOfString(docNo + 1);
        mTab.setValue("DocumentNo", newDocNo);
        ctx = windowNo = mTab = mField = value = oldValue = null;
    };	//	setDocumentNo

    /// <summary>
    /// Invoice Batch Line - Charge.
    //	- updates PriceEntered from Charge
    //  Calles tax
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutInvoiceBatch.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Charge_ID = Util.getValueOfInt(value);
        if (C_Charge_ID == null || Util.getValueOfInt(C_Charge_ID) == 0) {
            return "";
        }
        var sql = "SELECT ChargeAmt FROM C_Charge WHERE C_Charge_ID=@Param1";
        var Param = [];
        // SqlParameter[] Param = new SqlParameter[1];
        var idr = null;
        try {
            Param[0] = new VIS.DB.SqlParam("@Param1", C_Charge_ID);
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                mTab.setValue("PriceEntered", Util.getValueOfDecimal(idr.get(0)));
            }
            idr.close();

        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.severe, sql, err);
            return err.message;
        }
        //
        oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };	//	charge
    /// <summary>
    ///   Invoice Line - Tax.
    //		- basis: Charge, BPartner Location
    //		- sets C_Tax_ID
    // Calles Amount
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab"> tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutInvoiceBatch.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var column = mField.getColumnName();
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Charge_ID = 0;
        if (column.equals("C_Charge_ID")) {
            C_Charge_ID = Util.getValueOfInt(value);
        }
        else {
            C_Charge_ID = ctx.getContextAsInt(windowNo, "C_Charge_ID");
        }
        this.log.fine("C_Charge_ID=" + C_Charge_ID);
        if (C_Charge_ID == 0) {
            return this.Amt(ctx, windowNo, mTab, mField, value);	//
        }
        var C_BPartner_Location_ID = ctx.getContextAsInt(windowNo, "C_BPartner_Location_ID");
        if (C_BPartner_Location_ID == 0) {
            return this.Amt(ctx, windowNo, mTab, mField, value);	//
        }
        this.log.fine("BP_Location=" + C_BPartner_Location_ID);

        //	Dates
        //var billDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime(windowNo, "DateInvoiced"));//Sarab
        var billDate = Util.getValueOfDate(ctx.getContext("DateInvoiced"));
        this.log.fine("Bill Date=" + billDate);
        var shipDate = billDate;
        this.log.fine("Ship Date=" + shipDate);

        var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
        this.log.fine("Org=" + AD_Org_ID);

        var M_Warehouse_ID = ctx.getContextAsInt("#M_Warehouse_ID");
        this.log.fine("Warehouse=" + M_Warehouse_ID);

        var paramString = C_Charge_ID.toString().concat(",", billDate.toString(),
                                                             shipDate.toString(), ",",
                                                              AD_Org_ID.toString(), ",",
                                                              M_Warehouse_ID.toString(), ",",
                                                              C_BPartner_Location_ID.toString(), ",",
                                                              C_BPartner_Location_ID.toString(), ",",
                                                              ctx.getWindowContext(windowNo, "IsSOTrx", true).equals("Y"));


        var C_Tax_ID = VIS.dataContext.getJSONRecord("MTax/Get", paramString);

        //var C_Tax_ID = VAdvantage.Model.Tax.Get(ctx, 0, C_Charge_ID, billDate, shipDate,
        //AD_Org_ID, M_Warehouse_ID, C_BPartner_Location_ID, C_BPartner_Location_ID,
        // ctx.getContext("IsSOTrx").equals("Y"));



        if (C_Tax_ID == 0) {

            // ShowMessage.Error(VLogger.RetrieveError().toString(), true);
        }
        else {
            mTab.setValue("C_Tax_ID", Util.getValueOfInt(C_Tax_ID));
        }
        // ctx = windowNo = mTab = mField = value = oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    CalloutInvoiceBatch.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);

        var StdPrecision = ctx.getStdPrecision();

        //	get values
        var QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
        var PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
        this.log.fine("QtyEntered=" + QtyEntered + ", PriceEntered=" + PriceEntered);
        if (QtyEntered == null) {
            QtyEntered = VIS.Env.ZERO;
        }
        if (PriceEntered == null) {
            PriceEntered = VIS.Env.ZERO;
        }

        //	Line Net Amt
        var LineNetAmt = QtyEntered * PriceEntered;
        if (Util.scale(LineNetAmt) > StdPrecision) {
            LineNetAmt = LineNetAmt.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
        }

        //	Calculate Tax Amount
        var IsTaxIncluded = "Y" == ctx.getContext("IsTaxIncluded");

        var TaxAmt = null;
        if (mField.getColumnName().equals("TaxAmt")) {
            TaxAmt = mTab.getValue("TaxAmt");
        }
        else {
            var taxID = mTab.getValue("C_Tax_ID");
            if (taxID != null) {
                var C_Tax_ID = Util.getValueOfInt(taxID);

                //
                var paramString = C_Tax_ID.toString().concat(",", LineNetAmt.toString(), ",", //2
                                                              IsTaxIncluded.toString(), ",", //3
                                                              StdPrecision.toString() //4 
                                                             ); //7          
                var dr = null;
                TaxAmt = VIS.dataContext.getJSONRecord("MTax/CalculateTax", paramString);
                //


                //MTax tax = new MTax(ctx, C_Tax_ID, null);
                //TaxAmt = dr[0];
                mTab.setValue("TaxAmt", TaxAmt);
            }
        }

        //	
        if (IsTaxIncluded) {
            mTab.setValue("LineTotalAmt", LineNetAmt);
            mTab.setValue("LineNetAmt", (Util.getValueOfDecimal(LineNetAmt) * Util.getValueOfDecimal(TaxAmt)));
        }
        else {
            mTab.setValue("LineNetAmt", LineNetAmt);
            mTab.setValue("LineTotalAmt", (Util.getValueOfDecimal(LineNetAmt) + Util.getValueOfDecimal(TaxAmt)));

        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutInvoiceBatch = CalloutInvoiceBatch;
    //*************CalloutInvoiceBatch End*******



    //***********CalloutMovement Start*********
    /// <summary>
    /// Inventory Movement Callouts
    /// </summary>
    function CalloutMovement() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMovement"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutMovement, VIS.CalloutEngine);//inherit CalloutEngine

    /// <summary>
    /// Product modified
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="tab">model tab</param>
    /// <param name="field">model field</param>
    /// <param name="value">new value</param>
    /// <returns>Error message or ""</returns>
    CalloutMovement.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";
        //	Set Attribute
        if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
            && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0)
            mTab.setValue("M_AttributeSetInstance_ID", ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID"));
        else
            mTab.setValue("M_AttributeSetInstance_ID", null);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutMovement = CalloutMovement;
    //***********CalloutMovement End**********   


    //*************CalloutObjectData Start******
    function CalloutObjectData() {
        VIS.CalloutEngine.call(this, "VIS.CalloutObjectData"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutObjectData, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutObjectData.prototype.Objectchk = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        if (value == null) {
            return "";
        }

        var FO_OBJ_DATA_ID = Util.getValueOfInt(mTab.getValue("FO_OBJECT_DATA_ID"));//Object Data ID
        //Integer acc_ID=null; //Accomodation ID
        var oldstartdate;//From Date stored in Database
        var oldtilldate;//Till Date stored in Database
        var todaydate = Util.getValueOfDateTime(mTab.getValue("TODAYDATE"));//Date 
        if (FO_OBJ_DATA_ID == null) {
            return "";
        }
        else {
            var sql = "select FO_RES_ACCOMODATION_ID,DATE_FROM,TILL_DATE from FO_RES_ACCOMODATION where FO_OBJECT_DATA_ID=@FO_OBJ_DATA_ID";
            var dr = null;
            try {
                //SqlParameter[] param = new SqlParameter[1];
                var param = [];
                param[0] = new VIS.DB.SqlParam("@FO_OBJ_DATA_ID", FO_OBJ_DATA_ID);
                dr = VIS.DB.executeReader(sql, param, null);
                //PreparedStatement pst = DataBase.prepareStatement(sql, null);
                //pst.setInt(1, FO_OBJ_DATA_ID);
                //ResultSet rs = pst.executeQuery();
                while (dr.read()) {
                    //	acc_ID= rs.getInt(1);
                    oldstartdate = Util.getValueOfDateTime(dr[1]);
                    oldtilldate = Util.getValueOfDateTime(dr[2]);
                    // curr_date is the current date holding the cursor
                    var curr_date = oldstartdate;
                    //if (todaydate.after(oldtilldate) == true)
                    if (todaydate.compareTo(oldtilldate) > 0) {
                        mTab.setValue("RES_STATUS", false);
                        return "";
                    }
                        //else if (todaydate.before(oldstartdate) == true)
                    else if (todaydate.compareTo(oldstartdate) < 0) {
                        mTab.setValue("RES_STATUS", false);
                        return "";
                    }
                        //else if (todaydate.after(oldstartdate) && todaydate.before(oldtilldate))
                    else if ((todaydate.compareTo(oldstartdate) > 0) && (todaydate.compareTo(oldtilldate) < 0)) {
                        while (!(curr_date.compareTo(oldtilldate) == 0)) {
                            if (todaydate.compareTo(curr_date) == 0) {
                                mTab.setValue("RES_STATUS", true);
                            }
                            //curr_date = TimeUtil.addDays(curr_date, 1);
                            curr_date = curr_date.AddDays(1);
                        }
                    }
                }
                dr.close();
                //pst.close();
            }
            catch (err) {
                this.setCalloutActive(false);
                if (dr != null) {
                    dr.close();
                }
                this.log.severe(e.toString());
            }
            finally {
                if (dr != null) {
                    dr.close();
                }
            }
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutObjectData = CalloutObjectData;
    //*************CalloutObjectData End*********

    //************CalloutOffer Start********
    function CalloutOffer() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOffer"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOffer, VIS.CalloutEngine);//inherit CalloutEngine
    /**
 * 
 * @param ctx context
 * @param windowNo window no
 * @param mTab tab
 * @param mField field
 * @param value value
 * @return null or error message
 */
    CalloutOffer.prototype.Datechk = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";
        }
        var a = Util.getValueOfDateTime(value);
        if (a == null || a == 0) {
            return "";
        }
        var org_id = mTab.getValue("AD_ORG_ID");
        var days_Payable = 0;//Payable Days stored in settings window
        //String sql = "select  DAYSPAYABLE2 from fo_deposits where FO_DEPOSITS_ID=1000000 ";

        // Query changed because of the change of the table (Sandeep 6-10-2009)
        var sql = "select  DAYSPAYABLE2 from FO_SETTINGS where AD_ORG_ID=" + org_id;
        var dr = null;
        try {
            dr = VIS.DB.executeReader(sql, null, null);
            while (dr.read()) {
                days_Payable = Util.getValueOfInt(dr[0].toString());//.getInt(1);
                // Console.WriteLine("Days Payable"+days_Payable);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }

        var dt = Util.getValueOfDateTime(mTab.getValue("DATE1"));
        //Console.WriteLine("dt"+dt);
        //java.sql.Timestamp incdays;
        var incdays;
        incdays = dt.AddDays(days_Payable);//incdays = TimeUtil.AddDays(dt, days_Payable);
        //Console.WriteLine("Inc Days"+incdays);
        mTab.setValue("DATE2", incdays);

        var Offer;
        var Offer_num = 100;
        var sql1 = "select max(OFFERNO)from FO_OFFER";
        try {
            dr = VIS.DB.executeReader(sql1, null, null);
            while (dr.read()) {
                Offer_num = Util.getValueOfInt(dr[0].toString());//.getInt(1);
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        if (Offer_num == null | Offer_num == 0) {
            Offer = 100;

        }
        else {
            Offer = Offer_num + 1;

        }
        mTab.setValue("OFFERNO", Offer);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /**
* 
* @param ctx context
* @param windowNo window no
* @param mTab tab
* @param mField field
* @param value value
* @return null or error message
*/
    CalloutOffer.prototype.guestpricelist = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null) {
            return "";
        }
        //int add_ID = (int)value;
        var add_ID = Util.getValueOfInt(value);
        if (add_ID == null || add_ID == 0)
            return "";
        //int add_ID = (Integer)mTab.getValue("FO_ADDRESS_ID");
        var sql = "select FO_PRICE_LIST_ID from FO_ADDRESS_PRICE where FO_ADDRESS_ID=" + add_ID;
        var pricelist_ID = 0;
        var dr = null;
        try {
            //PreparedStatement pst = DataBase.prepareStatement(sql, null);

            //ResultSet rs = pst.executeQuery();
            dr = VIS.DB.executeReader(sql, null, null);
            while (dr.read()) {
                pricelist_ID = Util.getValueOfInt(dr[0]);//rs.getInt(1);
            }
            dr.close();
            // pst.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
            }
            this.log.severe(err.toString());
        }
        finally {
            if (dr != null) {
                dr.close();
            }
        }
        mTab.setValue("FO_PRICE_LIST_ID", pricelist_ID);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOffer = CalloutOffer;
    //************ CalloutOffer End *********

    //**********CalloutGLJournal Start*********

    function CalloutGLJournal() {
        VIS.CalloutEngine.call(this, "VIS.CalloutGLJournal");
    };
    //#endregion
    VIS.Utility.inheritPrototype(CalloutGLJournal, VIS.CalloutEngine); //inherit calloutengine

    /// <summary>
    /// Journal - Period.
    //  Check that selected period is in DateAcct Range or Adjusting Period
    //  Called when C_Period_ID or DateAcct, DateDoc changed
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">fields</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutGLJournal.prototype.Period = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        //  
        var colName = mField.getColumnName();

        this.setCalloutActive(true);

        var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID", false);
        var DateAcct = new Date();
        if (colName == "DateAcct") {
            //DateAcct = Util.getValueOfDateTime(value);
            // DateAcct = Util.getValueOfDateTime(value);
            DateAcct = value;
        }
        else {
            // DateAcct = Util.getValueOfDateTime(mTab.getValue("DateAcct"));
            //DateAcct = Util.getValueOfDateTime(mTab.getValue("DateAcct"));
            DateAcct = mTab.getValue("DateAcct");
        }
        var C_Period_ID = 0;
        if (colName == "C_Period_ID") {
            C_Period_ID = Util.getValueOfInt(value);

        }
        //  When DateDoc is changed, update DateAcct
        if (colName == "DateDoc") {
            mTab.setValue("DateAcct", value);
        }

            //  When DateAcct is changed, set C_Period_ID
        else if (colName == "DateAcct") {
            var sql = "SELECT C_Period_ID "
                + "FROM C_Period "
                + "WHERE C_Year_ID IN "
                + "	(SELECT C_Year_ID FROM C_Year WHERE C_Calendar_ID ="
                + "  (SELECT C_Calendar_ID FROM AD_ClientInfo WHERE AD_Client_ID=@param1))"
                + " AND @param2 BETWEEN StartDate AND EndDate"
                + " AND PeriodType='S'";
            var param = [];
            //SqlParameter[] param = new SqlParameter[2];
            var idr = null;
            try {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, AD_Client_ID);
                //pstmt.setTimestamp(2, DateAcct);
                param[0] = new VIS.DB.SqlParam("@param1", AD_Client_ID);
                param[1] = new VIS.DB.SqlParam("@param2", DateAcct);
                param[1].setIsDate(true);
                idr = VIS.DB.executeReader(sql, param, null);
                if (idr.read()) {
                    C_Period_ID = Util.getValueOfInt(idr.get(0));// rs.getInt(1);
                }
                idr.close();
            }
            catch (err) {
                if (idr != null) {
                    idr.close();
                }
                this.log.log(Level.SEVERE, sql, e);
                this.setCalloutActive(false);
                return err.message;
            }
            if (C_Period_ID != 0) {
                mTab.setValue("C_Period_ID", Util.getValueOfInt(C_Period_ID));
            }
        }
            //  When C_Period_ID is changed, check if in DateAcct range and set to end date if not
        else {
            var sql = "SELECT PeriodType, StartDate, EndDate "
                + "FROM C_Period WHERE C_Period_ID=@param";

            var param = [];
            //SqlParameter[] param = new SqlParameter[1];
            var idr = null;
            try {
                //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, C_Period_ID);
                param[0] = new VIS.DB.SqlParam("@param", C_Period_ID);
                idr = VIS.DB.executeReader(sql, param, null);
                if (idr.read()) {
                    var PeriodType = idr.get("periodtype");// rs.getString(1);
                    var StartDate = idr.get("startdate");// rs.getTimestamp(2);
                    var EndDate = idr.get("enddate");// rs.getTimestamp(3);
                    if (PeriodType == "S") //  Standard Periods
                    {
                        //  out of range - set to last day
                        if (DateAcct == null
                            || DateAcct < StartDate || DateAcct > EndDate)
                            mTab.setValue("DateAcct", EndDate);
                    }
                }
                idr.close();
            }
            catch (err) {
                if (idr != null) {
                    idr.close();
                }
                this.log.log(Level.SEVERE, sql, e);
                this.setCalloutActive(false);
                return e.message;
            }
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  	Journal_Period



    /// <summary>
    /// Journal/Line - rate.	Set CurrencyRate from DateAcct, C_ConversionType_ID, C_Currency_ID
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns>null or error message</returns>
    CalloutGLJournal.prototype.Rate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }

        //  Source info
        try {
            var Currency_ID = Util.getValueOfInt(mTab.getValue("C_Currency_ID"));
            var C_Currency_ID = Util.getValueOfInt(Currency_ID);
            var ConversionType_ID = Util.getValueOfInt(mTab.getValue("C_ConversionType_ID"));
            var C_ConversionType_ID = Util.getValueOfInt(ConversionType_ID);
            var DateAcct = mTab.getValue("DateAcct");
            if (DateAcct == null) {
                var currentDate = new Date();
                //DateAcct = DateTime.Now;//  new Timestamp(System.currentTimeMillis());
                DateAcct = currentDate.toGMTString();
            }
            //
            var C_AcctSchema_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_AcctSchema_ID", false));
            var paramString = C_AcctSchema_ID;
            var aas = VIS.dataContext.getJSONRecord("MAcctSchema/GetAcctSchema", paramString);
            // MAcctSchema aas = MAcctSchema.Get(ctx, C_AcctSchema_ID.Value);
            var AD_Client_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "AD_Client_ID", false));
            var AD_Org_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "AD_Org_ID", false));

            //var CurrencyRate = MConversionRate.GetRate(C_Currency_ID.Value, aas.GetC_Currency_ID(),
            //    DateAcct, C_ConversionType_ID.Value, AD_Client_ID.Value, AD_Org_ID.Value);
            var paramStr = C_Currency_ID + "," + aas["C_Currency_ID"] + "," + DateAcct + "," + C_ConversionType_ID + "," + AD_Client_ID + "," + AD_Org_ID;
            var CurrencyRate = VIS.dataContext.getJSONRecord("MConversionRate/GetRate", paramStr);
            this.log.fine("rate = " + CurrencyRate);
            if (CurrencyRate == null) {
                CurrencyRate = 0;
            }
            mTab.setValue("CurrencyRate", CurrencyRate);
        }
        catch (err) {

            this.log.log(Level.SEVERE, sql, e);
            this.setCalloutActive(false);
            return e.message;
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	rate

    /// <summary>
    /// JournalLine - Amt.  Convert the source amount to accounted amount (AmtAcctDr/Cr)Called when source amount (AmtSourceCr/Dr) or rate changes
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">window no</param>
    /// <param name="mTab">tab</param>
    /// <param name="mField">field</param>
    /// <param name="value">value</param>
    /// <returns> null or error message</returns>
    CalloutGLJournal.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        //	String colName = mField.getColumnName();
        if (value == null || value.toString() == "" || this.isCalloutActive()) {
            return "";
        }

        this.setCalloutActive(true);

        //  Get Target Currency & Precision from C_AcctSchema.C_Currency_ID
        var C_AcctSchema_ID = Util.getValueOfInt(ctx.getContextAsInt(windowNo, "C_AcctSchema_ID"));
        var paramString = C_AcctSchema_ID.toString();
        var aas = VIS.dataContext.getJSONRecord("MAcctSchema/GetAcctSchema", paramString);

        //MAcctSchema aas = MAcctSchema.Get(ctx, C_AcctSchema_ID);
        //var Precision = aas.GetStdPrecision();

        var Precision = aas["StdPrecision"];

        var CurrencyRate = mTab.getValue("CurrencyRate");
        if (CurrencyRate == null) {
            CurrencyRate = 1;
            mTab.setValue("CurrencyRate", CurrencyRate);
        }

        //  AmtAcct = AmtSource * CurrencyRate  ==> Precision
        var AmtSourceDr = mTab.getValue("AmtSourceDr");
        if (AmtSourceDr == null) {
            AmtSourceDr = 0;
        }
        var AmtSourceCr = mTab.getValue("AmtSourceCr");
        if (AmtSourceCr == null) {
            AmtSourceCr = 0;
        }

        // var AmtAcctDr = Decimal.Multiply(AmtSourceDr.Value, CurrencyRate.Value);

        var AmtAcctDr = AmtSourceDr * CurrencyRate;
        //AmtAcctDr = AmtAcctDr.setScale(Precision, BigDecimal.ROUND_HALF_UP);
        //AmtAcctDr = Decimal.Round(AmtAcctDr.Value, Precision);//, MidpointRounding.AwayFromZero);//   BigDecimal.ROUND_HALF_UP);
        AmtAcctDr.toFixed(Precision);
        mTab.setValue("AmtAcctDr", AmtAcctDr);
        //var AmtAcctCr = Decimal.Multiply(AmtSourceCr.Value, CurrencyRate.Value);
        var AmtAcctCr = AmtSourceCr * CurrencyRate;

        // AmtAcctCr = Decimal.Round(AmtAcctCr.Value, Precision);//, MidpointRounding.AwayFromZero);// BigDecimal.ROUND_HALF_UP);
        AmtAcctCr.toFixed(Precision);
        mTab.setValue("AmtAcctCr", AmtAcctCr);

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  amt

    //	CalloutGLJournal
    VIS.Model.CalloutGLJournal = CalloutGLJournal;
    //**************CalloutGLJournal End*********


    //***********CalloutProduction Start ******
    function CalloutProduction() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProduction"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutProduction, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Product modified
    /// Set Attribute Set Instance
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutProduction.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0) {
            return "";
        }
        //	Set Attribute
        if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
            && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0) {
            mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID")));
        }
        else {
            mTab.setValue("M_AttributeSetInstance_ID", null);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutProduction = CalloutProduction;
    //***********CalloutProduction End ******
    //***********CalloutRequest Start ******

    function CalloutRequest() {
        VIS.CalloutEngine.call(this, "VIS.CalloutRequest"); //must call
    }
    VIS.Utility.inheritPrototype(CalloutRequest, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Request - Copy Mail Text - <b>Callout</b>
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutRequest.prototype.CopyMail = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var colName = mField.getColumnName();
        this.log.info(colName + "=" + value);
        if (value == null || value.toString() == "") {
            return "";
        }
        var R_MailText_ID = Util.getValueOfInt(value);
        var sql = "SELECT MailHeader, MailText FROM R_MailText "
            + "WHERE R_MailText_ID=@Param1";
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[1];
        var idr = null;
        try {
            //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);

            //pstmt.setInt(1, R_MailText_ID.intValue());
            Param[0] = new VIS.DB.SqlParam("@Param1", Util.getValueOfInt(R_MailText_ID));
            //ResultSet rs = pstmt.executeQuery();
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                //String txt = rs.getString(2);
                var txt = idr.get("mailtext");
                txt = VIS.Env.parseContext(ctx, windowNo, txt, false, true);
                mTab.setValue("Result", txt);
            }
            idr.close();

        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  copyText


    /// <summary>
    /// Request - Copy Response Text - <b>Callout</b>
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>message or ""</returns>
    CalloutRequest.prototype.CopyResponse = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var colName = mField.getColumnName();
        this.log.info(colName + "=" + value);
        if (value == null || value.toString() == "") {
            return "";
        }
        var R_StandardResponse_ID = Util.getValueOfInt(value);
        var sql = "SELECT Name, ResponseText FROM R_StandardResponse "
            + "WHERE R_StandardResponse_ID=@Param1";
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[1];
        var idr = null;
        try {
            //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
            Param[0] = new VIS.DB.SqlParam("@Param1", Util.getValueOfInt(R_StandardResponse_ID));
            //pstmt.setInt(1, R_StandardResponse_ID.intValue());
            //ResultSet rs = pstmt.executeQuery();
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                //tring txt = rs.getString(2);
                //var txt = Util.getValueOfString(idr[1]);
                var txt = idr.get("responsetext");
                txt = VIS.Env.parseContext(ctx, windowNo, txt, false, true);
                mTab.setValue("Result", txt);
            }
            idr.close();

        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };  //  copyResponse

    /// <summary>
    /// Request - Chane of Request Type - <b>Callout</b>
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value"> The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutRequest.prototype.Type = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var colName = mField.getColumnName();
        this.log.info(colName + "=" + value);
        mTab.setValue("R_Status_ID", null);
        if (value == null || value.toString() == "") {
            return "";
        }
        //var R_RequestType_ID = ((Integer)value).intValue();
        var R_RequestType_ID = Util.getValueOfInt(value);
        if (R_RequestType_ID == 0) {
            return "";
        }
        var paramString = R_RequestType_ID.toString();


        //Get BankAccount information

        var R_Status_ID = VIS.dataContext.getJSONRecord("MRequestType/GetDefaultR_Status_ID", paramString);
        //MRequestType rt = MRequestType.Get(ctx, R_RequestType_ID);
        // var R_Status_ID = rt.GetDefaultR_Status_ID();
        if (R_Status_ID != 0) {
            //mTab.setValue("R_Status_ID", new Integer(R_Status_ID));
            mTab.setValue("R_Status_ID", Util.getValueOfInt(R_Status_ID));

        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };//	type
    VIS.Model.CalloutRequest = CalloutRequest;
    //***********CalloutRequest End ******

    //***********CalloutRequisition Start ******
    function CalloutRequisition() {
        VIS.CalloutEngine.call(this, "VIS.CalloutRequisition"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutRequisition, VIS.CalloutEngine);//inherit CalloutEngine

    /** Logger					*/


    /**
     *	Requisition Line - Product.
     *		- PriceStd
     *  @param ctx context
     *  @param WindowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutRequisition.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var M_Product_ID = value;
            if (M_Product_ID == null || M_Product_ID == 0)
                return "";
            //	setCalloutActive(true);
            //
            /**	Set Attribute
            if (ctx.getContextAsInt( Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID.intValue()
                && ctx.getContextAsInt( Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID") != 0)
                mTab.setValue("M_AttributeSetInstance_ID", Integer.valueOf(ctx.getContextAsInt( Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID")));
            else
                mTab.setValue("M_AttributeSetInstance_ID", null);
            **/
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
            var qty = mTab.getValue("Qty");
            var isSOTrx = false;
            //  MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //     M_Product_ID, C_BPartner_ID, qty, isSOTrx);
            //
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            // pp.SetM_PriceList_ID(M_PriceList_ID);
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
            //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            //DateTime orderDate = (DateTime)mTab.getValue("DateRequired");
            var orderDate = mTab.getValue("DateRequired");
            // pp.SetPriceDate(orderDate);


            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID, ",", //2
                                                        qty, ",", //3
                                                        isSOTrx, ",", //4 
                                                        M_PriceList_ID, ",", //5
                                                        M_PriceList_Version_ID, ",", //6
                                                        orderDate, ",", null); //7


            //Get product price information
            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);
            mTab.setValue("PriceActual", dr["PriceActual"]);

            //		
            //mTab.setValue("PriceActual", pp.GetPriceStd());
            ctx.setContext(windowNo, "EnforcePriceLimit", dr["EnforcePriceLimit"] ? "Y" : "N");	//	not used
            ctx.setContext(windowNo, "DiscountSchema", dr["DiscountSchema"] ? "Y" : "N");

            var sql = "SELECT C_OrderLine_ID FROM C_OrderLine"
                                           + " WHERE C_Order_ID ="
                                           + " (SELECT C_Order_ID "
                                           + " FROM C_Order "
                                           + " WHERE DocumentNo="
                                           + " (SELECT DocumentNo FROM M_Requisition WHERE M_Requisition.M_Requisition_id = " + mTab.getValue("M_Requisition_id") + ")"
                                           + " AND AD_Client_ID =" + ctx.getAD_Client_ID() + ")"
                                           + " AND M_Product_ID=" + value;
            var OrderLine = Util.getValueOfInt(VIS.DB.executeScalar(sql));
            if (OrderLine > 0) {
                mTab.setValue("C_OrderLine_ID", OrderLine);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err.message;
            //MessageBox.Show("CalloutRequisition- Product");
        }
        //	setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /**
     *	Order Line - Amount.
     *		- called from Qty, PriceActual
     *		- calculates LineNetAmt
     *  @param ctx context
     *  @param WindowNo current Window No
     *  @param mTab Grid Tab
     *  @param mField Grid Field
     *  @param value New Value
     *  @return null or error message
     */
    CalloutRequisition.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive() || value == null)
            return "";
        try {
            this.setCalloutActive(true);

            //	Qty changed - recalc price
            if (mField.getColumnName() == "Qty"
                && "Y" == ctx.getContext("DiscountSchema")) {
                var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                var qty = value;
                var isSOTrx = false;
                var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
                //pp.SetM_PriceList_ID(M_PriceList_ID);
                var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                ///DateTime orderDate = (DateTime)mTab.getValue("DateInvoiced");
                var orderDate = mTab.getValue("DateInvoiced");
                // pp.SetPriceDate(orderDate);
                //

                //*******************
                var paramString = M_Product_ID.concat(",", C_BPartner_ID, ",", //2
                                                          qty, ",", //3
                                                          isSOTrx, ",", //4 
                                                          M_PriceList_ID, ",", //5
                                                          M_PriceList_Version_ID, ",", //6
                                                          orderDate, ",", null); //7


                //Get product price information
                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


                //var rowDataDB = null;
                ////	only one row
                ////if (dr.read())
                //{
                //    //rowDataDB = this.readData(dr);

                //    mTab.setValue("PriceList", dr["PriceList"]);
                //    mTab.setValue("PriceLimit", dr.PriceLimit);
                //    mTab.setValue("PriceActual", dr.PriceActual);
                //    mTab.setValue("PriceEntered", dr.PriceEntered);
                //    mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
                //    mTab.setValue("Discount", dr.Discount);
                //    mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
                //    mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));
                //    ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
                //    ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");
                //}
                //*******************








                //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
                //    M_Product_ID, C_BPartner_ID, qty, isSOTrx);
                //

                mTab.setValue("PriceActual", dr["PriceActual"]);
            }
            var StdPrecision = ctx.getStdPrecision();
            var Qty = mTab.getValue("Qty");
            //Decimal PriceActual = (Decimal)mTab.getValue("PriceActual");
            var PriceActual = mTab.getValue("PriceActual");

            //	get values
            this.log.fine("amt - Qty=" + Qty + ", Price=" + PriceActual + ", Precision=" + StdPrecision);

            //	Multiply
            var LineNetAmt = Qty * PriceActual;
            if (Util.scale(LineNetAmt) > StdPrecision)
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);//, MidpointRounding.AwayFromZero);
            mTab.setValue("LineNetAmt", LineNetAmt);
            this.log.info("amt - LineNetAmt=" + LineNetAmt);
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.saveError("CalloutRequisation", err);
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutRequisition = CalloutRequisition;
    //*********** CalloutRequisition End *******

    //************ CalloutPaySelection Start ***
    function CalloutPaySelection() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPaySelection"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutPaySelection, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Payment Selection Line - Payment Amount.
    /// - called from C_PaySelectionLine.PayAmt
    /// - update DifferenceAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value">New Value</param>
    /// <returns> null or error message</returns>
    CalloutPaySelection.prototype.PayAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        if (this.isCalloutActive() || value == null || value == "") {
            return "";
        }
        //	get invoice info
        var ii = Util.getValueOfInt(mTab.getValue("C_Invoice_ID"));
        if (ii == null) {
            return "";
        }
        var C_Invoice_ID = ii;// ii.intValue();
        if (C_Invoice_ID == 0) {
            return "";
        }
        //
        var OpenAmt = Util.getValueOfDecimal(mTab.getValue("OpenAmt"));
        var PayAmt = Util.getValueOfDecimal(mTab.getValue("PayAmt"));
        var DiscountAmt = Util.getValueOfDecimal(mTab.getValue("DiscountAmt"));
        this.setCalloutActive(true);
        // var DifferenceAmt = Decimal.Subtract(Decimal.Subtract(OpenAmt, PayAmt), DiscountAmt);

        var DifferenceAmt = ((OpenAmt - PayAmt) - DiscountAmt);


        this.log.fine(" - OpenAmt=" + OpenAmt + " - PayAmt=" + PayAmt
            + ", Discount=" + DiscountAmt + ", Difference=" + DifferenceAmt);

        mTab.setValue("DifferenceAmt", DifferenceAmt);

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Payment Selection Line - Invoice.
    /// - called from C_PaySelectionLine.C_Invoice_ID
    /// - update PayAmt & DifferenceAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutPaySelection.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value == "") {
            return "";
        }
        //	get value
        var C_Invoice_ID = Util.getValueOfInt(value);
        if (C_Invoice_ID == 0) {
            return "";
        }
        var C_BankAccount_ID = ctx.getContextAsInt(windowNo, "C_BankAccount_ID");
        //var PayDate = CommonFunctions.CovertMilliToDate(ctx.getContextAsTime("PayDate"));// new DateTime(ctx.getContextAsTime("PayDate"));

        var PayDate = ctx.getContext("PayDate");
        this.setCalloutActive(true);

        var OpenAmt = VIS.Env.ZERO;


        var DiscountAmt = VIS.Env.ZERO;
        var isSOTrx = false;//            Boolean.FALSE;
        var sql = "SELECT currencyConvert(invoiceOpen(i.C_Invoice_ID, 0), i.C_Currency_ID,"
                + "ba.C_Currency_ID, i.DateInvoiced, i.C_ConversionType_ID, i.AD_Client_ID, i.AD_Org_ID) as OpenAmt,"
            + " paymentTermDiscount(i.GrandTotal,i.C_Currency_ID,i.C_PaymentTerm_ID,i.DateInvoiced,'" + PayDate + "') As DiscountAmt, i.IsSOTrx "
            + "FROM C_Invoice_v i, C_BankAccount ba "
            + "WHERE i.C_Invoice_ID=@Param2 AND ba.C_BankAccount_ID=@Param3";	//	#1..2
        var Param = [];
        //SqlParameter[] Param = new SqlParameter[3];
        var idr = null;
        try {
            //Param[0] = new VIS.DB.SqlParam("@Param1", PayDate);  //Temporary Commented By Sarab
            Param[0] = new VIS.DB.SqlParam("@Param2", C_Invoice_ID);
            Param[1] = new VIS.DB.SqlParam("@Param3", C_BankAccount_ID);
            idr = VIS.DB.executeReader(sql, Param, null);
            if (idr.read()) {
                OpenAmt = idr.get("openamt");
                DiscountAmt = idr.get("discountamt");
                IsSOTrx = "Y" == idr.get("issotrx");
            }
            idr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
        }

        this.log.fine(" - OpenAmt=" + OpenAmt + " (Invoice=" + C_Invoice_ID + ",BankAcct=" + C_BankAccount_ID + ")");
        mTab.setValue("OpenAmt", OpenAmt);
        mTab.setValue("PayAmt", (OpenAmt - DiscountAmt));
        mTab.setValue("DiscountAmt", DiscountAmt);
        mTab.setValue("DifferenceAmt", VIS.Env.ZERO);
        mTab.setValue("IsSOTrx", isSOTrx ? "Y" : "N");

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutPaySelection = CalloutPaySelection;
    //************ CalloutPaySelection End ******

    //*************CalloutOrderLine Start**************
    function CalloutOrderLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrderLine"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOrderLine, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutOrderLine.prototype.EndDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return ""; //Must sir but it was not the case
        }
        var SDate = new Date(mTab.getValue("StartDate"));
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        //var Sql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency; //By Sarab
        var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
        var days = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var invoice = Util.getValueOfInt(mTab.getValue("NoofCycle"));
        //var End = SDate.addDays(days * invoice);     //By sarab 
        //var End = SDate.setMonth(SDate.getMonth() + (days * invoice));        // By Karan
        var End = SDate.getDate() + "/" + (SDate.getMonth() + (months * invoice)) + "/" + SDate.getFullYear();
        if (End <= 0) {
            End = new Date();
        }
        else {
            End = new Date(End);
        }
        End = End.toISOString();
        mTab.setValue("EndDate", End);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutOrderLine.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return "";
        }
        var Cycles = Util.getValueOfDecimal(value);
        var cyclesCount = Util.getValueOfDecimal(mTab.getValue("QtyPerCycle"));
        var qty = Cycles * cyclesCount;
        mTab.setValue("QtyEntered", qty);
        mTab.setValue("QtyOrdered", qty);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutOrderLine.prototype.QtyPerCycle = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var cyclesCount = Util.getValueOfDecimal(value);
        var Cycles = Util.getValueOfDecimal(mTab.getValue("NoofCycle"));
        var qty = Cycles * cyclesCount;
        mTab.setValue("QtyEntered", qty);
        mTab.setValue("QtyOrdered", qty);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOrderLine = CalloutOrderLine;
    //**************CalloutOrderLine End*****************

    //***********CalloutOrderlineRecording Start********
    function CalloutOrderlineRecording() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrderlineRecording"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutOrderlineRecording, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutOrderlineRecording.prototype.Orderline = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var paramString = Util.getValueOfInt(value);
            var dr = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", paramString);

            //X_C_OrderLine ol = new X_C_OrderLine(Env.GetCtx(), Util.getValueOfInt(value), null);
            mTab.setValue("M_Product_ID", dr["M_Product_ID"]);
            mTab.setValue("Qty", dr["Qty"]);
            mTab.setValue("C_UOM_ID", dr["C_UOM_ID"]);
            mTab.setValue("C_BPartner_ID", dr["C_BPartner_ID"]);
            mTab.setValue("PlannedHours", dr["PlannedHours"]);
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOrderlineRecording = CalloutOrderlineRecording;
    //***********CalloutOrderlineRecording End********

    //****************CalloutCurrency Start ******
    function CalloutCurrency() {
        VIS.CalloutEngine.call(this, "VIS.CalloutCurrency"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutCurrency, VIS.CalloutEngine);//inherit CalloutEngine

    CalloutCurrency.prototype.Currency = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var sql = "";
        try {
            sql = "select C_Currency_id from m_pricelist where m_pricelist_ID = " + Util.getValueOfInt(value);
            var C_Currency_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            mTab.setValue("C_Currency_ID", C_Currency_ID);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };
    VIS.Model.CalloutCurrency = CalloutCurrency;
    //****************CalloutCurrency End ******

    //****************CalloutContractQty Start ******
    function CalloutContractQty() {
        VIS.CalloutEngine.call(this, "VIS.CalloutContractQty"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutContractQty, VIS.CalloutEngine);//inherit CalloutEngine
    //	Debug Steps		
    /// <summary>
    ///
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutContractQty.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        var price = Util.getValueOfDecimal(mTab.getValue("PriceActual"));
        var val = Util.getValueOfDecimal(value);
        //
        var C_Tax_ID = 0;
        var Rate = VIS.Env.ZERO;
        var LineAmount = "";
        var TotalRate = VIS.Env.ZERO;
        mTab.setValue("LineNetAmt", price * val);

        C_Tax_ID = Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
        var sqltax = "select rate from c_tax WHERE c_tax_id=" + C_Tax_ID + "";
        Rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sqltax, null, null));
        var LineNetAmt = Util.getValueOfDecimal(mTab.getValue("LineNetAmt"));
        TotalRate = Util.getValueOfDecimal((Util.getValueOfDecimal(LineNetAmt) * Util.getValueOfDecimal(Rate)) / 100);
        TotalRate = TotalRate.toFixed(2);
        mTab.setValue("taxamt", TotalRate);
        mTab.setValue("GrandTotal", ((price * val) + Util.getValueOfDecimal(mTab.getValue("TaxAmt"))));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutContractQty = CalloutContractQty;
    //****************CalloutContractQty End ******

    //*************CalloutContractProduct Start ******
    function CalloutContractProduct() {
        VIS.CalloutEngine.call(this, "VIS.CalloutContractProduct");//must call
    };
    VIS.Utility.inheritPrototype(CalloutContractProduct, VIS.CalloutEngine); //inherit prototype
    CalloutContractProduct.prototype.ContractProduct = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }

        var M_Product_ID = Util.getValueOfInt(mTab.getValue("M_Proudct_ID"));
        if (M_Product_ID != 0) {
            if (Util.getValueOfBool(value)) {
                mTab.setValue("PriceList", 0);
                mTab.setValue("PriceLimit", 0);
                mTab.setValue("PriceActual", 0);
                mTab.setValue("PriceEntered", 0);
            }
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutContractProduct = CalloutContractProduct;
    //************* CalloutContractProduct End ******

    //*************** CalloutContract ************  
    function CalloutContract() {
        VIS.CalloutEngine.call(this, "VIS.CalloutContract");//must call
    };
    VIS.Utility.inheritPrototype(CalloutContract, VIS.CalloutEngine); //inherit prototype
    CalloutContract.prototype.InvoiceCount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }

        var SDate = Util.getValueOfDateTime(mTab.getValue("StartDate"));
        var Edate = mTab.getValue("EndDate");
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));

        var Sql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
        var days = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var totaldays = (Edate - SDate).Days;
        var count = totaldays / days;
        mTab.setValue("TotalInvoice", count);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";

    };


    CalloutContract.prototype.EndDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        var SDate = new Date(mTab.getValue("StartDate"));
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
        var months = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var invoice = Util.getValueOfInt(mTab.getValue("TotalInvoice"));
        //var End = SDate.AddMonths(months * invoice).AddDays(-1);
        //   var End = SDate.setDate(SDate.getMonth() + (months * invoice));  by Karan
        var End = SDate.getDate() + "/" + (SDate.getMonth() + (months * invoice)) + "/" + SDate.getFullYear();
        mTab.setValue("EndDate", End);
        End = new Date(End);
        End = End.toISOString();
        SDate = SDate.toISOString();
        if (End < SDate) {
            mTab.setValue("EndDate", SDate);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutContract.prototype.BillStartDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var SDate = new Date(mTab.getValue("StartDate"));
        var billStartDate = new Date(mTab.getValue("BillStartDate"));
        if (billStartDate.toISOString() < SDate.toISOString()) {
            // ShowMessage.Info("StartDateShouldBeLessThanBillStartDate", true, null, null);
            VIS.ADialog.info("StartDateShouldBeLessThanBillStartDate");
            mTab.setValue("BillStartDate", mTab.getValue("StartDate"));
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutContract.prototype.StartDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var SDate = new Date(mTab.getValue("StartDate"));
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        var Sql = "Select NoOfDays from C_Frequency where C_Frequency_ID=" + frequency;
        var days = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var totalInvoice = Util.getValueOfInt(mTab.getValue("TotalInvoice"));
        var cycles = totalInvoice * days;
        //var endDate =SDate.addDays(Util.getValueOfDouble(cycles));
        var endDate = new Date(SDate.setDate(SDate.getDay() + Util.getValueOfDouble(cycles)));
        endDate = endDate.toISOString();
        mTab.setValue("EndDate", endDate);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    CalloutContract.prototype.StartDateChange = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var frequency = Util.getValueOfInt(mTab.getValue("C_Frequency_ID"));
        var Sql = "Select NoOfMonths from C_Frequency where C_Frequency_ID=" + frequency;
        var months = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));
        var totalInvoice = Util.getValueOfInt(mTab.getValue("TotalInvoice"));
        var SDate = new Date(value);
        //var endDate = SDate.addMonths(months * totalInvoice).addDays(-1);
        var endDate = SDate.setDate(SDate.getMonth() + (months * totalInvoice));
        endDate = new Date(SDate.setDate(SDate.getDay() - 1));
        mTab.setValue("EndDate", endDate.toISOString());
        if (endDate < SDate) {
            mTab.setValue("EndDate", SDate);
        }
        mTab.setValue("BillStartDate", mTab.getValue("StartDate"));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutContract = CalloutContract;
    //***************CalloutContract End ************

    //*************copyname Starts****************
    function copyname() {
        VIS.CalloutEngine.call(this, "VIS.copyname");//must call
    };
    VIS.Utility.inheritPrototype(copyname, VIS.CalloutEngine); //inherit prototype
    copyname.prototype.product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        mTab.setValue("Help", mTab.getValue("Name"));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    copyname.prototype.product2 = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        mTab.setValue("Description", mTab.getValue("Help"));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.copyname = copyname;
    //****************copyname Ends*************


    //*************CalloutUserTimeRec Start******************

    function CalloutUserTimeRec() {
        VIS.CalloutEngine.call(this, "VIS.CalloutUserTimeRec");//must call
    };
    VIS.Utility.inheritPrototype(CalloutUserTimeRec, VIS.CalloutEngine); //inherit prototype

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutUserTimeRec.prototype.IsInternal = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            // Util.getValueOfInt(value);
            var sql = "select ProfileType from S_Resource where AD_User_ID = " + Util.getValueOfInt(value);
            var pType = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
            if (pType != "") {
                if (pType.toUpper() == "I") {
                    mTab.setValue("IsInternal", true);
                }
                else if (pType.toUpper() == "E") {
                    mTab.setValue("IsInternal", false);
                }
            }
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutUserTimeRec = CalloutUserTimeRec;
    //**************** CalloutUserTimeRec End ***************

    //*************CalloutTimeExpense Start*******************
    function CalloutTimeExpense() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTimeExpense");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTimeExpense, VIS.CalloutEngine); //inherit prototype



    /// <summary>
    /// Expense Report Line
    //- called from M_Product_ID, S_ResourceAssignment_ID
    //set ExpenseAmt

    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current window no</param>
    /// <param name="mTab">grid tab</param>
    /// <param name="mField">grid field</param>
    /// <param name="value">new value</param>
    /// <returns>null or error message</returns>
    CalloutTimeExpense.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        var M_Product_ID = value;
        if (M_Product_ID == null || Util.getValueOfInt(M_Product_ID) == 0) {
            return "";
        }
        this.setCalloutActive(true);
        var priceActual = null;

        //	get expense date - or default to today's date
        //var DateExpense = VIS.Env.ctx.getContext("DateExpense");
        var DateExpense = ctx.getContext(windowNo, "DateExpense");
        var sql = null;
        var idr = null;
        try {
            var noPrice = true;

            //	Search Pricelist for current version
            sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"
                + "bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"
                + "bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"
                + "p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList_Version pv "
                + "WHERE p.M_Product_ID=pp.M_Product_ID"
                + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                + " AND pv.M_PriceList_ID=pl.M_PriceList_ID"
                + " AND pv.IsActive='Y'"
                + " AND p.M_Product_ID=@param1"		//	1
                + " AND pl.M_PriceList_ID=@param2"	//	2
                + " ORDER BY pv.ValidFrom DESC";
            //PreparedStatement pstmt = DataBase.prepareStatement(sql, null);
            var param = [];
            //pstmt.setInt(1, M_Product_ID.intValue());
            param[0] = new VIS.DB.SqlParam("@param1", Util.getValueOfInt(M_Product_ID));
            //pstmt.setInt(2, ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
            param[1] = new VIS.DB.SqlParam("@param2", ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
            //ResultSet rs = pstmt.executeQuery();
            idr = VIS.DB.executeReader(sql, param, null);
            while (idr.read() && noPrice) {
                // DateTime plDate = rs.GetDateTime("ValidFrom");
                var plDate = idr.get("validfrom");//.GetDateTime("ValidFrom");
                //	we have the price list
                //	if order date is after or equal PriceList validFrom
                // if (plDate == null || !DateExpense.before(plDate))
                if (plDate == null || !(DateExpense < plDate)) {
                    noPrice = false;
                    //	Price
                    //priceActual =Util.getValueOfDecimal(idr["PriceStd"]);//.GetDecimal("PriceStd");
                    priceActual = Util.getValueOfDecimal(idr.get("pricestd"));//.GetDecimal("PriceStd");

                    if (priceActual == null) {
                        priceActual = Util.getValueOfDecimal(idr.get("pricelist"));//.GetDecimal("PriceList");
                    }
                    if (priceActual == null) {
                        priceActual = Util.getValueOfDecimal(idr.get("pricelimit"));//.GetDecimal("PriceLimit");
                    }
                    //	Currency
                    var ii = Util.getValueOfInt(idr.get("c_currency_id"));
                    if (!(idr == null)) {
                        mTab.setValue("C_Currency_ID", ii);
                    }
                }
            }
            idr.close();
            //	no prices yet - look base pricelist
            if (noPrice) {
                //	Find if via Base Pricelist
                sql = "SELECT bomPriceStd(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceStd,"
                    + "bomPriceList(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceList,"
                    + "bomPriceLimit(p.M_Product_ID,pv.M_PriceList_Version_ID) AS PriceLimit,"
                    + "p.C_UOM_ID,pv.ValidFrom,pl.C_Currency_ID "
                    + "FROM M_Product p, M_ProductPrice pp, M_PriceList pl, M_PriceList bpl, M_PriceList_Version pv "
                    + "WHERE p.M_Product_ID=pp.M_Product_ID"
                    + " AND pp.M_PriceList_Version_ID=pv.M_PriceList_Version_ID"
                    + " AND pv.M_PriceList_ID=bpl.M_PriceList_ID"
                    + " AND pv.IsActive='Y'"
                    + " AND bpl.M_PriceList_ID=pl.BasePriceList_ID"	//	Base
                    + " AND p.M_Product_ID=@param1"		//  1
                    + " AND pl.M_PriceList_ID=@param2"	//	2
                    + " ORDER BY pv.ValidFrom DESC";
                var param1 = [];
                //pstmt = DataBase.prepareStatement(sql, null);
                //pstmt.setInt(1, M_Product_ID.intValue());
                param1[0] = new VIS.DB.SqlParam("@param1", Util.getValueOfInt(M_Product_ID));

                //pstmt.setInt(2, ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
                param1[1] = new VIS.DB.SqlParam("@param2", ctx.getContextAsInt(windowNo, "M_PriceList_ID"));
                //rs = pstmt.executeQuery();
                idr = VIS.DB.executeReader(sql, param1, null);
                while (idr.read() && noPrice) {
                    var plDate = idr.get("validfrom");//.GetDateTime("ValidFrom");
                    //	we have the price list
                    //	if order date is after or equal PriceList validFrom
                    if (plDate == null || !(DateExpense < plDate)) {
                        noPrice = false;
                        //	Price
                        priceActual = Util.getValueOfDecimal(idr.get("pricestd"));//.GetDecimal("PriceStd");
                        if (priceActual == null) {
                            priceActual = Util.getValueOfDecimal(idr.get("pricelist"));//.GetDecimal("PriceList");
                        }
                        if (priceActual == null) {
                            priceActual = Util.getValueOfDecimal(idr.get("pricelimit"));//.GetDecimal("PriceLimit");
                        }
                        //	Currency
                        var ii = Util.getValueOfInt(idr.get("c_currency_id"));
                        if (!(idr == null)) {
                            mTab.setValue("C_Currency_ID", ii);
                        }
                    }
                }
                idr.close();
            }
        }
        catch (err) {
            if (idr != null) {
                idr.close();
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return e.message;//.getLocalizedMessage();
        }

        //	finish
        this.setCalloutActive(false);	//	calculate amount
        if (priceActual == null)
            priceActual = VIS.Env.ZERO;
        mTab.setValue("ExpenseAmt", priceActual);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	Expense_Product

    /// <summary>
    ///  Expense - Amount.- called from ExpenseAmt, C_Currency_ID,- calculates ConvertedAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns> null or error message</returns>
    CalloutTimeExpense.prototype.Amount = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);

        //	get values
        var ExpenseAmt = mTab.getValue("ExpenseAmt");
        var C_Currency_From_ID = mTab.getValue("C_Currency_ID");
        var C_Currency_To_ID = ctx.getContextAsInt(windowNo, "$C_Currency_ID");
        //DateTime DateExpense = new DateTime(ctx.getContextAsTime(windowNo, "DateExpense"));
        var DateExpense = ctx.getContext(windowNo, "DateExpense");
        //
        this.log.fine("Amt=" + ExpenseAmt + ", C_Currency_ID=" + C_Currency_From_ID);
        //	Converted Amount = Unit price
        var ConvertedAmt = ExpenseAmt.toString();
        //	convert if required
        if (!ConvertedAmt.equals(VIS.Env.ZERO) && C_Currency_To_ID != Util.getValueOfInt(C_Currency_From_ID)) {
            var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID");
            var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
            var paramString = ConvertedAmt.toString() + "," + C_Currency_From_ID.toString() + "," + C_Currency_To_ID.toString() + "," +
                              AD_Client_ID.toString() + "," + AD_Org_ID.toString();

            //ConvertedAmt = VAdvantage.Model.MConversionRate.Convert(ctx,
            //    ConvertedAmt, Util.getValueOfInt(C_Currency_From_ID), C_Currency_To_ID,
            //    DateExpense, 0, AD_Client_ID, AD_Org_ID);
            ConvertedAmt = VIS.dataContext.getJSONRecord("MConversionRate/Convert", paramString);
        }
        mTab.setValue("ConvertedAmt", ConvertedAmt);
        this.log.fine("= ConvertedAmt=" + ConvertedAmt);

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };	//	Expense_Amount
    //	CalloutTimeExpense
    VIS.Model.CalloutTimeExpense = CalloutTimeExpense;
    //***************CalloutTimeExpense End*****************

    //*************CalloutTeamForcast Start***************
    function CalloutTeamForcast() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTeamForcast");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTeamForcast, VIS.CalloutEngine); //inherit prototype
    CalloutTeamForcast.prototype.ProductInfo = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }

        var M_Product_ID = value;
        if (M_Product_ID == null || M_Product_ID == 0)
            return "";
        var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
        if (M_PriceList_ID != 0) {
            var query = "Select M_PriceList_Version_ID from M_ProductPrice where M_Product_id=" + Util.getValueOfInt(value) +
                            " and M_PriceList_Version_ID in (select m_pricelist_version_id from m_pricelist_version" +
                          " where m_pricelist_id = " + M_PriceList_ID + " and isactive='Y')";
            var M_PriceList_Version_ID = Util.getValueOfInt(VIS.DB.executeScalar(query, null, null));
            if (M_PriceList_Version_ID != 0) {
                query = "Select PriceStd from M_ProductPrice where M_PriceList_Version_ID=" + M_PriceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
                var PriceStd = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));
                //ForcastLine.SetPriceStd(PriceStd);
                mTab.setValue("PriceStd", PriceStd);
                mTab.setValue("UnitPrice", PriceStd);
                mTab.setValue("PriceStd", (PriceStd * Util.getValueOfDecimal(mTab.getValue("QtyEntered"))));
            }
        }
        else {
            // ShowMessage.info("PriceLisetNotFound", true, null, null);
            VIS.ADialog.info("PriceLisetNotFound");
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutTeamForcast.prototype.CalculatePrice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) <= 0) {
            return "";
        }

        var price = Util.getValueOfDecimal(mTab.getValue("UnitPrice")) * Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
        // ForcastLine.SetQtyEntered(price);
        mTab.setValue("PriceStd", price);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutTeamForcast = CalloutTeamForcast;
    //************CalloutTeamForcast End****************


    //************CalloutTaxAmt Start***************
    function CalloutTaxAmt() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTaxAmt");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTaxAmt, VIS.CalloutEngine); //inherit prototype
    CalloutTaxAmt.prototype.TaxID = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var sql = "select Rate from C_Tax where C_Tax_ID = " + Util.getValueOfInt(value);
            var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql, null, null));
            if (rate != 0) {
                var taxAmt = (Util.getValueOfDecimal(mTab.getValue("ApprovedExpenseAmt")) * rate) / 100;
                taxAmt = taxAmt.toFixed(2);
                mTab.setValue("TaxAmt", taxAmt);
            }
            else {
                mTab.setValue("TaxAmt", VIS.Env.ZERO);
            }
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutTaxAmt.prototype.ExpenseAmt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            var sql = "select Rate from C_Tax where C_Tax_ID = " + Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
            var rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sql, null, null));
            if (rate != 0) {
                var taxAmt = (Util.getValueOfDecimal(value) * rate) / 100;
                taxAmt = taxAmt.toFixed(2);
                mTab.setValue("TaxAmt", taxAmt);
            }
            else {
                mTab.setValue("TaxAmt", VIS.Env.ZERO);
            }
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutTaxAmt = CalloutTaxAmt;
    //*************CalloutTaxAmt End**************

    //**************CalloutTax Start**********
    function CalloutTax() {
        VIS.CalloutEngine.call(this, "VIS.CalloutTax");//must call
    };
    VIS.Utility.inheritPrototype(CalloutTax, VIS.CalloutEngine); //inherit prototype
    CalloutTax.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Tax_ID = 0;
        var Rate = VIS.Env.ZERO;
        var LineAmount = "";
        var TotalRate = VIS.Env.ZERO;
        C_Tax_ID = Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
        var sqltax = "select rate from c_tax WHERE c_tax_id=" + C_Tax_ID + "";
        Rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sqltax, null, null));
        var LineNetAmt = Util.getValueOfDecimal(mTab.getValue("LineNetAmt"));
        TotalRate = Util.getValueOfDecimal((Util.getValueOfDecimal(LineNetAmt) * Util.getValueOfDecimal(Rate)) / 100);

        TotalRate = TotalRate.toFixed(2);

        //Decimal? qty = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
        //TotalRate = Decimal.Multiply(TotalRate, qty.Value);

        mTab.setValue("GrandTotal", (TotalRate + LineNetAmt));
        mTab.setValue("taxamt", TotalRate);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutTax = CalloutTax;
    //***********CalloutTax End *************

    //**************CalloutSetBP Start******************
    function CalloutSetBP() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetBP");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetBP, VIS.CalloutEngine); //inherit prototype
    CalloutSetBP.prototype.SetBP = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "" || Util.getValueOfInt(value) == 0) {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        try {
            this.setCalloutActive(true);
            mTab.setValue("C_BPartner_ID", Util.getValueOfInt(value));
            this.setCalloutActive(false);
        }
        catch (err) {
            this.setCalloutActive(false);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutSetBP = CalloutSetBP;
    //**************CalloutSetBP End******************

    //************CalloutProjectLine Start***************
    function CalloutProjectLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProjectLine");//must call
    };
    VIS.Utility.inheritPrototype(CalloutProjectLine, VIS.CalloutEngine); //inherit prototype
    CalloutProjectLine.prototype.Charge = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }

        var paramString = mTab.getValue("C_Charge_ID").toString();
        //X_C_Charge charge = new X_C_Charge(ctx, Cid, null);
        //var amt=charge.GetChargeAmt();
        var amt = VIS.dataContext.getJSONRecord("MCharge/GetCharge", paramString);
        mTab.setValue("PlannedQty", 1);
        mTab.setValue("PlannedPrice", amt);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutProjectLine = CalloutProjectLine;
    //**************CalloutProjectLine End*************

    //*************CalloutProductToOpportunity Start***********
    function CalloutProductToOpportunity() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProductToOpportunity");//must call
    };
    VIS.Utility.inheritPrototype(CalloutProductToOpportunity, VIS.CalloutEngine); //inherit prototype
    CalloutProductToOpportunity.prototype.ProductInfo = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return "";
        }


        // ViennaAdvantage.Model.X_C_ProjectLine oppLine = new ViennaAdvantage.Model.X_C_ProjectLine(ctx, Util.getValueOfInt(mTab.getRecord_ID()), null);
        //VAdvantage.Model.X_M_Product product = new VAdvantage.Model.X_M_Product(ctx, Util.getValueOfInt(value), null);
        var id = Util.getValueOfInt(mTab.getValue("C_ProjectTask_ID"));
        
            var Sql = "select c_project_id from C_ProjectPhase where C_ProjectPhase_id in(select C_ProjectPhase_id from" +
                        " C_ProjectTask where C_ProjectTask_id=" + id + ")";
       
        var projID = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));

        if (id == 0) {
            
            Sql = "select c_project_id from C_ProjectPhase where C_ProjectPhase_id in(" + mTab.getValue("C_ProjectPhase_ID") + " )";
            projID = Util.getValueOfInt(VIS.DB.executeScalar(Sql, null, null));

            if (projID == 0) {
                projID = Util.getValueOfInt(mTab.getValue("C_Project_ID"));
            }

        }


        var paramString = projID.toString();
        var dr = VIS.dataContext.getJSONRecord("MProject/GetProjectDetail", paramString);
        var priceList_Version_ID = Util.getValueOfInt(dr["M_PriceList_Version_ID"]);
        // X_C_Project proj = new X_C_Project(ctx, projID, null);
        //var query = "Select M_ProductPrice_id from M_ProductPrice where M_Product=" + Util.getValueOfInt(value);
        //var priceID = Util.getValueOfInt(VIS.DB.executeScalar(query,null,null)); 
        var query = "Select PriceList from M_ProductPrice where M_PriceList_Version_ID=" + priceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
        //ViennaAdvantage.Model.X_M_ProductPrice ProPrice = new ViennaAdvantage.Model.X_M_ProductPrice(ctx, proj.GetM_PriceList_Version_ID(), null);
        var PriceList = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));
        mTab.setValue("PriceList", PriceList);
        // oppLine.SetPriceList(Util.getValueOfDecimal(PriceList));

        query = "Select PriceStd from M_ProductPrice where M_PriceList_Version_ID=" + priceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
        var PriceStd = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));
        mTab.setValue("PlannedPrice", PriceStd);
        // oppLine.SetPlannedPrice(PriceStd);

        mTab.setValue("PlannedQty", 1);
        // oppLine.SetPlannedQty(1);

        query = "Select PriceLimit from M_ProductPrice where M_PriceList_Version_ID=" + priceList_Version_ID + " and M_Product_id=" + Util.getValueOfInt(value);
        var PriceLimit = Util.getValueOfDecimal(VIS.DB.executeScalar(query, null, null));
        var discount;
        try {

            discount = ((PriceList - PriceStd) * 100) / PriceList;
            if (isNaN(discount)) {
                this.setCalloutActive(false);
                return "PriceListNotSelected";
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return "PriceListNotSelected";
        }

        mTab.setValue("Discount", discount.toFixed(2));
        // oppLine.SetDiscount(Decimal.Subtract(PriceList ,PriceStd));

        mTab.setValue("PlannedMarginAmt", (PriceStd - PriceLimit));
        // oppLine.SetPlannedMarginAmt( Decimal.Subtract(PriceStd, PriceLimit));
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutProductToOpportunity = CalloutProductToOpportunity;
    //**********CalloutProductToOpportunity End**************

    //*************CalloutPriceListOpp Start**************
    function CalloutPriceListOpp() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPriceListOpp");//must call
    };
    VIS.Utility.inheritPrototype(CalloutPriceListOpp, VIS.CalloutEngine); //inherit prototype
    CalloutPriceListOpp.prototype.PriceList = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var sql = "";
        try {
            sql = "select M_PriceList_ID from M_PriceList_Version where M_PriceList_Version_ID = " + Util.getValueOfInt(value);
            var M_PriceList_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
            if (M_PriceList_ID != 0) {
                mTab.setValue("M_PriceList_ID", M_PriceList_ID);

                sql = "select C_Currency_id from m_pricelist where m_pricelist_ID = " + M_PriceList_ID;
                var C_Currency_ID = Util.getValueOfInt(VIS.DB.executeScalar(sql, null, null));
                if (C_Currency_ID != 0) {
                    mTab.setValue("C_Currency_ID", C_Currency_ID);
                }
                else {
                    //  ShowMessage.Info("CurrencyNotDefinedForThePriceList", true, null, null);
                    VIS.ADialog.info("CurrencyNotDefinedForThePriceList");
                }
            }

        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.saveError(sql, sql);
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutPriceListOpp = CalloutPriceListOpp;
    //**************CalloutPriceListOpp End*************
    //************CalloutInOut Start**********************
    function CalloutInOut() {
        VIS.CalloutEngine.call(this, "VIS.CalloutInOut");//must call
    };
    VIS.Utility.inheritPrototype(CalloutInOut, VIS.CalloutEngine); //inherit prototype

    /// <summary>
    /// C_Order - Order Defaults.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Order = function (ctx, windowNo, mTab, mField, value, oldValue) {
        // 
        
        if (value == null || value.toString() == "") {
            return "";
        }
        try {
            var C_Order_ID = Util.getValueOfInt(value.toString());
            if (C_Order_ID == null || C_Order_ID == 0) {
                return "";
            }
            //	No Callout Active to fire dependent values
            if (this.isCalloutActive())	//	prevent recursive
            {
                return "";
            }
            var paramString = C_Order_ID.toString();
            var dr = VIS.dataContext.getJSONRecord("MOrder/GetOrder", paramString);


            if (Util.getValueOfInt(dr["ID"]) != 0) {
                mTab.setValue("DateOrdered", dr["DateOrdered"]);
                mTab.setValue("POReference", dr["POReference"]);
                mTab.setValue("AD_Org_ID", Util.getValueOfInt(dr["AD_Org_ID"]));
                //
                mTab.setValue("DeliveryRule", dr["DeliveryRule"]);
                mTab.setValue("DeliveryViaRule", dr["DeliveryViaRule"]);
                mTab.setValue("M_Shipper_ID", Util.getValueOfInt(dr["M_Shipper_ID"]));
                mTab.setValue("FreightCostRule", dr["FreightCostRule"]);
                mTab.setValue("FreightAmt", dr["FreightAmt"]);

                mTab.setValue("C_BPartner_ID", Util.getValueOfInt(dr["C_BPartner_ID"]));
                //sraval: source forge bug # 1503219 - added to default ship to location
                mTab.setValue("C_BPartner_Location_ID", Util.getValueOfInt(dr["C_BPartner_Location_ID"]));

                mTab.setValue("AD_OrgTrx_ID", Util.getValueOfInt(dr["AD_OrgTrx_ID"]));
                mTab.setValue("C_Activity_ID", Util.getValueOfInt(dr["C_Activity_ID"]));
                mTab.setValue("C_Campaign_ID", Util.getValueOfInt(dr["C_Campaign_ID"]));
                mTab.setValue("C_Project_ID", Util.getValueOfInt(dr["C_Project_ID"]));
                mTab.setValue("User1_ID", Util.getValueOfInt(dr["User1_ID"]));
                mTab.setValue("User2_ID", Util.getValueOfInt(dr["User2_ID"]));
                mTab.setValue("M_Warehouse_ID", Util.getValueOfInt(dr["M_Warehouse_ID"]));

                var isReturnTrx = mTab.getValue("IsReturnTrx");
                if (isReturnTrx) {
                    mTab.setValue("Orig_Order_ID", dr["Orig_Order_ID"]);
                    mTab.setValue("Orig_InOut_ID", dr["Orig_InOut_ID"]);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
            //MessageBox.Show("CalloutInOut--Order Defaults");
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// InOut - DocType.
    /// - sets MovementType
    /// - gets DocNo
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_DocType_ID = Util.getValueOfInt(value.toString());// (int)value;
        if (C_DocType_ID == null || C_DocType_ID == 0) {
            return "";
        }
        var sql = "SELECT d.docBaseType, d.IsDocNoControlled, s.CurrentNext, d.IsReturnTrx "
            + "FROM C_DocType d, AD_Sequence s "
            + "WHERE C_DocType_ID=" + C_DocType_ID		//	1
            + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)";
        var dr = null;
        try {
            ctx.setContext(windowNo, "C_DocTypeTarget_ID", C_DocType_ID);
            dr = VIS.DB.executeReader(sql, null, null);
            if (dr.read()) {
                //	Set Movement Type
                var docBaseType = dr.get("docbasetype");
                var isReturnTrx = dr.get("isreturntrx") == "Y";
                if (docBaseType.equals("MMS") && !isReturnTrx)					//	Material Shipments
                {
                    mTab.setValue("MovementType", "C-");				//	Customer Shipments
                }
                else if (docBaseType.equals("MMS") && isReturnTrx)				//	Material Shipments
                {
                    mTab.setValue("MovementType", "C+");				//	Customer Returns
                }
                else if (docBaseType.equals("MMR") && !isReturnTrx)				//	Material Receipts
                {
                    mTab.setValue("MovementType", "V+");				//	Vendor Receipts
                }
                else if (docBaseType.equals("MMR") && isReturnTrx)				//	Material Receipts
                {
                    mTab.setValue("MovementType", "V-");					//	Return to Vendor
                }

                //	DocumentNo
                if (dr.get("isdocnocontrolled") == "Y") {
                    mTab.setValue("DocumentNo", "<" + dr.get("currentnext") + ">");
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            //MessageBox.Show("CalloutInOut--DocType");
            this.log.log(Level.SEVERE, sql, err);
            return err.message;
            //return e.getLocalizedMessage();
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_InOut - Defaults for BPartner.
    /// - Location
    /// - Contact
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var sql = "";
        var idr = null;
        if (value == null || value.toString() == "") {
            return "";
        }
        try {

            var C_BPartner_ID = Util.getValueOfInt(value.toString());// (int)value;
            if (C_BPartner_ID == null || C_BPartner_ID == 0) {
                return "";
            }

            var isReturnTrx = mTab.getValue("IsReturnTrx");

            //	sraval: source forge bug # 1503219
            var order = mTab.getValue("C_Order_ID");

            sql = "SELECT p.AD_Language, p.POReference,"
                + "SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + "l.C_BPartner_Location_ID, c.AD_User_ID "
                + "FROM C_BPartner p"
                + " LEFT OUTER JOIN C_BPartner_Location l ON (p.C_BPartner_ID=l.C_BPartner_ID)"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID;		//	1


            idr = VIS.DB.executeReader(sql, null, null);
            if (idr.read()) {
                //	Location
                var ii = Util.getValueOfInt(idr.get("c_bpartner_location_id"));
                // sraval: source forge bug # 1503219 - default location for material receipt
                if (order == null) {
                    //if (dr.wasNull())
                    if (ii == 0) {
                        mTab.setValue("C_BPartner_Location_ID", null);
                    }
                    else {
                        mTab.setValue("C_BPartner_Location_ID", ii);
                    }
                }
                //	Contact
                ii = Util.getValueOfInt(idr.get("ad_user_id"));
                //if (dr.wasNull())
                if (ii == 0) {
                    mTab.setValue("AD_User_ID", null);
                }
                else {
                    mTab.setValue("AD_User_ID", ii);
                }

                // Skip credit check for returns
                if (!isReturnTrx) {
                    //	CreditAvailable
                    var CreditAvailable = Util.getValueOfDouble(idr.get("creditavailable"));
                    //if (!dr.wasNull() && CreditAvailable < 0)
                    if (CreditAvailable < 0) {
                        //mTab.fireDataStatusEEvent("CreditLimitOver",DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable),false);
                        //MessageBox.Show("CreditLimitOver");
                        //ShowMessage.Info("CreditLimitOver", true, "", "");
                        VIS.ADialog.info("CreditLimitOver");
                    }
                }
            }
            idr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (idr != null) {
                idr.close();
                idr = null;
            }
            //MessageBox.Show("CalloutInOut--BPartner");
            this.log.log(Level.SEVERE, sql, e);
            //return e.getLocalizedMessage();
            return e.message;
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_Warehouse.
    /// Set Organization and Default Locator
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Warehouse = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        var M_Warehouse_ID = Util.getValueOfInt(value);// (int)value;
        if (M_Warehouse_ID == null || M_Warehouse_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);

        var sql = "SELECT w.AD_Org_ID, l.M_Locator_ID "
            + "FROM M_Warehouse w"
            + " LEFT OUTER JOIN M_Locator l ON (l.M_Warehouse_ID=w.M_Warehouse_ID AND l.IsDefault='Y') "
            + "WHERE w.M_Warehouse_ID=" + M_Warehouse_ID;		//	1
        var dr = null;
        try {
            dr = VIS.DB.executeReader(sql, null, null);
            if (dr.read()) {
                //	Org
                var ii = Util.getValueOfInt(dr.get("ad_org_id"));//.getInt(1));
                var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
                if (AD_Org_ID != ii) {
                    mTab.setValue("AD_Org_ID", ii);
                }
                //	Locator
                ii = Util.getValueOfInt(dr.get("m_locator_id"));// new int(dr.getInt(2));
                //if (dr.wasNull())
                if (ii == 0) {
                    ctx.setContext(windowNo, 0, "M_Locator_ID", null);
                }
                else {
                    this.log.config("M_Locator_ID=" + ii);
                    ctx.setContext(windowNo, "M_Locator_ID", ii);
                }
            }
            dr.close();
        }
        catch (err) {
            this.setCalloutActive(false);
            if (dr != null) {
                dr.close();
                dr = null;
            }
            //MessageBox.Show("CalloutInOut--Warehouse");
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// OrderLine Callout
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.OrderLine = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var C_OrderLine_ID = Util.getValueOfInt(value.toString());// (int)value;
        if (C_OrderLine_ID == null || C_OrderLine_ID == 0) {
            this.setCalloutActive(false);
            return "";
        }
        try {
            //	Get Details
            var paramString = C_OrderLine_ID.toString();
            var dr = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", paramString);
            // MOrderLine ol = new MOrderLine(ctx, C_OrderLine_ID, null);

            //	Get Details
            if (Util.getValueOfInt(dr["GetID"]) != 0) {
                mTab.setValue("M_Product_ID", Util.getValueOfInt(dr["M_Product_ID"]));
                mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(dr["M_AttributeSetInstance_ID"]));
                mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr["C_UOM_ID"]));
                //var movementQty = Decimal.Subtract(ol.GetQtyOrdered(), ol.GetQtyDelivered());
                var movementQty = (Util.getValueOfDecimal(dr["QtyOrdered"]) - Util.getValueOfDecimal(dr["QtyDelivered"]));
                mTab.setValue("MovementQty", movementQty);
                var qtyEntered = movementQty;
                if ((Util.getValueOfDecimal(dr["QtyEntered"])).toString().compareTo(Util.getValueOfDecimal(dr["QtyOrdered"])) != Util.getValueOfDecimal(dr["QtyOrdered"])) {
                    //qtyEntered = qtyEntered.multiply(ol.getQtyEntered()).divide(ol.getQtyOrdered(), 12, Decimal.ROUND_HALF_UP);
                    qtyEntered = ((qtyEntered * (Util.getValueOfDecimal(dr["QtyEntered"]))) / (Util.getValueOfDecimal(dr["QtyOrdered"]).toFixed(12)));//, MidpointRounding.AwayFromZero));
                }
                mTab.setValue("QtyEntered", qtyEntered);
                //
                mTab.setValue("C_Activity_ID", Util.getValueOfInt(dr["C_Activity_ID"]));
                mTab.setValue("C_Campaign_ID", Util.getValueOfInt(dr["C_Campaign_ID"]));
                mTab.setValue("C_Project_ID", Util.getValueOfInt(dr["C_Project_ID"]));
                mTab.setValue("C_ProjectPhase_ID", Util.getValueOfInt(dr["C_ProjectPhase_ID"]));
                mTab.setValue("C_ProjectTask_ID", Util.getValueOfInt(dr["C_ProjectTask_ID"]));
                mTab.setValue("AD_OrgTrx_ID", Util.getValueOfInt(dr["AD_OrgTrx_ID"]));
                mTab.setValue("User1_ID", Util.getValueOfInt(dr["User1_ID"]));
                mTab.setValue("User2_ID", Util.getValueOfInt(dr["User2_ID"]));
                //if (dr["IsReturnTrx"]=="true")
                if (Util.getValueOfBoolean(dr["IsReturnTrx"])) {
                    mTab.setValue("Orig_OrderLine_ID", Util.getValueOfInt(dr["Orig_OrderLine_ID"]));
                    var paramString = dr["Orig_InOutLine_ID"];
                    var line = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", paramString);

                    mTab.setValue("M_Locator_ID", line["M_Locator_ID"]);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_InOutLine - Default UOM/Locator for Product.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var M_Product_ID = Util.getValueOfInt(value);// (int)value;
        if (M_Product_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            //	Set Attribute & Locator
            var M_Locator_ID = 0;
            if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
                && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0) {
                mTab.setValue("M_AttributeSetInstance_ID",
                    Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID").toString()));
                //	Locator from Info Window - ASI
                //M_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");
                //if (M_Locator_ID != 0) {
                //    mTab.setValue("M_Locator_ID", Util.getValueOfInt(M_Locator_ID.toString()));
                //}
            }
            else {
                mTab.setValue("M_AttributeSetInstance_ID", null);
            }


            //try  //Temporary Commented BY Sarab
            //{
            //    object[] pqtyAll = VAdvantage.Classes.InfoLines.PQ.ToArray();
            //    for (var x = 0; x < pqtyAll.Length; x++)
            //    {
            //        object f = pqtyAll.GetValue(x);

            //        var AD_Session_ID = Util.getValueOfInt(((VAdvantage.Classes.InfoLines)f)._AD_Session_ID);
            //        var winNo = Util.getValueOfInt(((VAdvantage.Classes.InfoLines)f)._windowNo);
            //        Dictionary<int, Decimal> ProductQty = ((VAdvantage.Classes.InfoLines)f)._prodQty;

            //        List<int> key = ProductQty.Keys.ToList();
            //        if (AD_Session_ID == Env.GetCtx().GetAD_Session_ID() && winNo == windowNo && Util.getValueOfInt(value) == Util.getValueOfInt(key[0]))
            //        {
            //            var qty = Util.getValueOfDecimal(ProductQty[Util.getValueOfInt(value)]);
            //            mTab.setValue("QtyEntered", qty);
            //            VAdvantage.Classes.InfoLines.PQ.RemoveAt(x);
            //        }

            //    }
            //}
            //catch(err)
            //{
            //    for (var k = 0; k < VAdvantage.Classes.InfoLines.PQ.Count; k++)
            //    {
            //        VAdvantage.Classes.InfoLines.PQ.RemoveAt(k);
            //    }
            //}



            //
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            // var isSOTrx = ctx.getContext("IsSOTrx");
            if (isSOTrx) {
                this.setCalloutActive(false);
                return "";
            }






            //	PO - Set UOM/Locator/Qty

            //MProduct product = MProduct.Get(ctx, M_Product_ID);
            //mTab.setValue("C_UOM_ID", Util.getValueOfInt(product.GetC_UOM_ID().toString()));
            var paramString = M_Product_ID.toString();
            var C_UOM_ID = VIS.dataContext.getJSONRecord("MProduct/GetC_UOM_ID", paramString);
            mTab.setValue("C_UOM_ID", C_UOM_ID);
            var qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            mTab.setValue("MovementQty", qtyEntered);
            var qryBP = "SELECT C_BPartner_ID FROM M_InOut WHERE M_InOut_ID = " + Util.getValueOfInt(mTab.getValue("M_InOut_ID"));
            var bpartner = Util.getValueOfInt(VIS.DB.executeScalar(qryBP));
            var qryUom = "SELECT vdr.C_UOM_ID FROM M_Product p LEFT JOIN M_Product_Po vdr ON p.M_Product_ID= vdr.M_Product_ID WHERE p.M_Product_ID=" + M_Product_ID + " AND vdr.C_BPartner_ID = " + bpartner;
            var uom = Util.getValueOfInt(VIS.DB.executeScalar(qryUom));
            if (C_UOM_ID != 0) {
                if (C_UOM_ID != uom && uom != 0) {
                    var Res = Util.getValueOfDecimal(VIS.DB.executeScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + C_UOM_ID + " AND C_UOM_To_ID = " + uom + " AND M_Product_ID= " + M_Product_ID + " AND IsActive='Y'"));
                    if (Res > 0) {
                        mTab.setValue("QtyEntered", Util.getValueOfInt(mTab.getValue("QtyEntered")) * Res);
                        //OrdQty = MUOMConversion.ConvertProductTo(GetCtx(), _M_Product_ID, UOM, OrdQty);
                    }
                    else {
                        var res = Util.getValueOfDecimal(VIS.DB.executeScalar("SELECT trunc(multiplyrate,4) FROM C_UOM_Conversion WHERE C_UOM_ID = " + C_UOM_ID + " AND C_UOM_To_ID = " + uom + " AND IsActive='Y'"));
                        if (res > 0) {
                            mTab.setValue("QtyEntered", Util.getValueOfInt(mTab.getValue("QtyEntered")) * Res);
                            //OrdQty = MUOMConversion.Convert(GetCtx(), prdUOM, UOM, OrdQty);
                        }
                    }
                    mTab.setValue("C_UOM_ID", uom);
                }
                else {
                    mTab.setValue("C_UOM_ID", C_UOM_ID);
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// InOut Line - Quantity.
    /// - called from C_UOM_ID, qtyEntered, movementQty
    /// - enforces qty UOM relationship
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Qty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            this.log.log(Level.WARNING, "qty - init - M_Product_ID=" + M_Product_ID);
            var movementQty, qtyEntered;

            //	No Product
            if (M_Product_ID == 0) {
                qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                mTab.setValue("MovementQty", qtyEntered);
            }
                //	UOM Changed - convert from Entered -> Product
            else if (mField.getColumnName().toString().equals("C_UOM_ID")) {
                var C_UOM_To_ID = value;
                qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                var paramString = C_UOM_To_ID.toString();
                var precision = VIS.dataContext.getJSONRecord("MUOM/GetPrecision", paramString);
                var QtyEntered1 = qtyEntered.toFixed(precision);//, MidpointRounding.AwayFromZero);
                if (qtyEntered.toString().compareTo(QtyEntered1) != 0) {
                    this.log.fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                      + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", qtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", qtyEntered.toString());
                movementQty = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);
                //movementQty = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, qtyEntered.Value);
                if (movementQty == null) {
                    movementQty = qtyEntered;
                }
                var conversion = qtyEntered.compareTo(movementQty) != 0;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", qtyEntered=" + qtyEntered
                    + " -> " + conversion
                    + " movementQty=" + movementQty);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("MovementQty", movementQty);
            }
                //	No UOM defined
            else if (ctx.getContextAsInt(windowNo, "C_UOM_ID") == 0) {
                qtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                mTab.setValue("MovementQty", qtyEntered);
            }
                //	qtyEntered changed - calculate movementQty
            else if (mField.getColumnName().toString().equals("QtyEntered")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                qtyEntered = Util.getValueOfDecimal(value);
                var QtyEntered1 = qtyEntered.toFixed(precision);// MidpointRounding.AwayFromZero);
                if (qtyEntered.compareTo(QtyEntered1) != 0) {
                    this.log.fine("Corrected qtyEntered Scale UOM=" + C_UOM_To_ID
                        + "; qtyEntered=" + qtyEntered + "->" + QtyEntered1);
                    qtyEntered = QtyEntered1;
                    mTab.setValue("QtyEntered", qtyEntered);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", qtyEntered.toString());
                movementQty = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString);
                // movementQty = MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //     C_UOM_To_ID, qtyEntered.Value);
                if (movementQty == null) {
                    movementQty = qtyEntered;
                }
                var conversion = qtyEntered.compareTo(movementQty) != 0;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", qtyEntered=" + qtyEntered
                    + " -> " + conversion
                    + " movementQty=" + movementQty);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("MovementQty", movementQty);
            }
                //	movementQty changed - calculate qtyEntered (should not happen)
            else if (mField.getColumnName().toString().equals("MovementQty")) {
                var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
                movementQty = Util.getValueOfDecimal(value);

                var paramString = M_Product_ID.toString();
                precision = VIS.dataContext.getJSONRecord("MProduct/GetUOMPrecision", paramString);
                //var precision = MProduct.Get(ctx, M_Product_ID).GetUOMPrecision();
                var MovementQty1 = movementQty.toFixed(precision);//, MidpointRounding.AwayFromZero);
                if (movementQty.compareTo(MovementQty1) != 0) {
                    this.log.fine("Corrected movementQty "
                        + movementQty + "->" + MovementQty1);
                    movementQty = MovementQty1;
                    mTab.setValue("MovementQty", movementQty);
                }

                paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", movementQty.toString());
                qtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);

                //qtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //    C_UOM_To_ID, movementQty);
                if (qtyEntered == null) {
                    qtyEntered = movementQty;
                }
                var conversion = movementQty.compareTo(qtyEntered) != 0;
                this.log.fine("UOM=" + C_UOM_To_ID
                    + ", movementQty=" + movementQty
                    + " -> " + conversion
                    + " qtyEntered=" + qtyEntered);
                ctx.setContext(windowNo, "UOMConversion", conversion ? "Y" : "N");
                mTab.setValue("QtyEntered", qtyEntered);
            }

            // Check for RMA
            var isReturnTrx = "Y".equals(ctx.getContext("IsReturnTrx"));
            if (M_Product_ID != 0 && isReturnTrx) {
                var oLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                paramString = oLine_ID.toString();
                var oLine = VIS.dataContext.getJSONRecord("MOrderLine/GetOrderLine", paramString);
                //  MOrderLine oLine = new MOrderLine(ctx, oLine_ID, null);
                if (oLine.Get_ID() != 0) {
                    var orig_IOLine_ID = oLine["tOrig_InOutLine_ID"];
                    if (orig_IOLine_ID != 0) {
                        var paramString = orig_IOLine_ID.toString();
                        var orig_IOLine = VIS.dataContext.getJSONRecord("MInOutLine/GetMInOutLine", paramString);

                        // MInOutLine orig_IOLine = new MInOutLine(ctx, orig_IOLine_ID, null);
                        var shippedQty = orig_IOLine["MovementQty"];
                        movementQty = Util.getValueOfDecimal(mTab.getValue("MovementQty"));
                        if (shippedQty.toString().compareTo(movementQty) < 0) {
                            if (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y") {
                                // ShowMessage.Info("QtyShippedAndReturned", true, "", "");
                                VIS.ADialog.info("QtyShippedAndReturned");
                            }
                            else {
                                // ShowMessage.Info("QtyRecievedAndReturnd", true, "", "");
                                VIS.ADialog.info("QtyRecievedAndReturnd");
                            }
                            mTab.setValue("MovementQty", shippedQty);
                            movementQty = shippedQty;

                            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");

                            paramString = M_Product_ID.toString().concat(",", C_UOM_To_ID.toString(),
                                                               ",", movementQty.toString());
                            qtyEntered = VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString);
                            //qtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                            //        C_UOM_To_ID, movementQty);
                            if (qtyEntered == null) {
                                qtyEntered = movementQty;
                            }
                            mTab.setValue("QtyEntered", qtyEntered);
                            mTab.setValue("MovementQty", movementQty);
                            this.log.fine("qtyEntered : " + qtyEntered.toString() +
                                      "movementQty : " + movementQty.toString());
                        }
                    }
                }
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.severe(err.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// M_InOutLine - ASI.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="windowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns>error message or ""</returns>
    CalloutInOut.prototype.Asi = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        var M_ASI_ID = Util.getValueOfInt(value);// (int)value;
        if (M_ASI_ID == null || M_ASI_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
            var M_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");
            this.log.fine("M_Product_ID=" + M_Product_ID
                + ", M_ASI_ID=" + M_ASI_ID
                + " - M_Warehouse_ID=" + M_Warehouse_ID
                + ", M_Locator_ID=" + M_Locator_ID);
            //	Check Selection
            var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
            if (M_ASI_ID == M_AttributeSetInstance_ID) {
                var selectedM_Locator_ID = ctx.getContextAsInt(windowNo, "M_Locator_ID");
                if (selectedM_Locator_ID != 0) {
                    this.log.fine("Selected M_Locator_ID=" + selectedM_Locator_ID);
                    mTab.setValue("M_Locator_ID", selectedM_Locator_ID);
                }
            }
        }
        catch (errx) {
            this.setCalloutActive(false);
            this.log.severe(ex.toString());
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutInOut = CalloutInOut;
    //**************CalloutInOut End********************

    //************CalloutPaymentAllocate Start ********
    function CalloutPaymentAllocate() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPaymentAllocate"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutPaymentAllocate, VIS.CalloutEngine);//inherit CalloutEngine
    /// <summary>
    /// Payment_Invoice.
    /// when Invoice selected
    /// - set InvoiceAmt = invoiceOpen
    /// - DiscountAmt = C_Invoice_Discount (ID, DateTrx)
    /// - Amount = invoiceOpen (ID) - Discount
    /// - WriteOffAmt,OverUnderAmt = 0
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    CalloutPaymentAllocate.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        // 
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Invoice_ID = Util.getValueOfInt(value);
        if (this.isCalloutActive()		//	assuming it is resetting value
            || C_Invoice_ID == null || C_Invoice_ID == 0) {
            return "";
        }

        //	Check Payment
        var C_Payment_ID = ctx.getContextAsInt(windowNo, "C_Payment_ID");

        var paramString = C_Payment_ID.toString();

        //Get product price information
        var payment = null;
        payment = VIS.dataContext.getJSONRecord("MPayment/GetPayment", paramString);



        // MPayment payment = new MPayment(ctx, C_Payment_ID, null);
        if (payment["C_Charge_ID"] != 0 || payment["C_Invoice_ID"] != 0
            || payment["C_Order_ID"] != 0)
            return Msg.getMsg(ctx, "PaymentIsAllocated");

        this.setCalloutActive(true);
        //
        mTab.setValue("DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
        mTab.setValue("OverUnderAmt", VIS.Env.ZERO);

        var C_InvoicePaySchedule_ID = 0;
        if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
            && ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID") != 0) {
            C_InvoicePaySchedule_ID = ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID");
        }

        //  Payment Date
        var ts = Util.getValueOfDate(ctx.getContextAsTime(windowNo, "DateTrx"));
        //String sql = "SELECT C_BPartner_ID,C_Currency_ID,"		//	1..2
        //    + " invoiceOpen(C_Invoice_ID, @param1),"					//	3		#1
        //    + " invoiceDiscount(C_Invoice_ID,@param2,@param3), IsSOTrx "	//	4..5	#2/3
        //    + "FROM C_Invoice WHERE C_Invoice_ID=@param4";			//			#4
        var sql = "SELECT C_BPartner_ID,C_Currency_ID,"		//	1..2
            + " invoiceOpen(C_Invoice_ID, " + C_InvoicePaySchedule_ID + ") as invoiceOpen ,"					//	3		#1
            + " invoiceDiscount(C_Invoice_ID," + VIS.DB.to_date(ts, true) + "," + C_InvoicePaySchedule_ID + ") as invoiceDiscount , IsSOTrx "	//	4..5	#2/3
            + "FROM C_Invoice WHERE C_Invoice_ID=" + C_Invoice_ID;

        var idr = null;
        try {
            //SqlParameter[] param = new SqlParameter[4];
            //param[0] = new SqlParameter("@param1", C_InvoicePaySchedule_ID);
            //param[1] = new SqlParameter("@param2", DataBase.DB.TO_DATE(ts, true));
            //param[2] = new SqlParameter("@param3", C_InvoicePaySchedule_ID);
            //param[3] = new SqlParameter("@param4", C_Invoice_ID);
            //idr = DB.executeReader(sql, param, null);
            idr = VIS.DB.executeReader(sql, null, null);
            if (idr.read()) {
                var invoiceOpen = idr.get("invoiceopen");//.getBigDecimal(3);		//	Set Invoice OPen Amount
                if (invoiceOpen == null) {
                    invoiceOpen = VIS.Env.ZERO;
                }
                var discountAmt = idr.get("invoicediscount");//.getBigDecimal(4);		//	Set Discount Amt
                if (discountAmt == null) {
                    discountAmt = VIS.Env.ZERO;
                }
                mTab.setValue("InvoiceAmt", invoiceOpen);
                mTab.setValue("Amount", invoiceOpen - discountAmt);
                mTab.setValue("DiscountAmt", discountAmt);
                //  reset as dependent fields get reset
                ctx.setContext(windowNo, "C_Invoice_ID", C_Invoice_ID.toString());
                mTab.setValue("C_Invoice_ID", C_Invoice_ID);
            }
            idr.close();
        }
        catch (err) {
            if (idr != null) {
                idr.close();
                idr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Payment_Amounts.
    /// Change of:
    /// - IsOverUnderPayment -> set OverUnderAmt to 0
    /// - C_Currency_ID, C_ConvesionRate_ID -> convert all
    /// - PayAmt, DiscountAmt, WriteOffAmt, OverUnderAmt -> PayAmt
    /// make sure that add up to InvoiceOpenAmt
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="WindowNo"></param>
    /// <param name="mTab"></param>
    /// <param name="mField"></param>
    /// <param name="value"></param>
    /// <param name="oldValue"></param>
    /// <returns></returns>
    CalloutPaymentAllocate.prototype.Amounts = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        //if (value == DBNull.Value || value == null || value.toString() == "")
        //{
        //    return "";
        //}
        if (this.isCalloutActive())		//	assuming it is resetting value
        {
            return "";
        }
        //	No Invoice
        var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
        if (C_Invoice_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        //	Get Info from Tab
        var amount = Util.getValueOfDecimal(mTab.getValue("Amount"));
        var discountAmt = Util.getValueOfDecimal(mTab.getValue("DiscountAmt"));
        var writeOffAmt = Util.getValueOfDecimal(mTab.getValue("WriteOffAmt"));
        var overUnderAmt = Util.getValueOfDecimal(mTab.getValue("OverUnderAmt"));
        var invoiceAmt = Util.getValueOfDecimal(mTab.getValue("InvoiceAmt"));
        log.fine("Amt=" + amount + ", Discount=" + discountAmt
            + ", WriteOff=" + writeOffAmt + ", OverUnder=" + overUnderAmt
            + ", Invoice=" + invoiceAmt);

        //	Changed Column
        var colName = mField.getColumnName();
        //  PayAmt - calculate write off
        if (colName == "Amount") {
            //writeOffAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(invoiceAmt, amount), discountAmt), overUnderAmt);
            writeOffAmt = (((invoiceAmt - amount) - discountAmt) - overUnderAmt);

            mTab.setValue("WriteOffAmt", writeOffAmt);
        }
        else    //  calculate Amount
        {
            //amount = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(invoiceAmt, discountAmt), writeOffAmt), overUnderAmt);
            amount = (((invoiceAmt - discountAmt) - writeOffAmt) - overUnderAmt);

            mTab.setValue("Amount", amount);
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutPaymentAllocate = CalloutPaymentAllocate;
    //************ CalloutPaymentAllocate End *********

    //*********** CalloutPayment Start ****
    function CalloutPayment() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPayment"); //must call
    };
    VIS.Utility.inheritPrototype(CalloutPayment, VIS.CalloutEngine);//inherit CalloutEngine

    /// <summary>
    /// Payment_Invoice.
    /// when Invoice selected
    /// - set C_Currency_ID 
    /// - C_BPartner_ID
    /// - DiscountAmt = C_Invoice_Discount (ID, DateTrx)
    /// - PayAmt = invoiceOpen (ID) - Discount
    /// - WriteOffAmt = 0
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns> null or error message</returns>
    CalloutPayment.prototype.Invoice = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Invoice_ID = Util.getValueOfInt(value.toString());//(int)value;
        if (this.isCalloutActive()		//	assuming it is resetting value
         || C_Invoice_ID == null
         || C_Invoice_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        mTab.setValue("C_Order_ID", null);
        mTab.setValue("C_Charge_ID", null);
        mTab.setValue("IsPrepayment", false);//Boolean.FALSE);
        //
        mTab.setValue("DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
        mTab.setValue("IsOverUnderPayment", false);//Boolean.FALSE);
        mTab.setValue("OverUnderAmt", VIS.Env.ZERO);

        var C_InvoicePaySchedule_ID = 0;

        //------------14-11-2014---------
        //Pratap changes 7-8-14/////
        var dueAmount = 0;
        var _chk = 0;
        var _sqlAmt = "SELECT * FROM   (SELECT ips.C_InvoicePaySchedule_ID, "
        + " ips.DueAmt  FROM C_Invoice i  INNER JOIN C_InvoicePaySchedule ips "
        + " ON (i.C_Invoice_ID        =ips.C_Invoice_ID)  WHERE i.IsPayScheduleValid='Y' "
        + " AND ips.IsValid           ='Y'  AND ips.isactive          ='Y' "
        + " AND i.C_Invoice_ID    = " + C_Invoice_ID
        + "  AND ips.C_InvoicePaySchedule_ID NOT IN"
        + "(SELECT NVL(C_InvoicePaySchedule_ID,0) FROM C_InvoicePaySchedule WHERE c_payment_id IN"
        + "(SELECT NVL(c_payment_id,0) FROM C_InvoicePaySchedule) union "
        + " SELECT NVL(C_InvoicePaySchedule_id,0) FROM C_InvoicePaySchedule  WHERE c_cashline_id IN"
        + "(SELECT NVL(c_cashline_id,0) FROM C_InvoicePaySchedule )) "
        + " ORDER BY ips.duedate ASC  ) WHERE rownum=1";
        var drAmt = null;
        try {
            drAmt = VIS.DB.executeReader(_sqlAmt, null, null);
            if (drAmt.read()) {
                C_InvoicePaySchedule_ID = Util.getValueOfInt(drAmt.get("c_invoicepayschedule_id"));
                dueAmount = Util.getValueOfDecimal(drAmt.get("dueamt"));
                mTab.setValue("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);
                mTab.setValue("PayAmt", dueAmount);
                _chk = 1;
            }
            drAmt.close();
        }
        catch (err) {
            if (drAmt != null) {
                drAmt.close();
            }
            this.log.log(Level.SEVERE, _sqlAmt, e);
            return err.toString();
        }
        //-------------------------------



        if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
        && ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID") != 0) {
            C_InvoicePaySchedule_ID = ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID");
        }

        //  Payment Date
        var ts = new Date(mTab.getValue("DateTrx"));
        var tsDate;
        if (ts == null) {
            //ts = DateTime.Now.Date; //new DateTime(CommonFunctions.CurrentTimeMillis());
            ts = new Date();


        }



        tsDate = "TO_DATE( '" + ts.getMonth() + "-" + ts.getDate() + "-" + ts.getFullYear() + "', 'MM-DD-YYYY')";
        //
        var sql = "SELECT C_BPartner_ID,C_Currency_ID,"		//	1..2
            + " invoiceOpen(C_Invoice_ID, @param1) as invoiceOpen ,"					//	3		#1
            + " invoiceDiscount(C_Invoice_ID,@param2,@param3) as invoiceDiscount, IsSOTrx "	//	4..5	#2/3
            + "FROM C_Invoice WHERE C_Invoice_ID=@param4";			//			#4
        var dr = null;
        //SqlParameter[] param = new SqlParameter[4];
        var param = [];
        try {
            param[0] = new VIS.DB.SqlParam("@param1", C_InvoicePaySchedule_ID);
            param[1] = new VIS.DB.SqlParam("@param2", ts);
            param[1].setIsDate(true);
            param[2] = new VIS.DB.SqlParam("@param3", C_InvoicePaySchedule_ID);
            param[3] = new VIS.DB.SqlParam("@param4", C_Invoice_ID);
            dr = VIS.DB.executeReader(sql, param, null);

            if (dr.read()) {
                mTab.setValue("C_BPartner_ID", Util.getValueOfInt(dr.get("c_bpartner_id")));//.getInt(1)));
                var C_Currency_ID = Util.getValueOfInt(dr.get("c_currency_id"));//dr.getInt(2);					//	Set Invoice Currency
                mTab.setValue("C_Currency_ID", C_Currency_ID);
                //
                var invoiceOpen = Util.getValueOfDecimal(dr.get("invoiceopen"));//.getBigDecimal(3);		//	Set Invoice OPen Amount
                if (invoiceOpen == null) {
                    invoiceOpen = VIS.Env.ZERO;
                }
                var discountAmt = Util.getValueOfDecimal(dr.get("invoicediscount"));//.getBigDecimal(4);		//	Set Discount Amt
                if (discountAmt == null) {
                    discountAmt = VIS.Env.ZERO;
                }
                //mTab.setValue("PayAmt", Decimal.Subtract(invoiceOpen, discountAmt));                
                if (_chk == 0)//Pratap
                {
                    mTab.setValue("PayAmt", (invoiceOpen - discountAmt));
                }
                mTab.setValue("C_InvoicePaySchedule_ID", C_InvoicePaySchedule_ID);//Pratap
                mTab.setValue("DiscountAmt", discountAmt);
                //  reset as dependent fields get reset
                ctx.setContext(windowNo, "C_Invoice_ID", C_Invoice_ID.toString());
                // mTab.setValue("C_Invoice_ID", C_Invoice_ID);
            }
            dr.close();
        }
        catch (err) {
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }
        this.setCalloutActive(false);
        // ctx = mTab = mField = value = oldValue = null;
        return this.DocType(ctx, windowNo, mTab, mField, value);
    };
    /// <summary>
    /// Payment_Order.
    /// when Waiting Payment Order selected
    /// - set C_Currency_ID
    /// - C_BPartner_ID
    /// - DiscountAmt = C_Invoice_Discount (ID, DateTrx)
    /// - PayAmt = invoiceOpen (ID) - Discount
    /// - WriteOffAmt = 0
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField"> Grid Field</param>
    /// <param name="value"> New Value</param>
    /// <returns>null or error message</returns>
    CalloutPayment.prototype.Order = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        var C_Order_ID = value;
        if (this.isCalloutActive()		//	assuming it is resetting value
            || C_Order_ID == null
            || C_Order_ID == 0) {
            return "";
        }
        this.setCalloutActive(true);
        mTab.setValue("C_Invoice_ID", null);
        mTab.setValue("C_Charge_ID", null);
        mTab.setValue("IsPrepayment", true);// Boolean.TRUE);
        //
        mTab.setValue("DiscountAmt", VIS.Env.ZERO);
        mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
        mTab.setValue("IsOverUnderPayment", false);// Boolean.FALSE);
        mTab.setValue("OverUnderAmt", VIS.Env.ZERO);

        //  Payment Date
        var ts = mTab.getValue("DateTrx");
        if (ts == null) {
            //ts = DateTime.Now.Date; //new DateTime(CommonFunctions.CurrentTimeMillis());
            ts = new Date();
        }
        //
        var sql = "SELECT C_BPartner_ID,C_Currency_ID, GrandTotal "
            + "FROM C_Order WHERE C_Order_ID=@param1"; 	// #1
        var dr = null;
        var param = [];
        //SqlParameter[] param = new SqlParameter[1];
        try {
            param[0] = new VIS.DB.SqlParam("@param1", C_Order_ID);
            dr = VIS.DB.executeReader(sql, param, null);

            if (dr.read()) {
                mTab.setValue("C_BPartner_ID", Util.getValueOfInt(dr.get("c_bpartner_id")));//.getInt(1)));
                var C_Currency_ID = Util.getValueOfInt(dr.get("c_currency_id"));//.getInt(2);					//	Set Order Currency
                mTab.setValue("C_Currency_ID", C_Currency_ID);
                //
                var grandTotal = Util.getValueOfDecimal(dr.get("grandtotal"));//.getBigDecimal(3);		//	Set Pay Amount
                if (grandTotal == null) {
                    grandTotal = VIS.Env.ZERO;
                }
                mTab.setValue("PayAmt", grandTotal);
            }
            dr.close();
        }
        catch (err) {
            if (dr != null) {
                dr.close();
                dr = null;
            }
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            //return e.getLocalizedMessage();
            return err.message;
        }

        this.setCalloutActive(false);
        //ctx = mTab = mField = value = oldValue = null;
        return this.DocType(ctx, windowNo, mTab, mField, value);
    };
    /// <summary>
    /// Payment_Charge.
    /// - reset - C_BPartner_ID, Invoice, Order, Project,
    /// Discount, WriteOff
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutPayment.prototype.Charge = function (ctx, windowNo, mTab, mField, value) {
        //  
        if (value == null || value.toString() == "") {
            return "";

            var C_Charge_ID = value;
            if (this.isCalloutActive()		//	assuming it is resetting value
                || C_Charge_ID == null
                || C_Charge_ID == 0) {
                return "";
            }
            this.setCalloutActive(true);
            mTab.setValue("C_Invoice_ID", null);
            mTab.setValue("C_Order_ID", null);
            //	 mTab.setValue("C_Project_ID", null);
            mTab.setValue("IsPrepayment", false);// Boolean.FALSE);
            //
            mTab.setValue("DiscountAmt", VIS.Env.ZERO);
            mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
            mTab.setValue("IsOverUnderPayment", false);// Boolean.FALSE);
            mTab.setValue("OverUnderAmt", VIS.Env.ZERO);
            this.setCalloutActive(false);
            ctx = windowNo = mTab = mField = value = oldValue = null;
            return "";
        }
    };
    /// <summary>
    /// Payment_Document Type.
    /// Verify that Document Type (AP/AR) and Invoice (SO/PO) are in sync
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns
    CalloutPayment.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {


        var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
        var C_Order_ID = ctx.getContextAsInt(windowNo, "C_Order_ID");
        var C_DocType_ID = ctx.getContextAsInt(windowNo, "C_DocType_ID");
        this.log.fine("Payment_DocType - C_Invoice_ID=" + C_Invoice_ID + ", C_DocType_ID=" + C_DocType_ID);
        var paramString = C_DocType_ID.toString();

        var dt = VIS.dataContext.getJSONRecord("MDocType/GetDocType", paramString);
        var isSOTrx = Util.getValueOfBoolean(dt["IsSOTrx"]);
        if (C_DocType_ID != 0) {
            // dt = MDocType.Get(ctx, C_DocType_ID);

            // ctx.setIsSOTrx(windowNo, dt["IsSOTrx"]);
            ctx.setContext("IsSOTrx", isSOTrx ? "Y" : "N");
        }
        //	Invoice
        if (C_Invoice_ID != 0) {

            paramString = C_Invoice_ID.toString();
            var inv = VIS.dataContext.getJSONRecord("MInvoice/GetInvoice", paramString);
            //  MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            if (dt != null) {
                if (Util.getValueOfBoolean(inv["IsSOTrx"]) != isSOTrx) {
                    return "PaymentDocTypeInvoiceInconsistent";
                }
            }
        }
        //	Order Waiting Payment (can only be SO)
        if (C_Order_ID != 0 && !isSOTrx) {
            return "PaymentDocTypeInvoiceInconsistent";
        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    /// <summary>
    /// Payment_Amounts.
    /// Change of:
    /// - IsOverUnderPayment -> set OverUnderAmt to 0
    /// - C_Currency_ID, C_ConvesionRate_ID -> convert all
    /// - PayAmt, DiscountAmt, WriteOffAmt, OverUnderAmt -> PayAmt
    /// make sure that add up to InvoiceOpenAmt
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="WindowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <param name="oldValue">Old Value</param>
    /// <returns>null or error message</returns>
    CalloutPayment.prototype.Amounts = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (this.isCalloutActive())		//	assuming it is resetting value
        {
            return "";
        }
        var C_Invoice_ID = ctx.getContextAsInt(windowNo, "C_Invoice_ID");
        //	New Payment
        if (ctx.getContextAsInt(windowNo, "C_Payment_ID") == 0
            && ctx.getContextAsInt(windowNo, "C_BPartner_ID") == 0
            && C_Invoice_ID == 0) {
            return "";
        }
        var cur = mTab.getValue("C_Currency_ID");
        if (cur == null) {
            return "";
        }
        this.setCalloutActive(true);

        //	Changed Column
        var colName = mField.getColumnName();

        var C_InvoicePaySchedule_ID = 0;
        if (ctx.getContextAsInt(windowNo, "C_Invoice_ID") == C_Invoice_ID
            && ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID") != 0) {
            C_InvoicePaySchedule_ID = ctx.getContextAsInt(windowNo, "C_InvoicePaySchedule_ID");
        }
        //	Get Open Amount & Invoice Currency
        var invoiceOpenAmt = VIS.Env.ZERO;
        var discountAmt = VIS.Env.ZERO;
        var C_Currency_Invoice_ID = 0;
        if (C_Invoice_ID != 0) {
            var ts = mTab.getValue("DateTrx");
            if (ts == null) {
                //ts = DateTime.Now.Date; //new DateTime(CommonFunctions.CurrentTimeMillis());
                ts = new Date();
            }
            var sql = "SELECT C_BPartner_ID,C_Currency_ID,"		//	1..2
                + " invoiceOpen(C_Invoice_ID, @param1) as invoiceOpen,"					//	3		#1
                + " invoiceDiscount(C_Invoice_ID,@param2,@param3) as invoiceDiscount, IsSOTrx "	//	4..5	#2/3
                + "FROM C_Invoice WHERE C_Invoice_ID=@param4";			//			#4
            var dr = null;
            //SqlParameter[] param = new SqlParameter[4];
            var param = [];
            try {
                param[0] = new VIS.DB.SqlParam("@param1", C_InvoicePaySchedule_ID);
                param[1] = new VIS.DB.SqlParam("@param2", ts);
                param[1].setIsDate(true);
                param[2] = new VIS.DB.SqlParam("@param3", C_InvoicePaySchedule_ID);
                param[3] = new VIS.DB.SqlParam("@param4", C_Invoice_ID);
                dr = VIS.DB.executeReader(sql, param, null);

                if (dr.read()) {
                    C_Currency_Invoice_ID = Util.getValueOfInt(dr.get("c_currency_id"));//.getInt(2);
                    invoiceOpenAmt = Util.getValueOfDecimal(dr.get("invoiceopen"));//.getBigDecimal(3);		//	Set Invoice Open Amount
                    if (invoiceOpenAmt == null) {
                        invoiceOpenAmt = VIS.Env.ZERO;
                    }
                    discountAmt = Util.getValueOfDecimal(dr.get("invoicediscount"));//.getBigDecimal(4);
                }
                dr.close();
            }
            catch (err) {
                if (dr != null) {
                    dr.close();
                }
                this.log.log(Level.SEVERE, sql, err);
                this.setCalloutActive(false);
                //return e.getLocalizedMessage();

                return err.message;
            }
        }	//	get Invoice Info

        this.log.fine("Open=" + invoiceOpenAmt + " Discount= " + discountAmt
          + ", C_Invoice_ID=" + C_Invoice_ID
          + ", C_Currency_ID=" + C_Currency_Invoice_ID);

        //	Get Info from Tab
        var payAmt = Util.getValueOfDecimal(mTab.getValue("PayAmt") == null ? VIS.Env.ZERO : mTab.getValue("PayAmt"));
        var writeOffAmt = Util.getValueOfDecimal(mTab.getValue("WriteOffAmt") == null ? VIS.Env.ZERO : mTab.getValue("WriteOffAmt"));
        var overUnderAmt = Util.getValueOfDecimal((mTab.getValue("OverUnderAmt") == null ? VIS.Env.ZERO : mTab.getValue("OverUnderAmt")));
        var enteredDiscountAmt = Util.getValueOfDecimal((mTab.getValue("DiscountAmt") == null ? VIS.Env.ZERO : mTab.getValue("DiscountAmt")));

        this.log.fine("Pay=" + payAmt + ", Discount=" + enteredDiscountAmt
            + ", WriteOff=" + writeOffAmt + ", OverUnderAmt=" + overUnderAmt);
        //	Get Currency Info
        var C_Currency_ID = Util.getValueOfInt(cur);
        var paramString = C_Currency_ID.toString();
        var currency = VIS.dataContext.getJSONRecord("MCurrency/GetCurrency", paramString);
        //MCurrency currency = MCurrency.Get(ctx, C_Currency_ID);
        var ConvDate = mTab.getValue("DateTrx");
        var C_ConversionType_ID = 0;
        var ii = Util.getValueOfInt(mTab.getValue("C_ConversionType_ID"));
        if (ii != null) {
            C_ConversionType_ID = ii;
        }
        var AD_Client_ID = ctx.getContextAsInt(windowNo, "AD_Client_ID");
        var AD_Org_ID = ctx.getContextAsInt(windowNo, "AD_Org_ID");
        //	Get Currency Rate
        var currencyRate = VIS.Env.ONE;
        if ((C_Currency_ID > 0 && C_Currency_Invoice_ID > 0 &&
            C_Currency_ID != C_Currency_Invoice_ID)
            || colName == "C_Currency_ID" || colName == "C_ConversionType_ID") {
            this.log.fine("InvCurrency=" + C_Currency_Invoice_ID
                + ", PayCurrency=" + C_Currency_ID
                + ", Date=" + ConvDate + ", Type=" + C_ConversionType_ID);


            var paramStr = C_Currency_Invoice_ID + "," + C_Currency_ID + "," + ConvDate + "," + C_ConversionType_ID + "," + AD_Client_ID + "," + AD_Org_ID;
            currencyRate = VIS.dataContext.getJSONRecord("MConversionRate/GetRate", paramStr);

            //currencyRate = MConversionRate.GetRate(C_Currency_Invoice_ID, C_Currency_ID,
            //    ConvDate, C_ConversionType_ID, AD_Client_ID, AD_Org_ID);
            if (currencyRate == null || currencyRate.toString() == 0) {
                //	 mTab.setValue("C_Currency_ID", new int(C_Currency_Invoice_ID));	//	does not work
                this.setCalloutActive(false);
                if (C_Currency_Invoice_ID == 0) {
                    return "";		//	no error message when no invoice is selected
                }
                // VIS.ADialog.info("NoCurrencyConversion");
                return "NoCurrencyConversion";
            }
            //
            invoiceOpenAmt = (invoiceOpenAmt * currencyRate).toFixed(currency["StdPrecision"]);//, MidpointRounding.AwayFromZero);
            discountAmt = (discountAmt * currencyRate).toFixed(currency["StdPrecision"]);
            //currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
            this.log.fine("Rate=" + currencyRate + ", InvoiceOpenAmt=" + invoiceOpenAmt + ", DiscountAmt=" + discountAmt);
        }

        //	Currency Changed - convert all
        if (colName == "C_Currency_ID" || colName == "C_ConversionType_ID") {

            writeOffAmt = (writeOffAmt * currencyRate).toFixed(currency["StdPrecision"]);
            //  currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
            mTab.setValue("WriteOffAmt", writeOffAmt);
            overUnderAmt = (overUnderAmt * currencyRate).toFixed(currency["StdPrecision"]);
            //currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
            mTab.setValue("OverUnderAmt", overUnderAmt);

            // nnayak - Entered Discount amount should be converted to entered currency 
            enteredDiscountAmt = (enteredDiscountAmt * currencyRate).toFixed(currency["StdPrecision"]);
            //currency.GetStdPrecision());//, MidpointRounding.AwayFromZero);
            mTab.setValue("DiscountAmt", enteredDiscountAmt);

            payAmt = (((invoiceOpenAmt - discountAmt) - writeOffAmt) - overUnderAmt);
            mTab.setValue("PayAmt", payAmt);
        }

            //	No Invoice - Set Discount, Witeoff, Under/Over to 0
        else if (C_Invoice_ID == 0) {
            if (VIS.Env.ZERO != discountAmt) {
                mTab.setValue("DiscountAmt", VIS.Env.ZERO);
            }
            if (VIS.Env.ZERO != writeOffAmt) {
                mTab.setValue("WriteOffAmt", VIS.Env.ZERO);
            }
            if (VIS.Env.ZERO != overUnderAmt) {
                mTab.setValue("OverUnderAmt", VIS.Env.ZERO);
            }
        }
            //  PayAmt - calculate write off
        else if (colName == "PayAmt") {
            overUnderAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - writeOffAmt);
            if (VIS.Env.ZERO.compareTo(overUnderAmt) > 0) {
                if (Math.abs(writeOffAmt).compareTo(discountAmt) <= 0) {
                    discountAmt = (discountAmt + overUnderAmt);
                }
                else {
                    discountAmt = VIS.Env.ZERO;
                }
                overUnderAmt = (((invoiceOpenAmt - payAmt) - discountAmt) - writeOffAmt);
            }
            mTab.setValue("DiscountAmt", discountAmt);
            mTab.setValue("OverUnderAmt", overUnderAmt);
        }
        else    //  calculate PayAmt
        {
            /* nnayak - Allow reduction in discount, but not an increase. To give a discount that is higher
               than the calculated discount, users have to enter a write off */
            //Source code modified by Suganthi for Allowing positive and negative discount in order
            //if(EnteredDiscountAmt.compareTo(DiscountAmt)<0)
            discountAmt = enteredDiscountAmt;
            payAmt = (((invoiceOpenAmt - discountAmt) - writeOffAmt) - overUnderAmt);
            mTab.setValue("PayAmt", payAmt);
            mTab.setValue("DiscountAmt", discountAmt);
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutPayment = CalloutPayment;
    //*********** CalloutPayment End ******


    //***************CalloutOrderContract Start********************

    function CalloutOrderContract() {
        VIS.CalloutEngine.call(this, "VIS.CalloutOrderContract");//must call
    };
    VIS.Utility.inheritPrototype(CalloutOrderContract, VIS.CalloutEngine); //inherit prototype

    //	Debug Steps		


    /// <summary>
    /// Order Header Change - DocType.
    /// - InvoiceRuld/DeliveryRule/PaymentRule
    /// - temporary Document
    ///   Context:
    ///   - DocSubTypeSO
    ///   - HasCharges\
    ///   - (re-sets Business Partner info of required)
    ///            *  @param ctx      
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrderContract.prototype.DocType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** InvoiceRule AD_Reference_ID=150 */
        var INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** DeliveryRule AD_Reference_ID=151 */
        var DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var DELIVERYRULE_Availability = "A";
        /** Force = F */
        var DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var DELIVERYRULE_AfterReceipt = "R";

        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        var sql = "";
        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var C_DocType_ID = value;		//	Actually C_DocTypeTarget_ID
            if (C_DocType_ID == null || C_DocType_ID == 0) {
                return "";
            }
            var oldDocNo;
            //if (mTab.getValue("DocumentNo") == DBNull.Value)
            //{
            //    oldDocNo = DBNull.Value.toString();
            //}
            //else
            //{
            oldDocNo = mTab.getValue("DocumentNo");
            // }
            //	Re-Create new DocNo, if there is a doc number already
            //	and the existing source used a different Sequence number
            //var oldDocNo = (String)mTab.getValue("DocumentNo");
            //var newDocNo = (oldDocNo == null);
            var newDocNo = (oldDocNo == null);
            if (!newDocNo && oldDocNo.toString().startsWith("<") && oldDocNo.toString().endsWith(">"))
                newDocNo = true;
            var oldC_DocType_ID = mTab.getValue("C_DocType_ID");

            sql = "SELECT d.DocSubTypeSO,d.HasCharges,'N',"			//	1..3
                 + "d.IsDocNoControlled,s.CurrentNext,s.CurrentNextSys,"     //  4..6
                 + "s.AD_Sequence_ID,d.IsSOTrx, d.IsReturnTrx "              //	7..9
                /*//jz right outer join
                + "FROM C_DocType d, AD_Sequence s "
                + "WHERE C_DocType_ID=?"	//	#1
                + " AND d.DocNoSequence_ID=s.AD_Sequence_ID(+)"; */
                 + "FROM C_DocType d "
                 + "LEFT OUTER JOIN AD_Sequence s ON (d.DocNoSequence_ID=s.AD_Sequence_ID) "
                 + "WHERE C_DocType_ID=";	//	#1

            var AD_Sequence_ID = 0;
            //DataSet ds = new DataSet();
            var ds = null;
            //	Get old AD_SeqNo for comparison
            if (!newDocNo && oldC_DocType_ID != 0) {
                sql = sql + oldC_DocType_ID;
                ds = VIS.DB.executeDataset(sql, null);
                //ds.setInt(1, oldC_DocType_ID.intValue());
                //ResultSet dr = ds.executeQuery();
                for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                    // DataRow dr = ds.Tables[0].Rows[i];
                    AD_Sequence_ID = Util.getValueOfInt(ds.getRows()[i].getCell("AD_Sequence_ID"));
                }
            }
            else {
                sql = sql + C_DocType_ID;
                ds = VIS.DB.executeDataset(sql, null);
            }
            var DocSubTypeSO = "";
            var isSOTrx = true;
            var isReturnTrx = false;
            //	we found document type
            for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                // DataRow dr = ds.Tables[0].Rows[i];
                //	Set Context:	Document Sub Type for Sales Orders
                DocSubTypeSO = Util.getValueOfString(ds.getRows()[i].getCell("DocSubTypeSO"));
                if (DocSubTypeSO == null)
                    DocSubTypeSO = "--";
                ctx.setContext(windowNo, "OrderType", DocSubTypeSO);
                //	No Drop Ship other than Standard
                if (!DocSubTypeSO.toString().equals(DocSubTypeSO_Standard))
                    mTab.setValue("IsDropShip", "N");

                //	IsSOTrx
                if ("N".toString().equals(Util.getValueOfString(ds.getRows()[i].getCell("IsSOTrx"))))
                    isSOTrx = false;

                //IsReturnTrx
                isReturnTrx = "Y".toString().equals(Util.getValueOfString(ds.getRows()[i].getCell("IsReturnTrx")));

                //	Skip these steps for RMA. These are copied from the Original Order
                if (!isReturnTrx) {
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS))
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Force);
                    else if (DocSubTypeSO.toString().equals(DocSubTypeSO_Prepay))
                        mTab.setValue("DeliveryRule", DELIVERYRULE_AfterReceipt);
                    else
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Availability);

                    //	Invoice Rule
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS)
                        || DocSubTypeSO.toString().equals(DocSubTypeSO_Prepay)
                        || DocSubTypeSO.toString().equals(DocSubTypeSO_OnCredit))
                        mTab.setValue("InvoiceRule", INVOICERULE_Immediate);
                    else
                        mTab.setValue("InvoiceRule", INVOICERULE_AfterDelivery);

                    //	Payment Rule - POS Order
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS))
                        mTab.setValue("PaymentRule", PAYMENTRULE_Cash);
                    else
                        mTab.setValue("PaymentRule", PAYMENTRULE_OnCredit);

                    //	Set Context:
                    ctx.setContext(windowNo, "HasCharges", Util.getValueOfString(ds.getRows()[i].getCell("HasCharges")));
                }
                else // Returns
                {
                    if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS))
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Force);
                    else
                        mTab.setValue("DeliveryRule", DELIVERYRULE_Manual);
                }

                //	DocumentNo
                if (dr[3].toString().equals("Y"))			//	IsDocNoControlled
                {
                    if (!newDocNo && AD_Sequence_ID != Util.getValueOfInt(ds.getRows()[i].getCell("AD_Sequence_ID")))
                        newDocNo = true;
                    if (newDocNo)
                        try {
                        //if (/*Ini.IsPropertyBool(Ini._COMPIERESYS) &&*/ GetCtx().GetAD_Client_ID() < 1000000)
                        //{
                        //    mTab.setValue("DocumentNo", "<" + dr[5].toString() + ">");
                        //}
                        //else
                        //{
                            mTab.setValue("DocumentNo", "<" + Util.getValueOfString(ds.getRows()[i].getCell("CurrentNext")) + ">");
                        // }
                        }
                        catch (err) {
                            this.setCalloutActive(false);
                        }
                }
            }
            // Skip remaining steps for RMA
            if (isReturnTrx)
                return "";
            //  When BPartner is changed, the Rules are not set if
            //  it is a POS or Credit Order (i.e. defaults from Standard BPartner)
            //  This re-reads the Rules and applies them.
            if (DocSubTypeSO.toString().equals(DocSubTypeSO_POS) || DocSubTypeSO.toString().equals(DocSubTypeSO_Prepay))    //  not for POS/PrePay
            {

            }
            else {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                sql = "SELECT PaymentRule,C_PaymentTerm_ID,"            //  1..2
                    + "InvoiceRule,DeliveryRule,"                       //  3..4
                    + "FreightCostRule,DeliveryViaRule, "               //  5..6
                    + "PaymentRulePO,PO_PaymentTerm_ID "
                    + "FROM C_BPartner "
                    + "WHERE C_BPartner_ID=" + C_BPartner_ID;		//	#1
                ds = VIS.DB.executeDataset(sql, null);
                for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                    var dr = ds.getTables()[0].rows[i];
                    //	PaymentRule

                    //var s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].toString();
                    var s = Util.getValueOfString(dr.get(isSOTrx ? "paymentrule" : "paymentrulepo"));
                    if (s != null && s.toString().length != 0) {
                        if (isSOTrx && (s.toString().equals("B") || s.toString().equals("S") || s.toString().equals("U")))           	//	No Cash/Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        if (!isSOTrx && (s.toString().equals("B")))					//	No Cash for PO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    //var ii = Util.getValueOfInt(dr[isSOTrx ? "C_PaymentTerm_ID" : "PO_PaymentTerm_ID"].toString());
                    var ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                    //if (!dr.wasNull())
                    if (dr != null) {
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    }
                    //	InvoiceRule
                    s = dr[2].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = dr[3].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = dr[4].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = dr[5].toString();
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
            }   //  re-read customer rules
        }
        catch (err) {
            this.setCalloutActive(false);
            this.log.log(Level.SEVERE, sql, err);
            return e.message;

        }
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };


    /// <summary>
    /// Order Header - BPartner.
    /// - M_PriceList_ID (+ Context)
    /// - C_BPartner_Location_ID
    /// - Bill_BPartner_ID/Bill_Location_ID
    /// 	- AD_User_ID
    /// 	- POReference
    /// 	- SO_Description
    /// 	- IsDiscountPrinted
    /// 	- InvoiceRule/DeliveryRule/PaymentRule/FreightCost/DeliveryViaRule
    /// 	- C_PaymentTerm_ID
    /// </summary>
    /// <param name="ctx">Context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Model Tab</param>
    /// <param name="mField">Model Field</param>
    /// <param name="value">The new value</param>
    /// <returns>Error message or ""</returns>
    CalloutOrderContract.prototype.BPartner = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  

        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** InvoiceRule AD_Reference_ID=150 */
        var INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        var INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        var INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        var INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        var INVOICERULE_CustomerScheduleAfterDelivery = "S";

        /** DeliveryRule AD_Reference_ID=151 */
        var DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        var DELIVERYRULE_Availability = "A";
        /** Force = F */
        var DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        var DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        var DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        var DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        var DELIVERYRULE_AfterReceipt = "R";

        /** Sales Order Sub Type - SO	*/
        var DocSubTypeSO_Standard = "SO";
        /** Sales Order Sub Type - OB	*/
        var DocSubTypeSO_Quotation = "OB";
        /** Sales Order Sub Type - ON	*/
        var DocSubTypeSO_Proposal = "ON";
        /** Sales Order Sub Type - PR	*/
        var DocSubTypeSO_Prepay = "PR";
        /** Sales Order Sub Type - WR	*/
        var DocSubTypeSO_POS = "WR";
        /** Sales Order Sub Type - WP	*/
        var DocSubTypeSO_Warehouse = "WP";
        /** Sales Order Sub Type - WI	*/
        var DocSubTypeSO_OnCredit = "WI";
        /** Sales Order Sub Type - RM	*/
        var DocSubTypeSO_RMA = "RM";

        var sql = "";
        try {
            if (value == null || value.toString() == "") {
                return "";
            }
            var C_BPartner_ID = 0;
            if (value != null)
                C_BPartner_ID = Util.getValueOfInt(value.toString());
            if (C_BPartner_ID == 0)
                return "";

            // Skip rest of steps for RMA. These fields are copied over from the orignal order instead.
            var isReturnTrx = mTab.getValue("IsReturnTrx");
            if (isReturnTrx)
                return "";

            this.setCalloutActive(true);

            sql = "SELECT p.AD_Language,p.C_PaymentTerm_ID,"
                + " COALESCE(p.M_PriceList_ID,g.M_PriceList_ID) AS M_PriceList_ID, p.PaymentRule,p.POReference,"
                + " p.SO_Description,p.IsDiscountPrinted,"
                + " p.InvoiceRule,p.DeliveryRule,p.FreightCostRule,DeliveryViaRule,"
                + " p.SO_CreditLimit, p.SO_CreditLimit-p.SO_CreditUsed AS CreditAvailable,"
                + " lship.C_BPartner_Location_ID,c.AD_User_ID,"
                + " COALESCE(p.PO_PriceList_ID,g.PO_PriceList_ID) AS PO_PriceList_ID, p.PaymentRulePO,p.PO_PaymentTerm_ID,"
                + " lbill.C_BPartner_Location_ID AS Bill_Location_ID, p.SOCreditStatus, lbill.IsShipTo "
                + "FROM C_BPartner p"
                + " INNER JOIN C_BP_Group g ON (p.C_BP_Group_ID=g.C_BP_Group_ID)"
                + " LEFT OUTER JOIN C_BPartner_Location lbill ON (p.C_BPartner_ID=lbill.C_BPartner_ID AND lbill.IsBillTo='Y' AND lbill.IsActive='Y')"
                + " LEFT OUTER JOIN C_BPartner_Location lship ON (p.C_BPartner_ID=lship.C_BPartner_ID AND lship.IsShipTo='Y' AND lship.IsActive='Y')"
                + " LEFT OUTER JOIN AD_User c ON (p.C_BPartner_ID=c.C_BPartner_ID) "
                + "WHERE p.C_BPartner_ID=" + C_BPartner_ID + " AND p.IsActive='Y'";		//	#1
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";

            var ds = VIS.DB.executeDataset(sql, null);
            for (var i = 0; i < ds.getTables()[0].rows.count; i++) {
                var dr = ds.getTables()[0].rows[i];
                //	PriceList (indirect: IsTaxIncluded & Currency)
                var ii = Util.getValueOfInt(dr.get(isSOTrx ? "m_pricelist_id" : "po_pricelist_id"));
                if (dr != null && ii != 0) {
                    mTab.setValue("M_PriceList_ID", ii);
                }
                else {	//	get default PriceList
                    var i1 = ctx.getContextAsInt("#M_PriceList_ID");
                    if (i1 != 0)
                        mTab.setValue("M_PriceList_ID", i1);
                }
                //	Bill-To BPartner
                mTab.setValue("Bill_BPartner_ID", C_BPartner_ID);
                var bill_Location_ID = Util.getValueOfInt(dr.get("bill_location_id"));
                if (bill_Location_ID == 0)
                    mTab.setValue("Bill_Location_ID", null);
                else
                    mTab.setValue("Bill_Location_ID", bill_Location_ID);

                // Ship-To Location
                var shipTo_ID = Util.getValueOfInt(dr.get("c_bpartner_location_id"));
                //	overwritten by InfoBP selection - works only if InfoWindow
                //	was used otherwise creates error (uses last value, may belong to differnt BP)
                if (C_BPartner_ID.toString().equals(ctx.getContext("C_BPartner_ID"))) {
                    var loc = ctx.getContext("C_BPartner_Location_ID");
                    if (loc.toString().length > 0)
                        shipTo_ID = int.Parse(loc);
                }
                if (shipTo_ID == 0)
                    mTab.setValue("C_BPartner_Location_ID", null);
                else {
                    mTab.setValue("C_BPartner_Location_ID", shipTo_ID);
                    if ("Y".toString().equals(Util.getValueOfString(dr.get("isshipto"))))	//	set the same
                        mTab.setValue("Bill_Location_ID", shipTo_ID);
                }
                //	Contact - overwritten by InfoBP selection
                var contID = Util.getValueOfInt(dr.get("ad_user_id").toString());
                if (C_BPartner_ID.toString().toString().equals(ctx.getContext("C_BPartner_ID"))) {
                    var cont = ctx.getContext("AD_User_ID");
                    if (cont.toString().length > 0)
                        contID = Util.getValueOfInt(cont);
                }
                if (contID == 0)
                    mTab.setValue("AD_User_ID", null);
                else {
                    mTab.setValue("AD_User_ID", contID);
                    mTab.setValue("Bill_User_ID", contID);
                }

                //	CreditAvailable 
                if (isSOTrx) {
                    var CreditLimit = Util.getValueOfDouble(dr.get("so_creditlimit"));
                    //	var SOCreditStatus = dr.getString("SOCreditStatus");
                    if (CreditLimit != 0) {
                        var CreditAvailable = Util.getValueOfDouble(dr.get("creditavailable"));
                        if (dr != null && CreditAvailable < 0) {
                            //mTab.fireDataStatusEEvent("CreditLimitOver",
                            //    DisplayType.getNumberFormat(DisplayType.Amount).format(CreditAvailable),
                            //    false);
                            // MessageBox.Show("Create fireDataStatusEEvent");
                        }
                    }
                }

                //	VAdvantage.Model.PO Reference
                var s = dr.get("poreference").toString();
                if (s != null && s.toString().length != 0)
                    mTab.setValue("POReference", s);
                // should not be reset to null if we entered already value! VHARCQ, accepted YS makes sense that way
                // TODO: should get checked and removed if no longer needed!
                /*else
                    mTab.setValue("POReference", null);*/

                //	SO Description
                s = Util.getValueOfString(dr.get("so_description"));
                if (s != null && s.toString().trim().length != 0)
                    mTab.setValue("Description", s);
                //	IsDiscountPrinted
                s = Util.getValueOfString(dr.get("isdiscountprinted"));
                if (s != null && s.toString().length != 0)
                    mTab.setValue("IsDiscountPrinted", s);
                else
                    mTab.setValue("IsDiscountPrinted", "N");

                //	Defaults, if not Walkin Receipt or Walkin Invoice
                var OrderType = ctx.getContext("OrderType");
                mTab.setValue("InvoiceRule", INVOICERULE_AfterDelivery);
                mTab.setValue("DeliveryRule", DELIVERYRULE_Availability);
                mTab.setValue("PaymentRule", PAYMENTRULE_OnCredit);
                if (OrderType.toString().equals(DocSubTypeSO_Prepay)) {
                    mTab.setValue("InvoiceRule", INVOICERULE_Immediate);
                    mTab.setValue("DeliveryRule", DELIVERYRULE_AfterReceipt);
                }
                else if (OrderType.toString().equals(MOrder.DocSubTypeSO_POS))	//  for POS
                    mTab.setValue("PaymentRule", PAYMENTRULE_Cash);
                else {
                    //	PaymentRule
                    s = dr[isSOTrx ? "PaymentRule" : "PaymentRulePO"].toString();
                    if (s != null && s.length != 0) {
                        if (s.toString().equals("B"))				//	No Cache in Non POS
                            s = "P";
                        if (isSOTrx && (s.toString().equals("S") || s.toString().equals("U")))	//	No Check/Transfer for SO_Trx
                            s = "P";										//  Payment Term
                        mTab.setValue("PaymentRule", s);
                    }
                    //	Payment Term
                    ii = Util.getValueOfInt(dr.get(isSOTrx ? "c_paymentterm_id" : "po_paymentterm_id"));
                    if (dr != null && ii != 0)//ii=0 when dr return ""
                    {
                        mTab.setValue("C_PaymentTerm_ID", ii);
                    }
                    //	InvoiceRule
                    s = Util.getValueOfString(dr.get("invoicerule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("InvoiceRule", s);
                    //	DeliveryRule
                    s = Util.getValueOfString(dr.get("deliveryrule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryRule", s);
                    //	FreightCostRule
                    s = Util.getValueOfString(dr.get("freightcostrule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("FreightCostRule", s);
                    //	DeliveryViaRule
                    s = Util.getValueOfString(dr.get("deliveryviarule"));
                    if (s != null && s.length != 0)
                        mTab.setValue("DeliveryViaRule", s);
                }
            }
        }
        catch (err) {
            this.log.log(Level.SEVERE, sql, err);
            this.setCalloutActive(false);
            return e.message;
        }

        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    /// <summary>
    /// Order Line - Product.
    /// - reset C_Charge_ID / M_AttributeSetInstance_ID
    /// - PriceList, PriceStd, PriceLimit, C_Currency_ID, EnforcePriceLimit
    /// - UOM
    /// Calls Tax
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="windowNo">current Window No</param>
    /// <param name="mTab">Grid Tab</param>
    /// <param name="mField">Grid Field</param>
    /// <param name="value">New Value</param>
    /// <returns>null or error message</returns>
    CalloutOrderContract.prototype.Product = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (value == null || value.toString() == "") {
            return "";
        }
        try {  ///
            // Boolean flag1 = System.Convert.ToBoolean(value);
            //// Int32 M_Product_ID = 0;
            // var ProductType = "";
            var M_Product_ID = value;
            if (M_Product_ID == null || M_Product_ID == 0)
                return "";

            var isReturnTrx = "Y".toString().equals(ctx.getContext("IsReturnTrx"));
            if (isReturnTrx)
                return "";

            this.setCalloutActive(true);
            if (steps) {
                this.log.warning("init");
            }
            //
            mTab.setValue("C_Charge_ID", null);
            //	Set Attribute
            if (ctx.getContextAsInt(windowNo, "M_Product_ID") == M_Product_ID
                && ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID") != 0) {
                mTab.setValue("M_AttributeSetInstance_ID", Util.getValueOfInt(ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID")));
            }
            else {
                mTab.setValue("M_AttributeSetInstance_ID", null);
            }

            /*****	Price Calculation see also qty	****/
            var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
            var Qty = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
            //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
            //        M_Product_ID, C_BPartner_ID, Qty, isSOTrx);
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            // pp.SetM_PriceList_ID(M_PriceList_ID);
            /** PLV is only accurate if PL selected in header */
            var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
            //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
            var orderDate = mTab.getValue("DateOrdered");
            //pp.SetPriceDate(orderDate);
            //	
            //if (orderDate == null) {
            //    orderDate = Date.now();
            //}
            var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                       Qty.toString(), ",", //3
                                                       isSOTrx, ",", //4 
                                                       M_PriceList_ID.toString(), ",", //5
                                                       M_PriceList_Version_ID.toString(), ",", //6
                                                       null, ",", null, ",", null); //7

            var dr = null;
            dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);


            //mTab.setValue("PriceList", pp.GetPriceList());
            //mTab.setValue("PriceLimit", pp.GetPriceLimit());
            //mTab.setValue("PriceActual", pp.GetPriceStd());
            //mTab.setValue("PriceEntered", pp.GetPriceStd());
            //mTab.setValue("C_Currency_ID", Util.getValueOfInt(pp.GetC_Currency_ID()));
            //mTab.setValue("Discount", pp.GetDiscount());
            //mTab.setValue("C_UOM_ID", Util.getValueOfInt(pp.GetC_UOM_ID()));
            //mTab.setValue("QtyEntered", mTab.getValue("QtyEntered"));
            //ctx.setContext(windowNo, "EnforcePriceLimit", pp.IsEnforcePriceLimit() ? "Y" : "N");
            //ctx.setContext(windowNo, "DiscountSchema", pp.IsDiscountSchema() ? "Y" : "N");


            mTab.setValue("PriceList", dr["PriceList"]);
            mTab.setValue("PriceLimit", dr.PriceLimit);
            mTab.setValue("PriceActual", dr.PriceActual);
            mTab.setValue("PriceEntered", dr.PriceEntered);
            mTab.setValue("C_Currency_ID", Util.getValueOfInt(dr.C_Currency_ID));
            mTab.setValue("Discount", dr.Discount);
            mTab.setValue("C_UOM_ID", Util.getValueOfInt(dr.C_UOM_ID));
            mTab.setValue("QtyOrdered", mTab.getValue("QtyEntered"));
            ctx.setContext(windowNo, "EnforcePriceLimit", dr.IsEnforcePriceLimit ? "Y" : "N");
            ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");



            //	Check/Update Warehouse Setting
            //	var M_Warehouse_ID = ctx.getContextAsInt( Env.WINDOW_INFO, "M_Warehouse_ID");
            //	var wh = (int)mTab.getValue("M_Warehouse_ID");
            //	if (wh.intValue() != M_Warehouse_ID)
            //	{
            //		mTab.setValue("M_Warehouse_ID", new int(M_Warehouse_ID));
            //		ADiathis.log.warn(,windowNo, "WarehouseChanged");
            //	}

            //if (ctx.getContext("IsSOTrx"))
            if (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y") {
                //  MProduct product = MProduct.Get(ctx, M_Product_ID);
                var paramString = M_Product_ID.toString() + ",0";
                var isStocked = VIS.dataContext.getJSONRecord("MProduct/GetProduct", paramString);
                if (isStocked) {
                    var QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
                    var M_Warehouse_ID = ctx.getContextAsInt(windowNo, "M_Warehouse_ID");
                    var M_AttributeSetInstance_ID = ctx.getContextAsInt(windowNo, "M_AttributeSetInstance_ID");
                    var paramString = M_Warehouse_ID + "," + M_Product_ID + "," + M_AttributeSetInstance_ID;
                    var available = VIS.dataContext.getJSONRecord("MStorage/GetQtyAvailable", paramString);
                    //var available = MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                    if (available == null)
                        available = VIS.Env.ZERO;
                    if (available == 0) {
                        //mTab.fireDataStatusEEvent("NoQtyAvailable", "0", false);
                        //  ShowMessage.Info("NoQtyAvailable", true, "0", "");
                    }
                    else if (available.toString().compareTo(QtyEntered) < 0) {
                        //mTab.fireDataStatusEEvent("InsufficientQtyAvailable", available.toString(), false);
                        //  ShowMessage.Info("InsufficientQtyAvailable", true, available.toString(), "");
                    }
                    else {
                        var C_OrderLine_ID = 0;
                        if (mTab.getValue("C_OrderLine_ID") != null) {
                            C_OrderLine_ID = Util.getValueOfInt(mTab.getValue("C_OrderLine_ID"));
                        }

                        if (C_OrderLine_ID == null)
                            C_OrderLine_ID = 0;
                        paramString = M_Warehouse_ID.toString() + "," + M_Product_ID.toString() + "," + M_AttributeSetInstance_ID.toString() + "," + C_OrderLine_ID.toString();
                        var notReserved = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MOrderLine/GetNotReserved", paramString));
                        //var notReserved = MOrderLine.GetNotReserved(ctx,
                        //    M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                        //    C_OrderLine_ID);
                        if (notReserved == null)
                            notReserved = VIS.Env.ZERO;
                        //var total = available.subtract(notReserved);
                        var total = (available - notReserved);
                        if (total.toString().compareTo(QtyEntered) < 0) {
                            //var info = Msg.ParseTranslation(ctx, "@QtyAvailable@=" + available  //Temporary commented
                            //  + " - @QtyNotReserved@=" + notReserved + " = " + total);
                            //mTab.fireDataStatusEEvent("InsufficientQtyAvailable",info, false);
                            //ShowMessage.Info("InsufficientQtyAvailable", true, info, ""); //Temporary commented BY sarab
                        }
                    }
                }
            }
            this.setCalloutActive(false);

            if (steps) {
                this.log.warning("fini");
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            // MessageBox.Show("error in Product");
        }
        // ctx = windowNo = mTab = mField = value = oldValue = null;
        return this.Tax(ctx, windowNo, mTab, mField, value);
    };
    CalloutOrderContract.prototype.Tax = function (ctx, windowNo, mTab, mField, value, oldValue) {

        var C_Tax_ID = 0;
        var Rate = VIS.Env.ZERO;
        var TotalRate = VIS.Env.ZERO;
        var LineAmount = "";
        //var GrandTotal =VIS.Env.ZERO;
        //var Discount=Env.ZERO;
        //var taxamt =VIS.Env.ZERO;
        C_Tax_ID = Util.getValueOfInt(mTab.getValue("C_Tax_ID"));
        var sqltax = "select rate from c_tax WHERE c_tax_id=" + C_Tax_ID + "";
        Rate = Util.getValueOfDecimal(VIS.DB.executeScalar(sqltax, null, null));
        LineAmount = this.Amt(ctx, windowNo, mTab, mField, value);
        var LineNetAmt = Util.getValueOfDecimal(mTab.getValue("LineNetAmt"));
        TotalRate = Util.getValueOfDecimal((Util.getValueOfDecimal(LineNetAmt) * Util.getValueOfDecimal(Rate)) / 100);
        mTab.setValue("taxamt", TotalRate);
        // ctx = windowNo = mTab = mField = value = oldValue = null;
        return this.Amt(ctx, windowNo, mTab, mField, value);
    };

    CalloutOrderContract.prototype.Amt = function (ctx, windowNo, mTab, mField, value, oldValue) {
        //  
        
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        try {

            if (steps) {
                this.log.warning("init");
            }

            var C_UOM_To_ID = ctx.getContextAsInt(windowNo, "C_UOM_ID");
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var M_PriceList_ID = ctx.getContextAsInt(windowNo, "M_PriceList_ID");
            var aparam = M_PriceList_ID.toString();
            var dr = VIS.dataContext.getJSONRecord("MPriceList/GetPriceList", aparam);
            var StdPrecision = Util.getValueOfInt(dr[StdPrecision]);
            //var StdPrecision = MPriceList.GetPricePrecision(ctx, M_PriceList_ID);
            var QtyEntered, PriceEntered, PriceActual, PriceLimit, Discount, PriceList;
            //	get values
            QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            QtyEntered = Util.getValueOfDecimal(mTab.getValue("QtyEntered"));
            this.log.fine("QtyEntered=" + QtyEntered + ", Ordered=" + QtyEntered + ", UOM=" + C_UOM_To_ID);
            //
            PriceEntered = Util.getValueOfDecimal(mTab.getValue("PriceEntered"));
            PriceActual = Util.getValueOfDecimal(mTab.getValue("PriceActual"));

            Discount = Util.getValueOfDecimal(mTab.getValue("Discount"));
            PriceLimit = Util.getValueOfDecimal(mTab.getValue("PriceLimit"));
            PriceList = Util.getValueOfDecimal(mTab.getValue("PriceList"));
            this.log.fine("PriceList=" + PriceList + ", Limit=" + PriceLimit + ", Precision=" + StdPrecision);
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Qty changed - recalc price
            if ((mField.getColumnName().toString().equals("QtyEntered")
                || mField.getColumnName().toString().equals("QtyEntered")
                || mField.getColumnName().toString().equals("M_Product_ID"))
                && !"N".toString().equals(ctx.getContext("DiscountSchema"))) {
                var C_BPartner_ID = ctx.getContextAsInt(windowNo, "C_BPartner_ID");
                if (mField.getColumnName().toString().equals("QtyEntered")) {
                    var paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + QtyEntered.toString();
                    //QtyEntered = MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                    //    C_UOM_To_ID, QtyEntered);
                    QtyEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString));
                }
                if (QtyEntered == null) {

                }
                //  QtyEntered = QtyEntered;
                var isSOTrx = ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y";
                var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //var date = mTab.getValue("DateOrdered");
                //if (date == null) {
                //    date = Date.now();
                //}
                var paramString = M_Product_ID.toString().concat(",", C_BPartner_ID.toString(), ",", //2
                                                            QtyEntered.toString(), ",", //3
                                                            isSOTrx, ",", //4 
                                                            M_PriceList_ID.toString(), ",", //5
                                                            M_PriceList_Version_ID.toString(), ",", //6
                                                            null, ",", null,",", null); //7

                var dr = null;
                dr = VIS.dataContext.getJSONRecord("MProductPricing/GetProductPricing", paramString);



                //MProductPricing pp = new MProductPricing(ctx.getAD_Client_ID(), ctx.getAD_Org_ID(),
                //        M_Product_ID, C_BPartner_ID, QtyEntered, isSOTrx);
                //pp.SetM_PriceList_ID(M_PriceList_ID);
                //var M_PriceList_Version_ID = ctx.getContextAsInt(windowNo, "M_PriceList_Version_ID");
                //pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);
                //var date = Util.getValueOfDateTime(mTab.getValue("DateOrdered"));
                //pp.SetPriceDate(date);
                ////

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + dr.PriceStd.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));

                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, pp.GetPriceStd());
                if (PriceEntered == null)
                    PriceEntered = dr.PriceStd;
                //
                this.log.fine("QtyChanged -> PriceActual=" + dr.PriceStd
                    + ", PriceEntered=" + PriceEntered + ", Discount=" + dr.Discount);
                PriceActual = dr.PriceStd;
                mTab.setValue("PriceActual", dr.PriceActual);
                mTab.setValue("Discount", dr.Discount);
                mTab.setValue("PriceEntered", dr.PriceEntered);
                ctx.setContext(windowNo, "DiscountSchema", dr.IsDiscountSchema ? "Y" : "N");
            }
            else if (mField.getColumnName().toString().equals("PriceActual")) {
                PriceActual = Util.getValueOfDecimal(value);

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceActual.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));


                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null)
                    PriceEntered = PriceActual;
                //
                this.log.fine("PriceActual=" + PriceActual
                    + " -> PriceEntered=" + PriceEntered);
                mTab.setValue("PriceEntered", PriceEntered);
            }
            else if (mField.getColumnName().toString().equals("PriceEntered")) {
                PriceEntered = Util.getValueOfDecimal(value);

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceEntered.toString();

                PriceActual = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductTo", paramString));

                //PriceActual = (Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceEntered);
                if (PriceActual == null)
                    PriceActual = PriceEntered;
                //
                this.log.fine("PriceEntered=" + PriceEntered
                    + " -> PriceActual=" + PriceActual);
                mTab.setValue("PriceActual", PriceActual);
            }

            //  Discount entered - Calculate Actual/Entered
            if (mField.getColumnName().toString().equals("Discount")) {

                PriceActual = Util.getValueOfDecimal(((100.0 - Util.getValueOfDouble(Discount))
                    / 100.0 * Util.getValueOfDouble(PriceList)));
                if (Util.scale(PriceActual) > StdPrecision)
                    PriceActual = PriceActual.toFixed(StdPrecision);//);//, MidpointRounding.AwayFromZero);

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceActual.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));

                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceActual.Value);
                if (PriceEntered == null)
                    PriceEntered = PriceActual;
                mTab.setValue("PriceActual", PriceActual);
                mTab.setValue("PriceEntered", PriceEntered);
            }
                //	calculate Discount
            else {
                if (PriceList == 0) {
                    Discount = VIS.Env.ZERO;
                }
                else {
                    //Discount = new Bigvar ((PriceList.doubleValue() - PriceActual.doubleValue()) / PriceList.doubleValue() * 100.0);
                    Discount = Util.getValueOfDecimal((((PriceList) - (PriceActual)) / (PriceList) * 100.0));
                }
                if (Util.scale(Discount) > 2)
                    // Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                    Discount = Discount.toFixed(2);//);//, MidpointRounding.AwayFromZero);


                mTab.setValue("Discount", Discount);
            }
            this.log.fine("PriceEntered=" + PriceEntered + ", Actual=" + PriceActual + ", Discount=" + Discount);

            //	Check PriceLimit
            var epl = ctx.getContext("EnforcePriceLimit");
            var enforce = (ctx.getWindowContext(windowNo, "IsSOTrx", true) == "Y") && epl != null && epl.toString().equals("Y");
            var isReturnTrx = "Y".toString().equals(ctx.getContext("IsReturnTrx"));
            if (enforce && (VIS.MRole.getDefault().IsOverwritePriceLimit() || isReturnTrx))
                enforce = false; //Temporary Commented
            //	Check Price Limit?
            if (enforce && Util.getValueOfDouble(PriceLimit) != 0.0
              && PriceActual.toString().compare(PriceLimit) < 0) {
                PriceActual = PriceLimit;

                paramString = M_Product_ID.toString() + "," + C_UOM_To_ID.toString() + "," + PriceLimit.toString();

                PriceEntered = Util.getValueOfDecimal(VIS.dataContext.getJSONRecord("MUOMConversion/ConvertProductFrom", paramString));

                //PriceEntered = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
                //    C_UOM_To_ID, PriceLimit.Value);
                if (PriceEntered == null)
                    PriceEntered = PriceLimit;
                this.log.fine("(under) PriceEntered=" + PriceEntered + ", Actual" + PriceLimit);
                mTab.setValue("PriceActual", PriceLimit);
                mTab.setValue("PriceEntered", PriceEntered);
                //mTab.fireDataStatusEEvent("UnderLimitPrice", "", false);
                //ShowMessage.Info("UnderLimitPrice", true, "", ""); //Temporary commented by sarab
                VIS.ADialog.info("UnderLimitPrice");
                //	Repeat Discount calc
                if (PriceList != 0) {
                    Discount = Util.getValueOfDecimal((((PriceList) - (PriceActual)) / (PriceList) * 100.0));
                    if (Util.scale(Discount) > 2) {
                        // Discount = Decimal.Round(Discount.Value, 2);//, MidpointRounding.AwayFromZero);
                        Discount = Discount.toFixed(2);//);//, MidpointRounding.AwayFromZero);
                    }
                    mTab.setValue("Discount", Discount);
                }
            }

            //	Line Net Amt
            var LineNetAmt = (QtyEntered * PriceActual);
            if (Util.scale(LineNetAmt) > StdPrecision)
                // LineNetAmt = Decimal.Round(LineNetAmt, StdPrecision);//, MidpointRounding.AwayFromZero);
                LineNetAmt = LineNetAmt.toFixed(StdPrecision);//);//, MidpointRounding.AwayFromZero);
            this.log.info("LineNetAmt=" + LineNetAmt);
            mTab.setValue("LineNetAmt", LineNetAmt);


            mTab.setValue("GrandTotal", (Util.getValueOfDecimal(mTab.getValue("TaxAmt")) + LineNetAmt));
            //Decimal? GrandTotal = Decimal.Add(Util.getValueOfDecimal(mTab.getValue("TaxAmt")), LineNetAmt);


        }
        catch (err) {

            // MessageBox.Show("error in Amt" + ex.Message.toString());
            //return "";
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutOrderContract = CalloutOrderContract;
    //*********************************

    //***************CalloutKPI Start********************

    function CalloutKPI() {
        VIS.CalloutEngine.call(this, "VIS.CalloutKPI");//must call
    };
    VIS.Utility.inheritPrototype(CalloutKPI, VIS.CalloutEngine); //inherit prototype

    CalloutKPI.prototype.CalculationSelection = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "" || value == false) {
            return "";
        }
        this.setCalloutActive(true);

        if (mField.getColumnName() == "IsSum") {
            //DisplayType.Integer;
            mTab.setValue("IsMaximum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsMinimum", false);
        }
        else if (mField.getColumnName() == "IsMaximum") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsMinimum", false);
            mTab.setValue("IsCount", false);

        }
        else if (mField.getColumnName() == "IsCount") {
            mTab.setValue("IsMaximum", false);
            mTab.setValue("IsSum", false);
            mTab.setValue("IsMinimum", false);

        }
        else if (mField.getColumnName() == "IsMinimum") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsMaximum", false);

        }

        this.setCalloutActive(false);
        return "";
    };



    CalloutKPI.prototype.UpdateKPITableInContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        if (value == true) {
            ctx.setContext(windowNo, "AD_Table_ID", -1);
        }
        else {
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };



    CalloutKPI.prototype.UpdateTabIDContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        if (value < 1 || value == null || value.toString() == "") {
            ctx.setContext(windowNo, "AD_Tab_ID", -1);
        }
        else {
            ctx.setContext(windowNo, "AD_Tab_ID", value);
        }

        this.setCalloutActive(false);
        return "";
    };



    VIS.Model.CalloutKPI = CalloutKPI;

    //**************************CalloutKPI End******************************\\



    //***************CalloutDashboard Start********************

    function CalloutDashboard() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDashboard");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDashboard, VIS.CalloutEngine); //inherit prototype

    CalloutDashboard.prototype.UpdateTableInContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);

        if (value == true) {
            ctx.setContext(windowNo, "AD_Table_ID", -1);
        }
        else {
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };

    CalloutDashboard.prototype.UpdateTabIDContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        if (value < 1 || value == null || value.toString() == "") {
            ctx.setContext(windowNo, "AD_Tab_ID", -1);
        }
        else {
            ctx.setContext(windowNo, "AD_Tab_ID", value);
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };

    CalloutDashboard.prototype.UpdateTableViewIDOnTableContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (value > 0) {
            ctx.setContext(windowNo, "TableView_ID", -1);
        }

        this.setCalloutActive(false);
        return "";
    };


    CalloutDashboard.prototype.SelectFunctionOnDashboard = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null || value.toString() == "" || value == false) {
            return "";
        }
        this.setCalloutActive(true);
        var bl = Util.getValueOfBoolean(value);

        if (mField.getColumnName() == "IsSum") {
            //DisplayType.Integer;
            mTab.setValue("IsAvg", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsNone", false);
        }
        else if (mField.getColumnName() == "IsAvg") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsNone", false);

        }
        else if (mField.getColumnName() == "IsCount") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsAvg", false);
            mTab.setValue("IsNone", false);

        }
        else if (mField.getColumnName() == "IsNone") {
            mTab.setValue("IsSum", false);
            mTab.setValue("IsCount", false);
            mTab.setValue("IsAvg", false);

        }
        this.setCalloutActive(false);


        return "";
    }


    VIS.Model.CalloutDashboard = CalloutDashboard;

    //**************************CalloutDashboard End******************************\\


    //***************CalloutDashboard Start********************

    function CalloutDashboardView() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDashboardView");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDashboardView, VIS.CalloutEngine); //inherit prototype

    CalloutDashboardView.prototype.UpdateTabIDContext = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        if (value < 1 || value == null || value.toString() == "") {
            ctx.setContext(windowNo, "AD_Tab_ID", -1);
        }
        else {
            ctx.setContext(windowNo, "AD_Tab_ID", value);
        }

        this.setCalloutActive(false);
        return "";
    };

    CalloutDashboardView.prototype.GroupByChecked = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null) {
            return "";
        }
        this.setCalloutActive(true);
        var sql = "UPDATE RC_ViewColumn SET IsGroupBy='N' WHERE RC_View_ID=" + mTab.getValue("RC_View_ID") + " AND RC_ViewColumn_ID NOT IN(" + mTab.getValue("RC_ViewColumn_ID") + ")";
        var count = VIS.DB.executeQuery(sql);
        this.setCalloutActive(false);
        return "";

    };
    CalloutDashboardView.prototype.IsViewChecked = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (this.isCalloutActive() || value == null) {
            return "";
        }
        this.setCalloutActive(true);
        if (value) {
            mTab.setValue("AD_Tab_ID", -1);
            mTab.setValue("AD_Table_ID", -1);
        }
        else {
            mTab.setValue("TableView_ID", -1);
        }
        this.setCalloutActive(false);
        return "";

    };
    VIS.Model.CalloutDashboardView = CalloutDashboardView;

    //**************************CalloutDashboard End******************************\\

    //****************CalloutService Start***********
    function CalloutService() {
        VIS.CalloutEngine.call(this, "VIS.CalloutService");//must call
    };
    VIS.Utility.inheritPrototype(CalloutService, VIS.CalloutEngine); //inherit prototype

    /**
     *  @param ctx      Context
     *  @param WindowNo current Window No
     *  @param mTab     Model Tab
     *  @param mField   Model Field
     *  @param value    The new value
     *  @return Error message or ""
     */
    CalloutService.prototype.StatisticGroup = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        //When we change the Statistic Group the Statistic subgroup 
        //value will be cleared
        mTab.setValue("FO_STATISTICSUBGROUP_ID", 0);
        return "";
    }
    //CalloutService.prototype.SelectFunctionOnDashboard = function (ctx, windowNo, mTab, mField, value, oldValue) {

    //    if (this.isCalloutActive() || value == null || value.toString() == "" || value == false) {
    //        return "";
    //    }
    //    this.setCalloutActive(true);
    //    var bl = Util.getValueOfBoolean(value);

    //    if (mField.getColumnName() == "IsSum") {
    //        //DisplayType.Integer;
    //        mTab.setValue("IsAvg", false);
    //        mTab.setValue("IsCount", false);
    //        mTab.setValue("IsNone", false);
    //    }
    //    else if (mField.getColumnName() == "IsAvg") {
    //        mTab.setValue("IsSum", false);
    //        mTab.setValue("IsCount", false);
    //        mTab.setValue("IsNone", false);

    //    }
    //    else if (mField.getColumnName() == "IsCount") {
    //        mTab.setValue("IsSum", false);
    //        mTab.setValue("IsAvg", false);
    //        mTab.setValue("IsNone", false);

    //    }
    //    else if (mField.getColumnName() == "IsNone") {
    //        mTab.setValue("IsSum", false);
    //        mTab.setValue("IsCount", false);
    //        mTab.setValue("IsAvg", false);

    //    }
    //    this.setCalloutActive(false);


    //    return "";
    //}

    VIS.Model.CalloutService = CalloutService;
    //*****************CalloutService Ends*******************************
    //*******CalloutSetReadOnly Starts**************
    function CalloutSetReadOnly() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetReadOnly");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetReadOnly, VIS.CalloutEngine); //inherit prototype


    CalloutSetReadOnly.prototype.SetReadnly = function (ctx, windowNo, mTab, mField, value, oldValue) {
       
        if (value == null || value.toString() == "" || value.toString() == "E") {
            this.setCalloutActive(false);
            if (value != null) {
                if (value.toString() == "E") {
                    mTab.getField("VSS_PAYMENTTYPE").setReadOnly(true);
                    mTab.setValue("VSS_PAYMENTTYPE", "P");
                }
            }
            //
            return "";
        }
        this.setCalloutActive(true);
        if (Util.getValueOfString(mTab.getValue("CashType")) == "A" || Util.getValueOfString(mTab.getValue("CashType")) == "E") {
            mTab.setValue("VSS_PAYMENTTYPE", "P");
            mTab.getField("VSS_PAYMENTTYPE").setReadOnly(true);
        }
        else if (Util.getValueOfString(mTab.getValue("CashType")) == "F" || Util.getValueOfString(mTab.getValue("CashType")) == "R") {
            mTab.setValue("VSS_PAYMENTTYPE", "R");
            mTab.getField("VSS_PAYMENTTYPE").setReadOnly(true);
        }
        else if (Util.getValueOfString(mTab.getValue("CashType")) == "I") {
            mTab.getField("VSS_PAYMENTTYPE").setReadOnly(true);
        }
        else {
            mTab.getField("VSS_PAYMENTTYPE").setReadOnly(false);
        }

        if (Util.getValueOfString(mTab.getValue("VSS_PAYMENTTYPE")) == "P") {
            if (Util.getValueOfDecimal(mTab.getValue("amount")) > 0) {
                mTab.setValue("Amount", (0 - Util.getValueOfDecimal(mTab.getValue("amount"))));
            }
        }
        else if (Util.getValueOfString(mTab.getValue("VSS_PAYMENTTYPE")) == "R") {
            if (Util.getValueOfDecimal(mTab.getValue("amount")) < 0) {
                mTab.setValue("Amount", (0 - Util.getValueOfDecimal(mTab.getValue("amount"))));
            }
        }
        this.setCalloutActive(false);
        return "";
    }
    CalloutSetReadOnly.prototype.SetAmountValue = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            mTab.getField("VSS_PAYMENTTYPE").setReadOnly(false);
            this.setCalloutActive(false);
            return "";
        }
        this.setCalloutActive(true);
        if (Util.getValueOfString(mTab.getValue("VSS_PAYMENTTYPE")) == "P") {
            if (Util.getValueOfDecimal(mTab.getValue("amount")) > 0) {
                mTab.setValue("Amount", (0 - Util.getValueOfDecimal(mTab.getValue("amount"))));
            }
        }
        else if (Util.getValueOfString(mTab.getValue("VSS_PAYMENTTYPE")) == "R") {
            if (Util.getValueOfDecimal(mTab.getValue("amount")) < 0) {
                mTab.setValue("Amount", (0 - Util.getValueOfDecimal(mTab.getValue("amount"))));
            }
        }
        this.setCalloutActive(false);
        return "";
    }

    VIS.Model.CalloutSetReadOnly = CalloutSetReadOnly;
    //*******CalloutSetReadOnly Ends**************


    //***************CalloutSetContract Starts***********
    function CalloutSetContract() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetContract");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetContract, VIS.CalloutEngine); //inherit prototype


    CalloutSetContract.prototype.SetContract = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var M_Product_ID = ctx.getContextAsInt(windowNo, "M_Product_ID");
            var paramStr = M_Product_ID.toString().concat(","); //1
            var productType = VIS.dataContext.getJSONRecord("MProduct/GetProductType", paramStr);

            // var precision = gp;//MProduct.get(ctx, M_Product_ID).getUOMPrecision();

            // MProduct prod = new MProduct(ctx, M_Product_ID, null);
            if (productType == "S") {
                mTab.getField("IsContract").setReadOnly(false);
            }
            else {
                mTab.getValue("IsContract", false);
                mTab.getField("IsContract").setReadOnly(true);
            }
        }
        catch (ex) {
            this.log.severe(ex.toString());
        }
        this.setCalloutActive(false);
        return "";
    };
    VIS.Model.CalloutSetContract = CalloutSetContract;
    //*************** CalloutSetContract Ends*****
    //**************CalloutWorkflow Starts********
    function CallOutWorkflow() {
        VIS.CalloutEngine.call(this, "VIS.CallOutWorkflow ");//must call
    };
    VIS.Utility.inheritPrototype(CallOutWorkflow, VIS.CalloutEngine); //inherit prototype


    CallOutWorkflow.prototype.WorkflowType = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        var wfType = Util.getValueOfString(value.toString());
        if (wfType == "R") {
            var ID = Util.getValueOfInt(VIS.DB.executeScalar("SELECT AD_Table_ID FROM AD_Table WHERE IsActive='Y' AND TableName= 'VADMS_MetaData'"));
            if (ID == 0) {
                VIS.ADialog.info("No_VADMS", null, "", "");
                //ShowMessage.Error("No_VADMS", true);
                return VIS.Msg.getMsg("No_VADMS");
            }
            mTab.setValue("AD_Table_ID", ID);
        }
        this.setCalloutActive(false);
        return "";
    };
    CallOutWorkflow.prototype.SetSelectedColumn = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);

        if ((Util.getValueOfString(VIS.DB.executeScalar("SELECT ColumnName FROM AD_Column WHERE AD_Column_ID=" + value))).toLower() == "C_GenAttributeSetInstance_ID".toLower()) {
            ctx.setContext(windowNo, "IsGenericAttribute", "Y");
        }
        else {
            ctx.setContext(windowNo, "IsGenericAttribute", "N");
        }
        this.setCalloutActive(false);
        return "";
    };
    VIS.Model.CallOutWorkflow = CallOutWorkflow;
    //**************CalloutWorkflow Ends**********

    //**************CalloutSalesQuotation Starts**************
    function CalloutSalesQuotation() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSalesQuotation ");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSalesQuotation, VIS.CalloutEngine); //inherit prototype


    CalloutSalesQuotation.prototype.GetPaymentNote = function (ctx, windowNo, mTab, mField, value, oldValue) {

        if (value == null || value.toString() == "" || value == "") {
            return "";
        }
        if (this.isCalloutActive()) {
            return "";
        }
        this.setCalloutActive(true);
        var Note = Util.getValueOfString(VIS.DB.executeScalar("select documentnote from c_paymentterm where c_paymentterm_id=" + value));
        if (Note != null) {
            mTab.setValue("description", Note);
        }
        this.setCalloutActive(false);
        return "";

    }
    VIS.Model.CalloutSalesQuotation = CalloutSalesQuotation;
    //**************CalloutSalesQuotation Ends**************

    //*************CalloutSetDate Starts*****************
    function CalloutSetDate() {
        VIS.CalloutEngine.call(this, "VIS.CalloutSetDate ");//must call
    };
    VIS.Utility.inheritPrototype(CalloutSetDate, VIS.CalloutEngine); //inherit prototype


    CalloutSetDate.prototype.SetDate = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (value == null || value.toString() == "") {
            return "";
        }
        //if (this.isCalloutActive())
        //{
        //    return "";
        //}
        if (mTab.getValue("StartDate") == null || mTab.getValue("NoofCycle") == null) {
            this.setCalloutActive(false);
            return "";
        }
        else {
            try {
                this.setCalloutActive(true);
                //mTab.SetValue("EndDate", mTab.GetValue("StartDate"));
                var enddate = new Date(mTab.getValue("StartDate"));
                // int month = Util.GetValueOfInt(mTab.GetValue("NoofCycle")) + Util.GetValueOfInt(startdate.Value.Month);
                //DateTime? endate = new DateTime(startdate.Value.Year, month, startdate.Value.Day);
                var finalenddate = enddate.setMonth(enddate.getMonth() + Util.getValueOfInt(mTab.getValue("NoofCycle")));

                if (finalenddate <= 0) {
                    finalenddate = new Date();
                }
                else {
                    finalenddate = new Date(finalenddate);
                }
                finalenddate = finalenddate.toISOString();
                mTab.setValue("EndDate", finalenddate);

                this.setCalloutActive(false);
            }
            catch (err) {

            }
        }
        return "";
    }
    VIS.Model.CalloutSetDate = CalloutSetDate;
    //*************CalloutSetDate Ends*******************

    //*************CalloutDisplayButton Starts***********
    function CalloutDisplayButton() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDisplayButton ");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDisplayButton, VIS.CalloutEngine); //inherit prototype


    CalloutDisplayButton.prototype.DisplayButton = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (value == null || value.toString() == "") {
            return "";

        }
        if (Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")) != 0) {
            var sql = "select dc.DocSubTypeSO from c_doctype dc inner join c_docbasetype db on(dc.DocBaseType=db.DocBaseType)"
            + "where c_doctype_id=" + Util.getValueOfInt(mTab.getValue("C_DocTypeTarget_ID")) + " and db.DocBaseType='SOO' and dc.DocSubTypeSO in ('WR','WI')";
            var _DocBaseType = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
            if (_DocBaseType == "WR" || _DocBaseType == "WI") {
                mTab.setValue("InvoicePrint", "Y");
            }
            else {
                mTab.setValue("InvoicePrint", "N");

            }

        }
        else {
            this.setCalloutActive(false);
            return "";
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutDisplayButton = CalloutDisplayButton;
    //*************CalloutDisplayButton Ends*************
    //*************CalloutMoveConfirmLineSetQty**********//
    function CalloutMoveConfirmLine() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMoveConfirmLine");//must call
    };
    VIS.Utility.inheritPrototype(CalloutMoveConfirmLine, VIS.CalloutEngine); //inherit prototype

    CalloutMoveConfirmLine.prototype.SetQty = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }
        this.setCalloutActive(true);
        if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")))) {
            mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
            mTab.setValue("DifferenceQty", 0);
            mTab.setValue("ScrappedQty", 0);
            this.setCalloutActive(false);
            return "";
        }
        if (mField.getColumnName() == "ConfirmedQty") {         
            if (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) < 0) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("ScrappedQty", 0);
            mTab.setValue("DifferenceQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty"))));
           
        }
        else if (mField.getColumnName() == "DifferenceQty") {
            if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("DifferenceQty")))) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("ScrappedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("DifferenceQty"))));

        }
        else if (mField.getColumnName() == "ScrappedQty") {
            if (Util.getValueOfDecimal(mTab.getValue("ScrappedQty")) < 0) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            if (Util.getValueOfDecimal(mTab.getValue("TargetQty")) < (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty")))) {
                mTab.setValue("ConfirmedQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")));
                mTab.setValue("DifferenceQty", 0);
                mTab.setValue("ScrappedQty", 0);
                this.setCalloutActive(false);
                return "";
            }
            mTab.setValue("DifferenceQty", Util.getValueOfDecimal(mTab.getValue("TargetQty")) - (Util.getValueOfDecimal(mTab.getValue("ConfirmedQty")) + Util.getValueOfDecimal(mTab.getValue("ScrappedQty"))));

        }       

        this.setCalloutActive(false);
        return "";
    };
    VIS.Model.CalloutMoveConfirmLine = CalloutMoveConfirmLine;
    //*****************MoveConfirmLineSetQty********************//
    //*************CalloutDocumentType Starts***********
    function CalloutDocumentType() {
        VIS.CalloutEngine.call(this, "VIS.CalloutDocumentType");//must call
    };
    VIS.Utility.inheritPrototype(CalloutDocumentType, VIS.CalloutEngine); //inherit prototype


    CalloutDocumentType.prototype.SetSalesQuotation = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            if (Util.getValueOfString(mTab.getValue("DocSubTypeSO")) == 'OB' || Util.getValueOfString(mTab.getValue("DocSubTypeSO")) == 'ON') {
                mTab.setValue("IsSalesQuotation", true);
            }
            else {
                mTab.setValue("IsSalesQuotation", false);
            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutDocumentType = CalloutDocumentType;
    //*************CalloutDocumentType Ends*************
    //*************CalloutProjectCBParner Starts***********
    function CalloutProjectCBPartner() {
        VIS.CalloutEngine.call(this, "VIS.CalloutProjectCBPartner");//must call
    };
    VIS.Utility.inheritPrototype(CalloutProjectCBPartner, VIS.CalloutEngine); //inherit prototype


    CalloutProjectCBPartner.prototype.SetAddress = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            if (mField.getColumnName() == "C_BPartner_ID") {
                var sql = "SELECT au.ad_user_id,  cl.c_bpartner_location_id FROM c_bpartner cp  INNER JOIN c_bpartner_location cl ON cl.c_bpartner_id=cp.c_bpartner_id INNER JOIN Ad_User au ON au.c_bpartner_id   =cp.c_bpartner_id WHERE cp.c_bpartner_id= " + VIS.Utility.Util.getValueOfString(mTab.getValue("C_BPartner_ID")) + " AND cp.isactive       ='Y'  ORDER BY cp.created";

                var ds = VIS.DB.executeDataSet(sql, null);
                for (var i = 0; i < ds.tables[0].rows.length; i++) {
                    var dr = ds.tables[0].rows[i];
                    if (dr != null) {
                        var _Location_ID = VIS.Utility.Util.getValueOfInt(dr.getCell("C_BPartner_Location_ID"));
                        if (_Location_ID == 0)
                            mTab.setValue("C_BPartner_Location_ID", null);
                        else
                            mTab.setValue("C_BPartner_Location_ID", _Location_ID);

                        var _User_ID = VIS.Utility.Util.getValueOfInt(dr.getCell("AD_User_ID"));
                        if (_User_ID == 0)
                            mTab.setValue("AD_User_ID", null);
                        else
                            mTab.setValue("AD_User_ID", _User_ID);
                    }
                }
                //var _locatio_id = Util.getValueOfString(VIS.DB.executeScalar(sql, null, null));
                //if (parseInt(_locatio_id)>0) {
                //     mTab.setValue("C_BPartner_Location_ID", parseInt(_locatio_id));
                //}

            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutProjectCBPartner = CalloutProjectCBPartner;
    //*************CalloutProjectCBParner Ends*************
     //*************CalloutPaymentTypeSO Starts***********
    function CalloutPaymentTypeSO() {
        VIS.CalloutEngine.call(this, "VIS.CalloutPaymentTypeSO");//must call
    };
    VIS.Utility.inheritPrototype(CalloutPaymentTypeSO, VIS.CalloutEngine); //inherit prototype


    CalloutPaymentTypeSO.prototype.SetPaymentType = function (ctx, windowNo, mTab, mField, value, oldValue) {
        
        if ((this.isCalloutActive()) || value == null || value.toString() == "") {
            return "";
        }
        try {
            this.setCalloutActive(true);
            if (mField.getColumnName() == "PaymentRule") {

                if (mTab.getValue("PaymentRule") == null) {
                    mTab.setValue("PaymentMethod", null);
                }
                else {
                    var _PaymentMethod_ID = mTab.getValue("PaymentRule");
                    mTab.setValue("PaymentMethod", _PaymentMethod_ID);
                }

            }
        }
        catch (err) {
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        return "";
    }
    VIS.Model.CalloutPaymentTypeSO = CalloutPaymentTypeSO;
    //*************CalloutPaymentTypeSO Ends*************

    //*************CalloutMCampaign Starts***********
    function CalloutMCampaign() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMCampaign");//must call
    };
    VIS.Utility.inheritPrototype(CalloutMCampaign, VIS.CalloutEngine); //inherit prototype


    CalloutMCampaign.prototype.DateRequired = function (ctx, windowNo, mTab, mField, value, oldValue) {
        debugger;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var DateDoc, DateReq;
                DateDoc = new Date(mTab.getValue("DateContract"));
                DateReq = new Date(mTab.getValue("DateFinish"));
                if (DateReq.toISOString() < DateDoc.toISOString()) {
                    mTab.setValue("DateFinish", "");
                    this.setCalloutActive(false);
                    VIS.ADialog.info("DateInvalid", null, "", "");
                }
            this.log.fine("DateFinish=" + DateReq);
        }
        catch (err) {
            VIS.ADialog.info("DateError" + err, null, "", "");
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    VIS.Model.CalloutMCampaign = CalloutMCampaign;
    //*************CalloutMCampaign Ends*************
    
       //*************CalloutMRequest Starts***********
    function CalloutMRequest() {
        VIS.CalloutEngine.call(this, "VIS.CalloutMRequest");//must call
    };
    VIS.Utility.inheritPrototype(CalloutMRequest, VIS.CalloutEngine); //inherit prototype


    CalloutMRequest.prototype.DateRequired = function (ctx, windowNo, mTab, mField, value, oldValue) {
        debugger;
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var DateDoc, DateReq;
                DateDoc = new Date(mTab.getValue("StartDate"));
                DateReq = new Date(mTab.getValue("CloseDate"));
                if (DateReq.toISOString() < DateDoc.toISOString()) {
                    mTab.setValue("CloseDate", "");
                    this.setCalloutActive(false);
                    VIS.ADialog.info("CloseDateInvalid", null, "", "");
                }
            this.log.fine("CloseDate=" + DateReq);
        }
        catch (err) {
            VIS.ADialog.info("error in Date" + err, null, "", "");
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };
    CalloutMRequest.prototype.PlanDateRequired = function (ctx, windowNo, mTab, mField, value, oldValue) {
        if (this.isCalloutActive() || value == null || value.toString() == "") {
            return "";
        }

        this.setCalloutActive(true);
        try {
            var DateDoc, DateReq;
                DateDoc = new Date(mTab.getValue("DateStartPlan"));
                DateReq = new Date(mTab.getValue("DateCompletePlan"));
            
                if (DateReq.toISOString() < DateDoc.toISOString()) {
                    mTab.setValue("DateCompletePlan", "");
                    this.setCalloutActive(false);
                    VIS.ADialog.info("CmpDateInvalid", null, "", "");
                }
            this.log.fine("DateCompletePlan=" + DateReq);
        }
        catch (err) {
            VIS.ADialog.info("error in Date" + err, null, "", "");
            this.setCalloutActive(false);
            return err;
        }
        this.setCalloutActive(false);
        ctx = windowNo = mTab = mField = value = oldValue = null;
        return "";
    };

    VIS.Model.CalloutMRequest = CalloutMRequest;
    //*************CalloutMRequest Ends*************
})(VIS, jQuery);
