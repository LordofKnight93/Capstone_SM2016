﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iVolunteer.Models.SQL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class iVolunteerEntities : DbContext
    {
        public iVolunteerEntities()
            : base("name=iVolunteerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<SQL_AcAc_Relation> SQL_AcAc_Relation { get; set; }
        public virtual DbSet<SQL_AcAl_Relation> SQL_AcAl_Relation { get; set; }
        public virtual DbSet<SQL_Account> SQL_Account { get; set; }
        public virtual DbSet<SQL_AcGr_Relation> SQL_AcGr_Relation { get; set; }
        public virtual DbSet<SQL_AcIm_Relation> SQL_AcIm_Relation { get; set; }
        public virtual DbSet<SQL_AcPo_Relation> SQL_AcPo_Relation { get; set; }
        public virtual DbSet<SQL_AcPr_Relation> SQL_AcPr_Relation { get; set; }
        public virtual DbSet<SQL_Album> SQL_Album { get; set; }
        public virtual DbSet<SQL_Group> SQL_Group { get; set; }
        public virtual DbSet<SQL_GrPr_Relation> SQL_GrPr_Relation { get; set; }
        public virtual DbSet<SQL_GuestPr_Relation> SQL_GuestPr_Relation { get; set; }
        public virtual DbSet<SQL_Image> SQL_Image { get; set; }
        public virtual DbSet<SQL_Message> SQL_Message { get; set; }
        public virtual DbSet<SQL_Plan> SQL_Plan { get; set; }
        public virtual DbSet<SQL_Post> SQL_Post { get; set; }
        public virtual DbSet<SQL_Project> SQL_Project { get; set; }
        public virtual DbSet<SQL_HubConnection> SQL_HubConnection { get; set; }
        public virtual DbSet<SQL_Budget> SQL_Budget { get; set; }
    }
}
