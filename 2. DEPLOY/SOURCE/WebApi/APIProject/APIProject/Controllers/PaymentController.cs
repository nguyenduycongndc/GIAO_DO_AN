using Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class PaymentController : BaseAPIController
    {
        // GET: Payment
        public VNPayOutputModel vnp_ipn(VnpOutputModel input)
        {
            var model = vnPayBus.GetVnpIpn(input);
            return model;
        }
    }
}