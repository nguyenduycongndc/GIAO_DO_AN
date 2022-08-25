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
using System.Threading.Tasks;

namespace APIProject.Controllers
{
    public class HomeController : BaseController
    {
        [UserAuthenticationFilter]
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.CountCustomers = await cusBusiness.countCustomer();
            ViewBag.CountShops = await shopBusiness.CountShop();
            ViewBag.CountShippers = await shipperBusiness.CountShipper();
            ViewBag.SumTotalPrice = Util.ConvertCurrency(await orderServiceBus.GetOrderSale());
            ViewBag.NewCustomerPercent = await cusBusiness.countNewCustomerPercent();
            ViewBag.NewShiperPercent = await shipperBusiness.countNewShiperPercent();
            ViewBag.NewShopPercent = await shopBusiness.countNewShopPercent();
            ViewBag.ListCustomerEveryMonth = JsonConvert.SerializeObject(await cusBusiness.getCustomerEveryMonth());
            ViewBag.ListShiperEveryMonth = JsonConvert.SerializeObject(await shipperBusiness.getShiperEveryMonth());
            ViewBag.ListShopEveryMonth = JsonConvert.SerializeObject(await shopBusiness.getShopEveryMonth());
            ViewBag.ListMonth = JsonConvert.SerializeObject(await cusBusiness.getListMonth());
            ViewBag.CusRankCount = JsonConvert.SerializeObject(await cusBusiness.getCustomerRank());
            ViewBag.OrderSaleEveryMonth = JsonConvert.SerializeObject(await orderServiceBus.OrderSaleEveryMonth());

            return View();
        }

        [UserAuthenticationFilter]
        public ActionResult Introduce()
        {
            ViewBag.Title = "Home Page";
           // OverViewModel revenue = (OverViewModel)statisticBus.GetOverView().Result;
            OverViewModel revenue = new OverViewModel();
            ViewBag.Washer = revenue.Washer;
            ViewBag.Transaction = revenue.Transaction;
            ViewBag.Request = revenue.Request;
            ViewBag.Customer = revenue.Customer;            
            return View();
        }
        public JsonResult GetChart(string fromDate, string toDate, int type)
        {
            // var model = statisticBus.ChartByCreateDate(fromDate, toDate, type);
            var model = new List<int>();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        //lưu lại thông tin đối tượng vừa đăng nhập 
        [UserAuthenticationFilter]
        public JsonResult GetUserLogin()
        {
            try
            {
                if (Session[SystemParam.SESSION_LOGIN] != null)
                {
                    UserDetailOutputModel userLogin = (UserDetailOutputModel)Session[SystemParam.SESSION_LOGIN];
                    return Json(userLogin, JsonRequestBehavior.AllowGet);
                }
                return Json(new UserDetailOutputModel(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new UserDetailOutputModel(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Login()
        {
            ViewBag.Title = "Login Page";
            return View();
        }

        //đăng nhập web
        public JsonResult UserLogin(string phone, string password)
        {
            return Json(loginBusiness.CheckLoginWeb(phone, password), JsonRequestBehavior.AllowGet);
        }

        //đăng xuất
        public int Logout()
        {
            try
            {
                Session[SystemParam.SESSION_LOGIN] = null;
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

    }
}
