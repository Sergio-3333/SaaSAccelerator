using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface IClientsRepository : IBaseRepository<Clients>
{
    Clients GetByInstallationId(int installationId);
    Clients GetByLicenseId(int licenseId);
    Clients GetByEmail(string email);

    void CreateClient(Clients clientEntity);
    void UpdateClient(Clients clientEntity);
}
