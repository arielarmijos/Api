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
    
    public partial class AccessType
    {
        public AccessType()
        {
            this.MacroProduct = new HashSet<MacroProduct>();
        }
    
        public int AccessTypeId { get; set; }
        public string Description { get; set; }
    
        public virtual ICollection<MacroProduct> MacroProduct { get; set; }
    }
}
