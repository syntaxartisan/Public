using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public interface IOperationalSystemService
{
    Task<IReadOnlyList<OperationalSystem>> GetAllAsync();
    Task<OperationalSystem?> GetByIdAsync(int id);
    Task CreateAsync(OperationalSystem system);
    Task<bool> UpdateAsync(OperationalSystem system);
    Task<bool> DeleteAsync(int id);
}
