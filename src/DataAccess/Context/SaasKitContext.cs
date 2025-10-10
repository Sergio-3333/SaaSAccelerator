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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Subscriptions>(entity => {
            entity.ToTable("Subscriptions");
            entity.HasKey(e => e.SubID);

            entity.Property(e => e.MicrosoftID).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.SubStatus).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.PlanId).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.PurEmail).HasMaxLength(225).IsUnicode(false);
            entity.Property(e => e.Country).HasMaxLength(225).IsUnicode(false);
            entity.Property(e => e.PurTenantId).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.Term).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.StartDate).HasMaxLength(225).IsUnicode(false);
            entity.Property(e => e.EndDate).HasMaxLength(225).IsUnicode(false);
            entity.Property(e => e.Active).IsRequired(false);
            entity.Property(e => e.UserID).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.AutoRenew).IsRequired(false);
            entity.Property(e => e.SubName).IsRequired(false);

        });


        modelBuilder.Entity<Licenses>(entity => {
            entity.ToTable("Licenses");

            entity.HasKey(e => e.LicenseID);
            entity.Property(e => e.MicrosoftID).HasMaxLength(36).IsUnicode(false);
            entity.Property(e => e.LicenseKey).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Company).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.City).HasMaxLength(80).IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(80).IsUnicode(false);
            entity.Property(e => e.Phone).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Mobile).HasMaxLength(20).IsUnicode(false);
            entity.Property(e => e.Created).HasMaxLength(16).IsUnicode(false);
            entity.Property(e => e.LicenseExpires).HasMaxLength(16).IsUnicode(false);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.PurchasedLicenses).IsRequired();
            entity.Property(e => e.LicensesStd).IsRequired(false);
            entity.Property(e => e.LicensesBiz).IsRequired(false);
            entity.Property(e => e.Adr1).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Zip).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Comment).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.CountryId).IsRequired(false);
            entity.Property(e => e.CountryMS).HasMaxLength(100).IsUnicode(false);



            entity.HasMany(e => e.Clients).WithOne(c => c.License).HasForeignKey(c => c.LicenseID).OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<Clients>(entity => {
            entity.ToTable("Clients");
            entity.Property(e => e.MicrosoftID).HasMaxLength(36).IsUnicode(false);
            entity.HasKey(e => e.InstallationID);
            entity.Property(e => e.OWAEmail).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoCompany).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoContact).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoEmail).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoPhone).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.OWADispName).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.OWAEWSPWD).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.OWAEWSUID).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.OWAEWSURL).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.OWAInitials).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.OWADispLang).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.UserDevice).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.LastTokenRefresh).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.CampaignGUID).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.LastLocCheck).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoTitle).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoLinkedIn).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoWebSite).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.ContactInfoAddress).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.InternalNote).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.InstallDateATC).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.LastProcessedSkipConsent);
            entity.Property(e => e.UseEWS);
            entity.Property(e => e.NewsLetterUsageCounter);
            entity.Property(e => e.FlowUsageCounter);
            entity.Property(e => e.CJMode);
            entity.Property(e => e.TestMode);
            entity.Property(e => e.TimeZone);
            entity.Property(e => e.UserDevice);
            entity.Property(e => e.TradeID);
            entity.Property(e => e.TrialDays);
            entity.Property(e => e.FirstEmailSent);
            entity.Property(e => e.ClientTypeID);
            entity.Property(e => e.SkipConsent);
            entity.Property(e => e.OWAPersonColor);
            entity.Property(e => e.UsageCounter);
            entity.Property(e => e.PartnerID);
            entity.Property(e => e.ContactInfoCountryID);
            


            entity.Property(e => e.LicenseType).IsRequired(false);
            entity.HasOne(c => c.License)
                  .WithMany(l => l.Clients)
                  .HasForeignKey(c => c.LicenseID)
                  .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<SubLines>(entity =>
        {
            entity.ToTable("Sublines");
            entity.HasKey(e => e.SubLinesID);

            entity.Property(e => e.MicrosoftID).IsRequired();
            entity.Property(e => e.ChargeDate).HasMaxLength(225).IsUnicode(false);
            entity.Property(e => e.Status).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.PlanTest).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.UsersQ).IsRequired();
            entity.Property(e => e.Country).HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Plan).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.USDTotal).HasColumnType("decimal(18,2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
