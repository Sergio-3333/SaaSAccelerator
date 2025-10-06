using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Services;

public interface ISubLinesService
{
    // Creates a new billing subline from a complete subscription input model.
    int CreateFromDataModel(SubscriptionInputModel model);


    // Retrieves billing history (sublines) for a given MicrosoftId.
    SubLines GetHistoryByMicrosoftId(string microsoftId);
}
