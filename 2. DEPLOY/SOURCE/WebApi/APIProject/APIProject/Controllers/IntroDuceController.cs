using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.DB;
using Data.Business;
using Data.Model.APIWeb;
using Data.Utils;


namespace APIProject.Controllers
{
    public class IntroDuceController : BaseController
    {
        public WE_SHIPEntities cnn = new WE_SHIPEntities();
        // GET: IntroDuce
        public ActionResult HomeIntro()
        {
            ViewBag.Display_New = newsBusiness.Display_New();
            return View();
        }
        public ActionResult IntroDuce()
        {
            return View();
        }
        public ActionResult News()
        {
            ViewBag.Display_Promotion = newsBusiness.Display_Promotion();
            ViewBag.Display_New = newsBusiness.Display_New();
            ViewBag.Display_Advertisement = newsBusiness.Display_Advertisement();
            ViewBag.Display_BannerFood = newsBusiness.Display_BannerFood();
            return View();
            
        }

        public ActionResult NewsDetail(int ID)
        {
            ListNewsWebOutputModel data = newsBusiness.DisplayNewsDetail(ID);
            return View(data);
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult Recruitment()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult FeeShip()
        {
            return View();
        }
        public ActionResult DriverPartner()
        {
            return View();
        }

        public ActionResult RecruitmentDetail()
        {
            return View();
        }
        public ActionResult Order()
        {
            return View();
        }

        public ActionResult Complain()
        {
            return View();
        }

        public ActionResult ProhibitShipping()
        {
            return View();
        }

        public ActionResult CompensationPolicy()
        {
            return View();
        }

        public ActionResult TermsOfUse()
        {
            return View();
        }

        public ActionResult ShippingProcess()
        {
            return View();
        }

         

        public ActionResult WeOperate()
        {
            ViewBag.Province = from p in cnn.Provinces orderby p.Name select p;
            return View();
        }
    }
}