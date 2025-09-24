using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ILicensesRepository
{
    /// <summary>
    /// Inserta una nueva licencia en la base de datos.
    /// </summary>
    int CreateLicense(Licenses license);

    /// <summary>
    /// Actualiza una licencia existente.
    /// </summary>
    void UpdateLicense(Licenses license);

    /// <summary>
    /// Obtiene una licencia por su identificador.
    /// </summary>
    Licenses GetById(int licenseId);

    /// <summary>
    /// Obtiene una licencia por su clave de licencia.
    /// </summary>
    Licenses GetByLicenseKey(string licenseKey);

    /// <summary>
    /// Obtiene todas las licencias asociadas a un MicrosoftId.
    /// </summary>
    IEnumerable<Licenses> GetByMicrosoftId(string microsoftId);

    /// <summary>
    /// Obtiene una licencia por email.
    /// </summary>
    Licenses GetByEmail(string email);

    bool ExistsLicenseId(int id);
    bool ExistsLicenseKey(string key);


}
