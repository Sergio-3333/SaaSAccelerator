using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Clients
{
    public int InstallationID { get; set; } 

    public string MicrosoftId { get; set; } 

    public int LicenseID { get; set; } 

    public string OWAEmail { get; set; } 

    public int? LicenseType { get; set; } 

    public Licenses License { get; set; }

}
