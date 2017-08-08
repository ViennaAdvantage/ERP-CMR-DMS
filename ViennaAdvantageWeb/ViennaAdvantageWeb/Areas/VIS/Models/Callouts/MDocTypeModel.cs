using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MDocTypeModel
    {
        /// <summary>
        /// Get DocType Detail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetDocType(Ctx ctx,string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_DocType_ID;
            //Assign parameter value
            C_DocType_ID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter
            MDocType dt = MDocType.Get(ctx, C_DocType_ID);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["IsSOTrx"] = dt.IsSOTrx().ToString();
            return result;
       
        }
    }
}