using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class CarBrandOutputAPIModel
    {
        public int id { get; set; }
        public int num_models { get; set; }
        public int max_car_id { get; set; }
        public string img_url { get; set; }
        public string name { get; set; }
        public float avg_horsepower { get; set; }
        public float avg_price { get; set; }
    }
    public class CarModelOutputAPIModel
    {
        public int id { get; set; }
        public int horsepower { get; set; }
        public string make { get; set; }
        public string img_url { get; set; }
        public string model { get; set; }
        public decimal price { get; set; }
        public int year { get; set; }
    }
}
