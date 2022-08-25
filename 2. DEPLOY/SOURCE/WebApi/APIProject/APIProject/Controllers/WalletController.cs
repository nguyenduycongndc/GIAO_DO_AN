using APIProject.App_Start;
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
    [UserAuthenticationFilter(2, 0)]
    public class WalletController : BaseController
    {
        // GET: Wallet
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginWallet = UserLogins.Role;
            ViewBag.Province = reportBusiness.GetListProvince();
            return View();
        }

        [UserAuthenticationFilter]
        public PartialViewResult Search(int page, int status, string searchKey = "", string fromDate = "", string toDate = "")
        {
            ViewBag.Searchkey = searchKey;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            //IPagedList<WalletModel> list = ((List<WalletModel>)walletBusiness.Search(searchKey, fromDate, toDate).Result).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return PartialView("_listWallet"/*, list*/);
        }

        //Nút Tìm kiếm
        public PartialViewResult SearchWallet(int page, string searchKey , int? walletType, int? transactionType, int? provinceID, int? districtID, string fromDate , string toDate )
        {
            try
            {
                var listWallet = walletBusiness.SearchWallet(page, searchKey, walletType, transactionType, provinceID, districtID, fromDate, toDate);
                ViewBag.Searchkey = searchKey;
                ViewBag.fromDate = fromDate;
                ViewBag.toDate = toDate;
                ViewBag.provinceID = provinceID;
                ViewBag.districtID = districtID;
                ViewBag.walletType = walletType;
                ViewBag.transactionType = transactionType;
                return PartialView("_listWallet", listWallet);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_listWallet");
            }
        }

        public FileResult WalletExecel(string searchKey, int? walletType, int? transactionType, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            return File(walletBusiness.ExportWallet(searchKey, walletType, transactionType, provinceID, districtID, fromDate, toDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "wallet_transaction.xlsx");
        }
    }
}