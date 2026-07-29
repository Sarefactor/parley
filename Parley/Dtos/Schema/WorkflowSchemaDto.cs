using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class WorkflowSchemaDto
{
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("intent")]
    public string Intent { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("executionNodeId")]
    public Guid ExecutionNodeId { get; set; } = default!;

    [JsonInclude]
    [JsonPropertyName("nodes")]
    public List<NodeConfigDto> Nodes { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("workflowVariables")]
    public List<WorkflowVariableDto> WorkflowVariables { get; set; } = new();
}