using System;
using System.Text.Json.Serialization;
using Marketplace.SaaS.Accelerator.Services.Models.Attributes;


namespace Marketplace.SaaS.Accelerator.DataAccess;

public class SubscriptionInputModel
{

    [JsonPropertyName("Id")] 
    public string MicrosoftId { get; set; }

    public int LicenseId { get; set;  }

    public int SubID { get; set; }


    [JsonPropertyName("saasSubscriptionStatus")]
    public string Status { get; set; }


    [JsonPropertyName("planId")] 
    public string AMPPlanId { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    [JsonPropertyName("emailId")] 
    public string PurchaserEmail { get; set; }

    [JsonPropertyName("tenantId")] 
    public string PurchaserTenantId { get; set; }

    [JsonPropertyName("term")]
    public string Term { get; set; }

    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    [JsonPropertyName("autoRenew")]
    public bool? AutoRenew { get; set; }



    [JsonPropertyName("displayName")]
    public string Name { get; set; }

    [JsonPropertyName("mail")] 
    public string Email { get; set; }

    [JsonPropertyName("mobilePhone")] 
    public string Phone { get; set; }

    [JsonPropertyName("businessPhone")]
    public string Mobile { get; set; }

    [JsonPropertyName("companyName")]
    public string Company { get; set; }

    [JsonPropertyName("city")] 
    public string City { get; set; }

    [JsonPropertyName("street")]
    public string Adr1 { get; set; }

    [JsonPropertyName("postalCode")]
    public string Zip { get; set; }



    [JsonPropertyName("chargeDate")]
    public DateTime? ChargeDate { get; set; }

    [JsonPropertyName("planName")] 
    public string AMPlan { get; set; }


    [JsonPropertyName("Quantity")] 
    public int UsersQ { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("currencyCode")]
    public string Currency { get; set; }

    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }


    [FromRequestHeader("x-ms-requestid")]
    public string RequestID { get; set; }
}
