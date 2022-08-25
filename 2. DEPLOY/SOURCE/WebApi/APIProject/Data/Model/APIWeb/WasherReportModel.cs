using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class WasherReportModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int CountTransaction { get; set; }
        public int CountInvoice { get; set; }
        public int CountCancleCus { get; set; }
        public int CountCancleWasher { get; set; }
        public int Commission { get; set; }
        public string CommissionSTR { get { return string.Format("{0:#,0}", Commission); } }
    }
    public class PaymentReportModel
    {
        public string TransactionCode { get; set; }
        public string OrderCode { get; set; }
        public string WasherName { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Date { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public int Commission { get; set; }
        public string CommissionSTR { get { return string.Format("{0:#,0}", Commission); } }
        public int TotalPrice { get; set; }
        public string TotalPriceSTR { get { return string.Format("{0:#,0}", TotalPrice); } }
        public int Discount { get; set; }
        public string DiscountSTR { get { return string.Format("{0:#,0}", Discount); } }
        public int Status { get; set; }
        public string StatusStr { get { return Util.ConverStatusOrder(Status); } }
        public int PaymentType { get; set; }
    }
}
