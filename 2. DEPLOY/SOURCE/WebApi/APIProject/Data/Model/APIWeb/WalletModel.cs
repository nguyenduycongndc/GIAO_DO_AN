using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class WalletModel
    {
        public int ID { get; set; }
        public string WasherCode { get; set; }
        public string WasherName { get; set; }
        // ví cọc
        public int DepositPoint { get; set; }
        public string DepositPointSTR { get { return string.Format("{0:#,0}", DepositPoint); } }
        // ví thu nhập
        public int CashPoint { get; set; }
        public string CashPointSTR { get { return string.Format("{0:#,0}", CashPoint); } }
        public int? TotalWithdraw { get; set; }
        public string TotalWithdrawSTR { get { return TotalWithdraw.HasValue ? string.Format("{0:#,0}", TotalWithdraw.Value) : "0"; } }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }
    public class MemberWalletModel
    {
        public WalletModel Detail { get; set; }
        public List<TransactionMemberDetail> ListTransaction { get; set; }

    }
    public class TransactionMemberDetail
    {
        public DateTime CreateDate { get; set; }
        public string CreateDateSTR { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
        public int TypeWallet { get; set; }

        public int Amount { get; set; }
        public int TypeTransaction { get; set; }
        public int Type { get; set; }
        public string AmountSTR
        {
            get
            {
                if (Type.Equals(Constant.TRANSACTION_ADD_POINT))
                    return "+" + string.Format("{0:#,0}", Amount);
                else
                    return "-" + string.Format("{0:#,0}", Amount);
            }
        }

    }
}
