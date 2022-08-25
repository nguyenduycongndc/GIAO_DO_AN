using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ProvinceAndDistrictModel
    {
        public object province { get; set; }
        public object listDistrict { get; set; }
    }
    public class ProvinceModel
    {
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
        public string ProvinceType { get; set; }
    }
    public class DistrictModel
    {
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public string DistrictType { get; set; }
        public int ProvinceCode { get; set; }
        public int AreaCode { get; set; }
    }
}
