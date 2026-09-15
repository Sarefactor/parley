using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Core.Enums;
using Parley.Workflows.Nodes.Nodes.Transition;

namespace Parley.Workflows.Validation;

public class InputValidator : IValidateInput
{

    public bool Validate(WorkflowVariable workflowVariable,
                         string input,
                         List<ValidationRule> validationRules)
    {
        if (validationRules.Count == 0)
            return true;

        return validationRules.Select(rule =>
        {
            return workflowVariable.Type switch
            {
                VariableDataType.String => StringValidator.Validate(input,
                                                                    rule),
                VariableDataType.Integer => IntegerValidator.Validate(input,
                                                                      rule),
                VariableDataType.Bool => BoolValidator.Validate(input,
                                                                rule),
                VariableDataType.DateTime => DateTimeValidator.Validate(input,
                                                                        rule),
                _ => throw new NotSupportedException($"Validation evaluation not supported for {nameof(VariableDataType)}: {workflowVariable.Type}")
            };
        }).All(result => result);
    }

    public async Task<Guid> EvaluateTransition(Guid defaultTransitionNode,
                                               List<Transition> transitions,
                                               List<TransitionContext> transitionContexts)
    {
        foreach (var transition in transitions.OrderBy(t => t.Priority))
        {
            var isMatch = transition.TransitionRules.Select(tr =>
            {
                var transitionContext = transitionContexts.Single(x => x.Variable.Name == tr.TargetKey);

                return transitionContext.Variable.Type switch
                {
                    VariableDataType.String => StringValidator.EvaluateTransition(tr,
                                                                                  transitionContext),
                    VariableDataType.Integer => IntegerValidator.EvaluateTransition(tr,
                                                                                    transitionContext),
                    VariableDataType.Bool => BoolValidator.EvaluateTransition(tr,
                                                                              transitionContext),
                    VariableDataType.DateTime => DateTimeValidator.EvaluateTransition(tr,
                                                                                      transitionContext),
                    _ => throw new NotSupportedException($"Transition evaluation not supported for {nameof(VariableDataType)}: {transitionContext.Variable.Type}")
                };
            });

            if (isMatch.All(isMatch => isMatch))
                return transition.TargetNodeId;
        }

        return defaultTransitionNode;
    }
}