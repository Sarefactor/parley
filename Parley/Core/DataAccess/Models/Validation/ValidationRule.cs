using Parley.Core.Enums;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Validation;

public class ValidationRule
{
    public ValidationRule(StringComparisonType stringComparisonType,
                          string? matchString,
                          string? regexString,
                          NumberComparisonType numberComparisonType,
                          int? matchInt,
                          BoolComparisonType boolComparisonType,
                          bool? matchBool,
                          DateTime? matchDateTime)
    {
        StringComparisonType = stringComparisonType;
        MatchString = matchString;
        RegexString = regexString;
        NumberComparisonType = numberComparisonType;
        MatchInt = matchInt;
        BoolComparisonType = boolComparisonType;
        MatchBool = matchBool;
        MatchDateTime = matchDateTime;
    }


    [JsonInclude]
    [JsonPropertyName("stringComparisonType")]
    public StringComparisonType StringComparisonType { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("matchString")]
    public string? MatchString { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("regexString")]
    public string? RegexString { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("numberComparisonType")]
    public NumberComparisonType NumberComparisonType { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("matchInt")]
    public int? MatchInt { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("boolComparisonType")]
    public BoolComparisonType BoolComparisonType { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("matchBool")]
    public bool? MatchBool { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("matchDateTime")]
    public DateTime? MatchDateTime { get; protected set; }
}