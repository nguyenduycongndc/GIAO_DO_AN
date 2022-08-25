using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class GetBookCar //get dữ liệu đặt xe
    {
        public int ID { get; set; }
        public string Code { get; set; }//mã
        public int? CustomerID { get; set; }//id người dùng
        public int? ShiperID { get; set; }//id shiper
        public string CustomerName { get; set; }//tên người dùng
        public string CustomerPhone { get; set; }//sdt người dùng
        public string CustomerAddress { get; set; }//địa chỉ người dùng
        public string ShiperName { get; set; }//tên Shiper
        public string ShiperPhone { get; set; }//tên Shiper
        public int AreaID { get; set; } //id vùng đặt xe
        public DateTime BookingDate { get; set; }//thời gian đặt
        public DateTime CreatedDate { get; set; }//thời gian đặt
        public int TotalPrice { get; set; }//tổng tiền
        public string FinishAddress { get; set; }//địa chỉ đến
        public string Address { get; set; }//địa chỉ đi
        public int? StatusPayment { get; set; }//trạng thái thanh toán(0: chưa thanh toán, 1: đã thanh toán)
        public int? Status { get; set; }//trạng thái thanh toán(-1: hủy,0: chờ xn, 1: đã tiếp nhận, 2: đã xác nhận, 3: hoàn thành)
        public int IsActive { get; set; }
        public int? UserCancel { get; set; }//người hủy
        public string ReasonCancel { get; set; }//lý do hủy
        public int? PaymentType { get; set; }//hình thức thanh toán
        public int? BasePrice { get; set; }//phí dịch vụ
        public int? UsePoint { get; set; }//Điểm đã dùng
        public int? CouponID { get; set; }//ID khuyến mãi
        public string CouponCode { get; set; }//Mã khuyến mãi
        public int? CouponDiscount { get; set; }//giá tiền khuyến mãi
        public int? TypeCoupon { get; set; }//kiểu % hoặc tiền
        public int? Percent { get; set; }//kiểu % 
        public int? Discount { get; set; }//kiểu tiền 
        public int? Calculate { get; set; }
        public float? Rate { get; set; }
        public string RateNote { get; set; }
        public int TypeBooking { get; set; } //loại booking
        public DateTime? StartDate { get; set; }//thời gian xác nhận
        public DateTime? ConfirmDate { get; set; }//thời gian đón khách
        public DateTime? CompletedDate { get; set; }//thời gian hoàn thành
        public int? CarTypeID { get; set; } // id VehicleTypes
        public string VehicleName { get; set; } // name VehicleTypes


    }
    //public class ViewListCustomModel
    //{
    //    public int? TranId { get; set; }
    //    public int? CustomerId { get; set; }
    //    public string CustomerName { get; set; }
    //    public string CustomerPhone { get; set; }
    //    public string CustomerAddress { get; set; }
    //}
    public class GetTransactionModel : TransactionModel
    {

        public int IsBokingNow { get; set; }

        public string WasherCode { get; set; }
        public string WasheName { get; set; }
        public string ComboName { get; set; }
        public string ServiceName { get; set; }
        public string BookingNowStr
        {
            get
            {
                if (IsBokingNow == 1)
                {
                    return "Booking now";
                }
                else
                {
                    return "App order";
                }
            }
        }

    }
    public class TransactionModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Province_Name { get; set; }
       // public string Code_For_Province { get { return Util.GetCodeInProvince(Code, Province_Name); } }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatDateSTR { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public string CarDetail { get { return CarBrand + " - " + CarModel + " (" + LicensePlates + ")"; } }
        public string CarModel { get; set; }
        public string CarBrand { get; set; }
        public string CustomerPhone { get; set; }
        public string LicensePlates { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingDateSTR { get { return BookingDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public int Status { get; set; }
        public List<int> listIndex { get; set; }
        public string StatusStr
        {
            get
            {
                if (Status == 0)
                {
                    return "Cancel";
                }
                else if (Status == 1 || Status == 9)
                {
                    return "Waiting";
                }
                else if (Status == 2)
                {
                    return "Confirmed";
                }
                else if (Status == 3)
                {
                    return "Complete";
                }
                else if (Status == 4)
                {
                    return "No confirm";
                }
                else if (Status == 5 || Status == 6)
                {
                    return "Washing";
                }
                else
                {
                    return "";
                }
            }
        }
        public string CustomerName { get; set; }
        public int PaymentType { get; set; }
        public string PaymentTypeStr
        {
            get
            {
                if (PaymentType == 1)
                {
                    return "CASH";
                }
                else
                {
                    return "VNPAY";
                }
            }
        }
        public int? TransactionStatus { get; set; }

    }
    public class TransactionDetailModel : TransactionModel
    {
        public string CustomerPhone { get; set; }
        public List<GetService> ListService { get; set; }
        public string PromotionCode { get; set; }
        public int deductionPoint { get; set; }
        public int isUsePoint { get; set; }
        public int UsePoint { get; set; }
        public string WasherName { get; set; }
        public string WasherPhone { get; set; }
        public List<ImageService> ListImage { get; set; }
        public int TotalPrice { get; set; }
        public int BasePrice { get; set; }

    }
    public class ImageService
    {
        public string url { get; set; }
        public string content { get; set; }
        public int ID { get; set; }
        public int Type { get; set; }
        public int IsActive { get; set; }
    }
}
