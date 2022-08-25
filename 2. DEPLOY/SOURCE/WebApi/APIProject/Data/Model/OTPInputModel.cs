using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class OTPInputModel
    {
        public string To { get; set; }
        public int type { get; set; }
        public string from { get; set; }
        public string scheduled { get; set; }
        public string message { get; set; }
        public string requestId { get; set; }
        public int useUnicode { get; set; }
        public int maxMt { get; set; }
    }
}
