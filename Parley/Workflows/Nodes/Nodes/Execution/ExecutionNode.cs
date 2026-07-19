using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Parley.Classification;
using Parley.Configuration.Attributes;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Providers;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using System.Text.Json.Nodes;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Execution;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
public class ExecutionNode : ParleyNode<ParleyLink>
{
    private readonly IChatClientProvider ChatClientProvider;
    private readonly IWorkflowClassifier WorkflowClassifier;

    private WorkflowSchema WorkflowSchema { get; set; }

    public ExecutionNode(ParleyNodeContext context,
                               IChatClientProvider chatClientProvider,
                               IWorkflowStateManager workflowStateManager,
                               IWorkflowClassifier workflowClassifier)
        : base(nameof(ExecutionNode), context, workflowStateManager)
    {
        ChatClientProvider = chatClientProvider;
        WorkflowStateManager = workflowStateManager;
        WorkflowClassifier = workflowClassifier;
        WorkflowSchema = context.WorkflowSchema;
    }

    public override string DialogType => nameof(ExecutionNode);

    public override async ValueTask HandleAsync(ParleyLink parleyLink,
                                                IWorkflowContext context,
                                                CancellationToken cancellationToken = default)
    {
        var inputMessage = (string.IsNullOrWhiteSpace(parleyLink.LinkMessage)
                            || parleyLink.LinkMessage.Trim().ToLower() == "test")
                            ? "I want to book a holiday for august 20th for 4 weeks."
                            : parleyLink.LinkMessage;

        JsonObject? extractedVariables = null;

        if (WorkflowSchema.WorkflowVariables.Count > 0
            && !string.IsNullOrWhiteSpace(parleyLink.LinkMessage))
        {
            extractedVariables = await ClassifyWorkflowVariables(inputMessage, context, cancellationToken);
        }

        await WorkflowStateManager.InitialiseWorkflowVariables(context,
                                                               WorkflowSchema.GetAllWorkflowVariables(),
                                                               extractedVariables,
                                                               cancellationToken);

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }

    private async Task<JsonObject> ClassifyWorkflowVariables(string inputMessage,
                                                             IWorkflowContext context,
                                                             CancellationToken cancellationToken)
    {
        var classification = WorkflowClassifier.GetPromptAndSchema(new ClassificationOptions
        {
            ClassificationVariables = [],
            IsWorkflowClassification = true,
            Text = inputMessage
        },
        WorkflowSchema.WorkflowVariables);

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

        await WorkflowStateManager.InitialiseWorkflowVariables(context,
                                                               WorkflowSchema.GetAllWorkflowVariables(),
                                                               extractedVariables,
                                                               cancellationToken);

        return extractedVariables;
    }
}

[ExportTsClass]
public class ExecutionNodeOptions : ParleyNodeOptions {}