using System;
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

    public Clients Get(int id)
    {
        return _context.Clients.FirstOrDefault(c => c.InstallationID == id);
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
        return _context.Clients.FirstOrDefault(c => c.ContactInfoEmail == email);
    }

    public Clients GetByInstallationId(int installationId)
    {
        return _context.Clients.FirstOrDefault(c => c.InstallationID == installationId);
    }

    public void CreateOrUpdateClientFromSubscription(Subscriptions subscription, int licenseId, int installationId)
    {
        var existingClient = GetByInstallationId(installationId);

        if (existingClient != null)
        {
            existingClient.LicenseID = licenseId;
            existingClient.ContactInfoEmail = subscription.PurchaserEmail;
            existingClient.ContactInfoCompany = subscription.Name;
            existingClient.UsageCounter = subscription.AMPQuantity;
            existingClient.InternalNote = subscription.PurchaserTenantId;
            existingClient.CampaignGUID = subscription.MicrosoftId?.ToString();
            existingClient.LastAccessed = DateTime.UtcNow.ToString("yyyy-MM-dd");

            Save(existingClient);
        }
        else
        {
            var newClient = new Clients
            {
                InstallationID = installationId,
                LicenseID = licenseId,
                ContactInfoEmail = subscription.PurchaserEmail,
                ContactInfoCompany = subscription.Name,
                UsageCounter = subscription.AMPQuantity,
                InternalNote = subscription.PurchaserTenantId,
                CampaignGUID = subscription.MicrosoftId?.ToString(),
                Created = (subscription.StartDate ?? DateTime.UtcNow).ToString("yyyy-MM-dd")
        };

            Save(newClient);
        }
    }

}

