using Microsoft.Agents.AI.Workflows;
using Parley.Configuration.Attributes;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Workflows.Nodes.Nodes.Completion;

[ParleyNode]
[YieldsOutput(typeof(ParleyWorkflowOutput))]
public class CompletionNode : ParleyNode<ParleyLink>
{
    public CompletionNode(ParleyNodeContext context,
                          IWorkflowStateManager workflowStateManager)
    : base(nameof(CompletionNode), context, workflowStateManager) { }

    public override string DialogType => nameof(CompletionNode);

    public override async ValueTask HandleAsync(ParleyLink message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        await context.YieldOutputAsync(new ParleyWorkflowOutput(), cancellationToken);
    }

    public override WorkflowBuilder Configure(WorkflowBuilder builder,
                                              Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        return builder.WithOutputFrom(this);
    }
}

[ExportTsClass]
public class CompletionNodeOptions : ParleyNodeOptions {}