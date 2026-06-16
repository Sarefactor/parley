using Parley.Validation.Enums;

namespace Parley.Validation;

public class ParleyWorkflowValidationError
{
    public Guid WorkflowId { get; private set; }

    private List<ParleyWorkflowValidationErrorDetail> _errorDetails = [];
    public IReadOnlyList<ParleyWorkflowValidationErrorDetail> ErrorDetails => _errorDetails;

    private List<ParleyNodeValidationError> _nodeErrors = [];
    public IReadOnlyList<ParleyNodeValidationError> NodeErrors => _nodeErrors;

    public bool HasErrors => _errorDetails.Count > 0 || _nodeErrors.Count > 0;

    public ParleyWorkflowValidationError(Guid workflowId)
    {
        WorkflowId = workflowId;
    }

    public void AddError(string message, WorkflowErrorType type)
    {
        var errorDetail = new ParleyWorkflowValidationErrorDetail(message, type);
        _errorDetails.Add(errorDetail);
    }

    public void AddNodeError(Guid nodeId, string errorMessage, WorkflowErrorType type)
    {
        var existingNodeError = _nodeErrors.FirstOrDefault(x => x.NodeId == nodeId);

        if (existingNodeError != null)
        {
            existingNodeError.AddError(errorMessage, type);
            return;
        }

        var newNodeError = new ParleyNodeValidationError(nodeId);
        newNodeError.AddError(errorMessage, type);

        _nodeErrors.Add(newNodeError);
    }
}