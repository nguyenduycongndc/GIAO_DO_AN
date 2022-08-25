using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class TokenOutputModel
    {
        public int? CustomerID { get; set; }
        public int? AgentID { get; set; }
        public int Type
        {
            get
            {
                return CustomerID.HasValue ? Constant.CUSTOMER_ROLE : Constant.SHIPER_ROLE;
            }
        }
        public int MemberID { get; set; }
        public string Lang { get; set; }
    }
}
