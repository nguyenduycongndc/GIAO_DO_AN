using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ThreadCancelModel
    {
        public int OrderID { get; set; }
        public int MemberID { get; set; }
        public string Lang { get; set; }
        public string DeviceID { get; set; }
    }
    public class ThreadSendNotiFirst
    {
        public int OrderID { get; set; }
        public int FirstWasher { get; set; }

    }
}
