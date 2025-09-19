using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.Services.StatusHandlers;

public class UnsubscribeStatusHandler : AbstractSubscriptionStatusHandler
{
    private readonly IFulfillmentApiService fulfillmentApiService;
    private readonly ILogger<UnsubscribeStatusHandler> logger;

    public UnsubscribeStatusHandler(
        IFulfillmentApiService fulfillApiService,
        ISubscriptionsRepository subscriptionsRepository,
        ILicensesRepository licensesRepository,
        IClientsRepository clientsRepository,
        ISubLinesRepository subLinesRepository,
        ILogger<UnsubscribeStatusHandler> logger)
        : base(subscriptionsRepository, licensesRepository, clientsRepository, subLinesRepository)
    {
        this.fulfillmentApiService = fulfillApiService;
        this.logger = logger;
    }

    // Handles the unsubscribe flow for subscriptions in PendingUnsubscribe state
    public override async Task ProcessAsync(Guid subscriptionID)
    {
        logger?.LogInformation("Processing Unsubscribe for subscription: {SubscriptionId}", subscriptionID);

        // Retrieve subscription from local repository
        var subscription = GetSubscriptionByMicrosoftId(subscriptionID);
        if (subscription == null)
        {
            logger?.LogWarning("Subscription not found: {SubscriptionId}", subscriptionID);
            return;
        }

        logger?.LogInformation("Subscription found with plan: {PlanId}", subscription.AMPPlanId);

        // Skip if subscription is not in PendingUnsubscribe state
        if (!string.Equals(subscription.SubscriptionStatus, SubscriptionStatusEnum.PendingUnsubscribe.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            logger?.LogInformation("Subscription is not in PendingUnsubscribe state: {Status}", subscription.SubscriptionStatus);
            return;
        }

        try
        {
            // Cancel subscription via Fulfillment API
            await fulfillmentApiService.CancelSubscriptionAsync(subscriptionID);

            // Mark subscription as Unsubscribed and inactive
            subscriptionsRepository.UpdateSubscriptionStatus(
                subscriptionID.ToString(),
                SubscriptionStatusEnum.Unsubscribed.ToString(),
                false
            );

            logger?.LogInformation("Subscription marked as Unsubscribed: {SubscriptionId}", subscriptionID);
        }
        catch (Exception ex)
        {
            // Log error and mark subscription as UnsubscribeFailed
            logger?.LogError(ex, "Error unsubscribing subscription {SubscriptionId}", subscriptionID);

            subscriptionsRepository.UpdateSubscriptionStatus(
                subscriptionID.ToString(),
                SubscriptionStatusEnum.UnsubscribeFailed.ToString(),
                true
            );
        }
    }
}
