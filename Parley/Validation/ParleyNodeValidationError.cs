using Parley.Validation.Enums;

namespace Parley.Validation;

public class ParleyNodeValidationError
{
    public Guid NodeId { get; private set; }
    private List<ParleyNodeValidationErrorDetail> _errorDetails = [];
    public IReadOnlyList<ParleyNodeValidationErrorDetail> ErrorDetails => _errorDetails;

    public ParleyNodeValidationError(Guid nodeId)
    {
        NodeId = nodeId;
    }

    public void AddError(string message, WorkflowErrorType type)
    {
        var errorDetail = new ParleyNodeValidationErrorDetail(message, type);
        _errorDetails.Add(errorDetail);
    }
}