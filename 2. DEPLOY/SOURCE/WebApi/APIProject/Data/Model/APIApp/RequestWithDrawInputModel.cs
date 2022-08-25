using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class RequestWithDrawInputModel
    {
        public int shiperID { get; set; }
        public int bankID { get; set; }
        public string account { get; set; }
        public long amount { get; set; }
        public string owner { get; set; }
        public string content { get; set; }

    }
    public class RequestDepositInputModel
    {
        public long Amount { get; set; }
    }
}
