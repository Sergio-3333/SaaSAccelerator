using System;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionsRepository subscriptionRepository;
    private readonly IProductsRepository productRepository;
    private readonly int currentUserId;

    public SubscriptionService(ISubscriptionsRepository subscriptionRepo, IProductsRepository productRepository, int currentUserId = 0)
    {
        this.subscriptionRepository = subscriptionRepo;
        this.productRepository = productRepository;
        this.currentUserId = currentUserId;
    }

    public void UpdateStateOfSubscription(Guid subscriptionId, string status, bool isActivate)
    {
        subscriptionRepository.UpdateStatus(subscriptionId.ToString(), status, isActivate);
    }


    public SubscriptionResultExtension GetSubscriptionsBySubscriptionId(Guid subscriptionId, bool includeUnsubscribed = true)
    {
        var subscriptionDetail = subscriptionRepository.GetByMicrosoftId(subscriptionId.ToString());
        if (subscriptionDetail != null)
        {
            var subscritpionDetail = PrepareSubscriptionResponse(subscriptionDetail);
            if (subscritpionDetail != null)
            {
                return subscritpionDetail;
            }
        }
        return new SubscriptionResultExtension();
    }


    public SubscriptionResultExtension PrepareSubscriptionResponse(Subscriptions subscription, Products existingProductDetail = null)
    {
        if (existingProductDetail == null && int.TryParse(subscription.AMPPlanId, out var productId))
        {
            existingProductDetail = productRepository.Get(productId);
        }

        var subscritpionDetail = new SubscriptionResultExtension
        {
            Id = Guid.Parse(subscription.MicrosoftId),
            SubscribeId = subscription.Id,
            PlanId = string.IsNullOrEmpty(subscription.AMPPlanId) ? string.Empty : subscription.AMPPlanId,
            OfferId = subscription.AmpOfferId,
            Term = new TermResult
            {
                StartDate = subscription.StartDate.GetValueOrDefault(),
                EndDate = subscription.EndDate.GetValueOrDefault(),
            },
            Quantity = subscription.AMPQuantity,
            Name = existingProductDetail?.ProductName ?? subscription.Name,
            SubscriptionStatus = GetSubscriptionStatus(subscription.SubscriptionStatus),
            IsActiveSubscription = subscription.IsActive ?? false,
            CustomerEmailAddress = null,
            CustomerName = null,
            IsMeteringSupported = false
        };

        if (!Enum.TryParse<TermUnitEnum>(subscription.Term, out var termUnit))
            termUnit = TermUnitEnum.P1M;
        subscritpionDetail.Term.TermUnit = termUnit;

        subscritpionDetail.Purchaser = new PurchaserResult
        {
            EmailId = subscription.PurchaserEmail,
            TenantId = subscription.PurchaserTenantId ?? default
        };

        return subscritpionDetail;
    }

    public SubscriptionStatusEnumExtension GetSubscriptionStatus(string subscriptionStatus)
    {
        var parseSuccessfull = Enum.TryParse(subscriptionStatus, out SubscriptionStatusEnumExtension status);
        return parseSuccessfull ? status : SubscriptionStatusEnumExtension.UnRecognized;
    }
}
