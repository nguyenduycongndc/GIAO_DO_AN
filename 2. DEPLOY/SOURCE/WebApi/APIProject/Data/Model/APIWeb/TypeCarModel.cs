using Data.Utils;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class TypeCarModel
    {
        public int ID { get; set; }
        public string NameEN { get; set; }
        public string NameVN { get; set; }
        public string Note { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }
    public class CarModelInCarTypeModel
    {
        public int ID { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public int? Year { get; set; }
        public int IsActive { get; set; }
    }
    public class TypeCarDetailModel : TypeCarModel
    {
        public List<CarModelInCarTypeModel> listCar { get; set; }
    }
}
