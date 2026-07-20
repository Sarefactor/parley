using Parley.Core.DataAccess.Enums;
using Parley.Workflows.State;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Variables;

public partial class WorkflowVariable : ParleyVariable
{
    [JsonConstructor]
    private WorkflowVariable() { }

    public WorkflowVariable(string name,
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

    public WorkflowVariable(WorkflowVariable workflowVariable, object? value)
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
        => Value = value;

    public void SetObjectVariables(List<ParleyVariable> parleyVariables)
        => ObjectVariables = parleyVariables;

    public int GetListCountZeroIndex(string targetKey, VariableIterationContext? iterationContext = null)
    {
        var isPrimaryVariable = IsPrimaryVariableKey(targetKey, out var objectVariableKey);
        var primaryKey = ParseKey(targetKey);

        if (Value is JsonNode baseNode
            && baseNode.GetPath() == $"$.{primaryKey}"
            && TryGetNode(baseNode,
                          true,
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

    private bool IsPrimaryVariableKey(string targetKey, out string objectVariableKey)
    {
        var split = targetKey.Split(':', 2);
        objectVariableKey = split.Length == 2 ? split[1] : string.Empty;
        return split.Length == 1;
    }

    public string? GetVariableValueAsString(string targetKey, VariableIterationContext iterationContext)
    {
        var isPrimaryVariable = IsPrimaryVariableKey(targetKey, out var objectVariableKey);
        var primaryKey = ParseKey(targetKey);

        if (Value is JsonNode baseNode
            && baseNode.GetPath() == $"$.{primaryKey}"
            && TryGetNode(baseNode,
                          false,
                          out var targetNode,
                          iterationContext.PrimaryContext.IterationContext?.IterationCount,
                          objectVariableKey,
                          iterationContext.SecondaryContext?.IterationContext?.IterationCount))
        {
            return targetNode!.ToString();
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

        // TODECIDE: Throw error if key/s does not match variable name?
    }

    public static (string primaryKey, string? secondaryKey) ParseAndSplitKey(string variableKey)
    {
        if (!variableKey.Contains(':'))
            return (variableKey, null);

        return (variableKey.Split(':')[0], variableKey);
    }

    

    static bool TryGetNode(JsonNode root,
                           bool isTargetNodeAnArray,
                           out JsonNode? result,
                           params object?[] path)
    {
        result = null;
        JsonNode? currentNode = root;

        path = path.Where(part => part is not null
                                  && (part is not string text || !string.IsNullOrWhiteSpace(text)))
                    .ToArray();

        for (var i = 0; i < path.Length; i++)
        {
            var segment = path[i];

            switch (segment)
            {
                case string stringProperty when currentNode is JsonObject objT:

                    if (!objT.TryGetPropertyValue(stringProperty, out currentNode))
                        return false;
                    break;

                case int indexProperty when currentNode is JsonArray array:

                    if (isTargetNodeAnArray
                        && i == path.Length - 1)
                    {
                        result = array;
                        return true;
                    }

                    if (indexProperty < 0 || indexProperty >= array.Count)
                        return false;

                    currentNode = array[indexProperty];
                    break;

                default:
                    return false;
            }
        }

        result = currentNode;
        return true;
    }
}

public class VariableIterationContext
{
    public VariableIterationContext(string primaryKey,
                                   bool primaryIsList,
                                   string? secondaryKey = null,
                                   bool? secondaryIsList = null)
    {
        PrimaryContext = new VariableContext(primaryKey, primaryIsList);

        if (secondaryKey != null && secondaryIsList != null)
            SecondaryContext = new VariableContext(secondaryKey, (bool)secondaryIsList);
    }

    public VariableContext PrimaryContext { get; private set; }
    public VariableContext? SecondaryContext { get; private set; }

    public void SetIterationContext(IterationContext context,
                                    bool isPrimary)
        => (isPrimary ? PrimaryContext : SecondaryContext)?.SetIterationContext(context);

    public bool HasList()
        => PrimaryContext.IsList || SecondaryContext is { IsList: true };
}

public class VariableContext
{
    public VariableContext(string key,
                           bool isList)
    {
        Key = key;
        IsList = isList;
    }

    public string Key { get; set; } = string.Empty;
    public bool IsList { get; private set; }
    public IterationContext? IterationContext { get; private set; }

    public void SetIterationContext(IterationContext context)
        => IterationContext = context;
}