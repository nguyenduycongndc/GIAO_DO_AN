using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;

namespace Data.Model.APIWeb
{
    public class PointDetailOutputModel
    {
        public int ID { get; set; }
        public string TransactionID { get; set; }
        public int Type { set; get; }
        public int TransactionType { set; get; }
        public int BeforeBalance { set; get; }
        public int AfterBalance { set; get; }
        public string CustomerName { get; set; }
        public string Phone { set; get; }
        public string Address { set; get; }
        public DateTime? CreatDate { set; get; }
        public String GetStringCreateDate
        {
            set { }
            get
            {
                return CreatDate.HasValue ? CreatDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public int Point { set; get; }
    }
}
