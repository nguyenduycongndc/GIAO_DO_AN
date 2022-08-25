using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CreateTransactionModel
    {
        public List<int> additionService { get; set; }
        public int mainService { get; set; }
        public int CarID { get; set; }
        public int isInDoor { get; set; }
        public string BookingDateInput { get; set; }
        public string placeID { get; set; }
        public string couponCode { get; set; }
        public int comboID { get; set; }
        public int isBookingNow { get; set; }
        public string agentCode { get; set; }
        public string note { get; set; }
       // public int PaymentType = Constant.PAYMENT_TYPE_CASH;
        public int UsePoint { get; set; }
        public int customerID { get; set; }
        public string cusAddress { get; set; }
    }
}
