using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class WebhookProcessor : IWebhookProcessor
{
    private readonly IWebhookHandler webhookHandler;
    private readonly IFulfillmentApiService apiClient;
    private readonly ILogger<WebhookProcessor> logger;

    public WebhookProcessor(
        IFulfillmentApiService apiClient,
        IWebhookHandler webhookHandler,
        ILogger<WebhookProcessor> logger)
    {
        this.apiClient = apiClient;
        this.webhookHandler = webhookHandler;
        this.logger = logger;
    }

    /// <summary>
    /// Procesa las notificaciones entrantes del webhook y las envía al handler correspondiente.
    /// </summary>
    public async Task ProcessWebhookNotificationAsync(AzureWebHookPayLoad payload, SaaSApiClientConfiguration config)
    {
        if (payload == null)
        {
            logger.LogWarning("Webhook model is null. Ignoring request.");
            return;
        }

        logger.LogInformation(
            "Processing webhook: Action={Action}, MicrosoftId={MicrosoftId}, Plan={AMPlan}",
            payload.Action, payload.SubscriptionId, payload.PlanId);

        try
        {
            switch (payload.Action)
            {
                case "Subscribe":
                    await webhookHandler.SubscribedAsync(payload);
                    break;
                case "Unsubscribe":
                    await webhookHandler.UnsubscribedAsync(payload);
                    break;
                case "ChangeQuantity":
                    await webhookHandler.ChangeQuantityAsync(payload);
                    break;
                case "Suspend":
                    await webhookHandler.SuspendedAsync(payload);
                    break;
                case "Reinstate":
                    await webhookHandler.ReinstatedAsync(payload);
                    break;
                case "Renew":
                    await webhookHandler.RenewedAsync(payload);
                    break;
                default:
                    logger.LogWarning("Unknown webhook action: {Action}", payload.Action);
                    await webhookHandler.UnknownActionAsync(payload);
                    break;
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error processing webhook for MicrosoftId={MicrosoftId}",
                payload.SubscriptionId);
            throw;
        }
    }
}
