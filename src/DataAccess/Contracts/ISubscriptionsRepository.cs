using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ISubscriptionsRepository : IBaseRepository<Subscriptions>
{
    Subscriptions GetByMicrosoftId(string microsoftId);
    IEnumerable<Subscriptions> GetActiveSubscriptions();
    void UpdatePlan(int subscriptionId, string ampPlanId);
    void UpdateQuantity(int subscriptionId, int ampQuantity);
    void UpdateStatus(int subscriptionId, string status, bool isActive);
}
