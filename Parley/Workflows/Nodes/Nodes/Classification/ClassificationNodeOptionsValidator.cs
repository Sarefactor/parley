using Microsoft.Extensions.Options;
using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.Classification;

[ParleyNodeValidator]
public class ClassificationNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(ClassificationNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var hasErrors = false;

        if (!TrySerialiseOptions<ClassificationNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(ClassificationNodeOptions)}.",
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

            hasErrors = true;
        }

        return hasErrors;
    }
}