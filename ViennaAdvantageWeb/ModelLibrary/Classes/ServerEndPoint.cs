using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using VAdvantage.Utility;


namespace VAdvantage.Classes
{
    public class ServerEndPoint
    {

        public static ModelLibrary.CloudService.ServiceSoapClient GetCloudClient()
        {

            return null;

        }



        /// <summary>
        /// Get Access key
        /// </summary>
        /// <returns>path</returns>
        public static string GetAccesskey()
        {
            return null;
        }

        public static bool IsSSLEnabled()
        {
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsSSLEnabled"] != null && System.Web.Configuration.WebConfigurationManager.AppSettings["IsSSLEnabled"].ToString() != "")
            {
                if (System.Web.Configuration.WebConfigurationManager.AppSettings["IsSSLEnabled"].ToString() == "Y")
                {
                    return true;
                }
            }
            return false;
        }

        internal static String GetDeveloperKey()
        {

            return null;
        }

        internal static string GetYTUser()
        {
            return null;
        }




    }
}
