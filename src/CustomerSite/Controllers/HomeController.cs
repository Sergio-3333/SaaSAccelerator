using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Marketplace.SaaS.Accelerator.DataAccess;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Marketplace.SaaS.Models;
using Marketplace.SaaS.Accelerator.Services.Configurations;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers;

/// <summary>Home Controller.</summary>
public class HomeController : Controller
{
    private readonly IFulfillmentApiService apiService;
    private readonly ILogger<HomeController> logger;
    private readonly LicenseService licenseService;
    private readonly SubLinesService subLinesService;
    private readonly SubscriptionService subscriptionService;
    private readonly ClientsService clientsService;
    private readonly SubscriptionsRepository subscriptionsRepository;
    private readonly SaaSApiClientConfiguration _config;


    public HomeController(
         IFulfillmentApiService apiService,
         SubscriptionService subscriptionService,
         LicenseService licenseService,
         SubLinesService subLinesService,
         ClientsService clientsService,
         SubscriptionsRepository subscriptionsRepository,
         ILogger<HomeController> logger,
         SaaSApiClientConfiguration _config)
    {
        this.apiService = apiService;
        this.subscriptionService = subscriptionService;
        this.licenseService = licenseService;
        this.subLinesService = subLinesService;
        this.clientsService = clientsService;
        this.subscriptionsRepository = subscriptionsRepository;
        this.logger = logger;
        this._config = _config;
    }

    private async Task<string> GetTokenAsync(string tenantId, string clientId, string clientSecret, string resourceBase)
    {
        using var client = new HttpClient();
        var body = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", $"{resourceBase}/.default" }
        };

        var res = await client.PostAsync(
            $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",
            new FormUrlEncodedContent(body)
        );

        var raw = await res.Content.ReadAsStringAsync();

        Console.WriteLine($"[DEBUG] Respuesta de Azure AD para {resourceBase}: {res.StatusCode}\n{raw}");
        logger.LogInformation($"Respuesta de Azure AD para {resourceBase}: {res.StatusCode}\n{raw}");

        if (!res.IsSuccessStatusCode)
        {
            throw new Exception($"No se pudo obtener token para {resourceBase}. Status: {res.StatusCode}\n{raw}");
        }

