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

        if (existingClient != null)
        {
            existingClient.MicrosoftId = model.MicrosoftId;

            if (existingClient.LicenseID == 0)
            {
                existingClient.LicenseID = GenerateLicenseId();
            }

            existingClient.LicenseType = licenseType;

            clientsRepository.UpdateClient(existingClient);
        }

        else
        {
            // Create a new client record
            var newClient = new Clients
            {
                OWAEmail = model.PurchaserEmail,
                MicrosoftId = model.MicrosoftId,
                LicenseID = GenerateLicenseId(),
                LicenseType = licenseType,
                LastAccessed = "-",
                Created = "-"
        };

            clientsRepository.CreateClient(newClient);
        }
    }

    private static int GenerateLicenseId()
    {
        var random = new Random();
        return random.Next(1, 10001);
    }



    // Maps AMP plan name to internal license type ID
    private int ConvertLicenseType(string microsoftPlanId) =>
        microsoftPlanId switch
        {
            "atxttst001" => 1,
            "atxttst002" => 2,
            "atxttst003" => 3,
            "atxttst004" => 4,
            _ => throw new InvalidOperationException("Unrecognized plan")
        };


}
