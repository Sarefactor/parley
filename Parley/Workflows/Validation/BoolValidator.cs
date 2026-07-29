using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Core.Enums;

namespace Parley.Workflows.Validation;

public static class BoolValidator
{
    public static bool Validate(string input,
                                ValidationRule rule)
    {
        return bool.TryParse(input,
                             out var value)
               && Evaluate(value,
                           rule);
    }
 
    public static bool EvaluateTransition(TransitionRule rule,
                                          WorkflowVariable variable)
    {
        return variable is { Type: VariableDataType.Bool, Value: bool value }
               && Evaluate(value,
                           rule);
    }

    private static bool Evaluate(bool value,
                                 ValidationRule rule)
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