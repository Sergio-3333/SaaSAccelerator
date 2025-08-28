using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface IClientsRepository : IBaseRepository<Clients>
{
    Clients GetByLicenseId(int licenseId);
    IEnumerable<Clients> GetByEmail(string email);
}
