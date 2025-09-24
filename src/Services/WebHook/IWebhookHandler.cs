// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess;

namespace Marketplace.SaaS.Accelerator.Services.WebHook;

/// <summary>
/// Web Hook Handler Interface
/// </summary>
public interface IWebhookHandler
{

    /// <summary>
    /// Changes the quantity asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Change QuantityAsync</returns>
    Task ChangeQuantityAsync(SubscriptionInputModel model);


    /// <summary>
    /// Renewed subscription state
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Renewed Async</returns>
    Task RenewedAsync(SubscriptionInputModel model);

    /// <summary>
    /// Suspended the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Suspended Async</returns>
    Task SuspendedAsync(SubscriptionInputModel model);

    /// <summary>
    /// Unsubscribed the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Unsubscribed Async</returns>
    Task UnsubscribedAsync(SubscriptionInputModel model);

    /// <summary>
    /// Unknowstate the asynchronous.
    /// </summary>
    /// <param name="payload">The payload.</param>
    /// <returns>Unsubscribed Async</returns>
    Task UnknownActionAsync(SubscriptionInputModel model);

}