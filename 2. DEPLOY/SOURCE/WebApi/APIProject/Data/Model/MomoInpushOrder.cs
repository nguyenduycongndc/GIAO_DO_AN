using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class MomoInpushOrder
    {
        public string enviroment { get; set; }
        public string action { get; set; }
        public string merchantname { get; set; }
        public string merchantcode { get; set; }
        public string merchantnamelabel { get; set; }
        public string description { get; set; }
        public int amount { get; set; }
        public string orderId { get; set; }
        public string orderLabel { get; set; }
        public string appScheme { get; set; }
        public string partner { get; set; }
    }
    public class PaymentMomoInput
    {
        public string token { get; set; }
        public int transactionID { get; set; }
        public string customerNumber { get; set; }
        public int amount { get; set; }
    }

    public class PayAppMoMo
    {
        public string partnerCode { get; set; }
        public string partnerRefId { get; set; }
        public string customerNumber { get; set; }
        public string appData { get; set; }
        public string hash { get; set; }
        public double version = 2.0;
        public int payType = 3;
    }
    public class PaymentRespone
    {
        public int status { get; set; }
        public string message { get; set; }
        public string transid { get; set; }
        public long amount { get; set; }
        public string signature { get; set; }

    }
    public class ConfirmMomo
    {
        public string partnerCode { get; set; }
        public string partnerRefId { get; set; }
        public string requestType { get; set; }
        public string requestId { get; set; }
        public string momoTransId { get; set; }
        public string signature { get; set; }
    }
}
