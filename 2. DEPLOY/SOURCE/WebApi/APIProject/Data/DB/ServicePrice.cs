//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class ServicePrice
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ServicePrice()
        {
            this.Carts = new HashSet<Cart>();
            this.OrderServiceDetails = new HashSet<OrderServiceDetail>();
            this.ServiceImages = new HashSet<ServiceImage>();
            this.Topings = new HashSet<Toping>();
        }
    
        public int ID { get; set; }
        public Nullable<int> ServiceID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int ShopID { get; set; }
        public int BasePrice { get; set; }
        public int Discount { get; set; }
        public string Code { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int USDBasePrice { get; set; }
        public int USDPrice { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Cart> Carts { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderServiceDetail> OrderServiceDetails { get; set; }
        public virtual Service Service { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ServiceImage> ServiceImages { get; set; }
        public virtual Shop Shop { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Toping> Topings { get; set; }
    }
}