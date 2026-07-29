using Parley.Core.Enums;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Validation;

public class TransitionRule : ValidationRule
{
    public TransitionRule(string targetKey,
                          StringComparisonType stringComparisonType,
                          string? matchString,
                          string? regexString,
                          NumberComparisonType numberComparisonType,
                          int? matchInt,
                          BoolComparisonType boolComparisonType,
                          bool? matchBool,
                          DateTime? matchDateTime)
        : base(stringComparisonType,
               matchString,
               regexString,
               numberComparisonType,
               matchInt,
               boolComparisonType,
               matchBool,
               matchDateTime)
    {
        TargetKey = targetKey;
    }

    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; private set; } = string.Empty;
}