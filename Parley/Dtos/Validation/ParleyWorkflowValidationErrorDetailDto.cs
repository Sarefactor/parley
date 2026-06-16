using Parley.Configuration.Exceptions;
using Parley.Validation.Enums;

namespace Parley.Dtos.Validation;

public class ParleyWorkflowValidationErrorDetailDto
{
    public string ErrorMessage { get; set; } = string.Empty;
    public WorkflowErrorType Type { get; set; }
}