using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using PagedList.Mvc;
using Data.DB;
using Data.Model.APIApp;

namespace Data.Model.APIWeb
{
    public class CustomerDetailOutputModelWeb
    {
        public Member MemberInfo { get; set; }
        public Customer CustomerInfo { get; set; }
        public IPagedList<CustomerDetailOutputModel> lstCustomerOrder { get; set; }
        public IPagedList<HistoryGivePointWebOutputModel> lstCustomerPoint { get; set; }
    }
}
