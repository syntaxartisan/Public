using OperationsKnowledge.Models;

namespace OperationsKnowledge.Dtos;

public class PersonResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Department { get; set; } = "";

    public string Email { get; set; } = "";

    public string? PhoneNumber { get; set; }
}
