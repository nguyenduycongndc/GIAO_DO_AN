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
    
    public partial class CarShiper
    {
        public int ID { get; set; }
        public int VehicleTypeID { get; set; }
        public string LicensePlates { get; set; }
        public string CarColor { get; set; }
        public int ShiperID { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual Shiper Shiper { get; set; }
        public virtual VehicleType VehicleType { get; set; }
    }
}
