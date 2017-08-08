using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VAdvantage.Utility;
using VAdvantage.ProcessEngine;
using CrystalDecisions.CrystalReports.Engine;

using System.Data;
using System.IO;
using VAdvantage.DataBase;
using VAdvantage.Model;
using System.Data.OracleClient;
using VAdvantage.Logging;
using VAdvantage.Print;
using VAdvantage.Classes;

namespace VAdvantage.CrystalReport
{
    public class CrystalReportEngine :IReportEngine
    {
        private int AD_Process_ID = 0;
        private string reportName = "", imageField = "", imagePathField = "";
        private string SqlQuery = "";
        private bool isIncludeImage = false;
        private Ctx _ctx;
        private ProcessInfo _pi;


        public CrystalReportEngine(Ctx ctx, ProcessInfo pi)
        {
            _ctx = ctx;
            _pi = pi;

            //	Get AD_Table_ID and TableName
            String sql = "SELECT p.AD_Process_ID, p.ReportPath,p.SqlQuery,p.IncludeImage,p.ImageField,p.ImagePathField "
                + " FROM AD_PInstance pi"
                + " INNER JOIN AD_Process p ON (pi.AD_Process_ID=p.AD_Process_ID)"
                + " WHERE pi.AD_PInstance_ID='" + pi.GetAD_PInstance_ID() + "' ";

            IDataReader dr = null;
            try
            {
                dr = DataBase.DB.ExecuteReader(sql);
                //	Just get first 
                if (dr.Read())
                {
                    AD_Process_ID = Utility.Util.GetValueOfInt(dr[0].ToString());		//	required
                    reportName = dr[1].ToString();
                    SqlQuery = dr[2].ToString();

                    imageField = Utility.Util.GetValueOfString(dr[4].ToString());
                    imagePathField = dr[5].ToString();
                    isIncludeImage = "Y".Equals(dr[3].ToString());	//	required
                }

                dr.Close();
            }
            catch (Exception e1)
            {
                if (dr != null)
                {
                    dr.Close();
                }
                throw e1;
            }
        }

