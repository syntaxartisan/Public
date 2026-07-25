namespace OperationsKnowledge.Dtos;

public class OperationalSystemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public string Status { get; set; } = "";

    public string Description { get; set; } = "";
    public string Owner { get; set; } = "";
}
