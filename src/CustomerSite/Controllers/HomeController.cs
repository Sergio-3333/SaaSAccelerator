// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Marketplace.SaaS.Accelerator.DataAccess;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers;

/// <summary>Home Controller.</summary>
/// <seealso cref="BaseController"/>
public class HomeController : Controller
{
    private readonly IFulfillmentApiService apiService;

    private readonly ILogger<HomeController> logger;

    private readonly LicenseService licenseService;
    private readonly SubLinesService subLinesService;
    private readonly SubscriptionService subscriptionService;
    private readonly ClientsService clientsService;


    public HomeController(
         IFulfillmentApiService apiService,
         SubscriptionService subscriptionService,
         LicenseService licenseService,
         SubLinesService subLinesService,
         ClientsService clientsService,
         ILogger<HomeController> logger)
    {
        this.apiService = apiService;
        this.subscriptionService = subscriptionService;
        this.licenseService = licenseService;
        this.subLinesService = subLinesService;
        this.clientsService = clientsService;
        this.logger = logger;
    }


    private async Task<string> GetTokenAsync(string tenantId, string clientId, string clientSecret, string resource)
    {
        using var client = new HttpClient();
        var body = new Dictionary<string, string>
    {
        { "grant_type", "client_credentials" },
        { "client_id", clientId },
        { "client_secret", clientSecret },
        { "scope", $"{resource}/.default" }
    };

        var res = await client.PostAsync(
            $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",
            new FormUrlEncodedContent(body)
        );

        res.EnsureSuccessStatusCode();
        var json = await res.Content.ReadAsStringAsync();
        return JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
    }



    public async Task<SubscriptionInputModel> GetSubscriptionInputModelAsync(Guid subscriptionId,
    string tenantId, string clientId, string clientSecret)
    {
        var model = new SubscriptionInputModel();

        // === 1. Fulfillment ===
        var fulfillmentToken = await GetTokenAsync(tenantId, clientId, clientSecret, "https://marketplaceapi.microsoft.com");

        var resolved = await apiService.ResolveAsync(fulfillmentToken);
        if (resolved == null || resolved.SubscriptionId == default)
            return null;

        var details = await apiService.GetSubscriptionByIdAsync(resolved.SubscriptionId);

        // Rellenar datos Fulfillment
        model.MicrosoftId = resolved.SubscriptionId.ToString();
        model.PurchaserTenantId = details.Purchaser?.TenantId?.ToString();
        model.AMPPlanId = details.PlanId;
        model.Status = details.SaasSubscriptionStatus.ToString();
        model.StartDate = details.Term?.StartDate?.UtcDateTime;
        model.EndDate = details.Term?.EndDate?.UtcDateTime;
        model.UsersQ = details.Quantity ?? 0;
        model.PurchaserEmail = details.Purchaser?.EmailId;
        model.AutoRenew = details.AutoRenew;
        model.IsActive = true;
        model.UserId = details.PublisherId.GetHashCode();
        model.Term = details.Term?.TermUnit.ToString();
        model.AMPlan = details.Name;
        model.LicenseId = licenseService.GenerateUniqueLicenseId();

        // === 2. Billing ===
        var billingToken = await GetTokenAsync(tenantId, clientId, clientSecret, "https://api.partnercenter.microsoft.com");

        // URL fija: unbilled recon, periodo actual, solo Marketplace
        var billingUrl = "https://api.partnercenter.microsoft.com/v1/invoices/unbilled/lineitems?provider=Marketplace&period=current";

        using (var http = new HttpClient())
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", billingToken);

            var billingRes = await http.GetAsync(billingUrl);
            if (billingRes.IsSuccessStatusCode)
            {
                var billingJson = await billingRes.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(billingJson);

                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    var subId = item.GetProperty("subscriptionId").GetString();
                    if (string.Equals(subId, model.MicrosoftId, StringComparison.OrdinalIgnoreCase))
                    {
                        if (item.TryGetProperty("currencyCode", out var currency))
                            model.Currency = currency.GetString();
                        if (item.TryGetProperty("afterTaxTotal", out var amount) && amount.ValueKind == JsonValueKind.Number)
                            model.Amount = amount.GetDecimal();
                        if (item.TryGetProperty("chargeStartDate", out var chargeDate))
                        {
                            if (chargeDate.ValueKind == JsonValueKind.String &&
                                DateTime.TryParse(chargeDate.GetString(), out var parsed))
                            {
                                model.ChargeDate = parsed;
                            }
                        }
                        break;
                    }
                }
            }
        }


        // === 3. Graph ===
        var graphToken = await GetTokenAsync(tenantId, clientId, clientSecret, "https://graph.microsoft.com");

        using (var http = new HttpClient())
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", graphToken);

            var graphUrl = $"https://graph.microsoft.com/v1.0/organization/{model.PurchaserTenantId}";
            var graphRes = await http.GetAsync(graphUrl);
            if (graphRes.IsSuccessStatusCode)
            {
                var graphJson = await graphRes.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(graphJson);

                // Graph devuelve un array en "value"
                var org = doc.RootElement.GetProperty("value")[0];
                if (org.TryGetProperty("displayName", out var displayName))
                {
                    model.Name = displayName.GetString();
                    model.Company = displayName.GetString();
                }
                if (org.TryGetProperty("country", out var country))
                    model.Country = country.GetString();
                if (org.TryGetProperty("email", out var email))
                    model.Email = email.GetString();
                if (org.TryGetProperty("city", out var city))
                    model.City = city.GetString();
                if (org.TryGetProperty("businessPhones", out var phones) && phones.GetArrayLength() > 0)
                    model.Phone = phones[0].GetString();
            }
        }

        return model;
    }




    public async Task<IActionResult> Index(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return View("Error", "Token vacío.");

        token = token.Replace(' ', '+');

        // 1) Resolver suscripción y obtener datos completos
        var resolved = await apiService.ResolveAsync(token);
        if (resolved == null || resolved.SubscriptionId == default)
            return View("Error", "No se pudo resolver la suscripción.");

        var model = await GetSubscriptionInputModelAsync(
            resolved.SubscriptionId,
            "<tenantId>", "<clientId>", "<clientSecret>"
        );

        // 3) Crear suscripción y sublines
        subscriptionService.CreateSubscription(model);

        // 4) Crear o actualizar licencia y obtener LicenseId
        licenseService.SaveLicenseFromInputModel(model);

        // 5) Crear SubLines
        subLinesService.CreateFromDataModel(model);

        // 6) Crear o actualizar cliente
        clientsService.CreateOrUpdateClientFromSubscription(model);


        // 7) Confirmar
        return View("Confirmacion", model);
    }




    public IActionResult Landing(SubscriptionInputModel model)
    {
        logger.LogInformation($"Landing post-compra para Tenant: {model.PurchaserTenantId}");
        return Redirect("https://www.anttext.com/");
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionDetail = HttpContext.Features.Get<IExceptionHandlerFeature>();
        return View(exceptionDetail?.Error);
    }


}