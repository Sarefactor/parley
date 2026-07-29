using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Core.Enums;
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
                                          WorkflowVariable variable)
        => variable is { Type: VariableDataType.DateTime, Value: DateTime value }
           && ComparisonEvaluator.Evaluate(value,
                                           rule.MatchDateTime,
                                           rule.NumberComparisonType);
}