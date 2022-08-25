using Data.Model.APIApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CustomerDetailViewModel
    {
        public ListCustomerOutputModel CustomerInfo { get; set; }
        public List<MemberTransactionViewModel> MemberTransaction { get; set; }
        public List<CarCustomerViewModel> CarCustomer { get; set; }

        public List<ReviewInformation> Ratedetail { get; set; }
    }
}
