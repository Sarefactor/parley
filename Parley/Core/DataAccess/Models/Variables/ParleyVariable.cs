using Parley.Core.DataAccess.Enums;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Variables;

public partial class ParleyVariable
{
    [JsonConstructor]
    public ParleyVariable() { }

    public ParleyVariable(string name,
                          string description,
                          VariableDataType type,
                          bool isList,
                          bool isNullable)
    {
        Validate(name, description);

        Name = name;
        Description = description;
        Type = type;
        IsList = isList;
        Nullable = isNullable;
    }

    [JsonPropertyName("name")]
    [JsonInclude]
    public string Name { get; protected set; } = default!;

    [JsonPropertyName("description")]
    [JsonInclude]
    public string Description { get; protected set; } = default!;

    [JsonPropertyName("type")]
    [JsonInclude]
    public VariableDataType Type { get; protected set; }

    [JsonPropertyName("isList")]
    [JsonInclude]
    public bool IsList { get; protected set; }

    [JsonPropertyName("nullable")]
    [JsonInclude]
    public bool Nullable { get; protected set; }

    public static string ParseKey(string variableKey)
    {
        return variableKey.Contains(':') ? variableKey.Split(':')[0] : variableKey;
    }
}

public partial class ParleyVariable
{
    protected virtual void Validate(string name,
                      string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception($"Invalid value for {nameof(WorkflowVariable)} property {nameof(name)}.");

        if (string.IsNullOrWhiteSpace(description))
            throw new Exception($"Invalid value for {nameof(WorkflowVariable)} property {nameof(description)}.");
    }
}