using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class VehicleOutputModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public double Distance { get; set; }
        public double FirstDistance { get; set; }
        public int FirstPrice { get; set; }
        public int PerKmPrice { get; set; }
        public int Price { get; set; }
        public int BonusFee { get; set; }
        public string BonusFeeDescription { get; set; }
        public int OrderIndex { get; set; }
        public int IsMotorbike { get; set; }
    }
}
