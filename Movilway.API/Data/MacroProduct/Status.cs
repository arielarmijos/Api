//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Movilway.API.Data.MacroProduct
{
    using System;
    using System.Collections.Generic;
    
    public partial class Status
    {
        public Status()
        {
            this.BranchMp = new HashSet<BranchMp>();
            this.ProductRelation = new HashSet<ProductRelation>();
        }
    
        public int Id_Status { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<BranchMp> BranchMp { get; set; }
        public virtual ICollection<ProductRelation> ProductRelation { get; set; }
    }
}
