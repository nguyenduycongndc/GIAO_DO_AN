using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.ShopModel
{
    public class ServicePriceDetailModel : MenuByCategoryModel
    {
        public int ServiceType { get; set; }
        public int? ServiceID { get; set; }

    }
}
