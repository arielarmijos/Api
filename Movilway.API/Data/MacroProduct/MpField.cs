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
    
    public partial class MpField
    {
        public int Id_Mp { get; set; }
        public int PlatformId { get; set; }
        public int CountryId { get; set; }
        public int MpFieldNr { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string FieldFormat { get; set; }
        public Nullable<int> FieldLength { get; set; }
    
        public virtual MacroProduct MacroProduct { get; set; }
    }
}
