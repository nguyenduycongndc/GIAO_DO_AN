using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ConfigTransportWeightInputModel
    {
        public int TransportAreaID { get; set; }
        public List<DataConfigTransportAreaPriceDetail> ListConfigTransportAreaPrice { get; set; }
    }
}
