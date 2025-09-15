// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Marketplace.Saas.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Repositories;
using Marketplace.SaaS.Accelerator.Services.Contracts;
using Marketplace.SaaS.Accelerator.Services.Exceptions;
using Marketplace.SaaS.Accelerator.Services.Models;
using Marketplace.SaaS.Accelerator.Services.Services;
using Marketplace.SaaS.Accelerator.Services.StatusHandlers;
using Marketplace.SaaS.Accelerator.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Marketplace.SaaS.Models;

namespace Marketplace.SaaS.Accelerator.CustomerSite.Controllers;

/// <summary>Home Controller.</summary>
/// <seealso cref="BaseController"/>
public class HomeController : BaseController
{
    private readonly IFulfillmentApiService apiService;
    private readonly ISubscriptionsRepository subscriptionsRepository;
    private readonly IClientsRepository clientsRepository;
    private readonly ILicensesRepository licensesRepository;

    private readonly LicenseService licenseService;

    private readonly ISubscriptionStatusHandler unsubscribeStatusHandler;
    private readonly ISubscriptionStatusHandler pendingActivationStatusHandler;
    private readonly ISubscriptionStatusHandler pendingFulfillmentStatusHandler;

    public HomeController(
        IFulfillmentApiService apiService,
        ISubscriptionsRepository subscriptionsRepository,
        IClientsRepository clientsRepository,
        ILicensesRepository licensesRepository,
        ISubscriptionStatusHandler unsubscribeStatusHandler,
        ISubscriptionStatusHandler pendingActivationStatusHandler,
        ISubscriptionStatusHandler pendingFulfillmentStatusHandler,
        IAppVersionService appVersionService)
        : base(appVersionService)
    {
        this.apiService = apiService;
        this.subscriptionsRepository = subscriptionsRepository;
        this.clientsRepository = clientsRepository;
        this.licensesRepository = licensesRepository;

        this.unsubscribeStatusHandler = unsubscribeStatusHandler;
        this.pendingActivationStatusHandler = pendingActivationStatusHandler;
        this.pendingFulfillmentStatusHandler = pendingFulfillmentStatusHandler;
    }


