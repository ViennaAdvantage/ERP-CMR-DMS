using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MCashBookModel
    {
        /// <summary>
        /// Get CashBook Detail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetCashBook(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');                
            //Assign parameter value
            int C_CashBook_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value
            MCashBook cBook = new MCashBook(ctx, C_CashBook_ID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["C_Currency_ID"] = cBook.GetC_Currency_ID().ToString();
            return result;
           
        }
    }
}