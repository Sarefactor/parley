using Parley.Core.Enums;

namespace Parley.Workflows.Validation;

internal static class ComparisonEvaluator
{
    public static bool Evaluate<T>(T value,
                                   T? match,
                                   NumberComparisonType type) where T : struct, IComparable<T>
    {
        if (match is null)
            return false;

        var compare = value.CompareTo(match.Value);

        return type switch
        {
            NumberComparisonType.EqualTo => compare == 0,
            NumberComparisonType.NotEqualTo => compare != 0,
            NumberComparisonType.GreaterThan => compare > 0,
            NumberComparisonType.GreaterThanOrEqualTo => compare >= 0,
            NumberComparisonType.LessThan => compare < 0,
            NumberComparisonType.LessThanOrEqualTo => compare <= 0,
            _ => false
        };
    }
}