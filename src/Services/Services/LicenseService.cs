using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;


namespace Marketplace.SaaS.Accelerator.Services.Services;

public class LicenseService : ILicenseService
{
    private readonly ILicensesRepository licenseRepository;

    public LicenseService(ILicensesRepository licenseRepository)
    {
        this.licenseRepository = licenseRepository;
    }

    public Licenses GetLicenseById(int licenseId) =>
        licenseRepository.GetById(licenseId);

    public Licenses GetByLicenseKey(string licenseKey) =>
        licenseRepository.GetByLicenseKey(licenseKey);

    public IEnumerable<Licenses> GetByMicrosoftId(string microsoftId) =>
        licenseRepository.GetByMicrosoftId(microsoftId);

    public Licenses GetByEmail(string email) =>
        licenseRepository.GetByEmail(email);

    /// <summary>
    /// Crea o actualiza una licencia a partir de los datos recibidos en SubscriptionInputModel.
    /// </summary>
    public int SaveLicenseFromInputModel(SubscriptionInputModel model, int installationId)
    {
        // Determinar LicensesStd y LicensesBiz según el plan
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

        // Generar LicenseId y LicenseKey
        int newLicenseId = licenseRepository.GetNextLicenseId();
        string newLicenseKey = Guid.NewGuid().ToString("N").ToUpper();

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
            PurchasedLicenses = model.Quantity,
            Created = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            LicenseExpires = model.EndDate.ToString(),
            LicensesStd = licensesStd,
            LicensesBiz = licensesBiz
        };

        // Decidir si crear o actualizar
        var existing = licenseRepository.GetByEmail(model.PurchaserEmail);
        if (existing != null)
        {
            license.LicenseID = existing.LicenseID; // mantener el ID existente
            licenseRepository.UpdateLicense(license);
            return existing.LicenseID;
        }
        else
        {
            return licenseRepository.CreateLicense(license);
        }
    }
}
