using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class BookingDriverOutputModel
    {
        public int ID { get; set; }
        public int Countdown { get; set; }
        public string AvatarUrl { get; set; }
        public string CustomerName { get; set; }
        public int RankID { get; set; }
        public string RankName { get; set; }
        public string StartAddress { get; set; }
        public double StartLati { get; set; }
        public double StartLongi { get; set; }
        public string FinishAddress { get; set; }
        public double FinishLati { get; set; }
        public double FinishLongi { get; set; }
        public int BasePrice { get; set; }
        public int CouponDiscount { get; set; }
        public int Point { get; set; }
        public int TotalPrice { get; set; }
        public int DriverPrice { get; set; }
        public int PaymentMethod { get; set; }
        public int Status { get; set; }
        public int TypeBooking { get; set; }
        public DateTime TimeBooking { get; set; }
        public DateTime TimeExpired { get; set; }
        public int IsPushFirst { get; set; }
        public DateTime? TimePushFirst { get; set; }
    }
    public class BookingPackageOutputModel : BookingDriverOutputModel
    {

        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string PackageType { get; set; }
        public int TransportPackageType { get; set; }
        public double? Weight { get; set; }
        public int? CODFee { get; set; }
        public int IsPaymentReceiver { get; set; }
        public string Note { get; set; }

    }
    public class BookingPackageVIPOutputModel : BookingPackageOutputModel
    {

        public int? Width { get; set; }
        public int? Length { get; set; }
        public int? Height { get; set; }
        public int? ProvinceID { get; set; }
        public int? DistrictID { get; set; }

    }
    public class BookingFoodOutputModel : BookingDriverOutputModel
    {
        public List<CartDetailOutput> CartDetails { get; set; }
        public int TotalQuantity { get; set; }
        public int FoodPrice { get; set; }
        public string ShopName { get; set; }
        public string ShopPhone { get; set; }
    }
    public class OrderServiceDriver
    {
        public int ID { get; set; }
        public string AvatarUrl { get; set; }
        public string CustomerName { get; set; }
        public string ChatIDCustomer { get; set; }
        public int RankID { get; set; }
        public string RankName { get; set; }
        public string Phone { get; set; }
        public int EstimateTimeWait { get; set; }
        public int EstimateTimeShip { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public double LatiFrom { get; set; }
        public double LongiFrom { get; set; }
        public double LatiTo { get; set; }
        public double LongiTo { get; set; }
        public int PaymentMethod { get; set; }
        public string Transport { get; set; }
        public int BasePrice { get; set; }
        public int CouponDiscount { get; set; }
        public int Point { get; set; }
        public int TotalPrice { get; set; }
        public int DriverPrice { get; set; }
        public int Status { get; set; }
        public int TypeBooking { get; set; }
        public int IsMotorbike { get; set; }

    }
    public class OrderServicePackageDriver : OrderServiceDriver
    {
        public string AddressToDetail { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public List<string> Images { get; set; }
        public string PackageType { get; set; }
        public double? Weight { get; set; }
        public int? CODFee { get; set; }
        public int? PackageFee { get; set; }
        public int IsPaymentReceiver { get; set; }
        public int TransportPackageType { get; set; }
        public string Note { get; set; }
    }
    public class OrderServicePackageVIPDriver : OrderServicePackageDriver
    {
        public int? Width { get; set; }
        public int? Length { get; set; }
        public int? Height { get; set; }
        public int? ProvinceID { get; set; }
        public int? DistrictID { get; set; }
    }
    public class OrderServiceFoodDriver : OrderServiceDriver
    {
        public List<CartDetailOutput> CartDetails { get; set; }
        public string ShopName { get; set; }
        public string ShopPhone { get; set; }
        public int TotalQuantity { get; set; }
        public int FoodPrice { get; set; }
    }

    public class OrderServiceCustomer
    {
        public int ID { get; set; }
        public int EstimateTimeWait { get; set; }
        public int EstimateTimeShip { get; set; }
        public int DriverID { get; set; }
        public string ChatIDDriver { get; set; }
        public string AvatarUrl { get; set; }
        public string DriverName { get; set; }
        public string Phone { get; set; }
        public double Rating { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string LicensePlates { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public double LatiFrom { get; set; }
        public double LongiFrom { get; set; }
        public double LatiTo { get; set; }
        public double LongiTo { get; set; }
        public double EstimateDistance { get; set; }
        public int PaymentMethod { get; set; }
        public int IsPayment { get; set; }
        public string Transport { get; set; }
        public int BasePrice { get; set; }
        public int CouponDiscount { get; set; }
        public int Point { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }
        public int TypeBooking { get; set; }
        public int IsMotorbike { get; set; }

    }
    public class OrderServicePackageCustomer : OrderServiceCustomer
    {
        public string AddressToDetail { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public string PackageType { get; set; }
        public double? Weight { get; set; }
        public int? CODFee { get; set; }
        public string Note { get; set; }
        public int? IsPaymentReceiver { get; set; }
        public int TransportPackageType { get; set; }
    }
    public class OrderServicePackageVIPCustomer : OrderServicePackageCustomer
    {
        public int? Width { get; set; }
        public int? Length { get; set; }
        public int? Height { get; set; }
        public int? ProvinceID { get; set; }
        public int? DistrictID { get; set; }
    }
    public class OrderServiceFoodCustomer : OrderServiceCustomer
    {

        public List<CartDetailOutput> CartDetails { get; set; }
        public int TotalQuantity { get; set; }
        public int FoodPrice { get; set; }
        public string ShopPhone { get; set; }
        public string ShopName { get; set; }
    }
    public class OrderServiceInfo
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public string AvatarUrl { get; set; }
        public string CustomerName { get; set; }
        public int BasePrice { get; set; }
        public int CouponPrice { get; set; }
        public int PointPrice { get; set; }
        public int TotalPrice { get; set; }
        public int DriverPrice { get; set; }
        public string AddressFrom { get; set; }
        public string AddressTo { get; set; }
        public int TypeBooking { get; set; }
        public int PaymentMethod { get; set; }
        public int TransportType { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class DeclineOrderServiceInputModel
    {
        public int OrderServiceID { get; set; }
        public string Reason { get; set; }
    }
    public class ShiperLocationInputModel
    {
        public double lati { get; set; }
        public double longi { get; set; }
    }
}
