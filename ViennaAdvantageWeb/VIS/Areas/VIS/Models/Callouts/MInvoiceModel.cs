using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MInvoiceModel
    {
        /// <summary>
        /// GetInvoice
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetInvoice(Ctx ctx,string fields)
        {         
            string[] paramValue = fields.Split(',');
            int C_Invoice_ID;
            //Assign parameter value
            C_Invoice_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MInvoice inv = new MInvoice(ctx, C_Invoice_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["IsSOTrx"] = inv.IsSOTrx().ToString();
            return result;
               
        }
    }
}