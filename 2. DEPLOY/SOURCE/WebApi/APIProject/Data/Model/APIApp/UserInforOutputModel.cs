using Data.DB;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class UserInforOutputModel : CustomerDetailOutputModel
    {
        public string RefCode { get; set; }
        public string MyRefCode { get; set; }
        public double Rate { get; set; }
        public string WithdrawNote { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifyDateSTR { get { return ModifyDate.HasValue ? ModifyDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : ""; } }
        public int UserID { get; set; }
        public string RankName { get; set; }
        public int RankLevel { get; set; }
        public int RankUperPoint { get; set; }
        public int RankPoint { get; set; }
        public int UsePoint { get; set; }
        public string ContentRank { get; set; }
        public string Token { get; set; }
        public object ShiperArea { get; set; }
        public string Avatar { get; set; }
        public string KeyChat { get; set; }
        public string RoomID { get; set; }
        public List<CarOutputModel> ListCar { get; set; }
        public List<CustomerLocationModel> listLocation { get; set; }
        public object ListBank { get; set; }
        public int isNeedUpdate { get; set; }
        public string PassWord { get; set; }
        public int? AcceptService { get; set; }
        public int IsVip { get; set; }
        public double VipDiscount { get; set; }
        public string Code { get; set; }
        public int Commission { get; set; }
        public int CancelOrder { get; set; }
        public string LicensePlates { get; set; }
        public string CarColor { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int IsAcceptService { get; set; }
        public int IsInternal { get; set; }
    }
}