        public byte[] GenerateCrystalReport()
        {
            
            string reportPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Reports");
            string reportImagePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Images");
            
            

            //string _ReportImagePath = "";
            //string _ReportPath = "";

            string path = reportPath;

            //_ReportImagePath = reportImagePath;

            if (String.IsNullOrEmpty(path))
            {
                throw new MissingFieldException("CrystalReportPathNotSet");

            }
            if (reportName.IndexOf(":") < 0)
                reportPath = path + "\\" + reportName;
            else
                reportPath = reportName;


            ReportDocument rptBurndown = new ReportDocument();
            if (File.Exists(reportPath))   //Check if the crystal report file exists in a specified location.
            {
                try
                {
                    rptBurndown.Load(reportPath);

                    //Set Connection Info
                    ConnectionInfo.Get().SetAttributes(System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"]);


                    //Application will pick database info from the property file.
                    CrystalDecisions.Shared.ConnectionInfo crDbConnection = new CrystalDecisions.Shared.ConnectionInfo();
                    crDbConnection.IntegratedSecurity = false;
                    crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                    crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                    crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                    //crDbConnection.Type = ConnectionInfoType.Unknown;
                    crDbConnection.ServerName = ConnectionInfo.Get().Db_host;
                    CrystalDecisions.CrystalReports.Engine.Database crDatabase = rptBurndown.Database;
                    CrystalDecisions.Shared.TableLogOnInfo oCrTableLoginInfo;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table oCrTable in crDatabase.Tables)
                    {
                        crDbConnection.IntegratedSecurity = false;
                        crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                        crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                        crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                        //crDbConnection.Type = ConnectionInfoType.Unknown;
                        crDbConnection.ServerName = ConnectionInfo.Get().Db_host;

                        oCrTableLoginInfo = oCrTable.LogOnInfo;
                        oCrTableLoginInfo.ConnectionInfo = crDbConnection;
                        oCrTable.ApplyLogOnInfo(oCrTableLoginInfo);
                    }

                    //Create Parameter query
                    string sql = SqlQuery;
                    StringBuilder sb = new StringBuilder(" WHERE ");

                    if (_pi.GetRecord_ID() > 0 && _pi.GetTable_ID() > 0)
                    {
                        string tableName = DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_TABLE_ID =" + _pi.GetTable_ID()).ToString();
                        sb.Append(tableName).Append("_ID = ").Append(_pi.GetRecord_ID());
                    }

                    else
                    {

                        ProcessInfoUtil.SetParameterFromDB(_pi);
                        ProcessInfoParameter[] parameters = _pi.GetParameter();
                        if (parameters.Count() > 0)
                        {
                            int loopCount = 0;
                            for (int para = 0; para <= parameters.Count() - 1; para++)
                            {
                                string sInfo = parameters[para].GetInfo();
                                string sInfoTo = parameters[para].GetInfo_To();
                                if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
                                {
                                    continue;
                                }

                                if (loopCount > 0)
                                    sb.Append(" AND ");
                                string paramName = parameters[para].GetParameterName();
                                object paramValue = parameters[para].GetParameter();
                                object paramValueTo = parameters[para].GetParameter_To();

                                if (paramValue is DateTime)
                                {
                                    sb.Append(paramName).Append(" BETWEEN ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                    if (paramValueTo != null && paramValueTo.ToString() != String.Empty)
                                        sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo).AddDays(1), true));
                                    else
                                        sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValue).AddDays(1), true));

                                }
                                else if (paramValue != null && paramValue.ToString().Contains(','))
                                {
                                    sb.Append(paramName).Append(" IN (")
                                        .Append(paramValue.ToString()).Append(")");
                                }
                                else
                                {
                                    sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
                                        .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
                                }

                                loopCount++;
                            }
                        }
                    }

                    if (sb.Length > 7)
                            sql = sql + sb.ToString();

                    //if (form.IsIncludeProcedure())
                    //{
                    //    bool result = StartDBProcess(form.GetProcedureName(), parameters);
                    //}

                    DataSet ds = DB.ExecuteDataset(sql);

                    if (ds == null)
                    {
                        ValueNamePair error = VLogger.RetrieveError();
                        throw new Exception(error.GetValue() + "BlankReportWillOpen");
                    }

                    bool imageError = false;
                    if (isIncludeImage)
                    {
                        for (int i_img = 0; i_img <= ds.Tables[0].Rows.Count - 1; i_img++)
                        {
                            String ImagePath = "";
                            String ImageField = "";
                            if (ds.Tables[0].Rows[i_img][imagePathField] != null)
                            {
                                ImagePath = ds.Tables[0].Rows[i_img][imagePathField].ToString();
                                ImageField = imageField;

                                if (ds.Tables[0].Columns.Contains(ImageField))
                                {
                                    if (File.Exists(reportImagePath + "\\" + ImagePath))
                                    {
                                        byte[] b = StreamFile(reportImagePath + "\\" + ImagePath);
                                        ds.Tables[0].Rows[i_img][ImageField] = b;
                                    }
                                    else
                                    {
                                        //ds.Tables[0].Rows.RemoveAt(i_img);
                                        imageError = true;
                                    }
                                }
                                else
                                {
                                    imageError = true;
                                }
                            }
                            else
                            {
                                imageError = true;
                            }
                        }
                    }

                    if (imageError)
                    {
                        //   ShowMessage.Error("ErrorLoadingSomeImages", true);
                    }

                    //crystalReportViewer1.ReportSource = rptBurndown;
                    //crystalReportViewer1.Refresh();

                    System.IO.Stream oStream;
                    byte[] byteArray = null;

                    rptBurndown.SetDataSource(ds.Tables[0]);                //By karan approveed by lokesh......
                    //rptBurndown.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(100, 360, 100, 360));
                    oStream = rptBurndown.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                    byteArray = new byte[oStream.Length];
                    oStream.Read(byteArray, 0, Convert.ToInt32(oStream.Length));

                    return byteArray;

                    //if (form.IsDirectPrint())
                    //{
                    //    // rptBurndown.PrintOptions.PrinterName = Env.GetCtx().GetPrinterName();
                    //    //rptBurndown.PrintToPrinter(1, false, 0, 0);
                    //}
                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
            else
            {
                throw new MissingFieldException("CouldNotFindTheCrystalReport");
            }
        }


