using APIProject.App_Start;
using APIProject.Models;
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
    [UserAuthenticationFilter(2, 0)]
    public class NewsController : BaseController
    {
        // GET: News
        public ActionResult Index()
        {
            return View();
        }
 

        [UserAuthenticationFilter]

        public ActionResult CreateNews()
        {
            // ViewBag.itemNewsProduct = newsBusiness.getItemNewsCate(SystemParam.EN);
             //ViewBag.order = newsBusiness.GetOrderDisplay();
            return View();
        }
        //Thêm mới
        public JsonResult CreateNewWeb(ListNewsWebOutputModel create_new)
        {
            var userId = ((UserDetailOutputModel)Session[SystemParam.SESSION_LOGIN]).UserID;

            //return newsBusiness.CreateNew(create_new, userId);
            return Json(newsBusiness.CreateNew(create_new, userId), JsonRequestBehavior.AllowGet);
        }
        //thêm nháp
        public JsonResult DraffNewWeb(ListNewsWebOutputModel create_new)
        {
            var userId = ((UserDetailOutputModel)Session[SystemParam.SESSION_LOGIN]).UserID;

            //return newsBusiness.DraffNew(create_new, userId);
            return Json(newsBusiness.DraffNew(create_new, userId), JsonRequestBehavior.AllowGet);
        }
        //Tìm kiếm
        public PartialViewResult SearchView(int page, string title, int? Status)
        {
            //ViewBag.Name = Name;
            //ViewBag.IsActive = IsActive;
            //ViewBag.FromDate = FromDate;
            //ViewBag.ToDate = ToDate;
            
            try
            {
                var listcate = newsBusiness.SearchView(page, title, Status);
                ViewBag.title = title;
                ViewBag.Status = Status;
                return PartialView("_TableNews", listcate);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableNews");
            }


        }
        //binding dữ liệu lên form sửa
        [UserAuthenticationFilter]
        public ActionResult UpdateNews(int id)
        {
            ListNewsWebOutputModel data = newsBusiness.ViewNewsDetail(id);
            return View(data);
        }
        //lưu khi sửa
        public JsonResult SaveUpdateNew(ListNewsWebOutputModel category)
        {
            try
            {
                var query = newsBusiness.SaveEditNew(category);
                //return PartialView("Index", query);
                return Json(query);
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        //Xóa
        public JsonResult DeleteNews(int id)
        {
            var data = newsBusiness.DeleteNew(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //public PartialViewResult Search(int page, string Title = "", int? cateID = null, string FromDate = "", string ToDate = "")
        //{
        //    ViewBag.page = page;
        //    ViewBag.Title = Title;
        //    ViewBag.cateID = cateID;
        //    ViewBag.FromDate = FromDate;
        //    ViewBag.ToDate = ToDate;
        //    var data = newsBusiness.Search(Title, cateID, FromDate, ToDate).Result;
        //    IPagedList<ListNewsWebOutputModel> listdata = ((List<ListNewsWebOutputModel>)data).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
        //    return PartialView("_TableNews", listdata);

        //}


        //[UserAuthenticationFilter()]
        //public ActionResult Index()
        //{

        //    ViewBag.page = 1;
        //    ViewBag.Title = "";
        //    ViewBag.Type = null;
        //    ViewBag.FromDate = "";
        //    ViewBag.ToDate = "";
        //    ViewBag.itemNewsProduct = newsBusiness.getItemNewsCate(SystemParam.EN);
        //    return View();
        //}

        //[ValidateInput(false)]
        //public JsonResult AddNews(string Title, string Content, string Description, string UrlImage,int Type,int TypeSend, int OrderDisplay,int IsBanner,int Status,int CategoryID, int IsShowCreate,string Link)
        //{
        //    ListNewsWebOutputModel news = new ListNewsWebOutputModel();
        //    news.CategoryID = CategoryID;
        //    news.Title = Title;
        //    news.Name = Title;
        //    news.UserID = ((UserDetailOutputModel)Session[Sessions.LOGIN]).UserID;
        //    news.Description = Description;
        //    news.Content = Content;
        //    news.ShowCreate = IsShowCreate;
        //    news.Type = Type;
        //    news.Status = Status;
        //    news.TypeSend =TypeSend;
        //    news.UrlImage = UrlImage;
        //    news.OrderDisplay =OrderDisplay;
        //    news.IsBanner = IsBanner;
        //    news.Link = Link;
        //    var data = newsBusiness.CreateNewsDekko(news);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        //[ValidateInput(false)]
        //public JsonResult UpdateNewsDetail(int id, string Title, string Content, string Description, string UrlImage, int Type, int TypeSend, int OrderDisplay, int IsBanner, int Status, int CategoryID, int IsShowCreate,string Link)
        //{
        //    ListNewsWebOutputModel news = new ListNewsWebOutputModel();
        //    news.ID = id;
        //    news.CategoryID = CategoryID;
        //    news.Title = Title;
        //    news.Name = Title;
        //    news.UserID = ((UserDetailOutputModel)Session[Sessions.LOGIN]).UserID;
        //    news.Description = Description;
        //    news.Content = Content;
        //    news.Type = Type;
        //    news.ShowCreate = IsShowCreate;
        //    news.Status = Status;
        //    news.TypeSend = TypeSend;
        //    news.UrlImage = UrlImage;
        //    news.OrderDisplay = OrderDisplay;
        //    news.IsBanner = IsBanner;
        //    news.Link = Link;
        //    var data = newsBusiness.UpdateNewsDekko(news);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult DeleteNews(int id)
        //{
        //    var data = newsBusiness.DeleteNews(id);
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
    }
}