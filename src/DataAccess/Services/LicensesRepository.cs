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

    public int Save(Licenses license)
    {
        var existing = _context.Licenses
            .FirstOrDefault(l => l.LicenseID == license.LicenseID);

        if (existing != null)
        {
            existing.MicrosoftId = license.MicrosoftId;
            existing.Email = license.Email;
            existing.Company = license.Company;
            existing.Created = license.Created;
            existing.LicenseExpires = license.LicenseExpires;
            existing.PurchasedLicenses = license.PurchasedLicenses;
            existing.Comment = license.Comment;
            existing.Duration = license.Duration;
            existing.Status = license.Status;

            _context.Licenses.Update(existing);
            _context.SaveChanges();
            return existing.LicenseID;
        }

        _context.Licenses.Add(license);
        _context.SaveChanges();
        return license.LicenseID;
    }

    public Licenses Get(int licenseId) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseID == licenseId);

    public IEnumerable<Licenses> Get() =>
        _context.Licenses.OrderByDescending(l => l.Created);

    public Licenses GetByLicenseKey(string licenseKey) =>
        _context.Licenses.FirstOrDefault(l => l.LicenseKey == licenseKey);

    public IEnumerable<Licenses> GetByMicrosoftId(string microsoftId) =>
        _context.Licenses.Where(l => l.MicrosoftId == microsoftId);

    public void Remove(Licenses entity)
    {
        _context.Licenses.Remove(entity);
        _context.SaveChanges();
    }
}
