using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Parley.Classification;
using Parley.Configuration.Attributes;
using Parley.Providers;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;
namespace Parley.Workflows.Nodes.Nodes.Classification;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
public class ClassificationNode : ParleyNode<ParleyLink>
{
    private readonly IChatClientProvider ChatClientProvider;
    private readonly IWorkflowClassifier WorkflowClassifier;

    public ClassificationNode(ParleyNodeContext context,
                              IChatClientProvider chatClientProvider,
                              IWorkflowStateManager workflowStateManager,
                              IWorkflowClassifier workflowClassifier)
        : base(nameof(ClassificationNode), context, workflowStateManager)
    {
        ChatClientProvider = chatClientProvider;
        WorkflowStateManager = workflowStateManager;
        WorkflowClassifier = workflowClassifier;
    }

    public override string DialogType => nameof(ClassificationNode);

    public override async ValueTask HandleAsync(ParleyLink parleyLink,
                                                IWorkflowContext context,
                                                CancellationToken cancellationToken = default)
    {
        var options = GetNodeOptions<ClassificationNodeOptions>();

        var workflowVariable = await WorkflowStateManager.GetWorkflowVariable(context, options.TargetKey, cancellationToken);

        if (workflowVariable.Value is string input
            && !string.IsNullOrWhiteSpace(input))
            await Classify(input, context, cancellationToken);

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }

    private async Task Classify(string input, IWorkflowContext context, CancellationToken cancellationToken)
    {
        var contextVariables = await WorkflowStateManager.GetWorkflowVariablesFromContext(context, cancellationToken);

        var classification = WorkflowClassifier.GetPromptAndSchema(new ClassificationOptions
                                                                   {
                                                                       ClassificationVariables = [],
                                                                       IsWorkflowClassification = true,
                                                                       Text = input
                                                                   },
                                                                   NodeConfig.NodeVariables);

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, classification.Prompt)
        };

        var chatClient = ChatClientProvider.Provide();

        var options = new ChatOptions
        {
            ResponseFormat = ChatResponseFormat.ForJsonSchema(classification.JsonSchema, classification.SchemaName)
        };

        var response = await chatClient.GetResponseAsync(messages, options, cancellationToken);

        var extractedVariables = JsonNode.Parse(response.Text)?
                                         .AsObject()
                                 ?? throw new InvalidOperationException("Could not parse the extracted response.");

        await WorkflowStateManager.SetWorkflowVariables(context,
                                                        [.. NodeConfig.NodeVariables.Select(x => x.Name)],
                                                        extractedVariables,
                                                        cancellationToken);
    }
}

[ExportTsClass]
public class ClassificationNodeOptions : ParleyNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}

