using Microsoft.Office.Interop.Excel;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ShiftOutputModel
    {
        public int ID { get; set; }
        public string shift { get; set; }
        public int IsActive { get; set; }
    }
    public class ShiftViewModel {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<DayViewModel> ListDays { get; set; }
        public List<ShiftOutputModel> ListShift { get; set; }
    }
    public class DayViewModel
    {
        public int ID { get; set; }
        public string Day { get; set; }
    }
    public class ShiftInWeekModel {
        public string Date { get; set; }
        public List<string> ListShift { get; set; }
    }
}
