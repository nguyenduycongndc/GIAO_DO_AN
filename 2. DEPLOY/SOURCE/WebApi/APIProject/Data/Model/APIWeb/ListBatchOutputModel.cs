using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListBatchOutputModel
    {
        public int ID { get; set; }
        public string BatchCode { get; set; }
        public string BatchNameVN { get; set; }
        public string BatchNameEN { get; set; }
        public string CreateUserName { get; set; }
        public int UsedQTY { set; get; }
        public DateTime? CreateDate { set; get; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }

    }
}
