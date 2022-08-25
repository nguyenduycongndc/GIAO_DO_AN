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
    public class TransactionFoodBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public TransactionFoodBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        //tìm kiếm
        public IPagedList<TransactionFoodModel> SearchTransactionFood(int page, string searchKey, string fromDate, string toDate, int? status)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);
                var query = (from a in cnn.OrderServiceDetails
                             join b in cnn.OrderServices on a.OrderServiceID equals b.ID
                             join c in cnn.Customers on b.CustomerID equals c.ID
                             join e in cnn.Shops on b.ShopID equals e.ID
                             join f in cnn.ServicePrices on a.ServicePriceID equals f.ID
                             orderby b.BookingDate descending
                             group new { a,b,c,e} by a.OrderServiceID into g
                             select new TransactionFoodModel
                             {
                                 ID = g.FirstOrDefault().b.ID,
                                 Code = g.FirstOrDefault().b.Code,
                                 CustomerName = g.FirstOrDefault().c.Name,
                                 CustomerPhone = g.FirstOrDefault().c.Phone,
                                 CustomerAddress = g.FirstOrDefault().c.Address,
                                 ShiperName = g.FirstOrDefault().b.ShiperID.HasValue ? g.FirstOrDefault().b.Shiper.Name : "",
                                 ShiperPhone = g.FirstOrDefault().b.ShiperID.HasValue ? g.FirstOrDefault().b.Shiper.Phone : "",
                                 AreaID = g.FirstOrDefault().b.AreaID,
                                 TotalPrice = g.FirstOrDefault().b.TotalPrice,
                                 FinishAddress = g.FirstOrDefault().b.FinishAddress,
                                 Address = g.FirstOrDefault().b.Address,
                                 StatusPayment = g.FirstOrDefault().b.StatusPayment,
                                 Status = g.FirstOrDefault().b.Status,
                                 CreatedDate = g.FirstOrDefault().b.CreatedDate,
                                 BookingDate = g.FirstOrDefault().b.BookingDate,
                                 IsActive = g.FirstOrDefault().b.IsActive,
                                 TypeBooking = g.FirstOrDefault().b.TypeBooking,
                                 ShopName =g.FirstOrDefault().e.Name,
                                 ShopPhone = g.FirstOrDefault().e.Phone,
                                 PriceServicePrice = g.Sum(x => x.a.Price),
                                 Quantity = g.Sum(x => x.a.Quantity),
                                 BasePrice = g.FirstOrDefault().b.BasePrice,
                                 Note = g.FirstOrDefault().b.Note,
                                 StartDate = g.FirstOrDefault().b.StartDate,
                                 ConfirmDate = g.FirstOrDefault().b.ConfirmDate,
                                 CompletedDate = g.FirstOrDefault().b.CompletedDate,
                                 CancleDate = g.FirstOrDefault().b.CancleDate

                             }).ToList();

                

                var data = query.Where(x => x.TypeBooking == SystemParam.SHIP_FOOD);
                data = data.Where(x => x.IsActive != 0);

                if (!string.IsNullOrEmpty(searchKey))
                {
                    data = data.Where(x => x.CustomerName.ToUpper().Trim().Contains(searchKey.ToUpper().Trim())
                    || x.CustomerPhone.ToUpper().Trim().Contains(searchKey.ToUpper().Trim())
                    || x.Code.ToUpper().Trim().Contains(searchKey.ToUpper().Trim()));
                }
                if (status != null)
                {
                    data = data.Where(x => x.Status == status);
                }

                if (!string.IsNullOrEmpty(fromDate))
                {
                    data = data.Where(x => (startDate.HasValue ? ((x.CancleDate.HasValue ? x.CancleDate : x.CompletedDate.HasValue ? x.CompletedDate : x.ConfirmDate.HasValue ? x.ConfirmDate : x.CreatedDate) >= startDate.Value.Date) : true));
                    //data = data.Where(x => (startDate.HasValue ? (x.BookingDate.Date >= startDate.Value.Date) : true));
                }

                if (!string.IsNullOrEmpty(toDate))
                {
                    data = data.Where(x => (endDate.HasValue ? ((x.CancleDate.HasValue ? x.CancleDate : x.CompletedDate.HasValue ? x.CompletedDate : x.ConfirmDate.HasValue ? x.ConfirmDate : x.CreatedDate) <= endDate.Value.Date) : true));
                    //data = data.Where(x => (endDate.HasValue ? (x.BookingDate.Date <= endDate.Value.Date) : true));
                }

                return data.OrderByDescending(TranBus => TranBus.BookingDate).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<TransactionFoodModel>().ToPagedList(1, 1);
            }
        }
        //Count TransactionFood
        public long CountTransactionFood()
        {
            var data = (from a in cnn.OrderServiceDetails
                         join b in cnn.OrderServices on a.OrderServiceID equals b.ID
                         join c in cnn.Customers on b.CustomerID equals c.ID
                         join d in cnn.Shipers on b.ShiperID equals d.ID
                         join e in cnn.Shops on b.ShopID equals e.ID
                         join f in cnn.ServicePrices on a.ServicePriceID equals f.ID
                         orderby b.BookingDate descending
                         group a by a.OrderServiceID into g
                         select g).ToList();

            long sumPrice = 0;
            foreach (var dt in data)
            {
                sumPrice += dt.FirstOrDefault().OrderService.TotalPrice;
            }

            return sumPrice;
        }

        //xóa
        public JsonResultModel DeleteTransactionFoods(int id)
        {
            try
            {
                OrderService orderService = cnn.OrderServices.Find(id);
                orderService.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                //return SystemParam.SUCCESS;
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, orderService);

            }
            catch (Exception ex)
            {
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
                //return SystemParam.ERROR;
            }
        }
        //binding dữ liệu
        public TransactionFoodModel ViewBindingTransactionFood(int id)
        {
            try
            {
                var query = (from a in cnn.OrderServiceDetails
                             join b in cnn.OrderServices on a.OrderServiceID equals b.ID into Ord
                             from b in Ord.DefaultIfEmpty()
                             join c in cnn.Customers on b.CustomerID equals c.ID into Cu
                             from c in Cu.DefaultIfEmpty()
                             join d in cnn.Shipers on b.ShiperID equals d.ID into Shi
                             from d in Shi.DefaultIfEmpty()
                             join e in cnn.Shops on b.ShopID equals e.ID into Sho
                             from e in Sho.DefaultIfEmpty()
                             join f in cnn.ServicePrices on a.ServicePriceID equals f.ID into Ser
                             from f in Ser.DefaultIfEmpty()
                             join g in cnn.Coupons on b.CouponID equals g.ID into Cou
                             from g in Cou.DefaultIfEmpty()
                             orderby b.BookingDate descending
                             select new TransactionFoodModel()
                             {
                                 ID = a.OrderService.ID,
                                 Code = a.OrderService.Code,
                                 CustomerName = a.OrderService.Customer.Name,
                                 CustomerPhone = a.OrderService.Customer.Phone,
                                 CustomerAddress = a.OrderService.Customer.Address,
                                 ShiperName = a.OrderService.Shiper.Name,
                                 ShiperPhone = a.OrderService.Shiper.Phone,
                                 AreaID = a.OrderService.AreaID,
                                 TotalPrice = a.OrderService.TotalPrice,
                                 FinishAddress = a.OrderService.FinishAddress,
                                 Address = a.OrderService.Address,
                                 StatusPayment = a.OrderService.StatusPayment,
                                 Status = a.OrderService.Status,
                                 CreatedDate = a.OrderService.CreatedDate,
                                 BookingDate = a.OrderService.BookingDate,
                                 IsActive = a.OrderService.IsActive,
                                 TypeBooking = a.OrderService.TypeBooking,
                                 ShopName = a.OrderService.Shop.Name,
                                 ShopPhone = a.OrderService.Shop.Phone,
                                 ServicePriceName = a.ServicePrice.Name,
                                 PriceServicePrice = a.ServicePrice.Price,
                                 Note = a.Note,
                                 BasePriceService = a.ServicePrice.BasePrice,
                                 StartDate = a.OrderService.StartDate,
                                 ConfirmDate = a.OrderService.ConfirmDate,
                                 CompletedDate = a.OrderService.CompletedDate,
                                 PaymentType = a.OrderService.PaymentType,
                                 Calculate = (a.BasePrice * a.OrderService.Coupon.Percent) / 100,
                                 UsePoint = a.OrderService.UsePoint,
                                 CouponID = a.OrderService.Coupon.ID,
                                 CouponCode = a.OrderService.Coupon.Code != null ? a.OrderService.Coupon.Code : "",
                                 CouponDiscount = a.OrderService.Coupon.Discount,
                                 TypeCoupon = a.OrderService.Coupon.TypeCoupon,
                                 Percent = a.OrderService.Coupon.Percent,
                                 Discount = a.OrderService.Coupon.Discount,
                                 BasePrice = a.OrderService.BasePrice,
                                 ShiperID = a.OrderService.ShiperID,
                                 ShopID = a.OrderService.ShopID,
                                 RateShop = (float?)a.OrderService.RateShop,
                                 Rate = (float?)a.OrderService.Rate,
                                 RateNote = a.OrderService.NoteRate,
                                 RateShopNote = a.OrderService.NoteRateShop
                             }
                             );
                var data = query.Where(x => x.ID == id).FirstOrDefault();

                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new TransactionFoodModel();
            }
        }
        //data excel
        public List<TransactionFoodModel> DataExcel(string searchKey, string fromDate, string toDate, int? status)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(fromDate);
                DateTime? endDate = Util.ConvertToDate(toDate);
                var query = (from a in cnn.OrderServiceDetails
                             join b in cnn.OrderServices on a.OrderServiceID equals b.ID
                             join c in cnn.Customers on b.CustomerID equals c.ID
                             join d in cnn.Shipers on b.ShiperID equals d.ID
                             join e in cnn.Shops on b.ShopID equals e.ID
                             join f in cnn.ServicePrices on a.ServicePriceID equals f.ID
                             orderby b.BookingDate descending
                             group a by a.OrderServiceID into g
                             select g);

                var newList = new List<TransactionFoodModel>();

                foreach (var item in query)
                {
                    var quantity = 0;
                    var transactionFood = new TransactionFoodModel();
                    transactionFood.ID = item.FirstOrDefault().OrderService.ID;
                    transactionFood.Code = item.FirstOrDefault().OrderService.Code;
                    transactionFood.CustomerName = item.FirstOrDefault().OrderService.Customer.Name;
                    transactionFood.CustomerPhone = item.FirstOrDefault().OrderService.Customer.Phone;
                    transactionFood.CustomerAddress = item.FirstOrDefault().OrderService.Customer.Address;
                    transactionFood.ShiperName = item.FirstOrDefault().OrderService.Shiper.Name;
                    transactionFood.ShiperPhone = item.FirstOrDefault().OrderService.Shiper.Phone;
                    transactionFood.AreaID = item.FirstOrDefault().OrderService.AreaID;
                    transactionFood.TotalPrice = item.FirstOrDefault().OrderService.TotalPrice;
                    transactionFood.FinishAddress = item.FirstOrDefault().OrderService.FinishAddress;
                    transactionFood.Address = item.FirstOrDefault().OrderService.Address;
                    transactionFood.StatusPayment = item.FirstOrDefault().OrderService.StatusPayment;
                    transactionFood.Status = item.FirstOrDefault().OrderService.Status;
                    transactionFood.CreatedDate = item.FirstOrDefault().OrderService.CreatedDate;
                    transactionFood.BookingDate = item.FirstOrDefault().OrderService.BookingDate;
                    transactionFood.IsActive = item.FirstOrDefault().OrderService.IsActive;
                    transactionFood.TypeBooking = item.FirstOrDefault().OrderService.TypeBooking;
                    transactionFood.ShopName = item.FirstOrDefault().OrderService.Shop.Name;
                    transactionFood.ShopPhone = item.FirstOrDefault().OrderService.Shop.Phone;
                    transactionFood.ServicePriceName = item.FirstOrDefault().ServicePrice.Name;
                    transactionFood.PriceServicePrice = item.FirstOrDefault().ServicePrice.Price;
                    transactionFood.Note = item.FirstOrDefault().Note;
                    transactionFood.BasePriceService = item.FirstOrDefault().ServicePrice.BasePrice;
                    transactionFood.StartDate = item.FirstOrDefault().OrderService.StartDate;
                    transactionFood.ConfirmDate = item.FirstOrDefault().OrderService.ConfirmDate;
                    transactionFood.CompletedDate = item.FirstOrDefault().OrderService.CompletedDate;
                    foreach (var group in item)
                    {
                        quantity += group.Quantity;
                    }
                    transactionFood.Quantity = quantity;

                    newList.Add(transactionFood);
                }

                var data = newList.Where(x => x.TypeBooking == SystemParam.SHIP_FOOD);
                data = data.Where(x => x.IsActive != 0);

                if (!string.IsNullOrEmpty(searchKey))
                {
                    data = data.Where(x => x.CustomerName.ToUpper().Trim().Contains(searchKey.ToUpper().Trim())
                    || x.Code.ToUpper().Trim().Contains(searchKey.ToUpper().Trim()));
                }
                if (status != null)
                {
                    data = data.Where(x => x.Status == status);
                }

                if (!string.IsNullOrEmpty(fromDate))
                {
                    data = data.Where(x => (startDate.HasValue ? ((x.CancleDate.HasValue ? x.CancleDate : x.CompletedDate.HasValue ? x.CompletedDate : x.ConfirmDate.HasValue ? x.ConfirmDate : x.CreatedDate) >= startDate.Value.Date) : true));
                    //data = data.Where(x => (startDate.HasValue ? (x.BookingDate.Date >= startDate.Value.Date) : true));
                }

                if (!string.IsNullOrEmpty(toDate))
                {
                    data = data.Where(x => (endDate.HasValue ? ((x.CancleDate.HasValue ? x.CancleDate : x.CompletedDate.HasValue ? x.CompletedDate : x.ConfirmDate.HasValue ? x.ConfirmDate : x.CreatedDate) <= endDate.Value.Date) : true));
                    //data = data.Where(x => (endDate.HasValue ? (x.BookingDate.Date <= endDate.Value.Date) : true));
                }

                return data.OrderByDescending(TranBus => TranBus.BookingDate).ToList();
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<TransactionFoodModel>().ToList();
            }
        }
    }
}
