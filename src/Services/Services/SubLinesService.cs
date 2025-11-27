using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using System;

public class SubLinesService : ISubLinesService
{
    private readonly ISubLinesRepository subLinesRepository;

    public SubLinesService(ISubLinesRepository repository)
    {
        subLinesRepository = repository;
    }

    // Creates a new SubLine record from subscription input data
    public int CreateFromDataModel(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MicrosoftId))
            throw new ArgumentException("MicrosoftId cannot be empty.");

        var subLine = new SubLines
        {
            MicrosoftID = model.MicrosoftId,
            ChargeDate = DateTime.UtcNow.ToString("yyyyMMddHHmmssff"),
            Status = "Active", 
            PlanTest = model.AMPlan,
            UsersQ = model.UsersQ,
            Country = model.Country,
            Plan = ConvertLicenseType(model.AMPPlanId),
            USDTotal = CalculateTotal(model.AMPPlanId, model.Term, model.UsersQ)
        };

        return subLinesRepository.AddNewLine(subLine);
    }


    private static decimal CalculateTotal(string ampPlanId, string term, int quantity)
    {
        decimal pricePerUser = 0m;

        if (string.Equals(term, "P1M", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(ampPlanId, "atxt001", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 14.99m;
            else if (string.Equals(ampPlanId, "atxt002", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 18.99m;
            else if (string.Equals(ampPlanId, "atxtstd050", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 7.50m;
            else if (string.Equals(ampPlanId, "atxtbus050", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 9.50m;
            else if (string.Equals(ampPlanId, "atxtstd040", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 9.00m;
            else if (string.Equals(ampPlanId, "atxtbus040", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 11.50m;
            else if (string.Equals(ampPlanId, "atxtbus025", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 14.25m;
            else if (string.Equals(ampPlanId, "atxtstd025", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 11.25m;
            else if (string.Equals(ampPlanId, "atxtstd030", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 10.50m;
            else if (string.Equals(ampPlanId, "atxtbus030", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 13.25m;
        }

        else if (string.Equals(term, "P1Y", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(ampPlanId, "atxt001", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 143.90m;
            else if (string.Equals(ampPlanId, "atxt002", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 182.30m;
            else if (string.Equals(ampPlanId, "atxtstd050", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 90.00m;
            else if (string.Equals(ampPlanId, "atxtbus050", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 114.00m;
            else if (string.Equals(ampPlanId, "atxtstd040", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 108.00m;
            else if (string.Equals(ampPlanId, "atxtbus040", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 138.00m;
            else if (string.Equals(ampPlanId, "atxtbus025", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 171.00m;
            else if (string.Equals(ampPlanId, "atxtstd025", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 135.00m;
            else if (string.Equals(ampPlanId, "atxtstd030", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 126.00m;
            else if (string.Equals(ampPlanId, "atxtbus030", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 159.00m;
        }

        return quantity * pricePerUser;
    }



    private static string ConvertLicenseType(string ampPlanId)
    {
        if (string.Equals(ampPlanId, "atxt001", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ampPlanId, "atxtstd025", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ampPlanId, "atxtstd030", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ampPlanId, "atxtstd040", StringComparison.OrdinalIgnoreCase)
            || string.Equals(ampPlanId, "atxtstd050", StringComparison.OrdinalIgnoreCase))
        {
            return "Ant Text 365 Standard";
        }
        else
        {
            return "Ant Text 365 Business";
        }
    }



    // Retrieves all SubLine records associated with a given Microsoft ID
    public SubLines GetHistoryByMicrosoftId(string microsoftId)
    {
        return subLinesRepository.GetByMicrosoftId(microsoftId);
    }
}
