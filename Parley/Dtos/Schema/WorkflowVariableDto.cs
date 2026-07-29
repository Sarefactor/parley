using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class WorkflowVariableDto : ParleyVariableDto
{
    [JsonInclude]
    [JsonPropertyName("objectVariables")]
    public List<ParleyVariableDto> ObjectVariables { get; set; } = [];
}