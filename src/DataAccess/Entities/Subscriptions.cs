using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Subscriptions
{
    public string MicrosoftId { get; set; } 

    public string SubscriptionStatus { get; set; } 

    public string AMPPlanId { get; set; } 

    public bool? IsActive { get; set; } 

    public int? UserId { get; set; } 

    public string PurchaserEmail { get; set; } 

    public string PurchaserTenantId { get; set; } 

    public string Term { get; set; } 

    public DateTime? StartDate { get; set; } 

    public DateTime? EndDate { get; set; } 

    public bool? AutoRenew { get; set; }

    public string Name { get; set; }


}
