using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;
using Parley.Workflows.Nodes.Nodes.Choice;
using Parley.Workflows.Nodes.Nodes.Completion;
using Parley.Workflows.Nodes.Nodes.Execution;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parley.Workflows.Nodes.Nodes.Generation;

[ParleyNodeValidator]
internal class GenerationNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(GenerationNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var isValid = true;

        if (!TrySerialiseOptions<GenerationNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(GenerationNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Message))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the {nameof(GenerationNodeOptions.Message)} field.",
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

        return isValid;
    }
}