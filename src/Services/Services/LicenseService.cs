using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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


    // Retrieves a license using its unique license key
    public Licenses GetByLicenseKey(string licenseKey) =>
        licenseRepository.GetByLicenseKey(licenseKey);


    // Retrieves all licenses associated with a given Microsoft ID
    public IEnumerable<Licenses> GetByMicrosoftId(string microsoftId) =>
        licenseRepository.GetByMicrosoftId(microsoftId);


    // Retrieves a license using the purchaser's email
    public Licenses GetByEmail(string email) =>
        licenseRepository.GetByEmail(email);


    // Creates or updates a license using data from SubscriptionInputModel
    public int SaveLicenseFromInputModel(SubscriptionInputModel model)
    {
        // Determine license type based on AMP plan
        int? licensesStd;
        int? licensesBiz;

        if (model.AMPPlanId == "Ant Text 365 Standart")
        {
            licensesStd = 1;
            licensesBiz = 0;
        }
        else
        {
            licensesStd = 0;
            licensesBiz = 1;
        }

        // Generate a new license ID and license key
        string newLicenseKey = GenerateUniqueLicenseKey();

        // Build license entity from input model
        var license = new Licenses
        {
            LicenseID = model.LicenseId,
            MicrosoftId = model.MicrosoftId,
            LicenseKey = newLicenseKey,
            Company = model.Company,
            City = model.City,
            Name = model.Name,
            Email = model.PurchaserEmail,
            Phone = model.Phone,
            Status = model.Status.GetHashCode(),
            PurchasedLicenses = model.UsersQ,
            Created = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            LicenseExpires = model.EndDate.ToString(),
            LicensesStd = licensesStd,
            LicensesBiz = licensesBiz
        };

        // Check if a license already exists for this email
        var existing = licenseRepository.GetByEmail(model.PurchaserEmail);
        if (existing != null)
        {
            // Update existing license, preserving its ID
            license.LicenseID = existing.LicenseID;
            licenseRepository.UpdateLicense(license);
            return existing.LicenseID;
        }
        else
        {
            // Create new license and return its ID
            return licenseRepository.CreateLicense(license);
        }
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


    public int GenerateUniqueLicenseId()
    {
        var random = new Random();
        int licenseId;

        do
        {
            licenseId = random.Next(100000, 999999);
        }
        while (licenseRepository.ExistsLicenseId(licenseId));

        return licenseId;
    }



}
