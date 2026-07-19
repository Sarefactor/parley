using Microsoft.Agents.AI.Workflows;
using Parley.Configuration.Attributes;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Events;
using Parley.Workflows.Nodes.Nodes.Message;
using Parley.Workflows.State;
using Parley.Workflows.Validation;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Input;

[ParleyNode]
[SendsMessage(typeof(ParleyInputLink))]
internal sealed class InputNode : ParleyNode<ParleyLink>  
{
    private readonly IValidateInput _inputValidator;

    private InputNodeOptions Config { get; set; } = new();
    public override string DialogType => nameof(InputNode);

    public InputNode(ParleyNodeContext context,
                           IWorkflowStateManager workflowStateManager,
                           IValidateInput inputValidator)
    : base(nameof(InputNode), context, workflowStateManager)
    {
        Config = GetNodeOptions<InputNodeOptions>();
        _inputValidator = inputValidator;
    }



    public override async ValueTask HandleAsync(ParleyLink parleyLink, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var messageToSend = await BuildMessage(context, Config.Message, cancellationToken);

        await context.SendMessageAsync(new ParleyInputLink { Message = messageToSend }, cancellationToken);
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                              Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        RequestPort inputRequestPort = RequestPort.Create<ParleyInputLink, string>($"{NodeConfig.NodeId}:InputPort");
        builder.AddEdge(this, inputRequestPort);

        var validator = new ParleyInputNodeValidator($"{NodeConfig.NodeId}:Validator", NodeConfig, Config, WorkflowStateManager, _inputValidator);
        builder.AddEdge(inputRequestPort, validator);
        builder.AddEdge(validator, inputRequestPort);

        var transitionNode = nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value;
        builder.AddEdge(validator, transitionNode);

        return builder;
    }
}

[ExportTsClass]
public class InputNodeOptions : MessageNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;
}