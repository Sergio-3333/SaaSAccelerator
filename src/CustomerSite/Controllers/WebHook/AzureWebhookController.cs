using System;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers.WebHook;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
public class AzureWebhookController : ControllerBase
{

    private readonly IWebhookProcessor webhookProcessor;
    private readonly ValidateJwtToken validateJwtToken;
    private readonly SaaSApiClientConfiguration configuration;

    public AzureWebhookController(

        IWebhookProcessor webhookProcessor,
        ValidateJwtToken validateJwtToken,
        SaaSApiClientConfiguration configuration)
    {
        this.webhookProcessor = webhookProcessor;
        this.validateJwtToken = validateJwtToken;
        this.configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Post(AzureWebHookPayLoad payload)
    {
        try
        {
            if (Request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    var token = Request.Headers["Authorization"].ToString().Split(' ')[1];
                    await validateJwtToken.ValidateTokenAsync(token);
                }
                catch (Exception e)
                {
                    return Unauthorized($"JWT validation failed: {e.Message}");
                }
            }

            if (payload == null)
            {
                return BadRequest("Request payload is null.");
            }

            var json = JsonSerializer.Serialize(payload);
            Console.WriteLine($"Webhook received: {json}");

            await webhookProcessor.ProcessWebhookNotificationAsync(payload, configuration);
            return Ok();
        }
        catch (MarketplaceException ex)
        {
            Console.WriteLine($"Marketplace exception: {ex.Message}");
            return BadRequest($"Marketplace exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }
}
