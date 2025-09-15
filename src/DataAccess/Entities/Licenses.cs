using System.Collections.Generic;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Licenses
{
    public int LicenseID { get; set; } 

    public string MicrosoftId { get; set; } 

    public string LicenseKey { get; set; } 

    public string Company { get; set; } 

    public string City { get; set; }

    public string Name { get; set; }

    public string Email { get; set; } 

    public string Phone { get; set; } 

    public int Status { get; set; } 

    public int PurchasedLicenses { get; set; } 

    public string Created { get; set; } 

    public string LicenseExpires { get; set; } 

    public int? LicensesStd { get; set; } 

    public int? LicensesBiz { get; set; } 

    public ICollection<Clients> Clients { get; set; }



}
