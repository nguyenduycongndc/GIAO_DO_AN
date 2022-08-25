using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ShiperInputModel
    {
        public int ID { get; set; }
        public string Avatar { get; set; }
        public string ImgIdentify { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public int Sex { get; set; }
        public string Identify { get; set; }
        public int ComissionID { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int VehicleTypeID { get; set; }
        public string LicensePlate { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public string ShipperProvinces { get; set; }
        public int IsInternal { get; set; }
        public int IsVip { get; set; }
    }
}
