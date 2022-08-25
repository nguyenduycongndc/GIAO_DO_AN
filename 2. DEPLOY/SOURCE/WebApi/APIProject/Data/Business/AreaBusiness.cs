using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class AreaBusiness : GenericBusiness
    {
        public AreaBusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public List<AreaOutputModel> ListArea()
        {
            return cnn.Areas.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new AreaOutputModel
            {
                ID = u.ID,
                Name = u.Name,
                DistrictCode = u.DistrictCode,
                DistrictName = u.District.Name,
                ProvinceCode = u.District.ProvinceCode
            }).ToList();
        }

        // laasy chi tiet cua area
        public AreaOutputModel GetAreaByDistrict(int districtCode)
        {
            return cnn.Areas.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.DistrictCode.Equals(districtCode)).Select(u => new AreaOutputModel
            {
                ID = u.ID,
                Name = u.Name,
                DistrictCode = u.DistrictCode,
                DistrictName = u.District.Name,
                ProvinceCode = u.District.ProvinceCode
            }).FirstOrDefault();
        }

        public List<AreaOutputModel> GetAreaByProvince(int provinceCode)
        {
            return cnn.Areas.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.District.ProvinceCode.Equals(provinceCode)).Select(u => new AreaOutputModel
            {
                ID = u.ID,
                Name = u.Name,
                DistrictCode = u.DistrictCode,
                DistrictName = u.District.Name,
                ProvinceCode = u.District.ProvinceCode
            }).ToList();
        }
        public object createArea(string Name, int districtCode)
        {
            Area area = new Area();
            area.Name = Name;
            area.DistrictCode = districtCode;
            area.IsActive = SystemParam.ACTIVE;
            area.CreateDate = DateTime.Now;
            cnn.Areas.Add(area);
            cnn.SaveChanges();
            return ListArea();
        }

        public object create()
        {
            List<District> listDistrict = cnn.Districts.Where(u => u.ProvinceCode.Equals(1)).ToList();
            foreach (District district in listDistrict)
            {
                Area area = new Area();
                area.Name = district.Name;
                area.DistrictCode = district.Code;
                area.IsActive = SystemParam.ACTIVE;
                area.CreateDate = DateTime.Now;
                cnn.Areas.Add(area);
            }
            cnn.SaveChanges();
            return ListArea();
        }
        public bool AddAreaForAgent(List<int> listAgentID, int areaID)
        {
            List<int> listAgentAreaID = cnn.AgentAreas.Where(u => u.AgentID.Equals(areaID) && u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => u.AgentID).ToList();
            List<Agent> ListAgent = cnn.Agents.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && listAgentID.Contains(u.ID) && !listAgentAreaID.Contains(u.ID)).ToList();
            foreach (Agent agent in ListAgent)
            {
                AgentArea agentArea = new AgentArea();
                agentArea.AgentID = agent.ID;
                agentArea.AreaID = areaID;
                agentArea.IsActive = SystemParam.ACTIVE;
                agentArea.CreateDate = DateTime.Now;
                cnn.AgentAreas.Add(agentArea);
            }
            cnn.SaveChanges();
            return true;
        }
        public List<AgentArea> AddListArea(List<int> listAgentID)
        {
            List<AgentArea> output = new List<AgentArea>();
            if (listAgentID != null)
            {
                var listAreaActive = ListArea().Select(u => u.ID).ToList();
                foreach (int areaID in listAgentID)
                {
                    if (listAreaActive.Contains(areaID))
                    {
                        AgentArea agentArea = new AgentArea();
                        agentArea.AreaID = areaID;
                        agentArea.IsActive = SystemParam.ACTIVE;
                        agentArea.CreateDate = DateTime.Now;
                        output.Add(agentArea);
                    }
                }
            }
            return output;
        }
    }
}