#pragma warning disable 612, 618

        private bool StartDBProcess(String procedureName, ProcessInfoParameter[] list)
        {
            if (DatabaseType.IsPostgre)  //jz Only DB2 not support stored procedure now
            {
                return false;
            }

            //  execute on this thread/connection
            //String sql = "{call " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")}";
            try
            {
                //only oracle procedure are supported
                OracleCommand comm = new OracleCommand();
                OracleConnection conn = (OracleConnection)VAdvantage.DataBase.DB.GetConnection();
                conn.Open();
                comm.Connection = conn;
                comm.CommandText = procedureName;
                comm.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(comm);
                OracleParameter[] param = new OracleParameter[comm.Parameters.Count];
                int i = 0;
                StringBuilder orclParams = new StringBuilder();
                bool isDateTo = false;
                foreach (OracleParameter orp in comm.Parameters)
                {
                    if (isDateTo)
                    {
                        isDateTo = false;
                        continue;
                    }
                    Object paramvalue = list[i].GetParameter();
                    if (paramvalue != null)
                    {
                        if (orp.DbType == System.Data.DbType.DateTime)
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                            }
                            param[i] = new OracleParameter(orp.ParameterName, paramvalue);
                            if (list[i].GetParameter_To().ToString().Length > 0)
                            {
                                paramvalue = list[i].GetParameter_To();
                                paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                                param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                i++;
                                isDateTo = true;
                                continue;
                            }
                            else
                            {
                                if (comm.Parameters.Count > (i + 1))
                                {
                                    if (comm.Parameters[i + 1].ParameterName.Equals(comm.Parameters[i].ParameterName + "_TO", StringComparison.OrdinalIgnoreCase))
                                    {
                                        param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                        isDateTo = true;
                                        continue;
                                    }
                                }
                            }
                        }
                        else if (orp.DbType == System.Data.DbType.VarNumeric)
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                //continue;
                            }
                            else
                                paramvalue = 0;
                        }
                        else
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                paramvalue = GlobalVariable.TO_STRING(paramvalue.ToString());
                            }
                        }

                    }
                    param[i] = new OracleParameter(orp.ParameterName, paramvalue);
                    //orclParams.Append(orp.ParameterName).Append(": ").Append(_curTab.GetValue(list[i]));
                    //if (i < comm.Parameters.Count - 1)
                    //    orclParams.Append(", ");
                    i++;
                }

                //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
                int res = VAdvantage.SqlExec.Oracle.OracleHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, procedureName, param);
                //DataBase.DB.ExecuteQuery(sql, null);
            }
            catch (Exception e)
            {
                VLogger.Get().SaveError(e.Message, e);
                //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
                return false;

            }
            //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
            return true;
        }

