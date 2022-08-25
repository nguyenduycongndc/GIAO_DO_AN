using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class ReportBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public ReportBusiness(WE_SHIPEntities context = null) : base()
        {
        }

        //lấy list tỉnh/thành phố
        public List<Province> GetListProvince()
        {
            return cnn.Provinces.OrderBy(x => x.Name).ToList();
        }

        //Thống kê theo khánh hàng
        public IPagedList<ReportCustomerModel> SearchCustomerReport(int page, string serchKey, int? provinceID, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);

                //Lấy dữ liệu theo từng khách hàng
                var data = cnn.Customers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(serchKey) ? c.Phone.Contains(serchKey) || c.Name.Contains(serchKey) : true)
                && (provinceID.HasValue ? c.Province.ID.Equals(provinceID.Value) : true))
                    .Select(c => new
                    {
                        ID = c.ID,
                        Name = c.Name,
                        Phone = c.Phone,
                        CountCancle = c.QTYCancel,
                        order = c.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.CustomerID.Equals(c.ID) && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true)),
                    }).AsEnumerable().Select(cs => new ReportCustomerModel
                    {
                        ID = cs.ID,
                        Name = cs.Name,
                        Phone = cs.Phone,
                        CountCancle = cs.CountCancle,
                        CountTransaction = cs.order.Where(o => o.Status.Equals(SystemParam.ORDER_STATUS_DENY)).Count(),
                        Paid = cs.order.Select(o => o.TotalPrice).Sum(),
                        CountCompleteYet = cs.order.Where(o => o.Status.Equals(SystemParam.ORDER_STATUS_FINISH)).Count(),
                        CountPaymentInCash = cs.order.Where(o => o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_ON_DELIVERY)).Count(),
                        CountPaymentInVnPay = cs.order.Where(o => o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VN_PAY)).Count()
                    }).Where(x => x.CountTransaction > 0).OrderByDescending(c => c.CountTransaction).ThenByDescending(c => c.Paid).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ReportCustomerModel>().ToPagedList(1, 1);
            }
        }

        //Xuất bản excel thống kê theo khách hàng

        public ExcelPackage ExportCustomerReport(string serchKey, int? provinceID, string fromDate, string toDate)
        {
            try
            {
                List<ReportCustomerModel> data = GetDataCustomerReport(serchKey, provinceID, fromDate, toDate);
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/report_customer.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;

                if (data != null && data.Count() > 0)
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = stt;
                        sheet.Cells[row, 2].Value = dt.Name;
                        sheet.Cells[row, 3].Value = dt.Phone;
                        sheet.Cells[row, 4].Value = dt.CountTransaction;
                        sheet.Cells[row, 5].Value = string.Format("{0:#,0}", dt.Paid);
                        sheet.Cells[row, 6].Value = dt.CountCompleteYet;
                        sheet.Cells[row, 7].Value = dt.CountCancle;
                        sheet.Cells[row, 8].Value = dt.CountPaymentInVnPay;
                        sheet.Cells[row, 9].Value = dt.CountPaymentInCash;

                        row++;
                        stt++;
                    }
                return pack;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ExcelPackage();
            }
        }

        //Lấy dữ liệu thống kê theo khách hàng
        public List<ReportCustomerModel> GetDataCustomerReport(string serchKey, int? provinceID, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.Customers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(serchKey) ? c.Phone.Contains(serchKey) || c.Name.Contains(serchKey) : true)
               && (provinceID.HasValue ? c.Province.ID.Equals(provinceID.Value) : true))
                     .Select(c => new
                     {
                         ID = c.ID,
                         Name = c.Name,
                         Phone = c.Phone,
                         CountCancle = c.QTYCancel,
                         order = c.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.CustomerID.Equals(c.ID) && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true)),
                     }).AsEnumerable().Select(cs => new ReportCustomerModel
                     {
                         ID = cs.ID,
                         Name = cs.Name,
                         Phone = cs.Phone,
                         CountCancle = cs.CountCancle,
                         CountTransaction = cs.order.Count(),
                         Paid = cs.order.Select(o => o.TotalPrice).Sum(),
                         CountCompleteYet = cs.order.Where(o => o.Status.Equals(SystemParam.ORDER_STATUS_FINISH)).Count(),
                         CountPaymentInCash = cs.order.Where(o => o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_ON_DELIVERY)).Count(),
                         CountPaymentInVnPay = cs.order.Where(o => o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VN_PAY)).Count()
                     }).OrderByDescending(c => c.Paid).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ReportCustomerModel>();
            }
        }


        //Thống kê theo shiper

        public IPagedList<ShiperReportOuputModel> SearchShiperReport(int page, string searchKey, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.Shipers.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(searchKey) ? s.Phone.Contains(searchKey) || s.Name.Contains(searchKey) : true)
                && (provinceID.HasValue ? s.ShiperAreas.Select(x => x.Area.District.ProvinceID).Contains(provinceID.Value) : true)
                             && (districtID.HasValue ? s.ShiperAreas.Select(x => x.Area.DistrictID).Contains(districtID.Value) : true))
                    .Select(s => new
                    {
                        Id = s.ID,
                        name = s.Name,
                        phone = s.Phone,
                        shiperCommission = s.ConfigCommission.MastersBenefit,
                        order = s.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.ShiperID.HasValue && o.ShiperID == s.ID
                        && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true))
                    }).AsEnumerable().Select(s => new ShiperReportOuputModel
                    {
                        Id = s.Id,
                        name = s.name,
                        phone = s.phone,
                        shiperCommission = s.shiperCommission,
                        countTransasction = s.order.Count(),
                        countCompleteYet = s.order.Where(o => o.Status.Equals(SystemParam.ORDER_STATUS_FINISH)).Count(),
                        countCustomerCancel = s.order.Where(o => o.UserCancel.Equals(SystemParam.ORDER_USER_CANCLE)).Count(),
                        countShiperCancel = s.order.Where(o => o.UserCancel.Equals(SystemParam.ORDER_SHIPER_CANCLE)).Count()
                    }).Where(x => x.countTransasction > 0).OrderByDescending(c => c.countTransasction).ThenByDescending(c => c.shiperCommission).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ShiperReportOuputModel>().ToPagedList(1, 1);
            }
        }

        //Xuất bản excel thống kê đối tác
        public ExcelPackage ExportShiperReport(string searchKey, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                List<ShiperReportOuputModel> data = GetDataShiperReport(searchKey, provinceID, districtID, fromDate, toDate);
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/report_shipper.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;
                if (data != null && data.Count() > 0)
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = stt;
                        sheet.Cells[row, 2].Value = dt.name;
                        sheet.Cells[row, 3].Value = dt.phone;
                        sheet.Cells[row, 4].Value = dt.countTransasction;
                        sheet.Cells[row, 5].Value = dt.shiperCommission;
                        sheet.Cells[row, 6].Value = dt.countCompleteYet;
                        sheet.Cells[row, 7].Value = dt.countCustomerCancel;
                        sheet.Cells[row, 8].Value = dt.countShiperCancel;
                        row++;
                        stt++;
                    }
                return pack;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ExcelPackage();
            }
        }
        //Lấy dữ liệu thống kê đối tác
        public List<ShiperReportOuputModel> GetDataShiperReport(string searchKey, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.Shipers.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(searchKey) ? s.Phone.Contains(searchKey) || s.Name.Contains(searchKey) : true)
                && (provinceID.HasValue ? s.ShiperAreas.Select(x => x.Area.District.ProvinceID).Contains(provinceID.Value) : true)
                             && (districtID.HasValue ? s.ShiperAreas.Select(x => x.Area.DistrictID).Contains(districtID.Value) : true))
                    .Select(s => new
                    {
                        Id = s.ID,
                        name = s.Name,
                        phone = s.Phone,
                        shiperCommission = s.ConfigCommission.MastersBenefit,
                        order = s.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.ShiperID.HasValue && o.ShiperID == s.ID
                        && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true))
                    }).AsEnumerable().Select(s => new ShiperReportOuputModel
                    {
                        Id = s.Id,
                        name = s.name,
                        phone = s.phone,
                        shiperCommission = s.shiperCommission,
                        countTransasction = s.order.Count(),
                        countCompleteYet = s.order.Where(o => o.Status.Equals(SystemParam.ORDER_STATUS_FINISH)).Count(),
                        countCustomerCancel = s.order.Where(o => o.UserCancel.Equals(SystemParam.ORDER_USER_CANCLE)).Count(),
                        countShiperCancel = s.order.Where(o => o.UserCancel.Equals(SystemParam.ORDER_SHIPER_CANCLE)).Count()
                    }).OrderByDescending(s => s.shiperCommission).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ShiperReportOuputModel>();
            }
        }

        //báo cáo doanh số
        public IPagedList<StatisticPaymentOuputModel> SearchPaymentReport(int page, string searchKey, int? bookingType, int? paymentType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE)
                && o.Status.Equals(SystemParam.ORDER_STATUS_FINISH)
                && (provinceID.HasValue ? o.Area.District.ProvinceID.Equals(provinceID.Value) : true)
                && (districtID.HasValue ? o.Area.DistrictID.Equals(districtID.Value) : true)
                && (!String.IsNullOrEmpty(searchKey) ? o.Customer.Name.Contains(searchKey) || o.Customer.Phone.Contains(searchKey) : true)
                && (paymentType.HasValue ? o.PaymentType.Equals(paymentType.Value) : true)
                && (bookingType.HasValue ? o.TypeBooking.Equals(bookingType.Value) : true) && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true))
                    .Select(o => new StatisticPaymentOuputModel
                    {
                        ID = o.ID,
                        cusName = o.Customer.Name,
                        shiperName = o.Shiper.Name,
                        bookingType = o.TypeBooking,
                        bookingDate = o.BookingDate,
                        totalPrice = o.TotalPrice,
                        shiperCommission = o.ShiperCommission.HasValue ? o.ShiperCommission.Value : 0,
                        paymentType = o.PaymentType
                    }).OrderByDescending(o => o.bookingDate).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<StatisticPaymentOuputModel>().ToPagedList(1, 1);
            }
        }

        public ExcelPackage ExportPaymentReport(string searchKey, int? bookingType, int? paymentType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                List<StatisticPaymentOuputModel> data = GetDataPaymentReport(searchKey, bookingType, paymentType, provinceID, districtID, fromDate, toDate);
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/report_order.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;
                if (data != null && data.Count() > 0)
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = stt;
                        sheet.Cells[row, 2].Value = dt.ID;
                        sheet.Cells[row, 3].Value = dt.cusName;
                        switch (dt.bookingType)
                        {
                            case SystemParam.SHIP_DRIVER:
                                sheet.Cells[row, 4].Value = SystemParam.SHIP_DRIVER_STR;
                                break;
                            case SystemParam.SHIP_PACKAGE:
                                sheet.Cells[row, 4].Value = SystemParam.SHIP_PACKAGE_STR;
                                break;
                            case SystemParam.SHIP_FOOD:
                                sheet.Cells[row, 4].Value = SystemParam.SHIP_FOOD_STR;
                                break;
                            default:
                                sheet.Cells[row, 4].Value = "";
                                break;
                        }
                        sheet.Cells[row, 5].Value = dt.bookingDate.ToString(SystemParam.CONVERT_DATETIME);
                        sheet.Cells[row, 6].Value = string.Format("{0:#,0}", dt.totalPrice);
                        sheet.Cells[row, 7].Value = dt.paymentType.Equals(SystemParam.PAYMENT_TYPE_ON_DELIVERY) ? SystemParam.PAYMENT_TYPE_ON_DELIVERY_STR : SystemParam.PAYMENT_TYPE_VN_PAY_STR;
                        sheet.Cells[row, 8].Value = dt.shiperName;
                        sheet.Cells[row, 9].Value = string.Format("{0:#,0}", dt.shiperCommission);
                        row++;
                        stt++;
                    }
                return pack;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ExcelPackage();
            }
        }

        public List<StatisticPaymentOuputModel> GetDataPaymentReport(string searchKey, int? bookingType, int? paymentType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (td.HasValue)
                    td = td.Value.AddDays(1);
                var data = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE)
                && (provinceID.HasValue ? o.Area.District.ProvinceID.Equals(provinceID.Value) : true)
                && (districtID.HasValue ? o.Area.DistrictID.Equals(districtID.Value) : true)
                && o.Status.Equals(SystemParam.ORDER_STATUS_FINISH)
                && (!String.IsNullOrEmpty(searchKey) ? o.Customer.Name.Contains(searchKey) || o.Customer.Phone.Contains(searchKey) : true)
                && (paymentType.HasValue ? o.PaymentType.Equals(paymentType.Value) : true)
                && (bookingType.HasValue ? o.TypeBooking.Equals(bookingType.Value) : true) && (fd.HasValue ? o.CreatedDate >= fd.Value : true) && (td.HasValue ? o.CreatedDate <= td.Value : true))
                    .Select(o => new StatisticPaymentOuputModel
                    {
                        ID = o.ID,
                        cusName = o.Customer.Name,
                        shiperName = o.Shiper.Name,
                        bookingType = o.TypeBooking,
                        bookingDate = o.BookingDate,
                        totalPrice = o.TotalPrice,
                        shiperCommission = o.ShiperCommission.HasValue ? o.ShiperCommission.Value : 0,
                        status = o.Status,
                        paymentType = o.PaymentType
                    }).OrderByDescending(o => o.bookingDate).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<StatisticPaymentOuputModel>();
            }
        }
    }
}
