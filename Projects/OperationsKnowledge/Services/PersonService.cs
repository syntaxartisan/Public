using OperationsKnowledge.Models;
using OperationsKnowledge.Data;
using Microsoft.EntityFrameworkCore;

namespace OperationsKnowledge.Services;

public class PersonService : IPersonService
{
    private readonly OperationalSystemContext _context;

    public PersonService(OperationalSystemContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Person>> GetAllAsync()
    {
        return await _context.People.ToListAsync();
    }

    public async Task<Person?> GetByIdAsync(int id)
    {
        //return await _context.People.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Id == id);
        return await _context.People.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IReadOnlyList<OperationalSystem>> GetOwnedSystemsAsync(int id)
    {
        return await _context.OperationalSystems.Include(s => s.Owner).Where(s => s.OwnerId == id).ToListAsync();
    }

    public async Task CreateAsync(Person p)
    {
        await _context.People.AddAsync(p);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Person p)
    {
        var existing = await GetByIdAsync(p.Id);
        if (existing == null) { return false; }
        existing.Name = p.Name;
        existing.Department = p.Department;
        existing.Email = p.Email;
        existing.PhoneNumber = p.PhoneNumber;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) { return false; }

        var systems = await _context.OperationalSystems.Where(s => s.OwnerId == id).ToListAsync();
        foreach (var s in systems)
        {
            s.OwnerId = null;
        }

        _context.People.Remove(existing);

        await _context.SaveChangesAsync();
        return true;
    }
}
