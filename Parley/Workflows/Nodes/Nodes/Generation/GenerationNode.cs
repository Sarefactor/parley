using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using Parley.Classification;
using Parley.Configuration.Attributes;
using Parley.Providers;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Nodes.Message;
using Parley.Workflows.State;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Generation;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
public class GenerationNode : ParleyNode<ParleyLink>
{
    private readonly IChatClientProvider ChatClientProvider;
    private readonly IWorkflowClassifier WorkflowClassifier;

    public GenerationNode(ParleyNodeContext context,
                          IChatClientProvider chatClientProvider,
                          IWorkflowStateManager workflowStateManager,
                          IWorkflowClassifier workflowClassifier)
        : base(nameof(GenerationNode), context, workflowStateManager)
    {
        ChatClientProvider = chatClientProvider;
        WorkflowStateManager = workflowStateManager;
        WorkflowClassifier = workflowClassifier;
    }

    public override string DialogType => nameof(GenerationNode);

    public override async ValueTask HandleAsync(ParleyLink parleyLink,
                                                IWorkflowContext context,
                                                CancellationToken cancellationToken = default)
    {
        await Generate(context, cancellationToken);

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }

    private async Task Generate(IWorkflowContext context, CancellationToken cancellationToken)
    {
        var options = GetNodeOptions<GenerationNodeOptions>();

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, await BuildMessage(context, options.Message, cancellationToken))
        };

        var chatClient = ChatClientProvider.Provide();

        var response = await chatClient.GetResponseAsync(messages, null, cancellationToken);

        await SetWorkflowVariable(context, options.TargetKey, response.Text, cancellationToken);
    }
}

[ExportTsClass]
public class GenerationNodeOptions : MessageNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}