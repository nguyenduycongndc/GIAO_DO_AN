using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class OrderOutputModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int TotalPrice { get; set; }
        public string VehicleType { get; set; }
        public int BasePrice { get; set; }
        public int CODFee { get; set; }
        public int ShipperCommission { get; set; }
        public int IsReceiverPayment { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public double? Lati { get; set; }
        public double? Longi { get; set; }
        public int? CustomerID { get; set; }
        public int OrderActive { get; set; }
        public string ShiperName { get; set; }
        public string ShiperAvatar { get; set; }
        public string RecevieverName { get; set; }
        public int BookingType { get; set; }
        public string RecevieverPhone { get; set; }
        public string PlaceID { get; set; }
        public int? TransportType { get; set; }
        public int IsPayment { get; set; }
        public string ShiperPhone { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string FinishAddress { get; set; }
        public string Note { get; set; }
        public string ReasonCancel { get; set; }
        public string NoteRate { get; set; }
        public string NameService { get; set; }
        public int? ShopID { get; set; }
        public string ShopName { get; set; }
        public string ShopAvatar { get; set; }
        public string ShopPhone { get; set; }
        public string PackageType { get; set; }
        public int? PackageFee { get; set; }
        public object ListImage { get; set; }
        public object ListService { get; set; }
        public int PaymentType { get; set; }
        public double Distance { get; set; }
        public double Weight { get; set; }
        public int TimeShip { get; set; }
        public int TimeWait { get; set; }
        public int? CouponValue { get; set; }
        public int UsePoint { get; set; }
        public float ShiperRate { get; set; }
        public CarInfo CarShiper { get; set; }
        public int IsRate { get; set; }
        public int IsRateShop { get; set; }
        public List<ServiceDetail> ServiceDetail { get; set; }

    }


    public class ServiceDetail
    {
        public int Amount { get; set; }
        public string ItemName { get; set; }
        public int ItemPrice { get; set; }
    }

    public class CarInfo
    {
        public string LicensePlates { get; set; }
        public string CarColor { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string VehicleType { get; set; }
    }
}
