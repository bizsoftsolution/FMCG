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
    
    public partial class StockGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StockGroup()
        {
            this.Products = new HashSet<Product>();
            this.StockGroup1 = new HashSet<StockGroup>();
        }
    
        public int Id { get; set; }
        public string StockGroupName { get; set; }
        public string StockGroupCode { get; set; }
        public Nullable<int> UnderStockId { get; set; }
        public Nullable<int> CompanyId { get; set; }
    
        public virtual CompanyDetail CompanyDetail { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StockGroup> StockGroup1 { get; set; }
        public virtual StockGroup StockGroup2 { get; set; }
    }
}
