using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;

namespace Data.Model.APIWeb
{
    public class ListServiceCategoryModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string _valueIcon { get; set; }
        public int? ParentID { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int OrderDisplay { get; set; }
        public int IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
