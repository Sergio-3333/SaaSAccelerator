using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers.WebHook;

[Route("api/[controller]")]
[ApiController]
[IgnoreAntiforgeryToken]
public class AzureWebhookController : ControllerBase
{
    private readonly IWebhookProcessor webhookProcessor;
    private readonly ValidateJwtToken validateJwtToken;
    private readonly SaaSApiClientConfiguration configuration;
    private readonly ILogger<AzureWebhookController> logger;

    public AzureWebhookController(
        IWebhookProcessor webhookProcessor,
        ValidateJwtToken validateJwtToken,
        SaaSApiClientConfiguration configuration,
        ILogger<AzureWebhookController> logger)
    {
        this.webhookProcessor = webhookProcessor;
        this.validateJwtToken = validateJwtToken;
        this.configuration = configuration;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post(AzureWebHookPayLoad payload)
    {
        try
        {
            foreach (var header in Request.Headers)
            {
                logger.LogInformation($"[WEBHOOK] Header: {header.Key} = {header.Value}");
            }

            if (Request.Headers.ContainsKey("Authorization"))
            {
                try
                {
                    var token = Request.Headers["Authorization"].ToString().Split(' ')[1];
                    await validateJwtToken.ValidateTokenAsync(token);
                    logger.LogInformation("[WEBHOOK] JWT token validated successfully.");
                }
                catch (Exception e)
                {
                    logger.LogError(e, "[WEBHOOK] JWT validation failed.");
                    return Unauthorized($"JWT validation failed: {e.Message}");
                }
            }

            if (payload == null)
            {
                logger.LogWarning("[WEBHOOK] Payload is null.");
                return BadRequest("Request payload is null.");
            }

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
            logger.LogInformation($"[WEBHOOK] Payload recibido:\n{json}");

            await webhookProcessor.ProcessWebhookNotificationAsync(payload, configuration);

            return Ok();
        }
        catch (MarketplaceException ex)
        {
            logger.LogError(ex, $"[WEBHOOK] Marketplace exception: {ex.Message}");
            return BadRequest($"Marketplace exception: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[WEBHOOK] Unexpected error: {ex.Message}");
            return StatusCode(500, $"Unexpected error: {ex.Message}");
        }
    }
}
