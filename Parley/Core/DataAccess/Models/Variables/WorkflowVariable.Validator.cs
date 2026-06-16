using Parley.Core.DataAccess.Enums;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Core.DataAccess.Models.Variables;

public static class WorkflowVariableValidator
{
    public static void CollectValidationErrors(ParleyVariableDto dto,          
                                               Guid workflowId,
                                               Guid? nodeId,
                                               WorkflowVariableType type,
                                               ParleyValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            AddErrorToValidationContext($"Invalid value for {nameof(WorkflowVariable)} property {nameof(WorkflowVariable.Name)}.",
                                        workflowId,
                                        nodeId,
                                        type,
                                        context);

        if (string.IsNullOrWhiteSpace(dto.Description))
            AddErrorToValidationContext($"Invalid value for {nameof(WorkflowVariable)} property {nameof(WorkflowVariable.Description)}.",
                                        workflowId,
                                        nodeId,
                                        type,
                                        context);

        if (!Enum.IsDefined(dto.Type))
            AddErrorToValidationContext($"Invalid value for {nameof(WorkflowVariable)} property {nameof(WorkflowVariable.Type)}.",
                                        workflowId,
                                        nodeId,
                                        type,
                                        context);

        if (dto is WorkflowVariableDto workflowVariableDto
            && dto.Type == VariableDataType.Object)
        {
            foreach (var objectVariable in workflowVariableDto.ObjectVariables)
            {
                CollectValidationErrors(objectVariable,
                                        workflowId,
                                        nodeId,
                                        type,
                                        context);
            }
        }   
    }

    private static void AddErrorToValidationContext(string errorMessage,
                                                    Guid workflowId,
                                                    Guid? nodeId,
                                                    WorkflowVariableType variableType,
                                                    ParleyValidationContext context)
    {
        if (variableType == WorkflowVariableType.Schema)
        {
            context.AddWorkflowError(workflowId, errorMessage, WorkflowErrorType.Config, false);
            return;
        }

        context.AddNodeError(workflowId, (Guid)nodeId!, errorMessage, WorkflowErrorType.Config, false);
    }
}