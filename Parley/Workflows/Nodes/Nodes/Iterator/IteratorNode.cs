using Microsoft.Agents.AI.Workflows;
using Parley.Configuration.Attributes;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using Parley.Workflows.Validation;
using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Iterator;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
internal sealed class IteratorNode : ParleyNode<ParleyLink>
{
    private IteratorNodeOptions Config { get; set; } = new();
    public override string DialogType => nameof(IteratorNode);
    private string IterationKey { get; set; } = string.Empty;

    public IteratorNode(ParleyNodeContext context,
                        IWorkflowStateManager workflowStateManager,
                        IValidateInput inputValidator)
        : base(nameof(IteratorNode), context, workflowStateManager)
    {
        Config = GetNodeOptions<IteratorNodeOptions>();
        IterationKey = $"";
    }

    public override async ValueTask HandleAsync(ParleyLink parleyLink, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var iteratorContext = await WorkflowStateManager.GetIterationContext(NodeId,
                                                                             Config.TargetKey,
                                                                             context,
                                                                             cancellationToken);

        var workflowVariable = await WorkflowStateManager.GetWorkflowVariable(context, Config.TargetKey, cancellationToken);

        var variableIterationContext = workflowVariable.BuildVariableContext(Config.TargetKey);
        await SetVariableIterationContext(variableIterationContext, context, cancellationToken);

        if (iteratorContext.IterationCount >= workflowVariable.GetListCountZeroIndex(Config.TargetKey, variableIterationContext))
        {
            await WorkflowStateManager.ClearIterationContext(iteratorContext, context, cancellationToken);
            await context.SendMessageAsync(new ParleyLink((Guid)NodeConfig.SecondaryTransitionNode!), cancellationToken);
            return;
        }

        if (!iteratorContext.IsNew)
        {
            iteratorContext.Increment();
            await WorkflowStateManager.SetIterationContext(iteratorContext, context, cancellationToken);
        }

        await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                          Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        builder.AddEdge<ParleyLink>(this,
                                    nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.PrimaryTransitionNode);

        builder.AddEdge<ParleyLink>(this,
                                    nodes.Single(x => x.Key == NodeConfig.SecondaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.SecondaryTransitionNode);

        return builder;
    }
}

[ExportTsClass]
public class IteratorNodeOptions : ParleyNodeOptions
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}