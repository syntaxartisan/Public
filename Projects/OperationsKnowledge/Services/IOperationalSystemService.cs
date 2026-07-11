using OperationsKnowledge.Models;

namespace OperationsKnowledge.Services;

public interface IOperationalSystemService
{
    IEnumerable<OperationalSystem> GetAll();
    OperationalSystem? GetById(int id);
}
