using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;

using Microsoft.Marketplace.SaaS;
using Microsoft.Marketplace.SaaS.Models;

using Newtonsoft.Json;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class FulfillmentApiService : IFulfillmentApiService
{
    private readonly IMarketplaceSaaSClient marketplaceClient;
    private readonly SaaSApiClientConfiguration clientConfiguration;
    private readonly HttpClient _httpClient;


    public FulfillmentApiService(IMarketplaceSaaSClient marketplaceClient, SaaSApiClientConfiguration config, HttpClient httpClient)
    {
        this.marketplaceClient = marketplaceClient;
        this.clientConfiguration = config;
        this._httpClient = httpClient;
    }


    public async Task<Subscription> GetSubscriptionByIdAsync(Guid subscriptionId)
    {
        var subscription = (await marketplaceClient.Fulfillment.GetSubscriptionAsync(subscriptionId)).Value;
        return subscription;
    }

    public Subscription GetSubscriptionById(Guid subscriptionId)
    {
        return marketplaceClient.Fulfillment.GetSubscription(subscriptionId).Value;
    }

    public async Task<Response> ActivateSubscriptionAsync(Subscriptions subscription)
    {
        var payload = new SubscriberPlan
        {
            PlanId = subscription.AMPPlanId,
            Quantity = subscription.AMPQuantity,
        };

        var subscriptionId = Guid.Parse(subscription.MicrosoftId);

        return await marketplaceClient.Fulfillment.ActivateSubscriptionAsync(subscriptionId, payload);
    }


    public async Task<Response> PatchOperationStatusResultAsync(Guid subscriptionId, Guid operationId, UpdateOperationStatusEnum updateOperationStatus)
    {
        var update = new UpdateOperation { Status = updateOperationStatus };
        return await marketplaceClient.Operations.UpdateOperationStatusAsync(subscriptionId, operationId, update);
    }

    public async Task<List<Subscription>> GetAllSubscriptionAsync()
    {
        var subscriptions = await marketplaceClient.Fulfillment.ListSubscriptionsAsync().ToListAsync();
        return subscriptions;
    }

    public async Task<ResolvedSubscriptionResult> ResolveAsync(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://marketplaceapi.microsoft.com/api/saas/subscriptions/resolve");
        request.Headers.Add("x-ms-marketplace-token", token);
        request.Headers.Add("Accept", "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var resolved = JsonConvert.DeserializeObject<ResolvedSubscriptionResult>(json);

        return resolved;
    }


    public string GetSaaSAppURL()
    {
        return clientConfiguration.SaaSAppUrl;
    }
}
