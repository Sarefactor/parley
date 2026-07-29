using Parley.Core.Enums;
using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class ParleyVariableDto
{
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;
    
    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;
    
    [JsonInclude]
    [JsonPropertyName("type")]
    public VariableDataType Type { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("isList")]
    public bool IsList { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("nullable")]
    public bool IsNullable { get; set; }
}