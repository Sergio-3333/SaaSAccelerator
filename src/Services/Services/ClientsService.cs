using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using System;

public class ClientsService : IClientsService
{
    private readonly IClientsRepository clientsRepository;
    private readonly ILicensesRepository licensesRepository;


    public ClientsService(IClientsRepository clientsRepository, ILicensesRepository licensesRepository)
    {
        this.clientsRepository = clientsRepository;
        this.licensesRepository = licensesRepository;
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

        var license = licensesRepository.GetByEmail(model.PurchaserEmail);
        if (license == null)
            throw new InvalidOperationException($"No existe un License asociado al email {model.PurchaserEmail}");

        var existingClient = clientsRepository.GetByEmail(model.PurchaserEmail);

        int licenseType = ConvertLicenseType(model.AMPPlanId);

        if (existingClient != null)
        {
            existingClient.MicrosoftID = model.MicrosoftId;

            if (existingClient.LicenseID == 0)
            {
                existingClient.LicenseID = license.LicenseID;
            }
            else if (existingClient.LicenseID != license.LicenseID)
            {
                existingClient.LicenseID = license.LicenseID;
            }

            existingClient.LicenseType = licenseType;
            clientsRepository.UpdateClient(existingClient);
        }
        else
        {
            var newClient = new Clients
            {
                OWAEmail = model.PurchaserEmail,
                MicrosoftID = model.MicrosoftId,
                LicenseID = license.LicenseID,
                LicenseType = licenseType,
                LastAccessed = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                Created = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                ContactInfoCompany = " ",
                ContactInfoContact = model.Name ?? "0",
                ContactInfoPhone = " ",
                ContactInfoEmail = " ",
                UsageCounter = 0,
                ContactInfoOK = "Yes",
                PartnerID = 0,
                OWADispName = model.Name ?? "0",
                OWAEWSURL = " ",
                OWAEWSUID = " ",
                OWAEWSPWD = " ",
                OWAPersonColor = 0,
                OWAInitials = " ",
                OWAHasImage = 0,
                OWADispLang = " ",
                LastTokenRefresh = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                UseEWS = 0,
                TestMode = 0,
                TimeZone = 0,
                UserDevice = "0",
                TradeID = 0,
                FirstEmailSent = 0,
                ClientTypeID = 0,
                SkipConsent = 0,
                TrialDays = 0,
                CampaignGUID = "0",
                LastLocCheck = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                NewsLetterUsageCounter = 0,
                ContactInfoTitle = " ",
                ContactInfoWebSite = " ",
                ContactInfoAddress = " ",
                ContactInfoLinkedIn = " ",
                FlowUsageCounter = 0,
                CJMode = 0,
                InternalNote = " ",
                InstallDateATC = " ",
                ContactInfoCountryID = 0

            };

            clientsRepository.CreateClient(newClient);
        }
    }




    // Maps AMP plan name to internal license type ID
    private int ConvertLicenseType(string microsoftPlanId) =>
        microsoftPlanId switch
        {
            "atxt001" => 1,
            "atxt002" => 2,
            "atxtstd025" => 25,
            "atxtbus025" => 26,
            "atxtstd030" => 30,
            "atxtbus030" => 31,
            "atxtstd040" => 40,
            "atxtbus040" => 41,
            "atxtstd050" => 50,
            "atxtbus050" => 51,

            _ => throw new InvalidOperationException("Unrecognized plan")
        };


}
