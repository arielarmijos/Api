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
    
    public partial class MacroProductField
    {
        public MacroProductField()
        {
            this.MacroProductValue = new HashSet<MacroProductValue>();
        }
    
        public int MacroProductFieldId { get; set; }
        public int MacroProductId { get; set; }
        public int CountryId { get; set; }
        public int FieldTypeId { get; set; }
        public string FieldName { get; set; }
        public string FieldFormat { get; set; }
        public int FieldLength { get; set; }
        public string MapValueNameTo { get; set; }
        public string MapValueTo { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public bool Mandatory { get; set; }
        public System.DateTime DateCreated { get; set; }
        public string Description { get; set; }
    
        public virtual FieldType FieldType { get; set; }
        public virtual MacroProduct MacroProduct { get; set; }
        public virtual ICollection<MacroProductValue> MacroProductValue { get; set; }
    }
}