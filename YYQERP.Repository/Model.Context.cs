﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace YYQERP.Repository
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class YYQERPEntities : DbContext
    {
        public YYQERPEntities()
            : base("name=YYQERPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<UserSet> UserSet { get; set; }
        public virtual DbSet<RoleSet> RoleSet { get; set; }
        public virtual DbSet<MenuSet> MenuSet { get; set; }
        public virtual DbSet<RoleMenuSet> RoleMenuSet { get; set; }
        public virtual DbSet<DicSet> DicSet { get; set; }
        public virtual DbSet<ModelSet> ModelSet { get; set; }
        public virtual DbSet<ShelfSet> ShelfSet { get; set; }
        public virtual DbSet<ElementSet> ElementSet { get; set; }
        public virtual DbSet<BomDetailSet> BomDetailSet { get; set; }
        public virtual DbSet<BomSet> BomSet { get; set; }
        public virtual DbSet<PartDetailSet> PartDetailSet { get; set; }
        public virtual DbSet<PartSet> PartSet { get; set; }
        public virtual DbSet<ProductDetailSet> ProductDetailSet { get; set; }
        public virtual DbSet<StockInSet> StockInSet { get; set; }
        public virtual DbSet<StockSet> StockSet { get; set; }
        public virtual DbSet<ProductSet> ProductSet { get; set; }
        public virtual DbSet<StockOutSet> StockOutSet { get; set; }
        public virtual DbSet<MenuOperSet> MenuOperSet { get; set; }
        public virtual DbSet<OperSet> OperSet { get; set; }
        public virtual DbSet<RoleMenuOperSet> RoleMenuOperSet { get; set; }
        public virtual DbSet<PickMainSet> PickMainSet { get; set; }
        public virtual DbSet<PickSet> PickSet { get; set; }
        public virtual DbSet<CustomerSet> CustomerSet { get; set; }
        public virtual DbSet<SaleReportSet> SaleReportSet { get; set; }
    }
}
