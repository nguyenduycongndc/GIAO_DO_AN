using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Utils;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Linq;



namespace Data.Business
{
    public class ShiperBusiness : GenericBusiness
    {
        // PlaceBusiness placeBus = new PlaceBusiness();
        // AreaBusiness areaBus = new AreaBusiness();
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public ShiperBusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public JsonResultModel GetlistBankName()
        {
            try
            {
                List<BankOutputModel> data = new List<BankOutputModel>();
                data = cnn.Banks.Where(b => b.IsActive.Equals(SystemParam.ACTIVE))
                    .Select(b => new BankOutputModel
                    {
                        BankID = b.ID,
                        BankName = b.Name,
                        Code = b.Code
                    }).ToList();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        /// <summary>
        /// Tạo thẻ ngân hàng cho shiper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel CreateMemberBank(BankDetailModel input)
        {
            try
            {
                BankMember b = new BankMember();
                b.BankID = input.BankID;
                b.MemberID = input.MemberID;
                b.Account = input.Account;
                b.AccountOwner = input.AcountOwner;
                b.IsActive = SystemParam.ACTIVE;
                b.CrteatedDate = DateTime.Now;
                cnn.BankMembers.Add(b);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        public IPagedList<ListQAoutputModel> GetlistQA(int page, int limit, string searchKey)
        {
            try
            {
                var data = cnn.QAs.Where(q => q.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(searchKey) ? q.Question.Contains(searchKey) : true))
                    .Select(q => new ListQAoutputModel
                    {
                        ID = q.ID,
                        Question = q.Question,
                        Answer = q.Answer
                    }).OrderByDescending(q => q.ID).ToPagedList(page, limit);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListQAoutputModel>().ToPagedList(1, 1);
            }
        }
        /// <summary>
        /// Cập nhật thông tin tài khoản thẻ ngân hàng  của shiper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel UpdateBank(BankDetailModel input)
        {
            try
            {
                BankMember bm = cnn.BankMembers.Find(input.BankIDShiper);
                if (bm == null || !bm.MemberID.Equals(input.MemberID))
                    return rpBus.ErrorResult("Tài khoản không hợp lệ", SystemParam.PROCESS_ERROR);
                bm.AccountOwner = input.AcountOwner;
                bm.Account = input.Account;
                bm.BankID = input.BankID;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel UpadateShiperLocation(CustomerLocationModel input)
        {
            try
            {
                Member mb = cnn.Members.Find(input.MemberID);
                if (mb == null || !mb.ShiperID.HasValue)
                    return rpBus.ErrorResult(MessVN.ERROR_ROLE_INFO, SystemParam.PROCESS_ERROR);
                mb.Shiper.Lati = input.lati;
                mb.Shiper.Longi = input.Longi;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel UpdateAcceptServcieStatus(int memberID)
        {
            try
            {
                Member mb = cnn.Members.Find(memberID);
                if (!mb.ShiperID.HasValue)
                    return rpBus.ErrorResult(MessVN.ERROR_ROLE_INFO, SystemParam.PROCESS_ERROR);
                Shiper sp = cnn.Shipers.Find(mb.ShiperID.Value);
                sp.IsAcceptService = !sp.IsAcceptService;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
    }
}
