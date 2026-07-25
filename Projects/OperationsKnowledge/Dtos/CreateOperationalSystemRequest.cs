namespace OperationsKnowledge.Dtos;

public class CreateOperationalSystemRequest
{
    public string Name { get; set; } = "";

    public string Status { get; set; } = "";

    public string Description { get; set; } = "";
    public string Owner { get; set; } = "";
}
