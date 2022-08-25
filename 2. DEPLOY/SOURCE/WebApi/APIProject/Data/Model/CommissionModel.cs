using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class CommissionModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CarAutocareAcaDemy { get; set; }
        public string Duration { get; set; }
        public string Process { get; set; }
        public int MastersBenefit { get; set; }
    }

    public class ServiceAreaModel
    {
        public int ID { get; set; }
        public int ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }
    public class QAModels
    {
        public int ID { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public int OrderDisplay { get; set; }
        public int Type { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }

}
