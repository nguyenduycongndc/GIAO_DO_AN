using Data.DB;
using Data.Model;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class OrtherBusiness : GenericBusiness
    {
        public OrtherBusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public List<string> getTheReason(string lang, int type)
        {
            List<string> output = cnn.Reasons.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Value.Equals(type)).Select(u => lang.Equals(SystemParam.VN) ? u.ContentVN : u.ContentEN).ToList();
            return output;
        }
        public SystemResult GetListCommission(string search = "")
        {
            List<CommissionModel> query = cnn.ConfigCommissions.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Name.Contains(search)).Select(u => new CommissionModel
            {
                ID = u.ID,
                Name = u.Name,
                CarAutocareAcaDemy = u.CarAutocareAcaDemy,
                Duration = u.Duration,
                MastersBenefit = u.MastersBenefit.Value,
                Process = u.Process
            }).ToList();
            return resultBus.SucessResult(query);
        }
        public SystemResult UpdateCommission(CommissionModel item)
        {
            ConfigCommission config = cnn.ConfigCommissions.Find(item.ID);
            if (config == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            config.CarAutocareAcaDemy = item.CarAutocareAcaDemy;
            config.Name = item.Name;
            config.Duration = item.Duration;
            config.MastersBenefit = item.MastersBenefit;
            config.Process = item.Process;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult AddCommission(CommissionModel item)
        {
            ConfigCommission config = new ConfigCommission();
            config.CarAutocareAcaDemy = item.CarAutocareAcaDemy;
            config.Name = item.Name;
            config.Duration = item.Duration;
            config.MastersBenefit = item.MastersBenefit;
            config.Process = item.Process;
            config.IsActive = SystemParam.ACTIVE;
            config.CreateDate = DateTime.Now;
            cnn.ConfigCommissions.Add(config);
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult DeleteCommision(int ID)
        {
            ConfigCommission config = cnn.ConfigCommissions.Find(ID);
            if (config == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            config.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult SearchServiceArea(int? provinceID, int? districtID)
        {
            List<ServiceAreaModel> query = new List<ServiceAreaModel>();
            query = cnn.Areas.Where(u => !u.IsActive.Equals(SystemParam.INACTIVE) && (provinceID.HasValue ? u.District.ProvinceCode.Equals(provinceID.Value) : true) && (districtID.HasValue ? u.DistrictCode.Equals(districtID.Value) : true)).Select(u => new ServiceAreaModel
            {
                ID = u.ID,
                IsActive = u.IsActive,
                CreateDate = u.CreateDate,
                DistrictID = u.DistrictCode,
                DistrictName = u.District.Name,
                ProvinceName = u.District.Province.Name,
                ProvinceID = u.District.ProvinceCode,
            }).OrderByDescending(u => u.CreateDate).ToList();
            return resultBus.SucessResult(query);
        }
        public SystemResult UpdateServiceArea(ServiceAreaModel item)
        {
            Area area = cnn.Areas.Find(item.ID);
            if (area == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            area.IsActive = item.IsActive;
            area.DistrictCode = item.DistrictID;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult AddServiceArea(ServiceAreaModel item)
        {
            Area area = new Area();
            District district = cnn.Districts.Find(item.DistrictID);
            if (district == null)
                return resultBus.ErrorResult("District " + MessEN.NOT_FOUND_MESS);
            area.IsActive = item.IsActive;
            area.DistrictCode = item.DistrictID;
            area.Name = district.Name;
            area.CreateDate = DateTime.Now;
            cnn.Areas.Add(area);
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult DeleteArea(int ID)
        {
            Area area = cnn.Areas.Find(ID);
            if (area == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            area.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
       
    }
}
