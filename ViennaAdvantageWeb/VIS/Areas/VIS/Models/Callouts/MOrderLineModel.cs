using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MOrderLineModel
    {
        /// <summary>
        /// GetOrderLine
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Dictionary<String, String> GetOrderLine(Ctx ctx,string param)
        {
           
                string[] paramValue = param.Split(',');

                Dictionary<String, String> retDic = new Dictionary<string, string>();

                //Assign parameter value
                int id;
                id = Util.GetValueOfInt(paramValue[0].ToString());
                //End Assign parameter value

                MOrderLine orderline = new MOrderLine(ctx, id, null);               

                retDic["C_Tax_ID"] = Util.GetValueOfString(orderline.GetC_Tax_ID());
                retDic["PriceList"] = Util.GetValueOfString(orderline.GetPriceList());
                retDic["PriceLimit"] = Util.GetValueOfString(orderline.GetPriceLimit());
                retDic["PriceActual"] = Util.GetValueOfString(orderline.GetPriceActual());
                retDic["PriceEntered"] = Util.GetValueOfString(orderline.GetPriceEntered());
                retDic["C_Currency_ID"] = Util.GetValueOfString(orderline.GetC_Currency_ID());
                retDic["Discount"] = Util.GetValueOfString(orderline.GetDiscount());
                retDic["Discount"] = Util.GetValueOfString(orderline.GetDiscount());
                retDic["M_Product_ID"] = Util.GetValueOfString(orderline.GetM_Product_ID());
                retDic["Qty"] = Util.GetValueOfString(orderline.GetQtyEntered());
                retDic["C_UOM_ID"] = Util.GetValueOfString(orderline.GetC_UOM_ID());
                retDic["C_BPartner_ID"] = Util.GetValueOfString(orderline.GetC_BPartner_ID());
                retDic["PlannedHours"] = Util.GetValueOfString(orderline.GetQtyOrdered());
                retDic["M_AttributeSetInstance_ID"] = Util.GetValueOfString(orderline.GetM_AttributeSetInstance_ID());
                retDic["QtyOrdered"] = Util.GetValueOfString(orderline.GetQtyOrdered());
                retDic["QtyDelivered"] = Util.GetValueOfString(orderline.GetQtyDelivered());
                retDic["QtyEntered"] = Util.GetValueOfString(orderline.GetQtyEntered());
                retDic["C_Activity_ID"] = Util.GetValueOfString(orderline.GetC_Activity_ID());
                retDic["C_Campaign_ID"] = Util.GetValueOfString(orderline.GetC_Campaign_ID());
                retDic["C_Project_ID"] = Util.GetValueOfString(orderline.GetC_Project_ID());
                retDic["C_ProjectPhase_ID"] = Util.GetValueOfString(orderline.GetC_ProjectPhase_ID());
                retDic["C_ProjectTask_ID"] = Util.GetValueOfString(orderline.GetC_ProjectTask_ID());
                retDic["AD_OrgTrx_ID"] = Util.GetValueOfString(orderline.GetAD_OrgTrx_ID());
                retDic["User1_ID"] = Util.GetValueOfString(orderline.GetUser1_ID());
                retDic["User2_ID"] = Util.GetValueOfString(orderline.GetUser2_ID());
                retDic["IsReturnTrx"] = Util.GetValueOfString(orderline.GetParent().IsReturnTrx()).ToLower();
                retDic["Orig_InOutLine_ID"] = Util.GetValueOfString(orderline.GetOrig_InOutLine_ID());
                retDic["Orig_OrderLine_ID"] = Util.GetValueOfString(orderline.GetOrig_OrderLine_ID());
                retDic["GetID"] = Util.GetValueOfString(orderline.Get_ID());

                return retDic;
        }
        /// <summary>
        /// Get Not Reserved
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetNotReserved(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            //Assign parameter value
            int M_Warehouse_ID=Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID=Util.GetValueOfInt(paramValue[1].ToString());
            int M_AttributeSetInstance_ID=Util.GetValueOfInt(paramValue[2].ToString());
            int C_OrderLine_ID = Util.GetValueOfInt(paramValue[3].ToString());
            //End Assign parameter value
            return MOrderLine.GetNotReserved(ctx, M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, C_OrderLine_ID);    
            
        }
    }
}