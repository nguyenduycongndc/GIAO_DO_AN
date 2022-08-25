using Data.DB;
using Data.Model.APIWeb;
using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Business
{
    public class CategoryBusiness : GenericBusiness
    {
        public CategoryBusiness(WE_SHIPEntities context = null) : base()
        {

        }
        //public List<CategoryViewModel> Search(string name, string fromDate, string toDate)
        //{
        //    try
        //    {
        //        DateTime? startDate = Util.ConvertDate(fromDate);
        //        DateTime? endDate = Util.ConvertDate(toDate);
        //        if (endDate != null)
        //            endDate = endDate.Value.AddDays(1);
        //        if (String.IsNullOrEmpty(name))
        //            name = "";
        //        var query = (from cate in cnn.Categories
        //                     where cate.IsActive != SystemParam.INACTIVE
        //                     //&& (!String.IsNullOrEmpty(name) ? (cate.NameEN.Contains(name) || cate.NameVN.Contains(name)) : true)
        //                     && (startDate.HasValue ? cate.CreateDate >= startDate.Value : true)
        //                     && (endDate.HasValue ? cate.CreateDate <= endDate.Value : true)
        //                     select new CategoryViewModel()
        //                     {
        //                         ID = cate.ID,
        //                         Code = cate.Code,
        //                         Name = cate.NameEN,
        //                         NameVN = cate.NameVN,
        //                         Status = cate.IsActive,
        //                         DisplayOrder = cate.DisplayOrder.Value,
        //                         CreatedDate = cate.CreateDate

        //                     }).ToList();
        //        query = query.Where(x => Util.Converts(x.Name.ToLower()).Contains(Util.Converts(name.ToLower())) || Util.Converts(x.NameVN.ToLower()).Contains(Util.Converts(name.ToLower()))).ToList();
        //        return query;
        //    }
        //    catch
        //    {
        //        return new List<CategoryViewModel>();
        //    }
        //}
        //public SystemResult AddCategory(string code, int displayOrder, string nameEN, string nameVN)
        //{
        //    try
        //    {
        //        var checkCode = cnn.Categories.Where(x => x.Code.Equals(code) && x.IsActive == SystemParam.ACTIVE).FirstOrDefault();
        //        if (checkCode != null)
        //            return resultBus.ErrorResult("The code already exists");
        //        Category model = new Category()
        //        {
        //            Code = code,
        //            NameEN = nameEN,
        //            NameVN = nameVN,
        //            DisplayOrder = displayOrder,
        //            CreateDate = DateTime.Now,
        //            IsActive = 1
        //        };
        //        cnn.Categories.Add(model);
        //        cnn.SaveChanges();
        //        return resultBus.SucessResult(true);
        //    }
        //    catch
        //    {
        //        return resultBus.ErrorResult("The system is maintenance");
        //    }

        //}
        //public SystemResult EditCategory(int id, int displayOrder, int status, string nameEN, string nameVN)
        //{
        //    try
        //    {
        //        Category model = cnn.Categories.Find(id);
        //        if (model != null)
        //        {
        //            model.NameEN = nameEN;
        //            model.NameVN = nameVN;
        //            model.DisplayOrder = displayOrder;
        //            model.IsActive = status;
        //            cnn.SaveChanges();
        //            return resultBus.SucessResult(true);
        //        }
        //        else
        //        {
        //            return resultBus.ErrorResult("Can not find category id");
        //        }
        //    }
        //    catch
        //    {
        //        return resultBus.ErrorResult("The system is maintenance");
        //    }

        //}
        //public SystemResult GetById(int id)
        //{
        //    var model = cnn.Categories.Where(x => x.ID.Equals(id)).Select(x => new CategoryViewModel()
        //    {
        //        ID = x.ID,
        //        Code = x.Code,
        //        Name = x.NameEN,
        //        NameVN = x.NameVN,
        //        Status = x.IsActive,
        //        DisplayOrder = x.DisplayOrder.Value,
        //        IsProductCart = cnn.ProductOrderDetails.Where(t => t.Product.CategoryID == x.ID && t.ProductOrder.Status == (int)StatusOrderProduct.AdminConfirm && t.ProductOrder.IsActive == SystemParam.ACTIVE).Count() > 0 ? true : false
        //        /*x.Products.Select(b => b.ID).ToList().Where(t => cnn.ProductOrderDetails.Where(c => c.ID == t && c.IsActive == SystemParam.ACTIVE).Count() > 0 ? true : false).Count() > 0 ? true : false*/
        //    }).FirstOrDefault();
        //    if (model != null)
        //    {
        //        return resultBus.SucessResult(model);
        //    }
        //    else
        //    {
        //        return resultBus.ErrorResult("Can not find category id");
        //    }
        //}
        //public SystemResult DeleteCategory(int id)
        //{
        //    Category model = cnn.Categories.Find(id);
        //    if (cnn.Products.Where(u => u.CategoryID == id).Count() > 0)
        //    {
        //        return resultBus.ErrorResult("Cannot delete this product category!");
        //    }
        //    else if (model != null)
        //    {
        //        model.IsActive = SystemParam.INACTIVE;
        //        cnn.SaveChanges();
        //        return resultBus.SucessResult(true);
        //    }
        //    else
        //    {
        //        return resultBus.ErrorResult("Can not find category id");
        //    }
        //}
    }
}
