using System;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
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
        ILogger<PendingFulfillmentStatusHandler> logger)
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
        logger?.LogInformation("Procesando PendingFulfillment para suscripción: {0}", subscriptionID);

        var subscription = GetSubscriptionByMicrosoftId(subscriptionID);
        if (subscription == null)
        {
            logger?.LogWarning("Suscripción no encontrada: {0}", subscriptionID);
            return;
        }

        logger?.LogInformation("Suscripción encontrada con plan: {0}", JsonSerializer.Serialize(subscription.AMPPlanId));

        if (subscription.SubscriptionStatus == SubscriptionStatusEnumExtension.PendingFulfillmentStart.ToString())
        {
            try
            {
                subscriptionsRepository.UpdateStatus(
                    subscriptionID.ToString(),
                    SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                    true
                );

                logger?.LogInformation("Estado actualizado a PendingActivation para suscripción: {0}", subscriptionID);
            }
            catch (Exception ex)
            {
                logger?.LogError("Error al actualizar estado de suscripción {0}: {1}", subscriptionID, ex.Message);

                subscriptionsRepository.UpdateStatus(
                    subscriptionID.ToString(),
                    SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                    true
                );
            }
        }
        else
        {
            logger?.LogInformation("La suscripción no está en estado PendingFulfillmentStart: {0}", subscription.SubscriptionStatus);
        }
    }
}
