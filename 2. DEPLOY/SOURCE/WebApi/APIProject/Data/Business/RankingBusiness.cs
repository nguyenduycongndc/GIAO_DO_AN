using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class RankingBusiness : GenericBusiness
    {
        public RankingBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }
        TransactionHistoryBusiness tranbus = new TransactionHistoryBusiness();
        NotifyBusiness notibus = new NotifyBusiness();
        public object GetRankCustomer(string lang)
        {
            var RankCustomer = cnn.CustomerRanks.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderBy(u => u.Level).Select(u => new
            {
                ID = u.ID,
                Name = u.Name,
                Incentives = lang.Equals(SystemParam.VN) ? u.DescriptionVi : u.DescriptionEn,
                Policy = lang.Equals(SystemParam.VN) ? u.policyVN : u.Policy,
                MinPoint = u.MinPoint,
                MaxPoint = u.MaxPoint,
                Level = u.Level
            }).ToList();
            return RankCustomer;
        }

        public void CreateRankHistory(int point, int customerID, int type, string lang)
        {
            Customer customer = cnn.Customers.Find(customerID);
            if (customer != null)
            {

                int totalPoint = customer.RankingPoint.HasValue ? customer.RankingPoint.Value : 0;
                if (type.Equals(TypeTransaction.Subtract))
                {
                    if (totalPoint >= point)
                        totalPoint -= point;
                    else
                        totalPoint = 0;
                }
                else
                    totalPoint += point;
                customer.RankingPoint = totalPoint;
                int rankID = CheckUpdateRank(customer, totalPoint);
                if (rankID > 0)
                {
                    customer.CustomerRankID = rankID;
                    customer.RankDate = DateTime.Now;
                    customer.RankingPoint = 0;
                    totalPoint = 0;
                    GiftLevelUpRank(customer, rankID, lang);
                }
                HistoryRankingPoint points = new HistoryRankingPoint
                {
                    CreateDate = DateTime.Now,
                    CustomerID = customerID,
                    IsActive = SystemParam.ACTIVE,
                    Point = point,
                    TotalPoint = totalPoint,
                    Type = type,
                };
                cnn.HistoryRankingPoints.Add(points);
                cnn.SaveChanges();
            }
        }
        public int CheckUpdateRank(Customer customer, int totalPoint)
        {
            int level = customer.CustomerRank.Level.Value + 1;
            CustomerRank rank = cnn.CustomerRanks.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Level.Value.Equals(level)).FirstOrDefault();
            if (rank != null)
            {
                if (totalPoint >= rank.MinPoint)
                {
                    return rank.ID;
                }
            }
            return 0;
        }

        public void CheckDownRank(int customerID)
        {
            Customer cus = cnn.Customers.Find(customerID);
            string lang = String.IsNullOrEmpty(cus.Members.FirstOrDefault().Lang) ? SystemParam.VN : cus.Members.FirstOrDefault().Lang;
            if (cus.RankDate.HasValue && cus.RankDate.Value.AddYears(1) < DateTime.Now)
            {
                List<CustomerRank> LsRank = cnn.CustomerRanks.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(u => u.Level.Value).ToList();
                int? totalPoint = cus.RankingPoint;
                if (!totalPoint.HasValue)
                {
                    cus.RankDate = DateTime.Now;
                    cus.CustomerRankID = 1;
                }
                else
                {
                    CustomerRank myRank = LsRank.Where(u => u.MinPoint <= totalPoint.Value && (u.MaxPoint.HasValue ? u.MaxPoint.Value >= totalPoint.Value : true)).FirstOrDefault();
                    if (myRank.Level.Value < cus.CustomerRank.Level.Value)
                    {
                        string content = lang.Equals(SystemParam.VN) ? "Bậc của bạn đã được cập nhật thành " + myRank.Name : "Your rank has been updated to " + myRank.Name;
                        notibus.CreateNoti(cus.Members.FirstOrDefault().ID, Constant.NOTI_RANK, null, null, null, lang, content, true);
                    }
                    if (myRank != null)
                    {
                        cus.RankDate = DateTime.Now;
                        cus.CustomerRankID = myRank.ID;
                    }
                }
                cus.RankingPoint = 0;
                cnn.SaveChanges();
            }
        }

        public void GiftLevelUpRank(Customer customer, int rankID, string lang)
        {
            Wallet wallet = cnn.Wallets.Where(u => u.Member.CustomerID.Value.Equals(customer.ID)).FirstOrDefault();
            CustomerRank rank = cnn.CustomerRanks.Find(rankID);
            var config = cnn.Configs.ToList();
            int ExpriDayUpRankGift = int.Parse(config.Where(u => u.NameConstant.Equals("ExpriDayUpRankGift")).FirstOrDefault().ValueConstant);
            if (rank.PointBonus.HasValue && rank.PointBonus.Value > 0)
            {
                string code = Util.CreateMD5(DateTime.Now.ToString() + wallet.ID.ToString()).Substring(0, 8);
                tranbus.CreateTransaction(wallet.MemberID, rank.PointBonus.Value, Constant.TYPE_TRANSACTION_UPRANK, code, null, null, wallet.TYPE, lang, 1, "", null, rank.Name);
            }
            List<CustomerRankServiceBonu> lsService = rank.CustomerRankServiceBonus.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals((int)TypeServiceBonus.Extra)).ToList();
            if (lsService.Count > 0)
            {
                List<ServiceBonusCustomer> listServiceBonus = lsService.Select(u => new ServiceBonusCustomer
                {
                    RankID = rank.ID,
                    CustomerID = customer.ID,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now.Date,
                    Type = u.Type,
                    ExpriceTime = DateTime.Now.AddDays(ExpriDayUpRankGift + 1).Date,
                    ServiceID = u.ServiceID,
                    Status = 0
                }).ToList();
                cnn.ServiceBonusCustomers.AddRange(listServiceBonus);
                cnn.SaveChanges();
            }
            string content = customer.Members.FirstOrDefault().Lang == SystemParam.VN ? "Chúc mừng bạn đã được tăng lên bậc " + rank.Name + ". Cùng xem qua ưu đãi bạn nhận được nhé " : "Congratulations your rank is up to " + rank.Name + ". Let's see your gifts";
            notibus.CreateNoti(wallet.MemberID, Constant.NOTI_RANK, null, null, null, lang, content, true);
        }
        public List<RankingViewModel> Search(string name, string fromDate, string toDate)
        {
            DateTime? startDate = Util.ConvertDate(fromDate);
            DateTime? endDate = Util.ConvertDate(toDate);
            if (endDate != null)
                endDate = endDate.Value.AddDays(1);
            var query = (from r in cnn.CustomerRanks
                         where r.IsActive != SystemParam.INACTIVE
                         && (startDate.HasValue ? r.CreateDate >= startDate.Value : true)
                         && (endDate.HasValue ? r.CreateDate <= endDate.Value : true)
                         select new RankingViewModel
                         {
                             ID = r.ID,
                             Name = r.Name,
                             MinPoint = r.MinPoint,
                             MaxPoint = r.MaxPoint,
                             Description = r.DescriptionEn,
                             Status = r.IsActive,
                             CreatedDate = r.CreateDate
                         }).ToList();
            query = query.Where(x => Util.Converts(x.Name.ToLower()).Contains(Util.Converts(name.ToLower()))).ToList();
            return query;
        }
        public RankingDetailViewModel Detail(int ID)
        {
            var rank = cnn.CustomerRanks.Find(ID);

            RankingDetailViewModel query = new RankingDetailViewModel
            {
                ID = rank.ID,
                BirthDayProfit = rank.ProfitExtraBirthDay.Value,
                DescriptionEN = rank.DescriptionEn,
                DescriptionVI = rank.DescriptionVi,
                Level = rank.Level.Value,
                MaxPoint = rank.MaxPoint,
                MinPoint = rank.MinPoint,
                TitleVi = rank.TitleVi,
                TitleEn = rank.TitleEn,
                Name = rank.Name,
                ortherGift = rank.OrtherGift,
                PolicyEN = rank.Policy,
                PolicyVN = rank.policyVN,
                Status = rank.IsActive,
                PointBonus = rank.PointBonus.HasValue ? rank.PointBonus.Value : 0,
                profitCash = rank.ProfitCash,
                profitVnpay = rank.ProfitVPN,
                lsServiceBirthDay = ListServiceBonus(rank, (int)TypeServiceBonus.BirthDay),
                lsServiceBonus = ListServiceBonus(rank, (int)TypeServiceBonus.Extra),
            };
            return query;
        }
        public List<ServiceBonusFree> ListServiceBonus(CustomerRank rank, int type)
        {
            return rank.CustomerRankServiceBonus.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(type)).GroupBy(u => u.ServiceID).Select(u => new ServiceBonusFree
            {
                ID = u.Max(s => s.ID),
                ServiceID = u.FirstOrDefault().ServiceID,
                Name = u.FirstOrDefault().Service.NameEN,
                Qty = u.Count()
            }).ToList();
        }

        public SystemResult Edit(RankingDetailViewModel item)
        {
            var lsRanking = cnn.CustomerRanks.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            var rank = lsRanking.Where(u => u.ID.Equals(item.ID)).FirstOrDefault();
            if (rank == null || rank.IsActive.Equals(SystemParam.INACTIVE))
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            var rankUper = lsRanking.Where(u => u.Level.Value.Equals(rank.Level.Value + 1)).FirstOrDefault();
            var rankLower = lsRanking.Where(u => u.Level.Value.Equals(rank.Level.Value - 1)).FirstOrDefault();
            rank.ProfitCash = item.profitCash;
            rank.ProfitVPN = item.profitVnpay;
            rank.ProfitExtraBirthDay = item.BirthDayProfit;
            rank.OrtherGift = item.ortherGift;
            if (rankUper != null)
            {
                if (rankUper.MaxPoint.HasValue && item.MaxPoint > rankUper.MaxPoint)
                    return resultBus.ErrorResult(" Max point cannot be greater than " + String.Format("{0:n0}", rankUper.MaxPoint.Value));
                if (rankLower != null && item.MaxPoint < rankLower.MinPoint)
                    return resultBus.ErrorResult(" Max point must be greater than  " + String.Format("{0:n0}", rankLower.MinPoint));
                rank.MaxPoint = item.MaxPoint;
                rankUper.MinPoint = item.MaxPoint.Value + 1;
            }
            else
                rank.MaxPoint = null;
            rank.DescriptionEn = item.DescriptionEN;
            rank.DescriptionVi = item.DescriptionVI;
            rank.TitleEn = item.TitleEn;
            rank.TitleVi = item.TitleVi;
            rank.Policy = item.PolicyEN;
            rank.policyVN = item.PolicyVN;
            rank.Name = item.Name;
            rank.PointBonus = item.PointBonus;
            List<ServiceBonusFree> lsServiceBirthDay = ListServiceBonus(rank, (int)TypeServiceBonus.BirthDay);
            List<ServiceBonusFree> lsServiceBonus = ListServiceBonus(rank, (int)TypeServiceBonus.Extra);
            cnn.SaveChanges();
            if (item.lsServiceBirthDay != null && item.lsServiceBirthDay.Count > 0)
                Update(lsServiceBirthDay, (int)TypeServiceBonus.BirthDay, item.ID, item.lsServiceBirthDay);
            if (item.lsServiceBirthDay == null)
            {
                Update(lsServiceBirthDay, (int)TypeServiceBonus.BirthDay, item.ID);
            }
            if (item.lsServiceBonus != null && item.lsServiceBonus.Count > 0)
            {
                Update(lsServiceBonus, (int)TypeServiceBonus.Extra, item.ID, item.lsServiceBonus);
            }
            if (item.lsServiceBonus == null)
            {
                Update(lsServiceBirthDay, (int)TypeServiceBonus.Extra, item.ID);
            }
            return resultBus.SucessResult(true);
        }
        public void Update(List<ServiceBonusFree> data, int type, int ID, List<ServiceBonusFree> input = null)
        {
            if (input == null)
            {
                input = new List<ServiceBonusFree>();
            }
            List<CustomerRankServiceBonu> output = new List<CustomerRankServiceBonu>();
            List<CustomerRankServiceBonu> lsServiceBonus = cnn.CustomerRankServiceBonus.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.RankID.Equals(ID) && u.Type.Equals(type)).ToList();
            List<ServiceBonusFree> news = input.Where(u => u.Qty > data.Where(s => s.ServiceID.Equals(u.ServiceID)).Select(s => s.Qty).FirstOrDefault()).ToList();
            // thêm mới và só lượng
            foreach (ServiceBonusFree server in news)
            {
                ServiceBonusFree haved = data.Where(u => u.ServiceID.Equals(server.ServiceID)).FirstOrDefault();
                int count = server.Qty;
                if (haved != null)
                    count = server.Qty - haved.Qty;
                if (output.Count == 0)
                    output = CreateServiceBonus(server, ID, type, count);
                else
                {
                    List<CustomerRankServiceBonu> query = CreateServiceBonus(server, ID, type, count);
                    output = output.Concat(query).ToList();
                }
            }
            cnn.CustomerRankServiceBonus.AddRange(output);
            // List cần xoá và số lượng
            List<int> listInt = input.Select(s => s.ServiceID).ToList();
            List<ServiceBonusFree> ListServiceremove = data.Where(u => !listInt.Contains(u.ID) || u.Qty > data.Where(s => s.ServiceID.Equals(u.ServiceID)).Select(s => s.Qty).FirstOrDefault()).ToList();
            foreach (ServiceBonusFree server in ListServiceremove)
            {
                ServiceBonusFree haved = input.Where(u => u.ServiceID.Equals(server.ServiceID)).FirstOrDefault();
                int count = server.Qty;
                if (haved != null)
                    count = server.Qty - haved.Qty;
                RemoveService(lsServiceBonus, server.ServiceID, count);
            }
            cnn.SaveChanges();
        }
        public List<CustomerRankServiceBonu> CreateServiceBonus(ServiceBonusFree server, int ID, int type, int count)
        {
            List<CustomerRankServiceBonu> output = new List<CustomerRankServiceBonu>();
            for (int i = 0; i < count; i++)
            {
                CustomerRankServiceBonu query = new CustomerRankServiceBonu
                {
                    RankID = ID,
                    ServiceID = server.ServiceID,
                    Type = type,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now
                };
                output.Add(query);
            }
            return output;
        }

        public void RemoveService(List<CustomerRankServiceBonu> ListService, int serviceID, int count)
        {
            List<CustomerRankServiceBonu> listRemove = ListService.Where(u => u.ServiceID.Equals(serviceID)).OrderByDescending(u => u.ID).Take(count).ToList();
            foreach (CustomerRankServiceBonu service in listRemove)
            {
                service.IsActive = 0;
            }
        }

    }
}
