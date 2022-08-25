using APIProject.App_Start;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class NotificationController : BaseController
    {
        // GET: Notification
        [UserAuthenticationFilter(2, 0)]
        public ActionResult Index()
        {
            return View();
        }

        [System.Web.Http.HttpPost]
        public JsonResult CreateNoti([FromBody] NotifyInputModel input)
        {
            return Json(notifyBusiness.PushNotiToMember(input), JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult SearchNoti(int page, string searchKey = null, int? type = null, string toDate = null, string fromDate = null)
        {
            return PartialView("_listNotification", notifyBusiness.Search(page, searchKey, type, fromDate, toDate));
        }

    }

}