using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface ISubscriptionService
{
    /// <summary>
    /// Crea o actualiza una suscripción a partir del modelo de entrada.
    /// </summary>
    void CreateOrUpdateSubscription(SubscriptionInputModel model);

    /// <summary>
    /// Actualiza el estado de una suscripción.
    /// </summary>
    void UpdateStateOfSubscription(string microsoftId, string status, bool isActive);

    /// <summary>
    /// Obtiene una suscripción por MicrosoftId.
    /// </summary>
    Subscriptions GetByMicrosoftId(string microsoftId);

    /// <summary>
    /// Obtiene todas las suscripciones activas.
    /// </summary>
    IEnumerable<Subscriptions> GetActiveSubscriptions();
}
