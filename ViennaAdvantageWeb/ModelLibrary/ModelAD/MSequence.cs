using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data.OracleClient;
using Npgsql;
using MySql.Data.MySqlClient;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Threading;
using System.Runtime.CompilerServices;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MSequence : X_AD_Sequence
    {
        /**	Sequence for Table Document No's	*/
        private static String PREFIX_DOCSEQ = "DocumentNo_";
        /**	Start Number			*/
        public static int INIT_NO = 1000000;	//	1 Mio
        /**	Start System Number		*/
        public static int INIT_SYS_NO = 100;
        static VConnection vConn = new VConnection();

        private static VLogger s_log = VLogger.GetVLogger(typeof(MSequence).FullName);

        public MSequence(Ctx ctx, int AD_Sequence_ID, Trx trxName)
            : base(ctx, AD_Sequence_ID, trxName)
        {

            if (AD_Sequence_ID == 0)
            {
                //	setName (null);
                //
                SetIsTableID(false);
                SetStartNo(INIT_NO);
                SetCurrentNext(INIT_NO);
                SetCurrentNextSys(INIT_SYS_NO);
                SetIncrementNo(1);
                SetIsAutoSequence(true);
                SetIsAudited(false);
                SetStartNewYear(false);
            }
        }	//	MSequence

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MSequence(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }
        public MSequence(Ctx ctx, IDataReader idr, Trx trxName)
            : base(ctx, idr, trxName)
        {

        }

        /**
         * 	New Document Sequence Constructor
         *	@param ctx context
         *	@param AD_Client_ID owner
         *	@param tableName name
         *	@param trxName transaction
         */
        public MSequence(Ctx ctx, int AD_Client_ID, String tableName, Trx trxName)
            : this(ctx, 0, trxName)
        {
            SetClientOrg(AD_Client_ID, 0);			//	Client Ownership
            SetName(PREFIX_DOCSEQ + tableName);
            SetDescription("DocumentNo/Value for Table " + tableName);
        }	//	MSequence;

        /**
         * 	New Document Sequence Constructor
         *	@param ctx context
         *	@param AD_Client_ID owner
         *	@param sequenceName name
         *	@param StartNo start
         *	@param trxName trx
         */
        public MSequence(Ctx ctx, int AD_Client_ID, String sequenceName, int StartNo, Trx trxName)
            : this(ctx, 0, trxName)
        {

            SetClientOrg(AD_Client_ID, 0);			//	Client Ownership
            SetName(sequenceName);
            SetDescription(sequenceName);
            SetStartNo(StartNo);
            SetCurrentNext(StartNo);
            SetCurrentNextSys(StartNo / 10);
        }	//	MSequence;

        public static int GetNextID(int AD_Client_ID, String TableName, Trx txtName)
        {
            if (DatabaseType.IsMSSql)
                return GetNextIDMSSql(AD_Client_ID, TableName);
            else if (DatabaseType.IsMySql)
                return GetNextIDMySql(AD_Client_ID, TableName);
            else if (DatabaseType.IsOracle)
                return GetNextIDOracle(AD_Client_ID, TableName, txtName);
            else if (DatabaseType.IsPostgre)
                return GetNextIDPostgre(AD_Client_ID, TableName, txtName);
            else
                return -1;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int GetNextIDOracle(int AD_Client_ID, String TableName, Trx trxName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check ViennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            String updateSQL = "UPDATE AD_Sequence SET  CurrentNext = @CurrentNext@, CurrentNextSys = @CurrentNextSys@ "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' ";

            //Comment an UPDATE FOR TEST//
            Trx trx = Trx.Get("ConnSH" + DateTime.Now.Ticks + (new Random(4)).ToString());

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds = DB.ExecuteDataset(selectSQL, null, trx);

                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = (retValue + incrementNo).ToString();
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = (retValue + incrementNo).ToString();
                        }
                        updateSQL = updateSQL.Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString()).Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());

                        if (DataBase.DB.ExecuteQuery(updateSQL, null, trx) < 0)
                        {
                            retValue = -1;
                        }
                    }
                    if (retValue == -1)
                        trx.Rollback();
                    else
                    {
                        if (trx != null)
                            trx.Commit();
                    }

                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    if (trx != null)
                    {
                        trx.Rollback();
                    }
                    s_log.Severe(e.ToString());
                }
                finally
                {
                    trx.Close();
                }
            }	//	loop

            return retValue;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int GetNextExportID(int AD_Client_ID, String TableName, Trx trxName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check ViennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT Export_ID, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            trxName = null;
            //SqlParameter[] param = new SqlParameter[1];
            Trx trx = trxName;
            IDbConnection conn = null;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (trx != null)
                        conn = trx.GetConnection();
                    else
                        conn = DB.GetConnection();
                    //	Error
                    if (conn == null)
                        return -1;
#pragma warning disable 612, 618
                    DataSet ds = new DataSet();
                    //OracleDataAdapter da = new OracleDataAdapter(selectSQL, new OracleConnection(Ini.CreateConnectionString()));
                    OracleDataAdapter da = new OracleDataAdapter(selectSQL, (OracleConnection)conn);
                    OracleCommandBuilder bld = new OracleCommandBuilder(da);
                    da.Fill(ds);
                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        //	int AD_Sequence_ID = dr.getInt(4);
                        //
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = retValue + incrementNo;
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["Export_ID"].ToString());
                            ds.Tables[0].Rows[0]["Export_ID"] = retValue + incrementNo;
                        }
                        da.Update(ds);
                    }

                    conn = null;
