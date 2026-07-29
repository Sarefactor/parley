using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Core.Enums;
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
                                          WorkflowVariable variable)
        => variable is { Type: VariableDataType.Integer, Value: int value }
           && ComparisonEvaluator.Evaluate(value,
                                           rule.MatchInt,
                                           rule.NumberComparisonType);
}