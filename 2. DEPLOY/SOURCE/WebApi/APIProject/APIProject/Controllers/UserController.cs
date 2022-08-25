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
    [UserAuthenticationFilter(2, 3)]
    public class UserController : BaseController
    {
        
        // GET: User
        [UserAuthenticationFilter]
        public ActionResult Index()
        {
            UserDetailOutputModel userLogin = UserLogins;
            ViewBag.roleLogin = UserLogins.Role;
            if (UserLogins.Role != SystemParam.ROLE_ADMIN)
            {
                Session[SystemParam.SESSION_LOGIN] = null;
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [UserAuthenticationFilter()]
        public PartialViewResult Search(int Page, string Key)
        {

            ViewBag.page = Page;
            ViewBag.Key = Key;
            return PartialView("_TableUser", userBusiness.Search(Page, Key));
        }

        // Thêm Tài khoản User
        [UserAuthenticationFilter]
        [HttpPost]
        public JsonResult CreateUser(string Name, string Email, string Phone, string Password, int typeUser)
        {
            return Json(userBusiness.CreateUser(Name, Email, Phone, Password, typeUser), JsonRequestBehavior.AllowGet);
        }

        //Quên mật khẩu tài khoản admin
        public JsonResult ForgotPassword(string email)
        {
            return Json(userBusiness.ForgotPassword(email), JsonRequestBehavior.AllowGet);
        }

        //Đổi mật khẩu tài khoản admin
        public JsonResult ChangePassword(string CurrentPass, string newPass)
        {
            return Json(userBusiness.ChangePassword(UserLogins.UserID, CurrentPass, newPass));
        }
        //Sửa thông tin tài khoản
        [HttpPost]
        public JsonResult UpdateUserInfo(int id, string name, string email, string phone, string password, int role)
        {
            return Json(userBusiness.UpdateUserInfo(id, name, email, phone, password, role), JsonRequestBehavior.AllowGet);
        }
        //Xóa user
        [HttpPost]
        public JsonResult DeleteUser(int id)
        {
            return Json(userBusiness.DeleteUser(id), JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult GetUserDetail(int ID)
        {
            try
            {
                UserDetailOutputModel userDetail = userBusiness.GetUserDetail(ID);
                return PartialView("_UserDetail", userDetail);
            }
            catch
            {
                return PartialView("_UserDetail", new UserDetailOutputModel());
            }
        }
    }
}