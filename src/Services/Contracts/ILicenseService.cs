using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System.Collections.Generic;

public interface ILicenseService
{
    // Returns a license by its ID.
    Licenses GetLicenseById(SubscriptionInputModel model);

    // Returns a license by its license key.
    Licenses GetByLicenseKey(string licenseKey);

    // Returns all licenses associated with a Microsoft tenant ID.
    IEnumerable<Licenses> GetByMicrosoftId(string microsoftId);

    // Creates or updates a license using subscription input and links it to an installation.
    int SaveLicenseFromInputModel(SubscriptionInputModel model);


}
