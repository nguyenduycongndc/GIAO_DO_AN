using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ShopOutputModel
    {
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public string ProvinceName { get; set; }
        public string Address { get; set; }
        public string Avatar { get; set; }
        public double Rate { get; set; }
        public double Lati { get; set; }
        public double Longi { get; set; }
        public string ContactPhone { get; set; }
        public string ContactMail { get; set; }
        public double Distance { get; set; }
        public int CountRate { get; set; }
        public int Status { get; set; }
        public List<string> ListImage { get; set; }
    }
    public class ShopDeleteInputModel
    {
        public int ID { get; set; }
    }
}
