using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ILicensesRepository : IBaseRepository<Licenses>
{
    Licenses GetByLicenseKey(string licenseKey);
    IEnumerable<Licenses> GetByMicrosoftId(string microsoftId);
    IEnumerable<Licenses> GetByProductId(int productId);
}
