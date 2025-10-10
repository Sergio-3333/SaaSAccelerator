using System;
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

    void UpdateLicense(string microsoftId, Action<Licenses> updateAction);

    /// <summary>
    /// Obtiene una licencia por su identificador.
    /// </summary>
    Licenses GetById(int licenseId);


    /// <summary>
    /// Obtiene todas las licencias asociadas a un MicrosoftId.
    /// </summary>
    Licenses GetLicenseByMicrosoftId(string microsoftId);

    /// <summary>
    /// Obtiene una licencia por email.
    /// </summary>
    Licenses GetByEmail(string email);

    bool ExistsLicenseId(int id);
    bool ExistsLicenseKey(string key);


}
