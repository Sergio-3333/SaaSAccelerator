using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// Web Hook Handler Interface
/// </summary>
public interface IWebhookHandler
{

    /// <summary>
    /// Handles subscription creation (Subscribe).
    /// </summary>
    /// <param name="payload">The webhook payload.</param>
    /// <returns>Subscribed Async</returns>
    Task SubscribedAsync(AzureWebHookPayLoad payload);


    /// <summary>
    /// Changes the quantity asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Change Quantity Async</returns>
    Task ChangeQuantityAsync(AzureWebHookPayLoad payload);

    /// <summary>
    /// Renewed subscription state.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Renewed Async</returns>
    Task RenewedAsync(AzureWebHookPayLoad payload);

    /// <summary>
    /// Suspended the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Suspended Async</returns>
    Task SuspendedAsync(AzureWebHookPayLoad payload);

    /// <summary>
    /// Reinstated subscription state (after suspension).
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Reinstated Async</returns>
    Task ReinstatedAsync(AzureWebHookPayLoad payload);

    /// <summary>
    /// Unsubscribed the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Unsubscribed Async</returns>
    Task UnsubscribedAsync(AzureWebHookPayLoad payload);

    /// <summary>
    /// Unknown action fallback.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Unknown Action Async</returns>
    Task UnknownActionAsync(AzureWebHookPayLoad payload);


}