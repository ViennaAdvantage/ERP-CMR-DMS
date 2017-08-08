using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.Print
{
    public interface IReportEngine
    {
        byte[] GetReportBytes();
        String GetReportString();
        string GetReportFilePath(bool fetchByteArr, out byte[] bytes);
        string GetCsvReportFilePath(string data);
    }

    public interface IReportView
    {
        View GetView();
        MPrintFormat GetPrintFormat();
        void SetPrintFormat(MPrintFormat pf);
    }
}
