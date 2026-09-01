using Parley.Core.Enums;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Variables;

public class ParleyVariable
{
    [JsonConstructor]
    public ParleyVariable() { }

    public ParleyVariable(string name,
                          string description,
                          VariableDataType type,
                          bool isList,
                          bool isNullable)
    {
        Name = name;
        Description = description;
        Type = type;
        IsList = isList;
        Nullable = isNullable;
    }

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; protected set; } = default!;

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; protected set; } = default!;

    [JsonInclude]
    [JsonPropertyName("type")]
    public VariableDataType Type { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("isList")]
    public bool IsList { get; protected set; }

    [JsonInclude]
    [JsonPropertyName("nullable")]
    public bool Nullable { get; protected set; }

    public static string ParseKey(string variableKey)
    {
        return variableKey.Contains(':') ? variableKey.Split(':')[0] : variableKey;
    }
    
    public static string ParseNodeKey(string variableKey)
    {
        return variableKey.Contains(':') ? variableKey.Split(':')[1] : variableKey;
    }
}