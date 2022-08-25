using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class MemberTransactionViewModel
    {
        public int Point { get; set; }
        public int Discount { get; set; }
        public int Revenue { get; set; }
        public string comboCode { get; set; }
        public string PointStr { get { return string.Format("{0:#,0}", Point); } }
        public string DiscountStr { get { return string.Format("{0:#,0}", Discount); } }
        public string RevenueStr { get { return string.Format("{0:#,0}", Revenue); } }
        public string Description { get; set; }
        public string ServiceName { get; set; }
        public int Status { get; set; }
        public string StatusStr
        {
            get
            {
                return Util.ConverStatusOrder(Status);
            }
        }
        public DateTime Date { get; set; }
        public DateTime BookingDate { get; set; }
        public int Type { get; set; }
        public int? Rate { get; set; }
        public double RateTB { get; set; }

    }
    public class IDNameMemberModel
    {
        public int ID { set; get; }

        public string Name { set; get; }
        public string Phone { get; set; }
        public string NameAndPhone { get; set; }
    }
}
