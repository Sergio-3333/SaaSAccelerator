using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.Services.StatusHandlers;

public class PendingFulfillmentStatusHandler : AbstractSubscriptionStatusHandler
{
    private readonly IFulfillmentApiService fulfillmentApiService;
    private readonly ILogger<PendingFulfillmentStatusHandler> logger;

    public PendingFulfillmentStatusHandler(
        IFulfillmentApiService fulfillApiService,
        ISubscriptionsRepository subscriptionsRepository,
        ILicensesRepository licensesRepository,
        IClientsRepository clientsRepository,
        ISubLinesRepository subLinesRepository,
        ILogger<PendingFulfillmentStatusHandler> logger)
        : base(subscriptionsRepository, licensesRepository, clientsRepository, subLinesRepository)
    {
        this.fulfillmentApiService = fulfillApiService;
        this.logger = logger;
    }

    // Handles transition from PendingFulfillmentStart to PendingActivation
    public override async Task ProcessAsync(Guid subscriptionID)
    {
        logger?.LogInformation("Processing PendingFulfillment for subscription: {SubscriptionId}", subscriptionID);

        // Retrieve subscription from local repository
        var subscription = GetSubscriptionByMicrosoftId(subscriptionID);
        if (subscription == null)
        {
            logger?.LogWarning("Subscription not found: {SubscriptionId}", subscriptionID);
            return;
        }

        logger?.LogInformation("Subscription found with plan: {PlanId}", subscription.AMPPlanId);

        // Skip if subscription is not in PendingFulfillmentStart state
        if (!string.Equals(subscription.SubscriptionStatus, SubscriptionStatusEnum.PendingFulfillmentStart.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            logger?.LogInformation("Subscription is not in PendingFulfillmentStart state: {Status}", subscription.SubscriptionStatus);
            return;
        }

        try
        {
            // Optionally validate current status from Microsoft Marketplace
            var remoteSubscription = await fulfillmentApiService.GetSubscriptionByIdAsync(subscriptionID);
            logger?.LogInformation("Remote status from Microsoft: {Status}", remoteSubscription?.SaasSubscriptionStatus);

            // Update local subscription status to PendingActivation
            subscriptionsRepository.UpdateSubscriptionStatus(
                subscriptionID.ToString(),
                SubscriptionStatusEnum.PendingActivation.ToString(),
                true
            );

            logger?.LogInformation("Status updated to PendingActivation for subscription: {SubscriptionId}", subscriptionID);
        }
        catch (Exception ex)
        {
            // Log error and preserve current state
            logger?.LogError(ex, "Error updating subscription status {SubscriptionId}", subscriptionID);
        }
    }
}
