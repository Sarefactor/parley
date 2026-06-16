using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Core.DataAccess.Models.Schemas;

public static class WorkflowSchemaValidator
{
    public static void CollectValidationErrors(WorkflowSchemaDto dto,
                                               Guid workflowId,
                                               ParleyValidationContext context)
    {
        if (dto.ExecutionNodeId == Guid.Empty)
            context.AddWorkflowError(workflowId,
                                     $"Invalid value for {nameof(WorkflowSchema)} property {nameof(WorkflowSchema.ExecutionNodeId)}.",
                                     WorkflowErrorType.Config,
                                     false);

        if (string.IsNullOrWhiteSpace(dto.Name))
            context.AddWorkflowError(workflowId,
                                     $"Invalid value for {nameof(WorkflowSchema)} property {nameof(WorkflowSchema.Name)}.",
                                     WorkflowErrorType.Config,
                                     false);

        if (string.IsNullOrWhiteSpace(dto.Intent))
            context.AddWorkflowError(workflowId,
                                     $"Invalid value for {nameof(WorkflowSchema)} property {nameof(WorkflowSchema.Intent)}.",
                                     WorkflowErrorType.Config,
                                     false);

        if (string.IsNullOrWhiteSpace(dto.Description))
            context.AddWorkflowError(workflowId,
                                     $"Invalid value for {nameof(WorkflowSchema)} property {nameof(WorkflowSchema.Description)}.",
                                     WorkflowErrorType.Config,
                                     false);
    }
}