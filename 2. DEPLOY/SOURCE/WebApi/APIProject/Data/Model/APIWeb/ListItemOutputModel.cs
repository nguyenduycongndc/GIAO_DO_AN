using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Utils;
namespace Data.Model.APIWeb
{
    public class ServiceComboModel : ComboModel
    {
        public List<PackageSerModel> listAllMainService { get; set; }
        public List<PackageSerModel> listMainService { get; set; }
        public string Service
        {
            get
            {
                string output = "";
                foreach (PackageSerModel package in listMainService)
                {
                    if (listMainService.ElementAt(0).Equals(package))
                    {
                        output = listAllMainService.Where(u => u.ServiceID.Equals(package.ServiceID)).Count() + " " + package.Name;
                    }
                    else
                    {
                        output += " - " + listAllMainService.Where(u => u.ServiceID.Equals(package.ServiceID)).Count() + " " + package.Name;
                    }
                }
                return output;
            }
        }
        public string AdditionServiceName { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }

    public class ComboModel
    {
        public int ID { get; set; }
        public int IsActive { get; set; }
    }

    public class ComboDetailModel : ComboModel
    {
        public string NameEN { get; set; }
        public string NameVN { get; set; }

        public string Code { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string DescriptionEN { get; set; }
        public int Discount { get; set; }
        public List<PackageSerModel> AdditionService { get; set; }
        public List<ImageService> ListIamge { get; set; }
        public List<PackageDetailModel> ListPackageService { get; set; }
        public List<ComboPrice> ListComboServicePrice { get; set; }
    }
    public class PackageSerModel
    {
        public int ServiceID { get; set; }
        public int IsActive { get; set; }
        public string Name { get; set; }
    }
    public class PackageDetailModel : PackageSerModel
    {
        public int Count { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class ComboPrice
    {
        public int? ID { get; set; }
        public string SegmentName { get; set; }
        public int SegmentID { get; set; }
        public int BasePrice { get; set; }
        public double USDBasePrice { get; set; }
        public string PriceStr { get { return String.Format("{0:n0}", BasePrice); } }
        public string USDBasePriceStr { get { return String.Format("{0:n0}", USDBasePrice); } }
    }
}
