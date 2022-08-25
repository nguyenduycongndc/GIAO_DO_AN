using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ProductViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public int ProductID { get; set; }
        public string NameEN { get; set; }
        public string NameVN { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public int QTY { get; set; }
        public int BasePrice { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int Status { get; set; }
        public string StatusStr
        {
            get
            {
                if (Status == 1)
                {
                    return "Active";
                }
                else
                {
                    return "Deactive";
                }
            }
        }
        public List<string> ListImage { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
