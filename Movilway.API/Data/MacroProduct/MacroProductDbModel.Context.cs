﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MacroProductosEntities : DbContext
    {
        public MacroProductosEntities()
            : base("name=MacroProductosEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<AmountType> AmountType { get; set; }
        public DbSet<BalanceType> BalanceType { get; set; }
        public DbSet<FieldType> FieldType { get; set; }
        public DbSet<MacroProduct> MacroProduct { get; set; }
        public DbSet<MacroProductBranch> MacroProductBranch { get; set; }
        public DbSet<MacroProductChannel> MacroProductChannel { get; set; }
        public DbSet<MacroProductValue> MacroProductValue { get; set; }
        public DbSet<SubCategory> SubCategory { get; set; }
        public DbSet<MacroProductField> MacroProductField { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<FavoriteAmounts> FavoriteAmounts { get; set; }
        public DbSet<AccessType> AccessType { get; set; }
    }
}
