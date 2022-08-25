using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ConfigModel
    {
        public string FirstLogin { get; set; }
        public string Profit { get; set; }
        public string TimeWaiting { get; set; }
        public string LimitCatImage { get; set; }
        public string Shift { get; set; }
        public string MinBalanceSendRequest { get; set; }
        public string MinBalanceSendRequestFirst { get; set; }
        public string MinBalanceSendMessage { get; set; }
        public string MaxDistanceSendRequest { get; set; }
        public string DayWithdraw { get; set; }
        public string MinPointWithdraw { get; set; }
        public string MinBalanceWithdraw { get; set; }
        public string TimeDelayOrder { get; set; }
        public string TimeStartOrder { get; set; }
    }
    public class VehicleTypeModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int? IsActive { get; set; }
        public int OrderIndex { get; set; }
        public int IsMotorbike { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
    public class ActiveAreaModel
    {
        public int ID { get; set; }
        public string AreaName { get; set; }
        public int DistrictID { get; set; }//id district
        public string DistrictName { get; set; }//name district
        public string DistrictType { get; set; }//type district
        public int ProvinceID { get; set; }//id province
        public string ProvinceName { get; set; }//name province
        public string ProvinceType { get; set; }//Type province
        public int IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class ConfigCommissionModel
    {
        public int ID { get; set; }
        public string CommissionName { get; set; }
        public int MastersBenefit { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
