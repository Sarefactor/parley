using Parley.Validation.Enums;

namespace Parley.Validation;

public class ParleyWorkflowValidationErrorDetail
{
    public ParleyWorkflowValidationErrorDetail(string errorMessage,
                                               WorkflowErrorType type)
    {
        ErrorMessage = errorMessage;
        Type = type;
    }

    public string ErrorMessage { get; private set; } = string.Empty;
    public WorkflowErrorType Type { get; private set; }
}