using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class StatisticPaymentOuputModel
    {
        public int ID { get; set; }
        public string code { get; set; }
        public string cusName { get; set; }
        public string shiperName { get; set; }
        public int bookingType { get; set; }
        public DateTime bookingDate { get; set; }
        public int totalPrice { get; set; }
        public int shiperCommission { get; set; }
        public int status { get; set; }
        public int paymentType { get; set; }
    }
}
