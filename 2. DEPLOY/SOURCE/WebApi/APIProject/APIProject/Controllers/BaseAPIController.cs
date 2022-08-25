using Data.Business;
using Data.DB;
using Data.Model.APIApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using APIProject.Models;
using Data.Utils;

namespace APIProject.Controllers
{
    public class BaseAPIController : ApiController
    {
        protected WE_SHIPEntities Context;
        public LoginBusiness lgBus;
        public BookingBusiness bookBus;
        public NewsBusiness newsBus;
        public ServiceCategoryBusiness serviceCategoryBusiness;
        public ServicePriceBusiness servicePriceBusiness;
        public NotifyBusiness notiBus;
        public ShopBusiness shopBus;
        public ShiperBusiness shiperBus;
        public CartBusiness cartBus;
        public VNPayBusiness vnPayBus;
        public AddressBusiness addressBus;
        //public CustomerBusiness cusBus;
        public TransactionHistoryBusiness transactionBus;
        //public AreaBusiness areaBus;
        //public RequestAPIBusiness apiBus;
        //public StatisticBusiness statisticBus;
        public ConfigBusiness configBus;
        //public ServiceBusiness serviceBus;
        //public CarBusiness carBus;
        //public OrderImageBusiness orderImageBus;
        public OrderServiceBusiness orderBus;
        //public OneSignalBusiness oneSignal;
        //public PlaceBusiness placeBus;
        //public CouponBusiness couponBus;
        //public AgentBusiness agentBus;
        //public IntroBusiness introBus;
        //public SendRequestBusiness sendRequestBus;
        //public ProductBusiness proBus;
        public WalletBusiness walletBus;
        //public ShiftBusiness shiftBus;
        //public VNPayBusiness vnPay;
        //public QABusiness qaBus;
        //public OrtherBusiness ortherBus;
        //public BankBusiness bankBus;
        //public ServiceComboBusiness comboBus;
        //public RequestBusiness requestBus;
        //public AdditionServiceBusiness addBus;
        //public ReviewBusiness reviewBus;
        //public ReportBusiness reportBus;
        //public VNPayBusiness vnpay;
        //public RankingBusiness rankBus;
        //public OrderProductBusiness orderProBus;
        //public EmailBusiness emailBus;
        //public UserBusiness userBusiness;
        //public MomoBusiness momo;

        public BaseAPIController() : base()
        {
            lgBus = new LoginBusiness(this.GetContext());
            bookBus = new BookingBusiness(this.GetContext());
            //proBus = new ProductBusiness(this.GetContext());
            newsBus = new NewsBusiness(this.GetContext());
            serviceCategoryBusiness = new ServiceCategoryBusiness(this.GetContext());
            servicePriceBusiness = new ServicePriceBusiness(this.GetContext());
            notiBus = new NotifyBusiness(this.GetContext());
            shopBus = new ShopBusiness(this.GetContext());
            addressBus = new AddressBusiness(this.GetContext());
            shiperBus = new ShiperBusiness(this.GetContext());
            cartBus = new CartBusiness(this.GetContext());
            vnPayBus = new VNPayBusiness(this.GetContext());
            //placeBus = new PlaceBusiness(this.GetContext());
            //cusBus = new CustomerBusiness(this.GetContext());
            //apiBus = new RequestAPIBusiness(this.GetContext());
            transactionBus = new TransactionHistoryBusiness(this.GetContext());
            //statisticBus = new StatisticBusiness(this.GetContext());
            //serviceBus = new ServiceBusiness(this.GetContext());
            orderBus = new OrderServiceBusiness(this.GetContext());
            //oneSignal = new OneSignalBusiness(this.GetContext());
            //carBus = new CarBusiness(this.GetContext());
            //orderImageBus = new OrderImageBusiness(this.GetContext());
            //couponBus = new CouponBusiness(this.GetContext());
            configBus = new ConfigBusiness(this.GetContext());
            //areaBus = new AreaBusiness(this.GetContext());
            //agentBus = new AgentBusiness(this.GetContext());
            //introBus = new IntroBusiness(this.GetContext());
            //sendRequestBus = new SendRequestBusiness(this.GetContext());
            walletBus = new WalletBusiness(this.GetContext());
            //shiftBus = new ShiftBusiness(this.GetContext());
            //vnPay = new VNPayBusiness(this.GetContext());
            //qaBus = new QABusiness(this.GetContext());
            //ortherBus = new OrtherBusiness(this.GetContext());
            //comboBus = new ServiceComboBusiness(this.GetContext());
            //bankBus = new BankBusiness(this.GetContext());
            //requestBus = new RequestBusiness(this.GetContext());
            //addBus = new AdditionServiceBusiness(this.GetContext());
            //reviewBus = new ReviewBusiness(this.GetContext());
            //reportBus = new ReportBusiness(this.GetContext());
            //vnpay = new VNPayBusiness(this.GetContext());
            //orderProBus = new OrderProductBusiness(this.GetContext());
            //rankBus = new RankingBusiness(this.GetContext());
            //emailBus = new EmailBusiness();
            //userBusiness = new UserBusiness(this.GetContext());
            //momo = new MomoBusiness(this.GetContext());

        }
        public JsonModel response(int status, int code, string message, object data)
        {
            JsonModel result = new JsonModel();
            result.Status = status;
            result.Code = code;
            result.Message = message;
            result.Data = data;
            return result;
        }
        public JsonModel serverError()
        {
            JsonModel result = new JsonModel();
            result.Status = SystemParam.ERROR;
            result.Code = SystemParam.PROCESS_ERROR;
            result.Message = MessVN.ERROR_STR;
            result.Data = "";
            return result;
        }
        // lấy token request
        public string getTokenApp()
        {
            if (Request.Headers.Contains("token"))
            {
                return Request.Headers.GetValues("token").FirstOrDefault();
            }
            return "";
        }
        /// <summary>
        /// Create new context if null
        /// </summary>
        public WE_SHIPEntities GetContext()
        {
            if (Context == null)
            {
                Context = new WE_SHIPEntities();
            }
            return Context;
        }



    }
}