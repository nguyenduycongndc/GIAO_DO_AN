using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListShopOuputModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public double lati { get; set; }
        public double longi { get; set; }
        public string avatar { get; set; }
        public double rating { get; set; }
        public string description { get; set; }
        public float distance { get; set; }
        public int countOrder { get; set; }
        public int status { get; set; }
        public List<ServicePriceModel> ListServicePrice{ get; set; }
    }
    public class ServicePriceModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string UrlImage { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
    }
}
