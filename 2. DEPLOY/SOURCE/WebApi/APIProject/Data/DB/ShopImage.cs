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
    
    public partial class ShopImage
    {
        public int ID { get; set; }
        public string Path { get; set; }
        public int ShopID { get; set; }
        public int Type { get; set; }
    
        public virtual Shop Shop { get; set; }
    }
}
