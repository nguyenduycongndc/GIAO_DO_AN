using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class BankOutputModel
    {
        public string BankName { get; set; }
        public int BankID { get; set; }
        public string Code { get; set; }
    }
    public class BankDetailModel : BankOutputModel
    {
        public int BankIDShiper { get; set; }
        public string Account { get; set; }
        public string AcountOwner { get; set; }
        public int MemberID { get; set; }
    }
}
