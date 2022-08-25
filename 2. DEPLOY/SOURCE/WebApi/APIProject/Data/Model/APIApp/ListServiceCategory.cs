using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListServiceCategory
    {

        public List<ListBanerFood> ListBaner { get; set; }
        public List<ListCategory> lstServiceCategory { get; set; }
        public List<ListHotItem> ListHotItem { get; set; }

    }
    public class ListCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
    public class ListBanerFood
    {
        public int ID { get; set; }
        public string Image { get; set; }
    }

    public class ListHotItem
    {
        public int ID { get; set; }
        public string ShopName { get; set; }
        public double Rating { get; set; }
        public float Distance { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
    }
}
