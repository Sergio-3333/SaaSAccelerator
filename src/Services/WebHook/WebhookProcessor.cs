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

    // Processes incoming webhook notifications and routes them to the appropriate handler
    public async Task ProcessWebhookNotificationAsync(WebhookPayload payload, SaaSApiClientConfiguration config)
    {
        // Ignore null payloads
        if (payload == null)
        {
            logger.LogWarning("Webhook payload is null. Ignoring request.");
            return;
        }

        // Log basic payload metadata
        logger.LogInformation(
            "Processing webhook: Action={Action}, SubscriptionId={SubscriptionId}, OperationId={OperationId}",
            payload.Action, payload.SubscriptionId, payload.OperationId);

        try
        {
            // Route webhook to the correct handler based on action type
            switch (payload.Action)
            {
                case WebhookAction.Unsubscribe:
                    await webhookHandler.UnsubscribedAsync(payload);
                    break;

                case WebhookAction.ChangePlan:
                    await webhookHandler.ChangePlanAsync(payload);
                    break;

                case WebhookAction.ChangeQuantity:
                    await webhookHandler.ChangeQuantityAsync(payload);
                    break;

                case WebhookAction.Suspend:
                    await webhookHandler.SuspendedAsync(payload);
                    break;

                case WebhookAction.Reinstate:
                    await webhookHandler.ReinstatedAsync(payload);
                    break;

                case WebhookAction.Renew:
                    await webhookHandler.RenewedAsync(payload);
                    break;

                case WebhookAction.Transfer:
                    await webhookHandler.UnknownActionAsync(payload); // Transfer not explicitly handled
                    break;

                default:
                    // Log and route unknown actions to fallback handler
                    logger.LogWarning("Unknown webhook action: {Action}", payload.Action);
                    await webhookHandler.UnknownActionAsync(payload);
                    break;
            }
        }
        catch (Exception ex)
        {
            // Log error and propagate exception
            logger.LogError(ex,
                "Error processing webhook for SubscriptionId={SubscriptionId}",
                payload.SubscriptionId);
            throw;
        }
    }
}
