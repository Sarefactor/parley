using System.Text.Json.Serialization;

namespace Parley.Workflows.State;

public sealed record IterationContext
{
    public IterationContext(Guid iteratorKey, string targetKey)
    {
        IteratorId = iteratorKey;
        TargetKey = targetKey;
        IsNew = true;
    }

    [JsonConstructor]
    public IterationContext(Guid iteratorKey, string targetKey, int iterationCount)
    {
        IteratorId = iteratorKey;
        TargetKey = targetKey;
        IterationCount = iterationCount;
    }

    [JsonInclude]
    [JsonPropertyName("iteratorId")]
    public Guid IteratorId { get; private set; }

    [JsonInclude]
    [JsonPropertyName("targetKey")]
    public string TargetKey { get; private set; }

    [JsonInclude]
    [JsonPropertyName("iterationCount")]
    public int IterationCount { get; private set; }

    [JsonIgnore]
    public bool IsNew { get; set; }

    public void Increment()
        => IterationCount++;
};