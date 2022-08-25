using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{

    public class UpdateLocationDriverModel
    {
        public int ID { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }

    }
    public class LocationDriverModel
    {
        public string latitude { get; set; }
        public string longtitude { get; set; }
    }
}
