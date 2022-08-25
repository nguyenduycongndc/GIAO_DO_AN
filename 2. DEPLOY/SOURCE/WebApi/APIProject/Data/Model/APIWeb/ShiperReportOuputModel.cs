using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ShiperReportOuputModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public int countTransasction { get; set; }
        public int countCompleteYet { get; set; }
        public int countCustomerCancel { get; set; }
        public int countShiperCancel { get; set; }
        public int shiperCommission { get; set; }
        public int Paid { get; set; }
    }
}
