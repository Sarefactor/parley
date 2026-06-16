using Microsoft.Agents.AI.Workflows;
using Parley.Configuration.Attributes;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Events;
using Parley.Workflows.State;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Message;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
internal sealed class MessageNode : ParleyNode<ParleyLink>
{
    private MessageNodeOptions MessageNodeOptions { get; set; } = new();

    public MessageNode(ParleyNodeContext context,                            
                             IWorkflowStateManager workflowStateManager)
        : base(nameof(MessageNode), context, workflowStateManager)
    {
        MessageNodeOptions = GetNodeOptions<MessageNodeOptions>();
    }

    public override string DialogType => nameof(MessageNode);

    public override async ValueTask HandleAsync(ParleyLink message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var messageToSend = await BuildMessage(context, MessageNodeOptions.Message, cancellationToken);

        await context.AddEventAsync(new ParleyMessageEvent(messageToSend), cancellationToken);
        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }
}

[ExportTsClass]
public class MessageNodeOptions : ParleyNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}