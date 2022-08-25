using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListServicePriceOutputModel
    {
        public int ID { get; set; }
        public int ServiceID { get; set; }
        public string Name { get; set; }
        public int MyProperty { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int USDPrice { get; set; }
        public List<ListImageService> LstImage { get; set; }
    }

    public class ListImageService
    {
        public string Url { get; set; }
        public int ID { get; set; }
    }

}
