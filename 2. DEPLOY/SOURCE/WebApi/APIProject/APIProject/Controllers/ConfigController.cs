using APIProject.App_Start;
using Data.Model.APIWeb;
using Data.Utils;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using Data.DB;
using Data.Model.APIApp;
using Data.Business;
using Data.Model;
using Microsoft.Office.Interop.Excel;
using System.Web.Http;

namespace APIProject.Controllers
{
    //[UserAuthenticationFilter(2, 0)]

    public class ConfigController : BaseController
    {
        public WE_SHIPEntities cnn = new WE_SHIPEntities();
        public ActionResult Index()
        {
            ViewBag.Province = from p in cnn.Provinces orderby p.Name select p;
            var vehicle = from v in Context.VehicleTypes
                          where v.IsActive == SystemParam.ACTIVE
                          orderby v.Name
                          select v;
            ViewBag.VehicleType = vehicle.ToList();
            var data = configBusiness.GetDataSystemParamConfig();
            ViewBag.systemParamCofig = configBusiness.GetDataSystemParamConfig();
            return View();
        }
        //GET: Config
        //OrtherBusiness orther = new OrtherBusiness();
        // QABusiness qa = new QABusiness();
        // [UserAuthenticationFilter]
        // public ActionResult Index()
        // {
        //     ViewBag.listCity = cusBusiness.LoadCityCustomer();
        //     return View(configBusiness.config().Result);
        //     }
        public int CreateConfigTransportCost(int Type, int? VehicleTypeID, int? TransportType, int FirstDistance, int FirstPrice, int PerKmPrice, int? BonusFee, int? FirstWeight, int? FirstKgPrice, int? PerKgPrice, int? FeeCOD)
        {

            try
            {
                var res = configBusiness.CreateConfigTransportCost(Type, VehicleTypeID, TransportType, FirstDistance, FirstPrice, PerKmPrice, BonusFee, FirstWeight, FirstKgPrice, PerKgPrice, FeeCOD);
                return res;
            }
            catch
            {
                return SystemParam.RETURN_FALSE;
            }
        }
        public int SaveEditConfigTransportCost(int ID, int Type, int? VehicleTypeID, int? TypeTransport, int FirstDistance, int FirstPrice, int PerKmPrice, int? FirstWeight, double? PerKg, int? PerKgPrice)
        {
            try
            {
                var res = configBusiness.SaveEditConfigTransportCost(ID, Type, VehicleTypeID, TypeTransport, FirstDistance, FirstPrice, PerKmPrice, FirstWeight, PerKg, PerKgPrice);
                return res;
            }
            catch
            {
                return SystemParam.RETURN_FALSE;
            }
        }
        public PartialViewResult ModalEditConfigTransportCost(int ID)
        {
            try
            {
                ViewBag.Province = from p in cnn.Provinces select p;
                var vehicle = from v in Context.VehicleTypes
                              where v.IsActive == SystemParam.ACTIVE
                              orderby v.Name
                              select v;
                ViewBag.VehicleType = vehicle.ToList();
                var res = configBusiness.ModalEditConfigTransportCost(ID);
                return PartialView("_EditTransportCost", res);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_EditTransportCost");
            }
        }
        public PartialViewResult SearchConfigTranportCost(int Page, int? Type, int? VehicleTypeID, int? IsActive)
        {
            try
            {

                var res = configBusiness.SearchConfigTranportCost(Page, Type, VehicleTypeID, IsActive);
                return PartialView("_TableTransportCost", res);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableTransportCost");
            }
        }

