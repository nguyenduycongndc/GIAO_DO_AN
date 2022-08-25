using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using PagedList;
using System;
using SharpRaven;
using SharpRaven.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class CustomerBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public CustomerBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }

        /// <summary>
        /// Tạo Khách hàng mới
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        //    public UserInforOutputModel UpdateCustomer(UserInforOutputModel item, TokenOutputModel checkToken, string lang)
        //    {
        //        try
        //        {
        //            string phone = Util.ConvertPhone(item.Phone);
        //            if (checkToken.CustomerID.HasValue)
        //            {
        //                Customer cus = cnn.Customers.Find(checkToken.CustomerID.Value);
        //                cus.Name = item.Name;
        //                cus.Address = item.Address;
        //                try
        //                {
        //                    cus.DOB = DateTime.ParseExact(item.dobStr, SystemParam.CONVERT_DATETIME, null);
        //                }
        //                catch { }
        //                cus.Email = item.Email;
        //                cus.Sex = item.Sex;
        //                cus.Phone = item.Phone;
        //                cus.ProvinceCode = item.ProvinceCode;
        //                Member member = cnn.Members.Find(checkToken.MemberID);
        //                if (!string.IsNullOrEmpty(item.PassWord) && item.PassWord.Length > 0)
        //                {
        //                    member.Password = Util.CreateMD5(item.PassWord);
        //                }
        //                cnn.SaveChanges();
        //            }
        //            else if (checkToken.AgentID.HasValue)
        //            {
        //                Agent agent = cnn.Agents.Find(checkToken.AgentID.Value);
        //                agent.Name = item.Name;
        //                agent.Phone = item.Phone;
        //                agent.Email = item.Email;
        //                agent.AcceptService = item.AcceptService;
        //                cnn.SaveChanges();
        //            }
        //            else
        //                return null;
        //            return GetuserInfor(checkToken.CustomerID, checkToken.AgentID, lang);
        //        }
        //        catch (Exception ex)
        //        {
        //            string a = ex.ToString();
        //            return null;
        //        }
        //    }
        //    public SystemResult ChangeStatusCustomer(int cusID, int status)
        //    {
        //        Member member = cnn.Members.Where(u => u.CustomerID.Value.Equals(cusID)).FirstOrDefault();
        //        member.IsActive = status;
        //        cnn.SaveChanges();
        //        return resultBus.SucessResult("");
        //    }
        //    public CustomerOffice CreateOffice(int cusID, string VAT, string Name, string Address, string bankName, string account)
        //    {
        //        CustomerOffice cusO = new CustomerOffice
        //        {
        //            ID = 1000,
        //            Address = Address,
        //            Name = Name,
        //            CreateDate = DateTime.Now,
        //            IsActive = SystemParam.ACTIVE,
        //            CustomerID = cusID,
        //            VAT = VAT,
        //            BankName = bankName,
        //            Account = account
        //        };
        //        cnn.CustomerOffices.Add(cusO);
        //        cnn.SaveChanges();
        //        return cusO;
        //    }
        //    public UserInforOutputModel GetuserInfor(int? CusID, int? AgentID, string lang)
        //    {
        //        Member mber = cnn.Members.Where(member => (CusID.HasValue ? member.CustomerID.Value.Equals(CusID.Value) : true) && (AgentID.HasValue ? member.AgentID.Value.Equals(AgentID.Value) : true)).FirstOrDefault();
        //        UserInforOutputModel output = (from member in cnn.Members
        //                                       where (CusID.HasValue ? member.CustomerID.Value.Equals(CusID.Value) : true) && (AgentID.HasValue ? member.AgentID.Value.Equals(AgentID.Value) : true)
        //                                       select new UserInforOutputModel
        //                                       {
        //                                           UserID = CusID.HasValue ? CusID.Value : AgentID.Value,
        //                                           MemberID = member.ID,
        //                                           Address = CusID.HasValue ? member.Customer.Address : "",
        //                                           Token = member.Token,
        //                                           ProvinceCode = CusID.HasValue ? member.Customer.ProvinceCode : 1,
        //                                           DistrictCode = CusID.HasValue ? member.Customer.DistrictCode : 0,
        //                                           ProvinceName = CusID.HasValue ? (member.Customer.ProvinceCode.HasValue ? member.Customer.Province.Name : "Hà Nội") : "",
        //                                           DistrictName = CusID.HasValue ? (member.Customer.DistrictCode.HasValue ? member.Customer.District.Name : "") : "",
        //                                           DOB = CusID.HasValue ? member.Customer.DOB : null,
        //                                           RankPoint = CusID.HasValue ? member.Customer.RankingPoint.Value : 0,
        //                                           Name = CusID.HasValue ? member.Customer.Name : member.Agent.Name,
        //                                           Email = CusID.HasValue ? member.Customer.Email : member.Agent.Email,
        //                                           Phone = CusID.HasValue ? member.Customer.Phone : member.Agent.Phone,
        //                                           WithdrawPoint = CusID.HasValue ? 0 : member.Wallets.Where(u => u.TYPE.Equals(Constant.WALLET_WITHDRAW)).FirstOrDefault().Balance,
        //                                           Point = member.Wallets.Where(u => u.TYPE.Equals(Constant.WALLET_NO_WITHDRAW)).FirstOrDefault().Balance,
        //                                           Role = CusID.HasValue ? Constant.CUSTOMER_ROLE : Constant.AGENT_ROLE,
        //                                           Sex = CusID.HasValue ? member.Customer.Sex : 1,
        //                                           UrlAvatar = CusID.HasValue ? member.Customer.AvatarUrl : member.Agent.AvatarUrl,
        //                                           isNeedUpdate = mber.Password.Length > 0 ? 0 : 1,
        //                                           Wallet = member.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new { ID = u.ID, Balance = u.Balance, Type = u.TYPE }).ToList()
        //                                       }).FirstOrDefault();
        //        output.dobStr = output.DOB.HasValue ? output.DOB.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
        //        if (AgentID.HasValue)
        //        {
        //            Agent agent = cnn.Agents.Find(AgentID.Value);
        //            output.AgentArea = cnn.AgentAreas.Where(u => u.AgentID.Equals(AgentID.Value) && u.IsActive.Equals(SystemParam.ACTIVE) && u.Area.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new
        //            {
        //                ID = u.ID,
        //                Name = u.Area.Name,
        //                DistrictCode = u.Area.DistrictCode,
        //                DistrictName = u.Area.District.Name
        //            }).ToList();
        //            output.ListBank = cnn.BankMembers.Where(u => u.MemberID.Equals(output.MemberID) && u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new BankDetailModel
        //            {
        //                ID = u.ID,
        //                Acount = u.Acount,
        //                AcountOwner = u.AcountOwner,
        //                BankName = u.Bank.Name,
        //                BankID = u.BankID,
        //                Code = u.Bank.Code
        //            }).ToList();
        //            output.AcceptService = agent.AcceptService.HasValue ? agent.AcceptService.Value : 0;
        //            output.Code = agent.Code;
        //            output.Rate = agent.Rating.HasValue ? agent.Rating.Value : 0;
        //            output.longi = agent.longi.HasValue ? agent.longi.Value : 0;
        //            output.lati = agent.lati.HasValue ? agent.lati.Value : 0;
        //            output.ModifyDate = agent.ModifyDate;
        //            output.Commission = agent.CommissionID.HasValue ? agent.ConfigCommission.MastersBenefit.Value : 0;
        //            Wallet wallet = cnn.Wallets.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.TYPE.Equals(Constant.WALLET_WITHDRAW) && u.MemberID.Equals(mber.ID)).FirstOrDefault();
        //            MembersTransactionHistory history = cnn.MembersTransactionHistories.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.TYPE_TRANSACTION_WITHDRAW) && !u.Status.Equals(Constant.STATUS_REQUEST_REJECT) && u.MemberID.Equals(mber.ID)).OrderByDescending(u => u.ID).FirstOrDefault();
        //            string mess = walletBus.CheckWithDrawWallet(wallet, 0, history, lang);
        //            if (mess.Equals(SystemParam.SUCCES_STR))
        //                output.WithdrawNote = "";
        //            else
        //                output.WithdrawNote = mess;
        //        }
        //        else
        //        {
        //            Customer cus = cnn.Customers.Find(CusID.Value);
        //            CustomerRank nextRank = cnn.CustomerRanks.Where(u => u.Level.Value.Equals(cus.CustomerRank.Level.Value + 1)).FirstOrDefault();
        //            int order = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.LIMIT_CANCEL)).Select(u => u.ValueConstant).FirstOrDefault());
        //            output.CancelOrder = CusID.HasValue ? (cus.QTYCancel.HasValue ? order - cus.QTYCancel.Value : order) : order;
        //            output.ListCar = cnn.CarCustomers.Where(u => u.CustomerID.Equals(CusID.Value) && u.IsActive.Equals(SystemParam.ACTIVE) && u.CarModel.CarSegmentID.HasValue).Select(c => new CarOutputModel
        //            {
        //                carID = c.ID,
        //                CarBrand = c.CarModel.CarBrand.Name,
        //                CarModelID = c.CarModeID,
        //                CarModel = c.CarModel.Name,
        //                CarColor = c.CarColor,
        //                LicensePlates = c.LicensePlates,
        //                CarBrandID = c.CarModel.CarBrandID.Value,
        //                ManufacturingDate = c.ManufacturingDate,
        //                RegistrationDate = c.RegistrationDate.Value,
        //                Status = String.IsNullOrEmpty(c.StatusCar) ? "" : c.StatusCar,
        //                VehicleRegistration = String.IsNullOrEmpty(c.VehicleRegistration) ? "" : c.VehicleRegistration,
        //                ListImage = c.CarImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(ci => new
        //                {
        //                    Url = ci.Path,
        //                    ImageID = ci.ID
        //                }).ToList()
        //            }).OrderByDescending(u => u.CarBrandID).ToList();
        //            output.MyRefCode = output.Phone;
        //            output.RefCode = cus.RefCode;
        //            output.listLocation = cnn.CustomerLocations.Where(u => u.CustomerID.Equals(CusID.Value) && u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new CustomerLocationModel
        //            {
        //                ID = u.ID,
        //                lati = u.Lati,
        //                Longi = u.Longi,
        //                Name = u.Name,
        //                PlaceID = u.PlaceID,
        //                Address = u.Address,
        //                CustomerAddress = u.Address
        //            }).ToList();
        //            output.listOffice = cnn.CustomerOffices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.Equals(CusID.Value)).Select(u => new Model.APIApp.CustomerOffices
        //            {
        //                ID = u.ID,
        //                OfficeAddress = u.Address,
        //                OfficeName = u.Name,
        //                OfficeVAT = u.VAT,
        //                Account = u.Account,
        //                BankName = u.BankName
        //            }).ToList();
        //            output.RankName = cus.CustomerRankID.HasValue ? cus.CustomerRank.Name : "";
        //            output.RankLevel = cus.CustomerRankID.HasValue ? cus.CustomerRank.Level.Value : 1;
        //            output.RankUperPoint = 1;
        //            if (nextRank != null)
        //            {
        //                output.RankUperPoint = nextRank.MinPoint;
        //                int point = nextRank.MinPoint - output.RankPoint;
        //                string pointStr = string.Format("{0:#,0}", point);
        //                output.ContentRank = lang.Equals(SystemParam.VN) ? "Bạn cần " + pointStr + " điểm để lên hạng " + nextRank.Name + " .Tăng hạng để được nhiều ưu đãi hơn." : "You earn " + pointStr + " point to level up rank " + nextRank.Name + " .Level up for more deals.";
        //            }
        //            else
        //            {
        //                output.ContentRank = lang.Equals(SystemParam.VN) ? "Bạn đang ở hạng " + output.RankName + " .Tiếp tục tích để được nhiều ưu đãi" : "You'r rank is " + output.RankName + " .Continue to earn points for more deals.";

        //            }

        //        }
        //        if (!String.IsNullOrEmpty(output.UrlAvatar))
        //            output.UrlAvatar = output.UrlAvatar;
        //        else
        //            output.UrlAvatar = "";
        //        return output;
        //    }
        //    public List<Province> LoadCityCustomer()
        //    {
        //        List<Province> listCity = new List<Province>();
        //        var query = cnn.Provinces.ToList();

        //        if (query != null && query.Count() > 0)
        //        {
        //            listCity = query.ToList();
        //            return listCity;
        //        }
        //        else
        //            return new List<Province>();
        //    }

        //    public List<District> loadDistrict(int ProvinceID)
        //    {
        //        List<District> listDistrict = new List<District>();
        //        var query = from d in cnn.Districts
        //                    where d.ProvinceCode.Equals(ProvinceID)
        //                    select d;
        //        if (query != null && query.Count() > 0)
        //        {
        //            //listDistrict = query.ToList();
        //            return query.ToList();
        //        }
        //        else
        //            return new List<District>();
        //    }
        //    public List<CustomerRank> ListRank(string code)
        //    {
        //        Customer cus = cnn.Customers.Where(u => u.MyRefCode.Equals(code)).FirstOrDefault();
        //        List<CustomerRank> lsCus = cnn.CustomerRanks.Select(u => u).ToList();
        //        return lsCus;
        //    }
        public IPagedList<ListCustomerOutputModel> Search(int page, string codeOrName, int? Rank, int? prvovinceID,int? IsVip)
        {
            var cus = (from c in cnn.Customers
                       where (c.IsActive == SystemParam.ACTIVE)
                       && (c.Members.Any(m => m.IsActive == SystemParam.ACTIVE) || c.Members.Any(m => m.IsActive == 2))
                       && (!String.IsNullOrEmpty(codeOrName) ? c.Name.Contains(codeOrName) || c.Phone.Contains(codeOrName) : true)
                       && (prvovinceID.HasValue ? c.ProvinceID == prvovinceID : true)
                       && (IsVip.HasValue ? (IsVip.Value.Equals(SystemParam.CUSTOMER_VIP) ? c.IsVip == IsVip : (c.IsVip.Value.Equals(SystemParam.CUSTOMER_NORMAL) || !c.IsVip.HasValue)) : true)
                       && (Rank.HasValue ? c.CustomerRankID == Rank : true)
                       orderby c.CreatedDate descending
                       select new ListCustomerOutputModel()
                       {
                           CustomerID = c.ID,
                           CustomerName = c.Name,
                           PhoneNumber = c.Phone,
                           Email = c.Email,
                           Address = c.Address,
                           CreateDate = c.CreatedDate,
                           DOB = c.DOB,
                           IsVip = c.IsVip.HasValue ? c.IsVip.Value : SystemParam.CUSTOMER_NORMAL,
                           RankingPoint = c.RankingPoint,
                           RankingName = c.CustomerRank.Description,
                           Status = c.Members.Select(m => m.IsActive).FirstOrDefault()
                       }
                       ).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
            return cus;
        }
        public int InActiveCustomer(int ID)
        {
            try
            {
                Member mb = (from mem in cnn.Members where mem.CustomerID == ID select mem).FirstOrDefault();
                mb.IsActive = mb.IsActive.Equals(SystemParam.ACTIVE) ? SystemParam.DEACTIVE : SystemParam.ACTIVE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int IsVipCustomer(int ID)
        {
            try
            {
                Customer cus = cnn.Customers.Where(x => x.ID.Equals(ID)).FirstOrDefault();
                cus.IsVip = cus.IsVip.GetValueOrDefault().Equals(SystemParam.CUSTOMER_VIP) ? SystemParam.CUSTOMER_NORMAL : SystemParam.CUSTOMER_VIP;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int UpdateDiscountCustomer(int ID , double Discount)
        {
            try
            {
                Customer cus = cnn.Customers.Where(x => x.ID.Equals(ID)).FirstOrDefault();
                cus.VipDiscount = Discount;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }


        public List<CustomerRank> GetListCustomerRank()
        {
            try
            {
                List<CustomerRank> data = new List<CustomerRank>();
                data = cnn.CustomerRanks.Where(c => c.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<CustomerRank>();
            }
        }
        //    public object SearchTest(int page, string codeOrName = "", string phone = "", string email = "", int? active = null, string fDate = "", string tDate = "")
        //    {
        //        string a = Util.Converts(codeOrName.ToLower());
        //        DateTime? fromDate = Util.ConvertDate(fDate);
        //        DateTime? toDate = Util.ConvertDate(tDate);
        //        var listCustomerHaveRefCode = cnn.Members.Where(u => u.CustomerID.HasValue
        //        && u.IsLogin.HasValue
        //        && !u.IsActive.Equals(SystemParam.INACTIVE) && !String.IsNullOrEmpty(u.Customer.RefCode)).Select(u => u.Customer.RefCode).Distinct().ToList();
        //        var lsCus = cnn.Customers.Where(u => listCustomerHaveRefCode.Contains(u.Phone) && u.Members.FirstOrDefault().IsActive.Equals(SystemParam.ACTIVE)).ToList();
        //        if (toDate.HasValue)
        //            toDate = toDate.Value.AddDays(1);
        //        var lsCustomer = cnn.Members.Where(u =>
        //        u.CustomerID.HasValue
        //        && u.IsLogin.HasValue
        //        && !u.IsActive.Equals(SystemParam.INACTIVE)
        //        && u.Customer.Phone.Contains(phone)
        //        && u.Customer.Email.Contains(email)
        //        && (active.HasValue && active > 0 ? u.IsActive.Equals(active.Value) : true)
        //        && (fromDate.HasValue ? u.CreateDate >= fromDate.Value : true)
        //        && (toDate.HasValue ? u.CreateDate <= toDate.Value : true)
        //        && (u.Customer.MyRefCode.Contains(codeOrName) || u.Customer.Name.Contains(codeOrName))
        //        ).OrderByDescending(u => u.CustomerID).Select(cus => new ListCustomerOutputModel
        //        {
        //            Code = cus.Customer.MyRefCode,
        //            CustomerID = cus.Customer.ID,
        //            ProvinceName = cus.Customer.ProvinceCode.HasValue ? cus.Customer.Province.Name : "",
        //            CustomerName = cus.Customer.Name,
        //            PhoneNumber = cus.User,
        //            CarCustomerID = cus.Customer.CarCustomers.Select(u => u.ID).FirstOrDefault(),
        //            CarBrand = cus.Customer.CarCustomers.Select(u => u.CarModel.CarBrand.Name).FirstOrDefault(),
        //            CarModel = cus.Customer.CarCustomers.Select(u => u.CarModel.Name).FirstOrDefault(),
        //            Email = cus.Customer.Email,
        //            ProvinceCode = cus.Customer.ProvinceCode,
        //            DistrictCode = cus.Customer.DistrictCode,
        //            Status = cus.IsActive,
        //            Point = cus.Wallets.FirstOrDefault().Balance,
        //            CreateDate = cus.CreateDate,
        //            Sex = cus.Customer.Sex,
        //            RefCode = cus.Customer.RefCode,
        //            RankingName = cus.Customer.CustomerRank.Name,
        //            Address = cus.Customer.Address,
        //            QTYCancel = cus.Customer.QTYCancel,
        //            order = cus.Customer.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Count(),
        //            RefCustomer = lsCus.Where(u => u.Phone.Equals(cus.Customer.RefCode)).Select(u => u.Name).FirstOrDefault(),
        //            RefCustomerID = lsCus.Where(u => u.Phone.Equals(cus.Customer.RefCode)).Select(u => u.ID).FirstOrDefault(),
        //            Rate = cus.Customer.OrderServices.Where(u => u.IsRate.HasValue && u.IsRate.Value.Equals(SystemParam.ACTIVE) && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
        //            .GroupBy(u => u.CustomerID).Select(u => (double)(u.Select(o => o.Rate).Sum() / u.Count())).FirstOrDefault(),
        //            revenuaPackage = cus.Customer.OrderServices.Where(u => !u.ServiceComboSegmentID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE) && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
        //            .GroupBy(u => u.CustomerID).Select(u => u.Select(o => o.TotalPrice).Sum()).FirstOrDefault(),
        //            revenuaCombo = cus.Customer.OrderServices.Where(u => u.ServiceComboSegmentID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE) && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
        //            .GroupBy(u => new { u.CustomerID, u.CodeCombo }).Select(u => u.Select(o => o.TotalPrice).Sum()).Sum(),
        //        }).ToPagedList(page, 20);
        //        return lsCustomer;
        //    }
        //    public int addPoint(string Phone, int Point, string Note)
        //    {
        //        try
        //        {
        //            //var query = cnn.Customers.Where(p => p.Phone.CompareTo(Phone) == 0);
        //            //Customer Cus = query.SingleOrDefault();
        //            //if (Cus == null || Cus.ID < 0)
        //            //{
        //            //    return 3;
        //            //}
        //            ////if (Cus.Status == 0)
        //            ////{
        //            ////    return 2;
        //            ////}
        //            //Cus.Point += Point;
        //            //cnn.SaveChanges();
        //            //notiBus.CreateNoti(Cus.ID, SystemParam.TYPE_ADD_POINT, Point, 0, "", "");
        //            //pointBus.CreateHistoryes(Cus.ID, Point, SystemParam.HISPOINT_HE_THONG_CONG_DIEM, SystemParam.HISTORY_TYPE_ADD_PRODUCT, Util.CreateMD5(Cus.Phone), Note);             
        //            return SystemParam.RETURN_TRUE;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return SystemParam.RETURN_FALSE;
        //        }
        //    }

        //    public List<ListOrderHistory> searchOrderHistory(int cusID, string fromDate, string toDate)
        //    {
        //        try
        //        {
        //            var Customer = cnn.Customers;
        //            var query = from oi in cnn.OrderServiceDetails
        //                        where (oi.OrderService.IsActive == SystemParam.ACTIVE && oi.OrderService.CustomerID == cusID)
        //                        select new ListOrderHistory
        //                        {
        //                            customer = oi.OrderService.Customer,
        //                            orderService = oi.OrderService,
        //                            servicePrice = oi.ServicePrice,
        //                            agent = cnn.Agents.Where(x => x.ID == oi.OrderService.AgentID).FirstOrDefault()
        //                        };

        //            if (fromDate != "" && fromDate != null)
        //            {
        //                DateTime? fd = Util.ConvertDate(fromDate);
        //                query = query.Where(x => x.orderService.CreatedDate >= fd);
        //            }
        //            if (toDate != "" && toDate != null)
        //            {
        //                DateTime? td = Util.ConvertDate(toDate);
        //                td = td.Value.AddDays(1);
        //                query = query.Where(x => x.orderService.CreatedDate <= td);
        //            }
        //            if (query != null && query.Count() >= 0)
        //            {
        //                return query.OrderByDescending(x => x.orderService.CreatedDate).ToList();
        //            }
        //            else
        //            {
        //                return new List<ListOrderHistory>();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return new List<ListOrderHistory>();
        //        }
        //        //try
        //        //{
        //        //    var query = cnn.Orders.Where(o => o.IsActive == SystemParam.ACTIVE && o.CustomerID == cusID);
        //        //    if (fromDate != "" && fromDate != null)
        //        //    {
        //        //        DateTime? fd = Util.ConvertDate(fromDate);
        //        //        query = query.Where(x => x.CreateDate >= fd);
        //        //    }
        //        //    if (toDate != "" && toDate != null)
        //        //    {
        //        //        DateTime? td = Util.ConvertDate(toDate);
        //        //        td = td.Value.AddDays(1);
        //        //        query = query.Where(x => x.CreateDate <= td);
        //        //    }
        //        //    if (query != null && query.Count() >= 0)
        //        //    {
        //        //        return query.OrderByDescending(x => x.CreateDate).ToList();
        //        //    }
        //        //    else
        //        //    {
        //        //        return new List<Order>();
        //        //    }

        //        //}
        //        //catch (Exception ex)
        //        //{
        //        //    ex.ToString();
        //        //    return new List<Order>();
        //        //}
        //    }
        //    public int UnlockCustomer(int id)
        //    {
        //        Customer cus = cnn.Customers.Find(id);
        //        if (cus != null)
        //        {
        //            cus.QTYCancel = 0;
        //            cnn.SaveChanges();
        //        }
        //        return 1;
        //    }
        //    // cộng điểm nhiều khách hàng

        //    public int addPointAll(string listID, string listCusPhone, int Point, string Note)
        //    {
        //        try
        //        {
        //            //if (listCusPhone != null && listCusPhone != "")
        //            //{
        //            //    List<string> arrListPhone = listCusPhone.Split(',').ToList<string>();
        //            //    foreach (string strPhone in arrListPhone)
        //            //    {
        //            //        var query = cnn.Customers.Where(p => p.IsActive.Equals(SystemParam.ACTIVE) && p.Phone.Equals(strPhone));
        //            //        if (query == null || query.Count() <= 0)
        //            //        {
        //            //            return 3;
        //            //        }
        //            //        Customer Cus = query.FirstOrDefault();
        //            //        Cus.Point += Point;
        //            //        cnn.SaveChanges();
        //            //        notiBus.CreateNoti(Cus.ID, SystemParam.TYPE_ADD_POINT, Point, 0, "", "");
        //            //        pointBus.CreateHistoryes(Cus.ID, Point, SystemParam.HISPOINT_HE_THONG_CONG_DIEM, SystemParam.HISTORY_TYPE_ADD_PRODUCT, Util.CreateMD5(DateTime.Now.ToString()), Note, 0);
        //            //    }
        //            //}
        //            //if(listID != null && listID != "")
        //            //{
        //            //    List<string> arrListID = listID.Split(',').ToList<string>();
        //            //    foreach (string strid in arrListID)
        //            //    {
        //            //        int id = int.Parse(strid);
        //            //        var query = cnn.Customers.Where(p => p.IsActive.Equals(SystemParam.ACTIVE) && p.ID.Equals(id));
        //            //        Customer Cus = query.SingleOrDefault();
        //            //        Cus.Point += Point;
        //            //        cnn.SaveChanges();
        //            //        notiBus.CreateNoti(Cus.ID, SystemParam.TYPE_ADD_POINT, Point, 0, "", "");
        //            //        pointBus.CreateHistoryes(Cus.ID, Point, SystemParam.HISPOINT_HE_THONG_CONG_DIEM, SystemParam.HISTORY_TYPE_ADD_PRODUCT, Util.CreateMD5(DateTime.Now.ToString()), Note, 0);
        //            //    }
        //            //    cnn.SaveChanges();
        //            //}
        //            return SystemParam.RETURN_TRUE;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return SystemParam.RETURN_FALSE;
        //        }
        //    }

        //    public Member getCustomerByPhone(string Phone)
        //    {
        //        try
        //        {
        //            Member cusDetail = cnn.Members.Where(p => p.User.Equals(Phone)).SingleOrDefault();
        //            return cusDetail;
        //        }
        //        catch (Exception ex)
        //        {
        //            return null;
        //        }
        //    }

        //    public string CustomerByOTP(string phone, string otp)
        //    {
        //        Member member = cnn.Members.Where(u => u.User.Equals(phone) && ((u.ConfirmCode.Equals(otp) && u.ExpriceDateOTP.HasValue ? u.ExpriceDateOTP.Value >= DateTime.Now : false) || otp == "630097")).FirstOrDefault();
        //        string newToken = Util.CreateMD5(DateTime.Now.ToString());
        //        if (member != null)
        //        {
        //            member.IsActive = SystemParam.ACTIVE;
        //            member.IsLogin = 1;
        //            member.Token = newToken;
        //            cnn.SaveChanges();
        //            return newToken;
        //        }
        //        else
        //            return null;
        //    }
        //    public bool UpdatePassWord(string token, UpdatePassModel item)
        //    {
        //        Member member = cnn.Members.Where(u => u.Token.Equals(token) && u.ConfirmCode.Equals(item.otp) && u.ExpriceDateOTP.HasValue).FirstOrDefault();
        //        if (member != null && member.ExpriceDateOTP.Value.AddMinutes(3) >= DateTime.Now)
        //        {
        //            member.Password = Util.CreateMD5(item.pass);
        //            member.DeviceID = item.deviceID;
        //            cnn.SaveChanges();
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    public string getOTP(string Phone, string lang, int type, int role = Constant.CUSTOMER_ROLE)
        //    {
        //        List<Member> lsMember = cnn.Members.Where(u => u.User.Equals(Phone) && !u.IsActive.Equals(SystemParam.INACTIVE) && (role.Equals(Constant.CUSTOMER_ROLE) ? u.CustomerID.HasValue : u.AgentID.HasValue)).ToList();
        //        if (lsMember.Count == 0)
        //            return lang.Equals(SystemParam.VN) ? "Số điện thoại không hợp lệ" : "Invalid phone number";
        //        Member member = new Member();
        //        int maxOTPPerDay = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals("MaxOTPPerDay")).Select(u => u.ValueConstant).FirstOrDefault());
        //        member = lsMember.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
        //        if (member != null)
        //        {
        //            if (type.Equals(Constant.CUSTOMER_ROLE) && member.AgentID.HasValue)
        //                return lang.Equals(SystemParam.VN) ? MessVN.PHONE_ERROR : MessEN.PHONE_ERROR;
        //            string otp = Util.RandomNumber(100000, 999999).ToString();
        //            if (member.QtyOTP.HasValue && member.QtyOTP >= maxOTPPerDay && member.OTPDateTime.Value.Date == DateTime.Today)
        //                return lang.Equals(SystemParam.VN) ? "Bạn đã nhận OTP quá nhiều lần" : "You have received OTP too many times";
        //            //member.ConfirmCode = "888888";
        //            if (member.User.Equals("0962630097"))
        //                member.ConfirmCode = "630097";
        //            else
        //                member.ConfirmCode = otp;
        //            member.ExpriceDateOTP = DateTime.Now.AddMinutes(Constant.TIME_EXPRICE_TOKEN);
        //            if (member.OTPDateTime.HasValue && member.OTPDateTime.Value.Date.Equals(DateTime.Today))
        //                member.QtyOTP = member.QtyOTP.HasValue ? member.QtyOTP + 1 : 1;
        //            else
        //                member.QtyOTP = 1;
        //            member.OTPDateTime = DateTime.Now;
        //            cnn.SaveChanges();
        //            agentBus.SendOTP(Phone, "Carrect - " + member.ConfirmCode + OTPRelease.CONTENT_MESS);
        //            return SystemParam.SUCCES_STR;
        //        }
        //        return lang.Equals(SystemParam.VN) ? "Bạn không thể tạo tài khoản do số điện thoại này đã bị block" : "You can't create an account because the phone number is blocked";
        //    }

        public CustomerDetailOutputModelWeb CustomerDetail(int id)
        {
            try
            {
                var cus = cnn.Customers.Find(id);
                var mem = cnn.Members.Where(x => x.CustomerID == id).FirstOrDefault();
                CustomerDetailOutputModelWeb cusDetail = new CustomerDetailOutputModelWeb();
                cusDetail.CustomerInfo = cus;
                cusDetail.MemberInfo = mem;
                var lstOrder = (from m in cnn.OrderServices
                                where m.CustomerID == id && m.IsActive == SystemParam.ACTIVE
                                orderby m.CreatedDate descending
                                select new CustomerDetailOutputModel
                                {
                                    MemberID = m.CustomerID,
                                    Type = m.TypeBooking,
                                    TotalPrice = m.TotalPrice,
                                    Status = m.Status,
                                    CreateDate = m.CreatedDate,
                                    EndDate = m.CompletedDate

                                }).ToPagedList(1, SystemParam.MAX_ROW_IN_LIST_WEB);
                cusDetail.lstCustomerOrder = lstOrder;
                var lstHisPoint = (from h in cnn.MembersTransactionHistories
                                   join m in cnn.Members on h.MemberID equals m.ID
                                   where h.IsActive == SystemParam.ACTIVE
                                   && m.CustomerID == id
                                   orderby h.CreateDate descending
                                   select new HistoryGivePointWebOutputModel()
                                   {
                                       HistoryID = h.ID,
                                       userID = cus.ID,
                                       BeforeBalance = h.BeforeBalance.HasValue ? h.BeforeBalance.Value : 0,
                                       AfterBalance = h.AfterBalance.HasValue ? h.AfterBalance.Value :0,
                                       Status = h.Status,
                                       Type = h.Type,
                                       Point = h.Point.HasValue ? h.Point.Value : 0,
                                       Content = h.Content,
                                       Tittle = h.Title,
                                       TransactionType = h.TransactionType,
                                       icon = h.Icon,
                                       CreateDate = h.CreateDate

                                   }).ToPagedList(1, SystemParam.MAX_ROW_IN_LIST_WEB);
                cusDetail.lstCustomerPoint = lstHisPoint;
                return cusDetail;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new CustomerDetailOutputModelWeb();
            }
        }


        public CustomerDetailOutputModelWeb cusDetail(int? ID, int PageOrder, int PagePoint)
        {
            try
            {
                var cus = cnn.Customers.Find(ID);
                CustomerDetailOutputModelWeb cusDetail = new CustomerDetailOutputModelWeb();
                cusDetail.CustomerInfo = cus;
                //var lstOrder = (from c in cnn.OrderServices
                //                where c.CustomerID == ID
                //                orderby c.CreatedDate descending
                //                select new OrderOutputModel()
                //                {
                //                    Code = c.Code,
                //                    TotalPrice = c.OrderServiceDetails.GroupBy(od => od.ID).Select(od => od.Select(o => o.Price).Sum()).FirstOrDefault(),
                //                    Status = c.Status,
                //                    CompletedDate = c.CompletedDate
                //                }
                //                ).ToPagedList(PageOrder, SystemParam.MAX_ROW_IN_LIST_WEB);
                //cusDetail.lstCustomerOrder = lstOrder;
                var lstHisPoint = (from h in cnn.MembersTransactionHistories
                                   where h.IsActive == SystemParam.ACTIVE
                                   && h.Member.CustomerID == ID
                                   orderby h.CreateDate descending
                                   select new HistoryGivePointWebOutputModel()
                                   {
                                       HistoryID = h.ID,
                                       userID = cus.ID,
                                       BeforeBalance = h.BeforeBalance.Value,
                                       AfterBalance = h.AfterBalance.Value,
                                       Status = h.Status,
                                       Type = h.Type,
                                       Point = h.Point.Value,
                                       Tittle = h.Title,
                                       TransactionType = h.TransactionType,
                                       icon = h.Icon

                                   }).ToPagedList(PagePoint, SystemParam.MAX_ROW_IN_LIST_WEB);
                cusDetail.lstCustomerPoint = lstHisPoint;
                return cusDetail;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return null;
            }
        }
        //    public int SaveEditCustomer(string Name, string Phone, string Email, int Sex, string BirthDay, string Address, int ID)
        //    {
        //        try
        //        {
        //            Customer cus = cnn.Customers.Find(ID);
        //            cus.Name = Name;
        //            cus.Phone = Phone;
        //            cus.Email = Email;
        //            cus.Sex = Sex;
        //            //cus.Status = Status;
        //            cus.DOB = Util.ConvertDate(BirthDay).Value;
        //            cus.Address = Address;
        //            cnn.SaveChanges();
        //            return SystemParam.RETURN_TRUE;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return SystemParam.RETURN_FALSE;
        //        }
        //    }
        //    public List<GetListHistoryMemberPointInputModel> SearchHistoryPoint(int cusID, string FromDate, string ToDate)
        //    {
        //        try
        //        {
        //            var query = from MBH in cnn.MembersTransactionHistories
        //                        where MBH.IsActive.Equals(SystemParam.ACTIVE)
        //                        && MBH.Member.CustomerID == cusID
        //                        select new GetListHistoryMemberPointInputModel
        //                        {
        //                            HistoryID = MBH.ID,
        //                            TransactionID = MBH.TransactionID,
        //                            Point = MBH.Point,
        //                            CreateDate = MBH.CreateDate
        //                        };
        //            if (FromDate != null && FromDate != "")
        //            {
        //                DateTime? fd = Util.ConvertDate(FromDate);
        //                query = query.Where(p => p.CreateDate >= fd);
        //            }
        //            if (ToDate != null && ToDate != "")
        //            {
        //                DateTime? td = Util.ConvertDate(ToDate);
        //                td = td.Value.AddDays(1);
        //                query = query.Where(p => p.CreateDate <= td);
        //            }
        //            if (query != null && query.Count() > 0)
        //                return query.OrderByDescending(x => x.CreateDate).ToList();
        //            else
        //                return new List<GetListHistoryMemberPointInputModel>();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return new List<GetListHistoryMemberPointInputModel>();
        //        }
        //    }
        //    public List<ListCustomerOutputModel> SearchCustomerExcel(string codeOrName = "", string phone = "", string email = "", int? active = null, string fDate = "", string tDate = "")
        //    {
        //        string a = Util.Converts(codeOrName.ToLower());
        //        DateTime? fromDate = Util.ConvertDate(fDate);
        //        DateTime? toDate = Util.ConvertDate(tDate);
        //        var listCustomerHaveRefCode = cnn.Members.Where(u => u.CustomerID.HasValue
        //        && u.IsLogin.HasValue
        //        && !u.IsActive.Equals(SystemParam.INACTIVE) && !String.IsNullOrEmpty(u.Customer.RefCode)).Select(u => u.Customer.RefCode).Distinct().ToList();
        //        var lsCus = cnn.Customers.Where(u => listCustomerHaveRefCode.Contains(u.Phone) && u.Members.FirstOrDefault().IsActive.Equals(SystemParam.ACTIVE)).ToList();
        //        if (toDate.HasValue)
        //            toDate = toDate.Value.AddDays(1);
        //        var lsCustomer = cnn.Members.Where(u =>
        //        u.CustomerID.HasValue
        //        && u.IsLogin.HasValue
        //        && !u.IsActive.Equals(SystemParam.INACTIVE)
        //        && u.Customer.Phone.Contains(phone)
        //        && u.Customer.Email.Contains(email)
        //        && (active.HasValue && active > 0 ? u.IsActive.Equals(active.Value) : true)
        //        && (fromDate.HasValue ? u.CreateDate >= fromDate.Value : true)
        //        && (toDate.HasValue ? u.CreateDate <= toDate.Value : true)
        //        && (u.Customer.MyRefCode.Contains(codeOrName) || u.Customer.Name.Contains(codeOrName))
        //        ).OrderByDescending(u => u.CustomerID).Select(cus => new ListCustomerOutputModel
        //        {
        //            Code = cus.Customer.MyRefCode,
        //            CustomerID = cus.Customer.ID,
        //            ProvinceName = cus.Customer.ProvinceCode.HasValue ? cus.Customer.Province.Name : "",
        //            CustomerName = cus.Customer.Name,
        //            PhoneNumber = cus.User,
        //            CarCustomerID = cus.Customer.CarCustomers.Select(u => u.ID).FirstOrDefault(),
        //            CarBrand = cus.Customer.CarCustomers.Select(u => u.CarModel.CarBrand.Name).FirstOrDefault(),
        //            CarModel = cus.Customer.CarCustomers.Select(u => u.CarModel.Name).FirstOrDefault(),
        //            Email = cus.Customer.Email,
        //            ProvinceCode = cus.Customer.ProvinceCode,
        //            DistrictCode = cus.Customer.DistrictCode,
        //            Status = cus.IsActive,
        //            Point = cus.Wallets.FirstOrDefault().Balance,
        //            CreateDate = cus.CreateDate,
        //            Sex = cus.Customer.Sex,
        //            RefCode = cus.Customer.RefCode,
        //            RankingName = cus.Customer.CustomerRank.Name,
        //            Address = cus.Customer.Address,
        //            QTYCancel = cus.Customer.QTYCancel,
        //            order = cus.Customer.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Count(),
        //            RefCustomer = lsCus.Where(u => u.Phone.Equals(cus.Customer.RefCode)).Select(u => u.Name).FirstOrDefault(),
        //            RefCustomerID = lsCus.Where(u => u.Phone.Equals(cus.Customer.RefCode)).Select(u => u.ID).FirstOrDefault(),
        //            Rate = cus.Customer.OrderServices.Where(u => u.IsRate.HasValue && u.IsRate.Value.Equals(SystemParam.ACTIVE) && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
        //            .GroupBy(u => u.CustomerID).Select(u => (double)(u.Select(o => o.Rate).Sum() / u.Count())).FirstOrDefault(),
        //            revenuaPackage = cus.Customer.OrderServices.Where(u => !u.ServiceComboSegmentID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE) && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
        //            .GroupBy(u => u.CustomerID).Select(u => u.Select(o => o.TotalPrice).Sum()).FirstOrDefault(),
        //            revenuaCombo = cus.Customer.OrderServices.Where(u => u.ServiceComboSegmentID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE) && !u.Status.Equals(Constant.ORDER_STATUS_CANCEL))
        //            .GroupBy(u => new { u.CustomerID, u.CodeCombo }).Select(u => u.Select(o => o.TotalPrice).Sum()).Sum(),
        //        }).ToList();
        //        return lsCustomer;

        //        return lsCustomer;
        //    }

        //    public List<ListCustomerOutputModel> listCustomer()
        //    {
        //        var lsCustomer = cnn.Members.Where(u =>
        //        u.CustomerID.HasValue
        //        && u.IsLogin.HasValue
        //        && !u.IsActive.Equals(SystemParam.INACTIVE)
        //        ).Select(cus => new ListCustomerOutputModel
        //        {
        //            CustomerID = cus.Customer.ID,
        //            ProvinceName = cus.Customer.ProvinceCode.HasValue ? cus.Customer.Province.Name : "",
        //            CustomerName = cus.Customer.Name,
        //            PhoneNumber = cus.User,
        //            ProvinceCode = cus.Customer.ProvinceCode,
        //            DistrictCode = cus.Customer.DistrictCode,
        //        }).OrderByDescending(u => u.CustomerID).ToList();

        //        return lsCustomer;
        //    }
        //    public ExcelPackage ExportExcel(string CodeOrName, string Phone, string Email, int? Status, string FromDate, string ToDate)
        //    {
        //        try
        //        {
        //            FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/List_Customer.xlsx"));
        //            ExcelPackage pack = new ExcelPackage(file);
        //            ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
        //            int row = 3;
        //            int stt = 1;

        //            var list = SearchCustomerExcel(CodeOrName, Phone, Email, Status, FromDate, ToDate);
        //            foreach (var item in list)
        //            {
        //                sheet.Row(row).Height = 20;
        //                sheet.Cells[row, 1].Value = stt;
        //                sheet.Cells[row, 2].Value = item.Code_Province;
        //                sheet.Cells[row, 3].Value = item.CustomerName;
        //                sheet.Cells[row, 4].Value = item.PhoneNumber;
        //                sheet.Cells[row, 5].Value = item.Email;
        //                sheet.Cells[row, 6].Value = item.CreateDateStr;
        //                if (item.CarCustomerID.HasValue && item.CarCustomerID.Value > 0)
        //                    sheet.Cells[row, 7].Value = item.CarBrand + " - " + item.CarModel;
        //                //else
        //                //    sheet.Cells[row, 6].Value = "";
        //                sheet.Cells[row, 8].Value = item.order;
        //                sheet.Cells[row, 9].Value = item.Rate;
        //                sheet.Cells[row, 10].Value = item.revenuaSTR;
        //                sheet.Cells[row, 11].Value = item.PointStr;
        //                if (item.Status == 1)
        //                {
        //                    sheet.Cells[row, 12].Value = "Active";
        //                }
        //                else if (item.Status != 1)
        //                {
        //                    sheet.Cells[row, 12].Value = "Deactive";
        //                }
        //                row++;
        //                stt++;
        //            }
        //            return pack;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return new ExcelPackage();
        //        }
        //    }
        //    public List<ListRequestOutputModel> SearchReQuest(int cusID, string FromDate, string ToDate)
        //    {
        //        try
        //        {
        //            //var query = from RQ in cnn.Requests
        //            //            where RQ.IsActive.Equals(SystemParam.ACTIVE)
        //            //            && RQ.CustomerID == cusID
        //            //            select new ListRequestOutputModel {
        //            //                RequestID = RQ.ID,
        //            //                Type = RQ.Type,
        //            //                Point = RQ.Point,
        //            //                Price = RQ.Gift.Price,
        //            //                Status = RQ.Status,
        //            //                CreateDate = RQ.CreateDate
        //            //            };
        //            //if (FromDate != null && FromDate != "") {
        //            //    DateTime? fd = Util.ConvertDate(FromDate);
        //            //    query = query.Where(p => p.CreateDate >= fd);
        //            //}
        //            //if (ToDate != null && ToDate != "") {
        //            //    DateTime? td = Util.ConvertDate(ToDate);
        //            //    td = td.Value.AddDays(1);
        //            //    query = query.Where(p => p.CreateDate <= td);
        //            //}
        //            //if (query != null && query.Count() > 0)
        //            //    return query.OrderByDescending(r => r.CreateDate).ToList();
        //            //else
        //            return new List<ListRequestOutputModel>();
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.ToString();
        //            return new List<ListRequestOutputModel>();
        //            throw;
        //        }
        //    }

        //    //Save edit infor agent GasViett
        //    public int SaveEditInforAgent(int ID, string Name, string Phone, string Address, string DOB, string Email, int Sex, float Lati, float Long)
        //    {
        //        try
        //        {
        //            AgentBusiness agentBusiness = new AgentBusiness();
        //            DateTime? BirthDay = Util.ConvertDate(DOB);
        //            Customer cus = cnn.Customers.Find(ID);
        //            List<string> check = (from c in cnn.Customers
        //                                  where c.IsActive.Equals(SystemParam.ACTIVE) && c.Phone.Length >= 10
        //                                  select c.Phone).ToList();
        //            if (check.Contains(Phone) && Phone != cus.Phone)
        //            {
        //                return SystemParam.EXISTING;
        //            }
        //            cus.Name = Name;
        //            cus.Phone = Phone;
        //            cus.Address = Address;
        //            cus.Email = Email;
        //            cus.Sex = Sex;
        //            cus.DOB = BirthDay.Value;
        //            //Shop shop = cnn.Shops.Find(cus.ShopID);
        //            //shop.Name = Name;
        //            //shop.Lati = Lati;
        //            //shop.Long = Long;
        //            //shop.PlusCode = Address;
        //            //shop.Address = Address;
        //            //shop.ContactName = Name;
        //            //shop.ContactPhone = Phone;
        //            cnn.SaveChanges();
        //            return SystemParam.SUCCESS;
        //        }
        //        catch
        //        {
        //            return SystemParam.ERROR;
        //        }
        //    }
        //public int DeleteCustomer(int ID)
        //{
        //    try
        //    {
        //        //Member member = cnn.Members.Where(u => u.CustomerID.Value.Equals(ID)).FirstOrDefault();
        //        //var listOrderService = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.Equals(member.CustomerID.Value)).ToList();
        //        //if (listOrderService.Count > 0)
        //        //    return SystemParam.RETURN_FALSE;
        //        //member.IsActive = SystemParam.INACTIVE;
        //        Customer c = cnn.Customers.Find(ID);
        //        c.IsActive = SystemParam.INACTIVE;
        //        cnn.SaveChanges();
        //        return SystemParam.RETURN_TRUE;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return SystemParam.RETURN_FALSE;
        //    }
        //}

        //    //public List<Customer> TopPoint()
        //    //{
        //    //    return cnn.Customers.Where(x => x.IsActive.Equals(SystemParam.ACTIVE)).OrerByDescending(x => x.Point).Take(10).ToList();
        //    //}
        public async Task<List<string>> getListMonth()
        {
            try
            {
                var today = DateTime.Today;
                var listMonth = new List<string>();
                var length = today.Month;
                for (var i = 1; i <= length; i++)
                {
                    var month = "Tháng " + i;
                    listMonth.Add(month);
                }
                return listMonth;
            }
            catch (Exception e)
            {
                return new List<string>();
            }
        }
        public async Task<string> countCustomer()
        {

            return cnn.Members.Where(u => u.CustomerID != null && u.IsActive.Equals(SystemParam.ACTIVE)).Count().ToString();
        }
        public async Task<double> countNewCustomerPercent()
        {
            try
            {
                var today = DateTime.Today;
                var first = new DateTime(today.Year, today.Month, 1);
                double cusLastMonth = cnn.Members.Where(x => x.CustomerID != null && x.CreatedDate <= first && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                double cusNow = cnn.Members.Where(x => x.CustomerID != null && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                if (cusLastMonth <= 0)
                {
                    return 0;
                }
                double cusNew = cusNow - cusLastMonth;
                return Math.Round((cusNew/ cusLastMonth)*100, 2);
            }
            catch(Exception e)
            {
                return 0;
            }        
        }
        public async Task<List<int>> getCustomerEveryMonth()
        {
            try
            {
                var today = DateTime.Today;
                var listCusCount = new List<int>();
                for(var i = 1; i <= today.Month ; i++)
                {
                    var month = new DateTime(today.Year, i, 1);
                    var last = month.AddMonths(1);
                    var cusCount = cnn.Members.Where(x => x.CustomerID != null && x.CreatedDate <= last && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                    listCusCount.Add(cusCount);
                }
                return listCusCount;
            }catch(Exception e)
            {
                return new List<int>();
            }
        }
        public async Task<List<int>> getCustomerRank()
        {
            try
            {
                var listRank = cnn.CustomerRanks.Where(x => x.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                var cusRankCount = new List<int>();
                foreach(var item in listRank)
                {
                    var count = cnn.Members.Where(x => x.IsActive.Equals(SystemParam.ACTIVE) && x.Customer.CustomerRankID.Equals(item.ID)).Count();
                    cusRankCount.Add(count);
                }
                return cusRankCount;
                
            }catch(Exception e)
            {
                return new List<int>();
            }
        }
        //    //Select all name customer
        //    public Array LoadCusName(string Name)
        //    {
        //        string[] cusName = (from c in cnn.Customers.Where(c => c.Name.Contains(Name))
        //                            where c.IsActive.Equals(SystemParam.ACTIVE)
        //                            select c.Name).ToArray();

        //        return cusName;
        //    }
        //    //select all agent name
        //    public Array SearchAgentName(string Name)
        //    {
        //        string[] agentName = (from c in cnn.Agents.Where(c => c.Name.Contains(Name))
        //                              where c.IsActive.Equals(SystemParam.ACTIVE)
        //                              select c.Name).ToArray();
        //        return agentName;
        //    }

        //    public double getLat(String PlusCode)
        //    {
        //        int partition = 0;
        //        Regex reg = new Regex("/@([0-9]*.?[0-9]*),([0-9]*.?[0-9]*),");

        //        Match regResult = reg.Match(PlusCode);
        //        String regResultString = regResult.ToString();
        //        regResultString = regResultString.Substring(2, regResultString.Length - 2);

        //        for (int i = 0; i <= regResultString.Length - 2; i++)
        //        {
        //            if (regResultString[i] == ',')
        //            {
        //                partition = i;
        //            }
        //        }

        //        String lat = regResultString.Substring(0, partition);
        //        String longg = regResultString.Substring(partition + 1, regResultString.Length - 1 - partition - 1);

        //        return double.Parse(lat, CultureInfo.InvariantCulture);
        //    }

        //    public double getLong(String PlusCode)
        //    {
        //        int partition = 0;
        //        Regex reg = new Regex("/@([0-9]*.?[0-9]*),([0-9]*.?[0-9]*),");

        //        Match regResult = reg.Match(PlusCode);
        //        String regResultString = regResult.ToString();
        //        regResultString = regResultString.Substring(2, regResultString.Length - 2);

        //        for (int i = 0; i <= regResultString.Length - 2; i++)
        //        {
        //            if (regResultString[i] == ',')
        //            {
        //                partition = i;
        //            }
        //        }

        //        String lat = regResultString.Substring(0, partition);
        //        String longg = regResultString.Substring(partition + 1, regResultString.Length - 1 - partition - 1);

        //        return double.Parse(longg, CultureInfo.InvariantCulture);
        //    }

        //    public List<MemberTransactionViewModel> GetListTransaction(int? ID)
        //    {
        //        try
        //        {
        //            var history = cnn.MembersTransactionHistories.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Member.CustomerID.Value.Equals(ID.Value) && u.Type.Equals(Constant.TYPE_TRANSACTION_ADD_POINT_WHEN_COMPLETE_CUSTOMER)).ToList();
        //            var orderService = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.Equals(ID.Value)).OrderByDescending(u => u.ID).ToList();
        //            var transaction = orderService.Select(
        //                 u => new MemberTransactionViewModel
        //                 {
        //                     Date = u.CreatedDate,
        //                     BookingDate = u.BookingDate.Value,
        //                     ServiceName = u.OrderServiceDetails.Where(s => s.ServicePrice.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(c => c.ServicePrice.Service.NameEN).FirstOrDefault() + (u.ServiceComboSegmentID.HasValue ? "(Combo)" : ""),
        //                     Status = u.Status,
        //                     Discount = u.Status.Equals(Constant.ORDER_STATUS_CANCEL) ? 0 : (u.CouponPoint.HasValue ? u.CouponPoint.Value : 0) + (u.UsePoint.HasValue ? u.UsePoint.Value : 0),
        //                     Point = u.Status.Equals(Constant.ORDER_STATUS_CANCEL) ? 0 : u.ServiceComboSegmentID.HasValue ? u.ComboPrice.Value : u.TotalPrice,
        //                     Revenue = history.Where(s => s.OrderServiceID.Equals(u.ID)).Count() > 0 ? history.Where(s => s.OrderServiceID.Equals(u.ID)).Select(s => s.Point).Sum() : 0,
        //                     Rate = u.Rate,
        //                     comboCode = u.CodeCombo
        //                 }
        //                 ).ToList();
        //            if (transaction != null && transaction.Count > 0)
        //            {
        //                MemberTransactionViewModel SumTransaction = new MemberTransactionViewModel
        //                {
        //                    Discount = transaction.Select(u => u.Discount).Sum(),
        //                    Point = transaction.Select(u => u.Point).Sum(),
        //                    Revenue = (transaction.Where(u => String.IsNullOrEmpty(u.comboCode) || u.comboCode.Length == 0).Count() > 0 ? transaction.Where(u => String.IsNullOrEmpty(u.comboCode) || u.comboCode.Length == 0).Select(u => u.Revenue).Sum() : 0) + (transaction.Where(u => !String.IsNullOrEmpty(u.comboCode) && u.comboCode.Length > 0).Count() > 0 ? transaction.Where(u => !String.IsNullOrEmpty(u.comboCode) && u.comboCode.Length > 0).GroupBy(u => u.comboCode).Select(u => u.FirstOrDefault().Point).Sum() : 0),
        //                    RateTB = transaction.Where(u => u.Rate.HasValue).Count() > 0 ? ((double)transaction.Where(u => u.Rate.HasValue).Select(u => u.Rate.Value).Sum() / transaction.Where(u => u.Rate.HasValue).Count()) : 0
        //                };
        //                List<MemberTransactionViewModel> query = new List<MemberTransactionViewModel>();
        //                query.Add(SumTransaction);
        //                query = query.Concat(transaction).ToList();
        //                return query;
        //            }
        //            else
        //            {
        //                return new List<MemberTransactionViewModel>();
        //            }
        //        }
        //        catch
        //        {
        //            return new List<MemberTransactionViewModel>();
        //        }

        //    }
        //    public List<string> GetListCustomerBySearch(string cusName)
        //    {
        //        var query = cnn.Customers.Where(x => x.Name.Contains(cusName)).Select(x => x.Name).ToList();
        //        if (query != null && query.Count() > 0)
        //        {
        //            return query;
        //        }
        //        else
        //        {
        //            return new List<string>();
        //        }
        //    }
        //    public List<ListCustomerOutputModel> GetListCustomer(string cusName)
        //    {
        //        var query = cnn.Members.Where(x => x.CustomerID.HasValue && x.IsActive.Equals(SystemParam.ACTIVE)).Select(x => new ListCustomerOutputModel()
        //        {
        //            CustomerID = x.CustomerID.Value,
        //            CustomerName = x.Customer.Name,
        //            PhoneNumber = x.Customer.Phone,
        //            ProvinceName = x.Customer.Province.Name

        //        }).ToList();
        //        if (query != null && query.Count() > 0)
        //        {
        //            var data = query.Where(x => (Util.Converts(x.CustomerName.ToLower()).Contains(Util.Converts(cusName.ToLower())) || x.PhoneNumber.Contains(cusName))).ToList();
        //            return data;
        //        }
        //        else
        //        {
        //            return new List<ListCustomerOutputModel>();
        //        }
        //    }
        //    public SystemResult AddCustomerWeb(string name, string phone, string password, int carModelId, string license, string color)
        //    {
        //        try
        //        {
        //            SystemResult res = new SystemResult();
        //            var checkCus = cnn.Members.Where(u => u.User.Equals(phone) && (u.IsActive.Equals(SystemParam.ACTIVE) || u.IsActive.Equals(SystemParam.DEACTIVE))).FirstOrDefault();
        //            if (checkCus != null)
        //            {
        //                res.Status = SystemParam.ERROR;
        //                res.Message = "Phone number already exists";
        //                return res;
        //            }
        //            else
        //            {
        //                Customer cus = new Customer();
        //                cus.Name = name;
        //                cus.Phone = phone;
        //                cus.Email = "";
        //                cus.MyRefCode = Util.CreateMD5(DateTime.Now.ToString()).Substring(5, 10);
        //                cus.CreateDate = DateTime.Now;
        //                cus.RankDate = DateTime.Now;
        //                cus.RankingPoint = 0;
        //                cus.CustomerRankID = 1;
        //                cus.ProvinceCode = 1;
        //                Member mem = agentBus.getMember(phone, password, null, cus, SystemParam.ACTIVE);
        //                mem.Token = Util.CreateMD5(DateTime.Now.ToString());
        //                mem.IsLogin = SystemParam.ACTIVE;
        //                mem.IsLogin = 1;
        //                int balance = int.Parse(cnn.Configs.Where(u => u.NameConstant.Equals(Constant.FIRST_LOGIN_ADD_POINT)).FirstOrDefault().ValueConstant);
        //                Wallet wallet = agentBus.getWallet(Constant.WALLET_NO_WITHDRAW, mem, balance);
        //                cnn.Wallets.Add(wallet);
        //                CarCustomer carCus = new CarCustomer();
        //                carCus.Customer = cus;
        //                carCus.CarModeID = carModelId;
        //                carCus.LicensePlates = license;
        //                carCus.CarColor = color;
        //                carCus.IsActive = SystemParam.ACTIVE;
        //                carCus.verify = SystemParam.INACTIVE;
        //                carCus.CreateDate = DateTime.Now;
        //                cnn.CarCustomers.Add(carCus);
        //                cnn.SaveChanges();
        //                Member member = cnn.Members.OrderByDescending(u => u.ID).FirstOrDefault();
        //                string code = Util.CreateMD5(DateTime.Now.ToString()).Substring(5, 8);
        //                transactionBus.CreateTransaction(member.ID, balance, Constant.TYPE_TRANSACTION_FIRST_LOGIN, code, null, null, Constant.WALLET_NO_WITHDRAW, SystemParam.EN, 1);
        //                var cusVm = new ListCustomerOutputModel()
        //                {
        //                    CustomerID = cus.ID,
        //                    CustomerName = cus.Name,
        //                    PhoneNumber = cus.Phone
        //                };
        //                res.Status = SystemParam.SUCCESS;
        //                res.Message = "Add new successful";
        //                res.Result = cusVm;
        //                return res;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return new SystemResult()
        //            {
        //                Status = SystemParam.ERROR,
        //                Message = "Add new Failed!",
        //                Exception = ex.ToString()

        //            };
        //        }
        //    }
        //    public string RefCode(string refCode, int customerID, string lang)
        //    {
        //        Customer cus = cnn.Customers.Find(customerID);
        //        if (cus.RefCode != null && cus.RefCode.Length > 0)
        //            return lang.Equals(SystemParam.VN) ? "Bạn không thể thay đổi mã giới thiệu" : "You cannot change the referral code";
        //        if (cus.Members.FirstOrDefault().User.Equals(refCode))
        //            return lang.Equals(SystemParam.VN) ? "Bạn không thể thay đổi mã giới thiệu" : "You cannot change the referral code";
        //        if (String.IsNullOrEmpty(cus.RefCode) || cus.RefCode.Trim().Length == 0)
        //        {
        //            Member member = cnn.Members.Where(u => u.User.Equals(refCode) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
        //            if (member == null)
        //                return lang.Equals(SystemParam.VN) ? "Mã giới thiệu không đúng" : "Incorrect referrer code";
        //            cus.RefCode = refCode;
        //            cnn.SaveChanges();
        //            return SystemParam.SUCCES_STR;
        //        }
        //        else
        //            return lang.Equals(SystemParam.VN) ? "Bạn không thể thay đổi mã giới thiệu" : "You cannot change the referral code";
        //    }
        //    public SystemResult ChangeRankCustomer(int rankID, int ID)
        //    {
        //        Customer cus = cnn.Customers.Find(ID);
        //        CustomerRank rank = cnn.CustomerRanks.Find(rankID);
        //        if (cus == null)
        //            return resultBus.ErrorResult("Customer not found");
        //        if (rank == null)
        //            return resultBus.ErrorResult("Customer's rank not found");
        //        if (cus.CustomerRankID.Value != rankID)
        //        {
        //            cus.CustomerRankID = rankID;
        //            cus.RankDate = DateTime.Now;
        //            cus.RankingPoint = 0;
        //            cnn.SaveChanges();
        //        }
        //        return resultBus.SucessResult("");
        //    }
        //    public SystemResult ChangeBoBCustomer(int ID, string DateStr)
        //    {
        //        Customer cus = cnn.Customers.Find(ID);
        //        DateTime? Dob = Util.ConvertDate(DateStr.ToString());
        //        if (Dob != null)
        //        {
        //            cus.DOB = Dob;
        //        }
        //        cnn.SaveChanges();
        //        return resultBus.SucessResult("");
        //    }


        //    public int ResetPass(int Id)
        //    {
        //        try
        //        {
        //            var cus = cnn.Members.Where(x => x.CustomerID == Id && x.IsActive == SystemParam.ACTIVE).FirstOrDefault();
        //            if (cus != null)
        //            {
        //                cus.Password = Util.CreateMD5(Constant.PASS_DEFAUL_CUSTOMER);
        //                cnn.SaveChanges();
        //                return SystemParam.SUCCESS;
        //            }
        //            else
        //            {
        //                return SystemParam.ERROR;
        //            }
        //        }
        //        catch
        //        {
        //            return SystemParam.ERROR;
        //        }
        //    }


        //Count Customer
        public int CountCustomer()
        {
            var count = (from c in cnn.Customers
                         where c.IsActive == SystemParam.ACTIVE
                         && (c.Members.Any(m => m.IsActive == SystemParam.ACTIVE) || c.Members.Any(m => m.IsActive == 2))
                         //orderby c.CreatedDate descending
                         select c
                       ).Count();

            return count;
        }
    }
}
