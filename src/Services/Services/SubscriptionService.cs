using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

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

    public void CreateSubscription(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MicrosoftId))
            throw new ArgumentException("MicrosoftId no puede estar vacío.");

        // 1. Mapear y guardar suscripción
        var entity = MapToEntity(model);
        subscriptionRepository.AddSubscription(entity); // Usar Add en vez de Save para claridad

        // 2. Crear línea de facturación con los datos enriquecidos
        subLinesService.CreateFromDataModel(model);
    }

    public void UpdateSubscription(SubscriptionInputModel model)
    {
        var existing = subscriptionRepository.GetSubscriptionByMicrosoftId(model.MicrosoftId);
        if (existing == null)
            throw new InvalidOperationException("La suscripción no existe.");

        // Actualizar solo lo que corresponda
        existing.SubscriptionStatus = model.Status;
        existing.IsActive = model.IsActive;
        existing.AMPPlanId = model.AMPPlanId;

        subscriptionRepository.UpdateSubscription(existing);
    }

    public void UpdateStateOfSubscription(string microsoftId, string status, bool isActive)
    {
        subscriptionRepository.UpdateSubscriptionStatus(microsoftId, status, isActive);
    }

    public Subscriptions GetByMicrosoftId(string microsoftId)
    {
        return subscriptionRepository.GetSubscriptionByMicrosoftId(microsoftId);
    }


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
            Term = model.Term,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            AutoRenew = model.AutoRenew
        };
    }
}
