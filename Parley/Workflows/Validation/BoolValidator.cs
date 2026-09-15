using Parley.Core.DataAccess.Models.Validation;

using Parley.Core.Enums;
using Parley.Workflows.Nodes.Nodes.Transition;

namespace Parley.Workflows.Validation;

public static class BoolValidator
{
    public static bool Validate(string input,
                                ValidationRule rule)
    {
        return bool.TryParse(input,
                             out var value)
               && Evaluate(rule,
                           value);
    }
 
    public static bool EvaluateTransition(TransitionRule rule,
                                          TransitionContext context)
    {
        if (context.Variable is not { Type: VariableDataType.Bool })
            return false;

        var value = context.Variable.GetVariableValueAsBool(rule.TargetKey, context.Context);

        if (value == null)
            return false;

        return Evaluate(rule, (bool)value);
    }

    private static bool Evaluate(ValidationRule rule,
                                 bool value)
    {
        return rule.MatchBool is { } match
               && rule.BoolComparisonType switch
               {
                   BoolComparisonType.EqualTo => value == match,
                   BoolComparisonType.NotEqualTo => value != match,
                   _ => false
               };
    }
}