using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Classes
{
    public class LookupHelper
    {
        public static Lookup GetLookup(Ctx ctx, int windowNo, int Column_ID, int AD_Reference_ID,
                 String columnName, int AD_Reference_Value_ID,
                bool IsParent, String ValidationCode)
        {
            return VLookUpFactory.Get(ctx, windowNo, Column_ID, AD_Reference_ID, columnName, AD_Reference_Value_ID, IsParent, ValidationCode);
        }
    }
}