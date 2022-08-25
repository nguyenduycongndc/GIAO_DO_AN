using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using PagedList.Mvc;

namespace Data.Business
{
    public class CouponBusiness : GenericBusiness
    {
        public CouponBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }
        //public IPagedList<ListCouponModel> Search(int Page)
        //{
        //    try
        //    {
        //        var lst = (from c in cnn.Coupons
        //                   where c.IsActive == SystemParam.ACTIVE
        //                   orderby c.CreatedDate descending
        //                   select new ListCouponModel()
        //                   {
        //                       Code= c.Code,
        //                       Name = c.Name,
        //                       CreateDate = c.CreatedDate,
        //                       Content = c.Content,
        //                       FromDate = c.CreatedDate.ToString(),
        //                       Discount = c.Discount,
        //                       ExpriceDate = c.ExpriceDate,
        //                       TypeCoupon = c.TypeCoupon,
        //                       Type = c.Type,
        //                       Amount = c.QTY,
        //                       Remain = c.Remain

        //                   }).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
        //        return lst;
        //    }
        //    catch(Exception ex)
        //    {
        //        ex.ToString();
        //        return null;
        //    }
        //}
        //NotifyBusiness notiBus = new NotifyBusiness();
        //public List<CouponOutputModel> getlistCounponBuyRole(int? customerID, int? agentID, int type, int serviceID, int comboID, bool check = true)
        //{
        //    List<CouponOutputModel> output = new List<CouponOutputModel>();
        //    List<int> listCounponIDUsed = new List<int>();
        //    int typeCoupon = 0;
        //    var lsCounpon = cnn.Coupons.Where(u =>
        //    u.IsActive.Equals(SystemParam.ACTIVE)
        //    && (u.ExpriceDate.HasValue ? u.ExpriceDate.Value >= DateTime.Now : true)
        //    && (u.QTY > 0 ? u.Remain > 0 : true)
        //    && (u.ServiceID.HasValue ? u.ServiceID.Value.Equals(serviceID) : true)
        //    && (u.ComboID.HasValue ? u.ComboID.Value.Equals(comboID) : true)
        //    && (u.ExpriceDate.HasValue || u.QTY > 0)
        //    && !String.IsNullOrEmpty(u.Code)
        //    && u.Code.Length > 0).ToList();
        //    if (customerID.HasValue)
        //    {
        //        listCounponIDUsed = cnn.OrderServices.Where(u => u.CustomerID.Equals(customerID.Value) && u.CouponID.HasValue).Select(u => u.CouponID.Value).ToList();
        //        Customer customer = cnn.Customers.Find(customerID.Value);
        //        typeCoupon = Constant.COUPON_SERVICE;
        //        if (!type.Equals(Constant.COUPON_NOT_USED))
        //        {
        //            lsCounpon = lsCounpon.Where(u => listCounponIDUsed.Contains(u.ID)).ToList();
        //        }
        //        else
        //        {
        //            lsCounpon = lsCounpon.Where(u =>
        //            // lấy những mã gửi riêng
        //            (u.CouponCustomers.Count() > 0 ? u.CouponCustomers.Select(s => s.CustomerID).ToList().Contains(customerID.Value) : true)
        //            && (check ? u.Status.Equals(SystemParam.ACTIVE) : true)
        //            ).ToList();
        //            lsCounpon = lsCounpon.Where(u =>
        //            // lấy theo rank
        //            (u.RankID.HasValue ? u.RankID.Value.Equals(customer.CustomerRankID) : true)
        //            ).ToList();
        //        }
        //    }
        //    else if (agentID.HasValue)
        //    {
        //        Agent agent = cnn.Agents.Find(agentID.Value);
        //        listCounponIDUsed = cnn.ProductOrders.Where(u => u.AgentID.Equals(agentID.Value) && u.CouponID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => u.CouponID.Value).ToList();
        //        typeCoupon = Constant.COUPON_PRODUCT;
        //        if (type.Equals(Constant.COUPON_NOT_USED))
        //        {
        //            lsCounpon = lsCounpon.Where(u =>
        //                // lấy những mã gửi riêng
        //                (u.CouponWashers.Count() > 0 ? u.CouponWashers.Select(s => s.AgentID.Value).ToList().Contains(agentID.Value) : true)
        //                && (check ? u.Status.Equals(SystemParam.ACTIVE) : true)
        //                ).ToList();
        //            lsCounpon = lsCounpon.Where(u =>
        //            // lấy theo rank
        //            (u.ConfigCommissionID.HasValue ? u.ConfigCommissionID.Value.Equals(agent.CommissionID.Value) : true)
        //            ).ToList();
        //        }
        //    }
        //    if (!type.Equals(Constant.COUPON_NOT_USED))
        //        lsCounpon = lsCounpon.Where(u => listCounponIDUsed.Contains(u.ID)).ToList();
        //    else
        //        lsCounpon = lsCounpon.Where(u => !listCounponIDUsed.Contains(u.ID)).ToList();

