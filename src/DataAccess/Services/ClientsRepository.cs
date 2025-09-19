using System;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

// Repository for the Clients entity.
// Encapsulates basic CRUD operations using Entity Framework Core.
// Provides methods to create, update, and retrieve client records.

public class ClientsRepository : IClientsRepository
{
    private readonly SaasKitContext _context;

    // Constructor that injects the database context.
    public ClientsRepository(SaasKitContext context)
    {
        _context = context;
    }

    // Adds a new client to the database.
    public void CreateClient(Clients clientEntity)
    {
        _context.Clients.Add(clientEntity);
        _context.SaveChanges();
    }

    // Updates an existing client based on InstallationID.
    // Throws an exception if the client does not exist.
    public void UpdateClient(Clients clientEntity)
    {
        var existing = _context.Clients.FirstOrDefault(c => c.InstallationID == clientEntity.InstallationID);
        if (existing == null)
            throw new InvalidOperationException("Client does not exist.");

        _context.Entry(existing).CurrentValues.SetValues(clientEntity);
        _context.SaveChanges();
    }

    // Retrieves a client by InstallationID.
    public Clients GetByInstallationId(int installationId) =>
        _context.Clients.FirstOrDefault(c => c.InstallationID == installationId);

    // Retrieves a client by LicenseID.
    public Clients GetByLicenseId(int licenseId) =>
        _context.Clients.FirstOrDefault(c => c.LicenseID == licenseId);

    // Retrieves a client by OWA email.
    public Clients GetByEmail(string email) =>
        _context.Clients.FirstOrDefault(c => c.OWAEmail == email);
}

