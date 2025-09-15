using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class LicenseService : ILicenseService
{
    private readonly ILicensesRepository licenseRepository;

    public LicenseService(ILicensesRepository licenseRepository)
    {
        this.licenseRepository = licenseRepository;
    }

    public Licenses GetLicenseById(int licenseId) =>
        licenseRepository.Get(licenseId);

    public Licenses GetByLicenseKey(string licenseKey) =>
        licenseRepository.GetByLicenseKey(licenseKey);

    public IEnumerable<Licenses> GetByMicrosoftId(string microsoftId) =>
        licenseRepository.GetByMicrosoftId(microsoftId);

    public Licenses GetByEmail(string email) =>
        licenseRepository.GetByEmail(email);


    /// <summary>
    /// Crea o actualiza una licencia a partir de los datos recibidos en SubscriptionInputModel.
    /// Siempre se guarda, exista o no cliente.
    /// </summary>
    public int SaveLicenseFromInputModel(SubscriptionInputModel model, int installationId)
    {
        // Determinar LicensesStd y LicensesBiz según el plan
        int? licensesStd = 0;
        int? licensesBiz = 0;

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

        int newLicenseId = licenseRepository.GetNextLicenseId();
        string newLicenseKey = Guid.NewGuid().ToString("N").ToUpper();

        var license = new Licenses
        {
            LicenseID = newLicenseId, // 0 si es nuevo
            MicrosoftId = model.MicrosoftId,
            LicenseKey = newLicenseKey,
            Company = model.Company,
            City = model.City,
            Name = model.Name,
            Email = model.PurchaserEmail,
            Phone = model.Phone,
            Status = model.Status.GetHashCode(),
            PurchasedLicenses = model.Quantity, // cantidad comprada
            Created = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            LicenseExpires = model.EndDate.ToString(),
            LicensesStd = licensesStd,
            LicensesBiz = licensesBiz
        };

        return licenseRepository.Save(license);
    }


    public void RemoveLicense(int licenseId)
    {
        var license = licenseRepository.Get(licenseId);
        if (license != null)
        {
            licenseRepository.Remove(license);
        }
    }
}
