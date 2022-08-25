using APIProject.App_Start;
using Data.Model.APIWeb;
using Data.Utils;
using Newtonsoft.Json;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace APIProject.Controllers
{
    public class InvoiceController : BaseController
    {
        // GET: Invoice
        public ActionResult Index()
        {
            //ViewBag.CodeOrName = "";
            //ViewBag.PaymentType = null;
            //ViewBag.ServiceId = null;
            //ViewBag.AgentId = null;
            //ViewBag.FromDate = null;
            //ViewBag.ToDate = null;
            return View();
        }
        //public PartialViewResult Search(SearchOrderService key)
        //{
        //    ViewBag.CodeOrName = key.searchKey;
        //    ViewBag.PaymentType = key.PaymentType;
        //    ViewBag.ServiceId = key.ServiceID;
        //    ViewBag.AgentId = key.AgentID;
        //    ViewBag.FromDate = key.FromDate;
        //    ViewBag.ToDate = key.ToDate;
        //    ViewBag.IsExport = key.IsExport;
        //    IPagedList<OrderServiceModel> list = orderServiceBus.Search(key);
        //    return PartialView("_listInvoice", list);
        //}
        //public JsonResult GetListService()
        //{
        //    var list = orderServiceBus.GetListService();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        //[HttpGet]
        //public JsonResult GetInvoiceDetail(int id)
        //{
        //    var invoice = orderServiceBus.GetInvoiceDetail(id);
        //    if (invoice != null)
        //    {
        //        return Json(invoice, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new GetOrderServiceDetail(), JsonRequestBehavior.AllowGet);
        //    }
        //}
        //public JsonResult GetVAT(int id)
        //{
        //    var model = orderServiceBus.GetVAT(id);
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}

        //[UserAuthenticationFilter]
        //public FileResult ExportRequest(SearchOrderService input)
        //{
        //    return File(orderServiceBus.ExportExcel(input).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "List_Invoice.xlsx");
        //}
        //public int AcceptVAT(List<int> listid)
        //{
        //    try
        //    {
        //        var res = orderServiceBus.AcceptVAT(listid);
        //        return SystemParam.SUCCESS;
        //    }
        //    catch
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}
        //public FileResult ExportVATandAccept(string listid)
        //{
        //    List<int> list = JsonConvert.DeserializeObject<List<int>>(listid);
        //    var res = orderServiceBus.AcceptVAT(list);
        //    return File(orderServiceBus.ExportVAT(list).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "ExportVAT.xlsx");
        //}
        //public FileResult ExportVAT(string listid)
        //{
        //    List<int> list = JsonConvert.DeserializeObject<List<int>>(listid);
        //    return File(orderServiceBus.ExportVAT(list).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "ExportVAT.xlsx");
        //}
        //public ActionResult ViewInvoice(int? Id = 0)
        //{
        //    var invoice = orderServiceBus.GetVAT(Id.Value).Result;
        //    return View(invoice);
        //}
        //public FileResult GetPDF(int? Id)
        //{
        //    if (Id.HasValue)
        //    {
        //        var url = HttpContext.Request.Url.Scheme + "://" + HttpContext.Request.Url.Authority + "/Invoice/ViewInvoice?Id=" + Id;
        //        var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
        //        htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
        //        var pdfBytes = htmlToPdf.GeneratePdfFromFile(url, "");
        //        return File(pdfBytes, "aplication/.pdf", "Invoice.pdf");
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
    }
}