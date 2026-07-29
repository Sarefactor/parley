using Parley.Core.Enums;
using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class ValidationRuleDto
{
    [JsonInclude]
    [JsonPropertyName("stringComparisonType")]
    public StringComparisonType StringComparisonType { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("matchString")]
    public string? MatchString { get; set; }

    [JsonInclude]
    [JsonPropertyName("regexString")]
    public string? RegexString { get; set; }

    [JsonInclude]
    [JsonPropertyName("numberComparisonType")]
    public NumberComparisonType NumberComparisonType { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("matchInt")]
    public int? MatchInt { get; set; }

    [JsonInclude]
    [JsonPropertyName("boolComparisonType")]
    public BoolComparisonType BoolComparisonType { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("matchBool")]
    public bool? MatchBool { get; set; }

    [JsonInclude]
    [JsonPropertyName("matchDateTime")]
    public DateTime? MatchDateTime { get; set; }
}