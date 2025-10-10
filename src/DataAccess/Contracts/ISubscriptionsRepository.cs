using System;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ISubscriptionsRepository
{
    string AddSubscription(Subscriptions subscription);
    void UpdateSubscription(string microsoftId, Action<Subscriptions> updateAction);
    Subscriptions GetSubscriptionByMicrosoftId(string microsoftId);
}

