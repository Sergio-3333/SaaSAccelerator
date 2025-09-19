using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using System;
using System.Collections.Generic;

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
            MicrosoftId = model.MicrosoftId,
            ChargeDate = model.ChargeDate,
            Status = true, // Always marks the subline as active
            AMPlan = model.AMPlan,
            UsersQ = model.UsersQ,
            Country = model.Country,
            Currency = model.Currency,
            Amount = model.Amount ?? 0 // Defaults to 0 if null
        };

        return subLinesRepository.Save(subLine);
    }

    // Retrieves all SubLine records associated with a given Microsoft ID
    public IEnumerable<SubLines> GetHistoryByMicrosoftId(string microsoftId)
    {
        return subLinesRepository.GetByMicrosoftId(microsoftId);
    }
}
