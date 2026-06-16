using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.Choice;

[ParleyNodeValidator]
public class ChoiceNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(ChoiceNode);

    public override bool Validate(Guid workflowId,
                                  NodeConfigDto dto,
                                  IReadOnlyCollection<WorkflowVariableDto> workflowVariables,
                                  ParleyValidationContext context)
    {
        var hasErrors = false;

        if (!TrySerialiseOptions<ChoiceNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(ChoiceNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Message))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the {nameof(ChoiceNodeOptions.Message)} field.",
                                 WorkflowErrorType.Config,
                                 false);

            hasErrors = true;
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

        if (string.IsNullOrWhiteSpace(options.ErrorMessage))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Please enter a value in the {nameof(ChoiceNodeOptions.ErrorMessage)} field.",
                                 WorkflowErrorType.Config,
                                 false);

            hasErrors = true;
        }

        if (options.Choices.Count < 2)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Invalid value for {nameof(ChoiceNodeOptions)} property {nameof(options.Choices)}. Must have at least 2 options.",
                                 WorkflowErrorType.Config,
                                 false);

            hasErrors = true;
        }

        if (!Enum.IsDefined(options.ValidationType))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Invalid value for {nameof(ChoiceNodeOptions)} property {nameof(options.ValidationType)}. Value not defined.",
                                 WorkflowErrorType.Config,
                                 false);

            hasErrors = true;
        }

        return hasErrors;
    }
}