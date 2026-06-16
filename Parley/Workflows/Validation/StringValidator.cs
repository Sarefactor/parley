using Parley.Core.DataAccess.Enums;
using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.RegularExpressions;

namespace Parley.Workflows.Validation;

public static class StringValidator
{
    public static bool Validate(string input, ValidationRule rule)
        => !string.IsNullOrWhiteSpace(input) && Evaluate(input, rule);

    public static bool EvaluateTransition(TransitionRule rule, WorkflowVariable variable)
        => variable is { Type: VariableDataType.String, Value: string value }
           && !string.IsNullOrWhiteSpace(value)
           && Evaluate(value, rule);

    private static bool Evaluate(string input, ValidationRule rule)
    {
        var value = input.Trim();

        return rule.StringComparisonType switch
        {
            StringComparisonType.Match => value.Equals(rule.MatchString, StringComparison.Ordinal),
            StringComparisonType.MatchNoCase => value.Equals(rule.MatchString, StringComparison.OrdinalIgnoreCase),
            StringComparisonType.Contains => rule.MatchString is not null && value.Contains(rule.MatchString, StringComparison.Ordinal),
            StringComparisonType.ContainsNoCase => rule.MatchString is not null && value.Contains(rule.MatchString, StringComparison.OrdinalIgnoreCase),
            StringComparisonType.Regex => rule.RegexString is not null && Regex.IsMatch(value, rule.RegexString, RegexOptions.None, TimeSpan.FromSeconds(1)),
            _ => false
        };
    }
}