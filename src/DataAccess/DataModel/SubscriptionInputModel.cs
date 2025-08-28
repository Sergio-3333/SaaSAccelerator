using System;
namespace Marketplace.SaaS.Accelerator.DataAccess;

public class SubscriptionInputModel
{
    public string MicrosoftId { get; set; }
    public string AMPPlanId { get; set; }
    public int AMPQuantity { get; set; }
    public string PurchaserEmail { get; set; }
    public string Name { get; set; }
}

