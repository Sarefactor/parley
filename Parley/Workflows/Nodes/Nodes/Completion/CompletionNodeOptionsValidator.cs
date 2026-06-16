using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.Completion;

[ParleyNodeValidator]
public class CompletionNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(CompletionNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var hasErrors = false;

        if (!TrySerialiseOptions<CompletionNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(CompletionNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        return hasErrors;
    }
}