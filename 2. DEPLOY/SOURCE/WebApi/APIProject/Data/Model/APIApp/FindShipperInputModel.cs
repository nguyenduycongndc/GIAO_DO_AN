using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class FindDriverInputModel
    {
        public int VehicleID { get; set; }
        public int? CouponID { get; set; }
        public int? Point { get; set; }
        public int PaymentMethod { get; set; }
        public string LongiFrom { get; set; }
        public string LatiFrom { get; set; }
        public string AddressFrom { get; set; }
        public string LongiTo { get; set; }
        public string LatiTo { get; set; }
        public string AddressTo { get; set; }
    }
    public class FindDriverPackageInputModel: FindDriverInputModel
    {        
        public int OrderID { get; set; }
        public int DistrictID { get; set; }
        public int ProvinceID { get; set; }
        public string AddressToDetail { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        public int TransportType { get; set; }
        public int CODFee { get; set; }
        public string PackageType { get; set; }
        public double Weight { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public int PackageFee { get; set; }
        public string Note { get; set; }
        public int IsPaymentReceiver { get; set; }
        public List<string> Images { get; set; }
    }
    public class FindDriverFoodInputModel
    {
        public int ShopID { get; set; }
        public int? CouponID { get; set; }
        public int? Point { get; set; }
        public int PaymentMethod { get; set; }
        public string LongiTo { get; set; }
        public string LatiTo { get; set; }
        public string AddressTo { get; set; }
    }
}
