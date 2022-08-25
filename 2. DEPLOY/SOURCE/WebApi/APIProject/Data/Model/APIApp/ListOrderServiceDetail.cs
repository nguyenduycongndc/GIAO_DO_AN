using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ListOrderServiceDetail
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public int Amount { get; set; }
        public int Type { get; set; }
        public string Note { get; set; }
        public string Toping { get; set; }
    }
}
