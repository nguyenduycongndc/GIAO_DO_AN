using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ConfigDataModel
    {
        public WalletDataConfig WalletConfig { get; set; }
        public AreaDataCofig AreaConfig { get; set; }
        public TransactionDataConfig TransactionConfig { get; set; }
        public List<PeakHourConfig> PeakHourConfig { get; set; }
    }

    public class WalletDataConfig
    {
        public int ConfigWalletWithdrawID { get; set; }
        public string ConfigWalletWithdrawValue { get; set; }
        public int ConfigWalletNoWithdrawID { get; set; }
        public string ConfigWalletNoWithdrawValue { get; set; }
        public int ConfigTimeWithdrawID { get; set; }
        public string ConfigTimeWithdrawValue { get; set; }
    }

    public class AreaDataCofig
    {
        public int MaxAreaConfigID { get; set; }
        public string MaxAreaConfigValue { get; set; }
        public int StartTimeConfigID { get; set; }
        public string StartTimeConfigValue { get; set; }
        public int EndTimeConfigID { get; set; }
        public string EndTimeConfigValue { get; set; }

    }

    public class TransactionDataConfig
    {
        public int CountDownConfigID { get; set; }
        public string CountDownConfigValue { get; set; }
        public int CancelTimeConfigID { get; set; }
        public string CancelTimeConfigValue { get; set; }
        public int TimeCancelOrderID { get; set; }
        public string TimeCancelOrderValue { get; set; }
        public int MaxCODConfigID { get; set; }
        public string MaxCODConfigValue { get; set; }
    }

    public class PeakHourConfig
    {
        public int ID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
    }
}
