using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

// Repository for the Subscriptions entity.
// Handles creation, updates, and retrieval of subscription records.
// Uses EF Core to interact with the SaasKitContext database.

public class SubscriptionsRepository : ISubscriptionsRepository
{
    private readonly SaasKitContext _context;

    // Constructor that injects the database context.
    public SubscriptionsRepository(SaasKitContext context)
    {
        _context = context;
    }

    // Adds a new subscription to the database.
    // Returns the MicrosoftId of the newly created subscription.
    public string AddSubscription(Subscriptions subscription)
    {
        _context.Subscriptions.Add(subscription);
        _context.SaveChanges();
        return subscription.MicrosoftId;
    }

    // Updates an existing subscription based on MicrosoftId.
    // Throws an exception if the subscription does not exist.
    public void UpdateSubscription(string microsoftId, Action<Subscriptions> updateAction)
    {
        var existing = _context.Subscriptions
            .FirstOrDefault(s => s.MicrosoftId == microsoftId);

        if (existing == null)
            throw new InvalidOperationException("Subscription does not exist.");

        // Aquí aplicas los cambios que quieras
        updateAction(existing);

        _context.SaveChanges();
    }


    public bool ExistsByMicrosoftId(string microsoftId)
    {
        return _context.Subscriptions.Any(s => s.MicrosoftId == microsoftId);
    }


    // Retrieves a subscription by its MicrosoftId.
    public Subscriptions GetSubscriptionByMicrosoftId(string microsoftId) =>
        _context.Subscriptions.FirstOrDefault(s => s.MicrosoftId == microsoftId);

}
