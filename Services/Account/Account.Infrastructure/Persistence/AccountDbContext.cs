using Account.Domain.Entities;
using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Account.Infrastructure.Persistence;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
    }

    public DbSet<ChartOfAccount> ChartOfAccounts { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<JournalLine> JournalLines { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ChartOfAccount Configuration
        modelBuilder.Entity<ChartOfAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.AccountName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // JournalEntry Configuration
        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntryNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TotalDebit).HasPrecision(18, 2);
            entity.Property(e => e.TotalCredit).HasPrecision(18, 2);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        // JournalLine Configuration
        modelBuilder.Entity<JournalLine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Debit).HasPrecision(18, 2);
            entity.Property(e => e.Credit).HasPrecision(18, 2);

            entity.HasOne<JournalEntry>()
                .WithMany()
                .HasForeignKey(e => e.EntryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<ChartOfAccount>()
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
