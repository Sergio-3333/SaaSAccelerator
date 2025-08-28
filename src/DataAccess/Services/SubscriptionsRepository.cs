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
        var existing = _context.Subscriptions
            .FirstOrDefault(s => s.MicrosoftId == subscription.MicrosoftId);

        if (existing != null)
        {
            existing.SubscriptionStatus = subscription.SubscriptionStatus;
            existing.AMPPlanId = subscription.AMPPlanId;
            existing.AMPQuantity = subscription.AMPQuantity;
            existing.AmpOfferId = subscription.AmpOfferId;
            existing.Term = subscription.Term;
            existing.StartDate = subscription.StartDate;
            existing.EndDate = subscription.EndDate;
            existing.Name = subscription.Name;
            existing.PurchaserEmail = subscription.PurchaserEmail;
            existing.PurchaserTenantId = subscription.PurchaserTenantId;
            existing.ModifyDate = DateTime.UtcNow;

            _context.Subscriptions.Update(existing);
            _context.SaveChanges();
            return existing.Id;
        }

        subscription.CreateDate = DateTime.UtcNow;
        _context.Subscriptions.Add(subscription);
        _context.SaveChanges();
        return subscription.Id;
    }

    public Subscriptions Get(int id) =>
        _context.Subscriptions.FirstOrDefault(s => s.Id == id);

    public IEnumerable<Subscriptions> Get() =>
        _context.Subscriptions.OrderByDescending(s => s.CreateDate);

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
