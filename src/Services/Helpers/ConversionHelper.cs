using System;
using Marketplace.SaaS.Accelerator.DataAccess;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.Services.Helpers;

// Converts Microsoft Subscription model into your internal SubscriptionInputModel.
public static class ConversionHelper
{
    public static SubscriptionInputModel ToInputModel(this Subscription subscription)
    {
        if (subscription == null)
            throw new ArgumentNullException(nameof(subscription));

        return new SubscriptionInputModel
        {
            MicrosoftId = subscription.Id?.ToString() ?? throw new Exception("Subscription Id is required"),
            Status = subscription.SaasSubscriptionStatus.ToString(),
            AMPPlanId = subscription.PlanId,
            IsActive = subscription.SaasSubscriptionStatus.ToString() switch
            {
                "Subscribed" => true,
                _ => false
            },
            PurchaserEmail = subscription.Purchaser?.EmailId,
            PurchaserTenantId = subscription.Purchaser?.TenantId?.ToString(),
            Term = subscription.Term?.TermUnit?.ToString(),
            StartDate = subscription.Term?.StartDate?.UtcDateTime,
            EndDate = subscription.Term?.EndDate?.UtcDateTime,
            AutoRenew = subscription.AutoRenew,
            ChargeDate = DateTime.UtcNow,
            AMPlan = subscription.PlanId,
            UsersQ = subscription.Quantity ?? 0,

        };
    }
}
