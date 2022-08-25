using Data.DB;
using Data.Model.APIApp;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.Business
{
    public class CarBusiness : GenericBusiness
    {
        RequestAPIBusiness apiBus = new RequestAPIBusiness();
        public CarBusiness(WE_SHIPEntities context = null) : base()
        {

        }

        public object listCarBrand(string search, int? carBrandID, string lang)
        {
            ListCarBrandModel output = new ListCarBrandModel();
            output.listCarMode = cnn.CarModels.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Name.Contains(search) && (carBrandID.HasValue ? c.CarBrandID.Value.Equals(carBrandID.Value) : true) && c.CarSegmentID.HasValue).Select(c => new CarModeModel
            {
                CarModelID = c.ID,
                CarBrandName = c.CarBrandID.HasValue ? c.CarBrand.Name : "Nothing",
                CarBrandID = c.CarBrandID,
                Name = c.Name,
                SegmentName = lang.Equals(SystemParam.VN) ? c.CarSegment.NameVN : c.CarSegment.NameEN,
                SegmentDescription = c.CarSegment.Description
            }).ToList();
            output.listCarBrand = cnn.CarBrands.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && (carBrandID.HasValue ? c.ID.Equals(carBrandID.Value) : true) && c.Name.Contains(search)).Select(c => new
            {
                CarBrandID = c.ID,
                CarBrandName = c.Name
            }).ToList();
            return output;
        }
        public object GetListCarModel(string brandName = "", string search = "")
        {
            ListCarBrandModel output = new ListCarBrandModel();
            output.listCarMode = cnn.CarModels.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Name.Contains(search) && (c.CarBrand.Name.Contains(brandName) && c.CarSegmentID.HasValue)).Select(c => new CarModeModel
            {
                CarModelID = c.ID,
                CarBrandName = c.CarBrandID.HasValue ? c.CarBrand.Name : "Nothing",
                CarBrandID = c.CarBrandID,
                Name = c.Name,
                SegmentName = c.CarSegment.NameEN,
                SegmentDescription = c.CarSegment.Description
            }).ToList();
            output.listCarBrand = cnn.CarBrands.Where(c => c.IsActive.Equals(SystemParam.ACTIVE) && c.Name.Contains(brandName)).Select(c => new
            {
                CarBrandID = c.ID,
                CarBrandName = c.Name
            }).ToList();
            return output;
        }
        public string DeleteCarCustomer(int CarID, string lang)
        {
            CarCustomer carCus = cnn.CarCustomers.Find(CarID);
            if (carCus == null)
                return lang.Equals(SystemParam.EN) ? MessEN.NOT_FOUND_MESS : MessVN.NOT_FOUND_MESS;
            var listOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CarCustomerID.Value.Equals(CarID)).ToList();
            if (listOrder.Count > 0)
                return lang.Equals(SystemParam.VN) ? "Bạn không thế xoá xe này" : "You cant delete this car";
            carCus.IsActive = SystemParam.INACTIVE;
            cnn.SaveChanges();
            return SystemParam.SUCCES_STR;
        }
        public object ListSegment()
        {

            return cnn.CarSegments.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new ListGrade()
            {
                ID = u.ID,
                Name = u.NameEN,
            }).ToList();
        }
        public int? addCarCustomer(int customerID, CarModeInputModel car)
        {
            CarCustomer item = cnn.CarCustomers.Where(u => u.LicensePlates.Equals(car.LicensePlates) && u.IsActive.Equals(SystemParam.ACTIVE) && u.CarModel.CarSegmentID.HasValue).FirstOrDefault();
            if (item != null)
                return null;
            CarCustomer carCus = new CarCustomer();
            carCus.CustomerID = customerID;
            carCus.CarModeID = car.CarModelID;
            carCus.LicensePlates = car.LicensePlates;
            carCus.ManufacturingDate = car.ManufacturingDate;
            carCus.RegistrationDate = car.RegistrationDateTime;
            carCus.CarColor = car.CarColor;
            carCus.IsActive = SystemParam.ACTIVE;
            carCus.CreateDate = DateTime.Now;
            carCus.StatusCar = car.Status;
            carCus.VehicleRegistration = carCus.VehicleRegistration;
            carCus.verify = SystemParam.INACTIVE;
            cnn.CarCustomers.Add(carCus);
            cnn.SaveChanges();
            int ID = cnn.CarCustomers.OrderByDescending(u => u.ID).FirstOrDefault().ID;
            return ID;
        }
        public string UpdateCarCustomer(int customerID, CarModeInputModel car,string lang)
        {
            CarCustomer carCus = cnn.CarCustomers.Where(u => u.ID.Equals(car.CarID) && u.CustomerID.Equals(customerID)).FirstOrDefault();
            if (carCus == null)
                return lang.Equals(SystemParam.EN) ? MessEN.NOT_FOUND_MESS : MessVN.NOT_FOUND_MESS;
            var listOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CarCustomerID.Value.Equals(car.CarID)).ToList();
            if (listOrder.Count > 0)
                return lang.Equals(SystemParam.VN) ? "Bạn không thế sửa xe này" : "You cant edit this car";
            carCus.CarModeID = car.CarModelID;
            carCus.LicensePlates = car.LicensePlates;
            carCus.ManufacturingDate = car.ManufacturingDate;
            carCus.RegistrationDate = car.RegistrationDateTime;
            carCus.CarColor = car.CarColor;
            carCus.StatusCar = car.Status;
            carCus.VehicleRegistration = car.VehicleRegistration;
            return SystemParam.SUCCES_STR;
            cnn.SaveChanges();
        }
        public CarOutputModel GetCarDetail(int ID)
        {
            CarOutputModel query = cnn.CarCustomers.Where(u => u.ID.Equals(ID) && u.IsActive.Equals(SystemParam.ACTIVE)).Select(c => new CarOutputModel
            {
                carID = c.ID,
                CarBrand = c.CarModel.CarBrand.Name,
                CarModelID = c.CarModeID,
                CarModel = c.CarModel.Name,
                CarColor = c.CarColor,
                LicensePlates = c.LicensePlates,
                ManufacturingDate = c.ManufacturingDate,
                RegistrationDate = c.RegistrationDate.Value,
                CarBrandID = c.CarModel.CarBrandID.Value,
                Status = String.IsNullOrEmpty(c.StatusCar) ? "" : c.StatusCar,
                VehicleRegistration = String.IsNullOrEmpty(c.VehicleRegistration) ? "" : c.VehicleRegistration,
                ListImage = c.CarImages.Where(u => u.IsActive.Equals(SystemParam.ACTIVE)).Select(ci => new
                {
                    Url = ci.Path,
                    ImageID = ci.ID
                }).ToList()
            }).FirstOrDefault();
            return query;
        }
        public void addImageCarCustomer(List<string> Listpath, CarCustomer car)
        {
            foreach (string path in Listpath)
            {
                CarImage carimage = new CarImage();
                carimage.CarCustomerID = car.CustomerID;
                carimage.Path = path;
                carimage.CarCustomerID = car.ID;
                carimage.IsActive = SystemParam.ACTIVE;
                carimage.CraeteDate = DateTime.Now;
                cnn.CarImages.Add(carimage);
            }
            cnn.SaveChanges();
        }

        public object GetListCarBrand()
        {
            string listCarBrandStr = apiBus.GetJson(Constant.URL_CAR_BRAND);
            List<CarBrandOutputAPIModel> listCarBrand = JsonConvert.DeserializeObject<List<CarBrandOutputAPIModel>>(listCarBrandStr);
            string listCarModelStr = apiBus.GetJson(Constant.URL_CAR_MODEL);
            List<CarModelOutputAPIModel> listCarModel = JsonConvert.DeserializeObject<List<CarModelOutputAPIModel>>(listCarModelStr);
            List<CarBrandModel> listCar = listCarBrand.Select(u => new CarBrandModel
            {
                Name = u.name,
                listCarModel = listCarModel.Where(m => m.make.Equals(u.name)).Select(m => new CarModelModel
                {
                    Name = m.model,
                    Image = m.img_url,
                    Year = m.year
                }).ToList()
            }).ToList();
            return listCar;
        }
        public void CreateCarBrand(CarBrandModel car)
        {
            CarBrand carbrand = new CarBrand();
            carbrand.Name = car.Name;
            carbrand.IsActive = SystemParam.ACTIVE;
            carbrand.CreateDate = DateTime.Now;
            carbrand.CarModels = car.listCarModel.Select(
                u => new DB.CarModel
                {
                    Name = u.Name,
                    Parth = u.Image,
                    Year = u.Year,
                    IsActive = SystemParam.ACTIVE,
                    CreateDate = DateTime.Now
                }).ToList();
            cnn.CarBrands.Add(carbrand);
            cnn.SaveChanges();
        }

        public List<VehicleModel> Search(string cusName = "", string BrandName = "", string modelName = "", int? isVerify = null, string fromDateSTR = "", string toDateSTR = "")
        {
            DateTime? fromDate = Util.ConvertDate(fromDateSTR);
            DateTime? toDate = Util.ConvertDate(toDateSTR);
            List<VehicleModel> query = new List<VehicleModel>();
            query = cnn.CarCustomers.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE) &&
            (BrandName.Length > 0 ? u.CarModel.CarBrand.Name.Contains(BrandName) : true) &&
            (modelName.Length > 0 ? u.CarModel.Name.Contains(modelName) : true) &&
            (isVerify.HasValue ? u.verify.Value.Equals(isVerify.Value) : true) &&
            (fromDate.HasValue ? u.CreateDate >= fromDate.Value : true) &&
            (toDate.HasValue ? u.CreateDate <= toDate.Value : true)
            ).Select(u => new VehicleModel
            {
                ID = u.ID,
                BrandName = u.CarModel.CarBrand.Name,
                CustomerName = u.Customer.Name,
                isVeryfile = u.verify.Value,
                LicencePalte = u.LicensePlates,
                ModelName = u.CarModel.Name,
                SegmetName = u.CarModel.CarSegment != null ? u.CarModel.CarSegment.NameEN : "",
                Year = u.ManufacturingDate,
                CreatedDate=u.CreateDate
            }).OrderByDescending(u => u.ID).ToList();
            return query.Where(u => (!String.IsNullOrEmpty(cusName) && cusName.Length > 0 ? Util.Converts(u.CustomerName).Contains(Util.Converts(cusName)) : true)).ToList();
        }
        public VehicleDetailModel Detail(int ID)
        {
            try
            {
                VehicleDetailModel query = cnn.CarCustomers.Where(u => u.ID.Equals(ID)).Select(u => new VehicleDetailModel
                {
                    ID = u.ID,
                    BrandName = u.CarModel.CarBrand.Name,
                    CustomerName = u.Customer.Name,
                    isVeryfile = u.verify.Value,
                    LicencePalte = u.LicensePlates,
                    ModelName = u.CarModel.Name,
                    SegmetName = u.CarModel.CarSegment != null ? u.CarModel.CarSegment.NameEN : "",
                    Year = u.ManufacturingDate,
                    CarColor = u.CarColor,
                    RegistrationDate = u.RegistrationDate,
                    listImage = u.CarImages.Where(i => i.IsActive.Equals(SystemParam.ACTIVE)).Select(i => i.Path).ToList(),
                    CustomerAvatar = u.Customer.AvatarUrl
                }).FirstOrDefault();
                return query;
            }
            catch
            {

                return null;
            }

        }
        public List<string> GetListCarBrandBySearch(string brandName)
        {
            var query = cnn.CarBrands.Where(x => x.Name.Contains(brandName)).Select(x => x.Name).ToList();
            if (query != null && query.Count() > 0)
            {
                return query;
            }
            else
            {
                return new List<string>();
            }
        }

        public List<string> GetListCarModelByBrand(string modelName, string brandName = "")
        {
            var query = cnn.CarModels.Where(x => x.Name.Contains(modelName) && (!String.IsNullOrEmpty(brandName) && brandName.Length > 0 ? x.CarBrand.Name.Equals(brandName) : true) && !x.CarSegmentID.HasValue).Select(x => x.Name).ToList();
            if (query != null && query.Count() > 0)
            {
                return query;
            }
            else
            {
                return new List<string>();
            }
        }
        public List<string> GetListCarModelBySearch(string modelName, string brandName = "")
        {
            var query = cnn.CarModels.Where(x => x.Name.Contains(modelName) && (!String.IsNullOrEmpty(brandName) && brandName.Length > 0 ? x.CarBrand.Name.Equals(brandName) : true)).Select(x => x.Name).ToList();
            if (query != null && query.Count() > 0)
            {
                return query;
            }
            else
            {
                return new List<string>();
            }
        }
        public ExcelPackage ExportExcel(string cusName, string BrandName, string modelName, int? isVerify, string fromDateSTR, string toDateSTR)
        {
            try
            {
                FileInfo file = new FileInfo(HttpContext.Current.Server.MapPath(@"/Template/List_Vehicle.xlsx"));
                ExcelPackage pack = new ExcelPackage(file);
                ExcelWorksheet sheet = pack.Workbook.Worksheets[1];
                int row = 3;
                int stt = 1;

                var list = Search(cusName, BrandName, modelName, isVerify, fromDateSTR, toDateSTR);
                foreach (var item in list)
                {
                    sheet.Row(row).Height = 20;
                    sheet.Cells[row, 1].Value = stt;
                    sheet.Cells[row, 2].Value = item.CustomerName;
                    sheet.Cells[row, 3].Value = item.BrandName;
                    sheet.Cells[row, 4].Value = item.ModelName;
                    sheet.Cells[row, 5].Value = item.SegmetName;
                    sheet.Cells[row, 6].Value = item.LicencePalte;
                    sheet.Cells[row, 7].Value = item.Year;
                    sheet.Cells[row, 8].Value = item.isVeryfile;
                    row++;
                    stt++;
                }
                return pack;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return new ExcelPackage();
            }
        }
        public List<CarCustomerViewModel> GetCarModelByCusId(int id)
        {
            var list = cnn.CarCustomers.Where(x => x.CustomerID.Equals(id)).Select(x => new CarCustomerViewModel()
            {
                ID = x.ID,
                CarModelID = x.CarModeID,
                CarModelName = x.CarModel.Name,
                CarSegmentId = x.CarModel.CarSegment.ID,
                CarSegmentName = x.CarModel.CarSegment.NameEN
            }).ToList();
            if (list != null && list.Count() > 0)
            {
                return list;
            }
            else
            {
                return new List<CarCustomerViewModel>();
            }
        }
        public List<CarBrandViewModel> GetCarBrandSelect()
        {
            var list = cnn.CarBrands.OrderBy(x => x.Name).Select(x => new CarBrandViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();
            if (list != null && list.Count() > 0)
            {
                return list;
            }
            else
            {
                return new List<CarBrandViewModel>();
            }
        }
        public List<CarModeInputModel> GetCarModelSelect(int brandId)
        {
            var list = cnn.CarModels.Where(x => x.CarBrandID.Value.Equals(brandId) && x.CarSegmentID.HasValue).OrderBy(x => x.Name).Select(x => new CarModeInputModel
            {
                CarID = x.ID,
                Name = x.Name
            }).ToList();
            if (list != null && list.Count() > 0)
            {
                return list;
            }
            else
            {
                return new List<CarModeInputModel>();
            }
        }
        public List<CarSegmentViewModel> GetListSegment()
        {
            var list = cnn.CarSegments.Select(x => new CarSegmentViewModel
            {
                ID = x.ID,
                NameEN = x.NameEN,
            }).ToList();
            return list;
        }

        public SystemResult SearchTypeCar(string Search = "", string fDate = "", string tDate = "")
        {
            List<TypeCarModel> list = new List<TypeCarModel>();
            DateTime? fromDate = Util.ConvertDate(fDate);
            DateTime? toDate = Util.ConvertDate(tDate);
            if (toDate.HasValue)
                toDate = toDate.Value.AddDays(1);
            list = cnn.CarSegments.Where(u =>
            u.IsActive.Equals(SystemParam.ACTIVE)
            && (fromDate.HasValue ? u.CreateDate >= fromDate.Value : true)
            && (toDate.HasValue ? u.CreateDate <= toDate.Value : true)
            && (u.NameEN.Contains(Search))).Select(u => new TypeCarModel
            {
                ID = u.ID,
                CreateDate = u.CreateDate,
                NameEN = u.NameEN,
                NameVN = u.NameVN,
                Note = u.Description
            }).ToList();
            return resultBus.SucessResult(list);
        }

        public SystemResult TypeCarDetail(int ID)
        {
            TypeCarDetailModel list = new TypeCarDetailModel();
            list = cnn.CarSegments.Where(u =>
            u.ID.Equals(ID) &&
            u.IsActive.Equals(SystemParam.ACTIVE)).Select(u => new TypeCarDetailModel
            {
                ID = u.ID,
                CreateDate = u.CreateDate,
                NameEN = u.NameEN,
                NameVN = u.NameVN,
                Note = u.Description,
                listCar = u.CarModels.Where(c => c.IsActive.Equals(SystemParam.ACTIVE)).Select(c => new CarModelInCarTypeModel
                {
                    BrandName = c.CarBrand.Name,
                    ID = c.ID,
                    ModelName = c.Name,
                    Year = c.Year,
                    IsActive = c.IsActive
                }).OrderByDescending(c => c.ID).ToList()
            }).FirstOrDefault();
            return resultBus.SucessResult(list);
        }
        public SystemResult UpdateTypeCar(TypeCarDetailModel item)
        {
            CarSegment car = cnn.CarSegments.Find(item.ID);
            if (car == null)
                return resultBus.ErrorResult(MessEN.NOT_FOUND_MESS);
            car.NameEN = item.NameEN;
            car.NameVN = item.NameVN;
            car.Description = item.Note;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult DeleteCarOfSegment(int ID)
        {
            //List<CarModel> listCar = cnn.CarModels.Where(u => item.Where(c => c.IsActive.Equals(SystemParam.DEACTIVE)).Select(c => c.ID).ToList().Contains(u.ID)).ToList();
            CarModel carModel = cnn.CarModels.Find(ID);
            carModel.CarSegmentID = null;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult AddCarSegment(string brandName, string typeName, int segmentID)
        {
            CarBrand carbrand = cnn.CarBrands.Where(u => u.Name.Equals(brandName) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
            if (carbrand == null)
                return resultBus.ErrorResult("Car brand not found");
            CarModel carModel = cnn.CarModels.Where(u => u.Name.Equals(typeName) && u.IsActive.Equals(SystemParam.ACTIVE) && u.CarBrandID.Value.Equals(carbrand.ID)).FirstOrDefault();
            if (carModel == null)
                return resultBus.ErrorResult("Car model not found");
            carModel.CarSegmentID = segmentID;
            cnn.SaveChanges();
            return resultBus.SucessResult(true);
        }
        public SystemResult AddCarBrand(string brandName)
        {
            CarBrand carbrand = cnn.CarBrands.Where(u => u.Name.Equals(brandName) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
            if (carbrand != null)
                return resultBus.ErrorResult("Car brand already exist");
            CarBrand newcar = new CarBrand();
            newcar.Name = brandName;
            newcar.IsActive = SystemParam.ACTIVE;
            newcar.CreateDate = DateTime.Now;
            cnn.CarBrands.Add(newcar);
            cnn.SaveChanges();
            CarBrandViewModel newBrand = new CarBrandViewModel()
            {
                ID = newcar.ID,
                Name = newcar.Name
            };
            return resultBus.SucessResult(newBrand);
        }

        public SystemResult AddCarModel(string brandName, string typeName, int ID)
        {
            CarBrand carbrand = cnn.CarBrands.Where(u => u.Name.Equals(brandName) && u.IsActive.Equals(SystemParam.ACTIVE)).FirstOrDefault();
            if (carbrand == null)
                return resultBus.ErrorResult("Car brand not found");
            CarModel carModel = cnn.CarModels.Where(u => u.Name.Equals(typeName) && u.IsActive.Equals(SystemParam.ACTIVE) && u.CarBrandID.Value.Equals(carbrand.ID)).FirstOrDefault();
            if (carModel != null && carModel.CarSegmentID.HasValue)
                return resultBus.ErrorResult("Car model already exist");
            if (carModel!= null && !carModel.CarSegmentID.HasValue )
            {
                carModel.CarSegmentID = ID;
                cnn.SaveChanges(); 
                CarModeInputModel newModel = new CarModeInputModel()
                {
                    CarID = carModel.ID,
                    Name = carModel.Name
                };
                return resultBus.SucessResult(newModel);
            }
            else
            {
                CarModel car = new CarModel();
                car.Name = typeName;
                car.CarBrandID = carbrand.ID;
                car.CarSegmentID = ID;
                car.IsActive = SystemParam.ACTIVE;
                car.CreateDate = DateTime.Now;
                cnn.CarModels.Add(car);
                cnn.SaveChanges();
                CarModeInputModel newModel = new CarModeInputModel()
                {
                    CarID = car.ID,
                    Name = car.Name
                };
                return resultBus.SucessResult(newModel);
            }
        }
        public SystemResult DeleteCarCustomer(int id)
        {
            CarCustomer carCustomer = cnn.CarCustomers.Find(id);
            if (carCustomer != null)
            {
                var listOrder = cnn.OrderServices.Where(u => u.IsActive.Equals(SystemParam.ACTIVE) && u.CarCustomerID.Value.Equals(id)).ToList();
                if (listOrder.Count > 0)
                    return resultBus.ErrorResult("You cant delete this car");
                carCustomer.IsActive = SystemParam.INACTIVE;
                cnn.SaveChanges();
                return resultBus.SucessResult("");
            }
            else
            {
                return resultBus.ErrorResult("Not found car");
            }
        }
    }
}
