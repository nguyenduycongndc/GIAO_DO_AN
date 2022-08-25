using Data.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ShopHistoryBusinessModel : GenericBusiness
    {
        public int TypeService { get; set; }
        public string CustomerName { get; set; }
        public float TotalPrice { get; set; }
        public int Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? OrderServiceID { get; set; }
        public string ServiceStr { get { return getServiceStr(); } }
        public string getServiceStr()
        {
            if (OrderServiceID.HasValue)
            {
                var res = (from c in cnn.OrderServiceDetails
                           where c.OrderServiceID == OrderServiceID
                           select c.ServicePrice.Name).ToList();
                return res.Aggregate("",(a,b)=>a + b+",");
            }
            return null;
        }
    }
}
