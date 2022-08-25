using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class TransactionFoodModel
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
        public float? Rate { get; set; }//đánh giá tài xế
        public string RateNote { get; set; }
        public float? RateShop { get; set; }//đánh giá quán ăn
        public string RateShopNote { get; set; }
        public string ShopName { get; set; }// tên shop
        public string ShopPhone { get; set; }// sdt shop
        public int TypeBooking { get; set; }// loai booking
        public int Quantity { get; set; }//số lượng sản phẩm
        public int? ShopID { get; set; }//id shop
        public int? ServicePriceID { get; set; }//id ServicePrice
        public string ServicePriceName { get; set; }//tên ServicePrice
        public int PriceServicePrice { get; set; }//price ServicePrice
        public string Note { get; set; }//note món ăn
        public int BasePriceService { get; set; }//phí dịch vụ ServicePrice
        public DateTime? StartDate { get; set; }//thời gian xác nhận
        public DateTime? ConfirmDate { get; set; }//thời gian đón khách
        public DateTime? CompletedDate { get; set; }//thời gian hoàn thành
        public DateTime? CancleDate { get; set; }//thời gian hủy
    }
}
