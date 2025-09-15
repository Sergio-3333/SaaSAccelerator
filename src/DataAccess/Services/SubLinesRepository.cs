using System.Collections.Generic;
using System.Linq;
using Marketplace.SaaS.Accelerator.DataAccess.Context;
using Marketplace.SaaS.Accelerator.DataAccess.Contracts;
using Marketplace.SaaS.Accelerator.DataAccess.Entities;

namespace Marketplace.SaaS.Accelerator.DataAccess.Repositories;

public class SubLinesRepository : ISubLinesRepository
{
    private readonly SaasKitContext _context;

    public SubLinesRepository(SaasKitContext context)
    {
        _context = context;
    }

    public IEnumerable<SubLines> Get()
    {
        return _context.SubLines.ToList();
    }

    public SubLines Get(int id)
    {
        return _context.SubLines.FirstOrDefault(s => s.SubLinesId == id);
    }

    public int Save(SubLines entity)
    {
        // En este caso Save actúa igual que AddNewLine: siempre inserta
        _context.SubLines.Add(entity);
        _context.SaveChanges();
        return entity.SubLinesId;
    }

    public void Remove(SubLines entity)
    {
        // No se usa en tu lógica, pero lo dejamos por contrato
        _context.SubLines.Remove(entity);
        _context.SaveChanges();
    }

    public IEnumerable<SubLines> GetByMicrosoftId(int microsoftId)
    {
        return _context.SubLines
            .Where(s => s.MicrosoftId == microsoftId)
            .OrderByDescending(s => s.ChargeDate)
            .ToList();
    }

    public int AddNewLine(SubLines subLine)
    {
        _context.SubLines.Add(subLine);
        _context.SaveChanges();
        return subLine.SubLinesId;
    }
}
