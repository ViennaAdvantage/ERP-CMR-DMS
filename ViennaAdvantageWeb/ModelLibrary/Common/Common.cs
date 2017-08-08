using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;

namespace VAdvantage.Common
{
    public class Common
    {
        static public List<string> lstTableName = null;
        static public bool ISTENATRUNNINGFORERP = false;
        public static string NONBUSINESSDAY = "@DateIsInNonBusinessDay@";
        public static void GetAllTable()
        {

            lstTableName = new List<string>();
            DataSet ds = DB.ExecuteDataset("select tablename from ad_table where isactive='Y'");

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 350)
                {
                    ISTENATRUNNINGFORERP = true;
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstTableName.Add(Convert.ToString(ds.Tables[0].Rows[i]["TABLENAME"]));
                }

            }

        }
    }
}
