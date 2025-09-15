using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System.Collections.Generic;

public interface ILicenseService
{
    Licenses GetLicenseById(int licenseId);
    Licenses GetByLicenseKey(string licenseKey);
    IEnumerable<Licenses> GetByMicrosoftId(string microsoftId);

    int SaveLicenseFromInputModel(SubscriptionInputModel model, int installationId);

    void RemoveLicense(int licenseId);
}
