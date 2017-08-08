using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MPaymentModel
    {
        /// <summary>
        /// GetPayment
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetPayment(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Payment_ID;
            //Assign parameter value
            C_Payment_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MPayment payment = new MPayment(ctx, C_Payment_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["C_Charge_ID"] = payment.GetC_Charge_ID().ToString();
            result["C_Invoice_ID"] = payment.GetC_Invoice_ID().ToString();
            result["C_Order_ID"] = payment.GetC_Order_ID().ToString();
            return result;              
        }
    }
}