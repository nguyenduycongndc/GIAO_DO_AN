using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class GooglePlaceAPI
    {
        public Result result { get; set; }
        public string status { get; set; }
    }
    public class Result {
        public List<AddressComponents> address_components { get; set; }
        public Location geometry { get; set; }
        public string vicinity { get; set; }
        public string formatted_address { get; set; }
    }
    public class AddressComponents {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }

    }
    public class Location {

        public LocationDetail location { get; set; }
    }
    public class LocationDetail {
        public double lat { get; set; }
        public double lng { get; set; }

    }
}
