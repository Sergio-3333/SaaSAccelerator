using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;

public partial class Subscriptions
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubID { get; set; }

    public string MicrosoftID { get; set; } 

    public string SubStatus { get; set; } 

    public string PlanId { get; set; } 

    public bool? Active { get; set; } 

    public string UserID { get; set; } 

    public string PurEmail { get; set; } 

    public string PurTenantId { get; set; }

    public string Country { get; set; }

    public string Term { get; set; } 

    public DateTime? StartDate { get; set; } 

    public DateTime? EndDate { get; set; } 

    public bool? AutoRenew { get; set; }

    public string SubName { get; set; }


}
