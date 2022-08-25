using APIProject.App_Start;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using Data.DB;


namespace APIProject.Controllers
{
    public class TransactionDeliveryController : BaseController
    {
        public WE_SHIPEntities cnn = new WE_SHIPEntities();
        // GET: TransactionDelivery
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginTranDelivery = UserLogins.Role;
            ViewBag.Province = from p in cnn.Provinces orderby p.Name ascending select p;
            return View();
        }
        //Tìm kiếm
        public PartialViewResult SearchTransactionDelivery(int page, int? status,int? provinceID, int? districtID, int? transportType, int? isVip, string searchKey = "", string fromDate = "", string toDate = "")
        {
            try
            {
                var listcate = transactionDeliveryBusiness.SearchTransactionDelivery(page, searchKey , provinceID, districtID, fromDate, toDate, status, transportType,SystemParam.IN_PROVINCE, isVip);
                ViewBag.searchKey = searchKey;
                ViewBag.status = status;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.transportType = transportType;
                ViewBag.isVip = isVip;
                ViewBag.provinceID = provinceID;
                ViewBag.districtID = districtID;
                return PartialView("_tableTransactionDelivery", listcate);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_tableTransactionDelivery");
            }
        }
        //Xóa đặt xe
        public JsonResult DeleteTransactionDelivery(int id)
        {
            var data = transactionDeliveryBusiness.DeleteTransactionDeliverys(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //binding dữ liệu
        public ActionResult TransactionDeliveryDetail(int id)
        {
            TransactionDeliveryModel data = transactionDeliveryBusiness.ViewBindingTransactionDelivery(id);
            return View(data);
        }
        //lưu khi sửa
        public JsonResult SaveUpdateStatus(TransactionDeliveryModel transactionDeliveryModel)
        {
            try
            {
                var query = transactionDeliveryBusiness.SaveUpdateStatus(transactionDeliveryModel);
                //return PartialView("Index", query);
                return Json(query);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //xuất excel
        public ActionResult ExportExcelTransactionDelivery(string searchKey, int? provinceID, int? districtID, string fromDate, string toDate, int? status, int? transportType, int? isVip)
        {
            //empEntities context = new empEntities();

            //List<emp_table> FileData = context.emp_table.ToList();
            List<TransactionDeliveryModel> FileData = transactionDeliveryBusiness.DataExcel(searchKey, provinceID, districtID, fromDate, toDate, status, transportType,SystemParam.IN_PROVINCE, isVip);
            try
            {

                DataTable Dt = new DataTable();
                //Dt.Columns.Add("STT", typeof(string));
                Dt.Columns.Add("Mã đơn hàng", typeof(string));
                Dt.Columns.Add("Khách hàng", typeof(string));
                Dt.Columns.Add("SĐT", typeof(string));
                Dt.Columns.Add("Loại hàng", typeof(string));
                Dt.Columns.Add("TG đặt", typeof(string));
                Dt.Columns.Add("TG hoàn thành", typeof(string));
                Dt.Columns.Add("Địa chỉ lấy hàng", typeof(string));
                Dt.Columns.Add("Địa chỉ giao hàng", typeof(string));
                Dt.Columns.Add("Người gửi", typeof(string));
                Dt.Columns.Add("SĐT người gửi", typeof(string));
                Dt.Columns.Add("Người nhận", typeof(string));
                Dt.Columns.Add("SĐT người nhận", typeof(string));
                Dt.Columns.Add("Loại giao hàng", typeof(string));
                Dt.Columns.Add("Tài xế", typeof(string));
                Dt.Columns.Add("Thông tin thu hộ", typeof(string));
                Dt.Columns.Add("Giá trị hàng", typeof(string));
                Dt.Columns.Add("Cước Phí", typeof(string));
                Dt.Columns.Add("Khuyến mãi", typeof(string));
                Dt.Columns.Add("Người nhận thanh toán", typeof(string));
                Dt.Columns.Add("Trạng thái thanh toán", typeof(string));
                Dt.Columns.Add("Trạng thái giao dịch", typeof(string));
                Dt.Columns.Add("Ghi chú", typeof(string));
                foreach (var data in FileData)
                {
                    DataRow row = Dt.NewRow();
                    row[0] = data.Code;
                    row[1] = data.CustomerName;
                    row[2] = data.CustomerPhone;
                    row[3] = data.PackageType;
                    row[4] = data.BookingDate;
                    row[5] = data.CompletedDate;
                    row[6] = data.Address;
                    row[7] = data.FinishAddress;
                    row[8] = data.SenderName;
                    row[9] = data.SenderPhone;
                    row[10] = data.RecevieverName;
                    row[11] = data.RecevieverPhone;
                    switch (data.TransportType)
                    {
                        case SystemParam.TRANSPORT_TYPE_STANDARD:
                            row[12] = "Tiêu chuẩn";
                            break;
                        case SystemParam.TRANSPORT_TYPE_FAST:
                            row[12] = "Nhanh";
                            break;
                        case SystemParam.TRANSPORT_TYPE_WESEN:
                            row[12] = "Siêu tốc nội thành";
                            break;
                        case SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE:
                            row[12] = "Siêu tốc nội thành - VIP";
                            break;
                        case SystemParam.TRANSPORT_TYPE_VIP_STANDARD:
                            row[12] = "Tiêu chuẩn - VIP";
                            break;
                        case SystemParam.TRANSPORT_TYPE_VIP_AIRLINE:
                            row[12] = "Nhanh - VIP";
                            break;
                    }
                    row[13] = data.ShiperName;
                    row[14] = data.CODFee;
                    row[15] = data.PackageFee;
                    row[16] = data.TotalPrice;
                    row[17] = data.CouponDiscount;
                    row[18] = data.IsReceiverPayment == SystemParam.ORDER_RECEIVER_PAYMENT ? "Có" : "Không";
                    if (data.StatusPayment == 0)
                    {
                        row[19] = "Chưa thanh toán";
                    }
                    else
                    {
                        row[19] = "Đã thanh toán";
                    }
                    switch (data.Status)
                    {
                        case SystemParam.ORDER_STATUS_DENY:
                            row[20] = "Đơn Shipper bị từ chối";
                            break;
                        case SystemParam.ORDER_STATUS_PENDING:
                            row[20] = "Đơn Shipper chờ tiếp nhận";
                            break;
                        case SystemParam.ORDER_STATUS_DELIVERY:
                            row[20] = "Đơn Shipper đang tiếp nhận";
                            break;
                        case SystemParam.ORDER_STATUS_PICK_UP:
                            row[20] = "Đơn Shipper đã đón khách, đã lấy hàng, đồ ăn";
                            break;
                        case SystemParam.ORDER_STATUS_FINISH:
                            row[20] = "Đơn Shipper hoàn thành";
                            break;
                    }
                    row[21] = data.Note;
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
                    worksheet.Column(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    worksheet.Column(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
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