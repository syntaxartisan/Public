namespace OperationsKnowledge.Common;

public enum OperationResultStatus
{
    Success,
    NotFound,
    InvalidOwner
}

public class OperationResult
{
    public OperationResultStatus Status { get; }
    public OperationResult(OperationResultStatus status)
    {
        Status = status;
    }
}
