using OperationsKnowledge.Models;
using OperationsKnowledge.Data;
using OperationsKnowledge.Common;
using Microsoft.EntityFrameworkCore;

namespace OperationsKnowledge.Services;

public class OperationalSystemService : IOperationalSystemService
{
    private readonly OperationalSystemContext _context;

    public OperationalSystemService(OperationalSystemContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OperationalSystem>> GetAllAsync()
    {
        return await _context.OperationalSystems.Include(s => s.Owner).ToListAsync();
    }

    public async Task<OperationalSystem?> GetByIdAsync(int id)
    {
        return await _context.OperationalSystems.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<OperationResult> CreateAsync(OperationalSystem system)
    {
        if (system.OwnerId.HasValue)
        {
            bool ownerExists = await _context.People.AnyAsync(p => p.Id == system.OwnerId);
            if (!ownerExists) { return new OperationResult(OperationResultStatus.InvalidOwner); }
        }
        await _context.OperationalSystems.AddAsync(system);
        await _context.SaveChangesAsync();
        return new OperationResult(OperationResultStatus.Success);
    }

    public async Task<OperationResult> UpdateAsync(OperationalSystem system)
    {
        var existing = await GetByIdAsync(system.Id);
        if (existing == null) { return new OperationResult(OperationResultStatus.NotFound); }
        if (system.OwnerId.HasValue)
        {
            bool ownerExists = await _context.People.AnyAsync(p => p.Id == system.OwnerId);
            if (!ownerExists) { return new OperationResult(OperationResultStatus.InvalidOwner); }
        }
        existing.Name = system.Name;
        existing.Status = system.Status;
        existing.Description = system.Description;
        existing.OwnerId = system.OwnerId;
        await _context.SaveChangesAsync();
        return new OperationResult(OperationResultStatus.Success);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) { return false; }
        _context.OperationalSystems.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
