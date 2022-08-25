using APIProject.Models;
using Data.DB;
using Data.Model;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class AddressBusiness : GenericBusiness
    {
        public AddressBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public JsonResultModel GetListProvince()
        {
            var model = cnn.Provinces.Select(x => new AddressModel
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();
            return rpBus.SuccessResult(MessVN.SUCCESS_STR, model); ;
        }
        public JsonResultModel GetListDistrict(int ProvinceID)
        {
            var model = cnn.Districts.Where(x => x.ProvinceID.Equals(ProvinceID)).Select(x => new AddressModel
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();
            return rpBus.SuccessResult(MessVN.SUCCESS_STR, model); ;
        }
    }
}
