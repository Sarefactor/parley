using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class NodeConfigDto
{
    [JsonInclude]
    [JsonPropertyName("nodeId")]
    public Guid NodeId { get; set; }

    [JsonInclude]
    [JsonPropertyName("nodeType")]
    public string NodeType { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("primaryTransitionNode")]
    public Guid PrimaryTransitionNode { get; set; }

    [JsonInclude]
    [JsonPropertyName("secondaryTransitionNode")]
    public Guid? SecondaryTransitionNode { get; set; }

    [JsonInclude]
    [JsonPropertyName("nodeOptions")]
    public object NodeOptions { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("nodeVariables")]
    public List<WorkflowVariableDto> NodeVariables { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("transitions")]
    public List<TransitionDto> Transitions { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("validationRules")]
    public List<ValidationRuleDto> ValidationRules { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("position")]
    public NodePositionDto Position { get; set; } = new();
}