using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.Enums;
using Parley.Workflows.Nodes.Nodes.Transition;
using System.Text.RegularExpressions;

namespace Parley.Workflows.Validation;

public static class StringValidator
{
    public static bool Validate(string input,
                                ValidationRule rule)
    {
        return Evaluate(rule, input);
    }
    
    public static bool EvaluateTransition(TransitionRule rule,
                                          TransitionContext context)
    {
        if (context.Variable is not { Type: VariableDataType.String })
            return false;

        var value = context.Variable.GetVariableValueAsString(rule.TargetKey, context.Context);

        if (value == null)
            return false;

        return Evaluate(rule, value);
    }

    private static bool Evaluate(ValidationRule rule,
                                 string value)
    {
        return rule.StringComparisonType switch
        {
            StringComparisonType.Match => value.Equals(rule.MatchString,
                                                       StringComparison.Ordinal),
            StringComparisonType.MatchNoCase => value.Equals(rule.MatchString,
                                                             StringComparison.OrdinalIgnoreCase),
            StringComparisonType.Contains => rule.MatchString is not null && value.Contains(rule.MatchString,
                                                                                            StringComparison.Ordinal),
            StringComparisonType.ContainsNoCase => rule.MatchString is not null && value.Contains(rule.MatchString,
                                                                                                  StringComparison.OrdinalIgnoreCase),
            StringComparisonType.Regex => rule.RegexString is not null && Regex.IsMatch(value,
                                                                                        rule.RegexString,
                                                                                        RegexOptions.None,
                                                                                        TimeSpan.FromSeconds(1)),
            _ => false
        };
    }
}