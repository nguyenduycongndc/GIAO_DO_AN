using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class ComplainController : BaseController
    {
        // GET: Complain
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            return View();
        }
        [UserAuthenticationFilter]
        public ActionResult ComplainDetail(int ID)
        {
            return View(orderServiceBus.GetComplainDetail(ID));
        }
        public PartialViewResult Search(int page, int? type, string searchKey = null)
        {
            return PartialView("_ComplainTable", orderServiceBus.SearchComplain(page, searchKey, type));
        }
        public JsonResult del(int ID)
        {
             return Json( orderServiceBus.DelComplain(ID), JsonRequestBehavior.AllowGet);
        }
    }
}