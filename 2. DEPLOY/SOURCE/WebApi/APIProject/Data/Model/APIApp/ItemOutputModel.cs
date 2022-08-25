using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ServiceOutputModel
    {
        public int ServiceID { get; set; }
        public int Type { get; set; }
        public string CateName { get; set; }
        public int Price { get; set; }
        public double USDPrice { get; set; }
        public double USDBasePrice { get; set; }
        public List<string> ImageUrl { get; set; }
        public string MainImage { get; set; }
        public int BasePrice { get; set; }
        public int Discount { get; set; }
        public string Thumbnail { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public int IsFree { get; set; }
        public int IsUsed { get; set; }

        public int estTime { get; set; }

    }
    public class ServiceComboOutputModel
    {
        public int ComboID { get; set; }
        public string CateName { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public double USDPrice { get; set; }
        public double USDBasePrice { get; set; }
        public int Discount { get; set; }
        public int CountItem { get; set; }
        public List<string> ImageUrl { get; set; }
        public string MainImage { get; set; }
        public string Thumbnail { get; set; }
        public string Icon { get; set; }

        public List<ServiceOutputModel> listServices { get; set; }
        public List<int> listServiceID { get; set; }
        public string Description { get; set; }
        public int estTime { get; set; }

    }
    public class ListItemModel
    {
        public List<ServiceOutputModel> listInput { get; set; }
        public List<ServiceComboOutputModel> listInputCombo { get; set; }
    }
}
