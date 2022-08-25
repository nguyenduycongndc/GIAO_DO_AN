using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Business;
using Data.Utils;
using APIProject.App_Start;
using Newtonsoft.Json;
using PagedList;
using Data.Model;
using System.Web.Http;
using Data.DB;

namespace APIProject.Controllers
{
    public class ShipperController : BaseController
    {
        // GET: Shipper
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginShipper = UserLogins.Role;
            ViewBag.Province = from p in Context.Provinces orderby p.Name ascending select p;
            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult Create()
        {

            var cc = from conf in Context.ConfigCommissions
                     where conf.IsActive == SystemParam.ACTIVE
                     select conf;
            var vehicle = from v in Context.VehicleTypes
                          where v.IsActive == SystemParam.ACTIVE
                          orderby v.Name
                          select v;
            ViewBag.VehicleType = vehicle.ToList();
            ViewBag.Conf = cc.ToList();
            ViewBag.area = shipperBusiness.GetArea();
            return View();
        }
        
        [UserAuthenticationFilter]
        public ActionResult Edit(int ID)
        {
            var area = from a in Context.Areas
                       orderby a.Name
                       select a;
            var cc = from conf in Context.ConfigCommissions
                     where conf.IsActive == SystemParam.ACTIVE
                     select conf;
            var vehicle = from v in Context.VehicleTypes
                          where v.IsActive == SystemParam.ACTIVE
                          orderby v.Name
                          select v;
            ViewBag.VehicleType = vehicle.ToList();
            ViewBag.Conf = cc.ToList();
            ViewBag.Area = area.ToList();
            return View(shipperBusiness.GetShiperDetail(ID));
        }
        [UserAuthenticationFilter]
        public int AddPointShip(string ListIdPoint, string Point, string Content)
        {
            try
            {
                //UserDetailOutputModel userLogin = UserLogins;
                return shipperBusiness.AddPointShip(ListIdPoint, Point, Content);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.ERROR;
            }
        }
        [UserAuthenticationFilter]
        public ActionResult GetShipperDetail(int ID)
        {
            try
            {
                var obj = shipperBusiness.GetDetailShipper(ID);
                return View("_GetShipperDetail", obj);
            }
            catch
            {
                return View("_GetShipperDetail", new ShipperOutputModel());
            }
        }
        //tìm kiếmmm
        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, int? Status, string Key , int? provinceID, int? districtID,int? IsVip, string FromDate, string ToDate)
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLoginShipper = UserLogins.Role;
            ViewBag.Status = Status;
            ViewBag.Key = Key;
            ViewBag.FromDate = FromDate;
            ViewBag.ToDate = ToDate;
            ViewBag.provinceID = provinceID;
            ViewBag.districtID = districtID;
            ViewBag.IsVip = IsVip;
            return PartialView("_ListShipper", shipperBusiness.Search(Page, Status, Key, provinceID, districtID,IsVip, FromDate, ToDate));
        }
        public FileResult ExportListShipper(string searchKey, int? status, int? provinceID, int? districtID, int? IsVip, string fromDate, string toDate)
        {
            return File(shipperBusiness.ExportListShipper( status, searchKey, provinceID, districtID, IsVip, fromDate, toDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "listShipper.xlsx");
        }
        [System.Web.Http.HttpPost]
        public JsonResult EditShipper([FromBody] ShiperInputModel input)
        {
            return Json(shipperBusiness.EditShipper(input), JsonRequestBehavior.AllowGet);
        }

        [UserAuthenticationFilter]
        public PartialViewResult GetListBusiness(int Page, int ShipperID)
        {
            try
            {
                ViewBag.ShipperID = ShipperID;
                var res = shipperBusiness.GetListBusiness(Page, ShipperID);
                return PartialView("_TableBusiness", res);
            }
            catch
            {
                return PartialView("_TableBusiness");
            }
        }


        // thêm tài xế
        [UserAuthenticationFilter]
        [System.Web.Http.HttpPost]
        public JsonResult CreateShipper([FromBody] ShiperInputModel input)
        {
            return Json(shipperBusiness.CreateShipper(input), JsonRequestBehavior.AllowGet);
        }

        // Xóa Shipper 
        [UserAuthenticationFilter]
        public int DeleteShipper(int ID)
        {
            try
            {
                return shipperBusiness.DeleteShip(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        /// <summary>
        /// Ngưng hoạt động shipper
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpPut]
        public JsonResult DeActiveShiper(int id)
        {
            return Json(shipperBusiness.DeActiveShiper(id), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Tìm kiếm shipper theo keyword(Thuộc bộ chức năng cộng tiền cho shipper)
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public JsonResult GetListShipperByKeyword(string keyword)
        {
            return Json(shipperBusiness.GetListShipperByKeyword(keyword), JsonRequestBehavior.AllowGet);
        }
    }
}