using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ConfigTransportAreaModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int IsProvince { get; set; }
        public int? FromKm { get; set; }
        public int? ToKm { get; set; }
        public int Type { get; set; }
        public float PerKg { get; set; }
        public string TimeShip { get; set; }
        public int Price { get; set; }
    }
}
