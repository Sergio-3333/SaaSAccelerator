
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// Web hook Processor Interface
/// </summary>
public interface IWebhookProcessor
{
    /// <summary>
    /// Processes the Web hook notification asynchronous.
    /// </summary>
    /// <param name="details">The details.</param>
    /// <param name="config"></param>
    /// <returns>Processes the Web hook notification</returns>
    Task ProcessWebhookNotificationAsync(AzureWebHookPayLoad payload, SaaSApiClientConfiguration config);
}