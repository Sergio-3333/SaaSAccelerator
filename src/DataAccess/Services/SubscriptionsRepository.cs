using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly SaasKitContext _context;

    public SubscriptionsRepository(SaasKitContext context)
    {
        _context = context;
    }

    public int Save(Subscriptions subscription)
    {
        if (string.IsNullOrWhiteSpace(subscription.MicrosoftId))
            throw new ArgumentException("MicrosoftId no puede estar vacío.");

        var existing = _context.Subscriptions
            .FirstOrDefault(s => s.MicrosoftId == subscription.MicrosoftId);

        if (existing != null)
        {
            existing.SubscriptionStatus = subscription.SubscriptionStatus;
            existing.AMPPlanId = subscription.AMPPlanId;
            existing.IsActive = subscription.IsActive;
            existing.UserId = subscription.UserId;
            existing.PurchaserEmail = subscription.PurchaserEmail;
            existing.PurchaserTenantId = subscription.PurchaserTenantId;
            existing.Term = subscription.Term;
            existing.StartDate = subscription.StartDate;
            existing.EndDate = subscription.EndDate;
            existing.AutoRenew = subscription.AutoRenew;

            _context.Subscriptions.Update(existing);
            _context.SaveChanges();

            // Como la PK es string, devolvemos 0 o cambiamos la interfaz para devolver string
            return 0;
        }

        _context.Subscriptions.Add(subscription);
        _context.SaveChanges();

        return 0;
    }

    public Subscriptions Get(int id)
    {
        // Ya no tiene sentido buscar por int, la PK es string
        throw new NotSupportedException("Subscriptions usa MicrosoftId como clave primaria.");
    }

    public IEnumerable<Subscriptions> Get() =>
        _context.Subscriptions.ToList();

    public Subscriptions GetByMicrosoftId(string microsoftId) =>
        _context.Subscriptions.FirstOrDefault(s => s.MicrosoftId == microsoftId);

    public IEnumerable<Subscriptions> GetActiveSubscriptions() =>
        _context.Subscriptions.Where(s => s.IsActive == true);

    public void UpdateStatus(string microsoftId, string status, bool isActive)
    {
        var existing = _context.Subscriptions.FirstOrDefault(s => s.MicrosoftId == microsoftId);
        if (existing != null)
        {
            existing.SubscriptionStatus = status;
            existing.IsActive = isActive;
            _context.Subscriptions.Update(existing);
            _context.SaveChanges();
        }
    }

    public void Remove(Subscriptions entity)
    {
        _context.Subscriptions.Remove(entity);
        _context.SaveChanges();
    }
}
