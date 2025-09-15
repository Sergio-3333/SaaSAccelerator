using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface IClientsService
{
    /// <summary>
    /// Obtiene un cliente por su InstallationId.
    /// </summary>
    Clients GetClientByInstallationId(int installationId);

    /// <summary>
    /// Obtiene un cliente por su LicenseId.
    /// </summary>
    Clients GetClientByLicenseId(int licenseId);

    /// <summary>
    /// Obtiene un cliente por su email.
    /// </summary>
    Clients GetClientByEmail(string email);

    /// <summary>
    /// Crea o actualiza un cliente a partir de los datos recibidos de Microsoft.
    /// Si el email existe, actualiza MicrosoftId, LicenseId, LicenseType e InstallationId.
    /// Si no existe, crea un nuevo cliente con esos datos.
    /// </summary>
    void CreateOrUpdateClientFromSubscription(SubscriptionInputModel subscription);
}
