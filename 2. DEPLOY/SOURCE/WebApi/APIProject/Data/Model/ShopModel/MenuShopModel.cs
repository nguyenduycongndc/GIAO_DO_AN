using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.ShopModel
{
    public class MenuShopModel
    {
        public  int ID { get; set; }
        public  string Name { get; set; }
        public List<MenuByCategoryModel> Data { get; set; }
    }

    public class MenuByCategoryModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public int Discount { get; set; }
        public int IsActive { get; set; }
        public int Type { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }

}
