// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

static class ConversionHelper
{

    public static SubscriptionResult subscriptionResult(this Subscription subscription)
    {
        var subscriptionResult = new SubscriptionResult()
        {
            Id = subscription.Id ?? throw new MarketplaceException("Subscription Id cannot be null"),
            PublisherId = subscription.PublisherId,
            OfferId = subscription.OfferId,
            Name = subscription.Name,
            SaasSubscriptionStatus = (Models.SubscriptionStatusEnum)Enum.Parse(typeof(Models.SubscriptionStatusEnum), subscription.SaasSubscriptionStatus.ToString()),
            PlanId = subscription.PlanId,
            Quantity = subscription.Quantity ?? 0,
            Purchaser = new PurchaserResult()
            {
                EmailId = subscription.Purchaser.EmailId,
                ObjectId = subscription.Purchaser.ObjectId?.ToString() ?? throw new MarketplaceException("Purchaser ObjectId cannot be null"),
                TenantId = subscription.Purchaser.TenantId?.ToString() ?? throw new MarketplaceException("Purchaser Tenant Id cannot be null"),
            },
            Beneficiary = new BeneficiaryResult()
            {
                EmailId = subscription.Beneficiary.EmailId ?? throw new MarketplaceException("Beneficiary Email Id cannot be null"),
                ObjectId = subscription.Beneficiary.ObjectId ?? throw new MarketplaceException("Beneficiary Object Id cannot be null"),
                TenantId = subscription.Beneficiary.TenantId ?? throw new MarketplaceException("Beneficiary Tenant Id cannot be null"),
            },
            Term = new TermResult()
            {
                TermUnit = subscription.Term.TermUnit.HasValue ? (Models.TermUnitEnum)subscription.Term.TermUnit :         Models.TermUnitEnum.P1M,
                StartDate = subscription.Term.StartDate ?? default(DateTimeOffset),
                EndDate = subscription.Term.EndDate ?? default(DateTimeOffset),
            }
        };
        return subscriptionResult;
    }

    public static List<SubscriptionResult> subscriptionResultList(this List<Subscription> subscriptions) 
    {
        return subscriptions.Select(x => x.subscriptionResult()).ToList();
    }

}