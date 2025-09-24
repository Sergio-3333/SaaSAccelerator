using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
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
    public async Task ProcessWebhookNotificationAsync(SubscriptionInputModel model, SaaSApiClientConfiguration config)
    {
        if (model == null)
        {
            logger.LogWarning("Webhook model is null. Ignoring request.");
            return;
        }

        logger.LogInformation(
            "Processing webhook: Action={Action}, MicrosoftId={MicrosoftId}, Plan={AMPlan}",
            model.Action, model.MicrosoftId, model.AMPlan);

        try
        {
            switch (model.Action)
            {
                case WebhookAction.Unsubscribe:
                    await webhookHandler.UnsubscribedAsync(model);
                    break;

                case WebhookAction.ChangeQuantity:
                    await webhookHandler.ChangeQuantityAsync(model);
                    break;

                case WebhookAction.Suspend:
                    await webhookHandler.SuspendedAsync(model);
                    break;

                case WebhookAction.Renew:
                    await webhookHandler.RenewedAsync(model);
                    break;

                case WebhookAction.Transfer:
                    await webhookHandler.UnknownActionAsync(model); // Transfer no manejado explícitamente
                    break;

                default:
                    logger.LogWarning("Unknown webhook action: {Action}", model.Action);
                    await webhookHandler.UnknownActionAsync(model);
                    break;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Error processing webhook for MicrosoftId={MicrosoftId}",
                model.MicrosoftId);
            throw;
        }
    }
}
