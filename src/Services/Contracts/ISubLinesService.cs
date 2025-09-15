using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

public interface ISubLinesService
{
    /// <summary>
    /// Crea una nueva línea de facturación a partir de un DataModel completo.
    /// </summary>
    int CreateFromDataModel(SubscriptionInputModel model);

    /// <summary>
    /// Crea una nueva línea de facturación a partir de una suscripción existente.
    /// </summary>
    int CreateFromSubscription(Subscriptions subscription, int quantity, int usersQ, decimal? amount);

    /// <summary>
    /// Obtiene el histórico de sublíneas para un MicrosoftId.
    /// </summary>
    IEnumerable<SubLines> GetHistoryByMicrosoftId(int microsoftId);
}
