using OperationsKnowledge.Dtos;
using OperationsKnowledge.Models;

namespace OperationsKnowledge.Mappings;

public class OperationalSystemMapper
{
    public static OperationalSystemResponse ToResponse(OperationalSystem system)
    {
        return new OperationalSystemResponse
        {
            Id = system.Id,
            Name = system.Name,
            Status = system.Status,
            Description = system.Description,
            OwnerId = system.OwnerId,
            OwnerName = system.Owner?.Name
        };
    }
}
