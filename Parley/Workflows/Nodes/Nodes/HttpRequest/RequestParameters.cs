using System.Text.Json.Serialization;

namespace Parley.Workflows.Nodes.Nodes.HttpRequest;

public class RequestParameters
{
    [JsonPropertyName("parameterName")]
    public string ParameterName { get; set; } = string.Empty;

    [JsonPropertyName("targetKey")]
    public string TargetKey { get; set; } = string.Empty;
}