using Data.Utils;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ReportCustomerModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int CountTransaction { get; set; }
        public int CountCompleteYet{ get; set; }
        public int Paid { get; set; }
        public string PaidStr { get { return string.Format("{0:#,0}", Paid); } }
        public int CountCancle { get; set; }
        public int CountPaymentInVnPay { get; set; }
        public int CountPaymentInCash { get; set; }
    }

}
