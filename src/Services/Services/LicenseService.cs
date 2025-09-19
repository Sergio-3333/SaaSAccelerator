using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System;
using System.Collections.Generic;

public class LicenseService : ILicenseService
{
    private readonly ILicensesRepository licenseRepository;

    public LicenseService(ILicensesRepository licenseRepository)
    {
        this.licenseRepository = licenseRepository;
    }

    // Retrieves a license by its internal ID
    public Licenses GetLicenseById(int licenseId) =>
        licenseRepository.GetById(licenseId);

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
    public int SaveLicenseFromInputModel(SubscriptionInputModel model, int installationId)
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
        int newLicenseId = licenseRepository.GetNextLicenseId();
        string newLicenseKey = Guid.NewGuid().ToString("N").ToUpper();

        // Build license entity from input model
        var license = new Licenses
        {
            LicenseID = newLicenseId,
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
}
