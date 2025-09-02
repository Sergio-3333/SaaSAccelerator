using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface ILicenseService
{
    LicenseResult GetLicenseById(int licenseId);
    LicenseResult GetByLicenseKey(string licenseKey);
    IEnumerable<LicenseResult> GetByMicrosoftId(string microsoftId);
    int SaveLicense(LicenseResult license);
    void RemoveLicense(int licenseId);
}
