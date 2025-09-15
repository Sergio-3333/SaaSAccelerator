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
    /// Obtiene el histórico de sublíneas para un MicrosoftId.
    /// </summary>
    IEnumerable<SubLines> GetHistoryByMicrosoftId(string microsoftId);
}
