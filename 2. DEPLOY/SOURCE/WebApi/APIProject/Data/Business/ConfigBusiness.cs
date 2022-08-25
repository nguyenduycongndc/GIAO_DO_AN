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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Data.Business
{
    public class ConfigBusiness : GenericBusiness
    {
        RavenClient ravenClient = new RavenClient(SystemParam.DSN_SENTRY);
        public ConfigBusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public List<Config> showElement()
        {
            return cnn.Configs.Select(u => u).ToList();
        }

        public int CreateConfigTransportCost(int Type, int? VehicleTypeID, int? TransportType, int FirstDistance, int FirstPrice, int PerKmPrice, int? BonusFee, int? FirstWeight, int? FirstKgPrice, int? PerKgPrice, int? FeeCOD)
        {
            try
            {
                ConfigTransportCost ctc = new ConfigTransportCost();
                ctc.Type = Type;
                if (Type == SystemParam.SHIP_DRIVER || Type == SystemParam.SHIP_FOOD)
                {
                    ctc.VehicleTypeID = VehicleTypeID;
                    ctc.FirstDistance = FirstDistance;
                    ctc.FirstPrice = FirstPrice;
                    ctc.PerKmPrice = PerKmPrice;
                    ctc.BonusFee = BonusFee;
                    ctc.IsActive = SystemParam.ACTIVE;
                }
                else
                {
                    ctc.TransportType = TransportType;
                    ctc.FirstDistance = FirstDistance;
                    ctc.FirstPrice = FirstPrice;
                    ctc.PerKmPrice = PerKmPrice;
                    //add data for SHIP_PACKAGE (giao hàng)
                    ctc.FistKgPrice = FirstKgPrice;
                    ctc.FirstWeight = FirstWeight;
                    ctc.PerKgPrice = PerKgPrice;
                    ctc.FeeCOD = FeeCOD.Value;
                    ctc.IsActive = SystemParam.ACTIVE;

                }
                cnn.ConfigTransportCosts.Add(ctc);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public int SaveEditConfigTransportCost(int ID, int Type, int? VehicleTypeID, int? TypeTransport, int FirstDistance, int FirstPrice, int PerKmPrice, int? FirstWeight, double? PerKg, int? PerKgPrice)
        {
            try
            {
                //ConfigTransportCost ctc = new ConfigTransportCost();
                var ctc = cnn.ConfigTransportCosts.Find(ID);
                ctc.Type = Type;
                if (Type == SystemParam.SHIP_DRIVER || Type == SystemParam.SHIP_FOOD)
                {
                    ctc.VehicleTypeID = VehicleTypeID;
                    ctc.FirstDistance = FirstDistance;
                    ctc.FirstPrice = FirstPrice;
                    ctc.PerKmPrice = PerKmPrice;
                    ctc.BonusFee = null;

                    ctc.TransportType = null;
                    ctc.FistKgPrice = null;
                    ctc.FirstWeight = null;
                    ctc.PerKgPrice = null;
                    ctc.FeeCOD = null;
                }
                else
                {
                    ctc.TransportType = TypeTransport;
                    ctc.FirstDistance = FirstDistance;
                    ctc.FirstPrice = FirstPrice;
                    ctc.PerKmPrice = PerKmPrice;
                    //add data for SHIP_PACKAGE (giao hàng)
                    ctc.FistKgPrice = null;
                    ctc.FirstWeight = FirstWeight;
                    ctc.PerKgPrice = PerKgPrice;
                    ctc.FeeCOD = null;
                    ctc.PerKg = PerKg; 
                    ctc.VehicleTypeID = null;
                    ctc.BonusFee = null;

                }
                //cnn.ConfigTransportCosts.Add(ctc);
                cnn.SaveChanges();
                return SystemParam.RETURN_TRUE;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        public ConfigTransportCost ModalEditConfigTransportCost(int ID)
        {
            try
            {
                return cnn.ConfigTransportCosts.Find(ID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public List<ConfigTransportCostOuputModel> SearchConfigTranportCost(int Page, int? Type, int? VehicleTypeID, int? IsActive)
        {
            try
            {
                List<ConfigTransportCostOuputModel> data = new List<ConfigTransportCostOuputModel>();


                data = cnn.ConfigTransportCosts.Where(c => IsActive.HasValue ? (c.IsActive == SystemParam.ACTIVE) : (c.IsActive != SystemParam.ACTIVE_FALSE)
                        && (Type == null ? true : (Type == c.Type)) && (VehicleTypeID == null ? true : (VehicleTypeID == c.VehicleTypeID)))
                    .Select(c => new ConfigTransportCostOuputModel
                    {
                        ID = c.ID,
                        Type = c.Type,
                        FirstDistance = c.FirstDistance,
                        FirstPrice = c.FirstPrice,
                        VehicleType = c.VehicleTypeID.HasValue ? c.VehicleType.Name : "",
                        PerKmPrice = c.PerKmPrice,
                        TransportType = c.TransportType

                    }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<ConfigTransportCostOuputModel>();
            }
        }

        public int DeleteTransportCost(int id)
        {
            try
            {
                var ctc = cnn.ConfigTransportCosts.Find(id);
                ctc.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }


        //Thêm mới loại xe
        public JsonResultModel CreateVehicleType(VehicleTypeModel VehicleType)
        {
            try
            {
                VehicleType vehicle = new VehicleType();
                vehicle.Name = VehicleType.Name;
                vehicle.OrderIndex = VehicleType.OrderIndex;
                vehicle.Logo = VehicleType.Logo;
                vehicle.IsActive = 1;
                vehicle.IsMotorbike = VehicleType.IsMotorbike;
                vehicle.CreatedDate = DateTime.Now;
                cnn.VehicleTypes.Add(vehicle);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, vehicle);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }
        //public int CreateVehicleType(string name, int order, string img)
        //{
        //    try
        //    {
        //        VehicleType vehicle = new VehicleType();
        //        vehicle.Name = name;
        //        vehicle.OrderIndex = order;
        //        vehicle.Logo = img;
        //        vehicle.IsActive = 1;
        //        vehicle.CreatedDate = DateTime.Now;
        //        cnn.VehicleTypes.Add(vehicle);
        //        cnn.SaveChanges();
        //        return SystemParam.SUCCESS;
        //    }
        //    catch (Exception ex)
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}
        //binding theo id loại xe
        public VehicleTypeModel GetVehicleType(int id)
        {
            try
            {
                VehicleTypeModel data = new VehicleTypeModel();
                VehicleType mb = cnn.VehicleTypes.Find(id);

                var check = cnn.VehicleTypes.Where(m => m.ID == id).FirstOrDefault();

                //Trường hợp login là shiper
                if (check != null)
                {
                    data.ID = check.ID;
                    data.Name = check.Name;
                    data.Logo = check.Logo;
                    data.IsActive = check.IsActive;
                    data.OrderIndex = check.OrderIndex;
                    data.IsMotorbike = check.IsMotorbike;
                }

                return data;
            }
            catch (Exception ex)
            {
                return new VehicleTypeModel();
            }
        }
        //lưu khi sửa loại xe
        public bool SaveEditVehicleType(VehicleTypeModel request)
        {
            VehicleType sv = cnn.VehicleTypes.Find(request.ID);

            if (sv != null)
            {
                sv.Name = string.IsNullOrEmpty(request.Name) ? "" : request.Name;
                sv.Logo = string.IsNullOrEmpty(request.Logo) ? "" : request.Logo;
                sv.OrderIndex = request.OrderIndex;
                sv.IsActive = (int)request.IsActive;
                sv.IsMotorbike = request.IsMotorbike;
                cnn.SaveChanges();
                if (request.IsActive.Equals(SystemParam.ACTIVE_FALSE))
                {
                    var newVehicleType = cnn.VehicleTypes.FirstOrDefault(x => x.IsActive.Equals(SystemParam.ACTIVE));
                    if (newVehicleType != null)
                    {
                        var listCarShipper = cnn.CarShipers.Where(x => x.VehicleTypeID == sv.ID);
                        foreach (var item in listCarShipper)
                        {
                            item.VehicleTypeID = newVehicleType.ID;

                        }
                        cnn.SaveChanges();
                    }
                    var Configcost = cnn.ConfigTransportCosts.Where(x => x.VehicleTypeID == sv.ID && x.IsActive.Equals(SystemParam.ACTIVE));
                    foreach (var item in Configcost)
                    {
                        item.IsActive = SystemParam.ACTIVE_FALSE;
                    }
                    cnn.SaveChanges();
                }

            }


            return true;
        }

        //xóa loại xe
        public JsonResultModel DeleteVehicleType(int id)
        {
            try
            {
                VehicleType vehicleType = cnn.VehicleTypes.Find(id);
                if(vehicleType == null)
                {
                    return rpBus.ErrorResult("Không tìm thấy loại phương tiện", SystemParam.NOT_FOUND);
                }
                
                var newVehicleType = cnn.VehicleTypes.FirstOrDefault(x => x.IsActive==SystemParam.ACTIVE && x.IsMotorbike == vehicleType.IsMotorbike && x.ID != vehicleType.ID );
                if (newVehicleType != null)
                {
                    vehicleType.IsActive = SystemParam.NO_ACTIVE_DELETE;
                    var listCarShipper = cnn.CarShipers.Where(x => x.VehicleTypeID == id).ToList();
                    foreach (var item in listCarShipper)
                    {
                        item.VehicleTypeID = newVehicleType.ID;

                    }
                    var Configcost = cnn.ConfigTransportCosts.Where(x => x.VehicleTypeID == id && x.IsActive == SystemParam.ACTIVE).ToList();
                    foreach (var item in Configcost)
                    {
                        item.VehicleTypeID = newVehicleType.ID;
                    }
                    cnn.SaveChanges();
                    return rpBus.SuccessResult(MessVN.SUCCESS_STR, true);
                }
                else
                {
                    return rpBus.ErrorResult("Không tìm thấy loại phương tiện thay thế", SystemParam.NOT_FOUND);
                }
                
            }
            catch (Exception ex)
            {
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }

        //tìm kiếm Loại xe
        public IPagedList<VehicleTypeModel> SearchVehicleType(int page, string vehicleName, int? isActive)
        {
            try
            {
                var query = (from a in cnn.VehicleTypes
                             where (!String.IsNullOrEmpty(vehicleName) ? (vehicleName == a.Name) : true)
                             && isActive.HasValue ? (a.IsActive == isActive) : (a.IsActive != SystemParam.ACTIVE_FALSE)
                             orderby a.CreatedDate descending
                             select new VehicleTypeModel()
                             {
                                 ID = a.ID,
                                 Name = a.Name,
                                 Logo = a.Logo,
                                 OrderIndex = a.OrderIndex,
                                 IsMotorbike = a.IsMotorbike,
                                 IsActive = a.IsActive,
                             }).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);

                //if (!string.IsNullOrEmpty(vehicleName))
                //{
                //    query = query.Where(x => x.Name.ToUpper().Trim().Contains(vehicleName.ToUpper().Trim())).ToList();
                //}
                //if (isActive != null)
                //{
                //    query = query.Where(x => x.IsActive == isActive).ToList();
                //}
                //var group = query.ToList();
                //return query.ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return query;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<VehicleTypeModel>().ToPagedList(1, 1);
            }
        }

        // Hiển thị dữ liệu hạng thành viên
        public IPagedList<CustomerRank> ViewCustomerRank(int Page, int? IsActive)
        {
            try
            {
                var query = (from c in cnn.CustomerRanks
                             orderby c.Level ascending
                             select c).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return query.ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new List<CustomerRank>().ToPagedList(1, 1);
            }
        }

        public int UpdateCustomerRank(int ID, string Name, int MinPoint, int MaxPoint, string Description, int ProfitCash, int ProfitVPN, int PointBonus, int ProfitExtraBirthDay, string Policy)
        {
            try
            {
                var cust = cnn.CustomerRanks.Find(ID);
                cust.Name = Name;
                cust.MinPoint = MinPoint;
                //cust.IsActive = SystemParam.ACTIVE;
                cust.MaxPoint = MaxPoint;
                cust.Description = Description;
                cust.ProfitCash = ProfitCash;
                cust.ProfitVPN = ProfitVPN;
                cust.PointBonus = PointBonus;
                cust.ProfitExtraBirthDay = ProfitExtraBirthDay;
                cust.Policy = Policy;
                cust.CreateDate = DateTime.Now;
                //cust.Title = "";
                //cust.Level = 1;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }

        public CustomerRank ModalEditCustomerRank(int ID)
        {
            try
            {
                return cnn.CustomerRanks.Find(ID);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return null;
            }
        }

        public ConfigTransportAreaModel ModalEditTransportArea(int ID)
        {
            try
            {
                var query = (from c in cnn.ConfigTransportAreas
                             where c.IsActive == SystemParam.ACTIVE && c.ID == ID
                             select new ConfigTransportAreaModel()
                             {
                                 ID = c.ID,
                                 Name = c.Name,
                                 FromKm = c.FromKm,
                                 ToKm = c.ToKm,
                                 Type = c.Type,
                                 PerKg = (float)c.PerKg,
                                 Price = c.PerKgPrice,
                                 TimeShip = c.TimeShip,
                                 IsProvince = c.IsProvince

                             }).FirstOrDefault();
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                ex.ToString();
                return new ConfigTransportAreaModel();
            }
        }

        //xóa khu vực
        public int DeleteActiveArea(int id)
        {
            try
            {
                Area acticveArea = cnn.Areas.Find(id);
                acticveArea.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }
        //public JsonResultModel DeleteActiveArea(int id)
        //{
        //    try
        //    {
        //        Area acticveArea = cnn.Areas.Find(id);
        //        acticveArea.IsActive = SystemParam.NO_ACTIVE_DELETE;
        //        cnn.SaveChanges();
        //        return rpBus.SuccessResult(MessVN.SUCCESS_STR, acticveArea);
        //    }
        //    catch (Exception ex)
        //    {
        //        return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
        //    }
        //}




        //tìm kiếm khu vực
        public IPagedList<ActiveAreaModel> SearchActiveArea(int page, int districtId, int provinceId)
        {
            try
            {
                var query = (from a in cnn.Areas
                             join b in cnn.Districts on a.DistrictID equals b.ID
                             join c in cnn.Provinces on b.ProvinceID equals c.ID
                             where (districtId != 0 ? districtId == a.DistrictID : true) && (provinceId != 0 ? provinceId == c.ID : true)
                             && a.IsActive == SystemParam.ACTIVE
                             //orderby a.CreatedDate descending
                             orderby a.District.Name
                             select new ActiveAreaModel
                             {
                                 ID = a.ID,
                                 DistrictName = a.District != null ? a.District.Name : "",
                                 DistrictID = a.District.ID,
                                 ProvinceName = b.Province != null ? b.Province.Name : "",
                                 ProvinceID = b.Province.ID,
                                 IsActive = a.IsActive,
                                 CreatedDate = a.CreatedDate,
                             }).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ActiveAreaModel>().ToPagedList(1, 1);
            }
        }
        public IPagedList<ConfigCommissionViewModel> SearchCommission(int Page, string Name)
        {
            try
            {
                var data = cnn.ConfigCommissions.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && (!String.IsNullOrEmpty(Name) ? c.Name.Contains(Name) : true))
                    .Select(c => new ConfigCommissionViewModel
                    {
                        ID = c.ID,
                        Name = c.Name,

                    }).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ConfigCommissionViewModel>().ToPagedList(1, 1);
            }
        }

        public JsonResultModel GetListRank()
        {
            try
            {
                List<RankOutputModel> data = new List<RankOutputModel>();
                data = cnn.CustomerRanks.Where(c => c.IsActive.Equals(SystemParam.ACTIVE))
                    .Select(c => new RankOutputModel
                    {
                        ID = c.ID,
                        Name = c.Name,
                        Description = c.Description,
                        Point = c.MinPoint,
                        Policy = c.Policy,
                        Level = c.Level
                    }).OrderBy(c => c.Level).ToList();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, data);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        //Thêm mới khu vực
        public JsonResultModel CreateActiveArea(ActiveAreaModel activeAreaModel)
        {

            var query = (from a in cnn.Districts
                         where (activeAreaModel.DistrictID != 0 ? activeAreaModel.DistrictID == a.ID : true) && (activeAreaModel.DistrictName != null ? activeAreaModel.DistrictName == a.Name : true)
                         orderby a.ID descending
                         select new ActiveAreaModel
                         {
                             ID = a.ID,
                             DistrictName = a.Name != null ? a.Name : "",
                         });
            //var no = query.ToList();

            try
            {
                var checkData = cnn.Areas.Where(d => d.DistrictID == activeAreaModel.DistrictID).Count();
                if (checkData != 0)
                {
                    return rpBus.ErrorResult("Huyện này đã tồn tại!", SystemParam.PROCESS_ERROR);
                }
                var yes = query.ToList();
                Area area = new Area();
                area.DistrictID = activeAreaModel.DistrictID;
                District district = new District();
                district.ID = activeAreaModel.DistrictID;
                var Str = yes[0].DistrictName;
                var a = Str.IndexOf(" ");
                activeAreaModel.DistrictName = Str.Substring((a + 1));
                var b = activeAreaModel.DistrictName.IndexOf("xã");
                var c = activeAreaModel.DistrictName.IndexOf("phố");
                if (b != -1)
                {
                    activeAreaModel.DistrictName = activeAreaModel.DistrictName.Substring((b + 3));
                    //area.Name = name;
                }
                if (c != -1)
                {
                    activeAreaModel.DistrictName = activeAreaModel.DistrictName.Substring((c + 4));
                    //area.Name = name;
                }
                area.Name = activeAreaModel.DistrictName;
                area.IsActive = activeAreaModel.IsActive;
                area.CreatedDate = DateTime.Now;
                cnn.Areas.Add(area);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, area);
            }
            catch (Exception ex)
            {
                return rpBus.ErrorResult(ex.Message, SystemParam.PROCESS_ERROR);
            }
        }
        //binding theo id Khu vực
        public ActiveAreaModel GetAreaActive(int id)
        {
            try
            {
                ActiveAreaModel data = new ActiveAreaModel();
                Area mb = cnn.Areas.Find(id);

                //var check = cnn.Areas.Where(m => m.ID == id).FirstOrDefault();

                var check = (from a in cnn.Areas
                             join b in cnn.Districts on a.DistrictID equals b.ID
                             join c in cnn.Provinces on b.ProvinceID equals c.ID
                             select a).Where(x => x.ID == id).FirstOrDefault();

                //Trường hợp login là shiper
                if (check != null)
                {
                    data.ID = check.ID;
                    data.DistrictID = check.DistrictID;
                    data.ProvinceID = check.District.ProvinceID;
                    data.IsActive = check.IsActive;
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ActiveAreaModel();
            }
        }
        //lưu khi sửa khu vực
        public bool SaveEditAreaActive(ActiveAreaModel request)
        {
            Area sv = cnn.Areas.Find(request.ID);

            if (sv != null)
            {
                sv.DistrictID = request.DistrictID;
                sv.IsActive = (int)request.IsActive;
            }

            cnn.SaveChanges();
            return true;
        }

        public ConfigDataModel GetDataSystemParamConfig()
        {
            try
            {
                ConfigDataModel data = new ConfigDataModel();
                WalletDataConfig walletConfig = new WalletDataConfig();
                AreaDataCofig areaConfig = new AreaDataCofig();
                TransactionDataConfig transactionConfig = new TransactionDataConfig();
                List<PeakHourConfig> lstPeakHour = new List<PeakHourConfig>();
                List<Config> cf = cnn.Configs.ToList();

                //lấy các giá trị cấu hình ví:
                //ví cọc tối thiểu
                walletConfig.ConfigWalletNoWithdrawID = cf.Where(c => c.NameConstant.Equals(Constant.MIN_VALUE_WALLET_NO_WITHDRAW)).FirstOrDefault().ID;
                walletConfig.ConfigWalletNoWithdrawValue = cf.Where(c => c.NameConstant.Equals(Constant.MIN_VALUE_WALLET_NO_WITHDRAW)).FirstOrDefault().ValueConstant;
                //ví thu nhập tối thiểu
                walletConfig.ConfigWalletWithdrawID = cf.Where(c => c.NameConstant.Equals(Constant.MIN_VALUE_WALLET_WITHDRAW)).FirstOrDefault().ID;
                walletConfig.ConfigWalletWithdrawValue = cf.Where(c => c.NameConstant.Equals(Constant.MIN_VALUE_WALLET_WITHDRAW)).FirstOrDefault().ValueConstant;
                //thời gian rút tiền
                walletConfig.ConfigTimeWithdrawID = cf.Where(c => c.NameConstant.Equals(Constant.TIME_WITHDRAW)).FirstOrDefault().ID;
                walletConfig.ConfigTimeWithdrawValue = cf.Where(c => c.NameConstant.Equals(Constant.TIME_WITHDRAW)).FirstOrDefault().ValueConstant;

                //Lấy các giá trị cấu hình khu vự hoạt động:
                //Khu vực hoạt động tối đa
                areaConfig.MaxAreaConfigID = cf.Where(c => c.NameConstant.Equals(Constant.MAX_AREA_NUMBER)).FirstOrDefault().ID;
                areaConfig.MaxAreaConfigValue = cf.Where(c => c.NameConstant.Equals(Constant.MAX_AREA_NUMBER)).FirstOrDefault().ValueConstant;
                //Thời gian bắt đầu
                areaConfig.StartTimeConfigID = cf.Where(c => c.NameConstant.Equals(Constant.START_TIME)).FirstOrDefault().ID;
                areaConfig.StartTimeConfigValue = cf.Where(c => c.NameConstant.Equals(Constant.START_TIME)).FirstOrDefault().ValueConstant;
                //Thời gian kết thúc
                areaConfig.EndTimeConfigID = cf.Where(c => c.NameConstant.Equals(Constant.END_TIME)).FirstOrDefault().ID;
                areaConfig.EndTimeConfigValue = cf.Where(c => c.NameConstant.Equals(Constant.END_TIME)).FirstOrDefault().ValueConstant;

                //Lấy các giá trị cấu hình giao dịch:
                //thời gian xác nhận
                transactionConfig.CountDownConfigID = cf.Where(c => c.NameConstant.Equals(Constant.COUNT_DOWN)).FirstOrDefault().ID;
                transactionConfig.CountDownConfigValue = cf.Where(c => c.NameConstant.Equals(Constant.COUNT_DOWN)).FirstOrDefault().ValueConstant;
                //số lần hủy trong ngày
                transactionConfig.CancelTimeConfigID = cf.Where(c => c.NameConstant.Equals(Constant.CANCLE_TIME)).FirstOrDefault().ID;
                transactionConfig.CancelTimeConfigValue = cf.Where(c => c.NameConstant.Equals(Constant.CANCLE_TIME)).FirstOrDefault().ValueConstant;
                //Thời gian tự động hủy khi không có tài xế nhận
                transactionConfig.TimeCancelOrderID = cf.Where(c => c.NameConstant.Equals(Constant.TIME_CANCLE_ORDER)).FirstOrDefault().ID;
                transactionConfig.TimeCancelOrderValue = cf.Where(c => c.NameConstant.Equals(Constant.TIME_CANCLE_ORDER)).FirstOrDefault().ValueConstant;
                //phí thu hộ tối đa
                transactionConfig.MaxCODConfigID = cf.Where(c => c.NameConstant.Equals(Constant.MAX_COD_FEE)).FirstOrDefault().ID;
                transactionConfig.MaxCODConfigValue = cf.Where(c => c.NameConstant.Equals(Constant.MAX_COD_FEE)).FirstOrDefault().ValueConstant;

                //lấy danh sách các khung giờ cao điểm:
                List<ConfigTime> cft = cnn.ConfigTimes.ToList();

                lstPeakHour = cft.Select(u => new PeakHourConfig
                {
                    ID = u.ID,
                    StartTime = u.StartDate.ToString("H:mm"),
                    EndTime = u.EndDate.ToString("H:mm"),
                    Price = u.Price,
                    Description = u.Description
                }).ToList();

                data.WalletConfig = walletConfig;
                data.AreaConfig = areaConfig;
                data.TransactionConfig = transactionConfig;
                data.PeakHourConfig = lstPeakHour;
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new ConfigDataModel();
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu cấu hình ví
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel UpdateWalletDataConfig(WalletDataConfig input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.ConfigTimeWithdrawValue) || String.IsNullOrEmpty(input.ConfigWalletNoWithdrawValue) || String.IsNullOrEmpty(input.ConfigWalletWithdrawValue))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                List<Config> cf = cnn.Configs.ToList();
                cf.Where(c => c.NameConstant.Equals(Constant.MIN_VALUE_WALLET_NO_WITHDRAW)).FirstOrDefault().ValueConstant = input.ConfigWalletNoWithdrawValue.Replace(",", "");
                cf.Where(c => c.NameConstant.Equals(Constant.MIN_VALUE_WALLET_WITHDRAW)).FirstOrDefault().ValueConstant = input.ConfigWalletWithdrawValue.Replace(",", "");
                cf.Where(c => c.NameConstant.Equals(Constant.TIME_WITHDRAW)).FirstOrDefault().ValueConstant = input.ConfigTimeWithdrawValue;

                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        /// <summary>
        /// Cập nhật dữ liệu cấu hình khu vực hoạt động
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel UpdateAreaDataConfig(AreaDataCofig input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.MaxAreaConfigValue) || String.IsNullOrEmpty(input.StartTimeConfigValue) || String.IsNullOrEmpty(input.EndTimeConfigValue))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                List<Config> cf = cnn.Configs.ToList();
                cf.Where(c => c.NameConstant.Equals(Constant.MAX_AREA_NUMBER)).FirstOrDefault().ValueConstant = input.MaxAreaConfigValue;
                cf.Where(c => c.NameConstant.Equals(Constant.START_TIME)).FirstOrDefault().ValueConstant = input.StartTimeConfigValue;
                cf.Where(c => c.NameConstant.Equals(Constant.END_TIME)).FirstOrDefault().ValueConstant = input.EndTimeConfigValue;

                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                return rpBus.serverError();
            }
        }

        public JsonResultModel UpdateTransactionDataConfig(TransactionDataConfig input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.CountDownConfigValue) || String.IsNullOrEmpty(input.CancelTimeConfigValue) || String.IsNullOrEmpty(input.TimeCancelOrderValue) || String.IsNullOrEmpty(input.MaxCODConfigValue))
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                List<Config> cf = cnn.Configs.ToList();
                cf.Where(c => c.NameConstant.Equals(Constant.COUNT_DOWN)).FirstOrDefault().ValueConstant = input.CountDownConfigValue.Replace(",", "");
                cf.Where(c => c.NameConstant.Equals(Constant.CANCLE_TIME)).FirstOrDefault().ValueConstant = input.CancelTimeConfigValue.Replace(",", "");
                cf.Where(c => c.NameConstant.Equals(Constant.TIME_CANCLE_ORDER)).FirstOrDefault().ValueConstant = input.TimeCancelOrderValue.Replace(",", "");
                cf.Where(c => c.NameConstant.Equals(Constant.MAX_COD_FEE)).FirstOrDefault().ValueConstant = input.MaxCODConfigValue.Replace(",", "");

                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        public JsonResultModel UpdatePeakHourDataConfig(List<PeakHourConfig> input)
        {
            try
            {
                if(input == null)
                {
                    return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                }
                foreach(var item in input)
                {
                    if(String.IsNullOrEmpty(item.StartTime) || String.IsNullOrEmpty(item.EndTime) || item.Price <= 0 || String.IsNullOrEmpty(item.Description))
                    {
                        return rpBus.ErrorResult(MessVN.PARAM_ERROR, SystemParam.PROCESS_ERROR);
                    }
                }
                List<ConfigTime> cf = cnn.ConfigTimes.ToList();
                cnn.ConfigTimes.RemoveRange(cf);
                List<ConfigTime> data = input.Select(c => new ConfigTime
                {
                    StartDate = Util.ConvertDate(c.StartTime, "H:mm").Value,
                    EndDate = Util.ConvertDate(c.EndTime, "H:mm").Value,
                    IsActive = SystemParam.ACTIVE,
                    Price = c.Price,
                    CreatedDate = DateTime.Now,
                    Description = c.Description
                }).ToList();
                cnn.ConfigTimes.AddRange(data);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, null);

            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        //tìm kiếm hoa hồng
        public IPagedList<ConfigCommissionModel> SearchConfigCommission(int page, string nameCommission)
        {
            try
            {
                var query = (from a in cnn.ConfigCommissions
                             where (!String.IsNullOrEmpty(nameCommission) ? (a.Name.Contains(nameCommission)) : true)
                             && a.IsActive == SystemParam.ACTIVE
                             orderby a.CreatedDate descending
                             select new ConfigCommissionModel()
                             {
                                 ID = a.ID,
                                 CommissionName = a.Name,
                                 MastersBenefit = a.MastersBenefit,
                                 IsActive = a.IsActive,
                             }).ToPagedList(page, SystemParam.MAX_ROW_IN_LIST_WEB);
                return query;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ConfigCommissionModel>().ToPagedList(1, 1);
            }
        }
        //xóa hoa hồng
        public int DeleteConfigCommission(int id)
        {
            try
            {
                ConfigCommission configCommission = cnn.ConfigCommissions.Find(id);
                configCommission.IsActive = SystemParam.NO_ACTIVE_DELETE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        //Thêm mới hoa hồng
        public int CreateConfigCommission(string commissionName, int mastersBenefit)
        {
            try
            {
                ConfigCommission configCommission = new ConfigCommission();
                configCommission.Name = commissionName;
                configCommission.MastersBenefit = mastersBenefit;
                configCommission.IsActive = 1;
                configCommission.CreatedDate = DateTime.Now;
                cnn.ConfigCommissions.Add(configCommission);
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }
        //binding theo id hoa hồng
        public ConfigCommissionModel GetConfigCommission(int id)
        {
            try
            {
                ConfigCommissionModel data = new ConfigCommissionModel();
                ConfigCommission mb = cnn.ConfigCommissions.Find(id);

                var check = cnn.ConfigCommissions.Where(m => m.ID == id).FirstOrDefault();

                //Trường hợp login là shiper
                if (check != null)
                {
                    data.ID = check.ID;
                    data.CommissionName = check.Name;
                    data.MastersBenefit = check.MastersBenefit;
                    data.IsActive = check.IsActive;
                }

                return data;
            }
            catch (Exception ex)
            {
                return new ConfigCommissionModel();
            }
        }
        //lưu khi sửa hoa hồng
        public bool SaveEditConfigCommission(ConfigCommissionModel request)
        {
            ConfigCommission sv = cnn.ConfigCommissions.Find(request.ID);

            if (sv != null)
            {
                sv.Name = string.IsNullOrEmpty(request.CommissionName) ? "" : request.CommissionName;
                sv.MastersBenefit = request.MastersBenefit;
                sv.IsActive = request.IsActive;
            }

            cnn.SaveChanges();
            return true;
        }

        /// <summary>
        /// Lấy dữ liệu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <returns></returns>
        public List<ConfigTransportAreaModel> GetDataConfigTransportArea()
        {
            try
            {
                List<ConfigTransportAreaModel> data = new List<ConfigTransportAreaModel>();
                data = cnn.ConfigTransportAreas.Where(c => c.IsActive.Equals(SystemParam.ACTIVE))
                    .Select(c => new ConfigTransportAreaModel
                    {
                        ID = c.ID,
                        Name = c.Name,
                        Type = c.Type,
                        IsProvince = c.IsProvince,
                        PerKg = (float)c.PerKg,
                        FromKm = c.FromKm,
                        ToKm = c.ToKm,
                        TimeShip = c.TimeShip,
                        Price = c.PerKgPrice

                    }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<ConfigTransportAreaModel>();
            }
        }
        /// <summary>
        /// Chi tiết cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="transportAreaID"></param>
        /// <returns></returns>
        public List<DataConfigTransportAreaPriceDetail> GetDataConfigTransportAreaPriceDetail(int transportAreaID)
        {
            try
            {
                List<DataConfigTransportAreaPriceDetail> data = new List<DataConfigTransportAreaPriceDetail>();
                data = cnn.ConfigTransportWeights.Where(c => c.TransportAreaID.Equals(transportAreaID))
                    .Select(c => new DataConfigTransportAreaPriceDetail
                    {
                        Weight = (float)c.Weight,
                        Price = c.Price,
                    }).OrderByDescending(c => c.Weight).ToList();
                return data;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return new List<DataConfigTransportAreaPriceDetail>();
            }
        }

        /// <summary>
        /// Tạo dữ liêu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel CreateDataConfigTransportArea(ConfigTransportAreaInputModel input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.Name) || String.IsNullOrEmpty(input.TimeShip))
                    return rpBus.ErrorResult("Dữ liệu chưa hợp lệ", SystemParam.PROCESS_ERROR);
                ConfigTransportArea c = new ConfigTransportArea();
                c.IsProvince = input.IsProvince;
                c.Name = input.Name;
                c.FromKm = input.FromKm;
                c.ToKm = input.ToKm;
                c.PerKg = input.PerKg;
                c.PerKgPrice = input.PerKgPrice;
                c.TimeShip = input.TimeShip;
                c.Type = input.Type;
                c.IsActive = SystemParam.ACTIVE;
                c.CreatedDate = DateTime.Now;
                cnn.ConfigTransportAreas.Add(c);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }
        /// <summary>
        /// Cập nhật dữ liêu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel UpdateDataConfigTransportArea(ConfigTransportAreaInputModel input)
        {
            try
            {
                if (String.IsNullOrEmpty(input.Name) || String.IsNullOrEmpty(input.TimeShip) || input.ID == 0)
                    return rpBus.ErrorResult("Dữ liệu chưa hợp lệ", SystemParam.PROCESS_ERROR);
                ConfigTransportArea c = cnn.ConfigTransportAreas.Find(input.ID);
                c.IsProvince = input.IsProvince;
                c.Name = input.Name;
                c.FromKm = input.FromKm;
                c.ToKm = input.ToKm;
                c.PerKg = input.PerKg;
                c.PerKgPrice = input.PerKgPrice;
                c.TimeShip = input.TimeShip;
                c.Type = input.Type;


                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();
            }
        }

        /// <summary>
        /// Xóa dữ liêu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int DeleteDataConfigTransportArea(int ID)
        {
            try
            {
                ConfigTransportArea c = cnn.ConfigTransportAreas.Find(ID);
                c.IsActive = SystemParam.ACTIVE_FALSE;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return SystemParam.ERROR;
            }
        }

        /// <summary>
        /// Thêm dữ liệu cấu hình chi tết giá vận chuyển
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public JsonResultModel AddConfigTranspotAreaPrice(ConfigTransportWeightInputModel input)
        {
            try
            {
                if (input.TransportAreaID == 0 || input.ListConfigTransportAreaPrice.Count() == 0)
                    return rpBus.ErrorResult("Dữ liệu chưa hợp lệ", SystemParam.PROCESS_ERROR);
                List<ConfigTransportWeight> cfWeight = cnn.ConfigTransportWeights.Where(cf => cf.TransportAreaID.Equals(input.TransportAreaID)).ToList();
                cnn.ConfigTransportWeights.RemoveRange(cfWeight);
                List<ConfigTransportWeight> data = input.ListConfigTransportAreaPrice.Select(c => new ConfigTransportWeight
                {
                    IsActive = SystemParam.ACTIVE,
                    Price = c.Price,
                    Weight = c.Weight,
                    TransportAreaID = input.TransportAreaID,
                    CreatedDate = DateTime.Now
                }).ToList();
                cnn.ConfigTransportWeights.AddRange(data);
                cnn.SaveChanges();
                return rpBus.SuccessResult(MessVN.SUCCESS_STR, SystemParam.SUCCESS_CODE);
            }
            catch (Exception ex)
            {
                ravenClient.Capture(new SentryEvent(ex));
                return rpBus.serverError();

            }

        }
    }
}
