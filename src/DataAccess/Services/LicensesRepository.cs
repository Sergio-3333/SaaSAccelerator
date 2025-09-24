using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

// Repository for the Licenses entity.
// Provides methods to create, update, and query license records.
// Uses EF Core to interact with the SaasKitContext database.

public class LicensesRepository : ILicensesRepository
{
    private readonly SaasKitContext _context;

    // Constructor that injects the database context.
    public LicensesRepository(SaasKitContext context)
    {
        _context = context;
    }

    // Returns the next available LicenseID by incrementing the highest existing one.
    public int GetNextLicenseId()
    {
        var lastId = _context.Licenses
            .OrderByDescending(l => l.LicenseID)
            .Select(l => l.LicenseID)
            .FirstOrDefault();

        return lastId + 1;
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
    public void UpdateLicense(Licenses license)
    {
        var existing = _context.Licenses
            .FirstOrDefault(l => l.LicenseID == license.LicenseID);

        if (existing == null)
            throw new InvalidOperationException("License does not exist.");

        _context.Entry(existing).CurrentValues.SetValues(license);
        _context.SaveChanges();
    }

    // Retrieves a license by its LicenseID.
    public Licenses GetById(int licenseId) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseID == licenseId);

    // Retrieves a license by its LicenseKey.
    public Licenses GetByLicenseKey(string licenseKey) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);

    // Retrieves all licenses associated with a given MicrosoftId.
    public IEnumerable<Licenses> GetByMicrosoftId(string microsoftId) =>
        _context.Licenses.Where(l => l.MicrosoftId == microsoftId).ToList();

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
