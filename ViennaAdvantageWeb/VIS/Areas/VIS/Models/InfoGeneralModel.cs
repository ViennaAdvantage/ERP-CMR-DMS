using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.Classes;
using VAdvantage.Model;
using VIS.DBase;

namespace VIS.Models
{
    public class InfoGeneralModel
    {

        public List<InfoGenral> GetSchema(string tableName)
        {
            try
            {
                string sql = string.Empty;
                sql = @"SELECT c.ColumnName,
                            c.name,
                            c.IsIdentifier,
                            t.AD_Table_ID,
                            t.TableName 
                         FROM AD_Table t
                            INNER JOIN AD_Column c ON (t.AD_Table_ID=c.AD_Table_ID)
                            WHERE c.AD_Reference_ID=10
                            AND t.TableName='" + tableName + @"'
                           AND EXISTS (SELECT * FROM AD_Field f 
                     WHERE f.AD_Column_ID=c.AD_Column_ID
                     AND f.IsDisplayed='Y' AND f.IsEncrypted='N' AND f.ObscureType IS NULL)                    
                 ORDER BY c.IsIdentifier DESC, c.SeqNo";
                DataSet ds = DB.ExecuteDataset(sql);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                List<InfoGenral> lstInfoGen = new List<InfoGenral>();
                InfoGenral item = null;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (i == 3)
                    {
                        break;
                    }
                    item = new InfoGenral();
                    item.AD_Table_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Table_ID"]);
                    item.ColumnName = ds.Tables[0].Rows[i]["ColumnName"].ToString();
                    item.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                    item.IsIdentifier = ds.Tables[0].Rows[i]["IsIdentifier"].ToString() == "Y" ? true : false;
                    lstInfoGen.Add(item);

                }
                ds = null;
                return lstInfoGen;
            }
            catch
            {
                return null;
            }
        }

