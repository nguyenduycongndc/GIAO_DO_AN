using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class VehicleModel
    {
        public int ID { get; set; }
        public string CustomerName { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string SegmetName { get; set; }
        public string LicencePalte { get; set; }
        public string Year { get; set; }
        public int isVeryfile { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class VehicleDetailModel : VehicleModel
    {
        public string CustomerAvatar { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string RegistrationDateStr { get { return RegistrationDate.HasValue? RegistrationDate.Value.ToString(SystemParam.CONVERT_DATETIME) :""; } }
        public string CarColor { get; set; }
        public List<string> listImage { get; set; }
    }
}
