using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class SystemResultBusiness
    {
        public SystemResult SucessResult(object Result, string mess = "")
        {
            SystemResult result = new SystemResult();
            result.Status = SystemParam.SUCCESS;
            result.Code = SystemParam.SUCCESS_CODE;
            result.Message = mess;
            result.Result = Result;
            result.Exception = "";
            return result;
        }
        public SystemResult ErrorResult(string ex,int code = SystemParam.PROCESS_ERROR)
        {
            SystemResult result = new SystemResult();
            result.Status = SystemParam.ERROR;
            result.Code = code;
            result.Message = ex.ToString();
            result.Message = ex;
           // result.Result = result;
            result.Exception = ex;
            return result;
        }
    }
}
