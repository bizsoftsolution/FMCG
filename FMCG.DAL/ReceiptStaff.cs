//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FMCG.DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReceiptStaff
    {
        public long Id { get; set; }
        public long ReceiptId { get; set; }
        public Nullable<int> StaffId { get; set; }
    
        public virtual Receipt Receipt { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
