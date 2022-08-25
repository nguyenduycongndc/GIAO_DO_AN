using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class ComboServiceDetailModel
    {
        public int OrderServiceID { get; set; }
        public string ReasonNote { get; set; }
        public string Note { get; set; }
        public int CouponPoint { get; set; }
        public int UsePoint { get; set; }
        public string ComboCode { get; set; }
        public string ServiceCode { get; set; }
        public int? CarCustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAvatar { get; set; }
        public string CustomerAddress { get; set; }
        public DateTime BookingDate { get; set; }
        public String BookingDateStr { get { return BookingDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public double Distance { get; set; }
        public string CarImage { get; set; }
        public string LicensePlates { get; set; }
        public List<ServiceInCombo> ListService { get; set; }
        public int PaymentType { get; set; }
        public int BasePrice { get; set; }
        public int TotalPrice { get; set; }
        public int CarSegment { get; set; }
        public int Commission { get; set; }
        public double Longi { get; set; }
        public double Lati { get; set; }
        public int Status { get; set; }
        public int ComboID { get; set; }
        public string ComboName { get; set; }
        public CarOutputModel Car { get; set; }
    }
    public class ServiceInCombo
    {
        public string ServiceName { get; set; }
        public int ServicePrice { get; set; }
        public int ServiceType { get; set; }
    }
}
