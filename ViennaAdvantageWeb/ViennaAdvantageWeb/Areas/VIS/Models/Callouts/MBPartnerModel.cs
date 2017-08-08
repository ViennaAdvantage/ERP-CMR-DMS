using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MBPartnerModel
    {

        public Dictionary<String, String> GetBPartner(Ctx ctx,string fields)
        {           
                          
                string[] paramValue = fields.Split(',');
                int C_BPartner_ID;

                //Assign parameter value
                C_BPartner_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MBPartner bpartner = new MBPartner(ctx, C_BPartner_ID, null);
                Dictionary<String, String> retDic = new Dictionary<string, string>();            

                retDic["M_ReturnPolicy_ID"] = bpartner.GetM_ReturnPolicy_ID().ToString();
                retDic["M_ReturnPolicy_ID"] = bpartner.GetPO_ReturnPolicy_ID().ToString();
                return retDic;
        }
    }
}