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
    
    public partial class ComplainOrder
    {
        public int ID { get; set; }
        public int OrderServiceID { get; set; }
        public string ShiperNote { get; set; }
        public string AdminNote { get; set; }
        public int Status { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public Nullable<System.DateTime> CancelDate { get; set; }
        public Nullable<System.DateTime> CompleteDate { get; set; }
    
        public virtual OrderService OrderService { get; set; }
    }
}
