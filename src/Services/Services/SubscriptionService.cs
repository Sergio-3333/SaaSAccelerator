using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Marketplace.SaaS.Models;
using System;
using System.ComponentModel;

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


        subscriptionRepository.UpdateSubscription(existing.MicrosoftID, s =>
        {
            s.SubStatus = model.Status;
            s.Active = model.IsActive;
            s.PlanId = model.AMPPlanId;
        });
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
            MicrosoftID = model.MicrosoftId,
            SubStatus = model.Status,
            PlanId = model.AMPPlanId,
            Active = model.IsActive,
            UserID = model.UserId,
            PurEmail = model.PurchaserEmail,
            PurTenantId = model.PurchaserTenantId,
            AutoRenew = model.AutoRenew,
            Term = model.Term,
            StartDate = model.StartDate,
            EndDate = CalculateLicenseEndDate(model.Term),
            SubName = ConvertLicenseType(model.AMPPlanId),
            Country = model.Country
        };
    }

    public static DateTime CalculateLicenseEndDate(string term)
    {
        if (string.Equals(term, "P1M", StringComparison.OrdinalIgnoreCase))
        {
            // Si el término es P1M → +1 mes
            return DateTime.UtcNow.AddMonths(1);
        }
        else
        {
            // En cualquier otro caso → +1 año
            return DateTime.UtcNow.AddYears(1);
        }
    }


    private static string ConvertLicenseType(string ampPlanId)
    {
        if (string.Equals(ampPlanId, "atxttst001", StringComparison.OrdinalIgnoreCase))
        {
            return "Ant Text 365 Standard";
        }
        else
        {
            return "Ant Text 365 Business";
        }
    }



}
