using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Contracts;

public interface ISubLinesRepository : IBaseRepository<SubLines>
{
    /// <summary>
    /// Obtiene todas las sublíneas asociadas a un MicrosoftId.
    /// </summary>
    IEnumerable<SubLines> GetByMicrosoftId(int microsoftId);

    /// <summary>
    /// Inserta una nueva línea de suscripción (renovación o compra inicial).
    /// </summary>
    int AddNewLine(SubLines subLine);
}
