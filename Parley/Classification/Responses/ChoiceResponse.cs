using System.Text.Json.Serialization;

namespace Parley.Classification.Responses;

public class ChoiceResponse
{
    [JsonInclude]
    [JsonPropertyName("choice")]
    public string Choice { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("isValid")]
    public bool IsValid { get; set; } = false;
}