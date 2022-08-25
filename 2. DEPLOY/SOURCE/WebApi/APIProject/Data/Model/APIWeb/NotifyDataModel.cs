using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class NotifyDataModel
    {
        public int id { get; set; }
        public int type { get; set; }
        public string code { get; set; }
        public string content { get; set; }
        public DateTime timeEnd { get; set; }
    }
    public class NotiAdminModel
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Count { get; set; }
        public string Type { get; set; }
        public int IsActive { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class NotiAdminDetail : NotiAdminModel
    {
        public List<ListMember> listMember { get; set; }
    }

    public class ListMember
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public string Phone { get; set; }

    }

}
