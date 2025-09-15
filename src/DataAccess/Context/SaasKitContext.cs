using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Context;

public partial class SaasKitContext : DbContext
{
    public SaasKitContext() { }

    public SaasKitContext(DbContextOptions<SaasKitContext> options)
        : base(options) { }

    public virtual DbSet<Subscriptions> Subscriptions { get; set; }
    public virtual DbSet<Licenses> Licenses { get; set; }
    public virtual DbSet<Clients> Clients { get; set; }
    public virtual DbSet<SubLines> SubLines { get; set; }

    public override int SaveChanges()
    {
        FixDates();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        FixDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void FixDates()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.ClrType == typeof(DateTime) || property.Metadata.ClrType == typeof(DateTime?))
                {
                    if (property.CurrentValue is DateTime dateValue && dateValue == DateTime.MinValue)
                    {
                        property.CurrentValue = new DateTime(1753, 1, 1);
                    }
                }
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriptions>(entity =>
        {
            entity.ToTable("Subscriptions");
            entity.HasKey(e => e.MicrosoftId);

            entity.Property(e => e.MicrosoftId).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.SubscriptionStatus).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.AMPPlanId).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.PurchaserEmail).HasMaxLength(225).IsUnicode(false);
            entity.Property(e => e.PurchaserTenantId).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.Term).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.StartDate).HasColumnType("datetime2");
            entity.Property(e => e.EndDate).HasColumnType("datetime2");
        });

        modelBuilder.Entity<Licenses>(entity =>
        {
            entity.ToTable("Licenses");
            entity.HasKey(e => e.LicenseID);

            entity.Property(e => e.MicrosoftId).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.LicenseKey).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Company).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(80).IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(80).IsUnicode(false);
            entity.Property(e => e.Phone).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Created).HasMaxLength(16).IsUnicode(false);
            entity.Property(e => e.LicenseExpires).HasMaxLength(16).IsUnicode(false);
        });

        modelBuilder.Entity<Clients>(entity =>
        {
            entity.ToTable("Clients");
            entity.HasKey(e => e.InstallationID);

            entity.Property(e => e.OWAEmail).HasMaxLength(100).IsUnicode(false);

            entity.HasOne(c => c.License)
                  .WithMany(l => l.Clients)
                  .HasForeignKey(c => c.LicenseID);
        });

        modelBuilder.Entity<SubLines>(entity =>
        {
            entity.ToTable("Sublines");
            entity.HasKey(e => e.SubLinesId);

            entity.Property(e => e.MicrosoftId).IsRequired();
            entity.Property(e => e.ChargeDate).HasColumnType("date");
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.AMPlan).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UsersQ).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Currency).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
