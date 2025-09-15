using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

public class LicensesRepository : ILicensesRepository
{
    private readonly SaasKitContext _context;

    public LicensesRepository(SaasKitContext context)
    {
        _context = context;
    }

    public int GetNextLicenseId()
    {
        var lastId = _context.Licenses
            .OrderByDescending(l => l.LicenseID)
            .Select(l => l.LicenseID)
            .FirstOrDefault();

        return lastId + 1;
    }

    public int CreateLicense(Licenses license)
    {
        _context.Licenses.Add(license);
        _context.SaveChanges();
        return license.LicenseID;
    }

    public void UpdateLicense(Licenses license)
    {
        var existing = _context.Licenses
            .FirstOrDefault(l => l.LicenseID == license.LicenseID);

        if (existing == null)
            throw new InvalidOperationException("La licencia no existe.");

        _context.Entry(existing).CurrentValues.SetValues(license);
        _context.SaveChanges();
    }

    public Licenses GetById(int licenseId) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseID == licenseId);

    public Licenses GetByLicenseKey(string licenseKey) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);

    public IEnumerable<Licenses> GetByMicrosoftId(string microsoftId) =>
        _context.Licenses.Where(l => l.MicrosoftId == microsoftId).ToList();

    public Licenses GetByEmail(string email) =>
        _context.Licenses.FirstOrDefault(l => l.Email == email);
}
