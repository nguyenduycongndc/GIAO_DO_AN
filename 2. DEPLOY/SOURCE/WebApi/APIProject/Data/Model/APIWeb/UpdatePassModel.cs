using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class UpdatePassModel
    {
        public int UserID { get; set; }
        public string otp { get; set; }
        public string newPass { get; set; }
        public string oldPass { get; set; }
        public string deviceID { get; set; }
    }
}
