using APIProject.App_Start;
using Data.Business;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class StatisticShiperController : BaseController
    {
        // GET: StatisticWasher
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginStatisticShiper = UserLogins.Role;
            ViewBag.Province = reportBusiness.GetListProvince();
            return View();
        }
        [HttpGet]
        public PartialViewResult SearchShiperReport(int page, string searchKey , int? provinceID, int? districtID, string fromDate, string toDate)
        {
            ViewBag.searchKey = searchKey;
            ViewBag.fromDate = fromDate;
            ViewBag.toDate = toDate;
            ViewBag.provinceID = provinceID;
            ViewBag.districtID = districtID;
            return PartialView("_TableWasherReport", reportBusiness.SearchShiperReport(page, searchKey, provinceID, districtID, fromDate, toDate));
        }

        public FileResult ReportExecel(string searchKey, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            return File(reportBusiness.ExportShiperReport(searchKey, provinceID, districtID, fromDate, toDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "report_shipper.xlsx");
        }
    }
}