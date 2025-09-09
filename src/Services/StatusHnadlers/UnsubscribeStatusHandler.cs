using System;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
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
        ILogger<UnsubscribeStatusHandler> logger)
        : base(subscriptionsRepository, licensesRepository, clientsRepository)
    {
        this.fulfillmentApiService = fulfillApiService;
        this.logger = logger;
    }

    public override void Process(Guid subscriptionID)
    {
        ProcessAsync(subscriptionID).GetAwaiter().GetResult();
    }

    public async Task ProcessAsync(Guid subscriptionID)
    {
        logger?.LogInformation("Procesando Unsubscribe para suscripción: {0}", subscriptionID);

        var subscription = GetSubscriptionByMicrosoftId(subscriptionID);
        if (subscription == null)
        {
            logger?.LogWarning("Suscripción no encontrada: {0}", subscriptionID);
            return;
        }

        logger?.LogInformation("Suscripción encontrada con plan: {0}", JsonSerializer.Serialize(subscription.AMPPlanId));

        if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString())
        {
            try
            {
                await fulfillmentApiService.DeleteSubscriptionAsync(subscriptionID, subscription.AMPPlanId);

                subscriptionsRepository.UpdateStatus(
                    subscriptionID.ToString(),
                    SubscriptionStatusEnumExtension.Unsubscribed.ToString(),
                    false
                );

                logger?.LogInformation("Suscripción marcada como Unsubscribed: {0}", subscriptionID);
            }
            catch (Exception ex)
            {
                logger?.LogError("Error al desuscribir {0}: {1}", subscriptionID, ex.Message);

                subscriptionsRepository.UpdateStatus(
                    subscriptionID.ToString(),
                    SubscriptionStatusEnumExtension.UnsubscribeFailed.ToString(),
                    true
                );
            }
        }
        else
        {
            logger?.LogInformation("La suscripción no está en estado PendingUnsubscribe: {0}", subscription.SubscriptionStatus);
        }
    }
}
