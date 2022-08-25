using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CarCustomerViewModel
    {
        public int ID { get; set; }
        public int CarModelID { get; set; }
        public string BrandName { get; set; }
        public string CarModelName { get; set; }
        public string YearOfProduction { get; set; }
        public string Color { get; set; }
        public int CarStatus { get; set; }
        public string LicensePlates { get; set; }
        public List<string> CarImage { get; set; }
        public int? CarSegmentId { get; set; }
        public int IsVerify { get; set; }
        public string CarSegmentName { get; set; }


    }
}
