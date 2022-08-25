using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FireSharp;
using PagedList;

namespace Data.Business
{
    public class BookingBusiness : GenericBusiness
    {
        private static Random random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        const string nums = "0123456789";
        const int length = 6;
        public string fullUrl = Util.getFullUrl();

        public RequestAPIBusiness requestBus = new RequestAPIBusiness();
        public CartBusiness cartBus = new CartBusiness();
        PushNotifyBusiness pushNotifyBusiness = new PushNotifyBusiness();
        public BookingBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        // Lấy phí dịch vụ theo giờ cao điểm / ban đêm
        public ConfigTimeModel GetConfigTimePrice()
        {
            try
            {
                var config = cnn.ConfigTimes.Where(x => x.IsActive == SystemParam.ACTIVE).ToList();
                foreach (var item in config)
                {
                    if (item.StartDate.TimeOfDay <= item.EndDate.TimeOfDay)
                    {
                        if (DateTime.Now.TimeOfDay >= item.StartDate.TimeOfDay && DateTime.Now.TimeOfDay <= item.EndDate.TimeOfDay)
                        {
                            return new ConfigTimeModel
                            {
                                Price = item.Price,
                                Description = item.Description
                            };
                        }
                    }
                    else
                    {
                        if (DateTime.Now.TimeOfDay >= item.StartDate.TimeOfDay || DateTime.Now.TimeOfDay <= item.EndDate.TimeOfDay)
                        {
                            return new ConfigTimeModel
                            {
                                Price = item.Price,
                                Description = item.Description
                            };
                        }
                    }

                }
                return new ConfigTimeModel
                {
                    Price = 0,
                    Description = ""
                };
            }
            catch (Exception e)
            {
                return new ConfigTimeModel
                {
                    Price = 0,
                    Description = ""
                };
            }

        }
        public List<LocationOutputModel> GetNearShipers(double Longtitude, double Latitude, double Radius)
        {
            try
            {
                var shipper = (from m in cnn.Members
                               join s in cnn.Shipers on m.ShiperID equals s.ID
                               join csh in cnn.CarShipers on s.ID equals csh.ShiperID
                               join v in cnn.VehicleTypes on csh.VehicleTypeID equals v.ID
                               where m.IsActive == SystemParam.ACTIVE && s.Lati > 0 && s.Longi > 0
                               && s.IsAcceptService == true && m.Token.Length > 10 && m.IsLogin == true
                               select new
                               {
                                   Lati = s.Lati,
                                   Longi = s.Longi,
                                   IsMotorbike = v.IsMotorbike
                               }).ToList();
                List<LocationOutputModel> listDriverLocation = new List<LocationOutputModel>();
                foreach (var item in shipper)
                {
                    var sCoord = new GeoCoordinate(item.Lati, item.Longi);
                    var eCoord = new GeoCoordinate(Latitude, Longtitude);
                    var Distance = sCoord.GetDistanceTo(eCoord);
                    if (Distance <= Radius)
                    {
                        var location = new LocationOutputModel
                        {
                            Longtitude = item.Longi,
                            Latitude = item.Lati,
                            IsMotorbike = item.IsMotorbike
                        };
                        listDriverLocation.Add(location);
                    }
                }
                return listDriverLocation;
            }
            catch
            {
                return new List<LocationOutputModel>();
            }
        }
        public async Task UpdateLocationDriverProcedure()
        {
            try
            {
                IFirebaseConfig config = new FirebaseConfig
                {
                    AuthSecret = SystemParam.FIREBASE_SECRET,
                    BasePath = SystemParam.FIREBASE_BASEPATH
                };
                var firebaseClient = new FirebaseClient(config);
                var res = await firebaseClient.GetAsync("weship");
                Dictionary<string, LocationDriverModel> dict = JsonConvert.DeserializeObject<Dictionary<string, LocationDriverModel>>(res.Body);
                var model = dict.Select(x => new UpdateLocationDriverModel
                {
                    ID = Int32.TryParse(x.Key, out int j) ? Int32.Parse(x.Key) : 0,
                    Latitude = double.TryParse(x.Value.latitude, out double q) ? double.Parse(x.Value.latitude, System.Globalization.CultureInfo.InvariantCulture) : 0,
                    Longtitude = double.TryParse(x.Value.longtitude, out double k) ? double.Parse(x.Value.longtitude, System.Globalization.CultureInfo.InvariantCulture) : 0
                }).ToList();
                foreach (var item in model)
                {
                    if (item.ID != 0 && item.Latitude > -90 && item.Latitude < 90 && item.Longtitude < 180 && item.Longtitude > -180)
                    {
                        var shipper = cnn.Shipers.FirstOrDefault(x => x.ID == item.ID);
                        if (shipper != null)
                        {
                            if (shipper.Lati != item.Latitude || shipper.Longi != item.Longtitude)
                            {
                                shipper.Lati = item.Latitude;
                                shipper.Longi = item.Longtitude;
                                shipper.ModifyDate = DateTime.Now;
                                cnn.SaveChanges();
                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {

            }

        }
        public async Task UpdateLocationDriverProcedureTest()
        {
            try
            {
                IFirebaseConfig config = new FirebaseConfig
                {
                    AuthSecret = SystemParam.FIREBASE_SECRET,
                    BasePath = SystemParam.FIREBASE_BASEPATH

                };
                IFirebaseClient firebaseClient = new FirebaseClient(config);
                var res = firebaseClient.Get(@"/weship");
                Dictionary<string, LocationDriverModel> dict = JsonConvert.DeserializeObject<Dictionary<string, LocationDriverModel>>(res.Body.ToString());
                var model = dict.Select(x => new UpdateLocationDriverModel
                {
                    ID = Int32.TryParse(x.Key, out int j) ? Int32.Parse(x.Key) : 0,
                    Latitude = double.TryParse(x.Value.latitude, out double q) ? double.Parse(x.Value.latitude, System.Globalization.CultureInfo.InvariantCulture) : 0,
                    Longtitude = double.TryParse(x.Value.longtitude, out double k) ? double.Parse(x.Value.longtitude, System.Globalization.CultureInfo.InvariantCulture) : 0
                }).ToList();
                foreach (var item in model)
                {
                    if (item.ID != 0 && item.Latitude > -90 && item.Latitude < 90 && item.Longtitude < 180 && item.Longtitude > -180)
                    {
                        var shipper = cnn.Shipers.FirstOrDefault(x => x.ID == item.ID);
                        if (shipper != null)
                        {
                            if (shipper.Lati != item.Latitude || shipper.Longi != item.Longtitude)
                            {
                                shipper.Lati = item.Latitude;
                                shipper.Longi = item.Longtitude;
                                shipper.ModifyDate = DateTime.Now;
                                cnn.SaveChanges();
                            }

                        }
                    }
                }

            }
            catch (Exception e)
            {

            }

        }

        public List<CustomerLocationSuggestModel> GetCustomerLocation(int CusId, int Type)
        {
            try
            {
                if (Type == SystemParam.SHIP_FOOD)
                {
                    var model = cnn.OrderServices.Where(x => x.CustomerID == CusId && x.TypeBooking == Type && x.FinishAddress != null).OrderByDescending(x => x.ID)
                    .GroupBy(x => x.FinishAddress).Select(x => new CustomerLocationSuggestModel
                    {
                        Address = x.FirstOrDefault().FinishAddress,
                        Lati = x.FirstOrDefault().FinishLati.HasValue ? x.FirstOrDefault().FinishLati.Value : 0,
                        Longi = x.FirstOrDefault().FinishLongi.HasValue ? x.FirstOrDefault().FinishLongi.Value : 0
                    }).Take(4).ToList();
                    return model;
                }
                else
                {
                    var model = cnn.OrderServices.Where(x => x.CustomerID == CusId && x.TypeBooking == Type && x.Address != null).OrderByDescending(x => x.ID)
                        .GroupBy(x => x.Address).Select(x => new CustomerLocationSuggestModel
                        {
                            Address = x.FirstOrDefault().Address,
                            Lati = x.FirstOrDefault().Lati.HasValue ? x.FirstOrDefault().Lati.Value : 0,
                            Longi = x.FirstOrDefault().Longi.HasValue ? x.FirstOrDefault().Longi.Value : 0
                        }).Take(4).ToList();
                    return model;
                }

            }
            catch (Exception e)
            {
                return null;
            }

        }
        // Tạo Google Map API Request
        public static string GetGoogleMapApiRequest(string origin, string destination)
        {
            return SystemParam.GOOGLE_MAP_API + "?origin=" + origin + "&destination=" + destination + "&key=" + SystemParam.GOOGLE_MAP_Key;
        }
        // Tạo Google Map Detail Place
        public static string GetGoogleMapDetailPlaceApi(string placeid)
        {
            return SystemParam.GOOGLE_MAP_DETAIL_API + "?placeid=" + placeid + "&key=" + SystemParam.GOOGLE_MAP_Key;
        }

        // Lấy danh sách đặt xe (Cảnh báo dùng hạn chế (1000 Request = 7$))
        public List<VehicleOutputModel> GetListVehicle(string origin, string destination)
        {
            try
            {
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                var map = JsonConvert.DeserializeObject<Map>(json);
                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var vehicle = cnn.ConfigTransportCosts.Where(x => x.IsActive == SystemParam.ACTIVE &&
                    x.Type == SystemParam.SHIP_DRIVER).Select(x => new
                    {
                        Name = x.VehicleType.Name,
                        ID = x.VehicleType.ID,
                        Logo = x.VehicleType.Logo,
                        Distance = distance,
                        FirstDistance = x.FirstDistance,
                        FirstPrice = x.FirstPrice,
                        PerkmPrice = x.PerKmPrice,
                        OrderIndex = x.VehicleType.OrderIndex,
                        IsMotorbike = x.VehicleType.IsMotorbike
                    }).AsEnumerable().Select(x => new VehicleOutputModel
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Logo = Util.getFullUrl() + x.Logo,
                        FirstDistance = x.FirstDistance,
                        FirstPrice = x.FirstPrice,
                        Distance = x.Distance,
                        Price = CalculateShipperFee(distance, 0, SystemParam.SHIP_DRIVER, x.ID),
                        BonusFee = GetConfigTimePrice().Price,
                        BonusFeeDescription = GetConfigTimePrice().Description,
                        PerKmPrice = x.PerkmPrice,
                        OrderIndex = x.OrderIndex,
                        IsMotorbike = x.IsMotorbike
                    }).OrderBy(x => x.OrderIndex).ToList();
                return vehicle;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        // Lấy phí ship giao hàng (Cảnh báo dùng hạn chế (1000 Request = 7$))
        public TransportAreaOutputModel GetPackageDeliveryPrice(string origin, string destination, double weight)
        {
            try
            {
                // Gọi API tính khoảng cách
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                var map = JsonConvert.DeserializeObject<Map>(json);
                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var province_start = "";
                if (mapStart.status == "OK")
                {
                    province_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_start.Contains("Tỉnh Hà Tây") || province_start.Contains("Hanoi"))
                    {
                        province_start = "Hà Nội";
                    }
                }
                // Gọi API lấy chi tiết địa điểm đến
                var reqDetailEndAddress = GetGoogleMapDetailPlaceApi(place_id_finish);
                var jsonEndAddress = requestBus.GetJson(reqDetailEndAddress);
                var mapEnd = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonEndAddress);
                var province_end = "";
                if (mapEnd.status == "OK")
                {
                    province_end = mapEnd.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_end.Contains("Tỉnh Hà Tây") || province_end.Contains("Hanoi"))
                    {
                        province_end = "Hà Nội";
                    }
                }
                var startProvinceID = cnn.Provinces.Where(x => province_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var endProvinceID = cnn.Provinces.Where(x => province_end.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var model = new List<TransportAreaModel>();
                if (startProvinceID == endProvinceID)
                {
                    var wesen = new TransportAreaModel
                    {
                        TransportType = SystemParam.TRANSPORT_TYPE_WESEN,
                        Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_WESEN),
                        IsProvince = SystemParam.IN_PROVINCE,
                        Weight = weight,
                        Distance = distance,
                        BonusFee = GetConfigTimePrice().Price,
                        BonusFeeDescription = GetConfigTimePrice().Description,
                    };
                    var shipProvince = new TransportAreaModel
                    {
                        TransportType = SystemParam.TRANSPORT_TYPE_STANDARD,
                        Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_STANDARD, SystemParam.IN_PROVINCE),
                        IsProvince = SystemParam.IN_PROVINCE,
                        Weight = weight,
                        Distance = distance,
                        BonusFee = GetConfigTimePrice().Price,
                        BonusFeeDescription = GetConfigTimePrice().Description,
                    };
                    model.Add(wesen);
                    model.Add(shipProvince);
                    return new TransportAreaOutputModel
                    {
                        Type = 1,
                        ListTransport = model
                    };
                }
                else
                {
                    var transportArea = cnn.ConfigTransportAreas.Where(x => x.IsProvince == SystemParam.OUT_PROVINCE && x.IsActive == SystemParam.ACTIVE
                    && (x.FromKm.HasValue ? distance >= x.FromKm.Value : true) && (x.ToKm.HasValue ? distance < x.ToKm.Value : true)).ToList();
                    var transportStandard = new TransportAreaModel
                    {
                        TransportType = SystemParam.TRANSPORT_TYPE_STANDARD,
                        Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_STANDARD, SystemParam.OUT_PROVINCE),
                        IsProvince = SystemParam.OUT_PROVINCE,
                        Weight = weight,
                        Distance = distance,
                        BonusFee = GetConfigTimePrice().Price,
                        BonusFeeDescription = GetConfigTimePrice().Description,
                    };
                    if (transportStandard.Price > 0)
                    {
                        model.Add(transportStandard);
                    }

                    var transportFast = new TransportAreaModel
                    {
                        TransportType = SystemParam.TRANSPORT_TYPE_FAST,
                        Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_FAST, SystemParam.OUT_PROVINCE),
                        IsProvince = SystemParam.OUT_PROVINCE,
                        Weight = weight,
                        Distance = distance,
                        BonusFee = GetConfigTimePrice().Price,
                        BonusFeeDescription = GetConfigTimePrice().Description,
                    };
                    if (transportFast.Price > 0)
                    {
                        model.Add(transportFast);
                    }
                    if (model.Count <= 1)
                    {
                        return new TransportAreaOutputModel
                        {
                            Type = 2,
                            ListTransport = model
                        };
                    }
                    else
                    {
                        return new TransportAreaOutputModel
                        {
                            Type = 3,
                            ListTransport = model
                        };
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }

        }
        // Lấy phí ship giao hàng (Cảnh báo dùng hạn chế (1000 Request = 7$))
        public List<TransportAreaModel> GetPackageDeliveryPriceVIP(string origin, string destination, double weight, int type)
        {
            try
            {
                if (String.IsNullOrEmpty(origin) || String.IsNullOrEmpty(destination) || weight == 0)
                {
                    return new List<TransportAreaModel>();
                }
                // Gọi API tính khoảng cách
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                var map = JsonConvert.DeserializeObject<Map>(json);
                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var province_start = "";
                if (mapStart.status == "OK")
                {
                    province_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_start.Contains("Tỉnh Hà Tây") || province_start.Contains("Hanoi"))
                    {
                        province_start = "Hà Nội";
                    }
                }
                // Gọi API lấy chi tiết địa điểm đến
                var reqDetailEndAddress = GetGoogleMapDetailPlaceApi(place_id_finish);
                var jsonEndAddress = requestBus.GetJson(reqDetailEndAddress);
                var mapEnd = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonEndAddress);
                var province_end = "";
                if (mapEnd.status == "OK")
                {
                    province_end = mapEnd.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_end.Contains("Tỉnh Hà Tây") || province_end.Contains("Hanoi"))
                    {
                        province_end = "Hà Nội";
                    }
                }
                var startProvinceID = cnn.Provinces.Where(x => province_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var endProvinceID = cnn.Provinces.Where(x => province_end.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var model = new List<TransportAreaModel>();
                if (type == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                {
                    var wesen = new TransportAreaModel
                    {
                        TransportType = SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE,
                        Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_WESEN),
                        IsProvince = SystemParam.IN_PROVINCE,
                        Weight = weight,
                        Distance = distance,
                        BonusFee = GetConfigTimePrice().Price,
                        BonusFeeDescription = GetConfigTimePrice().Description,
                    };
                    model.Add(wesen);
                }
                else
                {
                    if (startProvinceID == endProvinceID)
                    {
                        var shipProvince = new TransportAreaModel
                        {
                            TransportType = SystemParam.TRANSPORT_TYPE_VIP_STANDARD,
                            Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_STANDARD, SystemParam.IN_PROVINCE),
                            IsProvince = SystemParam.IN_PROVINCE,
                            Weight = weight,
                            Distance = distance,
                            BonusFee = GetConfigTimePrice().Price,
                            BonusFeeDescription = GetConfigTimePrice().Description,
                        };
                        model.Add(shipProvince);
                    }
                    else
                    {
                        var transportArea = cnn.ConfigTransportAreas.Where(x => x.IsProvince == SystemParam.OUT_PROVINCE && x.IsActive == SystemParam.ACTIVE
                        && (x.FromKm.HasValue ? distance >= x.FromKm.Value : true) && (x.ToKm.HasValue ? distance < x.ToKm.Value : true)).ToList();
                        var transportStandard = new TransportAreaModel
                        {
                            TransportType = SystemParam.TRANSPORT_TYPE_VIP_STANDARD,
                            Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_STANDARD, SystemParam.OUT_PROVINCE),
                            IsProvince = SystemParam.OUT_PROVINCE,
                            Weight = weight,
                            Distance = distance,
                            BonusFee = GetConfigTimePrice().Price,
                            BonusFeeDescription = GetConfigTimePrice().Description,
                        };
                        if (transportStandard.Price > 0)
                        {
                            model.Add(transportStandard);
                        }

                        var transportFast = new TransportAreaModel
                        {
                            TransportType = SystemParam.TRANSPORT_TYPE_VIP_AIRLINE,
                            Price = CalculateShipperFee(distance, weight, SystemParam.SHIP_PACKAGE, 1, SystemParam.TRANSPORT_TYPE_FAST, SystemParam.OUT_PROVINCE),
                            IsProvince = SystemParam.OUT_PROVINCE,
                            Weight = weight,
                            Distance = distance,
                            BonusFee = GetConfigTimePrice().Price,
                            BonusFeeDescription = GetConfigTimePrice().Description,
                        };
                        if (transportFast.Price > 0)
                        {
                            model.Add(transportFast);
                        }
                    }
                }
                return model;
            }
            catch (Exception e)
            {
                return new List<TransportAreaModel>();
            }

        }
        // Lấy phí ship giao đồ ăn (Cảnh báo dùng hạn chế (1000 Request = 7$))
        public TransportFoodModel GetFoodDeliveryPrice(int ShopID, string destination)
        {
            try
            {
                var shop = cnn.Shops.FirstOrDefault(x => x.ID == ShopID && x.IsActive == SystemParam.ACTIVE);
                var origin = shop.Lati.ToString() + "," + shop.Logi.ToString();
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                var map = JsonConvert.DeserializeObject<Map>(json);
                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var price = CalculateShipperFee(distance, 0, SystemParam.SHIP_FOOD, 1);
                var config = cnn.ConfigTransportCosts.FirstOrDefault(x => x.Type == SystemParam.SHIP_FOOD && x.IsActive == SystemParam.ACTIVE);
                return new TransportFoodModel
                {
                    Price = price,
                    Distance = distance,
                    FirstDistance = config.FirstDistance,
                    FirstPrice = config.FirstPrice,
                    PerKmPrice = config.PerKmPrice,
                    BonusFee = GetConfigTimePrice().Price,
                    BonusFeeDescription = GetConfigTimePrice().Description
                };
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public int CalculateShipperFeeVip(double distance, double weight, int TransportType, int IsProvince = SystemParam.IN_PROVINCE, int IsCOD = SystemParam.ORDER_NOT_COD_FEE)
        {
            try
            {
                double cost = 0;
                int BonusFee = GetConfigTimePrice().Price;
                if (TransportType == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                {
                    var configCost = cnn.ConfigTransportCosts.FirstOrDefault(x => x.Type == SystemParam.SHIP_PACKAGE && x.IsActive == SystemParam.ACTIVE);
                    cost += configCost.FirstPrice;
                    if (distance > configCost.FirstDistance)
                        cost += (distance - configCost.FirstDistance) * configCost.PerKmPrice;
                    if (weight > configCost.FirstWeight)
                        cost += Math.Ceiling((weight - configCost.FirstWeight.GetValueOrDefault()) / configCost.PerKg.GetValueOrDefault()) * configCost.PerKgPrice.GetValueOrDefault();
                    if (IsCOD == SystemParam.ORDER_COD_FEE)
                    {
                        cost += configCost.FeeCOD.GetValueOrDefault();
                    }
                    cost += BonusFee;
                }
                else
                {
                    var TransportTypeConvert = TransportType == SystemParam.TRANSPORT_TYPE_VIP_STANDARD ? SystemParam.TRANSPORT_TYPE_STANDARD : SystemParam.TRANSPORT_TYPE_FAST;
                    var configCost = cnn.ConfigTransportAreas.FirstOrDefault(x => x.Type == TransportTypeConvert && x.IsProvince == IsProvince &&
                        ((x.FromKm.HasValue ? distance >= x.FromKm.Value : true) && (x.ToKm.HasValue ? distance < x.ToKm.Value : true))
                        && x.IsActive == SystemParam.ACTIVE);
                    var configWeight = cnn.ConfigTransportWeights.Where(x => x.TransportAreaID == configCost.ID && x.IsActive == SystemParam.ACTIVE && x.Weight >= weight).ToList();
                    double BaseWeight = 0;
                    int FirstPrice = 0;
                    if (configWeight.Count > 0)
                    {
                        BaseWeight = configWeight.Min(x => x.Weight);
                    }
                    else
                    {
                        BaseWeight = cnn.ConfigTransportWeights.Where(x => x.IsActive == SystemParam.ACTIVE && x.TransportAreaID == configCost.ID).Max(x => x == null ? 0 : x.Weight);
                    }
                    FirstPrice = cnn.ConfigTransportWeights.Where(x => x.Weight == BaseWeight && x.TransportAreaID == configCost.ID && x.IsActive == SystemParam.ACTIVE).Select(x => x.Price).FirstOrDefault();
                    cost += FirstPrice;
                    if (weight > BaseWeight)
                    {
                        cost += Math.Ceiling((weight - BaseWeight) / configCost.PerKg) * configCost.PerKgPrice;
                    }
                    if (IsCOD == SystemParam.ORDER_COD_FEE)
                    {
                        cost += configCost.FeeCOD.GetValueOrDefault();
                    }
                }
                return (int)cost;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return 0;
            }
        }
        // Tính phí Ship
        public int CalculateShipperFee(double distance, double weight, int Type, int VehicleTypeID, int TransportType = SystemParam.TRANSPORT_TYPE_STANDARD
            , int IsProvince = SystemParam.IN_PROVINCE, int IsCOD = SystemParam.ORDER_NOT_COD_FEE)
        {
            try
            {
                var configTime = cnn.ConfigTimes.Where(x => x.IsActive == SystemParam.ACTIVE);
                double cost = 0;
                int BonusFee = GetConfigTimePrice().Price;
                //  Loại hình Tài xề 
                if (Type == SystemParam.SHIP_DRIVER)
                {
                    var configCost = cnn.ConfigTransportCosts.FirstOrDefault(x => x.Type == Type && x.VehicleTypeID == VehicleTypeID && x.IsActive == SystemParam.ACTIVE);
                    // Phí Km đầu tiên
                    if (distance <= configCost.FirstDistance)
                    {
                        cost += configCost.FirstPrice;
                    }
                    else
                    {
                        cost += configCost.FirstPrice + (distance - configCost.FirstDistance) * configCost.PerKmPrice;
                    }
                    //cost += configCost.BonusFee.GetValueOrDefault();
                    cost += BonusFee;
                }// Loại hình Giao hàng
                else if (Type == SystemParam.SHIP_PACKAGE)
                {
                    if (TransportType == SystemParam.TRANSPORT_TYPE_WESEN)
                    {
                        var configCost = cnn.ConfigTransportCosts.FirstOrDefault(x => x.Type == Type && x.IsActive == SystemParam.ACTIVE);
                        cost += configCost.FirstPrice;
                        if (distance > configCost.FirstDistance)
                            cost += (distance - configCost.FirstDistance) * configCost.PerKmPrice;
                        if (weight > configCost.FirstWeight)
                            cost += Math.Ceiling((weight - configCost.FirstWeight.GetValueOrDefault()) / configCost.PerKg.GetValueOrDefault()) * configCost.PerKgPrice.GetValueOrDefault();
                        if (IsCOD == SystemParam.ORDER_COD_FEE)
                        {
                            cost += configCost.FeeCOD.GetValueOrDefault();
                        }

                        cost += BonusFee;
                    }
                    else
                    {
                        var configCost = cnn.ConfigTransportAreas.FirstOrDefault(x => x.Type == TransportType && x.IsProvince == IsProvince &&
                        ((x.FromKm.HasValue ? distance >= x.FromKm.Value : true) && (x.ToKm.HasValue ? distance < x.ToKm.Value : true))
                        && x.IsActive == SystemParam.ACTIVE);
                        var configWeight = cnn.ConfigTransportWeights.Where(x => x.TransportAreaID == configCost.ID
                        && x.IsActive == SystemParam.ACTIVE && x.Weight >= weight).ToList();
                        double BaseWeight = 0;
                        int FirstPrice = 0;
                        if (configWeight.Count > 0)
                        {
                            BaseWeight = configWeight.Min(x => x.Weight);
                        }
                        else
                        {
                            BaseWeight = cnn.ConfigTransportWeights.Where(x => x.IsActive == SystemParam.ACTIVE && x.TransportAreaID == configCost.ID).Max(x => x == null ? 0 : x.Weight);
                        }
                        FirstPrice = cnn.ConfigTransportWeights.Where(x => x.Weight == BaseWeight && x.TransportAreaID == configCost.ID && x.IsActive == SystemParam.ACTIVE).Select(x => x.Price).FirstOrDefault();
                        cost += FirstPrice;
                        if (weight > BaseWeight)
                        {
                            cost += Math.Ceiling((weight - BaseWeight) / configCost.PerKg) * configCost.PerKgPrice;
                        }
                        if (IsCOD == SystemParam.ORDER_COD_FEE)
                        {
                            cost += configCost.FeeCOD.GetValueOrDefault();
                        }
                    }
                }
                // Loại hình Giao đồ ăn
                else
                {
                    var configCost = cnn.ConfigTransportCosts.FirstOrDefault(x => x.Type == Type && x.IsActive == SystemParam.ACTIVE);
                    // Phí Km đầu tiên
                    if (distance <= configCost.FirstDistance)
                    {
                        cost += configCost.FirstPrice;
                    }
                    else
                    {
                        cost += configCost.FirstPrice + (distance - configCost.FirstDistance) * configCost.PerKmPrice;
                    }
                    //cost += configCost.BonusFee.GetValueOrDefault();
                    cost += BonusFee;
                }
                return (int)cost;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        // Lấy đơn đặt xe hiện tại của tài xế 
        public DataPageListOutputModel GetDriverOrderInfo(int ShiperID, int type, int page, int limit, int? typeBooking, string fromDate, string toDate)
        {
            try
            {
                DateTime? fd = Util.ConvertDate(fromDate);
                DateTime? td = Util.ConvertDate(toDate);
                if (type == 0)
                {
                    DataPageListOutputModel data = new DataPageListOutputModel();
                    var model = cnn.OrderServices.Where(x => x.ShiperID == ShiperID && (x.Status == SystemParam.ORDER_STATUS_DELIVERY || x.Status == SystemParam.ORDER_STATUS_PICK_UP)
                            && (typeBooking.HasValue ? (x.TypeBooking == typeBooking) : true) && (fd.HasValue ? x.CreatedDate >= fd.Value : true) && (td.HasValue ? x.CreatedDate <= td.Value : true)
                            ).Select(x => new OrderServiceInfo
                            {
                                ID = x.ID,
                                Status = x.Status,
                                AvatarUrl = fullUrl + x.Customer.AvatarUrl,
                                BasePrice = x.BasePrice,
                                CouponPrice = x.CouponPoint,
                                CustomerName = x.Customer.Name,
                                PaymentMethod = x.PaymentType,
                                PointPrice = x.UsePoint,
                                TotalPrice = x.TotalPrice,
                                TypeBooking = x.TypeBooking,
                                AddressFrom = x.Address,
                                AddressTo = x.FinishAddress,
                                DriverPrice = x.ShiperCommission.HasValue ? x.ShiperCommission.Value : 0,
                                CreatedDate = x.CreatedDate
                            }).OrderByDescending(x => x.ID).ToList();
                    var models = model.ToPagedList(page, limit);
                    data.page = page;
                    data.limit = limit;
                    data.totalPage = (int)Math.Ceiling((double)models.TotalItemCount);
                    data.data = model;
                    return data;
                }
                else if (type == 1)
                {
                    DataPageListOutputModel data = new DataPageListOutputModel();
                    var model = (from s in cnn.Shipers
                                 join sa in cnn.ShiperAreas on s.ID equals sa.ShiperID
                                 join o in cnn.OrderServices on sa.AreaID equals o.AreaID
                                 where s.ID == ShiperID && sa.IsActive == SystemParam.ACTIVE && o.Status == SystemParam.ORDER_STATUS_PENDING
                                 && (o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VN_PAY) ? o.StatusPayment.Value.Equals(SystemParam.ORDER_PAYMENT) : true)
                                 && (s.IsVip.Value.Equals(SystemParam.SHIPPER_VIP) ? (o.TypeBooking.Equals(SystemParam.SHIP_PACKAGE) && o.TransportType.Value != SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE) :
                                 (o.TransportType.Value <= SystemParam.TRANSPORT_TYPE_WESEN))
                                 select new OrderServiceInfo
                                 {
                                     ID = o.ID,
                                     Status = o.Status,
                                     AvatarUrl = fullUrl + o.Customer.AvatarUrl,
                                     BasePrice = o.BasePrice,
                                     CouponPrice = o.CouponPoint,
                                     CustomerName = o.Customer.Name,
                                     PaymentMethod = o.PaymentType,
                                     PointPrice = o.UsePoint,
                                     TotalPrice = o.TotalPrice,
                                     TypeBooking = o.TypeBooking,
                                     AddressFrom = o.Address,
                                     AddressTo = o.FinishAddress,
                                     TransportType = o.TransportType.HasValue ? o.TransportType.Value : 0,
                                     DriverPrice = o.ShiperCommission.HasValue ? o.ShiperCommission.Value : 0,
                                     CreatedDate = o.CreatedDate
                                 }).OrderByDescending(x => x.ID).ToList();
                    var models = model.ToPagedList(page, limit);
                    data.page = page;
                    data.limit = limit;
                    data.totalPage = (int)Math.Ceiling((double)models.TotalItemCount);
                    data.data = model;
                    return data;
                }
                else
                {
                    DataPageListOutputModel data = new DataPageListOutputModel();
                    var model = (from s in cnn.Shipers
                                 join sa in cnn.ShiperAreas on s.ID equals sa.ShiperID
                                 join o in cnn.OrderServices on sa.AreaID equals o.AreaID
                                 where s.ID == ShiperID && sa.IsActive == SystemParam.ACTIVE && o.Status == SystemParam.ORDER_STATUS_PENDING
                                 && (o.PaymentType.Equals(SystemParam.PAYMENT_TYPE_VN_PAY) ? o.StatusPayment.Value.Equals(SystemParam.ORDER_PAYMENT) : true)
                                 && (s.IsVip.Value.Equals(SystemParam.SHIPPER_VIP) ? (o.TransportType.Value == SystemParam.TRANSPORT_TYPE_WESEN || o.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_STANDARD 
                                 || o.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_AIRLINE) : (o.TransportType.Value == SystemParam.TRANSPORT_TYPE_WESEN))
                                 select new OrderServiceInfo
                                 {
                                     ID = o.ID,
                                     Status = o.Status,
                                     AvatarUrl = fullUrl + o.Customer.AvatarUrl,
                                     BasePrice = o.BasePrice,
                                     CouponPrice = o.CouponPoint,
                                     CustomerName = o.Customer.Name,
                                     PaymentMethod = o.PaymentType,
                                     PointPrice = o.UsePoint,
                                     TotalPrice = o.TotalPrice,
                                     TypeBooking = o.TypeBooking,
                                     AddressFrom = o.Address,
                                     AddressTo = o.FinishAddress,
                                     TransportType = o.TransportType.HasValue ? o.TransportType.Value : 0,
                                     DriverPrice = o.ShiperCommission.HasValue ? o.ShiperCommission.Value : 0,
                                     CreatedDate = o.CreatedDate
                                 }).OrderByDescending(x => x.ID).ToList();
                    var models = model.ToPagedList(page, limit);
                    data.page = page;
                    data.limit = limit;
                    data.totalPage = (int)Math.Ceiling((double)models.TotalItemCount);
                    data.data = model;
                    return data;
                }


            }
            catch (Exception e)
            {
                return null;
            }
        }

        // Từ chối tiếp nhận đặt xe
        public int DeclineOrderService(int OrderServiceID, int ShiperId)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return SystemParam.ORDER_NOT_FOUND;
                }
                var shiper = cnn.Shipers.Where(x => x.ID == ShiperId).FirstOrDefault();
                if (shiper == null)
                {
                    return SystemParam.SHIPPER_NOT_FOUND;
                }

                if (ShiperId == orderService.FirstShiperID && orderService.IsPushFirst == SystemParam.ORDER_NOT_PUSH_FIRST
                    && orderService.TransportType != SystemParam.TRANSPORT_TYPE_STANDARD && orderService.TransportType != SystemParam.TRANSPORT_TYPE_FAST)
                {
                    orderService.IsPushFirst = SystemParam.ORDER_PUSH_FIRST;
                    orderService.PushFirstDate = DateTime.Now;
                    cnn.SaveChanges();
                    // Lấy danh sách tài xế hoạt động trong khu vực
                    var listDriverID = (from sh in cnn.Shipers
                                        join m in cnn.Members on sh.ID equals m.ShiperID
                                        join a in cnn.ShiperAreas on sh.ID equals a.ShiperID
                                        join cs in cnn.CarShipers on sh.ID equals cs.ShiperID
                                        join w in cnn.Wallets on m.ID equals w.MemberID
                                        where m.IsActive == SystemParam.ACTIVE && (a.AreaID == orderService.AreaID || a.Area.Name == orderService.Area.Name) && sh.IsAcceptService == true && sh.ID != ShiperId
                                        && cs.VehicleTypeID == orderService.CarTypeID && m.DeviceID.Length > 10 && w.IsActive == SystemParam.ACTIVE && w.Type == Constant.WALLET_NO_WITHDRAW
                                        && (sh.IsInternal == SystemParam.SHIPPER_INTERNAL ? true : (orderService.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY ? true :
                                        (w.Balance >= orderService.TotalPrice)))
                                        group sh by sh.ID into g
                                        select g.FirstOrDefault().ID).ToList();
                    foreach (var item in listDriverID)
                    {
                        orderService.OtherShiper += item + ",";
                    }
                    PushDriverMultipleRequest(OrderServiceID, listDriverID, orderService.TypeBooking);
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }

        }
        // Tiếp nhận đặt xe , giao hàng , giao đồ ăn
        public int AcceptOrderService(int OrderServiceID, int ShiperId)
        {
            using (var dbContextTransaction = cnn.Database.BeginTransaction())
            {

                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == ShiperId);
                var member = cnn.Members.FirstOrDefault(x => x.ShiperID == ShiperId);
                if (orderService == null)
                {
                    dbContextTransaction.Rollback();
                    return SystemParam.ORDER_NOT_FOUND;
                }
                if (orderService.Status >= SystemParam.ORDER_STATUS_DELIVERY)
                {
                    dbContextTransaction.Rollback();
                    return SystemParam.BOOK_DRIVER_ALREADY_PICK_UP;
                }
                if (orderService.Status == SystemParam.ORDER_STATUS_DENY)
                {
                    dbContextTransaction.Rollback();
                    return SystemParam.BOOK_DRIVER_DECLINE;
                }
                if (shiper == null || member == null)
                {
                    dbContextTransaction.Rollback();
                    return SystemParam.SHIPPER_NOT_FOUND;
                }
                cnn.Database.ExecuteSqlCommand(
@"Update OrderService SET status = " + SystemParam.ORDER_STATUS_DELIVERY + " where ID = " + OrderServiceID);
                var TypeBooking = orderService.TypeBooking;
                var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                          SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_PACKAGE : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_ACCEPT_FOOD : 0;
                var content = TypeBooking == SystemParam.SHIP_DRIVER ? "Đặt xe" : TypeBooking == SystemParam.SHIP_PACKAGE ?
              "Giao hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "Giao đồ ăn" : "";
                if (shiper.IsInternal == SystemParam.SHIPPER_PARTNER)
                {
                    // Ví cọc
                    var wallet1 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_NO_WITHDRAW && x.MemberID == member.ID);
                    // Ví thu nhập
                    var wallet2 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_WITHDRAW && x.MemberID == member.ID);

                    if (wallet1 == null || wallet2 == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.WALLET_NOT_FOUND;
                    }
                    if (orderService.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY)
                    {
                        var totalPrice = orderService.TotalPrice;

                        if (wallet1.Balance >= totalPrice)
                        {
                            if (totalPrice > 0 && orderService.TransportType.GetValueOrDefault() < SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                            {
                                var transaction = new MembersTransactionHistory
                                {
                                    MemberID = member.ID,
                                    OrderServiceID = orderService.ID,
                                    Title = "Trừ tiền ví cọc ",
                                    Content = content + " thanh toán tiền mặt ",
                                    Point = totalPrice,
                                    Type = Constant.SUBTRACT_POINT,
                                    TransactionType = Constant.TYPE_TRANSACTION_ACCEPT_ORDER,
                                    TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                    BeforeBalance = wallet1.Balance,
                                    AfterBalance = wallet1.Balance - totalPrice,
                                    Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                    IsActive = SystemParam.ACTIVE,
                                    CreateDate = DateTime.Now,
                                    Icon = "",
                                    WalletID = wallet1.ID,
                                    IsExtra = true,
                                };
                                cnn.MembersTransactionHistories.Add(transaction);
                                wallet1.Balance -= totalPrice;
                                var noti = new Notification
                                {
                                    Title = "Hệ thống đã thu của bạn " + Util.ConvertCurrency(totalPrice) + "đ phí dịch vụ",
                                    Content = "Hệ thống đã thu của bạn " + Util.ConvertCurrency(totalPrice) + "đ phí dịch vụ",
                                    Type = SystemParam.NOTI_TYPE_NAVIGATE_MINUS_MONEY_DRIVER,
                                    IsActive = SystemParam.ACTIVE,
                                    CreateDate = DateTime.Now,
                                    IsRead = false,
                                    MemberID = member.ID,
                                    Code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                                    OrderServiceID = OrderServiceID
                                };
                                cnn.Notifications.Add(noti);
                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.WALLET_NOT_ENOUGH_MONEY;
                        }
                    }

                }

                var memberCustomer = cnn.Members.FirstOrDefault(x => x.CustomerID == orderService.CustomerID);
                if (memberCustomer == null)
                {
                    dbContextTransaction.Rollback();
                    return SystemParam.CUSTOMER_NOT_FOUND;
                }
                // Cập nhật thời gian chờ tài xế đến
                var shiperOrigin = shiper.Lati.ToString() + "," + shiper.Longi.ToString();
                var cusOrigin = orderService.Lati.ToString() + "," + orderService.Longi.ToString();
                var estimateTime = 0;
                var req = GetGoogleMapApiRequest(shiperOrigin, cusOrigin);
                var json = requestBus.GetJson(req);
                var map = JsonConvert.DeserializeObject<Map>(json);
                if (map == null)
                {
                    dbContextTransaction.Rollback();
                    return SystemParam.LOCATION_NOT_VALID;

                }
                try
                {
                    estimateTime = map.routes[0].legs[0].duration.Value / SystemParam.Minute;
                }
                catch (Exception e)
                {
                    estimateTime = 0;
                }
                orderService.TimeWait = estimateTime;
                orderService.StartDate = DateTime.Now;
                orderService.ShiperID = shiper.ID;

                var commission = cnn.Shipers.Where(x => x.ID == ShiperId && x.IsInternal == SystemParam.SHIPPER_PARTNER).Select(x => x.ConfigCommission.MastersBenefit).FirstOrDefault();
                orderService.ShiperCommission = orderService.BasePrice * commission / 100;
                var title = "Đã tìm thấy tài xế";
                pushNotifyBusiness.PushNotifyapp(title, type, OrderServiceID, memberCustomer.ID, 1);
                //orderService.Status = SystemParam.ORDER_STATUS_DELIVERY;
                cnn.SaveChanges();
                dbContextTransaction.Commit();
                dbContextTransaction.Dispose();
                return SystemParam.SUCCESS;

            }


        }
        // Kiểm tra Coupon 
        public bool CheckCoupon(int CouponId, int CustomerId, int? Type, int? Used)
        {
            try
            {
                var customer = cnn.Customers.FirstOrDefault(x => x.ID == CustomerId);
                var coupon = cnn.Coupons.FirstOrDefault(x => x.ID == CouponId && x.IsActive == SystemParam.ACTIVE);
                var CouponCustomer = cnn.CouponCustomers.Where(x => x.CouponID == CouponId).ToList();
                if (coupon == null)
                {
                    return false;
                }
                if (CouponCustomer.Count() == 0)
                {
                    if (coupon.Remain <= 0 || customer.CustomerRank.Level != coupon.CustomerRank.Level)
                    {
                        return false;
                    }
                }
                else
                {
                    var check = CouponCustomer.FirstOrDefault(x => x.CustomerID == CustomerId && x.CouponID == CouponId);
                    if (check == null)
                    {
                        return false;
                    }
                }
                if (Type.HasValue)
                {
                    if (coupon.Type != Type.Value)
                    {
                        return false;
                    }
                }
                if (Used.HasValue)
                {
                    var checkUsed = cnn.OrderServices.Any(x => x.CustomerID == CustomerId && x.CouponID == CouponId);
                    if (Used == SystemParam.COUPON_NOT_USED)
                    {
                        return !checkUsed;
                    }
                    else if (Used == SystemParam.COUPON_USED)
                    {
                        return checkUsed;
                    }
                }



                if (coupon.TypeTime == SystemParam.COUPON_TYPE_TIME_LIMIT)
                {
                    if (coupon.StartDate.HasValue)
                    {
                        if (DateTime.Now.Date < coupon.StartDate.Value.Date)
                            return false;
                    }
                    if (coupon.ExpriceDate.HasValue)
                    {
                        if (DateTime.Now.Date > coupon.ExpriceDate.Value.Date)
                            return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        // Lấy điểm khách hàng
        public int GetCustomerPoint(int CustomerId)
        {
            try
            {
                var cus = cnn.Customers.FirstOrDefault(x => x.ID == CustomerId);
                return cus.RankingPoint;
            }
            catch (Exception e)
            {
                return 0;
            }
        }
        public List<CouponOutputModel> GetListCoupon(int CustomerId, int? Type, int? Used)
        {
            try
            {
                var model = cnn.Coupons.Select(x => new CouponOutputModel
                {
                    ID = x.ID,
                    Code = x.Code,
                    Name = x.Name,
                    Content = x.Content,
                    StartDate = x.StartDate,
                    ExprireDate = x.ExpriceDate,
                    Discount = x.Discount,
                    Percent = x.Percent,
                    TypeCoupon = x.TypeCoupon,
                    Type = x.Type,
                    TypeTime = x.TypeTime,
                    RankId = x.RankID.HasValue ? x.RankID.Value : 0,
                    RankName = x.CustomerRank.Description,
                    ServiceName = x.Type == SystemParam.COUPON_TYPE_DRIVER ? SystemParam.COUPON_TYPE_DRIVER_STR : x.Type == SystemParam.COUPON_TYPE_PACKAGE
                                ? SystemParam.COUPON_TYPE_PACKAGE_STR : x.Type == SystemParam.COUPON_TYPE_FOOD ? SystemParam.COUPON_TYPE_FOOD_STR : ""
                }).AsEnumerable().Where(x => CheckCoupon(x.ID, CustomerId, Type, Used)).ToList();
                return model;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        // Không đón khách
        public int NotPickUpCustomer(int OrderServiceID, int ShiperID, string Reason)
        {
            try
            {
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {

                    var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);

                    if (orderService == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_NOT_FOUND;
                    }
                    var beforeStatus = orderService.Status;
                    if (orderService.Status == SystemParam.ORDER_STATUS_DENY)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_ALREADY_DECLINE;
                    }
                    cnn.Database.ExecuteSqlCommand(@"Update OrderService SET status = " + SystemParam.ORDER_STATUS_DENY + "  where ID = " + OrderServiceID);
                    if (orderService.Status == SystemParam.ORDER_STATUS_FINISH)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_ALREADY_COMPLETE;
                    }
                    if (orderService.ShiperID == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.BOOK_DRIVER_NOT_PICK_UP;
                    }
                    if (orderService.ShiperID != ShiperID)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_DRIVER_NO_PERMISSION;
                    }
                    var member = cnn.Members.FirstOrDefault(x => x.ShiperID == orderService.ShiperID);
                    var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == ShiperID);
                    var TypeBooking = orderService.TypeBooking;
                    var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                              SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD : 0;
                    var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Hủy đơn đặt xe " : TypeBooking == SystemParam.SHIP_PACKAGE ?
    "Hủy đơn giao hàng " : TypeBooking == SystemParam.SHIP_FOOD ? "Hủy đơn giao đồ ăn " : "";
                    title += orderService.Code;
                    var totalPrice = orderService.BasePrice - orderService.CouponPoint - orderService.UsePoint;
                    if (totalPrice < 0)
                    {
                        totalPrice = 0;
                    }
                    var walletCus = cnn.Wallets.FirstOrDefault(x => x.Member.CustomerID == orderService.CustomerID);
                    if (walletCus == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                    }
                    if (orderService.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY)
                    {
                        if (shiper.IsInternal == SystemParam.SHIPPER_PARTNER)
                        {
                            if (totalPrice > 0 && (orderService.TransportType.GetValueOrDefault() < SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE
                                || (orderService.TransportType.GetValueOrDefault() >= SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE && beforeStatus == SystemParam.ORDER_STATUS_PICK_UP)))
                            {
                                // Ví cọc
                                var wallet1 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_NO_WITHDRAW && x.MemberID == member.ID);
                                // Ví thu nhập
                                var wallet2 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_WITHDRAW && x.MemberID == member.ID);

                                if (wallet1 == null || wallet2 == null)
                                {
                                    dbContextTransaction.Rollback();
                                    return SystemParam.WALLET_NOT_FOUND;
                                }
                                var titleDriverRefund = "Bạn đã được hoàn lại " + Util.ConvertCurrency(totalPrice) + "đ vào ví cọc";
                                var typeDriverRefund = SystemParam.NOTI_TYPE_WITH_REFUND_MONEY;
                                var transaction = new MembersTransactionHistory
                                {
                                    MemberID = member.ID,
                                    OrderServiceID = orderService.ID,
                                    Title = "Hoàn tiền ví cọc ",
                                    Content = title + " thanh toán tiền mặt ",
                                    Point = totalPrice,
                                    Type = Constant.TRANSACTION_ADD_POINT,
                                    TransactionType = Constant.TYPE_TRANSACTION_REFUND_ORDER_CANCLE,
                                    TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                    BeforeBalance = wallet1.Balance,
                                    AfterBalance = wallet1.Balance + totalPrice,
                                    Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                    IsActive = SystemParam.ACTIVE,
                                    CreateDate = DateTime.Now,
                                    Icon = "",
                                    WalletID = wallet1.ID,
                                    IsExtra = false,
                                };
                                wallet1.Balance += totalPrice;
                                cnn.MembersTransactionHistories.Add(transaction);
                                pushNotifyBusiness.PushNotifyapp(titleDriverRefund, typeDriverRefund, OrderServiceID, member.ID, 2);
                            }
                            pushNotifyBusiness.PushNotifyapp(title, type, OrderServiceID, member.ID, 2);
                        }

                        if (orderService.UsePoint > 0)
                        {
                            var titleCusRefund = "Bạn đã được hoàn lại " + orderService.UsePoint + " điểm thưởng";
                            var typeCusRefund = SystemParam.NOTI_TYPE_WITH_REFUND_MONEY;
                            var transactionCus = new MembersTransactionHistory
                            {
                                MemberID = walletCus.MemberID,
                                OrderServiceID = orderService.ID,
                                Title = "Hoàn điểm tích lũy",
                                Content = title,
                                Point = orderService.UsePoint,
                                Type = Constant.TRANSACTION_ADD_POINT,
                                TransactionType = Constant.TYPE_TRANSACTION_REFUND_USE_POINT,
                                TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                BeforeBalance = walletCus.Balance,
                                AfterBalance = walletCus.Balance + orderService.UsePoint,
                                Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                IsActive = SystemParam.ACTIVE,
                                CreateDate = DateTime.Now,
                                Icon = "",
                                WalletID = walletCus.ID,
                                IsExtra = false,
                            };
                            walletCus.Balance += orderService.UsePoint;
                            cnn.MembersTransactionHistories.Add(transactionCus);
                            pushNotifyBusiness.PushNotifyapp(title, type, OrderServiceID, walletCus.MemberID, 1);
                            pushNotifyBusiness.PushNotifyapp(titleCusRefund, typeCusRefund, OrderServiceID, walletCus.MemberID, 1);
                        }
                        else
                        {
                            pushNotifyBusiness.PushNotifyapp(title, type, OrderServiceID, walletCus.MemberID, 1);
                        }
                        cnn.SaveChanges();
                    }
                    else if (orderService.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY && orderService.StatusPayment == SystemParam.ORDER_PAYMENT)
                    {
                        var pointCus = orderService.TotalPrice + orderService.UsePoint;
                        var titleCusRefund = "Bạn đã được hoàn lại " + pointCus + " điểm thưởng";
                        var typeCusRefund = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                        var transaction = new MembersTransactionHistory
                        {
                            MemberID = walletCus.Member.ID,
                            OrderServiceID = orderService.ID,
                            Title = "Hoàn tiền ví khách hàng ",
                            Content = title + " thanh toán VNPay ",
                            Point = orderService.TotalPrice + orderService.UsePoint,
                            Type = Constant.TRANSACTION_ADD_POINT,
                            TransactionType = Constant.TYPE_TRANSACTION_VNPAY_REFUND,
                            TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                            BeforeBalance = walletCus.Balance,
                            AfterBalance = walletCus.Balance + pointCus,
                            Status = Constant.STATUS_TRANSACTION_SUCCESS,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            Icon = "",
                            WalletID = walletCus.ID,
                            IsExtra = false,
                        };
                        walletCus.Balance += pointCus;
                        cnn.MembersTransactionHistories.Add(transaction);
                        cnn.SaveChanges();
                        pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, walletCus.MemberID, 1);
                        pushNotifyBusiness.PushNotifyapp(titleCusRefund, typeCusRefund, orderService.ID, walletCus.MemberID, 1);
                    }
                    orderService.CancleDate = DateTime.Now;
                    orderService.ReasonCancel = Reason;
                    orderService.UserCancel = SystemParam.ORDER_SHIPER_CANCLE;
                    cnn.SaveChanges();
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return SystemParam.SUCCESS;
                }

            }
            catch (Exception e)
            {
                return SystemParam.DECLINE_BOOK_DRIVER_FAIL;
            }
        }
        // Hủy chuyến lần 1
        public int DeclineOrderServiceFirst(int OrderServiceId, int CustomerId)
        {
            try
            {
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {
                    var constCancle = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_CANCLETIME).Select(x => x.ValueConstant).FirstOrDefault();
                    var MaxCancleTime = SystemParam.CUS_MAX_CANCLE_TIME;
                    if (constCancle != null && constCancle != "")
                    {
                        try
                        {
                            MaxCancleTime = Int32.Parse(constCancle);
                        }
                        catch
                        {
                            MaxCancleTime = SystemParam.CUS_MAX_CANCLE_TIME;
                        }
                    }
                    var OrderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceId);
                    if (OrderService == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_NOT_FOUND;
                    }
                    if (OrderService.Status > SystemParam.ORDER_STATUS_PENDING)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.BOOK_DRIVER_ALREADY_PICK_UP;
                    }
                    if (OrderService.Status == SystemParam.ORDER_STATUS_DENY)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.BOOK_DRIVER_ALREADY_DECLINE;
                    }
                    cnn.Database.ExecuteSqlCommand(@"Update OrderService SET status = " + SystemParam.ORDER_STATUS_DENY + "  where ID = " + OrderServiceId);
                    if (OrderService.CustomerID != CustomerId)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_CUSTOMER_NO_PERMISSION;
                    }
                    var customer = cnn.Customers.FirstOrDefault(x => x.ID == OrderService.CustomerID);
                    var member = cnn.Members.FirstOrDefault(x => x.CustomerID == OrderService.CustomerID);
                    if (customer.QTYCancel > MaxCancleTime)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.CUSTOMER_CAN_NOT_CANCLE_ORDER;
                    }
                    customer.QTYCancel++;
                    var walletCus = cnn.Wallets.FirstOrDefault(x => x.Member.CustomerID == CustomerId);
                    if (walletCus == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                    }
                    var TypeBooking = OrderService.TypeBooking;
                    var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER_SHIPER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                          SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE_SHIPER : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD_SHIPER : 0;
                    var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Hủy đơn đặt xe " : TypeBooking == SystemParam.SHIP_PACKAGE ?
    "Hủy đơn giao hàng " : TypeBooking == SystemParam.SHIP_FOOD ? "Hủy đơn giao đồ ăn " : "";
                    title += OrderService.Code;
                    if (OrderService.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY && OrderService.StatusPayment == SystemParam.ORDER_PAYMENT)
                    {
                        var point = OrderService.TotalPrice + OrderService.UsePoint;

                        var transaction = new MembersTransactionHistory
                        {
                            MemberID = walletCus.Member.ID,
                            OrderServiceID = OrderService.ID,
                            Title = title + " thanh toán VNPay ",
                            Content = title + " thanh toán VNPay ",
                            Point = point,
                            Type = Constant.TRANSACTION_ADD_POINT,
                            TransactionType = Constant.TYPE_TRANSACTION_VNPAY_REFUND,
                            TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                            BeforeBalance = walletCus.Balance,
                            AfterBalance = walletCus.Balance + point,
                            Status = Constant.STATUS_TRANSACTION_SUCCESS,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            Icon = "",
                            WalletID = walletCus.ID,
                            IsExtra = false,
                        };
                        walletCus.Balance += point;
                        cnn.MembersTransactionHistories.Add(transaction);
                        pushNotifyBusiness.PushNotifyapp(title, type, OrderService.ID, member.ID, 1);
                        if (point > 0)
                        {
                            var titleRefund = "Bạn đã được hoàn lại " + point + " điểm thưởng ";
                            var typeRefund = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                            pushNotifyBusiness.PushNotifyapp(titleRefund, typeRefund, OrderService.ID, member.ID, 1);
                        }
                        cnn.SaveChanges();
                    }
                    else if (OrderService.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY)
                    {
                        if (OrderService.UsePoint > 0)
                        {
                            var titleRefund = "Bạn đã được hoàn lại " + OrderService.UsePoint + " điểm thưởng ";
                            var typeRefund = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                            var transaction = new MembersTransactionHistory
                            {
                                MemberID = walletCus.Member.ID,
                                OrderServiceID = OrderService.ID,
                                Title = title,
                                Content = titleRefund,
                                Point = OrderService.UsePoint,
                                Type = Constant.TRANSACTION_ADD_POINT,
                                TransactionType = Constant.TYPE_TRANSACTION_REFUND_USE_POINT,
                                TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                BeforeBalance = walletCus.Balance,
                                AfterBalance = walletCus.Balance + OrderService.UsePoint,
                                Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                IsActive = SystemParam.ACTIVE,
                                CreateDate = DateTime.Now,
                                Icon = "",
                                WalletID = walletCus.ID,
                                IsExtra = false,
                            };
                            walletCus.Balance += OrderService.UsePoint;
                            cnn.MembersTransactionHistories.Add(transaction);
                            pushNotifyBusiness.PushNotifyapp(titleRefund, typeRefund, OrderService.ID, member.ID, 1);
                        }
                        cnn.SaveChanges();
                        pushNotifyBusiness.PushNotifyapp(title, type, OrderService.ID, member.ID, 1);
                    }
                    OrderService.CancleDate = DateTime.Now;
                    cnn.SaveChanges();
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return SystemParam.SUCCESS;
                }

            }
            catch (Exception ex)
            {
                return SystemParam.ERROR;
            }
        }
        // Khách hàng hủy chuyến , hủy đơn giao hàng
        public int DeclineOrderServiceSecond(int OrderServiceID, int CustomerID, string Reason)
        {
            try
            {
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {
                    var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                    if (orderService == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_NOT_FOUND;
                    }
                    if (orderService.Status == SystemParam.ORDER_STATUS_DENY)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_ALREADY_DECLINE;
                    }
                    if (orderService.Status == SystemParam.ORDER_STATUS_FINISH)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_ALREADY_COMPLETE;
                    }
                    cnn.Database.ExecuteSqlCommand(@"Update OrderService SET status = " + SystemParam.ORDER_STATUS_DENY + "  where ID = " + OrderServiceID);
                    if (orderService.TypeBooking == SystemParam.SHIP_FOOD)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_NOT_DECLINE;
                    }
                    if (orderService.ShiperID == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.BOOK_DRIVER_NOT_PICK_UP;
                    }
                    if (orderService.CustomerID != CustomerID)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_CUSTOMER_NO_PERMISSION;
                    }
                    var member = cnn.Members.FirstOrDefault(x => x.ShiperID == orderService.ShiperID);
                    var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                    var memberCus = cnn.Members.FirstOrDefault(x => x.CustomerID == orderService.CustomerID);
                    orderService.Status = SystemParam.ORDER_STATUS_DENY;
                    orderService.CancleDate = DateTime.Now;
                    orderService.ReasonCancel = Reason;
                    orderService.UserCancel = SystemParam.ORDER_USER_CANCLE;
                    var constCancle = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_CANCLETIME).Select(x => x.ValueConstant).FirstOrDefault();
                    var MaxCancleTime = SystemParam.CUS_MAX_CANCLE_TIME;
                    var customer = cnn.Customers.FirstOrDefault(x => x.ID == CustomerID);
                    if (constCancle != null && constCancle != "")
                    {
                        try
                        {
                            MaxCancleTime = Int32.Parse(constCancle);
                        }
                        catch
                        {
                            MaxCancleTime = SystemParam.CUS_MAX_CANCLE_TIME;
                        }
                    }
                    if (customer.QTYCancel > MaxCancleTime)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.CUSTOMER_CAN_NOT_CANCLE_ORDER;
                    }
                    customer.QTYCancel++;
                    var TypeBooking = orderService.TypeBooking;
                    var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER_SHIPER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                              SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE_SHIPER : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD_SHIPER : 0;
                    var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Hủy đơn đặt xe " : TypeBooking == SystemParam.SHIP_PACKAGE ?
    "Hủy đơn giao hàng " : TypeBooking == SystemParam.SHIP_FOOD ? "Hủy đơn giao đồ ăn " : "";
                    title += orderService.Code;
                    var totalPrice = orderService.BasePrice - orderService.UsePoint - orderService.CouponPoint;
                    if (totalPrice < 0)
                    {
                        totalPrice = 0;
                    }
                    if (orderService.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY)
                    {
                        if (shiper.IsInternal == SystemParam.SHIPPER_PARTNER)
                        {
                            if (totalPrice > 0 && orderService.TransportType.GetValueOrDefault() < SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                            {
                                // Ví cọc
                                var wallet1 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_NO_WITHDRAW && x.MemberID == member.ID);
                                // Ví thu nhập
                                var wallet2 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_WITHDRAW && x.MemberID == member.ID);

                                if (wallet1 == null || wallet2 == null)
                                {
                                    return SystemParam.WALLET_NOT_FOUND;
                                }
                                var titleDriverRefundMoney = "Bạn đã được hoàn lại " + Util.ConvertCurrency(totalPrice) + "đ vào ví cọc";
                                var typeDriverRefundMoney = SystemParam.NOTI_TYPE_WITH_REFUND_MONEY;
                                var transaction = new MembersTransactionHistory
                                {
                                    MemberID = member.ID,
                                    OrderServiceID = orderService.ID,
                                    Title = "Hoàn tiền ví cọc ",
                                    Content = title + " thanh toán tiền mặt ",
                                    Point = totalPrice,
                                    Type = Constant.TRANSACTION_ADD_POINT,
                                    TransactionType = Constant.TYPE_TRANSACTION_REFUND_ORDER_CANCLE,
                                    TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                    BeforeBalance = wallet1.Balance,
                                    AfterBalance = wallet1.Balance + totalPrice,
                                    Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                    IsActive = SystemParam.ACTIVE,
                                    CreateDate = DateTime.Now,
                                    Icon = "",
                                    WalletID = wallet1.ID,
                                    IsExtra = false,
                                };
                                wallet1.Balance += totalPrice;
                                cnn.MembersTransactionHistories.Add(transaction);
                                cnn.SaveChanges();
                                pushNotifyBusiness.PushNotifyapp(titleDriverRefundMoney, typeDriverRefundMoney, orderService.ID, member.ID, 2);
                            }
                            pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, member.ID, 2);
                        }
                        else
                        {
                            pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, member.ID, 2);
                        }

                        if (orderService.UsePoint > 0)
                        {
                            var titleCus = title;
                            var titleCusRefundUsePoint = "Bạn đã được hoàn lại " + orderService.UsePoint + " điểm thưởng";
                            var typeUsePoint = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                            var walletCus = cnn.Wallets.FirstOrDefault(x => x.Member.CustomerID == CustomerID);
                            if (walletCus == null)
                            {
                                dbContextTransaction.Rollback();
                                return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                            }
                            var transactionCus = new MembersTransactionHistory
                            {
                                MemberID = memberCus.ID,
                                OrderServiceID = orderService.ID,
                                Title = "Hoàn điểm tích lũy",
                                Content = title,
                                Point = orderService.UsePoint,
                                Type = Constant.TRANSACTION_ADD_POINT,
                                TransactionType = Constant.TYPE_TRANSACTION_REFUND_USE_POINT,
                                TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                BeforeBalance = walletCus.Balance,
                                AfterBalance = walletCus.Balance + orderService.UsePoint,
                                Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                IsActive = SystemParam.ACTIVE,
                                CreateDate = DateTime.Now,
                                Icon = "",
                                WalletID = walletCus.ID,
                                IsExtra = false,
                            };
                            walletCus.Balance += orderService.UsePoint;
                            cnn.MembersTransactionHistories.Add(transactionCus);
                            cnn.SaveChanges();
                            pushNotifyBusiness.PushNotifyapp(titleCus, type, orderService.ID, memberCus.ID, 1);
                            pushNotifyBusiness.PushNotifyapp(titleCusRefundUsePoint, typeUsePoint, orderService.ID, memberCus.ID, 1);
                        }
                        else
                        {
                            pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, memberCus.ID, 1);
                        }

                    }
                    else if (orderService.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY && orderService.StatusPayment == SystemParam.ORDER_PAYMENT)
                    {
                        var pointCus = orderService.TotalPrice + orderService.UsePoint;
                        var titleCusRefund = "Bạn đã được hoàn lại " + pointCus + " điểm thưởng";
                        var typeCusRefund = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                        var walletCus = cnn.Wallets.FirstOrDefault(x => x.Member.CustomerID == CustomerID);
                        if (walletCus == null)
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                        }
                        var transaction = new MembersTransactionHistory
                        {
                            MemberID = memberCus.ID,
                            OrderServiceID = orderService.ID,
                            Title = "Hoàn tiền ví khách hàng ",
                            Content = title + " thanh toán VNPay ",
                            Point = pointCus,
                            Type = Constant.TRANSACTION_ADD_POINT,
                            TransactionType = Constant.TYPE_TRANSACTION_VNPAY_REFUND,
                            TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                            BeforeBalance = walletCus.Balance,
                            AfterBalance = walletCus.Balance + pointCus,
                            Status = Constant.STATUS_TRANSACTION_SUCCESS,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            Icon = "",
                            WalletID = walletCus.ID,
                            IsExtra = false,
                        };
                        walletCus.Balance += pointCus;
                        cnn.MembersTransactionHistories.Add(transaction);
                        cnn.SaveChanges();
                        pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, memberCus.ID, 1);
                        pushNotifyBusiness.PushNotifyapp(titleCusRefund, typeCusRefund, orderService.ID, memberCus.ID, 1);
                        pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, member.ID, 2);
                    }
                    cnn.SaveChanges();
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return SystemParam.SUCCESS;
                }

            }
            catch (Exception e)
            {
                return SystemParam.DECLINE_BOOK_DRIVER_FAIL;
            }
        }
        // Hoàn thành đơn đặt xe , giao hàng , giao đồ ăn
        public int CompleteOrderService(int OrderServiceID, int ShiperID)
        {
            try
            {
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {
                    var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                    if (orderService == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_NOT_FOUND;
                    }
                    if (orderService.Status == SystemParam.ORDER_STATUS_FINISH)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_ALREADY_COMPLETE;
                    }
                    if (orderService.Status == SystemParam.ORDER_STATUS_DENY)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_ALREADY_DECLINE;
                    }
                    cnn.Database.ExecuteSqlCommand(@"Update OrderService SET status = " + SystemParam.ORDER_STATUS_FINISH + "  where ID = " + OrderServiceID);
                    var customer = cnn.Customers.FirstOrDefault(x => x.ID == orderService.CustomerID);
                    if (customer == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.CUSTOMER_NOT_FOUND;
                    }
                    var shipper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                    if (shipper == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.SHIPPER_NOT_FOUND;
                    }
                    if (orderService.ShiperID != ShiperID)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_DRIVER_NO_PERMISSION;
                    }
                    var memberShipper = cnn.Members.FirstOrDefault(x => x.ShiperID == orderService.ShiperID);

                    // Xử lý đơn hàng 
                    customer.QTYCancel = 0;
                    orderService.CompletedDate = DateTime.Now;
                    orderService.StatusPayment = SystemParam.ORDER_PAYMENT;

                    // APP Khách hàng 

                    var member = cnn.Members.FirstOrDefault(x => x.CustomerID == customer.ID);
                    var walletCus = cnn.Wallets.FirstOrDefault(x => x.MemberID == member.ID);
                    if (walletCus == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                    }
                    var customerRank = cnn.CustomerRanks.FirstOrDefault(x => x.ID == customer.CustomerRankID);
                    var EarnPointPercent = orderService.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY ? customerRank.ProfitCash : customerRank.ProfitVPN;
                    var EarnPoint = orderService.TotalPrice * EarnPointPercent / 100;
                    var birthday = customer.DOB.GetValueOrDefault().Date;
                    if (birthday == DateTime.Now.Date)
                    {
                        EarnPoint *= customerRank.ProfitExtraBirthDay / 100;
                    }
                    walletCus.Balance += EarnPoint;
                    var TypeBooking = orderService.TypeBooking;
                    var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                              SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_PACKAGE : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_FINISH_BOOKING_FOOD : 0;
                    var content = TypeBooking == SystemParam.SHIP_DRIVER ? "Hoàn thành đơn đặt xe" : TypeBooking == SystemParam.SHIP_PACKAGE ?
              "Hoàn thành đơn giao hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "Hoàn thành đơn giao đồ ăn" : "";
                    var transactionCus = new MembersTransactionHistory
                    {
                        MemberID = member.ID,
                        OrderServiceID = orderService.ID,
                        Title = "Cộng điểm tích lũy khách hàng",
                        Content = content,
                        Point = EarnPoint,
                        Type = Constant.TRANSACTION_ADD_POINT,
                        TransactionType = Constant.TYPE_TRANSACTION_EARN_POINT,
                        TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        BeforeBalance = walletCus.Balance - EarnPoint,
                        AfterBalance = walletCus.Balance,
                        Status = Constant.STATUS_TRANSACTION_SUCCESS,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        Icon = "",
                        WalletID = walletCus.ID,
                        IsExtra = false,
                    };
                    cnn.MembersTransactionHistories.Add(transactionCus);
                    var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Chuyến đi " : TypeBooking == SystemParam.SHIP_PACKAGE ?
                         "Đơn giao hàng " : TypeBooking == SystemParam.SHIP_FOOD ? "Đơn giao đồ ăn " : "";
                    title += orderService.Code + " đã được hoàn thành";
                    if (EarnPoint > 0)
                    {
                        var earnPointTitle = "Bạn đã được cộng " + Util.ConvertCurrency(EarnPoint) + " điểm vào tài khoản";
                        Notification notify = new Notification
                        {
                            Title = earnPointTitle,
                            Content = earnPointTitle,
                            Type = SystemParam.NOTI_TYPE_NAVIGATE_EARN_POINT_CUSTOMER,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            IsRead = false,
                            MemberID = member.ID,
                            Code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                            OrderServiceID = OrderServiceID
                        };
                        cnn.Notifications.Add(notify);
                        cnn.SaveChanges();
                    }
                    pushNotifyBusiness.PushNotifyapp(title, type, OrderServiceID, member.ID, 1);
                    // Thưởng điểm khi tăng hạng
                    customer.RankingPoint += orderService.TotalPrice;
                    var pointHistory = new HistoryRankingPoint
                    {
                        CreatedDate = DateTime.Now,
                        CustomerID = orderService.CustomerID,
                        IsActive = SystemParam.ACTIVE,
                        Point = orderService.TotalPrice,
                        TotalPoint = customer.RankingPoint,
                        Type = orderService.TypeBooking,
                    };
                    cnn.HistoryRankingPoints.Add(pointHistory);
                    cnn.SaveChanges();
                    var rankLevel = customer.CustomerRank.Level;
                    var nextRank = cnn.CustomerRanks.Where(x => x.Level > rankLevel).ToList();
                    var typeUpRank = SystemParam.NOTI_TYPE_NAVIGATE_UP_RANK;
                    var typeEarnPoint = SystemParam.NOTI_TYPE_NAVIGATE_EARN_POINT_CUSTOMER;
                    foreach (var item in nextRank)
                    {
                        if (customer.RankingPoint >= item.MinPoint)
                        {
                            var titleLevelUp = "Bạn đã đạt được hạng " + item.Description;
                            var titleEarnPoint = Util.ConvertCurrency(item.PointBonus) + " điểm thưởng đã được cộng vào tài khoản của bạn";
                            var transactionLevelUp = new MembersTransactionHistory
                            {
                                MemberID = member.ID,
                                OrderServiceID = orderService.ID,
                                Title = "Khách hàng đạt hạng " + item.Description,
                                Content = "Thưởng điểm lên hạng " + item.Description,
                                Point = item.PointBonus,
                                Type = Constant.TRANSACTION_ADD_POINT,
                                TransactionType = Constant.TYPE_TRANSACTION_EARN_POINT_LEVEL_UP,
                                TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                BeforeBalance = walletCus.Balance,
                                AfterBalance = walletCus.Balance + item.PointBonus,
                                Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                IsActive = SystemParam.ACTIVE,
                                CreateDate = DateTime.Now,
                                Icon = "",
                                WalletID = walletCus.ID,
                                IsExtra = false,
                            };
                            cnn.MembersTransactionHistories.Add(transactionLevelUp);
                            pushNotifyBusiness.PushNotifyapp(titleLevelUp, typeUpRank, orderService.ID, member.ID, 1);
                            pushNotifyBusiness.PushNotifyapp(titleEarnPoint, typeEarnPoint, orderService.ID, member.ID, 1);
                            walletCus.Balance += item.PointBonus;
                            customer.CustomerRankID = item.ID;
                            cnn.SaveChanges();
                        }

                    }
                    // APP Tài xế
                    var titleDriver = TypeBooking == SystemParam.SHIP_DRIVER ? "Hoàn thành đơn đặt xe " : TypeBooking == SystemParam.SHIP_PACKAGE ?
    "Hoàn thành đơn giao hàng " : TypeBooking == SystemParam.SHIP_FOOD ? "Hoàn thành đơn giao đồ ăn " : "";
                    titleDriver += orderService.Code;
                    if (shipper.IsInternal == SystemParam.SHIPPER_PARTNER)
                    {
                        // Ví cọc
                        var wallet1 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_NO_WITHDRAW && x.MemberID == memberShipper.ID);
                        // Ví thu nhập
                        var wallet2 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_WITHDRAW && x.MemberID == memberShipper.ID);

                        if (wallet1 == null || wallet2 == null)
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.WALLET_NOT_FOUND;
                        }
                        var ShiperEarning = orderService.ShiperCommission.GetValueOrDefault();
                        if (orderService.TypeBooking == SystemParam.SHIP_FOOD)
                        {
                            var orderFoodPrice = cnn.OrderServiceDetails.Where(x => x.OrderServiceID == OrderServiceID).Select(x => x.Price * x.Quantity).Sum();
                            ShiperEarning += orderFoodPrice;
                        }
                        var transaction = new MembersTransactionHistory
                        {
                            MemberID = memberShipper.ID,
                            OrderServiceID = orderService.ID,
                            Title = "Cộng tiền ví thu nhập ",
                            Content = content,
                            Point = ShiperEarning,
                            Type = Constant.TRANSACTION_ADD_POINT,
                            TransactionType = Constant.TYPE_TRANSACTION_TRANSFER_NO_WALLET,
                            TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                            BeforeBalance = wallet2.Balance,
                            AfterBalance = wallet2.Balance + ShiperEarning,
                            Status = Constant.STATUS_TRANSACTION_SUCCESS,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            Icon = "",
                            WalletID = wallet2.ID,
                            IsExtra = false,
                        };
                        cnn.MembersTransactionHistories.Add(transaction);
                        wallet2.Balance += ShiperEarning;

                        if (ShiperEarning > 0)
                        {
                            var titleDriverEarnMoney = Util.ConvertCurrency(ShiperEarning) + "đ đã được cộng vào ví thu nhập của bạn";
                            var typeDriverEarnMoney = SystemParam.NOTI_TYPE_WITH_EARN_MONEY;
                            pushNotifyBusiness.PushNotifyapp(titleDriverEarnMoney, typeDriverEarnMoney, OrderServiceID, memberShipper.ID, 2);
                        }
                        cnn.SaveChanges();
                        pushNotifyBusiness.PushNotifyapp(titleDriver, type, OrderServiceID, memberShipper.ID, 2);
                    }
                    else
                    {
                        pushNotifyBusiness.PushNotifyapp(titleDriver, type, OrderServiceID, memberShipper.ID, 2);
                    }
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return SystemParam.SUCCESS;
                }

            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Cập nhật vị trí tài xế
        public int UpdateShiperLocation(int ShiperID, double lati, double longi)
        {
            try
            {
                var shipper = cnn.Shipers.FirstOrDefault(x => x.ID == ShiperID);
                if (shipper == null)
                {
                    return SystemParam.SHIPPER_NOT_FOUND;
                }
                shipper.Lati = lati;
                shipper.Longi = longi;
                cnn.SaveChanges();
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Đã đón khách , lấy hàng , lấy đồ ăn
        public int PickUpCustomer(int OrderServiceID, int ShiperID)
        {
            try
            {
                using (var dbContextTransaction = cnn.Database.BeginTransaction())
                {
                    var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID && x.Status == SystemParam.ORDER_STATUS_DELIVERY);
                    if (orderService == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_NOT_FOUND;
                    }
                    var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                    var member = cnn.Members.FirstOrDefault(x => x.CustomerID == orderService.CustomerID);
                    if (shiper == null)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.SHIPPER_NOT_FOUND; ;
                    }
                    if (orderService.ShiperID != ShiperID)
                    {
                        dbContextTransaction.Rollback();
                        return SystemParam.ORDER_DRIVER_NO_PERMISSION;
                    }
                    orderService.Status = SystemParam.ORDER_STATUS_PICK_UP;
                    orderService.ConfirmDate = DateTime.Now;
                    var TypeBooking = orderService.TypeBooking;
                    var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                              SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_PACKAGE : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_CONFIRM_FOOD : 0;
                    var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Tài xế đã đón bạn" : TypeBooking == SystemParam.SHIP_PACKAGE ?
             "Tài xế đã lấy hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "Tài xế đã lấy đồ ăn" : "";
                    var content = TypeBooking == SystemParam.SHIP_DRIVER ? "Đặt xe" : TypeBooking == SystemParam.SHIP_PACKAGE ?
                  "Giao hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "Giao đồ ăn" : "";
                    if (shiper.IsInternal == SystemParam.SHIPPER_PARTNER && orderService.TransportType.GetValueOrDefault() >= SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                    {
                        // Ví cọc
                        var wallet1 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_NO_WITHDRAW && x.MemberID == member.ID);
                        // Ví thu nhập
                        var wallet2 = cnn.Wallets.FirstOrDefault(x => x.Type == Constant.WALLET_WITHDRAW && x.MemberID == member.ID);

                        if (wallet1 == null || wallet2 == null)
                        {
                            dbContextTransaction.Rollback();
                            return SystemParam.WALLET_NOT_FOUND;
                        }
                        if (orderService.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY)
                        {
                            var totalPrice = orderService.TotalPrice;

                            if (wallet1.Balance >= totalPrice)
                            {
                                if (totalPrice > 0)
                                {
                                    var transaction = new MembersTransactionHistory
                                    {
                                        MemberID = member.ID,
                                        OrderServiceID = orderService.ID,
                                        Title = "Trừ tiền ví cọc ",
                                        Content = content + " thanh toán tiền mặt ",
                                        Point = totalPrice,
                                        Type = Constant.SUBTRACT_POINT,
                                        TransactionType = Constant.TYPE_TRANSACTION_ACCEPT_ORDER,
                                        TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                                        BeforeBalance = wallet1.Balance,
                                        AfterBalance = wallet1.Balance - totalPrice,
                                        Status = Constant.STATUS_TRANSACTION_SUCCESS,
                                        IsActive = SystemParam.ACTIVE,
                                        CreateDate = DateTime.Now,
                                        Icon = "",
                                        WalletID = wallet1.ID,
                                        IsExtra = true,
                                    };
                                    cnn.MembersTransactionHistories.Add(transaction);
                                    wallet1.Balance -= totalPrice;
                                    var noti = new Notification
                                    {
                                        Title = "Hệ thống đã thu của bạn " + Util.ConvertCurrency(totalPrice) + "đ phí dịch vụ",
                                        Content = "Hệ thống đã thu của bạn " + Util.ConvertCurrency(totalPrice) + "đ phí dịch vụ",
                                        Type = SystemParam.NOTI_TYPE_NAVIGATE_MINUS_MONEY_DRIVER,
                                        IsActive = SystemParam.ACTIVE,
                                        CreateDate = DateTime.Now,
                                        IsRead = false,
                                        MemberID = member.ID,
                                        Code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                                        OrderServiceID = OrderServiceID
                                    };
                                    cnn.Notifications.Add(noti);
                                    cnn.SaveChanges();
                                }
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                                return SystemParam.WALLET_NOT_ENOUGH_MONEY;
                            }
                        }

                    }
                    cnn.SaveChanges();
                    pushNotifyBusiness.PushNotifyapp(title, type, orderService.ID, member.ID, 1);
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    return SystemParam.SUCCESS;
                }


            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }

        }
        // Kiểm tra quyền tài xế 
        public int CheckGetOrderServiceDriver(int OrderServiceID, int ShiperID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return SystemParam.ORDER_NOT_FOUND;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    return SystemParam.SHIPPER_NOT_FOUND;
                }
                if (orderService.ShiperID != ShiperID)
                {
                    return SystemParam.ORDER_DRIVER_NO_PERMISSION;
                }
                return SystemParam.SUCCESS;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Lấy thông tin đơn đặt xe khi tiếp nhận cho tài xế
        public OrderServiceDriver GetOrderServiceDriver(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    return null;
                }
                var model = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join m in cnn.Members on c.ID equals m.CustomerID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new OrderServiceDriver
                             {
                                 ID = os.ID,
                                 AddressFrom = os.Address,
                                 AddressTo = os.FinishAddress,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 ChatIDCustomer = m.KeyChat,
                                 RankID = c.CustomerRankID,
                                 RankName = cr.Description,
                                 CustomerName = c.Name,
                                 LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                 LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                 LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 Phone = c.Phone,
                                 BasePrice = os.BasePrice,
                                 CouponDiscount = os.CouponPoint,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1,
                                 DriverPrice = os.ShiperCommission.HasValue ? os.ShiperCommission.Value : 0,
                                 EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                 EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                 Status = os.Status,
                                 PaymentMethod = os.PaymentType,
                                 TypeBooking = os.TypeBooking
                             }).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao hàng khi tiếp nhận cho tài xế
        public OrderServicePackageDriver GetOrderServicePackageDriver(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    return null;
                }
                var vehicleTypeID = orderService.TransportType.GetValueOrDefault();
                var model = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join m in cnn.Members on c.ID equals m.CustomerID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new OrderServicePackageDriver
                             {
                                 ID = os.ID,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 CustomerName = c.Name,
                                 RankID = cr.ID,
                                 RankName = cr.Description,
                                 ChatIDCustomer = m.KeyChat,
                                 LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                 LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                 LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                 EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                 IsPaymentReceiver = os.IsPaymentReceiver,
                                 ReceiverName = os.RecevieverName,
                                 ReceiverPhone = os.RecevieverPhone,
                                 SenderName = os.SenderName,
                                 SenderPhone = os.SenderPhone,
                                 Phone = c.Phone,
                                 AddressFrom = os.Address,
                                 AddressTo = os.FinishAddress,
                                 AddressToDetail = os.FinishAddressDetail,
                                 PaymentMethod = os.PaymentType,
                                 BasePrice = os.BasePrice,
                                 CouponDiscount = os.CouponPoint,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.ShiperCommission.HasValue ? os.ShiperCommission.Value : 0,
                                 Status = os.Status,
                                 TypeBooking = os.TypeBooking,
                                 CODFee = os.CODFee,
                                 TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                 PackageFee = os.PackageFee,
                                 PackageType = os.PackageType,
                                 Weight = os.Weight,
                                 Note = os.Note,
                                 IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1,
                                 Images = cnn.OrderServiceImages.Where(x => x.OrderServiceID == os.ID).Select(x => fullUrl + x.Path).ToList()
                             }).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao hàng khi tiếp nhận cho tài xế
        public OrderServicePackageVIPDriver GetOrderServicePackageVIPDriver(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    return null;
                }
                var vehicleTypeID = orderService.TransportType.GetValueOrDefault();
                var model = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join m in cnn.Members on c.ID equals m.CustomerID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new OrderServicePackageVIPDriver
                             {
                                 ID = os.ID,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 CustomerName = c.Name,
                                 RankID = cr.ID,
                                 RankName = cr.Description,
                                 ChatIDCustomer = m.KeyChat,
                                 LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                 LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                 LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                 EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                 DistrictID = os.DistrictID,
                                 ProvinceID = os.ProvinceID,
                                 Height = os.Height,
                                 Length = os.Length,
                                 Width = os.Width,
                                 IsPaymentReceiver = os.IsPaymentReceiver,
                                 ReceiverName = os.RecevieverName,
                                 ReceiverPhone = os.RecevieverPhone,
                                 SenderName = os.SenderName,
                                 SenderPhone = os.SenderPhone,
                                 Phone = c.Phone,
                                 AddressFrom = os.Address,
                                 AddressTo = os.FinishAddress,
                                 AddressToDetail = os.FinishAddressDetail,
                                 PaymentMethod = os.PaymentType,
                                 BasePrice = os.BasePrice,
                                 CouponDiscount = os.CouponPoint,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.ShiperCommission.HasValue ? os.ShiperCommission.Value : 0,
                                 Status = os.Status,
                                 TypeBooking = os.TypeBooking,
                                 CODFee = os.CODFee,
                                 TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                 PackageFee = os.PackageFee,
                                 PackageType = os.PackageType,
                                 Weight = os.Weight,
                                 Note = os.Note,
                                 IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1,
                                 Images = cnn.OrderServiceImages.Where(x => x.OrderServiceID == os.ID).Select(x => fullUrl + x.Path).ToList()
                             }).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao đồ ăn khi tiếp nhận cho tài xế
        public OrderServiceFoodDriver GetOrderServiceFoodDriver(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    return null;
                }
                var order = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join m in cnn.Members on c.ID equals m.CustomerID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new
                             {
                                 ID = os.ID,
                                 AddressFrom = os.Address,
                                 AddressTo = os.FinishAddress,
                                 LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                 LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                 LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 ChatIDCustomer = m.KeyChat,
                                 RankID = c.CustomerRankID,
                                 ShopName = os.Shop.Name,
                                 ShopPhone = os.Shop.Phone,
                                 RankName = cr.Description,
                                 CustomerName = c.Name,
                                 PaymentMethod = os.PaymentType,
                                 BasePrice = os.BasePrice,
                                 CouponDiscount = os.CouponPoint,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.ShiperCommission.HasValue ? os.ShiperCommission.Value : 0,
                                 Phone = c.Phone,
                                 IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1,
                                 EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                 EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                 Status = os.Status,
                                 TypeBooking = os.TypeBooking,
                             }).FirstOrDefault();
                var model = new OrderServiceFoodDriver
                {
                    ID = order.ID,
                    CustomerName = order.CustomerName,
                    AddressFrom = order.AddressFrom,
                    AddressTo = order.AddressTo,
                    LatiFrom = order.LatiFrom,
                    LongiFrom = order.LongiFrom,
                    LatiTo = order.LatiTo,
                    LongiTo = order.LongiTo,
                    AvatarUrl = order.AvatarUrl,
                    RankID = order.RankID,
                    RankName = order.RankName,
                    ShopName = order.ShopName,
                    ShopPhone = order.ShopPhone,
                    ChatIDCustomer = order.ChatIDCustomer,
                    PaymentMethod = order.PaymentMethod,
                    BasePrice = order.BasePrice,
                    CouponDiscount = order.CouponDiscount,
                    EstimateTimeShip = order.EstimateTimeShip,
                    EstimateTimeWait = order.EstimateTimeWait,
                    Phone = order.Phone,
                    Point = order.Point,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    TypeBooking = order.TypeBooking,
                    DriverPrice = order.DriverPrice,
                    CartDetails = cartBus.GetOrderServiceDetails(order.ID),
                    IsMotorbike = order.IsMotorbike
                };
                model.FoodPrice = model.CartDetails.Sum(x => x.SumPrice);
                model.TotalQuantity = model.CartDetails.Sum(x => x.Quantity);
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Lấy thông tin đơn đặt xe 
        public BookingDriverOutputModel GetBookingDriverDetail(int OrderServiceID, int ShiperID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID && x.TypeBooking == SystemParam.SHIP_DRIVER);
                if (orderService == null)
                {
                    return null;
                }
                var commission = cnn.Shipers.Where(x => x.ID == ShiperID && x.IsInternal == SystemParam.SHIPPER_PARTNER).Select(x => x.ConfigCommission.MastersBenefit).FirstOrDefault();
                var configCountdown = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_COUNTDOWN).Select(x => x.ValueConstant).FirstOrDefault();
                var countdown = SystemParam.COUNTDOWN;
                if (configCountdown != null && configCountdown != "")
                {
                    try
                    {
                        countdown = Int32.Parse(configCountdown);
                    }
                    catch
                    {
                        countdown = SystemParam.COUNTDOWN;
                    }
                }
                var model = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new BookingDriverOutputModel
                             {
                                 ID = os.ID,
                                 PaymentMethod = os.PaymentType,
                                 CustomerName = c.Name,
                                 Countdown = countdown,
                                 StartAddress = os.Address,
                                 FinishAddress = os.FinishAddress,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 StartLati = os.Lati.HasValue ? os.Lati.Value : 0,
                                 StartLongi = os.Longi.HasValue ? os.Longi.Value : 0,
                                 FinishLati = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 FinishLongi = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 RankID = c.CustomerRankID,
                                 RankName = cr.Description,
                                 BasePrice = os.BasePrice,
                                 CouponDiscount = os.CouponPoint,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.BasePrice * commission / 100,
                                 Status = os.Status,
                                 TimeBooking = os.BookingDate,
                                 TypeBooking = os.TypeBooking,
                                 IsPushFirst = os.IsPushFirst,
                                 TimePushFirst = os.PushFirstDate
                             }).FirstOrDefault();
                if (model != null)
                {
                    if (model.IsPushFirst == SystemParam.ORDER_NOT_PUSH_FIRST)
                    {
                        model.TimeExpired = model.TimeBooking.AddSeconds(countdown);
                    }
                    else
                    {
                        if (model.TimePushFirst.HasValue)
                        {
                            model.TimeExpired = model.TimePushFirst.Value.AddSeconds(countdown);
                        }
                        else
                        {
                            model.TimeExpired = DateTime.Now.AddSeconds(countdown);
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao hàng
        public BookingPackageOutputModel GetBookingPackageDetail(int OrderServiceID, int ShiperID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID && x.TypeBooking == SystemParam.SHIP_PACKAGE);
                if (orderService == null)
                {
                    return null;
                }
                var commission = cnn.Shipers.Where(x => x.ID == ShiperID && x.IsInternal == SystemParam.SHIPPER_PARTNER).Select(x => x.ConfigCommission.MastersBenefit).FirstOrDefault();
                var configCountdown = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_COUNTDOWN).Select(x => x.ValueConstant).FirstOrDefault();
                var countdown = SystemParam.COUNTDOWN;
                if (configCountdown != null && configCountdown != "")
                {
                    try
                    {
                        countdown = Int32.Parse(configCountdown);
                    }
                    catch
                    {
                        countdown = SystemParam.COUNTDOWN;
                    }
                }
                var model = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new BookingPackageOutputModel
                             {
                                 ID = os.ID,
                                 PaymentMethod = os.PaymentType,
                                 CustomerName = c.Name,
                                 Countdown = countdown,
                                 StartAddress = os.Address,
                                 FinishAddress = os.FinishAddress,
                                 StartLati = os.Lati.HasValue ? os.Lati.Value : 0,
                                 StartLongi = os.Longi.HasValue ? os.Longi.Value : 0,
                                 FinishLati = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 FinishLongi = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 RankID = c.CustomerRankID,
                                 RankName = cr.Description,
                                 BasePrice = os.BasePrice,
                                 CODFee = os.CODFee,
                                 SenderName = os.SenderName,
                                 SenderPhone = os.SenderPhone,
                                 ReceiverName = os.RecevieverName,
                                 ReceiverPhone = os.RecevieverPhone,
                                 IsPaymentReceiver = os.IsPaymentReceiver,
                                 PackageType = os.PackageType,
                                 Weight = os.Weight,
                                 CouponDiscount = os.CouponPoint,
                                 TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                 Note = os.Note,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.BasePrice * commission / 100,
                                 Status = os.Status,
                                 TimeBooking = os.BookingDate,
                                 TypeBooking = os.TypeBooking,
                                 IsPushFirst = os.IsPushFirst,
                                 TimePushFirst = os.PushFirstDate
                             }).FirstOrDefault();
                if (model != null)
                {
                    if (model.IsPushFirst == SystemParam.ORDER_NOT_PUSH_FIRST)
                    {
                        model.TimeExpired = model.TimeBooking.AddSeconds(countdown);
                    }
                    else
                    {
                        if (model.TimePushFirst.HasValue)
                        {
                            model.TimeExpired = model.TimePushFirst.Value.AddSeconds(countdown);
                        }
                        else
                        {
                            model.TimeExpired = DateTime.Now.AddSeconds(countdown);
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao hàng VIP
        public BookingPackageVIPOutputModel GetBookingPackageVIPDetail(int OrderServiceID, int ShiperID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID && x.TypeBooking == SystemParam.SHIP_PACKAGE);
                if (orderService == null)
                {
                    return null;
                }
                var commission = cnn.Shipers.Where(x => x.ID == ShiperID && x.IsInternal == SystemParam.SHIPPER_PARTNER).Select(x => x.ConfigCommission.MastersBenefit).FirstOrDefault();
                var configCountdown = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_COUNTDOWN).Select(x => x.ValueConstant).FirstOrDefault();
                var countdown = SystemParam.COUNTDOWN;
                if (configCountdown != null && configCountdown != "")
                {
                    try
                    {
                        countdown = Int32.Parse(configCountdown);
                    }
                    catch
                    {
                        countdown = SystemParam.COUNTDOWN;
                    }
                }
                var model = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new BookingPackageVIPOutputModel
                             {
                                 ID = os.ID,
                                 PaymentMethod = os.PaymentType,
                                 CustomerName = c.Name,
                                 Countdown = countdown,
                                 StartAddress = os.Address,
                                 FinishAddress = os.FinishAddress,
                                 StartLati = os.Lati.HasValue ? os.Lati.Value : 0,
                                 StartLongi = os.Longi.HasValue ? os.Longi.Value : 0,
                                 FinishLati = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 FinishLongi = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 RankID = c.CustomerRankID,
                                 RankName = cr.Description,
                                 BasePrice = os.BasePrice,
                                 CODFee = os.CODFee,
                                 DistrictID = os.DistrictID,
                                 ProvinceID = os.ProvinceID,
                                 Height = os.Height,
                                 Length = os.Length,
                                 Width = os.Width,
                                 SenderName = os.SenderName,
                                 SenderPhone = os.SenderPhone,
                                 ReceiverName = os.RecevieverName,
                                 ReceiverPhone = os.RecevieverPhone,
                                 IsPaymentReceiver = os.IsPaymentReceiver,
                                 PackageType = os.PackageType,
                                 Weight = os.Weight,
                                 CouponDiscount = os.CouponPoint,
                                 TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                 Note = os.Note,
                                 Point = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.BasePrice * commission / 100,
                                 Status = os.Status,
                                 TimeBooking = os.BookingDate,
                                 TypeBooking = os.TypeBooking,
                                 IsPushFirst = os.IsPushFirst,
                                 TimePushFirst = os.PushFirstDate
                             }).FirstOrDefault();
                if (model != null)
                {
                    if (model.IsPushFirst == SystemParam.ORDER_NOT_PUSH_FIRST)
                    {
                        model.TimeExpired = model.TimeBooking.AddSeconds(countdown);
                    }
                    else
                    {
                        if (model.TimePushFirst.HasValue)
                        {
                            model.TimeExpired = model.TimePushFirst.Value.AddSeconds(countdown);
                        }
                        else
                        {
                            model.TimeExpired = DateTime.Now.AddSeconds(countdown);
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao đồ ăn
        public BookingFoodOutputModel GetBookingFoodDetail(int OrderServiceID, int ShiperID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID && x.TypeBooking == SystemParam.SHIP_FOOD);
                if (orderService == null)
                {
                    return null;
                }
                var configCountdown = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_COUNTDOWN).Select(x => x.ValueConstant).FirstOrDefault();
                var countdown = SystemParam.COUNTDOWN;
                if (configCountdown != null && configCountdown != "")
                {
                    try
                    {
                        countdown = Int32.Parse(configCountdown);
                    }
                    catch
                    {
                        countdown = SystemParam.COUNTDOWN;
                    }
                }
                var commission = cnn.Shipers.Where(x => x.ID == ShiperID && x.IsInternal == SystemParam.SHIPPER_PARTNER).Select(x => x.ConfigCommission.MastersBenefit).FirstOrDefault();
                var order = (from os in cnn.OrderServices
                             join c in cnn.Customers on os.CustomerID equals c.ID
                             join cr in cnn.CustomerRanks on c.CustomerRankID equals cr.ID
                             where os.ID == OrderServiceID
                             select new
                             {
                                 ID = os.ID,
                                 PaymentMethod = os.PaymentType,
                                 CustomerName = c.Name,
                                 Countdown = countdown,
                                 StartAddress = os.Address,
                                 FinishAddress = os.FinishAddress,
                                 StartLati = os.Lati.HasValue ? os.Lati.Value : 0,
                                 StartLongi = os.Longi.HasValue ? os.Longi.Value : 0,
                                 FinishLati = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                 FinishLongi = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                 AvatarUrl = fullUrl + c.AvatarUrl,
                                 RankID = c.CustomerRankID,
                                 RankName = cr.Description,
                                 BasePrice = os.BasePrice,
                                 PackageType = os.PackageType,
                                 CouponPrice = os.CouponPoint,
                                 PointPrice = os.UsePoint,
                                 TotalPrice = os.TotalPrice,
                                 DriverPrice = os.BasePrice * commission / 100,
                                 Status = os.Status,
                                 TimeBooking = os.BookingDate,
                                 TypeBooking = os.TypeBooking,
                                 IsPushFirst = os.IsPushFirst,
                                 TimePushFirst = os.PushFirstDate,
                                 ShopName = os.Shop.Name,
                                 ShopPhone = os.Shop.Phone
                             }).FirstOrDefault();

                var model = new BookingFoodOutputModel
                {
                    ID = order.ID,
                    CustomerName = order.CustomerName,
                    Countdown = order.Countdown,
                    TimeExpired = DateTime.Now.AddSeconds(SystemParam.COUNTDOWN),
                    StartAddress = order.StartAddress,
                    FinishAddress = order.FinishAddress,
                    StartLongi = order.StartLongi,
                    StartLati = order.StartLati,
                    FinishLati = order.FinishLati,
                    FinishLongi = order.FinishLongi,
                    AvatarUrl = order.AvatarUrl,
                    RankID = order.RankID,
                    RankName = order.RankName,
                    PaymentMethod = order.PaymentMethod,
                    BasePrice = order.BasePrice,
                    CouponDiscount = order.CouponPrice,
                    DriverPrice = order.DriverPrice,
                    Point = order.PointPrice,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status,
                    TypeBooking = order.TypeBooking,
                    IsPushFirst = order.IsPushFirst,
                    TimePushFirst = order.TimePushFirst,
                    TimeBooking = order.TimeBooking,
                    CartDetails = cartBus.GetOrderServiceDetails(order.ID),
                    ShopName = order.ShopName,
                    ShopPhone = order.ShopPhone
                };
                model.FoodPrice = model.CartDetails.Sum(x => x.SumPrice);
                model.TotalQuantity = model.CartDetails.Sum(x => x.Quantity);
                if (model != null)
                {
                    if (model.IsPushFirst == SystemParam.ORDER_NOT_PUSH_FIRST)
                    {
                        model.TimeExpired = model.TimeBooking.AddSeconds(countdown);
                    }
                    else
                    {
                        if (model.TimePushFirst.HasValue)
                        {
                            model.TimeExpired = model.TimePushFirst.Value.AddSeconds(countdown);
                        }
                        else
                        {
                            model.TimeExpired = DateTime.Now.AddSeconds(countdown);
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // Lấy thông tin đơn đặt xe khi tiếp nhận cho khách hàng
        public OrderServiceCustomer GetOrderServiceCustomer(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    var model = (from os in cnn.OrderServices
                                 join vt in cnn.VehicleTypes on os.CarTypeID equals vt.ID
                                 where os.ID == OrderServiceID
                                 select new OrderServiceCustomer
                                 {
                                     ID = os.ID,
                                     TypeBooking = os.TypeBooking,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     EstimateDistance = os.Distance,
                                     EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                     EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     BasePrice = os.BasePrice,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     TotalPrice = os.TotalPrice,
                                     Transport = vt.Name,
                                     Status = os.Status,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    return model;
                }
                else
                {
                    var model = (from os in cnn.OrderServices
                                 join s in cnn.Shipers on os.ShiperID equals s.ID
                                 join m in cnn.Members on s.ID equals m.ShiperID
                                 join cs in cnn.CarShipers on s.ID equals cs.ShiperID
                                 join vt in cnn.VehicleTypes on os.CarTypeID equals vt.ID
                                 where os.ID == OrderServiceID && cs.IsActive == SystemParam.ACTIVE && cs.VehicleTypeID == os.CarTypeID
                                 select new OrderServiceCustomer
                                 {
                                     ID = os.ID,
                                     TypeBooking = os.TypeBooking,
                                     AvatarUrl = fullUrl + s.AvartarUrl,
                                     DriverName = s.Name,
                                     Phone = s.Phone,
                                     Rating = s.Rating,
                                     CarBrand = cs.CarBrand,
                                     CarModel = cs.CarModel,
                                     LicensePlates = cs.LicensePlates,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     EstimateDistance = os.Distance,
                                     EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                     EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     TotalPrice = os.TotalPrice,
                                     Transport = vt.Name,
                                     Status = os.Status,
                                     DriverID = s.ID,
                                     ChatIDDriver = m.KeyChat,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    return model;
                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao hàng khi tiếp nhận cho khách hàng
        public OrderServicePackageCustomer GetOrderServicePackageCustomer(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    var model = (from os in cnn.OrderServices
                                 where os.ID == OrderServiceID
                                 select new OrderServicePackageCustomer
                                 {
                                     ID = os.ID,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     AddressToDetail = os.FinishAddressDetail,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     Note = os.Note,
                                     TotalPrice = os.TotalPrice,
                                     Status = os.Status,
                                     TypeBooking = os.TypeBooking,
                                     CODFee = os.CODFee,
                                     PackageType = os.PackageType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                     Weight = os.Weight,
                                     IsPaymentReceiver = os.IsPaymentReceiver,
                                     SenderName = os.SenderName,
                                     SenderPhone = os.SenderPhone,
                                     ReceiverName = os.RecevieverName,
                                     ReceiverPhone = os.RecevieverPhone,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    return model;
                }
                else
                {
                    var model = (from os in cnn.OrderServices
                                 join s in cnn.Shipers on os.ShiperID equals s.ID
                                 join m in cnn.Members on s.ID equals m.ShiperID
                                 join cs in cnn.CarShipers on s.ID equals cs.ShiperID
                                 where os.ID == OrderServiceID && cs.IsActive == SystemParam.ACTIVE && cs.VehicleTypeID == os.CarTypeID
                                 select new OrderServicePackageCustomer
                                 {
                                     ID = os.ID,
                                     AvatarUrl = fullUrl + s.AvartarUrl,
                                     DriverID = s.ID,
                                     ChatIDDriver = m.KeyChat,
                                     DriverName = s.Name,
                                     Phone = s.Phone,
                                     Rating = s.Rating,
                                     CarBrand = cs.CarBrand,
                                     CarModel = cs.CarModel,
                                     LicensePlates = cs.LicensePlates,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     AddressToDetail = os.FinishAddressDetail,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                     EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     TotalPrice = os.TotalPrice,
                                     Status = os.Status,
                                     TypeBooking = os.TypeBooking,
                                     CODFee = os.CODFee,
                                     PackageType = os.PackageType,
                                     TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                     Weight = os.Weight,
                                     IsPaymentReceiver = os.IsPaymentReceiver,
                                     SenderName = os.SenderName,
                                     SenderPhone = os.SenderPhone,
                                     ReceiverName = os.RecevieverName,
                                     ReceiverPhone = os.RecevieverPhone,
                                     Note = os.Note,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    return model;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao hàng khi tiếp nhận cho khách hàng
        public OrderServicePackageVIPCustomer GetOrderServicePackageVIPCustomer(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    var model = (from os in cnn.OrderServices
                                 where os.ID == OrderServiceID
                                 select new OrderServicePackageVIPCustomer
                                 {
                                     ID = os.ID,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     AddressToDetail = os.FinishAddressDetail,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     Note = os.Note,
                                     TotalPrice = os.TotalPrice,
                                     Status = os.Status,
                                     TypeBooking = os.TypeBooking,
                                     Width = os.Width,
                                     Length = os.Length,
                                     Height = os.Height,
                                     DistrictID = os.DistrictID,
                                     ProvinceID = os.ProvinceID,
                                     CODFee = os.CODFee,
                                     PackageType = os.PackageType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                     Weight = os.Weight,
                                     IsPaymentReceiver = os.IsPaymentReceiver,
                                     SenderName = os.SenderName,
                                     SenderPhone = os.SenderPhone,
                                     ReceiverName = os.RecevieverName,
                                     ReceiverPhone = os.RecevieverPhone,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    return model;
                }
                else
                {
                    var model = (from os in cnn.OrderServices
                                 join s in cnn.Shipers on os.ShiperID equals s.ID
                                 join m in cnn.Members on s.ID equals m.ShiperID
                                 join cs in cnn.CarShipers on s.ID equals cs.ShiperID
                                 where os.ID == OrderServiceID && cs.IsActive == SystemParam.ACTIVE && cs.VehicleTypeID == os.CarTypeID
                                 select new OrderServicePackageVIPCustomer
                                 {
                                     ID = os.ID,
                                     AvatarUrl = fullUrl + s.AvartarUrl,
                                     DriverID = s.ID,
                                     ChatIDDriver = m.KeyChat,
                                     DriverName = s.Name,
                                     Phone = s.Phone,
                                     Rating = s.Rating,
                                     CarBrand = cs.CarBrand,
                                     CarModel = cs.CarModel,
                                     LicensePlates = cs.LicensePlates,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     AddressToDetail = os.FinishAddressDetail,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                     EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     TotalPrice = os.TotalPrice,
                                     Width = os.Width,
                                     Length = os.Length,
                                     Height = os.Height,
                                     DistrictID = os.DistrictID,
                                     ProvinceID = os.ProvinceID,
                                     Status = os.Status,
                                     TypeBooking = os.TypeBooking,
                                     CODFee = os.CODFee,
                                     PackageType = os.PackageType,
                                     TransportPackageType = os.TransportType.HasValue ? os.TransportType.Value : 0,
                                     Weight = os.Weight,
                                     IsPaymentReceiver = os.IsPaymentReceiver,
                                     SenderName = os.SenderName,
                                     SenderPhone = os.SenderPhone,
                                     ReceiverName = os.RecevieverName,
                                     ReceiverPhone = os.RecevieverPhone,
                                     Note = os.Note,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    return model;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // Lấy thông tin đơn giao đồ ăn khi tiếp nhận cho khách hàng
        public OrderServiceFoodCustomer GetOrderServiceFoodCustomer(int OrderServiceID)
        {
            try
            {
                var orderService = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (orderService == null)
                {
                    return null;
                }
                var shiper = cnn.Shipers.FirstOrDefault(x => x.ID == orderService.ShiperID);
                if (shiper == null)
                {
                    var order = (from os in cnn.OrderServices
                                 where os.ID == OrderServiceID
                                 select new
                                 {
                                     OrderServiceID = os.ID,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     TotalPrice = os.TotalPrice,
                                     Status = os.Status,
                                     TypeBooking = os.TypeBooking,
                                     ShopPhone = os.Shop.Phone,
                                     ShopName = os.Shop.Name,
                                     ShopID = os.ShopID.HasValue ? os.ShopID.Value : 0,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    var model = new OrderServiceFoodCustomer
                    {
                        ID = order.OrderServiceID,
                        AddressFrom = order.AddressFrom,
                        AddressTo = order.AddressTo,
                        LatiFrom = order.LatiFrom,
                        LongiFrom = order.LongiFrom,
                        LatiTo = order.LatiTo,
                        LongiTo = order.LongiTo,
                        PaymentMethod = order.PaymentMethod,
                        IsPayment = order.IsPayment,
                        BasePrice = order.BasePrice,
                        CouponDiscount = order.CouponDiscount,
                        Point = order.Point,
                        ShopName = order.ShopName,
                        ShopPhone = order.ShopPhone,
                        TotalPrice = order.TotalPrice,
                        Status = order.Status,
                        TypeBooking = order.TypeBooking,
                        CartDetails = cartBus.GetOrderServiceDetails(order.OrderServiceID),
                        IsMotorbike = order.IsMotorbike
                    };
                    model.FoodPrice = model.CartDetails.Sum(x => x.SumPrice);
                    model.TotalQuantity = model.CartDetails.Sum(x => x.Quantity);
                    return model;
                }
                else
                {
                    var order = (from os in cnn.OrderServices
                                 join s in cnn.Shipers on os.ShiperID equals s.ID
                                 join m in cnn.Members on s.ID equals m.ShiperID
                                 join cs in cnn.CarShipers on s.ID equals cs.ShiperID
                                 join vt in cnn.VehicleTypes on cs.VehicleTypeID equals vt.ID
                                 where os.ID == OrderServiceID && cs.IsActive == SystemParam.ACTIVE && cs.VehicleTypeID == os.CarTypeID
                                 select new
                                 {
                                     OrderServiceID = os.ID,
                                     AvatarUrl = fullUrl + s.AvartarUrl,
                                     DriverID = s.ID,
                                     ChatIDDriver = m.KeyChat,
                                     DriverName = s.Name,
                                     Phone = s.Phone,
                                     Rating = s.Rating,
                                     CarBrand = cs.CarBrand,
                                     CarModel = cs.CarModel,
                                     LicensePlates = cs.LicensePlates,
                                     AddressFrom = os.Address,
                                     AddressTo = os.FinishAddress,
                                     ShopPhone = os.Shop.Phone,
                                     ShopName = os.Shop.Name,
                                     LatiFrom = os.Lati.HasValue ? os.Lati.Value : 0,
                                     LongiFrom = os.Longi.HasValue ? os.Longi.Value : 0,
                                     LatiTo = os.FinishLati.HasValue ? os.FinishLati.Value : 0,
                                     LongiTo = os.FinishLongi.HasValue ? os.FinishLongi.Value : 0,
                                     EstimateTimeWait = os.TimeWait.HasValue ? os.TimeWait.Value : 0,
                                     EstimateTimeShip = os.TimeShip.HasValue ? os.TimeShip.Value : 0,
                                     PaymentMethod = os.PaymentType,
                                     IsPayment = os.StatusPayment.HasValue ? os.StatusPayment.Value : 0,
                                     BasePrice = os.BasePrice,
                                     CouponDiscount = os.CouponPoint,
                                     Point = os.UsePoint,
                                     TotalPrice = os.TotalPrice,
                                     Status = os.Status,
                                     TypeBooking = os.TypeBooking,
                                     ShopID = os.ShopID.HasValue ? os.ShopID.Value : 0,
                                     IsMotorbike = os.CarTypeID.HasValue ? os.VehicleType.IsMotorbike : 1
                                 }).FirstOrDefault();
                    var model = new OrderServiceFoodCustomer
                    {
                        ID = order.OrderServiceID,
                        AvatarUrl = order.AvatarUrl,
                        DriverID = order.DriverID,
                        ChatIDDriver = order.ChatIDDriver,
                        DriverName = order.DriverName,
                        Phone = order.Phone,
                        Rating = order.Rating,
                        CarBrand = order.CarBrand,
                        CarModel = order.CarModel,
                        LicensePlates = order.LicensePlates,
                        AddressFrom = order.AddressFrom,
                        AddressTo = order.AddressTo,
                        LatiFrom = order.LatiFrom,
                        LongiFrom = order.LongiFrom,
                        ShopName = order.ShopName,
                        ShopPhone = order.ShopPhone,
                        LatiTo = order.LatiTo,
                        LongiTo = order.LongiTo,
                        EstimateTimeWait = order.EstimateTimeWait,
                        EstimateTimeShip = order.EstimateTimeShip,
                        PaymentMethod = order.PaymentMethod,
                        IsPayment = order.IsPayment,
                        BasePrice = order.BasePrice,
                        CouponDiscount = order.CouponDiscount,
                        Point = order.Point,
                        TotalPrice = order.TotalPrice,
                        Status = order.Status,
                        TypeBooking = order.TypeBooking,
                        CartDetails = cartBus.GetOrderServiceDetails(order.OrderServiceID),
                        IsMotorbike = order.IsMotorbike
                    };
                    model.FoodPrice = model.CartDetails.Sum(x => x.SumPrice);
                    model.TotalQuantity = model.CartDetails.Sum(x => x.Quantity);
                    return model;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Gen Code của đơn đặt xe
        public string GenerateCodeOrderService(int Type)
        {
            var code = "";
            if (Type == SystemParam.SHIP_DRIVER)
            {
                code += "DX";
            }
            else if (Type == SystemParam.SHIP_PACKAGE)
            {
                code += "GH";
            }
            else if (Type == SystemParam.SHIP_FOOD)
            {
                code += "GDA";
            }
            code += DateTime.Now.Millisecond.ToString();
            code += DateTime.Now.Month.ToString();
            code += DateTime.Now.Day.ToString();
            var randomStr = new string(Enumerable.Repeat(nums, length).Select(s => s[random.Next(s.Length)]).ToArray());
            code += randomStr;
            return code;
        }
        public int CheckApplyCoupon(string CouponCode, int CustomerID)
        {
            try
            {

                var coupon = cnn.Coupons.FirstOrDefault(x => x.Code == CouponCode);
                if (coupon == null)
                {
                    return SystemParam.COUPON_NOT_FOUND;
                }
                if (CheckCoupon(coupon.ID, CustomerID, coupon.Type, SystemParam.COUPON_NOT_USED))
                {
                    return coupon.ID;
                }
                else
                {
                    return SystemParam.COUPON_NOT_VALID;
                }

            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Lấy chi tiết mã giảm giá
        public CouponOutputModel GetCouponDetail(int ID)
        {
            var model = cnn.Coupons.Where(x => x.ID == ID).Select(x => new CouponOutputModel
            {
                ID = x.ID,
                Code = x.Code,
                Name = x.Name,
                Content = x.Content,
                StartDate = x.StartDate,
                ExprireDate = x.ExpriceDate,
                Discount = x.Discount,
                Percent = x.Percent,
                TypeCoupon = x.TypeCoupon,
                RankId = x.RankID.HasValue ? x.RankID.Value : 0,
                RankName = x.CustomerRank.Description,
                ServiceName = x.Type == SystemParam.COUPON_TYPE_DRIVER ? SystemParam.COUPON_TYPE_DRIVER_STR : x.Type == SystemParam.COUPON_TYPE_PACKAGE
                                ? SystemParam.COUPON_TYPE_PACKAGE_STR : x.Type == SystemParam.COUPON_TYPE_FOOD ? SystemParam.COUPON_TYPE_FOOD_STR : ""
            }).FirstOrDefault();
            return model;
        }
        public int CreateTransactionVnPay(int OrderServiceID)
        {
            try
            {
                var order = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
                if (order == null)
                {
                    return SystemParam.ORDER_NOT_FOUND;
                }
                var member = cnn.Members.FirstOrDefault(x => x.CustomerID == order.CustomerID);
                var content = order.TypeBooking == SystemParam.SHIP_DRIVER ? "dat xe " : order.TypeBooking == SystemParam.SHIP_PACKAGE ?
                      "giao hang " : order.TypeBooking == SystemParam.SHIP_FOOD ? "giao do an " : "";
                content += order.Code;
                var transaction = new MembersTransactionHistory
                {
                    MemberID = member.ID,
                    OrderServiceID = order.ID,
                    CreateDate = DateTime.Now,
                    Title = SystemParam.PAYMENT_TYPE_VN_PAY_STR,
                    Content = "Thanh toan don " + content + " .So tien " + Util.ConvertCurrency(order.TotalPrice) + " VND",
                    IsActive = SystemParam.ACTIVE,
                    Point = order.TotalPrice,
                    Type = Constant.TRANSACTION_SUBTRACT_POINT,
                    TransactionType = Constant.TYPE_TRANSACTION_VNPAY,
                    TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                    Status = Constant.STATUS_TRANSACTION_WAITING,
                    Icon = "",
                    IsExtra = false,
                };
                cnn.MembersTransactionHistories.Add(transaction);
                cnn.SaveChanges();
                return transaction.ID;
            }
            catch (Exception e)
            {
                return SystemParam.ERROR;
            }
        }
        // Tạo đơn tài xế
        public int CreateOrderDriver(int CustomerID, string LatiFrom, string LongiFrom, string AddressFrom, string LatiTo, string LongiTo, string AddressTo,
                     int VehicleID, int CouponID, int Point, int PaymentMethod)
        {
            try
            {
                if (String.IsNullOrEmpty(LatiFrom) || String.IsNullOrEmpty(LongiFrom) || String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo))
                {
                    return SystemParam.LOCATION_EMPTY;
                }
                var cus = cnn.Customers.Where(x => x.ID == CustomerID).FirstOrDefault();
                if (cus == null)
                {
                    return SystemParam.CUSTOMER_NOT_FOUND;
                }
                var origin = LatiFrom + "," + LongiFrom;
                var destination = LatiTo + "," + LongiTo;
                var LatiFromVal = double.Parse(LatiFrom, System.Globalization.CultureInfo.InvariantCulture);
                var LongiFromVal = double.Parse(LongiFrom, System.Globalization.CultureInfo.InvariantCulture);
                var LatiToVal = double.Parse(LatiTo, System.Globalization.CultureInfo.InvariantCulture);
                var LongiToVal = double.Parse(LongiTo, System.Globalization.CultureInfo.InvariantCulture);
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                // Google Map Json
                var map = JsonConvert.DeserializeObject<Map>(json);
                if (map == null || AddressFrom == "" || AddressTo == "")
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                if (place_id == null || place_id_finish == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }

                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var area_start = "";
                if (mapStart.status == "OK")
                {
                    area_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_2") || x.types.Contains("locality")).Select(x => x.long_name).FirstOrDefault();
                }
                if (area_start == "" || area_start == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var AreaId = 0;
                if (area_start.Trim().Contains("Từ Liêm") || area_start.Trim().Contains("Thạch Tân"))
                {
                    AreaId = cnn.Areas.Where(x => AddressFrom.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                else
                {
                    AreaId = cnn.Areas.Where(x => area_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                if (AreaId == 0)
                {
                    return SystemParam.AREA_NOT_FOUND;
                }


                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var timeShip = map.routes[0].legs[0].duration.Value / SystemParam.Minute;
                var basePrice = CalculateShipperFee(distance, 0, SystemParam.SHIP_DRIVER, VehicleID);
                var totalPrice = basePrice;
                if (basePrice <= 0)
                {
                    return SystemParam.ORDER_TRANSPORT_DRIVER_NOT_VALID;
                }
                var couponPoint = 0;
                if (CouponID != 0)
                {
                    if (CheckCoupon(CouponID, CustomerID, SystemParam.COUPON_TYPE_DRIVER, SystemParam.COUPON_NOT_USED))
                    {
                        using (var dbContextTransaction = cnn.Database.BeginTransaction())
                        {
                            var coupon = cnn.Coupons.FirstOrDefault(x => x.ID == CouponID);
                            if (coupon == null)
                            {
                                return SystemParam.COUPON_NOT_VALID;
                            }
                            var CouponCustomer = cnn.CouponCustomers.Where(x => x.CouponID == CouponID).ToList();
                            if (CouponCustomer.Count() == 0)
                            {
                                if (coupon.Remain <= 0)
                                {
                                    return SystemParam.COUPON_NOT_REMAIN;
                                }
                                cnn.Database.ExecuteSqlCommand(@"Update Coupon SET remain = remain - 1  where ID = " + coupon.ID);
                            }


                            if (coupon.TypeCoupon == SystemParam.COUPON_TYPE_COUPON_PERCENT && coupon.Percent.HasValue)
                            {
                                couponPoint = (basePrice * coupon.Percent.Value / 100);
                            }
                            else if (coupon.TypeCoupon == SystemParam.COUPON_TYPE_COUPON_DISCOUNT && coupon.Discount.HasValue)
                            {
                                couponPoint = coupon.Discount.Value;
                            }
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                        }
                    }
                    else
                    {
                        return SystemParam.COUPON_NOT_VALID;
                    }
                }
                if (couponPoint > totalPrice)
                {
                    totalPrice = 0;
                }
                else
                {
                    totalPrice -= couponPoint;
                }
                var walletCus = cnn.Wallets.Where(x => x.Member.CustomerID == cus.ID).FirstOrDefault();
                if (walletCus == null)
                {
                    return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                }
                var usePoint = 0;
                if (Point > walletCus.Balance)
                {
                    return SystemParam.POINT_NOT_VALID;
                }
                else
                {
                    if (Point <= totalPrice)
                    {
                        totalPrice -= Point;
                        usePoint = Point;
                        walletCus.Balance -= Point;

                    }
                    else
                    {
                        usePoint = totalPrice;
                        walletCus.Balance -= totalPrice;
                        totalPrice = 0;
                    }

                }
                if (PaymentMethod == SystemParam.PAYMENT_TYPE_VN_PAY && totalPrice < SystemParam.MIN_PRICE_PAYMENT_VNPAY)
                {
                    return SystemParam.ORDER_VNPAY_MINPRICE_INVALID;
                }
                var NearestShipper = FindNearestDriver(VehicleID, AreaId, LatiFromVal, LongiFromVal, totalPrice, PaymentMethod);
                if (NearestShipper == null)
                {
                    return SystemParam.SHIPPER_NOT_FOUND;
                }
                if (place_id.Length > 30)
                {
                    place_id = place_id.Substring(0, 30);
                }
                if (place_id_finish.Length > 30)
                {
                    place_id_finish = place_id_finish.Substring(0, 30);
                }
                var OrderService = new OrderService
                {
                    Code = GenerateCodeOrderService(SystemParam.SHIP_DRIVER),
                    CustomerID = CustomerID,
                    AreaID = AreaId,
                    Status = SystemParam.ORDER_STATUS_PENDING,
                    IsActive = SystemParam.ACTIVE,
                    TypeBooking = SystemParam.SHIP_DRIVER,
                    TotalPrice = totalPrice,
                    BasePrice = basePrice,
                    CouponPoint = couponPoint,
                    Distance = distance,
                    TimeShip = timeShip,
                    FinishPlaceID = place_id_finish,
                    FinishAddress = AddressTo,
                    FinishLati = LatiToVal,
                    FinishLongi = LongiToVal,
                    IsPaymentReceiver = SystemParam.ORDER_NOT_PAYMENT,
                    UsePoint = usePoint,
                    CouponID = (int?)CouponID != 0 ? (int?)CouponID : null,
                    IsRate = SystemParam.ORDER_NOT_RATE,
                    PaymentType = PaymentMethod,
                    StatusPayment = SystemParam.ORDER_NOT_PAYMENT,
                    CreatedDate = DateTime.Now,
                    BookingDate = DateTime.Now,
                    Longi = LongiFromVal,
                    Lati = LatiFromVal,
                    PlaceID = place_id,
                    Address = AddressFrom,
                    IsPushFirst = SystemParam.ORDER_NOT_PUSH_FIRST,
                    CarTypeID = VehicleID,
                };
                cnn.OrderServices.Add(OrderService);
                cnn.SaveChanges();
                if (usePoint > 0)
                {
                    var transactionCus = new MembersTransactionHistory
                    {
                        MemberID = cnn.Members.Where(x => x.CustomerID == CustomerID).Select(x => x.ID).FirstOrDefault(),
                        OrderServiceID = OrderService.ID,
                        Title = "Dùng điểm tích lũy khách hàng",
                        Content = "Đặt xe",
                        Point = usePoint,
                        Type = Constant.TRANSACTION_SUBTRACT_POINT,
                        TransactionType = Constant.TYPE_TRANSACTION_USE_POINT,
                        TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        BeforeBalance = walletCus.Balance + usePoint,
                        AfterBalance = walletCus.Balance,
                        Status = Constant.STATUS_TRANSACTION_SUCCESS,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        Icon = "",
                        WalletID = walletCus.ID,
                        IsExtra = false,
                    };
                    cnn.MembersTransactionHistories.Add(transactionCus);
                    cnn.SaveChanges();
                }


                return OrderService.ID;
            }
            catch (Exception e)
            {
                return SystemParam.BOOK_DRIVER_FAIL;
            }
        }
        // Tạo đơn giao hàng
        public int CreateOrderPackage(int CustomerID, string LatiFrom, string LongiFrom, string AddressFrom, string LatiTo, string LongiTo
            , string AddressTo, string AddressToDetail, string SenderName, string SenderPhone, string ReceiverName, string ReceiverPhone, int CouponID
            , int Point, int PaymentMethod, int TransportType, int CODFee, string PackageType, double Weight, int PackageFee, string Note
            , int IsPaymentReceiver, List<string> Images)
        {
            try
            {
                if (String.IsNullOrEmpty(LatiFrom) || String.IsNullOrEmpty(LongiFrom) || String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo))
                {
                    return SystemParam.LOCATION_EMPTY;
                }
                var origin = LatiFrom + "," + LongiFrom;
                var destination = LatiTo + "," + LongiTo;
                var LatiFromVal = double.Parse(LatiFrom, System.Globalization.CultureInfo.InvariantCulture);
                var LongiFromVal = double.Parse(LongiFrom, System.Globalization.CultureInfo.InvariantCulture);
                var LatiToVal = double.Parse(LatiTo, System.Globalization.CultureInfo.InvariantCulture);
                var LongiToVal = double.Parse(LongiTo, System.Globalization.CultureInfo.InvariantCulture);
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                // Google Map Json
                var map = JsonConvert.DeserializeObject<Map>(json);
                if (map == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                if (place_id == null || place_id_finish == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }

                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var area_start = "";
                var province_start = "";
                if (mapStart.status == "OK")
                {
                    area_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_2") || x.types.Contains("locality")).Select(x => x.long_name).FirstOrDefault();
                    province_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_start.Contains("Tỉnh Hà Tây") || province_start.Contains("Hanoi"))
                    {
                        province_start = "Hà Nội";
                    }
                }
                // Gọi API lấy chi tiết địa điểm đến
                var reqDetailEndAddress = GetGoogleMapDetailPlaceApi(place_id_finish);
                var jsonEndAddress = requestBus.GetJson(reqDetailEndAddress);
                var mapEnd = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonEndAddress);
                var province_end = "";
                if (mapEnd.status == "OK")
                {
                    province_end = mapEnd.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_end.Contains("Tỉnh Hà Tây") || province_end.Contains("Hanoi"))
                    {
                        province_end = "Hà Nội";
                    }
                }
                var startProvinceID = cnn.Provinces.Where(x => province_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var endProvinceID = cnn.Provinces.Where(x => province_end.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                if (area_start == "" || province_start == "" || province_end == "" || area_start == null || province_start == null || province_end == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var AreaId = 0;
                if (area_start.Trim().Contains("Từ Liêm") || area_start.Trim().Contains("Thạch Tân"))
                {
                    AreaId = cnn.Areas.Where(x => AddressFrom.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                else
                {
                    AreaId = cnn.Areas.Where(x => area_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                if (AreaId == 0)
                {
                    return SystemParam.AREA_NOT_FOUND;
                }
                var isProvince = startProvinceID == endProvinceID ? SystemParam.IN_PROVINCE : SystemParam.OUT_PROVINCE;
                var maxCODFee = SystemParam.MAX_COD_FEE;
                var configCODFee = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_MAXCODFEE).Select(x => x.ValueConstant).FirstOrDefault();
                if (configCODFee != null && configCODFee != "")
                {
                    try
                    {
                        maxCODFee = Int32.Parse(configCODFee);
                    }
                    catch
                    {
                        maxCODFee = SystemParam.MAX_COD_FEE;
                    }
                }
                if (CODFee > maxCODFee)
                {
                    return SystemParam.COD_FEE_NOT_VALID;
                }
                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var timeShip = map.routes[0].legs[0].duration.Value / SystemParam.Minute;
                var basePrice = CalculateShipperFee(distance, Weight, SystemParam.SHIP_PACKAGE, 1, TransportType, isProvince);
                var totalPrice = basePrice;
                if (basePrice <= 0)
                {
                    return SystemParam.ORDER_TRANSPORT_PACKAGE_NOT_VALID;
                }
                var couponPoint = 0;
                if (CouponID != 0)
                {
                    if (CheckCoupon(CouponID, CustomerID, SystemParam.COUPON_TYPE_PACKAGE, SystemParam.COUPON_NOT_USED))
                    {
                        using (var dbContextTransaction = cnn.Database.BeginTransaction())
                        {
                            var coupon = cnn.Coupons.FirstOrDefault(x => x.ID == CouponID);
                            if (coupon == null)
                            {
                                return SystemParam.COUPON_NOT_VALID;
                            }
                            var CouponCustomer = cnn.CouponCustomers.Where(x => x.CouponID == CouponID).ToList();
                            if (CouponCustomer.Count() == 0)
                            {
                                if (coupon.Remain <= 0)
                                {
                                    return SystemParam.COUPON_NOT_REMAIN;
                                }
                                cnn.Database.ExecuteSqlCommand(@"Update Coupon SET remain = remain - 1  where ID = " + coupon.ID);
                            }
                            if (coupon.TypeCoupon == SystemParam.COUPON_TYPE_COUPON_PERCENT && coupon.Percent.HasValue)
                            {
                                couponPoint = (basePrice * coupon.Percent.Value / 100);
                            }
                            else if (coupon.TypeCoupon == SystemParam.COUPON_TYPE_COUPON_DISCOUNT && coupon.Discount.HasValue)
                            {
                                couponPoint = coupon.Discount.Value;
                            }
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                        }
                    }
                    else
                    {
                        return SystemParam.COUPON_NOT_VALID;
                    }
                }

                if (couponPoint > totalPrice)
                {
                    totalPrice = 0;
                }
                else
                {
                    totalPrice -= couponPoint;
                }
                var walletCus = cnn.Wallets.Where(x => x.Member.CustomerID == CustomerID).FirstOrDefault();
                if (walletCus == null)
                {
                    return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                }
                var usePoint = 0;
                if (Point > walletCus.Balance)
                {
                    return SystemParam.POINT_NOT_VALID;
                }
                else
                {
                    if (Point <= totalPrice)
                    {
                        totalPrice -= Point;
                        usePoint = Point;
                        walletCus.Balance -= Point;
                    }
                    else
                    {
                        usePoint = totalPrice;
                        walletCus.Balance -= totalPrice;
                        totalPrice = 0;
                    }
                }
                var VehicleID = cnn.VehicleTypes.Where(x => x.IsMotorbike == 1 && x.IsActive == SystemParam.ACTIVE).Select(x => x.ID).FirstOrDefault();
                if (PaymentMethod == SystemParam.PAYMENT_TYPE_VN_PAY && totalPrice < SystemParam.MIN_PRICE_PAYMENT_VNPAY)
                {
                    return SystemParam.ORDER_VNPAY_MINPRICE_INVALID;
                }
                if (TransportType == SystemParam.TRANSPORT_TYPE_STANDARD || TransportType == SystemParam.TRANSPORT_TYPE_FAST)
                {
                    var NearestShipper = FindNearestDriver(VehicleID, AreaId, LatiFromVal, LongiFromVal, totalPrice, PaymentMethod, SystemParam.SHIPPER_INTERNAL);
                    if (NearestShipper == null)
                    {
                        return SystemParam.SHIPPER_NOT_FOUND;
                    }
                }
                else
                {
                    var NearestShipper = FindNearestDriver(VehicleID, AreaId, LatiFromVal, LongiFromVal, totalPrice, PaymentMethod);
                    if (NearestShipper == null)
                    {
                        return SystemParam.SHIPPER_NOT_FOUND;
                    }
                }
                if (place_id.Length > 30)
                {
                    place_id = place_id.Substring(0, 30);
                }
                if (place_id_finish.Length > 30)
                {
                    place_id_finish = place_id_finish.Substring(0, 30);
                }
                var OrderService = new OrderService
                {
                    Code = GenerateCodeOrderService(SystemParam.SHIP_PACKAGE),
                    CustomerID = CustomerID,
                    AreaID = AreaId,
                    Status = SystemParam.ORDER_STATUS_PENDING,
                    IsActive = SystemParam.ACTIVE,
                    TypeBooking = SystemParam.SHIP_PACKAGE,
                    TotalPrice = totalPrice,
                    BasePrice = basePrice,
                    CouponPoint = couponPoint,
                    SenderName = SenderName,
                    SenderPhone = SenderPhone,
                    RecevieverName = ReceiverName,
                    RecevieverPhone = ReceiverPhone,
                    Distance = distance,
                    TimeShip = timeShip,
                    Weight = Weight,
                    CODFee = CODFee,
                    PackageFee = PackageFee,
                    PackageType = PackageType,
                    Note = Note,
                    IsPaymentReceiver = IsPaymentReceiver,
                    FinishPlaceID = place_id_finish,
                    FinishAddress = AddressTo,
                    FinishLati = LatiToVal,
                    FinishLongi = LongiToVal,
                    FinishAddressDetail = AddressToDetail,
                    UsePoint = usePoint,
                    CouponID = (int?)CouponID != 0 ? (int?)CouponID : null,
                    IsRate = SystemParam.ORDER_NOT_RATE,
                    PaymentType = PaymentMethod,
                    StatusPayment = SystemParam.ORDER_NOT_PAYMENT,
                    CreatedDate = DateTime.Now,
                    BookingDate = DateTime.Now,
                    Longi = LongiFromVal,
                    Lati = LatiFromVal,
                    PlaceID = place_id,
                    Address = AddressFrom,
                    IsPushFirst = SystemParam.ORDER_NOT_PUSH_FIRST,
                    TransportType = TransportType,
                    CarTypeID = VehicleID,
                };
                cnn.OrderServices.Add(OrderService);
                cnn.SaveChanges();
                foreach (var item in Images)
                {
                    var orderImage = new OrderServiceImage
                    {
                        OrderServiceID = OrderService.ID,
                        Path = item,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                    };
                    cnn.OrderServiceImages.Add(orderImage);
                }
                cnn.SaveChanges();
                if (usePoint > 0)
                {
                    var transactionCus = new MembersTransactionHistory
                    {
                        MemberID = cnn.Members.Where(x => x.CustomerID == CustomerID).Select(x => x.ID).FirstOrDefault(),
                        OrderServiceID = OrderService.ID,
                        Title = "Dùng điểm tích lũy khách hàng",
                        Content = "Giao hàng",
                        Point = usePoint,
                        Type = Constant.TRANSACTION_SUBTRACT_POINT,
                        TransactionType = Constant.TYPE_TRANSACTION_USE_POINT,
                        TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        BeforeBalance = walletCus.Balance + usePoint,
                        AfterBalance = walletCus.Balance,
                        Status = Constant.STATUS_TRANSACTION_SUCCESS,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        Icon = "",
                        WalletID = walletCus.ID,
                        IsExtra = false,
                    };
                    cnn.MembersTransactionHistories.Add(transactionCus);
                    cnn.SaveChanges();
                }
                return OrderService.ID;
            }
            catch (Exception e)
            {
                return SystemParam.BOOK_DRIVER_FAIL;
            }
        }
        // Tạo đơn giao hàng
        public int CreateOrderPackageVIP(int CustomerID, string LatiFrom, string LongiFrom, string AddressFrom, string LatiTo, string LongiTo
            , string AddressTo, string AddressToDetail, string SenderName, string SenderPhone, string ReceiverName, string ReceiverPhone
            , int Point, int PaymentMethod, int TransportType, int CODFee, string PackageType, double Weight, int PackageFee, string Note
            , int IsPaymentReceiver, List<string> Images, int Width, int Length, int Height, int ProvinceID, int DistrictID)
        {
            try
            {
                if (String.IsNullOrEmpty(LatiFrom) || String.IsNullOrEmpty(LongiFrom))
                {
                    return SystemParam.LOCATION_EMPTY;
                }

                var origin = LatiFrom + "," + LongiFrom;
                string destination;
                double LatiToVal, LongiToVal;
                if (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo))
                {
                    destination = SystemParam.LAT_DEFAULT + "," + SystemParam.LONG_DEFAULT;
                    LatiToVal = SystemParam.LAT_DEFAULT;
                    LongiToVal = SystemParam.LONG_DEFAULT;
                }
                else
                {
                    destination = LatiTo + "," + LongiTo;
                    LatiToVal = double.Parse(LatiTo, System.Globalization.CultureInfo.InvariantCulture);
                    LongiToVal = double.Parse(LongiTo, System.Globalization.CultureInfo.InvariantCulture);
                }
                var LatiFromVal = double.Parse(LatiFrom, System.Globalization.CultureInfo.InvariantCulture);
                var LongiFromVal = double.Parse(LongiFrom, System.Globalization.CultureInfo.InvariantCulture);
                
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                // Google Map Json
                var map = JsonConvert.DeserializeObject<Map>(json);
                if (map == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                if (place_id == null || place_id_finish == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }

                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var area_start = "";
                var province_start = "";
                if (mapStart.status == "OK")
                {
                    area_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_2") || x.types.Contains("locality")).Select(x => x.long_name).FirstOrDefault();
                    province_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_start.Contains("Tỉnh Hà Tây") || province_start.Contains("Hanoi"))
                    {
                        province_start = "Hà Nội";
                    }
                }
                // Gọi API lấy chi tiết địa điểm đến
                var reqDetailEndAddress = GetGoogleMapDetailPlaceApi(place_id_finish);
                var jsonEndAddress = requestBus.GetJson(reqDetailEndAddress);
                var mapEnd = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonEndAddress);
                var province_end = "";
                if (mapEnd.status == "OK")
                {
                    province_end = mapEnd.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_end.Contains("Tỉnh Hà Tây") || province_end.Contains("Hanoi"))
                    {
                        province_end = "Hà Nội";
                    }
                }
                var startProvinceID = cnn.Provinces.Where(x => province_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var endProvinceID = cnn.Provinces.Where(x => province_end.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                if (area_start == "" || province_start == "" || province_end == "" || area_start == null || province_start == null || province_end == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var AreaId = 0;
                if (area_start.Trim().Contains("Từ Liêm") || area_start.Trim().Contains("Thạch Tân"))
                {
                    AreaId = cnn.Areas.Where(x => AddressFrom.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                else
                {
                    AreaId = cnn.Areas.Where(x => area_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                if (AreaId == 0)
                {
                    return SystemParam.AREA_NOT_FOUND;
                }
                var isProvince = startProvinceID == endProvinceID ? SystemParam.IN_PROVINCE : SystemParam.OUT_PROVINCE;
                double distance = 0;
                var timeShip = 0;
                var basePrice = 0;
                var couponPoint = 0;
                var totalPrice = 0;
                if (!String.IsNullOrEmpty(LatiTo) && !String.IsNullOrEmpty(LongiTo) && Weight > 0)
                {
                    distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                    timeShip = map.routes[0].legs[0].duration.Value / SystemParam.Minute;
                    basePrice = CalculateShipperFeeVip(distance, Weight, TransportType, isProvince);
                    var cus = cnn.Customers.FirstOrDefault(x => x.ID.Equals(CustomerID));
                    couponPoint = (int)((double)basePrice * cus.VipDiscount.GetValueOrDefault() / 100);
                    totalPrice = basePrice - couponPoint;
                }
                var walletCus = cnn.Wallets.Where(x => x.Member.CustomerID == CustomerID).FirstOrDefault();
                if (walletCus == null)
                {
                    return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                }
                var usePoint = 0;
                if (Point > walletCus.Balance)
                {
                    return SystemParam.POINT_NOT_VALID;
                }
                else
                {
                    if (Point <= totalPrice)
                    {
                        totalPrice -= Point;
                        usePoint = Point;
                        walletCus.Balance -= Point;
                    }
                    else
                    {
                        usePoint = totalPrice;
                        walletCus.Balance -= totalPrice;
                        totalPrice = 0;
                    }
                }
                var VehicleID = cnn.VehicleTypes.Where(x => x.IsMotorbike == 1 && x.IsActive == SystemParam.ACTIVE).Select(x => x.ID).FirstOrDefault();
                if (PaymentMethod == SystemParam.PAYMENT_TYPE_VN_PAY && totalPrice < SystemParam.MIN_PRICE_PAYMENT_VNPAY)
                {
                    return SystemParam.ORDER_VNPAY_MINPRICE_INVALID;
                }
                var NearestShipper = FindNearestDriver(VehicleID, AreaId, LatiFromVal, LongiFromVal, totalPrice, PaymentMethod, SystemParam.SHIPPER_ALL, SystemParam.SHIPPER_VIP);
                if (NearestShipper == null)
                {
                    return SystemParam.SHIPPER_NOT_FOUND;
                }
                var OrderService = new OrderService
                {
                    Code = GenerateCodeOrderService(SystemParam.SHIP_PACKAGE),
                    CustomerID = CustomerID,
                    AreaID = AreaId,
                    Status = SystemParam.ORDER_STATUS_PENDING,
                    IsActive = SystemParam.ACTIVE,
                    TypeBooking = SystemParam.SHIP_PACKAGE,
                    TotalPrice = totalPrice,
                    BasePrice = basePrice,
                    CouponPoint = couponPoint,
                    SenderName = SenderName,
                    SenderPhone = SenderPhone,
                    RecevieverName = ReceiverName,
                    RecevieverPhone = ReceiverPhone,
                    Distance = distance,
                    TimeShip = timeShip,
                    Weight = Weight,
                    CODFee = CODFee,
                    PackageFee = PackageFee,
                    PackageType = PackageType,
                    Note = Note,
                    IsPaymentReceiver = IsPaymentReceiver,
                    FinishPlaceID = (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo)) ? null : place_id_finish,
                    FinishAddress = AddressTo,
                    FinishLati = (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo)) ? 0 : LatiToVal,
                    FinishLongi = (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo)) ? 0 : LongiToVal,
                    FinishAddressDetail = (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo)) ? "" : AddressToDetail,
                    UsePoint = usePoint,
                    DistrictID = (int?)DistrictID != 0 ? (int?)DistrictID : null,
                    ProvinceID = (int?)ProvinceID != 0 ? (int?)ProvinceID : null,
                    Height = (int?)Height != 0 ? (int?)Height : null,
                    Width = (int?)Width != 0 ? (int?)Width : null,
                    Length = (int?)Height != 0 ? (int?)Height : null,
                    IsRate = SystemParam.ORDER_NOT_RATE,
                    PaymentType = PaymentMethod,
                    StatusPayment = SystemParam.ORDER_NOT_PAYMENT,
                    CreatedDate = DateTime.Now,
                    BookingDate = DateTime.Now,
                    Longi = LongiFromVal,
                    Lati = LatiFromVal,
                    PlaceID = place_id,
                    Address = AddressFrom,
                    IsPushFirst = SystemParam.ORDER_NOT_PUSH_FIRST,
                    TransportType = TransportType,
                    CarTypeID = VehicleID,
                };
                cnn.OrderServices.Add(OrderService);
                cnn.SaveChanges();
                foreach (var item in Images)
                {
                    var orderImage = new OrderServiceImage
                    {
                        OrderServiceID = OrderService.ID,
                        Path = item,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                    };
                    cnn.OrderServiceImages.Add(orderImage);
                }
                cnn.SaveChanges();
                if (usePoint > 0)
                {
                    var transactionCus = new MembersTransactionHistory
                    {
                        MemberID = cnn.Members.Where(x => x.CustomerID == CustomerID).Select(x => x.ID).FirstOrDefault(),
                        OrderServiceID = OrderService.ID,
                        Title = "Dùng điểm tích lũy khách hàng",
                        Content = "Giao hàng",
                        Point = usePoint,
                        Type = Constant.TRANSACTION_SUBTRACT_POINT,
                        TransactionType = Constant.TYPE_TRANSACTION_USE_POINT,
                        TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        BeforeBalance = walletCus.Balance + usePoint,
                        AfterBalance = walletCus.Balance,
                        Status = Constant.STATUS_TRANSACTION_SUCCESS,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        Icon = "",
                        WalletID = walletCus.ID,
                        IsExtra = false,
                    };
                    cnn.MembersTransactionHistories.Add(transactionCus);
                    cnn.SaveChanges();
                }
                return OrderService.ID;
            }
            catch (Exception e)
            {
                return SystemParam.BOOK_DRIVER_FAIL;
            }
        }
        // S đơn giao hàng
        public int UpdateOrderPackageVIP(int OrderID, string LatiTo, string LongiTo, string AddressTo, string AddressToDetail, string ReceiverName, string ReceiverPhone
            , int TransportType, int CODFee, string PackageType, double Weight, int PackageFee, string Note
            , int IsPaymentReceiver, List<string> Images, int Width, int Length, int Height, int ProvinceID, int DistrictID)
        {
            try
            {
                var order = cnn.OrderServices.FirstOrDefault(x => x.ID.Equals(OrderID));
                var origin = order.Lati.Value.ToString() + "," + order.Longi.Value.ToString();
                var destination = LatiTo + "," + LongiTo;
                if (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo))
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var LatiToVal = double.Parse(LatiTo, System.Globalization.CultureInfo.InvariantCulture);
                var LongiToVal = double.Parse(LongiTo, System.Globalization.CultureInfo.InvariantCulture);
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                // Google Map Json
                var map = JsonConvert.DeserializeObject<Map>(json);
                if (map == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                if (place_id == null || place_id_finish == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }

                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var area_start = "";
                var province_start = "";
                if (mapStart.status == "OK")
                {
                    area_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_2") || x.types.Contains("locality")).Select(x => x.long_name).FirstOrDefault();
                    province_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_start.Contains("Tỉnh Hà Tây") || province_start.Contains("Hanoi"))
                    {
                        province_start = "Hà Nội";
                    }
                }
                // Gọi API lấy chi tiết địa điểm đến
                var reqDetailEndAddress = GetGoogleMapDetailPlaceApi(place_id_finish);
                var jsonEndAddress = requestBus.GetJson(reqDetailEndAddress);
                var mapEnd = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonEndAddress);
                var province_end = "";
                if (mapEnd.status == "OK")
                {
                    province_end = mapEnd.result.address_components.Where(x => x.types.Contains("administrative_area_level_1")).Select(x => x.long_name).FirstOrDefault();
                    if (province_end.Contains("Tỉnh Hà Tây") || province_end.Contains("Hanoi"))
                    {
                        province_end = "Hà Nội";
                    }
                }
                var startProvinceID = cnn.Provinces.Where(x => province_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                var endProvinceID = cnn.Provinces.Where(x => province_end.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                if (area_start == "" || province_start == "" || province_end == "" || area_start == null || province_start == null || province_end == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var isProvince = startProvinceID == endProvinceID ? SystemParam.IN_PROVINCE : SystemParam.OUT_PROVINCE;
                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var timeShip = map.routes[0].legs[0].duration.Value / SystemParam.Minute;
                var basePrice = CalculateShipperFeeVip(distance, Weight, TransportType, isProvince);
                var totalPrice = 0;
                var couponPoint = 0;
                var cus = cnn.Customers.FirstOrDefault(x => x.ID.Equals(order.CustomerID));
                couponPoint = (int)((double)basePrice * cus.VipDiscount.GetValueOrDefault() / 100);
                totalPrice = basePrice - couponPoint;

                order.FinishLati = LatiToVal;
                order.FinishLongi = LongiToVal;
                order.FinishAddress = AddressTo;
                order.FinishAddressDetail = AddressToDetail;
                order.RecevieverName = ReceiverName;
                order.RecevieverPhone = ReceiverPhone;
                order.Weight = Weight;
                order.Length = Length;
                order.Width = Width;
                order.Height = Height;
                order.Note = Note;
                order.TransportType = TransportType;
                order.TotalPrice = basePrice - couponPoint;
                order.TimeShip = timeShip;
                order.BasePrice = basePrice;
                order.CouponPoint = couponPoint;
                order.PackageFee = PackageFee;
                order.PackageType = PackageType;
                order.CODFee = CODFee;
                order.IsPaymentReceiver = IsPaymentReceiver;
                order.ShiperCommission = order.BasePrice * order.Shiper.ConfigCommission.MastersBenefit / 100;
                order.ProvinceID = (int?)ProvinceID != 0 ? (int?)ProvinceID : null;
                order.DistrictID = (int?)DistrictID != 0 ? (int?)DistrictID : null;
                cnn.SaveChanges();
                var orderImageOld = cnn.OrderServiceImages.Where(x => x.OrderServiceID.Equals(OrderID)).ToList();
                cnn.OrderServiceImages.RemoveRange(orderImageOld);
                cnn.SaveChanges();
                foreach (var item in Images)
                {
                    var orderImage = new OrderServiceImage
                    {
                        OrderServiceID = OrderID,
                        Path = item,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                    };
                    cnn.OrderServiceImages.Add(orderImage);
                }
                cnn.SaveChanges();
                return OrderID;
            }
            catch (Exception e)
            {
                return SystemParam.BOOK_DRIVER_FAIL;
            }
        }
        // Tạo đơn giao đồ ăn
        public int CreateOrderFood(int CustomerID, string LatiTo, string LongiTo, string AddressTo, int ShopID, int CouponID, int Point, int PaymentMethod)
        {
            try
            {
                if (String.IsNullOrEmpty(LatiTo) || String.IsNullOrEmpty(LongiTo))
                {
                    return SystemParam.LOCATION_EMPTY;
                }
                var shop = cnn.Shops.FirstOrDefault(x => x.ID == ShopID && x.IsActive == SystemParam.ACTIVE);
                if (shop == null)
                {
                    return SystemParam.SHOP_NOT_FOUND;
                }
                var cart = cnn.Carts.Where(x => x.CustomerID == CustomerID && x.ShopID == ShopID && x.IsActive == SystemParam.ACTIVE && x.ServicePrice.IsActive == SystemParam.ACTIVE).ToList();
                if (cart == null || cart.Count == 0)
                {
                    return SystemParam.CART_EMPTY;
                }
                var totalPrice = cart.Sum(x => x.Quantity * x.Price);
                if (shop.Lati <= 0 || shop.Logi <= 0)
                {
                    return SystemParam.SHOP_LOCATION_NOT_VALID;
                }
                var origin = shop.Lati.ToString() + "," + shop.Logi.ToString();
                var destination = LatiTo + "," + LongiTo;
                var LatiToVal = double.Parse(LatiTo, System.Globalization.CultureInfo.InvariantCulture);
                var LongiToVal = double.Parse(LongiTo, System.Globalization.CultureInfo.InvariantCulture);
                var req = GetGoogleMapApiRequest(origin, destination);
                var json = requestBus.GetJson(req);
                // Google Map Json
                var map = JsonConvert.DeserializeObject<Map>(json);
                if (map == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }

                var place_id = map.geocoded_waypoints[0].place_id;
                var place_id_finish = map.geocoded_waypoints[1].place_id;
                if (place_id == null || place_id_finish == null)
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                if (place_id.Length > 30)
                {
                    place_id = place_id.Substring(0, 30);
                }
                if (place_id_finish.Length > 30)
                {
                    place_id_finish = place_id_finish.Substring(0, 30);
                }
                var start_address = map.routes[0].legs[0].start_address;
                var end_address = map.routes[0].legs[0].end_address;
                var AddressFrom = shop.Address;
                // Gọi API lấy chi tiết địa điểm đi
                var reqDetailStartAddress = GetGoogleMapDetailPlaceApi(place_id);
                var jsonStartAddress = requestBus.GetJson(reqDetailStartAddress);
                var mapStart = JsonConvert.DeserializeObject<GooglePlaceAPI>(jsonStartAddress);
                var area_start = "";
                if (mapStart.status == "OK")
                {
                    area_start = mapStart.result.address_components.Where(x => x.types.Contains("administrative_area_level_2") || x.types.Contains("locality")).Select(x => x.long_name).FirstOrDefault();
                }
                if (area_start == "")
                {
                    return SystemParam.LOCATION_NOT_VALID;
                }
                var AreaId = 0;
                if (area_start.Trim().Contains("Từ Liêm") || area_start.Trim().Contains("Thạch Tân"))
                {
                    AreaId = cnn.Areas.Where(x => AddressFrom.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                else
                {
                    AreaId = cnn.Areas.Where(x => area_start.ToLower().Trim().Contains(x.Name.ToLower().Trim())).Select(x => x.ID).FirstOrDefault();
                }
                if (AreaId == 0)
                {
                    return SystemParam.AREA_NOT_FOUND;
                }

                var distance = Math.Round((float)map.routes[0].legs[0].distance.Value / (float)SystemParam.Km, 1);
                var timeShip = map.routes[0].legs[0].duration.Value / SystemParam.Minute;
                var basePrice = CalculateShipperFee(distance, 0, SystemParam.SHIP_FOOD, 1);
                if (basePrice <= 0)
                {
                    return SystemParam.ORDER_TRANSPORT_FOOD_NOT_VALID;
                }
                totalPrice += basePrice;
                var couponPoint = 0;
                if (CouponID != 0)
                {
                    if (CheckCoupon(CouponID, CustomerID, SystemParam.COUPON_TYPE_FOOD, SystemParam.COUPON_NOT_USED))
                    {
                        using (var dbContextTransaction = cnn.Database.BeginTransaction())
                        {
                            var coupon = cnn.Coupons.FirstOrDefault(x => x.ID == CouponID);
                            if (coupon == null)
                            {
                                return SystemParam.COUPON_NOT_VALID;
                            }
                            var CouponCustomer = cnn.CouponCustomers.Where(x => x.CouponID == CouponID).ToList();
                            if (CouponCustomer.Count() == 0)
                            {
                                if (coupon.Remain <= 0)
                                {
                                    return SystemParam.COUPON_NOT_REMAIN;
                                }
                                cnn.Database.ExecuteSqlCommand(@"Update Coupon SET remain = remain - 1  where ID = " + coupon.ID);
                            }
                            if (coupon.TypeCoupon == SystemParam.COUPON_TYPE_COUPON_PERCENT && coupon.Percent.HasValue)
                            {
                                couponPoint = (totalPrice * coupon.Percent.Value / 100);
                            }
                            else if (coupon.TypeCoupon == SystemParam.COUPON_TYPE_COUPON_DISCOUNT && coupon.Discount.HasValue)
                            {
                                couponPoint = coupon.Discount.Value;
                            }
                            dbContextTransaction.Commit();
                            dbContextTransaction.Dispose();
                        }
                    }
                    else
                    {
                        return SystemParam.COUPON_NOT_VALID;
                    }
                }
                if (couponPoint > totalPrice)
                {
                    totalPrice = 0;
                }
                else
                {
                    totalPrice -= couponPoint;
                }
                var walletCus = cnn.Wallets.Where(x => x.Member.CustomerID == CustomerID).FirstOrDefault();
                if (walletCus == null)
                {
                    return SystemParam.WALLET_CUSTOMER_NOT_FOUND;
                }
                var usePoint = 0;
                if (Point > walletCus.Balance)
                {
                    return SystemParam.POINT_NOT_VALID;
                }
                else
                {
                    if (Point <= totalPrice)
                    {
                        totalPrice -= Point;
                        usePoint = Point;
                        walletCus.Balance -= Point;
                    }
                    else
                    {
                        usePoint = totalPrice;
                        walletCus.Balance -= totalPrice;
                        totalPrice = 0;
                    }
                }
                var VehicleID = cnn.VehicleTypes.Where(x => x.IsMotorbike == 1 && x.IsActive == SystemParam.ACTIVE).Select(x => x.ID).FirstOrDefault();
                if (PaymentMethod == SystemParam.PAYMENT_TYPE_VN_PAY && totalPrice < SystemParam.MIN_PRICE_PAYMENT_VNPAY)
                {
                    return SystemParam.ORDER_VNPAY_MINPRICE_INVALID;
                }
                var NearestShipper = FindNearestDriver(VehicleID, AreaId, shop.Lati, shop.Logi, totalPrice, PaymentMethod);
                if (NearestShipper == null)
                {
                    return SystemParam.SHIPPER_NOT_FOUND;
                }
                if (place_id.Length > 30)
                {
                    place_id = place_id.Substring(0, 30);
                }
                if (place_id_finish.Length > 30)
                {
                    place_id_finish = place_id_finish.Substring(0, 30);
                }
                var OrderService = new OrderService
                {
                    Code = GenerateCodeOrderService(SystemParam.SHIP_FOOD),
                    CustomerID = CustomerID,
                    AreaID = AreaId,
                    Status = SystemParam.ORDER_STATUS_PENDING,
                    IsActive = SystemParam.ACTIVE,
                    TypeBooking = SystemParam.SHIP_FOOD,
                    TotalPrice = totalPrice,
                    BasePrice = basePrice,
                    CouponPoint = couponPoint,
                    Distance = distance,
                    TimeShip = timeShip,
                    FinishPlaceID = place_id_finish,
                    FinishAddress = AddressTo,
                    FinishLati = LatiToVal,
                    FinishLongi = LongiToVal,
                    UsePoint = usePoint,
                    CouponID = (int?)CouponID != 0 ? (int?)CouponID : null,
                    IsRate = SystemParam.ORDER_NOT_RATE,
                    PaymentType = PaymentMethod,
                    StatusPayment = SystemParam.ORDER_NOT_PAYMENT,
                    CreatedDate = DateTime.Now,
                    BookingDate = DateTime.Now,
                    Longi = shop.Logi,
                    Lati = shop.Lati,
                    PlaceID = place_id,
                    Address = AddressFrom,
                    IsPushFirst = SystemParam.ORDER_NOT_PUSH_FIRST,
                    ShopID = ShopID,
                    CarTypeID = VehicleID,
                };
                cnn.OrderServices.Add(OrderService);
                cnn.SaveChanges();
                if (usePoint > 0)
                {
                    var transactionCus = new MembersTransactionHistory
                    {
                        MemberID = cnn.Members.Where(x => x.CustomerID == CustomerID).Select(x => x.ID).FirstOrDefault(),
                        OrderServiceID = OrderService.ID,
                        Title = "Dùng điểm tích lũy khách hàng",
                        Content = "Giao hàng",
                        Point = usePoint,
                        Type = Constant.TRANSACTION_SUBTRACT_POINT,
                        TransactionType = Constant.TYPE_TRANSACTION_USE_POINT,
                        TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                        BeforeBalance = walletCus.Balance + usePoint,
                        AfterBalance = walletCus.Balance,
                        Status = Constant.STATUS_TRANSACTION_SUCCESS,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                        Icon = "",
                        WalletID = walletCus.ID,
                        IsExtra = false,
                    };
                    cnn.MembersTransactionHistories.Add(transactionCus);
                    cnn.SaveChanges();
                }
                foreach (var item in cart)
                {
                    var orderDetail = new OrderServiceDetail
                    {
                        OrderServiceID = OrderService.ID,
                        ServicePriceID = item.ServicePriceID,
                        Price = item.Price,
                        BasePrice = item.BasePrice,
                        Quantity = item.Quantity,
                        Note = item.Note,
                        Toping = item.Toping,
                        IsActive = SystemParam.ACTIVE,
                        CreateDate = DateTime.Now,
                    };
                    cnn.OrderServiceDetails.Add(orderDetail);
                    item.IsActive = SystemParam.ACTIVE_FALSE;
                }
                cnn.SaveChanges();
                return OrderService.ID;
            }
            catch (Exception e)
            {
                return SystemParam.BOOK_DRIVER_FAIL;
            }
        }
        public void FindDriver(int OrderServiceID)
        {
            var order = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderServiceID);
            var isInternal = SystemParam.SHIPPER_PARTNER;
            if (order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_FAST || order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_STANDARD)
            {
                isInternal = SystemParam.SHIPPER_INTERNAL;
            }
            else if (order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_WESEN)
            {
                isInternal = SystemParam.SHIPPER_ALL;
            }
            else if (order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_VIP_STANDARD || order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_VIP_AIRLINE)
            {
                isInternal = SystemParam.SHIPPER_VIP_NO_COUNTDOWN;
            }
            else if (order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
            {
                isInternal = SystemParam.SHIPPER_VIP_COUNTDOWN;
            }
            if (isInternal == SystemParam.SHIPPER_INTERNAL)
            {
                // Lấy danh sách tài xế hoạt động trong khu vực
                var listDriverID = (from sh in cnn.Shipers
                                    join m in cnn.Members on sh.ID equals m.ShiperID
                                    join a in cnn.ShiperAreas on sh.ID equals a.ShiperID
                                    join cs in cnn.CarShipers on sh.ID equals cs.ShiperID
                                    where m.IsActive == SystemParam.ACTIVE && (a.AreaID == order.AreaID || a.Area.Name == order.Area.Name) && sh.IsAcceptService == true
                                    && sh.Longi > 0 && sh.Lati > 0 && cs.VehicleTypeID == order.CarTypeID && m.DeviceID.Length > 10
                                    && sh.IsInternal == isInternal
                                    group sh by sh.ID into g
                                    select g.FirstOrDefault().ID
                                  ).ToList();
                PushDriverMultipleRequest(OrderServiceID, listDriverID, order.TypeBooking);
            }
            else if (isInternal == SystemParam.SHIPPER_ALL)
            {
                // Lấy danh sách tài xế hoạt động trong khu vực
                var listDriverID = (from sh in cnn.Shipers
                                    join m in cnn.Members on sh.ID equals m.ShiperID
                                    join a in cnn.ShiperAreas on sh.ID equals a.ShiperID
                                    join cs in cnn.CarShipers on sh.ID equals cs.ShiperID
                                    where m.IsActive == SystemParam.ACTIVE && (a.AreaID == order.AreaID || a.Area.Name == order.Area.Name) && sh.IsAcceptService == true
                                    && sh.Longi > 0 && sh.Lati > 0 && cs.VehicleTypeID == order.CarTypeID && m.DeviceID.Length > 10
                                    group sh by sh.ID into g
                                    select g.FirstOrDefault().ID
                                  ).ToList();
                PushDriverMultipleRequest(OrderServiceID, listDriverID, order.TypeBooking);
            }
            else if (isInternal == SystemParam.SHIPPER_VIP_NO_COUNTDOWN)
            {
                // Lấy danh sách tài xế hoạt động trong khu vực
                var listDriverID = (from sh in cnn.Shipers
                                    join m in cnn.Members on sh.ID equals m.ShiperID
                                    join a in cnn.ShiperAreas on sh.ID equals a.ShiperID
                                    join cs in cnn.CarShipers on sh.ID equals cs.ShiperID
                                    where m.IsActive == SystemParam.ACTIVE && (a.AreaID == order.AreaID || a.Area.Name == order.Area.Name) && sh.IsAcceptService == true
                                    && sh.Longi > 0 && sh.Lati > 0 && cs.VehicleTypeID == order.CarTypeID && m.DeviceID.Length > 10 && sh.IsVip.Value == SystemParam.SHIPPER_VIP
                                    group sh by sh.ID into g
                                    select g.FirstOrDefault().ID
                                  ).ToList();
                PushDriverMultipleRequest(OrderServiceID, listDriverID, order.TypeBooking);
            }
            else if (isInternal == SystemParam.SHIPPER_PARTNER)
            {
                if (order.TypeBooking == SystemParam.SHIP_DRIVER)
                {
                    var NearestShipper = FindNearestDriver(order.CarTypeID.GetValueOrDefault(), order.AreaID, order.Lati.GetValueOrDefault(), order.Longi.GetValueOrDefault(), order.TotalPrice, order.PaymentType, isInternal, SystemParam.SHIPPER_NORMAL);
                    if (NearestShipper != null)
                    {
                        order.FirstShiperID = NearestShipper.ID;
                        cnn.SaveChanges();
                        var driverId = new List<int>();
                        driverId.Add(NearestShipper.ID);
                        PushDriverMultipleRequest(order.ID, driverId, order.TypeBooking);
                    }
                }
                else
                {
                    var NearestShipper = FindNearestDriver(order.CarTypeID.GetValueOrDefault(), order.AreaID, order.Lati.GetValueOrDefault(), order.Longi.GetValueOrDefault(), order.TotalPrice, order.PaymentType, isInternal, SystemParam.SHIPPER_ALL);
                    if (NearestShipper != null)
                    {
                        order.FirstShiperID = NearestShipper.ID;
                        cnn.SaveChanges();
                        var driverId = new List<int>();
                        driverId.Add(NearestShipper.ID);
                        PushDriverMultipleRequest(order.ID, driverId, order.TypeBooking);
                    }
                }

            }
            else if (isInternal == SystemParam.SHIPPER_VIP_COUNTDOWN)
            {
                var NearestShipper = FindNearestDriver(order.CarTypeID.GetValueOrDefault(), order.AreaID, order.Lati.GetValueOrDefault(), order.Longi.GetValueOrDefault(), order.TotalPrice, order.PaymentType, 0, SystemParam.SHIPPER_VIP);
                if (NearestShipper != null)
                {
                    order.FirstShiperID = NearestShipper.ID;
                    cnn.SaveChanges();
                    var driverId = new List<int>();
                    driverId.Add(NearestShipper.ID);
                    PushDriverMultipleRequest(order.ID, driverId, order.TypeBooking);
                }
            }


        }

        // Tiến trình gửi yêu cầu tiếp nhận cho tài xế
        public async Task PushDriverRequestProcedure()
        {
            var model = cnn.OrderServices.Where(x => x.IsActive == SystemParam.ACTIVE && x.Status == SystemParam.ORDER_STATUS_PENDING && x.ShiperID == null
                        && (x.TypeBooking != SystemParam.SHIP_PACKAGE || x.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)).ToList();
            var CountDown = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_COUNTDOWN).Select(x => x.ValueConstant).FirstOrDefault();
            var countdownVal = SystemParam.COUNTDOWN;
            if (CountDown != null && CountDown != "")
            {
                try
                {
                    countdownVal = Convert.ToInt32(CountDown);
                }
                catch
                {
                    countdownVal = SystemParam.COUNTDOWN;
                }
            }
            var TimeCancleOrder = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_TIMECANCLEORDER).Select(x => x.ValueConstant).FirstOrDefault();
            var timeCancleOrderVal = SystemParam.TIME_CANCLE_ORDER;

            if (TimeCancleOrder != null && TimeCancleOrder != "")
            {

                try
                {
                    timeCancleOrderVal = Convert.ToInt32(TimeCancleOrder);
                }
                catch
                {
                    timeCancleOrderVal = SystemParam.TIME_CANCLE_ORDER;
                }
            }
            foreach (var item in model)
            {

                if ((item.PushFirstDate.HasValue ? (DateTime.Compare(item.PushFirstDate.Value.AddSeconds(countdownVal), DateTime.Now) < 0)
                    : (DateTime.Compare(item.BookingDate.AddSeconds(countdownVal), DateTime.Now) < 0))
                    && !(item.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY && item.StatusPayment == SystemParam.ORDER_NOT_PAYMENT))
                {
                    item.IsPushFirst = SystemParam.ORDER_PUSH_FIRST;
                    item.PushFirstDate = DateTime.Now;
                    cnn.SaveChanges();
                    // Lấy danh sách tài xế hoạt động trong khu vực
                    var listDriverID = (from sh in cnn.Shipers
                                        join m in cnn.Members on sh.ID equals m.ShiperID
                                        join a in cnn.ShiperAreas on sh.ID equals a.ShiperID
                                        join cs in cnn.CarShipers on sh.ID equals cs.ShiperID
                                        join w in cnn.Wallets on m.ID equals w.MemberID
                                        where m.IsActive == SystemParam.ACTIVE && (a.AreaID == item.AreaID || a.Area.Name == item.Area.Name) && sh.IsAcceptService == true && sh.ID != item.FirstShiperID
                                        && sh.Longi > 0 && sh.Lati > 0 && cs.VehicleTypeID == item.CarTypeID && m.DeviceID.Length > 10
                                        && w.IsActive == SystemParam.ACTIVE && w.Type == Constant.WALLET_NO_WITHDRAW && (item.TransportType.Value == SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE ? sh.IsVip.Value == SystemParam.SHIPPER_VIP : true)
                                        && (item.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY ? true : (sh.IsInternal == SystemParam.SHIPPER_INTERNAL ? true : (w.Balance >= item.TotalPrice)))
                                        group sh by sh.ID into g
                                        select g.FirstOrDefault().ID
                                      ).ToList();
                    var ListOtherShipper = new List<string>();
                    var ListPushShipperID = new List<int>();
                    if (!String.IsNullOrEmpty(item.OtherShiper))
                    {
                        ListOtherShipper = item.OtherShiper.Split(',').ToList();
                    }
                    foreach (var item1 in listDriverID)
                    {
                        if (!ListOtherShipper.Contains(item1.ToString()))
                        {
                            item.OtherShiper += item1 + ",";
                            ListPushShipperID.Add(item1);
                        }
                    }
                    cnn.SaveChanges();
                    PushDriverMultipleRequest(item.ID, ListPushShipperID, item.TypeBooking);
                }
                else if (DateTime.Compare(item.BookingDate.AddSeconds(timeCancleOrderVal), DateTime.Now) < 0 && item.IsPushFirst == SystemParam.ORDER_PUSH_FIRST)
                {
                    item.Status = SystemParam.ORDER_STATUS_DENY;
                    item.CancleDate = DateTime.Now;
                    cnn.SaveChanges();
                    var TypeBooking = item.TypeBooking;
                    var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                          SystemParam.NOTI_TYPE_NAVIGATE_DENY_PACKAGE : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_DENY_FOOD : 0;
                    var content = TypeBooking == SystemParam.SHIP_DRIVER ? "đặt xe" : TypeBooking == SystemParam.SHIP_PACKAGE ?
                  "giao hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "giao đồ ăn" : "";
                    var walletCus = cnn.Wallets.FirstOrDefault(x => x.Member.CustomerID == item.CustomerID);
                    var member = cnn.Members.FirstOrDefault(x => x.CustomerID == item.CustomerID);
                    if (item.PaymentType == SystemParam.PAYMENT_TYPE_VN_PAY && item.StatusPayment == SystemParam.ORDER_PAYMENT)
                    {
                        var titleVNPAY = "Hệ thống đã hoàn lại " + Util.ConvertCurrency(item.TotalPrice) + " điểm thưởng vào tài khoản của bạn";
                        var typeVNPAY = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                        var transaction = new MembersTransactionHistory
                        {
                            MemberID = walletCus.Member.ID,
                            OrderServiceID = item.ID,
                            Title = "Hoàn tiền ví khách hàng ",
                            Content = "Hủy đơn " + content + " thanh toán VNPay ",
                            Point = item.TotalPrice,
                            Type = Constant.TRANSACTION_ADD_POINT,
                            TransactionType = Constant.TYPE_TRANSACTION_VNPAY_REFUND,
                            TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                            BeforeBalance = walletCus.Balance,
                            AfterBalance = walletCus.Balance + item.TotalPrice,
                            Status = Constant.STATUS_TRANSACTION_SUCCESS,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            Icon = "",
                            WalletID = walletCus.ID,
                            IsExtra = false,
                        };
                        walletCus.Balance += item.TotalPrice;
                        cnn.MembersTransactionHistories.Add(transaction);
                        pushNotifyBusiness.PushNotifyapp(titleVNPAY, typeVNPAY, item.ID, member.ID, 1);
                    }
                    else if (item.PaymentType == SystemParam.PAYMENT_TYPE_ON_DELIVERY && item.UsePoint > 0)
                    {
                        var typeOndelivery = SystemParam.NOTI_TYPE_NAVIGATE_REFUND_MONEY_CUSTOMER;
                        var titleOnDelivery = "Hệ thống đã hoàn lại " + Util.ConvertCurrency(item.TotalPrice) + " điểm thưởng vào tài khoản của bạn";
                        var transactionCus = new MembersTransactionHistory
                        {
                            MemberID = walletCus.Member.ID,
                            OrderServiceID = item.ID,
                            Title = "Hoàn điểm tích lũy",
                            Content = " Hủy đơn " + content,
                            Point = item.UsePoint,
                            Type = Constant.TRANSACTION_ADD_POINT,
                            TransactionType = Constant.TYPE_TRANSACTION_REFUND_USE_POINT,
                            TransactionID = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray()),
                            BeforeBalance = walletCus.Balance,
                            AfterBalance = walletCus.Balance + item.UsePoint,
                            Status = Constant.STATUS_TRANSACTION_SUCCESS,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            Icon = "",
                            WalletID = walletCus.ID,
                            IsExtra = false,
                        };
                        walletCus.Balance += item.UsePoint;
                        cnn.MembersTransactionHistories.Add(transactionCus);
                        pushNotifyBusiness.PushNotifyapp(titleOnDelivery, typeOndelivery, item.ID, member.ID, 1);
                    }

                    var titleNotFound = SystemParam.SHIPPER_NOT_FOUND_STR;
                    cnn.SaveChanges();
                    pushNotifyBusiness.PushNotifyapp(titleNotFound, type, item.ID, member.ID, 1);
                }

            }

        }
        // Đẩy thông báo cho khách hàng 
        public void PushCustomerRequestVnPay(int OrderId, int CustomerId, int TypeBooking)
        {
            var memberID = cnn.Members.Where(x => x.CustomerID == CustomerId && x.IsActive == SystemParam.ACTIVE).Select(x => x.ID).FirstOrDefault();
            var order = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderId);
            var title = "Thanh toán Vnpay " + Util.ConvertCurrency(order.TotalPrice) + " VND";
            var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_DRIVER_VNPAY : TypeBooking == SystemParam.SHIP_PACKAGE ?
                      SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_PACKAGE_VNPAY : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_BOOKING_FOOD_VNPAY : 0;
            if (memberID != 0 && order != null)
            {
                pushNotifyBusiness.PushNotifyapp(title, type, order.ID, memberID, 1);
            }

        }
        // Đẩy thông báo cho tài xế 
        public void PushDriverRequest(int OrderId, int ShipperID, int TypeBooking)
        {
            var memberID = cnn.Members.Where(x => x.ShiperID == ShipperID && x.IsActive == SystemParam.ACTIVE).Select(x => x.ID).FirstOrDefault();
            var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Có khách hàng đang đặt xe" : TypeBooking == SystemParam.SHIP_PACKAGE ?
                      "Có khách hàng đang đặt đơn giao hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "Có khách hàng đang đặt đơn giao đồ ăn" : "";
            var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_BOOK_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
                      SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_BOOK_FOOD : 0;
            var typeIcon = fullUrl + (TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_BOOK_DRIVER_ICON : TypeBooking == SystemParam.SHIP_PACKAGE ?
                      SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_ICON : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_BOOK_FOOD_ICON : "");

            if (memberID != 0)
            {
                pushNotifyBusiness.PushNotifyapp(title, type, OrderId, memberID, 3);

            }

        }// Đẩy thông báo cho nhiều tài xế 
        public void PushDriverMultipleRequest(int OrderId, List<int> listShiper, int TypeBooking)
        {
            var title = TypeBooking == SystemParam.SHIP_DRIVER ? "Có khách hàng đang đặt xe" : TypeBooking == SystemParam.SHIP_PACKAGE ?
          "Có khách hàng đang đặt đơn giao hàng" : TypeBooking == SystemParam.SHIP_FOOD ? "Có khách hàng đang đặt đơn giao đồ ăn" : "";
            var type = TypeBooking == SystemParam.SHIP_DRIVER ? SystemParam.NOTI_TYPE_NAVIGATE_BOOK_DRIVER : TypeBooking == SystemParam.SHIP_PACKAGE ?
          SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_INTERNAL : TypeBooking == SystemParam.SHIP_FOOD ? SystemParam.NOTI_TYPE_NAVIGATE_BOOK_FOOD : 0;
            var order = cnn.OrderServices.FirstOrDefault(x => x.ID == OrderId);
            if (order.TransportType.GetValueOrDefault() >= SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
            {
                title += " VIP";
            }
            //if (order != null)
            //{
            //    if (order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_STANDARD || order.TransportType.GetValueOrDefault() == SystemParam.TRANSPORT_TYPE_WESEN)
            //        type = SystemParam.NOTI_TYPE_NAVIGATE_BOOK_PACKAGE_INTERNAL;
            //}
            if (listShiper.Count > 0)
            {
                var CountDown = cnn.Configs.Where(x => x.NameConstant == SystemParam.CONFIG_COUNTDOWN).Select(x => x.ValueConstant).FirstOrDefault();
                var countdownVal = SystemParam.COUNTDOWN;
                if (CountDown != null && CountDown != "")
                {
                    try
                    {
                        countdownVal = Convert.ToInt32(CountDown);
                    }
                    catch
                    {
                        countdownVal = SystemParam.COUNTDOWN;
                    }
                }
                NotifyDataModel notifyData = new NotifyDataModel();
                notifyData.id = OrderId;
                notifyData.type = type;
                notifyData.code = "";
                notifyData.content = title;
                notifyData.timeEnd = DateTime.Now.AddSeconds(countdownVal);
                List<string> listDevice = new List<string>();
                foreach (var item in listShiper)
                {
                    var member = cnn.Members.FirstOrDefault(x => x.ShiperID == item);
                    if (member != null && member.DeviceID != null && member.DeviceID.Length > 10)
                    {
                        Notification notify = new Notification
                        {
                            Title = title,
                            Content = title,
                            Type = type,
                            IsActive = SystemParam.ACTIVE,
                            CreateDate = DateTime.Now,
                            IsRead = false,
                            MemberID = member.ID,
                            Code = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()),
                            OrderServiceID = OrderId
                        };
                        cnn.Notifications.Add(notify);
                        listDevice.Add(member.DeviceID);
                    }
                }
                if (order.TypeBooking == SystemParam.SHIP_PACKAGE && order.TransportType.GetValueOrDefault() != SystemParam.TRANSPORT_TYPE_VIP_IN_PROVINCE)
                {
                    string value = pushNotifyBusiness.PushNotify(notifyData, listDevice, SystemParam.WE_SHIP_NOTI, notifyData.content, 2);
                    pushNotifyBusiness.PushOneSignal(value, 2);
                }
                else
                {
                    string value = pushNotifyBusiness.PushNotify(notifyData, listDevice, SystemParam.WE_SHIP_NOTI, notifyData.content, 3);
                    pushNotifyBusiness.PushOneSignal(value, 3);
                }

            }
            cnn.SaveChanges();

        }

        // Tìm Tài xế gần nhất 
        public ShiperNearModel FindNearestDriver(int VehicleID, int AreaId, double LatiFrom, double LongiFrom, int TotalPrice, int PaymentMethod
                                            , int isInternal = SystemParam.SHIPPER_PARTNER, int isVip = SystemParam.SHIPPER_NORMAL)
        {
            var area = cnn.Areas.FirstOrDefault(x => x.ID == AreaId);
            // Lấy danh sách tài xế hoạt động trong khu vực
            var shipper = (from sh in cnn.Shipers
                           join m in cnn.Members on sh.ID equals m.ShiperID
                           join a in cnn.ShiperAreas on sh.ID equals a.ShiperID
                           join cs in cnn.CarShipers on sh.ID equals cs.ShiperID
                           join w in cnn.Wallets on m.ID equals w.MemberID
                           where (a.AreaID == AreaId || a.Area.Name == area.Name) && sh.IsAcceptService == true && sh.Longi > 0 && sh.Lati > 0 && m.IsActive == SystemParam.ACTIVE
                           && cs.VehicleTypeID == VehicleID && m.DeviceID.Length > 10 && w.IsActive == SystemParam.ACTIVE && w.Type == Constant.WALLET_NO_WITHDRAW
                           && (sh.IsInternal == SystemParam.SHIPPER_INTERNAL ? true : (PaymentMethod == SystemParam.PAYMENT_TYPE_VN_PAY ? true :
                               (w.Balance >= TotalPrice))) && (isInternal == SystemParam.SHIPPER_INTERNAL ? (sh.IsInternal == SystemParam.SHIPPER_INTERNAL) : true)
                               && (isVip == SystemParam.SHIPPER_VIP ? (sh.IsVip.Value == SystemParam.SHIPPER_VIP) : isVip == SystemParam.SHIPPER_NORMAL ? (!sh.IsVip.HasValue || sh.IsVip.Value == SystemParam.SHIPPER_NORMAL) : true)
                           group sh by sh.ID into g
                           select new ShiperNearModel
                           {
                               ID = g.FirstOrDefault().ID,
                               Longi = g.FirstOrDefault().Longi,
                               Lati = g.FirstOrDefault().Lati,
                               IsInternal = g.FirstOrDefault().IsInternal,
                               ModifyDate = g.FirstOrDefault().ModifyDate
                           }).ToList();
            // Tính khoảng cách vị trí shipper đến điểm xuất phát của khách hàng
            if (shipper.Count == 0)
            {
                return null;
            }
            foreach (var item in shipper)
            {
                var sCoord = new GeoCoordinate(item.Lati, item.Longi);
                var eCoord = new GeoCoordinate(LatiFrom, LongiFrom);
                item.Distance = sCoord.GetDistanceTo(eCoord);
            }
            var shipperActive = shipper.Where(x => x.ModifyDate.HasValue ? (x.ModifyDate.Value > DateTime.Now.AddDays(-2)) : false);
            var shipperNotactive = shipper.Where(x => x.ModifyDate.HasValue ? (x.ModifyDate.Value < DateTime.Now.AddDays(-2)) : true);
            if (shipperActive.Count() > 0)
            {
                var NearestShipper = shipperActive.OrderBy(x => x.IsInternal).ThenBy(x => x.Distance).FirstOrDefault();
                return NearestShipper;
            }
            else
            {
                var NearestShipper = shipperNotactive.OrderBy(x => x.IsInternal).ThenBy(x => x.Distance).FirstOrDefault();
                return NearestShipper;
            }

        }

    }
}
