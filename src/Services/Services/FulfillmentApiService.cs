using Azure;
using Marketplace.SaaS.Accelerator.Services.Configurations;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.Marketplace.SaaS;
using Microsoft.Marketplace.SaaS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

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

    // Retrieves a subscription asynchronously using its ID via the Fulfillment API
    public async Task<Subscription> GetSubscriptionByIdAsync(Guid subscriptionId)
    {
        var subscription = (await marketplaceClient.Fulfillment.GetSubscriptionAsync(subscriptionId)).Value;
        return subscription;
    }

    // Retrieves a subscription synchronously using its ID via the Fulfillment API
    public Subscription GetSubscriptionById(Guid subscriptionId)
    {
        return marketplaceClient.Fulfillment.GetSubscription(subscriptionId).Value;
    }

    // Activates a subscription by sending the selected plan to the Fulfillment API
    public async Task<Response> ActivateSubscriptionAsync(Microsoft.Marketplace.SaaS.Models.Subscription subscription)
    {
        var body = new SubscriberPlan
        {
            PlanId = subscription.PlanId
        };

        return await marketplaceClient.Fulfillment.ActivateSubscriptionAsync(
            subscriptionId: subscription.Id.Value,
            body: body
        );
    }

    // Cancels a subscription by calling the Fulfillment API's delete endpoint
    public async Task CancelSubscriptionAsync(Guid subscriptionId)
    {
        await marketplaceClient.Fulfillment.DeleteSubscriptionAsync(subscriptionId);
    }

    // Updates the status of an operation (e.g. activation, cancellation) for a given subscription
    public async Task<Response> PatchOperationStatusResultAsync(Guid subscriptionId, Guid operationId, UpdateOperationStatusEnum updateOperationStatus)
    {
        var update = new UpdateOperation { Status = updateOperationStatus };
        return await marketplaceClient.Operations.UpdateOperationStatusAsync(subscriptionId, operationId, update);
    }

    // Retrieves all subscriptions associated with the current SaaS account
    public async Task<List<Subscription>> GetAllSubscriptionAsync()
    {
        var subscriptions = await marketplaceClient.Fulfillment.ListSubscriptionsAsync().ToListAsync();
        return subscriptions;
    }

    // Resolves a subscription from a marketplace token (used during initial onboarding)
    public async Task<ResolvedSubscription> ResolveAsync(string token)
    {
        var result = await marketplaceClient.Fulfillment.ResolveAsync(token);
        return result.Value;
    }


    // Returns the configured SaaS application URL
    public string GetSaaSAppURL()
    {
        return clientConfiguration.SaaSAppUrl;
    }
}

