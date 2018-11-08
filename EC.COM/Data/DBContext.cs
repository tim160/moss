using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace EC.COM.Data
{
    public class DBContext: DbContext
    {
        public DbSet<CompanyInvitationModel> CompanyInvitations { get; set; }
        public DbSet<VarInfoModel> VarInfoes { get; set; }        

        public DBContext(): base("ECEntities")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<CompanyInvitationModel>()
                .ToTable("company_invitation");

            modelBuilder
                .Entity<VarInfoModel>()
                .ToTable("var_info");

            base.OnModelCreating(modelBuilder);
        }
    }
}