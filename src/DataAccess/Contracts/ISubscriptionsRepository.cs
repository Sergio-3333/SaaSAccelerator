using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ISubscriptionsRepository
{
    string AddSubscription(Subscriptions subscription);
    void UpdateSubscription(Subscriptions subscription);
    Subscriptions GetSubscriptionByMicrosoftId(string microsoftId);
    void UpdateSubscriptionStatus(string microsoftId, string subscriptionStatus, bool isActive);
}

