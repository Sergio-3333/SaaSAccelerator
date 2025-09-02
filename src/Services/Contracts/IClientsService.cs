using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface IClientsService
{
    IEnumerable<ClientModel> GetAllClients();
    ClientModel GetClientByInstallationId(int installationId);
    ClientModel GetClientByLicenseId(int licenseId);
    ClientModel GetClientByEmail(string email);
    void CreateOrUpdateClientFromSubscription(SubscriptionModel subscription, int licenseId, int installationId);
}
