using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class TransactionHistoryBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public TransactionHistoryBusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public MembersTransactionHistory CreateTransactionHistory(MembersTransactionHistory input)
        {
            MembersTransactionHistory mb = new MembersTransactionHistory();
            mb.MemberID = input.MemberID;
            mb.OrderServiceID = input.OrderServiceID;
            mb.Title = input.Title;
            mb.Content = input.Content;
            mb.Point = input.Point;
            mb.Type = input.Type;
            mb.TransactionType = input.TransactionType;
            mb.TransactionID = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10);
            mb.BeforeBalance = input.BeforeBalance;
            mb.AfterBalance = input.AfterBalance;
            mb.Status = input.Status;
            mb.IsActive = SystemParam.ACTIVE;
            mb.CreateDate = DateTime.Now;
            mb.ConfirmDate = input.ConfirmDate;
            mb.Icon = input.Icon;
            mb.WalletID = input.WalletID;
            mb.CodeVNPay = input.CodeVNPay;
            mb.KeyVNPay = input.KeyVNPay;
            mb.IsExtra = true;
            return mb;

        }


        //Lấy danh sách lịch sửa điểm của khách hàng
        public IPagedList<MemberPointHistory> GetListHistoryPointMember(int page, int limit, int memberID)
        {
            try
            {
                Member memb = cnn.Members.Find(memberID);
                var data = cnn.MembersTransactionHistories.Where(m => m.IsActive.Equals(SystemParam.ACTIVE)
                && (m.TransactionType == Constant.TYPE_TRANSACTION_USE_POINT || m.TransactionType == Constant.TYPE_TRANSACTION_REFUND_USE_POINT
                || m.TransactionType == Constant.TYPE_TRANSACTION_EARN_POINT || m.TransactionType == Constant.TYPE_TRANSACTION_VNPAY_REFUND
                || m.TransactionType == Constant.TYPE_TRANSACTION_EARN_POINT_LEVEL_UP) && m.MemberID == memberID)
                    .Select(m => new MemberPointHistory
                    {
                        ID = m.ID,
                        Type = m.TransactionType,
                        CreatedDate = m.CreateDate,
                        Point = m.Point.HasValue ? m.Point.Value : 0,
                        Content = m.Content,
                        IsPlus = m.Type
                    }).OrderByDescending(h => h.ID).ToPagedList(page, limit);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<MemberPointHistory>().ToPagedList(1, 1);
            }
        }


        //tìm kiếm
        public IPagedList<GetBookCar> SearchTransaction(int page, string searchKey, string fromDate, string toDate, int? status)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);
                var data = cnn.OrderServices.Where(w => w.IsActive != 0 && w.TypeBooking == SystemParam.SHIP_DRIVER
                    && ((!String.IsNullOrEmpty(searchKey) ? w.Customer.Name.Contains(searchKey) : true)
                    || (!String.IsNullOrEmpty(searchKey) ? w.Customer.Phone.Contains(searchKey) : true)
                    || (!String.IsNullOrEmpty(searchKey) ? w.Code.Contains(searchKey) : true))
                    && (startDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) >= startDate.Value : true)
                    && (endDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) <= endDate.Value : true)
                    //&& (startDate.HasValue ? w.BookingDate >= startDate.Value : true)
                    //&& (endDate.HasValue ? w.BookingDate <= endDate.Value : true)
                    && (status != null ? status == w.Status : true)
                    ).Select(TranBus => new GetBookCar
                    {
                        ID = TranBus.ID,
                        Code = TranBus.Code,
                        CustomerName = TranBus.Customer.Name,
                        CustomerPhone = TranBus.Customer.Phone,
                        ShiperID = TranBus.Shiper.ID,
                        ShiperName = TranBus.Shiper.Name,
                        AreaID = TranBus.AreaID,
                        TotalPrice = TranBus.TotalPrice,
                        FinishAddress = TranBus.FinishAddress,
                        Address = TranBus.Address,
                        StatusPayment = TranBus.StatusPayment,
                        Status = TranBus.Status,
                        CreatedDate = TranBus.CreatedDate,
                        BookingDate = TranBus.BookingDate,
                        IsActive = TranBus.IsActive,
                        TypeBooking = TranBus.TypeBooking,
                    }).OrderByDescending(TranBus => TranBus.CreatedDate).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
                //var query = (from a in cnn.OrderServices
                //             join b in cnn.Customers on a.CustomerID equals b.ID
                //             join c in cnn.Shipers on a.ShiperID equals c.ID into jl
                //             from c in jl.DefaultIfEmpty()
                //             orderby a.BookingDate descending
                //             select new GetBookCar()
                //             {
                //                 ID = a.ID,
                //                 Code = a.Code,
                //                 CustomerName = b.Name,
                //                 ShiperName = c.Name,
                //                 AreaID = a.AreaID,
                //                 TotalPrice = a.TotalPrice,
                //                 FinishAddress = a.FinishAddress,
                //                 Address = a.Address,
                //                 StatusPayment = a.StatusPayment,
                //                 Status = a.Status,
                //                 CreatedDate = a.CreatedDate,
                //                 BookingDate = a.BookingDate,
                //                 IsActive = a.IsActive,
                //                 TypeBooking = a.TypeBooking,
                //             }).ToList();
                //query = query.Where(x => x.IsActive != 0).ToList();

                //query = query.Where(x => x.TypeBooking == 1).ToList();
                //if (!string.IsNullOrEmpty(searchKey))
                //{
                //    query = query.Where(x => x.CustomerName.ToUpper().Trim().Contains(searchKey.ToUpper().Trim())
                //    || x.Code.ToUpper().Trim().Contains(searchKey.ToUpper().Trim())).ToList();
                //}
                //if (status != null)
                //{
                //    query = query.Where(x => x.Status == status).ToList();
                //}

                //if (!string.IsNullOrEmpty(fromDate))
                //{
                //    query = query.Where(x => (startDate.HasValue ? x.BookingDate.Date >= startDate.Value.Date : true)).ToList();
                //}

                //if (!string.IsNullOrEmpty(toDate))
                //{
                //    query = query.Where(x => (endDate.HasValue ? x.BookingDate.Date <= endDate.Value.Date : true)).ToList();
                //}
                ////var group = query.ToList();

                //return query.ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<GetBookCar>().ToPagedList(1, 1);
            }
        }
        //Count TransactionHistory
        public long CountTransactionHistory()
        {
            var data = cnn.OrderServices.Where(w => w.IsActive != 0 && w.TypeBooking == SystemParam.SHIP_DRIVER
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
        public JsonResultModel DeleteTransactionBookCars(int id)
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
        //binding dữ liệu Ví
        public GetBookCar ViewBindingTransaction(int id)
        {
            try
            {
                var query = (from a in cnn.OrderServices
                                 //join b in cnn.Customers on a.CustomerID equals b.ID
                             join b in cnn.Customers on a.CustomerID equals b.ID into ac
                             from x in ac.DefaultIfEmpty()
                             join c in cnn.Shipers on a.ShiperID equals c.ID into abc
                             from f in abc.DefaultIfEmpty()
                                 //join d in cnn.Coupons on a.CouponID equals d.ID
                             join d in cnn.Coupons on a.CouponID equals d.ID into abcd
                             from z in abcd.DefaultIfEmpty()
                             join e in cnn.VehicleTypes on a.CarTypeID equals e.ID into abcdf
                             from v in abcdf.DefaultIfEmpty()
                             orderby a.BookingDate descending
                             //group a by a.CustomerID into g
                             select new GetBookCar()
                             {
                                 ID = a.ID,
                                 //CustomerName = a.Customer.Name != null ? a.Customer.Name : "",
                                 //CustomerPhone = a.Customer.Phone != null ? a.Customer.Phone : "",
                                 //CustomerAddress = a.Customer.Address != null ? a.Customer.Address : "",
                                 CustomerName = x != null ? x.Name : "",
                                 CustomerPhone = x != null ? x.Phone : "",
                                 CustomerAddress = x != null ? x.Address : "",
                                 ShiperName = f != null ? f.Name : "",
                                 ShiperID = f.ID,
                                 ShiperPhone = f != null ? f.Phone : "",
                                 Address = a.Address != null ? a.Address : "",
                                 UserCancel = a.UserCancel,
                                 ReasonCancel = a.ReasonCancel != null ? a.ReasonCancel : "",
                                 PaymentType = a.PaymentType,
                                 BasePrice = a.BasePrice,
                                 UsePoint = a.UsePoint,
                                 //CouponID = a.Coupon.ID,
                                 //CouponCode = a.Coupon.Code != null ? a.Coupon.Code : "",
                                 //CouponDiscount = a.Coupon.Discount,
                                 //TypeCoupon = a.Coupon.TypeCoupon,
                                 CouponID = z.ID,
                                 CouponCode = z != null ? z.Code : "",
                                 CouponDiscount = z.Discount,
                                 TypeCoupon = z.TypeCoupon,
                                 TotalPrice = a.TotalPrice,
                                 Percent = a.Coupon.Percent,
                                 Discount = a.Coupon.Discount,
                                 Calculate = (a.BasePrice * a.Coupon.Percent) / 100,
                                 BookingDate = a.BookingDate,
                                 Status = a.Status,
                                 StartDate = a.StartDate,
                                 ConfirmDate = a.ConfirmDate,
                                 CompletedDate = a.CompletedDate,
                                 Rate = (float?)a.Rate,
                                 RateNote = a.NoteRate,
                                 CarTypeID = v.ID,
                                 VehicleName = v != null ? v.Name : "",
                             });
                var data = query.Where(x => x.ID == id).FirstOrDefault();

                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new GetBookCar();
            }
        }
        //data excel
        public List<GetBookCar> DataExcel(string searchKey, string fromDate, string toDate, int? status)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);
                var data = cnn.OrderServices.Where(w => w.IsActive != 0 && w.TypeBooking == SystemParam.SHIP_DRIVER
                    && ((!String.IsNullOrEmpty(searchKey) ? w.Customer.Name.Contains(searchKey) : true)
                    || (!String.IsNullOrEmpty(searchKey) ? w.Code.Contains(searchKey) : true))
                    && (startDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) >= startDate.Value : true)
                    && (endDate.HasValue ? (w.CancleDate.HasValue ? w.CancleDate : w.CompletedDate.HasValue ? w.CompletedDate : w.ConfirmDate.HasValue ? w.ConfirmDate : w.CreatedDate) <= endDate.Value : true)
                    //&& (startDate.HasValue ? w.BookingDate >= startDate.Value : true)
                    //&& (endDate.HasValue ? w.BookingDate <= endDate.Value : true)
                    && (status != null ? status == w.Status : true)
                    ).Select(TranBus => new GetBookCar
                    {
                        ID = TranBus.ID,
                        Code = TranBus.Code,
                        CustomerName = TranBus.Customer.Name,
                        ShiperID = TranBus.Shiper.ID,
                        ShiperName = TranBus.Shiper.Name,
                        AreaID = TranBus.AreaID,
                        TotalPrice = TranBus.TotalPrice,
                        FinishAddress = TranBus.FinishAddress,
                        Address = TranBus.Address,
                        StatusPayment = TranBus.StatusPayment,
                        Status = TranBus.Status,
                        CreatedDate = TranBus.CreatedDate,
                        BookingDate = TranBus.BookingDate,
                        IsActive = TranBus.IsActive,
                        TypeBooking = TranBus.TypeBooking,
                    }).OrderByDescending(TranBus => TranBus.CreatedDate).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<GetBookCar>().ToList();
            }
        }
    }
}
