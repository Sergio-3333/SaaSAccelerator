using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

public interface ILicenseService
{
    // Returns a license by its ID.
    Licenses GetLicenseById(SubscriptionInputModel model);

    // Returns all licenses associated with a Microsoft tenant ID.
    Licenses GetLicenseByMicrosoftId(string microsoftId);

    // Creates or updates a license using subscription input and links it to an installation.
    int SaveLicenseFromInputModel(SubscriptionInputModel model);


}
