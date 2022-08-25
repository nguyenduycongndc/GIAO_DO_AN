using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using PagedList.Mvc;
using Data.Business;
using Data.DB;
using System.Web.Http;

namespace APIProject.Controllers
{
    public class ShopController : BaseController
    {
        public WE_SHIPEntities cnn = new WE_SHIPEntities();
        // GET: Shop
        [UserAuthenticationFilter]

        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginShop = UserLogins.Role;
            ViewBag.Province = from p in cnn.Provinces orderby p.Name ascending select p;
            return View();
        }
        public FileResult ExportListShop(string searchKey, int? status,int? provinceID,int? districtID, string fromDate, string toDate)
        {
            return File(shopBusiness.ExportListShop(searchKey, status, provinceID, districtID, fromDate, toDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "listShop.xlsx");
        }
        public ActionResult Create()
        {
            ViewBag.Province = from p in cnn.Provinces orderby p.Name ascending select p;
            return View();
        }
        public JsonResult CreateShop(string urlAvatar, string urlCertificate, string Name, string Phone, string Email, int ProvinceId, int DistrictId, string Address, float Long, float Lat)
        {
            var result = shopBusiness.CreateShop(urlAvatar, urlCertificate, Name, Phone, Email, ProvinceId, DistrictId, Address, Long, Lat);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //[UserAuthenticationFilter]

        public PartialViewResult Search(int Page, string searchKey,int? provinceID,int? districtID, int? status, string FromDate, string ToDate)
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginShop = UserLogins.Role;
            ViewBag.searchKey = searchKey;
            ViewBag.status = status;
            ViewBag.fromDate = FromDate;
            ViewBag.toDate = ToDate;
            ViewBag.provinceID = provinceID;
            ViewBag.districtID = districtID;
            
            return PartialView("_TableShop", shopBusiness.Search(Page, searchKey, status,provinceID,districtID, FromDate, ToDate));
        }
        public ActionResult GetShopDetail(int ID)
        {
            var shopDetail = cnn.Shops.Find(ID);
            return View(shopDetail);
        }
        public PartialViewResult searchHistoryBusiness(int page, int ShopID)
        {
            ViewBag.ShopID = ShopID;
            var shopHistoryBusiness = shopBusiness.searchHistoryBusiness(page, ShopID);
            return PartialView("ListShopBusiness", shopHistoryBusiness);
        }
        public PartialViewResult searchServicePrice(int page, int ShopID)
        {
            ViewBag.ShopID = ShopID;
            var ServicePrice = shopBusiness.searchServicePrice(page, ShopID);
            return PartialView("ListServicePrice", ServicePrice);
        }
        public JsonResult LoadDistrict(int? ProvinceID)
        {
            if (ProvinceID.HasValue)
            {
                var lstD = (from d in cnn.Districts where d.ProvinceID == ProvinceID orderby d.Name select new { d.ID, d.Name }).ToList();
                return Json(lstD, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult loadModalEditShop(int ID)
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginShop = UserLogins.Role;
            ViewBag.lstProvince = from p in cnn.Provinces select p;
            var query = shopBusiness.loadModalEditShop(ID);
            return PartialView("_EditShop", query);
        }
        // Xóa Shipper 
        [UserAuthenticationFilter]
        public int DeleteShop(int ID)
        {
            try
            {
                return shopBusiness.DeleteShop(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public int InActiveShop(int ID)
        {
            try
            {
                return shopBusiness.InActiveShop(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        /// <summary>
        /// Cập nhật thông tin shop
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult UpdateShopInfo([FromBody] ListShopOutputModel input)
        {
            return Json(shopBusiness.UpdateShopInfo(input), JsonRequestBehavior.AllowGet);
        }
    }
}