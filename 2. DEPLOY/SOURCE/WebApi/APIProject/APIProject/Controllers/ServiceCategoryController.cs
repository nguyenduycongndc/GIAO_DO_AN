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
    public class ServiceCategoryController : BaseController
    {
        // GET: ProductCategory
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult Create(string Name, int Order, string img, string code)
        {
            var data = serviceCategoryBusiness.CreateServiceCategory(Name, Order, img, code);
            return base.Json(data, JsonRequestBehavior.AllowGet);
        }
        //Tìm kiếm
        public PartialViewResult SearchServiceCategory(int page, int? IsActive, string Name = "", string FromDate = "", string ToDate = "")
        {
            try
            {
                var listcate = serviceCategoryBusiness.SearchServiceCategory(page, Name, FromDate, ToDate, IsActive);
                ViewBag.name = Name;
                ViewBag.isActive = IsActive;
                ViewBag.fromDate = FromDate;
                ViewBag.toDate = ToDate;
                return PartialView("_TableServiceCategory", listcate);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableServiceCategory");
            }


        }
        //Lưu khi sửa ServiceCategory
        public PartialViewResult SaveServiceCategory(ListServiceCategoryModel category)
        {
            try
            {
                var query = serviceCategoryBusiness.EditCategory(category);
                return PartialView("Index", query);
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        //Binding dữ liệu lên modal sửa
        public JsonResult GetCategoryInfo(int id)
        {
            try
            {
                var cate = serviceCategoryBusiness.GetCategoryInfo(id);
                return base.Json(cate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Xóa
        public JsonResult DeleteServiceCategory(int id)
        {
            var data = serviceCategoryBusiness.DeleteServiceCategory(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //public PartialViewResult Search(int page, string name, string fromDate, string toDate)
        //{
        //    ViewBag.name = name;
        //    ViewBag.fromDate = fromDate;
        //    ViewBag.toDate = toDate;
        //    IPagedList<CategoryViewModel> list = cateBusiness.Search(name, fromDate, toDate).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
        //    return PartialView("_listServiceCategory", list);
        //}
        //[HttpPost]
        //public JsonResult AddCategory(string code, int displayOrder, string nameEN, string nameVN)
        //{
        //    var result = cateBusiness.AddCategory(code, displayOrder, nameEN, nameVN);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public JsonResult EditCategory(int id, int displayOrder, int status, string nameEN, string nameVN)
        //{
        //    var result = cateBusiness.EditCategory(id, displayOrder, status,nameEN,nameVN);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetById(int id)
        //{
        //    var model = cateBusiness.GetById(id);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public JsonResult DeleteCategory(int id)
        //{
        //    var result = cateBusiness.DeleteCategory(id);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
    }

}