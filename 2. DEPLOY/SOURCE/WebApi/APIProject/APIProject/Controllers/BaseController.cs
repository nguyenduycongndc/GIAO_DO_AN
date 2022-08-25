using Data.Business;
using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class BaseController : Controller
    {
        protected WE_SHIPEntities Context;
        public LoginBusiness loginBusiness;
        public ServiceCategoryBusiness serviceCategoryBusiness;
        public NewsBusiness newsBusiness;
        public WalletBusiness walletBusiness;
        //public BatchBusiness batchBusiness;
        //public CardBusiness cardBusiness;
        //public RequestBusiness requestBusiness;
        //public NewsBusiness newsBusiness;
        public CustomerBusiness cusBusiness;
        public CouponBusiness couponBusiness;
        public ReportBusiness reportBusiness;
        //public RankBusiness rankBusiness;
        public ConfigBusiness configBusiness;
        //public TransactionHistoryBusiness pointBusiness;
        //public WarrantyBusiness warrantyBusiness;
        //public StatisticBusiness statisticBus;
        public OrderServiceBusiness orderServiceBus;
        public UserBusiness userBusiness;
        //public ServiceBusiness serviceBusiness;
        public NotifyBusiness notifyBusiness;
        public ShopBusiness shopBusiness;
        //public AgentBusiness agentBusiness;
        //public PlaceBusiness placeBusiness;
        //public CarBusiness carBusiness;
        //public AdditionServiceBusiness additionServiceBusiness;
        //public VNPayBusiness vnpay;
        //public ServiceComboBusiness comboBus;
        //public WalletBusiness walletBusiness;
        //public ReviewBusiness reviewBusiness;
        public CategoryBusiness cateBusiness;
        //public ProductBusiness productBusiness;
        //public RankingBusiness rankBusiness;
        //public OrderProductBusiness orderProductBusiness;
        //public ShiftBusiness shiftBusiness;
        public ShipperBusiness shipperBusiness;
        public TransactionHistoryBusiness transactionBusiness;
        public TransactionFoodBusiness transactionFoodBusiness;
        public TransactionDeliveryBusiness transactionDeliveryBusiness;
        public VNPayBusiness vnpayBus;
        public BaseController() : base()
        {
            loginBusiness = new LoginBusiness(this.GetContext());
            //batchBusiness = new BatchBusiness(this.GetContext());
            //cardBusiness = new CardBusiness(this.GetContext());
            //requestBusiness = new RequestBusiness(this.GetContext());
            newsBusiness = new NewsBusiness(this.GetContext());
            cusBusiness = new CustomerBusiness(this.GetContext());
            couponBusiness = new CouponBusiness(this.GetContext());
            walletBusiness = new WalletBusiness(this.GetContext());
            reportBusiness = new ReportBusiness(this.GetContext());
            //rankBusiness = new RankBusiness(this.GetContext());
            configBusiness = new ConfigBusiness(this.GetContext());
            //pointBusiness = new TransactionHistoryBusiness(this.GetContext());
            //warrantyBusiness = new WarrantyBusiness(this.GetContext());
            //statisticBus = new StatisticBusiness(this.GetContext());
            orderServiceBus = new OrderServiceBusiness(this.GetContext());
            userBusiness = new UserBusiness(this.GetContext());
            //serviceBusiness = new ServiceBusiness(this.GetContext());
            notifyBusiness = new NotifyBusiness(this.GetContext());
            shopBusiness = new ShopBusiness(this.GetContext());
            //agentBusiness = new AgentBusiness(this.GetContext());
            //placeBusiness = new PlaceBusiness(this.GetContext());
            //carBusiness = new CarBusiness(this.GetContext());
            //vnpay = new VNPayBusiness(this.GetContext());
            //comboBus = new ServiceComboBusiness(this.GetContext());
            //additionServiceBusiness = new AdditionServiceBusiness(this.GetContext());
            //walletBusiness = new WalletBusiness(this.GetContext());
            //reviewBusiness = new ReviewBusiness(this.GetContext());
            cateBusiness = new CategoryBusiness(this.GetContext());
            //productBusiness= new ProductBusiness(this.GetContext());
            //rankBusiness = new RankingBusiness(this.GetContext());
            //orderProductBusiness = new OrderProductBusiness(this.GetContext());
            //shiftBusiness = new ShiftBusiness(this.GetContext());
            serviceCategoryBusiness = new ServiceCategoryBusiness(this.GetContext());
            shipperBusiness = new ShipperBusiness(this.GetContext());
            transactionBusiness = new TransactionHistoryBusiness(this.GetContext());
            transactionFoodBusiness = new TransactionFoodBusiness(this.GetContext());
            transactionDeliveryBusiness = new TransactionDeliveryBusiness(this.GetContext());
            vnpayBus = new VNPayBusiness(this.GetContext());
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

        public UserDetailOutputModel UserLogins
        {
            get
            {
                return Session[SystemParam.SESSION_LOGIN] as UserDetailOutputModel;
                
            }
        }




    }
}