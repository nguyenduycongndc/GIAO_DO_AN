using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CouponOutputModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? Percent { get; set; }
        public int? Discount { get; set; }
        public int TypeCoupon { get; set; }
        public int Type { get; set; }
        public int TypeTime { get; set; }
        public int RankId { get; set; }
        public string RankName { get; set; }
        public string ServiceName { get; set; }
        public string Content { get; set; }
        public DateTime? ExprireDate { get; set; }
        public string ExprireDateStr
        {
            get
            {
                return ExprireDate.HasValue ? ExprireDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public DateTime? StartDate { get; set; }
        public string StartDateStr
        {
            get
            {
                return StartDate.HasValue ? StartDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        
    }
    public class CouponInputModel
    {
        public string CouponCode { get; set; }
        //public int Type { get; set; }
    }
}
