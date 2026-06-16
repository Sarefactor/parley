using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.Execution;

[ParleyNodeValidator]
public class ExecutionNodeOptionsOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(ExecutionNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var hasErrors = false;

        if (!TrySerialiseOptions<ExecutionNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(ExecutionNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        return hasErrors;
    }
}