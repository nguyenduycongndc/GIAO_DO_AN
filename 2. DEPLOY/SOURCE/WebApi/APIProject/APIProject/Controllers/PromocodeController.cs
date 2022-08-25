using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Model.APIWeb;
using Data.Business;
using Data.Utils;
using PagedList;
using APIProject.App_Start;
using System.IO;

namespace APIProject.Controllers
{
    [UserAuthenticationFilter(2, 0)]
    public class PromocodeController : BaseController
    {
        [UserAuthenticationFilter]
        // GET: Promocode
        public ActionResult Index()
        {
            //ViewBag.LoadListCoupons = couponBusiness.LoadListCoupons();
            //var listRank = rankBusiness.Search("", null, null);
            ViewBag.CustomerRank = (from cr in Context.CustomerRanks where cr.IsActive == SystemParam.ACTIVE select cr);
            return View();
        }
        [HttpGet]
        [UserAuthenticationFilter]
        public PartialViewResult Search(int Page, string Code, int? CouponType, string CouponFromDate, string CouponToDate)
        {
            try
            {
                ViewBag.Code = Code;
                ViewBag.Type = CouponType;
                ViewBag.FromDate = CouponFromDate;
                ViewBag.ToDate = CouponToDate;
                var listCoupons = couponBusiness.Search(Page, Code, CouponType, CouponFromDate, CouponToDate);
                return PartialView("_listPromocode", listCoupons);
            }
            catch (Exception e)
            {
                e.ToString();
                return PartialView("_listPromocode", new List<ListCouponModel>().ToPagedList(1, 1));
            }
        }
        public int CreateCoupon(string Name, string Code, string Content, int TypeCoupon, int type, int TypeTime, string Amount, int allCustomer, string CreateDate, string ExpriceDate, int QTY, int? rank = null, List<int> listCusID = null)
        {
            try
            {
                UserDetailOutputModel userLogin = UserLogins;

                return couponBusiness.CreateCoupon(Name, Code, Content, TypeCoupon, type,  TypeTime, Amount, allCustomer.Equals(1) ? true : false, CreateDate, ExpriceDate, QTY, rank, listCusID, SystemParam.ACTIVE);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public int DeleteCoupon(int ID)
        {
            try
            {
                return couponBusiness.DeleteCoupon(ID);
            }
            catch
            {
                return SystemParam.ERROR;
            }
        }
        public PartialViewResult ModalEditCoupon(int? ID)
        {
            try
            {
                //ViewBag.listRank = rankBusiness.Search("", null, null);
                ViewBag.CustomerRank = (from cr in Context.CustomerRanks where cr.IsActive == SystemParam.ACTIVE select cr);
                ListCouponModel CouponDetail = couponBusiness.ModalEditCoupon(ID);
                return PartialView("_editPromocode", CouponDetail);
            }
            catch (Exception e)
            {
                e.ToString();
                return PartialView("_editPromocde", new ListCouponModel());
            }
        }
        [HttpPost]
        public int SaveEditCoupon(ListCouponModel obj, List<int> listCusID = null)
        {
            try
            {
                return couponBusiness.SaveEditCoupon(obj, listCusID);
            }
            catch (Exception e)
            {
                e.ToString();
                return SystemParam.RETURN_FALSE;
            }
        }
        //[HttpPost]
        //public JsonResult UploadImage()
        //{
        //    SystemResult result = new SystemResult();
        //    if (Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            //  Get all files from Request object  
        //            HttpFileCollectionBase files = Request.Files;
        //            //string fileName;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                HttpPostedFileBase file = files[i];
        //                string fname;
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }
        //                // Get the complete folder path and store the file inside it.  
        //                fname = Path.Combine(Server.MapPath("~/Uploads/images"), fname);
        //                file.SaveAs(fname);
        //            }
        //            result.Message = "Upload success";
        //            result.Status = SystemParam.SUCCESS;
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //        catch (Exception ex)
        //        {
        //            result.Message = "Upload image failed: " + ex.ToString();
        //            result.Status = SystemParam.ERROR;
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    else
        //    {
        //        result.Message = "Please select image";
        //        result.Status = SystemParam.ERROR;
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //}


        //[HttpPost]
        //public JsonResult GetListCusByCoupon(int Id)
        //{
        //    return Json(couponBusiness.GetListCusByCoupon(Id), JsonRequestBehavior.AllowGet);
        //}
    }
}