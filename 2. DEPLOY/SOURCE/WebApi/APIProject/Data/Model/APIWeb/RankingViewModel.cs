using Data.Utils;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIWeb
{
    public class RankingViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MinPoint { get; set; }
        public int? MaxPoint { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public string RangePoint
        {
            get
            {
                if (MaxPoint.HasValue)
                {
                    return String.Format("{0:n0}", MinPoint) + " - " + String.Format("{0:n0}", MaxPoint.Value);
                }
                else
                {
                    return "> " + String.Format("{0:n0}", MinPoint);
                }
            }
        }
        public string StatusStr
        {
            get
            {
                if (Status == 1)
                {
                    return "Active";
                }
                else
                {
                    return "Deactive";
                }
            }
        }
        public DateTime? CreatedDate { get; set; }
        public string CreatDateSTR { get { return CreatedDate.HasValue ? CreatedDate.Value.ToString(SystemParam.CONVERT_DATETIME_HAVE_HOUR) : ""; } }
    }
    public class RankingDetailViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MinPoint { get; set; }
        public string TitleVi { get; set; }
        public string TitleEn { get; set; }
        public string MinPointStr { get { return String.Format("{0:n0}", MinPoint); } }
        public int? MaxPoint { get; set; }
        public string MaxPointStr { get { return MaxPoint.HasValue ? String.Format("{0:n0}", MaxPoint.Value) : ""; } }
        public string DescriptionEN { get; set; }
        public string DescriptionVI { get; set; }
        public int Status { get; set; }
        public int profitCash { get; set; }
        public int profitVnpay { get; set; }
        public int PointBonus { get; set; }
        public string PointBonusStr { get { return String.Format("{0:n0}", PointBonus); } }
        public int BirthDayProfit { get; set; }
        public List<ServiceBonusFree> lsServiceBonus { get; set; }
        public List<ServiceBonusFree> lsServiceBirthDay { get; set; }
        public string ortherGift { get; set; }
        public int Level { get; set; }
        public string PolicyEN { get; set; }
        public string PolicyVN { get; set; }

    }
    public class ServiceBonusFree
    {
        public int ServiceID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
    }
}
