using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public class OperationalSystemService : IOperationalSystemService
{
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
}
