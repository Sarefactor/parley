using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Workflows.Links;
using Parley.Workflows.State;

namespace Parley.Workflows.Nodes.Nodes.Bool;

[SendsMessage(typeof(ParleyLink))]
[SendsMessage(typeof(ParleyInputLink))]
internal sealed class ConfirmationNodeValidator : Executor<string>
{
    private readonly IWorkflowStateManager _workflowStateManager;

    public ConfirmationNodeValidator(string id,
                             NodeConfig nodeConfig,
                             ConfirmationNodeOptions options,
                             IWorkflowStateManager workflowStateManager)
        : base(id)
    {
        NodeConfig = nodeConfig;
        NodeOptions = options;
        _workflowStateManager = workflowStateManager;
    }

    private NodeConfig NodeConfig { get; set; }
    private ConfirmationNodeOptions NodeOptions { get; set; }

    public override async ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        bool? answer = ParseAffirmation(message);

        if (answer is null)
        {
            await context.SendMessageAsync(new ParleyInputLink() { Message = NodeOptions.Message }, cancellationToken);
            return;
        }

        var workflowVariable = await _workflowStateManager.GetWorkflowVariable(context, NodeOptions.TargetKey, cancellationToken);

        workflowVariable.SetValue(answer.Value);
        await _workflowStateManager.SetWorkflowVariable(context, workflowVariable, cancellationToken);

        if ((bool)answer)
        {
            await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
            return;
        }

        await context.SendMessageAsync(new ParleyLink((Guid)NodeConfig.SecondaryTransitionNode!), cancellationToken);
    }

    private static bool? ParseAffirmation(string message)
        => message.Trim().ToLowerInvariant() switch
                                             {
                                                 "yes" or "y" => true,
                                                 "no" or "n" => false,
                                                 _ => null,
                                             };
}
