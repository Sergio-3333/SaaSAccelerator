using System;
using System.Collections.Generic;
using Marketplace.SaaS.Accelerator.DataAccess;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;
using Marketplace.SaaS.Accelerator.DataAccess.Services;
using Marketplace.SaaS.Accelerator.Services.Models;

namespace Marketplace.SaaS.Accelerator.Services.Services;

public class SubLinesService : ISubLinesService
{
    private readonly ISubLinesRepository subLinesRepository;

    public SubLinesService(ISubLinesRepository repository)
    {
        subLinesRepository = repository;
    }

    public int CreateFromDataModel(SubscriptionInputModel model)
    {
        if (string.IsNullOrWhiteSpace(model.MicrosoftId))
            throw new ArgumentException("MicrosoftId no puede estar vacío.");

        var subLine = new SubLines
        {
            MicrosoftId = model.MicrosoftId,
            ChargeDate = model.ChargeDate,
            Status = true,
            AMPlan = model.AMPlan,
            Quantity = model.Quantity,
            UsersQ = model.UsersQ,
            Country = model.Country,
            Currency = model.Currency,
            Amount = model.Amount ?? 0
        };

        return subLinesRepository.Save(subLine);
    }

    public IEnumerable<SubLines> GetHistoryByMicrosoftId(string microsoftId)
    {
        return subLinesRepository.GetByMicrosoftId(microsoftId);
    }
}
