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
    
    public partial class Purchase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Purchase()
        {
            this.JournalSuppliers = new HashSet<JournalSupplier>();
            this.PaymentSuppliers = new HashSet<PaymentSupplier>();
            this.PurchaseDetails = new HashSet<PurchaseDetail>();
        }
    
        public long Id { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public string RefNo { get; set; }
        public string InvoiceNo { get; set; }
        public Nullable<int> SupplierId { get; set; }
        public Nullable<int> TransactionTypeId { get; set; }
        public Nullable<decimal> ItemAmount { get; set; }
        public Nullable<decimal> DiscountAmount { get; set; }
        public Nullable<decimal> GSTAmount { get; set; }
        public Nullable<decimal> ExtraAmount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Narration { get; set; }
        public Nullable<int> CompanyId { get; set; }
    
        public virtual CompanyDetail CompanyDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<JournalSupplier> JournalSuppliers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentSupplier> PaymentSuppliers { get; set; }
        public virtual Purchase Purchase1 { get; set; }
        public virtual Purchase Purchase2 { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual TransactionType TransactionType { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
    }
}