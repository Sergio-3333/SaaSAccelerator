using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using System.Threading.Tasks;

public class WebhookProcessor : IWebhookProcessor
{
    private readonly IWebhookHandler webhookHandler;
    private readonly IFulfillmentApiService apiClient;

    public WebhookProcessor(
        IFulfillmentApiService apiClient,
        IWebhookHandler webhookHandler)
    {
        this.apiClient = apiClient;
        this.webhookHandler = webhookHandler;
    }

    public async Task ProcessWebhookNotificationAsync(WebhookPayload payload, SaaSApiClientConfiguration config)
    {
        switch (payload.Action)
        {
            case WebhookAction.Unsubscribe:
                await this.webhookHandler.UnsubscribedAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.ChangePlan:
                await this.webhookHandler.ChangePlanAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.ChangeQuantity:
                await this.webhookHandler.ChangeQuantityAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.Suspend:
                await this.webhookHandler.SuspendedAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.Reinstate:
                await this.webhookHandler.ReinstatedAsync(payload).ConfigureAwait(false);
                break;

            case WebhookAction.Renew:
                await this.webhookHandler.RenewedAsync().ConfigureAwait(false);
                break;

            default:
                await this.webhookHandler.UnknownActionAsync(payload).ConfigureAwait(false);
                break;
        }
    }
}
