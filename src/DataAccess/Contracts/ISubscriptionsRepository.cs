using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ISubscriptionsRepository : IBaseRepository<Subscriptions>
{
    int Save(Subscriptions subscription);
    Subscriptions GetByMicrosoftId(string microsoftId);
    IEnumerable<Subscriptions> GetActiveSubscriptions();
    void UpdateStatus(string microsoftId, string subscriptionStatus, bool isActive);

}
