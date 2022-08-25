using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class OrderProductModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string WasherName { get; set; }
        public string WasherPhone { get; set; }
        public int Price { get; set; }
        public string PriceStr { get { return string.Format("{0:#,0}", Price); } }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public int Status { get; set; }
        public string StatusStr
        {
            get
            {
                string str = "";
                switch (Status)
                {
                    case (int)StatusOrderProduct.WaitingAdminConfirm:
                        str = "Pending";
                        break;
                    case (int)StatusOrderProduct.AdminConfirm:
                        str = "Approve";
                        break;
                    case (int)StatusOrderProduct.Complete:
                        str = "Complete";
                        break;
                    case (int)StatusOrderProduct.AdminReject:
                        str = "Reject";
                        break;
                }
                return str;
            }

        }
    }
    public class OrderProductDetailModel : OrderProductModel
    {
        public int BasePrice { get; set; }
        public string BasePriceStr { get { return string.Format("{0:#,0}", BasePrice); } }
        public string CouponCode { get; set; }
        public int Discount { get; set; }
        public string DiscountStr { get { return string.Format("{0:#,0}", Discount); } }
        public List<ProductOrderDetails> ListOrderDetail { get; set; }
    }
    public class ProductOrderDetails
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int qty { get; set; }
        public int PerPrice { get; set; }
        public int BasePrice { get; set; }
        public string Description { get; set; }
        public string PerPriceStr { get { return string.Format("{0:#,0}", PerPrice); } }
        public int TotalPrice { get; set; }
        public string TotalPriceStr { get { return string.Format("{0:#,0}", TotalPrice); } }
    }
}
