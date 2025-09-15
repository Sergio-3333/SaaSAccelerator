using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ILicensesRepository : IBaseRepository<Licenses>
{
  int Save(Licenses license);
  Licenses GetByLicenseKey(string licenseKey);
  IEnumerable<Licenses> GetByMicrosoftId(string microsoftId);
  public Licenses GetByEmail(string email);
   int GetNextLicenseId();


}