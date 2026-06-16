using Parley.Core.DataAccess.Enums;
using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;

namespace Parley.Workflows.Validation;

public class InputValidator : IValidateInput
{
    public bool Validate(WorkflowVariable workflowVariable, string input, List<ValidationRule> validationRules)
    {
        if (validationRules.Count == 0)
            return true;

        return validationRules.Select(rule =>
        {
            return workflowVariable.Type switch
            {
                VariableDataType.String => StringValidator.Validate(input, rule),
                VariableDataType.Integer => IntegerValidator.Validate(input, rule),
                VariableDataType.Bool => BoolValidator.Validate(input, rule),
                VariableDataType.DateTime => DateTimeValidator.Validate(input, rule),
                _ => throw new NotSupportedException($"Validation evaluation not supported for {nameof(VariableDataType)}: {workflowVariable.Type}")
            };
        }).All(result => result);
    }

    public Guid EvaluateTransition(Guid defaultTransitionNode, List<Transition> transitions, ICollection<WorkflowVariable> workflowVariables)
    {
        foreach (var transition in transitions.OrderBy(t => t.Priority))
        {
            var isMatch = transition.TransitionRules.Select(tr =>
            {
                var variable = workflowVariables.FirstOrDefault(dv => dv.Name == tr.TargetKey);

                if (variable == null)
                    return false;

                return variable.Type switch
                {
                    VariableDataType.String => StringValidator.EvaluateTransition(tr, variable),
                    VariableDataType.Integer => IntegerValidator.EvaluateTransition(tr, variable),
                    VariableDataType.Bool => BoolValidator.EvaluateTransition(tr, variable),
                    VariableDataType.DateTime => DateTimeValidator.EvaluateTransition(tr, variable),
                    _ => throw new NotSupportedException($"Transition evaluation not supported for {nameof(VariableDataType)}: {variable.Type}")
                };
            }).All(result => result);

            if (isMatch)
                return transition.TargetNodeId;
        }

        return defaultTransitionNode;
    }
}