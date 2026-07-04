using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.Bool;

[ParleyNodeValidator]
public class ConfirmationNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(ConfirmationNode);

    public override bool Validate(Guid workflowId,
                                  NodeConfigDto dto,
                                  IReadOnlyCollection<WorkflowVariableDto> workflowVariables,
                                  ParleyValidationContext context)
    {
        var isValid = true;

        if (!TrySerialiseOptions<ConfirmationNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                     dto.NodeId,
                     $"Encountered an error while serialising the options into {nameof(ConfirmationNodeOptions)}.",
                     WorkflowErrorType.Config,
                     false);

            return false;
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

        if (string.IsNullOrWhiteSpace(options.Message))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the message field.",
                                 WorkflowErrorType.Config,
                                 false);

            isValid = false;
        }

        return isValid;
    }
}