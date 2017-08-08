/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductAttributes
 * Purpose        : Product attributes setting using x-classes
 * Class Used     : X_M_ProductAttributes
 * Chronological    Development
 * Raghunandan     04-Feb-2015
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;

namespace VAdvantage.Model
{
    public class MProductAttributes : X_M_ProductAttributes
    {
        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProductAttributes(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /// <summary>
        /// Load Cosntructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MProductAttributes(Ctx ctx, int M_ProductAttributes_ID, Trx trxName)
            : base(ctx, M_ProductAttributes_ID, trxName)
        {
        }

    }
}
