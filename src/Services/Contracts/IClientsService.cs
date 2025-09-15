using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface IClientsService
{
    // Consultas
    IEnumerable<Clients> GetAllClients();
    Clients GetClientByInstallationId(int installationId);
    Clients GetClientByLicenseId(int licenseId);
    Clients GetClientByEmail(string email);

    // Escenario 1: actualizar cliente existente
    void UpdateExistingClientFromPurchase(SubscriptionInputModel subscription);

    // Escenario 2: crear cliente nuevo
    void CreateNewClientFromPurchase(SubscriptionInputModel subscription, int licenseId);

    // (Opcional) Método unificado para compatibilidad con código antiguo
    void CreateOrUpdateClientFromSubscription(SubscriptionInputModel subscription, int licenseId, int installationId);
}
