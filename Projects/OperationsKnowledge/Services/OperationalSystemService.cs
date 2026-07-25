using OperationsKnowledge.Models;
using OperationsKnowledge.Data;

namespace OperationsKnowledge.Services;

public class OperationalSystemService : IOperationalSystemService
{
    private readonly OperationalSystemContext _context;

    public OperationalSystemService(OperationalSystemContext context)
    {
        _context = context;
    }

    public IEnumerable<OperationalSystem> GetAll()
    {
        return _context.OperationalSystems.ToList();
    }

    public OperationalSystem? GetById(int id)
    {
        return _context.OperationalSystems.Find(id);
    }

    public OperationalSystem? Create(OperationalSystem system)
    {
        _context.OperationalSystems.Add(system);
        _context.SaveChanges();
        return system;
    }

    public OperationalSystem? Update(OperationalSystem system)
    {
        var existing = GetById(system.Id);
        if (existing == null) { return null; }
        existing.Name = system.Name;
        existing.Status = system.Status;
        existing.Description = system.Description;
        existing.Owner = system.Owner;
        _context.SaveChanges();
        return existing;
    }

    public bool Delete(int id)
    {
        var system = GetById(id);
        if (system == null) { return false; }
        _context.OperationalSystems.Remove(system);
        _context.SaveChanges();
        return true;
    }
}
