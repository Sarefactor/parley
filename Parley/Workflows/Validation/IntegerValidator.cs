using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.Enums;
using Parley.Workflows.Nodes.Nodes.Transition;
using System.Globalization;

namespace Parley.Workflows.Validation;

public static class IntegerValidator
{
    public static bool Validate(string input,
                                ValidationRule rule)
        => int.TryParse(input,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out var value)
           && ComparisonEvaluator.Evaluate(value,
                                           rule.MatchInt,
                                           rule.NumberComparisonType);

    public static bool EvaluateTransition(TransitionRule rule,
                                          TransitionContext context)
    {
        if (context.Variable is not { Type: VariableDataType.Integer })
            return false;

        var value = context.Variable.GetVariableValueAsComparable<int>(rule.TargetKey, context.Context);

        if (value == null)
            return false;

        return ComparisonEvaluator.Evaluate((int)value, rule.MatchInt, rule.NumberComparisonType);


    }
}