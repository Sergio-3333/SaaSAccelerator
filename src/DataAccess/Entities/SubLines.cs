using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;


public partial class SubLines
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SubLinesID { get; set; }                      // Clave primaria

    public string MicrosoftID { get; set; }                     // Relación con tabla Subscriptions

    public DateTime ChargeDate { get; set; }                 // Fecha de cargo mensual

    public int Status { get; set; }                         // Activo/Inactivo

    public string Plan { get; set; }                       // Plan adquirido

    public int UsersQ { get; set; }                          // Número de usuarios

    public string Country { get; set; }                      // País del comprador

    public string PlanTest { get; set; }                     // Moneda

    public decimal? USDTotal { get; set; }                     // Precio total (calculable más adelante)

}
