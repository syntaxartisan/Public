using OperationsKnowledge.Common;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public interface IOperationalSystemService
{
    Task<IReadOnlyList<OperationalSystem>> GetAllAsync();
    Task<OperationalSystem?> GetByIdAsync(int id);
    Task<OperationResult> CreateAsync(OperationalSystem system);
    Task<OperationResult> UpdateAsync(OperationalSystem system);
    Task<bool> DeleteAsync(int id);
}
