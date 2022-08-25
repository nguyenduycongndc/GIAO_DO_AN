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
    public class TransactionFoodController : BaseController
    {
        // GET: TransactionFood
        [UserAuthenticationFilter]

        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginTranfood = UserLogins.Role;
            return View();
        }
        //Tìm kiếm
        public PartialViewResult SearchTransactionFood(int page, int? status, string searchKey = "", string fromDate = "", string toDate = "")
        {
            try
            {
                var listcate = transactionFoodBusiness.SearchTransactionFood(page, searchKey, fromDate, toDate, status);
                ViewBag.searchKey = searchKey;
                ViewBag.status = status;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                return PartialView("_tableTransactionFood", listcate);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_tableTransactionFood");
            }
        }
        //Xóa đặt xe
        public JsonResult DeleteTransactionFoods(int id)
        {
            var data = transactionFoodBusiness.DeleteTransactionFoods(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //binding dữ liệu
        public ActionResult TransactionFoodDetail(int id)
        {
            TransactionFoodModel data = transactionFoodBusiness.ViewBindingTransactionFood(id);
            return View(data);
        }
        //xuất excel
        public ActionResult ExportExcelTransactionFood(string searchKey, string fromDate, string toDate, int? status)
        {
            //empEntities context = new empEntities();

            //List<emp_table> FileData = context.emp_table.ToList();
            List<TransactionFoodModel> FileData = transactionFoodBusiness.DataExcel( searchKey,  fromDate,  toDate, status);


            try
            {

                DataTable Dt = new DataTable();
                //Dt.Columns.Add("STT", typeof(string));
                Dt.Columns.Add("Mã đơn hàng", typeof(string));
                Dt.Columns.Add("Khách hàng", typeof(string));
                Dt.Columns.Add("Số lượng sản phẩm", typeof(string));
                Dt.Columns.Add("TG đặt", typeof(string));
                Dt.Columns.Add("Tổng tiền", typeof(string));
                Dt.Columns.Add("Địa chỉ", typeof(string));
                Dt.Columns.Add("Địa chỉ quán", typeof(string));
                Dt.Columns.Add("Cửa hàng", typeof(string));
                Dt.Columns.Add("Tài xế", typeof(string));
                Dt.Columns.Add("Trạng thái thanh toán", typeof(string));
                Dt.Columns.Add("Trạng thái giao dịch", typeof(string));


                foreach (var data in FileData)
                {
                    DataRow row = Dt.NewRow();
                    row[0] = data.Code;
                    row[1] = data.CustomerName;
                    row[2] = data.Quantity;
                    row[3] = data.BookingDate;
                    row[4] = data.TotalPrice;
                    row[5] = data.FinishAddress;
                    row[6] = data.Address;
                    row[7] = data.ShopName;
                    row[8] = data.ShiperName;
                    if (data.StatusPayment == 0)
                    {
                        row[9] = "Chưa thanh toán";
                    }
                    else
                    {
                        row[9] = "Đã thanh toán";
                    }
                    switch (data.Status)
                    {
                        case SystemParam.ORDER_STATUS_DENY:
                            row[10] = "Đơn Shipper bị từ chối";
                            break;
                        case SystemParam.ORDER_STATUS_PENDING:
                            row[10] = "Đơn Shipper chờ tiếp nhận";
                            break;
                        case SystemParam.ORDER_STATUS_DELIVERY:
                            row[10] = "Đơn Shipper đang tiếp nhận";
                            break;
                        case SystemParam.ORDER_STATUS_PICK_UP:
                            row[10] = "Đơn Shipper đã lấy đồ ăn";
                            break;
                        case SystemParam.ORDER_STATUS_FINISH:
                            row[10] = "Đơn Shipper hoàn thành";
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