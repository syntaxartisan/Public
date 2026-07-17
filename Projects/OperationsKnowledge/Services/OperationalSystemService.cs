using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public class OperationalSystemService : IOperationalSystemService
{
    private int _nextId = 3;
    private readonly List<OperationalSystem> _systems =
        [
            new()
            {
                Id = 1,
                Name = "Payroll",
                Status = "Active",
                Description = "Processes employee payroll."
            },
            new()
            {
                Id = 2,
                Name = "Monitoring",
                Status = "Active",
                Description = "Infrastructure monitoring."
            }
        ];
    
    public IEnumerable<OperationalSystem> GetAll()
    {
        return _systems;
    }

    public OperationalSystem? GetById(int id)
    {
        return _systems.FirstOrDefault(s => s.Id == id);
    }

    public OperationalSystem? Create(OperationalSystem system)
    {
        system.Id = _nextId++;
        _systems.Add(system);
        return system;
    }

    public OperationalSystem? Update(OperationalSystem system)
    {
        var existing = _systems.FirstOrDefault(s => s.Id == system.Id);
        if (existing == null) { return null; }
        existing.Name = system.Name;
        existing.Status = system.Status;
        existing.Description = system.Description;
        existing.Owner = system.Owner;
        return system;
    }

    public bool Delete(int id)
    {
        int index = _systems.FindIndex(s => s.Id == id);
        if (index == -1) { return false; }
        _systems.RemoveAt(index);
        return true;
    }
}
