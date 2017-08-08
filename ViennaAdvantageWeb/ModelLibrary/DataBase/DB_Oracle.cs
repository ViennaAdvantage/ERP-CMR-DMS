using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{
    class DB_Oracle : ViennaDatabase
    {

        private string connectionString = null;
        private System.Data.IDbConnection con = null;


        public int GetStandardPort()
        {
            return 1541;
        }

        public bool SupportsBLOB()
        {
            return true;
        }

        public string ConvertStatement(string oraStatement)
        {
            return oraStatement;
        }

        public string TO_DATE(DateTime? time, bool dayOnly)
        {
            StringBuilder dateString = new StringBuilder("");
            string myDate = "";

            if (time == null)
            {
                if (dayOnly)
                    return "TRUNC(SysDate)";
                return "SysDate";
            }

            dateString = new StringBuilder("TO_DATE('");
            //  YYYY-MM-DD HH24:MI:SS.mmmm  JDBC Timestamp format
            //String myDate = time.ToString("yyyy-mm-dd");
            //myDate = time.ToString("yyyy-MM-dd HH:mm:ss");
            myDate = time.Value.ToString();//"yyyy-MM-dd");
            if (dayOnly)
            {
                myDate = time.Value.ToString("yyyy-MM-dd");
                dateString.Append(myDate);
                dateString.Append("','YYYY-MM-DD')");
            }
            else
            {
                myDate = time.Value.ToString("yyyy-MM-dd HH:mm:ss");
                dateString.Append(myDate);	//	cut off miliseconds
                dateString.Append("','YYYY-MM-DD HH24:MI:SS')");
            }
            return dateString.ToString();
        }

        public string TO_CHAR(string columnName, int displayType, string AD_Language)
        {
            StringBuilder retValue = new StringBuilder("TRIM(TO_CHAR(");
           
                retValue.Append(columnName);

                //  Numbers
                if (VAdvantage.Classes.DisplayType.IsNumeric(displayType))
                {
                    if (displayType == VAdvantage.Classes.DisplayType.Amount)
                        retValue.Append(",'9G999G990D00'");
                    else
                        retValue.Append(",'TM9'");
                    //  TO_CHAR(GrandTotal,'9G999G990D00','NLS_NUMERIC_CHARACTERS='',.''')
                    //if (!Language.isDecimalPoint(AD_Language))      //  reversed
                    //    retValue.append(",'NLS_NUMERIC_CHARACTERS='',.'''");
                }
                else if (VAdvantage.Classes.DisplayType.IsDate(displayType))
                {
                    retValue.Append(",'")
                        .Append("yyyy-MM-dd")
                        .Append("'");
                }
                retValue.Append("))");
                //
                return retValue.ToString();
        }

        public string TO_NUMBER(decimal? number, int displayType)
        {
            if (number == null)
                return "NULL";
            Decimal result = number.Value;
            //int scale = VAdvantage.Classes.DisplayType.GetDefaultPrecision(displayType);
            //if (scale > Decimal.  number.Value.())
            //{
            //    try
            //    {
            //        result = number.setScale(scale, BigDecimal.ROUND_HALF_UP);
            //    }
            //    catch (Exception e)
            //    {
            //        //  log.severe("Number=" + number + ", Scale=" + " - " + e.getMessage());
            //    }
            //}
            return result.ToString();
        }

        public int GetNextID(string Name)
        {
            throw new NotImplementedException();
        }

        public bool CreateSequence(string name, int increment, int minvalue, int maxvalue, int start, Trx trxName)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            return DatabaseType.DB_ORACLE;
        }




        public System.Data.IDbConnection GetCachedConnection(bool autoCommit, int transactionIsolation)
        {
            if (con == null)
            {
                con = new OracleConnection(connectionString);
            }
            return con;
        }

        public void SetConnectionString(string conString)
        {
            connectionString = conString;
        }


        public System.Data.DataSet ExecuteDatasetPaging(string sql, int page, int pageSize, int increment)
        {
            DataSet ds = null;
            OracleConnection connection = new OracleConnection(connectionString);
            try
            {
                connection.Open();
                OracleDataAdapter adapter = new OracleDataAdapter();
                adapter.SelectCommand = new OracleCommand(sql);
                adapter.SelectCommand.Connection = (OracleConnection)connection;
                ds = new DataSet();

                if (page < 1)// Set rowcount =PageNumber * PageSize for best performance
                {
                    page = 1;
                }
                adapter.Fill(ds, ((page - 1) * pageSize) + increment, pageSize - increment, "Data");

                //adapter.FillSchema(ds, SchemaType.Mapped, "DataSchema");

                //if (ds != null && ds.Tables.Count > 1)
                //{
                //    DataTable data = ds.Tables["Data"];
                //    DataTable schema = ds.Tables["DataSchema"];
                //    data.Merge(schema);
                //    ds.Tables.Remove(schema);
                //}
            }
            catch
            {
                //
                ds = null;
            }
            finally
            {
                connection.Close();
            }
            return ds;
        }
    }
}
