using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.StatusHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class AbstractSubscriptionStatusHandler : ISubscriptionStatusHandler
{
    protected readonly ISubscriptionsRepository subscriptionsRepository;
    protected readonly ILicensesRepository licensesRepository;
    protected readonly IClientsRepository clientsRepository;
    protected readonly ISubLinesRepository subLinesRepository;

    public AbstractSubscriptionStatusHandler(
        ISubscriptionsRepository subscriptionsRepository,
        ILicensesRepository licensesRepository,
        IClientsRepository clientsRepository,
        ISubLinesRepository subLinesRepository)
    {
        this.subscriptionsRepository = subscriptionsRepository;
        this.licensesRepository = licensesRepository;
        this.clientsRepository = clientsRepository;
        this.subLinesRepository = subLinesRepository;
    }

    // Base handler method to be overridden by concrete status handlers
    public virtual Task ProcessAsync(Guid subscriptionId)
    {
        throw new NotImplementedException("This handler does not implement ProcessAsync.");
    }

    // Retrieves a subscription entity using its Microsoft ID (converted from Guid)
    protected Subscriptions GetSubscriptionByMicrosoftId(Guid subscriptionId)
        => subscriptionsRepository.GetSubscriptionByMicrosoftId(subscriptionId.ToString());

    // Retrieves a license using the subscription ID as the license key
    protected Licenses GetLicenseBySubscriptionId(Guid subscriptionId)
        => licensesRepository.GetByLicenseKey(subscriptionId.ToString());

    // Retrieves all billing lines associated with the subscription ID
    protected IEnumerable<SubLines> GetSubLinesBySubscriptionId(Guid subscriptionId)
        => subLinesRepository.GetByMicrosoftId(subscriptionId.ToString());

    // Retrieves a client entity using a string-based client ID (parsed to int)
    protected Clients GetClientById(string clientId)
    {
        if (!int.TryParse(clientId, out var id))
            throw new ArgumentException("Invalid ClientId");

        return clientsRepository.GetByInstallationId(id);
    }

    // Updates the subscription status and sets its end date to now
    protected void UpdateSubscriptionStatus(Subscriptions subscription, string newStatus)
    {
        subscription.SubscriptionStatus = newStatus;
        subscription.EndDate = DateTime.UtcNow;
        subscriptionsRepository.UpdateSubscription(subscription);
    }

    // Extends the license expiration date based on the subscription's end date
    protected void UpdateLicenseExpirationFromSubscription(Guid subscriptionId)
    {
        var subscription = GetSubscriptionByMicrosoftId(subscriptionId);
        if (subscription == null)
            throw new InvalidOperationException("Subscription not found.");

        var license = GetLicenseBySubscriptionId(subscriptionId);
        if (license == null)
            throw new InvalidOperationException("License not found.");

        if (subscription.EndDate.HasValue)
        {
            var newExpiration = subscription.EndDate.Value.AddDays(14);
            license.LicenseExpires = newExpiration.ToString("yyyy-MM-dd");
        }
        else
        {
            throw new InvalidOperationException("Subscription has no EndDate defined.");
        }

        licensesRepository.UpdateLicense(license);
    }

    // Marks a license as expired by setting its status to 2
    protected void MarkLicenseAsExpired(Licenses license)
    {
        license.Status = 2;
        licensesRepository.UpdateLicense(license);
    }
}
