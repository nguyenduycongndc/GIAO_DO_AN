using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class LoginBusiness : GenericBusiness
    {
        //TransactionHistoryBusiness transactionBus = new TransactionHistoryBusiness();
        //RequestAPIBusiness apiBus = new RequestAPIBusiness();
        //CustomerBusiness cusBus = new CustomerBusiness();
        //ShiperBusiness agentBus = new ShiperBusiness();
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        string hostUrl = Util.getFullUrl();
        public LoginBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public JsonResultModel Register(LoginAppInputModel input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.PassWord) || String.IsNullOrEmpty(input.Phone) || String.IsNullOrEmpty(input.name) || String.IsNullOrEmpty(input.email))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);

                if (!Util.validPhone(Util.ConvertPhone(input.Phone)))
                    return rpBus.ErrorResult(MessVN.INVALID_PHONE, SystemParam.PROCESS_ERROR);

                if (!Util.ValidateEmail(Util.ConvertPhone(input.email)))
                    return rpBus.ErrorResult(MessVN.EMAIL_ERROR, SystemParam.PROCESS_ERROR);

                int checkPhone = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.User.Equals(input.Phone) && c.CustomerID != null).Count();
                int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && (c.Customer.Email.Equals(input.email) || c.Shop.Email.Equals(input.email) || c.Shiper.Email.Equals(input.email))).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                int rankID = cnn.CustomerRanks.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Level.Equals(1)).Select(c => c.ID).FirstOrDefault();
                string token = Util.CreateMD5(DateTime.Now.ToString());
                Customer cus = new Customer();
                cus.Name = input.name;
                cus.Phone = input.Phone;
                cus.Email = input.email;
                cus.AvatarUrl = "/Uploads/avatar.jpg";
                cus.QTYCancel = 0;
                cus.IsActive = SystemParam.ACTIVE;
                cus.RankingPoint = 0;
                cus.CreatedDate = DateTime.Now;
                cus.CustomerRankID = rankID;

                //Lưu lại thông tin để dăng nhập
                string keyChat = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                string roomID = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                List<Member> mb = new List<Member>();
                Member m = new Member();
                m.User = input.Phone;
                m.DeviceID = input.deviceID;
                m.Password = Util.GenPass(input.PassWord);
                m.Token = token;
                m.IsActive = SystemParam.ACTIVE;
                m.CreatedDate = DateTime.Now;
                m.IsLogin = true;
                m.ExpriceDateOTP = DateTime.Now;
                m.Status = SystemParam.ACTIVE;
                m.QtyOTP = 0;
                m.OTPDateTime = DateTime.Now;
                m.KeyChat = keyChat;
                m.RoomID = roomID;
                mb.Add(m);
                cus.Members = mb;
                cnn.Customers.Add(cus);
                List<Wallet> wallet = new List<Wallet>();

                Wallet w = new Wallet
                {
                    Balance = 0,
                    IsActive = SystemParam.ACTIVE,
                    Type = 1,
                    CreatedDate = DateTime.Now
                };
                wallet.Add(w);
                m.Wallets = wallet;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, GetUserInfo(m.ID));
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();

            }
        }       
        public JsonResultModel RegisterFake(LoginAppInputModel input)
        {
            try
            {
                int checkPhone = cnn.Members.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.User.Equals(input.Phone)).Count();
                int checkMail = cnn.Members.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && (c.Customer.Email.Equals(input.email) || c.Shop.Email.Equals(input.email) || c.Shiper.Email.Equals(input.email))).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                DateTime? dob = DateTime.Now;
                Shiper shiper = new Shiper();
                shiper.Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(0, 10);
                shiper.AvartarUrl = "/Uploads/5618032021avatar.jpg";
                shiper.Name = input.name;
                shiper.DOB = dob;
                shiper.Identification = "023920931239";
                shiper.Phone = input.Phone;
                shiper.Email = input.email;
                shiper.Address = "Five Star Kim Giang";
                shiper.CommissionID = 1;
                shiper.CreatedDate = DateTime.Now;
                shiper.RatingAdmin = 0;
                shiper.IsActive = SystemParam.ACTIVE;
                shiper.Lati = 20.9858584;
                shiper.Longi = 105.810133;
                shiper.IsAcceptService = true;
                shiper.ModifyDate = DateTime.Now;
                shiper.Rating = 0;
                shiper.Sex = 1;
                shiper.IsInternal = 0;
                //add car for shipper
                List<CarShiper> lst = new List<CarShiper>();
                CarShiper cs = new CarShiper();
                cs.CarBrand = "Lexus";
                cs.CarModel = "Wave";
                cs.LicensePlates = "P30-12398";
                cs.IsActive = SystemParam.ACTIVE;
                cs.CreatedDate = DateTime.Now;
                cs.VehicleTypeID = 1;
                cs.CarColor = "";
                lst.Add(cs);
                shiper.CarShipers = lst;
                List<Member> mb = new List<Member>();
                Member m = new Member();
                m.User = input.Phone;
                m.DeviceID = input.deviceID;
                m.Password = Util.GenPass(input.PassWord);
                m.Token = "";
                m.IsActive = SystemParam.ACTIVE;
                m.CreatedDate = DateTime.Now;
                m.IsLogin = true;
                m.ExpriceDateOTP = DateTime.Now;
                m.Status = SystemParam.ACTIVE;
                m.QtyOTP = 0;
                m.OTPDateTime = DateTime.Now;
                //Create wallet for shiper
                List<Wallet> w = new List<Wallet>();
                w.Add(new Wallet() { Balance = 0, Type = Constant.WALLET_NO_WITHDRAW, CreatedDate = DateTime.Now, IsActive = SystemParam.ACTIVE });
                w.Add(new Wallet() { Balance = 0, Type = Constant.WALLET_WITHDRAW, CreatedDate = DateTime.Now, IsActive = SystemParam.ACTIVE });
                m.Wallets = w; //add wallet to member
                //Add area for shipper
                List<ShiperArea> lsa = new List<ShiperArea>();
                m.KeyChat = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                m.RoomID = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                mb.Add(m);
                shiper.Members = mb;
                cnn.Shipers.Add(shiper);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        /// <summary>
        /// đăng nhập vào app
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public JsonResultModel CheckLoginApp(LoginAppInputModel input)
        {
            try
            {
                if (!Util.validPhone(Util.ConvertPhone(input.Phone)))
                    return rpBus.ErrorResult(MessVN.INVALID_PHONE, SystemParam.PROCESS_ERROR);
                //Lấy user với phone
                Member m = cnn.Members.Where(mbe => mbe.IsActive > SystemParam.ACTIVE_FALSE && mbe.User.Equals(input.Phone) && (!input.role.Equals(Constant.CUSTOMER_ROLE) ? !mbe.CustomerID.HasValue : mbe.CustomerID.HasValue)).FirstOrDefault();
                if (m == null)
                    return rpBus.ErrorResult(MessVN.LOGIN_ERROR, SystemParam.PROCESS_ERROR);
                if (m.IsActive.Equals(SystemParam.DEACTIVE) || m.Status.Equals(SystemParam.DEACTIVE))
                    return rpBus.ErrorResult("Tài khoản của bạn tạm thời bị khóa, vui lòng liên hệ lại với admin!", SystemParam.PROCESS_ERROR);

                //Trường hợp đăng nhập lại bằng mật khẩu cũ
                if (Util.CheckPass(input.PassWord, m.Password) && String.IsNullOrEmpty(m.OldPassword))
                {
                    string token = Util.CreateMD5(DateTime.Now.ToString());
                    m.Token = token;
                    m.DeviceID = input.deviceID;
                    m.IsLogin = true;
                    m.OldPassword = "";
                    cnn.SaveChanges();
                    return rpBus.SuccessResult(MessVN.SUCCESS_STR, GetUserInfo(m.ID));
                }
                //Trường hợp đăng nhập bằng mật khẩu mới
                if (Util.CheckPass(input.PassWord, m.OldPassword))
                {
                    string token = Util.CreateMD5(DateTime.Now.ToString());
                    m.Token = token;
                    m.DeviceID = input.deviceID;
                    m.IsLogin = true;
                    m.Password = m.OldPassword;
                    m.OldPassword = "";
                    cnn.SaveChanges();
                    return rpBus.SuccessResult(MessVN.SUCCESS_STR, GetUserInfo(m.ID));
                }


                return rpBus.ErrorResult(MessVN.LOGIN_ERROR, SystemParam.PROCESS_ERROR);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return null;
            }
        }

        /// <summary>
        /// Cập nhật thông tin member
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>

        public JsonResultModel UpdateMemberInfo(CustomerDetailOutputModel input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.Name) || String.IsNullOrEmpty(input.Phone) || String.IsNullOrEmpty(input.Email))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                if (!Util.validPhone(Util.ConvertPhone(input.Phone)))
                    return rpBus.ErrorResult(MessVN.INVALID_PHONE, SystemParam.PROCESS_ERROR);

                if (!Util.ValidateEmail(Util.ConvertPhone(input.Email)))
                    return rpBus.ErrorResult(MessVN.EMAIL_ERROR, SystemParam.PROCESS_ERROR);

                Member mb = cnn.Members.Find(input.MemberID);
                

                //Trường hợp là khách hàng
                if (mb.CustomerID.HasValue)
                {
                    int checkPhone = cnn.Members.Where(m => m.IsActive > SystemParam.ACTIVE_FALSE && m.CustomerID.HasValue && m.User.Equals(input.Phone) && !m.ID.Equals(mb.ID)).Count();
                    int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.CustomerID.HasValue && (c.Customer.Email.Equals(input.Email) || c.Shiper.Email.Equals(input.Email) || c.Shop.Email.Equals(input.Email)) && !c.ID.Equals(input.MemberID)).Count();
                    if (checkPhone > 0)
                        return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                    if (checkMail > 0)
                        return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);

                    mb.User = input.Phone;
                    mb.Customer.Phone = input.Phone;
                    mb.Customer.Name = input.Name;
                    mb.Customer.Email = input.Email;
                    mb.Customer.DOB = !String.IsNullOrEmpty(input.dobStr) ? Util.ConvertDate(input.dobStr) : mb.Customer.DOB;
                    mb.Customer.Sex = input.Sex;
                    mb.Customer.Address = input.Address;
                }
                //Trường hợp là shiper
                if (mb.ShiperID.HasValue)
                {
                    int checkPhone = cnn.Members.Where(m => m.IsActive > SystemParam.ACTIVE_FALSE && m.ShiperID.HasValue && m.User.Equals(input.Phone) && !m.ID.Equals(mb.ID)).Count();
                    int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.ShiperID.HasValue && (c.Customer.Email.Equals(input.Email) || c.Shiper.Email.Equals(input.Email) || c.Shop.Email.Equals(input.Email)) && !c.ID.Equals(input.MemberID)).Count();
                    if (checkPhone > 0)
                        return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                    if (checkMail > 0)
                        return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                    mb.Shiper.Name = input.Name;
                    mb.User = input.Phone;
                    mb.Shiper.Phone = input.Phone;
                    mb.Shiper.Email = input.Email;
                    mb.Shiper.Address = input.Address;
                    mb.Shiper.DOB = !String.IsNullOrEmpty(input.dobStr) ? Util.ConvertDate(input.dobStr) : mb.Customer.DOB;
                    mb.Shiper.Sex = input.Sex.HasValue ? input.Sex.Value : SystemParam.MALE;
                }

                //Trường hợp là shop 
                if (mb.ShopID.HasValue)
                {
                    int checkPhone = cnn.Members.Where(m => m.IsActive > SystemParam.ACTIVE_FALSE && m.ShopID.HasValue && m.User.Equals(input.Phone) && !m.ID.Equals(mb.ID)).Count();
                    int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.ShopID.HasValue && (c.Customer.Email.Equals(input.Email) || c.Shiper.Email.Equals(input.Email) || c.Shop.Email.Equals(input.Email)) && !c.ID.Equals(input.MemberID)).Count();
                    if (checkPhone > 0)
                        return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                    if (checkMail > 0)
                        return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                    mb.User = input.Phone;
                    mb.Shop.Phone = input.Phone;
                    mb.Shop.Name = input.Name;
                    mb.Shop.Lati = input.lati;
                    mb.Shop.Logi = input.longi;
                    mb.Shop.Email = input.Email;
                    mb.Shop.Address = input.Address;
                }
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, GetUserInfo(mb.ID));
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        //Gửi mã xác nhận đổi mật khẩu
        public JsonResultModel FogotPassword(string email)
        {
            try
            {
                EmailBusiness em = new EmailBusiness();
                Member mb = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && (m.Customer.Email.Equals(email) || m.Shiper.Email.Equals(email) || m.Shop.Email.Equals(email))).FirstOrDefault();
                if (mb == null)
                    return rpBus.ErrorResult(MessVN.EMAIL_ERROR, SystemParam.PROCESS_ERROR);
                Random r = new Random();
                string code = r.Next(1000, 9999).ToString();
                mb.ConfirmCode = code;
                mb.ExpriceDateOTP = DateTime.Now.AddMinutes(1);
                cnn.SaveChanges();
                em.configClient(mb.CustomerID.HasValue ? mb.Customer.Email : (mb.ShiperID.HasValue ? mb.Shiper.Email : mb.Shop.Email), "Mã xác thực đặt lại mật khẩu", "Mã xác thực đặt lại mật khẩu của bạn là: " + code);
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel CheckOTP(string email, string code)
        {
            try
            {
                Member mb = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && (m.Customer.Email.Equals(email) || m.Shiper.Email.Equals(email) || m.Shiper.Email.Equals(email))
                && m.ConfirmCode.Equals(code) && m.ExpriceDateOTP >= DateTime.Now).FirstOrDefault();
                if (mb == null)
                    return rpBus.ErrorResult("Mã xác nhận không hợp lệ", SystemParam.PROCESS_ERROR);
                mb.OldPassword = "Is_Check_OK";
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel ResetPassword(string email, string pass)
        {
            try
            {
                Member mb = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && (m.Customer.Email.Equals(email) || m.Shiper.Email.Equals(email) || m.Shiper.Email.Equals(email)) && m.OldPassword.Equals("Is_Check_OK")).FirstOrDefault();
                if (mb == null)
                    return rpBus.ErrorResult("Xác thực mã chưa thành công", SystemParam.PROCESS_ERROR);
                mb.OldPassword = Util.GenPass(pass);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public JsonResultModel LogOutApp(int memberID)
        {
            try
            {
                Member m = cnn.Members.Find(memberID);
                m.Token = "";
                m.DeviceID = "";
                m.IsLogin = false;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        //Cập nhật deviceID
        public JsonResultModel UpdateDeviceID(LoginAppInputModel input)
        {
            try
            {
                Member m = cnn.Members.Find(input.UserID);
                m.DeviceID = input.deviceID;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.ERROR);
            }
        }
        public int? checkToken(string token)
        {
            var mb = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && m.Token.Equals(token)).FirstOrDefault();
            int? result = null;
            if (mb != null)
                result = mb.ID;
            return result;
        }
        public Member checkShopToken(string token)
        {
            var mb = cnn.Members.Where(m => m.IsActive.Equals(SystemParam.ACTIVE) && m.Token.Equals(token)).FirstOrDefault();
            return mb;
        }
        /// <summary>
        /// đăng nhập vào app
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public JsonResultModel ChangePass(UpdatePassModel input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.newPass) || String.IsNullOrEmpty(input.oldPass))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                if (input.newPass == input.oldPass)
                    return rpBus.ErrorResult(MessVN.INVALID_NEW_PASS, SystemParam.PROCESS_ERROR);
                Member m = cnn.Members.Find(input.UserID);
                if (Util.CheckPass(input.oldPass, m.Password))
                {
                    m.Password = Util.GenPass(input.newPass);
                    cnn.SaveChanges();
                    return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
                }

                return rpBus.ErrorResult(MessVN.INCORRECT_OLD_PASS, SystemParam.PROCESS_ERROR);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public bool ConFirmCode(string code, int cusID)
        {
            Member cus = cnn.Members.Find(cusID);
            if (cus.ConfirmCode.Equals(code))
                return true;
            else
                return false;
        }
        /// <summary>
        /// Đăng nhập cho web
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public JsonResultModel CheckLoginWeb(string phone, string password)
        {
            try
            {
                UserDetailOutputModel query = new UserDetailOutputModel();
                User us = cnn.Users.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && (u.UserPhone.Equals(phone) || u.UserName.Equals(phone))).FirstOrDefault();
                if (us == null)
                    return rpBus.response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.ERROR_MESSAGE_LOGIN_FAIL, null);

                if (Util.CheckPass(password, us.Password))
                {
                    UserDetailOutputModel data = new UserDetailOutputModel();
                    data.UserName = us.UserName;
                    data.Role = us.Role;
                    data.UserID = us.ID;
                    data.Phone = us.UserPhone;
                    data.Email = us.Email;
                    HttpContext.Current.Session[SystemParam.SESSION_LOGIN] = data;
                    return rpBus.response(SystemParam.SUCCESS, SystemParam.SUCCESS_CODE, SystemParam.SUCCES_STR, null);
                }
                return rpBus.response(SystemParam.ERROR, SystemParam.PROCESS_ERROR, SystemParam.ERROR_MESSAGE_LOGIN_FAIL, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }

        }

        public UserInforOutputModel GetUserInfo(int id)
        {
            try
            {
                UserInforOutputModel data = new UserInforOutputModel();
                Member mb = cnn.Members.Find(id);

                //Trường hợp login là shiper
                if (mb.ShiperID.HasValue)
                {
                    CarShiper car = mb.Shiper.CarShipers.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.ShiperID.Equals(mb.ShiperID.Value)).FirstOrDefault();
                    data.MemberID = mb.ID;
                    data.UserID = mb.ShiperID.Value;
                    data.Avatar = hostUrl + mb.Shiper.AvartarUrl;
                    data.Code = mb.Shiper.Code;
                    data.Name = mb.Shiper.Name;
                    data.Phone = mb.Shiper.Phone;
                    data.IsInternal = mb.Shiper.IsInternal;
                    data.Rate = mb.Shiper.Rating;
                    data.DOB = mb.Shiper.DOB;
                    data.Email = mb.Shiper.Email;
                    data.lati = mb.Shiper.Lati;
                    data.longi = mb.Shiper.Longi;
                    data.Token = mb.Token;
                    data.KeyChat = mb.KeyChat;
                    data.IsVip = mb.Shiper.IsVip.GetValueOrDefault();
                    data.BalanceWalletWithDraw = mb.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.MemberID.Equals(mb.ID) && w.Type.Equals(Constant.WALLET_WITHDRAW)).FirstOrDefault().Balance;
                    data.BalanceWalletNoWithDraw = mb.Wallets.Where(w => w.IsActive.Equals(SystemParam.ACTIVE) && w.MemberID.Equals(mb.ID) && w.Type.Equals(Constant.WALLET_NO_WITHDRAW)).FirstOrDefault().Balance;
                    data.RoomID = mb.RoomID;
                    data.Role = Constant.SHIPER_ROLE;
                    data.Sex = mb.Shiper.Sex;
                    data.Address = mb.Shiper.Address;
                    data.IsAcceptService = mb.Shiper.IsAcceptService.Equals(true) ? 1 : 0;
                    data.listLocation = mb.Shiper.ShiperAreas.Select(a => new CustomerLocationModel
                    {
                        ID = a.ID,
                        Name = a.Area.Name
                    }).ToList();
                    if (car != null)
                    {
                        data.CarColor = car.CarColor;
                        data.CarBrand = car.CarBrand;
                        data.CarModel = car.CarModel;
                        data.LicensePlates = car.LicensePlates;
                    }
                    data.ListBank = mb.BankMembers.Select(b => new BankDetailModel
                    {
                        BankIDShiper = b.ID,
                        BankName = b.Bank.Name,
                        Account = b.Account,
                        AcountOwner = b.AccountOwner,
                        BankID = b.ID,
                        Code = b.Bank.Code
                    }).ToList();
                }

                //Trường hơp là khách hàng đăng nhập

                if (mb.CustomerID.HasValue)
                {
                    CustomerRank cus = cnn.CustomerRanks.Find(mb.Customer.CustomerRankID);
                    data.MemberID = mb.ID;
                    data.UserID = mb.CustomerID.Value;
                    data.Name = mb.Customer.Name;
                    data.Avatar = hostUrl + mb.Customer.AvatarUrl;
                    data.DOB = mb.Customer.DOB;
                    data.Phone = mb.Customer.Phone;
                    data.Email = mb.Customer.Email;
                    data.Email = mb.Customer.Email;
                    data.Address = mb.Customer.Address;
                    data.lati = mb.Customer.Lati.HasValue ? mb.Customer.Lati.Value : 0;
                    data.longi = mb.Customer.Longi.HasValue ? mb.Customer.Longi.Value : 0;
                    data.Token = mb.Token;
                    data.KeyChat = mb.KeyChat;
                    data.RoomID = mb.RoomID;
                    data.IsVip = mb.Customer.IsVip.GetValueOrDefault();
                    data.VipDiscount = mb.Customer.VipDiscount.GetValueOrDefault();
                    data.Role = Constant.CUSTOMER_ROLE;
                    data.RankLevel = cus.Level;
                    data.RankName = cus.Description;
                    data.RankPoint = mb.Customer.RankingPoint;
                    data.Sex = mb.Customer.Sex;
                    data.UsePoint = cnn.Wallets.Where(x => x.MemberID == mb.ID).Select(x => x.Balance).FirstOrDefault();
                }

                //Trường hợp shop đăng nhập
                if (mb.ShopID.HasValue)
                {
                    data.MemberID = mb.ID;
                    data.UserID = mb.ShopID.Value;
                    data.Phone = mb.Shop.Phone;
                    data.Avatar = hostUrl + mb.Shop.ShopImages.Where(s => s.Type.Equals(SystemParam.TYPE_SHOP_IMAGE)).Select(s => s.Path).FirstOrDefault();
                    data.Name = mb.Shop.Name;
                    data.lati = mb.Shop.Lati;
                    data.longi = mb.Shop.Logi;
                    data.Token = mb.Token;
                    data.Rate = mb.Shop.Rate;
                    data.KeyChat = mb.KeyChat;
                    data.Email = mb.Shop.Email;
                    data.RoomID = mb.RoomID;
                    data.Address = mb.Shop.Address;
                    data.Role = Constant.SHOP_ROLE;
                    data.Status = mb.Status;
                }
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new UserInforOutputModel();
            }
        }

        //Lấy dữ liệu màn hình home
        public JsonResultModel GetHomeScreen()
        {
            NewsBusiness news = new NewsBusiness();
            try
            {
                HomeScreenOutPutModel data = new HomeScreenOutPutModel();
                data.listBaner = news.GetNewsHomeScreen(SystemParam.NEWS_TYPE_BANER_HOME);
                data.listPromotion = news.GetNewsHomeScreen(SystemParam.NEWS_TYPE_PROMOTION);
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }


        //Thay ảnh đại diện
        public JsonResultModel ChangeAvatar(int memeberID)
        {
            try
            {
                Member m = cnn.Members.Find(memeberID);
                string result = Util.UploadFile("image").FirstOrDefault();
                //Trường hợp là khách hàng
                if (m.CustomerID.HasValue)
                {
                    m.Customer.AvatarUrl = result;
                }
                //trường hợp là shiper
                if (m.ShiperID.HasValue)
                {
                    m.Shiper.AvartarUrl = result;
                }

                //trường hợp là shop
                if (m.ShopID.HasValue)
                {
                    ShopImage si = m.Shop.ShopImages.Where(s => s.ShopID.Equals(m.ShopID.Value) && s.Type.Equals(SystemParam.TYPE_IMAGE)).FirstOrDefault();
                    si.Path = result;
                }
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, hostUrl + result);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
    }
}
