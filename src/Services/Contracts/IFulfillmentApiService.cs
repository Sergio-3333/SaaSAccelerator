using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface IFulfillmentApiService
{
    const string CONTENTTYPEURLENCODED = "application/x-www-form-urlencoded";
    const string CONTENTTYPEAPPLICATIONJSON = "application/json";
    const string MARKETPLACETOKEN = "x-ms-marketplace-token";

    Task<Subscription> GetSubscriptionByIdAsync(Guid subscriptionId);
    Subscription GetSubscriptionById(Guid subscriptionId);

    Task<Response> ActivateSubscriptionAsync(Subscriptions subscription);

    Task<Response> PatchOperationStatusResultAsync(Guid subscriptionId, Guid operationId, UpdateOperationStatusEnum updateOperationStatus);

    Task<List<Subscription>> GetAllSubscriptionAsync();

    Task<bool> DeleteSubscriptionAsync(Guid subscriptionId, string planId);

    string GetSaaSAppURL();
}
