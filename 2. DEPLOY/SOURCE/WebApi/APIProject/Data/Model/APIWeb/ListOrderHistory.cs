using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb {

    public class ListOrderHistory {
        public OrderService orderService { get; set; }
        public Customer customer { get; set; }
        public ServicePrice servicePrice { get; set; }
    }
}
