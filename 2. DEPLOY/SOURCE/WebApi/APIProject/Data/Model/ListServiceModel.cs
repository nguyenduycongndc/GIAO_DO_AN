using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model
{
    public class ListServiceModels
    {
        public List<int> listServiceFree { get; set; }
        public List<int> listServiceFreeUpRank { get; set; }
        public List<int> listServiceUsed { get; set; }
    }
    public class AdditionServiceExtra {
        public List<int> additionService { get; set; }
        public int orderServiceID { get; set; }

    }
}
