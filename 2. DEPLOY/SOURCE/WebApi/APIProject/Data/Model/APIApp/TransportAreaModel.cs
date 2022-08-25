using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class TransportAreaOutputModel
    {
        public int Type { get; set; }
        public List<TransportAreaModel> ListTransport { get; set; }
    }
    public class TransportAreaModel
    {
        public int TransportType { get; set; }
        public double Weight { get; set; }
        public double Distance { get; set; }
        public int IsProvince { get; set; }
        public int Price { get; set; }
        public int BonusFee { get; set; }
        public string BonusFeeDescription { get; set; }
    }
    public class TransportFoodModel
    {
        public double Distance { get; set; }
        public double FirstDistance { get; set; }
        public int FirstPrice { get; set; }
        public int PerKmPrice { get; set; }
        public int Price { get; set; }
        public int BonusFee { get; set; }
        public string BonusFeeDescription { get; set; }
    }
}
