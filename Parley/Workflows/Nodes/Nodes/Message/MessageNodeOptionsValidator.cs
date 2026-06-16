using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;
using Parley.Workflows.Nodes.Nodes.Input;

namespace Parley.Workflows.Nodes.Nodes.Message;

[ParleyNodeValidator]
public class MessageNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(MessageNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var hasErrors = false;

        if (!TrySerialiseOptions<InputNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(MessageNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Message))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the {nameof(MessageNodeOptions.Message)} field.",
                                 WorkflowErrorType.Config,
                                 false);

            hasErrors = true;
        }

        return hasErrors;
    }
}