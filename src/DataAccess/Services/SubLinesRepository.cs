using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

// Repository for the SubLines entity.
// Provides methods to insert, retrieve, and delete subline records.
// Uses EF Core to interact with the SaasKitContext database.

public class SubLinesRepository : ISubLinesRepository
{
    private readonly SaasKitContext _context;

    // Constructor that injects the database context.
    public SubLinesRepository(SaasKitContext context)
    {
        _context = context;
    }


    // Retrieves all sublines associated with a given MicrosoftId,
    // ordered by ChargeDate descending.
    public SubLines GetByMicrosoftId(string microsoftId) =>
            _context.SubLines
                .Where(s => s.MicrosoftID == microsoftId)
                .OrderByDescending(s => s.SubLinesID)
                .FirstOrDefault();


    // Inserts a new subline into the database.
    public int AddNewLine(SubLines subLine)
    {
        _context.SubLines.Add(subLine);
        _context.SaveChanges();
        return subLine.SubLinesID;
    }
}
