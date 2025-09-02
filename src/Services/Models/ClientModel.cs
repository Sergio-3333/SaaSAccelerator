using System;

namespace Marketplace.SaaS.Accelerator.Services.Models;

public class ClientModel
{
    public int InstallationID { get; set; }
    public int LicenseID { get; set; }
    public string ContactInfoEmail { get; set; }
    public string ContactInfoCompany { get; set; }
    public int? UsageCounter { get; set; }
    public string InternalNote { get; set; }
    public string CampaignGUID { get; set; }
    public String Created { get; set; }
    public String LastAccessed { get; set; }
}

public class SubscriptionModel
{
    public string PurchaserEmail { get; set; }
    public string Name { get; set; }
    public int? Quantity { get; set; }
    public string PurchaserTenantId { get; set; }
    public string MicrosoftId { get; set; }
    public DateTime? StartDate { get; set; }
}
