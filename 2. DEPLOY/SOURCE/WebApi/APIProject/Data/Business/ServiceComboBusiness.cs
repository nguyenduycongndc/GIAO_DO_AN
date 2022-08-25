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
using System.Windows.Media.Animation;

namespace Data.Business
{
    public class ServiceComboBusiness : GenericBusiness
    {
        public ServiceComboBusiness(WE_SHIPEntities context = null) : base()
        {
        }

        public ComboServiceDetailModel GetComboDetail(string comboCode, string lang, TokenOutputModel checkToken)
        {

            ComboServiceDetailModel query = new ComboServiceDetailModel();
            var data = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CodeCombo.Equals(comboCode)).ToList();
            query = data.GroupBy(u => new { u.CodeCombo, u.CarCustomerID }).Select(u => new ComboServiceDetailModel
            {
                OrderServiceID = u.Min(s => s.ID),
                ComboCode = comboCode,
                CarImage = u.Select(s => s.CarCustomer.CarImages.Select(i => i.Path).FirstOrDefault()).FirstOrDefault(),
                LicensePlates = u.Select(s => s.CarCustomer.LicensePlates).FirstOrDefault(),
                BookingDate = u.OrderBy(s => s.ID).Select(s => s.BookingDate.Value).FirstOrDefault(),
                BasePrice = u.Select(s => s.ComboPrice.Value).FirstOrDefault(),
                CustomerAddress = u.Select(s => s.Address).FirstOrDefault(),
                CustomerAvatar = u.Select(s => s.Customer.AvatarUrl).FirstOrDefault(),
                CustomerName = u.Select(s => s.Customer.Name).FirstOrDefault(),
                CustomerPhone = u.Select(s => s.Customer.Phone).FirstOrDefault(),
                Lati = u.Select(s => s.Latitude).FirstOrDefault(),
                Longi = u.Select(s => s.Longitude).FirstOrDefault(),
                ReasonNote = u.Select(s => s.Reason).FirstOrDefault(),
                Note = u.Select(s => s.Note).FirstOrDefault(),
                CouponPoint = u.FirstOrDefault().CouponPoint.HasValue ? u.FirstOrDefault().CouponPoint.Value : 0,
                UsePoint = u.FirstOrDefault().UsePoint.HasValue ? u.FirstOrDefault().UsePoint.Value : 0,
                CarSegment = u.Select(s => s.CarCustomer.CarModel.CarSegmentID.Value).FirstOrDefault(),
                Status = u.Select(s => s.Status).FirstOrDefault(),
                PaymentType = Constant.PAYMENT_TYPE_VNP,
                ServiceCode = u.Select(s => s.Code).FirstOrDefault(),
                ComboID = u.Select(s => s.ServiceComboSegment.ServiceComboID).FirstOrDefault(),
                ComboName = u.Select(s => lang.Equals(SystemParam.VN) ? s.ServiceComboSegment.ServiceCombo.NameVN : s.ServiceComboSegment.ServiceCombo.NameEN).FirstOrDefault()
            }
            ).FirstOrDefault();
            if (query == null)
                return query;
            if (checkToken.AgentID.HasValue)
            {
                Agent agent = cnn.Agents.Find(checkToken.AgentID.Value);
                int? commission = agent.ConfigCommission.MastersBenefit;
                if (commission.HasValue)
                    query.Commission = (query.BasePrice + query.CouponPoint + query.UsePoint) * commission.Value / 100;
                if (agent.lati.HasValue && agent.longi.HasValue)
                    query.Distance = Util.Distance(agent.lati.Value, agent.longi.Value, query.Lati, query.Longi);
            }
            query.TotalPrice = query.BasePrice;
            query.CarCustomerID = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CodeCombo.Equals(comboCode)).FirstOrDefault().CarCustomerID;
            query.Car = cnn.CarCustomers.Where(u => u.ID.Equals(query.CarCustomerID.Value)).Select(c => new CarOutputModel
            {
                carID = c.ID,
                CarBrand = c.CarModel.CarBrand.Name,
                CarModelID = c.CarModeID,
                CarModel = c.CarModel.Name,
                CarColor = c.CarColor,
                LicensePlates = c.LicensePlates,
                ManufacturingDate = c.ManufacturingDate,
                RegistrationDate = c.RegistrationDate,
                ListImage = c.CarImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(ci => new
                {
                    Url = ci.Path,
                    ImageID = ci.ID
                }).OrderByDescending(ci => ci.ImageID).ToList(),
                CarImage = c.CarImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).OrderByDescending(u => u.ID).Select(u => u.Path).FirstOrDefault(),
            }).FirstOrDefault();
            query.ListService = cnn.ServiceComboDetails.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ServiceComboID.Equals(query.ComboID)).OrderBy(u => u.DisplayOrder).Select(u => new ServiceInCombo
            {
                ServiceName = lang.Equals(SystemParam.VN) ? u.Service.NameVN : u.Service.NameEN,
                ServicePrice = u.Service.ServicePrices.Where(s => s.CarSegmentID.Equals(query.CarSegment)).Select(s => s.Price.Value).FirstOrDefault(),
                ServiceType = u.Service.Type
            }).ToList();

            return query;
        }
        public List<ServiceComboModel> SearchCombo(string FromDate, string ToDate, string Searchkey)
        {
            try
            {
                List<ServiceComboModel> query = new List<ServiceComboModel>();
                DateTime? fromDate = Util.ConvertDate(FromDate);
                DateTime? toDate = Util.ConvertDate(ToDate);
                if (toDate != null)
                    toDate = toDate.Value.AddDays(1);
                query = cnn.ServiceComboes.Where(u =>
                (Searchkey.Length > 0 ? u.NameEN.Contains(Searchkey) : true) &&
                (fromDate.HasValue ? u.CreateDate >= fromDate.Value : true) &&
                (toDate.HasValue ? u.CreateDate <= toDate.Value : true) && !u.IsActive.Equals(SystemParam.INACTIVE)).OrderByDescending(u => u.ID)
                    .Select(u => new ServiceComboModel
                    {
                        ID = u.ID,
                        AdditionServiceName = u.ServiceComboDetails.Where(s => s.Service.Type.Equals(Constant.TYPE_ADDITION_SERVICE) && s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => s.Service.NameEN).FirstOrDefault(),
                        IsActive = u.IsActive,
                        Name = u.NameEN,
                        CreateDate = u.CreateDate,
                        listAllMainService = u.ServiceComboDetails.Where(s => s.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE) && s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => new PackageSerModel { ServiceID = s.ServiceID, Name = s.Service.NameEN }).ToList(),
                        listMainService = u.ServiceComboDetails.Where(s => s.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE) && s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => new PackageSerModel { ServiceID = s.ServiceID, Name = s.Service.NameEN }).Distinct().ToList(),
                    }).ToList();
                return query;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ServiceComboModel>();
            }
        }

        public ComboDetailModel ComboDetail(int ID)
        {
            ComboDetailModel query = new ComboDetailModel();
            query = cnn.ServiceComboes.Where(u => u.ID.Equals(ID) && !u.IsActive.Equals(SystemParam.INACTIVE) && u.ID.Equals(ID)).Select(u => new ComboDetailModel
            {
                ID = u.ID,
                AdditionService = u.ServiceComboDetails.Where(s => s.Service.Type.Equals(Constant.TYPE_ADDITION_SERVICE) && s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => new PackageSerModel { ServiceID = s.ServiceID, Name = s.Service.NameEN, IsActive = s.IsActive }).ToList(),
                IsActive = u.IsActive,
                NameEN = u.NameEN,
                Icon = u.icon,
                NameVN = u.NameVN,
                Code = u.ComboCode,
                Description = u.Depcription,
                DescriptionEN = u.DescriptionEN,
                Discount = u.ServiceComboSegments.Where(s => s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => s.Discount.Value).FirstOrDefault(),
                ListIamge = u.ServiceImages.Where(s => s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => new ImageService { ID = u.ID, url = s.Image, Type = s.Type }).ToList(),
                ListPackageService = u.ServiceComboDetails.Where(s => s.IsActive.Equals(SystemParam.ACTIVE) && s.Service.Type.Equals(Constant.TYPE_MAIN_SERVICE)).Select(s => new PackageDetailModel
                {
                    ServiceID = s.ServiceID,
                    DisplayOrder = s.DisplayOrder.Value,
                    Name = s.Service.NameEN,
                    Count = u.ServiceComboDetails.Where(sc => sc.IsActive.Equals(SystemParam.ACTIVE) && sc.ServiceID.Equals(s.ServiceID)).ToList().Count
                }).Distinct().ToList(),
                ListComboServicePrice = u.ServiceComboSegments.Where(s => s.IsActive.Equals(SystemParam.ACTIVE)).Select(s => new ComboPrice
                {
                    ID = s.ID,
                    BasePrice = s.BasaPrice.Value,
                    SegmentID = s.CarSegmentID,
                    SegmentName = s.CarSegment.NameEN,
                    USDBasePrice = s.USDBasePrice.HasValue ? s.USDBasePrice.Value : 0,
                }).ToList(),
            }).FirstOrDefault();
            return query;
        }
        public bool CreatCombo(ComboDetailModel item)
        {
            try
            {
                ServiceCombo combo = new ServiceCombo();
                combo.NameEN = item.NameEN;
                combo.NameVN = item.NameVN;
                combo.IsActive = item.IsActive;
                combo.icon = item.Icon;
                combo.Depcription = item.Description;
                combo.DescriptionEN = item.DescriptionEN;
                combo.ServiceImages = GetListImageCombo(item);
                combo.ServiceComboDetails = GetListMainCombo(item);
                combo.ServiceComboSegments = GetListServiceSegment(item);
                combo.CreateDate = DateTime.Now;
                cnn.ServiceComboes.Add(combo);
                cnn.SaveChanges();
                ServiceCombo query = cnn.ServiceComboes.OrderByDescending(u => u.ID).Select(u => u).FirstOrDefault();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<ServiceImage> GetListImageCombo(ComboDetailModel item)
        {
            List<ServiceImage> ListImage = item.ListIamge.Select(u => new ServiceImage
            {
                Image = u.url,
                Type = u.Type,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now
            }).ToList();
            return ListImage;
        }
        public List<ServiceComboDetail> GetListMainCombo(ComboDetailModel item)
        {

            List<ServiceComboDetail> output = new List<ServiceComboDetail>();
            foreach (PackageDetailModel pack in item.ListPackageService)
            {
                for (int i = 0; i < pack.Count; i++)
                {
                    ServiceComboDetail service = new ServiceComboDetail();
                    service.IsActive = SystemParam.ACTIVE;
                    service.CreateDate = DateTime.Now;
                    service.ServiceID = pack.ServiceID;
                    service.DisplayOrder = pack.DisplayOrder;
                    output.Add(service);
                }
            }
            if(item.AdditionService!=null)
            {
                foreach (PackageSerModel additionservice in item.AdditionService)
                {
                    ServiceComboDetail additionService = new ServiceComboDetail();
                    additionService.IsActive = SystemParam.ACTIVE;
                    additionService.CreateDate = DateTime.Now;
                    additionService.ServiceID = additionservice.ServiceID;
                    output.Add(additionService);
                }
            }    
            return output;
        }

        public List<ServiceComboSegment> GetListServiceSegment(ComboDetailModel item)
        {
            List<ServiceComboSegment> output = item.ListComboServicePrice.Select(u => new ServiceComboSegment
            {
                CarSegmentID = u.SegmentID,
                BasaPrice = u.BasePrice,
                Price = u.BasePrice * (100 - item.Discount) / 100,
                USDBasePrice = u.USDBasePrice,
                USDPrice = (double)u.USDBasePrice * (100 - item.Discount) / 100,
                Discount = item.Discount,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now
            }).ToList();
            return output;
        }

        public bool DeleteCombo(int ID)
        {
            ServiceCombo combo = cnn.ServiceComboes.Find(ID);
            if (combo != null)
            {
                var listorder = cnn.OrderServices.Where(u => u.ServiceComboSegment.ServiceComboID.Equals(ID)).ToList();
                if (listorder.Count > 0)
                    return false;
                combo.IsActive = SystemParam.INACTIVE;
                foreach (var data in combo.ServiceComboDetails)
                {
                    data.IsActive = SystemParam.INACTIVE;
                }
                foreach (var data in combo.ServiceImages)
                {
                    data.IsActive = SystemParam.INACTIVE;
                }
                foreach (var data in combo.ServiceComboSegments)
                {
                    data.IsActive = SystemParam.INACTIVE;
                }
                cnn.SaveChanges();
                return true;
            }
            return false;

        }

        public bool UpdateCombo(ComboDetailModel item)
        {
            try
            {
                ServiceCombo combo = cnn.ServiceComboes.Find(item.ID);
                combo.NameEN = item.NameEN;
                combo.NameVN = item.NameVN;
                combo.IsActive = item.IsActive;
                combo.Depcription = item.Description;
                combo.DescriptionEN = item.DescriptionEN;
                combo.icon = item.Icon;
                cnn.SaveChanges();
                UpdateImageCombo(item);
                DelteMainService(item);
                foreach (var mainService in item.ListPackageService)
                {
                    UpdateComboMainService(mainService, item.ID);
                }
                if(item.AdditionService!=null&&item.AdditionService.Count>0)
                {
                    UpdateAdditionService(item.AdditionService, item.ID);
                }    
                UpdateService(item.ListComboServicePrice, item.ID, item.Discount);
                return true;
            }
            catch
            {

                return false;
            }
        }
        public void UpdateImageCombo(ComboDetailModel item)
        {
            List<ServiceImage> ListService = cnn.ServiceImages.Where(u => u.ServiceComboID.Value.Equals(item.ID) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            ServiceImage mainSerivice = ListService.Where(u => u.Type.Equals(Constant.MAIN_IMAGE)).FirstOrDefault();
            mainSerivice.Image = item.ListIamge.Where(u => u.Type.Equals(Constant.MAIN_IMAGE)).Select(u => u.url).FirstOrDefault();
            ServiceImage thumbnail = ListService.Where(u => u.Type.Equals(Constant.THUMBNAIL_IMAGE)).FirstOrDefault();
            thumbnail.Image = item.ListIamge.Where(u => u.Type.Equals(Constant.THUMBNAIL_IMAGE)).Select(u => u.url).FirstOrDefault();
            List<ServiceImage> ListServiceRemove = ListService.Where(u => u.Type.Equals(Constant.ORTHER_IMAGE)).ToList();
            cnn.ServiceImages.RemoveRange(ListServiceRemove);
            List<ServiceImage> ListAdd = item.ListIamge.Where(u => u.Type.Equals(Constant.ORTHER_IMAGE)).Select(u => new ServiceImage
            {
                Image = u.url,
                Type = u.Type,
                IsActive = SystemParam.ACTIVE,
                CreateDate = DateTime.Now,
                ServiceComboID = item.ID
            }).ToList();
            cnn.ServiceImages.AddRange(ListAdd);
            cnn.SaveChanges();

        }
        public void DelteMainService(ComboDetailModel item)
        {
            List<int> list = item.ListPackageService.Select(u => u.ServiceID).ToList();
            List<ServiceComboDetail> listService = cnn.ServiceComboDetails.Where(u => u.ServiceComboID.Equals(item.ID) && !list.Contains(u.ServiceID) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            foreach (ServiceComboDetail query in listService)
            {
                query.IsActive = SystemParam.INACTIVE;
            }
            cnn.SaveChanges();
        }

        public bool UpdateComboMainService(PackageDetailModel item, int ID)
        {
            List<ServiceComboDetail> listService = cnn.ServiceComboDetails.Where(u => u.ServiceComboID.Equals(ID) && u.ServiceID.Equals(item.ServiceID) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            if (item.Count > listService.Count)
            {
                for (int i = 0; i < item.Count - listService.Count; i++)
                {
                    ServiceComboDetail service = new ServiceComboDetail();
                    service.IsActive = SystemParam.ACTIVE;
                    service.CreateDate = DateTime.Now;
                    service.ServiceID = item.ServiceID;
                    service.DisplayOrder = item.DisplayOrder;
                    service.ServiceComboID = ID;
                    cnn.ServiceComboDetails.Add(service);
                }
            }
            else if (item.Count < listService.Count)
            {
                for (int i = 0; i < listService.Count - item.Count; i++)
                {
                    ServiceComboDetail service = listService[i];
                    service.IsActive = SystemParam.INACTIVE;
                }
            }
            else
            {
                foreach (ServiceComboDetail service in listService)
                {
                    service.DisplayOrder = item.DisplayOrder;
                }
            }
            cnn.SaveChanges();
            return true;
        }
        public bool UpdateAdditionService(List<PackageSerModel> item, int ID)
        {
            if (item == null) {
                return true;
            }
            var additionService = cnn.ServiceComboDetails.Where(u => u.ServiceComboID.Equals(ID) && u.Service.Type.Equals(Constant.TYPE_ADDITION_SERVICE) && u.IsActive.Equals(SystemParam.ACTIVE)).ToList();
            item = item.Where(u => !additionService.Select(s => s.ID).ToList().Contains(u.ServiceID)).ToList();
            foreach (var data in item)
            {
                ServiceComboDetail service = new ServiceComboDetail();
                service.IsActive = SystemParam.ACTIVE;
                service.CreateDate = DateTime.Now;
                service.ServiceID = data.ServiceID;
                service.ServiceComboID = ID;
                cnn.ServiceComboDetails.Add(service);
            }
            cnn.SaveChanges();
            return true;
        }
        public bool UpdateService(List<ComboPrice> LSitem, int ID, int Discount)
        {
            List<ServiceComboSegment> listService = cnn.ServiceComboSegments.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.ServiceComboID.Equals(ID)).ToList();
            foreach (ServiceComboSegment data in listService)
            {
                ComboPrice item = LSitem.Where(u => u.SegmentID.Equals(data.CarSegmentID)).FirstOrDefault();
                if (item != null)
                {
                    data.BasaPrice = item.BasePrice;
                    data.USDBasePrice = item.USDBasePrice;
                    data.Discount = Discount;
                    data.Price = item.BasePrice * (100 - Discount) / 100;
                    data.USDPrice = (double)item.USDBasePrice * (100 - Discount) / 100;
                }
            }
            cnn.SaveChanges();
            return true;
        }
        public List<string> GetServiceComboBySearch(string Searchkey)
        {
            var query = cnn.ServiceComboes.Where(x => x.NameEN.Contains(Searchkey)).Select(x => x.NameEN).ToList();
            if (query != null && query.Count() > 0)
            {
                return query;
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
