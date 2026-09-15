using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Workflows.Nodes.Nodes.Transition;

namespace Parley.Workflows.Validation;

public interface IValidateInput
{
    bool Validate(WorkflowVariable workflowVariable,
                  string input,
                  List<ValidationRule> validationRules);

    Task<Guid> EvaluateTransition(Guid defaultTransitionNode,
                                  List<Transition> transitions,
                                  List<TransitionContext> transitionContexts);
}