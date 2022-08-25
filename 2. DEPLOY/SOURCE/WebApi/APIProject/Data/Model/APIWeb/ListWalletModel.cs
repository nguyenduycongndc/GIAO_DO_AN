using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListWalletModel
    {
        public int ID { get; set; }
        public int Code { get; set; }
        public string Username { get; set; }
        public int Icon { get; set; }
        public int Usertype { get; set; }
        public int MemberID { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int Balance { get; set; }
        public int Type { get; set; }
        public int IsActive { get; set; }
    }

    public class ViewListWalletModel
    {
        public int? CusId { get; set; }
        public string CusCode { get; set; }
        public string CusName { get; set; }
        public string CusPhone { get; set; }
        public int? TypeCustomer { get; set; }
        public int? PileWallet { get; set; }
        public int? IncomeWallet { get; set; }
        public long? WithDrawSum { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int IsActive { get; set; }
        //public int? MemId { get; set; }
        //public int? MemType { get; set; }
        //public int? MemTransactionType { get; set; }
        //public int? MemPoint { get; set; }
        //public int? MemAfterBalance { get; set; }
        //public DateTime? CreateDate { get; set; }
    }
    public class TransactionHistoryWalletModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int WalletType { get; set; }
        public int MemType { get; set; }
        public int TransactionType { get; set; }
        public int? Point { get; set; }
        public int? AfterBalance { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
