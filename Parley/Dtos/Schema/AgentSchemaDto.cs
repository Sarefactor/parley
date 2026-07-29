using System.Text.Json.Serialization;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Dtos.Schema;

[ExportTsClass]
public class AgentSchemaDto
{
    [JsonInclude]
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("instructions")]
    public string Instructions { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("workflowSchemas")]
    public List<WorkflowSchemaDto> WorkflowSchemas { get; set; } = new();
}