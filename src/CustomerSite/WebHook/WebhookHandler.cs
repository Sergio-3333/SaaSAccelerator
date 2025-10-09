using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;


public class WebhookHandler : IWebhookHandler
{
    private readonly ISubscriptionsRepository subscriptionRepository;
    private readonly ILicensesRepository licensesRepository;
    private readonly ISubLinesRepository subLinesRepository;

    private readonly SaasKitContext _context;



    private readonly ILogger<WebhookHandler> logger;

    public WebhookHandler(SaasKitContext _context, ISubscriptionsRepository subscriptionRepository, ILogger<WebhookHandler> logger, ILicensesRepository licensesRepository, ISubLinesRepository subLinesRepository)
    {
        this.subscriptionRepository = subscriptionRepository;
        this.subLinesRepository = subLinesRepository;
        this.licensesRepository = licensesRepository;
        this.logger = logger;
        this._context = _context;

    }




    public async Task SubscribedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Activando automáticamente suscripción {payload.SubscriptionId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);
        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {payload.SubscriptionId} no encontrada en la BDD. No se puede activar.");
            return;
        }

         subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.SubStatus = "Active";
            s.Active = true;
        });


        logger.LogInformation($"Suscripción {payload.SubscriptionId} actualizada a estado Active.");
    }






    public async Task UnsubscribedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Cancelación de suscripción {payload.SubscriptionId}");

        // 1️⃣ Actualizar la suscripción principal
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);

        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {payload.SubscriptionId} no encontrada en la BDD.");
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
            logger.LogWarning($"No se encontró licencia con MicrosoftId={payload.SubscriptionId}");
        }

        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);

        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow,
            Status = 0,
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = 0
        };

        // 3️⃣ Guardar en la BDD
        subLinesRepository.AddNewLine(newLine);

    }






    public async Task ChangeQuantityAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Cambio de cantidad en licencia de suscripción {payload.SubscriptionId}");

        // 1️⃣ Recuperar la licencia asociada a la suscripción
        var license = licensesRepository.GetLicenseByMicrosoftId(payload.SubscriptionId?.Trim());
        if (license == null)
        {
            logger.LogWarning($"No se encontró licencia para la suscripción {payload.SubscriptionId}");
            return;
        }

            licensesRepository.UpdateLicense(license.MicrosoftID, l =>
            {
                l.PurchasedLicenses = payload.Quantity;
            });

        logger.LogInformation($"Licencia de suscripción {payload.SubscriptionId} actualizada con nueva cantidad: {payload.Quantity}");
    }






    public async Task SuspendedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook] Suspensión de suscripción {payload.SubscriptionId}");

        // 1️⃣ Actualizar la suscripción
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);
        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {payload.SubscriptionId} no encontrada en la BDD.");
            return;
        }

        subscriptionRepository.UpdateSubscription(subscription.MicrosoftID, s =>
        {
            s.SubStatus = "Suspended";
            s.Active = false;
            s.AutoRenew = false;
        });


        // 2️⃣ Actualizar licencias asociadas
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
            ChargeDate = DateTime.UtcNow,
            Status = 0,
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = 0
        };

        // 3️⃣ Guardar en la BDD
        subLinesRepository.AddNewLine(newLine);

        logger.LogInformation($"Suscripción {payload.SubscriptionId} y licencias asociadas marcadas como suspendidas.");
    }






    public async Task ReinstatedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook]Reinstaurada la suscripcion: {payload.SubscriptionId}");

        // 1️⃣ Actualizar la suscripción
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);

        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {payload.SubscriptionId} no encontrada en la BDD.");
            return;
        }

        DateTime newExpiry;

        if (subscription.Term == "P1Y")
        {
            newExpiry = DateTime.UtcNow.AddYears(1);
        }
        else
        {
            newExpiry = DateTime.UtcNow.AddMonths(1);
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
            l.LicenseExpires = newExpiry.ToString("yyyyMMddHHmmss");

        });


        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);
        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow,
            Status = 1,
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = latestLine.USDTotal
        };

        // 3️⃣ Guardar en la BDD
        subLinesRepository.AddNewLine(newLine);

        logger.LogInformation($"Suscripción {payload.SubscriptionId} y licencias asociadas marcadas como suspendidas.");
    }




    public async Task RenewedAsync(AzureWebHookPayLoad payload)
    {
        logger.LogInformation($"[Webhook]Reinstaurada la suscripcion: {payload.SubscriptionId}");

        // 1️⃣ Actualizar la suscripción
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(payload.SubscriptionId);

        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {payload.SubscriptionId} no encontrada en la BDD.");
            return;
        }

        DateTime newExpiry;

        if (subscription.Term == "P1Y")
        {
            newExpiry = DateTime.UtcNow.AddYears(1);
        }
        else
        {
            newExpiry = DateTime.UtcNow.AddMonths(1);
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
            l.LicenseExpires = newExpiry.ToString("yyyyMMddHHmmss");

        });

        var existingLine = subLinesRepository.GetByMicrosoftId(payload.SubscriptionId);

        var latestLine = existingLine;
        var newLine = new SubLines
        {
            MicrosoftID = latestLine.MicrosoftID,
            ChargeDate = DateTime.UtcNow,
            Status = 1,
            PlanTest = latestLine.PlanTest,
            UsersQ = latestLine.UsersQ,
            Country = latestLine.Country,
            Plan = latestLine.Plan,
            USDTotal = latestLine.USDTotal
        };

        // 3️⃣ Guardar en la BDD
        subLinesRepository.AddNewLine(newLine);

        logger.LogInformation($"Suscripción {payload.SubscriptionId} y licencias asociadas marcadas como suspendidas.");
    }




    public async Task UnknownActionAsync(AzureWebHookPayLoad payload)
    {
        logger.LogWarning($"[Webhook] Acción desconocida para suscripción {payload.SubscriptionId} con estado {payload.Status}");
        // Aquí podrías registrar el evento en una tabla de auditoría
    }
}
