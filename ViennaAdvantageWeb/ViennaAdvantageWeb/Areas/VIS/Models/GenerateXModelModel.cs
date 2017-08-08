using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Data;
using VAdvantage.DataBase;

namespace ViennaAdvantage.Tool.Model
{
    public class GenerateXModelModel
    {
        /// <summary>
        /// Data set for combo box
        /// </summary>
        /// <returns></returns>
        public  DataSet GetTable()
        {
            string strQuery = "select Name, AD_TABLE_ID,TableName from AD_TABLE order by name";
            //execute
            return DB.ExecuteDataset(strQuery,null);
        }
        /// <summary>
        /// Dataset to bind checked list box
        /// </summary>
        /// <returns></returns>
        public DataSet GetEntity()
        {
            string strQuery = "select ad_entitytype_id, entitytype, name from ad_entitytype";
            //execute
            return DB.ExecuteDataset(strQuery,null);
        }
    }
}
