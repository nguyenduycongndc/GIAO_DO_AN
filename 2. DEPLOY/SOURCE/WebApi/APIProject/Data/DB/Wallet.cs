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
    
    public partial class Wallet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Wallet()
        {
            this.MembersTransactionHistories = new HashSet<MembersTransactionHistory>();
        }
    
        public int ID { get; set; }
        public int MemberID { get; set; }
        public int Balance { get; set; }
        public int Type { get; set; }
        public int IsActive { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual Member Member { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MembersTransactionHistory> MembersTransactionHistories { get; set; }
    }
}
