using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class TransitionRuleDto : ValidationRuleDto
{
    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}