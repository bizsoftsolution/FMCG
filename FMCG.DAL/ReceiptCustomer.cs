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
    
    public partial class ReceiptCustomer
    {
        public long Id { get; set; }
        public Nullable<long> ReceiptId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public long SalesId { get; set; }
        public Nullable<decimal> Amount { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Receipt Receipt { get; set; }
        public virtual Sale Sale { get; set; }
    }
}
