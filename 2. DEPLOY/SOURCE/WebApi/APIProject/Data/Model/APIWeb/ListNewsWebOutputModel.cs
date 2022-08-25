using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListNewsWebOutputModel
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public string UrlImage { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public int TypeSend { get; set; }
        public string Name { set; get; }
        public string Link { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public List<ListNewsRelate> ListNewsRelate { get; set; }
        public List<ListAdvertisement> ListAdvertisement { get; set; }
        public int IsActive { get; set; }
        public int OrderDisplay { get; set; }
        public string CreateUser { get; set; }
        public int UserID { get; set; }
        public int IsBanner { get; set; }
        public int ShowCreate { get; set; }
    }

    public class ListNewsRelate
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class ListAdvertisement
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
