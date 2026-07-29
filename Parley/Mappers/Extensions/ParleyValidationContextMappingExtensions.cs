using Parley.Dtos.Validation;
using Parley.Validation;

namespace Parley.Mappers.Extensions;

public static class ParleyValidationContextMappingExtensions
{
    public static ParleyValidationContextDto MapValidationDto(this ParleyValidationContext context)
    {
        return new ParleyValidationContextDto
        {
            AgentErrorMessages = context.AgentErrorMessages.Select(x => x)
                                                           .ToList(),
            WorkflowErrors = context.WorkflowErrors.Select(MapWorkflowValidationDto)
                                                   .ToList()
        };
    }

    private static ParleyWorkflowValidationErrorDto MapWorkflowValidationDto(ParleyWorkflowValidationError workflowError)
    {
        return new ParleyWorkflowValidationErrorDto
        {
            WorkflowId = workflowError.WorkflowId,
            ErrorDetails = workflowError.ErrorDetails.Select(x => MapWorkflowErrorDetailDto(x))
                                                     .ToList(),
            NodeErrors = workflowError.NodeErrors.Select(x => MapNodeValidationDto(x))
                                                 .ToList()
        };
    }

    private static ParleyWorkflowValidationErrorDetailDto MapWorkflowErrorDetailDto(ParleyWorkflowValidationErrorDetail workflowErrorDetail)
    {
        return new ParleyWorkflowValidationErrorDetailDto
        {
            ErrorMessage = workflowErrorDetail.ErrorMessage,
            Type = workflowErrorDetail.Type
        };
    }

    private static ParleyNodeValidationErrorDto MapNodeValidationDto(this ParleyNodeValidationError nodeError)
    {
        return new ParleyNodeValidationErrorDto
        {
            NodeId = nodeError.NodeId,
            ErrorDetails = nodeError.ErrorDetails.Select(MapNodeErrorDetailDto)
                                                 .ToList()
        };
    }

    private static ParleyNodeValidationErrorDetailDto MapNodeErrorDetailDto(ParleyNodeValidationErrorDetail nodeErrorDetail)
    {
        return new ParleyNodeValidationErrorDetailDto
        {
            ErrorMessage = nodeErrorDetail.ErrorMessage,
            Type = nodeErrorDetail.Type
        };
    }
}