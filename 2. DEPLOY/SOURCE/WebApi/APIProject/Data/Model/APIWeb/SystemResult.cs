using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class SystemResult
    {
        public int Status { get; set; }
        public int Code { get; set; }
        public object Result { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
