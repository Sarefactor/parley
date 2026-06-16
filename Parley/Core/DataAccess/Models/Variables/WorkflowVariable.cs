using Parley.Core.DataAccess.Enums;
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
}