using Parley.Core.DataAccess.Models.Variables;

namespace Parley.Workflows.Nodes.Nodes.Transition;

public class TransitionContext
{
    public TransitionContext(WorkflowVariable workflowVariable,
                             VariableIterationContext variableIterationContext)
    {
        Variable = workflowVariable;
        Context = variableIterationContext;
    }

    public WorkflowVariable Variable { get; set; }
    public VariableIterationContext Context { get; set; }
}
