using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ReviewInputModel
    {
        public int orderServiceID { get; set; }
        public double point { get; set; }
        public string Note { get; set; }
    }
}
