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
    
    public partial class Notification
    {
        public int ID { get; set; }
        public int MemberID { get; set; }
        public Nullable<int> NewsID { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public bool IsRead { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreateDate { get; set; }
        public Nullable<int> OrderServiceID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    
        public virtual Member Member { get; set; }
    }
}