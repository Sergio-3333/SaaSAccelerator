using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.Services.StatusHandlers;

public abstract class AbstractSubscriptionStatusHandler : ISubscriptionStatusHandler
{
    protected readonly ISubscriptionsRepository subscriptionsRepository;
    protected readonly ILicensesRepository licensesRepository;
    protected readonly IClientsRepository clientsRepository;

    public AbstractSubscriptionStatusHandler(
        ISubscriptionsRepository subscriptionsRepository,
        ILicensesRepository licensesRepository,
        IClientsRepository clientsRepository)
    {
        this.subscriptionsRepository = subscriptionsRepository;
        this.licensesRepository = licensesRepository;
        this.clientsRepository = clientsRepository;
    }

    public abstract void Process(Guid subscriptionId);

    protected Subscriptions GetSubscriptionByMicrosoftId(Guid subscriptionId)
    {
        return subscriptionsRepository.GetByMicrosoftId(subscriptionId.ToString());
    }

    public virtual Task ProcessAsync(Guid subscriptionId)
    {
        throw new NotImplementedException("Este handler no implementa ProcessAsync.");
    }



    protected Licenses GetLicenseBySubscriptionId(Guid subscriptionId)
    {
        return licensesRepository.GetByLicenseKey(subscriptionId.ToString());
    }

    protected Clients GetClientById(string clientId)
    {
        int id = int.Parse(clientId); 
        return clientsRepository.GetByInstallationId(id);
    }

}
