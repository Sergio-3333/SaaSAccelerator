using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Licenses
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LicenseID { get; set; } 

    public string MicrosoftId { get; set; } 

    public string LicenseKey { get; set; } 

    public string Company { get; set; } 

    public string City { get; set; }

    [Column("Contact")]
    public string Name { get; set; }

    public string Email { get; set; } 

    public string Phone { get; set; } 

    public int Status { get; set; } 

    public int PurchasedLicenses { get; set; } 

    public string Created { get; set; } 

    public string LicenseExpires { get; set; } 

    public int? LicensesStd { get; set; } 

    public int? LicensesBiz { get; set; }

    public int PartnerID { get; set; }

    public int ProductID { get; set; }

    public string Adr1 { get; set; }

    public string Adr2 { get; set; }

    public int CountryId { get; set; }

    public string VatNo { get; set; }

    public string GLN { get; set; }

    public string Zip { get; set; }






    public ICollection<Clients> Clients { get; set; }



}
