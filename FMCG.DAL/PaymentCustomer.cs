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
    
    public partial class PaymentCustomer
    {
        public long Id { get; set; }
        public Nullable<long> PaymentId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<long> SalesReturnId { get; set; }
        public Nullable<decimal> Amount { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual SalesReturn SalesReturn { get; set; }
    }
}