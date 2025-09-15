using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Contracts;

public interface ISubscriptionService
{
    /// <summary>
    /// Crea una nueva suscripción a partir de los datos recibidos de Microsoft.
    /// </summary>
    void CreateSubscription(SubscriptionInputModel model);

    /// <summary>
    /// Actualiza una suscripción existente con datos recibidos de Microsoft.
    /// </summary>
    void UpdateSubscription(SubscriptionInputModel model);

    /// <summary>
    /// Actualiza únicamente el estado y el flag de activo de una suscripción.
    /// </summary>
    void UpdateStateOfSubscription(string microsoftId, string status, bool isActive);

    /// <summary>
    /// Obtiene una suscripción por su MicrosoftId.
    /// </summary>
    Subscriptions GetByMicrosoftId(string microsoftId);

}
