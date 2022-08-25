using Data.Model.APIWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ConfigOutputModel
    {
        public ConfigWallet wallet{ get; set; }
        public ConfigTransaction Transaction { get; set; }
        public List<ShiftViewModel> Shift { get; set; }
        public ConfigGPS GPS { get; set; }
        public ConfigBirthdayGift BirthDay { get; set; }
        public ConfigRankup rankUp { get; set; }
        public string Distance { get; set; }

        public string Name_vn { get; set; }
        public string Name_en { get; set; }
    }

    public class ConfigWallet
    {
        public string MinimumBalanceInDesposit { get; set; }
        public string MinimumBalanceInCash { get; set; }
        public string WithdrawTime { get; set; }
        public string MinimumBanlanceSendMess { get; set; }
        public string MinBalanceSendRequestFirst { get; set; }
        public string Profit { get; set; }
    }
    public class ConfigTransaction
    {
        public string ExceptTimeOfWait { get; set; }
        public string CancelTransaction { get; set; }
        public string ConfirmationTime { get; set; }
        public string PendingTime { get; set; }
    }
    public class ConfigShiftTime {
        public int ID { get; set; }
        public string time { get; set; }
        public int IsActive { get; set; }
    }
    public class ConfigGPS {
        public string GPSNotValid { get; set; }
        public string MaxArea { get; set; }
    
    }
    public class ConfigBirthdayGift
    {
        public string Before { get; set; }
        public string After { get; set; }
    }
    public class ConfigRankup
    {
        public string Expired { get; set; }
    }
}
