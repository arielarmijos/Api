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
    
    public partial class FieldType
    {
        public FieldType()
        {
            this.MacroProductField = new HashSet<MacroProductField>();
        }
    
        public int FieldTypeId { get; set; }
        public string FieldTypeName { get; set; }
        public string FieldTypeAcronym { get; set; }
        public string FieldTypeDescription { get; set; }
    
        public virtual ICollection<MacroProductField> MacroProductField { get; set; }
    }
}
