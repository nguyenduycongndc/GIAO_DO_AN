using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CarOutputModel
    {
        public int carID { get; set; }
        public string CarModel { get; set; }
        public int CarModelID { get; set; }
        public string CarBrand { get; set; }
        public int CarBrandID { get; set; }
        public string LicensePlates { get; set; }
        public string ManufacturingDate { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationDateStr { get { return RegistrationDate.HasValue ?RegistrationDate.Value.ToString(SystemParam.CONVERT_DATETIME) :""; } }
        public string CarColor { get; set; }
        public object ListImage { get; set; }
        public string CarImage { get; set; }
        public string Status { get; set; }
        public string VehicleRegistration { get; set; }
    }
    public class CarBrandModel
    {
        public string Name { get; set; }
        public List<CarModelModel> listCarModel { get; set; }
    }
    public class CarModelModel
    {
        public string Name { get; set; }
        public int Year { get; set; }
        public string Image { get; set; }
    }
}
