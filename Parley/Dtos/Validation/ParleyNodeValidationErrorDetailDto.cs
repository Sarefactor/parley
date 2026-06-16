using Parley.Validation.Enums;

namespace Parley.Dtos.Validation;

public class ParleyNodeValidationErrorDetailDto
{
    public string ErrorMessage { get; set; } = string.Empty;
    public WorkflowErrorType Type { get; set; }
}