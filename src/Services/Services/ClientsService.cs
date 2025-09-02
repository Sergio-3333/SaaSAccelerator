using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class ClientsService : IClientsService
{
    private readonly IClientsRepository clientsRepository;

    public ClientsService(IClientsRepository clientsRepository)
    {
        this.clientsRepository = clientsRepository;
    }

    public IEnumerable<ClientModel> GetAllClients()
    {
        return clientsRepository.Get()
            .Select(c => MapToModel(c))
            .ToList();
    }

    public ClientModel GetClientByInstallationId(int installationId)
    {
        var client = clientsRepository.GetByInstallationId(installationId);
        return client != null ? MapToModel(client) : null;
    }

    public ClientModel GetClientByLicenseId(int licenseId)
    {
        var client = clientsRepository.GetByLicenseId(licenseId);
        return client != null ? MapToModel(client) : null;
    }

    public ClientModel GetClientByEmail(string email)
    {
        var client = clientsRepository.GetByEmail(email);
        return client != null ? MapToModel(client) : null;
    }

    public void CreateOrUpdateClientFromSubscription(SubscriptionModel subscription, int licenseId, int installationId)
    {
        var subscriptionEntity = new DataAccess.Entities.Subscriptions
        {
            PurchaserEmail = subscription.PurchaserEmail,
            Name = subscription.Name,
            AMPQuantity = subscription.Quantity ?? 0,
            PurchaserTenantId = subscription.PurchaserTenantId,
            MicrosoftId = subscription.MicrosoftId,
            StartDate = subscription.StartDate
        };

        clientsRepository.CreateOrUpdateClientFromSubscription(subscriptionEntity, licenseId, installationId);
    }

    private ClientModel MapToModel(DataAccess.Entities.Clients entity)
    {
        return new ClientModel
        {
            InstallationID = entity.InstallationID,
            LicenseID = entity.LicenseID,
            ContactInfoEmail = entity.ContactInfoEmail,
            ContactInfoCompany = entity.ContactInfoCompany,
            UsageCounter = entity.UsageCounter,
            InternalNote = entity.InternalNote,
            CampaignGUID = entity.CampaignGUID,
            Created = FormatDateString(entity.Created),
            LastAccessed = FormatDateString(entity.LastAccessed)
        };
    }

    private string FormatDateString(string dateString)
    {
        if (DateTime.TryParse(dateString, out var date))
        {
            return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        return dateString;
    }

}