    public async Task<IActionResult> Index(string token = null)
    {
        try
        {
            SubscriptionResultExtension subscriptionExtension = new SubscriptionResultExtension();

            if (User.Identity.IsAuthenticated)
            {
                var userDetail = GetCurrentUserDetail();

                if (!string.IsNullOrEmpty(token))
                {
                    token = token.Replace(' ', '+');
                    var newSubscription = await apiService.ResolveAsync(token).ConfigureAwait(false);

                    if (newSubscription != null && newSubscription.SubscriptionId != default)
                    {
                        var subscriptionData = await apiService.GetSubscriptionByIdAsync(newSubscription.SubscriptionId).ConfigureAwait(false);
                        // 1. Insertar o actualizar suscripción
                        var subscription = new Subscriptions
                        {
                            MicrosoftId = subscriptionData.Id.ToString(),
                            SubscriptionStatus = subscriptionData.SaasSubscriptionStatus.ToString(),
                            AMPPlanId = subscriptionData.PlanId,
                            IsActive = true,
                            CreateBy = null,
                            CreateDate = DateTime.UtcNow,
                            ModifyDate = null, 
                            UserId = subscriptionData.Beneficiary.Puid.GetHashCode(),
                            Name = subscriptionData.Name,
                            AMPQuantity = subscriptionData.Quantity.GetHashCode(),
                            PurchaserEmail = subscriptionData.Beneficiary.EmailId,
                            PurchaserTenantId = subscriptionData.Beneficiary.TenantId.ToString(),
                            AmpOfferId = subscriptionData.OfferId,
                            Term = subscriptionData.Term.TermUnit.ToString(),
                            StartDate = subscriptionData.Term?.StartDate?.DateTime,
                            EndDate = subscriptionData.Term?.EndDate?.DateTime
                        };

                        int subscriptionId = subscriptionsRepository.Save(subscription);


                        // 2. Crear licencia
                        var license = new LicenseResult
                        {
                            MicrosoftId = subscriptionData.Id.ToString(),
                            LicenseKey = Guid.NewGuid().ToString(),

                            Email = subscriptionData.Purchaser.EmailId,

                            Status = 1, // Activo

                            WebOrderID = subscriptionData.WebOrderId,
                            LicenseExpires = subscriptionData.Term?.EndDate?.ToString("yyyy-MM-dd"),

                            LicensesStd = subscriptionData.PlanId.GetHashCode() == 1 ? 1 : 0,
                            LicensesBiz = subscriptionData.PlanId.GetHashCode() == 1 ? 0 : 1
                        };


                        int licenseId = licenseService.SaveLicense(license);

                        // 3. Generar installationId (temporal o real)
                        int installationId = new Random().Next(1000, 9999); // o tu lógica real

                        // 4. Insertar o actualizar cliente
                        ClientsService.CreateOrUpdateClientFromSubscription(subscriptionData, licenseId, installationId);

                        // 6. Preparar datos para la vista
                        subscriptionExtension = subscriptionService.GetSubscriptionsBySubscriptionId(newSubscription.SubscriptionId, true);
                        subscriptionExtension.ShowWelcomeScreen = false;
                        subscriptionExtension.CustomerEmailAddress = userDetail.EmailAddress;
                        subscriptionExtension.CustomerName = userDetail.FullName;
                    }
                }
                else
                {
                    TempData["ShowWelcomeScreen"] = "True";
                    subscriptionExtension.ShowWelcomeScreen = true;
                }

                return View(subscriptionExtension);
            }
            else
            {
                if (!string.IsNullOrEmpty(token))
                {
                    return Challenge(
                        new AuthenticationProperties { RedirectUri = "/?token=" + token },
                        OpenIdConnectDefaults.AuthenticationScheme);
                }

                TempData["ShowWelcomeScreen"] = "True";
                subscriptionExtension.ShowWelcomeScreen = true;
                return View(subscriptionExtension);
            }
        }
        catch (Exception ex)
        {
            logger.LogError($"Message:{ex.Message} :: {ex.InnerException}");
            return View("Error", ex);
        }
    }



