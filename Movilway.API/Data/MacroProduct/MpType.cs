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
    
    public partial class MpType
    {
        public MpType()
        {
            this.MacroProduct = new HashSet<MacroProduct>();
        }
    
        public int Id_MpType { get; set; }
        public string Name { get; set; }
    
        public virtual ICollection<MacroProduct> MacroProduct { get; set; }
    }
}
