using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class AreaOutputModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int DistrictCode { get; set; }
        public string DistrictName { get; set; }
        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; }
    }
}
