using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.Enums;
using Parley.Workflows.Nodes.Nodes.Transition;
using System.Globalization;

namespace Parley.Workflows.Validation;

public static class DateTimeValidator
{
    public static bool Validate(string input,
                                ValidationRule rule)
        => DateTime.TryParse(input,
                             CultureInfo.InvariantCulture,
                             DateTimeStyles.None,
                             out var value)
           && ComparisonEvaluator.Evaluate(value,
                                           rule.MatchDateTime,
                                           rule.NumberComparisonType);

    public static bool EvaluateTransition(TransitionRule rule,
                                          TransitionContext context)
    {
        if (context.Variable is not { Type: VariableDataType.DateTime })
            return false;

        var value = context.Variable.GetVariableValueAsComparable<DateTime>(rule.TargetKey, context.Context);

        if (value == null)
            return false;

        return ComparisonEvaluator.Evaluate((DateTime)value, rule.MatchDateTime, rule.NumberComparisonType);
    }
}