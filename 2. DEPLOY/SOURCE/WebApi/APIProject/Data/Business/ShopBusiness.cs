using APIProject.Models;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PagedList;
using PagedList.Mvc;
using Data.Model.ShopModel;
using System.Device.Location;
using OfficeOpenXml;
using System.IO;

namespace Data.Business
{
    public class ShopBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public string fullUrl = Util.getFullUrl();

        public ShopBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        public JsonResultModel CreateShop(string urlAvatar, string urlCertificate, string Name, string Phone, string Email, int ProvinceId, int DistrictId, string Address, float Long, float Lat)
        {
            try
            {
                int checkPhone = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.User.Equals(Phone) && (c.ShiperID.HasValue || c.ShopID.HasValue)).Count();
                int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && (c.ShiperID.HasValue || c.ShopID.HasValue) && ( c.Shop.Email.Equals(Email) || c.Shiper.Email.Equals(Email))).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                if ( Long <= 0 || Lat <= 0)
                {
                    return rpBus.ErrorResult("Địa chỉ Google Map không hợp lệ", SystemParam.PROCESS_ERROR);
                }
                Shop newShop = new Shop();
                newShop.Name = Name;
                newShop.Phone = Phone;
                newShop.Email = Email;
                newShop.ProvinceID = ProvinceId;
                newShop.DistrictID = DistrictId;
                newShop.Address = Address;
                newShop.Logi = Long;
                newShop.Lati = Lat;
                newShop.Description = "";
                newShop.CreatedDate = DateTime.Now;
                newShop.IsActive = SystemParam.ACTIVE;
                newShop.Rate = 1;
                //Add shop image and shop cetificate image
                newShop.ShopImages = AddShopImage(urlAvatar, urlCertificate);
                //add an account for shop
                List<Member> mb = new List<Member>();
                Member m = new Member();
                m.User = Phone;
                m.DeviceID = "";
                m.Password = Util.GenPass("123456");
                m.Token = "";
                m.IsActive = SystemParam.ACTIVE;
                m.CreatedDate = DateTime.Now;
                m.IsLogin = true;
                m.ExpriceDateOTP = DateTime.Now;
                m.Status = SystemParam.ACTIVE;
                m.QtyOTP = 0;
                m.OTPDateTime = DateTime.Now;
                m.KeyChat = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                m.RoomID = Util.CreateMD5(DateTime.Now.Millisecond.ToString());
                mb.Add(m);
                newShop.Members = mb;
                cnn.Shops.Add(newShop);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return rpBus.serverError();
            }
        }
        public List<ShopImage> AddShopImage(string urlAvatar, string urlCertificate)
        {
            try
            {
                List<ShopImage> lst = new List<ShopImage>();
                ShopImage avatar = new ShopImage();
                avatar.Path = urlAvatar;
                avatar.Type = SystemParam.TYPE_SHOP_IMAGE;
                lst.Add(avatar);
                ShopImage license = new ShopImage();
                license.Path = urlCertificate;
                license.Type = SystemParam.TYPE_SHOP_LICENSE;
                lst.Add(license);
                return lst;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<ShopImage>();
            }

        }
        public IPagedList<ListShopOutputModel> Search(int Page, string searchKey, int? status,int? provinceID,int? districtID, string FromDate, string ToDate)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(FromDate);
                DateTime? endDate = Util.ConvertToDate(ToDate);

                List<ListShopOutputModel> list = new List<ListShopOutputModel>();
                var query = (from c in cnn.Shops
                             where c.Members.Any(m => (m.IsActive == SystemParam.ACTIVE || m.IsActive == SystemParam.DEACTIVE) && m.ShopID == c.ID)
                             && (status.HasValue ? c.Members.Any(m => m.IsActive.Equals(status.Value)) : true)
                             && (startDate.HasValue ? c.CreatedDate >= startDate.Value : true)
                             && (endDate.HasValue ? c.CreatedDate <= endDate.Value : true)
                             && (provinceID.HasValue ? c.ProvinceID.Equals(provinceID.Value) : true)
                             && (districtID.HasValue ? c.DistrictID.Equals(districtID.Value) : true)
                             && (!String.IsNullOrEmpty(searchKey) ? c.Name.ToLower().Contains(searchKey) || c.Phone.Contains(searchKey) || c.Email.Contains(searchKey) : true)
                             orderby c.CreatedDate descending
                             select new ListShopOutputModel()
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 ContactPhone = c.Phone,
                                 Address = c.Address,
                                 Province = c.Province.Name,
                                 District = c.District.Name,
                                 CreateDate = c.CreatedDate,
                                 Email = c.Email,
                                 Status = c.Members.FirstOrDefault().Status,
                                 Rate = (float)c.Rate
                             }).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<ListShopOutputModel>().ToPagedList(1, 1);
            }
        }
        public List<ListShopOutputModel> GetDataListShop(string searchKey, int? status, int? provinceID, int? districtID, string FromDate, string ToDate)
        {
            try
            {
                DateTime? startDate = Util.ConvertFromDate(FromDate);
                DateTime? endDate = Util.ConvertToDate(ToDate);
                var query = (from c in cnn.Shops
                             where c.Members.Any(m => (m.IsActive == SystemParam.ACTIVE || m.IsActive == SystemParam.DEACTIVE) && m.ShopID == c.ID)
                             && (status.HasValue ? c.Members.Any(m => m.IsActive.Equals(status.Value)) : true)
                             && (startDate.HasValue ? c.CreatedDate >= startDate.Value : true)
                             && (endDate.HasValue ? c.CreatedDate <= endDate.Value : true)
                             && (provinceID.HasValue ? c.ProvinceID.Equals(provinceID.Value) : true)
                             && (districtID.HasValue ? c.DistrictID.Equals(districtID.Value) : true)
                             && (!String.IsNullOrEmpty(searchKey) ? c.Name.ToLower().Contains(searchKey) || c.Phone.Contains(searchKey) || c.Email.Contains(searchKey) : true)
                             orderby c.CreatedDate descending
                             select new ListShopOutputModel()
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 ContactPhone = c.Phone,
                                 Address = c.Address,
                                 Province = c.Province.Name,
                                 District = c.District.Name,
                                 CreateDate = c.CreatedDate,
                                 Email = c.Email,
                                 Status = c.Members.FirstOrDefault().Status,
                                 Rate = (float)c.Rate
                             }).ToList();
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new List<ListShopOutputModel>();
            }
        }
        public ExcelPackage ExportListShop(string searchKey,int? status, int? provinceID, int? districtID, string fromDate, string toDate)
        {
            try
            {
                List<ListShopOutputModel> data = GetDataListShop(searchKey, status, provinceID,districtID, fromDate, toDate);
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/listShop.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;
                if (data != null && data.Count() > 0)
                    foreach (var dt in data)
                    {
                        sheet.Cells[row, 1].Value = stt;
                        sheet.Cells[row, 2].Value = dt.Name;
                        sheet.Cells[row, 3].Value = dt.ContactPhone;
                        sheet.Cells[row, 4].Value = dt.Email;
                        sheet.Cells[row, 5].Value = dt.Province;
                        sheet.Cells[row, 6].Value = dt.District;
                        sheet.Cells[row, 7].Value = dt.Address;
                        switch (dt.Status)
                        {
                            case SystemParam.ACTIVE:
                                sheet.Cells[row, 8].Value = "Hoạt động";
                                break;
                            case SystemParam.DEACTIVE:
                                sheet.Cells[row, 8].Value = "Ngừng hoạt động";
                                break;
                            case SystemParam.INACTIVE:
                                sheet.Cells[row, 8].Value = "Tạm đóng cửa";
                                break;
                        }
                        sheet.Cells[row, 9].Value = dt.CreateDate.GetValueOrDefault().ToString(SystemParam.CONVERT_DATETIME);
                        row++;
                        stt++;
                    }
                return pack;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ExcelPackage();
            }
        }

        //Count Shop
        public async Task<int> CountShop()
        {
            var query = cnn.Members.Where(x => x.ShopID != null && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
            return query;

        }
        public async Task<double> countNewShopPercent()
        {
            try
            {
                var today = DateTime.Today;
                var first = new DateTime(today.Year, today.Month, 1);
                double shopLastMonth = cnn.Members.Where(x => x.ShopID != null && x.CreatedDate <= first && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                double shopNow = cnn.Members.Where(x => x.ShopID != null && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                if(shopLastMonth <= 0)
                {
                    return 0;
                }
                double shopNew = shopNow - shopLastMonth;
                return Math.Round((shopNew / shopLastMonth) * 100, 2);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public async Task<List<int>> getShopEveryMonth()
        {
            try
            {
                var today = DateTime.Today;
                var listShopCount = new List<int>();
                for (var i = 1; i <= today.Month; i++)
                {
                    var month = new DateTime(today.Year, i, 1);
                    var last = month.AddMonths(1);
                    var ShopCount = cnn.Members.Where(x => x.ShopID != null && x.CreatedDate <= last && x.IsActive.Equals(SystemParam.ACTIVE)).Count();
                    listShopCount.Add(ShopCount);
                }
                return listShopCount;
            }
            catch (Exception e)
            {
                return new List<int>();
            }
        }
        public ListShopOutputModel loadModalEditShop(int ID)
        {
            try
            {
                var query = (from c in cnn.Shops
                             where c.Members.Any(m => m.IsActive == SystemParam.ACTIVE && m.ShopID == c.ID)
                             && c.ID == ID
                             select new ListShopOutputModel()
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 Email = c.Email,
                                 Phone = c.Phone,
                                 Address = c.Address,
                                 Province = c.Province.Name,
                                 District = c.District.Name,
                                 CreateDate = c.CreatedDate,
                                 Long = c.Logi,
                                 Lati = c.Lati,
                                 ProvinceID = c.ProvinceID,
                                 DistrictID = c.DistrictID,
                                 Avartar = c.ShopImages.Where(i => i.Type == 1).FirstOrDefault().Path,
                                 License = c.ShopImages.Where(i => i.Type == 2).FirstOrDefault().Path,
                                 Status = c.Members.Where(m => m.ShopID == ID).FirstOrDefault().IsActive
                             }).FirstOrDefault();
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new ListShopOutputModel();
            }
        }


        public JsonResultModel UpdateShopInfo(ListShopOutputModel input)
        {
            try
            {
                Member mb = cnn.Members.Where(m => m.ShopID == input.ID).FirstOrDefault();
                if (mb == null)
                {
                    return rpBus.ErrorResult("Không tìm thấy đối tượng!", SystemParam.PROCESS_ERROR);
                }
                int checkPhone = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.ShopID != input.ID && (c.ShiperID.HasValue || c.ShopID.HasValue) && c.User.Equals(input.Phone) ).Count();
                int checkMail = cnn.Members.Where(c => c.IsActive > SystemParam.ACTIVE_FALSE && c.ShopID != input.ID && (c.ShiperID.HasValue || c.ShopID.HasValue) && (c.Shop.Email.Equals(input.Email) || c.Shiper.Email.Equals(input.Email))).Count();
                if (checkPhone > 0)
                    return rpBus.ErrorResult(MessVN.PHONE_USED, SystemParam.PROCESS_ERROR);
                if (checkMail > 0)
                    return rpBus.ErrorResult(MessVN.EMAIL_USED, SystemParam.PROCESS_ERROR);
                if (input.Long <= 0 || input.Lati <= 0)
                {
                    return rpBus.ErrorResult("Địa chỉ Google Map không hợp lệ", SystemParam.PROCESS_ERROR);
                }
                //Xóa hết ảnh cũ của shop
                List<ShopImage> lstImg = cnn.ShopImages.Where(i => i.ShopID.Equals(input.ID)).ToList();
                cnn.ShopImages.RemoveRange(lstImg);
                mb.Shop.Name = input.Name;
                mb.Shop.Phone = input.Phone;
                mb.Shop.Email = input.Email;
                mb.Status = input.Status;
                mb.Shop.Address = input.Address;
                mb.Shop.ProvinceID = input.ProvinceID;
                mb.Shop.DistrictID = input.DistrictID;
                mb.Shop.Logi = input.Long;
                mb.Shop.Lati = input.Lati;
                //Thêm lại ảnh của shop
                List<ShopImage> img = new List<ShopImage>();
                ShopImage si = new ShopImage();
                //Thêm lại ảnh đại diện
                si.Path = input.Avartar;
                si.ShopID = input.ID;
                si.Type = SystemParam.TYPE_SHOP_IMAGE;
                img.Add(si);
                ShopImage sim = new ShopImage();
                //Thêm ảnh giấy phép kinh doanh
                sim.Path = input.License;
                sim.ShopID = input.ID;
                sim.Type = SystemParam.TYPE_SHOP_LICENSE;
                img.Add(sim);
                mb.Shop.ShopImages = img;
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public bool CheckExistShop(string Phone)
        {
            try
            {
                var shop = cnn.Shops.Where(s => s.Members.Any(p => p.IsActive == SystemParam.ACTIVE) && s.Phone == Phone).FirstOrDefault();
                if (shop != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return false;
            }
        }

        public JsonResultModel getMenuByShop(int? serviceID, int? isActive, int shopID, int type = 1)
        {
            if (type == 1)
            {
                List<MenuShopModel> data = cnn.Services
                    .Where(u =>
                    (serviceID.HasValue ? u.ID.Equals(serviceID.Value) : true) &&
                    u.ServicePrices.Where(m => m.ShopID.Equals(shopID) && (isActive.HasValue ? m.IsActive.Equals(isActive.Value) : (m.IsActive != SystemParam.ACTIVE_FALSE))).Count() > 0
                    ).Select(u => new MenuShopModel
                    {
                        ID = u.ID,
                        Name = u.Name,
                        Data = u.ServicePrices
                        .Where(m => m.ShopID.Equals(shopID) && (isActive.HasValue ? m.IsActive.Equals(isActive.Value) : (m.IsActive != SystemParam.ACTIVE_FALSE)))
                        .Select(m => new MenuByCategoryModel
                        {
                            ID = m.ID,
                            Name = m.Name,
                            Description = m.Description,
                            IsActive = m.IsActive,
                            Price = m.Price,
                            BasePrice = m.BasePrice,
                            Type = m.Type,
                            CategoryID = u.ID,
                            CategoryName = u.Name,
                            Images = m.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => fullUrl + i.Image).ToList(),
                        }).ToList()
                    }).ToList();

                return rpBus.SuccessResult("", data);
            }
            List<MenuByCategoryModel> dataToping = cnn.ServicePrices.Where(m => m.ShopID.Equals(shopID) && (isActive.HasValue ? m.IsActive.Equals(isActive.Value) : (m.IsActive != SystemParam.ACTIVE_FALSE)))
                        .Select(m => new MenuByCategoryModel
                        {
                            ID = m.ID,
                            Name = m.Name,
                            Price = m.Price,
                            IsActive = m.IsActive,
                            Description = m.Description,
                            BasePrice = m.BasePrice,
                            Type = m.Type,
                            CategoryID = m.ServiceID.HasValue ? m.ServiceID.Value : 0,
                            CategoryName = m.Service != null ? m.Service.Name : "",
                            Images = m.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => fullUrl + i.Image).ToList(),
                        }).ToList();
            return rpBus.SuccessResult("", dataToping);
        }

        public JsonResultModel getServiceDetail(int ID, int shopID)
        {
            MenuByCategoryModel dataToping = cnn.ServicePrices.Where(m => m.ID.Equals(ID) && m.ShopID.Equals(shopID) && m.IsActive != SystemParam.ACTIVE_FALSE)
                        .Select(m => new MenuByCategoryModel
                        {
                            ID = m.ID,
                            Name = m.Name,
                            Price = m.Price,
                            BasePrice = m.BasePrice,
                            Discount = m.Discount,
                            Description = m.Description,
                            IsActive = m.IsActive,
                            Type = m.Type,
                            CategoryID = m.ServiceID.HasValue ? m.ServiceID.Value : 0,
                            CategoryName = m.Service != null ? m.Service.Name : "",
                            Images = m.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => fullUrl + i.Image).ToList(),
                        }).FirstOrDefault();
            if (dataToping == null)
            {
                return rpBus.ErrorResult("Dữ liệu không tồn tại", 103);
            }
            return rpBus.SuccessResult("", dataToping);
        }
        public IPagedList<ShopHistoryBusinessModel> searchHistoryBusiness(int page, int ShopID)
        {
            try
            {
                var res = (from c in cnn.OrderServices
                           where c.ShopID == ShopID
                           orderby c.CreatedDate
                           select new ShopHistoryBusinessModel()
                           {
                               CustomerName = c.Customer.Name,
                               OrderServiceID = c.ID,
                               CreatedDate = c.CreatedDate,
                               CompletedDate = c.CompletedDate,
                               Status = c.Status,
                               TotalPrice = c.TotalPrice
                           }).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return res;
            }
            catch (Exception ex)
            {
                ex.ToString();
                ravenClient.Capture(new SentryEvent(ex));
                return null;
            }
        }
        public IPagedList<ListServicePriceModelWeb> searchServicePrice(int page, int ShopID)
        {
            try
            {
                var res = (from c in cnn.ServicePrices
                           where c.ShopID == ShopID && c.IsActive == SystemParam.ACTIVE
                           orderby c.CreatedDate
                           select new ListServicePriceModelWeb()
                           {
                               ServiceStr = c.Service.Name,
                               ServicePriceStr = c.Name,
                               Price = c.Price,
                               CreatedDate = c.CreatedDate
                           }).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return res;
            }
            catch (Exception ex)
            {
                ex.ToString();
                ravenClient.Capture(new SentryEvent(ex));
                return null;
            }
        }
        public int DeleteShop(int ID)
        {
            try
            {
                Member mb = cnn.Members.Where(m => m.ShopID == ID).FirstOrDefault();
                mb.IsActive = SystemParam.INACTIVE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public int InActiveShop(int ID)
        {
            try
            {
                Member m = (from mem in cnn.Members where mem.ShopID == ID select mem).FirstOrDefault();
                m.IsActive = 2; //shop ngừng hoạt động
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.ERROR;
            }
        }
        public JsonResultModel deleteService(int ID)
        {
            var dataToping = cnn.ServicePrices.Where(m => m.ID.Equals(ID)).FirstOrDefault();
            if (dataToping == null)
            {
                return rpBus.ErrorResult("Dữ liệu không tồn tại", 103);
            }
            dataToping.IsActive = SystemParam.ACTIVE_FALSE;
            cnn.SaveChanges();
            return rpBus.SuccessResult("Xóa thành công", "");
        }
        public JsonResultModel createService(ServicePriceDetailModel input, int shopID)
        {
            ServicePrice servicePrice = new ServicePrice
            {
                Name = input.Name,
                Price = input.Price,
                BasePrice = input.BasePrice,
                Discount = input.BasePrice - input.Price,
                Code = Util.CreateMD5(DateTime.Now.ToString()).Substring(8),
                CreatedDate = DateTime.Now,
                Description = input.Description,
                IsActive = input.IsActive,
                ServiceID = input.ServiceID,
                Type = input.ServiceType,
                ShopID = shopID,
                ServiceImages = input.Images.Select(u => new ServiceImage
                {
                    CreateDate = DateTime.Now,
                    Image = u,
                    IsActive = SystemParam.ACTIVE,
                    Type = 1,
                }).ToList()
            };
            cnn.ServicePrices.Add(servicePrice);
            cnn.SaveChanges();
            return getServiceDetail(servicePrice.ID, shopID);
        }

        public JsonResultModel updateService(ServicePriceDetailModel input, int shopID)
        {
            ServicePrice servicePrice = cnn.ServicePrices.Where(u => u.ID.Equals(input.ID)).FirstOrDefault();
            var serviceImage = cnn.ServiceImages.Where(u => u.ServiceID.Equals(input.ID)).ToList();
            var serviceImageAdd = input.Images.Select(u => new ServiceImage
            {
                CreateDate = DateTime.Now,
                Image = u,
                IsActive = SystemParam.ACTIVE,
                Type = 1,
                ServiceID = input.ID
            }).ToList();

            servicePrice.Name = input.Name;
            servicePrice.Price = input.Price;
            servicePrice.BasePrice = input.BasePrice;
            servicePrice.Discount = input.BasePrice - input.Price;
            servicePrice.Description = input.Description;
            servicePrice.ServiceID = input.ServiceID;
            servicePrice.IsActive = input.IsActive;
            servicePrice.Type = input.ServiceType;
            cnn.ServiceImages.RemoveRange(serviceImage);
            cnn.SaveChanges();
            cnn.ServiceImages.AddRange(serviceImageAdd);
            cnn.SaveChanges();

            return getServiceDetail(servicePrice.ID, shopID);
        }
        public JsonResultModel checkCreateOrUpdate(ServicePriceDetailModel input, int shopID)
        {
            if (input.ID > 0)
            {
                var servicePrice = cnn.ServicePrices.Where(u => u.ID.Equals(input.ID) && u.ShopID.Equals(shopID)).FirstOrDefault();
                if (servicePrice == null)
                {
                    return rpBus.ErrorResult("Dữ liệu không tồn tại", 500);
                }
                if (input.ServiceID.HasValue)
                {
                    var service = cnn.Services.Where(u => u.ID.Equals(input.ServiceID.Value) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                    if (service == null)
                    {
                        return rpBus.ErrorResult("Danh mục sản phẩm không tồn tại", 500);
                    }
                }
                return updateService(input, shopID);
            }
            return createService(input, shopID);
        }
        public JsonResultModel changeStatus(int status , int shopID)
        {
            var shop = cnn.Members.FirstOrDefault(x => x.ShopID.Value.Equals(shopID) && x.IsActive.Equals(SystemParam.ACTIVE));
            if(shop == null)
            {
                return rpBus.ErrorResult("Dữ liệu không tồn tại", 500);
            }
            shop.Status = status;
            cnn.SaveChanges();
            return rpBus.SuccessResult(MessVN.SUCCESS_STR, "");
        }

        /// <summary>
        /// Lấy chi tiết shop cho app
        /// </summary>
        /// <param name="shopID"></param>
        /// <param name="lati"></param>
        /// <param name="longi"></param>
        /// <returns></returns>
        public JsonResultModel GetShopDetail(int shopID, float lati, float longi)
        {
            try
            {

                ShopOutputModel data = new ShopOutputModel();
                Shop s = cnn.Shops.Find(shopID);
                data.ShopID = s.ID;
                data.ShopName = s.Name;
                data.ContactPhone = s.Phone;
                data.ContactMail = s.Email;
                data.CountRate = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.IsRateShop.Value.Equals(SystemParam.ACTIVE) && o.ShopID.Value == s.ID).Count();
                var shopRate = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE) && o.IsRateShop.Value.Equals(SystemParam.ACTIVE) && o.ShopID.Value == shopID).Sum(x => x.RateShop) ?? 0;
                data.Rate = data.CountRate != 0 ? (shopRate/data.CountRate) : 0;
                data.Address = s.Address;
                data.Longi = s.Logi;
                data.Lati = s.Lati;
                data.Distance = (float)new GeoCoordinate(lati, longi).GetDistanceTo(new GeoCoordinate(s.Lati, s.Logi)) / 1000;
                data.Status = s.Members.Where(x => x.ShopID == s.ID).Select(x => x.Status).FirstOrDefault();
                data.Avatar = fullUrl + (s.ShopImages.Any(si => si.ShopID.Equals(s.ID) && si.Type.Equals(SystemParam.TYPE_IMAGE)) ?
                    s.ShopImages.Where(si => si.Type.Equals(SystemParam.TYPE_IMAGE) && si.ShopID.Equals(s.ID)).FirstOrDefault().Path : "");
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        /// <summary>
        /// Lấy chi tiết review shop
        /// </summary>
        /// <param name="shopID"></param>
        /// <returns></returns>
        public JsonResultModel GetReviewShop(int shopID, int? rateNumber)
        {
            try
            {
                int value = 0;
                if (rateNumber.HasValue)
                    value += rateNumber.Value;
                if (value > 5)
                    value = 5;
                GetReviewShopOutputModel data = new GetReviewShopOutputModel();

                List<OrderService> od = cnn.OrderServices.Where(o => o.IsActive.Equals(SystemParam.ACTIVE)
                && o.ShopID == shopID && o.IsRateShop == SystemParam.ACTIVE
                && (rateNumber.HasValue && rateNumber > 0 ? o.Rate >= rateNumber.Value
                && o.Rate <= value : true)).ToList();
                if (od.Count() == 0)
                    return rpBus.SuccessResult("Shop chưa có lượt đánh giá nào!", null);

                data.sumStar1 = od.Where(o => o.RateShop <= 1).Count();
                data.sumStar2 = od.Where(o => o.RateShop >= 2 && o.RateShop < 3).Count();
                data.sumStar3 = od.Where(o => o.RateShop >= 3 && o.RateShop < 4).Count();
                data.sumStar4 = od.Where(o => o.RateShop >= 4 && o.RateShop < 5).Count();
                data.sumStar5 = od.Where(o => o.RateShop >= 5).Count();
                data.listMemberReview = od.Select(o => new ListMemerReview
                {
                    rateNumber = (float)o.RateShop,
                    memberName = o.Customer.Name,
                    note = o.NoteRateShop
                }).ToList();
                data.countVote = od.Count();
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
