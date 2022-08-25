using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class CategoryViewModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameVN { get; set; }
        public int Status { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public string StatusStr { 
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
        public bool IsProductCart { get; set; }
    }
}
