using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess;

namespace Marketplace.SaaS.Accelerator.Services.Services;

/// <summary>
/// Servicio para obtener datos de Microsoft Graph y enriquecer un SubscriptionInputModel.
/// </summary>
public class GraphService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GraphService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    /// Enriquecer un SubscriptionInputModel con datos del usuario principal del tenant.
    public async Task<SubscriptionInputModel> EnrichWithGraphAsync(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PurchaserTenantId))
            throw new ArgumentException("El TenantId no puede estar vacío para consultar Graph.");

        var token = await GetTokenAsync(model.PurchaserTenantId);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/users");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        dynamic users = JsonConvert.DeserializeObject(json);

        var primaryUser = ((IEnumerable<dynamic>)users.value)
            .FirstOrDefault(u => !string.IsNullOrEmpty((string)u.mail));

        if (primaryUser != null)
        {
            model.Name ??= primaryUser.displayName;
            model.PurchaserEmail ??= primaryUser.mail;
            model.Email ??= primaryUser.userPrincipalName;
            model.Phone ??= primaryUser.mobilePhone;
            model.Company ??= primaryUser.companyName;
            model.City ??= primaryUser.officeLocation;
        }

        return model;
    }


    /// <summary>
    /// Obtiene un token de acceso para Microsoft Graph usando client_credentials.
    /// </summary>
    private async Task<string> GetTokenAsync(string tenantId)
    {
        var values = new Dictionary<string, string>
        {
            { "client_id", _config["Graph:ClientId"] },
            { "client_secret", _config["Graph:ClientSecret"] },
            { "scope", "https://graph.microsoft.com/.default" },
            { "grant_type", "client_credentials" }
        };

        var response = await _httpClient.PostAsync(
            $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token",
            new FormUrlEncodedContent(values)
        );

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<dynamic>(json);
        return token.access_token;
    }
}
