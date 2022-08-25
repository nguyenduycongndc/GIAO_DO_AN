using Data.Business;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Http;

namespace APIProject.Controllers
{

    public class ServiceController : BaseAPIController
    {
        ResponseBusiness rp = new ResponseBusiness();
        // API App


        /// <summary>
        /// 1.API login
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object Login([FromBody] LoginAppInputModel input)
        {
            return lgBus.CheckLoginApp(input);
        }

        [HttpPost]
        public object UpdatePassword([FromBody] UpdatePassModel input)
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.UserID = memberID.Value;
            return lgBus.ChangePass(input);
        }

        /// <summary>
        /// 4.Register
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object Register([FromBody] LoginAppInputModel input)
        {
            return lgBus.Register(input);
        }
        [HttpPost]
        public object RegisterFake([FromBody] LoginAppInputModel input)
        {
            return lgBus.RegisterFake(input);
        }
        [HttpGet]
        public object GetUserInfo()
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return rp.SuccessResult(MessVN.SUCCESS_STR, lgBus.GetUserInfo(memberID.Value));
        }


        /// <summary>
        /// Cập nhật thông tin member
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public object UpdateMemberInfo([FromBody] CustomerDetailOutputModel input)
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.MemberID = memberID.Value;
            return lgBus.UpdateMemberInfo(input);
        }

        /// <summary>
        /// Đăng xuất khỏi app
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object LogoutApp()
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return lgBus.LogOutApp(memberID.Value);
        }
        /// <summary>
        /// Cập nhật deviceID
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object UpdateDeviceID([FromBody] LoginAppInputModel input)
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.UserID = memberID.Value;
            return lgBus.UpdateDeviceID(input);
        }

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object FogotPassword([FromBody] LoginAppInputModel input)
        {
            return lgBus.FogotPassword(input.email);
        }
        [HttpPost]
        /// <summary>
        /// Check mã xác nhận
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object CheckOtpCode([FromBody] LoginAppInputModel input)
        {
            return lgBus.CheckOTP(input.email, input.OTP);
        }
        /// <summary>
        /// Đặt lại mật khẩu
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object ResetPassword([FromBody] LoginAppInputModel input)
        {
            return lgBus.ResetPassword(input.email, input.PassWord);
        }
        /// <summary>
        /// Đổi ảnh đại diện
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object ChangeAvatar()
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return lgBus.ChangeAvatar(memberID.Value);
        }
        /// <summary>
        /// Lấy danh sách tài khoản ngân hàng của shiper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetListBankOfShiper()
        {
            string token = GetToken();
            int? shiperID = lgBus.checkToken(token);
            if (!shiperID.HasValue)
                return rp.tokenError();
            return walletBus.GetListBankOfShiper(shiperID.Value);
        }
        /// <summary>
        /// Lấy danh sách tài khoản ngân hàng của shiper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetListProvince()
        {
            return addressBus.GetListProvince();
        }
        /// <summary>
        /// Lấy danh sách tài khoản ngân hàng của shiper
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetListDistrict(int ProvinceID)
        {
            return addressBus.GetListDistrict(ProvinceID);
        }
        /// <summary>
        /// Rút tiền đối với shiper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object WithDraw([FromBody] RequestWithDrawInputModel input)
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.shiperID = memberID.Value;
            return walletBus.CreateRequestWithDraw(input);
        }

        /// <summary>
        /// Chuyển tiền từ ví thu nhập sang ví cọc đối với shiper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object RechargeMoneyToDepositWallet(RequestWithDrawInputModel input)
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            if (input.amount < SystemParam.MIN_TRANSACTION_MONEY)
                return rp.ErrorResult("Số tiền mỗi lần chuyển phải từ 50.000 trở lên", SystemParam.PROCESS_ERROR);

            if (input.amount > SystemParam.MAX_TRANSACTION_MONEY)
                return rp.ErrorResult("Số tiền mỗi lần chuyển phải từ 10.000.000 trở xuống", SystemParam.PROCESS_ERROR);
            input.shiperID = memberID.Value;
            return walletBus.RechargeMoneyToDepositWallet(input);
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// Nạp tiền vào ví cọc
        public object RechargeMoneyToWallet(RequestDepositInputModel input)
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            if (input.Amount < SystemParam.MIN_TRANSACTION_MONEY)
                return rp.ErrorResult("Số tiền mỗi lần nạp phải từ 50.000 trở lên", SystemParam.PROCESS_ERROR);
            if (input.Amount > SystemParam.MAX_TRANSACTION_MONEY)
                return rp.ErrorResult("Số tiền mỗi lần nạp phải từ 10.000.000 trở xuống", SystemParam.PROCESS_ERROR);
            return walletBus.RechargeMoneyToWallet(memberID.Value, input.Amount);
        }

        public object UpdateAcceptServcieStatus()
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return shiperBus.UpdateAcceptServcieStatus(memberID.Value);
        }
        /// <summary>
        /// Lấy danh sách thông báo
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetNotify(int? type = null, int page = 1, int limit = 15)
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return notiBus.GetListNotify(memberID.Value, type, page, limit);
        }
        [HttpGet]
        public object CheckViewdNotify()
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return notiBus.CheckViewdNoti(memberID.Value);
        }
        // Xem thông báo 
        [HttpPost]
        public object ViewNotify(int ID = 0, int readAll = 0)
        {
            try
            {
                string token = GetToken();
                if (token.Length == 0)
                    return rp.tokenError();
                int? memberID = lgBus.checkToken(token);
                if (!memberID.HasValue)
                    return rp.tokenError();
                return notiBus.ViewNotify(ID, readAll, memberID.Value);
            }
            catch (Exception e)
            {
                return serverError();
            }
        }

        /// <summary>
        /// Lấy list rank
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetListRank()
        {
            return configBus.GetListRank();
        }

        [HttpGet]
        /// <summary>
        /// Lấy danh sách tên các ngân hàng có trên hệ thống
        /// </summary>
        /// <returns></returns>
        /// 
        public object GetListBankName()
        {
            return shiperBus.GetlistBankName();
        }
        /// <summary>
        /// Create shiper bank
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object CreateMemberBank([FromBody] BankDetailModel input)
        {
            if (String.IsNullOrEmpty(input.Account) || String.IsNullOrEmpty(input.AcountOwner) || input.BankID == 0)
                return rp.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.MemberID = memberID.Value;
            return shiperBus.CreateMemberBank(input);
        }


        [HttpPost]
        /// <summary>
        /// Cập nhật thông tin tài khoản ngân hàng của shiper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object UpdateBankMember([FromBody] BankDetailModel input)
        {
            if (String.IsNullOrEmpty(input.Account) || String.IsNullOrEmpty(input.AcountOwner) || input.BankID == 0)
                return rp.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.MemberID = memberID.Value;
            return shiperBus.UpdateBank(input);
        }

        /// <summary>
        /// Cập nhật vị trí của tài xế
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public object UpadateShiperLocation([FromBody] CustomerLocationModel input)
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            input.MemberID = memberID.Value;
            return shiperBus.UpadateShiperLocation(input);
        }

        ///// <summary>
        ///// 15 HomeScreen
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        public object GetHomeScreen()
        {
            return lgBus.GetHomeScreen();
        }

        ///// <summary>
        ///// 15 GetListNew
        ///// </summary>
        ///// <returns></returns>
        [HttpGet]
        public object GetListNews(int page, int limit, int? type = null)
        {
            try
            {
                DataPageListOutputModel data = new DataPageListOutputModel();
                var query = newsBus.GetListNews(page, limit, type, SystemParam.ACTIVE);
                data.page = page;
                data.limit = limit;
                data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
                data.data = query;

                return rp.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                return rp.serverError();
            }
        }

        /// <summary>
        /// Lấy danh sách lịch sử điểm của khách hàng
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetListHistoryPointMember(int page, int limit)
        {
            try
            {
                string token = GetToken();
                int? memberID = lgBus.checkToken(token);
                if (!memberID.HasValue)
                    return rp.tokenError();
                DataPageListOutputModel data = new DataPageListOutputModel();
                var query = transactionBus.GetListHistoryPointMember(page, limit, memberID.Value);
                data.page = page;
                data.limit = limit;
                data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
                data.data = query;

                return rp.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                return rp.serverError();
            }
        }
        /// <summary>
        /// Lấy danh sách câu hỏi thường gặp 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetListQA(int page, int limit, string searchKey = "")
        {
            try
            {
                DataPageListOutputModel data = new DataPageListOutputModel();
                var query = shiperBus.GetlistQA(page, limit, searchKey);
                data.page = page;
                data.limit = limit;
                data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
                data.data = query;

                return rp.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                return rp.serverError();
            }
        }
        /// <summary>
        /// Get list order
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetListOrder(int page, int limit, int? type = null,int? status = null,string fromDate = "",string toDate = "")
        {
            try
            {
                string token = GetToken();
                int? memberID = lgBus.checkToken(token);
                if (!memberID.HasValue)
                    return rp.tokenError();

                DataPageListOutputModel data = new DataPageListOutputModel();
                var query = orderBus.GetListOrder(page, limit, memberID.Value, type,status,fromDate,toDate);
                data.page = page;
                data.limit = limit;
                data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
                data.data = query;

                return rp.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                return rp.serverError();
            }
        }
        /// <summary>
        /// Lấy danh sách dịch vụ
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="searchKey"></param>
        /// <param name="cateID"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetListServicePrice(int page, int limit, string searchKey = "", int? cateID = null, int? shopID = null, int status = 1)
        {
            try
            {
                DataPageListOutputModel data = new DataPageListOutputModel();
                var query = servicePriceBusiness.GetListServicePrice(page, limit, searchKey, cateID, shopID, status);
                data.page = page;
                data.limit = limit;
                data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
                data.data = query;

                return rp.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                return rp.serverError();
            }
        }
        /// <summary>
        /// Lấy danh sách shop theo danh mục
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="cateID"></param>
        /// <param name="lati"></param>
        /// <param name="longi"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetlistShopByCategory(int page, int limit, int cateID, float lati, float longi, int type)
        {
            DataPageListOutputModel data = new DataPageListOutputModel();
            var query = servicePriceBusiness.GetlistShopByCategory(page, limit, cateID, lati, longi, type);
            data.page = page;
            data.limit = limit;
            data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
            data.data = query;

            return rp.SuccessResult(MessVN.SUCCESS_STR, data);
        }

        public object GetListShopByFood(int page, int limit, float lati, float longi, string searchKey = "")
        {
            DataPageListOutputModel data = new DataPageListOutputModel();
            var query = servicePriceBusiness.GetListShopByFood(page, limit, lati, longi, searchKey);
            data.page = page;
            data.limit = limit;
            data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
            data.data = query;

            return rp.SuccessResult(MessVN.SUCCESS_STR, data);
        }
        /// <summary>
        /// Lấy chi tiết shop cho app
        /// </summary>
        /// <param name="shopID"></param>
        /// <param name="lati"></param>
        /// <param name="longi"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetShopDetail(int shopID, float lati, float longi)
        {
            return shopBus.GetShopDetail(shopID, lati, longi);
        }
        /// <summary>
        /// Lấy danh sách thực đơn trong shop
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetMenuByShop(int shopID)
        {
            return shopBus.getMenuByShop(null, SystemParam.ACTIVE, shopID);
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpGet]
        public object GetOderServiceDetail(int orderID)
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return orderBus.GetOrderServiceDetail(orderID, memberID.Value);
        }

        public object GetReviewShop(int shopID, int? rateNumber = null)
        {
            return shopBus.GetReviewShop(shopID, rateNumber);
        }
        /// <summary>
        /// Lấy danh sách lịch sử ví
        /// </summary>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetListWalletHistory(int page, int limit, int? type = null)
        {
            string token = GetToken();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            DataPageListOutputModel data = new DataPageListOutputModel();
            var query = walletBus.GetListWalletHistory(page, limit, memberID.Value, type);
            data.page = page;
            data.limit = limit;
            data.totalPage = (int)Math.Ceiling((double)query.TotalItemCount);
            data.data = query;
            return rp.SuccessResult(MessVN.SUCCESS_STR, data);
        }
        /// <summary>
        /// Lấy chi tiết dịch vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetServicePriceDetail(int id)
        {
            return servicePriceBusiness.GetServicePriceDetail(id);
        }



        /// <summary>
        /// GetNewsDetail
        /// </summary>
        /// <returns></returns>
        public object GetNewsDetail(int id)
        {
            return newsBus.GetNewsDetail(id);
        }

        /// <summary>
        /// Get list service category
        /// </summary>
        /// <returns></returns>

        public object GetListServiceCategory(float? lati = null, float? longi = null)
        {
            return serviceCategoryBusiness.GetListServiceCategory(lati, longi);
        }


        /// <summary>
        /// Đánh giá đơn hàng
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public object OrderReview([FromBody] RateDataInputModel input)
        {
            string token = GetToken();
            if (token.Length == 0)
                return rp.tokenError();
            int? memberID = lgBus.checkToken(token);
            if (!memberID.HasValue)
                return rp.tokenError();
            return orderBus.OrderReview(input.OrderID, input.Rate, input.NoteRate, input.Type);
        }
        public object UpdateOrderReview()
        {
            return orderBus.UpdateOrderReview();
        }



        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="vnp_Amount"></param>
        ///// <param name="vnp_BankCode"></param>
        ///// <param name="vnp_CardType"></param>
        ///// <param name="vnp_OrderInfo"></param>
        ///// <param name="vnp_PayDate"></param>
        ///// <param name="vnp_ResponseCode"></param>
        ///// <param name="vnp_TmnCode"></param>
        ///// <param name="vnp_TransactionNo"></param>
        ///// <param name="vnp_TxnRef"></param>
        ///// <param name="vnp_SecureHashType"></param>
        ///// <param name="vnp_SecureHash"></param>
        ///// <param name="vnp_BankTranNo"></param>
        ///// <returns></returns>
        [HttpGet]
        public VNPayOutputModel vnp_ipn(string vnp_Amount, string vnp_BankCode, string vnp_CardType, string vnp_OrderInfo, string vnp_PayDate, string vnp_ResponseCode, string vnp_TmnCode, string vnp_TransactionNo, string vnp_TxnRef, string vnp_SecureHashType, string vnp_SecureHash, string vnp_BankTranNo = "")
        {
            VnpOutputModel vnp = new VnpOutputModel();
            vnp.vnp_Amount = vnp_Amount;
            vnp.vnp_BankCode = vnp_BankCode;
            vnp.vnp_BankTranNo = vnp_BankTranNo;
            vnp.vnp_CardType = vnp_CardType;
            vnp.vnp_OrderInfo = vnp_OrderInfo;
            vnp.vnp_PayDate = vnp_PayDate;
            vnp.vnp_ResponseCode = vnp_ResponseCode;
            vnp.vnp_TmnCode = vnp_TmnCode;
            vnp.vnp_TransactionNo = vnp_TransactionNo;
            vnp.vnp_TxnRef = vnp_TxnRef;
            vnp.vnp_SecureHashType = vnp_SecureHashType;
            vnp.vnp_SecureHash = vnp_SecureHash;
            return vnPayBus.GetVnpIpn(vnp);
        }


        public string GetToken()
        {
            if (Request.Headers.Contains("token"))
            {
                return Request.Headers.GetValues("token").FirstOrDefault();
            }
            return "";
        }
    }
    public class DataTest
    {
        public object data1 { get; set; }
        public object data2 { get; set; }
    }
}
