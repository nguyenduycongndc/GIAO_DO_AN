using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class RequestTransactionModel
    {
        public int ID { get; set; }
        public string WasherName { get; set; }
        public string WasherPhone { get; set; }
        public int Amount { get; set; }
        public string AmountSTR { get { return string.Format("{0:#,0}", Amount); } }
        public string BrankName { get; set; }
        public string Acount { get; set; }
        public string Owner { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Date { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }
    public class RequestAddCarModel
    {

        public int ID { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public DateTime CreateDate { get; set; }
        public string Date { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }

    }
}
