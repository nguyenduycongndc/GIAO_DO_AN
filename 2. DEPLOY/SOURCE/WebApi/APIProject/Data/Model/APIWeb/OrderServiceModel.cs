using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class OrderServiceModel : GetOrderService
    {

        public string LicensePlates { get; set; }
        public string CarDetail { get { return CarBrand + " - " + CarModel + " (" + LicensePlates + ")"; } }
        public string WasherName { get; set; }
        public string CustomerPhone { get; set; }
        public string PackageName { get; set; }
        public string PriceStr { get { return String.Format("{0:n0}", Price); } }
        public int PaymentType { get; set; }
        public int Discount { get { return UsePoint.GetValueOrDefault() + CouponPoint.GetValueOrDefault(); } }
        public int? UsePoint { get; set; }
        public int? CouponPoint { get; set; }
        public string DiscountStr { get { return String.Format("{0:n0}", Discount); } }
        public int? IsExportVAT { get; set; }
    }

    public class GetOrderService
    {
        public int ID { get; set; }
      //  public string Code { get { return Util.GetCodeInProvince(CodeCustomer, ProvinceName); } }
        public string CustomerName { get; set; }
        public string CarBrand { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        public string DateStr { get { return Date.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public string CarModel { get; set; }
        public int Price { get; set; }
        public int TotalPrice { get; set; }
        public string TotalPriceStr { get { return String.Format("{0:n0}", TotalPrice); } }
        public string ProvinceName { get; set; }
        public string CodeCustomer { get; set; }

    }

    public class SearchOrderService
    {
        public int Page { get; set; }
        public string searchKey { get; set; }
        public string FromDateStr { get; set; }
        public int? PaymentType { get; set; }

        public string ToDateStr { get; set; }
        public DateTime? FromDate
        {
            get
            {
                try
                {
                    return Util.ConvertDate(FromDateStr);
                    //return DateTime.ParseExact(FromDateStr, SystemParam.CONVERT_DATETIME_HAVE_HOUR, null);
                }
                catch
                {
                    return null;
                }
            }
        }
        public DateTime? ToDate
        {
            get
            {
                try
                {
                    DateTime? date = Util.ConvertDate(ToDateStr);
                    return date.HasValue ? date.Value.AddDays(1) : date;
                    //return DateTime.ParseExact(ToDateStr, SystemParam.CONVERT_DATETIME_HAVE_HOUR, null);
                }
                catch
                {
                    return null;
                }
            }
        }

        public int? AgentID { get; set; }
        public int? ServiceID { get; set; }
        public int? IsExport { get; set; }
    }

    public class GetOrderServiceDetail : GetOrderService
    {
        public string phone { get; set; }

        public string CarInfo { get { return CarBrand + " - " + CarModel + " (" + CarSegment + ")"; } }
        public string CarSegment { get; set; }
        public int PaymentType { get; set; }
        public string WasherCode { get; set; }
        public DateTime? StartTime { get; set; }
        public int EstTime { get; set; }
        public DateTime? EstCompleteTime { get { return (StartTime.HasValue ? StartTime.Value.AddMinutes(EstTime) : StartTime); } }
        public string ExecutionTime
        {
            get
            {
                return StartTime.HasValue ?
                    StartTime.Value.ToString("HH:mm") + "-" + EstCompleteTime.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) + " (" + (EstTime / 60).ToString() + " hours " + (EstTime % 60).ToString() + " minutes )"
                    : "";
            }
        }
        public List<GetService> ListService { get; set; }
        public string PromotionCode { get; set; }
        public int deductionPoint { get; set; }
        public string deductionPointStr { get { return string.Format("{0:#,0}", deductionPoint); } }
        public int isUsePoint { get; set; }
        public int UsePoint { get; set; }
        public string UsePointStr { get { return string.Format("{0:#,0}", UsePoint); } }
    }
    public class GetService
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
    }
}