#pragma warning restore 612, 618

        private byte[] StreamFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);

            // Create a byte array of file stream length
            byte[] ImageData = new byte[fs.Length];

            //Read block of bytes from stream into the byte array
            fs.Read(ImageData, 0, System.Convert.ToInt32(fs.Length));

            //Close the File Stream
            fs.Close();
            return ImageData; //return the byte data
        }


        public byte[] GetReportBytes()
        {
            return GenerateCrystalReport();
        }

        public String GenerateCrystalFilePath(bool fetchBytes, byte[] bytes)
        {

            string reportPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Reports");
            string reportImagePath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "CReports\\Images");



            //string _ReportImagePath = "";
            //string _ReportPath = "";

            string path = reportPath;

            //_ReportImagePath = reportImagePath;

            if (String.IsNullOrEmpty(path))
            {
                throw new MissingFieldException("CrystalReportPathNotSet");

            }
            if (reportName.IndexOf(":") < 0)
                reportPath = path + "\\" + reportName;
            else
                reportPath = reportName;


            ReportDocument rptBurndown = new ReportDocument();
            if (File.Exists(reportPath))   //Check if the crystal report file exists in a specified location.
            {
                try
                {
                    rptBurndown.Load(reportPath);

                    //Set Connection Info
                    ConnectionInfo.Get().SetAttributes(System.Configuration.ConfigurationManager.AppSettings["oracleConnectionString"]);


                    //Application will pick database info from the property file.
                    CrystalDecisions.Shared.ConnectionInfo crDbConnection = new CrystalDecisions.Shared.ConnectionInfo();
                    crDbConnection.IntegratedSecurity = false;
                    crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                    crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                    crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                    //crDbConnection.Type = ConnectionInfoType.Unknown;
                    crDbConnection.ServerName = ConnectionInfo.Get().Db_host;
                    CrystalDecisions.CrystalReports.Engine.Database crDatabase = rptBurndown.Database;
                    CrystalDecisions.Shared.TableLogOnInfo oCrTableLoginInfo;
                    foreach (CrystalDecisions.CrystalReports.Engine.Table oCrTable in crDatabase.Tables)
                    {
                        crDbConnection.IntegratedSecurity = false;
                        crDbConnection.DatabaseName = ConnectionInfo.Get().Db_name;
                        crDbConnection.UserID = ConnectionInfo.Get().Db_uid;
                        crDbConnection.Password = ConnectionInfo.Get().Db_pwd;
                        //crDbConnection.Type = ConnectionInfoType.Unknown;
                        crDbConnection.ServerName = ConnectionInfo.Get().Db_host;

                        oCrTableLoginInfo = oCrTable.LogOnInfo;
                        oCrTableLoginInfo.ConnectionInfo = crDbConnection;
                        oCrTable.ApplyLogOnInfo(oCrTableLoginInfo);
                    }

                    //Create Parameter query
                    string sql = SqlQuery;
                    StringBuilder sb = new StringBuilder(" WHERE ");

                    if (_pi.GetRecord_ID() > 0 && _pi.GetTable_ID() > 0)
                    {
                        string tableName = DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_TABLE_ID =" + _pi.GetTable_ID()).ToString();
                        sb.Append(tableName).Append("_ID = ").Append(_pi.GetRecord_ID());
                    }

                    else
                    {

                        ProcessInfoUtil.SetParameterFromDB(_pi);
                        ProcessInfoParameter[] parameters = _pi.GetParameter();
                        if (parameters.Count() > 0)
                        {
                            int loopCount = 0;
                            for (int para = 0; para <= parameters.Count() - 1; para++)
                            {
                                string sInfo = parameters[para].GetInfo();
                                string sInfoTo = parameters[para].GetInfo_To();
                                if ((String.IsNullOrEmpty(sInfo) && String.IsNullOrEmpty(sInfoTo)) || sInfo == "NULL")
                                {
                                    continue;
                                }

                                if (loopCount > 0)
                                    sb.Append(" AND ");
                                string paramName = parameters[para].GetParameterName();
                                object paramValue = parameters[para].GetParameter();
                                object paramValueTo = parameters[para].GetParameter_To();

                                if (paramValue is DateTime)
                                {
                                    sb.Append(paramName).Append(" BETWEEN ").Append(GlobalVariable.TO_DATE((DateTime)paramValue, true));
                                    if (paramValueTo != null && paramValueTo.ToString() != String.Empty)
                                        sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValueTo).AddDays(1), true));
                                    else
                                        sb.Append(" AND ").Append(GlobalVariable.TO_DATE(((DateTime)paramValue).AddDays(1), true));

                                }
                                else if (paramValue != null && paramValue.ToString().Contains(','))
                                {
                                    sb.Append(paramName).Append(" IN (")
                                        .Append(paramValue.ToString()).Append(")");
                                }
                                else
                                {
                                    sb.Append("Upper(").Append(paramName).Append(")").Append(" = Upper(")
                                        .Append(GlobalVariable.TO_STRING(paramValue.ToString()) + ")");
                                }

                                loopCount++;
                            }
                        }
                    }

                    if (sb.Length > 7)
                        sql = sql + sb.ToString();

                    //if (form.IsIncludeProcedure())
                    //{
                    //    bool result = StartDBProcess(form.GetProcedureName(), parameters);
                    //}

                    DataSet ds = DB.ExecuteDataset(sql);

                    if (ds == null)
                    {
                        ValueNamePair error = VLogger.RetrieveError();
                        throw new Exception(error.GetValue() + "BlankReportWillOpen");
                    }

                    bool imageError = false;
                    if (isIncludeImage)
                    {
                        for (int i_img = 0; i_img <= ds.Tables[0].Rows.Count - 1; i_img++)
                        {
                            String ImagePath = "";
                            String ImageField = "";
                            if (ds.Tables[0].Rows[i_img][imagePathField] != null)
                            {
                                ImagePath = ds.Tables[0].Rows[i_img][imagePathField].ToString();
                                ImageField = imageField;

                                if (ds.Tables[0].Columns.Contains(ImageField))
                                {
                                    if (File.Exists(reportImagePath + "\\" + ImagePath))
                                    {
                                        byte[] b = StreamFile(reportImagePath + "\\" + ImagePath);
                                        ds.Tables[0].Rows[i_img][ImageField] = b;
                                    }
                                    else
                                    {
                                        //ds.Tables[0].Rows.RemoveAt(i_img);
                                        imageError = true;
                                    }
                                }
                                else
                                {
                                    imageError = true;
                                }
                            }
                            else
                            {
                                imageError = true;
                            }
                        }
                    }

                    if (imageError)
                    {
                        //   ShowMessage.Error("ErrorLoadingSomeImages", true);
                    }

                    //crystalReportViewer1.ReportSource = rptBurndown;
                    //crystalReportViewer1.Refresh();

                    System.IO.Stream oStream;

                    rptBurndown.SetDataSource(ds.Tables[0]);                //By karan approveed by lokesh......

                    if (fetchBytes)
                    {
                        //rptBurndown.PrintOptions.ApplyPageMargins(new CrystalDecisions.Shared.PageMargins(100, 360, 100, 360));
                        oStream = rptBurndown.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                        bytes = new byte[oStream.Length];
                        oStream.Read(bytes, 0, Convert.ToInt32(oStream.Length));
                        oStream.Close();
                    }


                    string FILE_PATH = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";

                    if (!Directory.Exists(FILE_PATH))
                        Directory.CreateDirectory(FILE_PATH);

                    string filePath = FILE_PATH + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".pdf";

                    rptBurndown.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);

                    return filePath.Substring(filePath.IndexOf("TempDownload"));

                }
                catch (Exception ex)
                {
                    throw ex;

                }
            }
            else
            {
                throw new MissingFieldException("CouldNotFindTheCrystalReport");
            }
        }

        public string GetReportString()
        {
            return null;
        }
        public String GetReportFilePath(bool fetchBytes, out byte[] bytes)
        {
            bytes = null;
            return GenerateCrystalFilePath(fetchBytes, bytes);
        }

        public string GetCsvReportFilePath(string data)
        {
            return null;
        }
    }
}
