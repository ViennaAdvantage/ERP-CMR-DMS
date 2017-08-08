using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MInOutModel
    {    
      /// <summary>
      /// GetInOut
      /// </summary>
      /// <param name="ctx"></param>
      /// <param name="param"></param>
      /// <returns></returns>
        public Dictionary<String, String> GetInOut(Ctx ctx,string param)
        {         
            string[] paramValue = param.Split(',');
            int Orig_InOut_ID;

            //Assign parameter value
            Orig_InOut_ID = Util.GetValueOfInt(paramValue[0].ToString());
            MInOut io = new MInOut(ctx, Orig_InOut_ID, null);
            //End Assign parameter

            Dictionary<String, String> retDic = new Dictionary<string, string>();                
            retDic["C_Project_ID"] = io.GetC_Project_ID().ToString();
            retDic["C_Campaign_ID"] = io.GetC_Campaign_ID().ToString();
            retDic["C_Activity_ID"] = io.GetC_Activity_ID().ToString();
            retDic["AD_OrgTrx_ID"] = io.GetAD_OrgTrx_ID().ToString();
            retDic["User1_ID"] = io.GetUser1_ID().ToString();
            retDic["User2_ID"] = io.GetUser2_ID().ToString();
            return retDic;
              
        }
     
    }
}