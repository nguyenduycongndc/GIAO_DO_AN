using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class WithdrawalRequestController : BaseController
    {
        // GET: WithdrawalRequest
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult Search(int Page = 1 , string Name = null, int? Status = null, string fromDate = null, string toDate = null)
        {
            try
            {
                var res = shipperBusiness.SearchWithDraw(Page,Name,Status,fromDate,toDate);
                return PartialView("_TableWithDraw", res);
            }
            catch
            {
                return PartialView("_TableWithDraw");
            }
        }
        //Chấp nhận yêu cầu rút tiền
        public JsonResult AcceptWithDraw(string lstIdShiper, int type)
        {
            var data = shipperBusiness.AcceptWithDraw(lstIdShiper,type);
            return Json(data, JsonRequestBehavior.AllowGet);
        }        
        //Từ chối yêu cầu rút tiền
        public JsonResult DenyWithDraw(string lstIdShiper, string reason)
        {
            var data = shipperBusiness.DenyWithDraw(lstIdShiper, reason);
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}