        try
        {
            using var doc = JsonDocument.Parse(raw);

            if (doc.RootElement.TryGetProperty("access_token", out var token))
            {
                return token.GetString();
            }
            else if (doc.RootElement.TryGetProperty("error", out var error))
            {
                var desc = doc.RootElement.GetProperty("error_description").GetString();
                logger.LogError($"Error de AAD al pedir token para {resourceBase}: {error.GetString()} - {desc}");
                throw new Exception($"Error de AAD: {error.GetString()} - {desc}");
            }
            else
            {
                logger.LogError($"Respuesta inesperada al pedir token para {resourceBase}: {raw}");
                throw new Exception("Respuesta inesperada de Azure AD");
            }
        }
        catch (JsonException)
        {
            logger.LogError($"Respuesta no JSON al pedir token para {resourceBase}: {raw}");
            throw;
        }
    }

    public async Task<SubscriptionInputModel> GetSubscriptionInputModelAsync(
        Guid subscriptionId,
        string tenantId, string clientId, string clientSecret)
    {
        var model = new SubscriptionInputModel();

        // === 1. Fulfillment ===
        var details = await apiService.GetSubscriptionByIdAsync(subscriptionId);

        if (details == null)
            return null;

        var fulfillmentJson = JsonSerializer.Serialize(details, new JsonSerializerOptions { WriteIndented = true });
        logger.LogInformation($"[Fulfillment API] JSON recibido:\n{fulfillmentJson}");

        model.MicrosoftId = subscriptionId.ToString();
        model.PurchaserTenantId = details.Purchaser?.TenantId?.ToString();
        model.AMPPlanId = details.PlanId;
        model.Status = details.SaasSubscriptionStatus.ToString();
        model.StartDate = DateTime.Now;
        model.EndDate = details.Term.EndDate?.UtcDateTime;
        model.UsersQ = details.Quantity ?? 0;
        model.PurchaserEmail = details.Purchaser?.EmailId;
        model.AutoRenew = details.AutoRenew;
        model.IsActive = true;
        model.UserId = details.PublisherId;
        model.Term = details.Term?.TermUnit.ToString();
        model.AMPlan = details.Name;

        

        // === 2. Graph ===
        var graphToken = await GetTokenAsync(tenantId, clientId, clientSecret, "https://graph.microsoft.com");

        using (var http = new HttpClient())
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", graphToken);

            var graphUrl = $"https://graph.microsoft.com/v1.0/organization/{model.PurchaserTenantId}";
            var graphRes = await http.GetAsync(graphUrl);
            var graphJson = await graphRes.Content.ReadAsStringAsync();
            logger.LogInformation($"[Graph API] JSON recibido:\n{graphJson}");

            if (graphRes.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(graphJson);

                JsonElement org;

                // Si la respuesta tiene "value" (array), úsalo
                if (doc.RootElement.TryGetProperty("value", out var value) && value.ValueKind == JsonValueKind.Array && value.GetArrayLength() > 0)
                {
                    org = value[0];
                }
                else
                {
                    // Si no, la respuesta es un objeto único
                    org = doc.RootElement;
                }

                if (org.TryGetProperty("displayName", out var displayName))
                {
                    model.Company = displayName.GetString();
                }

                if (org.TryGetProperty("state", out var country))
                    model.Country = country.GetString();
                if (org.TryGetProperty("city", out var city))
                    model.City = city.GetString();
                if (org.TryGetProperty("street", out var street))
                    model.Adr1 = street.GetString();
                if (org.TryGetProperty("postalCode", out var ZIP))
                    model.Zip = ZIP.GetString();
            }
            else
            {
                logger.LogWarning($"Error al llamar a Graph API: {graphRes.StatusCode}\n{graphJson}");
            }


            // === 3. Graph DisplayName ===
            var userUrl = $"https://graph.microsoft.com/v1.0/users/{details.Purchaser.ObjectId}";
  

            var userRes = await http.GetAsync(userUrl);
            var userJson = await userRes.Content.ReadAsStringAsync();
            logger.LogInformation($"[Graph API /users] JSON recibido:\n{userJson}");

            if (userRes.IsSuccessStatusCode)
            {
                using var userDoc = JsonDocument.Parse(userJson);
                var userRoot = userDoc.RootElement;

                if (userRoot.TryGetProperty("displayName", out var displayName))
                {
                    model.Name = displayName.GetString(); 
                }

                if (userRoot.TryGetProperty("mobilePhone", out var mobile))
                {
                    model.Mobile = mobile.GetString(); 
                }

                if (userRoot.TryGetProperty("businessPhones", out var phones) && phones.ValueKind == JsonValueKind.Array && phones.GetArrayLength() > 0)
                    model.Phone = phones[0].GetString();
            }
            else
            {
                logger.LogWarning($"Error al llamar a Graph API /users: {userRes.StatusCode}\n{userJson}");
            }


        }



        return model;
    }



    public async Task<IActionResult> Index(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Content("Error: token vacío");

        logger.LogInformation($"Token recibido: {token}");

        // 1. Resolver suscripción con el token
        ResolvedSubscription resolved = await apiService.ResolveAsync(token);
        if (resolved?.Id == null || resolved.Id == Guid.Empty)
            return Content("Error: no se pudo resolver la suscripción");

        logger.LogInformation($"SubscriptionId resuelto: {resolved.Id}");

        // 2. Obtener datos completos desde Fulfillment API
        var model = await GetSubscriptionInputModelAsync(
            resolved.Id.Value,
            _config.TenantId,
            _config.ClientId,
            _config.ClientSecret
        );

        if (model == null)
            return Content("Error: no se pudo obtener el modelo de suscripción");

        // 3. Activar en Microsoft
        var subscriptionFromApi = await apiService.GetSubscriptionByIdAsync(resolved.Id.Value);
        await apiService.ActivateSubscriptionAsync(subscriptionFromApi);

        // 4. Guardar en BDD solo si no existe ya ese MicrosoftId
        if (!subscriptionsRepository.ExistsByMicrosoftId(model.MicrosoftId))
        {
            subscriptionService.CreateSubscription(model);
            subLinesService.CreateFromDataModel(model);
            licenseService.SaveLicenseFromInputModel(model);
            subscriptionsRepository.UpdateSubscription(resolved.Id.ToString(), s =>
            {
                s.SubStatus = "Active";
                s.Active = true;
            });

        }

        clientsService.CreateOrUpdateClientFromSubscription(model);

        // 5. Redirigir al sitio final
        return Redirect("https://www.anttext.com/");
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionDetail = HttpContext.Features.Get<IExceptionHandlerFeature>();
        return View(exceptionDetail?.Error);
    }
}