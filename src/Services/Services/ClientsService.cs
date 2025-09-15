using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class ClientsService : IClientsService
{
    private readonly IClientsRepository clientsRepository;
    private readonly ILicensesRepository licensesRepository;

    public ClientsService(IClientsRepository clientsRepository, ILicensesRepository licensesRepository)
    {
        this.clientsRepository = clientsRepository;
        this.licensesRepository = licensesRepository;
    }

    // Consultas
    public IEnumerable<Clients> GetAllClients()
    {
        return clientsRepository.Get();
    }

    public Clients GetClientByInstallationId(int installationId)
    {
        return clientsRepository.GetByInstallationId(installationId);
    }

    public Clients GetClientByLicenseId(int licenseId)
    {
        return clientsRepository.GetByLicenseId(licenseId);
    }

    public Clients GetClientByEmail(string email)
    {
        return clientsRepository.GetByEmail(email);
    }

    // Escenario 1: actualizar cliente existente
    public void UpdateExistingClientFromPurchase(SubscriptionInputModel subscription)
    {
        var license = licensesRepository.GetByEmail(subscription.PurchaserEmail);
        if (license == null)
            throw new InvalidOperationException($"No se encontró licencia para {subscription.PurchaserEmail}");

        var client = clientsRepository.GetByLicenseId(license.LicenseID);
        if (client == null)
            throw new InvalidOperationException($"No se encontró cliente para LicenseId {license.LicenseID}");

        client.LicenseType = subscription.AMPPlanId == "Ant Text 365 Standart" ? 1 : 2;
        client.MicrosoftId = subscription.MicrosoftId.GetHashCode();
        client.LicenseID = license.LicenseID;

        clientsRepository.UpdateClient(client);
    }

    // Escenario 2: crear cliente nuevo
    public void CreateNewClientFromPurchase(SubscriptionInputModel subscription, int licenseId)
    {
        int installationId = GenerateInstallationId();

        var clientEntity = new Clients
        {
            InstallationID = installationId,
            LicenseID = licenseId,
            MicrosoftId = subscription.MicrosoftId.GetHashCode(),
            OWAEmail = subscription.PurchaserEmail,
            LicenseType = subscription.AMPPlanId == "Ant Text 365 Standart" ? 1 : 2
        };

        clientsRepository.CreateClient(clientEntity);
    }

    // Método unificado para compatibilidad
    public void CreateOrUpdateClientFromSubscription(SubscriptionInputModel subscription, int licenseId, int installationId)
    {
        var existingClient = clientsRepository.GetByInstallationId(installationId);
        if (existingClient != null)
        {
            UpdateExistingClientFromPurchase(subscription);
        }
        else
        {
            CreateNewClientFromPurchase(subscription, licenseId);
        }
    }

    private int GenerateInstallationId()
    {
        return new Random().Next(100000, 999999);
    }
}
