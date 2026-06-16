using Microsoft.Agents.AI.Workflows;
using Parley.Configuration.Attributes;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Nodes.Message;
using Parley.Workflows.State;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Bool;

[ParleyNode]
[SendsMessage(typeof(ParleyInputLink))]
internal sealed class ConfirmationNode : ParleyNode<ParleyLink>  
{
    public ConfirmationNode(ParleyNodeContext context,
                    IWorkflowStateManager workflowStateManager)
    : base(nameof(ConfirmationNode), context, workflowStateManager)
    {
        Config = GetNodeOptions<ConfirmationNodeOptions>();
    }

    private ConfirmationNodeOptions Config { get; set; } = new();
    public override string DialogType => nameof(ConfirmationNode);

    public override async ValueTask HandleAsync(ParleyLink parleyLink, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.SendMessageAsync(new ParleyInputLink { Message = Config.Message}, cancellationToken);
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                              Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        RequestPort inputRequestPort = RequestPort.Create<ParleyInputLink, string>($"{NodeConfig.NodeId}:InputPort");
        builder.AddEdge(this, inputRequestPort);

        var validator = new ConfirmationNodeValidator($"{NodeConfig.NodeId}:{nameof(ConfirmationNodeValidator)}", NodeConfig, Config, WorkflowStateManager);
        builder.AddEdge(inputRequestPort, validator);
        builder.AddEdge(validator, inputRequestPort);

        builder.AddEdge<ParleyLink>(validator,
                                    nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.PrimaryTransitionNode);

        builder.AddEdge<ParleyLink>(validator,
                                    nodes.Single(x => x.Key == NodeConfig.SecondaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.SecondaryTransitionNode);

        //var transitionNode = nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value;
        //builder.AddEdge(validator, transitionNode);

        //var secondaryTransitionNode = nodes.Single(x => x.Key == NodeConfig.SecondaryTransitionNode).Value;
        //builder.AddEdge(validator, secondaryTransitionNode);

        return builder;
    }
}

[ExportTsClass]
public class ConfirmationNodeOptions : MessageNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}