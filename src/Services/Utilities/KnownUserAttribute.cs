using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using System.Linq;
using Marketplace.SaaS.Accelerator.Services.Utilities;

public class ValidCustomerAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly ISubscriptionsRepository subscriptionsRepository;

    public ValidCustomerAttribute(ISubscriptionsRepository subscriptionsRepository)
    {
        this.subscriptionsRepository = subscriptionsRepository;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var email = context.HttpContext.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimConstants.CLAIM_EMAILADDRESS)?.Value;

        var tenantId = context.HttpContext.User?.Claims
            .FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

        var activeSubscriptions = subscriptionsRepository.GetActiveSubscriptions();

        var isValidCustomer = activeSubscriptions.Any(s =>
            s.PurchaserEmail == email &&
            s.PurchaserTenantId == tenantId);

        if (!isValidCustomer)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary {
                { "controller", "Account" },
                { "action", "AccessDenied" }
            });
        }
    }
}
