; (function (VIS, $) {

    function ProcessInfo(strTitle, AD_Process_ID, Table_ID, Record_ID) {
        this.title = strTitle;
        this.AD_Process_ID = AD_Process_ID;
        this.table_ID = Table_ID;
        this.tableName;
        this.record_ID = Record_ID;
        this.AD_User_ID;
        this.AD_Client_ID;
        this.className = null;

        this.AD_PInstance_ID = 0;
        this.summary = "";
        this.error = false;
        this.batch = false;
        //Process timed out		
        this.timeout = false;
        //private List<ProcessInfoLog> m_logs = null;
        this.parameter = null;
        this.AD_PrintFormat_Table_ID = 0;
        this.AD_PrintFormat_ID = 0;
        this.isReportFormat = false;
        this.isCrystal = false;
        this.totalrecords = 0;
    };

    ProcessInfo.prototype.toJson = function () {

        var o = {
            "Title": this.title,
            "Process_ID": this.AD_Process_ID,
            "AD_PInstance_ID": this.AD_PInstance_ID,
            "Record_ID": this.record_ID,
            "Error": this.error,
            "Summary": this.getSummary(),
            "ClassName": this.className,

            "AD_Table_ID": this.table_ID,
            "AD_TableName": this.tableName,
            "AD_User_ID": this.AD_User_ID,
            "AD_Client_ID": this.AD_Client_ID,
            "Batch": this.batch,
            "TimeOut": this.timeout,
            "AD_PrintFormat_Table_ID": this.AD_PrintFormat_Table_ID,
            "AD_PrintFormat_ID": this.AD_PrintFormat_ID,
        }
        return o;

    };

    ProcessInfo.prototype.fromJson = function (o) {
        var info = null;
        if (this instanceof ProcessInfo) {
            this.title = o.Title;
            this.AD_Process_ID = o.Process_ID;
            this.className = o.ClassName;
            this.record_ID = o.Record_ID;
            this.error = o.Error;
            this.AD_PInstance_ID = o.AD_PInstance_ID;
            this.summary = o.Summary;

            this.table_ID = o.AD_Table_ID;
            this.tableName = o.AD_TableName;
            this.AD_User_ID = o.AD_User_ID;
            this.AD_Client_ID = o.AD_Client_ID;;
            this.batch = o.Batch;
            this.timeout = o.TimeOut;
            this.AD_PrintFormat_Table_ID = o.AD_PrintFormat_Table_ID;
            this.AD_PrintFormat_ID = o.AD_PrintFormat_ID;

            this.isCrystal = o.IsCrystal;
            this.isReportFormat = o.IsReportFormat;
            this.totalrecords = o.TotalRecords;

            info = this;
        }
        else {
            info = new ProcessInfo(o.Title, o.Process_ID);
            info.className = o.ClassName;
            info.record_ID = o.Record_ID;
            info.error = o.Error;
            info.AD_PInstance_ID = o.AD_PInstance_ID;
            info.summary = o.Summary;

            info.table_ID = o.AD_Table_ID;
            info.tableName = o.AD_TableName;
            info.AD_User_ID = o.AD_User_ID;
            info.AD_Client_ID = o.AD_Client_ID;;
            info.batch = o.Batch;
            info.timeout = o.TimeOut;
            info.AD_PrintFormat_Table_ID = o.AD_PrintFormat_Table_ID;
            info.AD_PrintFormat_ID = o.AD_PrintFormat_ID;

            info.isCrystal = o.IsCrystal;
            info.isReportFormat = o.IsReportFormat;
            info.totalrecords = Convert.ToInt32(TotalRecords);
        }
        return info;
    };

    ProcessInfo.prototype.setError = function (error) {
        this.error = error;
    };

    ProcessInfo.prototype.getIsError = function () {
        return this.error;
    };

    ProcessInfo.prototype.getIsBatch = function () {
        return this.batch;
    };

    ProcessInfo.prototype.setIsBatch = function (batch) {
        this.batch = batch;
    };

    ProcessInfo.prototype.getAD_PInstance_ID = function () {
        return this.AD_PInstance_ID;
    };

    ProcessInfo.prototype.setAD_PInstance_ID = function (AD_PInstance_ID) {
        this.AD_PInstance_ID = AD_PInstance_ID;
    };

    ProcessInfo.prototype.getAD_Process_ID = function () {
        return this.AD_Process_ID;
    };

    ProcessInfo.prototype.setAD_Process_ID = function (AD_Process_ID) {
        this.AD_Process_ID = AD_Process_ID;
    };

    ProcessInfo.prototype.getClassName = function () {
        return this.className;
    };

    ProcessInfo.prototype.setClassName = function (className) {
        this.className = className;
        if (className != null && className.length == 0)
            this.className = null;
    };

    ProcessInfo.prototype.getTable_ID = function () {
        return this.table_ID;
    };

    ProcessInfo.prototype.setTable_ID = function (AD_Table_ID) {
        this.table_ID = AD_Table_ID;
    };

    ProcessInfo.prototype.setTable_Name = function (tableName) {
        this.tableName = tableName;
    };

    ProcessInfo.prototype.getTable_Name = function () {
        return this.tableName;
    };

    ProcessInfo.prototype.getRecord_ID = function () {
        return this.record_ID;
    };

    ProcessInfo.prototype.setRecord_ID = function (Record_ID) {
        this.record_ID = Record_ID;
    };

    ProcessInfo.prototype.getTitle = function () {
        return this.title;
    };

    ProcessInfo.prototype.setTitle = function (title) {
        this.title = title;
    };

    ProcessInfo.prototype.setAD_Client_ID = function (AD_Client_ID) {
        this.AD_Client_ID = AD_Client_ID;
    };

    ProcessInfo.prototype.getAD_Client_ID = function () {
        return this.AD_Client_ID;
    };

    ProcessInfo.prototype.setAD_User_ID = function (AD_User_ID) {
        this.AD_User_ID = AD_User_ID;
    };

    ProcessInfo.prototype.getAD_User_ID = function () {
        return this.AD_User_ID;
    };

    ProcessInfo.prototype.getParameter = function () {
        return this.parameter;
    };

    ProcessInfo.prototype.setParameter = function (parameter) {
        this.parameter = parameter;
    };

    ProcessInfo.prototype.setIsTimeout = function (timeout) {
        this.timeout = timeout;
    };

    ProcessInfo.prototype.getIsTimeout = function () {
        return this.timeout;
    };

    ProcessInfo.prototype.set_AD_PrintFormat_Table_ID = function (AD_PrintFormat_Table_ID) {
        this.AD_PrintFormat_Table_ID = AD_PrintFormat_Table_ID;
    };

    ProcessInfo.prototype.get_AD_PrintFormat_Table_ID = function () {
        return this.AD_PrintFormat_Table_ID;
    };

    ProcessInfo.prototype.set_AD_PrintFormat_ID = function (AD_PrintFormat_ID) {
        this.AD_PrintFormat_ID = AD_PrintFormat_ID;
    };

    ProcessInfo.prototype.Get_AD_PrintFormat_ID = function () {
        return this.AD_PrintFormat_ID;
    };

    ProcessInfo.prototype.setIsCrystal = function (isCrystal) {
        this.isCrystal = isCrystal;
    };

    ProcessInfo.prototype.getIsCrystal = function () {
        return this.isCrystal;
    };

    ProcessInfo.prototype.setIsReportFormat = function (isRF) {
        this.isReportFormat = isRF;
    };

    ProcessInfo.prototype.getIsReportFormat = function () {
        return this.isReportFormat;
    };

    ProcessInfo.prototype.gGetTotalRecord = function () {
        return this.totalrecords;
    };

    ProcessInfo.prototype.setSummary = function (summary, error) {

        if (arguments.length == 2) {
            this.setError(error)
        }

        this.summary = summary;
    };

    ProcessInfo.prototype.getSummary = function () {
        return this.summary;
        //	return Util.cleanMnemonic(m_Summary);
    };

    /**************************************************************************
	 * 	Add to Log
	 *	@param Log_ID Log ID
	 *	@param P_ID Process ID
	 *	@param P_Date Process Date
	 *	@param P_Number Process Number
	 *	@param P_Msg Process Message
	 */
    ProcessInfo.prototype.addLog = function (Log_ID, P_ID, P_Date, P_Number, P_Msg) {
        return this.addLogEntry(new ProcessInfoLog(Log_ID, P_ID, P_Date, P_Number, P_Msg));
    };	//	

    /**
	 * 	Add to Log.
	 * 	Checks for duplicates;
	 *	@param logEntry log entry
	 */
    ProcessInfo.prototype.addLogEntry = function (logEntry) {
        if (logEntry == null)
            return null;
        if (this.logs == null)
            this.logs = [];
        //
        var newID = logEntry.getLog_ID();
        for (var i = 0; i < this.logs.length; i++) {
            var thisEntry = this.logs[i];
            var thisID = thisEntry.getLog_ID();
            if (newID == thisID)
                return thisEntry;		//	already exists
        }
        this.logs.push(logEntry);
        return logEntry;
    };	//	addLog


    /**
	 *	Set Log of Process.
	 *  <pre>
	 *  - Translated Process Message
	 *  - List of log entries
	 *      Date - Number - Msg
	 *  </pre>
	 *	@param html if true with HTML markup
	 *	@return Log Info
	 */
    ProcessInfo.prototype.getLogInfo = function (html) {
        if (this.logs == null)
            return "";
        //
        var sb = new StringBuilder();
        //SimpleDateFormat dateTimeFormat = DisplayType.getDateFormat(DisplayTypeConstants.DateTime);
        //SimpleDateFormat dateFormat = DisplayType.getDateFormat(DisplayTypeConstants.Date);
        if (html)
            sb.append("<table width=\"100%\" border=\"1\" cellspacing=\"0\" cellpadding=\"2\">");
        //
        //	boolean hasIDCol = false;
        var hasDateCol = false;
        var hasNoCol = false;
        var hasMsgCol = false;
        for (var i = 0; i < this.logs.length; i++) {
            var log = this.logs[i];
            //	if (log.getP_ID() != 0)
            //		hasIDCol = true;
            if (log.getP_Date() != null)
                hasDateCol = true;
            if (log.getP_Number() != null)
                hasNoCol = true;
            if (log.getP_Msg() != null)
                hasMsgCol = true;
        }

        for (i = 0; i < this.logs.length; i++) {
            if (html)
                sb.append("<tr>");
            else if (i > 0)
                sb.append("\n");
            //
            var log = this.logs[i];
            /**
            if (log.getP_ID() != 0)
                sb.append(html ? "<td>" : "")
                    .append(log.getP_ID())
                    .append(html ? "</td>" : " \t");	**/
            //
            if (log.getP_Date() != null) {
                sb.append(html ? "<td>" : "");
                var ts = log.getP_Date();
                //if (TimeUtil.isDay(ts))
                //  sb.append(dateFormat.format(ts));
                // else
                //  sb.append(dateTimeFormat.format(ts));
                sb.append(html ? "</td>" : " \t");
            }
            else if (hasDateCol)
                sb.append(html ? "<td>&nbsp;</td>" : " \t");
            //
            if (log.getP_Number() != null) {
                sb.append(html ? "<td>" : "")
                    .append(log.getP_Number())
                    .append(html ? "</td>" : " \t");
            }
            else if (hasNoCol)
                sb.append(html ? "<td>&nbsp;</td>" : " \t");
            //
            if (log.getP_Msg() != null) {
                sb.append(html ? "<td>" : "")
                    .append(VIS.Msg.parseTranslation(VIS.Env.getCtx(), log.getP_Msg()))
                    .append(html ? "</td>" : "");
            }
            else if (hasMsgCol)
                sb.append(html ? "<td>&nbsp;</td>" : "");
            //
            if (html)
                sb.append("</tr>");
        }
        if (html)
            sb.append("</table>");
        return sb.toString();
    }	//	getLogInfo



    ProcessInfo.prototype.dispose = function () {
        this.title = null;
        this.AD_Process_ID = null;
        this.table_ID = null;
        this.tableName = null;
        this.record_ID = null;
        this.AD_User_ID = null;
        this.AD_Client_ID == null;
        this.className = null;

        this.AD_PInstance_ID = null;
        this.summary = null;
        this.error = null;
        this.batch = null;
        //Process timed out		
        this.timeout = null;
        //private List<ProcessInfoLog> m_logs = null;
        this.parameter = null;
        this.AD_PrintFormat_Table_ID = null;
        this.AD_PrintFormat_ID = null;
        this.isReportFormat = null;
        this.isCrystal = null;
        this.totalrecords = null;
    };




    /*****************************************************/
    /********       Process Info Util            ********/
    /***************************************************/

    /**
 * 	Process Info with Utilities
 *
 */
    VIS.ProcessInfoUtil = {

        /**
	 *	Set Log of Process.
	 * 	@param pi process info
	 */
        setLogFromDB: function (pi) {
            //	s_log.fine("setLogFromDB - AD_PInstance_ID=" + pi.getAD_PInstance_ID());
            var sql = "SELECT Log_ID, P_ID, P_Date, P_Number, P_Msg "
                + "FROM AD_PInstance_Log "
                + "WHERE AD_PInstance_ID= " + pi.getAD_PInstance_ID()
                + " ORDER BY Log_ID";


            var dr = null;
            try {

                dr = VIS.DB.executeDataReader(sql);

                while (dr.read())
                    //	int Log_ID, int P_ID, Timestamp P_Date, BigDecimal P_Number, String P_Msg
                    pi.addLog(dr.getInt(0), dr.getInt(1), dr.getDateTime(2), dr.getDecimal(3), dr.getString(4));

                //dr.dispose();
            }
            catch (e) {
                console.log(e);
            }
            finally {
                if (dr)
                    dr.dispose();
            }
        }	//	getLogFromDB

    };


    /*****************************************************/
    /********               End                  ********/
    /***************************************************/



    /**
     * 	Process Info Log (VO)
     */
    function ProcessInfoLog(Log_ID,P_ID, P_Date, P_Number, P_Msg) {
        this.P_ID;
        this.P_Date;
        this.P_Number;
        this.P_Msg;

        if (Log_ID) {
            this.setLog_ID(Log_ID);
        }
        else
            this.setLog_ID(this.Log_ID++);

        this.setP_ID(P_ID);
        this.setP_Date(P_Date);
        this.setP_Number(P_Number);
        this.setP_Msg(P_Msg);
    };
    ProcessInfoLog.prototype.Log_ID = 0;

    /* Get Log_ID
    * @return id
    */
    ProcessInfoLog.prototype.getLog_ID = function () {
        return this.Log_ID;
    };

    /**
     * 	Set Log_ID
     *	@param Log_ID id
     */
    ProcessInfoLog.prototype.setLog_ID = function (Log_ID) {
        this.Log_ID = Log_ID;
    };

    /**
     * Method getP_ID
     * @return int
     */
    ProcessInfoLog.prototype.getP_ID = function () {
        return this.P_ID;
    };
    /**
     * Method setP_ID
     * @param P_ID int
     */
    ProcessInfoLog.prototype.setP_ID = function (P_ID) {
        this.P_ID = P_ID;
    };

    /**
     * Method getP_Date
     * @return Timestamp
     */
    ProcessInfoLog.prototype.getP_Date = function () {
        return this.P_Date;
    };
    /**
     * Method setP_Date
     * @param P_Date Timestamp
     */
    ProcessInfoLog.prototype.setP_Date = function (P_Date) {
        this.P_Date = P_Date;
    };

    /**
     * Method getP_Number
     * @return BigDecimal
     */
    ProcessInfoLog.prototype.getP_Number = function () {
        return this.P_Number;
    };
    /**
     * Method setP_Number
     * @param P_Number BigDecimal
     */
    ProcessInfoLog.prototype.setP_Number = function (P_Number) {
        this.P_Number = P_Number;
    };

    /**
     * Method getP_Msg
     * @return String
     */
    ProcessInfoLog.prototype.getP_Msg = function () {
        return this.P_Msg;
    };
    /**
     * Method setP_Msg
     * @param P_Msg String
     */
    ProcessInfoLog.prototype.setP_Msg = function (P_Msg) {
        this.P_Msg = P_Msg;
    };


    //	ProcessInfoLog

    VIS.ProcessInfo = ProcessInfo;
})(VIS, jQuery);