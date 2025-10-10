using System;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Microsoft.Extensions.Logging;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

// Repository for the Licenses entity.
// Provides methods to create, update, and query license records.
// Uses EF Core to interact with the SaasKitContext database.

public class LicensesRepository : ILicensesRepository
{
    private readonly SaasKitContext _context;
    private readonly ILogger<LicensesRepository> logger;


    // Constructor that injects the database context.
    public LicensesRepository(SaasKitContext context, ILogger<LicensesRepository> logger)
    {
        _context = context;
        this.logger = logger;

    }



    // Adds a new license to the database and returns its generated LicenseID.
    public int CreateLicense(Licenses license)
    {
        _context.Licenses.Add(license);
        _context.SaveChanges();
        return license.LicenseID;
    }


    // Updates an existing license based on LicenseID.
    // Throws an exception if the license does not exist.
    public void UpdateLicense(string microsoftId, Action<Licenses> updateAction)
    {
        if (string.IsNullOrWhiteSpace(microsoftId))
        {
            logger.LogError("[Repo] El MicrosoftId recibido es NULL o vacío. No se puede actualizar.");
            return;
        }

        var entity = _context.Licenses
            .FirstOrDefault(x => x.MicrosoftID == microsoftId);

        if (entity == null)
        {
            logger.LogWarning($"[Repo] No se encontró License con MicrosoftId={microsoftId} para actualizar.");
            return;
        }

        updateAction(entity);
        _context.SaveChanges();
    }



    // Retrieves a license by its LicenseID.
    public Licenses GetById(int licenseId) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseID == licenseId);


    // Retrieves all licenses associated with a given MicrosoftId.
    public Licenses GetLicenseByMicrosoftId(string microsoftId) =>
        _context.Licenses.FirstOrDefault(l => l.MicrosoftID == microsoftId);

    // Retrieves a license by the purchaser's email.
    public Licenses GetByEmail(string email) =>
        _context.Licenses.FirstOrDefault(l => l.Email == email);

    public bool ExistsLicenseId(int id)
    {
        return _context.Licenses.Any(l => l.LicenseID == id);
    }

    public bool ExistsLicenseKey(string key)
    {
        return _context.Licenses.Any(l => l.LicenseKey == key);
    }
}
