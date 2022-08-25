using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    class GetReviewShopOutputModel
    {
        public int sumStar1 { get; set; }
        public int sumStar2 { get; set; }
        public int sumStar3 { get; set; }
        public int sumStar4 { get; set; }
        public int sumStar5 { get; set; }
        public int countVote { get; set; }
        public List<ListMemerReview> listMemberReview { get; set; }
    }

    public class ListMemerReview
    {
        public float rateNumber { get; set; }
        public string memberName { get; set; }
        public string note { get; set; }
    }
}
