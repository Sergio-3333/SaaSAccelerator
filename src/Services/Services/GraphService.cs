using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;


public class GraphService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public GraphService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<GraphUser> GetPrimaryUserAsync(string tenantId)
    {
        var token = await GetTokenAsync(tenantId);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/users");
        var json = await response.Content.ReadAsStringAsync();
        var users = JsonConvert.DeserializeObject<GraphUserList>(json);

        return users.value.FirstOrDefault(u => !string.IsNullOrEmpty(u.mail));
    }

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

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<dynamic>(json);
        return token.access_token;
    }
}
