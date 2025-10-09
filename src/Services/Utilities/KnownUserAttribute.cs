using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using System.Linq;
using Marketplace.SaaS.Accelerator.Services.Utilities;

public class KnowCustomerAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly ISubscriptionsRepository subscriptionsRepository;

    public KnowCustomerAttribute(ISubscriptionsRepository subscriptionsRepository)
    {
        this.subscriptionsRepository = subscriptionsRepository;
    }

    // Custom authorization logic to validate if the current user is a known, active customer
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Extract email claim from the authenticated user
        var email = context.HttpContext.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimConstants.CLAIM_EMAILADDRESS)?.Value;

        // Extract tenant ID claim
        var tenantId = context.HttpContext.User?.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        // Extract subscription ID claim
        var microsoftId = context.HttpContext.User?.Claims
            .FirstOrDefault(c => c.Type == "subscriptionId")?.Value;

        // If subscription ID is missing, redirect to AccessDenied
        if (string.IsNullOrEmpty(microsoftId))
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                { "controller", "Account" },
                { "action", "AccessDenied" }
            });
            return;
        }

        // Retrieve subscription from repository
        var subscription = subscriptionsRepository.GetSubscriptionByMicrosoftId(microsoftId);

        // Check if subscription is active
        bool isActive = subscription != null &&
                        subscription.SubStatus == "Subscribed";

        // Validate customer identity against subscription data
        var isValidCustomer = subscription != null
            && subscription.PurEmail == email
            && subscription.PurTenantId == tenantId
            && isActive;

        // If validation fails, redirect to AccessDenied
        if (!isValidCustomer)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                { "controller", "Account" },
                { "action", "AccessDenied" }
            });
        }
    }
}
