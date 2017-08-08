using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MProjectModel
    {
        /// <summary>
        /// GetProjectDetail
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectDetail(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int projID;

            //Assign parameter value
            projID = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter value

            X_C_Project proj = new X_C_Project(ctx, projID, null);
            Dictionary<string, string> result = new Dictionary<string, string>();
            result["M_PriceList_Version_ID"] = proj.GetM_PriceList_Version_ID().ToString();
            return result;
        }
    }
}