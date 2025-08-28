using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using System;

public class ClientsService
{
    private readonly IClientsRepository _clientsRepository;

    public ClientsService(IClientsRepository clientsRepository)
    {
        _clientsRepository = clientsRepository;
    }

    public void CreateOrUpdateClientFromSubscription(Subscriptions subscription, int licenseId)
    {
        var existingClient = _clientsRepository.GetByLicenseId(licenseId);

        if (existingClient != null)
        {
            existingClient.ContactInfoEmail = subscription.PurchaserEmail;
            existingClient.ContactInfoCompany = subscription.Name;
            existingClient.UsageCounter = subscription.AMPQuantity;
            existingClient.InternalNote = subscription.PurchaserTenantId;
            existingClient.CampaignGUID = subscription.MicrosoftId;
            existingClient.LastAccessed = DateTime.UtcNow.ToString("yyyy-MM-dd");

            _clientsRepository.Save(existingClient);
        }
        else
        {
            var newClient = new Clients
            {
                LicenseID = licenseId,
                ContactInfoEmail = subscription.PurchaserEmail,
                ContactInfoCompany = subscription.Name,
                UsageCounter = subscription.AMPQuantity,
                InternalNote = subscription.PurchaserTenantId,
                CampaignGUID = subscription.MicrosoftId,
                Created = subscription.StartDate?.ToString("yyyy-MM-dd") ?? DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            _clientsRepository.Save(newClient);
        }
    }
}
