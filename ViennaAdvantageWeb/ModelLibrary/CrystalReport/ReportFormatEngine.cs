using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Print;
using System.Reflection;
using VAdvantage.Utility;

namespace VAdvantage.ReportFormat
{
    public class ReportFormatEngine
    {

        internal static IReportEngine Get(Utility.Ctx p_ctx, ProcessEngine.ProcessInfo _pi, out int totalRecords, bool IsArabicReportFromOutside)
        {
            IReportEngine re = null;

            Type type = null;

            try
            {
                Assembly asm = Assembly.Load("VARCOMSvc");
                type = asm.GetType("ViennaAdvantage.Classes.ReportFromatWrapper");
                ConstructorInfo cinfo = type.GetConstructor(new Type[] { typeof(Ctx), typeof(string), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) });
                re = (IReportEngine)cinfo.Invoke(new object[] { p_ctx, _pi.GetTitle(), _pi.GetAD_Process_ID(), _pi.GetTable_ID(), _pi.GetRecord_ID(), 0, 0, _pi.GetAD_PInstance_ID() });



                MethodInfo mInfo = type.GetMethod("Init");
                totalRecords = Convert.ToInt32(mInfo.Invoke(re,new object[]{IsArabicReportFromOutside}));

            }
            catch
            {
                totalRecords = 0;
            }

            return re;

        }

        internal static IReportEngine Get(Utility.Ctx p_ctx, ProcessEngine.ProcessInfo _pi, bool IsArabicReportFromOutside)
        {
            int i = 0;
            return Get(p_ctx, _pi, out i, IsArabicReportFromOutside);
        }



    }
}
