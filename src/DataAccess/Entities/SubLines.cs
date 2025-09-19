using System;

namespace Marketplace.SaaS.Accelerator.DataAccess.Entities;


public partial class SubLines
{
    public int SubLinesId { get; set; }                      // Clave primaria

    public string MicrosoftId { get; set; }                     // Relación con tabla Subscriptions

    public DateTime ChargeDate { get; set; }                 // Fecha de cargo mensual

    public bool Status { get; set; }                         // Activo/Inactivo

    public string AMPlan { get; set; }                       // Plan adquirido

    public int UsersQ { get; set; }                          // Número de usuarios

    public string Country { get; set; }                      // País del comprador

    public string Currency { get; set; }                     // Moneda

    public decimal? Amount { get; set; }                     // Precio total (calculable más adelante)

}
