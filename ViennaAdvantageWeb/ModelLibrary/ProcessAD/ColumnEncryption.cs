using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Model;
using VAdvantage.Classes;
using VAdvantage.Utility;


namespace VAdvantage.Process
{
    public class ColumnEncryption : SvrProcess
    {
        /** Enable/Disable Encryption		*/
        private bool p_IsEncrypted = false;
        /** Change Encryption Settings		*/
        private bool p_ChangeSetting = false;
        /** Maximum Length					*/
        private int p_MaxLength = 0;
        /** Test Value						*/
        private String p_TestValue = null;
        /** The Column						*/
        private int p_AD_Column_ID = 0;

        /**
         *  Prepare - e.g., get Parameters.
         */
        protected override void Prepare()
        {
           
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else if (name.Equals("IsEncrypted"))
                    p_IsEncrypted = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("ChangeSetting"))
                    p_ChangeSetting = "Y".Equals(para[i].GetParameter());
                else if (name.Equals("MaxLength"))
                    p_MaxLength = para[i].GetParameterAsInt();
                else if (name.Equals("TestValue"))
                    p_TestValue = (String)para[i].GetParameter();
                else
                    log.Log(VAdvantage.Logging.Level.SEVERE, "Unknown Parameter: " + name);
            }
            p_AD_Column_ID = GetRecord_ID();
        }	//	prepare

        /**
         * 	Process
         *	@return info
         *	@throws Exception
         */
        protected override String DoIt()// throws Exception
        {
            log.Info("AD_Column_ID=" + p_AD_Column_ID
                + ", IsEncrypted=" + p_IsEncrypted
                + ", ChangeSetting=" + p_ChangeSetting
                + ", MaxLength=" + p_MaxLength);
            MColumn column = new MColumn(GetCtx(), p_AD_Column_ID, null);
            if (column.Get_ID() == 0 || column.Get_ID() != p_AD_Column_ID)
                throw new Exception("@NotFound@ @AD_Column_ID@ - " + p_AD_Column_ID);
            //
            String columnName = column.GetColumnName();
            int dt = column.GetAD_Reference_ID();

            //	Can it be enabled?
            if (column.IsKey()
                || column.IsParent()
                || column.IsStandardColumn()
                || column.IsVirtualColumn()
                || column.IsIdentifier()
                || column.IsTranslated()
                || DisplayType.IsLookup(dt)
                || DisplayType.IsLOB(dt)
                || "DocumentNo".Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase)
                || "Value".Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase)
                || "Name".Equals(column.GetColumnName(), StringComparison.OrdinalIgnoreCase))
            {
                if (column.IsEncrypted())
                {
                    column.SetIsEncrypted(false);
                    column.Save();
                }
                return columnName + ": cannot be encrypted";
            }

            //	Start
            AddLog(0, null, null, "Encryption Class = " + SecureEngineUtility.SecureEngine.GetClassName());
            bool error = false;

            //	Test Value
            if (p_TestValue != null && p_TestValue.Length > 0)
            {
                String encString = SecureEngineUtility.SecureEngine.Encrypt(p_TestValue);
                AddLog(0, null, null, "Encrypted Test Value=" + encString);
                String clearString = SecureEngineUtility.SecureEngine.Decrypt(encString);
                if (p_TestValue.Equals(clearString))
                    AddLog(0, null, null, "Decrypted=" + clearString
                        + " (same as test value)");
                else
                {
                    AddLog(0, null, null, "Decrypted=" + clearString
                        + " (NOT the same as test value - check algorithm)");
                    error = true;
                }
                int encLength = encString.Length;
                AddLog(0, null, null, "Test Length=" + p_TestValue.Length + " -> " + encLength);
                if (encLength <= column.GetFieldLength())
                    AddLog(0, null, null, "Encrypted Length (" + encLength
                        + ") fits into field (" + column.GetFieldLength() + ")");
                else
                {
                    AddLog(0, null, null, "Encrypted Length (" + encLength
                        + ") does NOT fit into field (" + column.GetFieldLength() + ") - resize field");
                    error = true;
                }
            }

            //	Length Test
            if (p_MaxLength != 0)
            {
                String testClear = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                while (testClear.Length < p_MaxLength)
                    testClear += testClear;
                testClear = testClear.Substring(0, p_MaxLength);
                log.Config("Test=" + testClear + " (" + p_MaxLength + ")");
                //
                String encString = SecureEngineUtility.SecureEngine.Encrypt(testClear);
                int encLength = encString.Length;
                AddLog(0, null, null, "Test Max Length=" + testClear.Length + " -> " + encLength);
                if (encLength <= column.GetFieldLength())
                    AddLog(0, null, null, "Encrypted Max Length (" + encLength
                        + ") fits into field (" + column.GetFieldLength() + ")");
                else
                {
                    AddLog(0, null, null, "Encrypted Max Length (" + encLength
                        + ") does NOT fit into field (" + column.GetFieldLength() + ") - resize field");
                    error = true;
                }
            }

            if (p_IsEncrypted != column.IsEncrypted())
            {
                if (error || !p_ChangeSetting)
                    AddLog(0, null, null, "Encryption NOT changed - Encryption=" + column.IsEncrypted());
                else
                {
                    column.SetIsEncrypted(p_IsEncrypted);
                    if (column.Save())
                        AddLog(0, null, null, "Encryption CHANGED - Encryption=" + column.IsEncrypted());
                    else
                        AddLog(0, null, null, "Save Error");
                }
            }
            return "Encryption=" + column.IsEncrypted();
        }
    }
}
