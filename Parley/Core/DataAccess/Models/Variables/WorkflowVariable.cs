using Parley.Core.Enums;
using Parley.Core.Extensions;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Variables;

public class WorkflowVariable : ParleyVariable
{
    [JsonConstructor]
    private WorkflowVariable() { }

    public WorkflowVariable(string name,
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

    public WorkflowVariable(WorkflowVariable workflowVariable,
                            object? value)
    {
        Name = workflowVariable.Name;
        Description = workflowVariable.Description;
        Type = workflowVariable.Type;
        IsList = workflowVariable.IsList;
        Nullable = workflowVariable.Nullable;
        Value = value;
        ObjectVariables = workflowVariable.ObjectVariables;
    }

    [JsonInclude]
    [JsonPropertyName("objectVariables")]
    public List<ParleyVariable> ObjectVariables { get; private set; } = new();

    [JsonInclude]
    [JsonPropertyName("value")]
    public object? Value { get; private set; } = new();

    public void SetValue(object value)
    {
        if (IsList)
            return;

        var isValid = (Type, value) switch
        {
            (VariableDataType.String, string) => true,
            (VariableDataType.Integer, int) => true,
            (VariableDataType.Bool, string) => true,
            (VariableDataType.DateTime, DateTime) => true,
            _ => false
        };

        if (!isValid)
            return;

        Value = new JsonObject
        {
            [Name] = JsonSerializer.SerializeToNode(value, value.GetType())
        };
    }


    public void SetObjectVariables(List<ParleyVariable> parleyVariables)
        => ObjectVariables = parleyVariables;

    public int GetListCountZeroIndex(string targetKey,
                                     VariableIterationContext? iterationContext = null)
    {
        var isPrimaryVariable = IsPrimaryVariableKey(targetKey, out var objectVariableKey);
        var primaryKey = ParseKey(targetKey);

        if (Value is JsonNode baseNode
            && baseNode.GetPath() == $"$.{primaryKey}"
            && baseNode.TryGetNode(true,
                                   out var targetNode,
                                   iterationContext?.PrimaryContext.IterationContext?.IterationCount,
                                   objectVariableKey,
                                   iterationContext?.SecondaryContext?.IterationContext?.IterationCount)
            && targetNode is JsonArray targetArrayNode)
        {
            return targetArrayNode.Count - 1;
        }

        return -1;
    }

    public string? GetVariableValueAsString(string targetKey,
                                            VariableIterationContext iterationContext)
    {
        var isPrimaryVariable = IsPrimaryVariableKey(targetKey, out var objectVariableKey);
        var primaryKey = ParseKey(targetKey);

        if (Value is JsonNode baseNode
            && baseNode.TryGetNode(false,
                                   out var targetNode,
                                   iterationContext.PrimaryContext.IterationContext?.IterationCount,
                                   objectVariableKey,
                                   iterationContext.SecondaryContext?.IterationContext?.IterationCount)
            && targetNode is JsonValue jsonValue
            && jsonValue.TryGetValue<string>(out var stringValue))
        {
            return stringValue;
        }

        return targetKey;
    }

    public VariableIterationContext BuildVariableContext(string targetKey)
    {
        var key = targetKey.Contains(':') ? targetKey.Split(':')[0]
                                          : targetKey;

        var secondaryKey = targetKey.Contains(':') ? targetKey
                                                   : null;

        bool? secondaryIsList = secondaryKey != null ? ObjectVariables.First(ov => ov.Name == targetKey.Split(':')[1]).IsList
                                                     : null;

        return new VariableIterationContext(key, IsList, secondaryKey, secondaryIsList);
    }

    public static bool IsPrimaryVariableKey(string targetKey,
                                            out string objectVariableKey)
    {
        var split = targetKey.Split(':', 2);
        objectVariableKey = split.Length == 2 ? split[1] : string.Empty;
        return split.Length == 1;
    }

    public static (string primaryKey, string? secondaryKey) ParseAndSplitKey(string variableKey)
    {
        if (!variableKey.Contains(':'))
            return (variableKey, null);

        return (variableKey.Split(':')[0], variableKey);
    }
}