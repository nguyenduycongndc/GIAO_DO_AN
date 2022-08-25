using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ConfigCustomerRank
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Policy { get; set; }
        public int ProfitCash { get; set; }
        public int ProfitVPN { get; set; }
        public int MinPoint { get; set; }
        public int MaxPoint { get; set; }
        public Nullable<int> EarnPoint { get; set; }
        public int PointBonus { get; set; }
        public int ProfitExtraBirthDay { get; set; }
        public int Level { get; set; }
        public int IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public string Title { get; set; }

    }
}
