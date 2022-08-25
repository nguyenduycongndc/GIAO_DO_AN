using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Common;
using System.Web;
using System.Windows.Ink;
using static Data.Model.APIWeb.OrderDetailEditOutput;
using SharpRaven;
using SharpRaven.Data;
using APIProject.Models;

namespace Data.Business
{
    public class OrderServiceBusiness : GenericBusiness
    {
        string fullHost = Util.getFullUrl();
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        TransactionHistoryBusiness transactionBus;
        CartBusiness cartBus;
        OneSignalBusiness oneSignalBus;
        LoginBusiness lgBus;
        CouponBusiness couponBus;
        //  PlaceBusiness placeBus;
        //SendRequestBusiness sendRequestBus;
        NotifyBusiness notiBus;
        // AgentBusiness agentBus;
        CustomerBusiness cusBus;
        RequestAPIBusiness apiBus;
        // RankingBusiness rankBus;
        // ServiceBusiness serviceBus;
        //  UserBusiness userBus;
        PushNotifyBusiness pushNotiBus;
        public OrderServiceBusiness(WE_SHIPEntities context = null) : base()
        {
            transactionBus = new TransactionHistoryBusiness(this.cnn);
            oneSignalBus = new OneSignalBusiness(this.cnn);
            lgBus = new LoginBusiness(this.cnn);
            couponBus = new CouponBusiness(this.cnn);
            // placeBus = new PlaceBusiness(this.cnn);
            // sendRequestBus = new SendRequestBusiness(this.cnn);
            notiBus = new NotifyBusiness(this.cnn);
            //agentBus = new AgentBusiness(this.cnn);
            cusBus = new CustomerBusiness(this.cnn);
            apiBus = new RequestAPIBusiness(this.cnn);
            cartBus = new CartBusiness(this.cnn);
            //rankBus = new RankingBusiness(this.cnn);
            // serviceBus = new ServiceBusiness(this.cnn);
            // userBus = new UserBusiness(this.cnn);
        }
        int[] washing = { Constant.ORDER_STATUS_CONFIRM, Constant.ORDER_STATUS_CONFIRM_WASHING, Constant.ORDER_STATUS_WASHING };

        //Search cusName autocomplete
        public Array SearchCusName(string Name)
        {
            string[] cusName = (from c in cnn.OrderServices.Where(c => c.Customer.Name.Contains(Name))
                                where c.IsActive.Equals(SystemParam.ACTIVE)
                                select c.Customer.Name).ToArray();
            return cusName;
        }

