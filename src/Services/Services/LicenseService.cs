using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System;
using System.Text;

public class LicenseService : ILicenseService
{
    private readonly ILicensesRepository licenseRepository;

    public LicenseService(ILicensesRepository licenseRepository)
    {
        this.licenseRepository = licenseRepository;
    }

    // Retrieves a license by its internal ID
    public Licenses GetLicenseById(SubscriptionInputModel model) =>
        licenseRepository.GetById(model.LicenseId);


    // Retrieves all licenses associated with a given Microsoft ID
    public Licenses GetLicenseByMicrosoftId(string microsoftId) =>
        licenseRepository.GetLicenseByMicrosoftId(microsoftId);


    // Retrieves a license using the purchaser's email
    public Licenses GetByEmail(string email) =>
        licenseRepository.GetByEmail(email);


    // Creates or updates a license using data from SubscriptionInputModel
    public int SaveLicenseFromInputModel(SubscriptionInputModel model)
    {
        // Determine license type based on AMP plan
        int? licensesStd;
        int? licensesBiz;
        string comment;

        if (model.AMPPlanId == "atxttst001" || model.AMPPlanId == "atxttst003")
        {
            licensesStd = model.UsersQ;
            licensesBiz = 0;
            comment = "Ant Text MS Standard";
        }
        else
        {
            licensesStd = 0;
            licensesBiz = model.UsersQ;
            comment = "Ant Text MS Bussines";

        }

        // Generate a new license ID and license key
        string newLicenseKey = GenerateUniqueLicenseKey();

        // Build license entity from input model
        var license = new Licenses
        {
            LicenseID = model.LicenseId,
            MicrosoftID = model.MicrosoftId,
            LicenseKey = newLicenseKey,
            Company = model.Company ?? "0",
            City = model.City ?? "0",
            Name = model.Name ?? "0",
            Email = model.PurchaserEmail ?? "0",
            Phone = model.Phone ?? "0",
            Mobile= model.Mobile ?? "0",
            Status = 2,
            PurchasedLicenses = model.UsersQ,
            Created = DateTime.UtcNow.ToString("yyyyMMddHHmmssff"),
            LicenseExpires = GetLicenseExpires(model.Term),
            LicensesStd = licensesStd,
            LicensesBiz = licensesBiz,
            PartnerID = 0,
            ProductID = 0,
            Adr1 = model.Adr1 ?? "0",
            Adr2 = "0",
            GLN = "0",
            VatNo = "0",
            CountryId = 0,
            CountryMS = model.Country ?? "0",
            Zip = model.Zip ?? "0",
            Comment = comment


        };

            // Create new license and return its ID
            return licenseRepository.CreateLicense(license);
    }

    public static string GetLicenseExpires(string term)
    {
        string licenseExpires;

        if (string.Equals(term, "P1M", StringComparison.OrdinalIgnoreCase))
        {
            licenseExpires = DateTime.UtcNow
                .AddMonths(1)
                .ToString("yyyyMMddHHmmssff");
        }
        else
        {
            licenseExpires = DateTime.UtcNow
                .AddYears(1)
                .ToString("yyyyMMddHHmmssff");
        }

        return licenseExpires;
    }


    public string GenerateUniqueLicenseKey()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        string key;

        do
        {
            int length = random.Next(13, 18);
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                sb.Append(chars[random.Next(chars.Length)]);

            key = sb.ToString();
        }
        while (licenseRepository.ExistsLicenseKey(key));

        return key;
    }



}
