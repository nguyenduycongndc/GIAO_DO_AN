using Data.DB;
using Data.Model;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class ServiceBusiness : GenericBusiness
    {
        public ServiceBusiness(WE_SHIPEntities context = null) : base()
        {
            if (context != null)
            {
                this.cnn = context;
            }
        }
        AdditionServiceBusiness addBus = new AdditionServiceBusiness();


        public List<CategoryModel> getListCategory(string lang)
        {
            try
            {
                List<CategoryModel> listCategory = new List<CategoryModel>();
                var query = from c in cnn.CategoryNews
                            where c.IsActive == SystemParam.ACTIVE
                            select new CategoryModel
                            {
                                CategoryID = c.ID,
                                Name = lang.Equals(SystemParam.VN) ? c.NameVN : c.NameEN,
                            };

                if (query != null && query.Count() > 0)
                {
                    listCategory = query.ToList();
                    return listCategory;
                }
                else
                    return listCategory;
            }
            catch (Exception)
            {
                return new List<CategoryModel>();
            }

        }


        public ListItemModel GetListService(int? CarID, int type, string lang, int? mainServiceID = null, int? orderID = null)
        {
            ListItemModel data = new ListItemModel();
            string url = Constant.HTTP + HttpContext.Current.Request.Url.Host;
            List<ServiceOutputModel> listService = new List<ServiceOutputModel>();
            List<ServiceComboOutputModel> listServiceCombo = new List<ServiceComboOutputModel>();
            if (!CarID.HasValue)
            {

                listService = (from s in cnn.Services
                               where s.IsActive.Equals(SystemParam.ACTIVE)
                               orderby s.OrderDisplay.Value descending
                               select new ServiceOutputModel
                               {
                                   ServiceID = s.ID,
                                   Description = lang.Equals(SystemParam.VN) ? s.Description : s.DescriptionEN,
                                   Type = s.Type,
                                   Icon = s.Icon,
                                   ImageUrl = s.ServiceImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.ORTHER_IMAGE)).Select(u => (String.IsNullOrEmpty(u.Image) || u.Image.Length == 0 || u.Image.Contains(Constant.HTTP)) ? u.Image : url + u.Image).ToList(),
                                   CateName = lang.Equals(SystemParam.VN) ? s.NameVN : s.NameEN,
                                   MainImage = s.ServiceImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.MAIN_IMAGE)).Select(u => (String.IsNullOrEmpty(u.Image) || u.Image.Length == 0 || u.Image.Contains(Constant.HTTP)) ? u.Image : url + u.Image).FirstOrDefault(),
                                   Thumbnail = s.ServiceImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.THUMBNAIL_IMAGE)).Select(u => (String.IsNullOrEmpty(u.Image) || u.Image.Length == 0 || u.Image.Contains(Constant.HTTP)) ? u.Image : url + u.Image).FirstOrDefault(),
                               }).ToList();
                listServiceCombo = (from sc in cnn.ServiceComboes
                                    where sc.IsActive.Equals(SystemParam.ACTIVE)
                                    orderby sc.ID descending
                                    select new ServiceComboOutputModel
                                    {
                                        ComboID = sc.ID,
                                        Description = lang.Equals(SystemParam.VN) ? sc.Depcription : sc.DescriptionEN,
                                        Icon = sc.icon,
                                        ImageUrl = sc.ServiceImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.ORTHER_IMAGE)).Select(u => (String.IsNullOrEmpty(u.Image) || u.Image.Length == 0 || u.Image.Contains(Constant.HTTP)) ? u.Image : url + u.Image).ToList(),
                                        CateName = lang.Equals(SystemParam.VN) ? sc.NameVN : sc.NameEN,
                                        CountItem = sc.ServiceComboDetails.Count,
                                        MainImage = sc.ServiceImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.MAIN_IMAGE)).Select(u => (String.IsNullOrEmpty(u.Image) || u.Image.Length == 0 || u.Image.Contains(Constant.HTTP)) ? u.Image : url + u.Image).FirstOrDefault(),
                                        Thumbnail = sc.ServiceImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.THUMBNAIL_IMAGE)).Select(u => (String.IsNullOrEmpty(u.Image) || u.Image.Length == 0 || u.Image.Contains(Constant.HTTP)) ? u.Image : url + u.Image).FirstOrDefault(),
                                        listServiceID = cnn.ServiceComboDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.ServiceComboID.Equals(sc.ID)).Select(s => s.ServiceID).ToList(),
                                    }).ToList();
            }
            else
            {
                CarCustomer car = cnn.CarCustomers.Where(u => u.ID.Equals(CarID.Value) && u.CarModel.CarSegmentID.HasValue && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
                var config = cnn.Configs.ToList();
                int beforeBirthDay = int.Parse(config.Where(u => u.NameConstant.Equals("BeforeBirthDay")).FirstOrDefault().ValueConstant);
                int afterBirthDay = int.Parse(config.Where(u => u.NameConstant.Equals("AfterBirthDay")).FirstOrDefault().ValueConstant);
                List<ServiceBonusCustomer> listServiceDOB = CreateServiceBonusBirthDay(beforeBirthDay, afterBirthDay, car.CustomerID);
                orderID = orderID.HasValue ? orderID.Value : 0;
                ListServiceModels List = GetListServiceOrder(orderID.Value, car);
                List<int> listServiceFree = List.listServiceFree;
                List<int> listServiceUsed = List.listServiceUsed;
                int segmentID = car.CarModel.CarSegmentID.Value;
                if (car != null)
                {
                    var service = (cnn.ServicePrices
                         .Where(u => u.CarSegmentID.Equals(segmentID) && u.IsActive.Equals(SystemParam.ACTIVE) && u.Service.IsActive.Equals(SystemParam.ACTIVE))
                         .OrderBy(u => u.Service.OrderDisplay.Value).ThenByDescending(u => u.ID)
                         ).ToList();
                    listService = service.Select(u => new ServiceOutputModel
                    {
                        Description = lang.Equals(SystemParam.VN) ? u.Service.Description : u.Service.DescriptionEN,
                        ServiceID = u.ServiceID.Value,
                        Icon = u.Service.Icon,
                        CateName = lang.Equals(SystemParam.VN) ? u.Service.NameVN : u.Service.NameEN,
                        Type = u.Service.Type,
                        IsUsed = listServiceUsed.Contains(u.ServiceID.Value) ? 1 : 0,
                        IsFree = listServiceFree.Contains(u.ServiceID.Value) ? 1 : 0,
                        estTime = u.EstTime.Value,
                        ImageUrl = u.Service.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.ORTHER_IMAGE)).Select(i => (String.IsNullOrEmpty(i.Image) || i.Image.Length == 0 || i.Image.Contains(Constant.HTTP)) ? i.Image : url + i.Image).ToList(),
                        Price = u.Price.HasValue ? u.Price.Value : 0,
                        BasePrice = u.BasePrice.HasValue ? u.BasePrice.Value : 0,
                        USDPrice = u.USDPrice.HasValue ? u.USDPrice.Value : 0,
                        USDBasePrice = u.USDBasePrice.HasValue ? u.USDBasePrice.Value : 0,
                        Discount = u.BasePrice.HasValue ? u.Discount.Value : 0,
                        MainImage = u.Service.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.MAIN_IMAGE)).Select(i => (String.IsNullOrEmpty(i.Image) || i.Image.Length == 0 || i.Image.Contains(Constant.HTTP)) ? i.Image : url + i.Image).FirstOrDefault(),
                        Thumbnail = u.Service.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.THUMBNAIL_IMAGE)).Select(i => (String.IsNullOrEmpty(i.Image) || i.Image.Length == 0 || i.Image.Contains(Constant.HTTP)) ? i.Image : url + i.Image).FirstOrDefault(),
                    }).ToList();
                    listServiceCombo = (from scS in cnn.ServiceComboSegments
                                        join sc in cnn.ServiceComboes on scS.ServiceComboID equals sc.ID
                                        where scS.IsActive.Equals(SystemParam.ACTIVE) && scS.CarSegmentID.Equals(car.CarModel.CarSegmentID.Value) && sc.IsActive.Equals(SystemParam.ACTIVE)
                                        orderby scS.ID descending
                                        select new ServiceComboOutputModel
                                        {
                                            ComboID = scS.ServiceComboID,
                                            Description = lang.Equals(SystemParam.VN) ? sc.Depcription : sc.DescriptionEN,
                                            ImageUrl = sc.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.ORTHER_IMAGE)).Select(i => (String.IsNullOrEmpty(i.Image) || i.Image.Length == 0 || i.Image.Contains(Constant.HTTP)) ? i.Image : url + i.Image).ToList(),
                                            CateName = lang.Equals(SystemParam.VN) ? sc.NameVN : sc.NameEN,
                                            Price = scS.Price,
                                            Icon = scS.ServiceCombo.icon,
                                            BasePrice = scS.BasaPrice.Value,
                                            USDPrice = scS.USDPrice.HasValue ? scS.USDPrice.Value : 0,
                                            USDBasePrice = scS.USDBasePrice.HasValue ? scS.USDBasePrice.Value : 0,
                                            Discount = scS.Discount.Value,
                                            CountItem = sc.ServiceComboDetails.Count,
                                            MainImage = sc.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.MAIN_IMAGE)).Select(i => (String.IsNullOrEmpty(i.Image) || i.Image.Length == 0 || i.Image.Contains(Constant.HTTP)) ? i.Image : url + i.Image).FirstOrDefault(),
                                            Thumbnail = sc.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Type.Equals(Constant.THUMBNAIL_IMAGE)).Select(i => (String.IsNullOrEmpty(i.Image) || i.Image.Length == 0 || i.Image.Contains(Constant.HTTP)) ? i.Image : url + i.Image).FirstOrDefault(),
                                            listServiceID = cnn.ServicePrices.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.CarSegmentID.Equals(car.CarModel.CarSegmentID.Value) && sc.ServiceComboDetails.Select(u => u.ServiceID).ToList().Contains(s.ServiceID.Value)).Select(s => s.ID).ToList()
                                        }).ToList();
                }
            }
            data.listInput = listService.Where(u => u.Type.Equals(type)).ToList();
            data.listInputCombo = listServiceCombo.Select(u => new ServiceComboOutputModel
            {
                ComboID = u.ComboID,
                ImageUrl = u.ImageUrl,
                Description = u.Description,
                listServiceID = u.listServiceID,
                CateName = u.CateName,
                CountItem = u.CountItem,
                MainImage = u.MainImage,
                BasePrice = u.BasePrice,
                Discount = u.Discount,
                Thumbnail = u.Thumbnail,
                USDPrice = u.USDPrice,
                USDBasePrice = u.USDBasePrice,
                Price = u.Price,
                Icon = u.Icon,
                listServices = u.listServiceID != null ? listService.Where(s => u.listServiceID.Contains(s.ServiceID)).ToList() : null
            }).ToList();
            if (mainServiceID.HasValue && type.Equals(Constant.TYPE_ADDITION_SERVICE))
            {
                List<int> listID = cnn.MainServiceAdditionServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.MainServiceID.Equals(mainServiceID.Value)).Select(u => u.AdditionServiceID).ToList();
                data.listInput = listService.Where(u => u.Type.Equals(type) && listID.Contains(u.ServiceID)).ToList();
            }
            return data;
        }

        public ListServiceModels GetListServiceOrder(int? orderID, CarCustomer car)
        {
            ListServiceModels query = new ListServiceModels();
            List<int> listServiceUsed = cnn.OrderServiceDetails.Where(u => orderID.HasValue ? u.OrderItemID.Equals(orderID.Value) : false).Select(u => u.ServicePrice.ServiceID.Value).ToList();
            List<int> listServiceFree = cnn.ServiceBonusCustomers.Where(u => u.CustomerID.Equals(car.CustomerID) && u.Status.Equals(0) && u.Type.Equals((int)TypeServiceBonus.Extra) && u.ExpriceTime >= DateTime.Today).Select(u => u.ServiceID).ToList();
            query.listServiceFreeUpRank = listServiceFree;
            List<int> listServiceFreeDOB = new List<int>();
            if (car.Customer.DOB.HasValue)
            {
                var config = cnn.Configs.ToList();
                int beforeBirthDay = int.Parse(config.Where(u => u.NameConstant.Equals("BeforeBirthDay")).FirstOrDefault().ValueConstant);
                int afterBirthDay = int.Parse(config.Where(u => u.NameConstant.Equals("AfterBirthDay")).FirstOrDefault().ValueConstant);
                if (DateTime.Today.AddDays(-beforeBirthDay) <= car.Customer.DOB.Value && car.Customer.DOB.Value >= DateTime.Today.AddDays(afterBirthDay))
                {
                    List<ServiceBonusCustomer> lsservice = cnn.ServiceBonusCustomers.Where(u => u.CustomerID.Equals(car.CustomerID) && u.Type.Equals((int)TypeServiceBonus.BirthDay) && u.CreateDate <= car.Customer.DOB.Value && u.ExpriceTime >= car.Customer.DOB).ToList();
                    List<int> listServiceDoB = new List<int>();
                    if (lsservice == null || lsservice.Count == 0)
                    {
                        List<ServiceBonusCustomer> listServiceDOB = CreateServiceBonusBirthDay(beforeBirthDay, afterBirthDay, car.CustomerID);
                        listServiceDoB = listServiceDOB.Select(u => u.ServiceID).ToList();
                    }
                    else
                    {
                        listServiceDoB = lsservice.Where(u => u.Status.Equals(0)).Select(u => u.ServiceID).ToList();
                    }
                    if (listServiceDoB.Count > 0)
                    {

                        listServiceFree = listServiceFree.Concat(listServiceDoB).ToList();
                    }
                }
            }
            query.listServiceFree = listServiceFree;
            query.listServiceUsed = listServiceUsed;
            return query;
        }
        public List<ServiceBonusCustomer> CreateServiceBonusBirthDay(int beforeBirthDay, int afterBirthDay, int cusID)
        {
            Customer cus = cnn.Customers.Find(cusID);
            List<CustomerRankServiceBonu> lsService = cus.CustomerRank.CustomerRankServiceBonus.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals((int)TypeServiceBonus.BirthDay)).ToList();
            if (lsService.Count > 0)
            {
                if (DateTime.Today.AddDays(-beforeBirthDay).DayOfYear <= cus.DOB.Value.DayOfYear && cus.DOB.Value.DayOfYear <= DateTime.Today.AddDays(afterBirthDay).DayOfYear)
                {
                    List<ServiceBonusCustomer> listServiceDOBhave = cnn.ServiceBonusCustomers.Where(u => u.CustomerID.Equals(cusID) && u.Type.Equals((int)TypeServiceBonus.BirthDay)).ToList();
                    listServiceDOBhave = listServiceDOBhave.Where(u => u.CreateDate.Year.Equals(DateTime.Now.Year)).ToList();
                    if (listServiceDOBhave.Count == 0)
                    {
                        List<ServiceBonusCustomer> listServiceDOB = lsService.Select(u => new ServiceBonusCustomer
                        {
                            RankID = cus.CustomerRankID.Value,
                            CustomerID = cusID,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now.Date.AddDays(-beforeBirthDay),
                            Type = u.Type,
                            ExpriceTime = DateTime.Now.AddDays(afterBirthDay).Date,
                            ServiceID = u.ServiceID,
                            Status = 0
                        }).ToList();
                        cnn.ServiceBonusCustomers.AddRange(listServiceDOB);
                        cnn.SaveChanges();
                        return listServiceDOB;
                    }
                }
                return null;
            }
            return null;
        }

        public Boolean CheckExistingItem(string itemCode)
        {
            try
            {
                var item = cnn.Services.Where(u => u.Code.Equals(itemCode) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                if (item != null && item.Count() > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        public List<MainServiceFilter> getlistServiceFliter(string Lang)
        {
            List<MainServiceFilter> query = cnn.Services.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(u => new MainServiceFilter
            {
                ServiceID = u.ID,
                ServiceName = Lang.Equals(SystemParam.VN) ? u.NameVN : u.NameEN
            }).ToList();
            List<MainServiceFilter> listCombo = cnn.ServiceComboes.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new MainServiceFilter
            {
                comboID = u.ID,
                ServiceName = Lang.Equals(SystemParam.VN) ? u.NameVN : u.NameEN

            }).ToList();
            List<MainServiceFilter> output = query.Concat(listCombo).ToList();
            return output;
        }

        public string CheckListService(int orderServiceID, string lang)
        {
            OrderService orderService = cnn.OrderServices.Find(orderServiceID);
            if (orderService == null)
                return lang.Equals(SystemParam.VN) ? MessVN.NOT_FOUND_MESS : MessEN.NOT_FOUND_MESS;
            List<ListServiceModel> listService = orderService.OrderServiceDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new ListServiceModel
            {
                serviceID = u.ServicePrice.ServiceID.Value,
                Name = lang.Equals(SystemParam.VN) ? u.ServicePrice.Service.NameVN : u.ServicePrice.Service.NameEN,
                Price = u.ServicePrice.Price.Value,
                Type = u.ServicePrice.Service.Type,
                icon = u.ServicePrice.Service.Icon
            }).ToList();
            int segmentID = orderService.CarCustomer.CarModel.CarSegmentID.Value;
            ListServiceModel mainService = listService.Where(u => u.Type.Equals(Constant.TYPE_MAIN_SERVICE)).FirstOrDefault();
            List<int> additionService = listService.Where(u => u.Type.Equals(Constant.TYPE_ADDITION_SERVICE)).Select(u => u.serviceID).ToList();
            if (mainService == null)
                return lang.Equals(SystemParam.VN) ? MessVN.MAIN_SERVICE_NOT_FOUND : MessEN.MAIN_SERVICE_NOT_FOUND;
            else
            {
                var lsmainService = cnn.ServicePrices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ServiceID.Value.Equals(mainService.serviceID) && u.CarSegmentID.Equals(segmentID)).ToList();
                if (lsmainService == null || lsmainService.Count == 0)
                    return lang.Equals(SystemParam.VN) ? MessVN.MAIN_SERVICE_NOT_FOUND : MessEN.MAIN_SERVICE_NOT_FOUND;
                if (additionService.Count > 0)
                {
                    var lsAdditionService = cnn.ServicePrices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && additionService.Contains(u.ServiceID.Value) && u.CarSegmentID.Equals(segmentID)).ToList();
                    if (lsAdditionService.Count < additionService.Count)
                        return lang.Equals(SystemParam.VN) ? MessVN.ADDITION_SERVICE_ERROR : MessEN.ADDITION_SERVICE_ERROR;
                }
            }
            return SystemParam.SUCCES_STR;
        }

        public List<ServiceBusinessModel> Search(string code = "", string name = "", string fromDate = "", string toDate = "")
        {
            DateTime? fDate = Util.ConvertDate(fromDate);
            DateTime? tDate = Util.ConvertDate(toDate);
            tDate = tDate.HasValue ? tDate.Value.AddDays(1) : Util.ConvertDate(toDate);
            List<ServiceBusinessModel> query = new List<ServiceBusinessModel>();
            query = cnn.Services
                .Where(u => u.Code.Contains(code) && !u.IsActive.Equals(SystemParam.INACTIVE)
                && (u.NameEN.Contains(name) || u.NameVN.Contains(name))
                && (fDate.HasValue ? u.CreateDate >= fDate.Value : true)
                && (tDate.HasValue ? u.CreateDate <= tDate.Value : true)
                ).Select(u => new ServiceBusinessModel
                {
                    ID = u.ID,
                    Code = u.Code,
                    DisplayOrder = u.OrderDisplay.Value,
                    NameEN = u.NameEN,
                    NameVN = u.NameVN,
                    Type = u.Type,
                    IsActive = u.IsActive,
                    EstTimeFrom = u.ServicePrices.Where(c => c.IsActive.Equals(SystemParam.ACTIVE)).OrderBy(c => c.EstTime.Value).Select(c => c.EstTime.Value).FirstOrDefault(),
                    EstTimeTo = u.ServicePrices.Where(c => c.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(c => c.EstTime.Value).Select(c => c.EstTime.Value).FirstOrDefault(),
                    CreateDate = u.CreateDate
                }).OrderByDescending(u => u.ID).ToList();
            return query;
        }

        public ServiceDetailModel Detail(int ID)
        {
            ServiceDetailModel query = new ServiceDetailModel();
            query = cnn.Services
                .Where(u => u.ID.Equals(ID)
                ).Select(u => new ServiceDetailModel
                {
                    ID = u.ID,
                    Code = u.Code,
                    DisplayOrder = u.OrderDisplay.Value,
                    NameEN = u.NameEN,
                    Color = u.Color,
                    Description = u.Description,
                    DescriptionEN = u.DescriptionEN,
                    NameVN = u.NameVN,
                    Type = u.Type,
                    Icon = u.Icon,
                    ListImage = u.ServiceImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(s => new ImageServiceModel
                    {
                        ID = s.ID,
                        url = s.Image,
                        Type = s.Type,
                        IsActive = s.IsActive
                    }).ToList(),
                    Listjob = u.ServiceRequiteImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).GroupBy(i => i.Content).Select(i => new JobDetail
                    {
                        ID = i.FirstOrDefault().ID,
                        Content = i.FirstOrDefault().Content,
                        DisplayOrder = i.FirstOrDefault().OrderDisplay.Value,
                        IsActive = i.FirstOrDefault().IsActive,
                    }).OrderBy(i => i.ID).ToList(),
                    ListServicePrice = u.ServicePrices.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => new ServicePriceSegment
                    {
                        ID = i.ID,
                        Price = i.BasePrice.Value,
                        USDPrice = i.USDBasePrice.HasValue? i.USDBasePrice.Value : 0,
                        SegmentID = i.CarSegmentID,
                        SegmentName = i.CarSegment.NameEN,
                        Time = i.EstTime.Value
                    }).ToList(),
                    Discount = u.ServicePrices.Where(i => i.IsActive.Equals(SystemParam.ACTIVE) && i.Discount.HasValue).Select(i => i.Discount.Value).FirstOrDefault(),
                    IsActive = u.IsActive,
                }).OrderByDescending(u => u.ID).FirstOrDefault();
            var service = cnn.Services.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            var ListMainServiceAdditionService = cnn.MainServiceAdditionServices.Where(i => i.Service.IsActive.Equals(SystemParam.ACTIVE) && i.IsActive.Equals(SystemParam.ACTIVE) && (query.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? i.MainServiceID.Equals(query.ID) : i.AdditionServiceID.Equals(query.ID))).ToList();
            query.ListMainServiceAdditionService = ListMainServiceAdditionService.Select(i => new Model.APIApp.MainServiceAdditionServiceModel
            {
                MainServiceID = i.MainServiceID,
                AddtionServiceID = i.AdditionServiceID,
                ServieName = query.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? service.Where(u => u.ID.Equals(i.AdditionServiceID)).Select(u => u.NameEN).FirstOrDefault() : service.Where(u => u.ID.Equals(i.MainServiceID)).Select(u => u.NameEN).FirstOrDefault()
            }).ToList();
            return query;
        }

        public bool CreateService(ServiceDetailModel item)
        {
            Service service = new Service();
            service.Code = item.Code;
            service.CreateDate = DateTime.Now;
            service.IsActive = item.IsActive;
            service.NameEN = item.NameEN;
            service.NameVN = item.NameVN;
            service.Color = item.Color;
            service.Icon = item.Icon;
            service.Description = item.Description;
            service.DescriptionEN = item.DescriptionEN;
            service.OrderDisplay = item.DisplayOrder;
            service.Type = item.Type;
            service.ServicePrices = CreateServicePrice(item.ListServicePrice, item.Discount, item.NameEN, item.NameVN);
            if (item.Type == 1)
            {
                service.ServiceImages = CreateImageService(item);
            }
            service.ServiceRequiteImages = CreateImageRequire(item);
            cnn.Services.Add(service);
            cnn.SaveChanges();
            int id = service.ID;
            var mainServiceAdditionService = item.ListMainServiceAdditionService.Select(u => new MainServiceAdditionService
            {
                MainServiceID = item.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? id : u.MainServiceID.Value,
                AdditionServiceID = item.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? u.AddtionServiceID.Value : id,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now,
            }).ToList();
            cnn.MainServiceAdditionServices.AddRange(mainServiceAdditionService);
            cnn.SaveChanges();
            return true;
        }
        public SystemResult DeleteService(int id)
        {
            Service service = cnn.Services.Find(id);
            if (service != null)
            {
                var listOrder = cnn.OrderServiceDetails.Where(u => u.ServicePrice.ServiceID.Value.Equals(id)).ToList();
                if (listOrder.Count == 0)
                {
                    service.IsActive = SystemParam.INACTIVE;
                    cnn.SaveChanges();
                    return resultBus.SucessResult(null);
                }
                else
                    return resultBus.ErrorResult("Cant delete Package Service");
            }
            else
            {
                return resultBus.ErrorResult("");
            }
        }
        public List<ServicePrice> CreateServicePrice(List<ServicePriceSegment> item, int discount, string NameEN, string NameVN)
        {
            List<ServicePrice> query = item.Select(u => new ServicePrice
            {
                BasePrice = u.Price,
                USDBasePrice = u.USDPrice,
                Discount = discount,
                Price = u.Price * (100 - discount) / 100,
                USDPrice = (double)(u.USDPrice * (100 - discount) / 100),
                IsActive = SystemParam.ACTIVE,
                EstTime = u.Time,
                CreateDate = DateTime.Now,
                Name = NameVN,
                Code = NameEN,
                CarSegmentID = u.SegmentID
            }).ToList();
            return query;
        }
        public List<ServiceImage> CreateImageService(ServiceDetailModel item)
        {
            List<ServiceImage> query = item.ListImage.Select(u => new ServiceImage
            {
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now,
                Image = u.url,
                Type = u.Type
            }).ToList();
            return query;
        }
        public List<ServiceRequiteImage> CreateImageRequire(ServiceDetailModel item)
        {
            List<ServiceRequiteImage> output = new List<ServiceRequiteImage>();
            for (int i = 1; i <= 2; i++)
            {
                foreach (var u in item.Listjob)
                {
                    ServiceRequiteImage data = new ServiceRequiteImage();
                    data.Content = u.Content;
                    data.OrderDisplay = u.DisplayOrder;
                    data.TypeImage = i;
                    data.IsActive = SystemParam.ACTIVE;
                    data.CreateDate = DateTime.Now;
                    output.Add(data);
                }
                //List<ServiceRequiteImage> query = item.Listjob.Select(u =>
                //new ServiceRequiteImage
                //{
                //    Content = u.Content,
                //    OrderDisplay = u.DisplayOrder,
                //    TypeImage = i,
                //    IsActive = SystemParam.ACTIVE,
                //    CreateDate = DateTime.Now
                //}).ToList();
                //if (output.Count == 0)
                //    output = query;
                //else
                //    output.Concat(query);
            }
            return output;
        }

        public bool UpdateService(ServiceDetailModel item)
        {
            Service service = cnn.Services.Find(item.ID);
            service.NameEN = item.NameEN;
            service.NameVN = item.NameVN;
            service.Description = item.Description;
            service.DescriptionEN = item.DescriptionEN;
            service.OrderDisplay = item.DisplayOrder;
            service.Color = item.Color;
            service.Icon = item.Icon;
            service.IsActive = item.IsActive;
            var lsImage = cnn.ServiceImages.Where(u => u.ServiceID.Value.Equals(item.ID)).ToList();
            foreach (var i in lsImage)
            {
                cnn.ServiceImages.Remove(i);
            }
            List<int> listID = new List<int>();
            if (item.ListMainServiceAdditionService != null && item.ListMainServiceAdditionService.Count > 0)
            {
                listID = item.ListMainServiceAdditionService.Select(u => u.MainServiceID.HasValue ? u.MainServiceID.Value : u.AddtionServiceID.Value).ToList();
            }
            // lấy ra 1 list
            var listMainServiceAdditionService = cnn.MainServiceAdditionServices.Where(i =>
              i.Service.IsActive.Equals(SystemParam.ACTIVE)
              && i.Service1.IsActive.Equals(SystemParam.ACTIVE)
              && i.IsActive.Equals(SystemParam.ACTIVE)
              && service.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? i.MainServiceID.Equals(service.ID) : i.AdditionServiceID.Equals(service.ID)).ToList();
            // tìm ra list remove
            var ListRemove = listMainServiceAdditionService.Where(i =>
              (service.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? !listID.Contains(i.AdditionServiceID) : !listID.Contains(i.MainServiceID))).ToList();
            List<int> listAdd = listID.Where(i => service.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? !listMainServiceAdditionService.Select(u => u.AdditionServiceID).ToList().Contains(i) : !listMainServiceAdditionService.Select(u => u.MainServiceID).ToList().Contains(i)).ToList();
            // listAdd
            var mainServiceAdditionService = listAdd.Select(u => new MainServiceAdditionService
            {
                MainServiceID = item.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? item.ID : u,
                AdditionServiceID = item.Type.Equals(Constant.TYPE_MAIN_SERVICE) ? u : item.ID,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now,
            }).ToList();
            cnn.MainServiceAdditionServices.RemoveRange(ListRemove);
            cnn.MainServiceAdditionServices.AddRange(mainServiceAdditionService);
            cnn.SaveChanges();
            UpdateServicePrice(item);
            UpdateJob(item);
            if (item.ListImage != null)
                foreach (var image in item.ListImage)
                {
                    cnn.ServiceImages.Add(UploadImage(image.Type, item.ID, image.url));
                }
            cnn.SaveChanges();
            return true;
        }
        public void UpdateServicePrice(ServiceDetailModel item)
        {
            var ListService = cnn.ServicePrices.Where(u => u.ServiceID.Value.Equals(item.ID)).ToList();
            foreach (var service in ListService)
            {
                ServicePriceSegment serviceP = item.ListServicePrice.Where(u => u.SegmentID.Equals(service.ID)).FirstOrDefault();
                service.BasePrice = serviceP.Price;
                service.USDBasePrice = serviceP.USDPrice;
                service.EstTime = serviceP.Time;
                service.Discount = item.Discount;
                service.Price = serviceP.Price * (100 - item.Discount) / 100;
                service.USDPrice = (double)serviceP.USDPrice * (100 - item.Discount) / 100;
            }
            cnn.SaveChanges();
        }
        public void UpdateJob(ServiceDetailModel item)
        {
            var listimageRequire = cnn.ServiceRequiteImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ServiceID.Equals(item.ID)).ToList();

            foreach (var job in item.Listjob)
            {
                int ID = job.ID;
                var imageRequire = listimageRequire.Where(u => u.ID.Equals(ID) || u.Content.ToLower().Equals(job.Content.ToLower())).FirstOrDefault();
                if (!String.IsNullOrEmpty(job.Content))
                {
                    if (imageRequire == null)
                    {
                        for (int i = 1; i <= 2; i++)
                        {
                            ServiceRequiteImage query = new ServiceRequiteImage();
                            query.Content = job.Content;
                            query.OrderDisplay = job.DisplayOrder;
                            query.TypeImage = i;
                            query.IsActive = SystemParam.ACTIVE;
                            query.CreateDate = DateTime.Now;
                            query.ServiceID = item.ID;
                            cnn.ServiceRequiteImages.Add(query);
                        }
                    }
                    else
                    {
                        var listImage = cnn.ServiceRequiteImages.Where(u => u.ServiceID.Equals(imageRequire.ServiceID) && u.Content.ToLower().Equals(imageRequire.Content.ToLower()) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
                        foreach (var image in listImage)
                        {
                            image.Content = job.Content;
                            image.OrderDisplay = job.DisplayOrder;
                        }
                    }
                }
            }
            List<string> listSTR = item.Listjob.Where(c => !String.IsNullOrEmpty(c.Content)).Select(c => c.Content.ToLower()).ToList();
            List<ServiceRequiteImage> qwerty = cnn.ServiceRequiteImages.Where(u => u.ServiceID.Equals(item.ID) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            qwerty = qwerty.Where(u => !listSTR.Contains(u.Content.ToLower())).ToList();
            foreach (var image in qwerty)
            {
                image.IsActive = SystemParam.INACTIVE;
            }
            cnn.SaveChanges();
        }
        public ServiceImage UploadImage(int type, int serviceID, string image)
        {
            ServiceImage i = new ServiceImage();
            i.ServiceID = serviceID;
            i.IsActive = SystemParam.ACTIVE;
            i.CreateDate = DateTime.Now;
            i.Image = image;
            i.Type = type;
            return i;
        }
        public bool DeleteIamge(int ID)
        {
            try
            {
                ServiceImage i = cnn.ServiceImages.Find(ID);
                i.IsActive = SystemParam.INACTIVE;
                cnn.SaveChanges();
                Util.DeleteIamgeLocal(i.Image);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