        //    output = lsCounpon.Where(u => u.Type.Equals(typeCoupon)).Select(u => new CouponOutputModel
        //    {
        //        CouponCode = u.Code,
        //        CouponID = u.ID,
        //        ExpriceDate = u.ExpriceDate,
        //        Discount = u.TypeCoupon.Value.Equals(Constant.COUPON_TYPE_DISCOUNT) ? u.Discount.Value : u.Persent.Value,
        //        TypeDiscount = u.TypeCoupon.Value,
        //        Path = u.Name,
        //        Content = u.Content
        //    }).ToList();
        //    return output;
        //}
        //public List<ListCouponModel> LoadListCoupons()
        //{
        //    List<ListCouponModel> list = new List<ListCouponModel>();
        //    var query = (from c in cnn.Coupons
        //                 orderby c.Name
        //                 where c.IsActive == 1
        //                 select new ListCouponModel()
        //                 {
        //                     ID = c.ID,
        //                     Code = c.Code,
        //                     Name = c.Name,
        //                     Content = c.Content,
        //                     //Type = c.Type,
        //                     Status = c.Status,
        //                     QTY = c.QTY,
        //                     Remain = c.Remain,
        //                     CreateDate = c.CreateDate,
        //                     ExpriceDate = c.ExpriceDate
        //                 }).ToList();
        //    if (query != null && query.Count() > 0)
        //    {
        //        return list;
        //    }
        //    else
        //    {
        //        return new List<ListCouponModel>();
        //    }
        //}
        public IPagedList<ListCouponModel> Search(int Page, string Code, int? TypeCoupon, string FromDate, string ToDate)
        {
            try
            {
                List<ListCouponModel> list = new List<ListCouponModel>();
                DateTime? startdate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);
                var query = (from c in cnn.Coupons
                             where !c.IsActive.Equals(SystemParam.INACTIVE)
                             && (!String.IsNullOrEmpty(Code) ? c.Code.Contains(Code) : true)
                             && (TypeCoupon.HasValue ? c.Type.Equals(TypeCoupon.Value) : true)

                             && (((startdate.HasValue ? c.StartDate >= startdate.Value : true)
                             && (endDate.HasValue ? c.StartDate <= endDate.Value : true)) ||
                             ((startdate.HasValue ? c.ExpriceDate >= startdate.Value : true)
                             && (endDate.HasValue ? c.ExpriceDate <= endDate.Value : true)))
                             orderby c.ID descending
                             select new ListCouponModel
                             {
                                 ID = c.ID,
                                 Code = c.Code,
                                 Content = c.Content,
                                 TypeCoupon = c.TypeCoupon,
                                 Type = c.Type,
                                 Discount = c.Discount,
                                 Redeme = (c.QTY - c.Remain),
                                 Percent = c.Percent,
                                 Status = c.Status,
                                 QTY = c.QTY,
                                 Remain = c.Remain,
                                 CreateDate = c.StartDate,
                                 ExpriceDate = c.ExpriceDate,
                                 StartDate = c.StartDate
                             }).ToPagedList(Page, 20);
                //query = query.Where(c => (Type == Constant.COUPON_NOT_USED ? (c.GetStrExpriceDate != ""): true) || (Type == SystemParam.STATUS_COUPONS_TYPE_UNEXPIRED ? (c.GetStrExpriceDate == "") : true)).ToList();
                //if (query != null && query.Count() > 0)
                //{
                //    foreach (var value in query)
                //    {
                //        list.Add(value);
                //    }

                //}
                return query;
            }
            catch (Exception e)
            {
                e.ToString();
                return null;
            }
        }
        public int CreateCoupon(string Name, string Code, string Content, int TypeCoupon, int type,  int TypeTime, string Amount, bool allCustomer, string CreateDate, string ExpriceDate, int QTY, int? rank = null, List<int> listCusID = null, int status = SystemParam.ACTIVE)
        {

            try
            {
                DateTime? StartDate = Util.ConvertDate(CreateDate);
                DateTime? EndDate = Util.ConvertDate(ExpriceDate);
                if (allCustomer)
                {
                    listCusID = cnn.Members.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CustomerID.HasValue && u.IsLogin).Select(u => u.CustomerID.Value).ToList();
                    QTY = listCusID.Count();
                }
                Coupon cp = new Coupon();
                List<Member> lsCus = new List<Member>();
                if (listCusID != null && listCusID.Count > 0)
                {
                    cp.CouponCustomers = listCusID.Select(u => new CouponCustomer
                    {
                        CreatedDate = DateTime.Now,
                        CustomerID = u,
                        IsActive = SystemParam.ACTIVE
                    }).ToList();
                    lsCus = cnn.Members.Where(u => listCusID.Contains(u.CustomerID.Value)).ToList();

                }
                cp.Name = Name;
                cp.Code = Code;
                cp.Content = Content;
                cp.TypeCoupon = TypeCoupon;
                cp.Type = type;
                cp.QTY = QTY;
                cp.Remain = QTY;
                if (TypeCoupon == Constant.COUPON_TYPE_PERCENT)
                {
                    cp.Percent = Int32.Parse(Amount.Replace(",", ""));
                }
                else
                {
                    cp.Discount = Int32.Parse(Amount.Replace(",", ""));
                }

                cp.CreatedDate = DateTime.Now;
                cp.IsActive = SystemParam.ACTIVE;
                cp.Status = 1;
                if (!allCustomer)
                {
                    cp.RankID = rank;
                }

                if (TypeTime == 1)//type có thời hạn 1
                {
                    cp.StartDate = StartDate;
                    cp.ExpriceDate = EndDate;
                }
                cnn.Coupons.Add(cp);
                cnn.SaveChanges();
                if (lsCus.Count > 0 && status.Equals(SystemParam.ACTIVE))
                {
                    //SenNotiCoupon(lsCus, cp, role);
                }

                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
                throw;
            }
        }
        //public void SenNotiCoupon(List<Member> lsCus, Coupon coupons, int role)
        //{
        //    List<int> listCusVN = lsCus.Where(u => u.Lang.Equals(SystemParam.VN)).Select(u => u.ID).ToList();
        //    List<int> listCusEN = lsCus.Where(u => !u.Lang.Equals(SystemParam.VN)).Select(u => u.ID).ToList();
        //    if (listCusVN.Count > 0)
        //    {
        //        notiBus.CreateNotiAllCustomer("Mã khuyến mại của bạn là: " + coupons.Code, "Khuyến mãi", role, SystemParam.ACTIVE, listCusVN, coupons.Code, Constant.NOTI_COUPON, null, "Mã khuyến mại của bạn là: " + coupons.Code);
        //    }
        //    if (listCusEN.Count > 0)
        //    {
        //        notiBus.CreateNotiAllCustomer("Your promotional code is: " + coupons.Code, "Promotion", role, SystemParam.ACTIVE, listCusEN, coupons.Code, Constant.NOTI_COUPON, null, "Your promotional code is: " + coupons.Code);
        //    }
        //}
        public ListCouponModel ModalEditCoupon(int? ID)
        {
            try
            {
                ListCouponModel couponDetail = new ListCouponModel();
                var query = (from c in cnn.Coupons
                             where c.IsActive.Equals(SystemParam.ACTIVE)
                             && c.ID == ID.Value
                             select c).ToList();
                var data = query.Select(c => new ListCouponModel
                {
                    ID = c.ID,
                    Code = c.Code,
                    Content = c.Content,
                    TypeCoupon = c.TypeCoupon,
                    Type = c.Type,
                    Status = c.Status,
                    Percent = c.Percent,
                    Discount = c.Discount,
                    Name = c.Name,
                    QTY = c.QTY,
                    listCustomer = (c.RankID != null ? c.CouponCustomers.Select(u => new CouponCustomerView { ID = u.CustomerID, Name = u.Customer.Name, Phone = u.Customer.Phone }).ToList() : null),
                    Remain = c.Remain,
                    CreateDate = c.CreatedDate,
                    StartDate = c.StartDate,
                    ExpriceDate = c.ExpriceDate,
                    rankId = c.RankID,
                    TypeTime = c.TypeTime,

                }).FirstOrDefault();
                if (data != null && data.ID > 0)
                {
                    return couponDetail = data;
                }
                return couponDetail;

            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ListCouponModel();
            }
        }
        public int SaveEditCoupon(ListCouponModel obj, List<int> listCusID = null)
        {
            //DateTime StartDate = Convert.ToDateTime(obj.CreateDate);
            try
            {
                Coupon data = cnn.Coupons.Find(obj.ID);
                data.Code = obj.Code;
                data.TypeCoupon = obj.TypeCoupon;
                data.Content = obj.Content;
                data.RankID = obj.rankId > 0 ? obj.rankId : null;
                if (obj.TypeCoupon == Constant.COUPON_TYPE_PERCENT)
                {
                    data.Percent = Int32.Parse(obj.Amount.Replace(",", ""));
                }
                else
                {
                    data.Discount = Int32.Parse(obj.Amount.Replace(",", ""));
                }
                data.QTY = obj.QTY;
                if (obj.Name != "" && obj.Name != null)
                {
                    data.Name = obj.Name;
                }
                data.Remain = obj.Remain;
                data.TypeTime = obj.TypeTime;
                if (obj.FromDate != "" && obj.FromDate != null && obj.ToDate != "" && obj.ToDate != null)
                {
                    data.StartDate = Util.ConvertDate(obj.FromDate).Value;
                    data.ExpriceDate = Util.ConvertDate(obj.ToDate).Value;
                }
                else
                {
                    data.ExpriceDate = null;
                }
                if (listCusID != null && listCusID.Count > 0)
                {
                    List<CouponCustomer> query = listCusID.Select(u => new CouponCustomer
                    {
                        CouponID = data.ID,
                        CreatedDate = DateTime.Now,
                        CustomerID = u,
                        IsActive = SystemParam.ACTIVE
                    }).ToList();
                    cnn.CouponCustomers.AddRange(query);
                    var lsCus = cnn.Members.Where(u => listCusID.Contains(u.CustomerID.Value)).ToList();
                    List<int> listCusVN = lsCus.Where(u => u.Lang.Equals(SystemParam.VN)).Select(u => u.ID).ToList();
                    List<int> listCusEN = lsCus.Where(u => !u.Lang.Equals(SystemParam.VN)).Select(u => u.ID).ToList();
                    if (listCusVN.Count > 0)
                    {
                        //notiBus.CreateNotiAllCustomer("Bạn được tặng một mã khuyến mại. Mã của bạn là: " + data.Code, "CarRect gửi tặng bạn một mã khuyến mại", Constant.CUSTOMER_ROLE, SystemParam.ACTIVE, listCusVN, data.Code);
                    }
                    if (listCusEN.Count > 0)
                    {
                        //notiBus.CreateNotiAllCustomer("You are offered a promotional code. Your code is: " + data.Code, "CarRect sends you a promotional code", Constant.CUSTOMER_ROLE, SystemParam.ACTIVE, listCusEN, data.Code);
                    }
                }
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int DeleteCoupon(int ID)
        {
            try
            {
                Coupon coupon = cnn.Coupons.Find(ID);
                coupon.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;

            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        //public CouponOutputModel checkCoupon(TokenOutputModel checkToken, string couponCode, int serviceID, int comboID)
        //{
        //    List<CouponOutputModel> listNotUse = getlistCounponBuyRole(checkToken.CustomerID, checkToken.AgentID, Constant.COUPON_NOT_USED, serviceID, comboID, false);
        //    if (String.IsNullOrEmpty(couponCode) || couponCode.Length == 0)
        //        return null;
        //    var coupon = listNotUse.Where(u => u.CouponCode.ToLower().Equals(couponCode.ToLower())).FirstOrDefault();
        //    return coupon;
        //}

        //public List<CouponCustomerView> GetListCusByCoupon(int Id, int? type)
        //{
        //    try
        //    {
        //        if (type.Value == 1)
        //        {
        //            return cnn.CouponWashers.Where(x => x.CouponID == Id && x.IsActive == SystemParam.ACTIVE).Select(c => new CouponCustomerView
        //            {

        //                ID = c.Agent.ID,
        //                Name = c.Agent.Name,
        //                Phone = c.Agent.Phone
        //            }).ToList();
        //        }
        //        else
        //        {
        //            return cnn.CouponCustomers.Where(x => x.CouponID == Id && x.IsActive == SystemParam.ACTIVE).Select(c => new CouponCustomerView
        //            {

        //                ID = c.Customer.ID,
        //                Name = c.Customer.Name,
        //                Phone = c.Customer.Phone
        //            }).ToList();
        //        }
        //    }
        //    catch
        //    {
        //        return new List<CouponCustomerView>();
        //    }
        //}

        //public List<ConfigCommissionViewModel> GetListConfig(int type)
        //{


        //    if (type == 1)
        //    {
        //        return cnn.ConfigCommissions.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new ConfigCommissionViewModel
        //        {
        //            ID = u.ID,
        //            MastersBenefit = u.MastersBenefit,
        //            Name = u.Name
        //        }).ToList();
        //    }
        //    else
        //    {
        //        return cnn.CustomerRanks.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new ConfigCommissionViewModel
        //        {
        //            ID = u.ID,
        //            MastersBenefit = null,
        //            Name = u.Name
        //        }).ToList();
        //    }
        //}

    }
}