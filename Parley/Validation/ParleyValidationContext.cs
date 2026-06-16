using Parley.Validation.Enums;

namespace Parley.Validation;

public class ParleyValidationContext
{
    private List<string> _agentErrorMessages = [];
    public IReadOnlyList<string> AgentErrorMessages => _agentErrorMessages;

    private List<ParleyWorkflowValidationError> _workflowErrors = [];
    public IReadOnlyList<ParleyWorkflowValidationError> WorkflowErrors => _workflowErrors;

    public bool HasErrors => _agentErrorMessages.Count > 0 || _workflowErrors.Any(x => x.HasErrors);

    public bool HasCriticalError { get; private set; } = false;

    public void AddAgentError(string message)
        => _agentErrorMessages.Add(message);

    public void AddWorkflowError(Guid workflowId, string errorMessage, WorkflowErrorType type, bool isCriticalError)
    {
        var existingWorkflowError = _workflowErrors.FirstOrDefault(x => x.WorkflowId == workflowId);

        if (existingWorkflowError != null)
        {
            existingWorkflowError.AddError(errorMessage, type);
            return;
        }

        var newWorkflowError = new ParleyWorkflowValidationError(workflowId);
        newWorkflowError.AddError(errorMessage, type);

        _workflowErrors.Add(newWorkflowError);

        if (isCriticalError)
            HasCriticalError = isCriticalError;
    }

    public void AddNodeError(Guid workflowId, Guid nodeId, string errorMessage, WorkflowErrorType type, bool isCriticalError)
    {
        var existingWorkflowError = _workflowErrors.FirstOrDefault(x => x.WorkflowId == workflowId);

        if (existingWorkflowError != null)
        {
            existingWorkflowError.AddNodeError(nodeId, errorMessage, type);
            return;
        }

        var newWorkflowError = new ParleyWorkflowValidationError(workflowId);
        newWorkflowError.AddNodeError(nodeId, errorMessage, type);

        _workflowErrors.Add(newWorkflowError);

        if (isCriticalError)
            HasCriticalError = isCriticalError;
    }
}