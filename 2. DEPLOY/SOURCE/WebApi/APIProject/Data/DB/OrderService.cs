//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderService()
        {
            this.ComplainOrders = new HashSet<ComplainOrder>();
            this.OrderServiceDetails = new HashSet<OrderServiceDetail>();
            this.OrderServiceImages = new HashSet<OrderServiceImage>();
            this.OrderServiceStatusHistories = new HashSet<OrderServiceStatusHistory>();
        }
    
        public int ID { get; set; }
        public string Code { get; set; }
        public int CustomerID { get; set; }
        public int AreaID { get; set; }
        public Nullable<int> ShiperID { get; set; }
        public Nullable<int> FirstShiperID { get; set; }
        public string OtherShiper { get; set; }
        public int Status { get; set; }
        public int IsActive { get; set; }
        public int TypeBooking { get; set; }
        public int TotalPrice { get; set; }
        public int BasePrice { get; set; }
        public int CouponPoint { get; set; }
        public double Distance { get; set; }
        public Nullable<double> Weight { get; set; }
        public string FinishPlaceID { get; set; }
        public string FinishAddress { get; set; }
        public string FinishAddressDetail { get; set; }
        public Nullable<double> FinishLati { get; set; }
        public Nullable<double> FinishLongi { get; set; }
        public string PackageType { get; set; }
        public Nullable<int> PackageFee { get; set; }
        public Nullable<int> CODFee { get; set; }
        public int IsPaymentReceiver { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string RecevieverName { get; set; }
        public string RecevieverPhone { get; set; }
        public string ServiceID { get; set; }
        public int UsePoint { get; set; }
        public int IsRate { get; set; }
        public Nullable<double> Rate { get; set; }
        public string NoteRate { get; set; }
        public Nullable<int> IsRateShop { get; set; }
        public Nullable<double> RateShop { get; set; }
        public string NoteRateShop { get; set; }
        public Nullable<int> CouponID { get; set; }
        public string Note { get; set; }
        public int PaymentType { get; set; }
        public Nullable<int> StatusPayment { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public System.DateTime BookingDate { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> CancleDate { get; set; }
        public Nullable<double> Longi { get; set; }
        public Nullable<double> Lati { get; set; }
        public string ReasonCancel { get; set; }
        public string PlaceID { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> PushFirstDate { get; set; }
        public int IsPushFirst { get; set; }
        public Nullable<int> UserCancel { get; set; }
        public string ShiperNote { get; set; }
        public Nullable<int> ShopID { get; set; }
        public Nullable<int> TransportType { get; set; }
        public Nullable<int> CarTypeID { get; set; }
        public Nullable<int> TimeWait { get; set; }
        public Nullable<int> TimeShip { get; set; }
        public Nullable<int> ShiperCommission { get; set; }
        public Nullable<int> TransportAreaID { get; set; }
        public Nullable<int> Height { get; set; }
        public Nullable<int> Length { get; set; }
        public Nullable<int> Width { get; set; }
        public Nullable<int> ProvinceID { get; set; }
        public Nullable<int> DistrictID { get; set; }
    
        public virtual Area Area { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ComplainOrder> ComplainOrders { get; set; }
        public virtual ConfigTransportArea ConfigTransportArea { get; set; }
        public virtual Coupon Coupon { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual District District { get; set; }
        public virtual VehicleType VehicleType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderServiceDetail> OrderServiceDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderServiceImage> OrderServiceImages { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderServiceStatusHistory> OrderServiceStatusHistories { get; set; }
        public virtual Shiper Shiper { get; set; }
        public virtual Shop Shop { get; set; }
        public virtual Province Province { get; set; }
    }
}
