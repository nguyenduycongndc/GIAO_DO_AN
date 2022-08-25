using Data.DB;
using Data.Model.APIApp;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class PlaceBusiness : GenericBusiness
    {
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        AreaBusiness areaBus = new AreaBusiness();
        public PlaceBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }
        public GooglePlaceAPI GetPalceAPI(string placeID)
        {
            string url = "https://maps.googleapis.com/maps/api/place/details/json?placeid=" + placeID + "&key=" + Constant.KEY_GOOGLE_MAP + "&language=vi";
            GooglePlaceAPI apiURl = JsonConvert.DeserializeObject<GooglePlaceAPI>(apiBus.GetJson(url));
            if (apiURl.status.Equals(Constant.STATUS_SUCCESS))
            {
                return apiURl;
            }
            else
                return null;
        }
        public Province GetProvince(GooglePlaceAPI apiURl)
        {
            if (apiURl != null)
            {
                List<AddressComponents> listPlaceDetail = apiURl.result.address_components;
                AddressComponents Province = listPlaceDetail.Where(u => u.types.Contains(Constant.KEY_PROVINCE)).FirstOrDefault();
                Province pro = cnn.Provinces.Where(u => u.Name.Contains(Province.long_name)).FirstOrDefault();
                return pro;
            }
            return null;
        }

        public District GetDistrict(GooglePlaceAPI apiURl)
        {
            if (apiURl != null)
            {
                List<AddressComponents> listPlaceDetail = apiURl.result.address_components;
                AddressComponents District = listPlaceDetail.Where(u => u.types.Contains(Constant.KEY_DISTRICT)).FirstOrDefault();
                if (District == null)
                    return null;
                string distict = District.long_name;
                if (District.long_name.ToLower().Contains("từ liêm"))
                    distict = "Từ liêm";
                District dist = cnn.Districts.Where(u => u.Name.Contains(distict)).FirstOrDefault();
                return dist;
            }
            return null;
        }
        public LocationDetail GetLocation(GooglePlaceAPI apiURl)
        {
            if (apiURl != null)
                return apiURl.result.geometry.location;
            return null;
        }

        public List<ProvinceModel> getProvince()
        {
            List<int> listProvinceCode = areaBus.ListArea().Select(a => a.ProvinceCode).Distinct().ToList();
            var listProvice = from p in cnn.Provinces
                              where listProvinceCode.Contains(p.Code)
                              select new ProvinceModel
                              {
                                  ProvinceCode = p.Code,
                                  ProvinceName = p.Name,
                                  ProvinceType = p.Type
                              };

            return listProvice.ToList();
        }
        public List<DistrictModel> getAreaByProvince(int ProvinceCode)
        {
            List<int> listDistrictCode = areaBus.ListArea().Select(a => a.DistrictCode).Distinct().ToList();
            var listDistrict = from d in cnn.Districts
                               join a in cnn.Areas
                               on d.Code equals a.DistrictCode
                               where d.ProvinceCode.Equals(ProvinceCode) && listDistrictCode.Contains(d.Code) && a.IsActive.Equals(SystemParam.ACTIVE)
                               orderby d.ProvinceCode
                               select new DistrictModel
                               {
                                   DistrictCode = d.Code,
                                   DistrictName = d.Name,
                                   DistrictType = d.Type,
                                   ProvinceCode = d.ProvinceCode,
                                   AreaCode = a.ID
                               };
            return listDistrict.ToList();
        }
        public string checkArea(string placeID, string lang)
        {
            GooglePlaceAPI placeDetail = GetPalceAPI(placeID);
            for (int i = 0; i < 10; i++)
            {
                if (placeDetail == null)
                    placeDetail = GetPalceAPI(placeID);
                else
                    break;
            }
            if (placeDetail == null)
                return lang.Equals(SystemParam.VN) ? MessVN.PLACE_ERROR : MessEN.PLACE_ERROR;
            District district = GetDistrict(placeDetail);
            if (district == null)
                return lang.Equals(SystemParam.VN) ? MessVN.AREA_ERROR : MessEN.AREA_ERROR;
            Area area = cnn.Areas.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.DistrictCode.Equals(district.Code)).FirstOrDefault();
            if (area == null)
                return lang.Equals(SystemParam.VN) ? MessVN.AREA_ERROR : MessEN.AREA_ERROR;
            return SystemParam.SUCCES_STR;
        }
        public string createLocation(CustomerLocationModel item, int customerID, string lang)
        {
            var place = GetPalceAPI(item.PlaceID);
            if (place == null)
                return lang.Equals(SystemParam.VN) ? MessVN.LOCATION_ERROR : MessEN.LOCATION_ERROR;
            string mess = checkArea(item.PlaceID, lang);
            if (!mess.Equals(SystemParam.SUCCES_STR))
                return mess;
            CustomerLocation cusL = new CustomerLocation();
            cusL.CustomerID = customerID;
            cusL.CreateDate = DateTime.Now;
            cusL.IsActive = SystemParam.ACTIVE;
            cusL.PlaceID = item.PlaceID;
            cusL.Longi = place.result.geometry.location.lng;
            cusL.Lati = place.result.geometry.location.lat;
            cusL.Name = item.Name;
            cusL.Address = item.CustomerAddress;
            cnn.CustomerLocations.Add(cusL);
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;
        }
        public string UpdateLocation(CustomerLocationModel item, string lang)
        {
            var place = GetPalceAPI(item.PlaceID);
            if (place == null)
                return lang.Equals(SystemParam.VN) ? MessVN.LOCATION_ERROR : MessEN.LOCATION_ERROR;
            CustomerLocation cusL = cnn.CustomerLocations.Find(item.ID);
            if (cusL == null)
                return lang.Equals(SystemParam.VN) ? MessVN.NOT_FOUND_MESS : MessEN.NOT_FOUND_MESS;
            string mess = checkArea(item.PlaceID, lang);
            if (!mess.Equals(SystemParam.SUCCES_STR))
                return mess;
            cusL.PlaceID = item.PlaceID;
            cusL.Longi = place.result.geometry.location.lng;
            cusL.Lati = place.result.geometry.location.lat;
            cusL.Name = item.Name;
            cusL.Address = item.CustomerAddress;
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;
        }
        public string Delete(int ID, string lang)
        {

            CustomerLocation cusL = cnn.CustomerLocations.Find(ID);
            if (cusL == null)
                return lang.Equals(SystemParam.VN) ? MessVN.NOT_FOUND_MESS : MessEN.NOT_FOUND_MESS;
            cusL.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;
        }
    }
}
