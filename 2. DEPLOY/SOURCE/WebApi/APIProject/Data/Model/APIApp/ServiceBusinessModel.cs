using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ServiceBusinessModel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string NameEN { get; set; }
        public string NameVN { get; set; }
        public int EstTimeFrom { get; set; }
        public int EstTimeTo { get; set; }
        public int Type { get; set; }
        public int DisplayOrder { get; set; }
        public int IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
    }
    public class ServiceDetailModel : ServiceBusinessModel
    {
        public List<ImageServiceModel> ListImage { get; set; }
        public int Discount { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public string DescriptionEN { get; set; }
        public string Color { get; set; }
        public List<JobDetail> Listjob { get; set; }
        public List<ServicePriceSegment> ListServicePrice { get; set; }
        public List<MainServiceAdditionServiceModel> ListMainServiceAdditionService { get; set; }
    }
    public class ImageServiceModel
    {
        public int ID { get; set; }
        public string url { get; set; }
        public int Type { get; set; }
        public int IsActive { get; set; }
    }
    public class MainServiceAdditionServiceModel {
        public int? MainServiceID { get; set; }
        public int? AddtionServiceID { get; set; }
        public string ServieName { get; set; }
    }
    public class ServicePriceSegment
    {
        public int ID { get; set; }
        public int SegmentID { get; set; }
        public string SegmentName { get; set; }
        public int Time { get; set; }
        public int Price { get; set; }
        public double USDPrice { get; set; }
        public string ServiceName { get; set; }
    }
    public class JobDetail
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
        public int IsActive { get; set; }
    }

    public class MainServiceFilter
    {
        public int? ServiceID { get; set; }
        public string ServiceName { get; set; }
        public int? comboID { get; set; }
    }
}
