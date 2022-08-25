using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class ListCustomerOutputModel
    {

        public int CustomerID { set; get; }
        public int? CarCustomerID { set; get; }
        public string CarBrand { set; get; }
        public string CarModel { set; get; }
        public string Code { get; set; }

        public int IsVip { get; set; }
        public string RefCode { get; set; }
        public string RefCustomer { get; set; }
        public int? RefCustomerID { get; set; }
        public int RankingPoint { get; set; }
        public DateTime RankingDate { get; set; }
        public string RankingDateStr { get { return RankingDate.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR); } }
        public string RankingName { get; set; }
        public int RankID { get; set; }
        public string CustomerName { set; get; }
        public string PhoneNumber { set; get; }
        public DateTime? DOB { set; get; }
        public string DOBStr
        {
            set { }
            get
            {
                return DOB.HasValue ? DOB.Value.ToString(SystemParam.CONVERT_DATETIME) : "";
            }
        }
        public string Email { set; get; }
        public int? Point { get; set; }
        public string TypeLoginStr { set; get; }
        public int? Status { get; set; }
        public string StatusStr
        {
            get
            {
                if (Status == SystemParam.ACTIVE)
                {
                    return "Active";
                }
                else
                {
                    return "Deactive";
                }
            }
        }
        public DateTime? CreateDate { set; get; }
        public int? ProvinceCode { get; set; }
        public int? DistrictCode { get; set; }
        public string CreateDateStr
        {
            set { }
            get
            {
                return CreateDate.HasValue ? CreateDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : "";
            }
        }
        public int? Sex { set; get; }
        public string Address { set; get; }
        public int order { get; set; }
        public double? Rate { get; set; }
        public int revenua { get { return revenuaCombo.GetValueOrDefault() + revenuaPackage.GetValueOrDefault(); } }
        public int? revenuaCombo { get; set; }
        public int? revenuaPackage { get; set; }
        public int? QTYCancel { get; set; }
        public string revenuaSTR { get { return string.Format("{0:#,0}", revenua); } }
        public string PointStr { get { return string.Format("{0:#,0}", Point); } }
        public string ProvinceName { get; set; }
       // public string Code_Province { get { return Util.GetCodeInProvince(Code, ProvinceName); } }
    }
}
