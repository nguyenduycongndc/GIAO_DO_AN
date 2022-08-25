using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class OneSignalOutputModel
    {
        public string id { get; set; }
        public int recipients { get; set; }
        public object errors { get; set; }
        public object external_id { get; set; }
    }
}
