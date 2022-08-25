using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;

namespace Data.Model.APIWeb
{
    public class ListCouponModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Amount { get; set; }
        public Nullable<int> Percent { get; set; }
        public Nullable<int> Discount { get; set; }
        public int TypeCoupon { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public int QTY { get; set; }
        public int Remain { get; set; }//còn lại
        public int Redeme { get; set; }//đã dũng
        public string ImageUrl { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpriceDate { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? rankId { get; set; }
        public int TypeTime { get; set; }
        public int? configComissionID { get; set; }
        public List<CouponCustomerView> listCustomer { get; set; }
        public bool allcustomer { get; set; }
        public string GetStrCreateDate
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string GetStrExpriceDate
        {
            get
            {
                return ExpriceDate.HasValue ? ExpriceDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
                //return ExpriceDate.Value.ToString(SystemParam.CONVERT_DATETIME);
            }
            set { }
        }
    }
    public class CouponCustomerView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}
