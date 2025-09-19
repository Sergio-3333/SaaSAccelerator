using System;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.Services.StatusHandlers;

public class PendingActivationStatusHandler : AbstractSubscriptionStatusHandler
{
    private readonly IFulfillmentApiService fulfillmentApiService;
    private readonly ILogger<PendingActivationStatusHandler> logger;
    private readonly ISubLinesRepository subLinesRepository;

    public PendingActivationStatusHandler(
        IFulfillmentApiService fulfillApiService,
        ISubscriptionsRepository subscriptionsRepository,
        ILicensesRepository licensesRepository,
        IClientsRepository clientsRepository,
        ISubLinesRepository subLinesRepository,
        ILogger<PendingActivationStatusHandler> logger)
        : base(subscriptionsRepository, licensesRepository, clientsRepository, subLinesRepository)
    {
        this.fulfillmentApiService = fulfillApiService;
        this.subLinesRepository = subLinesRepository;
        this.logger = logger;
    }

    // Handles activation logic for subscriptions in PendingActivation state
    public override async Task ProcessAsync(Guid subscriptionId)
    {
        logger?.LogInformation("Processing activation for subscription: {0}", subscriptionId);

        // Retrieve subscription from repository
        var subscription = subscriptionsRepository.GetSubscriptionByMicrosoftId(subscriptionId.ToString());
        if (subscription == null)
        {
            logger?.LogWarning("Subscription not found: {0}", subscriptionId);
            return;
        }

        // Skip if subscription is not in PendingActivation state
        if (!string.Equals(subscription.SubscriptionStatus, SubscriptionStatusEnum.PendingActivation.ToString(), StringComparison.OrdinalIgnoreCase))
        {
            logger?.LogInformation("Subscription is not in PendingActivation state: {0}", subscription.SubscriptionStatus);
            return;
        }

        try
        {
            // Fetch subscription from Marketplace SDK
            var sdkSubscription = await fulfillmentApiService.GetSubscriptionByIdAsync(subscriptionId);

            // Attempt activation via Fulfillment API
            var activationResult = await fulfillmentApiService.ActivateSubscriptionAsync(sdkSubscription);

            if (!activationResult.IsError)
            {
                // Mark subscription as Subscribed and active
                subscriptionsRepository.UpdateSubscriptionStatus(
                    subscriptionId.ToString(),
                    SubscriptionStatusEnum.Subscribed.ToString(),
                    true
                );

                // Fetch updated subscription details
                var updatedSubscription = await fulfillmentApiService.GetSubscriptionByIdAsync(subscriptionId);

                // Update license with new expiration and quantity
                var license = GetLicenseBySubscriptionId(subscriptionId);
                if (license != null)
                {
                    license.Status = 2;
                    license.LicenseExpires = updatedSubscription.Term?.EndDate?.ToString("yyyy-MM-dd");
                    license.PurchasedLicenses = updatedSubscription.Quantity ?? 0;
                    licensesRepository.UpdateLicense(license);
                }

                logger?.LogInformation("Subscription activated successfully: {0}", subscriptionId);
            }
            else
            {
                // Mark subscription as ActivationFailed
                subscriptionsRepository.UpdateSubscriptionStatus(
                    subscriptionId.ToString(),
                    SubscriptionStatusEnum.ActivationFailed.ToString(),
                    false
                );

                logger?.LogError("Subscription activation failed: {0}", subscriptionId);
            }
        }
        catch (Exception ex)
        {
            // Log exception and mark subscription as ActivationFailed
            logger?.LogError(ex, "Error activating subscription {SubscriptionId}", subscriptionId);

            subscriptionsRepository.UpdateSubscriptionStatus(
                subscriptionId.ToString(),
                SubscriptionStatusEnum.ActivationFailed.ToString(),
                false
            );
        }
    }
}
