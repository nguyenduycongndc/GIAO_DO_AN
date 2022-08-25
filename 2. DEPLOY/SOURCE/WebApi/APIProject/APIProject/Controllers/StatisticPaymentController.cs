using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Model.APIWeb;
using Data.Business;
using PagedList;
using Data.Utils;
using APIProject.App_Start;

namespace APIProject.Controllers
{
    public class StatisticPaymentController : BaseController
    {
        // GET: StatisticPayment

        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginStatisticPayment = UserLogins.Role;
            ViewBag.Province = reportBusiness.GetListProvince();
            return View();
        }

        public PartialViewResult SearchPaymentReport(int page, string searchKey , int? bookingType, int? paymentType , int? provinceID, int? districtID, string fromDate , string toDate )
        {
            ViewBag.searchKey = searchKey;
            ViewBag.bookingType = bookingType;
            ViewBag.paymentType = paymentType;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.provinceID = provinceID;
            ViewBag.districtID = districtID;
            return PartialView("_TablePaymentReport", reportBusiness.SearchPaymentReport(page, searchKey, bookingType, paymentType, provinceID, districtID, fromDate, toDate));
        }
        public FileResult ExportListPayReport(string searchKey, int? bookingType, int? paymentType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            return File(reportBusiness.ExportPaymentReport(searchKey, bookingType, paymentType, provinceID, districtID, fromDate, toDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "report_order.xlsx");
        }
    }
}