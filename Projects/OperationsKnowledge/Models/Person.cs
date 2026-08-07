namespace OperationsKnowledge.Models;

public class Person
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Department { get; set; } = "";

    public string Email { get; set; } = "";

    public string? PhoneNumber { get; set; }

    public ICollection<OperationalSystem> OwnedSystems { get; set; }
    = new List<OperationalSystem>();
}
