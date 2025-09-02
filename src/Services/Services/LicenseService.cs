using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.Saas.Accelerator.Services.Services;

public class LicenseService : ILicenseService
{
    private readonly ILicensesRepository licenseRepository;

    public LicenseService(ILicensesRepository licenseRepository)
    {
        this.licenseRepository = licenseRepository;
    }

    public LicenseResult GetLicenseById(int licenseId)
    {
        var license = licenseRepository.Get(licenseId);
        return license == null ? null : MapToResult(license);
    }

    public LicenseResult GetByLicenseKey(string licenseKey)
    {
        var license = licenseRepository.GetByLicenseKey(licenseKey);
        return license == null ? null : MapToResult(license);
    }

    public IEnumerable<LicenseResult> GetByMicrosoftId(string microsoftId)
    {
        var licenses = licenseRepository.GetByMicrosoftId(microsoftId);
        return licenses.Select(MapToResult);
    }

    public int SaveLicense(LicenseResult licenseResult)
    {
        var license = new Licenses
        {
            LicenseID = licenseResult.LicenseID,
            LicenseKey = licenseResult.LicenseKey,
            Company = licenseResult.Company,
            Email = licenseResult.Email,
            LicenseExpires = licenseResult.LicenseExpires,
            PurchasedLicenses = licenseResult.PurchasedLicenses,
            Status = licenseResult.Status,
            Comment = licenseResult.Comment
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

    private static LicenseResult MapToResult(Licenses license)
    {
        return new LicenseResult
        {
            LicenseID = license.LicenseID,
            LicenseKey = license.LicenseKey,
            Company = license.Company,
            Email = license.Email,
            LicenseExpires = license.LicenseExpires,
            PurchasedLicenses = license.PurchasedLicenses,
            Status = license.Status,
            Comment = license.Comment
        };
    }
}
