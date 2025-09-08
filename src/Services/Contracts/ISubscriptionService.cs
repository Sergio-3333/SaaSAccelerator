using System;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface ISubscriptionService
{
    void UpdateStateOfSubscription(Guid subscriptionId, string status, bool isActivate);
    SubscriptionResultExtension GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool includeUnsubscribed = true);
    SubscriptionStatusEnumExtension GetSubscriptionStatus(string subscriptionStatus);

}
