using Data.Model.APIApp;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class HistoryGivePointWebOutputModel
    {
        public int HistoryID { set; get; }
        public int userID { get; set; }
        public string CustomerName { set; get; }
        public int Point { set; get; }
        public string Code { get; set; }
        public DateTime? CreateDate { set; get; }
        public string CreateDateStr
        {
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }

        public int Type { set; get; }
        public int isActive { set; get; }
        public int Status { set; get; }
        public int AfterBalance { set; get; }
        public int BeforeBalance { set; get; }
        public int TransactionType { set; get; }
        public string Tittle { get; set; }
        public string Content { get; set; }
        public string icon { get; set; }
    }
}
