﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EC.COM.Data
{
    public class DBContext: DbContext
    {
        public DbSet<CompanyInvitationModel> CompanyInvitations { get; set; }

        public DBContext(): base("ECEntities")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CompanyInvitationModel>()
                .ToTable("company_invitation");

            base.OnModelCreating(modelBuilder);
        }
    }
}