#pragma warning disable 612, 618
                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    s_log.Severe(e.ToString());
                }
                conn = null;
            }	//	loop

            return retValue;
        }

        public static int GetNextIDPostgre(int AD_Client_ID, String TableName, Trx trxName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check ViennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;
            // string db_Name = "vienna";

            //if (DataBase.DB.UseMigratedConnection)
            // {
            // db_Name = WindowMigration.MDialog.GetMConnection().Db_name;
            //}
            // else
            // {
            // db_Name = VConnection.Get().Db_name;
            //  }

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            String updateSQL = "UPDATE AD_Sequence SET  CurrentNext = @CurrentNext@, CurrentNextSys = @CurrentNextSys@ "
                + "WHERE Upper(Name)=Upper('" + TableName + "')"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' ";

            //Comment an UPDATE FOR TEST//
            Trx trx = Trx.Get("ConnSH" + DateTime.Now.Ticks + (new Random(4)).ToString());

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    ds = DB.ExecuteDataset(selectSQL, null, trx);

                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = (retValue + incrementNo).ToString();
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = (retValue + incrementNo).ToString();
                        }
                        updateSQL = updateSQL.Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString()).Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());

                        if (DataBase.DB.ExecuteQuery(updateSQL, null, trx) < 0)
                        {
                            retValue = -1;
                        }
                    }
                    if (retValue == -1)
                        trx.Rollback();
                    else
                    {
                        if (trx != null)
                            trx.Commit();
                    }

                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    if (trx != null)
                    {
                        trx.Rollback();
                    }
                    s_log.Severe(e.ToString());
                }
                finally
                {
                    trx.Close();
                }
            }	//	loop

            return retValue;
        }


        public static int GetNextIDMySql(int AD_Client_ID, String TableName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check viennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Name='" + TableName + "'"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            //SqlParameter[] param = new SqlParameter[1];

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    MySqlDataAdapter da = new MySqlDataAdapter(selectSQL, new MySqlConnection(DB.GetConnectionString()));
                    MySqlCommandBuilder bld = new MySqlCommandBuilder(da);
                    da.Fill(ds);
                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        //	int AD_Sequence_ID = dr.getInt(4);
                        //
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = retValue + incrementNo;
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = retValue + incrementNo;
                        }
                        da.Update(ds);
                    }
                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    s_log.Severe(e.ToString());
                }
            }	//	loop

            return retValue;
        }

        public static int GetNextIDMSSql(int AD_Client_ID, String TableName)
        {
            if (TableName == null || TableName.Length == 0)
                throw new ArgumentException("TableName missing");
            int retValue = -1;

            //	Check viennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Name='" + TableName + "'"
                + " AND IsActive='Y' AND IsTableID='Y' AND IsAutoSequence='Y' "
                + " FOR UPDATE";

            //SqlParameter[] param = new SqlParameter[1];

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(selectSQL, new SqlConnection(DB.GetConnectionString()));
                    SqlCommandBuilder bld = new SqlCommandBuilder(da);

                    da.Fill(ds);
                    for (int irow = 0; irow <= ds.Tables[0].Rows.Count - 1; irow++)
                    {
                        //	int AD_Sequence_ID = dr.getInt(4);
                        //
                        int incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                        if (viennaSys)
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNextSys"] = retValue + incrementNo;
                        }
                        else
                        {
                            retValue = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                            ds.Tables[0].Rows[0]["CurrentNext"] = retValue + incrementNo;
                        }
                        da.Update(ds);
                    }
                    break;		//	EXIT
                }
                catch (Exception e)
                {
                    s_log.Severe(e.ToString());
                }
            }	//	loop

            return retValue;
        }

        public int GetNextID()
        {
            int retValue = GetCurrentNext();
            SetCurrentNext(retValue + GetIncrementNo());
            return retValue;
        }

        /// <summary>
        /// Get Document No from table
        /// </summary>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        /// 

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxIn, Ctx ctx)
        {
            if (TableName == null || TableName.Length == 0)
            {
                throw new ArgumentException("TableName missing");
            }

            //	Check ViennaSys
            bool viennaSys = false;
            if (viennaSys && AD_Client_ID > 11)
                viennaSys = false;

            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
                + " AND AD_Client_ID = " + AD_Client_ID
                + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
            if (VAdvantage.DataBase.DatabaseType.IsOracle)
            {
                selectSQL += " ORDER BY AD_Client_ID DESC ";
            }
            selectSQL += "FOR UPDATE";


            String updateSQL = "UPDATE AD_Sequence SET  CurrentNext =@CurrentNext@ , CurrentNextSys = @CurrentNextSys@ WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
                + " AND AD_Client_ID = " + AD_Client_ID
                + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

            Trx trx = null;

            if (trxIn == null || !trxIn.UseSameTrxForDocNo)
            {
                trx = Trx.Get("ConnDH" + DateTime.Now.Ticks + (new Random(4)).ToString());
            }
            else
            {
                trx = trxIn;
            }

            //
            int incrementNo = 0;
            int next = -1;
            String prefix = "";
            String suffix = "";
            try
            {
                DataSet ds = new DataSet();
                ds = DB.ExecuteDataset(selectSQL, null, trx);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
                    suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
                    incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());

                    if (viennaSys)
                    {
                        next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                        ds.Tables[0].Rows[0]["CurrentNextSys"] = (next + incrementNo).ToString();
                    }
                    else
                    {
                        next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                        ds.Tables[0].Rows[0]["CurrentNext"] = (next + incrementNo).ToString();
                    }

                    updateSQL = updateSQL.Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString()).Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString());

                    if (DB.ExecuteQuery(updateSQL, null, trx) < 0)
                    {
                        next = -2;
                    }

                    if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                    {
                        if (next == -2)
                        {
                            if (trx != null)
                                trx.Rollback();
                        }
                        else
                        {
                            if (trx != null)
                                trx.Commit();
                        }
                    }
                }
                else
                {
                    MSequence seq = new MSequence(ctx, AD_Client_ID, TableName, trx);
                    next = seq.GetNextID();


                    if (seq.Save(trx))
                    {
                        if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                        {
                            if (trx != null)
                                trx.Commit();
                        }
                    }
                    else
                    {
                        if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                        {
                            if (trx != null)
                                trx.Rollback();
                        }
                    }

                }
            }
            catch
            {
                next = -2;
                if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                {
                    if (trx != null)
                    {
                        trx.Rollback();
                    }
                }
            }
            finally
            {
                if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                {
                    if (trx != null)
                        trx.Close();
                }
            }

            if (next < 0)
                return null;

            //	create DocumentNo
            StringBuilder doc = new StringBuilder();
            if (prefix != null && prefix.Length > 0)
                doc.Append(prefix);
            doc.Append(next);
            if (suffix != null && suffix.Length > 0)
                doc.Append(suffix);
            String documentNo = doc.ToString();

            return documentNo;
        }





        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName,Ctx ctx)
        //{
        //    if (TableName == null || TableName.Length == 0)
        //    {
        //        throw new ArgumentException("TableName missing");
        //    }

        //    //	Check ViennaSys
        //    bool viennaSys = false;
        //    if (viennaSys && AD_Client_ID > 11)
        //        viennaSys = false;

        //    String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Sequence_ID "
        //        + "FROM AD_Sequence "
        //        + "WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
        //        + " AND AD_Client_ID = " + AD_Client_ID
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
        //    if (VAdvantage.DataBase.DatabaseType.IsOracle)
        //    {
        //        selectSQL += " ORDER BY AD_Client_ID DESC ";
        //    }
        //    selectSQL += "FOR UPDATE";


        //    String updateSQL = "UPDATE AD_Sequence SET  CurrentNext =@CurrentNext@ , CurrentNextSys = @CurrentNextSys@ WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
        //        + " AND AD_Client_ID = " + AD_Client_ID
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

        //    Trx trx = Trx.GetTrx("Conn" + DateTime.Now.Ticks + (new Random(4)).ToString());

        //    //
        //    int incrementNo = 0;
        //    int next = -1;
        //    String prefix = "";
        //    String suffix = "";
        //    try
        //    {
        //        //if (DatabaseType.IsOracle)
        //        //{
        //        //}
        //        //else if (DatabaseType.IsPostgre)
        //        //{

        //        //}
        //        //else if (DatabaseType.IsMSSql)
        //        //{
        //        //}
        //        //else if (DatabaseType.IsMySql)
        //        //{
        //        //}

        //        DataSet ds = new DataSet();
        //        ds = DB.ExecuteDataset(selectSQL, null, trx);

        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
        //            suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
        //            incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());

        //            if (viennaSys)
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNextSys"] = (next + incrementNo).ToString();
        //            }
        //            else
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNext"] = (next + incrementNo).ToString();
        //            }

        //            updateSQL = updateSQL.Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString()).Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString());

        //            if (DB.ExecuteQuery(updateSQL, null, trx) < 0)
        //            {
        //                next = -2;
        //            }


        //            if (next == -2)
        //                trx.Rollback();
        //            else
        //            {
        //                if (trx != null)
        //                    trx.Commit();
        //            }

        //        }
        //        else
        //        {
        //            MSequence seq = new MSequence(ctx, AD_Client_ID, TableName, trx);
        //            next = seq.GetNextID();

        //            if ((Util.GetValueOfInt(ctx.GetContext("#AD_Client_ID")) == 0)
        //                && ((Util.GetValueOfInt(ctx.GetContext("#AD_Client_ID")) == 0)
        //                && (Util.GetValueOfInt(ctx.GetContext("#AD_Client_ID")) != AD_Client_ID)))
        //            {
        //                trx.Commit();
        //            }
        //            else
        //            {
        //                if (seq.Save(trx))
        //                {
        //                    trx.Commit();
        //                }
        //                else
        //                {
        //                    trx.Close();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        next = -2;
        //        if (trx != null)
        //        {
        //            trx.Rollback();
        //        }
        //    }
        //    finally
        //    {
        //        trx.Close();
        //    }

        //    if (next < 0)
        //        return null;

        //    //	create DocumentNo
        //    StringBuilder doc = new StringBuilder();
        //    if (prefix != null && prefix.Length > 0)
        //        doc.Append(prefix);
        //    doc.Append(next);
        //    if (suffix != null && suffix.Length > 0)
        //        doc.Append(suffix);
        //    String documentNo = doc.ToString();

        //    return documentNo;
        //}


        ///// <summary>
        ///// Get Document No from table
        ///// </summary>
        ///// <param name="AD_Client_ID">client</param>
        ///// <param name="TableName">table name</param>
        ///// <param name="trxName">optional Transaction Name</param>
        ///// <returns>document no or null</returns>
        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static String GetDocumentNo(int AD_Client_ID, String TableName, Trx trxName)
        //{
        //    if (TableName == null || TableName.Length == 0)
        //    {
        //        throw new ArgumentException("TableName missing");
        //    }

        //    //	Check viennaSys
        //    bool viennaSys = false;
        //    if (viennaSys && AD_Client_ID > 11)
        //        viennaSys = false;
        //    //if (CLogMgt.isLevel(LOGLEVEL))
        //      //  _log.Log(LOGLEVEL, TableName + " - ViennaSys=" + ViennaSys + " [" + trxName + "]");

        //    String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Sequence_ID "
        //        + "FROM AD_Sequence "
        //        + "WHERE Name='" + PREFIX_DOCSEQ + TableName + "'"
        //        //jz fix duplicated nextID  + " AND AD_Client_ID IN (0,?)"
        //        + " AND AD_Client_ID = " + AD_Client_ID
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
        //    if (VAdvantage.DataBase.DatabaseType.IsOracle)
        //    {
        //        selectSQL += " ORDER BY AD_Client_ID DESC ";
        //    }
        //  //  selectSQL += "FOR UPDATE";


        //    //
        //    int incrementNo = 0;
        //    int next = -1;
        //    String prefix = "";
        //    String suffix = "";
        //    try
        //    {
        //        //
        //        //IDataAdapter dap = null;
        //        //DataSet ds1 = DataBase.DB.ExecuteDataset(selectSQL,null,null);
        //        //dap.Fill(ds);

        //        OracleCommandBuilder ocb = null;
        //        MySqlCommandBuilder mscb = null;
        //        SqlCommandBuilder scb = null;
        //        NpgsqlCommandBuilder ncb = null;

        //        IDataAdapter dap = null;
        //        //check 

        //        if (DatabaseType.IsOracle)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            ocb = new OracleCommandBuilder((OracleDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsPostgre)
        //        {

        //            string strDB1 = vConn.Db_name;
        //            //string strDB = Ini.GetProperty(GlobalVariable.DBNAME_NODE);
        //            string finalSQL = "set search_path to " + strDB1 + ", public;" + selectSQL;
        //            dap = GetDataAdapter(finalSQL);
        //            ncb = new NpgsqlCommandBuilder((NpgsqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMSSql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            scb = new SqlCommandBuilder((SqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMySql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            mscb = new MySqlCommandBuilder((MySqlDataAdapter)dap);
        //        }

        //        DataSet ds = new DataSet();
        //        dap.Fill(ds);

        //       // _log.Fine("AC=" + conn.getAutoCommit() + " -Iso=" + conn.getTransactionIsolation() 
        //        //		+ " - Type=" + pstmt.getResultSetType() + " - Concur=" + pstmt.getResultSetConcurrency());
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {
        //            prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
        //            suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
        //            incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());

        //            if (viennaSys)
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNextSys"] = next + incrementNo;
        //            }
        //            else
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNext"] = next + incrementNo;
        //            }
        //            dap.Update(ds);

        //        }
        //        else
        //        {
        //            _log.Warning("(Table) - no record found - " + TableName);
        //            MSequence seq = new MSequence(Utility.Env.GetContext(), AD_Client_ID, TableName, null);
        //            next = seq.GetNextID();
        //            seq.Save();
        //            //seq.saveNew();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Log(Level.SEVERE, "(Table) [" + trxName + "]", e);
        //        next = -2;
        //    }

        //    if (next < 0)
        //        return null;

        //    //	create DocumentNo
        //    StringBuilder doc = new StringBuilder();
        //    if (prefix != null && prefix.Length > 0)
        //        doc.Append(prefix);
        //    doc.Append(next);
        //    if (suffix != null && suffix.Length > 0)
        //        doc.Append(suffix);
        //    String documentNo = doc.ToString();

        //    return documentNo;
        //}
        //#pragma warning disable 612, 618
        //        private static IDataAdapter GetDataAdapter(string selectSQL)
        //        {
        //            if (DatabaseType.IsOracle)
        //                return new OracleDataAdapter(selectSQL, Ini.CreateConnectionString());
        //            else if (DatabaseType.IsPostgre)
        //                return new NpgsqlDataAdapter(selectSQL, Ini.CreateConnectionString());
        //            else if (DatabaseType.IsMSSql)
        //                return new SqlDataAdapter(selectSQL, Ini.CreateConnectionString());
        //            else if (DatabaseType.IsMySql)
        //                return new MySqlDataAdapter(selectSQL, Ini.CreateConnectionString());

        //            return null;
        //        }
        //#pragma warning restore 612, 618

        /// <summary>
        /// Get Document No based on Document Type
        /// </summary>
        /// <param name="C_DocType_ID">document type</param>
        /// <param name="trxName">optional Transaction Name</param>
        /// <returns>document no or null</returns>
        /// 
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static String GetDocumentNo(int C_DocType_ID, Trx trxIn, Ctx ctx)
        {
            if (C_DocType_ID == 0)
            {
                s_log.Severe("C_DocType_ID=0");
                return null;
            }
            MDocType dt = MDocType.Get(ctx, C_DocType_ID);	//	wrong for SERVER, but r/o
            if (dt != null && !dt.IsDocNoControlled())
            {
                s_log.Finer("DocType_ID=" + C_DocType_ID + " Not DocNo controlled");
                return null;
            }
            if (dt == null || dt.GetDocNoSequence_ID() == 0)
            {
                s_log.Warning("No Sequence for DocType - " + dt);
                return null;
            }

            //	Check ViennaSys
            Boolean viennaSys = false; // Ini.IsPropertyBool(Ini._VIENNASYS);
            //if (CLogMgt.isLevel(LOGLEVEL))
            //_log.Log(LOGLEVEL, "DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
            String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Client_ID, AD_Sequence_ID "
                + "FROM AD_Sequence "
                + "WHERE AD_Sequence_ID=" + dt.GetDocNoSequence_ID()
                + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

            selectSQL += "FOR UPDATE";

            String updateSQL = "UPDATE AD_Sequence SET CurrentNext = @CurrentNext@, CurrentNextSys=@CurrentNextSys@ "
               + " WHERE AD_Sequence_ID=" + dt.GetDocNoSequence_ID()
               + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";

            Trx trx = null;

            if (trxIn == null || !trxIn.UseSameTrxForDocNo)
            {
                trx = Trx.Get("ConnDDH" + DateTime.Now.Ticks + (new Random(4)).ToString());
            }
            else
            {
                trx = trxIn;
            }

            //	+ " FOR UPDATE";
            //
            int incrementNo = 0;
            int next = -1;
            String prefix = "";
            String suffix = "";
            try
            {

                DataSet ds = new DataSet();
                ds = DB.ExecuteDataset(selectSQL, null, trx);

                // _log.Fine("AC=" + conn.getAutoCommit() + " -Iso=" + conn.getTransactionIsolation() 
                //	+ " - Type=" + pstmt.getResultSetType() + " - Concur=" + pstmt.getResultSetConcurrency());

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
                    prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
                    suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
                    int AD_Client_ID = int.Parse(ds.Tables[0].Rows[0]["AD_Client_ID"].ToString());
                    if (viennaSys && AD_Client_ID > 11)
                        viennaSys = false;
                    //	AD_Sequence_ID = dr.getInt(7);
                    if (viennaSys)
                    {
                        next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
                        ds.Tables[0].Rows[0]["CurrentNextSys"] = (next + incrementNo).ToString();
                    }
                    else
                    {
                        next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
                        ds.Tables[0].Rows[0]["CurrentNext"] = (next + incrementNo).ToString();
                    }

                    updateSQL = updateSQL.Replace("@CurrentNextSys@", ds.Tables[0].Rows[0]["CurrentNextSys"].ToString()).Replace("@CurrentNext@", ds.Tables[0].Rows[0]["CurrentNext"].ToString());

                    if (DB.ExecuteQuery(updateSQL, null, trx) < 0)
                    {
                        next = -2;
                    }

                    if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                    {
                        if (next == -2)
                            trx.Rollback();
                        else
                        {
                            trx.Commit();
                        }
                    }
                }
                else
                {
                    s_log.Warning("(DocType)- no record found - " + dt);
                    next = -2;
                }
            }
            catch (Exception e)
            {
                s_log.Log(Level.SEVERE, "(DocType) [" + trx + "]", e);
                next = -2;
                if (trx != null)
                {
                    if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                    {
                        trx.Rollback();
                    }
                }
            }
            finally
            {
                if (trxIn == null || !trxIn.UseSameTrxForDocNo)
                {
                    trx.Close();
                }
            }
            //	Error
            if (next < 0)
                return null;

            //	create DocumentNo
            StringBuilder doc = new StringBuilder();
            if (prefix != null && prefix.Length > 0)
                doc.Append(prefix);
            doc.Append(next);
            if (suffix != null && suffix.Length > 0)
                doc.Append(suffix);
            String documentNo = doc.ToString();
            //_log.Finer(documentNo + " (" + incrementNo + ")" + " - C_DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
            return documentNo;
        }


        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public static String GetDocumentNo(int C_DocType_ID, Trx trxName,Ctx ctx)
        //{
        //    if (C_DocType_ID == 0)
        //    {
        //        _log.Severe("C_DocType_ID=0");
        //        return null;
        //    }
        //    MDocType dt = MDocType.Get(ctx, C_DocType_ID);	//	wrong for SERVER, but r/o
        //    if (dt != null && !dt.IsDocNoControlled())
        //    {
        //        _log.Finer("DocType_ID=" + C_DocType_ID + " Not DocNo controlled");
        //        return null;
        //    }
        //    if (dt == null || dt.GetDocNoSequence_ID() == 0)
        //    {
        //        _log.Warning("No Sequence for DocType - " + dt);
        //        return null;
        //    }

        //    //	Check ViennaSys
        //    Boolean viennaSys = false; // Ini.IsPropertyBool(Ini._ViennaSYS);
        //    //if (CLogMgt.isLevel(LOGLEVEL))
        //    //_log.Log(LOGLEVEL, "DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
        //    String selectSQL = "SELECT CurrentNext, CurrentNextSys, IncrementNo, Prefix, Suffix, AD_Client_ID, AD_Sequence_ID "
        //        + "FROM AD_Sequence "
        //        + "WHERE AD_Sequence_ID=" + dt.GetDocNoSequence_ID()
        //        + " AND IsActive='Y' AND IsTableID='N' AND IsAutoSequence='Y' ";
        //    //	+ " FOR UPDATE";
        //    //
        //    int incrementNo = 0;
        //    int next = -1;
        //    String prefix = "";
        //    String suffix = "";
        //    try
        //    {
        //        OracleCommandBuilder ocb = null;
        //        MySqlCommandBuilder mscb = null;
        //        SqlCommandBuilder scb = null;
        //        NpgsqlCommandBuilder ncb = null;

        //        IDataAdapter dap = null;


        //        if (DatabaseType.IsOracle)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            ocb = new OracleCommandBuilder((OracleDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsPostgre)
        //        {
        //            string strDB = vConn.Db_name;//            Ini.GetProperty(GlobalVariable.DBNAME_NODE);
        //            string finalSQL = "set search_path to " + strDB + ", public;" + selectSQL;
        //            dap = GetDataAdapter(finalSQL);
        //            ncb = new NpgsqlCommandBuilder((NpgsqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMSSql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            scb = new SqlCommandBuilder((SqlDataAdapter)dap);
        //        }
        //        else if (DatabaseType.IsMySql)
        //        {
        //            dap = GetDataAdapter(selectSQL);
        //            mscb = new MySqlCommandBuilder((MySqlDataAdapter)dap);
        //        }
        //        DataSet ds = new DataSet();
        //        dap.Fill(ds);

        //        // _log.Fine("AC=" + conn.getAutoCommit() + " -Iso=" + conn.getTransactionIsolation() 
        //        //	+ " - Type=" + pstmt.getResultSetType() + " - Concur=" + pstmt.getResultSetConcurrency());

        //        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        //        {
        //            incrementNo = int.Parse(ds.Tables[0].Rows[0]["IncrementNo"].ToString());
        //            prefix = ds.Tables[0].Rows[0]["Prefix"].ToString();
        //            suffix = ds.Tables[0].Rows[0]["Suffix"].ToString();
        //            int AD_Client_ID = int.Parse(ds.Tables[0].Rows[0]["AD_Client_ID"].ToString());
        //            if (viennaSys && AD_Client_ID > 11)
        //                viennaSys = false;
        //            //	AD_Sequence_ID = dr.getInt(7);
        //            if (viennaSys)
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNextSys"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNextSys"] = next + incrementNo;
        //            }
        //            else
        //            {
        //                next = int.Parse(ds.Tables[0].Rows[0]["CurrentNext"].ToString());
        //                ds.Tables[0].Rows[0]["CurrentNext"] = next + incrementNo;
        //            }
        //            dap.Update(ds);
        //        }
        //        else
        //        {
        //            _log.Warning("(DocType)- no record found - " + dt);
        //            next = -2;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        _log.Log(Level.SEVERE, "(DocType) [" + trxName + "]", e);
        //        next = -2;
        //    }
        //    //	Error
        //    if (next < 0)
        //        return null;

        //    //	create DocumentNo
        //    StringBuilder doc = new StringBuilder();
        //    if (prefix != null && prefix.Length > 0)
        //        doc.Append(prefix);
        //    doc.Append(next);
        //    if (suffix != null && suffix.Length > 0)
        //        doc.Append(suffix);
        //    String documentNo = doc.ToString();
        //    _log.Finer(documentNo + " (" + incrementNo + ")" + " - C_DocType_ID=" + C_DocType_ID + " [" + trxName + "]");
        //    return documentNo;
        //}


        /// <summary>
        /// Create Table ID Sequence 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if created</returns>
        public static Boolean CreateTableSequence(Ctx ctx, String TableName, Trx trxName)
        {
            MSequence seq = new MSequence(ctx, 0, trxName);
            seq.SetClientOrg(0, 0);
            seq.SetName(TableName);
            seq.SetDescription("Table " + TableName);
            seq.SetIsTableID(true);
            return seq.Save();
        }	//	createTableSequence



        /// <summary>
        /// Delete Table ID Sequence
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="TableName">table name</param>
        /// <param name="trxName">transaction</param>
        /// <returns>true if </returns>
        public static Boolean DeleteTableSequence(Ctx ctx, String TableName, Trx trxName)
        {
            MSequence seq = Get(ctx, TableName, trxName);
            return seq.Delete(true);
        }	//	deleteTableSequence



        /// <summary>
        /// Get Table Sequence
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="tableName">table name</param>
        /// <param name="trxName"></param>
        /// <returns>Sequence</returns>
        public static MSequence Get(Ctx ctx, String tableName, Trx trxName)
        {
            String sql = "SELECT * FROM AD_Sequence "
                + "WHERE UPPER(Name)='" + tableName.ToUpper() + "'"
                + " AND IsTableID='Y'";
            MSequence retValue = null;
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                DataRow dr = null;
                int totalCount = dt.Rows.Count;
                for (int i = 0; i < totalCount; i++)
                {
                    dr = dt.Rows[i];
                    retValue = new MSequence(ctx, dr, null);
                    if (i > 0)
                    {
                        s_log.Log(Level.SEVERE, "More then one sequence for " + tableName);
                    }
                }
                dr = null;

            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }


            return retValue;
        }	//	get



        /// <summary>
        /// Check/Initialize Client DocumentNo/Value Sequences 	
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_Client_ID">client</param>
        /// <param name="trxName"></param>
        /// <returns>true if no error</returns>
        public static Boolean CheckClientSequences(Ctx ctx, int AD_Client_ID, Trx trxName)
        {
            String sql = "SELECT TableName "
                + "FROM AD_Table t "
                + "WHERE IsActive='Y' AND IsView='N'"
                //	Get all Tables with DocumentNo or Value
                + " AND AD_Table_ID IN "
                    + "(SELECT AD_Table_ID FROM AD_Column "
                    + "WHERE ColumnName = 'DocumentNo' OR ColumnName = 'Value')"
                //	Ability to run multiple times
                + " AND 'DocumentNo_' || TableName NOT IN "
                    + "(SELECT Name FROM AD_Sequence s "
                    + "WHERE s.AD_Client_ID=@AD_Client_ID)";
            int counter = 0;
            Boolean success = true;
            //
            DataTable dt = null;

            SqlParameter[] param = new SqlParameter[1];
            param[0] = new SqlParameter("@AD_Client_ID", AD_Client_ID);
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, param);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();

                int totCount = dt.Rows.Count;

                for (int i = 0; i < totCount; i++)
                {
                    DataRow dr = dt.Rows[i];

                    String tableName = dr["TableName"].ToString();
                    s_log.Fine("Add: " + tableName);
                    MSequence seq = new MSequence(ctx, AD_Client_ID, tableName, trxName);
                    if (seq.Save())
                    {
                        counter++;
                    }
                    else
                    {
                        s_log.Severe("Not created - AD_Client_ID=" + AD_Client_ID
                            + " - " + tableName);
                        success = false;
                    }

                }


            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                s_log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                dt = null;
            }
            s_log.Info("AD_Client_ID=" + AD_Client_ID
                + " - created #" + counter
                + " - success=" + success);
            return success;
        }	//	checkClientSequences

        /// <summary>
        /// Get next DocumentNo
        /// </summary>
        /// <returns>document no</returns>
        public String GetDocumentNo()
        {
            //	create DocumentNo
            StringBuilder doc = new StringBuilder();
            String prefix = GetPrefix();
            if (prefix != null && prefix.Length > 0)
                doc.Append(prefix);
            doc.Append(GetNextID());
            String suffix = GetSuffix();
            if (suffix != null && suffix.Length > 0)
                doc.Append(suffix);
            return doc.ToString();
        }	//	getDocumentNo

        /// <summary>
        /// 	Validate Table Sequence Values
        /// </summary>
        /// <returns>true if updated</returns>
        public bool ValidateTableIDValue()
        {
            if (!IsTableID())
            {
                return false;
            }
            String tableName = GetName();
            int AD_Column_ID = DataBase.DB.GetSQLValue(null, "SELECT MAX(c.AD_Column_ID) "
                + "FROM AD_Table t"
                + " INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID) "
                + "WHERE t.TableName='" + tableName + "'"
                + " AND c.ColumnName='" + tableName + "_ID'");
            if (AD_Column_ID <= 0)
            {
                return false;
            }
            //
            MSystem system = MSystem.Get(GetCtx());
            int IDRangeEnd = 0;
            //if (system.GetIDRangeEnd() != null)
            {
                IDRangeEnd = Utility.Util.GetValueOfInt(system.GetIDRangeEnd());//.intValue();
            }
            bool change = false;
            String info = null;

            //	Current Next
            String sql = "SELECT MAX(" + tableName + "_ID) FROM " + tableName;
            if (IDRangeEnd > 0)
            {
                sql += " WHERE " + tableName + "_ID < " + IDRangeEnd;
            }
            int maxTableID = DataBase.DB.GetSQLValue(null, sql);
            if (maxTableID < INIT_NO)
            {
                maxTableID = INIT_NO - 1;
            }
            maxTableID++;		//	Next
            if (GetCurrentNext() < maxTableID)
            {
                SetCurrentNext(maxTableID);
                info = "CurrentNext=" + maxTableID;
                change = true;
            }

            //	Get Max System_ID used in Table
            sql = "SELECT MAX(" + tableName + "_ID) FROM " + tableName
                + " WHERE " + tableName + "_ID < " + INIT_NO;
            int maxTableSysID = DataBase.DB.GetSQLValue(null, sql);
            if (maxTableSysID <= 0)
            {
                maxTableSysID = INIT_SYS_NO - 1;
            }
            maxTableSysID++;	//	Next
            if (GetCurrentNextSys() < maxTableSysID)
            {
                SetCurrentNextSys(maxTableSysID);
                if (info == null)
                {
                    info = "CurrentNextSys=" + maxTableSysID;
                }
                else
                {
                    info += " - CurrentNextSys=" + maxTableSysID;
                }
                change = true;
            }
            if (info != null)
            {
                log.Config(GetName() + " - " + info);
            }
            return change;
        }	//	validate

    }

}
