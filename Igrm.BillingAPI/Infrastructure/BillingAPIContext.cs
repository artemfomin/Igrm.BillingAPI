using Igrm.BillingAPI.Models.Business;
using Igrm.BillingAPI.Models.Business.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Igrm.BillingAPI.Infrastructure
{
    public class BillingAPIContext : DbContext
    {
        public DbSet<Bill> Bills;
        public DbSet<Receipt> Receipts;
        public DbSet<Order> Orders;
        public DbSet<Settlement> Settlements;

        public BillingAPIContext(DbContextOptions<BillingAPIContext> options) : base(options)
        {
            Bills = Set<Bill>();
            Receipts  = Set<Receipt>();
            Orders = Set<Order>();
            Settlements = Set<Settlement>();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                        .Property<byte[]>("RowVersion")
                        .IsRowVersion();

            modelBuilder.Entity<Order>()
                        .HasAlternateKey(c => c.OrderNumber);

            modelBuilder.Entity<Receipt>()
                        .Property<byte[]>("RowVersion")
                        .IsRowVersion();

            modelBuilder.Entity<Bill>()
                        .Property<byte[]>("RowVersion")
                        .IsRowVersion();

            modelBuilder.Entity<Settlement>()
                        .Property<byte[]>("RowVersion")
                        .IsRowVersion();
        }

    }
}
