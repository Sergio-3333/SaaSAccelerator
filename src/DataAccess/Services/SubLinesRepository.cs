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

    // Retrieves all sublines from the database.
    public IEnumerable<SubLines> Get()
    {
        return _context.SubLines.ToList();
    }

    // Retrieves a single subline by its ID.
    public SubLines Get(int id)
    {
        return _context.SubLines.FirstOrDefault(s => s.SubLinesId == id);
    }

    // Inserts a new subline into the database.
    // Equivalent to AddNewLine — always performs an insert.
    public int Save(SubLines entity)
    {
        _context.SubLines.Add(entity);
        _context.SaveChanges();
        return entity.SubLinesId;
    }

    // Removes a subline from the database.
    // Not used in current logic, but included for contract completeness.
    public void Remove(SubLines entity)
    {
        _context.SubLines.Remove(entity);
        _context.SaveChanges();
    }

    // Retrieves all sublines associated with a given MicrosoftId,
    // ordered by ChargeDate descending.
    public SubLines GetByMicrosoftId(string microsoftId) =>
            _context.SubLines
                .Where(s => s.MicrosoftId == microsoftId)
                .OrderByDescending(s => s.SubLinesId)
                .FirstOrDefault();


    // Inserts a new subline into the database.
    public int AddNewLine(SubLines subLine)
    {
        _context.SubLines.Add(subLine);
        _context.SaveChanges();
        return subLine.SubLinesId;
    }
}
