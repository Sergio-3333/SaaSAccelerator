namespace Marketplace.SaaS.Accelerator.Services.Models;


public class LicenseResult

{
    public int LicenseID { get; set; }
    public string LicenseKey { get; set; }
    public string Company { get; set; }
    public string Email { get; set; }
    public string LicenseExpires { get; set; }
    public int PurchasedLicenses { get; set; }
    public int Status { get; set; }
    public string Comment { get; set; }
}
