using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class ServicePriceBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        string hostUrl = Util.getFullUrl();
        public ServicePriceBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public IPagedList<ListShopOuputModel> GetlistShopByCategory(int page, int limit, int cateID, float lati, float longi, int type)
        {
            try
            {
                //type = 1 sắp xếp theo các shop gần nhấ, 2 sắp xếp các shop bán chạy nhất
                List<int> lstShopID = cnn.ServicePrices.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.ServiceID == cateID).GroupBy(s => s.ShopID).Select(s => s.FirstOrDefault().ShopID).ToList();
                //List<Shop> lstShop = cnn.Shops.Where(s => lstShopID.Contains(s.ID) && s.Members.Any(m => m.IsActive.Equals(SystemParam.ACTIVE))).ToList();
                var lstShop = (from s in cnn.Shops
                               join m in cnn.Members on s.ID equals m.ShopID
                               join si in cnn.ShopImages on s.ID equals si.ShopID
                               where m.IsActive.Equals(SystemParam.ACTIVE) && si.Type.Equals(SystemParam.TYPE_SHOP_IMAGE)
                               select new
                               {
                                   ID = s.ID,
                                   Avartar = si.Path,
                                   Description = s.Description,
                                   Name = s.Name,
                                   Rating = s.Rate,
                                   Lati = s.Lati,
                                   Logi = s.Logi,
                                   Status = m.Status,
                                   shop = s,
                                   countOrder = s.OrderServices.Count(),
                                   CountRate = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.IsRateShop.Value.Equals(SystemParam.ACTIVE) && o.ShopID.Value == s.ID).Count(),
                                   SumRate = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.IsRateShop.Value.Equals(SystemParam.ACTIVE) && o.ShopID.Value == s.ID).Sum(x => x.RateShop) ?? 0
                               }).ToList();
                var data = lstShop.Select(s => new ListShopOuputModel
                {
                    id = s.ID,
                    avatar = hostUrl + s.Avartar,
                    description = s.Description,
                    name = s.Name,
                    lati = s.Lati,
                    longi = s.Logi,
                    countOrder = s.countOrder,
                    rating = s.CountRate != 0 ? (s.SumRate / s.CountRate) : 0,
                    status = s.Status,
                    distance = (float)new GeoCoordinate(lati, longi).GetDistanceTo(new GeoCoordinate(s.Lati, s.Logi)) / 1000,
                    ListServicePrice = cnn.ServicePrices.Where(x => x.ShopID.Equals(s.ID) && x.Type.Equals(SystemParam.SERVICE_TYPE_MAIN) && x.IsActive.Equals(SystemParam.ACTIVE) ).Select(x => new ServicePriceModel
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Price = x.Price,
                        BasePrice = x.BasePrice,
                        UrlImage = hostUrl + x.ServiceImages.Select(si => si.Image).FirstOrDefault()
                    }).Take(5).ToList()
                }).Where(x => x.distance <= SystemParam.SHOP_MAX_DISTANCE && lstShopID.Contains(x.id));
                data = type.Equals(1) ? data.Where(x => x.distance <= SystemParam.SHOP_MAX_DISTANCE).OrderByDescending(s => s.status).ThenBy(s => s.distance) : data.OrderByDescending(s => s.status).ThenByDescending(s => s.countOrder);
                return data.ToPagedList(page, limit);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListShopOuputModel>().ToPagedList(1, 1);
            }
        }

        public IPagedList<ListShopOuputModel> GetListShopByFood(int page, int limit, float lati, float longi, string searchKey)
        {
            try
            {
                var data = (from s in cnn.Shops
                            join sp in cnn.ServicePrices on s.ID equals sp.ShopID into spl
                            from sp in spl.DefaultIfEmpty()
                            join m in cnn.Members on s.ID equals m.ShopID
                            join si in cnn.ShopImages on s.ID equals si.ShopID
                            where m.IsActive.Equals(SystemParam.ACTIVE) && m.Status.Equals(SystemParam.ACTIVE) && sp.IsActive.Equals(SystemParam.ACTIVE) && si.Type.Equals(SystemParam.TYPE_SHOP_IMAGE) &&
                               (!String.IsNullOrEmpty(searchKey) ? (sp.Name.Contains(searchKey) || s.Name.Contains(searchKey)) : true) 
                               group new { s, m } by s.ID into sg 
                            select new 
                            {
                                id = sg.Select(x => x.s.ID).FirstOrDefault(),
                                avatar = hostUrl + sg.Select(x => x.s.ShopImages.Select(si => si.Path).FirstOrDefault()).FirstOrDefault(),
                                description = sg.Select(x => x.s.Description).FirstOrDefault(),
                                name = sg.Select(x => x.s.Name).FirstOrDefault(),
                                rating = sg.Select(x=> x.s.Rate).FirstOrDefault(),
                                latitude = sg.Select(x => x.s.Lati).FirstOrDefault(),
                                longitude = sg.Select(x => x.s.Logi).FirstOrDefault(),
                                status = sg.Select(x => x.m.Status).FirstOrDefault(),
                                CountRate = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.IsRateShop.Value.Equals(SystemParam.ACTIVE) && o.ShopID.Value == sg.Select(x => x.s.ID).FirstOrDefault()).Count(),
                                SumRate = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.IsRateShop.Value.Equals(SystemParam.ACTIVE) && o.ShopID.Value == sg.Select(x => x.s.ID).FirstOrDefault()).Sum(x => x.RateShop) ?? 0
            }).AsEnumerable().Select(s => new ListShopOuputModel
                            {
                                id = s.id,
                                avatar = s.avatar,
                                description = s.description,
                                lati = s.latitude,
                                longi = s.longitude,
                                name = s.name,
                                rating = s.CountRate != 0 ? (s.SumRate / s.CountRate) : 0,
                                distance = (float)new GeoCoordinate(lati, longi).GetDistanceTo(new GeoCoordinate(s.latitude, s.longitude)) / 1000,
                                status = s.status,
                                ListServicePrice = cnn.ServicePrices.Where(x => x.ShopID.Equals(s.id) && x.Type.Equals(SystemParam.SERVICE_TYPE_MAIN) && x.IsActive.Equals(SystemParam.ACTIVE)  && (!String.IsNullOrEmpty(searchKey) ? x.Name.Contains(searchKey) : true)).Select(x => new ServicePriceModel
                                {
                                    ID = x.ID,
                                    Name = x.Name,
                                    Price = x.Price,
                                    BasePrice = x.BasePrice,
                                    UrlImage = hostUrl + x.ServiceImages.Select(si => si.Image).FirstOrDefault()
                                }).Take(5).ToList()
                            }).Where(x => x.distance <= SystemParam.SHOP_MAX_DISTANCE).OrderByDescending(x => x.status).ThenBy(x => x.distance).ToPagedList(page, limit);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListShopOuputModel>().ToPagedList(1, 1);
            }
        }
        public IPagedList<ListServicePriceOutputModel> GetListServicePrice(int page, int limit, string searchKey, int? cateID, int? shopID, int type)
        {
            try
            {
                List<ServiceImage> listServiceImage = cnn.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && (cateID.HasValue ? i.ServicePrice.ServiceID == cateID : true)
                && (shopID.HasValue ? i.ServicePrice.ShopID == shopID : true) && (!String.IsNullOrEmpty(searchKey) ? i.ServicePrice.Name.Contains(searchKey) : true) && i.ServicePrice.Type == type).ToList();

                var data = cnn.ServicePrices.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && (cateID.HasValue && cateID.Value > 0 ? s.ServiceID.Equals(cateID.Value) : true)
                 && (!String.IsNullOrEmpty(searchKey) ? s.Name.Contains(searchKey) : true) && (shopID.HasValue ? s.ShopID == shopID : true) && s.Type == type)
               .Select(s => new ListServicePriceOutputModel
               {
                   ID = s.ID,
                   Name = s.Name,
                   Price = s.Price,
                   BasePrice = s.BasePrice,
                   USDPrice = s.USDPrice,
                   CreatedDate = s.CreatedDate,
                   LstImage = listServiceImage.Where(im => im.ServiceID.Equals(s.ID)).Select(im => new ListImageService
                   {
                       ID = im.ID,
                       Url = hostUrl + im.Image
                   }).ToList(),
                   Code = s.Code
               }).OrderByDescending(s => s.ID).ToPagedList(page, limit);

                    
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ListServicePriceOutputModel>().ToPagedList(1, 1);
            }
        }

        //Lấy chi tiết dịch vụ

        public JsonResultModel GetServicePriceDetail(int id)
        {
            try
            {
                ServicePrice s = cnn.ServicePrices.Find(id);
                ListServicePriceOutputModel data = new ListServicePriceOutputModel();
                data.ID = s.ID;
                data.Name = s.Name;
                data.Price = s.Price;
                data.BasePrice = s.BasePrice;
                data.USDPrice = s.USDPrice;
                data.CreatedDate = s.CreatedDate;
                data.Description = s.Description;
                data.LstImage = s.ServiceImages.Where(im => im.IsActive.Equals(SystemParam.ACTIVE)).Select(im => new ListImageService
                {
                    ID = im.ID,
                    Url = im.Image
                }).ToList();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
    }
}
