using System;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

public class ClientsRepository : IClientsRepository
{
    private readonly SaasKitContext _context;

    public ClientsRepository(SaasKitContext context)
    {
        _context = context;
    }

    public void CreateClient(Clients clientEntity)
    {
        _context.Clients.Add(clientEntity);
        _context.SaveChanges();
    }

    public void UpdateClient(Clients clientEntity)
    {
        var existing = _context.Clients.FirstOrDefault(c => c.InstallationID == clientEntity.InstallationID);
        if (existing == null)
            throw new InvalidOperationException("El cliente no existe.");

        _context.Entry(existing).CurrentValues.SetValues(clientEntity);
        _context.SaveChanges();
    }

    public Clients GetByInstallationId(int installationId) =>
        _context.Clients.FirstOrDefault(c => c.InstallationID == installationId);

    public Clients GetByLicenseId(int licenseId) =>
        _context.Clients.FirstOrDefault(c => c.LicenseID == licenseId);

    public Clients GetByEmail(string email) =>
        _context.Clients.FirstOrDefault(c => c.OWAEmail == email);
}
