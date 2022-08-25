using Data.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;
using PagedList.Mvc;

namespace Data.Model.APIWeb
{
    public class ShipperDetailOutputModelWeb : ShiperInputModel
    {
        public List<string> lstImgIdentify { get; set; }
        public List<Area> LstAreaShiper { get; set; }
    }
    public class ShipperDetailModel
    {
        public int ID { get; set; }
        public Shiper ship { get; set; }
        public List<ListBankOutputModelWeb> shipListBank { get; set; }
        public IPagedList<OrderOutputModel> lstCustomerOrder { get; set; }
    }
    public class ListBankOutputModelWeb
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string AccountOwner { get; set; }
        public string BankName { get; set; }
        public int BankID { get; set; }
    }
}
