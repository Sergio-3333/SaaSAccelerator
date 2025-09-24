using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using System;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionsRepository subscriptionRepository;
    private readonly ISubLinesService subLinesService;

    public SubscriptionService(
        ISubscriptionsRepository subscriptionRepo,
        ISubLinesService subLinesService)
    {
        this.subscriptionRepository = subscriptionRepo;
        this.subLinesService = subLinesService;
    }

    // Creates a new subscription and its corresponding billing line
    public void CreateSubscription(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MicrosoftId))
            throw new ArgumentException("MicrosoftId cannot be empty.");

        // Map input model to entity and persist subscription
        var entity = MapToEntity(model);
        subscriptionRepository.AddSubscription(entity);

    }

    // Updates an existing subscription with new status and plan
    public void UpdateSubscription(SubscriptionInputModel model)
    {
        var existing = subscriptionRepository.GetSubscriptionByMicrosoftId(model.MicrosoftId);
        if (existing == null)
            throw new InvalidOperationException("Subscription does not exist.");

        // Update only relevant fields
        existing.SubscriptionStatus = model.Status;
        existing.IsActive = model.IsActive;
        existing.AMPPlanId = model.AMPPlanId;

        subscriptionRepository.UpdateSubscription(existing);
    }

    // Updates subscription status and active flag directly
    public void UpdateStateOfSubscription(string microsoftId, string status, bool isActive)
    {
        subscriptionRepository.UpdateSubscriptionStatus(microsoftId, status, isActive);
    }

    // Retrieves a subscription by its Microsoft ID
    public Subscriptions GetByMicrosoftId(string microsoftId)
    {
        return subscriptionRepository.GetSubscriptionByMicrosoftId(microsoftId);
    }

    // Maps SubscriptionInputModel to Subscriptions entity
    private Subscriptions MapToEntity(SubscriptionInputModel model)
    {
        return new Subscriptions
        {
            MicrosoftId = model.MicrosoftId,
            SubscriptionStatus = model.Status,
            AMPPlanId = model.AMPPlanId,
            IsActive = model.IsActive,
            UserId = model.UserId,
            PurchaserEmail = model.PurchaserEmail,
            PurchaserTenantId = model.PurchaserTenantId,
            AutoRenew = model.AutoRenew,
            Term = model.Term,
            StartDate = model.StartDate,
            EndDate = model.EndDate

        };

    }

}
            