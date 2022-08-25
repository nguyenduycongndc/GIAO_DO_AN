using APIProject.App_Start;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class TransactionController : BaseController
    {
        [UserAuthenticationFilter]
        // GET: Transaction
        public ActionResult Index()
        {
            //    ViewBag.Search = "";
            //    ViewBag.FromDate = "";
            //    ViewBag.ToDate = "";
            //    ViewBag.Status = null;
            //    ViewBag.ServiceID = null;
            //    ViewBag.listService = serviceBusiness.Search();
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginTran = UserLogins.Role;
            return View();
        }
        //    [UserAuthenticationFilter]
        //    public PartialViewResult Search(int page, string search, string fromDate, string toDate, int? status, int? serviceID, int Addservice)
        //    {
        //        ViewBag.Search = search;
        //        ViewBag.FromDate = fromDate;
        //        ViewBag.ToDate = toDate;
        //        ViewBag.Status = status;
        //        ViewBag.ServiceID = serviceID;
        //        ViewBag.Addservice = Addservice;
        //        ViewBag.listService = serviceBusiness.Search().Select(u => u.ID).ToList();
        //        IPagedList<GetTransactionModel> list = pointBusiness.Search(search, fromDate, toDate, status, serviceID, Addservice, page);
        //        return PartialView("_listTransaction", list);
        //    }
        //    public JsonResult TestSearch(string search, string fromDate, string toDate, int? status, int? serviceID)
        //    {
        //        var list = new object();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public JsonResult GetListService()
        //    {
        //        var list = orderServiceBus.GetListService();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public JsonResult GetListCarBrand()
        //    {
        //        var list = carBusiness.GetCarBrandSelect();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public JsonResult GetListCarModelByBrand(int brandId)
        //    {
        //        var list = carBusiness.GetCarModelSelect(brandId);
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    //public JsonResult GetListModelByBrand(string brandName, string search)
        //    //{
        //    //    var list = ((List<ListCarBrandModel>)carBusiness.GetListCarModel(brandName, search)).Select(x => x.listCarMode);
        //    //    return Json(list, JsonRequestBehavior.AllowGet);
        //    //}
        //    [UserAuthenticationFilter]
        //    [HttpPost]
        //    public JsonResult GetListCustomerAutocomplete(string cusName = "")
        //    {
        //        var list = cusBusiness.GetListCustomer(cusName);
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    public JsonResult TestDetail(int id)
        //    {
        //        var list = orderServiceBus.GetOrderServiceDetail(id, null, "en");
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }

        //    [UserAuthenticationFilter]
        //    public JsonResult GetCarModelByCusId(int cusId)
        //    {
        //        var list = carBusiness.GetCarModelByCusId(cusId);
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public JsonResult GetListPackageService(int type, int segmentId)
        //    {
        //        var list = orderServiceBus.GetListServiceByType(type, segmentId);
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter(2, 0)]
        //    [HttpPost]
        //    public JsonResult CreateCustomer(string name, string phone, string password, int carModelId, string license, string color)
        //    {
        //        var result = cusBusiness.AddCustomerWeb(name, phone, password, carModelId, license, color);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public JsonResult GetListShift(string Date)
        //    {
        //        var list = orderServiceBus.GetListShift(Date);
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter(2, 0)]
        //    [HttpPost]
        //    public JsonResult CreateTransaction(CreateTransactionModel data)
        //    {
        //        if (data.additionService == null)
        //        {
        //            data.additionService = new List<int>();
        //        }
        //        var result = orderServiceBus.AddTransaction(data);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public PartialViewResult Booking()
        //    {
        //        return PartialView("_Booking");
        //    }
        //    [UserAuthenticationFilter]
        //    public PartialViewResult Detail(int id)
        //    {
        //        ViewBag.listWasher = orderServiceBus.listWasher(id);
        //        var list = orderServiceBus.GetOrderServiceDetail(id, null, "en");
        //        return PartialView("_Detail", list);
        //    }
        //    [HttpPost]
        //    [UserAuthenticationFilter(2, 0)]
        //    public string CancelTransaction(int id, string Content)
        //    {
        //        return orderServiceBus.CancelTransaction(id, Content);
        //    }
        //    [HttpPost]
        //    [UserAuthenticationFilter(2, 3)]
        //    public JsonResult DeleteTransaction(int id)
        //    {
        //        var result = orderServiceBus.DeleteTransaction(id);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    [UserAuthenticationFilter]
        //    public FileResult ExportRequest(string search, string fromDate, string toDate, int? status, int? serviceID, int AddService)
        //    {
        //        return File(pointBusiness.ExportExcel(search, fromDate, toDate, status, serviceID, AddService).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "List_Transaction.xlsx");
        //    }
        //    [HttpPost]
        //    [UserAuthenticationFilter(2, 0)]
        //    public JsonResult ChangeWasher(int orderID = 0, int washerID = 0)
        //    {
        //        var result = orderServiceBus.ChangeWasher(orderID, washerID);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //Tìm kiếm
        public PartialViewResult SearchTransaction(int page, int? status, string searchKey = "", string fromDate = "", string toDate = "")
        {
            try
            {
                var listcate = transactionBusiness.SearchTransaction(page, searchKey, fromDate, toDate, status);
                ViewBag.searchKey = searchKey;
                ViewBag.status = status;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                return PartialView("_tableTransaction", listcate);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_tableTransaction");
            }
        }
        //Xóa đặt xe
        public JsonResult DeleteTransactionBookCar(int id)
        {
            var data = transactionBusiness.DeleteTransactionBookCars(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //binding dữ liệu
        public ActionResult TransactionDetail(int id)
        {
            GetBookCar data = transactionBusiness.ViewBindingTransaction(id);
            return View(data);
        }
        //xuất excel
        public ActionResult ExportExcelTransactionBookCar(string searchKey, string fromDate, string toDate, int? status)
        {
            //empEntities context = new empEntities();

            //List<emp_table> FileData = context.emp_table.ToList();
            List<GetBookCar> FileData = transactionBusiness.DataExcel(searchKey, fromDate, toDate, status);


            try
            {

                DataTable Dt = new DataTable();
                Dt.Columns.Add("Mã đơn hàng", typeof(string));
                Dt.Columns.Add("Khách hàng", typeof(string));
                Dt.Columns.Add("TG đặt", typeof(string));
                Dt.Columns.Add("Tổng tiền", typeof(string));
                Dt.Columns.Add("Địa chỉ đi", typeof(string));
                Dt.Columns.Add("Địa chỉ đến", typeof(string));
                Dt.Columns.Add("Tài xế", typeof(string));
                Dt.Columns.Add("Trạng thái thanh toán", typeof(string));
                Dt.Columns.Add("Trạng thái giao dịch", typeof(string));


                foreach (var data in FileData)
                {
                    DataRow row = Dt.NewRow();
                    row[0] = data.Code;
                    row[1] = data.CustomerName;
                    row[2] = data.BookingDate;
                    row[3] = data.TotalPrice;
                    row[4] = data.Address;
                    row[5] = data.FinishAddress;
                    row[6] = data.ShiperName;
                    if (data.StatusPayment == 0)
                    {
                        row[7] = "Chưa thanh toán";
                    }
                    else
                    {
                        row[7] = "Đã thanh toán";
                    }
                    switch (data.Status)
                    {
                        case SystemParam.ORDER_STATUS_DENY:
                            row[8] = "Đơn Shipper bị từ chối";
                            break;
                        case SystemParam.ORDER_STATUS_PENDING:
                            row[8] = "Đơn Shipper chờ tiếp nhận";
                            break;
                        case SystemParam.ORDER_STATUS_DELIVERY:
                            row[8] = "Đơn Shipper đang tiếp nhận";
                            break;
                        case SystemParam.ORDER_STATUS_PICK_UP:
                            row[8] = "Đơn Shipper đã đón khách";
                            break;
                        case SystemParam.ORDER_STATUS_FINISH:
                            row[8] = "Đơn Shipper hoàn thành";
                            break;
                    }
                    Dt.Rows.Add(row);

                }

                var memoryStream = new MemoryStream();
                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                    worksheet.Cells["A1"].LoadFromDataTable(Dt, true, TableStyles.None);
                    worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                    worksheet.DefaultRowHeight = 18;


                    worksheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    worksheet.Column(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.DefaultColWidth = 20;
                    worksheet.Column(2).AutoFit();

                    Session["DownloadExcel_FileManager"] = excelPackage.GetAsByteArray();
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        //dowload excel về máy
        public ActionResult Download()
        {

            if (Session["DownloadExcel_FileManager"] != null)
            {
                byte[] data = Session["DownloadExcel_FileManager"] as byte[];
                return File(data, "application/octet-stream", "FileManager.xlsx");
            }
            else
            {
                return new EmptyResult();
            }
        }

    }
}