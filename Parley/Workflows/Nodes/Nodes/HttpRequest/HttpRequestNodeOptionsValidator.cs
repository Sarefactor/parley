using Parley.Configuration.Attributes;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Workflows.Nodes.Nodes.HttpRequest;

[ParleyNodeValidator]
public class HttpRequestNodeOptionsValidator : ParleyNodeOptionsValidator
{
    public override string NodeType => nameof(HttpRequestNode);

    public override bool Validate(Guid workflowId, NodeConfigDto dto, IReadOnlyCollection<WorkflowVariableDto> workflowVariables, ParleyValidationContext context)
    {
        var isValid = true;

        if (!TrySerialiseOptions<HttpRequestNodeOptions>(dto.NodeOptions, out var options)
            || options == null)
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Encountered an error while serialising the options into {nameof(HttpRequestNodeOptions)}.",
                                 WorkflowErrorType.Config,
                                 false);

            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Url))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Invalid value for {nameof(HttpRequestNodeOptions)} property {nameof(options.Url)}.",
                                 WorkflowErrorType.Config,
                                 false);

            isValid = false;
        }

        if (!Enum.IsDefined(options.MethodType))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Invalid value for {nameof(HttpRequestNodeOptions)} property {nameof(options.MethodType)}. Value not defined.",
                                 WorkflowErrorType.Config,
                                 false);
            
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(options.ContentType))
        {
            context.AddNodeError(workflowId,
                                 dto.NodeId,
                                 $"Invalid value for {nameof(HttpRequestNodeOptions)} property {nameof(options.ContentType)}.",
                                 WorkflowErrorType.Config,
                                 false);
            
            isValid = false;
        }
        
        return isValid;
    }
}