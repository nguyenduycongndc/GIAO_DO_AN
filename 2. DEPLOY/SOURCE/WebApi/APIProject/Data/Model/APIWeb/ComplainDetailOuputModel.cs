using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ComplainDetailOuputModel
    {
        public int ID { get; set; }
        public string cusName { get; set; }
        public string cusPhone { get; set; }
        public string cusAddress { get; set; }
        public string ShiperName { get; set; }
        public string ShiperPhone { get; set; }
        public string FinishAddress { get; set; }
        public int BookingType { get; set; }
        public string UserCancel { get; set; }
        public string ReasonCancel { get; set; }
        public string ShiperNote { get; set; }
        public string AdminNote { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? CompleteDate { get; set; }
        public  int PaymentType { get; set; }
        public int UsePoint { get; set; }
        public int CommissionValue { get; set; }
        public int TotalPrice { get; set; }
        public int BasePrice { get; set; }
        public double? Rateting { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
