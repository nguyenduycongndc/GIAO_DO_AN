using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    class ListBankOutputModel
    {
        public int balance { get; set; }
        public List<ListBank> listBankInfo { get; set; }
    }

    public class ListBank
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string AccountOwner { get; set; }
        public string BankName { get; set; }
        public int BankID { get; set; }
    }
}
