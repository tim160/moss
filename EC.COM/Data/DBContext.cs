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
        public virtual DbSet<company> company { get; set; }
        public DbSet<email> emails { get; set; }
        public DbSet<user> users { get; set; }

        public DbSet<partner> partners { get; set; }
        public DbSet<company_payments> company_paymentss { get; set; }
        public DbSet<company_payment_training> company_payment_trainings { get; set; }

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

            modelBuilder.Entity<company>()
                .Property(e => e.next_payment_amount)
                .HasPrecision(19, 4);
            modelBuilder.Entity<email>()
                .ToTable("email");
            modelBuilder.Entity<user>()
                .ToTable("user");
            modelBuilder.Entity<partner>()
              .ToTable("partner");
            modelBuilder.Entity<company_payment_training>()
              .ToTable("company_payment_training");
            modelBuilder.Entity<company_payments>()
              .ToTable("company_payments");

      base.OnModelCreating(modelBuilder);
        }
    }
}