﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Testv1.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LABTestDBEntities : DbContext
    {
        public LABTestDBEntities()
            : base("name=LABTestDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblLanguage> tblLanguages { get; set; }
        public virtual DbSet<tblLevel> tblLevels { get; set; }
        public virtual DbSet<tblSyllabu> tblSyllabus { get; set; }
        public virtual DbSet<tblSyllabusLanguage> tblSyllabusLanguages { get; set; }
        public virtual DbSet<tblTrade> tblTrades { get; set; }
    }
}
