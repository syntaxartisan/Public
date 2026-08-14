using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public interface IPersonService
{
    Task<IReadOnlyList<Person>> GetAllAsync();
    Task<Person?> GetByIdAsync(int id);
    Task<IReadOnlyList<OperationalSystem>> GetOwnedSystemsAsync(int id);
    Task CreateAsync(Person p);
    Task<bool> UpdateAsync(Person p);
    Task<bool> DeleteAsync(int id);
}
