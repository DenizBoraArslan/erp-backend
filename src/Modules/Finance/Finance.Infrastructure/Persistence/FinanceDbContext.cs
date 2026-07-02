using Finance.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Infrastructure.Persistence
{
    public class FinanceDbContext : DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
                entity.HasIndex(i => i.InvoiceNumber).IsUnique();
                entity.Property(i => i.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(i => i.Status).HasConversion<string>();
                entity.Property(i => i.TotalAmount).HasPrecision(18, 2);
                entity.Property(i => i.PaidAmount).HasPrecision(18, 2);
                entity.HasMany(i => i.Items)
                      .WithOne()
                      .HasForeignKey(x => x.InvoiceId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(i => i.Payments)
                      .WithOne()
                      .HasForeignKey(p => p.InvoiceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Description).IsRequired().HasMaxLength(500);
                entity.Property(i => i.UnitPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount).HasPrecision(18, 2);
                entity.Property(p => p.Method).HasConversion<string>();
                entity.Property(p => p.Note).HasMaxLength(500);
            });
        }
    }
}
