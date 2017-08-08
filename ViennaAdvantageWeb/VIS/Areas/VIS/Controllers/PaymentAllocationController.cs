using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class PaymentAllocationController : Controller
    {
        //
        // GET: /VIS/PaymentAllocation/
        public ActionResult Index()
        {
            return View();
        }

        public string SaveCashData(string paymentData, string cashData, string invoiceData, string currency, bool isCash, int _C_BPartner_ID, int _windowNo, string payment, string DateTrx,
            string appliedamt, string discount, string writeOff, string open)
        {

            List<Dictionary<string, string>> pData = null;
            List<Dictionary<string, string>> cData = null;
            List<Dictionary<string, string>> iData = null;
            Ctx ct = Session["ctx"] as Ctx;
            DateTime date = Convert.ToDateTime(DateTrx);
            if (paymentData != null)
            {
                pData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(paymentData);
            }
            if (cashData != null)
            {
                cData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(cashData);
            }
            if (paymentData != null)
            {
                iData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(invoiceData);
            }


            PaymentAllocation payments = new PaymentAllocation(ct);
            payments.SaveCashData(pData, cData, iData, currency, isCash, _C_BPartner_ID, _windowNo, payment, date, appliedamt, discount, writeOff, open);

            return "";
        }

        public string SavePaymentData(string paymentData, string cashData, string invoiceData, string currency, bool isCash, int _C_BPartner_ID, int _windowNo, string payment, string DateTrx,
        string appliedamt, string discount, string writeOff, string open)
        {
            List<Dictionary<string, string>> pData = null;
            List<Dictionary<string, string>> cData = null;
            List<Dictionary<string, string>> iData = null;
            Ctx ct = Session["ctx"] as Ctx;
            DateTime date = Convert.ToDateTime(DateTrx);
            if (paymentData != null)
            {
                pData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(paymentData);
            }
            if (cashData != null)
            {
                cData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(cashData);
            }
            if (paymentData != null)
            {
                iData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(invoiceData);
            }


            PaymentAllocation payments = new PaymentAllocation(ct);
            payments.SavePaymentData(pData, cData, iData, currency, isCash, _C_BPartner_ID, _windowNo, payment, date, appliedamt, discount, writeOff, open);

            return "";
        }

    }


}