        public List<InfoColumn> GetDisplayCol(int AD_Table_ID)
        {
            try
            {
                string sql = @"SELECT c.ColumnName,c.Name,
                              c.AD_Reference_ID,
                              c.IsKey,
                              f.IsDisplayed,
                              c.AD_Reference_Value_ID,
                              c.ColumnSQL
                            FROM AD_Column c
                            INNER JOIN AD_Table t
                            ON (c.AD_Table_ID=t.AD_Table_ID)
                            INNER JOIN AD_Tab tab
                            ON (t.AD_Window_ID=tab.AD_Window_ID)
                            INNER JOIN AD_Field f
                            ON (tab.AD_Tab_ID  =f.AD_Tab_ID
                            AND f.AD_Column_ID =c.AD_Column_ID)
                            WHERE t.AD_Table_ID=" + AD_Table_ID + @"
                            AND (c.IsKey       ='Y'
                            OR (f.IsEncrypted  ='N'
                            AND f.ObscureType IS NULL))                            
                            ORDER BY c.IsKey DESC,
                              f.SeqNo";

                DataSet ds = DB.ExecuteDataset(sql);

                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                List<InfoColumn> lstCols = new List<InfoColumn>();
                InfoColumn item = null;
                int displayType = 0;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    item = new InfoColumn();
                    item.IsKey = ds.Tables[0].Rows[i]["IsKey"].ToString() == "Y" ? true : false;
                    item.IsDisplayed = ds.Tables[0].Rows[i]["IsDisplayed"].ToString() == "Y" ? true : false;


                    displayType = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Reference_ID"]);

                    if (item.IsKey)
                    {
                    }
                    else if (!item.IsDisplayed)
                    {
                        continue;
                    }
                    else if (!(displayType == DisplayType.YesNo
                        || displayType == DisplayType.Amount
                        || displayType == DisplayType.Number
                        || displayType == DisplayType.Quantity
                        || displayType == DisplayType.Integer
                        || displayType == DisplayType.String
                        || displayType == DisplayType.Text
                        || displayType == DisplayType.Memo
                        || DisplayType.IsDate(displayType)
                        || displayType == DisplayType.List))
                    {
                        continue;
                    }
                    else if (!(ds.Tables[0].Rows[i]["ColumnSQL"] == null || ds.Tables[0].Rows[i]["ColumnSQL"] == DBNull.Value))
                    {
                        continue;
                    }

                    item.ColumnName = ds.Tables[0].Rows[i]["ColumnName"].ToString();
                    item.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                    item.AD_Reference_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Reference_ID"]);
                    item.IsKey = ds.Tables[0].Rows[i]["IsKey"].ToString() == "Y" ? true : false;
                    item.IsDisplayed = ds.Tables[0].Rows[i]["IsDisplayed"].ToString() == "Y" ? true : false;
                    if (!(ds.Tables[0].Rows[i]["AD_Reference_Value_ID"] == null || ds.Tables[0].Rows[i]["AD_Reference_Value_ID"] == DBNull.Value))
                    {
                        item.AD_Reference_Value_ID = Convert.ToInt32(ds.Tables[0].Rows[i]["AD_Reference_Value_ID"]);
                        item.RefList = GetRefList(item.AD_Reference_Value_ID);
                    }
                    item.ColumnSQL = ds.Tables[0].Rows[i]["ColumnSQL"].ToString();

                    lstCols.Add(item);

                }

                return lstCols;
            }
            catch
            {
                return null;
            }
        }
        private List<InfoRefList> GetRefList(int AD_Reference_ID)
        {
            try
            {
                String sql = "SELECT Value, Name FROM AD_Ref_List "
                    + "WHERE AD_Reference_ID=" + AD_Reference_ID + " AND IsActive='Y' ORDER BY 1";
                DataSet ds = null;
                List<InfoRefList> list = new List<InfoRefList>();
                try
                {
                    ds = DB.ExecuteDataset(sql, null, null);
                    InfoRefList itm = new InfoRefList();
                    itm.Key = "";
                    itm.Value = "";
                    list.Add(itm);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        itm = new InfoRefList();
                        itm.Key = ds.Tables[0].Rows[i]["Value"].ToString();
                        itm.Value = ds.Tables[0].Rows[i]["Name"].ToString();

                        list.Add(itm);
                    }
                    ds = null;
                }
                catch (Exception)
                {

                }

                return list;
            }
            catch
            {
                return null;
            }
        }


        public List<DataObject> GetData(string sql, string tableName, VAdvantage.Utility.Ctx ctx)
        {
            try
            {
                sql = sql.Replace('●', '%');
                sql = MRole.GetDefault(ctx).AddAccessSQL(sql, tableName,
                                MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
                DataSet data = DBase.DB.ExecuteDataset(sql);
                if (data == null )
                {
                    return null;
                }

                List<DataObject> dyndata = new List<DataObject>();
                DataObject item = null;
                List<object> values = null;
                for (int i = 0; i < data.Tables[0].Columns.Count; i++)  //columns
                {
                    item = new DataObject();

                    item.ColumnName = data.Tables[0].Columns[i].ColumnName;
                    values = new List<object>();
                    for (int j = 0; j < data.Tables[0].Rows.Count; j++)  //rows
                    {

                        values.Add(data.Tables[0].Rows[j][data.Tables[0].Columns[i].ColumnName]);
                    }
                    item.Values = values;
                    item.RowCount = data.Tables[0].Rows.Count;
                    dyndata.Add(item);
                }
                return dyndata;
            }
            catch
            {
                return null;
            }
        }

    }
    public class InfoGenral
    {
        public int AD_Table_ID
        {
            get;
            set;
        }

        public string ColumnName
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public bool IsIdentifier
        {
            get;
            set;
        }


    }

    public class InfoColumn
    {
        public InfoColumn()
        {

        }

        public InfoColumn(String colHeader, String columnName, bool readOnly, String colSQL, int colClass)
        {
            Name = colHeader;
            ColumnName = columnName;
            ColumnSQL = colSQL;
            AD_Reference_ID = colClass;
        }

        public string ColumnName
        {
            get;
            set;
        }
        public int AD_Reference_ID
        {
            get;
            set;
        }
        public bool IsKey
        {
            get;
            set;
        }
        public bool IsDisplayed
        {
            get;
            set;
        }
        public int AD_Reference_Value_ID
        {
            get;
            set;
        }
        public string ColumnSQL
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public int _sequence
        {
            get;
            set;
        }
        public List<InfoRefList> RefList
        {
            get;
            set;
        }
        public InfoColumn Seq(int sequence)
        {
            _sequence = sequence;
            return this;
        }
    }

}