namespace Parley.Dtos.Validation;

public class ParleyWorkflowValidationErrorDto
{
    public Guid WorkflowId { get; set; }
    public List<ParleyWorkflowValidationErrorDetailDto> ErrorDetails { get; set; } = [];
    public List<ParleyNodeValidationErrorDto> NodeErrors { get; set; } = [];
}