using System.Text.Json.Serialization;

namespace Parley.Dtos.Schema;

public class NodePositionDto
{
    [JsonInclude]
    [JsonPropertyName("x")]
    public double X { get; set; }
    [JsonInclude]
    [JsonPropertyName("y")]
    public double Y { get; set; }
}