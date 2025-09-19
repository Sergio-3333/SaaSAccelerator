using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

// Interface for interacting with the Microsoft SaaS Fulfillment API.
// Defines methods for managing subscriptions, resolving tokens, and updating operation status.

public interface IFulfillmentApiService
{
    // Constants for content types and marketplace token header.
    const string CONTENTTYPEURLENCODED = "application/x-www-form-urlencoded";
    const string CONTENTTYPEAPPLICATIONJSON = "application/json";
    const string MARKETPLACETOKEN = "x-ms-marketplace-token";

    // Retrieves a subscription asynchronously by its ID.
    Task<Subscription> GetSubscriptionByIdAsync(Guid subscriptionId);

    // Retrieves a subscription synchronously by its ID.
    Subscription GetSubscriptionById(Guid subscriptionId);

    // Activates a subscription via the fulfillment API.
    Task<Response> ActivateSubscriptionAsync(Subscription subscription);

    // Updates the status of an operation (e.g., Activate, Unsubscribe).
    Task<Response> PatchOperationStatusResultAsync(Guid subscriptionId, Guid operationId, UpdateOperationStatusEnum updateOperationStatus);

    // Retrieves all subscriptions asynchronously.
    Task<List<Subscription>> GetAllSubscriptionAsync();

    // Cancels a subscription by its ID.
    Task CancelSubscriptionAsync(Guid subscriptionId);

    // Resolves a subscription from a marketplace token.
    Task<ResolvedSubscriptionResult> ResolveAsync(string token);

    // Returns the SaaS application URL used in fulfillment flows.
    string GetSaaSAppURL();
}
