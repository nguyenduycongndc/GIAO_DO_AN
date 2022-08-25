using APIProject.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using Data.Utils;
using Data.Model;
using Data.Model.APIWeb;
using Data.DB;

namespace APIProject.Controllers
{
    [UserAuthenticationFilter(2, 0)]
    public class CustomerController : BaseController
    {

        // GET: Customer
        public WE_SHIPEntities cnn = new WE_SHIPEntities();
        public ActionResult Index()
        {
            ViewBag.listCusRank = cusBusiness.GetListCustomerRank();
            ViewBag.Province = from p in cnn.Provinces orderby p.Name ascending select p;
            return View();

        }
        //[UserAuthenticationFilter]
        //public ActionResult SearchCusName(string Name)
        //{
        //    return Json(cusBusiness.LoadCusName(Name), JsonRequestBehavior.AllowGet);
        //}
        //[UserAuthenticationFilter]
        //public PartialViewResult LoadDistrict(int ProvinceID)
        //{
        //    ViewBag.listDistrict = cusBusiness.loadDistrict(ProvinceID);
        //    return PartialView("_ListDistrict");
        //}

        [UserAuthenticationFilter]
        public PartialViewResult Search(int page, string codeOrName, int? Rank, int? prvovinceID,int? IsVip)
        {
            ViewBag.codeOrName = codeOrName;
            ViewBag.rank = Rank;
            ViewBag.provinceID = prvovinceID;
            ViewBag.IsVip = IsVip;
            return PartialView("_ListCustomer", cusBusiness.Search(page, codeOrName, Rank, prvovinceID,IsVip));
        }
        //[UserAuthenticationFilter]
        //public int AddPoint(string Phone, int Point, string Note)
        //{
        //    return cusBusiness.addPoint(Phone, Point, Note);
        //}
        //[UserAuthenticationFilter]
        //public int addPointAll(string listID, string listCusPhone, int Point, string Note)
        //{
        //    return cusBusiness.addPointAll(listID, listCusPhone, Point, Note);
        //}
        //public int UnlockCustomer(int ID)
        //{
        //    return cusBusiness.UnlockCustomer(ID);
        //}
        [UserAuthenticationFilter]
        public ActionResult CusDetail(int id)
        {
            try
            {
                var list = cusBusiness.CustomerDetail(id);
                return View("_CustomerDetail", list);
            }
            catch
            {
                return View("_CustomerDetail", new CustomerDetailViewModel());
            }
        }
        [UserAuthenticationFilter]
        public int InActiveCustomer(int ID)
        {
            return cusBusiness.InActiveCustomer(ID);
        }
        [UserAuthenticationFilter]
        public int IsVipCustomer(int ID)
        {
            return cusBusiness.IsVipCustomer(ID);
        }        
        [UserAuthenticationFilter]
        public int UpdateDiscountCustomer(int ID,double Discount)
        {
            return cusBusiness.UpdateDiscountCustomer(ID,Discount);
        }
        [UserAuthenticationFilter]
        public PartialViewResult CustomerDetail(int id, int PageOrder, int PagePoint)
        {
            try
            {
                var list = cusBusiness.cusDetail(id, PageOrder, PagePoint);
                if (list != null)
                {
                    return PartialView("_CustomerDetail", list);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            //    //ViewBag.CusDetail = cusBusiness.cusDetail(ID);
            //    //ViewBag.MemberRank = rankBusiness.getRankByLever(1);
            //    //ViewBag.SliverRank = rankBusiness.getRankByLever(2);
            //    //ViewBag.GoldRank = rankBusiness.getRankByLever(3);
            //    //ViewBag.PlatinumRank = rankBusiness.getRankByLever(4);
            //    //ViewBag.PageBefore = Page;        
        }
        //[UserAuthenticationFilter]
        //public int ChangeStatusCustomer(int id, int status)
        //{
        //    SystemResult result = cusBusiness.ChangeStatusCustomer(id, status);
        //    return result.Status;
        //}
        //[UserAuthenticationFilter]
        //public JsonResult ChangeRankCustomer(int id, int customerID)
        //{
        //    var result = cusBusiness.ChangeRankCustomer(id, customerID);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        //[UserAuthenticationFilter]
        //public int SaveEditCustomer(string Name, string Phone, string Email, int Sex, string BirthDay, string Address, int ID, float Lati, float Long)
        //{
        //    try
        //    {
        //        return cusBusiness.SaveEditCustomer(Name, Phone, Email, Sex, BirthDay, Address, ID);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return SystemParam.RETURN_FALSE;
        //    }
        //}

        //[UserAuthenticationFilter]
        //public PartialViewResult SearchHistoryPoint(int? id)
        //{
        //    List<MemberTransactionViewModel> list = cusBusiness.GetListTransaction(id).ToList();
        //    return PartialView("_ListHistoryPoint", list);
        //}

        //[UserAuthenticationFilter]
        //public FileResult ExportRequest(string CodeOrName, string Phone, string Email, int? Status, string FromDate, string ToDate)
        //{
        //    return File(cusBusiness.ExportExcel(CodeOrName, Phone, Email, Status, FromDate, ToDate).GetAsByteArray(), "application / vnd.openxmlformats - officedocument.spreadsheetml.sheet", "List_Customer.xlsx");
        //}

        ////[UserAuthenticationFilter]
        ////public PartialViewResult SearchHistoryPoint(int Point, string Description, DateTime Date)
        ////{
        ////    ViewBag.Point = Point;
        ////    ViewBag.Description = Description;
        ////    ViewBag.Date = Date;
        ////    IPagedList<MemberTransactionViewModel> list = cusBusiness.SearchHistoryPoint(Point, Description, Date.ToString()).ToPagedList(Point);
        ////    //IPagedList<GetListHistoryMemberPointInputModel> list = cusBusiness.SearchHistoryPoint(cusID, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
        ////    return PartialView("_ListHistoryPoint");
        ////}

        //[UserAuthenticationFilter]
        //public PartialViewResult SearchReQuest(int Page, int cusID, string FromDate, string ToDate)
        //{
        //    ViewBag.cusID = cusID;
        //    ViewBag.FromDateRQ = FromDate;
        //    ViewBag.ToDateRQ = ToDate;
        //    IPagedList<ListRequestOutputModel> list = cusBusiness.SearchReQuest(cusID, FromDate, ToDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
        //    return PartialView("_ListRequest", list);
        //}

        //[UserAuthenticationFilter]
        //public int DeleteCustomer(int ID)
        //{
        //    return cusBusiness.DeleteCustomer(ID);
        //}

        //[UserAuthenticationFilter]
        //public PartialViewResult searchOrderHistory(int Page, int cusID, string fromDate, string toDate)
        //{
        //    ViewBag.cusID = cusID;
        //    ViewBag.FromDateOH = fromDate;
        //    ViewBag.ToDateOH = toDate;
        //    IPagedList<ListOrderHistory> list = cusBusiness.searchOrderHistory(cusID, fromDate, toDate).ToPagedList(Page, SystemParam.MAX_ROW_IN_LIST_WEB);
        //    return PartialView("_ListOrderHistory", list);
        //}

        //public ActionResult GoHome()
        //{
        //    return Json(Url.Action("Index", "Customer"));
        //}

        ////Delete customer
        //[UserAuthenticationFilter]
        //public JsonResult DelCus(int ID)
        //{
        //    return Json(cusBusiness.DeleteCustomer(ID), JsonRequestBehavior.AllowGet);

        //}

        //public JsonResult GetCusTagInput()
        //{
        //    var listcus = cusBusiness.listCustomer().Select(c => new CustomerTagInputModel
        //    {
        //        Id = c.CustomerID,
        //        Phone = c.PhoneNumber,
        //        Name = c.CustomerName
        //    });
        //    return Json(listcus, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        //public int ResetPass(int Id)
        //{
        //    return cusBusiness.ResetPass(Id);
        //}


        //public int ChangeBoBCustomer(int Id, string DOB)
        //{
        //    try
        //    {
        //        var res = cusBusiness.ChangeBoBCustomer(Id, DOB);
        //        return SystemParam.SUCCESS;
        //    }
        //    catch
        //    {
        //        return SystemParam.ERROR;
        //    }
        //}

    }
}