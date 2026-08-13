using OperationsKnowledge.Models;

namespace OperationsKnowledge.Dtos;

public class OperationalSystemResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Status { get; set; } = "";

    public string Description { get; set; } = "";

    public int? OwnerId { get; set; }

    public string? OwnerName { get; set; }
}
