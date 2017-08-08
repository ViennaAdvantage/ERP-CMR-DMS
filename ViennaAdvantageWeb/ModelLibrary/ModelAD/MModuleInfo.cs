using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MModuleInfo : X_AD_ModuleInfo
    {
        public MModuleInfo(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        public MModuleInfo(Ctx ctx, int id, Trx trxName)
            : base(ctx, id, trxName)
        {
        }

        protected override bool BeforeSave(bool newRecord)
        {
            string prefix = "", sql = "";
            int prefixCount = 0;

            if (GetPrefix() != null)
            {
                prefix = GetPrefix().ToUpper();
            }

            if (prefix != "")
            {
                sql = "SELECT COUNT(prefix) FROM AD_ModuleInfo WHERE UPPER(prefix) = '" + prefix + "'";
                //prefixCount = Convert.ToInt32(DB.ExecuteScalar(Sql));
            }


            if (prefix == "VIS_")
            {
                sql += " AND  UPPER(Name) =  '" + GetName().ToUpper() + "'";
            }



            if (base.Get_ColumnIndex("ModuleTechnology") > -1) // sb cloud database
            {
                string mTech = base.Get_Value("ModuleTechnology").ToString();
                sql += " AND ModuleTechnology = " + mTech;
            }




            prefixCount = Convert.ToInt32(DB.ExecuteScalar(sql));


            if ((newRecord && prefixCount > 0) || prefixCount > 1)
            {
                log.SaveError("PrefixNotAvailable", "", false);
                return false;
            }



            //Check Assembly Name
            string assemblyname = "";
            if (GetAssemblyName() != null)
            {
                assemblyname = GetAssemblyName().ToUpper();
            }

            int asmCount = 0;
            if (assemblyname != "")
            {
                sql = "SELECT COUNT(assemblyname) FROM AD_ModuleInfo WHERE UPPER(assemblyname)='" + assemblyname + "'";



                if (prefix == "VIS_")
                {
                    sql += " AND  UPPER(Name) =  '" + GetName().ToUpper() + "'";
                }

                if (base.Get_ColumnIndex("ModuleTechnology") > -1) // sb cloud database
                {
                    string mTech = base.Get_Value("ModuleTechnology").ToString();
                    sql += " AND ModuleTechnology = " + mTech;
                }



                asmCount = Convert.ToInt32(DB.ExecuteScalar(sql));

                if ((newRecord && asmCount > 0) || asmCount>1)
                {
                    log.SaveError("AssemblyNameNotAvailable", "", false);
                    //ShowMessage.Info("AssemblyNameNotAvailable", true, "", null);
                    return false;
                }
            }

            return base.BeforeSave(newRecord);
        }
    }
}
