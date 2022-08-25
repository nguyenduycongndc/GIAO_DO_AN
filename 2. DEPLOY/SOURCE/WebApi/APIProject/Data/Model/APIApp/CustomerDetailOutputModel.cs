using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Data.Model.APIApp
{
    public class CustomerDetailOutputModel
    {
        public int MemberID { get; set; }
        public string Phone { set; get; }
        public int Role { get; set; }
        public string Name { set; get; }
        public DateTime? DOB { set; get; }
        public string dobStr { get; set; }
        public int? Sex { set; get; }
        public string Email { set; get; }
        public string ProvinceName { set; get; }
        public string DistrictName { set; get; }
        public double lati { get; set; }
        public double longi { get; set; }
        public int? ProvinceCode { set; get; }
        public int? DistrictCode { set; get; }
        public string Address { set; get; }
        public int TotalPrice { get; set; }
        public int Type { get; set; }
        public int WithdrawPoint { get; set; }
        public int Point { get; set; }
        public string UrlAvatar { get; set; }
        public int Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int BalanceWalletWithDraw { get; set; }
        public int BalanceWalletNoWithDraw { get; set; }
    }
}