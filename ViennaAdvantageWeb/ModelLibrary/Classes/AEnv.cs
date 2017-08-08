/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : 
 * Chronological Development
 * Veena Pandey     28-May-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Model;
using VAdvantage.Process;
using VAdvantage.SqlExec;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;
using VAdvantage.Classes;
using VAdvantage.Common;

using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using VAdvantage.Controller;

namespace VAdvantage.Classes
{
    /// <summary>
    /// 
    /// </summary>
    public class AEnv
    {
        /** Workflow Menu		*/
       // private static bool? _workflow = null;
        /** Workflow Menu		*/
        //private static int workflowWindowID = 0;

        private static VLogger log = VLogger.GetVLogger(typeof(AEnv).FullName);


        /**	Window Cache		*/
        private static CCache<int, GridWindowVO> s_windows
            = new CCache<int, GridWindowVO>("AD_Window", 10);

        /// <summary>
        /// Get Window Model object from cache 
        /// </summary>
        /// <param name="WindowNo">window Number </param>
        /// <param name="AD_Window_ID">window id </param>
        /// <param name="AD_Menu_ID"> menu id </param>
        /// <returns>return windowVo if found</returns>
        public static GridWindowVO GetMWindowVO(Ctx ctx, int windowNo, int AD_Window_ID, int AD_Menu_ID)
        {
            log.Config("Window=" + windowNo + ", AD_Window_ID=" + AD_Window_ID);
            GridWindowVO mWindowVO = null;
            if (AD_Window_ID != 0 && Ini.IsCacheWindow())	//	try cache
            {
                mWindowVO = s_windows[AD_Window_ID];
                if (mWindowVO != null)
                {
                    mWindowVO = mWindowVO.Clone(windowNo);
                    log.Info("Cached=" + mWindowVO);
                }
            }

            //  Create Window Model on Client
            if (mWindowVO == null)
            {
                log.Config("create local");
                mWindowVO = GridWindowVO.Create(ctx, windowNo, AD_Window_ID, AD_Menu_ID);
                if (mWindowVO != null)
                    s_windows[AD_Window_ID] = mWindowVO;
            }	//	from Client
            if (mWindowVO == null)
                return null;

            //  Check (remote) context
            //Utility.Ctx ctx = Utility.Env.GetContext();
            if (!mWindowVO.GetCtx().Equals(ctx))
            {
            }
            return mWindowVO;
        }   //  getWindow

        public static bool CheckServerConnection(string hostNameOrAddress = null)
        {
            bool pingStatus = false;

            using (System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping())
            {
                string data = "aaaaaa";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                try
                {
                    System.Net.NetworkInformation.PingReply reply = p.Send(hostNameOrAddress ?? "ViennaSolutions.com", timeout, buffer);
                    pingStatus = (reply.Status == System.Net.NetworkInformation.IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }
            return pingStatus;
        }


        public static bool DnsTest(string dns = null)
        {
            try
            {
                System.Net.IPHostEntry ipHe = System.Net.Dns.GetHostEntry(dns);
                return true;
            }
            catch
            {
                return false;
            }
        }




    }

}
