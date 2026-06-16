using Microsoft.Agents.AI.Workflows;
using Parley.Configuration.Attributes;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using Parley.Workflows.Validation;
using TypeGen.Core.TypeAnnotations;
namespace Parley.Workflows.Nodes.Nodes.Transition;

[ParleyNode]
[SendsMessage(typeof(ParleyLink))]
public class TransitionNode : ParleyNode<ParleyLink>
{
    private readonly IValidateInput _inputValidator;

    public TransitionNode(ParleyNodeContext context,
                          IWorkflowStateManager workflowStateManager,
                          IValidateInput inputValidator)
        : base(nameof(TransitionNode), context, workflowStateManager)
    {
        _inputValidator = inputValidator;
    }

    public override string DialogType => nameof(TransitionNode);

    public override async ValueTask HandleAsync(ParleyLink parleyLink,
                                                IWorkflowContext context,
                                                CancellationToken cancellationToken = default)
    {
        var transitionNode = _inputValidator.EvaluateTransition(NodeConfig.PrimaryTransitionNode,
                                                                NodeConfig.Transitions,
                                                                await WorkflowStateManager.GetWorkflowVariablesFromContext(context, cancellationToken));

        await context.SendMessageAsync(new ParleyLink(transitionNode), cancellationToken);
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                              Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        builder.AddEdge<ParleyLink>(this,
                                    nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value,
                                    link => link?.TransitionNode == NodeConfig.PrimaryTransitionNode);

        foreach (var transition in NodeConfig.Transitions)
        {
            builder.AddEdge<ParleyLink>(this,
                                        nodes.Single(x => x.Key == transition.TargetNodeId).Value,
                                        link => link?.TransitionNode == transition.TargetNodeId);
        }

        return builder;
    }
}

[ExportTsClass]
public class TransitionNodeOptions : ParleyNodeOptions {}