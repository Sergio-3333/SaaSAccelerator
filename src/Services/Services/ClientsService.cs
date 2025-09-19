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

    // Retrieves a client using the installation ID
    public Clients GetClientByInstallationId(int installationId) =>
        clientsRepository.GetByInstallationId(installationId);

    // Retrieves a client using the license ID
    public Clients GetClientByLicenseId(int licenseId) =>
        clientsRepository.GetByLicenseId(licenseId);

    // Retrieves a client using their email address
    public Clients GetClientByEmail(string email) =>
        clientsRepository.GetByEmail(email);

    // Creates or updates a client based on subscription data
    public void CreateOrUpdateClientFromSubscription(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.PurchaserEmail))
            throw new ArgumentException("Client email cannot be empty.");

        var existingClient = clientsRepository.GetByEmail(model.PurchaserEmail);

        // Determine license type based on AMP plan
        int licenseType = ConvertLicenseType(model.AMPPlanId);

        // Generate a unique installation ID based on current timestamp
        int installationId = GenerateInstallationId();

        // Save license and get its ID
        int licenseId = licenseService.SaveLicenseFromInputModel(model, licenseType);

        if (existingClient != null)
        {
            // Update existing client with new subscription details
            existingClient.MicrosoftId = model.MicrosoftId;
            existingClient.LicenseID = licenseId;
            existingClient.LicenseType = licenseType;
            existingClient.InstallationID = installationId;

            clientsRepository.UpdateClient(existingClient);
        }
        else
        {
            // Create a new client record
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

    // Maps AMP plan name to internal license type ID
    private int ConvertLicenseType(string microsoftPlanId) =>
        microsoftPlanId switch
        {
            "Ant Text 365 Standart" => 1,
            "Ant Text 365 Bussiness" => 2,
            _ => throw new InvalidOperationException("Unrecognized plan")
        };

    // Generates a numeric installation ID based on current UTC timestamp
    private int GenerateInstallationId() =>
        (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
}
