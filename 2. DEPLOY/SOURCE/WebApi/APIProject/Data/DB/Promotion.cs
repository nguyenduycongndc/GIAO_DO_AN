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
    
    public partial class Promotion
    {
        public int ID { get; set; }
        public int NewsID { get; set; }
        public int DisplayOrder { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
    
        public virtual News News { get; set; }
    }
}
