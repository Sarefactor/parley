using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Parley.Classification;
using Parley.Classification.Responses;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Providers;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Events;
using Parley.Workflows.State;
using System.Text.Json;

namespace Parley.Workflows.Nodes.Nodes.Choice;

[SendsMessage(typeof(ParleyLink))]
[SendsMessage(typeof(ParleyInputLink))]
internal sealed class ParleyChoiceNodeValidator : Executor<string>
{
    private readonly IWorkflowStateManager _workflowStateManager;
    private readonly ITextClassifier _textClassifier;
    private readonly IChatClientProvider _chatClientProvider;

    public ParleyChoiceNodeValidator(string id,
                                     NodeConfig nodeConfig,
                                     ChoiceNodeOptions options,
                                     IWorkflowStateManager workflowStateManager,
                                     ITextClassifier textClassifier,
                                     IChatClientProvider chatClientProvider)
        : base(id)
    {
        NodeConfig = nodeConfig;
        NodeOptions = options;
        _workflowStateManager = workflowStateManager;
        _textClassifier = textClassifier;
        _chatClientProvider = chatClientProvider;
    }

    private NodeConfig NodeConfig { get; set; }

    private ChoiceNodeOptions NodeOptions { get; set; }

    public override async ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var workflowVariable = await _workflowStateManager.GetWorkflowVariable(context,
                                                                               NodeOptions.TargetKey,
                                                                               cancellationToken);

        switch (NodeOptions.ValidationType){
            case ChoiceValidationType.Default:
                await HandleDefaultChoiceValidation(message, workflowVariable, context, cancellationToken);
                break;

            case ChoiceValidationType.AgentBreakdown:
                await HandleAgentChoiceValidation(message, workflowVariable, context, cancellationToken);
                break;
        }
    }

    private async Task HandleDefaultChoiceValidation(string input,
                                                     WorkflowVariable workflowVariable,
                                                     IWorkflowContext context,
                                                     CancellationToken cancellationToken)
    {
        var choice = NodeOptions.Choices.FirstOrDefault(x => x.Trim().Equals(input.Trim(), StringComparison.OrdinalIgnoreCase));

        if (choice == null)
        {
            await context.AddEventAsync(new ParleyMessageEvent(NodeOptions.ErrorMessage));
            await context.SendMessageAsync(new ParleyInputLink { Message = NodeOptions.Message, Choices = NodeOptions.Choices, Type = ParleyInputType.Choice }, cancellationToken);
            return;
        }

        workflowVariable.SetValue(choice);
        await _workflowStateManager.SetWorkflowVariable(context, workflowVariable, cancellationToken);

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);    
    }

    private async Task HandleAgentChoiceValidation(string input,
                                                   WorkflowVariable workflowVariable,
                                                   IWorkflowContext context,
                                                   CancellationToken cancellationToken)
    {
        var choiceResponse = await AgentChoiceValidator(input, cancellationToken);

        if (!choiceResponse.IsValid)
        {
            await context.AddEventAsync(new ParleyMessageEvent(NodeOptions.ErrorMessage));
            await context.SendMessageAsync(new ParleyInputLink { Message = NodeOptions.Message, Choices = NodeOptions.Choices, Type = ParleyInputType.Choice }, cancellationToken);
            return;
        }

        workflowVariable.SetValue(choiceResponse.Choice);
        await _workflowStateManager.SetWorkflowVariable(context, workflowVariable, cancellationToken);

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }

    private async Task<ChoiceResponse> AgentChoiceValidator(string input, CancellationToken cancellationToken)
    {
        try
        {
            var classification = _textClassifier.GetPromptAndSchemaForChoices(input, NodeOptions.Choices);

            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, classification.Prompt)
            };

            var chatClient = _chatClientProvider.Provide();

            var options = new ChatOptions
            {
                ResponseFormat = ChatResponseFormat.ForJsonSchema(classification.JsonSchema, classification.SchemaName)
            };

            var response = await chatClient.GetResponseAsync(messages, options, cancellationToken);

            return JsonSerializer.Deserialize<ChoiceResponse>(response.Text) ?? new();
        }

        catch(Exception ex)
        {
            var exMessage = ex.Message;
            return new();
        }
    }
}
