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
    
    public partial class CustomerRank
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CustomerRank()
        {
            this.Coupons = new HashSet<Coupon>();
            this.Customers = new HashSet<Customer>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Policy { get; set; }
        public int ProfitCash { get; set; }
        public int ProfitVPN { get; set; }
        public int MinPoint { get; set; }
        public int EarnPoint { get; set; }
        public int MaxPoint { get; set; }
        public int PointBonus { get; set; }
        public int ProfitExtraBirthDay { get; set; }
        public int Level { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Title { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Coupon> Coupons { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
