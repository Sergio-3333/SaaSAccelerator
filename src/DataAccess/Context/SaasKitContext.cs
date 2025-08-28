using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.DataAccess.Context;

public partial class SaasKitContext : DbContext
{
    public SaasKitContext()
    {
    }

    public SaasKitContext(DbContextOptions<SaasKitContext> options)
        : base(options)
    {
    }


    public virtual DbSet<Subscriptions> Subscriptions { get; set; }
    public virtual DbSet<License> Licenses { get; set; }
    public virtual DbSet<Clients> Clients { get; set; }
    public virtual DbSet<Products> Products { get; set; }




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
                    if (property.CurrentValue is DateTime dateValue)
                    {
                        if (dateValue == DateTime.MinValue)
                        {
                            property.CurrentValue = new DateTime(1753, 1, 1);
                        }
                    }
                }
            }
        }
    }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //dotnet 8lts require this line
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Subscriptions>(entity =>
        {
            entity.ToTable("Subscriptions");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.MicrosoftId)
                .HasColumnName("MicrosoftId")
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.Property(e => e.SubscriptionStatus)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.AMPPlanId)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.IsActive);

            entity.Property(e => e.CreateBy);

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime");

            entity.Property(e => e.ModifyDate)
                .HasColumnType("datetime");

            entity.Property(e => e.UserId);

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.AMPQuantity);

            entity.Property(e => e.PurchaserEmail)
                .HasMaxLength(225)
                .IsUnicode(false);

            entity.Property(e => e.PurchaserTenantId)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.Property(e => e.AmpOfferId)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.Term)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.StartDate)
                .HasColumnType("datetime2");

            entity.Property(e => e.EndDate)
                .HasColumnType("datetime2");

        });

        modelBuilder.Entity<Licenses>(entity =>
        {
            entity.ToTable("Licenses");

            entity.HasKey(e => e.LicenseID);

            entity.Property(e => e.MicrosoftId)
                .HasMaxLength(36)
                .IsUnicode(false);

            entity.Property(e => e.LicenseKey)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Company)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Adr1)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Adr2)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Zip)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.City)
                .HasMaxLength(80)
                .IsUnicode(false);

            entity.Property(e => e.Contact)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.Email)
                .HasMaxLength(80)
                .IsUnicode(false);

            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.VatNo)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.GLN)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.Created)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.Property(e => e.LicenseExpires)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.Property(e => e.Comment)
                .HasMaxLength(1000)
                .IsUnicode(false);
        });


        modelBuilder.Entity<Clients>(entity =>
        {
            entity.ToTable("Clients");

            entity.HasKey(e => e.InstallationID);

            entity.Property(e => e.LastAccessed)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.Property(e => e.Created)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoCompany)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoContact)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoPhone)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoEmail)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoOK)
                .HasMaxLength(3)
                .IsUnicode(false);

            entity.Property(e => e.OWADispName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OWAEmail)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OWAEWSURL)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OWAEWSUID)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OWAEWSPWD)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.OWAInitials)
                .HasMaxLength(3)
                .IsUnicode(false);

            entity.Property(e => e.OWADispLang)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.Property(e => e.LastTokenRefresh)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.Property(e => e.UserDevice)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.CampaignGUID)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.LastLocCheck)
                .HasMaxLength(16)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoTitle)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoWebSite)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoAddress)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.ContactInfoLinkedIn)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.InternalNote)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.InstallDateATC)
                .HasMaxLength(16)
                .IsUnicode(false);

            modelBuilder.Entity<Clients>()
                .HasOne(c => c.License)
                .WithMany(l => l.Clients)
                .HasForeignKey(c => c.LicenseID);

        });


        modelBuilder.Entity<Products>(entity =>
        {
            entity.ToTable("Products");

            entity.HasKey(e => e.ProductID);

            entity.Property(e => e.HostApplicationName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.ProductInfoURL)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.Property(e => e.ProductDownloadURL)
                .HasMaxLength(255)
                .IsUnicode(false);
        });





        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}