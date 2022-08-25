using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CreateServiceInputModel
    {
        public string Code { set; get; }
        public int ID { set; get; }
        public string Name { set; get; }
        public string Price { set; get; }
        public string ImageUrl { set; get; }
        public int CarSegmentID { get; set; }
        public int EstTime { set; get; }
        public int ServiceID { set; get; }
    }
}
