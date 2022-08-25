using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class UserBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public UserBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }

        public IPagedList<UserDetailOutputModel> Search(int Page, string Key = "")
        {
            try
            {
                var query = (from u in cnn.Users
                             where u.IsActive.Equals(SystemParam.ACTIVE)
                             && (!String.IsNullOrEmpty(Key) ? (u.UserPhone.Contains(Key) || u.UserName.Contains(Key)) : true)
                             orderby u.ID descending
                             select new UserDetailOutputModel
                             {
                                 IsActive = u.IsActive,
                                 UserID = u.ID,
                                 Role = u.Role,
                                 UserName = u.UserName,
                                 Phone = u.UserPhone,
                                 CreateDate = u.CreatedDate,
                                 Email = u.Email,
                             }).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);

                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<UserDetailOutputModel>().ToPagedList(1, 1);
            }
        }



        // thêm tài khoản
        public JsonResultModel CreateUser(string Name, string Email, string Phone, string Password, int typeUser)
        {
            try
            {
                if (String.IsNullOrEmpty(Name) || String.IsNullOrEmpty(Email) || String.IsNullOrEmpty(Phone) || String.IsNullOrEmpty(Password))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                List<User> user = cnn.Users.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                int checkMail = user.Where(u => u.Email == Email).Count();
                int checkPhone = user.Where(u => u.UserPhone == Phone).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                User us = new User();
                us.UserName = Name;
                us.Email = Email;
                us.UserPhone = Phone;
                us.Password = Util.GenPass(Password);
                us.CreatedDate = DateTime.Now;
                us.Role = typeUser;
                us.IsActive = SystemParam.ACTIVE;
                cnn.Users.Add(us);
                cnn.SaveChanges();
                return rpBus.SuccessResult(SystemParam.SUCCES_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }


        public JsonResultModel UpdateUserInfo(int id, string name, string email, string phone, string password, int role)
        {
            try
            {
                if (String.IsNullOrEmpty(name) || String.IsNullOrEmpty(email) || String.IsNullOrEmpty(phone))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                List<User> user = cnn.Users.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && !u.ID.Equals(id)).ToList();
                int checkMail = user.Where(u => u.Email == email).Count();
                int checkPhone = user.Where(u => u.UserPhone == phone).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                User us = cnn.Users.Find(id);
                us.UserName = name;
                us.UserPhone = phone;
                us.Email = email;
                us.Password = !String.IsNullOrEmpty(password) ? Util.GenPass(password) : us.Password;
                us.Role = role;
                cnn.SaveChanges();
                return rpBus.SuccessResult(SystemParam.SUCCES_STR, null);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }


        }

        public JsonResultModel ForgotPassword(string email)
        {
            try
            {
                EmailBusiness em = new EmailBusiness();
                User us = cnn.Users.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Email.Equals(email)).FirstOrDefault();
                if (us == null)
                    return rpBus.ErrorResult(MessVN.EMAIL_ERROR, SystemParam.PROCESS_ERROR);
                Random rd = new Random();
                string newPass = rd.Next(100000, 999999).ToString();
                us.Password = Util.GenPass(newPass);
                cnn.SaveChanges();
                em.configClient(email, "[HỆ THỐNG WESHIP]", "Mật khẩu mới của bạn là " + newPass);
                return rpBus.SuccessResult(SystemParam.SUCCES_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel ChangePassword(int ID, string CurrentPassword, string NewPassword)
        {
            try
            {
                User u = cnn.Users.Find(ID);
                if (Util.CheckPass(CurrentPassword, u.Password))
                {
                    u.Password = Util.GenPass(NewPassword);
                    cnn.SaveChanges();
                    return rpBus.SuccessResult(SystemParam.SUCCES_STR, null);
                }
                return rpBus.ErrorResult(SystemParam.ERROR_MESSAGE_CHECK_PASS_FAIL, SystemParam.PROCESS_ERROR);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }


        public JsonResultModel DeleteUser(int ID)
        {
            try
            {
                User u = cnn.Users.Find(ID);
                u.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return rpBus.SuccessResult(SystemParam.SUCCES_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public UserDetailOutputModel GetUserDetail(int userID)
        {
            try
            {
                UserDetailOutputModel userDetail = new UserDetailOutputModel();

                var query = (from u in cnn.Users
                             where u.IsActive.Equals(SystemParam.ACTIVE) && u.ID.Equals(userID)
                             select new UserDetailOutputModel
                             {
                                 UserID = u.ID,
                                 Role = u.Role,
                                 UserName = u.UserName,
                                 Phone = u.UserPhone,
                                 Email = u.Email,
                                 IsActive = u.IsActive,
                                 CreateDate = u.CreatedDate,
                             }).FirstOrDefault();
                if (query != null && query.UserID > 0)
                {
                    return userDetail = query;
                }
                return userDetail;
            }
            catch
            {
                return new UserDetailOutputModel();
            }
        }

    }
}
