using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Clients
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InstallationID { get; set; } 

    public string MicrosoftId { get; set; } 

    public int LicenseID { get; set; } 

    public string OWAEmail { get; set; } 

    public int? LicenseType { get; set; } 

    public Licenses License { get; set; }

    public string LastAccessed { get; set; }

    public string Created { get; set; }



}
