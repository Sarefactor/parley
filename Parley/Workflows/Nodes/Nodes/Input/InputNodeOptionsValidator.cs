using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.Input;

[ParleyNodeValidator]
public class InputNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(InputNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var isValid = true;

        if (!TrySerialiseOptions<InputNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(InputNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Message))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the {nameof(InputNodeOptions.Message)} field.",
                                 WorkflowErrorType.Config,
                                 false);

            isValid = false;
        }

        var targetVariable = GetParleyVariableDto(options.TargetKey, workflowVariables);

        if (targetVariable == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Could not locate the target variable: {options.TargetKey}.",
                                 WorkflowErrorType.Config,
                                 false);

            isValid = false;
        }

        if (targetVariable != null
            && (targetVariable.Type == Core.DataAccess.Enums.VariableDataType.Object
                || targetVariable.IsList))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"The target variable for an input node cannot be of type object or a list.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        if (targetVariable != null)
        {
            ValidateValidationRules(workflowId,
                                    dto,
                                    targetVariable,
                                    context);
        }

        if (string.IsNullOrWhiteSpace(options.ErrorMessage))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the {nameof(InputNodeOptions.ErrorMessage)} field.",
                                 WorkflowErrorType.Config,
                                 false);

            isValid = false;
        }

        return isValid;
    }
}