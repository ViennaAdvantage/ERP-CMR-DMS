/********************************************************
 * Module Name    : 
 * Purpose        : Inventory Move Line Model
 * Class Used     : X_M_Movementline
 * Chronological Development
 * Veena         26-Oct-2009
 ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    /// <summary>
    /// Inventory Move Line Model
    /// </summary>
    public class MMovementLine : X_M_MovementLine
    {
        /** Parent							*/
        private MMovement _parent = null;
        public Decimal? OnHandQty = 0;
        private decimal qtyReserved = 0;
        private MStorage storage = null;
        private decimal qtyAvailable = 0;
        private int _mvlOldAttId = 0, _mvlNewAttId = 0;
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_MovementLine_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MMovementLine(Ctx ctx, int M_MovementLine_ID, Trx trxName)
            : base(ctx, M_MovementLine_ID, trxName)
        {
            if (M_MovementLine_ID == 0)
            {
                //	SetM_LocatorTo_ID (0);	// @M_LocatorTo_ID@
                //	SetM_Locator_ID (0);	// @M_Locator_ID@
                //	SetM_MovementLine_ID (0);			
                //	SetLine (0);	
                //	SetM_Product_ID (0);
                SetM_AttributeSetInstance_ID(0);	//	ID
                SetMovementQty(Env.ZERO);	// 1
                SetTargetQty(Env.ZERO);	// 0
                SetScrappedQty(Env.ZERO);
                SetConfirmedQty(Env.ZERO);
                SetProcessed(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transation</param>
        public MMovementLine(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent constructor
        /// </summary>
        /// <param name="parent">parent</param>
        public MMovementLine(MMovement parent)
            : this(parent.GetCtx(), 0, parent.Get_TrxName())
        {
            SetClientOrg(parent);
            SetM_Movement_ID(parent.GetM_Movement_ID());
        }

        /// <summary>
        /// Get AttributeSetInstance To
        /// </summary>
        /// <returns>asi</returns>
        public new int GetM_AttributeSetInstanceTo_ID()
        {
            int M_AttributeSetInstanceTo_ID = base.GetM_AttributeSetInstanceTo_ID();
            if (M_AttributeSetInstanceTo_ID == 0)
                M_AttributeSetInstanceTo_ID = base.GetM_AttributeSetInstance_ID();
            return M_AttributeSetInstanceTo_ID;
        }

        /// <summary>
        /// Add to Description
        /// </summary>
        /// <param name="description">description text</param>
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /// <summary>
        /// Get Product
        /// </summary>
        /// <returns>product or null if not defined</returns>
        public MProduct GetProduct()
        {
            if (GetM_Product_ID() != 0)
                return MProduct.Get(GetCtx(), GetM_Product_ID());
            return null;
        }

        /// <summary>
        /// Set Product - Callout
        /// </summary>
        /// <param name="oldM_Product_ID">old value</param>
        /// <param name="newM_Product_ID">new value</param>
        /// <param name="windowNo">window</param>
        public void SetM_Product_ID(String oldM_Product_ID, String newM_Product_ID, int windowNo)
        {
            if (newM_Product_ID == null || newM_Product_ID.Length == 0)
                return;
            int M_Product_ID = int.Parse(newM_Product_ID);
            if (M_Product_ID == 0)
                return;
            //
            base.SetM_Product_ID(M_Product_ID);
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_Product_ID") == M_Product_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID") != 0)
                SetM_AttributeSetInstance_ID(GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "M_AttributeSetInstance_ID"));
            else
                SetM_AttributeSetInstance_ID(0);
        }


        /// <summary>
        /// Set Movement Qty - enforce UOM
        /// </summary>
        /// <param name="movementQty">qty</param>
        public new void SetMovementQty(Decimal? movementQty)
        {
            if (movementQty != null)
            {
                MProduct product = GetProduct();
                if (product != null)
                {
                    int precision = product.GetUOMPrecision();
                    movementQty = Decimal.Round(movementQty.Value, precision, MidpointRounding.AwayFromZero);
                }
            }
            base.SetMovementQty(movementQty);
        }

        /// <summary>
        /// Get Parent
        /// </summary>
        /// 
        /// <returns>Parent Movement</returns>
        public MMovement GetParent()
        {
            if (_parent == null)
                _parent = new MMovement(GetCtx(), GetM_Movement_ID(), Get_TrxName());
            return _parent;
        }


        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            //	Set Line No
            if (GetLine() == 0)
            {
                String sql = "SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID=" + GetM_Movement_ID();
                int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                SetLine(ii);
            }

            // Check Locator For Header Warehouse
            MMovement mov = new MMovement(GetCtx(), GetM_Movement_ID(), Get_TrxName());
            MLocator loc = new MLocator(GetCtx(), GetM_Locator_ID(), Get_TrxName());
            Tuple<string, string, string> aInfo = null;
            if (Env.HasModulePrefix("DTD001_", out aInfo))
            {
                if (mov.GetDTD001_MWarehouseSource_ID() == loc.GetM_Warehouse_ID())
                {

                }
                else
                {
                    String sql = "SELECT M_Locator_ID FROM M_Locator WHERE M_Warehouse_ID=" + mov.GetDTD001_MWarehouseSource_ID() + " AND IsDefault = 'Y'";
                    int ii = DataBase.DB.GetSQLValue(Get_TrxName(), sql);
                    SetM_Locator_ID(ii);
                }
            }

            if (GetM_Locator_ID() == GetM_LocatorTo_ID())
            {
                log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "'From @M_Locator_ID@' and '@M_LocatorTo_ID@' cannot be same."));//change message according to requirement
                return false;
            }

            if (Env.Signum(GetMovementQty()) == 0)
            {
                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "MovementQty"));
                return false;
            }
            //Amit
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("M_Product_ID")) != GetM_Product_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ProdNotChanged"));
                    return false;
                }
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("M_Locator_ID")) != GetM_Locator_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_LocatorNotChanged"));
                    return false;
                }
                if (!newRecord && Util.GetValueOfInt(Get_ValueOld("M_RequisitionLine_ID")) != GetM_RequisitionLine_ID())
                {
                    log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_ReqNotChanged"));
                    return false;
                }
            }
            if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_MODULEINFO_ID) FROM AD_MODULEINFO WHERE PREFIX='VA203_'", null, null)) > 0)
            {
                if (GetM_RequisitionLine_ID() > 0)
                {
                    MRequisitionLine reqline = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), null);
                    if (GetM_AttributeSetInstance_ID() != reqline.GetM_AttributeSetInstance_ID())
                    {
                        log.SaveError("Message", Msg.GetMsg(GetCtx(), "VA203_AttributeInstanceMustBeSame"));
                        return false;
                    }
                }
            }
            // IF Doc Status = InProgress then No record Save
            MMovement move = new MMovement(GetCtx(), GetM_Movement_ID(), null);
            if (newRecord && move.GetDocStatus() == "IP")
            {
                log.SaveError("Message", Msg.GetMsg(GetCtx(), "DTD001_CannotCreate"));
                return false;
            }

            //	Qty Precision
            if (newRecord || Is_ValueChanged("QtyEntered"))
                SetMovementQty(GetMovementQty());
            int M_Warehouse_ID = 0; MWarehouse wh = null;
            string qry = "select m_warehouse_id from m_locator where m_locator_id=" + GetM_Locator_ID();
            M_Warehouse_ID = Util.GetValueOfInt(DB.ExecuteScalar(qry));

            wh = MWarehouse.Get(GetCtx(), M_Warehouse_ID);
            qry = "SELECT NVL(SUM(NVL(QtyOnHand,0)),0) AS QtyOnHand FROM M_Storage where m_locator_id=" + GetM_Locator_ID() + " and m_product_id=" + GetM_Product_ID();
            if (GetM_AttributeSetInstance_ID() != 0)
            {
                qry += " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID();
            }
            OnHandQty = Convert.ToDecimal(DB.ExecuteScalar(qry));
            if (wh.IsDisallowNegativeInv() == true)
            {
                if (GetDescription() != "RC")
                {
                    if (GetMovementQty() < 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "Qty Available " + OnHandQty));
                        //ShowMessage.Info("Qty Available " + OnHandQty, true, null, null);
                        return false;
                    }
                    else if ((OnHandQty - GetMovementQty()) < 0)
                    {
                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "Qty Available " + OnHandQty));
                        //ShowMessage.Info("Qty Available " + OnHandQty, true, null, null);
                        return false;
                    }
                }
            }
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                qry = "SELECT   NVL(SUM(NVL(QtyOnHand,0)- qtyreserved),0) AS QtyAvailable  FROM M_Storage where m_locator_id=" + GetM_Locator_ID() + " and m_product_id=" + GetM_Product_ID();
                if (GetM_AttributeSetInstance_ID() != 0)
                {
                    qry += " AND M_AttributeSetInstance_ID=" + GetM_AttributeSetInstance_ID();
                }
                qtyAvailable = Convert.ToDecimal(DB.ExecuteScalar(qry));
                qtyReserved = Util.GetValueOfDecimal(Get_ValueOld("MovementQty"));
                if (wh.IsDisallowNegativeInv() == true)
                {
                    if ((qtyAvailable < (GetMovementQty() - qtyReserved)))
                    {

                        log.SaveError("Message", Msg.GetElement(GetCtx(), "DTD001_QtyNotAvailable"));
                        return false;
                    }

                }
            }
            //	Mandatory Instance
            if (GetM_AttributeSetInstanceTo_ID() == 0)
            {
                if (GetM_AttributeSetInstance_ID() != 0)	//	Set to from
                    SetM_AttributeSetInstanceTo_ID(GetM_AttributeSetInstance_ID());
                else
                {
                    if (Env.HasModulePrefix("DTD001_", out mInfo))
                    {
                        MProduct product = GetProduct();
                        if (product != null
                            && product.GetM_AttributeSet_ID() != 0)
                        {
                            //MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                            //if (mas.IsInstanceAttribute() 
                            //    && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                            //{
                            //    log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstanceTo_ID"));
                            //    return false;
                            //}
                            if (GetDTD001_AttributeNumber() == "" || GetDTD001_AttributeNumber() == null)
                            {
                                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));

                                return false;
                            }
                        }
                        else
                        {
                            if (product != null)
                            {
                                if (product.GetM_AttributeSet_ID() == 0 && (GetDTD001_AttributeNumber() == "" || GetDTD001_AttributeNumber() == null))
                                    return true;
                                else
                                {
                                    //log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));
                                    //ShowMessage.Info("a", true, "Product is not of Attribute Type", null); 
                                    log.SaveError("Product is not of Attribute Type", Msg.GetElement(GetCtx(), "DTD001_AttributeNumber"));
                                    return false;
                                }

                                //Check No Of Attributes Are Equal To Quantity Or Less Than

                                int Count = CountAttributes(GetDTD001_AttributeNumber());
                                if (Count != GetMovementQty())
                                {
                                    if (Count > GetMovementQty())
                                    {
                                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "DTD001_MovementAttrbtGreater"));
                                        return false;
                                    }
                                    else
                                    {
                                        log.SaveError("Error", Msg.GetMsg(GetCtx(), "DTD001_MovementAttrbtLess"));
                                        return false;
                                    }
                                }
                            }

                        }

                    }
                    else
                    {
                        MProduct product = GetProduct();
                        if (product != null
                            && product.GetM_AttributeSet_ID() != 0)
                        {
                            MAttributeSet mas = MAttributeSet.Get(GetCtx(), product.GetM_AttributeSet_ID());
                            if (mas.IsInstanceAttribute()
                                && (mas.IsMandatory() || mas.IsMandatoryAlways()))
                            {
                                log.SaveError("FillMandatory", Msg.GetElement(GetCtx(), "M_AttributeSetInstanceTo_ID"));
                                return false;
                            }
                        }
                    }
                }
            }	//	ASI

            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                if (!newRecord && GetM_RequisitionLine_ID() != 0 && GetConfirmedQty() == 0)
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), null);
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + (GetMovementQty() - qtyReserved));
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), null);
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), 0, null);
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + (GetMovementQty() - qtyReserved));
                    if (!storage.Save())
                    {
                        return false;
                    }
                }
                //vikas 11/21/2014
                _mvlNewAttId = GetM_AttributeSetInstance_ID();
                if (_mvlOldAttId != _mvlNewAttId && !newRecord)
                {
                    //  Set QtyReserved On Storage Correspng to New attributesetinstc_id
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), null);
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), 0, null);
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }

                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), _mvlOldAttId, null);
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), 0, null);
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() - qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }
                }//vikas

                if (newRecord && GetM_RequisitionLine_ID() != 0 && GetDescription() != "RC")
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), null);
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() + GetMovementQty());
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), null);
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), 0, null);
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() + GetMovementQty());
                    if (!storage.Save())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected override bool BeforeDelete()
        {
            qtyReserved = GetMovementQty();
            return true;
        }
        protected override bool AfterDelete(bool success)
        {
            Tuple<String, String, String> mInfo = null;
            if (Env.HasModulePrefix("DTD001_", out mInfo))
            {
                if (GetM_RequisitionLine_ID() != 0)
                {
                    MRequisitionLine requisition = new MRequisitionLine(GetCtx(), GetM_RequisitionLine_ID(), null);
                    requisition.SetDTD001_ReservedQty(requisition.GetDTD001_ReservedQty() - qtyReserved);
                    if (!requisition.Save())
                    {
                        return false;
                    }
                    storage = MStorage.Get(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), GetM_AttributeSetInstance_ID(), null);
                    if (storage == null)
                    {
                        storage = MStorage.GetCreate(GetCtx(), GetM_Locator_ID(), GetM_Product_ID(), 0, null);
                    }
                    storage.SetQtyReserved(storage.GetQtyReserved() - qtyReserved);
                    if (!storage.Save())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static int CountAttributes(string Attributes)
        {
            int n = 0;
            if (Attributes != null)
            {
                foreach (var c in Attributes)
                {
                    if ((c == '\n') || (c == '\r')) n++;
                }

            }
            return n + 1;
        }

    }
}
