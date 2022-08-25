using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class OrderProductBusiness : GenericBusiness
    {
        ProductBusiness proBus = new ProductBusiness();
        public OrderProductBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public SystemResult Search(string search = "", int? status = null, string FromDate = "", string ToDate = "")
        {
            List<OrderProductModel> query = new List<OrderProductModel>();
            DateTime? fDate = Util.ConvertDate(FromDate);
            DateTime? tDate = Util.ConvertDate(ToDate);
            if (tDate != null)
                tDate = tDate.Value.AddDays(1);
            if (String.IsNullOrEmpty(search))
                search = "";
            var listOrderProduct = cnn.ProductOrders.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && !u.Status.Equals((int)StatusOrderProduct.Reject)
            && !u.Status.Equals((int)StatusOrderProduct.Ordering)
            && (status.HasValue ? u.Status.Equals(status.Value) : true)
            && (fDate.HasValue ? u.CreateDate >= fDate : true)
            && (tDate.HasValue ? u.CreateDate <= tDate : true)
            ).ToList();
            query = listOrderProduct.Where(u => Util.Converts(u.Agent.Name).Contains(Util.Converts(search)) || Util.Converts(u.Code).Contains(Util.Converts(search))).Select(u => new OrderProductModel
            {
                ID = u.ID,
                Code = u.Code,
                Price = u.TotalPrice,
                Status = u.Status,
                WasherName = u.Agent.Name,
                WasherPhone = u.Agent.Phone,
                CreateDate = u.CreateDate,
            }).OrderByDescending(t=>t.CreateDate).ToList();
            return resultBus.SucessResult(query);
        }
        public SystemResult Detail(int ID)
        {
            OrderProductDetailModel query = new OrderProductDetailModel();
            var listOrderProduct = cnn.ProductOrders.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && u.ID.Equals(ID)
            ).ToList();
            query = listOrderProduct.Select(u => new OrderProductDetailModel
            {
                ID = u.ID,
                Code = u.Code,
                Price = u.TotalPrice,
                Status = u.Status,
                WasherName = u.Agent.Name,
                WasherPhone = u.Agent.Phone,
                CreateDate = u.CreateDate,
                CouponCode = u.CouponID.HasValue ? u.Coupon.Code : "",
                BasePrice = u.BasePrice.HasValue ? u.BasePrice.Value : u.TotalPrice,
                Discount = u.Discount.HasValue ? u.Discount.Value : 0,
                ListOrderDetail = u.ProductOrderDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.QTY > 0).Select(s => new ProductOrderDetails
                {
                    ID = s.ID,
                    PerPrice = s.Product.Price,
                    BasePrice = s.Product.BasePrice,
                    Description = s.Product.Description,
                    TotalPrice = s.Price,
                    qty = s.QTY,
                    Name = s.Product.NameEN,
                }).ToList()
            }).FirstOrDefault();
            return resultBus.SucessResult(query);
        }

        public SystemResult ChangeStatus(int ID, int status, string note = "")
        {
            return proBus.ChangeStatusProductOrder(status, ID, null, note);
        }
    }
}
