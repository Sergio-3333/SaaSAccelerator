using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;


public partial class SubLines

{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubLinesID { get; set; }                      

    public string MicrosoftID { get; set; }                     

    public string ChargeDate { get; set; }                

    public string Status { get; set; }                         

    public string Plan { get; set; }                      

    public int UsersQ { get; set; }                         

    public string Country { get; set; }                      

    public string PlanTest { get; set; }                    

    public decimal? USDTotal { get; set; }                   

}