        public JsonResult DeleteTransportCost(int id)
        {
            var data = configBusiness.DeleteTransportCost(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Tìm kiếm loại xe
        public PartialViewResult SearchVehicleType(string vehicleName, int? isActive, int page = 1 /*string vehicleName, int? isActive*/)
        {
            try
            {
                var listcate = configBusiness.SearchVehicleType(page, /*null, null*/ vehicleName, isActive);
                ViewBag.vehicleName = vehicleName;
                return PartialView("_tableVehicleType", listcate);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_tableVehicleType");
            }
        }
        //Thêm mới loại xe
        //public int Create(string name, int oder, string img)
        //{
        //    return configBusiness.CreateVehicleType(name, oder, img);

        //}
        public JsonResult Create(VehicleTypeModel VehicleType)
        {
            return Json(configBusiness.CreateVehicleType(VehicleType), JsonRequestBehavior.AllowGet);
        }
        //Binding dữ liệu lên modal sửa loại xe
        public JsonResult GetVehicleType(int id)
        {
            try
            {
                var cate = configBusiness.GetVehicleType(id);
                return base.Json(cate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Lưu  khi sửa loại xe
        public bool SaveEditVehicleType(VehicleTypeModel VehicleType)
        {
            try
            {
                var query = configBusiness.SaveEditVehicleType(VehicleType);
                return query;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        //Xóa loại xe
        public JsonResult DeleteVehicleType(int id)
        {
            var data = configBusiness.DeleteVehicleType(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        // Hiển thị bảng Hạng thành viên Customer Rank
        public PartialViewResult ViewCustomerRank(int Page, int? IsActive)
        {
            try
            {

                var cust = configBusiness.ViewCustomerRank(Page, IsActive);
                return PartialView("_TableCustomerRank", cust);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableCustomerRank");
            }
        }

        public int updateCustomerRank(UpdateCustomerRankModel input)
        {
            try
            {
                var res = configBusiness.UpdateCustomerRank(input.ID, input.Name, input.MinPoint, input.MaxPoint, input.Description, input.ProfitCash, input.ProfitVPN, input.PointBonus, input.ProfitExtraBirthDay, input.Policy);
                return res;
            }
            catch
            {
                return SystemParam.RETURN_FALSE;
            }
        }

        public PartialViewResult ModalEditCustomerRank(int ID)
        {
            try
            {
                var res = configBusiness.ModalEditCustomerRank(ID);
                return PartialView("_EditCustomerRank", res);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_EditCustomerRank");
            }
        }

        //Xóa khu vực
        public JsonResult DeleteActiveArea(int id)
        {
            var data = configBusiness.DeleteActiveArea(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //Tìm kiếm khu vực
        public PartialViewResult SearchActiveArea(int page = 1, int districtId = 0, int provinceId = 0)
        {
            try
            {
                var listcate = configBusiness.SearchActiveArea(page, districtId, provinceId);
                ViewBag.districtId = districtId;
                ViewBag.provinceId = provinceId;
                return PartialView("_tableActiveArea", listcate);

            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_tableActiveArea");
            }

        }
        //Thêm mới khu vực
        public JsonResult createArea(ActiveAreaModel activeAreaModel)
        {
            //return configBusiness.CreateActiveArea(name, districtId, isActive);
            return Json(configBusiness.CreateActiveArea(activeAreaModel), JsonRequestBehavior.AllowGet);
        }
        //Binding dữ liệu lên modal sửa khu vực
        public JsonResult GetArea(int id)
        {
            try
            {
                var cate = configBusiness.GetAreaActive(id);
                return base.Json(cate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Lưu  khi sửa khu vực
        public bool SaveEditAreaActive(ActiveAreaModel activeAreaModel)
        {
            try
            {
                var query = configBusiness.SaveEditAreaActive(activeAreaModel);
                return query;
            }
            catch (Exception ex)
            {

                throw;
            }


        }
        public PartialViewResult SearchCommission(int Page, string Name)
        {
            try
            {
                var list = configBusiness.SearchCommission(Page, Name);
                return PartialView("_lstCommission", list);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_lstCommissdon");
            }

        }
        /// <summary>
        /// Sửa dữ liệu cấu hình ví
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult UpdateWalletDataConfig([FromBody] WalletDataConfig input)
        {
            return Json(configBusiness.UpdateWalletDataConfig(input), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật dữ liệu cấu hình vị trí
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult UpdateAreaDataConfig([FromBody] AreaDataCofig input)
        {
            return Json(configBusiness.UpdateAreaDataConfig(input), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhật dữ liệu cấu hình giao dịch
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult UpdateTransactionDataConfig([FromBody] TransactionDataConfig input)
        {
            return Json(configBusiness.UpdateTransactionDataConfig(input), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Cập nhậ dữ liệu cấu hình khung giờ cao điểm
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult UpdatePeakHourDataConfig([FromBody] List<PeakHourConfig> input)
        {
            return Json(configBusiness.UpdatePeakHourDataConfig(input), JsonRequestBehavior.AllowGet);
        }

        //Tìm kiếm HOA HỒNG
        public PartialViewResult SearchConfigCommission(int page, string nameCommission)
        {
            try
            {
                var listcate = configBusiness.SearchConfigCommission(page, nameCommission);
                ViewBag.nameCommission = nameCommission;
                return PartialView("_tableComission", listcate);

            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_tableComission");
            }

        }
        //Xóa hoa hồng
        public JsonResult DeleteConfigCommission(int id)
        {
            var data = configBusiness.DeleteConfigCommission(id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        //Thêm mới hoa hồng
        public int createConfigCommission(string commissionName, int mastersBenefit)
        {
            return configBusiness.CreateConfigCommission(commissionName, mastersBenefit);
        }
        //Binding dữ liệu lên modal sửa hoa hồng
        public JsonResult GetConfigCommissions(int id)
        {
            try
            {
                var cate = configBusiness.GetConfigCommission(id);
                return base.Json(cate, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        //Lưu  khi sửa hoa hồng
        public bool saveEditConfigCommission(ConfigCommissionModel commission)
        {
            try
            {
                var query = configBusiness.SaveEditConfigCommission(commission);
                return query;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        /// <summary>
        /// Lấy dữ liệu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <returns></returns>
        public PartialViewResult GetDataConfigTransportArea()
        {
            //return Json(configBusiness.GetDataConfigTransportArea(), JsonRequestBehavior.AllowGet);
            try
            {
                var getdata = configBusiness.GetDataConfigTransportArea();

                return PartialView("_TableTransportArea", getdata);

            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_TableTransportArea");
            }
        }

        /// <summary>
        /// Chi tiết cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="transportAreaID"></param>
        /// <returns></returns>
        public JsonResult GetDataConfigTransportAreaPriceDetail(int transportAreaID)
        {
            return Json(configBusiness.GetDataConfigTransportAreaPriceDetail(transportAreaID), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Tạo dữ liêu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult CreateDataConfigTransportArea([FromBody] ConfigTransportAreaInputModel input)
        {
            return Json(configBusiness.CreateDataConfigTransportArea(input), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Cập nhật dữ liêu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult UpdateDataConfigTransportArea([FromBody] ConfigTransportAreaInputModel input)
        {
            return Json(configBusiness.UpdateDataConfigTransportArea(input), JsonRequestBehavior.AllowGet);
        }


        public PartialViewResult ModalEditTransportArea(int ID)
        {
            try
            {
                var res = configBusiness.ModalEditTransportArea(ID);
                return PartialView("_EditTransportArea", res);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_EditTransportArea");
            }
        }

        public PartialViewResult ModalDetailTransportArea(int transportAreaID)
        {
            try
            {
                var res = configBusiness.GetDataConfigTransportAreaPriceDetail(transportAreaID);
                return PartialView("_DetailTransportArea", res);
            }
            catch (Exception ex)
            {
                ex.ToString();
                return PartialView("_DetailTransportArea");
            }
        }

        /// <summary>
        /// Xóa dữ liêu cấu hình giá dịch vụ chuyển phát nhanh
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>

        //[System.Web.Http.HttpPost]
        //public JsonResult DeleteDataConfigTransportArea([FromBody] ConfigTransportAreaInputModel input)
        //{
        //    return Json(configBusiness.DeleteDataConfigTransportArea(input), JsonRequestBehavior.AllowGet);
        //}

        public int DeleteDataConfigTransportArea(int ID)
        {
            try
            {
                return configBusiness.DeleteDataConfigTransportArea(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }

        /// <summary>
        /// Thêm dữ liệu cấu hình chi tết giá vận chuyển
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public JsonResult AddConfigTranspotAreaPrice([FromBody] ConfigTransportWeightInputModel input)
        {
            return Json(configBusiness.AddConfigTranspotAreaPrice(input), JsonRequestBehavior.AllowGet);
        }
    }
}