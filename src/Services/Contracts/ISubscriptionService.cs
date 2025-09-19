using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface ISubscriptionService
{
    // Creates a new subscription using data received from Microsoft.
    void CreateSubscription(SubscriptionInputModel model);

    // Updates an existing subscription with new data from Microsoft.
    void UpdateSubscription(SubscriptionInputModel model);

    // Updates only the status and active flag of a subscription.
    void UpdateStateOfSubscription(string microsoftId, string status, bool isActive);

    // Retrieves a subscription by its MicrosoftId.
    Subscriptions GetByMicrosoftId(string microsoftId);
}
