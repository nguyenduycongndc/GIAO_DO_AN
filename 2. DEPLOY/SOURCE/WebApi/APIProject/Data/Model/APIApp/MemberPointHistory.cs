using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class MemberPointHistory
    {
        public int ID { get; set; }
        public int Type { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Point { get; set; }
        public string Content { get; set; }
        public int IsPlus { get; set; }
    }
}
