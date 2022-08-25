using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class UpdateCustomerRankModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MinPoint { get; set; }
        public int MaxPoint { get; set; }
        public string Description { get; set; }
        public int ProfitCash { get; set; }
        public int ProfitVPN { get; set; }
        public int PointBonus { get; set; }
        public int ProfitExtraBirthDay { get; set; }
        public string Policy { get; set; }
    }
}
