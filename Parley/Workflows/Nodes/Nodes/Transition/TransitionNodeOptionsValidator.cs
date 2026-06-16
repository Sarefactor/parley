using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;
namespace Parley.Workflows.Nodes.Nodes.Transition;

[ParleyNodeValidator]
public class TransitionNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(TransitionNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var hasErrors = false;

        if (!TrySerialiseOptions<TransitionNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(TransitionNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        return hasErrors;
    }
}