using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CustomerLocationModel
    {
        public int ID { get; set; }
        public double Longi { get; set; }
        public double lati { get; set; }
        public string Name { get; set; }
        public string PlaceID { get; set; }
        public string Address { get; set; }
        public string CustomerAddress { get; set; }
        public int MemberID { get; set; }
    }
    public class CustomerLocationSuggestModel
    {
        public string Address { get; set; }
        public double Longi { get; set; }
        public double Lati { get; set; }
    }
}
