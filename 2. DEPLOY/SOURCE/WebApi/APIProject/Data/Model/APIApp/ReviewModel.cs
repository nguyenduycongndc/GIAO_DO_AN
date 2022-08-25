using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ReviewModel
    {
        public int ID { get; set; }
        public string WasherCode { get; set; }
        public string WasherName { get; set; }
        public int NumberOfRating { get; set; }
        public int NumberOfCompleteOrder { get; set; }
        public double Rating { get; set; }
        public string RatingOfAdmin { get; set; }
        public int NumberOfComment { get; set; }
        public DateTime? DateTime { get; set; }
        public string DateTimeStr { get { return DateTime.HasValue ? DateTime.Value.ToString(SystemParam.CONVERT_DATETIME) : ""; } }
    }
    public class ReviewInformation
    {
        public string Phone { get; set; }
        public string PhoneSTR { get { return Phone; } }
        public DateTime? DateTime { get; set; }
        public string DateTimeStr { get { return DateTime.HasValue ? DateTime.Value.ToString(SystemParam.CONVERT_DATETIME) : ""; } }
        public int Rating { get; set; }
        public string Note { get; set; }
        public string washerNote { get; set; }
        public string CustomerName { get; set; }
    }
    public class ReviewDetail
    {
        public ReviewModel review { get; set; }
        public List<ReviewInformation> ListReviewInformation { get; set; }

    }
}
