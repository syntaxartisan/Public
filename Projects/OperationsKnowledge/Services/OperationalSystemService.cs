using OperationsKnowledge.Models;
using OperationsKnowledge.Data;
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
        return await _context.OperationalSystems.ToListAsync();
    }

    public async Task<OperationalSystem?> GetByIdAsync(int id)
    {
        return await _context.OperationalSystems.FindAsync(id);
    }

    public async Task CreateAsync(OperationalSystem system)
    {
        await _context.OperationalSystems.AddAsync(system);
        _context.SaveChanges();
    }

    public async Task<bool> UpdateAsync(OperationalSystem system)
    {
        var existing = await GetByIdAsync(system.Id);
        if (existing == null) { return false; }
        existing.Name = system.Name;
        existing.Status = system.Status;
        existing.Description = system.Description;
        existing.OwnerId = system.OwnerId;
        existing.Owner = system.Owner;
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) { return false; }
        _context.OperationalSystems.Remove(existing);
        _context.SaveChanges();
        return true;
    }
}