    /// <summary>
    /// Subscription this instance.
    /// </summary>
    /// <returns> Subscription instance.</returns>
    public IActionResult Subscriptions()
    {
        this.logger.Info("Home Controller / Subscriptions ");
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                this.TempData["ShowWelcomeScreen"] = "True";
                SubscriptionViewModel subscriptionDetail = new SubscriptionViewModel();
                subscriptionDetail.Subscriptions = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, default, true).ToList();
                foreach (var subscription in subscriptionDetail.Subscriptions)
                {
                    Plans planDetail = this.planRepository.GetById(subscription.PlanId);
                    subscription.IsAutomaticProvisioningSupported = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported"));
                    subscription.AcceptSubscriptionUpdates = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("AcceptSubscriptionUpdates"));
                    subscription.IsPerUserPlan = planDetail.IsPerUser.HasValue ? planDetail.IsPerUser.Value : false;
                }

                subscriptionDetail.SaaSAppUrl = this.apiService.GetSaaSAppURL();

                if (this.TempData["ErrorMsg"] != null)
                {
                    subscriptionDetail.IsSuccess = false;
                    subscriptionDetail.ErrorMessage = Convert.ToString(this.TempData["ErrorMsg"]);
                }

                return this.View(subscriptionDetail);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Get All Subscription List for Current Logged in User.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    public IActionResult SubscriptionDetail(Guid subscriptionId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionDetail subscriptionId:{subscriptionId}"));
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                if (subscriptionDetail == null)
                {
                    this.logger.LogError($"Cannot find subscription or subscription associated to the current user");
                    return this.RedirectToAction(nameof(this.Index));
                }
                subscriptionDetail.PlanList = this.subscriptionService.GetAllSubscriptionPlans();

                return this.PartialView(subscriptionDetail);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Get Subscription Details for selected Subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    public IActionResult SubscriptionQuantityDetail(Guid subscriptionId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionQuantityDetail subscriptionId:{subscriptionId}"));
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                if (subscriptionDetail == null)
                {
                    this.logger.LogError($"Cannot find subscription or subscription associated to the current user");
                    return this.RedirectToAction(nameof(this.Index));
                }
                return this.PartialView(subscriptionDetail);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptions the log detail.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <returns> Subscription log detail.</returns>
    public IActionResult SubscriptionLogDetail(Guid subscriptionId)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionQuantityDetail subscriptionId:{subscriptionId}"));
        try
        {
            if (this.User.Identity.IsAuthenticated)
            {
                // Validate subscription from same customer
                var subscriptionDetail = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                if(subscriptionDetail == null)
                {
                    this.logger.LogError($"Cannot find subscription or subscription associated to the current user");
                    return this.RedirectToAction(nameof(this.Index));
                }

                List<SubscriptionAuditLogs> subscriptionAudit = new List<SubscriptionAuditLogs>();
                subscriptionAudit = this.subscriptionLogRepository.GetSubscriptionBySubscriptionId(subscriptionId).ToList();
                return this.PartialView(subscriptionAudit);
            }
            else
            {
                return this.RedirectToAction(nameof(this.Index));
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// The Error.
    /// </summary>
    /// <returns>
    /// The <see cref="IActionResult" />.
    /// </returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionDetail = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
        return this.View(exceptionDetail?.Error);
    }

    /// <summary>
    /// Processes the message.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="status">The status.</param>
    /// <returns>
    /// Return View.
    /// </returns>
    public IActionResult ProcessMessage(string action, string status)
    {
        try
        {
            if (status.Equals("Activate"))
            {
                return this.PartialView();
            }
            else
            {
                return this.View();
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Home Controller / ActivatedMessage Exception: {ex.Message}");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptions the details.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <param name="operation">The operation.</param>
    /// <returns> Subscription Detials.</returns>
    public IActionResult SubscriptionDetails(Guid subscriptionId, string planId, string operation)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ActivateSubscription subscriptionId:{subscriptionId} :: planId : {planId} :: operation:{operation}"));
        try
        {
            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();
            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddUser(this.GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                this.TempData["ShowWelcomeScreen"] = false;

                subscriptionDetail = this.subscriptionService.GetSubscriptionsBySubscriptionId(subscriptionId);
                var planDetails = this.planRepository.GetById(subscriptionDetail.PlanId);
                var subscriptionParmaeters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, planDetails.PlanGuid);
                var inputParanetrs = subscriptionParmaeters.Where(s => s.Type.ToLower() == "input");
                if (inputParanetrs != null && inputParanetrs.ToList().Count() > 0)
                {
                    subscriptionDetail.SubscriptionParameters = inputParanetrs.ToList();
                }

                subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                subscriptionDetail.CustomerName = this.CurrentUserName;
                subscriptionDetail.IsAutomaticProvisioningSupported = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported"));
            }

            return this.View("Index", subscriptionDetail);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
            return this.View("Error", ex);
        }
    }

    /// <summary>
    /// Subscriptions the operation.
    /// </summary>
    /// <param name="subscriptionResultExtension">The subscription result extension.</param>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <param name="operation">The operation.</param>
    /// <returns>
    /// Subscriptions operation.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubscriptionOperationAsync(SubscriptionResultExtension subscriptionResultExtension, Guid subscriptionId, string planId, string operation)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / SubscriptionOperation subscriptionId:{subscriptionId} :: planId : {planId} :: operation:{operation}"));
        if (this.User.Identity.IsAuthenticated)
        {
            try
            {
                var userDetails = this.userRepository.GetPartnerDetailFromEmail(this.CurrentUserEmailAddress);

                if (subscriptionId != default)
                {
                    this.logger.Info("GetPartnerSubscription");
                    var oldValue = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId, true).FirstOrDefault();
                    if (oldValue == null)
                    {
                        this.logger.LogError($"Cannot find subscription or subscription associated to the current user");
                        return this.RedirectToAction(nameof(this.Index));
                    }
                    Plans planDetail = this.planRepository.GetById(oldValue.PlanId);
                    this.logger.Info("GetUserIdFromEmailAddress");
                    var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                    if (operation == "Activate")
                    {
                        try
                        {
                            this.logger.Info(HttpUtility.HtmlEncode($"Save Subscription Parameters:  {JsonSerializer.Serialize(subscriptionResultExtension.SubscriptionParameters)}" ));
                            if (subscriptionResultExtension.SubscriptionParameters != null && subscriptionResultExtension.SubscriptionParameters.Count() > 0)
                            {
                                var inputParms = subscriptionResultExtension.SubscriptionParameters.ToList().Where(s => s.Type.ToLower() == "input");
                                if (inputParms != null)
                                {
                                    var inputParmsList = inputParms.ToList();
                                    this.subscriptionService.AddSubscriptionParameters(inputParmsList, currentUserId);
                                }
                            }

                            if (Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported")))
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"UpdateStateOfSubscription PendingActivation: SubscriptionId: {subscriptionId} "));
                                if (oldValue.SubscriptionStatus.ToString() != SubscriptionStatusEnumExtension.PendingActivation.ToString())
                                {
                                    this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnumExtension.PendingActivation.ToString(), true);
                                    if (oldValue != null)
                                    {
                                        SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                                        {
                                            Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                                            SubscriptionId = oldValue.SubscribeId,
                                            NewValue = SubscriptionStatusEnumExtension.PendingActivation.ToString(),
                                            OldValue = oldValue.SubscriptionStatus.ToString(),
                                            CreateBy = currentUserId,
                                            CreateDate = DateTime.Now,
                                        };
                                        this.subscriptionLogRepository.Save(auditLog);
                                    }
                                }

                                this.pendingActivationStatusHandlers.Process(subscriptionId);
                            }
                            else
                            {
                                this.pendingFulfillmentStatusHandlers.Process(subscriptionId);
                            }
                            
                            await _webNotificationService.PushExternalWebNotificationAsync(subscriptionId, subscriptionResultExtension.SubscriptionParameters);
                        }
                        catch (MarketplaceException fex)
                        {
                            this.logger.Error(fex.Message);
                        }
                    }

                    if (operation == "Deactivate")
                    {
                        this.subscriptionService.UpdateStateOfSubscription(subscriptionId, SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString(), true);
                        if (oldValue != null)
                        {
                            SubscriptionAuditLogs auditLog = new SubscriptionAuditLogs()
                            {
                                Attribute = Convert.ToString(SubscriptionLogAttributes.Status),
                                SubscriptionId = oldValue.SubscribeId,
                                NewValue = SubscriptionStatusEnumExtension.PendingUnsubscribe.ToString(),
                                OldValue = oldValue.SubscriptionStatus.ToString(),
                                CreateBy = currentUserId,
                                CreateDate = DateTime.Now,
                            };
                            this.subscriptionLogRepository.Save(auditLog);
                        }

                        this.unsubscribeStatusHandlers.Process(subscriptionId);
                    }
                }

                this.notificationStatusHandlers.Process(subscriptionId);

                return this.RedirectToAction(nameof(this.ProcessMessage), new { action = operation, status = operation });
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
                return this.View("Error", ex);
            }
        }
        else
        {
            return this.RedirectToAction(nameof(this.Index));
        }
    }

    /// <summary>
    /// Changes the subscription plan.
    /// </summary>
    /// <param name="subscriptionDetail">The subscription detail.</param>
    /// <returns>Changes subscription plan.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeSubscriptionPlan(SubscriptionResult subscriptionDetail)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{ JsonSerializer.Serialize(subscriptionDetail)}"));
        if (this.User.Identity.IsAuthenticated)
        {
            try
            {
                if (subscriptionDetail.Id != default && !string.IsNullOrEmpty(subscriptionDetail.PlanId))
                {
                    try
                    {
                        //initiate change plan
                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                        
                        var jsonResult = await this.apiService.ChangePlanForSubscriptionAsync(subscriptionDetail.Id, subscriptionDetail.PlanId).ConfigureAwait(false);
                        var changePlanOperationStatus = OperationStatusEnum.InProgress;

                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            int _counter = 0;

                            //loop untill the operation status has moved away from inprogress or notstarted, generally this will be the result of webhooks' action aganist this operation
                            while (OperationStatusEnum.InProgress.Equals(changePlanOperationStatus) || OperationStatusEnum.NotStarted.Equals(changePlanOperationStatus))
                            {
                                var changePlanOperationResult = await this.apiService.GetOperationStatusResultAsync(subscriptionDetail.Id, jsonResult.OperationId).ConfigureAwait(false);
                                changePlanOperationStatus = changePlanOperationResult.Status;

                                this.logger.Info(HttpUtility.HtmlEncode($"Plan Change Progress. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: ***** OperationId: {jsonResult.OperationId} Operationstatus: { changePlanOperationStatus }."));
                                await this.applicationLogService.AddApplicationLog($"Plan Change Progress. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operationstatus: { changePlanOperationStatus }.").ConfigureAwait(false);

                                //wait and check every 5secs
                                await Task.Delay(5000);
                                _counter++;
                                if (_counter > 100)
                                {
                                    //if loop has been executed for more than 100 times then break, to avoid infinite loop just in case
                                    break;
                                }
                            }

                            if (changePlanOperationStatus == OperationStatusEnum.Succeeded)
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"Plan Change Success. SubscriptionId: {subscriptionDetail.Id} ToPlan : {subscriptionDetail.PlanId} UserId: ***** OperationId: {jsonResult.OperationId}."));
                                await this.applicationLogService.AddApplicationLog($"Plan Change Success. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: {currentUserId} OperationId: {jsonResult.OperationId}.").ConfigureAwait(false);
                            }
                            else
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"Plan Change Failed. SubscriptionId: {subscriptionDetail.Id} ToPlan : {subscriptionDetail.PlanId} UserId: ****** OperationId: {jsonResult.OperationId} Operation status { changePlanOperationStatus }."));
                                await this.applicationLogService.AddApplicationLog($"Plan Change Failed. SubscriptionId: {subscriptionDetail.Id} ToPlan: {subscriptionDetail.PlanId} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operation status { changePlanOperationStatus }.").ConfigureAwait(false);

                                throw new MarketplaceException($"Plan change operation failed with operation status {changePlanOperationStatus}.");
                            }
                        }
                    }
                    catch (MarketplaceException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                    }
                }

                return this.RedirectToAction(nameof(this.Subscriptions));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
                return this.View("Error", ex);
            }
        }

        return this.RedirectToAction(nameof(this.Index));
    }

    /// <summary>
    /// Changes the quantity plan.
    /// </summary>
    /// <param name="subscriptionDetail">The subscription detail.</param>
    /// <returns>Changes subscription quantity.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeSubscriptionQuantity(SubscriptionResult subscriptionDetail)
    {
        this.logger.Info(HttpUtility.HtmlEncode($"Home Controller / ChangeSubscriptionPlan  subscriptionDetail:{JsonSerializer.Serialize(subscriptionDetail)}"));
        if (this.User.Identity.IsAuthenticated)
        {
            try
            {
                if (subscriptionDetail != null && subscriptionDetail.Id != default && subscriptionDetail.Quantity > 0)
                {
                    try
                    {
                        //initiate change quantity
                        var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                        
                        if (this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionDetail.Id).FirstOrDefault() == null)
                        {
                            this.logger.LogError($"Cannot find subscription or subscription associated to the current user");
                            return this.RedirectToAction(nameof(this.Index));
                        }

                        var jsonResult = await this.apiService.ChangeQuantityForSubscriptionAsync(subscriptionDetail.Id, subscriptionDetail.Quantity).ConfigureAwait(false);
                        var changeQuantityOperationStatus = OperationStatusEnum.InProgress;
                        if (jsonResult != null && jsonResult.OperationId != default)
                        {
                            int _counter = 0;

                            while (OperationStatusEnum.InProgress.Equals(changeQuantityOperationStatus) || OperationStatusEnum.NotStarted.Equals(changeQuantityOperationStatus))
                            {
                                //loop untill the operation status has moved away from inprogress or notstarted, generally this will be the result of webhooks' action aganist this operation
                                var changeQuantityOperationResult = await this.apiService.GetOperationStatusResultAsync(subscriptionDetail.Id, jsonResult.OperationId).ConfigureAwait(false);
                                changeQuantityOperationStatus = changeQuantityOperationResult.Status;

                                this.logger.Info(HttpUtility.HtmlEncode($"Quantity Change Progress. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: **** OperationId: {jsonResult.OperationId} Operationstatus: { changeQuantityOperationStatus }."));
                                await this.applicationLogService.AddApplicationLog($"Quantity Change Progress. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operationstatus: { changeQuantityOperationStatus }.").ConfigureAwait(false);

                                //wait and check every 5secs
                                await Task.Delay(5000);
                                _counter++;
                                if (_counter > 100)
                                {
                                    //if loop has been executed for more than 100 times then break, to avoid infinite loop just in case
                                    break;
                                }
                            }

                            if (changeQuantityOperationStatus == OperationStatusEnum.Succeeded)
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"Quantity Change Success. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: ***** OperationId: {jsonResult.OperationId}."));
                                await this.applicationLogService.AddApplicationLog($"Quantity Change Success. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: {currentUserId} OperationId: {jsonResult.OperationId}.").ConfigureAwait(false);
                            }
                            else
                            {
                                this.logger.Info(HttpUtility.HtmlEncode($"Quantity Change Failed. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: ***** OperationId: {jsonResult.OperationId} Operationstatus: { changeQuantityOperationStatus }."));
                                await this.applicationLogService.AddApplicationLog($"Quantity Change Failed. SubscriptionId: {subscriptionDetail.Id} ToQuantity: {subscriptionDetail.Quantity} UserId: {currentUserId} OperationId: {jsonResult.OperationId} Operationstatus: { changeQuantityOperationStatus }.").ConfigureAwait(false);
                                    
                                throw new MarketplaceException($"Quantity Change operation failed with operation status {changeQuantityOperationStatus}.");
                            }
                        }
                    }
                    catch (MarketplaceException fex)
                    {
                        this.TempData["ErrorMsg"] = fex.Message;
                        this.logger.LogError($"Message:{fex.Message} :: {fex.InnerException}   ");
                    }
                }

                return this.RedirectToAction(nameof(this.Subscriptions));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
                return this.View("Error", ex);
            }
        }
        else
        {
            return this.RedirectToAction(nameof(this.Index));
        }
    }

    /// <summary>
    /// Views the subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription identifier.</param>
    /// <param name="planId">The plan identifier.</param>
    /// <param name="operation">The operation.</param>
    /// <returns> Subscriptions View. </returns>
    public IActionResult ViewSubscription(Guid subscriptionId, string planId, string operation)
    {
        try
        {
            SubscriptionResultExtension subscriptionDetail = new SubscriptionResultExtension();

            if (this.User.Identity.IsAuthenticated)
            {
                var userId = this.userService.AddUser(this.GetCurrentUserDetail());
                var currentUserId = this.userService.GetUserIdFromEmailAddress(this.CurrentUserEmailAddress);
                this.subscriptionService = new SubscriptionService(this.subscriptionRepository, this.planRepository, userId);
                var planDetails = this.planRepository.GetById(planId);
                this.TempData["ShowWelcomeScreen"] = false;
                subscriptionDetail = this.subscriptionService.GetPartnerSubscription(this.CurrentUserEmailAddress, subscriptionId).FirstOrDefault();
                if (subscriptionDetail == null)
                {
                    this.logger.LogError($"Cannot find subscription or subscription associated to the current user");
                    return this.RedirectToAction(nameof(this.Index));
                }
                subscriptionDetail.ShowWelcomeScreen = false;
                subscriptionDetail.CustomerEmailAddress = this.CurrentUserEmailAddress;
                subscriptionDetail.CustomerName = this.CurrentUserName;
                subscriptionDetail.SubscriptionParameters = this.subscriptionService.GetSubscriptionsParametersById(subscriptionId, planDetails.PlanGuid);
                subscriptionDetail.IsAutomaticProvisioningSupported = Convert.ToBoolean(this.applicationConfigRepository.GetValueByName("IsAutomaticProvisioningSupported"));
            }

            return this.View("Index", subscriptionDetail);
        }
        catch (Exception ex)
        {
            this.logger.LogError($"Message:{ex.Message} :: {ex.InnerException}   ");
            return this.View("Error", ex);
        }
    }


}