using APIProject.Models;
using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class TransactionDeliveryBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        BookingBusiness bookBusiness = new BookingBusiness();
        public TransactionDeliveryBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        //tìm kiếm
        public IPagedList<TransactionDeliveryModel> SearchTransactionDelivery(int page, string searchKey, int? provinceID, int? districtID, string fromDate, string toDate, int? status, int? transportType, int isProvince, int? isVip)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);
                var data = cnn.OrderServices.Where(w => w.IsActive != 0 && w.TypeBooking == SystemParam.SHIP_PACKAGE
                    && ((!String.IsNullOrEmpty(searchKey) ? w.Customer.Name.Contains(searchKey) : true)
                    || (!String.IsNullOrEmpty(searchKey) ? w.Customer.Phone.Contains(searchKey) : true)
                    || (!String.IsNullOrEmpty(searchKey) ? w.Code.Contains(searchKey) : true))
                    && (startDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) >= startDate.Value : true)
                    && (endDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) <= endDate.Value : true)
                    && (provinceID.HasValue ? w.Area.District.ProvinceID.Equals(provinceID.Value) : true)
                    && (districtID.HasValue ? w.Area.DistrictID.Equals(districtID.Value) : true)
                    && (status != null ? status == w.Status : true)
                    && (isVip.HasValue ? (isVip.Value == SystemParam.SHIPPER_VIP ? w.TransportType.Value >= SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE : w.TransportType.Value < SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE) : true)
                    && (transportType.HasValue ? (transportType.Value == SystemParam.TRANSPORT_TYPE_STANDARD ? (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_STANDARD || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_STANDARD)
                    : (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_FAST || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_AIRLINE)) : true)
                    && (isProvince == SystemParam.IN_PROVINCE ? (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_WESEN || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE) :
                    (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_STANDARD || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_FAST || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_STANDARD || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_AIRLINE))
                    ).Select(TranDe => new TransactionDeliveryModel
                    {
                        ID = TranDe.ID,
                        Code = TranDe.Code,
                        CustomerName = TranDe.Customer.Name,
                        ShiperName = TranDe.Shiper.Name,
                        AreaID = TranDe.AreaID,
                        CustomerPhone = TranDe.Customer.Phone,
                        TotalPrice = TranDe.TotalPrice,
                        FinishAddress = TranDe.FinishAddress,
                        Address = TranDe.Address,
                        StatusPayment = TranDe.StatusPayment,
                        Status = TranDe.Status,
                        CreatedDate = TranDe.CreatedDate,
                        BookingDate = TranDe.BookingDate,
                        IsActive = TranDe.IsActive,
                        PackageType = TranDe.PackageType,
                        TypeBooking = TranDe.TypeBooking,
                        TransportType = TranDe.TransportType,
                        CODFee = (int)TranDe.CODFee,
                        PackageFee = (int)TranDe.PackageFee,
                        SenderName = TranDe.SenderName,
                        RecevieverName = TranDe.RecevieverName,
                        RecevieverPhone = TranDe.RecevieverPhone,
                        Weight = (float)TranDe.Weight,
                    }).OrderByDescending(TranDe => TranDe.CreatedDate).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<TransactionDeliveryModel>().ToPagedList(1, 1);
            }
        }
        //Count TransactionDelivery
        public long CountTransactionDelivery()
        {
            var data = cnn.OrderServices.Where(w => w.IsActive != 0 && (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_WESEN || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                   ).Select(TranBus => new GetBookCar
                   {
                       TotalPrice = TranBus.TotalPrice,
                   }).ToList();

            long sumTotalPrice = 0;
            foreach (var dt in data)
            {
                sumTotalPrice += dt.TotalPrice;
            }

            return sumTotalPrice;
        }
        //xóa
        public JsonResultModel DeleteTransactionDeliverys(int id)
        {
            try
            {
                OrderService orderService = cnn.OrderServices.Find(id);
                orderService.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, orderService);
            }
            catch (Exception ex)
            {
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }
        //binding dữ liệu
        public TransactionDeliveryModel ViewBindingTransactionDelivery(int id)
        {
            try
            {
                var query = (from a in cnn.OrderServices
                             join b in cnn.Customers on a.CustomerID equals b.ID into zbc
                             from f in zbc.DefaultIfEmpty()
                             join c in cnn.Shipers on a.ShiperID equals c.ID into ac
                             from e in ac.DefaultIfEmpty()
                             join d in cnn.Coupons on a.CouponID equals d.ID into abcd
                             from z in abcd.DefaultIfEmpty()
                             orderby a.BookingDate descending
                             select new TransactionDeliveryModel()
                             {
                                 ID = a.ID,
                                 //CustomerName = a.Customer != null ? a.Customer.Name : "",
                                 //CustomerPhone = a.Customer != null ? a.Customer.Phone : "",
                                 //CustomerAddress = a.Customer != null ? a.Customer.Address : "",
                                 CustomerName = f != null ? f.Name : "",
                                 CustomerPhone = f != null ? f.Phone : "",
                                 CustomerAddress = f != null ? f.Address : "",
                                 ShiperName = e != null ? e.Name : "",
                                 ShiperPhone = e != null ? e.Phone : "",
                                 Address = a.Address != null ? a.Address : "",
                                 UserCancel = a.UserCancel,
                                 ReasonCancel = a.ReasonCancel != null ? a.ReasonCancel : "",
                                 PaymentType = a.PaymentType,
                                 BasePrice = a.BasePrice,
                                 UsePoint = a.UsePoint,
                                 CouponID = z.ID,
                                 CouponCode = z != null ? z.Code : "",
                                 CouponDiscount = z.Discount,
                                 TypeCoupon = z.TypeCoupon,
                                 //CouponID = a.Coupon.ID,
                                 //CouponCode = a.Coupon != null ? a.Coupon.Code : "",
                                 //CouponDiscount = a.Coupon.Discount,
                                 //TypeCoupon = a.Coupon.TypeCoupon,
                                 TotalPrice = a.TotalPrice,
                                 Percent = a.Coupon.Percent,
                                 Discount = a.Coupon.Discount,
                                 Calculate = (a.BasePrice * a.Coupon.Percent) / 100,
                                 BookingDate = a.BookingDate,
                                 Status = a.Status,
                                 Rate = (float?)a.Rate,
                                 RateNote = a.NoteRate,
                                 StartDate = a.StartDate,
                                 ConfirmDate = a.ConfirmDate,
                                 CompletedDate = a.CompletedDate,
                                 TransportType = a.TransportType,
                             });
                var data = query.Where(x => x.ID == id).FirstOrDefault();

                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new TransactionDeliveryModel();
            }
        }
        //lưu khi sửa
        public bool SaveUpdateStatus(TransactionDeliveryModel req)
        {
            OrderService orderService = cnn.OrderServices.Find(req.ID);
            if (orderService == null)
            {
                return false;
            }
            if (req.Status.GetValueOrDefault() != orderService.Status)
            {
                if (req.Status.GetValueOrDefault() == SystemParam.ORDER_STATUS_FINISH)
                {
                    if (bookBusiness.CompleteOrderService(orderService.ID, orderService.ShiperID.GetValueOrDefault()) == SystemParam.SUCCESS)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (req.Status.GetValueOrDefault() == SystemParam.ORDER_STATUS_DENY)
                {
                    if (bookBusiness.DeclineOrderService(orderService.ID, orderService.ShiperID.GetValueOrDefault()) == SystemParam.SUCCESS)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (req.Status.GetValueOrDefault() == SystemParam.ORDER_STATUS_PICK_UP)
                {
                    if (bookBusiness.PickUpCustomer(orderService.ID, orderService.ShiperID.GetValueOrDefault()) == SystemParam.SUCCESS)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

            }
            return true;
        }
        //data excel
        public List<TransactionDeliveryModel> DataExcel(string searchKey, int? provinceID, int? districtID, string fromDate, string toDate, int? status, int? transportType, int isProvince,int? isVip)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);

                var data = cnn.OrderServices.Where(w => w.IsActive != 0 && w.TypeBooking == SystemParam.SHIP_PACKAGE
                    && ((!String.IsNullOrEmpty(searchKey) ? w.Customer.Name.Contains(searchKey) : true)
                    || (!String.IsNullOrEmpty(searchKey) ? w.Code.Contains(searchKey) : true))
                    && (startDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) >= startDate.Value : true)
                    && (endDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) <= endDate.Value : true)
                    && (provinceID.HasValue ? w.Area.District.ProvinceID.Equals(provinceID.Value) : true)
                    && (districtID.HasValue ? w.Area.DistrictID.Equals(districtID.Value) : true)
                    && (status != null ? status == w.Status : true)
                    && (isVip.HasValue ? (isVip.Value == SystemParam.SHIPPER_VIP ? w.TransportType.Value >= SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE : w.TransportType.Value < SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE) : true)
                    && (transportType.HasValue ? (transportType.Value == SystemParam.TRANSPORT_TYPE_STANDARD ? (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_STANDARD || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_STANDARD)
                    : (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_FAST || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_AIRLINE)) : true)
                    && (isProvince == SystemParam.IN_PROVINCE ? (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_WESEN || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE) :
                    (w.TransportType.Value == SystemParam.TRANSPORT_TYPE_STANDARD || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_FAST || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_STANDARD || w.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_AIRLINE))
                    ).Select(TranDe => new TransactionDeliveryModel
                    {
                        ID = TranDe.ID,
                        Code = TranDe.Code,
                        CustomerName = TranDe.Customer.Name,
                        CustomerPhone = TranDe.Customer.Phone,
                        ShiperName = TranDe.Shiper.Name,
                        AreaID = TranDe.AreaID,
                        TotalPrice = TranDe.TotalPrice,
                        FinishAddress = TranDe.FinishAddress,
                        Address = TranDe.Address,
                        StatusPayment = TranDe.StatusPayment,
                        Status = TranDe.Status,
                        CreatedDate = TranDe.CreatedDate,
                        BookingDate = TranDe.BookingDate,
                        IsActive = TranDe.IsActive,
                        PackageType = TranDe.PackageType,
                        TypeBooking = TranDe.TypeBooking,
                        SenderName = TranDe.SenderName,
                        IsReceiverPayment = TranDe.IsPaymentReceiver,
                        CompletedDate = TranDe.CompletedDate,
                        SenderPhone = TranDe.SenderPhone,
                        Note = TranDe.Note,
                        CouponDiscount = TranDe.CouponPoint,
                        PackageFee = TranDe.PackageFee.HasValue ? TranDe.PackageFee.Value : 0,
                        TransportType = TranDe.TransportType,
                        RecevieverName = TranDe.RecevieverName,
                        RecevieverPhone = TranDe.RecevieverPhone,
                        CODFee = (int)TranDe.CODFee,
                    }).OrderByDescending(TranDe => TranDe.CreatedDate).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<TransactionDeliveryModel>().ToList();
            }
        }
    }
}
