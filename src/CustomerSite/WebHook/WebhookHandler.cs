using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;



public class WebhookHandler : IWebhookHandler
{
    private readonly ISubscriptionsRepository subscriptionRepository;
    private readonly ILicensesRepository licensesRepository;
    private readonly ISubLinesRepository subLinesRepository;




    private readonly ILogger<WebhookHandler> logger;

    public WebhookHandler(ISubscriptionsRepository subscriptionRepository, ILogger<WebhookHandler> logger, ILicensesRepository licensesRepository, ISubLinesRepository subLinesRepository)
    {
        this.subscriptionRepository = subscriptionRepository;
        this.subLinesRepository = subLinesRepository;
        this.licensesRepository = licensesRepository;
        this.logger = logger;

    }




    public async Task SubscribedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Activating the subscription {payload.SubscriptionId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);
        if (subscription == null)
        {
            logger.LogWarning($"Subscription {payload.SubscriptionId} is not in the DBB. Impossible to active");
            return;
        }

         subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.SubStatus = "Active";
            s.Active = true;
        });


        logger.LogInformation($"Subscription {payload.SubscriptionId} updated to Active");
    }






    public async Task UnsubscribedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Event for cancel the subscription {payload.SubscriptionId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);

        if (subscription == null)
        {
            logger.LogWarning($"Subscription {payload.SubscriptionId} is not in the DBB.");
            return;
        }


        subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.SubStatus = "Unsubscribe";
            s.Active = false;
            s.AutoRenew = false;
        });


        var license = licensesRepository.GetLicenseByMicrosoftId(payload.SubscriptionId);
        if (license != null)
        {
            licensesRepository.UpdateLicense(license.MicrosoftID, l => l.Status = 3);
        }
        else
        {
            logger.LogWarning($"There is no license with MicrosoftId={payload.SubscriptionId}");
        }

        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);

        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow.ToString("yyyyMMddHHmmssff"),
            Status = "Unsubscribe",
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = latestLine.USDTotal
        };

        subLinesRepository.AddNewLine(newLine);

    }






    public async Task ChangeQuantityAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Event for change the quantity on: {payload.SubscriptionId}");

        var license = licensesRepository.GetLicenseByMicrosoftId(payload.SubscriptionId?.Trim());
        if (license == null)
        {
            logger.LogWarning($"Subscription {payload.SubscriptionId} is not in the DBB");
            return;
        }

            licensesRepository.UpdateLicense(license.MicrosoftID, l =>
            {
                l.PurchasedLicenses = payload.Quantity;
            });

        logger.LogInformation($"Subscription: {payload.SubscriptionId} updated with new quantity: {payload.Quantity}");
    }






    public async Task SuspendedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Event for suspending subscription: {payload.SubscriptionId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);
        if (subscription == null)
        {
            logger.LogWarning($"Subscription {payload.SubscriptionId} is not in the DBB.");
            return;
        }

        subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.SubStatus = "Suspended";
            s.Active = false;
            s.AutoRenew = false;
        });


        var license = licensesRepository.GetLicenseByMicrosoftId(payload.SubscriptionId);

        licensesRepository.UpdateLicense(license.MicrosoftID, l =>
            {
                l.Status = 3;
            });


        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);

        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow.ToString("yyyyMMddHHmmssff"),
            Status = "Suspended",
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = latestLine.USDTotal
        };

        subLinesRepository.AddNewLine(newLine);

        logger.LogInformation($"Subscription {payload.SubscriptionId} and licenses are suspended now");
    }






    public async Task ReinstatedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Event for reinstated the subscription: {payload.SubscriptionId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);

        if (subscription == null)
        {
            logger.LogWarning($"Subscription {payload.SubscriptionId} is not in the DBB.");
            return;
        }

        string newExpiry;

        if (subscription.Term == "P1Y")
        {
            newExpiry = DateTime.UtcNow.AddYears(1).ToString("yyyyMMddHHmmssff");
        }
        else
        {
            newExpiry = DateTime.UtcNow.AddMonths(1).ToString("yyyyMMddHHmmssff");
        }

        subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.EndDate = newExpiry;
            s.SubStatus = "Active";
            s.Active = true;
            s.AutoRenew = true;
        });


        var license = licensesRepository.GetLicenseByMicrosoftId(payload.SubscriptionId);

        licensesRepository.UpdateLicense(license.MicrosoftID, l =>
        {
            l.Status = 2;
            l.LicenseExpires = newExpiry;

        });


        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);
        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow.ToString("yyyyMMddHHmmssff"),
            Status = "Reinstated",
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = latestLine.USDTotal
        };

        subLinesRepository.AddNewLine(newLine);

        logger.LogInformation($"Subscription {payload.SubscriptionId} and licenses are active again");
    }




    public async Task RenewedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Event for the renew of the subscription: {payload.SubscriptionId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);

        if (subscription == null)
        {
            logger.LogWarning($"Subscription {payload.SubscriptionId} is not in the DBB.");
            return;
        }

        string newExpiry;

        if (subscription.Term == "P1Y")
        {
            newExpiry = DateTime.UtcNow.AddYears(1).ToString("yyyyMMddHHmmssff");
        }
        else
        {
            newExpiry = DateTime.UtcNow.AddMonths(1).ToString("yyyyMMddHHmmssff");
        }

        subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.EndDate = newExpiry;
            s.SubStatus = "Active";
            s.Active = true;
            s.AutoRenew = true;
        });


        var license = licensesRepository.GetLicenseByMicrosoftId(payload.SubscriptionId);

        licensesRepository.UpdateLicense(license.MicrosoftID, l =>
        {
            l.Status = 2;
            l.LicenseExpires = newExpiry;

        });

        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);

        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow.ToString("yyyyMMddHHmmssff"),
            Status = "Renew",
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = latestLine.USDTotal
        };

        subLinesRepository.AddNewLine(newLine);

        logger.LogInformation($"Susbcription {payload.SubscriptionId} and licenses are now renewed");
    }




    public async Task UnknownActionAsync(AzureWebHookPayLoad payload)
    {
        logger.LogWarning($"[Webhook] Action for the subscription {payload.SubscriptionId} with the state {payload.Status} is not aviable");
    }
}
