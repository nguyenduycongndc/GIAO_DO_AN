using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ConfigTransportCostOuputModel
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public int? TransportType { get; set; }
        public string VehicleType { get; set; }
        public double FirstDistance { get; set; }
        public double FirstPrice { get; set; }
        public double PerKmPrice { get; set; }
    }
}
