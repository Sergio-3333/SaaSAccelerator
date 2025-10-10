using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

/// <summary>
/// Service contract for managing Clients.
/// Provides methods to retrieve and synchronize client data.
/// </summary>
public interface IClientsService
{
    /// <summary>
    /// Retrieves a client by its InstallationId.
    /// </summary>
    /// <param name="installationId">The installation ID of the client.</param>
    /// <returns>The matching client entity, or null if not found.</returns>
    Clients GetClientByInstallationId(int installationId);

    /// <summary>
    /// Retrieves a client by its LicenseId.
    /// </summary>
    /// <param name="licenseId">The license ID associated with the client.</param>
    /// <returns>The matching client entity, or null if not found.</returns>
    Clients GetClientByLicenseId(int licenseId);

    /// <summary>
    /// Retrieves a client by its email address.
    /// </summary>
    /// <param name="email">The OWA email of the client.</param>
    /// <returns>The matching client entity, or null if not found.</returns>
    Clients GetClientByEmail(string email);

    /// <summary>
    /// Creates or updates a client based on subscription data received from Microsoft.
    /// If the email exists, updates MicrosoftId, LicenseId, LicenseType, and InstallationId.
    /// If not, creates a new client with the provided data.
    /// </summary>
    /// <param name="subscription">The subscription input model containing client data.</param>
    void CreateOrUpdateClientFromSubscription(SubscriptionInputModel subscription);
}
