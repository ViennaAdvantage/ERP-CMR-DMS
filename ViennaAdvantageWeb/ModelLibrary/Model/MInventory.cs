/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInventory
 * Purpose        : Physical Inventory Model
 * Class Used     : X_M_Inventory, DocAction
 * Chronological    Development
 * Raghunandan     22-Jun-2009
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
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInventory : X_M_Inventory, DocAction
    {
        #region VAriables
        //	Process Message 			
        private String _processMsg = null;
        //	Just Prepared Flag			
        private bool _justPrepared = false;
        //	Cache						
        private static CCache<int, MInventory> _cache = new CCache<int, MInventory>("M_Inventory", 5, 5);
        //	Lines						
        private MInventoryLine[] _lines = null;

        private string sql = "";
        private Decimal? trxQty = 0;
        private bool isGetFromStorage = false;

        #endregion


        /*	Get Inventory from Cache
	 *	@param ctx context
	 *	@param M_Inventory_ID id
	 *	@return MInventory
	 */
        public static MInventory Get(Ctx ctx, int M_Inventory_ID)
        {
            int key = M_Inventory_ID;
            MInventory retValue = (MInventory)_cache[key];
            if (retValue != null)
                return retValue;
            retValue = new MInventory(ctx, M_Inventory_ID, null);
            if (retValue.Get_ID() != 0)
                _cache.Add(key, retValue);
            return retValue;
        }

        /**
         * 	Standard Constructor
         *	@param ctx context 
         *	@param M_Inventory_ID id
         *	@param trxName transaction
         */
        public MInventory(Ctx ctx, int M_Inventory_ID, Trx trxName)
            : base(ctx, M_Inventory_ID, trxName)
        {

            if (M_Inventory_ID == 0)
            {
                //	setName (null);
                //  setM_Warehouse_ID (0);		//	FK
                SetMovementDate(DateTime.Now);
                SetDocAction(DOCACTION_Complete);	// CO
                SetDocStatus(DOCSTATUS_Drafted);	// DR
                SetIsApproved(false);
                SetMovementDate(DateTime.Now);	// @#Date@
                SetPosted(false);
                SetProcessed(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MInventory(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /**
         * 	Warehouse Constructor
         *	@param wh warehouse
         */
        public MInventory(MWarehouse wh)
            : this(wh.GetCtx(), 0, wh.Get_TrxName())
        {
            SetClientOrg(wh);
            SetM_Warehouse_ID(wh.GetM_Warehouse_ID());
        }



        /**
         * 	Get Lines
         *	@param requery requery
         *	@return array of lines
         */
        public MInventoryLine[] GetLines(bool requery)
        {
            if (_lines != null && !requery)
                return _lines;
            //
            List<MInventoryLine> list = new List<MInventoryLine>();
            String sql = "SELECT * FROM M_InventoryLine WHERE M_Inventory_ID=" + GetM_Inventory_ID() + " ORDER BY Line";
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInventoryLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally { dt = null; }
            _lines = new MInventoryLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /**
         * 	Add to Description
         *	@param description text
         */
        public void AddDescription(String description)
        {
            String desc = GetDescription();
            if (desc == null)
                SetDescription(description);
            else
                SetDescription(desc + " | " + description);
        }

        /**
         * 	Overwrite Client/Org - from Import.
         * 	@param AD_Client_ID client
         * 	@param AD_Org_ID org
         */
        public void SetClientOrg(int AD_Client_ID, int AD_Org_ID)
        {
            base.SetClientOrg(AD_Client_ID, AD_Org_ID);
        }

        /**
         * 	String Representation
         *	@return Info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInventory[");
            sb.Append(Get_ID())
                .Append("-").Append(GetDocumentNo())
                .Append(",M_Warehouse_ID=").Append(GetM_Warehouse_ID())
                .Append("]");
            return sb.ToString();
        }

        /**
         * 	Get Document Info
         *	@return document Info (untranslated)
         */
        public String GetDocumentInfo()
        {
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            return dt.GetName() + " " + GetDocumentNo();
        }

        /// <summary>
        /// Create PDF
        /// </summary>
        /// <returns>File or null</returns>
        public FileInfo CreatePDF()
        {
            try
            {
                string fileName = Get_TableName() + Get_ID() + "_" + CommonFunctions.GenerateRandomNo()
                                    + ".txt"; //.pdf
                string filePath = Path.GetTempPath() + fileName;

                //File temp = File.createTempFile(Get_TableName() + Get_ID() + "_", ".pdf");
                //FileStream fOutStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                FileInfo temp = new FileInfo(filePath);
                if (!temp.Exists)
                {
                    return CreatePDF(temp);
                }
            }
            catch (Exception e)
            {
                log.Severe("Could not create PDF - " + e.Message);
            }
            return null;
        }

        /**
         * 	Create PDF file
         *	@param file output file
         *	@return file if success
         */
        public FileInfo CreatePDF(FileInfo file)
        {
            //	ReportEngine re = ReportEngine.get (GetCtx(), ReportEngine.INVOICE, getC_Invoice_ID());
            //	if (re == null)
            //return null;
            //	return re.getPDF(file);

            using (StreamWriter sw = file.CreateText())
            {
                sw.WriteLine("Hello");
                sw.WriteLine("And");
                sw.WriteLine("Welcome");
            }

            return file;
        }



        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (GetC_DocType_ID() == 0)
            {
                MDocType[] types = MDocType.GetOfDocBaseType(GetCtx(), MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY);
                if (types.Length > 0)	//	get first
                    SetC_DocType_ID(types[0].GetC_DocType_ID());
                else
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(), "@NotFound@ @C_DocType_ID@"));
                    return false;
                }
            }

            //Set Is Adjusted
            if (!IsInternalUse())
            {
                if (Is_ValueChanged("MovementDate"))
                {
                    SetIsAdjusted(false);
                }
            }

            //	Warehouse Org
            if (newRecord
                || Is_ValueChanged("AD_Org_ID") || Is_ValueChanged("M_Warehouse_ID"))
            {
                MWarehouse wh = MWarehouse.Get(GetCtx(), GetM_Warehouse_ID());
                if (wh.GetAD_Org_ID() != GetAD_Org_ID())
                {
                    log.SaveError("WarehouseOrgConflict", "");
                    return false;
                }
            }

            return true;
        }


        /**
         * 	Set Processed.
         * 	Propergate to Lines/Taxes
         *	@param processed processed
         */
        public void SetProcessed(bool processed)
        {
            base.SetProcessed(processed);
            if (Get_ID() == 0)
                return;
            String sql = "UPDATE M_InventoryLine SET Processed='"
                + (processed ? "Y" : "N")
                + "' WHERE M_Inventory_ID=" + GetM_Inventory_ID();
            int noLine = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
            _lines = null;
            log.Fine("Processed=" + processed + " - Lines=" + noLine);
        }


        /**
         * 	Process document
         *	@param processAction document action
         *	@return true if performed
         */
        public bool ProcessIt(String processAction)
        {
            _processMsg = null;
            DocumentEngine engine = new DocumentEngine(this, GetDocStatus());
            return engine.ProcessIt(processAction, GetDocAction());
        }



        /**
         * 	Unlock Document.
         * 	@return true if success 
         */
        public bool UnlockIt()
        {
            log.Info(ToString());
            SetProcessing(false);
            return true;
        }

        /**
         * 	Invalidate Document
         * 	@return true if success 
         */
        public bool InvalidateIt()
        {
            log.Info(ToString());
            SetDocAction(DOCACTION_Prepare);
            return true;
        }

        /**
         *	Prepare Document
         * 	@return new status (In Progress or Invalid) 
         */
        public String PrepareIt()
        {
            log.Info(ToString());
            _processMsg = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_BEFORE_PREPARE);
            if (_processMsg != null)
                return DocActionVariables.STATUS_INVALID;

            //	Std Period open?
            if (!MPeriod.IsOpen(GetCtx(), GetMovementDate(), MDocBaseType.DOCBASETYPE_MATERIALPHYSICALINVENTORY))
            {
                _processMsg = "@PeriodClosed@";
                return DocActionVariables.STATUS_INVALID;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetMovementDate()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return DocActionVariables.STATUS_INVALID;
            }


            MInventoryLine[] lines = GetLines(false);
            if (lines.Length == 0)
            {
                _processMsg = "@NoLines@";
                return DocActionVariables.STATUS_INVALID;
            }

            //	TODO: Add up Amounts
            //	setApprovalAmt();


            _justPrepared = true;
            if (!DOCACTION_Complete.Equals(GetDocAction()))
                SetDocAction(DOCACTION_Complete);
            return DocActionVariables.STATUS_INPROGRESS;
        }

        /**
         * 	Approve Document
         * 	@return true if success 
         */
        public bool ApproveIt()
        {
            log.Info(ToString());
            SetIsApproved(true);
            return true;
        }

        /**
         * 	Reject Approval
         * 	@return true if success 
         */
        public bool RejectIt()
        {
            log.Info(ToString());
            SetIsApproved(false);
            return true;
        }

        /**
         * 	Complete Document
         * 	@return new status (Complete, In Progress, Invalid, Waiting ..)
         */
        public String CompleteIt()
        {
            MInventoryLine[] lines = GetLines(false);
            // Check for Is Adjusted
            if (!IsInternalUse())
            {
                //if (!IsAdjusted())
                //{
                //    _processMsg = "Please Run Update Qty First";
                //    return DocActionVariables.STATUS_INVALID;
                //}
                for (int i = 0; i < lines.Length; i++)
                {
                    MInventoryLine line = lines[i];
                    decimal currentQty = GetCurrentQty(Util.GetValueOfInt(line.GetM_Product_ID()), Util.GetValueOfInt(line.GetM_Locator_ID()),
                                                   Util.GetValueOfInt(line.GetM_AttributeSetInstance_ID()), Util.GetValueOfDateTime(GetMovementDate()));
                    line.SetQtyBook(currentQty);
                    line.SetOpeningStock(currentQty);
                    if (line.GetAdjustmentType() == "A")
                    {
                        line.SetDifferenceQty(Util.GetValueOfDecimal(line.GetOpeningStock()) - Util.GetValueOfDecimal(line.GetAsOnDateCount()));
                    }
                    else if (line.GetAdjustmentType() == "D")
                    {
                        line.SetAsOnDateCount(Util.GetValueOfDecimal(line.GetOpeningStock()) - Util.GetValueOfDecimal(line.GetDifferenceQty()));
                    }
                    line.SetQtyCount(Util.GetValueOfDecimal(line.GetQtyBook()) - Util.GetValueOfDecimal(line.GetDifferenceQty()));
                    if (!line.Save())
                    {

                    }
                    MInventory inventory = new MInventory(GetCtx(), GetM_Inventory_ID(), Get_TrxName());
                    inventory.SetIsAdjusted(true);
                    inventory.Save();
                }
            }
            //	Re-Check
            if (!_justPrepared)
            {
                String status = PrepareIt();
                if (!DocActionVariables.STATUS_INPROGRESS.Equals(status))
                    return status;
            }
            //	Implicit Approval
            if (!IsApproved())
                ApproveIt();
            log.Info(ToString());
            //
           
            for (int i = 0; i < lines.Length; i++)
            {
                MInventoryLine line = lines[i];

                if (!line.IsActive())
                    continue;

                line.CreateMA(false);

                MTransaction trx = null;
                if (line.GetM_AttributeSetInstance_ID() == 0)
                {
                    Decimal qtyDiff = Decimal.Negate(line.GetQtyInternalUse());

                    if (Env.Signum(qtyDiff) == 0)
                        qtyDiff = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                    //
                    if (Env.Signum(qtyDiff) > 0)
                    {
                        //	Storage
                        MStorage storage = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), 0, Get_TrxName());
                        if (storage == null)
                            storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                line.GetM_Product_ID(), 0, Get_TrxName());
                        Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), qtyDiff);
                        log.Fine("Diff=" + qtyDiff
                           + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew);
                        storage.SetQtyOnHand(qtyNew);
                        storage.SetDateLastInventory(GetMovementDate());
                        if (!storage.Save(Get_TrxName()))
                        {
                            _processMsg = "Storage not updated(1)";
                            return DocActionVariables.STATUS_INVALID;
                        }
                        log.Fine(storage.ToString());

                        // Done to Update Current Qty at Transaction
                        MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                        int attribSet_ID = pro.GetM_AttributeSet_ID();
                        isGetFromStorage = false;
                        if (attribSet_ID > 0)
                        {
                            sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                    + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + "  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                            {
                                trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true);
                                isGetFromStorage = true;
                            }
                        }
                        else
                        {
                            sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                     + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                            if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                            {
                                trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false);
                                isGetFromStorage = true;
                            }
                        }
                        if (!isGetFromStorage)
                        {
                            trxQty = GetProductQtyFromStorage(line);
                        }
                        // Done to Update Current Qty at Transaction

                        //	Transaction
                        trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                            MTransaction.MOVEMENTTYPE_InventoryIn,
                            line.GetM_Locator_ID(), line.GetM_Product_ID(), 0,
                            qtyDiff, GetMovementDate(), Get_TrxName());
                        trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                        trx.SetCurrentQty(trxQty + qtyDiff);
                        if (!trx.Save())
                        {
                            _processMsg = "Transaction not inserted(1)";
                            return DocActionVariables.STATUS_INVALID;
                        }

                        //Update Transaction for Current Quantity
                        Decimal currentQty = qtyDiff + trxQty.Value;
                        UpdateTransaction(line, trx, currentQty);
                        //UpdateCurrentRecord(line, trx, qtyDiff);


                    }
                    else	//	negative qty
                    {
                        MInventoryLineMA[] mas = MInventoryLineMA.Get(GetCtx(),
                            line.GetM_InventoryLine_ID(), Get_TrxName());
                        for (int j = 0; j < mas.Length; j++)
                        {
                            MInventoryLineMA ma = mas[j];
                            //	Storage
                            MStorage storage = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                                line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            if (storage == null)
                                storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                    line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            //
                            Decimal maxDiff = qtyDiff;
                            if (Env.Signum(maxDiff) < 0
                                && ma.GetMovementQty().CompareTo(Decimal.Negate(maxDiff)) < 0)
                                maxDiff = Decimal.Negate(ma.GetMovementQty());
                            Decimal qtyNew = Decimal.Add(ma.GetMovementQty(), maxDiff);	//	Storage+Diff

                            log.Fine("MA Qty=" + ma.GetMovementQty()
                                + ",Diff=" + qtyDiff + "|" + maxDiff
                                + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew
                                + " {" + ma.GetM_AttributeSetInstance_ID() + "}");
                            //
                            storage.SetQtyOnHand(qtyNew);
                            storage.SetDateLastInventory(GetMovementDate());
                            if (!storage.Save(Get_TrxName()))
                            {
                                _processMsg = "Storage not updated (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            log.Fine(storage.ToString());

                            // Done to Update Current Qty at Transaction
                            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                            int attribSet_ID = pro.GetM_AttributeSet_ID();
                            isGetFromStorage = false;
                            if (attribSet_ID > 0)
                            {
                                sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                        + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                                {
                                    trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true);
                                    isGetFromStorage = true;
                                }
                            }
                            else
                            {
                                sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                      + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                                {
                                    trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false);
                                    isGetFromStorage = true;
                                }
                            }
                            if (!isGetFromStorage)
                            {
                                trxQty = GetProductQtyFromStorage(line);
                            }
                            // Done to Update Current Qty at Transaction


                            //	Transaction
                            trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                                MTransaction.MOVEMENTTYPE_InventoryIn,
                                line.GetM_Locator_ID(), line.GetM_Product_ID(), ma.GetM_AttributeSetInstance_ID(),
                                maxDiff, GetMovementDate(), Get_TrxName());
                            trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                            trx.SetCurrentQty(trxQty + qtyDiff);
                            if (!trx.Save())
                            {
                                _processMsg = "Transaction not inserted (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            Decimal currentQty = qtyDiff + trxQty.Value;
                            UpdateTransaction(line, trx, currentQty);
                            //UpdateCurrentRecord(line, trx, qtyDiff);
                            //
                            qtyDiff = Decimal.Subtract(qtyDiff, maxDiff);
                            if (Env.Signum(qtyDiff) == 0)
                                break;
                        }
                        // nnayak - if the quantity issued was greator than the quantity onhand, we need to create a transaction 
                        // for the remaining quantity
                        if (Env.Signum(qtyDiff) != 0)
                        {
                            MStorage storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                                    line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                            Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), qtyDiff);
                            log.Fine("Count=" + line.GetQtyCount()
                                + ",Book=" + line.GetQtyBook() + ", Difference=" + qtyDiff
                                + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew);
                            //
                            storage.SetQtyOnHand(qtyNew);

                            storage.SetDateLastInventory(GetMovementDate());
                            if (!storage.Save(Get_TrxName()))
                            {
                                _processMsg = "Storage not updated (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }
                            log.Fine(storage.ToString());

                            // Done to Update Current Qty at Transaction
                            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                            int attribSet_ID = pro.GetM_AttributeSet_ID();
                            isGetFromStorage = false;
                            if (attribSet_ID > 0)
                            {
                                sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                        + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + "  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                                {
                                    trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true);
                                    isGetFromStorage = true;
                                }
                            }
                            else
                            {
                                sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                      + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                                if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                                {
                                    trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false);
                                    isGetFromStorage = true;
                                }
                            }
                            if (!isGetFromStorage)
                            {
                                trxQty = GetProductQtyFromStorage(line);
                            }
                            // Done to Update Current Qty at Transaction

                            //	Transaction
                            trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                                MTransaction.MOVEMENTTYPE_InventoryIn,
                                line.GetM_Locator_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                                qtyDiff, GetMovementDate(), Get_TrxName());
                            trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                            trx.SetCurrentQty(trxQty + qtyDiff);
                            if (!trx.Save())
                            {
                                _processMsg = "Transaction not inserted (MA)";
                                return DocActionVariables.STATUS_INVALID;
                            }

                            //Update Transaction for Current Quantity
                            Decimal currentQty = qtyDiff + trxQty.Value;
                            UpdateTransaction(line, trx, currentQty);
                            //UpdateCurrentRecord(line, trx, qtyDiff);
                            //
                        }
                    }	//	negative qty
                }

                //	Fallback
                if (trx == null)
                {
                    //	Storage
                    MStorage storage = MStorage.Get(GetCtx(), line.GetM_Locator_ID(),
                        line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                    if (storage == null)
                        storage = MStorage.GetCreate(GetCtx(), line.GetM_Locator_ID(),
                            line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(), Get_TrxName());
                    //
                    Decimal qtyDiff = Decimal.Negate(line.GetQtyInternalUse());
                    if (Env.ZERO.CompareTo(qtyDiff) == 0)
                        qtyDiff = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                    Decimal qtyNew = Decimal.Add(storage.GetQtyOnHand(), qtyDiff);
                    log.Fine("Count=" + line.GetQtyCount()
                       + ",Book=" + line.GetQtyBook() + ", Difference=" + qtyDiff
                        + " - OnHand=" + storage.GetQtyOnHand() + "->" + qtyNew);
                    //
                    storage.SetQtyOnHand(qtyNew);
                    storage.SetDateLastInventory(GetMovementDate());
                    if (!storage.Save(Get_TrxName()))
                    {
                        _processMsg = "Storage not updated(2)";
                        return DocActionVariables.STATUS_INVALID;
                    }
                    log.Fine(storage.ToString());

                    // Done to Update Current Qty at Transaction
                    MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
                    int attribSet_ID = pro.GetM_AttributeSet_ID();
                    isGetFromStorage = false;
                    if (attribSet_ID > 0)
                    {
                        sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + " AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                        {
                            trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), true);
                            isGetFromStorage = true;
                        }
                    }
                    else
                    {
                        sql = @"SELECT COUNT(*)   FROM m_transaction
                                    WHERE IsActive = 'Y' AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                                      + " AND M_AttributeSetInstance_ID = 0  AND movementdate <= " + GlobalVariable.TO_DATE(GetMovementDate(), true);
                        if (Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null)) > 0)
                        {
                            trxQty = GetProductQtyFromTransaction(line, GetMovementDate(), false);
                            isGetFromStorage = true;
                        }
                    }
                    if (!isGetFromStorage)
                    {
                        trxQty = GetProductQtyFromStorage(line);
                    }
                    // Done to Update Current Qty at Transaction

                    //	Transaction
                    trx = new MTransaction(GetCtx(), line.GetAD_Org_ID(),
                        MTransaction.MOVEMENTTYPE_InventoryIn,
                        line.GetM_Locator_ID(), line.GetM_Product_ID(), line.GetM_AttributeSetInstance_ID(),
                        qtyDiff, GetMovementDate(), Get_TrxName());
                    trx.SetM_InventoryLine_ID(line.GetM_InventoryLine_ID());
                    trx.SetCurrentQty(trxQty + qtyDiff);
                    if (!trx.Save())
                    {
                        _processMsg = "Transaction not inserted(2)";
                        return DocActionVariables.STATUS_INVALID;
                    }

                    //Update Transaction for Current Quantity
                    Decimal currentQty = qtyDiff + trxQty.Value;
                    UpdateTransaction(line, trx, currentQty);
                    //UpdateCurrentRecord(line, trx, qtyDiff);
                }	//	Fallback
                /************************************************************************************************************/
                Tuple<String, String, String> mInfo = null;
                if (Env.HasModulePrefix("DTD001_", out mInfo))
                {
                    if (line.GetM_RequisitionLine_ID() > 0)
                    {
                        MRequisitionLine reqLine = new MRequisitionLine(GetCtx(), line.GetM_RequisitionLine_ID(), Get_Trx());
                        reqLine.SetDTD001_DeliveredQty(Decimal.Add(reqLine.GetDTD001_DeliveredQty(), line.GetQtyInternalUse()));
                        reqLine.Save();

                        MStorage newsg = MStorage.Get(GetCtx(), line.GetM_Locator_ID(), line.GetM_Product_ID(), 0, Get_Trx());
                        newsg.SetDTD001_QtyReserved(Decimal.Subtract(newsg.GetDTD001_QtyReserved(), line.GetQtyInternalUse()));
                        if (!newsg.Save())
                        {
                            _processMsg = "Storage Not Updated";
                            return DocActionVariables.STATUS_INVALID;
                        }
                    }
                }
                /**********************************************************************************************************/


            }	//	for all lines

            //	User Validation
            String valid = ModelValidationEngine.Get().FireDocValidate(this, ModalValidatorVariables.DOCTIMING_AFTER_COMPLETE);
            if (valid != null)
            {
                _processMsg = valid;
                return DocActionVariables.STATUS_INVALID;
            }
            //
            SetProcessed(true);
            SetDocAction(DOCACTION_Close);
            return DocActionVariables.STATUS_COMPLETED;
        }

        //this method is used to get opening balance from transaction based on parameter
        private decimal GetCurrentQty(int M_Product_ID, int M_Locator_ID, int M_AttributeSetInstance_ID, DateTime? movementDate)
        {
            decimal currentQty = 0;
            string query = "";
            int result = 0;
            query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
            result = Util.GetValueOfInt(DB.ExecuteScalar(query));
            if (result > 0)
            {
                query = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query));
            }
            else
            {
                query = "SELECT COUNT(*) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                result = Util.GetValueOfInt(DB.ExecuteScalar(query));
                if (result > 0)
                {
                    query = @"SELECT currentqty FROM M_Transaction WHERE M_Transaction_ID =
                            (SELECT MAX(M_Transaction_ID)   FROM M_Transaction
                            WHERE movementdate =     (SELECT MAX(movementdate) FROM M_Transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID + @")
                            AND  M_Product_ID = " + M_Product_ID + " AND M_Locator_ID = " + M_Locator_ID + " AND M_AttributeSetInstance_ID = " + M_AttributeSetInstance_ID;
                    currentQty = Util.GetValueOfDecimal(DB.ExecuteScalar(query));
                }
            }
            return currentQty;
        }

        /// <summary>
        /// Update Transaction Tab to set Current Qty
        /// </summary>
        /// <param name="line"></param>
        /// <param name="trx"></param>
        /// <param name="qtyDiff"></param>
        private void UpdateTransaction(MInventoryLine line, MTransaction trxM, decimal qtyDiffer)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            MTransaction trx = null;
            MInventoryLine inventoryLine = null;
            MInventory inventory = null;
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";
            DataSet ds = new DataSet();
            try
            {
                if (attribSet_ID > 0)
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + qtyDiffer + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true)
                               + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID()
                               + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }
                else
                {
                    //sql = "UPDATE M_Transaction SET CurrentQty = MovementQty + " + qtyDiffer + " WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //    + " AND M_AttributeSetInstance_ID = 0 ";
                    sql = @"SELECT M_AttributeSetInstance_ID ,  M_Locator_ID ,  M_Product_ID ,  movementqty ,  currentqty ,  movementdate ,  TO_CHAR(Created, 'DD-MON-YY HH24:MI:SS') , m_transaction_id , MovementType , M_InventoryLine_ID
                              FROM m_transaction WHERE movementdate >= " + GlobalVariable.TO_DATE(trxM.GetMovementDate().Value.AddDays(1), true)
                               + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 "
                               + " ORDER BY movementdate ASC , m_transaction_id ASC , created ASC";
                }

                ds = DB.ExecuteDataset(sql, null, Get_TrxName());
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int i = 0;
                        for (i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            if (Util.GetValueOfString(ds.Tables[0].Rows[i]["MovementType"]) == "I+" &&
                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]) > 0)
                            {
                                inventoryLine = new MInventoryLine(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]), Get_TrxName());
                                inventory = new MInventory(GetCtx(), Util.GetValueOfInt(inventoryLine.GetM_Inventory_ID()), null);
                                if (!inventory.IsInternalUse())
                                {
                                    //break;
                                    inventoryLine.SetQtyBook(qtyDiffer);
                                    inventoryLine.SetOpeningStock(qtyDiffer);
                                    inventoryLine.SetDifferenceQty(Decimal.Subtract(qtyDiffer, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"])));
                                    if (!inventoryLine.Save())
                                    {
                                        log.Info("Quantity Book and Quantity Differenec Not Updated at Inventory Line Tab <===> " + Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_InventoryLine_ID"]));
                                    }

                                    trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                                    trx.SetMovementQty(Decimal.Negate(Decimal.Subtract(qtyDiffer, Util.GetValueOfDecimal(ds.Tables[0].Rows[i]["currentqty"]))));
                                    if (!trx.Save())
                                    {
                                        log.Info("Movement Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                                    }
                                    else
                                    {
                                        qtyDiffer = trx.GetCurrentQty();
                                    }
                                    if (i == ds.Tables[0].Rows.Count - 1)
                                    {
                                        MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        if (storage == null)
                                        {
                                            storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                     Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                        }
                                        if (storage.GetQtyOnHand() != qtyDiffer)
                                        {
                                            storage.SetQtyOnHand(qtyDiffer);
                                            storage.Save();
                                        }
                                    }
                                    continue;
                                }
                            }
                            trx = new MTransaction(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]), Get_TrxName());
                            trx.SetCurrentQty(qtyDiffer + trx.GetMovementQty());
                            if (!trx.Save())
                            {
                                log.Info("Current Quantity Not Updated at Transaction Tab for this ID" + Util.GetValueOfInt(ds.Tables[0].Rows[i]["m_transaction_id"]));
                            }
                            else
                            {
                                qtyDiffer = trx.GetCurrentQty();
                            }
                            if (i == ds.Tables[0].Rows.Count - 1)
                            {
                                MStorage storage = MStorage.Get(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                                  Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                if (storage == null)
                                {
                                    storage = MStorage.GetCreate(GetCtx(), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Locator_ID"]),
                                                             Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_Product_ID"]), Util.GetValueOfInt(ds.Tables[0].Rows[i]["M_AttributeSetInstance_ID"]), Get_TrxName());
                                }
                                if (storage.GetQtyOnHand() != qtyDiffer)
                                {
                                    storage.SetQtyOnHand(qtyDiffer);
                                    storage.Save();
                                }
                            }
                        }
                    }
                }
                ds.Dispose();
            }
            catch
            {
                if (ds != null)
                {
                    ds.Dispose();
                }
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }

        private void UpdateCurrentRecord(MInventoryLine line, MTransaction trxM, decimal qtyDiffer)
        {
            MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            int attribSet_ID = pro.GetM_AttributeSet_ID();
            string sql = "";

            try
            {
                if (attribSet_ID > 0)
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }

                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
                    //     + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                }
                else
                {
                    sql = @"SELECT Count(*) from M_Transaction  WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                    int count = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                    if (count > 0)
                    {
                        sql = @"SELECT count(*)  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + "  and m_locator_ID=" + line.GetM_Locator_ID() + " )order by m_transaction_id desc";
                        int recordcount = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        if (recordcount > 0)
                        {
                            sql = @"SELECT tr.currentqty  FROM m_transaction tr  WHERE tr.movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + @" and
                     tr.m_product_id =" + line.GetM_Product_ID() + "  and tr.m_locator_ID=" + line.GetM_Locator_ID() + @" and tr.movementdate in (select max(movementdate) from m_transaction where
                     movementdate<=" + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " and m_product_id =" + line.GetM_Product_ID() + " and m_locator_ID=" + line.GetM_Locator_ID() + ") order by m_transaction_id desc";

                            Decimal? quantity = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
                            trxM.SetCurrentQty(Util.GetValueOfDecimal(Decimal.Add(Util.GetValueOfDecimal(quantity), Util.GetValueOfDecimal(qtyDiffer))));
                            if (!trxM.Save())
                            {

                            }
                        }
                        else
                        {
                            trxM.SetCurrentQty(qtyDiffer);
                            if (!trxM.Save())
                            {

                            }
                        }
                        //trxM.SetCurrentQty(

                    }
                    //sql = "UPDATE M_Transaction SET CurrentQty = CurrentQty + " + qtyDiffer + " WHERE MovementDate > " + GlobalVariable.TO_DATE(trxM.GetMovementDate(), true) + " AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
                }

                // int countUpd = Util.GetValueOfInt(DB.ExecuteQuery(sql, null, Get_TrxName()));
            }
            catch
            {
                log.Info("Current Quantity Not Updated at Transaction Tab");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromStorage(MInventoryLine line)
        {
            return 0;
            //MProduct pro = new MProduct(Env.GetCtx(), line.GetM_Product_ID(), Get_TrxName());
            //int attribSet_ID = pro.GetM_AttributeSet_ID();
            //string sql = "";

            //if (attribSet_ID > 0)
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID()
            //         + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
            //}
            //else
            //{
            //    sql = @"SELECT SUM(qtyonhand) FROM M_Storage WHERE M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID();
            //}
            //return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="movementDate"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        private decimal? GetProductQtyFromTransaction(MInventoryLine line, DateTime? movementDate, bool isAttribute)
        {
            decimal result = 0;
            string sql = "";

            if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id  =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID())) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id  =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID() + @")
                           AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = " + line.GetM_AttributeSetInstance_ID();
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate = " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ")) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate <= " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            else if (!isAttribute && Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @" 
                                         AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND M_AttributeSetInstance_ID = 0 ")) > 0)
            {
                sql = @"SELECT currentqty FROM m_transaction WHERE m_transaction_id =
                        (SELECT MAX(m_transaction_id)   FROM m_transaction
                          WHERE movementdate =     (SELECT MAX(movementdate) FROM m_transaction WHERE movementdate < " + GlobalVariable.TO_DATE(movementDate, true) + @"
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND  M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) " + @")
                          AND M_Product_ID = " + line.GetM_Product_ID() + " AND M_Locator_ID = " + line.GetM_Locator_ID() + " AND ( M_AttributeSetInstance_ID = 0 OR M_AttributeSetInstance_ID IS NULL ) ";
                result = Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, Get_TrxName()));
            }
            return result;
        }

        /**
         * 	Check Material Policy.
         * 	(NOT USED)
         * 	Sets line ASI
         */
        private void CheckMaterialPolicy()
        {
            int no = MInventoryLineMA.DeleteInventoryMA(GetM_Inventory_ID(), Get_TrxName());
            if (no > 0)
            {
                log.Config("Delete old #" + no);
            }
            MInventoryLine[] lines = GetLines(false);

            //	Incoming Trx
            MClient client = MClient.Get(GetCtx());

            //	Check Lines
            for (int i = 0; i < lines.Length; i++)
            {
                MInventoryLine line = lines[i];
                bool needSave = false;

                //	Attribute Set Instance
                if (line.GetM_AttributeSetInstance_ID() == 0)
                {
                    MProduct product = MProduct.Get(GetCtx(), line.GetM_Product_ID());
                    Decimal qtyDiff = Decimal.Negate(line.GetQtyInternalUse());
                    if (Env.ZERO.CompareTo(qtyDiff) == 0)
                        qtyDiff = Decimal.Subtract(line.GetQtyCount(), line.GetQtyBook());
                    log.Fine("Count=" + line.GetQtyCount()
                       + ",Book=" + line.GetQtyBook() + ", Difference=" + qtyDiff);
                    if (Env.Signum(qtyDiff) > 0)	//	In
                    {
                        MAttributeSetInstance asi = new MAttributeSetInstance(GetCtx(), 0, Get_TrxName());
                        asi.SetClientOrg(GetAD_Client_ID(), 0);
                        asi.SetM_AttributeSet_ID(product.GetM_AttributeSet_ID());
                        if (asi.Save())
                        {
                            line.SetM_AttributeSetInstance_ID(asi.GetM_AttributeSetInstance_ID());
                            needSave = true;
                        }
                    }
                    else	//	Outgoing Trx
                    {
                        MProductCategory pc = MProductCategory.Get(GetCtx(), product.GetM_Product_Category_ID());
                        String MMPolicy = pc.GetMMPolicy();
                        if (MMPolicy == null || MMPolicy.Length == 0)
                            MMPolicy = client.GetMMPolicy();
                        //
                        MStorage[] storages = MStorage.GetAllWithASI(GetCtx(),
                            line.GetM_Product_ID(), line.GetM_Locator_ID(),
                            MClient.MMPOLICY_FiFo.Equals(MMPolicy), Get_TrxName());
                        Decimal qtyToDeliver = Decimal.Negate(qtyDiff);
                        for (int ii = 0; ii < storages.Length; ii++)
                        {
                            MStorage storage = storages[ii];
                            if (ii == 0)
                            {
                                if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                                {
                                    line.SetM_AttributeSetInstance_ID(storage.GetM_AttributeSetInstance_ID());
                                    needSave = true;
                                    log.Config("Direct - " + line);
                                    qtyToDeliver = Env.ZERO;
                                }
                                else
                                {
                                    log.Config("Split - " + line);
                                    MInventoryLineMA ma = new MInventoryLineMA(line,
                                        storage.GetM_AttributeSetInstance_ID(), Decimal.Negate(storage.GetQtyOnHand()));
                                    if (!ma.Save())
                                    {
                                        ;
                                    }
                                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                                    log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                                }
                            }
                            else	//	 create addl material allocation
                            {
                                MInventoryLineMA ma = new MInventoryLineMA(line,
                                    storage.GetM_AttributeSetInstance_ID(),
                                    Decimal.Negate(qtyToDeliver));
                                if (storage.GetQtyOnHand().CompareTo(qtyToDeliver) >= 0)
                                    qtyToDeliver = Env.ZERO;
                                else
                                {
                                    ma.SetMovementQty(Decimal.Negate(storage.GetQtyOnHand()));
                                    qtyToDeliver = Decimal.Subtract(qtyToDeliver, storage.GetQtyOnHand());
                                }
                                if (!ma.Save())
                                {
                                    ;
                                }
                                log.Fine("#" + ii + ": " + ma + ", QtyToDeliver=" + qtyToDeliver);
                            }
                            if (Env.Signum(qtyToDeliver) == 0)
                                break;
                        }	//	 for all storages

                        //	No AttributeSetInstance found for remainder
                        if (Env.Signum(qtyToDeliver) != 0)
                        {
                            MInventoryLineMA ma = new MInventoryLineMA(line,
                                0, Decimal.Negate(qtyToDeliver));
                            if (!ma.Save())
                            {
                                ;
                            }
                            log.Fine("##: " + ma);
                        }
                    }	//	outgoing Trx
                }	//	attributeSetInstance

                if (needSave && !line.Save())
                {
                    log.Severe("NOT saved " + line);
                }
            }	//	for all lines

        }

        /**
         * 	Void Document.
         * 	@return false 
         */
        public bool VoidIt()
        {
            log.Info(ToString());
            if (DOCSTATUS_Closed.Equals(GetDocStatus())
                || DOCSTATUS_Reversed.Equals(GetDocStatus())
                || DOCSTATUS_Voided.Equals(GetDocStatus()))
            {
                _processMsg = "Document Closed: " + GetDocStatus();
                return false;
            }

            //	Not Processed
            if (DOCSTATUS_Drafted.Equals(GetDocStatus())
                || DOCSTATUS_Invalid.Equals(GetDocStatus())
                || DOCSTATUS_InProgress.Equals(GetDocStatus())
                || DOCSTATUS_Approved.Equals(GetDocStatus())
                || DOCSTATUS_NotApproved.Equals(GetDocStatus()))
            {
                //	Set lines to 0
                MInventoryLine[] lines = GetLines(false);
                for (int i = 0; i < lines.Length; i++)
                {
                    MInventoryLine line = lines[i];
                    Decimal oldCount = line.GetQtyCount();
                    Decimal oldInternal = line.GetQtyInternalUse();
                    if (oldCount.CompareTo(line.GetQtyBook()) != 0
                        || Env.Signum(oldInternal) != 0)
                    {
                        line.SetQtyInternalUse(Env.ZERO);
                        line.SetQtyCount(line.GetQtyBook());
                        line.AddDescription("Void (" + oldCount + "/" + oldInternal + ")");
                        line.Save(Get_TrxName());
                    }
                }
            }
            else
            {
                return ReverseCorrectIt();
            }

            SetProcessed(true);
            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Close Document.
         * 	@return true if success 
         */
        public bool CloseIt()
        {
            log.Info(ToString());

            SetDocAction(DOCACTION_None);
            return true;
        }

        /**
         * 	Reverse Correction
         * 	@return false 
         */
        public bool ReverseCorrectIt()
        {
            log.Info(ToString());
            MDocType dt = MDocType.Get(GetCtx(), GetC_DocType_ID());
            if (!MPeriod.IsOpen(GetCtx(), GetMovementDate(), dt.GetDocBaseType()))
            {
                _processMsg = "@PeriodClosed@";
                return false;
            }

            // is Non Business Day?
            if (MNonBusinessDay.IsNonBusinessDay(GetCtx(), GetMovementDate()))
            {
                _processMsg = Common.Common.NONBUSINESSDAY;
                return false;
            }


            //	Deep Copy
            MInventory reversal = new MInventory(GetCtx(), 0, Get_TrxName());
            CopyValues(this, reversal, GetAD_Client_ID(), GetAD_Org_ID());
            reversal.SetDocStatus(DOCSTATUS_Drafted);
            reversal.SetDocAction(DOCACTION_Complete);
            reversal.SetIsApproved(false);
            reversal.SetPosted(false);
            reversal.SetProcessed(false);
            reversal.AddDescription("{->" + GetDocumentNo() + ")");
            if (!reversal.Save())
            {
                _processMsg = "Could not create Inventory Reversal";
                return false;
            }

            //	Reverse Line Qty
            MInventoryLine[] oLines = GetLines(true);
            for (int i = 0; i < oLines.Length; i++)
            {
                MInventoryLine oLine = oLines[i];
                MInventoryLine rLine = new MInventoryLine(GetCtx(), 0, Get_TrxName());
                CopyValues(oLine, rLine, oLine.GetAD_Client_ID(), oLine.GetAD_Org_ID());
                rLine.SetM_Inventory_ID(reversal.GetM_Inventory_ID());
                rLine.SetParent(reversal);
                //
                rLine.SetQtyBook(oLine.GetQtyCount());		//	switch
                rLine.SetQtyCount(oLine.GetQtyBook());
                rLine.SetQtyInternalUse(Decimal.Negate(oLine.GetQtyInternalUse()));
                if (!rLine.Save())
                {
                    _processMsg = "Could not create Inventory Reversal Line";
                    return false;
                }
            }
            if (!IsInternalUse())
            {
                reversal.SetIsAdjusted(true);
                if (!reversal.Save())
                {
                    _processMsg = "Could not update Inventory Reversal";
                    return false;
                }
            }
            //
            if (!reversal.ProcessIt(DocActionVariables.ACTION_COMPLETE))
            {
                _processMsg = "Reversal ERROR: " + reversal.GetProcessMsg();
                return false;
            }
            reversal.CloseIt();
            reversal.SetDocStatus(DOCSTATUS_Reversed);
            reversal.SetDocAction(DOCACTION_None);
            reversal.Save();
            _processMsg = reversal.GetDocumentNo();

            //	Update Reversed (this)
            AddDescription("(" + reversal.GetDocumentNo() + "<-)");
            SetProcessed(true);
            SetDocStatus(DOCSTATUS_Reversed);	//	may come from void
            SetDocAction(DOCACTION_None);

            return true;
        }

        /**
         * 	Reverse Accrual
         * 	@return false 
         */
        public bool ReverseAccrualIt()
        {
            log.Info(ToString());
            return false;
        }

        /** 
         * 	Re-activate
         * 	@return false 
         */
        public bool ReActivateIt()
        {
            log.Info(ToString());
            return false;
        }


        /*
         * 	Get Summary
         *	@return Summary of Document
         */
        public String GetSummary()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetDocumentNo());
            //	: Total Lines = 123.00 (#1)
            sb.Append(": ")
                .Append(Msg.Translate(GetCtx(), "ApprovalAmt")).Append("=").Append(GetApprovalAmt())
                .Append(" (#").Append(GetLines(false).Length).Append(")");
            //	 - Description
            if (GetDescription() != null && GetDescription().Length > 0)
                sb.Append(" - ").Append(GetDescription());
            return sb.ToString();
        }

        /**
         * 	Get Process Message
         *	@return clear text error message
         */
        public String GetProcessMsg()
        {
            return _processMsg;
        }

        /**
         * 	Get Document Owner (Responsible)
         *	@return AD_User_ID
         */
        public int GetDoc_User_ID()
        {
            return GetUpdatedBy();
        }

        /**
         * 	Get Document Currency
         *	@return C_Currency_ID
         */
        public int GetC_Currency_ID()
        {
            //	MPriceList pl = MPriceList.get(GetCtx(), getM_PriceList_ID());
            //	return pl.getC_Currency_ID();
            return 0;
        }


        #region DocAction Members


        public Env.QueryParams GetLineOrgsQueryInfo()
        {
            return null;
        }

        public DateTime? GetDocumentDate()
        {
            return null;
        }

        public string GetDocBaseType()
        {
            return null;
        }

       

        public void SetProcessMsg(string processMsg)
        {

        }



        #endregion
    }
}
