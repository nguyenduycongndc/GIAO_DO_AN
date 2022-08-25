using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class DataPageListOutputModel
    {
        public int page { get; set; }
        public int limit { get; set; }
        public double totalPage { get; set; }
        public object data { get; set; }
    }
}
