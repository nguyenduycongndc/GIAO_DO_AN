using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListComplainOuputModel
    {
        public int ID { get; set; }
        public string CusName { get; set; }
        public string CusPhone { get; set; }
        public int BookingType { get; set; }
        public string ShiperName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ComplainType { get; set; }
    }
}
