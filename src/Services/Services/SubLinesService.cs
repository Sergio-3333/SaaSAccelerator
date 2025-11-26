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

        if (string.Equals(ampPlanId, "atxt001", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(term, "P1M", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 14.99m;
            else if (string.Equals(term, "P1Y", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 143.90m;
        }
        else if (string.Equals(ampPlanId, "atxt002", StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(term, "P1M", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 18.99m;
            else if (string.Equals(term, "P1Y", StringComparison.OrdinalIgnoreCase))
                pricePerUser = 182.30m;
        }

        return quantity * pricePerUser;
    }


    private static string ConvertLicenseType(string ampPlanId)
    {
        if (string.Equals(ampPlanId, "atxt001", StringComparison.OrdinalIgnoreCase))
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
