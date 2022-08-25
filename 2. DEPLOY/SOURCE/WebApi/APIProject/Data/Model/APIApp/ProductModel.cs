using Data.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.APIApp
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public int QTY { get; set; }
        public int BasePrice { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public List<string> ListImage { get; set; }
        public bool Isdelete { get; set; }
    }
    public class ProductDetailViewModel : ProductModel
    {
        public string Code { get; set; }
        public string NameVN { get; set; }
        public string BasePriceStr { get { return string.Format("{0:#,0}", BasePrice); } }
        public int DisplayOrder { get; set; }
        public int Status { get; set; }
    }

    public class ListOrder
    {
        public int OrderID { get; set; }
        public string Code { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public int Discount { get; set; }
        public int QTY { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateDateStr { get { return CreateDate.ToString(SystemParam.CONVERT_DATETIME); } }
        public DateTime? CompleteDate { get; set; }
        public string CompleteDateStr { get { return CompleteDate.HasValue ? CompleteDate.Value.ToString(SystemParam.CONVERT_DATETIME) : ""; } }
        public DateTime? ConfirmDate { get; set; }
        public string ConfirmDateStr { get { return ConfirmDate.HasValue ? ConfirmDate.Value.ToString(SystemParam.CONVERT_DATETIME) : ""; } }
        public int Status { get; set; }
        public string Image { get; set; }
    }
    public class OrderrDetail : ListOrder
    {
        public string WasherName { get; set; }
        public string WasherCode { get; set; }
        public string CounponCode { get; set; }
        public string WasherPhone { get; set; }
        public List<OrderProductDetail> ListOrderProductDetail { get; set; }
    }

    public class OrderProductDetail
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int QTY { get; set; }
        public int PerPrice { get; set; }
        public int Price { get; set; }
        public int BasePrice { get; set; }
        public int Discouint { get; set; }
        public List<string> ListImage { get; set; }
    }

    public class WasherCartInputModel
    {
        public string counponCode { get; set; }
        public List<WasherCartDetailInputModel> listProduct { get; set; }

    }
    public class WasherCartDetailInputModel
    {
        public int productID { get; set; }
        public int qty { get; set; }

    }
}
