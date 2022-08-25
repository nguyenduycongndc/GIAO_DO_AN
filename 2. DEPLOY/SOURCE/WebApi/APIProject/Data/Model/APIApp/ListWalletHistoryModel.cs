using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListWalletHistoryModel
    {
        public int id { get; set; }
        public int amount { get; set; }
        public string code { get; set; }
        public DateTime createdDate { get; set; }
        public string content { get; set; }
        public string title { get; set; }
        public int type { get; set; }
        public int typeTransaction { get; set; }
        public int isPlus { get; set; }
        public string icon
        {
            get
            {
                switch (typeTransaction)
                {
                    case Constant.TYPE_TRANSACTION_WITHDRAW:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_WITHDRAW_ICON;

                    case Constant.TYPE_TRANSACTION_REFUND_WITHDRAW:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_REFUND_WITHDRAW_ICON;

                    case Constant.TYPE_TRANSACTION_TRANSFER_WALLET:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_TRANSFER_WALLET_ICON;
                    case Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET_EXCHANGE:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET_EXCHANGE_ICON;

                    case Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET_ICON;

                    case Constant.TYPE_TRANSACTION_RECHARGE:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_RECHARGE_ICON;

                    case Constant.TYPE_TRANSACTION_RECHARGE_ADMIN:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_RECHARGE_ADMIN_ICON;

                    case Constant.TYPE_TRANSACTION_ACCEPT_ORDER:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_ACCEPT_ORDER_ICON;

                    case Constant.TYPE_TRANSACTION_REFUND_ORDER_CANCLE:
                        return Util.getFullUrl() + Constant.TYPE_TRANSACTION_REFUND_ORDER_CANCLE_ICON;
                    default:
                        return Util.getFullUrl() + "/Content/images/logo-weship.png";
                }
            }
        }
    }
}
