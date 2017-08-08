using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MOrderModel
    {
        /// <summary>
        /// GetOrder
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetOrder(Ctx ctx, string fields)
        {
           
                string[] paramValue = fields.ToString().Split(',');
                int C_Order_ID;

                //Assign parameter value
                C_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MOrder order = new MOrder(ctx, C_Order_ID, null);
                // End Assign parameter value

                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment
                retDic["C_BPartner_ID"] = Util.GetValueOfString(order.GetC_BPartner_ID());
                retDic["C_BPartner_Location_ID"] = Util.GetValueOfString(order.GetC_BPartner_Location_ID());
                retDic["Bill_BPartner_ID"] = Util.GetValueOfString(order.GetBill_BPartner_ID());
                retDic["Bill_Location_ID"] = Util.GetValueOfString(order.GetBill_Location_ID());
                if (order.GetAD_User_ID() != 0)
                    retDic["AD_User_ID"] = Util.GetValueOfString(order.GetAD_User_ID());
                if (order.GetBill_User_ID() != 0)
                retDic["Bill_User_ID"] = Util.GetValueOfString(order.GetBill_User_ID());
                retDic["M_PriceList_ID"] = Util.GetValueOfString(order.GetM_PriceList_ID());
                retDic["PaymentRule"] = order.GetPaymentRule();
                retDic["C_PaymentTerm_ID"] = Util.GetValueOfString(order.GetC_PaymentTerm_ID());
                //mTab.setValue ("DeliveryRule", X_C_Order.DELIVERYRULE_Manual);
                retDic["Bill_Location_ID"] = Util.GetValueOfString(order.GetBill_Location_ID());
                retDic["InvoiceRule"] = order.GetInvoiceRule();
                retDic["PaymentRule"] = order.GetPaymentRule();
                retDic["DeliveryViaRule"] = order.GetDeliveryViaRule();
                retDic["FreightCostRule"] = order.GetFreightCostRule();
                retDic["ID"] = Util.GetValueOfString(order.Get_ID());
                retDic["DateOrdered"] = Util.GetValueOfString(order.GetDateOrdered());
                retDic["POReference"] = order.GetPOReference();
                retDic["AD_Org_ID"] = Util.GetValueOfString(order.GetAD_Org_ID());
                retDic["DeliveryRule"] = order.GetDeliveryRule();
                retDic["DeliveryViaRule"] = order.GetDeliveryViaRule();
                retDic["M_Shipper_ID"] = Util.GetValueOfString(order.GetM_Shipper_ID());
                retDic["FreightAmt"] = Util.GetValueOfString(order.GetFreightAmt());
                retDic["AD_OrgTrx_ID"] = Util.GetValueOfString(order.GetAD_OrgTrx_ID());
                retDic["C_Activity_ID"] = Util.GetValueOfString(order.GetC_Activity_ID());
                retDic["C_Campaign_ID"] = Util.GetValueOfString(order.GetC_Campaign_ID());
                retDic["C_Project_ID"] = Util.GetValueOfString(order.GetC_Project_ID());
                retDic["User1_ID"] = Util.GetValueOfString(order.GetUser1_ID());
                retDic["User2_ID"] = Util.GetValueOfString(order.GetUser2_ID());
                retDic["M_Warehouse_ID"] = Util.GetValueOfString(order.GetM_Warehouse_ID());
                retDic["Orig_Order_ID"] = Util.GetValueOfString(order.GetOrig_Order_ID());
                retDic["Orig_InOut_ID"] = Util.GetValueOfString(order.GetOrig_InOut_ID());
                return retDic;
        }
    }
}