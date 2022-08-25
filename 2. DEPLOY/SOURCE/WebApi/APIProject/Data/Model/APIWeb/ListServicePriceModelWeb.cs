using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListServicePriceModelWeb
    {
        public string ServiceStr { get; set; }
        public string ServicePriceStr { get; set; }
        public float BasePrice { get; set; }
        public float Price { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
