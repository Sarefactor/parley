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

    public int GetListCount(string targetKey)
    {
        if (IsList == false
            || Value == null
            || Value is not JsonArray jsonArray)
            return 0;

        return jsonArray.Count;
    }

    public string? GetVariableValueAsString(string variableKey, WorkflowVariableIterationContext iterationContext)
    {
        // TODO: Lots of mega super duper fun times to be had here.
        return null;
    }

    public WorkflowVariableIterationContext BuildVariableContext(string targetKey)
    {
        var key = targetKey.Contains(':') ? targetKey.Split(':')[0]
                                          : targetKey;

        var secondaryKey = targetKey.Contains(':') ? targetKey
                                                   : null;

        bool? secondaryIsList = secondaryKey != null ? ObjectVariables.First(ov => ov.Name == targetKey.Split(':')[1]).IsList
                                                     : null;

        return new WorkflowVariableIterationContext(key, IsList, secondaryKey, secondaryIsList);

        // TODECIDE: Throw error if key/s does not match variable name?
    }

    public static (string primaryKey, string? secondaryKey) ParseAndSplitKey(string variableKey)
    {
        if (!variableKey.Contains(':'))
            return (variableKey, null);

        return (variableKey.Split(':')[0], variableKey);
    }
}

public class WorkflowVariableIterationContext
{
    public WorkflowVariableIterationContext(string primaryKey,
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