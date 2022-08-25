using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class HomeScreenOutPutModel
    {
        public CustomerLogin customerDetail { get; set; }
        public List<NewsAppOutputModel> listBaner { get; set; }
        public List<NewsAppOutputModel> listPromotion { get; set; }
        public object listItem { get; set; }
        public object listCombo { get; set; }
    }
    public class CustomerLogin {

        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public int Point { get; set; }
        public int IsNeedUpate { get; set; }
    }
}
