/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MPackageLine
 * Purpose        : Package Line Model
 * Class Used     : X_M_PackageLine
 * Chronological    Development
 * Raghunandan     21-Oct-2009
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
    public class MPackageLine : X_M_PackageLine
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_PackageLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPackageLine(Ctx ctx, int M_PackageLine_ID, Trx trxName)
            : base(ctx, M_PackageLine_ID, trxName)
        {
            if (M_PackageLine_ID == 0)
            {
                //	setM_Package_ID (0);
                //	setM_InOutLine_ID (0);
                SetQty(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MPackageLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="parent">header</param>
        public MPackageLine(MPackage parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_Package_ID(parent.GetM_Package_ID());
        }

        /// <summary>
        /// Set Shipment Line
        /// </summary>
        /// <param name="line">line</param>
        public void SetInOutLine(MInOutLine line)
        {
            SetM_InOutLine_ID(line.GetM_InOutLine_ID());
            SetQty(line.GetMovementQty());
        }
        /// <summary>
        /// Edited :  Abhishek , 17/10/2014

        public Decimal SetMovementLine(VAdvantage.Model.MMovementLine line)
        {
            Decimal _count = 0;
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'"));
            if (_CountDTD001 > 0)
            {
                SetM_MovementLine_ID(line.GetM_MovementLine_ID());
                SetDTD001_TotalQty(line.GetMovementQty());
                SetM_Product_ID(line.GetM_Product_ID());
                SetM_AttributeSetInstance_ID(line.GetM_AttributeSetInstance_ID());
                decimal totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_MovementLine_ID=" + line.GetM_MovementLine_ID()));
                SetQty(line.GetMovementQty() - totalPackQty);
                _count = Util.GetValueOfDecimal(line.GetMovementQty()) - totalPackQty;
                SetDTD001_AlreadyPackQty(totalPackQty);
                SetConfirmedQty(line.GetMovementQty() - totalPackQty);
                SetDTD001_ConfirmDate(System.DateTime.Now);

            }
            return _count;
        }
        public void SetInoutLine(VAdvantage.Model.MInOutLine line)
        {
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_'"));
            if (_CountDTD001 > 0)
            {
                SetM_InOutLine_ID(line.GetM_InOut_ID());
                SetDTD001_TotalQty(line.GetMovementQty());
                SetM_Product_ID(line.GetM_Product_ID());
                SetM_AttributeSetInstance_ID(line.GetM_AttributeSetInstance_ID());
                decimal totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID()));
                SetQty(line.GetMovementQty() - totalPackQty);
                SetDTD001_AlreadyPackQty(totalPackQty);
                SetConfirmedQty(line.GetMovementQty() - totalPackQty);
                SetDTD001_ConfirmDate(System.DateTime.Now);
            }
        }
        protected override bool BeforeSave(bool newRecord)
        {
            Decimal difference = 0;
            int _CountDTD001 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='DTD001_' AND IsActive='Y'"));
            if (_CountDTD001 > 0)
            {
                decimal totalPackQty = 0;
                if (GetConfirmedQty() > GetQty())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ConfirmQtyNotGreater"));
                    return false;
                }
                if (GetM_InOutLine_ID() > 0 && newRecord)
                {
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                }
                else if (GetM_MovementLine_ID() > 0 && newRecord)
                {
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                }
                else if (GetM_InOutLine_ID() > 0 && !(newRecord))
                {
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_InOutLine_ID=" + GetM_InOutLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_PackageLine_ID<>" + GetM_PackageLine_ID()));
                }
                else if (GetM_MovementLine_ID() > 0 && !(newRecord))
                {
                    totalPackQty = Util.GetValueOfDecimal(DB.ExecuteScalar("SELECT SUM(Qty) FROM M_PackageLine WHERE M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_PackageLine_ID<>" + GetM_PackageLine_ID()));
                }
                else if (!(GetM_MovementLine_ID() > 0) && !(GetM_InOutLine_ID() > 0))
                {
                    difference = GetQty();
                    difference = Decimal.Subtract(difference, GetConfirmedQty());
                    difference = Decimal.Subtract(difference, GetScrappedQty());
                    SetDifferenceQty(difference);
                    return true;
                }
                else if (!(GetM_MovementLine_ID() > 0) && !(GetM_InOutLine_ID() > 0) && !newRecord)
                {
                    totalPackQty = 0;
                }
                SetDTD001_AlreadyPackQty(totalPackQty);
                if (totalPackQty + GetQty() > GetDTD001_TotalQty())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_MoreThenTotalQty"));
                    //  ShowMessage.Warn(Msg.GetMsg(GetCtx(), "DTD001_MoreThenTotalQty"), null, "", "");
                    return false;
                }
                //_sql = "SELECT COUNT(*) FROM   M_MovementLine WHERE IsActive='Y' AND  M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID();
                //int MvAttribute = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM   M_MovementLine WHERE IsActive='Y' AND  M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID() + " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID()));
                //if (MvAttribute > 0)
                //{

                //}
                //else
                //{
                //    return false;
                //}
                difference = GetQty();
                difference = Decimal.Subtract(difference, GetConfirmedQty());
                difference = Decimal.Subtract(difference, GetScrappedQty());
                SetDifferenceQty(difference);

                int _pckgAttrbt = GetM_AttributeSetInstance_ID();
                int MvAttribute = Util.GetValueOfInt(DB.ExecuteScalar("SELECT M_AttributeSetInstance_ID  FROM   M_MovementLine WHERE IsActive='Y' AND  M_MovementLine_ID=" + GetM_MovementLine_ID() + " AND M_Product_ID=" + GetM_Product_ID()));
                if (MvAttribute == _pckgAttrbt)
                {

                }
                else
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_AttributeNotSetMvL"));
                    // ShowMessage.Warn(Msg.GetMsg(GetCtx(), "DTD001_AttributeNotSetMvL"), null, "", "");
                    return false;
                }

            }

            return true;
        }

        /// </summary>
        /// <param name="line"></param>
    }
}
