using Data.Business;
using Data.DB;
using Data.Model.APIApp;
using Data.Model.ShopModel;
using Data.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace APIProject.Controllers
{
    public class ShopAPIController : BaseAPIController
    {
        ResponseBusiness rp = new ResponseBusiness();

        public string GetToken()
        {
            if (Request.Headers.Contains("token"))
            {
                return Request.Headers.GetValues("token").FirstOrDefault();
            }
            return "";
        }
        
        [HttpGetAttribute]
        public object detail(int id)
        {
            try
            {
                string token = GetToken();
                Member member = lgBus.checkShopToken(token);
                if (member == null)
                {
                    return rp.tokenError();
                }
                else if (!member.ShopID.HasValue)
                {
                    return rp.ErrorResult("Không có quyền thực hiện chức năng", 500);
                }
                return shopBus.getServiceDetail(id, member.ShopID.Value);
            }
            catch
            {
                return rp.ErrorResult(MessVN.ERROR_STR, SystemParam.PROCESS_ERROR);
            }
        }
        [HttpPost]
        public object DeleteService([FromBody] ShopDeleteInputModel input)
        {
            try
            {
                string token = GetToken();
                Member member = lgBus.checkShopToken(token);
                if (member == null)
                {
                    return rp.tokenError();
                }
                else if (!member.ShopID.HasValue)
                {
                    return rp.ErrorResult("Không có quyền thực hiện chức năng", 500);
                }
                return shopBus.deleteService(input.ID);
            }
            catch
            {
                return rp.ErrorResult(MessVN.ERROR_STR, SystemParam.PROCESS_ERROR);
            }
        }

        [HttpPost]
        public object CreateOrUpdateService()
        {
            try
            {
                string token = GetToken();
                Member member = lgBus.checkShopToken(token);
                if (member == null)
                {
                    return rp.tokenError();
                }
                else if (!member.ShopID.HasValue)
                {
                    return rp.ErrorResult("Không có quyền thực hiện chức năng", 500);
                }
                var httpRequest = HttpContext.Current.Request;
                var data = httpRequest.Form;
                ServicePriceDetailModel item = new ServicePriceDetailModel
                {
                    Name = httpRequest.Form.GetValues("Name").FirstOrDefault(),
                    Price = Util.ParseInt(data.GetValues("Price").FirstOrDefault(), 0).GetValueOrDefault(),
                    BasePrice = Util.ParseInt(data.GetValues("BasePrice").FirstOrDefault(), 0).GetValueOrDefault(),
                    Description = data.GetValues("Description").FirstOrDefault(),
                    ServiceID = Util.ParseInt(data.GetValues("ServiceID").FirstOrDefault()),
                    ServiceType = Util.ParseInt(data.GetValues("ServiceType").FirstOrDefault(), 1).GetValueOrDefault(),
                    IsActive = Util.ParseInt(data.GetValues("IsActive").FirstOrDefault(), 1).GetValueOrDefault(),
                    Images = Util.UploadFile("image"),
                    ID = Util.ParseInt(data.GetValues("ID").FirstOrDefault()).GetValueOrDefault(),
                };
                return shopBus.checkCreateOrUpdate(item, member.ShopID.Value);
            }
            catch
            {
                return rp.ErrorResult(MessVN.ERROR_STR, SystemParam.PROCESS_ERROR);
            }
        }
        [HttpPost]
        public object ChangeStatus([FromBody] ChangeStatusModel input)
        {
            try
            {
                string token = GetToken();
                Member member = lgBus.checkShopToken(token);
                if (member == null)
                {
                    return rp.tokenError();
                }
                else if (!member.ShopID.HasValue)
                {
                    return rp.ErrorResult("Không có quyền thực hiện chức năng", 500);
                }
                return shopBus.changeStatus(input.Status, member.ShopID.Value);
            }
            catch
            {
                return rp.ErrorResult(MessVN.ERROR_STR, SystemParam.PROCESS_ERROR);
            }
        }

        [HttpGet]
        public object list(int? serviceID = null, int? isActive = null, int type = 1)
        {
            try
            {
                string token = GetToken();
                Member member = lgBus.checkShopToken(token);
                if (member == null)
                {
                    return rp.tokenError();
                }
                else if (!member.ShopID.HasValue)
                {
                    return rp.ErrorResult("Không có quyền thực hiện chức năng", 500);
                }
                return shopBus.getMenuByShop(serviceID, isActive, member.ShopID.Value, type);
            }
            catch
            {
                return rp.ErrorResult(MessVN.ERROR_STR, SystemParam.PROCESS_ERROR);
            }
        }

    }
}