        public IPagedList<OrderOutputModel> GetListOrder(int page, int limit, int memberID, int? type,int? status,string fromDate,string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertFromDate(fromDate);
                DateTime? td = Util.ConvertToDate(toDate);
                var member = cnn.Members.FirstOrDefault(x => x.ID == memberID);
                var data = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && ((member.ShiperID.HasValue ? o.ShiperID == member.ShiperID : true)
                && (member.ShopID.HasValue ? o.ShopID == member.ShopID : true) && (member.CustomerID.HasValue ? o.CustomerID == member.CustomerID : true)) && (type.HasValue && type > 0 ? o.TypeBooking.Equals(type.Value) : true)
                && (status.HasValue ? (o.Status == status.Value) : true) && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true)
                )
                    .Select(o => new
                    {
                        ID = o.ID,
                        Code = o.Code,
                        CustomerName = o.Customer.Name,
                        CustomerPhone = o.Customer.Phone,
                        TotalPrice = o.TotalPrice,
                        BasePrice = o.BasePrice,
                        Status = o.Status,
                        CreateDate = o.CreatedDate,
                        VehicleType = o.CarTypeID.HasValue ? o.VehicleType.Name : "",
                        Lati = o.Lati,
                        Longi = o.Longi,
                        BookingDate = o.BookingDate,
                        BookingType = o.TypeBooking,
                        TransportType = o.TransportType,
                        Address = o.Address,
                        FinishAddress = o.FinishAddress,
                        ShopID = o.ShopID,
                        PaymentType = o.PaymentType,
                        IsPayment = o.StatusPayment.HasValue ? o.StatusPayment.Value : SystemParam.ORDER_NOT_PAYMENT,
                        Note = o.Note,
                        order = o.OrderServiceDetails.Where(od => od.OrderServiceID == o.ID)
                    }).AsEnumerable().Select(o => new OrderOutputModel
                    {
                        ID = o.ID,
                        Code = o.Code,
                        CustomerName = o.CustomerName,
                        CustomerPhone = o.CustomerPhone,
                        TotalPrice = o.TotalPrice,
                        BasePrice = o.BasePrice,
                        Status = o.Status,
                        CreateDate = o.CreateDate,
                        VehicleType = o.VehicleType,
                        Lati = o.Lati,
                        Longi = o.Longi,
                        BookingDate = o.BookingDate,
                        BookingType = o.BookingType,
                        TransportType = o.TransportType,
                        Address = o.Address,
                        FinishAddress = o.FinishAddress,
                        PaymentType = o.PaymentType,
                        ShopID = o.ShopID,
                        IsPayment = o.IsPayment,
                        Note = o.Note,
                        ServiceDetail = o.order.Count() > 0 ?
                        o.order.GroupBy(od => od.ServicePriceID).Select(od => new ServiceDetail
                        {
                            Amount = od.Count(),
                            ItemName = od.FirstOrDefault().ServicePrice.Name,
                            ItemPrice = od.FirstOrDefault().ServicePrice.Price
                        }).ToList() : null
                    }).OrderByDescending(o => o.ID).ToPagedList(page, limit);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<OrderOutputModel>().ToPagedList(1, 1);
            }
        }
        public JsonResultModel GetOrderServiceDetail(int orderID, int memberID)
        {
            try
            {
                Member m = cnn.Members.Find(memberID);
                OrderService od = cnn.OrderServices.Where(o => o.ID.Equals(orderID) && ((m.CustomerID.HasValue ? o.CustomerID.Equals(m.CustomerID.Value) : true)
                || (m.ShopID.HasValue ? o.ShopID == m.ShopID : true) || (m.ShiperID.HasValue ? o.ShiperID == m.ShiperID : true))).FirstOrDefault();
                if (od == null)
                    return rpBus.ErrorResult("Không tìm thấy đơn hàng", SystemParam.PROCESS_ERROR);
                OrderOutputModel data = new OrderOutputModel();
                data.ID = od.ID;
                data.Code = od.Code;
                data.Note = od.Note;
                data.CustomerName = od.Customer.Name;
                data.CustomerPhone = od.Customer.Phone;
                data.TransportType = od.TransportType.GetValueOrDefault();
                data.IsPayment = od.StatusPayment.GetValueOrDefault();
                data.TotalPrice = od.TotalPrice;
                data.BasePrice = od.BasePrice;
                data.CODFee = od.CODFee.GetValueOrDefault();
                data.Weight = od.Weight.GetValueOrDefault();
                data.IsReceiverPayment = od.IsPaymentReceiver;
                data.IsRate = od.IsRate;
                data.IsRateShop = od.IsRateShop.GetValueOrDefault();
                data.Status = od.Status;
                data.ShipperCommission = od.ShiperCommission.HasValue ? od.ShiperCommission.Value : 0;
                data.BookingDate = od.BookingDate;
                data.ConfirmDate = od.ConfirmDate;
                data.CompletedDate = od.CompletedDate;
                data.Lati = od.Lati;
                data.Longi = od.Longi;
                data.ShiperName = od.ShiperID.HasValue ? od.Shiper.Name : "";
                data.ShiperPhone = od.ShiperID.HasValue ? od.Shiper.Phone : "";
                data.ShiperAvatar = od.ShiperID.HasValue ? fullHost + od.Shiper.AvartarUrl : "";
                data.ShiperRate = od.ShiperID.HasValue ? (float)od.Shiper.Rating : 0;
                data.RecevieverName = od.RecevieverName;
                data.RecevieverPhone = od.RecevieverPhone;
                data.BookingType = od.TypeBooking;
                data.PlaceID = od.PlaceID;
                data.CustomerName = od.Customer.Name;
                data.CustomerPhone = od.Customer.Phone;
                data.Address = od.Address;
                data.FinishAddress = od.FinishAddress;
                data.ShopName = od.ShopID.HasValue ? od.Shop.Name : "";
                data.ShopPhone = od.ShopID.HasValue ? od.Shop.Phone : "";
                data.ShopAvatar = od.ShopID.HasValue ? fullHost + od.Shop.ShopImages.Where(s => s.Type.Equals(SystemParam.TYPE_SHOP_IMAGE)).FirstOrDefault().Path : "";
                data.PackageFee = od.PackageFee;
                data.PackageType = od.PackageType;
                data.Distance = od.Distance;
                data.TimeShip = od.TimeShip.HasValue ? od.TimeShip.Value : 0;
                data.PaymentType = od.PaymentType;
                data.TimeWait = od.TimeWait.HasValue ? od.TimeWait.Value : 0;
                data.CouponValue = od.CouponPoint;
                data.UsePoint = od.UsePoint;
                data.StartDate = od.StartDate;
                data.ListImage = od.OrderServiceImages.Any(o => o.OrderServiceID.Equals(od.ID)) ? od.OrderServiceImages.Where(o => o.OrderServiceID.Equals(od.ID))
                    .Select(o => new OrderServiceImage
                    {
                        ID = o.ID,
                        Path = fullHost + o.Path
                    }).ToList() : null;
                data.ListService = od.OrderServiceDetails.Any(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.OrderServiceID == od.ID) ? od.OrderServiceDetails.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.OrderServiceID == od.ID)
              .Select(o => new
              {
                  ID = o.ID,
                  Price = o.Price,
                  BasePrice = o.BasePrice,
                  Name = o.ServicePrice.Name,
                  Type = o.ServicePrice.Type,
                  Amount = o.Quantity,
                  Note = o.Note,
                  Toping = o.Toping
              }).AsEnumerable().Select(o => new ListOrderServiceDetail
              {
                  ID = o.ID,
                  Price = o.Price,
                  BasePrice = o.BasePrice,
                  Name = o.Name,
                  Type = o.Type,
                  Amount = o.Amount,
                  Note = o.Note,
                  Toping = cartBus.ConvertTopingIDtoName(o.Toping)
              }).ToList() : null;
                data.CarShiper = od.ShiperID.HasValue ? od.Shiper.CarShipers.Select(c => new CarInfo
                {
                    VehicleType = c.VehicleType.Name,
                    CarBrand = c.CarBrand,
                    CarColor = c.CarColor,
                    CarModel = c.CarModel,
                    LicensePlates = c.LicensePlates
                }).FirstOrDefault() : null;
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        //Tìm kiếm thông tin khiếu nại
        public IPagedList<ListComplainOuputModel> SearchComplain(int page, string searchKey, int? type)
        {
            try
            {
                var data = cnn.ComplainOrders.Where(c => (type.HasValue ? c.OrderService.TypeBooking.Equals(type.Value) : true)
                && (!String.IsNullOrEmpty(searchKey) ? c.OrderService.Customer.Name.Contains(searchKey) || c.OrderService.Customer.Phone.Contains(searchKey) : true))
                    .Select(c => new ListComplainOuputModel
                    {
                        ID = c.ID,
                        CusName = c.OrderService.Customer.Name,
                        CusPhone = c.OrderService.Customer.Phone,
                        ComplainType = c.OrderService.TypeBooking,
                        CreatedDate = c.CreatedDate,
                        ShiperName = c.OrderService.Shiper.Name
                    }).OrderByDescending(c => c.ID).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListComplainOuputModel>().ToPagedList(1, 1);
            }
        }

        //Lấy chi tiết khiếu nại
        public ComplainDetailOuputModel GetComplainDetail(int id)
        {
            try
            {
                ComplainDetailOuputModel data = new ComplainDetailOuputModel();

                ComplainOrder c = cnn.ComplainOrders.Find(id);
                data.cusName = c.OrderService.Customer.Name;
                data.cusPhone = c.OrderService.Customer.Phone;
                data.cusAddress = c.OrderService.Customer.Address;
                data.ShiperName = c.OrderService.Shiper.Name;
                data.ShiperPhone = c.OrderService.Shiper.Phone;
                data.FinishAddress = c.OrderService.FinishAddress;
                data.BookingType = c.OrderService.TypeBooking;
                data.UserCancel = c.OrderService.UserCancel.HasValue ? (c.OrderService.UserCancel == SystemParam.ORDER_SHIPER_CANCLE ? "Tài xế" : "Khách hànng") : "";
                data.ReasonCancel = c.OrderService.ReasonCancel;
                data.AdminNote = c.AdminNote;
                data.BookingDate = c.OrderService.BookingDate;
                data.ConfirmDate = c.OrderService.ConfirmDate;
                data.CompleteDate = c.OrderService.CompletedDate;
                data.Rateting = c.OrderService.Rate;
                data.Status = c.Status;
                data.PaymentType = c.OrderService.PaymentType;
                data.UsePoint = c.OrderService.UsePoint;
                data.TotalPrice = c.OrderService.TotalPrice;
                data.BasePrice = c.OrderService.BasePrice;
                data.CreatedDate = c.CreatedDate;
                data.CommissionValue = c.OrderService.CouponID.HasValue ? (c.OrderService.Coupon.Percent.HasValue ? c.OrderService.Coupon.Percent.Value : c.OrderService.Coupon.Discount.Value) : 0;

                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ComplainDetailOuputModel();
            }
        }

        public JsonResultModel OrderReview(int OrderID, float Rate, string NoteRate, int Type)
        {
            try
            {
                OrderService od = cnn.OrderServices.Find(OrderID);
                if (od == null)
                {
                    return rpBus.ErrorResult("Không tìm thấy đơn hàng", SystemParam.FAIL);
                }
                else if (Type == SystemParam.RATE_SHIPPER && od.IsRate == SystemParam.ORDER_RATE)
                {
                    return rpBus.ErrorResult("Bạn đã dánh giá tài xế", SystemParam.FAIL);
                }
                else if (Type == SystemParam.RATE_SHOP && od.IsRateShop.GetValueOrDefault() == SystemParam.ORDER_RATE)
                {
                    return rpBus.ErrorResult("Bạn đã dánh giá cửa hàng", SystemParam.FAIL);
                }
                if (Type == SystemParam.RATE_SHIPPER)
                {
                    od.Rate = Rate;
                    od.NoteRate = NoteRate;
                    od.IsRate = SystemParam.ORDER_RATE;
                    cnn.SaveChanges();
                    var sumRate = cnn.OrderServices.Where(x => x.ShiperID == od.ShiperID && x.IsRate == SystemParam.ORDER_RATE).Sum(x => x.Rate);
                    var countRate = cnn.OrderServices.Where(x => x.ShiperID == od.ShiperID && x.IsRate == SystemParam.ORDER_RATE).Count();
                    var shipper = cnn.Shipers.FirstOrDefault(x => x.ID == od.ShiperID);
                    shipper.Rating = sumRate.GetValueOrDefault() / countRate;
                    cnn.SaveChanges();
                }
                if (Type == SystemParam.RATE_SHOP)
                {
                    od.RateShop = Rate;
                    od.NoteRateShop = NoteRate;
                    od.IsRateShop = SystemParam.ORDER_RATE;
                    cnn.SaveChanges();
                    var sumRateShop = cnn.OrderServices.Where(x => x.ShopID == od.ShopID && x.IsRateShop == SystemParam.ORDER_RATE).Sum(x => x.RateShop);
                    var countRateShop = cnn.OrderServices.Where(x => x.ShopID == od.ShopID && x.IsRateShop == SystemParam.ORDER_RATE).Count();
                    var shop = cnn.Shops.FirstOrDefault(x => x.ID == od.ShopID);
                    shop.Rate = sumRateShop.GetValueOrDefault() / countRateShop;
                    cnn.SaveChanges();
                }
                //pushNotiBus.PushNotifyapp("Khách hàng vừa đánh giá bạn",SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER_ICON);
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public JsonResultModel UpdateOrderReview()
        {
            try
            {
                var shopList = cnn.Shops.Where(x => x.IsActive == SystemParam.ACTIVE && x.ID == 41).ToList();
                foreach (var item in shopList)
                {
                    var sumRateShop = cnn.OrderServices.Where(x => x.ShopID == item.ID && x.IsRateShop == SystemParam.ORDER_RATE).Sum(x => x.RateShop);
                    var countRateShop = cnn.OrderServices.Where(x => x.ShopID == item.ID && x.IsRateShop == SystemParam.ORDER_RATE).Count();
                    var shop = cnn.Shops.FirstOrDefault(x => x.ID == item.ID);
                    if (countRateShop > 0)
                    {
                        shop.Rate = sumRateShop.GetValueOrDefault() / countRateShop;
                    }
                    else
                    {
                        shop.Rate = 0;
                    }

                    cnn.SaveChanges();
                }
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        // hàm xóa complain
        public JsonResultModel DelComplain(int ID)
        {
            try
            {
                ComplainDetailOuputModel data = new ComplainDetailOuputModel();
                ComplainOrder c = cnn.ComplainOrders.Find(ID);
                cnn.ComplainOrders.Remove(c);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }

        }
        public async Task<List<long>> OrderSaleEveryMonth()
        {
            try
            {
                var today = DateTime.Today;
                var listSale = new List<long>();
                for (int i = 1; i <= today.Month; i++)
                {
                    var first = new DateTime(today.Year, i, 1);
                    var last = first.AddMonths(1);
                    // Doanh số đến từ những đơn thanh toán VNPAY
                    var sale = cnn.OrderServices.Where(x => x.StatusPayment == SystemParam.ORDER_PAYMENT && x.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY
                    && x.CreatedDate >= first && x.CreatedDate <= last).Sum(x => (int?)x.TotalPrice) ?? 0;
                    // Doanh số đến từ Tài xế nạp tiền ví cọc
                    var sale2 = cnn.MembersTransactionHistories.Where(x => x.TransactionType.Equals(Constant.TYPE_TRANSACTION_RECHARGE)
                    && x.Status.Equals(Constant.STATUS_TRANSACTION_SUCCESS) && x.CreateDate >= first && x.CreateDate <= last).Sum(x => x.Point) ?? 0;
                    listSale.Add(sale + sale2);
                }
                return listSale;
            }
            catch (Exception e)
            {
                return new List<long>();
            }
        }
        public async Task<long> GetOrderSale()
        {
            try
            {
                var today = DateTime.Today;
                var first = new DateTime(today.Year, 1, 1);
                var last = first.AddYears(1);
                // Doanh số đến từ những đơn thanh toán VNPAY
                var sale = cnn.OrderServices.Where(x => x.StatusPayment == SystemParam.ORDER_PAYMENT && x.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY
                 && x.CreatedDate >= first && x.CreatedDate <= last).Sum(x => (int?)x.TotalPrice) ?? 0;
                // Doanh số đến từ Tài xế nạp tiền ví cọc
                var sale2 = cnn.MembersTransactionHistories.Where(x => x.TransactionType.Equals(Constant.TYPE_TRANSACTION_RECHARGE)
                && x.Status.Equals(Constant.STATUS_TRANSACTION_SUCCESS) && x.CreateDate >= first && x.CreateDate <= last).Sum(x => x.Point) ?? 0;
                return sale + sale2;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}
