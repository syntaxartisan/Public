using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public interface IOperationalSystemService
{
    IEnumerable<OperationalSystem> GetAll();
    OperationalSystem? GetById(int id);
    OperationalSystem? Create(OperationalSystem system);
    OperationalSystem? Update(OperationalSystem system);
    bool Delete(int id);
}
