using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using System;

public class ClientsService : IClientsService
{
    private readonly IClientsRepository clientsRepository;
    private readonly ILicenseService licenseService;

    public ClientsService(IClientsRepository clientsRepository, ILicenseService licenseService)
    {
        this.clientsRepository = clientsRepository;
        this.licenseService = licenseService;
    }

    public Clients GetClientByInstallationId(int installationId) =>
        clientsRepository.GetByInstallationId(installationId);

    public Clients GetClientByLicenseId(int licenseId) =>
        clientsRepository.GetByLicenseId(licenseId);

    public Clients GetClientByEmail(string email) =>
        clientsRepository.GetByEmail(email);

    public void CreateOrUpdateClientFromSubscription(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PurchaserEmail))
            throw new ArgumentException("El email del cliente no puede estar vacío.");

        var existingClient = clientsRepository.GetByEmail(model.PurchaserEmail);

        int licenseType = ConvertLicenseType(model.AMPPlanId);
        int installationId = GenerateInstallationId();
        int licenseId = licenseService.SaveLicenseFromInputModel(model, licenseType);

        if (existingClient != null)
        {
            existingClient.MicrosoftId = model.MicrosoftId;
            existingClient.LicenseID = licenseId;
            existingClient.LicenseType = licenseType;
            existingClient.InstallationID = installationId;

            clientsRepository.UpdateClient(existingClient);
        }
        else
        {
            var newClient = new Clients
            {
                OWAEmail = model.PurchaserEmail,
                MicrosoftId = model.MicrosoftId,
                LicenseID = licenseId,
                LicenseType = licenseType,
                InstallationID = installationId
            };

            clientsRepository.CreateClient(newClient);
        }
    }

    private int ConvertLicenseType(string microsoftPlanId) =>
        microsoftPlanId switch
        {
            "Ant Text 365 Standart" => 1,
            "Ant Text 365 Premium" => 2,
            _ => throw new InvalidOperationException("Plan no reconocido")
        };

    private int GenerateInstallationId() =>
        (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
