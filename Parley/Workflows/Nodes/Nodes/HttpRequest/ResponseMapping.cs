using System.Text.Json.Serialization;

namespace Parley.Workflows.Nodes.Nodes.HttpRequest;

public class ResponseMapping
{
    [JsonPropertyName("sourcePath")]
    public string SourcePath { get; set; } = default!;
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = default!;
}