/********************************************************
 * ModuleName     : 
 * Purpose        : 
 * Class Used     : X_M_Transaction
 * Chronological    Development
 * Raghunandan     08-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;

using VAdvantage.Model;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MTransaction : X_M_Transaction
    {
        /**
	 * 	Standard Constructor
	 *	@param ctx context
	 *	@param M_Transaction_ID id
	 *	@param trxName transaction
	 */
        public MTransaction(Ctx ctx, int M_Transaction_ID, Trx trxName)
            : base(ctx, M_Transaction_ID, trxName)
        {
            if (M_Transaction_ID == 0)
            {
                //	setM_Transaction_ID (0);		//	PK
                //	setM_Locator_ID (0);
                //	setM_Product_ID (0);
                SetMovementDate(DateTime.Now);
                SetMovementQty(Env.ZERO);
                //	setMovementType (MOVEMENTTYPE_CustomerShipment);
            }
        }

        /**
	 * 	Load Constructor
	 *	@param ctx context
	 *	@param dr result set
	 *	@param trxName transaction
	 */
        public MTransaction(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
        * 	Detail Constructor
        *	@param ctx context
        *	@param AD_Org_ID org
        * 	@param MovementType movement type
        * 	@param M_Locator_ID locator
        * 	@param M_Product_ID product
        * 	@param M_AttributeSetInstance_ID attribute
        * 	@param MovementQty qty
        * 	@param MovementDate optional date
        *	@param trxName transaction
        */
        public MTransaction(Ctx ctx, int AD_Org_ID, String MovementType,
            int M_Locator_ID, int M_Product_ID, int M_AttributeSetInstance_ID,
            Decimal MovementQty, DateTime? MovementDate, Trx trxName)
            : base(ctx, 0, trxName)
        {

            SetAD_Org_ID(AD_Org_ID);
            SetMovementType(MovementType);
            if (M_Locator_ID == 0)
                throw new ArgumentException("No Locator");
            SetM_Locator_ID(M_Locator_ID);
            if (M_Product_ID == 0)
                throw new ArgumentException("No Product");
            SetM_Product_ID(M_Product_ID);
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            //
            //if (MovementQty != null)		//	Can be 0
                SetMovementQty(MovementQty);
            if (MovementDate == null)
                SetMovementDate(DateTime.Now);
            else
                SetMovementDate(MovementDate);
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MTransaction[");
            sb.Append(Get_ID()).Append(",").Append(GetMovementType())
                .Append(",Qty=").Append(GetMovementQty())
                .Append(",M_Product_ID=").Append(GetM_Product_ID())
                .Append(",ASI=").Append(GetM_AttributeSetInstance_ID())
                .Append("]");
            return sb.ToString();
        }

    }
}