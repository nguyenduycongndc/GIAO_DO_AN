using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIApp
{
    public class LoginAppInputModel
    {
        public int UserID { get; set; }
        public string name { get; set; }
        public string Phone { get; set; }
        public string PassWord { get; set; }
        public string OTP { get; set; }
        public string token { get; set; }
        public int Type { get; set; }
        public string deviceID { get; set; }
        public string email { get; set; }
        public int role { get; set; }
    }

    public class ChangePass
    {
        public string newPass { get; set; }
        public string OldPass { get; set; }
    }
}