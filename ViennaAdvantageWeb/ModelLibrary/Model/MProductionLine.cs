/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MProductionLine
 * Purpose        : Production Plan model.
 * Class Used     : X_M_ProductionLine
 * Chronological    Development
 * Raghunandan     24-Nov-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MProductionLine : X_M_ProductionLine
    {
        /// <summary>
        /// 	Std Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="M_ProductionLine_ID"></param>
        /// <param name="trxName"></param>
        public MProductionLine(Context ctx, int M_ProductionLine_ID, Trx trxName)
            : base(ctx, M_ProductionLine_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MProductionLine(Context ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || Utility.Util.GetValueOfInt(newM_Product_ID) == 0)
            {
                return;
            }
            int M_Product_ID = Utility.Util.GetValueOfInt(newM_Product_ID);
            base.SetM_Product_ID(M_Product_ID);
            if (M_Product_ID == 0)
            {
                SetM_AttributeSetInstance_ID(0);
                return;
            }
            //	Set Attribute
            int M_AttributeSetInstance_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID");
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                && M_AttributeSetInstance_ID != 0)
            {
                SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            }
            else
            {
                SetM_AttributeSetInstance_ID(0);
            }
        }
    }
}
