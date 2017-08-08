/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_M_CostQueue
 * Chronological Development
 * Veena Pandey     18-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MCostQueue : X_M_CostQueue
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MCostQueue).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">multi-key</param>
        /// <param name="trxName">transaction</param>
        public MCostQueue(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, ignored, trxName)
        {
            if (ignored == 0)
            {
                //	setC_AcctSchema_ID (0);
                //	setM_AttributeSetInstance_ID (0);
                //	setM_CostElement_ID (0);
                //	setM_CostType_ID (0);
                //	setM_Product_ID (0);
                SetCurrentCostPrice(Env.ZERO);
                SetCurrentQty(Env.ZERO);
            }
            else
                throw new ArgumentException("Multi-Key");
        }

        /// <summary>
        /// Load Construor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public MCostQueue(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Parent Constructor
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute Set Instance</param>
        /// <param name="mas">Acct Schema</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="M_CostElement_ID">cost element</param>
        /// <param name="trxName">transaction</param>
        public MCostQueue(MProduct product, int M_AttributeSetInstance_ID,
            MAcctSchema mas, int AD_Org_ID, int M_CostElement_ID, Trx trxName)
            : this(product.GetCtx(), 0, trxName)
        {
            SetClientOrg(product.GetAD_Client_ID(), AD_Org_ID);
            SetC_AcctSchema_ID(mas.GetC_AcctSchema_ID());
            SetM_CostType_ID(mas.GetM_CostType_ID());
            SetM_Product_ID(product.GetM_Product_ID());
            SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            SetM_CostElement_ID(M_CostElement_ID);
        }

        /// <summary>
        /// Adjust Qty based on in Lifo/Fifo order
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">costing level ASI</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="Org_ID">costing level org</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="Qty">quantity to be reduced</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost price reduced or null of error</returns>
        public static Decimal? AdjustQty(MProduct product, int M_ASI_ID, MAcctSchema mas,
                int Org_ID, MCostElement ce, Decimal Qty, Trx trxName)
        {
            if (Env.Signum(Qty) == 0)
                return Env.ZERO;
            MCostQueue[] costQ = GetQueue(product, M_ASI_ID, mas, Org_ID, ce, trxName);
            Decimal remainingQty = Qty;
            for (int i = 0; i < costQ.Length; i++)
            {
                MCostQueue queue = costQ[i];
                //	Negative Qty i.e. add
                if (Env.Signum(remainingQty) < 0)
                {
                    Decimal oldQty = queue.GetCurrentQty();
                    Decimal newQty = Decimal.Subtract(oldQty, remainingQty);
                    queue.SetCurrentQty(newQty);
                    if (queue.Save())
                    {
                        _log.Fine("Qty=" + remainingQty
                            + "(!), ASI=" + queue.GetM_AttributeSetInstance_ID()
                            + " - " + oldQty + " -> " + newQty);
                        return queue.GetCurrentCostPrice();
                    }
                    else
                        return null;
                }

                //	Positive queue
                if (Env.Signum(queue.GetCurrentQty()) > 0)
                {
                    Decimal reduction = remainingQty;
                    if (reduction.CompareTo(queue.GetCurrentQty()) > 0)
                        reduction = queue.GetCurrentQty();
                    Decimal oldQty = queue.GetCurrentQty();
                    Decimal newQty = Decimal.Subtract(oldQty, reduction);
                    queue.SetCurrentQty(newQty);
                    if (queue.Save())
                    {
                        _log.Fine("Qty=" + reduction
                            + ", ASI=" + queue.GetM_AttributeSetInstance_ID()
                            + " - " + oldQty + " -> " + newQty);
                        remainingQty = Decimal.Subtract(remainingQty, reduction);
                    }
                    else
                        return null;
                    //
                    if (Env.Signum(remainingQty) == 0)
                    {
                        return queue.GetCurrentCostPrice();
                    }
                }
            }	//	for queue	

            _log.Fine("RemainingQty=" + remainingQty);
            return null;
        }

        /// <summary>
        /// Get/Create Cost Queue Record.
        ///	CostingLevel is not validated
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_AttributeSetInstance_ID">real asi</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="AD_Org_ID">real org</param>
        /// <param name="M_CostElement_ID">element</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost queue or null</returns>
        public static MCostQueue Get(MProduct product, int M_AttributeSetInstance_ID,
            MAcctSchema mas, int AD_Org_ID, int M_CostElement_ID, Trx trxName)
        {
            MCostQueue costQ = null;
            String sql = "SELECT * FROM M_CostQueue "
                + "WHERE AD_Client_ID=@client AND AD_Org_ID=@org"
                + " AND M_Product_ID=@pro"
                + " AND M_AttributeSetInstance_ID=@asi"
                + " AND M_CostType_ID=@ct AND C_AcctSchema_ID=@accs"
                + " AND M_CostElement_ID=@ce";
            try
            {
                SqlParameter[] param = new SqlParameter[7];
                param[0] = new SqlParameter("@client", product.GetAD_Client_ID());
                param[1] = new SqlParameter("@org", AD_Org_ID);
                param[2] = new SqlParameter("@pro", product.GetM_Product_ID());
                param[3] = new SqlParameter("@asi", M_AttributeSetInstance_ID);
                param[4] = new SqlParameter("@ct", mas.GetM_CostType_ID());
                param[5] = new SqlParameter("@accs", mas.GetC_AcctSchema_ID());
                param[6] = new SqlParameter("@ce", M_CostElement_ID);

                DataSet ds = DataBase.DB.ExecuteDataset(sql, param);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        costQ = new MCostQueue(product.GetCtx(), dr, trxName);
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            //	New
            if (costQ == null)
                costQ = new MCostQueue(product, M_AttributeSetInstance_ID,
                    mas, AD_Org_ID, M_CostElement_ID, trxName);
            return costQ;
        }

        /// <summary>
        /// Calculate Cost based on Qty based on in Lifo/Fifo order
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">costing level ASI</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="Org_ID">costing level org</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="Qty">quantity to be reduced</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost for qty or null of error</returns>
        public static Decimal? GetCosts(MProduct product, int M_ASI_ID, MAcctSchema mas,
            int Org_ID, MCostElement ce, Decimal Qty, Trx trxName)
	    {
		    if (Env.Signum(Qty) == 0)
			    return Env.ZERO;
		    MCostQueue[] costQ = GetQueue(product, M_ASI_ID, 
			    mas, Org_ID, ce, trxName);
		    //
		    Decimal cost = Env.ZERO;
		    Decimal remainingQty = Qty;
		    Decimal? firstPrice = null;
		    Decimal? lastPrice = null;
		    //
		    for (int i = 0; i < costQ.Length; i++)
		    {
			    MCostQueue queue = costQ[i];
			    //	Negative Qty i.e. add
			    if (Env.Signum(remainingQty) <= 0)
			    {
				    Decimal oldQty = queue.GetCurrentQty();
				    lastPrice = queue.GetCurrentCostPrice();
                    Decimal costBatch = Decimal.Multiply((Decimal)lastPrice, remainingQty);
                    cost = Decimal.Add(cost, costBatch);
                    _log.Config("ASI=" + queue.GetM_AttributeSetInstance_ID()
                        + " - Cost=" + lastPrice + " * Qty=" + remainingQty + "(!) = " + costBatch);
				    return cost;
			    }
    			
			    //	Positive queue
                if (Env.Signum(queue.GetCurrentQty()) > 0)
			    {
				    Decimal reduction = remainingQty;
				    if (reduction.CompareTo(queue.GetCurrentQty()) > 0)
					    reduction = queue.GetCurrentQty();
				    Decimal oldQty = queue.GetCurrentQty();
				    lastPrice = queue.GetCurrentCostPrice();
                    Decimal costBatch = Decimal.Multiply((Decimal)lastPrice, reduction);
                    cost = Decimal.Add(cost, costBatch);
                    _log.Fine("ASI=" + queue.GetM_AttributeSetInstance_ID()
                      + " - Cost=" + lastPrice + " * Qty=" + reduction + " = " + costBatch);
                    remainingQty = Decimal.Subtract(remainingQty, reduction);
				    //	Done
				    if (Env.Signum(remainingQty) == 0)
				    {
					    _log.Config("Cost=" + cost);
					    return cost;
				    }
				    if (firstPrice == null)
					    firstPrice = lastPrice;
			    }
		    }	//	for queue

		    if (lastPrice == null)
		    {
			    lastPrice = MCost.GetSeedCosts(product, M_ASI_ID, mas, Org_ID, 
				    ce.GetCostingMethod(), 0);
			    if (lastPrice == null)
			    {
				    _log.Info("No Price found");
				    return null;
			    }
			    _log.Info("No Cost Queue");
		    }
            Decimal costBatch1 = Decimal.Multiply((Decimal)lastPrice, remainingQty);
		    _log.Fine("RemainingQty=" + remainingQty + " * LastPrice=" + lastPrice + " = " + costBatch1);
            cost = Decimal.Add(cost, costBatch1);
		    _log.Config("Cost=" + cost);
		    return cost;
	    }

        /// <summary>
        /// Get Cost Queue Records in Lifo/Fifo order
        /// </summary>
        /// <param name="product">product</param>
        /// <param name="M_ASI_ID">costing level ASI</param>
        /// <param name="mas">accounting schema</param>
        /// <param name="Org_ID">costing level org</param>
        /// <param name="ce">Cost Element</param>
        /// <param name="trxName">transaction</param>
        /// <returns>cost queue or null</returns>
        public static MCostQueue[] GetQueue(MProduct product, int M_ASI_ID, MAcctSchema mas,
            int Org_ID, MCostElement ce, Trx trxName)
        {
            List<MCostQueue> list = new List<MCostQueue>();
            String sql = "SELECT * FROM M_CostQueue "
                + "WHERE AD_Client_ID=@client AND AD_Org_ID=@org"
                + " AND M_Product_ID=@prod"
                + " AND M_CostType_ID=@ct AND C_AcctSchema_ID=@accs"
                + " AND M_CostElement_ID=@ce";
            if (M_ASI_ID != 0)
                sql += " AND M_AttributeSetInstance_ID=@asi";
            sql += " AND CurrentQty<>0 "
                + "ORDER BY M_AttributeSetInstance_ID ";
            if (!ce.IsFifo())
                sql += "DESC";
            try
            {
                SqlParameter[] param = null;
                if (M_ASI_ID != 0)
                {
                    param = new SqlParameter[7];
                }
                else
                {
                    param = new SqlParameter[6];
                }
                param[0] = new SqlParameter("@client", product.GetAD_Client_ID());
                param[1] = new SqlParameter("@org", Org_ID);
                param[2] = new SqlParameter("@prod", product.GetM_Product_ID());
                param[3] = new SqlParameter("@ct", mas.GetM_CostType_ID());
                param[4] = new SqlParameter("@accs", mas.GetC_AcctSchema_ID());
                param[5] = new SqlParameter("@ce", ce.GetM_CostElement_ID());
                if (M_ASI_ID != 0)
                {
                    param[6] = new SqlParameter("@asi", M_ASI_ID);
                }
                DataSet ds = DataBase.DB.ExecuteDataset(sql, param,trxName);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MCostQueue(product.GetCtx(), dr, trxName));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MCostQueue[] costQ = new MCostQueue[list.Count];
            costQ = list.ToArray();
            return costQ;
        }

        /// <summary>
        /// Update Record.
        ///	((OldAvg*OldQty)+(Price*Qty)) / (OldQty+Qty)
        /// </summary>
        /// <param name="amt">total Amount</param>
        /// <param name="qty">quantity</param>
        /// <param name="precision">costing precision</param>
        public void SetCosts(Decimal amt, Decimal qty, int precision)
        {
            Decimal oldSum = Decimal.Multiply(GetCurrentCostPrice(), GetCurrentQty());
            Decimal newSum = amt;	//	is total already
            Decimal sumAmt = Decimal.Add(oldSum, newSum);
            Decimal sumQty = Decimal.Add(GetCurrentQty(), qty);
            if (Env.Signum(sumQty) != 0)
            {
                Decimal cost = Decimal.Round(Decimal.Divide(sumAmt, sumQty), precision, MidpointRounding.AwayFromZero);
                SetCurrentCostPrice(cost);
            }
            //
            SetCurrentQty(Decimal.Add(GetCurrentQty(), qty));
        }
    }
}
