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
using System.Data.Entity.Validation;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class ServiceCategoryBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        string hostUrl = Util.getFullUrl();
        public ServiceCategoryBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        //Thêm mới
        public JsonResultModel CreateServiceCategory(string Name, int Order, string img, string code)
        {
            try
            {
                var checkData = cnn.Services.Where(m => m.Code == code).Count();
                if (checkData != 0)
                {
                    return rpBus.ErrorResult(MessVN.ERROR_CODE, SystemParam.PROCESS_ERROR);
                }

                Service service = new Service();
                service.Name = Name;
                service.OrderDisplay = Order;
                service.Icon = img;
                service.Code = code;
                service.Type = 1;
                service.IsActive = 1;
                service.CreateDate = DateTime.Now;
                cnn.Services.Add(service);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, service);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }

        public JsonResultModel GetListServiceCategory(float? lati, float? longi)
        {
            try
            {
                ListServiceCategory data = new ListServiceCategory();
                data.lstServiceCategory = cnn.Services.Where(s => s.IsActive.Equals(SystemParam.ACTIVE))
                    .Select(s => new ListCategory
                    {
                        ID = s.ID,
                        Name = s.Name,
                        Icon = hostUrl + s.Icon
                    }).OrderByDescending(s => s.ID).ToList();

                data.ListBaner = cnn.News.Where(n => n.IsActive.Equals(SystemParam.ACTIVE) && n.Type.Equals(SystemParam.NEWS_TYPE_BANER_FOOD))
                    .Select(n => new ListBanerFood
                    {
                        ID = n.ID,
                        Image = hostUrl + n.UrlImage
                    }).ToList();

                if (lati.HasValue && longi.HasValue)
                    data.ListHotItem = (from s in cnn.Shops
                               join m in cnn.Members on s.ID equals m.ShopID
                               join si in cnn.ShopImages on s.ID equals si.ShopID
                               where m.IsActive.Equals(SystemParam.ACTIVE) && si.Type.Equals(SystemParam.TYPE_SHOP_IMAGE)
                               group new { s, si,m } by s.ID into g
                               select new
                               {
                                   ID = g.FirstOrDefault().s.ID,
                                   Description = g.FirstOrDefault().s.Description,
                                   ShopName = g.FirstOrDefault().s.Name,
                                   Rating = g.FirstOrDefault().s.Rate,
                                   ImageUrl = hostUrl + g.FirstOrDefault().si.Path,
                                   Lati = g.FirstOrDefault().s.Lati,
                                   Logi = g.FirstOrDefault().s.Logi,
                                   Status = g.FirstOrDefault().m.Status
                               }).AsEnumerable().Select(x => new ListHotItem
                               {
                                   ID = x.ID,
                                   Description = x.Description,
                                   ShopName = x.ShopName,
                                   Rating = x.Rating,
                                   ImageUrl = x.ImageUrl,
                                   Distance = (float)new GeoCoordinate(lati.Value, longi.Value).GetDistanceTo(new GeoCoordinate(x.Lati, x.Logi)) / 1000,
                                   Status = x.Status
                               }).Where(x => x.Distance <= 50).OrderByDescending(x => x.Status).ThenBy(x=> x.Distance).ToList();

                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }

        }

        //Tìm kiếm
        public IPagedList<ListServiceCategoryModel> SearchServiceCategory(int Page, string Name, string FromDate, string ToDate, int? IsActive)
        {
            try
            {
                DateTime? startDate = Util.ConvertDate(FromDate);
                DateTime? endDate = Util.ConvertDate(ToDate);
                if (endDate.HasValue)
                    endDate = endDate.Value.AddDays(1);
                var data = cnn.Services.Where(m => m.IsActive != 0
                    && ((!String.IsNullOrEmpty(Name) ? m.Name.Contains(Name) : true)
                    || (!String.IsNullOrEmpty(Name) ? m.Code.Contains(Name) : true))
                    && (IsActive != null ? m.IsActive == IsActive : true)
                    && (startDate.HasValue ? m.CreateDate >= startDate.Value : true)
                    && (endDate.HasValue ? m.CreateDate <= endDate.Value : true)
                    ).Select(LiS => new ListServiceCategoryModel
                    {
                        ID = LiS.ID,
                        Code = LiS.Code,
                        Name = LiS.Name,
                        Icon = LiS.Icon,
                        ParentID = LiS.ParentID,
                        Description = LiS.Description,
                        Type = LiS.Type,
                        OrderDisplay = LiS.OrderDisplay,
                        IsActive = LiS.IsActive,
                        CreatedDate = LiS.CreateDate
                    }).OrderByDescending(LiS => LiS.CreatedDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<ListServiceCategoryModel>().ToPagedList(1, 1);
            }
        }
        //binding theo id
        public ListServiceCategoryModel GetCategoryInfo(int id)
        {
            try
            {
                ListServiceCategoryModel data = new ListServiceCategoryModel();
                Service mb = cnn.Services.Find(id);

                var checkCategory = cnn.Services.Where(m => m.ID == id).FirstOrDefault();

                //Trường hợp login là shiper
                if (checkCategory != null)
                {
                    data.ID = checkCategory.ID;
                    data.Name = checkCategory.Name;
                    data.Code = checkCategory.Code;
                    data.Description = checkCategory.Description;
                    data.CreatedDate = checkCategory.CreateDate;
                    data.Icon = hostUrl + checkCategory.Icon;
                    data._valueIcon = checkCategory.Icon;
                    data.IsActive = checkCategory.IsActive;
                    data.Type = checkCategory.Type;
                    data.ParentID = checkCategory.ParentID;
                    data.OrderDisplay = checkCategory.OrderDisplay;
                }

                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ListServiceCategoryModel();
            }
        }
        //lưu khi sửa
        public bool EditCategory(ListServiceCategoryModel request)
        {
            Service sv = cnn.Services.Find(request.ID);
            //var checkCategory = cnn.Services.Where(m => m.ID == request.ID).FirstOrDefault();

            if (sv != null)
            {
                sv.Name = string.IsNullOrEmpty(request.Name) ? "" : request.Name;
                //sv.Code = string.IsNullOrEmpty(request.Code) ? "" : request.Code;
                sv.Icon = string.IsNullOrEmpty(request.Icon) ? "" : request.Icon;
                sv.IsActive = request.IsActive;
                sv.OrderDisplay = request.OrderDisplay;
            }

            cnn.SaveChanges();
            return true;
        }
        //xóa
        public JsonResultModel DeleteServiceCategory(int id)
        {
            try
            {
                Service services = cnn.Services.Find(id);
                services.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                //return SystemParam.SUCCESS;
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, services);
            }
            catch (Exception ex)
            {
                //return SystemParam.ERROR;
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }
    }
}
