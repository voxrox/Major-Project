﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DxcGRE.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Major_ProjectEntities : DbContext
    {
        public Major_ProjectEntities()
            : base("name=Major_ProjectEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<employee> employees { get; set; }
        public virtual DbSet<gre_manager> gre_manager { get; set; }
        public virtual DbSet<dashboard> dashboards { get; set; }
        public virtual DbSet<feedback> feedbacks { get; set; }
    }
}
