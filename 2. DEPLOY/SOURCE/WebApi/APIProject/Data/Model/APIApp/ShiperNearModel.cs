using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ShiperNearModel
    {
        public int ID { get; set; }
        public int OrderCount { get; set; }
        public string Code { get; set; }
        public double Longi { get; set; }
        public double Lati { get; set; }
        public string DeviceID { get; set; }
        public string ShipperOrigin { get; set; }
        public double Distance { get; set; }
        public int? IsInternal { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
