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
    public class StatisticCustomerController : BaseController
    {
        // GET: StatisticCustomer
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginStatisticCustomer = UserLogins.Role;
            ViewBag.Province = reportBusiness.GetListProvince();
            return View();
        }
        [HttpGet]
        public PartialViewResult SearchCustomerReport(int page, string serchKey = null, int? provinceID = null, string fromDate = null, string toDate = null)
        {
            ViewBag.searchKey = serchKey;
            ViewBag.provinceID = provinceID;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            return PartialView("_TableCustomerReport", reportBusiness.SearchCustomerReport(page, serchKey, provinceID, fromDate, toDate));
        }
        /// <summary>
        /// Xuất bản excel thống kê theo khách hàng
        /// </summary>
        /// <param name="serchKey"></param>
        /// <param name="provinceID"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public FileResult ExportListCusReport(string serchKey, int? provinceID, string fromDate, string toDate)
        {
            return File(reportBusiness.ExportCustomerReport(serchKey, provinceID, fromDate, toDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "report_customer.xlsx");
        }
    }
}