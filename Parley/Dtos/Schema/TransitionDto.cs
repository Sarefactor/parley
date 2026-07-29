using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class TransitionDto
{
    [JsonInclude]
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    [JsonInclude]
    [JsonPropertyName("targetNodeId")]
    public Guid TargetNodeId { get; set; }

    [JsonInclude]
    [JsonPropertyName("transitionRules")]
    public List<TransitionRuleDto> TransitionRules { get; set; } = new();
}