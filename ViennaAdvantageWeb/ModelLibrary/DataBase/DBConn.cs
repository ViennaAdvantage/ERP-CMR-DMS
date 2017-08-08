using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VAdvantage.DataBase
{
   public  class DBConn
    {

       static DBConn()
       {
             SetConnectionString();
        }

        private static string connectionString = null;


        public static void SetConnectionString()
        {
            CreateConnectionString();
            DB.SetDBTarget(VConnection.Get());
        }

        public static void SetOracleConnectionString(string connString)
        {
            VConnection vconn = VConnection.Get();
            vconn.Db_Type = DatabaseType.DB_ORACLE;
            vconn.SetAttributes(connString);
            DB.SetDBTarget(VConnection.Get());
            connectionString = connString;
        }


        public static void SetOracleConnectionString(string host_name, string port_number, string user_id, string password, string serviceName)
        {
            connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host_name + ")(PORT=" + port_number + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + serviceName + ")));User Id=" + user_id + ";Password=" + password + ";";
            VConnection vconn = VConnection.Get();
            vconn.Db_Type = DatabaseType.DB_ORACLE;
            vconn.SetAttributes(connectionString);
            DB.SetDBTarget(VConnection.Get());
        }




        public static string CreateConnectionString()
        {
            
            if (DataBase.DB.UseMigratedConnection)
            {
                if (DataBase.DB.MigrationConnection != null && DataBase.DB.MigrationConnection != "")
                {
                    VConnection vconn = VConnection.Get();
                    vconn.SetAttributes(DataBase.DB.MigrationConnection);

                    return DataBase.DB.MigrationConnection;
                }


                return "";// Ini.CreateConnectionString(WindowMigration.MDialog.GetMConnection());
            }
            else
            {
                if (connectionString == null)
                {
                    //connectionString =  System.Configuration.ConfigurationSettings.AppSettings["oracleConnectionString"];
                    VConnection vconn = VConnection.Get();
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"];

                    if(!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_ORACLE;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        return connectionString;
                    }
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["PostgreSQLConnectionString"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_POSTGRESQL;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        return connectionString;
                    }
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["PostgreSQLPlusConnectionString"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_POSTGRESQL;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        return connectionString;
                    }
                    connectionString = System.Configuration.ConfigurationManager.AppSettings["MSSQLConnectionString"];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        vconn.Db_Type = DatabaseType.DB_MSSQL;
                        vconn.SetAttributes(connectionString);
                        vconn.GetDatabase().SetConnectionString(connectionString);
                        return connectionString;
                    }


                    // connectionString =  vconn.CreateDBConnectionString();
                    //return Ini.CreateConnectionString(vconn);
                }

                return connectionString;

                //VConnection vconn = VConnection.Get();
                // return Ini.CreateConnectionString(vconn);
            }

            //s_conn.Add(1, constr);


            //}
            //return constr;   //return the connection string to the user
        }
    }
}
