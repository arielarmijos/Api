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
    
    public partial class BranchMp
    {
        public int Id_Mp { get; set; }
        public int PlatformId { get; set; }
        public int CountryId { get; set; }
        public int Branch_Id { get; set; }
        public int StatusId { get; set; }
    
        public virtual MacroProduct MacroProduct { get; set; }
        public virtual Status Status { get; set; }
    }
}
