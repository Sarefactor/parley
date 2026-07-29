using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;

namespace Parley.Workflows.Validation;

public interface IValidateInput
{
    bool Validate(WorkflowVariable workflowVariable,
                  string input,
                  List<ValidationRule> validationRules);

    Guid EvaluateTransition(Guid defaultTransitionNode,
                            List<Transition> transitions,
                            ICollection<WorkflowVariable> workflowVariables);
}