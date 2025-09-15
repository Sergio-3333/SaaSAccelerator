using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System.Linq;
using System;

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly SaasKitContext _context;

    public SubscriptionsRepository(SaasKitContext context)
    {
        _context = context;
    }

    public string AddSubscription(Subscriptions subscription)
    {
        _context.Subscriptions.Add(subscription);
        _context.SaveChanges();
        return subscription.MicrosoftId;
    }

    public void UpdateSubscription(Subscriptions subscription)
    {
        var existing = _context.Subscriptions
            .FirstOrDefault(s => s.MicrosoftId == subscription.MicrosoftId);

        if (existing == null)
            throw new InvalidOperationException("La suscripción no existe.");

        _context.Entry(existing).CurrentValues.SetValues(subscription);
        _context.SaveChanges();
    }

    public Subscriptions GetSubscriptionByMicrosoftId(string microsoftId) =>
        _context.Subscriptions.FirstOrDefault(s => s.MicrosoftId == microsoftId);

    public void UpdateSubscriptionStatus(string microsoftId, string status, bool isActive)
    {
        var existing = _context.Subscriptions.FirstOrDefault(s => s.MicrosoftId == microsoftId);
        if (existing != null)
        {
            existing.SubscriptionStatus = status;
            existing.IsActive = isActive;
            _context.SaveChanges();
        }
    }
}
