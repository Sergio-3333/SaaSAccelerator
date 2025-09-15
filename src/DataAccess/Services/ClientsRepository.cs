using System.Collections.Generic;
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

    public IEnumerable<Clients> Get()
    {
        return _context.Clients.ToList();
    }

    public Clients Get(int installationId)
    {
        return _context.Clients.FirstOrDefault(c => c.InstallationID == installationId);
    }

    public int Save(Clients entity)
    {
        var existing = Get(entity.InstallationID);
        if (existing == null)
        {
            _context.Clients.Add(entity);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(entity);
        }
        _context.SaveChanges();
        return entity.InstallationID;
    }

    public void Remove(Clients entity)
    {
        _context.Clients.Remove(entity);
        _context.SaveChanges();
    }

    public Clients GetByLicenseId(int licenseId)
    {
        return _context.Clients.FirstOrDefault(c => c.LicenseID == licenseId);
    }

    public Clients GetByEmail(string email)
    {
        return _context.Clients.FirstOrDefault(c => c.OWAEmail == email);
    }

    public Clients GetByInstallationId(int installationId)
    {
        return _context.Clients.FirstOrDefault(c => c.InstallationID == installationId);
    }

    public void CreateClient(Clients clientEntity)
    {
        _context.Clients.Add(clientEntity);
        _context.SaveChanges();
    }

    public void UpdateClient(Clients clientEntity)
    {
        var existing = GetByLicenseId(clientEntity.LicenseID);
        if (existing != null)
        {
            _context.Entry(existing).CurrentValues.SetValues(clientEntity);
            _context.SaveChanges();
        }
    }
}
