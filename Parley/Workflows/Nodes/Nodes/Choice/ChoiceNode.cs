using Microsoft.Agents.AI.Workflows;
using Parley.Classification;
using Parley.Configuration.Attributes;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Providers;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Nodes.Message;
using Parley.Workflows.State;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Choice;

[ParleyNode]
[SendsMessage(typeof(ParleyInputLink))]
internal sealed class ChoiceNode : ParleyNode<ParleyLink>  
{
    private readonly ITextClassifier _textClassifier;
    private readonly IChatClientProvider _chatClientProvider;

    public ChoiceNode(ParleyNodeContext context,
                      IWorkflowStateManager workflowStateManager,
                      ITextClassifier textClassifier,
                      IChatClientProvider chatClientProvider)
        : base(nameof(ChoiceNode), context, workflowStateManager)
    {
        NodeOptions = GetNodeOptions<ChoiceNodeOptions>();
        _textClassifier = textClassifier;
        _chatClientProvider = chatClientProvider;
    }

    private ChoiceNodeOptions NodeOptions { get; set; } = new();
    public override string DialogType => nameof(ChoiceNode);

    public override async ValueTask HandleAsync(ParleyLink parleyLink, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.SendMessageAsync(new ParleyInputLink { Message = NodeOptions.Message, Choices = NodeOptions.Choices, Type = ParleyInputType.Choice}, cancellationToken);
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                              Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        RequestPort inputRequestPort = RequestPort.Create<ParleyInputLink, string>($"{NodeConfig.NodeId}:InputPort");
        builder.AddEdge(this, inputRequestPort);

        var validator = new ParleyChoiceNodeValidator($"{NodeConfig.NodeId}:{nameof(ParleyChoiceNodeValidator)}", NodeConfig, NodeOptions, WorkflowStateManager, _textClassifier, _chatClientProvider);
        builder.AddEdge(inputRequestPort, validator);
        builder.AddEdge(validator, inputRequestPort);

        var transitionNode = nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value;
        builder.AddEdge(validator, transitionNode);

        return builder;
    }
}

[ExportTsClass]
public class ChoiceNodeOptions : MessageNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("choices")]
    public List<string> Choices { get; set; } = [];

    [JsonInclude]
    [JsonPropertyName("validationType")]
    public ChoiceValidationType ValidationType { get; set; }
}