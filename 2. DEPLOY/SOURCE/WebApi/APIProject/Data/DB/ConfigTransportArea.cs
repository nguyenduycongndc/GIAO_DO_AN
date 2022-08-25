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
    
    public partial class ConfigTransportArea
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ConfigTransportArea()
        {
            this.ConfigTransportWeights = new HashSet<ConfigTransportWeight>();
            this.OrderServices = new HashSet<OrderService>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public int IsProvince { get; set; }
        public Nullable<int> FromKm { get; set; }
        public Nullable<int> ToKm { get; set; }
        public double PerKg { get; set; }
        public int PerKgPrice { get; set; }
        public string TimeShip { get; set; }
        public Nullable<int> FeeCOD { get; set; }
        public int Type { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ConfigTransportWeight> ConfigTransportWeights { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderService> OrderServices { get; set; }
    }
}
