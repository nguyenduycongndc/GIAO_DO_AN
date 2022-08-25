using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class NewsAppOutputModel
    {
        public int NewsID { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateSTR
        {
            get
            {
                return CreateDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR);
            }
        }
        public string Title { get; set; }
        public int Type { get; set; }
        public string UrlImage { get; set; }
        public string Description { get; set; }
        public int CategoryID { get; set; }
        public string CreateUser { get; set; }
        public int IsShowCreate { get; set; }
        public string Link { get; set; }
    }
    public class GetNewsDetail
    {
        public NewsAppOutputModel newDetail { get; set; }
        public List<NewsAppOutputModel> listNew { get; set; }

    }

    public class CategorynewAndNew
    {
        public object listCateNews { get; set; }
        public object listNews { get; set; }
    }
}
