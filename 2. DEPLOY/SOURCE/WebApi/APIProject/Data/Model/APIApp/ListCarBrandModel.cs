using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListCarBrandModel
    {
        public object listCarMode { get; set; }
        public object listCarBrand { get; set; }
    }

    public class ListGrade
    {
      public  int ID { get; set; }
       public string Name { get; set; }
    }
    public class CarModeModel
    {
        public int CarModelID { get; set; }
        public string CarBrandName { get; set; }
        public int? CarBrandID { get; set; }
        public string Name { get; set; }
        public string SegmentName { get; set; }
        public string SegmentDescription { get; set; }
    }
    public class CarModeInputModel : CarModeModel
    {
        public string LicensePlates { get; set; }
        public string ManufacturingDate { get; set; }
        public string RegistrationDateStr { get; set; }
        public DateTime RegistrationDateTime { get; set; }
        public string CarColor { get; set; }
        public int CarID { get; set; }
        public string Status { get; set; }
        public string VehicleRegistration { get; set; }

    }
}
