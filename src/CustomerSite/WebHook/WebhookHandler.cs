using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Repositories;
using Marketplace.SaaS.Accelerator.Services.WebHook;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class WebhookHandler : IWebhookHandler
{
    private readonly ISubscriptionsRepository subscriptionRepository;
    private readonly ILicensesRepository licensesRepository;
    private readonly ISubLinesRepository subLinesRepository;
    private readonly SubLinesService subLinesService;


    private readonly ILogger<WebhookHandler> logger;

    public WebhookHandler(ISubscriptionsRepository subscriptionRepository, ILogger<WebhookHandler> logger)
    {
        this.subscriptionRepository = subscriptionRepository;
        this.logger = logger;
    }

    public async Task FulfillmentStartedAsync(SubscriptionInputModel model)
    {
        logger.LogInformation($"[Webhook] Activando automáticamente suscripción {model.MicrosoftId}");

        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(model.MicrosoftId);
        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {model.MicrosoftId} no encontrada en la BDD. No se puede activar.");
            return;
        }

        subscription.SubscriptionStatus = "Active";
        subscriptionRepository.UpdateSubscription(subscription);

        logger.LogInformation($"Suscripción {model.MicrosoftId} actualizada a estado Active.");
    }

    public async Task RenewedAsync(SubscriptionInputModel model)
    {
        logger.LogInformation($"[Webhook] Renovación de suscripción {model.MicrosoftId}");

        // 1️⃣ Actualizar la suscripción
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(model.MicrosoftId);
        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {model.MicrosoftId} no encontrada.");
            return;
        }

        subscription.EndDate = DateTime.UtcNow.AddMonths(1);
        subscription.SubscriptionStatus = "Active";
        subscriptionRepository.UpdateSubscription(subscription);

        // 2️⃣ Actualizar licencias relacionadas
        var licenses = licensesRepository.GetByMicrosoftId(model.MicrosoftId);
        foreach (var license in licenses)
        {
            license.Status = 1;
            license.LicenseExpires = DateTime.UtcNow.AddMonths(1).ToString();
            licensesRepository.UpdateLicense(license);
        }

        // 3️⃣ Crear sublines 
        subLinesService.CreateFromDataModel(model);

        logger.LogInformation($"Suscripción {model.MicrosoftId}, licencias y sublines actualizadas tras renovación.");
    }


    public async Task UnsubscribedAsync(SubscriptionInputModel model)
    {
        logger.LogInformation($"[Webhook] Cancelación de suscripción {model.MicrosoftId}");

        // 1️⃣ Actualizar la suscripción principal
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(model.MicrosoftId);
        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {model.MicrosoftId} no encontrada en la BDD.");
            return;
        }

        subscription.SubscriptionStatus = "Canceled";
        subscriptionRepository.UpdateSubscription(subscription);

        // 2️⃣ Actualizar licencias relacionadas
        var licenses = licensesRepository.GetByMicrosoftId(model.MicrosoftId);
        foreach (var license in licenses)
        {
            license.Status = 2;
            licensesRepository.UpdateLicense(license);
        }


        // 3️⃣ Crear sublines 
        subLinesService.CreateFromDataModel(model);

        logger.LogInformation($"Suscripción {model.MicrosoftId}, licencias y sublines marcadas como canceladas.");
    }



    public async Task ChangeQuantityAsync(SubscriptionInputModel model)
    {
        logger.LogInformation($"[Webhook] Cambio de cantidad en licencia de suscripción {model.MicrosoftId}");

        // 1️⃣ Recuperar la licencia asociada a la suscripción
        var licenses = licensesRepository.GetByMicrosoftId(model.MicrosoftId);
        if (licenses == null)
        {
            logger.LogWarning($"No se encontró licencia para la suscripción {model.MicrosoftId}");
            return;
        }

        // 2️⃣ Actualizar la cantidad
        foreach (var license in licenses)
        {
            license.PurchasedLicenses = model.UsersQ;
            licensesRepository.UpdateLicense(license);
        }

        logger.LogInformation($"Licencia de suscripción {model.MicrosoftId} actualizada con nueva cantidad: {model.UsersQ}");
    }


    public async Task SuspendedAsync(SubscriptionInputModel model)
    {
        logger.LogInformation($"[Webhook] Suspensión de suscripción {model.MicrosoftId}");

        // 1️⃣ Actualizar la suscripción
        var subscription = subscriptionRepository.GetSubscriptionByMicrosoftId(model.MicrosoftId);
        if (subscription == null)
        {
            logger.LogWarning($"Suscripción {model.MicrosoftId} no encontrada en la BDD.");
            return;
        }

        subscription.SubscriptionStatus = "Suspended";
        subscriptionRepository.UpdateSubscription(subscription);

        // 2️⃣ Actualizar licencias asociadas
        var licenses = licensesRepository.GetByMicrosoftId(model.MicrosoftId);
        foreach (var license in licenses)
        {
            license.Status = 2;
            licensesRepository.UpdateLicense(license);
        }

        logger.LogInformation($"Suscripción {model.MicrosoftId} y licencias asociadas marcadas como suspendidas.");
    }


    public async Task UnknownActionAsync(SubscriptionInputModel model)
    {
        logger.LogWarning($"[Webhook] Acción desconocida para suscripción {model.MicrosoftId} con estado {model.Status}");
        // Aquí podrías registrar el evento en una tabla de auditoría
    }
}
