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
    
    public partial class JournalCheque
    {
        public long Id { get; set; }
        public Nullable<long> JournalId { get; set; }
        public Nullable<int> BankId { get; set; }
        public string ChequeNo { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public Nullable<System.DateTime> CollectionDate { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public Nullable<decimal> ReturnCharge { get; set; }
    
        public virtual Bank Bank { get; set; }
        public virtual Journal Journal { get; set; }
    }
}
