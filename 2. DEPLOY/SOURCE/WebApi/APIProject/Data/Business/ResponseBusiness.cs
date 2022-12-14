using APIProject.Models;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class ResponseBusiness
    {
        public JsonResultModel response(int status, int code, string message, object data)
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = status;
            result.Code = code;
            result.Message = message;
            result.Data = data;
            return result;
        }

        public JsonResultModel tokenError()
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = SystemParam.ERROR;
            result.Code = SystemParam.ERROR_TOKEN_NOTFOUND;
            result.Message = MessVN.TOKEN_ERROR;
            result.Data = "";
            return result;
        }
        public JsonResultModel serverError()
        {
            JsonResultModel result = new JsonResultModel();
            result.Status = SystemParam.ERROR;
            result.Message = MessVN.ERROR_STR;
            result.Data = "";
            return result;
        }
        public JsonResultModel SuccessResult(string mess, object data)
        {
            JsonResultModel result = new JsonResultModel();
            result.Message = mess;
            result.Status = SystemParam.SUCCESS;
            result.Code = SystemParam.SUCCESS_CODE;
            result.Data = data;
            return result;
        }
        public JsonResultModel ErrorResult(string mess, int code)
        {
            JsonResultModel result = new JsonResultModel();
            result.Message = mess;
            result.Status = SystemParam.ERROR;
            result.Code = code;
            result.Data = null;
            return result;
        }
        public JsonResultModel Exception(string mess)
        {
            JsonResultModel result = new JsonResultModel();
            result.Message = MessVN.ERROR_STR;
            result.Status = SystemParam.ERROR;
            result.Code = SystemParam.PROCESS_ERROR;
            result.Data = null;
            result.ErrorMsg = mess;
            // oneSignal.SaveLog(mess, "");
            return result;
        }
    }
}
